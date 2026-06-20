# Kiểm tra insert-du-lieu-10-du-an-tre-moi.sql

## 1. Phạm vi kiểm tra

Đã kiểm tra file `insert-du-lieu-10-du-an-tre-moi.sql` theo schema trong `data7.sql`, entity trong `Models/Entities`, cấu hình `QuanLyDuAnDbContext`, `Constants/TrangThai.cs`, các service nghiệp vụ dự án, công việc, chi tiết, tiến độ, đề xuất, ngân sách, phân công, team, nhân viên dự án, yêu cầu đổi quản lý và các tài liệu liên quan trong `docs`.

Không chạy script vào SQL Server. Không chỉnh sửa `data7.sql`, source code, entity, DbContext, migration hoặc schema.

## 2. Các bảng được sử dụng

Script sau sửa thao tác `INSERT` vào 24 bảng nghiệp vụ:

| STT | Bảng | Vai trò |
| ---: | --- | --- |
| 1 | `DU_AN` | Dự án DATA8 |
| 2 | `TEAM_DU_AN` | Team phụ trách dự án |
| 3 | `NHAN_VIEN_DU_AN` | Thành viên dự án |
| 4 | `NHAT_KY_DU_AN` | Nhật ký team dự án |
| 5 | `NHAT_KY_PHU_TRACH_DU_AN` | Nhật ký nhân sự dự án |
| 6 | `NHAT_KY_QUAN_LY_DU_AN` | Nhật ký quản lý dự án |
| 7 | `YEU_CAU_DOI_QUAN_LY` | Yêu cầu đổi quản lý |
| 8 | `DANH_MUC_CONG_VIEC` | Danh mục công việc |
| 9 | `DE_XUAT_NGAN_SACH` | Đề xuất ngân sách |
| 10 | `NGAN_SACH` | Ngân sách active |
| 11 | `NHAT_KY_NGAN_SACH` | Nhật ký ngân sách |
| 12 | `DE_XUAT_CONG_VIEC` | Đề xuất công việc |
| 13 | `CONG_VIEC` | Công việc thật |
| 14 | `CHI_PHI` | Chi phí |
| 15 | `NHAT_KY_CHI_PHI` | Nhật ký chi phí |
| 16 | `CT_CONG_VIEC` | Chi tiết công việc |
| 17 | `PHAN_CONG_CONG_VIEC` | Phân công công việc |
| 18 | `NHAT_KY_PHAN_CONG_CONG_VIEC` | Nhật ký phân công công việc |
| 19 | `PHAN_CONG_CT_CONG_VIEC` | Phân công chi tiết |
| 20 | `NHAT_KY_PHAN_CONG_CT_CONG_VIEC` | Nhật ký phân công chi tiết |
| 21 | `TIEN_DO_CONG_VIEC` | Báo cáo tiến độ |
| 22 | `FILE_TIEN_DO_CONG_VIEC` | File minh chứng tiến độ |
| 23 | `PHONG_CHAT` | Phòng chat dự án |
| 24 | `THANH_VIEN_PHONG_CHAT` | Thành viên phòng chat |

Không có thao tác ghi dữ liệu vào `AI_DATASET`, `AI_KET_QUA`, `AI_NGUYEN_NHAN`, `AI_MODEL`, `DM_NGUYEN_NHAN`.

## 3. Lỗi tên bảng và tên cột

Đã đối chiếu tự động danh sách cột trong các câu `INSERT INTO dbo.<Bang>(...)` với các block `CREATE TABLE [dbo].[...]` trong `data7.sql`. Kết quả sau sửa: 24/24 bảng dùng đúng tên bảng và cột.

