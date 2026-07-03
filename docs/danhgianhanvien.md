# Phân tích chức năng đánh giá nhân viên theo dự án

## 1. Tổng quan vấn đề

Tài liệu này chỉ đọc và phân tích source hiện tại, chưa sửa code, chưa sửa database, chưa tạo migration, chưa seed dữ liệu, chưa đổi giao diện, chưa đổi CSS/font.

Kết luận nhanh sau khi đọc lại source hiện tại: chức năng `Đánh giá nhân viên` vẫn được thiết kế theo phạm vi dự án vì entity `DanhGiaNhanVien` có `MaDuAn`, màn hình nhận `maDuAn`, form lưu hidden `MaDuAn`, service validate nhân viên phải thuộc `NhanVienDuAn` của dự án. Sau lần chỉnh sửa hiện tại, khi chưa chọn dự án (`maDuAn == null`) service không load dropdown nhân viên và không load danh sách nhân viên. Khi có `maDuAn`, dropdown và bảng danh sách đều lấy từ `NhanVienDuAn` join `NguoiDung`, có lọc `nvda.MaDuAn == maDuAn.Value`. Nếu một dự án như DATA10-009 vẫn hiện 6 nhân viên, nguyên nhân theo source hiện tại không phải do load toàn bộ hệ thống, mà là vì 6 nhân viên đó thuộc nguồn `NhanVienDuAn` của chính dự án đó, hoặc dữ liệu `NhanVienDuAn` của dự án đang được nhập quá rộng so với khái niệm "người thực sự thực hiện công việc".

Điểm cần chú ý nghiệp vụ: source hiện chỉ xem `NhanVienDuAn` là nguồn chính để đưa nhân viên vào danh sách đánh giá. Các nguồn tham gia thực tế như `PhanCongCtCongViec`, `PhanCongCongViec`, `TienDoCongViec`, `TeamDuAn`/`NhanVienTeam` đang được dùng cho thống kê/quyền leader, nhưng chưa được dùng như nguồn độc lập để sinh dropdown người được đánh giá.

## 2. Danh sách file liên quan

