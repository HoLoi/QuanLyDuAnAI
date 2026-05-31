# MVC - FastAPI Integration Rules (AI)

Ngày cập nhật: 2026-05-27  
Mục tiêu: quy tắc tích hợp và contract vận hành AI reason-only.

## 1. Boundary bắt buộc

- MVC: quyền, workflow, business rule, tổng hợp dữ liệu, ghi SQL.
- FastAPI: compute-only (validate/train/analyze/test), không ghi bảng nghiệp vụ.
- Ground truth train: `AI_NGUYEN_NHAN` (manager xác nhận) được đồng bộ về `AI_DATASET.MaDMNguyenNhan`.
- Không dùng `AI_KET_QUA` làm nhãn train.

## 2. Contract API đang dùng

- `GET /health`
- `POST /dataset/validate`
- `POST /admin/dataset/quality-report`
- `POST /admin/model/train-recommendation`
- `POST /model/train`
- `GET /model/list`
- `GET /admin/model/{modelFile}`
- `POST /admin/model/validate`
- `POST /admin/model/compare`
- `POST /admin/model/set-active`
- `POST /admin/model/reload`
- `DELETE /admin/model/{modelFile}`
- `GET /admin/model/export-metadata/{modelFile}`
- `POST /analyze/delay-reason`
- `POST /admin/model/test-reason`

Legacy alias (chỉ tương thích):
- `POST /predict/project` -> alias `analyze/delay-reason`
- `POST /admin/model/test-predict` -> alias `test-reason`

## 3. Payload strict

- FastAPI dùng `StrictBaseModel(extra="forbid")`.
- Analyze request nhận:
  - `maDuAn`
  - `feature` (đủ toàn bộ feature trong `FEATURE_COLUMNS`)
  - `danhMucNguyenNhan`
  - `reasonConfidenceThreshold`
- Không gửi `LaDuAnTre` lên FastAPI khi analyze.

## 4. Feature contract hiện tại (22 feature)

Nhóm nền tảng:
- `SoNhanVienDuAn`
- `TongSoCongViec`
- `SoCongViecTre`
- `TyLeCongViecTre`
- `ChiPhiDuKien`
- `ChiPhiThucTe`
- `ChenhLechChiPhi`
- `SoLanThayDoiNhanSu`
- `SoLanThayDoiQuanLy`
- `SoNgayTreTienDo`

Nhóm đề xuất công việc:
- `SoDeXuatCongViecChoDuyet`
- `SoDeXuatCongViecBiTuChoi`
- `ThoiGianDuyetCongViecTrungBinh` (đơn vị: ngày)

Nhóm đề xuất ngân sách:
- `SoDeXuatNganSachChoDuyet`
- `SoDeXuatNganSachBiTuChoi`
- `ThoiGianDuyetNganSachTrungBinh` (đơn vị: ngày)

Nhóm tiến độ:
- `SoBaoCaoTienDoChoDuyet`
- `SoBaoCaoTienDoBiTuChoi`
- `SoBaoCaoTienDoYeuCauBoSung`
- `TyLeBaoCaoTienDoBiTuChoi`
- `SoLanCapNhatTienDo`
- `SoNgayChamCapNhatTienDo`

## 5. Rule tổng hợp AI_DATASET

- Chỉ tổng hợp dự án `HoanThanh` hoặc `LuuTru`.
- Không tổng hợp: `KhoiTao`, `DangThucHien`, `TamDung`, `ChoXacNhanHoanThanh`, `DaHuy`.
- `LaDuAnTre`: giữ nguyên rule nghiệp vụ MVC.
- `MaDMNguyenNhan`: lấy từ `AI_NGUYEN_NHAN` manager xác nhận gần nhất, chỉ gán khi `LaDuAnTre=1`.
- `SoNgayChamCapNhatTienDo`: số ngày từ lần cập nhật tiến độ gần nhất đến `NgayKetThucDuAn`; nếu không có `NgayKetThucDuAn` thì dùng ngày hiện tại.

## 6. Rule train

- Chỉ train `NguyenNhan`.
- Chỉ nhận dòng có:
  - `LaDuAnTre=1`
  - `MaDMNguyenNhan > 0`
  - đủ toàn bộ 22 feature
- Gate:
  - `MIN_REASON_TRAIN_ROWS=30`
  - `MIN_REASON_CLASS_COUNT=2`
  - `MIN_REASON_ROWS_PER_CLASS=5`

## 7. Rule analyze/predict

- Chỉ analyze khi `LaDuAnTre=true`.
- `reasonConfidenceThreshold` mặc định `0.6`.
- Nếu confidence thấp hơn ngưỡng hoặc model lỗi/không map danh mục: fallback `RuleFallback`.
- `DoTinCayKetQua` là xác suất/độ chắc chắn cho mẫu hiện tại, không phải độ chính xác tuyệt đối toàn hệ thống.

## 8. Danh mục nguyên nhân chuẩn

Tên chuẩn đang dùng:
- `Thiếu nhân sự`
- `Thay đổi yêu cầu liên tục`
- `Quy trình xử lý chậm`
- `Vượt ngân sách`
- `Rủi ro kỹ thuật`
- `Phối hợp công việc chưa tốt`
- `Thông tin đầu vào chưa đầy đủ`
- `Ước lượng thời gian chưa chính xác`
- `Tiến độ cập nhật không đầy đủ`
- `Khác`
