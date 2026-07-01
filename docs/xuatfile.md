# Phân tích và đề xuất cải thiện chức năng xuất file

Tài liệu này mô tả trạng thái hiện tại (AS-IS) của chức năng xuất file trong source tại thời điểm rà soát và đề xuất hướng cải thiện (TO-BE). Phạm vi của tài liệu chỉ là phân tích; không có source code, cơ sở dữ liệu, schema, migration, quyền hay workflow nào được thay đổi.

## 1. Phạm vi source đã đọc

Đã tìm kiếm toàn bộ dự án bằng các nhóm từ khóa `XuatFile`, `XuatBaoCao`, `Export`, `Download`, `FileResult`, `FileContentResult`, `FileStreamResult`, `Excel`, `EPPlus`, `ClosedXML`, `OpenXml`, `Pdf`, `Csv`, `Workbook`, `Worksheet`, `Content-Disposition` và `ThongKe.XuatFile`. Các nhóm source đã đối chiếu trực tiếp gồm:

- Controller: `DashboardController`, `DuAnController`, `CongViecController`, `ChiTietCongViecController`, `NganSachController`, `NhanSuController`, `TienDoCongViecController`, `DanhGiaDuAnController`, `DanhGiaNhanVienController`, `AiController`, `AiDatasetController`.
- Service nghiệp vụ và interface tương ứng: `IDashboardService`/`DashboardService`, `IDuAnService`/`DuAnService`, `ICongViecService`/`CongViecService`, `IChiTietCongViecService`/`ChiTietCongViecService`, `INganSachService`/`NganSachService`, `INhanSuService`/`NhanSuService`, `ITienDoCongViecService`/`TienDoCongViecService`, `IDanhGiaDuAnService`/`DanhGiaDuAnService`, `IDanhGiaNhanVienService`/`DanhGiaNhanVienService`, `IAiService`/`AiService`, `IAiDatasetService`/`AiDatasetService`.
- Hạ tầng xuất file: `Services/Interfaces/IExportFileService.cs`, `Services/Implementations/ExportFileService.cs`, `Services/Exporting/ExportFileContracts.cs`, `Helpers/ExportSupport.cs`.
- View và ViewModel dùng chung: `Views/Shared/_ExportDropdown.cshtml`, `ViewModels/Common/ExportDropdownViewModel.cs`, các `Index.cshtml`/`Dashboard.cshtml` và item/page ViewModel của từng module.
- Tải tệp có sẵn: `FileDuAnService`, `FileTienDoCongViecService`, `DuAnController.TaiFileDuAn`, `TienDoCongViecController.TaiFileTienDo`.
- Quyền và cấu hình: `Constants/Permissions.cs`, `PermissionDependencyProvider`, `Data/KhoiTaoTaiKhoanMacDinh.cs`, `Services/CauHinhDichVu.cs`, `QuanLyDuAn.csproj`.
- AI metadata: `AiController.ExportMetadata`, `IAiApiService`, `AiApiService`, endpoint FastAPI `/admin/model/export-metadata/{model_file}` và service model liên quan.

Không tìm thấy action dựng báo cáo bằng EPPlus, OpenXml SDK hoặc thư viện PDF khác. Source hiện tại dùng ClosedXML `0.105.0` cho Excel và QuestPDF `2024.12.1` cho PDF. CSV được dựng trực tiếp bằng `StringBuilder` và UTF-8 BOM.

## 2. Tổng quan chức năng xuất file hiện tại

### AS-IS

Hệ thống có 11 màn hình tạo báo cáo qua `IExportFileService`, mỗi màn hình hỗ trợ Excel (`.xlsx`), PDF (`.pdf`) và CSV (`.csv`):

1. Thống kê tổng quan.
2. Danh sách dự án.
3. Danh sách công việc.
4. Chi tiết công việc.
5. Ngân sách.
6. Nhân sự.
7. Tiến độ công việc.
8. Đánh giá dự án.
9. Đánh giá nhân viên.
10. Tổng quan AI.
11. Quản lý AI Dataset.

Tổng đếm chính xác là **11 action `XuatFile`**. Tất cả đều trả file bằng `Controller.File(byte[], contentType, fileName)`.

Ngoài ra còn có:

- `AiController.ExportMetadata`: xuất một file JSON mô tả model, không dùng `IExportFileService`.
- `DuAnController.TaiFileDuAn`: tải lại tệp dự án đã được người dùng tải lên.
- `TienDoCongViecController.TaiFileTienDo`: tải lại tệp minh chứng tiến độ đã có.

Hai action `TaiFile...` là download tệp nguyên bản, không phải báo cáo được hệ thống dựng. Chúng không được đánh giá về bố cục Excel/PDF trong tài liệu này.

### Cơ chế dùng chung

`Views/Shared/_ExportDropdown.cshtml` tạo ba liên kết GET tới action `XuatFile`, lần lượt thêm `format=excel`, `format=pdf`, `format=csv`. `ExportDropdownViewModel.RouteValues` mang bộ lọc hiện tại của màn hình sang action xuất. Không có JavaScript/AJAX dựng báo cáo hoặc tải blob; trình duyệt điều hướng trực tiếp đến URL tải file.

`ExportFileService` chuẩn hóa định dạng:

- `pdf` thành PDF;
- `csv` thành CSV;
- giá trị trống, `excel` hoặc giá trị không nhận diện đều thành Excel.

Tên file AS-IS có dạng `<FileNamePrefix>_yyyy-MM-dd.<ext>`. Tên sheet Excel luôn là `BaoCao`.

## 3. Luồng xử lý thực tế

Luồng chung của 11 báo cáo:

1. Razor View kiểm tra `Permissions.ThongKe.XuatFile`.
2. View tạo `ExportDropdownViewModel`, truyền các giá trị lọc đang có trên `Model`.
3. `_ExportDropdown.cshtml` tạo liên kết GET đến `Controller.XuatFile`.
4. Action kiểm tra quyền, gọi lại service nghiệp vụ với cùng bộ lọc.
5. Với các màn hình phân trang, action truyền `paginate: false` hoặc gọi phương thức `GetAllAsync` để lấy toàn bộ kết quả đã lọc.
6. Service áp dụng soft delete, bộ lọc và phạm vi theo vai trò/người dùng tùy module.
7. Controller chuyển ViewModel thành `ExportFileRequest`, khai báo tiêu đề, thông tin người xuất, mô tả bộ lọc, cột và dòng.
8. `ExportFileService.Export` dựng Excel/PDF/CSV trong bộ nhớ.
9. Controller trả file cho trình duyệt.

Luồng Excel:

- `XLWorkbook` tạo sheet `BaoCao`.
- Dòng 1 là tiêu đề báo cáo; dòng 2-4 là ngày xuất, người xuất, bộ lọc.
- Dòng 6 là tiêu đề cột; dữ liệu bắt đầu ở dòng 7.
- Tất cả cột được `AdjustToContents()`.
- Không có freeze pane, auto filter, wrap text, giới hạn độ rộng, kiểu dữ liệu số/ngày thực, dòng tổng hợp hoặc thiết lập in.

Luồng PDF:

- QuestPDF tạo A4 ngang, lề 24, cỡ chữ mặc định 10.
- Hiển thị tiêu đề, ngày xuất, người xuất, bộ lọc và một bảng các cột có độ rộng bằng nhau.
- Header bảng được khai báo bằng `table.Header`, nên QuestPDF có thể lặp header khi bảng qua trang.
- Không có footer/số trang, font tiếng Việt được chỉ định rõ, chiến lược khổ giấy theo số cột, giới hạn độ dài hoặc căn lề theo kiểu dữ liệu.

Luồng CSV:

- Có bốn dòng metadata, một dòng trống, sau đó là header và dữ liệu.
- Có escape dấu phẩy, dấu nháy kép và xuống dòng.
- Có UTF-8 BOM để tăng khả năng Excel nhận đúng tiếng Việt.
- Dấu phân cách cố định là dấu phẩy; dữ liệu số, ngày, tiền và phần trăm đều đã bị chuyển thành chuỗi.

Luồng JSON metadata:

`Views/Ai/Models` gọi `AiController.ExportMetadata(modelFile, modelType)`. Action kiểm tra `Permissions.AI.Train`, gọi `AiService.LayTrangModelAsync`, serialize `ChiTietModel` bằng `JsonSerializer` với `WriteIndented = true`, rồi trả `metadata-{modelFile}.json` bằng UTF-8. Action không sử dụng endpoint `IAiApiService.ExportMetadataAsync`; endpoint FastAPI tương ứng tồn tại nhưng luồng MVC hiện tại lấy chi tiết model qua `LayTrangModelAsync`.

Luồng tải tệp có sẵn:

- Dự án: View chi tiết dự án -> `DuAnController.TaiFileDuAn` -> `IFileDuAnService.GetDownloadInfoAsync` -> kiểm tra tệp thuộc đúng dự án và `_service.CanAccessAsync(projectId)` -> `PhysicalFile`.
- Tiến độ: `_FileList.cshtml` -> `TienDoCongViecController.TaiFileTienDo` -> `IFileTienDoCongViecService.GetDownloadInfoAsync` -> service kiểm tra quyền xem chi tiết tiến độ hiện tại -> `PhysicalFile`.

