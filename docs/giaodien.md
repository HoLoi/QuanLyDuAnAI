# Kiểm tra đồng bộ giao diện hệ thống

## 1. Mục tiêu kiểm tra

Tài liệu này thống kê mức độ đồng bộ giao diện giữa các chức năng trong dự án ASP.NET Core MVC hiện tại. Phạm vi chỉ là phân tích View/CSS/JS và ghi nhận điểm chưa đồng bộ để quyết định ưu tiên chuẩn hóa UI.

Không sửa code, không đổi nghiệp vụ, không đổi route/controller/model/database.

## 2. Phạm vi source đã đọc

Đã rà soát các nhóm file sau trong project MVC `QuanLyDuAn/QuanLyDuAn`:

* `Views/**/*.cshtml`, gồm `_ViewStart.cshtml`, `_ViewImports.cshtml`, các view chức năng, partial view, form, table, modal, component nhỏ.
* `Views/Shared/**/*.cshtml`, gồm `_Layout.cshtml`, `_ExportDropdown.cshtml`, `_ValidationScriptsPartial.cshtml`, `Error.cshtml`.
* `wwwroot/css/**/*.css`, gồm `site.css`, `layout/sidebar.css`, `shared/export-dropdown.css`, CSS theo module như `CongViec`, `TienDoCongViec`, `DuAn`, `Dashboard`, `AI`, `ChatDuAn`, `DanhGiaDuAn`, `DanhGiaNhanVien`, v.v.
* `wwwroot/js/**/*.js`, gồm `site.js`, `phanquyen/index.js`, `ai/charts.js`. JS liên quan UI chủ yếu nằm inline trong một số view như `Dashboard/Index.cshtml`, `DuAn/_Table.cshtml`, `DuAn/_Form.cshtml`, `DuyetDeXuatCongViec/Index.cshtml`, `DuyetDeXuatNganSach/Index.cshtml`, `TienDoCongViec/Index.cshtml`.

Ghi chú phạm vi:

* Có link menu tới `ChiPhi` trong `_Layout.cshtml` và `Dashboard/Index.cshtml`, nhưng trong `Views` hiện tại chưa thấy thư mục/view riêng `ChiPhi`. Vì vậy mục Chi phí được ghi là chưa xác định rõ, cần kiểm tra thêm controller/route hoặc source bị thiếu view.
* Các màn Account đăng nhập/quên mật khẩu/OTP/reset dùng `Layout = null` có chủ đích, nhưng vẫn được ghi nhận vì khác layout chuẩn của màn authenticated.

## 3. Style chuẩn hiện tại của hệ thống

### 3.1 Layout chuẩn

Layout chuẩn đang nằm tại:

* `Views/Shared/_Layout.cshtml`
* `wwwroot/css/site.css`
* `wwwroot/css/layout/sidebar.css`
* `wwwroot/css/TaiKhoanCaNhan/index.css` được import toàn cục trong layout để phục vụ dropdown tài khoản.

Đặc điểm chuẩn:

* Tất cả màn authenticated đi qua `_ViewStart.cshtml` với `Layout = "_Layout"`.
* Shell chính dùng `app-shell`, `app-sidebar`, `app-main`, `app-topbar`, `app-content`.
* Sidebar dùng `side-link`, `sidebar-menu-link`, `sidebar-active`, icon Bootstrap Icons.
* Tiêu đề topbar lấy từ `ViewData["Title"]` và hiển thị bằng `page-title`.
* Thông báo hệ thống dùng `TempData["Success"]`, `TempData["Error"]`, `TempData["Warning"]` và class Bootstrap `alert alert-success|danger|warning`.

Màn đối chiếu tốt:

* `Views/Shared/_Layout.cshtml`
* `Views/CongViec/Index.cshtml`
* `Views/TienDoCongViec/Index.cshtml`
* `Views/DuAn/Index.cshtml`
* `Views/NhanSu/Index.cshtml`

### 3.2 Card/container chuẩn

Chuẩn chung trong `site.css` là `card-soft`, `app-content`, `empty-state`. Tuy nhiên thực tế nhiều màn list hiện đang dùng chuẩn module mới:

* Wrapper page: `cong-viec-page`, `du-an-page`, `nhan-su-page`, `team-page`, `thanh-vien-team-page`.
* Header/card: `dashboard-hero`, `dashboard-card`, `*-header-card`, `*-section-card`.
* Card có nền trắng, border `#e4e9f3` hoặc `var(--line)`, shadow nhẹ, radius khoảng `14px - 18px`.

Màn đang làm chuẩn tốt nhất cho layout list nghiệp vụ:

* `Views/CongViec/Index.cshtml` + `wwwroot/css/CongViec/index.css`
* `Views/DuAn/Index.cshtml` + `wwwroot/css/DuAn/index.css`
* `Views/TienDoCongViec/Index.cshtml` + `wwwroot/css/TienDoCongViec/index.css`

### 3.3 Button chuẩn

Có hai hệ button cùng tồn tại:

* Bootstrap chuẩn: `btn btn-primary`, `btn btn-outline-secondary`, `btn btn-sm`.
* Hệ action riêng đang lặp lại nhiều nơi: `btn-action`, `btn-subtle`, `btn-confirm`, `btn-reopen`, `btn-danger-soft`.

Hệ `btn-action` đang được dùng rộng ở `CongViec`, `TienDoCongViec`, `DuAn`, `NhanSu`, `Team`, `ThanhVienTeam`, `AI`, `DanhGiaDuAn`, `DanhGiaNhanVien`. Đây nên được coi là chuẩn thao tác trong bảng/form.

Chưa đồng bộ:

* Nhiều module tự tạo nút riêng: `du-an-primary-btn`, `dxcv-action-btn`, `dxns-action-btn`, `ngan-sach-header-action`, `duyet-de-xuat-*-action-btn`, `yeu-cau-action-btn`, `team-btn-primary`, `thanh-vien-btn-primary`.
* Một số nút không có icon trong khi đa số màn list có icon.

### 3.4 Table chuẩn

Chuẩn chung trong `site.css`:

* `app-data-table`
* `table-actions`
* `col-actions`
* `table-responsive-soft`

Chuẩn thực tế tốt:

