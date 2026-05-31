# Quy Chuẩn Giao Diện Hiện Tại (ASP.NET MVC)

## 1. Phạm vi áp dụng
Tài liệu này là chuẩn giao diện hiện hành của dự án, dùng làm mốc để chỉnh sửa các lần sau.

Nguyên tắc bắt buộc:
- Không đổi controller, action, route, model, viewmodel, logic nghiệp vụ.
- Chỉ chỉnh View/CSS khi đồng bộ giao diện.
- Giữ tiếng Việt có dấu và UTF-8 cho file `.cshtml`, `.css`, `.js`, `.cs`, `.md`.

## 2. Màn hình chuẩn tham chiếu
- Chuẩn bố cục bộ lọc: `Views/TienDoCongViec/_Filter.cshtml` + `wwwroot/css/TienDoCongViec/index.css`.
- Chuẩn nút thao tác trong bảng: `Views/CongViec/_Table.cshtml` + `wwwroot/css/CongViec/index.css`.

## 3. Chuẩn bộ lọc
- Bố cục filter theo 2 hàng: điều kiện chính + nhóm thời gian/nút thao tác.
- Nút `Lọc dữ liệu`, `Làm mới` canh theo cụm hành động, không lệch hàng.
- Input/select/date/button đồng nhất chiều cao, bo góc, viền, padding.

## 4. Chuẩn card và spacing
- Card nền sáng, viền mảnh, bo góc lớn, shadow nhẹ.
- Khoảng cách giữa filter, card, table dùng grid nhất quán.
- Không trộn quá nhiều style card khác nhau trong cùng một màn hình nếu không có lý do nghiệp vụ.

## 5. Chuẩn table danh sách (quy chuẩn mới)
### 5.1 Nguyên tắc hiển thị dữ liệu
- Ưu tiên không gian cho cột nghiệp vụ chính:
  - Tên
  - Trạng thái
  - Tiến độ
  - Ngày tháng
  - Người phụ trách
  - Ngân sách/điểm/mức độ (nếu có)
  - Thao tác
- Hạn chế tối đa thông tin kỹ thuật trong table.

### 5.2 Quy tắc cột mã/ID
- Ẩn khỏi giao diện table tất cả mã nội bộ phục vụ kỹ thuật:
  - `Mã dự án`, `Mã công việc`, `Mã nhân viên`, `Mã tiến độ`, `Mã CTCV`, `Mã team`, `ID` nội bộ, v.v.
- Không xóa dữ liệu mã trong backend:
  - Mã vẫn dùng bình thường cho route, action, submit form, hidden input, JavaScript, Ajax, modal.
- Chỉ giữ mã trên giao diện khi đó là mã nghiệp vụ người dùng phải nhìn thấy (ví dụ mã chứng từ thực tế). Nếu giữ, phải nêu rõ lý do nghiệp vụ trong tài liệu.

### 5.3 Chuẩn cột thao tác
- Dùng hệ `btn-action` và biến thể chuẩn (`btn-subtle`, `btn-confirm`, `btn-reopen`, `btn-danger-soft`).
- Không hiển thị thao tác kiểu link rời rạc.
- Không đổi màu tùy tiện theo từng màn hình.

### 5.4 Kỹ thuật hiển thị table
- Dùng wrapper cuộn ngang khi cần (`table-responsive-soft`, `*table-scroll`).
- Đảm bảo không lệch header/body.
- Không để nút thao tác xuống dòng bất thường do cột quá chật.

## 6. Chuẩn badge/trạng thái
- Badge nền nhạt, chữ đậm, màu nhất quán theo nhóm trạng thái.
- Tránh dùng màu quá gắt hoặc mỗi màn hình một quy ước khác nhau.

## 7. Quy tắc CSS dùng chung và CSS module
- `wwwroot/css/site.css`: giữ rule dùng chung cho layout/table/action cơ bản.
- CSS module: chỉ giữ phần đặc thù nghiệp vụ.
- Không định nghĩa trùng lặp nhiều kiểu cho cùng một loại nút thao tác.

## 8. Quy tắc tiếng Việt và UTF-8
- Không đổi encoding file làm lỗi dấu tiếng Việt.
- Không thay đổi `meta charset="utf-8"` trong layout.
- Không để xuất hiện chuỗi lỗi mã hóa (mojibake) trong giao diện.

## 9. Kết quả rà soát table toàn hệ thống (hiện tại)
### 9.1 Đã đồng bộ ẩn mã kỹ thuật khỏi table
- Dự án
- Công việc
- Chi tiết công việc
- Tiến độ công việc (đã bỏ hiển thị mã CTCV trong dòng table)
- Phân công công việc
- Phân công chi tiết công việc
- Đề xuất công việc
- Duyệt đề xuất công việc
- Đề xuất ngân sách
- Duyệt đề xuất ngân sách
- Ngân sách
- Nhân sự
- Nhóm/Team
- Thành viên dự án
- Team dự án
- Danh mục công việc
- Chức danh
- Đánh giá dự án
- Đánh giá nhân viên
- Yêu cầu đổi quản lý
- Duyệt yêu cầu đổi quản lý

### 9.2 Màn hình vẫn giữ cột mã/ID trong table
- Không có (theo phạm vi table danh sách hiện tại).

### 9.3 Màn hình cần đồng bộ tiếp theo chuẩn “ẩn mã kỹ thuật trong table”
- Không có màn hình nào còn lệch theo tiêu chí này ở các bảng danh sách đã rà soát.

## 10. Quy tắc duy trì cho các lần sửa tiếp theo
- Khi thêm màn hình list mới:
  - Mặc định không hiển thị mã nội bộ trong table.
  - Chỉ hiển thị mã khi có lý do nghiệp vụ rõ ràng.
- Khi review UI:
  - Ưu tiên đọc table theo hướng người dùng nghiệp vụ, không theo hướng cấu trúc database.