## 4. Danh sách chức năng xuất file

| STT | Module | Màn hình | Định dạng | Controller/Action | Service | Permission | Dữ liệu xuất |
| --- | ------ | -------- | --------- | ----------------- | ------- | ---------- | ------------ |
| 1 | Dashboard | `Views/Dashboard/Index.cshtml` | XLSX, PDF, CSV | `DashboardController.XuatFile` | `IDashboardService.GetDashboardAsync`, `IExportFileService.Export` | `ThongKe.XuatFile` | Tổng dự án, công việc, nhân sự, ngân sách, chi phí, tỷ lệ ngân sách, cảnh báo, tối đa 12 dự án cho biểu đồ, top dự án trễ/vượt ngân sách |
| 2 | Dự án | `Views/DuAn/Index.cshtml` | XLSX, PDF, CSV | `DuAnController.XuatFile` | `IDuAnService.GetAllAsync`, `IExportFileService.Export` | `ThongKe.XuatFile` | Mã/tên/loại/quản lý, thời gian, tiến độ, trạng thái, thời hạn, số team/thành viên |
| 3 | Công việc | `Views/CongViec/Index.cshtml` | XLSX, PDF, CSV | `CongViecController.XuatFile` | `ICongViecService.GetPageAsync(..., paginate:false)`, `IExportFileService.Export` | `ThongKe.XuatFile` | Công việc, dự án, danh mục, mức độ, mốc thời gian, tình trạng hạn, số ngày trễ, chi phí, trạng thái |
| 4 | Chi tiết công việc | `Views/ChiTietCongViec/Index.cshtml` | XLSX, PDF, CSV | `ChiTietCongViecController.XuatFile` | `IChiTietCongViecService.GetPageAsync(..., paginate:false)`, `IExportFileService.Export` | `ThongKe.XuatFile`; service còn xác thực khả năng truy cập công việc cha | Mã/tên/nội dung chi tiết, ngày bắt đầu/hoàn thành, trạng thái |
| 5 | Ngân sách | `Views/NganSach/Index.cshtml` | XLSX, PDF, CSV | `NganSachController.XuatFile` | `INganSachService.GetPageAsync(..., paginate:false)`, `IExportFileService.Export` | `ThongKe.XuatFile` | Mã, dự án, version, số tiền, ngày duyệt/cập nhật, trạng thái, người đề xuất/duyệt |
| 6 | Nhân sự | `Views/NhanSu/Index.cshtml` | XLSX, PDF, CSV | `NhanSuController.XuatFile` | `INhanSuService.GetAllAsync`, `IExportFileService.Export` | Chỉ `ThongKe.XuatFile` tại action | Mã, họ tên, chức danh, điện thoại, ngày sinh, username, email, trạng thái tài khoản |
| 7 | Tiến độ công việc | `Views/TienDoCongViec/Index.cshtml` | XLSX, PDF, CSV | `TienDoCongViecController.XuatFile` | `ITienDoCongViecService.GetPageAsync(..., paginate:false)`, `IExportFileService.Export` | `ThongKe.XuatFile`; service áp dụng scope tiến độ | Dự án/công việc/chi tiết, người thực hiện, phần trăm, trạng thái, báo cáo gần nhất, trạng thái duyệt |
| 8 | Đánh giá dự án | `Views/DanhGiaDuAn/Index.cshtml` | XLSX, PDF, CSV | `DanhGiaDuAnController.XuatFile` | `IDanhGiaDuAnService.GetPageAsync(..., paginate:false)`, `IExportFileService.Export` | `ThongKe.XuatFile` và service yêu cầu `DanhGiaDuAn.Xem` | Dự án, quản lý, trạng thái, số việc/trễ, trạng thái đánh giá, điểm, xếp loại, ngày/người đánh giá, người duyệt |
| 9 | Đánh giá nhân viên | `Views/DanhGiaNhanVien/Index.cshtml` | XLSX, PDF, CSV | `DanhGiaNhanVienController.XuatFile` | `IDanhGiaNhanVienService.GetPageAsync(..., paginate:false)`, `IExportFileService.Export` | `ThongKe.XuatFile` và service yêu cầu `DanhGiaNhanVien.Xem` | Dự án, nhân viên, vai trò, trạng thái đánh giá, điểm, xếp loại, tỷ lệ hoàn thành, ngày/người đánh giá |
| 10 | AI Dashboard | `Views/Ai/Dashboard.cshtml` | XLSX, PDF, CSV | `AiController.XuatFile` | `IAiService.LayDashboardAsync`, `IExportFileService.Export` | `AI.Dashboard` và `ThongKe.XuatFile` | Tổng lượt phân tích/xác nhận, tỷ lệ xác nhận, dataset, dự án trễ chưa xác nhận, nguyên nhân theo tổng thể/quản lý/team |
| 11 | AI Dataset | `Views/AiDataset/Index.cshtml` | XLSX, PDF, CSV | `AiDatasetController.XuatFile` | `IAiDatasetService.LayDatasetHopLeDeTrainAsync`, `IExportFileService.Export` | Chỉ `ThongKe.XuatFile` tại action | 25 đặc trưng/nhãn dataset hợp lệ để train, gồm mã dự án, nhân sự, công việc, chi phí, đề xuất, tiến độ, trễ và mã nguyên nhân |
| 12 | AI Model | Màn hình danh sách/chi tiết model | JSON | `AiController.ExportMetadata` | `IAiService.LayTrangModelAsync` | `AI.Train` | Toàn bộ `ChiTietModel` đã serialize; tên `metadata-{modelFile}.json` |
| 13 | File dự án | Chi tiết dự án | Giữ nguyên định dạng tệp tải lên | `DuAnController.TaiFileDuAn` | `IFileDuAnService.GetDownloadInfoAsync` | `DuAn.Xem` và `CanAccessAsync` | Tệp dự án nguyên bản |
| 14 | File minh chứng tiến độ | Danh sách tiến độ/_FileList | Giữ nguyên định dạng tệp tải lên | `TienDoCongViecController.TaiFileTienDo` | `IFileTienDoCongViecService.GetDownloadInfoAsync` | `TienDo.Xem` và kiểm tra scope chi tiết | Tệp minh chứng nguyên bản |

Tên file của 11 báo cáo lần lượt dùng prefix: `thong-ke-tong-quan`, `du-an`, `cong-viec`, `chi-tiet-cong-viec`, `ngan-sach`, `nhan-su`, `tien-do-cong-viec`, `danh-gia-du-an`, `danh-gia-nhan-vien`, `bao-cao-ai-dashboard`, `ai-dataset`; hậu tố chỉ có ngày `yyyy-MM-dd`.

## 5. Đánh giá chất lượng file xuất hiện tại

### 5.1 Bố cục và hình thức

#### Điểm đã làm được

- Có tiêu đề báo cáo, ngày giờ xuất, người xuất và mô tả bộ lọc ở cả Excel, PDF, CSV.
- Header Excel/PDF có nền xanh nhạt `#eaf2ff`, gần với giao diện xanh dương của hệ thống.
- Excel có chữ đậm cho tiêu đề và header, có đường viền từng ô dữ liệu.
- PDF dùng A4 ngang, phù hợp hơn A4 dọc với các bảng nhiều cột.

#### Vấn đề AS-IS

- Excel không merge/căn giữa theo ý nghĩa trình bày ngoài thao tác merge; tiêu đề không đặt màu/chủ đề hệ thống, không có tên hệ thống/đơn vị.
- Không có số thứ tự. Nhiều báo cáo đang đưa khóa nội bộ như “Mã dự án”, “Mã công việc”, “Mã đánh giá” làm cột đầu.
- Không có freeze pane hoặc auto filter. Với dataset 25 cột hay danh sách dài, việc tra cứu khó.
- `worksheet.Columns().AdjustToContents()` áp dụng không giới hạn. Cột “Nội dung” hoặc chuỗi Dashboard ghép nhiều chỉ số có thể làm cột rất rộng; ngược lại không có wrap text.
- Tất cả cột và ô dùng cách trình bày gần như giống nhau; không căn trái/giữa/phải theo loại dữ liệu.
- Không có dòng tổng hợp cuối bảng. Báo cáo ngân sách không xuất các tổng `TongNganSach`, `TongNganSachDangHieuLuc`, `TongDaSuDung`, `TongConLai` vốn đã được service tính; Dashboard dồn tổng hợp và chi tiết vào ba cột chung.
- PDF luôn A4 ngang và chia mọi cột bằng nhau. Các báo cáo 12 cột (dự án/công việc/đánh giá dự án) và đặc biệt AI Dataset 25 cột sẽ có ô rất hẹp, chữ xuống dòng dày và khó đọc/in.
- PDF không có số trang, footer, thông tin hệ thống hoặc cấu hình tránh chia một hàng dài bất hợp lý.