* `Views/CongViec/_Table.cshtml`: dùng `table app-data-table workflow-table`, `workflow-table-wrapper table-responsive-soft`, `table-actions`, `btn-action`.
* `Views/TienDoCongViec/_Table.cshtml`: dùng `table workflow-table app-data-table tien-do-table`, có responsive chuyển hàng thành block trên mobile.
* `Views/DuAn/_Table.cshtml`: dùng `table workflow-table app-data-table du-an-table`, table-scroll riêng và action buttons đồng bộ với icon.

Chưa đồng bộ:

* `DeXuatCongViec`, `DeXuatNganSach`, `DuyetDeXuatCongViec`, `DuyetDeXuatNganSach`, `NganSach`, `YeuCauDoiQuanLy` dùng table riêng như `dxcv-table`, `dxns-table`, `duyet-de-xuat-*-table`, `ngan-sach-table`, `yeu-cau-table` mà không dùng `app-data-table`.
* `ChucDanh`, `DanhMucCongViec`, `TeamDuAn`, `NhanVienDuAn` có `table-bordered` trong một số table, nhìn nặng hơn chuẩn list hiện tại.

### 3.5 Form chuẩn

Chuẩn hiện tại:

* Label nhỏ, font-weight 600, màu muted.
* Input/select cao khoảng 40px, border `#d6e0ef`, radius khoảng `0.72rem`.
* Filter thường chia 2 hàng: hàng điều kiện chính và hàng thời gian/nút.
* Form action dùng `btn-action btn-confirm`, `btn-action btn-subtle`.

Màn đối chiếu:

* `Views/CongViec/_Filter.cshtml` với `workflow-filter`, `workflow-filter-row-main`, `workflow-filter-row-time`.
* `Views/TienDoCongViec/_Filter.cshtml` với `td-filter-form`, `td-filter-grid-primary`, `td-filter-grid-secondary`.
* `Views/DuAn/_Filter.cshtml` với `du-an-filter-grid-main`, `du-an-filter-grid-time`.

Chưa đồng bộ:

* `DuAn/_Form.cshtml`, `Team/_Form.cshtml`, `NhanSu/_Form.cshtml`, `ChucDanh/_Form.cshtml`, `ThanhVienTeam/_Form.cshtml` vẫn dùng Bootstrap grid trực tiếp `row`, `col-md-*`, `mb-2` nhiều hơn class form module.
* `DuAn/_Form.cshtml` có nhiều `style="display:inline;"` và `style="display:none;"`.

### 3.6 Badge/trạng thái chuẩn

Chuẩn chung trong `site.css` mới chỉ có:

* `badge-soft-danger`
* `badge-soft-success`

Chuẩn thực tế đang dùng nhiều hơn:

* `workflow-badge` + `workflow-success`, `workflow-pending`, `workflow-blocked`, `workflow-paused`, `workflow-cancelled`, `workflow-idle`, `workflow-active`.
* Badge module riêng: `du-an-status-badge`, `dxcv-status-badge`, `dxns-status-badge`, `td-report-badge`, `td-ctcv-badge`, `ngan-sach-status-badge`, `duyet-de-xuat-*-status-badge`, `score-badge`, `status-badge`.

Chưa đồng bộ:

* Cùng ý nghĩa trạng thái nhưng mỗi module đặt class và màu riêng. Ví dụ trạng thái chờ duyệt có `workflow-pending`, `td-badge-report-pending`, `dxcv-status-pending`, `dxns-status-pending`, `duyet-de-xuat-*-status-badge.is-pending`.
* `score-badge`, `summary-item`, `criteria-row`, `empty-box` bị định nghĩa lặp ở CSS `DanhGiaDuAn` và `DanhGiaNhanVien`.

### 3.7 Modal/thông báo chuẩn

Thông báo chuẩn:

* `_Layout.cshtml` dùng Bootstrap `alert`.
* Validation summary thường dùng `text-danger` kèm card/module wrapper.

Modal chuẩn:

* `DuAn/_Form.cshtml` dùng Bootstrap modal (`modal fade`, `modal-dialog`, `modal-content`, `modal-header`, `modal-footer`) cho mở lại/tạm dừng dự án.

Chưa đồng bộ:

* Một số màn dùng collapse/detail row thay cho modal (`DeXuatCongViec/_Table.cshtml`, `DeXuatNganSach/_Table.cshtml`, `DuyetDeXuatCongViec/_Table.cshtml`, `DuyetDeXuatNganSach/_Table.cshtml`). Đây không sai nghiệp vụ nhưng khác cách trình bày chi tiết.
* JS confirm/disable submit đang inline trong view duyệt đề xuất, chưa gom vào JS chung.

## 4. Bảng tổng hợp mức độ đồng bộ giao diện

