# Phân tích chức năng Thống kê và Dashboard

## Phạm vi đã đọc

Tài liệu này bám theo source hiện tại của dự án ASP.NET MVC trong `QuanLyDuAn/QuanLyDuAn` và các tài liệu nghiệp vụ trong `docs/`.

Các nhóm file đã đối chiếu:

- Controller: `DashboardController.cs`, `AiController.cs`, `AiDatasetController.cs`, các controller có xuất file như `DuAnController.cs`, `CongViecController.cs`, `TienDoCongViecController.cs`, `NganSachController.cs`, `NhanSuController.cs`, `DanhGiaDuAnController.cs`, `DanhGiaNhanVienController.cs`.
- Service: `DashboardService.cs`, `AiService.cs`, `AiDatasetService.cs`, `ExportFileService.cs`, các service liên quan đến đánh giá, dự án, công việc, ngân sách, tiến độ.
- ViewModel: `ViewModels/Dashboard/*`, `ViewModels/Ai/*`, `DanhGiaDuAnThongKeViewModel.cs`, `DanhGiaNhanVienThongKeViewModel.cs`, các `PageViewModel` có chỉ số tóm tắt.
- View: `Views/Dashboard/Index.cshtml`, `Views/Ai/Dashboard.cshtml`, `Views/AiDataset/Index.cshtml`, các partial lọc, bảng, summary của dự án, công việc, tiến độ, ngân sách, nhân sự, đánh giá.
- CSS/JS: `wwwroot/css/Dashboard/index.css`, `wwwroot/css/AI/index.css`, `wwwroot/css/site.css`, `wwwroot/css/layout/sidebar.css`, `wwwroot/js/ai/charts.js`, các CSS module liên quan.
- Tài liệu nghiệp vụ: `docs/workflow-he-thong.md`, `docs/mvc-project-analysis.md`, `readme.md`, `docs/readme.md`.

Không thấy `ThongKeController.cs`, `Views/ThongKe/*` hoặc `wwwroot/css/ThongKe/*` trong source hiện tại. Chức năng thống kê đang nằm ở `Dashboard`, `AI`, `AI Dataset`, các màn hình danh sách nghiệp vụ và các màn hình đánh giá.

## Phần 1. Phân tích hiện trạng

### 1. Dashboard tổng quan

- Màn hình: Dashboard tổng quan.
- View: `Views/Dashboard/Index.cshtml`.
- Controller: `DashboardController.Index`.
- Service lấy dữ liệu: `DashboardService.GetDashboardAsync`.
- ViewModel: `DashboardViewModel`.
- CSS: `wwwroot/css/Dashboard/index.css`.
- Biểu đồ: Chart.js lấy từ CDN trong view.

Dashboard tổng quan đang có bộ lọc thời gian:

- Từ ngày.
- Đến ngày.
- Lọc nhanh: hôm nay, 7 ngày gần đây, tháng này, quý này, năm nay, tất cả.

Dashboard tổng quan đang có các KPI/card:

- Tổng dự án.
- Tổng công việc.
- Tổng đề xuất.
- Tổng nhân sự.
- Tổng ngân sách.
- Tổng chi phí đã chi.
- Dự án hoàn thành trong kỳ.
- Ngân sách còn lại.
- Tỷ lệ sử dụng ngân sách.

Dashboard tổng quan đang có nhóm trạng thái dự án:

- Khởi tạo.
- Đang thực hiện.
- Tạm dừng.
- Chờ xác nhận hoàn thành.
- Hoàn thành, kèm số đúng hạn và trễ hạn.
- Lưu trữ.

Dashboard tổng quan đang có nhóm cảnh báo:

- Công việc trễ hạn.
- Nhân sự quá tải.
- Dự án vượt ngân sách.
- Thiếu dữ liệu AI.

Dashboard tổng quan đang có nhóm đề xuất/yêu cầu chờ duyệt:

- Đề xuất công việc chờ duyệt.
- Đề xuất ngân sách chờ duyệt.
- Yêu cầu đổi quản lý chờ duyệt.

Dashboard tổng quan đang có biểu đồ:

- Biểu đồ cột: tiến độ các dự án, dùng `Model.TenDuAn` và `Model.PhanTramTienDo`.
- Biểu đồ đường/vùng: chi phí theo dự án, dùng `Model.TenDuAn` và `Model.ChiPhiTheoDuAn`.

Dashboard tổng quan đang có khối AI:

- Cảnh báo AI mức cao: lấy từ `AiService.LayDashboardAsync`, ánh xạ `SoDuAnTreChuaXacNhan` thành `SoCanhBaoDo`.
- Nguyên nhân trễ nhiều nhất: lấy 5 nguyên nhân phổ biến từ `NguyenNhanPhoBien`.

Dashboard tổng quan đang có xuất file:

- Action: `DashboardController.XuatFile`.
- Quyền kiểm tra: `Permissions.ThongKe.XuatFile`.
- Định dạng: Excel, PDF, CSV thông qua `ExportFileService`.
- Nội dung xuất: các KPI tổng quan, cảnh báo và dữ liệu theo dự án.

Điểm đáng chú ý trong hiện trạng:

- `DashboardController.Index` chưa kiểm tra `Permissions.ThongKe.Xem` trực tiếp. Sidebar chỉ hiện menu khi có quyền, nhưng nếu biết URL thì action chưa chặn bằng controller.
- `DashboardController.XuatFile` có nhiều chuỗi tiếng Việt bị lỗi mã hóa trong source, ví dụ tiêu đề báo cáo và nhãn nhóm dữ liệu đang hiển thị dạng mojibake.
- `DashboardService` có `WorkflowHealthItems` và `Suggestions` trong ViewModel nhưng đang trả danh sách rỗng, trong khi view vẫn hiển thị khu vực gợi ý ưu tiên.

### 2. Tổng quan AI

- Màn hình: Tổng quan AI.
- View: `Views/Ai/Dashboard.cshtml`.
- Controller: `AiController.Dashboard`.
- Service lấy dữ liệu: `AiService.LayDashboardAsync`.
- ViewModel: `AiDashboardPageViewModel`.
- CSS: `wwwroot/css/AI/index.css`.
- JS: `wwwroot/js/ai/charts.js`.
- Biểu đồ: Chart.js.

Tổng quan AI đang có các KPI/card:

- Trạng thái service AI.
- Model active.
- Tổng model nguyên nhân.
- Tổng lượt phân tích AI.
- Tổng dòng dataset.
- Dòng đủ điều kiện train.
- Dự án trễ đã xác nhận.
- Dự án trễ chưa xác nhận.
- Độ chính xác model active.
- Precision macro.
- Recall macro.
- F1 macro.

Tổng quan AI đang có biểu đồ:

- Biểu đồ tròn: phân bố nguyên nhân đã xác nhận.
- Biểu đồ cột: dataset theo trạng thái train.
- Biểu đồ đường: accuracy các phiên bản model nguyên nhân.
- Biểu đồ cột ngang: feature importance của model active.

Tổng quan AI đang có bảng:

- Cảnh báo dự án trễ theo AI, gồm dự án, nguyên nhân gợi ý, độ tin cậy, nguồn, thời gian, trạng thái dữ liệu.

Phân quyền AI:

- Controller kiểm tra `Permissions.AI.Dashboard`.
- `AiService` có lọc phạm vi dữ liệu: Admin thấy toàn bộ; người không phải Admin bị giới hạn theo dự án có quyền truy cập.

### 3. Quản lý dữ liệu AI

- Màn hình: Quản lý dữ liệu AI.
- View: `Views/AiDataset/Index.cshtml`.
- Controller: `AiDatasetController.Index`.
- Service lấy dữ liệu: `AiDatasetService.KhoiTaoTrangDatasetAsync`.
- ViewModel: `AiDatasetPageViewModel` và các ViewModel quản lý dataset.
- CSS: `wwwroot/css/AI/index.css`.

Màn hình này đang có thống kê:

- Tổng số dòng AI_DATASET.
- Số dòng trễ có nguyên nhân.
- Số dòng chưa đủ nhãn train.
- Chất lượng dataset trong DB.
- Tổng số dòng.
- Dòng hợp lệ để huấn luyện.
- Số dòng thiếu nhãn train.
- Số dự án được xác định trễ.
- Số dự án không trễ.
- Số dòng có nguyên nhân xác nhận.
- Số dòng huấn luyện tối thiểu.
- Đủ điều kiện huấn luyện.

Màn hình này có thao tác:

- Tổng hợp dataset.
- Tổng hợp lại.
- Kiểm tra chất lượng dataset.
- Tổng hợp theo dự án.
- Xuất file dataset hợp lệ để huấn luyện.

Điểm chưa hợp lý:

- Hai nút "Tổng hợp dataset" và "Tổng hợp lại" cùng submit về `TongHopDataset`, nên khác chữ nhưng cùng hành vi.
- Bộ feature trong view đang hiển thị tên kỹ thuật như `SoNhanVienDuAn`, `TongSoCongViec`, `AI_DATASET`. Điều này phù hợp cho quản trị AI, nhưng không nên xuất hiện trong Dashboard nghiệp vụ tổng quan.

### 4. Thống kê trong đánh giá dự án

- Màn hình: Đánh giá dự án.
- View: `Views/DanhGiaDuAn/Index.cshtml`, `_SummaryCards.cshtml`, `_ProjectStats.cshtml`, `_AiInsightCard.cshtml`.
- Controller: `DanhGiaDuAnController`.
- Service: `DanhGiaDuAnService`.
- ViewModel: `DanhGiaDuAnPageViewModel`, `DanhGiaDuAnThongKeViewModel`.

Đang có thống kê dạng summary:

- Tổng.
- Chưa đánh giá.
- Nháp.
- Chờ duyệt.
- Đã duyệt.
- Từ chối.

Thống kê chi tiết theo dự án có:

- Tên dự án, quản lý, trạng thái.
- Ngày bắt đầu, ngày kết thúc dự kiến, ngày hoàn thành thực tế.
- Số ngày còn lại/quá hạn.
- Phần trăm hoàn thành.
- Tổng công việc, công việc hoàn thành, công việc trễ hạn.
- Tổng chi tiết công việc, chi tiết hoàn thành, chi tiết trễ hạn.
- Tỷ lệ hoàn thành.
- Số báo cáo tiến độ.
- Tổng ngân sách, tổng chi phí, tỷ lệ sử dụng ngân sách.
- Số file dự án.
- Thông tin AI: có dữ liệu AI, dự án trễ theo AI, nguyên nhân AI dự đoán, độ tin cậy, xác nhận của quản lý, trạng thái dữ liệu AI.

### 5. Thống kê trong đánh giá nhân viên

- Màn hình: Đánh giá nhân viên.
- View: `Views/DanhGiaNhanVien/Index.cshtml`, `_SummaryCards.cshtml`, `_EmployeeStats.cshtml`.
- Controller: `DanhGiaNhanVienController`.
- Service: `DanhGiaNhanVienService`.
- ViewModel: `DanhGiaNhanVienPageViewModel`, `DanhGiaNhanVienThongKeViewModel`.

Đang có thống kê dạng summary:

- Tổng.
- Chưa đánh giá.
- Nháp.
- Chờ duyệt.
- Đã duyệt.
- Từ chối.

Thống kê chi tiết theo nhân viên có:

- Dự án, nhân viên, vai trò.
- Tổng chi tiết được giao.
- Chi tiết hoàn thành, đang làm, trễ hạn.
- Tỷ lệ hoàn thành.
- Số lần cập nhật tiến độ.
- Số báo cáo đã duyệt.
- Số báo cáo bị từ chối hoặc yêu cầu bổ sung.
- Số file minh chứng.
- Lần cập nhật gần nhất.
- Điểm trung bình tiến độ.

### 6. Thống kê dạng summary trong các màn hình danh sách

Các màn hình sau không phải dashboard thống kê riêng, nhưng có chỉ số tóm tắt, lọc và xuất file:

- Dự án: có lọc từ khóa, loại dự án, trạng thái, khoảng ngày, loại mốc ngày; có xuất file.
- Công việc: có summary tổng công việc, đang thực hiện, chờ xác nhận hoàn thành, hoàn thành, bị cản; có lọc dự án, trạng thái, từ khóa, khoảng ngày, loại mốc ngày; có xuất file.
- Tiến độ: có lọc dự án, công việc, chi tiết, từ khóa, khoảng ngày báo cáo; có xuất file.
- Ngân sách: có tổng ngân sách, tổng ngân sách đang hiệu lực; có lọc dự án, trạng thái; có xuất file.
- Nhân sự: có lọc từ khóa, chức danh, trạng thái tài khoản; có xuất file.
- AI Dataset: có xuất file dataset hợp lệ.
- Chi tiết công việc: có xuất file theo công việc.

