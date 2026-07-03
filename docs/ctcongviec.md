# Phân tích workflow chi tiết công việc và đồng bộ trạng thái

## 1. Tổng quan vấn đề

Tài liệu này chỉ đọc source và phân tích AS-IS của workflow `CT_CONG_VIEC` trong dự án ASP.NET Core MVC hiện tại. Không sửa code, không sửa database, không tạo migration, không seed dữ liệu, không đổi giao diện/CSS/font.

Kết luận ngắn: `CT_CONG_VIEC` hiện có các trạng thái nghiệp vụ như `ChuaBatDau`, `DangThucHien`, `BiCanCan`, `ChoXacNhanHoanThanh`, `HoanThanh`, `TamDung`, `DaHuy`. CRUD `ChiTietCongViec` không còn cho đổi trạng thái trực tiếp; trạng thái thật của chi tiết được đổi khi báo cáo tiến độ trong `TienDoCongViecService.XuLyBaoCaoTienDoAsync` được duyệt. Không tìm thấy action/service/view riêng để "mở lại" chi tiết công việc từ `TamDung`, `BiCanCan`, `HoanThanh` về `DangThucHien`.

Điểm gây kẹt quan trọng:

- `TienDoCongViecService.BiKhoaCapNhatTheoTrangThai` khóa cập nhật nếu `trangThaiCtCongViec` là `TamDung`, `DaHuy`, `LuuTru`, hoặc hoàn thành; vì vậy chi tiết đang `TamDung` không gửi được báo cáo để quay lại `DangThucHien` (`QuanLyDuAn/QuanLyDuAn/Services/Implementations/TienDoCongViecService.cs:906`).
- `TienDoCongViecService.KiemTraKhongDuocLuiTrangThai` không cho trạng thái mới có thứ tự thấp hơn trạng thái hiện tại hoặc báo cáo đã duyệt gần nhất (`TienDoCongViecService.cs:829`). `LayThuTuTrangThaiChiTiet` đặt `DangThucHien = 1`, `BiCanCan = 2`, `TamDung = 5`; do đó `BiCanCan -> DangThucHien` và `TamDung -> DangThucHien` đều là lùi trạng thái (`TienDoCongViecService.cs:861`).
- `ChiTietCongViecController` chỉ có `Index`, `Them`, `Sua`, `Xoa`, `XuatFile`, không có `MoLai`, `TiepTuc`, `Resume`, `Reopen` (`QuanLyDuAn/QuanLyDuAn/Controllers/ChiTietCongViecController.cs:32`, `:51`, `:79`, `:107`, `:126`).
- `IChiTietCongViecService` chỉ có `GetPageAsync`, `AddAsync`, `UpdateAsync`, `RemoveAsync`, không có method mở lại (`QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IChiTietCongViecService.cs:7`).

## 2. Danh sách file liên quan

Các nơi đã kiểm tra bằng từ khóa: `CT_CONG_VIEC`, `CtCongViec`, `ChiTietCongViec`, `TrangThaiCTCV`, `BiCanCan`, `TamDung`, `DangThucHien`, `ChoXacNhanHoanThanh`, `MoLai`, `TiepTuc`, `CapNhatTienDo`, `DuyetTienDo`, `TrangThaiWorkflowService`, `DongBoTrangThai`, `SyncStatus`, `Reopen`, `Resume`, `Continue`.

| Nhóm | File/class/method liên quan | Vai trò | Ghi chú |
|---|---|---|---|
| Controller chi tiết | `Controllers/ChiTietCongViecController.cs`, class `ChiTietCongViecController`, methods `Index`, `Them`, `Sua`, `Xoa`, `XuatFile` | Danh sách, thêm, sửa metadata, xóa mềm, xuất file | Không tìm thấy action mở lại/tiếp tục |
| Service chi tiết | `Services/Implementations/ChiTietCongViecService.cs`, class `ChiTietCongViecService`, methods `GetPageAsync`, `AddAsync`, `UpdateAsync`, `RemoveAsync` | Xử lý CRUD, quyền, khóa theo trạng thái công việc cha, gọi đồng bộ workflow | `AddAsync` ép `TrangThaiCTCV = ChuaBatDau`; `UpdateAsync` không đổi trạng thái |
| Interface chi tiết | `Services/Interfaces/IChiTietCongViecService.cs` | Contract service | Không có `MoLaiChiTietCongViecAsync` |
| Entity chi tiết | `Models/Entities/CtCongViec.cs`, class `CtCongViec`, properties `MaChiTietCV`, `MaCongViec`, `TrangThaiCTCV`, `NgayKetThucCTCV`, `IsDeleted` | Bảng `CT_CONG_VIEC` | Không có trường lý do mở lại |
| DbContext mapping | `Data/QuanLyDuAnDbContext.cs`, `DbSet<CtCongViec>`, mapping `CT_CONG_VIEC`, property `TrangThaiCTCV` | EF Core mapping | Mapping `TrangThaiCTCV` max length 50; FK tới `CONG_VIEC` |
| View chi tiết | `Views/ChiTietCongViec/Index.cshtml`, `_Form.cshtml`, `_Table.cshtml` | UI danh sách/thêm/sửa | Không có nút `Bị cản trở`, `Tạm dừng`, `Mở lại`, `Tiếp tục`; chỉ hiển thị trạng thái |
| ViewModel chi tiết | `ViewModels/ChiTietCongViec/*` | Dữ liệu UI | `ChiTietCongViecCreateUpdateViewModel.TrangThaiCTCV` còn tồn tại nhưng form không gửi để đổi trạng thái |
| Controller tiến độ | `Controllers/TienDoCongViecController.cs`, methods `CapNhatTienDo`, `DuyetBaoCaoTienDo`, `YeuCauBoSungBaoCaoTienDo`, `TuChoiBaoCaoTienDo` | Gửi/duyệt báo cáo tiến độ | Đây là luồng đổi trạng thái thật của `CT_CONG_VIEC` khi duyệt |
| Service tiến độ | `Services/Implementations/TienDoCongViecService.cs` | Tạo báo cáo chờ duyệt, duyệt/từ chối/yêu cầu bổ sung, set `TrangThaiCTCV` | Có rule không lùi trạng thái; không có mở lại |
| View tiến độ | `Views/TienDoCongViec/_Table.cshtml`, `_UpdateForm.cshtml`, `_History.cshtml` | UI báo cáo, lịch sử, duyệt | Dropdown lấy từ `TrangThaiDeXuatOptions`, không có nút mở lại riêng |
| Workflow service | `Services/Implementations/TrangThaiWorkflowService.cs`, interface `ITrangThaiWorkflowService.cs` | Đồng bộ `CT_CONG_VIEC -> CONG_VIEC -> DU_AN` | Đồng bộ công việc theo chi tiết và dự án theo công việc |
| Công việc cha | `Controllers/CongViecController.cs`, `Services/Implementations/CongViecService.cs`, `Views/CongViec/_Table.cshtml` | Có mở lại công việc cha | Chỉ mở lại `CONG_VIEC` đã hoàn thành, không mở lại `CT_CONG_VIEC` |
| Dự án | `Controllers/DuAnController.cs`, `Services/Implementations/DuAnService.cs`, `Views/DuAn/_Form.cshtml`, `Views/DuAn/Details.cshtml` | Có bắt đầu, tạm dừng, mở lại dự án | Không tự tạm dừng dự án do chi tiết tạm dừng |
| Phân công chi tiết | `Controllers/PhanCongChiTietCongViecController.cs`, `Services/Implementations/PhanCongChiTietCongViecService.cs` | Phân công/xóa phân công chi tiết, ghi nhật ký phân công | Không đổi `TrangThaiCTCV`, không gọi workflow sync |
| Constants | `Constants/TrangThai.cs`, `Constants/Permissions.cs` | Mã trạng thái, quyền | Không có permission mở lại chi tiết riêng |
| JavaScript | Đã kiểm tra các view liên quan và `wwwroot` qua grep từ khóa | Không tìm thấy JS riêng đổi trạng thái chi tiết | Không tìm thấy `SyncStatus`, `Reopen`, `Resume`, `Continue` cho `CT_CONG_VIEC` |

## 3. Entity và bảng dữ liệu liên quan

