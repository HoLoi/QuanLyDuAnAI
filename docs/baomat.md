# PHÂN TÍCH BẢO MẬT HỆ THỐNG QUẢN LÝ DỰ ÁN

> Thời điểm rà soát source: 02/07/2026. Trạng thái trong tài liệu được hiểu theo source hiện có tại checkout `dvl20`, không phải theo README hay giả định triển khai.

## 1. Phạm vi và phương pháp rà soát

Rà soát tĩnh được thực hiện trên hai khối:

- ASP.NET Core MVC: `Program.cs`, `Services/CauHinhDichVu.cs`, toàn bộ `Controllers`, `Services/Interfaces`, `Services/Implementations`, `Data/QuanLyDuAnDbContext.cs`, `Data/KhoiTaoTaiKhoanMacDinh.cs`, các entity Identity/`NguoiDung`, ViewModel và Razor View tài khoản, `appsettings*.json`, `Properties/launchSettings.json`, `.csproj`, migration và thư mục `wwwroot/uploads`.
- FastAPI: `app/main.py`, `app/config.py`, `app/schemas.py`, toàn bộ `app/routers`, `app/services`, `app/ml`, `run.py`, thư mục `models` và `logs`.
- Đối chiếu thêm các hằng quyền/trạng thái, tài liệu trong `docs`, script SQL và cấu trúc artefact triển khai. README/docs chỉ dùng đối chiếu; kết luận bên dưới lấy source đang chạy làm nguồn sự thật.

Phương pháp: truy vết form/action/service/database cho từng luồng; kiểm kê attribute xác thực, quyền và anti-forgery; kiểm tra cấu hình cookie/HTTPS/CORS/secret; truy vết đường đi upload/download; tìm raw SQL, log và thao tác file model. Có 55 action `[HttpPost]` không thấy `[ValidateAntiForgeryToken]` tại chính action. Đây là source audit, chưa chạy ứng dụng, chưa kiểm tra SMTP/SQL thật, chưa pentest, DAST, quét dependency hay kiểm tra cấu hình reverse proxy/firewall ngoài repository.

Nhãn dùng trong tài liệu:

- **Đã dùng**: có đăng ký và có đường gọi từ controller/service.
- **Có code/chưa chắc được gọi**: có hiện thực nhưng không tìm được đường gọi đầy đủ.
- **Chỉ giao diện**: có View/form nhưng xử lý server thiếu.
- **Chưa triển khai**: không tìm thấy cơ chế trong source.
- **Không thể kết luận**: phụ thuộc hạ tầng/secret ngoài source.

## 2. Kiến trúc bảo mật tổng thể hiện tại

```text
Trình duyệt
  -> ASP.NET Core MVC (cookie tự tạo, role/permission claim, kiểm tra scope trong controller/service)
     -> EF Core -> SQL Server
     -> HttpClient/AiApiService (HTTP JSON, không credential)
        -> FastAPI (CORS nhưng không authentication/authorization)
           -> file .joblib + metadata + log cục bộ
```

- **Xác thực trình duyệt**: `AccountController.Login` gọi `AccountService.AuthenticateAsync`; controller tự gọi `HttpContext.SignInAsync`. Dự án tham chiếu Identity và dùng `PasswordHasher<Aspnetusers>`, nhưng không đăng ký/không dùng `UserManager`, `SignInManager` hay đầy đủ Identity middleware.
- **Tài khoản và token**: bảng ánh xạ `Aspnetusers`, `Aspnetuserroles`, `Aspnetroleclaims`, `Aspnetuserclaims`, `Aspnetusertokens`; hồ sơ nghiệp vụ nằm ở `NguoiDung`.
- **Phân quyền**: role và permission được nạp vào cookie lúc đăng nhập trong `AccountService.AuthenticateAsync`. `PermissionHelper`/`PhanQuyenService.GetGrantedPermissionNamesAsync` đọc claim trong cookie; nhiều service kiểm tra thêm dự án/team/leader/phân công.
- **Email**: `GmailEmailService` dùng SMTP TLS; kích hoạt do `NhanSuService`, quên mật khẩu do `AccountService`.
- **Token**: token kích hoạt ngẫu nhiên 32 byte, chỉ lưu SHA-256 trong payload; quên mật khẩu dùng OTP sáu số đã băm SHA-256 và một mã phiên GUID.
- **Cookie**: `Program.cs` đăng ký cookie authentication và `AccountController` tạo ticket.
- **AI**: MVC quyết định quyền giao diện/nghiệp vụ trước phần lớn lời gọi; FastAPI thực hiện compute và quản lý file model, không kết nối SQL Server. FastAPI không tự xác thực bên gọi.

## 3. Các cơ chế bảo mật đang được sử dụng

