# PHÂN TÍCH CHỈNH SỬA DASHBOARD VÀ GIAO DIỆN THỬ MODEL NGUYÊN NHÂN

## 1. Mục tiêu phân tích

Tài liệu này phân tích hai vấn đề giao diện theo source hiện tại:

1. Nhãn tên dự án dài làm hai biểu đồ “Tiến độ dự án” và “Chi phí theo dự án” khó đọc.
2. Khu vực “Thử model nguyên nhân” bị tràn tên model và đang đặt phần trăm confidence làm nội dung chính, dễ bị hiểu nhầm là accuracy của toàn model.

Phạm vi hiện tại chỉ đọc source và đề xuất. Chưa sửa code, CSDL, migration, model, thuật toán hay contract MVC–FastAPI.

## 2. Kiến trúc và luồng dữ liệu liên quan

### Luồng Dashboard

`DashboardService.GetDashboardAsync` → `DashboardController.Index` → `DashboardViewModel` → `Views/Dashboard/Index.cshtml` → JavaScript inline dùng Chart.js → canvas biểu đồ.

MVC áp dụng quyền và phạm vi dự án, lọc dữ liệu, tính tiến độ/chi phí và tạo ba danh sách song song `TenDuAn`, `PhanTramTienDo`, `ChiPhiTheoDuAn`. Razor chỉ tuần tự hóa các danh sách sang JavaScript.

### Luồng thử model

`Views/Ai/Models.cshtml` → `AiController.TestPredict` → `AiService.TestPredictAsync` → `AiApiService.TestPredictAsync` → `POST /admin/model/test-reason` → `model_service.test_predict` → `PredictionService.test_predict` → JSON response → `AiTestReasonResponseViewModel` → `AiModelPageViewModel.KetQuaTestPhanTich` → Razor.

MVC là system-of-record: lấy snapshot `AI_DATASET`, danh mục nguyên nhân, kiểm tra dự án trễ và hậu kiểm kết quả. FastAPI chỉ nạp model, tính dự đoán/confidence và trả response; endpoint thử không ghi SQL Server. Nhánh thử model không gọi `LuuKetQuaPhanTichAsync`, nên không tạo `AI_KET_QUA`.

## 3. Hiện trạng biểu đồ Dashboard

### 3.1 File và thành phần liên quan

| Thành phần | Đường dẫn file | Class/phương thức/biến | Vai trò |
|---|---|---|---|
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/DashboardController.cs` | `DashboardController.Index` | Kiểm tra `Permissions.ThongKe.Xem`, gọi service, trả View |
| Interface | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDashboardService.cs` | `GetDashboardAsync` | Contract lấy Dashboard theo bộ lọc |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DashboardService.cs` | `GetDashboardAsync`, `projectsForChart` | Áp dụng scope/filter, lấy tối đa 12 dự án, tổng hợp tiến độ và chi phí |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/Dashboard/DashboardViewModel.cs` | `TenDuAn`, `PhanTramTienDo`, `ChiPhiTheoDuAn`, `DuAnTheoDoi` | Mang dữ liệu biểu đồ và bảng theo dõi |
| Razor/JavaScript | `QuanLyDuAn/QuanLyDuAn/Views/Dashboard/Index.cshtml` | `labels`, `progressData`, `costData`, `render`, `progressChart`, `costChart` | Serialize dữ liệu và tạo biểu đồ |
| CSS | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/Dashboard/index.css` | `.chart-grid`, `.chart-card`, `.chart-wrapper`, `.chart-wrapper canvas` | Grid hai cột, container và responsive |
| Thư viện | `QuanLyDuAn/QuanLyDuAn/Views/Dashboard/Index.cshtml` | `https://cdn.jsdelivr.net/npm/chart.js` | Nạp Chart.js từ CDN, không khóa version |

Không có file JavaScript riêng cho Dashboard; cấu hình biểu đồ nằm inline trong View.

### 3.2 Cách dữ liệu đang được tạo

