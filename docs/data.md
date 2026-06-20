# Phân tích dữ liệu data7.sql

## 1. Phạm vi và mục tiêu

Tài liệu này phân tích file `data7.sql` trong thư mục gốc dự án `QuanLyDuAnAI`, đối chiếu với `QuanLyDuAnDbContext`, entity, service nghiệp vụ, `Constants/TrangThai.cs` và các tài liệu hiện có trong `docs`.

Mục tiêu là cung cấp đủ thông tin để một AI khác có thể thiết kế thêm một file SQL cho 10 dự án mới mà không cần suy đoán schema, workflow hoặc nhãn AI. Tài liệu này chỉ phân tích; không tạo dữ liệu mới, không chỉnh sửa `data7.sql`, không sửa source code và không thay đổi cơ sở dữ liệu.

Nguồn đã kiểm tra:

| Nhóm nguồn | Tệp hoặc thành phần |
| --- | --- |
| SQL | `data7.sql` |
| DbContext | `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs` |
| Trạng thái | `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs` |
| AI dataset | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs`, `AiService.cs` |
| Dự án và workflow | `DuAnService.cs`, `CongViecService.cs`, `TienDoCongViecService.cs`, `DyetDeXuatCongViecService.cs`, `DuyetDeXuatNganSachService.cs` |
| Tài liệu | `docs/mvc-ai-integration-rules.md`, `docs/ai-he-thong-phan-tich.md`, `docs/workflow-he-thong.md`, `docs/motacsdl.md`, `docs/congviec.md`, `docs/ctcongviec.md`, `docs/phancong.md`, `docs/duyetdexuatcongviec.md`, `docs/duyetngansach.md`, `docs/phantichnguyennhantre.md` |

Ghi chú kỹ thuật: `data7.sql` đang lưu ở encoding UTF-16. Tài liệu này được tạo bằng UTF-8.

## 2. Tổng quan file data7.sql

`data7.sql` là bản xuất dữ liệu và schema kiểu SQL Server, có DDL `CREATE TABLE`, `ALTER TABLE ... ADD CONSTRAINT`, và dữ liệu `INSERT`. File không phải script bổ sung an toàn để chạy nhiều lần; nó giống bản export toàn bộ môi trường dữ liệu demo hiện tại hơn là seed script có kiểm tra tồn tại.

Tổng quan thao tác:

| Nội dung | Kết quả đọc từ `data7.sql` |
| --- | --- |
| Mục tiêu thực tế | Xuất schema và dữ liệu vận hành demo gồm 10 dự án trễ hoặc có dấu hiệu trễ |
| Dữ liệu khởi tạo toàn hệ thống hay bổ sung | Gần với dữ liệu khởi tạo/toàn hệ thống vì có `AspNetRoles`, `AspNetUsers`, quyền, danh mục, team, người dùng, dự án, công việc |
| Số dự án trong `DU_AN` | 10 |
| Số bảng có `INSERT` | 40 |
| Có `UPDATE` | Không phát hiện |
| Có `DELETE` | Không phát hiện |
| Có `MERGE` | Không phát hiện |
| Có transaction | Không phát hiện `BEGIN TRAN`, `COMMIT`, `ROLLBACK` |
| Có `IDENTITY_INSERT` | Có, dùng cho nhiều bảng identity như `DU_AN`, `CONG_VIEC`, `CT_CONG_VIEC`, `TIEN_DO_CONG_VIEC`, `NGAN_SACH`, `DM_NGUYEN_NHAN` |
| Có biến tạm, bảng tạm, cursor | Không phát hiện `DECLARE`, bảng `#`, hoặc `CURSOR` trong phần dữ liệu |
| Có dynamic SQL | Không phát hiện `EXEC(...)` hoặc `sp_executesql` |
| Có `SCOPE_IDENTITY()` | Không có |
| Có kiểm tra tồn tại trước insert | Không có |
| An toàn khi chạy lại | Không an toàn, vì ghi cứng khóa chính và không kiểm tra trùng |

Danh sách bảng có `INSERT`:

| Bảng | Số dòng | Khoảng dòng trong file |
| --- | ---: | --- |
| `AspNetRoleClaims` | 105 | 1097-1202 |
| `AspNetRoles` | 3 | 1205-1207 |
| `AspNetUserRoles` | 17 | 1209-1225 |
| `AspNetUserTokens` | 2 | 1245-1246 |
| `AspNetUsers` | 17 | 1227-1243 |
| `CHI_PHI` | 62 | 1444-1505 |
| `CHUC_DANH` | 5 | 1510-1514 |
| `CONG_VIEC` | 62 | 1250-1311 |
| `CT_CONG_VIEC` | 123 | 1316-1439 |
| `DANH_MUC_CONG_VIEC` | 31 | 1519-1549 |
| `DANH_MUC_MAN_HINH` | 33 | 1554-1586 |
| `DANH_MUC_QUYEN` | 80 | 1591-1670 |
| `DE_XUAT_CONG_VIEC` | 72 | 1675-1746 |
| `DE_XUAT_NGAN_SACH` | 14 | 1751-1764 |
| `DM_NGUYEN_NHAN` | 10 | 1769-1778 |
| `DU_AN` | 10 | 1783-1792 |
| `FILE_TIEN_DO_CONG_VIEC` | 592 | 1797-2393 |
| `LOAI_DU_AN` | 3 | 2398-2400 |
| `MUC_DO_UU_TIEN` | 4 | 2405-2408 |
| `NGAN_SACH` | 10 | 2413-2422 |
| `NGUOI_DUNG` | 17 | 2427-2443 |
| `NHAN_VIEN_DU_AN` | 57 | 2446-2502 |
| `NHAN_VIEN_TEAM` | 9 | 2504-2512 |
| `NHAT_KY_CHI_PHI` | 62 | 2516-2577 |
| `NHAT_KY_DU_AN` | 21 | 2582-2602 |
| `NHAT_KY_NGAN_SACH` | 10 | 2607-2616 |
| `NHAT_KY_PHAN_CONG_CONG_VIEC` | 123 | 2621-2744 |
| `NHAT_KY_PHAN_CONG_CT_CONG_VIEC` | 35 | 2749-2783 |
| `NHAT_KY_PHU_TRACH_DU_AN` | 59 | 2788-2846 |
| `NHAT_KY_QUAN_LY_DU_AN` | 186 | 2851-3037 |
| `PHAN_CONG_CONG_VIEC` | 123 | 3040-3163 |
| `PHAN_CONG_CT_CONG_VIEC` | 123 | 3165-3288 |
| `PHONG_CHAT` | 10 | 3292-3301 |
| `TEAM` | 3 | 3306-3308 |
| `TEAM_DU_AN` | 21 | 3311-3331 |
| `THANH_VIEN_PHONG_CHAT` | 66 | 3958-4023 |
| `TIEN_DO_CONG_VIEC` | 592 | 3335-3931 |
| `TIEU_CHI_DANH_GIA` | 20 | 3936-3955 |
| `YEU_CAU_DOI_QUAN_LY` | 6 | 4027-4032 |
| `__EFMigrationsHistory` | 1 | 1093 |

Trình tự thực thi dữ liệu trong file không hoàn toàn theo thứ tự cha-con nghiệp vụ. Ví dụ `CONG_VIEC` được insert trước `DANH_MUC_CONG_VIEC`, `CHI_PHI` trước `NGAN_SACH`, `FILE_TIEN_DO_CONG_VIEC` trước `TIEN_DO_CONG_VIEC`. Điều này có thể vẫn chạy trong bản export nếu ràng buộc FK được tạo sau phần dữ liệu, nhưng không nên dùng lại làm mẫu cho seed bổ sung khi database đã có constraint.

## 3. Danh sách 10 dự án hiện tại

Bảng dưới đây dùng trực tiếp dữ liệu `DU_AN`, `LOAI_DU_AN`, `NGUOI_DUNG`. Cột `Số ngày trễ` và `Có trễ theo rule` là tính toán theo rule hiện tại trong `AiDatasetService.XacDinhDuAnTre`: dự án trễ nếu quá hạn chưa hoàn thành, có công việc trễ, tỷ lệ công việc trễ từ 30%, hoặc `SoNgayTreTienDo > 0`. Tính toán lấy ngày hiện tại của phiên làm việc là 2026-06-20.

