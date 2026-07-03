# Phân tích chức năng đăng nhập và ghi nhớ đăng nhập

## 1. Tổng quan luồng đăng nhập

Luồng đăng nhập hiện tại là luồng cookie authentication tự xử lý, không phải luồng ASP.NET Core Identity đầy đủ qua `SignInManager`.

1. Người dùng mở `/Account/Login`.
2. `QuanLyDuAn/QuanLyDuAn/Controllers/AccountController.cs`, class `AccountController`, method `Login(string? returnUrl = null)` GET, dòng 21-32:
   - Nếu `User.Identity.IsAuthenticated == true` thì redirect về `Dashboard/Index`.
   - Gán `ViewData["ReturnUrl"] = returnUrl`.
   - Trả về `View(new DangNhapViewModel())`.
3. `QuanLyDuAn/QuanLyDuAn/Views/Account/Login.cshtml`, dòng 45-72, hiển thị form POST tới action `Login`.
4. Checkbox "Ghi nhớ đăng nhập" nằm ở `Login.cshtml`, dòng 65-66, bind bằng `asp-for="GhiNhoDangNhap"`.
5. Khi submit, `AccountController.Login(DangNhapViewModel model, string? returnUrl = null)` POST, dòng 34-71, nhận model và `returnUrl`.
6. Controller kiểm tra `ModelState.IsValid` tại dòng 40-43.
7. Controller gọi `_accountService.AuthenticateAsync(model.TenDangNhap, model.MatKhau)` tại dòng 47.
8. `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AccountService.cs`, class `AccountService`, method `AuthenticateAsync(string userName, string password)`, dòng 34-167:
   - Tìm tài khoản theo `NormalizedUserName` hoặc `UserName`, dòng 36-49.
   - Chặn hồ sơ đã xóa, dòng 51-54.
   - Chặn tài khoản bị khóa qua `LockoutEnabled`/`LockoutEnd`, dòng 56-63.
   - Chặn tài khoản chưa kích hoạt qua `EmailConfirmed`, dòng 65-68.
   - Kiểm tra `PasswordHash` và mật khẩu bằng `PasswordHasher<Aspnetusers>`, dòng 70-79.
   - Tạo `ClaimsPrincipal`, dòng 81-167.
9. Controller tạo cookie bằng `HttpContext.SignInAsync(...)`, dòng 49-57:
   - Scheme: `CookieAuthenticationDefaults.AuthenticationScheme`.
   - `AuthenticationProperties.IsPersistent = model.GhiNhoDangNhap`.
   - `AuthenticationProperties.AllowRefresh = true`.
   - `AuthenticationProperties.ExpiresUtc = model.GhiNhoDangNhap ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8)`.
10. Sau đăng nhập thành công, controller redirect về `returnUrl` nếu local, dòng 65-68; nếu không có `returnUrl`, redirect `Dashboard/Index`, dòng 70.

Không tìm thấy `SignInManager.PasswordSignInAsync` trong project. Nơi đã kiểm tra: toàn bộ `QuanLyDuAn/QuanLyDuAn` bằng tìm kiếm `PasswordSignInAsync`, `SignInManager`, `AddIdentity`, `AddDefaultIdentity`, `ConfigureApplicationCookie`.

## 2. Danh sách file liên quan