## Phần 2. Đánh giá giao diện

### Ưu điểm

- Dashboard tổng quan có bố cục rõ: header, bộ lọc, KPI, trạng thái, cảnh báo, chờ duyệt, biểu đồ, AI, gợi ý.
- Dashboard tổng quan và Tổng quan AI dùng phong cách khá gần nhau: nền trắng, viền nhạt, shadow nhẹ, màu xanh/tím/xanh ngọc, card bo góc, responsive grid.
- Có responsive cơ bản cho desktop, tablet, mobile: grid 4 cột xuống 2 cột và 1 cột.
- Có trạng thái rỗng cho biểu đồ và AI.
- Có nút xuất file đặt trong header/khu vực bảng ở nhiều màn hình.
- Các màn hình danh sách nghiệp vụ đã có bộ lọc tương đối đầy đủ theo module.
- Màn hình Tổng quan AI có nhiều biểu đồ phù hợp với dữ liệu model/dataset.

### Nhược điểm

- Dashboard tổng quan có khá nhiều card số liệu nhưng thiếu bảng chi tiết hành động, ví dụ không có danh sách top dự án trễ, top nhân sự quá tải, top dự án vượt ngân sách.
- Một số card mang tính đếm tổng nhưng chưa nêu rõ ý nghĩa nghiệp vụ đủ sâu, ví dụ "Tổng đề xuất" đang cộng đề xuất công việc và đề xuất ngân sách, chưa tách rõ theo trạng thái hoặc theo loại.
- Khu vực "Gợi ý ưu tiên" hiện gần như rỗng vì `DashboardService` trả `Suggestions = []`. Việc hiển thị khu vực này làm người dùng tưởng hệ thống đã có engine gợi ý, nhưng thực tế chưa có dữ liệu.
- Khu vực "Yêu cầu đổi quản lý chờ duyệt" hiển thị "Chưa có màn hình duyệt phù hợp để mở trực tiếp", trong khi source có `DuyetYeuCauDoiQuanLyController` và sidebar cũng có menu duyệt yêu cầu đổi quản lý. Đây là điểm không đồng bộ điều hướng.
- Dashboard tổng quan dùng chữ "Dashboard" trong sidebar và tiêu đề thương hiệu có "Project Intelligence". Nếu yêu cầu thuần tiếng Việt, nên đổi thành "Thống kê" hoặc "Tổng quan".
- Màn hình AI Dataset hiển thị nhiều tên trường kỹ thuật. Điều này chấp nhận được cho Admin/AI, nhưng không đồng bộ với ngôn ngữ nghiệp vụ của Dashboard tổng quan.
- `ExportFileService` sinh metadata trong Excel/PDF/CSV bằng chuỗi không dấu như "Ngay xuat", "Nguoi xuat", "Bo loc", "Khong xac dinh". Chưa đạt yêu cầu tiếng Việt có dấu.
- `DashboardController.XuatFile` có chuỗi tiếng Việt bị lỗi mã hóa. Đây là lỗi UX khi xuất báo cáo.
- `Dashboard/index.css` và `AI/index.css` khá đồng bộ, nhưng các module khác dùng nhiều hệ class riêng như `du-an-*`, `workflow-filter`, `dgda-*`, `dgnv-*`, `ai-*`, nên toàn hệ thống chưa có một design system thống nhất cho dashboard/thống kê.
- Card Dashboard hiện bo góc 18-20px, lớn hơn các bảng module nghiệp vụ và có nhiều khoảng trắng. Với màn hình vận hành cần quét nhanh, có thể làm card gọn hơn.

## Phần 3. Thống kê đang thiếu

### Nhóm dự án

Nên bổ sung:

- Số dự án theo trạng thái trong kỳ và hiện tại.
- Số dự án theo loại dự án.
- Số dự án theo mức độ ưu tiên nếu dữ liệu ưu tiên đã có trên công việc hoặc dự án.
- Số dự án theo quản lý.
- Số dự án theo team.
- Top dự án trễ hạn.
- Top dự án gần đến hạn.
- Top dự án vượt ngân sách.
- Tỷ lệ hoàn thành trung bình theo quản lý/team.
- Dự án thiếu thành viên, thiếu danh mục công việc, thiếu công việc.

Hiện đã có một phần:

- Trạng thái dự án.
- Dự án hoàn thành trong kỳ.
- Dự án vượt ngân sách.
- Dự án thiếu AI_DATASET.
- Tiến độ theo 12 dự án gần nhất.

### Nhóm công việc

Nên bổ sung:

- Công việc theo trạng thái.
- Công việc theo dự án.
- Công việc theo nhân viên được phân công.
- Công việc theo tháng.
- Công việc trễ hạn theo mức độ trễ.
- Công việc bị cản.
- Công việc chờ xác nhận hoàn thành quá lâu.
- Công việc chưa có chi tiết.
- Tỷ lệ hoàn thành công việc theo dự án/team/nhân viên.

Hiện đã có một phần:

- Tổng công việc.
- Công việc trễ hạn.
- Summary trạng thái trong màn hình Công việc.

### Nhóm tiến độ

Nên bổ sung:

- Số báo cáo tiến độ theo trạng thái: chờ duyệt, đã duyệt, yêu cầu bổ sung, từ chối.
- Tỷ lệ báo cáo bị từ chối.
- Thời gian duyệt báo cáo tiến độ trung bình.
- Số ngày chậm cập nhật tiến độ.
- Danh sách chi tiết công việc lâu chưa cập nhật.
- Tiến độ theo ngày/tuần/tháng.
- Tiến độ theo nhân viên và theo dự án.

Hiện đã có một phần:

- Các chỉ số tiến độ nằm trong `AI_DATASET`.
- Bộ lọc ngày báo cáo trong màn hình Tiến độ.
- Thống kê tiến độ trong đánh giá dự án/nhân viên.

### Nhóm tài chính

Nên bổ sung:

- Ngân sách được duyệt theo dự án.
- Chi phí thực tế theo dự án.
- Chênh lệch ngân sách/chi phí.
- Tỷ lệ sử dụng ngân sách theo dự án.
- Top dự án vượt ngân sách.
- Chi phí theo tháng/quý/năm.
- Đề xuất ngân sách theo trạng thái.
- Thời gian duyệt đề xuất ngân sách trung bình.

Hiện đã có một phần:

- Tổng ngân sách.
- Tổng chi phí.
- Ngân sách còn lại.
- Tỷ lệ sử dụng ngân sách.
- Chi phí theo dự án.
- Dự án vượt ngân sách.
- Một số feature tài chính trong `AI_DATASET`.

### Nhóm AI

Nên bổ sung:

- Số lần phân tích AI theo thời gian.
- Tỷ lệ kết quả AI đã được quản lý xác nhận.
- Tỷ lệ kết quả AI có thể đã cũ so với AI_DATASET mới nhất.
- Top nguyên nhân trễ phổ biến theo dự án, quản lý, team, thời gian.
- Độ phủ AI_DATASET theo dự án.
- Số dự án trễ chưa xác nhận nguyên nhân.
- Chất lượng model theo phiên bản và cảnh báo dataset lệch lớp.
- Tỷ lệ fallback rule so với model AI.

Hiện đã có một phần:

- Tổng lượt phân tích AI.
- Tổng xác nhận nguyên nhân trong DB.
- Nguyên nhân phổ biến.
- Dự án trễ chưa xác nhận.
- Dataset đủ điều kiện train.
- Biểu đồ model/dataset/feature importance.

### Nhóm đánh giá

Nên bổ sung:

- Điểm trung bình dự án theo tháng/quý/năm.
- Điểm trung bình nhân viên theo tháng/quý/năm.
- Top dự án điểm thấp.
- Top nhân viên điểm cao/thấp.
- Tỷ lệ đánh giá chờ duyệt/từ chối.
- Thời gian từ hoàn thành dự án đến đánh giá.
- Tỷ lệ đánh giá có tham chiếu AI.

Hiện đã có một phần:

- Summary trạng thái đánh giá dự án.
- Summary trạng thái đánh giá nhân viên.
- Thống kê chi tiết hỗ trợ form đánh giá.

## Phần 4. Thống kê theo thời gian

Hệ thống hiện có thống kê theo thời gian nhưng chưa đầy đủ.

Đã có:

- Dashboard tổng quan lọc theo từ ngày/đến ngày.
- Dashboard tổng quan lọc nhanh hôm nay, 7 ngày gần đây, tháng này, quý này, năm nay, tất cả.
- `DashboardService` lọc dự án theo `NgayTaoDuAn`, công việc theo `NgayTaoCongViec`, đề xuất công việc theo `NgayDeXuatCongViec`, đề xuất ngân sách theo `NgayDeXuat`, ngân sách theo `NgayCapNhatNganSach`, chi phí theo `NgayChi`.
- Công việc có lọc từ ngày/đến ngày và loại mốc ngày.
- Dự án có lọc từ ngày/đến ngày và loại mốc ngày.
- Tiến độ có lọc từ ngày/đến ngày báo cáo.
- Đánh giá dự án và đánh giá nhân viên có lọc thời gian.

Chưa có:

- Biểu đồ xu hướng theo ngày/tuần/tháng/quý/năm trên Dashboard tổng quan.
- Thống kê số dự án tạo mới, hoàn thành, trễ hạn theo thời gian.
- Thống kê công việc hoàn thành/trễ hạn theo thời gian.
- Thống kê chi phí theo tháng/quý/năm.
- Thống kê lượt phân tích AI và xác nhận AI theo thời gian.
- Thống kê điểm đánh giá theo thời gian.

Đề xuất áp dụng:

- Dashboard tổng quan: thêm biểu đồ đường theo tháng cho dự án tạo mới, dự án hoàn thành, công việc hoàn thành, công việc trễ, chi phí.
- Dashboard tài chính: dùng theo tháng/quý/năm cho ngân sách, chi phí, chênh lệch.
- Dashboard tiến độ: dùng theo tuần/tháng cho báo cáo tiến độ, trạng thái duyệt, tỷ lệ từ chối.
- Dashboard AI: dùng theo tháng cho lượt phân tích, số xác nhận, số dự án trễ chưa xác nhận, phân bố nguyên nhân.
- Dashboard đánh giá: dùng theo quý/năm cho điểm trung bình dự án và nhân viên.

## Phần 5. Bộ lọc thống kê

### Hiện trạng

Dashboard tổng quan hiện có:

- Từ ngày.
- Đến ngày.
- Lọc nhanh theo ngày/tuần/tháng/quý/năm/tất cả.

Các màn hình nghiệp vụ khác có bộ lọc riêng:

- Dự án: từ khóa, loại dự án, trạng thái, từ ngày, đến ngày, loại mốc ngày.
- Công việc: dự án, trạng thái, từ khóa, từ ngày, đến ngày, loại mốc ngày.
- Tiến độ: dự án, công việc, chi tiết công việc, từ khóa, từ ngày báo cáo, đến ngày báo cáo.
- Ngân sách: dự án, trạng thái.
- Nhân sự: từ khóa, chức danh, trạng thái tài khoản.
- Đánh giá dự án: từ khóa, dự án/trạng thái và thời gian theo form hiện tại.
- Đánh giá nhân viên: từ khóa, dự án, nhân viên/trạng thái và thời gian theo form hiện tại.

### Bộ lọc chung nên bổ sung cho dashboard thống kê

- Từ ngày.
- Đến ngày.
- Trạng thái.
- Dự án.
- Quản lý.
- Nhân viên.
- Team.
- Loại dự án.
- Mốc ngày: ngày tạo, ngày bắt đầu, ngày kết thúc dự kiến, ngày hoàn thành thực tế, ngày chi, ngày duyệt.

### Có nên áp dụng cho tất cả dashboard hay không

Không nên áp dụng toàn bộ bộ lọc cho mọi dashboard. Nên có bộ lọc nền tảng dùng chung và bộ lọc chuyên biệt theo màn hình.

Bộ lọc nền tảng nên dùng chung:

- Từ ngày.
- Đến ngày.
- Dự án.
- Trạng thái.

Bộ lọc chuyên biệt:

- Dashboard dự án: loại dự án, quản lý, team.
- Dashboard công việc/tiến độ: nhân viên, công việc, chi tiết công việc, trạng thái duyệt.
- Dashboard tài chính: dự án, trạng thái ngân sách, loại mốc ngày chi/ngày duyệt.
- Dashboard AI: model, nguyên nhân, trạng thái xác nhận, trạng thái dataset.
- Dashboard đánh giá: trạng thái đánh giá, người đánh giá, dự án, nhân viên.

## Phần 6. Biểu đồ

### Biểu đồ hiện có

Dashboard tổng quan:

- Biểu đồ cột tiến độ các dự án.
- Biểu đồ đường có vùng nền cho chi phí theo dự án.

Tổng quan AI:

- Biểu đồ tròn phân bố nguyên nhân đã xác nhận.
- Biểu đồ cột dataset theo trạng thái train.
- Biểu đồ đường accuracy theo phiên bản model.
- Biểu đồ cột ngang feature importance.

Các màn hình đánh giá và danh sách nghiệp vụ:

- Chủ yếu là card, bảng, filter; chưa thấy biểu đồ thống kê riêng.

### Đánh giá biểu đồ hiện có

- Biểu đồ tiến độ dự án phù hợp với phần trăm hoàn thành, nhưng chỉ lấy 12 dự án mới nhất theo `MaDuAn`, chưa chắc là 12 dự án quan trọng nhất.
- Biểu đồ chi phí theo dự án dùng line chart cho dữ liệu theo danh mục dự án. Nếu trục X là tên dự án, biểu đồ cột sẽ dễ hiểu hơn biểu đồ đường vì không phải chuỗi thời gian.
- Biểu đồ nguyên nhân AI dạng tròn phù hợp với tỷ lệ top nguyên nhân, nhưng cần giới hạn số nhóm và có nhóm "Khác" nếu nhiều nguyên nhân.
- Biểu đồ accuracy theo phiên bản model dùng line chart là phù hợp vì có trục thời gian/phiên bản.
- Feature importance dùng cột ngang là phù hợp.

### Đề xuất biểu đồ theo loại dữ liệu

Biểu đồ cột:

- Dự án theo trạng thái.
- Công việc theo trạng thái.
- Chi phí theo dự án.
- Ngân sách theo dự án.
- Đề xuất chờ duyệt theo loại.
- Số báo cáo tiến độ theo trạng thái.
- Số lần phân tích AI theo tháng.

Biểu đồ đường:

- Dự án tạo mới/hoàn thành theo tháng.
- Công việc hoàn thành/trễ hạn theo tháng.
- Chi phí theo tháng.
- Lượt phân tích AI theo thời gian.
- Điểm trung bình đánh giá theo thời gian.

Biểu đồ tròn:

- Tỷ trọng trạng thái dự án nếu số trạng thái ít.
- Phân bố nguyên nhân trễ phổ biến.
- Tỷ lệ đánh giá theo trạng thái.

Biểu đồ vùng:

- Chi phí lũy kế theo thời gian.
- Tỷ lệ sử dụng ngân sách theo tháng.
- Số lượng công việc tồn đọng theo thời gian.

Biểu đồ tiến độ:

- Thanh tiến độ hoàn thành dự án.
- Thanh tiến độ theo nhóm công việc.
- Gauge hoặc progress bar cho tỷ lệ sử dụng ngân sách.

Biểu đồ ngân sách:

- Cột nhóm ngân sách dự kiến và chi phí thực tế theo dự án.
- Cột chênh lệch chi phí.
- Đường chi phí theo thời gian.

Biểu đồ AI:

- Cột số dự án trễ chưa xác nhận theo quản lý.
- Tròn phân bố nguyên nhân.
- Đường lượt phân tích AI theo tháng.
- Cột ngang feature importance.
- Cột nhóm kết quả AI: đã xác nhận, chưa xác nhận, kết quả có thể đã cũ.

## Phần 7. Xuất file

### Hiện trạng

`ExportFileService` hỗ trợ:

- Excel.
- PDF.
- CSV.

Các module đã có xuất file:

- Dashboard tổng quan.
- Dự án.
- Công việc.
- Chi tiết công việc.
- Tiến độ.
- Ngân sách.
- Nhân sự.
- Đánh giá dự án.
- Đánh giá nhân viên.
- AI Dataset.

AI Model có xuất metadata JSON ở `AiController.ExportMetadata`, không đi qua `ExportFileService`.

### Module nên có Excel

Nên áp dụng cho:

- Dự án.
- Công việc.
- Tiến độ.
- Nhân sự.
- Ngân sách.
- Chi phí.
- Đánh giá dự án.
- Đánh giá nhân viên.
- AI Dataset.
- Dashboard tổng quan.

Excel phù hợp cho dữ liệu dạng bảng và cần lọc/sắp xếp tiếp.

### Module nên có PDF

Nên áp dụng cho:

- Dashboard tổng quan.
- Báo cáo dự án.
- Báo cáo tiến độ.
- Báo cáo ngân sách/chi phí.
- Đánh giá dự án.
- Đánh giá nhân viên.
- Báo cáo AI tổng quan.

PDF phù hợp cho báo cáo gửi quản lý, lưu hồ sơ hoặc in ấn.

### Module nên có CSV

Nên áp dụng cho:

- AI Dataset.
- Dự án.
- Công việc.
- Tiến độ.
- Ngân sách/chi phí.
- Nhân sự.

CSV phù hợp cho phân tích dữ liệu hoặc nhập sang công cụ khác.

### Module không cần hoặc ít cần xuất file

- Chat dự án: không nên xuất đại trà; nếu có chỉ nên xuất lịch sử theo dự án và phải kiểm soát quyền riêng.
- Phân quyền: không cần CSV thường xuyên; nếu xuất nên là Excel/PDF audit quyền.
- Tài khoản cá nhân: không cần xuất file.
- Màn hình thao tác train/predict AI: không cần xuất file thao tác, chỉ cần xuất metadata model hoặc báo cáo AI đã phân tích.

### Điểm cần cải thiện trong xuất file

- Sửa chuỗi không dấu trong `ExportFileService` thành tiếng Việt có dấu.
- Sửa chuỗi mojibake trong `DashboardController.XuatFile`.
- Bổ sung xuất báo cáo Tổng quan AI bằng Excel/PDF/CSV nếu người dùng có `AI.Dashboard` và `ThongKe.XuatFile`.
- Với PDF dashboard nên xuất dạng báo cáo tổng hợp có phần KPI, biểu đồ hoặc bảng tóm tắt, không chỉ bảng ba cột.