| Entity/bảng | File/property | Ý nghĩa | Liên quan workflow |
|---|---|---|---|
| `CtCongViec` / `CT_CONG_VIEC` | `Models/Entities/CtCongViec.cs`, `TrangThaiCTCV`, `NgayKetThucCTCV` | Chi tiết công việc | Trạng thái thật nằm ở `TrangThaiCTCV`; ngày hoàn thành thật set khi duyệt trạng thái hoàn thành |
| `CongViec` / `CONG_VIEC` | `Models/Entities/CongViec.cs`, `TrangThaiCongViec`, `NgayKetThucCVThucTe` | Công việc cha | Đồng bộ theo trạng thái các chi tiết trong `TrangThaiWorkflowService` |
| `DuAn` / `DU_AN` | `Models/Entities/DuAn.cs`, `TrangThaiDuAn`, `PhanTramHoanThanh`, `NgayHoanThanhThucTeDuAn` | Dự án | Đồng bộ phần trăm và một số trạng thái theo công việc |
| `TienDoCongViec` / `TIEN_DO_CONG_VIEC` | `Models/Entities/TienDoCongViec.cs`, `TrangThaiCTCVDeXuat`, `TrangThaiTienDo`, `GhiChuDuyet` | Báo cáo tiến độ | Là bản ghi chờ duyệt; chỉ khi `DaDuyet` mới set `CT_CONG_VIEC.TrangThaiCTCV` |
| `PhanCongCtCongViec` / `PHAN_CONG_CT_CONG_VIEC` | `Models/Entities/PhanCongCtCongViec.cs` | Phân công người thực hiện chi tiết | Là điều kiện để nhân viên gửi báo cáo tiến độ |
| `NhatKyPhanCongCtCongViec` / `NHAT_KY_PHAN_CONG_CT_CONG_VIEC` | `QuanLyDuAnDbContext.cs:679`, `PhanCongChiTietCongViecService.GhiNhatKy` | Nhật ký phân công chi tiết | Chỉ ghi thêm/xóa phân công, không ghi đổi trạng thái chi tiết |
| `NhatKyQuanLyDuAn` / `NHAT_KY_QUAN_LY_DU_AN` | `QuanLyDuAnDbContext.cs:711`, `TrangThaiWorkflowService`, `CongViecService`, `DuAnService` | Nhật ký quản lý dự án | Có ghi khi rollback trạng thái công việc/dự án hoặc mở lại công việc/dự án |

DbContext liên quan:

- `QuanLyDuAnDbContext.CtCongViec` (`Data/QuanLyDuAnDbContext.cs:27`).
- Mapping `CtCongViec` tới bảng `CT_CONG_VIEC`, `TrangThaiCTCV` max length 50, FK `MaCongViec` tới `CongViec` (`Data/QuanLyDuAnDbContext.cs:268`).
- Mapping `TienDoCongViec.TrangThaiCTCVDeXuat` max length 50 và FK tới `CtCongViec` (`Data/QuanLyDuAnDbContext.cs:800`).

## 4. Danh sách trạng thái hiện có

Nguồn chính: `Constants/TrangThai.cs`, `TienDoCongViecService.KiemTraTrangThaiChiTietHopLe`, `TrangThaiWorkflowService.TinhTrangThaiCongViecTheoChiTiet`, các view filter/form.

| Tên trạng thái code | Tên hiển thị | Đối tượng áp dụng | Ý nghĩa nghiệp vụ | UI chọn được | Service tự set | Có chuyển tiếp khác | File/method liên quan |
|---|---|---|---|---|---|---|---|
| `ChuaBatDau` | Chưa bắt đầu | `CT_CONG_VIEC`, `CONG_VIEC` | Chưa có tiến triển | Chi tiết: không chọn trong CRUD; có thể là option báo cáo nếu rule cho phép | `ChiTietCongViecService.AddAsync` set mặc định; workflow có thể set công việc nếu mọi chi tiết chưa bắt đầu | Có thể đi tới `DangThucHien` qua báo cáo nếu chưa bị khóa | `TrangThai.cs`; `ChiTietCongViecService.cs:156`; `TienDoCongViecService.cs:811` |
| `DangThucHien` | Đang thực hiện | `CT_CONG_VIEC`, `CONG_VIEC`, `DU_AN` | Đang xử lý | Dự án có nút bắt đầu/tái kích hoạt; chi tiết có thể đề xuất qua tiến độ nếu không lùi | Workflow set công việc khi có tiến triển nhưng chưa đủ hoàn thành/blocker | Có thể đi tới `BiCanCan`, `ChoXacNhanHoanThanh`, `HoanThanh`, `TamDung`, `DaHuy` qua báo cáo chi tiết | `TienDoCongViecService.cs:812`; `TrangThaiWorkflowService.cs:141`; `DuAnService.cs:1314` |
| `BiCanCan` | Bị cản trở | `CT_CONG_VIEC`, `CONG_VIEC` | Có blocker | Chi tiết có thể đề xuất qua tiến độ nếu từ trạng thái thấp hơn; không có nút riêng | Workflow set `CONG_VIEC = BiCanCan` nếu có chi tiết bị cản trở | Không quay về `DangThucHien` theo source hiện tại vì bị coi là lùi trạng thái | `TienDoCongViecService.cs:813`; `TrangThaiWorkflowService.cs:137` |
| `ChoXacNhanHoanThanh` | Chờ xác nhận hoàn thành | `CT_CONG_VIEC`, `CONG_VIEC`, `DU_AN` | Chờ cấp quản lý xác nhận | Chi tiết có thể đề xuất qua tiến độ; công việc/dự án có UI xác nhận | Workflow set công việc khi tất cả chi tiết hoàn thành; set dự án khi tất cả công việc hoàn thành | Có thể lên `HoanThanh`; không thấy rule từ chối chi tiết tự đưa về `DangThucHien` | `TienDoCongViecService.cs:814`; `TrangThaiWorkflowService.cs:134`; `CongViecService.cs:188` |
| `HoanThanh` | Hoàn thành | `CT_CONG_VIEC`, `CONG_VIEC`, `DU_AN` | Đã hoàn tất | Chi tiết có thể đề xuất qua tiến độ, cần file minh chứng; công việc/dự án có xác nhận | Duyệt báo cáo chi tiết set chi tiết; xác nhận công việc/dự án set cấp trên | Công việc/dự án có mở lại; chi tiết không có mở lại | `TienDoCongViecService.cs:576`; `CongViecService.cs:198`; `DuAnService.cs:1398` |
| `TamDung` | Tạm dừng | `CT_CONG_VIEC`, `CONG_VIEC`, `DU_AN` | Tạm ngưng xử lý | Dự án có form tạm dừng; chi tiết có thể đề xuất qua tiến độ từ trạng thái thấp hơn | Không thấy tự set dự án/công việc sang tạm dừng từ chi tiết | Chi tiết bị khóa cập nhật, không có đường quay lại; dự án `TamDung` có `TransitionToDangThucHien` trong status check | `TienDoCongViecService.cs:816`, `:906`; `DuAnService.cs:1455`; `DuAnService.cs:1186` |
| `DaHuy` | Đã hủy | `CT_CONG_VIEC`, `CONG_VIEC`, `DU_AN`, báo cáo | Đã hủy/đóng | Chi tiết có thể đề xuất qua tiến độ nếu rule cho phép | Không thấy tự hủy chi tiết từ workflow | Bị khóa cập nhật; không thấy mở lại chi tiết | `TienDoCongViecService.cs:817`, `:920`; `TrangThai.cs` |
| `ChoDuyet` | Chờ duyệt | `TIEN_DO` | Báo cáo tiến độ đang chờ duyệt | Tạo tự động khi gửi báo cáo | `CapNhatTienDoAsync` set `TrangThaiTienDo = ChoDuyet` | Sang `DaDuyet`, `YeuCauBoSung`, `TuChoi` | `TienDoCongViecService.cs:456` |
| `DaDuyet` | Đã duyệt | `TIEN_DO` | Báo cáo được duyệt | Nút duyệt | Khi duyệt, cập nhật `CT_CONG_VIEC.TrangThaiCTCV` | Không xử lý lại báo cáo đã xử lý | `TienDoCongViecService.cs:491`, `:551`, `:576` |
| `YeuCauBoSung` | Yêu cầu bổ sung | `TIEN_DO` | Báo cáo cần bổ sung | Nút yêu cầu bổ sung | Chỉ đổi `TienDoCongViec.TrangThaiTienDo`, không đổi chi tiết | Không thấy tự đổi `CT_CONG_VIEC` | `TienDoCongViecService.cs:496`, `:514` |
| `TuChoi` | Từ chối | `TIEN_DO` | Báo cáo bị từ chối | Nút từ chối | Chỉ đổi `TienDoCongViec.TrangThaiTienDo`, không đổi chi tiết | Không thấy tự đổi `CT_CONG_VIEC` | `TienDoCongViecService.cs:501`, `:514` |
| `LuuTru` / `Archived` | Lưu trữ | Chủ yếu dự án/công việc/khóa | Đã lưu trữ/đóng | Không tìm thấy UI chọn cho chi tiết | Dùng để khóa cập nhật | Không thấy mở lại chi tiết | `TrangThai.cs`; `TienDoCongViecService.cs:923` |

