# Phân tích chức năng Danh mục công việc

Ngày phân tích: 17/06/2026. Tài liệu này chỉ mô tả source hiện tại và đề xuất nghiệp vụ TO-BE, chưa triển khai code.

## Trạng thái triển khai

Ngày triển khai: 17/06/2026.

### File đã sửa hoặc tạo mới

- Tạo mới `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDanhMucCongViecScopeService.cs`.
- Tạo mới `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhMucCongViecScopeService.cs`.
- Sửa `QuanLyDuAn/QuanLyDuAn/Services/CauHinhDichVu.cs` để đăng ký DI cho scope service.
- Sửa `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDanhMucCongViecService.cs`.
- Sửa `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhMucCongViecService.cs`.
- Sửa `QuanLyDuAn/QuanLyDuAn/Controllers/DanhMucCongViecController.cs`.
- Sửa `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhMucCongViec/DanhMucCongViecPageViewModel.cs`.
- Sửa `QuanLyDuAn/QuanLyDuAn/Views/DanhMucCongViec/Index.cshtml`.
- Sửa `QuanLyDuAn/QuanLyDuAn/Views/DanhMucCongViec/_Form.cshtml`.
- Sửa `QuanLyDuAn/QuanLyDuAn/Views/DanhMucCongViec/_Table.cshtml`.
- Sửa `QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Table.cshtml`.
- Sửa `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml`.
- Cập nhật `docs/danhmuccongviec.md`.

Không sửa `_Layout.cshtml` vì source hiện tại chỉ tính biến `canDanhMuc`, chưa render link/menu Danh mục công việc trong sidebar.

### Helper hoặc service scope

Đã tạo `DanhMucCongViecScopeService` với interface `IDanhMucCongViecScopeService`.

Service này cung cấp:

- `CanAccessProjectAsync(ClaimsPrincipal user, int maDuAn, string permission)`.
- `GetAccessibleProjectIdsAsync(ClaimsPrincipal user, string permission)`.

Rule kiểm tra:

- Trước hết kiểm tra permission bằng `IPhanQuyenService.GetGrantedPermissionNamesAsync`.
- Nếu user là `Admin` hoặc `Manager`, giữ nguyên nghiệp vụ hiện tại: có permission thì được phép, không bắt buộc là Leader.
- Nếu user là `Employee`, bắt buộc phải là leader của team đang phụ trách đúng dự án:
  - có dòng `NhanVienTeam` đúng `MaNguoiDung`;
  - `NhanVienTeam.IsLeader == true`;
  - team đó có dòng `TeamDuAn` đúng `MaDuAn`;
  - `Team.IsDeleted != true`;
  - `Team.TrangThaiTeam` thuộc trạng thái hoạt động theo `TrangThai.HoatDong` hoặc biến thể hiển thị.
- Không dùng `NhanVienDuAn.VaiTroTrongDuAn == Leader` để thay thế điều kiện `NhanVienTeam.IsLeader`.

### Rule Admin, Manager và Employee

- `Admin`: có permission `DanhMucCongViec.Xem/Them/Sua/Xoa` tương ứng thì được xem/thao tác như trước; không thêm scope dự án.
- `Manager`: có permission tương ứng thì được xem/thao tác như trước; không giới hạn theo dự án quản lý trong lần sửa này.
- `Employee Leader`: phải có permission tương ứng và phải là `NhanVienTeam.IsLeader == true` của team đang được gán vào đúng dự án qua `TeamDuAn`; team phải chưa xóa mềm và đang hoạt động.
- `Employee bình thường`: dù có permission thủ công vẫn bị chặn nếu không phải leader hợp lệ trong đúng phạm vi dự án.

### Cách chặn Employee bình thường và URL trực tiếp

- `DanhMucCongViecController.Index` kiểm tra `DanhMucCongViec.Xem` và `CanAccessProjectAsync` theo `locMaDuAn`; nếu không đạt thì `Forbid()`.
- `DanhMucCongViecController.Sua` kiểm tra `DanhMucCongViec.Sua`, đọc danh mục từ DB, lấy `MaDuAn` thật, chặn nếu `locMaDuAn` không khớp, rồi kiểm tra scope theo dự án thật.
- `DanhMucCongViecController.LuuDanhMucCongViec` kiểm tra permission trước khi lưu, kiểm tra dự án gốc khi sửa, kiểm tra dự án đích khi thêm hoặc đổi dự án, và trả `Forbid()` nếu không đạt scope.
- `DanhMucCongViecController.XoaDanhMucCongViec` gọi service xóa có truyền user; service đọc danh mục từ DB và kiểm tra scope theo `MaDuAn` thật trước khi xóa mềm.
- `DanhMucCongViecService` cũng tự kiểm tra scope ở các method có user: `GetPagedAsync`, `SaveAsync`, `DeleteAsync`.

### Cách chống sửa request thủ công

- Thay `locMaDuAn`: action `Sua` chặn nếu `locMaDuAn` khác `MaDuAn` thật của danh mục; `Index` chặn nếu user không có scope với `locMaDuAn`.
- Thay `Form.MaDuAn`: `SaveAsync` kiểm tra scope trên dự án đích; nếu sửa danh mục thì controller và service còn kiểm tra scope trên dự án gốc.
- Thay `MaDanhMucCV`: controller/service đọc danh mục thật theo ID từ database; xóa/sửa đều kiểm tra `MaDuAn` thật của danh mục trước khi thao tác.
- POST trực tiếp dù View không hiển thị nút: controller kiểm tra permission + scope, service kiểm tra lại scope và ném `UnauthorizedAccessException`; controller trả `Forbid()`.
- Dự án chưa có team phụ trách: Employee không có dòng `TeamDuAn` nối team mình làm leader với dự án, nên `CanAccessProjectAsync` trả `false`.

