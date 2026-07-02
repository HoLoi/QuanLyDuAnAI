# KIỂM TRA TỔNG QUÁT NGHIỆP VỤ HỆ THỐNG

> **Trạng thái tài liệu:** AS-IS theo source tại ngày 02/07/2026.  
> **Phạm vi:** `QuanLyDuAn/QuanLyDuAn/`, `QuanLyDuAnAIService/`, `README.md` và các tài liệu trong `docs/`.  
> **Nguyên tắc:** source hiện tại là nguồn sự thật; không suy luận từ tên Controller hoặc tài liệu cũ.  
> **Phạm vi thay đổi:** chỉ ghi đè tài liệu này; không sửa source, CSDL hoặc migration.

---

## Mục lục

1. [Phương pháp và thống kê phạm vi](#1-phương-pháp-và-thống-kê-phạm-vi)
2. [Kiến trúc và module AS-IS](#2-kiến-trúc-và-module-as-is)
3. [Xác thực, tài khoản và phiên đăng nhập](#3-xác-thực-tài-khoản-và-phiên-đăng-nhập)
4. [Permission, role và scope](#4-permission-role-và-scope)
5. [Kiểm soát backend và trạng thái khóa dữ liệu](#5-kiểm-soát-backend-và-trạng-thái-khóa-dữ-liệu)
6. [Anti-forgery, action GET và model binding](#6-anti-forgery-action-get-và-model-binding)
7. [Upload, download và export](#7-upload-download-và-export)
8. [Soft delete và nhật ký](#8-soft-delete-và-nhật-ký)
9. [Transaction và duyệt đồng thời](#9-transaction-và-duyệt-đồng-thời)
10. [Đối chiếu AI MVC–FastAPI](#10-đối-chiếu-ai-mvcfastapi)
11. [Danh sách phát hiện chuẩn hóa](#11-danh-sách-phát-hiện-chuẩn-hóa)
12. [Kết luận tổng thể](#12-kết-luận-tổng-thể)
13. [Danh sách ưu tiên xử lý](#13-danh-sách-ưu-tiên-xử-lý)
14. [Phụ lục permission](#14-phụ-lục-permission)
15. [Thống kê cuối tài liệu](#15-thống-kê-cuối-tài-liệu)

---

## 1. Phương pháp và thống kê phạm vi

### 1.1 Nguồn đã đọc

- Khởi động và cấu hình: `QuanLyDuAn/QuanLyDuAn/Program.cs`, `Services/CauHinhDichVu.cs`.
- 30 file `Controllers/*Controller.cs`.
- Toàn bộ `Services/Interfaces/`, `Services/Implementations/`, `Services/Exporting/`.
- `Constants/`, `Helpers/`, `Data/`, `Models/Entities/`, `ViewModels/`.
- 126 file `Views/**/*.cshtml`, 5 file `wwwroot/js/**/*.js`, 57 file `wwwroot/css/**/*.css`.
- Toàn bộ mã Python trong `QuanLyDuAnAIService/app/`, cấu hình, sample và metadata model.
- `README.md` và các tài liệu liên quan trong `docs/`.

Các thư mục sinh tự động `bin/`, `obj/`, `.git/`, cache Python không được tính là source nghiệp vụ. Migration chỉ được đọc để đối chiếu constraint; không chạy và không thay đổi.

### 1.2 Phương pháp đếm

| Hạng mục | Tổng số | Đã kiểm tra | Chưa kiểm tra | Ghi chú |
|---|---:|---:|---:|---|
| Controller | 30 | 30 | 0 | Đếm `Controllers/*Controller.cs` |
| Module nghiệp vụ | 29 | 29 | 0 | 30 Controller trừ `HomeController` hạ tầng |
| View `.cshtml` | 126 | 126 | 0 | Đếm đệ quy dưới `Views/` |
| JavaScript | 5 | 5 | 0 | Đếm đệ quy dưới `wwwroot/js/`; script inline được đọc nhưng không tính là file |
| CSS | 57 | 57 | 0 | Đếm đệ quy dưới `wwwroot/css/` |
| Action MVC | 160 | 160 | 0 | Public action trả `IActionResult`/`Task<IActionResult>` |
| GET | 73 | 73 | 0 | Gồm 69 `[HttpGet]` và 4 action theo convention |
| POST | 87 | 87 | 0 | Đếm `[HttpPost]` |
| Permission constant | 81 | 81 | 0 | Không tính `ClaimTypesCustom.Permission` vì đây là tên claim |

Không tiếp tục dùng số “khoảng 187 nút”: nút HTML, link, form submit, partial và thao tác JavaScript không có một định nghĩa đếm ổn định. Tài liệu thay bằng ma trận action/backend có thể kiểm chứng.

### 1.3 Hiệu chỉnh tài liệu cũ

Qua đối chiếu có **18 nội dung sai hoặc không còn đúng đã được sửa**, nổi bật:

1. 73 permission → 81 constant, 80 giá trị unique.
2. 70 POST ước tính → 87 POST chính xác.
3. Trạng thái chi tiết công việc không nhận từ form khi lưu.
4. Admin bị service chat chặn cả đọc và gửi.
5. Nút xóa nhân sự hiện kiểm tra đúng `NhanSu.Xoa`.
6. `NhatKy.Xem` được dùng tại tab hoạt động trong `DuAn/Details`.
7. `ChiPhi.Xem` được dùng gián tiếp ở Dashboard/chi tiết dự án.
8. `PermissionHelper` đọc claim trong cookie, không truy vấn DB mỗi request.
9. View file dùng action tải có kiểm quyền, đồng thời file vật lý vẫn có URL static.
10. Tiến độ có kiểm extension và 10 MB; file dự án thì không.
11. Admin seed có `Chat.Xem`, nhưng role Admin bị service loại khỏi phòng.
12. Khóa tài khoản có chặn tự khóa và Admin cuối cùng ở backend.
13. Xóa nhân sự là soft delete hồ sơ, không xóa Identity account.
14. Duyệt công việc/ngân sách/đổi quản lý dùng `Serializable`.
15. Duyệt tiến độ dùng transaction mặc định, không phải `Serializable`.
16. Không có GET thay đổi dữ liệu được chứng minh.
17. CSV có BOM UTF-8 nhưng chưa trung hòa ký tự mở đầu công thức.
18. Bảng tổng hợp lỗi cũ cộng sai; toàn bộ mã và số lượng đã được tạo lại.

Đã bổ sung **10 nhóm nội dung còn thiếu**: bảo mật tài khoản; kiểm soát backend; ma trận trạng thái khóa; soft delete; nhật ký; upload/download; export; GET; over-posting; thông báo lỗi.

Đã loại bỏ **4 phát hiện cũ** vì source hiện tại không chứng minh hoặc chỉ là nhận xét chất lượng: trạng thái chi tiết công việc tùy ý, Admin xem chat qua URL, nút xóa nhân sự dùng quyền sửa, và việc khởi tạo trực tiếp `TaiKhoanCaNhanService` như một “lỗi”.

---

## 2. Kiến trúc và module AS-IS

### 2.1 Kiến trúc

| Thành phần | Trách nhiệm AS-IS | Bằng chứng |
|---|---|---|
| ASP.NET Core MVC .NET 8 | Auth cookie, permission, scope, workflow, transaction, system-of-record | `Program.cs`, Controllers, Services, `QuanLyDuAnDbContext.cs` |
| SQL Server/EF Core | Dữ liệu nghiệp vụ, claim/role, AI dataset/kết quả/metadata | `Data/QuanLyDuAnDbContext.cs`, Entities |
| FastAPI | Validate dataset, train/predict, file `.joblib`, metadata và log cục bộ | `QuanLyDuAnAIService/app/main.py`, routers, services |

FastAPI không có dependency/đoạn mã kết nối SQL Server. MVC tổng hợp dữ liệu và gọi HTTP qua `AiApiService`; vì vậy MVC/SQL Server vẫn là system-of-record.

### 2.2 Danh sách Controller/module

| Nhóm | Controller |
|---|---|
| Tài khoản | `Account`, `TaiKhoanCaNhan`, `NhanSu`, `PhanQuyen`, `ChucDanh` |
| Tổ chức | `Team`, `ThanhVienTeam` |
| Dự án | `DuAn`, `TeamDuAn`, `NhanVienDuAn`, `YeuCauDoiQuanLy`, `DuyetYeuCauDoiQuanLy` |
| Công việc | `DanhMucCongViec`, `CongViec`, `ChiTietCongViec`, `PhanCongCongViec`, `PhanCongChiTietCongViec`, `TienDoCongViec` |
| Đề xuất/tài chính | `DeXuatCongViec`, `DuyetDeXuatCongViec`, `DeXuatNganSach`, `DuyetDeXuatNganSach`, `NganSach` |
| Đánh giá/chat | `DanhGiaDuAn`, `DanhGiaNhanVien`, `ChatDuAn` |
| AI/thống kê | `Ai`, `AiDataset`, `Dashboard` |
| Hạ tầng | `Home` |

Không có `ChiPhiController` và `NhatKyController`; điều này không đồng nghĩa nghiệp vụ không tồn tại. Chi phí được tạo trong `DyetDeXuatCongViecService.DuyetAsync`; nhật ký được ghi bởi nhiều service và hiển thị trong `DuAn/Details`.

---

## 3. Xác thực, tài khoản và phiên đăng nhập

### 3.1 Luồng tài khoản

| Chức năng | AS-IS | File/hàm |
|---|---|---|
| Login | Tra `Aspnetusers`, kiểm khóa, `EmailConfirmed`, hash mật khẩu; tạo role/permission claim và cookie | `AccountService.AuthenticateAsync`, `AccountController.Login` |
| Logout | POST, `[Authorize]`, có anti-forgery | `AccountController.Logout` |
| Kích hoạt | Token riêng trong `AspNetUserTokens`; hết hạn theo `AccountActivation:TokenLifetimeHours`; đặt mật khẩu và xác nhận email | `AccountService.TaoFormKichHoatAsync`, `KichHoatTaiKhoanAsync` |
| Gửi lại kích hoạt | Cooldown cấu hình mặc định 60 giây, transaction `Serializable`, hoàn tác token mới nếu SMTP lỗi | `NhanSuService.GuiLaiEmailKichHoatAsync` |
| Quên mật khẩu | Nhận username/email; nếu không hợp lệ trả luồng chung, gửi OTP khi hợp lệ | `AccountService.GuiOtpDatLaiMatKhauAsync` |
| OTP | 6 chữ số ngẫu nhiên, chỉ lưu SHA-256, hết hạn 3 phút | `TaoMaOtp`, `XacNhanOtpDatLaiMatKhauAsync` |
| Reset mật khẩu | Cần token phiên đã verify, hết hạn 3 phút; đổi `SecurityStamp`, xóa token phiên | `DatLaiMatKhauBangOtpAsync` |
| Đổi mật khẩu | Kiểm mật khẩu hiện tại, policy ViewModel, đổi hash/stamp | `TaiKhoanCaNhanService.DoiMatKhauAsync` |

### 3.2 Password, lockout và cookie

- Policy kích hoạt/reset/đổi mật khẩu yêu cầu tối thiểu 8 ký tự, chữ hoa, chữ thường, số và ký tự đặc biệt tại các ViewModel trong `ViewModels/Account/` và `ViewModels/TaiKhoanCaNhan/`.
- Cookie hết hạn 8 giờ, `SlidingExpiration=true`; không thấy cấu hình `SameSite`, `SecurePolicy` hoặc `HttpOnly` tùy chỉnh nên dùng mặc định framework (`Program.cs`).
- `AuthenticateAsync` đọc `LockoutEnd` nhưng không tăng `AccessFailedCount` và không tự đặt lockout khi sai mật khẩu.
- Không đăng ký `CookieAuthenticationEvents.OnValidatePrincipal`; `SecurityStamp` có được đổi khi khóa/reset nhưng không được so sánh mỗi request.
- Permission/role được nạp vào cookie lúc login. `PhanQuyenService.GetGrantedPermissionNamesAsync` chỉ đọc `ClaimsPrincipal`; không truy vấn claim role từ DB mỗi request.
- Login không join/kiểm `NguoiDung.IsDeleted`. Đây là khác biệt quan trọng với tài liệu cũ.

### 3.3 Xóa và khóa nhân sự

| Kiểm tra | Xóa | Khóa |
|---|---|---|
| Permission Controller | `NhanSu.Xoa` | `NhanSu.Khoa` |
| UI | Nút đúng permission; nút khóa ẩn với chính mình | `_Table.cshtml` |
| Tự thao tác | Backend xóa **không** nhận ID người thao tác | Backend chặn `id == currentUserId` |
| Admin cuối cùng | Không kiểm khi xóa | Đếm trong transaction `Serializable`, chặn còn ≤1 Admin active |
| Giới hạn Admin | Tối đa 3 khi tạo/đổi role | `NhanSuService.MaxAdminCount=3` |
| Đang quản lý dự án/leader/team/dự án | Chặn xóa | Chặn quản lý dự án/leader; task chưa hoàn thành chỉ log cảnh báo |
| Kiểu xóa | `NguoiDung.IsDeleted=true`; không xóa `Aspnetusers`, role/claim | `LockoutEnd=UtcNow+100 năm`, đổi `SecurityStamp` |

Kết luận: chống tự khóa và khóa Admin cuối cùng đã có cả backend. Chống tự xóa/xóa Admin cuối cùng chưa có; tài khoản Identity của hồ sơ soft-delete vẫn tồn tại.

---

## 4. Permission, role và scope

### 4.1 Số lượng chính xác

- **81 constant permission** trong `Constants/Permissions.cs`, không tính constant tên claim.
- **80 giá trị permission duy nhất**.
- **1 alias:** `Permissions.AI.DuDoan = Permissions.AI.PhanTichNguyenNhan`.
- Alias không được Controller/View tham chiếu trực tiếp; giá trị đích được dùng bởi `AiController`, `DuAnController`, `AiService`.

### 4.2 Seed tối thiểu theo role

`KhoiTaoTaiKhoanMacDinh.DamBaoTaiKhoanAdminMacDinhAsync` gọi `DamBaoRoleClaimToiThieuAsync`:

| Role | Số permission seed tối thiểu | Nhóm chính |
|---|---:|---|
| Admin | 32 | Nhân sự, phân quyền, chức danh, team, duyệt đổi QL, thống kê, nhật ký, xem chat, duyệt đánh giá, AI dataset/train/dashboard |
| Manager | 47 | Dự án, staffing, danh mục/công việc, duyệt, ngân sách/chi phí, chat, đánh giá, AI phân tích/xác nhận |
| Employee | 22 | Xem dự án/công việc, đề xuất, chi tiết/phân công theo scope, cập nhật tiến độ, chat, thống kê, xem đánh giá |

Sau seed, `DamBaoRoleKhongCoQuyenAsync` còn loại các permission bị cấm theo `PermissionDependencyProvider.GetDeniedPermissionsForRole`. Do đó bảng trên là danh sách đầu vào tối thiểu, không phải lời khẳng định mọi claim luôn còn lại nếu rule denied thay đổi.

### 4.3 Phân loại sử dụng

| Phân loại | Permission |
|---|---|
| Dùng trực tiếp ở Controller | Hầu hết nhóm: Thống kê, Nhân sự, Phân quyền, Chức danh, Team, Dự án, Công việc, Tiến độ, Đề xuất/Duyệt, Ngân sách, Đánh giá, Chat, AI |
| Dùng gián tiếp ở View/service | `ChiPhi.Xem` (Dashboard và `DuAn/Details`); `NhatKy.Xem` (layout/tab hoạt động); `DuyetDeXuatCongViec.Duyet`, `TienDo.Duyet` còn điều khiển workflow xác nhận/mở lại |
| Chưa có action thực thi tương ứng | `ChiPhi.Them`, `ChiPhi.Sua` chỉ xuất hiện trong seed/permission definition |
| Tương thích cũ | `AI.DuDoan` alias, không tạo giá trị claim mới |
| Dư thừa đã chứng minh | Chưa đủ cơ sở gọi permission nào “dư thừa”; `ChiPhi.Them/Sua` là điểm nghiệp vụ cần thống nhất |

Dashboard không trỏ tới `ChiPhiController`: các khối chi phí/ngân sách trỏ về module `NganSach`/chi tiết dự án. Vì vậy kết luận cũ “link chết vì không có Controller cùng tên” bị loại.

### 4.4 Chat Admin

- Admin được seed `Chat.Xem`; layout tính `canChat` nhưng còn dùng cờ role để không mở nghiệp vụ chat cho Admin.
- `ChatDuAnService.GetPageAsync`, `GetPhongChatDuocThamGiaAsync`, `GetTinNhanAsync`, `GuiTinNhanAsync` đều gọi `KiemTraKhongChoAdminChatDuAn`.
- `DongBoThanhVienPhongChatAsync` loại Admin khỏi danh sách thành viên; `CoQuyenVaoDuAnAsync` trả `false` với Admin.

AS-IS nhất quán: Admin có claim xem để tương thích/menu nhưng **không được đọc hoặc gửi chat dự án**. Không còn bằng chứng cho lỗi “Admin xem chat qua URL”.

### 4.5 Scope chính

| Vai trò | Scope backend |
|---|---|
| Admin | Quản trị hệ thống; không mặc nhiên được thao tác nghiệp vụ dự án/chat |
| Manager | `DuAn.MaNguoiDung == currentUserId` cho workflow quản lý |
| Employee | Thành viên `NhanVienDuAn`; một số action còn cần phân công |
| Leader | Không phải Identity role; là `NhanVienTeam.IsLeader`/vai trò dữ liệu và phải qua scope service |

---

## 5. Kiểm soát backend và trạng thái khóa dữ liệu

### 5.1 Ma trận thao tác nhạy cảm

| Thao tác | View | Controller | Service: role/scope/trạng thái | Gọi URL trực tiếp |
|---|---|---|---|---|
| Xóa nhân sự | `NhanSu.Xoa` | `NhanSu.Xoa` | Ràng buộc dự án/team/phân công | Bị kiểm permission và constraint; còn KT-05 |
| Khóa/mở khóa | `NhanSu.Khoa/MoKhoa` | Cùng permission | Chặn tự khóa, Admin cuối, manager/leader | Bị kiểm |
| Xóa dự án | `DuAn.Xoa` | `DuAn.Xoa` | Manager/scope và ràng buộc dữ liệu | Bị kiểm |
| Tạm dừng/mở lại/xác nhận hoàn thành | `DuAn.Sua` hoặc cờ workflow | Controller + permission | `DuAnService.CheckManagerPermissionAsync`, trạng thái nguồn | Bị kiểm |
| Phân công/gỡ | `PhanCong*.ThucHien` | Cùng permission | Manager/Leader, dự án/công việc hợp lệ | Bị kiểm |
| Gửi/duyệt/từ chối tiến độ | `TienDo.CapNhat/Duyet` | Cùng permission | Người được phân công/Manager dự án, trạng thái `ChoDuyet` | Bị kiểm |
| Duyệt/từ chối đề xuất | `Duyet*.Duyet` | Cùng permission | Manager dự án, trạng thái chờ | Bị kiểm |
| Xác nhận nguyên nhân AI | `AI.XacNhan` | `DuAnController`/`AiController` | Manager phụ trách, dự án trễ, kết quả chính thức | Bị kiểm |
| Train/kích hoạt/xóa model | `AI.Train` | `AiController` | Permission, metadata DB và FastAPI | Bị kiểm; có anti-forgery |

Ẩn nút chỉ là UX; kết luận “bị chặn” ở bảng trên dựa trên Controller/service, không dựa riêng vào View.

### 5.2 Trạng thái chi tiết công việc

Đường dữ liệu thực tế:

`ChiTietCongViecCreateUpdateViewModel → ChiTietCongViecController.Them/Sua → ChiTietCongViecService.AddAsync/UpdateAsync → CtCongViec → SaveChanges`

- Form `_Form.cshtml` chỉ hiển thị text “Chưa bắt đầu”, không có select/input `TrangThaiCTCV`.
- ViewModel còn property `TrangThaiCTCV`, nhưng `AddAsync` bỏ qua và gán `TrangThai.ChuaBatDau`.
- `UpdateAsync` chỉ cập nhật tên, nội dung, ngày bắt đầu; không gán trạng thái từ form.
- Trạng thái thật thay đổi qua `TienDoCongViecService` sau luồng báo cáo/duyệt và `TrangThaiWorkflowService`.

Kết luận: không có đường over-post trạng thái chi tiết công việc trong create/update hiện tại.

### 5.3 Ma trận trạng thái khóa

| Đối tượng | Trạng thái | Xem | Sửa/xóa | Thêm dữ liệu con | Ghi chú |
|---|---|---|---|---|---|
| Dự án | Chuẩn bị/đang thực hiện | Theo scope | Manager + permission | Có theo workflow | `DuAnService` |
| Dự án | Tạm dừng | Theo scope | Chỉ transition hợp lệ | Hạn chế thao tác vận hành | |
| Dự án | Chờ xác nhận HT | Theo scope | Chỉ xác nhận/mở lại theo rule | Đề xuất mới có thể kéo về đang thực hiện khi duyệt | |
| Dự án | Hoàn thành/lưu trữ/đã hủy | Theo scope | Phần lớn khóa; mở lại có điều kiện | Không | |
| Công việc | Chưa bắt đầu/đang làm | Theo dự án | Workflow permission | Có chi tiết/phân công |
| Công việc | Chờ xác nhận/hoàn thành | Theo dự án | Xác nhận hoặc mở lại | Khóa cập nhật thường |
| Chi tiết CV | Chưa bắt đầu/đang làm | Theo scope/phân công | Theo trạng thái cha | Báo cáo tiến độ |
| Chi tiết CV | Chờ xác nhận/hoàn thành | Theo scope | Qua duyệt/mở lại, không qua form CRUD | File minh chứng gắn báo cáo |
| Báo cáo tiến độ | Chờ duyệt | Người gửi/reviewer | Duyệt, yêu cầu bổ sung, từ chối | File không xóa riêng |
| Đề xuất CV/NS | Chờ duyệt | Theo scope | Người tạo hủy; Manager duyệt/từ chối | Duyệt sinh CV/NS/chi phí |
| Đánh giá | Nháp/chờ duyệt | Theo scope | Tác giả sửa nháp; Admin/Manager duyệt theo module | Chi tiết tiêu chí |
| Model AI | Active | `AI.Train/Dashboard` | Có thể đổi active; xóa có kiểm | Không | DB metadata + file FastAPI |

---

## 6. Anti-forgery, action GET và model binding

### 6.1 Cấu hình toàn cục

- `Program.cs` chỉ thêm `AuthorizeFilter` toàn cục.
- Không có `AutoValidateAntiforgeryTokenAttribute`, filter anti-forgery global hoặc `[IgnoreAntiforgeryToken]`.
- Razor form dùng Form Tag Helper thường sinh hidden token, nhưng token ở form **không được server kiểm** nếu action không có filter.
- Các request JavaScript không thấy cơ chế header anti-forgery toàn cục.

Vì vậy 55 POST không có `[ValidateAntiForgeryToken]` là thiếu bảo vệ server-side, không chỉ là thiếu annotation “trang trí”.

### 6.2 Bảng POST và anti-forgery

| Controller | Action POST | Thay đổi dữ liệu/trạng thái | Bảo vệ action | Bảo vệ toàn cục | Kết luận |
|---|---|---|---|---|---|
| Account | Login, Logout, Activate, ForgotPassword, VerifyOtp, ResetPassword | Có/session/email/token | 6/6 | Không | Đủ tại action |
| Ai | Train, Predict, TestPredict, XacNhan, Validate, Compare, SetActive, Reload, Delete | 7 có hiệu ứng; 2 kiểm tra | 9/9 | Không | Đủ |
| AiDataset | Tổng hợp all/project, kiểm chất lượng | 2 ghi DB, 1 đọc | 3/3 | Không | Đủ |
| NhanSu | Lưu, gửi kích hoạt, xóa, khóa, mở khóa | Có | 5/5 | Không | Đủ |
| PhanQuyen | Save | Có | 1/1 | Không | Đủ |
| TaiKhoanCaNhan | Cập nhật, đổi mật khẩu | Có | 2/2 | Không | Đủ |
| DanhGiaDuAn | Lưu, gửi duyệt, duyệt, từ chối | Có | 4/4 | Không | Đủ |
| ChatDuAn | Gửi tin | Có | 0/1 | Không | **Thiếu** |
| ChiTietCongViec | Thêm, sửa, xóa | Có | 0/3 | Không | **Thiếu** |
| ChucDanh | Lưu, xóa | Có | 0/2 | Không | **Thiếu** |
| CongViec | Xác nhận HT, mở lại | Có | 0/2 | Không | **Thiếu** |
| DanhGiaNhanVien | Lưu, gửi duyệt, duyệt, từ chối | Có | 0/4 | Không | **Thiếu** |
| DanhMucCongViec | Lưu, xóa | Có | 0/2 | Không | **Thiếu** |
| DeXuatCongViec | Tạo, hủy | Có | 0/2 | Không | **Thiếu** |
| DeXuatNganSach | Tạo, hủy | Có | 0/2 | Không | **Thiếu** |
| DuAn | 10 action workflow/file/CRUD | Có | 2/10 | Không | **Thiếu 8** |
| DuyetDeXuatCongViec | Duyệt, từ chối | Có | 0/2 | Không | **Thiếu** |
| DuyetDeXuatNganSach | Duyệt, từ chối | Có | 0/2 | Không | **Thiếu** |
| DuyetYeuCauDoiQuanLy | Approve, Reject | Có | 0/2 | Không | **Thiếu** |
| NhanVienDuAn | Thêm, cập nhật vai trò, xóa | Có | 0/3 | Không | **Thiếu** |
| PhanCongChiTietCongViec | Thêm, xóa | Có | 0/2 | Không | **Thiếu** |
| PhanCongCongViec | Thêm, xóa | Có | 0/2 | Không | **Thiếu** |
| Team | Lưu, xóa | Có | 0/2 | Không | **Thiếu** |
| TeamDuAn | Lưu, xóa | Có | 0/2 | Không | **Thiếu** |
| ThanhVienTeam | Lưu, xóa, gán leader | Có | 0/3 | Không | **Thiếu** |
| TienDoCongViec | 6 action báo cáo/file | Có | 0/6 | Không | **Thiếu** |
| YeuCauDoiQuanLy | Create, Cancel | Có | 0/2 | Không | **Thiếu** |

Tổng: 87 POST; 32 có filter action; 55 không có. Có 84 POST tạo hiệu ứng dữ liệu/session/email/model và 3 POST kiểm tra thuần (`ValidateModel`, `CompareModel`, `KiemTraChatLuongDataset`).

### 6.3 GET

| Loại GET | Số lượng | Kết luận |
|---|---:|---|
| Hiển thị/partial/form | 59 | Không thấy ghi dữ liệu trực tiếp |
| Export | 12 | GET tạo file trả về, không ghi nghiệp vụ; gồm export metadata AI |
| Download file nghiệp vụ | 2 | `TaiFileDuAn`, `TaiFileTienDo` |
| GET thay đổi dữ liệu | 0 | Không phát hiện action GET gọi workflow ghi dữ liệu |

`AiController.Train` GET chỉ dựng trang train; việc train thật là POST. Các hàm “đảm bảo phòng chat” có thể tạo phòng trong service khi tải trang chat, nhưng đây là lazy initialization nội bộ trong GET `ChatDuAn.Index`; cần lưu ý về tính idempotent, chưa xếp là lỗ hổng CSRF vì không thực hiện hành động người dùng nhạy cảm.

### 6.4 Model binding/over-posting

| Form | Kiểu nhận | Kiểm soát trường nhạy cảm |
|---|---|---|
| Nhân sự | `NhanSuCreateUpdateViewModel` | Service xác minh role, giới hạn Admin, ràng buộc đổi role |
| Dự án | ViewModel riêng | Manager/owner và trạng thái được service xác định lại |
| Chi tiết CV | ViewModel riêng | Bỏ qua trạng thái gửi lên; create gán `ChuaBatDau` |
| Tiến độ | ViewModel riêng | Service xác minh assignee, trạng thái đề xuất, file |
| Đánh giá | ViewModel riêng | Người đánh giá/trạng thái/điểm được kiểm tại service |
| Phân quyền | ID permission chọn | Service map với danh mục hợp lệ và normalize dependency |
| AI model | Tên file/ID scalar | Service/FastAPI validate model và permission |

Không Controller nhạy cảm nào trong nhóm trên nhận thẳng Entity để bind toàn bộ. Chưa thấy đường gửi thêm `IsDeleted`, `IsActive`, owner/Manager để ghi trực tiếp mà không kiểm soát backend.

---

## 7. Upload, download và export

### 7.1 Ma trận upload/download

| Loại file | Nơi lưu | Dung lượng | Định dạng | Quyền tải qua action | Xóa file | Nguy cơ |
|---|---|---:|---|---|---|---|
| Dự án | `wwwroot/uploads/duan/{maDuAn}` | Không giới hạn trong service | Không whitelist/MIME | `DuAnController.TaiFileDuAn` + `DuAn.Xem` + `CanAccessAsync` | Soft-delete DB và xóa vật lý | Static URL; upload không giới hạn |
| Tiến độ | `wwwroot/uploads/tiendocongviec/{maChiTietCV}` | 10 MB/file | Whitelist extension trong `TienDoCongViecService` | `TaiFileTienDo` + `TienDo.Xem` + scope | Không cho xóa riêng sau gửi | Static URL; không kiểm magic-byte/MIME |
| Công việc | Có Entity `FileCongViec` | Không thấy chức năng upload active | — | Không có action tải riêng | — | Schema/Entity tồn tại, không suy diễn là tính năng |
| Chi tiết CV | Có Entity `FileCtCongViec` | Không thấy chức năng upload active | — | Không có action tải riêng | — | Tương tự |
| Minh chứng đánh giá | Không thấy luồng upload | — | — | — | — | Không có bằng chứng chức năng |
| Model AI | `QuanLyDuAnAIService/models` | FastAPI quản lý | `.joblib` + JSON metadata | MVC Admin gọi API; export metadata qua Controller | FastAPI + soft-delete metadata MVC | Không nằm dưới MVC `wwwroot` |

Chi tiết kỹ thuật:

- `Path.GetFileName` loại phần directory từ tên gốc; tên lưu dùng `Guid.NewGuid():N + extension`, giảm traversal/đoán tên.
- `GetPhysicalPath` ghép đường dẫn từ DB; người dùng không truyền trực tiếp đường dẫn vào action tải.
- View liên kết tới action `TaiFileDuAn`/`TaiFileTienDo`, không liên kết URL tĩnh.
- Tuy vậy, DB lưu `/uploads/...`; `Program.cs` gọi `UseStaticFiles()` trước auth và thư mục nằm trong `wwwroot`. Nếu biết chính xác GUID URL, request static không đi qua Controller authorization. Source đủ chứng minh đường bypass; runtime test chỉ cần xác nhận cấu hình deploy không chặn thêm ở reverse proxy.

### 7.2 Tính nhất quán file–DB

- File dự án được ghi disk trước khi insert `FileDuAn`/`SaveChangesAsync`; catch không xóa file vừa ghi.
- Tiến độ lưu danh sách physical path và có nhánh bù xóa khi transaction lỗi trong `TienDoCongViecService`.
- Xóa file dự án xóa vật lý trước `SaveChanges`; nếu DB save lỗi, record có thể còn nhưng file đã mất.

### 7.3 Export

| Định dạng | Cách tạo | Unicode | Permission/scope |
|---|---|---|---|
| PDF | QuestPDF trong `ExportFileService` | Font/text Unicode | Action yêu cầu `ThongKe.XuatFile` + quyền xem module |
| Excel | ClosedXML, tạo workbook trong memory | Unicode | Dữ liệu lấy từ service đã áp filter/scope |
| CSV | `Encoding.UTF8` + BOM | Tốt cho tiếng Việt | Cùng quyền/filter |

- Tên file được normalize và gắn timestamp.
- Export dựng `byte[]`/workbook trong memory; dữ liệu rất lớn có nguy cơ RAM/thời gian, cần load test.
- `EscapeCsv` chỉ quote dấu phẩy, quote và newline; không trung hòa ô bắt đầu bằng `=`, `+`, `-`, `@`. Đây là rủi ro cần test với Excel/LibreOffice.
- Các query export chính tái sử dụng filter và scope module; các bảng soft-delete chính được service lọc. Không khẳng định mọi join phụ đều hoàn hảo nếu chưa có test dataset chứa bản ghi xóa.

---

## 8. Soft delete và nhật ký

### 8.1 Entity có `IsDeleted`

Có 23 Entity khai báo `IsDeleted`: `AiModel`, `AiNguyenNhan`, `ChiPhi`, `CongViec`, `CtCongViec`, `DanhMucCongViec`, `DeXuatCongViec`, `DeXuatNganSach`, `DuAn`, `DanhGiaDuAn`, `DanhGiaNhanVien`, hai bảng chi tiết đánh giá, bốn bảng file, `NguoiDung`, `NganSach`, `PhongChat`, `Team`, `TinNhan`, `YeuCauDoiQuanLy`.

`QuanLyDuAnDbContext` không cấu hình global query filter; từng query phải tự ghi `IsDeleted != true`.

### 8.2 Mức bao phủ

- Dịch vụ danh sách/chi tiết chính thường lọc soft delete.
- `AiDatasetService` lọc dự án, công việc, danh mục, chi tiết, ngân sách, chi phí và nguyên nhân đã xóa khi tổng hợp feature.
- `AiService.BuildReasonTrainDataset` chỉ lấy `LaDuAnTre=1`, có nhãn xác nhận và đủ feature; không dùng `AI_KET_QUA` làm nhãn.
- Export dùng danh sách từ service module nên kế thừa filter chính.
- Quan hệ không có `IsDeleted` (ví dụ nhiều bảng phân công/thành viên) được xóa cứng hoặc tồn tại theo FK; không thể áp một rule soft-delete chung.
- Rủi ro trọng yếu đã chứng minh là `AuthenticateAsync` không lọc `NguoiDung.IsDeleted`, xem KT-01.

### 8.3 Nhật ký

| Hành động có log | Bảng/nguồn | Dữ liệu |
|---|---|---|
| Tạo/chuyển trạng thái/mở lại dự án | `NhatKyQuanLyDuAn` | Người quản lý, hành động, thời gian |
| Đổi quản lý | `NhatKyQuanLyDuAn`, `NhatKyPhuTrachDuAn` | Quản lý cũ/mới, khoảng phụ trách |
| Gán/gỡ team dự án | `NhatKyDuAn`/log dự án | Team, hành động, thời gian |
| Tạo/hủy/duyệt/từ chối đề xuất | `NhatKyQuanLyDuAn` | Mã đề xuất, trạng thái |
| Ngân sách/chi phí | `NhatKyNganSach`, `NhatKyChiPhi` | Snapshot số tiền/trạng thái/ngày |
| Mở lại công việc, workflow tiến độ | Nhật ký quản lý/trạng thái liên quan | Hành động và thời gian |

`DuAnService` hợp nhất nhật ký để hiển thị tab hoạt động trong `Views/DuAn/Details.cshtml`; tab được điều khiển bởi `Permissions.NhatKy.Xem`. Không có màn hình nhật ký độc lập. Log chủ yếu là mô tả hành động/snapshot chuyên biệt, không phải audit trail đồng nhất trước–sau cho mọi Entity. Thao tác tài khoản, permission, chat và AI model chưa có một bảng audit nghiệp vụ chung; log kỹ thuật dùng `ILogger` ở một số lỗi.

---

## 9. Transaction và duyệt đồng thời

| Luồng | Transaction/isolation | Recheck trạng thái trong transaction | Constraint/concurrency | Kết luận |
|---|---|---|---|---|
| Duyệt đề xuất công việc | `Serializable` | Có: load lại đề xuất, kiểm chờ duyệt; kiểm công việc đã sinh | Không thấy rowversion; có kiểm tồn tại theo đề xuất | Đã phòng double-approve ở mức transaction |
| Duyệt ngân sách | `Serializable` | Có: load lại, kiểm trạng thái; xử lý ngân sách active | Không rowversion | Đã phòng cạnh tranh chính |
| Duyệt đổi quản lý | `Serializable` | Có: load lại yêu cầu/trạng thái và dự án | Không rowversion | Đã phòng double-approve chính |
| Gửi tiến độ | Transaction mặc định | Kiểm pending trước transaction, rồi insert | Không unique constraint “một pending/chi tiết” được thấy | Cần test đồng thời |
| Duyệt/từ chối tiến độ | Transaction mặc định | Entity/trạng thái được đọc trước `BeginTransactionAsync` | Không rowversion | Cần test hai reviewer |
| Xác nhận hoàn thành dự án/công việc | Service kiểm trạng thái, một `SaveChanges`/workflow | Có kiểm trạng thái nguồn | Không rowversion | Cần test cạnh tranh transition |
| Train model | FastAPI ghi file/metadata; MVC đồng bộ `AiModel` | Kiểm response/model | Hai hệ lưu trữ, không có distributed transaction | Cần test lỗi giữa API và DB |
| Kích hoạt/xóa model | FastAPI trước/sau metadata MVC tùy action | Có validate | Không distributed transaction | Cần test rollback/reconcile |

Không kết luận race cho ba luồng `Serializable`. Phát hiện concurrency chỉ giữ cho tiến độ/model vì cần runtime kiểm tra interleaving thực tế.

---

## 10. Đối chiếu AI MVC–FastAPI

### 10.1 Contract và nguồn nhãn

- 22 feature được định nghĩa ở MVC trong các ViewModel/dataset mapping của `AiDatasetService` và ở FastAPI `app/constants.py: FEATURE_COLUMNS`.
- `AiDatasetService.GanSnapshotVaoDataset` ghi snapshot; `HasDuFeature` kiểm đầy đủ.
- Điều kiện train MVC: `AiService.BuildReasonTrainDataset` lấy dự án trễ (`LaDuAnTre=1`), `MaDMNguyenNhan` có giá trị và đủ feature.
- FastAPI `reason_dataset_policy.classify_reason_dataset`: tối thiểu 30 dòng dùng được, ít nhất 2 lớp, mỗi lớp tối thiểu 5 dòng (`MIN_REASON_TRAIN_ROWS=30`, `MIN_REASON_CLASS_COUNT=2`, `MIN_REASON_ROWS_PER_CLASS=5`).
- Ground truth đến từ `AiNguyenNhan` do Manager xác nhận; `AiDatasetService` map `MaDMNguyenNhan`. `AiKetQua` chỉ là output/history, không tham gia `BuildReasonTrainDataset`.

### 10.2 Điều kiện phân tích và quyền

| Thao tác | Permission | Điều kiện service |
|---|---|---|
| Dashboard AI | `AI.Dashboard`/`AI.Xem` | Theo role seed |
| Dataset | `AI.Dataset` | Admin |
| Train/model | `AI.Train` | Admin |
| Phân tích chính thức | `AI.PhanTichNguyenNhan` | Manager phụ trách, không Admin, dự án được xác định trễ |
| Xác nhận | `AI.XacNhan` | Manager phụ trách, nguyên nhân hợp lệ, có kết quả chính thức/dataset |

### 10.3 Model active, lỗi và confidence

- FastAPI quản lý alias `reason_active.joblib` và metadata active; MVC lưu `AiModel.IsActive`/`IsDeleted`.
- Khi FastAPI lỗi, model chưa sẵn sàng, confidence dưới ngưỡng hoặc kết quả mâu thuẫn rule, `AiService` dùng `RuleFallback`; đây vẫn là gợi ý.
- `AiService` lưu kết quả chính thức vào `AiKetQua`; test predict không ghi lịch sử chính thức.
- View chi tiết dự án hiển thị phần trăm/nhãn độ tin cậy và nguồn dự đoán/fallback.
- Manager xác nhận nguyên nhân thật vào `AiNguyenNhan`; xác nhận đồng bộ nhãn snapshot mới nhất.

Kết luận kiến trúc AI phù hợp vai trò hỗ trợ, không tự thay đổi trạng thái dự án và không ghi SQL Server từ FastAPI.

---

## 11. Danh sách phát hiện chuẩn hóa

### KT-01: Hồ sơ nhân sự đã soft-delete vẫn có thể đăng nhập bằng Identity account

* **Loại phát hiện:** A
* **Mức độ:** Cao
* **Mức chắc chắn:** Cao
* **Module:** Tài khoản/Nhân sự
* **File:** `Services/Implementations/NhanSuService.cs`; `AccountService.cs`
* **Class/hàm/action:** `DeleteAsync`; `AuthenticateAsync`; `AccountController.Login`
* **Luồng nghiệp vụ:** Xóa nhân sự → chỉ đặt `NguoiDung.IsDeleted` → Identity account/role còn nguyên → login chỉ truy vấn `Aspnetusers`.
* **Bằng chứng source:** `DeleteAsync` không khóa/xóa `Aspnetusers`; `AuthenticateAsync` không join `NguoiDung` và không kiểm `IsDeleted`.
* **Cách tái hiện:** Tạo nhân sự active không còn ràng buộc dự án/team; xóa; đăng xuất; đăng nhập lại bằng credential cũ.
* **Kết quả hiện tại:** Authentication có thể thành công và phát cookie claim cũ.
* **Kết quả mong đợi:** Hồ sơ đã xóa không được xác thực.
* **Ảnh hưởng:** Tài khoản bị “xóa” vẫn truy cập endpoint theo claim/scope còn hiệu lực.
* **Nguyên nhân:** Soft delete hồ sơ tách rời lifecycle Identity.
* **Hướng xử lý đề xuất:** Khi xóa phải vô hiệu hóa Identity và `AuthenticateAsync` phải kiểm hồ sơ active; thống nhất cleanup role/claim.
* **Có cần sửa CSDL không:** Không.

### KT-02: 55 POST không được server kiểm anti-forgery

* **Loại phát hiện:** A
* **Mức độ:** Cao
* **Mức chắc chắn:** Cao
* **Module:** 20 Controller nghiệp vụ
* **File:** `Program.cs`; các Controller liệt kê tại 6.2
* **Class/hàm/action:** 55 action `[HttpPost]` không có `[ValidateAntiForgeryToken]`
* **Luồng nghiệp vụ:** Trình duyệt có cookie → request cross-site POST → action thay đổi dữ liệu.
* **Bằng chứng source:** Không có filter global/ignore; chỉ 32/87 POST có filter action.
* **Cách tái hiện:** Dùng trang khác auto-submit form tới một action thiếu filter khi nạn nhân đang đăng nhập; kiểm tra 400/đột biến dữ liệu.
* **Kết quả hiện tại:** Server không yêu cầu/validate token ở 55 action.
* **Kết quả mong đợi:** Mọi POST dùng cookie và thay đổi trạng thái phải validate token.
* **Ảnh hưởng:** CSRF trên workflow dự án, phân công, duyệt, chat, file.
* **Nguyên nhân:** Bảo vệ được áp rời rạc.
* **Hướng xử lý đề xuất:** Dùng global auto-validate hoặc bổ sung filter có kiểm thử AJAX/form.
* **Có cần sửa CSDL không:** Không.

### KT-03: File dự án và tiến độ có thể bỏ qua authorization bằng URL static

* **Loại phát hiện:** A
* **Mức độ:** Cao
* **Mức chắc chắn:** Cao
* **Module:** File dự án/tiến độ
* **File:** `Program.cs`; `FileDuAnService.cs`; `TienDoCongViecService.cs`
* **Class/hàm/action:** `UseStaticFiles`; `UploadAsync`; `LuuFileMinhChungChoTienDoAsync`
* **Luồng nghiệp vụ:** Upload vào `wwwroot/uploads` → biết GUID URL → static middleware trả file trước auth.
* **Bằng chứng source:** Đường dẫn DB `/uploads/...`, physical path dưới web root; static middleware đứng trước authentication.
* **Cách tái hiện:** Lấy URL lưu trong DB/log/network rồi mở ở trình duyệt ẩn danh.
* **Kết quả hiện tại:** Source cho phép static middleware phục vụ file mà không gọi action tải có scope.
* **Kết quả mong đợi:** Chỉ tải qua endpoint authorization hoặc static layer riêng có kiểm quyền.
* **Ảnh hưởng:** Lộ tài liệu nếu URL bị chia sẻ/rò rỉ.
* **Nguyên nhân:** Lưu file private trong public web root.
* **Hướng xử lý đề xuất:** Đưa file ra ngoài `wwwroot` hoặc chặn `/uploads` khỏi static và chỉ serve qua Controller.
* **Có cần sửa CSDL không:** Không.

### KT-04: Login không tăng số lần sai và không kích hoạt lockout tự động

* **Loại phát hiện:** A
* **Mức độ:** Cao
* **Mức chắc chắn:** Cao
* **Module:** Tài khoản
* **File:** `Services/Implementations/AccountService.cs`
* **Class/hàm/action:** `AuthenticateAsync`
* **Luồng nghiệp vụ:** Gửi mật khẩu sai lặp lại → `VerifyHashedPassword` thất bại → trả lỗi.
* **Bằng chứng source:** Không cập nhật `AccessFailedCount`, không có threshold/throttle; chỉ đọc khóa do Admin đặt.
* **Cách tái hiện:** Gửi nhiều login sai và kiểm `Aspnetusers.AccessFailedCount`/khả năng tiếp tục thử.
* **Kết quả hiện tại:** Không có giới hạn thử sai ở ứng dụng.
* **Kết quả mong đợi:** Rate limit/lockout có ngưỡng và reset hợp lý.
* **Ảnh hưởng:** Tăng khả năng brute-force/password spraying.
* **Nguyên nhân:** Dùng `PasswordHasher` trực tiếp thay cho sign-in manager/logic lockout.
* **Hướng xử lý đề xuất:** Thêm rate limiting và cập nhật lockout atomically, tránh lộ tồn tại tài khoản.
* **Có cần sửa CSDL không:** Không.

### KT-05: Xóa nhân sự không chặn tự xóa hoặc xóa Admin cuối cùng

* **Loại phát hiện:** A
* **Mức độ:** Cao
* **Mức chắc chắn:** Cao
* **Module:** Nhân sự
* **File:** `NhanSuController.cs`; `NhanSuService.cs`; `Views/NhanSu/_Table.cshtml`
* **Class/hàm/action:** `XoaNhanSu`; `DeleteAsync`; `KiemTraRangBuocXoaAsync`
* **Luồng nghiệp vụ:** User có `NhanSu.Xoa` POST ID chính mình/Admin cuối → service chỉ kiểm quan hệ dự án/team/task → soft-delete.
* **Bằng chứng source:** Không truyền current user vào `DeleteAsync`; không đếm Admin trong đường xóa. Các chặn tương ứng chỉ có ở `LockAccountAsync`.
* **Cách tái hiện:** Dùng Admin cuối không còn quan hệ nghiệp vụ, POST xóa chính hồ sơ đó.
* **Kết quả hiện tại:** Điều kiện backend cho phép tới `IsDeleted=true`.
* **Kết quả mong đợi:** Chặn tự xóa và đảm bảo còn ít nhất một Admin active.
* **Ảnh hưởng:** Mất tài khoản quản trị khả dụng; kết hợp KT-01 tạo trạng thái “đã xóa nhưng vẫn login”.
* **Nguyên nhân:** Rule khóa không được áp cho rule xóa.
* **Hướng xử lý đề xuất:** Dùng transaction `Serializable`, current user ID và kiểm Admin active giống luồng khóa.
* **Có cần sửa CSDL không:** Không.

### KT-06: Cookie không tự thu hồi khi khóa, xóa hoặc đổi permission/role

* **Loại phát hiện:** A
* **Mức độ:** Cao
* **Mức chắc chắn:** Cao
* **Module:** Auth/Phân quyền/Nhân sự
* **File:** `Program.cs`; `AccountService.cs`; `PhanQuyenService.cs`; `PermissionHelper.cs`
* **Class/hàm/action:** Cookie config; `AuthenticateAsync`; `SaveRolePermissionsAsync`; `GetGrantedPermissionNamesAsync`
* **Luồng nghiệp vụ:** Login → claim vào cookie 8h sliding → Admin đổi quyền/khóa/xóa → request sau vẫn dùng principal cũ.
* **Bằng chứng source:** Permission helper chỉ đọc claim; không `OnValidatePrincipal`, không security-stamp validation, refresh sign-in hoặc session revocation.
* **Cách tái hiện:** Login user A; Admin thu hồi quyền/khóa A; không logout A; gọi action bằng cookie hiện tại.
* **Kết quả hiện tại:** Claim/session cũ có thể tiếp tục tới khi cookie hết hạn.
* **Kết quả mong đợi:** Thay đổi bảo mật quan trọng làm principal hết hiệu lực trong thời gian ngắn.
* **Ảnh hưởng:** Quyền đã thu hồi hoặc tài khoản khóa vẫn có cửa sổ truy cập.
* **Nguyên nhân:** Cookie auth tùy biến không có validation lifecycle.
* **Hướng xử lý đề xuất:** Validate stamp/version mỗi request hoặc chu kỳ ngắn; revoke session khi đổi quyền/khóa/xóa.
* **Có cần sửa CSDL không:** Có thể cần nếu chọn bảng session/version; có thể không nếu dùng stamp hiện có.

### KT-07: Upload file dự án không giới hạn kích thước, extension hoặc MIME

* **Loại phát hiện:** A
* **Mức độ:** Trung bình
* **Mức chắc chắn:** Cao
* **Module:** File dự án
* **File:** `Services/Implementations/FileDuAnService.cs`
* **Class/hàm/action:** `UploadAsync`
* **Luồng nghiệp vụ:** Manager upload `IFormFile` → chỉ kiểm khác rỗng → copy vào web root.
* **Bằng chứng source:** Không thấy max size, whitelist extension, content type/magic-byte; giữ extension gốc.
* **Cách tái hiện:** Upload file rất lớn hoặc extension HTML/SVG và kiểm file được lưu.
* **Kết quả hiện tại:** Service chấp nhận mọi loại/kích thước không rỗng.
* **Kết quả mong đợi:** Giới hạn dung lượng và định dạng theo nghiệp vụ; lưu private.
* **Ảnh hưởng:** Cạn dung lượng, lưu nội dung chủ động trong static root.
* **Nguyên nhân:** Validation upload dự án chưa tương đương tiến độ.
* **Hướng xử lý đề xuất:** Whitelist, max size, kiểm signature/MIME, quota và scan nếu cần.
* **Có cần sửa CSDL không:** Không.

### KT-08: File dự án và bản ghi DB có thể lệch khi lỗi

* **Loại phát hiện:** A
* **Mức độ:** Thấp
* **Mức chắc chắn:** Cao
* **Module:** File dự án
* **File:** `Services/Implementations/FileDuAnService.cs`
* **Class/hàm/action:** `UploadAsync`, `DeleteAsync`
* **Luồng nghiệp vụ:** Ghi/xóa file vật lý và `SaveChangesAsync` không có compensating action.
* **Bằng chứng source:** Upload ghi disk trước save; delete xóa disk trước save.
* **Cách tái hiện:** Gây lỗi DB sau khi thao tác filesystem.
* **Kết quả hiện tại:** Có thể orphan file hoặc DB trỏ file mất.
* **Kết quả mong đợi:** Hoàn tác filesystem hoặc job reconcile.
* **Ảnh hưởng:** Rác disk hoặc lỗi tải.
* **Nguyên nhân:** Filesystem không tham gia transaction DB.
* **Hướng xử lý đề xuất:** Compensating cleanup và reconciliation.
* **Có cần sửa CSDL không:** Không.

### KT-09: Duyệt/gửi tiến độ và đồng bộ model cần kiểm thử cạnh tranh

* **Loại phát hiện:** B
* **Mức độ:** Trung bình
* **Mức chắc chắn:** Trung bình
* **Module:** Tiến độ/AI model
* **File:** `TienDoCongViecService.cs`; `AiService.cs`; FastAPI `model_service.py`, `model_storage.py`
* **Class/hàm/action:** `CapNhatTienDoAsync`, `XuLyBaoCaoTienDoAsync`, train/set-active/delete model
* **Luồng nghiệp vụ:** Hai request cùng gửi/duyệt hoặc MVC–FastAPI lỗi giữa hai bước.
* **Bằng chứng source:** Transaction tiến độ dùng isolation mặc định và một số check nằm trước begin; model có hai nguồn lưu trữ không có distributed transaction.
* **Cách tái hiện:** Cần kiểm thử runtime bằng hai request song song và fault injection sau bước FastAPI/trước DB save.
* **Kết quả hiện tại:** Chưa đủ bằng chứng khẳng định sinh trùng hoặc lệch.
* **Kết quả mong đợi:** Một transition thắng; request còn lại báo xung đột; model active tự reconcile.
* **Ảnh hưởng:** Có thể trùng báo cáo/chuyển trạng thái hoặc lệch metadata.
* **Nguyên nhân:** Thiếu concurrency token/unique invariant rõ ở các đường này.
* **Hướng xử lý đề xuất:** Test trước; chỉ thêm constraint/token/transaction khi tái hiện.
* **Có cần sửa CSDL không:** Có thể, nhưng chỉ ghi nhận.

### KT-10: CSV có khả năng formula injection khi mở bằng spreadsheet

* **Loại phát hiện:** B
* **Mức độ:** Trung bình
* **Mức chắc chắn:** Trung bình
* **Module:** Export
* **File:** `Services/Implementations/ExportFileService.cs`
* **Class/hàm/action:** `BuildCsv`, `EscapeCsv`
* **Luồng nghiệp vụ:** Dữ liệu người dùng bắt đầu `=`, `+`, `-`, `@` → export CSV → mở bằng Excel/LibreOffice.
* **Bằng chứng source:** Escape CSV đúng cấu trúc nhưng không neutralize công thức.
* **Cách tái hiện:** Tạo tên/mô tả `=1+1` hoặc payload an toàn, export và mở trong môi trường cô lập.
* **Kết quả hiện tại:** Cần kiểm thử runtime để xác nhận ứng dụng spreadsheet diễn giải.
* **Kết quả mong đợi:** Nội dung được hiển thị như text.
* **Ảnh hưởng:** Công thức ngoài ý muốn trong file người dùng tải.
* **Nguyên nhân:** CSV escaping và spreadsheet formula neutralization là hai lớp khác nhau.
* **Hướng xử lý đề xuất:** Prefix/quote an toàn cho ô text nguy hiểm và test cả CSV/Excel.
* **Có cần sửa CSDL không:** Không.

### KT-11: Cần thống nhất phạm vi nghiệp vụ Chi phí độc lập

* **Loại phát hiện:** C
* **Mức độ:** Thấp
* **Mức chắc chắn:** Cao
* **Module:** Chi phí/Ngân sách
* **File:** `Permissions.cs`; `DyetDeXuatCongViecService.cs`; Dashboard và `DuAn/Details`
* **Class/hàm/action:** `Permissions.ChiPhi.*`; `DuyetAsync`
* **Luồng nghiệp vụ:** Duyệt đề xuất công việc sinh `ChiPhi`; UI chỉ xem tổng hợp/gián tiếp.
* **Bằng chứng source:** Không `ChiPhiController`; `ChiPhi.Xem` được dùng gián tiếp; `Them/Sua` chưa có action.
* **Cách tái hiện:** Đăng nhập Manager và theo các link tài chính.
* **Kết quả hiện tại:** Chi phí là dữ liệu sinh từ workflow, không phải CRUD độc lập.
* **Kết quả mong đợi:** Cần chủ sản phẩm xác nhận đây là thiết kế cuối hay cần module riêng.
* **Ảnh hưởng:** Permission `Them/Sua` gây kỳ vọng không khớp UI.
* **Nguyên nhân:** Permission model rộng hơn implementation.
* **Hướng xử lý đề xuất:** Giữ workflow gián tiếp hoặc đặc tả module riêng; không tự thêm Controller.
* **Có cần sửa CSDL không:** Không.

### KT-12: Cần thống nhất có cần màn hình nhật ký độc lập

* **Loại phát hiện:** C
* **Mức độ:** Giao diện
* **Mức chắc chắn:** Cao
* **Module:** Nhật ký
* **File:** `Permissions.cs`; `DuAnService.cs`; `Views/DuAn/Details.cshtml`; `_Layout.cshtml`
* **Class/hàm/action:** `NhatKy.Xem`, truy vấn activity
* **Luồng nghiệp vụ:** User có quyền → xem tab hoạt động trong chi tiết dự án.
* **Bằng chứng source:** Permission đang dùng; không có `NhatKyController`/màn hình tổng hợp.
* **Cách tái hiện:** Mở chi tiết dự án với/không có `NhatKy.Xem`.
* **Kết quả hiện tại:** Nhật ký theo ngữ cảnh dự án.
* **Kết quả mong đợi:** Cần thống nhất có cần tra cứu toàn hệ thống hay tab hiện tại đã đủ.
* **Ảnh hưởng:** Khả năng truy vết liên dự án hạn chế, không phải lỗi phân quyền.
* **Nguyên nhân:** Thiết kế contextual audit.
* **Hướng xử lý đề xuất:** Đặc tả nhu cầu trước khi thay đổi.
* **Có cần sửa CSDL không:** Không.

### 11.1 Tổng hợp phát hiện

| Loại | Cao | Trung bình | Thấp | Giao diện | Tổng |
|---|---:|---:|---:|---:|---:|
| A | 6 | 1 | 1 | 0 | 8 |
| B | 0 | 2 | 0 | 0 | 2 |
| C | 0 | 0 | 1 | 1 | 2 |
| **Tổng** | **6** | **3** | **2** | **1** | **12** |

Không có phát hiện mức **Nghiêm trọng** sau khi chuẩn hóa: static file được hạ từ Nghiêm trọng xuống Cao vì tên lưu là GUID và View không công khai URL tĩnh, dù bypass authorization vẫn được source chứng minh.

---

## 12. Kết luận tổng thể

### 12.1 Nghiệp vụ đang đúng

- Scope dự án/Manager/Employee/Leader được kiểm chủ yếu tại service, không chỉ ẩn nút.
- Chi tiết công việc luôn tạo `ChuaBatDau`; trạng thái thật đi qua tiến độ đã duyệt/workflow.
- Ba luồng duyệt đề xuất công việc, ngân sách và đổi quản lý dùng `Serializable` và recheck trạng thái.
- Khóa tài khoản chặn tự khóa và Admin cuối cùng.
- Chat chặn Admin ở service và membership, dù Admin có claim xem.
- File View tải qua Controller có permission/scope.
- AI dùng 22 feature đồng bộ, ground truth từ `AI_NGUYEN_NHAN`, không dùng `AI_KET_QUA` làm nhãn.
- FastAPI không ghi SQL Server; AI chỉ hỗ trợ và Manager xác nhận.

### 12.2 Lỗi cần sửa ngay

Chỉ các lỗi loại A mức Cao:

1. KT-01 — hồ sơ xóa vẫn có thể login.
2. KT-02 — 55 POST thiếu anti-forgery.
3. KT-03 — static URL bypass quyền file.
4. KT-04 — login không có failed-attempt lockout/rate limit.
5. KT-05 — xóa không chặn self/Admin cuối.
6. KT-06 — session/claim không bị thu hồi.

### 12.3 Rủi ro cần kiểm thử

- KT-09 — cạnh tranh tiến độ và nhất quán model MVC–FastAPI.
- KT-10 — CSV formula injection với ứng dụng spreadsheet thực tế.

Ngoài ra nên runtime test SMTP delivery/rollback token, reverse proxy `/uploads`, dung lượng upload và export dataset lớn; source review không thay thế các kiểm thử này.

### 12.4 Điểm cần thống nhất nghiệp vụ

- KT-11 — chi phí chỉ sinh/quản lý gián tiếp hay cần module độc lập.
- KT-12 — nhật ký theo tab dự án hay cần màn hình tổng hợp.
- Admin chat **không còn là điểm mơ hồ trong source**: service hiện chủ đích chặn Admin. Nếu sản phẩm muốn Admin đọc chat, đó là yêu cầu thay đổi nghiệp vụ mới.

### 12.5 Nội dung không cần sửa chỉ vì khác mẫu kiến trúc

- Không dùng SignalR: chat polling/refresh vẫn là lựa chọn khả dụng.
- Không có repository pattern: service dùng DbContext trực tiếp không tự thân là lỗi.
- FastAPI không ghi DB: là phân tách trách nhiệm hợp lý.
- Leader không phải role hệ thống: là role dữ liệu theo team/dự án.
- Chi phí sinh khi duyệt đề xuất: phù hợp AS-IS nếu được chủ sản phẩm xác nhận.
- AI chỉ là công cụ hỗ trợ/fallback: đúng ranh giới quyết định nghiệp vụ.

---

## 13. Danh sách ưu tiên xử lý

### Ưu tiên 1 – lỗi đã xác nhận

1. KT-01: Chặn authentication cho hồ sơ soft-delete.
2. KT-06: Thu hồi/validate session sau khóa, xóa, đổi role/permission.
3. KT-05: Chặn tự xóa và xóa Admin cuối.
4. KT-02: Bảo vệ anti-forgery cho 55 POST.
5. KT-03: Loại đường static đối với file private.
6. KT-04: Bổ sung rate limit/failed-attempt lockout.
7. KT-07: Validation upload dự án.
8. KT-08: Compensating file–DB.

### Ưu tiên 2 – cần test trước khi sửa

1. KT-09: Test song song gửi/duyệt tiến độ và fault injection model.
2. KT-10: Test CSV/Excel formula.
3. Test deploy thực tế của `/uploads`, SMTP, export lớn và cleanup file.

### Ưu tiên 3 – thống nhất nghiệp vụ

1. KT-11: Module/quyền Chi phí.
2. KT-12: Màn hình Nhật ký.
3. Nếu thay đổi chủ đích hiện tại: quyền Admin đọc chat.

### Ưu tiên 4 – cải thiện chất lượng

- DI thống nhất, tối ưu export streaming, UI/CSS, SignalR, refactor service lớn.
- Các mục này không được tính vào 12 phát hiện và không phải lỗi cần sửa ngay.

---

## 14. Phụ lục permission

Nguồn: `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`. Số thứ tự liên tục và mỗi constant xuất hiện đúng một lần.

| # | Constant | Giá trị | Trạng thái dùng |
|---:|---|---|---|
| 1 | `ThongKe.Xem` | `ThongKe.Xem` | Controller/View |
| 2 | `ThongKe.XuatFile` | `ThongKe.XuatFile` | Các action export |
| 3 | `NhanSu.Xem` | `NhanSu.Xem` | Controller/View |
| 4 | `NhanSu.Them` | `NhanSu.Them` | Controller/View |
| 5 | `NhanSu.Sua` | `NhanSu.Sua` | Controller/View |
| 6 | `NhanSu.Xoa` | `NhanSu.Xoa` | Controller/View |
| 7 | `NhanSu.Khoa` | `NhanSu.Khoa` | Controller/View |
| 8 | `NhanSu.MoKhoa` | `NhanSu.MoKhoa` | Controller/View |
| 9 | `PhanQuyen.Xem` | `PhanQuyen.Xem` | Controller/View |
| 10 | `PhanQuyen.Luu` | `PhanQuyen.Luu` | Controller/View |
| 11 | `ChucDanh.Xem` | `ChucDanh.Xem` | Controller/View |
| 12 | `ChucDanh.Them` | `ChucDanh.Them` | Controller/View |
| 13 | `ChucDanh.Sua` | `ChucDanh.Sua` | Controller/View |
| 14 | `ChucDanh.Xoa` | `ChucDanh.Xoa` | Controller/View |
| 15 | `Nhom.Xem` | `Nhom.Xem` | `TeamController`/View |
| 16 | `Nhom.Them` | `Nhom.Them` | `TeamController`/View |
| 17 | `Nhom.Sua` | `Nhom.Sua` | `TeamController`/View |
| 18 | `Nhom.Xoa` | `Nhom.Xoa` | `TeamController`/View |
| 19 | `ThanhVienNhom.Xem` | `ThanhVienNhom.Xem` | Controller |
| 20 | `ThanhVienNhom.Them` | `ThanhVienNhom.Them` | Controller/View |
| 21 | `ThanhVienNhom.Xoa` | `ThanhVienNhom.Xoa` | Controller/View |
| 22 | `DuyetYeuCauDoiQuanLy.Xem` | cùng tên | Controller/View |
| 23 | `DuyetYeuCauDoiQuanLy.Duyet` | cùng tên | Controller/Service/View |
| 24 | `DuAn.Xem` | `DuAn.Xem` | Controller/View |
| 25 | `DuAn.Them` | `DuAn.Them` | Controller/View |
| 26 | `DuAn.Sua` | `DuAn.Sua` | Controller/View |
| 27 | `DuAn.Xoa` | `DuAn.Xoa` | Controller/View |
| 28 | `YeuCauDoiQuanLy.Xem` | cùng tên | Controller |
| 29 | `YeuCauDoiQuanLy.Them` | cùng tên | Controller/Service/View |
| 30 | `TeamDuAn.Xem` | `TeamDuAn.Xem` | Controller/View |
| 31 | `TeamDuAn.Them` | `TeamDuAn.Them` | Controller/View |
| 32 | `TeamDuAn.Xoa` | `TeamDuAn.Xoa` | Controller/View |
| 33 | `ThanhVienDuAn.Xem` | cùng tên | `NhanVienDuAnController`/View |
| 34 | `ThanhVienDuAn.Them` | cùng tên | Controller/View |
| 35 | `ThanhVienDuAn.Xoa` | cùng tên | Controller/View |
| 36 | `CongViec.Xem` | `CongViec.Xem` | Controller/View |
| 37 | `DanhMucCongViec.Xem` | cùng tên | Controller/Service/View |
| 38 | `DanhMucCongViec.Them` | cùng tên | Controller/Service |
| 39 | `DanhMucCongViec.Sua` | cùng tên | Controller/Service |
| 40 | `DanhMucCongViec.Xoa` | cùng tên | Controller/Service |
| 41 | `DeXuatCongViec.Xem` | cùng tên | Controller/View |
| 42 | `DeXuatCongViec.Them` | cùng tên | Controller/View |
| 43 | `DuyetDeXuatCongViec.Xem` | cùng tên | Controller/View |
| 44 | `DuyetDeXuatCongViec.Duyet` | cùng tên | Controller/View/workflow |
| 45 | `ChiTietCongViec.Xem` | cùng tên | Controller/View |
| 46 | `ChiTietCongViec.Them` | cùng tên | Controller/View |
| 47 | `ChiTietCongViec.Sua` | cùng tên | Controller/View |
| 48 | `ChiTietCongViec.Xoa` | cùng tên | Controller/View |
| 49 | `PhanCongCongViec.Xem` | cùng tên | Controller/View |
| 50 | `PhanCongCongViec.ThucHien` | cùng tên | Controller/View |
| 51 | `PhanCongChiTietCongViec.Xem` | cùng tên | Controller/View |
| 52 | `PhanCongChiTietCongViec.ThucHien` | cùng tên | Controller/View |
| 53 | `TienDo.Xem` | `TienDo.Xem` | Controller/View |
| 54 | `TienDo.CapNhat` | `TienDo.CapNhat` | Controller/View |
| 55 | `TienDo.Duyet` | `TienDo.Duyet` | Controller/View/workflow |
| 56 | `DeXuatNganSach.Xem` | cùng tên | Controller/View |
| 57 | `DeXuatNganSach.Them` | cùng tên | Controller/View |
| 58 | `DuyetNganSach.Xem` | `DuyetNganSach.Xem` | Controller/View |
| 59 | `DuyetNganSach.Duyet` | `DuyetNganSach.Duyet` | Controller/View |
| 60 | `NganSach.Xem` | `NganSach.Xem` | Controller/View |
| 61 | `ChiPhi.Xem` | `ChiPhi.Xem` | Dùng gián tiếp Dashboard/Details |
| 62 | `ChiPhi.Them` | `ChiPhi.Them` | Seed/definition, chưa có action |
| 63 | `ChiPhi.Sua` | `ChiPhi.Sua` | Seed/definition, chưa có action |
| 64 | `AI.Xem` | `AI.Xem` | Layout/permission cha |
| 65 | `AI.Dataset` | `AI.Dataset` | `AiDatasetController` |
| 66 | `AI.Train` | `AI.Train` | `AiController` |
| 67 | `AI.PhanTichNguyenNhan` | cùng tên | `AiController`, `DuAnController`, service |
| 68 | `AI.DuDoan` | alias của #67 | Tương thích cũ, không dùng trực tiếp |
| 69 | `AI.Dashboard` | `AI.Dashboard` | `AiController`/layout |
| 70 | `AI.XacNhan` | `AI.XacNhan` | `AiController`, `DuAnController`, service |
| 71 | `DanhGiaDuAn.Xem` | cùng tên | Controller/Service/View |
| 72 | `DanhGiaDuAn.DanhGia` | cùng tên | Controller/Service/View |
| 73 | `DanhGiaDuAn.Sua` | cùng tên | Controller/Service/View |
| 74 | `DanhGiaDuAn.Duyet` | cùng tên | Controller/Service |
| 75 | `DanhGiaNhanVien.Xem` | cùng tên | Controller/Service/View |
| 76 | `DanhGiaNhanVien.DanhGia` | cùng tên | Controller/Service/View |
| 77 | `DanhGiaNhanVien.Sua` | cùng tên | Controller/Service/View |
| 78 | `DanhGiaNhanVien.Duyet` | cùng tên | Controller/Service |
| 79 | `Chat.Xem` | `Chat.Xem` | Controller/Service/View |
| 80 | `Chat.Gui` | `Chat.Gui` | Controller/Service/View |
| 81 | `NhatKy.Xem` | `NhatKy.Xem` | Tab hoạt động/layout |

Kiểm tra số học: 81 dòng constant = 80 giá trị unique + 1 alias.

---

## 15. Thống kê cuối tài liệu

| Hạng mục | Số lượng chính xác | Ghi chú |
|---|---:|---|
| Controller đã kiểm tra | 30 | Toàn bộ `Controllers/` |
| Module nghiệp vụ | 29 | Không tính `HomeController` |
| Permission constant | 81 | Không tính constant tên claim |
| Permission unique | 80 | Theo giá trị string sau resolve alias |
| Permission alias | 1 | `AI.DuDoan` |
| View `.cshtml` | 126 | Đếm đệ quy |
| File JavaScript | 5 | Không tính inline script |
| Action GET | 73 | 69 explicit + 4 convention |
| Action POST | 87 | `[HttpPost]` |
| POST thay đổi dữ liệu/trạng thái | 84 | Gồm DB, cookie/session, email hoặc model; loại 3 validation-only |
| POST được bảo vệ anti-forgery | 32 | Filter tại action; global = 0 |
| Vấn đề loại A | 8 | KT-01…KT-08 |
| Vấn đề loại B | 2 | KT-09…KT-10 |
| Vấn đề loại C | 2 | KT-11…KT-12 |
| Nghiêm trọng | 0 | |
| Cao | 6 | KT-01…KT-06 |
| Trung bình | 3 | KT-07, KT-09, KT-10 |
| Thấp | 2 | KT-08, KT-11 |
| Giao diện | 1 | KT-12 |

Đối chiếu:

- Loại A/B/C: 8 + 2 + 2 = **12 vấn đề**.
- Mức độ: 0 + 6 + 3 + 2 + 1 = **12 vấn đề**.
- Hai bảng tại 11.1 và 15 cho cùng kết quả.

**Xác nhận phạm vi:** Chỉ `docs/kiemtratongquat.md` được cập nhật. Không chỉnh sửa source code, View, Controller, Service, JavaScript, CSS, FastAPI, CSDL hoặc migration.
