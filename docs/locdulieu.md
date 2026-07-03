# Phân tích lọc dữ liệu duyệt công việc và duyệt ngân sách

Tài liệu này chỉ ghi nhận kết quả đọc source trong checkout `dvl22`. Không sửa code runtime, không sửa CSDL, không tạo migration, không seed dữ liệu, không đổi giao diện/CSS/font.

## 1. Tổng quan vấn đề

Hai màn hình được kiểm tra:

- Duyệt đề xuất công việc: route mặc định `/DuyetDeXuatCongViec/Index`.
- Duyệt đề xuất ngân sách: route mặc định `/DuyetDeXuatNganSach/Index`.

Kết luận AS-IS: cả hai màn hình duyệt hiện chỉ có 2 filter trên UI:

- `locMaDuAn`: ô `input type="number"` có label "Mã dự án", người dùng phải tự biết mã dự án.
- `locTrangThai`: `select` trạng thái, đã tương đối hợp lý.

Không tìm thấy filter người đề xuất, ngày đề xuất, ngày duyệt, từ khóa, mã đề xuất, mã người gửi, mã người duyệt, mã nhân viên, mã team, mã loại công việc hoặc khoảng tiền trên hai màn hình duyệt. Các nơi đã kiểm tra: controller, service, ViewModel, view `Index/_Filter/_Table`, entity, shared pagination, JS approval và constants.

## 2. Danh sách file liên quan

### Duyệt đề xuất công việc

| Nhóm | File/class/method liên quan |
|---|---|
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/DuyetDeXuatCongViecController.cs`, class `DuyetDeXuatCongViecController`, action `Index`, `Duyet`, `TuChoi`. |
| Service interface | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDuyetDeXuatCongViecService.cs`, interface `IDuyetDeXuatCongViecService`, method `GetPageAsync`, `ApproveAsync`, `RejectAsync`. |
| Service implementation | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DyetDeXuatCongViecService.cs`, class `DyetDeXuatCongViecService`, method `GetPageAsync`, `ApproveAsync`, `RejectAsync`, `EnsureIsProjectManagerAsync`, `GetCurrentUserIdAsync`. Lưu ý tên file/class là `Dyet...`, thiếu chữ `u`. |
| DI registration | `QuanLyDuAn/QuanLyDuAn/Services/CauHinhDichVu.cs`, dòng đăng ký `AddScoped<IDuyetDeXuatCongViecService, DyetDeXuatCongViecService>()`. `Program.cs` đã kiểm tra nhưng không tìm thấy đăng ký trực tiếp theo tên service này. |
| View | `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatCongViec/Index.cshtml`, `_Filter.cshtml`, `_Table.cshtml`. |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/DuyetDeXuatCongViec/DuyetDeXuatCongViecPageViewModel.cs`, `DuyetDeXuatCongViecItemViewModel.cs`. |
| Entity | `QuanLyDuAn/QuanLyDuAn/Models/Entities/DeXuatCongViec.cs`, `DuAn.cs`, `NguoiDung.cs`, `DanhMucCongViec.cs`, `MucDoUuTien.cs`, `CongViec.cs`, `ChiPhi.cs`, `NganSach.cs`. |
| DbContext | `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`, `DbSet<DeXuatCongViec>`, `DbSet<DuAn>`, `DbSet<NguoiDung>`, mapping `Entity<DeXuatCongViec>`. |
| Constants | `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`, nested class `Permissions.DuyetDeXuatCongViec`; `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs`, constants `ChoDuyet`, `DaDuyet`, `TuChoi`, `DaHuy`, method `GetCommonStatusVariants`, `ToDisplay`. |
| JavaScript | `QuanLyDuAn/QuanLyDuAn/wwwroot/js/approval/index.js`, xử lý `.js-confirm-submit` và `.js-toggle-detail`. |
| CSS liên quan nhưng không sửa | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/approval.css`, `wwwroot/css/DuyetDeXuatCongViec/index.css`. |
| Tài liệu cũ đã đối chiếu | `docs/duyetdexuatcongviec.md`. |

### Duyệt đề xuất ngân sách

| Nhóm | File/class/method liên quan |
|---|---|
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/DuyetDeXuatNganSachController.cs`, class `DuyetDeXuatNganSachController`, action `Index`, `Duyet`, `TuChoi`. |
| Service interface | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDuyetDeXuatNganSachService.cs`, interface `IDuyetDeXuatNganSachService`, method `GetPageAsync`, `ApproveAsync`, `RejectAsync`. |
| Service implementation | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuyetDeXuatNganSachService.cs`, class `DuyetDeXuatNganSachService`, method `GetPageAsync`, `ApproveAsync`, `RejectAsync`, `EnsureIsProjectManagerAsync`, `GetCurrentUserIdAsync`. |
| DI registration | `QuanLyDuAn/QuanLyDuAn/Services/CauHinhDichVu.cs`, dòng đăng ký `AddScoped<IDuyetDeXuatNganSachService, DuyetDeXuatNganSachService>()`. `Program.cs` đã kiểm tra nhưng không tìm thấy đăng ký trực tiếp theo tên service này. |
| View | `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatNganSach/Index.cshtml`, `_Filter.cshtml`, `_Table.cshtml`. |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/DuyetDeXuatNganSach/DuyetDeXuatNganSachPageViewModel.cs`, `DuyetDeXuatNganSachItemViewModel.cs`. |
| Entity | `QuanLyDuAn/QuanLyDuAn/Models/Entities/DeXuatNganSach.cs`, `NganSach.cs`, `DuAn.cs`, `NguoiDung.cs`, `NhatKyNganSach.cs`, `NhatKyQuanLyDuAn.cs`. |
| DbContext | `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`, `DbSet<DeXuatNganSach>`, `DbSet<NganSach>`, `DbSet<DuAn>`, `DbSet<NguoiDung>`, mapping `Entity<DeXuatNganSach>`. |
| Constants | `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`, nested class `Permissions.DuyetNganSach`; `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs`. |
| JavaScript | `QuanLyDuAn/QuanLyDuAn/wwwroot/js/approval/index.js`. |
| CSS liên quan nhưng không sửa | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/approval.css`, `wwwroot/css/DuyetDeXuatNganSach/index.css`. |
| Tài liệu cũ đã đối chiếu | `docs/duyetngansach.md`. |