| STT | Chức năng | File view | Mức độ đồng bộ | Vấn đề chính | Mức độ ảnh hưởng | Ưu tiên sửa |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | Account / Đăng nhập | `Views/Account/Login.cshtml`, `ForgotPassword.cshtml`, `ResetPassword.cshtml`, `VerifyOtp.cshtml`, `AccessDenied.cshtml` | Trung bình | Login/OTP/reset dùng `Layout = null`, tự import CSS; `AccessDenied` dùng layout chuẩn | Trung bình | Trung bình |
| 2 | Dashboard | `Views/Dashboard/Index.cshtml` | Trung bình | Có CSS riêng lớn, nhiều card/chart riêng, 6 inline style cho progress width | Trung bình | Trung bình |
| 3 | Nhân sự | `Views/NhanSu/*` | Cao | Dùng `workflow-filter`, `app-data-table`, `btn-action`; form còn Bootstrap grid trực tiếp | Nhẹ | Thấp |
| 4 | Chức danh | `Views/ChucDanh/*` | Trung bình | Header/card riêng, table `table-bordered`, form Bootstrap đơn giản | Nhẹ | Thấp |
| 5 | Phân quyền | `Views/PhanQuyen/Index.cshtml` | Trung bình | Giao diện accordion/card đặc thù, CSS riêng và JS riêng | Trung bình | Trung bình |
| 6 | Team/Nhóm | `Views/Team/*` | Cao | Dùng `workflow-filter`, `app-data-table`, `btn-action`; form còn Bootstrap grid | Nhẹ | Thấp |
| 7 | Dự án | `Views/DuAn/Index.cshtml`, `_Filter.cshtml`, `_Table.cshtml`, `_Form.cshtml`, `Details.cshtml` | Trung bình | Index khá đồng bộ nhưng có hệ `du-an-*` riêng; `_Form` inline style; `Details` CSS rất lớn và nhiều inline style | Trung bình | Cao |
| 8 | Thành viên dự án | `Views/NhanVienDuAn/*` | Trung bình | Dùng CSS `ThanhVienDuAn/index.css`, table `table-bordered`, nút/back/status riêng | Nhẹ | Trung bình |
| 9 | Team dự án | `Views/TeamDuAn/*` | Trung bình | Dùng CSS `TeamDuAn/index.css`, table `table-bordered`, class `team-*` dễ trùng với Team | Nhẹ | Trung bình |
| 10 | Danh mục công việc | `Views/DanhMucCongViec/*` | Trung bình | CSS riêng, table `table-bordered`, filter không dùng `workflow-filter` | Nhẹ | Trung bình |
| 11 | Công việc | `Views/CongViec/*` | Cao | Đây là màn chuẩn tốt; chỉ có nhiều CSS module riêng | Nhẹ | Thấp |
| 12 | Chi tiết công việc | `Views/ChiTietCongViec/*` | Trung bình | Gần chuẩn Công việc nhưng CSS riêng rất lớn, 1 inline style progress | Trung bình | Trung bình |
| 13 | Phân công công việc | `Views/PhanCongCongViec/*` | Trung bình | Dùng `pc-*` riêng, table không dùng `app-data-table`, nhiều CSS trùng với phân công chi tiết | Trung bình | Trung bình |
| 14 | Phân công chi tiết công việc | `Views/PhanCongChiTietCongViec/*` | Trung bình | Dùng `pct-*` riêng, table không dùng `app-data-table`, CSS gần trùng `PhanCongCongViec` | Trung bình | Trung bình |
| 15 | Tiến độ công việc | `Views/TienDoCongViec/*` | Cao | Màn chuẩn filter/table tốt; có 1 inline style progress width, nút gradient khác `CongViec` | Nhẹ | Thấp |
| 16 | Đề xuất công việc | `Views/DeXuatCongViec/*` | Trung bình | CSS/action/table/badge riêng `dxcv-*`, không dùng `app-data-table`, detail row riêng | Trung bình | Cao |
| 17 | Duyệt đề xuất công việc | `Views/DuyetDeXuatCongViec/*` | Thấp-Trung bình | Prefix CSS rất dài, table/button/badge riêng, inline JS toggle/confirm | Trung bình | Cao |
| 18 | Đề xuất ngân sách | `Views/DeXuatNganSach/*` | Trung bình | Gần trùng `DeXuatCongViec` nhưng CSS riêng `dxns-*`, không dùng class chung | Trung bình | Cao |
| 19 | Duyệt ngân sách | `Views/DuyetDeXuatNganSach/*` | Thấp-Trung bình | Gần copy style duyệt công việc bằng prefix riêng, inline JS riêng | Trung bình | Cao |
| 20 | Chi phí | Chưa thấy `Views/ChiPhi/*` | Chưa xác định rõ | Có link tới `ChiPhi` nhưng chưa thấy view trong phạm vi đọc | Chưa xác định | Cần kiểm tra thêm |
| 21 | Đánh giá dự án | `Views/DanhGiaDuAn/*` | Trung bình | Nhiều CSS tách file nhưng một số class lại trùng/định nghĩa lại trong `index.css` | Trung bình | Cao |
| 22 | Đánh giá nhân viên | `Views/DanhGiaNhanVien/*` | Trung bình | Gần trùng `DanhGiaDuAn`, nhiều CSS riêng và class lặp | Trung bình | Cao |
| 23 | Chat | `Views/ChatDuAn/*` | Thấp-Trung bình | UI chat đặc thù, tách 7 CSS, không theo card/table/form chuẩn | Trung bình | Trung bình |
| 24 | AI Dataset | `Views/AiDataset/Index.cshtml` | Trung bình | Dùng chung `AI/index.css`, layout AI riêng; table/form không theo `app-data-table` hoàn toàn | Trung bình | Trung bình |
| 25 | AI Train/Model | `Views/Ai/Train.cshtml`, `Models.cshtml` | Trung bình | Dùng hệ `ai-*` riêng, button/badge riêng | Trung bình | Trung bình |
| 26 | AI phân tích nguyên nhân trễ | `Views/Ai/Predict.cshtml`, `Dashboard.cshtml` | Trung bình | Form/grid/chart/card riêng trong `AI/index.css`, không dùng filter/table chuẩn nghiệp vụ | Trung bình | Trung bình |
| 27 | Yêu cầu đổi quản lý | `Views/YeuCauDoiQuanLy/*` | Trung bình | CSS/button/table/badge riêng `yeu-cau-*`, không dùng `app-data-table` | Trung bình | Trung bình |
| 28 | Duyệt yêu cầu đổi quản lý | `Views/DuyetYeuCauDoiQuanLy/*` | Trung bình | CSS/button/table/badge riêng `duyet-yeu-cau-*`, gần giống nhóm duyệt đề xuất nhưng không dùng chung | Trung bình | Trung bình |
| 29 | Tài khoản cá nhân | `Views/TaiKhoanCaNhan/*` | Trung bình | CSS rất lớn được import toàn cục trong layout, ảnh hưởng phạm vi rộng hơn cần thiết | Trung bình | Cao |
| 30 | Home/Error | `Views/Home/*`, `Views/Shared/Error.cshtml` | Thấp | View scaffold đơn giản, không theo style dashboard/card hiện tại | Nhẹ | Thấp |

## 5. Chi tiết các giao diện chưa đồng bộ

### 5.1 Account / Đăng nhập

* File view: `Views/Account/Login.cshtml`, `ForgotPassword.cshtml`, `ResetPassword.cshtml`, `VerifyOtp.cshtml`, `AccessDenied.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/Account/login.css`, `wwwroot/css/site.css`.
* Màn hình đang khác so với chuẩn ở điểm: các màn login/quên mật khẩu/reset/OTP dùng `Layout = null` và tự import Bootstrap, Bootstrap Icons, `site.css`, `Account/login.css`; không đi qua `app-shell`, `app-topbar`, `app-content`.
* Ví dụ class/style đang dùng: `auth-page-body`, `auth-shell`, `auth-card`, `auth-input`, `auth-submit-btn`.
* Giao diện chuẩn nên đối chiếu: `_Layout.cshtml` cho shell chung; vẫn có thể giữ layout guest riêng nhưng nên tái dùng token màu/radius/button từ `site.css`.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: giữ `Layout = null` nếu đây là chủ ý cho auth page, nhưng gom token auth về biến chung, đồng bộ `auth-card` với `card-soft`, `auth-submit-btn` với hệ `btn-action/btn-confirm` hoặc Bootstrap `btn-primary` đã chuẩn hóa.

