# PHÂN TÍCH CHỨC NĂNG QUẢN LÝ NHÂN SỰ HIỆN TẠI

## 1. Phạm vi source đã đọc

### Controller
- `QuanLyDuAn/QuanLyDuAn/Controllers/NhanSuController.cs`
- `QuanLyDuAn/QuanLyDuAn/Controllers/AccountController.cs`
- `QuanLyDuAn/QuanLyDuAn/Controllers/TaiKhoanCaNhanController.cs`
- `QuanLyDuAn/QuanLyDuAn/Controllers/ChucDanhController.cs`
- `QuanLyDuAn/QuanLyDuAn/Controllers/PhanQuyenController.cs`
- `QuanLyDuAn/QuanLyDuAn/Controllers/ThanhVienTeamController.cs`
- `QuanLyDuAn/QuanLyDuAn/Controllers/NhanVienDuAnController.cs`

### Service và interface
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/INhanSuService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanSuService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IAccountService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AccountService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/ITaiKhoanCaNhanService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TaiKhoanCaNhanService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IEmailService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/GmailEmailService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IPermissionHelper.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PermissionHelper.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IPhanQuyenService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanQuyenService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PermissionDependencyProvider.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChucDanhService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/CauHinhDichVu.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ThanhVienTeamService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanVienDuAnService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanCongCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChatDuAnService.cs`

### ViewModel/DTO
- `QuanLyDuAn/QuanLyDuAn/ViewModels/NhanSu/NhanSuCreateUpdateViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/NhanSu/NhanSuViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/NhanSu/NhanSuPageViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/NhanSu/ChucDanhOptionViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/NhanSu/VaiTroHeThongOptionViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Auth/DangNhapViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Account/ForgotPasswordViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Account/VerifyOtpViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Account/ResetPasswordViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/TaiKhoanCaNhan/DoiMatKhauViewModel.cs`

### Entity/DbContext
- `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/NguoiDung.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/Aspnetusers.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/Aspnetroles.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/Aspnetuserroles.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/Aspnetroleclaims.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/Aspnetusertokens.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/ChucDanh.cs`
- Các entity tham chiếu nhân sự qua FK/khóa nghiệp vụ trong `QuanLyDuAnDbContext` như: `DuAn`, `NhanVienTeam`, `NhanVienDuAn`, `PhanCongCongViec`, `PhanCongCtCongViec`, `DanhGiaNhanVien`, `DanhGiaDuAn`, `YeuCauDoiQuanLy`, `TinNhan`, `ThanhVienPhongChat`, `NhatKy*`.

### Razor View
- `QuanLyDuAn/QuanLyDuAn/Views/NhanSu/Index.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/NhanSu/_Form.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/NhanSu/_Filter.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/NhanSu/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/Account/Login.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/Account/ForgotPassword.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/Account/VerifyOtp.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/Account/ResetPassword.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/Shared/_ValidationScriptsPartial.cshtml`

### CSS/JavaScript
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/NhanSu/index.css`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/Account/login.css` (được dùng ở luồng login/forgot/reset)
- `QuanLyDuAn/QuanLyDuAn/wwwroot/lib/jquery-validation/dist/jquery.validate.min.js`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js`

### Constants/Permission
- `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`
- `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs`

### Authentication
- `QuanLyDuAn/QuanLyDuAn/Program.cs`
- `QuanLyDuAn/QuanLyDuAn/Data/KhoiTaoTaiKhoanMacDinh.cs`

### Email/configuration
- `QuanLyDuAn/QuanLyDuAn/appsettings.json`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/GmailEmailService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IEmailService.cs`

### SQL/migration nếu có
- `QuanLyDuAn/QuanLyDuAn/Migrations/20260527125053_Init.cs`
- `QuanLyDuAn/QuanLyDuAn/Migrations/20260527125053_Init.Designer.cs`
- `QuanLyDuAn/QuanLyDuAn/Migrations/QuanLyDuAnDbContextModelSnapshot.cs`

## 2. Kiến trúc chức năng Nhân sự hiện tại

Luồng hiện tại bám theo mô hình MVC + service:

`Razor View -> Controller -> Service -> EF Core (QuanLyDuAnDbContext) -> SQL Server`

- `NhanSuController` xử lý request màn hình nhân sự, gọi `INhanSuService`.
- `NhanSuService` xử lý nghiệp vụ tạo/sửa/xóa mềm nhân sự, tạo tài khoản, gán role, khóa/mở khóa.
- `AccountController` + `AccountService` xử lý đăng nhập và quên mật khẩu OTP.
- `Program.cs` cấu hình cookie authentication, authorization toàn cục.
- Kiểm tra quyền thao tác chủ yếu ở Controller qua `IPermissionHelper.HasPermissionAsync`.

## 3. Các bảng và entity liên quan

| Entity/Bảng | Vai trò | Trường quan trọng | Quan hệ |
| ----------- | ------- | ----------------- | ------- |
| `NguoiDung` / `NGUOI_DUNG` | Hồ sơ nhân sự nghiệp vụ | `MaNguoiDung`, `MaChucDanh`, `Id`, `HoTenNguoiDung`, `SdtNguoiDung`, `NgaySinh`, `IsDeleted`, `DeletedAt`, `DeletedBy` | FK `MaChucDanh -> CHUC_DANH`; FK `Id -> AspNetUsers.Id`; được nhiều bảng nghiệp vụ tham chiếu bằng `MaNguoiDung` |
| `Aspnetusers` / `AspNetUsers` | Tài khoản đăng nhập | `Id`, `MaNguoiDung`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `PasswordHash`, `EmailConfirmed`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount` | FK `MaNguoiDung -> NGUOI_DUNG.MaNguoiDung` |
| `Aspnetroles` / `AspNetRoles` | Vai trò hệ thống | `Id`, `Name`, `NormalizedName` | Liên kết qua `AspNetUserRoles` và `AspNetRoleClaims` |
| `Aspnetuserroles` / `AspNetUserRoles` | Bảng gán user-role | `Asp_Id`, `Id` (role id) | FK `Asp_Id -> AspNetUsers.Id`, FK `Id -> AspNetRoles.Id` |
| `Aspnetroleclaims` / `AspNetRoleClaims` | Gán permission cho role | `Asp_Id` (role id), `MaDanhMucQuyen`, `ClaimType`, `ClaimValue` | FK `Asp_Id -> AspNetRoles.Id`, FK `MaDanhMucQuyen -> DANH_MUC_QUYEN` |
| `Aspnetusertokens` / `AspNetUserTokens` | Lưu token OTP/phiên reset mật khẩu | `Id`, `LoginProvider`, `Name`, `Value` | FK `Id -> AspNetUsers.Id`; dùng cho forgot/reset password |
| `ChucDanh` / `CHUC_DANH` | Danh mục chức danh nhân sự | `MaChucDanh`, `TenChucDanh`, `MoTaChucDanh` | FK từ `NGUOI_DUNG.MaChucDanh` |

Khác biệt rõ:
- **Hồ sơ nhân sự** nằm ở `NGUOI_DUNG`.
- **Tài khoản đăng nhập** nằm ở `AspNetUsers`.
- Hai bảng nối bằng 2 chiều: `NguoiDung.Id` (string) và `Aspnetusers.MaNguoiDung` (int), tạo ràng buộc liên kết profile-account.

## 4. Form thêm nhân sự hiện tại

| Trường giao diện | ViewModel property | Entity/Bảng lưu | Bắt buộc | Validation | Ghi chú |
| ---------------- | ------------------ | --------------- | -------- | ---------- | ------- |
| Họ tên | `HoTenNguoiDung` | `NGUOI_DUNG.HoTenNguoiDung` | Có | `[Required]`, `[MaxLength(255)]` | Nhập trực tiếp |
| Chức danh | `MaChucDanh` | `NGUOI_DUNG.MaChucDanh` | Có | `[Required]` | Chọn từ `DanhSachChucDanh` |
| Số điện thoại | `SdtNguoiDung` | `NGUOI_DUNG.SdtNguoiDung`, `AspNetUsers.PhoneNumber` | Có | `[Required]`, `[MaxLength(20)]`, regex số | Service kiểm tra trùng số |
| Ngày sinh | `NgaySinh` | `NGUOI_DUNG.NgaySinh` | Có | `[Required]` | Không có annotation chặn ngày tương lai ở form Nhân sự |
| Địa chỉ | `DiaChiNguoiDung` | `NGUOI_DUNG.DiaChiNguoiDung` | Có | `[Required]`, `[MaxLength(255)]` | Textarea |
| Tên đăng nhập | `UserName` | `AspNetUsers.UserName`, `NormalizedUserName` | Có | `[Required]`, `[MaxLength(256)]` | Khi sửa: readonly |
| Email | `Email` | `AspNetUsers.Email`, `NormalizedEmail` | Có | `[Required]`, `[MaxLength(256)]`, `[EmailAddress]` | Khi sửa: readonly, service cấm đổi |
| Vai trò hệ thống | `RoleId` | `AspNetUserRoles.Id` | Có | `[Required]` | Chọn từ role hệ thống |
| Mật khẩu (thêm mới) | `Password` | `AspNetUsers.PasswordHash` | Có khi tạo mới | `[MinLength(6)]`, `[MaxLength(100)]` + `IValidatableObject` buộc nhập khi tạo | Không có trường xác nhận mật khẩu ở form Nhân sự |
| Reset mật khẩu (khi sửa) | `ResetPassword` | `AspNetUsers.PasswordHash` | Không | `[MinLength(6)]`, `[MaxLength(100)]` | Nếu nhập thì hash lại và gửi email thông báo reset |

## 5. Luồng thêm nhân sự hiện tại

1. Admin bấm **Lưu** trên `Views/NhanSu/_Form.cshtml`, submit POST đến `NhanSuController.LuuNhanSu`.
2. Controller (`Controllers/NhanSuController.cs`) kiểm tra `ModelState`. Nếu lỗi thì nạp lại danh sách + dropdown và trả lại view `Index`.
3. Controller xác định thêm mới khi `Form.MaNguoiDung == null`, kiểm tra quyền `Permissions.NhanSu.Them`.
4. Controller gọi `NhanSuService.SaveAsync(model, laAdminDangThaoTac)`.
5. Service (`Services/Implementations/NhanSuService.cs`) validate:
   - `MaChucDanh` tồn tại.
   - `UserName` chưa tồn tại theo `NormalizedUserName`.
   - `Email` chưa tồn tại theo `NormalizedEmail`.
   - `SdtNguoiDung` chưa trùng trong `NGUOI_DUNG` chưa xóa.
   - `RoleId` tồn tại.
   - Nếu role mới là admin: chỉ admin mới tạo được + không vượt `MaxAdminCount = 3`.
6. Service mở transaction `BeginTransactionAsync`.
7. Tạo bản ghi `NguoiDung` trước, `SaveChangesAsync` để lấy `MaNguoiDung`.
8. Tạo `Aspnetusers` sau:
   - Gán `EmailConfirmed = true`.
   - Gán `LockoutEnabled = true`.
   - Hash mật khẩu bằng `PasswordHasher<Aspnetusers>.HashPassword(account, model.Password!)`.
9. Tạo bản ghi `Aspnetuserroles` để gán role.
10. Gán ngược `NguoiDung.Id = userId` rồi `SaveChangesAsync`, `CommitAsync`.
11. Controller set `TempData["Success"]`, redirect về `Index`.

Transaction/rollback:
- Có transaction explicit cho luồng tạo mới nhân sự + tài khoản + role.
- Nếu lỗi trước commit: transaction rollback (do exception).
- Không thấy create hồ sơ và account tách rời ngoài transaction trong `SaveAsync` nhánh thêm mới.

## 6. Luồng sửa nhân sự

- Action: `NhanSuController.Sua(int id)` lấy form; lưu bằng `NhanSuController.LuuNhanSu` với `MaNguoiDung != null`.
- Quyền: cần `Permissions.NhanSu.Sua`.
- Trường được sửa:
  - Được: họ tên, địa chỉ, SĐT, ngày sinh, chức danh, role (`RoleId`), reset mật khẩu (tùy chọn).
  - Không được: username (readonly ở view), email (readonly ở view + service chặn bằng exception nếu khác).
- Validation trùng:
  - SĐT: kiểm tra trùng trong `NguoiDung` (trừ chính mình).
  - Không có luồng đổi username/email nên không có check trùng cho 2 trường khi sửa.
- Đổi role:
  - Kiểm tra role tồn tại.
  - Kiểm tra quyền gán admin và giới hạn tối đa admin.
  - Kiểm tra ràng buộc nghiệp vụ qua `KiemTraRangBuocDoiRoleAsync` (đang quản lý dự án, leader team, thuộc dự án/team, còn công việc chưa hoàn thành -> chặn đổi role).
- Reset mật khẩu khi sửa:
  - Nếu `ResetPassword` có giá trị: hash lại `PasswordHash`, đổi `SecurityStamp`, sau đó thử gửi email thông báo.
- Cập nhật đồng thời nhiều bảng:
  - `NGUOI_DUNG` (thông tin profile)
  - `AspNetUsers` (phone + password hash nếu reset)
  - `AspNetUserRoles` (nếu đổi role)
- Ảnh hưởng cookie/claim:
  - Không thấy logic buộc đăng nhập lại khi role đổi hoặc khi username/email thay đổi (username/email thực tế không cho đổi ở màn này).
  - Claim chỉ được tạo khi đăng nhập; phiên hiện có của user bị sửa role không thấy refresh cưỡng bức.

## 7. Luồng khóa, mở khóa và xóa nhân sự

### Khóa tài khoản
- Action: `NhanSuController.KhoaTaiKhoan` -> `NhanSuService.LockAccountAsync`.
- Cập nhật: `AspNetUsers.LockoutEnabled = true`, `LockoutEnd = UtcNow + 100 năm`.
- Không đổi `NguoiDung.IsDeleted`.
- Chặn khóa nếu:
  - Nhân sự không tồn tại.
  - Đang quản lý dự án.
  - Đang là leader team.
- Còn công việc chưa hoàn thành: chỉ ghi log warning, không chặn khóa.
- Người bị khóa: đăng nhập bị chặn tại `AccountService.AuthenticateAsync` (kiểm tra `LockoutEnd`).
- Cookie hiện có: không thấy logic revoke ngay sau khi admin khóa; source không chủ động sign-out user đang online.

### Mở khóa
- Action: `NhanSuController.MoKhoaTaiKhoan` -> `NhanSuService.UnlockAccountAsync`.
- Cập nhật: `LockoutEnd = null`, `AccessFailedCount = 0`.

### Xóa nhân sự
- Action: `NhanSuController.XoaNhanSu` -> `NhanSuService.DeleteAsync`.
- Kiểu xóa: **xóa mềm** profile (`NguoiDung.IsDeleted = true`, `DeletedAt = UtcNow`).
- Không xóa thật account ở `AspNetUsers`.
- Ràng buộc chặn xóa (`KiemTraRangBuocXoaAsync`):
  - Đang quản lý dự án.
  - Đang là leader team.
  - Còn thuộc team/dự án.
  - Thuộc team đang tham gia dự án.
  - Còn công việc chưa hoàn thành.

Rule đặc biệt:
- Chưa thấy rule chặn khóa chính mình/chặn khóa admin cuối cùng ở `NhanSuService`.

## 8. Luồng đăng nhập của tài khoản nhân sự

- Form: `Views/Account/Login.cshtml`.
- Action: `AccountController.Login` (POST, có `[ValidateAntiForgeryToken]`).
- Dịch vụ: `AccountService.AuthenticateAsync`.
- Dữ liệu đăng nhập:
  - View ghi "Tên đăng nhập".
  - Service tìm theo `NormalizedUserName` hoặc `UserName` (không kiểm tra email tại login).
- Kiểm tra mật khẩu:
  - `PasswordHasher<Aspnetusers>.VerifyHashedPassword`.
- Kiểm tra khóa:
  - Nếu `LockoutEnabled` và `LockoutEnd > UtcNow` => từ chối đăng nhập.
- Tạo claim/cookie:
  - Claim cơ bản: `NameIdentifier`, `Name`, `MaNguoiDung`.
  - Claim role từ `AspNetUserRoles` + `AspNetRoles`.
  - Claim permission từ `AspNetRoleClaims` (và fallback `DanhMucQuyen`).
  - Claim user riêng từ `AspNetUserClaims`.
  - Tạo cookie auth qua `HttpContext.SignInAsync` (thời hạn 8h hoặc 7 ngày nếu nhớ đăng nhập).
- Email confirmed:
  - Không thấy kiểm tra `EmailConfirmed` khi login.
- Đổi mật khẩu lần đầu:
  - Chưa thấy cờ/luồng bắt buộc đổi lần đầu.
- Quên/reset mật khẩu:
  - Có: forgot password OTP (`KhoiTaoQuenMatKhauAsync`, `XacNhanOtpDatLaiMatKhauAsync`, `DatLaiMatKhauBangOtpAsync`).

## 9. Phân tích email hiện tại

- Email hiện vừa là thông tin tài khoản vừa dùng cho quên mật khẩu OTP.
- Ở form tạo nhân sự, email là bắt buộc.
- Tính unique email:
  - Có check nghiệp vụ trong `NhanSuService.SaveAsync` theo `NormalizedEmail`.
  - Chưa thấy unique index DB cho `Email`/`NormalizedEmail` trong migration đã đọc.
- Xác minh email:
  - Tạo tài khoản mới set cứng `EmailConfirmed = true`.
  - Không thấy luồng gửi email xác minh.
- Gửi email khi tạo nhân sự:
  - Không thấy gửi email ở nhánh tạo mới.
- Có hạ tầng SMTP/email service:
  - Có `IEmailService` + `GmailEmailService` dùng `System.Net.Mail.SmtpClient`.
- Khả năng tái sử dụng:
  - Đang được dùng cho forgot password OTP và email thông báo reset mật khẩu từ admin.
- Chức năng kích hoạt tài khoản:
  - Chưa thấy.
- Token kích hoạt/reset:
  - Có token reset OTP trong `AspNetUserTokens` cho quên mật khẩu.
  - Chưa thấy token kích hoạt tài khoản nhân sự mới.

## 10. Phân tích mật khẩu hiện tại

- Ai nhập mật khẩu:
  - Admin nhập mật khẩu khi tạo nhân sự mới (`NhanSu` form).
  - Admin có thể nhập mật khẩu mới để reset trong màn sửa nhân sự.
  - Người dùng tự nhập mật khẩu mới ở luồng quên mật khẩu OTP và đổi mật khẩu cá nhân.
- Mật khẩu có hash:
  - Có, dùng `PasswordHasher<Aspnetusers>`.
- Method/thư viện:
  - `HashPassword` và `VerifyHashedPassword` từ `Microsoft.AspNetCore.Identity`.
- Yêu cầu độ mạnh:
  - Chỉ thấy min/max length (6-100), chưa thấy policy phức tạp (hoa/thường/số/ký tự đặc biệt).
- Xác nhận mật khẩu:
  - Có ở `ResetPasswordViewModel` và `DoiMatKhauViewModel`.
  - Không có ở form tạo/sửa nhân sự (`NhanSu`).
- Bắt buộc đổi mật khẩu lần đầu:
  - Chưa thấy triển khai.
- Admin có thể biết mật khẩu nhân viên:
  - Về quy trình, admin là người nhập khi tạo/reset nên biết mật khẩu plaintext tại thời điểm nhập.
- Nguy cơ lộ mật khẩu:
  - Không thấy ghi log mật khẩu trong source đã đọc.
  - Form dùng input password.
  - Không thấy trả `PasswordHash` ra UI.
  - `TempData/ModelState` có thể chứa message lỗi chung, không thấy nhét mật khẩu vào message.
- Chức năng đổi/reset mật khẩu:
  - Có đổi mật khẩu cá nhân.
  - Có reset bằng OTP qua email.
  - Có reset bởi admin tại màn sửa nhân sự.

## 11. Phân quyền của chức năng Nhân sự

| Hành động | Controller/Action | Permission | Kiểm tra scope/rule bổ sung |
| --------- | ----------------- | ---------- | --------------------------- |
| Xem nhân sự | `NhanSuController.Index` | `NhanSu.Xem` | Không có scope theo phòng ban/team |
| Thêm nhân sự | `NhanSuController.LuuNhanSu` (nhánh create) | `NhanSu.Them` | Service chặn tạo role Admin nếu người thao tác không phải admin; giới hạn tối đa 3 admin |
| Sửa nhân sự | `NhanSuController.Sua`, `LuuNhanSu` (nhánh update) | `NhanSu.Sua` | Service chặn đổi email, ràng buộc đổi role theo dự án/team/công việc |
| Xóa nhân sự | `NhanSuController.XoaNhanSu` | `NhanSu.Xoa` | Service xóa mềm + chặn nếu còn liên kết nghiệp vụ |
| Khóa tài khoản | `NhanSuController.KhoaTaiKhoan` | `NhanSu.Khoa` | Service chặn nếu đang quản lý dự án/leader team |
| Mở khóa tài khoản | `NhanSuController.MoKhoaTaiKhoan` | `NhanSu.MoKhoa` | Không có rule bổ sung ngoài tồn tại account |
| Đổi vai trò | `NhanSuController.LuuNhanSu` (update `RoleId`) | `NhanSu.Sua` | Service `KiemTraRangBuocDoiRoleAsync`, giới hạn admin, chỉ admin mới gán admin |