## Phần 8. Phân quyền thống kê

### Hiện trạng trong `Permissions.cs`

Đã có:

- `ThongKe.Xem`.
- `ThongKe.XuatFile`.

Không cần thêm lại hai quyền này vì đã tồn tại.

### Hiện trạng sử dụng quyền

- Sidebar dùng `ThongKe.Xem` để hiện Dashboard.
- Nút xuất file dùng `ThongKe.XuatFile`.
- Các action xuất file ở nhiều controller kiểm tra `ThongKe.XuatFile`.
- `DashboardController.XuatFile` kiểm tra `ThongKe.XuatFile`.
- `DashboardController.Index` chưa kiểm tra `ThongKe.Xem` trong controller.
- `AiController.Dashboard` kiểm tra `AI.Dashboard`, không dùng `ThongKe.Xem`.
- `AiDatasetController.Index` kiểm tra `AI.Dataset`.

### Role mặc định

Trong `KhoiTaoTaiKhoanMacDinh.cs`, quyền tối thiểu cho Admin, Manager, Employee đều có:

- `ThongKe.Xem`.
- `ThongKe.XuatFile`.

Admin và Manager có thêm nhiều quyền AI hơn Employee. Employee vẫn có `ThongKe.XuatFile` theo seed hiện tại, cần cân nhắc lại vì xuất toàn hệ thống có thể vượt phạm vi cần thiết.

### Đề xuất phân quyền theo vai trò

Admin:

- Xem toàn bộ Dashboard tổng quan.
- Xem toàn bộ Tổng quan AI.
- Xuất toàn bộ báo cáo.
- Xem thống kê phân quyền, nhân sự, hệ thống.

Manager:

- Xem dashboard theo dự án mình quản lý và dự án mình tham gia.
- Xem thống kê công việc, tiến độ, ngân sách, đánh giá trong phạm vi dự án.
- Xuất báo cáo trong phạm vi dự án được phép.
- Xem AI Dashboard theo phạm vi dự án.
- Xác nhận nguyên nhân AI cho dự án có quyền quản lý.

Leader:

- Nếu hệ thống phân biệt leader theo team/dự án, nên xem thống kê công việc, tiến độ, nhân sự trong team/dự án được phân công.
- Không nên xem ngân sách toàn hệ thống nếu không có quyền tài chính.
- Xuất báo cáo công việc/tiến độ trong phạm vi phụ trách.

Employee:

- Chỉ nên xem thống kê cá nhân: công việc được giao, tiến độ đã gửi, tỷ lệ hoàn thành, báo cáo bị yêu cầu bổ sung/từ chối, đánh giá cá nhân.
- Không nên có quyền xuất toàn bộ Dashboard tổng quan nếu dữ liệu không lọc theo scope.
- Nếu giữ `ThongKe.XuatFile`, controller/service phải lọc dữ liệu theo scope trước khi xuất.

Đề xuất kỹ thuật:

- Thêm kiểm tra `ThongKe.Xem` trực tiếp trong `DashboardController.Index`.
- Tách quyền nếu cần chi tiết hơn: `ThongKe.TongQuan`, `ThongKe.AI`, `ThongKe.TaiChinh`, nhưng không bắt buộc ở giai đoạn đầu vì `ThongKe.Xem` và quyền module hiện đã tồn tại.
- Đảm bảo mọi thống kê và xuất file đều áp dụng scope dữ liệu, không chỉ kiểm tra quyền menu.

## Phần 9. UI/UX đề xuất

### Dashboard tổng quan mong muốn

[BỘ LỌC]

- Từ ngày.
- Đến ngày.
- Dự án.
- Quản lý.
- Team.
- Trạng thái.
- Loại mốc ngày.
- Nút lọc, làm mới, xuất file.

[KPI CARD]

- Tổng dự án.
- Dự án đang thực hiện.
- Dự án hoàn thành.
- Dự án trễ.
- Tổng công việc.
- Công việc trễ.
- Tổng nhân sự.
- Nhân sự quá tải.
- Tổng ngân sách.
- Chi phí thực tế.
- Tỷ lệ sử dụng ngân sách.
- Dự án thiếu dữ liệu AI.

[BIỂU ĐỒ]

- Dự án theo trạng thái: cột hoặc tròn.
- Xu hướng dự án theo tháng: đường.
- Công việc theo trạng thái: cột.
- Chi phí và ngân sách theo dự án: cột nhóm.
- Tiến độ dự án: cột hoặc progress bar.
- Nguyên nhân trễ AI: tròn hoặc cột ngang.

[BẢNG CHI TIẾT]

- Top dự án trễ hạn.
- Top dự án vượt ngân sách.
- Top nhân sự quá tải.
- Top công việc trễ hạn.
- Dự án thiếu dữ liệu AI.
- Đề xuất/yêu cầu chờ duyệt lâu nhất.

[HÀNH ĐỘNG]

- Xem dự án trễ.
- Xem công việc trễ.
- Xem ngân sách vượt.
- Mở dữ liệu AI.
- Mở duyệt đề xuất công việc.
- Mở duyệt ngân sách.
- Mở duyệt yêu cầu đổi quản lý.

### Dashboard AI mong muốn

[KPI CARD]

- Tổng lượt phân tích.
- Tổng xác nhận nguyên nhân.
- Dự án trễ chưa xác nhận.
- Dataset đủ điều kiện train.
- Model active.
- Độ chính xác model active.

[BIỂU ĐỒ]

- Nguyên nhân trễ phổ biến.
- Lượt phân tích AI theo tháng.
- Tỷ lệ xác nhận AI.
- Feature importance.
- Accuracy theo phiên bản model.

[BẢNG CHI TIẾT]

- Dự án trễ chưa xác nhận nguyên nhân.
- Kết quả AI có thể đã cũ.
- Nguyên nhân phổ biến theo quản lý/team.

### Dashboard tài chính mong muốn

[KPI CARD]

- Tổng ngân sách active.
- Tổng chi phí.
- Ngân sách còn lại.
- Dự án vượt ngân sách.
- Đề xuất ngân sách chờ duyệt.

[BIỂU ĐỒ]