### 5.2 Dashboard

* File view: `Views/Dashboard/Index.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/Dashboard/index.css`, `wwwroot/css/shared/export-dropdown.css`, script Chart.js inline trong view.
* Màn hình đang khác so với chuẩn ở điểm: dashboard có hệ card/chart riêng, table mini riêng, inline style cho progress width và chart màu inline trong JS.
* Ví dụ class/style đang dùng: `dashboard-overview-page`, `overview-card`, `status-pill-card`, `warning-card`, `dashboard-mini-table`, `style="width: @BuildRate(...)%"`.
* Giao diện chuẩn nên đối chiếu: `CongViec/Index.cshtml` cho header/card/list; `site.css` cho table/card cơ bản.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: giữ cấu trúc dashboard đặc thù nhưng chuẩn hóa card metric thành class dùng chung `app-stat-card`, chuyển progress width sang CSS variable hoặc helper class khi có thể, gom script chart sang JS module riêng nếu tiếp tục mở rộng.

### 5.3 Chức danh

* File view: `Views/ChucDanh/Index.cshtml`, `_Form.cshtml`, `_Table.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/ChucDanh/index.css`.
* Màn hình đang khác so với chuẩn ở điểm: table dùng `table table-bordered mt-3 app-data-table`, form dùng Bootstrap grid/basic button nhiều hơn form chuẩn module.
* Ví dụ class/style đang dùng: `chuc-danh-header-card`, `chuc-danh-section-card`, `table-bordered`, `btn btn-primary`.
* Giao diện chuẩn nên đối chiếu: `NhanSu`, `Team`, `CongViec`.
* Mức độ ảnh hưởng: Nhẹ.
* Gợi ý sửa: bỏ `table-bordered` nếu không cần, dùng `workflow-filter`/`btn-action` cho thao tác, đồng bộ header/card với `dashboard-card` hoặc chuẩn `*-section-card`.

### 5.4 Phân quyền

* File view: `Views/PhanQuyen/Index.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/PhanQuyen/index.css`, `wwwroot/js/phanquyen/index.js`.
* Màn hình đang khác so với chuẩn ở điểm: màn này dùng card group/accordion/pill checkbox đặc thù, không theo table/form list chuẩn.
* Ví dụ class/style đang dùng: `phan-quyen-page`, `phan-quyen-header-card`, `permission-group`, `permission-card`, dữ liệu `data-permission`, `data-parent-permission`.
* Giao diện chuẩn nên đối chiếu: `_Layout.cshtml` cho page title/shell; `CongViec` chỉ để đối chiếu spacing/card, không nên ép thành table.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: giữ accordion vì phù hợp nghiệp vụ phân quyền, nhưng đồng bộ button/search/card spacing theo `btn-action`, `card-soft`, `form-control` chuẩn. Không đổi logic dependency permission.

### 5.5 Dự án

* File view: `Views/DuAn/Index.cshtml`, `_Filter.cshtml`, `_Table.cshtml`, `_Form.cshtml`, `Details.cshtml`, `ChiTiet.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/DuAn/index.css`, `wwwroot/css/DuAn/details.css`, inline script trong `_Form.cshtml`, `_Table.cshtml`, `Details.cshtml`.
* Màn hình đang khác so với chuẩn ở điểm: `Index` khá đồng bộ nhưng dùng toàn bộ hệ `du-an-*`; `_Form` còn nhiều inline style; `Details` có CSS riêng rất lớn và nhiều inline style cho progress/animation.
* Ví dụ class/style đang dùng: `du-an-header-card`, `du-an-filter-btn-primary`, `du-an-status-badge`, `style="display:inline;"`, `style="display:none;"`, `style="--delay: 80ms;"`, `style="width: @Model.PhanTramHoanThanh%;"`.
* Giao diện chuẩn nên đối chiếu: `CongViec/Index.cshtml`, `TienDoCongViec/_Filter.cshtml`, `site.css` `app-data-table`.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: ưu tiên `_Form` và `Details`; chuyển inline display sang class `d-inline`/`d-none` hoặc class module, chuẩn hóa `du-an-filter-btn-*` về `btn-filter-apply/reset`, giữ `du-an-status-badge` nhưng ánh xạ màu theo `workflow-*`.

### 5.6 Thành viên dự án / Team dự án / Thành viên team

* File view: `Views/NhanVienDuAn/*`, `Views/TeamDuAn/*`, `Views/ThanhVienTeam/*`.
* File CSS/JS liên quan: `wwwroot/css/ThanhVienDuAn/index.css`, `wwwroot/css/TeamDuAn/index.css`, `wwwroot/css/ThanhVienTeam/index.css`.
* Màn hình đang khác so với chuẩn ở điểm: `NhanVienDuAn` dùng CSS tên `ThanhVienDuAn`, `TeamDuAn` dùng class `team-*` dễ trùng nghĩa với module `Team`, table có `table-bordered`, button/back/status riêng.
* Ví dụ class/style đang dùng: `thanh-vien-du-an-page`, `team-du-an-page`, `team-back-btn`, `team-status-badge`, `thanh-vien-table`, `team-table`, `table-bordered`.
* Giao diện chuẩn nên đối chiếu: `Team`, `NhanSu`, `CongViec`.
* Mức độ ảnh hưởng: Nhẹ-Trung bình.
* Gợi ý sửa: thống nhất tên prefix theo module thật (`nhan-vien-du-an-*` hoặc `thanh-vien-du-an-*`), bỏ `table-bordered` nếu không cần, đưa nút về `btn-action` và table về `app-data-table`.

### 5.7 Danh mục công việc

* File view: `Views/DanhMucCongViec/Index.cshtml`, `_Filter.cshtml`, `_Form.cshtml`, `_Table.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/DanhMucCongViec/index.css`.
* Màn hình đang khác so với chuẩn ở điểm: filter/form/table dùng hệ `danh-muc-*`, table `table-bordered app-data-table`, filter không dùng `workflow-filter`.
* Ví dụ class/style đang dùng: `danh-muc-header-card`, `danh-muc-filter-form`, `danh-muc-btn-primary`, `danh-muc-table-scroll`.
* Giao diện chuẩn nên đối chiếu: `CongViec`, `DuAn`, `TienDoCongViec`.
* Mức độ ảnh hưởng: Nhẹ.
* Gợi ý sửa: đồng bộ filter sang 2 hàng như `workflow-filter`, dùng `btn-filter-apply/reset`, bỏ table border nặng.

