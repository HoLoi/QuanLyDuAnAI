# Phân tích chức năng Công việc

Tài liệu này ghi nhận trạng thái AS-IS của chức năng `/CongViec` theo source hiện tại. Nhiệm vụ này chỉ đọc source và lập tài liệu, chưa sửa Controller, Service, View, CSS, JavaScript, CSDL, migration, route, permission hoặc workflow.

## 1. Phạm vi source đã đọc

| STT | Đường dẫn file | Class/View | Vai trò trong chức năng |
|-----|----------------|------------|--------------------------|
| 1 | `QuanLyDuAn/QuanLyDuAn/Controllers/CongViecController.cs` | `CongViecController` | Route `/CongViec`, action `Index`, workflow POST, xuất file. |
| 2 | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/ICongViecService.cs` | `ICongViecService` | Contract `GetPageAsync`, xác nhận hoàn thành, mở lại công việc. |
| 3 | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs` | `CongViecService` | Tạo `IQueryable`, data scope, filter, search, count, phân trang, projection, workflow UI. |
| 4 | `QuanLyDuAn/QuanLyDuAn/ViewModels/CongViec/CongViecPageViewModel.cs` | `CongViecPageViewModel` | Model trang danh sách, bộ lọc, phân trang, permissions. |
| 5 | `QuanLyDuAn/QuanLyDuAn/ViewModels/CongViec/CongViecItemViewModel.cs` | `CongViecItemViewModel` | Dữ liệu từng dòng công việc, trạng thái, phân công, chi tiết. |
| 6 | `QuanLyDuAn/QuanLyDuAn/ViewModels/CongViec/CongViecDuAnOptionViewModel.cs` | `CongViecDuAnOptionViewModel` | Option dropdown dự án. |
| 7 | `QuanLyDuAn/QuanLyDuAn/Views/CongViec/Index.cshtml` | View `Index` | Trang chính, summary cards, import CSS, filter, table, pagination, export. |
| 8 | `QuanLyDuAn/QuanLyDuAn/Views/CongViec/_Filter.cshtml` | Partial `_Filter` | Form GET lọc dữ liệu, nút `Lọc dữ liệu`, nút `Làm mới`. |
| 9 | `QuanLyDuAn/QuanLyDuAn/Views/CongViec/_Table.cshtml` | Partial `_Table` | Bảng desktop, card mobile, action phân công, chi tiết, workflow. |
| 10 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/CongViec/index.css` | CSS module Công việc | Layout, filter, buttons, table, responsive của trang Công việc. |
| 11 | `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml` | Layout chung | Import CSS chung, sidebar route Công việc, render section Styles. |
| 12 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/site.css` | CSS chung | Font, table action buttons, layout responsive chung. |
| 13 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/ui.css` | CSS chung UI | `.btn-filter-apply`, `.btn-filter-reset`, `.btn-action`. |
| 14 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/export-dropdown.css` | CSS export | Nút và menu xuất file. |
| 15 | `QuanLyDuAn/QuanLyDuAn/wwwroot/js/site.js` | JS chung | Confirm cho POST; không can thiệp form GET filter. |
| 16 | `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Pagination.cshtml` | Partial `_Pagination` | Giữ query string khi phân trang và đổi page size. |
| 17 | `QuanLyDuAn/QuanLyDuAn/Views/Shared/_ExportDropdown.cshtml` | Partial `_ExportDropdown` | Link xuất Excel/PDF/CSV với route values filter. |
| 18 | `QuanLyDuAn/QuanLyDuAn/ViewModels/Common/PaginationViewModel.cs` | `PaginationViewModel` | Chuẩn hóa page number/page size, tính `Skip`. |
| 19 | `QuanLyDuAn/QuanLyDuAn/ViewModels/Common/PagedResultViewModel.cs` | `PagedResultViewModel<T>` | Kiểu dùng chung; module Công việc không dùng trực tiếp. |
| 20 | `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs` | `Permissions` | Permission `CongViec.Xem`, `ThongKe.XuatFile`, workflow liên quan. |
| 21 | `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs` | `TrangThai` | Hằng trạng thái, chuẩn hóa, display, biến thể trạng thái. |
| 22 | `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs` | `QuanLyDuAnDbContext` | DbSet và mapping các bảng Công việc, Dự án, Danh mục, Mức độ, Phân công. |
| 23 | `QuanLyDuAn/QuanLyDuAn/Models/Entities/CongViec.cs` | `CongViec` | Entity bảng `CONG_VIEC`. |
| 24 | `QuanLyDuAn/QuanLyDuAn/Models/Entities/DanhMucCongViec.cs` | `DanhMucCongViec` | Entity bảng `DANH_MUC_CONG_VIEC`. |
| 25 | `QuanLyDuAn/QuanLyDuAn/Models/Entities/DuAn.cs` | `DuAn` | Entity bảng `DU_AN`. |
| 26 | `QuanLyDuAn/QuanLyDuAn/Models/Entities/MucDoUuTien.cs` | `MucDoUuTien` | Entity bảng `MUC_DO_UU_TIEN`. |
| 27 | `QuanLyDuAn/QuanLyDuAn/Models/Entities/PhanCongCongViec.cs` | `PhanCongCongViec` | Entity bảng `PHAN_CONG_CONG_VIEC`. |
| 28 | `QuanLyDuAn/QuanLyDuAn/Models/Entities/CtCongViec.cs` | `CtCongViec` | Entity bảng `CT_CONG_VIEC`. |
| 29 | `QuanLyDuAn/QuanLyDuAn/Models/Entities/PhanCongCtCongViec.cs` | `PhanCongCtCongViec` | Entity bảng `PHAN_CONG_CT_CONG_VIEC`. |
| 30 | `QuanLyDuAn/QuanLyDuAn/Models/Entities/ChiPhi.cs` | `ChiPhi` | Chi phí đã chi theo công việc. |
| 31 | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanCongCongViecService.cs` | `PhanCongCongViecService` | Luồng liên kết từ nút Phân công công việc. |
| 32 | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanCongChiTietCongViecService.cs` | `PhanCongChiTietCongViecService` | Luồng phân công chi tiết liên quan. |
| 33 | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChiTietCongViecService.cs` | `ChiTietCongViecService` | Luồng liên kết từ nút Chi tiết công việc. |
| 34 | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TienDoCongViecService.cs` | `TienDoCongViecService` | Query tiến độ liên quan công việc, đối chiếu nguy cơ tương tự. |

## 2. Kiến trúc chức năng Công việc hiện tại

Luồng chính:

`Request /CongViec`
→ `CongViecController.Index(...)`
→ `ICongViecService.GetPageAsync(...)`
→ `CongViecService.GetPageAsync(...)`
→ LINQ query trên `_context.CongViec`, `_context.DanhMucCongViec`, `_context.DuAn`, `_context.MucDoUuTien`, `_context.ChiPhi`
→ filter/data scope/search/sort/count/paging
→ `CongViecPageViewModel`
→ `Views/CongViec/Index.cshtml`
→ `_Filter`, `_Table`, `_Pagination`, `_ExportDropdown`
→ CSS từ layout chung và `wwwroot/css/CongViec/index.css`.

Bằng chứng:

- File: `Controllers/CongViecController.cs`
- Class/View: `CongViecController`
- Method/action/selector: `Index(...)`
- Code hoặc biểu thức liên quan: dòng 31-47 nhận `locMaDuAn`, `locTrangThai`, `tuKhoa`, `tuNgay`, `denNgay`, `locTheoNgay`, `pageNumber`, `pageSize`, kiểm tra `Permissions.CongViec.Xem`, rồi gọi `_service.GetPageAsync(...)`.
- Nhận xét: Controller không tự query EF; lỗi LINQ phát sinh trong service.

## 3. Route và action của chức năng

| Route | HTTP method | Controller action | Service method | View/Output |
|-------|-------------|-------------------|----------------|-------------|
| `/CongViec` hoặc `/CongViec/Index` | GET | `CongViecController.Index(...)` | `GetPageAsync(locMaDuAn, locTrangThai, tuKhoa, tuNgay, denNgay, locTheoNgay, pageNumber, pageSize)` | `Views/CongViec/Index.cshtml` |
| `/CongViec/XacNhanHoanThanh` | POST | `XacNhanHoanThanh(...)` | `XacNhanHoanThanhCongViecAsync(maCongViec)` | Redirect về `Index` với filter cũ |
| `/CongViec/MoLai` | POST | `MoLai(...)` | `MoLaiCongViecAsync(maCongViec, lyDo)` | Redirect về `Index` với filter cũ |
| `/CongViec/XuatFile` | GET | `XuatFile(...)` | `GetPageAsync(..., paginate: false)` | File export qua `IExportFileService` |

## 4. Permission và phạm vi dữ liệu

- Permission xem danh sách: `Permissions.CongViec.Xem` trong `Permissions.cs` dòng 85-88; `CongViecController.Index` kiểm tra ở dòng 42-43.
- Permission xuất file: `Permissions.ThongKe.XuatFile` trong `Permissions.cs` dòng 10-13; `CongViecController.XuatFile` kiểm tra ở dòng 113-114.
- Workflow xác nhận/mở lại: `CoQuyenXuLyWorkflowCongViecAsync` yêu cầu `CongViec.Xem` và một trong `TienDo.Duyet` hoặc `DuyetDeXuatCongViec.Duyet` tại `CongViecController.cs` dòng 152-158.
- Role được đọc trong `CongViecService.GetCurrentUserRoleFlagsAsync`: role name normalize uppercase, trả về `ADMIN`, `MANAGER`, `EMPLOYEE` tại dòng 525-545.
- `allowedProjectIds` được lấy ở `CongViecService.GetAccessibleProjectIdsAsync` dòng 449-485:
  - Admin hoặc user không phải Manager/Employee: tất cả dự án chưa xóa (`DuAn.IsDeleted != true`).
  - Manager: các dự án `DuAn.MaNguoiDung == currentUserId`.
  - Employee: các dự án có dòng `NhanVienDuAn.MaNguoiDung == currentUserId`.
- Query danh sách áp dụng `allowedProjectIds.Contains(x.MaDuAn)` tại dòng 71-74, không được bỏ điều kiện này để né lỗi.
- Riêng Employee không phải Manager/Admin bị siết thêm ở dòng 115-139: chỉ thấy công việc thuộc dự án mà user là Leader hoặc công việc được phân công trong `_context.PhanCongCongViec.Any(...)`.

## 5. Entity và quan hệ dữ liệu

Quan hệ AS-IS được map trong `QuanLyDuAnDbContext`, không khai báo navigation property trong các entity đã đọc.

`DU_AN`
→ `DANH_MUC_CONG_VIEC`
→ `CONG_VIEC`
→ `CT_CONG_VIEC`

Các bảng liên quan: `MUC_DO_UU_TIEN`, `CHI_PHI`, `PHAN_CONG_CONG_VIEC`, `PHAN_CONG_CT_CONG_VIEC`.

| Bảng/entity | Khóa chính | Khóa ngoại/mapping | Navigation property trong entity |
|-------------|------------|--------------------|----------------------------------|
| `DU_AN` / `DuAn` | `MaDuAn` | `MaNguoiDung`, `MaLoaiDuAn`; mapping dòng 460-479 | Không có property navigation trong `DuAn.cs`. |
| `DANH_MUC_CONG_VIEC` / `DanhMucCongViec` | `MaDanhMucCV` | `MaDuAn` → `DU_AN`, mapping dòng 365-374 | Không có navigation trong `DanhMucCongViec.cs`. |
| `CONG_VIEC` / `CongViec` | `MaCongViec` | `MaDanhMucCV` → `DANH_MUC_CONG_VIEC`, `MaMucDo` → `MUC_DO_UU_TIEN`, `MaDeXuatCV` → `DE_XUAT_CONG_VIEC`; mapping dòng 245-267 | Không có navigation trong `CongViec.cs`. |
| `CT_CONG_VIEC` / `CtCongViec` | `MaChiTietCV` | `MaCongViec` → `CONG_VIEC`, mapping dòng 268-280 | Không có navigation trong `CtCongViec.cs`. |
| `MUC_DO_UU_TIEN` / `MucDoUuTien` | `MaMucDo` | Được join từ `CONG_VIEC.MaMucDo`; mapping dòng 532-538 | Không có navigation trong `MucDoUuTien.cs`. |
| `CHI_PHI` / `ChiPhi` | `MaChiPhi` | `MaCongViec` → `CONG_VIEC`, mapping dòng 219-237 | Không có navigation trong `ChiPhi.cs`. |
| `PHAN_CONG_CONG_VIEC` / `PhanCongCongViec` | `{ MaNguoiDung, MaCongViec }` | `MaCongViec` → `CONG_VIEC`, mapping dòng 725-737 | Không có navigation trong `PhanCongCongViec.cs`. |
| `PHAN_CONG_CT_CONG_VIEC` / `PhanCongCtCongViec` | `{ MaNguoiDung, MaChiTietCV }` | `MaChiTietCV` → `CT_CONG_VIEC`, mapping dòng 738-749 | Không có navigation trong `PhanCongCtCongViec.cs`. |