| Bảng | Cột đang dùng trong SQL | Cột thật trong schema | Kết quả | Cách sửa |
| ---- | ----------------------- | --------------------- | ------- | -------- |
| `DU_AN` | `MaNguoiDung`, `MaLoaiDuAn`, `TenDuAn`, `MoTaDuAn`, `NgayTaoDuAn`, `NgayBatDauDuAn`, `NgayKetThucDuAn`, `NgayHoanThanhThucTeDuAn`, `PhanTramHoanThanh`, `TrangThaiDuAn`, `GhiChuDuAn`, `IsDeleted`, `DeletedAt`, `DeletedBy` | Khớp `data7.sql` và entity `DuAn` | Đúng | Không sửa |
| `NGAN_SACH` | `MaNguoiDungDuyet`, `MaNguoiDungDeXuat`, `MaDuAn`, `[NganSach]`, `Version`, `IsActive`, `MoTaNganSach`, `NgayCapNhatNganSach`, `NgayDuyetNganSach`, `TrangThaiNganSach`, `IsDeleted`, `DeletedAt`, `DeletedBy` | Cột SQL thật là `NganSach`; entity map `SoTienNganSach` sang `NganSach` | Đúng | Giữ `[NganSach]`, không đổi sang `SoTienNganSach` |
| `TIEN_DO_CONG_VIEC` | `MaChiTietCV`, `MaNguoiDung`, `MaNguoiDungDuyet`, `ThoiGianDuyet`, `GhiChuDuyet`, `PhanTram`, `GhiChuTienDo`, `ThoiGianCapNhat`, `TrangThaiCTCVDeXuat`, `TrangThaiTienDo` | Khớp schema/entity | Đúng | Không sửa |
| `FILE_TIEN_DO_CONG_VIEC` | `MaTienDo`, `TenFileTDCV`, `DuongDanFileTDCV`, `NgayUploadFileTDCV`, `IsDeleted`, `DeletedAt`, `DeletedBy` | Khớp schema/entity | Đúng | Không sửa |
| Các bảng còn lại | Cột trong script đều khớp cột `CREATE TABLE` | Khớp schema | Đúng | Không sửa tên cột |

## 4. Lỗi kiểu dữ liệu và NULL

Không phát hiện lỗi kiểu dữ liệu sau đối chiếu entity và schema:

| Nhóm cột | Kết quả |
| --- | --- |
| Khóa ngoại `int` | Dùng biến `INT`, lấy từ dữ liệu nền hoặc `SCOPE_IDENTITY()` |
| Tiền `decimal(18,2)` | Dùng giá trị số trong ngưỡng an toàn |
| `bit` | Dùng `0`, `1`, `NULL` đúng kiểu |
| Ngày giờ | Dùng literal ISO `yyyy-MM-ddTHH:mm:ss` hoặc `DATEADD` từ mốc ISO |
| Unicode | Chuỗi tiếng Việt dùng `N'...'` |
| Nullable FK | `MaNguoiDungDuyet`, ngày duyệt của trạng thái `ChoDuyet` để `NULL` |

## 5. Lỗi khóa chính và identity

| Mức độ | Vị trí | Nội dung cũ | Vấn đề | Nội dung đã sửa |
| ------ | ------ | ----------- | ------ | --------------- |
| Nghiêm trọng | Các `DECLARE` bên trong vòng `WHILE @BatchNo`, `WHILE @i`, `WHILE @d`, `WHILE @r` | Khai báo `@Member`, `@i`, `@MaCongViec`, `@MaChiPhi`, `@d`, `@r`, `@ReportCount`, `@MaTienDo` trong thân vòng lặp | T-SQL local variable có scope theo batch; khai báo trong vòng lặp có nguy cơ lỗi khi vòng lặp chạy lần thứ hai | Đưa biến dùng lặp ra vùng khai báo chung; trong vòng lặp chỉ dùng `SET`/`SELECT` để gán lại |
| Nhẹ | Biến phần trăm báo cáo tiến độ | Dùng lại tên `@PhanTram` cho cả phần trăm dự án và phần trăm tiến độ | Dễ nhầm ngữ nghĩa, tăng rủi ro stale value khi script lớn | Đổi biến tiến độ thành `@PhanTramTienDo` |

Các identity mới vẫn được lấy ngay sau câu `INSERT` tương ứng bằng `SCOPE_IDENTITY()`; không dùng `IDENTITY_INSERT`, không dùng `MAX(Id) + 1`.

## 6. Lỗi khóa ngoại

Không phát hiện sai khóa ngoại sau sửa. Các kiểm tra bổ sung trong script bảo vệ:

| Quan hệ | Cách kiểm tra/sửa |
| --- | --- |
| `CONG_VIEC.MaDanhMucCV` thuộc đúng dự án | Lấy từ `@DanhMucMoi` theo `BatchNo` |
| `CONG_VIEC.MaDeXuatCV` | Lấy `SCOPE_IDENTITY()` ngay sau `DE_XUAT_CONG_VIEC` đã duyệt |
| `CHI_PHI.MaNganSach` cùng dự án với `CONG_VIEC` | Có kiểm tra trước `COMMIT` qua `@CongViecMoi` và `@NganSachMoi` |
| `PHAN_CONG_CT_CONG_VIEC.MaNguoiDung` thuộc dự án | Có kiểm tra trước `COMMIT` với `NHAN_VIEN_DU_AN` |
| `FILE_TIEN_DO_CONG_VIEC.MaTienDo` | Lấy `@MaTienDo` ngay sau insert tiến độ |
| `YEU_CAU_DOI_QUAN_LY` | Chuỗi đổi quản lý đã duyệt theo manager cũ -> manager trung gian -> manager cuối |