Không tìm thấy controller tên `DeXuatNganSachController` là màn duyệt; file này thuộc màn tạo/xem đề xuất của người đề xuất. Không tìm thấy controller tên `DuyetNganSachController`; màn duyệt ngân sách dùng `DuyetDeXuatNganSachController`.

## 3. Luồng chức năng duyệt đề xuất công việc

### Action danh sách

- File: `QuanLyDuAn/QuanLyDuAn/Controllers/DuyetDeXuatCongViecController.cs`.
- Class: `DuyetDeXuatCongViecController`.
- Method: `Index(int? locMaDuAn, string? locTrangThai, int pageNumber = 1, int pageSize = 20)`.
- HTTP: `[HttpGet]`.
- Route/URL mặc định: `/DuyetDeXuatCongViec/Index`, hoặc `/DuyetDeXuatCongViec?locMaDuAn=...&locTrangThai=...&pageNumber=...&pageSize=...` theo conventional route.
- Quyền xem: `_permission.HasPermissionAsync(User, Permissions.DuyetDeXuatCongViec.Xem)`.
- Service gọi: `_service.GetPageAsync(locMaDuAn, locTrangThai, pageNumber, pageSize)`.
- Permission list cho view: `_phanQuyenService.GetGrantedPermissionNamesAsync(User)` gán vào `vm.Permissions`.

### Action duyệt

- File: `DuyetDeXuatCongViecController.cs`.
- Method: `Duyet(int maDeXuatCv, int? locMaDuAn, string? locTrangThai)`.
- HTTP: `[HttpPost]`.
- Quyền duyệt: `Permissions.DuyetDeXuatCongViec.Duyet`.
- Service gọi: `_service.ApproveAsync(maDeXuatCv)`.
- Redirect: `RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai })`.
- Nhận xét: action duyệt giữ `locMaDuAn` và `locTrangThai`, nhưng không nhận/không redirect `pageNumber`, `pageSize`; do đó sau khi duyệt có thể quay về trang 1.

### Action từ chối

- File: `DuyetDeXuatCongViecController.cs`.
- Method: `TuChoi(int maDeXuatCv, int? locMaDuAn, string? locTrangThai, int pageNumber = 1, int pageSize = 20)`.
- HTTP: `[HttpPost]`.
- Quyền từ chối: dùng cùng quyền `Permissions.DuyetDeXuatCongViec.Duyet`.
- Service gọi: `_service.RejectAsync(maDeXuatCv)`.
- Redirect: `RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai, pageNumber, pageSize })`.
- Nhận xét: action từ chối giữ cả filter và phân trang.

### Service/list query

- File: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DyetDeXuatCongViecService.cs`.
- Method: `GetPageAsync(int? locMaDuAn, string? locTrangThai, int pageNumber, int pageSize, bool paginate)`.
- Current user: `GetCurrentUserIdAsync()` đọc `ClaimTypes.NameIdentifier`, tra `Aspnetusers.MaNguoiDung`.
- Query chính:
  - Entity/bảng nguồn: `_context.DeXuatCongViec` (`DE_XUAT_CONG_VIEC`), `_context.DuAn` (`DU_AN`), `_context.NguoiDung` (`NGUOI_DUNG`), `_context.DanhMucCongViec`, `_context.MucDoUuTien`.
  - Điều kiện mặc định: `dx.IsDeleted != true`, `da.IsDeleted != true`, `da.MaNguoiDung == currentUserId`.
  - Scope dữ liệu: chỉ dự án mà người dùng hiện tại là quản lý dự án theo `DU_AN.MaNguoiDung`.
  - Filter dự án: `query.Where(x => x.MaDuAn == locMaDuAn.Value)`.
  - Filter trạng thái: `TrangThai.GetCommonStatusVariants(locTrangThai)` rồi `filterValues.Contains(x.TrangThaiCongViecDeXuat)`.
  - Sắp xếp: `OrderByDescending(x => x.NgayDeXuatCongViec).ThenByDescending(x => x.MaDeXuatCV)`.
  - Phân trang: `PaginationViewModel.Create`, `Skip(pagination.Skip)`, `Take(pagination.PageSize)`.

### View/list

- `Views/DuyetDeXuatCongViec/Index.cshtml` render `_Filter`, `_Table`, shared `_Pagination`, và script `~/js/approval/index.js`.
- `Views/DuyetDeXuatCongViec/_Filter.cshtml` có form GET.
- `Views/DuyetDeXuatCongViec/_Table.cshtml` có form POST duyệt/từ chối, hidden field giữ filter.

## 4. Luồng chức năng duyệt đề xuất ngân sách

### Action danh sách

- File: `QuanLyDuAn/QuanLyDuAn/Controllers/DuyetDeXuatNganSachController.cs`.
- Class: `DuyetDeXuatNganSachController`.
- Method: `Index(int? locMaDuAn, string? locTrangThai, int pageNumber = 1, int pageSize = 20)`.
- HTTP: `[HttpGet]`.
- Route/URL mặc định: `/DuyetDeXuatNganSach/Index`, hoặc `/DuyetDeXuatNganSach?locMaDuAn=...&locTrangThai=...&pageNumber=...&pageSize=...`.
- Quyền xem: `_permission.HasPermissionAsync(User, Permissions.DuyetNganSach.Xem)`.
- Service gọi: `_service.GetPageAsync(locMaDuAn, locTrangThai, pageNumber, pageSize)`.

### Action duyệt

- File: `DuyetDeXuatNganSachController.cs`.
- Method: `Duyet(int maDeXuatNs, int? locMaDuAn, string? locTrangThai)`.
- HTTP: `[HttpPost]`.
- Quyền duyệt: `Permissions.DuyetNganSach.Duyet`.
- Service gọi: `_service.ApproveAsync(maDeXuatNs)`.
- Redirect: `RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai })`.
- Nhận xét: giống công việc, action duyệt giữ filter dự án/trạng thái nhưng không giữ `pageNumber`, `pageSize`.

### Action từ chối

- File: `DuyetDeXuatNganSachController.cs`.
- Method: `TuChoi(int maDeXuatNs, int? locMaDuAn, string? locTrangThai, int pageNumber = 1, int pageSize = 20)`.
- HTTP: `[HttpPost]`.
- Quyền từ chối: dùng cùng quyền `Permissions.DuyetNganSach.Duyet`.
- Service gọi: `_service.RejectAsync(maDeXuatNs)`.
- Redirect: `RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai, pageNumber, pageSize })`.

### Service/list query

- File: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuyetDeXuatNganSachService.cs`.
- Method: `GetPageAsync(int? locMaDuAn, string? locTrangThai, int pageNumber, int pageSize, bool paginate)`.
- Current user: `GetCurrentUserIdAsync()` đọc `ClaimTypes.NameIdentifier`, tra `Aspnetusers.MaNguoiDung`.
- Query chính:
  - Entity/bảng nguồn: `_context.DeXuatNganSach` (`DE_XUAT_NGAN_SACH`), `_context.DuAn`, `_context.NguoiDung`.
  - Điều kiện mặc định: `dx.IsDeleted != true`, `da.IsDeleted != true`, `da.MaNguoiDung == currentUserId`.
  - Scope dữ liệu: chỉ dự án mà người dùng hiện tại là quản lý dự án theo `DU_AN.MaNguoiDung`.
  - Filter dự án: `query.Where(x => x.MaDuAn == locMaDuAn.Value)`.
  - Filter trạng thái: `TrangThai.GetCommonStatusVariants(locTrangThai)` rồi `filterValues.Contains(x.TrangThaiDeXuat)`.
  - Sắp xếp: `OrderByDescending(x => x.NgayDeXuat).ThenByDescending(x => x.MaDeXuatNS)`.
  - Phân trang: `PaginationViewModel.Create`, `Skip(pagination.Skip)`, `Take(pagination.PageSize)`.