| Nhóm | File | Class/method/property liên quan | Nhận xét |
|---|---|---|---|
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/DanhGiaNhanVienController.cs` | `DanhGiaNhanVienController`, `Index`, `Form`, `Luu`, `GuiDuyet`, `Duyet`, `TuChoi`, `XuatFile`, `ChiTiet` | Controller chính của chức năng. |
| Service interface | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDanhGiaNhanVienService.cs` | `IDanhGiaNhanVienService`, `GetPageAsync`, `GetFormAsync`, `LuuDanhGiaAsync`, `GuiDuyetAsync`, `DuyetAsync`, `TuChoiAsync`, `GetChiTietAsync` | Interface service. |
| Service implementation | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaNhanVienService.cs` | `DanhGiaNhanVienService`, `GetPageAsync`, `LayDanhSachNhanVienTheoScopeAsync`, `XayDungThongKeNhanVienAsync`, `GetFormAsync`, `LuuDanhGiaAsync`, `CoQuyenTaoDanhGiaNhanVien`, `CoThuocScopeLeaderAsync` | Nơi quyết định danh sách nhân viên, scope quyền, validate lưu. |
| DI | `QuanLyDuAn/QuanLyDuAn/Services/CauHinhDichVu.cs` | `AddScoped<IDanhGiaNhanVienService, DanhGiaNhanVienService>()` | Đăng ký service. |
| Entity đánh giá | `QuanLyDuAn/QuanLyDuAn/Models/Entities/DanhGiaNhanVien.cs` | `MaDanhGiaNhanVien`, `MaNguoiDung`, `MaDuAn`, `MaNguoiDungDanhGia`, `DiemTongDanhGiaNV`, `NgayDanhGiaNV`, `XepLoai`, `NhanXetTongQuanNV`, `TrangThaiDanhGiaNV`, `MaNguoiDungDuyet`, `IsDeleted` | Bảng header đánh giá nhân viên. |
| Entity chi tiết đánh giá | `QuanLyDuAn/QuanLyDuAn/Models/Entities/CtDanhGiaNhanVien.cs` | `MaChiTietDGNV`, `MaDanhGiaNhanVien`, `MaTieuChi`, `MaCongViec`, `NoiDungDanhGiaNhanVien`, `DiemDanhGiaNV`, `IsDeleted` | Chi tiết tiêu chí, có optional `MaCongViec`. Không có `MaChiTietCV`. |
| DbContext mapping | `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs` | `DbSet<DanhGiaNhanVien>`, `DbSet<CtDanhGiaNhanVien>`, mapping `DANH_GIA_NHAN_VIEN`, `CT_DANH_GIA_NHAN_VIEN` | Mapping FK tới `NguoiDung`, `DuAn`, `TieuChiDanhGia`, `CongViec`. |
| ViewModel page | `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaNhanVien/DanhGiaNhanVienPageViewModel.cs` | `MaDuAn`, `MaNhanVien`, `DanhSach`, `DanhSachDuAn`, `DanhSachNhanVien`, `Form`, `Permissions` | Model của trang index/filter/list. |
| ViewModel form | `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaNhanVien/DanhGiaNhanVienFormViewModel.cs` | `MaDanhGiaNhanVien`, `MaDuAn`, `MaNhanVien`, `TieuChi`, `NhanXetTongQuan`, `CoTheLuu`, `ThongKe` | Model submit form. |
| ViewModel item | `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaNhanVien/DanhGiaNhanVienItemViewModel.cs` | `MaDuAn`, `MaNhanVien`, `CoDanhGia`, `CoTheDanhGia`, `CoTheSua`, `CoTheDuyet` | Row list. |
| ViewModel chi tiết | `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaNhanVien/DanhGiaNhanVienChiTietViewModel.cs` | `MaDanhGiaNhanVien`, `MaDuAn`, `MaNhanVien`, `TieuChi`, `ThongKe` | Model màn chi tiết. |
| ViewModel tiêu chí | `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaNhanVien/DanhGiaNhanVienTieuChiViewModel.cs` | `MaTieuChi`, `MaCongViec`, `DiemDanhGiaNV`, `NoiDungDanhGiaNhanVien` | Validate điểm 1-10 và nhận xét 500 ký tự. |
| View index | `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaNhanVien/Index.cshtml` | `Model.Form`, `_Filter`, `_SummaryCards`, `_Form`, `_Table`, `_EmptyState` | Trang chính. |
| Filter view | `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaNhanVien/_Filter.cshtml` | `<select name="maDuAn">`, `<select name="maNhanVien">`, `Model.DanhSachDuAn`, `Model.DanhSachNhanVien` | Dropdown dự án/nhân viên render server-side. |
| Form view | `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaNhanVien/_Form.cshtml` | hidden `MaDanhGiaNhanVien`, `MaDuAn`, `MaNhanVien`, input `DiemDanhGiaNV`, `MaCongViec`, `NoiDungDanhGiaNhanVien`, `NhanXetTongQuan` | Form lưu đánh giá. |
| Table view | `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaNhanVien/_Table.cshtml` | link `Form`, `ChiTiet`, form `GuiDuyet`, `Duyet`, `TuChoi` | Thao tác trên từng nhân viên/dự án. |
| Empty state | `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaNhanVien/_EmptyState.cshtml` | `thongBao`, `Model.ThongBaoRong` | Sau chỉnh sửa hiện tại đã dùng thông báo chọn dự án hoặc dự án chưa có nhân viên. |
| Detail view | `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaNhanVien/ChiTiet.cshtml` | hiển thị `_EmployeeStats`, bảng tiêu chí | Màn chi tiết. |
| Nguồn điều hướng | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml` | link `asp-controller="DanhGiaNhanVien"`, `asp-route-maDuAn="@Model.MaDuAn"` | Vào đánh giá nhân viên từ chi tiết dự án. |
| Nguồn điều hướng | `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_Table.cshtml` | link `asp-controller="DanhGiaNhanVien"`, `asp-route-maDuAn="@item.MaDuAn"`, `asp-route-nguon="du-an"` | Vào đánh giá nhân viên từ đánh giá dự án. |
| Menu | `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml` | `canDanhGiaNhanVien`, link `DanhGiaNhanVien/Index` | Menu độc lập không truyền `maDuAn`. |
| Permission constants | `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs` | `Permissions.DanhGiaNhanVien.Xem`, `DanhGia`, `Sua`, `Duyet` | Tên quyền. |
| Permission definitions | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PermissionDependencyProvider.cs` | definitions cho `DanhGiaNhanVien` | Có dấu hiệu mojibake trong file này ở text hiển thị, không sửa trong nhiệm vụ này. |
| Seed tiêu chí/quyền | `QuanLyDuAn/QuanLyDuAn/Data/KhoiTaoTaiKhoanMacDinh.cs` | seed `TieuChiDanhGia` loại `DanhGiaNhanVien`, seed permission | Có dùng để tạo tiêu chí đánh giá. |
| JavaScript | Đã kiểm tra `QuanLyDuAn/QuanLyDuAn/wwwroot/js` và `Views/DanhGiaNhanVien` | Không tìm thấy JS/AJAX riêng cho `DanhGiaNhanVien` | Dropdown render server-side. |

Các từ khóa đã kiểm tra bằng `rg`: `DanhGiaNhanVien`, `DanhGia`, `Đánh giá nhân viên`, `Đánh giá`, `EmployeeReview`, `ReviewEmployee`, `NhanVienDanhGia`, `DanhGiaDuAn`, `MaDanhGia`, `DiemDanhGia`, `NhanXet`, `MaDuAn`, `MaNguoiDung`, `MaNhanVien`, `NguoiDanhGia`, `NguoiDuocDanhGia`, `GetNhanVien`, `GetEmployees`, `SelectList`, `Dropdown`, `DanhSachNhanVien`, `LoadNhanVien`, `ProjectEmployees`, `ThanhVienDuAn`, `NhanVienDuAn`, `PhanCongCongViec`, `PhanCongCtCongViec`, `TeamDuAn`, `NhanVienTeam`.

Không tìm thấy class/method tên `EmployeeReview`, `ReviewEmployee`, `NhanVienDanhGia`, `GetEmployees`, `ProjectEmployees` trong luồng đánh giá nhân viên. Không tìm thấy endpoint AJAX load nhân viên theo dự án.

## 3. Mục đích nghiệp vụ hiện tại của chức năng đánh giá

Source hiện tại nghiêng rõ về đánh giá nhân viên theo dự án:

- Có field `MaDuAn`: `DanhGiaNhanVien.MaDuAn` trong `QuanLyDuAn/QuanLyDuAn/Models/Entities/DanhGiaNhanVien.cs`, mapping FK `FK_DANH_GIA_TRONG_DU_AN` trong `QuanLyDuAnDbContext.cs`.
- Có field người được đánh giá: `DanhGiaNhanVien.MaNguoiDung`; trong service thường alias thành `MaNhanVien`, ví dụ `GetPageAsync` select `MaNhanVien = nvda.MaNguoiDung`.
- Có field người đánh giá: `DanhGiaNhanVien.MaNguoiDungDanhGia`; mapping FK `FK_DGNV_DANH_GIA_NGUOI_DU`.
- Có ngày đánh giá/duyệt: `NgayDanhGiaNV`, `NgayDuyetDanhGiaNV`.
- Có điểm tổng và xếp loại: `DiemTongDanhGiaNV`, `XepLoai`.
- Có nhận xét tổng quan: `NhanXetTongQuanNV`.
- Có trạng thái workflow: `TrangThaiDanhGiaNV`, `LyDoTuChoiDanhGiaNV`, `MaNguoiDungDuyet`.
- Có tiêu chí chi tiết: `CtDanhGiaNhanVien.MaTieuChi`, `DiemDanhGiaNV`, `NoiDungDanhGiaNhanVien`.
- Có optional `MaCongViec` ở chi tiết đánh giá, nhưng không có `MaChiTietCV` trong entity đánh giá.

Không thấy thiết kế đánh giá theo kỳ/tháng/quý. Không có field kỳ đánh giá riêng. Ngày đánh giá chỉ dùng để lọc thời gian và ghi thời điểm thao tác.

Không thấy entity đánh giá theo team riêng. `TeamDuAn`/`NhanVienTeam` chỉ đang dùng cho scope leader.

Không thấy thiết kế đánh giá theo chi tiết công việc vì `CtDanhGiaNhanVien` không có `MaChiTietCV`. Có `MaCongViec` optional ở chi tiết tiêu chí, nhưng form chỉ cho nhập "Mã công việc (tùy chọn)" dạng số, không chọn task/chi tiết từ dropdown.

Kết luận: nghiệp vụ hiện tại là đánh giá tổng thể nhân viên trong phạm vi dự án, nhưng nguồn danh sách người được đánh giá hiện dựa trên bảng `NhanVienDuAn`, không dựa trực tiếp trên phân công/tiến độ thực tế.

## 4. Luồng hiện tại của chức năng đánh giá nhân viên

Luồng vào màn hình:

1. Người dùng vào `GET /DanhGiaNhanVien/Index`.
2. Controller `DanhGiaNhanVienController.Index` nhận `maDuAn`, `maNhanVien`, `tuKhoa`, `trangThai`, `tuNgayDanhGia`, `denNgayDanhGia`.
3. Controller kiểm tra quyền `Permissions.DanhGiaNhanVien.Xem`.
4. Controller gọi `IDanhGiaNhanVienService.GetPageAsync(...)`.
5. Service `DanhGiaNhanVienService.GetPageAsync` dựng danh sách dự án theo scope bằng `LayDanhSachDuAnTheoScopeAsync`.
6. Nếu `maDuAn` null, service trả danh sách nhân viên và dropdown nhân viên rỗng, đặt `ThongBaoRong = "Vui lòng chọn dự án để đánh giá nhân viên."`.
7. Nếu `maDuAn` có giá trị, service dựng query chính từ `NhanVienDuAn` join `DuAn` join `NguoiDung`, lọc `DuAn.IsDeleted != true`, `NguoiDung.IsDeleted != true`, lọc theo scope và bắt buộc `nvda.MaDuAn == maDuAn.Value`.
8. Service nạp dropdown nhân viên bằng `LayDanhSachNhanVienTheoScopeAsync(currentUserId, roleFlags, maDuAn, leaderProjectIds, leaderTeamIds)`; method này trả rỗng nếu `maDuAn` null và chỉ query `NhanVienDuAn` của dự án nếu có `maDuAn`.
9. View `Index.cshtml` render `_Filter.cshtml`, `_SummaryCards.cshtml`, `_Table.cshtml` hoặc `_EmptyState.cshtml`.

Luồng tạo/sửa đánh giá:

1. Từ `_Table.cshtml`, nút `Đánh giá` hoặc `Sửa đánh giá` gọi `GET /DanhGiaNhanVien/Form?maDuAn=...&maNhanVien=...`.
2. Controller `Form` kiểm tra quyền `Permissions.DanhGiaNhanVien.DanhGia` hoặc `Permissions.DanhGiaNhanVien.Sua`.
3. Controller gọi `GetPageAsync` để giữ list/filter, rồi gọi `GetFormAsync(maDuAn, maNhanVien)`.
4. Service `GetFormAsync` kiểm tra không tự đánh giá, tìm `DuAn`, kiểm tra nhân viên tồn tại trong `NhanVienDuAn` của dự án, kiểm tra quyền manager/leader, nạp tiêu chí, nạp đánh giá mới nhất của chính người đánh giá hiện tại.
5. View `_Form.cshtml` submit `POST /DanhGiaNhanVien/Luu`, gồm hidden `MaDanhGiaNhanVien`, `MaDuAn`, `MaNhanVien`.
6. Controller `Luu` kiểm tra quyền và gọi `LuuDanhGiaAsync(form)`.
7. Service `LuuDanhGiaAsync` validate `MaDuAn`, `MaNhanVien`, danh sách `TieuChi`, quyền, nhân viên thuộc `NhanVienDuAn`, điểm 1-10, độ dài nhận xét, rồi lưu `DanhGiaNhanVien` và `CtDanhGiaNhanVien`.

Luồng duyệt/từ chối:

- `POST /DanhGiaNhanVien/GuiDuyet`: chỉ người tạo đánh giá được gửi duyệt; manager dự án không cần gửi vì khi lưu được tự set `DaDuyet`.
- `POST /DanhGiaNhanVien/Duyet`: chỉ manager của dự án (`duAn.MaNguoiDung == currentUserId`) được duyệt.
- `POST /DanhGiaNhanVien/TuChoi`: chỉ manager của dự án được từ chối và phải nhập lý do.

Luồng chi tiết:

- `GET /DanhGiaNhanVien/ChiTiet?maDanhGiaNhanVien=...` gọi `GetChiTietAsync`.
- Service join `DanhGiaNhanVien`, `DuAn`, `NguoiDung`, `NhanVienDuAn`, kiểm tra quyền xem theo admin/manager/chính nhân viên/người đánh giá/leader scope.

URL/route đang dùng:

| Action | HTTP | URL mặc định | Nơi gọi |
|---|---|---|---|
| `Index` | GET | `/DanhGiaNhanVien` hoặc `/DanhGiaNhanVien/Index?maDuAn=...&maNhanVien=...` | Menu, `DuAn/Details`, `DanhGiaDuAn/_Table`, filter. |
| `Form` | GET | `/DanhGiaNhanVien/Form?maDuAn=...&maNhanVien=...` | `_Table.cshtml`. |
| `Luu` | POST | `/DanhGiaNhanVien/Luu` | `_Form.cshtml`. |
| `GuiDuyet` | POST | `/DanhGiaNhanVien/GuiDuyet` | `_Form.cshtml`, `_Table.cshtml`. |
| `Duyet` | POST | `/DanhGiaNhanVien/Duyet` | `_Table.cshtml`. |
| `TuChoi` | POST | `/DanhGiaNhanVien/TuChoi` | `_Table.cshtml`. |
| `ChiTiet` | GET | `/DanhGiaNhanVien/ChiTiet?maDanhGiaNhanVien=...` | `_Table.cshtml`. |
| `XuatFile` | GET | `/DanhGiaNhanVien/XuatFile?format=...&maDuAn=...` | `Index.cshtml` export dropdown. |

## 5. Nguồn dropdown nhân viên hiện tại

Dropdown nhân viên nằm trong `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaNhanVien/_Filter.cshtml`:

- `<select name="maNhanVien" class="form-select">`.
- Dữ liệu là `Model.DanhSachNhanVien`.
- `DanhSachNhanVien` được khai báo trong `DanhGiaNhanVienPageViewModel`.
- `DanhSachNhanVien` được gán ở `DanhGiaNhanVienService.GetPageAsync` bằng `LayDanhSachNhanVienTheoScopeAsync(...)`.

Method load dropdown chính xác:

`QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaNhanVienService.cs`, `LayDanhSachNhanVienTheoScopeAsync`:

- Query từ `from nvda in _context.NhanVienDuAn join nd in _context.NguoiDung on nvda.MaNguoiDung equals nd.MaNguoiDung`.
- Điều kiện ban đầu: `nd.IsDeleted != true`.
- Nếu `maDuAn.HasValue`: `query = query.Where(x => x.MaDuAn == maDuAn.Value)`.
- Nếu role manager: lọc dự án có `DuAn.MaNguoiDung == currentUserId` và `DuAn.IsDeleted != true`.
- Nếu không phải manager/admin: lọc theo `leaderProjectIds`, `TeamDuAn`, `NhanVienTeam`.
- Sau đó `GroupBy(x => new { x.MaNguoiDung, x.HoTenNguoiDung })`, nên một nhân viên xuất hiện một lần dù thuộc nhiều dự án.

Kết luận sau chỉnh sửa hiện tại: dropdown nhân viên không lấy `_context.NguoiDung.ToListAsync()` toàn hệ thống. Khi `maDuAn` null, `LayDanhSachNhanVienTheoScopeAsync` trả danh sách rỗng nên không còn gom nhân viên từ nhiều dự án trong scope. Khi `maDuAn` có giá trị, dropdown lấy từ `NhanVienDuAn` join `NguoiDung` và lọc `nvda.MaDuAn == maDuAn.Value`.

## 6. Nguyên nhân load toàn bộ nhân viên hệ thống

Không tìm thấy đoạn code dạng `_context.NguoiDung.ToListAsync()` hoặc `new SelectList(_context.NguoiDung...)` trong luồng `DanhGiaNhanVien`.

Đoạn code hiện tại quyết định nguồn nhân viên:

| File | Method | Đoạn liên quan | Bảng/entity | Điều kiện hiện tại | Thiếu gì |
|---|---|---|---|---|---|
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaNhanVienService.cs` | `LayDanhSachNhanVienTheoScopeAsync` | Nếu `maDuAn` null thì trả rỗng; nếu có `maDuAn` thì query `_context.NhanVienDuAn` join `_context.NguoiDung` và lọc `nvda.MaDuAn == maDuAn.Value` | `NhanVienDuAn`, `NguoiDung`, `DuAn`, `TeamDuAn`, `NhanVienTeam` | `NguoiDung.IsDeleted != true`; loại current user; manager lọc dự án do mình quản lý; leader lọc team/project scope | Không còn gom tất cả nhân viên khi chưa chọn dự án; chưa lọc tài khoản bị khóa/chưa kích hoạt; chưa dựa trên phân công/tiến độ thực tế. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaNhanVienService.cs` | `GetPageAsync` | Nếu `maDuAn` null thì trả rỗng; nếu có `maDuAn` thì query chính từ `_context.NhanVienDuAn` join `DuAn` join `NguoiDung` và lọc `nvda.MaDuAn == maDuAn.Value` | `NhanVienDuAn` | `duAnTheoScope.Contains(nvda.MaDuAn)`, role manager/leader, `x.MaNhanVien != currentUserId` | Bảng chính hiện chỉ hiện nhân viên thuộc dự án đã chọn theo `NhanVienDuAn`. |
| `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml` | menu `DanhGiaNhanVien/Index` | Link không truyền `maDuAn` | Route UI | Vào màn hình độc lập | Khi vào từ menu, service hiện empty state "Vui lòng chọn dự án để đánh giá nhân viên." |

Kết luận nguyên nhân:

- Nếu người dùng vào từ `DuAn/Details` hoặc `DanhGiaDuAn/_Table`, route có `maDuAn`, service sẽ lọc dropdown và bảng chính theo `maDuAn`.
- Nếu người dùng vào từ menu độc lập, route không có `maDuAn`, service không load nhân viên và yêu cầu chọn dự án.
- Nếu một dự án cụ thể vẫn hiện nhiều nhân viên, khả năng cao là bảng `NhanVienDuAn` của chính dự án đó đang chứa nhiều người, không phải service đang lấy tất cả dự án.
- Đây không phải lỗi do JS hay `SelectList`; dropdown là server-side render.

## 7. Entity và bảng dữ liệu đánh giá

| Field yêu cầu | Field thực tế | Entity/file | Ghi chú |
|---|---|---|---|
| `MaDanhGia` | `MaDanhGiaNhanVien` | `DanhGiaNhanVien.cs` | Khóa chính. |
| `MaDuAn` | `MaDuAn` | `DanhGiaNhanVien.cs` | FK tới `DuAn`, chứng minh đánh giá theo dự án. |
| `MaNguoiDanhGia` | `MaNguoiDungDanhGia` | `DanhGiaNhanVien.cs` | Người tạo/người đánh giá. |
| `MaNguoiDuocDanhGia` | `MaNguoiDung` | `DanhGiaNhanVien.cs` | Người được đánh giá, service alias là `MaNhanVien`. |
| `Diem` | `DiemTongDanhGiaNV`; chi tiết `DiemDanhGiaNV` | `DanhGiaNhanVien.cs`, `CtDanhGiaNhanVien.cs` | Tổng điểm và điểm theo tiêu chí. |
| `NhanXet` | `NhanXetTongQuanNV`; chi tiết `NoiDungDanhGiaNhanVien` | `DanhGiaNhanVien.cs`, `CtDanhGiaNhanVien.cs` | Nhận xét tổng và nhận xét tiêu chí. |
| `NgayDanhGia` | `NgayDanhGiaNV` | `DanhGiaNhanVien.cs` | Set `DateTime.Now` khi lưu/gửi duyệt. |
| `TieuChi` | `MaTieuChi` | `CtDanhGiaNhanVien.cs` | Join `TieuChiDanhGia`. |
| `MaCongViec` | `MaCongViec` | `CtDanhGiaNhanVien.cs` | Optional ở chi tiết tiêu chí. |
| `MaChiTietCV` | Không tìm thấy trong entity đánh giá | Đã kiểm tra `DanhGiaNhanVien.cs`, `CtDanhGiaNhanVien.cs` | Không có đánh giá theo chi tiết công việc. |
| `MaTeam` | Không tìm thấy trong entity đánh giá | Đã kiểm tra entity đánh giá | Team chỉ dùng scope. |
| `IsDeleted` | `IsDeleted`, `DeletedAt`, `DeletedBy` | `DanhGiaNhanVien.cs`, `CtDanhGiaNhanVien.cs` | Có xóa mềm ở entity, nhưng không tìm thấy action xóa đánh giá trong controller. |
| `CreatedAt/UpdatedAt` | Không tìm thấy | Đã kiểm tra entity đánh giá | Không có audit tạo/cập nhật riêng. |

Ràng buộc trùng:

1. Một nhân viên có thể có nhiều bản ghi trong cùng dự án về mặt database vì `QuanLyDuAnDbContext.cs` chỉ khai báo `HasKey(e => e.MaDanhGiaNhanVien)`, không thấy unique composite trên `MaDuAn + MaNguoiDung` hoặc `MaDuAn + MaNguoiDung + MaNguoiDungDanhGia`.
2. Source không chặn tạo trùng tuyệt đối theo `MaDuAn + MaNguoiDung`. `GetFormAsync` tìm đánh giá mới nhất theo `MaDuAn + MaNguoiDung + MaNguoiDungDanhGia + IsDeleted != true`; nếu cùng người đánh giá vào lại form thì sửa bản mới nhất của mình.
3. Nếu nhiều người đánh giá cùng một nhân viên trong cùng dự án, source có thể có nhiều bản ghi vì điều kiện lấy form gắn thêm `MaNguoiDungDanhGia == currentUserId`.
4. Không tìm thấy action xóa đánh giá trong `DanhGiaNhanVienController`; chỉ thấy soft delete chi tiết cũ khi lưu lại: `CtDanhGiaNhanVien.IsDeleted = true`.
5. Không thấy lịch sử đánh giá chuyên biệt; các bản ghi cũ có thể tồn tại nếu được tạo bởi người đánh giá khác hoặc tạo mới trong tình huống không có `MaDanhGiaNhanVien`, nhưng UI đang lấy bản mới nhất theo cặp.
6. Điểm tổng tính bằng trung bình tiêu chí trong `TinhDiemTongKet`, sau đó round và lưu `DiemTongDanhGiaNV`.
7. Đánh giá nhân viên được dùng trong export `XuatFile`; chưa thấy luồng AI/dataset trực tiếp đọc `DanhGiaNhanVien` trong phần đã kiểm tra. `DashboardService` có thể dùng thống kê chung nhưng không thấy bằng chứng trực tiếp trong luồng này, cần audit riêng nếu muốn kết luận dashboard/AI toàn hệ thống.

Không nên tự thêm unique constraint hoặc migration trong lần này. Nếu cần chống trùng theo nghiệp vụ, hướng tối thiểu là chặn ở service trước khi cân nhắc DB constraint.

## 8. Nguồn nhân viên thực sự tham gia dự án

| Nguồn | Entity/bảng | Cách join tới dự án | Loại nhân viên lấy được | Có nên dùng cho dropdown đánh giá không | Ưu/nhược điểm |
|---|---|---|---|---|---|
| Thành viên dự án | `NhanVienDuAn` / `NHAN_VIEN_DU_AN` | `NhanVienDuAn.MaDuAn -> DuAn.MaDuAn`, `NhanVienDuAn.MaNguoiDung -> NguoiDung.MaNguoiDung` | Nhân viên được ghi nhận chính thức trong dự án, có `VaiTroTrongDuAn`, `NgayThamGiaDuAn` | Có. Đây là nguồn hiện tại và là nguồn nền tảng. | Ưu: rõ quan hệ dự án; đã dùng trong service. Nhược: nếu dữ liệu thêm rộng, có thể bao gồm người không tham gia thực tế công việc. |
| Team dự án | `TeamDuAn`, `Team`, `NhanVienTeam` | `TeamDuAn.MaDuAn -> DuAn`; `TeamDuAn.MaTeam -> NhanVienTeam.MaTeam`; `NhanVienTeam.MaNguoiDung -> NguoiDung` | Thành viên thuộc team được gắn vào dự án | Có thể dùng bổ sung nếu team là nguồn quản trị chính. | Ưu: phù hợp nghiệp vụ team/leader. Nhược: nếu team lớn nhưng chỉ một phần làm dự án, dễ đưa thừa; cần lọc `Team.IsDeleted`, trạng thái team nếu dùng. |
| Phân công công việc cha | `PhanCongCongViec`, `CongViec`, `DanhMucCongViec` | `PhanCongCongViec.MaCongViec -> CongViec.MaCongViec -> DanhMucCongViec.MaDanhMucCV -> DanhMucCongViec.MaDuAn` | Nhân viên được giao công việc cấp cha trong dự án | Nên dùng nếu source nghiệp vụ xem phân công cha là tham gia thực tế. | Ưu: phản ánh giao việc thực. Nhược: không có role dự án; cần distinct và lọc `IsDeleted` qua `DanhMucCongViec`, `CongViec`. |
| Phân công chi tiết công việc | `PhanCongCtCongViec`, `CtCongViec`, `CongViec`, `DanhMucCongViec` | `PhanCongCtCongViec.MaChiTietCV -> CtCongViec -> CongViec -> DanhMucCongViec.MaDuAn` | Nhân viên được giao chi tiết công việc trong dự án | Rất nên dùng làm nguồn ưu tiên nếu muốn "tham gia thực tế". | Ưu: gần nhất với thực thi công việc; đang được service dùng để tính thống kê. Nhược: nếu dự án chỉ giao ở cấp cha mà chưa chia chi tiết thì sẽ thiếu người. |
| Báo cáo tiến độ | `TienDoCongViec`, `CtCongViec`, `CongViec`, `DanhMucCongViec` | `TienDoCongViec.MaChiTietCV -> CtCongViec -> CongViec -> DanhMucCongViec.MaDuAn`; `TienDoCongViec.MaNguoiDung` là người báo cáo | Người có báo cáo tiến độ trong dự án | Có thể dùng bổ sung, không nên là nguồn duy nhất. | Ưu: chứng minh tham gia thực tế. Nhược: người chưa báo cáo nhưng được giao việc sẽ bị bỏ sót; cần lọc trạng thái/xóa. |
| Người quản lý dự án | `DuAn.MaNguoiDung` | `DuAn.MaNguoiDung -> NguoiDung.MaNguoiDung` | Manager dự án | Thường không nên là người được đánh giá nếu manager là người đánh giá; chỉ giữ nếu nghiệp vụ yêu cầu đánh giá manager. | Ưu: rõ vai trò quản lý. Nhược: dễ tự đánh giá hoặc sai vai trò nếu đưa vào dropdown người được đánh giá. Source hiện loại tự đánh giá bằng `x.MaNhanVien != currentUserId`. |
| Người tạo đề xuất/công việc | `DeXuatCongViec.MaNguoiDungDeXuat`, `DeXuatNganSach.MaNguoiDungDeXuat`, `CongViec.MaDeXuatCV` | Qua đề xuất hoặc công việc tới dự án | Người đề xuất, không chắc là người thực hiện | Không nên dùng làm nguồn chính khi chưa xác nhận nghiệp vụ. | Ưu: có liên hệ dự án. Nhược: đề xuất không đồng nghĩa tham gia thực hiện. |

Source hiện tại dùng:

- `NhanVienDuAn` làm nguồn chính cho danh sách/list/dropdown.
- `PhanCongCtCongViec` và `TienDoCongViec` để xây dựng thống kê hỗ trợ trong `XayDungThongKeNhanVienAsync`.
- `TeamDuAn`/`NhanVienTeam` để xác định phạm vi leader trong `LayDanhSachDuAnLeaderScopeAsync` và `CoThuocScopeLeaderAsync`.
- `PhanCongCongViec` chưa được dùng trong `DanhGiaNhanVienService` để sinh danh sách người được đánh giá.

## 9. Quy tắc nghiệp vụ đề xuất cho danh sách nhân viên được đánh giá

Quy tắc hiện có từ source:

- Đánh giá theo dự án: dùng `MaDuAn`.
- Người được đánh giá phải thuộc `NhanVienDuAn` của dự án khi mở form/lưu.
- Không cho tự đánh giá: `currentUserId == maNhanVien` bị chặn trong `GetFormAsync` và `LuuDanhGiaAsync`.
- Admin không được thao tác đánh giá: `KiemTraKhongChoAdminTacNghiep`.
- Manager dự án được đánh giá và khi lưu thì bản đánh giá tự `DaDuyet`.
- Leader được đánh giá trong phạm vi team/project nếu thỏa `TeamDuAn` và `NhanVienTeam`.

Quy tắc đề xuất:

- Khi đã vào đánh giá cho một dự án cụ thể, dropdown nhân viên nên chỉ hiện nhân viên của dự án đó.
- Nguồn nên là kết hợp distinct từ các nguồn tham gia thực tế:
  - `NhanVienDuAn` làm nền tảng.
  - `PhanCongCtCongViec` để bắt người được giao chi tiết công việc.
  - `PhanCongCongViec` để bắt người được giao công việc cha.
  - `TienDoCongViec` để bắt người có báo cáo tiến độ.
  - `TeamDuAn` + `NhanVienTeam` nếu team là nguồn phân công dự án chính.
- Nên loại nhân sự `NguoiDung.IsDeleted == true`.
- Nên cân nhắc loại tài khoản chưa kích hoạt/bị khóa bằng `Aspnetusers.EmailConfirmed` và `Aspnetusers.LockoutEnd`, vì module nhân sự có các trạng thái này.
- Nên loại manager khỏi danh sách người được đánh giá nếu nghiệp vụ xác nhận manager là người đánh giá, không phải đối tượng đánh giá.
- Không nên tự bịa đánh giá theo task/chi tiết khi entity hiện chỉ có `MaDuAn` ở header và optional `MaCongViec` ở chi tiết.

Quy tắc cần hỏi lại nghiệp vụ:

- Có đánh giá manager dự án không?
- Có cho nhiều leader cùng đánh giá một nhân viên trong cùng dự án không?
- Một nhân viên trong một dự án được đánh giá một lần duy nhất hay nhiều lần theo thời điểm/người đánh giá?
- Nếu nhân viên thuộc `NhanVienDuAn` nhưng không có phân công/tiến độ, có được đánh giá không?
- Nếu nhân viên có phân công/tiến độ nhưng chưa có `NhanVienDuAn`, có nên tự động hiện trong dropdown không hay dữ liệu phải được đồng bộ vào `NhanVienDuAn` trước?

## 10. Permission, role và data scope

Permission:

- `Permissions.DanhGiaNhanVien.Xem`: xem danh sách/chi tiết/export.
- `Permissions.DanhGiaNhanVien.DanhGia`: tạo đánh giá/gửi duyệt.
- `Permissions.DanhGiaNhanVien.Sua`: sửa đánh giá/gửi duyệt.
- `Permissions.DanhGiaNhanVien.Duyet`: duyệt/từ chối.

Controller:

- `DanhGiaNhanVienController` có `[Authorize]`.
- `Index`, `ChiTiet`, `XuatFile` kiểm tra quyền xem.
- `Form`, `Luu`, `GuiDuyet` kiểm tra quyền đánh giá hoặc sửa.
- `Duyet`, `TuChoi` kiểm tra quyền duyệt.

Service:

- Mỗi method chính gọi `KiemTraQuyenTheoClaim(...)`.
- Role flags lấy từ `Aspnetuserroles` join `Aspnetroles`: `ADMIN`, `MANAGER`, `EMPLOYEE`.
- `KiemTraKhongChoAdminTacNghiep` chặn admin tạo/sửa/gửi/duyệt/từ chối.
- Manager scope: dự án có `DuAn.MaNguoiDung == currentUserId`.
- Leader scope: `NhanVienTeam.IsLeader == true`, dự án từ `NhanVienDuAn.VaiTroTrongDuAn` leader hoặc `TeamDuAn`, và nhân viên cùng `NhanVienTeam`.
- Employee không tự động có quyền đánh giá người khác nếu không là leader/manager; `CoQuyenTaoDanhGiaNhanVien` chỉ trả true cho manager dự án hoặc leader scope.

Data scope:

- List chính `GetPageAsync`: manager chỉ thấy nhân viên trong dự án mình quản lý; leader thấy nhân viên trong team thuộc project leader scope; admin bị trả list rỗng do nhánh `roleFlags.IsAdmin` trả `danhSachDuAn` rỗng và `query.Where(x => false)`.
- Form/lưu: bắt buộc `NhanVienDuAn.Any(x => x.MaDuAn == form.MaDuAn && x.MaNguoiDung == form.MaNhanVien)`.
- Chi tiết: admin xem được; manager dự án xem được; nhân viên được đánh giá xem được; người đánh giá xem được; leader scope xem được.

Chưa thấy:

- Chưa thấy kiểm tra `Aspnetusers.EmailConfirmed`/`LockoutEnd` trong dropdown/list đánh giá.
- Chưa thấy loại trừ manager dự án khỏi dropdown nếu manager không phải current user.
- Chưa thấy loại trừ admin theo role của người được đánh giá trong dropdown, ngoài việc admin hiện tại không được thao tác.
- Chưa thấy rule chống đánh giá nhân viên ngoài dự án bằng phân công/tiến độ; rule hiện tại là phải thuộc `NhanVienDuAn`.

## 11. UI hiện tại

| View | Field/UI | Nguồn dữ liệu | Có lọc theo dự án không | Nhận xét UX |
|---|---|---|---|---|
| `Views/DanhGiaNhanVien/Index.cshtml` | Tiêu đề, mô tả, badge nguồn, danh sách | `DanhGiaNhanVienPageViewModel` | Hiển thị `TenDuAnDangLoc` nếu `MaDuAn` có giá trị | Mô tả "trong dự án đang chọn", nhưng màn độc lập vẫn cho "Tất cả" dự án. |
| `Views/DanhGiaNhanVien/_Filter.cshtml` | Dropdown `Dự án` | `Model.DanhSachDuAn` từ `LayDanhSachDuAnTheoScopeAsync` | Có, khi user chọn `maDuAn` và submit | Không có onchange/AJAX để reload dropdown nhân viên ngay khi đổi dự án. |
| `Views/DanhGiaNhanVien/_Filter.cshtml` | Dropdown `Nhân viên` | `Model.DanhSachNhanVien` từ `LayDanhSachNhanVienTheoScopeAsync` | Có nếu request hiện tại có `maDuAn`; không render khi `maDuAn` null | Sau chỉnh sửa hiện tại, chưa chọn dự án thì không còn dropdown nhân viên rộng. |
| `Views/DanhGiaNhanVien/_Table.cshtml` | Danh sách nhân viên có thể đánh giá | `Model.DanhSach` từ query `NhanVienDuAn` | Có nếu `maDuAn`; nếu không thì theo scope nhiều dự án | Có hiển thị dự án, nhân viên, vai trò, tiến độ, trạng thái. |
| `Views/DanhGiaNhanVien/_Table.cshtml` | Nút `Đánh giá`, `Sửa đánh giá`, `Xem đánh giá`, `Gửi duyệt`, `Duyệt`, `Từ chối` | `CoTheDanhGia`, `CoTheSua`, `CoTheGuiDuyet`, `CoTheDuyet`, `CoTheTuChoi` | Theo row | Ẩn/hiện theo permission/service flag. |
| `Views/DanhGiaNhanVien/_Form.cshtml` | Hidden `MaDuAn`, `MaNhanVien` | `DanhGiaNhanVienFormViewModel` | Form gắn chặt một dự án/nhân viên | Server vẫn validate lại, tránh chỉ tin hidden input. |
| `Views/DanhGiaNhanVien/_Form.cshtml` | Điểm `DiemDanhGiaNV` | `TieuChi[i].DiemDanhGiaNV` | Không liên quan | Input có `min=1`, `max=10`; server cũng validate. |
| `Views/DanhGiaNhanVien/_Form.cshtml` | `MaCongViec` tùy chọn | `TieuChi[i].MaCongViec` | Công việc options được service nạp thành dictionary tên, nhưng UI là input số | UX chưa thân thiện; chưa validate `MaCongViec` thuộc dự án khi submit. |
| `Views/DanhGiaNhanVien/_Form.cshtml` | Nhận xét tiêu chí/tổng quan | `NoiDungDanhGiaNhanVien`, `NhanXetTongQuan` | Không liên quan | Có `maxlength=500`; server validate. |
| `Views/DanhGiaNhanVien/_EmptyState.cshtml` | Empty message | `Model.ThongBaoRong`, fallback theo `MaDuAn` | Có phân biệt chưa chọn dự án và dự án không có nhân viên | Nội dung hiện có thể báo "Vui lòng chọn dự án..." hoặc "Dự án này chưa có nhân viên...". |
| `Views/DanhGiaNhanVien/ChiTiet.cshtml` | Chi tiết đánh giá | `DanhGiaNhanVienChiTietViewModel` | Theo `maDanhGiaNhanVien` | Có hiển thị dữ liệu hỗ trợ và tiêu chí. |

## 12. JavaScript liên quan

Đã kiểm tra `QuanLyDuAn/QuanLyDuAn/wwwroot/js` và các view `Views/DanhGiaNhanVien`.

Không tìm thấy:

- AJAX load nhân viên theo dự án.
- `onchange` dự án để reload nhân viên.
- Endpoint JSON trả danh sách nhân viên.
- Select2/autocomplete riêng cho đánh giá nhân viên.
- Submit form bằng `fetch`/AJAX.

Source hiện dùng server-side render dropdown. Nếu sửa tối thiểu, có thể sửa bằng query/service/viewmodel trước, chưa cần JavaScript. Nếu muốn UX đổi dự án là dropdown nhân viên cập nhật ngay, khi đó mới cân nhắc thêm JS hoặc endpoint AJAX, nhưng không bắt buộc cho nghiệp vụ đúng.

## 13. Có cần sửa database không

Để chỉ load nhân viên thuộc dự án, không cần sửa database nếu dựa vào các bảng quan hệ đã có:

- `NhanVienDuAn` đã có khóa `{ MaDuAn, MaNguoiDung }`.
- `PhanCongCongViec` đã có `{ MaNguoiDung, MaCongViec }`.
- `PhanCongCtCongViec` đã có `{ MaNguoiDung, MaChiTietCV }`.
- `TeamDuAn` đã có `{ MaTeam, MaDuAn }`.
- `NhanVienTeam` đã có `{ MaNguoiDung, MaTeam }`.
- `TienDoCongViec` đã có `MaNguoiDung`, `MaChiTietCV`.

Không cần migration để sửa dropdown/list theo dự án. Có thể sửa bằng query/service:

- Chốt `maDuAn` khi vào từ dự án.
- Khi `maDuAn` null, cân nhắc yêu cầu chọn dự án trước hoặc vẫn hiển thị theo scope nhưng label rõ ràng.
- Nếu nghiệp vụ yêu cầu nguồn tham gia thực tế, service có thể union/distinct từ các nguồn hiện có.

Unique constraint:

- Không thấy unique DB cho `DanhGiaNhanVien` theo `MaDuAn + MaNguoiDung`.
- Không nên tự thêm migration trong lần này.
- Nếu cần chống trùng, có thể chặn ở service sau khi xác nhận quy tắc: theo nhân viên/dự án hay theo nhân viên/dự án/người đánh giá.

Không cần thêm bảng quan hệ nếu dùng các bảng hiện có. Chỉ cần thêm bảng nếu nghiệp vụ muốn lịch sử kỳ đánh giá, tự đánh giá, peer review, hoặc workflow đánh giá nhiều cấp độc lập.

## 14. Kết luận nhanh

1. Sau chỉnh sửa hiện tại, source không còn load toàn bộ nhân viên hệ thống khi `maDuAn == null`. `DanhGiaNhanVienService.GetPageAsync` trả `DanhSachNhanVien = new List<...>()`, `DanhSach = new List<...>()`, `Form = null`, `CanChonDuAnTruoc = true`, `ThongBaoRong = "Vui lòng chọn dự án để đánh giá nhân viên."`.
2. Không tìm thấy đoạn code hiện tại gây load toàn bộ `NguoiDung` trong chức năng `DanhGiaNhanVien`. Nguồn list/dropdown là `NhanVienDuAn` join `NguoiDung`.
3. Nếu người dùng vẫn thấy giống "toàn bộ nhân viên", nguyên nhân phù hợp nhất với source hiện tại là dữ liệu `NhanVienDuAn` của dự án đó đang chứa nhiều người, hoặc hệ thống đang hiểu "nhân viên dự án" là dòng trong `NhanVienDuAn`, không phải người thật sự có phân công/tiến độ.
4. Khi vào từ `Đánh giá dự án`, `Views/DanhGiaDuAn/_Table.cshtml` truyền đúng `asp-route-maDuAn="@item.MaDuAn"` và `asp-route-nguon="du-an"` sang `DanhGiaNhanVienController.Index`.
5. `DanhGiaNhanVienController.Index` nhận đúng parameter `int? maDuAn` và truyền thẳng vào `IDanhGiaNhanVienService.GetPageAsync(...)`.
6. Khi `maDuAn` có giá trị, bảng chính và dropdown đều lọc theo `nvda.MaDuAn == maDuAn.Value`.
7. Danh sách đang lấy theo `NhanVienDuAn`, không lấy theo `PhanCongCongViec`, `PhanCongCtCongViec`, `TienDoCongViec`, cũng không union nhiều nguồn.
8. Nếu mục tiêu nghiệp vụ là "chỉ nhân viên thật sự thực hiện dự án", cần tiếp tục chỉnh query và validation; hướng nên chọn là phương án C theo từng bước: giữ `NhanVienDuAn` là nguồn membership chính, sau đó thêm bộ lọc/nguồn người có phân công hoặc tiến độ theo quy tắc được xác nhận.
9. Không cần sửa database cho việc lọc theo nguồn phân công/tiến độ nếu dùng các bảng hiện có. Chỉ cần sửa database nếu muốn thêm kỳ đánh giá, lịch sử nhiều kỳ, hoặc unique constraint.

## 15. Đề xuất chỉnh sửa sau khi xác nhận

Nếu mục tiêu là "chỉ nhân viên thuộc dự án":

1. Source hiện tại đã đi đúng hướng này sau chỉnh sửa: `GetPageAsync` và `LayDanhSachNhanVienTheoScopeAsync` đều dùng `NhanVienDuAn` và lọc `MaDuAn`.
2. Cần kiểm tra dữ liệu `NHAN_VIEN_DU_AN` của dự án cụ thể, ví dụ DATA10-009, để xác nhận 6 người đang hiển thị có đúng là 6 dòng membership của dự án không.
3. Không cần sửa database, không cần AJAX, không cần đổi workflow duyệt.

Nếu mục tiêu là "chỉ nhân viên thực sự tham gia thực hiện dự án":

1. Cần quyết định nguồn hợp lệ: `PhanCongCongViec`, `PhanCongCtCongViec`, `TienDoCongViec`, hoặc kết hợp với `NhanVienDuAn`.
2. Cần sửa đồng bộ ở `DanhGiaNhanVienService.GetPageAsync`, `LayDanhSachNhanVienTheoScopeAsync`, `GetFormAsync`, `LuuDanhGiaAsync`.
3. Không chỉ sửa dropdown; nếu không sửa validation lưu thì UI có thể chọn được người nhưng service vẫn chặn theo `NhanVienDuAn`.
4. Có thể đổi label view thành "Nhân viên trong dự án" hoặc "Nhân viên có phân công trong dự án" tùy rule cuối cùng.
5. Có thể thêm checkbox/filter "Chỉ người có phân công" nếu muốn giữ cả hai góc nhìn: membership dự án và thực hiện thực tế.
6. Nếu chặn submit người không có phân công, service phải kiểm tra cùng một nguồn với dropdown.
7. Không cần sửa database nếu chỉ dùng bảng hiện có. Cần migration chỉ khi thêm kỳ đánh giá, bảng quan hệ mới, hoặc unique constraint.

## 16. Test case kiểm thử

| Mã test | Màn hình/chức năng | Tình huống | Dữ liệu | Kết quả mong đợi | Cách kiểm tra | Ghi chú |
|---|---|---|---|---|---|---|
| DGN-01 | Dropdown nhân viên | Vào đánh giá nhân viên của dự án A | `maDuAn=A`, nhân viên A1/A2 thuộc dự án A | Dropdown chỉ hiện nhân viên tham gia dự án A theo nguồn đã chọn | Mở `/DanhGiaNhanVien?maDuAn=A`, kiểm tra `<select name="maNhanVien">` | Hiện source dùng `NhanVienDuAn`. |
| DGN-02 | Dropdown nhân viên | Nhân viên thuộc dự án B nhưng không thuộc dự án A | B1 thuộc B, không thuộc A | B1 không xuất hiện | So sánh dữ liệu `NhanVienDuAn` hoặc nguồn distinct đã chọn | Bắt lỗi scope chéo dự án. |
| DGN-03 | Nguồn chi tiết công việc | Nhân viên được phân công chi tiết công việc trong dự án A | `PhanCongCtCongViec` qua `CtCongViec -> CongViec -> DanhMucCongViec.MaDuAn=A` | Theo source hiện tại: không tự xuất hiện nếu không có `NhanVienDuAn`; nếu chọn phương án thực hiện thực tế thì phải xuất hiện | Query dữ liệu, mở dropdown | Cần sửa thêm nếu dùng nguồn này. |
| DGN-04 | Nguồn công việc cha | Nhân viên được phân công công việc cha trong dự án A | `PhanCongCongViec` qua `CongViec -> DanhMucCongViec.MaDuAn=A` | Theo source hiện tại: không tự xuất hiện nếu không có `NhanVienDuAn`; nếu chọn phương án thực hiện thực tế thì phải xuất hiện | Query dữ liệu và mở dropdown | Cần sửa thêm nếu dùng nguồn này. |
| DGN-05 | Nguồn team dự án | Nhân viên thuộc team dự án A | `TeamDuAn.MaDuAn=A`, `NhanVienTeam.MaTeam` | Theo source hiện tại: dùng cho leader scope, không phải nguồn dropdown độc lập | Query dữ liệu và mở dropdown | Cần quyết định team có là nguồn chính không. |
| DGN-06 | Nhân sự inactive | Nhân viên đã xóa/nghỉ/khóa | `NguoiDung.IsDeleted=true` hoặc tài khoản bị khóa | Không xuất hiện nếu nguồn query lọc đúng | Kiểm tra query và UI | Hiện source chắc chắn lọc `NguoiDung.IsDeleted`; chưa lọc `Aspnetusers.LockoutEnd`. |
| DGN-07 | Manager dự án | Manager có bị loại hoặc giữ lại đúng rule | `DuAn.MaNguoiDung=managerId` | Theo source hiện tại: chặn tự đánh giá; manager khác có thể hiện nếu có trong `NhanVienDuAn` và không phải current user | Kiểm tra dropdown và submit | Cần hỏi lại có loại manager khỏi đối tượng không. |
| DGN-08 | Submit thủ công ngoài dự án | Sửa HTML/request đổi `MaNhanVien` sang người ngoài dự án | `MaDuAn=A`, `MaNhanVien=B1` ngoài A | Service từ chối | POST `/DanhGiaNhanVien/Luu` thủ công | Hiện source chặn bằng `NhanVienDuAn` join `NguoiDung`. |
| DGN-09 | Chặn trùng | Đánh giá lại cùng nhân viên trong cùng dự án | Cùng `MaDuAn`, `MaNhanVien` | Không tạo trùng nếu rule chặn được xác nhận | Kiểm tra số bản ghi `DanhGiaNhanVien` | Hiện chưa có unique DB; service sửa bản mới nhất theo người đánh giá. |
| DGN-10 | Dự án chưa có nhân viên | Vào dự án A không có ứng viên | `maDuAn=A`, không có `NhanVienDuAn` hợp lệ | UI hiển thị "Dự án này chưa có nhân viên để đánh giá." | Mở trang và kiểm tra empty state | Đã có property `ThongBaoRong`. |
| DGN-11 | Manager scope | Manager chỉ đánh giá dự án mình quản lý | Manager M, dự án A của M, dự án B không của M | M chỉ thấy/đánh giá A | Kiểm tra `/DanhGiaNhanVien?maDuAn=B` và form | Service lọc `DuAn.MaNguoiDung == currentUserId`. |
| DGN-12 | Leader scope | Leader chỉ đánh giá thành viên/team nếu nghiệp vụ cho phép | Leader L, team T, dự án A có T | L chỉ thấy thành viên team T trong A | Kiểm tra list/form | Source dùng `TeamDuAn` + `NhanVienTeam` cho scope leader. |
| DGN-13 | Employee thường | Employee không có quyền đánh giá người khác | Employee không leader | Không thấy nút hoặc service từ chối | Kiểm tra UI và gọi form trực tiếp | `CoQuyenTaoDanhGiaNhanVien` không cho employee thường. |
| DGN-14 | Validate điểm | Điểm ngoài khoảng hợp lệ | `DiemDanhGiaNV=0` hoặc `11` | Bị chặn | Submit form | ViewModel `[Range(1,10)]`, service validate 1-10. |
| DGN-15 | Font tiếng Việt | Form/list hiển thị tiếng Việt | Tên dự án/nhân viên/nhận xét có dấu | Không lỗi font | Mở trang, kiểm tra file UTF-8 | Cần kiểm tra sau mọi lần sửa view/doc. |
| DGN-16 | Điều hướng từ Đánh giá dự án | Bấm nút sang Đánh giá nhân viên | Row đánh giá dự án có `MaDuAn=A` | URL có `maDuAn=A`, `nguon=du-an`; controller nhận đúng `maDuAn` | Inspect link render hoặc log route model binding | Source `DanhGiaDuAn/_Table.cshtml` truyền `asp-route-maDuAn`. |
| DGN-17 | Membership rộng, phân công hẹp | Dự án A có 6 dòng `NhanVienDuAn` nhưng chỉ 2 người có phân công | 6 membership, 2 assignment | Nếu dùng `NhanVienDuAn`: hiện 6. Nếu dùng người thực hiện thực tế: hiện 2 | So sánh UI với query `NhanVienDuAn` và query phân công | Case đúng với nghi ngờ DATA10-009. |
| DGN-18 | Có membership nhưng chưa có phân công/tiến độ | Nhân viên có trong `NhanVienDuAn`, không có `PhanCong*`/`TienDo` | Nhân viên A3 | Theo source hiện tại: vẫn hiện. Theo rule thực hiện thực tế: không hiện | Kiểm tra list/dropdown | Cần quyết định nghiệp vụ. |
| DGN-19 | Có phân công chi tiết nhưng không có membership | Nhân viên có `PhanCongCtCongViec`, không có `NhanVienDuAn` | Nhân viên A4 | Theo source hiện tại: không hiện và service chặn lưu. Nếu đổi nguồn, phải quyết định có cho đánh giá không | POST form thủ công và kiểm tra dropdown | Không nên để UI và service lệch nhau. |
| DGN-20 | Dropdown khi `maDuAn` null | Vào menu độc lập `/DanhGiaNhanVien` | Không có query `maDuAn` | Không load nhân viên, hiện thông báo chọn dự án trước | Mở URL và kiểm tra source HTML | Source hiện tại đã xử lý. |
| DGN-21 | Dropdown khi `maDuAn` có giá trị | Vào `/DanhGiaNhanVien?maDuAn=A` | Dự án A có membership | Chỉ lấy đúng nguồn đã chọn cho A | So sánh với query nguồn | Hiện nguồn là `NhanVienDuAn`. |
| DGN-22 | Submit người không thuộc nguồn hợp lệ | Sửa request để đánh giá người ngoài nguồn | `MaDuAn=A`, `MaNhanVien` không hợp lệ | Service chặn | POST `/DanhGiaNhanVien/Luu` thủ công | Hiện chặn ngoài `NhanVienDuAn`; nếu đổi sang nguồn thực hiện, phải chặn theo nguồn mới. |

## 17. Checklist thông tin cần gửi lại cho ChatGPT

1. Route từ Đánh giá dự án sang Đánh giá nhân viên có truyền `maDuAn` không: Có, trong `Views/DanhGiaDuAn/_Table.cshtml` dùng `asp-route-maDuAn="@item.MaDuAn"`.
2. Controller `Index` có nhận `maDuAn` không: Có, `DanhGiaNhanVienController.Index(int? maDuAn, ...)`.
3. Service `GetPageAsync` có dùng `maDuAn` không: Có, khi null trả empty; khi có giá trị lọc `nvda.MaDuAn == maDuAn.Value`.
4. Dropdown nhân viên lấy từ method nào: `DanhGiaNhanVienService.LayDanhSachNhanVienTheoScopeAsync`.
5. Method đó query từ bảng nào: `NhanVienDuAn` join `NguoiDung`, có scope thêm từ `DuAn`, `TeamDuAn`, `NhanVienTeam`.
6. Khi `maDuAn` null method trả gì: danh sách rỗng.
7. Khi `maDuAn` có giá trị method trả gì: danh sách nhân viên thuộc `NhanVienDuAn` của dự án đó, loại current user, loại `NguoiDung.IsDeleted`, có scope manager/leader.
8. Danh sách trong bảng lấy từ bảng nào: `GetPageAsync` query chính từ `NhanVienDuAn` join `DuAn` join `NguoiDung`.
9. Có dùng `PhanCongCongViec` không: Không dùng để sinh list/dropdown hiện tại.
10. Có dùng `PhanCongCtCongViec` không: Không dùng để sinh list/dropdown; có dùng trong `XayDungThongKeNhanVienAsync`.
11. Có dùng `TienDoCongViec` không: Không dùng để sinh list/dropdown; có dùng trong thống kê tiến độ.
12. Có dùng `TeamDuAn`/`NhanVienTeam` không: Có dùng cho scope leader, không phải nguồn ứng viên độc lập.
13. Có chặn submit người ngoài dự án không: Có, chặn theo membership `NhanVienDuAn` của `MaDuAn`.
14. Có chặn submit người không có phân công không: Không; service không yêu cầu có `PhanCongCongViec`, `PhanCongCtCongViec` hoặc `TienDoCongViec`.
15. Có cần sửa database không: Không cho hướng lọc/query bằng bảng hiện có; có nếu muốn thêm kỳ đánh giá, lịch sử, hoặc unique constraint.
16. Nên sửa theo phương án A/B/C nào: Đề xuất phương án C theo từng bước nếu mục tiêu là phản ánh người thực sự làm việc; còn nếu chỉ cần "nhân viên thuộc dự án" thì source hiện tại đang là phương án A.

## 18. Kiểm tra lại sau chỉnh sửa hiện tại

Các file đã đọc lại trong lần kiểm tra này:

- `QuanLyDuAn/QuanLyDuAn/Controllers/DanhGiaNhanVienController.cs`.
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDanhGiaNhanVienService.cs`.
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaNhanVienService.cs`.
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaNhanVien/*`.
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaNhanVien/Index.cshtml`, `_Filter.cshtml`, `_Table.cshtml`, `_EmptyState.cshtml`, `_Form.cshtml`.
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_Table.cshtml`.
- `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml`.
- `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml`.
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DanhGiaNhanVien.cs`, `CtDanhGiaNhanVien.cs`, `NhanVienDuAn.cs`, `TeamDuAn.cs`, `NhanVienTeam.cs`, `PhanCongCongViec.cs`, `PhanCongCtCongViec.cs`, `TienDoCongViec.cs`, `CongViec.cs`, `CtCongViec.cs`, `DanhMucCongViec.cs`.
- `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`.

Kết quả kiểm tra source hiện tại:

| Điểm kiểm tra | Kết luận | Bằng chứng source |
|---|---|---|
| Khi `maDuAn == null` | Không load dropdown nhân viên, không load danh sách nhân viên | `DanhGiaNhanVienService.GetPageAsync` nhánh `if (!maDuAn.HasValue)` gán list rỗng và `ThongBaoRong`. |
| Khi `maDuAn` có giá trị | Bảng chính lọc đúng dự án | `GetPageAsync` query `NhanVienDuAn` có `nvda.MaDuAn == maDuAn.Value`. |
| Dropdown nhân viên | Lọc đúng dự án khi có `maDuAn` | `LayDanhSachNhanVienTheoScopeAsync` có `where nvda.MaDuAn == maDuAn.Value`. |
| View filter | Không hiện dropdown nhân viên khi chưa chọn dự án | `_Filter.cshtml` chỉ render dropdown nhân viên trong nhánh `if (daChonDuAn)`. |
| View table | Render `Model.DanhSach` đã được service lọc | `_Table.cshtml` chỉ foreach `Model.DanhSach`, không tự query thêm. |
| Empty state | Có thông báo chọn dự án hoặc không có nhân viên | `_EmptyState.cshtml` ưu tiên `Model.ThongBaoRong`. |
| Form/lưu | Chặn người ngoài dự án theo `NhanVienDuAn` | `GetFormAsync` và `LuuDanhGiaAsync` validate membership bằng `NhanVienDuAn` join `NguoiDung`. |
| Người thực hiện thật | Chưa dùng làm nguồn danh sách | `PhanCongCongViec`, `PhanCongCtCongViec`, `TienDoCongViec` không nằm trong query list/dropdown. |

## 19. Luồng từ Đánh giá dự án sang Đánh giá nhân viên

Trong `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_Table.cshtml`, link sang đánh giá nhân viên gọi:

- Controller: `DanhGiaNhanVien`.
- Action: `Index`.
- Route/query truyền: `asp-route-maDuAn="@item.MaDuAn"`.
- Route/query nguồn: `asp-route-nguon="du-an"`.
- Route/query quay lại: `asp-route-returnUrl="@currentUrl"`.
- Không truyền `maDanhGiaDuAn`.
- Không thấy truyền nhầm tên như `MaDuAn`, `id`, `projectId`; tên route là `maDuAn`, khớp parameter action.

URL thực tế sẽ có dạng tương đương:

```text
/DanhGiaNhanVien?maDuAn=<MaDuAn>&nguon=du-an&returnUrl=<currentUrl-encoded>
```

Trong `QuanLyDuAn/QuanLyDuAn/Controllers/DanhGiaNhanVienController.cs`, action `Index` nhận:

```csharp
public async Task<IActionResult> Index(
    int? maDuAn,
    int? maNhanVien,
    string? tuKhoa,
    string? trangThai,
    string? nguon,
    DateTime? tuNgayDanhGia,
    DateTime? denNgayDanhGia,
    int pageNumber = 1,
    int pageSize = 10)