| Nhóm | File | Class/Method/Biến liên quan | Kết luận |
|---|---|---|---|
| Controller đăng nhập | `QuanLyDuAn/QuanLyDuAn/Controllers/AccountController.cs` | `AccountController`, GET `Login`, POST `Login`, POST `Logout`, `_accountService` | File chính xử lý login/logout |
| View Login | `QuanLyDuAn/QuanLyDuAn/Views/Account/Login.cshtml` | `@model DangNhapViewModel`, form `asp-action="Login"`, checkbox `asp-for="GhiNhoDangNhap"` | View bind đúng vào ViewModel |
| ViewModel Login | `QuanLyDuAn/QuanLyDuAn/ViewModels/Auth/DangNhapViewModel.cs` | `TenDangNhap`, `MatKhau`, `GhiNhoDangNhap` | Có property bool cho checkbox |
| Service đăng nhập | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AccountService.cs` | `AccountService.AuthenticateAsync` | Xác thực user/password, lock, activation, role/claim |
| Interface service | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IAccountService.cs` | `AuthenticateAsync(string userName, string password)` | Không nhận RememberMe; RememberMe xử lý ở controller |
| Cookie config | `QuanLyDuAn/QuanLyDuAn/Program.cs` | `AddAuthentication().AddCookie(...)` | Cấu hình cookie 8 giờ, sliding expiration |
| Tag helper | `QuanLyDuAn/QuanLyDuAn/Views/_ViewImports.cshtml` | `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers` | Bật tag helper cho `asp-for` |
| Logout UI | `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml` | form `asp-controller="Account" asp-action="Logout"` | Logout là POST có anti-forgery |
| JavaScript | `QuanLyDuAn/QuanLyDuAn/wwwroot/js/...` | `site.js`, `layout/sidebar.js`, `ChatDuAn/chat.js`, `approval/index.js`, `phanquyen/index.js`, `ai/charts.js` | Không tìm thấy JS riêng xử lý form login hoặc gán `GhiNhoDangNhap` |

## 3. Phân tích View Login

File: `QuanLyDuAn/QuanLyDuAn/Views/Account/Login.cshtml`.

- Dòng 1 khai báo model: `QuanLyDuAn.ViewModels.Auth.DangNhapViewModel`.
- Dòng 45 khai báo form: `<form asp-action="Login" method="post" class="auth-form">`.
- Dòng 46 có `@Html.AntiForgeryToken()`.
- Dòng 47 có hidden input `name="returnUrl"` lấy từ `ViewData["ReturnUrl"]`.
- Dòng 52-54 bind `TenDangNhap` bằng `asp-for`.
- Dòng 58-60 bind `MatKhau` bằng `asp-for`.
- Dòng 65 bind checkbox bằng `<input asp-for="GhiNhoDangNhap" class="form-check-input" />`.
- Dòng 66 bind label bằng `<label asp-for="GhiNhoDangNhap" class="form-check-label"></label>`.

Kết luận bind checkbox:

- Checkbox có dùng `asp-for`: Có, property `GhiNhoDangNhap`.
- `name/id` theo source sẽ do tag helper sinh từ property `GhiNhoDangNhap`. Với model trực tiếp `DangNhapViewModel`, tên kỳ vọng là `name="GhiNhoDangNhap"` và `id="GhiNhoDangNhap"`.
- Hidden input đi kèm checkbox không hiện trực tiếp trong source Razor, nhưng do `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers` ở `Views/_ViewImports.cshtml` dòng 3, `input asp-for` cho bool sẽ được MVC tag helper render thành checkbox và hidden fallback cùng tên để gửi `false` khi không tick.
- Label dùng cùng `asp-for="GhiNhoDangNhap"`, nên `for` kỳ vọng khớp `id="GhiNhoDangNhap"`.
- Form dùng POST đúng action `Login`. Không khai báo `asp-controller`, nhưng view nằm trong `Views/Account`, nên post về `Account/Login`.
- Không thấy JavaScript/AJAX xử lý form login trong view; view chỉ load `jquery` và `_ValidationScriptsPartial` ở dòng 77-78.

Kết luận: checkbox có bind đúng. Khi tick, server có cơ sở nhận `GhiNhoDangNhap = true`; khi không tick, hidden fallback/tag helper và model binder có cơ sở nhận `false`. Không thấy nguy cơ name mismatch trong source hiện tại.

## 4. Phân tích ViewModel Login

File: `QuanLyDuAn/QuanLyDuAn/ViewModels/Auth/DangNhapViewModel.cs`, class `DangNhapViewModel`.