### View/list

- `Views/DuyetDeXuatNganSach/Index.cshtml` render `_Filter`, `_Table`, shared `_Pagination`, và script `~/js/approval/index.js`.
- `Views/DuyetDeXuatNganSach/_Filter.cshtml` có form GET.
- `Views/DuyetDeXuatNganSach/_Table.cshtml` có form POST duyệt/từ chối, hidden field giữ filter.

## 5. Filter hiện tại của duyệt đề xuất công việc

| Tên filter hiển thị trên giao diện | Tên input HTML | Kiểu input hiện tại | Người dùng đang phải nhập mã gì | Property ViewModel/tham số action | Query string/form field | Backend dùng filter | Entity/bảng/cột lọc | Nhận xét UX |
|---|---|---|---|---|---|---|---|---|
| Mã dự án | `locMaDuAn` | `input type="number"` | Mã dự án (`DU_AN.MaDuAn`) | `DuyetDeXuatCongViecPageViewModel.LocMaDuAn`; action `Index(int? locMaDuAn, ...)`; service `GetPageAsync(int? locMaDuAn, ...)` | GET query string `locMaDuAn`; hidden field trong form POST duyệt/từ chối | `DyetDeXuatCongViecService.GetPageAsync`, đoạn `if (locMaDuAn.HasValue) query = query.Where(x => x.MaDuAn == locMaDuAn.Value)` | ViewModel item `MaDuAn`, lấy từ `DeXuatCongViec.MaDuAn` join `DuAn.MaDuAn`; bảng `DE_XUAT_CONG_VIEC`, `DU_AN` | Chưa hợp lý: người dùng phải nhớ mã dự án dù list đã có `TenDuAn`. Nên đổi sang dropdown/searchable select theo tên dự án trong scope quản lý. |
| Trạng thái đề xuất | `locTrangThai` | `select` | Không phải nhập mã tay; chọn option trạng thái | `DuyetDeXuatCongViecPageViewModel.LocTrangThai`; action `Index(string? locTrangThai, ...)`; service `GetPageAsync(string? locTrangThai, ...)` | GET query string `locTrangThai`; hidden field trong form POST duyệt/từ chối | `DyetDeXuatCongViecService.GetPageAsync`, `TrangThai.GetCommonStatusVariants(locTrangThai)`, filter `TrangThaiCongViecDeXuat` | `DeXuatCongViec.TrangThaiCongViecDeXuat` / `DE_XUAT_CONG_VIEC.TrangThaiCongViecDeXuat` | Hợp lý cơ bản vì là select; options lấy từ constants `TrangThai`. Có thể giữ. |

Không tìm thấy filter người đề xuất, người duyệt, mã đề xuất, mã nhân viên, mã team, mã loại công việc, ngày gửi, từ khóa ở `Views/DuyetDeXuatCongViec/_Filter.cshtml`, `DuyetDeXuatCongViecController.Index`, `DuyetDeXuatCongViecPageViewModel`, `DyetDeXuatCongViecService.GetPageAsync`.

## 6. Filter hiện tại của duyệt đề xuất ngân sách

