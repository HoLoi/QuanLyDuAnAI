# Phân tích AS-IS chức năng Chi tiết công việc

Tài liệu này chỉ ghi nhận cách chức năng **Chi tiết công việc** đang được triển khai trong source hiện tại. Không có đề xuất patch, không thay đổi route, workflow, permission, database hoặc giao diện.

## 1. Tổng quan chức năng hiện tại

### AS-IS

- Chi tiết công việc được lưu ở entity `CtCongViec`, bảng `CT_CONG_VIEC`, và luôn thuộc một Công việc qua `MaCongViec`.
- Công việc (`CongViec`) thuộc Danh mục công việc qua `MaDanhMucCV`; Danh mục công việc (`DanhMucCongViec`) thuộc Dự án qua `MaDuAn`.
- Màn hình chính của Chi tiết công việc là `ChiTietCongViecController.Index(int maCongViec, ...)`, route mặc định `/ChiTietCongViec/Index?maCongViec=...`.
- Quyền controller:
  - Xem: `Permissions.ChiTietCongViec.Xem`.
  - Thêm: `Permissions.ChiTietCongViec.Them`.
  - Sửa: `Permissions.ChiTietCongViec.Sua`.
  - Xóa: `Permissions.ChiTietCongViec.Xoa`.
- Ngoài permission, service còn kiểm tra scope trong `ChiTietCongViecService.CoTheCapNhatAsync`:
  - `Admin` hoặc `Manager`: được cập nhật.
  - `Employee`: chỉ được cập nhật khi là leader team hoặc leader dự án.
  - Role khác có thể cập nhật nếu là leader dự án hoặc được phân công Công việc cha.
- Người được phân công thực hiện Chi tiết công việc được quản lý qua `PHAN_CONG_CT_CONG_VIEC`. `PhanCongChiTietCongViecService.KiemTraNhanVienHopLeAsync` yêu cầu nhân viên thuộc dự án và đã được phân công Công việc cha.
- Trong workflow tổng thể, Chi tiết công việc là bước sau Công việc cha: từ danh sách Công việc có link sang `/ChiTietCongViec/Index`, từ Chi tiết công việc có link sang phân công chi tiết, rồi nhân viên được phân công cập nhật tiến độ tại module `TienDoCongViec`.

## 2. Cấu trúc dữ liệu Chi tiết công việc

Nguồn: `Models/Entities/CtCongViec.cs`, `Data/QuanLyDuAnDbContext.cs`, migration `20260527125053_Init.cs`.

| Trường | Kiểu C# | Kiểu migration | Null | Ý nghĩa AS-IS | Mặc định trong code |
| --- | --- | --- | --- | --- | --- |
| `MaChiTietCV` | `int` | `int identity` | Không | Khóa chính bảng `CT_CONG_VIEC` | SQL identity |
| `MaCongViec` | `int` | `int` | Không | FK tới `CONG_VIEC.MaCongViec` | Không có |
| `TenCTCV` | `string?` | `nvarchar(max)` | Có | Tên ngắn chi tiết công việc | Khi thêm/sửa nếu rỗng thì lưu `null` |
| `NoiDungChiTietCV` | `string?` | `nvarchar(max)` | Có | Nội dung chi tiết cần làm | Form/service bắt buộc nhập, tối đa 255 ký tự ở validate service |
| `NgayTaoCTCV` | `DateTime?` | `datetime2` | Có | Thời điểm tạo | `DateTime.Now` trong `ChiTietCongViecService.AddAsync` |
| `NgayBatDauCTCV` | `DateTime?` | `datetime2` | Có | Ngày bắt đầu chi tiết | Form bắt buộc; service lưu `.Date` |
| `NgayKetThucCTCV` | `DateTime?` | `datetime2` | Có | Ngày kết thúc thực tế theo trạng thái | Set `DateTime.Now` khi trạng thái là hoàn thành, ngược lại `null` |
| `TrangThaiCTCV` | `string?` | `nvarchar(50)` | Có | Trạng thái nghiệp vụ của chi tiết công việc | Form mặc định `ChuaBatDau`; service lưu từ form |
| `IsDeleted` | `bool?` | `bit` | Có | Soft delete | Thêm mới set `false`; xóa set `true` |
| `DeletedAt` | `DateTime?` | `datetime2` | Có | Thời điểm xóa mềm | Set `DateTime.Now` khi xóa |
| `DeletedBy` | `int?` | `int` | Có | Người xóa mềm | Set current user khi xóa |

Quan hệ:

- `CT_CONG_VIEC.MaCongViec` -> `CONG_VIEC.MaCongViec`, constraint `FK_CT_CONG__CO_CONG_VIE`.
- `CT_CONG_VIEC.DeletedBy` -> `NGUOI_DUNG.MaNguoiDung`, constraint `FK_CT_CONG_VIEC_DELETED_BY`.
- `PHAN_CONG_CT_CONG_VIEC.MaChiTietCV` -> `CT_CONG_VIEC.MaChiTietCV`.
- `TIEN_DO_CONG_VIEC.MaChiTietCV` -> `CT_CONG_VIEC.MaChiTietCV`.
- `FILE_CT_CONG_VIEC.MaChiTietCV` -> `CT_CONG_VIEC.MaChiTietCV`, nhưng không tìm thấy controller/service đang dùng `FileCtCongViec` trong source đã đọc.

Không có trường phần trăm hoàn thành trong `CT_CONG_VIEC`. Phần trăm hiển thị của tiến độ được tính theo trạng thái trong `TienDoCongViecService.TinhPhanTramTheoTrangThai`.

Không có trường `NguoiTao`, `NguoiSua`, `NgaySua` trong `CtCongViec`.

## 3. Luồng hiển thị danh sách

### AS-IS

- Controller: `ChiTietCongViecController.Index`.
- Service: `ChiTietCongViecService.GetPageAsync`.
- Điều kiện lọc chính: `CtCongViec.MaCongViec == maCongViec && IsDeleted != true`.
- Có lọc soft delete.
- Không lọc chi tiết theo người được phân công ở màn `ChiTietCongViec`; scope cập nhật chỉ ảnh hưởng `CoTheCapNhat` và quyền thao tác.
- Service lấy Công việc cha bằng `LayCongViecAsync`, sau đó tính:
  - `CoTheCapNhat` từ `CoTheCapNhatAsync` và trạng thái Công việc cha không bị khóa.
  - `CoThePhanCongChiTietCongViec` từ `CoThePhanCongChiTietCongViecAsync`.
  - `TongSoChiTiet`, `SoChiTietHoanThanh`, `PhanTramTienDo`.
- Danh sách sắp xếp `NgayTaoCTCV` giảm dần, rồi `MaChiTietCV` giảm dần.
- Các trường hiển thị trong bảng: tên CTCV, nội dung, ngày bắt đầu, ngày kết thúc, ngày tạo, trạng thái, số người được phân công, thao tác.
- Các nút:
  - `Phân công`: cần `Permissions.PhanCongChiTietCongViec.Xem` và `item.CoThePhanCongChiTietCongViec`.
  - `Sửa`: cần `Permissions.ChiTietCongViec.Sua` và `Model.CoTheCapNhat`.
  - `Xóa`: cần `Permissions.ChiTietCongViec.Xoa` và `Model.CoTheCapNhat`.
  - Form thêm: cần `Permissions.ChiTietCongViec.Them` và `Model.CoTheCapNhat`.

## 4. Luồng tạo Chi tiết công việc

`View -> Controller -> Service -> Validate -> Ghi database -> Đồng bộ trạng thái -> Redirect/View`

### AS-IS

- View: `Views/ChiTietCongViec/_Form.cshtml`.
- Controller POST: `ChiTietCongViecController.Them`.
- Service: `ChiTietCongViecService.AddAsync`.

Quyền tạo:

- Controller yêu cầu `Permissions.ChiTietCongViec.Them`.
- Service yêu cầu `KiemTraQuyenCapNhatAsync`, dựa trên `CoTheCapNhatAsync`.

Điều kiện Công việc/Dự án:

- `KiemTraTrangThaiCongViecTruocKhiThemAsync` chặn nếu Công việc cha đã hoàn thành, `TamDung`, `DaHuy`.
- Hàm này cũng lấy Dự án qua Danh mục công việc và chặn nếu Dự án đã hoàn thành, `Archived`/`LuuTru`, hoặc `DaHuy`.
- Không chặn rõ `ChoXacNhanHoanThanh` ở hàm tạo; nhưng UI có banner nhắc khi Công việc cha đang `ChoXacNhanHoanThanh`.

Ngày bắt đầu/kết thúc:

- Form tạo chỉ có `NgayBatDauCTCV`, không có input `NgayKetThucCTCV`.
- `KiemTraDuLieuDauVao` bắt buộc `NgayBatDauCTCV`.
- Ngày bắt đầu chi tiết không được trước `CongViec.NgayBatDauCongViec`.
- Ngày bắt đầu chi tiết không được sau `CongViec.NgayKetThucCVDuKien`.
- Không có ngày kết thúc dự kiến riêng cho chi tiết trên form.
- `NgayKetThucCTCV` được set `DateTime.Now` nếu trạng thái tạo là hoàn thành; ngược lại `null`.

Trạng thái khi tạo:

- `GetPageAsync` khởi tạo form mặc định `TrangThaiCTCV = TrangThai.ChuaBatDau`.
- `_Form.cshtml` có dropdown trạng thái gồm `ChuaBatDau`, `DangThucHien`, `BiCanCan`, `TamDung`, `HoanThanh`.
- `AddAsync` lấy trạng thái từ form: `TrangThai.ToCode(form.TrangThaiCTCV)`.
- `KiemTraDuLieuDauVao` chỉ chấp nhận 5 trạng thái trên, không chấp nhận `ChoXacNhanHoanThanh`/`DaHuy` khi tạo trực tiếp từ màn này.
- Người dùng có thể chọn `HoanThanh` ngay khi tạo nếu có quyền thêm và scope cập nhật.
- Không có tự động phân công người tạo hoặc nhân viên nào trong `AddAsync`.

Ghi DB và đồng bộ:

- `AddAsync` thêm `CtCongViec`, gọi `SaveChangesAsync`.
- Sau đó gọi `ITrangThaiWorkflowService.DongBoChuoiTrangThaiTuCongViecAsync(congViec.MaCongViec, currentUserId, "Thêm chi tiết công việc")`.
- Sau đồng bộ gọi `SaveChangesAsync` lần nữa.
- Không có transaction bao quanh cả insert và đồng bộ.

Trường hợp vừa tạo `ChuaBatDau`:

- `TrangThaiWorkflowService.TinhTrangThaiCongViecTheoChiTiet` trả `ChuaBatDau` nếu tất cả chi tiết đều `ChuaBatDau`.
- Nhánh `coTienTrien` chỉ true khi có `DangThucHien`, `ChoXacNhanHoanThanh`, `TamDung`, `DaHuy`, hoặc trạng thái khác `ChuaBatDau`.
- Vì vậy, khi vừa tạo một hoặc nhiều Chi tiết công việc và tất cả đều `ChuaBatDau`, Công việc cha không bị chuyển sang `DangThucHien` bởi nhánh mặc định.

## 5. Luồng sửa Chi tiết công việc

### AS-IS

- Controller POST: `ChiTietCongViecController.Sua`.
- Service: `ChiTietCongViecService.UpdateAsync`.
- Quyền: controller cần `Permissions.ChiTietCongViec.Sua`; service cần `KiemTraQuyenCapNhatAsync`.
- Trạng thái Công việc cha: `KiemTraTrangThaiCongViecChoCapNhat(congViec, "sửa")` chặn nếu Công việc cha đã hoàn thành, `TamDung`, `DaHuy`.
- Không kiểm tra trạng thái Dự án trong `UpdateAsync` ngoài scope cập nhật; chỉ `AddAsync` có kiểm tra Dự án đã hoàn thành/đóng.

Trường được sửa:

- `TenCTCV`.
- `NoiDungChiTietCV`.
- `NgayBatDauCTCV`.
- `TrangThaiCTCV`.

Không sửa:

- `MaCongViec` chỉ dùng làm khóa tìm.
- `NgayTaoCTCV`.
- `NgayKetThucCTCV` không nhập trực tiếp; bị tính lại theo trạng thái.
- Không có `NguoiSua`, `NgaySua`.

Trạng thái khi sửa:

- `_Table.cshtml` render form sửa inline với dropdown gồm `ChuaBatDau`, `DangThucHien`, `BiCanCan`, `TamDung`, `HoanThanh`.
- `UpdateAsync` lưu trực tiếp `TrangThai.ToCode(form.TrangThaiCTCV)`.
- Nếu trạng thái sửa là hoàn thành thì set `NgayKetThucCTCV = DateTime.Now`; nếu không thì set `null`.
- Không kiểm tra chi tiết đã được phân công hay đã có báo cáo tiến độ trước khi cho sửa.
- Không kiểm tra không được lùi trạng thái trong `ChiTietCongViecService.UpdateAsync`.

Đồng bộ:

- Sau khi lưu chi tiết, service gọi `DongBoChuoiTrangThaiTuCongViecAsync`.
- Không có transaction bao trùm lưu chi tiết và đồng bộ.

### Rủi ro

- Nếu trạng thái thật đã được cập nhật qua báo cáo tiến độ đã duyệt, form sửa vẫn có thể ghi đè trực tiếp `TrangThaiCTCV` vì `UpdateAsync` không kiểm tra lịch sử `TIEN_DO_CONG_VIEC`.

## 6. Luồng xóa Chi tiết công việc

### AS-IS

- Controller POST: `ChiTietCongViecController.Xoa`.
- Service: `ChiTietCongViecService.RemoveAsync`.
- Xóa mềm: set `IsDeleted = true`, `DeletedAt = DateTime.Now`, `DeletedBy = currentUserId`.
- Quyền: controller cần `Permissions.ChiTietCongViec.Xoa`; service cần `KiemTraQuyenCapNhatAsync`.
- Điều kiện trạng thái: chặn nếu Công việc cha đã hoàn thành, `TamDung`, `DaHuy`.
- Không kiểm tra Chi tiết đã phân công hay đã có báo cáo tiến độ/file liên quan trước khi xóa mềm.
- Sau xóa gọi `DongBoChuoiTrangThaiTuCongViecAsync`.
- Nếu xóa chi tiết cuối cùng, `TinhTrangThaiCongViecTheoChiTiet` nhận danh sách rỗng và trả `ChuaBatDau`; Công việc cha có thể được đồng bộ về `ChuaBatDau` nếu không đang ở trạng thái khóa.

## 7. Trạng thái Chi tiết công việc

### Trạng thái thấy trong source

Nguồn chính: `TrangThai.cs`, `ChiTietCongViecService.KiemTraDuLieuDauVao`, `TienDoCongViecService.KiemTraTrangThaiChiTietHopLe`, `LayDanhSachTrangThaiDeXuatCoTheChon`, views.

| Trạng thái | Ý nghĩa hiển thị | Gán ở đâu | Chuyển đến từ đâu | Ghi chú |
| --- | --- | --- | --- | --- |
| `ChuaBatDau` | Chưa bắt đầu | Form tạo/sửa Chi tiết; báo cáo tiến độ đề xuất/duyệt; mặc định form | Tạo trực tiếp, sửa trực tiếp, báo cáo được duyệt | TienDo coi thứ tự 0 |
| `DangThucHien` | Đang thực hiện | Form tạo/sửa; báo cáo tiến độ được duyệt | Tạo/sửa trực tiếp hoặc báo cáo | TienDo coi thứ tự 1 |
| `BiCanCan` | Bị cản trở | Form tạo/sửa; báo cáo tiến độ được duyệt | Tạo/sửa trực tiếp hoặc báo cáo | Khi có chi tiết `BiCanCan`, Công việc cha thành `BiCanCan` |
| `ChoXacNhanHoanThanh` | Chờ xác nhận hoàn thành | Chỉ có trong tiến độ đề xuất/duyệt, không có ở dropdown tạo/sửa Chi tiết | Báo cáo tiến độ | Bắt buộc file minh chứng khi gửi/duyệt báo cáo |
| `HoanThanh` | Hoàn thành | Form tạo/sửa; báo cáo tiến độ được duyệt | Tạo/sửa trực tiếp hoặc báo cáo | Set `NgayKetThucCTCV = DateTime.Now` |
| `TamDung` | Tạm dừng | Form tạo/sửa; báo cáo tiến độ được duyệt | Tạo/sửa trực tiếp hoặc báo cáo | Khóa cập nhật tiến độ/phân công theo một số service |
| `DaHuy` | Đã hủy | Chỉ có trong tiến độ đề xuất/duyệt, không có dropdown tạo/sửa Chi tiết | Báo cáo tiến độ | Khóa cập nhật tiến độ/phân công |

State machine:

- Không có state machine tập trung cho Chi tiết công việc.
- Có helper normalize/display trong `TrangThai.cs`.
- Có kiểm tra hợp lệ rải rác:
  - `ChiTietCongViecService.KiemTraDuLieuDauVao` cho tạo/sửa trực tiếp.
  - `TienDoCongViecService.KiemTraTrangThaiChiTietHopLe`, `LayThuTuTrangThaiChiTiet`, `KiemTraKhongDuocLuiTrangThai` cho báo cáo tiến độ.
  - `PhanCongChiTietCongViecService.KiemTraTrangThaiChiTietCongViec` để khóa phân công.

## 8. Quan hệ giữa thời gian và trạng thái

### AS-IS

- Không tìm thấy background service, scheduled job, middleware hoặc logic tải trang tự động cập nhật `TrangThaiCTCV` theo ngày.
- `Program.cs` không đăng ký `AddHostedService`; `CauHinhDichVu.cs` chỉ đăng ký scoped services.
- `NgayBatDauCTCV` được dùng để validate khi tạo/sửa và hiển thị.
- Không có logic “trước ngày bắt đầu tự gán `ChuaBatDau`”.
- Không có logic “đến ngày bắt đầu tự chuyển `DangThucHien`”.
- Không có logic “qua ngày kết thúc tự chuyển trạng thái”.
- Không tìm thấy trạng thái/nhãn `QuaHan` riêng cho Chi tiết công việc trong module `ChiTietCongViec`/`TienDoCongViec`.
- `TrangThai.TreTienDo = "Tre"` tồn tại trong constants, nhưng không được dùng làm `TrangThaiCTCV` trong luồng Chi tiết công việc đã đọc.
- `NgayKetThucCTCV` hiện là ngày kết thúc thực tế được set khi trạng thái là hoàn thành, không phải hạn dự kiến nhập từ người dùng.

Phân biệt:

- Trạng thái nghiệp vụ lưu DB: `CT_CONG_VIEC.TrangThaiCTCV`.
- Tình trạng thời gian: chưa có nhãn quá hạn riêng cho Chi tiết công việc trong màn này.
- Trạng thái cập nhật do báo cáo tiến độ được duyệt: `TienDoCongViecService.XuLyBaoCaoTienDoAsync` set `contextInfo.ChiTietCongViec.TrangThaiCTCV = trangThaiDeXuat` khi `trangThaiDich == DaDuyet`.

## 9. Báo cáo tiến độ và trạng thái thật

Luồng:

`Nhân viên gửi báo cáo -> ChoDuyet -> Duyệt/Từ chối/Yêu cầu bổ sung -> Cập nhật CT_CONG_VIEC`

### AS-IS

- Controller: `TienDoCongViecController.CapNhatTienDo`, `DuyetBaoCaoTienDo`, `YeuCauBoSungBaoCaoTienDo`, `TuChoiBaoCaoTienDo`.
- Service: `TienDoCongViecService`.
- Báo cáo mới không cập nhật trực tiếp `CT_CONG_VIEC.TrangThaiCTCV`.
- `CapNhatTienDoAsync` tạo record `TienDoCongViec` với:
  - `TrangThaiCTCVDeXuat = TrangThai.ToCode(form.TrangThaiCTCVMoi)`.
  - `TrangThaiTienDo = TrangThai.ChoDuyet`.
  - `PhanTram` tính từ trạng thái đề xuất.
- Người gửi báo cáo chọn `TrangThaiCTCVMoi` từ `_UpdateForm.cshtml`.
- Danh sách trạng thái đề xuất do `LayDanhSachTrangThaiDeXuatCoTheChon` tạo: `ChuaBatDau`, `DangThucHien`, `BiCanCan`, `ChoXacNhanHoanThanh`, `HoanThanh`, `TamDung`, `DaHuy`, lọc theo thứ tự không lùi.
- Có kiểm tra không lùi trạng thái trong `KiemTraKhongDuocLuiTrangThai`.
- Có chặn báo cáo rỗng nếu trạng thái đề xuất không đổi, không có ghi chú, không có file.
- Khi đề xuất `ChoXacNhanHoanThanh` hoặc `HoanThanh`, `KiemTraFileMinhChungHopLe` bắt buộc file khi gửi; `KiemTraFileMinhChungTonTaiTheoDeXuat` kiểm tra lại khi duyệt.
- Chỉ khi báo cáo được xử lý với `TrangThai.DaDuyet`, `XuLyBaoCaoTienDoAsync` mới:
  - set `TienDo.TrangThaiTienDo = DaDuyet`;
  - set người duyệt/thời gian duyệt;
  - set `CT_CONG_VIEC.TrangThaiCTCV = trangThaiDeXuat`;
  - set `NgayKetThucCTCV = DateTime.Now` nếu hoàn thành, ngược lại `null`;
  - gọi `DongBoChuoiTrangThaiTuCongViecAsync`.