Ghi chú:
- Service không tự kiểm tra permission claim; kiểm tra chủ yếu ở controller.
- Admin **không tự bypass mặc định** trong `PermissionHelper` (chỉ đọc claim permission), trừ các rule nghiệp vụ trong `NhanSuService` có kiểm tra `User.IsInRole("ADMIN"/"Admin")`.

## 12. Quan hệ với các module khác

| Module | Cách tham chiếu nhân sự | Ảnh hưởng khi khóa/xóa/đổi tài khoản |
| ------ | ----------------------- | ------------------------------------ |
| Team và thành viên team | `NHAN_VIEN_TEAM.MaNguoiDung`, `ThanhVienTeamService` | Khóa: không thêm/gán leader cho tài khoản lock; Xóa mềm bị chặn nếu còn trong team |
| Thành viên dự án | `NHAN_VIEN_DU_AN.MaNguoiDung`, `NhanVienDuAnService` | Khóa: không thêm nhân viên lock vào dự án; Xóa mềm bị chặn nếu còn trong dự án |
| Quản lý dự án | `DU_AN.MaNguoiDung` | Đổi role/khóa/xóa bị chặn nếu đang quản lý dự án |
| Phân công công việc | `PHAN_CONG_CONG_VIEC.MaNguoiDung`, `PhanCongCongViecService` | Tài khoản lock không được phân công; xóa nhân sự bị chặn nếu còn việc chưa hoàn thành |
| Phân công chi tiết công việc | `PHAN_CONG_CT_CONG_VIEC.MaNguoiDung` | Tham chiếu FK nhân sự; rủi ro nếu xóa cứng (hiện tại xóa mềm profile nên giảm rủi ro FK) |
| Đánh giá nhân viên/dự án | `DANH_GIA_*` có nhiều cột `MaNguoiDung*` | Xóa mềm không xóa dữ liệu đánh giá lịch sử |
| Đề xuất công việc/ngân sách | `DE_XUAT_*` cột `MaNguoiDung*` | Đổi role có thể bị chặn nếu còn dữ liệu liên quan |
| Chat dự án | `TIN_NHAN.MaNguoiDung`, `THANH_VIEN_PHONG_CHAT.MaNguoiDung` | `ChatDuAnService` lọc nhân sự `IsDeleted != true`; admin không tham gia chat dự án |
| Nhật ký | nhiều bảng `NHAT_KY_*` lưu `MaNguoiDung` | Lịch sử vẫn giữ khi profile bị soft delete |
| AI dataset/kết quả | qua `DU_AN` và module dự án | Ảnh hưởng gián tiếp khi đổi quản lý/nhân sự dự án |

## 13. Các quy tắc nghiệp vụ hiện tại

- Giới hạn tối đa tài khoản admin: **có** (`NhanSuService.MaxAdminCount = 3`).
- Chỉ admin được tạo/gán vai trò admin: **có**.
- Không cho trùng username/email khi tạo: **có** (service check normalized).
- Không cho trùng số điện thoại: **có** (service check `NguoiDung.SdtNguoiDung`).
- Không cho đổi email khi sửa nhân sự: **có**.
- Không cho đổi role trong nhiều trường hợp đang dính nghiệp vụ (quản lý dự án, leader team, còn phân công...): **có**.
- Không cho xóa nhân sự đang được sử dụng: **có**.
- Xóa nhân sự là soft delete: **có** (`IsDeleted`, `DeletedAt`).
- Chặn thao tác dựa trên permission: **có** ở controller.
- Không cho khóa chính mình: **Chưa thấy triển khai trong source**.
- Không cho khóa admin cuối cùng: **Chưa thấy triển khai trong source**.
- Giới hạn số lần đăng nhập sai và lockout tự động: **Chưa thấy triển khai rõ** (có trường `AccessFailedCount`, nhưng không thấy tăng sai mật khẩu trong `AuthenticateAsync`).

## 14. Các vấn đề và rủi ro phát hiện

### 14.1 Bảo mật

1) **Mức độ: Cao**  
   - Bằng chứng: `appsettings.json` đang chứa trực tiếp khóa cấu hình email gồm cả trường mật khẩu ứng dụng.  
   - Tác động: lộ credential SMTP nếu source bị truy cập.  
   - Chưa sửa ở bước này.

2) **Mức độ: Trung bình**  
   - Bằng chứng: tạo nhân sự mới bắt admin nhập trực tiếp mật khẩu (`Views/NhanSu/_Form.cshtml`, `NhanSuCreateUpdateViewModel`).  
   - Tác động: quy trình cấp phát mật khẩu thủ công, tăng rủi ro lộ mật khẩu ban đầu.  
   - Chưa sửa ở bước này.

3) **Mức độ: Trung bình**  
   - Bằng chứng: `NhanSuService` set `EmailConfirmed = true` khi tạo account, không có xác thực email thực.  
   - Tác động: email không được verify nhưng được coi đã xác nhận.  
   - Chưa sửa ở bước này.

### 14.2 Toàn vẹn dữ liệu

1) **Mức độ: Trung bình**  
   - Bằng chứng: migration `20260527125053_Init.cs` không thấy unique index cho `AspNetUsers.NormalizedUserName`, `NormalizedEmail`, `NguoiDung.SdtNguoiDung`.  
   - Tác động: nếu race condition hoặc import trực tiếp DB có thể phát sinh bản ghi trùng.  
   - Chưa sửa ở bước này.

2) **Mức độ: Thấp**  
   - Bằng chứng: soft delete chỉ áp dụng profile `NguoiDung`, tài khoản `AspNetUsers` không bị disable tự động khi xóa mềm.  
   - Tác động: cần kiểm tra thêm luồng đăng nhập với user có profile đã soft delete; nguy cơ lệch trạng thái profile-account.  
   - Chưa sửa ở bước này.

### 14.3 Nghiệp vụ

1) **Mức độ: Trung bình**  
   - Bằng chứng: chưa thấy chặn khóa chính mình/chặn khóa admin cuối cùng trong `NhanSuService.LockAccountAsync`.  
   - Tác động: có thể khóa nhầm tài khoản quan trọng vận hành hệ thống.  
   - Chưa sửa ở bước này.

2) **Mức độ: Trung bình**  
   - Bằng chứng: đổi role không có cơ chế thu hồi phiên đăng nhập hiện tại của user bị đổi role.  
   - Tác động: quyền thực thi có thể lệch tạm thời đến lần đăng nhập kế tiếp.  
   - Chưa sửa ở bước này.

### 14.4 Trải nghiệm người dùng

1) **Mức độ: Thấp**  
   - Bằng chứng: form Nhân sự không có trường xác nhận mật khẩu và không có hướng dẫn mạnh mật khẩu.  
   - Tác động: dễ nhập nhầm mật khẩu cấp cho nhân viên.  
   - Chưa sửa ở bước này.

2) **Mức độ: Thấp**  
   - Bằng chứng: action POST của Nhân sự (`LuuNhanSu`, `XoaNhanSu`, `KhoaTaiKhoan`, `MoKhoaTaiKhoan`) không gắn `[ValidateAntiForgeryToken]` rõ ràng.  
   - Tác động: phụ thuộc vào cơ chế mặc định của form tag helper; cần kiểm chuẩn chính sách CSRF toàn hệ thống.  
   - Chưa sửa ở bước này.

## 15. Mức độ sẵn sàng cho quy trình kích hoạt qua email

| Thành phần cần có | Hiện trạng | Có thể tái sử dụng | Cần bổ sung |
| ----------------- | ---------- | ------------------ | ----------- |
| Email bắt buộc và unique | Bắt buộc ở ViewModel; unique bằng check service | Một phần | Unique index DB cứng |
| Email service | Đã có `IEmailService` + `GmailEmailService` | Có | Hardening cấu hình secret |
| SMTP configuration | Có trong `appsettings.json` | Có | Tách secret khỏi source |
| Trạng thái `ChoKichHoat` | Chưa thấy | Không | Thêm trạng thái |
| Token kích hoạt | Chưa thấy | Chưa | Bổ sung luồng token kích hoạt |
| Hash token | Có kỹ thuật hash OTP reset (`SHA256`) | Có thể tham khảo | Cần áp dụng cho activation token |
| Thời hạn token | Có cho OTP reset (3 phút) | Có thể tham khảo | Cần policy riêng cho activation |
| Liên kết chỉ dùng một lần | Có ý tưởng trong `AspNetUserTokens` cho reset | Có thể tái sử dụng pattern | Cần triển khai cho kích hoạt |
| Trang đặt mật khẩu | Có `ResetPassword` cho forgot flow | Có thể tái sử dụng UI/validation | Cần tách context activation |
| Validation mật khẩu | Có min length + compare ở reset | Có | Có thể cần policy mạnh hơn |
| Gửi lại email kích hoạt | Chưa thấy | Không | Bổ sung |
| Thu hồi token cũ | Có `XoaTokenQuenMatKhauAsync` cho reset | Có thể tái sử dụng pattern | Bổ sung cho activation |
| Nhật ký gửi/kích hoạt | Chưa thấy | Không | Bổ sung |
| Chặn đăng nhập trước kích hoạt | Chưa thấy (không check email confirmed) | Không | Bổ sung |
| Quên mật khẩu | Đã có OTP reset | Có | Tối ưu bảo mật/thời hạn |
| Bắt buộc đổi mật khẩu lần đầu | Chưa thấy | Không | Bổ sung |

## 16. Sơ đồ workflow AS-IS

### 16.1 Thêm nhân sự

```mermaid
flowchart LR
    A[Admin] --> B[Form NhanSu _Form.cshtml]
    B --> C[NhanSuController.LuuNhanSu]
    C --> D[INhanSuService.SaveAsync]
    D --> E[Tao NguoiDung]
    E --> F[Tao AspNetUsers + hash PasswordHash]
    F --> G[Tao AspNetUserRoles]
    G --> H[Commit transaction]
    H --> I[Redirect Index + TempData]
```

### 16.2 Đăng nhập

```mermaid
flowchart LR
    A[Nguoi dung] --> B[Account/Login]
    B --> C[AccountController.Login]
    C --> D[AccountService.AuthenticateAsync]
    D --> E[Kiem tra username]
    E --> F[Kiem tra PasswordHash]
    F --> G[Kiem tra LockoutEnd]
    G --> H[Tao claims role/permission]
    H --> I[SignIn cookie]
```

## 17. Các câu hỏi còn chưa xác định được

- DB production thực tế có thêm unique index ngoài migration trong repo hay không.
- Có cơ chế global anti-forgery filter ở nơi khác ngoài các controller đã đọc hay không.
- Không thấy `appsettings.Development.json` trong phạm vi đã đọc nên chưa đối chiếu khác biệt cấu hình môi trường.
- Không có log/audit chuyên biệt cho sự kiện tạo tài khoản nhân sự trong phạm vi source đã đọc.
- Mức độ đồng bộ cookie/claim sau đổi role phụ thuộc middleware/chính sách phiên runtime, source hiện không cưỡng bức sign-out.

## 18. Kết luận AS-IS

- Chức năng Nhân sự hiện tại tạo tài khoản theo mô hình **admin nhập trực tiếp thông tin + mật khẩu**, service tạo `NguoiDung` và `AspNetUsers` trong transaction, rồi gán role.
- Email hiện là dữ liệu tài khoản bắt buộc, có dùng cho forgot password OTP; chưa có kích hoạt email cho tài khoản mới.
- Mật khẩu hiện được hash bằng `PasswordHasher` của ASP.NET Core Identity; có đổi/reset mật khẩu nhưng chưa có bắt buộc đổi lần đầu.
- Mức độ an toàn ở mức trung bình: có hash mật khẩu, có lockout thủ công, có OTP reset; nhưng còn rủi ro về quản trị secret SMTP trong source và quy trình cấp mật khẩu thủ công.
- Thành phần tái sử dụng tốt cho bước kích hoạt email sau này: `IEmailService`, bảng `AspNetUserTokens`, pattern hash/token expiry trong `AccountService`.
- Thành phần còn thiếu để chuyển sang quy trình mời kích hoạt: trạng thái chờ kích hoạt, token activation 1 lần, chặn login trước kích hoạt, resend/thu hồi token, nhật ký kích hoạt.
# TRẠNG THÁI SAU TRIỂN KHAI KÍCH HOẠT TÀI KHOẢN QUA EMAIL

## 1. File đã sửa

