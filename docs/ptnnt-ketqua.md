# KẾT QUẢ CHỈNH SỬA LUỒNG PHÂN TÍCH NGUYÊN NHÂN TRỄ

## 1. Kết quả tổng quan

Đã hoàn thành:

- Bỏ mục **Phân tích nguyên nhân trễ** khỏi sidebar nhưng giữ các mục AI khác.
- Lấy `DuAn/Details/{id}#phan-tich-nguyen-nhan-tre` làm nơi duy nhất chạy, chạy lại và xác nhận nguyên nhân.
- Cho phép Manager chọn nguyên nhân thực tế khác gợi ý AI.
- Chuyển đánh giá dự án thành nơi chỉ đọc kết quả AI.
- Giữ `/Ai/Predict` dưới dạng route redirect/compatibility wrapper, không render view cũ.
- Chuyển công cụ `TestPredict` sang `Ai/Models` và đổi quyền backend thành `AI.Train`.
- Xóa view Predict và các endpoint/property đánh giá dùng để chạy hoặc xác nhận AI.
- Build solution thành công.

## 2. Danh sách file đã sửa

| STT | File | Nội dung chỉnh sửa |
| --- | --- | --- |
| 1 | `Controllers/AiController.cs` | GET Predict redirect sang Details/Index; POST Predict và xác nhận cũ trở thành compatibility wrapper; TestPredict chuyển sang Models và dùng `AI.Train` |
| 2 | `Controllers/DanhGiaDuAnController.cs` | Xóa action phân tích/xác nhận AI; thêm anti-forgery cho các POST đánh giá còn lại |
| 3 | `Services/Implementations/AiService.cs` | Cho phép xác nhận khác dự đoán; kiểm tra có kết quả chính thức theo đúng dataset; cấp danh mục nguyên nhân cho Details; cấp dữ liệu test cho Models |
| 4 | `Services/Implementations/DanhGiaDuAnService.cs` | Xóa luồng chạy/xác nhận AI và logic permission OR sai; giữ việc đọc kết quả AI |
| 5 | `Services/Interfaces/IAiService.cs` | Xóa API khởi tạo trang Predict và wrapper dự đoán không còn consumer |
| 6 | `Services/Interfaces/IDanhGiaDuAnService.cs` | Xóa API phân tích/xác nhận AI tại đánh giá |
| 7 | `ViewModels/Ai/AiDelayAnalysisViewModels.cs` | Bổ sung danh mục nguyên nhân cho panel Details |
| 8 | `ViewModels/Ai/AiModelPageViewModel.cs` | Bổ sung input/kết quả thử model |
| 9 | `ViewModels/Ai/AiPredictPageViewModel.cs` | Giữ lại phần feature input còn được pipeline/test model sử dụng; xóa state chỉ phục vụ view Predict |
| 10 | `ViewModels/DanhGiaDuAn/DanhGiaDuAnFormViewModel.cs` | Xóa state/form xác nhận AI |
| 11 | `ViewModels/DanhGiaDuAn/DanhGiaDuAnThongKeViewModel.cs` | Xóa state chạy/tự chạy AI; giữ dữ liệu read-only |
| 12 | `ViewModels/DanhGiaDuAn/DanhGiaDuAnAiInsightViewModel.cs` | Xóa vì không còn consumer AJAX |
| 13 | `Views/Shared/_Layout.cshtml` | Bỏ menu Predict và sửa điều kiện để nhóm AI không rỗng |
| 14 | `Views/Ai/Models.cshtml` | Thêm khu thử model, anti-forgery và hiển thị kết quả test |
| 15 | `Views/Ai/Predict.cshtml` | Xóa vì route không còn render view |
| 16 | `Views/DuAn/Details.cshtml` | Thêm select nguyên nhân thực tế; hiển thị xác nhận; giữ anchor |
| 17 | `Views/DanhGiaDuAn/_Form.cshtml` | Xóa form ẩn, form xác nhận và toàn bộ JavaScript AJAX/auto-analyze |
| 18 | `Views/DanhGiaDuAn/_AiInsightCard.cshtml` | Rút thành card read-only ngắn gọn |
| 19 | `wwwroot/css/DuAn/details.css` | Bố trí select xác nhận trong panel AI |

