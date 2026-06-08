# Nhật ký sửa đồng bộ giao diện

## 1. Phạm vi đã sửa

* CSS dùng chung: tạo nền `shared/ui.css`, thêm nhóm shared cho proposal/approval và tách CSS dropdown tài khoản.
* Đề xuất công việc/ngân sách: thêm class chung cho page, header, section card, summary, filter, form button, table, action button và badge trạng thái.
* Duyệt đề xuất: thêm class chung cho page, header, section, filter, table, action, badge, detail card; chuyển JS inline confirm/toggle detail sang file riêng.
* Tài khoản cá nhân: tách CSS dropdown topbar khỏi CSS trang hồ sơ cá nhân; CSS hồ sơ chỉ import trong các view tài khoản.

## 2. File đã thay đổi

| STT | File | Loại thay đổi | Ghi chú |
| --- | ---- | ------------- | ------- |
| 1 | `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml` | Import CSS | Thêm `shared/ui.css`, `layout/account-menu.css`; bỏ import toàn cục `TaiKhoanCaNhan/index.css`. |
| 2 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/ui.css` | Tạo mới | CSS chung cho page, card, table wrapper, empty state, button, filter, badge trạng thái. |
| 3 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/proposal.css` | Tạo mới | CSS chung cho nhóm đề xuất công việc/ngân sách. |
| 4 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/approval.css` | Tạo mới | CSS chung cho nhóm duyệt đề xuất. |
| 5 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/layout/account-menu.css` | Tạo mới | CSS riêng cho dropdown tài khoản trên topbar. |
| 6 | `QuanLyDuAn/QuanLyDuAn/wwwroot/js/approval/index.js` | Tạo mới | JS dùng chung cho confirm submit và toggle dòng chi tiết. |
| 7 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/TaiKhoanCaNhan/index.css` | Cập nhật | Bỏ CSS dropdown topbar, giữ CSS trang tài khoản. |
| 8 | `QuanLyDuAn/QuanLyDuAn/Views/TaiKhoanCaNhan/*.cshtml` | Import CSS | Thêm `@section Styles` import CSS trang tài khoản. |
| 9 | `QuanLyDuAn/QuanLyDuAn/Views/DeXuatCongViec/*` | Cập nhật class/import | Thêm `proposal-*`, `app-*`, `btn-action`, `status-badge` cạnh class cũ. |
| 10 | `QuanLyDuAn/QuanLyDuAn/Views/DeXuatNganSach/*` | Cập nhật class/import | Thêm `proposal-*`, `app-*`, `btn-action`, `status-badge` cạnh class cũ. |
| 11 | `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatCongViec/*` | Cập nhật class/import/JS | Thêm `approval-*`, `app-*`, `btn-action`, `status-badge`; import JS riêng. |
| 12 | `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatNganSach/*` | Cập nhật class/import/JS | Thêm `approval-*`, `app-*`, `btn-action`, `status-badge`; import JS riêng. |

## 3. CSS dùng chung đã tạo hoặc cập nhật

* `wwwroot/css/shared/ui.css`: `app-page`, `app-page-header`, `app-page-title`, `app-page-subtitle`, `app-section-card`, `app-table-scroll`, `app-empty-state`, `btn-action`, `btn-subtle`, `btn-confirm`, `btn-danger-soft`, `btn-filter-apply`, `btn-filter-reset`, `status-badge`, `status-pending`, `status-approved`, `status-rejected`, `status-done`, `status-active`, `status-paused`, `status-neutral`.
* `wwwroot/css/shared/proposal.css`: `proposal-page`, `proposal-header-card`, `proposal-section-card`, `proposal-summary-grid`, `proposal-summary-card`, `proposal-action-btn`, `proposal-table`, `proposal-detail-grid`.
* `wwwroot/css/shared/approval.css`: `approval-page`, `approval-header-card`, `approval-section-card`, `approval-section-head`, `approval-table`, `approval-table-actions`, `approval-action-btn`, `approval-detail-card`, `approval-detail-grid`.
* `wwwroot/css/layout/account-menu.css`: `tai-khoan-dropdown`, `tai-khoan-toggle`, `tai-khoan-avatar`, `tai-khoan-name`, `tai-khoan-menu`, `tai-khoan-menu-item`.

## 4. Class đã đổi hoặc gom chung

| Class cũ | Class chung thêm/kế thừa | Module ảnh hưởng |
| --- | --- | --- |
| `de-xuat-cong-viec-page`, `de-xuat-ngan-sach-page` | `app-page`, `proposal-page` | Đề xuất công việc, đề xuất ngân sách |
| `dxcv-header-card`, `dxns-header-card` | `app-page-header`, `proposal-header-card` | Đề xuất công việc, đề xuất ngân sách |
| `dxcv-section-card`, `dxns-section-card` | `app-section-card`, `proposal-section-card` | Đề xuất công việc, đề xuất ngân sách |
| `dxcv-action-btn`, `dxns-action-btn` | `btn-action`, `proposal-action-btn`, `btn-confirm`, `btn-subtle`, `btn-danger-soft` | Đề xuất công việc, đề xuất ngân sách |
| `dxcv-table`, `dxns-table` | `app-data-table`, `proposal-table` | Đề xuất công việc, đề xuất ngân sách |
| `dxcv-status-badge`, `dxns-status-badge` | `status-badge`, `status-*` | Đề xuất công việc, đề xuất ngân sách |
| `duyet-de-xuat-cong-viec-page`, `duyet-de-xuat-ngan-sach-page` | `app-page`, `approval-page` | Duyệt đề xuất công việc, duyệt ngân sách |
| `duyet-de-xuat-*-section-card` | `app-section-card`, `approval-section-card` | Duyệt đề xuất công việc, duyệt ngân sách |
| `duyet-de-xuat-*-action-btn` | `btn-action`, `approval-action-btn`, `btn-confirm`, `btn-subtle`, `btn-danger-soft` | Duyệt đề xuất công việc, duyệt ngân sách |
| `duyet-de-xuat-*-status-badge` | `status-badge` | Duyệt đề xuất công việc, duyệt ngân sách |
| `duyet-de-xuat-*-detail-*` | `approval-detail-*` | Duyệt đề xuất công việc, duyệt ngân sách |

## 5. Inline style/JS inline đã xử lý

* Inline style: chưa phát sinh hoặc xử lý inline style mới trong phạm vi đợt này.
* JS inline đã chuyển: hai đoạn script confirm submit và toggle detail trong `DuyetDeXuatCongViec/Index.cshtml` và `DuyetDeXuatNganSach/Index.cshtml`.
* File JS mới: `QuanLyDuAn/QuanLyDuAn/wwwroot/js/approval/index.js`.
* View đã import script: `Views/DuyetDeXuatCongViec/Index.cshtml`, `Views/DuyetDeXuatNganSach/Index.cshtml`.

## 6. Kiểm tra lỗi font chữ

* Chưa phát hiện lỗi font tiếng Việt trong các text đã sửa.
* Không sửa text nghiệp vụ ngoài việc giữ nguyên nội dung hiển thị hiện có.
* Cần kiểm tra lại bằng trình duyệt các màn đã sửa để xác nhận hiển thị tiếng Việt thực tế không bị ảnh hưởng bởi cache CSS/JS.

## 7. Những phần chưa sửa

* Chưa động tới các module ngoài phạm vi đợt này: `DuAn Details/Form`, đánh giá dự án/nhân viên, phân công, chat, AI, ngân sách chính, yêu cầu đổi quản lý.
* Chưa xóa mạnh CSS cũ trong `DeXuatCongViec`, `DeXuatNganSach`, `DuyetDeXuatCongViec`, `DuyetDeXuatNganSach` vì cần thêm lượt kiểm tra visual trước khi loại bỏ fallback.
* CSS filter/form cũ vẫn được giữ để tránh làm vỡ layout đang ổn định.
* `docs/giaodien.md` không được chỉnh sửa theo yêu cầu.

## 8. Lưu ý kiểm thử

* [x] Build project: `dotnet build QuanLyDuAn/QuanLyDuAn/QuanLyDuAn.csproj --no-restore -p:UseAppHost=false -p:CopyBuildOutputToOutputDirectory=false` thành công, còn 2 warning CS1998 cũ ở `FileTienDoCongViecService.cs`.
* [ ] Mở các màn đề xuất công việc/ngân sách.
* [ ] Mở các màn duyệt đề xuất công việc/ngân sách.
* [ ] Mở hồ sơ cá nhân, cập nhật hồ sơ, đổi mật khẩu.
* [ ] Kiểm tra button thêm/sửa/xóa/hủy/duyệt/từ chối/quay lại.
* [ ] Kiểm tra modal/collapse/detail row.
* [ ] Kiểm tra filter/search.
* [ ] Kiểm tra responsive table.
* [ ] Kiểm tra dropdown tài khoản.
* [x] Kiểm tra tiếng Việt có dấu: rà UTF-8/mojibake theo chuỗi lỗi thường gặp trong các file đã sửa, chưa phát hiện lỗi.
* [ ] Kiểm tra không lỗi console JS.

# Đợt 2 - Nhật ký sửa tiếp đồng bộ giao diện

## 1. Phạm vi đã sửa đợt 2

* Dự án Details/Form: giảm inline display trong `_Form.cshtml`, chuyển progress width trong `Details.cshtml` sang CSS variable, thêm class chung cho page/card/button.
* Đánh giá dự án và đánh giá nhân viên: tạo shared evaluation CSS, chuẩn hóa summary/table/badge trạng thái.
* Phân công công việc và phân công chi tiết công việc: tạo shared assignment CSS, chuẩn hóa page/card/form/table/button.
* Ngân sách chính và yêu cầu đổi quản lý: chuẩn hóa page/card/filter/table/button/badge; màn duyệt yêu cầu dùng lại shared approval CSS/JS.

## 2. File đã thay đổi đợt 2

| STT | File | Loại thay đổi | Ghi chú |
| --- | ---- | ------------- | ------- |
| 1 | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Form.cshtml` | Cập nhật UI | Đổi `display:none` sang `d-none`, `display:inline` sang `d-inline`; JS toggle dùng class thay vì style. |
| 2 | `QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml` | Cập nhật UI | Thêm `app-page`, `app-section-card`, `btn-action`; chuyển progress width sang `--progress-width`; đổi delay inline sang class. |
| 3 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/DuAn/details.css` | Cập nhật CSS | Thêm `progress-*-dynamic` và `reveal-delay-*`. |
| 4 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/evaluation.css` | Tạo mới | CSS chung cho đánh giá dự án/nhân viên. |
| 5 | `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/*` | Cập nhật class/import | Thêm `evaluation-*`, `app-data-table`, `status-badge`. |
| 6 | `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaNhanVien/*` | Cập nhật class/import | Thêm `evaluation-*`, `app-data-table`, `status-badge`. |
| 7 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/assignment.css` | Tạo mới | CSS chung cho phân công công việc/chi tiết công việc. |
| 8 | `QuanLyDuAn/QuanLyDuAn/Views/PhanCongCongViec/*` | Cập nhật class/import | Thêm `assignment-*`, `app-*`, `btn-action`, `app-data-table`. |
| 9 | `QuanLyDuAn/QuanLyDuAn/Views/PhanCongChiTietCongViec/*` | Cập nhật class/import | Thêm `assignment-*`, `app-*`, `btn-action`, `app-data-table`. |
| 10 | `QuanLyDuAn/QuanLyDuAn/Views/NganSach/*` | Cập nhật class | Thêm `app-*`, `btn-action`, `status-badge`, `app-data-table`. |
| 11 | `QuanLyDuAn/QuanLyDuAn/Views/YeuCauDoiQuanLy/*` | Cập nhật class | Thêm `app-*`, `btn-action`, `status-badge`, `app-data-table`. |
| 12 | `QuanLyDuAn/QuanLyDuAn/Views/DuyetYeuCauDoiQuanLy/*` | Cập nhật class/import/JS | Tái dùng `approval.css` và `approval/index.js`; thêm `approval-*`, `btn-action`, `status-badge`, `app-data-table`. |
| 13 | `QuanLyDuAn/QuanLyDuAn/wwwroot/js/approval/index.js` | Cập nhật JS | Cho phép xử lý thêm selector `.js-review-submit`. |

## 3. CSS dùng chung đã tạo hoặc cập nhật

* Tạo `wwwroot/css/shared/evaluation.css`: `evaluation-page`, `evaluation-card`, `evaluation-summary`, `evaluation-summary-item`, `evaluation-table-wrap`, `evaluation-table`, `evaluation-table-actions`, `evaluation-status-badge`, `evaluation-late-badge`.
* Tạo `wwwroot/css/shared/assignment.css`: `assignment-page`, `assignment-header-card`, `assignment-summary-grid`, `assignment-summary-card`, `assignment-form`, `assignment-section-card`, `assignment-table-scroll`, `assignment-table`, `assignment-empty-state`, `assignment-inline-note`, `assignment-action-btn`.
* Cập nhật `wwwroot/css/DuAn/details.css`: `progress-fill-dynamic`, `progress-bar-dynamic`, `reveal-delay-80`, `reveal-delay-120`, `reveal-delay-160`, `reveal-delay-200`, `reveal-delay-220`, `reveal-delay-230`, `reveal-delay-240`.
* Cập nhật `wwwroot/js/approval/index.js`: dùng chung cho `.js-confirm-submit` và `.js-review-submit`.

## 4. Class đã đổi hoặc gom chung

| Class cũ | Class chung thêm/kế thừa | Module ảnh hưởng |
| -------- | ------------------------ | ---------------- |
| `section-card` | `app-section-card`, `reveal-delay-*` | `DuAn/Details` |
| `progress-fill`, `progress-bar` | `progress-fill-dynamic`, `progress-bar-dynamic` | `DuAn/Details` |
| `danh-gia-du-an-page`, `danh-gia-nhan-vien-page` | `app-page`, `evaluation-page` | Đánh giá dự án, đánh giá nhân viên |
| `danh-gia-card`, `dgnv-card` | `app-section-card`, `evaluation-card` | Đánh giá dự án, đánh giá nhân viên |
| `summary-item` | `evaluation-summary-item` | Đánh giá dự án, đánh giá nhân viên |
| `dgda-table`, `dgnv-table` | `app-data-table`, `evaluation-table` | Đánh giá dự án, đánh giá nhân viên |
| `status-da-duyet`, `status-cho-duyet`, `status-tu-choi` | `status-approved`, `status-pending`, `status-rejected` | Đánh giá dự án, đánh giá nhân viên |
| `pc-*`, `pct-*` page/card/table/button | `assignment-*`, `app-*`, `btn-action` | Phân công công việc, phân công chi tiết |
| `ngan-sach-table`, `yeu-cau-table`, `duyet-yeu-cau-table` | `app-data-table` | Ngân sách, yêu cầu đổi quản lý |
| `ngan-sach-status-badge`, `yeu-cau-status-badge`, `duyet-yeu-cau-status-badge` | `status-badge` | Ngân sách, yêu cầu đổi quản lý |
| `duyet-yeu-cau-action-btn` | `btn-action`, `approval-action-btn` | Duyệt yêu cầu đổi quản lý |

## 5. Inline style/JS inline đã xử lý

* `DuAn/_Form.cshtml`: chuyển `style="display:none"` sang `d-none`, các form transition từ `style="display:inline"` sang `d-inline`; JS toggle ghi chú tạm dừng dùng `classList.toggle`.
* `DuAn/Details.cshtml`: chuyển `style="width: ..."` sang `style="--progress-width: ..."` và CSS `width: var(--progress-width)`; chuyển inline `--delay` sang các class `reveal-delay-*`.
* `DuyetYeuCauDoiQuanLy/Index.cshtml` và `Details.cshtml`: bỏ JS inline confirm/disable submit, import `wwwroot/js/approval/index.js`.

## 6. Kiểm tra lỗi font chữ

* Chưa phát hiện lỗi font/mojibake trong các file đã sửa ở đợt 2.
* Không sửa text nghiệp vụ ngoài class/import/UI wrapper.
* Vẫn cần kiểm tra trực tiếp trên trình duyệt để xác nhận không có lỗi do cache CSS/JS.

## 7. Những phần chưa sửa

* Chưa xóa các file CSS nhỏ lẻ của `DanhGiaDuAn`/`DanhGiaNhanVien`; chỉ thêm shared CSS và class chung để tránh mất fallback.
* Chưa xóa CSS module cũ của `PhanCongCongViec`, `PhanCongChiTietCongViec`, `NganSach`, `YeuCauDoiQuanLy`, `DuyetYeuCauDoiQuanLy`.
* Chưa xử lý các module ngoài phạm vi đợt 2 như `Chat`, `AI`, `Dashboard`, `ChiTietCongViec`, `TienDoCongViec`.

## 8. Lưu ý kiểm thử

* [x] Build project thành công sau nhóm `DuAn`.
* [x] Build project thành công sau nhóm đánh giá.
* [x] Build project thành công sau nhóm phân công.
* [x] Build project thành công sau nhóm ngân sách/yêu cầu đổi quản lý.
* [ ] Mở `DuAn/Details` và form dự án.
* [ ] Kiểm tra modal/tác vụ dự án còn hoạt động.
* [ ] Mở đánh giá dự án và đánh giá nhân viên.
* [ ] Kiểm tra form điểm, badge trạng thái, bảng danh sách.
* [ ] Mở phân công công việc và phân công chi tiết.
* [ ] Kiểm tra nút phân công/thu hồi/quay lại.
* [ ] Mở ngân sách và yêu cầu đổi quản lý.
* [ ] Kiểm tra duyệt/từ chối/yêu cầu còn hoạt động.
* [ ] Kiểm tra responsive table.
* [ ] Kiểm tra dropdown tài khoản không bị ảnh hưởng.
* [ ] Kiểm tra console JS không lỗi.
* [x] Kiểm tra tiếng Việt có dấu bằng rà UTF-8/mojibake trên các file đã sửa.

# Đợt 3 - Kiểm tra sau sửa và chuẩn hóa phần còn lại

## 1. Phạm vi đã kiểm tra

* Đã rà tĩnh các nhóm đã sửa ở đợt 1 và đợt 2: `DeXuatCongViec`, `DeXuatNganSach`, `DuyetDeXuatCongViec`, `DuyetDeXuatNganSach`, `TaiKhoanCaNhan`, `DuAn/Details`, `DuAn/_Form`, `DanhGiaDuAn`, `DanhGiaNhanVien`, `PhanCongCongViec`, `PhanCongChiTietCongViec`, `NganSach`, `YeuCauDoiQuanLy`, `DuyetYeuCauDoiQuanLy`.
* Đã rà các module còn lại trong phạm vi đợt 3: `ChiTietCongViec`, `TienDoCongViec`, `Dashboard`, `AI`, `Chat`.
* Đã kiểm tra import `approval/index.js` ở các màn duyệt đang dùng class `js-confirm-submit` hoặc `js-review-submit`; selector trong JS vẫn chỉ tác động các form có class này.
* Đã kiểm tra không còn inline `style="width:"` trong nhóm `ChiTietCongViec`, `TienDoCongViec`, `Dashboard`; các width động được chuyển sang CSS variable.
* Chưa kiểm tra bằng browser/console runtime vì chưa chạy ứng dụng và đăng nhập dữ liệu thực tế trong trình duyệt.

## 2. File đã thay đổi đợt 3

| STT | File | Loại thay đổi | Ghi chú |
| --- | ---- | ------------- | ------- |
| 1 | `QuanLyDuAn/QuanLyDuAn/Views/ChiTietCongViec/Index.cshtml` | Chuẩn hóa nhẹ UI | Thêm `app-page`, `app-page-header`, `app-section-card`, `btn-action`; chuyển progress width sang `--progress-width`. |
| 2 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChiTietCongViec/index.css` | Cập nhật CSS | Sửa selector progress bị lặp scope, thêm `progress-line-fill-dynamic`. |
| 3 | `QuanLyDuAn/QuanLyDuAn/Views/TienDoCongViec/_Table.cshtml` | Chuẩn hóa nhẹ UI | Chuyển progress width sang `--progress-width` với class `td-progress-fill`. |
| 4 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/TienDoCongViec/index.css` | Cập nhật CSS | Thêm `td-progress-fill`. |
| 5 | `QuanLyDuAn/QuanLyDuAn/Views/TienDoCongViec/_History.cshtml` | Sửa font chữ | Sửa các nhãn tiếng Việt bị mojibake rõ nghĩa trong lịch sử báo cáo tiến độ. |
| 6 | `QuanLyDuAn/QuanLyDuAn/Views/TienDoCongViec/_UpdateForm.cshtml` | Sửa font chữ | Sửa các nhãn/placeholder/nút tiếng Việt bị mojibake rõ nghĩa trong form báo cáo tiến độ. |
| 7 | `QuanLyDuAn/QuanLyDuAn/Views/Dashboard/Index.cshtml` | Chuẩn hóa nhẹ UI | Thêm class `app-*`, `btn-action`, `app-data-table`, `app-empty-state`; chuyển progress width sang CSS variable. |
| 8 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/Dashboard/index.css` | Cập nhật CSS | Thêm `dashboard-progress-fill`. |
| 9 | `QuanLyDuAn/QuanLyDuAn/Views/AiDataset/Index.cshtml` | Chuẩn hóa nhẹ UI | Thêm `app-page`, `app-section-card`, `btn-action`, `app-empty-state`. |
| 10 | `QuanLyDuAn/QuanLyDuAn/Views/Ai/Train.cshtml` | Chuẩn hóa nhẹ UI | Thêm class chung cho page/card/nút/empty state, giữ chart và logic train. |
| 11 | `QuanLyDuAn/QuanLyDuAn/Views/Ai/Models.cshtml` | Chuẩn hóa nhẹ UI | Thêm class chung cho page/card/table/nút/empty state, giữ chart/model panel. |
| 12 | `QuanLyDuAn/QuanLyDuAn/Views/Ai/Predict.cshtml` | Chuẩn hóa nhẹ UI | Thêm class chung cho page/card/nút, giữ form dữ liệu và logic predict. |
| 13 | `QuanLyDuAn/QuanLyDuAn/Views/Ai/Dashboard.cshtml` | Chuẩn hóa nhẹ UI | Thêm class chung cho page/card/table/empty state, giữ chart và dữ liệu AI. |
| 14 | `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/Index.cshtml` | Chuẩn hóa nhẹ UI | Thêm `app-page`, `app-section-card` cạnh class chat cũ. |
| 15 | `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_RoomList.cshtml` | Chuẩn hóa nhẹ UI | Thêm `form-control`, `btn-action`, `app-empty-state`; giữ logic chọn phòng/tìm kiếm. |
| 16 | `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_ChatHeader.cshtml` | Chuẩn hóa nhẹ UI | Thêm `btn-action btn-subtle` cho nút chi tiết dự án. |
| 17 | `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_MessageForm.cshtml` | Chuẩn hóa nhẹ UI | Thêm `form-control`, `btn-action btn-confirm`, `app-empty-state`; giữ logic gửi tin nhắn. |
| 18 | `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_EmptyState.cshtml` | Chuẩn hóa nhẹ UI | Thêm `app-empty-state`. |

## 3. CSS đã dọn hoặc giữ lại

* Đã dọn/sửa an toàn: selector `.chi-tiet-dashboard .chi-tiet-dashboard .progress-line-fill` thành `.chi-tiet-dashboard .progress-line-fill`.
* Đã thêm rule width động: `progress-line-fill-dynamic`, `td-progress-fill`, `dashboard-progress-fill`.
* Giữ lại toàn bộ CSS module cũ của `Proposal`, `Approval`, `Evaluation`, `Assignment`, `ChiTietCongViec`, `TienDoCongViec`, `Dashboard`, `AI`, `ChatDuAn` vì còn vai trò fallback và chưa có visual check trình duyệt.
* Không xóa file CSS nhỏ/lẻ của `DanhGiaDuAn`, `DanhGiaNhanVien`, `ChatDuAn` trong đợt này vì chưa đủ bằng chứng visual để gom file an toàn.

## 4. Module còn lại đã sửa nhẹ

* `ChiTietCongViec`: thêm class chung cho page/header/card/button và bỏ inline width trực tiếp của progress.
* `TienDoCongViec`: bỏ inline width trực tiếp của progress, sửa nhãn tiếng Việt bị lỗi mã hóa rõ nghĩa ở `_History` và `_UpdateForm`.
* `Dashboard`: giữ cấu trúc dashboard/chart, chỉ thêm class chung cho wrapper/filter/button/table/empty state và chuyển status progress sang CSS variable.
* `AI`: giữ card/chart/model panel đặc thù, chỉ thêm class chung cho page/card/table/button/empty state.
* `Chat`: giữ UI chat đặc thù, chỉ thêm class chung cho page/panel/input/button/empty state; không gom CSS chat.

## 5. Kiểm tra JS/console

* Đã kiểm tra tĩnh `wwwroot/js/approval/index.js`: selector vẫn giới hạn ở `.js-confirm-submit`, `.js-review-submit`, `.js-toggle-detail`.
* Đã kiểm tra các view duyệt có import `approval/index.js` đúng nơi cần dùng.
* Giữ nguyên các script đặc thù còn lại: chart Dashboard/AI, scroll chat, collapse/form của các module hiện có.
* Chưa kiểm tra browser/console runtime.

## 6. Kiểm tra lỗi font chữ

* Đã phát hiện và sửa lỗi tiếng Việt rõ nghĩa trong `Views/TienDoCongViec/_History.cshtml` và `Views/TienDoCongViec/_UpdateForm.cshtml`.
* Đã kiểm tra targeted mojibake trên các file đã chạm bằng UTF-8 read; không phát hiện chuỗi lỗi mục tiêu sau sửa.
* Các view có nội dung động từ database vẫn cần kiểm tra thủ công trong browser nếu dữ liệu nguồn đã từng bị lưu sai mã hóa.

## 7. Kết quả build

* Sau nhóm `ChiTietCongViec`/`TienDoCongViec`: `dotnet build QuanLyDuAn\QuanLyDuAn\QuanLyDuAn.csproj --no-restore -p:UseAppHost=false -p:CopyBuildOutputToOutputDirectory=false` thành công, còn 2 warning CS1998 cũ ở `FileTienDoCongViecService.cs`.
* Sau nhóm `Dashboard`/`AI`: cùng lệnh build thành công, còn 2 warning CS1998 cũ ở `FileTienDoCongViecService.cs`.
* Sau nhóm `Chat`: cùng lệnh build thành công, còn 2 warning CS1998 cũ ở `FileTienDoCongViecService.cs`.

## 8. Những phần chưa nên sửa tiếp

* Chưa nên xóa CSS module cũ hoặc class cũ vì vẫn cần fallback và cần kiểm tra visual trên browser.
* Chưa nên gom các file CSS `ChatDuAn` vì chat là UI đặc thù và cần kiểm tra layout/tương tác thực tế.
* Chưa nên tách/gom script chart Dashboard/AI trong đợt này vì có dữ liệu Razor serialize trực tiếp và rủi ro ảnh hưởng biểu đồ.
* Chưa nên sửa thêm workflow/action/permission/form field trong `TienDoCongViec`, `AI`, `Chat` vì phạm vi đợt 3 chỉ là UI nhẹ.