| Tên filter hiển thị trên giao diện | Tên input HTML | Kiểu input hiện tại | Người dùng đang phải nhập mã gì | Property ViewModel/tham số action | Query string/form field | Backend dùng filter | Entity/bảng/cột lọc | Nhận xét UX |
|---|---|---|---|---|---|---|---|---|
| Mã dự án | `locMaDuAn` | `input type="number"` | Mã dự án (`DU_AN.MaDuAn`) | `DuyetDeXuatNganSachPageViewModel.LocMaDuAn`; action `Index(int? locMaDuAn, ...)`; service `GetPageAsync(int? locMaDuAn, ...)` | GET query string `locMaDuAn`; hidden field trong form POST duyệt/từ chối | `DuyetDeXuatNganSachService.GetPageAsync`, đoạn `if (locMaDuAn.HasValue) query = query.Where(x => x.MaDuAn == locMaDuAn.Value)` | ViewModel item `MaDuAn`, lấy từ `DeXuatNganSach.MaDuAn` join `DuAn.MaDuAn`; bảng `DE_XUAT_NGAN_SACH`, `DU_AN` | Chưa hợp lý: người dùng phải nhớ mã dự án. Nên đổi sang dropdown/searchable select theo tên dự án trong scope quản lý. |
| Trạng thái đề xuất | `locTrangThai` | `select` | Không phải nhập mã tay; chọn option trạng thái | `DuyetDeXuatNganSachPageViewModel.LocTrangThai`; action `Index(string? locTrangThai, ...)`; service `GetPageAsync(string? locTrangThai, ...)` | GET query string `locTrangThai`; hidden field trong form POST duyệt/từ chối | `DuyetDeXuatNganSachService.GetPageAsync`, `TrangThai.GetCommonStatusVariants(locTrangThai)`, filter `TrangThaiDeXuat` | `DeXuatNganSach.TrangThaiDeXuat` / `DE_XUAT_NGAN_SACH.TrangThaiDeXuat` | Hợp lý cơ bản vì là select; options lấy từ constants `TrangThai`. Có thể giữ. |

Không tìm thấy filter người đề xuất, người duyệt, mã đề xuất ngân sách, mã ngân sách, ngày gửi, từ khóa, khoảng số tiền ở `Views/DuyetDeXuatNganSach/_Filter.cshtml`, `DuyetDeXuatNganSachController.Index`, `DuyetDeXuatNganSachPageViewModel`, `DuyetDeXuatNganSachService.GetPageAsync`.

## 7. Các ô đang nhập mã bằng tay và đánh giá UX

| Màn hình | Ô nhập mã bằng tay | File/property liên quan | Đánh giá |
|---|---|---|---|
| Duyệt đề xuất công việc | `locMaDuAn`, label "Mã dự án", placeholder "Nhập mã dự án" | `Views/DuyetDeXuatCongViec/_Filter.cshtml`; `DuyetDeXuatCongViecPageViewModel.LocMaDuAn`; `DyetDeXuatCongViecService.GetPageAsync` | Ưu tiên cao. Đây là ID kỹ thuật, người dùng bình thường không nhớ. Source đã join ra `TenDuAn`, nên có đủ cơ sở đổi sang chọn theo tên. |
| Duyệt đề xuất ngân sách | `locMaDuAn`, label "Mã dự án", placeholder "Nhập mã dự án" | `Views/DuyetDeXuatNganSach/_Filter.cshtml`; `DuyetDeXuatNganSachPageViewModel.LocMaDuAn`; `DuyetDeXuatNganSachService.GetPageAsync` | Ưu tiên cao. Cùng vấn đề UX với màn công việc. |

Không tìm thấy ô nhập mã người gửi/người duyệt/nhân viên/team/đề xuất/loại công việc/ngân sách/trạng thái. Trạng thái hiện là select, không phải nhập mã tay.

Backend không có lỗi nghiệp vụ rõ ràng trong filter hiện tại; vấn đề chính là UI/UX và thiếu bộ lọc thuận tiện. Backend query theo ID vẫn hợp lệ, nên khi sửa UI có thể giữ value là ID để tránh thay đổi database.

## 8. Nguồn dữ liệu có thể dùng cho dropdown/autocomplete

### Dự án (`locMaDuAn`)

- Nên dùng: searchable dropdown nếu số dự án nhiều; select thường nếu danh sách nhỏ.
- Value giữ nguyên: `MaDuAn`.
- Text hiển thị: `TenDuAn`, fallback có thể dùng `"Dự án {MaDuAn}"`.
- Nguồn dữ liệu: bảng/entity `DU_AN` / `DuAn`, property `MaDuAn`, `TenDuAn`, `MaNguoiDung`, `IsDeleted`.
- Scope bắt buộc: với màn duyệt hiện tại, service list đang dùng `da.MaNguoiDung == currentUserId`; dropdown cũng phải chỉ lấy các dự án `DuAn.IsDeleted != true && DuAn.MaNguoiDung == currentUserId`.
- Có cần endpoint API autocomplete không: chưa bắt buộc nếu dùng select nạp server-side. Nếu dữ liệu dự án rất lớn, có thể thêm endpoint JSON có debounce, nhưng nên cân nhắc sau.
- Có cần sửa service lấy option không: có, nên thêm method hoặc mở rộng `GetPageAsync` để nạp danh sách option dự án theo scope duyệt.
- Có cần sửa ViewModel không: có, thêm list option, ví dụ `DanhSachDuAn`.
- Có ảnh hưởng database không: không.

Nguồn tham khảo trong source hiện có: `DeXuatCongViecService.GetEligibleProjectOptionsAsync` và `DeXuatNganSachService.GetEligibleProjectOptionsAsync` đã có pattern tạo danh sách dự án cho màn đề xuất thường, nhưng scope của màn đề xuất thường là leader/member được phép đề xuất và loại trừ dự án đang quản lý. Màn duyệt phải dùng scope khác: dự án do người hiện tại quản lý (`DuAn.MaNguoiDung == currentUserId`), đúng với `DyetDeXuatCongViecService.GetPageAsync` và `DuyetDeXuatNganSachService.GetPageAsync`.

### Người đề xuất

- Nên dùng: searchable dropdown hoặc autocomplete theo họ tên.
- Value nên giữ: `MaNguoiDungDeXuat`.
- Text hiển thị: `NguoiDung.HoTenNguoiDung`, có thể kèm username/email nếu cần join `Aspnetusers`.
- Nguồn dữ liệu: entity `NguoiDung`, property `MaNguoiDung`, `HoTenNguoiDung`, `IsDeleted`; dữ liệu hiện đang join trong service bằng `dx.MaNguoiDungDeXuat equals nd.MaNguoiDung`.
- Scope bắt buộc: chỉ người có đề xuất trong tập dự án người duyệt được phép xem. Không nên nạp toàn bộ nhân sự nếu không cần.
- Có cần endpoint API autocomplete không: nếu danh sách người đề xuất lớn, nên có endpoint JSON filter theo `term` và theo project scope; nếu ít, server-side select đủ.
- Có cần sửa service/ViewModel không: có, thêm `LocMaNguoiDungDeXuat` và list option hoặc endpoint.
- Có ảnh hưởng database không: không.