- Query gốc là `duAnThongKeQuery`, đã giới hạn bởi scope người dùng và các bộ lọc dự án, quản lý, team, loại, trạng thái, khoảng/mốc ngày.
- `projectsForChart` dùng `OrderByDescending(x => x.MaDuAn).Take(12)`: biểu đồ không lấy toàn bộ; lấy tối đa 12 dự án có mã số lớn nhất trong tập đã lọc. Đây là quy tắc “12 dự án mới theo mã”, không phải Top 12 theo tiến độ hoặc chi phí.
- Tiến độ lấy trực tiếp từ `DuAn.PhanTramHoanThanh`, null thành `0`; service không tính lại tỷ lệ tại View.
- Chi phí lấy từ `CHI_PHI` join ngân sách, group theo `MaDuAn`, cộng `SoTienDaChi`; dự án không có chi phí thành `0`. Khoảng ngày còn được áp dụng theo `NgayChi`.
- Cả hai biểu đồ dùng cùng thứ tự `projectsForChart`, nên ba danh sách giữ đúng index.
- Labels hiện là toàn bộ `TenDuAn`; chỉ khi null mới dùng `Dự án #{MaDuAn}`. Chuỗi rỗng/whitespace chưa được thay thế trong phép gán này.
- Entity `DuAn` chỉ có khóa số `MaDuAn` và `TenDuAn`; không tìm thấy trường “mã dự án” dạng chuỗi riêng. Chuỗi dạng `DATA10-003 - ...` trong ví dụ, nếu có, đang là một phần của `TenDuAn`.
- View dùng `JsonSerializer.Serialize`, không format/cắt nhãn. Tiến độ và chi phí được đưa sang JavaScript dưới dạng số.
- Chart.js dùng biểu đồ cột dọc (`type: "bar"`), `responsive: true`, `maintainAspectRatio: false`. Không cấu hình `ticks`, `autoSkip`, `maxRotation`, `minRotation`, padding hay callback tooltip.
- Tooltip mặc định của Chart.js dùng label gốc trong `labels`; vì labels chưa bị rút gọn nên hiện tại tooltip có tên đầy đủ. Source không có tooltip callback bảo đảm điều này sau khi rút gọn.
- `.chart-wrapper` có chiều cao cố định 290 px; xuống 270 px ở viewport dưới 1200 px và 250 px dưới 768 px. `overflow: hidden`; canvas chỉ có `max-width: 100%`.

### 3.3 Nguyên nhân giao diện bị rối

1. `labels` chứa nguyên văn toàn bộ tên dự án, kể cả chuỗi dài không có điểm ngắt phù hợp.
2. Có thể có 12 label dài trong một chart rộng chỉ nửa hàng ở desktop (`.chart-grid` hai cột).
3. Cả hai chart không cấu hình trục X. Chart.js tự bố trí/auto-skip và xoay theo mặc định của version được CDN phân phối; ứng dụng không kiểm soát `autoSkip`, `maxRotation`, `minRotation` hoặc giới hạn chiều dài.
4. Container cao cố định và `overflow: hidden`, nên nhãn xoay dài chiếm diện tích vẽ, có nguy cơ bị dồn hoặc cắt.
5. Hai chart đang dùng chung một mảng label đầy đủ; chưa tách label hiển thị khỏi label nguồn để vừa rút gọn trục vừa giữ tooltip đầy đủ.
6. CDN không khóa version Chart.js, nên chi tiết mặc định về ticks có thể thay đổi theo thời điểm tải.

Vấn đề không nằm ở công thức tiến độ/chi phí và không cần đổi query nghiệp vụ.

### 3.4 Phương án chỉnh sửa đề xuất

| Hạng mục | File dự kiến sửa | Thay đổi đề xuất | Lý do | Mức ảnh hưởng |
|---|---|---|---|---|
| Label trục | `Views/Dashboard/Index.cshtml` | Giữ `fullProjectLabels`, tạo `axisProjectLabels` bằng helper JavaScript cục bộ | Chỉ thay phần trình bày, không chạm ViewModel/service | Thấp |
| Tooltip | Cùng View | Callback tooltip lấy tên đầy đủ theo `dataIndex` | Rút gọn trục nhưng không mất thông tin | Thấp |
| Ticks | Cùng View | Cấu hình giống nhau cho hai trục X: `autoSkip: true`, giới hạn ticks phù hợp, `maxRotation: 0`, `minRotation: 0`, padding nhỏ | Tránh nhãn xiên/chồng và giữ hai chart nhất quán | Thấp |
| CSS container | `wwwroot/css/Dashboard/index.css` | Chỉ tinh chỉnh class chart Dashboard nếu runtime còn cắt nhãn; không dùng CSS global | Giữ responsive, tránh ảnh hưởng chart AI/khác | Thấp, có điều kiện |
| Query/ViewModel | Không đề xuất sửa | Giữ `Take(12)`, thứ tự và các danh sách hiện tại | Source đã có giới hạn; lỗi giải được tại View/JS | Không ảnh hưởng |