| STT | Mã dự án | Tên dự án | Loại dự án | Quản lý | Ngày bắt đầu | Ngày kết thúc kế hoạch | Ngày hoàn thành thực tế | Trạng thái | Số ngày trễ | Nguyên nhân trễ |
| --- | -------: | --- | --- | --- | --- | --- | --- | --- | ---: | --- |
| 1 | 1 | Hệ thống quản lý bán hàng Minh Phát | Phát triển phần mềm | Trần Hoàng Nam | 2026-06-17 | 2026-06-20 | Chưa có | `DangThucHien` | 2 | Chỉ có trong `DU_AN.GhiChuDuAn`: thiếu nhân sự kiểm thử; chưa có `AI_NGUYEN_NHAN` |
| 2 | 2 | Nâng cấp cổng thông tin nội bộ An Phú | Bảo trì, nâng cấp | Trần Hoàng Nam | 2026-06-17 | 2026-06-20 | Chưa có | `DangThucHien` | 2 | Chỉ có trong `DU_AN.GhiChuDuAn`: thay đổi yêu cầu giao diện/phân quyền; chưa có `AI_NGUYEN_NHAN` |
| 3 | 3 | Ứng dụng quản lý kho Đông Nam | Phát triển phần mềm | Lê Thị Thanh Hương | 2026-06-17 | 2026-06-20 | Chưa có | `DangThucHien` | 2 | Chỉ có trong `DU_AN.GhiChuDuAn`: quy trình đối chiếu/xác nhận chậm; chưa có `AI_NGUYEN_NHAN` |
| 4 | 4 | Hệ thống đặt lịch khám trực tuyến | Phát triển phần mềm | Lê Thị Thanh Hương | 2026-06-17 | 2026-06-20 | Chưa có | `DangThucHien` | 2 | Chỉ có trong `DU_AN.GhiChuDuAn`: rủi ro kỹ thuật; chưa có `AI_NGUYEN_NHAN` |
| 5 | 5 | Nâng cấp hệ thống chăm sóc khách hàng | Bảo trì, nâng cấp | Lê Thị Thanh Hương | 2026-06-17 | 2026-06-20 | Chưa có | `DangThucHien` | 1 | Chỉ có trong `DU_AN.GhiChuDuAn`: chuyển đổi dữ liệu cũ; chưa có `AI_NGUYEN_NHAN` |
| 6 | 6 | Nền tảng quản lý đào tạo trực tuyến | Phát triển phần mềm | Phạm Quốc Bảo | 2026-06-17 | 2026-06-20 | Chưa có | `DangThucHien` | 1 | Chỉ có trong `DU_AN.GhiChuDuAn`: phối hợp frontend/API/kiểm thử; chưa có `AI_NGUYEN_NHAN` |
| 7 | 7 | Mô hình AI phân loại phản hồi khách hàng | Nghiên cứu AI | Phạm Quốc Bảo | 2026-06-17 | 2026-06-20 | Chưa có | `DangThucHien` | 1 | Chỉ có trong `DU_AN.GhiChuDuAn`: thông tin đầu vào và dữ liệu gán nhãn chưa đầy đủ; chưa có `AI_NGUYEN_NHAN` |
| 8 | 8 | Hệ thống quản lý bảo trì thiết bị | Phát triển phần mềm | Phạm Quốc Bảo | 2026-06-17 | 2026-06-20 | Chưa có | `DangThucHien` | 1 | Chỉ có trong `DU_AN.GhiChuDuAn`: ước lượng thời gian chưa sát; chưa có `AI_NGUYEN_NHAN` |
| 9 | 9 | AI hỗ trợ phân tích rủi ro dự án | Nghiên cứu AI | Phạm Quốc Bảo | 2026-06-17 | 2026-06-20 | Chưa có | `DangThucHien` | 1 | Chỉ có trong `DU_AN.GhiChuDuAn`: tiến độ cập nhật dữ liệu rủi ro chưa đầy đủ; chưa có `AI_NGUYEN_NHAN` |
| 10 | 10 | Cổng quản lý hồ sơ nhân sự điện tử | Phát triển phần mềm | Phạm Quốc Bảo | 2026-06-17 | 2026-06-20 | Chưa có | `DangThucHien` | 1 | Chỉ có trong `DU_AN.GhiChuDuAn`: thay đổi yêu cầu phân quyền hồ sơ; chưa có `AI_NGUYEN_NHAN` |

Kiểm tra trễ theo rule hiện tại:

| Mã dự án | Tổng công việc | Công việc trễ tính theo `CONG_VIEC` | Tỷ lệ công việc trễ | `SoNgayTreTienDo` dự kiến | Có trễ theo rule `AiDatasetService` |
| ---: | ---: | ---: | ---: | ---: | --- |
| 1 | 6 | 2 | 33.33% | 2 | Có |
| 2 | 6 | 2 | 33.33% | 2 | Có |
| 3 | 6 | 2 | 33.33% | 2 | Có |
| 4 | 6 | 3 | 50.00% | 2 | Có |
| 5 | 6 | 4 | 66.67% | 1 | Có |
| 6 | 8 | 3 | 37.50% | 1 | Có |
| 7 | 6 | 3 | 50.00% | 1 | Có |
| 8 | 6 | 4 | 66.67% | 1 | Có |
| 9 | 6 | 4 | 66.67% | 1 | Có |
| 10 | 6 | 4 | 66.67% | 1 | Có |

Lưu ý quan trọng: nhiều `NgayKetThucCVThucTe`, `TIEN_DO_CONG_VIEC.ThoiGianCapNhat`, `ThoiGianDuyet` và file minh chứng nằm ở 2026-06-21, tức sau ngày hiện tại 2026-06-20. Vì vậy kết quả “có trễ” dựa trên dữ liệu đã ghi trong SQL, nhưng dữ liệu có dấu hiệu được tạo với mốc tương lai.

## 4. Các bảng và số lượng dữ liệu

Thống kê nhóm dữ liệu theo từng dự án:

| Mã dự án | Nhân viên | Team | Danh mục CV | Công việc | CT công việc | Phân công CV | Phân công CT | Ngân sách | Đề xuất NS | Chi phí | Đề xuất CV | Báo cáo tiến độ |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | 6 | 2 | 3 | 6 | 12 | 12 | 12 | 1 | 1 | 6 | 7 | 58 |
| 2 | 5 | 2 | 3 | 6 | 12 | 12 | 12 | 1 | 2 | 6 | 7 | 58 |
| 3 | 6 | 2 | 3 | 6 | 12 | 12 | 12 | 1 | 2 | 6 | 7 | 58 |
| 4 | 5 | 2 | 3 | 6 | 12 | 12 | 12 | 1 | 1 | 6 | 7 | 58 |
| 5 | 6 | 2 | 3 | 6 | 12 | 12 | 12 | 1 | 1 | 6 | 7 | 58 |
| 6 | 7 | 3 | 4 | 8 | 15 | 15 | 15 | 1 | 1 | 8 | 9 | 70 |
| 7 | 5 | 2 | 3 | 6 | 12 | 12 | 12 | 1 | 2 | 6 | 7 | 58 |
| 8 | 6 | 2 | 3 | 6 | 12 | 12 | 12 | 1 | 1 | 6 | 7 | 58 |
| 9 | 5 | 2 | 3 | 6 | 12 | 12 | 12 | 1 | 2 | 6 | 7 | 58 |
| 10 | 6 | 2 | 3 | 6 | 12 | 12 | 12 | 1 | 1 | 6 | 7 | 58 |

Thống kê nhóm phụ trợ:

| Mã dự án | File tiến độ | Nhật ký quản lý DA | Đánh giá DA | Đánh giá NV | Phòng chat | Thành viên chat | `AI_DATASET` | `AI_KET_QUA` | `AI_NGUYEN_NHAN` |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | 58 | 19 | 0 | 0 | 1 | 7 | 0 | 0 | 0 |
| 2 | 58 | 19 | 0 | 0 | 1 | 6 | 0 | 0 | 0 |
| 3 | 58 | 20 | 0 | 0 | 1 | 7 | 0 | 0 | 0 |
| 4 | 58 | 17 | 0 | 0 | 1 | 6 | 0 | 0 | 0 |
| 5 | 58 | 17 | 0 | 0 | 1 | 7 | 0 | 0 | 0 |
| 6 | 70 | 22 | 0 | 0 | 1 | 8 | 0 | 0 | 0 |
| 7 | 58 | 19 | 0 | 0 | 1 | 6 | 0 | 0 | 0 |
| 8 | 58 | 17 | 0 | 0 | 1 | 7 | 0 | 0 | 0 |
| 9 | 58 | 19 | 0 | 0 | 1 | 6 | 0 | 0 | 0 |
| 10 | 58 | 17 | 0 | 0 | 1 | 6 | 0 | 0 | 0 |

Các bảng AI quan trọng `AI_DATASET`, `AI_KET_QUA`, `AI_NGUYEN_NHAN`, `AI_MODEL` không có dòng insert trong `data7.sql`. File có `DM_NGUYEN_NHAN` với 10 dòng danh mục, nhưng chưa có nhãn manager xác nhận cho 10 dự án.

## 5. Quan hệ khóa và thứ tự insert

Quan hệ nghiệp vụ chính:

`DU_AN -> DANH_MUC_CONG_VIEC -> CONG_VIEC -> CT_CONG_VIEC -> TIEN_DO_CONG_VIEC -> FILE_TIEN_DO_CONG_VIEC`

`DU_AN -> NHAN_VIEN_DU_AN -> PHAN_CONG_CONG_VIEC / PHAN_CONG_CT_CONG_VIEC`

`DU_AN -> TEAM_DU_AN -> TEAM -> NHAN_VIEN_TEAM`

`DU_AN -> DE_XUAT_CONG_VIEC -> CONG_VIEC -> CHI_PHI -> NGAN_SACH`

`DU_AN -> DE_XUAT_NGAN_SACH -> NGAN_SACH -> NHAT_KY_NGAN_SACH`

`DU_AN -> AI_DATASET -> AI_KET_QUA`, và `DU_AN -> AI_NGUYEN_NHAN`; cả hai AI table dùng `DM_NGUYEN_NHAN`.

Bảng khóa chính và cách file lấy mã:

| Bảng | Khóa chính theo source/schema | Identity | Khóa ngoại quan trọng | Bảng phải có trước khi seed bổ sung | Cách file hiện tại lấy mã |
| --- | --- | --- | --- | --- | --- |
| `DU_AN` | `MaDuAn` | Có | `MaNguoiDung`, `MaLoaiDuAn` | `NGUOI_DUNG`, `LOAI_DU_AN` | Ghi cứng `MaDuAn` 1-10, dùng `IDENTITY_INSERT` |
| `DANH_MUC_CONG_VIEC` | `MaDanhMucCV` | Có | `MaDuAn` | `DU_AN` | Ghi cứng 1-31 |
| `CONG_VIEC` | `MaCongViec` | Có | `MaDanhMucCV`, `MaMucDo`, `MaDeXuatCV` | `DANH_MUC_CONG_VIEC`, `MUC_DO_UU_TIEN`, `DE_XUAT_CONG_VIEC` | Ghi cứng 1-62 |
| `CT_CONG_VIEC` | `MaChiTietCV` | Có | `MaCongViec` | `CONG_VIEC` | Ghi cứng 1-123 |
| `TIEN_DO_CONG_VIEC` | `MaTienDo` | Có | `MaChiTietCV`, `MaNguoiDung`, `MaNguoiDungDuyet` | `CT_CONG_VIEC`, `NGUOI_DUNG` | Ghi cứng 1-592 |
| `FILE_TIEN_DO_CONG_VIEC` | `MaFileTDCV` | Có | `MaTienDo` | `TIEN_DO_CONG_VIEC` | Ghi cứng 1-592 |
| `NHAN_VIEN_DU_AN` | `MaDuAn`, `MaNguoiDung` | Không | `MaDuAn`, `MaNguoiDung` | `DU_AN`, `NGUOI_DUNG` | Ghi cứng cặp khóa |
| `TEAM_DU_AN` | `MaTeam`, `MaDuAn` | Không | `MaTeam`, `MaDuAn` | `TEAM`, `DU_AN` | Ghi cứng cặp khóa |
| `PHAN_CONG_CONG_VIEC` | `MaNguoiDung`, `MaCongViec` | Không | `MaNguoiDung`, `MaCongViec` | `NHAN_VIEN_DU_AN`, `CONG_VIEC` | Ghi cứng cặp khóa |
| `PHAN_CONG_CT_CONG_VIEC` | `MaNguoiDung`, `MaChiTietCV` | Không | `MaNguoiDung`, `MaChiTietCV` | `NHAN_VIEN_DU_AN`, `CT_CONG_VIEC` | Ghi cứng cặp khóa |
| `DE_XUAT_CONG_VIEC` | `MaDeXuatCV` | Có | `MaDuAn`, `MaDanhMucCV`, `MaMucDo`, `MaNguoiDungDeXuat`, `MaNguoiDungDuyet` | `DU_AN`, `DANH_MUC_CONG_VIEC`, `NGUOI_DUNG` | Ghi cứng 3-74, không có 1-2 |
| `DE_XUAT_NGAN_SACH` | `MaDeXuatNS` | Có | `MaDuAn`, `MaNganSachCu`, `MaNguoiDungDeXuat`, `MaNguoiDungDuyet` | `DU_AN`, `NGUOI_DUNG`, tùy trường hợp `NGAN_SACH` cũ | Ghi cứng 1-14 |
| `NGAN_SACH` | `MaNganSach` | Có | `MaDuAn`, `MaNguoiDungDeXuat`, `MaNguoiDungDuyet` | `DU_AN`, `NGUOI_DUNG` | Ghi cứng 1-10 |
| `CHI_PHI` | `MaChiPhi` | Có | `MaCongViec`, `MaNganSach` | `CONG_VIEC`, `NGAN_SACH` | Ghi cứng 1-62 |
| `PHONG_CHAT` | `MaPhongChat` | Có | `MaDuAn` | `DU_AN` | Ghi cứng 1-10 |
| `THANH_VIEN_PHONG_CHAT` | `MaPhongChat`, `MaNguoiDung` | Không | `MaPhongChat`, `MaNguoiDung` | `PHONG_CHAT`, `NGUOI_DUNG` | Ghi cứng cặp khóa |
| `YEU_CAU_DOI_QUAN_LY` | `MaYeuCauDoiQuanLy` | Có | `MaDuAn`, `MaQuanLyHienTai`, `MaQuanLyDeXuat`, `MaNguoiDungDuyet` | `DU_AN`, `NGUOI_DUNG` | Ghi cứng 1-6 |
| `DM_NGUYEN_NHAN` | `MaDMNguyenNhan` | Có | Không | Không | Ghi cứng 1-10 |
| `AI_DATASET` | `MaData` | Có | `MaDuAn`, `MaDMNguyenNhan` | `DU_AN`, `DM_NGUYEN_NHAN`, `AI_NGUYEN_NHAN` nếu cần nhãn | Không có dòng |
| `AI_KET_QUA` | `MaAiKetQua` | Có | `MaDuAn`, `MaData`, `MaModel`, `MaDMNguyenNhan` | `DU_AN`, `AI_DATASET`, `AI_MODEL`, `DM_NGUYEN_NHAN` | Không có dòng |
| `AI_NGUYEN_NHAN` | `MaAINguyenNhan` | Có | `MaDuAn`, `MaDMNguyenNhan` | `DU_AN`, `DM_NGUYEN_NHAN` | Không có dòng |

Rủi ro khóa:

| Loại mã | Tình trạng trong file | Rủi ro khi tạo dữ liệu mới |
| --- | --- | --- |
| Ghi cứng identity | Rất nhiều bảng dùng `IDENTITY_INSERT` | Dễ trùng nếu database đã có dữ liệu |
| Lấy bằng biến | Không thấy | Không có mẫu an toàn để tái sử dụng |
| Lấy bằng truy vấn theo tên | Không thấy trong `data7.sql` | Nếu dùng sau này phải bảo đảm tên duy nhất |
| Dùng `SCOPE_IDENTITY()` | Không thấy | Cần dùng khi viết script bổ sung mới |
| Phụ thuộc ID hiện tại | Có, gần như toàn bộ quan hệ phụ thuộc ID 1-592 | Không thể chạy an toàn trên DB khác nếu ID đã lệch |

## 6. Phân tích chi tiết từng dự án

| Mã dự án | Tóm tắt cấu trúc dữ liệu | Nhận xét nghiệp vụ |
| ---: | --- | --- |
| 1 | 6 nhân viên, 2 team, 3 danh mục, 6 công việc, 12 chi tiết, 58 báo cáo tiến độ, 1 ngân sách, 1 đề xuất ngân sách, 7 đề xuất công việc | Có tín hiệu thiếu nhân sự trong `GhiChuDuAn`; feature thực tế lại có 6 nhân viên và 6 nhật ký phụ trách, cần giải thích rõ nếu dùng làm nhãn `Thiếu nhân sự` |
| 2 | 5 nhân viên, 2 team, 3 danh mục, 6 công việc, 12 chi tiết, 58 báo cáo, 2 đề xuất ngân sách | Có đề xuất ngân sách bị từ chối và ghi chú thay đổi yêu cầu; phù hợp một phần với lớp `Thay đổi yêu cầu liên tục` |
| 3 | 6 nhân viên, 2 team, 3 danh mục, 6 công việc, 12 chi tiết, 58 báo cáo, 2 đề xuất ngân sách, 1 yêu cầu đổi quản lý đã duyệt | Có tín hiệu quy trình chậm/đổi quản lý; dữ liệu có 1 `YEU_CAU_DOI_QUAN_LY` được duyệt |
| 4 | 5 nhân viên, 2 team, 3 danh mục, 6 công việc, 12 chi tiết, 58 báo cáo | Ghi chú là rủi ro kỹ thuật, nhưng feature chủ yếu vẫn giống các dự án khác; cần thêm tín hiệu kỹ thuật rõ hơn nếu tạo dữ liệu mới |
| 5 | 6 nhân viên, 2 team, 3 danh mục, 6 công việc, 12 chi tiết, 58 báo cáo | Ghi chú chuyển đổi dữ liệu cũ; có 4/6 công việc trễ nhưng chưa có nhãn chuẩn |
| 6 | 7 nhân viên, 3 team, 4 danh mục, 8 công việc, 15 chi tiết, 70 báo cáo, 8 chi phí, 9 đề xuất công việc, 1 yêu cầu đổi quản lý đã duyệt | Là dự án lớn nhất trong 10 dự án, có đa dạng hơn về team/công việc/báo cáo |
| 7 | 5 nhân viên, 2 team, 3 danh mục, 6 công việc, 12 chi tiết, 58 báo cáo, 2 đề xuất ngân sách | Ghi chú thiếu thông tin đầu vào; nên bổ sung dữ liệu file/tiến độ/đề xuất thể hiện thiếu dữ liệu nếu sinh tiếp |
| 8 | 6 nhân viên, 2 team, 3 danh mục, 6 công việc, 12 chi tiết, 58 báo cáo | Ghi chú ước lượng thời gian chưa chính xác; tỷ lệ công việc trễ cao 66.67% là tín hiệu phù hợp |
| 9 | 5 nhân viên, 2 team, 3 danh mục, 6 công việc, 12 chi tiết, 58 báo cáo, 2 đề xuất ngân sách | Ghi chú tiến độ cập nhật không đầy đủ, nhưng thực tế mỗi báo cáo đều có file minh chứng; cần tránh mâu thuẫn khi tạo mới |
| 10 | 6 nhân viên, 2 team, 3 danh mục, 6 công việc, 12 chi tiết, 58 báo cáo | Ghi chú thay đổi yêu cầu phân quyền hồ sơ; giống dự án 2 về nguyên nhân nhưng chưa có nhãn AI |