### Giao diện

- `Views/DuAn/_Table.cshtml`: nút `Danh mục công việc` chỉ hiển thị nếu `CanAccessProjectAsync(User, duAn.MaDuAn, DanhMucCongViec.Xem)` trả true.
- `Views/DuAn/Details.cshtml`: quick action `Danh mục công việc` dùng cùng rule theo `Model.MaDuAn`.
- `Views/DanhMucCongViec/Index.cshtml`, `_Form.cshtml`, `_Table.cshtml`: form/nút lưu/sửa/xóa dùng các cờ backend tính sẵn `CoTheThemDanhMuc`, `CoTheSuaDanhMuc`, `CoTheXoaDanhMuc`.
- Dropdown/danh sách dự án của module dùng `GetDuAnOptionsAsync(user, permission)`, với Employee chỉ trả các dự án thuộc team đang phụ trách hợp lệ.

### Kết quả build

Đã chạy:

```text
dotnet build QuanLyDuAn\QuanLyDuAn.sln --no-restore
```

Kết quả:

- Build succeeded.
- 0 error.
- 0 warning trong lượt build cuối cùng.

### Các trường hợp đã kiểm thử

Đã kiểm thử bằng build và đối chiếu tĩnh các đường controller/service/view sau khi sửa:

- Manager có `DanhMucCongViec.Xem/Them/Sua/Xoa`: scope service giữ rule permission-only cho Manager.
- Employee có permission và là leader team phụ trách dự án: `CanAccessProjectAsync` trả true khi có `NhanVienTeam.IsLeader == true`, `TeamDuAn` đúng dự án, team chưa xóa và đang hoạt động.
- Employee có permission nhưng không phải Leader: không có dòng `NhanVienTeam.IsLeader == true` đúng team dự án nên bị `Forbid()`.
- Employee là Leader team khác: không có `TeamDuAn` đúng `MaDuAn` nên bị `Forbid()`.
- Employee từng là Leader nhưng không còn `IsLeader`: query bắt buộc `IsLeader == true` nên bị `Forbid()`.
- Employee có `NhanVienDuAn.VaiTroTrongDuAn = Leader` nhưng không còn là `NhanVienTeam.IsLeader` của team đang phụ trách dự án: không được phép vì scope service không dùng `VaiTroTrongDuAn` thay thế.
- Employee thuộc dự án nhưng chỉ là thành viên: không có điều kiện leader team phụ trách nên bị `Forbid()`.
- Employee có permission nhưng dự án chưa có team phụ trách: không có dòng `TeamDuAn` nên bị `Forbid()`.
- Employee Leader đúng dự án nhưng thiếu permission tương ứng: `CanAccessProjectAsync` kiểm tra permission trước nên không được phép.
- Sửa request `locMaDuAn`, `Form.MaDuAn`, `MaDanhMucCV`: controller/service đều lấy dữ liệu thật từ DB và kiểm tra scope trên dự án thật trước khi ghi/xóa.

Chưa chạy kiểm thử UI thủ công bằng trình duyệt hoặc test tự động vì dự án chưa có bộ test tự động cho các ma trận tài khoản này trong source hiện tại.

### Xác nhận phạm vi thay đổi

- Không tạo migration.
- Không thay đổi database.
- Không đổi route.
- Không đổi action name.
- Không đổi tên permission.
- Không sửa seed permission.
- Không thêm `DanhMucCongViec.*` vào danh sách cấm của `Employee`.
- Không thay đổi workflow Công việc, Chi tiết công việc, Đề xuất công việc, Đề xuất ngân sách, Phân công công việc hoặc Tiến độ công việc.

### UTF-8 và lỗi font

Đã quét các file vừa sửa bằng `rg` với các dấu hiệu mojibake phổ biến:

```text
Ã|Â|Æ|á»|áº|â€
```

Kết quả: không phát hiện lỗi font trong các file vừa sửa. Các chuỗi tiếng Việt mới thêm hiển thị đúng dấu.

## 1. Phạm vi source đã đọc

Các file đã đọc trực tiếp:

- `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`
- `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DanhMucCongViec.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/Team.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/TeamDuAn.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/NhanVienTeam.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/NhanVienDuAn.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DuAn.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/CongViec.cs`
- `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`
- `QuanLyDuAn/QuanLyDuAn/Data/KhoiTaoTaiKhoanMacDinh.cs`
- `QuanLyDuAn/QuanLyDuAn/Controllers/DanhMucCongViecController.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDanhMucCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhMucCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhMucCongViec/DanhMucCongViecCreateUpdateViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhMucCongViec/DanhMucCongViecPageViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhMucCongViec/DanhMucCongViecViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhMucCongViec/DuAnOptionViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhMucCongViec/Index.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhMucCongViec/_Filter.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhMucCongViec/_Form.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhMucCongViec/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IPermissionHelper.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PermissionHelper.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IPhanQuyenService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanQuyenService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PermissionDependencyProvider.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DeXuatCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DeXuatNganSachService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DyetDeXuatCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuyetDeXuatNganSachService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanCongCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanCongChiTietCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TienDoCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TeamDuAnService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ThanhVienTeamService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanSuService.cs`
- `quanlyduan.sql`

Các từ khóa đã tìm trong source: `DanhMucCongViec`, `DanhMucCongViec.Xem`, `IsLeader`, `TEAM_DU_AN`, `NHAN_VIEN_TEAM`, `HasPermissionAsync`, `GetGrantedPermissionNamesAsync`, `User.IsInRole`, `Employee`, `Manager`, `Forbid`, `Unauthorized` và các route/link đến `DanhMucCongViec`.