**Giải pháp chọn:** cột dọc hiện tại + label rút gọn cục bộ + tooltip tên đầy đủ + ticks không xoay và auto-skip có kiểm soát. Không chọn đổi sang biểu đồ ngang ở lần sửa tối thiểu vì hai chart đang ghép cặp trong grid, tối đa 12 mục, và biểu đồ ngang sẽ cần chiều cao động cũng như thay đổi cách đọc đáng kể. Chỉ cân nhắc biểu đồ ngang sau kiểm thử runtime nếu label rút gọn vẫn không đủ rõ.

Không chọn thêm Top N: source đã âm thầm giới hạn 12 theo `MaDuAn` từ trước. Thay tiêu chí sang chi phí/tiến độ sẽ đổi ý nghĩa thống kê. Nên bổ sung chú thích “Hiển thị tối đa 12 dự án có mã mới nhất trong phạm vi lọc” nếu muốn làm rõ hiện trạng, nhưng không đổi tập dữ liệu trong chỉnh sửa UI này.

### 3.5 Quy tắc tạo nhãn biểu đồ

- Dùng một hàm JavaScript cục bộ trong `Index.cshtml`, áp dụng chung cho hai chart.
- Chuẩn hóa hiển thị bằng `trim`; không sửa dữ liệu gốc.
- Tối đa đề xuất **24 ký tự hiển thị**. Nếu dài hơn, lấy 23 ký tự Unicode an toàn rồi thêm `…`. Trong JavaScript nên dùng `Array.from(text)` để cắt theo Unicode code point, tránh cắt giữa surrogate pair; tiếng Việt dựng sẵn như `ế`, `ộ` vẫn nguyên vẹn.
- Nếu tên bắt đầu bằng đoạn mã trước dấu ` - `, ưu tiên giữ mã và rút gọn phần tên trong tổng ngân sách 24 ký tự, ví dụ `DATA10-003 - cổng quản…`.
- Nếu không nhận ra mã, rút gọn toàn chuỗi.
- Nếu tên null/rỗng: ViewModel hiện chỉ có label fallback cho null. Khi sửa hiển thị nên fallback `Dự án #<MaDuAn>`; để làm đúng cần có `MaDuAn` tương ứng. Phương án tối thiểu là dùng `Dự án chưa đặt tên`; phương án rõ hơn nhưng tùy chọn là thêm danh sách mã hiển thị riêng trong ViewModel. Không dùng chuỗi rỗng.
- Nếu chỉ có mã/tên ngắn: giữ nguyên.
- Tooltip callback luôn lấy `fullProjectLabels[context.dataIndex]`, không lấy label đã rút gọn.
- Không đổi sang biểu đồ ngang trong phương án chính; không cần chiều cao động.
- Không thêm giới hạn mới. Giữ tối đa 12 hiện tại và công khai phạm vi nếu bổ sung chú thích.

### 3.6 Tác động đến số liệu

- Không đổi `PhanTramHoanThanh`.
- Không đổi query/group/sum chi phí.
- Không đổi `OrderByDescending(MaDuAn).Take(12)` trong phạm vi UI fix.
- Không bỏ phần tử khỏi ba mảng; label rút gọn chỉ là bản sao hiển thị.
- Không đổi scope, permission, filter hoặc thứ tự index dữ liệu.
- Các chart trạng thái và theo tháng không dùng helper label dự án, nên không bị ảnh hưởng.

## 4. Hiện trạng giao diện thử model nguyên nhân

### 4.1 File và thành phần liên quan