### 5.8 Chi tiết công việc

* File view: `Views/ChiTietCongViec/Index.cshtml`, `_Form.cshtml`, `_Table.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/ChiTietCongViec/index.css`, `wwwroot/css/shared/export-dropdown.css`.
* Màn hình đang khác so với chuẩn ở điểm: gần chuẩn `CongViec` nhưng CSS riêng định nghĩa lại `workflow-badge`, `btn-action`, `btn-danger-soft`, mobile card; có inline style progress.
* Ví dụ class/style đang dùng: `chi-tiet-dashboard`, `detail-hero`, `dashboard-card`, `chi-tiet-table`, `style="width:@Model.CongViec.PhanTramTienDo%"`.
* Giao diện chuẩn nên đối chiếu: `CongViec/Index.cshtml` và `CongViec/_Table.cshtml`.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: kế thừa class từ `CongViec` nhiều hơn, chỉ giữ CSS riêng cho phần detail đặc thù; chuyển progress width sang CSS variable nếu chuẩn hóa được.

### 5.9 Phân công công việc / Phân công chi tiết công việc

* File view: `Views/PhanCongCongViec/*`, `Views/PhanCongChiTietCongViec/*`.
* File CSS/JS liên quan: `wwwroot/css/PhanCongCongViec/index.css`, `wwwroot/css/PhanCongChiTietCongViec/index.css`.
* Màn hình đang khác so với chuẩn ở điểm: hai module gần giống nhau nhưng mỗi module có prefix và CSS riêng; table không dùng `app-data-table`; form class `assignment-form-*` bị lặp.
* Ví dụ class/style đang dùng: `pc-header-card`, `pct-header-card`, `pc-table`, `pct-table`, `assignment-form-grid`, `assignment-form-actions`.
* Giao diện chuẩn nên đối chiếu: `CongViec`, `NhanSu`, `Team`.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: gom class chung cho phân công như `assignment-page`, `assignment-table`, `assignment-form`; giữ prefix `pc-`/`pct-` chỉ cho phần thật sự khác.

### 5.10 Đề xuất công việc

* File view: `Views/DeXuatCongViec/Index.cshtml`, `DieuHuong.cshtml`, `_Filter.cshtml`, `_Form.cshtml`, `_Table.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/DeXuatCongViec/index.css`.
* Màn hình đang khác so với chuẩn ở điểm: dùng hệ `dxcv-*` riêng cho toàn bộ page, action, table, badge, warning card; table không dùng `app-data-table`; detail dùng collapse row.
* Ví dụ class/style đang dùng: `dxcv-header-card`, `dxcv-summary-grid`, `dxcv-action-btn`, `dxcv-table`, `dxcv-status-badge`.
* Giao diện chuẩn nên đối chiếu: `DuAn/Index.cshtml` và `CongViec/_Table.cshtml`.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: giữ summary đặc thù nhưng đổi table sang `app-data-table`, action sang `btn-action`, badge sang class trạng thái chung.

### 5.11 Đề xuất ngân sách

* File view: `Views/DeXuatNganSach/Index.cshtml`, `_Filter.cshtml`, `_Form.cshtml`, `_Table.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/DeXuatNganSach/index.css`.
* Màn hình đang khác so với chuẩn ở điểm: gần trùng cấu trúc `DeXuatCongViec` nhưng dùng prefix `dxns-*` và CSS riêng, tạo trùng lặp nhiều rule về summary, card, table, action, status.
* Ví dụ class/style đang dùng: `dxns-header-card`, `dxns-summary-grid`, `dxns-action-btn`, `dxns-table`, `dxns-status-badge`.
* Giao diện chuẩn nên đối chiếu: `DeXuatCongViec` để gom chung trước, sau đó đối chiếu `CongViec/DuAn`.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: tạo class chung `proposal-page`, `proposal-summary-card`, `proposal-action-btn`, `proposal-status-badge` cho cả công việc và ngân sách.

### 5.12 Duyệt đề xuất công việc / Duyệt ngân sách

* File view: `Views/DuyetDeXuatCongViec/Index.cshtml`, `_Filter.cshtml`, `_Table.cshtml`; `Views/DuyetDeXuatNganSach/Index.cshtml`, `_Filter.cshtml`, `_Table.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/DuyetDeXuatCongViec/index.css`, `wwwroot/css/DuyetDeXuatNganSach/index.css`, inline JS trong hai `Index.cshtml`.
* Màn hình đang khác so với chuẩn ở điểm: prefix class rất dài và gần trùng giữa hai module; table/action/status/detail-card đều riêng; JS confirm/toggle detail copy ở cả hai view.
* Ví dụ class/style đang dùng: `duyet-de-xuat-cong-viec-action-btn`, `duyet-de-xuat-ngan-sach-action-btn`, `duyet-de-xuat-*-status-badge`, `js-confirm-submit`, `js-toggle-detail`.
* Giao diện chuẩn nên đối chiếu: `CongViec/_Table.cshtml`, `DeXuatCongViec/_Table.cshtml`, `DeXuatNganSach/_Table.cshtml`.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: ưu tiên gom style duyệt chung `approval-page`, `approval-table`, `approval-action-btn`, `approval-status-badge`; chuyển JS confirm/toggle sang `wwwroot/js/site.js` hoặc `wwwroot/js/approval/index.js`.

### 5.13 NganSach

* File view: `Views/NganSach/Index.cshtml`, `_Filter.cshtml`, `_Table.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/NganSach/index.css`, `wwwroot/css/shared/export-dropdown.css`.
* Màn hình đang khác so với chuẩn ở điểm: dùng class riêng `ngan-sach-*`, table riêng không dùng `app-data-table`.
* Ví dụ class/style đang dùng: `ngan-sach-header-card`, `ngan-sach-filter-form`, `ngan-sach-table`, `ngan-sach-status-badge`.
* Giao diện chuẩn nên đối chiếu: `DuAn/Index.cshtml`, `DeXuatNganSach`, `CongViec/_Table.cshtml`.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: đổi table sang `app-data-table`, gom badge ngân sách vào hệ badge chung, giữ stat/header riêng nếu cần.

### 5.14 Chi phí