| Cơ chế | Trạng thái hiện tại | Bằng chứng source | Mức độ bảo vệ | Hạn chế/rủi ro |
| --- | --- | --- | --- | --- |
| ASP.NET Core Identity primitives | Đã dùng một phần | `AccountService`, `TaiKhoanCaNhanService`, `KhoiTaoTaiKhoanMacDinh` dùng `PasswordHasher<Aspnetusers>`; entity `Aspnetusers` | Băm/kiểm tra mật khẩu theo PasswordHasher | Không dùng `AddIdentity`, `UserManager`, `SignInManager`, token provider hay password policy trung tâm |
| Cookie authentication | Đã dùng | `Program.cs:AddCookie`; `AccountController.Login/Logout` | Cookie ticket có thời hạn | Không cấu hình tường minh `HttpOnly`, `SecurePolicy`, `SameSite`, tên cookie; không validate security stamp/tình trạng khóa mỗi request |
| Role | Đã dùng | `AccountService.AuthenticateAsync`, `GetAliasRoles`, `User.IsInRole` tại service/controller | Phân lớp Admin/Manager/Employee | Role đóng băng trong cookie đến khi đăng nhập lại/hết hạn |
| Permission claim | Đã dùng | `PhanQuyenService`, `PermissionHelper`, `Permissions.cs` | Nhiều action quan trọng kiểm tra quyền server-side | Claim cũng đóng băng trong cookie; một số controller chỉ có `[Authorize]` rồi giao quyền cho service |
| Scope dự án/team/phân công | Đã dùng nhưng không đồng nhất | `DuAnService`, `CongViecService.KiemTraQuyenXuLyTrangThaiCongViecAsync`, `ChiTietCongViecService.KiemTraQuyenCapNhatAsync`, các service đánh giá | Giảm IDOR ở nhiều luồng | Cần test từng endpoint; file tĩnh trong `wwwroot` vượt qua toàn bộ scope download |
| Băm mật khẩu | Đã dùng | `PasswordHasher<Aspnetusers>.HashPassword/VerifyHashedPassword` | Tốt hơn tự băm | Policy chỉ nằm ở từng ViewModel; reset/đổi mật khẩu chỉ yêu cầu 6 ký tự |
| `EmailConfirmed` | Đã dùng | `AccountService.AuthenticateAsync`, `KichHoatTaiKhoanAsync`, `KhoiTaoQuenMatKhauAsync` | Chặn đăng nhập/reset với email chưa xác nhận | Tài khoản seed mặc định được xác nhận sẵn |
| Token kích hoạt | Đã dùng | `AccountActivationTokenHelper`, `NhanSuService.SaveAsync/GuiLaiEmailKichHoatAsync` | Ngẫu nhiên mạnh, lưu hash, có hạn, một lần | URL có thể là HTTP; giá trị lifetime do cấu hình, chưa test runtime |
| OTP reset | Đã dùng | `AccountService.KhoiTaoQuenMatKhauAsync`, `XacNhanOtpDatLaiMatKhauAsync` | 3 phút, tối đa 5 lần sai, OTP không lưu rõ | Không rate-limit số lần yêu cầu; mã phiên xuất hiện trong URL; không thông báo hậu đổi |
| Khóa tài khoản | Có khóa quản trị | `NhanSuService.LockAccountAsync/UnlockAccountAsync`; login kiểm tra `LockoutEnd` | Chặn lần đăng nhập mới | Sai mật khẩu không tăng thất bại; cookie cũ không bị vô hiệu |
| Security stamp | Có ghi nhưng chưa được dùng để kiểm tra phiên | đổi/reset/kích hoạt cập nhật `SecurityStamp` | Tiềm năng bảo vệ | Cookie principal không chứa/không validate stamp |
| Anti-forgery | Dùng không đồng nhất | Có ở Account, NhanSu, PhanQuyen, AI; thiếu tại 55 POST khác | Bảo vệ tốt ở action có attribute | Nhiều thao tác sửa/xóa/duyệt/upload chịu CSRF |
| Model validation server | Đã dùng | DataAnnotations; Pydantic `StrictBaseModel(extra="forbid")` | Chặn thiếu/sai trường và over-posting ở nhiều form/API | MVC vẫn bind page ViewModel lớn; policy mật khẩu phân tán |
| EF Core | Đã dùng | `QuanLyDuAnDbContext`, LINQ trong services | Truy vấn LINQ được tham số hóa | Không suy ra được quyền SQL tối thiểu; IDOR vẫn là rủi ro logic |
| Upload | Một phần | avatar và tiến độ có extension/size; `FileDuAnService` không có whitelist/size | Đổi tên UUID giảm đụng tên/path traversal | Không MIME/magic-byte; file dự án tùy ý; tất cả dưới `wwwroot` |
| Soft delete | Đã dùng nhiều entity | `IsDeleted`, `DeletedAt`, `DeletedBy` trong service/entity | Hỗ trợ giữ lịch sử | Không phải ranh giới bảo mật; query bỏ sót điều kiện vẫn có thể lộ |
| Audit log | Có log vận hành rời rạc | `GmailEmailService`, `NhanSuService`, `AiApiService`, FastAPI `log_service` | Có dấu vết train/analyze/lỗi email | Thiếu audit đăng nhập, reset, đổi mật khẩu, quyền; log AI có thể chứa payload |
| CORS | Đã dùng | `app/main.py:CORSMiddleware` | Origin lấy từ cấu hình | `allow_credentials=True`, method/header `"*"`; CORS không thay thế API auth |
| HTTPS/HSTS | Một phần | `Program.cs` chỉ `UseHsts/UseHttpsRedirection` khi không Development | Production trực tiếp có redirect/HSTS | profile HTTP bind mọi interface; activation/AI base URL đang có thể dùng HTTP |
| Secret | Một phần | `.csproj:UserSecretsId`; `EmailSettings:*`; `.env` FastAPI | Có đường dùng User Secrets/env | connection string có giá trị trong repo; seed chứa credential mặc định dự đoán được |

## 4. Phân tích luồng đăng nhập

1. `Views/Account/Login.cshtml` bind `DangNhapViewModel` gồm `TenDangNhap`, `MatKhau`, `GhiNhoDangNhap`; POST có anti-forgery.
2. `AccountController.Login` kiểm tra ModelState rồi gọi `AccountService.AuthenticateAsync`.
3. Service tìm theo `NormalizedUserName` hoặc `UserName`; **không đăng nhập bằng email/mã nhân sự**.
4. Service kiểm tra `LockoutEnabled && LockoutEnd > UtcNow`, `EmailConfirmed`, `PasswordHash`, rồi `VerifyHashedPassword`.
5. Service truy vấn role, alias role, `IsLeader`, role claim và user claim; tạo `ClaimsPrincipal`.
6. Controller gọi `SignInAsync`; phiên thường 8 giờ, ghi nhớ 7 ngày. `AllowRefresh=true`; `Program.cs` đặt `SlidingExpiration=true`, `ExpireTimeSpan=8 giờ`.
7. `returnUrl` chỉ được dùng khi `Url.IsLocalUrl(returnUrl)`, vì vậy đường này đã chống open redirect.

Đánh giá:

- Thông báo sai user và sai password giống nhau, nhưng trạng thái “bị khóa” và “chưa kích hoạt” khác nhau, nên có thể xác nhận sự tồn tại/trạng thái của username đã biết.
- Không có chống brute force: `AccessFailedCount` không tăng, không đặt `LockoutEnd` sau số lần sai, không rate limit/CAPTCHA. `LockoutEnabled` của user mới không mang lại lockout tự động.
- Không thấy kiểm tra `NguoiDung.IsDeleted` khi login. Nếu chỉ soft-delete hồ sơ mà không khóa/xóa account, source login vẫn có thể cấp cookie.
- Các thuộc tính cookie `HttpOnly`, `Secure`, `SameSite`, tên cookie không được cấu hình tường minh; chúng phụ thuộc default framework/môi trường và chưa thể xác nhận hiệu quả triển khai.
- Không có session server-side; không thấy `AddSession/UseSession`. Session fixation theo session ID không áp dụng trực tiếp, nhưng ticket cũ vẫn là vấn đề.
- Khi khóa tài khoản, đổi role/permission, đổi/reset password, source chỉ cập nhật DB/stamp; không có `ValidatePrincipal`/security-stamp validator. Cookie cũ tiếp tục chứa role/claim cũ đến hết hạn hoặc logout.
- Logout chỉ xóa cookie ở trình duyệt hiện tại; không có “đăng xuất mọi thiết bị”.

