# Phân tích hiện trạng chức năng Quản lý dự án

## Mục lục

1. [Tổng quan chức năng dự án](#1-tổng-quan-chức-năng-dự-án)
2. [Cấu trúc dữ liệu dự án đang sử dụng](#2-cấu-trúc-dữ-liệu-dự-án-đang-sử-dụng)
3. [Trạng thái và workflow dự án](#3-trạng-thái-và-workflow-dự-án)
4. [Logic xác định tiến độ dự án](#4-logic-xác-định-tiến-độ-dự-án)
5. [Logic nhận biết dự án trễ hiện tại](#5-logic-nhận-biết-dự-án-trễ-hiện-tại)
6. [Phân tích bảng danh sách dự án hiện tại](#6-phân-tích-bảng-danh-sách-dự-án-hiện-tại)
7. [Cách hiển thị trạng thái hiện tại](#7-cách-hiển-thị-trạng-thái-hiện-tại)
8. [Bộ lọc và tìm kiếm dự án](#8-bộ-lọc-và-tìm-kiếm-dự-án)
9. [ViewModel danh sách dự án](#9-viewmodel-danh-sách-dự-án)
10. [Query EF Core lấy danh sách dự án](#10-query-ef-core-lấy-danh-sách-dự-án)
11. [Quyền và phạm vi xem dự án](#11-quyền-và-phạm-vi-xem-dự-án)
12. [JavaScript, CSS và responsive](#12-javascript-css-và-responsive)
13. [Các điểm chưa phù hợp với nhu cầu nhận biết dự án trễ](#13-các-điểm-chưa-phù-hợp-với-nhu-cầu-nhận-biết-dự-án-trễ)
14. [Các ràng buộc cần giữ khi chỉnh sửa sau này](#14-các-ràng-buộc-cần-giữ-khi-chỉnh-sửa-sau-này)
15. [Danh sách file liên quan đã đọc](#15-danh-sách-file-liên-quan-đã-đọc)

Tài liệu này mô tả đúng hiện trạng source code đang có trong checkout `QuanLyDuAnAI` tại thời điểm phân tích. Không có thay đổi Controller, Service, View, ViewModel, Entity, JavaScript, schema hoặc dữ liệu. File `Views/DuAn/Create.cshtml` và `Views/DuAn/Edit.cshtml` không tồn tại trong source hiện tại; tạo và sửa dự án đang đi qua `Views/DuAn/Index.cshtml` và partial `_Form.cshtml`.

## 1. Tổng quan chức năng dự án

### 1.1. Màn hình và luồng chính

Chức năng dự án hiện có các bề mặt sau:

| Bề mặt | File/Action | Vai trò hiện tại |
| --- | --- | --- |
| Danh sách, lọc, thống kê nhỏ, form tạo/sửa | `Controllers/DuAnController.cs` action `Index`, `Sua`, `LuuDuAn`; `Views/DuAn/Index.cshtml`, `_Filter.cshtml`, `_Table.cshtml`, `_Form.cshtml` | Hiển thị danh sách dự án có phân trang, bộ lọc, form tạo/cập nhật và các nút điều hướng. |
| Chi tiết dự án | `DuAnController.Details`, `DuAnController.ChiTiet`; `Views/DuAn/Details.cshtml`, `Views/DuAn/ChiTiet.cshtml` | Hiển thị thông tin đầy đủ hơn: ngày, quản lý, tiến độ, tài chính, công việc, cảnh báo quá hạn, AI nguyên nhân trễ, điều hướng module liên quan. |
| Upload/tải/xóa file dự án | `ThemFileDuAn`, `TaiFileDuAn`, `XoaFileDuAn`; `IFileDuAnService` | Quản lý tệp ở chi tiết dự án. |
| Workflow dự án | `BatDauDuAn`, `YeuCauHoanThanh`, `XacNhanHoanThanh`, `MoLaiDuAn`, `TamDungDuAn`; `DuAnService` | Chuyển trạng thái nghiệp vụ dự án. |
| Xuất file danh sách | `DuAnController.XuatFile` | Xuất danh sách theo bộ lọc hiện tại nếu có quyền `ThongKe.XuatFile`. |
| Phân tích nguyên nhân trễ | `PhanTichNguyenNhanTre`, `XacNhanNguyenNhanTre`; `IAiService` | Chỉ có ở chi tiết dự án, không tham gia bảng danh sách dự án. |

Luồng danh sách:

1. `DuAnController.Index` kiểm tra quyền `Permissions.DuAn.Xem`.
2. Controller lấy danh sách quyền bằng `IPhanQuyenService.GetGrantedPermissionNamesAsync(User)`.
3. Controller gọi `IDuAnService.GetPagedAsync(...)`.
4. `DuAnService.GetPagedAsync` tạo query từ `DU_AN` join `LOAI_DU_AN`, áp dụng scope theo role, lọc, đếm tổng, phân trang, rồi trả về `PagedResultViewModel<DuAnViewModel>`.
5. `Views/DuAn/Index.cshtml` render header, các card thống kê theo trang hiện tại, form, bộ lọc, partial `_Table`, partial `_Pagination`.

Luồng chi tiết:

1. `DuAnController.Details` kiểm tra `Permissions.DuAn.Xem`, gọi `IDuAnService.GetChiTietAsync(id)`.
2. `DuAnService.GetChiTietAsync` lấy dữ liệu dự án, quản lý, file, số lượng công việc/chi tiết, trạng thái workflow, cảnh báo ngày, tổng hợp công việc, ngân sách, deadline gần nhất, hoạt động gần đây.
3. `GanNguCanhDetailsAsync` gán quyền, quyền quản lý file và panel AI từ `IAiService.LayPhanTichNguyenNhanDuAnAsync`.
4. `Views/DuAn/Details.cshtml` hiển thị cảnh báo quá hạn/sắp đến hạn và số công việc trễ trong phần tổng quan, nhưng đây không phải dữ liệu của bảng danh sách.

### 1.2. Quyền chức năng

Các quyền dự án nằm trong `Constants/Permissions.cs`:

| Quyền | Giá trị | Nơi dùng chính |
| --- | --- | --- |
| Xem | `DuAn.Xem` | Vào `Index`, `Details`, tải file dự án, nút Chi tiết. |
| Thêm | `DuAn.Them` | Hiện form tạo, lưu dự án mới. |
| Sửa | `DuAn.Sua` | Sửa dự án, upload/xóa file, bắt đầu, tạm dừng, yêu cầu hoàn thành, xác nhận hoàn thành, mở lại. |
| Xóa | `DuAn.Xoa` | Hiện nút xóa và gọi `XoaDuAn`. |

Ngoài quyền claim, service còn kiểm tra scope nghiệp vụ:

| Nhóm | Scope danh sách trong `DuAnService.GetPagedAsync` | Scope thao tác trong `DuAnService` |
| --- | --- | --- |
| Admin | Không bị lọc bởi nhánh Manager/Employee, nên xem toàn bộ dự án không xóa mềm. | Vẫn cần quyền claim tương ứng; các thao tác workflow trong service kiểm tra `CheckManagerPermissionAsync`, nghĩa là phải là `DU_AN.MaNguoiDung`. |
| Manager | Danh sách bị lọc `MaNguoiDung == currentUserId`. | Thao tác sửa/xóa/workflow phải là quản lý hiện tại của dự án. |
| Employee | Danh sách bị lọc theo `NHAN_VIEN_DU_AN.MaNguoiDung == currentUserId`. | Bảng có thể hiện nút theo quyền claim, nhưng service dự án vẫn chặn nếu không phải quản lý dự án. |
| Leader team phụ trách | Không có nhánh riêng trong `DuAnService.GetPagedAsync`; nếu là Employee thì vẫn dựa trên `NhanVienDuAn`. Một số nút như Danh mục công việc dùng `IDanhMucCongViecScopeService.CanAccessProjectAsync`. | Không có quyền workflow dự án riêng cho leader trong `DuAnService`. |
| Nhân viên không thuộc dự án | Nếu role Employee thì không thấy dự án không thuộc `NhanVienDuAn`. | Không thao tác được do không vào scope và không phải quản lý. |

## 2. Cấu trúc dữ liệu dự án đang sử dụng

Entity chính là `Models/Entities/DuAn.cs`, map bảng `DU_AN` trong `Data/QuanLyDuAnDbContext.cs`.

| Thuộc tính | Kiểu code | Nguồn dữ liệu | Nơi sử dụng | Hiển thị ở bảng danh sách |
| --- | --- | --- | --- | --- |
| `MaDuAn` | `int` | `DU_AN.MaDuAn` | Key, route `Details/Sua/Xoa`, join AI/file/ngân sách/team/công việc | Không là cột riêng; dùng trong link/nút. |
| `MaNguoiDung` | `int` | `DU_AN.MaNguoiDung` | Quản lý dự án, scope Manager, kiểm tra `CheckManagerPermissionAsync` | Không hiển thị tên quản lý ở bảng. |
| `MaLoaiDuAn` | `int` | `DU_AN.MaLoaiDuAn` | Join `LoaiDuAn`, lọc loại, form tạo/sửa | Không hiện mã, chỉ hiện tên loại. |
| `TenDuAn` | `string?` | `DU_AN.TenDuAn`, max length 255 | Danh sách, chi tiết, form, export, dashboard | Có, cột `Tên dự án`. |
| `MoTaDuAn` | `string?` | `DU_AN.MoTaDuAn` | Tìm kiếm, form, chi tiết, export | Không hiển thị trong bảng. |
| `NgayTaoDuAn` | `DateTime?` | `DU_AN.NgayTaoDuAn` | Danh sách, lọc theo ngày tạo, chi tiết, dashboard | Có, cột `Ngày tạo`. |
| `NgayBatDauDuAn` | `DateTime?` | `DU_AN.NgayBatDauDuAn` | Form, lọc, chi tiết, export | Có, ghép trong cột `Thời gian`. |
| `NgayKetThucDuAn` | `DateTime?` | `DU_AN.NgayKetThucDuAn` | Form, lọc, chi tiết, tính quá hạn ở chi tiết, AI dataset, dashboard | Có, ghép trong cột `Thời gian`, không có cảnh báo. |
| `NgayHoanThanhThucTeDuAn` | `DateTime?` | `DU_AN.NgayHoanThanhThucTeDuAn` | Set khi `ConfirmCompletionAsync`; xóa khi `MoLaiDuAnAsync`; AI/dashboards dùng xác định hoàn thành trễ | Không hiển thị ở bảng. |
| `PhanTramHoanThanh` | `int?` entity, `int` ViewModel | `DU_AN.PhanTramHoanThanh`; cập nhật bởi `TrangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync` | Danh sách, chi tiết, dashboard, export, `CheckProjectStatusAsync` | Có, badge tiến độ trong cột `Nhóm/Thành viên`. |
| `TrangThaiDuAn` | `string?` | `DU_AN.TrangThaiDuAn`, max length 50 | Workflow, lọc, badge, kiểm tra khóa sửa, AI/dashboards | Có, cột `Trạng thái`. |
| `GhiChuDuAn` | `string?` | `DU_AN.GhiChuDuAn`, max length 255 | Lý do tạm dừng/mở lại, form, chi tiết | Không hiển thị ở bảng. |
| `IsDeleted` | `bool?` | `DU_AN.IsDeleted` | Soft-delete query danh sách/chi tiết/xóa | Không hiển thị. |
| `DeletedAt` | `DateTime?` | `DU_AN.DeletedAt` | Set trong `DeleteAsync` | Không hiển thị. |
| `DeletedBy` | `int?` | `DU_AN.DeletedBy` | Set trong `DeleteAsync`, FK `NguoiDung` | Không hiển thị. |

Dữ liệu tổng hợp đang dùng:

| Dữ liệu | Nguồn | Nơi dùng | Có trong danh sách |
| --- | --- | --- | --- |
| Tên loại dự án | `LOAI_DU_AN.TenLoai` | `GetPagedAsync`, `GetChiTietAsync` | Có. |
| Tên người quản lý | `NGUOI_DUNG.HoTenNguoiDung` | `GetChiTietAsync`, `GetByIdAsync`; trong danh sách hiện gán `string.Empty` | Không. |
| Số team | `TEAM_DU_AN.Count` | `GetPagedAsync`, `GetChiTietAsync` | Có, dạng `SoLuongTeam / SoLuongThanhVien`. |
| Số thành viên | `NHAN_VIEN_DU_AN.Count` | `GetPagedAsync`, `GetChiTietAsync` | Có, dạng `SoLuongTeam / SoLuongThanhVien`. |
| Ngân sách đã duyệt | `NGAN_SACH` active, not deleted, trạng thái duyệt | `HasApprovedBudget` danh sách/chi tiết; tổng hợp tài chính chi tiết | Có, badge đã/chưa duyệt ngân sách. |
| Chi phí đã dùng | `CHI_PHI` join `NGAN_SACH` | Chỉ chi tiết dự án/dashboard | Không. |
| Số công việc trễ | `CONG_VIEC.NgayKetThucCVDuKien`, `TrangThaiCongViec` | Chi tiết dự án, Dashboard, AI dataset | Không. |
| Deadline công việc gần nhất | `CONG_VIEC` chưa hoàn thành | Chi tiết dự án | Không. |
| `AI_DATASET.LaDuAnTre`, `SoNgayTreTienDo` | `AiDatasetService`/bảng AI | AI dataset, AI panel, đánh giá/dashboards tùy module | Không dùng trong bảng danh sách dự án. |

## 3. Trạng thái và workflow dự án

Các trạng thái dự án lấy từ `Constants/TrangThai.cs`. Source dùng cả mã không dấu và một số biến thể hiển thị thông qua `TrangThai.Normalize`, `EqualsValue`, `ToCode`, `ToDisplay`, `GetCommonStatusVariants`.

| Trạng thái | Giá trị lưu/code chính | Tên hiển thị | Cách vào trạng thái | Tự động/thủ công | Khóa sửa | Hành động kế tiếp theo source |
| --- | --- | --- | --- | --- | --- | --- |
| Khởi tạo | `KhoiTao` | Khởi tạo | Tạo dự án mới trong `SaveAsync`; form create luôn set trạng thái khởi tạo | Thủ công khi tạo | Chưa khóa sửa, nhưng xóa chỉ được nếu không phát sinh dữ liệu | Bắt đầu dự án nếu có thành viên, danh mục, công việc; xóa nếu chưa có liên quan. |
| Đang thực hiện | `DangThucHien` | Đang thực hiện | `TransitionToDangThucHienAsync`, `CheckAutoTransitionAsync`, mở lại dự án, rollback từ chờ xác nhận khi phát sinh công việc chưa hoàn thành | Có cả thủ công và tự động | Không khóa sửa; có thể tạm dừng/yêu cầu hoàn thành | Tạm dừng, yêu cầu hoàn thành, chuyển về từ mở lại. |
| Chờ xác nhận hoàn thành | `ChoXacNhanHoanThanh` | Chờ xác nhận hoàn thành | `RequestCompletionAsync`, `ValidateStatusTransitionAsync`, hoặc `TrangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync` khi tất cả công việc hoàn thành | Có cả thủ công và tự động | Không cho tạm dừng; nếu công việc mở lại có thể rollback về đang thực hiện | Xác nhận hoàn thành hoặc rollback tự động về đang thực hiện. |
| Hoàn thành | `HoanThanh`, biến thể `Done`, `Completed` | Hoàn thành | `ConfirmCompletionAsync` hoặc lưu form chuyển trạng thái hợp lệ; set `NgayHoanThanhThucTeDuAn = DateTime.Now`, `PhanTramHoanThanh = 100` khi lưu trực tiếp trạng thái hoàn thành | Thủ công | `SaveAsync` chặn chỉnh sửa dự án hoàn thành, trừ chuyển lưu trữ hợp lệ | Mở lại; lưu trữ nếu hoàn thành đúng hạn. |
| Tạm dừng | `TamDung` | Tạm dừng | `PauseProjectAsync` hoặc lưu form sang tạm dừng nếu có ghi chú | Thủ công | `TrangThaiWorkflowService` không tự cập nhật trạng thái dự án khi đang tạm dừng; `TienDoCongViecService` khóa cập nhật theo trạng thái dự án tạm dừng | Form/status check cho phép chuyển về `DangThucHien`. |
| Đã hủy | `DaHuy` | Đã hủy | Có constant và helper, nhưng không thấy action dự án chuyển sang `DaHuy` trong `DuAnController`/`DuAnService` | Không có luồng dự án hiện tại | Workflow sync bỏ qua nếu đã hủy | Không có nút chuyển trạng thái trong bảng dự án. |
| Lưu trữ | `Archived` | Lưu trữ | `_Form.cshtml` gửi `LuuDuAn` với `Form.TrangThaiDuAn = Archived`; `ValidateStatusTransitionAsync` chỉ cho từ hoàn thành đúng hạn | Thủ công | Workflow sync bỏ qua | Không thấy luồng mở lại từ lưu trữ trong source dự án. |
| Trễ tiến độ | `Tre` | Trễ tiến độ | Constant dùng chung, nhưng không được dùng làm trạng thái dự án trong workflow `DuAnService` | Không áp dụng cho dự án | Không áp dụng | Không có nút/luồng dự án. |

Các luồng cụ thể:

| Luồng | Source hiện tại |
| --- | --- |
| Khởi tạo -> Đang thực hiện | `TransitionToDangThucHienAsync` yêu cầu người dùng hiện tại là quản lý dự án, trạng thái hiện tại `KhoiTao`, có ít nhất 1 thành viên, có danh mục công việc, có công việc. `CheckAutoTransitionAsync` cũng tự chuyển khi dự án khởi tạo đủ 3 điều kiện này. |
| Đang thực hiện -> Chờ xác nhận hoàn thành | `RequestCompletionAsync` yêu cầu quản lý dự án, trạng thái đang thực hiện, `ValidateCompletionAsync` pass. `TrangThaiWorkflowService` cũng tự đưa lên `ChoXacNhanHoanThanh` khi mọi công việc hoàn thành. |
| Chờ xác nhận hoàn thành -> Hoàn thành | `ConfirmCompletionAsync` yêu cầu quản lý dự án, trạng thái chờ xác nhận, validate lại hoàn thành, rồi set `TrangThaiDuAn = HoanThanh` và `NgayHoanThanhThucTeDuAn = DateTime.Now`. |
| Hoàn thành -> Mở lại | `MoLaiDuAnAsync` yêu cầu lý do, quản lý dự án, trạng thái hoàn thành; set lại `DangThucHien`, xóa ngày hoàn thành thực tế, ghi chú lý do, soft-delete `AI_NGUYEN_NHAN` cũ và clear `AI_DATASET.MaDMNguyenNhan`. |
| Đang thực hiện -> Tạm dừng | `PauseProjectAsync` yêu cầu lý do, quản lý dự án, không phải hoàn thành; set `TrangThaiDuAn = TamDung`, `GhiChuDuAn`. |
| Tạm dừng -> Tiếp tục | `CheckProjectStatusAsync` đặt `CanTransitionToDangThucHien = true`; form lưu có thể chuyển sang `DangThucHien` qua `ValidateStatusTransitionAsync` và `ValidateCanStartProjectAsync`. |
| Hoàn thành -> Lưu trữ | Chỉ qua `_Form.cshtml` gọi `LuuDuAn`; `ValidateStatusTransitionAsync` yêu cầu hiện tại là hoàn thành và `DuAnHoanThanhDungHan(existing)` đúng. Dự án hoàn thành trễ không được lưu trữ theo logic hiện tại. |

## 4. Logic xác định tiến độ dự án

Tiến độ dự án vừa được lưu ở `DU_AN.PhanTramHoanThanh`, vừa được đồng bộ theo công việc ở `TrangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync`.

Nguồn tính chính:

| Mốc | Source | Hiện trạng |
| --- | --- | --- |
| Khi tạo dự án | `DuAnService.SaveAsync` | Set `PhanTramHoanThanh = 0`. |
| Khi lưu chuyển thẳng trạng thái hoàn thành | `DuAnService.SaveAsync` | Nếu trạng thái mới là `HoanThanh`, set `PhanTramHoanThanh = 100` và ngày hoàn thành nếu chưa có. |
| Khi đồng bộ từ công việc | `TrangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync` | Lấy tất cả `CONG_VIEC` không xóa mềm thuộc dự án, tính trung bình theo trạng thái công việc rồi lưu vào `DU_AN.PhanTramHoanThanh`. |
| Khi xem chi tiết | `DuAnService.GetChiTietAsync` | Vừa hiển thị `DU_AN.PhanTramHoanThanh`, vừa tính thêm `TienDoCongViec.TiLeHoanThanh = số công việc hoàn thành / tổng công việc * 100` để hiển thị tổng quan công việc. |

Công thức trong `TrangThaiWorkflowService`:

| Trạng thái công việc | Điểm tiến độ |
| --- | --- |
| Hoàn thành (`HoanThanh`, `Done`, `Completed`, hoặc hiển thị tương đương) | 100 |
| Chờ xác nhận hoàn thành | 90 |
| Đang thực hiện, bị cản trở, tạm dừng | 50 |
| Chưa bắt đầu/khác | 0 |

Thời điểm đồng bộ:

| Từ đâu | Source gọi đồng bộ |
| --- | --- |
| Chi tiết công việc -> Công việc -> Dự án | `TrangThaiWorkflowService.DongBoChuoiTrangThaiTuCongViecAsync` gọi `DongBoTrangThaiCongViecTheoChiTietAsync`, sau đó `DongBoTrangThaiDuAnTheoCongViecAsync`. |
| Xác nhận hoàn thành công việc | `CongViecService.XacNhanHoanThanhCongViecAsync` gọi `DongBoTrangThaiDuAnTheoCongViecAsync`. |
| Mở lại công việc | `CongViecService.MoLaiCongViecAsync` gọi `DongBoTrangThaiDuAnTheoCongViecAsync`. |
| Báo cáo tiến độ chi tiết | `TienDoCongViecService` dùng trạng thái báo cáo được duyệt để cập nhật `CT_CONG_VIEC.TrangThaiCTCV`, sau đó đồng bộ workflow. Chỉ báo cáo được duyệt mới là mốc trạng thái chính thức; báo cáo chờ duyệt/từ chối/yêu cầu bổ sung không tự đổi trạng thái chi tiết. |

Các trường hợp:

| Trường hợp | Hiện trạng |
| --- | --- |
| Không có công việc | `TinhPhanTramDuAnTheoCongViec` trả 0; `ValidateCompletionAsync` không cho hoàn thành vì `totalTasks > 0` không đạt. |
| Không có chi tiết công việc | Đồng bộ công việc theo chi tiết trả `ChuaBatDau`; dự án tính theo trạng thái công việc hiện có. |
| Công việc đã hoàn thành nhưng dự án chưa xác nhận hoàn thành | `TrangThaiWorkflowService` có thể đưa dự án lên `ChoXacNhanHoanThanh`; `ConfirmCompletionAsync` mới set hoàn thành và ngày hoàn thành thực tế. |
| Dự án chờ xác nhận nhưng có công việc bị mở lại/chưa hoàn thành | `DongBoTrangThaiDuAnTheoCongViecAsync` rollback dự án về `DangThucHien` và ghi nhật ký nếu có người thực hiện. |
| Dự án hoàn thành rồi được mở lại | `MoLaiDuAnAsync` set `DangThucHien`, clear ngày hoàn thành thực tế, xóa mềm nhãn nguyên nhân AI cũ và clear label dataset. |

## 5. Logic nhận biết dự án trễ hiện tại

Kết luận chính: trong bảng danh sách dự án hiện tại không có logic nhận biết dự án trễ. `Đang thực hiện` là trạng thái workflow lưu trong `DU_AN.TrangThaiDuAn`; `trễ hạn/quá hạn` là đặc điểm được tính ở một số module khác hoặc ở chi tiết dự án, không phải trạng thái dự án trong danh sách.

### 5.1. Nơi có logic trễ/quá hạn

| Nơi | Logic | Lưu DB hay tính lúc chạy | Có đưa vào danh sách dự án không |
| --- | --- | --- | --- |
| `DuAnService.GetChiTietAsync` | `IsQuaHan = true` nếu `NgayKetThucDuAn.Date < DateTime.Today` và trạng thái không hoàn thành/lưu trữ; `CongViecTreHan` đếm công việc chưa hoàn thành đã quá `NgayKetThucCVDuKien`. | Tính lúc chạy trong `DuAnChiTietViewModel`. | Không. |
| `AiDatasetService.BuildFeatureSnapshotsAsync` | `XacDinhDuAnTre` trả true nếu hoàn thành trễ, dự án chưa hoàn thành quá hạn, có công việc trễ, tỷ lệ công việc trễ >= 30%, hoặc `SoNgayTreTienDo > 0`. | Khi tổng hợp chính thức thì lưu `AI_DATASET.LaDuAnTre`, `SoCongViecTre`, `SoNgayTreTienDo`; khi snapshot tạm thì tính lúc chạy. | Không. |
| `DashboardService` | Đếm dự án hoàn thành đúng/trễ, công việc trễ, top dự án trễ. | Tính lúc chạy cho dashboard. | Không. |
| `DanhGiaDuAnService` | Dùng dữ liệu AI/dataset và ngày hoàn thành để phân biệt `HoanThanhDungHan`, `HoanThanhTreHan`, `DangThucHienQuaHan` trong màn đánh giá. | Tính trong module đánh giá/AI. | Không. |

### 5.2. Đối chiếu 10 trường hợp bắt buộc

| Trường hợp | Source có xử lý không | Điều kiện/source | Kết quả lưu hay runtime | Có vào ViewModel danh sách | Có hiển thị ở bảng | Ảnh hưởng `TrangThaiDuAn` |
| --- | --- | --- | --- | --- | --- | --- |
| 1. Đang thực hiện nhưng ngày hiện tại > ngày kết thúc dự kiến | Có ở chi tiết và AI, không ở danh sách | `DuAnService.GetChiTietAsync`: `IsQuaHan`; `AiDatasetService.XacDinhDuAnTre`: `quaHanDuAn` | Runtime chi tiết; AI có thể lưu `LaDuAnTre` khi tổng hợp | Không | Không | Không đổi, vẫn `DangThucHien`. |
| 2. Đã hoàn thành nhưng ngày hoàn thành thực tế > ngày kết thúc dự kiến | Có ở AI/dashboard/đánh giá, không ở danh sách | `AiDatasetService.XacDinhDuAnTre`; `DashboardService` đếm `DuAnHoanThanhTreHan`; `DuAnHoanThanhDungHan` chỉ true nếu hoàn thành <= dự kiến | Runtime hoặc AI dataset | Danh sách có ngày hoàn thành trong VM nhưng bảng không dùng; không có cờ trễ | Không | Không đổi, vẫn `HoanThanh`; chỉ ảnh hưởng `CanArchive` vì hoàn thành trễ không được lưu trữ. |
| 3. Chưa quá ngày kết thúc nhưng có công việc trễ | Có ở chi tiết và AI | `CongViecTreHan` trong `GetChiTietAsync`; `AiDatasetService` đếm `soCongViecTre` | Runtime chi tiết; AI có thể lưu | Không | Không | Không đổi trạng thái dự án. |
| 4. Có công việc trễ nhưng trạng thái vẫn `Đang thực hiện` | Có khả năng xảy ra | Workflow chỉ dựa trạng thái công việc, không dựa deadline; bảng chỉ render `TrangThaiDuAn` | Không lưu cờ riêng trong `DU_AN` | Không | Không, chỉ thấy badge `Đang thực hiện` | Không đổi sang trễ hạn. |
| 5. Tạm dừng nhưng đã vượt ngày kết thúc | Chi tiết đánh dấu quá hạn vì chỉ loại trừ hoàn thành/lưu trữ; AI cũng có thể xem là trễ nếu không hoàn thành | `GetChiTietAsync` không loại trừ `TamDung` khỏi `IsQuaHan` | Runtime chi tiết | Không | Không | Không đổi, vẫn `TamDung`. |
| 6. Chờ xác nhận hoàn thành nhưng đã vượt ngày kết thúc | Chi tiết đánh dấu quá hạn; AI có thể xem là trễ | `GetChiTietAsync` không loại trừ `ChoXacNhanHoanThanh`; `XacDinhDuAnTre` `quaHanDuAn` khi chưa hoàn thành | Runtime chi tiết/AI | Không | Không | Không đổi, vẫn `ChoXacNhanHoanThanh`. |
| 7. Hoàn thành đúng hạn | Có | `DuAnHoanThanhDungHan` trong `DuAnService`; `DashboardService` | Runtime | Không có cờ trong danh sách | Không phân biệt, chỉ `Hoàn thành` | Cho phép `CanArchive = true`. |
| 8. Hoàn thành trễ | Có ở AI/dashboard/đánh giá; gián tiếp trong dự án qua `CanArchive = false` | `NgayHoanThanhThucTeDuAn.Date > NgayKetThucDuAn.Date` | Runtime/AI dataset | Không | Không phân biệt, chỉ `Hoàn thành` | Không đổi trạng thái; không cho lưu trữ. |
| 9. Không có ngày kết thúc | Form create/update yêu cầu ngày kết thúc; entity vẫn nullable | `DuAnCreateUpdateViewModel` có `[Required]`; nếu dữ liệu cũ null thì chi tiết không tính `IsQuaHan` | Không | Có `NgayKetThucDuAn` nullable nhưng không cờ trễ | Bảng render rỗng trong cột thời gian | Không đổi. |
| 10. Đã lưu trữ hoặc đã hủy | Có helper loại trừ trong workflow/AI | `TrangThaiWorkflowService` bỏ qua `LuuTru/DaHuy`; chi tiết không đánh quá hạn nếu `LuuTru`, nhưng không loại trừ `DaHuy`; danh sách vẫn lọc được nếu có dữ liệu | Runtime | Không | Chỉ badge `Lưu trữ`; `DaHuy` rơi vào màu trung tính nếu có | Không đổi. |

### 5.3. Kết luận về `Đang thực hiện` và `Trễ hạn`

| Câu hỏi | Kết luận AS-IS |
| --- | --- |
| `Đang thực hiện` và `Trễ hạn` độc lập hay dùng chung? | Độc lập. `Đang thực hiện` là trạng thái workflow trong `DU_AN.TrangThaiDuAn`; trễ/quá hạn là logic tính theo ngày/công việc ở chi tiết, dashboard, AI. |
| Hệ thống có xem `Trễ hạn` là trạng thái dự án không? | Không thấy trạng thái dự án `Trễ hạn`. Có constant `TrangThai.TreTienDo = "Tre"` nhưng không dùng trong workflow dự án. |
| ViewModel danh sách có `LaDuAnTre`, `IsDelayed`, `SoNgayTre` không? | Không. `DuAnViewModel` chỉ có thông tin cơ bản, tiến độ, số team/thành viên, ngân sách. |
| Có sử dụng `AI_DATASET.LaDuAnTre` cho bảng dự án không? | Không. `DuAnService.GetPagedAsync` không join `AI_DATASET`. |
| Logic AI và logic giao diện dự án có dùng chung nguồn tính trễ không? | Không. AI có `AiDatasetService.XacDinhDuAnTre`; chi tiết dự án có `IsQuaHan`/`CongViecTreHan`; bảng danh sách không dùng cả hai. |
| Dự án đang thực hiện nhưng quá hạn hiện thể hiện thế nào trên giao diện danh sách? | Vẫn là badge trạng thái `Đang thực hiện`, ngày kết thúc hiển thị bình thường trong cột `Thời gian`; không có badge phụ/cảnh báo/số ngày trễ. |

## 6. Phân tích bảng danh sách dự án hiện tại

`Views/DuAn/Index.cshtml` gọi partial `Views/DuAn/_Table.cshtml`. Bảng nằm trong `div.du-an-table-scroll`, table class `table workflow-table app-data-table du-an-table`.

Các cột theo thứ tự:

| Cột giao diện | Thuộc tính nguồn | Cách hiển thị | Responsive | Điều kiện đặc biệt |
| --- | --- | --- | --- | --- |
| Tên dự án | `DuAnViewModel.TenDuAn` | Text đậm `fw-semibold` | Không có class ẩn; nằm trong bảng `min-width: 1120px` và cuộn ngang | Không hiển thị mã/mô tả. |
| Loại | `TenLoaiDuAn` | Text thường | Không ẩn | Lấy từ join `LoaiDuAn`. |
| Ngày tạo | `NgayTaoDuAn` | `dd/MM/yyyy HH:mm` | Class `col-date`, `white-space: nowrap` từ `site.css` | Null render rỗng. |
| Thời gian | `NgayBatDauDuAn`, `NgayKetThucDuAn` | `dd/MM/yyyy - dd/MM/yyyy` | Class `col-date`, không ẩn, bảng cuộn ngang | Không đổi màu khi quá hạn. |
| Trạng thái | `TrangThaiDuAn` | Badge `du-an-status-badge @LayCssTrangThai(...)`, text `TrangThai.ToDisplay` | Class `col-status`, nowrap | Không kiểm tra ngày/công việc trễ. |
| Nhóm/Thành viên | `SoLuongTeam`, `SoLuongThanhVien`, `PhanTramHoanThanh`, `HasApprovedBudget` | Dòng `team / thành viên`; badge tiến độ; badge ngân sách | Class `col-count`, nowrap cho cột; badge có `white-space: nowrap` | Tiến độ chỉ theo số phần trăm, không nói trễ. |
| Thao tác | Quyền và `MaDuAn` | Các nút Chi tiết, Nhóm phụ trách, Danh mục công việc, Thành viên, Đề xuất công việc, Đề xuất ngân sách, Sửa, Xóa | `col-actions` min-width 254px; nút nowrap; cuộn ngang | Nút hiện theo quyền claim và scope danh mục công việc. |

Hiện trạng cụ thể:

| Chủ đề | AS-IS |
| --- | --- |
| Ngày tháng | Danh sách dùng `dd/MM/yyyy HH:mm` cho ngày tạo, `dd/MM/yyyy` cho ngày bắt đầu/kết thúc. |
| Phần trăm tiến độ | Hiển thị badge theo `PhanTramHoanThanh`: >=100 `is-done`, >=70 `is-high`, >=40 `is-medium`, còn lại `is-low`. |
| Người quản lý | `DuAnViewModel` có `TenNguoiQuanLy`, nhưng query danh sách gán `string.Empty` và bảng không có cột quản lý. |
| Trạng thái | Dùng helper local `LayCssTrangThai` và `TrangThai.ToDisplay`. |
| Dropdown | Không có dropdown thao tác; các nút hiển thị trực tiếp theo từng quyền. |
| JavaScript trong bảng | Inline function `handleDeXuatCongViecClick` chỉ cảnh báo nếu chưa có ngân sách duyệt khi bấm đề xuất công việc. |
| Responsive | Không dùng `table-responsive` Bootstrap mà dùng `.du-an-table-scroll { overflow-x: auto; }`; table `min-width: 1120px`. Không có cột ẩn bằng `d-none`, `d-md-table-cell`. |
| Phân trang | Có `_Pagination`, giữ query string, hỗ trợ đổi `pageSize`. |
| Lọc/tìm kiếm | Server-side qua query string, không có lọc client. |
| Số ngày còn lại/trễ | Không hiển thị. |
| Cảnh báo quá hạn | Không có trong bảng danh sách. |

## 7. Cách hiển thị trạng thái hiện tại

Trong `_Table.cshtml`, helper local:

```text
LayCssTrangThai(trangThai):
- DangThucHien -> is-active
- ChoXacNhanHoanThanh -> is-pending
- LaHoanThanhCongViec -> is-done
- TamDung -> is-paused
- KhoiTao -> is-init
- còn lại -> is-neutral
```

CSS trong `wwwroot/css/DuAn/index.css`:

| Class | Ý nghĩa/màu |
| --- | --- |
| `.du-an-status-badge.is-active` | Nền xanh nhạt, chữ xanh: dùng cho `Đang thực hiện`. |
| `.du-an-status-badge.is-pending` | Nền vàng nhạt: dùng cho `Chờ xác nhận hoàn thành`. |
| `.du-an-status-badge.is-done` | Nền xanh lá nhạt: dùng cho hoàn thành. |
| `.du-an-status-badge.is-paused` | Nền xám: dùng cho tạm dừng. |
| `.du-an-status-badge.is-init` | Nền tím/xanh nhạt: dùng cho khởi tạo. |
| `.du-an-status-badge.is-neutral` | Nền xám trung tính: trạng thái không map, ví dụ `DaHuy` nếu xuất hiện. |

Nhận xét kỹ thuật:

| Nội dung | Hiện trạng |
| --- | --- |
| Có dùng helper chuẩn không | Có dùng `TrangThai.EqualsValue`, `TrangThai.LaHoanThanhCongViec`, `TrangThai.ToDisplay`. |
| Có so sánh chuỗi trực tiếp không | Có một số đoạn dùng `string.Equals(..., StringComparison.OrdinalIgnoreCase)` trong service cho trạng thái hiện tại/mục tiêu sau khi đã `ToCode`, nhưng bảng dùng helper. |
| Nguy cơ lệch mã/tiếng Việt | Đã giảm nhờ `Normalize`, `GetCommonStatusVariants`, `ToDisplay`; tuy nhiên trạng thái không nằm trong helper sẽ rơi về chính chuỗi nguồn hoặc màu `is-neutral`. |
| Dự án đang thực hiện nhưng quá hạn có badge khác không | Không. Vẫn `is-active` và text `Đang thực hiện`. |
| CSS cảnh báo có tồn tại nhưng chưa dùng ở bảng không | Có `.btn-danger-soft`, `badge-soft-danger` trong site, các class warning/deadline ở `details.css`; nhưng bảng danh sách không dùng class trễ/quá hạn cho dòng/cột trạng thái. |

## 8. Bộ lọc và tìm kiếm dự án

Bộ lọc nằm ở `_Filter.cshtml`, xử lý server-side trong `DuAnController.Index` và `DuAnService.GetPagedAsync`.

| Bộ lọc | Query string | Nơi xử lý | Hiện trạng |
| --- | --- | --- | --- |
| Từ khóa | `tuKhoa` | `GetPagedAsync` | Tìm theo `TenDuAn`, `TenLoaiDuAn`, `MoTaDuAn`. |
| Loại dự án | `locMaLoaiDuAn` | `GetPagedAsync` | So sánh `MaLoaiDuAn`. |
| Trạng thái | `locTrangThaiDuAn` | `GetPagedAsync` | Dùng `TrangThai.GetCommonStatusVariants` rồi `Contains`. |
| Lọc theo ngày | `locTheoNgay` | `GetPagedAsync` | `NgayTao`, `NgayBatDau`, `NgayKetThuc`. |
| Từ ngày/đến ngày | `tuNgay`, `denNgay` | `GetPagedAsync` | Chuẩn hóa ngày, nếu đảo khoảng thì swap; `denNgay` dùng `< denNgay + 1`. |
| Dự án trễ | Không có | Không có | Không có filter quá hạn/công việc trễ. |
| Sắp xếp | Không có tham số | `GetPagedAsync` | Luôn `OrderByDescending(MaDuAn)`. |
| Page size | `pageSize` | `_Pagination`, `GetPagedAsync` | Hỗ trợ theo `PaginationViewModel.AllowedPageSizes`. |

Không có sắp xếp theo ngày kết thúc, không đưa dự án quá hạn lên trước, không có filter dự án hoàn thành trễ.

## 9. ViewModel danh sách dự án

`DuAnPageViewModel`:

| Thuộc tính | Vai trò |
| --- | --- |
| `Pagination` | Dữ liệu phân trang. |
| `DanhSach` | Danh sách `DuAnViewModel`. |
| `Form` | Form tạo/sửa trong cùng trang. |
| `DanhSachLoaiDuAn` | Select loại dự án. |
| `TuKhoa`, `LocMaLoaiDuAn`, `LocTrangThaiDuAn`, `TuNgay`, `DenNgay`, `LocTheoNgay` | Giữ bộ lọc. |
| `Permissions` | Điều kiện hiển thị form/nút. |

`DuAnViewModel` dùng cho bảng:

| Thuộc tính | Nguồn | Loại | Dùng cho giao diện |
| --- | --- | --- | --- |
| `MaDuAn` | `DU_AN` | Trực tiếp | Route action. |
| `TenDuAn` | `DU_AN` | Trực tiếp | Cột tên. |
| `MoTaDuAn` | `DU_AN` | Trực tiếp | Tìm kiếm/export, không render bảng. |
| `MaNguoiDung` | `DU_AN` | Trực tiếp | Không render. |
| `TenNguoiQuanLy` | Hiện danh sách gán rỗng | Thiếu dữ liệu thực tế | Không render. |
| `MaLoaiDuAn`, `TenLoaiDuAn` | `DU_AN`, `LOAI_DU_AN` | Trực tiếp/join | Cột loại, filter. |
| `NgayTaoDuAn`, `NgayBatDauDuAn`, `NgayKetThucDuAn`, `NgayHoanThanhThucTeDuAn` | `DU_AN` | Trực tiếp | Bảng dùng tạo/bắt đầu/kết thúc, không dùng ngày hoàn thành thực tế. |
| `PhanTramHoanThanh` | `DU_AN.PhanTramHoanThanh ?? 0` | Trực tiếp từ cột đã đồng bộ | Badge tiến độ. |
| `TrangThaiDuAn` | `DU_AN.TrangThaiDuAn` | Trực tiếp | Badge trạng thái. |
| `SoLuongTeam`, `SoLuongThanhVien` | Count subquery | Tính trong query | Cột nhóm/thành viên. |
| `HasApprovedBudget` | `NganSach.Any(...)` | Tính trong query | Badge ngân sách. |

Dữ liệu còn thiếu để giao diện phân biệt dự án trễ:

| Dữ liệu thiếu | Vì sao thiếu |
| --- | --- |
| Cờ `LaDuAnTre`/`IsDelayed` cho danh sách | `DuAnViewModel` không có thuộc tính tương đương. |
| `SoNgayTre` hoặc `SoNgayConLai` | Chỉ có trong `DuAnChiTietViewModel`, không có trong `DuAnViewModel`. |
| Số công việc trễ | Chỉ có `DuAnWorkStatusSummaryViewModel.CongViecTreHan` ở chi tiết, không có danh sách. |
| Phân biệt hoàn thành đúng hạn/trễ | `NgayHoanThanhThucTeDuAn` có trong VM nhưng bảng không dùng và không có cờ diễn giải. |
| Nguồn AI `LaDuAnTre` | Không join `AI_DATASET`, không gọi `AiDatasetService`. |

## 10. Query EF Core lấy danh sách dự án

`DuAnService.GetPagedAsync`:

| Thành phần | Hiện trạng |
| --- | --- |
| Bảng chính | `_context.DuAn` join `_context.LoaiDuAn`. |
| Soft-delete | `where da.IsDeleted != true`. |
| Projection | Project trực tiếp sang `DuAnViewModel`. |
| Count tổng | `await query.CountAsync()` trước paging. |
| Sắp xếp/paging | `OrderByDescending(x => x.MaDuAn).Skip(...).Take(...).ToListAsync()`. |
| Scope Manager | `query.Where(x => x.MaNguoiDung == currentUserId)`. |
| Scope Employee | `query.Where(x => _context.NhanVienDuAn.Any(nv => nv.MaDuAn == x.MaDuAn && nv.MaNguoiDung == currentUserId))`. |
| Tính team/thành viên | Subquery `_context.TeamDuAn.Count`, `_context.NhanVienDuAn.Count` trong projection. |
| Tính ngân sách | Subquery `_context.NganSach.Any` trong projection. |
| Tính công việc trễ | Không có. |
| Tính tiến độ | Lấy `da.PhanTramHoanThanh ?? 0`, không tính lại từ công việc trong query. |
| N+1 | Không có vòng lặp sau materialization cho bảng; subquery count/any nằm trong SQL projection, EF thường dịch thành correlated subquery. |
| Tải toàn bộ rồi phân trang | `GetPagedAsync` không tải toàn bộ trước khi paging. `GetAllAsync` dùng cho export/nhánh lỗi form thì tải toàn bộ theo filter. |
| Hàm khó translate | Query có `ToLower().Contains`, `filterValues.Contains`, các subquery Count/Any. Không thấy `string.Format`, interpolation, `AsEnumerable`, hoặc `ToListAsync` trước paging trong `GetPagedAsync`. |

Rủi ro hiện trạng:

| Rủi ro | Mức |
| --- | --- |
| `TenNguoiQuanLy` trong danh sách không được join, nên nếu cần hiển thị quản lý phải bổ sung query đúng cách. | Trung bình |
| Nếu bổ sung trễ hạn bằng cách tính sau paging thì cần tránh N+1; nếu tính trong SQL cần dùng biểu thức translate được. | Cao |
| `DateTime.Today`/`DateTime.Now` trong query cần cân nhắc translate và timezone nếu đưa vào EF query. | Trung bình |

## 11. Quyền và phạm vi xem dự án

| Nhóm | Xem danh sách | Thêm | Sửa/workflow | Xóa | Chi tiết |
| --- | --- | --- | --- | --- | --- |
| Admin | Nếu có `DuAn.Xem`, thấy toàn bộ dự án không xóa mềm vì không bị nhánh Manager/Employee lọc. | Nếu có `DuAn.Them`; dự án mới gán quản lý là user hiện tại qua `GetCurrentUserIdAsync`. | Có nút nếu có `DuAn.Sua`, nhưng service yêu cầu `MaNguoiDung == currentUserId`; Admin không tự bypass trong `DuAnService`. | Có nút nếu có `DuAn.Xoa`, service vẫn yêu cầu là quản lý và chỉ khởi tạo/chưa phát sinh dữ liệu. | Nếu có `DuAn.Xem`. |
| Manager quản lý dự án | Nếu có `DuAn.Xem`, chỉ thấy dự án `MaNguoiDung` của mình. | Nếu có `DuAn.Them`. | Nếu có `DuAn.Sua` và là quản lý dự án. | Nếu có `DuAn.Xoa`, là quản lý, dự án khởi tạo và không liên quan. | Có. |
| Leader team phụ trách | Không có scope riêng trong danh sách dự án; phải thuộc `NhanVienDuAn` nếu role Employee. | Theo quyền claim nhưng service tạo/sửa vẫn gắn/kiểm tra quản lý. | Không có quyền workflow riêng nếu không phải quản lý. | Không. | Có nếu danh sách thấy/route truy cập và có quyền. |
| Thành viên dự án | Nếu role Employee và thuộc `NhanVienDuAn`, thấy dự án. | Theo claim, nhưng service thao tác quản lý sẽ chặn nếu không phải quản lý. | Không nếu không phải quản lý. | Không nếu không phải quản lý. | Có nếu `DuAn.Xem`. |
| Nhân viên không thuộc dự án | Không thấy khi role Employee. | Không đủ scope thao tác dự án khác. | Không. | Không. | Không theo danh sách; route chi tiết hiện chỉ kiểm tra quyền `DuAn.Xem`, service `GetChiTietAsync` không áp dụng scope danh sách nên đây là điểm cần chú ý nếu route bị đoán. |

Điều kiện hiển thị nút ở bảng chủ yếu dựa trên quyền claim, riêng nút `Danh mục công việc` gọi thêm `DanhMucCongViecScopeService.CanAccessProjectAsync`. Các nút workflow chi tiết/form dựa vào `ProjectStatusCheckViewModel`, trạng thái hiện tại và quyền `DuAn.Sua`.

## 12. JavaScript, CSS và responsive

File dùng trực tiếp:

| File | Vai trò |
| --- | --- |
| `wwwroot/css/DuAn/index.css` | Layout trang danh sách, filter, bảng, badge trạng thái/tiến độ/ngân sách, action button, responsive grid. |
| `wwwroot/css/DuAn/details.css` | Trang chi tiết: hero, cảnh báo, deadline, workflow, AI panel, ngân sách. |
| `wwwroot/css/site.css` | Style chung table `app-data-table`, `col-date`, `col-status`, `col-count`, `col-actions`, `badge-soft-danger`. |
| `wwwroot/css/shared/export-dropdown.css` | Dropdown xuất file trong header bảng. |
| `wwwroot/js/site.js`, `wwwroot/js/layout/sidebar.js` | Script chung layout, không có logic trễ hạn dự án. |
| Inline script `_Table.cshtml` | `handleDeXuatCongViecClick` cảnh báo ngân sách khi đi đề xuất công việc. |
| Inline script `_Form.cshtml` | Ẩn/hiện ghi chú tạm dừng khi chọn trạng thái `TamDung`. |

Responsive hiện tại:

| Nội dung | Hiện trạng |
| --- | --- |
| Bảng | `.du-an-table-scroll` cuộn ngang, `.du-an-table` `min-width: 1120px`. |
| Cột bị ẩn | Không có class responsive ẩn cột. |
| Ngày/trạng thái | `.app-data-table .col-date`, `.col-status`, `.col-count` dùng `white-space: nowrap`. |
| Action | `.col-actions` min-width 254px; nút nowrap. |
| Nguy cơ che khuất | Trên màn nhỏ phải cuộn ngang; nếu người dùng không cuộn, cột trạng thái/thời gian có thể nằm ngoài vùng nhìn thấy. Không có cảnh báo sticky/summary ngoài bảng. |
| Dark mode | Không thấy cơ chế dark mode riêng cho module dự án. |

## 13. Các điểm chưa phù hợp với nhu cầu nhận biết dự án trễ

| Điểm | Bằng chứng source | Ảnh hưởng người dùng | Mức độ |
| --- | --- | --- | --- |
| Bảng chỉ hiển thị trạng thái workflow, không hiển thị trễ hạn | `_Table.cshtml` cột trạng thái chỉ dùng `TrangThaiDuAn` và `LayCssTrangThai` | Dự án `Đang thực hiện` quá hạn trông giống dự án còn hạn. | Cao |
| Không có badge phụ `Trễ hạn`/`Quá hạn` | `DuAnViewModel` không có cờ trễ; `_Table.cshtml` không tính ngày | Người dùng phải mở chi tiết hoặc dashboard/AI để biết. | Cao |
| Không tính số ngày trễ trong danh sách | `SoNgayConLai`, `IsQuaHan` chỉ có `DuAnChiTietViewModel` | Không biết mức độ trễ ngay trên bảng. | Cao |
| Công việc trễ chưa tổng hợp lên bảng | `CongViecTreHan` chỉ set trong `DuAnWorkStatusSummaryViewModel` ở `GetChiTietAsync` | Dự án chưa quá hạn nhưng có công việc trễ không lộ trên danh sách. | Cao |
| Không phân biệt hoàn thành đúng hạn/trễ trong bảng | `NgayHoanThanhThucTeDuAn` có trong VM nhưng `_Table.cshtml` không dùng; `DashboardService` có logic riêng | Dự án hoàn thành trễ vẫn chỉ là `Hoàn thành`. | Trung bình |
| Không có bộ lọc dự án trễ | `_Filter.cshtml` chỉ có keyword, loại, trạng thái, ngày | Không thể lọc nhanh nhóm cần xử lý. | Cao |
| Không ưu tiên dự án quá hạn | `GetPagedAsync` luôn `OrderByDescending(MaDuAn)` | Dự án rủi ro có thể nằm ở trang sau. | Trung bình |
| Ngày kết thúc hiển thị như ngày thường | `_Table.cshtml` cột `Thời gian` không có class cảnh báo | Người dùng phải tự so sánh ngày. | Trung bình |
| Logic AI không dùng cho bảng | `DuAnService.GetPagedAsync` không join/call `AiDatasetService`; `AiDatasetService` có `LaDuAnTre` riêng | Nguồn AI có thể biết dự án trễ nhưng danh sách không phản ánh. | Cao |
| CSS cảnh báo có ở chi tiết nhưng không áp dụng bảng | `details.css` có `.deadline-state.over`, `.warning-list`; `index.css` chỉ badge trạng thái workflow | Cảnh báo chỉ xuất hiện sau khi vào chi tiết. | Trung bình |
| Quản lý dự án không hiển thị trong bảng | `TenNguoiQuanLy = string.Empty` trong danh sách; bảng không có cột quản lý | Khó biết ai chịu trách nhiệm dự án trễ. | Thấp/Trung bình |

## 14. Các ràng buộc cần giữ khi chỉnh sửa sau này

- Không thay đổi workflow trạng thái dự án nếu chỉ bổ sung nhận biết dự án trễ.
- Không tự chuyển `DangThucHien` thành trạng thái mới như `TreHan`; source hiện tại không xem trễ là trạng thái dự án.
- Không dùng `AI_DATASET.LaDuAnTre` làm trạng thái nghiệp vụ của `DU_AN.TrangThaiDuAn`.
- Nếu dùng AI/dataset để tham khảo, phải phân biệt rõ dữ liệu AI với trạng thái workflow.
- Không làm thay đổi quyền claim và scope dữ liệu hiện tại nếu chưa có yêu cầu riêng.
- Không phá phân trang: lọc/sắp xếp mới phải chạy trước `Count/Skip/Take` nếu là server-side.
- Không tạo N+1 khi tổng hợp số công việc trễ/ngày trễ cho danh sách.
- Không dùng biểu thức EF Core khó dịch như format chuỗi, `ToString` ngày, `StringComparison` trong query SQL.
- Không sửa schema, migration, Entity hoặc thêm cột SQL ở bước giao diện nếu chưa được yêu cầu.
- Không làm bảng quá nhiều chữ giải thích; cần chỉ báo ngắn, rõ.
- Không làm hỏng responsive: bảng hiện dựa vào cuộn ngang và `min-width`.
- Giữ UTF-8 tiếng Việt.
- Không thay đổi chức năng thêm, sửa, xóa, tạm dừng, hoàn thành, mở lại, lưu trữ hiện có.
- Phải giữ ranh giới trạng thái workflow (`TrangThaiDuAn`) và đặc điểm trễ hạn.

## 15. Danh sách file liên quan đã đọc

### Controller

| File | Vai trò |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs` | Entry point danh sách, chi tiết, CRUD, workflow, file, export, AI nguyên nhân trễ. |
| `QuanLyDuAn/QuanLyDuAn/Controllers/TienDoCongViecController.cs` | Entry point báo cáo/duyệt tiến độ, liên quan đồng bộ trạng thái chi tiết công việc. |
| `QuanLyDuAn/QuanLyDuAn/Controllers/AiController.cs` | Bề mặt phân tích/huấn luyện AI nguyên nhân trễ. |
| `QuanLyDuAn/QuanLyDuAn/Controllers/AiDatasetController.cs` | Bề mặt dataset AI, export có cột `LaDuAnTre`. |

### Service

| File | Vai trò |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs` | Query danh sách/chi tiết, CRUD, workflow dự án, tính cảnh báo chi tiết. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TrangThaiWorkflowService.cs` | Đồng bộ trạng thái chi tiết công việc -> công việc -> dự án, cập nhật phần trăm dự án. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs` | Workflow công việc, xác nhận/mở lại công việc và gọi đồng bộ dự án. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChiTietCongViecService.cs` | CRUD chi tiết công việc và các điểm gọi đồng bộ trạng thái. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TienDoCongViecService.cs` | Báo cáo tiến độ, duyệt trạng thái đề xuất, khóa cập nhật theo trạng thái dự án/công việc/chi tiết. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs` | Tính snapshot AI, `LaDuAnTre`, `SoCongViecTre`, `SoNgayTreTienDo`. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiService.cs` | Phân tích nguyên nhân trễ và xác nhận nhãn AI. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DashboardService.cs` | Thống kê dự án/công việc trễ, top dự án trễ, hoàn thành đúng/trễ hạn. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs` | Dữ liệu hỗ trợ đánh giá, AI insight và phân loại tình trạng tiến độ. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhMucCongViecScopeService.cs` | Scope cho nút/điều hướng danh mục công việc từ bảng dự án. |

### Interface

| File | Vai trò |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDuAnService.cs` | Contract danh sách, chi tiết, CRUD và workflow dự án. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/ITrangThaiWorkflowService.cs` | Contract đồng bộ trạng thái. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/ICongViecService.cs` | Contract công việc. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IChiTietCongViecService.cs` | Contract chi tiết công việc. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/ITienDoCongViecService.cs` | Contract tiến độ công việc. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IAiDatasetService.cs` | Contract dataset/snapshot AI. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IAiService.cs` | Contract phân tích AI. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDashboardService.cs` | Contract dashboard. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IPermissionHelper.cs` | Kiểm tra quyền ở Controller/View. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IPhanQuyenService.cs` | Lấy tập quyền người dùng. |

### Entity

| File | Vai trò |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/DuAn.cs` | Entity `DU_AN`. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/CongViec.cs` | Entity công việc, có ngày dự kiến/thực tế và trạng thái công việc. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/CtCongViec.cs` | Entity chi tiết công việc. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/TienDoCongViec.cs` | Entity báo cáo tiến độ chi tiết công việc. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/AiDataset.cs` | Entity dataset AI, có `LaDuAnTre`, `SoCongViecTre`, `SoNgayTreTienDo`. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/AiKetQua.cs` | Kết quả AI. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/AiNguyenNhan.cs` | Nhãn nguyên nhân AI đã xác nhận. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/LoaiDuAn.cs` | Loại dự án. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/NganSach.cs` | Ngân sách dự án. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/ChiPhi.cs` | Chi phí. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/NhanVienDuAn.cs` | Thành viên dự án và vai trò trong dự án. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/TeamDuAn.cs` | Team phụ trách dự án. |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/NhatKyQuanLyDuAn.cs` | Nhật ký workflow dự án. |

### ViewModel

| File | Vai trò |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnPageViewModel.cs` | VM trang danh sách và form. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnViewModel.cs` | VM từng dòng bảng danh sách. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnCreateUpdateViewModel.cs` | VM form tạo/sửa, validation ngày/trạng thái. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnChiTietViewModel.cs` | VM chi tiết dự án, có `IsQuaHan`, `SoNgayConLai`, tổng hợp công việc. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/ProjectStatusCheckViewModel.cs` | Điều kiện workflow và quyền nút chuyển trạng thái. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnWorkStatusSummaryViewModel.cs` | Tổng hợp công việc trong chi tiết, có `CongViecTreHan`. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnDeadlinePreviewViewModel.cs` | Deadline công việc gần nhất trong chi tiết. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnBudgetSummaryViewModel.cs` | Tổng hợp ngân sách/chi phí chi tiết. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnRecentWorkItemViewModel.cs` | Công việc gần đây. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnRecentFileViewModel.cs` | Tệp gần đây. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnMemberPreviewViewModel.cs` | Thành viên nổi bật. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnActivityPreviewViewModel.cs` | Hoạt động gần đây. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiDelayAnalysisViewModels.cs` | Panel AI nguyên nhân trễ trong chi tiết dự án. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/Common/PaginationViewModel.cs` | Cấu hình phân trang và page size. |

### Razor View

| File | Vai trò |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Index.cshtml` | Trang danh sách, thống kê trang hiện tại, form, filter, bảng, phân trang. |
| `QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Table.cshtml` | Bảng danh sách dự án và nút hành động. |
| `QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Filter.cshtml` | Bộ lọc danh sách. |
| `QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Form.cshtml` | Form tạo/sửa và nút workflow trong màn danh sách. |
| `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml` | Trang chi tiết dự án, cảnh báo quá hạn, công việc trễ, AI. |
| `QuanLyDuAn/QuanLyDuAn/Views/DuAn/ChiTiet.cshtml` | View chi tiết cũ/khác còn trong thư mục. |
| `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Pagination.cshtml` | Phân trang dùng query string. |
| `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml` | Layout, menu theo quyền, include CSS/JS chung. |
| `QuanLyDuAn/QuanLyDuAn/Views/Shared/_ExportDropdown.cshtml` | Dropdown xuất file dùng trong `Index`. |

### Constant/Helper

| File | Vai trò |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs` | Mã trạng thái, tên hiển thị, normalize/compare/status variants. |
| `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs` | Tên quyền claim. |
| `QuanLyDuAn/QuanLyDuAn/Helpers/ExportSupport.cs` | Format filter/ngày cho export. |

### JavaScript

| File | Vai trò |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/js/site.js` | Script chung. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/js/layout/sidebar.js` | Sidebar responsive/collapse. |
| Inline script trong `Views/DuAn/_Table.cshtml` | Cảnh báo ngân sách khi mở đề xuất công việc. |
| Inline script trong `Views/DuAn/_Form.cshtml` | Bật/tắt ghi chú tạm dừng. |

### CSS

| File | Vai trò |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/DuAn/index.css` | CSS trang danh sách dự án. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/DuAn/details.css` | CSS trang chi tiết dự án. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/site.css` | CSS chung, bảng, badge, layout. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/export-dropdown.css` | CSS dropdown export. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/ui.css` | Thành phần UI dùng chung. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/layout/sidebar.css` | Sidebar/menu. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/layout/account-menu.css` | Menu tài khoản. |

### DbContext

| File | Vai trò |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs` | DbSet và mapping bảng/cột/FK, gồm `DU_AN`, `AI_DATASET`, `CONG_VIEC`, `CT_CONG_VIEC`, `TIEN_DO_CONG_VIEC`. |

### Module liên quan đến tính trễ

| File | Vai trò |
| --- | --- |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs` | Nguồn tính `LaDuAnTre`, `SoNgayTreTienDo`, `SoCongViecTre` cho AI. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DashboardService.cs` | Thống kê dự án/công việc trễ và hoàn thành trễ. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs` | Diễn giải tình trạng tiến độ/trễ trong đánh giá dự án. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TrangThaiWorkflowService.cs` | Đồng bộ tiến độ/trạng thái, không tạo trạng thái trễ hạn. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TienDoCongViecService.cs` | Duyệt báo cáo tiến độ làm nguồn trạng thái chính thức cho chi tiết công việc. |

## Tự kiểm tra trước khi hoàn tất

| Mục | Kết quả |
| --- | --- |
| Đã đọc Controller, Service, ViewModel, View | Đã đọc các file chính và partial bảng/lọc/form. |
| Đã phân tích bảng danh sách dự án | Đã phân tích `_Table.cshtml`, CSS `index.css`, `site.css`. |
| Đã xác định logic dự án trễ | Có ở chi tiết, AI dataset, dashboard/đánh giá; không có ở bảng danh sách. |
| Đã phân biệt `Đang thực hiện` với `Trễ hạn` | Đã ghi rõ là độc lập. |
| Đã kiểm tra đang thực hiện quá ngày kết thúc | Có trong chi tiết/AI, không ở bảng. |
| Đã kiểm tra hoàn thành trễ | Có trong AI/dashboard và ảnh hưởng lưu trữ, không ở bảng. |
| Đã kiểm tra công việc trễ tổng hợp lên dự án | Có ở chi tiết/AI/dashboard, không lên danh sách. |
| Đã kiểm tra bộ lọc/phân trang | Đã ghi query string, page size, giữ filter qua `_Pagination`. |
| Đã kiểm tra quyền/scope | Đã ghi claim và scope Manager/Employee/service manager permission. |
| Đã kiểm tra CSS/badge/responsive | Đã ghi màu badge, cuộn ngang, không cột ẩn. |
| Không chỉnh sửa source/DB | Chỉ tạo `docs/duan.md`. |
| UTF-8 | File này được tạo bằng nội dung tiếng Việt UTF-8. |

## 16. Kết quả triển khai nhận biết dự án trễ

### File đã sửa

| Nhóm | File | Nội dung |
| --- | --- | --- |
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs` | Thêm tham số `locTinhTrangThoiHan` cho danh sách, sửa, thao tác workflow, chi tiết và export; giữ filter khi redirect/phân trang/xuất file. |
| Interface | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDuAnService.cs` | Bổ sung tham số lọc tình trạng thời hạn cho `GetAllAsync` và `GetPagedAsync`. |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs` | Tính tình trạng thời hạn khi lấy danh sách/chi tiết; lọc server-side trước `CountAsync`; gom số công việc trễ theo nhóm dự án. |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnViewModel.cs` | Thêm cờ/tên/CSS tình trạng thời hạn và số ngày/số công việc trễ. |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnPageViewModel.cs` | Thêm `LocTinhTrangThoiHan`. |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnChiTietViewModel.cs` | Thêm dữ liệu tình trạng thời hạn để trang chi tiết dùng cùng logic với danh sách. |
| Helper | `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnDeadlineStatusHelper.cs` | Helper dùng chung để xác định `Trễ`, `Quá hạn`, `Hoàn thành trễ`, `Hoàn thành đúng hạn`, `Còn hạn`, `Chưa xác định`, `Không đánh giá`. |
| Razor View | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Index.cshtml` | Truyền filter thời hạn sang bảng, form, export và phân trang. |
| Razor View | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Filter.cshtml` | Thêm bộ lọc `Tình trạng thời hạn`. |
| Razor View | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Table.cshtml` | Hiển thị badge thời hạn dưới badge workflow và đánh dấu nhẹ ngày kết thúc khi trễ/quá hạn. |
| Razor View | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Form.cshtml` | Giữ `LocTinhTrangThoiHan` qua form lưu và các thao tác workflow trong form. |
| Razor View | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml` | Hiển thị chip thời hạn và dùng dữ liệu tính chung cho cảnh báo hoàn thành trễ/công việc trễ. |
| CSS | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/DuAn/index.css` | Thêm style badge thời hạn, màu trễ/công việc trễ/đúng hạn/chưa xác định và chỉnh grid filter. |
| CSS | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/DuAn/details.css` | Thêm style chip thời hạn trong trang chi tiết. |

### Logic sau triển khai

- `TrangThaiDuAn` vẫn chỉ là trạng thái workflow; không thêm trạng thái nghiệp vụ mới vào Entity hoặc cơ sở dữ liệu.
- Dự án chưa kết thúc (`Khởi tạo`, `Đang thực hiện`, `Tạm dừng`, `Chờ xác nhận hoàn thành`) có `NgayKetThucDuAn < DateTime.Today` sẽ hiển thị `Trễ X ngày` hoặc `Quá hạn X ngày`.
- Dự án `Hoàn thành` hoặc `Lưu trữ` dùng `NgayHoanThanhThucTeDuAn` so với `NgayKetThucDuAn`; không tiếp tục tăng ngày trễ theo ngày hiện tại.
- Dự án hoàn thành đúng hạn hiển thị `Hoàn thành đúng hạn`; hoàn thành trễ hiển thị `Hoàn thành trễ X ngày`.
- Dự án `Đã hủy` hiển thị `Không đánh giá`; dự án không có ngày kết thúc hiển thị `Chưa xác định`.
- Công việc trễ được tính từ `CONG_VIEC` qua `DANH_MUC_CONG_VIEC`: công việc chưa hoàn thành quá hạn dự kiến hoặc công việc đã hoàn thành trễ so với ngày dự kiến.
- Thứ tự ưu tiên hiển thị: dự án quá hạn, hoàn thành trễ, có công việc trễ, hoàn thành đúng hạn, còn hạn, chưa xác định/không đánh giá.

### ViewModel và bộ lọc

- `DuAnViewModel` bổ sung `IsQuaHan`, `IsHoanThanhTre`, `IsHoanThanhDungHan`, `CoCongViecTre`, `IsConHan`, `IsChuaXacDinh`, `IsKhongDanhGia`, `SoNgayTre`, `SoCongViecTre`, `MaTinhTrangThoiHan`, `TinhTrangThoiHan`, `CssTinhTrangThoiHan`.
- Bộ lọc `locTinhTrangThoiHan` có các lựa chọn: `Đang quá hạn`, `Có công việc trễ`, `Hoàn thành trễ`, `Hoàn thành đúng hạn`, `Còn hạn`.
- Bộ lọc xử lý trong `DuAnService.GetPagedAsync` trước `CountAsync`, `Skip`, `Take`; `_Pagination.cshtml` giữ query string nên chuyển trang vẫn giữ filter.

### EF Core và tránh N+1

- Không join `AI_DATASET` và không dùng `AI_DATASET.LaDuAnTre`.
- `GetPagedAsync` vẫn áp dụng scope quyền, filter từ khóa/loại/trạng thái/ngày/thời hạn trước `CountAsync`.
- Sau khi lấy đúng một trang dự án, service dùng một query group theo `MaDuAn` để lấy `SoCongViecTre`, rồi gán vào ViewModel trong bộ nhớ.
- Không gọi query riêng cho từng dòng dự án để tính tình trạng thời hạn.

### Kiểm tra

- Đã chạy `dotnet build QuanLyDuAn\QuanLyDuAn\QuanLyDuAn.csproj --no-restore`: build thành công.
- Build còn 2 warning cũ ở `FileTienDoCongViecService.cs` về async method không có `await`; không phát sinh lỗi build từ thay đổi này.
- Đã kiểm tra bằng code các trường hợp: đang thực hiện quá hạn, tạm dừng quá hạn, chờ xác nhận quá hạn, hoàn thành đúng hạn, hoàn thành trễ, chưa quá hạn nhưng có công việc trễ, đã hủy, không có ngày kết thúc.
- Không sửa schema, migration, Entity mapping, DbContext hoặc dữ liệu cơ sở dữ liệu.