### Trạng thái

- Nên dùng: giữ select hiện tại.
- Value: `TrangThai.ChoDuyet`, `TrangThai.DaDuyet`, `TrangThai.TuChoi`, `TrangThai.DaHuy`.
- Text hiển thị: `TrangThai.ChoDuyetHienThi`, `DaDuyetHienThi`, `TuChoiHienThi`, `DaHuyHienThi`.
- Nguồn dữ liệu: constants `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs`.
- Có cần endpoint API autocomplete không: không.
- Có cần sửa database không: không.

### Ngày đề xuất/ngày duyệt

- Nên dùng: date range `tuNgay` -> `denNgay` và select nhỏ `locTheoNgay` nếu muốn lọc theo ngày đề xuất hoặc ngày duyệt.
- Nguồn field công việc: `DeXuatCongViec.NgayDeXuatCongViec`, `DeXuatCongViec.NgayDuyetDeXuatCongViec`.
- Nguồn field ngân sách: `DeXuatNganSach.NgayDeXuat`, `DeXuatNganSach.NgayDuyet`.
- Source hiện có để tham khảo: màn đề xuất thường đã có `tuNgay`, `denNgay`, `locTheoNgay` trong `DeXuatCongViecService.GetPageAsync` và `DeXuatNganSachService.GetPageAsync`.
- Có cần sửa database không: không.

### Từ khóa

- Duyệt công việc nên tìm trong: `TenCongViecDeXuat`, `MoTaCongViecDeXuat`, `TenDuAn`, `NguoiDungDeXuat`.
- Duyệt ngân sách nên tìm trong: `LyDoDeXuat`, `TenDuAn`, `NguoiDungDeXuat`.
- Nguồn field đã tồn tại: xem entity `DeXuatCongViec.cs`, `DeXuatNganSach.cs`, `DuAn.cs`, `NguoiDung.cs`.
- Có cần sửa database không: không.

### Khoảng tiền

- Duyệt công việc có thể lọc `ChiPhiDeXuat` nếu cần: field `DeXuatCongViec.ChiPhiDeXuat`.
- Duyệt ngân sách có thể lọc `NganSachDeXuat`: field `DeXuatNganSach.NganSachDeXuat`.
- User yêu cầu riêng màn ngân sách có khoảng số tiền đề xuất: source có field `NganSachDeXuat`, nên có thể làm.
- Có cần sửa database không: không.

### Mã đề xuất

- Công việc: `DeXuatCongViec.MaDeXuatCV`.
- Ngân sách: `DeXuatNganSach.MaDeXuatNS`.
- Nên xem xét có thật sự cần filter này không. Nếu giữ, label nên rõ "Mã đề xuất" và có thể đặt trong tìm kiếm nâng cao, không phải filter chính.
- Có cần sửa database không: không.

### Team

- Không tìm thấy filter `MaTeam` trong hai màn duyệt hiện tại.
- Nếu sau này cần, nguồn có thể từ `Team`, `TeamDuAn`, `NhanVienTeam`, nhưng cần phân tích thêm scope theo dự án; không nên thêm vội vào màn duyệt khi source hiện tại không dùng.

## 9. Phân quyền và phạm vi dữ liệu khi lọc

| Câu hỏi | Kết quả từ source |
|---|---|
| Ai được xem danh sách duyệt công việc? | Người dùng đăng nhập có `Permissions.DuyetDeXuatCongViec.Xem`, kiểm tra trong `DuyetDeXuatCongViecController.Index`. |
| Ai được duyệt/từ chối công việc? | Người dùng có `Permissions.DuyetDeXuatCongViec.Duyet`, kiểm tra trong action `Duyet` và `TuChoi`; service còn bắt buộc là quản lý dự án bằng `EnsureIsProjectManagerAsync`. |
| Ai được xem danh sách duyệt ngân sách? | Người dùng có `Permissions.DuyetNganSach.Xem`, kiểm tra trong `DuyetDeXuatNganSachController.Index`. |
| Ai được duyệt/từ chối ngân sách? | Người dùng có `Permissions.DuyetNganSach.Duyet`, kiểm tra trong action `Duyet` và `TuChoi`; service còn bắt buộc là quản lý dự án bằng `EnsureIsProjectManagerAsync`. |
| Có lọc theo dự án người dùng quản lý không? | Có. Cả hai service list đều có `da.MaNguoiDung == currentUserId`; approve/reject cũng kiểm tra `DuAn.MaNguoiDung == currentUserId`. |
| Admin có xem toàn bộ không? | Không tìm thấy nhánh bypass Admin trong `DyetDeXuatCongViecService.GetPageAsync` hoặc `DuyetDeXuatNganSachService.GetPageAsync`. Dù có permission, query hiện vẫn bó theo `DuAn.MaNguoiDung == currentUserId`. |
| Manager có chỉ xem dự án mình phụ trách không? | Có, theo `DuAn.MaNguoiDung == currentUserId`. Source không đọc role Manager trực tiếp ở hai service duyệt, mà dùng quan hệ quản lý dự án. |
| Leader có được duyệt không hay chỉ đề xuất? | Không tìm thấy logic cho Leader duyệt trong hai service duyệt. Các service đề xuất thường (`DeXuatCongViecService`, `DeXuatNganSachService`) cho Leader tạo đề xuất theo `NhanVienTeam.IsLeader == true`, nhưng màn duyệt yêu cầu là quản lý dự án. |
| Dropdown dự án sau này phải tuân thủ scope nào? | Phải dùng cùng scope list: `DuAn.IsDeleted != true && DuAn.MaNguoiDung == currentUserId`, trừ khi sau này nghiệp vụ xác nhận Admin được xem toàn bộ. |
| Dropdown người đề xuất sau này phải tuân thủ scope nào? | Chỉ lấy người đề xuất thuộc các đề xuất trong dự án người duyệt được phép quản lý/xem; không lấy toàn bộ `NGUOI_DUNG` nếu không cần. |

