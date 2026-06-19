# Phân tích hiển thị trạng thái phân công công việc và chi tiết công việc

## 1. Phạm vi source đã kiểm tra

Đã kiểm tra source MVC trong `QuanLyDuAn/QuanLyDuAn`, tập trung vào entity, DbContext, controller, service, ViewModel và Razor View của các luồng công việc, chi tiết công việc, phân công công việc và phân công chi tiết công việc.

Các file chính đã kiểm tra:

| Nhóm | File |
| --- | --- |
| Entity | `QuanLyDuAn/QuanLyDuAn/Models/Entities/CongViec.cs` |
| Entity | `QuanLyDuAn/QuanLyDuAn/Models/Entities/CtCongViec.cs` |
| Entity | `QuanLyDuAn/QuanLyDuAn/Models/Entities/PhanCongCongViec.cs` |
| Entity | `QuanLyDuAn/QuanLyDuAn/Models/Entities/PhanCongCtCongViec.cs` |
| Entity | `QuanLyDuAn/QuanLyDuAn/Models/Entities/NhanVienDuAn.cs` |
| Entity | `QuanLyDuAn/QuanLyDuAn/Models/Entities/NguoiDung.cs` |
| Entity | `QuanLyDuAn/QuanLyDuAn/Models/Entities/DanhMucCongViec.cs` |
| Entity nhật ký | `QuanLyDuAn/QuanLyDuAn/Models/Entities/NhatKyPhanCongCongViec.cs` |
| Entity nhật ký | `QuanLyDuAn/QuanLyDuAn/Models/Entities/NhatKyPhanCongCtCongViec.cs` |
| DbContext | `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs` |
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/CongViecController.cs` |
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/ChiTietCongViecController.cs` |
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/PhanCongCongViecController.cs` |
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/PhanCongChiTietCongViecController.cs` |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs` |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChiTietCongViecService.cs` |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanCongCongViecService.cs` |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanCongChiTietCongViecService.cs` |
| Service liên quan | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TienDoCongViecService.cs` |
| Service liên quan | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs` |
| Interface | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/ICongViecService.cs` |
| Interface | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IChiTietCongViecService.cs` |
| Interface | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IPhanCongCongViecService.cs` |
| Interface | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IPhanCongChiTietCongViecService.cs` |
| Permission | `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs` |
| DI | `QuanLyDuAn/QuanLyDuAn/Services/CauHinhDichVu.cs` |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/CongViec/*.cs` |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/ChiTietCongViec/*.cs` |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/PhanCongCongViec/*.cs` |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/PhanCongChiTietCongViec/*.cs` |
| View liên quan | `QuanLyDuAn/QuanLyDuAn/Views/CongViec/Index.cshtml` |
| View liên quan | `QuanLyDuAn/QuanLyDuAn/Views/CongViec/_Table.cshtml` |
| View liên quan | `QuanLyDuAn/QuanLyDuAn/Views/ChiTietCongViec/Index.cshtml` |
| View liên quan | `QuanLyDuAn/QuanLyDuAn/Views/ChiTietCongViec/_Table.cshtml` |
| View liên quan | `QuanLyDuAn/QuanLyDuAn/Views/PhanCongCongViec/Index.cshtml` |
| View liên quan | `QuanLyDuAn/QuanLyDuAn/Views/PhanCongCongViec/_Form.cshtml` |
| View liên quan | `QuanLyDuAn/QuanLyDuAn/Views/PhanCongCongViec/_Table.cshtml` |
| View liên quan | `QuanLyDuAn/QuanLyDuAn/Views/PhanCongChiTietCongViec/Index.cshtml` |
| View liên quan | `QuanLyDuAn/QuanLyDuAn/Views/PhanCongChiTietCongViec/_Form.cshtml` |
| View liên quan | `QuanLyDuAn/QuanLyDuAn/Views/PhanCongChiTietCongViec/_Table.cshtml` |
| Màn hình có dữ liệu người thực hiện | `QuanLyDuAn/QuanLyDuAn/Views/TienDoCongViec/_Table.cshtml` |
| Chi tiết dự án | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml` |

## 2. Cấu trúc dữ liệu phân công hiện tại

### 2.1 Phân công công việc

Entity `PhanCongCongViec` trong `QuanLyDuAn/QuanLyDuAn/Models/Entities/PhanCongCongViec.cs` có các thuộc tính:

| Thuộc tính | Ý nghĩa |
| --- | --- |
| `MaNguoiDung` | Người được phân công |
| `MaCongViec` | Công việc được phân công |
| `NgayGiaoViec` | Thời điểm giao việc |

Trong `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`, `PHAN_CONG_CONG_VIEC` được cấu hình:

| Thành phần | Cấu hình hiện tại |
| --- | --- |
| `DbSet` | `public virtual DbSet<PhanCongCongViec> PhanCongCongViec` |
| Bảng | `ToTable("PHAN_CONG_CONG_VIEC")` |
| Khóa chính | `HasKey(e => new { e.MaNguoiDung, e.MaCongViec })` |
| FK công việc | `HasOne<CongViec>().WithMany().HasForeignKey(d => d.MaCongViec)` |
| FK người dùng | `HasOne<NguoiDung>().WithMany().HasForeignKey(d => d.MaNguoiDung)` |

Kết luận từ khóa chính ghép: một công việc có thể được phân công cho nhiều người vì nhiều dòng có thể cùng `MaCongViec`; cùng một người không thể có hai dòng cho cùng một công việc vì khóa chính là `{ MaNguoiDung, MaCongViec }`. Vì vậy, khi đếm số người được phân công cho công việc, số bản ghi theo `MaCongViec` về mặt ràng buộc hiện tại tương đương số người dùng khác nhau. Tuy nhiên dùng `Distinct` theo `MaNguoiDung` vẫn là lựa chọn phòng thủ tốt nếu sau này có import dữ liệu hoặc thay đổi schema.

Entity phân công công việc không có `IsDeleted`, `DeletedAt`, `DeletedBy`, `TrangThai` hoặc `IsActive`. Service hiện xóa cứng bản ghi khi gỡ phân công. Vì vậy bản ghi tồn tại trong `PHAN_CONG_CONG_VIEC` chính là phân công đang có hiệu lực, với điều kiện người dùng liên quan chưa bị xóa mềm nếu cần hiển thị tên.

### 2.2 Phân công chi tiết công việc

Entity `PhanCongCtCongViec` trong `QuanLyDuAn/QuanLyDuAn/Models/Entities/PhanCongCtCongViec.cs` có các thuộc tính:

| Thuộc tính | Ý nghĩa |
| --- | --- |
| `MaNguoiDung` | Người được phân công |
| `MaChiTietCV` | Chi tiết công việc được phân công |
| `NgayGiaoCTCV` | Thời điểm giao chi tiết công việc |
| `VaiTroTrongCTCV` | Vai trò trong chi tiết công việc |

Trong `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`, `PHAN_CONG_CT_CONG_VIEC` được cấu hình:

| Thành phần | Cấu hình hiện tại |
| --- | --- |
| `DbSet` | `public virtual DbSet<PhanCongCtCongViec> PhanCongCtCongViec` |
| Bảng | `ToTable("PHAN_CONG_CT_CONG_VIEC")` |
| Khóa chính | `HasKey(e => new { e.MaNguoiDung, e.MaChiTietCV })` |
| FK chi tiết công việc | `HasOne<CtCongViec>().WithMany().HasForeignKey(d => d.MaChiTietCV)` |
| FK người dùng | `HasOne<NguoiDung>().WithMany().HasForeignKey(d => d.MaNguoiDung)` |

Kết luận từ khóa chính ghép: một chi tiết công việc có thể được phân công cho nhiều người vì nhiều dòng có thể cùng `MaChiTietCV`; cùng một người không thể có hai dòng cho cùng một chi tiết vì khóa chính là `{ MaNguoiDung, MaChiTietCV }`.

Entity phân công chi tiết cũng không có `IsDeleted`, trạng thái hoặc cờ hiệu lực. Service hiện xóa cứng khi gỡ phân công. Vì vậy bản ghi còn trong `PHAN_CONG_CT_CONG_VIEC` là phân công chi tiết đang có hiệu lực, với điều kiện người dùng chưa bị xóa mềm nếu cần hiển thị tên.

### 2.3 Điều kiện xác định phân công còn hiệu lực

Điều kiện nên dùng:

| Loại | Điều kiện hiệu lực |
| --- | --- |
| Công việc | Có bản ghi trong `PHAN_CONG_CONG_VIEC` với `MaCongViec` tương ứng |
| Chi tiết công việc | Có bản ghi trong `PHAN_CONG_CT_CONG_VIEC` với `MaChiTietCV` tương ứng |
| Khi hiển thị tên người | Join `NGUOI_DUNG` và lọc `NguoiDung.IsDeleted != true` |

Không nên dùng bảng nhật ký để xác định trạng thái hiện tại. `NhatKyPhanCongCongViec` và `NhatKyPhanCongCtCongViec` chỉ ghi `HanhDong...`, `ThoiGian...`, người được tác động và người ghi; service phân công gọi `GhiNhatKy` sau khi thêm hoặc xóa bản ghi chính. Một công việc từng có log phân công nhưng bản ghi chính đã bị xóa không được xem là đang phân công.

Rủi ro cần chú ý:

- `PHAN_CONG_CONG_VIEC` và `PHAN_CONG_CT_CONG_VIEC` không có xóa mềm, nên không có nguy cơ đếm bản ghi `IsDeleted = true` trong chính bảng phân công.
- Nếu join tên người mà không lọc `NguoiDung.IsDeleted != true`, có thể hiển thị hoặc đếm người đã bị xóa mềm. `PhanCongCongViecService.GetPageAsync` hiện chưa lọc `nd.IsDeleted != true` ở danh sách đã phân công công việc; `PhanCongChiTietCongViecService.GetPageAsync` có lọc `nd.IsDeleted != true` ở danh sách phân công chi tiết.
- Entity không khai báo navigation property collection trên `CongViec`, `CtCongViec`, `NguoiDung`; Fluent API dùng `WithMany()` không chỉ định navigation. Vì vậy phương án đọc danh sách nên dùng DbSet/subquery/group query trực tiếp thay vì kỳ vọng `cv.PhanCongCongViecs.Count`.

## 3. Luồng phân công công việc hiện tại

`PhanCongCongViecController` trong `QuanLyDuAn/QuanLyDuAn/Controllers/PhanCongCongViecController.cs` có các action:

| Class | Phương thức | Mục đích | Dữ liệu phân công được truy vấn? |
| --- | --- | --- | --- |
| `PhanCongCongViecController` | `Index(int maCongViec)` | Kiểm tra `Permissions.PhanCongCongViec.Xem`, gọi service tải trang phân công | Có, thông qua `PhanCongCongViecService.GetPageAsync` |
| `PhanCongCongViecController` | `ThemPhanCong(PhanCongCongViecPageViewModel vm)` | Kiểm tra `Permissions.PhanCongCongViec.ThucHien`, gọi thêm phân công | Có kiểm tra trùng trong service |
| `PhanCongCongViecController` | `XoaPhanCong(int maCongViec, int maNguoiDung)` | Kiểm tra `Permissions.PhanCongCongViec.ThucHien`, gọi xóa phân công | Có tìm bản ghi trong service |

`PhanCongCongViecService` trong `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanCongCongViecService.cs`:

| Class | Phương thức | Mục đích | Dữ liệu phân công được truy vấn? |
| --- | --- | --- | --- |
| `PhanCongCongViecService` | `GetPageAsync(int maCongViec)` | Tải summary công việc, danh sách đã phân công và danh sách nhân sự có thể phân công | Có, query `PhanCongCongViec` join `NguoiDung` theo `MaCongViec` |
| `PhanCongCongViecService` | `AddAsync(PhanCongCongViecCreateViewModel input)` | Thêm phân công công việc | Có, dùng `AnyAsync` chống trùng `{ MaCongViec, MaNguoiDung }`, sau đó `Add` bản ghi chính |
| `PhanCongCongViecService` | `RemoveAsync(int maCongViec, int maNguoiDung)` | Gỡ phân công công việc | Có, `FirstOrDefaultAsync` rồi `_context.PhanCongCongViec.Remove(entity)` |
| `PhanCongCongViecService` | `KiemTraNhanVienHopLeAsync(int maDuAn, int maNhanVien)` | Kiểm tra người được phân công tồn tại, chưa bị xóa mềm, tài khoản không khóa, thuộc dự án | Có kiểm tra `NhanVienDuAn` |
| `PhanCongCongViecService` | `KiemTraQuyenPhanCongAsync(int maDuAn, int maNguoiDungHienTai)` | Kiểm tra quyền nghiệp vụ phân công | Không đếm phân công, chỉ kiểm tra vai trò |

Hệ thống hiện cho phép nhiều nhân viên cho một công việc. Khi phân công lại cho người đã tồn tại, service báo lỗi "Nhân viên này đã được phân công cho công việc này.". Khi gỡ phân công, service xóa cứng bản ghi khỏi `PHAN_CONG_CONG_VIEC` và ghi nhật ký `NHAT_KY_PHAN_CONG_CONG_VIEC`.

Người được chọn bắt buộc thuộc dự án qua `NhanVienDuAn`, không bị xóa mềm và tài khoản không bị khóa. Quyền nghiệp vụ cho thao tác phân công gồm Manager là quản lý dự án, leader dự án hoặc trưởng team trong dự án. Service hiện trả `false` cho Admin trong `KiemTraQuyenPhanCongAsync`, nên Admin không trực tiếp thực hiện phân công dù có thể có permission theo seed.

## 4. Luồng phân công chi tiết công việc hiện tại

`PhanCongChiTietCongViecController` trong `QuanLyDuAn/QuanLyDuAn/Controllers/PhanCongChiTietCongViecController.cs` có các action:

| Class | Phương thức | Mục đích | Dữ liệu phân công được truy vấn? |
| --- | --- | --- | --- |
| `PhanCongChiTietCongViecController` | `Index(int maChiTietCv)` | Kiểm tra `Permissions.PhanCongChiTietCongViec.Xem`, gọi service tải trang phân công chi tiết | Có, thông qua `PhanCongChiTietCongViecService.GetPageAsync` |
| `PhanCongChiTietCongViecController` | `ThemPhanCong(PhanCongChiTietCongViecPageViewModel vm)` | Kiểm tra `Permissions.PhanCongChiTietCongViec.ThucHien`, gọi thêm phân công chi tiết | Có kiểm tra trùng trong service |
| `PhanCongChiTietCongViecController` | `XoaPhanCong(int maChiTietCv, int maNguoiDung)` | Kiểm tra `Permissions.PhanCongChiTietCongViec.ThucHien`, gọi xóa phân công chi tiết | Có tìm bản ghi trong service |

`PhanCongChiTietCongViecService` trong `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanCongChiTietCongViecService.cs`:

| Class | Phương thức | Mục đích | Dữ liệu phân công được truy vấn? |
| --- | --- | --- | --- |
| `PhanCongChiTietCongViecService` | `GetPageAsync(int maChiTietCv)` | Tải summary chi tiết, danh sách đã phân công, danh sách nhân sự có thể phân công | Có, query `PhanCongCtCongViec` join `NguoiDung`; danh sách có thể phân công join thêm `PhanCongCongViec` |
| `PhanCongChiTietCongViecService` | `AddAsync(PhanCongChiTietCongViecCreateViewModel input)` | Thêm phân công chi tiết | Có, dùng `AnyAsync` chống trùng `{ MaChiTietCV, MaNguoiDung }`, sau đó `Add` bản ghi chính |
| `PhanCongChiTietCongViecService` | `RemoveAsync(int maChiTietCv, int maNguoiDung)` | Gỡ phân công chi tiết | Có, `FirstOrDefaultAsync` rồi `_context.PhanCongCtCongViec.Remove(entity)` |
| `PhanCongChiTietCongViecService` | `KiemTraNhanVienHopLeAsync(int maDuAn, int maCongViec, int maNhanVien)` | Kiểm tra người được phân công tồn tại, chưa bị xóa mềm, thuộc dự án và đã được phân công công việc cha | Có kiểm tra `NhanVienDuAn` và `PhanCongCongViec` |
| `PhanCongChiTietCongViecService` | `KiemTraQuyenPhanCongAsync(int maDuAn, int maNguoiDungHienTai)` | Kiểm tra quyền nghiệp vụ phân công chi tiết | Không đếm phân công, chỉ kiểm tra vai trò |

Hệ thống hiện cho phép nhiều nhân viên cho một chi tiết công việc. Tuy nhiên nhân viên được phân công chi tiết phải đồng thời thuộc dự án và đã được phân công ở công việc cha. Vì vậy theo workflow hiện tại:

- Có thể có công việc cha đã phân công nhưng một số chi tiết chưa phân công.
- Không thể thêm phân công chi tiết cho người chưa có phân công công việc cha.
- Nếu công việc cha chưa phân công cho ai, danh sách `ThanhVienCoThePhanCong` của chi tiết rỗng vì query join `PhanCongCongViec`.
- Cùng một người được phép vừa được phân công ở công việc cha vừa được phân công ở một hoặc nhiều chi tiết thuộc công việc đó; đây là điều kiện bắt buộc cho chi tiết.

Khi gỡ phân công chi tiết, service xóa cứng bản ghi khỏi `PHAN_CONG_CT_CONG_VIEC` và ghi nhật ký `NHAT_KY_PHAN_CONG_CT_CONG_VIEC`.

## 5. Bảng danh sách công việc hiện tại

### 5.1 Controller và Service tải dữ liệu

`CongViecController.Index` trong `QuanLyDuAn/QuanLyDuAn/Controllers/CongViecController.cs` kiểm tra `Permissions.CongViec.Xem`, gọi:

```text
CongViecService.GetPageAsync(locMaDuAn, locTrangThai, tuKhoa, tuNgay, denNgay, locTheoNgay, pageNumber, pageSize)
```

`CongViecService.GetPageAsync` trong `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs` là hàm chính tải bảng danh sách công việc. Query hiện tại:

- Join `CongViec`, `DanhMucCongViec`, `DuAn`, `MucDoUuTien`.
- Group join `ChiPhi` để tính `ChiPhiDaChi`.
- Lọc `cv.IsDeleted != true`, `dm.IsDeleted != true`, `da.IsDeleted != true`.
- Áp dụng phạm vi dự án theo role: Admin thấy dự án chưa xóa; Manager thấy dự án mình quản lý; Employee thấy dự án tham gia.
- Với Employee không phải Manager/Admin, lọc thêm: là leader dự án hoặc có bản ghi `PhanCongCongViec` với chính người dùng hiện tại.
- Phân trang sau `CountAsync`, `OrderByDescending(NgayTaoCongViec)`, `Skip/Take`, rồi `ToListAsync`.

Dữ liệu phân công hiện được dùng trong hàm này chỉ để lọc phạm vi Employee và để set cờ hành động `CoThePhanCongCongViec`, chưa được đưa vào ViewModel để hiển thị trạng thái hoặc số người được phân công.

Hàm hiện không dùng `AsNoTracking`. Đây không phải lỗi chức năng, nhưng khi bổ sung dữ liệu chỉ đọc có thể cân nhắc dùng `AsNoTracking` nếu thống nhất với pattern của module.

### 5.2 ViewModel

`CongViecItemViewModel` trong `QuanLyDuAn/QuanLyDuAn/ViewModels/CongViec/CongViecItemViewModel.cs` hiện có:

| Thuộc tính | Vai trò hiện tại |
| --- | --- |
| `MaCongViec`, `MaDuAn`, `MaDanhMucCV`, `MaMucDo` | Định danh |
| `TenDuAn`, `TenDanhMucCV`, `TenMucDo`, `TenCongViec`, `MoTaCongViec` | Hiển thị thông tin công việc |
| `ChiPhiDaChi` | Hiển thị chi phí |
| `NgayBatDauCongViec`, `NgayKetThucCVDuKien`, `NgayKetThucCVThucTe`, `NgayTaoCongViec` | Hiển thị thời gian |
| `TrangThaiCongViec`, `TrangThaiHienThi`, `CssTrangThai`, `ThongDiepWorkflow` | Hiển thị trạng thái workflow |
| `CoThePhanCongCongViec` | Cờ cho phép hiện nút hành động phân công |
| `CoTheXacNhanHoanThanh`, `CoTheMoLai` | Cờ hành động workflow |

ViewModel chưa có trường thể hiện:

- Công việc đã phân công hay chưa.
- Số người được phân công.
- Danh sách tên người được phân công.

Nếu bổ sung, không nên tạo trường trùng chức năng với `CoThePhanCongCongViec` vì cờ này là quyền/thao tác, không phải trạng thái dữ liệu. Có thể thêm `SoNguoiDuocPhanCong` và tính `DaPhanCong = SoNguoiDuocPhanCong > 0` ở ViewModel dạng property chỉ đọc, hoặc tính trực tiếp trong View. Nếu cần serialize/export rõ ràng, có thể thêm cả hai nhưng về mặt dữ liệu chỉ cần số lượng.

### 5.3 Razor View

`QuanLyDuAn/QuanLyDuAn/Views/CongViec/_Table.cshtml` hiển thị bảng desktop với các cột:

| Cột | Nội dung |
| --- | --- |
| `Dự án` | Tên dự án, danh mục |
| `Công việc` | Tên công việc, mô tả |
| `Mức độ / Chi phí` | Mức độ ưu tiên, chi phí đã chi |
| `Mốc thời gian` | Bắt đầu, kết thúc dự kiến |
| `Trạng thái` | Badge workflow |
| `Thao tác` | Nút Phân công, Chi tiết, Xác nhận hoàn thành, Mở lại |

Mobile card hiển thị thông tin tương tự và các nút hành động. Nút `Phân công` chỉ xuất hiện khi có `Permissions.PhanCongCongViec.Xem` và `item.CoThePhanCongCongViec`.

Vị trí phù hợp để thêm trạng thái phân công:

- Desktop: thêm cột nhỏ `Phân công` giữa `Trạng thái` và `Thao tác`, hoặc gộp thành dòng phụ trong cột `Công việc` để tránh tăng độ rộng bảng.
- Mobile: thêm một dòng trong `task-mobile-meta`, ví dụ `Phân công: Chưa phân công` hoặc `Đã phân công · 3 người`.

Bảng desktop đã có 6 cột và cột thao tác khá nhiều nút, nên nếu thêm cột riêng cần giữ nội dung ngắn dạng badge. Không nên đưa danh sách tên dài vào bảng desktop mặc định.

### 5.4 Trạng thái phân công hiện đang hiển thị

Hiện tại bảng công việc chưa hiển thị trạng thái phân công và chưa hiển thị số người được phân công. Nút `Phân công` là hành động điều hướng sang `PhanCongCongViecController.Index`, không phải bằng chứng công việc đã được phân công.

Trang riêng `QuanLyDuAn/QuanLyDuAn/Views/PhanCongCongViec/Index.cshtml` có summary `Đã phân công` bằng `Model.DanhSachPhanCong.Count`, nhưng thông tin này chỉ xuất hiện sau khi mở trang phân công của từng công việc, không xuất hiện trên bảng danh sách công việc.

## 6. Bảng danh sách chi tiết công việc hiện tại

### 6.1 Controller và Service tải dữ liệu

`ChiTietCongViecController.Index` trong `QuanLyDuAn/QuanLyDuAn/Controllers/ChiTietCongViecController.cs` kiểm tra `Permissions.ChiTietCongViec.Xem`, gọi:

```text
ChiTietCongViecService.GetPageAsync(maCongViec, pageNumber, pageSize)
```

`ChiTietCongViecService.GetPageAsync` trong `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChiTietCongViecService.cs` là hàm chính tải bảng chi tiết công việc. Query hiện tại:

- Gọi `LayCongViecAsync(maCongViec)` để lấy công việc cha chưa xóa mềm.
- Tạo query `_context.CtCongViec.Where(x => x.MaCongViec == maCongViec && x.IsDeleted != true)`.
- Đếm tổng chi tiết và số chi tiết hoàn thành.
- Phân trang bằng `OrderByDescending(NgayTaoCTCV)`, `ThenByDescending(MaChiTietCV)`, `Skip/Take`.
- Project sang `ChiTietCongViecItemViewModel`.
- Set cùng một cờ `CoThePhanCongChiTietCongViec` cho từng dòng dựa trên quyền nghiệp vụ với công việc cha.

Hàm này chưa query `PhanCongCtCongViec` để hiển thị trạng thái phân công hay số người được phân công cho từng chi tiết.

Chi tiết chưa phân công vẫn được hiển thị vì danh sách lấy từ `CtCongViec` theo công việc cha và không join bắt buộc bảng phân công. Đây là điểm thuận lợi để bổ sung trạng thái "Chưa phân công" mà không làm mất dòng.

### 6.2 ViewModel

`ChiTietCongViecItemViewModel` trong `QuanLyDuAn/QuanLyDuAn/ViewModels/ChiTietCongViec/ChiTietCongViecItemViewModel.cs` hiện có:

| Thuộc tính | Vai trò hiện tại |
| --- | --- |
| `MaChiTietCV`, `MaCongViec` | Định danh |
| `TenCTCV`, `NoiDungChiTietCV` | Hiển thị nội dung chi tiết |
| `NgayTaoCTCV`, `NgayBatDauCTCV`, `NgayKetThucCTCV` | Hiển thị thời gian |
| `TrangThaiCTCV`, `TrangThaiHienThi`, `CssTrangThai`, `ThongDiepWorkflow` | Hiển thị trạng thái workflow |
| `CoThePhanCongChiTietCongViec` | Cờ cho phép hiện nút hành động phân công |

ViewModel chưa có:

- Trạng thái đã phân công/chưa phân công.
- Số người được phân công cho chi tiết.
- Danh sách tên người thực hiện.

`TienDoCongViecItemViewModel` trong `QuanLyDuAn/QuanLyDuAn/ViewModels/TienDoCongViec/TienDoCongViecItemViewModel.cs` đã có `NguoiThucHien`, và `TienDoCongViecService.GetPageAsync` hiện lấy tên người thực hiện từ `PhanCongCtCongViec` theo cụm `detailIds`. Tuy nhiên ViewModel này thuộc màn hình tiến độ, không được dùng bởi bảng chi tiết công việc ở `ChiTietCongViec/_Table.cshtml`.

### 6.3 Razor View

`QuanLyDuAn/QuanLyDuAn/Views/ChiTietCongViec/_Table.cshtml` hiển thị bảng desktop với các cột:

| Cột | Nội dung |
| --- | --- |
| `Tên CTCV` | Tên chi tiết |
| `Nội dung` | Nội dung chi tiết |
| `Ngày bắt đầu` | Ngày bắt đầu |
| `Ngày kết thúc` | Ngày kết thúc |
| `Ngày tạo` | Ngày tạo |
| `Trạng thái` | Badge workflow |
| `Thao tác` | Nút Phân công, Sửa, Xóa |

Mobile card hiển thị tên, trạng thái, nội dung, các mốc ngày và nút hành động.

Vị trí phù hợp để thêm trạng thái phân công:

- Desktop: thêm cột `Phân công` giữa `Trạng thái` và `Thao tác`, hoặc hiển thị dòng phụ dưới `Tên CTCV`.
- Mobile: thêm dòng trong `mobile-meta` hoặc dưới trạng thái.

Bảng chi tiết hiện đã có 7 cột và còn dòng collapse sửa inline với `colspan="7"`. Nếu thêm cột desktop riêng, cần cập nhật `colspan` tương ứng trong View. Đây là sửa View cục bộ, không ảnh hưởng route/workflow, nhưng cần làm đồng bộ để tránh vỡ layout.

### 6.4 Trạng thái phân công hiện đang hiển thị

Hiện tại bảng chi tiết công việc chưa hiển thị trạng thái phân công và chưa hiển thị số người được phân công. Nút `Phân công` là hành động, không phải trạng thái.

Trang riêng `QuanLyDuAn/QuanLyDuAn/Views/PhanCongChiTietCongViec/Index.cshtml` có summary `Đã phân công` bằng `Model.DanhSachPhanCong.Count`, nhưng thông tin này chỉ xuất hiện trong trang phân công của một chi tiết cụ thể.

## 7. Quyền và phạm vi dữ liệu

Quyền trong `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`:

| Module | Permission |
| --- | --- |
| Công việc | `Permissions.CongViec.Xem` |
| Chi tiết công việc | `Permissions.ChiTietCongViec.Xem`, `Them`, `Sua`, `Xoa` |
| Phân công công việc | `Permissions.PhanCongCongViec.Xem`, `ThucHien` |
| Phân công chi tiết công việc | `Permissions.PhanCongChiTietCongViec.Xem`, `ThucHien` |
| Tiến độ | `Permissions.TienDo.Xem`, `CapNhat`, `Duyet` |

Phạm vi xem danh sách công việc hiện do `CongViecService.GetPageAsync` quyết định theo role và dự án:

- Admin thấy các dự án chưa xóa mềm.
- Manager thấy dự án mình quản lý.
- Employee thấy dự án mình tham gia, nhưng với danh sách công việc còn bị thu hẹp: leader dự án thấy công việc trong dự án, người thường chỉ thấy công việc được phân công cho mình.

Phạm vi xem chi tiết tiến độ trong `TienDoCongViecService.GetAccessibleChiTietCongViecIdsAsync` cũng dựa trên manager dự án, leader dự án, trưởng team và chi tiết được phân công trực tiếp.

Bổ sung số lượng phân công vào các bảng hiện có không làm lộ dữ liệu ngoài phạm vi nếu tính số lượng chỉ trên các dòng đã qua filter danh sách hiện tại. Tên người được phân công nhạy hơn số lượng vì có thể lộ danh sách nhân sự của dự án. Theo UI hiện tại:

- Trang phân công riêng đã hiển thị tên nhân viên khi người dùng có quyền `PhanCongCongViec.Xem` hoặc `PhanCongChiTietCongViec.Xem`.
- Bảng tiến độ đã hiển thị `NguoiThucHien` theo chi tiết trong phạm vi màn hình tiến độ.
- Bảng công việc/chi tiết nên ưu tiên hiển thị trạng thái và số lượng. Nếu hiển thị tên, nên giới hạn rút gọn và chỉ khi permission hiện tại cho phép xem phân công tương ứng.

Không cần thay đổi permission hoặc workflow để bổ sung số lượng. Nên giữ nguyên kiểm tra quyền hiện tại và chỉ thêm dữ liệu hiển thị trong phạm vi query đã được phép xem.

## 8. Các vấn đề phát hiện

| STT | Vấn đề | Căn cứ source | Tác động |
| --- | --- | --- | --- |
| 1 | Bảng công việc không có trạng thái phân công | `CongViecItemViewModel` không có trường số lượng/trạng thái; `CongViec/_Table.cshtml` chỉ có cột trạng thái workflow và nút `Phân công` | Người dùng không biết công việc đã giao cho ai/chưa khi nhìn danh sách |
| 2 | Bảng công việc không có số người được phân công | `CongViecService.GetPageAsync` không project count từ `PhanCongCongViec` | Phải vào từng trang phân công mới biết số người |
| 3 | Bảng chi tiết công việc không có trạng thái phân công | `ChiTietCongViecItemViewModel` không có trường số lượng/trạng thái; `ChiTietCongViec/_Table.cshtml` chỉ có nút `Phân công` | Người dùng không biết chi tiết nào chưa giao |
| 4 | Bảng chi tiết công việc không có số người được phân công | `ChiTietCongViecService.GetPageAsync` không query `PhanCongCtCongViec` | Không nhìn nhanh được mức phân bổ nhân sự theo chi tiết |
| 5 | Tên người thực hiện đã có ở màn hình tiến độ nhưng không dùng chung với bảng chi tiết | `TienDoCongViecService.GetPageAsync` group `PhanCongCtCongViec` theo `detailIds`, còn `ChiTietCongViecService.GetPageAsync` không làm việc này | Cùng dữ liệu có ở DB nhưng hiển thị không nhất quán giữa màn hình |
| 6 | Danh sách đã phân công công việc chưa lọc `NguoiDung.IsDeleted != true` | `PhanCongCongViecService.GetPageAsync` join `NguoiDung` nhưng không có điều kiện `nd.IsDeleted != true` ở danh sách `DanhSachPhanCong` | Nếu người dùng bị xóa mềm vẫn còn bản ghi phân công, trang phân công công việc có thể hiển thị tên người đã xóa mềm |
| 7 | View chi tiết có `colspan="7"` cố định | `ChiTietCongViec/_Table.cshtml` dùng `td colspan="7"` cho dòng sửa inline | Khi thêm cột cần cập nhật colspan để không lệch bảng |

## 9. Khả năng bổ sung thông tin phân công mà không sửa database

Database hiện có đủ dữ liệu để bổ sung:

- `PHAN_CONG_CONG_VIEC` chứa quan hệ hiện hành giữa người dùng và công việc.
- `PHAN_CONG_CT_CONG_VIEC` chứa quan hệ hiện hành giữa người dùng và chi tiết công việc.
- `NGUOI_DUNG` có `HoTenNguoiDung` và `IsDeleted` để lọc tên hiển thị.
- Khóa chính ghép chống trùng cùng người trong cùng công việc/chi tiết.

Không cần sửa database, không cần migration, không cần thêm cột `DaPhanCong` hoặc `SoNguoiDuocPhanCong`. Đây là dữ liệu tính được từ bảng phân công chính thức.

Có thể bổ sung bằng ViewModel, Service và View:

- ViewModel: thêm trường chỉ phục vụ hiển thị như `SoNguoiDuocPhanCong`, có thể thêm property chỉ đọc `DaPhanCong => SoNguoiDuocPhanCong > 0`.
- Service: tính số lượng phân công theo cụm sau khi có danh sách trang hiện tại, hoặc bằng subquery EF Core dịch được sang SQL.
- View: hiển thị badge ngắn, ví dụ `Chưa phân công`, `Đã phân công · 1 người`, `Đã phân công · 3 người`.

## 10. Phương án đề xuất

### 10.1 Công việc

Đề xuất bổ sung vào `CongViecItemViewModel`:

```csharp
public int SoNguoiDuocPhanCong { get; set; }
public bool DaPhanCong => SoNguoiDuocPhanCong > 0;
```

Chỉ thêm `TenNguoiDuocPhanCong` nếu thật sự cần tooltip hoặc danh sách rút gọn. Với bảng công việc hiện nhiều cột và cột thao tác đã dày, giai đoạn đầu nên hiển thị số lượng thay vì tên.

Trong `CongViecService.GetPageAsync`, sau khi `danhSach = await danhSachQuery.ToListAsync();`, lấy `maCongViecIds` của trang hiện tại rồi query theo cụm:

```text
PhanCongCongViec
  .Where(pc => maCongViecIds.Contains(pc.MaCongViec))
  .GroupBy(pc => pc.MaCongViec)
  .Select(g => new { MaCongViec = g.Key, SoNguoi = g.Select(x => x.MaNguoiDung).Distinct().Count() })
```

Sau đó map dictionary vào từng item. Cách này không thay đổi query lọc/phân trang hiện tại, không kéo toàn bộ bảng phân công và tránh N+1.

Nếu cần chỉ đếm người dùng còn hiệu lực, join thêm `NguoiDung` và lọc `nd.IsDeleted != true`. Với yêu cầu hiển thị cho người dùng cuối, nên lọc người dùng chưa xóa mềm để tránh tính người đã xóa.

### 10.2 Chi tiết công việc

Đề xuất bổ sung vào `ChiTietCongViecItemViewModel`:

```csharp
public int SoNguoiDuocPhanCong { get; set; }
public bool DaPhanCong => SoNguoiDuocPhanCong > 0;
```

Nếu muốn đồng bộ với màn hình tiến độ, có thể thêm:

```csharp
public List<string> TenNguoiDuocPhanCong { get; set; } = new();
```

Tuy nhiên bảng chi tiết đã nhiều cột. Nên ưu tiên số lượng, còn tên cụ thể xem ở trang phân công chi tiết hoặc màn hình tiến độ.

Trong `ChiTietCongViecService.GetPageAsync`, sau khi lấy `danhSach`, query theo cụm `MaChiTietCV` của trang hiện tại:

```text
PhanCongCtCongViec
  .Where(pc => maChiTietIds.Contains(pc.MaChiTietCV))
  .GroupBy(pc => pc.MaChiTietCV)
  .Select(g => new { MaChiTietCV = g.Key, SoNguoi = g.Select(x => x.MaNguoiDung).Distinct().Count() })
```

Nếu cần tên người, có thể học theo `TienDoCongViecService.GetPageAsync`: lấy `detailIds`, join `PhanCongCtCongViec` với `NguoiDung`, lọc `nd.IsDeleted != true`, `ToListAsync`, group trong memory theo `MaChiTietCV`. Cách này đã xuất hiện trong source nên tương thích với pattern hiện tại.

### 10.3 Cách tính số người được phân công

So sánh các cách:

| Cách | Ưu điểm | Nhược điểm | Đánh giá |
| --- | --- | --- | --- |
| Navigation property và `Count` | Code ngắn nếu có navigation | Entity hiện không có navigation collection | Không phù hợp source hiện tại |
| Subquery `Count` trong projection | EF Core dịch được nếu dùng biểu thức đơn giản | Có thể tạo subquery cho mỗi dòng trong SQL; khó thêm tên người | Có thể dùng cho số lượng đơn giản |
| `Any` | Tốt cho `DaPhanCong` | Không có số lượng | Chỉ phù hợp nếu không cần count |
| Group theo `MaCongViec`/`MaChiTietCV` sau khi có ID trang | Tránh N+1, tương thích phân trang, không tải dư nhiều | Cần map dictionary sau `ToListAsync` | Phù hợp nhất |
| Include toàn bộ bảng phân công | Đơn giản nếu navigation có sẵn | Dễ tải dư, entity hiện không có navigation | Không nên dùng |
| Query riêng từng dòng | Dễ viết | N+1 query, chậm khi phân trang nhiều dòng | Không dùng |

Vì khóa chính đã chống trùng theo người dùng + công việc/chi tiết, `Count()` và `Distinct().Count()` hiện cho kết quả giống nhau. Nên dùng `Distinct` theo `MaNguoiDung` để diễn đạt đúng yêu cầu "bao nhiêu người" và an toàn hơn.

### 10.4 Cách hiển thị trên giao diện

Đề xuất format:

```text
Chưa phân công
Đã phân công · 1 người
Đã phân công · 3 người
```

Phù hợp style hiện tại:

- Dùng badge giống `workflow-badge` hoặc class riêng cùng tinh thần Bootstrap icon/badge.
- Trạng thái chưa phân công nên dùng màu trung tính/cảnh báo nhẹ.
- Trạng thái đã phân công dùng màu thành công hoặc trung tính tích cực.
- Không đặt nhầm trong cột `Thao tác`; cần phân biệt thông tin trạng thái với nút `Phân công`.

Với bảng công việc, nên thêm cột ngắn hoặc dòng phụ dưới tên công việc. Với bảng chi tiết, vì đã có nhiều cột, phương án dòng phụ dưới `Tên CTCV` có thể ít làm vỡ layout hơn cột mới. Nếu vẫn thêm cột riêng, cần cập nhật `colspan` dòng sửa inline.

### 10.5 Phương án tránh N+1 query

Phương án phù hợp nhất:

1. Giữ query danh sách hiện tại và phân trang như cũ.
2. Sau khi lấy danh sách trang hiện tại bằng `ToListAsync`, lấy danh sách ID (`MaCongViec` hoặc `MaChiTietCV`).
3. Query bảng phân công theo cụm với `Contains`.
4. Group theo ID, dùng `Distinct().Count()` theo `MaNguoiDung`.
5. Map kết quả vào ViewModel bằng dictionary.

Cách này tương thích với EF Core, tránh các biểu thức dễ lỗi dịch như `string.Format`, custom function trong `IQueryable`, `StringComparison` trong database query, và không kéo toàn bộ dữ liệu về memory để đếm.

## 11. Danh sách file dự kiến cần sửa

| STT | File | Thành phần | Thay đổi dự kiến | Có ảnh hưởng workflow không |
| --- | --- | --- | --- | --- |
| 1 | `QuanLyDuAn/QuanLyDuAn/ViewModels/CongViec/CongViecItemViewModel.cs` | `CongViecItemViewModel` | Thêm trường số người phân công và property trạng thái hiển thị nếu cần | Không |
| 2 | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs` | `CongViecService.GetPageAsync` | Sau phân trang, group count `PhanCongCongViec` theo danh sách `MaCongViec` của trang | Không |
| 3 | `QuanLyDuAn/QuanLyDuAn/Views/CongViec/_Table.cshtml` | Bảng desktop/mobile công việc | Hiển thị badge `Chưa phân công` hoặc `Đã phân công · n người` | Không |
| 4 | `QuanLyDuAn/QuanLyDuAn/ViewModels/ChiTietCongViec/ChiTietCongViecItemViewModel.cs` | `ChiTietCongViecItemViewModel` | Thêm trường số người phân công và property trạng thái hiển thị nếu cần | Không |
| 5 | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChiTietCongViecService.cs` | `ChiTietCongViecService.GetPageAsync` | Sau phân trang, group count `PhanCongCtCongViec` theo danh sách `MaChiTietCV` của trang | Không |
| 6 | `QuanLyDuAn/QuanLyDuAn/Views/ChiTietCongViec/_Table.cshtml` | Bảng desktop/mobile chi tiết | Hiển thị badge phân công; nếu thêm cột desktop thì cập nhật `colspan` dòng collapse | Không |

Nếu muốn đưa số lượng vào file export, có thể sửa thêm:

| STT | File | Thành phần | Thay đổi dự kiến | Có ảnh hưởng workflow không |
| --- | --- | --- | --- | --- |
| 7 | `QuanLyDuAn/QuanLyDuAn/Controllers/CongViecController.cs` | `XuatFile` | Thêm cột export trạng thái/số người phân công | Không |
| 8 | `QuanLyDuAn/QuanLyDuAn/Controllers/ChiTietCongViecController.cs` | `XuatFile` | Thêm cột export trạng thái/số người phân công | Không |

Hai file export không bắt buộc cho mục tiêu hiển thị bảng hiện tại.

## 12. Các file không cần sửa

| File | Lý do không cần sửa |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/PhanCongCongViec.cs` | Entity đã đủ dữ liệu hiện hành để đếm |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/PhanCongCtCongViec.cs` | Entity đã đủ dữ liệu hiện hành để đếm |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/CongViec.cs` | Không cần thêm cột/property lưu trạng thái tính toán |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/CtCongViec.cs` | Không cần thêm cột/property lưu trạng thái tính toán |
| `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs` | DbSet và quan hệ đã có; không cần đổi mapping |
| `QuanLyDuAn/QuanLyDuAn/Controllers/PhanCongCongViecController.cs` | Luồng phân công riêng đã hoạt động, không cần đổi route/action |
| `QuanLyDuAn/QuanLyDuAn/Controllers/PhanCongChiTietCongViecController.cs` | Luồng phân công chi tiết riêng đã hoạt động, không cần đổi route/action |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanCongCongViecService.cs` | Không cần đổi nghiệp vụ phân công để hiển thị số lượng ở bảng danh sách |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanCongChiTietCongViecService.cs` | Không cần đổi nghiệp vụ phân công chi tiết để hiển thị số lượng ở bảng danh sách |
| `QuanLyDuAn/QuanLyDuAn/Views/PhanCongCongViec/*.cshtml` | Trang riêng đã hiển thị danh sách và số đã phân công |
| `QuanLyDuAn/QuanLyDuAn/Views/PhanCongChiTietCongViec/*.cshtml` | Trang riêng đã hiển thị danh sách và số đã phân công |
| `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs` | Không cần thêm hoặc đổi permission |
| `QuanLyDuAn/QuanLyDuAn/Migrations/*` | Không sửa database, không tạo migration |

## 13. Rủi ro khi triển khai

| Rủi ro | Nguyên nhân | Cách tránh |
| --- | --- | --- |
| N+1 query | Query count riêng cho từng công việc/chi tiết trong vòng lặp | Query theo cụm ID của trang hiện tại rồi map dictionary |
| Đếm người đã xóa mềm | Join tên người nhưng không lọc `NguoiDung.IsDeleted != true` | Khi cần tên hoặc count người còn hiệu lực, join `NguoiDung` và lọc `IsDeleted != true` |
| Lệch giao diện bảng chi tiết | Thêm cột nhưng quên cập nhật `colspan="7"` | Nếu thêm cột, cập nhật colspan theo số cột mới |
| Nhầm nút hành động với trạng thái | `CoThePhanCong...` là quyền hiện nút, không phải trạng thái phân công | Dùng trường ViewModel riêng cho số lượng/trạng thái |
| Không nhất quán giữa màn hình | Màn hình tiến độ đã có `NguoiThucHien`, bảng chi tiết dùng logic khác | Dùng cùng quy tắc: bản ghi chính trong bảng phân công còn tồn tại và người dùng chưa xóa mềm khi hiển thị tên |
| Tải dư tên người | Hiển thị danh sách tên đầy đủ trên bảng nhiều dòng | Giai đoạn đầu chỉ hiển thị số lượng; tên xem ở trang phân công hoặc tooltip rút gọn nếu cần |
| Lỗi dịch EF Core | Đưa custom method, `string.Format`, `StringComparison` vào `IQueryable` | Chỉ dùng `Where`, `Contains`, `GroupBy`, `Select`, `Distinct`, `Count` dịch được sang SQL |

## 14. Kết luận

| Câu hỏi | Trả lời |
| --- | --- |
| 1. Hiện tại bảng công việc đã hiển thị trạng thái phân công chưa? | Chưa. `CongViec/_Table.cshtml` chỉ hiển thị trạng thái workflow và nút hành động `Phân công`. |
| 2. Hiện tại bảng công việc đã hiển thị số người được phân công chưa? | Chưa. `CongViecItemViewModel` và `CongViecService.GetPageAsync` chưa có trường/count phân công. |
| 3. Hiện tại bảng chi tiết công việc đã hiển thị trạng thái phân công chưa? | Chưa. `ChiTietCongViec/_Table.cshtml` chỉ có nút hành động `Phân công`. |
| 4. Hiện tại bảng chi tiết công việc đã hiển thị số người được phân công chưa? | Chưa. `ChiTietCongViecItemViewModel` và `ChiTietCongViecService.GetPageAsync` chưa query `PhanCongCtCongViec` để đếm. |
| 5. Database hiện tại có đủ dữ liệu để bổ sung hay không? | Có. `PHAN_CONG_CONG_VIEC` và `PHAN_CONG_CT_CONG_VIEC` là bảng phân công chính thức, có khóa chính ghép chống trùng người trong cùng công việc/chi tiết. |
| 6. Có cần sửa database hay không? | Không. Không cần thêm bảng, cột, migration hoặc lưu `DaPhanCong`/`SoNguoiDuocPhanCong`. |
| 7. Những ViewModel, Service và View nào dự kiến cần sửa? | `CongViecItemViewModel`, `CongViecService.GetPageAsync`, `Views/CongViec/_Table.cshtml`, `ChiTietCongViecItemViewModel`, `ChiTietCongViecService.GetPageAsync`, `Views/ChiTietCongViec/_Table.cshtml`. |
| 8. Có thể triển khai mà không thay đổi workflow và quyền hiện tại hay không? | Có. Chỉ bổ sung dữ liệu hiển thị trong phạm vi danh sách đã được phép xem; không đổi route, action, permission hay nghiệp vụ phân công. |
| 9. Phương án nào phù hợp nhất để tránh N+1 query? | Sau khi lấy trang hiện tại, lấy danh sách ID rồi query bảng phân công theo cụm, `GroupBy` theo `MaCongViec` hoặc `MaChiTietCV`, `Distinct().Count()` theo `MaNguoiDung`, map dictionary vào ViewModel. |
| 10. Có nên hiển thị tên người được phân công ngay trên bảng hay chỉ hiển thị trạng thái và số lượng? | Nên ưu tiên trạng thái và số lượng trên bảng. Tên cụ thể nên xem ở trang phân công hoặc màn hình tiến độ; nếu hiển thị trên bảng thì chỉ nên rút gọn/tooltip và tôn trọng permission xem phân công. |

## 15. Kết quả triển khai

Ngày triển khai: 19/06/2026.

Các file thực tế đã sửa:

- `QuanLyDuAn/QuanLyDuAn/ViewModels/CongViec/CongViecItemViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/CongViec/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/ChiTietCongViec/ChiTietCongViecItemViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChiTietCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/ChiTietCongViec/_Table.cshtml`
- `docs/phancong.md`

Thuộc tính ViewModel đã thêm:

- `CongViecItemViewModel`: `SoNguoiDuocPhanCong` và `DaPhanCong => SoNguoiDuocPhanCong > 0`.
- `ChiTietCongViecItemViewModel`: `SoNguoiDuocPhanCong` và `DaPhanCong => SoNguoiDuocPhanCong > 0`.

Cách Service lấy số lượng phân công:

- `CongViecService.GetPageAsync` giữ nguyên query lọc, data scope, sắp xếp và phân trang hiện có. Sau khi lấy danh sách trang hiện tại bằng `ToListAsync`, service lấy các `MaCongViec`, query `PhanCongCongViec` theo cụm ID, join `NguoiDung`, lọc `NguoiDung.IsDeleted != true`, group theo `MaCongViec`, đếm `MaNguoiDung` khác nhau bằng `Distinct().Count()`, rồi map dictionary vào từng ViewModel.
- `ChiTietCongViecService.GetPageAsync` giữ nguyên query chi tiết, đếm tổng, đếm hoàn thành, sắp xếp, phân trang và cờ thao tác hiện có. Sau khi lấy danh sách trang hiện tại, service lấy các `MaChiTietCV`, query `PhanCongCtCongViec` theo cụm ID, join `NguoiDung`, lọc `NguoiDung.IsDeleted != true`, group theo `MaChiTietCV`, đếm `MaNguoiDung` khác nhau bằng `Distinct().Count()`, rồi map dictionary vào từng ViewModel.

Cách tránh N+1 query:

- Không query số người trong vòng lặp từng dòng.
- Không tải toàn bộ bảng phân công.
- Chỉ query bảng phân công theo danh sách ID của trang hiện tại.
- Vòng lặp cuối chỉ map kết quả từ dictionary đã có sẵn vào ViewModel.

Cách hiển thị desktop:

- Bảng công việc hiển thị badge phân công dưới tên công việc trong cột `Công việc`.
- Bảng chi tiết công việc hiển thị badge phân công dưới tên chi tiết trong cột `Tên CTCV`.
- Nội dung hiển thị: `Chưa phân công` hoặc `Đã phân công · n người`.
- Không thêm cột mới, nên header/body và `colspan="7"` của dòng sửa inline chi tiết không thay đổi.

Cách hiển thị mobile:

- Card công việc thêm dòng `Phân công` trong phần thông tin mobile.
- Card chi tiết công việc thêm dòng `Phân công` trong phần thông tin mobile.
- Không hiển thị danh sách tên nhân viên trên bảng hoặc card.

Kết quả kiểm tra phạm vi:

- Không sửa database.
- Không tạo migration.
- Không thêm property vào Entity `CongViec` hoặc `CtCongViec`.
- Không sửa `QuanLyDuAnDbContext`.
- Không sửa controller, route, action, form action, workflow hoặc permission.
- Không dùng bảng nhật ký phân công để xác định trạng thái hiện tại.

Kết quả build:

- Đã chạy `dotnet build QuanLyDuAn/QuanLyDuAn.sln` ngày 19/06/2026.
- Build thành công, không có lỗi compile hoặc lỗi Razor compilation.
- Build có 2 warning CS1998 sẵn có trong `Services/Implementations/FileTienDoCongViecService.cs`, không phát sinh từ phần hiển thị trạng thái phân công.

Kiểm tra font tiếng Việt:

- Các chuỗi mới được lưu đúng dấu: `Chưa phân công`, `Đã phân công`, `người`, `Phân công`.
- Không phát hiện chuỗi mojibake trong các file Razor/ViewModel đã sửa khi kiểm tra bằng `Select-String`.

Các vấn đề còn tồn tại:

- Chưa chạy kiểm thử giao diện bằng trình duyệt trong dữ liệu thật tại thời điểm ghi mục này.

## 16. Tách cột phân công và bổ sung số lượng chi tiết công việc

Ngày cập nhật: 19/06/2026.

Nguyên nhân thông tin phân công chỉ xuất hiện khi cửa sổ hẹp:

- Source trước khi sửa đặt badge phân công của bảng Công việc trong cột `Công việc`, ngay dưới tên công việc, còn mobile card có dòng riêng `Phân công`.
- Source trước khi sửa đặt badge phân công của bảng Chi tiết công việc trong cột `Tên CTCV`, ngay dưới tên chi tiết, còn mobile card có dòng riêng `Phân công`.
- Bảng desktop không có cột riêng `Phân công`, và bảng Công việc cũng chưa có cột riêng `Chi tiết công việc`.
- CSS bảng Công việc dùng `table-layout: fixed` với `width: 100%`, nhiều nội dung mô tả bị line-clamp hoặc bị bóp trong cột tên; vì vậy thông tin đặt dưới tên không phải vùng hiển thị rõ ở desktop. Khi về layout mobile `d-lg-none`, thông tin xuất hiện rõ vì được đặt trong metadata card.

File đã sửa:

- `QuanLyDuAn/QuanLyDuAn/ViewModels/CongViec/CongViecItemViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/CongViec/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/CongViec/index.css`
- `QuanLyDuAn/QuanLyDuAn/Views/ChiTietCongViec/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChiTietCongViec/index.css`
- `docs/phancong.md`

Cột mới trong bảng Công việc:

- Thêm cột desktop `Phân công` giữa `Trạng thái` và `Chi tiết công việc`.
- Thêm cột desktop `Chi tiết công việc` giữa `Phân công` và `Thao tác`.
- Thứ tự cột desktop hiện là: `Dự án`, `Công việc`, `Mức độ / Chi phí`, `Mốc thời gian`, `Trạng thái`, `Phân công`, `Chi tiết công việc`, `Thao tác`.
- Badge phân công không còn đặt dưới tên công việc trong bảng desktop.
- Mobile card vẫn hiển thị `Phân công` và được bổ sung `Chi tiết công việc`.

Cột mới trong bảng Chi tiết công việc:

- Thêm cột desktop `Phân công` giữa `Trạng thái` và `Thao tác`.
- Badge phân công không còn đặt dưới `Tên CTCV` trong bảng desktop.
- Mobile card vẫn hiển thị dòng `Phân công`.

Property số lượng chi tiết đã thêm:

- `CongViecItemViewModel.SoLuongChiTietCongViec`
- `CongViecItemViewModel.DaCoChiTietCongViec => SoLuongChiTietCongViec > 0`

Cách Service đếm chi tiết:

- `CongViecService.GetPageAsync` giữ nguyên tìm kiếm, bộ lọc, data scope, role, sắp xếp, phân trang, workflow và cờ thao tác.
- Sau khi lấy danh sách công việc của trang hiện tại, service lấy các `MaCongViec`, query `CtCongViec` theo cụm ID, lọc `IsDeleted != true`, group theo `MaCongViec`, `Count()`, rồi map dictionary vào `SoLuongChiTietCongViec`.

Điều kiện loại dữ liệu không hiệu lực:

- Số lượng chi tiết chỉ đếm `CtCongViec.IsDeleted != true`.
- Số người phân công vẫn lọc `NguoiDung.IsDeleted != true`.
- Không dùng bảng nhật ký phân công.

Cách tránh N+1:

- Không query chi tiết trong vòng lặp từng công việc.
- Không query phân công trong vòng lặp từng công việc hoặc từng chi tiết.
- Với bảng Công việc: sau query phân trang có một query group số người phân công và một query group số chi tiết.
- Với bảng Chi tiết công việc: giữ query group số người phân công chi tiết theo danh sách `MaChiTietCV` của trang hiện tại.

Các `colspan` đã cập nhật:

- Bảng Chi tiết công việc cập nhật dòng collapse/form sửa inline từ `colspan="7"` thành `colspan="8"`.
- Bảng Công việc không có dòng `colspan` trong `_Table.cshtml` vì empty state hiện là block ngoài table, không phải một hàng trong tbody.

CSS/min-width đã cập nhật:

- `wwwroot/css/CongViec/index.css`: bảng desktop `workflow-table` có `min-width: 1280px`, thêm width cho `.col-assignment` và `.col-detail-count`, wrapper tiếp tục `overflow-x: auto`.
- `wwwroot/css/ChiTietCongViec/index.css`: bảng desktop `chi-tiet-table` có `min-width: 1120px`, thêm width cho `.col-assignment`, wrapper `chi-tiet-table-wrapper` có `overflow-x: auto`.

Kết quả desktop, chia đôi màn hình, tablet và mobile:

- Kiểm tra source xác nhận desktop table có cột riêng `Phân công` và không nằm trong block mobile `d-lg-none`.
- Kiểm tra source xác nhận mobile card vẫn có dòng `Phân công`; mobile Công việc có thêm dòng `Chi tiết công việc`.
- Chưa thể kiểm tra bằng trình duyệt thật trong môi trường hiện tại. Lần chạy app trong sandbox bị lỗi kết nối SQL Server local `LAPTOP-SI5JBDIU\SQLEXPRESS01` và lỗi quyền ghi Windows EventLog; yêu cầu chạy ngoài sandbox để kiểm tra runtime đã không được cấp quyền.

Kết quả build:

- Đã chạy `dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore`.
- Build thành công, không có lỗi compile hoặc lỗi Razor compilation.
- Build còn 2 warning CS1998 sẵn có trong `Services/Implementations/FileTienDoCongViecService.cs`, không phát sinh từ phần thay đổi này.

Kết quả font tiếng Việt:

- Các chuỗi mới được lưu đúng dấu: `Phân công`, `Chưa phân công`, `Đã phân công`, `Chi tiết công việc`, `Chưa có chi tiết`, `người`, `chi tiết`.
- Đã kiểm tra UTF-8 byte-level cho toàn bộ file đã sửa trong mục này; tất cả decode UTF-8 nghiêm ngặt thành công và không phát hiện chuỗi mojibake.

Hạn chế còn lại:

- Chưa xác nhận bằng browser ở desktop toàn màn hình, sidebar mở/thu gọn, chia đôi màn hình, tablet và mobile do không thể chạy ứng dụng runtime trong môi trường hiện tại khi chưa có quyền ngoài sandbox.
