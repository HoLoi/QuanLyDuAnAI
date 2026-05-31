# AI Hệ thống phân tích (Source Of Truth)

Ngày cập nhật: 2026-05-27  
Phạm vi source đã đọc: MVC (`QuanLyDuAn/QuanLyDuAn`), FastAPI (`QuanLyDuAnAIService/app`), seed SQL (`seed-demo-data-simple.sql`, `seed-demo-data-ai-expanded.sql`).

## 1. Kiến trúc hiện tại

- MVC là system-of-record cho `AI_DATASET`, `AI_KET_QUA`, `AI_NGUYEN_NHAN`, `AI_MODEL`.
- FastAPI là compute-only, không ghi SQL nghiệp vụ.
- Pipeline chính là reason-only (`NguyenNhan`), không dùng luồng delay prediction làm workflow chính.

## 2. Luồng dữ liệu AI

1. Dữ liệu nghiệp vụ -> `AiDatasetService` tổng hợp vào `AI_DATASET`.
2. `LaDuAnTre` được quyết định bởi rule MVC.
3. Nếu `LaDuAnTre=1`, manager xác nhận nguyên nhân -> `AI_NGUYEN_NHAN`.
4. Tổng hợp lần sau đồng bộ `MaDMNguyenNhan` từ xác nhận manager vào `AI_DATASET`.
5. Train gửi dataset hợp lệ sang FastAPI (`/model/train`).
6. Analyze gửi feature + danh mục nguyên nhân sang FastAPI (`/analyze/delay-reason`).
7. Kết quả phân tích lưu ở `AI_KET_QUA`; xác nhận quản lý vẫn nằm ở `AI_NGUYEN_NHAN`.

## 3. Contract feature hiện tại

`AI_DATASET` và schema FastAPI đã đồng bộ 22 feature theo 4 nhóm:

- Nền tảng tiến độ/chi phí/nhân sự: 10 feature gốc.
- Đề xuất công việc: `SoDeXuatCongViecChoDuyet`, `SoDeXuatCongViecBiTuChoi`, `ThoiGianDuyetCongViecTrungBinh`.
- Đề xuất ngân sách: `SoDeXuatNganSachChoDuyet`, `SoDeXuatNganSachBiTuChoi`, `ThoiGianDuyetNganSachTrungBinh`.
- Tiến độ báo cáo: `SoBaoCaoTienDoChoDuyet`, `SoBaoCaoTienDoBiTuChoi`, `SoBaoCaoTienDoYeuCauBoSung`, `TyLeBaoCaoTienDoBiTuChoi`, `SoLanCapNhatTienDo`, `SoNgayChamCapNhatTienDo`.

Ghi chú rule:
- `ThoiGianDuyet*TrungBinh`: đơn vị ngày.
- `TyLeBaoCaoTienDoBiTuChoi`: phần trăm, làm tròn 2 chữ số.
- `SoNgayChamCapNhatTienDo`: từ lần cập nhật tiến độ gần nhất đến ngày kết thúc dự án; nếu không có ngày kết thúc thì dùng ngày hiện tại.

## 4. Gate train

- Chỉ train khi:
  - `LaDuAnTre=1`
  - `MaDMNguyenNhan` hợp lệ (>0)
  - đủ toàn bộ 22 feature
- Ngưỡng chất lượng:
  - tối thiểu 30 dòng
  - tối thiểu 2 lớp nguyên nhân
  - tối thiểu 5 dòng mỗi lớp

## 5. Rule analyze

- Không analyze nguyên nhân nếu `LaDuAnTre != true`.
- Threshold confidence mặc định 0.6.
- Nếu confidence thấp/lỗi model/không map được danh mục -> fallback theo luật.
- UI phải giải thích rõ: độ tin cậy là xác suất cho mẫu hiện tại, không phải accuracy tuyệt đối.

## 6. Danh mục nguyên nhân chuẩn

Đã chuẩn hóa tên nguyên nhân:
- `Quy trình xử lý chậm` (thay `Chậm phê duyệt`)
- `Phối hợp công việc chưa tốt` (thay `Công việc phụ thuộc bị chậm`)
- `Thông tin đầu vào chưa đầy đủ` (thay `Thiếu dữ liệu hoặc tài liệu`)

Các tên này cần đồng bộ giữa:
- `DM_NGUYEN_NHAN`
- seed SQL
- FastAPI constants + keyword mapping
- UI hiển thị danh mục