Không có `DANH_GIA_DU_AN` hoặc `DANH_GIA_NHAN_VIEN` cho bất kỳ dự án nào, dù bảng tiêu chí đánh giá có dữ liệu.

## 7. Phân tích nhân sự, team và phân công

Danh sách quản lý, team và leader:

| Mã dự án | Quản lý | Team | Leader | Số thành viên `NHAN_VIEN_DU_AN` |
| ---: | --- | --- | --- | ---: |
| 1 | Trần Hoàng Nam | Backend và Dữ liệu; Frontend và UI/UX | Võ Thành Đạt; Lê Ngọc Mai | 6 |
| 2 | Trần Hoàng Nam | Frontend và UI/UX; Team Phân tích và Kiểm thử | Lê Ngọc Mai; Đặng Quốc Khánh | 5 |
| 3 | Lê Thị Thanh Hương | Backend và Dữ liệu; Team Phân tích và Kiểm thử | Võ Thành Đạt; Đặng Quốc Khánh | 6 |
| 4 | Lê Thị Thanh Hương | Backend và Dữ liệu; Frontend và UI/UX | Võ Thành Đạt; Lê Ngọc Mai | 5 |
| 5 | Lê Thị Thanh Hương | Backend và Dữ liệu; Frontend và UI/UX | Võ Thành Đạt; Lê Ngọc Mai | 6 |
| 6 | Phạm Quốc Bảo | Backend và Dữ liệu; Frontend và UI/UX; Team Phân tích và Kiểm thử | Võ Thành Đạt; Lê Ngọc Mai; Đặng Quốc Khánh | 7 |
| 7 | Phạm Quốc Bảo | Backend và Dữ liệu; Team Phân tích và Kiểm thử | Võ Thành Đạt; Đặng Quốc Khánh | 5 |
| 8 | Phạm Quốc Bảo | Backend và Dữ liệu; Frontend và UI/UX | Võ Thành Đạt; Lê Ngọc Mai | 6 |
| 9 | Phạm Quốc Bảo | Backend và Dữ liệu; Team Phân tích và Kiểm thử | Võ Thành Đạt; Đặng Quốc Khánh | 5 |
| 10 | Phạm Quốc Bảo | Frontend và UI/UX; Team Phân tích và Kiểm thử | Lê Ngọc Mai; Đặng Quốc Khánh | 6 |

Kiểm tra phân công:

| Nội dung kiểm tra | Kết quả |
| --- | --- |
| Phân công công việc cho người không thuộc dự án | Không phát hiện |
| Phân công chi tiết cho người không thuộc dự án | Không phát hiện |
| Trùng khóa `PHAN_CONG_CONG_VIEC` | Không phát hiện |
| Trùng khóa `PHAN_CONG_CT_CONG_VIEC` | Không phát hiện |
| Trùng thành viên `NHAN_VIEN_DU_AN` | Không phát hiện |
| Dự án thiếu manager | Không |
| Dự án thiếu leader | Không |
| Dự án thiếu team | Không |
| Dự án thiếu nhân viên thực hiện | Không |

Mức độ đa dạng nhân sự còn hạn chế: chỉ có 3 manager cho 10 dự án, 3 team, và các leader lặp lại nhiều lần. Dự án 6 đa dạng hơn vì có 3 team và 7 thành viên.

## 8. Phân tích công việc và chi tiết công việc

Tổng dữ liệu công việc:

| Nhóm | Số dòng | Trạng thái |
| --- | ---: | --- |
| `CONG_VIEC` | 62 | `HoanThanh`: 20, `DangThucHien`: 42 |
| `CT_CONG_VIEC` | 123 | `HoanThanh`: 50, `DangThucHien`: 52, `BiCanCan`: 21 |
| `PHAN_CONG_CONG_VIEC` | 123 | Khóa chính cặp `MaNguoiDung`, `MaCongViec` |
| `PHAN_CONG_CT_CONG_VIEC` | 123 | Khóa chính cặp `MaNguoiDung`, `MaChiTietCV` |

Nhận xét:

| Vấn đề | Phân tích |
| --- | --- |
| Công việc hoàn thành nhưng dự án chưa hoàn thành | Có, 20 công việc `HoanThanh` trong khi tất cả dự án vẫn `DangThucHien`; điều này hợp lý nếu dự án còn công việc đang xử lý |
| Công việc hoàn thành nhưng chi tiết chưa hoàn thành | Cần kiểm tra khi chạy DB thật; theo thống kê tổng có 50 chi tiết `HoanThanh`, 52 `DangThucHien`, 21 `BiCanCan` |
| Công việc trễ | Có 2-4 công việc trễ mỗi dự án theo `NgayKetThucCVDuKien` và `NgayKetThucCVThucTe` hoặc trạng thái chưa hoàn thành |
| Mẫu dữ liệu | Phần lớn dự án có cùng cấu trúc 6 công việc, 12 chi tiết; dự án 6 là ngoại lệ có 8 công việc, 15 chi tiết |

## 9. Phân tích thời gian và trạng thái

Khoảng thời gian theo bảng:

| Bảng | Mốc sớm nhất | Mốc muộn nhất |
| --- | --- | --- |
| `DU_AN` | 2026-06-17 | 2026-06-20 |
| `DANH_MUC_CONG_VIEC` | 2026-06-17 20:06:24 | 2026-06-17 20:32:36 |
| `DE_XUAT_NGAN_SACH` | 2026-06-17 21:47:06 | 2026-06-17 22:58:42 |
| `NGAN_SACH` | 2026-06-17 21:48:29 | 2026-06-17 22:58:42 |
| `DE_XUAT_CONG_VIEC` | 2026-06-18 | 2026-06-20 |
| `CONG_VIEC` | 2026-06-18 | 2026-06-21 18:15:00 |
| `CT_CONG_VIEC` | 2026-06-18 | 2026-06-20 |
| `TIEN_DO_CONG_VIEC` | 2026-06-18 08:00:00 | 2026-06-21 21:07:00 |
| `FILE_TIEN_DO_CONG_VIEC` | 2026-06-18 08:00:00 | 2026-06-21 16:37:00 |
| `YEU_CAU_DOI_QUAN_LY` | 2026-06-20 14:00:00 | 2026-06-21 15:45:00 |
| `NHAT_KY_PHU_TRACH_DU_AN` | 2026-06-17 15:58:15 | 2026-06-21 16:30:00 |
| `NHAT_KY_QUAN_LY_DU_AN` | 2026-06-17 15:44:55 | 2026-06-21 15:45:00 |

Trạng thái đang dùng:

| Nhóm | Cột | Giá trị trong dữ liệu |
| --- | --- | --- |
| Dự án | `DU_AN.TrangThaiDuAn` | `DangThucHien`: 10 |
| Công việc | `CONG_VIEC.TrangThaiCongViec` | `HoanThanh`: 20, `DangThucHien`: 42 |
| Chi tiết công việc | `CT_CONG_VIEC.TrangThaiCTCV` | `HoanThanh`: 50, `DangThucHien`: 52, `BiCanCan`: 21 |
| Báo cáo tiến độ | `TIEN_DO_CONG_VIEC.TrangThaiTienDo` | `DaDuyet`: 430, `TuChoi`: 71, `YeuCauBoSung`: 91 |
| Đề xuất công việc | `DE_XUAT_CONG_VIEC.TrangThaiCongViecDeXuat` | `DaDuyet`: 62, `TuChoi`: 10 |
| Đề xuất ngân sách | `DE_XUAT_NGAN_SACH.TrangThaiDeXuat` | `DaDuyet`: 10, `TuChoi`: 4 |
| Ngân sách | `NGAN_SACH.TrangThaiNganSach` | `DaDuyet`: 10 |

Đối chiếu với `Constants/TrangThai.cs`: các mã trạng thái trên là mã chuẩn, không phải tên hiển thị. Không phát hiện trạng thái kiểu `Đã duyệt` thay cho `DaDuyet` trong các nhóm chính.

Các nguyên tắc thời gian:

| Nguyên tắc | Kết quả |
| --- | --- |
| Ngày bắt đầu dự án không sau ngày kết thúc | Đạt: 10 dự án bắt đầu 2026-06-17, kết thúc kế hoạch 2026-06-20 |
| Ngày đề xuất trước ngày duyệt | Đạt với các dòng có ngày duyệt trong `DE_XUAT_CONG_VIEC` và `DE_XUAT_NGAN_SACH` |
| Báo cáo tiến độ tạo trước khi duyệt/từ chối/yêu cầu bổ sung | Đạt theo mẫu `ThoiGianCapNhat` trước `ThoiGianDuyet` |
| Báo cáo gửi lại sau từ chối/yêu cầu bổ sung | Có nhiều báo cáo theo cùng chi tiết; cần kiểm tra từng chuỗi nếu cần tuyệt đối, nhưng mẫu tổng thể có báo cáo sau trạng thái `TuChoi`/`YeuCauBoSung` |
| Công việc nằm trong khoảng dự án | Một số `NgayKetThucCVThucTe` là 2026-06-21, sau `DU_AN.NgayKetThucDuAn` 2026-06-20; đây là tín hiệu trễ |
| Không tạo dữ liệu tương lai | Không đạt nếu lấy ngày hiện tại là 2026-06-20: phát hiện 523 mốc ngày ở 2026-06-21 |

