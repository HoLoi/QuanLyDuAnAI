from __future__ import annotations

import unicodedata

from app.config import settings
from app.constants import (
    DEFAULT_LONG_DELAY_DAYS_THRESHOLD,
    REASON_APPROVAL_DELAY,
    REASON_COST_OVERRUN,
    REASON_DEPENDENCY_DELAY,
    REASON_DOCS_DATA_GAP,
    REASON_ESTIMATION_GAP,
    REASON_OTHER,
    REASON_PROGRESS_UPDATE_GAP,
    REASON_REQUIREMENT_CHANGE,
    REASON_SOURCE_MODEL,
    REASON_SOURCE_RULE_FALLBACK,
    REASON_STAFF_SHORTAGE,
    REASON_TECHNICAL_RISK,
)
from app.ml.feature_builder import build_feature_frame
from app.schemas import (
    DanhMucNguyenNhanItem,
    PredictFeatureInput,
    PredictProjectRequest,
    PredictProjectResponse,
    RelatedReasonItem,
    TestPredictResponse,
)


def _feature_to_dict(feature: PredictFeatureInput) -> dict:
    return feature.model_dump()


def _normalize_text(text: str) -> str:
    lowered = text.strip().lower()
    no_marks = unicodedata.normalize("NFD", lowered)
    no_marks = "".join(ch for ch in no_marks if unicodedata.category(ch) != "Mn")
    return unicodedata.normalize("NFC", no_marks)


def _normalize_delay_ratio(value: float) -> float:
    ratio = float(value or 0.0)
    if ratio > 1.0:
        ratio = ratio / 100.0
    return max(0.0, min(ratio, 1.0))