## 7. Lỗi trạng thái và workflow

| Mức độ | Vị trí | Nội dung cũ | Vấn đề | Nội dung đã sửa |
| ------ | ------ | ----------- | ------ | --------------- |
| Nghiêm trọng | Logic tạo `CT_CONG_VIEC` cho `DATA8-07` | `WHEN @d = @DetailPerWork AND @BatchNo IN (5, 7, 10) THEN N'BiCanCan'` | `DATA8-07` có báo cáo cuối `ChoDuyet`; nếu đặt trạng thái thật `BiCanCan` thì trạng thái thật bị thay đổi bởi báo cáo chưa duyệt | Đổi thành `@BatchNo IN (5, 10)` để `DATA8-07` tạo tín hiệu trễ qua `YeuCauBoSung`/`ChoDuyet`, không khóa trạng thái thật bằng báo cáo chưa duyệt |
| Trung bình | Kiểm tra trước `COMMIT` | Chưa có kiểm tra trạng thái chi tiết khớp báo cáo `DaDuyet` mới nhất | Script có thể tự tạo trạng thái thật không tương ứng báo cáo đã duyệt | Bổ sung `OUTER APPLY` lấy báo cáo `DaDuyet` mới nhất và so với `@ChiTietMoi.TrangThaiCTCV` |

Các trạng thái dùng sau sửa đều thuộc `Constants/TrangThai.cs`: `HoanThanh`, `DangThucHien`, `ChoXacNhanHoanThanh`, `BiCanCan`, `ChoDuyet`, `DaDuyet`, `TuChoi`, `YeuCauBoSung`.

## 8. Lỗi logic thời gian

Không phát hiện literal thời gian vượt quá `2026-06-22T23:59:59`. Kiểm tra literal cho kết quả:

| Nội dung | Kết quả |
| --- | --- |
| Số literal ISO tìm thấy | 35 |
| Ngày literal lớn nhất | `2026-06-22 23:59:59` |
| Có `GETDATE()`/`SYSDATETIME()`/`CURRENT_TIMESTAMP` | Không |

Script có kiểm tra trước `COMMIT` cho thứ tự đề xuất, duyệt đề xuất, tạo công việc, báo cáo tiến độ, phân công chi tiết và ngày lớn nhất trong batch.

## 9. Lỗi nghiệp vụ đề xuất và ngân sách

Không phát hiện sai tên cột hoặc sai trạng thái. Cách lưu phù hợp `data7.sql` và service:

| Nhóm | Kết quả |
| --- | --- |
| `DE_XUAT_NGAN_SACH` `DaDuyet` | Có `NgayDeXuat < NgayDuyet`, có `MaNguoiDungDuyet`, sinh `NGAN_SACH` |
| `DE_XUAT_NGAN_SACH` `TuChoi` | Có ngày duyệt sau ngày đề xuất, không sinh ngân sách |
| `DE_XUAT_NGAN_SACH` `ChoDuyet` | Không có ngày duyệt và người duyệt |
| `NGAN_SACH` | Dùng cột SQL `[NganSach]`, `Version = 1`, `IsActive = 1`, `TrangThaiNganSach = N'DaDuyet'` |
| `CHI_PHI` | Liên kết đúng `MaCongViec` và `MaNganSach` cùng dự án |

## 10. Lỗi công việc, chi tiết và phân công

| Mức độ | Vị trí | Nội dung cũ | Vấn đề | Nội dung đã sửa |
| ------ | ------ | ----------- | ------ | --------------- |
| Trung bình | Block thay đổi nhân sự | Nhật ký `Xóa nhân sự khỏi dự án...` cho `@DevHuy` | Một số dự án có `StaffChanges >= 2` nhưng `@DevHuy` không nằm trong `@MemberMask`; ghi nhật ký gỡ người chưa chắc thuộc dự án | Đổi sang thêm/hỗ trợ nhân sự; nếu nhân sự chưa thuộc `NHAN_VIEN_DU_AN` thì insert thành viên dự án trước khi ghi nhật ký |
| Trung bình | Table variable `@Member` sau khi đưa ra ngoài vòng lặp | Ban đầu cần reset theo từng batch | Nếu reset bằng `DELETE` sẽ trái tinh thần “chỉ thêm dữ liệu” khi kiểm tra tĩnh | Thêm cột `BatchNo` vào `@Member`, khóa chính `(BatchNo, Code)`, mọi truy vấn lọc `BatchNo = @BatchNo` |