## 6. ViewModel của màn hình Công việc

| ViewModel | Thuộc tính | Nguồn dữ liệu | Mục đích hiển thị/lọc |
|-----------|------------|---------------|-----------------------|
| `CongViecPageViewModel` | `DanhSach` | `CongViecService.GetPageAsync` | Danh sách dòng công việc. |
| `CongViecPageViewModel` | `Pagination` | `PaginationViewModel.Create(...)` | Phân trang. |
| `CongViecPageViewModel` | `DanhSachDuAn` | `GetProjectOptionsAsync(allowedProjectIds)` | Dropdown dự án. |
| `CongViecPageViewModel` | `LocMaDuAn`, `LocTrangThai`, `TuKhoa`, `TuNgay`, `DenNgay`, `LocTheoNgay` | Request query string và chuẩn hóa trong service | Giữ trạng thái filter. |
| `CongViecPageViewModel` | `Permissions` | `CongViecController.Index`, `_phanQuyenService.GetGrantedPermissionNamesAsync(User)` | Điều khiển nút/khả năng thao tác trong view. |
| `CongViecItemViewModel` | `MaCongViec`, `MaDuAn`, `MaDanhMucCV`, `MaMucDo` | Join `CONG_VIEC`, `DANH_MUC_CONG_VIEC`, `DU_AN`, `MUC_DO_UU_TIEN` | Khóa và route action. |
| `CongViecItemViewModel` | `TenDuAn`, `TenDanhMucCV`, `TenMucDo`, `TenCongViec`, `MoTaCongViec` | Projection trong query dòng 56-62 | Hiển thị và tìm kiếm. |
| `CongViecItemViewModel` | `NgayBatDauCongViec`, `NgayKetThucCVDuKien`, `NgayKetThucCVThucTe`, `NgayTaoCongViec` | Entity `CongViec` | Lọc ngày và cột mốc thời gian. |
| `CongViecItemViewModel` | `ChiPhiDaChi` | `cpGroup.Sum(x => x.SoTienDaChi ?? 0m)` | Cột mức độ/chi phí. |
| `CongViecItemViewModel` | `TrangThaiCongViec`, `TrangThaiHienThi`, `CssTrangThai`, `ThongDiepWorkflow` | `GanThongTinWorkflowUi` | Badge trạng thái và thông điệp workflow. |
| `CongViecItemViewModel` | `SoNguoiDuocPhanCong`, `DaPhanCong` | `GanSoNguoiDuocPhanCongAsync` | Cột phân công. |
| `CongViecItemViewModel` | `SoLuongChiTietCongViec`, `DaCoChiTietCongViec` | `GanSoLuongChiTietCongViecAsync` | Cột chi tiết công việc. |
| `CongViecItemViewModel` | `CoThePhanCongCongViec`, `CoTheXacNhanHoanThanh`, `CoTheMoLai` | Các method gán quyền sau materialization | Điều khiển nút thao tác. |
| `CongViecDuAnOptionViewModel` | `MaDuAn`, `TenDuAn` | `GetProjectOptionsAsync` | Dropdown `locMaDuAn`. |

## 7. Luồng tải danh sách và phân trang

Thứ tự thực thi trong `CongViecService.GetPageAsync`:

1. Lấy user hiện tại, role flags, `allowedProjectIds`, project options, chuẩn hóa ngày và `locTheoNgay` tại dòng 38-43.
2. Tạo query ban đầu từ `_context.CongViec` join `DanhMucCongViec`, `DuAn`, `MucDoUuTien`, left group join `ChiPhi` chưa xóa tại dòng 45-69.
3. Áp dụng soft-delete: `cv.IsDeleted != true && dm.IsDeleted != true && da.IsDeleted != true` dòng 51; `ChiPhi` filter `x.IsDeleted != true` dòng 50.
4. Project sang `CongViecItemViewModel` khi query vẫn là `IQueryable`, gồm fallback chuỗi nội suy cho `TenDanhMucCV`, `TenDuAn`, `TenMucDo` tại dòng 56, 58, 60.
5. Áp dụng data scope `allowedProjectIds.Contains(x.MaDuAn)` dòng 71-74.
6. Áp dụng filter dự án `x.MaDuAn == locMaDuAn.Value` dòng 76-79.
7. Nếu có `tuKhoa`, chuẩn hóa `var keyword = tuKhoa.Trim().ToLower()` và áp dụng search trên `TenCongViec`, `MoTaCongViec`, `TenDanhMucCV`, `TenDuAn` dòng 81-89.
8. Nếu có trạng thái, dùng `TrangThai.GetCommonStatusVariants(locTrangThai)` rồi `filterValues.Contains(x.TrangThaiCongViec)` dòng 91-98.
9. Nếu có ngày, lọc `NgayKetThucCVDuKien` hoặc `NgayTaoCongViec` tùy `locTheoNgayResolved` dòng 100-113.
10. Nếu Employee không phải Manager/Admin, lấy vai trò dự án rồi tiếp tục lọc Leader hoặc phân công trực tiếp dòng 115-139.
11. `CountAsync()` trên query đã filter dòng 141.
12. Tạo `PaginationViewModel.Create(pageNumber, pageSize, totalItems)` dòng 142.
13. Sort `OrderByDescending(x => x.NgayTaoCongViec).ThenByDescending(x => x.MaCongViec)` dòng 144-146.
14. Nếu `paginate == true`, áp dụng `Skip(pagination.Skip).Take(pagination.PageSize)` dòng 148-153.
15. Materialize bằng `ToListAsync()` dòng 155.
16. Sau materialization mới gán số người phân công, số chi tiết, quyền thao tác, thông tin workflow UI dòng 157-161.

Nhận xét: lỗi EF Core xảy ra trước materialization, tại bước `CountAsync()` hoặc `ToListAsync()` tùy request, vì `Where` keyword vẫn thuộc `IQueryable`.

## 8. Bộ lọc chức năng Công việc

| Tham số | Tên field/query | Kiểu dữ liệu | Điều kiện áp dụng | Cách giữ qua phân trang |
|---------|-----------------|--------------|-------------------|--------------------------|
| Từ khóa | `tuKhoa` | `string?` | Chỉ áp dụng khi `!string.IsNullOrWhiteSpace(tuKhoa)`; service `Trim().ToLower()` | `_Pagination.cshtml` copy mọi query string trừ `pageNumber`, `pageSize`; export route giữ `tuKhoa`. |
| Dự án | `locMaDuAn` | `int?` | Khi có giá trị: `x.MaDuAn == locMaDuAn.Value` | Query string được giữ trong `_Pagination.cshtml`; form có select giữ `Model.LocMaDuAn`. |
| Trạng thái | `locTrangThai` | `string?` | Dùng `TrangThai.GetCommonStatusVariants` rồi `Contains` | Query string được giữ; select dùng `TrangThai.EqualsValue`. |
| Loại ngày | `locTheoNgay` | `string?` | Rỗng thì `"NgayTao"`; `"HanCongViec"` lọc hạn công việc, còn lại lọc ngày tạo | Query string được giữ; select giữ `Model.LocTheoNgay`. |
| Từ ngày | `tuNgay` | `DateTime?` | Chuẩn hóa `.Date`; nếu từ > đến thì swap | Query string được giữ; input date format `yyyy-MM-dd`. |
| Đến ngày | `denNgay` | `DateTime?` | Chuẩn hóa `.Date`, so sánh `< denNgay + 1 ngày` | Query string được giữ; input date format `yyyy-MM-dd`. |
| Page | `pageNumber` | `int` | Controller default `1`; `PaginationViewModel.NormalizePageNumber` chống nhỏ hơn 1 | Link phân trang tự set `pageNumber`; đổi page size reset về 1. |
| Page size | `pageSize` | `int` | Controller default `20`; allowed `{10,20,50,100}` | `_Pagination.cshtml` select `pageSize`, giữ các filter khác. |

## 9. Phân tích lỗi EF Core không dịch được LINQ

### 9.1 Cách tái hiện lỗi

Request có khả năng kích hoạt lỗi: `GET /CongViec?tuKhoa=<giá trị không rỗng>` hoặc request có thêm filter khác nhưng `tuKhoa` vẫn không rỗng.

- Truy cập không có keyword: không đi vào block dòng 81-89, nên không dùng `x.TenDanhMucCV.ToLower().Contains(keyword)` và `x.TenDuAn.ToLower().Contains(keyword)`. Theo source, các filter dự án/trạng thái/ngày không tự gọi fallback string interpolation trong `Where` keyword.
- Chỉ khi nhập từ khóa: `tuKhoa.Trim().ToLower()` dòng 83 rồi query `Where` dòng 84-88. Lúc này các property `TenDanhMucCV` và `TenDuAn` là kết quả projection có fallback chuỗi nội suy dòng 56, 58.
- Request xuất file cũng gọi `GetPageAsync(..., paginate: false)` dòng 116 của controller. Nếu export kèm `tuKhoa`, query vẫn đi qua cùng block search và có nguy cơ lỗi tương tự.

### 9.2 File và method gây lỗi

- File: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs`
- Class/View: `CongViecService`
- Method/action/selector: `GetPageAsync(...)`
- Code hoặc biểu thức liên quan: projection dòng 52-69 và filter keyword dòng 81-89.
- Nhận xét: `query` vẫn là `IQueryable<CongViecItemViewModel>`; chưa có `ToListAsync()` trước khi `Where` keyword được thêm vào.

### 9.3 Biểu thức LINQ thực tế

Projection liên quan:

```csharp
TenDanhMucCV = dm.TenDanhMucCV ?? $"Danh mục {dm.MaDanhMucCV}",
TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}",
TenMucDo = md.TenMucDo ?? $"Mức độ {md.MaMucDo}",
TenCongViec = cv.TenCongViec ?? string.Empty,
MoTaCongViec = cv.MoTaCongViec ?? string.Empty,
```

Search keyword:

```csharp
var keyword = tuKhoa.Trim().ToLower();
query = query.Where(x =>
    x.TenCongViec.ToLower().Contains(keyword) ||
    x.MoTaCongViec.ToLower().Contains(keyword) ||
    x.TenDanhMucCV.ToLower().Contains(keyword) ||
    x.TenDuAn.ToLower().Contains(keyword));