### 5.2 Định dạng dữ liệu

- `ExportColumnDefinition.ValueSelector` luôn trả `string`; vì vậy Excel nhận ngày, tiền, số và phần trăm dưới dạng text thay vì kiểu dữ liệu thực.
- `ExportSupport.FormatCurrency` tạo chuỗi có `VNĐ`; Excel không thể cộng/lọc số tự nhiên và việc căn phải không được áp dụng.
- Phần trăm được ghép dấu `%` thành chuỗi. Không có number format phần trăm.
- `FormatNumber` dùng `InvariantCulture`, trong khi tiền dùng `vi-VN`; quy ước dấu thập phân không đồng nhất giữa các cột.
- Giá trị null thường thành chuỗi rỗng, một số báo cáo dùng `"-"` cho chưa có đánh giá. Quy ước dữ liệu trống chưa thống nhất.
- Trạng thái phần lớn đã qua `TrangThai.ToDisplay`, `TrangThaiHienThi` hoặc logic riêng cho `ChuaDanhGia`. Tuy nhiên `TienDoCongViecController.XuatFile` xuất trực tiếp `TrangThaiCTCV` và `TrangThaiDuyetBaoCaoGanNhat`; cần xác nhận các ViewModel này đã luôn là nhãn hiển thị, nếu không có thể lộ mã kỹ thuật.
- Dashboard và AI Dashboard chuyển số sang chuỗi trước khi xuất (`ToString`, chuỗi ghép nhiều chỉ số), làm mất khả năng tính toán.
- AI Dataset xuất “Chi phí dự kiến/thực tế/chênh lệch” bằng `FormatNumber`, không định dạng tiền Việt Nam và không ghi đơn vị.

### 5.3 Tên file và tên sheet

- Prefix hiện tại an toàn ở mức cơ bản: `NormalizeFileNamePrefix` thay ký tự không hợp lệ và khoảng trắng bằng dấu gạch ngang.
- Hậu tố chỉ có ngày, nên nhiều lần tải cùng báo cáo trong một ngày tạo cùng tên và trình duyệt phải tự thêm `(1)`, `(2)`.
- Tên file dùng dạng kỹ thuật có dấu gạch ngang, chưa diễn đạt rõ bằng tên báo cáo tiếng Việt không dấu/PascalCase.
- Sheet Excel luôn là `BaoCao`, không phân biệt module.
- `metadata-{modelFile}.json` dùng trực tiếp `modelFile`; chưa qua hàm loại bỏ ký tự không an toàn. Dù `modelFile` được dùng để tìm model hợp lệ, tên download vẫn nên được chuẩn hóa rõ ràng.
- Không có mã dự án/công việc trong tên file của báo cáo chi tiết, nên khó phân biệt nhiều file đã tải.

### 5.4 Tiếng Việt và font chữ

- Chuỗi tiếng Việt trong controller/helper và tài liệu source được lưu UTF-8; CSV chủ động thêm UTF-8 BOM, là điểm phù hợp khi mở bằng Microsoft Excel.
- Excel không chỉ định font lạ, nên dùng font mặc định của workbook và thường hỗ trợ tiếng Việt.
- PDF không đăng ký hoặc chỉ định font hỗ trợ tiếng Việt. QuestPDF sẽ phụ thuộc font fallback của môi trường triển khai; đây là rủi ro khi deploy sang máy/container khác với môi trường phát triển.
- CSV dùng dấu phẩy. Trên máy Windows có regional setting dùng dấu phẩy làm dấu thập phân và dấu chấm phẩy làm list separator, Excel có thể mở toàn bộ dòng vào một cột dù encoding đúng.
- Cần kiểm thử byte-level và file đầu ra thực tế; không nên chỉ dựa vào cách PowerShell/terminal hiển thị tiếng Việt.

### 5.5 Trải nghiệm khi xem, lọc và in

- Nút xuất nằm thống nhất trong dropdown và giữ phần lớn bộ lọc màn hình qua query string.
- Không có trạng thái “đang tạo file”; báo cáo lớn được dựng đồng bộ trong RAM, trình duyệt chỉ chờ phản hồi.
- Excel thiếu auto filter, freeze header, wrap text và thiết lập vùng in.
- PDF không thích ứng theo số cột; AI Dataset 25 cột gần như không phù hợp để in nguyên bảng trên A4 ngang.
- CSV có metadata trước header. Điều này tốt khi đọc thủ công nhưng làm khó import vào công cụ mong header ở dòng đầu.
- Báo cáo không ghi số dòng, phạm vi vai trò cụ thể hoặc tên dự án/team đã resolve; phần bộ lọc thường ghi mã như `Mã dự án: 12`, không ghi tên dễ đọc.

## 6. Kiểm tra tính đúng đắn và phạm vi dữ liệu

### Đồng bộ bộ lọc và phân trang

- Dashboard giữ đủ `tuNgay`, `denNgay`, `locNhanh`, dự án, quản lý, team, trạng thái, loại dự án và loại mốc ngày.
- Dự án giữ đủ từ khóa, loại, trạng thái, ngày, loại mốc ngày và tình trạng thời hạn.
- Công việc giữ đủ dự án, trạng thái, từ khóa, ngày, loại mốc ngày và tình trạng thời hạn; còn hỗ trợ tương thích `treHan=true`.
- Ngân sách, tiến độ, đánh giá dự án, đánh giá nhân viên và nhân sự giữ các filter chính của màn hình.
- Các báo cáo danh sách gọi service với `paginate: false` hoặc `GetAllAsync`, nên xuất toàn bộ dữ liệu đã lọc, không chỉ trang hiện tại. Đây là hành vi đúng với kỳ vọng “xuất báo cáo”.
- Dashboard không phải bản sao toàn bộ màn hình: biểu đồ dự án chỉ lấy tối đa 12 dự án trong `DashboardService`, và file chỉ ghi một số KPI/top list thành ba cột. Các biểu đồ trạng thái, timeline và một số thống kê trên ViewModel không được đưa vào file.
- AI Dataset nhận `thieuDataset` từ View nhưng action luôn gọi `LayDatasetHopLeDeTrainAsync`; biến này chỉ xuất hiện trong dòng mô tả bộ lọc, không làm thay đổi query. Vì vậy khi màn hình đang ở trạng thái “thiếu dataset”, file vẫn là dataset hợp lệ để train, không phải danh sách phần dữ liệu đang thiếu. Đây là sai lệch rõ giữa nhãn bộ lọc và dữ liệu.
- View đánh giá nhân viên truyền thêm `nguon`, nhưng action `XuatFile` không nhận tham số này. Nếu `nguon` làm thay đổi trạng thái/màn hình nguồn, file không phản ánh thông tin đó.

### Scope theo vai trò và soft delete

- `DuAnService.GetAllAsync`: Manager chỉ dự án do mình quản lý; Employee chỉ dự án có bản ghi `NhanVienDuAn`; nhóm vai trò khác (Admin) không bị giới hạn. `DuAn.IsDeleted` được loại.
- `CongViecService.GetPageAsync`: dùng dự án truy cập được; Employee không phải Manager/Admin còn bị giới hạn về dự án leader hoặc công việc được phân công. Công việc, danh mục, dự án và chi phí soft delete được loại.
- `ChiTietCongViecService.GetPageAsync`: tải công việc cha qua logic kiểm tra truy cập/cập nhật hiện hành và loại `CtCongViec.IsDeleted`.
- `NganSachService.GetPageAsync`: Manager lấy dự án quản lý; Employee lấy dự án tham gia; Admin/nhóm khác lấy toàn bộ dự án chưa xóa. Ngân sách và dự án soft delete được loại.
- `TienDoCongViecService.GetPageAsync`: áp dụng scope theo Admin/Manager/Employee, dự án/công việc/chi tiết và phân công; cần giữ nguyên khi cải thiện file.
- `DanhGiaDuAnService` và `DanhGiaNhanVienService`: tự kiểm tra quyền xem module và tạo danh sách dự án theo scope. Soft delete được áp dụng cho dự án, người dùng và bản đánh giá.
- `DashboardService`: tạo `DashboardScope` theo người dùng hiện tại và dùng nó cho query dự án/KPI. Các bảng nghiệp vụ chính có điều kiện `IsDeleted != true`.
- `AiService.LayDashboardAsync`: lấy danh sách dự án được phép theo role trước khi tổng hợp.
- `AiDatasetService.LayDatasetHopLeDeTrainAsync`: query dataset hợp lệ toàn hệ thống, không thấy scope theo người dùng trong action xuất.
- `NhanSuService.GetAllAsync`: query toàn bộ nhân sự chưa soft delete; không có scope theo vai trò.

### Quyền và nguy cơ lộ dữ liệu