- Từ chối hoặc yêu cầu bổ sung chỉ đổi `TrangThaiTienDo`, người duyệt, thời gian duyệt, ghi chú duyệt; không cập nhật `CT_CONG_VIEC`.
- Admin bị chặn thao tác nghiệp vụ tiến độ bởi `KiemTraKhongChoAdminTacNghiep`.

## 10. Phân công Chi tiết công việc

### AS-IS

- Controller: `PhanCongChiTietCongViecController`.
- Service: `PhanCongChiTietCongViecService`.
- Bảng: `PHAN_CONG_CT_CONG_VIEC`, khóa chính kép `{MaNguoiDung, MaChiTietCV}`.
- Một Chi tiết công việc có thể phân công cho nhiều người vì khóa chính gồm người dùng + chi tiết.
- Người có quyền thực hiện phân công:
  - Controller cần `Permissions.PhanCongChiTietCongViec.ThucHien`.
  - Service `KiemTraQuyenPhanCongAsync`:
    - Admin: trả `false`, không được phân công theo service.
    - Manager: được nếu là quản lý dự án (`DuAn.MaNguoiDung == currentUserId`).
    - Leader dự án hoặc leader team thuộc dự án: được.
    - Member thường: không được.
- Nhân viên được phân công phải:
  - tồn tại và chưa xóa;
  - tài khoản không bị khóa;
  - thuộc dự án (`NhanVienDuAn`);
  - đã được phân công Công việc cha (`PhanCongCongViec`).
- Thêm phân công không thay đổi `TrangThaiCTCV`.
- Gỡ phân công không thay đổi `TrangThaiCTCV`.
- Có nhật ký phân công: `NhatKyPhanCongCtCongViec`, ghi trong `GhiNhatKy`.
- Nhân viên chưa được phân công không được cập nhật tiến độ vì `TienDoCongViecService.CapNhatTienDoAsync` kiểm tra `PhanCongCtCongViec.Any(...)`.

## 11. Đồng bộ trạng thái lên Công việc cha

Nguồn: `TrangThaiWorkflowService.DongBoTrangThaiCongViecTheoChiTietAsync`, `TinhTrangThaiCongViecTheoChiTiet`, `DongBoChuoiTrangThaiTuCongViecAsync`.

### AS-IS

- `DongBoChuoiTrangThaiTuCongViecAsync` gọi:
  1. `DongBoTrangThaiCongViecTheoChiTietAsync(maCongViec, ...)`;
  2. lấy `MaDuAn` theo Công việc;
  3. `DongBoTrangThaiDuAnTheoCongViecAsync(maDuAn, ...)`.
- `DongBoTrangThaiCongViecTheoChiTietAsync` bỏ qua nếu Công việc hiện tại đã hoàn thành, `TamDung`, hoặc `DaHuy`.
- Danh sách chi tiết đưa vào tính toán chỉ gồm `CtCongViec` cùng `MaCongViec` và `IsDeleted != true`.

Quy tắc `TinhTrangThaiCongViecTheoChiTiet`:

| Trường hợp | Trạng thái Công việc cha |
| --- | --- |
| Không có Chi tiết công việc | `ChuaBatDau` |
| Tất cả Chi tiết là hoàn thành (`HoanThanh`, `Done`, `Completed`, hiển thị tương đương) | `ChoXacNhanHoanThanh` |
| Có ít nhất một Chi tiết `BiCanCan` | `BiCanCan` |
| Có ít nhất một Chi tiết `DangThucHien` | `DangThucHien` |
| Có ít nhất một Chi tiết `ChoXacNhanHoanThanh` | `DangThucHien` |
| Có ít nhất một Chi tiết `TamDung` | `DangThucHien` |
| Có ít nhất một Chi tiết `DaHuy` | `DangThucHien` |
| Có trạng thái khác `ChuaBatDau` không thuộc các nhánh trước | `DangThucHien` |
| Tất cả Chi tiết `ChuaBatDau` | `ChuaBatDau` |
| Hỗn hợp `HoanThanh` và `ChuaBatDau` | `DangThucHien` |

Ngày kết thúc Công việc:

- Nếu trạng thái mới là `ChoXacNhanHoanThanh` hoặc hoàn thành, set `CongViec.NgayKetThucCVThucTe = DateTime.Now` nếu chưa có.
- Nếu trạng thái mới không phải các trạng thái trên, set `NgayKetThucCVThucTe = null`.

Rollback:

- Nếu Công việc đang `ChoXacNhanHoanThanh` nhưng trạng thái tính lại không còn `ChoXacNhanHoanThanh`, service cho rollback sang trạng thái mới và có thể ghi nhật ký dự án.
- Nếu Công việc đã `HoanThanh`, `TamDung`, `DaHuy`, service không đồng bộ ngược.

Kiểm tra đặc biệt:

- Khi vừa tạo một hoặc nhiều Chi tiết công việc và tất cả đều `ChuaBatDau`, method `TinhTrangThaiCongViecTheoChiTiet` trả `ChuaBatDau`, không chuyển Công việc cha sang `DangThucHien`.

## 12. Đồng bộ trạng thái lên Dự án

Nguồn: `TrangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync`.

### AS-IS

- Khi `DongBoChuoiTrangThaiTuCongViecAsync` chạy, sau Công việc sẽ đồng bộ Dự án.
- Service lấy tất cả Công việc chưa xóa thuộc các Danh mục công việc chưa xóa của Dự án.
- Luôn tính lại `duAn.PhanTramHoanThanh = TinhPhanTramDuAnTheoCongViec(trangThaiCongViec)` trước khi kiểm tra trạng thái khóa.
- Nếu Dự án hiện tại đã hoàn thành, `Archived`, `DaHuy`, hoặc `TamDung`, service không đổi `TrangThaiDuAn`.
- Dự án chuyển lên `ChoXacNhanHoanThanh` khi tất cả Công việc thuộc Dự án đều là hoàn thành theo `TrangThai.LaHoanThanhCongViec`.
- Dự án rollback từ `ChoXacNhanHoanThanh` về `DangThucHien` khi không còn tất cả Công việc hoàn thành.
- Dự án đã `HoanThanh` không bị tự mở lại bởi service này.
- Việc tạo Chi tiết công việc có thể gián tiếp đổi Dự án nếu nó làm Công việc cha đổi trạng thái và kết quả toàn bộ Công việc của Dự án thay đổi. Tuy nhiên nếu chỉ tạo `ChuaBatDau` dưới một Công việc chưa khóa, Công việc cha được tính `ChuaBatDau`; Dự án chỉ bị ảnh hưởng theo quy tắc chung của các Công việc.

## 13. Kiểm tra giao diện tạo và sửa

### Form tạo

Nguồn: `Views/ChiTietCongViec/_Form.cshtml`.

- Trường `TenCTCV`, text, tối đa 255.
- Trường `NoiDungChiTietCV`, text, tối đa 255, `required`.
- Trường `NgayBatDauCTCV`, date, `required`.
- Trường `TrangThaiCTCV`, dropdown, `required`.
- Dropdown gồm: `ChuaBatDau`, `DangThucHien`, `BiCanCan`, `TamDung`, `HoanThanh`.
- Không có trường `NgayKetThucCTCV`.
- Không có cảnh báo client-side về ngày nằm ngoài thời gian Công việc cha; server-side có kiểm tra trong `KiemTraDuLieuDauVao`.
- Không có nhãn thời gian “Chưa đến hạn”, “Đang trong thời gian”, “Quá hạn” trong form này.
- Có nguy cơ người dùng tự chọn `HoanThanh` khi tạo mới vì dropdown có `HoanThanh` và service chấp nhận.

### Form sửa

Nguồn: `Views/ChiTietCongViec/_Table.cshtml`.

- Form sửa inline trong table desktop và mobile.
- Trường sửa: `TenCTCV`, `NoiDungChiTietCV`, `NgayBatDauCTCV`, `TrangThaiCTCV`.
- Dropdown sửa giống form tạo: `ChuaBatDau`, `DangThucHien`, `BiCanCan`, `TamDung`, `HoanThanh`.
- Không có field nhập `NgayKetThucCTCV`.
- Không có client-side custom validation ngoài thuộc tính HTML.
- Server-side dùng lại `KiemTraDuLieuDauVao`.

## 14. Permission và data scope

### Permission

Nguồn: `Constants/Permissions.cs`, controllers.

| Nghiệp vụ | Permission |
| --- | --- |
| Xem Chi tiết công việc | `ChiTietCongViec.Xem` |
| Thêm Chi tiết công việc | `ChiTietCongViec.Them` |
| Sửa Chi tiết công việc | `ChiTietCongViec.Sua` |
| Xóa Chi tiết công việc | `ChiTietCongViec.Xoa` |
| Xem phân công Chi tiết công việc | `PhanCongChiTietCongViec.Xem` |
| Thực hiện phân công Chi tiết công việc | `PhanCongChiTietCongViec.ThucHien` |
| Xem tiến độ | `TienDo.Xem` |
| Cập nhật tiến độ | `TienDo.CapNhat` |
| Duyệt tiến độ | `TienDo.Duyet` |

Seed quyền mặc định:

- Manager có `ChiTietCongViec.Xem`, `PhanCongChiTietCongViec.Xem`, `PhanCongChiTietCongViec.ThucHien`, `TienDo.Xem`, `TienDo.Duyet`.
- Employee có `ChiTietCongViec.Xem/Them/Sua/Xoa`, `PhanCongChiTietCongViec.Xem/ThucHien`, `TienDo.Xem/CapNhat`.
- Permission dependency provider khai báo nhóm quyền Chi tiết công việc và phân công chi tiết.

### Data scope

- Chi tiết công việc:
  - Admin/Manager được `CoTheCapNhatAsync`.
  - Employee chỉ được cập nhật nếu là leader team hoặc leader dự án.
  - Role khác có thể được nếu là leader dự án hoặc được phân công Công việc cha.