| Thành phần | Đường dẫn file | Class/action/phương thức | Vai trò |
|---|---|---|---|
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/AiController.cs` | `Models` GET, `TestPredict` POST | Mở trang và xử lý form thử model |
| Interface MVC | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IAiService.cs` | `LayTrangModelAsync`, `TestPredictAsync` | Contract nghiệp vụ MVC |
| Service MVC | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiService.cs` | `LayTrangModelAsync`, `TestPredictAsync`, `HauKiemKetQuaTestNguyenNhan` | Lấy project/model, snapshot, danh mục, hậu kiểm |
| API client | `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IAiApiService.cs`; `Services/Implementations/AiApiService.cs` | `TestPredictAsync` | POST `/admin/model/test-reason` |
| Page ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiModelPageViewModel.cs` | `MaDuAnTest`, `ModelTestDuocChon`, `KetQuaTestPhanTich` | Trạng thái trang và kết quả |
| Request/response DTO | `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiPredictApiViewModels.cs` | `AiTestReasonRequestViewModel`, `AiTestReasonResponseViewModel` | `modelFile`, feature, catalog; confidence/reason/model/source |
| Razor | `QuanLyDuAn/QuanLyDuAn/Views/Ai/Models.cshtml` | form `asp-action="TestPredict"`, `.ai-grid-4`, bốn `.ai-stat-card` | Hiển thị form và kết quả; không có partial |
| CSS | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/AI/index.css` | `.ai-grid-4`, `.ai-stat-card`, `.ai-stat-value`, media query | Grid 4/2/1 cột và style card |
| FastAPI router | `QuanLyDuAnAIService/app/routers/admin_router.py` | `test_reason` | Endpoint `/admin/model/test-reason` |
| FastAPI schema | `QuanLyDuAnAIService/app/schemas.py` | `TestPredictRequest`, `TestPredictResponse` | Contract strict |
| FastAPI model service | `QuanLyDuAnAIService/app/services/model_service.py` | `ModelService.test_predict` | Nạp filename được chọn hoặc model active |
| FastAPI prediction | `QuanLyDuAnAIService/app/services/prediction_service.py` | `_predict_label_with_confidence`, `test_predict` | `predict_proba`, map nguyên nhân, fallback nhất quán |
| Threshold | `QuanLyDuAnAIService/app/config.py` | `reason_confidence_threshold` | Mặc định 0.6 cho luồng phân tích thật |

Không có partial View riêng cho kết quả thử model.

### 4.2 Luồng dữ liệu kết quả thử model

- Dự án được chọn từ `DanhSachDuAnTest`, do `LayTrangModelAsync` lấy các dự án chưa xóa và có ít nhất một `AI_DATASET`, sắp theo `TenDuAn`.
- Model được chọn từ `LichSuModelNguyenNhan`; giá trị option là `AiModelInfoViewModel.TenFile` chuyển vào `AiModelVersionMetricViewModel.TenModel`.
- MVC tìm snapshot `AI_DATASET` mới nhất theo `NgayTongHop`, rồi `MaData`; nếu dataset cho biết dự án không trễ thì dừng.
- MVC gửi `modelFile`, đủ 22 feature và `danhMucNguyenNhan` sang FastAPI.
- FastAPI nạp chính filename đó; nếu rỗng thì dùng model nguyên nhân đang hoạt động. `modelUsed` chính là `request.modelFile` hoặc tên model active đã resolve, không phải display name tùy ý.
- Response gồm `confidence` (`float`/C# `double` không nullable), `suggestedReasonCode`, `suggestedReason`, `explanation`, `modelUsed`, `reasonSource`.
- `reasonSource` thực tế là `NguyenNhanModel` hoặc `RuleFallback`.
- Kết quả test chỉ gán vào `vm.KetQuaTestPhanTich`; không gọi nhánh lưu `AI_KET_QUA`.
- Filename hiển thị đồng thời là định danh dùng để nạp model. Việc thay chuỗi dữ liệu sẽ ảnh hưởng logic; việc chỉ bọc/rút gọn ở HTML/CSS thì không.

### 4.3 Nguyên nhân tên model bị tràn

- Kết quả dùng `.ai-grid.ai-grid-4` với `repeat(4, minmax(0, 1fr))`; responsive chuyển 2 cột dưới 1200 px và 1 cột dưới 768 px.
- Track grid đã cho phép co nhờ `minmax(0, 1fr)`, nhưng `.ai-stat-card` và `.ai-stat-value` không có `min-width: 0`, `max-width`, `overflow`, `overflow-wrap`, `word-break` hoặc `text-overflow`.
- Filename dài là một token không có khoảng trắng. Quy tắc wrap bình thường không bắt buộc bẻ giữa token, nên `<strong class="ai-stat-value">` vẽ tràn khỏi card.
- `.ai-text-truncate-2` có ellipsis/word-break nhưng card Model không dùng class này.
- Lỗi nằm ở style hiển thị cục bộ, không liên quan tên file, metadata hay API.

### 4.4 Phương án xử lý tên model

Chọn một class riêng, ví dụ class ngữ nghĩa dành cho giá trị filename trong **khu vực kết quả test**, không sửa global `.ai-stat-value`:

- đặt `min-width: 0` cho card/giá trị liên quan;
- `max-width: 100%`;
- `overflow-wrap: anywhere` là quy tắc chính để filename dài có thể xuống tối đa khoảng hai dòng mà không nới grid;
- không cần `word-break: break-all`, vì bẻ mọi vị trí làm tên ngắn khó đọc; chỉ dùng `word-break: break-word` làm fallback trình duyệt nếu cần;
- phương án chính không dùng ellipsis CSS một dòng vì che phần đuôi timestamp/extension có ích;
- thêm `title` bằng tên đầy đủ để desktop xem lại nguyên chuỗi; nội dung DOM vẫn là tên đầy đủ nên màn hình hẹp cũng đọc được qua wrap;
- không dùng kích thước cố định và không đổi `.joblib`, đường dẫn, metadata hay value của `<select>`.

Nếu kiểm thử cho thấy card quá cao với filename cực dài, phương án phụ mới là line-clamp hai dòng + `title`. Không cần thư viện tooltip.

## 5. Chuẩn hóa cách hiển thị mức độ tin cậy

### 5.1 Ý nghĩa giá trị hiện tại

- `PredictionService._predict_label_with_confidence` lấy xác suất lớn nhất từ `predict_proba`; nếu model không có `predict_proba`, mặc định là `1.0`. Với Decision Tree hiện tại, đây là độ chắc chắn của dự đoán cho **mẫu đang thử**, không phải accuracy trên tập kiểm thử.
- FastAPI làm tròn response đến 4 chữ số (`round(confidence, 4)`). MVC giữ `double`; Razor mới nhân 100 và làm tròn hiển thị qua `Confidence.ToString("P0")`.
- Thang contract là 0–1. Source không clamp hoặc validate miền riêng cho `confidence`; `predict_proba` hợp lệ thông thường trả 0–1. Schema/C# hiện không nullable; null hoặc JSON không hợp lệ sẽ khiến deserialize/API operation thất bại thay vì tạo một kết quả card bình thường. `NaN` không phải dữ liệu JSON chuẩn. UI vẫn nên phòng thủ cho non-finite/ngoài miền nếu DTO sau này được nới.
- `reasonConfidenceThreshold = 0.6` được gửi cố định trong luồng phân tích thật và FastAPI cũng có mặc định môi trường 0.6. Endpoint **test-reason không áp threshold này để fallback do confidence thấp**; nó chỉ fallback khi kết quả “Vượt ngân sách” mâu thuẫn feature hoặc khi MVC hậu kiểm cùng điều kiện.
- Do đó mốc “Thấp dưới 0.60” phù hợp với ranh giới chấp nhận model của luồng phân tích thật, nhưng không được mô tả như threshold accuracy.
- Khi `RuleFallback`, `test_predict` vẫn trả confidence vừa tính từ model dù nguyên nhân cuối đã được thay bằng luật. Confidence đó không đo độ chắc chắn của kết quả luật, nên không nên tiếp tục ánh xạ thành mức của card kết quả cuối.
- Màn hình hiện có câu “Kết quả thử không ghi vào AI_KET_QUA”, nhưng chưa giải thích confidence không phải accuracy.
- Màn hình phân tích thật có cách hiển thị `DoTinCayKetQua` riêng; không dùng formatter `P0` của màn hình test. Cần kiểm tra tính đồng nhất khi triển khai, nhưng không mở rộng CSS/logic ngoài yêu cầu nếu chưa cần.

### 5.2 Bảng ánh xạ đề xuất

Source dùng thang 0–1:

| Giá trị gốc | Phần trăm tương ứng | Mức hiển thị |
|---|---:|---|
| null/không hữu hạn/ngoài 0–1 | — | Không xác định |
| `< 0.60` | `< 60%` | Thấp |
| `0.60` đến `< 0.80` | `60%` đến `< 80%` | Trung bình |
| `0.80` đến `< 0.95` | `80%` đến `< 95%` | Cao |
| `>= 0.95` | `>= 95%` | Rất cao |

Riêng `ReasonSource == "RuleFallback"`: không áp bảng cho kết quả cuối; hiển thị “Theo quy tắc” và chú thích “Không áp dụng confidence cho kết quả theo quy tắc”.

### 5.3 Vị trí nên thực hiện ánh xạ

Phương án phù hợp nhất hiện tại là một helper hiển thị nhỏ, cục bộ cho `Models.cshtml` (Razor function hoặc giá trị tính trước ở đầu View), vì:

- chỉ tìm thấy card test đang dùng `AiTestReasonResponseViewModel.Confidence`;
- đây là quy tắc trình bày, không phải business rule và không nên sửa `AiService`/FastAPI;
- tránh tạo kiến trúc helper dùng chung khi chưa có người dùng thứ hai cùng DTO;
- tập trung xử lý null/non-finite/ngoài miền và `RuleFallback` tại một chỗ thay vì lặp trong markup.

Nếu sau đó màn hình phân tích thật được chuẩn hóa cùng thang/mốc, mới tách thành display helper dùng chung trong MVC. Không đặt ánh xạ ở JavaScript vì kết quả đã render server-side.

### 5.4 Cách hiển thị được chọn

Chọn **Phương án B**:

- Nhãn card đổi thành **“Mức độ tin cậy”**.
- Dòng chính: `Thấp`, `Trung bình`, `Cao`, `Rất cao`.
- Dòng phụ nhỏ hoặc `title`: `Xác suất của kết quả trên mẫu hiện tại: 100%`. Đây không phải nội dung chính và phải đi cùng giải thích không phải accuracy chung.
- Lý do chọn B: người quản trị model vẫn cần đối chiếu xác suất chính xác khi test, nhưng hierarchy bằng chữ loại bỏ ấn tượng “100% chính xác”. Phương án A an toàn hơn về diễn giải nhưng làm mất thông tin chẩn đoán hữu ích khỏi UI.
- `RuleFallback`: dòng chính **“Theo quy tắc”**, dòng phụ **“Không áp dụng confidence cho kết quả theo quy tắc”**. Không hiển thị mức “Rất cao” dù response còn chứa confidence của model.
- Null/non-finite/ngoài 0–1: **“Không xác định”**, không format phần trăm.
- API lỗi: `KetQuaTestPhanTich` không có, card không render; giữ `CanhBao` hiện tại.
- Không gọi “Rất chính xác” và không dùng “Độ chính xác 100%”.

### 5.5 Nội dung giải thích cho người dùng

Đề xuất đặt một dòng nhỏ ngay dưới hàng kết quả:

> Mức độ tin cậy phản ánh độ chắc chắn của kết quả đối với dự án đang thử, không phải độ chính xác tổng thể của model.

Với fallback, thêm ngắn gọn tại card: “Kết quả được chọn theo quy tắc; confidence của model không áp dụng.”

## 6. Danh sách file dự kiến phải chỉnh sửa

| STT | Đường dẫn file | Nội dung cần chỉnh | Bắt buộc/Tùy chọn |
|---:|---|---|---|
| 1 | `QuanLyDuAn/QuanLyDuAn/Views/Dashboard/Index.cshtml` | Label rút gọn, tooltip tên đầy đủ, ticks dùng chung cho hai chart | Bắt buộc |
| 2 | `QuanLyDuAn/QuanLyDuAn/Views/Ai/Models.cshtml` | Class filename, title; ánh xạ mức, RuleFallback và chú thích confidence | Bắt buộc |
| 3 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/AI/index.css` | CSS cục bộ wrap filename/min-width và dòng chú thích | Bắt buộc |
| 4 | `QuanLyDuAn/QuanLyDuAn/wwwroot/css/Dashboard/index.css` | Tinh chỉnh container/padding nếu runtime còn cắt nhãn | Tùy chọn |
| 5 | `QuanLyDuAn/QuanLyDuAn/ViewModels/Dashboard/DashboardViewModel.cs` | Chỉ thêm mã hiển thị nếu bắt buộc phân biệt fallback tên rỗng bằng `MaDuAn` | Tùy chọn, chưa cần cho phương án tối thiểu |
| 6 | `QuanLyDuAn/QuanLyDuAn/Views/Shared/...` | Không đề xuất file mới/helper dùng chung ở bước đầu | Không cần |