```

### 9.4 Thành phần được EF Core dịch được

Trong biểu thức hiện tại, các phần thường có thể dịch sang SQL Server:

- `Contains(keyword)` trên string.
- `ToLower()` trên cột/biểu thức string translatable.
- `?? string.Empty` cho `TenCongViec` và `MoTaCongViec`.
- So sánh số nguyên `x.MaDuAn == locMaDuAn.Value`.
- `allowedProjectIds.Contains(x.MaDuAn)` với danh sách int.
- `filterValues.Contains(x.TrangThaiCongViec)` với danh sách string.
- Lọc ngày với `HasValue`, `.Value >=`, `.Value <`.
- `CountAsync`, `OrderByDescending`, `Skip`, `Take` nếu biểu thức query trước đó dịch được.

### 9.5 Thành phần không dịch được

Thành phần gây lỗi là fallback chuỗi nội suy nằm trong projection nhưng bị dùng lại trong `Where` keyword:

- `dm.TenDanhMucCV ?? $"Danh mục {dm.MaDanhMucCV}"`
- `da.TenDuAn ?? $"Dự án {da.MaDuAn}"`

Khi compiler hạ chuỗi nội suy có số nguyên thành `string.Format(...)`, EF Core provider hiện tại không dịch được `string.Format` bên trong SQL query. Thông báo lỗi khớp với hai fallback này: `string.Format("Danh mục {0}", MaDanhMucCV)` và `string.Format("Dự án {0}", MaDuAn)`.

`TenMucDo = md.TenMucDo ?? $"Mức độ {md.MaMucDo}"` cũng là fallback cùng kiểu, nhưng hiện không nằm trong `Where` keyword. Nó vẫn có thể xuất hiện trong projection cuối nếu EF cố dịch toàn projection, nhưng thông báo lỗi hiện tại nêu rõ lỗi trong `Where` với `TenDanhMucCV` và `TenDuAn`.

### 9.6 Nguyên nhân xuất hiện string.Format

Trong C#, biểu thức `$"Danh mục {dm.MaDanhMucCV}"` và `$"Dự án {da.MaDuAn}"` có thể được biên dịch thành `string.Format("Danh mục {0}", ...)` khi nằm trong expression tree của LINQ. Vì projection chưa materialize, EF Core nhận expression tree chứa `string.Format` và cố dịch sang SQL. Provider không hỗ trợ dịch `string.Format`, nên ném `InvalidOperationException`.

### 9.7 Tác động của lỗi

- Danh sách `/CongViec` có `tuKhoa` không chạy được.
- Vì `totalItems = await query.CountAsync()` nằm sau filter keyword, lỗi có thể xuất hiện ngay ở bước đếm tổng số bản ghi, trước khi phân trang.
- Phân trang server-side bị chặn vì `PaginationViewModel` cần `totalItems`.
- Xuất file có `tuKhoa` có thể lỗi vì `XuatFile` dùng lại `GetPageAsync(..., paginate: false)`.
- Data scope không phải nguyên nhân lỗi và không nên bị bỏ: `allowedProjectIds.Contains(x.MaDuAn)` và điều kiện Employee/Leader/Phân công là phần bảo vệ phạm vi dữ liệu.

### 9.8 Các vị trí có nguy cơ tương tự

| Vị trí | Bằng chứng | Mức liên quan | Nhận xét |
|--------|------------|---------------|----------|
| `CongViecService.GetPageAsync` dòng 56, 58 + dòng 87, 88 | Fallback `$"Danh mục {id}"`, `$"Dự án {id}"` rồi search keyword trên property projected | Trực tiếp | Đây là nguồn lỗi hiện tại. |
| `CongViecService.GetPageAsync` dòng 60 | `TenMucDo = md.TenMucDo ?? $"Mức độ {md.MaMucDo}"` | Trung bình | Không nằm trong search hiện tại, nhưng vẫn là interpolation trong `IQueryable` projection. |
| `CongViecService.GetProjectOptionsAsync` dòng 497-503 | `OrderBy(x => x.TenDuAn)`, projection `TenDuAn = x.TenDuAn ?? $"Dự án {x.MaDuAn}"` | Thấp/trung bình | Không có `Where`/`OrderBy` trên fallback; projection có thể vẫn cần kiểm tra khi EF dịch. |
| `TienDoCongViecService.GetPageAsync` dòng 58-99 | Search dùng `(x.TenDuAn ?? string.Empty).ToLower().Contains(keyword)` trên raw nullable string, fallback hiển thị sau `ToListAsync` dòng 321-325 | Đối chiếu an toàn hơn | Không dùng `$"Dự án {id}"` trong search SQL. |
| `PhanCongCongViecService.GetPageAsync` dòng 52-62 | Projection `TenNhanVien = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}"` rồi `ToListAsync` | Có nguy cơ mẫu chung, không trực tiếp lỗi `/CongViec` | Không có search/order trên fallback trong cùng query. |
| `PhanCongChiTietCongViecService.GetPageAsync` dòng 45-55 | Projection `TenNhanVien = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}"` | Có nguy cơ mẫu chung, không trực tiếp lỗi `/CongViec` | Không có search/order trên fallback trong cùng query. |
| `ChiTietCongViecService.GetPageAsync` dòng 35-71 | Search không có; projection dùng `?? string.Empty` | Thấp | Không có `string.Format` fallback trong query chính. |

### 9.9 Các hướng sửa có thể áp dụng ở bước sau

| Hướng xử lý | Ưu điểm | Rủi ro | Tác động phân trang | Mức độ phù hợp |
|-------------|---------|--------|----------------------|----------------|
| Chỉ search SQL trên trường thật: `cv.TenCongViec`, `cv.MoTaCongViec`, `dm.TenDanhMucCV`, `da.TenDuAn`, dùng fallback sau materialization | Loại bỏ `string.Format` khỏi `Where`; giữ server-side filtering/count/paging | Nếu người dùng kỳ vọng search `"Dự án 5"` khi `TenDuAn` null thì kết quả có thể thay đổi | Giữ nguyên `CountAsync`, `Skip`, `Take` trên SQL | Rất phù hợp |
| Tách điều kiện tên và mã: search `TenDuAn`/`TenDanhMucCV` dạng string, và nếu keyword parse được số thì so `MaDuAn`, `MaDanhMucCV` | Vẫn tìm được fallback theo mã mà không tạo chuỗi | Cần quy định rõ keyword nào match mã; tránh match quá rộng | Giữ server-side paging | Phù hợp nếu muốn giữ khả năng tìm theo mã fallback |
| Dùng biểu thức SQL-translatable thay cho interpolation, ví dụ so sánh trực tiếp trường và mã | Không kéo dữ liệu về RAM | Cần viết cẩn thận để không đổi kết quả tìm kiếm | Giữ server-side paging | Phù hợp |
| Chỉ tạo chuỗi hiển thị sau `ToListAsync()` và sau phân trang | Loại bỏ fallback khỏi expression tree | Search trên fallback hiển thị không còn trong SQL nếu không thêm điều kiện mã riêng | Giữ server-side paging nếu filter vẫn ở SQL | Phù hợp |
| Dùng client-side evaluation toàn bộ dataset trước khi filter | Dễ né lỗi dịch SQL | Rủi ro lớn về hiệu năng, data scope, count, paging; không phù hợp yêu cầu | Phá server-side count/paging nếu làm trước filter | Không khuyến nghị |

### 9.10 Hướng được khuyến nghị

Khuyến nghị cho bước sửa sau: giữ toàn bộ filter/data scope/pagination trên SQL, đưa fallback chuỗi hiển thị ra khỏi phần `Where` keyword. Cụ thể nên search trên cột thật (`cv.TenCongViec`, `cv.MoTaCongViec`, `dm.TenDanhMucCV`, `da.TenDuAn`) và nếu cần giữ hành vi tìm theo fallback `"Dự án {id}"`, `"Danh mục {id}"` thì tách điều kiện so mã dựa trên keyword parse được. Không dùng `AsEnumerable()`/`ToListAsync()` trước filter toàn bộ dữ liệu.

## 10. Phân tích giao diện bộ lọc

`Views/CongViec/_Filter.cshtml` định nghĩa form:

- File: `Views/CongViec/_Filter.cshtml`
- Class/View: Partial `_Filter`
- Method/action/selector: `<form asp-action="Index" method="get" class="workflow-filter">`
- Code hoặc biểu thức liên quan: dòng 4-83.
- Nhận xét: form GET submit về action `Index`, tạo query string cho các field lọc. Không có JavaScript reset riêng.

Layout:

- `.workflow-filter` là grid trong CSS dòng 190-200.
- `.workflow-filter-row-main` có 3 cột desktop dòng 218-220.
- `.workflow-filter-row-time` có 4 cột desktop và `align-items: end` dòng 222-225.
- `.filter-actions` là flex, wrap, canh phải dòng 266-273.
- Tablet dưới `1199.98px`: `.workflow-filter-row-time` còn 2 cột, `.filter-actions` span 2 cột và canh trái dòng 596-612.
- Mobile dưới `767.98px`: các row về 1 cột, `.filter-actions` stretch full width, hai nút width 100% dòng 615-644.

## 11. Phân tích nút “Lọc dữ liệu”

- File: `Views/CongViec/_Filter.cshtml`
- Class/View: Partial `_Filter`
- Method/action/selector: `button.btn-filter-apply`
- Code hoặc biểu thức liên quan: dòng 74-77.
- Nhận xét: nút là `<button type="submit" class="btn-filter-apply">Lọc dữ liệu</button>`, submit form GET về `Index`.

CSS:

- `wwwroot/css/CongViec/index.css` dòng 275-289: dùng chung nền tảng với reset button: `min-height: 40px`, `flex: 1 1 8.5rem`, `border-radius: 10px`, `padding: 0.52rem 0.82rem`, `font-weight: 600`, `font-size: 0.86rem`, `display: inline-flex`, `align-items: center`, `gap: 0.36rem`.
- Dòng 291-300: `background: var(--cv-primary)`, `border-color: var(--cv-primary)`, `color: #fff`, hover `#2563eb`.
- `wwwroot/css/shared/ui.css` dòng 83-96 cũng định nghĩa `.btn-filter-apply` gradient chung. Vì layout import `shared/ui.css` trước section styles, và `CongViec/index.css` được import trong `@section Styles` sau layout dòng 84, rule module có cùng/nhỉnh hơn specificity và được load sau, nên module CSS đang ghi đè màu chính.

## 12. Phân tích nút “Làm mới”

### 12.1 HTML/Razor

- File: `Views/CongViec/_Filter.cshtml`
- Class/View: Partial `_Filter`
- Method/action/selector: `a.btn-filter-reset`
- Code hoặc biểu thức liên quan:

```html
<a asp-action="Index" class="btn-filter-reset">
    Làm mới
</a>
```

- Nhận xét: nút là thẻ `<a>`, không phải `<button>` hoặc `<input>`.

### 12.2 Route và hành vi reset

`asp-action="Index"` không truyền route values, nên link điều hướng về action `CongViecController.Index` không giữ query string lọc hiện tại. Hành vi là điều hướng về `/CongViec/Index` hoặc route tương đương `/CongViec`, xóa filter qua URL mới, không reset form bằng JavaScript và không submit giá trị mặc định.

### 12.3 CSS đang áp dụng

CSS trực tiếp:

- `wwwroot/css/CongViec/index.css` dòng 275-289: `.cong-viec-page .btn-filter-apply, .cong-viec-page .btn-filter-reset` đặt kích thước, padding, font, display inline-flex, `align-items`, gap, transition. Thiếu `justify-content: center` ở desktop/tablet.
- `wwwroot/css/CongViec/index.css` dòng 302-311: `.btn-filter-reset` nền `var(--cv-neutral-soft)`, border `#e2e8f0`, màu `#334155`, hover `#eef2f7`, border hover `#cfd8e3`.
- `wwwroot/css/CongViec/index.css` dòng 640-644: mobile width 100% và `justify-content: center`.

CSS chung có thể ảnh hưởng:

- `wwwroot/css/shared/ui.css` dòng 98-103: `.btn-subtle, .btn-filter-reset` đặt border `#cdd8e8`, background `#f6f9ff`, color `#436082`. Bị module CSS ghi đè do `.cong-viec-page .btn-filter-reset` specificity cao hơn và load sau.
- Bootstrap `.btn` không áp dụng vì link không có class `btn`.
- `site.css` không có selector trực tiếp cho `.btn-filter-reset`; chỉ áp dụng font chung nếu selector thuộc `input, textarea, select, button, .btn...`, nhưng link không nằm trong danh sách đó. Font vẫn kế thừa từ body.

### 12.4 Nguyên nhân hiển thị chưa đồng bộ

Theo source hiện tại, các nguyên nhân có bằng chứng:

- Nút “Lọc dữ liệu” là `<button type="submit">`, còn “Làm mới” là `<a>`. Hai element khác loại nên default CSS/line-height/focus behavior có thể khác nếu chưa reset đủ.
- CSS chung của hai nút không đặt `justify-content: center` ở desktop/tablet; chỉ mobile mới đặt. Với link “Làm mới”, text có thể lệch cảm giác so với submit button trong flex item.
- `.btn-filter-reset` không đặt `text-decoration: none`; link `<a>` có thể nhận underline từ user agent hoặc rule hover/focus khác nếu trình duyệt/CSS chung áp dụng. Source hiện tại chỉ set `text-decoration` cho `.hero-action` và `.btn-action`, không set cho `.btn-filter-reset`.
- `.btn-filter-reset:hover` không đặt lại `color`, `text-decoration`, `outline` hoặc `box-shadow`; trạng thái hover/focus chưa đầy đủ so với nút submit.
- Hai nút dùng màu khác: apply là primary xanh đặc, reset là neutral nhạt. Đây có thể là chủ ý phân cấp hành động, nhưng nếu yêu cầu “đồng bộ” theo chiều cao/khoảng cách/trạng thái thì cần chỉnh trong CSS module.

### 12.5 Responsive

- Desktop: `.workflow-filter-row-time` 4 cột, action nằm cột cuối, `.filter-actions` canh phải; hai nút `flex: 1 1 8.5rem` nên có thể chia hàng nếu cột hẹp.
- Dưới `1199.98px`: action span 2 cột và canh trái, giúp có thêm chiều ngang.
- Dưới `767.98px`: action full width, hai nút width 100%, `justify-content: center`. Responsive mobile đã có xử lý rõ hơn desktop/tablet.

### 12.6 Phạm vi sửa an toàn ở bước sau

Nên sửa trong phạm vi `Views/CongViec/_Filter.cshtml` và/hoặc `wwwroot/css/CongViec/index.css`. Ưu tiên CSS module có scope `.cong-viec-page` để tránh ảnh hưởng màn hình khác. Chưa cần sửa CSS chung `shared/ui.css` nếu chỉ muốn đồng bộ nút của màn hình Công việc. Không cần đổi route `/CongViec`, workflow, permission, service hoặc DB để xử lý giao diện này.

## 13. CSS và JavaScript liên quan

| File | Selector/function | Thành phần tác động | Ghi chú |
|------|-------------------|---------------------|--------|
| `Views/CongViec/Index.cshtml` | `@section Styles` | Import `~/css/CongViec/index.css`, `~/css/shared/export-dropdown.css` | Module CSS load sau CSS chung trong layout. |
| `Views/Shared/_Layout.cshtml` | CSS links dòng 77-84 | Bootstrap, `site.css`, `shared/ui.css`, scoped CSS, layout CSS, then page Styles | Thứ tự import quyết định ghi đè. |
| `wwwroot/css/CongViec/index.css` | `.workflow-filter`, `.workflow-filter-row-*`, `.filter-actions` | Layout filter | Grid + flex. |
| `wwwroot/css/CongViec/index.css` | `.btn-filter-apply`, `.btn-filter-reset` | Hai nút filter | Selector chính của nút “Làm mới”. |
| `wwwroot/css/CongViec/index.css` | media max `1199.98px`, `767.98px` | Responsive filter và buttons | Mobile đặt width 100%, center. |
| `wwwroot/css/shared/ui.css` | `.btn-filter-apply`, `.btn-filter-reset` | Style chung cùng class | Bị module CSS ghi đè phần lớn trên trang này. |
| `wwwroot/css/site.css` | `.app-data-table .table-actions .btn` | Nút trong bảng, không phải nút filter | Ảnh hưởng các action có class `.btn` trong `_Table`. |
| `wwwroot/css/shared/export-dropdown.css` | `.export-main-btn`, `.export-dropdown-menu` | Nút xuất file | Import riêng trong `Index`. |
| `wwwroot/js/site.js` | submit listener cho POST | Confirm cho form POST | Bỏ qua form GET filter vì method khác `post`. |

## 14. Thống kê đầu trang

`Views/CongViec/Index.cshtml` dòng 17-21 tính:

- `tongCongViec = Model.DanhSach.Count`
- `dangThucHien = Model.DanhSach.Count(...)`
- `choXacNhan = Model.DanhSach.Count(...)`
- `hoanThanh = Model.DanhSach.Count(...)`
- `biCan = Model.DanhSach.Count(...)`

Nhận xét: số liệu tính trên `Model.DanhSach`, tức danh sách đã materialize sau phân trang. Vì `DanhSach` được lấy sau `Skip/Take` khi `paginate == true`, summary cards đang phản ánh trang hiện tại sau filter, không phải toàn bộ phạm vi người dùng hay toàn bộ kết quả sau filter. Đây là rủi ro hiển thị nếu người dùng kỳ vọng thống kê toàn bộ danh sách.

## 15. Cột dữ liệu trong bảng Công việc

| Cột giao diện | ViewModel property | Nguồn dữ liệu | Cách tính/hiển thị |
|---------------|--------------------|---------------|---------------------|
| Dự án | `TenDuAn`, `TenDanhMucCV` | `da.TenDuAn`, `dm.TenDanhMucCV` trong projection | Desktop hiển thị project + category; mobile tách Dự án/Danh mục. |
| Công việc | `TenCongViec`, `MoTaCongViec` | `cv.TenCongViec`, `cv.MoTaCongViec` | Text chính và mô tả compact. |
| Mức độ/chi phí | `TenMucDo`, `ChiPhiDaChi` | `md.TenMucDo`, `cpGroup.Sum(...)` | Chi phí format `N0`. |
| Mốc thời gian | `NgayBatDauCongViec`, `NgayKetThucCVDuKien` | Entity `CongViec` | Format `dd/MM/yyyy`. |
| Trạng thái | `TrangThaiHienThi`, `CssTrangThai` | `GanThongTinWorkflowUi`, `TrangThai.ToDisplay`, `LayCssTrangThai` | Badge icon theo `GetStatusIcon`. |
| Phân công | `SoNguoiDuocPhanCong` | `GanSoNguoiDuocPhanCongAsync` | `"Chưa phân công"` hoặc `"Đã phân công · {n} người"`. |
| Chi tiết công việc | `SoLuongChiTietCongViec` | `GanSoLuongChiTietCongViecAsync` | `"Chưa có chi tiết"` hoặc `"{n} chi tiết"`. |
| Thao tác | `CoThePhanCongCongViec`, `CoTheXacNhanHoanThanh`, `CoTheMoLai`, `Permissions` | Service + ViewBag permissions | Link Phân công, Chi tiết, form Xác nhận hoàn thành, form Mở lại. |

## 16. Chức năng xuất file

- Route: `GET /CongViec/XuatFile`.
- Quyền: `Permissions.ThongKe.XuatFile` tại `CongViecController.cs` dòng 113-114.
- Bộ lọc giữ lại: `Index.cshtml` dòng 105-116 tạo `ExportDropdownViewModel.RouteValues` gồm `locMaDuAn`, `locTrangThai`, `tuKhoa`, `tuNgay`, `denNgay`, `locTheoNgay`.
- Query: `CongViecController.XuatFile` dòng 116 gọi lại `_service.GetPageAsync(..., paginate: false)`.
- Loại file: `_ExportDropdown.cshtml` tạo route `format=excel`, `format=pdf`, `format=csv`; controller parse bằng `_exportFileService.ParseFormat(format)`.
- Rủi ro: nếu export có `tuKhoa`, service dùng cùng search keyword và có thể phát sinh lỗi `string.Format` tương tự. `paginate: false` chỉ bỏ `Skip/Take`, không bỏ `Where` keyword.

## 17. Các rủi ro phát hiện thêm

| Nhóm | Bằng chứng | Rủi ro |
|------|------------|--------|
| Lỗi runtime | `CongViecService.GetPageAsync` dòng 56, 58, 84-88 | `string.Format` từ chuỗi nội suy trong `IQueryable` khi có keyword. |
| Hiệu năng | Không nên dùng client-side evaluation trước filter; query hiện có `CountAsync`, `Skip`, `Take` server-side | Nếu sửa bằng `ToListAsync` trước filter sẽ phá hiệu năng và phân trang. |
| Phân trang | `Index.cshtml` summary tính trên `Model.DanhSach.Count`; service áp dụng `Skip/Take` trước `ToListAsync` | Summary đầu trang là theo trang hiện tại, không phải toàn bộ kết quả. |
| Data scope | `allowedProjectIds.Contains(x.MaDuAn)` dòng 71-74 và Employee scope dòng 115-139 | Không được bỏ để né lỗi LINQ. |
| Null handling | Fallback hiển thị dùng interpolation trong projection trước materialization | Nên tạo fallback hiển thị sau khi query đã chạy hoặc dùng biểu thức translatable. |
| Responsive | `.filter-actions` desktop thiếu `justify-content: center` trong từng nút; mobile mới center | Nút link “Làm mới” có thể lệch cảm giác so với nút submit. |
| CSS conflict | `shared/ui.css` cũng định nghĩa `.btn-filter-reset`; module CSS ghi đè do specificity/load order | Nếu sửa CSS chung có thể ảnh hưởng module khác. |
| Encoding tiếng Việt | Source và tài liệu dùng tiếng Việt có dấu | Cần lưu `docs/congviec.md` UTF-8, tránh mojibake. |

## 18. Danh sách file dự kiến cần sửa ở bước tiếp theo

| File | Lý do có thể cần sửa | Mức độ bắt buộc | Không được ảnh hưởng |
|------|----------------------|-----------------|----------------------|
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs` | Sửa query search/fallback để EF dịch được, giữ server-side paging | Bắt buộc cho lỗi LINQ | Data scope, soft-delete, filter, sort, count, workflow. |
| `QuanLyDuAn/QuanLyDuAn/Views/CongViec/_Filter.cshtml` | Nếu cần đổi markup nút reset hoặc thêm class/icon riêng | Có thể cần | Route `/CongViec`, tên field filter, form GET. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/CongViec/index.css` | Đồng bộ nút “Làm mới” với “Lọc dữ liệu”, hover/focus/responsive | Có thể cần | CSS dùng chung và màn hình khác. |
| `docs/congviec.md` | Cập nhật kết quả triển khai sau khi sửa | Có thể cần | Nội dung AS-IS hiện tại cần tách rõ với TO-BE/đã sửa. |

## 19. Đề xuất kế hoạch sửa theo nhóm nhỏ

1. Nhóm 1: sửa truy vấn LINQ trong `CongViecService.GetPageAsync`, giữ query SQL-translatable, build.
2. Nhóm 2: kiểm tra tìm kiếm, lọc dự án/trạng thái/ngày, phân trang và xuất file khi có/không có `tuKhoa`.
3. Nhóm 3: sửa CSS nút “Làm mới” trong scope `.cong-viec-page`, ưu tiên `wwwroot/css/CongViec/index.css`.
4. Nhóm 4: kiểm tra responsive desktop/tablet/mobile của filter actions.
5. Nhóm 5: cập nhật tài liệu kết quả triển khai, ghi rõ file đã sửa và kiểm chứng.

## 20. Kết luận AS-IS

- Lỗi LINQ thực sự xuất phát từ `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs`, method `GetPageAsync(...)`.
- Điều kiện kích hoạt chính là request có `tuKhoa` không rỗng. Service chuẩn hóa bằng `tuKhoa.Trim().ToLower()` rồi tìm trên `x.TenDanhMucCV` và `x.TenDuAn` khi hai property này được project từ fallback chuỗi nội suy.
- `string.Format("Danh mục {0}", ...)` và `string.Format("Dự án {0}", ...)` xuất hiện vì compiler hạ `$"Danh mục {dm.MaDanhMucCV}"` và `$"Dự án {da.MaDuAn}"` trong expression tree của `IQueryable`.
- Lỗi có thể chặn `CountAsync()` trước phân trang, nên ảnh hưởng trực tiếp server-side pagination. Xuất file cũng có nguy cơ khi có `tuKhoa` vì dùng lại `GetPageAsync(..., paginate: false)`.
- Nút “Làm mới” hiện là thẻ `<a asp-action="Index" class="btn-filter-reset">`, điều hướng về action `Index` không giữ query string, không dùng JavaScript reset.
- CSS điều khiển chính của nút “Làm mới” nằm ở `wwwroot/css/CongViec/index.css` selector `.cong-viec-page .btn-filter-reset`; có CSS chung cùng class trong `wwwroot/css/shared/ui.css` nhưng module CSS đang ghi đè trên trang này.
- File có khả năng cần sửa ở bước sau: `CongViecService.cs` cho lỗi LINQ; `_Filter.cshtml` và/hoặc `wwwroot/css/CongViec/index.css` cho giao diện nút reset; tài liệu nếu cần ghi nhận kết quả.
- Đã đủ dữ liệu source hiện tại để viết prompt sửa lỗi chính xác mà không đoán file, không đoán kiến trúc và không thay đổi nghiệp vụ.