* File view: chưa thấy `Views/ChiPhi/*` trong source đã đọc.
* File CSS/JS liên quan: chưa xác định rõ, cần kiểm tra thêm.
* Màn hình đang khác so với chuẩn ở điểm: `_Layout.cshtml` có menu `Chi phí`, `Dashboard/Index.cshtml` có link `ChiPhi/Index`, nhưng không có view tương ứng trong `Views`.
* Ví dụ class/style đang dùng: chưa xác định rõ, cần kiểm tra thêm.
* Giao diện chuẩn nên đối chiếu: nếu bổ sung view, nên đối chiếu `NganSach` và `CongViec`.
* Mức độ ảnh hưởng: Chưa xác định.
* Gợi ý sửa: kiểm tra controller/action và view runtime; nếu view sinh từ nơi khác thì bổ sung vào lần rà soát sau.

### 5.15 Đánh giá dự án

* File view: `Views/DanhGiaDuAn/Index.cshtml`, `ChiTiet.cshtml`, `_Filter.cshtml`, `_Form.cshtml`, `_Table.cshtml`, `_SummaryCards.cshtml`, `_ScoreBadge.cshtml`, `_ProjectStats.cshtml`, `_AiInsightCard.cshtml`, `_EmptyState.cshtml`, `_TrangThaiDanhGiaBadge.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/DanhGiaDuAn/index.css`, `table.css`, `form.css`, `summary-cards.css`, `score-badge.css`, `empty-state.css`, `filter.css`.
* Màn hình đang khác so với chuẩn ở điểm: nhiều CSS tách file nhưng `index.css` cũng định nghĩa lại nhiều class như `dgda-table`, `score-badge`, `summary-item`, `criteria-row`, `mini-stat`. Có nguy cơ trùng/ghi đè.
* Ví dụ class/style đang dùng: `danh-gia-du-an-page`, `danh-gia-card`, `dgda-filter`, `dgda-table`, `score-badge`, `status-badge`.
* Giao diện chuẩn nên đối chiếu: `CongViec` cho filter/table; `DanhGiaNhanVien` để gom style đánh giá chung.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: gom style chung cho đánh giá vào `wwwroot/css/DanhGia/shared.css` hoặc `wwwroot/css/Evaluation/index.css`; bỏ file tách 1-3 dòng nếu đã được index bao phủ.

### 5.16 Đánh giá nhân viên

* File view: `Views/DanhGiaNhanVien/Index.cshtml`, `ChiTiet.cshtml`, `_Filter.cshtml`, `_Form.cshtml`, `_Table.cshtml`, `_SummaryCards.cshtml`, `_ScoreBadge.cshtml`, `_EmployeeStats.cshtml`, `_EmptyState.cshtml`, `_TrangThaiDanhGiaBadge.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/DanhGiaNhanVien/index.css`, `filter.css`, `table.css`, `form.css`, `summary-cards.css`, `score-badge.css`, `empty-state.css`, `employee-stats.css`.
* Màn hình đang khác so với chuẩn ở điểm: gần trùng hệ `DanhGiaDuAn` nhưng dùng prefix `dgnv-*`; import quá nhiều CSS trong `Index.cshtml`.
* Ví dụ class/style đang dùng: `danh-gia-nhan-vien-page`, `dgnv-card`, `dgnv-filter`, `dgnv-table`, `score-badge`, `summary-item`.
* Giao diện chuẩn nên đối chiếu: `DanhGiaDuAn` để gom chung; `CongViec` cho table/action.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: chuẩn hóa thành hệ `evaluation-*`, chỉ giữ `dgda-*`/`dgnv-*` cho phần dữ liệu riêng; giảm số CSS import trong view.

### 5.17 Chat

* File view: `Views/ChatDuAn/Index.cshtml`, `_RoomList.cshtml`, `_ChatHeader.cshtml`, `_MessageList.cshtml`, `_MessageItem.cshtml`, `_MessageForm.cshtml`, `_EmptyState.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/ChatDuAn/index.css`, `room-list.css`, `chat-header.css`, `message-list.css`, `message-item.css`, `message-form.css`, `empty-state.css`.
* Màn hình đang khác so với chuẩn ở điểm: chat là UI đặc thù, tách 7 file CSS, không dùng card/table/form chuẩn nhiều; nút search và send dùng Bootstrap/riêng lẫn nhau.
* Ví dụ class/style đang dùng: `chat-du-an-page`, `chat-layout`, `chat-sidebar`, `chat-room-list`, `chat-message-form`, `message-bubble`.
* Giao diện chuẩn nên đối chiếu: chỉ đối chiếu token màu, radius, button, input từ `site.css`; không nên ép chat thành table/card list.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: gom CSS Chat thành 1-2 file (`index.css`, `messages.css`) hoặc giữ tách file nhưng quy ước rõ; đồng bộ input/button với `form-control`, `btn-action`.

### 5.18 AI Dataset / AI Train / Model / Phân tích nguyên nhân trễ

* File view: `Views/AiDataset/Index.cshtml`, `Views/Ai/Train.cshtml`, `Models.cshtml`, `Predict.cshtml`, `Dashboard.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/AI/index.css`, `wwwroot/css/shared/export-dropdown.css`, `wwwroot/js/ai/charts.js`.
* Màn hình đang khác so với chuẩn ở điểm: dùng hệ `ai-*` riêng cho card/grid/form/table/badge; một số action dùng `ai-link-btn`, `ai-primary-btn` thay vì `btn-action`; chart/table dùng class riêng.
* Ví dụ class/style đang dùng: `ai-page`, `ai-header-card`, `ai-panel-card`, `ai-table`, `ai-badge-ok`, `ai-badge-warn`, `ai-chart-shell`.
* Giao diện chuẩn nên đối chiếu: `CongViec` cho table/action; `Dashboard` cho chart card.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: giữ màu/nhận diện AI ở mức header hoặc badge, nhưng chuẩn hóa button/table/form về class chung; dùng `ai-*` cho phần chart/model đặc thù.

### 5.19 Yêu cầu đổi quản lý / Duyệt yêu cầu đổi quản lý