- Tất cả dropdown báo cáo chỉ hiện khi có `ThongKe.XuatFile`; action cũng kiểm tra quyền này.
- Seed mặc định cấp `ThongKe.XuatFile` cho Admin, Manager và Employee.
- `NhanSuController.XuatFile` không kiểm tra thêm `NhanSu.Xem`, trong khi dữ liệu gồm username, email, số điện thoại, ngày sinh và trạng thái tài khoản. Employee có thể gọi URL trực tiếp để xuất toàn bộ nhân sự dù không được cấp mặc định `NhanSu.Xem`. Đây là rủi ro nghiêm trọng.
- `AiDatasetController.XuatFile` không kiểm tra `AI.Dataset`, chỉ kiểm tra `ThongKe.XuatFile`. Employee mặc định có quyền xuất nhưng không có `AI.Dataset`, nên có thể gọi URL trực tiếp để lấy toàn bộ dataset huấn luyện và mã nguyên nhân.
- `DashboardController.XuatFile`, `DuAnController.XuatFile`, `CongViecController.XuatFile`, `NganSachController.XuatFile`, `TienDoCongViecController.XuatFile` không kiểm tra đồng thời quyền xem module tương ứng tại controller. Một số service giữ scope dữ liệu, nhưng nguyên tắc phòng thủ theo quyền màn hình chưa đồng nhất.
- `AiController.XuatFile` là action chặt hơn: yêu cầu cả `AI.Dashboard` và `ThongKe.XuatFile`.
- Hai service đánh giá gọi `KiemTraQuyenTheoClaim` nên yêu cầu quyền xem ngay cả khi controller chỉ kiểm tra quyền xuất.
- Không thấy mật khẩu, password hash, token hoặc claim quyền được đưa vào các báo cáo. Báo cáo nhân sự có dữ liệu cá nhân/nội bộ nhưng không có bí mật xác thực.

### Hiệu năng

- Mọi báo cáo được nạp toàn bộ vào `List<object>` rồi dựng toàn bộ file thành `byte[]` trong RAM. Không có giới hạn số dòng, streaming, cảnh báo kích thước hoặc xử lý nền.
- `paginate: false` vẫn thực hiện `CountAsync` và một số truy vấn enrichment/summary. Với dữ liệu lớn, tổng số query và bộ nhớ tăng.
- `DuAnService.GetAllAsync` có các subquery `Count`/`Any` trong projection; EF Core thường dịch thành SQL correlated subquery, không phải N+1 phía client, nhưng vẫn có thể tốn chi phí trên tập lớn.
- Dashboard cố ý chạy nhiều query tổng hợp có logging; xuất file gọi lại toàn bộ `GetDashboardAsync`, nên chi phí tương đương tải lại Dashboard.
- AI Dataset 25 cột và có thể nhiều dòng; đây là luồng có nguy cơ RAM/thời gian phản hồi cao nhất.

## 7. Các lỗi và rủi ro phát hiện

| Mức độ | Vấn đề | Bằng chứng trong source | Ảnh hưởng | Hướng xử lý |
| ------ | ------ | ----------------------- | --------- | ----------- |
| Cao | Xuất toàn bộ nhân sự chỉ cần quyền xuất thống kê | `NhanSuController.XuatFile` chỉ kiểm tra `ThongKe.XuatFile`; `NhanSuService.GetAllAsync` không scope; seed cấp quyền xuất cho Employee | Lộ email, số điện thoại, ngày sinh, username và trạng thái tài khoản | Yêu cầu thêm `NhanSu.Xem` tại View/action; giữ nguyên query nghiệp vụ |
| Cao | Xuất AI Dataset không yêu cầu quyền module AI Dataset | `AiDatasetController.XuatFile` chỉ kiểm tra `ThongKe.XuatFile`; seed Employee không có `AI.Dataset` nhưng có quyền xuất | Người dùng ngoài module có thể lấy toàn bộ dữ liệu huấn luyện và nhãn | Yêu cầu đồng thời `AI.Dataset` và `ThongKe.XuatFile` |
| Cao | `thieuDataset` không tác động dữ liệu xuất | Action nhận `thieuDataset` nhưng luôn gọi `LayDatasetHopLeDeTrainAsync`; biến chỉ nằm trong `AppliedFiltersText` | File tuyên bố filter nhưng dữ liệu không theo filter màn hình | Tách đúng ý nghĩa: hoặc bỏ nhãn filter, hoặc gọi query đúng danh sách thiếu nếu nghiệp vụ có |
| Cao | PDF AI Dataset 25 cột không khả dụng khi đọc/in | 25 `ExportColumnDefinition`; `BuildPdf` luôn A4 ngang và mọi cột `RelativeColumn()` bằng nhau | Chữ quá nhỏ, xuống dòng dày, bảng rất dài | Không cung cấp PDF cho dataset rộng hoặc chia thành nhóm bảng/phụ lục phù hợp |
| Trung bình | Excel biến mọi giá trị thành text | `ValueSelector` trả `string`; `BuildExcel` gán chuỗi cho mọi cell | Không cộng/lọc/sắp xếp đúng kiểu số, ngày, tiền, phần trăm | Mở rộng contract tối thiểu để mang giá trị kiểu và number format |
| Trung bình | Excel thiếu filter/freeze/wrap và width cap | `BuildExcel` chỉ style header, border và `AdjustToContents()` | Khó tra cứu; cột nội dung có thể quá rộng | Freeze dòng 6, bật auto filter, wrap cột dài, giới hạn width |
| Trung bình | Tên file trùng trong cùng ngày | `dateSuffix = yyyy-MM-dd` | Trình duyệt tự thêm hậu tố khó quản lý | Dùng `yyyyMMdd_HHmmss` |
| Trung bình | Dashboard file không phản ánh đầy đủ Dashboard | Controller chỉ dựng các KPI/top list; service biểu đồ lấy tối đa 12 dự án | Người dùng tưởng file là bản đầy đủ của màn hình | Ghi rõ phạm vi trong tiêu đề/bộ lọc hoặc bổ sung các bảng có giá trị nghiệp vụ |
| Trung bình | Quyền xuất và quyền xem module chưa đồng nhất | Nhiều action chỉ kiểm tra `ThongKe.XuatFile`; chỉ AI Dashboard và service đánh giá kiểm tra quyền kép | URL trực tiếp có thể vượt qua việc ẩn màn hình | Chuẩn hóa kiểm tra kép theo module, không thay đổi danh mục quyền |
| Trung bình | Không có giới hạn/streaming cho file lớn | `Rows` là `List<object>`, kết quả là `byte[]`; `paginate:false` | RAM cao, request timeout, giảm khả năng phục vụ đồng thời | Đặt ngưỡng/cảnh báo hợp lý; tối ưu query và kiểm thử tải trước khi cân nhắc cơ chế phức tạp |
| Trung bình | PDF không có số trang và font được kiểm soát | `BuildPdf` không có `page.Footer`, không chỉ định font | Khó in/đối chiếu; rủi ro font tiếng Việt theo môi trường | Dùng font phổ biến có hỗ trợ tiếng Việt và footer `Trang x/y` |
| Thấp | Sheet luôn tên kỹ thuật `BaoCao` | `workbook.Worksheets.Add("BaoCao")` | Khó phân biệt khi ghép/copy workbook | Cho phép tên sheet ngắn theo từng báo cáo |
| Thấp | Metadata filename chưa chuẩn hóa | `metadata-{modelFile}.json` dùng trực tiếp modelFile | Tên tải xuống có thể dài/khó quản lý | Chuẩn hóa basename và thêm timestamp |
| Thấp | CSV có metadata trước header và dấu phân cách cố định | `BuildCsv` ghi 4 dòng metadata; dùng dấu phẩy | Khó import tự động hoặc mở theo regional setting | Xác định CSV dành cho người đọc hay máy; nếu dành cho máy, header ở dòng đầu và metadata tách riêng |

## 8. Bố cục file xuất đề xuất

### Nguyên tắc chung TO-BE

- Giữ `IExportFileService` hiện có; chỉ mở rộng vừa đủ, không cần đổi kiến trúc.
- Dòng 1: tên báo cáo rõ ràng, màu navy/xanh dương nhẹ.
- Dòng 2-5: thời gian xuất, người xuất, điều kiện lọc, phạm vi dữ liệu/số dòng.
- Header: đậm, nền xanh nhạt, chữ dễ đọc, có border.
- Freeze header và bật auto filter cho Excel.
- Kiểu dữ liệu thật: ngày giờ, số, tiền, phần trăm; dùng number format thay vì chuỗi.
- Căn trái cho văn bản; giữa cho STT/trạng thái/ngày ngắn; phải cho số/tiền/phần trăm.
- Wrap text và giới hạn độ rộng cho nội dung dài.
- Font phổ biến như Arial/Calibri cho Excel; font được đóng gói hoặc xác nhận hỗ trợ tiếng Việt cho PDF.
- Không dùng quá nhiều màu; giữ navy - xanh dương - trắng phù hợp giao diện.

### Dashboard tổng quan