| Property | Dòng | Kiểu | Attribute | Ghi chú |
|---|---:|---|---|---|
| `TenDangNhap` | 7-9 | `string` | `[Required]`, `[Display(Name = "Tên đăng nhập")]` | Dùng ở view và controller |
| `MatKhau` | 11-14 | `string` | `[Required]`, `[DataType(DataType.Password)]`, `[Display(Name = "Mật khẩu")]` | Dùng ở view và controller |
| `GhiNhoDangNhap` | 16-17 | `bool` | `[Display(Name = "Ghi nhớ đăng nhập")]` | Dùng trực tiếp cho checkbox và `IsPersistent` |

Không tìm thấy `ReturnUrl` trong `DangNhapViewModel`. `returnUrl` hiện đi bằng hidden input `name="returnUrl"` ở `Login.cshtml` dòng 47 và tham số action `string? returnUrl = null` ở `AccountController.cs` dòng 36.

Không tìm thấy `[BindNever]`, ignore attribute, thiếu getter/setter, hoặc kiểu sai trên `GhiNhoDangNhap`. Property là `bool` có `{ get; set; }`.

## 5. Phân tích Controller Login

File: `QuanLyDuAn/QuanLyDuAn/Controllers/AccountController.cs`, class `AccountController`.

GET Login:

- Method: `public IActionResult Login(string? returnUrl = null)`, dòng 21-22.
- Nếu đã authenticated thì redirect `Dashboard/Index`, dòng 24-27.
- Gán `ViewData["ReturnUrl"]`, dòng 29.
- Trả về `new DangNhapViewModel()`, dòng 31.

POST Login:

- Method: `public async Task<IActionResult> Login(DangNhapViewModel model, string? returnUrl = null)`, dòng 34-36.
- Có `[HttpPost]`, dòng 34.
- Có `[ValidateAntiForgeryToken]`, dòng 35.
- Có kiểm tra `ModelState.IsValid`, dòng 40-43.
- Gọi service xác thực bằng `model.TenDangNhap`, `model.MatKhau`, dòng 47.
- Không dùng `PasswordSignInAsync`.
- Không dùng `SignInManager.SignInAsync`.
- Dùng `HttpContext.SignInAsync` với `AuthenticationProperties`, dòng 49-57.

Dòng code quyết định RememberMe:

```csharp
IsPersistent = model.GhiNhoDangNhap,
AllowRefresh = true,
ExpiresUtc = model.GhiNhoDangNhap ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8)
```

Kết luận controller:

- Controller có đọc giá trị `model.GhiNhoDangNhap`.
- Controller có truyền giá trị này vào `AuthenticationProperties.IsPersistent`.
- Không thấy gán lại `GhiNhoDangNhap = false` trước khi đăng nhập.
- Điểm dễ gây hiểu nhầm: cả hai nhánh đều set `ExpiresUtc`. Nhánh không tick có ticket hết hạn sau 8 giờ; nhánh tick có ticket hết hạn sau 7 ngày. Tuy nhiên cookie browser chỉ persistent khi `IsPersistent = true`.

## 6. Phân tích cấu hình Cookie/Identity

File: `QuanLyDuAn/QuanLyDuAn/Program.cs`.