* File view: `Views/YeuCauDoiQuanLy/*`, `Views/DuyetYeuCauDoiQuanLy/*`.
* File CSS/JS liên quan: `wwwroot/css/YeuCauDoiQuanLy/index.css`, `wwwroot/css/DuyetYeuCauDoiQuanLy/index.css`.
* Màn hình đang khác so với chuẩn ở điểm: hai module dùng hệ class riêng `yeu-cau-*` và `duyet-yeu-cau-*`, table riêng, status/action riêng.
* Ví dụ class/style đang dùng: `yeu-cau-header-card`, `yeu-cau-table`, `yeu-cau-status-badge`, `duyet-yeu-cau-table`, `duyet-yeu-cau-action-btn`.
* Giao diện chuẩn nên đối chiếu: `DuyetDeXuatCongViec` nếu gom nhóm duyệt; `CongViec` cho table/action chung.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: gom chung các màn yêu cầu/duyệt theo pattern `request-page`, `approval-page`; table nên thêm `app-data-table`.

### 5.20 Tài khoản cá nhân

* File view: `Views/TaiKhoanCaNhan/Index.cshtml`, `CapNhat.cshtml`, `DoiMatKhau.cshtml`.
* File CSS/JS liên quan: `wwwroot/css/TaiKhoanCaNhan/index.css`.
* Màn hình đang khác so với chuẩn ở điểm: CSS `TaiKhoanCaNhan/index.css` được import toàn cục trong `_Layout.cshtml`, trong khi phần lớn rule chỉ phục vụ trang tài khoản và dropdown.
* Ví dụ class/style đang dùng: `tai-khoan-ca-nhan-page`, `tai-khoan-header-card`, `tai-khoan-card`, `tai-khoan-dropdown`, `tai-khoan-menu`.
* Giao diện chuẩn nên đối chiếu: `_Layout.cshtml` cho dropdown; page account nên import CSS riêng bằng `@section Styles`.
* Mức độ ảnh hưởng: Trung bình.
* Gợi ý sửa: tách CSS dropdown tài khoản thành `layout/account-menu.css` import toàn cục; CSS trang hồ sơ chỉ import trong `TaiKhoanCaNhan` views.

## 6. Các CSS đang bị phân tán hoặc trùng lặp

CSS chính nên coi là nền tảng:

* `wwwroot/css/site.css`: biến màu, layout shell, sidebar cơ bản, `card-soft`, `app-data-table`, `table-actions`, badge mềm, auth card cơ bản.
* `wwwroot/css/layout/sidebar.css`: style sidebar bổ sung.
* `wwwroot/css/shared/export-dropdown.css`: dropdown export dùng lại ở nhiều màn.

CSS đang phân tán mạnh:

* `wwwroot/css/DanhGiaDuAn/*`: 7 file, có file chỉ 1-3 dòng và có rule trùng với `index.css`.
* `wwwroot/css/DanhGiaNhanVien/*`: 8 file, gần trùng mô hình `DanhGiaDuAn`.
* `wwwroot/css/ChatDuAn/*`: 7 file cho một màn chat.
* `wwwroot/css/DuAn/details.css`: 1086 dòng, lớn hơn nhiều module khác.
* `wwwroot/css/TaiKhoanCaNhan/index.css`: 641 dòng và đang import toàn cục qua `_Layout.cshtml`.
* `wwwroot/css/CongViec/index.css`, `TienDoCongViec/index.css`, `DuAn/index.css`, `Dashboard/index.css`, `AI/index.css`: đều lớn và tự định nghĩa nhiều token/card/button riêng.

Class có khả năng trùng chức năng:

* Button/action: `btn-action`, `btn-subtle`, `btn-confirm`, `btn-reopen`, `btn-danger-soft`, `dxcv-action-btn`, `dxns-action-btn`, `du-an-primary-btn`, `du-an-filter-btn-primary`, `duyet-de-xuat-*-action-btn`, `yeu-cau-action-btn`, `ngan-sach-header-action`, `team-btn-primary`, `thanh-vien-btn-primary`, `ai-link-btn`, `ai-primary-btn`.
* Table wrapper: `table-responsive-soft`, `workflow-table-wrapper`, `tien-do-table-scroll`, `du-an-table-scroll`, `dxcv-table-scroll`, `dxns-table-scroll`, `ngan-sach-table-scroll`, `duyet-de-xuat-*-table-scroll`, `yeu-cau-table-scroll`.
* Badge/status: `workflow-badge`, `du-an-status-badge`, `dxcv-status-badge`, `dxns-status-badge`, `ngan-sach-status-badge`, `td-report-badge`, `td-ctcv-badge`, `duyet-de-xuat-*-status-badge`, `status-badge`, `score-badge`.
* Card/header: `dashboard-card`, `du-an-section-card`, `dxcv-section-card`, `dxns-section-card`, `ngan-sach-section-card`, `team-section-card`, `thanh-vien-section-card`, `danh-gia-card`, `ai-panel-card`.
* Filter: `workflow-filter`, `td-filter-form`, `du-an-filter-form`, `dxcv-filter-form`, `dxns-filter-form`, `ngan-sach-filter-form`, `duyet-de-xuat-*-filter-form`, `dgda-filter`, `dgnv-filter`.

Class chỉ dùng cho 1 màn nhưng nên đưa về class chung:

* `dxcv-action-btn` và `dxns-action-btn`: nên gom thành `proposal-action-btn` hoặc dùng `btn-action`.
* `duyet-de-xuat-cong-viec-action-btn` và `duyet-de-xuat-ngan-sach-action-btn`: nên gom thành `approval-action-btn`.
* `pc-table` và `pct-table`: nên gom thành `assignment-table`.
* `dgda-table` và `dgnv-table`: nên gom thành `evaluation-table`.
* `dxcv-status-*`, `dxns-status-*`, `duyet-de-xuat-*-status-badge`: nên map về token trạng thái chung.

Inline style nên chuyển vào CSS/class:

* `Views/Dashboard/Index.cshtml`: progress width trong `status-pill-card` dùng `style="width: ...%"`.
* `Views/DuAn/Details.cshtml`: progress width, animation delay `--delay`, progress bar width.
* `Views/DuAn/_Form.cshtml`: `style="display:none;"`, nhiều `style="display:inline;"`.
* `Views/ChiTietCongViec/Index.cshtml`: progress line width.
* `Views/TienDoCongViec/_Table.cshtml`: progress bar width.

## 7. Đề xuất chuẩn hóa giao diện

File CSS nên làm chuẩn chính:

* Giữ `wwwroot/css/site.css` làm file nền tảng cho layout, token màu, card/table/button/form/badge chung.
* Tách thêm `wwwroot/css/shared/ui.css` nếu muốn giảm tải `site.css`; file này chứa `app-page`, `app-page-header`, `app-section-card`, `app-filter`, `app-action-btn`, `app-status-badge`, `app-table-scroll`.
* Giữ `wwwroot/css/shared/export-dropdown.css` cho export.
* Giữ CSS module chỉ cho phần nghiệp vụ đặc thù.

Class dùng chung nên có:

* Page: `app-page`, `app-page-header`, `app-page-title`, `app-page-subtitle`.
* Card/container: `app-section-card`, `app-stat-grid`, `app-stat-card`, `app-empty-state`.
* Filter/form: `app-filter`, `app-filter-main`, `app-filter-time`, `app-filter-field`, `btn-filter-apply`, `btn-filter-reset`.
* Table: `app-table-scroll`, `app-data-table`, `app-table-actions`, `col-actions`.
* Button: `btn-action`, `btn-subtle`, `btn-confirm`, `btn-reopen`, `btn-danger-soft`.
* Badge: `status-badge`, `status-pending`, `status-approved`, `status-rejected`, `status-done`, `status-active`, `status-paused`, `status-neutral`.

Quy ước đặt class:

* Class chung dùng prefix `app-*`, `btn-*`, `status-*`.
* Class module dùng prefix ngắn theo module thật nhưng chỉ cho phần đặc thù, ví dụ `du-an-*`, `ai-*`, `chat-*`.
* Không tạo prefix quá dài cho pattern dùng lại, ví dụ nên tránh nhân rộng `duyet-de-xuat-cong-viec-*` nếu có thể dùng `approval-*`.
* Table list mặc định thêm `app-data-table`; module table chỉ bổ sung width/column riêng.

CSS nên gom:

* Gom `DeXuatCongViec/index.css` và `DeXuatNganSach/index.css` phần card/action/table/badge chung thành nhóm proposal shared.
* Gom `DuyetDeXuatCongViec/index.css`, `DuyetDeXuatNganSach/index.css`, `DuyetYeuCauDoiQuanLy/index.css` phần duyệt chung thành nhóm approval shared.
* Gom `PhanCongCongViec/index.css` và `PhanCongChiTietCongViec/index.css` phần assignment chung.
* Gom `DanhGiaDuAn/*` và `DanhGiaNhanVien/*` phần evaluation chung.
* Tách `TaiKhoanCaNhan/index.css` thành CSS layout account-menu và CSS page tài khoản.

CSS nên bỏ hoặc đổi tên:

* Các file 1-3 dòng trong `DanhGiaDuAn`/`DanhGiaNhanVien` nếu rule đã có trong `index.css`.
* Prefix dài `duyet-de-xuat-cong-viec-*`, `duyet-de-xuat-ngan-sach-*` nên đổi dần sang `approval-*`.
* `team-*` trong `TeamDuAn` nên đổi hoặc scope rõ hơn để không lẫn với module `Team`.

Không đề xuất thay đổi nghiệp vụ. Các đề xuất trên chỉ nhằm đồng bộ UI/CSS.

## 8. Danh sách ưu tiên sửa

| Ưu tiên | Chức năng | Lý do cần sửa trước | Hướng xử lý ngắn gọn |
| --- | --- | --- | --- |
| 1 | Dự án Details/Form | `DuAn/details.css` rất lớn, nhiều inline style; `_Form` có inline display và modal/action riêng | Chuẩn hóa inline style, giữ logic workflow, đưa progress/modal/action về class chung |
| 2 | Đề xuất công việc + Đề xuất ngân sách | Hai module gần trùng UI nhưng tách `dxcv-*` và `dxns-*` | Tạo shared proposal CSS, chuẩn hóa action/table/badge |
| 3 | Duyệt đề xuất công việc + Duyệt ngân sách | Hai màn gần copy, prefix dài, JS inline lặp | Tạo shared approval CSS/JS, dùng `app-data-table` và `btn-action` |
| 4 | Đánh giá dự án + Đánh giá nhân viên | Nhiều CSS tách nhỏ và class trùng `score-badge`, `summary-item`, `criteria-row` | Gom shared evaluation CSS, giảm import trong view |
| 5 | Tài khoản cá nhân | CSS page đang import toàn cục qua layout | Tách dropdown account-menu khỏi CSS trang tài khoản |
| 6 | Phân công công việc + Phân công chi tiết | CSS gần trùng `pc-*`/`pct-*`, table chưa dùng `app-data-table` | Gom assignment shared, chuẩn hóa table/action |
| 7 | Chat | 7 file CSS cho một màn đặc thù | Gom còn 1-2 file hoặc lập quy ước rõ, đồng bộ token input/button |
| 8 | NganSach/YeuCauDoiQuanLy/DuyetYeuCauDoiQuanLy | Table/action/badge riêng, chưa dùng nhiều class chung | Thêm `app-data-table`, map badge/action về chuẩn |
| 9 | Danh mục công việc/Chức danh/TeamDuAn/NhanVienDuAn | Lệch nhẹ: `table-bordered`, filter/button riêng | Bỏ border nặng, dùng filter/action chuẩn |
| 10 | Dashboard/AI | Có đặc thù chart/AI nhưng vẫn lệch token button/table/card | Chuẩn hóa phần button/table/card, giữ phần chart/model đặc thù |

## 9. Kết luận

Hệ thống đã có nền UI khá rõ trong `_Layout.cshtml`, `site.css`, `CongViec`, `TienDoCongViec`, `DuAn`, `NhanSu`: layout sidebar/topbar, card trắng shadow nhẹ, filter 2 hàng, table có wrapper, action button có icon, badge nền nhạt.

Vấn đề chính không phải thiếu CSS, mà là CSS đang phân tán theo từng module và nhiều module tự tạo lại cùng một loại button/table/badge/card. Những màn nên ưu tiên chuẩn hóa trước là các cặp có trùng lặp rõ: đề xuất công việc/ngân sách, duyệt đề xuất công việc/ngân sách, đánh giá dự án/nhân viên, phân công công việc/phân công chi tiết. Sau đó xử lý các màn có CSS lớn hoặc import toàn cục như `DuAn/details.css` và `TaiKhoanCaNhan/index.css`.

Không thấy cần thay đổi nghiệp vụ để đồng bộ giao diện. Hướng sửa nên là gom CSS dùng chung, đổi class về chuẩn, giảm inline style, và chỉ giữ CSS module cho phần đặc thù thật sự.