Không cần sửa `DashboardService`, `AiService`, `AiApiService` hay FastAPI cho hai lỗi UI hiện tại.

## 7. Những thành phần tuyệt đối không được thay đổi

- Database, schema, bảng, cột, dữ liệu và migration.
- Entity AI/Dashboard khi không có nhu cầu contract thật.
- Giá trị confidence gốc trong response/dữ liệu.
- Decision Tree, `predict_proba`, threshold API và dữ liệu train.
- File `.joblib`, tên file thật, đường dẫn và metadata model.
- Contract JSON MVC–FastAPI.
- Công thức tiến độ, chi phí, scope và business rule Dashboard.
- Permission, role, workflow.
- Cách phân tích thật lưu `AI_KET_QUA`; endpoint test vẫn không lưu.
- `AI_KET_QUA` không thành ground truth; `AI_NGUYEN_NHAN` vẫn là xác nhận quản lý.
- MVC vẫn là system-of-record; FastAPI vẫn compute-only.

## 8. Kịch bản kiểm thử sau khi sửa

### Dashboard

| Mã | Dữ liệu đầu vào | Kết quả mong đợi |
|---|---|---|
| DB-01 | Một dự án tên ngắn | Label giữ nguyên; số liệu đúng |
| DB-02 | Tên dài hơn 24 ký tự | Trục có `…`, không chồng/tràn |
| DB-03 | Tên có dấu tiếng Việt | Không mất dấu, không mojibake, không cắt lỗi ký tự |
| DB-04 | `DATA10-003 - tên dài` | Giữ phần mã, rút gọn phần sau nhất quán |
| DB-05 | Tên rất dài không có khoảng trắng | Label vẫn bị giới hạn; chart không vỡ |
| DB-06 | Chỉ 1 dự án | Một cột, tooltip đúng |
| DB-07 | Khoảng 10 dự án | Labels đọc được, hai chart đồng nhất |
| DB-08 | Hơn 12 dự án trong scope | Vẫn đúng tối đa 12 theo `MaDuAn` giảm dần; có chú thích phạm vi nếu được thêm |
| DB-09 | Thu nhỏ trình duyệt qua 1200/768 px | Chart chuyển một cột, canvas không tràn |
| DB-10 | Hover/tap cột có tên dài | Tooltip hiển thị nguyên tên gốc |
| DB-11 | So sánh trước/sau cùng bộ lọc | Tiến độ và chi phí từng index không đổi |
| DB-12 | Tên null/rỗng | Có fallback dễ hiểu, không có label rỗng |
| DB-13 | Chart trạng thái/theo tháng | Không bị helper label dự án tác động |

