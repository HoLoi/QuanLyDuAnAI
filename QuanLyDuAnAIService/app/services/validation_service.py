from __future__ import annotations

from app.config import settings
from app.constants import DELAY_STATUS_COLUMN, FEATURE_COLUMNS, REASON_LABEL_COLUMN
from app.ml.feature_builder import get_missing_features, rows_to_dicts
from app.schemas import (
    DatasetQualityReportResponse,
    DatasetValidateRequest,
    DatasetValidateResponse,
    TrainRecommendationResponse,
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
        valid_rows = 0
        missing_rows = 0
        delayed_rows = 0
        rows_with_reason = 0
        warnings: list[str] = []

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
                continue

            if is_delay and has_reason:
                valid_rows += 1

        if valid_rows < settings.min_reason_train_rows:
            warnings.append(
                f"Số dòng train hợp lệ ({valid_rows}) nhỏ hơn MIN_REASON_TRAIN_ROWS ({settings.min_reason_train_rows})."
            )

        return DatasetValidateResponse(
            tongSoDong=total,
            soDongHopLe=valid_rows,
            soDongThieuDuLieu=missing_rows,
            soDongDuAnTre=delayed_rows,
            soDongCoNguyenNhan=rows_with_reason,
            canhBaoDataset=warnings,
        )

    def quality_report(self, request: DatasetValidateRequest) -> DatasetQualityReportResponse:
        records = rows_to_dicts(request.dataset)
        total = len(records)
        if total == 0:
            return DatasetQualityReportResponse(
                tongSoDong=0,
                soDongHopLe=0,
                soDongThieuLabel=0,
                soDongThieuFeature=0,
                tyLeNullTungFeature={feature: 0.0 for feature in FEATURE_COLUMNS},
                tyLeClass={},
                duTrainHayKhong=False,
                warningMessages=["Dataset rỗng."],
                blockingErrors=["Không có dữ liệu train."],
            )

        missing_label = 0
        missing_feature = 0
        valid_rows = 0
        null_counts = {feature: 0 for feature in FEATURE_COLUMNS}
        class_counts: dict[int, int] = {}

        for record in records:
            missing = get_missing_features(record)
            for feature in FEATURE_COLUMNS:
                if record.get(feature) is None:
                    null_counts[feature] += 1

            is_delay = self._is_delay_project(record.get(DELAY_STATUS_COLUMN))
            has_reason = self._valid_reason(record.get(REASON_LABEL_COLUMN))

            if not is_delay or not has_reason:
                missing_label += 1

            if missing:
                missing_feature += 1

            if not missing and is_delay and has_reason:
                valid_rows += 1
                reason_code = int(record.get(REASON_LABEL_COLUMN))
                class_counts[reason_code] = class_counts.get(reason_code, 0) + 1

        class_ratio = {
            str(reason): round(count / valid_rows, 4) for reason, count in class_counts.items()
        } if valid_rows > 0 else {}
        null_ratios = {key: round(value / total, 4) for key, value in null_counts.items()}

        warning_messages: list[str] = []
        blocking_errors: list[str] = []

        if valid_rows < settings.min_reason_train_rows:
            blocking_errors.append(
                f"Số dòng train hợp lệ ({valid_rows}) nhỏ hơn MIN_REASON_TRAIN_ROWS ({settings.min_reason_train_rows})."
            )

        if len(class_counts) < settings.min_reason_class_count:
            blocking_errors.append(
                f"Dataset cần tối thiểu {settings.min_reason_class_count} nguyên nhân khác nhau."
            )
        elif min(class_counts.values()) < settings.min_reason_rows_per_class:
            blocking_errors.append(
                "Dataset mất cân bằng: mỗi nguyên nhân cần ít nhất "
                f"{settings.min_reason_rows_per_class} dòng."
            )
            warning_messages.append("Dataset đang mất cân bằng nguyên nhân.")

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