## 5. Phân tích luồng quên mật khẩu và đặt lại mật khẩu

Luồng thực tế **không dùng link reset token của ASP.NET Identity** mà dùng OTP:

1. Người dùng nhập email **hoặc username** tại `AccountController.ForgotPassword`.
2. `AccountService.KhoiTaoQuenMatKhauAsync` tìm theo username/email.
3. Nếu không có account, email trống hoặc `EmailConfirmed=false`, service vẫn trả mã phiên ngẫu nhiên và controller vẫn chuyển tới VerifyOtp với thông báo chung.
4. Nếu hợp lệ, service xóa token reset cũ, sinh OTP sáu số bằng `RandomNumberGenerator`, lưu hash SHA-256 + hạn 3 phút trong `Aspnetusertokens`.
5. `GmailEmailService.SendAsync` gửi OTP; không tạo URL từ `Host` cho luồng reset.
6. `VerifyOtp` nhận `maPhien`; tối đa 5 lần sai, đúng thì xóa OTP và tạo token `VERIFIED` hạn 3 phút.
7. `ResetPassword` kiểm tra phiên VERIFIED; POST nhận mật khẩu mới/xác nhận.
8. `DatLaiMatKhauBangOtpAsync` băm mật khẩu, đổi security/concurrency stamp, xóa các token reset và mở `LockoutEnd`.

Điểm an toàn: thông báo ban đầu tương đối chung; email đã là bằng chứng sở hữu; OTP không lưu rõ; so sánh hash constant-time; hạn ngắn; giới hạn năm lần; token VERIFIED bị xóa sau reset nên chống dùng lại; reset đổi stamp và xóa token cũ.

Hạn chế:

- Không rate limit/cooldown số yêu cầu quên mật khẩu theo IP/account/email; mỗi yêu cầu hợp lệ thay OTP cũ và gửi mail, có thể gây mail bombing/DoS.
- OTP chỉ có 1.000.000 khả năng. Giới hạn 5 lần trên token là hữu ích, nhưng kẻ tấn công có thể liên tục tạo token mới vì không có rate limit.
- `maPhien` nằm trong query string và hidden field; bản thân nó chưa đủ reset nếu chưa VerifyOtp. Tuy vậy, sau khi OTP được xác nhận, chỉ `maPhien` là bearer secret trong 3 phút.
- Không kiểm tra `LockoutEnd` hay `NguoiDung.IsDeleted` khi khởi tạo/reset. Reset còn chủ động xóa `LockoutEnd`, nên người sở hữu email có thể mở khóa tài khoản do quản trị viên khóa.
- Mật khẩu reset chỉ 6–100 ký tự; không bắt buộc hoa/thường/số/đặc biệt, không chặn mật khẩu phổ biến/rò rỉ, không có lịch sử.
- Security stamp đổi nhưng cookie không validate stamp; phiên cũ không bị đăng xuất.
- Không gửi email thông báo sau reset; không có audit sự kiện reset.
- Không log OTP/URL trong `AccountService`/`GmailEmailService`; email được mask trong log. Tuy nhiên lỗi SMTP được log kèm exception.
- Host Header Injection không áp dụng cho reset hiện tại vì không tạo URL. `AccountActivation:AppBaseUrl` chỉ dùng kích hoạt.

Kết luận: cơ chế xác minh email bằng OTP là đúng hướng và tốt hơn một form reset trực tiếp, nhưng thiếu rate limit, kiểm tra trạng thái account, vô hiệu hóa cookie cũ và policy mật khẩu mạnh.

## 6. Phân tích chức năng thay đổi mật khẩu khi đã đăng nhập

`TaiKhoanCaNhanController.DoiMatKhau` có `[Authorize]`, POST và `[ValidateAntiForgeryToken]`. `DoiMatKhauViewModel` yêu cầu mật khẩu hiện tại, mật khẩu mới, xác nhận trùng; 6–100 ký tự. `TaiKhoanCaNhanService.DoiMatKhauAsync` lấy đúng `NameIdentifier`, xác minh hash mật khẩu hiện tại, cấm mật khẩu mới trùng chuỗi hiện tại, băm mật khẩu mới và đổi `SecurityStamp`/`ConcurrencyStamp`.

Đã có: xác thực lại bằng mật khẩu cũ, confirm, CSRF, server validation, đúng account hiện tại. Chưa có: policy hoa/thường/số/đặc biệt; kiểm tra mật khẩu phổ biến/lịch sử; OTP tăng cường cho Admin/Manager; email cảnh báo; audit log; sign-out các phiên khác. Cookie hiện tại và cookie thiết bị khác không tự mất hiệu lực vì không validate stamp. Không thấy action Admin đặt trực tiếp mật khẩu người khác; quản trị viên chỉ tạo/kích hoạt/khóa account. Nếu không biết mật khẩu hiện tại, đường hợp lệ duy nhất là quên mật khẩu qua email.

## 7. Có nên thêm xác nhận email khi quên hoặc thay đổi mật khẩu?

### 7.1 Quên mật khẩu

**Không nên thêm một bước “xác nhận email” trùng lặp.** `AccountService.KhoiTaoQuenMatKhauAsync` đã gửi OTP tới email đã xác nhận; nhập đúng OTP chính là xác minh quyền sở hữu email. Giá trị bảo mật nên được bổ sung ở rate limit/cooldown, chặn account bị khóa/xóa, policy mật khẩu, vô hiệu phiên cũ, thông báo hậu đổi và audit; không phải thêm một email-confirmation thứ hai.

### 7.2 Thay đổi mật khẩu khi đang đăng nhập

- Tài khoản thường: giữ yêu cầu đúng mật khẩu hiện tại; đây là xác thực lại phù hợp quy mô khóa luận.
- Admin/Manager hoặc phiên lâu/thiết bị-IP bất thường: nên thêm OTP email như bước tăng cường, nhưng chỉ sau khi đã sửa security-stamp/cookie/rate-limit.
- Không có mật khẩu hiện tại: bắt buộc chuyển sang luồng quên mật khẩu.
- Sau đổi: gửi email cảnh báo, ghi audit, vô hiệu các phiên khác. Có thể giữ phiên hiện tại bằng cách phát hành lại cookie sau khi stamp đổi.
- Nếu tương lai cho đổi email: phải xác minh email mới trước khi ghi chính thức. Hiện `NhanSuService.SaveAsync` cấm đổi email và hồ sơ cá nhân không bind email, nên chức năng đổi email **chưa triển khai**.

