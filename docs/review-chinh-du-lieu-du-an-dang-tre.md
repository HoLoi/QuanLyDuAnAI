# Review chỉnh dữ liệu dự án đang trễ

## Kết quả chỉnh sửa

Đã sửa trực tiếp file:

`docs/insert-du-lieu-van-hanh-tien-do-du-an-tre-fixed.sql`

Không chạy SQL vào database. Không sửa `data3.sql`. Không tạo `fixed-v2`.

## Thống kê chính

| Chỉ số | Số lượng |
| --- | ---: |
| Tổng số báo cáo tiến độ | 592 |
| Số báo cáo cuối đã đổi khỏi mẫu `100% HoanThanh` | 73 |
| Số chi tiết `HoanThanh` | 50 |
| Số chi tiết `DangThucHien` | 52 |
| Số chi tiết `BiCanCan` | 21 |
| Số báo cáo cuối `TuChoi` | 10 |
| Số báo cáo cuối `YeuCauBoSung` | 10 |
| Số công việc `DangThucHien` | 42 |
| Số công việc `HoanThanh` | 20 |
| Số dự án `DangThucHien` | 10 |
| Số dự án `HoanThanh` | 0 |
| Tổng file tiến độ | 592 |

## Trạng thái cuối từng dự án

| MaDuAn | TrangThaiDuAn | PhanTramHoanThanh | Chi tiết HoanThanh | Chi tiết DangThucHien | Chi tiết BiCanCan | Công việc DangThucHien |
| ---: | --- | ---: | ---: | ---: | ---: | ---: |
| 1 | `DangThucHien` | 68 | 5 | 5 | 2 | 4 |
| 2 | `DangThucHien` | 72 | 5 | 5 | 2 | 4 |
| 3 | `DangThucHien` | 69 | 5 | 5 | 2 | 4 |
| 4 | `DangThucHien` | 71 | 5 | 5 | 2 | 4 |
| 5 | `DangThucHien` | 74 | 5 | 5 | 2 | 4 |
| 6 | `DangThucHien` | 76 | 5 | 7 | 3 | 6 |
| 7 | `DangThucHien` | 70 | 5 | 5 | 2 | 4 |
| 8 | `DangThucHien` | 73 | 5 | 5 | 2 | 4 |
| 9 | `DangThucHien` | 67 | 5 | 5 | 2 | 4 |
| 10 | `DangThucHien` | 75 | 5 | 5 | 2 | 4 |

## Nội dung đã chỉnh

- Giữ nguyên `MaTienDo`, `MaFileTDCV` và quan hệ file tiến độ.
- Giữ nguyên các báo cáo ban đầu, báo cáo giữa kỳ, báo cáo bị từ chối, yêu cầu bổ sung, bị cản và các file đính kèm.
- Chỉ đổi các báo cáo cuối được chọn để không còn đồng loạt `PhanTram = 100`, `TrangThaiCTCVDeXuat = HoanThanh`, `TrangThaiTienDo = DaDuyet`.
- Cập nhật đồng bộ `CT_CONG_VIEC` theo báo cáo `DaDuyet` hợp lệ gần nhất.
- Không dùng báo cáo cuối `TuChoi` hoặc `YeuCauBoSung` để cập nhật trạng thái thật của chi tiết.
- Cập nhật `CONG_VIEC` theo trạng thái chi tiết: công việc còn chi tiết chưa hoàn thành được giữ `DangThucHien`.
- Cập nhật cả 10 `DU_AN` về `DangThucHien`, phần trăm dưới 100 và `NgayHoanThanhThucTeDuAn = NULL`.
- Không đặt ngày hoàn thành thực tế cho công việc `DangThucHien`.

## Kiểm tra kỹ thuật

- Không còn tham chiếu cột sai dạng `alias.HoTen`.
- Các tham chiếu họ tên dùng `HoTenNguoiDung`.
- Không phát hiện lỗi tên cột tĩnh trong phần SELECT cuối file.
- Không có báo cáo thiếu file.
- Không có file mồ côi.
- Không có đường dẫn file trùng.
- Không có dự án `HoanThanh`.
- Không có dự án có `PhanTramHoanThanh = 100`.
- Không có dự án thiếu công việc chưa hoàn thành.
- Không có dự án thiếu ít nhất 2 chi tiết chưa hoàn thành.
- Không có dự án thiếu chi tiết `BiCanCan`.
- Transaction vẫn giữ `SET XACT_ABORT ON`, `BEGIN TRY`, `BEGIN TRANSACTION`, `COMMIT`, `BEGIN CATCH`, `ROLLBACK`, `THROW`.
- `IDENTITY_INSERT` được bật/tắt từng bảng, không bật đồng thời nhiều bảng.

## Xác nhận nghiệp vụ

- 10 dự án vẫn chưa hoàn thành.
- 10 dự án đều ở trạng thái `DangThucHien`.
- Dữ liệu thể hiện dự án bị trễ do đã quá hạn nhưng vẫn còn công việc và chi tiết đang xử lý.
- Không sử dụng kịch bản nguyên nhân `Vượt ngân sách`.
- Không sử dụng kịch bản nguyên nhân `Khác`.
- Không xóa hai nguyên nhân này khỏi danh mục.

## File đã chỉnh

- `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre-fixed.sql`

## Ghi chú chạy lại

File này vẫn có cơ chế chống chạy trùng ở đầu transaction. Nếu database đã có dữ liệu mô phỏng từ lần chạy trước, script sẽ dừng trước phần insert để tránh trùng khóa chính và dữ liệu phân công/nhân sự.
