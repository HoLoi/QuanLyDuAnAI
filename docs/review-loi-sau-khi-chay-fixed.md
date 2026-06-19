# Review lỗi sau khi chạy file fixed

## A. Kết luận

**DỮ LIỆU ĐÃ COMMIT, CHỈ CẦN CHẠY LẠI FILE KIỂM TRA**

File phát sinh lỗi thực tế là:

`docs/insert-du-lieu-van-hanh-tien-do-du-an-tre-fixed.sql`

Lỗi `Invalid column name 'HoTen'` nằm trong các câu `SELECT` kiểm tra cuối file, sau `COMMIT TRANSACTION` và sau batch `GO`. Vì vậy phần `INSERT`/`UPDATE` chính đã có khả năng rất cao được commit đầy đủ trước khi lỗi xảy ra. Không được chạy lại toàn bộ file insert.

Cần chạy trước:

`docs/kiem-tra-du-lieu-sau-khi-chay-fixed.sql`

Sau đó có thể chạy:

`docs/kiem-tra-du-lieu-van-hanh-tien-do-fixed.sql`

Không tạo `fixed-v2.sql` vì lỗi không nằm trong phần ghi dữ liệu.

## B. Nguyên nhân lỗi

Bảng liên quan: `[dbo].[NGUOI_DUNG]`

Schema thật trong `data3.sql`:

- Mã người dùng: `[MaNguoiDung]`
- Họ tên người dùng: `[HoTenNguoiDung]`
- Xóa mềm: `[IsDeleted]`
- Không có cột `[HoTen]`
- Không có cột `[Email]` hoặc `[TenDangNhap]` trong bảng `[dbo].[NGUOI_DUNG]`; email/tên đăng nhập nằm ở bảng `[dbo].[AspNetUsers]`

Các vị trí lỗi trong `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre-fixed.sql`:

| STT | Dòng | Câu SQL | Alias sai | Cột sai | Cột đúng |
| --- | ---: | ------- | --------- | ------- | -------- |
| 1 | 1536 | `SELECT td.MaTienDo, da.MaDuAn, da.TenDuAn, td.MaNguoiDung, nd.HoTen` | `nd` | `HoTen` | `HoTenNguoiDung` |
| 2 | 1567 | `qlht.HoTen AS TenQuanLyHienTai` | `qlht` | `HoTen` | `HoTenNguoiDung` |
| 3 | 1567 | `qldx.HoTen AS TenQuanLyDeXuat` | `qldx` | `HoTen` | `HoTenNguoiDung` |
| 4 | 1577 | `SELECT nk.MaNhatKyPTDA, ..., nd.HoTen, ...` | `nd` | `HoTen` | `HoTenNguoiDung` |

Các lỗi tương tự cũng tồn tại trong file gốc `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre.sql` tại các dòng 1537, 1568 và 1578.

## C. Vị trí commit và lỗi

| Mốc | Dòng trong file fixed | Ghi chú |
| --- | ---: | ------ |
| `BEGIN TRY` | 5 | Bắt đầu khối ghi dữ liệu chính |
| `BEGIN TRANSACTION` | 6 | Transaction bao toàn bộ phần insert/update |
| `COMMIT TRANSACTION` | 1451 | Phần dữ liệu chính đã commit tại đây |
| `BEGIN CATCH` | 1453 | Chỉ xử lý lỗi xảy ra trước khi commit trong batch này |
| `ROLLBACK TRANSACTION` | 1486 | Chỉ chạy nếu lỗi xảy ra trong TRY trước commit |
| `THROW` | 1488 | Ném lại lỗi trong CATCH |
| `GO` sau CATCH | 1490 | Kết thúc batch ghi dữ liệu |
| Lỗi đầu tiên `HoTen` | 1536 | Nằm trong batch SELECT kiểm tra, sau commit |
| Lỗi tiếp theo `HoTen` | 1567, 1577 | Các SELECT kiểm tra sau đó |

Kết luận: lỗi xảy ra **sau commit**. SQL Server không rollback dữ liệu đã commit chỉ vì SELECT kiểm tra ở batch sau bị sai cột.

## D. Danh sách sửa đổi