Phương án vừa đủ cho đề tài: mật khẩu hiện tại cho mọi user + OTP bổ sung cho Admin/Manager; không cần xây hệ thống thiết bị/MFA đầy đủ ở giai đoạn chính.

## 8. Phân tích chính sách mật khẩu và khóa tài khoản

Source không đăng ký `IdentityOptions.Password/Lockout`, vì vậy không nên diễn giải default Identity như policy đang thực thi. Policy thực tế là DataAnnotations của từng ViewModel:

| Luồng | Policy thực tế |
| --- | --- |
| Kích hoạt | `ActivateAccountViewModel`: 8–100, ít nhất một hoa, thường, số, ký tự đặc biệt |
| Reset | `ResetPasswordViewModel`: 6–100, không yêu cầu thành phần |
| Đổi khi đăng nhập | `DoiMatKhauViewModel`: 6–100, confirm; service cấm trùng mật khẩu hiện tại |
| Số ký tự khác nhau | Không cấu hình |
| Mật khẩu phổ biến/rò rỉ | Chưa triển khai |
| Lịch sử mật khẩu | Chưa triển khai |

Khóa: tài khoản mới có `LockoutEnabled=true`; seed mặc định có `LockoutEnabled=false`. `AccountService.AuthenticateAsync` chỉ đọc trạng thái khóa, không tăng `AccessFailedCount`, nên không có `MaxFailedAccessAttempts` hay lockout time do sai đăng nhập. Khóa quản trị tại `NhanSuService.LockAccountAsync` đặt `LockoutEnd` rất xa trong tương lai; mở khóa đặt null và reset count. Reset password cũng đặt `LockoutEnd=null`, đây là lỗi ranh giới nghiệp vụ.

## 9. Phân tích cookie và phiên đăng nhập

| Thuộc tính | Hiện trạng source |
| --- | --- |
| `HttpOnly` | Không cấu hình tường minh; phụ thuộc default handler |
| `SecurePolicy` | Không cấu hình; môi trường HTTP vẫn có thể phát cookie |
| `SameSite` | Không cấu hình tường minh |
| `ExpireTimeSpan` | 8 giờ |
| `SlidingExpiration` | `true` |
| Ghi nhớ | Controller đặt `ExpiresUtc` 7 ngày và `IsPersistent=true` |
| Tên cookie | Không cấu hình |
| Login/Logout/Denied path | `/Account/Login`, `/Account/Logout`, `/Account/AccessDenied` |
| Security stamp validation | Chưa có |
| Đăng xuất một thiết bị | Có, POST Logout |
| Đăng xuất tất cả | Chưa có |

Không có kho phiên server-side hay danh sách thiết bị. Sau đổi/reset password, khóa account, đổi role/permission, cookie cũ vẫn có thể chạy vì authorization đọc principal trong ticket. `LockoutEnd`, `EmailConfirmed`, `IsDeleted` không được đọc lại mỗi request. Đây là rủi ro cao nhất ở lớp phiên.

## 10. Phân quyền và kiểm soát phạm vi dữ liệu

Ba lớp hiện có:

1. Role: role DB được đưa vào `ClaimTypes.Role`, có alias `ADMIN -> Admin`, `MANAGER -> Manager`, `USER/USER_LEADER -> Employee`.
2. Permission: role/user claim được đưa vào cookie; `PermissionHelper.HasPermissionAsync` đọc lại từ principal, không truy vấn DB.
3. Scope: nhiều service kiểm tra quản lý dự án, team leader, thành viên hoặc người được phân công. Ví dụ `CongViecService.KiemTraQuyenXuLyTrangThaiCongViecAsync`, `ChiTietCongViecService.KiemTraQuyenCapNhatAsync`, `TienDoCongViecService.CoTheXemTienDoChiTietAsync`; service đánh giá tự kiểm tra permission claim.

Action nhạy cảm:

- Tài khoản/phân quyền: `NhanSuController` và `PhanQuyenController` kiểm tra permission server-side.
- AI dataset/train/model: `AiDatasetController`/`AiController` kiểm tra `Permissions.AI.*` trước gọi service.
- Dự án/duyệt/công việc/đánh giá/xuất: phần lớn controller hoặc service có kiểm tra permission/scope, nhưng 55 POST thiếu anti-forgery.
- Phân tích/xác nhận nguyên nhân: `AiController` và `DuAnController` kiểm tra permission; `AiService.KiemTraQuyenPhanTich` kiểm tra context.

Không thể kết luận mọi IDOR chỉ bằng kiểm kê attribute; cần chạy test ma trận. Có một bypass chắc chắn: file trong `wwwroot/uploads` được `UseStaticFiles` phục vụ trước authentication, nên dù `TaiFileDuAn`/`TaiFileTienDo` kiểm tra scope, ai biết URL vật lý vẫn tải trực tiếp. Ngoài ra, quyền bị thu hồi không tác động cookie cũ.

## 11. Bảo mật request và giao diện web

- 55 POST thiếu `[ValidateAntiForgeryToken]`, gồm chat, tạo/sửa/xóa công việc, dự án, team/thành viên/phân công, duyệt/từ chối đề xuất, đánh giá nhân viên, tiến độ và upload/xóa file. Razor Form Tag Helper thường sinh token, nhưng không có attribute/filter server-side thì token không được xác minh.
- Account, hồ sơ cá nhân, NhanSu, PhanQuyen, AI và một số action DanhGiaDuAn có anti-forgery.
- Razor mặc định encode output. Các vị trí `Html.Raw` trong Dashboard/AI dùng kết quả `JsonSerializer.Serialize`, giảm rủi ro hơn raw user text nhưng cần encoder phù hợp và test chuỗi `</script>`.
- Chat View dùng Razor model; không thấy `Html.Raw` cho nội dung chat. Server vẫn cần giới hạn độ dài/nội dung và CSP.
- Open redirect login đã dùng `Url.IsLocalUrl`.
- Pydantic API dùng `extra="forbid"`; MVC dùng ViewModel thay vì bind entity ở đa số action, giảm over-posting. Một số PageViewModel lớn cần tiếp tục whitelist thuộc tính.
- Production có `UseExceptionHandler("/Home/Error")`; Development có thể hiện developer exception theo template mặc định, không nên public.
- Không thấy CSP, `X-Content-Type-Options`, `X-Frame-Options`/`frame-ancestors`, Referrer Policy hay Permissions Policy.

