# Review sửa lỗi HoTen trong file fixed

## A. Nguyên nhân lỗi

File `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre-fixed.sql` dùng sai tên cột họ tên người dùng trong các truy vấn kiểm tra cuối file.

- Bảng liên quan: `[dbo].[NGUOI_DUNG]`
- Cột sai: `HoTen`
- Cột đúng theo `data3.sql`: `HoTenNguoiDung`
- Các dòng đã sửa: 1536, 1567, 1577

Schema thật của `[dbo].[NGUOI_DUNG]` trong `data3.sql` có các cột liên quan:

- `MaNguoiDung`
- `MaChucDanh`
- `Id`
- `HoTenNguoiDung`
- `IsDeleted`
- `DeletedAt`
- `DeletedBy`

Không có cột `HoTen` trong bảng này.

## B. Danh sách thay đổi

| STT | Vị trí | Nội dung cũ | Nội dung mới |
| --- | ------ | ----------- | ------------ |
| 1 | Dòng 1536 | `nd.HoTen` | `nd.HoTenNguoiDung AS HoTen` |
| 2 | Dòng 1567 | `qlht.HoTen AS TenQuanLyHienTai` | `qlht.HoTenNguoiDung AS TenQuanLyHienTai` |
| 3 | Dòng 1567 | `qldx.HoTen AS TenQuanLyDeXuat` | `qldx.HoTenNguoiDung AS TenQuanLyDeXuat` |
| 4 | Dòng 1577 | `nd.HoTen` | `nd.HoTenNguoiDung AS HoTen` |

Alias hiển thị sau `AS` được giữ nguyên.

## C. Kiểm tra bổ sung

- Không phát hiện tham chiếu cột sai khác trong phần SELECT cuối file qua kiểm tra tĩnh theo schema thật trong `data3.sql`.
- Không còn tham chiếu dạng `alias.HoTen`.
- Không sửa bất kỳ câu `INSERT` hoặc `UPDATE` nào.
- Không đổi dữ liệu nghiệp vụ, ID, ngày tháng, trạng thái, người báo cáo, người quản lý, nhân sự dự án hoặc đường dẫn file.
- Lỗi nằm sau `COMMIT TRANSACTION`: commit ở dòng 1451, lỗi đầu tiên ở dòng 1536.
- Do lỗi nằm trong phần SELECT kiểm tra sau commit, dữ liệu có khả năng đã được commit trước khi lỗi xảy ra.
- File vẫn giữ UTF-8 BOM.

## D. Hướng dẫn chạy lại

Không chạy lại toàn bộ phần `INSERT` nếu dữ liệu đã được commit.

Nên chạy phần truy vấn kiểm tra cuối file đã sửa, hoặc chạy file kiểm tra riêng:

`docs/kiem-tra-du-lieu-van-hanh-tien-do-fixed.sql`

Chỉ chạy lại toàn bộ `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre-fixed.sql` khi bạn đã xác nhận cơ sở dữ liệu chưa có dữ liệu mô phỏng trong các vùng ID của script và cơ chế chống chạy trùng cho phép chạy an toàn.