- `QuanLyDuAn/QuanLyDuAn/Controllers/NhanSuController.cs`: thêm CSRF cho POST, thêm gửi lại email kích hoạt, điều chỉnh thông báo tạo nhân sự.
- `QuanLyDuAn/QuanLyDuAn/Controllers/AccountController.cs`: thêm GET/POST `Activate`.
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/INhanSuService.cs`, `IAccountService.cs`, `IEmailService.cs`: mở rộng hợp đồng nhân sự/account/email.
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanSuService.cs`: tạo account chưa kích hoạt, tạo token hash, gửi email sau commit, gửi lại email kích hoạt.
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AccountService.cs`: chặn login chưa kích hoạt, chặn forgot password cho account chưa kích hoạt, xử lý activation token một lần.
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/GmailEmailService.cs`: thêm template email kích hoạt.
- `QuanLyDuAn/QuanLyDuAn/Services/AccountActivationOptions.cs`, `AccountActivationTokenHelper.cs`: cấu hình và helper token activation.
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Account/ActivateAccountViewModel.cs`, `Views/Account/Activate.cshtml`: form đặt mật khẩu khi kích hoạt.
- `QuanLyDuAn/QuanLyDuAn/ViewModels/NhanSu/*`, `Views/NhanSu/_Form.cshtml`, `_Table.cshtml`, `_Filter.cshtml`: bỏ nhập password trực tiếp, thêm trạng thái/nút gửi lại.
- `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs`: thêm trạng thái `chokichhoat`.
- `QuanLyDuAn/QuanLyDuAn/Program.cs`, `appsettings.json`, `appsettings.Development.json`: bind `AccountActivation`, bỏ secret SMTP khỏi source.
- `docs/nhansu.md`: cập nhật phần TO-BE này.

## 2. Workflow TO-BE đã triển khai

```text
Admin tạo nhân sự
→ MVC tạo profile + account chưa kích hoạt
→ MVC tạo token hash
→ MVC gửi email
→ Nhân viên mở link
→ MVC xác minh token
→ Nhân viên đặt mật khẩu
→ MVC kích hoạt account
→ Token bị xóa
→ Nhân viên đăng nhập
```

## 3. Trạng thái tài khoản

- `LockoutEnd > DateTime.UtcNow`: `Đã khóa`.
- `EmailConfirmed = false` và không bị khóa: `Chờ kích hoạt`.
- `EmailConfirmed = true` và không bị khóa: `Đang hoạt động`.
- Không dùng `LockoutEnd` để biểu diễn trạng thái chờ kích hoạt.

## 4. Token activation

- Lưu trong `AspNetUserTokens` với `LoginProvider = "QuanLyDuAn"` và `Name = "AccountActivation"`.
- `Value` là JSON payload gồm `TokenHash`, `CreatedAtUtc`, `ExpiresAtUtc`.
- Token raw sinh bằng `RandomNumberGenerator.GetBytes(32)`, mã hóa URL-safe và chỉ gửi qua link email.
- DB chỉ lưu SHA-256 hash của token URL.
- Thời hạn mặc định 24 giờ qua `AccountActivation:TokenLifetimeHours`.
- Kích hoạt thành công sẽ xóa token.
- Gửi lại email sẽ thu hồi token cũ, tạo token mới và chặn gửi liên tục theo `AccountActivation:ResendCooldownSeconds`.

## 5. Email

- Service: `IEmailService` / `GmailEmailService`.
- Method: `SendAccountActivationEmailAsync(...)`.
- Email có tên hệ thống, tên nhân viên, username, link kích hoạt, thời hạn link và cảnh báo bỏ qua nếu không yêu cầu.
- Email không chứa mật khẩu, hash token hoặc secret.
- Cấu hình secret qua User Secrets hoặc environment variables: `EmailSettings__SenderEmail`, `EmailSettings__Username`, `EmailSettings__AppPassword`.
- Có thể cấu hình URL production bằng `AccountActivation__AppBaseUrl`.

## 6. Đăng nhập

- `AccountService.AuthenticateAsync` chặn tài khoản chưa `EmailConfirmed`.
- Tài khoản chưa có `PasswordHash` không được verify password và không phát sinh exception.
- Forgot password OTP không gửi cho tài khoản chưa kích hoạt.

## 7. Database/migration

- Không thay đổi schema database và không tạo migration mới.
- Việc kiểm tra trùng `NormalizedUserName` và `NormalizedEmail` được thực hiện trong `NhanSuService` trước khi tạo tài khoản.
- Luồng tạo tài khoản sử dụng transaction với isolation level `Serializable` để giảm nguy cơ tạo dữ liệu trùng khi có nhiều request đồng thời.
- Database hiện chưa có unique index bắt buộc cho `NormalizedUserName` và `NormalizedEmail`. Đây là giới hạn còn lại; unique index mới là cơ chế bảo đảm cứng ở DB nếu được bổ sung sau này bằng migration chính thức qua lệnh `Add-Migration`.
- Không thêm bảng token mới vì `AspNetUserTokens` đáp ứng được.

## 8. Kiểm thử

```text
dotnet build QuanLyDuAn\QuanLyDuAn.sln --no-restore
Kết quả: Build succeeded, 2 Warning(s), 0 Error(s). Hai warning nằm ở `FileTienDoCongViecService.cs` và là warning async thiếu await có sẵn ngoài phạm vi chỉnh sửa.
```

- Chưa chạy manual SMTP/DB activation end-to-end vì không cập nhật database runtime và không dùng secret email trong source.

## 9. Hạn chế còn lại

- Database chưa có unique index cứng cho `NormalizedUserName` và `NormalizedEmail`; nếu cần bảo đảm ở cấp DB, người phát triển nên tạo migration chính thức sau này.
- Cần cấu hình secret email ngoài source trước khi gửi email thật.
- Cần manual test end-to-end các case SMTP thành công/thất bại, link hết hạn, link đã dùng và resend trên database runtime.

# RÀ SOÁT LỖI SAU TRIỂN KHAI KÍCH HOẠT TÀI KHOẢN QUA EMAIL

## 1. Phạm vi source đã kiểm tra

- Controller: `Controllers/NhanSuController.cs`, `Controllers/AccountController.cs`.
- Service/interface: `Services/Interfaces/INhanSuService.cs`, `Services/Interfaces/IAccountService.cs`, `Services/Interfaces/IEmailService.cs`, `Services/Implementations/NhanSuService.cs`, `Services/Implementations/AccountService.cs`, `Services/Implementations/GmailEmailService.cs`.
- Helper/options: `Services/AccountActivationOptions.cs`, `Services/AccountActivationTokenHelper.cs`, `Services/CauHinhDichVu.cs`.
- Entity/DbContext: `Data/QuanLyDuAnDbContext.cs`, `Models/Entities/Aspnetusers.cs`, `Models/Entities/Aspnetusertokens.cs`, `Models/Entities/NguoiDung.cs`, `Models/Entities/Aspnetuserroles.cs`.
- ViewModel/view: `ViewModels/Account/ActivateAccountViewModel.cs`, `ViewModels/Account/ForgotPasswordViewModel.cs`, `ViewModels/Account/ResetPasswordViewModel.cs`, `ViewModels/NhanSu/NhanSuCreateUpdateViewModel.cs`, `Views/Account/Activate.cshtml`, `Views/NhanSu/_Form.cshtml`, `Views/NhanSu/_Table.cshtml`, `Views/NhanSu/Index.cshtml`, `Views/Shared/_Layout.cshtml`.
- Cấu hình: `Program.cs`, `appsettings.json`, `appsettings.Development.json`, `Properties/launchSettings.json`, `QuanLyDuAn.csproj`.
- Migration để đối chiếu schema: `Migrations/20260527125053_Init.cs`, `Migrations/QuanLyDuAnDbContextModelSnapshot.cs` (chỉ đọc, không sửa).

## 2. Workflow thực tế hiện tại

Luồng tạo nhân sự:

`Admin tạo nhân sự -> NhanSuService.SaveAsync tạo NguoiDung + Aspnetusers(EmailConfirmed=false, chưa PasswordHash) + Aspnetuserroles + Aspnetusertokens activation -> Commit DB -> gửi email kích hoạt -> nhân viên mở link -> AccountController/AccountService xác minh token -> đặt mật khẩu -> EmailConfirmed=true -> xóa token -> đăng nhập`

Luồng gửi lại email kích hoạt:

`Admin bấm gửi lại -> NhanSuService.GuiLaiEmailKichHoatAsync kiểm tra trạng thái/cooldown -> xóa token cũ + tạo token mới -> Commit DB -> gửi email mới -> trả null nếu thành công, trả warning string nếu SMTP lỗi`

## 3. Các vấn đề đã xác nhận

### Controller hiển thị đồng thời thông báo thành công và cảnh báo

- Mức độ: Trung bình
- Trạng thái: Đã xác nhận
- File liên quan: `Controllers/NhanSuController.cs`, `Views/Shared/_Layout.cshtml`
- Method/action liên quan: `NhanSuController.GuiLaiEmailKichHoat`
- Bằng chứng từ source: Controller luôn gán `TempData["Success"] = "Đã gửi lại email kích hoạt.";` ngay sau khi gọi service, kể cả khi service trả warning; layout render cả `Success` và `Warning`.
- Nguyên nhân: Không phân nhánh theo giá trị trả về `warning`.
- Tác động: Quản trị viên hiểu nhầm email đã gửi thành công dù SMTP lỗi.
- Cách tái hiện: Cấu hình SMTP sai -> bấm gửi lại email kích hoạt.
- Hướng xử lý đề xuất: Chỉ set `Success` khi service trả `null`; nếu có chuỗi cảnh báo thì set `Warning`/`Error` và không set `Success`.
- Có cần thay đổi database không: Không

### Resend thu hồi token cũ và commit token mới trước khi gửi email

- Mức độ: Cao
- Trạng thái: Đã xác nhận
- File liên quan: `Services/Implementations/NhanSuService.cs`
- Method/action liên quan: `GuiLaiEmailKichHoatAsync`
- Bằng chứng từ source: `RemoveRange(oldTokens)` + `Add(new token)` + `SaveChangesAsync` + `CommitAsync`, sau đó mới gọi `_emailService.SendAccountActivationEmailAsync(...)`.
- Nguyên nhân: Thiết kế commit DB trước SMTP để tránh transaction mở lâu.
- Tác động: Nếu SMTP fail, link cũ đã mất hiệu lực; người dùng không nhận link mới; admin có thể tưởng đã gửi.
- Cách tái hiện: Cho SMTP fail tại thời điểm resend.
- Hướng xử lý đề xuất: Cân nhắc outbox hoặc cơ chế giữ token cũ đến khi gửi mới thành công; nếu chưa đổi kiến trúc thì UI phải cảnh báo rõ và cho retry sớm.
- Có cần thay đổi database không: Không (nếu xử lý bằng nghiệp vụ); Có (nếu áp dụng outbox)

### Cooldown có thể chặn gửi lại ngay sau lỗi SMTP

- Mức độ: Trung bình
- Trạng thái: Đã xác nhận
- File liên quan: `Services/Implementations/NhanSuService.cs`
- Method/action liên quan: `GuiLaiEmailKichHoatAsync`
- Bằng chứng từ source: Cooldown dựa vào `CreatedAtUtc` của token mới đã commit; sau lỗi SMTP token này vẫn tồn tại và bị tính cooldown.
- Nguyên nhân: Token mới không rollback khi gửi mail thất bại.
- Tác động: Admin không thể resend ngay lập tức sau lỗi kỹ thuật tạm thời.
- Cách tái hiện: SMTP fail -> bấm resend lại trong khoảng < `ResendCooldownSeconds`.
- Hướng xử lý đề xuất: Không áp cooldown cho token chưa gửi thành công hoặc cho phép bypass cooldown với lỗi SMTP xác định.
- Có cần thay đổi database không: Không

### Luồng tạo nhân sự commit dữ liệu trước khi gửi email

- Mức độ: Trung bình
- Trạng thái: Đã xác nhận
- File liên quan: `Services/Implementations/NhanSuService.cs`, `Controllers/NhanSuController.cs`
- Method/action liên quan: `SaveAsync`, `LuuNhanSu`
- Bằng chứng từ source: Trong nhánh create, transaction commit trước khi gửi mail; nếu gửi thất bại thì service trả warning `"Đã tạo nhân sự nhưng chưa gửi được email..."`.
- Nguyên nhân: Không có outbox; SMTP tách khỏi transaction DB.
- Tác động: Có thể tồn tại account chờ kích hoạt nhưng chưa có email đầu tiên.
- Cách tái hiện: Tạo nhân sự khi SMTP lỗi.
- Hướng xử lý đề xuất: Giữ cảnh báo rõ + hướng dẫn thao tác resend; cân nhắc outbox để tăng độ tin cậy.
- Có cần thay đổi database không: Không bắt buộc

### appsettings hiện có AppBaseUrl thiếu scheme

- Mức độ: Cao
- Trạng thái: Đã xác nhận
- File liên quan: `appsettings.json`, `Services/Implementations/NhanSuService.cs`
- Method/action liên quan: `TaoActivationUrl`
- Bằng chứng từ source: `AccountActivation:AppBaseUrl` đang là dạng `192.168.2.27:5037`; method yêu cầu `Uri.TryCreate(...Absolute)` và scheme `http/https`, nếu sai sẽ throw.
- Nguyên nhân: Cấu hình không đúng định dạng URL tuyệt đối.
- Tác động: Không gửi được email kích hoạt/resend vì không tạo được link.
- Cách tái hiện: Dùng nguyên cấu hình hiện tại rồi tạo nhân sự/resend.
- Hướng xử lý đề xuất: Cấu hình `http://192.168.x.x:5037` hoặc `https://...`; thêm validate lúc startup.
- Có cần thay đổi database không: Không

## 4. Các vấn đề có nguy cơ xảy ra

### Không có lock chống race condition khi hai resend chạy đồng thời

- Mức độ: Trung bình
- Trạng thái: Có nguy cơ
- File liên quan: `Services/Implementations/NhanSuService.cs`
- Method/action liên quan: `GuiLaiEmailKichHoatAsync`
- Bằng chứng từ source: Đọc `oldTokens` và cooldown trước transaction; transaction mặc định (không chỉ định isolation level), không có concurrency token/retry.
- Nguyên nhân: Thiếu cơ chế idempotency hoặc lock logic theo user.
- Tác động: Hai request có thể cùng vượt check cooldown và ghi đè token lẫn nhau.
- Cách tái hiện: Gửi song song 2 request POST resend.
- Hướng xử lý đề xuất: Thêm khóa nghiệp vụ theo user hoặc nâng isolation phù hợp + xử lý retry conflict.
- Có cần thay đổi database không: Không bắt buộc

### Không bắt riêng DbUpdateException/concurrency khi resend và activate

- Mức độ: Trung bình
- Trạng thái: Có nguy cơ
- File liên quan: `Services/Implementations/NhanSuService.cs`, `Services/Implementations/AccountService.cs`
- Method/action liên quan: `GuiLaiEmailKichHoatAsync`, `KichHoatTaiKhoanAsync`
- Bằng chứng từ source: Không có catch riêng `DbUpdateException`/`DbUpdateConcurrencyException`.
- Nguyên nhân: Xử lý lỗi gộp `Exception`.
- Tác động: Khó phân loại lỗi vận hành, khó gợi ý thao tác retry chuẩn.
- Cách tái hiện: Gây conflict ghi đồng thời token/account.
- Hướng xử lý đề xuất: Bắt riêng lỗi DB để trả thông điệp phù hợp và log đầy đủ.
- Có cần thay đổi database không: Không

### Không có outbox/retry SMTP có kiểm soát

- Mức độ: Trung bình
- Trạng thái: Có nguy cơ
- File liên quan: `Services/Implementations/NhanSuService.cs`, `Services/Implementations/GmailEmailService.cs`
- Method/action liên quan: `SaveAsync`, `GuiLaiEmailKichHoatAsync`, `SendAsync`
- Bằng chứng từ source: Gửi SMTP trực tiếp, không queue/outbox, không retry policy.
- Nguyên nhân: Kiến trúc gửi mail đồng bộ.
- Tác động: Lỗi mạng thoáng qua gây fail gửi; admin phải thao tác lại.
- Cách tái hiện: Mất mạng ngắn hoặc timeout SMTP.
- Hướng xử lý đề xuất: Outbox + worker retry idempotent.
- Có cần thay đổi database không: Có (nếu triển khai outbox)

### Không validate mạnh cấu hình SMTP trước khi gửi

- Mức độ: Thấp
- Trạng thái: Có nguy cơ
- File liên quan: `Services/Implementations/GmailEmailService.cs`, `Program.cs`
- Method/action liên quan: `SendAsync`
- Bằng chứng từ source: Chỉ check rỗng 3 field (`SenderEmail`, `Username`, `AppPassword`), không validate `SenderEmail` format hoặc `SenderEmail` khớp tài khoản xác thực.
- Nguyên nhân: Dùng `IConfiguration` trực tiếp, không có options validation startup.
- Tác động: Lỗi runtime khi gửi mới phát hiện sai cấu hình.
- Cách tái hiện: Để `SenderEmail` không hợp lệ hoặc lệch account.
- Hướng xử lý đề xuất: Dùng options class + validate startup (`ValidateOnStart`).
- Có cần thay đổi database không: Không

## 5. Phân tích lỗi gửi lại email hiện tại

### Tổng kết theo checklist resend

- Đã kiểm tra tồn tại nhân sự+tài khoản: Có (`join NguoiDung + Aspnetusers`).
- Chặn account đã kích hoạt: Có (`EmailConfirmed`).
- Chặn account bị khóa: Có (`LockoutEnd > UtcNow`).
- Chặn email rỗng: Có.
- Tìm token cũ đúng provider/name: Có (`QuanLyDuAn` + `AccountActivation`).
- Cooldown: Có (`CreatedAtUtc + ResendCooldownSeconds`).
- Thu hồi token cũ: Có (`RemoveRange(oldTokens)`).
- Tạo token mới đúng cấu trúc hash payload: Có.
- Commit trước gửi mail: Có.
- SMTP fail thì token mới vẫn còn hiệu lực: Có.
- Resend lại ngay sau lỗi SMTP bị cooldown: Có thể xảy ra (đã xác nhận về logic).
- Nguy cơ mất token cũ khi token mới chưa gửi được: Có (đã xác nhận).
- Rollback token khi SMTP fail: Không có.
- Giữ token cũ đến khi gửi mới thành công: Chưa có.
- Race condition resend đồng thời: Có nguy cơ.
- Transaction hiện tại chủ yếu bao quanh thao tác DB: Đúng.
- Xử lý DbUpdateException/concurrency: Chưa có bắt riêng.
- Logging: Có log lỗi với `MaNguoiDung`, không log raw token/url/secret trong các message hiện tại.

## 6. Phân tích lỗi link localhost

### Không fallback âm thầm về request hiện tại khi `AppBaseUrl` sai/rỗng

- Mức độ: Thấp
- Trạng thái: Đã xác nhận
- File liên quan: `Services/Implementations/NhanSuService.cs`
- Method/action liên quan: `TaoActivationUrl`
- Bằng chứng từ source: Code mới throw khi `AppBaseUrl` rỗng/sai; block fallback theo `HttpContext` đã bị comment.
- Nguyên nhân: Chủ động ngăn link localhost sai ngữ cảnh.
- Tác động: Tránh gửi nhầm link localhost nhưng yêu cầu cấu hình chuẩn.
- Cách tái hiện: Để `AppBaseUrl` rỗng hoặc sai scheme.
- Hướng xử lý đề xuất: Giữ hành vi fail-fast; thêm kiểm tra ngay startup để phát hiện sớm.
- Có cần thay đổi database không: Không

### Rủi ro truy cập từ điện thoại phụ thuộc cấu hình profile chạy

- Mức độ: Trung bình
- Trạng thái: Có nguy cơ
- File liên quan: `Properties/launchSettings.json`, `appsettings.json`
- Method/action liên quan: chạy profile `http`/`https`
- Bằng chứng từ source: Có profile chạy LAN `http://192.168.2.27:5037`; profile IIS Express và localhost vẫn tồn tại.
- Nguyên nhân: Dễ chạy nhầm profile.
- Tác động: Link hợp lệ về mặt URL nhưng thiết bị khác không truy cập được.
- Cách tái hiện: Chạy bằng IIS Express rồi mở link trên điện thoại.
- Hướng xử lý đề xuất: Chuẩn hóa profile vận hành/dev; kiểm thử LAN + firewall.
- Có cần thay đổi database không: Không

## 7. Phân tích cấu hình SMTP và User Secrets

### Cấu hình SMTP trong code

- `GmailEmailService` dùng key: `EmailSettings:SmtpServer`, `Port`, `SenderEmail`, `SenderName`, `Username`, `AppPassword`.
- Mặc định server/port fallback: `smtp.gmail.com` và `587`.
- `EnableSsl = true`, `UseDefaultCredentials = false`, `Credentials = new NetworkCredential(username, appPassword)`.
- Không hard-code App Password.

### Đánh giá

- `SmtpServer`, `Port`, SSL và credentials hiện đúng pattern Gmail.
- Chưa thấy trim `AppPassword`; nếu secret lưu kèm khoảng trắng đầu/cuối sẽ có thể fail auth SMTP.
- Chưa bắt riêng `SmtpException` để phân biệt lỗi auth/network/recipient.
- Không log secret trong source hiện tại.

### User Secrets/môi trường

- `QuanLyDuAn.csproj` có `UserSecretsId`.
- `Program.cs` dùng `CreateBuilder`, mặc định hỗ trợ đọc user secrets ở Development.
- Service email dùng `IConfiguration` trực tiếp; thay đổi user secrets thường cần restart app để đảm bảo áp dụng ổn định.
- Cần kiểm tra thủ công khi dùng `dotnet user-secrets set` đúng project `.csproj`.

## 8. Phân tích transaction và activation token

### Transaction tạo nhân sự

- `SaveAsync` nhánh create dùng transaction `IsolationLevel.Serializable`.
- Check trùng username/email/sđt nằm trong transaction này.
- Profile (`NguoiDung`), account (`Aspnetusers`), role (`Aspnetuserroles`), token (`Aspnetusertokens`) được lưu nhất quán trước commit.
- Email gửi sau commit.

### Transaction resend

- `GuiLaiEmailKichHoatAsync` mở transaction mặc định, xóa token cũ + thêm token mới + commit trước SMTP.
- Không giữ transaction khi gửi mail, giảm lock DB nhưng tạo khoảng trống consistency DB-vs-email.

### Activation token

- Raw token sinh bằng RNG an toàn (`RandomNumberGenerator.GetBytes(32)`), URL-safe.
- DB lưu hash SHA-256 (`TokenHash`) trong JSON payload (`CreatedAtUtc`, `ExpiresAtUtc`).
- Deserialize lỗi trả `null` (bị bỏ qua trong việc chọn cooldown payload latest).
- So khớp hash dùng `CryptographicOperations.FixedTimeEquals`.
- Sau kích hoạt thành công token bị xóa.
- Token cũ sau resend bị thu hồi do `RemoveRange(oldTokens)`.
- PK `AspNetUserTokens` là `{Id, LoginProvider, Name}`.
- UTC dùng nhất quán.
- `TokenLifetimeHours <= 0` và `ResendCooldownSeconds <= 0` đều bị ép `Math.Max(1, ...)`.

## 9. Phân tích Controller và thông báo giao diện

### Render thông báo

- `Views/Shared/_Layout.cshtml` hiển thị lần lượt `TempData["Success"]`, `TempData["Error"]`, `TempData["Warning"]`.
- `Warning` có style `alert alert-warning` và hiển thị đúng vị trí cùng vùng alert.

### Vấn đề message không loại trừ nhau

- `NhanSuController.GuiLaiEmailKichHoat` set `Success` vô điều kiện rồi mới set `Warning` nếu service trả chuỗi.
- Do đó có thể hiện đồng thời 2 thông điệp trái nghĩa.

### CSRF/nút resend

- Nút resend ở `Views/NhanSu/_Table.cshtml` dùng form POST tới `GuiLaiEmailKichHoat`.
- Action có `[ValidateAntiForgeryToken]` nên có kiểm CSRF server-side.
- UI chưa có chống bấm liên tục phía client, đang dựa kiểm cooldown phía server.

## 10. Rủi ro bảo mật

### Cấu hình nhạy cảm bị lộ qua repo

- Mức độ: Trung bình
- Trạng thái: Có nguy cơ
- File liên quan: `appsettings.json`
- Method/action liên quan: cấu hình `EmailSettings`
- Bằng chứng từ source: Có key nhạy cảm cấu hình email trong file cấu hình dự án (dù đang để trống).
- Nguyên nhân: Quản lý secret chưa tách hoàn toàn khỏi config commit.
- Tác động: Dễ commit nhầm secret thật trong tương lai.
- Cách tái hiện: Điền secret trực tiếp vào file rồi commit.
- Hướng xử lý đề xuất: Bắt buộc secrets qua User Secrets/ENV; thêm guard CI scan secret.
- Có cần thay đổi database không: Không

### Lỗi SMTP chưa được phân loại chi tiết

- Mức độ: Thấp
- Trạng thái: Có nguy cơ
- File liên quan: `Services/Implementations/GmailEmailService.cs`, `Services/Implementations/NhanSuService.cs`
- Method/action liên quan: `SendAsync`, `SaveAsync`, `GuiLaiEmailKichHoatAsync`
- Bằng chứng từ source: Bắt lỗi tổng quát `Exception`.
- Nguyên nhân: Chưa bóc tách lớp lỗi transport/auth/config.
- Tác động: Khó điều tra nhanh nguyên nhân vận hành.
- Cách tái hiện: Gây lần lượt lỗi auth/network/timeout.
- Hướng xử lý đề xuất: Bắt `SmtpException` riêng, log mã lỗi phù hợp, không lộ thông tin nhạy cảm.
- Có cần thay đổi database không: Không

## 11. Kịch bản kiểm thử bắt buộc

| Mã | Điều kiện | Các bước | Kết quả mong đợi | Kết quả hiện tại nếu xác định được |
| --- | --- | --- | --- | --- |
| TC01 | SMTP đúng, AppBaseUrl đúng | Admin tạo nhân sự | Tạo account chờ kích hoạt, nhận email, mở link đặt mật khẩu được | Chưa test thủ công |
| TC02 | App Password sai | Tạo nhân sự | Dữ liệu vẫn lưu, UI báo warning gửi mail thất bại | Đúng theo source |
| TC03 | Thiếu Username SMTP | Tạo nhân sự | Ném lỗi cấu hình, trả warning phù hợp | Đúng theo source |
| TC04 | Thiếu SenderEmail | Tạo nhân sự | Ném lỗi cấu hình, trả warning phù hợp | Đúng theo source |
| TC05 | Sai SmtpServer | Tạo nhân sự | SMTP fail, account vẫn tạo, có warning | Đúng theo source |
| TC06 | Port bị firewall chặn | Tạo nhân sự | Timeout/send fail, có warning và account tồn tại | Chưa test thủ công |
| TC07 | Email người nhận sai | Tạo/resend | SMTP báo lỗi recipient, hiển thị warning/error | Chưa test thủ công |
| TC08 | Bấm resend trong cooldown | Bấm resend liên tiếp | Bị chặn với thông báo chờ X giây | Đúng theo source |
| TC09 | Bấm resend sau cooldown | Chờ > cooldown rồi resend | Tạo token mới, gửi email mới | Chưa test thủ công |
| TC10 | Resend khi SMTP lỗi | Cấu hình SMTP sai rồi resend | Token mới đã commit, warning trả về | Đúng theo source |
| TC11 | Resend khi account đã kích hoạt | Kích hoạt xong rồi resend | Bị chặn "Tài khoản đã được kích hoạt" | Đúng theo source |
| TC12 | Resend khi account bị khóa | Khóa account rồi resend | Bị chặn "Tài khoản đang bị khóa" | Đúng theo source |
| TC13 | Link chứa localhost | Cấu hình base URL localhost, gửi mail | Link chỉ dùng được trên máy đó | Chưa test thủ công |
| TC14 | `AppBaseUrl` thiếu scheme | Đặt `192.168.x.x:5037` | Throw lỗi cấu hình URL, không gửi mail | Đúng theo source |
| TC15 | `AppBaseUrl` thiếu port cần thiết | Đặt URL thiếu port thực chạy | Link không truy cập đúng endpoint | Chưa test thủ công |
| TC16 | User Secrets ghi đè AppBaseUrl | Set secret AppBaseUrl localhost | Link theo giá trị secret override | Chưa test thủ công |
| TC17 | Mở link trên cùng máy | Nhấn link từ email trên máy host | Vào form activate | Chưa test thủ công |
| TC18 | Mở link trên điện thoại cùng Wi-Fi | Nhấn link trên điện thoại | Truy cập được nếu host/profile/firewall đúng | Chưa test thủ công |
| TC19 | Link hết hạn | Chờ quá hạn token rồi mở | Báo link không hợp lệ/hết hạn | Đúng theo source |
| TC20 | Link đã sử dụng | Kích hoạt thành công rồi mở lại | Báo đã kích hoạt hoặc link hết hạn | Đúng theo source |
| TC21 | Link cũ sau resend | Resend thành công rồi mở link cũ | Link cũ không dùng được | Đúng theo source |
| TC22 | Hai request resend đồng thời | Gửi 2 POST song song | Chỉ một token cuối cùng hợp lệ, không trạng thái sai | Có nguy cơ race |
| TC23 | Hai request activate đồng thời | Gửi 2 POST activate song song | Chỉ 1 request thành công, request còn lại fail an toàn | Có nguy cơ cần test |
| TC24 | Profile bị soft delete trước kích hoạt | Xóa mềm nhân sự rồi activate | Không cho kích hoạt hoặc xử lý nhất quán | Chưa đủ bằng chứng |
| TC25 | Đổi user secrets rồi restart app | Cập nhật secrets + restart | Cấu hình mới có hiệu lực | Chưa test thủ công |
| TC26 | Chạy IIS Express | Start profile IIS Express, gửi mail | Link phải phù hợp môi trường truy cập | Chưa test thủ công |
| TC27 | Chạy profile Project Kestrel | Start profile `http`/`https` project | Link theo base URL cấu hình, truy cập LAN được | Chưa test thủ công |
| TC28 | Email vào Spam | Gửi email thật | Người dùng vẫn tìm được email (Spam/Inbox) | Chưa test thủ công |
| TC29 | DB commit thành công nhưng SMTP lỗi | Cố tình SMTP fail sau commit | Account/token tồn tại, UI cảnh báo đúng | Đúng theo source |
| TC30 | SMTP gửi thành công nhưng app lỗi sau đó | Inject lỗi sau SendAsync | Không gửi trùng/mất trạng thái | Chưa test thủ công |

## 12. Danh sách file cần sửa ở bước triển khai tiếp theo

- `Controllers/NhanSuController.cs` (logic set `TempData` theo kết quả resend).
- `Services/Implementations/NhanSuService.cs` (chiến lược resend khi SMTP fail, idempotency/race handling, bắt lỗi DB chi tiết).
- `Services/Implementations/GmailEmailService.cs` (bắt riêng `SmtpException`, validate config tốt hơn, trim app password).
- `Program.cs` + thêm options class email (validate cấu hình tại startup).
- `appsettings*.json`/user secrets usage guide (chuẩn hóa `AccountActivation:AppBaseUrl`, tránh commit secret).
- (Đề xuất kiến trúc) thêm outbox email nếu cần độ tin cậy cao.

## 13. Thứ tự ưu tiên sửa lỗi

### Ưu tiên 1 – phải sửa ngay

- Sửa controller không set `Success` vô điều kiện ở resend.
- Chuẩn hóa `AccountActivation:AppBaseUrl` đúng absolute URL có scheme.
- Cải thiện thông báo và thao tác retry khi SMTP lỗi sau khi token mới đã commit.
- Rà soát lại policy resend để tránh chặn retry ngay sau lỗi kỹ thuật.

### Ưu tiên 2 – nên sửa trước khi demo

- Bắt riêng `SmtpException`/`DbUpdateException` để thông báo và log chuẩn hơn.
- Thêm validate cấu hình SMTP/AppBaseUrl ở startup.
- Kiểm thử LAN thật với profile chạy đúng và firewall.
- Kiểm thử race condition cho resend/activate đồng thời.

### Ưu tiên 3 – cải tiến sau

- Áp dụng outbox pattern cho email activation.
- Retry có kiểm soát, tránh gửi trùng.
- Bổ sung audit log gửi activation theo `MaNguoiDung`.
- Tăng idempotency cho resend.

## 14. Kết luận

- Luồng kích hoạt qua email đã được triển khai đầy đủ ở mức chức năng chính: tạo account chưa kích hoạt, tạo token hash, gửi email, kích hoạt và đặt mật khẩu, chặn login khi chưa kích hoạt.
- Có 2 lỗi trọng tâm cần xử lý sớm: sai thông điệp UI resend và rủi ro consistency khi commit token mới trước SMTP fail.
- Cấu hình `AppBaseUrl` hiện tại trong `appsettings.json` chưa hợp lệ (thiếu scheme) là nguyên nhân trực tiếp gây lỗi tạo link.
- Kiến trúc hiện chưa có outbox/retry mạnh cho email; cần chấp nhận hạn chế này hoặc nâng cấp ở bước tiếp theo.

# TRẠNG THÁI SAU KHI KHẮC PHỤC LỖI EMAIL KÍCH HOẠT

## 1) Kết quả đã khắc phục

1. Controller resend không còn hiển thị đồng thời success và warning.
2. `AccountActivation:AppBaseUrl` được dùng từ cấu hình ứng dụng (`appsettings.json` / `appsettings.Development.json`), không hard-code trong C#.
3. Gmail credentials (`SenderEmail`, `Username`, `AppPassword`) vẫn lấy từ User Secrets hoặc biến môi trường.
4. `AccountActivation:AppBaseUrl` được validate bắt buộc là URL tuyệt đối HTTP/HTTPS và không cho `localhost`/loopback.
5. Luồng tạo activation URL không còn fallback sang `Request.Host` hoặc `localhost`.
6. SMTP lỗi đã được phân loại rõ hơn theo nhóm xác thực, kết nối, timeout, recipient, lỗi SMTP chung.
7. Khi resend thất bại lúc gửi email, token mới được hoàn tác; token cũ còn hạn sẽ được khôi phục.
8. Cooldown không bị áp sai bởi token của lần gửi SMTP thất bại (do đã hoàn tác token mới).
9. Race condition resend được giảm bằng transaction `Serializable` và bắt lỗi `DbUpdateException`/`DbUpdateConcurrencyException`.

## 2) Chi tiết từng điểm sửa

### Controller không còn 2 thông báo trái nghĩa

- File: `QuanLyDuAn/QuanLyDuAn/Controllers/NhanSuController.cs`
- Action `GuiLaiEmailKichHoat` đã đổi sang:
  - Chỉ set `TempData["Success"]` khi service trả `null` hoặc chuỗi rỗng.
  - Nếu có chuỗi cảnh báo thì set `TempData["Warning"]` và không set success.

### Chuẩn hóa AppBaseUrl và validate lúc startup

- File: `QuanLyDuAn/QuanLyDuAn/Services/AccountActivationOptions.cs`
  - `AppBaseUrl` mặc định `string.Empty`, không nullable.
- File: `QuanLyDuAn/QuanLyDuAn/Program.cs`
  - Validate `TokenLifetimeHours > 0`.
  - Validate `ResendCooldownSeconds > 0`.
  - Validate `AppBaseUrl` là absolute HTTP/HTTPS và không phải localhost/loopback.
  - Dùng `ValidateOnStart()` để fail-fast khi cấu hình sai.
- File: `QuanLyDuAn/QuanLyDuAn/appsettings.json`
  - `AccountActivation:AppBaseUrl` dùng dạng chuẩn: `http://192.168.2.27:5037`.
- File: `QuanLyDuAn/QuanLyDuAn/appsettings.Development.json`
  - Đồng bộ `AppBaseUrl` dạng absolute để tránh ghi đè rỗng trong môi trường Development.

### Không còn fallback tạo link localhost

- File: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanSuService.cs`
- Method `TaoActivationUrl(...)`:
  - Lấy path từ `LinkGenerator.GetPathByAction`.
  - Lấy base URL từ `_activationOptions.AppBaseUrl`.
  - Throw lỗi cấu hình nếu rỗng/sai định dạng/scheme không hợp lệ.
  - Throw lỗi nếu host là localhost/loopback.
  - Ghép URL bằng `new Uri(base, path)` an toàn.

### Phân loại SMTP lỗi rõ hơn và không lộ secret

- File: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/GmailEmailService.cs`
- Đã chuẩn hóa input:
  - `senderEmail`, `username` được trim.
  - `appPassword` được bỏ khoảng trắng và trim (hỗ trợ trường hợp lưu kiểu `abcd efgh ...`).
- Đã validate trước khi gửi:
  - `SmtpServer`, `Port`, `SenderEmail`, `Username`, `AppPassword`, email người nhận, `subject`, `body`, `activationUrl`.
- Cấu hình SMTP dùng chuẩn Gmail:
  - `smtp.gmail.com`, port `587`, `EnableSsl = true`, `UseDefaultCredentials = false`, `NetworkCredential(username, appPassword)`.
- Có bắt riêng `SmtpException` để ghi log theo `StatusCode` + operation + loại exception.
- Không log app password, token raw, full activation URL chứa token.

### Token resend khi SMTP fail

- File: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanSuService.cs`
- Luồng resend hiện tại:
  1. Đọc token cũ + kiểm cooldown.
  2. Tạo token mới.
  3. Ghi DB token mới trong transaction ngắn (`Serializable`), không gửi SMTP trong transaction.
  4. Gửi email.
  5. Nếu SMTP fail:
     - Mở transaction bù.
     - Xóa token mới vừa tạo.
     - Khôi phục token cũ còn hạn (nếu có snapshot hợp lệ).
     - Commit transaction bù.
  6. Trả warning đúng ngữ cảnh.

### Cooldown sau SMTP failure

- Vì token mới được hoàn tác khi gửi mail lỗi, lần resend tiếp theo không bị block bởi cooldown của token lỗi đó.
- Admin có thể gửi lại ngay sau lỗi SMTP (nếu token cũ đã khôi phục hoặc hệ thống không còn token resend lỗi).

### Giảm race condition resend

- Dùng transaction `IsolationLevel.Serializable` cho đoạn thao tác DB resend.
- Re-check cooldown trong transaction trước khi thay token.
- Bắt riêng `DbUpdateException` và `DbUpdateConcurrencyException`:
  - Trả thông báo: `Yêu cầu gửi lại email đang được xử lý. Vui lòng thử lại sau.`

## 3) Lưu ý cấu hình bắt buộc

- Cấu hình Gmail credentials đặt trong User Secrets:
  - `EmailSettings:SenderEmail`
  - `EmailSettings:Username`
  - `EmailSettings:AppPassword`
- Không đưa các giá trị thật này vào source.

- Cấu hình base URL hệ thống đặt trong `appsettings.json`:
  - `AccountActivation:AppBaseUrl` ví dụ: `http://192.168.x.x:5037`
- Không chuyển `AccountActivation:AppBaseUrl` sang User Secrets.

## 4) Cảnh báo về User Secrets override

Nếu trước đây từng cấu hình:

```powershell
dotnet user-secrets set "AccountActivation:AppBaseUrl" "https://localhost:7298"
```

thì giá trị User Secrets sẽ ghi đè `appsettings.json`, làm link quay về localhost.

Khi gặp tình huống này, developer tự chạy:

```powershell
dotnet user-secrets remove "AccountActivation:AppBaseUrl"
```

Ghi chú: tài liệu chỉ hướng dẫn thao tác, không tự động chạy lệnh.

## 5) File đã sửa trong đợt fix này

- `QuanLyDuAn/QuanLyDuAn/Controllers/NhanSuController.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/AccountActivationOptions.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanSuService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/GmailEmailService.cs`
- `QuanLyDuAn/QuanLyDuAn/Program.cs`
- `QuanLyDuAn/QuanLyDuAn/appsettings.json`
- `QuanLyDuAn/QuanLyDuAn/appsettings.Development.json`
- `docs/nhansu.md`

## 6) Build result

- Đã chạy `dotnet build` sau khi sửa.
- Kết quả được tổng hợp ở báo cáo cuối cùng của phiên làm việc.

## 7) Hạn chế còn lại

1. Chưa triển khai outbox/queue email (theo ràng buộc không đổi schema).
2. Nếu gửi email thành công nhưng transaction bù/DB gặp sự cố hiếm, cần admin theo dõi log để xử lý thủ công.
3. Chưa có trạng thái gửi email riêng trong DB (theo ràng buộc không thêm cột/bảng), nên chủ yếu dựa vào xử lý bù và log ứng dụng.

# RÀ SOÁT TRUY CẬP LINK KÍCH HOẠT TRÊN THIẾT BỊ KHÁC TRONG MẠNG LAN

## 1. Hiện tượng thực tế

- Truy cập `http://192.168.2.27:5037` trên máy tính host có thể hoạt động.
- Truy cập cùng địa chỉ từ điện thoại trong LAN có thể gặp màn hình trắng hoặc tải liên tục.
- Cần phân biệt rõ: lỗi truy cập host/cổng, lỗi giao thức HTTP-HTTPS, lỗi redirect, lỗi action `Account/Activate`, lỗi tài nguyên client, hoặc lỗi môi trường mạng.
- Kết luận trong mục này chỉ dựa trên source/cấu hình đã đọc, không thay đổi source, không thay đổi database/migration.

## 2. Cấu hình hosting hiện tại

- `Properties/launchSettings.json` có 3 profile:
  - `http` (`Project`): `applicationUrl = "http://192.168.2.27:5037"`.
  - `https` (`Project`): `applicationUrl = "https://localhost:7298;http://192.168.2.27:5037"`.
  - `IIS Express`: dùng `iisSettings.iisExpress.applicationUrl = "http://localhost:11893"` và `sslPort = 44302`.
- `Program.cs` middleware thực tế theo thứ tự:
  1. `UseExceptionHandler("/Home/Error")` + `UseHsts()` chỉ khi `!app.Environment.IsDevelopment()`.
  2. `UseHttpsRedirection()`.
  3. `UseStaticFiles()`.
  4. `UseRouting()`.
  5. `UseAuthentication()`.
  6. `UseAuthorization()`.
  7. `MapControllerRoute(...)`.
- Có global authorization filter trong `AddControllersWithViews(...)`; controller `AccountController` có `[AllowAnonymous]` ở cấp class.

## 3. Ý nghĩa các endpoint HTTP và HTTPS

- `https://localhost:7298`: endpoint HTTPS cục bộ trên host (chỉ bind localhost theo `launchSettings` profile `https`).
- `http://192.168.2.27:5037`: endpoint HTTP LAN (bind theo IP LAN khi chạy profile phù hợp).
- `https://192.168.2.27:5037`: không được cấu hình trong `applicationUrl`; gửi HTTPS vào cổng chỉ khai báo HTTP sẽ gây lỗi SSL protocol.
- `https://192.168.2.27:7298`: không được đảm bảo hoạt động vì HTTPS hiện bind vào `localhost:7298`, không phải IP LAN.
- Log kiểu:
  - `Now listening on: https://localhost:7298`
  - `Now listening on: http://192.168.2.27:5037`
  chỉ chứng minh đúng 2 endpoint trên, không chứng minh endpoint `https://192.168.2.27:5037` hoặc `https://192.168.2.27:7298`.

## 4. Phân tích lỗi ERR_CONNECTION_REFUSED

### Không có process lắng nghe đúng IP/cổng/giao thức

- Mức độ: Cao
- Trạng thái: Có nguy cơ
- File hoặc cấu hình liên quan: `Properties/launchSettings.json`, profile đang chạy thực tế
- Bằng chứng: Chỉ profile `Project` mới khai báo endpoint Kestrel theo `applicationUrl`; `IIS Express` khai báo localhost.
- Nguyên nhân: Chạy sai profile, process cũ giữ cổng, hoặc endpoint bind không khớp IP LAN hiện tại.
- Tác động: Điện thoại báo `ERR_CONNECTION_REFUSED` dù host có thể mở trang khác.
- Cách kiểm tra: `netstat -ano | findstr :5037`, kiểm tra log `Now listening on`.
- Hướng xử lý đề xuất: Chạy đúng profile `Project`, xác nhận endpoint listen đúng IP LAN/cổng.
- Có cần sửa source không: Không
- Có cần thay đổi database không: Không

### IP LAN thay đổi sau khi đổi mạng Wi-Fi

- Mức độ: Trung bình
- Trạng thái: Có nguy cơ
- File hoặc cấu hình liên quan: `Properties/launchSettings.json`, `appsettings.json`, `appsettings.Development.json`
- Bằng chứng: Các cấu hình đang hard-code `192.168.2.27`.
- Nguyên nhân: DHCP cấp IP mới; endpoint/profile và `AppBaseUrl` không còn khớp.
- Tác động: Link email cũ trỏ sai host; thiết bị khác không truy cập được.
- Cách kiểm tra: `ipconfig`, đối chiếu IP thật với cấu hình.
- Hướng xử lý đề xuất: Cập nhật IP thật, restart app, gửi lại email mới.
- Có cần sửa source không: Không bắt buộc (chỉ cập nhật config theo môi trường)
- Có cần thay đổi database không: Không

## 5. Phân tích lỗi ERR_SSL_PROTOCOL_ERROR

### Gửi HTTPS vào cổng chỉ phục vụ HTTP

- Mức độ: Trung bình
- Trạng thái: Đã xác nhận
- File hoặc cấu hình liên quan: `Properties/launchSettings.json`
- Bằng chứng: `http://192.168.2.27:5037` là endpoint HTTP; không có cấu hình `https://192.168.2.27:5037`.
- Nguyên nhân: Trình duyệt mở `https://192.168.2.27:5037`.
- Tác động: Trình duyệt báo `ERR_SSL_PROTOCOL_ERROR`.
- Cách kiểm tra: Mở đúng `http://192.168.2.27:5037` và so sánh với `https://192.168.2.27:5037`.
- Hướng xử lý đề xuất: Dùng đúng giao thức HTTP cho endpoint 5037, hoặc cấu hình HTTPS LAN đúng chuẩn certificate/IP.
- Có cần sửa source không: Không bắt buộc
- Có cần thay đổi database không: Không

### Truy cập HTTPS IP LAN khi chỉ bind HTTPS localhost

- Mức độ: Trung bình
- Trạng thái: Đã xác nhận
- File hoặc cấu hình liên quan: `Properties/launchSettings.json` profile `https`
- Bằng chứng: HTTPS khai báo `https://localhost:7298`; không có HTTPS bind `192.168.2.27`.
- Nguyên nhân: Thiết bị ngoài host gọi `https://192.168.2.27:7298`.
- Tác động: Có thể `ERR_CONNECTION_REFUSED` hoặc lỗi TLS tùy trạng thái listen/chứng chỉ.
- Cách kiểm tra: Đối chiếu log `Now listening on`, thử URL HTTPS LAN.
- Hướng xử lý đề xuất: Nếu test LAN, ưu tiên HTTP LAN hoặc cấu hình HTTPS bind IP LAN + certificate phù hợp.
- Có cần sửa source không: Không bắt buộc
- Có cần thay đổi database không: Không

## 6. Nguy cơ HTTPS redirection

### `UseHttpsRedirection()` có thể chuyển hướng request HTTP LAN

- Mức độ: Cao
- Trạng thái: Có nguy cơ
- File hoặc cấu hình liên quan: `Program.cs`
- Bằng chứng: `app.UseHttpsRedirection();` được bật không điều kiện môi trường.
- Nguyên nhân: Request `http://192.168.2.27:5037` có thể bị redirect sang endpoint HTTPS.
- Tác động: Nếu redirect sang HTTPS không truy cập được trên điện thoại thì trang sẽ tải mãi/trắng hoặc lỗi.
- Cách kiểm tra: `curl.exe -v http://192.168.2.27:5037` và xem có `HTTP/1.1 30x` + `Location: https://...` không.
- Hướng xử lý đề xuất: Trong giai đoạn test LAN Development, cân nhắc cấu hình không cưỡng bức HTTPS hoặc bảo đảm HTTPS LAN hoạt động đầy đủ trước khi redirect.
- Có cần sửa source không: Chưa xác định
- Có cần thay đổi database không: Không

Ghi chú bằng chứng:
- Trạng thái hiện tại chỉ xác nhận middleware có tồn tại.
- Chưa có log/response thực nghiệm trong tài liệu để kết luận redirect đã thực sự xảy ra.

## 7. Phân tích màn hình trắng và tải liên tục

### Redirect sang HTTPS endpoint không sẵn sàng trên LAN

- Mức độ: Cao
- Trạng thái: Có nguy cơ
- File hoặc cấu hình liên quan: `Program.cs`, `Properties/launchSettings.json`
- Bằng chứng: Có `UseHttpsRedirection`; HTTPS profile chỉ bind `localhost:7298`.
- Nguyên nhân: Điện thoại đi theo redirect tới HTTPS không truy cập được hoặc cert không tin cậy.
- Tác động: Trình duyệt thể hiện loading liên tục/màn hình trắng.
- Cách kiểm tra: Bắt response đầu bằng `curl.exe -v` và theo dõi chain redirect.
- Hướng xử lý đề xuất: Tách rõ mode test LAN HTTP và mode HTTPS chuẩn hóa certificate.
- Có cần sửa source không: Chưa xác định
- Có cần thay đổi database không: Không

### Action trả HTML nhưng resource client lỗi tải

- Mức độ: Trung bình
- Trạng thái: Chưa đủ bằng chứng
- File hoặc cấu hình liên quan: `Views/Account/Activate.cshtml`, `Views/Shared/_Layout.cshtml`
- Bằng chứng: Các resource chủ yếu dùng `~/...` hoặc CDN (`fonts.googleapis.com`, `cdn.jsdelivr.net`), không thấy hard-code `localhost`.
- Nguyên nhân: Có thể lỗi mạng ngoài (CDN bị chặn), JS runtime, hoặc asset pipeline; chưa thấy bằng chứng lỗi cụ thể.
- Tác động: Giao diện hiển thị trắng/thiếu style/script.
- Cách kiểm tra: DevTools network/console trên thiết bị, kiểm tra status tải CSS/JS.
- Hướng xử lý đề xuất: Ưu tiên kiểm tra network request thực tế trước khi kết luận lỗi view/js.
- Có cần sửa source không: Chưa xác định
- Có cần thay đổi database không: Không

## 8. Kiểm tra Account/Activate

### Endpoint `Account/Activate` cho phép anonymous đúng thiết kế

- Mức độ: Trung bình
- Trạng thái: Đã xác nhận
- File hoặc cấu hình liên quan: `Controllers/AccountController.cs`, `Program.cs`
- Bằng chứng: `AccountController` có `[AllowAnonymous]` ở cấp class; global auth filter yêu cầu đăng nhập cho các controller khác.
- Nguyên nhân: Thiết kế cho phép người chưa đăng nhập mở link kích hoạt.
- Tác động: Không bị chặn bởi authorization toàn cục.
- Cách kiểm tra: Truy cập `/Account/Activate?userId=[REDACTED]&token=[REDACTED]` khi chưa login.
- Hướng xử lý đề xuất: Giữ nguyên.
- Có cần sửa source không: Không
- Có cần thay đổi database không: Không

### Không thấy `.Result`/`.Wait()` gây treo đồng bộ

- Mức độ: Thấp
- Trạng thái: Đã xác nhận
- File hoặc cấu hình liên quan: `Controllers/AccountController.cs`, `Services/Implementations/NhanSuService.cs`
- Bằng chứng: Các luồng chính dùng `await` async.
- Nguyên nhân: Không có bằng chứng deadlock kiểu sync-over-async trong các file đã đọc.
- Tác động: Giảm khả năng treo do chặn thread đồng bộ.
- Cách kiểm tra: Đọc source action/service liên quan.
- Hướng xử lý đề xuất: Duy trì async end-to-end.
- Có cần sửa source không: Không
- Có cần thay đổi database không: Không

### Redirect vòng lặp trong Activate

- Mức độ: Trung bình
- Trạng thái: Chưa đủ bằng chứng
- File hoặc cấu hình liên quan: `Controllers/AccountController.cs`
- Bằng chứng: Action GET `Activate` có thể redirect về `Login` khi token/userId thiếu hoặc lỗi service; không thấy vòng lặp trực tiếp trong code này.
- Nguyên nhân: Chỉ có thể kết luận khi có trace request chain thực tế.
- Tác động: Có thể gây cảm giác tải lại liên tục nếu cấu hình bên ngoài can thiệp.
- Cách kiểm tra: Ghi log request bắt đầu/kết thúc, kiểm tra Location chain.
- Hướng xử lý đề xuất: Thu thập log middleware/request trước khi sửa.
- Có cần sửa source không: Chưa xác định
- Có cần thay đổi database không: Không

## 9. Kiểm tra firewall và mạng LAN

### Thiết bị không cùng điều kiện mạng hoặc bị chặn cổng

- Mức độ: Cao
- Trạng thái: Có nguy cơ
- File hoặc cấu hình liên quan: Yếu tố môi trường ngoài source
- Bằng chứng: Không thể xác minh từ source; đây là nhóm nguyên nhân hạ tầng phổ biến khi host truy cập được nhưng thiết bị khác không truy cập được.
- Nguyên nhân: Firewall Windows/antivirus chặn TCP 5037, profile mạng Public/Private không khớp rule, phone dùng 4G/5G, VPN/proxy, guest Wi-Fi, AP/client isolation.
- Tác động: Điện thoại không kết nối được tới host dù URL đúng.
- Cách kiểm tra: `Test-NetConnection`, rule firewall, subnet/IP thiết bị, tắt dữ liệu di động và VPN.
- Hướng xử lý đề xuất: Kiểm tra tuần tự hạ tầng trước khi quy lỗi nghiệp vụ.
- Có cần sửa source không: Không
- Có cần thay đổi database không: Không

## 10. Quy trình chẩn đoán từng bước

1. Chạy project bằng profile `Project` (không phải `IIS Express`).
2. Đọc log `Now listening on`.
3. Trên host mở `http://localhost:5037` (nếu profile có bind localhost) hoặc URL host tương ứng.
4. Trên host mở `http://192.168.2.27:5037`.
5. Trên host chạy `curl.exe -v http://192.168.2.27:5037`.
6. Kiểm tra có redirect HTTPS (`Location: https://...`) hay không.
7. Chạy `netstat -ano | findstr :5037` để xác nhận cổng listen.
8. Kiểm tra firewall Windows và phần mềm bảo mật.
9. Xác nhận điện thoại và host cùng Wi-Fi/subnet.
10. Tắt 4G/5G và VPN trên điện thoại.
11. Mở trang chủ từ điện thoại bằng HTTP LAN.
12. Nếu trang chủ mở được, mở `/Account/Activate`.
13. Nếu chỉ `Activate` lỗi, kiểm tra action/service/database query.
14. Xem log request bắt đầu/kết thúc cho route `Account/Activate`.
15. Xác nhận `[AllowAnonymous]` không bị cấu hình khác ghi đè.
16. Kiểm tra redirect loop nếu có nhiều 30x liên tiếp.
17. Kiểm tra resource View/JS/CSS có lỗi tải.
18. Sau mọi thay đổi cấu hình: restart ứng dụng và gửi lại email kích hoạt mới.

Lệnh kiểm tra tham chiếu:

```powershell
ipconfig
netstat -ano | findstr :5037
curl.exe -v http://192.168.2.27:5037
Test-NetConnection 192.168.2.27 -Port 5037
```

Hướng dẫn mở firewall (không tự chạy ở bước này):

```powershell
netsh advfirewall firewall add rule name="QuanLyDuAnAI HTTP 5037" dir=in action=allow protocol=TCP localport=5037 profile=private
```

## 11. Cấu hình khuyến nghị khi test nội bộ

- Khuyến nghị profile LAN Development:

```json
"http": {
  "commandName": "Project",
  "dotnetRunMessages": true,
  "launchBrowser": true,
  "applicationUrl": "http://0.0.0.0:5037",
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}
```

- Hoặc bind theo IP cụ thể:

```json
"applicationUrl": "http://192.168.2.27:5037"
```

So sánh:
- `0.0.0.0:5037`
  - Ưu điểm: lắng nghe trên mọi network interface, ít phải đổi cấu hình khi đổi adapter.
  - Nhược điểm: link gửi email không thể dùng `0.0.0.0`; cần biết IP thực để chia sẻ.
- `192.168.2.27:5037`
  - Ưu điểm: rõ ràng host LAN cần truy cập.
  - Nhược điểm: đổi Wi-Fi/IP là phải cập nhật lại.

`AccountActivation:AppBaseUrl` nên giữ URL thực của host:

```json
"AccountActivation": {
  "TokenLifetimeHours": 24,
  "ResendCooldownSeconds": 60,
  "AppBaseUrl": "http://192.168.2.27:5037"
}
```

Lưu ý:
- `0.0.0.0` chỉ dùng để bind server, không dùng trong link email.
- Link email phải là IP/hostname thực mà thiết bị khác truy cập được.
- Khi IP đổi: cập nhật `AppBaseUrl`, restart app, gửi email mới.
- Không ghi token thật vào tài liệu; dùng dạng:
  - `http://192.168.x.x:5037/Account/Activate?userId=[REDACTED]&token=[REDACTED]`

## 12. Các vấn đề đã xác nhận

### Ý nghĩa endpoint từ cấu hình profile hiện tại

- Mức độ: Cao
- Trạng thái: Đã xác nhận
- File hoặc cấu hình liên quan: `Properties/launchSettings.json`
- Bằng chứng: Profile `https` khai báo `https://localhost:7298;http://192.168.2.27:5037`; profile `http` khai báo `http://192.168.2.27:5037`; `IIS Express` dùng localhost.
- Nguyên nhân: Cách bind endpoint theo từng profile.
- Tác động: URL dùng trên điện thoại phụ thuộc profile chạy thực tế.
- Cách kiểm tra: Xem profile start + log `Now listening on`.
- Hướng xử lý đề xuất: Chuẩn hóa profile khi test LAN.
- Có cần sửa source không: Không
- Có cần thay đổi database không: Không

### Middleware `UseHttpsRedirection` đang bật trong Development

- Mức độ: Trung bình
- Trạng thái: Đã xác nhận
- File hoặc cấu hình liên quan: `Program.cs`
- Bằng chứng: `app.UseHttpsRedirection();` được gọi ngoài khối `if (!app.Environment.IsDevelopment())`.
- Nguyên nhân: Thiết kế middleware pipeline hiện tại.
- Tác động: Có thể ảnh hưởng request HTTP LAN nếu endpoint HTTPS không dùng được trên điện thoại.
- Cách kiểm tra: Kiểm tra response 30x với `curl.exe -v`.
- Hướng xử lý đề xuất: Đánh giá theo mục tiêu test LAN trước khi chỉnh.
- Có cần sửa source không: Chưa xác định
- Có cần thay đổi database không: Không

### `Account/Activate` không thiếu `[AllowAnonymous]`

- Mức độ: Trung bình
- Trạng thái: Đã xác nhận
- File hoặc cấu hình liên quan: `Controllers/AccountController.cs`
- Bằng chứng: `[AllowAnonymous]` áp dụng toàn bộ `AccountController`.
- Nguyên nhân: Thiết kế cho phép truy cập link kích hoạt trước đăng nhập.
- Tác động: Không bị chặn bởi auth policy toàn cục.
- Cách kiểm tra: Đọc annotation controller/action.
- Hướng xử lý đề xuất: Giữ nguyên.
- Có cần sửa source không: Không
- Có cần thay đổi database không: Không

### View Activate không hard-code `localhost` cho resource nội bộ

- Mức độ: Thấp
- Trạng thái: Đã xác nhận
- File hoặc cấu hình liên quan: `Views/Account/Activate.cshtml`, `Views/Shared/_Layout.cshtml`
- Bằng chứng: Dùng `~/...` và CDN; không thấy URL tuyệt đối `http://localhost...`.
- Nguyên nhân: Resource nội bộ resolve theo host hiện tại.
- Tác động: Giảm khả năng trắng trang do hard-code localhost.
- Cách kiểm tra: Đọc markup view/layout.
- Hướng xử lý đề xuất: Tiếp tục tránh URL tuyệt đối localhost trong view/script.
- Có cần sửa source không: Không
- Có cần thay đổi database không: Không

## 13. Các vấn đề cần kiểm tra thủ công

### Redirect HTTP sang HTTPS có thực sự xảy ra trên runtime hiện tại

- Mức độ: Cao
- Trạng thái: Chưa đủ bằng chứng
- File hoặc cấu hình liên quan: `Program.cs`, runtime profile đang chạy
- Bằng chứng: Source cho thấy khả năng redirect; chưa có bản ghi response 302 thực tế trong tài liệu.
- Nguyên nhân: Cần chứng cứ runtime/log.
- Tác động: Nếu có redirect sai endpoint HTTPS thì điện thoại dễ lỗi tải vô hạn.
- Cách kiểm tra: `curl.exe -v http://192.168.2.27:5037` và xem `Location`.
- Hướng xử lý đề xuất: Chốt bằng chứng runtime trước khi kết luận nguyên nhân chính.
- Có cần sửa source không: Chưa xác định
- Có cần thay đổi database không: Không

### Firewall/rule mạng LAN/adapter có chặn truy cập từ điện thoại hay không

- Mức độ: Cao
- Trạng thái: Chưa đủ bằng chứng
- File hoặc cấu hình liên quan: Môi trường hệ điều hành/mạng
- Bằng chứng: Không thể suy ra từ source.
- Nguyên nhân: Rule firewall, network profile, AP isolation, VPN/4G, subnet khác.
- Tác động: Truy cập từ host được nhưng thiết bị khác thất bại.
- Cách kiểm tra: `Test-NetConnection`, cấu hình mạng Windows/router, kiểm tra Wi-Fi điện thoại.
- Hướng xử lý đề xuất: Kiểm tra hạ tầng theo checklist mục 10.
- Có cần sửa source không: Không
- Có cần thay đổi database không: Không

### Process cũ giữ cổng hoặc chạy nhầm profile

- Mức độ: Trung bình
- Trạng thái: Có nguy cơ
- File hoặc cấu hình liên quan: `Properties/launchSettings.json`, tiến trình runtime
- Bằng chứng: Có nhiều profile (`http`, `https`, `IIS Express`) dễ gây nhầm; chưa có netstat/log phiên chạy cụ thể trong tài liệu.
- Nguyên nhân: IDE start nhầm profile hoặc process cũ chưa tắt.
- Tác động: Endpoint thực tế khác endpoint kỳ vọng.
- Cách kiểm tra: Kiểm tra profile active + PID listen theo `netstat`.
- Hướng xử lý đề xuất: Dọn process cũ và chuẩn hóa profile chạy thử.
- Có cần sửa source không: Không
- Có cần thay đổi database không: Không

## 14. Kết luận

- Theo source/config hiện tại, endpoint LAN chính xác là HTTP `http://192.168.2.27:5037`; endpoint HTTPS hiện khai báo cho `localhost:7298`.
- Khác biệt URL cần ghi nhớ:
  - `https://localhost:7298`: dùng tại host.
  - `http://192.168.2.27:5037`: dùng cho LAN.
  - `https://192.168.2.27:5037`: sai giao thức đối với cổng HTTP.
  - `https://192.168.2.27:7298`: không được đảm bảo vì HTTPS không bind IP LAN trong cấu hình hiện tại.
- Đã xác nhận từ source:
  - Có `UseHttpsRedirection`.
  - `Account/Activate` có `[AllowAnonymous]`.
  - View `Activate` không dùng URL localhost hard-code cho tài nguyên nội bộ.
- Chưa đủ bằng chứng để kết luận nguyên nhân duy nhất của màn hình trắng là `UseHttpsRedirection`; cần kiểm chứng runtime theo checklist mục 10.
- Các yếu tố firewall/LAN/VPN/AP isolation là nguyên nhân môi trường thường gặp và không phải lỗi nghiệp vụ kích hoạt tài khoản.

# RÀ SOÁT EMAIL XUẤT HIỆN TRONG ĐÃ GỬI NHƯNG NGƯỜI NHẬN KHÔNG NHẬN ĐƯỢC

## 1. Hiện tượng thực tế

- Ứng dụng báo gửi email kích hoạt thành công (không có `Warning`/`Error` từ luồng tạo nhân sự hoặc gửi lại).
- Trong tài khoản Gmail người gửi, mục **Đã gửi** có bản sao email.
- Tài khoản người nhận **không thấy** email trong Hộp thư đến (Inbox).

Phân biệt các trạng thái trong chuỗi giao thư:

| Bước | Trạng thái | Ý nghĩa |
| ---- | ---------- | ------- |
| 1 | Ứng dụng gọi `SendMailAsync` không exception | Code SMTP hoàn tất bình thường |
| 2 | Gmail SMTP chấp nhận thư | Máy chủ gửi tiếp nhận lệnh gửi |
| 3 | Gmail lưu vào **Đã gửi** | Tài khoản gửi ghi nhận thao tác gửi thành công |
| 4 | Hệ thống mail đích chấp nhận thư | Mail server người nhận nhận thư từ Gmail |
| 5 | Thư vào Inbox | Người nhận thấy ở Hộp thư đến |
| 6 | Thư vào Spam/Quảng cáo/Tất cả thư | Đã giao nhưng không ở Inbox |
| 7 | Bounce/filter/quarantine | Giao thất bại hoặc bị chặn sau bước 3 |

**Hiện tượng quan sát được chứng minh ít nhất đến bước 3.** Không thể suy ra bước 4–5 chỉ vì có trong **Đã gửi**.

## 2. Phạm vi source đã đọc

### Controller
- `QuanLyDuAn/QuanLyDuAn/Controllers/NhanSuController.cs`
- `QuanLyDuAn/QuanLyDuAn/Controllers/AccountController.cs`

### Service và interface
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IEmailService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/GmailEmailService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/INhanSuService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanSuService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AccountService.cs` (luồng OTP, đối chiếu pattern gửi mail)
- `QuanLyDuAn/QuanLyDuAn/Services/AccountActivationOptions.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/AccountActivationTokenHelper.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/CauHinhDichVu.cs`

### Entity/DbContext
- `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/Aspnetusers.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/NguoiDung.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/Aspnetusertokens.cs`

### View
- `QuanLyDuAn/QuanLyDuAn/Views/NhanSu/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/NhanSu/Index.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml`

### Cấu hình
- `QuanLyDuAn/QuanLyDuAn/Program.cs`
- `QuanLyDuAn/QuanLyDuAn/appsettings.json`
- `QuanLyDuAn/QuanLyDuAn/appsettings.Development.json`
- `QuanLyDuAn/QuanLyDuAn/Properties/launchSettings.json`
- `QuanLyDuAn/QuanLyDuAn/QuanLyDuAn.csproj`

## 3. Luồng gửi email thực tế

### Tạo nhân sự mới

```text
NhanSuController.LuuNhanSu (POST)
  → NhanSuService.SaveAsync (nhánh create)
    → Transaction: NguoiDung + Aspnetusers + Aspnetuserroles + Aspnetusertokens
    → Commit DB
    → TaoActivationUrl(userId, token)
    → IEmailService.SendAccountActivationEmailAsync(email, hoTen, userName, activationUrl, lifetimeHours)
      → GmailEmailService.SendAsync(toEmail, subject, body)
        → SmtpClient.SendMailAsync(mail)  [await]
  → Controller: Success nếu warning == null; Warning nếu SMTP/URL lỗi
```

### Gửi lại email kích hoạt

```text
NhanSuController.GuiLaiEmailKichHoat (POST)
  → NhanSuService.GuiLaiEmailKichHoatAsync(id)
    → Join NguoiDung + Aspnetusers theo MaNguoiDung
    → Transaction Serializable: thu hồi token cũ, tạo token mới, Commit
    → TaoActivationUrl + SendAccountActivationEmailAsync(data.Email, ...)
    → Nếu SMTP fail: hoàn tác token mới, khôi phục token cũ còn hạn
  → Controller: Success chỉ khi service trả null; Warning nếu có chuỗi cảnh báo
```

## 4. Địa chỉ người nhận được lấy từ đâu

| Luồng | Nguồn dữ liệu | Trường truyền vào email service |
| ----- | ------------- | -------------------------------- |
| Tạo nhân sự | `model.Email.Trim()` → lưu `Aspnetusers.Email` | Biến cục bộ `email` sau trim |
| Gửi lại kích hoạt | Query join `NguoiDung` + `Aspnetusers` theo `MaNguoiDung == id` | `data.Email` từ `Aspnetusers.Email` |

Kiểm tra chi tiết:

| # | Kiểm tra | Kết quả từ source |
| - | -------- | ----------------- |
| 1 | Lấy đúng `Aspnetusers.Email` | Có — không lấy từ `NguoiDung` (bảng này không có cột email) |
| 2 | Lấy nhầm email người gửi | Không — `SenderEmail` chỉ dùng cho `mail.From` và SMTP auth |
| 3 | Lấy nhầm email admin đang đăng nhập | Không — không dùng `HttpContext.User` cho recipient |
| 4 | Truyền nhầm thứ tự tham số | Không — `SendAccountActivationEmailAsync(toEmail, recipientName, userName, activationUrl, lifetimeHours)` khớp interface |
| 5 | `NguoiDung` vs `Aspnetusers` không đồng bộ email | Không áp dụng — email chỉ ở `Aspnetusers`; join qua `nd.Id == tk.Id` |
| 6 | Khoảng trắng đầu/cuối | Có trim khi tạo (`model.Email.Trim()`); `GmailEmailService` trim lại `toEmail` trước gửi |
| 7 | Ký tự ẩn/xuống dòng | Chưa validate riêng; `[EmailAddress]` trên ViewModel giảm rủi ro nhập form |
| 8 | Normalize khác giá trị gửi | Không — gửi `email` đã trim, không gửi `NormalizedEmail` |
| 9 | Sai đuôi miền (@gmail.con, …) | Không phát hiện ở source nếu cú pháp hợp lệ; cần đối chiếu DB thủ công |
| 10 | Email cũ trong DB | Có thể nếu admin nhập sai lúc tạo; email readonly khi sửa, không tự sửa DB |
| 11 | Join sai lấy email nhân sự khác | Không — filter `where nd.MaNguoiDung == id` |

**Kết luận:** Source truyền recipient từ `Aspnetusers.Email` đúng thiết kế. Nếu người nhận không thấy thư, cần đối chiếu email trong DB với email thực tế người dùng đang kiểm tra.

## 5. Phân tích GmailEmailService

### Cấu hình MailMessage thực tế

| Thuộc tính | Giá trị | Ghi chú |
| ---------- | ------- | ------- |
| `From` | `new MailAddress(senderAddress.Address, senderName)` | `SenderEmail` từ config, đã trim |
| `To` | `mail.To.Add(recipientAddress)` | Một recipient, không dùng Bcc thay To |
| `CC` | Không set | — |
| `Bcc` | Không set | — |
| `Subject` | Tham số `subject` | Activation: `"Kích hoạt tài khoản Quản lý dự án AI"` |
| `Body` | Plain text | `IsBodyHtml = false` |
| `SubjectEncoding` / `BodyEncoding` | UTF-8 | — |

### SMTP client

- `SmtpServer`: `smtp.gmail.com` (fallback nếu config rỗng)
- `Port`: `587`
- `EnableSsl = true`
- `UseDefaultCredentials = false`
- `Credentials = new NetworkCredential(username, appPassword)`
- `AppPassword`: bỏ khoảng trắng và trim

### Kiểm tra lỗi thường gặp

| Lỗi tiềm ẩn | Trạng thái |
| ----------- | ---------- |
| Dùng Bcc thay To | Không |
| To rỗng | Bị chặn validate trước gửi |
| Gửi về SenderEmail | Không — trừ khi admin nhập trùng email người gửi |
| Gán nhầm Username vào recipient | Không |
| `message.To.Clear()` sau khi thêm | Không |
| `SendMailAsync` không await | Không — có `await smtp.SendMailAsync(mail)` |
| Exception bị nuốt | Không — `SmtpException` được log và rethrow `InvalidOperationException` |
| Fire-and-forget `_ = SendMailAsync` | Không |

## 6. Phân tích kết quả SMTP

### GmailEmailService → NhanSuService → Controller

**Tạo nhân sự:**

- `SendMailAsync` thành công → `SaveAsync` trả `null` → Controller set `TempData["Success"] = "Đã tạo nhân sự và gửi email kích hoạt đến địa chỉ đã đăng ký."`
- `SendMailAsync` hoặc tạo URL lỗi → catch trong `SaveAsync`, trả warning → Controller set `Success` = `"Đã lưu nhân sự"` + `Warning` mô tả lỗi gửi mail

**Gửi lại:**

- Thành công → service trả `null` → Controller chỉ set `Success`
- Thất bại → service trả chuỗi warning → Controller chỉ set `Warning`, không set `Success`

**Không hiển thị success khi `SendMailAsync` throw:** Đúng theo source hiện tại (sau các đợt fix trước).

### Phân loại lỗi SMTP

- `GmailEmailService` bắt riêng `SmtpException`, log `StatusCode`, map message thân thiện (auth, connection, recipient, timeout).
- `NhanSuService.GuiLaiEmailKichHoatAsync` có `catch (SmtpException)` nhưng `GmailEmailService` bọc lại thành `InvalidOperationException` → nhánh `SmtpException` thực tế không chạy; lỗi vẫn được xử lý qua `catch (Exception)`.

### Bounce sau khi SMTP chấp nhận

- Source **không** theo dõi bounce/Delivery Status Notification.
- Gmail có thể chấp nhận thư (bước 2–3) rồi mail server đích từ chối sau đó (bước 7) — ứng dụng không biết.

## 7. Ý nghĩa mục Đã gửi

### SMTP accepted không đồng nghĩa email đã vào Inbox

- Mức độ: Trung bình
- Trạng thái: Đã xác nhận về mặt luồng kỹ thuật
- File liên quan: `GmailEmailService.cs`, `NhanSuController.cs`
- Method liên quan: `SendAsync`, `LuuNhanSu`, `GuiLaiEmailKichHoat`
- Bằng chứng: `SendMailAsync` hoàn thành không exception; thư xuất hiện trong mục **Đã gửi** của Gmail gửi.
- Nguyên nhân: SMTP/`SendMailAsync` chỉ xác nhận máy chủ gửi (Gmail) đã tiếp nhận thư để relay; không xác nhận delivery tới Inbox người nhận.
- Tác động: Admin có thể hiểu nhầm “gửi thành công” = “người nhận chắc chắn đã nhận”.
- Cách tái hiện: Gửi activation → kiểm tra **Đã gửi** có thư → Inbox người nhận không có.
- Cách kiểm tra: Spam, Quảng cáo, Tất cả thư, bounce ở hộp gửi, đối chiếu recipient trong log (đã che).
- Hướng xử lý: Điều chỉnh copy UI (nếu cần): “Yêu cầu gửi email đã được máy chủ thư chấp nhận. Vui lòng kiểm tra Hộp thư đến, Spam hoặc Quảng cáo.”; dùng log recipient che bớt để đối chiếu.
- Có cần sửa source không: Có thể (chỉ message UI/logging; không đổi workflow)
- Có cần thay đổi database không: Không

**Mục Đã gửi chỉ chứng minh Gmail đã chấp nhận thao tác gửi từ tài khoản người gửi, chưa chứng minh thư đã vào Inbox của người nhận.**

## 8. Phân tích template email kích hoạt

Template trong `SendAccountActivationEmailAsync`:

- **Subject:** `Kích hoạt tài khoản Quản lý dự án AI` — hợp lý, không quá “spammy”.
- **Sender name:** `QuanLyDuAn AI` (config `SenderName`).
- **Body:** Plain text UTF-8, không HTML.
- **Nội dung:** Lời chào + tên đăng nhập + link activation + thời hạn + cảnh báo bỏ qua nếu không yêu cầu.
- **Không chứa:** mật khẩu, hash token, App Password.
- **Link mẫu (redacted):** `http://192.168.2.27:5037/Account/Activate?userId=[REDACTED]&token=[REDACTED]`

Đánh giá yếu tố spam:

| Yếu tố | Đánh giá |
| ------ | -------- |
| HTML lỗi | Không áp dụng (plain text) |
| Chữ viết hoa / dấu chấm than quá mức | Thấp |
| Từ ngữ giống phishing | Trung bình — có link đặt mật khẩu + token dài |
| Link HTTP IP private | Cao — xem mục 9 |
| Token dài trong query string | Trung bình |
| Text ẩn / HTML đáng ngờ | Không |

## 9. Nguy cơ link HTTP IP nội bộ bị lọc

### Link activation dùng HTTP + IP LAN private

- Mức độ: Trung bình
- Trạng thái: Có nguy cơ
- File liên quan: `NhanSuService.cs`, `appsettings.json`, `AccountActivationOptions.cs`
- Method liên quan: `TaoActivationUrl`, `SendAccountActivationEmailAsync`
- Bằng chứng: `AccountActivation:AppBaseUrl = "http://192.168.2.27:5037"`; link ghép path `/Account/Activate?userId=...&token=...`.
- Nguyên nhân: Email chứa URL không HTTPS, trỏ IP private RFC1918, có query token dài — pattern thường bị bộ lọc thư đánh dấu đáng ngờ.
- Tác động: SMTP vẫn thành công và có trong **Đã gửi**, nhưng thư có thể vào Spam/Quảng cáo hoặc bị filter phía người nhận.
- Cách tái hiện: Gửi activation → kiểm tra Spam/Quảng cáo/Tất cả thư người nhận.
- Cách kiểm tra: TC01–TC05, TC09; xem header nếu email vào Spam.
- Hướng xử lý: Khi triển khai thật dùng `https://ten-mien-that/Account/Activate`; giai đoạn dev LAN chấp nhận rủi ro hoặc hướng dẫn người nhận kiểm tra Spam.
- Có cần sửa source không: Không bắt buộc ở bước phân tích (không tự đổi sang localhost, không bỏ token)
- Có cần thay đổi database không: Không

## 10. Kiểm tra bounce và delivery failure

Source **không** đọc bounce webhook hay hộp thư đến của sender.

Cần kiểm tra thủ công ở tài khoản **người gửi**:

- Thư từ `Mail Delivery Subsystem`, `mailer-daemon@...`
- Tiêu đề: `Delivery Status Notification`, `Undelivered Mail Returned to Sender`

Mã lỗi tham chiếu (chỉ ghi nếu thấy trong bounce thật, không bịa):

- `550`, `551`, `552`, `553`, `554`
- `5.1.1`, `5.2.1`, `5.7.1`
- `Recipient address rejected`, `Mailbox unavailable`, `Message blocked`

**Trạng thái hiện tại:** Chưa có bounce thật trong phạm vi phân tích source — thuộc nhóm **Chưa đủ bằng chứng** cho đến khi kiểm tra hộp thư gửi.

## 11. Kiểm tra Spam, filter và blocked sender

Nguyên nhân ngoài source phổ biến nhất khi **Đã gửi** có thư nhưng Inbox trống:

1. **Spam / Thư rác**
2. **Quảng cáo / Promotions** (Gmail)
3. **Tất cả thư (All Mail)** — đã giao nhưng không nằm Inbox
4. **Filter/rule** tự chuyển hoặc xóa
5. **Blocked addresses** — chặn sender
6. **Forwarding** — thư chuyển sang hộp khác

Không thể xác nhận từ source; bắt buộc kiểm tra thủ công trên tài khoản **người nhận**.

## 12. Các lỗi source đã xác nhận

**Không phát hiện lỗi source trực tiếp** khiến email “gửi thành công về mặt SMTP” nhưng gán sai recipient, không await, hoặc nuốt exception rồi vẫn báo success.

Các điểm đã xác nhận **đúng**:

- Recipient từ `Aspnetusers.Email`, có trim ở tầng service/email.
- `mail.To` dùng đúng địa chỉ người nhận; `From` dùng `SenderEmail`.
- `await smtp.SendMailAsync(mail)` được gọi đúng.
- Exception SMTP được log và propagate; Controller không set success khi service trả warning (resend) hoặc trả warning kèm lưu nhân sự (create).

## 13. Các nguy cơ từ source

### Thông báo UI gợi ý đã gửi tới người nhận

- Mức độ: Trung bình
- Trạng thái: Có nguy cơ
- File liên quan: `NhanSuController.cs`
- Method liên quan: `LuuNhanSu`, `GuiLaiEmailKichHoat`
- Bằng chứng: `"Đã tạo nhân sự và gửi email kích hoạt đến địa chỉ đã đăng ký."` / `"Đã gửi lại email kích hoạt."` khi SMTP không lỗi.
- Nguyên nhân: Message mô tả hành động gửi qua SMTP, không phân biệt “accepted” vs “delivered to Inbox”.
- Tác động: Admin kỳ vọng người nhận thấy ngay trong Inbox.
- Hướng xử lý: Làm rõ “máy chủ thư đã chấp nhận”; hướng dẫn kiểm tra Spam (chỉ khi product owner đồng ý đổi copy).
- Có cần sửa source không: Có thể
- Có cần thay đổi database không: Không

### Link HTTP IP private trong body activation

- Xem mục 9.

### SenderEmail và Username không được validate khớp nhau

- Mức độ: Thấp
- Trạng thái: Có nguy cơ
- File liên quan: `GmailEmailService.cs`
- Bằng chứng: Hai field config độc lập; Gmail thường yêu cầu khớp nhưng source không ép.
- Tác động: Nếu lệch, có thể fail SMTP — **không** khớp hiện tượng “Đã gửi có thư”.
- Có cần sửa source không: Không bắt buộc cho case này
- Có cần thay đổi database không: Không

### Catch SmtpException ở NhanSuService không hiệu lực

- Mức độ: Thấp
- Trạng thái: Có nguy cơ (dead code)
- File liên quan: `NhanSuService.cs`
- Method liên quan: `GuiLaiEmailKichHoatAsync`
- Bằng chứng: `GmailEmailService` bọc `SmtpException` → `InvalidOperationException`.
- Tác động: Không ảnh hưởng hiện tượng delivery; lỗi vẫn vào `catch (Exception)`.
- Có cần sửa source không: Không bắt buộc ở bước này
- Có cần thay đổi database không: Không

## 14. Các nguyên nhân ngoài source

| Nguyên nhân | Khả năng với hiện tượng quan sát |
| ----------- | -------------------------------- |
| Email vào Spam/Quảng cáo/Tất cả thư | **Cao** |
| Filter/rule hoặc blocked sender ở người nhận | Trung bình |
| Địa chỉ recipient sai (typo lúc admin nhập) | Trung bình — cần đối chiếu DB vs email thật |
| Bounce sau khi Gmail chấp nhận | Trung bình — kiểm tra mailer-daemon ở sender |
| Mailbox đầy / tạm ngưng | Chưa đủ bằng chứng |
| Mail server đích trì hoãn (greylisting) | Chưa đủ bằng chứng |
| Gmail delivery policy / reputation sender mới | Có nguy cơ |
| Firewall/AP — **không** liên quan trực tiếp vì SMTP đã gửi và có **Đã gửi** | Loại trừ cho bước gửi |

## 15. Kịch bản kiểm thử

| Mã | Mô tả | Kết quả mong đợi | Trạng thái thực hiện |
| -- | ----- | ---------------- | -------------------- |
| TC01 | Gửi email text đơn giản thủ công từ Gmail web tới đúng recipient | Nếu không nhận → lỗi ngoài app | **Chưa test thủ công** |
| TC02 | App gửi email plain không link (OTP pattern hoặc body test) | So sánh với activation | **Chưa test thủ công** |
| TC03 | Gửi tới Gmail khác | Kiểm tra cross-Gmail delivery | **Chưa test thủ công** |
| TC04 | Gửi tới Outlook/Hotmail | Kiểm tra hệ thống nhận khác | **Chưa test thủ công** |
| TC05 | Kiểm tra Spam, Quảng cáo, Tất cả thư, Thùng rác | Tìm thư activation | **Chưa test thủ công** |
| TC06 | Filter, blocked, forwarding ở người nhận | Loại trừ rule chặn | **Chưa test thủ công** |
| TC07 | Tìm bounce ở sender (Mail Delivery Subsystem) | Xác định reject sau accept | **Chưa test thủ công** |
| TC08 | Đối chiếu recipient trong log (MaskEmail) với DB/UI | Xác nhận đúng địa chỉ | **Có thể sau khi deploy logging** |
| TC09 | Phân tích activation với domain HTTPS production (chỉ phân tích) | Giảm spam score khi go-live | Phân tích — không triển khai |

## 16. Logging chẩn đoán

### Trước bước phân tích

- Log lỗi SMTP có `StatusCode` nhưng **không** log recipient che bớt khi bắt đầu/thành công.

### Đã bổ sung (an toàn)

File: `GmailEmailService.cs`

- Helper `MaskEmail` — che dạng `u***r@domain.com`.
- Trước gửi: log `Recipient`, `Sender`, `SmtpHost`, `Port` (không log token, URL, App Password, body).
- Sau `SendMailAsync` thành công: log `SMTP da chap nhan email` + recipient che.
- Khi lỗi: log thêm `Recipient` che + `StatusCode`.

**Không log:** App Password, token raw, full activation URL, HTML/body chứa token, User Secrets.

## 17. File cần sửa nếu có

| File | Thay đổi ở bước này | Ghi chú |
| ---- | ------------------- | ------- |
| `GmailEmailService.cs` | Đã thêm logging `MaskEmail` | Chẩn đoán recipient |
| `NhanSuController.cs` | Không sửa | Có thể điều chỉnh copy UI sau |
| `NhanSuService.cs` | Không sửa workflow | — |
| `docs/nhansu.md` | Đã cập nhật mục này | — |

Không sửa database, migration, workflow kích hoạt.

## 18. Kết luận

1. **Vị trí lỗi trong chuỗi:** Với bằng chứng “app success + **Đã gửi** có thư”, lỗi **không nằm** ở các bước `NhanSuController` → `NhanSuService` → `GmailEmailService` → SMTP accept (bước 1–3). Khả năng cao nằm ở **bước 4–7** (delivery, Spam, filter, bounce, sai địa chỉ thực tế) — **ngoài phạm vi xác nhận từ source**.

2. **Recipient:** Lấy từ `Aspnetusers.Email`, trim, truyền đúng vào `mail.To` — **không phát hiện gán nhầm từ code**.

3. **SMTP:** `await SendMailAsync` đúng; exception không bị nuốt im lặng.

4. **Template:** Plain text; link `http://192.168.x.x:5037/Account/Activate?...` có nguy cơ bị lọc spam — **có nguy cơ**, chưa xác nhận là nguyên nhân duy nhất.

5. **Hành động tiếp theo (vận hành):** TC01–TC08; kiểm Spam/Quảng cáo; kiểm bounce sender; đối chiếu email DB vs log `Recipient` đã che.

6. **Không kết luận** “đã giao tới Inbox” chỉ vì SMTP không báo lỗi hoặc có mục **Đã gửi**.

## 19. Rà soát bổ sung ngày 2026-06-17

- Đã đọc lại source thật theo phạm vi yêu cầu và đối chiếu lại với tài liệu này.
- Không phát hiện lỗi source mới trong chuỗi `NhanSuController` → `NhanSuService` → `GmailEmailService`:
  - Tạo nhân sự mới dùng `model.Email.Trim()` làm recipient.
  - Gửi lại email kích hoạt lấy recipient từ `Aspnetusers.Email` qua query theo `MaNguoiDung`.
  - `GmailEmailService` trim `toEmail`, validate bằng `MailAddress.TryCreate`, thêm vào `mail.To`, và `await smtp.SendMailAsync(mail)`.
  - Exception SMTP không bị nuốt im lặng; service/controller chỉ báo thành công khi tầng gửi không throw hoặc không trả warning.
- Logging an toàn trong `GmailEmailService` đã đủ cho chẩn đoán bước tiếp theo: log recipient/sender đã che, SMTP host, port và `SmtpException.StatusCode`; không log token raw, full activation URL, App Password hoặc body chứa token.
- Không sửa thêm source ở lần rà soát này vì chưa có bằng chứng source gửi sai recipient hoặc báo thành công khi SMTP throw.
- Trọng tâm kiểm tra tiếp theo là vận hành: đối chiếu recipient đã che trong log với email thật trong DB/UI, kiểm Spam/Quảng cáo/Tất cả thư, filter/blocked sender của người nhận, và bounce/Delivery Status Notification trong tài khoản Gmail gửi.
- Không thay đổi database, không tạo hoặc sửa migration.

# TRẠNG THÁI SAU KHI CẢI THIỆN CHẨN ĐOÁN EMAIL KHÔNG VÀO INBOX

> Lưu ý: Mục này ghi lại trạng thái tại thời điểm từng bổ sung chức năng email diagnostic. Trạng thái hiện hành sau khi đã xóa chức năng này được mô tả ở mục `TRẠNG THÁI SAU KHI XÓA CHỨC NĂNG GỬI EMAIL KIỂM TRA` ở cuối tài liệu.

## 1. File đã sửa

- `QuanLyDuAn/QuanLyDuAn/Controllers/NhanSuController.cs`
  - Đổi thông báo thành công để không khẳng định email đã vào Inbox.
  - Thêm action POST `GuiEmailKiemTra` cho Admin gửi email plain text không chứa link.
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IEmailService.cs`
  - Thêm `SendDiagnosticEmailAsync`.
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/GmailEmailService.cs`
  - Thêm email diagnostic plain text.
  - Rà lại template activation cho ngắn gọn, rõ nguồn gửi, không thêm HTML/tracking.
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanSuService.cs`
  - Chuẩn hóa xử lý exception theo hướng `GmailEmailService` log SMTP rồi throw message thân thiện; service xử lý chung và giữ nguyên hoàn tác token.
- `QuanLyDuAn/QuanLyDuAn/Views/NhanSu/Index.cshtml`
  - Thêm form `Gửi email kiểm tra` cho Admin.
- `docs/nhansu.md`
  - Cập nhật phần trạng thái này.

## 2. Nội dung thông báo UI mới

Thông báo tạo nhân sự mới khi SMTP không lỗi:

```text
Đã tạo nhân sự. Máy chủ thư đã chấp nhận yêu cầu gửi email kích hoạt đến địa chỉ đã đăng ký. Vui lòng kiểm tra Hộp thư đến, Spam, Quảng cáo hoặc Tất cả thư.
```

Thông báo gửi lại activation khi SMTP không lỗi:

```text
Máy chủ thư đã chấp nhận yêu cầu gửi lại email kích hoạt. Vui lòng kiểm tra Hộp thư đến, Spam, Quảng cáo hoặc Tất cả thư.
```

Thông báo email kiểm tra khi SMTP không lỗi:

```text
Máy chủ thư đã chấp nhận email kiểm tra đến [email đã che]. Vui lòng kiểm tra Hộp thư đến, Spam, Quảng cáo hoặc Tất cả thư.
```

Các thông báo này không dùng cụm “đã vào Inbox” hoặc “người nhận đã nhận được”.

## 3. Ý nghĩa SMTP accepted

`SendMailAsync` không throw và email xuất hiện trong mục **Đã gửi** chỉ chứng minh Gmail SMTP đã chấp nhận thao tác gửi từ tài khoản người gửi. Điều này không chứng minh thư đã vào Inbox người nhận.

Các bước sau vẫn nằm ngoài xác nhận trực tiếp của ứng dụng:

- Mail server người nhận chấp nhận thư.
- Gmail/Outlook đưa thư vào Inbox.
- Thư bị đưa vào Spam, Quảng cáo, Tất cả thư hoặc Thùng rác.
- Thư bị filter/rule, blocked sender, quarantine hoặc bounce sau khi Gmail đã chấp nhận.

## 4. Logging recipient đã che

`GmailEmailService` tiếp tục log an toàn:

- Recipient đã che.
- Sender đã che.
- SMTP host.
- Port.
- `SmtpException.StatusCode` khi lỗi.

Không log:

- App Password.
- Token raw.
- Full activation URL.
- Body email chứa token.
- Password người dùng.
- User Secrets.

## 5. Chế độ gửi email kiểm tra

Đã thêm action:

```text
POST /NhanSu/GuiEmailKiemTra
```

Ràng buộc:

- Chỉ người có quyền `NhanSu.Xem` và role `ADMIN`/`Admin` được dùng.
- Có `[ValidateAntiForgeryToken]`.
- Không lưu lịch sử gửi vào database.
- Không chứa token hoặc URL trong nội dung email.

Nội dung email:

```text
Tiêu đề: Kiểm tra gửi email từ hệ thống

Đây là email kiểm tra từ hệ thống Quản lý dự án AI.
Email này không chứa liên kết kích hoạt.
```

Mục tiêu kiểm thử:

- Nếu email plain text cũng không thấy ở Inbox/Spam/Tất cả thư: ưu tiên kiểm tra địa chỉ nhận, filter, blocked sender, bounce hoặc chính sách hệ thống mail nhận.
- Nếu email plain text nhận được nhưng email activation không thấy: tăng khả năng body/link activation bị filter.

## 6. Template activation sau rà soát

Email activation vẫn là plain text UTF-8, không HTML, không ảnh tracking, không link hiển thị khác URL thật.

Nội dung đã chỉnh theo hướng:

- Nói rõ tài khoản trên hệ thống Quản lý dự án AI đã được tạo.
- Hiển thị tên đăng nhập.
- Hướng dẫn mở link để đặt mật khẩu và kích hoạt.
- Nêu thời hạn link và dùng một lần.
- Nhắc bỏ qua nếu không yêu cầu tài khoản.

Không thay đổi:

- Route `Account/Activate`.
- Token trong query string.
- Thời hạn token.
- Workflow kích hoạt.

## 7. Xử lý exception sau chuẩn hóa

Chọn hướng A:

- `GmailEmailService` bắt `SmtpException`, log recipient đã che và `StatusCode`, rồi throw `InvalidOperationException` với message thân thiện.
- `NhanSuService.GuiLaiEmailKichHoatAsync` dùng `catch (Exception)` để xử lý lỗi gửi, giữ nguyên logic hoàn tác token mới/khôi phục token cũ còn hạn.
- Không để UI lộ thông tin kỹ thuật SMTP.

## 8. Kịch bản kiểm thử đã thực hiện

- Đã build solution bằng `dotnet build`.
- Đã rà source bằng `rg` cho các chuỗi gửi mail và thông báo liên quan.
- Chưa thực hiện được kiểm thử thực tế trên Gmail người nhận vì môi trường hiện tại không có quyền truy cập hộp thư và không chạy phiên Visual Studio tương tác.

## 9. Kết quả thực tế

- Build thành công.
- Chức năng diagnostic đã có đường gọi riêng, không cần database.
- Chưa xác nhận email plain text vào Inbox/Spam/Tất cả thư vì cần kiểm tra thủ công trên tài khoản nhận.
- Chưa xác nhận activation email vào Inbox/Spam/Tất cả thư sau chỉnh template vì cần gửi thực tế.
- Chưa xác nhận bounce vì cần kiểm tra thủ công trong Gmail người gửi.

## 10. Hạn chế còn lại

- SMTP accepted không đồng nghĩa delivered.
- Ứng dụng chưa theo dõi bounce tự động.
- Link HTTP private IP vẫn có nguy cơ bị lọc.
- Development LAN chỉ phù hợp demo nội bộ.
- Production nên dùng domain HTTPS thật.
- Nếu email trong `Aspnetusers.Email` bị nhập sai nhưng vẫn đúng cú pháp, ứng dụng không tự biết người dùng đang kiểm tra một hộp thư khác.

## 11. Khuyến nghị production

- Dùng domain HTTPS hợp lệ cho `AccountActivation:AppBaseUrl`.
- Cấu hình Gmail hoặc SMTP provider có uy tín gửi thư tốt hơn cho production.
- Kiểm tra SPF/DKIM/DMARC nếu chuyển sang domain riêng.
- Nếu cần tracking delivery thật, thiết kế outbox/bounce processing/webhook trong một hạng mục riêng có migration chính thức.

## 12. Kết luận

- Không phát hiện lỗi source mới cho thấy gửi sai recipient.
- Đã giảm hiểu nhầm ở UI: hệ thống chỉ nói máy chủ thư đã chấp nhận yêu cầu gửi.
- Đã có email diagnostic plain text để tách lỗi giao nhận chung khỏi nguy cơ template/link activation bị lọc.
- Không sửa database, không tạo hoặc sửa migration, không đổi token activation, không đổi workflow kích hoạt.

# TRẠNG THÁI SAU KHI XÓA CHỨC NĂNG GỬI EMAIL KIỂM TRA

## 1. Lý do xóa

Chức năng **Gửi email kiểm tra** chỉ được thêm để chẩn đoán delivery email trong giai đoạn rà soát. Sau khi xác nhận source không gửi sai recipient và SMTP Gmail đã chấp nhận thư, khu vực này không còn cần thiết trên màn hình Nhân sự và làm giao diện rối.

## 2. File đã sửa

- `QuanLyDuAn/QuanLyDuAn/Controllers/NhanSuController.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IEmailService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/GmailEmailService.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/NhanSu/Index.cshtml`
- `docs/nhansu.md`

Không sửa `NhanSuService`, `_Table.cshtml`, `_Filter.cshtml`, `_Layout.cshtml`, `Program.cs`, `appsettings.json`, `appsettings.Development.json` ngoài việc đọc và rà soát.

## 3. Thành phần giao diện đã xóa

Đã xóa khỏi `Views/NhanSu/Index.cshtml`:

- Ô nhập `Email kiểm tra`.
- Nút `Gửi email kiểm tra`.
- Form POST gọi action test.
- Biến điều kiện `canSendDiagnosticEmail` chỉ phục vụ form test.

Header danh sách Nhân sự trở lại theo cấu trúc:

```text
Danh sách nhân sự
Xuất file
Bảng nhân sự
```

## 4. Action/controller đã xóa

Đã xóa khỏi `NhanSuController`:

- Action POST `GuiEmailKiemTra`.
- Permission check chỉ phục vụ action test.
- TempData message chỉ phục vụ email test.
- Helper `MaskEmail` trong controller vì chỉ còn dùng cho action test.
- Dependency trực tiếp `IEmailService` trong `NhanSuController`.

Các action `LuuNhanSu`, `GuiLaiEmailKichHoat`, `XoaNhanSu`, `KhoaTaiKhoan`, `MoKhoaTaiKhoan`, `XuatFile` được giữ nguyên.

## 5. Interface/service đã xóa

Đã xóa khỏi `IEmailService`:

```csharp
Task SendDiagnosticEmailAsync(string toEmail);
```

Đã xóa implementation tương ứng khỏi `GmailEmailService`.

Các method còn giữ:

- `SendAsync`
- `SendAccountActivationEmailAsync`

Helper `MaskEmail` trong `GmailEmailService` vẫn được giữ vì logging an toàn vẫn dùng để che recipient/sender.

## 6. Luồng email kích hoạt được giữ nguyên

Workflow không đổi:

```text
Admin tạo nhân sự
→ Tạo tài khoản chờ kích hoạt
→ Tạo activation token
→ Gửi email kích hoạt
→ Người dùng mở link
→ Đặt mật khẩu
→ EmailConfirmed = true
→ Xóa token
→ Đăng nhập
```

Không thay đổi token activation, hash token, thời hạn token, cooldown resend, route `Account/Activate`, `[AllowAnonymous]`, trạng thái tài khoản hoặc permission.

## 7. Logging an toàn được giữ lại

`GmailEmailService` vẫn log:

- Recipient đã che.
- Sender đã che.
- SMTP host.
- Port.
- `SmtpException.StatusCode` khi lỗi.

Không log token raw, full activation URL, App Password, body email chứa token, password người dùng hoặc User Secrets.

## 8. Thông báo UI sau chỉnh sửa

Thông báo tạo nhân sự mới khi SMTP không lỗi vẫn là:

```text
Đã tạo nhân sự. Máy chủ thư đã chấp nhận yêu cầu gửi email kích hoạt đến địa chỉ đã đăng ký. Vui lòng kiểm tra Hộp thư đến, Spam, Quảng cáo hoặc Tất cả thư.
```

Thông báo gửi lại activation khi SMTP không lỗi vẫn là:

```text
Máy chủ thư đã chấp nhận yêu cầu gửi lại email kích hoạt. Vui lòng kiểm tra Hộp thư đến, Spam, Quảng cáo hoặc Tất cả thư.
```

Không dùng các câu “người nhận đã nhận được email”, “email đã vào Inbox” hoặc “email đã giao thành công”.

## 9. Kết quả build

Đã chạy build sau khi xóa chức năng test email:

```powershell
dotnet build QuanLyDuAn\QuanLyDuAn.sln --no-restore
```

Kết quả: build thành công. Còn warning cũ `CS1998` trong `FileTienDoCongViecService.cs`, không phát sinh từ thay đổi xóa email test.

## 10. Kết quả kiểm thử giao diện

Đã kiểm tra bằng source:

- Không còn form `GuiEmailKiemTra` trong `Views/NhanSu/Index.cshtml`.
- Không còn ô `Email kiểm tra`.
- Không còn nút `Gửi email kiểm tra`.
- Header danh sách còn tiêu đề và nút xuất file theo permission.
- `_Table.cshtml` vẫn giữ nút `Gửi kích hoạt` cho tài khoản chờ kích hoạt.
- `_Filter.cshtml` không bị thay đổi.

Chưa chạy trình duyệt tương tác trong lượt này, nên chưa xác nhận trực quan bằng screenshot.

## 11. Kết quả kiểm thử gửi kích hoạt

Đã kiểm tra bằng source:

- `NhanSuService.SaveAsync` vẫn gọi `SendAccountActivationEmailAsync` sau khi commit transaction tạo nhân sự/tài khoản/token.
- `NhanSuService.GuiLaiEmailKichHoatAsync` vẫn tạo token mới, gửi email, và hoàn tác/khôi phục token khi gửi lỗi.
- `GmailEmailService` vẫn dùng `mail.To.Add(recipientAddress)` và `await smtp.SendMailAsync(mail)`.
- Controller chỉ hiện success khi service không trả warning.

Chưa gửi email thực tế trong lượt này vì không chạy phiên ứng dụng và không truy cập Gmail người gửi/người nhận.

## 12. Hạn chế delivery còn lại

Gmail Sent chỉ chứng minh SMTP accepted, không chứng minh thư đã vào Inbox.

Nếu người nhận không thấy thư, vẫn cần kiểm tra thủ công:

- Spam.
- Quảng cáo.
- Tất cả thư.
- Filter/rule.
- Blocked sender.
- Forwarding.
- Mail Delivery Subsystem.
- `mailer-daemon`.
- Bounce code.

Không phát hiện lỗi source gửi sai recipient trong lần rà soát này.

## 13. Xác nhận không đổi database

- Không thêm bảng.
- Không thêm cột.
- Không tạo migration.
- Không sửa migration.
- Không sửa `ModelSnapshot`.
- Không chạy `Add-Migration`.
- Không chạy `Update-Database`.
- Không thêm outbox, delivery log hoặc trạng thái email.

## 14. Kết luận

Đã xóa hoàn toàn chức năng diagnostic email khỏi source runtime và giao diện Nhân sự. Luồng email kích hoạt tài khoản vẫn được giữ nguyên, logging an toàn vẫn còn, và các vấn đề delivery phía Inbox/Spam/filter/bounce vẫn cần kiểm tra ngoài ứng dụng khi Gmail đã chấp nhận thư.
# RÀ SOÁT TOÀN DIỆN CÁC YẾU TỐ ẢNH HƯỞNG ĐẾN EMAIL KÍCH HOẠT

## 1. Hiện tượng thực tế đã xác nhận

- Admin tạo nhân sự mới hoặc bấm `Gửi kích hoạt`.
- Ứng dụng báo máy chủ thư đã chấp nhận yêu cầu gửi email.
- Log an toàn có dạng `Bat dau gui email... Recipient=[đã che]... SmtpHost=smtp.gmail.com, Port=587` và `SMTP da chap nhan email`.
- Gmail người gửi có bản sao thư trong mục **Đã gửi**.
- Header bản sao phía người gửi có `Return-Path`, `Received ... by smtp.gmail.com with ESMTPSA`, `From`, `To`, `Content-Type: text/plain; charset=utf-8`, `Content-Transfer-Encoding: base64`.
- Recipient trong `To` đúng.
- `SenderEmail` và `Username` trong User Secrets được xác nhận giống nhau.
- App Password hoạt động vì SMTP đã xác thực và Gmail SMTP đã chấp nhận thư.
- Gửi thủ công từ Gmail Web đến cùng người nhận thì nhận được.
- Gửi thủ công đúng link activation đầy đủ có `userId` và `token` thì vẫn nhận được.
- Email activation do ứng dụng gửi qua SMTP không xuất hiện trong mailbox người nhận, kể cả tìm theo `rfc822msgid:`.
- Hiện tượng phát sinh theo thời điểm: hôm trước cùng chức năng nhận bình thường, hôm nay mới phát sinh.

Kết luận ở mức bằng chứng hiện có: source đã gửi được đến ranh giới Gmail SMTP accepted, nhưng chưa chứng minh thư đã được giao vào mailbox người nhận.

## 2. Phạm vi source đã đọc

Đã rà soát:

- `Controllers/NhanSuController.cs`
- `Controllers/AccountController.cs`
- `Services/Interfaces/IEmailService.cs`
- `Services/Interfaces/INhanSuService.cs`
- `Services/Interfaces/IAccountService.cs`
- `Services/Implementations/GmailEmailService.cs`
- `Services/Implementations/NhanSuService.cs`
- `Services/Implementations/AccountService.cs`
- `Services/AccountActivationOptions.cs`
- `Services/AccountActivationTokenHelper.cs`
- `Services/CauHinhDichVu.cs`
- `ViewModels/NhanSu/*`
- `ViewModels/Account/*`
- `Views/NhanSu/Index.cshtml`
- `Views/NhanSu/_Form.cshtml`
- `Views/NhanSu/_Table.cshtml`
- `Views/NhanSu/_Filter.cshtml`
- `Views/Account/Activate.cshtml`
- `Views/Shared/_Layout.cshtml`
- `Models/Entities/Aspnetusers.cs`
- `Models/Entities/Aspnetusertokens.cs`
- `Models/Entities/NguoiDung.cs`
- `Data/QuanLyDuAnDbContext.cs`
- `Program.cs`
- `appsettings.json`
- `appsettings.Development.json`
- `Properties/launchSettings.json`
- `QuanLyDuAn.csproj`
- `docs/nhansu.md`

Đã tìm các nhóm từ khóa liên quan `SendMailAsync`, `SmtpClient`, `MailMessage`, `MailAddress`, `SendAccountActivationEmailAsync`, `GuiLaiEmailKichHoat`, `TaoActivationUrl`, `EmailSettings`, `AccountActivation`, `SubjectEncoding`, `BodyEncoding`, `HeadersEncoding`, `AppBaseUrl`, `TokenLifetimeHours`, `ResendCooldownSeconds`.

## 3. Luồng tạo nhân sự và gửi email

Luồng thực tế:

```text
Views/NhanSu/_Form.cshtml
→ NhanSuController.LuuNhanSu
→ NhanSuService.SaveAsync
→ kiểm tra chức danh/username/email/số điện thoại/role/Admin
→ transaction Serializable
→ tạo NGUOI_DUNG
→ tạo AspNetUsers
→ tạo AspNetUserRoles
→ tạo AspNetUserTokens chứa hash activation token
→ commit database
→ TaoActivationUrl(userId, rawToken)
→ SendAccountActivationEmailAsync(email, hoTen, userName, activationUrl, lifetimeHours)
→ GmailEmailService.SendAsync
→ SmtpClient.SendMailAsync
→ controller hiển thị TempData
```

Nguồn recipient trong tạo mới là biến `email = model.Email.Trim()` được lưu vào `Aspnetusers.Email`, sau đó truyền trực tiếp vào `SendAccountActivationEmailAsync`. Source không dùng `NormalizedEmail` để gửi, không lấy email Admin và không lấy email người gửi làm recipient.

Email được gửi sau khi transaction tạo nhân sự/tài khoản/token đã commit. Nếu SMTP lỗi sau commit, tài khoản vẫn ở trạng thái chờ kích hoạt và service trả warning để Admin dùng chức năng gửi lại.

## 4. Luồng gửi lại email kích hoạt

Luồng thực tế:

```text
Views/NhanSu/_Table.cshtml
→ form POST GuiLaiEmailKichHoat(maNguoiDung)
→ NhanSuController.GuiLaiEmailKichHoat
→ NhanSuService.GuiLaiEmailKichHoatAsync
→ join NGUOI_DUNG với AspNetUsers theo nd.Id == tk.Id
→ lấy tk.Email
→ kiểm tra khóa tài khoản/chưa kích hoạt/email/cooldown
→ snapshot token cũ
→ transaction Serializable
→ xóa token activation cũ
→ tạo token mới
→ commit token mới
→ TaoActivationUrl(data.Id, rawToken)
→ SendAccountActivationEmailAsync(data.Email, hoTen, userName, activationUrl, lifetimeHours)
→ nếu gửi lỗi: xóa token mới và khôi phục token cũ còn hạn nếu có
```

Recipient trong resend lấy từ `Aspnetusers.Email` của đúng tài khoản join theo `NguoiDung.Id`. Không thấy source lấy nhầm email sender, email Admin hoặc email của người đang đăng nhập. Không có retry tự động. Cooldown hiện lấy từ `AccountActivation:ResendCooldownSeconds`, mặc định 60 giây.

## 5. Nguồn địa chỉ người nhận

- Tạo mới: `model.Email.Trim()` → `Aspnetusers.Email` → biến `email` truyền vào email service.
- Gửi lại: `tk.Email` từ `Aspnetusers` sau join với `NguoiDung` → `data.Email` truyền vào email service.
- `GmailEmailService.SendAsync` tiếp tục `Trim()` recipient bằng `var recipientEmail = toEmail?.Trim()`.
- `MailAddress.TryCreate(recipientEmail, out var recipientAddress)` kiểm tra cú pháp trước khi gửi.
- `mail.To.Add(recipientAddress)` là nơi gán người nhận thực tế.

Đánh giá: đã xác nhận đúng từ source. Nếu email trong DB sai với email người nhận thực tế thì ứng dụng vẫn gửi đúng dữ liệu DB; cần đối chiếu DB/log masked recipient với email thật ngoài source.

## 6. Cấu hình GmailEmailService

| Mục | Giá trị/source | Đánh giá | Nguy cơ |
|---|---|---|---|
| SMTP host | `EmailSettings:SmtpServer`, fallback `smtp.gmail.com` | Đúng với Gmail SMTP | Nếu User Secrets override sai thì sẽ lỗi SMTP, nhưng hiện Gmail accepted |
| Port | `EmailSettings:Port`, fallback `587` | Đúng cho STARTTLS Gmail | Port sai thường gây exception, không khớp hiện tượng Sent có thư |
| SenderEmail | `EmailSettings:SenderEmail.Trim()` | Đã xác nhận trùng Username | Nếu khác Username có thể giảm alignment, nhưng hiện đã xác nhận trùng |
| SenderName | `EmailSettings:SenderName` hoặc `QuanLyDuAn AI` | Bình thường | Không phải nguyên nhân trực tiếp |
| Username | `EmailSettings:Username.Trim()` | Đã xác nhận trùng SenderEmail | App Password hoạt động |
| AppPassword | remove space + trim | Không log secret | Nếu sai sẽ auth fail, nhưng hiện SMTP accepted |
| Recipient | tham số `toEmail` đã trim | Đúng | Cần đối chiếu email DB với email thật nếu người dùng báo không nhận |

## 7. Cấu hình MailMessage

| Thuộc tính | Giá trị thực tế | Nguồn cấu hình | Đánh giá | Nguy cơ |
|---|---|---|---|---|
| `From` | `new MailAddress(senderAddress.Address, senderName)` | `SenderEmail`, `SenderName` | Đúng | SenderName tiếng Việt/ASCII hỗn hợp không bất thường |
| `To` | `mail.To.Add(recipientAddress)` | recipient đã trim | Đúng | Không thấy gửi về sender/Admin |
| `CC` | Không set | Mặc định rỗng | Đúng | Không có |
| `Bcc` | Không set | Mặc định rỗng | Đúng | Không có |
| `ReplyToList` | Không set | Mặc định rỗng | Đúng | Không có Reply-To bất thường |
| `Subject` | `Kích hoạt tài khoản Quản lý dự án AI` | hard-code trong activation method | Hợp lệ | Nhiều email cùng subject có thể bị gom conversation ở UI |
| `Body` | Plain text tiếng Việt có username, lifetime và activation URL | template activation | Hợp lệ | Link HTTP/IP private + token dài có thể bị filter ở phía nhận |
| `IsBodyHtml` | `false` | source | Đúng | Không có HTML ẩn/tracking |
| `SubjectEncoding` | `Encoding.UTF8` | source | Đúng | Subject MIME encoded-word do thư viện tạo |
| `BodyEncoding` | `Encoding.UTF8` | source | Đúng | Có thể dẫn tới base64 transfer encoding, bình thường |
| `HeadersEncoding` | `Encoding.UTF8` | source | Đúng | Không thấy header custom |
| `Priority` | Không set | mặc định Normal | Đúng | Không có |
| `DeliveryNotificationOptions` | Không set | mặc định None | Bình thường | Không yêu cầu DSN, nên bounce/DSN không được ứng dụng theo dõi |
| `Headers` | Không thêm custom header | source | Đúng | Không có `In-Reply-To`, `References`, custom `Message-ID` |

`MailMessage` được tạo cục bộ trong mỗi lần gọi `SendAsync` và dispose bằng `using var`. Không thấy tái sử dụng object giữa nhiều email.

## 8. Cấu hình SmtpClient

| Thuộc tính | Giá trị thực tế | Đánh giá | Nguy cơ |
|---|---|---|---|
| `Host` | `smtp.gmail.com` từ config/fallback | Đúng | Không phải nguyên nhân nếu Gmail accepted |
| `Port` | `587` | Đúng | Không phải nguyên nhân nếu Gmail accepted |
| `EnableSsl` | `true` | Đúng cho Gmail STARTTLS | Không thấy lỗi TLS |
| `UseDefaultCredentials` | `false` | Đúng | Không dùng credential Windows |
| `Credentials` | `NetworkCredential(username, appPassword)` | Đúng | Secret không log |
| `Timeout` | Không set, dùng mặc định `SmtpClient` | Bình thường | Nếu mạng treo có thể exception timeout; không khớp accepted |
| `DeliveryMethod` | Không set, mặc định Network | Đúng | Không có pickup folder |
| `TargetName` | Không set | Bình thường | Không có chỉ dấu lỗi |
| `ClientCertificates` | Không set | Bình thường | Không cần cho Gmail |

`SmtpClient` được tạo cục bộ trong mỗi lần gửi, không reuse giữa request và được dispose. `await smtp.SendMailAsync(mail)` được gọi trực tiếp, không fire-and-forget.

## 9. DI lifetime và thread safety

- `IEmailService` đăng ký `AddScoped<IEmailService, GmailEmailService>()`.
- `INhanSuService` và `IAccountService` cũng đăng ký scoped.
- `GmailEmailService` chỉ giữ `IConfiguration` và `ILogger`, không giữ recipient, body, `MailMessage` hay `SmtpClient` trong field.
- `MailMessage` và `SmtpClient` là biến local theo từng lần gửi.

Đánh giá: chưa thấy nguy cơ request sau dùng lại dữ liệu request trước. Với thiết kế hiện tại, thread-safety tốt hơn so với việc dùng singleton có state hoặc reuse `SmtpClient`.

## 10. Xử lý exception và logging

- `GmailEmailService.SendAsync` validate cấu hình, sender, recipient, subject, body trước khi gửi.
- `SmtpException` được log với recipient đã che, operation, status code và exception type.
- Sau đó service throw `InvalidOperationException` với thông điệp thân thiện.
- `NhanSuService.SaveAsync` bắt exception gửi email sau commit và trả warning.
- `NhanSuService.GuiLaiEmailKichHoatAsync` bắt exception gửi lại, log lỗi và hoàn tác/khôi phục token.
- Log không chứa token raw, full activation URL, App Password, body email hoặc password.

Điểm cần lưu ý ở UI tạo mới: khi `SaveAsync` trả warning, controller hiện vẫn set `TempData["Success"] = "Đã lưu nhân sự"` và thêm `TempData["Warning"]`. Đây là nguy cơ gây hiểu nhầm ở UI nếu SMTP lỗi, nhưng không giải thích hiện tượng Gmail accepted + Sent có thư.

## 11. Phân tích header thực tế

| Header | Nguồn tạo | Ý nghĩa | Kết luận |
|---|---|---|---|
| `Return-Path` | Gmail/SMTP envelope | Địa chỉ bounce/envelope sender | Khớp email gửi là tín hiệu alignment tốt |
| `Received: ... smtp.gmail.com with ESMTPSA` | Gmail SMTP | Thư được nộp qua SMTP authenticated | Xác nhận Gmail SMTP accepted |
| `From` | Source set từ `SenderEmail`, Gmail giữ lại | Người gửi hiển thị | Khớp tài khoản gửi |
| `To` | Source set bằng `mail.To.Add` | Người nhận | Đã xác nhận đúng recipient |
| `Subject` | Source set | Tiêu đề activation | Hợp lệ, cố định |
| `Content-Type: text/plain; charset=utf-8` | `MailMessage`/SMTP MIME | Body plain text UTF-8 | Phù hợp source |
| `Content-Transfer-Encoding: base64` | MIME encoder | Cách mã hóa body UTF-8 | Bình thường với tiếng Việt, không tự chứng minh lỗi |
| `Message-ID` | Thư viện/Gmail SMTP, source không set custom | Định danh thư | Cần dùng đúng Message-ID bản gửi để tìm phía nhận |
| `Date` / `X-Google-Original-Date` | `MailMessage`/Gmail | Thời điểm gửi | Ví dụ đang tương đương theo timezone |

Header phía sender chưa có `Delivered-To`, `X-Received`, `Authentication-Results` của mailbox người nhận là bình thường, vì các header đó thường nằm trên bản sao recipient sau khi thư được giao. Bản sao trong Sent không đủ để chứng minh delivered.

## 12. Phân tích Message-ID

Source không thiết lập custom `Message-ID`, không thêm `In-Reply-To` và không thêm `References`. Vì vậy:

- Message-ID không phải do code nghiệp vụ tự tạo.
- Một lần SMTP accepted thường tương ứng một message riêng.
- Tìm `rfc822msgid:` phía người nhận không thấy là bằng chứng mạnh rằng mailbox người nhận chưa có bản thư đó, nhưng chưa chỉ ra nguyên nhân nằm ở source.
- Cần đối chiếu Message-ID của bản Sent và log thời điểm gửi để kiểm tra mỗi request chỉ sinh một thư.

## 13. Phân tích Date và timezone

Ví dụ:

```text
Date: Tue, 16 Jun 2026 23:07:14 -0700 (PDT)
X-Google-Original-Date: 17 Jun 2026 13:07:15 +0700
```

Hai thời điểm này tương đương gần như cùng lúc: 23:07 PDT ngày 16/06/2026 tương ứng 13:07 ICT ngày 17/06/2026. Chênh lệch khoảng một giây không cho thấy đồng hồ hệ thống sai.

Kết luận: timezone có thể làm người dùng tìm nhầm ngày trong giao diện Gmail, nhưng chưa có bằng chứng nó ảnh hưởng delivery.

## 14. Phân tích Subject và conversation/threading

Subject activation hiện cố định:

```text
Kích hoạt tài khoản Quản lý dự án AI
```

Không thấy source set `In-Reply-To`, `References` hoặc custom `Message-ID`. Gmail vẫn có thể gom conversation theo subject/participants ở tầng giao diện. Điều này có thể làm người nhận khó thấy email mới nếu nó bị xếp trong một thread cũ, nhưng không giải thích việc tìm theo `rfc822msgid:` không có kết quả.

Phép thử sau này, nếu cần chẩn đoán UI/threading, có thể dùng nhánh thử nghiệm riêng với subject duy nhất, ví dụ:

```text
Kích hoạt tài khoản Quản lý dự án AI - {UserName}
Kích hoạt tài khoản Quản lý dự án AI - {UserName} - {dd/MM/yyyy HH:mm:ss}
```

Đây chỉ là phép thử hiển thị/threading, không được khẳng định sẽ sửa delivery.

## 15. Phân tích template/body/encoding

Template hiện là plain text:

```text
Xin chào {recipientName},

Tài khoản của bạn trên hệ thống Quản lý dự án AI đã được tạo.

Tên đăng nhập: {userName}

Để đặt mật khẩu và kích hoạt tài khoản, vui lòng mở liên kết sau trong thời hạn {lifetimeHours} giờ:
{activationUrl}

Liên kết này chỉ sử dụng được một lần.

Nếu bạn không yêu cầu tài khoản này, vui lòng bỏ qua email.

Trân trọng,
Hệ thống Quản lý dự án AI
```

Đánh giá:

- Không dùng HTML.
- Không có ảnh, tracking pixel, text ẩn hoặc link hiển thị khác URL thật.
- Có tiếng Việt UTF-8, vì vậy MIME/base64 là bình thường.
- Có các cụm `kích hoạt tài khoản`, `đặt mật khẩu`, `liên kết`, và URL có token dài. Đây là nội dung có thể bị hệ thống lọc đánh giá khác email văn bản thường.
- Tuy nhiên, người dùng đã thử gửi thủ công đúng link activation đầy đủ từ Gmail Web và vẫn nhận được. Vì vậy không được kết luận URL/token là nguyên nhân duy nhất.

## 16. Phân tích activation URL và token

- URL được tạo bởi `TaoActivationUrl` từ path route `Account/Activate` và `AccountActivation:AppBaseUrl`.
- `AppBaseUrl` hiện là `http://192.168.2.27:5037` trong `appsettings.json` và `appsettings.Development.json`.
- `Program.cs` validate `AppBaseUrl` là HTTP/HTTPS absolute URL và không dùng localhost/loopback.
- Token raw chỉ xuất hiện trong URL email. Database lưu payload chứa SHA-256 hash, thời điểm tạo và hết hạn.
- Link Development dùng HTTP + IP private LAN + query token dài. Đây là yếu tố có nguy cơ bị bộ lọc đánh giá khác domain HTTPS thật, nhưng bằng chứng gửi thủ công cùng link vẫn nhận được làm giả thuyết này chưa đủ chắc.

## 17. Phân tích tần suất gửi và request đồng thời

Các tình huống có thể tạo nhiều email:

- Admin tạo nhiều nhân sự liên tiếp.
- Bấm `Gửi kích hoạt` nhiều lần.
- Double-click nút gửi lại.
- Refresh/resubmit request POST.
- Hai request đồng thời.

Biện pháp hiện có:

- Resend có cooldown `ResendCooldownSeconds`, hiện 60 giây.
- Resend dùng transaction Serializable khi thay token.
- Không có retry tự động trong `GmailEmailService`.
- Không thấy chống double-submit phía UI.

Đánh giá: gửi nhiều email giống nhau trong thời gian ngắn là nguy cơ có thể làm Gmail xử lý khác theo thời điểm, nhưng hiện chưa có log SMTP status hoặc thông báo Gmail xác nhận rate-limit/delay. Không được kết luận Gmail rate-limit khi chưa có bounce hoặc mã lỗi.

## 18. Những điểm source đã xác nhận đúng

- Recipient lấy từ `Aspnetusers.Email`.
- Email được trim trước khi gửi.
- `MailAddress.TryCreate` validate sender/recipient.
- `mail.To.Add(recipientAddress)` gán đúng người nhận.
- Không thấy gửi về sender/Admin/Bcc.
- `await smtp.SendMailAsync(mail)` được gọi đúng.
- `SmtpException` không bị nuốt rồi báo success trong email service.
- `MailMessage` và `SmtpClient` tạo mới mỗi lần gửi.
- `IEmailService` scoped, không giữ state recipient/body giữa request.
- `SenderEmail` và `Username` được người dùng xác nhận giống nhau.
- App Password hoạt động vì Gmail SMTP accepted.
- Email activation là plain text UTF-8, không HTML ẩn/tracking.
- Activation URL không dùng localhost.
- Token không log raw và không lưu raw trong DB.

## 19. Những nguy cơ còn tồn tại trong source

- Tạo mới nhân sự nếu SMTP lỗi có thể vừa có `Success = "Đã lưu nhân sự"` vừa có `Warning`, dễ gây hiểu nhầm ở UI. Đây là nguy cơ thông báo, không phải nguyên nhân delivery khi SMTP accepted.
- Không có client-side chống double-submit cho nút tạo/gửi lại.
- Không theo dõi bounce tự động vì không có outbox/delivery webhook.
- Không set `DeliveryNotificationOptions`; ứng dụng không yêu cầu DSN và không đọc mailbox bounce.
- Subject cố định có thể bị Gmail gom conversation ở UI.
- Link Development là HTTP private IP và token dài, có nguy cơ bị bộ lọc đánh giá khác domain HTTPS thật.

## 20. Những yếu tố ngoài phạm vi source

| Yếu tố | Trạng thái | Bằng chứng hiện có | Bằng chứng còn thiếu | Cách kiểm tra | Có thể sửa bằng source không |
|---|---|---|---|---|---|
| Gmail tạm trì hoãn giao thư SMTP | Có nguy cơ | SMTP accepted, Sent có thư nhưng recipient chưa thấy | Log/bounce/recipient header sau khi thư tới | Chờ vài giờ, tìm Message-ID, kiểm tra bounce | Không chắc; chỉ có thể giảm tần suất/nội dung |
| Gmail Web và Gmail SMTP được xử lý khác nhau | Có nguy cơ | Gmail Web gửi thủ công nhận được, SMTP app không thấy | Bằng chứng policy/filter từ Gmail | So sánh header Web vs SMTP | Có thể tinh chỉnh template/domain nếu có bằng chứng |
| Rate limit/anti-abuse tạm thời | Chưa đủ bằng chứng | Hiện tượng mới phát sinh theo ngày | SMTP/bounce/rate warning | Kiểm tra Mail Delivery Subsystem và log nhiều lần gửi | Không sửa khi chưa có mã lỗi |
| Reputation tạm thời của tài khoản gửi | Chưa đủ bằng chứng | Sent accepted nhưng delivery khác thường | Cảnh báo Gmail hoặc bounce | Thử sau vài giờ, gửi tới nhiều Gmail/Outlook | Ngoài source |
| Gmail conversation/threading | Có nguy cơ | Subject cố định | Người nhận có thread cũ chứa thư mới hay không | Tìm All Mail, thread, Message-ID | Có thể thử subject duy nhất sau này |
| Filter/rule người nhận | Ngoài phạm vi source | Người nhận không thấy Inbox | Kiểm tra rule/blocked sender/forwarding | Kiểm tra Gmail settings người nhận | Không |
| Bounce đến trễ | Chưa đủ bằng chứng | SMTP accepted không đảm bảo delivered | Email bounce trong sender mailbox | Tìm `mailer-daemon`, `Mail Delivery Subsystem` | Không, nếu không thêm hệ thống theo dõi |
| Sự cố Gmail tạm thời theo thời điểm | Chưa đủ bằng chứng | Hôm trước được, hôm nay lỗi | Thông báo dịch vụ/Gmail status hoặc thử nhiều thời điểm | Lặp lại test sau vài giờ | Không |

## 21. Ma trận giả thuyết nguyên nhân

| Giả thuyết | Bằng chứng ủng hộ | Bằng chứng phản bác | Trạng thái | Khả năng | Cách kiểm tra tiếp | Có cần sửa source không |
|---|---|---|---|---|---|---|
| Sai recipient | Người nhận không thấy thư | Source lấy `Aspnetusers.Email`, trim, `To` header đúng | Đã loại trừ | Thấp | Đối chiếu DB/log masked/header To | Không |
| SenderEmail khác Username | Có thể ảnh hưởng alignment | Người dùng xác nhận giống nhau, Return-Path/From khớp | Đã loại trừ | Thấp | Kiểm tra User Secrets thủ công | Không |
| App Password sai | Có thể gây lỗi SMTP | SMTP accepted, Sent có thư | Đã loại trừ | Thấp | Không cần nếu vẫn accepted | Không |
| SMTP auth thất bại | Có thể không gửi được | Header ESMTPSA và Sent có thư | Đã loại trừ | Thấp | Xem log SMTP error nếu có | Không |
| `SendMailAsync` không await | Có thể báo success sớm | Source dùng `await smtp.SendMailAsync(mail)` | Đã loại trừ | Thấp | Đã đọc source | Không |
| Exception bị nuốt | Có thể báo success sai | `SmtpException` log rồi throw; service/controller nhận warning/error | Đã loại trừ với email service | Thấp | Kiểm tra log khi cố tình sai cấu hình | Không trong bước này |
| Email bị gửi về sender | Sent có thư | `mail.To.Add(recipientAddress)`, header `To` đúng recipient | Đã loại trừ | Thấp | Kiểm tra header To | Không |
| MIME/UTF-8/Base64 lỗi | Base64 xuất hiện trong header | `text/plain; charset=utf-8`, source set UTF-8; Gmail Web nhận cùng nội dung link thủ công | Chưa đủ bằng chứng | Thấp-Trung bình | Decode body từ Sent và so với template | Chưa |
| Date/time sai | Có thể xếp sai ngày | Date PDT và X-Google-Original-Date +0700 tương đương | Đã loại trừ | Thấp | So sánh UTC timestamp | Không |
| Subject giống nhau bị thread | Subject cố định | Tìm `rfc822msgid` không thấy phía nhận | Có nguy cơ | Thấp-Trung bình | Kiểm tra All Mail/thread, thử subject duy nhất ở nhánh test | Chưa |
| URL HTTP/IP LAN bị lọc | Link private IP + HTTP + token dài | Gửi thủ công cùng link từ Gmail Web vẫn nhận | Có nguy cơ | Trung bình | So sánh SMTP activation không link/domain HTTPS | Chưa |
| Token dài bị lọc | Query token dài | Manual same full link nhận được | Chưa đủ bằng chứng | Thấp-Trung bình | Thử nội dung same token qua SMTP và Web | Chưa |
| Nội dung activation bị lọc | Có cụm đặt mật khẩu/kích hoạt | Template plain text, không HTML ẩn; manual cùng link nhận | Có nguy cơ | Trung bình | So sánh header/body Web vs SMTP | Chưa |
| Gửi nhiều thư giống nhau trong thời gian ngắn | Có resend/tạo nhiều account | Cooldown 60s, không có log rate-limit | Có nguy cơ | Trung bình | Kiểm tra log số lần gửi và Message-ID | Chưa |
| Double submit/request đồng thời | UI không chống double-submit | Resend transaction/cooldown giảm rủi ro | Có nguy cơ | Thấp-Trung bình | Kiểm tra log nhiều request cùng MaNguoiDung | Có thể sau này |
| Gmail SMTP accepted nhưng delivery bị trì hoãn | Sent có thư, recipient chưa thấy | Chưa có bounce/status từ Gmail | Ngoài phạm vi source | Trung bình-Cao | Chờ, tìm Message-ID sau vài giờ, bounce | Không |
| Gmail Web và Gmail SMTP được xử lý khác nhau | Web manual nhận, SMTP app không | Chưa có policy chính thức | Ngoài phạm vi source | Trung bình-Cao | So sánh full headers Web vs SMTP | Không trực tiếp |
| Mailbox người nhận không nhận thư SMTP tại thời điểm đó | Recipient không thấy kể cả rfc822msgid | Web manual nhận cùng ngày phản bác một phần | Ngoài phạm vi source | Trung bình | Gửi SMTP tới Gmail khác/Outlook | Không |
| Bounce đến trễ | SMTP accepted vẫn có thể bounce sau | Chưa thấy bounce được cung cấp | Chưa đủ bằng chứng | Trung bình | Tìm `Mail Delivery Subsystem`, `mailer-daemon` sau vài giờ | Không |
| Sự cố Gmail tạm thời | Hôm trước được, hôm nay lỗi | Chưa có trạng thái dịch vụ/bằng chứng ngoài | Ngoài phạm vi source | Trung bình | Thử lại sau vài giờ, nhiều recipient | Không |

## 22. Kịch bản kiểm thử tiếp theo

Chưa đánh dấu các test dưới đây là đã thực hiện trong lượt rà soát này:

- TC01: Gửi một activation duy nhất sau vài giờ, không bấm resend liên tục.
- TC02: Gửi activation đến một Gmail khác.
- TC03: Tìm theo Message-ID trên người nhận bằng `rfc822msgid:`.
- TC04: Kiểm tra mailer-daemon/bounce sau vài giờ ở Gmail người gửi.
- TC05: So sánh header thư Gmail Web và SMTP cho cùng recipient/link.
- TC06: Kiểm tra subject duy nhất trong một nhánh thử nghiệm riêng.
- TC07: Theo dõi thời gian từ SMTP accepted đến mailbox received.
- TC08: Kiểm tra có request gửi trùng trong log không.
- TC09: Kiểm tra một lần gửi tương ứng đúng một Message-ID.
- TC10: Kiểm tra DI lifetime và concurrent send; hiện source cho thấy service scoped và không giữ state.

## 23. File có thể cần sửa ở bước sau

Chỉ nên sửa khi có thêm bằng chứng:

- `Services/Implementations/GmailEmailService.cs`: nếu cần thêm header an toàn, subject thử nghiệm, timeout rõ ràng hoặc logging không nhạy cảm bổ sung.
- `Services/Implementations/NhanSuService.cs`: nếu cần chống concurrent/double submit chặt hơn hoặc giảm gửi lặp.
- `Controllers/NhanSuController.cs`: nếu cần chỉnh thông báo UI tạo mới khi warning để tránh success + warning cùng lúc.
- `Views/NhanSu/_Table.cshtml` hoặc JS module liên quan: nếu cần disable button sau submit.
- `docs/nhansu.md`: tiếp tục ghi kết quả test thực tế.

Không nên sửa các file này trong bước hiện tại vì yêu cầu chỉ phân tích và cập nhật tài liệu.

## 24. Những phần không nên sửa

- Không đổi `smtp.gmail.com`, port 587, TLS hoặc App Password khi SMTP đã accepted.
- Không đổi recipient nếu header `To` và source đã đúng.
- Không đổi token activation, thời hạn token, route `Account/Activate` hoặc `[AllowAnonymous]`.
- Không thêm retry vô hạn, outbox, delivery tracking giả, bảng/cột/migration.
- Không log token raw, full activation URL, App Password, body email chứa token hoặc User Secrets.
- Không kết luận email đã vào Inbox chỉ vì có Sent hoặc SMTP accepted.

## 25. Kết luận

Rà soát source không phát hiện lỗi gửi sai recipient, không phát hiện `SendMailAsync` thiếu `await`, không phát hiện exception SMTP bị nuốt, không phát hiện `MailMessage` gửi về sender/Admin/Bcc, và không phát hiện state dùng lại giữa request.

Với bằng chứng hiện có, ứng dụng đã gửi tới mức Gmail SMTP authenticated accepted. Hiện tượng thư có trong Sent nhưng không xuất hiện ở mailbox người nhận nhiều khả năng nằm ở giai đoạn sau SMTP accepted: delivery nội bộ Gmail, filter/spam/threading, bounce đến trễ, chính sách xử lý khác giữa Gmail Web và Gmail SMTP, tần suất gửi tự động theo thời điểm hoặc sự cố tạm thời ngoài source. Các giả thuyết này cần kiểm chứng bằng Message-ID, bounce, kiểm tra mailbox người nhận và so sánh header Web/SMTP trước khi quyết định sửa source.

Trong lượt này chỉ cập nhật tài liệu, không sửa source runtime, không sửa database, không tạo hoặc sửa migration, không thay đổi workflow kích hoạt và không thay đổi token activation.