## 12. Bảo mật file upload

| Nhóm | Hiện trạng |
| --- | --- |
| Avatar | `TaiKhoanCaNhanService`: whitelist `.jpg/.jpeg/.png/.webp`, tối đa 2 MB, UUID; không MIME/magic-byte; lưu `wwwroot/uploads/avatars` |
| File dự án | `FileDuAnService.UploadAsync`: chỉ kiểm tra không rỗng, dùng `Path.GetFileName`, giữ extension và UUID; **không whitelist, MIME, magic-byte, giới hạn size** |
| File tiến độ/minh chứng | `TienDoCongViecService`: whitelist PDF/Office/image/ZIP, tối đa 10 MB, UUID; không MIME/magic-byte |
| File công việc/chi tiết công việc | Có entity `FileCongViec`, `FileCtCongViec` nhưng không tìm thấy upload service/controller đang dùng; không thể kết luận là tính năng hoạt động |
| File xuất báo cáo | Sinh trong memory bởi `ExportFileService`, trả qua MVC; không phải upload |

Path traversal từ tên upload được giảm bởi `Path.GetFileName` và tên lưu UUID. Model filename được `SAFE_FILE_PATTERN` và kiểm tra parent path. Tuy nhiên:

- Tất cả upload thực tế nằm trong `wwwroot` và `UseStaticFiles` cho phép truy cập URL trực tiếp không authorization.
- File dự án cho phép extension tùy ý, kể cả nội dung có thể được browser/server diễn giải; rủi ro tăng mạnh nếu server static phục vụ MIME nguy hiểm.
- Chỉ kiểm extension, không kiểm signature/MIME; file giả mạo được chấp nhận.
- `GetPhysicalPath` ghép đường dẫn DB mà không kiểm resolved path còn trong webroot; người dùng thường không sửa DB, nên đây là defense-in-depth.
- Controller download có scope, nhưng không bảo vệ được URL static. File trùng được tránh bằng GUID.

## 13. Bảo mật SQL Server và EF Core

- `Program.cs` dùng `UseSqlServer` và service dùng LINQ/EF Core; không tìm thấy `FromSql*`, `ExecuteSql*` hay ghép raw SQL trong source C# đang rà soát. Nguy cơ SQL injection trực tiếp thấp.
- Có transaction cho kích hoạt, tạo nhân sự, cập nhật quyền, duyệt và một số nghiệp vụ quan trọng.
- `EnableSensitiveDataLogging` không xuất hiện, nên source không bật nó.
- `ConnectionStrings:DefaultConnection` có giá trị trực tiếp trong `appsettings.json` thuộc repository workspace. Tài liệu không ghi giá trị; cần coi là secret cần di chuyển/rotate.
- Không thể kết luận SQL login có quyền tối thiểu, TLS DB, backup encryption hay firewall từ source.
- IDOR là rủi ro logic quan trọng hơn SQL injection: mọi endpoint lấy ID phải kiểm permission + scope tại server. Bypass file static đã xác nhận.

## 14. Bảo mật email và secret

- SMTP đọc các khóa `EmailSettings:SmtpServer`, `Port`, `SenderEmail`, `SenderName`, `Username`, `AppPassword`; `EnableSsl=true`, không dùng default credentials.
- `.csproj` có `UserSecretsId`; appsettings hiện để trống Username/AppPassword, cho thấy có chủ đích cấp qua User Secrets/env, nhưng không thể xác nhận secret runtime.
- Connection string đang được điền trong repository. `KhoiTaoTaiKhoanMacDinh.TaoNguoiDungMacDinhAsync` chứa credential mặc định có thể dự đoán và tự tạo account Admin khi khởi động: rủi ro rất cao nếu deploy nguyên trạng.
- Email log mask người nhận/người gửi; source không log token kích hoạt/OTP. Exception SMTP vẫn được log.
- Kích hoạt dùng `AccountActivation:AppBaseUrl`, không lấy `Host` request; `Program.cs` validate absolute HTTP/HTTPS và cấm loopback. Điều này chống Host Header Injection, nhưng **cho phép HTTP**, nên token có thể lộ trên mạng.
- Token kích hoạt có lifetime cấu hình và cooldown gửi lại phía server; token mới thay token cũ, gửi lỗi có logic hoàn tác. Không có rate limit theo IP/toàn hệ thống.

## 15. Bảo mật FastAPI và giao tiếp MVC–AI

- Không có API key, bearer token, mTLS hay chữ ký request ở `app/main.py`/router. Mọi endpoint đều public đối với mạng có thể chạm service.
- `/model/train`, `/admin/model/set-active`, `/admin/model/reload`, `DELETE /admin/model/{model_file}`, export/detail/log/system-info có thể gọi trực tiếp, bỏ qua quyền MVC.
- Swagger `/docs`, `/redoc`, OpenAPI được FastAPI mở mặc định; không có cấu hình tắt production.
- `/health` lộ version và tên model đang nạp; `/admin/system-info`/`logs/summary` có thể lộ thêm thông tin vận hành.
- `run.py` bind `0.0.0.0:8001` và `reload=True`; đây không phải cấu hình production an toàn.
- CORS lấy origin từ `ALLOW_ORIGINS`, default hai localhost; `allow_credentials=True`, methods/headers `"*"`. CORS không chặn curl/server-side và không bảo vệ admin API.
- MVC `AiApiService` không gửi credential; base URL hiện dùng HTTP. Service log payload/body lỗi tối đa 4.000 ký tự, có thể ghi dữ liệu dự án/AI nhạy cảm.
- Retry chỉ xảy ra với 5xx/408/429. Vì dùng chung cho mọi method, POST train/reload và DELETE có thể bị lặp khi server đã thực hiện nhưng response lỗi; thao tác không có idempotency key.
- FastAPI không truy cập SQL Server; dataset do MVC gửi. Pydantic `StrictBaseModel` chặn field dư và định kiểu khá chặt, nhưng không có giới hạn request body/rate limit.
- `model_storage.ensure_safe_model_file/model_path` chống path traversal bằng regex, extension và parent check. Tuy nhiên endpoint không auth; kẻ gọi vẫn xóa/kích hoạt các tên model hợp lệ.
- `joblib.load` có khả năng thực thi mã khi file model độc hại. Source không có endpoint upload model trực tiếp, nên khai thác cần quyền ghi filesystem/chuỗi cung ứng; quyền thư mục model phải được giới hạn.