## 21. Kết quả chỉnh sửa

### 21.1. File đã sửa

| File | Nội dung đã sửa |
| ---- | --------------- |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs` | Bỏ fallback chuỗi nội suy khỏi projection còn thuộc `IQueryable`; chuyển fallback tên danh mục, dự án và mức độ ưu tiên sang xử lý sau `ToListAsync()`; giữ nguyên thứ tự filter, `CountAsync()`, `OrderBy`, `Skip/Take` và materialization. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/CongViec/index.css` | Đồng bộ style cho `.cong-viec-page .btn-filter-apply` và `.cong-viec-page .btn-filter-reset`: căn giữa, cùng chiều cao, bỏ gạch chân link, giữ màu hover, thêm focus-visible và giữ responsive trong scope trang Công việc. |
| `docs/congviec.md` | Bổ sung phần kết quả chỉnh sửa, build và kiểm thử sau triển khai; không xóa nội dung phân tích AS-IS. |

### 21.2. Cách sửa lỗi LINQ

- Query cũ gây lỗi vì `CongViecService.GetPageAsync(...)` project trực tiếp các giá trị fallback như `$"Danh mục {dm.MaDanhMucCV}"`, `$"Dự án {da.MaDuAn}"`, `$"Mức độ {md.MaMucDo}"` trong expression tree của `IQueryable`. Khi có `tuKhoa`, điều kiện `Where` tiếp tục tìm trên `x.TenDanhMucCV` và `x.TenDuAn`, làm EF Core phải dịch fallback chuỗi nội suy thành `string.Format(...)` và phát sinh lỗi `Translation of method 'string.Format' failed`.
- Query mới chỉ đưa giá trị cột thật hoặc chuỗi rỗng an toàn vào projection phía SQL:
  - `cv.TenCongViec`
  - `cv.MoTaCongViec`
  - `dm.TenDanhMucCV ?? string.Empty`
  - `da.TenDuAn ?? string.Empty`
  - `md.TenMucDo ?? string.Empty`
- Điều kiện tìm kiếm vẫn giữ đúng phạm vi cũ: tên công việc, mô tả công việc, tên danh mục công việc và tên dự án. Không thêm tìm kiếm theo mã số và không đổi logic nghiệp vụ.
- Fallback hiển thị được chuyển sang method `GanTenHienThiMacDinh(List<CongViecItemViewModel> danhSach)` và chỉ chạy sau:
  - filter;
  - `CountAsync()`;
  - `OrderByDescending(...).ThenByDescending(...)`;
  - `Skip(...).Take(...)` khi `paginate == true`;
  - `ToListAsync()`.
- Query mới được EF Core dịch sang SQL vì phần `Where` chỉ còn dùng các biểu thức thông thường trên cột hoặc chuỗi rỗng: toán tử `??`, `ToLower()` và `Contains()`. Không còn `string.Format`, helper C# tùy chỉnh hoặc chuỗi nội suy trong phần query phải dịch sang SQL.
- Xác nhận không dùng `AsEnumerable()`, `ToList()` hoặc `ToListAsync()` trước khi hoàn tất lọc, đếm và phân trang. `ToListAsync()` vẫn là bước materialization cuối của danh sách trang hiện tại hoặc danh sách xuất file khi `paginate == false`.
- `GetProjectOptionsAsync()` cũng được điều chỉnh cùng nguyên tắc: fallback `Dự án {MaDuAn}` được tạo sau khi danh sách dropdown đã materialize, tránh để chuỗi nội suy nằm trong projection EF Core.

### 21.3. Cách sửa nút Làm mới

- Không sửa markup Razor. Nút vẫn là:
  ```html
  <a asp-action="Index" class="btn-filter-reset">Làm mới</a>
  ```
- Hành vi reset giữ nguyên: điều hướng về action `Index` và không mang query string, do đó xóa toàn bộ bộ lọc hiện tại.
- CSS đã sửa trong scope `.cong-viec-page`, không sửa `shared/ui.css` và không dùng CSS inline.
- Selector được cập nhật:
  - `.cong-viec-page .btn-filter-apply, .cong-viec-page .btn-filter-reset`
  - `.cong-viec-page .btn-filter-apply:hover`
  - `.cong-viec-page .btn-filter-apply:focus-visible, .cong-viec-page .btn-filter-reset:focus-visible`
  - `.cong-viec-page .btn-filter-reset:hover`
- Hai nút được đồng bộ bằng `min-height: 40px`, `display: inline-flex`, `align-items: center`, `justify-content: center`, `box-sizing: border-box`, `text-align: center`, `text-decoration: none`, `line-height: 1.2` và `white-space: nowrap`.
- Trạng thái hover của nút reset giữ dạng secondary/outline, không hiện gạch chân và không đổi màu chữ ngoài ý muốn. Trạng thái `focus-visible` dùng outline và box-shadow nhẹ để giữ khả năng truy cập bằng bàn phím.
- Responsive hiện có vẫn giữ trong `index.css`; selector mobile tiếp tục nằm dưới `.cong-viec-page`, nên phạm vi ảnh hưởng chỉ trong màn Công việc.

### 21.4. Kết quả build

Lệnh đã chạy:

```powershell
dotnet build QuanLyDuAn\QuanLyDuAn\QuanLyDuAn.csproj --no-restore
```

Kết quả:

- Build thành công.
- Không có lỗi biên dịch.
- Có 2 warning sẵn có trong `FileTienDoCongViecService.cs` mã `CS1998` về async method không có `await`; các warning này không phát sinh từ thay đổi chức năng Công việc.

### 21.5. Kết quả kiểm thử

| Trường hợp | Kết quả | Ghi chú |
| ---------- | ------- | ------- |
| Build dự án | Đạt | `dotnet build ... --no-restore` trả về exit code 0. |
| Kiểm tra tĩnh lỗi `string.Format` | Đạt | Không còn fallback chuỗi nội suy trong projection `IQueryable` của `GetPageAsync(...)`; không dùng `string.Format` trong `CongViecService.cs`. |
| Kiểm tra không client-side evaluation trước phân trang | Đạt | Không thêm `AsEnumerable()`; `ToListAsync()` vẫn nằm sau `CountAsync()`, `OrderBy`, `Skip/Take`. |
| Tìm kiếm theo tên công việc, mô tả, tên danh mục, tên dự án | Đạt ở mức source/build | Biểu thức tìm kiếm vẫn giữ các trường cũ và chỉ dùng cột thật/chuỗi rỗng. Chưa chạy request thực tế vì ứng dụng không khởi động được trong môi trường hiện tại. |
| Tìm kiếm từ khóa không tồn tại, chữ hoa/thường, tiếng Việt có dấu | Đạt ở mức source/build | Logic `Trim().ToLower()` và `Contains()` được giữ nguyên. Chưa chạy request thực tế vì lỗi startup ngoài phạm vi thay đổi. |
| Lọc dự án, trạng thái, ngày tạo, hạn công việc, từ ngày, đến ngày | Đạt ở mức source/build | Các nhánh filter giữ nguyên trong `GetPageAsync(...)`. |
| Phân trang khi có filter/từ khóa | Đạt ở mức source/build | `CountAsync()` chạy sau filter và trước `Skip/Take`; query string phân trang không bị sửa. |
| Xuất file có/không có từ khóa | Đạt ở mức source/build | `XuatFile` vẫn dùng `GetPageAsync(..., paginate: false)`; search không còn chứa fallback `string.Format`. |
| Nút Làm mới reset query string | Đạt ở mức source | Markup `<a asp-action="Index" class="btn-filter-reset">` không đổi. |
| Giao diện nút Làm mới desktop/tablet/mobile | Đạt ở mức CSS/source | CSS đã scope theo `.cong-viec-page`, đồng bộ chiều cao/căn giữa/hover/focus. Chưa xác nhận bằng browser vì ứng dụng không khởi động được trong môi trường hiện tại. |
| Data scope Admin/Manager/Employee/Leader/được phân công | Đạt ở mức source/build | Không sửa các điều kiện `allowedProjectIds.Contains(...)`, `NhanVienDuAn`, Leader hoặc `PhanCongCongViec.Any(...)`. Chưa chạy kiểm thử tài khoản thực tế vì lỗi startup ngoài phạm vi thay đổi. |
| Khởi động ứng dụng để kiểm thử runtime | Chưa đạt do môi trường | `dotnet run --no-build --project QuanLyDuAn\QuanLyDuAn\QuanLyDuAn.csproj --urls http://127.0.0.1:5037` dừng ở startup seed vì không kết nối được SQL Server `LAPTOP-SI5JBDIU\SQLEXPRESS01/QuanLyDuAnAI1` và logger EventLog báo không có quyền mở source `.NET Runtime`. Lỗi xảy ra trước khi app lắng nghe request, không phải lỗi biên dịch của thay đổi này. |

### 21.6. Những nội dung giữ nguyên

- Không sửa database.
- Không tạo hoặc sửa migration.
- Không sửa route `/CongViec`.
- Không sửa tên controller action.
- Không sửa permission.
- Không nới lỏng hoặc chuyển data scope sang client-side.
- Không sửa workflow trạng thái công việc, xác nhận hoàn thành, mở lại, phân công hoặc chi tiết công việc.
- Không sửa entity, DbContext hoặc public contract của service interface.
- Không sửa CSS dùng chung `shared/ui.css` hoặc layout chung.
- Không thay đổi cơ chế phân trang server-side: filter trước, `CountAsync()` trước `Skip/Take`, materialization sau cùng.

## 22. Phân tích nhận biết Công việc đúng hạn và trễ hạn

Mục này bổ sung phân tích AS-IS theo source hiện tại, chỉ phân biệt dữ liệu thời hạn với trạng thái workflow. Không xem `Trễ hạn`, `Quá hạn`, `Hoàn thành trễ` là `TrangThaiCongViec` vì `Constants/TrangThai.cs` không triển khai các giá trị này như trạng thái Công việc trong danh sách `/CongViec`.

Nguyên tắc cần giữ: `TrangThaiCongViec` là trạng thái nghiệp vụ như `ChuaBatDau`, `DangThucHien`, `BiCanCan`, `ChoXacNhanHoanThanh`, `HoanThanh`, `TamDung`, `DaHuy`. Tình trạng thời hạn là lớp thông tin riêng, ví dụ `Đang thực hiện + Trễ 5 ngày`, `Hoàn thành + Hoàn thành trễ 2 ngày`, `Hoàn thành + Hoàn thành đúng hạn`, `Chờ xác nhận hoàn thành + Quá hạn 1 ngày`. Source hiện tại không tự chuyển trạng thái workflow khi vượt hạn và không tự hoàn thành khi tới ngày kết thúc dự kiến.

### 22.1. Dữ liệu thời gian của Công việc