File/method kiểm tra quyền:

- `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`: `Permissions.DuyetDeXuatCongViec.Xem`, `.Duyet`, `Permissions.DuyetNganSach.Xem`, `.Duyet`.
- `DuyetDeXuatCongViecController.Index/Duyet/TuChoi`: `_permission.HasPermissionAsync(...)`.
- `DuyetDeXuatNganSachController.Index/Duyet/TuChoi`: `_permission.HasPermissionAsync(...)`.
- `DyetDeXuatCongViecService.GetPageAsync`: `da.MaNguoiDung == currentUserId`.
- `DyetDeXuatCongViecService.EnsureIsProjectManagerAsync`: `_context.DuAn.AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true && x.MaNguoiDung == maNguoiDung)`.
- `DuyetDeXuatNganSachService.GetPageAsync`: `da.MaNguoiDung == currentUserId`.
- `DuyetDeXuatNganSachService.EnsureIsProjectManagerAsync`: cùng pattern.

## 10. JavaScript/frontend liên quan

- File JS trực tiếp dùng bởi cả hai màn duyệt: `QuanLyDuAn/QuanLyDuAn/wwwroot/js/approval/index.js`.
- `Index.cshtml` của cả hai màn đều include `<script src="~/js/approval/index.js" asp-append-version="true"></script>`.
- JS hiện có:
  - Gắn submit handler cho `.js-confirm-submit, .js-review-submit`.
  - Hiển thị `window.confirm` theo `data-confirm`.
  - Disable nút submit sau khi submit.
  - Toggle detail row qua `.js-toggle-detail` và `data-target`.
- Không tìm thấy submit filter bằng JavaScript trong hai màn duyệt; filter submit bằng HTML form GET.
- Không tìm thấy AJAX cho filter trong hai màn duyệt.
- Không tìm thấy modal duyệt/từ chối trong hai màn duyệt; action là form POST inline.
- Không tìm thấy JS reset filter; nút "Làm mới" là link `asp-action="Index"`.
- Phân trang giữ query string bằng `Views/Shared/_Pagination.cshtml`, đọc `ViewContext.HttpContext.Request.Query`, bỏ `pageNumber`, giữ các query khác, dùng `asp-all-route-data`.
- Không tìm thấy thư viện `select2`, `tom-select`, `bootstrap-select`, `choices`, `selectize` trong view/wwwroot ngoài Bootstrap/jQuery chuẩn. Vì vậy nếu sửa nên ưu tiên `select` HTML server-side hoặc `datalist`/autocomplete nhẹ trước khi thêm thư viện ngoài.

## 11. Kết luận nhanh

- Ô filter đang bắt người dùng nhập mã bằng tay: `locMaDuAn` ở cả hai màn hình.
- Màn hình bị ảnh hưởng:
  - `Views/DuyetDeXuatCongViec/_Filter.cshtml`.
  - `Views/DuyetDeXuatNganSach/_Filter.cshtml`.
- Vì sao UX chưa hợp lý: label và input yêu cầu "Mã dự án" trong khi người dùng thường nhớ tên dự án, không nhớ ID kỹ thuật. Backend đã join `DuAn.TenDuAn`, nên source có đủ dữ liệu để hiển thị tên.
- Có lỗi backend không: không thấy lỗi filter backend nghiêm trọng; query theo ID và trạng thái hoạt động đúng theo source. Vấn đề chính là UI/UX và thiếu các filter tiện dụng.
- Có cần sửa database không: không.
- Cần sửa gì nếu được xác nhận sau:
  - View: đổi `input type="number"` mã dự án thành select/searchable/autocomplete và thêm filter mới.
  - ViewModel: thêm danh sách option và property filter mới.
  - Controller: nhận thêm tham số filter và nạp/forward dữ liệu.
  - Service: query option theo scope, thêm điều kiện filter người đề xuất/ngày/từ khóa/tiền.
  - JavaScript: không bắt buộc nếu dùng select thường; có thể cần nếu autocomplete debounce.
- Mức độ ưu tiên:
  - Cao: đổi filter `Mã dự án` sang dropdown/searchable select theo `TenDuAn`.
  - Trung bình: thêm người đề xuất, ngày gửi, từ khóa.
  - Trung bình: thêm khoảng số tiền cho duyệt ngân sách.
  - Thấp: thêm filter mã đề xuất nếu thật sự cần tra cứu kỹ thuật.

## 12. Đề xuất chỉnh sửa sau khi xác nhận

### Duyệt đề xuất công việc

1. File cần sửa:
   - `Controllers/DuyetDeXuatCongViecController.cs`.
   - `Services/Interfaces/IDuyetDeXuatCongViecService.cs`.
   - `Services/Implementations/DyetDeXuatCongViecService.cs`.
   - `ViewModels/DuyetDeXuatCongViec/DuyetDeXuatCongViecPageViewModel.cs`.
   - Có thể thêm option ViewModel mới, ví dụ `DuyetDeXuatCongViecDuAnOptionViewModel`, `DuyetDeXuatCongViecNguoiDeXuatOptionViewModel`.
   - `Views/DuyetDeXuatCongViec/_Filter.cshtml`.
   - `Views/DuyetDeXuatCongViec/_Table.cshtml` để hidden fields duyệt/từ chối giữ thêm filter mới.
2. Method cần sửa:
   - Controller `Index`, `Duyet`, `TuChoi`.
   - Service `GetPageAsync`; có thể thêm private method `GetManagedProjectOptionsAsync`, `GetRequesterOptionsAsync`.
3. ViewModel cần thêm property:
   - `int? LocMaNguoiDungDeXuat`.
   - `DateTime? TuNgay`, `DateTime? DenNgay`.
   - `string? TuKhoa`.
   - `List<...DuAnOptionViewModel> DanhSachDuAn`.
   - `List<...NguoiDeXuatOptionViewModel> DanhSachNguoiDeXuat`.