## 10. Phân tích đề xuất, ngân sách và chi phí

| Mã dự án | Tổng NS đề xuất | NS được duyệt | NS active | Chi phí dự kiến từ đề xuất CV đã duyệt | Chi phí thực tế | Còn lại | Đề xuất NS | Đề xuất CV |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- | --- |
| 1 | 85,000,000 | 85,000,000 | 85,000,000 | 44,000,000 | 44,000,000 | 41,000,000 | `DaDuyet`: 1 | `DaDuyet`: 6, `TuChoi`: 1 |
| 2 | 148,000,000 | 58,000,000 | 58,000,000 | 30,000,000 | 30,000,000 | 28,000,000 | `DaDuyet`: 1, `TuChoi`: 1 | `DaDuyet`: 6, `TuChoi`: 1 |
| 3 | 225,000,000 | 105,000,000 | 105,000,000 | 51,000,000 | 51,000,000 | 54,000,000 | `DaDuyet`: 1, `TuChoi`: 1 | `DaDuyet`: 6, `TuChoi`: 1 |
| 4 | 65,000,000 | 65,000,000 | 65,000,000 | 47,000,000 | 47,000,000 | 18,000,000 | `DaDuyet`: 1 | `DaDuyet`: 6, `TuChoi`: 1 |
| 5 | 75,000,000 | 75,000,000 | 75,000,000 | 45,000,000 | 45,000,000 | 30,000,000 | `DaDuyet`: 1 | `DaDuyet`: 6, `TuChoi`: 1 |
| 6 | 145,000,000 | 145,000,000 | 145,000,000 | 72,000,000 | 72,000,000 | 73,000,000 | `DaDuyet`: 1 | `DaDuyet`: 8, `TuChoi`: 1 |
| 7 | 192,000,000 | 82,000,000 | 82,000,000 | 48,000,000 | 48,000,000 | 34,000,000 | `DaDuyet`: 1, `TuChoi`: 1 | `DaDuyet`: 6, `TuChoi`: 1 |
| 8 | 70,000,000 | 70,000,000 | 70,000,000 | 41,000,000 | 41,000,000 | 29,000,000 | `DaDuyet`: 1 | `DaDuyet`: 6, `TuChoi`: 1 |
| 9 | 218,000,000 | 88,000,000 | 88,000,000 | 43,000,000 | 43,000,000 | 45,000,000 | `DaDuyet`: 1, `TuChoi`: 1 | `DaDuyet`: 6, `TuChoi`: 1 |
| 10 | 78,000,000 | 78,000,000 | 78,000,000 | 40,000,000 | 40,000,000 | 38,000,000 | `DaDuyet`: 1 | `DaDuyet`: 6, `TuChoi`: 1 |

Không dự án nào vượt ngân sách theo dữ liệu `NGAN_SACH.NganSach` active và `CHI_PHI.SoTienDaChi`. Vì vậy lớp nguyên nhân `Vượt ngân sách` chưa có tín hiệu tốt trong 10 dự án này, dù `DM_NGUYEN_NHAN` có danh mục đó.

Thời gian duyệt trung bình tính từ dữ liệu:

| Nhóm | Nhận xét |
| --- | --- |
| `DE_XUAT_CONG_VIEC` | Trung bình rất thấp, khoảng 0.00-0.01 ngày vì đề xuất và duyệt cách nhau theo giờ |
| `DE_XUAT_NGAN_SACH` | Trung bình rất thấp, khoảng 0.00 ngày; các đề xuất bị từ chối cũng có `NgayDuyet` |
| Rủi ro | Nếu muốn tạo nguyên nhân `Quy trình xử lý chậm`, cần kéo dài thời gian duyệt hoặc thêm nhiều chờ duyệt thật; dữ liệu hiện tại chưa tạo tín hiệu mạnh |

## 11. Phân tích báo cáo tiến độ

| Mã dự án | Tổng báo cáo | `DaDuyet` | `TuChoi` | `YeuCauBoSung` | Có file minh chứng | Không có file | Báo cáo gần nhất |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| 1 | 58 | 42 | 7 | 9 | 58 | 0 | 2026-06-21 16:23 |
| 2 | 58 | 42 | 7 | 9 | 58 | 0 | 2026-06-21 16:37 |
| 3 | 58 | 42 | 7 | 9 | 58 | 0 | 2026-06-21 16:23 |
| 4 | 58 | 42 | 7 | 9 | 58 | 0 | 2026-06-21 16:37 |
| 5 | 58 | 42 | 7 | 9 | 58 | 0 | 2026-06-21 16:37 |
| 6 | 70 | 52 | 8 | 10 | 70 | 0 | 2026-06-21 16:37 |
| 7 | 58 | 42 | 7 | 9 | 58 | 0 | 2026-06-21 16:37 |
| 8 | 58 | 42 | 7 | 9 | 58 | 0 | 2026-06-21 16:37 |
| 9 | 58 | 42 | 7 | 9 | 58 | 0 | 2026-06-21 16:37 |
| 10 | 58 | 42 | 7 | 9 | 58 | 0 | 2026-06-21 16:37 |

Nhận xét:

| Nội dung | Kết quả |
| --- | --- |
| Một chi tiết có thể có nhiều báo cáo | Có, tổng 592 báo cáo cho 123 chi tiết |
| Báo cáo bị từ chối/yêu cầu bổ sung có gửi lại | Có dấu hiệu có gửi lại vì mỗi chi tiết có chuỗi nhiều báo cáo; cần kiểm tra theo từng `MaChiTietCV` nếu viết test tự động |
| Chỉ `DaDuyet` mới tác động trạng thái thật | Theo `TienDoCongViecService`, chỉ khi xử lý duyệt `DaDuyet` mới cập nhật `CT_CONG_VIEC.TrangThaiCTCV`; dữ liệu SQL cuối cùng phải đồng bộ theo trạng thái đã duyệt mới nhất |
| Báo cáo sau dự án hoàn thành/khóa | Dự án chưa `HoanThanh` hoặc `Archived`, nhưng nhiều báo cáo sau `NgayKetThucDuAn` và sau ngày hiện tại 2026-06-20 |
| File minh chứng | Tất cả 592 báo cáo có `FILE_TIEN_DO_CONG_VIEC`, không có trường hợp thiếu minh chứng |

## 12. Phân tích dữ liệu AI và 22 feature

`data7.sql` không có bản ghi `AI_DATASET`, `AI_KET_QUA`, `AI_NGUYEN_NHAN`. Do đó không có giá trị AI đã được lưu để đối chiếu trực tiếp. Bảng dưới mô tả cách `AiDatasetService` tổng hợp từ source và đánh giá dữ liệu hiện tại có đủ nguồn nghiệp vụ hay không.

| Feature | Bảng nguồn | Quy tắc tổng hợp theo source | Dữ liệu hiện tại có đủ không | Nhận xét |
| --- | --- | --- | --- | --- |
| `SoNhanVienDuAn` | `NHAN_VIEN_DU_AN` | Đếm distinct `MaNguoiDung` theo `MaDuAn` | Có | 5-7 nhân viên/dự án |
| `TongSoCongViec` | `CONG_VIEC` + `DANH_MUC_CONG_VIEC` | Đếm công việc chưa xóa mềm theo dự án | Có | 6 hoặc 8 |
| `SoCongViecTre` | `CONG_VIEC` | Nếu hoàn thành: `NgayKetThucCVThucTe > NgayKetThucCVDuKien`; nếu chưa hoàn thành: `DateTime.Today > NgayKetThucCVDuKien` | Có | 2-4 |
| `TyLeCongViecTre` | `CONG_VIEC` | `SoCongViecTre / TongSoCongViec * 100`, làm tròn 2 chữ số | Có | 33.33%-66.67% |
| `ChiPhiDuKien` | `NGAN_SACH` | Tổng `NganSach` active, `DaDuyet` | Có | Không phải tổng đề xuất công việc |
| `ChiPhiThucTe` | `CHI_PHI` + `NGAN_SACH` | Tổng `SoTienDaChi` theo ngân sách của dự án | Có | Tất cả thấp hơn ngân sách |
| `ChenhLechChiPhi` | `NGAN_SACH`, `CHI_PHI` | `ChiPhiThucTe - ChiPhiDuKien` | Có | Luôn âm, không có vượt ngân sách |
| `SoLanThayDoiNhanSu` | `NHAT_KY_PHU_TRACH_DU_AN` | Đếm hành động thay đổi nhân sự theo hàm `LaHanhDongThayDoiNhanSu` | Có | Dữ liệu nhật ký nhiều, nhưng cần kiểm tra chuỗi hành động nếu muốn exact |
| `SoLanThayDoiQuanLy` | `YEU_CAU_DOI_QUAN_LY` | Đếm yêu cầu `DaDuyet`, có `NgayDuyet`, `MaQuanLyHienTai != MaQuanLyDeXuat` | Có | Dự án 3 và 6 có 1 lần |
| `SoNgayTreTienDo` | `DU_AN`, `CONG_VIEC` | Nếu dự án hoàn thành có ngày thực tế thì so với `NgayKetThucDuAn`; nếu chưa hoàn thành lấy max ngày trễ công việc | Có | 1-2 |
| `SoDeXuatCongViecChoDuyet` | `DE_XUAT_CONG_VIEC` | Đếm trạng thái `ChoDuyet` | Có | Hiện bằng 0 toàn bộ |
| `SoDeXuatCongViecBiTuChoi` | `DE_XUAT_CONG_VIEC` | Đếm trạng thái `TuChoi` | Có | Mỗi dự án có 1 |
| `ThoiGianDuyetCongViecTrungBinh` | `DE_XUAT_CONG_VIEC` | Trung bình ngày từ `NgayDeXuatCongViec` đến `NgayDuyetDeXuatCongViec` | Có | Gần 0 ngày |
| `SoDeXuatNganSachChoDuyet` | `DE_XUAT_NGAN_SACH` | Đếm trạng thái `ChoDuyet` | Có | Hiện bằng 0 toàn bộ |
| `SoDeXuatNganSachBiTuChoi` | `DE_XUAT_NGAN_SACH` | Đếm trạng thái `TuChoi` | Có | Dự án 2, 3, 7, 9 có 1 |
| `ThoiGianDuyetNganSachTrungBinh` | `DE_XUAT_NGAN_SACH` | Trung bình ngày từ `NgayDeXuat` đến `NgayDuyet` | Có | Gần 0 ngày |
| `SoBaoCaoTienDoChoDuyet` | `TIEN_DO_CONG_VIEC` | Đếm trạng thái `ChoDuyet` | Có | Hiện bằng 0 toàn bộ |
| `SoBaoCaoTienDoBiTuChoi` | `TIEN_DO_CONG_VIEC` | Đếm trạng thái `TuChoi` | Có | 7 hoặc 8 |
| `SoBaoCaoTienDoYeuCauBoSung` | `TIEN_DO_CONG_VIEC` | Đếm trạng thái `YeuCauBoSung` | Có | 9 hoặc 10 |
| `TyLeBaoCaoTienDoBiTuChoi` | `TIEN_DO_CONG_VIEC` | `TuChoi / tổng báo cáo * 100` | Có | 11.43%-12.07% |
| `SoLanCapNhatTienDo` | `TIEN_DO_CONG_VIEC` | Tổng báo cáo tiến độ | Có | 58 hoặc 70 |
| `SoNgayChamCapNhatTienDo` | `TIEN_DO_CONG_VIEC`, `DU_AN` | Max 0 giữa `NgayKetThucDuAn` và lần cập nhật tiến độ gần nhất | Có nhưng hiện = 0 | Vì báo cáo mới nhất sau `NgayKetThucDuAn` |

