# Review script insert dữ liệu vận hành tiến độ dự án trễ

## A. Kết luận tổng thể

**CÓ THỂ CHẠY SAU KHI SỬA**

Đã kiểm tra tĩnh hai file:

- `data3.sql` - nguồn schema, constraint và dữ liệu hiện tại.
- `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre.sql` - script bổ sung cần review.

Không chạy script vào database. Không chỉnh sửa `data3.sql`.

Kết luận cho file gốc: không phát hiện lỗi chắc chắn về tên bảng, tên cột, kiểu dữ liệu, khóa chính, identity hoặc khóa ngoại. Tuy nhiên file gốc **không nên chạy nguyên trạng** vì có lỗi nghiệp vụ trong chuỗi tiến độ và ngày báo cáo:

- 20 chi tiết có báo cáo 100%/`HoanThanh` xuất hiện trước báo cáo trung gian khi sắp theo `ThoiGianCapNhat`.
- 13 chi tiết có báo cáo trước `NgayBatDauCTCV`.
- 40 báo cáo `ChoDuyet` vẫn mở nhưng chi tiết/công việc/dự án được cập nhật hoàn thành sau đó.

Đã tạo bản sửa: `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre-fixed.sql`.

## B. Lỗi chắc chắn gây thất bại khi chạy

| STT | Dòng hoặc câu lệnh | Lỗi | Nguyên nhân | Cách sửa |
| --- | ------------------ | --- | ----------- | -------- |
| 1 | Không có | Không phát hiện lỗi chắc chắn gây lỗi SQL runtime qua kiểm tra tĩnh | Các bảng/cột/FK/PK/identity được đối chiếu với `data3.sql`; ID mới không trùng dữ liệu hiện tại | Không cần sửa ở nhóm lỗi SQL chắc chắn |

Ghi chú: chưa kiểm tra bằng SQL Server runtime/parser, nên không tuyên bố đã chạy thành công thực tế.

## C. Rủi ro dữ liệu và nghiệp vụ

| STT | Vấn đề | Mức độ | Ảnh hưởng | Đề xuất |
| --- | ------ | ------ | --------- | ------- |
| 1 | Các chi tiết CT 40, 42, 46, 47, 48, 52, 53, 54, 58, 59, 60, 76, 77, 78, 82, 83, 84, 102, 106, 108 có báo cáo sau báo cáo hoàn thành | Cao | Khi xem lịch sử theo thời gian, chi tiết đã 100% rồi lại quay về 65-70% hoặc còn `ChoDuyet` | Dùng file `-fixed.sql`, đã sắp lại thời gian để báo cáo cuối luôn là `DaDuyet` 100% `HoanThanh` |
| 2 | CT 13-24 và CT 42 có báo cáo trước `NgayBatDauCTCV` | Cao | Dữ liệu tiến độ không khớp thời gian chi tiết công việc | Dùng file `-fixed.sql`, đã đưa `ThoiGianCapNhat` về sau hoặc bằng `NgayBatDauCTCV` |
| 3 | 40 báo cáo `ChoDuyet` còn mở trước khi cập nhật hoàn thành | Trung bình | Có thể trái invariant màn hình báo cáo tiến độ: còn báo cáo chờ duyệt nhưng công việc đã hoàn thành | File `-fixed.sql` đổi các mốc này thành trạng thái đã xử lý hợp lệ, không còn `ChoDuyet` mở |
| 4 | `UPDATE CONG_VIEC` và `DU_AN` chuyển thẳng sang `HoanThanh` | Cao | Bỏ qua service workflow `ChoXacNhanHoanThanh` nếu muốn mô phỏng đúng thao tác UI | Nếu cần mô phỏng chặt UI, thêm log/xử lý xác nhận hoàn thành hoặc đổi trạng thái cuối thành `ChoXacNhanHoanThanh`; file fixed giữ `HoanThanh` vì mục tiêu dữ liệu yêu cầu hoàn thành trễ |
| 5 | Chống chạy trùng dựa vào khoảng identity cố định | Trung bình | Nếu DB đã có dữ liệu vận hành khác trong các khoảng ID này thì script dừng dù chưa từng chạy file | Trước khi chạy, xác nhận các bảng identity đang trống hoặc chưa vượt vùng ID; cách chắc hơn là dùng dấu mốc riêng theo tên file/ghi chú |

## D. Kiểm tra khóa ngoại

