# Phân tích chức năng thu gọn menu sidebar

## 1. Phạm vi source đã kiểm tra

Phạm vi đã kiểm tra trong source thực tế:

| STT | File | Vai trò hiện tại | Có cần sửa dự kiến không | Lý do |
| --- | ---- | ---------------- | ------------------------ | ----- |
| 1 | QuanLyDuAn/QuanLyDuAn/Views/_ViewStart.cshtml | Khai báo layout mặc định `Layout = "_Layout"` cho các View MVC. | Không | Chỉ cần giữ layout mặc định, không cần đổi trỏ layout. |
| 2 | QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml | Layout chính, chứa trực tiếp sidebar, topbar, content, menu theo quyền, script mobile sidebar và account menu. | Có | Cần thêm nút desktop collapse, class/thuộc tính hỗ trợ trạng thái collapsed, tooltip/title/ARIA nếu triển khai. |
| 3 | QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml.css | File scoped CSS rỗng chức năng, chỉ ghi chú CSS layout đã chuyển sang `wwwroot/css/layout/sidebar.css`. | Không | Không còn chứa rule layout/sidebar thực tế. |
| 4 | QuanLyDuAn/QuanLyDuAn/wwwroot/css/site.css | CSS nền tảng: `.app-shell`, `.app-main`, `.app-content`, bảng chung `.app-data-table`, style brand/menu nền cũ. | Có thể | Nếu muốn đặt biến CSS toàn cục hoặc điều chỉnh shell/content chung; nên ưu tiên ít sửa và tránh trùng với `layout/sidebar.css`. |
| 5 | QuanLyDuAn/QuanLyDuAn/wwwroot/css/layout/sidebar.css | CSS chính của sidebar, active menu, toggle mobile, backdrop, responsive mobile. | Có | Nơi phù hợp nhất để thêm width collapsed, ẩn chữ, căn icon, tooltip CSS và breakpoint desktop/mobile. |
| 6 | QuanLyDuAn/QuanLyDuAn/wwwroot/css/layout/account-menu.css | CSS menu tài khoản topbar. | Không | Không liên quan đến chiều rộng sidebar, chỉ cần tránh ảnh hưởng topbar. |
| 7 | QuanLyDuAn/QuanLyDuAn/wwwroot/js/site.js | JS xác nhận form POST toàn hệ thống. | Không hoặc có thể | Hiện không xử lý sidebar; nếu muốn gom logic layout vào file JS chung thì có thể thêm, nhưng tốt hơn là tách file layout/sidebar riêng nếu chấp nhận thêm file. |
| 8 | QuanLyDuAn/QuanLyDuAn/wwwroot/js/approval/index.js | JS riêng màn hình duyệt. | Không | Không liên quan layout/sidebar. |
| 9 | QuanLyDuAn/QuanLyDuAn/wwwroot/js/phanquyen/index.js | JS riêng phân quyền. | Không | Không liên quan layout/sidebar. |
| 10 | QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/ui.css | CSS khung trang chung: `.app-page`, `.app-section-card`, `.app-table-scroll`. | Không | Đã có `width: 100%` và `overflow-x: auto` cho bảng, không cần sửa để collapse sidebar. |
| 11 | QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/proposal.css | CSS chung đề xuất, bảng min-width 980px. | Không | Bảng đã cuộn ngang trong wrapper, chỉ hưởng lợi từ content rộng hơn. |
| 12 | QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/approval.css | CSS chung duyệt, bảng min-width 920px. | Không | Không cần đổi nghiệp vụ hay bảng. |
| 13 | QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/evaluation.css | CSS chung đánh giá, bảng min-width 980px. | Không | Layout chung rộng hơn là đủ, không cần sửa module. |
| 14 | QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/assignment.css | CSS chung phân công, bảng min-width 720px. | Không | Không liên quan trực tiếp sidebar. |
| 15 | QuanLyDuAn/QuanLyDuAn/Views/Account/Login.cshtml | Trang đăng nhập dùng `Layout = null`. | Không | Không đi qua sidebar/layout chính. |
| 16 | QuanLyDuAn/QuanLyDuAn/Views/Account/ForgotPassword.cshtml | Trang quên mật khẩu dùng `Layout = null`. | Không | Không đi qua sidebar/layout chính. |
| 17 | QuanLyDuAn/QuanLyDuAn/Views/Account/ResetPassword.cshtml | Trang đặt lại mật khẩu dùng `Layout = null`. | Không | Không đi qua sidebar/layout chính. |
| 18 | QuanLyDuAn/QuanLyDuAn/Views/Account/VerifyOtp.cshtml | Trang OTP dùng `Layout = null`. | Không | Không đi qua sidebar/layout chính. |
| 19 | QuanLyDuAn/QuanLyDuAn/Views/Account/Activate.cshtml | Trang kích hoạt dùng `Layout = null`. | Không | Không đi qua sidebar/layout chính. |
| 20 | QuanLyDuAn/QuanLyDuAn/Views/CongViec/Index.cshtml và _Table.cshtml | Màn hình Công việc, có bảng desktop trong `.table-responsive-soft`. | Không | Bị ảnh hưởng bởi bề rộng content nhưng không cần sửa View. |
| 21 | QuanLyDuAn/QuanLyDuAn/Views/ChiTietCongViec/Index.cshtml và _Table.cshtml | Màn hình Chi tiết công việc, dùng bảng `.app-data-table`. | Không | Chỉ cần layout chung rộng hơn. |
| 22 | QuanLyDuAn/QuanLyDuAn/Views/TienDoCongViec/Index.cshtml và _Table.cshtml | Màn hình Tiến độ, bảng trong `.table-responsive tien-do-table-scroll`. | Không | Đã có wrapper cuộn ngang. |
| 23 | QuanLyDuAn/QuanLyDuAn/Views/DuAn/Index.cshtml, Details.cshtml và _Table.cshtml | Màn hình Dự án và chi tiết dự án. | Không | Dùng CSS module riêng, hưởng lợi từ content rộng hơn. |
| 24 | QuanLyDuAn/QuanLyDuAn/Views/NhanSu/Index.cshtml và _Table.cshtml | Màn hình Nhân sự. | Không | Không cần sửa cho sidebar collapsed. |
| 25 | QuanLyDuAn/QuanLyDuAn/Views/NhanVienDuAn/Index.cshtml và _Table.cshtml | Màn hình Nhân viên dự án, dùng CSS `ThanhVienDuAn/index.css`. | Không | Bảng có min-width và wrapper CSS riêng. |
| 26 | QuanLyDuAn/QuanLyDuAn/Views/DeXuatCongViec/Index.cshtml và _Table.cshtml | Màn hình Đề xuất công việc. | Không | Bảng dùng `.app-table-scroll` và CSS proposal. |
| 27 | QuanLyDuAn/QuanLyDuAn/Views/DeXuatNganSach/Index.cshtml và _Table.cshtml | Màn hình Đề xuất ngân sách. | Không | Bảng dùng `.app-table-scroll` và CSS proposal. |
| 28 | QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatCongViec/Index.cshtml và _Table.cshtml | Màn hình Duyệt đề xuất công việc. | Không | Bảng dùng `.app-table-scroll` và CSS approval. |
| 29 | QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatNganSach/Index.cshtml và _Table.cshtml | Màn hình Duyệt đề xuất ngân sách. | Không | Bảng dùng `.app-table-scroll` và CSS approval. |
| 30 | QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/Index.cshtml và _Table.cshtml | Màn hình Đánh giá dự án. | Không | Bảng dùng `.evaluation-table-wrap`. |
| 31 | QuanLyDuAn/QuanLyDuAn/Views/DanhGiaNhanVien/Index.cshtml và _Table.cshtml | Màn hình Đánh giá nhân viên. | Không | Bảng dùng `.evaluation-table-wrap`. |
| 32 | QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/Index.cshtml và CSS ChatDuAn | Màn hình Chat dự án, có sidebar nội bộ của chat. | Không | Sidebar chat là thành phần module riêng, không phải sidebar layout. |