## 5. Luồng hiện tại của chi tiết công việc

### 5.1. Tạo chi tiết công việc

- Controller: `ChiTietCongViecController.Them(ChiTietCongViecCreateUpdateViewModel form)` kiểm tra `Permissions.ChiTietCongViec.Them`, rồi gọi `_service.AddAsync(form)` (`Controllers/ChiTietCongViecController.cs:51`).
- Service: `ChiTietCongViecService.AddAsync` lấy công việc cha, validate dữ liệu, kiểm tra quyền/scope, kiểm tra trạng thái công việc/dự án, lấy current user (`Services/Implementations/ChiTietCongViecService.cs:140`).
- Trạng thái mặc định: `TrangThaiCTCV = TrangThai.ChuaBatDau`; `NgayKetThucCTCV = null`; `NgayTaoCTCV = DateTime.Now` (`ChiTietCongViecService.cs:148`).
- Ai được tạo: ngoài permission controller, service `CoTheCapNhatAsync` cho Admin/Manager; Employee chỉ khi leader team hoặc leader dự án; role khác nếu leader dự án hoặc được phân công công việc cha (`ChiTietCongViecService.cs:349`).
- Đồng bộ: sau insert gọi `ITrangThaiWorkflowService.DongBoChuoiTrangThaiTuCongViecAsync(congViec.MaCongViec, currentUserId, "Thêm chi tiết công việc")` trong transaction (`ChiTietCongViecService.cs:166`).
- Không ghi nhật ký chi tiết riêng khi tạo. Không tự phân công người tạo.

### 5.2. Phân công chi tiết công việc

- Controller: `PhanCongChiTietCongViecController.ThemPhanCong` yêu cầu `Permissions.PhanCongChiTietCongViec.ThucHien` (`Controllers/PhanCongChiTietCongViecController.cs:41`).
- Service: `PhanCongChiTietCongViecService.AddAsync` kiểm tra trạng thái chi tiết, quyền phân công, nhân viên hợp lệ, nhân viên đã được phân công công việc cha, sau đó thêm `PhanCongCtCongViec` (`Services/Implementations/PhanCongChiTietCongViecService.cs:89`).
- Trạng thái có đổi không: không đổi `CtCongViec.TrangThaiCTCV`.
- Nhật ký: có `GhiNhatKy(..., TrangThai.HanhDongThemPhanCongChiTiet)` vào `NhatKyPhanCongCtCongViec` (`PhanCongChiTietCongViecService.cs:130`, `:303`).
- Đồng bộ lên công việc/dự án: không tìm thấy gọi `ITrangThaiWorkflowService` trong `PhanCongChiTietCongViecService`.
- Khóa phân công: chặn khi chi tiết hoàn thành, `DaHuy`, `LuuTru`, `TamDung`; không chặn `BiCanCan` (`PhanCongChiTietCongViecService.cs:187`).

### 5.3. Nhân viên cập nhật tiến độ chi tiết công việc

- Controller: `TienDoCongViecController.CapNhatTienDo` yêu cầu `Permissions.TienDo.CapNhat`, gọi `TienDoCongViecService.CapNhatTienDoAsync` (`Controllers/TienDoCongViecController.cs:74`).
- Cập nhật trực tiếp hay chờ duyệt: không cập nhật trực tiếp `CT_CONG_VIEC`; tạo bản ghi `TienDoCongViec` với `TrangThaiCTCVDeXuat`, `TrangThaiTienDo = ChoDuyet` (`TienDoCongViecService.cs:456`).
- Người được cập nhật: không phải Admin, phải được phân công chi tiết qua `PHAN_CONG_CT_CONG_VIEC` (`TienDoCongViecService.cs:407`, `:423`).
- Khóa cập nhật nếu dự án/công việc/chi tiết đã hoàn thành, `DaHuy`, `TamDung`, `LuuTru`; riêng công việc và dự án cũng bị khóa nếu `TamDung` (`TienDoCongViecService.cs:420`, `:906`).
- Trạng thái được phép đề xuất: `ChuaBatDau`, `DangThucHien`, `BiCanCan`, `ChoXacNhanHoanThanh`, `HoanThanh`, `TamDung`, `DaHuy` (`TienDoCongViecService.cs:782`).
- Nếu đang `TamDung`: không được cập nhật do bị khóa (`TienDoCongViecService.cs:920`).
- Nếu đang `BiCanCan`: không bị khóa, nhưng không được chọn `DangThucHien` vì `KiemTraKhongDuocLuiTrangThai` và thứ tự `BiCanCan = 2`, `DangThucHien = 1` (`TienDoCongViecService.cs:829`, `:869`, `:872`).

### 5.4. Duyệt tiến độ chi tiết công việc

- Controller: `DuyetBaoCaoTienDo`, `YeuCauBoSungBaoCaoTienDo`, `TuChoiBaoCaoTienDo` yêu cầu `Permissions.TienDo.Duyet` (`Controllers/TienDoCongViecController.cs:111`, `:137`, `:163`).
- Người duyệt: không phải Admin, không được tự duyệt báo cáo của mình, phải nằm trong scope dự án review qua Manager quản lý dự án, leader dự án hoặc team leader (`TienDoCongViecService.cs:536`, `:539`, `:729`).
- Khi duyệt `DaDuyet`: set `TienDoCongViec.TrangThaiTienDo = DaDuyet`, set `CtCongViec.TrangThaiCTCV = trangThaiDeXuat`, set `NgayKetThucCTCV = DateTime.Now` nếu trạng thái là hoàn thành, ngược lại `null`, rồi gọi đồng bộ chuỗi từ công việc (`TienDoCongViecService.cs:551`, `:576`, `:580`).
- Khi `YeuCauBoSung` hoặc `TuChoi`: yêu cầu `GhiChuDuyet`; chỉ đổi trạng thái báo cáo, không đổi `CT_CONG_VIEC.TrangThaiCTCV`, không gọi workflow sync trong nhánh này (`TienDoCongViecService.cs:514`, `:556`).

### 5.5. Xác nhận hoàn thành chi tiết công việc

- Không tìm thấy action riêng `XacNhanHoanThanhChiTietCongViec`.
- Hoàn thành chi tiết đi qua báo cáo tiến độ: nhân viên đề xuất `HoanThanh` hoặc `ChoXacNhanHoanThanh`; khi báo cáo được duyệt thì `CT_CONG_VIEC.TrangThaiCTCV` đổi theo đề xuất (`TienDoCongViecService.cs:576`).
- Nếu đề xuất `ChoXacNhanHoanThanh` hoặc `HoanThanh`, bắt buộc có file minh chứng khi gửi/duyệt (`TienDoCongViecService.cs:929`, `:949`).

### 5.6. Mở lại chi tiết công việc

- Không tìm thấy chức năng mở lại chi tiết công việc.
- Không tìm thấy `ChiTietCongViecController.MoLai`, `ChiTietCongViecService.MoLai...`, `IChiTietCongViecService.MoLai...`, view nút `Mở lại` trong `Views/ChiTietCongViec`.
- Không có yêu cầu lý do mở lại chi tiết, không có nhật ký mở lại chi tiết, không có đồng bộ cha/con cho hành động mở lại chi tiết vì hành động chưa tồn tại.

### 5.7. Tạm dừng / bị cản trở

- `TamDung` và `BiCanCan` được set cho chi tiết khi báo cáo tiến độ đề xuất trạng thái đó được duyệt (`TienDoCongViecService.cs:576`).
- Không tìm thấy nút riêng trên màn `ChiTietCongViec` để set tạm dừng/bị cản.
- Sau khi `TamDung`: không có đường quay lại `DangThucHien` vì cập nhật tiến độ bị khóa (`TienDoCongViecService.cs:906`).
- Sau khi `BiCanCan`: không có đường quay lại `DangThucHien` vì rule không cho lùi trạng thái (`TienDoCongViecService.cs:829`).
- Ảnh hưởng công việc cha: workflow set `CONG_VIEC = BiCanCan` nếu có bất kỳ chi tiết `BiCanCan`; nếu có `TamDung` nhưng không có `BiCanCan` và chưa tất cả hoàn thành, công việc cha thành/giữ `DangThucHien` (`TrangThaiWorkflowService.cs:137`, `:140`).

## 6. Sơ đồ chuyển trạng thái hiện tại