| Bảng ghi dữ liệu | FK kiểm tra | Kết quả |
| --- | --- | --- |
| `PHAN_CONG_CT_CONG_VIEC` | `MaNguoiDung=6`, `MaChiTietCV=1` | Hợp lệ. Người dùng 6 tồn tại, active; CT 1 tồn tại; cặp chưa có trong data3.sql |
| `NHAN_VIEN_DU_AN` | `MaDuAn`, `MaNguoiDung` | 5 bản ghi đều trỏ tới dự án 1/3/5/8/10 và người dùng active; không trùng PK hiện tại |
| `YEU_CAU_DOI_QUAN_LY` | `MaDuAn`, `MaQuanLyHienTai`, `MaQuanLyDeXuat`, `MaNguoiDungDuyet` | Hợp lệ. Dự án 2/3/4/6/7/10 tồn tại; quản lý hiện tại khớp data3.sql tại thời điểm đầu script; người duyệt 1 tồn tại |
| `NHAT_KY_QUAN_LY_DU_AN` | `MaDuAn`, `MaNguoiDung` | Hợp lệ. Dự án 3/6 tồn tại; quản lý mới 4/5 tồn tại và active |
| `NHAT_KY_PHU_TRACH_DU_AN` | `MaDuAn`, `MaNguoiDung` | Hợp lệ. Tất cả dự án/người dùng tồn tại và active |
| `TIEN_DO_CONG_VIEC` | `MaChiTietCV`, `MaNguoiDung`, `MaNguoiDungDuyet` | Hợp lệ về FK. Tất cả CT 1-123 tồn tại; người báo cáo active và thuộc đúng dự án; người duyệt active |
| `FILE_TIEN_DO_CONG_VIEC` | `MaTienDo` | Hợp lệ. 592 file tham chiếu đúng 592 báo cáo được insert trước đó trong cùng transaction |
| `DU_AN` update | `MaNguoiDung` | Hợp lệ. Quản lý cuối 2/4/5 đều tồn tại và active |
| `CONG_VIEC` update | `MaCongViec` | Hợp lệ. 62 công việc tồn tại |
| `CT_CONG_VIEC` update | `MaChiTietCV` | Hợp lệ. 123 chi tiết tồn tại |

## E. Kiểm tra IDENTITY và khóa chính

| Bảng | PK / Identity | Khoảng ID hiện tại trong data3.sql | Khoảng ID file gốc dùng | Trùng? |
| --- | --- | --- | --- | --- |
| `TIEN_DO_CONG_VIEC` | `MaTienDo` identity | 0 | 1-592 | Không |
| `FILE_TIEN_DO_CONG_VIEC` | `MaFileTDCV` identity | 0 | 1-592 | Không |
| `YEU_CAU_DOI_QUAN_LY` | `MaYeuCauDoiQuanLy` identity | 0 | 1-6 | Không |
| `NHAT_KY_QUAN_LY_DU_AN` | `MaNhatKyQLDA` identity | 184 | 185-186 | Không |
| `NHAT_KY_PHU_TRACH_DU_AN` | `MaNhatKyPTDA` identity | 52 | 53-59 | Không |
| `PHAN_CONG_CT_CONG_VIEC` | PK (`MaNguoiDung`, `MaChiTietCV`) | Có dữ liệu cũ | (6, 1) | Không |
| `NHAN_VIEN_DU_AN` | PK (`MaDuAn`, `MaNguoiDung`) | Có dữ liệu cũ | (1,14), (3,16), (5,15), (8,12), (10,14) | Không |

`IDENTITY_INSERT` trong file gốc chỉ bật cho một bảng tại một thời điểm và đều có `OFF`. Không có `GO` nằm trong transaction/TRY chính.

## F. Kiểm tra workflow tiến độ

| Chỉ số | File gốc | File fixed |
| --- | ---: | ---: |
| Số chi tiết | 123 | 123 |
| Số báo cáo | 592 | 592 |
| Số báo cáo `DaDuyet` | 430 | 450 |
| Số báo cáo `TuChoi` | 61 | 61 |
| Số báo cáo `YeuCauBoSung` | 61 | 81 |
| Số báo cáo `ChoDuyet` | 40 | 0 |
| Số báo cáo thiếu file | 0 | 0 |
| Số file mồ côi | 0 | 0 |
| Số đường dẫn file trùng | 0 | 0 |
| Số chuỗi tiến độ bất hợp lý | 20 chi tiết | 0 |
| Số lỗi ngày báo cáo trước ngày bắt đầu CT | 13 chi tiết | 0 |

Trạng thái sử dụng đều nằm trong constants/source: `DangThucHien`, `BiCanCan`, `HoanThanh`, `DaDuyet`, `TuChoi`, `YeuCauBoSung`, `ChoDuyet`.