## 2. Layout đang được sử dụng

### 2.1 Layout chính

Layout chính là `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml`. File `QuanLyDuAn/QuanLyDuAn/Views/_ViewStart.cshtml` đặt `Layout = "_Layout"`, nên phần lớn View MVC dùng layout này theo mặc định.

Trong `_Layout.cshtml`, layout chia thành:

- `<div class="app-shell">` là khung ngoài.
- `<aside class="app-sidebar" id="appSidebar">` là sidebar khi người dùng đã đăng nhập.
- `<div class="app-main">` chứa topbar và nội dung.
- `<header class="app-topbar">` là thanh trên.
- `<main class="app-content">` chứa `@RenderBody()`.

Không thấy layout riêng cho nhóm role hoặc module AI. Các trang Account như Login, ForgotPassword, ResetPassword, VerifyOtp, Activate dùng `Layout = null`, tự khai báo CSS/icon riêng và không dùng sidebar.

### 2.2 Partial View liên quan

Không có `Views/Shared/_Sidebar.cshtml`, `_Menu.cshtml` hoặc `_Navigation.cshtml` trong thư mục `Views/Shared`. Sidebar được viết trực tiếp trong `_Layout.cshtml`, không tách partial.

Các partial trong `Views/Shared` hiện có `_ExportDropdown.cshtml`, `_Pagination.cshtml`, `_ValidationScriptsPartial.cshtml`, không phải partial layout/sidebar.

### 2.3 Cách các View sử dụng layout

Các View chức năng không khai báo layout riêng, nên nhận `_Layout` từ `_ViewStart.cshtml`. Các View chức năng thường chỉ bổ sung CSS qua `@section Styles`, ví dụ:

- `Views/CongViec/Index.cshtml` nạp `wwwroot/css/CongViec/index.css`.
- `Views/DuAn/Index.cshtml` nạp `wwwroot/css/DuAn/index.css`.
- `Views/TienDoCongViec/Index.cshtml` nạp `wwwroot/css/TienDoCongViec/index.css`.
- `Views/DanhGiaDuAn/Index.cshtml` và `Views/DanhGiaNhanVien/Index.cshtml` nạp CSS shared evaluation và CSS module.
- `Views/ChatDuAn/Index.cshtml` nạp nhóm CSS ChatDuAn riêng.

Do đó, thay đổi sidebar nên đặt ở layout/CSS/JS layout chung, không cần sửa từng View chức năng.

## 3. Cấu trúc HTML của sidebar hiện tại

### 3.1 Logo

Logo nằm trong:

```html
<a asp-controller="Dashboard" asp-action="Index" class="brand-wrap text-decoration-none">
    <span class="brand-logo"><i class="bi bi-rocket-takeoff-fill"></i></span>
    <div>
        <h1 class="brand-title">QuanLyDuAn AI</h1>
        <p class="brand-subtitle">Project Intelligence</p>
    </div>
</a>
```

Khi thu gọn, phần nên giữ là `.brand-logo`; phần nên ẩn là khối chữ chứa `.brand-title` và `.brand-subtitle`.

### 3.2 Nhóm menu

Menu nằm trong `<nav class="side-nav sidebar-menu">`. Các nhóm được render bằng `.sidebar-section` và tiêu đề nhóm `.sidebar-section-title`.

Các nhóm hiện có:

- HỆ THỐNG
- DỰ ÁN
- CÔNG VIỆC
- TÀI CHÍNH
- ĐÁNH GIÁ
- TRAO ĐỔI
- AI

Các nhóm chỉ render khi có ít nhất một quyền tương ứng. Ví dụ nhóm HỆ THỐNG chỉ render nếu có quyền Dashboard, Nhân sự, Chức danh, Team hoặc Phân quyền.

### 3.3 Menu item

Mỗi menu item là thẻ `<a>` có class `side-link sidebar-menu-link`. Bên trong gồm:

- `.sidebar-menu-icon` chứa icon Bootstrap Icons.
- `.sidebar-menu-text` chứa tên chức năng.

Không thấy submenu/dropdown/collapse trong sidebar chính. Menu hiện là danh sách phẳng theo nhóm, nên thu gọn chỉ còn icon ít rủi ro hơn so với hệ thống có nhiều cấp menu.

### 3.4 Trạng thái active

Active được xác định server-side trong `_Layout.cshtml` bằng:

- `ViewContext.RouteData.Values["controller"]`
- `ViewContext.RouteData.Values["action"]`
- helper cục bộ `IsActiveController(controllerName)`
- helper cục bộ `IsActiveRoute(controllerName, actionName)`

Khi active, link nhận class `active sidebar-active`. CSS trong `wwwroot/css/layout/sidebar.css` đổi nền gradient, đổi màu icon/text và thêm thanh chỉ báo `::before` bên trái.

Khi sidebar thu gọn, active vẫn có thể giữ bằng nền và thanh chỉ báo, nhưng cần kiểm tra lại vị trí `::before` vì hiện đang đặt `left: -0.7rem`.

### 3.5 Menu theo permission

`_Layout.cshtml` inject `IPhanQuyenService`, lấy danh sách quyền bằng `GetGrantedPermissionNamesAsync(User!)`, sau đó render menu theo các biến `canDashboard`, `canNhanSu`, `canDuAn`, `canCongViec`, `canTienDo`, `canDuyetNganSach`, `canAiDataset`, v.v.