| Đối tượng | Từ trạng thái | Sang trạng thái | Trigger/action | Người thực hiện | File/method | Có ghi nhật ký | Có đồng bộ lên cấp trên |
|---|---|---|---|---|---|---|---|
| `CT_CONG_VIEC` | không có record | `ChuaBatDau` | Tạo chi tiết | Người có `ChiTietCongViec.Them` và scope cập nhật | `ChiTietCongViecController.Them`; `ChiTietCongViecService.AddAsync` | Không thấy nhật ký chi tiết | Có gọi `DongBoChuoiTrangThaiTuCongViecAsync` |
| `CT_CONG_VIEC` | `ChuaBatDau` | `DangThucHien` | Báo cáo tiến độ được duyệt | Nhân viên được phân công gửi; Manager/leader duyệt | `TienDoCongViecService.CapNhatTienDoAsync`; `XuLyBaoCaoTienDoAsync` | Record `TIEN_DO_CONG_VIEC`; không thấy nhật ký riêng | Có |
| `CT_CONG_VIEC` | `DangThucHien` | `BiCanCan` | Báo cáo tiến độ được duyệt | Nhân viên được phân công + người duyệt hợp lệ | `TienDoCongViecService.XuLyBaoCaoTienDoAsync` | Record tiến độ | Có |
| `CT_CONG_VIEC` | `BiCanCan` | `DangThucHien` | Không tìm thấy | Không tìm thấy | Không tìm thấy | Không tìm thấy | Không tìm thấy |
| `CT_CONG_VIEC` | `DangThucHien` | `TamDung` | Báo cáo tiến độ được duyệt | Nhân viên được phân công + người duyệt hợp lệ | `TienDoCongViecService.XuLyBaoCaoTienDoAsync` | Record tiến độ | Có |
| `CT_CONG_VIEC` | `TamDung` | `DangThucHien` | Không tìm thấy | Không tìm thấy | Không tìm thấy | Không tìm thấy | Không tìm thấy |
| `CT_CONG_VIEC` | `DangThucHien`/`BiCanCan` | `ChoXacNhanHoanThanh` | Báo cáo tiến độ được duyệt, có minh chứng | Nhân viên được phân công + người duyệt hợp lệ | `TienDoCongViecService.XuLyBaoCaoTienDoAsync` | Record tiến độ | Có |
| `CT_CONG_VIEC` | `ChoXacNhanHoanThanh` | `HoanThanh` | Báo cáo tiến độ mới được duyệt | Nhân viên được phân công + người duyệt hợp lệ | `TienDoCongViecService.XuLyBaoCaoTienDoAsync` | Record tiến độ | Có |
| `CT_CONG_VIEC` | `ChoXacNhanHoanThanh` | `DangThucHien` khi từ chối/yêu cầu bổ sung | Không tìm thấy | Không tìm thấy | `YeuCauBoSung/TuChoi` chỉ đổi `TienDoCongViec.TrangThaiTienDo` | Record tiến độ có trạng thái từ chối/bổ sung | Không |
| `CT_CONG_VIEC` | `HoanThanh` | `DangThucHien` | Không tìm thấy | Không tìm thấy | Không tìm thấy | Không tìm thấy | Không tìm thấy |
| `CONG_VIEC` | theo chi tiết bất kỳ | `BiCanCan` | Đồng bộ khi có chi tiết `BiCanCan` | Service hệ thống theo action nguồn | `TrangThaiWorkflowService.TinhTrangThaiCongViecTheoChiTiet` | Chỉ ghi rollback từ `ChoXacNhanHoanThanh`, không ghi mọi lần | Có gọi đồng bộ dự án |
| `CONG_VIEC` | theo chi tiết | `DangThucHien` | Đồng bộ khi có tiến triển, không tất cả hoàn thành, không blocker | Service hệ thống | `TrangThaiWorkflowService.TinhTrangThaiCongViecTheoChiTiet` | Có thể ghi nếu rollback từ `ChoXacNhanHoanThanh` | Có |
| `CONG_VIEC` | theo chi tiết | `ChoXacNhanHoanThanh` | Tất cả chi tiết hoàn thành | Service hệ thống | `TrangThaiWorkflowService.TinhTrangThaiCongViecTheoChiTiet` | Không thấy nhật ký riêng | Có |
| `CONG_VIEC` | `ChoXacNhanHoanThanh` | `HoanThanh` | Xác nhận hoàn thành công việc | Người có quyền workflow công việc | `CongViecController.XacNhanHoanThanh`; `CongViecService.XacNhanHoanThanhCongViecAsync` | Không thấy nhật ký trong method này | Có gọi sync dự án |
| `CONG_VIEC` | `HoanThanh` | `DangThucHien` | Mở lại công việc | Người có quyền workflow công việc, cần lý do | `CongViecController.MoLai`; `CongViecService.MoLaiCongViecAsync` | Có `NhatKyQuanLyDuAn` | Có gọi sync dự án |
| `DU_AN` | `KhoiTao` | `DangThucHien` | Bắt đầu dự án | Manager dự án | `DuAnController`, `DuAnService.TransitionToDangThucHienAsync` | Không thấy trong method | Không áp dụng |
| `DU_AN` | `DangThucHien` | `TamDung` | Tạm dừng dự án | Manager dự án, cần ghi chú | `DuAnController.TamDungDuAn`; `DuAnService.PauseProjectAsync` | Không thấy trong method | Không áp dụng |
| `DU_AN` | `TamDung` | `DangThucHien` | Có status check cho phép bắt đầu lại, nhưng `TransitionToDangThucHienAsync` chỉ cho `KhoiTao` | Manager dự án | Mâu thuẫn giữa `CheckProjectStatusAsync` và `TransitionToDangThucHienAsync` | Không thấy | Không áp dụng |
| `DU_AN` | `HoanThanh` | `DangThucHien` | Mở lại dự án | Manager dự án, cần lý do | `DuAnController.MoLaiDuAn`; `DuAnService.MoLaiDuAnAsync` | Có `NhatKyQuanLyDuAn` | Không áp dụng |
| `DU_AN` | theo công việc | `ChoXacNhanHoanThanh` | Tất cả công việc hoàn thành | Workflow sync | `TrangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync` | Không thấy | Không áp dụng |
| `DU_AN` | `ChoXacNhanHoanThanh` | `DangThucHien` | Có công việc chưa hoàn thành sau sync | Workflow sync | `TrangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync` | Có `NhatKyQuanLyDuAn` nếu có user | Không áp dụng |

Không tự bịa transition: các dòng "Không tìm thấy" là các transition được tìm theo keyword nhưng không có trong source đã kiểm tra.

## 7. Chức năng mở lại/tiếp tục hiện có

| Chức năng | Đối tượng | File/method | Điều kiện | Quyền | Trạng thái nguồn -> đích | Có lý do | Nhật ký | Đồng bộ |
|---|---|---|---|---|---|---|---|---|
| Mở lại công việc | `CONG_VIEC` | `CongViecController.MoLai`; `CongViecService.MoLaiCongViecAsync` | Chỉ công việc đã hoàn thành; dự án không được hoàn thành | `CoQuyenXuLyWorkflowCongViecAsync` ở controller/service scope | `HoanThanh` -> `DangThucHien` | Có, bắt buộc `lyDo` | Có `NhatKyQuanLyDuAn` | Có `DongBoTrangThaiDuAnTheoCongViecAsync` |
| Mở lại dự án | `DU_AN` | `DuAnController.MoLaiDuAn`; `DuAnService.MoLaiDuAnAsync` | Chỉ dự án hoàn thành | `Permissions.DuAn.Sua` và `CheckManagerPermissionAsync` | `HoanThanh` -> `DangThucHien` | Có, bắt buộc `lyDo` | Có `NhatKyQuanLyDuAn` | Không áp dụng |
| Tiếp tục dự án từ tạm dừng | `DU_AN` | Có `CheckProjectStatusAsync` set `CanTransitionToDangThucHien = true` khi `TamDung`; nhưng `TransitionToDangThucHienAsync` chỉ cho `KhoiTao` | Mâu thuẫn source | Manager | Không xác nhận được chạy thành công | Không thấy | Không thấy | Không áp dụng |
| Mở lại chi tiết công việc | `CT_CONG_VIEC` | Không tìm thấy | Không tìm thấy | Không tìm thấy | Không tìm thấy | Không tìm thấy | Không tìm thấy | Không tìm thấy |

Kết luận: chỉ có mở lại `CONG_VIEC` và `DU_AN`; không có mở lại `CT_CONG_VIEC`.

## 8. Đồng bộ trạng thái lên công việc và dự án

### 8.1. Khi `CT_CONG_VIEC` thay đổi có tự đồng bộ `CONG_VIEC` không?

Có trong các luồng service gọi `ITrangThaiWorkflowService.DongBoChuoiTrangThaiTuCongViecAsync`:

- Tạo chi tiết: `ChiTietCongViecService.AddAsync` (`ChiTietCongViecService.cs:166`).
- Sửa metadata chi tiết: `ChiTietCongViecService.UpdateAsync` (`ChiTietCongViecService.cs:209`).
- Xóa mềm chi tiết: `ChiTietCongViecService.RemoveAsync` (`ChiTietCongViecService.cs:247`).
- Duyệt báo cáo tiến độ `DaDuyet`: `TienDoCongViecService.XuLyBaoCaoTienDoAsync` (`TienDoCongViecService.cs:580`).

Không thấy gọi đồng bộ ở:

- Phân công/xóa phân công chi tiết.
- Gửi báo cáo tiến độ ở trạng thái `ChoDuyet`.
- Từ chối/yêu cầu bổ sung báo cáo.
- Mở lại chi tiết, vì chức năng chưa có.

### 8.2. Khi `CONG_VIEC` thay đổi có tự đồng bộ `DU_AN` không?

Có trong:

- `TrangThaiWorkflowService.DongBoChuoiTrangThaiTuCongViecAsync`: sau khi đồng bộ công việc theo chi tiết, lấy `MaDuAn` và gọi `DongBoTrangThaiDuAnTheoCongViecAsync` (`TrangThaiWorkflowService.cs:18`).
- `CongViecService.XacNhanHoanThanhCongViecAsync` gọi `DongBoTrangThaiDuAnTheoCongViecAsync` (`CongViecService.cs:204`).
- `CongViecService.MoLaiCongViecAsync` gọi `DongBoTrangThaiDuAnTheoCongViecAsync` (`CongViecService.cs:241`).

### 8.3. Quy tắc đồng bộ hiện tại

`TrangThaiWorkflowService.DongBoTrangThaiCongViecTheoChiTietAsync`:

- Không đồng bộ nếu công việc hiện tại đã hoàn thành, `TamDung`, hoặc `DaHuy` (`TrangThaiWorkflowService.cs:37`).
- Lấy tất cả `CtCongViec.TrangThaiCTCV` chưa xóa mềm của công việc (`TrangThaiWorkflowService.cs:45`).
- Nếu không có chi tiết: `CONG_VIEC = ChuaBatDau` (`TrangThaiWorkflowService.cs:129`).
- Nếu tất cả chi tiết hoàn thành: `CONG_VIEC = ChoXacNhanHoanThanh` (`TrangThaiWorkflowService.cs:134`).
- Nếu có bất kỳ chi tiết `BiCanCan`: `CONG_VIEC = BiCanCan` (`TrangThaiWorkflowService.cs:137`).
- Nếu có tiến triển gồm `DangThucHien`, `ChoXacNhanHoanThanh`, `TamDung`, `DaHuy`, hoặc trạng thái khác `ChuaBatDau`: `CONG_VIEC = DangThucHien` (`TrangThaiWorkflowService.cs:140`).
- Set `NgayKetThucCVThucTe` nếu công việc mới là `ChoXacNhanHoanThanh` hoặc hoàn thành; ngược lại set `null` (`TrangThaiWorkflowService.cs:66`).

`TrangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync`:

- Luôn tính `DuAn.PhanTramHoanThanh` bằng trung bình phần trăm trạng thái công việc (`TrangThaiWorkflowService.cs:95`, `:150`).
- Không đổi trạng thái dự án nếu dự án đã hoàn thành, `LuuTru`, `DaHuy`, hoặc `TamDung` (`TrangThaiWorkflowService.cs:97`).
- Nếu tất cả công việc hoàn thành và dự án chưa `ChoXacNhanHoanThanh`: set `DU_AN = ChoXacNhanHoanThanh` (`TrangThaiWorkflowService.cs:105`, `:125`).
- Nếu dự án đang `ChoXacNhanHoanThanh` nhưng không còn tất cả công việc hoàn thành: rollback `DU_AN = DangThucHien`, có ghi `NhatKyQuanLyDuAn` nếu có người thực hiện (`TrangThaiWorkflowService.cs:106`, `:111`, `:114`).
- Không thấy rule tự set dự án `BiCanCan` hoặc `TamDung` chỉ vì công việc/chi tiết bị cản trở/tạm dừng.

### 8.4. Trường hợp thiếu hoặc gây kẹt

- Thiếu đường mở lại chi tiết: `BiCanCan -> DangThucHien`, `TamDung -> DangThucHien`, `HoanThanh -> DangThucHien` đều không có.
- `TamDung` chi tiết bị khóa cập nhật tiến độ nên không tự thoát được.
- `BiCanCan` chi tiết không bị khóa nhưng không được lùi về `DangThucHien`.
- Công việc cha `BiCanCan` sẽ tự về `DangThucHien` nếu không còn chi tiết `BiCanCan` và có tiến triển, nhưng hiện thiếu hành động làm chi tiết hết `BiCanCan`.
- Dự án không tự `BiCanCan`; điều này hợp lý nếu chỉ dùng dashboard/cảnh báo, nhưng cần UI báo blocker nếu muốn quản lý thấy.

## 9. Đánh giá nghiệp vụ: có cần đồng bộ không

### Khi chi tiết công việc bị `BiCanCan`

- Nên đồng bộ công việc cha thành `BiCanCan`: Có. Source hiện đã làm điều này trong `TinhTrangThaiCongViecTheoChiTiet`; đây là hợp lý vì một blocker chi tiết có thể chặn công việc cha.
- Nên đồng bộ dự án thành `BiCanCan`: Không nên tự đổi trạng thái chính của dự án. Source hiện không tự set dự án `BiCanCan`; nên giữ dự án `DangThucHien` và hiển thị cảnh báo/dashboard số công việc bị cản trở.

### Khi chi tiết công việc `TamDung`

- Đồng bộ công việc cha thành `TamDung`: Không nên nếu chỉ một phần chi tiết tạm dừng; có thể giữ công việc `DangThucHien` kèm cảnh báo. Nếu toàn bộ chi tiết tạm dừng thì có thể cân nhắc rule riêng sau này.
- Dự án có nên bị tạm dừng: Không. Tạm dừng dự án là quyết định quản lý cấp dự án, source hiện yêu cầu action `PauseProjectAsync` và ghi chú (`DuAnService.cs:1455`).
- Tạm dừng cấp chi tiết khác với tạm dừng dự án: cấp chi tiết là một đầu việc nhỏ bị tạm ngưng; cấp dự án khóa phạm vi rộng và không nên bị kéo theo tự động.

### Khi chi tiết được mở lại về `DangThucHien`

- Cần đồng bộ công việc cha: Có. Nếu chi tiết hết `BiCanCan`/`TamDung`, công việc cha cần tính lại theo toàn bộ chi tiết.
- Cần đồng bộ dự án: Có theo chuỗi hiện có, nhưng chỉ nên áp dụng các rule hiện tại: cập nhật phần trăm, rollback dự án từ `ChoXacNhanHoanThanh` về `DangThucHien` nếu phát sinh việc chưa hoàn thành; không tự mở dự án `HoanThanh/TamDung/DaHuy`.

### Khi chi tiết hoàn thành

- Công việc cha nên chuyển `ChoXacNhanHoanThanh` khi tất cả chi tiết hoàn thành, đúng với source hiện tại.
- Công việc cha không nên tự `HoanThanh` nếu workflow yêu cầu quản lý xác nhận; source hiện có `CongViecService.XacNhanHoanThanhCongViecAsync`.
- Dự án nên `ChoXacNhanHoanThanh` khi tất cả công việc hoàn thành, nhưng `HoanThanh` vẫn cần manager xác nhận; source hiện làm vậy.

### Khi báo cáo bị từ chối hoặc yêu cầu bổ sung

- Không nên đổi trạng thái thật của chi tiết công việc; trạng thái báo cáo nằm ở `TIEN_DO_CONG_VIEC`. Source hiện đang giữ đúng ranh giới này.
- Nếu báo cáo bị từ chối trong trạng thái `ChoXacNhanHoanThanh`, có thể cần rule nghiệp vụ riêng để đưa chi tiết về `DangThucHien`, nhưng source hiện không làm. Nếu thêm, phải làm có chủ đích và ghi nhật ký/lý do.

Kết luận:

- Đồng bộ lên công việc: Có, sau khi trạng thái thật của chi tiết thay đổi hoặc chi tiết được thêm/xóa. Điều kiện: không ghi đè công việc đã hoàn thành, tạm dừng, đã hủy trừ khi có action mở lại chuyên biệt.
- Đồng bộ lên dự án: Có, nhưng giới hạn ở phần trăm, `ChoXacNhanHoanThanh`, rollback từ `ChoXacNhanHoanThanh` về `DangThucHien`; không tự đổi dự án sang `TamDung`, `HoanThanh`, `DaHuy`.
- Trạng thái dự án không nên tự động thay đổi: `TamDung`, `HoanThanh`, `DaHuy`, `LuuTru`; đặc biệt không tự `TamDung` vì một chi tiết tạm dừng.

