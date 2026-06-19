# Review điều chỉnh thời gian tiến độ

## A. Thống kê trước và sau

- Ngày lớn nhất trước khi sửa: `2026-06-26T00:00:00.0000000`.
- Ngày lớn nhất sau khi sửa: `2026-06-21T23:59:59.0000000`.
- Số mốc thời gian sau 21/06 đã chỉnh: `561`.
- Số báo cáo tiến độ đã chỉnh thời gian gửi: `592`.
- Số thời gian duyệt đã chỉnh: `592`.
- Số file upload đã chỉnh: `590`.
- Số yêu cầu đổi quản lý đã chỉnh: `2 yêu cầu / 4 mốc thời gian`.
- Số nhật ký đã chỉnh: `2`.
- Số ngày tham gia nhân sự đã chỉnh: `0`.
- Số ngày hoàn thành thực tế công việc đã đưa về trước hạn giới hạn 21/06: `16`.

## B. Kiểm tra thứ tự

- Số trường hợp duyệt trước hoặc bằng thời gian báo cáo sau khi sửa: `0`.
- Số chuỗi báo cáo bị đảo thứ tự sau khi sửa: `0`.
- Số báo cáo lại trước thời gian từ chối/yêu cầu bổ sung sau khi sửa: `0`.
- Số file upload sau báo cáo hoặc sau thời gian duyệt sau khi sửa: `0`.
- Số báo cáo thiếu file: `0`.
- Số file mồ côi: `0`.

## C. Xác nhận trạng thái

- 10 dự án vẫn được cập nhật về `DangThucHien` trong file SQL.
- Không thêm cập nhật dự án sang `HoanThanh`.
- Không thay đổi `PhanTram`, `TrangThaiTienDo`, `TrangThaiCTCVDeXuat`, `TrangThaiCTCV`, `TrangThaiCongViec` hoặc `TrangThaiDuAn` trong lần chỉnh này.
- Không thay đổi ID báo cáo, ID file, ID yêu cầu đổi quản lý hoặc ID nhật ký.
- Không xóa hoặc tạo thêm báo cáo/file tiến độ; số báo cáo là `592`, số file là `592`.
- Không chạy SQL vào database.

## D. Danh sách mốc thời gian cuối

| Nhóm dữ liệu | Ngày lớn nhất sau sửa |
| --- | --- |
| Báo cáo tiến độ - thời gian gửi | <= 2026-06-21 23:59:59 |
| Báo cáo tiến độ - thời gian duyệt | <= 2026-06-21 23:59:59 |
| File upload tiến độ | <= 2026-06-21 23:59:59 |
| Yêu cầu đổi quản lý | <= 2026-06-21 23:59:59 |
| Nhật ký quản lý | <= 2026-06-21 23:59:59 |
| Nhật ký nhân sự | <= 2026-06-21 23:59:59 |

## E. Truy vấn kiểm tra đã bổ sung

Đã bổ sung các nhóm truy vấn cuối file:

- `19.10`: kiểm tra ngày lớn nhất theo từng nhóm dữ liệu.
- `19.11`: kiểm tra báo cáo có thời gian duyệt không sau thời gian gửi.
- `19.12`: kiểm tra báo cáo `ChoDuyet` nhưng lại có người duyệt/thời gian duyệt/ghi chú duyệt.
- `19.13`: kiểm tra chuỗi báo cáo bị đảo thời gian trong từng chi tiết.
- `19.14`: kiểm tra file upload sau thời gian báo cáo hoặc sau thời gian duyệt.
- `19.15`: kiểm tra báo cáo gửi lại trước khi báo cáo `TuChoi` hoặc `YeuCauBoSung` được xử lý.