- Phân công chi tiết:
  - Admin bị service chặn nghiệp vụ phân công.
  - Manager chỉ được nếu là quản lý dự án.
  - Leader dự án hoặc leader team được.
  - Member thường không được.
- Tiến độ:
  - Admin xem được nhưng bị chặn cập nhật/duyệt nghiệp vụ.
  - Manager thấy chi tiết thuộc dự án mình quản lý hoặc chi tiết mình được phân công.
  - Employee thấy chi tiết thuộc dự án mình leader/team leader hoặc chi tiết mình được phân công.
  - Cập nhật tiến độ yêu cầu người dùng được phân công trực tiếp vào `PHAN_CONG_CT_CONG_VIEC`.
  - Duyệt tiến độ yêu cầu scope dự án quản lý/leader/team leader và không được tự duyệt báo cáo của chính mình.

## 15. Transaction, concurrency và toàn vẹn dữ liệu

### AS-IS

- Tạo Chi tiết công việc:
  - Không có transaction.
  - `SaveChangesAsync` một lần sau khi thêm chi tiết, rồi gọi đồng bộ, rồi `SaveChangesAsync` lần nữa.
  - Nếu lưu chi tiết thành công nhưng đồng bộ cha lỗi, dữ liệu có thể lệch tạm thời.
- Sửa Chi tiết công việc:
  - Không có transaction.
  - Lưu chi tiết trước, đồng bộ sau, rồi lưu lần nữa.
- Xóa Chi tiết công việc:
  - Không có transaction.
  - Soft delete trước, đồng bộ sau.
- Báo cáo tiến độ:
  - `CapNhatTienDoAsync` có transaction cho tạo báo cáo và lưu file metadata; có rollback DB và xóa file vật lý đã lưu nếu lỗi.
  - `XuLyBaoCaoTienDoAsync` có transaction cho duyệt/từ chối/yêu cầu bổ sung, cập nhật CTCV và đồng bộ cha.
- Không thấy concurrency token/rowversion cho `CT_CONG_VIEC`.
- Không thấy cơ chế chống submit hai lần ở form Chi tiết công việc.
- Tạo/sửa Chi tiết công việc không kiểm tra trùng nội dung/tên trong cùng Công việc.
- `GetPageAsync` của Chi tiết gọi thêm query đếm phân công theo danh sách trang, tránh N+1 cho số người phân công.
- Có nhiều `SaveChangesAsync` trong CRUD Chi tiết, cần ghi nhận rủi ro nhất quán như trên.

## 16. Các điểm hợp lý trong thiết kế hiện tại

- Controller và Service tách riêng: `ChiTietCongViecController` mỏng, nghiệp vụ nằm trong `ChiTietCongViecService`.
- Có soft delete cho `CT_CONG_VIEC`.
- Có service đồng bộ trạng thái trung tâm `TrangThaiWorkflowService`.
- Báo cáo tiến độ phải được duyệt `DaDuyet` mới cập nhật trạng thái thật của Chi tiết công việc.
- Có kiểm tra người cập nhật tiến độ phải được phân công Chi tiết công việc.
- Có chặn gửi báo cáo khi đang có báo cáo `ChoDuyet`.
- Có bắt buộc file minh chứng khi đề xuất `ChoXacNhanHoanThanh` hoặc `HoanThanh`.
- Có kiểm tra không lùi trạng thái trong luồng báo cáo tiến độ.
- Phân công chi tiết yêu cầu nhân viên thuộc Dự án và đã được phân công Công việc cha.
- Đồng bộ Công việc cha xử lý đúng trường hợp tất cả Chi tiết `ChuaBatDau` là giữ `ChuaBatDau`.

## 17. Các điểm chưa hợp lý hoặc có rủi ro

| Mức độ | File/method | Hành vi hiện tại | Tình huống | Ảnh hưởng |
| --- | --- | --- | --- | --- |
| Cao | `Views/ChiTietCongViec/_Form.cshtml`, `ChiTietCongViecService.AddAsync` | Cho chọn trạng thái khi tạo, gồm `HoanThanh`; service lưu trực tiếp từ form | Người tạo chọn `HoanThanh` ngay khi thêm chi tiết | Chi tiết mới có thể hoàn thành không qua báo cáo/duyệt; Công việc cha có thể chuyển `ChoXacNhanHoanThanh` nếu tất cả chi tiết hoàn thành |
| Cao | `Views/ChiTietCongViec/_Table.cshtml`, `ChiTietCongViecService.UpdateAsync` | Form sửa cho sửa trực tiếp trạng thái và service ghi đè | Chi tiết đã có báo cáo duyệt `DangThucHien`, người có quyền sửa chọn lại `ChuaBatDau`/`HoanThanh` | Trạng thái thật có thể lệch lịch sử tiến độ đã duyệt |
| Trung bình | `ChiTietCongViecService.UpdateAsync`, `RemoveAsync` | Không kiểm tra đã phân công hoặc đã có báo cáo tiến độ | Sửa/xóa chi tiết đang có người thực hiện/báo cáo | Dữ liệu nghiệp vụ và lịch sử liên quan còn tồn tại nhưng chi tiết bị đổi/xóa mềm |
| Trung bình | `ChiTietCongViecService.AddAsync/UpdateAsync/RemoveAsync` | Không có transaction giữa lưu chi tiết và đồng bộ cha | `SaveChangesAsync` đầu thành công, đồng bộ hoặc save sau lỗi | Trạng thái Công việc/Dự án có thể lệch với Chi tiết |
| Trung bình | `ChiTietCongViecService.KiemTraDuLieuDauVao` | Chỉ kiểm tra ngày bắt đầu trong khoảng Công việc cha; không có ngày kết thúc dự kiến chi tiết | Cần quản lý hạn riêng của chi tiết | Không biểu diễn được quá hạn chi tiết theo hạn dự kiến |
| Trung bình | Module Chi tiết/Tiến độ | Không phân biệt trạng thái nghiệp vụ với tình trạng quá hạn ở Chi tiết | Chi tiết chưa hoàn thành nhưng thời gian thực tế đã trễ | Không có nhãn `QuaHan`/`Tre` cho Chi tiết trong module này |
| Thấp | `ChiTietCongViecService.AddAsync` | Không tự phân công người tạo hoặc nhân viên | Tạo chi tiết xong chưa có người được gán | Nhân viên không thể cập nhật tiến độ cho tới khi phân công |
| Thấp | `TrangThaiWorkflowService` và các service khác | Logic trạng thái nằm rải rác nhiều service | Thêm trạng thái hoặc đổi rule | Dễ không đồng nhất nếu sửa một nơi mà quên nơi khác |
| Thấp | Dữ liệu trạng thái | Có helper `TrangThai`, nhưng nhiều bảng lưu string nullable | Dữ liệu cũ/nhập tay dùng biến thể khác | Cần normalize thường xuyên bằng `TrangThai.ToCode/EqualsValue` |

Không thấy rủi ro “vừa tạo tất cả `ChuaBatDau` làm Công việc cha thành `DangThucHien`” trong source hiện tại; rule hiện tại trả `ChuaBatDau`.

## 18. Đối chiếu với hướng nghiệp vụ dự kiến

| STT | Hướng dự kiến | Đánh dấu | Bằng chứng |
| --- | --- | --- | --- |
| 1 | Khi tạo Chi tiết công việc, trạng thái mặc định là `ChuaBatDau` | Đúng một phần | `GetPageAsync` set form mặc định `ChuaBatDau`, nhưng `_Form.cshtml` cho chọn trạng thái khác và `AddAsync` lưu từ form |
| 2 | Người tạo không được tự chọn trạng thái khi tạo mới | Đang sai | `_Form.cshtml` có dropdown `TrangThaiCTCV` khi tạo |
| 3 | Ngày bắt đầu và ngày kết thúc không tự động quyết định trạng thái nghiệp vụ | Đúng một phần | Không có auto theo ngày bắt đầu/hạn; nhưng `NgayKetThucCTCV` bị set theo trạng thái hoàn thành |
| 4 | Đến ngày bắt đầu không tự động chuyển `DangThucHien` | Đã đúng | Không tìm thấy job/middleware/load-page logic chuyển trạng thái theo `NgayBatDauCTCV` |
| 5 | Qua ngày kết thúc không tự động chuyển `HoanThanh` | Đã đúng | Không có ngày kết thúc dự kiến chi tiết; không có logic auto hoàn thành theo ngày |
| 6 | Nếu quá ngày kết thúc mà chưa hoàn thành thì hiển thị là quá hạn | Chưa có | Không thấy nhãn/quy tắc quá hạn trong Chi tiết/Tiến độ; `TreTienDo` không dùng cho `TrangThaiCTCV` |
| 7 | Trạng thái thật được cập nhật từ hoạt động thực tế hoặc báo cáo tiến độ đã được duyệt | Đúng một phần | Luồng tiến độ đúng: chỉ `DaDuyet` mới cập nhật; nhưng form tạo/sửa Chi tiết vẫn cập nhật trực tiếp trạng thái |
| 8 | Khi tất cả Chi tiết còn `ChuaBatDau`, Công việc cha phải giữ `ChuaBatDau` | Đã đúng | `TinhTrangThaiCongViecTheoChiTiet` trả `ChuaBatDau` khi `coTienTrien == false` |
| 9 | Khi có Chi tiết thực sự bắt đầu, Công việc cha mới chuyển `DangThucHien` | Đúng một phần | Có `DangThucHien` sẽ chuyển cha; nhưng tạo/sửa thủ công cũng có thể set `DangThucHien`, không nhất thiết từ hoạt động thực tế đã duyệt |
| 10 | Khi tất cả Chi tiết hoàn thành, Công việc cha chuyển `ChoXacNhanHoanThanh`, không chuyển thẳng `HoanThanh` | Đã đúng | `TinhTrangThaiCongViecTheoChiTiet` trả `ChoXacNhanHoanThanh` khi tất cả chi tiết hoàn thành |
| 11 | `HoanThanh` cần bước xác nhận của người có thẩm quyền | Đúng một phần | Công việc cha cần `CongViecService.XacNhanHoanThanhCongViecAsync`; Chi tiết công việc lại có thể `HoanThanh` trực tiếp từ form tạo/sửa hoặc từ báo cáo duyệt |
| 12 | Mọi thay đổi trạng thái Chi tiết phải đồng bộ theo chuỗi `CT_CONG_VIEC -> CONG_VIEC -> DU_AN` | Đúng một phần | `AddAsync`, `UpdateAsync`, `RemoveAsync`, duyệt báo cáo đều gọi đồng bộ chuỗi; nhưng nếu có nguồn khác ghi DB trực tiếp thì không có guard ở entity/DB |

