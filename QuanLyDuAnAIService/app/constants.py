from __future__ import annotations

FEATURE_COLUMNS = [
    "SoNhanVienDuAn",
    "TongSoCongViec",
    "SoCongViecTre",
    "TyLeCongViecTre",
    "ChiPhiDuKien",
    "ChiPhiThucTe",
    "ChenhLechChiPhi",
    "SoLanThayDoiNhanSu",
    "SoLanThayDoiQuanLy",
    "SoNgayTreTienDo",
    "SoDeXuatCongViecChoDuyet",
    "SoDeXuatCongViecBiTuChoi",
    "ThoiGianDuyetCongViecTrungBinh",
    "SoDeXuatNganSachChoDuyet",
    "SoDeXuatNganSachBiTuChoi",
    "ThoiGianDuyetNganSachTrungBinh",
    "SoBaoCaoTienDoChoDuyet",
    "SoBaoCaoTienDoBiTuChoi",
    "SoBaoCaoTienDoYeuCauBoSung",
    "TyLeBaoCaoTienDoBiTuChoi",
    "SoLanCapNhatTienDo",
    "SoNgayChamCapNhatTienDo",
]

DELAY_STATUS_COLUMN = "LaDuAnTre"
REASON_LABEL_COLUMN = "MaDMNguyenNhan"
MODEL_ALGORITHM = "DecisionTreeClassifier"
MODEL_FILE_PREFIX = "decision_tree_reason"
MODEL_EXTENSION = ".joblib"
METADATA_EXTENSION = ".metadata.json"
MODEL_TYPE_REASON = "NguyenNhan"
MODEL_TYPES = (MODEL_TYPE_REASON,)
ACTIVE_MODEL_ALIAS_BY_TYPE = {
    MODEL_TYPE_REASON: "reason_active.joblib",
}
LEGACY_DEFAULT_MODEL_ALIAS = "decision_tree_active.joblib"
REASON_SOURCE_RULE_FALLBACK = "RuleFallback"
REASON_SOURCE_MODEL = "NguyenNhanModel"

SERVICE_VERSION = "1.1.0"

EVENT_TRAIN = "train"
EVENT_ANALYZE = "analyze"
EVENT_SYSTEM = "system"

REASON_STAFF_SHORTAGE = "Thiếu nhân sự"
REASON_REQUIREMENT_CHANGE = "Thay đổi yêu cầu liên tục"
REASON_APPROVAL_DELAY = "Quy trình xử lý chậm"
REASON_COST_OVERRUN = "Vượt ngân sách"
REASON_TECHNICAL_RISK = "Rủi ro kỹ thuật"
REASON_DEPENDENCY_DELAY = "Phối hợp công việc chưa tốt"
REASON_DOCS_DATA_GAP = "Thông tin đầu vào chưa đầy đủ"
REASON_ESTIMATION_GAP = "Ước lượng thời gian chưa chính xác"
REASON_PROGRESS_UPDATE_GAP = "Tiến độ cập nhật không đầy đủ"
REASON_OTHER = "Khác"

DEFAULT_LONG_DELAY_DAYS_THRESHOLD = 14
