# Phân tích thêm NgayHoanThanhThucTeDuAn

## 1. Mục tiêu và định nghĩa dữ liệu mới
- `NgayHoanThanhThucTeDuAn` là ngày hoàn thành thực tế của dự án, khác với `NgayKetThucDuAn` (dự kiến).
- Cần thống nhất quy tắc gán: thường gán khi quản lý xác nhận hoàn thành; khi mở lại dự án thì xóa giá trị.
- Nếu cần dữ liệu lịch sử, có thể backfill theo ngày hoàn thành công việc muộn nhất, hiện đang được suy ra trong đánh giá dự án ở [QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs](QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs) và hiển thị ở [QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_ProjectStats.cshtml](QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_ProjectStats.cshtml).

## 2. Tầng dữ liệu và EF Core
- Bảng DU_AN trong [quanlyduan.sql](quanlyduan.sql) chưa có cột này, nên schema chưa hỗ trợ lưu ngày hoàn thành thực tế.
- Entity dự án hiện chưa có thuộc tính tương ứng trong [QuanLyDuAn/QuanLyDuAn/Models/Entities/DuAn.cs](QuanLyDuAn/QuanLyDuAn/Models/Entities/DuAn.cs).
- DbContext cấu hình DU_AN hiện chỉ map các cột hiện hữu trong [QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs](QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs).

## 3. Luồng nghiệp vụ và dịch vụ
- Xác nhận hoàn thành dự án hiện chỉ đổi trạng thái, chưa set ngày hoàn thành thực tế trong [QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs](QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs) và action gọi ở [QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs](QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs).
- Mở lại dự án hiện chỉ chuyển trạng thái; nếu có ngày hoàn thành thực tế thì cần xóa để tránh mâu thuẫn trạng thái trong [QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs](QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs).
- Luồng tự động đồng bộ trạng thái dự án từ công việc (hoàn thành -> chờ xác nhận) nằm ở [QuanLyDuAn/QuanLyDuAn/Services/Implementations/TrangThaiWorkflowService.cs](QuanLyDuAn/QuanLyDuAn/Services/Implementations/TrangThaiWorkflowService.cs) và các service công việc ở [QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs](QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs). Quy tắc đặt ngày hoàn thành thực tế cần quyết định ở bước xác nhận hay ở bước hoàn tất toàn bộ công việc.
- Đánh giá dự án đang suy ra ngày hoàn thành thực tế bằng max ngày hoàn thành công việc trong [QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs](QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs). Khi có trường mới, cần ưu tiên dùng trường này và fallback về cách tính cũ nếu null.
- Dashboard tổng quan có chỉ số “Dự án hoàn thành trong kỳ” tại [QuanLyDuAn/QuanLyDuAn/Services/Implementations/DashboardService.cs](QuanLyDuAn/QuanLyDuAn/Services/Implementations/DashboardService.cs). Nếu KPI này nên phản ánh “hoàn thành thực tế” thì cần chuyển từ `NgayKetThucDuAn` sang `NgayHoanThanhThucTeDuAn`.

## 4. ViewModel và mapping dữ liệu
- Các ViewModel dự án hiện chỉ mang ngày kết thúc dự kiến, ví dụ [QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnViewModel.cs](QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnViewModel.cs), [QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnChiTietViewModel.cs](QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnChiTietViewModel.cs), [QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnCreateUpdateViewModel.cs](QuanLyDuAn/QuanLyDuAn/ViewModels/DuAn/DuAnCreateUpdateViewModel.cs). Nếu cần hiển thị/chỉnh sửa ngày hoàn thành thực tế thì phải bổ sung trường và mapping.
- ViewModel thống kê đánh giá đã có `NgayKetThucThucTeDuAn` ở [QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnThongKeViewModel.cs](QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnThongKeViewModel.cs), nhưng dữ liệu hiện lấy từ công việc. Khi thêm cột mới, mapping cần điều chỉnh.

## 5. Controller và xuất báo cáo
- Action xác nhận hoàn thành và mở lại dự án ở [QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs](QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs) là điểm gọi nghiệp vụ chính để set/clear ngày hoàn thành thực tế.
- Xuất file danh sách dự án hiện chỉ có cột ngày kết thúc dự kiến trong [QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs](QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs). Nếu thêm ngày hoàn thành thực tế thì cần bổ sung cột export.
- Xuất AI dataset ở [QuanLyDuAn/QuanLyDuAn/Controllers/AiDatasetController.cs](QuanLyDuAn/QuanLyDuAn/Controllers/AiDatasetController.cs) không bị ảnh hưởng trừ khi thêm feature mới từ ngày hoàn thành thực tế.

## 6. Giao diện (Views)
- Danh sách và chi tiết dự án chỉ hiển thị ngày kết thúc dự kiến trong [QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Table.cshtml](QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Table.cshtml) và [QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml](QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml). Nếu cần hiển thị ngày hoàn thành thực tế thì phải bổ sung UI.
- Khối thống kê đánh giá dự án đã hiển thị “Ngày kết thúc thực tế” trong [QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_ProjectStats.cshtml](QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_ProjectStats.cshtml), nhưng hiện lấy dữ liệu suy ra từ công việc.
- Dashboard tổng quan ở [QuanLyDuAn/QuanLyDuAn/Views/Dashboard/Index.cshtml](QuanLyDuAn/QuanLyDuAn/Views/Dashboard/Index.cshtml) đang hiển thị số dự án hoàn thành trong kỳ theo dữ liệu service; nếu đổi logic tính toán sẽ ảnh hưởng con số này.
- Mô tả feature trong màn hình AI dataset có đề cập `SoNgayChamCapNhatTienDo` dùng `NgayKetThucDuAn` tại [QuanLyDuAn/QuanLyDuAn/Views/AiDataset/Index.cshtml](QuanLyDuAn/QuanLyDuAn/Views/AiDataset/Index.cshtml). Nếu chuyển sang dùng ngày hoàn thành thực tế thì cần cập nhật mô tả.