## 2. Cấu trúc chức năng hiện tại

### Entity và quan hệ dữ liệu

- `DanhMucCongViec` ánh xạ bảng `DANH_MUC_CONG_VIEC`, gồm `MaDanhMucCV`, `MaDuAn`, `TenDanhMucCV`, `MoTaDanhMucCV`, `NgayTaoDMCV`, `IsDeleted`, `DeletedAt`, `DeletedBy`.
- `QuanLyDuAnDbContext` cấu hình `DanhMucCongViec` có khóa chính `MaDanhMucCV`, FK `MaDuAn` đến `DuAn`, constraint `FK_DANH_MUC_GOM_DU_AN`.
- `CongViec` có `MaDanhMucCV` và được dùng để kiểm tra danh mục đang có công việc trước khi xóa.
- `Team`, `TeamDuAn`, `NhanVienTeam`, `NhanVienDuAn` không được `DanhMucCongViecService` sử dụng hiện tại.

### Controller action

`DanhMucCongViecController` có `[Authorize]` ở cấp controller và các action:

- `GET Index(string? tuKhoa, int? locMaDuAn, int pageNumber = 1, int pageSize = 20)`
- `GET Sua(int id, string? tuKhoa, int? locMaDuAn, int pageNumber = 1, int pageSize = 20)`
- `POST LuuDanhMucCongViec(DanhMucCongViecPageViewModel vm)`
- `POST XoaDanhMucCongViec(int maDanhMucCV, string? tuKhoa, int? locMaDuAn)`

Route theo MVC mặc định, ví dụ:

- `/DanhMucCongViec/Index?locMaDuAn=...`
- `/DanhMucCongViec/Sua/{id}?locMaDuAn=...`
- `/DanhMucCongViec/LuuDanhMucCongViec`
- `/DanhMucCongViec/XoaDanhMucCongViec`

### Service method

`IDanhMucCongViecService`:

- `GetAllAsync(string? tuKhoa, int? maDuAn)`
- `GetPagedAsync(string? tuKhoa, int? maDuAn, int pageNumber = 1, int pageSize = 20)`
- `GetByIdAsync(int id)`
- `GetDuAnOptionsAsync()`
- `SaveAsync(DanhMucCongViecCreateUpdateViewModel model)`
- `DeleteAsync(int id)`

`DanhMucCongViecService` chỉ nhận DB context, không nhận `ClaimsPrincipal`, `IHttpContextAccessor`, `IPermissionHelper` hoặc helper scope.

### View và ViewModel

- `DanhMucCongViecPageViewModel` chứa danh sách, form, danh sách dự án, filter, pagination và `HashSet<string> Permissions`.
- `Index.cshtml` hiển thị form khi có `DanhMucCongViec.Them` hoặc `DanhMucCongViec.Sua`; luôn hiển thị filter và bảng nếu đã vào được trang.
- `_Form.cshtml` submit về `LuuDanhMucCongViec`, dùng hidden `MaDanhMucCV`, `MaDuAn`, `LocMaDuAn`, `TuKhoa`.
- `_Table.cshtml` ẩn/hiện nút `Sửa` theo `DanhMucCongViec.Sua`, nút `Xóa` theo `DanhMucCongViec.Xoa`.
- `_Filter.cshtml` chỉ lọc theo từ khóa, giữ `locMaDuAn` hidden.

### Permission đang sử dụng

Tên permission lấy từ `Constants/Permissions.cs`:

- `DanhMucCongViec.Xem`
- `DanhMucCongViec.Them`
- `DanhMucCongViec.Sua`
- `DanhMucCongViec.Xoa`

`PermissionDependencyProvider` khai báo `Them`, `Sua`, `Xoa` phụ thuộc `Xem`. `KhoiTaoTaiKhoanMacDinh` cấp mặc định đủ 4 quyền Danh mục công việc cho role `Manager`, không cấp mặc định cho role `Employee`. Danh sách quyền bị cấm của `Employee` trong `PermissionDependencyProvider` không cấm các quyền `DanhMucCongViec.*`, nên source hiện tại không ngăn tuyệt đối việc cấp thủ công các quyền này cho `Employee`.

### Menu hoặc điểm truy cập giao diện

- `_Layout.cshtml` có biến `canDanhMuc = grantedPermissions.Contains(Permissions.DanhMucCongViec.Xem)` nhưng không render link sidebar `DanhMucCongViec`.
- `Views/DuAn/_Table.cshtml` hiển thị nút `Danh mục công việc` trên từng dòng dự án nếu user có `DanhMucCongViec.Xem`.
- `Views/DuAn/Details.cshtml` hiển thị quick action `Danh mục công việc` nếu user có `DanhMucCongViec.Xem`.
- Các nút này không kiểm tra role `Employee`, không kiểm tra Leader, không kiểm tra dự án có thuộc phạm vi user hiện tại hay không.

## 3. Cơ chế xác định Leader đang tồn tại trong source

Không có helper chung dành riêng cho "Leader đúng phạm vi dự án". Các module khác đang tự kiểm tra trong private method.

Cơ chế chính đang xuất hiện:

- `NhanVienTeam.IsLeader == true`: xác định trưởng nhóm trong bảng `NHAN_VIEN_TEAM`.
- `TeamDuAn`: nối team với dự án. Khi kiểm tra leader theo team phụ trách, source thường lấy `TeamDuAn.MaTeam` theo `MaDuAn`, rồi kiểm tra user có dòng `NhanVienTeam` của team đó với `IsLeader == true`.
- `NhanVienDuAn.VaiTroTrongDuAn`: một số luồng còn xem `Leader` hoặc `Trưởng nhóm` là vai trò trong dự án.