4. Controller cần nạp danh sách dropdown gì:
   - Có thể để service trả trong page VM: danh sách dự án quản lý và danh sách người đề xuất theo scope.
5. Service cần query option nào:
   - Dự án: `_context.DuAn.Where(x => x.IsDeleted != true && x.MaNguoiDung == currentUserId)`.
   - Người đề xuất: distinct từ `DeXuatCongViec` join `NguoiDung` trong các dự án của current manager.
6. View cần đổi input nào:
   - Đổi `input type="number" name="locMaDuAn"` thành `select name="locMaDuAn"` hiển thị `TenDuAn`.
   - Thêm select/autocomplete người đề xuất.
   - Thêm `input type="date"` từ ngày/đến ngày.
   - Thêm `input type="search"` từ khóa.
7. Query filter backend có giữ nguyên value ID không:
   - Có. `locMaDuAn` vẫn là `MaDuAn`; `LocMaNguoiDungDeXuat` vẫn là `MaNguoiDung`.
8. Có cần sửa database không:
   - Không.
9. Có cần sửa JavaScript không:
   - Không nếu dùng select thường.
   - Có nếu chọn autocomplete endpoint/debounce.
10. Có cần test phân quyền không:
   - Có, bắt buộc test Manager chỉ thấy dự án mình quản lý và Admin hiện không bypass scope nếu không đổi nghiệp vụ.

### Duyệt đề xuất ngân sách

1. File cần sửa:
   - `Controllers/DuyetDeXuatNganSachController.cs`.
   - `Services/Interfaces/IDuyetDeXuatNganSachService.cs`.
   - `Services/Implementations/DuyetDeXuatNganSachService.cs`.
   - `ViewModels/DuyetDeXuatNganSach/DuyetDeXuatNganSachPageViewModel.cs`.
   - Có thể thêm option ViewModel mới cho dự án/người đề xuất.
   - `Views/DuyetDeXuatNganSach/_Filter.cshtml`.
   - `Views/DuyetDeXuatNganSach/_Table.cshtml` để hidden fields giữ thêm filter mới.
2. Method cần sửa:
   - Controller `Index`, `Duyet`, `TuChoi`.
   - Service `GetPageAsync`; có thể thêm private method lấy option.
3. ViewModel cần thêm property:
   - `int? LocMaNguoiDungDeXuat`.
   - `DateTime? TuNgay`, `DateTime? DenNgay`.
   - `decimal? TuSoTien`, `decimal? DenSoTien`.
   - `string? TuKhoa`.
   - `List<...DuAnOptionViewModel> DanhSachDuAn`.
   - `List<...NguoiDeXuatOptionViewModel> DanhSachNguoiDeXuat`.
4. Controller cần nạp danh sách dropdown gì:
   - Danh sách dự án trong scope duyệt; danh sách người đề xuất trong scope.
5. Service cần query option nào:
   - Dự án: `_context.DuAn.Where(x => x.IsDeleted != true && x.MaNguoiDung == currentUserId)`.
   - Người đề xuất: distinct từ `DeXuatNganSach` join `NguoiDung`.
6. View cần đổi input nào:
   - Đổi `input type="number" name="locMaDuAn"` thành select/searchable select.
   - Thêm người đề xuất, ngày gửi, khoảng tiền `NganSachDeXuat`, từ khóa.
7. Query filter backend có giữ nguyên value ID không:
   - Có.
8. Có cần sửa database không:
   - Không.
9. Có cần sửa JavaScript không:
   - Không nếu dùng select thường; có nếu autocomplete.
10. Có cần test phân quyền không:
   - Có.

## 13. Test case kiểm thử