### Thử model nguyên nhân

| Mã | Dữ liệu đầu vào | Kết quả mong đợi |
|---|---|---|
| AI-01 | Tên model ngắn | Hiển thị nguyên tên, card không đổi bất thường |
| AI-02 | `decision_tree_reason_20260628_140040.joblib` hoặc dài hơn | Tự xuống dòng trong card |
| AI-03 | Filename dài không khoảng trắng | Không tràn container/đẩy card khác |
| AI-04 | Confidence `0` từ nguồn model | “Thấp”; phụ `0%` |
| AI-05 | Confidence `0.59` | “Thấp”; phụ giữ giá trị tương ứng |
| AI-06 | Confidence `0.60` | “Trung bình” |
| AI-07 | Confidence `0.79` | “Trung bình” |
| AI-08 | Confidence `0.80` | “Cao” |
| AI-09 | Confidence `0.94` | “Cao” |
| AI-10 | Confidence `0.95` | “Rất cao” |
| AI-11 | Confidence `1.00` | Chính “Rất cao”; phụ “Xác suất ... 100%”, không gọi accuracy |
| AI-12 | Confidence null (test phòng thủ/DTO nới) | “Không xác định”; không exception format |
| AI-13 | NaN, vô hạn, âm hoặc >1 (unit test helper) | “Không xác định”; không hiển thị phần trăm sai |
| AI-14 | `ReasonSource = NguyenNhanModel` | Áp bảng mức confidence |
| AI-15 | `ReasonSource = RuleFallback` | Chính “Theo quy tắc”; confidence “Không áp dụng” |
| AI-16 | FastAPI lỗi/timeout | Không render kết quả cũ; hiển thị cảnh báo hiện có |
| AI-17 | Desktop ≥1200 px | Bốn card cân bằng; filename wrap trong card |
| AI-18 | Màn hình hẹp <768 px | Một cột; không scroll ngang do filename |
| AI-19 | Hover tên model đã wrap | `title` cho biết tên đầy đủ, không đổi value thật |
| AI-20 | Ghi nhận số dòng `AI_KET_QUA` trước/sau test | Không phát sinh bản ghi |
| AI-21 | Chọn model cụ thể | `modelUsed` khớp filename đã gửi |
| AI-22 | Chọn “model đang hoạt động” | `modelUsed` là tên model active do FastAPI resolve |
| AI-23 | Kết quả “Vượt ngân sách” mâu thuẫn feature | MVC/FastAPI hậu kiểm ra `RuleFallback`; UI không gán mức confidence model cho luật |
| AI-24 | So sánh accuracy card phía trên với confidence test | Hai nhãn/nội dung phân biệt rõ, không gây hiểu là cùng chỉ số |