Các cách dùng cụ thể:

- `DeXuatCongViecService` và `DeXuatNganSachService`: nếu dự án có team phụ trách trong `TeamDuAn`, chỉ user là `NhanVienTeam.IsLeader == true` của team thuộc dự án đó được đề xuất. Nếu dự án chưa có team phụ trách, source cho nhân viên thuộc `NhanVienDuAn` được đề xuất. Không join `Team.IsDeleted`, không kiểm tra `Team.TrangThaiTeam`, không kiểm tra tài khoản bị khóa trong chính query leader.
- `PhanCongCongViecService`: Manager được phép nếu là `DuAn.MaNguoiDung`; ngoài ra user được phép nếu là leader của team thuộc `TeamDuAn` của dự án, hoặc có `NhanVienDuAn.VaiTroTrongDuAn == Leader`.
- `PhanCongChiTietCongViecService`: nếu `NhanVienDuAn.VaiTroTrongDuAn` là Member thì chặn; nếu là Leader thì cho; nếu không, tiếp tục kiểm tra `NhanVienTeam.IsLeader == true` trong team thuộc `TeamDuAn`.
- `TienDoCongViecService`: tập dự án được duyệt hoặc xem thêm gồm dự án user là `NhanVienDuAn` Leader và dự án user là leader của team tham gia `TeamDuAn`.
- `TeamDuAnService`: khi gán team vào dự án, bắt buộc team có leader; leader của team bắt buộc được đưa vào `NhanVienDuAn` với `VaiTroTrongDuAn = Leader`.
- `ThanhVienTeamService`: chỉ nhân sự role `Employee` mới được thêm vào team; team đang phụ trách dự án thì không cho đổi leader hoặc xóa leader khỏi team.

Kết luận về Leader từ source hiện tại:

- Leader của team bất kỳ không đủ trong các module có scope; phải là leader của team có dòng `TeamDuAn` đúng dự án, hoặc trong vài luồng phải có `NhanVienDuAn.VaiTroTrongDuAn == Leader`.
- Một dự án có nhiều team thì leader của bất kỳ team nào đang có `TeamDuAn` với dự án đó đều được coi là leader phạm vi trong các query kiểu `TeamDuAn` + `NhanVienTeam`.
- Employee là leader của team nhưng team không phụ trách dự án thì không đạt điều kiện ở các module dùng `TeamDuAn`.
- Nếu user bị gỡ khỏi `NhanVienTeam`, bị gỡ `IsLeader`, hoặc team bị gỡ khỏi `TeamDuAn`, các query leader kiểu này không còn trả true. Nếu chỉ còn `NhanVienDuAn.VaiTroTrongDuAn == Leader`, một số module như phân công/tiến độ vẫn có thể cho phép theo vai trò dự án.
- Các bảng `TeamDuAn`, `NhanVienTeam`, `NhanVienDuAn` không có cột `IsDeleted`; các query leader hiện hữu không lọc soft-delete trên chính các bảng này. Một số query có lọc `Team.IsDeleted`, một số query scope không join `Team`.
- Khi chưa có team phụ trách dự án, `DeXuatCongViecService` và `DeXuatNganSachService` cho nhân viên thuộc dự án thao tác đề xuất. Quy tắc này chưa được áp dụng cho Danh mục công việc.

## 4. Luồng nghiệp vụ AS-IS

### Xem danh sách danh mục công việc

`Người dùng -> DanhMucCongViecController.Index -> kiểm tra permission DanhMucCongViec.Xem -> yêu cầu có locMaDuAn -> service GetDuAnOptionsAsync lấy toàn bộ dự án chưa xóa -> kiểm tra locMaDuAn có trong danh sách toàn hệ thống -> service GetPagedAsync lọc danh mục theo MaDuAn, IsDeleted != true -> View Index`

Hiện không kiểm tra role, không kiểm tra Manager có quản lý dự án hay không, không kiểm tra Employee có là Leader hay không.

### Xem theo dự án

Điểm vào từ `DuAn/_Table.cshtml` và `DuAn/Details.cshtml` truyền `locMaDuAn`. View chỉ kiểm tra `DanhMucCongViec.Xem`. Controller sau đó kiểm tra `locMaDuAn` tồn tại trong `GetDuAnOptionsAsync`, mà danh sách này là tất cả dự án `IsDeleted != true`.

`Người dùng -> nút trong Dự án hoặc URL trực tiếp -> Controller Index -> permission Xem -> service GetDuAnOptionsAsync toàn bộ dự án -> service GetPagedAsync theo locMaDuAn -> View`

### Thêm danh mục

`Người dùng -> View hiển thị form nếu có Them hoặc Sua -> POST LuuDanhMucCongViec -> lấy selectedMaDuAn từ LocMaDuAn hoặc Form.MaDuAn -> service GetDuAnOptionsAsync toàn bộ dự án để kiểm tra tồn tại -> ModelState -> kiểm tra permission DanhMucCongViec.Them khi MaDanhMucCV == null -> service SaveAsync -> kiểm tra dự án tồn tại IsDeleted != true -> kiểm tra trùng tên trong dự án -> insert DanhMucCongViec IsDeleted = false -> SaveChanges -> Redirect`

Không kiểm tra trạng thái dự án. Không kiểm tra phạm vi dự án. Không kiểm tra Leader.

### Sửa danh mục

GET:

`Người dùng -> Controller Sua -> kiểm tra permission DanhMucCongViec.Sua -> yêu cầu locMaDuAn -> service GetByIdAsync(id) chỉ theo MaDanhMucCV và IsDeleted != true -> service GetDuAnOptionsAsync toàn bộ dự án -> selectedProject theo locMaDuAn -> gán form.MaDuAn = selectedProject.MaDuAn -> service GetPagedAsync theo locMaDuAn -> View Index`

POST:

`Người dùng -> POST LuuDanhMucCongViec -> selectedMaDuAn từ LocMaDuAn hoặc Form.MaDuAn -> kiểm tra dự án tồn tại toàn hệ thống -> ModelState -> kiểm tra permission Sua nếu MaDanhMucCV != null -> service SaveAsync -> load entity theo MaDanhMucCV và IsDeleted != true -> nếu đổi MaDuAn và đã có CongViec thì chặn -> cập nhật MaDuAn, Ten, MoTa -> SaveChanges`

Rủi ro quan trọng: GET `Sua` không xác minh danh mục `id` thuộc `locMaDuAn`; POST cho phép đổi `MaDuAn` của danh mục nếu danh mục chưa có công việc.

### Xóa hoặc xóa mềm danh mục

`Người dùng -> POST XoaDanhMucCongViec -> kiểm tra permission DanhMucCongViec.Xoa -> service DeleteAsync(id) -> kiểm tra có CongViec IsDeleted != true dùng danh mục hay không -> load danh mục theo id và IsDeleted != true -> set IsDeleted = true, DeletedAt = UtcNow -> SaveChanges -> Redirect`

`locMaDuAn` chỉ dùng để redirect, không dùng để xác thực danh mục thuộc dự án đang xem.

### Kiểm tra dữ liệu liên quan trước khi xóa

`DeleteAsync` chỉ kiểm tra `CongViec.Any(x => x.MaDanhMucCV == id && x.IsDeleted != true)`. Nếu có công việc chưa xóa mềm, không cho xóa. Không kiểm tra đề xuất công việc, chi phí hoặc dữ liệu khác trực tiếp.

### Kiểm tra trạng thái dự án

Không có kiểm tra `TrangThaiDuAn` trong `DanhMucCongViecController` hoặc `DanhMucCongViecService`. Dự án hoàn thành, tạm dừng, lưu trữ hoặc đã hủy nếu vẫn `IsDeleted != true` thì vẫn có thể xuất hiện trong `GetDuAnOptionsAsync` và được thêm/sửa/xóa danh mục nếu user có permission.

### Kiểm tra permission

- `Index`: `DanhMucCongViec.Xem`
- `Sua` GET: `DanhMucCongViec.Sua`
- `LuuDanhMucCongViec` POST thêm: `DanhMucCongViec.Them`
- `LuuDanhMucCongViec` POST sửa: `DanhMucCongViec.Sua`
- `XoaDanhMucCongViec` POST: `DanhMucCongViec.Xoa`
- View: dùng `Permissions` để ẩn/hiện form, nút sửa, nút xóa.

Permission được lấy từ claims hiện tại bằng `PhanQuyenService.GetGrantedPermissionNamesAsync`; `PermissionHelper` chỉ kiểm tra có ít nhất một permission trong danh sách truyền vào.

### Kiểm tra phạm vi dự án và Leader

Trong chính chức năng Danh mục công việc: chưa có.

Không có kiểm tra:

- role `Employee`
- role `Manager`
- role `Admin`
- user hiện tại có thuộc dự án hay không
- user hiện tại có là `NhanVienTeam.IsLeader` hay không
- team của user có dòng `TeamDuAn` với dự án hay không
- `NhanVienDuAn.VaiTroTrongDuAn`
- `Team.IsDeleted` hoặc `Team.TrangThaiTeam`

## 5. Ma trận quyền AS-IS

Bảng này phản ánh source hiện tại của chức năng Danh mục công việc. "Có" nghĩa là khi tài khoản thực sự có permission tương ứng trong claim tại thời điểm request.

| Người dùng | Có permission | Là Leader đúng phạm vi | Xem menu | Xem danh sách | Thêm | Sửa | Xóa | Truy cập URL trực tiếp |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Admin | Có `DanhMucCongViec.Xem/Them/Sua/Xoa` nếu được cấp | Không kiểm tra | Không có link sidebar; có nút từ Dự án/Chi tiết nếu có `Xem` | Có, nếu có `Xem` | Có, nếu có `Them` | Có, nếu có `Sua` | Có, nếu có `Xoa` | Có, nếu có permission tương ứng |
| Admin mặc định từ `KhoiTaoTaiKhoanMacDinh` | Không thấy cấp `DanhMucCongViec.*` trong seed đọc được | Không kiểm tra | Không | Không | Không | Không | Không | Bị `Forbid()` do thiếu permission |
| Manager | Mặc định có đủ 4 quyền trong seed | Không kiểm tra | Không có link sidebar; có nút từ Dự án/Chi tiết nếu có `Xem` | Có với mọi dự án chưa xóa | Có với mọi dự án chưa xóa | Có với mọi danh mục chưa xóa | Có với mọi danh mục chưa xóa nếu chưa có công việc | Có |
| Employee Leader | Nếu được cấp permission | Không kiểm tra trong module | Không có link sidebar; có nút từ Dự án/Chi tiết nếu có `Xem` | Có với mọi dự án chưa xóa, không chỉ dự án leader | Có với mọi dự án chưa xóa nếu có `Them` | Có với mọi danh mục chưa xóa nếu có `Sua` | Có với mọi danh mục chưa xóa nếu có `Xoa` và chưa có công việc | Có |
| Employee bình thường | Mặc định không có `DanhMucCongViec.*`; nếu được cấp thủ công thì có | Không kiểm tra trong module | Nếu được cấp `Xem`, có nút từ Dự án/Chi tiết | Nếu được cấp `Xem`, có với mọi dự án chưa xóa | Nếu được cấp `Them`, có | Nếu được cấp `Sua`, có | Nếu được cấp `Xoa`, có nếu chưa có công việc | Nếu được cấp permission tương ứng thì có |