- Sheet/bảng 1 “TongQuan”: KPI theo hai cột “Chỉ tiêu - Giá trị”, có nhóm Tổng quan/Cảnh báo.
- Bảng 2 “TheoDuAn”: STT, dự án, tiến độ, chi phí.
- Bảng 3 “CanhBao”: top dự án trễ và top vượt ngân sách với cột riêng thay vì chuỗi ghép.
- Ghi rõ dữ liệu theo scope và nếu chỉ lấy top/tối đa 12 dự án.
- PDF phù hợp vì số cột ít; dùng A4 dọc cho KPI, ngang cho bảng theo dự án nếu cần.

### Dự án

- Cột: STT, tên dự án, loại, quản lý, bắt đầu, hạn kết thúc, hoàn thành thực tế, tiến độ, trạng thái, tình trạng thời hạn, số team, số thành viên.
- “Mã dự án” có thể giữ ở cuối hoặc ẩn tùy nhu cầu đối soát; không nên thay STT.
- Tiến độ dùng number format phần trăm.
- PDF A4 ngang, ưu tiên 9-10 cột nghiệp vụ; có thể bỏ mã và các cột đếm ít quan trọng khỏi PDF nhưng vẫn giữ trong Excel/CSV.

### Công việc và chi tiết công việc

- Công việc: STT, tên, dự án, danh mục, mức độ, bắt đầu, hạn, hoàn thành, tình trạng hạn, số ngày trễ, chi phí, trạng thái.
- Chi tiết: STT, tên chi tiết, nội dung, bắt đầu, hoàn thành, trạng thái; mã nội bộ để cuối.
- Wrap text cho tên/nội dung, giới hạn độ rộng 35-50 ký tự hiển thị.
- Cố định timestamp `dd/MM/yyyy HH:mm` cho mốc có ý nghĩa thời gian; không hạ xuống date-only với dữ liệu deadline hiện đang có giờ.

### Ngân sách

- Cột: STT, dự án, phiên bản, ngân sách, trạng thái hiệu lực/trạng thái duyệt, ngày duyệt, ngày cập nhật, người đề xuất, người duyệt.
- Số tiền là decimal với format `#,##0 "VNĐ"`.
- Cuối bảng có tổng ngân sách, ngân sách hiệu lực, đã sử dụng và còn lại từ dữ liệu service hiện có.
- PDF A4 ngang, có thể bỏ mã nội bộ.

### Nhân sự

- Chỉ xuất sau khi xác nhận quyền `NhanSu.Xem`.
- Cột nghiệp vụ: STT, họ tên, chức danh, điện thoại, email, trạng thái tài khoản. Ngày sinh/username chỉ giữ nếu yêu cầu báo cáo thực tế; hạn chế dữ liệu cá nhân không cần thiết.
- Không xuất password hash, token, claim hoặc trường kỹ thuật; source hiện tại chưa xuất các trường này.
- PDF A4 ngang hoặc dọc tùy số cột sau khi tối giản.

### Tiến độ công việc

- Cột: STT, dự án, công việc, chi tiết, người thực hiện, tiến độ, trạng thái công việc, báo cáo gần nhất, trạng thái duyệt.
- Phần trăm là số thực; chuẩn hóa nhãn trạng thái qua helper hiện hành.
- Có thể thêm tổng số chi tiết theo trạng thái ở cuối nếu ViewModel/service đã có dữ liệu, không phát sinh query dư thừa.

### Đánh giá dự án và đánh giá nhân viên

- Đánh giá dự án: STT, dự án, quản lý, trạng thái dự án, số việc/trễ, trạng thái đánh giá, điểm, xếp loại, ngày, người đánh giá/duyệt.
- Đánh giá nhân viên: STT, dự án, nhân viên, vai trò, trạng thái đánh giá, điểm, xếp loại, tỷ lệ hoàn thành, ngày, người đánh giá.
- Điểm dùng số `0.00`; tỷ lệ dùng number format phần trăm; chưa đánh giá hiển thị “Chưa đánh giá”, không dùng mã kỹ thuật.
- PDF A4 ngang; header lặp qua trang.

### AI Dashboard

- Tách các nhóm Tổng quan, Dataset, Nguyên nhân phổ biến, Theo quản lý, Theo team thành bảng/section riêng.
- Không ghép “nhóm - nguyên nhân” hoặc nhiều số vào một ô nếu có thể tách thành cột.
- Ghi rõ đây là thống kê hỗ trợ phân tích, không phải kết luận/ground truth nghiệp vụ.

### AI Dataset

- Excel/CSV là định dạng chính. Excel dùng freeze pane, auto filter và nhóm màu header rất nhẹ theo cụm đặc trưng.
- Không khuyến nghị PDF 25 cột. Nếu vẫn cần PDF, phải chia thành các bảng: thông tin dự án, công việc/chi phí, đề xuất/duyệt, tiến độ/nhãn; mỗi bảng lặp mã dự án.
- Tiền dùng format tiền; tỷ lệ dùng format phần trăm; mã nguyên nhân cần thêm tên nguyên nhân nếu service đã có dữ liệu liên kết.
- Phải làm rõ `thieuDataset`: báo cáo dataset hợp lệ hay báo cáo dòng thiếu; không gắn nhãn filter sai.

### JSON metadata

- Giữ JSON thuần, UTF-8 và indent như hiện tại.
- Có thể thêm timestamp vào filename; không cần ép vào bố cục Excel/PDF.
- Chuẩn hóa tên model trong tên file, nhưng không thay đổi nội dung metadata.

### Quy tắc tên file đề xuất

- `BaoCaoTongQuan_yyyyMMdd_HHmmss.xlsx`
- `DanhSachDuAn_yyyyMMdd_HHmmss.xlsx`
- `DanhSachCongViec_yyyyMMdd_HHmmss.xlsx`
- `ChiTietCongViec_<MaCongViec>_yyyyMMdd_HHmmss.xlsx`
- `BaoCaoNganSach_yyyyMMdd_HHmmss.xlsx`
- `DanhSachNhanSu_yyyyMMdd_HHmmss.xlsx`
- `BaoCaoTienDoCongViec_yyyyMMdd_HHmmss.xlsx`
- `DanhGiaDuAn_yyyyMMdd_HHmmss.xlsx`
- `DanhGiaNhanVien_yyyyMMdd_HHmmss.xlsx`
- `BaoCaoTongQuanAI_yyyyMMdd_HHmmss.xlsx`
- `AIDatasetHopLe_yyyyMMdd_HHmmss.xlsx` hoặc `AIDatasetThieu_yyyyMMdd_HHmmss.xlsx` sau khi tách đúng nghiệp vụ.
- `MetadataModel_<TenModelAnToan>_yyyyMMdd_HHmmss.json`

Thay phần mở rộng theo định dạng thực tế. Tên chỉ dùng chữ cái Latin, chữ số và dấu gạch dưới để tránh ký tự không an toàn.

## 9. Danh sách file source dự kiến cần chỉnh sửa