## 9. Rủi ro và lưu ý triển khai

- Rút gọn quá mạnh khiến nhiều dự án có cùng prefix khó phân biệt; giảm rủi ro bằng giữ mã/prefix và tooltip đầy đủ.
- Dashboard đã giới hạn 12. Đổi sang Top N theo tiêu chí khác hoặc giảm thêm mà không thông báo sẽ làm người dùng hiểu sai phạm vi.
- Sửa global `.ai-stat-value` có thể làm mọi card AI đổi layout; phải dùng class riêng cho filename/kết quả test.
- “Rất cao” vẫn có thể bị hiểu là model luôn đúng; dòng giải thích “mẫu hiện tại, không phải accuracy tổng thể” là bắt buộc.
- Màn hình phân tích thật và màn hình test hiện dùng DTO/formatter khác nhau. Nếu chuẩn hóa một màn hình, cần kiểm tra thuật ngữ ở màn hình kia để tránh hai thang diễn giải xung đột.
- `RuleFallback` giữ confidence của model trong response test nhưng nguyên nhân cuối đến từ luật; hiển thị confidence như độ chắc chắn của fallback là sai ngữ nghĩa.
- Không cần sửa cả MVC và FastAPI cho vấn đề trình bày. Sửa FastAPI sẽ tăng phạm vi và rủi ro contract không cần thiết.
- `overflow: hidden` của chart wrapper có thể cắt nhãn sau khi chỉnh ticks; phải kiểm tra runtime ở các breakpoint trước khi quyết định sửa CSS.
- Cắt chuỗi JavaScript bằng index UTF-16 đơn giản có thể làm hỏng surrogate pair; dùng xử lý Unicode an toàn và kiểm tra UTF-8 sau khi sửa.
- CDN Chart.js không khóa version làm hành vi mặc định có thể trôi; cấu hình ticks/tooltip tường minh giúp giảm phụ thuộc, nhưng việc khóa version là quyết định vận hành riêng, không bắt buộc cho fix này.