Phân công công việc và chi tiết lấy assignee từ `@Member` của đúng batch. Script có `NOT EXISTS` chống trùng `PHAN_CONG_CONG_VIEC`; chi tiết công việc tạo một phân công duy nhất theo khóa ghép.

## 11. Lỗi báo cáo tiến độ và file

Không phát hiện sai cột. Đã bổ sung kiểm tra trạng thái chi tiết theo báo cáo `DaDuyet` mới nhất như mục 7.

| Kiểm tra | Kết quả |
| --- | --- |
| `ChoDuyet` | `ThoiGianDuyet` và `MaNguoiDungDuyet` là `NULL` |
| `DaDuyet`/`TuChoi`/`YeuCauBoSung` | Có `ThoiGianDuyet > ThoiGianCapNhat` |
| File minh chứng | `NgayUploadFileTDCV = ThoiGianCapNhat + 10 phút` |
| Cột file | Khớp entity `FileTienDoCongViec` |

## 12. Lỗi thay đổi nhân sự và quản lý

| Mức độ | Vị trí | Nội dung cũ | Vấn đề | Nội dung đã sửa |
| ------ | ------ | ----------- | ------ | --------------- |
| Trung bình | Lookup dữ liệu nền user | Tìm `NGUOI_DUNG` bằng `HoTenNguoiDung` | `data7.sql` có tên bị xóa rồi tạo lại; tên hiển thị không phải khóa tự nhiên an toàn | Đổi lookup sang `NGUOI_DUNG` join `AspNetUsers` theo `NormalizedEmail`, vẫn lọc `IsDeleted = 0` |
| Trung bình | Đổi quản lý 2 lần | Đã có chuỗi trung gian nhưng cần giữ manager hiện tại của lần 2 đúng với lần 1 | Nếu lần 1 không đưa về manager trung gian thì lần 2 sai nghiệp vụ | Giữ `@QuanLyDeXuatLan1 = @ManagerHuong` khi `@ManagerChanges >= 2`; lần 2 đổi `@ManagerHuong -> @MaQuanLyChinh` |

## 13. Kiểm tra 10 dự án trễ

| Dự án | Trạng thái | Tổng CV | CV trễ | Tỷ lệ trễ | Số ngày trễ | Bằng chứng trễ |
| ----- | ---------- | ------: | -----: | --------: | ----------: | -------------- |
| `DATA8-01` | `HoanThanh` | 9 | 9 | 100% | 6 | `NgayHoanThanhThucTeDuAn > NgayKetThucDuAn`, công việc hoàn thành sau kế hoạch |
| `DATA8-02` | `DangThucHien` | 10 | 10 | 100% | 9 | Kế hoạch đã qua, còn 4 công việc chưa hoàn thành, có đề xuất bị từ chối/chờ duyệt |
| `DATA8-03` | `DangThucHien` | 7 | 7 | 100% | 7 | Duyệt chậm, nhiều báo cáo `YeuCauBoSung`, còn công việc chưa hoàn thành |
| `DATA8-04` | `HoanThanh` | 8 | 8 | 100% | 8 | Hoàn thành thực tế sau kế hoạch và chi phí thực tế vượt ngân sách |
| `DATA8-05` | `DangThucHien` | 6 | 6 | 100% | 6 | Có chi tiết `BiCanCan` được báo cáo duyệt, còn công việc chưa hoàn thành |
| `DATA8-06` | `HoanThanh` | 12 | 12 | 100% | 9 | Hoàn thành sau kế hoạch, có đổi quản lý và phối hợp nhiều team |
| `DATA8-07` | `DangThucHien` | 5 | 5 | 100% | 5 | Nhiều `YeuCauBoSung`/`ChoDuyet`, còn công việc chưa hoàn thành |
| `DATA8-08` | `HoanThanh` | 7 | 7 | 100% | 11 | Hoàn thành thực tế sau kế hoạch |
| `DATA8-09` | `ChoXacNhanHoanThanh` | 6 | 6 | 100% | 4 | Tất cả công việc hoàn thành muộn, dự án đang chờ xác nhận hoàn thành |
| `DATA8-10` | `DangThucHien` | 11 | 11 | 100% | 10 | Có 2 lần đổi quản lý, nhiều từ chối tiến độ, còn công việc chưa hoàn thành |

## 14. Kiểm tra Unicode và encoding