Thu gọn sidebar không cần thay đổi permission vì điều kiện render vẫn nằm server-side. Không thấy group heading rỗng trong layout hiện tại vì mỗi nhóm đều có điều kiện bao ngoài. Khi collapsed, nên ẩn `.sidebar-section-title` để tiết kiệm chiều ngang và tránh chữ chiếm chỗ.

## 4. CSS layout hiện tại

### 4.1 Chiều rộng sidebar

`wwwroot/css/site.css` có `.app-sidebar { width: 260px; ... }`, nhưng `wwwroot/css/layout/sidebar.css` được nạp sau `site.css` trong `_Layout.cshtml` và ghi đè bằng:

```css
.app-sidebar {
    position: sticky;
    top: 0;
    height: 100vh;
    overflow-y: auto;
    width: 272px;
    padding: 1rem 0.85rem 1.15rem;
}
```

Vì vậy chiều rộng sidebar thực tế ở desktop là 272px.

### 4.2 Cách bố trí nội dung chính

`.app-shell` dùng Flexbox:

```css
.app-shell {
    display: flex;
    min-height: 100vh;
}
```

`.app-main` dùng:

```css
.app-main {
    flex: 1;
    display: flex;
    flex-direction: column;
    min-width: 0;
}
```

Không thấy `margin-left` cố định cho content. Vì sidebar là flex item, nếu giảm width của `.app-sidebar`, `.app-main` có thể tự mở rộng theo flex mà không cần sửa Controller/backend/từng View.

### 4.3 Responsive breakpoint

Có hai nguồn CSS breakpoint:

- `site.css` dùng `@media (max-width: 992px)` và đổi `.app-shell` sang `flex-direction: column`, sidebar `width: 100%`.
- `layout/sidebar.css` dùng `@media (max-width: 991.98px)` và chuyển sidebar thành `position: fixed`, `width: min(84vw, 312px)`, ẩn bằng `transform: translateX(-106%)`.

Do `layout/sidebar.css` nạp sau, cơ chế mobile overlay trong file này là cơ chế thực tế quan trọng hơn.

### 4.4 CSS liên quan đến mobile

Mobile hiện có:

- `.app-sidebar` fixed, trượt từ trái vào.
- `body.sidebar-open .app-sidebar { transform: translateX(0); }`.
- `.sidebar-backdrop` fixed toàn màn hình, chỉ hiện khi `body.sidebar-open`.
- `body.sidebar-open { overflow: hidden; }`.
- `.sidebar-toggle` chỉ hiện dưới desktop; trong `@media (min-width: 992px)` bị `display: none`.

Điều này phù hợp hướng giữ mobile overlay riêng, không áp dụng trạng thái icon-only trên mobile.

### 4.5 CSS có thể tái sử dụng

Có thể tái sử dụng các class hiện có:

- `.sidebar-menu-icon` đã có kích thước cố định `1.8rem`, flex basis cố định, căn giữa tốt.
- `.sidebar-menu-text` tách riêng, có thể ẩn khi collapsed.
- `.sidebar-section-title` tách riêng, có thể ẩn khi collapsed.
- `.brand-logo`, `.brand-title`, `.brand-subtitle` tách riêng, có thể ẩn chữ logo khi collapsed.
- `.app-main { min-width: 0; }` và các wrapper bảng `overflow-x: auto` giúp giảm nguy cơ vỡ layout.

Chưa thấy class collapsed hiện có như `sidebar-collapsed`, `app-sidebar-collapsed` hoặc CSS cũ bỏ dở cho thu gọn desktop.

## 5. JavaScript hiện tại

### 5.1 Toggle desktop

Chưa có toggle desktop để thu gọn/mở rộng sidebar. Nút hiện tại:

```html
<button type="button" class="sidebar-toggle d-lg-none" data-sidebar-toggle aria-label="Mở menu">
```

Class `d-lg-none` và CSS `@media (min-width: 992px) { .sidebar-toggle { display: none; } }` làm nút này chỉ phục vụ mobile/tablet dưới 992px.

### 5.2 Menu mobile

`_Layout.cshtml` có script inline:

- Tìm `[data-sidebar-toggle]`.
- Tìm `[data-sidebar-dismiss]`.
- Toggle class `sidebar-open` trên `body`.
- Bấm backdrop đóng sidebar.
- Nhấn Escape đóng sidebar.
- Khi resize lên `window.innerWidth >= 992`, gọi `setSidebarState(false)`.

Đây là cơ chế mobile overlay hiện tại. Chưa thấy logic tự đóng sidebar sau khi bấm menu item trên mobile.

### 5.3 Lưu trạng thái

Không thấy `localStorage`, `sessionStorage` hoặc cookie dùng cho trạng thái sidebar trong source layout/site JS. `site.js` hiện chỉ xử lý xác nhận form POST.

Với yêu cầu giữ trạng thái khi chuyển trang/tải lại, `localStorage` là phù hợp nhất vì:

- Không cần backend.
- Không cần database/migration.
- Không phụ thuộc tài khoản.
- Phù hợp trạng thái giao diện trên cùng trình duyệt.

Key đề xuất: `QuanLyDuAnAI.sidebar.collapsed`. Nếu muốn tránh nhấp nháy khi tải trang, cần áp dụng class sớm trước khi CSS/layout render hoàn chỉnh, ví dụ script nhỏ trong `<head>` hoặc đầu `<body>` thêm class vào `<html>`/`body` theo localStorage.

### 5.4 Các script liên quan khác

Layout nạp:

- jQuery từ `~/lib/jquery/dist/jquery.min.js`.
- Bootstrap bundle từ `~/lib/bootstrap/dist/js/bootstrap.bundle.min.js`.
- `~/js/site.js`.

Bootstrap Collapse đang được dùng trong một số bảng/module, nhưng sidebar chính không dùng Bootstrap Collapse/Offcanvas. Không thấy AJAX/PJAX/HTMX/Turbo làm layout chuyển trang một phần; navigation là reload trang thông thường nên localStorage đủ để giữ trạng thái.

## 6. Cách sidebar hoạt động hiện tại

### 6.1 Desktop

Ở desktop từ 992px trở lên:

- Sidebar luôn hiển thị, sticky, cao 100vh, rộng 272px.
- Nút hamburger bị ẩn.
- Nội dung chính nằm bên phải trong flex item `.app-main`.
- Không có trạng thái thu gọn.

### 6.2 Tablet

Dưới 992px:

- Sidebar chuyển thành fixed overlay.
- Mặc định bị đẩy ra ngoài bằng `transform: translateX(-106%)`.
- Nút hamburger trong topbar hiện ra.
- Backdrop và khóa cuộn body hoạt động khi `body.sidebar-open`.

### 6.3 Mobile

Mobile dùng cùng cơ chế tablet:

- Sidebar không chiếm chiều ngang content khi đóng.
- Mở bằng nút hamburger.
- Backdrop đóng menu.
- Body bị khóa cuộn khi menu mở.

Chưa thấy xử lý focus trap, chưa thấy tự đóng khi chọn menu, và chưa thấy cập nhật `aria-expanded` cho nút hamburger khi mở/đóng.

## 7. Ảnh hưởng đến vùng nội dung chính

Vùng nội dung chính là `.app-main` và `.app-content`. Vì `.app-main` là flex item `flex: 1`, content có thể mở rộng khi sidebar giảm width. Không thấy content dùng `margin-left` cố định theo sidebar.

Các trang dữ liệu nhiều cột phần lớn đã có wrapper cuộn ngang hoặc min-width:

- Công việc: `_Table.cshtml` dùng `.workflow-table-wrapper table-responsive-soft`, CSS module đặt `overflow-x: auto`.
- Tiến độ: `_Table.cshtml` dùng `.table-responsive tien-do-table-scroll`, CSS có `max-width: 100%`, `overflow-x: auto`.
- Dự án: CSS `DuAn/index.css` đặt vùng bảng `overflow-x: auto`, bảng min-width 1120px.
- Đề xuất công việc/ngân sách: shared proposal đặt `.proposal-table { min-width: 980px; }`.
- Duyệt đề xuất: shared approval đặt `.approval-table { min-width: 920px; }`.
- Đánh giá: shared evaluation đặt `.evaluation-table { min-width: 980px; }`.
- Nhân viên dự án: CSS `ThanhVienDuAn/index.css` đặt `.thanh-vien-table { min-width: 980px; }`.
- Chat dự án: là layout grid riêng `340px minmax(0, 1fr)` trong content, có breakpoint về 1 cột.

Không thấy từng View dùng Bootstrap `.container` hoặc `.container-fluid` làm giới hạn chính. Các trang thường dùng `.app-page`, `.app-section-card`; các class này không đặt max-width cố định. Vì vậy có thể mở rộng nội dung chủ yếu bằng thay đổi layout chung.

## 8. Đánh giá các phương án thu gọn

### 8.1 Click để thu gọn/mở rộng

Phù hợp với cấu trúc hiện tại. Sidebar là flex item có width cố định, content là flex item `flex: 1`; chỉ cần đổi width sidebar bằng class collapsed thì content mở rộng tự nhiên. Không cần reload trang.

Ảnh hưởng menu con thấp vì sidebar chính không có submenu/dropdown. Menu theo nhóm vẫn ổn nếu ẩn group heading khi collapsed. Rất phù hợp cho bảng nhiều cột vì giải phóng khoảng 190-200px nếu sidebar từ 272px còn khoảng 72-84px.

Có thể lưu trạng thái bằng `localStorage`. Không cần sửa Controller, Service, ViewModel, Entity, DbContext, migration hoặc permission.

### 8.2 Hover để tự mở rộng

Không nên dùng làm cơ chế chính. Nếu hover làm sidebar đổi width trong flex layout, `.app-main` sẽ co giãn liên tục, các bảng nhiều cột dễ bị cảm giác nhảy layout. Người dùng có thể vô tình rê chuột qua cạnh trái và làm content thay đổi.

Thiết bị cảm ứng không có hover ổn định, nên cơ chế này không phù hợp tablet/mobile. Khi người dùng đang bấm menu hoặc cần đọc tooltip, tự mở/đóng cũng có thể gây khó chịu.

### 8.3 Click kết hợp tooltip

Đây là phương án phù hợp nhất với hệ thống hiện tại:

- Click quyết định trạng thái layout, nên giao diện ổn định.
- Khi collapsed chỉ còn icon, hover/focus icon hiện tooltip hoặc có `title`/ARIA để biết tên chức năng.
- Content không nhảy theo hover.
- Mobile vẫn giữ cơ chế overlay hiện tại.
- Không cần backend.

Điểm cần chú ý là tooltip phải hỗ trợ bàn phím/screen reader, không chỉ dựa vào hover chuột.

### 8.4 Hover mở tạm dạng overlay

Có thể triển khai mà không làm content nhảy nếu sidebar collapsed vẫn giữ width nhỏ và phần mở rộng hover dùng overlay nổi đè lên content. Tuy nhiên cách này phức tạp hơn CSS/JS:

- Cần xử lý z-index, shadow, hit area, pointer events.
- Cần xử lý khi hover vào/ra, focus bàn phím, mobile/touch.
- Dễ xung đột với active indicator và backdrop mobile.

Với dự án hiện tại, không cần thiết ở giai đoạn đầu. Có thể cân nhắc sau nếu người dùng thấy tooltip chưa đủ.

| Tiêu chí | Click thu gọn | Hover tự mở | Click + tooltip | Hover overlay |
| --- | --- | --- | --- | --- |
| Dễ sử dụng | Tốt, rõ trạng thái | Trung bình, dễ vô tình kích hoạt | Tốt nhất, rõ và dễ học | Trung bình, cần tinh chỉnh |
| Ổn định giao diện | Tốt | Kém nếu đổi width thật | Tốt nhất | Tốt nếu overlay đúng |
| Phù hợp bảng nhiều cột | Tốt | Kém do co giãn liên tục | Tốt nhất | Tốt |
| Phù hợp mobile | Cần tách mobile overlay | Không phù hợp | Phù hợp nếu chỉ áp dụng desktop | Không nên áp dụng mobile |
| Khả năng truy cập | Tốt nếu dùng button/ARIA | Kém hơn vì hover | Tốt nếu có tooltip/focus/ARIA | Khó hơn |
| Độ phức tạp triển khai | Thấp đến vừa | Vừa | Vừa | Cao |
| Nguy cơ lỗi | Thấp | Cao | Thấp đến vừa | Vừa đến cao |

## 9. Phương án phù hợp nhất

Phương án phù hợp nhất là Phương án C: click để thu gọn/mở rộng, khi thu gọn chỉ giữ icon và hover/focus chỉ hiện tooltip, sidebar không tự mở rộng.

Kết luận này dựa trên source hiện tại:

- Sidebar desktop là flex item cố định width 272px.
- Content là flex item `flex: 1`, có thể tự mở rộng.
- Menu không có submenu nhiều cấp.
- Icon và text đã tách class riêng.
- Mobile đã có overlay/backdrop riêng, nên không nên dùng chung class collapsed desktop cho mobile.
- Các bảng lớn đã có wrapper cuộn ngang, nên tăng chiều rộng content là lợi ích trực tiếp.

## 10. Thiết kế đề xuất

### 10.1 Trạng thái mở rộng

Desktop mặc định nên giữ sidebar mở rộng:

- Width 272px như hiện tại.
- Hiển thị logo icon, tên hệ thống, subtitle.
- Hiển thị tiêu đề nhóm và tên menu.
- Active state giữ gradient và thanh chỉ báo.

### 10.2 Trạng thái thu gọn

Desktop collapsed nên:

- Sidebar còn khoảng 72-84px.
- Ẩn `.brand-title`, `.brand-subtitle`, `.sidebar-section-title`, `.sidebar-menu-text`.
- Căn giữa `.brand-logo` và `.sidebar-menu-icon`.
- Giữ active background hoặc active indicator để người dùng biết đang ở menu nào.
- Không để chữ menu chiếm chỗ bằng opacity-only nếu vẫn giữ width; nên dùng width/visibility/display phù hợp.

### 10.3 Hành vi desktop

Nên thêm một nút desktop trong layout, dạng `<button>`, đặt gần brand hoặc topbar. Nút đổi trạng thái collapsed trên `body` hoặc `.app-shell`, ví dụ class `sidebar-collapsed`.

Không nên reload trang. Khi bấm nút:

```text
Mở rộng -> click -> thu gọn -> content mở rộng
Thu gọn -> click -> mở rộng -> content trở lại
```

### 10.4 Hành vi mobile

Mobile nên giữ cơ chế hiện tại:

- Sidebar đóng là ẩn hoàn toàn, không phải icon-only.
- Mở bằng hamburger.
- Sidebar dạng overlay.
- Bấm backdrop hoặc Escape đóng.
- Có thể bổ sung đóng khi chọn menu.

Class collapsed desktop không nên áp dụng dưới 992px để tránh xung đột với `body.sidebar-open`.

### 10.5 Tooltip

Khi collapsed, mỗi link nên có tên menu qua:

- `title` đơn giản, hoặc
- Bootstrap tooltip nếu muốn đẹp hơn, hoặc
- CSS tooltip custom.

Bootstrap JS đã có trong layout, nhưng hiện chưa thấy khởi tạo tooltip chung. Nếu dùng Bootstrap tooltip cần thêm logic init/dispose. Cách ít rủi ro là dùng `title` kết hợp text ẩn cho screen reader, sau đó nâng cấp tooltip sau.

### 10.6 Lưu trạng thái localStorage

Đề xuất:

- Dùng `localStorage`.
- Key: `QuanLyDuAnAI.sidebar.collapsed`.
- Giá trị: `"true"` hoặc `"false"`.
- Mặc định lần đầu: mở rộng.
- Chỉ đọc/ghi trạng thái desktop từ 992px trở lên.
- Khi resize xuống mobile, không xóa localStorage; chỉ không áp dụng class collapsed vào mobile.

Để tránh nhấp nháy, nên áp dụng class càng sớm càng tốt trước khi paint chính. Nếu đặt script cuối body, trang có thể render mở rộng rồi mới thu gọn gây giật.

### 10.7 Khả năng truy cập

Nút toggle nên:

- Là `<button type="button">`.
- Có `aria-label`, ví dụ `Thu gọn menu` hoặc `Mở rộng menu`.
- Có `aria-expanded="true/false"` phản ánh trạng thái mở rộng.
- Hỗ trợ Enter/Space tự nhiên của button.
- Có focus state nhìn thấy.
- Không chỉ dùng màu để báo active; nên giữ nền, thanh chỉ báo hoặc shape rõ.

Tooltip/label khi collapsed nên hỗ trợ cả hover và focus, hoặc có text cho screen reader.

## 11. Danh sách file dự kiến cần sửa

Nếu triển khai sau bước phân tích, danh sách sửa dự kiến tối thiểu:

1. `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml`
   - Thành phần cần sửa: HTML sidebar/topbar và script sidebar hiện tại.
   - Thay đổi dự kiến: thêm nút desktop collapse, thêm title/ARIA cho menu item, thêm class/attribute phục vụ collapsed, cập nhật JS đọc/ghi localStorage và `aria-expanded`.
   - Ảnh hưởng toàn hệ thống: có, vì là layout chung sau đăng nhập.
   - Ảnh hưởng mobile: có thể, nhưng phải giữ cơ chế mobile hiện tại và tách điều kiện breakpoint.
   - Ảnh hưởng quyền: không.
   - Ảnh hưởng workflow: không.

2. `QuanLyDuAn/QuanLyDuAn/wwwroot/css/layout/sidebar.css`
   - Thành phần cần sửa: rule `.app-sidebar`, `.brand-wrap`, `.sidebar-section-title`, `.sidebar-menu-link`, `.sidebar-menu-icon`, `.sidebar-menu-text`, responsive breakpoint.
   - Thay đổi dự kiến: thêm trạng thái desktop collapsed, width nhỏ, ẩn chữ, căn icon, transition, tooltip/focus style nếu dùng CSS tooltip.
   - Ảnh hưởng toàn hệ thống: có trong layout sau đăng nhập.
   - Ảnh hưởng mobile: có nếu không giới hạn bằng `@media (min-width: 992px)`, nên phải giới hạn rõ.
   - Ảnh hưởng quyền: không.
   - Ảnh hưởng workflow: không.

3. `QuanLyDuAn/QuanLyDuAn/wwwroot/css/site.css`
   - Thành phần cần sửa: có thể là biến CSS hoặc rule shell chung nếu cần.
   - Thay đổi dự kiến: chỉ nên sửa nếu cần đặt `--sidebar-width`/`--sidebar-collapsed-width` toàn cục hoặc loại bỏ trùng lặp width 260px cũ.
   - Ảnh hưởng toàn hệ thống: có.
   - Ảnh hưởng mobile: có thể nếu sửa breakpoint cũ.
   - Ảnh hưởng quyền/workflow: không.
   - Ghi chú: có thể tránh sửa file này nếu toàn bộ logic đặt trong `layout/sidebar.css`.

4. `QuanLyDuAn/QuanLyDuAn/wwwroot/js/site.js` hoặc file JS layout mới như `QuanLyDuAn/QuanLyDuAn/wwwroot/js/layout/sidebar.js`
   - Thành phần cần sửa: logic trạng thái sidebar.
   - Thay đổi dự kiến: đọc/ghi localStorage, set class collapsed, cập nhật ARIA, xử lý breakpoint, tránh gắn event nhiều lần.
   - Ảnh hưởng toàn hệ thống: có nếu dùng `site.js`; thấp hơn nếu tạo file riêng và nạp trong layout.
   - Ảnh hưởng mobile: có, vì cần phối hợp với `sidebar-open`.
   - Ảnh hưởng quyền/workflow: không.