## 19. Danh sách file liên quan

| STT | File | Vai trò | Method/action quan trọng | Có khả năng cần sửa sau này |
| --- | --- | --- | --- | --- |
| 1 | `Models/Entities/CtCongViec.cs` | Entity `CT_CONG_VIEC` | Properties | Có |
| 2 | `Data/QuanLyDuAnDbContext.cs` | DbSet và cấu hình quan hệ | `OnModelCreating` cho `CtCongViec`, `PhanCongCtCongViec`, `TienDoCongViec` | Có |
| 3 | `Migrations/20260527125053_Init.cs` | Schema ban đầu | CreateTable `CT_CONG_VIEC`, `PHAN_CONG_CT_CONG_VIEC`, `TIEN_DO_CONG_VIEC` | Có nếu cần migration sau này |
| 4 | `Constants/TrangThai.cs` | Constants/helper trạng thái | `ToCode`, `ToDisplay`, `EqualsValue`, `LaHoanThanhCongViec` | Có |
| 5 | `Constants/Permissions.cs` | Constants permission | `ChiTietCongViec`, `PhanCongChiTietCongViec`, `TienDo` | Có nếu đổi quyền |
| 6 | `Controllers/ChiTietCongViecController.cs` | Controller CRUD Chi tiết | `Index`, `Them`, `Sua`, `Xoa`, `XuatFile` | Có |
| 7 | `Services/Interfaces/IChiTietCongViecService.cs` | Interface service | `GetPageAsync`, `AddAsync`, `UpdateAsync`, `RemoveAsync` | Có |
| 8 | `Services/Implementations/ChiTietCongViecService.cs` | Nghiệp vụ CRUD Chi tiết | `AddAsync`, `UpdateAsync`, `RemoveAsync`, `KiemTraDuLieuDauVao`, `CoTheCapNhatAsync` | Có |
| 9 | `ViewModels/ChiTietCongViec/ChiTietCongViecCreateUpdateViewModel.cs` | ViewModel form | Properties/validation attributes | Có |
| 10 | `ViewModels/ChiTietCongViec/ChiTietCongViecItemViewModel.cs` | ViewModel dòng bảng | Properties | Có |
| 11 | `ViewModels/ChiTietCongViec/ChiTietCongViecPageViewModel.cs` | ViewModel trang | `CoTheCapNhat`, `Permissions` | Có |
| 12 | `ViewModels/ChiTietCongViec/ChiTietCongViecSummaryViewModel.cs` | Summary Công việc cha | `TrangThaiCongViec`, `PhanTramTienDo` | Có |
| 13 | `Views/ChiTietCongViec/Index.cshtml` | Trang danh sách/tạo | Render form, bảng, banner | Có |
| 14 | `Views/ChiTietCongViec/_Form.cshtml` | Form tạo | Dropdown trạng thái tạo | Có |
| 15 | `Views/ChiTietCongViec/_Table.cshtml` | Bảng và form sửa inline | Dropdown trạng thái sửa, nút phân công/sửa/xóa | Có |
| 16 | `wwwroot/css/ChiTietCongViec/index.css` | CSS module Chi tiết | Badge/form/table CSS | Có nếu chỉnh UI |
| 17 | `Services/Interfaces/ITrangThaiWorkflowService.cs` | Interface đồng bộ trạng thái | 3 method đồng bộ | Có |
| 18 | `Services/Implementations/TrangThaiWorkflowService.cs` | Đồng bộ Công việc/Dự án | `TinhTrangThaiCongViecTheoChiTiet`, `DongBoChuoiTrangThaiTuCongViecAsync` | Có |
| 19 | `Services/Implementations/CongViecService.cs` | Danh sách Công việc và xác nhận hoàn thành | `XacNhanHoanThanhCongViecAsync`, `MoLaiCongViecAsync` | Có |
| 20 | `Controllers/CongViecController.cs` | Controller xác nhận/mở lại Công việc | `XacNhanHoanThanh`, `MoLai` | Có |
| 21 | `Models/Entities/PhanCongCtCongViec.cs` | Entity phân công chi tiết | Properties | Có |
| 22 | `Models/Entities/NhatKyPhanCongCtCongViec.cs` | Entity nhật ký phân công | Properties | Có |
| 23 | `Controllers/PhanCongChiTietCongViecController.cs` | Controller phân công chi tiết | `Index`, `ThemPhanCong`, `XoaPhanCong` | Có |
| 24 | `Services/Interfaces/IPhanCongChiTietCongViecService.cs` | Interface phân công | `GetPageAsync`, `AddAsync`, `RemoveAsync` | Có |
| 25 | `Services/Implementations/PhanCongChiTietCongViecService.cs` | Nghiệp vụ phân công chi tiết | `KiemTraNhanVienHopLeAsync`, `KiemTraQuyenPhanCongAsync` | Có |
| 26 | `ViewModels/PhanCongChiTietCongViec/*.cs` | ViewModel phân công | Form/item/page/summary/options | Có |
| 27 | `Views/PhanCongChiTietCongViec/*.cshtml` | UI phân công | Form, bảng, summary | Có nếu chỉnh UI |
| 28 | `wwwroot/css/PhanCongChiTietCongViec/index.css` | CSS phân công chi tiết | Layout/form/table CSS | Có nếu chỉnh UI |
| 29 | `Models/Entities/TienDoCongViec.cs` | Entity báo cáo tiến độ | Properties | Có |
| 30 | `Models/Entities/FileTienDoCongViec.cs` | Entity file minh chứng tiến độ | Properties | Có |
| 31 | `Controllers/TienDoCongViecController.cs` | Controller tiến độ | `CapNhatTienDo`, `DuyetBaoCaoTienDo`, `YeuCauBoSungBaoCaoTienDo`, `TuChoiBaoCaoTienDo` | Có |
| 32 | `Services/Interfaces/ITienDoCongViecService.cs` | Interface tiến độ | Methods tiến độ/duyệt | Có |
| 33 | `Services/Implementations/TienDoCongViecService.cs` | Nghiệp vụ tiến độ và cập nhật trạng thái thật | `CapNhatTienDoAsync`, `XuLyBaoCaoTienDoAsync`, `KiemTraKhongDuocLuiTrangThai` | Có |
| 34 | `Services/Interfaces/IFileTienDoCongViecService.cs` | Interface file tiến độ | `UploadAsync`, `DeleteAsync`, `GetDownloadInfoAsync` | Có |
| 35 | `Services/Implementations/FileTienDoCongViecService.cs` | Tải file minh chứng | `GetDownloadInfoAsync`; upload/delete hiện bị chặn | Có |
| 36 | `ViewModels/TienDoCongViec/*.cs` | ViewModel tiến độ | Form cập nhật, duyệt, item, lịch sử | Có |
| 37 | `Views/TienDoCongViec/*.cshtml` | UI tiến độ | `_UpdateForm`, `_History`, `_Table` | Có |
| 38 | `wwwroot/css/TienDoCongViec/index.css` | CSS tiến độ | Badge trạng thái, bảng, form báo cáo | Có nếu chỉnh UI |
| 39 | `Services/CauHinhDichVu.cs` | Đăng ký DI | Đăng ký các service liên quan | Có nếu thêm service |
| 40 | `Program.cs` | Cấu hình app | Không có hosted job trạng thái theo ngày | Ít khả năng |
| 41 | `Data/KhoiTaoTaiKhoanMacDinh.cs` | Seed quyền mặc định | Gán quyền Manager/Employee, map permission | Có nếu đổi role mặc định |
| 42 | `Services/Implementations/PermissionDependencyProvider.cs` | Cây permission | Definition Chi tiết/Phân công chi tiết | Có nếu đổi quyền |
| 43 | `Models/Entities/FileCtCongViec.cs` | Entity file riêng của CTCV | Properties; chưa thấy service/controller dùng | Có nếu phát triển file CTCV |

## 20. Kết luận AS-IS

- Trạng thái Chi tiết công việc hiện được điều khiển bởi cả form tạo/sửa trực tiếp và báo cáo tiến độ đã duyệt. Báo cáo chưa duyệt không cập nhật trạng thái thật.
- Khi tạo mới, form mặc định là `ChuaBatDau`, nhưng người dùng có quyền có thể chọn `DangThucHien`, `BiCanCan`, `TamDung`, hoặc `HoanThanh`; service lưu trạng thái từ form.
- Ngày tháng không tự động chuyển trạng thái theo thời gian. Không có job tự chuyển `DangThucHien` khi đến ngày bắt đầu, không có auto `HoanThanh` khi qua ngày kết thúc.
- `NgayKetThucCTCV` là dữ liệu được set khi trạng thái là hoàn thành, không phải hạn kết thúc dự kiến do người dùng nhập.
- Công việc cha đang đồng bộ đúng trong trường hợp tất cả Chi tiết `ChuaBatDau`: giữ `ChuaBatDau`, không rơi sang `DangThucHien`.
- Vấn đề quan trọng nhất cần ưu tiên khi bước sang giai đoạn chỉnh sửa là khóa việc chọn/ghi đè trạng thái thủ công ở form Chi tiết, đặc biệt khi tạo mới hoặc khi đã có báo cáo tiến độ được duyệt.
- Các file dự kiến cần xem xét nếu chỉnh sửa sau này: `Views/ChiTietCongViec/_Form.cshtml`, `Views/ChiTietCongViec/_Table.cshtml`, `ChiTietCongViecService.cs`, `TienDoCongViecService.cs`, `TrangThaiWorkflowService.cs`, và các ViewModel Chi tiết/Tiến độ liên quan.

## Trạng thái sau chỉnh sửa

### Phạm vi file đã sửa

- `Views/ChiTietCongViec/_Form.cshtml`: bỏ dropdown chọn `TrangThaiCTCV` ở form tạo Chi tiết công việc. Form chỉ hiển thị dòng chỉ đọc `Trạng thái khi tạo: Chưa bắt đầu`, không gửi hidden input trạng thái.
- `Views/ChiTietCongViec/_Table.cshtml`: bỏ dropdown sửa `TrangThaiCTCV` trong form sửa inline desktop/mobile. Trạng thái hiện tại chỉ hiển thị bằng badge và ghi chú `Trạng thái được cập nhật thông qua báo cáo tiến độ đã được duyệt.` Nhãn ngày kết thúc trong bảng được đổi thành `Ngày hoàn thành thực tế`.
- `Controllers/ChiTietCongViecController.cs`: đổi nhãn cột export từ `Ngày kết thúc` sang `Ngày hoàn thành thực tế`; không đổi route, action hoặc dữ liệu xuất.
- `ViewModels/ChiTietCongViec/ChiTietCongViecCreateUpdateViewModel.cs`: giữ property `TrangThaiCTCV` để tránh ảnh hưởng binding hiện có, nhưng bỏ validation `[Required]` để form CRUD Chi tiết công việc không bắt buộc gửi trạng thái.
- `Services/Implementations/ChiTietCongViecService.cs`: chuẩn hóa backend để không tin cậy trạng thái từ client trong CRUD Chi tiết công việc; bổ sung transaction cho tạo, sửa, xóa mềm và đồng bộ trạng thái cha.