```

Controller truyền `maDuAn` vào `_service.GetPageAsync(...)` và gán `vm.Nguon = nguon`. Không thấy log/debug/TempData nào làm thay đổi `maDuAn`. Nếu route helper render đúng URL, `maDuAn` sẽ đến được controller theo model binding mặc định của ASP.NET Core MVC.

Các nguồn điều hướng khác:

- `Views/DuAn/Details.cshtml` cũng có link `asp-controller="DanhGiaNhanVien"`, `asp-action="Index"`, `asp-route-maDuAn="@Model.MaDuAn"`.
- `Views/Shared/_Layout.cshtml` có menu độc lập vào `DanhGiaNhanVien/Index` không truyền `maDuAn`; đây là trường hợp service hiện trả trạng thái yêu cầu chọn dự án.

## 20. So sánh NhanVienDuAn và người thực sự thực hiện dự án

| Khái niệm | Bảng/entity nguồn | Code hiện có dùng không | Có lọc theo `maDuAn` không | Ý nghĩa nghiệp vụ | Có nên dùng cho đánh giá không |
|---|---|---|---|---|---|
| Nhân viên thuộc dự án | `NhanVienDuAn` (`MaDuAn`, `MaNguoiDung`, `VaiTroTrongDuAn`) | Có, là nguồn chính của list/dropdown và validation | Có, `nvda.MaDuAn == maDuAn.Value` khi đã chọn dự án | Membership chính thức của dự án theo source hiện tại | Có, nếu nghiệp vụ là đánh giá thành viên dự án. Nếu DATA10-009 có 6 dòng `NhanVienDuAn`, UI hiện 6 là đúng theo code. |
| Nhân viên thuộc team dự án | `TeamDuAn` + `NhanVienTeam` | Có dùng cho leader scope, không dùng làm nguồn list độc lập | Có qua `TeamDuAn.MaDuAn` trong scope leader | Xác định leader/team có quyền nhìn/đánh giá ai | Có thể dùng bổ sung nếu team là nguồn tham gia chính, nhưng phải đồng bộ validation. |
| Nhân viên được phân công công việc cha | `PhanCongCongViec` + `CongViec` + `DanhMucCongViec.MaDuAn` | Không dùng để sinh list/dropdown hiện tại | Có thể join tới dự án qua `CongViec.MaDanhMucCV -> DanhMucCongViec.MaDuAn` | Người có giao việc cấp cha | Nên dùng nếu muốn phản ánh người thực sự làm việc, nhưng hiện chưa dùng. |
| Nhân viên được phân công chi tiết công việc | `PhanCongCtCongViec` + `CtCongViec` + `CongViec` + `DanhMucCongViec.MaDuAn` | Không dùng để sinh list/dropdown; có dùng trong `XayDungThongKeNhanVienAsync` | Có thể join tới dự án qua chuỗi chi tiết -> công việc -> danh mục -> dự án | Người trực tiếp làm task/detail | Nên ưu tiên nếu muốn đánh giá người thực sự thực hiện dự án. |
| Nhân viên có báo cáo tiến độ | `TienDoCongViec` + `CtCongViec` + `CongViec` + `DanhMucCongViec.MaDuAn` | Không dùng để sinh list/dropdown; có dùng trong thống kê | Có thể join tới dự án qua `MaChiTietCV` | Người đã cập nhật tiến độ thực tế | Nên dùng bổ sung để không bỏ sót người có hoạt động thực tế. |
| Người quản lý dự án | `DuAn.MaNguoiDung` | Có dùng cho manager scope/quyền duyệt/tự duyệt | Có theo `DuAn.MaDuAn` | Người quản lý và thường là người đánh giá | Nên hỏi lại nghiệp vụ. Source hiện chỉ loại current user khỏi list, chưa loại mọi manager khỏi đối tượng đánh giá. |

Kết luận cho dữ liệu DATA10-009 đang hiển thị 6 người:

- Không truy vấn database thật trong lần này, nên không khẳng định được 6 dòng dữ liệu cụ thể trong SQL Server.
- Nhưng theo source hiện tại, nếu UI hiển thị 6 nhân viên khi URL có `maDuAn` của DATA10-009, 6 nhân viên đó đến từ query `NhanVienDuAn` join `NguoiDung` với điều kiện `NhanVienDuAn.MaDuAn == <MaDuAn DATA10-009>`, sau khi loại current user và `NguoiDung.IsDeleted`.
- Không có code nào trong list/dropdown hiện tại lấy 6 người từ `PhanCongCongViec`, `PhanCongCtCongViec`, `TienDoCongViec`, hoặc union nhiều nguồn.
- Vì vậy câu đúng nhất là: "UI đang đúng theo `NhanVienDuAn`, nhưng chưa chắc đúng theo người thực sự thực hiện công việc."

## 21. Kết luận nguyên nhân còn hiển thị nhiều nhân viên

Nguyên nhân có khả năng cao nhất theo source hiện tại:

1. Code hiện tại không còn load toàn bộ nhân viên khi chưa có `maDuAn`.
2. Route từ `Đánh giá dự án` sang `Đánh giá nhân viên` có truyền `maDuAn`.
3. Controller nhận `maDuAn` và service dùng `maDuAn`.
4. View render `Model.DanhSach` và `Model.DanhSachNhanVien` từ service, không tự ghi đè bằng nguồn khác.
5. Danh sách đang là "nhân viên thuộc `NhanVienDuAn` của dự án", không phải "nhân viên được phân công hoặc có tiến độ".
6. Nếu DATA10-009 hiện 6 người như Bùi Hoàng Phúc, Đặng Quốc Khánh, Lê Ngọc Mai, Nguyễn Thảo Vy, Trần Gia Huy, Trần Ngọc Lan, source cho thấy các tên này phải đến từ `NhanVienDuAn` của chính dự án đó nếu URL có `maDuAn` đúng.
7. Nếu 6 người này không phải người thực sự làm DATA10-009, vấn đề nằm ở quy tắc nghiệp vụ hoặc dữ liệu membership `NhanVienDuAn`, không phải ở filter `maDuAn` hiện tại.

Không tìm thấy lỗi dạng:

- `maDuAn` không được truyền từ `DanhGiaDuAn/_Table.cshtml`.
- `Index` không nhận `maDuAn`.
- `GetPageAsync` nhận `maDuAn` nhưng không dùng.
- `LayDanhSachNhanVienTheoScopeAsync` bị gọi với `maDuAn null` khi đã chọn dự án.
- ViewModel bị ghi đè `DanhSachNhanVien` bằng toàn bộ `NguoiDung`.

Điểm chưa đáp ứng nếu nghiệp vụ mới là "người thực sự thực hiện dự án":

- Chưa lọc theo `PhanCongCongViec`.
- Chưa lọc theo `PhanCongCtCongViec`.
- Chưa lọc theo `TienDoCongViec`.
- Chưa chặn submit người có membership nhưng không có phân công/tiến độ.

## 22. Phương án sửa tiếp theo được đề xuất

### Phương án A - Giữ `NhanVienDuAn` là nguồn chính

- Quy tắc: chỉ đánh giá nhân viên có trong `NhanVienDuAn` của dự án.
- Ưu điểm: phù hợp source hiện tại, ít sửa, dropdown/list/form/lưu đang cùng một rule.
- Nhược điểm: nếu `NhanVienDuAn` được thêm quá rộng, UI vẫn hiện nhiều người không thực sự làm việc.
- Không cần sửa database.
- Chỉ cần kiểm tra hoặc làm sạch dữ liệu `NhanVienDuAn` nếu thấy dự án có quá nhiều thành viên.

### Phương án B - Chỉ lấy người có phân công hoặc tiến độ

- Nguồn có thể dùng: `PhanCongCongViec`, `PhanCongCtCongViec`, `TienDoCongViec`.
- Ưu điểm: phản ánh người thật sự làm việc tốt hơn.
- Nhược điểm: có thể bỏ sót người thuộc dự án nhưng chưa được giao việc hoặc chưa báo cáo; phải sửa validation lưu vì hiện đang validate theo `NhanVienDuAn`.
- Cần sửa `GetPageAsync`, `LayDanhSachNhanVienTheoScopeAsync`, `GetFormAsync`, `LuuDanhGiaAsync`, và test chống sửa request thủ công.
- Không cần sửa database nếu các bảng hiện có đủ dữ liệu.

### Phương án C - Kết hợp theo từng bước

- Giữ `NhanVienDuAn` là nguồn membership chính để đảm bảo không đánh giá người ngoài dự án.
- Thêm rule hoặc filter "chỉ người có tham gia công việc" dựa trên `PhanCongCongViec`, `PhanCongCtCongViec`, `TienDoCongViec`.
- Có thể chọn intersection: người phải có `NhanVienDuAn` và có phân công/tiến độ. Cách này an toàn với validation hiện tại hơn.
- Có thể chọn union: người có `NhanVienDuAn` hoặc có phân công/tiến độ. Cách này cần quyết định nếu người có phân công nhưng chưa có `NhanVienDuAn` thì service cho đánh giá hay yêu cầu đồng bộ vào `NhanVienDuAn`.
- Đề xuất cho hệ thống hiện tại: chọn phương án C theo kiểu intersection hoặc checkbox/filter trước, vì nó giữ mô hình dự án hiện có nhưng giải quyết nghi ngờ "thành viên dự án quá rộng".
- Không cần thêm JavaScript nếu lọc server-side qua form; có thể thêm checkbox/filter sau khi xác nhận nghiệp vụ.
- Không sửa database trong bước tiếp theo nếu chỉ đổi query/service/view.

## 23. Cập nhật ràng buộc điểm đánh giá 1-10

Ngày cập nhật: 2026-07-04.

Phạm vi: module `DanhGiaNhanVien`, không sửa database, không tạo migration, không đổi schema và không thay đổi workflow đánh giá nhân viên hiện tại.

Các lớp đã ràng buộc:

| Lớp | File/method | Nội dung |
|---|---|---|
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaNhanVien/DanhGiaNhanVienTieuChiViewModel.cs` | `DiemDanhGiaNV` trước đó đã có `[Range(1, 10)]`; đã bổ sung thông báo tiếng Việt và `[Required(ErrorMessage = "Vui lòng nhập điểm đánh giá.")]`. |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaNhanVien/DanhGiaNhanVienFormViewModel.cs` | `TieuChi` có `[MinLength(1, ErrorMessage = "Vui lòng nhập ít nhất một tiêu chí đánh giá.")]`. |
| Razor View | `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaNhanVien/_Form.cshtml` | Input điểm dùng `type="number"`, `min="1"`, `max="10"`, `step="1"`, `required` và hiển thị lỗi bằng `asp-validation-for`. |
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/DanhGiaNhanVienController.cs`, action `Luu` | Đã có kiểm tra `ModelState.IsValid`; khi invalid không gọi service lưu. |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaNhanVienService.cs`, method `LuuDanhGiaAsync` | Kiểm tra từng `DiemDanhGiaNV` trước khi lưu; điểm ngoài 1-10 bị chặn server-side. |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaNhanVienService.cs`, method `GuiDuyetAsync` | Kiểm tra lại điểm đã lưu trong `CT_DANH_GIA_NHAN_VIEN`; nếu có điểm null/ngoài 1-10 thì không cho gửi duyệt. |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaNhanVienService.cs`, method `DuyetAsync` | Kiểm tra lại điểm đã lưu trước khi duyệt; nếu có điểm null/ngoài 1-10 thì không cho duyệt. |

Test case cần chạy/bảo đảm:

| Mã test | Tình huống | Kết quả mong đợi |
|---|---|---|
| DGNV-DIEM-01 | Nhập điểm 1 | Lưu được |
| DGNV-DIEM-02 | Nhập điểm 10 | Lưu được |
| DGNV-DIEM-03 | Nhập điểm 0 | Bị chặn bởi ViewModel/Service |
| DGNV-DIEM-04 | Nhập điểm 11 | Bị chặn bởi ViewModel/Service |
| DGNV-DIEM-05 | Nhập điểm âm | Bị chặn bởi ViewModel/Service |
| DGNV-DIEM-06 | Bỏ trống điểm | Bị chặn bởi `required`/ModelState hoặc service không nhận điểm hợp lệ |
| DGNV-DIEM-07 | Sửa HTML/request gửi điểm 99 | Service `LuuDanhGiaAsync` vẫn chặn |
| DGNV-DIEM-08 | Bản có điểm sai trong DB cũ | `GuiDuyetAsync`/`DuyetAsync` không cho chuyển trạng thái |
| DGNV-DIEM-09 | Toàn bộ điểm hợp lệ | Lưu/gửi duyệt/duyệt đúng workflow hiện tại |
| DIEM-UTF8-01 | Kiểm tra font tiếng Việt | View và docs giữ UTF-8 |
| DIEM-DB-01 | Kiểm tra migration/schema | Không có migration mới, không sửa database |

Kết quả kiểm tra sau cập nhật điểm:

- Đã chạy `dotnet build .\QuanLyDuAn\QuanLyDuAn.sln`: build thành công, `0 Error(s)`, còn `2 Warning(s)` cũ ở `FileTienDoCongViecService.cs`.
- Đã kiểm tra UTF-8 cho các file đã sửa trong module `DanhGiaNhanVien`: không có ký tự replacement `U+FFFD`.
- Không tạo migration mới; thư mục `QuanLyDuAn/QuanLyDuAn/Migrations` chỉ có migration cũ.