## 10. Quy tắc workflow đề xuất

### Quy tắc source hiện có

- `CT_CONG_VIEC`: tạo mới luôn `ChuaBatDau`.
- `CT_CONG_VIEC`: đổi trạng thái thật khi báo cáo tiến độ được duyệt `DaDuyet`.
- `CT_CONG_VIEC`: không được lùi trạng thái qua báo cáo tiến độ.
- `CT_CONG_VIEC`: `TamDung`, `HoanThanh`, `DaHuy`, `LuuTru` bị khóa cập nhật tiến độ.
- `CONG_VIEC`: có chi tiết `BiCanCan` thì công việc cha `BiCanCan`.
- `CONG_VIEC`: tất cả chi tiết hoàn thành thì công việc cha `ChoXacNhanHoanThanh`.
- `DU_AN`: tất cả công việc hoàn thành thì dự án `ChoXacNhanHoanThanh`; dự án `HoanThanh/TamDung/DaHuy/LuuTru` không bị sync đổi trạng thái.

### Quy tắc đề xuất cho chi tiết công việc

- `ChuaBatDau -> DangThucHien`: cho phép qua báo cáo tiến độ hoặc action bắt đầu nếu cần.
- `DangThucHien -> BiCanCan`: cho phép, nên bắt buộc ghi chú nguyên nhân.
- `BiCanCan -> DangThucHien`: cần thêm action `MoLai/TiepTuc` riêng, bắt buộc lý do hoặc ghi chú xử lý blocker.
- `DangThucHien -> TamDung`: cho phép nếu có lý do tạm dừng; nên phân biệt tạm dừng do quản lý và do nhân viên báo cáo.
- `TamDung -> DangThucHien`: cần thêm action `TiepTuc/MoLai` riêng, bắt buộc lý do.
- `DangThucHien -> ChoXacNhanHoanThanh`: qua báo cáo tiến độ có minh chứng.
- `ChoXacNhanHoanThanh -> HoanThanh`: khi báo cáo/hoàn thành được duyệt.
- `ChoXacNhanHoanThanh -> DangThucHien`: chỉ khi từ chối/yêu cầu bổ sung được thiết kế là rollback trạng thái thật; source hiện chưa có.
- `HoanThanh -> DangThucHien`: chỉ khi mở lại có lý do và người có quyền; cần cân nhắc vì hiện source không cho lùi.

### Quy tắc đề xuất cho công việc cha

- Nếu có ít nhất một chi tiết `BiCanCan`: công việc cha = `BiCanCan` (source hiện có).
- Nếu không còn chi tiết `BiCanCan` và có chi tiết đang xử lý hoặc vừa mở lại: công việc cha = `DangThucHien`.
- Nếu tất cả chi tiết hoàn thành: công việc cha = `ChoXacNhanHoanThanh`, không tự `HoanThanh`.
- Nếu có chi tiết `TamDung` nhưng không phải toàn bộ: giữ `DangThucHien` kèm cảnh báo.
- Nếu toàn bộ chi tiết `TamDung`: có thể cân nhắc công việc cha `TamDung`, nhưng chỉ khi nghiệp vụ xác nhận.

### Quy tắc đề xuất cho dự án

- Không tự chuyển dự án sang `TamDung` chỉ vì một chi tiết/công việc tạm dừng.
- Không tự chuyển dự án sang `BiCanCan`; dùng cảnh báo/số liệu blocker.
- Dự án chỉ `TamDung` khi người có quyền tạm dừng dự án.
- Dự án chỉ `HoanThanh` khi đủ điều kiện và người có quyền xác nhận.
- Khi mở lại chi tiết/công việc, chỉ sync dự án nếu dự án đang ở trạng thái phụ thuộc tiến độ như `ChoXacNhanHoanThanh`; không tự mở `HoanThanh` nếu chưa mở dự án bằng action riêng.

## 11. UI hiện tại và nút còn thiếu

| View | Nút/field trạng thái | Trạng thái nào hiển thị | Action gọi tới | Có thiếu nút mở lại không | Nhận xét |
|---|---|---|---|---|---|
| `Views/ChiTietCongViec/_Form.cshtml` | Field đọc "Trạng thái khi tạo" | Luôn `ChuaBatDau` | POST `Them` | Có | Không có dropdown trạng thái khi tạo |
| `Views/ChiTietCongViec/_Table.cshtml` | Badge trạng thái, icon theo CSS | `TrangThaiHienThi` từ service | Không gọi action đổi trạng thái | Có | Form sửa chỉ có tên/nội dung/ngày bắt đầu, ghi chú trạng thái cập nhật qua báo cáo tiến độ |
| `Views/ChiTietCongViec/Index.cshtml` | Banner trạng thái công việc cha | Hoàn thành/chờ xác nhận/thông điệp workflow | Không đổi trạng thái chi tiết | Có | Không có modal lý do mở lại chi tiết |
| `Views/TienDoCongViec/_UpdateForm.cshtml` | Dropdown `TrangThaiCTCVMoi` | `TrangThaiDeXuatOptions` | POST `CapNhatTienDo` | Có | Đây là đường đề xuất đổi trạng thái, không phải mở lại riêng |
| `Views/TienDoCongViec/_Table.cshtml` | Nút `Báo cáo`, `Lịch sử` | Badge trạng thái chi tiết, trạng thái duyệt | `_UpdateForm`, `_History` | Có | Nếu có lịch sử thì code ưu tiên nút `Lịch sử`; nút báo cáo nằm ở nhánh `else if` |
| `Views/CongViec/_Table.cshtml` | Nút `Mở lại` | Công việc hoàn thành | POST `CongViec/MoLai` | Không cho công việc, có cho chi tiết | Chỉ áp dụng `CONG_VIEC` |
| `Views/DuAn/_Form.cshtml`, `Views/DuAn/Details.cshtml` | Nút mở lại/tạm dừng dự án | Dự án | POST `MoLaiDuAn`, `TamDungDuAn` | Không áp dụng chi tiết | Chỉ áp dụng `DU_AN` |

Không tìm thấy:

- Nút `Bị cản trở` riêng cho chi tiết.
- Nút `Tạm dừng` riêng cho chi tiết.
- Nút `Mở lại`/`Tiếp tục` riêng cho chi tiết.
- Modal nhập lý do mở lại chi tiết.
- Form POST đổi trạng thái chi tiết trực tiếp trong `ChiTietCongViecController`.

## 12. Permission, role và data scope

| Hành động | Permission/controller | Scope/service | Kết luận |
|---|---|---|---|
| Xem chi tiết công việc | `Permissions.ChiTietCongViec.Xem`; `ChiTietCongViecController.Index` | Service lấy theo `MaCongViec`, không lọc danh sách theo assignee | Ai có quyền xem module xem được danh sách của công việc truyền vào |
| Thêm chi tiết | `Permissions.ChiTietCongViec.Them` | `ChiTietCongViecService.KiemTraQuyenCapNhatAsync` | Admin/Manager hoặc leader/team leader/scope công việc |
| Sửa chi tiết metadata | `Permissions.ChiTietCongViec.Sua` | `CoTheCapNhatAsync`, khóa nếu công việc cha hoàn thành/tạm dừng/đã hủy | Không sửa trạng thái thật |
| Xóa chi tiết | `Permissions.ChiTietCongViec.Xoa` | `CoTheCapNhatAsync`, khóa nếu công việc cha hoàn thành/tạm dừng/đã hủy | Xóa mềm, có sync |
| Phân công chi tiết | `Permissions.PhanCongChiTietCongViec.ThucHien` | `KiemTraQuyenPhanCongAsync`: Admin không thao tác, Manager phải quản lý dự án, leader/team leader được | Người được phân công phải thuộc dự án và đã được phân công công việc cha |
| Cập nhật tiến độ | `Permissions.TienDo.CapNhat` | Không Admin; phải được phân công chi tiết; không có báo cáo chờ duyệt; không bị khóa trạng thái | Nhân viên không được phân công không được cập nhật |
| Duyệt tiến độ | `Permissions.TienDo.Duyet` | Không Admin; không tự duyệt; Manager quản lý dự án/leader/team leader | Manager/Leader có quyền phù hợp mới duyệt |
| Mở lại công việc | Controller gọi `CoQuyenXuLyWorkflowCongViecAsync` | `CongViecService.KiemTraQuyenXuLyTrangThaiCongViecAsync` | Có cho `CONG_VIEC`, không cho `CT_CONG_VIEC` |
| Mở lại chi tiết | Không tìm thấy permission/action/service | Không tìm thấy | Cần quyết định dùng permission hiện có hay thêm permission mới |