| Kiểm tra | Kết quả |
| --- | --- |
| UTF-8 strict | Đạt |
| Chuỗi tiếng Việt trong script | Dùng `N'...'` |
| Mojibake trong file khi đọc UTF-8 | Không phát hiện; terminal PowerShell có thể hiển thị sai codepage nhưng file decode UTF-8 đúng |
| Mật khẩu/token/secret trong report | Không sao chép |

## 15. Kiểm tra transaction và chạy lại

| Kiểm tra | Kết quả |
| --- | --- |
| `SET NOCOUNT ON` | Có |
| `SET XACT_ABORT ON` | Có |
| `BEGIN TRY`/`BEGIN TRANSACTION`/`COMMIT`/`CATCH ROLLBACK` | Có |
| `GO` trong transaction | Không |
| Kiểm tra batch tồn tại | Có: dừng nếu `DU_AN.TenDuAn LIKE N'DATA8-%'` |
| Xóa dữ liệu cũ để chạy lại | Không |
| `IDENTITY_INSERT` | Không |
| `MAX(Id) + 1` | Không |

## 16. Những thay đổi đã thực hiện

| Mức độ | Vị trí | Nội dung cũ | Vấn đề | Nội dung đã sửa |
| ------ | ------ | ----------- | ------ | --------------- |
| Nghiêm trọng | Biến trong vòng lặp | `DECLARE` biến/table variable bên trong `WHILE` | Có thể lỗi khi chạy nhiều vòng trong cùng batch | Khai báo một lần ngoài vòng lặp, dùng `SET`/`SELECT` trong vòng lặp |
| Nghiêm trọng | `CT_CONG_VIEC` batch 7 | `BiCanCan` dù báo cáo cuối `ChoDuyet` | Trạng thái thật có thể do báo cáo chưa duyệt tác động | Loại batch 7 khỏi logic `BiCanCan`; thêm kiểm tra latest `DaDuyet` |
| Trung bình | Lookup user | Tìm theo `HoTenNguoiDung` | Tên hiển thị không duy nhất, có bản ghi đã xóa trong `data7.sql` | Tìm theo `AspNetUsers.NormalizedEmail` |
| Trung bình | Nhật ký nhân sự | Ghi “xóa nhân sự” cho người chưa chắc thuộc dự án | Sai nghiệp vụ thay đổi nhân sự | Insert thành viên nếu thiếu rồi ghi nhật ký thêm/hỗ trợ/cập nhật |
| Trung bình | `@Member` | Nếu đưa ra ngoài vòng lặp cần reset | Tránh dùng `DELETE` dù chỉ table variable | Thêm `BatchNo` và lọc theo batch |
| Nhẹ | Biến phần trăm | `@PhanTram` dùng chung ngữ nghĩa | Dễ nhầm với phần trăm dự án | Đổi thành `@PhanTramTienDo` |
| Nhẹ | Comment cuối file | Có chữ `INSERT/UPDATE/DELETE/MERGE` trong comment | Gây nhiễu kiểm tra tĩnh | Đổi comment sang cảnh báo phạm vi bảng AI |

## 17. Những điểm còn rủi ro

| Mức độ | Nội dung | Ảnh hưởng | Hướng xử lý |
| --- | --- | --- | --- |
| Lưu ý | Chưa chạy script vào SQL Server thật | Chưa xác nhận parse/execute bằng engine thật | Chỉ chạy trên DB thử nghiệm khi được phép |
| Lưu ý | Script mô phỏng dữ liệu workflow bằng SQL trực tiếp, không gọi service | Một số trường `DateTime.Now` runtime được thay bằng mốc ISO cố định | Đã kiểm tra bằng source và thêm check trước `COMMIT` |
| Lưu ý | `TOP 1` vẫn dùng với `@Member` nội bộ batch | Không phụ thuộc dữ liệu nền; chỉ chọn người trong danh sách batch | Đã thêm `WHERE BatchNo = @BatchNo`, thứ tự xác định rõ |

## 18. Kết luận

Sau sửa, file `insert-du-lieu-10-du-an-tre-moi.sql` phù hợp với schema thật trong `data7.sql`, entity và cách mapping của `QuanLyDuAnDbContext`. Không phát hiện tên cột sai sau đối chiếu tự động. Các lỗi chính đã sửa thuộc nhóm cú pháp T-SQL khi lặp, lookup dữ liệu nền, nhật ký nhân sự và trạng thái chi tiết theo báo cáo tiến độ.

File hiện vẫn chưa được chạy vào database theo đúng yêu cầu kiểm thử không thao tác DB.