| Thuộc tính | Ý nghĩa thực tế | Nguồn dữ liệu | Khi nào được gán | Có dùng để xác định trễ không |
| ---------- | --------------- | ------------- | ---------------- | ----------------------------- |
| `NgayBatDauCongViec` | Ngày bắt đầu Công việc | `Models/Entities/CongViec.cs`, bảng `CONG_VIEC`; projected trong `CongViecService.GetPageAsync` | Được tạo từ luồng đề xuất/duyệt Công việc hoặc dữ liệu hiện có; màn `/CongViec` chỉ đọc | Không đủ để tính trễ; dùng làm mốc thông tin |
| `NgayKetThucCVDuKien` | Ngày kết thúc dự kiến, tức hạn Công việc | `CongViec.NgayKetThucCVDuKien`; export gọi là `Hạn kết thúc`; filter `locTheoNgay = HanCongViec` | Được gán khi Công việc được tạo từ đề xuất: `DyetDeXuatCongViecService` set từ `NgayKetThucCVDeXuatDuKien`; danh sách chỉ đọc | Có, là mốc deadline chính của Công việc |
| `NgayKetThucCVThucTe` | Ngày hoàn thành thực tế theo workflow hiện tại | `CongViec.NgayKetThucCVThucTe`; projected vào `CongViecItemViewModel` | `TrangThaiWorkflowService.DongBoTrangThaiCongViecTheoChiTietAsync` set `DateTime.Now` khi Công việc lên `ChoXacNhanHoanThanh` hoặc hoàn thành; `CongViecService.XacNhanHoanThanhCongViecAsync` chỉ set nếu còn rỗng; `MoLaiCongViecAsync` xóa về `null` | Có thể dùng để so với `NgayKetThucCVDuKien`, nhưng cần ghi rõ ngày này có thể là lúc đủ điều kiện/chờ xác nhận, không hẳn lúc quản lý bấm xác nhận |
| `TrangThaiCongViec` | Trạng thái workflow của Công việc | `CongViec.TrangThaiCongViec`, normalize bằng `TrangThai.ToCode`, display bằng `TrangThai.ToDisplay` | Được đồng bộ từ Chi tiết công việc trong `TrangThaiWorkflowService`; xác nhận/mở lại trong `CongViecService` | Cần dùng để biết đã hoàn thành hay chưa, nhưng không thay thế tình trạng thời hạn |

Khi tất cả Chi tiết công việc hoàn thành, `TrangThaiWorkflowService.DongBoTrangThaiCongViecTheoChiTietAsync` đưa Công việc lên `ChoXacNhanHoanThanh` và set `NgayKetThucCVThucTe = DateTime.Now` nếu đang rỗng. Khi người có quyền xác nhận, `CongViecService.XacNhanHoanThanhCongViecAsync` đổi `TrangThaiCongViec = HoanThanh` và chỉ set `NgayKetThucCVThucTe` nếu chưa có giá trị. Vì vậy, trong luồng chuẩn, ngày thực tế thường được ghi sớm ở thời điểm lên `ChoXacNhanHoanThanh`, không phải thời điểm xác nhận cuối cùng. Khi mở lại bằng `MoLaiCongViecAsync`, source đặt `TrangThaiCongViec = DangThucHien` và xóa `NgayKetThucCVThucTe = null`.

Kết luận dữ liệu: Công việc có đủ cặp `NgayKetThucCVDuKien` và `NgayKetThucCVThucTe` để xác định hoàn thành đúng hạn hoặc trễ hạn ở mức dữ liệu hiện có. Rủi ro nghiệp vụ là ngày thực tế đang đại diện thời điểm hệ thống ghi nhận đủ điều kiện/chờ xác nhận, không lưu riêng ngày nhân viên hoàn tất và ngày quản lý xác nhận.

### 22.2. Logic Công việc đang quá hạn

Điều kiện nghiệp vụ có thể xác định:

```text
TrangThaiCongViec chưa hoàn thành
và NgayKetThucCVDuKien < DateTime.Today
```

Source hiện tại có tính logic này ở một số module khác, nhưng chưa tính trong danh sách `/CongViec`:

| Nơi | Logic hiện có | Lưu DB hay runtime | Có vào `CongViecItemViewModel` | Có hiển thị ở bảng `/CongViec` |
| --- | ------------- | ------------------ | ------------------------------ | ------------------------------ |
| `DashboardService` | `CongViecTreHanQuery`: có hạn, hạn `< DateTime.Now`, trạng thái không thuộc hoàn thành | Runtime query | Không | Không, chỉ dashboard |
| `DashboardService.LayTopCongViecTreAsync` | Có hạn, hạn `< DateTime.Today`, chưa hoàn thành, tính `SoNgayTre` | Runtime query | Không | Không, chỉ top dashboard |
| `AiDatasetService` | Nếu chưa hoàn thành thì `homNay > NgayKetThucCVDuKien`; nếu hoàn thành thì so ngày thực tế với dự kiến | Runtime snapshot/dataset | Không | Không, dùng AI dataset |
| `DanhGiaDuAnService` | Tách `CongViecDangTreHan` và `CongViecHoanThanhTreHan` | Runtime thống kê | Không | Không, ở Đánh giá dự án |
| `DuAnService` | Đếm `SoCongViecTre` theo Công việc quá hạn hoặc hoàn thành trễ | Runtime, gắn vào ViewModel Dự án | Không | Không, ở danh sách/chi tiết Dự án |
| `CongViecService.GetPageAsync` | Chỉ filter ngày theo `NgayTao` hoặc `NgayKetThucCVDuKien`; không tính quá hạn | Runtime query danh sách | Không | Không |

Phân tích theo trạng thái:

| Trạng thái | Có thể xác định quá hạn từ dữ liệu hiện có | Source xử lý hiện tại | Ảnh hưởng `TrangThaiCongViec` |
| ---------- | ------------------------------------------ | --------------------- | ----------------------------- |
| `ChuaBatDau` nhưng quá hạn | Có, nếu có `NgayKetThucCVDuKien < Today` | Dashboard/AI/DanhGia có thể đếm như chưa hoàn thành; `/CongViec` không hiển thị | Không đổi trạng thái |
| `DangThucHien` nhưng quá hạn | Có | Dashboard top trễ và AI dataset xử lý; `/CongViec` không có badge trễ | Không đổi trạng thái |
| `BiCanCan` nhưng quá hạn | Có | Được xem là chưa hoàn thành trong các logic đếm trễ; `/CongViec` chỉ hiển thị badge bị cản trở | Không đổi trạng thái |
| `ChoXacNhanHoanThanh` nhưng quá hạn | Có nhưng cần thận trọng vì có thể đã có `NgayKetThucCVThucTe` khi lên chờ xác nhận | Dashboard đang chỉ loại `HoanThanh`, nên có thể vẫn đếm chờ xác nhận quá hạn; AI dataset coi chưa hoàn thành nếu `LaHoanThanhCongViec` false | Không đổi trạng thái |
| `TamDung` nhưng quá hạn | Có theo dữ liệu ngày, nhưng nghiệp vụ có thể cần quyết định có tính tiếp số ngày trễ không | Các logic đếm chưa hoàn thành có thể tính nếu không loại trừ | Không đổi trạng thái |
| `DaHuy` | Dữ liệu ngày có thể có, nhưng nên không đánh giá thời hạn như công việc còn hiệu lực | `/CongViec` không loại riêng trong ViewModel thời hạn; Dự án helper có mẫu `DaHuy = Không đánh giá` ở cấp Dự án | Không đổi trạng thái |

Hiện không có filter `treHan`, `quaHan`, `hoanThanhTreHan`, `hoanThanhDungHan` trong `CongViecController.Index`, `ICongViecService.GetPageAsync`, `CongViecPageViewModel` hoặc `_Filter.cshtml`. Dashboard có link `href="@Url.Action("Index", "CongViec", new { treHan = true })"`, nhưng `CongViecController.Index` không nhận tham số `treHan`, nên link này không tạo được bộ lọc trễ trong màn Công việc hiện tại.

### 22.3. Logic Công việc hoàn thành đúng hạn hoặc trễ hạn

Điều kiện có thể dùng từ dữ liệu hiện có:

```text
Hoàn thành đúng hạn: TrangThaiCongViec hoàn thành
và NgayKetThucCVThucTe <= NgayKetThucCVDuKien
```

```text
Hoàn thành trễ: TrangThaiCongViec hoàn thành
và NgayKetThucCVThucTe > NgayKetThucCVDuKien
```

| Vấn đề | Kết luận AS-IS |
| ------ | -------------- |
| Có thể xác định chính xác không | Có thể xác định ở mức dữ liệu lưu hiện tại nếu cả ngày dự kiến và ngày thực tế đều có. Nếu thiếu một trong hai ngày thì chưa đủ dữ liệu. |
| Ngày hoàn thành thực tế gán lúc nào | Gán khi workflow đưa Công việc lên `ChoXacNhanHoanThanh` trong `TrangThaiWorkflowService`; khi xác nhận `HoanThanh`, `CongViecService` giữ nguyên nếu đã có. |
| Chờ quản lý xác nhận có làm sai số ngày không | Có thể làm lệch theo hướng ngày thực tế sớm hơn ngày xác nhận cuối cùng, vì không lưu riêng ngày quản lý xác nhận nếu `NgayKetThucCVThucTe` đã có từ lúc chờ xác nhận. |
| Dashboard đang dùng gì | Dashboard đếm Công việc đang trễ nếu chưa hoàn thành và hạn đã qua; timeline tháng cũng tính Công việc trễ nếu hoàn thành trễ hoặc chưa hoàn thành trong tháng deadline. |
| AI dataset đang dùng gì | `AiDatasetService` tính `SoCongViecTre`: hoàn thành thì so `NgayKetThucCVThucTe.Date > NgayKetThucCVDuKien.Date`, chưa hoàn thành thì `homNay > NgayKetThucCVDuKien.Date`. |
| Bảng Công việc có phân biệt đúng hạn/trễ không | Chưa. `_Table.cshtml` chỉ hiển thị badge workflow và ngày bắt đầu/kết thúc dự kiến, không hiển thị `NgayKetThucCVThucTe`, `Hoàn thành đúng hạn`, `Hoàn thành trễ` hoặc số ngày trễ. |

### 22.4. Các trường hợp cần đối chiếu

| Trường hợp | Source xử lý | Kết quả hiện tại | Có hiển thị | Có đủ dữ liệu |
| ---------- | ------------ | ---------------- | ----------- | ------------- |
| Công việc đang thực hiện và chưa đến hạn | `TrangThaiCongViec = DangThucHien`, hạn >= hôm nay | Hiển thị badge `Đang thực hiện`; không có tình trạng `Còn hạn` | Có trạng thái và hạn dự kiến | Có |
| Công việc đang thực hiện nhưng đã quá hạn | Các module Dashboard/AI có thể tính; `/CongViec` không tính | Vẫn chỉ là `Đang thực hiện` | Không có badge trễ trong bảng | Có nếu có hạn |
| Công việc chưa bắt đầu nhưng đã quá hạn | Có thể tính bằng hạn và trạng thái chưa hoàn thành | Vẫn chỉ là `Chưa bắt đầu` | Không có badge trễ | Có nếu có hạn |
| Công việc bị cản trở và đã quá hạn | Có thể tính như chưa hoàn thành quá hạn | Vẫn chỉ là `Bị cản trở` | Badge đỏ là blocker, không phải deadline | Có nếu có hạn |
| Công việc chờ xác nhận nhưng đã quá hạn | Có thể tính, nhưng cần lưu ý ngày thực tế có thể đã được set khi chờ xác nhận | Vẫn chỉ là `Chờ xác nhận hoàn thành` | Không có số ngày quá hạn | Có nếu có hạn; ý nghĩa ngày thực tế cần thận trọng |
| Công việc hoàn thành trước hạn | So `NgayKetThucCVThucTe.Date < NgayKetThucCVDuKien.Date` | Có thể xác định trong dữ liệu; danh sách không tách | Không | Có nếu đủ hai ngày |
| Công việc hoàn thành đúng ngày | So bằng ngày | Có thể xác định; danh sách không tách | Không | Có nếu đủ hai ngày |
| Công việc hoàn thành trễ | So `NgayKetThucCVThucTe.Date > NgayKetThucCVDuKien.Date` | AI/DanhGia/Dự án có logic; danh sách không tách | Không | Có nếu đủ hai ngày |
| Công việc được mở lại sau khi hoàn thành | `MoLaiCongViecAsync` đặt `DangThucHien` và xóa `NgayKetThucCVThucTe` | Mất mốc hoàn thành cũ trên entity Công việc | Chỉ thấy `Đang thực hiện` | Không còn đủ dữ liệu hoàn thành cũ nếu không đọc nhật ký |
| Công việc không có ngày kết thúc dự kiến | Các logic trễ thường bỏ qua | Không xác định đúng/trễ | Bảng để trống ngày kết thúc | Không |
| Công việc đã hủy | Có trạng thái `DaHuy`; không nên tự tính trễ như công việc hiệu lực nếu chưa có rule | Chỉ hiển thị `Đã hủy` | Không có tình trạng thời hạn | Dữ liệu có thể có nhưng không nên kết luận nếu thiếu rule |
| Công việc tạm dừng nhưng đã quá hạn | Dữ liệu có thể tính; hiện không có rule dừng đếm ngày trễ | Chỉ hiển thị `Tạm dừng` | Không | Có nếu có hạn, nhưng cần quyết định nghiệp vụ |