## 16. Nhật ký, giám sát và phản ứng sự cố

| Sự kiện | Hiện trạng |
| --- | --- |
| Login thành công/thất bại/lockout | Chưa thấy audit |
| Yêu cầu quên mật khẩu/OTP/reset/đổi mật khẩu | Chưa thấy audit |
| Đổi email | Chức năng chưa có |
| Đổi role/permission | DB cập nhật, chưa thấy audit bảo mật |
| Kích hoạt/khóa/mở khóa | Có một số log lỗi/cảnh báo, thiếu audit thành công đầy đủ |
| Train/analyze AI | FastAPI `log_service.log_event` có |
| Kích hoạt/xóa model | Không thấy audit tương xứng trong router admin |
| Access denied | Chưa thấy audit |

`AiApiService` log payload request và response lỗi; dataset có thể chứa dữ liệu dự án. `log_service` ghi error text. Cần redact, phân quyền file log, retention/rotation và correlation ID. Không thấy SIEM/cảnh báo/response playbook; không thể kết luận giám sát hạ tầng.

## 17. Danh sách lỗ hổng và rủi ro phát hiện

| Mã | Mức độ | Vấn đề | Bằng chứng source | Khả năng bị khai thác | Ảnh hưởng | Hướng khắc phục |
| --- | --- | --- | --- | --- | --- | --- |
| SEC-01 | Nghiêm trọng | FastAPI admin/model không xác thực, bind mọi interface | `run.py`; `app/main.py`; `admin_router.py`; `model_router.py` | Cao nếu port 8001 reachable | Train/xóa/kích hoạt model, đọc metadata/log/system info; phá tính toàn vẹn AI | Bind loopback/private, API key/HMAC hoặc mTLS, chặn firewall, bảo vệ/tắt docs |
| SEC-02 | Cao | Cookie cũ không bị vô hiệu khi đổi/reset mật khẩu, khóa hoặc đổi quyền | `Program.cs:AddCookie`; `AccountService`; `TaiKhoanCaNhanService`; `NhanSuService`; `PhanQuyenService` | Cao khi cookie bị đánh cắp hoặc user bị thu quyền | Duy trì truy cập tối đa 7 ngày với quyền cũ | Validate stamp/account DB định kỳ; reissue/revoke ticket |
| SEC-03 | Cao | 55 POST thiếu anti-forgery | Danh sách controller ở mục 11/phụ lục kiểm thử | Cao với user đang đăng nhập truy cập trang độc hại | Sửa/xóa/duyệt/upload ngoài ý muốn | Global `AutoValidateAntiforgeryToken` hoặc bổ sung attribute; token AJAX |
| SEC-04 | Cao | Upload nằm trong static webroot, bypass scope download | `Program.cs:UseStaticFiles`; `wwwroot/uploads`; `File*Service` | Cao nếu đoán/nhận URL | Lộ file dự án/minh chứng/avatar | Lưu ngoài webroot, tải qua authorized action |
| SEC-05 | Cao | File dự án không whitelist/size/MIME/signature | `FileDuAnService.UploadAsync` | Cao với project manager có quyền upload | Lưu file độc hại, DoS dung lượng, nội dung nguy hiểm | Giới hạn size, extension, MIME, magic bytes; tên tải an toàn |
| SEC-06 | Cao | Seed tự tạo Admin với credential hard-code dự đoán được | `KhoiTaoTaiKhoanMacDinh.DamBaoTaiKhoanAdminMacDinhAsync/TaoNguoiDungMacDinhAsync`; gọi tại `Program.cs` | Cao nếu deploy DB mới/credential không đổi | Chiếm quyền hệ thống | Không seed password cố định; bootstrap qua secret một lần, buộc đổi |
| SEC-07 | Cao | Không lockout/rate-limit login | `AccountService.AuthenticateAsync` | Cao từ mạng | Brute force/password spraying | Tăng count atomically, lockout, rate limit |
| SEC-08 | Trung bình | Forgot-password không rate limit/cooldown; reset mở khóa account quản trị khóa | `AccountService.KhoiTaoQuenMatKhauAsync/DatLaiMatKhauBangOtpAsync` | Trung bình | Mail bombing, DoS OTP, vượt quyết định khóa | Rate limit IP/account; không xóa khóa quản trị khi reset |
| SEC-09 | Trung bình | Policy reset/đổi chỉ 6 ký tự và không thống nhất kích hoạt | ba ViewModel password | Cao | Mật khẩu yếu | Policy dùng chung tối thiểu 8–12, breached-password tùy chọn |
| SEC-10 | Trung bình | HTTP được phép cho activation và MVC–FastAPI | `Program.cs` validation; `appsettings`; `launchSettings`; `AiApiOptions` | Trung bình trên mạng không tin cậy | Lộ token/payload, MITM | HTTPS hoặc loopback/private network; chỉ cho HTTPS production |
| SEC-11 | Trung bình | Cookie flag không cấu hình tường minh | `Program.cs:AddCookie` | Phụ thuộc môi trường | Cookie gửi qua kênh/chế độ không mong muốn | Đặt `HttpOnly`, `Secure=Always`, `SameSite=Lax/Strict`, tên riêng |
| SEC-12 | Trung bình | FastAPI không giới hạn request/rate; CORS quá rộng về method/header | `app/main.py`, `config.py` | Trung bình | DoS CPU/memory, abuse training | Body limit, rate limit, giới hạn method/header |
| SEC-13 | Trung bình | Retry có thể lặp POST/DELETE không idempotent | `AiApiService.SendForDataAsync/ShouldRetry` | Thấp-trung bình khi timeout/5xx | Train/xóa/reload lặp | Chỉ retry GET/an toàn; idempotency key cho train |
| SEC-14 | Trung bình | Log AI có thể chứa payload/response/error dự án | `AiApiService` warning log; FastAPI `log_service` | Trung bình với người đọc log | Lộ dữ liệu nghiệp vụ | Redact, giảm body log, retention và ACL |
| SEC-15 | Trung bình | Thiếu security headers | không có middleware/header trong `Program.cs` | Trung bình | Tăng tác động XSS/clickjacking/sniffing | CSP, nosniff, frame-ancestors, referrer/permissions policy |
| SEC-16 | Trung bình | `joblib.load` tin file trên disk | `model_storage.load_model` | Cần quyền ghi model dir | Thực thi mã trong process AI | ACL read/write tối thiểu, checksum/sign model, không nhận upload |
| SEC-17 | Thấp | Login tiết lộ trạng thái account | exception khác nhau trong `AuthenticateAsync` | Trung bình | Enumeration username/trạng thái | Thông báo chung; log chi tiết nội bộ |
| SEC-18 | Thấp | Không MIME/signature cho avatar/minh chứng | `TaiKhoanCaNhanService`, `TienDoCongViecService` | Trung bình | File giả mạo/nội dung active | Decode ảnh; kiểm magic bytes/MIME |
| SEC-19 | Thấp | `/health`, docs, system-info lộ metadata | FastAPI routers/default docs | Cao nếu reachable | Hỗ trợ reconnaissance | Giảm health output; hạn chế docs/admin |
| SEC-20 | Thấp | Không audit bảo mật đầy đủ | mục 16 | Không phải khai thác trực tiếp | Khó điều tra/phát hiện | Structured audit không chứa secret/token |
| SEC-21 | Khuyến nghị tăng cường | Không MFA/OTP tăng cường Admin/Manager | không thấy code | N/A | Phòng thủ tài khoản đặc quyền còn mỏng | OTP email trước, TOTP về sau |
| SEC-22 | Khuyến nghị tăng cường | Không quản lý phiên theo thiết bị/đăng xuất tất cả | không thấy store/session UI | N/A | User không tự thu hồi phiên | Stamp/version phiên trước; bảng thiết bị là mở rộng |
| SEC-23 | Khuyến nghị tăng cường | Không kiểm mật khẩu phổ biến/rò rỉ | các ViewModel/service password | N/A | Password spraying hiệu quả hơn | deny-list/breached-password service sau |