- Ngân sách và chi phí theo dự án.
- Chi phí theo tháng.
- Chênh lệch chi phí.

[BẢNG CHI TIẾT]

- Top dự án vượt ngân sách.
- Đề xuất ngân sách chờ duyệt lâu.

### Dashboard tiến độ và công việc mong muốn

[KPI CARD]

- Tổng công việc.
- Đang thực hiện.
- Chờ xác nhận hoàn thành.
- Hoàn thành.
- Bị cản.
- Trễ hạn.

[BIỂU ĐỒ]

- Công việc theo trạng thái.
- Công việc trễ theo dự án.
- Báo cáo tiến độ theo trạng thái.
- Tiến độ theo tháng.

[BẢNG CHI TIẾT]

- Công việc trễ hạn.
- Chi tiết công việc chưa cập nhật.
- Báo cáo tiến độ chờ duyệt.

## Phần 10. Danh sách file cần sửa

### Controller

- `Controllers/DashboardController.cs`
  - Thêm kiểm tra `Permissions.ThongKe.Xem` cho `Index`.
  - Sửa chuỗi tiếng Việt bị lỗi mã hóa trong `XuatFile`.
  - Bổ sung dữ liệu bảng top dự án trễ, top công việc trễ, top vượt ngân sách nếu mở rộng dashboard.

- `Controllers/AiController.cs`
  - Cân nhắc thêm action xuất báo cáo Tổng quan AI.
  - Giữ kiểm tra `AI.Dashboard` và scope dữ liệu hiện có.

- `Controllers/AiDatasetController.cs`
  - Làm rõ hành vi hai nút tổng hợp dataset/tổng hợp lại nếu cần.
  - Cân nhắc giữ xuất CSV/Excel cho dataset và hạn chế PDF nếu bảng quá kỹ thuật.

- `Controllers/DuAnController.cs`
  - Nếu bổ sung dashboard dự án, cần endpoint thống kê theo loại, trạng thái, quản lý, team, thời gian.

- `Controllers/CongViecController.cs`
  - Nếu bổ sung dashboard công việc, cần endpoint thống kê theo trạng thái, nhân viên, dự án, thời gian.

- `Controllers/TienDoCongViecController.cs`
  - Nếu bổ sung dashboard tiến độ, cần endpoint thống kê báo cáo tiến độ theo trạng thái, thời gian duyệt, tỷ lệ từ chối.

- `Controllers/NganSachController.cs`
  - Nếu bổ sung dashboard tài chính, cần endpoint thống kê ngân sách, chi phí, chênh lệch theo dự án/tháng.

- `Controllers/DanhGiaDuAnController.cs`
  - Nếu bổ sung dashboard đánh giá, cần thống kê điểm trung bình, trạng thái đánh giá, top dự án điểm thấp.

- `Controllers/DanhGiaNhanVienController.cs`
  - Nếu bổ sung dashboard đánh giá nhân viên, cần thống kê điểm trung bình, top nhân viên, tỷ lệ hoàn thành theo thời gian.

### Service

- `Services/Implementations/DashboardService.cs`
  - Bổ sung dữ liệu xu hướng theo thời gian.
  - Bổ sung thống kê theo dự án/quản lý/team/trạng thái.
  - Bổ sung top cảnh báo chi tiết.
  - Tạo dữ liệu thật cho `WorkflowHealthItems` và `Suggestions` hoặc bỏ khỏi view nếu chưa dùng.
  - Rà lại logic `DuAnDungTienDo` hiện đang tính là mọi trạng thái không phải `TreTienDo`, trong khi trạng thái dự án chuẩn không thấy nhóm `TreTienDo` nổi bật trong workflow.

- `Services/Interfaces/IDashboardService.cs`
  - Cập nhật chữ ký nếu thêm bộ lọc dự án, quản lý, nhân viên, team, trạng thái.

- `Services/Implementations/AiService.cs`
  - Bổ sung thống kê AI theo thời gian, tỷ lệ xác nhận, tỷ lệ fallback, kết quả có thể đã cũ.

- `Services/Implementations/AiDatasetService.cs`
  - Bổ sung thống kê độ phủ dataset theo dự án và thời gian tổng hợp gần nhất.

- `Services/Implementations/ExportFileService.cs`
  - Sửa metadata xuất file sang tiếng Việt có dấu.
  - Cân nhắc template PDF riêng cho dashboard tổng quan thay vì bảng thuần.

- Các service nghiệp vụ liên quan
  - `DuAnService.cs`, `CongViecService.cs`, `TienDoCongViecService.cs`, `NganSachService.cs`, `DanhGiaDuAnService.cs`, `DanhGiaNhanVienService.cs` cần thêm hàm thống kê nếu không muốn dồn hết vào `DashboardService`.

### ViewModel

- `ViewModels/Dashboard/DashboardViewModel.cs`
  - Thêm danh sách dữ liệu xu hướng theo thời gian.
  - Thêm nhóm thống kê top dự án/công việc/nhân sự/ngân sách.
  - Thêm bộ lọc dự án, quản lý, team, trạng thái.
  - Thêm dữ liệu biểu đồ phân bố trạng thái.

- `ViewModels/Dashboard/WorkflowHealthItemViewModel.cs`
  - Nếu tiếp tục dùng, cần service trả dữ liệu thật.

- `ViewModels/Dashboard/SuggestionItemViewModel.cs`
  - Nếu tiếp tục dùng, cần service trả gợi ý thật dựa trên cảnh báo.

- `ViewModels/Ai/AiDashboardPageViewModel.cs`
  - Thêm thống kê theo thời gian, tỷ lệ xác nhận, tỷ lệ kết quả cũ/fallback nếu mở rộng AI Dashboard.

- `ViewModels/Ai/AiDashboardViewModel.cs`
  - Nếu Dashboard tổng quan tiếp tục nhúng AI, nên thêm các chỉ số nghiệp vụ AI ngắn gọn thay vì chỉ `SoCanhBaoDo`.

### View

- `Views/Dashboard/Index.cshtml`
  - Sửa điều hướng yêu cầu đổi quản lý chờ duyệt để trỏ đúng `DuyetYeuCauDoiQuanLy`.
  - Bổ sung bảng top cảnh báo.
  - Bổ sung biểu đồ trạng thái và biểu đồ xu hướng thời gian.
  - Bỏ hoặc ẩn phần gợi ý ưu tiên nếu service chưa có dữ liệu thật.
  - Đổi "Dashboard" thành "Thống kê" nếu muốn thuần tiếng Việt.