### Hành vi sau chỉnh sửa

- Form tạo đã bỏ chọn trạng thái. Người dùng không thể chọn `ChuaBatDau`, `DangThucHien`, `BiCanCan`, `TamDung`, hoặc `HoanThanh` từ giao diện tạo.
- `AddAsync` luôn gán `TrangThaiCTCV = TrangThai.ChuaBatDau`, `NgayKetThucCTCV = null`, `NgayTaoCTCV = DateTime.Now`, `IsDeleted = false`. Giá trị `TrangThaiCTCV` do request thủ công gửi lên bị bỏ qua.
- Form sửa đã bỏ sửa trạng thái. Người dùng chỉ sửa các trường thông tin hiện có như tên, nội dung và ngày bắt đầu.
- `UpdateAsync` giữ nguyên `TrangThaiCTCV` và `NgayKetThucCTCV` đang có trong database. Service không đọc, không chuyển đổi và không ghi trạng thái từ `form.TrangThaiCTCV`.
- Luồng báo cáo tiến độ vẫn là nguồn cập nhật trạng thái thật: báo cáo mới ở `ChoDuyet`, chỉ khi duyệt `DaDuyet` trong `TienDoCongViecService.XuLyBaoCaoTienDoAsync` mới cập nhật `CT_CONG_VIEC.TrangThaiCTCV`, cập nhật `NgayKetThucCTCV` nếu trạng thái được duyệt là `HoanThanh`, rồi đồng bộ Công việc/Dự án.
- Logic đồng bộ Công việc/Dự án trong `TrangThaiWorkflowService` không bị thay đổi. Quy tắc tất cả Chi tiết `ChuaBatDau` thì Công việc cha giữ `ChuaBatDau`; tất cả Chi tiết `HoanThanh` thì Công việc cha chuyển `ChoXacNhanHoanThanh`, không chuyển thẳng `HoanThanh`, vẫn được giữ nguyên.
- Không bổ sung cơ chế tự động đổi trạng thái theo ngày, không thêm background job, hosted service, middleware hoặc JavaScript đổi trạng thái theo thời gian.

### Transaction và toàn vẹn dữ liệu

- `ChiTietCongViecService.AddAsync`, `UpdateAsync`, và `RemoveAsync` đã được bọc bằng transaction của DbContext.
- Thao tác ghi Chi tiết công việc và gọi `DongBoChuoiTrangThaiTuCongViecAsync` nằm trong cùng transaction. Nếu đồng bộ trạng thái Công việc/Dự án lỗi, thay đổi tạo/sửa/xóa mềm Chi tiết công việc sẽ rollback cùng transaction.
- Soft delete trong `RemoveAsync` vẫn giữ nguyên: `IsDeleted = true`, `DeletedAt`, `DeletedBy`; không chuyển sang hard delete.

### Kiểm tra sau chỉnh sửa

- Kiểm tra source xác nhận `Views/ChiTietCongViec` không còn input hoặc dropdown `TrangThaiCTCV`.
- Kiểm tra source xác nhận `ChiTietCongViecService` không còn `TrangThai.ToCode(form.TrangThaiCTCV)`.
- Kiểm tra source xác nhận `UpdateAsync` không còn gán `entity.TrangThaiCTCV` hoặc tính lại `entity.NgayKetThucCTCV` từ form.
- Build đã chạy bằng `dotnet build QuanLyDuAn\QuanLyDuAn.sln`: thành công, 0 error. Có 2 warning cũ ở `Services/Implementations/FileTienDoCongViecService.cs` về async method không có `await`, không thuộc phạm vi chỉnh sửa này.
- Kiểm tra UTF-8 trên các file đã sửa: các file đọc được bằng UTF-8 strict.
- Kiểm tra nhanh các chuỗi lỗi font thường gặp trong các file đã sửa: không phát hiện mojibake do lần chỉnh sửa này.

### Xác nhận phạm vi không thay đổi

- Không sửa database.
- Không tạo migration.
- Không sửa file migration hoặc SQL schema.
- Không đổi controller route.
- Không đổi permission.
- Không đổi data scope.
- Không đổi quy trình phân công Chi tiết công việc.
- Không đổi quy trình gửi, duyệt, từ chối hoặc yêu cầu bổ sung báo cáo tiến độ.
- Không đổi quy trình xác nhận hoàn thành Công việc.
- Không đổi trạng thái trong `Constants/TrangThai.cs`.
- Không đổi workflow AI hoặc database AI.

## 21. Phân tích nhận biết Chi tiết công việc đúng hạn và trễ hạn

Mục này bổ sung phân tích AS-IS theo source hiện tại sau các chỉnh sửa đã ghi ở trên. Chỉ đọc source và tài liệu hóa, không thay đổi source, giao diện, database, migration hoặc workflow.

Nguyên tắc cần giữ: `TrangThaiCTCV` là trạng thái workflow của Chi tiết công việc. `Trễ hạn`, `Quá hạn`, `Hoàn thành đúng hạn`, `Hoàn thành trễ` là tình trạng thời hạn, không phải trạng thái workflow nếu source chưa triển khai như vậy. Cách hiểu đúng phải là `Đang thực hiện + Trễ 5 ngày`, `Hoàn thành + Hoàn thành trễ 2 ngày`, `Hoàn thành + Hoàn thành đúng hạn`, `Chờ xác nhận hoàn thành + Quá hạn 1 ngày`. Không có logic tự chuyển trạng thái khi vượt hạn và không tự hoàn thành khi tới ngày kết thúc.

### 21.1. Xác định ý nghĩa các ngày của Chi tiết công việc

| Thuộc tính | Ý nghĩa thực tế | Nguồn dữ liệu | Khi nào được gán | Là hạn dự kiến hay ngày thực tế |
| ---------- | --------------- | ------------- | ---------------- | ------------------------------- |
| `NgayTaoCTCV` | Thời điểm tạo Chi tiết công việc | `CtCongViec.NgayTaoCTCV`, `ChiTietCongViecService.AddAsync` | Gán `DateTime.Now` khi thêm mới | Không phải hạn, không phải ngày hoàn thành |
| `NgayBatDauCTCV` | Ngày bắt đầu Chi tiết công việc | Form `_Form.cshtml`, inline edit `_Table.cshtml`, `ChiTietCongViecService.AddAsync/UpdateAsync` | Người dùng nhập, service lưu `.Date`; validate không trước ngày bắt đầu Công việc và không sau `CongViec.NgayKetThucCVDuKien` | Ngày bắt đầu, không phải deadline |
| `NgayKetThucCTCV` | Ngày hoàn thành thực tế theo workflow hiện tại | `CtCongViec.NgayKetThucCTCV`, projected trong `ChiTietCongViecService.GetPageAsync` | `AddAsync` luôn set `null`; `UpdateAsync` giữ nguyên; `TienDoCongViecService.XuLyBaoCaoTienDoAsync` set `DateTime.Now` khi báo cáo được duyệt `DaDuyet` với trạng thái đề xuất hoàn thành, ngược lại set `null` | Ngày hoàn thành thực tế, không phải hạn dự kiến |
| `TrangThaiCTCV` | Trạng thái workflow Chi tiết công việc | `CtCongViec.TrangThaiCTCV`, `TrangThai.ToCode/ToDisplay` | CRUD tạo mặc định `ChuaBatDau`; cập nhật thật qua báo cáo tiến độ được duyệt | Không phải dữ liệu thời hạn |
| `ThoiGianCapNhat` | Thời điểm nhân viên gửi báo cáo tiến độ | `TienDoCongViec.ThoiGianCapNhat` | `CapNhatTienDoAsync` set `DateTime.Now` khi tạo báo cáo `ChoDuyet` | Có thể tham khảo thời điểm báo hoàn thành, nhưng source không dùng làm ngày hoàn thành thực tế |
| `ThoiGianDuyet` | Thời điểm người duyệt xử lý báo cáo | `TienDoCongViec.ThoiGianDuyet` | `XuLyBaoCaoTienDoAsync` set `DateTime.Now` khi duyệt/từ chối/yêu cầu bổ sung | Không phải deadline; gần với thời điểm `NgayKetThucCTCV` được set khi duyệt hoàn thành |
| `TrangThaiCTCVDeXuat` | Trạng thái Chi tiết do người thực hiện đề xuất | `TienDoCongViec.TrangThaiCTCVDeXuat` | `CapNhatTienDoAsync` lưu trạng thái đề xuất, ban đầu báo cáo ở `ChoDuyet` | Không phải ngày, nhưng cho biết báo cáo có đề xuất hoàn thành hay không |

Không tìm thấy trường `NgayKetThucCTCVDuKien`, `HanCTCV`, `HanChiTietCongViec`, `DeadlineCTCV`, `NgayDuKien` hoặc dữ liệu tương đương trong `CtCongViec`, ViewModel Chi tiết, form tạo/sửa, bảng tiến độ, phân công, `QuanLyDuAnDbContext` hoặc migration hiện hành. Hạn duy nhất có liên quan là `CongViec.NgayKetThucCVDuKien` của Công việc cha; source chỉ dùng hạn này để validate `NgayBatDauCTCV` không sau hạn Công việc cha, không quy định đây là deadline riêng của từng Chi tiết.

Kết luận bắt buộc: `NgayKetThucCTCV` hiện là ngày hoàn thành thực tế, không phải hạn dự kiến.

### 21.2. Khả năng xác định Chi tiết công việc quá hạn

Điều kiện mong muốn:

```text
Chi tiết chưa hoàn thành
và hạn dự kiến của Chi tiết < DateTime.Today
```

Source hiện tại không có hạn dự kiến riêng cho Chi tiết nên không thể xác định chính xác `Chi tiết chưa hoàn thành nhưng đã quá hạn`.