Giá trị feature dự kiến nếu tổng hợp theo source:

| Mã dự án | NV | CV | CV trễ | Tỷ lệ trễ | CP dự kiến | CP thực tế | Chênh lệch | Đổi NSự | Đổi QL | Ngày trễ | DXCV chờ | DXCV từ chối | DXNS chờ | DXNS từ chối | TĐ chờ | TĐ từ chối | TĐ bổ sung | Tỷ lệ TĐ từ chối | Lần TĐ | Chậm TĐ |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | 6 | 6 | 2 | 33.33 | 85,000,000 | 44,000,000 | -41,000,000 | 6 | 0 | 2 | 0 | 1 | 0 | 0 | 0 | 7 | 9 | 12.07 | 58 | 0 |
| 2 | 5 | 6 | 2 | 33.33 | 58,000,000 | 30,000,000 | -28,000,000 | 5 | 0 | 2 | 0 | 1 | 0 | 1 | 0 | 7 | 9 | 12.07 | 58 | 0 |
| 3 | 6 | 6 | 2 | 33.33 | 105,000,000 | 51,000,000 | -54,000,000 | 6 | 1 | 2 | 0 | 1 | 0 | 1 | 0 | 7 | 9 | 12.07 | 58 | 0 |
| 4 | 5 | 6 | 3 | 50.00 | 65,000,000 | 47,000,000 | -18,000,000 | 5 | 0 | 2 | 0 | 1 | 0 | 0 | 0 | 7 | 9 | 12.07 | 58 | 0 |
| 5 | 6 | 6 | 4 | 66.67 | 75,000,000 | 45,000,000 | -30,000,000 | 6 | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 7 | 9 | 12.07 | 58 | 0 |
| 6 | 7 | 8 | 3 | 37.50 | 145,000,000 | 72,000,000 | -73,000,000 | 8 | 1 | 1 | 0 | 1 | 0 | 0 | 0 | 8 | 10 | 11.43 | 70 | 0 |
| 7 | 5 | 6 | 3 | 50.00 | 82,000,000 | 48,000,000 | -34,000,000 | 5 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 7 | 9 | 12.07 | 58 | 0 |
| 8 | 6 | 6 | 4 | 66.67 | 70,000,000 | 41,000,000 | -29,000,000 | 6 | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 7 | 9 | 12.07 | 58 | 0 |
| 9 | 5 | 6 | 4 | 66.67 | 88,000,000 | 43,000,000 | -45,000,000 | 6 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 7 | 9 | 12.07 | 58 | 0 |
| 10 | 6 | 6 | 4 | 66.67 | 78,000,000 | 40,000,000 | -38,000,000 | 6 | 0 | 1 | 0 | 1 | 0 | 0 | 0 | 7 | 9 | 12.07 | 58 | 0 |

Điều kiện train:

| Điều kiện | Kết quả với `data7.sql` |
| --- | --- |
| `LaDuAnTre = 1` | Dự kiến có thể đạt nếu chạy tổng hợp với dữ liệu hiện tại |
| Có `MaDMNguyenNhan` hợp lệ | Chưa đạt, vì không có `AI_NGUYEN_NHAN` để đồng bộ nhãn |
| Có đủ 22 feature | Nguồn nghiệp vụ đủ để tính, nhưng chưa có dòng `AI_DATASET` |
| Không dùng `AI_KET_QUA` làm nhãn train | Đúng theo source; file cũng không có `AI_KET_QUA` |
| Nhãn lấy từ `AI_NGUYEN_NHAN` | Chưa đạt, vì bảng này không có dữ liệu |

## 13. Phân bố nguyên nhân trễ

`DM_NGUYEN_NHAN` có 10 danh mục chuẩn, nhưng `AI_NGUYEN_NHAN` không có dòng nào. Vì vậy phân bố nhãn train hiện tại là 0 cho tất cả lớp.

| Mã nguyên nhân | Tên nguyên nhân | Số dự án hiện tại | Danh sách dự án |
| ---: | --- | ---: | --- |
| 1 | Thiếu nhân sự | 0 | Chưa có |
| 2 | Thay đổi yêu cầu liên tục | 0 | Chưa có |
| 3 | Quy trình xử lý chậm | 0 | Chưa có |
| 4 | Vượt ngân sách | 0 | Chưa có |
| 5 | Rủi ro kỹ thuật | 0 | Chưa có |
| 6 | Phối hợp công việc chưa tốt | 0 | Chưa có |
| 7 | Thông tin đầu vào chưa đầy đủ | 0 | Chưa có |
| 8 | Ước lượng thời gian chưa chính xác | 0 | Chưa có |
| 9 | Tiến độ cập nhật không đầy đủ | 0 | Chưa có |
| 10 | Khác | 0 | Chưa có |

Phân tích:

| Nội dung | Nhận xét |
| --- | --- |
| Cân bằng lớp | Chưa có lớp nào có nhãn manager xác nhận |
| Nguyên nhân chưa có dữ liệu | Tất cả 10 lớp chưa có `AI_NGUYEN_NHAN` |
| Tên nguyên nhân cũ | `DM_NGUYEN_NHAN` trong file có 10 tên chuẩn theo docs AI; không thấy tên cũ như `Chậm phê duyệt` |
| Dự án cùng nguyên nhân có feature phù hợp không | Chưa xác định vì chưa có nhãn thật; chỉ có `DU_AN.GhiChuDuAn` dạng mô tả tự do |
| Có nên thêm lớp `Khác` | Nên hạn chế; `Khác` chỉ dùng khi không thể ánh xạ 9 lớp chính |

## 14. Đánh giá mức độ đa dạng dữ liệu

| Khía cạnh | Đánh giá |
| --- | --- |
| Tên dự án | Khá đa dạng theo miền nghiệp vụ: bán hàng, cổng nội bộ, kho, y tế, chăm sóc khách hàng, đào tạo, AI, bảo trì, rủi ro, nhân sự |
| Thời gian dự án | Quá giống nhau: tất cả bắt đầu 2026-06-17 và kết thúc 2026-06-20 |
| Số công việc | Quá giống nhau: 9/10 dự án có 6 công việc; dự án 6 có 8 |
| Số chi tiết | Quá giống nhau: 9/10 có 12 chi tiết; dự án 6 có 15 |
| Số nhân viên | Có biến thiên nhẹ 5-7 |
| Team phụ trách | Lặp lại 3 team trên toàn bộ dữ liệu |
| Manager | 3 manager, trong đó Phạm Quốc Bảo quản lý 5/10 dự án |
| Ngân sách | Có biến thiên 58-145 triệu được duyệt, tổng đề xuất có cả dòng bị từ chối |
| Chi phí | Không dự án nào vượt ngân sách; thiếu mẫu `Vượt ngân sách` |
| Số ngày trễ | Chỉ 1-2 ngày, chưa đa dạng |
| Đổi nhân sự | Số nhật ký phụ trách khá đều, 5-8 |
| Đổi quản lý | Chỉ dự án 3 và 6 có đổi quản lý đã duyệt |
| Tỷ lệ công việc trễ | Có 33.33%, 37.50%, 50.00%, 66.67% |
| Đề xuất bị từ chối | Mỗi dự án đều có đúng 1 đề xuất công việc bị từ chối; chưa tự nhiên |
| Báo cáo bị từ chối/bổ sung | Gần như lặp khuôn 42/7/9 cho 9 dự án, 52/8/10 cho dự án 6 |
| Nguyên nhân trễ | Chưa có nhãn chuẩn; chỉ có ghi chú tự do |