## G. Kiểm tra thay đổi nhân sự và quản lý

| Dự án | Loại thay đổi | Người cũ | Người mới | Kết quả | Hợp lệ? |
| --- | --- | --- | --- | --- | --- |
| 3 | Đổi quản lý | Trần Hoàng Nam (2) | Lê Thị Thanh Hương (4) | `DaDuyet`, update `DU_AN.MaNguoiDung=4` | Có |
| 6 | Đổi quản lý | Lê Thị Thanh Hương (4) | Phạm Quốc Bảo (5) | `DaDuyet`, update `DU_AN.MaNguoiDung=5` | Có |
| 2 | Đổi quản lý | Trần Hoàng Nam (2) | Phạm Quốc Bảo (5) | `TuChoi`, không update quản lý | Có |
| 4 | Đổi quản lý | Lê Thị Thanh Hương (4) | Trần Hoàng Nam (2) | `TuChoi`, không update quản lý | Có |
| 7 | Đổi quản lý | Phạm Quốc Bảo (5) | Lê Thị Thanh Hương (4) | `TuChoi`, không update quản lý | Có |
| 10 | Đổi quản lý | Phạm Quốc Bảo (5) | Trần Hoàng Nam (2) | `TuChoi`, không update quản lý | Có |
| 1 | Thêm nhân sự | Không áp dụng | Nguyễn Thảo Vy (14) | Insert `NHAN_VIEN_DU_AN` + nhật ký | Có |
| 3 | Thêm nhân sự | Không áp dụng | Trần Ngọc Lan (16) | Insert `NHAN_VIEN_DU_AN` + nhật ký | Có |
| 5 | Thêm nhân sự | Không áp dụng | Đặng Quốc Khánh (15) | Insert `NHAN_VIEN_DU_AN` + nhật ký | Có |
| 8 | Thêm nhân sự | Không áp dụng | Trần Gia Huy (12) | Insert `NHAN_VIEN_DU_AN` + nhật ký | Có |
| 10 | Thêm nhân sự | Không áp dụng | Nguyễn Thảo Vy (14) | Insert `NHAN_VIEN_DU_AN` + nhật ký | Có |

Không có thao tác xóa/gỡ nhân viên, nên không có rủi ro phân công trỏ tới người bị gỡ khỏi dự án.

## H. Danh sách chỉnh sửa bắt buộc

1. Sửa thời gian các báo cáo của CT 40, 42, 46, 47, 48, 52, 53, 54, 58, 59, 60, 76, 77, 78, 82, 83, 84, 102, 106, 108 để báo cáo cuối theo thời gian luôn là `DaDuyet` 100% `HoanThanh`.
2. Sửa thời gian báo cáo đầu của CT 13-24 và CT 42 để không nhỏ hơn `NgayBatDauCTCV`.
3. Không để báo cáo `ChoDuyet` còn mở nếu sau đó đã cập nhật CT/CV/DU sang hoàn thành.
4. Nên chạy `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre-fixed.sql` thay cho file gốc.
5. Nếu muốn mô phỏng workflow UI nghiêm ngặt, cân nhắc thêm dữ liệu/log xác nhận hoàn thành hoặc đổi trạng thái cuối `CONG_VIEC`/`DU_AN` sang `ChoXacNhanHoanThanh`. Nếu mục tiêu là bộ dữ liệu dự án đã hoàn thành trễ, trạng thái `HoanThanh` trong file fixed là nhất quán với mục tiêu đó.

## Kiểm tra SELECT cuối file

Các SELECT cuối file dùng đúng bảng/cột. Không có lệnh ghi dữ liệu. Query 19.1 dùng `COUNT(DISTINCT ...)` cho chi tiết/báo cáo/file nên không bị phóng đại bởi join file. Các query 19.2-19.7 có thể chạy độc lập sau transaction.

## Kiểm tra cú pháp tĩnh

Đã kiểm tra tĩnh:

- Không có `GO` bên trong transaction/TRY chính.
- `BEGIN TRY`, `BEGIN TRANSACTION`, `COMMIT TRANSACTION`, `BEGIN CATCH`, `ROLLBACK`, `THROW` có thứ tự hợp lệ về mặt batch.
- `THROW 51000, N'...', 1;` và `THROW;` đúng dạng SQL Server.
- CATCH có cố gắng tắt `IDENTITY_INSERT` cho từng bảng trước khi rollback.

Chưa kiểm tra bằng SQL Server runtime/parser và không kết nối database thật.