## 3. Luồng mới sau chỉnh sửa

### Chi tiết dự án

- `DuAn/Details/{id}` đọc panel qua `AiService.LayPhanTichNguyenNhanDuAnAsync`.
- Chỉ Manager hiện tại có `AI.PhanTichNguyenNhan` và dự án đủ điều kiện mới thấy/chạy phân tích.
- Phân tích vẫn gọi duy nhất `AiService.PhanTichNguyenNhanDuAnAsync`.
- Phân tích tạm thời không ghi `AI_KET_QUA`; phân tích chính thức tiếp tục ghi theo logic hiện tại.
- Manager có `AI.XacNhan` chọn một nguyên nhân hợp lệ rồi xác nhận/thay đổi ngay tại Details.

### Đánh giá dự án

- Chỉ đọc kết quả AI mới nhất và xác nhận Manager.
- Không có nút chạy AI, form ẩn, AJAX, auto-analyze hoặc form xác nhận.
- Mở form đánh giá không gọi pipeline phân tích và không tạo `AI_KET_QUA`.

### Route cũ

- GET `/Ai/Predict?maDuAn=x` yêu cầu `AI.PhanTichNguyenNhan`, sau đó redirect đến `/DuAn/Details/x#phan-tich-nguyen-nhan-tre`.
- GET `/Ai/Predict` không có mã dự án redirect đến `DuAn/Index`.
- POST Predict và xác nhận cũ còn giữ làm compatibility wrapper, gọi service chung rồi redirect về Details; không phụ thuộc view Predict.

### Test model

- `TestPredict` nằm trong `Ai/Models`.
- Controller yêu cầu `AI.Train`.
- POST có anti-forgery.
- Dữ liệu feature được lấy từ dataset dự án đã chọn.
- Endpoint FastAPI `/admin/model/test-reason` và quy tắc không ghi `AI_KET_QUA` được giữ nguyên.

### Dashboard

`Ai/Dashboard` và quyền `AI.Dashboard` không thay đổi.

## 4. Permission sau chỉnh sửa

| Hành động | Permission | Scope/backend |
| --- | --- | --- |
| Xem kết quả AI tại chi tiết | `DuAn.Xem` | Có quyền xem dự án |
| Xem AI trong đánh giá | Quyền mở/xem form đánh giá | Manager đúng phạm vi dự án |
| Chạy/chạy lại AI | `AI.PhanTichNguyenNhan` | Đồng thời là Manager hiện tại; Admin bị chặn |
| Xác nhận/thay đổi nguyên nhân | `AI.XacNhan` | Đồng thời là Manager hiện tại; dự án hoàn thành trễ; có dataset/kết quả chính thức |
| Thử/quản trị model | `AI.Train` | Theo quyền quản trị AI |
| Xem Dashboard | `AI.Dashboard` | Theo scope Dashboard hiện tại |

Không còn biểu thức `Manager || AI.XacNhan` trong luồng đánh giá. Các điều kiện AND quan trọng tiếp tục được kiểm tra riêng tại backend.

## 5. Logic xác nhận nguyên nhân

`AiService.XacNhanNguyenNhanAsync` hiện kiểm tra:

1. Có `AI.XacNhan`.
2. Là Manager, không phải Admin.
3. Là Manager hiện tại của dự án.
4. Dự án hoàn thành trễ.
5. Dataset mới nhất có `LaDuAnTre = true`.
6. Có ít nhất một `AI_KET_QUA` của đúng dự án và đúng dataset hiện tại.
7. Mã Manager chọn tồn tại trong `DM_NGUYEN_NHAN`.

Không còn yêu cầu mã Manager chọn phải bằng `AI_KET_QUA.MaDMNguyenNhan`.

Nguyên nhân cuối cùng được thêm/cập nhật trong `AI_NGUYEN_NHAN`, đồng bộ sang `AI_DATASET.MaDMNguyenNhan`; lịch sử `AI_KET_QUA` không bị sửa hoặc xóa.