class PredictionService:
    @staticmethod
    def _map_muc_phu_hop(score: float) -> str:
        if score >= 0.75:
            return "Cao"
        if score >= 0.5:
            return "Trung bình"
        return "Thấp"

    @staticmethod
    def _get_ranked_probabilities(model, frame) -> list[tuple[int, float]]:
        if not hasattr(model, "predict_proba"):
            return []

        probabilities = model.predict_proba(frame)[0]
        classes = getattr(model, "classes_", [])
        ranked: list[tuple[int, float]] = []

        for idx, reason_class in enumerate(classes):
            try:
                reason_code = int(reason_class)
                score = float(probabilities[idx])
            except Exception:
                continue
            ranked.append((reason_code, score))

        ranked.sort(key=lambda item: item[1], reverse=True)
        return ranked

    @staticmethod
    def _is_cost_reason_name(reason_name: str | None) -> bool:
        if not reason_name:
            return False
        normalized = _normalize_text(reason_name).replace(" ", "")
        return "vuotngansach" in normalized or "ngansach" in normalized or "chiphi" in normalized

    @staticmethod
    def _is_cost_overrun_consistent(feature_data: dict) -> bool:
        budget_plan = float(feature_data.get("ChiPhiDuKien", 0.0) or 0.0)
        budget_actual = float(feature_data.get("ChiPhiThucTe", 0.0) or 0.0)
        budget_delta = float(feature_data.get("ChenhLechChiPhi", budget_actual - budget_plan) or 0.0)

        if budget_actual <= budget_plan or budget_delta <= 0:
            return False
        if budget_plan <= 0:
            return budget_delta > 0

        overrun_ratio = (budget_actual - budget_plan) / budget_plan
        return overrun_ratio >= settings.high_cost_overrun_threshold

    def _map_reason_to_catalog(
        self,
        reason_name: str,
        danh_muc_nguyen_nhan: list[DanhMucNguyenNhanItem] | None,
    ) -> tuple[int | None, str]:
        if not danh_muc_nguyen_nhan:
            return None, reason_name

        normalized_reason = _normalize_text(reason_name)
        keyword_map: dict[str, list[str]] = {
            _normalize_text(REASON_STAFF_SHORTAGE): ["thieu nhan su", "nhan su"],
            _normalize_text(REASON_REQUIREMENT_CHANGE): ["thay doi yeu cau", "yeu cau"],
            _normalize_text(REASON_APPROVAL_DELAY): ["phe duyet", "duyet", "quy trinh", "xu ly cham"],
            _normalize_text(REASON_COST_OVERRUN): ["vuot ngan sach", "ngan sach", "chi phi"],
            _normalize_text(REASON_TECHNICAL_RISK): ["ky thuat", "rui ro"],
            _normalize_text(REASON_DEPENDENCY_DELAY): ["phu hop cong viec", "phoi hop cong viec", "phu thuoc", "cong viec tre"],
            _normalize_text(REASON_DOCS_DATA_GAP): ["thong tin dau vao", "thieu du lieu", "thieu tai lieu", "tai lieu"],
            _normalize_text(REASON_ESTIMATION_GAP): ["uoc luong", "thoi gian chua chinh xac"],
            _normalize_text(REASON_PROGRESS_UPDATE_GAP): ["cap nhat tien do", "tien do cap nhat"],
            _normalize_text(REASON_OTHER): ["khac"],
        }
        keywords = keyword_map.get(normalized_reason, [normalized_reason])

        for item in danh_muc_nguyen_nhan:
            candidate = _normalize_text(item.tenNguyenNhan)
            if candidate == normalized_reason or any(keyword in candidate for keyword in keywords):
                try:
                    return int(item.maDMNguyenNhan), item.tenNguyenNhan
                except Exception:
                    return None, item.tenNguyenNhan

        return None, reason_name

    def _map_reason_code_to_catalog(
        self,
        reason_code: int,
        danh_muc_nguyen_nhan: list[DanhMucNguyenNhanItem] | None,
    ) -> tuple[int | None, str] | None:
        if not danh_muc_nguyen_nhan:
            return None
        for item in danh_muc_nguyen_nhan:
            try:
                if int(item.maDMNguyenNhan) == int(reason_code):
                    return int(item.maDMNguyenNhan), item.tenNguyenNhan
            except Exception:
                continue
        return None

    def suggest_reason(
        self,
        feature_data: dict,
        danh_muc_nguyen_nhan: list[DanhMucNguyenNhanItem] | None,
        allow_cost_reason: bool = True,
    ) -> tuple[int | None, str, str]:
        task_delay_ratio = _normalize_delay_ratio(feature_data.get("TyLeCongViecTre", 0.0) or 0.0)
        overdue_tasks = float(feature_data.get("SoCongViecTre", 0.0) or 0.0)
        staff_changes = float(feature_data.get("SoLanThayDoiNhanSu", 0.0) or 0.0)
        manager_changes = float(feature_data.get("SoLanThayDoiQuanLy", 0.0) or 0.0)
        delay_days = float(feature_data.get("SoNgayTreTienDo", 0.0) or 0.0)
        pending_task_proposals = float(feature_data.get("SoDeXuatCongViecChoDuyet", 0.0) or 0.0)
        rejected_task_proposals = float(feature_data.get("SoDeXuatCongViecBiTuChoi", 0.0) or 0.0)
        avg_task_approval_days = float(feature_data.get("ThoiGianDuyetCongViecTrungBinh", 0.0) or 0.0)
        pending_budget_proposals = float(feature_data.get("SoDeXuatNganSachChoDuyet", 0.0) or 0.0)
        rejected_budget_proposals = float(feature_data.get("SoDeXuatNganSachBiTuChoi", 0.0) or 0.0)
        avg_budget_approval_days = float(feature_data.get("ThoiGianDuyetNganSachTrungBinh", 0.0) or 0.0)
        rejected_progress_ratio = float(feature_data.get("TyLeBaoCaoTienDoBiTuChoi", 0.0) or 0.0)
        requested_progress_reports = float(feature_data.get("SoBaoCaoTienDoYeuCauBoSung", 0.0) or 0.0)
        late_progress_update_days = float(feature_data.get("SoNgayChamCapNhatTienDo", 0.0) or 0.0)

        if allow_cost_reason and self._is_cost_overrun_consistent(feature_data):
            reason_name = REASON_COST_OVERRUN
            explanation = "Chi phí thực tế đang vượt ngưỡng ngân sách kế hoạch."
        elif staff_changes >= settings.high_staff_change_threshold:
            reason_name = REASON_STAFF_SHORTAGE
            explanation = "Số lần thay đổi nhân sự cao, dễ gây đứt gãy tiến độ."
        elif (
            pending_task_proposals + pending_budget_proposals >= 3
            or rejected_task_proposals + rejected_budget_proposals >= 2
            or avg_task_approval_days >= 5
            or avg_budget_approval_days >= 5
        ):
            reason_name = REASON_APPROVAL_DELAY
            explanation = "Các chỉ số phê duyệt đề xuất cho thấy quy trình xử lý bị chậm."
        elif manager_changes >= settings.high_manager_change_threshold:
            reason_name = REASON_APPROVAL_DELAY
            explanation = "Có thay đổi quản lý/phê duyệt, làm chậm quyết định dự án."
        elif (
            rejected_progress_ratio >= 25
            or requested_progress_reports >= 2
            or late_progress_update_days >= 7
        ):
            reason_name = REASON_PROGRESS_UPDATE_GAP
            explanation = "Báo cáo tiến độ bị từ chối/bổ sung nhiều hoặc cập nhật chậm."
        elif task_delay_ratio >= 0.5 or overdue_tasks >= 5:
            reason_name = REASON_DEPENDENCY_DELAY
            explanation = "Tỷ lệ công việc trễ cao, nghiêng về nhóm phụ thuộc bị chậm."
        elif delay_days >= DEFAULT_LONG_DELAY_DAYS_THRESHOLD:
            reason_name = REASON_ESTIMATION_GAP
            explanation = "Số ngày trễ tiến độ cao, khả năng ước lượng thời gian chưa chính xác."
        elif task_delay_ratio >= settings.high_delay_ratio_threshold:
            reason_name = REASON_PROGRESS_UPDATE_GAP
            explanation = "Có dấu hiệu cập nhật tiến độ chưa đầy đủ so với thực tế."
        elif overdue_tasks > 0:
            reason_name = REASON_REQUIREMENT_CHANGE
            explanation = "Xuất hiện việc trễ cục bộ, có thể liên quan thay đổi yêu cầu trong quá trình triển khai."
        else:
            reason_name = REASON_OTHER
            explanation = "Dữ liệu chưa đủ tín hiệu rõ ràng, gợi ý nguyên nhân tổng quát là Khác."

        reason_code, reason_display_name = self._map_reason_to_catalog(reason_name, danh_muc_nguyen_nhan)
        return reason_code, reason_display_name, explanation

    def build_explanation(
        self,
        feature_data: dict,
        feature_importance: dict[str, float] | None,
        confidence: float,
    ) -> str:
        top_features: list[tuple[str, float]] = []
        if feature_importance:
            top_features = sorted(feature_importance.items(), key=lambda item: item[1], reverse=True)[:3]

        triggered_signals: list[str] = []
        if _normalize_delay_ratio(feature_data.get("TyLeCongViecTre", 0.0) or 0.0) >= settings.high_delay_ratio_threshold:
            triggered_signals.append("Tỷ lệ công việc trễ cao")
        if self._is_cost_overrun_consistent(feature_data):
            triggered_signals.append("Chi phí thực tế vượt dự kiến")
        if float(feature_data.get("SoLanThayDoiNhanSu", 0.0) or 0.0) >= settings.high_staff_change_threshold:
            triggered_signals.append("Biến động nhân sự lớn")
        if float(feature_data.get("SoNgayTreTienDo", 0.0) or 0.0) >= DEFAULT_LONG_DELAY_DAYS_THRESHOLD:
            triggered_signals.append("Số ngày trễ tiến độ cao")
        if float(feature_data.get("SoDeXuatCongViecChoDuyet", 0.0) or 0.0) + float(feature_data.get("SoDeXuatNganSachChoDuyet", 0.0) or 0.0) >= 3:
            triggered_signals.append("Nhiều đề xuất đang chờ duyệt")
        if float(feature_data.get("TyLeBaoCaoTienDoBiTuChoi", 0.0) or 0.0) >= 25:
            triggered_signals.append("Tỷ lệ báo cáo tiến độ bị từ chối cao")

        parts: list[str] = []
        if triggered_signals:
            parts.append("Tín hiệu nổi bật: " + ", ".join(triggered_signals[:3]) + ".")
        if top_features:
            feature_text = ", ".join(f"{name} ({score:.2f})" for name, score in top_features if score > 0)
            if feature_text:
                parts.append(f"Đặc trưng ảnh hưởng chính: {feature_text}.")
        parts.append(f"Độ tin cậy mô hình: {confidence:.2f}.")

        return " ".join(parts)

    @staticmethod
    def _predict_label_with_confidence(model, frame) -> tuple[int, float, list[tuple[int, float]]]:
        prediction = int(model.predict(frame)[0])
        confidence = 1.0
        ranked_probabilities: list[tuple[int, float]] = []
        if hasattr(model, "predict_proba"):
            ranked_probabilities = PredictionService._get_ranked_probabilities(model, frame)
            if ranked_probabilities:
                confidence = float(ranked_probabilities[0][1])
            else:
                proba = model.predict_proba(frame)[0]
                confidence = float(max(proba))
        return prediction, confidence, ranked_probabilities

    def _build_related_reasons(
        self,
        ranked_probabilities: list[tuple[int, float]],
        main_reason_code: int | None,
        danh_muc_nguyen_nhan: list[DanhMucNguyenNhanItem] | None,
    ) -> list[RelatedReasonItem]:
        if not ranked_probabilities:
            return []

        related_reasons: list[RelatedReasonItem] = []
        for reason_code, score in ranked_probabilities:
            if main_reason_code is not None and int(reason_code) == int(main_reason_code):
                continue

            mapped = self._map_reason_code_to_catalog(reason_code, danh_muc_nguyen_nhan)
            if mapped:
                mapped_code, mapped_name = mapped
            else:
                mapped_code, mapped_name = int(reason_code), f"Nguyên nhân {reason_code}"

            related_reasons.append(
                RelatedReasonItem(
                    maDMNguyenNhan=mapped_code,
                    tenNguyenNhan=mapped_name,
                    score=round(float(score), 4),
                    mucPhuHop=self._map_muc_phu_hop(float(score)),
                )
            )
            if len(related_reasons) >= 3:
                break

        return related_reasons

    def _predict_reason_by_model(
        self,
        reason_model,
        frame,
        danh_muc_nguyen_nhan: list[DanhMucNguyenNhanItem] | None,
    ) -> tuple[int | None, str, float, str, list[RelatedReasonItem]] | None:
        reason_pred, reason_conf, ranked_probabilities = self._predict_label_with_confidence(reason_model, frame)
        mapped = self._map_reason_code_to_catalog(reason_pred, danh_muc_nguyen_nhan)
        related_reasons = self._build_related_reasons(ranked_probabilities, reason_pred, danh_muc_nguyen_nhan)
        muc_phu_hop = self._map_muc_phu_hop(float(reason_conf))
        if mapped:
            code, name = mapped
            return code, name, reason_conf, muc_phu_hop, related_reasons
        if danh_muc_nguyen_nhan is None:
            return int(reason_pred), f"Nguyên nhân {reason_pred}", reason_conf, muc_phu_hop, related_reasons
        return None

    def predict_project(
        self,
        request: PredictProjectRequest,
        reason_model=None,
        reason_model_name: str | None = None,
        reason_feature_importance: dict[str, float] | None = None,
    ) -> PredictProjectResponse:
        feature_data = _feature_to_dict(request.feature)
        frame = build_feature_frame([feature_data], strict=True)

        threshold = request.reasonConfidenceThreshold
        if threshold is None:
            threshold = settings.reason_confidence_threshold

        reason_source = REASON_SOURCE_RULE_FALLBACK
        reason_code: int | None = None
        reason_name: str | None = None
        reason_model_used: str | None = None
        warning_message: str | None = None
        model_confidence = 0.0
        muc_phu_hop: str | None = None
        danh_sach_nguyen_nhan_lien_quan: list[RelatedReasonItem] = []
        noi_dung_phan_tich: str | None = None

        if reason_model is not None and reason_model_name:
            try:
                reason_from_model = self._predict_reason_by_model(
                    reason_model=reason_model,
                    frame=frame,
                    danh_muc_nguyen_nhan=request.danhMucNguyenNhan,
                )
                if reason_from_model is not None:
                    reason_code_model, reason_name_model, reason_conf, reason_muc_phu_hop, related_reasons = reason_from_model
                    reason_model_used = reason_model_name
                    model_confidence = float(reason_conf)
                    muc_phu_hop = reason_muc_phu_hop
                    danh_sach_nguyen_nhan_lien_quan = related_reasons
                    if reason_conf >= float(threshold):
                        reason_source = REASON_SOURCE_MODEL
                        reason_code = reason_code_model
                        reason_name = reason_name_model
                        noi_dung_phan_tich = self.build_explanation(feature_data, reason_feature_importance, reason_conf)
                    else:
                        warning_message = "Độ tin cậy mô hình thấp, hệ thống chuyển sang gợi ý theo luật."
                else:
                    warning_message = "Model nguyên nhân không map được danh mục nguyên nhân."
            except Exception:
                warning_message = "Model nguyên nhân gặp lỗi, đã fallback sang luật gợi ý."

        if reason_source != REASON_SOURCE_MODEL:
            reason_code, reason_name, fallback_explanation = self.suggest_reason(feature_data, request.danhMucNguyenNhan)
            noi_dung_phan_tich = fallback_explanation
            if warning_message is None and reason_model is None:
                warning_message = "Chưa có model nguyên nhân hoạt động, hệ thống đang dùng luật gợi ý."
        elif self._is_cost_reason_name(reason_name) and not self._is_cost_overrun_consistent(feature_data):
            reason_code, reason_name, fallback_explanation = self.suggest_reason(
                feature_data,
                request.danhMucNguyenNhan,
                allow_cost_reason=False,
            )
            reason_source = REASON_SOURCE_RULE_FALLBACK
            warning_message = "Kết quả AI không phù hợp với dữ liệu hiện tại, hệ thống đã chuyển sang luật gợi ý."
            noi_dung_phan_tich = fallback_explanation

        final_confidence = model_confidence if model_confidence > 0 else 0.5
        if muc_phu_hop is None:
            muc_phu_hop = self._map_muc_phu_hop(final_confidence)

        return PredictProjectResponse(
            maDMNguyenNhanDuDoan=reason_code,
            tenNguyenNhanDuDoan=reason_name,
            doTinCayKetQua=round(float(final_confidence), 4),
            mucPhuHop=muc_phu_hop,
            danhSachNguyenNhanLienQuan=danh_sach_nguyen_nhan_lien_quan or None,
            reasonSource=reason_source,
            modelNguyenNhanUsed=reason_model_used,
            canhBaoNguyenNhan=warning_message,
            noiDungPhanTich=noi_dung_phan_tich,
        )

    def test_predict(
        self,
        feature: PredictFeatureInput,
        model,
        model_name: str,
        danh_muc_nguyen_nhan: list[DanhMucNguyenNhanItem] | None = None,
        feature_importance: dict[str, float] | None = None,
    ) -> TestPredictResponse:
        feature_data = feature.model_dump()
        frame = build_feature_frame([feature_data], strict=True)

        prediction, confidence, _ = self._predict_label_with_confidence(model, frame)
        mapped = self._map_reason_code_to_catalog(prediction, danh_muc_nguyen_nhan)
        if mapped:
            reason_code, reason_name = mapped
        else:
            reason_code, reason_name, _ = self.suggest_reason(feature_data, danh_muc_nguyen_nhan)
        reason_source = REASON_SOURCE_MODEL

        if self._is_cost_reason_name(reason_name) and not self._is_cost_overrun_consistent(feature_data):
            reason_code, reason_name, fallback_explanation = self.suggest_reason(
                feature_data,
                danh_muc_nguyen_nhan,
                allow_cost_reason=False,
            )
            reason_source = REASON_SOURCE_RULE_FALLBACK
            explanation = "Kết quả AI không phù hợp với dữ liệu hiện tại. " + fallback_explanation
        else:
            explanation = self.build_explanation(feature_data, feature_importance, confidence)

        return TestPredictResponse(
            confidence=round(confidence, 4),
            suggestedReasonCode=reason_code,
            suggestedReason=reason_name,
            explanation=explanation,
            modelUsed=model_name,
            reasonSource=reason_source,
        )