Tổng hợp: **1 Nghiêm trọng, 6 Cao, 9 Trung bình, 4 Thấp, 3 Khuyến nghị tăng cường**.

## 18. Đề xuất kiến trúc bảo mật TO-BE

### Giai đoạn 1 – Ưu tiên sửa ngay

1. Cô lập FastAPI: chỉ loopback/private interface; thêm shared API key/HMAC ở MVC và dependency auth ở mọi router quản trị; tắt/restrict docs; không chạy reload production.
2. Sửa phiên: cấu hình cookie tường minh; kiểm stamp/account lock/deleted/role-version; vô hiệu phiên cũ sau reset/đổi/khóa.
3. Áp `AutoValidateAntiforgeryToken` toàn MVC, test 55 POST và AJAX.
4. Thêm login/forgot-password rate limit + lockout; thống nhất policy mật khẩu.
5. Di chuyển upload khỏi `wwwroot`; siết `FileDuAnService`; download chỉ qua action scope.
6. Loại credential seed/connection string khỏi repo, rotate; bắt buộc HTTPS activation và production.

Đây là phần cần thiết cho khóa luận hiện tại và không cần đổi schema nếu dùng security stamp/DB account hiện có, middleware rate limit, global anti-forgery, filesystem ngoài webroot và API key qua secret.

### Giai đoạn 2 – Tăng cường

Email thông báo đổi/reset; OTP cho Admin/Manager; nút đăng xuất mọi thiết bị dựa trên stamp; audit bảo mật; security headers; redact log; idempotency/retry an toàn; kiểm magic bytes. Phần lớn không cần schema; audit có thể dùng structured file/log sink trước.

### Giai đoạn 3 – Mở rộng tương lai

TOTP/MFA, thiết bị tin cậy, cảnh báo IP/thiết bị bất thường, quản lý từng phiên, kiểm mật khẩu rò rỉ. Bảng OTP/phiên thiết bị chỉ là tùy chọn tương lai, không phải yêu cầu chính của lần sửa tới.

## 19. Đề xuất luồng quên mật khẩu an toàn

1. Nhận email/username và luôn trả thông báo chung.
2. Rate limit theo IP + định danh băm; cooldown gửi lại.
3. Chỉ xử lý account tồn tại, `EmailConfirmed`, không soft-delete và không bị khóa quản trị.
4. Sinh OTP/token bằng CSPRNG, lưu hash + expiry + attempt.
5. Gửi qua SMTP; không log token.
6. Nếu dùng link, tạo từ base URL HTTPS tin cậy; với OTP hiện tại giữ mã phiên opaque.
7. Kiểm token/OTP, giới hạn lần sai và one-time.
8. Áp policy mật khẩu chung.
9. Băm mật khẩu, đổi security stamp; không tự mở khóa quản trị.
10. Xóa token reset và vô hiệu cookie cũ.
11. Gửi email thông báo thành công.
12. Ghi audit metadata tối thiểu, không token/OTP.

## 20. Đề xuất luồng đổi mật khẩu an toàn

1. Yêu cầu đăng nhập và account còn hoạt động.
2. Nhập đúng mật khẩu hiện tại.
3. Nhập/xác nhận mật khẩu mới.
4. Xác minh anti-forgery.
5. Áp policy chung, cấm trùng hiện tại.
6. Với Admin/Manager hoặc tín hiệu rủi ro, gửi OTP email có rate limit.
7. Chỉ đổi sau xác minh; đổi security stamp.
8. Phát lại cookie hiện tại nếu quyết định giữ phiên, đồng thời làm cookie khác hết hiệu lực; hoặc buộc đăng nhập lại toàn bộ.
9. Gửi email cảnh báo và ghi audit không chứa mật khẩu.

## 21. Danh sách file có khả năng cần chỉnh sửa ở bước sau

