# Phân trang server-side

Tài liệu này phản ánh trạng thái source hiện tại sau các đợt triển khai phân trang trong `QuanLyDuAn/QuanLyDuAn`. Source code là nguồn chính. Phân trang chỉ áp dụng cho danh sách chính phù hợp với page số; chat, dashboard và AI Dataset hiện tại giữ nguyên hoặc cần cơ chế riêng.

Không thay đổi trong đợt này: route, workflow nghiệp vụ, permission, data scope, soft-delete, database schema, quan hệ MVC - FastAPI, AI ground truth, Human-in-the-Loop, format export và package bên ngoài.

## Trạng thái triển khai hiện tại

| Module | Trạng thái | Cách xử lý | Page size | Export | Build | Ghi chú |
| ------ | ---------- | ---------- | --------- | ------ | ----- | ------- |
| Nền tảng dùng chung | Đã hoàn thành | `PaginationViewModel`, `PagedResultViewModel`, `_Pagination.cshtml` | 10/20/50/100, mặc định 20 | Không áp dụng | Thành công | Validate page number/page size, giữ query string |
| Công việc | Đã hoàn thành | Server-side `CountAsync` + sort + `Skip/Take` | 10/20/50/100 | Xuất toàn bộ kết quả lọc | Thành công | Export dùng `paginate: false` |
| Đề xuất công việc | Đã hoàn thành | Server-side | 10/20/50/100 | Không có export riêng trong scope sửa | Thành công | Giữ filter dự án, trạng thái, ngày |
| Duyệt đề xuất công việc | Đã hoàn thành | Server-side | 10/20/50/100 | Không có export riêng trong scope sửa | Thành công | Giữ data scope duyệt |
| Đề xuất ngân sách | Đã hoàn thành | Server-side | 10/20/50/100 | Không có export riêng trong scope sửa | Thành công | Giữ filter và form đề xuất |
| Duyệt đề xuất ngân sách | Đã hoàn thành | Server-side | 10/20/50/100 | Không có export riêng trong scope sửa | Thành công | Giữ workflow duyệt/từ chối |
| Yêu cầu đổi quản lý | Đã hoàn thành | Server-side | 10/20/50/100 | Không có export riêng trong scope sửa | Thành công | Giữ trạng thái yêu cầu |
| Duyệt yêu cầu đổi quản lý | Đã hoàn thành | Server-side | 10/20/50/100 | Không có export riêng trong scope sửa | Thành công | Giữ permission duyệt |
| Dự án | Đã hoàn thành | Server-side qua `GetPagedAsync` | 10/20/50/100 | Xuất toàn bộ kết quả lọc qua `GetAllAsync` | Thành công | Không đổi route/workflow dự án |
| Nhân sự | Đã hoàn thành | Server-side qua `GetPagedAsync` | 10/20/50/100 | Xuất toàn bộ kết quả lọc qua `GetAllAsync` | Thành công | Không đổi quản lý tài khoản |
| Team | Đã hoàn thành | Server-side qua `GetPagedAsync` | 10/20/50/100 | Không có export | Thành công | Giữ tìm kiếm, trạng thái, số thành viên |
| Thành viên team | Đã hoàn thành | Server-side danh sách thành viên team | 10/20/50/100 | Không có export | Thành công | Giữ danh sách team/nhân sự để thêm |
| Danh mục công việc | Đã hoàn thành | Server-side danh sách danh mục theo dự án | 10/20/50/100 | Không có export | Thành công | Giữ `locMaDuAn`, thông tin dự án |
| Chi tiết công việc | Đã hoàn thành | Server-side chỉ danh sách chi tiết của công việc | 10/20/50/100 | Xuất toàn bộ chi tiết của công việc | Thành công | Thống kê tổng tính trên toàn bộ chi tiết |
| Ngân sách | Đã hoàn thành | Server-side danh sách ngân sách chính | 10/20/50/100 | Xuất toàn bộ kết quả lọc | Thành công | Tổng ngân sách/đã chi/còn lại tính theo toàn bộ filter và scope |
| Thành viên dự án | Đã hoàn thành | Server-side chỉ danh sách thành viên đã tham gia | 10/20/50/100 | Không có export | Thành công | Danh sách ứng viên giữ nguyên, không dùng chung page |
| Team dự án | Đã hoàn thành | Server-side chỉ danh sách team đã gán | 10/20/50/100 | Không có export | Thành công | Team ứng viên và thành viên team đang chọn giữ nguyên |
| Tiến độ công việc | Đã hoàn thành | Server-side page dòng chính `CT_CONG_VIEC`, timeline/file theo ID trang | 10/20/50/100 | Xuất toàn bộ kết quả lọc | Thành công | Không phân trang số timeline con |
| Đánh giá dự án | Đã hoàn thành | Page theo dự án trong scope, kèm đánh giá mới nhất nếu có | 10/20/50/100 | Xuất toàn bộ kết quả lọc | Thành công | Dữ liệu hỗ trợ tính theo ID dự án của trang |
| Đánh giá nhân viên | Đã hoàn thành | Page theo cặp `MaDuAn + MaNhanVien` | 10/20/50/100 | Xuất toàn bộ kết quả lọc | Thành công | Tránh đếm trùng do công việc/chi tiết/phân công |
| AI Model | Đã hoàn thành | UI pagination sau khi MVC hợp nhất dữ liệu FastAPI | 10/20/50/100 | Export metadata giữ nguyên | Thành công | Chưa tối ưu lượng dữ liệu FastAPI trả về |
| Chat tin nhắn | Không áp dụng phân trang số | Nên dùng cursor/load more nếu triển khai sau | Không áp dụng | Không áp dụng | Không ảnh hưởng | Giữ nguyên để không ảnh hưởng cuộn và form gửi tin |
| Dashboard | Giữ nguyên | Top list/card thống kê | Không áp dụng | Giữ nguyên | Thành công | Không phân trang card, top list, preview |
| AI Dataset | Giữ nguyên | Card/thống kê/thao tác tổng hợp | Không áp dụng | Giữ nguyên | Thành công | Không thêm bảng row, không đổi ground truth |