| STT | File | Vị trí | Cột sai | Cột đúng | Đã sửa |
| --- | ---- | ------ | ------- | -------- | ------ |
| 1 | `docs/kiem-tra-du-lieu-sau-khi-chay-fixed.sql` | Các truy vấn kiểm tra người dùng | `nd.HoTen` | `nd.HoTenNguoiDung` | Có |
| 2 | `docs/kiem-tra-du-lieu-sau-khi-chay-fixed.sql` | Kiểm tra đổi quản lý | `qlCu/qlMoi/qlDuAn.HoTen` | `HoTenNguoiDung` | Có |
| 3 | `docs/kiem-tra-du-lieu-van-hanh-tien-do-fixed.sql` | SELECT 19.4 | `nd.HoTen` | `nd.HoTenNguoiDung AS HoTen` | Có |
| 4 | `docs/kiem-tra-du-lieu-van-hanh-tien-do-fixed.sql` | SELECT 19.6 | `qlht.HoTen`, `qldx.HoTen` | `HoTenNguoiDung` | Có |
| 5 | `docs/kiem-tra-du-lieu-van-hanh-tien-do-fixed.sql` | SELECT 19.7 | `nd.HoTen` | `nd.HoTenNguoiDung AS HoTen` | Có |

Giữ nguyên alias hiển thị sau `AS`, ví dụ `AS HoTen`, `AS TenQuanLyHienTai`.

## E. Trạng thái dữ liệu

Bảng có thể đã được insert/update đầy đủ vì nằm trước `COMMIT TRANSACTION`:

- `[dbo].[PHAN_CONG_CT_CONG_VIEC]`: thêm cặp `(MaNguoiDung=6, MaChiTietCV=1)`
- `[dbo].[YEU_CAU_DOI_QUAN_LY]`: ID 1-6
- `[dbo].[NHAT_KY_QUAN_LY_DU_AN]`: ID 185-186
- `[dbo].[NHAN_VIEN_DU_AN]`: 5 nhân sự bổ sung
- `[dbo].[NHAT_KY_PHU_TRACH_DU_AN]`: ID 53-59
- `[dbo].[TIEN_DO_CONG_VIEC]`: ID 1-592
- `[dbo].[FILE_TIEN_DO_CONG_VIEC]`: ID 1-592
- `[dbo].[CT_CONG_VIEC]`: cập nhật 123 chi tiết
- `[dbo].[CONG_VIEC]`: cập nhật 62 công việc
- `[dbo].[DU_AN]`: cập nhật 10 dự án, cộng 2 update đổi quản lý trước đó

Bảng có thể chưa được insert: không xác định bảng ghi dữ liệu nào còn thiếu theo cấu trúc script, vì lỗi xảy ra sau commit.

ID mới đã dùng nếu commit thành công:

- `MaTienDo`: 1-592
- `MaFileTDCV`: 1-592
- `MaYeuCauDoiQuanLy`: 1-6
- `MaNhatKyQLDA`: 185-186
- `MaNhatKyPTDA`: 53-59

Nguy cơ khi chạy lại file insert:

- Trùng primary key các bảng identity ở các vùng ID trên.
- Trùng cặp `(MaNguoiDung, MaChiTietCV)` trong `[dbo].[PHAN_CONG_CT_CONG_VIEC]`.
- Trùng cặp `(MaDuAn, MaNguoiDung)` trong `[dbo].[NHAN_VIEN_DU_AN]`.
- Các update trạng thái dự án/công việc bị chạy lại, làm khó phân biệt lịch sử.

Vì vậy không chạy lại `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre-fixed.sql`.

## F. Hướng dẫn chạy

1. Chạy `docs/kiem-tra-du-lieu-sau-khi-chay-fixed.sql`.
2. Đọc kết quả số lượng bản ghi:
   - Nếu `SoBaoCaoTrongVungMoPhong = 592`
   - Và `SoFileTrongVungMoPhong = 592`
   - Và `SoYeuCauDoiQuanLyTrongVungMoPhong = 6`
   thì dữ liệu mô phỏng đã có trong database.
3. Nếu dữ liệu đã có:
   - Không chạy lại file insert.
   - Chạy `docs/kiem-tra-du-lieu-van-hanh-tien-do-fixed.sql` để thay thế phần SELECT cuối bị lỗi.
4. Nếu dữ liệu chưa có và các vùng ID đều trống:
   - Báo lại kết quả kiểm tra trước khi tạo script insert mới.
   - Không tự chạy lại file cũ vì có thể database đang ở trạng thái khác với `data3.sql`.
5. Chạy lại file kiểm tra sau khi cần đối chiếu lần cuối.

## G. Kiểm tra thêm lỗi tên cột

Đã rà các file:

- `data3.sql`
- `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre.sql`
- `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre-fixed.sql`
- `docs/kiem-tra-du-lieu-sau-khi-chay-fixed.sql`
- `docs/kiem-tra-du-lieu-van-hanh-tien-do-fixed.sql`

Không phát hiện tham chiếu sai khác trong phần file kiểm tra mới. Hai file insert cũ vẫn giữ nguyên, không ghi đè, và vẫn còn lỗi `alias.HoTen` ở phần SELECT cuối.