| STT | File | Vai trò hiện tại | Nội dung cần chỉnh sửa | Mức ưu tiên |
| --- | ---- | ---------------- | ---------------------- | ----------- |
| 1 | `Services/Implementations/ExportFileService.cs` | Dựng XLSX/PDF/CSV và tên file | Kiểu dữ liệu/number format, freeze, filter, wrap, width cap, căn lề, font/footer PDF, timestamp filename | Cao |
| 2 | `Services/Exporting/ExportFileContracts.cs` | Contract tiêu đề/cột/dòng | Thêm metadata tối thiểu cho kiểu cột, format, alignment, tên sheet; tránh abstraction dư thừa | Cao |
| 3 | `Helpers/ExportSupport.cs` | Format ngày, tiền, số, filter, người xuất | Chuẩn hóa nhãn null, tên filter, phạm vi; hạn chế convert số thành string khi xuất Excel | Cao |
| 4 | `Controllers/NhanSuController.cs` | Query và khai báo cột báo cáo nhân sự | Kiểm tra thêm `NhanSu.Xem`, rà cột dữ liệu cá nhân, tên file | Cao |
| 5 | `Controllers/AiDatasetController.cs` | Xuất dataset train | Kiểm tra thêm `AI.Dataset`, đồng bộ `thieuDataset`, định dạng tiền/tỷ lệ, chiến lược PDF | Cao |
| 6 | `Controllers/DashboardController.cs` | Chuyển Dashboard ViewModel thành ba cột | Tách KPI/bảng chi tiết, mô tả giới hạn top/12 dự án, kiểu số thực | Trung bình |
| 7 | `Controllers/DuAnController.cs` | Khai báo báo cáo dự án | Thêm STT, kiểu ngày/phần trăm, tên filter dạng text, cân nhắc quyền xem kép | Trung bình |
| 8 | `Controllers/CongViecController.cs` | Khai báo báo cáo công việc | Kiểu thời gian/tiền/số, STT, tên dự án filter, quyền xem kép | Trung bình |
| 9 | `Controllers/ChiTietCongViecController.cs` | Khai báo báo cáo chi tiết | Wrap nội dung, tên file có mã công việc, STT | Trung bình |
| 10 | `Controllers/NganSachController.cs` | Khai báo báo cáo ngân sách | Dòng tổng hợp, kiểu tiền/ngày, trạng thái hiệu lực, quyền xem kép | Trung bình |
| 11 | `Controllers/TienDoCongViecController.cs` | Khai báo báo cáo tiến độ | Chuẩn hóa trạng thái, kiểu phần trăm/ngày, quyền xem kép | Trung bình |
| 12 | `Controllers/DanhGiaDuAnController.cs` | Khai báo báo cáo đánh giá dự án | Kiểu điểm/ngày, STT, nhãn filter, bố cục PDF | Trung bình |
| 13 | `Controllers/DanhGiaNhanVienController.cs` | Khai báo báo cáo đánh giá nhân viên | Xử lý/loại `nguon` rõ ràng, kiểu điểm/tỷ lệ/ngày | Trung bình |
| 14 | `Controllers/AiController.cs` | Xuất AI Dashboard và JSON metadata | Tách nhóm dữ liệu, chuẩn hóa metadata filename/timestamp | Trung bình |
| 15 | `Views/Shared/_ExportDropdown.cshtml` | Dropdown định dạng dùng chung | Cho phép ẩn định dạng không phù hợp, ví dụ PDF của AI Dataset | Trung bình |
| 16 | `ViewModels/Common/ExportDropdownViewModel.cs` | Cấu hình route/hiện CSV | Thêm cờ hiển thị PDF nếu cần; hiện chỉ có `HienThiCsv` | Trung bình |
| 17 | Các `Views/*/Index.cshtml`, `Views/Ai/Dashboard.cshtml`, `Views/AiDataset/Index.cshtml` | Truyền filter vào route xuất | Đồng bộ filter, quyền module và định dạng được phép | Trung bình |
| 18 | Service/interface nghiệp vụ của từng module | Tạo dữ liệu đã lọc/scope | Chỉ chỉnh nếu cần DTO export hoặc tổng hợp sẵn; giữ query/scope hiện tại | Thấp đến trung bình |
| 19 | `QuanLyDuAn.csproj` | Khai báo ClosedXML/QuestPDF | Không cần đổi package ở bước đầu; chỉ cập nhật nếu có lỗi/nhu cầu đã kiểm chứng | Thấp |
| 20 | `wwwroot/css/shared/export-dropdown.css` | Giao diện dropdown | Chỉ chỉnh nếu trạng thái disabled/loading hoặc nhãn định dạng thay đổi | Thấp |

Không cần tạo migration, sửa Entity/DbContext, thêm bảng/cột hoặc đổi workflow. Không cần thêm thư viện Excel/PDF mới vì ClosedXML và QuestPDF đã đáp ứng nền tảng.

## 10. Kế hoạch chỉnh sửa đề xuất

1. **Chuẩn hóa dữ liệu đầu vào.** Xác định cột, kiểu dữ liệu, nhãn null và tổng hợp của từng báo cáo; sửa sai lệch `thieuDataset`; không thay đổi query nghiệp vụ ngoài việc đồng bộ filter/scope.
2. **Chuẩn hóa tiêu đề và định dạng cột.** Mở rộng contract dùng chung ở mức tối thiểu cho kiểu dữ liệu, number format, alignment, wrap và width.
3. **Cải thiện bố cục file.** Thêm tiêu đề/phạm vi/số dòng, freeze pane, auto filter, wrap/width cap cho Excel; footer/font/khổ giấy phù hợp cho PDF.
4. **Đồng bộ bộ lọc và phạm vi dữ liệu.** Đối chiếu từng route value với tham số action/service; resolve mã dự án/quản lý/team thành tên hiển thị khi có sẵn.
5. **Kiểm tra quyền.** Bổ sung kiểm tra quyền xem module bên cạnh `ThongKe.XuatFile`, ưu tiên `NhanSu` và `AiDataset`; không tạo hoặc đổi tên quyền.
6. **Kiểm tra UTF-8 và tiếng Việt.** Kiểm tra source UTF-8, CSV BOM, font PDF và mở thử trên máy có regional setting Việt Nam.
7. **Kiểm thử file đầu ra.** Sinh file mẫu nhỏ/lớn cho từng module, mở bằng Excel/PDF reader, đối chiếu số dòng/tổng tiền/bộ lọc/scope và kiểm thử tải đồng thời.

Thứ tự an toàn: sửa kiểm tra quyền và filter sai trước; sau đó nâng cấp service dựng file dùng chung; cuối cùng tinh chỉnh riêng từng báo cáo.

## 11. Tiêu chí nghiệm thu sau khi chỉnh sửa

- Excel mở được bằng Microsoft Excel và LibreOffice, không có cảnh báo file hỏng.
- PDF mở/in được, không tràn/cắt bảng; header lặp qua trang và có số trang.
- CSV mở đúng cột hoặc có hướng dẫn rõ theo regional setting; UTF-8 BOM và tiếng Việt đúng dấu.
- Không có chuỗi mojibake hoặc font mất glyph tiếng Việt.
- Dữ liệu và tổng số dòng đúng với bộ lọc đang hiển thị, trừ phần nào được ghi rõ là top/giới hạn.
- Xuất toàn bộ kết quả đã lọc, không chỉ trang hiện tại.
- Manager chỉ nhận dữ liệu trong dự án mình quản lý/phạm vi service hiện hành.
- Employee không xuất được dữ liệu ngoài dự án, phân công hoặc module được phép.
- Admin không bị áp dụng nhầm scope Manager/Employee.
- `NhanSu/XuatFile` yêu cầu quyền xem nhân sự; `AiDataset/XuatFile` yêu cầu quyền AI Dataset.
- Soft delete được loại đúng như màn hình.
- Không xuất password, token, password hash, quyền nội bộ hoặc trường kỹ thuật không cần thiết.
- Trạng thái là nhãn tiếng Việt, không phải mã kỹ thuật.
- Ngày giờ, tiền và phần trăm là kiểu/định dạng đúng, có thể sắp xếp và tính toán trong Excel.
- Tiêu đề, người xuất, thời gian xuất, bộ lọc và phạm vi dữ liệu rõ ràng.
- Header được cố định, có bộ lọc; cột dài wrap và không rộng bất hợp lý.
- Tên file có timestamp đến giây, không trùng khi tải nhiều lần và không có ký tự không an toàn.
- PDF AI Dataset không được cung cấp dưới dạng 25 cột không đọc được; nếu có PDF phải chia nhóm hợp lý.
- File lớn không gây timeout/RAM vượt ngưỡng đã thống nhất.
- Không ảnh hưởng route, workflow, schema, quyền danh mục, chức năng danh sách và tải tệp có sẵn.

## 12. Kết luận

### AS-IS

Chức năng xuất file hiện tại đã có nền tảng tương đối thống nhất: 11 báo cáo dùng chung `IExportFileService`, hỗ trợ ba định dạng thật, giữ phần lớn bộ lọc, tái sử dụng service nghiệp vụ và chủ động bỏ phân trang để xuất toàn bộ tập kết quả. Phần lớn query đã áp dụng soft delete và scope vai trò như màn hình. Vì vậy logic nghiệp vụ cốt lõi không cần viết lại.

Vấn đề chính nằm ở ba nhóm:

1. **Quyền/phạm vi:** `NhanSu/XuatFile` và `AiDataset/XuatFile` chỉ yêu cầu `ThongKe.XuatFile`, tạo rủi ro truy cập trực tiếp ngoài quyền module.
2. **Tính đúng với filter:** `thieuDataset` được ghi vào mô tả nhưng không tác động query; Dashboard file cũng chỉ là bản tóm tắt một phần.
3. **Trình bày/kỹ thuật file:** mọi giá trị bị chuyển thành text; Excel thiếu filter/freeze/wrap/width cap; PDF dùng một bố cục A4 ngang cho cả bảng 3 cột lẫn 25 cột; tên file chỉ có ngày.

### TO-BE

Source hiện tại cung cấp đủ controller, service, ViewModel, helper, quyền và thư viện để triển khai bước cải thiện tiếp theo mà không đổi kiến trúc, schema hay workflow. Nên ưu tiên theo thứ tự:

1. Khóa đúng quyền cho Nhân sự và AI Dataset.
2. Đồng bộ filter AI Dataset và xác nhận phạm vi nội dung Dashboard.
3. Nâng cấp `ExportFileService`/contract để giữ kiểu dữ liệu và cải thiện Excel/PDF.
4. Tinh chỉnh cột, tổng hợp và tên file theo từng module.

Chỉ sau khi ba điểm ưu tiên đầu được xử lý và kiểm thử file thực tế mới nên xem xét tối ưu file rất lớn hoặc streaming; hiện chưa có bằng chứng cần thêm kiến trúc hay thư viện mới.

## 13. Kết quả triển khai

### File đã chỉnh sửa