| Mã test | Màn hình | Tình huống | Dữ liệu filter | Kết quả mong đợi | Cách kiểm tra | Ghi chú |
|---|---|---|---|---|---|---|
| LDL-01 | Duyệt đề xuất công việc | Lọc theo dự án bằng tên/dropdown | Chọn dự án A, value `MaDuAn` | Danh sách chỉ có đề xuất công việc của dự án A trong scope quản lý | Mở `/DuyetDeXuatCongViec?locMaDuAn=<id>` và đối chiếu `TenDuAn` | Sau khi sửa UI. |
| LDL-02 | Duyệt đề xuất công việc | Lọc theo người đề xuất | Chọn `MaNguoiDungDeXuat` | Chỉ hiện đề xuất do người đó gửi trong scope | Đối chiếu cột/detail `NguoiDungDeXuat` | Filter chưa có hiện tại. |
| LDL-03 | Duyệt đề xuất công việc | Lọc trạng thái Chờ duyệt | `locTrangThai=ChoDuyet` | Chỉ hiện đề xuất có `TrangThaiCongViecDeXuat` chờ duyệt | Kiểm tra badge trạng thái và query string | Hiện tại đã có. |
| LDL-04 | Duyệt đề xuất công việc | Lọc theo khoảng ngày | `tuNgay`, `denNgay` theo ngày đề xuất | Chỉ hiện đề xuất có `NgayDeXuatCongViec` trong khoảng | So với ngày hiển thị trong bảng/detail | Filter chưa có hiện tại. |
| LDL-05 | Duyệt đề xuất công việc | Xóa lọc | Bấm "Làm mới" | URL không còn query filter, danh sách quay về mặc định theo scope | Kiểm tra URL và danh sách | Hiện tại reset có link `asp-action="Index"`. |
| LDL-06 | Duyệt ngân sách | Lọc theo dự án bằng tên/dropdown | Chọn dự án A, value `MaDuAn` | Danh sách chỉ có đề xuất ngân sách của dự án A trong scope quản lý | Mở `/DuyetDeXuatNganSach?locMaDuAn=<id>` và đối chiếu `TenDuAn` | Sau khi sửa UI. |
| LDL-07 | Duyệt ngân sách | Lọc theo người đề xuất | Chọn `MaNguoiDungDeXuat` | Chỉ hiện đề xuất do người đó gửi trong scope | Đối chiếu detail `NguoiDungDeXuat` | Filter chưa có hiện tại. |
| LDL-08 | Duyệt ngân sách | Lọc trạng thái Chờ duyệt | `locTrangThai=ChoDuyet` | Chỉ hiện đề xuất có `TrangThaiDeXuat` chờ duyệt | Kiểm tra badge trạng thái và query string | Hiện tại đã có. |
| LDL-09 | Duyệt ngân sách | Lọc theo khoảng ngày | `tuNgay`, `denNgay` theo ngày đề xuất | Chỉ hiện đề xuất có `NgayDeXuat` trong khoảng | So với ngày hiển thị trong bảng/detail | Filter chưa có hiện tại. |
| LDL-10 | Duyệt ngân sách | Xóa lọc | Bấm "Làm mới" | URL không còn query filter, danh sách quay về mặc định theo scope | Kiểm tra URL và danh sách | Hiện tại reset có link `asp-action="Index"`. |
| LDL-11 | Phân quyền | Manager chỉ thấy dữ liệu trong phạm vi dự án được quản lý | Tài khoản quản lý dự án A, không quản lý B | Chỉ thấy đề xuất của A; dropdown dự án không có B | Đăng nhập manager, kiểm tra list/dropdown | Theo source `DuAn.MaNguoiDung == currentUserId`. |
| LDL-12 | Phân quyền | Admin thấy dữ liệu đúng phạm vi quyền | Tài khoản Admin có permission duyệt | Theo source hiện tại, vẫn chỉ thấy dự án có `DuAn.MaNguoiDung == currentUserId` nếu không đổi service | Đăng nhập Admin, kiểm tra list/dropdown | Cần xác nhận nghiệp vụ nếu muốn Admin xem toàn bộ. |
| LDL-13 | Phân trang | Filter không bị mất khi chuyển trang | `locMaDuAn`, `locTrangThai`, filter mới nếu có | Query string vẫn giữ khi bấm trang sau/trước | Kiểm tra URL do `_Pagination.cshtml` tạo | Shared pagination hiện giữ query. |
| LDL-14 | Duyệt/từ chối | Filter vẫn đúng sau khi duyệt hoặc từ chối xong redirect về danh sách | Đang ở trang/filter bất kỳ | Sau POST, quay lại danh sách với filter đúng; page cũng giữ nếu action nhận page | Test approve/reject form | Hiện tại `TuChoi` giữ page; `Duyet` chưa giữ page. |
| LDL-15 | UX | Không còn bắt người dùng nhập mã dự án/mã người dùng bằng tay | Dùng dropdown/searchable theo tên | Người dùng chọn tên dự án/người đề xuất; value ID gửi ngầm | Inspect HTML form và thao tác lọc | Mục tiêu sau khi sửa. |

## 14. Checklist thông tin cần gửi lại cho ChatGPT

1. File controller duyệt đề xuất công việc: `QuanLyDuAn/QuanLyDuAn/Controllers/DuyetDeXuatCongViecController.cs`.
2. Action danh sách duyệt đề xuất công việc: `DuyetDeXuatCongViecController.Index`.
3. Action duyệt/từ chối đề xuất công việc: `DuyetDeXuatCongViecController.Duyet`, `DuyetDeXuatCongViecController.TuChoi`.
4. File view duyệt đề xuất công việc: `Views/DuyetDeXuatCongViec/Index.cshtml`, `_Filter.cshtml`, `_Table.cshtml`.
5. ViewModel/filter model duyệt đề xuất công việc: `DuyetDeXuatCongViecPageViewModel`, `DuyetDeXuatCongViecItemViewModel`.
6. Các input đang nhập mã bằng tay ở duyệt đề xuất công việc: `locMaDuAn` (`input type="number"`, label "Mã dự án").
7. File controller duyệt đề xuất ngân sách: `QuanLyDuAn/QuanLyDuAn/Controllers/DuyetDeXuatNganSachController.cs`.
8. Action danh sách duyệt đề xuất ngân sách: `DuyetDeXuatNganSachController.Index`.
9. Action duyệt/từ chối đề xuất ngân sách: `DuyetDeXuatNganSachController.Duyet`, `DuyetDeXuatNganSachController.TuChoi`.
10. File view duyệt đề xuất ngân sách: `Views/DuyetDeXuatNganSach/Index.cshtml`, `_Filter.cshtml`, `_Table.cshtml`.
11. ViewModel/filter model duyệt đề xuất ngân sách: `DuyetDeXuatNganSachPageViewModel`, `DuyetDeXuatNganSachItemViewModel`.
12. Các input đang nhập mã bằng tay ở duyệt đề xuất ngân sách: `locMaDuAn` (`input type="number"`, label "Mã dự án").
13. Service/query lọc dữ liệu hiện tại: `DyetDeXuatCongViecService.GetPageAsync`, `DuyetDeXuatNganSachService.GetPageAsync`; filter `locMaDuAn`, `locTrangThai`; scope `DuAn.MaNguoiDung == currentUserId`.
14. Permission/data scope hiện tại: `Permissions.DuyetDeXuatCongViec.Xem/Duyet`, `Permissions.DuyetNganSach.Xem/Duyet`; service bắt buộc dự án do người hiện tại quản lý.
15. Nguồn dropdown đề xuất cho dự án/người đề xuất/trạng thái: `DuAn.MaDuAn/TenDuAn`, `NguoiDung.MaNguoiDung/HoTenNguoiDung`, constants `TrangThai`.
16. Có cần sửa CSDL không: không.
17. Có cần thêm JavaScript không: không nếu dùng select thường; có thể cần nếu chọn autocomplete debounce.
18. Kết luận: nên sửa cả UI và backend lọc/list option; không cần sửa database. Backend filter ID hiện có thể giữ, nhưng cần mở rộng ViewModel/Controller/Service để nạp option và thêm filter theo người đề xuất/ngày/từ khóa/số tiền.