Constants liên quan:

- `Permissions.ChiTietCongViec`: `Xem`, `Them`, `Sua`, `Xoa` (`Constants/Permissions.cs`).
- `Permissions.PhanCongChiTietCongViec`: `Xem`, `ThucHien`.
- `Permissions.TienDo`: `Xem`, `CapNhat`, `Duyet`.
- Không tìm thấy `Permissions.ChiTietCongViec.MoLai`.

## 13. Nhật ký và truy vết

| Luồng | Có ghi nhật ký không | Bảng/service | Ghi chú |
|---|---|---|---|
| Thêm/sửa/xóa chi tiết | Không thấy nhật ký chi tiết riêng | Không tìm thấy `NhatKyCongViec` hoặc `NhatKyCtCongViec` được dùng | Chỉ gọi đồng bộ workflow; xóa có `DeletedAt/DeletedBy` |
| Phân công/xóa phân công chi tiết | Có | `NhatKyPhanCongCtCongViec`, `PhanCongChiTietCongViecService.GhiNhatKy` | Ghi hành động thêm/xóa phân công |
| Gửi báo cáo tiến độ | Có bản ghi nghiệp vụ | `TIEN_DO_CONG_VIEC` | `TrangThaiTienDo = ChoDuyet`, có ghi chú/file |
| Duyệt/từ chối/yêu cầu bổ sung báo cáo | Có trên bản ghi tiến độ | `TIEN_DO_CONG_VIEC.MaNguoiDungDuyet`, `ThoiGianDuyet`, `GhiChuDuyet`, `TrangThaiTienDo` | Không có nhật ký riêng ngoài bản ghi tiến độ |
| Đồng bộ rollback công việc từ `ChoXacNhanHoanThanh` | Có điều kiện | `TrangThaiWorkflowService.ThemNhatKyDuAnNeuCanAsync` -> `NhatKyQuanLyDuAn` | Chỉ khi có `maNguoiDungThucHien` |
| Đồng bộ rollback dự án từ `ChoXacNhanHoanThanh` | Có điều kiện | `NhatKyQuanLyDuAn` | Chỉ khi có user |
| Mở lại công việc | Có | `CongViecService.MoLaiCongViecAsync` -> `NhatKyQuanLyDuAn` | Bắt buộc lý do |
| Mở lại dự án | Có | `DuAnService.MoLaiDuAnAsync` -> `NhatKyQuanLyDuAn` | Bắt buộc lý do |
| Mở lại chi tiết | Không tìm thấy | Không tìm thấy | Nếu thêm nên ghi log; có thể dùng `NhatKyQuanLyDuAn` hoặc tạo log chi tiết nếu đã có bảng phù hợp |

Nếu sau này thêm mở lại chi tiết công việc, nên bắt buộc lý do và ghi tối thiểu: mã chi tiết, mã công việc cha, trạng thái cũ, trạng thái mới, người thực hiện, thời gian, lý do. Nếu không thêm bảng mới, có thể ghi vào `NhatKyQuanLyDuAn` vì workflow hiện đã dùng bảng này cho rollback/mở lại công việc/dự án.

## 14. Kết luận nhanh

- Trạng thái đang gây kẹt: `TamDung`, `BiCanCan`, và `HoanThanh` ở cấp `CT_CONG_VIEC`.
- Có chức năng mở lại chi tiết công việc chưa: Không tìm thấy.
- Có chức năng mở lại công việc cha chưa: Có, `CongViecController.MoLai` và `CongViecService.MoLaiCongViecAsync`.
- Có thiếu nút UI không: Có, thiếu nút/modal mở lại/tiếp tục chi tiết ở `Views/ChiTietCongViec` hoặc `Views/TienDoCongViec`.
- Có thiếu action controller không: Có, thiếu action mở lại chi tiết.
- Có thiếu service method không: Có, thiếu method mở lại/tiếp tục chi tiết.
- Có thiếu đồng bộ lên công việc/dự án không: Luồng hiện có gọi sync khi duyệt báo cáo; nhưng vì thiếu mở lại chi tiết nên thiếu điểm gọi sync cho hành động mở lại.
- Có cần sửa database không: Không bắt buộc nếu chỉ thêm mở lại chi tiết với lý do ghi vào `NhatKyQuanLyDuAn` hoặc `GhiChuDuyet/TienDo`; chỉ cần DB nếu muốn bảng nhật ký chi tiết riêng hoặc trường lý do riêng trên `CT_CONG_VIEC`.
- Cần sửa View/Controller/Service/WorkflowService/constants: Có thể cần View, Controller, Service, có thể không cần constants nếu dùng `DangThucHien` sẵn có; WorkflowService có thể giữ nguyên nếu gọi sync sau mở lại.
- Mức độ ưu tiên: Cao, vì trạng thái `TamDung`/`BiCanCan` có thể làm chi tiết và công việc cha kẹt.

## 15. Đề xuất chỉnh sửa sau khi xác nhận

1. Nên thêm chức năng "Mở lại/Tiếp tục chi tiết công việc": Có.
2. Controller nên đặt ở `ChiTietCongViecController`, action POST tên `MoLai` hoặc `TiepTuc`.
3. Service method nên tên `MoLaiChiTietCongViecAsync(int maCongViec, int maChiTietCv, string lyDo)` hoặc `TiepTucChiTietCongViecAsync(...)`.
4. Trạng thái được phép mở lại: tối thiểu `BiCanCan`, `TamDung`; cân nhắc `HoanThanh` nếu nghiệp vụ cho phép.
5. Trạng thái đích: `DangThucHien`.
6. Cần nhập lý do: Có, bắt buộc, vì đây là thao tác lùi/thoát trạng thái đặc biệt.
7. Cần ghi nhật ký: Có, tối thiểu `NhatKyQuanLyDuAn`; tốt hơn nếu có bảng nhật ký chi tiết riêng nhưng không bắt buộc DB nếu dùng bảng hiện có.
8. Cần gọi đồng bộ công việc cha: Có, gọi `DongBoChuoiTrangThaiTuCongViecAsync`.
9. Cần gọi đồng bộ dự án: Có gián tiếp qua `DongBoChuoiTrangThaiTuCongViecAsync`.
10. Cần thêm nút UI: Có, hiển thị ở trạng thái hợp lệ, kèm modal lý do.
11. Cần sửa database: Không bắt buộc cho bản sửa tối thiểu.
12. Cần thêm permission: Có thể dùng `Permissions.ChiTietCongViec.Sua` cho bản tối thiểu; nếu muốn tách quyền, thêm `Permissions.ChiTietCongViec.MoLai` nhưng sẽ kéo theo phân quyền/seed quyền.
13. Cần test phân quyền: Có, đặc biệt Admin/Manager/Leader/Employee được phân công và không được phân công.

Đề xuất scope sửa tối thiểu không DB:

- `IChiTietCongViecService`: thêm method mở lại.
- `ChiTietCongViecService`: validate trạng thái nguồn, quyền, dự án/công việc cha không khóa, set `TrangThaiCTCV = DangThucHien`, `NgayKetThucCTCV = null`, ghi `NhatKyQuanLyDuAn`, gọi sync.
- `ChiTietCongViecController`: thêm POST action.
- `Views/ChiTietCongViec/_Table.cshtml`: thêm nút/modal mở lại ở dòng chi tiết đủ điều kiện.
- `ChiTietCongViecItemViewModel`: thêm cờ `CoTheMoLai` nếu muốn view không tự tính.

## 16. Test case kiểm thử