## 10. Kết luận và mức độ sẵn sàng chỉnh sửa

Đã đủ dữ liệu source để viết prompt và thực hiện bước sửa UI tiếp theo.

- **Dashboard:** chắc chắn sửa `Views/Dashboard/Index.cshtml`; `wwwroot/css/Dashboard/index.css` chỉ sửa nếu kiểm thử runtime cho thấy còn cắt/thiếu không gian. Giữ cột dọc, tối đa 12 và toàn bộ số liệu; dùng label trục rút gọn 24 ký tự, tooltip tên đầy đủ, ticks không xoay/auto-skip có kiểm soát.
- **Tên model:** chắc chắn sửa `Views/Ai/Models.cshtml` và `wwwroot/css/AI/index.css`; dùng class cục bộ, `min-width: 0`, `overflow-wrap: anywhere`, tối đa hai dòng nếu cần và `title` tên đầy đủ.
- **Confidence:** sửa tại lớp hiển thị MVC, chọn Phương án B. Dòng chính là mức chữ; phần trăm chỉ là thông tin phụ của mẫu hiện tại. `RuleFallback` hiển thị “Theo quy tắc/Không áp dụng”.
- Không cần thay FastAPI, CSDL, migration, package, model, threshold hay API contract.
- Điểm chưa xác minh bằng runtime: kích thước pixel thực tế trên các trình duyệt, tooltip touch trên màn hình hẹp và việc có cần nới chart wrapper hay không. Các điểm này không cản trở việc sửa tối thiểu; chúng quyết định duy nhất phần CSS Dashboard tùy chọn.