## 12. Các file không cần sửa

Không cần sửa:

- Controller: sidebar là vấn đề layout phía client, không đổi route/action.
- Service: quyền menu đã có từ `IPhanQuyenService`, không đổi logic phân quyền.
- ViewModel: không cần dữ liệu mới từ backend.
- Entity: không lưu trạng thái vào database.
- DbContext: không thêm bảng/cột.
- Migration: không có thay đổi schema.
- Permission: không thêm/sửa tên quyền.
- Từng View chức năng: content nằm trong `.app-main` và hưởng lợi từ layout chung; bảng đã có wrapper responsive.
- CSS module của Công việc, Tiến độ, Dự án, Đề xuất, Duyệt, Đánh giá, Chat: chưa cần sửa ở bước đầu, chỉ kiểm tra sau triển khai.
- Trang Account dùng `Layout = null`: không liên quan sidebar sau đăng nhập.

## 13. Rủi ro triển khai

Các rủi ro cần kiểm soát:

- Sidebar thu gọn nhưng content không mở rộng: thấp, vì layout dùng flex; vẫn cần kiểm tra do width có thể bị ghi đè ở `site.css` và `sidebar.css`.
- Sidebar thu gọn nhưng chữ menu vẫn chiếm chỗ: cần ẩn `.sidebar-menu-text` đúng cách.
- Logo bị vỡ: cần ẩn text brand và căn `.brand-logo`.
- Icon không căn giữa: cần chỉnh `.sidebar-menu-link` và `.sidebar-menu-icon` trong collapsed.
- Tooltip không hoạt động: nếu dùng Bootstrap tooltip phải init JS; nếu dùng `title` thì ít rủi ro hơn.
- Active state bị mất: class active là server-side, nhưng CSS collapsed phải giữ nền/indicator.
- Submenu không thể mở khi collapsed: hiện sidebar chính không có submenu, rủi ro thấp.
- Group heading vẫn chiếm diện tích: cần ẩn `.sidebar-section-title`.
- Hover làm content nhảy qua lại: tránh dùng hover tự mở làm cơ chế chính.
- Sidebar che nội dung: mobile overlay đã che có chủ đích; desktop collapsed không nên dùng overlay mặc định.
- Xuất hiện thanh cuộn ngang: cần kiểm tra `.app-shell`, `.app-main`, `.app-content`, các bảng min-width.
- Bảng bị vỡ responsive: cần kiểm tra các màn hình bảng nhiều cột.
- Mobile và desktop dùng chung class gây xung đột: giới hạn collapsed trong `@media (min-width: 992px)`.
- Trạng thái localStorage không đồng bộ với breakpoint mobile: không áp dụng collapsed dưới 992px.
- Sidebar nhấp nháy khi tải trang: cần apply class sớm.
- Event JavaScript bị gắn nhiều lần: layout reload bình thường, nhưng vẫn nên bọc init idempotent nếu đưa vào file JS chung.
- Nút toggle không hỗ trợ bàn phím: dùng `<button>`.
- Thiếu `aria-expanded`, `aria-label`: cần cập nhật trong JS.
- Người dùng bàn phím không biết tên icon khi collapsed: cần label/tooltip/focus text phù hợp.
- CSS dùng `!important` quá nhiều: không nên dùng, vì hiện CSS có thứ tự nạp rõ.
- Sửa CSS dùng chung ảnh hưởng trang đăng nhập: trang Account dùng `Layout = null`, nhưng `site.css` có style auth; nên ưu tiên sửa `layout/sidebar.css` hơn `site.css`.

## 14. Kế hoạch kiểm tra sau triển khai

Desktop mở rộng:

- Sidebar hiển thị icon và chữ.
- Nội dung chính đúng vị trí.
- Menu active đúng.
- Logo hiển thị đủ.
- Không làm thay đổi quyền hiển thị menu.

Desktop thu gọn:

- Chỉ còn icon.
- Chữ và tiêu đề nhóm được ẩn.
- Icon căn giữa.
- Tooltip/title đúng.
- Nội dung chính mở rộng.
- Không có thanh cuộn ngang toàn trang.
- Menu active vẫn rõ bằng nền/thanh chỉ báo.

Chuyển trang:

- Trạng thái sidebar được giữ.
- Không nhấp nháy mở rồi đóng.
- Menu active cập nhật đúng theo controller/action mới.

Refresh trình duyệt:

- Trạng thái localStorage được khôi phục.
- Mặc định lần đầu là mở rộng.

Mobile:

- Sidebar không giữ dạng icon nhỏ.
- Sidebar ẩn hoàn toàn khi đóng.
- Nút hamburger mở overlay.
- Backdrop đóng sidebar.
- Chọn menu xong sidebar đóng nếu có bổ sung logic này.
- Nội dung không bị che sau khi đóng.
- Body không bị khóa cuộn sau khi đóng.

Accessibility:

- Nút toggle dùng bàn phím được.
- `aria-expanded` thay đổi đúng.
- Có `aria-label`.
- Focus state nhìn thấy.
- Icon-only vẫn có nhãn cho screen reader.

Giao diện cần kiểm tra:

- Công việc.
- Chi tiết công việc.
- Tiến độ.
- Dự án.
- Nhân sự.
- Đề xuất công việc.
- Đề xuất ngân sách.
- Đánh giá dự án.
- Đánh giá nhân viên.
- Chat dự án.

## 15. Kết luận

1. Layout chính đang nằm ở `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml`.
2. Sidebar nằm trực tiếp trong layout, không tách partial riêng.
3. Sidebar/content hiện dùng Flexbox qua `.app-shell`; không dùng margin-left cố định cho content.
4. Chiều rộng sidebar desktop thực tế là 272px do `wwwroot/css/layout/sidebar.css` ghi đè `site.css`.
5. Nội dung chính có thể tự mở rộng khi sidebar thu gọn vì `.app-main` là `flex: 1` và `min-width: 0`.
6. Hiện chưa có JavaScript toggle sidebar desktop collapsed.
7. Hiện đã có cơ chế mobile menu bằng hamburger, class `body.sidebar-open`, fixed sidebar overlay và backdrop.
8. Click phù hợp hơn hover cho cơ chế chính.
9. Không nên dùng hover tự mở làm cơ chế chính vì dễ làm layout co giãn và không phù hợp thiết bị cảm ứng.
10. Nên dùng tooltip/title khi sidebar thu gọn để icon-only vẫn hiểu được.
11. Có thể lưu trạng thái bằng localStorage, không cần backend.
12. Không cần sửa backend.
13. Không cần sửa database hoặc tạo migration.
14. Không ảnh hưởng permission hoặc workflow nếu chỉ thay đổi layout/CSS/JS.
15. File dự kiến cần sửa khi triển khai: `_Layout.cshtml`, `wwwroot/css/layout/sidebar.css`, có thể `wwwroot/js/site.js` hoặc JS layout mới, và chỉ khi cần mới sửa `wwwroot/css/site.css`.
16. Rủi ro lớn nhất là xung đột giữa trạng thái collapsed desktop và cơ chế mobile overlay, kèm hiện tượng nhấp nháy khi khôi phục localStorage sau khi trang đã render.