| Mã test | Màn hình/chức năng | Tình huống | Trạng thái ban đầu | Thao tác | Kết quả mong đợi ở CT_CONG_VIEC | Kết quả mong đợi ở CONG_VIEC | Kết quả mong đợi ở DU_AN | Nhật ký mong đợi | Ghi chú |
|---|---|---|---|---|---|---|---|---|---|
| CTCV-01 | Tiến độ | Chi tiết đang thực hiện -> bị cản trở | `DangThucHien` | Nhân viên gửi báo cáo `BiCanCan`, người có quyền duyệt | `TrangThaiCTCV = BiCanCan` | `TrangThaiCongViec = BiCanCan` nếu sync chạy | Dự án giữ trạng thái hiện tại, cập nhật phần trăm | Record `TIEN_DO_CONG_VIEC` `DaDuyet` | Source hiện hỗ trợ |
| CTCV-02 | Mở lại chi tiết | Chi tiết bị cản trở -> mở lại đang thực hiện | `BiCanCan` | POST mở lại có lý do | `TrangThaiCTCV = DangThucHien` | Nếu không còn blocker: `DangThucHien` hoặc theo chi tiết còn lại | Không tự `TamDung/HoanThanh`; có thể rollback từ `ChoXacNhan` | Nhật ký mở lại | Source hiện chưa có, cần thêm |
| CTCV-03 | Mở lại chi tiết | Chi tiết tạm dừng -> mở lại đang thực hiện | `TamDung` | POST mở lại có lý do | `TrangThaiCTCV = DangThucHien` | Tính lại theo chi tiết | Không tự tạm dừng/hoàn thành | Nhật ký mở lại | Source hiện chưa có; hiện bị khóa báo cáo |
| CTCV-04 | Tiến độ | Chờ xác nhận hoàn thành -> hoàn thành | `ChoXacNhanHoanThanh` | Báo cáo/duyệt `HoanThanh` có file | `TrangThaiCTCV = HoanThanh`, `NgayKetThucCTCV` có giá trị | Nếu tất cả chi tiết hoàn thành: `ChoXacNhanHoanThanh` | Nếu tất cả công việc hoàn thành: `ChoXacNhanHoanThanh` | Record tiến độ `DaDuyet` | Source hiện hỗ trợ qua tiến độ |
| CTCV-05 | Tiến độ | Chờ xác nhận hoàn thành -> từ chối/yêu cầu bổ sung | `ChoXacNhanHoanThanh` | Duyệt báo cáo sang `TuChoi`/`YeuCauBoSung` | Trạng thái thật không đổi theo source hiện tại | Không đổi do không sync | Không đổi | Record tiến độ có `GhiChuDuyet` | Nếu muốn rollback cần sửa rule |
| CTCV-06 | Mở lại chi tiết | Hoàn thành -> mở lại đang thực hiện nếu workflow cho phép | `HoanThanh` | POST mở lại có lý do | `TrangThaiCTCV = DangThucHien`, `NgayKetThucCTCV = null` | Công việc cha tính lại, có thể từ `ChoXacNhan/HoanThanh` về `DangThucHien` nếu không khóa | Dự án tính lại nếu không khóa | Nhật ký mở lại | Source hiện chưa có; cần quyết định có cho không |
| CTCV-07 | Đồng bộ | Một chi tiết bị cản trở thì công việc cha đồng bộ thế nào | Một chi tiết `BiCanCan` | Duyệt báo cáo/sync | Chi tiết giữ `BiCanCan` | `BiCanCan` | Dự án không tự `BiCanCan` | Có record tiến độ | Source hiện có |
| CTCV-08 | Đồng bộ | Tất cả chi tiết hoàn thành thì công việc cha đồng bộ thế nào | Tất cả chi tiết `HoanThanh` | Duyệt chi tiết cuối | Tất cả `HoanThanh` | `ChoXacNhanHoanThanh` | Có thể `ChoXacNhanHoanThanh` nếu mọi công việc hoàn thành | Record tiến độ | Source hiện có |
| CTCV-09 | Đồng bộ | Mở lại chi tiết khiến công việc cha từ bị cản trở về đang thực hiện | Công việc `BiCanCan`, chi tiết blocker `BiCanCan` | Mở lại chi tiết blocker | Chi tiết `DangThucHien` | `DangThucHien` nếu không còn blocker | Giữ hoặc rollback theo rule dự án | Nhật ký mở lại và có thể nhật ký rollback | Source hiện chưa có action |
| CTCV-10 | Đồng bộ dự án | Đồng bộ từ công việc lên dự án sau khi trạng thái chi tiết thay đổi | Dự án theo trạng thái công việc | Duyệt báo cáo/mở lại | Chi tiết đổi thật | Công việc tính lại | Dự án cập nhật phần trăm, có thể `ChoXacNhan`/rollback `DangThucHien` | Nhật ký nếu rollback dự án | Source hiện có trong workflow sync |
| CTCV-11 | Permission | Nhân viên không được phân công không được đổi trạng thái | Bất kỳ không khóa | Gửi báo cáo | Không đổi | Không đổi | Không đổi | Không có record mới | Source hiện chặn bằng `PhanCongCtCongViec` |
| CTCV-12 | Permission | Manager/Leader có quyền phù hợp mới được duyệt hoặc mở lại | Báo cáo `ChoDuyet` | Duyệt/mở lại | Chỉ đổi nếu có scope | Sync nếu đổi | Sync nếu đổi | Record duyệt/nhật ký | Source hiện có cho duyệt; mở lại chi tiết cần thêm |
| CTCV-13 | Khóa trạng thái cha | Dự án đã hoàn thành/đã hủy thì không cho đổi chi tiết nếu source có rule này | Dự án `HoanThanh/DaHuy/LuuTru/TamDung` | Gửi/duyệt báo cáo | Không đổi | Không đổi | Không đổi | Không có | Source hiện khóa trong `BiKhoaCapNhatTheoTrangThai` |
| CTCV-14 | Nhật ký | Kiểm tra ghi nhật ký khi đổi trạng thái | Bất kỳ | Duyệt đổi trạng thái/mở lại | Đổi theo thao tác | Sync | Sync | Hiện chỉ có record tiến độ; mở lại đề xuất có log | Cần bổ sung nếu muốn log riêng |
| CTCV-15 | UI | Nút mở lại đúng trạng thái và ẩn ở trạng thái không hợp lệ | `BiCanCan`, `TamDung`, `HoanThanh`, `DangThucHien` | Xem danh sách chi tiết | N/A | N/A | N/A | N/A | Source hiện thiếu nút; test sau khi sửa |

## 17. Checklist thông tin cần gửi lại cho ChatGPT

1. File controller chi tiết công việc: `QuanLyDuAn/QuanLyDuAn/Controllers/ChiTietCongViecController.cs`.
2. File service chi tiết công việc: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChiTietCongViecService.cs`.
3. File workflow service đồng bộ trạng thái: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TrangThaiWorkflowService.cs`.
4. File constants trạng thái: `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs`.
5. Entity `CT_CONG_VIEC`: `QuanLyDuAn/QuanLyDuAn/Models/Entities/CtCongViec.cs`.
6. Entity `CONG_VIEC`: `QuanLyDuAn/QuanLyDuAn/Models/Entities/CongViec.cs`.
7. Entity `DU_AN`: `QuanLyDuAn/QuanLyDuAn/Models/Entities/DuAn.cs`.
8. Các trạng thái hiện có của chi tiết công việc: `ChuaBatDau`, `DangThucHien`, `BiCanCan`, `ChoXacNhanHoanThanh`, `HoanThanh`, `TamDung`, `DaHuy`.
9. Các action hiện có để đổi trạng thái chi tiết công việc: không có action trực tiếp; đổi qua `TienDoCongViecController.CapNhatTienDo` + `DuyetBaoCaoTienDo`.
10. Có/không chức năng mở lại chi tiết công việc: Không tìm thấy.
11. Có/không chức năng mở lại công việc cha: Có, `CongViecController.MoLai` và `CongViecService.MoLaiCongViecAsync`.
12. Method đồng bộ từ chi tiết lên công việc: `TrangThaiWorkflowService.DongBoTrangThaiCongViecTheoChiTietAsync`.
13. Method đồng bộ từ công việc lên dự án: `TrangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync`.
14. View hiện có nút bị cản trở/tạm dừng/mở lại không: `Views/ChiTietCongViec` không có; `Views/TienDoCongViec/_UpdateForm.cshtml` có dropdown đề xuất trạng thái nhưng không có mở lại riêng.
15. Permission nào dùng cho cập nhật/duyệt/mở lại: cập nhật chi tiết CRUD dùng `ChiTietCongViec.*`; cập nhật tiến độ dùng `TienDo.CapNhat`; duyệt dùng `TienDo.Duyet`; mở lại công việc dùng workflow công việc hiện có; mở lại chi tiết chưa có permission.
16. Nhật ký nào được ghi khi đổi trạng thái: đổi trạng thái qua tiến độ lưu trong `TIEN_DO_CONG_VIEC`; sync rollback có thể ghi `NhatKyQuanLyDuAn`; không có nhật ký chi tiết riêng.
17. Kết luận có cần sửa database không: Không bắt buộc cho hướng sửa tối thiểu.
18. Kết luận nên sửa View/Controller/Service/WorkflowService nào: sửa `ChiTietCongViecController`, `IChiTietCongViecService`, `ChiTietCongViecService`, `Views/ChiTietCongViec/_Table.cshtml`; có thể giữ `TrangThaiWorkflowService` và chỉ gọi sync.
19. Quy tắc đồng bộ đề xuất: chi tiết đổi thật -> sync công việc -> sync dự án có giới hạn; không tự đổi dự án sang `TamDung/BiCanCan/HoanThanh`.
20. Test case quan trọng nhất cần chạy sau khi sửa: CTCV-02, CTCV-03, CTCV-07, CTCV-09, CTCV-11, CTCV-12.