Kết luận AS-IS ngắn: permission là lớp chặn duy nhất của module. Leader và phạm vi dự án không tham gia quyết định.

## 6. Các lỗ hổng hoặc điểm chưa thống nhất

| Mức độ | File | Class/method | Hành vi hiện tại | Rủi ro |
| --- | --- | --- | --- | --- |
| Cao | `Controllers/DanhMucCongViecController.cs` | `Index` | Chỉ kiểm tra `DanhMucCongViec.Xem`, sau đó cho xem danh mục của bất kỳ `locMaDuAn` tồn tại và chưa xóa. | Employee bình thường hoặc Manager không thuộc phạm vi dự án, nếu có permission, vẫn xem được danh mục bằng URL trực tiếp. |
| Cao | `Services/Implementations/DanhMucCongViecService.cs` | `GetDuAnOptionsAsync` | Lấy toàn bộ dự án `IsDeleted != true`, không lọc theo Manager, Employee Leader hoặc thành viên dự án. | Dropdown/điểm chọn dự án không phản ánh phạm vi nghiệp vụ; user có permission có thể chọn dự án ngoài phạm vi. |
| Cao | `Controllers/DanhMucCongViecController.cs`, `Services/Implementations/DanhMucCongViecService.cs` | `LuuDanhMucCongViec`, `SaveAsync` | POST thêm/sửa chỉ dựa vào permission và `MaDuAn` gửi lên hoặc `LocMaDuAn`; service chỉ kiểm tra dự án tồn tại. | Có permission là có thể thêm/sửa danh mục ở dự án ngoài phạm vi. |
| Cao | `Services/Implementations/DanhMucCongViecService.cs` | `SaveAsync` | Khi sửa, nếu `entity.MaDuAn != maDuAn` và danh mục chưa có công việc, service cho đổi dự án. | User có thể sửa request để chuyển danh mục sang dự án khác nếu có permission, không có scope guard từ DB gốc. |
| Cao | `Controllers/DanhMucCongViecController.cs`, `Services/Implementations/DanhMucCongViecService.cs` | `XoaDanhMucCongViec`, `DeleteAsync` | Xóa chỉ nhận `maDanhMucCV`, không kiểm tra `locMaDuAn` và không xác minh phạm vi dự án của danh mục từ DB. | User có permission xóa có thể xóa mềm danh mục ngoài phạm vi bằng id, miễn chưa có công việc liên kết. |
| Cao | `Controllers/DanhMucCongViecController.cs` | `Sua` GET | Lấy form theo `id`, sau đó gán `form.MaDuAn = selectedProject.MaDuAn` từ `locMaDuAn`; không kiểm tra danh mục thuộc dự án đang xem. | Có thể mở form sửa danh mục của dự án A trong context dự án B; POST có thể dẫn đến đổi dự án nếu danh mục chưa có công việc. |
| Cao | `Views/DuAn/_Table.cshtml`, `Views/DuAn/Details.cshtml` | Link `DanhMucCongViec` | Nút hiển thị chỉ dựa trên `DanhMucCongViec.Xem`. | Employee bình thường nếu được cấp `Xem` sẽ thấy điểm vào chức năng, bất kể có phải Leader đúng phạm vi không. |
| Trung bình | `Views/Shared/_Layout.cshtml` | Sidebar | Tính `canDanhMuc` nhưng không render link Danh mục công việc trong sidebar. | Giao diện không thống nhất: có permission nhưng không có menu sidebar; người dùng phải đi từ Dự án/Chi tiết hoặc URL. |
| Cao | `Services/Implementations/DanhMucCongViecService.cs` | Toàn service | Service không nhận user hiện tại và không tự bảo vệ permission/scope. | Nếu service được gọi từ controller khác hoặc future code quên guard, nghiệp vụ không được bảo vệ ở lớp service. |
| Trung bình | `Controllers/DanhMucCongViecController.cs` | `LuuDanhMucCongViec` | Khi `ModelState` không hợp lệ, controller đã gọi `GetDuAnOptionsAsync` và render lại dữ liệu trước khi kiểm tra `Them` hoặc `Sua`. | User thiếu quyền thêm/sửa nhưng có quyền xem có thể gửi POST lỗi để đọc lại danh sách dự án toàn hệ thống; chưa ghi DB nhưng thứ tự guard không chặt. |
| Cao | `Services/Implementations/DanhMucCongViecService.cs` | `SaveAsync`, `DeleteAsync` | Không kiểm tra `DuAn.TrangThaiDuAn`. | Danh mục vẫn có thể thay đổi khi dự án ở trạng thái đã hoàn thành, tạm dừng, lưu trữ hoặc đã hủy nếu dự án chưa xóa. |
| Trung bình | `PermissionDependencyProvider.cs`, `KhoiTaoTaiKhoanMacDinh.cs` | Role constraints/seed | Employee mặc định không có `DanhMucCongViec.*`, nhưng denied list của Employee không cấm các quyền này. | Có thể cấp quyền Danh mục công việc cho Employee qua cấu hình phân quyền; module sẽ cho thao tác vì không kiểm tra Leader. |
| Trung bình | Các service tham khảo scope | `DeXuatCongViecService`, `DeXuatNganSachService`, `PhanCong*`, `TienDoCongViecService` | Logic Leader đang nằm trong private method riêng từng service, không có helper chung. | Nếu triển khai Danh mục công việc bằng cách copy logic, dễ lệch điều kiện giữa các module. |
| Trung bình | Các query Leader tham khảo | `DeXuatCongViecService`, `DeXuatNganSachService`, `PhanCong*`, `TienDoCongViecService` | Một số query leader không join `Team` để lọc `Team.IsDeleted` hoặc `TrangThaiTeam`; các bảng `TeamDuAn`, `NhanVienTeam`, `NhanVienDuAn` không có soft-delete. | Leader của team đã bị xóa mềm ở bảng `Team` có thể vẫn được tính ở các query không join `Team`, nếu dữ liệu liên kết còn tồn tại. |
| Thấp | `ViewModels/DanhMucCongViec/DanhMucCongViecCreateUpdateViewModel.cs` | Data annotations | Nội dung validation trong terminal hiển thị mojibake khi đọc bằng PowerShell hiện tại. | Cần kiểm tra encoding trước khi sửa file sau này để tránh làm hỏng tiếng Việt; tài liệu này chưa sửa code. |