Khi tạo 10 dự án mới, cần đa dạng hóa mạnh hơn về ngày bắt đầu/kết thúc, độ dài dự án, số lượng công việc, số ngày trễ, tỷ lệ báo cáo bị từ chối, tình trạng thiếu file, thời gian duyệt, ngân sách vượt/không vượt và nhãn `AI_NGUYEN_NHAN`.

## 15. Lỗi, bất thường và rủi ro

| Mức độ | Vị trí hoặc đoạn SQL | Vấn đề | Ảnh hưởng | Hướng xử lý đề xuất |
| --- | --- | --- | --- | --- |
| Nghiêm trọng | Toàn file, các bảng identity | Script ghi cứng ID và dùng `IDENTITY_INSERT`, không kiểm tra tồn tại | Chạy lại hoặc chạy trên DB có dữ liệu dễ trùng khóa chính | File bổ sung mới phải dùng biến bảng map, `SCOPE_IDENTITY()` hoặc truy vấn khóa tự nhiên duy nhất |
| Nghiêm trọng | `AI_DATASET`, `AI_NGUYEN_NHAN`, `AI_KET_QUA` | Không có dòng AI nào cho 10 dự án | Chưa thể train vì thiếu `AI_DATASET.MaDMNguyenNhan` từ manager xác nhận | Sau khi tổng hợp dataset phải tạo/xác nhận `AI_NGUYEN_NHAN`, rồi đồng bộ vào `AI_DATASET`; không dùng `AI_KET_QUA` làm nhãn |
| Nghiêm trọng | Nhiều bảng ngày giờ, ví dụ `CONG_VIEC` dòng 1250 trở đi, `TIEN_DO_CONG_VIEC`, `FILE_TIEN_DO_CONG_VIEC` | Có 523 mốc ngày sau 2026-06-20, trong khi ngày hiện tại của phiên là 2026-06-20 | Dữ liệu tương lai làm báo cáo/hoàn thành có vẻ xảy ra trước khi đến ngày đó; rule AI dùng `DateTime.Today` nên kết quả phụ thuộc thời điểm chạy | Với dataset mới, chọn mốc quá khứ hoặc cố định ngày hiện tại giả lập; tránh dùng ngày tương lai nếu không ghi rõ chủ ý |
| Trung bình | `DU_AN` | Tất cả 10 dự án là `DangThucHien`, `NgayHoanThanhThucTeDuAn` rỗng | Không có mẫu dự án `HoanThanh` trễ, trong khi rule tổng hợp AI mặc định chỉ tổng hợp trạng thái chính thức nếu bật `chiNhanTrangThaiChinhThuc` | Tạo thêm cả dự án `HoanThanh` trễ có `NgayHoanThanhThucTeDuAn` sau kế hoạch |
| Trung bình | `DE_XUAT_CONG_VIEC`, `DE_XUAT_NGAN_SACH`, `TIEN_DO_CONG_VIEC` | Không có trạng thái `ChoDuyet` trong dữ liệu hiện tại | Các feature `So*ChoDuyet` luôn 0, thiếu tín hiệu backlog | Tạo một số dự án còn đề xuất/báo cáo chờ duyệt nếu hợp workflow |
| Trung bình | `TIEN_DO_CONG_VIEC` | Mẫu tiến độ lặp: 9 dự án đều 58 báo cáo với 42/7/9 | Dữ liệu dễ bị học theo khuôn thay vì tín hiệu nghiệp vụ | Đa dạng số lần cập nhật, tỷ lệ từ chối, yêu cầu bổ sung và thiếu minh chứng |
| Trung bình | `CHI_PHI`, `NGAN_SACH` | Không có dự án vượt ngân sách | Lớp `Vượt ngân sách` không có feature tương ứng | Nếu tạo nhãn `Vượt ngân sách`, phải có `ChiPhiThucTe > ChiPhiDuKien` |
| Trung bình | `DU_AN.GhiChuDuAn` | Nguyên nhân trễ chỉ nằm ở ghi chú tự do | Không phải ground truth cho train | Phải tạo `AI_NGUYEN_NHAN` theo manager xác nhận |
| Trung bình | `DE_XUAT_CONG_VIEC`, `DE_XUAT_NGAN_SACH` | Thời gian duyệt trung bình gần 0 ngày | Không thể hiện rõ nguyên nhân quy trình chậm | Kéo dài `NgayDuyet` so với `NgayDeXuat` cho lớp quy trình |
| Trung bình | `DANH_GIA_DU_AN`, `DANH_GIA_NHAN_VIEN` | Không có dữ liệu đánh giá | Thiếu dữ liệu hậu nghiệm để phân tích chất lượng dự án/nhân sự | Có thể thêm nếu kịch bản cần, nhưng không bắt buộc cho 22 feature AI hiện tại |
| Nhẹ | Toàn file | File là UTF-16 | Không lỗi SQL Server, nhưng dễ đọc sai bằng tool mặc định UTF-8 | Khi phân tích hoặc sinh tài liệu cần xác định encoding rõ ràng |
| Nhẹ | `DE_XUAT_CONG_VIEC` | `MaDeXuatCV` bắt đầu từ 3, thiếu 1-2 | Không sai nếu DB export có lịch sử, nhưng là dấu hiệu phụ thuộc ID hiện tại | Script mới không nên giả định ID liên tục |
| Nhẹ | `FILE_TIEN_DO_CONG_VIEC` | Mọi báo cáo đều có file | Thiếu kịch bản không có minh chứng | Tạo một số báo cáo không file nếu workflow cho phép |
| Nhẹ | `SoNgayChamCapNhatTienDo` dự kiến | Bằng 0 vì báo cáo mới nhất sau ngày kết thúc dự án | Không hỗ trợ lớp `Tiến độ cập nhật không đầy đủ` | Với lớp này, để lần cập nhật gần nhất trước `NgayKetThucDuAn` nhiều ngày |
| Lưu ý | `DM_NGUYEN_NHAN` | Có đủ 10 tên chuẩn | Tốt, nhưng chưa được dùng làm nhãn | Giữ nguyên danh mục chuẩn |
| Lưu ý | `NHAN_VIEN_DU_AN`, `PHAN_CONG_*` | Không phát hiện phân công ngoài dự án hoặc trùng khóa | Điểm tốt cần duy trì | Tiếp tục kiểm tra tự động khi sinh SQL mới |
| Lưu ý | `ALTER TABLE ... ADD CONSTRAINT` sau insert | Thứ tự export có thể không phù hợp seed bổ sung | Script bổ sung chạy trên DB có constraint sẽ cần thứ tự cha-con đúng | Viết thứ tự insert an toàn thay vì bắt chước thứ tự export |

Tổng rủi ro đã phân loại: 3 nghiêm trọng, 6 trung bình, 4 nhẹ, 3 lưu ý.

## 16. Quy tắc cần tuân thủ khi tạo 10 dự án mới

Bảng bắt buộc nên có cho mỗi dự án mới:

| Nhóm | Bảng |
| --- | --- |
| Nền dự án | `DU_AN`, `DANH_MUC_CONG_VIEC`, `CONG_VIEC`, `CT_CONG_VIEC` |
| Nhân sự | `NHAN_VIEN_DU_AN`, `TEAM_DU_AN`, `PHAN_CONG_CONG_VIEC`, `PHAN_CONG_CT_CONG_VIEC` |
| Ngân sách/chi phí | `DE_XUAT_NGAN_SACH`, `NGAN_SACH`, `DE_XUAT_CONG_VIEC`, `CHI_PHI` |
| Tiến độ | `TIEN_DO_CONG_VIEC`, tùy kịch bản `FILE_TIEN_DO_CONG_VIEC` |
| Nhật ký | `NHAT_KY_QUAN_LY_DU_AN`, `NHAT_KY_PHU_TRACH_DU_AN`, `NHAT_KY_PHAN_CONG_CONG_VIEC`, `NHAT_KY_PHAN_CONG_CT_CONG_VIEC`, `NHAT_KY_NGAN_SACH`, `NHAT_KY_CHI_PHI` |
| Chat | `PHONG_CHAT`, `THANH_VIEN_PHONG_CHAT` |
| AI train | `AI_DATASET`, `AI_NGUYEN_NHAN` sau khi xác định feature và manager xác nhận |

Bảng tùy kịch bản:

| Bảng | Khi nên tạo |
| --- | --- |
| `YEU_CAU_DOI_QUAN_LY` | Khi muốn feature `SoLanThayDoiQuanLy` > 0 |
| `DANH_GIA_DU_AN`, `DANH_GIA_NHAN_VIEN`, các bảng chi tiết đánh giá | Khi cần dữ liệu đánh giá sau dự án |
| `AI_KET_QUA` | Khi mô phỏng kết quả dự đoán đã chạy; không dùng làm nhãn train |
| `FILE_CONG_VIEC`, `FILE_CT_CONG_VIEC`, `FILE_DU_AN` | Khi cần minh chứng ngoài báo cáo tiến độ |

Thứ tự insert an toàn cho script bổ sung:

1. Tra cứu hoặc tạo danh mục nền đã có: `LOAI_DU_AN`, `MUC_DO_UU_TIEN`, `DM_NGUYEN_NHAN`, `NGUOI_DUNG`, `TEAM`.
2. Insert `DU_AN`, lưu `MaDuAn` bằng `SCOPE_IDENTITY()` hoặc `OUTPUT INSERTED.MaDuAn`.
3. Insert `TEAM_DU_AN`, `NHAN_VIEN_DU_AN`, `NHAT_KY_DU_AN`, `NHAT_KY_PHU_TRACH_DU_AN`.
4. Insert `DANH_MUC_CONG_VIEC`, lưu map tên danh mục -> `MaDanhMucCV`.
5. Insert `DE_XUAT_NGAN_SACH`, duyệt hoặc từ chối theo kịch bản.
6. Insert `NGAN_SACH` cho đề xuất được duyệt, rồi `NHAT_KY_NGAN_SACH`.
7. Insert `DE_XUAT_CONG_VIEC`, duyệt/từ chối theo kịch bản.
8. Insert `CONG_VIEC` cho đề xuất được duyệt, lưu `MaCongViec`.
9. Insert `CHI_PHI` và `NHAT_KY_CHI_PHI` cho công việc có chi phí.
10. Insert `CT_CONG_VIEC`, lưu `MaChiTietCV`.
11. Insert `PHAN_CONG_CONG_VIEC`, `PHAN_CONG_CT_CONG_VIEC` và nhật ký phân công.
12. Insert chuỗi `TIEN_DO_CONG_VIEC`, chỉ cập nhật trạng thái thật theo báo cáo `DaDuyet`.
13. Insert file minh chứng nếu có.
14. Insert `PHONG_CHAT`, `THANH_VIEN_PHONG_CHAT`.
15. Sau khi dữ liệu nghiệp vụ ổn, tạo hoặc cập nhật `AI_DATASET` đủ 22 feature.
16. Insert `AI_NGUYEN_NHAN` cho manager xác nhận; đồng bộ `AI_DATASET.MaDMNguyenNhan`.

Nguyên tắc ID và chạy lại:

| Quy tắc | Cách làm |
| --- | --- |
| Tránh ghi cứng identity | Dùng biến `@MaDuAn`, `@MaCongViec`, `@MaChiTietCV` nhận từ `SCOPE_IDENTITY()` hoặc `OUTPUT` |
| Tránh tìm bằng tên không duy nhất | Đặt mã tự nhiên/tên dự án có tiền tố batch duy nhất, ví dụ `DATA8-01 - ...` |
| Script chạy lại | Bọc mỗi dự án trong kiểm tra `IF NOT EXISTS (SELECT 1 FROM DU_AN WHERE TenDuAn = N'...')` |
| Giới hạn phạm vi | Mọi `UPDATE`/`DELETE` nếu có phải join qua danh sách `@DuAnMoi` |
| Không đụng dữ liệu cũ | Không cập nhật `DM_NGUYEN_NHAN`, user, role, permission nếu chỉ thêm dự án |

Quy tắc thời gian:

| Nội dung | Quy tắc |
| --- | --- |
| Mốc dự án | Nên dùng mốc quá khứ so với ngày chạy, hoặc ghi rõ ngày giả lập |
| Đề xuất | `NgayDeXuat` trước `NgayDuyet`; trạng thái `ChoDuyet` thì `NgayDuyet` rỗng |
| Báo cáo tiến độ | `ThoiGianCapNhat` trước `ThoiGianDuyet`; gửi lại phải sau bản bị `TuChoi` hoặc `YeuCauBoSung` |
| Hoàn thành trễ | `NgayKetThucCVThucTe` hoặc `NgayHoanThanhThucTeDuAn` phải sau kế hoạch |
| Chậm cập nhật tiến độ | Để lần `ThoiGianCapNhat` gần nhất trước `NgayKetThucDuAn` nhiều ngày |
| Không dùng tương lai | Không tạo ngày sau ngày chạy nếu không có chủ ý kiểm thử |

Tạo tín hiệu feature phù hợp nguyên nhân:

| Nguyên nhân | Tín hiệu nên tạo |
| --- | --- |
| `Thiếu nhân sự` | Ít nhân viên, nhiều nhật ký thêm/rút người, nhiều công việc chưa phân công hoặc trễ do quá tải |
| `Thay đổi yêu cầu liên tục` | Nhiều `DE_XUAT_CONG_VIEC`, nhiều đề xuất bị từ chối, nhiều công việc mới sau khi dự án đã chạy |
| `Quy trình xử lý chậm` | Thời gian duyệt đề xuất/báo cáo dài, nhiều `ChoDuyet`, nhiều `YeuCauBoSung` |
| `Vượt ngân sách` | `ChiPhiThucTe > ChiPhiDuKien`, nhiều đề xuất ngân sách bị từ chối hoặc tăng ngân sách |
| `Rủi ro kỹ thuật` | Công việc kỹ thuật có `BiCanCan`, nhiều báo cáo bị yêu cầu bổ sung, hoàn thành thực tế sau kế hoạch |
| `Phối hợp công việc chưa tốt` | Nhiều team, nhiều phụ thuộc chéo, nhiều báo cáo bị từ chối do thiếu phối hợp |
| `Thông tin đầu vào chưa đầy đủ` | Báo cáo thiếu file, yêu cầu bổ sung tài liệu, chi tiết bị `BiCanCan` |
| `Ước lượng thời gian chưa chính xác` | Nhiều công việc hoàn thành sau `NgayKetThucCVDuKien`, tỷ lệ trễ cao nhưng ít lỗi duyệt/ngân sách |
| `Tiến độ cập nhật không đầy đủ` | `SoNgayChamCapNhatTienDo` cao, ít báo cáo gần cuối dự án, nhiều báo cáo chậm |
| `Khác` | Chỉ dùng khi không thuộc 9 nhóm trên |

## 17. Thông tin cần giữ nguyên

| Nhóm | Thông tin cần giữ |
| --- | --- |
| Danh mục nguyên nhân | Giữ 10 tên chuẩn trong `DM_NGUYEN_NHAN` |
| Mã trạng thái | Dùng mã code trong `Constants/TrangThai.cs`: `ChoDuyet`, `DaDuyet`, `TuChoi`, `YeuCauBoSung`, `DangThucHien`, `HoanThanh`, `BiCanCan`, `ChoXacNhanHoanThanh`, `Archived` |
| Ground truth AI | Nhãn train phải đến từ `AI_NGUYEN_NHAN`, đồng bộ về `AI_DATASET.MaDMNguyenNhan` |
| Boundary FastAPI | FastAPI compute-only, không ghi SQL |
| 22 feature | Giữ đúng tên feature hiện tại |
| Workflow duyệt | Chỉ bản `DaDuyet` mới tạo/cập nhật trạng thái thật hoặc dữ liệu thật |
| Phân công | Người được phân công phải thuộc dự án |
| Ngân sách | Chi phí phải liên kết `NGAN_SACH` và `CONG_VIEC` hợp lệ |

## 18. Thông tin còn chưa xác định

| Nội dung | Trạng thái |
| --- | --- |
| Manager đã xác nhận nguyên nhân nào cho 10 dự án | Chưa xác định từ dữ liệu hiện có vì không có `AI_NGUYEN_NHAN` |
| Giá trị `AI_DATASET` đã lưu | Chưa xác định từ dữ liệu hiện có vì không có `AI_DATASET` |
| Kết quả AI dự đoán | Chưa xác định từ dữ liệu hiện có vì không có `AI_KET_QUA` |
| Dự án đã hoàn thành trễ thật hay chỉ đang trễ | Chưa hoàn thành theo `DU_AN.TrangThaiDuAn`; chỉ có công việc/tiến độ tạo tín hiệu trễ |
| Ý đồ dùng ngày 2026-06-21 | Chưa xác định từ dữ liệu hiện có; có thể là dữ liệu mô phỏng tương lai, nhưng không có chú thích trong SQL |
| Dữ liệu đánh giá sau dự án | Chưa có |
| Mật khẩu/token/user secret | Không đưa vào tài liệu; file có bảng tài khoản nhưng không cần sao chép thông tin nhạy cảm |

## 19. Kết luận

`data7.sql` cung cấp nền dữ liệu vận hành khá dày cho 10 dự án: có dự án, nhân sự, team, danh mục, công việc, chi tiết, phân công, ngân sách, chi phí, đề xuất, báo cáo tiến độ, file minh chứng, nhật ký và chat. Theo rule hiện tại của `AiDatasetService`, cả 10 dự án đều có thể được xác định là trễ nhờ công việc trễ và tỷ lệ công việc trễ.

Tuy nhiên file chưa đủ để train AI nguyên nhân trễ vì thiếu `AI_DATASET` và thiếu nhãn manager xác nhận trong `AI_NGUYEN_NHAN`. Nguyên nhân hiện chỉ nằm trong `DU_AN.GhiChuDuAn`, không phải ground truth. Ngoài ra script không an toàn để chạy lại vì ghi cứng ID và không kiểm tra tồn tại. Khi tạo 10 dự án mới, cần viết script bổ sung theo thứ tự FK an toàn, dùng ID động, thêm nhãn `AI_NGUYEN_NHAN` đúng workflow, tránh dữ liệu tương lai ngoài chủ ý và đa dạng hóa feature theo từng nguyên nhân trễ.