## 16. Kết quả triển khai chức năng thu gọn sidebar

Ngày triển khai: 2026-06-19.

File thực tế đã sửa:

- `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/layout/sidebar.css`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/layout/sidebar.js`.
- `docs/layoutmenu.md`.

Không sửa `wwwroot/css/site.css` vì toàn bộ thay đổi chiều rộng, căn icon, tooltip và responsive có thể đặt trong `wwwroot/css/layout/sidebar.css`, là file đang ghi đè style sidebar thực tế.

Nút toggle desktop được đặt trong khu vực brand/logo của sidebar, cạnh link logo `QuanLyDuAn AI`. Nút là `<button type="button">`, dùng `data-sidebar-collapse-toggle`, `aria-controls="appSidebar"`, `aria-expanded`, `aria-label`, `title` và icon Bootstrap Icons. Nút chỉ hiển thị ở desktop từ 992px trở lên; mobile tiếp tục dùng nút hamburger `[data-sidebar-toggle]` trong topbar.

Class trạng thái collapsed là `sidebar-collapsed`, đặt trên `body`. Chiều rộng mở rộng giữ nguyên `272px`. Chiều rộng thu gọn desktop là `76px`, nằm trong khoảng yêu cầu 72px đến 84px.

Khi mở rộng, sidebar hiển thị đầy đủ logo, tên hệ thống, subtitle, tiêu đề nhóm và tên menu. Khi thu gọn, CSS trong `@media (min-width: 992px)` ẩn `.brand-text`, `.brand-title`, `.brand-subtitle`, `.sidebar-section-title` và `.sidebar-menu-text` bằng `display: none`; logo, nút toggle và icon menu được căn giữa. Menu link vẫn giữ vùng click tối thiểu, active state còn nền/gradient và thanh chỉ báo `::before` được chỉnh lại vị trí để không lệch khỏi sidebar thu gọn.

Trạng thái được lưu bằng `localStorage`, key `QuanLyDuAnAI.sidebar.collapsed`, giá trị `"true"` hoặc `"false"`. Lần đầu chưa có key thì sidebar mở rộng. Khi người dùng bấm thu gọn sẽ lưu `"true"`, khi mở rộng lại sẽ lưu `"false"`. Mobile không xóa trạng thái desktop; khi quay lại desktop, JavaScript đọc lại localStorage và khôi phục trạng thái.

Để tránh nhấp nháy khi tải trang, `_Layout.cshtml` có một script ngắn ngay sau thẻ `<body>`: trong `try/catch`, nếu viewport đang là desktop và localStorage đang là `"true"` thì thêm `body.sidebar-collapsed` trước khi render `.app-shell`. Logic sự kiện đầy đủ không đặt trong script sớm mà tách sang `wwwroot/js/layout/sidebar.js`.

Desktop từ 992px trở lên dùng cơ chế click để thu gọn/mở rộng, không có hover tự bung sidebar. JavaScript cập nhật class `sidebar-collapsed`, icon, `aria-label`, `aria-expanded`, `title`, tooltip label và localStorage. `.app-main` vẫn là flex item `flex: 1; min-width: 0`, không thêm `margin-left` hoặc width cố định, nên nội dung chính tự mở rộng theo chiều rộng sidebar mới.

Tablet/mobile dưới 992px giữ nguyên cơ chế overlay: sidebar ẩn bằng transform khi đóng, mở bằng nút hamburger `[data-sidebar-toggle]`, có backdrop `[data-sidebar-dismiss]`, dùng `body.sidebar-open`, bấm backdrop hoặc Escape để đóng. Khi resize lên desktop, overlay mobile được đóng. Khi xuống mobile, `sidebar-collapsed` bị gỡ khỏi body để không xuất hiện sidebar icon-only trên mobile, nhưng localStorage desktop vẫn được giữ.

Tooltip/title: mỗi `.sidebar-menu-link` được JavaScript lấy tên từ `.sidebar-menu-text`, gắn `data-sidebar-label` và `aria-label`. Khi collapsed, link có thêm `title`; JavaScript dùng một tooltip DOM duy nhất `.sidebar-floating-tooltip` gắn vào `body`, hiển thị theo vị trí fixed khi hover hoặc focus vào menu icon. Không dùng Bootstrap Tooltip, nên không có rủi ro tạo nhiều tooltip instance khi bấm toggle nhiều lần.

ARIA: nút desktop toggle cập nhật `aria-expanded="true"` khi sidebar đang mở rộng và `aria-expanded="false"` khi sidebar đang thu gọn; `aria-label` đổi giữa `Thu gọn menu` và `Mở rộng menu`. Nút hamburger mobile được gắn `aria-controls="appSidebar"` và cập nhật `aria-expanded` theo trạng thái `sidebar-open`.

Kết quả build: đã chạy `dotnet build QuanLyDuAn/QuanLyDuAn.sln` thành công sau khi restore NuGet ngoài sandbox. Lần build đầy đủ có 2 warning `CS1998` ở `Services/Implementations/FileTienDoCongViecService.cs`, không liên quan thay đổi sidebar. Sau chỉnh sửa cuối, đã chạy lại `dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore` thành công với 0 warning, 0 error.

Kết quả kiểm tra JavaScript: đã chạy `node --check QuanLyDuAn/QuanLyDuAn/wwwroot/js/layout/sidebar.js` thành công, không có lỗi cú pháp.

Kết quả kiểm tra trình duyệt: chưa kiểm tra được runtime UI trong trình duyệt. Lần chạy trong sandbox bị lỗi khởi động do quyền ghi Windows EventLog khi app gặp lỗi kết nối SQL Server `LAPTOP-SI5JBDIU\SQLEXPRESS01`; yêu cầu chạy runtime ngoài sandbox đã không được cấp quyền, nên không xác nhận bằng trình duyệt các màn Dashboard, Công việc, Chi tiết công việc, Tiến độ, Dự án, Nhân sự, Đề xuất, Đánh giá và Chat dự án. Không tuyên bố đã kiểm tra console trình duyệt hoặc localStorage runtime.

Kết quả kiểm tra tiếng Việt: các chuỗi mới dùng trong source là `Thu gọn menu`, `Mở rộng menu`, `Đóng menu` và các nhãn menu hiện có vẫn được giữ trong file UTF-8. Đã kiểm tra UTF-8 ở mức file sau khi sửa và không phát hiện mojibake trong các chuỗi tiếng Việt trọng yếu.

Vấn đề còn tồn tại:

- Chưa kiểm tra runtime bằng trình duyệt do app không khởi động được trong sandbox và quyền chạy ngoài sandbox không được cấp.
- Cần kiểm tra thủ công sau khi app chạy được với SQL Server local: toggle desktop, tooltip hover/focus, localStorage, không nhấp nháy khi refresh, mobile overlay, backdrop, Escape, resize và các bảng nhiều cột.

## 17. Chuyển nút thu gọn xuống footer sidebar

Ngày cập nhật: 2026-06-19.

Vị trí cũ: nút `data-sidebar-collapse-toggle` nằm trong vùng brand, cạnh link logo `QuanLyDuAn AI`, tạo bố cục dạng `[Logo QuanLyDuAn AI] [Nút thu gọn]`. Vị trí này chưa phù hợp vì làm brand bị chia thành hai cột, logo/tên hệ thống bị co hẹp và cảm giác đầu sidebar mất cân đối.

Vị trí mới: nút được di chuyển xuống `.sidebar-footer`, nằm sau `<nav class="side-nav sidebar-menu">` và trước `</aside>`. Không tạo nút mới; selector JavaScript `[data-sidebar-collapse-toggle]` vẫn trỏ tới chính nút cũ sau khi di chuyển.

Cấu trúc sidebar hiện là ba vùng:

1. `.sidebar-brand-area`: vùng logo và tên hệ thống, cố định ở trên.
2. `.side-nav.sidebar-menu`: vùng danh sách menu theo quyền, là vùng duy nhất cuộn dọc.
3. `.sidebar-footer`: vùng footer cố định ở dưới, chứa nút thu gọn/mở rộng.

File đã sửa:

- `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/layout/sidebar.css`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/layout/sidebar.js`.
- `docs/layoutmenu.md`.