Cấu hình authentication/cookie ở dòng 25-33:

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
```

Kết quả kiểm tra:

| Cấu hình | Có/Không | Giá trị hiện tại |
|---|---|---|
| `AddAuthentication` | Có | Scheme `CookieAuthenticationDefaults.AuthenticationScheme` |
| `AddCookie` | Có | Dòng 26 |
| `LoginPath` | Có | `/Account/Login` |
| `LogoutPath` | Có | `/Account/Logout` |
| `AccessDeniedPath` | Có | `/Account/AccessDenied` |
| `ExpireTimeSpan` | Có | `TimeSpan.FromHours(8)` |
| `SlidingExpiration` | Có | `true` |
| `Cookie.Name` | Không tìm thấy | Dùng mặc định của cookie middleware |
| `Cookie.HttpOnly` | Không tìm thấy cấu hình tường minh | Dùng mặc định middleware |
| `Cookie.SecurePolicy` | Không tìm thấy cấu hình tường minh | Dùng mặc định middleware |
| `Cookie.SameSite` | Không tìm thấy cấu hình tường minh | Dùng mặc định middleware |
| `AddIdentity`/`AddDefaultIdentity` | Không tìm thấy | Không dùng đầy đủ Identity sign-in |
| `ConfigureApplicationCookie` | Không tìm thấy | Cấu hình nằm trong `AddCookie` |

Phân tích persistent cookie:

- Nếu `GhiNhoDangNhap = false`: `AccountController` đặt `IsPersistent = false` và `ExpiresUtc = UtcNow + 8 giờ`. Với cookie authentication, `IsPersistent = false` thường làm cookie trình duyệt là session cookie, tức header `Set-Cookie` không có `Expires`/`Max-Age`, dù ticket bên trong vẫn có hạn 8 giờ.
- Nếu `GhiNhoDangNhap = true`: `IsPersistent = true` và `ExpiresUtc = UtcNow + 7 ngày`; cookie trình duyệt kỳ vọng có `Expires` hoặc `Max-Age`, nên tồn tại sau khi đóng/mở lại trình duyệt đến khi hết hạn hoặc logout.
- `SlidingExpiration = true` trong `Program.cs` dòng 31 và `AllowRefresh = true` trong `AccountController.cs` dòng 55 cho phép middleware gia hạn ticket khi người dùng hoạt động, theo cơ chế cookie authentication.
- Không thấy middleware tùy chỉnh hoặc cấu hình khác ghi đè `IsPersistent`.

Nguyên nhân khiến cảm giác giống nhau:

- Reload tab hoặc đóng/mở tab trong cùng phiên trình duyệt thường không phân biệt rõ session cookie và persistent cookie.
- Nhánh không tick vẫn có ticket hết hạn 8 giờ, nên nếu trình duyệt vẫn giữ session thì người dùng vẫn đăng nhập trong khoảng này.
- Nhiều trình duyệt có tính năng khôi phục phiên sau khi đóng/mở lại; cần đóng toàn bộ trình duyệt và kiểm tra DevTools `Expires/Max-Age` để phân biệt.
- Bấm Logout luôn xóa cookie, nên tick hay không tick đều phải đăng nhập lại sau logout. Đây là đúng.

## 7. Phân tích Logout

Controller logout:

- File: `QuanLyDuAn/QuanLyDuAn/Controllers/AccountController.cs`.
- Method: `public async Task<IActionResult> Logout()`, dòng 73-80.
- Có `[Authorize]`, dòng 73.
- Có `[HttpPost]`, dòng 74.
- Có `[ValidateAntiForgeryToken]`, dòng 75.
- Gọi `await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);`, dòng 78.
- Redirect về `Login`, dòng 79.

UI logout:

- File: `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml`.
- Form logout ở dòng 355-361.
- Form dùng `asp-controller="Account"`, `asp-action="Logout"`, `method="post"`.
- Có `@Html.AntiForgeryToken()` ở dòng 356.

Kết luận logout:

- Logout xóa cookie đúng scheme cookie authentication.
- Không thấy `HttpContext.Session.Clear()` vì project không thấy cấu hình session cho login.
- Nếu người dùng bấm "Đăng xuất" thì dù đã tick RememberMe, cookie vẫn bị xóa và lần sau phải đăng nhập lại. Đây là hành vi đúng, không phải lỗi RememberMe.

## 8. Phân tích JavaScript liên quan nếu có

Đã kiểm tra các nơi sau:

- `QuanLyDuAn/QuanLyDuAn/Views/Account/Login.cshtml`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/site.js`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/layout/sidebar.js`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/ChatDuAn/chat.js`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/approval/index.js`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/phanquyen/index.js`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/ai/charts.js`.
- Toàn bộ `QuanLyDuAn/QuanLyDuAn/Views` với các từ khóa `auth-form`, `GhiNhoDangNhap`, `Account/Login`, `submit`, `serialize`, `ajax`, `fetch`, `FormData`, `preventDefault`.

Không tìm thấy JavaScript riêng cho form đăng nhập. Không tìm thấy AJAX login, `serialize()` login, `fetch()` login, reset form trước submit, hoặc logic tự set `GhiNhoDangNhap = false`.

Các `fetch` tìm thấy thuộc module chat, không thuộc login: `QuanLyDuAn/QuanLyDuAn/wwwroot/js/ChatDuAn/chat.js`.

## 9. Nguyên nhân khả nghi

### Kết luận nhanh

| Hạng mục | Kết luận | Mức chắc chắn |
|---|---|---|
| Checkbox có bind đúng không | Có. View dùng `asp-for="GhiNhoDangNhap"` và ViewModel có `bool GhiNhoDangNhap` | Cao |
| Server có nhận true/false không | Source cho thấy model binder có đủ điều kiện nhận đúng true/false | Cao |
| RememberMe có được truyền vào sign-in không | Có. `IsPersistent = model.GhiNhoDangNhap` | Cao |
| Có dùng `PasswordSignInAsync` không | Không tìm thấy | Cao |
| Cookie có persistent khi tick không | Source đặt `IsPersistent = true` khi tick; cần xác nhận bằng response header `Set-Cookie` runtime | Trung bình đến cao |
| Cookie không tick là session cookie không | Source đặt `IsPersistent = false`; cần xác nhận bằng `Set-Cookie` không có `Expires/Max-Age` runtime | Trung bình đến cao |
| Lỗi ở View/ViewModel/Controller/JS | Chưa thấy lỗi bind hoặc JS làm mất checkbox | Cao |
| Lý do người dùng thấy giống nhau | Cách test có thể chưa đúng: reload tab/đóng tab/logout không phải phép thử phân biệt RememberMe; không tick vẫn có ticket 8 giờ trong phiên trình duyệt | Cao |

Dòng code khả nghi nhất cần kiểm runtime:

```csharp
ExpiresUtc = model.GhiNhoDangNhap ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8)
```

Dòng này không sai về mặt bảo mật mặc định, nhưng làm nhánh không tick vẫn có thời hạn ticket 8 giờ. Trải nghiệm trong cùng phiên trình duyệt vì vậy có thể giống tick RememberMe nếu chỉ reload, đóng tab, hoặc mở lại nhanh mà session browser chưa kết thúc. Điểm phân biệt thật sự phải nằm ở `Set-Cookie` có/không có `Expires` hoặc `Max-Age`.

## 10. Đề xuất hướng sửa sau khi xác nhận

Chưa sửa code trong lần rà soát này.

Nếu xác nhận bằng DevTools thấy checkbox không gửi đúng:

- Sửa view để giữ `asp-for="GhiNhoDangNhap"` hoặc đảm bảo `name="GhiNhoDangNhap"` khớp ViewModel.
- Không đổi sang `RememberMe` nếu ViewModel vẫn là `GhiNhoDangNhap`, trừ khi đổi đồng bộ cả ViewModel và Controller.

Nếu xác nhận ViewModel thiếu/sai property:

- Giữ property dạng `public bool GhiNhoDangNhap { get; set; }`.
- Tránh `[BindNever]` hoặc property chỉ có getter.

Nếu xác nhận controller không dùng RememberMe trong một nhánh khác:

- Bảo đảm `AuthenticationProperties.IsPersistent = model.GhiNhoDangNhap`.
- Nếu dùng `SignInManager` sau này, truyền `isPersistent: model.GhiNhoDangNhap`.

Nếu muốn cấu hình cookie rõ ràng hơn:

- Cân nhắc cấu hình tường minh trong `AddCookie`:
  - `options.Cookie.Name`.
  - `options.Cookie.HttpOnly = true`.
  - `options.Cookie.SecurePolicy` phù hợp môi trường.
  - `options.Cookie.SameSite`.
  - `options.ExpireTimeSpan`.
  - `options.SlidingExpiration`.

Nếu không có lỗi code mà chỉ do cách hiểu:

- Giải thích cho người dùng: RememberMe chỉ khác biệt rõ khi đóng toàn bộ trình duyệt rồi mở lại hoặc khi kiểm `Set-Cookie`. Nó không tạo khác biệt khi reload trang, đóng/mở tab trong cùng phiên, hoặc sau khi bấm Logout.

## 11. Test case kiểm thử

| Mã test | Tình huống | Dữ liệu nhập | Checkbox RememberMe | Kết quả mong đợi | Cách kiểm tra | Ghi chú |
|---|---|---|---|---|---|---|
| DN-01 | Đăng nhập đúng, không tick RememberMe | Tài khoản hợp lệ, mật khẩu đúng | Không tick | Đăng nhập thành công, redirect `returnUrl` local hoặc `Dashboard/Index`; cookie trình duyệt là session cookie | DevTools -> Application -> Cookies hoặc Network -> response `Set-Cookie` | Kỳ vọng không có `Expires/Max-Age` trên cookie browser |
| DN-02 | Đăng nhập đúng, tick RememberMe | Tài khoản hợp lệ, mật khẩu đúng | Tick | Đăng nhập thành công; cookie persistent | DevTools Network so sánh `Set-Cookie` | Kỳ vọng có `Expires` hoặc `Max-Age`, thời hạn khoảng 7 ngày |
| DN-03 | Đăng nhập sai mật khẩu | Tên đăng nhập đúng, mật khẩu sai | Tick hoặc không tick | Không tạo cookie đăng nhập; hiển thị lỗi từ `AccountService.AuthenticateAsync` | Network không có auth cookie mới; vẫn ở Login | Service ném lỗi tại kiểm tra `PasswordHasher` |
| DN-04 | Đăng nhập đúng rồi bấm Logout | Tài khoản hợp lệ | Tick hoặc không tick | Cookie bị xóa, redirect về Login, lần sau phải đăng nhập lại | DevTools xem cookie bị expire/delete; truy cập Dashboard bị redirect Login | Đây là hành vi đúng |
| DN-05 | Đăng nhập có tick RememberMe rồi đóng toàn bộ trình duyệt và mở lại | Tài khoản hợp lệ | Tick | Vẫn còn đăng nhập nếu cookie chưa hết hạn và không logout | Đóng toàn bộ process trình duyệt, mở lại URL Dashboard | Cẩn thận chế độ browser profile/incognito |
| DN-06 | Đăng nhập không tick RememberMe rồi đóng toàn bộ trình duyệt và mở lại | Tài khoản hợp lệ | Không tick | Phải đăng nhập lại nếu session cookie bị xóa khi kết thúc browser session | Đóng toàn bộ process trình duyệt, mở lại URL Dashboard | Nếu browser khôi phục session, cần kiểm thêm `Set-Cookie` |
| DN-07 | So sánh Set-Cookie của 2 trường hợp tick và không tick | Tài khoản hợp lệ | Tick và không tick | Tick có `Expires/Max-Age`; không tick không có `Expires/Max-Age` | DevTools -> Network -> request POST `/Account/Login` -> Response Headers | Đây là test quyết định persistent cookie |
| DN-08 | Kiểm tra returnUrl sau đăng nhập | Truy cập trang cần auth để sinh `ReturnUrl`, sau đó login | Tick hoặc không tick | Redirect về `returnUrl` nếu `Url.IsLocalUrl(returnUrl)` true | Network/URL sau login | Controller xử lý tại dòng 65-68 |
| DN-09 | Kiểm tra tài khoản bị khóa hoặc chưa kích hoạt | User `LockoutEnd > UtcNow` hoặc `EmailConfirmed = false` | Tick hoặc không tick | Không đăng nhập, không tạo cookie | Dùng dữ liệu test có trạng thái khóa/chưa kích hoạt | Logic ở `AccountService.AuthenticateAsync` dòng 56-68 |
| DN-10 | Kiểm tra checkbox bị bỏ qua nếu submit bằng JavaScript/AJAX nếu có | Form login hiện tại | Tick | Không áp dụng vì hiện không thấy AJAX login | Tìm source JS và quan sát Network request form POST thường | Nếu sau này thêm AJAX, cần đảm bảo gửi `GhiNhoDangNhap` |

### Hướng dẫn test thực tế bằng trình duyệt

Test A: Không tick "Ghi nhớ đăng nhập"

1. Mở DevTools -> Application -> Cookies.
2. Đăng nhập không tick.
3. Kiểm tra cookie đăng nhập.
4. Kỳ vọng: cookie không có `Expires/Max-Age` rõ ràng, là session cookie.
5. Đóng toàn bộ trình duyệt rồi mở lại.
6. Kỳ vọng: phải đăng nhập lại.

Test B: Tick "Ghi nhớ đăng nhập"

1. Xóa cookie cũ.
2. Đăng nhập có tick.
3. Kiểm tra `Set-Cookie`.
4. Kỳ vọng: cookie có `Expires/Max-Age`.
5. Đóng toàn bộ trình duyệt rồi mở lại.
6. Kỳ vọng: vẫn còn đăng nhập nếu cookie chưa hết hạn.

Test C: Bấm Đăng xuất

1. Đăng nhập có tick RememberMe.
2. Bấm Đăng xuất.
3. Kỳ vọng: cookie bị xóa và phải đăng nhập lại.
4. Đây là hành vi đúng, không phải lỗi.

Test D: So sánh response header

1. Dùng Network tab.
2. So sánh response header `Set-Cookie` giữa login có tick và không tick.
3. Ghi lại khác biệt `Expires/Max-Age` nếu có.

## 12. Checklist thông tin cần gửi lại cho ChatGPT

1. File Controller đăng nhập: `QuanLyDuAn/QuanLyDuAn/Controllers/AccountController.cs`.
2. Method GET Login: `AccountController.Login(string? returnUrl = null)`, dòng 21-32.
3. Method POST Login: `AccountController.Login(DangNhapViewModel model, string? returnUrl = null)`, dòng 34-71.
4. File View Login.cshtml: `QuanLyDuAn/QuanLyDuAn/Views/Account/Login.cshtml`.
5. File ViewModel đăng nhập: `QuanLyDuAn/QuanLyDuAn/ViewModels/Auth/DangNhapViewModel.cs`.
6. Property checkbox RememberMe hiện tại: `DangNhapViewModel.GhiNhoDangNhap`, kiểu `bool`, dòng 16-17.
7. Dòng code gọi `PasswordSignInAsync` hoặc `SignInAsync`: không tìm thấy `PasswordSignInAsync`; dùng `HttpContext.SignInAsync` tại `AccountController.cs` dòng 49-57.
8. Cấu hình cookie trong Program.cs/Startup.cs: `QuanLyDuAn/QuanLyDuAn/Program.cs` dòng 25-33.
9. Action Logout: `AccountController.Logout()`, dòng 73-80; UI logout ở `Views/Shared/_Layout.cshtml` dòng 355-361.
10. Kết luận: chưa thấy lỗi code ở View/ViewModel/Controller/JavaScript; khả năng cao là do cách test chưa phân biệt session cookie và persistent cookie, hoặc cần xác nhận bằng `Set-Cookie`.
11. Ảnh hoặc nội dung `Set-Cookie` khi không tick RememberMe: chưa có trong source; cần lấy bằng DevTools/Network runtime.
12. Ảnh hoặc nội dung `Set-Cookie` khi tick RememberMe: chưa có trong source; cần lấy bằng DevTools/Network runtime.