- `Views/Ai/Dashboard.cshtml`
  - Bổ sung bộ lọc thời gian/model/nguyên nhân nếu cần.
  - Bổ sung xuất báo cáo AI.
  - Bổ sung biểu đồ lượt phân tích theo thời gian và tỷ lệ xác nhận.

- `Views/AiDataset/Index.cshtml`
  - Làm rõ hai nút tổng hợp dataset/tổng hợp lại.
  - Tách phần kỹ thuật khỏi phần cảnh báo nghiệp vụ.

- `Views/DanhGiaDuAn/*`
  - Bổ sung biểu đồ điểm trung bình và trạng thái đánh giá nếu mở dashboard đánh giá.

- `Views/DanhGiaNhanVien/*`
  - Bổ sung biểu đồ điểm trung bình, hiệu suất và trạng thái đánh giá nếu mở dashboard đánh giá nhân viên.

- `Views/Shared/_Layout.cshtml`
  - Đổi nhãn sidebar "Dashboard" thành "Thống kê tổng quan" hoặc "Tổng quan" nếu muốn thống nhất tiếng Việt.
  - Cân nhắc tách menu "Thống kê" thành nhóm riêng nếu có nhiều dashboard con.

### CSS

- `wwwroot/css/Dashboard/index.css`
  - Giảm bo góc/khoảng trắng nếu muốn dashboard vận hành gọn hơn.
  - Thêm style cho bảng top cảnh báo và bộ lọc nâng cao.
  - Đồng bộ button với `_ExportDropdown`.

- `wwwroot/css/AI/index.css`
  - Bổ sung style cho bộ lọc dashboard AI và trạng thái xuất file nếu thêm.

- `wwwroot/css/site.css`
  - Tách token màu/spacing/card chung để tránh mỗi module tự định nghĩa lại.

- `wwwroot/css/layout/sidebar.css`
  - Cập nhật nhãn/active style nếu thêm nhóm menu Thống kê.

- CSS module đánh giá và nghiệp vụ
  - `DanhGiaDuAn/*`, `DanhGiaNhanVien/*`, `CongViec/index.css`, `DuAn/index.css`, `NganSach/index.css`, `TienDoCongViec/index.css` cần chuẩn hóa card/filter/table nếu muốn giao diện dashboard đồng bộ.

### JavaScript

- `wwwroot/js/ai/charts.js`
  - Có thể mở rộng helper render biểu đồ dùng chung cho Dashboard tổng quan, không chỉ AI.

- `Views/Dashboard/Index.cshtml`
  - Nên tách script Chart.js inline ra file JS riêng nếu dashboard mở rộng nhiều biểu đồ.

- Thư viện biểu đồ
  - Hiện dùng Chart.js từ CDN.
  - Không thấy ApexCharts hoặc HighCharts trong source liên quan.
  - Nên tiếp tục dùng Chart.js để tránh thêm thư viện mới nếu nhu cầu vẫn là cột, đường, tròn, vùng.

## Phần 11. Kết luận

### Những gì đang tốt

- Hệ thống đã có Dashboard tổng quan thực sự, không chỉ là trang rỗng.
- Dashboard tổng quan đã tổng hợp nhiều nhóm dữ liệu quan trọng: dự án, công việc, nhân sự, ngân sách, đề xuất, cảnh báo, AI.
- Tổng quan AI có nhiều chỉ số và biểu đồ tốt cho quản trị model/dataset.
- Cơ chế xuất file dùng chung đã hỗ trợ Excel, PDF, CSV và được áp dụng ở nhiều module.
- Bộ lọc thời gian đã xuất hiện ở Dashboard tổng quan và nhiều màn hình nghiệp vụ.
- Quyền `ThongKe.Xem` và `ThongKe.XuatFile` đã tồn tại trong `Permissions.cs` và được dùng trong sidebar/xuất file.

### Những gì cần cải thiện

- Cần chặn quyền `ThongKe.Xem` trực tiếp trong `DashboardController.Index`.
- Cần sửa lỗi mã hóa tiếng Việt trong `DashboardController.XuatFile`.
- Cần sửa chuỗi không dấu trong `ExportFileService`.
- Cần bổ sung dữ liệu thật hoặc bỏ khu vực "Gợi ý ưu tiên" đang rỗng.
- Cần sửa điều hướng yêu cầu đổi quản lý chờ duyệt trên Dashboard tổng quan.
- Cần bổ sung bảng chi tiết top cảnh báo, vì dashboard hiện có nhiều số đếm nhưng ít dữ liệu hành động.
- Cần bổ sung thống kê theo thời gian bằng biểu đồ đường/vùng.
- Cần thống nhất style giữa Dashboard, AI, đánh giá và các module nghiệp vụ.
- Cần rà lại quyền xuất file của Employee vì hiện seed tối thiểu có `ThongKe.XuatFile`.

### Đề xuất triển khai tối ưu

Giai đoạn 1 nên làm các chỉnh sửa ít rủi ro:

- Sửa quyền xem Dashboard ở controller.
- Sửa lỗi mã hóa tiếng Việt trong xuất file.
- Sửa điều hướng duyệt yêu cầu đổi quản lý.
- Bỏ hoặc cấp dữ liệu thật cho gợi ý ưu tiên.
- Bổ sung bảng top cảnh báo trên Dashboard tổng quan.

Giai đoạn 2 nên mở rộng thống kê:

- Thêm bộ lọc dự án, quản lý, team, trạng thái.
- Thêm biểu đồ dự án/công việc/ngân sách theo thời gian.
- Thêm biểu đồ trạng thái dự án và công việc.
- Thêm thống kê AI theo thời gian và tỷ lệ xác nhận.

Giai đoạn 3 nên chuẩn hóa hệ thống báo cáo:

- Tách dashboard theo nhóm: tổng quan, dự án, công việc/tiến độ, tài chính, AI, đánh giá.
- Dùng chung component CSS cho card, filter, chart, table.
- Chuẩn hóa scope dữ liệu cho Manager, Leader, Employee khi xem và xuất file.
- Thiết kế mẫu PDF riêng cho dashboard thay vì dùng bảng xuất file chung cho mọi báo cáo.