Class brand là `.sidebar-brand-area`. Class menu cuộn là `.sidebar-menu` kết hợp `.side-nav`. Class footer là `.sidebar-footer`.

Footer cố định bằng Flexbox: `.app-sidebar` dùng `display: flex`, `flex-direction: column`, `height: 100vh`, `overflow: hidden`. `.sidebar-brand-area` và `.sidebar-footer` dùng `flex-shrink: 0`; `.sidebar-menu` dùng `flex: 1 1 auto`, `min-height: 0`, `overflow-y: auto`, `overflow-x: hidden`. Vì vậy brand và footer đứng yên, chỉ menu cuộn khi danh sách dài, không cần `position: fixed` hoặc `position: absolute`.

Khi sidebar mở rộng, nút footer hiển thị dạng icon và chữ `Thu gọn menu`, chiếm toàn bộ chiều rộng footer, có style nhẹ tương đồng menu item phụ. Icon dùng `bi-chevron-double-left`; `aria-label="Thu gọn menu"` và `aria-expanded="true"`.

Khi sidebar thu gọn, sidebar vẫn giữ chiều rộng `76px`; footer vẫn ở cuối sidebar; nút chỉ còn icon căn giữa, text `.sidebar-collapse-text` bị ẩn bằng `display: none`. Icon đổi sang `bi-chevron-double-right`; `aria-label="Mở rộng menu"`, `aria-expanded="false"` và `title="Mở rộng menu"`.

JavaScript có sửa nhẹ nhưng không đổi cơ chế: giữ nguyên class `sidebar-collapsed`, key localStorage `QuanLyDuAnAI.sidebar.collapsed`, logic tránh nhấp nháy, breakpoint desktop/mobile, tooltip menu và selector `[data-sidebar-collapse-toggle]`. Chỉ đổi icon trái/phải và cập nhật thêm text trong `.sidebar-collapse-text` theo trạng thái.

Kết quả build: `dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore` thành công. Build còn 2 warning `CS1998` ở `Services/Implementations/FileTienDoCongViecService.cs`, không liên quan layout/sidebar.

Kết quả kiểm tra JavaScript: `node --check QuanLyDuAn/QuanLyDuAn/wwwroot/js/layout/sidebar.js` thành công.

Kết quả runtime desktop: đã chạy ứng dụng trên `http://127.0.0.1:5088`, đăng nhập bằng tài khoản seed mặc định `admin`. Ở desktop mở rộng, sidebar rộng `272px`, `#appSidebar` có `display: flex`, `flex-direction: column`, `overflow-y: hidden`; nút nằm trong `.sidebar-footer`, không còn nằm trong `.sidebar-brand-area`; nút hiển thị `Thu gọn menu`, icon trái, `aria-expanded="true"`. Ở desktop thu gọn, sidebar rộng `76px`, `body` có `sidebar-collapsed`, text brand/menu/footer đều ẩn, icon nút đổi sang phải, `aria-label="Mở rộng menu"`, `aria-expanded="false"`. Sau khi reload, trạng thái collapsed vẫn được khôi phục ở `76px`.

Kết quả menu dài: với tài khoản admin, `.sidebar-menu` có `overflow-y: auto`, `flex: 1 1 auto` và `scrollHeight >= clientHeight`; footer vẫn cách đáy sidebar khoảng 16-18px theo padding, không cuộn theo menu và không che menu cuối.

Kết quả mobile: ở viewport 390px, `.sidebar-footer` và nút collapse desktop đều `display: none`, hamburger mobile hiển thị, sidebar là `position: fixed` overlay và mặc định transform ra ngoài màn hình. Bấm hamburger mở sidebar, `body.sidebar-open` được thêm, `aria-expanded="true"` và body `overflow: hidden`. Bấm backdrop hoặc nhấn Escape đều đóng sidebar, `aria-expanded="false"` và body trở lại `overflow: visible`. Console trình duyệt không ghi nhận lỗi JavaScript trong các kiểm tra này.

Kết quả font tiếng Việt: các file đã sửa được kiểm tra strict UTF-8 bằng byte-level decode. Các chuỗi `Thu gọn menu`, `Mở rộng menu`, `Hệ thống`, `Dự án`, `Công việc` không bị mojibake.