- Hạ tầng xuất file: `Services/Exporting/ExportFileContracts.cs`, `Services/Implementations/ExportFileService.cs`, `Helpers/ExportSupport.cs`.
- Controller: `DashboardController.cs`, `DuAnController.cs`, `CongViecController.cs`, `ChiTietCongViecController.cs`, `NganSachController.cs`, `NhanSuController.cs`, `TienDoCongViecController.cs`, `DanhGiaDuAnController.cs`, `DanhGiaNhanVienController.cs`, `AiController.cs`, `AiDatasetController.cs`.
- Giao diện chọn định dạng: `ViewModels/Common/ExportDropdownViewModel.cs`, `Views/Shared/_ExportDropdown.cshtml`, `Views/AiDataset/Index.cshtml`.
- Tài liệu: `docs/xuatfile.md`.

Không chỉnh sửa Entity, DbContext, schema, migration, seed, SQL, FastAPI, package hoặc hai luồng `TaiFileDuAn`/`TaiFileTienDo`.

### Quyền và bộ lọc

- Cả 11 action `XuatFile` hiện yêu cầu đồng thời `ThongKe.XuatFile` và quyền xem module tương ứng. Riêng AI Dashboard tiếp tục yêu cầu `AI.Dashboard`; AI Dataset yêu cầu thêm `AI.Dataset`.
- `NhanSuController.XuatFile` kiểm tra quyền trước khi gọi `NhanSuService.GetAllAsync`; file nhân sự đã bỏ ngày sinh và username, chỉ giữ cột nghiệp vụ cùng mã đối soát ở cuối.
- Source chưa có query xuất “dữ liệu thiếu dataset”. Vì vậy `thieuDataset` không còn được ghi như một bộ lọc tác động dữ liệu; báo cáo và tên file luôn mô tả đúng dữ liệu thực tế là dataset hợp lệ để huấn luyện.
- Các route filter hiện có và `paginate: false`/`GetAllAsync` được giữ nguyên. Không thay đổi query hoặc scope Admin/Manager/Employee trong service.

### Excel

- Contract cột hỗ trợ giá trị `object?`, number format, alignment, wrap text, độ rộng tối thiểu/tối đa, độ rộng PDF và khả năng hiện theo từng định dạng.
- Dòng 1-6 lần lượt là tiêu đề, thời gian xuất, người xuất, bộ lọc, phạm vi/tổng số dòng và header.
- Có tên sheet riêng, STT cho báo cáo danh sách, freeze header, AutoFilter, border nhẹ, navy/xanh nhạt, căn lề theo kiểu dữ liệu, wrap và giới hạn độ rộng.
- Ngày giờ, số, tiền, điểm và tỷ lệ được ghi bằng kiểu dữ liệu thật. Tiền dùng `#,##0 "VNĐ"`; tỷ lệ nguồn 0-100 dùng `0.##"%"` để không bị nhân thêm 100.
- Báo cáo ngân sách dùng trực tiếp bốn tổng đã có trong `NganSachPageViewModel`, không phát sinh query mới.
- Tên file có timestamp `yyyyMMdd_HHmmss`.

### PDF

- Có header báo cáo, thời gian, người xuất, bộ lọc, phạm vi, tổng số dòng và footer `Trang x/y`.
- Dùng font Lato được QuestPDF cung cấp trong runtime hiện tại; smoke test đã tạo PDF thành công với chuỗi tiếng Việt.
- Hỗ trợ A4 dọc/ngang theo request, độ rộng cột tương đối và căn lề theo loại dữ liệu; header bảng tiếp tục lặp khi qua trang.
- Các mã kỹ thuật/nhóm cột ít cần khi in được ẩn khỏi một số PDF nhưng vẫn giữ trong Excel/CSV.
- PDF AI Dataset đã bị ẩn ở View và bị action từ chối cả khi gọi URL trực tiếp.

### CSV

- CSV được xác định là định dạng dữ liệu phục vụ tra cứu/import: header nằm ở dòng đầu, không chen metadata trước bảng.
- Giữ UTF-8 BOM và escape dấu phẩy, dấu nháy kép, CR/LF.
- Giữ giá trị ngày/số/tiền/tỷ lệ theo định dạng đọc được; trường chứa dấu phẩy hoặc xuống dòng được quote đúng.

### Tinh chỉnh theo module

- Dashboard và AI Dashboard tách quản lý, tiến độ, ngân sách, chi phí, chênh lệch, đối tượng và đơn vị thành cột riêng thay vì ghép nhiều KPI vào một chuỗi. Metadata Dashboard ghi rõ bảng theo dự án lấy tối đa 12 dự án; AI Dashboard ghi rõ AI chỉ hỗ trợ phân tích.
- Dự án, công việc, chi tiết công việc, tiến độ và hai báo cáo đánh giá dùng ngày/giờ, tiền, điểm, tỷ lệ và số lượng typed; mã nội bộ chuyển về cuối hoặc ẩn khỏi PDF.
- Tiến độ chuẩn hóa trạng thái qua `TrangThai.ToDisplay`.
- AI Dataset giữ nguyên 22 feature và điều kiện hợp lệ để train; chỉ thay cách trình bày/kiểu dữ liệu và không thêm join.
- Metadata model giữ nguyên nội dung JSON, chỉ chuẩn hóa basename và thêm timestamp.

### Build và kiểm thử

- `dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore`: thành công, 0 lỗi, 2 warning có sẵn tại `FileTienDoCongViecService` về method async không có `await`. File này thuộc luồng tải tệp nguyên bản và không được chỉnh sửa.
- Smoke test độc lập đã tạo Excel, PDF và CSV bằng `ExportFileService`.
- Excel được mở lại bằng ClosedXML; xác nhận đúng sheet, tiếng Việt, dữ liệu và AutoFilter.
- PDF có header file hợp lệ và được QuestPDF sinh thành công.
- CSV có UTF-8 BOM, header ở dòng đầu và escape đúng chuỗi có dấu phẩy, xuống dòng, dấu nháy kép.
- Kiểm tra tĩnh xác nhận đủ 11 action có cặp quyền xuất + quyền xem và các action danh sách vẫn dùng đường lấy toàn bộ dữ liệu đã lọc.

### Nội dung chưa thể kiểm thử trực tiếp

- Chưa chạy kiểm thử tích hợp với database và tài khoản thật cho ma trận Admin/Manager/Employee, do không khởi động runtime/database trong lượt triển khai này. Scope service không bị thay đổi và đã được kiểm tra tĩnh.
- Chưa render PDF thành ảnh hoặc mở bằng ứng dụng desktop để đánh giá trực quan mọi báo cáo dữ liệu thật. Smoke test xác nhận file PDF được sinh hợp lệ, nhưng chưa thay thế kiểm thử in thực tế với tập dữ liệu lớn.
- Chưa mở file bằng Microsoft Excel GUI; workbook đã được ClosedXML mở lại thành công, không có lỗi cấu trúc.

## 14. Rà soát đầu ra thực tế và chỉnh sửa vòng 2

### 14.1 Lỗi phát hiện từ file thực tế

- Excel Dashboard ghi số `16` ở báo cáo có nhiều cột không áp dụng cho dòng KPI, gây cảm giác giá trị bị lặp sang ô không liên quan.
- PDF có thể chia nội dung của một hàng giữa hai trang.
- Dashboard dùng một bảng chung cho KPI, cảnh báo, dự án trễ và ngân sách nên có nhiều ô trống, ý nghĩa cột thay đổi theo dòng.
- Metadata còn hiển thị mã kỹ thuật như `tatca`, `NgayTao`, `NgayBatDau`, `NgayKetThuc`.
- Cột Quản lý của danh sách dự án bị trống.
- Tiền VNĐ có thể hiển thị phần thập phân.
- CSV chưa tách hoàn toàn khỏi định dạng trình bày dành cho người đọc.

### 14.2 Nguyên nhân kỹ thuật

- `DashboardController.XuatFile` dùng một `DashboardExportRow` phẳng với các trường tùy chọn `GiaTri`, `DonVi`, `QuanLy`, `TienDo`, `NganSach`, `ChiPhi`, `ChenhLech` cho nhiều loại dòng khác nhau. Giá trị `16` là giá trị nghiệp vụ thật của KPI như tổng dự án/công việc trễ, không phải `MinWidth`, `MaxWidth` hoặc `PdfRelativeWidth`; source không có đường gán các thuộc tính độ rộng vào `ValueSelector`. Tuy nhiên bảng phẳng vẫn dựng toàn bộ cột cho mọi dòng, khiến KPI `16` xuất hiện trong một cấu trúc có nhiều cột không mang ý nghĩa với dòng đó. Đồng thời nhánh null cũ gán `string.Empty` thay vì xóa nội dung ô thật sự, nên không thể phân biệt chắc chắn blank cell và empty-string cell khi kiểm tra workbook.
- PDF cũ tạo từng ô trực tiếp trong bảng và gọi `ShowEntire` cho từng ô. Các ô trong cùng hàng là các phần tử phân trang riêng, nên không có một container duy nhất giữ toàn bộ hàng.
- Dashboard chỉ có một danh sách hàng/cột dùng chung; contract chưa có khái niệm section.
- Controller đưa trực tiếp mã filter vào `BuildFiltersText`; `ResolveTextOrDefault` chỉ xử lý null/trống, không đổi mã thành nhãn.
- `DuAnService.GetAllAsync` và `GetPagedAsync` project `TenNguoiQuanLy = string.Empty` dù `DuAn` có `MaNguoiDung`. Controller map đúng property nhưng property đã rỗng từ service.
- PDF dùng number format tiền để trình bày nhưng chưa chuẩn hóa giá trị qua một quy tắc làm tròn thống nhất trước khi format.
- CSV dùng chung `FormatValue` với Excel/PDF, nên number format trình bày có thể đưa ký hiệu `%`, `VNĐ` hoặc culture Việt Nam vào dữ liệu trao đổi.