## Nền tảng dùng chung

Các file dùng chung:

- `ViewModels/Common/PaginationViewModel.cs`
- `ViewModels/Common/PagedResultViewModel.cs`
- `Views/Shared/_Pagination.cshtml`

`PaginationViewModel` chuẩn hóa `pageNumber < 1` về `1`, chỉ nhận page size `10`, `20`, `50`, `100`, mặc định `20`, tính `TotalPages`, `FromItem`, `ToItem`, `HasPreviousPage`, `HasNextPage` và không sinh trang `0`. Nếu page vượt tổng số trang, page được đưa về trang hợp lệ cuối cùng.

Partial `_Pagination.cshtml` không hard-code controller/action, giữ query string hiện tại, ghi đè `pageNumber`, giữ `pageSize`, đổi page size về trang 1, hiển thị `0 kết quả` khi rỗng và không hiển thị `1-0`. Partial có form GET riêng và được đặt ngoài form filter/form nhập liệu trong các view đã sửa để tránh nested form.

## Module đã triển khai trong lượt cuối

### Tiến độ công việc

- Controller/action: `TienDoCongViecController.Index`, `TienDoCongViecController.XuatFile`.
- Service: `ITienDoCongViecService.GetPageAsync`, `TienDoCongViecService.GetPageAsync`.
- ViewModel/view: `TienDoCongViecPageViewModel`, `Views/TienDoCongViec/Index.cshtml`.
- Dòng chính là `CT_CONG_VIEC` theo `MaChiTietCV`.
- Query áp dụng permission, data scope, soft-delete, `locMaDuAn`, `locMaCongViec`, `locMaChiTietCv`, `tuKhoa`, `tuNgayBaoCao`, `denNgayBaoCao` trước `CountAsync`.
- Sort ổn định theo `MaChiTietCV`, sau đó `Skip/Take`.
- Chỉ nạp `TIEN_DO_CONG_VIEC`, `FILE_TIEN_DO_CONG_VIEC`, người cập nhật/người duyệt và phân công cho các `MaChiTietCV` thuộc trang hiện tại.
- Timeline lịch sử và file vẫn render trong collapse hiện tại, không dùng page số riêng.
- Export gọi `paginate: false`, xuất toàn bộ kết quả lọc/scope.

### Đánh giá dự án

- Controller/action: `DanhGiaDuAnController.Index`, `DanhGiaDuAnController.XuatFile`.
- Service: `IDanhGiaDuAnService.GetPageAsync`, `DanhGiaDuAnService.GetPageAsync`.
- ViewModel/view: `DanhGiaDuAnPageViewModel`, `Views/DanhGiaDuAn/Index.cshtml`.
- Một dòng là một dự án trong scope, kèm bản đánh giá mới nhất nếu có.
- Filter theo từ khóa, dự án, trạng thái đánh giá và ngày đánh giá được áp dụng trước `CountAsync`.
- Page theo dự án chính; thống kê công việc, tình trạng trễ hạn và dữ liệu đánh giá chỉ nạp cho dự án thuộc trang hiện tại.
- Giữ nguyên trạng thái `Nhap`, `ChoDuyet`, `DaDuyet`, `TuChoi`, workflow lưu/gửi duyệt/duyệt/từ chối và AI insight.
- Export gọi `paginate: false`.

### Đánh giá nhân viên