## 7. Nghiệp vụ TO-BE đề xuất

Chưa sửa code. Quy tắc đề xuất để triển khai sau:

1. Người dùng phải có permission tương ứng: `DanhMucCongViec.Xem`, `DanhMucCongViec.Them`, `DanhMucCongViec.Sua`, `DanhMucCongViec.Xoa`.
2. Nếu người dùng có role `Employee`, phải kiểm tra thêm phạm vi dự án.
3. Với dự án có team phụ trách, Employee chỉ được phép nếu là `NhanVienTeam.IsLeader == true` của team có dòng `TeamDuAn` đúng `MaDuAn`.
4. Nếu chọn phương án đồng bộ với các module đề xuất hiện tại, cần quyết định rõ có cho Employee thuộc `NhanVienDuAn` thao tác khi dự án chưa có team phụ trách hay không. Nghiệp vụ mới trong yêu cầu đang nghiêng về "chỉ Leader hợp lệ", nên nên từ chối Employee bình thường khi chưa có team phụ trách.
5. Employee bình thường không thấy nút từ Dự án/Chi tiết, không thấy danh sách dự án, không thấy form/nút thêm/sửa/xóa.
6. Employee bình thường phải bị backend từ chối khi gọi URL trực tiếp, kể cả khi đã được cấp permission Danh mục công việc.
7. GET và POST phải dùng cùng rule: `Index`, `Sua`, `LuuDanhMucCongViec`, `XoaDanhMucCongViec`.
8. Danh sách dự án hiển thị hoặc chấp nhận trong `locMaDuAn` phải được lọc theo phạm vi user.
9. Khi sửa hoặc xóa phải lấy `MaDuAn` thật từ danh mục trong database rồi kiểm tra scope trên `MaDuAn` đó; không tin `MaDuAn` hoặc `locMaDuAn` từ form.
10. Không chỉ ẩn giao diện; controller và service vẫn phải bảo vệ nghiệp vụ.
11. Không đổi quyền, route, database hoặc workflow của module khác nếu không bắt buộc.

Đối với `Admin` và `Manager`: source hiện tại của Danh mục công việc không áp dụng scope. Nếu nghiệp vụ không yêu cầu đổi, TO-BE nên giữ điều kiện permission là chính cho Admin/Manager. Nếu muốn giới hạn Manager theo dự án quản lý, cần xác nhận riêng vì đây là thay đổi nghiệp vụ so với module hiện tại.

## 8. Ma trận quyền TO-BE

| Đối tượng | Permission | Điều kiện phạm vi | Kết quả |
| --- | --- | --- | --- |
| Admin | Có permission tương ứng | Theo nghiệp vụ hiện tại của Admin; module hiện chưa kiểm tra scope Admin | Được phép |
| Manager | Có permission tương ứng | Theo nghiệp vụ hiện tại của Manager; module hiện chưa yêu cầu thuộc dự án quản lý | Được phép |
| Employee Leader | Có permission tương ứng | Là `NhanVienTeam.IsLeader == true` của team có `TeamDuAn.MaDuAn` đúng dự án; nếu sau này chọn hỗ trợ `NhanVienDuAn.VaiTroTrongDuAn == Leader` thì phải ghi rõ và dùng thống nhất | Được phép |
| Employee bình thường | Dù có permission | Không phải Leader đúng phạm vi dự án | Không hiển thị và từ chối truy cập |
| Employee Leader | Không có permission | Dù đúng phạm vi | Không được phép |

Nếu triển khai muốn bám đúng nhất theo các module đề xuất công việc/ngân sách hiện tại, cần bổ sung quyết định cho trường hợp "dự án chưa có team phụ trách": source đề xuất hiện tại cho nhân viên thuộc dự án đề xuất, còn nghiệp vụ mới yêu cầu Employee bình thường không được thao tác Danh mục công việc.

## 9. Vị trí dự kiến cần sửa ở bước triển khai sau

Chỉ liệt kê, chưa chỉnh sửa.

### Controller/action

- `QuanLyDuAn/QuanLyDuAn/Controllers/DanhMucCongViecController.cs`
  - `Index`: kiểm tra permission + scope theo `locMaDuAn`.
  - `Sua`: kiểm tra permission + scope theo `MaDuAn` thật của danh mục trong DB, đồng thời chặn mismatch `id` và `locMaDuAn`.
  - `LuuDanhMucCongViec`: kiểm tra permission trước khi load dữ liệu phụ; khi thêm kiểm tra scope theo `MaDuAn` được chọn; khi sửa kiểm tra scope theo danh mục hiện hữu từ DB và kiểm soát đổi dự án.
  - `XoaDanhMucCongViec`: kiểm tra scope theo danh mục trong DB trước khi xóa.

