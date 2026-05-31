# Workflow hệ thống (AS-IS theo source hiện tại)

## 1. Phạm vi source đã đọc

- `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TrangThaiWorkflowService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChiTietCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TienDoCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs`
- `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`
- `quanlyduan.sql`

## 2. Trạng thái thực tế đang dùng

### 2.1 Dự án (`DU_AN.TrangThaiDuAn`)

- `KhoiTao`
- `DangThucHien`
- `ChoXacNhanHoanThanh`
- `HoanThanh`
- `TamDung`
- Có check thêm trạng thái khóa trong nhiều nơi: `DaHuy`, `LuuTru`

### 2.2 Công việc (`CONG_VIEC.TrangThaiCongViec`)

- `ChuaBatDau`
- `DangThucHien`
- `BiCanCan`
- `ChoXacNhanHoanThanh`
- `HoanThanh`
- `TamDung`
- `DaHuy`

### 2.3 Chi tiết công việc (`CT_CONG_VIEC.TrangThaiCTCV`)

- `ChuaBatDau`
- `DangThucHien`
- `BiCanCan`
- `ChoXacNhanHoanThanh`
- `HoanThanh`
- `TamDung`
- `DaHuy`

### 2.4 Tiến độ (`TIEN_DO_CONG_VIEC.TrangThaiTienDo`)

- `ChoDuyet`
- `DaDuyet`
- `YeuCauBoSung`
- `TuChoi`

### 2.5 Đề xuất

- Đề xuất công việc (`DE_XUAT_CONG_VIEC.TrangThaiCongViecDeXuat`): `ChoDuyet`, `DaDuyet`, `TuChoi`, `DaHuy`
- Đề xuất ngân sách (`DE_XUAT_NGAN_SACH.TrangThaiDeXuat`): `ChoDuyet`, `DaDuyet`, `TuChoi`, `DaHuy`

### 2.6 Đánh giá

- Đánh giá dự án và đánh giá nhân viên dùng nhóm trạng thái: `Nhap`, `ChoDuyet`, `DaDuyet`, `TuChoi`

## 3. Workflow thật sự theo service

### 3.1 Dự án

- `DuAnService.CheckAutoTransitionAsync`: tự chuyển `KhoiTao -> DangThucHien` khi có đủ thành viên, danh mục công việc, công việc.
- `DuAnService.TransitionToDangThucHienAsync`: chuyển thủ công `KhoiTao -> DangThucHien` với điều kiện tương tự auto.
- `DuAnService.RequestCompletionAsync`: `DangThucHien -> ChoXacNhanHoanThanh` khi `ValidateCompletionAsync` đạt.
- `DuAnService.ConfirmCompletionAsync`: `ChoXacNhanHoanThanh -> HoanThanh` và validate lại điều kiện hoàn thành.
- `DuAnService.MoLaiDuAnAsync`: `HoanThanh -> DangThucHien`, bắt buộc lý do, có ghi nhật ký.
- `DuAnService.PauseProjectAsync`: chuyển sang `TamDung`, không cho tạm dừng dự án đã `HoanThanh`.

### 3.2 Công việc

- `TrangThaiWorkflowService.TinhTrangThaiCongViecTheoChiTiet`:
  - không có chi tiết: `ChuaBatDau`
  - tất cả chi tiết hoàn thành: `ChoXacNhanHoanThanh`
  - có chi tiết bị cản: `BiCanCan`
  - còn lại: `DangThucHien`
- `CongViecService.XacNhanHoanThanhCongViecAsync`: `ChoXacNhanHoanThanh -> HoanThanh`, sau đó sync dự án.
- `CongViecService.MoLaiCongViecAsync`: `HoanThanh -> DangThucHien`, bắt buộc lý do, sau đó sync dự án.

### 3.3 Chi tiết công việc

- `ChiTietCongViecService.AddAsync/UpdateAsync/RemoveAsync` cập nhật dữ liệu chi tiết và gọi `ITrangThaiWorkflowService.DongBoChuoiTrangThaiTuCongViecAsync`.
- Trạng thái chi tiết còn có thể được cập nhật qua duyệt tiến độ (`TienDoCongViecService.XuLyBaoCaoTienDoAsync` nhánh `DaDuyet`).