- Controller/action: `DanhGiaNhanVienController.Index`, `DanhGiaNhanVienController.XuatFile`.
- Service: `IDanhGiaNhanVienService.GetPageAsync`, `DanhGiaNhanVienService.GetPageAsync`.
- ViewModel/view: `DanhGiaNhanVienPageViewModel`, `Views/DanhGiaNhanVien/Index.cshtml`.
- Một dòng là một cặp `MaDuAn + MaNhanVien`, kèm bản đánh giá mới nhất nếu có.
- Filter theo `maDuAn`, `maNhanVien`, `tuKhoa`, `trangThai`, `tuNgayDanhGia`, `denNgayDanhGia` trước `CountAsync`.
- Page theo cặp dự án - nhân viên; thống kê chi tiết/công việc chỉ tính cho các cặp thuộc trang hiện tại.
- Giữ nguyên workflow lưu/gửi duyệt/duyệt/từ chối, permission và trạng thái đánh giá.
- Export gọi `paginate: false`.

### AI Model

- Controller/action: `AiController.Models`.
- Service: `IAiService.LayTrangModelAsync`, `AiService.LayTrangModelAsync`.
- ViewModel/view: `AiModelPageViewModel`, `Views/Ai/Models.cshtml`.
- Danh sách model vẫn lấy từ FastAPI rồi hợp nhất tại MVC như hiện tại.
- `DanhSachModel` được phân trang UI sau khi nhận toàn bộ danh sách; `LichSuModelNguyenNhan` vẫn giữ đầy đủ để biểu đồ, cảnh báo chất lượng và metadata không mất dữ liệu.
- Không sửa FastAPI contract, train, activate, validate, compare, reload, delete hoặc export metadata.
- Đây là UI pagination, chưa tối ưu lượng dữ liệu trả từ FastAPI.

## Export

Các export đã rà trong scope phân trang:

- `CongViecController.XuatFile`: `paginate: false`.
- `DuAnController.XuatFile`: dùng `GetAllAsync`.
- `NhanSuController.XuatFile`: dùng `GetAllAsync`.
- `ChiTietCongViecController.XuatFile`: `paginate: false`.
- `NganSachController.XuatFile`: `paginate: false`.
- `TienDoCongViecController.XuatFile`: `paginate: false`.
- `DanhGiaDuAnController.XuatFile`: `paginate: false`.
- `DanhGiaNhanVienController.XuatFile`: `paginate: false`.
- `AiController.ExportMetadata`: giữ nguyên metadata của model đang chọn.

Các module Team, Thành viên team, Danh mục công việc, Thành viên dự án, Team dự án hiện không có action export riêng trong scope source đã rà.

## Build và kiểm thử

Lệnh đã chạy sau từng giai đoạn và cuối cùng:

```text
dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore
```

Kết quả hiện tại:

- Build thành công.
- 0 error.
- 2 warning cũ, không phát sinh từ phân trang:
  - `Services/Implementations/FileTienDoCongViecService.cs(26,27)` - `CS1998`.
  - `Services/Implementations/FileTienDoCongViecService.cs(31,27)` - `CS1998`.

Đã rà logic tĩnh từ source:

- `pageNumber <= 0` được chuẩn hóa về `1`.
- `pageSize=999` hoặc page size ngoài `10/20/50/100` được chuẩn hóa về `20`.
- Page vượt tổng số trang được đưa về trang cuối hợp lệ.
- Filter/search không gửi `pageNumber` cũ trong form filter hiện có nên mặc định quay về trang 1.
- Partial đổi page size bằng GET và đặt `pageNumber=1`.
- Link phân trang giữ query string hiện tại.
- Empty state hiển thị `0 kết quả`.
- Export các module đã sửa không dùng trang hiện tại.

Chưa kiểm thử runtime bằng dữ liệu thật trong trình duyệt/database cho các trường hợp 0 dòng, 1 dòng, 20 dòng, 21 dòng, trang cuối, mobile card, permission theo từng vai trò, timeline tiến độ, dữ liệu hỗ trợ đánh giá và model active. Các trường hợp này cần test thủ công khi có dữ liệu phù hợp.

## Kết luận sau triển khai

- Số module/màn hình đã hoàn thành phân trang server-side hoặc UI pagination phù hợp: 21.
- Số module hoàn thành một phần: 0 trong scope hiện tại.
- Số module chưa triển khai phân trang: 0 trong danh sách mục tiêu hiện tại.
- Số chức năng không áp dụng phân trang số hoặc giữ nguyên: 3 gồm Chat tin nhắn, Dashboard, AI Dataset.
- Module hoàn thành thêm trong lượt cuối: Tiến độ công việc, Đánh giá dự án, Đánh giá nhân viên, AI Model.
- Build cuối thành công với 2 warning cũ `CS1998` trong `FileTienDoCongViecService.cs`.
- Không đổi route, workflow, permission, data scope, database schema, FastAPI contract, AI ground truth, Human-in-the-Loop và không cài package phân trang ngoài.
