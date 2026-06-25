# Phân tích hiện trạng chức năng Chi tiết dự án

Tài liệu này ghi nhận hiện trạng source code mới nhất của chức năng **Chi tiết dự án** trong dự án `QuanLyDuAnAI`. Source code là nguồn sự thật; các tài liệu `docs/duan.md`, `docs/congviec.md`, `docs/ctcongviec.md` chỉ dùng để đối chiếu. Nhiệm vụ này chỉ đọc source và tạo tài liệu, không chỉnh sửa Controller, Service, View, CSS, JavaScript, database, schema, migration, workflow hoặc nghiệp vụ.

## Mục lục

1. [Route và bề mặt chức năng Chi tiết dự án](#1-route-và-bề-mặt-chức-năng-chi-tiết-dự-án)
2. [Quyền và phạm vi dữ liệu](#2-quyền-và-phạm-vi-dữ-liệu)
3. [Thông tin tổng quan dự án](#3-thông-tin-tổng-quan-dự-án)
4. [Tình trạng thời hạn của dự án](#4-tình-trạng-thời-hạn-của-dự-án)
5. [Thống kê Công việc trong Chi tiết dự án](#5-thống-kê-công-việc-trong-chi-tiết-dự-án)
6. [Thống kê Công việc hoàn thành đúng hạn và hoàn thành trễ](#6-thống-kê-công-việc-hoàn-thành-đúng-hạn-và-hoàn-thành-trễ)
7. [Đánh giá sự đầy đủ của thống kê hiện tại](#7-đánh-giá-sự-đầy-đủ-của-thống-kê-hiện-tại)
8. [Đề xuất nhóm thống kê cho bước chỉnh sửa sau](#8-đề-xuất-nhóm-thống-kê-cho-bước-chỉnh-sửa-sau)
9. [Thống kê ngân sách và chi phí](#9-thống-kê-ngân-sách-và-chi-phí)
10. [Thành viên và nhóm phụ trách](#10-thành-viên-và-nhóm-phụ-trách)
11. [Tài liệu dự án](#11-tài-liệu-dự-án)
12. [Panel AI nguyên nhân trễ](#12-panel-ai-nguyên-nhân-trễ)
13. [Phân tích giao diện và responsive](#13-phân-tích-giao-diện-và-responsive)
14. [Query, hiệu năng và nguy cơ N+1](#14-query-hiệu-năng-và-nguy-cơ-n1)
15. [Các rủi ro làm sai thống kê](#15-các-rủi-ro-làm-sai-thống-kê)
16. [Danh sách file có khả năng cần sửa sau này](#16-danh-sách-file-có-khả-năng-cần-sửa-sau-này)
17. [Kết luận bắt buộc](#17-kết-luận-bắt-buộc)
18. [Tự kiểm tra](#18-tự-kiểm-tra)
19. [Kết quả triển khai thống kê Chi tiết dự án](#19-kết-quả-triển-khai-thống-kê-chi-tiết-dự-án)
20. [Phân tích trường hợp Dự án hoàn thành trễ nhưng Công việc đúng hạn 100%](#20-phân-tích-trường-hợp-dự-án-hoàn-thành-trễ-nhưng-công-việc-đúng-hạn-100)

## 1. Route và bề mặt chức năng Chi tiết dự án

### 1.1. Route/action hiện có

| Route | HTTP method | Controller action | Service method | View | Được gọi từ đâu |
| --- | --- | --- | --- | --- | --- |
| `/DuAn` hoặc `/DuAn/Index` | GET | `DuAnController.Index` | `IDuAnService.GetPagedAsync` | `Views/DuAn/Index.cshtml` | Sidebar `_Layout.cshtml`, các nút quay lại từ module liên quan. |
| `/DuAn/Details/{id}` hoặc `/DuAn/Details?id=...` | GET | `DuAnController.Details` | `IDuAnService.GetChiTietAsync` | `Views/DuAn/Details.cshtml` | Nút `Chi tiết` trong `Views/DuAn/_Table.cshtml`, Chat dự án, Yêu cầu đổi quản lý. |
| `/DuAn/ChiTiet/{id}` hoặc `/DuAn/ChiTiet?id=...` | GET | `DuAnController.ChiTiet` | Gọi lại `Details(...)` | Thực tế trả về `Views/DuAn/Details.cshtml` | Không thấy link nội bộ trỏ tới `DuAn/ChiTiet`; action tồn tại như alias tương thích. |
| `/DuAn/ThemFileDuAn` | POST | `ThemFileDuAn` | `IFileDuAnService.UploadAsync` | Redirect `Details` | Form upload trong `Details.cshtml`. |
| `/DuAn/TaiFileDuAn` | GET | `TaiFileDuAn` | `IFileDuAnService.GetDownloadInfoAsync` | `PhysicalFile` hoặc redirect `Details` | Danh sách file trong `Details.cshtml`. |
| `/DuAn/XoaFileDuAn` | POST | `XoaFileDuAn` | `IFileDuAnService.DeleteAsync` | Redirect `Details` | Form xóa file trong `Details.cshtml`. |
| `/DuAn/PhanTichNguyenNhanTre` | POST | `PhanTichNguyenNhanTre` | `IAiService.PhanTichNguyenNhanDuAnAsync`, sau đó `GetChiTietAsync` | `View("Details", vm)` | Panel AI trong `Details.cshtml`. |
| `/DuAn/XacNhanNguyenNhanTre` | POST | `XacNhanNguyenNhanTre` | `IAiService.XacNhanNguyenNhanAsync` | Redirect `Details` | Panel AI trong `Details.cshtml`. |
| `/DuAn/XacNhanHoanThanh` | POST | `XacNhanHoanThanh` | `IDuAnService.ConfirmCompletionAsync` | Redirect `Index` | Nút workflow ở chi tiết và danh sách/form. |
| `/DuAn/MoLaiDuAn` | POST | `MoLaiDuAn` | `IDuAnService.MoLaiDuAnAsync` | Redirect `Index` | Modal mở lại ở chi tiết và danh sách/form. |
| `/DuAn/BatDauDuAn` | POST | `BatDauDuAn` | `IDuAnService.TransitionToDangThucHienAsync` | Redirect `Index` | Form/danh sách dự án. |
| `/DuAn/TamDungDuAn` | POST | `TamDungDuAn` | `IDuAnService.PauseProjectAsync` | Redirect `Index` | Form/danh sách dự án. |
| `/DuAn/YeuCauHoanThanh` | POST | `YeuCauHoanThanh` | `IDuAnService.RequestCompletionAsync` | Redirect `Index` | Form/danh sách dự án. |
| `/DuAn/XuatFile` | GET | `XuatFile` | `IDuAnService.GetAllAsync` | File export | Dropdown export trong `Index.cshtml`. |

### 1.2. `Details.cshtml` và `ChiTiet.cshtml`

| Nội dung | Hiện trạng |
| --- | --- |
| `/DuAn/Details/{id}` có tồn tại không | Có. `DuAnController.Details` kiểm tra `Permissions.DuAn.Xem`, gọi `GetChiTietAsync`, gán ngữ cảnh quyền/AI rồi `return View(vm)`. View mặc định là `Views/DuAn/Details.cshtml`. |
| `/DuAn/ChiTiet/{id}` có tồn tại không | Có. `DuAnController.ChiTiet` chỉ `return await Details(...)`, không gọi `View("ChiTiet")`. |
| Action nào trả `Details.cshtml` | `Details`; `PhanTichNguyenNhanTre` cũng trả `View("Details", vm)` khi phân tích xong. `ChiTiet` gián tiếp trả `Details`. |
| Action nào trả `ChiTiet.cshtml` | Không tìm thấy action nào trả trực tiếp `Views/DuAn/ChiTiet.cshtml`. |
| Nút `Chi tiết` ở danh sách dẫn đến đâu | `Views/DuAn/_Table.cshtml` dùng `asp-action="Details"`, `asp-route-id="@duAn.MaDuAn"`. |
| Dashboard/module khác dẫn đến đâu | `Views/ChatDuAn/_ChatHeader.cshtml`, `Views/YeuCauDoiQuanLy/Create.cshtml`, `Views/YeuCauDoiQuanLy/Index.cshtml` trỏ `DuAn/Details`; sidebar trỏ `DuAn/Index`. |
| Hai trang có trùng chức năng không | Không. `Details.cshtml` dùng `DuAnChiTietViewModel` và là màn hình chi tiết hiện đại; `ChiTiet.cshtml` dùng `DuAnCreateUpdateViewModel`, layout card đơn giản, không có thống kê, file, AI, workflow chi tiết. |
| Trang cũ không còn sử dụng | `Views/DuAn/ChiTiet.cshtml` có dấu hiệu là view cũ/không còn được route hiện tại trả về. |
| Nguy cơ không thống nhất | Có. File `ChiTiet.cshtml` còn tồn tại với model khác nên người bảo trì dễ hiểu nhầm có hai màn chi tiết. Tuy nhiên runtime hiện đi qua `Details.cshtml`. |

## 2. Quyền và phạm vi dữ liệu

### 2.1. Permission theo action/nút

| Bề mặt | Permission | File/class/method bằng chứng | Nhận xét |
| --- | --- | --- | --- |
| Xem danh sách và chi tiết | `Permissions.DuAn.Xem` | `DuAnController.Index`, `Details`, `ChiTiet`, `TaiFileDuAn` | Controller kiểm tra quyền claim trước khi gọi service. |
| Sửa dự án/workflow/upload/xóa file | `Permissions.DuAn.Sua` | `Sua`, `LuuDuAn`, `BatDauDuAn`, `TamDungDuAn`, `YeuCauHoanThanh`, `XacNhanHoanThanh`, `MoLaiDuAn`, `ThemFileDuAn`, `XoaFileDuAn` | Service workflow còn kiểm tra người dùng là quản lý dự án. |
| Tạo dự án | `Permissions.DuAn.Them` | `LuuDuAn` khi `model.MaDuAn == null` | Không thuộc chi tiết nhưng liên quan form danh sách. |
| Xóa dự án | `Permissions.DuAn.Xoa` | `XoaDuAn` | Xóa mềm và chỉ khi không có dữ liệu liên quan. |
| Xuất file | `Permissions.ThongKe.XuatFile` | `XuatFile` | Xuất danh sách, không phải export chi tiết dự án. |
| Phân tích nguyên nhân trễ | `Permissions.AI.PhanTichNguyenNhan` | `PhanTichNguyenNhanTre`; `AiService.LayPhanTichNguyenNhanDuAnAsync` | Panel còn yêu cầu manager hiện tại và dự án có điều kiện trễ. |
| Xác nhận nguyên nhân trễ | `Permissions.AI.XacNhan` | `XacNhanNguyenNhanTre`; `AiService.XacNhanNguyenNhanAsync` | Chỉ xác nhận khi dự án trễ theo dataset chính thức và có kết quả hợp lệ. |
| Xem tài chính | `NganSach.Xem`, `DeXuatNganSach.Xem`, `ChiPhi.Xem` | Biến `canSeeBudget`, `canSeeChiPhi`, `canSeeFinance` trong `Details.cshtml` | Quyền điều khiển khu vực tài chính và link nhanh. |
| Xem công việc/chi tiết/team/thành viên | Các quyền module tương ứng | `Details.cshtml` biến `canSeeTasks`, `canSeeWorkDetails`, `canSeeTeams`, `canSeeMembers` | Điều khiển link nhanh và preview. |
| Xem hoạt động | `Permissions.NhatKy.Xem` | `Details.cshtml` biến `canSeeActivity` | Chỉ ảnh hưởng panel hoạt động gần đây. |

### 2.2. Data scope theo role

| Vai trò | Danh sách `GetPagedAsync`/`GetAllAsync` | Chi tiết `GetChiTietAsync` | Nhận xét rủi ro |
| --- | --- | --- | --- |
| Admin | Không bị lọc bởi nhánh Manager/Employee, xem mọi `DU_AN.IsDeleted != true`. | Chỉ lọc `MaDuAn == id && IsDeleted != true`, nên Admin có thể xem mọi dự án nếu có quyền `DuAn.Xem`. | Phù hợp với Admin nếu quyền được cấp. |
| Manager | Danh sách lọc `MaNguoiDung == currentUserId`. | Không kiểm tra `MaNguoiDung == currentUserId`. | Có rủi ro: Manager biết `MaDuAn` có thể vào chi tiết dự án ngoài phạm vi nếu có `DuAn.Xem`. |
| Employee | Danh sách lọc tồn tại `NhanVienDuAn` theo user. | Không kiểm tra người dùng thuộc dự án. | Có rủi ro tương tự: Employee biết `MaDuAn` có thể xem chi tiết dự án ngoài phạm vi nếu có `DuAn.Xem`. |
| Leader | Không có nhánh riêng trong `DuAnService`; nếu role Employee vẫn dựa vào `NhanVienDuAn`. | Không có scope leader ở `GetChiTietAsync`; một số link Danh mục dùng `IDanhMucCongViecScopeService`. | Scope xem chi tiết dự án không nhất quán với scope danh sách. |

Kết luận quyền/scope: controller kiểm tra permission, nhưng `GetChiTietAsync` chưa áp dụng scope giống danh sách. Đây là vấn đề thực tế nên đưa vào bước chỉnh sửa sau, độc lập với thống kê thời hạn.

## 3. Thông tin tổng quan dự án

| Thông tin | Property ViewModel | Nguồn dữ liệu | Cách tính | Vị trí hiển thị |
| --- | --- | --- | --- | --- |
| Tên dự án | `TenDuAn` | `DU_AN.TenDuAn` | Projection trong `GetChiTietAsync` | Hero title. |
| Loại dự án | `TenLoaiDuAn`, `MaLoaiDuAn` | Join `DU_AN` - `LOAI_DU_AN` | Lấy `loai.TenLoai` | Chip hero, thông tin chính. |
| Mô tả | `MoTaDuAn` | `DU_AN.MoTaDuAn` | Lấy trực tiếp | Card `Mô tả dự án`. |
| Quản lý dự án | `MaNguoiDung`, `TenNguoiQuanLy` | `DU_AN.MaNguoiDung`, `NGUOI_DUNG.HoTenNguoiDung` | Query riêng theo `MaNguoiDung` | `Thông tin chính`; dùng kiểm tra nút workflow/file. |
| Ngày tạo | `NgayTaoDuAn` | `DU_AN.NgayTaoDuAn` | Lấy trực tiếp | `Thông tin chính`. |
| Ngày bắt đầu | `NgayBatDauDuAn` | `DU_AN.NgayBatDauDuAn` | Lấy trực tiếp | `Thông tin chính`. |
| Ngày kết thúc dự kiến | `NgayKetThucDuAn` | `DU_AN.NgayKetThucDuAn` | Lấy trực tiếp | `Thông tin chính`, helper thời hạn, cảnh báo. |
| Ngày hoàn thành thực tế | `NgayHoanThanhThucTeDuAn` | `DU_AN.NgayHoanThanhThucTeDuAn` | Set ở `ConfirmCompletionAsync`; xóa ở `MoLaiDuAnAsync` | `Thông tin chính`; phân biệt với ngày dự kiến. |
| Trạng thái workflow | `TrangThaiDuAn` | `DU_AN.TrangThaiDuAn` | `TrangThai.ToDisplay` khi hiển thị | Chip hero, workflow card. |
| Tình trạng thời hạn | `TinhTrangThoiHan`, `CssTinhTrangThoiHan` | `NgayKetThucDuAn`, `NgayHoanThanhThucTeDuAn`, `TrangThaiDuAn`, `SoCongViecTre` | `DuAnDeadlineStatusHelper.Apply` | Chip hero, cảnh báo. |
| Phần trăm hoàn thành | `PhanTramHoanThanh` | `DU_AN.PhanTramHoanThanh` | Đồng bộ bởi `TrangThaiWorkflowService`; hiển thị trực tiếp | Hero progress. |
| Ghi chú | `GhiChuDuAn` | `DU_AN.GhiChuDuAn` | Lưu từ form/tạm dừng/mở lại | `Thông tin chính`. |
| Số team | `SoLuongTeam` | `TEAM_DU_AN` | `CountAsync(x => x.MaDuAn == id)` | `Thống kê nhanh`. |
| Số thành viên | `SoLuongThanhVien` | `NHAN_VIEN_DU_AN` | `CountAsync(x => x.MaDuAn == id)` | `Thống kê nhanh`. |
| Số Công việc | `SoLuongCongViec`, `TienDoCongViec.TongCongViec` | `CONG_VIEC` join `DANH_MUC_CONG_VIEC` | Lọc `dm.MaDuAn == id`, `dm.IsDeleted != true`, `cv.IsDeleted != true` | `Thống kê nhanh`, `Tiến độ công việc`. |
| Số Chi tiết công việc | `SoLuongChiTietCongViec` | `CT_CONG_VIEC` join công việc dự án | Lọc `ct.IsDeleted != true` | `Thống kê nhanh`. |
| Ngân sách | `NganSachTongHop.TongNganSachDaDuyet` | `NGAN_SACH` | Active, not deleted, trạng thái đã duyệt | `Tài chính dự án`. |
| Chi phí | `NganSachTongHop.TongChiPhiDaDung` | `CHI_PHI` join `NGAN_SACH` | Sum `SoTienDaChi` theo ngân sách dự án | `Tài chính dự án`. |
| File dự án | `DanhSachFile`, `TepGanDay` | `FILE_DU_AN` | Lọc `MaDuAn`, `IsDeleted != true`, sort upload desc | `Tài liệu dự án`, preview `5 tệp mới tải lên`. |

Đánh giá: màn hình đã hiển thị khá đầy đủ thông tin dự án, gồm cả ngày dự kiến và ngày hoàn thành thực tế. Điểm thiếu chính không nằm ở thông tin dự án, mà nằm ở thống kê kết quả Công việc: chưa có card riêng `Hoàn thành đúng hạn` và `Hoàn thành trễ`.

## 4. Tình trạng thời hạn của dự án

`TrangThaiDuAn` là trạng thái workflow. `Tình trạng thời hạn` là dữ liệu tính trong `DuAnDeadlineStatusHelper.Apply`, không ghi vào `DU_AN.TrangThaiDuAn`.

| Trường hợp | Source hiện xử lý | ViewModel | Giao diện | Nhận xét |
| --- | --- | --- | --- | --- |
| Dự án chưa bắt đầu và còn hạn | `Apply` thấy chưa kết thúc, không trễ, không có công việc trễ | `IsConHan`, `TinhTrangThoiHan = "Còn hạn"` | Chip `Thời hạn: Còn hạn` | Có xử lý. |
| Dự án chưa bắt đầu nhưng đã quá hạn | `today > NgayKetThucDuAn` và chưa hoàn thành/hủy/lưu trữ | `IsQuaHan`, `SoNgayTre` | Chip đỏ, cảnh báo `Đã trễ hạn` | Có xử lý nhưng text dùng chung với trạng thái đang thực hiện. |
| Dự án đang thực hiện và còn hạn | Tương tự còn hạn; nếu có `SoCongViecTre > 0` thì ưu tiên `công việc trễ` | `IsConHan` hoặc `CoCongViecTre` | Chip xanh hoặc cảnh báo công việc trễ | Có xử lý. |
| Dự án đang thực hiện nhưng quá hạn | `today > NgayKetThucDuAn` | `IsQuaHan`, `SoNgayTre` | Chip `Trễ X ngày`, warning | Có xử lý. |
| Dự án tạm dừng và quá hạn | Prefix trong helper là `Quá hạn` nếu `TamDung` | `IsQuaHan` | Chip/warning | Có xử lý, vẫn đánh giá quá hạn. |
| Dự án chờ xác nhận và quá hạn | Prefix trong helper là `Quá hạn` nếu `ChoXacNhanHoanThanh` | `IsQuaHan` | Chip/warning | Có xử lý. |
| Dự án hoàn thành đúng hạn | `LaHoanThanhCongViec` hoặc `LuuTru`, có ngày thực tế <= dự kiến | `IsHoanThanhDungHan` | Chip `Hoàn thành đúng hạn` | Có xử lý. |
| Dự án hoàn thành trễ | Có ngày thực tế > ngày dự kiến | `IsHoanThanhTre`, `SoNgayTre` | Chip/warning `Hoàn thành trễ X ngày` | Có xử lý. |
| Dự án đã hủy | Helper đặt `Không đánh giá` | `IsKhongDanhGia` | Chip neutral | Có xử lý nếu trạng thái tồn tại. |
| Dự án lưu trữ | Helper xem như trạng thái đã kết thúc | `IsHoanThanhDungHan` hoặc `IsHoanThanhTre` nếu có ngày | Chip theo kết quả | Có xử lý. |
| Dự án thiếu ngày dự kiến | Helper đặt `Chưa xác định` | `IsChuaXacDinh` | Chip unknown | Có xử lý. |
| Dự án hoàn thành nhưng thiếu ngày thực tế | Nếu có `SoCongViecTre > 0` thì hiển thị công việc trễ, ngược lại `Chưa xác định` | `IsChuaXacDinh` hoặc `CoCongViecTre` | Chip unknown/work-late | Có xử lý fallback, nhưng không xác định đúng/trễ dự án. |

## 5. Thống kê Công việc trong Chi tiết dự án

### 5.1. Query lấy Công việc

`DuAnService.GetChiTietAsync` tạo `queryCongViec` từ `_context.CongViec` join `_context.DanhMucCongViec`, lọc:

```text
dm.MaDuAn == id
dm.IsDeleted != true
cv.IsDeleted != true
```

Các thống kê trong chi tiết dự án đều lấy từ toàn bộ `queryCongViec`, không phải từ danh sách 5 công việc gần đây.

### 5.2. Chỉ số hiện có

| Chỉ số | Property | Cách tính | Có tính toàn bộ hay giới hạn | Có hiển thị | Phù hợp trạng thái nào |
| --- | --- | --- | --- | --- | --- |
| Tổng số Công việc | `SoLuongCongViec`, `TienDoCongViec.TongCongViec` | `queryCongViec.CountAsync()` | Toàn bộ dự án | Có | Mọi trạng thái. |
| Chưa bắt đầu | `CongViecChuaBatDau` | `TrangThai.GetCommonStatusVariants(ChuaBatDau)` | Toàn bộ | Có | Hữu ích khi dự án chưa/đang thực hiện. |
| Đang thực hiện | `CongViecDangThucHien` | `TrangThai.GetCommonStatusVariants(DangThucHien)` | Toàn bộ | Có | Hữu ích khi dự án đang thực hiện. |
| Bị cản trở | Không có property riêng trong `DuAnWorkStatusSummaryViewModel` | `CheckProjectStatusAsync` có `HasBlockedTasks`; chi tiết không có count riêng | Không có count riêng | Không có card riêng | Thiếu nếu muốn biết số lượng bị cản trở. |
| Chờ xác nhận hoàn thành | Không có property riêng | Workflow service có trạng thái, nhưng chi tiết không count | Không có count riêng | Không | Thiếu nếu muốn xem backlog chờ xác nhận. |
| Hoàn thành | `CongViecHoanThanh` | Trạng thái `HoanThanh`, `Hoàn thành`, `Done`, `Completed` | Toàn bộ | Có | Mọi trạng thái; khi dự án hoàn thành sẽ bằng tổng. |
| Tạm dừng | `CongViecTamDung` | `TrangThai.GetCommonStatusVariants(TamDung)` | Toàn bộ | Có | Hữu ích khi dự án đang thực hiện/tạm dừng. |
| Đã hủy | Không có property riêng | Không đếm riêng | Không có | Không | Thiếu nếu dữ liệu có công việc hủy. |
| Công việc đang quá hạn | Không có property riêng | Gộp trong `CongViecTreHan` nếu chưa hoàn thành và hạn < hôm nay | Toàn bộ | Có, nhưng nhãn chung `Trễ hạn` | Cần tách rõ với hoàn thành trễ. |
| Công việc hoàn thành trễ | Không có property riêng | Gộp trong `CongViecTreHan` nếu hoàn thành và ngày thực tế > dự kiến | Toàn bộ | Có, nhưng nhãn chung `Trễ hạn` | Thiếu card riêng. |
| Công việc sắp đến hạn | Không có property riêng | Không thấy logic count sắp đến hạn | Không có | Không | Thiếu nếu cần điều hành hiện tại. |
| Tỷ lệ hoàn thành | `TiLeHoanThanh`, `TiLeHoanThanhCap` | `CongViecHoanThanh / TongCongViec * 100` | Toàn bộ | Có | Mọi trạng thái. |
| Deadline gần nhất | `DeadlineGanNhat` | Công việc chưa hoàn thành có `NgayKetThucCVDuKien`, order asc, first | Toàn bộ, lấy 1 item | Có | Hữu ích khi chưa hoàn thành; không có khi tất cả đã hoàn thành. |

### 5.3. Điểm quan trọng

- Source có dùng `TrangThai.GetCommonStatusVariants` cho các trạng thái phổ biến và `TrangThai.LaHoanThanhCongViec` ở nhiều module.
- `CongViecTreHan` trong `GetChiTietAsync` hiện là chỉ số gộp:
  - công việc hoàn thành trễ: `TrangThaiCongViec` thuộc nhóm hoàn thành, có `NgayKetThucCVThucTe`, và `NgayKetThucCVThucTe.Date > NgayKetThucCVDuKien.Date`;
  - công việc chưa hoàn thành đang quá hạn: không thuộc nhóm hoàn thành và `NgayKetThucCVDuKien.Date < DateTime.Today`.
- Vì gộp hai ý nghĩa vào một property, giao diện không phân biệt được Công việc đang quá hạn với Công việc đã hoàn thành trễ.
- `CongViecGanDay` chỉ lấy 5 công việc theo `NgayTaoCongViec` giảm dần, hiển thị tên, trạng thái, danh mục; chưa hiển thị hạn, ngày hoàn thành, hay tình trạng đúng/trễ.

### 5.4. Danh sách Công việc gần đây

| Cột/Thông tin | Hiện có | Nguồn | Thiếu gì |
| --- | --- | --- | --- |
| Tên công việc | Có | `DuAnRecentWorkItemViewModel.TenCongViec` | Không có link trực tiếp đến item công việc. |
| Danh mục công việc | Có | `TenDanhMucCV` | Không có lọc danh mục kèm link. |
| Trạng thái workflow | Có | `TrangThaiCongViec` | Không có badge thời hạn riêng. |
| Ngày tạo | Có trong ViewModel | `NgayTaoCongViec` | Không hiển thị trong `Details.cshtml`. |
| Hạn dự kiến | Có trong ViewModel | `NgayKetThucDuKien` | Không hiển thị trong danh sách gần đây. |
| Ngày hoàn thành thực tế | Không có property | `CongViec.NgayKetThucCVThucTe` chưa project | Cần bổ sung nếu muốn nhận biết hoàn thành trễ ngay tại preview. |
| Tình trạng đúng/trễ | Không | Chưa tính | Cần bổ sung nếu giữ preview này làm hỗ trợ đánh giá. |

## 6. Thống kê Công việc hoàn thành đúng hạn và hoàn thành trễ

### 6.1. Khả năng tính từ dữ liệu hiện có

Điều kiện có thể tính chính xác ở cấp Công việc:

```text
Công việc hoàn thành đúng hạn:
TrangThaiCongViec thuộc nhóm hoàn thành
và NgayKetThucCVThucTe.Date <= NgayKetThucCVDuKien.Date
```

```text
Công việc hoàn thành trễ:
TrangThaiCongViec thuộc nhóm hoàn thành
và NgayKetThucCVThucTe.Date > NgayKetThucCVDuKien.Date
```

| Chỉ số | Có đủ dữ liệu | Source đã tính ở đâu | Chi tiết dự án đang hiển thị | Có cần sửa DB |
| --- | --- | --- | --- | --- |
| Số Công việc hoàn thành đúng hạn | Có nếu có đủ 2 ngày | `CongViecService` list đã có logic filter `HoanThanhDungHan`; `DanhGiaDuAnService` tách logic đúng/trễ; `DashboardService` có logic thời gian cho dự án/công việc | Chưa hiển thị | Không |
| Số Công việc hoàn thành trễ | Có nếu có đủ 2 ngày | `GetChiTietAsync` đang gộp vào `CongViecTreHan`; `DanhGiaDuAnService` có `CongViecHoanThanhTreHan`; `CongViecService` có filter `HoanThanhTre` | Chưa hiển thị riêng | Không |
| Số Công việc hoàn thành thiếu dữ liệu ngày | Có thể xác định | Chưa thấy tính riêng ở chi tiết | Chưa hiển thị | Không |
| Tỷ lệ hoàn thành đúng hạn | Có thể tính | Chưa thấy trong chi tiết | Chưa hiển thị | Không |
| Tỷ lệ hoàn thành trễ | Có thể tính | `DanhGiaDuAnService` có `TyLeCongViecTreHan` nhưng gộp đang trễ + hoàn thành trễ | Chưa hiển thị riêng | Không |
| Tổng số ngày trễ của Công việc hoàn thành | Có thể tính từ chênh lệch ngày | Chưa thấy tính ở chi tiết | Chưa hiển thị | Không |
| Số ngày trễ trung bình | Có thể tính | Chưa thấy tính ở chi tiết | Chưa hiển thị | Không |
| Số ngày trễ lớn nhất | Có thể tính | Chưa thấy tính ở chi tiết | Chưa hiển thị | Không |
| Công việc hoàn thành trễ nhiều nhất | Có thể tính | Chưa thấy tính ở chi tiết | Chưa hiển thị | Không |

### 6.2. Độ tin cậy của `NgayKetThucCVThucTe`

| Câu hỏi | Kết luận theo source |
| --- | --- |
| Ngày này gán khi Công việc lên `ChoXacNhanHoanThanh` hay khi xác nhận `HoanThanh` | `TrangThaiWorkflowService.DongBoTrangThaiCongViecTheoChiTietAsync` gán `NgayKetThucCVThucTe = DateTime.Now` khi trạng thái mới là `ChoXacNhanHoanThanh` hoặc hoàn thành. `CongViecService.XacNhanHoanThanhCongViecAsync` chỉ gán nếu chưa có. |
| Có thể được gán sớm hơn thời điểm quản lý xác nhận không | Có. Khi mọi chi tiết hoàn thành và Công việc lên `ChoXacNhanHoanThanh`, ngày thực tế đã được gán trước xác nhận cuối. |
| Khi mở lại Công việc có bị xóa không | Có. `CongViecService.MoLaiCongViecAsync` set `NgayKetThucCVThucTe = null`. |
| Nếu Công việc hoàn thành nhiều lần entity có giữ lịch sử lần cũ không | Không. Entity `CongViec` chỉ có một `NgayKetThucCVThucTe`; lịch sử mở lại/xác nhận chỉ có thể suy từ nhật ký nếu có, không phải trường kết quả cuối cùng nhiều lần. |
| Có thể dùng ngày này đánh giá kết quả cuối cùng đến mức nào | Có thể dùng cho thống kê hiện trạng cuối cùng của entity, nhưng cần ghi chú đây là ngày hệ thống ghi nhận đủ điều kiện/chờ xác nhận, không nhất thiết là ngày quản lý xác nhận cuối. |

Kết luận: đủ dữ liệu để bổ sung thống kê Công việc hoàn thành đúng hạn/trễ mà không sửa database, nhưng cần ghi rõ giới hạn nghiệp vụ của `NgayKetThucCVThucTe`.

## 7. Đánh giá sự đầy đủ của thống kê hiện tại

### 7.1. Khi dự án đang thực hiện

Các chỉ số hiện có hoặc nên có:

| Nhóm | Hiện trạng | Đánh giá |
| --- | --- | --- |
| Tổng Công việc | Có | Hữu ích. |
| Chưa bắt đầu | Có | Hữu ích. |
| Đang thực hiện | Có | Hữu ích. |
| Bị cản trở | Chỉ có boolean trong `StatusCheck`, không có count/card | Thiếu nhẹ cho điều hành. |
| Chờ xác nhận | Không có count/card | Thiếu nếu cần kiểm soát việc sắp hoàn thành. |
| Đang quá hạn | Có nhưng bị gộp trong `CongViecTreHan` | Hữu ích nhưng cần tách. |
| Sắp đến hạn | Chỉ có `DeadlineGanNhat`, không có số lượng | Có thể bổ sung nếu cần cảnh báo sớm. |
| Đã hoàn thành | Có | Hữu ích. |

### 7.2. Khi dự án đã hoàn thành

Vấn đề người dùng nêu là **thiếu sót thực tế**:

- Khi dự án hoàn thành, `CongViecHoanThanh` thường bằng `TongCongViec`.
- Các card `Đang thực hiện`, `Chưa bắt đầu`, `Tạm dừng` thường bằng 0.
- `DeadlineGanNhat` không còn vì query bỏ qua công việc hoàn thành.
- Card `Trễ hạn` có thể vẫn có số, nhưng đang gộp Công việc hoàn thành trễ với Công việc đang quá hạn. Khi tất cả đã hoàn thành, số này thực chất là hoàn thành trễ, nhưng giao diện không nói rõ.
- Người dùng không biết Công việc nào hoàn thành đúng hạn.
- Người dùng không biết Công việc nào hoàn thành trễ.
- Không đánh giá được chất lượng lập kế hoạch/thực hiện sau khi dự án kết thúc.

Mức kết luận: **Thiếu và nên bổ sung**. Chưa đến mức phá workflow, nhưng là thiếu quan trọng ở màn Chi tiết dự án vì source đã có dữ liệu và các module khác đã bắt đầu tách logic này.

## 8. Đề xuất nhóm thống kê cho bước chỉnh sửa sau

### 8.1. Mô hình hai nhóm

| Nhóm | Chỉ số đề xuất | Nhận xét |
| --- | --- | --- |
| Nhóm A - Tình hình hiện tại | Tổng Công việc, Chưa bắt đầu, Đang thực hiện, Bị cản trở, Chờ xác nhận, Đang quá hạn, Sắp đến hạn | Hữu ích cho dự án chưa hoàn thành. Không nên để quá nhiều card nếu không có không gian. |
| Nhóm B - Kết quả theo thời hạn | Đã hoàn thành, Hoàn thành đúng hạn, Hoàn thành trễ, Chưa đủ dữ liệu xác định, Tỷ lệ hoàn thành đúng hạn, Số ngày trễ trung bình, Số ngày trễ lớn nhất | Hữu ích nhất khi dự án hoàn thành hoặc gần hoàn thành. |

### 8.2. Cách hiển thị nên cân nhắc

| Câu hỏi | Đề xuất |
| --- | --- |
| Có nên luôn hiển thị cả hai nhóm không | Nên hiển thị ngắn gọn cả hai nhóm, nhưng ưu tiên theo trạng thái. Dự án đang thực hiện ưu tiên Nhóm A; dự án hoàn thành ưu tiên Nhóm B. |
| Có nên ẩn card bằng 0 không | Không nên ẩn toàn bộ vì gây nhảy layout và mất so sánh; có thể làm mờ hoặc gom vào bảng phân bố. |
| Dùng card, thanh tiến độ, biểu đồ hay bảng nhỏ | Với Chi tiết dự án, nên dùng ít card chính + bảng nhỏ. Biểu đồ nên để Đánh giá dự án/Dashboard. |
| Có nên hiển thị quá nhiều chỉ số cùng lúc không | Không. Màn hiện đã nhiều khu vực. Nên chọn chỉ số có ý nghĩa hành động. |
| Chỉ số thật sự cần thiết | `Tổng Công việc`, `Đang quá hạn`, `Hoàn thành đúng hạn`, `Hoàn thành trễ`, kèm tỷ lệ đúng hạn nếu còn chỗ. |
| Chỉ số nên đưa sang Đánh giá dự án | Số ngày trễ trung bình, số ngày trễ lớn nhất, Công việc trễ nhiều nhất, phân tích nguyên nhân sâu. |

### 8.3. Phương án tối thiểu

| Chỉ số tối thiểu | Có đủ dữ liệu | Có đủ cho Chi tiết dự án không |
| --- | --- | --- |
| Tổng Công việc | Có | Có, làm mẫu số. |
| Đang quá hạn | Có | Có, phục vụ điều hành hiện tại. |
| Hoàn thành đúng hạn | Có | Có, phục vụ đánh giá kết quả. |
| Hoàn thành trễ | Có | Có, giải quyết đúng vấn đề người dùng nêu. |

Đánh giá: phương án tối thiểu này đủ tốt cho Chi tiết dự án nếu kèm bảng phân bố workflow hiện tại nhỏ. Không cần mặc định thêm quá nhiều card.

## 9. Thống kê ngân sách và chi phí

| Nội dung | Hiện trạng |
| --- | --- |
| Ngân sách được duyệt | `tongNganSachDaDuyet` sum `NGAN_SACH.SoTienNganSach`, lọc `MaDuAn`, `IsDeleted != true`, `IsActive == true`, trạng thái đã duyệt. |
| Chi phí đã dùng | `tongChiPhiDaDung` sum `CHI_PHI.SoTienDaChi` join `NGAN_SACH`, lọc `cp.IsDeleted != true`, `ns.IsDeleted != true`, `ns.MaDuAn == id`. |
| Ngân sách còn lại | `TongNganSachDaDuyet - TongChiPhiDaDung` nếu có ngân sách đã duyệt. |
| Tỷ lệ sử dụng | `TongChiPhiDaDung / TongNganSachDaDuyet * 100`, cap hiển thị 100. |
| Vượt ngân sách | `TongChiPhiDaDung > TongNganSachDaDuyet`. |
| Không có ngân sách | Hiển thị empty state `Chưa có ngân sách được duyệt`. |
| Nguy cơ tính trùng | Có thể tính trùng nếu một chi phí gắn ngân sách không active/cũ nhưng vẫn thuộc dự án, vì query chi phí không lọc `ns.IsActive == true` hay trạng thái đã duyệt. Đây là điểm cần xem lại nếu yêu cầu tài chính chính xác theo ngân sách active. |
| Khi dự án hoàn thành | Vẫn hiển thị kết quả tài chính cuối nếu có quyền. |

## 10. Thành viên và nhóm phụ trách

| Nội dung | Hiện trạng |
| --- | --- |
| Tổng số thành viên | `SoLuongThanhVien = CountAsync(NhanVienDuAn.MaDuAn == id)`. |
| Tổng số team | `SoLuongTeam = CountAsync(TeamDuAn.MaDuAn == id)`. |
| Danh sách thành viên nổi bật | `thanhVienNoiBat` join `NhanVienDuAn` - `NguoiDung`, lọc `nd.IsDeleted != true`, order `NgayThamGiaDuAn` desc, take 5. |
| Leader | Hiển thị qua `VaiTroTrongDuAn` và `TrangThai.ToDisplayVaiTroTrongDuAn`; không kiểm tra `NhanVienTeam.IsLeader` ở panel này. |
| Người quản lý | `TenNguoiQuanLy` từ `DU_AN.MaNguoiDung`. |
| Hai team/two leader | `TeamDuAn` cho phép nhiều team; màn chi tiết chỉ đếm số team, không liệt kê leader từng team. |
| Link module | Có link `TeamDuAn`, `NhanVienDuAn`, `DanhMucCongViec`, `CongViec`, `ChiTietCongViec`, `ChatDuAn`, đánh giá. |
| Trùng lặp thông tin | Số team/thành viên xuất hiện ở `Thống kê nhanh`; preview thành viên lặp lại vai trò nhưng không quá nặng. |

## 11. Tài liệu dự án

| Nội dung | Hiện trạng |
| --- | --- |
| Upload | Form `ThemFileDuAn` trong `Details.cshtml`, chỉ hiện khi `CoTheQuanLyFile`. |
| Tải xuống | `TaiFileDuAn` yêu cầu `DuAn.Xem`, gọi `GetDownloadInfoAsync`, kiểm tra `projectId == maDuAn`. |
| Xóa | `XoaFileDuAn` yêu cầu `DuAn.Sua`, service kiểm tra quản lý dự án. |
| Permission | Controller dùng `DuAn.Xem`/`DuAn.Sua`; view chỉ cho quản lý hiện tại + `DuAn.Sua` upload/xóa. |
| Data scope | Upload/xóa kiểm tra `duAn.MaNguoiDung == currentUserId`; tải xuống không kiểm tra scope ngoài quyền xem và `maDuAn` truyền vào. Nếu `Details` chưa chặn scope, tải file cũng có rủi ro theo người biết mã file/mã dự án. |
| Giới hạn loại file | `FileDuAnService.UploadAsync` không kiểm tra extension allow-list. |
| Giới hạn dung lượng | Không thấy kiểm tra kích thước ngoài `file.Length == 0`. |
| Tên file | Dùng `Path.GetFileName(file.FileName).Trim()`, lưu tên gốc vào DB. |
| Đường dẫn lưu | `wwwroot/uploads/duan/{maDuAn}/{Guid}{extension}`. |
| Chống path traversal | Lúc upload dùng `Path.GetFileName` và tên lưu Guid; download dùng đường dẫn DB qua `GetPhysicalPath`, chưa kiểm tra resolved path vẫn nằm trong `wwwroot`. |
| Khóa theo trạng thái dự án | Không thấy khóa upload/xóa khi dự án hoàn thành/lưu trữ/tạm dừng; chỉ khóa theo quản lý. |

## 12. Panel AI nguyên nhân trễ

| Nội dung | Hiện trạng |
| --- | --- |
| Điều kiện hiện panel | Panel luôn render trong `Details.cshtml`, badge mặc định `Chưa áp dụng`; nút phân tích chỉ hiện nếu `aiPanel.CoThePhanTich`. |
| Điều kiện gọi AI | `DuAnController.PhanTichNguyenNhanTre` yêu cầu `AI.PhanTichNguyenNhan`; `AiService` còn yêu cầu manager hiện tại và context trễ tạm thời/chính thức. |
| Dự án được phép phân tích | `LayPhanTichNguyenNhanDuAnAsync` đặt `CoThePhanTich = CoQuyenPhanTich && LaManagerHienTai && (LaPhanTichTamThoi || LaPhanTichChinhThuc)`. |
| Có phân tích khi dự án chưa thực sự trễ không | Không nên; context trả `Dự án không trễ` hoặc `Chưa áp dụng`. |
| Kết quả tạm thời/chính thức | Badge `Tạm thời` khi đang trễ; `Chính thức` khi hoàn thành trễ. |
| Ai được xác nhận | `CoTheXacNhan = CoQuyenXacNhan && LaManagerHienTai && LaPhanTichChinhThuc && có MaDMNguyenNhanDuDoan`. |
| Có ghi DB không | Phân tích chính thức có thể tạo/refresh `AI_DATASET` và `AI_KET_QUA`; xác nhận ghi `AI_NGUYEN_NHAN`/label dataset theo `AiService`. |
| FastAPI có ghi nghiệp vụ không | MVC service là nơi điều phối; FastAPI chỉ nhận request phân tích/model, không ghi trực tiếp `DU_AN`/workflow. |
| Ảnh hưởng trạng thái/thống kê dự án | Không đổi `TrangThaiDuAn`, không đổi thống kê vận hành. |
| UI có quá nhiều chữ không | Panel tương đối dày nhưng không phải phần dài nhất; cần giữ ngắn nếu thêm thống kê mới. |

## 13. Phân tích giao diện và responsive

| Khu vực | Desktop | Mobile | Vấn đề | Nhận xét |
| --- | --- | --- | --- | --- |
| Header/hero | Hero lớn, chip loại/trạng thái/thời hạn/tiến độ, nút workflow | Flex wrap, nút có thể xếp nhiều dòng | Nhiều chip và nút có thể chiếm cao | Vẫn rõ ràng. |
| Badge trạng thái | Workflow và thời hạn tách riêng | Giữ chip | Màu đỏ dùng cho quá hạn/hoàn thành trễ, cam cho công việc trễ | Có phân biệt dự án trễ và công việc trễ ở chip, chưa tách công việc đang trễ/hoàn thành trễ. |
| Nút quay lại/workflow | Nằm hero actions | Wrap | `MoLaiDuAn` modal chỉ giữ một phần filter hidden, thiếu `tuNgay/denNgay/locTheoNgay/locTinhTrangThoiHan` trong modal | Nhỏ nhưng có thể làm mất filter khi redirect. |
| Cảnh báo hạn | Card `Cảnh báo dự án` | Dạng list | Card đặt khá thấp sau tài chính | Nếu thêm thống kê mới nên không đẩy cảnh báo quá xa. |
| Thông tin tổng quan | Grid 4 cột, card thông tin | 2 cột ở tablet | Nhiều card nhưng đọc được | Ngày thực tế đã có. |
| Tiến độ công việc | Sidebar card, 5 dòng | Sidebar rơi xuống dưới | Thiếu `Bị cản trở`, `Chờ xác nhận`, tách đúng/trễ | Đây là khu vực chính cần chỉnh. |
| Công việc gần đây | Preview 5 item | Grid co lại | Không hiển thị hạn/ngày thực tế/tình trạng thời hạn | Dễ khiến người dùng không thấy Công việc hoàn thành trễ. |
| Tài chính | Card ngân sách/chi phí/progress | Dọc | Có thể tính chi phí theo mọi ngân sách dự án | Giao diện rõ. |
| Thành viên/team | Count + preview thành viên + link team | Dọc | Không liệt kê team/leader | Đủ cho chi tiết ngắn. |
| File | Upload/list/download/delete | Dọc | Không giới hạn loại/dung lượng | UI đủ, logic cần hardening nếu làm bảo mật. |
| Hoạt động | 2 nguồn nhật ký, take 5 | Dọc | Không bao gồm mọi loại hoạt động như tiến độ/phân công/file | Là preview, không phải audit log đầy đủ. |
| AI | Panel trong cột chính | Wrap | Có thể dài | Không ảnh hưởng workflow. |
| Link điều hướng | Quick action grid | Auto-fit | Nhiều link | Chấp nhận được. |

CSS `details.css` có class `.chip-deadline.is-late`, `.is-work-late`, `.is-on-time`, `.deadline-state.over`, `.work-item.warn`; chưa có class riêng cho `work-item completed-late` hoặc `completed-on-time`. `index.css` có badge thời hạn dự án ở danh sách. `site.css` và `ui.css` cung cấp button/table chung. `site.js` chỉ confirm POST, không có logic thống kê.

## 14. Query, hiệu năng và nguy cơ N+1

### 14.1. Query trong `GetChiTietAsync`

`GetChiTietAsync` chạy nhiều query tuần tự: project, manager, counts, first work id, file list, status check, budget checks, nhiều count trạng thái công việc, recent work, nearest deadline, budget sums, members, two activity logs. Không thấy query trong vòng lặp, nhưng số round-trip khá nhiều.

| Điểm | Hiện trạng | Nhận xét |
| --- | --- | --- |
| N+1 | Không có vòng lặp query rõ ràng trong `GetChiTietAsync` | Ổn ở mức N+1. |
| Query tuần tự | Nhiều `CountAsync` riêng cho từng trạng thái | Có thể gom group theo trạng thái sau này. |
| Tải toàn bảng | Không thấy tải toàn bộ bảng không lọc; `DanhGiaDuAnService` có nơi materialize rồi group theo dự án đã lọc | Chi tiết dự án ổn hơn nhưng nhiều round-trip. |
| `AsNoTracking` | Hầu hết query read-only không dùng `AsNoTracking` | Có thể tối ưu sau. |
| EF Core khó dịch | `queryCongViec.Select` dùng fallback chuỗi nội suy trong `CongViecGanDay`, nhưng không dùng tiếp trong `Where`; rủi ro thấp | Nếu thêm filter nên tránh helper/string interpolation trong `IQueryable`. |
| AI mỗi lần mở trang | `GanNguCanhDetailsAsync` gọi `LayPhanTichNguyenNhanDuAnAsync`; method này đọc context/kết quả, không gọi phân tích FastAPI tự động | Không phân tích AI mỗi lần mở. |
| File hoạt động | `hoatDongQuanLy` và `hoatDongPhuTrach` mỗi query take 5 rồi concat | Không N+1, nhưng hoạt động chưa toàn diện. |

### 14.2. Nếu bổ sung thống kê đúng/trễ

Nên lấy từ `queryCongViec` bằng một query materialize tối thiểu các cột:

```text
MaCongViec, TenCongViec, TrangThaiCongViec, NgayKetThucCVDuKien, NgayKetThucCVThucTe
```

Sau đó group/tính trong bộ nhớ cho đúng một dự án, hoặc dùng `GroupBy` SQL nếu cần. Không cần query từng công việc. Không cần sửa database. Không dùng `AI_DATASET.LaDuAnTre` làm nguồn thống kê vận hành.

## 15. Các rủi ro làm sai thống kê

| Rủi ro | Bằng chứng/source | Tác động |
| --- | --- | --- |
| `NgayKetThucCVThucTe` gán khi chờ xác nhận | `TrangThaiWorkflowService` gán khi trạng thái mới là `ChoXacNhanHoanThanh` | Hoàn thành trễ theo ngày này có thể sớm hơn ngày quản lý xác nhận. |
| Ngày xác nhận và ngày hoàn tất khác nhau | `CongViecService.XacNhanHoanThanhCongViecAsync` chỉ gán ngày nếu chưa có | Cần ghi chú ý nghĩa ngày. |
| Mở lại Công việc làm mất ngày cũ | `MoLaiCongViecAsync` set null | Không có lịch sử hoàn thành nhiều lần trong entity. |
| Mở lại Dự án làm mất ngày dự án cũ | `MoLaiDuAnAsync` set `NgayHoanThanhThucTeDuAn = null` | Đánh giá dự án cuối cùng mất mốc cũ nếu mở lại. |
| Công việc thiếu hạn dự kiến | `NgayKetThucCVDuKien` nullable | Cần nhóm `Chưa đủ dữ liệu xác định`. |
| Công việc hoàn thành thiếu ngày thực tế | Có thể xảy ra với dữ liệu cũ | Cần nhóm thiếu dữ liệu. |
| Biến thể trạng thái `Done`, `Completed` | `TrangThai.LaHoanThanhCongViec` hỗ trợ | Phải dùng helper/variants. |
| Soft-deleted Công việc | `queryCongViec` lọc `cv.IsDeleted != true` | Đúng hiện tại. |
| Danh mục đã xóa | `queryCongViec` lọc `dm.IsDeleted != true` | Đúng hiện tại. |
| Dự án hủy | Helper đặt `Không đánh giá`; workflow không có action hủy dự án | Không nên tính trễ như dự án vận hành. |
| Công việc tạm dừng | Logic hiện `CongViecTreHan` tính tạm dừng quá hạn như chưa hoàn thành | Cần quyết định nghiệp vụ nếu muốn loại tạm dừng. |
| `DateTime.Now` và `DateTime.Today` | Có cả hai trong Dashboard/DanhGia/Workflow | Nên chuẩn hóa so sánh `.Date` khi triển khai. |
| Summary toàn bộ hay danh sách giới hạn | `TienDoCongViec` tính toàn bộ; `CongViecGanDay` chỉ 5 item | Không dùng preview 5 item làm thống kê. |
| AI dataset cũ | `AiService` có cảnh báo dataset cũ | Không dùng AI dataset thay thống kê trực tiếp. |

## 16. Danh sách file có khả năng cần sửa sau này

| Nhóm | File | Lý do có thể cần sửa | Mức độ | Rủi ro ảnh hưởng |
| --- | --- | --- | --- | --- |
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs` | Nếu sửa scope `Details` hoặc giữ filter workflow modal. | Trung bình | Quyền truy cập/redirect. |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs` | Tách thống kê Công việc đang quá hạn, hoàn thành đúng hạn, hoàn thành trễ, thiếu dữ liệu; tối ưu query. | Cao | Dễ sai số liệu nếu không dùng đúng trạng thái/ngày. |
| Interface | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDuAnService.cs` | Chỉ cần sửa nếu thêm method/scope parameter; thống kê trong VM có thể không cần đổi contract. | Thấp | Contract service. |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnWorkStatusSummaryViewModel.cs` | Thêm properties: `CongViecDangQuaHan`, `CongViecHoanThanhDungHan`, `CongViecHoanThanhTre`, `CongViecThieuDuLieuNgay`, tỷ lệ. | Cao | Binding view. |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnRecentWorkItemViewModel.cs` | Nếu preview cần hiển thị ngày hoàn thành/tình trạng thời hạn. | Trung bình | UI preview. |
| View | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml` | Sửa card `Tiến độ công việc`, thêm nhóm `Kết quả theo thời hạn`, sửa preview công việc. | Cao | Giao diện chính. |
| View cũ | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/ChiTiet.cshtml` | Không cần cho thống kê; có thể ghi nhận/xử lý ở bước dọn sau nếu được phép. | Thấp | Dễ nhầm model nếu tự dùng lại. |
| Partial | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Table.cshtml` | Nếu link/scope/filter cần điều chỉnh từ danh sách. | Thấp/Trung bình | Điều hướng. |
| CSS | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/DuAn/details.css` | Thêm style cho card đúng hạn/trễ, tránh lẫn màu bị cản trở/quá hạn/lỗi. | Trung bình | Responsive/màu sắc. |
| CSS | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/DuAn/index.css` | Chỉ cần nếu danh sách dự án cũng đổi theo. | Thấp | Danh sách dự án. |
| JavaScript | `QuanLyDuAn/QuanLyDuAn/wwwroot/js/site.js` | Không cần cho thống kê; chỉ confirm chung. | Thấp | Không nên sửa nếu không cần. |
| Helper | `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnDeadlineStatusHelper.cs` | Không nên nhồi logic Công việc vào helper dự án nếu không rõ; có thể tạo helper riêng nếu cần. | Trung bình | Dễ trộn dự án và công việc. |
| Tài liệu | `docs/ctduan.md` | Cập nhật kết quả sau khi sửa. | Thấp | Audit trail. |

Không đưa Entity, DbContext hoặc migration vào nhóm bắt buộc. Để bổ sung Công việc hoàn thành đúng hạn/trễ, **không cần sửa database** vì `CONG_VIEC.NgayKetThucCVDuKien`, `CONG_VIEC.NgayKetThucCVThucTe`, `CONG_VIEC.TrangThaiCongViec` đã đủ.

## 17. Kết luận bắt buộc

1. Màn hình Chi tiết dự án hiện tại **chưa đầy đủ** cho đánh giá kết quả Công việc theo thời hạn.
2. Phần làm tốt: route chính rõ (`Details`), hiển thị tổng quan dự án, ngày dự kiến/ngày thực tế dự án, tình trạng thời hạn dự án, tài chính, file, thành viên, hoạt động, AI, link nhanh.
3. Thống kê thiếu: Công việc hoàn thành đúng hạn, Công việc hoàn thành trễ, Công việc hoàn thành thiếu dữ liệu ngày, Công việc đang quá hạn tách riêng, Bị cản trở/Chờ xác nhận dạng count.
4. Có thiếu thống kê Công việc hoàn thành đúng hạn.
5. Có thiếu thống kê Công việc hoàn thành trễ.
6. Khi dự án hoàn thành, các thống kê trạng thái hiện tại mất nhiều ý nghĩa: hầu hết Công việc đều `Hoàn thành`, các trạng thái vận hành về 0, không phản ánh chất lượng đúng/trễ.
7. Có đủ dữ liệu hiện tại để bổ sung mà không sửa database.
8. Chỉ số tối thiểu nên bổ sung: `Tổng Công việc`, `Đang quá hạn`, `Hoàn thành đúng hạn`, `Hoàn thành trễ`; có thể thêm `Chưa đủ dữ liệu xác định`.
9. Nên tách `Tình hình hiện tại` và `Kết quả theo thời hạn`.
10. File dự kiến cần sửa chính: `DuAnService.cs`, `DuAnWorkStatusSummaryViewModel.cs`, `Details.cshtml`, `details.css`; có thể thêm `DuAnRecentWorkItemViewModel.cs` nếu sửa preview.
11. Có vấn đề cần sửa cùng hoặc ghi nhận: scope `Details` chưa giống danh sách; file download phụ thuộc route/id và chưa kiểm tra scope độc lập; query chi tiết nhiều round-trip; responsive nhiều card nhưng không lỗi nghiêm trọng; màu cảnh báo chưa tách hết nghĩa.
12. Có thể viết prompt chỉnh sửa chính xác từ tài liệu này.

## 18. Tự kiểm tra

| Mục | Kết quả |
| --- | --- |
| Đã đọc `DuAnController` | Đã đọc `QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs`. |
| Đã đọc toàn bộ logic `GetChiTietAsync` | Đã đọc và phân tích query project, công việc, tài chính, file, thành viên, hoạt động, deadline. |
| Đã đọc `Details.cshtml` và `ChiTiet.cshtml` | Đã xác định `Details.cshtml` là màn hình runtime; `ChiTiet.cshtml` là view cũ không được action trả trực tiếp. |
| Đã xác định trang đang được sử dụng | `/DuAn/Details/{id}` và alias `/DuAn/ChiTiet/{id}` đều về `Details.cshtml`. |
| Đã phân tích ViewModel Chi tiết dự án | Đã lập bảng property nguồn dữ liệu. |
| Đã phân tích thống kê Công việc | Đã chỉ ra `CongViecTreHan` đang gộp hai nhóm. |
| Đã tách Công việc đang quá hạn và Công việc hoàn thành trễ | Đã phân tích điều kiện tách. |
| Đã kiểm tra khả năng tính hoàn thành đúng hạn | Có đủ dữ liệu, không cần DB. |
| Đã kiểm tra ngày dự kiến và ngày thực tế | Đã kiểm tra `NgayKetThucCVDuKien`, `NgayKetThucCVThucTe`, `NgayKetThucDuAn`, `NgayHoanThanhThucTeDuAn`. |
| Đã kiểm tra workflow mở lại | `MoLaiCongViecAsync` xóa ngày thực tế Công việc; `MoLaiDuAnAsync` xóa ngày thực tế Dự án. |
| Đã kiểm tra tài chính | Đã ghi ngân sách/chi phí/vượt ngân sách. |
| Đã kiểm tra thành viên, team, file | Đã phân tích count, preview, upload/download/delete. |
| Đã kiểm tra panel AI | Đã phân tích điều kiện tạm thời/chính thức/xác nhận. |
| Đã kiểm tra permission và data scope | Đã chỉ ra rủi ro scope `Details`. |
| Đã kiểm tra responsive | Đã phân tích CSS và layout theo khu vực. |
| Đã kiểm tra nguy cơ N+1 | Không thấy N+1 trực tiếp; có nhiều query tuần tự. |
| Đã xác định có cần sửa database không | Không cần sửa DB cho thống kê Công việc đúng hạn/trễ. |
| Đã tạo đúng `docs/ctduan.md` | Tài liệu này là file duy nhất được tạo trong nhiệm vụ. |
| Không sửa file khác | Không chỉnh sửa source code, database, schema, migration hoặc workflow. |
| UTF-8 | File được tạo bằng Markdown tiếng Việt UTF-8; cần kiểm tra lại bằng lệnh đọc UTF-8 và quét mojibake sau khi ghi. |

## 19. Kết quả triển khai thống kê Chi tiết dự án

Mục này ghi nhận kết quả chỉnh sửa trực tiếp chức năng **Chi tiết dự án** sau phần phân tích AS-IS. Nội dung cũ phía trên được giữ nguyên để làm mốc đối chiếu.

### 19.1. File đã sửa

| File | Nội dung chỉnh sửa | Ghi chú |
| --- | --- | --- |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs` | Sửa scope `GetChiTietAsync`, thêm `CanAccessAsync`, tính lại thống kê Công việc từ dữ liệu vận hành, bổ sung tình trạng thời hạn cho 5 Công việc gần đây. | Không sửa workflow, không dùng AI dataset. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDuAnService.cs` | Thêm contract `CanAccessAsync(int maDuAn)`. | Dùng cho kiểm tra scope tải file. |
| `QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs` | `TaiFileDuAn` kiểm tra người dùng có quyền xem đúng dự án chứa file trước khi trả file. | Không đổi cơ chế lưu/xóa/upload file. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnWorkStatusSummaryViewModel.cs` | Bổ sung các property thống kê hiện tại và kết quả theo thời hạn. | Không thêm property vào Entity. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnRecentWorkItemViewModel.cs` | Bổ sung ngày hoàn tất thực tế, mã/text/css tình trạng thời hạn, số ngày trễ. | Chỉ phục vụ preview 5 Công việc gần đây. |
| `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml` | Tổ chức lại khu vực thống kê thành `Tình hình hiện tại` và `Kết quả theo thời hạn`; preview Công việc hiển thị hạn, ngày hoàn tất và badge thời hạn. | Không sửa `Views/DuAn/ChiTiet.cshtml`. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/DuAn/details.css` | Thêm style scoped cho ô thống kê, nhóm thống kê và badge thời hạn Công việc. | Không sửa CSS chung. |
| `docs/ctduan.md` | Thêm mục triển khai này. | Không cập nhật `docs/duan.md`, `docs/congviec.md`, `docs/ctcongviec.md`. |

### 19.2. ViewModel đã bổ sung

`DuAnWorkStatusSummaryViewModel` đã có thêm:

| Property | Ý nghĩa |
| --- | --- |
| `CongViecDangQuaHan` | Công việc chưa hoàn thành chính thức, không hủy, có hạn dự kiến đã qua. |
| `CongViecHoanThanhDungHan` | Công việc hoàn thành chính thức, đủ ngày dự kiến và ngày thực tế, thực tế không sau dự kiến. |
| `CongViecHoanThanhTre` | Công việc hoàn thành chính thức, đủ ngày, thực tế sau dự kiến. |
| `CongViecHoanThanhThieuDuLieuNgay` | Công việc hoàn thành chính thức nhưng thiếu ngày dự kiến hoặc ngày thực tế. |
| `CongViecBiCanTro` | Số Công việc ở trạng thái `Bị cản trở`. |
| `CongViecChoXacNhan` | Số Công việc ở trạng thái `Chờ xác nhận hoàn thành`. |
| `TyLeHoanThanhDungHan` | Tỷ lệ đúng hạn trên mẫu số `Hoàn thành đúng hạn + Hoàn thành trễ`. |
| `TyLeHoanThanhDungHanCap` | Giá trị cap 100 để dùng cho UI nếu cần. |

`DuAnRecentWorkItemViewModel` đã có thêm `NgayKetThucThucTe`, `MaTinhTrangThoiHan`, `TinhTrangThoiHan`, `CssTinhTrangThoiHan`, `SoNgayTre`.

### 19.3. Logic thống kê đã triển khai

`DuAnService.GetChiTietAsync` vẫn lấy Công việc theo đúng dự án bằng join `CONG_VIEC` với `DANH_MUC_CONG_VIEC`, giữ điều kiện:

```text
dm.MaDuAn == id
dm.IsDeleted != true
cv.IsDeleted != true
```

Sau đó service materialize một danh sách tối thiểu gồm mã, tên, danh mục, trạng thái, ngày tạo, ngày kết thúc dự kiến và ngày hoàn thành thực tế. Thống kê được tính trong bộ nhớ cho đúng một dự án, tránh nhồi helper C# vào `IQueryable` và tránh N+1.

| Nhóm | Logic |
| --- | --- |
| Công việc hoàn thành đúng hạn | `TrangThai.LaHoanThanhCongViec(...)`, có đủ `NgayKetThucCVDuKien` và `NgayKetThucCVThucTe`, `NgayKetThucCVThucTe.Date <= NgayKetThucCVDuKien.Date`. |
| Công việc hoàn thành trễ | `TrangThai.LaHoanThanhCongViec(...)`, có đủ hai ngày, `NgayKetThucCVThucTe.Date > NgayKetThucCVDuKien.Date`. |
| Công việc hoàn thành thiếu dữ liệu ngày | Đã hoàn thành chính thức nhưng thiếu ít nhất một trong hai trường ngày. |
| Công việc đang quá hạn | Chưa hoàn thành chính thức, không phải đã hủy, có `NgayKetThucCVDuKien`, hạn dự kiến trước `DateTime.Today`. |
| Công việc chờ xác nhận | Không tính vào đúng hạn/trễ chính thức. Nếu có ngày thực tế, chỉ hiển thị tình trạng phụ trong preview. |
| Công việc đã hủy | Không tính vào đang quá hạn, đúng hạn hoặc hoàn thành trễ. |

`CongViecTreHan` cũ được giữ để tương thích, nhưng hiện là tổng hợp `CongViecDangQuaHan + CongViecHoanThanhTre`. Màn `Details.cshtml` không còn dùng chỉ số gộp này để biểu diễn hai ý nghĩa khác nhau.

`TyLeHoanThanhDungHan` được tính:

```text
CongViecHoanThanhDungHan / (CongViecHoanThanhDungHan + CongViecHoanThanhTre) * 100
```

Nếu mẫu số bằng 0 thì tỷ lệ hiển thị là `0%`, không chia cho 0. Nhóm thiếu dữ liệu ngày không được đưa vào mẫu số.

### 19.4. Bố cục giao diện mới

Khu vực `Thống kê Công việc` trong `Views/DuAn/Details.cshtml` hiện có:

| Khu vực | Nội dung |
| --- | --- |
| Bốn ô nổi bật | `Tổng Công việc`, `Đang quá hạn`, `Hoàn thành đúng hạn`, `Hoàn thành trễ`. |
| `Tình hình hiện tại` | `Chưa bắt đầu`, `Đang thực hiện`, `Bị cản trở`, `Chờ xác nhận`, `Đang quá hạn`, `Tạm dừng`. |
| `Kết quả theo thời hạn` | `Đã hoàn thành`, `Hoàn thành đúng hạn`, `Hoàn thành trễ`, `Chưa đủ dữ liệu`, `Tỷ lệ hoàn thành đúng hạn`. |
| Preview Công việc gần đây | Hiển thị tên, danh mục, trạng thái workflow, hạn dự kiến, ngày hoàn tất nếu có và badge thời hạn ngắn. |

CSS mới nằm trong `wwwroot/css/DuAn/details.css`, dùng màu tách bạch:

| Loại | Màu/nhóm CSS |
| --- | --- |
| Đang quá hạn | Đỏ cảnh báo `overdue` / `is-overdue`. |
| Hoàn thành trễ | Đỏ đậm nhẹ `completed-late` / `is-completed-late`. |
| Hoàn thành đúng hạn | Xanh lá `completed-on-time` / `is-completed-on-time`. |
| Chờ xác nhận | Vàng/cam `pending` / `is-pending`. |
| Bị cản trở | Cam nhạt `blocked`. |
| Chưa đủ dữ liệu hoặc không đánh giá | Xám `missing`, `is-missing`, `is-neutral`. |

Responsive giữ grid co giãn theo vùng hiện có: sidebar desktop hiển thị dạng 2 cột nhỏ, tablet/mobile chuyển theo chiều rộng sẵn có; badge cho phép xuống dòng để không tràn chữ.

### 19.5. Data scope đã sửa

`DuAnService.GetChiTietAsync` hiện áp dụng scope giống danh sách:

| Vai trò | Quy tắc sau sửa |
| --- | --- |
| Admin | Xem được mọi dự án chưa xóa nếu có quyền `DuAn.Xem`. |
| Manager | Chỉ xem dự án có `DU_AN.MaNguoiDung == currentUserId`. |
| Employee | Chỉ xem dự án mà `NHAN_VIEN_DU_AN.MaNguoiDung == currentUserId`. |

Nếu người dùng không thuộc phạm vi, `GetChiTietAsync` trả `null`, controller không để lộ dữ liệu chi tiết. `DuAnController.TaiFileDuAn` sau khi xác nhận file thuộc dự án còn gọi `CanAccessAsync(projectId)`; nếu không thuộc scope thì trả `Forbid()` trước khi trả file vật lý.

### 19.6. Query, N+1 và EF Core

- Không dùng preview 5 Công việc để tính tổng.
- Không query riêng cho từng Công việc.
- Không có query trong vòng lặp.
- Danh sách Công việc thống kê được lấy bằng một query đã lọc đúng dự án và cột tối thiểu.
- Các chuỗi hiển thị như `Hoàn thành trễ X ngày`, `Đang quá hạn X ngày`, `Hoàn tất đúng hạn, chờ xác nhận` được tạo sau materialize.
- Không dùng `AI_DATASET.LaDuAnTre` hoặc kết quả AI để tính thống kê vận hành.

### 19.7. Kết quả build và kiểm thử

Đã chạy:

```text
dotnet build
```

Kết quả:

```text
Build succeeded.
2 Warning(s)
0 Error(s)
```

Hai warning hiện còn ở `Services/Implementations/FileTienDoCongViecService.cs` về async method thiếu `await`, không phát sinh từ thay đổi thống kê Chi tiết dự án.

Sau khi restore đã hoàn tất, chạy thêm:

```text
dotnet build --no-restore
```

Kết quả:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

Các trường hợp logic đã đối chiếu theo source:

| Trường hợp | Kết quả sau sửa |
| --- | --- |
| Dự án đang thực hiện có cả Công việc đang chạy và Công việc đã hoàn thành | Hai nhóm `Tình hình hiện tại` và `Kết quả theo thời hạn` cùng hiển thị. |
| Dự án đã hoàn thành, mọi Công việc đều hoàn thành | UI vẫn hiển thị đúng hạn/trễ/thiếu dữ liệu thay vì chỉ thấy các trạng thái vận hành bằng 0. |
| Hoàn thành đúng ngày hoặc trước hạn | Tính vào `Hoàn thành đúng hạn`. |
| Hoàn thành sau hạn | Tính vào `Hoàn thành trễ`. |
| Hoàn thành thiếu ngày dự kiến hoặc ngày thực tế | Tính vào `Chưa đủ dữ liệu`. |
| Chưa hoàn thành đã qua hạn | Tính vào `Đang quá hạn`. |
| Đã hủy | Không tính vào đang quá hạn hoặc kết quả đúng/trễ. |
| Chờ xác nhận hoàn thành | Không tính là hoàn thành chính thức; preview có tình trạng phụ nếu có ngày thực tế. |
| Không có Công việc | Empty state cũ được giữ, không chia cho 0. |
| Nhập URL trực tiếp ngoài scope | `GetChiTietAsync` trả `null`, controller không lộ dữ liệu chi tiết. |
| Tải file ngoài scope dự án | `TaiFileDuAn` kiểm tra `CanAccessAsync(projectId)` trước khi trả file. |

### 19.8. Xác nhận phạm vi

- Không sửa database, schema, migration, SQL hoặc `DbContext`.
- Không sửa Entity.
- Không sửa workflow dự án, Công việc, Chi tiết công việc, tiến độ, phân công hoặc AI.
- Không sửa `Views/DuAn/ChiTiet.cshtml`.
- Không sửa `docs/duan.md`, `docs/congviec.md`, `docs/ctcongviec.md`.
- Các file chỉnh sửa được giữ UTF-8 và cần được quét mojibake sau lần ghi cuối.

## 20. Phân tích trường hợp Dự án hoàn thành trễ nhưng Công việc đúng hạn 100%

Mục này bổ sung kiểm toán sau triển khai thống kê ở mục 19, nhằm giải thích hiện tượng một Dự án có `Trạng thái: Hoàn thành`, `Tình trạng thời hạn: Hoàn thành trễ`, nhưng nhóm Công việc lại hiển thị `Hoàn thành đúng hạn: N`, `Hoàn thành trễ: 0`, `Tỷ lệ đúng hạn: 100%`.

### 20.1. Mô tả hiện tượng

Hiện tượng cần kiểm tra không nên mặc định là lỗi. Theo source hiện tại, Dự án và Công việc được đánh giá thời hạn bằng hai cấp dữ liệu khác nhau:

| Cấp | Trường hạn dự kiến | Trường ngày thực tế | Nguồn gán ngày thực tế | Ý nghĩa chính |
| --- | --- | --- | --- | --- |
| Dự án | `DuAn.NgayKetThucDuAn` | `DuAn.NgayHoanThanhThucTeDuAn` | `DuAnService.ConfirmCompletionAsync` | Ngày quản lý xác nhận hoàn thành Dự án. |
| Công việc | `CongViec.NgayKetThucCVDuKien` | `CongViec.NgayKetThucCVThucTe` | `TrangThaiWorkflowService.DongBoTrangThaiCongViecTheoChiTietAsync` hoặc `CongViecService.XacNhanHoanThanhCongViecAsync` | Ngày Công việc đủ điều kiện/chờ xác nhận hoặc ngày được xác nhận nếu trước đó chưa có ngày thực tế. |

Vì vậy có thể xảy ra tình huống toàn bộ Công việc hoàn tất đúng hạn riêng, nhưng Dự án vẫn hoàn thành trễ vì người quản lý xác nhận Dự án sau hạn Dự án.

### 20.2. Công thức Dự án hoàn thành trễ

`DuAnDeadlineStatusHelper.Apply` xác định Dự án đã kết thúc bằng `TrangThai.LaHoanThanhCongViec(duAn.TrangThaiDuAn)` hoặc `TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.LuuTru)`. Khi có đủ `NgayKetThucDuAn` và `NgayHoanThanhThucTeDuAn`, helper dùng:

```text
NgayHoanThanhThucTeDuAn.Date > NgayKetThucDuAn.Date
```

để kết luận `Hoàn thành trễ`.

Các điểm đã đối chiếu:

| Nội dung | Kết luận từ source | File/method |
| --- | --- | --- |
| `NgayKetThucDuAn` | Là ngày kết thúc dự kiến của Dự án. | `Models/Entities/DuAn.cs`, `ViewModels/DuAn/DuAnDeadlineStatusHelper.cs` |
| `NgayHoanThanhThucTeDuAn` được gán ở đâu | Gán trong `ConfirmCompletionAsync` bằng `DateTime.Now`. | `Services/Implementations/DuAnService.cs` |
| Khi yêu cầu hoàn thành | `RequestCompletionAsync` chỉ chuyển `TrangThaiDuAn = ChoXacNhanHoanThanh`, không gán ngày hoàn thành thực tế. | `Services/Implementations/DuAnService.cs` |
| Khi tất cả Công việc hoàn thành | `TrangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync` có thể chuyển Dự án lên `ChoXacNhanHoanThanh`, nhưng không gán `NgayHoanThanhThucTeDuAn`. | `Services/Implementations/TrangThaiWorkflowService.cs` |
| Khi mở lại Dự án | `MoLaiDuAnAsync` chuyển về `DangThucHien` và xóa `NgayHoanThanhThucTeDuAn = null`. | `Services/Implementations/DuAnService.cs` |
| So sánh ngày | Helper dùng `.Date`, không lấy giờ/phút/giây để phân loại đúng/trễ. | `ViewModels/DuAn/DuAnDeadlineStatusHelper.cs` |
| `LuuTru` | Được xem như trạng thái đã kết thúc để đánh giá ngày thực tế nếu có. | `ViewModels/DuAn/DuAnDeadlineStatusHelper.cs` |
| `DaHuy` | Trả về `Không đánh giá`. | `ViewModels/DuAn/DuAnDeadlineStatusHelper.cs` |

Kết luận: theo code hiện tại, ngày hoàn thành thực tế của Dự án là ngày xác nhận cuối cùng của quản lý, không phải ngày Công việc cuối cùng hoàn tất.

### 20.3. Công thức Công việc hoàn thành đúng hạn

Trong `DuAnService.GetChiTietAsync`, thống kê Công việc chỉ đưa vào nhóm kết quả hoàn thành khi `TrangThai.LaHoanThanhCongViec(trangThaiCongViec)` trả về đúng. Công việc `ChoXacNhanHoanThanh` không được tính là hoàn thành chính thức.

```text
Hoàn thành đúng hạn:
TrangThai.LaHoanThanhCongViec(TrangThaiCongViec)
và có NgayKetThucCVDuKien
và có NgayKetThucCVThucTe
và NgayKetThucCVThucTe.Date <= NgayKetThucCVDuKien.Date
```

```text
Hoàn thành trễ:
TrangThai.LaHoanThanhCongViec(TrangThaiCongViec)
và có NgayKetThucCVDuKien
và có NgayKetThucCVThucTe
và NgayKetThucCVThucTe.Date > NgayKetThucCVDuKien.Date
```

Các điểm đã đối chiếu:

| Nội dung | Kết luận từ source | File/method |
| --- | --- | --- |
| Khi Công việc lên `ChoXacNhanHoanThanh` | `TrangThaiWorkflowService.DongBoTrangThaiCongViecTheoChiTietAsync` gán `NgayKetThucCVThucTe = DateTime.Now` nếu chưa có. | `Services/Implementations/TrangThaiWorkflowService.cs` |
| Khi xác nhận Công việc | `CongViecService.XacNhanHoanThanhCongViecAsync` chuyển trạng thái sang `HoanThanh`; chỉ gán ngày thực tế nếu trước đó chưa có. | `Services/Implementations/CongViecService.cs` |
| Khi mở lại Công việc | `MoLaiCongViecAsync` xóa `NgayKetThucCVThucTe = null`. | `Services/Implementations/CongViecService.cs` |
| Biến thể trạng thái | Thống kê dùng `TrangThai.LaHoanThanhCongViec` và `TrangThai.EqualsValue`, không chỉ so sánh một chuỗi. | `Services/Implementations/DuAnService.cs`, `Constants/TrangThai.cs` |
| Dùng ngày hiện tại | Công việc đã hoàn thành trễ dùng `NgayKetThucCVThucTe`, không dùng `DateTime.Today`. | `Services/Implementations/DuAnService.cs` |
| So sánh đúng hạn | Dùng `<=`, nên hoàn thành trước hạn hoặc đúng ngày đều là đúng hạn. | `Services/Implementations/DuAnService.cs` |

Kết luận: thống kê Công việc đang đo đúng/trễ theo hạn riêng của từng Công việc, không đo trực tiếp theo hạn Dự án.

### 20.4. Khác biệt ý nghĩa ngày hoàn thành

Đây là khác biệt quan trọng nhất:

| Tình huống | Kết quả theo code hiện tại |
| --- | --- |
| Công việc cuối cùng hoàn tất/chờ xác nhận trước hạn Dự án | `NgayKetThucCVThucTe` của Công việc có thể nằm trước hoặc đúng hạn Công việc. |
| Quản lý xác nhận Công việc sau đó | Nếu `NgayKetThucCVThucTe` đã có, `XacNhanHoanThanhCongViecAsync` không ghi đè ngày này. |
| Dự án chuyển lên `ChoXacNhanHoanThanh` | Workflow chỉ đổi trạng thái, không gán `NgayHoanThanhThucTeDuAn`. |
| Quản lý xác nhận Dự án sau hạn Dự án | `ConfirmCompletionAsync` gán `NgayHoanThanhThucTeDuAn = DateTime.Now`; Dự án bị tính `Hoàn thành trễ`. |

Ví dụ hợp lệ theo code:

| Mốc | Ngày |
| --- | --- |
| Hạn Dự án | 10/06/2026 |
| Hạn Công việc cuối | 08/06/2026 |
| Công việc cuối hoàn tất | 08/06/2026 |
| Dự án được xác nhận | 12/06/2026 |

Kết quả:

```text
Công việc đúng hạn: 100%
Dự án hoàn thành trễ: 2 ngày
```

Đây không tự động là lỗi logic thống kê. Nó phản ánh việc hai cấp đang dùng hai mốc thời gian khác nhau: Công việc dùng ngày hoàn tất/chờ xác nhận của Công việc; Dự án dùng ngày xác nhận hoàn thành cuối cùng của quản lý.

### 20.5. Kiểm tra tính nhất quán giữa hạn Dự án và hạn Công việc

Source có validation ở luồng đề xuất Công việc:

| Ràng buộc | Source hiện có | File/method |
| --- | --- | --- |
| `NgayBatDauCongViecDeXuat <= NgayKetThucCVDeXuatDuKien` | Có kiểm tra. | `DeXuatCongViecService.CreateAsync` |
| `NgayBatDauCongViecDeXuat >= NgayBatDauDuAn` | Có kiểm tra nếu Dự án có ngày bắt đầu. | `DeXuatCongViecService.CreateAsync` |
| `NgayKetThucCVDeXuatDuKien <= NgayKetThucDuAn` | Có kiểm tra nếu Dự án có ngày kết thúc. | `DeXuatCongViecService.CreateAsync` |
| Bước duyệt đề xuất kiểm tra lại ngày | Không thấy kiểm tra lại; `DyetDeXuatCongViecService.ApproveAsync` dùng ngày trong đề xuất để tạo `CongViec`. | `Services/Implementations/DyetDeXuatCongViecService.cs` |

Nhận xét:

- Với dữ liệu đi đúng qua `DeXuatCongViecService.CreateAsync`, hạn Công việc không được vượt hạn Dự án.
- Bước duyệt đang tin đề xuất đã hợp lệ. Nếu có dữ liệu cũ, import SQL, sửa trực tiếp DB, hoặc đề xuất được tạo trước khi validation được bổ sung, vẫn có thể tồn tại Công việc có `NgayKetThucCVDuKien > NgayKetThucDuAn`.
- Trường hợp hạn Công việc sau hạn Dự án có thể tạo ra hiện tượng Công việc đúng hạn theo hạn riêng nhưng Dự án chắc chắn trễ theo hạn Dự án. Đây là dữ liệu/kế hoạch không nhất quán hoặc thiếu validation lặp ở bước duyệt, không phải lỗi của công thức đúng hạn Công việc.

### 20.6. Kiểm tra mẫu số tỷ lệ 100%

`TyLeHoanThanhDungHan` trong `DuAnService.GetChiTietAsync` được tính:

```text
CongViecHoanThanhDungHan
/
(CongViecHoanThanhDungHan + CongViecHoanThanhTre)
* 100
```

Các nhóm bị loại khỏi mẫu số:

| Nhóm | Có trong mẫu số tỷ lệ đúng hạn không | Lý do |
| --- | --- | --- |
| Hoàn thành đúng hạn | Có | Có đủ ngày và xác định được đúng hạn. |
| Hoàn thành trễ | Có | Có đủ ngày và xác định được trễ. |
| Hoàn thành nhưng thiếu ngày | Không | Chưa xác định được đúng hay trễ. |
| Chờ xác nhận hoàn thành | Không | Chưa hoàn thành chính thức. |
| Đã hủy | Không | Không đánh giá kết quả đúng/trễ. |
| Soft-deleted Công việc | Không | Query lọc `cv.IsDeleted != true`. |
| Công việc thuộc danh mục đã xóa | Không | Query lọc `dm.IsDeleted != true`. |

Hệ quả:

```text
10 Công việc đúng hạn
0 Công việc trễ
5 Công việc hoàn thành thiếu dữ liệu ngày
= Tỷ lệ đúng hạn 100%
```

Tỷ lệ này đúng theo công thức hiện tại nhưng nhãn `Tỷ lệ hoàn thành đúng hạn` có thể làm người dùng hiểu là 100% trên toàn bộ Công việc hoàn thành. Nhãn chính xác hơn nên là `Tỷ lệ đúng hạn trên Công việc đủ dữ liệu`, hoặc UI nên hiển thị mẫu số như `10/10 Công việc đủ dữ liệu`, đồng thời vẫn hiển thị `5 thiếu dữ liệu`.

### 20.7. Kết quả đối chiếu dữ liệu thực tế

Đã tìm connection string trong `QuanLyDuAn/QuanLyDuAn/appsettings.json`:

```text
Data Source=LAPTOP-SI5JBDIU\SQLEXPRESS01;Initial Catalog=QuanLyDuAnAI3;Integrated Security=True;Trust Server Certificate=True
```

Đã thử kết nối chỉ đọc:

| Cách thử | Kết quả |
| --- | --- |
| `sqlcmd -S LAPTOP-SI5JBDIU\SQLEXPRESS01 -d QuanLyDuAnAI3 -E` | Không kết nối được; ODBC báo lỗi encryption/SSL. |
| `System.Data.SqlClient` với `Encrypt=False` và server `LAPTOP-SI5JBDIU\SQLEXPRESS01` | Không kết nối được; lỗi `The target principal name is incorrect. Cannot generate SSPI context.` |
| `System.Data.SqlClient` với `localhost\SQLEXPRESS01` và `.\SQLEXPRESS01` | Không kết nối được; cùng lỗi SSPI. |
| `(localdb)\MSSQLLocalDB` | Không có/không tạo được LocalDB instance. |

Vì vậy chưa xác nhận được dữ liệu thực tế trong database từ môi trường lệnh hiện tại. Không có truy vấn ghi dữ liệu nào được chạy.

Truy vấn SELECT nên chạy lại khi SQL Server xác thực được:

```sql
SET NOCOUNT ON;

WITH WorkStats AS (
    SELECT
        da.MaDuAn,
        COUNT(cv.MaCongViec) AS TongCongViec,
        SUM(CASE WHEN cv.TrangThaiCongViec IN (N'HoanThanh', N'Hoàn thành', N'Done', N'Completed') THEN 1 ELSE 0 END) AS CVHoanThanh,
        SUM(CASE WHEN cv.TrangThaiCongViec IN (N'HoanThanh', N'Hoàn thành', N'Done', N'Completed')
                  AND cv.NgayKetThucCVDuKien IS NOT NULL
                  AND cv.NgayKetThucCVThucTe IS NOT NULL
                  AND CONVERT(date, cv.NgayKetThucCVThucTe) <= CONVERT(date, cv.NgayKetThucCVDuKien)
                 THEN 1 ELSE 0 END) AS CVDungHan,
        SUM(CASE WHEN cv.TrangThaiCongViec IN (N'HoanThanh', N'Hoàn thành', N'Done', N'Completed')
                  AND cv.NgayKetThucCVDuKien IS NOT NULL
                  AND cv.NgayKetThucCVThucTe IS NOT NULL
                  AND CONVERT(date, cv.NgayKetThucCVThucTe) > CONVERT(date, cv.NgayKetThucCVDuKien)
                 THEN 1 ELSE 0 END) AS CVTre,
        SUM(CASE WHEN cv.TrangThaiCongViec IN (N'HoanThanh', N'Hoàn thành', N'Done', N'Completed')
                  AND (cv.NgayKetThucCVDuKien IS NULL OR cv.NgayKetThucCVThucTe IS NULL)
                 THEN 1 ELSE 0 END) AS CVThieuNgay,
        SUM(CASE WHEN cv.NgayKetThucCVDuKien IS NOT NULL
                  AND da.NgayKetThucDuAn IS NOT NULL
                  AND CONVERT(date, cv.NgayKetThucCVDuKien) > CONVERT(date, da.NgayKetThucDuAn)
                 THEN 1 ELSE 0 END) AS CVHanSauHanDuAn,
        MAX(cv.NgayKetThucCVThucTe) AS NgayCVThucTeCuoi
    FROM DU_AN da
    LEFT JOIN DANH_MUC_CONG_VIEC dm ON dm.MaDuAn = da.MaDuAn AND ISNULL(dm.IsDeleted, 0) = 0
    LEFT JOIN CONG_VIEC cv ON cv.MaDanhMucCV = dm.MaDanhMucCV AND ISNULL(cv.IsDeleted, 0) = 0
    WHERE ISNULL(da.IsDeleted, 0) = 0
    GROUP BY da.MaDuAn
)
SELECT
    da.MaDuAn,
    da.TenDuAn,
    da.TrangThaiDuAn,
    da.NgayBatDauDuAn,
    da.NgayKetThucDuAn,
    da.NgayHoanThanhThucTeDuAn,
    DATEDIFF(day, CONVERT(date, da.NgayKetThucDuAn), CONVERT(date, da.NgayHoanThanhThucTeDuAn)) AS SoNgayDuAnTre,
    ws.TongCongViec,
    ws.CVHoanThanh,
    ws.CVDungHan,
    ws.CVTre,
    ws.CVThieuNgay,
    ws.CVHanSauHanDuAn,
    ws.NgayCVThucTeCuoi
FROM DU_AN da
JOIN WorkStats ws ON ws.MaDuAn = da.MaDuAn
WHERE ISNULL(da.IsDeleted, 0) = 0
  AND da.NgayKetThucDuAn IS NOT NULL
  AND da.NgayHoanThanhThucTeDuAn IS NOT NULL
  AND CONVERT(date, da.NgayHoanThanhThucTeDuAn) > CONVERT(date, da.NgayKetThucDuAn)
ORDER BY SoNgayDuAnTre DESC, da.MaDuAn;
```

Cần chạy thêm truy vấn chi tiết cho từng Dự án nghi vấn để xem từng `CongViec.NgayKetThucCVDuKien`, `CongViec.NgayKetThucCVThucTe`, `HanCongViecCoSauHanDuAnKhong`, và nếu cần đối chiếu `NHAT_KY_QUAN_LY_DU_AN` để tìm mốc quản lý xác nhận. Source hiện có `NhatKyQuanLyDuAn.NkThoiGianQLDA` và `NkHanhDongQLDA`, nhưng log tự động lên `ChoXacNhanHoanThanh` của Dự án trong `TrangThaiWorkflowService` không thấy ghi nhật ký tương ứng, nên mốc chờ xác nhận Dự án có thể không luôn truy vết được đầy đủ.

### 20.8. Bảng phân loại nguyên nhân

| Mã | Nhóm nguyên nhân | Điều kiện nhận biết | Là lỗi code hay dữ liệu | Mức độ khả năng |
| --- | --- | --- | --- | --- |
| N1 | Xác nhận Dự án chậm | Tất cả Công việc hoàn thành đúng hạn, nhưng `NgayHoanThanhThucTeDuAn.Date > NgayKetThucDuAn.Date`; `NgayCVThucTeCuoi` trước hoặc bằng hạn Dự án. | Hợp lệ theo code hiện tại; có thể là thiết kế ngày chưa thể hiện đủ mốc nghiệp vụ. | Cao theo source. |
| N2 | Hạn Công việc nằm sau hạn Dự án | Có `CongViec.NgayKetThucCVDuKien.Date > DuAn.NgayKetThucDuAn.Date`. | Dữ liệu/kế hoạch không nhất quán hoặc thiếu kiểm tra lại ở bước duyệt; cần SELECT để xác nhận dữ liệu thật. | Trung bình. |
| N3 | Tỷ lệ 100% bỏ qua Công việc thiếu dữ liệu | Có `CongViecHoanThanhThieuDuLieuNgay > 0` nhưng `CongViecHoanThanhTre = 0`. | Công thức đúng theo mẫu số hiện tại, nhưng nhãn UI dễ gây hiểu nhầm. | Trung bình. |
| N4 | Ngày Công việc và ngày Dự án mang ý nghĩa khác nhau | `NgayKetThucCVThucTe` được gán khi Công việc chờ xác nhận/được xác nhận, còn `NgayHoanThanhThucTeDuAn` được gán khi quản lý xác nhận Dự án. | Không đồng nhất semantics thời gian; không nhất thiết là lỗi code. | Cao theo source. |
| N5 | Logic trạng thái hoàn thành không thống nhất | Một nơi chỉ so chuỗi `HoanThanh`, nơi khác dùng `TrangThai.LaHoanThanhCongViec`; làm lệch tập Công việc. | Có thể là lỗi code nếu tìm thấy nơi thống kê bỏ biến thể trạng thái. Với `GetChiTietAsync` hiện tại chưa thấy lỗi này. | Thấp cho màn Chi tiết hiện tại. |
| N6 | Dữ liệu được nhập bằng SQL bỏ qua validation | Ngày Công việc/hạn Dự án không hợp lệ dù UI/service tạo mới có validate. | Lỗi dữ liệu hoặc dữ liệu lịch sử. | Chưa xác định do chưa kết nối DB được. |
| N7 | Mở lại rồi hoàn thành lại | `MoLaiDuAnAsync` và `MoLaiCongViecAsync` xóa ngày thực tế cũ; không có lịch sử kết quả nhiều lần trong entity chính. | Giới hạn mô hình dữ liệu/lịch sử; có thể làm khó giải thích nguyên nhân trễ sau nhiều vòng mở lại. | Trung bình. |
| N8 | Sai khác giờ và ngày | Một logic dùng giờ/phút, logic khác dùng `.Date`. | Có thể gây lỗi biên ngày nếu tồn tại. Với helper và thống kê Chi tiết hiện tại đều dùng `.Date`, nên không phải nguyên nhân chính. | Thấp. |

### 20.9. Kết luận nguyên nhân chính

Kết luận chắc chắn từ source:

```text
Nguyên nhân chính là hai cấp đang dùng hai mốc thời gian khác nhau; kết quả hiện tại hợp lệ theo code nhưng giao diện có thể gây hiểu nhầm.
```

Cụ thể:

- Dự án hoàn thành trễ được tính theo `DuAn.NgayHoanThanhThucTeDuAn`, mà source gán khi quản lý xác nhận Dự án trong `DuAnService.ConfirmCompletionAsync`.
- Công việc hoàn thành đúng hạn được tính theo `CongViec.NgayKetThucCVThucTe` so với `CongViec.NgayKetThucCVDuKien`; ngày thực tế Công việc có thể được gán sớm hơn khi Công việc lên `ChoXacNhanHoanThanh`.
- Vì thế `Công việc đúng hạn 100%` không đồng nghĩa tuyệt đối với `Dự án hoàn thành đúng hạn`.
- Chờ xác nhận, nghiệm thu/quản lý xác nhận, hoặc các bước sau Công việc có thể làm Dự án hoàn thành trễ dù các Công việc không trễ theo hạn riêng.

Các khả năng cần xác nhận thêm bằng dữ liệu thực tế:

- Có Công việc nào có hạn sau hạn Dự án không.
- Có Dự án nào `CVThieuNgay > 0` nhưng tỷ lệ vẫn hiển thị `100%` không.
- Có dữ liệu lịch sử hoặc nhập SQL bỏ qua validation không.
- Nếu tồn tại hạn Công việc sau hạn Dự án hoặc thiếu ngày thực tế/dự kiến bất thường, cần phân loại là `Dữ liệu không nhất quán` thay vì kết luận ngay là lỗi logic thống kê.

Không có bằng chứng từ source rằng `GetChiTietAsync` hiện đang dùng sai trường, dùng danh sách preview 5 Công việc để tính tổng, dùng `DateTime.Today` cho Công việc đã hoàn thành, hoặc tính `ChoXacNhanHoanThanh` như hoàn thành chính thức.

### 20.10. Đề xuất hướng chỉnh sửa tiếp theo

| Hướng | Khi áp dụng | Đề xuất cụ thể | Ảnh hưởng |
| --- | --- | --- | --- |
| A - Chỉ sửa cách hiển thị | Logic và dữ liệu đúng, nhưng người dùng hiểu nhầm `100%`. | Đổi nhãn thành `Tỷ lệ đúng hạn trên Công việc đủ dữ liệu`; hiển thị mẫu số `N/M`; thêm dòng ngắn `Dự án hoàn thành trễ do ngày xác nhận Dự án sau hạn`. | Ít rủi ro, không cần DB. |
| B - Sửa logic ngày hoàn thành Dự án | Nghiệp vụ muốn `NgayHoanThanhThucTeDuAn` là ngày hoàn tất nghiệp vụ, không phải ngày quản lý xác nhận. | Cần xác định mốc mới: ngày Công việc cuối hoàn tất, ngày chuyển `ChoXacNhanHoanThanh`, hoặc ngày gửi yêu cầu hoàn thành. | Ảnh hưởng workflow, Dashboard, Đánh giá dự án, AI; nếu cần lưu riêng nhiều mốc thì phải sửa DB. |
| C - Sửa validation ngày | SELECT phát hiện hạn Công việc vượt hạn Dự án hoặc bước duyệt có thể nhận dữ liệu cũ không hợp lệ. | Kiểm tra lại ngày trong `DyetDeXuatCongViecService.ApproveAsync` trước khi tạo `CongViec`; cân nhắc validation khi sửa Dự án nếu thay đổi hạn làm lệch Công việc hiện có. | Không cần sửa DB nếu chỉ thêm validation service. |
| D - Sửa dữ liệu | Database thật có bản ghi Công việc/hạn Dự án không nhất quán. | Lập danh sách bản ghi cần người quản trị xem lại; không tự sửa dữ liệu trong ứng dụng. | Cần quy trình dữ liệu riêng. |
| E - Sửa công thức thống kê | Muốn tỷ lệ phản ánh toàn bộ Công việc hoàn thành, kể cả thiếu dữ liệu. | Có thể thêm tỷ lệ phụ hoặc đổi công thức thành `Đúng hạn / Hoàn thành`, nhưng phải giải thích nhóm thiếu dữ liệu. | Cần thống nhất định nghĩa KPI trước khi sửa. |

Khuyến nghị gần nhất: ưu tiên Hướng A để giảm hiểu nhầm trên màn Chi tiết dự án; đồng thời chạy SELECT ở mục 20.7 để xác định có cần Hướng C hoặc D hay không. Không nên dùng `AI_DATASET.LaDuAnTre` để giải thích hoặc thay thế dữ liệu vận hành trực tiếp.