| Khả năng | Source hiện tại | Ưu điểm | Nhược điểm |
| -------- | --------------- | ------- | ---------- |
| Dùng hạn riêng của Chi tiết | Không có trường dữ liệu | Chính xác nếu được bổ sung sau này | Hiện không thể làm nếu không thêm dữ liệu/schema hoặc nguồn deadline rõ ràng |
| Dùng hạn Công việc cha `NgayKetThucCVDuKien` | Có thể join từ `CtCongViec -> CongViec`, nhưng source chưa quy định là hạn Chi tiết | Không cần thêm cột nếu chỉ cảnh báo tạm | Nhiều Chi tiết cùng một Công việc sẽ chung hạn; không phản ánh kế hoạch riêng; có thể đánh dấu sai nếu Chi tiết cần xong sớm hơn hoặc muộn hơn |
| Dùng `ThoiGianCapNhat` của báo cáo tiến độ | Có trong `TIEN_DO_CONG_VIEC` | Biết thời điểm nhân viên gửi báo cáo | Đây là thời điểm báo cáo, không phải deadline; không đánh giá được chưa hoàn thành quá hạn nếu chưa có deadline |
| Dùng `NgayKetThucCTCV` | Có trong entity và ViewModel | Có ngày hoàn thành thực tế khi đã hoàn thành | Không áp dụng cho chi tiết chưa hoàn thành; dùng nó làm hạn là sai ý nghĩa |
| Dùng `NgayBatDauCTCV` cộng một quy ước mặc định | Có ngày bắt đầu | Không cần schema | Source không có quy ước thời lượng, chỉ là ước lượng không có căn cứ |

Một số module thống kê đang có logic chưa đủ tin cậy: `DanhGiaDuAnService` và `DanhGiaNhanVienService` có đếm `ChiTietTreHan` dựa trên `NgayKetThucCTCV < DateTime.Today/Now` và trạng thái chưa hoàn thành. Vì `NgayKetThucCTCV` là ngày hoàn thành thực tế, logic này không thể đại diện chính xác cho Chi tiết quá hạn. Với Chi tiết chưa hoàn thành, `NgayKetThucCTCV` thường `null`, nên không thể biết đã quá hạn hay chưa.

### 21.3. Khả năng xác định Chi tiết hoàn thành đúng hạn hoặc trễ

| Câu hỏi | Kết luận AS-IS |
| ------- | -------------- |
| Có ngày hoàn thành thực tế không | Có: `NgayKetThucCTCV`, set khi báo cáo hoàn thành được duyệt |
| Có ngày kết thúc dự kiến riêng không | Không |
| Có thể xác định `Hoàn thành đúng hạn` không | Không thể xác định chính xác từ dữ liệu hiện tại |
| Có thể xác định `Hoàn thành trễ` không | Không thể xác định chính xác từ dữ liệu hiện tại |
| Thiếu dữ liệu nào | Thiếu deadline/hạn dự kiến riêng của Chi tiết công việc hoặc một rule nghiệp vụ chính thức cho nguồn deadline |
| Có thể xác định gián tiếp theo Công việc cha không | Có thể ước lượng bằng `CongViec.NgayKetThucCVDuKien`, nhưng source chưa quy định và không phản ánh deadline riêng của từng Chi tiết |
| Mức kết luận | `Không thể xác định từ dữ liệu hiện tại` nếu yêu cầu chính xác; `Xác định tương đối` nếu sau này chấp nhận dùng hạn Công việc cha làm cảnh báo tạm |

Không được tự giả định `NgayKetThucCTCV` là hạn dự kiến. Nhãn UI hiện tại đã sửa đúng thành `Ngày hoàn thành thực tế` trong `_Table.cshtml` và export `ChiTietCongViecController.XuatFile`.

### 21.4. Ngày hoàn thành thực tế của Chi tiết

| Vấn đề | Kết luận source |
| ------ | --------------- |
| Set khi báo cáo `HoanThanh` được duyệt hay khi đề xuất | Chỉ khi báo cáo được xử lý `DaDuyet` trong `TienDoCongViecService.XuLyBaoCaoTienDoAsync`; báo cáo gửi mới chỉ là `ChoDuyet` |
| CRUD trực tiếp có set ngày hoàn thành không | Sau chỉnh sửa hiện tại, không. `AddAsync` luôn `NgayKetThucCTCV = null`; `UpdateAsync` không gán `NgayKetThucCTCV` |
| Khi trạng thái thay đổi khỏi hoàn thành | `XuLyBaoCaoTienDoAsync` set `NgayKetThucCTCV = null` nếu trạng thái được duyệt không phải hoàn thành |
| Có luồng mở lại Chi tiết riêng không | Không thấy method mở lại Chi tiết riêng trong `ChiTietCongViecService`; Chi tiết có thể đổi trạng thái qua báo cáo tiến độ nếu rule không lùi cho phép |
| Dùng ngày duyệt hay ngày báo cáo của nhân viên | `NgayKetThucCTCV = DateTime.Now` tại thời điểm duyệt, gần với `ThoiGianDuyet`, không dùng `TienDoCongViec.ThoiGianCapNhat` |
| Người quản lý duyệt chậm có thể làm tăng số ngày trễ không | Có, nếu sau này so ngày hoàn thành thực tế với deadline, ngày thực tế sẽ là ngày duyệt chứ không phải ngày nhân viên gửi báo cáo hoàn thành |
| Source có lưu thời điểm nhân viên báo hoàn thành không | Có `ThoiGianCapNhat` trên báo cáo tiến độ, nhưng không được dùng làm `NgayKetThucCTCV` và không có trường riêng “ngày nhân viên hoàn tất” |

### 21.5. Các trường hợp cần đối chiếu

| Trường hợp | Trạng thái lưu trong DB | Ngày thực tế có hay không | Có đủ dữ liệu đánh giá đúng/trễ không | Giao diện hiện thể hiện thế nào |
| ---------- | ----------------------- | ------------------------- | ------------------------------------ | ------------------------------ |
| Chi tiết chưa bắt đầu | `ChuaBatDau` | Thường không | Không, thiếu hạn dự kiến | Badge `Chưa bắt đầu`, ngày bắt đầu/ngày tạo |
| Chi tiết đang thực hiện | `DangThucHien` | Không | Không | Badge `Đang thực hiện`, không có badge trễ |
| Chi tiết bị cản trở | `BiCanCan` | Không | Không | Badge `Bị cản trở`, màu workflow blocker |
| Chi tiết chờ xác nhận hoàn thành | `ChoXacNhanHoanThanh` nếu báo cáo được duyệt trạng thái này | Không, vì chỉ `HoanThanh` mới set `NgayKetThucCTCV` | Không | Badge `Chờ xác nhận hoàn thành`, không có ngày hoàn thành |
| Chi tiết hoàn thành | `HoanThanh`/biến thể hoàn thành | Có `NgayKetThucCTCV` | Không, thiếu hạn dự kiến | Badge `Hoàn thành`, hiển thị `Ngày hoàn thành thực tế` |
| Chi tiết tạm dừng | `TamDung` | Không, hoặc bị xóa nếu chuyển từ hoàn thành sang không hoàn thành | Không | Badge `Tạm dừng` |
| Chi tiết đã hủy | `DaHuy` | Không, hoặc bị xóa nếu chuyển khỏi hoàn thành | Không nên đánh giá nếu chưa có rule | Badge `Đã hủy` nếu trạng thái có trong dữ liệu |
| Chi tiết có báo cáo hoàn thành đang chờ duyệt | `TrangThaiCTCV` thật chưa đổi; `TienDoCongViec.TrangThaiTienDo = ChoDuyet`, `TrangThaiCTCVDeXuat = HoanThanh` | Chưa set | Không | Màn tiến độ thể hiện báo cáo chờ duyệt; màn Chi tiết chỉ thấy trạng thái thật hiện tại |
| Chi tiết có báo cáo hoàn thành được duyệt | `TrangThaiCTCV = HoanThanh` | Có, set `DateTime.Now` khi duyệt | Không đủ để biết đúng/trễ nếu không có deadline | Màn Chi tiết hiển thị Hoàn thành và ngày hoàn thành thực tế |
| Chi tiết có báo cáo hoàn thành bị từ chối | Trạng thái thật không đổi | Không set mới | Không | Màn tiến độ có lịch sử từ chối; màn Chi tiết không coi đã hoàn thành |
| Chi tiết không có hạn dự kiến | Tất cả trường hợp hiện tại | Không có deadline | Không | Không hiển thị hạn dự kiến |
| Chi tiết hoàn thành sau ngày kết thúc Công việc cha | Có thể so `NgayKetThucCTCV` với `CongViec.NgayKetThucCVDuKien` nếu join | Chỉ là đánh giá tương đối theo hạn cha | Không chính xác nếu cần deadline riêng | Màn Chi tiết không hiển thị hạn cha |
| Chi tiết hoàn thành trước ngày kết thúc Công việc cha | Có thể so tương đối | Chỉ là tham khảo | Không chính xác tuyệt đối | Màn Chi tiết không hiển thị hạn cha |

### 21.6. Phân tích giao diện Chi tiết công việc

| Khu vực | Dữ liệu ngày hiện có | Trạng thái | Tình trạng thời hạn |
| ------- | -------------------- | ---------- | ------------------- |
| Desktop `_Table.cshtml` | `Ngày bắt đầu`, `Ngày hoàn thành thực tế`, `Ngày tạo` | Badge workflow | Không có hạn dự kiến, không có `Hoàn thành trễ`, không có số ngày trễ |
| Mobile `_Table.cshtml` | `Bắt đầu`, `Hoàn thành thực tế`, `Ngày tạo` | Badge workflow ở header card | Không có badge deadline |
| Form tạo `_Form.cshtml` | Chỉ nhập `NgayBatDauCTCV` | Trạng thái khi tạo chỉ đọc `Chưa bắt đầu` | Không nhập hạn dự kiến |
| Form sửa inline | Chỉ sửa `NgayBatDauCTCV`, trạng thái chỉ đọc | Không sửa trạng thái từ CRUD | Không sửa/hiển thị hạn dự kiến |
| Export `ChiTietCongViecController.XuatFile` | `Ngày bắt đầu`, `Ngày hoàn thành thực tế` | Text trạng thái | Không export hạn dự kiến/tình trạng trễ |

Nhãn `Ngày hoàn thành thực tế` đang dùng đúng ý nghĩa cho `NgayKetThucCTCV`. Không thấy CSS riêng cho tình trạng deadline trong `wwwroot/css/ChiTietCongViec/index.css`; các màu warning/danger hiện phục vụ workflow pending/blocker/xóa, không phải quá hạn. Responsive không làm mất thông tin trễ vì thông tin deadline chưa có trong ViewModel/UI.

### 21.7. ViewModel và query

Các ViewModel Chi tiết hiện có:

| ViewModel | Dữ liệu liên quan thời hạn | Nhận xét |
| --------- | -------------------------- | -------- |
| `ChiTietCongViecItemViewModel` | `NgayTaoCTCV`, `NgayBatDauCTCV`, `NgayKetThucCTCV`, `TrangThaiCTCV` | Không có hạn dự kiến, cờ trễ, số ngày trễ, tình trạng thời hạn hoặc ngày hạn Công việc cha |
| `ChiTietCongViecCreateUpdateViewModel` | `NgayBatDauCTCV`, `NgayKetThucCTCV`, `TrangThaiCTCV` | `NgayKetThucCTCV` không được form gửi/sử dụng trong CRUD hiện tại; còn lại để tương thích model |
| `ChiTietCongViecSummaryViewModel` | Trạng thái Công việc cha, tổng số/hoàn thành/phần trăm | Không có `NgayKetThucCVDuKien` của Công việc cha |
| `ChiTietCongViecPageViewModel` | Danh sách, summary, form, quyền | Không có filter hoặc summary deadline |

`ChiTietCongViecService.GetPageAsync` hiện:

- Lấy Công việc cha bằng `LayCongViecAsync(maCongViec)`, nhưng summary không project `NgayKetThucCVDuKien`.
- Query chính chỉ đọc `CtCongViec` theo `MaCongViec` và `IsDeleted != true`.
- Đếm tổng số và số hoàn thành bằng `TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh)`.
- Sort theo `NgayTaoCTCV` giảm dần, `MaChiTietCV` giảm dần, rồi phân trang.
- Project một trang sang `ChiTietCongViecItemViewModel`.
- Batch đếm phân công theo danh sách `MaChiTietCV`, tránh N+1.

Nếu sau này chỉ muốn hiển thị hạn Công việc cha như cảnh báo tương đối, có thể bổ sung vào summary hoặc từng item mà không cần N+1 vì service đã có `congViec`. Nếu cần lọc/sắp xếp theo tình trạng deadline trên toàn bộ danh sách, phải đưa điều kiện vào query trước `CountAsync()`/`Skip`/`Take`. Nếu cần chính xác cho từng Chi tiết, cần có trường deadline riêng hoặc nguồn dữ liệu chính thức khác; không thể suy ra chắc chắn từ dữ liệu hiện tại.

### 21.8. Quan hệ với báo cáo tiến độ

| Nội dung | Kết luận |
| -------- | -------- |
| Báo cáo hoàn thành chờ duyệt có được xem là đã hoàn thành không | Không. `CapNhatTienDoAsync` chỉ tạo `TienDoCongViec` ở `ChoDuyet`, không cập nhật `CtCongViec.TrangThaiCTCV` |
| Chỉ `DaDuyet` mới cập nhật trạng thái thật | Đúng. `XuLyBaoCaoTienDoAsync` chỉ đổi `CT_CONG_VIEC` khi `trangThaiDich = DaDuyet` |
| Ngày hoàn thành lấy theo ngày duyệt hay ngày gửi báo cáo | Theo ngày duyệt: `NgayKetThucCTCV = DateTime.Now` trong xử lý duyệt |
| Duyệt chậm ảnh hưởng thế nào | Nếu sau này dùng `NgayKetThucCTCV` để so deadline, duyệt chậm có thể làm tăng số ngày trễ so với thời điểm nhân viên đã gửi báo cáo hoàn thành |
| `TIEN_DO_CONG_VIEC` giúp xác định thời điểm báo hoàn thành không | Có thể tham khảo `ThoiGianCapNhat` của báo cáo có `TrangThaiCTCVDeXuat = HoanThanh`, nhưng source hiện không dùng làm ngày hoàn thành thật |
| Nguồn chính hiện tại | `CtCongViec.TrangThaiCTCV` và `CtCongViec.NgayKetThucCTCV` sau báo cáo được duyệt |

### 21.9. Đối chiếu Dự án -> Công việc -> Chi tiết công việc

| Cấp | Ngày dự kiến | Ngày thực tế | Có thể tính trễ | Nơi đang tính | Có hiển thị ở danh sách |
| --- | ------------ | ------------ | --------------- | ------------- | ----------------------- |
| Dự án | `DuAn.NgayKetThucDuAn` | `DuAn.NgayHoanThanhThucTeDuAn` hoặc fallback thống kê | Có | `DuAnDeadlineStatusHelper`, `DuAnService`, `DashboardService`, `DanhGiaDuAnService`, `AiService` | Có ở module Dự án/Dashboard |
| Công việc | `CongViec.NgayKetThucCVDuKien` | `CongViec.NgayKetThucCVThucTe` | Có nếu đủ hai ngày | `DashboardService`, `AiDatasetService`, `DanhGiaDuAnService`, `DuAnService` | Chưa hiển thị ở danh sách `/CongViec` |
| Chi tiết công việc | Không có trường hạn dự kiến riêng | `CtCongViec.NgayKetThucCTCV` | Không thể xác định chính xác | `DanhGiaDuAnService`/`DanhGiaNhanVienService` có đếm `ChiTietTreHan` theo `NgayKetThucCTCV`, nhưng không đủ tin cậy vì đây là ngày thực tế | Chưa hiển thị ở danh sách `/ChiTietCongViec` |

Logic hiện chưa thống nhất: Dự án có helper deadline riêng; Công việc có logic đúng/trễ ở Dashboard/AI/DanhGia/Dự án nhưng chưa đưa vào màn Công việc; Chi tiết thiếu hạn dự kiến riêng nên mọi logic trễ ở cấp này chỉ là không đủ dữ liệu hoặc ước lượng. Có thể cân nhắc helper/service dùng chung ở bước sau, nhưng không tạo helper trong nhiệm vụ này.

### 21.10. Rủi ro cần kiểm tra trước khi chỉnh sửa sau

| Rủi ro | Bằng chứng AS-IS | Nhận xét |
| ------ | ---------------- | -------- |
| Chi tiết không có hạn dự kiến | Entity, ViewModel, Form, DbContext không có field deadline riêng | Không thể xác định đúng/trễ chính xác |
| Dùng hạn Công việc cha cho tất cả Chi tiết có thể sai nghiệp vụ | Source chỉ dùng hạn cha để validate ngày bắt đầu không sau hạn Công việc | Nếu dùng sau này nên ghi là cảnh báo tương đối |
| Ngày hoàn thành thực tế lấy theo ngày duyệt | `TienDoCongViecService.XuLyBaoCaoTienDoAsync` set `NgayKetThucCTCV = DateTime.Now` khi duyệt | Duyệt chậm có thể làm tăng trễ |
| Báo cáo chờ duyệt bị hiểu nhầm là hoàn thành thật | Báo cáo `ChoDuyet` không đổi `CT_CONG_VIEC` | UI cần tách trạng thái thật và trạng thái đề xuất |
| Logic trễ Chi tiết trong thống kê đang dùng sai mốc | `DanhGiaDuAnService`/`DanhGiaNhanVienService` đếm `NgayKetThucCTCV < hôm nay` | Cần xem lại nếu phát triển deadline Chi tiết |
| Query bổ sung số ngày trễ gây N+1 | Hiện service đã batch phân công theo page | Giữ pattern batch hoặc join SQL |
| Tính sau phân trang nhưng filter trễ không khớp tổng | `GetPageAsync` count trước pagination | Nếu thêm filter deadline cần áp dụng trước count |
| Summary card chỉ tính trên trang hiện tại | Chi tiết hiện summary tổng/hoàn thành tính từ query trước phân trang, tốt hơn Công việc | Nếu thêm summary deadline cần tính từ query đúng scope |
| Dùng `DateTime.Now` và `.Date` không thống nhất | Tiến độ dùng `DateTime.Now`; thống kê dùng `DateTime.Today`/`DateTime.Now` | Cần chuẩn hóa khi triển khai |
| Công việc cha đã hủy/tạm dừng | Service khóa cập nhật nhưng không có rule deadline riêng | Không nên tự tính trễ nếu nghiệp vụ muốn loại khỏi đánh giá |
| EF Core translate lỗi | Query Chi tiết hiện đơn giản; nguy cơ xuất hiện nếu thêm helper/string interpolation vào `IQueryable` | Chỉ dùng biểu thức SQL-translatable trước `ToListAsync()` |

### 21.11. Kết luận chức năng Chi tiết công việc

- Hiện tại không xác định được chính xác Chi tiết công việc hoàn thành đúng hạn hoặc trễ hạn vì thiếu ngày kết thúc dự kiến/deadline riêng cho Chi tiết.
- Nhãn phân tích phù hợp cho tình trạng thời hạn của Chi tiết công việc hiện tại là `Chưa đủ dữ liệu xác định`.
- `NgayKetThucCTCV` là ngày hoàn thành thực tế, được set khi báo cáo tiến độ được duyệt hoàn thành; không phải hạn dự kiến.
- Việc dùng `CongViec.NgayKetThucCVDuKien` để cảnh báo Chi tiết là khả năng tương đối, không phải logic chính thức trong source hiện tại. Cách này có ưu điểm là không cần thêm dữ liệu nhưng có nguy cơ sai vì mọi Chi tiết cùng Công việc sẽ chung hạn.
- Nếu muốn xác định chính xác, cần có deadline riêng cho Chi tiết công việc hoặc một nguồn deadline chính thức tương đương. Nếu nguồn đó là cột mới thì bắt buộc sửa database/schema/migration; nếu chỉ dùng hạn Công việc cha thì không sửa database nhưng chỉ là ước lượng.
- File có khả năng cần sửa ở bước tiếp theo nếu triển khai: `CtCongViec.cs`, `QuanLyDuAnDbContext.cs`, migration nếu thêm field; `ChiTietCongViecCreateUpdateViewModel.cs`, `ChiTietCongViecItemViewModel.cs`, `ChiTietCongViecSummaryViewModel.cs`, `ChiTietCongViecService.cs`, `Views/ChiTietCongViec/_Form.cshtml`, `Views/ChiTietCongViec/_Table.cshtml`, `Views/ChiTietCongViec/Index.cshtml`, `wwwroot/css/ChiTietCongViec/index.css`, `TienDoCongViecService.cs`, `DanhGiaDuAnService.cs`, `DanhGiaNhanVienService.cs` nếu cần thống nhất thống kê.
- Không được tự chuyển `TrangThaiCTCV` thành `TreHan`/`QuaHan`, không tự động đổi trạng thái khi quá hạn, không coi báo cáo `ChoDuyet` là hoàn thành thật, không dùng `NgayKetThucCTCV` làm deadline.