### 14.3 Source đã chỉnh sửa

- `Services/Exporting/ExportFileContracts.cs`
- `Services/Implementations/ExportFileService.cs`
- `Helpers/ExportSupport.cs`
- `Controllers/DashboardController.cs`
- `Controllers/DuAnController.cs`
- `Controllers/CongViecController.cs`
- `Controllers/AiDatasetController.cs`
- `ViewModels/Dashboard/DashboardViewModel.cs`
- `Services/Implementations/DashboardService.cs`
- `Services/Implementations/DuAnService.cs`
- `docs/xuatfile.md`

Không chỉnh sửa database, schema, migration, Entity, DbContext, seed, SQL, FastAPI, permission, workflow, điều kiện dataset hợp lệ, 22 feature AI hoặc hai luồng tải tệp nguyên bản.

### 14.4 Kết quả sau sửa

- Contract có `ExportSectionDefinition` để một request chứa nhiều bảng/sheet và `ExportCellValue` để giữ giá trị typed cùng number format riêng cho từng ô KPI.
- `SetCellValue` chỉ nhận giá trị do `ValueSelector` trả về. Nếu giá trị là null, service gọi `cell.Clear(XLClearOptions.Contents)`; không dùng `0`, `16`, độ rộng cột hoặc chuỗi rỗng làm fallback.
- Dashboard Excel có ba sheet:
  - `TongQuan`: bảng `TỔNG QUAN` và `CẢNH BÁO`.
  - `TheoDuAn`: bảng `THỐNG KÊ THEO DỰ ÁN`, ghi rõ “Danh sách theo dõi tối đa 12 dự án”.
  - `CanhBao`: bảng `DỰ ÁN TRỄ` và `DỰ ÁN VƯỢT NGÂN SÁCH`.
- Dashboard PDF hiển thị tuần tự năm section trên A4 ngang; không còn bảng chín cột dùng chung cho mọi loại dòng.
- Mỗi hàng PDF là một outer table cell có `ShowEntire()`, bên trong là một bảng một hàng. Vì QuestPDF phân trang outer cell như một khối, toàn bộ các ô của hàng chuyển trang cùng nhau; header của outer table vẫn lặp lại và footer `Trang x/y` được giữ.
- PDF đặt nền trang, outer row và data cell màu trắng để tránh vùng trong suốt/đen khi render.
- `ExportSupport.ToDisplayFilterValue` chuẩn hóa các mã `tatca`, `homnay`, `7ngay`, `thangnay`, `quynay`, `namnay`, `NgayTao`, `NgayBatDau`, `NgayKetThuc`, `HanCongViec`. Trạng thái tiếp tục dùng `TrangThai.ToDisplay`; filter thời hạn trống hiển thị “Tất cả”.
- Dashboard resolve dự án, quản lý, team và loại dự án từ option hiện có thay vì chỉ ghi ID.
- `DuAnService.GetAllAsync/GetPagedAsync` dùng left join server-side với `NguoiDung` chưa soft delete để project tên quản lý; null/không còn người quản lý hiển thị “Chưa phân công”. Không có query theo từng dòng.
- Dashboard bổ sung `DuAnTheoDoi` bằng các query nhóm/batch có sẵn theo tối đa 12 dự án, gồm quản lý, tiến độ, ngân sách, chi phí và chênh lệch. Top vượt ngân sách cũng có tên quản lý.
- Tiền Excel/PDF được làm tròn tại lớp xuất bằng `MidpointRounding.AwayFromZero`, vẫn ghi numeric trong Excel và hiển thị `#,##0 "VNĐ"`. Dữ liệu database và phép tính nghiệp vụ không thay đổi.
- CSV có formatter riêng:
  - ngày giờ `yyyy-MM-dd HH:mm:ss`;
  - decimal/double/float dùng `InvariantCulture`;
  - không phân cách hàng nghìn;
  - không thêm `VNĐ` hoặc `%`;
  - null là field rỗng;
  - boolean là `true`/`false`;
  - giữ UTF-8 BOM, header dòng đầu và escaping chuẩn.
- AI Dataset tiếp tục ẩn PDF, giữ feature/nhãn/query/quyền hiện hành; Excel giữ kiểu số, AutoFilter, freeze header và freeze hai cột đầu (STT, mã dự án).

### 14.5 Kiểm thử

- Build:
  - Lệnh: `dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore`
  - Lần biên dịch đầy đủ sau thay đổi: thành công, `0 Error(s)`, `2 Warning(s)`.
  - Hai warning `CS1998` có sẵn tại `FileTienDoCongViecService.cs` dòng 26 và 31; file này thuộc luồng tải tệp nguyên bản và không được chỉnh sửa.
  - Lần build xác nhận cuối sau khi dọn file kiểm thử tạm: thành công, `0 Error(s)`, `0 Warning(s)` do build tăng dần không biên dịch lại file có warning cũ.
- File mẫu sinh tại `output/export-round2`:
  - `BaoCaoTongQuan_Vong2.xlsx`
  - `BaoCaoTongQuan_Vong2.pdf`
  - `DanhSachDuAn_170Dong.pdf`
  - `AIDatasetHopLe_Vong2.xlsx`
  - `AIDatasetHopLe_Vong2.csv`
- Excel Dashboard:
  - Mở lại thành công bằng ClosedXML.
  - Xác nhận đủ ba sheet `TongQuan`, `TheoDuAn`, `CanhBao`.
  - Dòng “Tổng dự án” có số `16` duy nhất tại cột Giá trị; cột Đơn vị và các ô ngoài bảng là `IsEmpty`.
  - Dòng “Tỷ lệ sử dụng ngân sách” có giá trị numeric `4.35` và đơn vị `%`.
  - Tiền `45529195001.02` được xuất thành numeric `45529195001`, không phải chuỗi.
- AI Dataset Excel:
  - Mở lại thành công bằng ClosedXML.
  - AutoFilter còn hiệu lực; tỷ lệ `4.36` vẫn là numeric và không bị nhân 100.
- PDF Dashboard:
  - QuestPDF sinh `2` trang.
  - Render bằng Poppler `pdftoppm` ở 120 DPI và kiểm tra trực quan cả hai trang: năm section tách rõ, tiếng Việt đúng, header/footer không cắt, không có ô vuông hoặc ký tự thay thế.
  - `pypdf` xác nhận footer `Trang 1/2`, `Trang 2/2`; tiền không còn đuôi `,02 VNĐ`/`,55 VNĐ`.
- PDF danh sách dự án:
  - Dữ liệu đại diện `170` dòng, tên ngắn và tên dài 2-4 dòng, gồm cả “Chưa phân công”.
  - Kết quả `12` trang; header lặp và STT liên tục.
  - Mỗi tên có marker `ROW-nnnn-BEGIN`/`ROW-nnnn-END`; `pypdf` xác nhận cả `170/170` cặp marker nằm trên cùng một trang, không có hàng bị chia.
  - Render bằng Poppler và kiểm tra trực quan các trang đầu/trang tiếp theo cho thấy hàng dài chuyển nguyên khối.
- CSV AI Dataset:
  - Parser chuẩn Python đọc `20` dòng dữ liệu, `5` cột mỗi dòng trong mẫu.
  - BOM đúng; header ở dòng đầu; không có `VNĐ`, `%` hoặc phân cách hàng nghìn.
  - Decimal dùng dấu chấm; ví dụ `45529195002.02` và `4.36` được giữ raw.
- Tất cả source/tài liệu chỉnh sửa đọc được bằng UTF-8 strict, không có `U+FFFD`.

### 14.6 Vấn đề còn lại

- Chưa chạy action MVC với database và tài khoản thật nên chưa kiểm thử tích hợp lại ma trận quyền/scope Admin, Manager, Employee. Điều kiện quyền và query scope vòng 1 không bị thay đổi.
- File mẫu dùng dữ liệu đại diện đúng contract xuất hiện tại, không phải bản chụp dữ liệu production.
- Chưa mở workbook bằng Microsoft Excel GUI; workbook đã được ClosedXML mở lại và kiểm tra kiểu/ô/filter thành công.