### 3.4 Tiến độ

- `TienDoCongViecService.CapNhatTienDoAsync` tạo báo cáo mới ở `ChoDuyet`, không ghi trực tiếp trạng thái thật.
- `DuyetBaoCaoTienDoAsync` / `YeuCauBoSungBaoCaoTienDoAsync` / `TuChoiBaoCaoTienDoAsync` đi qua `XuLyBaoCaoTienDoAsync`.
- Khi `DaDuyet`: cập nhật `CT_CONG_VIEC.TrangThaiCTCV`, sau đó sync dây chuyền lên `CONG_VIEC` và `DU_AN`.

## 4. Đồng bộ workflow

### 4.1 Service đồng bộ trung tâm

- `ITrangThaiWorkflowService` + `TrangThaiWorkflowService`

### 4.2 Chiều đồng bộ

- `CT_CONG_VIEC -> CONG_VIEC`: `DongBoTrangThaiCongViecTheoChiTietAsync`
- `CONG_VIEC -> DU_AN`: `DongBoTrangThaiDuAnTheoCongViecAsync`
- Gọi chuỗi: `DongBoChuoiTrangThaiTuCongViecAsync`

### 4.3 Rollback và reopen

- Có rollback tự động ở mức dự án:
  - nếu dự án đang `ChoXacNhanHoanThanh` nhưng xuất hiện công việc chưa hoàn thành thì về `DangThucHien`.
- Có reopen thủ công:
  - `CongViecService.MoLaiCongViecAsync`
  - `DuAnService.MoLaiDuAnAsync`
- Không có rollback tự động từ `HoanThanh` ở công việc/dự án trong sync trung tâm vì có early-return khi đã khóa trạng thái.

### 4.4 Auto transition

- Auto `KhoiTao -> DangThucHien` ở dự án.
- Auto `DangThucHien -> ChoXacNhanHoanThanh` ở dự án nếu toàn bộ công việc hoàn thành (qua sync service).
- Công việc không auto sang `HoanThanh`; chỉ auto đến `ChoXacNhanHoanThanh`.

## 5. Workflow AI trong ngữ cảnh hệ thống

- Dữ liệu AI được tổng hợp bởi `AiDatasetService` sang `AI_DATASET`.
- Dự đoán thật (`AiController.Predict` POST) gọi `AiService.DuDoanDuAnAsync` và lưu `AI_KET_QUA`.
- Dự đoán thử (`AiController.TestPredict`) không lưu DB kết quả dự đoán.
- Xác nhận nguyên nhân quản lý lưu vào `AI_NGUYEN_NHAN` qua `AiService.XacNhanNguyenNhanAsync`.

## 6. AS-IS và TO-BE

### 6.1 AS-IS

- Workflow tổng thể đã tách rõ tuyến: báo cáo tiến độ -> duyệt -> cập nhật trạng thái thật -> sync cha.
- `ITrangThaiWorkflowService` đang là đầu mối sync cho chuỗi trạng thái công việc/dự án.

### 6.2 TO-BE (không tự đổi code trong tài liệu này)

- Giữ nguyên terminology và trạng thái hiện tại.
- Khi phát triển tiếp, ưu tiên giữ một nguồn sync trung tâm duy nhất là `ITrangThaiWorkflowService` để tránh phân tán rule.

## 7. Điểm bất hợp lý/rủi ro phát hiện

- `TrangThai.LuuTru` dùng code `Archived` (khác pattern tiếng Việt còn lại), cần lưu ý khi đối chiếu dữ liệu SQL/import cũ.
- Một số thông báo tiếng Việt trong source đang có dấu hiệu lỗi encoding (mojibake), không ảnh hưởng rule nhưng ảnh hưởng UX khi throw exception.
- Dữ liệu seed trong `quanlyduan.sql` có thể chưa phản ánh đầy đủ permission mới nhất (ví dụ quyền AI xác nhận), cần phân biệt với dữ liệu được đảm bảo runtime bởi `KhoiTaoTaiKhoanMacDinh`.
