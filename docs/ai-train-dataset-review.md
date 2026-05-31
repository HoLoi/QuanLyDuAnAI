# AI Train + Dataset Review

Ngày cập nhật: 2026-05-27  
Phạm vi đọc source: `AiDatasetService`, `AiService`, FastAPI (`constants.py`, `schemas.py`, `feature_builder.py`, `validation_service.py`, `model_service.py`, `prediction_service.py`), seed SQL và sample data.

## 1. Train flow đang chạy thực tế

- MVC gọi `AiDatasetService.LayDatasetNguyenNhanHopLeDeTrainAsync`.
- Filter train:
  - `LaDuAnTre=1`
  - `MaDMNguyenNhan` hợp lệ
  - đủ toàn bộ feature trong contract hiện hành
- MVC map sang `AiReasonTrainDatasetItemViewModel` rồi gửi FastAPI `/model/train`.
- FastAPI validate lại, build DataFrame theo `FEATURE_COLUMNS`, train `DecisionTreeClassifier` và trả metadata.

## 2. Dataset contract hiện tại

Contract train/analyze đã mở rộng từ 10 lên 22 feature:
- 10 feature nền tảng cũ.
- 3 feature đề xuất công việc.
- 3 feature đề xuất ngân sách.
- 6 feature báo cáo tiến độ.

Các feature mới đều lấy từ dữ liệu DB hiện có, không đổi workflow nghiệp vụ.

## 3. Rule tổng hợp feature mới

- Lọc soft-delete: `IsDeleted != 1` ở các bảng có cờ.
- Trạng thái dùng biến thể chuẩn từ `TrangThai.GetCommonStatusVariants`.
- Không N+1: dữ liệu được nạp theo cụm và group theo `MaDuAn`.

Chi tiết:
- `SoDeXuatCongViecChoDuyet`, `SoDeXuatCongViecBiTuChoi`: từ `DE_XUAT_CONG_VIEC`.
- `ThoiGianDuyetCongViecTrungBinh`: trung bình ngày `NgayDuyetDeXuatCongViec - NgayDeXuatCongViec` (>=0).
- `SoDeXuatNganSachChoDuyet`, `SoDeXuatNganSachBiTuChoi`: từ `DE_XUAT_NGAN_SACH`.
- `ThoiGianDuyetNganSachTrungBinh`: trung bình ngày `NgayDuyet - NgayDeXuat` (>=0).
- `SoBaoCaoTienDo*`, `SoLanCapNhatTienDo`: từ `TIEN_DO_CONG_VIEC` join `CT_CONG_VIEC -> CONG_VIEC -> DANH_MUC_CONG_VIEC`.
- `TyLeBaoCaoTienDoBiTuChoi`: `(SoBaoCaoTienDoBiTuChoi / SoLanCapNhatTienDo) * 100`, làm tròn 2 chữ số.
- `SoNgayChamCapNhatTienDo`: số ngày từ lần cập nhật tiến độ gần nhất đến `NgayKetThucDuAn`; nếu thiếu ngày kết thúc thì dùng ngày hiện tại.

## 4. Gate chất lượng train

Giữ nguyên ngưỡng:
- `MIN_REASON_TRAIN_ROWS=30`
- `MIN_REASON_CLASS_COUNT=2`
- `MIN_REASON_ROWS_PER_CLASS=5`

Nhưng điều kiện đủ feature nay là đủ 22 feature thay vì 10.

## 5. Đồng bộ FastAPI

Đã đồng bộ các lớp:
- `FEATURE_COLUMNS` trong `constants.py`.
- `DatasetRow` + `PredictFeatureInput` trong `schemas.py`.
- `feature_builder.py` và `validation_service.py` tự động áp dụng contract mới qua `FEATURE_COLUMNS`.
- `prediction_service.py` bổ sung tín hiệu fallback từ nhóm đề xuất/phê duyệt và báo cáo tiến độ.

## 6. Danh mục nguyên nhân

Đã đổi tên chuẩn ở constants/seed theo yêu cầu:
- `Chậm phê duyệt` -> `Quy trình xử lý chậm`
- `Công việc phụ thuộc bị chậm` -> `Phối hợp công việc chưa tốt`
- `Thiếu dữ liệu hoặc tài liệu` -> `Thông tin đầu vào chưa đầy đủ`

## 7. Kết luận

- Pipeline vẫn giữ workflow reason-only hiện tại.
- Feature mở rộng giúp tăng tín hiệu cho nhóm nguyên nhân liên quan phê duyệt, phối hợp và chất lượng cập nhật tiến độ.
- Không thêm nguồn dữ liệu ngoài DB hiện có, không đổi quyền, không đổi boundary MVC/FastAPI.