## 6. Thành phần đã xóa hoặc giữ tương thích

### Đã xóa

- Menu `Ai/Predict`.
- `Views/Ai/Predict.cshtml`.
- Action phân tích và xác nhận AI trong `DanhGiaDuAnController`.
- API service tương ứng trong `IDanhGiaDuAnService`/`DanhGiaDuAnService`.
- Form ẩn, AJAX, auto-analyze và form xác nhận tại đánh giá.
- `DanhGiaDuAnAiInsightViewModel` không còn consumer.
- `KhoiTaoTrangPredictAsync`, `DuDoanDuAnAsync` không còn consumer.

### Giữ tương thích

- GET/POST `AiController.Predict`.
- `AiController.XacNhanNguyenNhan`.
- `AiPredictPageViewModel` được giữ như feature-input DTO vì pipeline test model và mapping 22 feature vẫn dùng; không còn là page state cho view Predict.

### Phải giữ

- Ba method pipeline `PhanTichNguyenNhanDuAnAsync`, `LayPhanTichNguyenNhanDuAnAsync`, `XacNhanNguyenNhanAsync`.
- Dashboard, dataset, train/model, kết quả và lịch sử AI.

## 7. Kết quả build và kiểm thử

### Build

Lệnh:

```powershell
dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore
```

Kết quả:

- Build succeeded.
- `0 Error(s)`.
- `2 Warning(s)` cũ tại `FileTienDoCongViecService.cs` về async không có `await`; không phát sinh từ thay đổi này.

### Kiểm tra tĩnh đã thực hiện

- Không còn reference `PhanTichAiDuAn`, `TuDongPhanTichAi`, `CoThePhanTichAi`, form xác nhận AI tại đánh giá hoặc render `View("Predict")`.
- `Views/Ai/Predict.cshtml` không còn tồn tại.
- TestPredict chỉ được gọi từ Models và backend kiểm tra `AI.Train`.
- Razor Views được compile trong build.
- Không có file FastAPI hoặc SQL nào có thời gian sửa trong phiên triển khai.

### Kiểm thử runtime

Đã thử khởi động:

```powershell
dotnet run --no-build --project QuanLyDuAn/QuanLyDuAn/QuanLyDuAn.csproj --urls http://127.0.0.1:5098
```

Ứng dụng chưa thể khởi động trong môi trường hiện tại do không mở được kết nối SQL Server `LAPTOP-SI5JBDIU\SQLEXPRESS01`; quá trình ghi lỗi tiếp tục bị chặn bởi quyền Windows Event Log (`Access is denied`). Vì vậy chưa thể xác minh redirect và giao diện bằng trình duyệt/runtime trong lượt này. Không thay đổi cấu hình, database hoặc logging để né lỗi môi trường.

Workspace không được Git nhận dạng là repository dù có mục `.git`, nên không có `git diff`; phạm vi được kiểm tra bằng danh sách file đã sửa, tìm reference, build và kiểm tra thời gian file SQL/FastAPI.

## 8. Kiểm tra UTF-8

- Tất cả file source/view/CSS còn tồn tại trong danh sách thay đổi đã được strict-decode UTF-8 thành công.
- Không phát hiện các marker mojibake được yêu cầu trong các file đã sửa.
- Tiếng Việt mới thêm giữ đầy đủ dấu.
- `docs/ptnnt-ketqua.md` được tạo bằng UTF-8.

## 9. Những phần không thay đổi

Không sửa:

- Database, schema, entity mapping hoặc migration.
- File SQL/seed SQL và dữ liệu hiện có.
- FastAPI router, schema, service hoặc model.
- Bộ 22 feature và contract request/response.
- Endpoint `/analyze/delay-reason`, `/admin/model/test-reason`.
- Dataset/train pipeline, threshold, Decision Tree hoặc rule fallback.
- Các permission AI và runtime seed.
- Workflow dự án và workflow duyệt đánh giá ngoài việc loại bỏ hành động ghi AI khỏi màn hình đánh giá.