### Service/interface

- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDanhMucCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhMucCongViecService.cs`
  - Cần thêm đường lấy danh mục kèm `MaDuAn` để controller kiểm tra scope.
  - Cần lọc `GetDuAnOptionsAsync` theo user/scope hoặc thêm method mới không phá vỡ call cũ.
  - Nên đưa kiểm tra scope quan trọng vào service hoặc helper dùng chung để tránh controller là lớp bảo vệ duy nhất.

### View/ViewModel

- `Views/DanhMucCongViec/Index.cshtml`
- `Views/DanhMucCongViec/_Form.cshtml`
- `Views/DanhMucCongViec/_Table.cshtml`
- `Views/DanhMucCongViec/_Filter.cshtml`
- `ViewModels/DanhMucCongViec/DanhMucCongViecPageViewModel.cs`
- `ViewModels/DanhMucCongViec/DuAnOptionViewModel.cs`

Cần thêm tín hiệu phạm vi như `CoTheQuanLyDanhMuc` hoặc lọc danh sách dự án trước khi vào view. Không nên chỉ dựa vào `Permissions`.

### Menu/partial

- `Views/DuAn/_Table.cshtml`: nút `Danh mục công việc` phải xét permission + scope dự án.
- `Views/DuAn/Details.cshtml`: quick action `Danh mục công việc` phải xét permission + scope dự án.
- `Views/Shared/_Layout.cshtml`: hiện chưa có link sidebar Danh mục công việc; nếu sau này thêm link thì phải xét scope, vì Danh mục công việc cần ngữ cảnh dự án.

### Helper kiểm tra scope có thể tái sử dụng

Chưa thấy helper chung sẵn có. Có thể tham khảo private logic trong:

- `DeXuatCongViecService.EnsureCanProposeForProjectAsync`
- `DeXuatNganSachService.EnsureCanProposeForProjectAsync`
- `PhanCongCongViecService.KiemTraQuyenPhanCongAsync`
- `PhanCongChiTietCongViecService.KiemTraQuyenPhanCongAsync`
- `TienDoCongViecService.GetProjectIdsForReviewAsync`

Nên tạo helper/service nhỏ dùng chung, ví dụ kiểm tra `CanManageDanhMucCongViecAsync(user, maDuAn, permission)`, thay vì copy private query vào nhiều action. Helper cần xác định rõ:

- role hiện tại từ claims/Identity.
- `MaNguoiDung` hiện tại.
- Admin/Manager giữ rule hiện tại hay cần scope.
- Employee phải là leader của team phụ trách dự án.
- Có join `Team` để loại `Team.IsDeleted == true` và kiểm tra `TrangThaiTeam` hay không.
- Có chấp nhận `NhanVienDuAn.VaiTroTrongDuAn == Leader` như một nguồn Leader hợp lệ hay không.

### Permission seed

Không cần đổi tên permission. Có thể không cần đổi seed nếu chỉ thực thi rule scope ở backend. Nếu muốn ngăn cấp quyền Danh mục công việc cho Employee bình thường từ UI phân quyền, cần cân nhắc thêm `DanhMucCongViec.*` vào denied list của Employee, nhưng điều này là thay đổi chính sách phân quyền rộng hơn và không bắt buộc nếu backend đã chặn scope.

### Database

Không thấy cần migration hoặc thay đổi database cho nghiệp vụ này. Các bảng hiện có đủ để kiểm tra:

- `NHAN_VIEN_TEAM.IsLeader`
- `TEAM_DU_AN`
- `NHAN_VIEN_DU_AN.VaiTroTrongDuAn`
- `TEAM.IsDeleted`, `TEAM.TrangThaiTeam`
- `DU_AN.IsDeleted`, `DU_AN.TrangThaiDuAn`

## 10. Kết luận

- Hiện Employee bình thường mặc định không có quyền Danh mục công việc, nên mặc định không thấy và không truy cập được. Tuy nhiên nếu Employee bình thường được cấp `DanhMucCongViec.Xem/Them/Sua/Xoa`, source hiện tại sẽ cho xem và thao tác bằng giao diện hoặc URL trực tiếp vì module chỉ kiểm tra permission.
- Source hiện tại của riêng chức năng Danh mục công việc chưa phân biệt Employee Leader và Employee bình thường.
- Các lớp bảo vệ hiện tại chưa đầy đủ cho nghiệp vụ hai tầng permission + phạm vi Leader. Controller có permission guard, view có ẩn/hiện theo permission, nhưng service không có scope guard và không có kiểm tra Leader.
- Cần sửa các nhóm file: `DanhMucCongViecController`, `IDanhMucCongViecService`, `DanhMucCongViecService`, view/viewmodel Danh mục công việc, link từ `DuAn/_Table.cshtml`, `DuAn/Details.cshtml`, và nên thêm helper/service kiểm tra scope dùng chung.
- Không cần migration hoặc thay đổi database nếu dùng các bảng hiện có.
- Rủi ro triển khai mức trung bình đến cao: thay đổi nhỏ về code nhưng ảnh hưởng trực tiếp bảo mật dữ liệu theo dự án. Cần test cả GET/POST, URL trực tiếp, sửa request `MaDuAn`/`MaDanhMucCV`, Employee có permission nhưng không phải Leader, Employee Leader đúng dự án, Manager/Admin với permission, và dự án có/không có team phụ trách.