### 22.5. Phân tích giao diện Công việc

`Views/CongViec/_Table.cshtml` có hai phần:

| Khu vực | Dữ liệu thời gian hiển thị | Trạng thái hiển thị | Tình trạng thời hạn |
| ------- | -------------------------- | ------------------- | ------------------- |
| Bảng desktop | Cột `Mốc thời gian`: `Bắt đầu` = `NgayBatDauCongViec`, `Kết thúc` = `NgayKetThucCVDuKien` | Badge `workflow-badge` từ `TrangThaiHienThi` và `CssTrangThai` | Không có badge phụ, không có số ngày trễ, không hiển thị ngày thực tế |
| Card mobile | Meta `Bắt đầu`, `Kết thúc dự kiến` | Badge workflow ở header card | Không có badge phụ, không có số ngày trễ, không hiển thị ngày thực tế |
| Export | `Ngày bắt đầu`, `Hạn kết thúc`, `Trạng thái` | Text trạng thái | Không export ngày thực tế hoặc tình trạng thời hạn |

CSS `wwwroot/css/CongViec/index.css` có màu warning/danger cho workflow pending/blocker và nút nguy hiểm, nhưng không có selector riêng cho `is-late`, `deadline`, `overdue`, `Hoàn thành trễ` trong màn Công việc. Vì vậy người dùng không thể phân biệt trực tiếp `Đang thực hiện - Trễ`, `Chờ xác nhận - Quá hạn`, `Hoàn thành - Hoàn thành trễ` trên bảng hiện tại. Responsive không làm mất thêm thông tin trễ vì thông tin đó vốn chưa được đưa vào ViewModel/UI; mobile vẫn giữ ngày dự kiến và badge workflow.

### 22.6. ViewModel và query

`CongViecItemViewModel` đã có:

- `NgayBatDauCongViec`.
- `NgayKetThucCVDuKien`.
- `NgayKetThucCVThucTe`.
- `TrangThaiCongViec`, `TrangThaiHienThi`, `CssTrangThai`.
- `SoLuongChiTietCongViec`.

`CongViecItemViewModel` chưa có:

- `IsQuaHan`.
- `IsHoanThanhTre`.
- `IsHoanThanhDungHan`.
- `SoNgayTre`.
- `TinhTrangThoiHan`.
- `CssTinhTrangThoiHan`.
- `SoLuongChiTietCongViecTre`.

`CongViecService.GetPageAsync` hiện giữ query SQL-side cho filter/search/count/sort/pagination, rồi mới enrichment sau `ToListAsync()` cho tên fallback, phân công, số chi tiết, quyền workflow và badge workflow. Nếu bổ sung tình trạng thời hạn sau này:

- Có thể tính sau khi materialize đúng một trang nếu chỉ cần hiển thị badge trên trang hiện tại.
- Nếu cần lọc/sắp xếp theo quá hạn hoặc tổng số đúng với toàn bộ kết quả, phải đưa điều kiện SQL-translatable vào query trước `CountAsync()` và `Skip/Take`.
- Không nên dùng `AsEnumerable()` hoặc `ToListAsync()` trước filter/count vì sẽ phá phân trang server-side.
- Tránh helper C#, `string.Format`, chuỗi nội suy, `StringComparison` hoặc method tùy chỉnh trong `IQueryable`; lỗi EF Core tương tự phần đã sửa trước có thể quay lại.
- Nếu tính `SoLuongChiTietCongViecTre`, nên batch theo danh sách `MaCongViec` của page như `GanSoLuongChiTietCongViecAsync`, tránh N+1. Tuy nhiên Chi tiết hiện thiếu hạn dự kiến riêng nên số chi tiết trễ chưa xác định chính xác.

### 22.7. Bộ lọc và thống kê

| Nội dung | Hiện trạng |
| -------- | ---------- |
| Lọc Công việc quá hạn | Chưa có trong `CongViecController.Index`, `_Filter.cshtml`, `CongViecPageViewModel`, `CongViecService.GetPageAsync` |
| Lọc Công việc hoàn thành trễ | Chưa có |
| Lọc Công việc hoàn thành đúng hạn | Chưa có |
| Sắp xếp Công việc trễ lên đầu | Chưa có; service sort theo `NgayTaoCongViec` giảm dần rồi `MaCongViec` giảm dần |
| Summary card Công việc trễ | Chưa có ở `/CongViec`; chỉ có tổng, đang thực hiện, chờ xác nhận, hoàn thành, bị cản |
| Phạm vi summary card | `Views/CongViec/Index.cshtml` tính trên `Model.DanhSach`, tức trang hiện tại sau phân trang, không phải toàn bộ kết quả filter |
| Dashboard đếm Công việc trễ | Có ở `DashboardService`, runtime query toàn dashboard |
| Export Công việc trễ | Export `/CongViec/XuatFile` không có cột tình trạng thời hạn hoặc ngày thực tế |

### 22.8. Đối chiếu Dự án -> Công việc -> Chi tiết công việc

| Cấp | Ngày dự kiến | Ngày thực tế | Có thể tính trễ | Nơi đang tính | Có hiển thị ở danh sách |
| --- | ------------ | ------------ | --------------- | ------------- | ----------------------- |
| Dự án | `DuAn.NgayKetThucDuAn` | `DuAn.NgayHoanThanhThucTeDuAn` hoặc fallback từ Công việc trong một số thống kê | Có | `DuAnDeadlineStatusHelper`, `DuAnService`, `DashboardService`, `DanhGiaDuAnService`, `AiService` | Có ở module Dự án/Dashboard |
| Công việc | `CongViec.NgayKetThucCVDuKien` | `CongViec.NgayKetThucCVThucTe` | Có nếu đủ hai ngày; đang quá hạn nếu chưa hoàn thành và hạn < hôm nay | `DashboardService`, `AiDatasetService`, `DanhGiaDuAnService`, `DuAnService` | Chưa hiển thị ở danh sách `/CongViec` |
| Chi tiết công việc | Không có trường hạn dự kiến riêng | `CtCongViec.NgayKetThucCTCV` | Không xác định chính xác từ dữ liệu hiện tại | Một số thống kê đang đếm theo `NgayKetThucCTCV`, nhưng mốc này là ngày thực tế nên không đủ tin cậy | Chưa hiển thị tình trạng thời hạn ở `/ChiTietCongViec` |

Logic hiện chưa thống nhất hoàn toàn: Dự án có helper riêng, Công việc có logic trễ rải ở Dashboard/AI/DanhGia/Dự án, còn Chi tiết công việc không có deadline riêng nhưng một số thống kê vẫn dùng `NgayKetThucCTCV` để đếm `ChiTietTreHan`. Ở bước chỉnh sửa sau có thể cân nhắc helper/service dùng chung cho tình trạng thời hạn, nhưng không tạo helper trong bước tài liệu này.

### 22.9. Rủi ro cần kiểm tra trước khi chỉnh sửa sau

| Rủi ro | Bằng chứng AS-IS | Hướng ghi nhận |
| ------ | ---------------- | -------------- |
| Ngày hoàn thành thực tế Công việc được gán quá sớm | `TrangThaiWorkflowService` set `NgayKetThucCVThucTe` khi lên `ChoXacNhanHoanThanh` | Khi hiển thị hoàn thành trễ cần ghi chú ngày thực tế là ngày hệ thống ghi nhận đủ điều kiện/chờ xác nhận |
| Chờ xác nhận bị hiểu nhầm là hoàn thành thật | `CongViecService.XacNhanHoanThanhCongViecAsync` mới đổi sang `HoanThanh`; trước đó là `ChoXacNhanHoanThanh` | Tách workflow và tình trạng thời hạn |
| Mở lại Công việc làm mất ngày hoàn thành cũ | `MoLaiCongViecAsync` set `NgayKetThucCVThucTe = null` | Không thể đánh giá lần hoàn thành cũ từ entity hiện tại |
| Dùng `DateTime.Now` và `.Date` chưa thống nhất | Dashboard có nơi dùng `DateTime.Now`, nơi dùng `DateTime.Today`; AI dùng `DateTime.Today`; so sánh hoàn thành thường dùng `.Date` | Nên chuẩn hóa nếu triển khai |
| `DaHuy`/`TamDung` vẫn bị tính như chưa hoàn thành trễ | Một số query chỉ loại hoàn thành | Cần rule nghiệp vụ rõ trước khi filter hiển thị |
| Summary card chỉ tính trên trang hiện tại | `Index.cshtml` dùng `Model.DanhSach.Count` | Nếu thêm card trễ toàn bộ cần query tổng riêng |
| Filter trễ sau phân trang làm sai tổng | `GetPageAsync` count trước pagination | Nếu có filter trễ phải đưa vào query trước `CountAsync()` |
| N+1 khi đếm chi tiết trễ | Hiện số chi tiết được batch theo page | Giữ pattern batch; không query từng dòng |
| EF Core translate lỗi | Lịch sử đã sửa fallback string interpolation trong `CongViecService` | Không đưa helper/string interpolation/StringComparison vào `IQueryable` |

### 22.10. Kết luận chức năng Công việc

- Công việc có đủ dữ liệu cơ bản để biết hoàn thành đúng hạn hoặc trễ hạn: `NgayKetThucCVDuKien`, `NgayKetThucCVThucTe`, `TrangThaiCongViec`.
- Logic tính Công việc trễ đã tồn tại rải rác ở `DashboardService`, `AiDatasetService`, `DanhGiaDuAnService`, `DuAnService`, nhưng chưa nằm trong `CongViecService.GetPageAsync` và chưa được đưa vào `CongViecItemViewModel`.
- Danh sách `/CongViec` hiện thiếu cờ tình trạng thời hạn, số ngày trễ, ngày hoàn thành thực tế hiển thị, filter/sort theo trễ hạn và summary card trễ hạn.
- Không cần sửa database để nhận biết đúng/trễ cho Công việc, vì cặp ngày dự kiến/thực tế đã có. Có thể cần sửa ViewModel/service/view/CSS nếu bước sau muốn hiển thị.
- File có khả năng cần sửa ở bước tiếp theo: `CongViecItemViewModel.cs`, `CongViecPageViewModel.cs`, `CongViecService.cs`, `CongViecController.cs`, `Views/CongViec/_Filter.cshtml`, `Views/CongViec/_Table.cshtml`, `Views/CongViec/Index.cshtml`, `wwwroot/css/CongViec/index.css`, và có thể `DashboardService.cs` nếu muốn link dashboard lọc đúng.
- Ràng buộc không được phá khi chỉnh sửa sau: không biến `Trễ hạn` thành `TrangThaiCongViec`, không tự động đổi workflow theo ngày, không bỏ data scope/permission, không phá server-side pagination, không tạo N+1, không dùng biểu thức EF Core khó dịch, không sửa schema nếu chỉ làm hiển thị Công việc.

## 23. Kết quả triển khai tình trạng thời hạn Công việc

### 23.1. File đã sửa