## 7. AI dataset và FastAPI
- Dataset contract hiện không có trường ngày hoàn thành thực tế trong [QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiDatasetApiViewModels.cs](QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiDatasetApiViewModels.cs), và danh sách feature cố định ở [QuanLyDuAnAIService/app/constants.py](QuanLyDuAnAIService/app/constants.py) + schema ở [QuanLyDuAnAIService/app/schemas.py](QuanLyDuAnAIService/app/schemas.py) + build logic ở [QuanLyDuAnAIService/app/ml/feature_builder.py](QuanLyDuAnAIService/app/ml/feature_builder.py).
- Nếu muốn đưa `NgayHoanThanhThucTeDuAn` vào feature hoặc thay đổi cách tính `SoNgayChamCapNhatTienDo`, cần đồng bộ MVC ↔ FastAPI và cập nhật sample data trong [QuanLyDuAnAIService/sample_data](QuanLyDuAnAIService/sample_data).

## 8. SQL seed và dữ liệu mẫu
- Schema hiện tại không có cột mới trong [quanlyduan.sql](quanlyduan.sql).
- Các script seed chèn vào DU_AN có danh sách cột cố định, như [seed-demo-data-simple.sql](seed-demo-data-simple.sql), [seed-demo-data-ai-expanded.sql](seed-demo-data-ai-expanded.sql), [seed-ai-training-extra.sql](seed-ai-training-extra.sql). Nếu thêm cột mà không có default, các insert/update này sẽ cần cập nhật.
- Backfill dữ liệu lịch sử (nếu cần) có thể dùng max `NgayKetThucCVThucTe` từ công việc làm giá trị ban đầu để đồng bộ với logic đánh giá hiện hữu, nhưng đây là đề xuất nghiệp vụ, không thực hiện trong phạm vi yêu cầu “không sửa SQL”.

## 9. Bảng tổng hợp và kết luận

| Khu vực | Tệp liên quan | Lý do cần xem xét | Ghi chú |
| --- | --- | --- | --- |
| Schema DB | [quanlyduan.sql](quanlyduan.sql) | Thiếu cột lưu ngày hoàn thành thực tế | Cần thống nhất kiểu dữ liệu và nullability |
| Entity + DbContext | [QuanLyDuAn/QuanLyDuAn/Models/Entities/DuAn.cs](QuanLyDuAn/QuanLyDuAn/Models/Entities/DuAn.cs), [QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs](QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs) | Chưa có property/map cho trường mới | Bổ sung để EF Core tracking |
| Nghiệp vụ dự án | [QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs](QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs), [QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs](QuanLyDuAn/QuanLyDuAn/Controllers/DuAnController.cs) | Xác nhận hoàn thành/mở lại là điểm set/clear dữ liệu | Quy tắc gán cần rõ ràng |
| Đánh giá dự án | [QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs](QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs), [QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnThongKeViewModel.cs](QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnThongKeViewModel.cs), [QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_ProjectStats.cshtml](QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_ProjectStats.cshtml) | Đang suy ra ngày hoàn thành từ công việc | Ưu tiên trường mới, fallback nếu null |
| Dashboard | [QuanLyDuAn/QuanLyDuAn/Services/Implementations/DashboardService.cs](QuanLyDuAn/QuanLyDuAn/Services/Implementations/DashboardService.cs), [QuanLyDuAn/QuanLyDuAn/Views/Dashboard/Index.cshtml](QuanLyDuAn/QuanLyDuAn/Views/Dashboard/Index.cshtml) | KPI “hoàn thành trong kỳ” có thể cần dựa vào ngày thực tế | Cần thống nhất định nghĩa KPI |
| UI dự án | [QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Table.cshtml](QuanLyDuAn/QuanLyDuAn/Views/DuAn/_Table.cshtml), [QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml](QuanLyDuAn/QuanLyDuAn/Views/DuAn/Details.cshtml) | Chưa hiển thị ngày hoàn thành thực tế | Tùy yêu cầu hiển thị |
| AI dataset | [QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs](QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs), [QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiDatasetApiViewModels.cs](QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiDatasetApiViewModels.cs), [QuanLyDuAnAIService/app/constants.py](QuanLyDuAnAIService/app/constants.py), [QuanLyDuAnAIService/app/schemas.py](QuanLyDuAnAIService/app/schemas.py), [QuanLyDuAnAIService/app/ml/feature_builder.py](QuanLyDuAnAIService/app/ml/feature_builder.py) | Feature hiện chưa chứa trường mới | Cần đồng bộ MVC ↔ FastAPI nếu bổ sung |
| Seed dữ liệu | [seed-demo-data-simple.sql](seed-demo-data-simple.sql), [seed-demo-data-ai-expanded.sql](seed-demo-data-ai-expanded.sql), [seed-ai-training-extra.sql](seed-ai-training-extra.sql) | Insert/update DU_AN dùng danh sách cột cố định | Cần cập nhật nếu thêm cột không default |

Kết luận:
- `NgayHoanThanhThucTeDuAn` chạm tới nhiều tầng: schema, EF, dịch vụ nghiệp vụ, thống kê đánh giá, dashboard và khả năng mở rộng AI dataset.
- Điểm quyết định quan trọng là quy tắc gán/clear giá trị (xác nhận hoàn thành hay hoàn tất công việc tự động), vì nó ảnh hưởng KPI và đánh giá trễ hạn.
- Theo yêu cầu, báo cáo này chỉ phân tích phạm vi ảnh hưởng, không thực hiện chỉnh sửa code, migration hay SQL.