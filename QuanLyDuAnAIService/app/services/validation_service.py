from __future__ import annotations

from app.constants import DELAY_STATUS_COLUMN, FEATURE_COLUMNS, REASON_LABEL_COLUMN
from app.ml.feature_builder import get_missing_features, rows_to_dicts
from app.schemas import (
    DatasetQualityReportResponse,
    DatasetValidateRequest,
    DatasetValidateResponse,
    TrainRecommendationResponse,
)
from app.services.reason_dataset_policy import (
    build_blocking_errors,
    classify_reason_dataset,
    null_feature_ratios,
)


class ValidationService:
    @staticmethod
    def _is_delay_project(value: object) -> bool:
        return value in (1, True)

    @staticmethod
    def _valid_reason(value: object) -> bool:
        try:
            return int(value) > 0
        except Exception:
            return False

    def validate_dataset(self, request: DatasetValidateRequest) -> DatasetValidateResponse:
        records = rows_to_dicts(request.dataset)
        total = len(records)
        missing_rows = 0
        delayed_rows = 0
        rows_with_reason = 0

        for record in records:
            missing = get_missing_features(record)
            is_delay = self._is_delay_project(record.get(DELAY_STATUS_COLUMN))
            has_reason = self._valid_reason(record.get(REASON_LABEL_COLUMN))

            if is_delay:
                delayed_rows += 1
            if has_reason:
                rows_with_reason += 1

            if missing:
                missing_rows += 1
        classification = classify_reason_dataset(request.dataset)
        warnings = classification.warnings + build_blocking_errors(classification)

        return DatasetValidateResponse(
            tongSoDong=total,
            soDongHopLe=classification.used_rows,
            soDongThieuDuLieu=missing_rows,
            soDongDuAnTre=delayed_rows,
            soDongCoNguyenNhan=rows_with_reason,
            validRowsBeforeClassFilter=classification.valid_rows_before_class_filter,
            usedRows=classification.used_rows,
            accumulatingRows=classification.accumulating_rows,
            eligibleClassCount=classification.eligible_class_count,
            accumulatingClassCount=classification.accumulating_class_count,
            canhBaoDataset=warnings,
        )

    def quality_report(self, request: DatasetValidateRequest) -> DatasetQualityReportResponse:
        records = rows_to_dicts(request.dataset)
        total = len(records)
        classification = classify_reason_dataset(request.dataset)
        if total == 0:
            return DatasetQualityReportResponse(
                tongSoDong=0,
                soDongHopLe=0,
                soDongThieuLabel=0,
                soDongThieuFeature=0,
                tyLeNullTungFeature={feature: 0.0 for feature in FEATURE_COLUMNS},
                tyLeClass={},
                validRowsBeforeClassFilter=0,
                usedRows=0,
                accumulatingRows=0,
                eligibleClassCount=0,
                accumulatingClassCount=0,
                usedClassDistribution={},
                droppedClassDistribution={},
                duTrainHayKhong=False,
                warningMessages=["Dataset rỗng."],
                blockingErrors=["Không có dữ liệu train."],
            )

        missing_label = 0
        missing_feature = 0

        for record in records:
            missing = get_missing_features(record)

            is_delay = self._is_delay_project(record.get(DELAY_STATUS_COLUMN))
            has_reason = self._valid_reason(record.get(REASON_LABEL_COLUMN))

            if not is_delay or not has_reason:
                missing_label += 1

            if missing:
                missing_feature += 1
        valid_rows = classification.used_rows
        class_ratio = {
            reason: round(count / valid_rows, 4)
            for reason, count in classification.used_class_distribution.items()
        } if valid_rows > 0 else {}
        null_ratios = null_feature_ratios(request.dataset)

        warning_messages: list[str] = list(classification.warnings)
        blocking_errors: list[str] = build_blocking_errors(classification)

        if missing_feature > 0:
            warning_messages.append("Dataset có dòng thiếu feature.")
        if missing_label > 0:
            warning_messages.append("Dataset có dòng không đủ điều kiện label nguyên nhân (LaDuAnTre=1, MaDMNguyenNhan hợp lệ).")

        is_trainable = len(blocking_errors) == 0

        return DatasetQualityReportResponse(
            tongSoDong=total,
            soDongHopLe=valid_rows,
            soDongThieuLabel=missing_label,
            soDongThieuFeature=missing_feature,
            tyLeNullTungFeature=null_ratios,
            tyLeClass=class_ratio,
            validRowsBeforeClassFilter=classification.valid_rows_before_class_filter,
            usedRows=classification.used_rows,
            accumulatingRows=classification.accumulating_rows,
            eligibleClassCount=classification.eligible_class_count,
            accumulatingClassCount=classification.accumulating_class_count,
            usedClassDistribution=classification.used_class_distribution,
            droppedClassDistribution=classification.dropped_class_distribution,
            duTrainHayKhong=is_trainable,
            warningMessages=warning_messages,
            blockingErrors=blocking_errors,
        )

    def train_recommendation(self, request: DatasetValidateRequest) -> TrainRecommendationResponse:
        report = self.quality_report(request)
        should_train = report.duTrainHayKhong
        imbalance_warning = any("mất cân bằng" in msg.lower() for msg in report.warningMessages + report.blockingErrors)
        missing_warning = any("thiếu" in msg.lower() for msg in report.warningMessages + report.blockingErrors)

        reason = "Dataset đạt điều kiện cơ bản để train." if should_train else "Dataset chưa đạt điều kiện train."

        messages = []
        messages.extend(report.warningMessages)
        messages.extend(report.blockingErrors)
        if not messages:
            messages.append("Không có cảnh báo.")

        return TrainRecommendationResponse(
            nenTrain=should_train,
            lyDo=reason,
            canhBaoImbalance=imbalance_warning,
            canhBaoThieuDuLieu=missing_warning,
            recommendationMessages=messages,
        )