| File | Nội dung |
| ---- | -------- |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/CongViec/CongViecDeadlineStatus.cs` | Thêm mã tình trạng/filter thời hạn trong phạm vi ViewModel Công việc: `ConHan`, `QuaHan`, `HoanTatDungHan`, `HoanTatTre`, `HoanThanhDungHan`, `HoanThanhTre`, `KhongDanhGia`, `ChuaXacDinh` và filter `QuaHan`, `ChoXacNhanTre`, `HoanThanhTre`, `HoanThanhDungHan`, `ConHan`. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/CongViec/CongViecItemViewModel.cs` | Bổ sung thuộc tính hiển thị thời hạn: `IsQuaHan`, `IsHoanThanhTre`, `IsHoanThanhDungHan`, `IsKhongDanhGiaThoiHan`, `SoNgayTre`, `MaTinhTrangThoiHan`, `TinhTrangThoiHan`, `CssTinhTrangThoiHan`. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/CongViec/CongViecPageViewModel.cs` | Bổ sung `LocTinhTrangThoiHan` để giữ filter trên form, phân trang và export. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/ICongViecService.cs` | Mở rộng contract `GetPageAsync` với tham số `locTinhTrangThoiHan`. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs` | Thêm filter thời hạn SQL-translatable trước `CountAsync`; tính badge thời hạn sau `ToListAsync`; giữ nguyên data scope, search, lọc ngày, phân trang và enrichment batch hiện có. |
| `QuanLyDuAn/QuanLyDuAn/Controllers/CongViecController.cs` | Thêm tham số `locTinhTrangThoiHan`, giữ tương thích link dashboard `treHan=true`, truyền filter vào service và export thêm cột thời hạn. |
| `QuanLyDuAn/QuanLyDuAn/Views/CongViec/Index.cshtml` | Truyền `LocTinhTrangThoiHan` xuống table và export route values. |
| `QuanLyDuAn/QuanLyDuAn/Views/CongViec/_Filter.cshtml` | Thêm select `Tình trạng thời hạn` với các lựa chọn: tất cả, đang quá hạn, chờ xác nhận trễ, hoàn thành trễ, hoàn thành đúng hạn, còn hạn. |
| `QuanLyDuAn/QuanLyDuAn/Views/CongViec/_Table.cshtml` | Hiển thị badge workflow và badge tình trạng thời hạn riêng ở desktop/mobile; thêm ngày `Hoàn tất` khi có `NgayKetThucCVThucTe`; giữ hidden filter trong form xác nhận/mở lại. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/CongViec/index.css` | Thêm style badge thời hạn trong scope `.cong-viec-page`, chỉnh grid filter để có thêm select thời hạn, giữ responsive desktop/tablet/mobile. |
| `docs/congviec.md` | Ghi nhận kết quả triển khai, build và phạm vi không đổi. |

Không sửa `docs/ctcongviec.md`, entity, DbContext, migration, SQL schema hoặc dữ liệu.

### 23.2. Logic xác định thời hạn

Logic hiển thị nằm trong `CongViecService.GanThongTinThoiHan(...)`, chạy sau khi query đã `CountAsync`, `OrderBy`, `Skip/Take`, `ToListAsync`. Ngày tham chiếu dùng `DateTime.Today`; mọi so sánh dùng `.Date`.

| Trường hợp | Mã | Hiển thị | Số ngày trễ |
| ---------- | -- | -------- | ----------- |
| `DaHuy` | `KhongDanhGia` | `Không đánh giá` | `0` |
| Không có `NgayKetThucCVDuKien` | `ChuaXacDinh` | `Chưa xác định` | `0` |
| `HoanThanh` nhưng thiếu `NgayKetThucCVThucTe` | `ChuaXacDinh` | `Chưa xác định ngày hoàn thành` | `0` |
| `HoanThanh`, thực tế <= dự kiến | `HoanThanhDungHan` | `Hoàn thành đúng hạn` | `0` |
| `HoanThanh`, thực tế > dự kiến | `HoanThanhTre` | `Hoàn thành trễ X ngày` | Theo chênh lệch thực tế - dự kiến |
| `ChoXacNhanHoanThanh`, có ngày thực tế <= dự kiến | `HoanTatDungHan` | `Hoàn tất đúng hạn` | `0` |
| `ChoXacNhanHoanThanh`, có ngày thực tế > dự kiến | `HoanTatTre` | `Hoàn tất trễ X ngày` | Theo chênh lệch thực tế - dự kiến |
| `ChoXacNhanHoanThanh`, chưa có ngày thực tế, đã qua hạn | `QuaHan` | `Quá hạn X ngày` | Theo hôm nay - dự kiến |
| `ChuaBatDau`, `DangThucHien`, `BiCanCan` đã qua hạn | `QuaHan` | `Trễ X ngày` | Theo hôm nay - dự kiến |
| `TamDung` đã qua hạn | `QuaHan` | `Quá hạn X ngày` | Theo hôm nay - dự kiến |
| Chưa qua hạn | `ConHan` | `Còn hạn` | `0` |

`TrangThaiCongViec` không bị đổi. `Trễ hạn`, `Quá hạn`, `Hoàn thành trễ` chỉ là tình trạng thời hạn trong ViewModel.

### 23.3. Logic cho Chờ xác nhận hoàn thành

`ChoXacNhanHoanThanh` được xử lý riêng trong `CongViecService.GanThongTinThoiHan(...)`:

- Nếu đã có `NgayKetThucCVThucTe`, so ngày thực tế với `NgayKetThucCVDuKien`.
- Nếu đúng hạn, hiển thị `Hoàn tất đúng hạn`.
- Nếu trễ, hiển thị `Hoàn tất trễ X ngày`.
- Nếu chưa có `NgayKetThucCVThucTe`, dùng hạn dự kiến so với `DateTime.Today`; quá hạn thì `Quá hạn X ngày`, chưa quá hạn thì `Còn hạn`.

Dùng chữ `Hoàn tất` để không nhầm với trạng thái workflow `HoanThanh`, vì Công việc vẫn cần bước xác nhận cuối cùng.

### 23.4. Bộ lọc tình trạng thời hạn

`CongViecController.Index` và `XuatFile` nhận `locTinhTrangThoiHan`. Nếu link cũ từ Dashboard gửi `treHan=true` mà chưa có `locTinhTrangThoiHan`, controller ánh xạ sang `QuaHan`.

Filter được áp dụng trong `CongViecService.ApDungLocTinhTrangThoiHan(...)` trước `CountAsync()` và trước phân trang:

| Filter | Điều kiện chính |
| ------ | --------------- |
| `QuaHan` | Có hạn, hạn < hôm nay, chưa hoàn thành, không `ChoXacNhanHoanThanh`, không `DaHuy` |
| `ChoXacNhanTre` | `ChoXacNhanHoanThanh`, có hạn, ngày thực tế trễ hạn hoặc chưa có ngày thực tế nhưng hôm nay đã qua hạn |
| `HoanThanhTre` | Hoàn thành, có hạn, có ngày thực tế, ngày thực tế > hạn |
| `HoanThanhDungHan` | Hoàn thành, có hạn, có ngày thực tế, ngày thực tế <= hạn |
| `ConHan` | Có hạn, hạn >= hôm nay, chưa hoàn thành, không `ChoXacNhanHoanThanh`, không `DaHuy` |

Các điều kiện filter chỉ dùng cột `TrangThaiCongViec`, `NgayKetThucCVDuKien`, `NgayKetThucCVThucTe`, danh sách biến thể trạng thái từ `TrangThai.GetCommonStatusVariants(...)`, `Contains(...)`, so sánh ngày và `.Date`. Không gọi helper C# trong `Where`, không dùng `string.Format`, chuỗi nội suy, `ToString("dd/MM/yyyy")`, `StringComparison` hoặc `TrangThai.ToDisplay` trong query.

### 23.5. Server-side pagination, N+1 và EF Core

- Lọc tình trạng thời hạn chạy trên `IQueryable` trước `CountAsync()`, nên tổng số bản ghi và phân trang khớp điều kiện filter.
- Service vẫn giữ thứ tự: data scope -> filter/search/ngày/trạng thái/thời hạn -> Employee scope -> `CountAsync()` -> sort -> `Skip/Take` -> `ToListAsync()`.
- Tình trạng thời hạn hiển thị được tính trên danh sách đã materialize của đúng trang hiện tại, không tạo query riêng theo từng Công việc.
- Các batch enrichment cũ `GanSoNguoiDuocPhanCongAsync` và `GanSoLuongChiTietCongViecAsync` giữ nguyên, không phát sinh N+1.
- Không đưa lại fallback chuỗi nội suy vào projection/query EF Core; các fallback tên dự án/danh mục/mức độ vẫn chạy sau materialization như trước.

### 23.6. Giao diện và xuất file

`Views/CongViec/_Table.cshtml` hiển thị:

- Badge trạng thái workflow hiện có.
- Badge tình trạng thời hạn bên dưới.
- Cột thời gian dùng nhãn ngắn `Bắt đầu`, `Hạn`, `Hoàn tất`.
- Ngày `Hạn` được nhấn nhẹ màu đỏ khi Công việc đang quá hạn, chờ xác nhận trễ hoặc hoàn thành trễ.
- Card mobile có cùng badge thời hạn và ngày hoàn tất khi có.

`CongViecController.XuatFile` bổ sung cột:

- `Ngày hoàn thành thực tế`.
- `Tình trạng thời hạn`.
- `Số ngày trễ`.

Export vẫn giữ quyền `Permissions.ThongKe.XuatFile` và giữ các cột cũ.

### 23.7. Kết quả build và kiểm tra

Lệnh đã chạy:

```powershell
dotnet build
```

Kết quả: không build được ở root vì thư mục root không chứa project/solution, MSBuild báo `MSB1003`.

Lệnh build đúng project:

```powershell
dotnet build QuanLyDuAn\QuanLyDuAn\QuanLyDuAn.csproj
```

Kết quả:

- Build thành công.
- `0` error.
- Có `2` warning `CS1998` cũ ở `Services/Implementations/FileTienDoCongViecService.cs`, không thuộc thay đổi Công việc.
- Lần build đầu bị `NU1301` do sandbox chặn NuGet; sau khi được cấp quyền network cho `dotnet build`, restore/build thành công.

Các trường hợp kiểm tra ở mức source/build:

| Nhóm | Kết quả |
| ---- | ------- |
| `DangThucHien`, `ChuaBatDau`, `BiCanCan` quá hạn | Hiển thị `Trễ X ngày` |
| `TamDung` quá hạn | Hiển thị `Quá hạn X ngày` |
| `ChoXacNhanHoanThanh` có ngày thực tế đúng hạn/trễ | Hiển thị `Hoàn tất đúng hạn` hoặc `Hoàn tất trễ X ngày` |
| `ChoXacNhanHoanThanh` chưa có ngày thực tế | Hiển thị `Quá hạn X ngày` hoặc `Còn hạn` theo hạn dự kiến |
| `HoanThanh` đúng hạn/trễ | Hiển thị `Hoàn thành đúng hạn` hoặc `Hoàn thành trễ X ngày` |
| `DaHuy` | Hiển thị `Không đánh giá`, không dùng badge đỏ |
| Thiếu hạn dự kiến | Hiển thị `Chưa xác định` |
| Hoàn thành thiếu ngày thực tế | Hiển thị `Chưa xác định ngày hoàn thành` |
| Lọc thời hạn | Áp dụng trước `CountAsync()` và `Skip/Take` |
| Phân trang/export | Giữ query string/filter mới qua `_Pagination`, workflow forms và export route values |

### 23.8. Phạm vi không thay đổi

- Không sửa database.
- Không sửa schema.
- Không tạo hoặc chạy migration.
- Không sửa entity `CongViec`, `CtCongViec`.
- Không sửa `QuanLyDuAnDbContext`.
- Không dùng `AI_DATASET` làm nguồn xác định trễ hạn.
- Không sửa workflow Công việc, Chi tiết công việc, Dự án, phân công, báo cáo tiến độ hoặc duyệt tiến độ.
- Không thêm tình trạng thời hạn cho Chi tiết công việc.
- Không dùng `NgayKetThucCTCV` làm hạn dự kiến.
- Không cập nhật `docs/ctcongviec.md`.