| File | Lý do có thể cần sửa | Nội dung dự kiến |
| --- | --- | --- |
| `QuanLyDuAn/QuanLyDuAn/Program.cs` | Cookie, stamp, CSRF, HTTPS/header/rate limit | Cấu hình cookie; principal validation; global anti-forgery; rate limit/header |
| `Controllers/AccountController.cs` | Thông báo/login/reset và refresh/revoke phiên | Thông báo chung, luồng hậu reset |
| `Services/Implementations/AccountService.cs` | Lockout/rate/state/password/session | Đếm sai, trạng thái deleted/locked, policy chung, không mở khóa khi reset |
| `Controllers/TaiKhoanCaNhanController.cs` | Luồng hậu đổi | Reissue/sign-out và thông báo |
| `Services/Implementations/TaiKhoanCaNhanService.cs` | Policy/stamp/audit | Policy chung, audit |
| Các controller có POST ở mục 11 | CSRF | Attribute hoặc dựa global filter; token AJAX |
| `Services/Implementations/FileDuAnService.cs` | Upload yếu | Size/whitelist/MIME/signature, storage ngoài webroot |
| `TaiKhoanCaNhanService.cs`, `TienDoCongViecService.cs` | Signature/MIME | Xác thực nội dung file |
| `Data/KhoiTaoTaiKhoanMacDinh.cs` | Credential seed | Bootstrap từ secret, buộc đổi; không hard-code |
| `appsettings*.json`, `launchSettings.json` | Secret/HTTP | Placeholder an toàn; URL HTTPS/private |
| `Services/Implementations/AiApiService.cs`, `AiApiOptions.cs` | Auth/retry/log | Header nội bộ, retry idempotent, redact |
| `QuanLyDuAnAIService/app/main.py` | API auth/CORS/docs/limit | Middleware/dependency auth, CORS hẹp, docs production |
| `app/config.py` | Secret/API/CORS | Đọc API key và production flags từ env |
| `app/routers/admin_router.py`, `model_router.py` | Endpoint quản trị public | Dependency authorization/audit |
| `run.py` | Bind/reload | Host cấu hình, `reload=False` production |
| `app/ml/model_storage.py` | Tin cậy artefact | ACL/checksum/signature defense-in-depth |

Không cần repository pattern, microservice mới, schema hay migration cho phương án chính.

## 22. Kịch bản kiểm thử bảo mật cần thực hiện

| Test case | Tiền điều kiện | Các bước | Kết quả mong đợi | Kết quả hiện tại suy ra từ source |
| --- | --- | --- | --- | --- |
| Login sai nhiều lần | Account hoạt động | Sai password > ngưỡng | Lockout + thông báo chung + audit/rate limit | Không lockout tự động; count không tăng |
| Email không tồn tại | Không có email | Gửi ForgotPassword | Thông báo giống account có thật, không gửi mail | Đạt phần thông báo; vẫn tạo mã phiên giả |
| Gửi reset liên tục | Account hợp lệ | Gửi nhiều request | 429/cooldown, không mail bombing | Mỗi request thay token và gửi mail |
| OTP hết hạn | Có OTP, chờ >3 phút | Verify | Từ chối, xóa token | Đạt theo source |
| OTP sai | Có OTP | Nhập sai | Từ chối; khóa token sau ngưỡng | Đạt, 5 lần |
| Token đã dùng | Reset thành công | Dùng lại maPhien | Từ chối | Đạt vì xóa token reset |
| Dùng lại link/session reset | Như trên | GET/POST lại | Từ chối | Đạt |
| Đổi password sai mật khẩu cũ | Đang login | POST sai current | Không đổi | Đạt |
| CSRF đổi password | Đang login | POST thiếu token | 400 | Đạt tại `DoiMatKhau` |
| CSRF action dự án/duyệt | Đang login | POST chéo origin thiếu token | 400 | Nhiều action hiện có thể được xử lý vì thiếu validation |
| Cookie cũ sau reset/đổi | Hai browser đã login | Đổi/reset ở A, dùng B | B bị buộc login lại | B dự kiến vẫn hợp lệ |
| Cookie cũ sau khóa | User đang login, Admin khóa | Tiếp tục request | Bị từ chối ngay/chu kỳ ngắn | Dự kiến vẫn hợp lệ |
| Thu hồi role/permission | User đang login | Admin thu quyền; user gọi lại | Bị từ chối | Cookie giữ claim cũ |
| User gọi action Admin | User thường | Gọi NhanSu/PhanQuyen/AI admin MVC | 403 | Nhiều action trọng yếu đạt nhờ permission; phải test toàn ma trận |
| File dự án khác qua action | Thành viên dự án A | Gọi `TaiFileDuAn` file B | 403/404 | Controller có kiểm scope, cần runtime test |
| File qua URL static | Biết `/uploads/...` | GET trực tiếp không cookie | 403/404 | Dự kiến 200 vì `UseStaticFiles` |
| Gọi FastAPI admin trực tiếp | Reach port 8001 | DELETE/set-active/train | 401/403 | Hiện xử lý không auth |
| Upload giả extension | Có quyền upload | Nội dung script/ZIP đổi `.jpg`/`.pdf` | Từ chối signature | Avatar/minh chứng dự kiến chấp nhận nếu extension/size đúng |
| Upload file dự án executable | Project manager | Upload extension nguy hiểm | Từ chối | Dự kiến chấp nhận |
| Upload vượt dung lượng | Có quyền | >2 MB avatar, >10 MB evidence, file dự án rất lớn | Từ chối mọi loại | Avatar/evidence đạt; file dự án không giới hạn |
| Path traversal upload | Có quyền | filename `../x` | Không thoát thư mục | Tên được basename + GUID, dự kiến đạt |
| Path traversal model | Reach FastAPI | model_file `../x.joblib` | 400 | Đạt nhờ regex/parent check |
| Body AI quá lớn | Reach FastAPI | Dataset cực lớn | 413/rate limit | Chưa có giới hạn ứng dụng |
| Retry train | Làm response 5xx sau commit | MVC gọi train | Không tạo trùng | Có thể lặp do retry |

## 23. Kết luận cuối tài liệu

Hệ thống đã có các lớp nền tảng đáng ghi nhận: PasswordHasher, email-confirmed activation token mạnh và một lần, OTP reset có hạn/giới hạn sai, cookie authentication, role + permission claim, nhiều kiểm tra scope, EF Core LINQ, anti-forgery ở các luồng tài khoản, và kiểm tên model chống path traversal.

Điểm mạnh nhất là luồng kích hoạt và OTP không lưu secret rõ. Lỗ hổng quan trọng nhất là FastAPI quản trị không xác thực và có thể bind ra mạng; tiếp theo là cookie/claim cũ không bị thu hồi, CSRF rộng và upload static bypass authorization.

- Quên mật khẩu: **không cần thêm bước xác nhận email trùng lặp**, vì OTP email đã xác minh quyền sở hữu. Cần rate limit, kiểm trạng thái account, policy mạnh, revoke phiên và email hậu đổi.
- Đổi mật khẩu: user thường cần mật khẩu hiện tại như hiện nay; Admin/Manager nên thêm OTP email vừa đủ, không cần MFA phức tạp ngay.
- Ba ưu tiên đầu: **(1) khóa và xác thực FastAPI; (2) sửa vòng đời cookie/security stamp và lockout; (3) áp anti-forgery toàn cục đồng thời đưa upload khỏi `wwwroot`/siết `FileDuAnService`.**
