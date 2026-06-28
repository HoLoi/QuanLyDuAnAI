# PHÂN TÍCH VIỆC LOẠI BỎ MÀN HÌNH PHÂN TÍCH NGUYÊN NHÂN TRỄ ĐỘC LẬP

## 1. Kết luận nhanh

**Kết luận được chọn: Nên bỏ khỏi menu nhưng giữ route tương thích tạm thời.**

Màn hình độc lập `/Ai/Predict` không sở hữu một pipeline phân tích chính thức riêng. Cả ba điểm vào `AiController.Predict`, `DuAnController.PhanTichNguyenNhanTre` và `DanhGiaDuAnController.PhanTichAiDuAn` cuối cùng đều gọi `AiService.PhanTichNguyenNhanDuAnAsync`. Vì vậy việc bỏ mục menu không làm mất pipeline tổng hợp dữ liệu, gọi FastAPI, lưu `AI_KET_QUA`, xác nhận `AI_NGUYEN_NHAN` hoặc đồng bộ nhãn.

Vị trí chính thức nên là **màn hình chi tiết dự án**:

- Chạy/chạy lại phân tích: tại chi tiết dự án.
- Xem kết quả mới nhất và xác nhận nguyên nhân: tại chi tiết dự án.
- Đánh giá dự án: chỉ đọc kết quả và nguyên nhân đã xác nhận để hỗ trợ đánh giá, không tự động chạy AI và không dùng quyền đánh giá thay quyền AI.
- Tổng quan/lịch sử tổng hợp: tiếp tục ở `Ai/Dashboard`; source hiện không có màn hình lịch sử chi tiết từng bản ghi `AI_KET_QUA`.

Không nên xóa ngay `AiController.Predict`, `TestPredict`, `XacNhanNguyenNhan`, `Views/Ai/Predict.cshtml` và `AiPredictPageViewModel`. Màn hình cũ còn có nghiệp vụ quản trị/thử model (`TestPredict`) không xuất hiện tại chi tiết dự án. Cần bỏ menu trước, chuyển link nội bộ, giữ GET cũ để điều hướng tương thích, rồi mới quyết định tách hoặc loại bỏ phần thử model.

## 2. Phạm vi source đã kiểm tra

### 2.1. Layout, route và menu

- `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml`
  - Các biến `canAi`, `canAiDataset`, `canAiTrain`, `canAiAnalyzeReason`, `canAiDashboard`.
  - Nhóm menu AI và link `Ai/Predict`.
  - Helper cục bộ `IsActiveController`, `IsActiveRoute`.
- `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml.css`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/layout/sidebar.css`.
- `QuanLyDuAn/QuanLyDuAn/Program.cs`
  - Route mặc định `{controller=Dashboard}/{action=Index}/{id?}`.
- Không thấy layout riêng cho Admin, Manager hoặc Employee; các vai trò dùng chung `_Layout.cshtml`.
- Không thấy partial sidebar/navbar riêng hoặc JavaScript riêng dùng để sinh/điều khiển active item AI. Trạng thái active được tính trực tiếp trong Razor bằng controller/action hiện tại.

### 2.2. Controller

- `Controllers/AiController.cs`
  - `Dashboard`, `XuatFile`, `Train` GET/POST.
  - `Predict` GET/POST, `TestPredict`, `XacNhanNguyenNhan`.
  - `Models`, `ValidateModel`, `CompareModel`, `SetActiveModel`, `ReloadModel`, `DeleteModel`, `ExportMetadata`.
- `Controllers/DuAnController.cs`
  - `Details`, alias `ChiTiet`.
  - `PhanTichNguyenNhanTre`, `XacNhanNguyenNhanTre`.
  - `GanNguCanhDetailsAsync`.
- `Controllers/DanhGiaDuAnController.cs`
  - `Index`, `Form`, `Luu`, `GuiDuyet`, `Duyet`, `TuChoi`, `ChiTiet`.
  - `PhanTichAiDuAn`, `XacNhanNguyenNhan`.
  - `RedirectToReturnUrlOrIndex`.
- `Controllers/AiDatasetController.cs` được rà soát để phân biệt quyền dataset với quyền phân tích.
- `Controllers/DashboardController.cs` và `Views/Dashboard/Index.cshtml` được tìm kiếm link AI; không thấy link trực tiếp đến `Ai/Predict`.

### 2.3. Service và interface

- `Services/Interfaces/IAiService.cs`.
- `Services/Implementations/AiService.cs`
  - `KhoiTaoTrangPredictAsync`.
  - `LayPhanTichNguyenNhanDuAnAsync`.
  - `PhanTichNguyenNhanDuAnAsync`.
  - `DuDoanDuAnAsync`, `TestPredictAsync`.
  - `XacNhanNguyenNhanAsync`.
  - `LayNguCanhPhanTichAsync`, `KiemTraQuyenPhanTich`.
  - `LayHoacLamMoiDatasetChinhThucAsync`, `GoiFastApiPhanTichAsync`.
  - `LuuKetQuaPhanTichAsync`.
  - Các hàm nạp kết quả mới nhất và xác nhận vào page/panel.
  - `GetAccessibleProjectIdsAsync`, `GetCurrentUserRoleFlagsAsync`.
- `Services/Interfaces/IAiApiService.cs`.
- `Services/Implementations/AiApiService.cs`
  - `DuDoanDuAnAsync` gọi POST `/analyze/delay-reason`.
  - `TestPredictAsync` gọi POST `/admin/model/test-reason`.
  - Xử lý lỗi/validation bộ 22 feature.
- `Services/Interfaces/IAiDatasetService.cs`.
- `Services/Implementations/AiDatasetService.cs`
  - `TongHopDatasetChoDuAnAsync`, `BuildFeatureSnapshotAsync`.
  - Tổng hợp 22 feature.
  - Lấy nhãn từ `AI_NGUYEN_NHAN`.
  - Điều kiện dữ liệu hợp lệ để train.
- `Services/Interfaces/IDanhGiaDuAnService.cs`.
- `Services/Implementations/DanhGiaDuAnService.cs`
  - `GetPageAsync`, `GetFormAsync`, `LuuDanhGiaAsync`.
  - `PhanTichAiDuAnAsync`, `XacNhanNguyenNhanAsync`.
  - `GuiDuyetAsync`, `DuyetAsync`, `TuChoiAsync`, `GetChiTietAsync`.
  - `LoadThongTinHoTroDanhGiaAsync`, `LoadKetQuaAiMoiNhatAsync`.
  - `BuildAiInsightViewModelAsync`.
- `Services/Implementations/DuAnService.cs`
  - `MoLaiDuAnAsync`: giữ `AI_KET_QUA`, soft-delete xác nhận cũ và bỏ label dataset.

### 2.4. View, ViewModel, CSS và JavaScript

- `Views/Ai/Predict.cshtml`.
- `ViewModels/Ai/AiPredictPageViewModel.cs`.
- `ViewModels/Ai/AiPredictApiViewModels.cs`.
- `Views/Ai/Dashboard.cshtml`, `ViewModels/Ai/AiDashboardPageViewModel.cs`.
- `Views/DuAn/Details.cshtml`.
- `Views/DuAn/ChiTiet.cshtml` (view cũ, không được action hiện tại trả trực tiếp).
- `ViewModels/DuAn/DuAnChiTietViewModel.cs`.
- `ViewModels/Ai/AiProjectDelayAnalysisPanelViewModel.cs`.
- `Views/DanhGiaDuAn/Index.cshtml`, `_Form.cshtml`, `_AiInsightCard.cshtml`, `_ProjectStats.cshtml`, `ChiTiet.cshtml`.
- Các ViewModel trong `ViewModels/DanhGiaDuAn`, đặc biệt `DanhGiaDuAnFormViewModel`, `DanhGiaDuAnThongKeViewModel`, `DanhGiaDuAnAiInsightViewModel`.
- JavaScript đổi dự án nằm inline trong `Views/Ai/Predict.cshtml`.
- JavaScript AJAX phân tích AI nằm inline trong `Views/DanhGiaDuAn/_Form.cshtml`.
- CSS AI dùng chung tại `wwwroot/css/AI/index.css`; CSS đánh giá tại `wwwroot/css/shared/evaluation.css`; CSS chi tiết dự án tại `wwwroot/css/DuAn/details.css`.

### 2.5. Permission, claim và seed runtime

- `Constants/Permissions.cs`.
- `Services/Implementations/PermissionHelper.cs`.
- `Services/Interfaces/IPermissionHelper.cs`.
- `Services/Implementations/PhanQuyenService.cs`.
- `Services/Implementations/PermissionDependencyProvider.cs`.
- `Data/KhoiTaoTaiKhoanMacDinh.cs`.
- `Models/Entities/Aspnetroleclaims.cs`, `Aspnetuserclaims.cs`, `DanhMucQuyen.cs`, `DanhMucManHinh.cs`.
- Các scope thực tế trong `AiService`, `DanhGiaDuAnService`, `DuAnService`.

### 2.6. FastAPI và tài liệu/test liên quan

- `QuanLyDuAnAIService/app/routers/prediction_router.py`.
- `QuanLyDuAnAIService/app/services/prediction_service.py`.
- `QuanLyDuAnAIService/app/services/validation_service.py`.
- `QuanLyDuAnAIService/app/services/model_service.py`.
- `QuanLyDuAnAIService/app/schemas.py`.
- Tìm kiếm toàn solution trong `docs`, controller, service, view, ViewModel, `wwwroot` và FastAPI theo các từ khóa được yêu cầu.
- Không thấy project unit test/integration test cho các route AI; chỉ thấy `test.puml`, không phải test thực thi.

## 3. Luồng AS-IS của màn hình phân tích độc lập

### 3.1. Từ menu đến action

1. `_Layout.cshtml` lấy tập quyền được cấp từ `IPhanQuyenService`.
2. Nhóm AI xuất hiện khi có ít nhất một quyền AI liên quan.
3. Item **Phân tích nguyên nhân trễ** chỉ xuất hiện khi có `AI.PhanTichNguyenNhan`.
4. Item dẫn đến GET `/Ai/Predict`.
5. `AiController.Predict(int? maDuAn)` kiểm tra lại `AI.PhanTichNguyenNhan`; nếu thiếu trả `Forbid()`.
6. Action gọi `AiService.KhoiTaoTrangPredictAsync` và trả `Views/Ai/Predict.cshtml`.

Menu và GET action dùng cùng permission nên không có trường hợp bình thường menu hiển thị nhưng GET bị từ chối do khác permission. Tuy nhiên service còn áp role/scope ở lúc chạy phân tích, nên một user được cấp claim ngoài Manager có thể mở trang nhưng không chạy được dự án.

### 3.2. Khởi tạo trang

`KhoiTaoTrangPredictAsync`:

- Admin thấy tất cả dự án; user khác chỉ thấy `projectIds` từ `GetAccessibleProjectIdsAsync`.
- Nạp danh sách model nguyên nhân từ FastAPI.
- Nạp model đang hoạt động/độ chính xác.
- Nếu có `maDuAn`, đọc `AI_DATASET` mới nhất, gắn 22 feature lên form, kiểm tra dataset cũ và nạp kết quả `AI_KET_QUA` mới nhất.
- Trang cho phép hiển thị/chỉnh trực tiếp 22 input feature, nhưng POST phân tích chính thức không tin các feature này: `DuDoanDuAnAsync` chỉ lấy `MaDuAn` rồi gọi pipeline chung.

### 3.3. Chạy phân tích chính thức

`POST /Ai/Predict`:

- Có `[ValidateAntiForgeryToken]`.
- Kiểm tra `AI.PhanTichNguyenNhan`.
- Kiểm tra `ModelState`.
- Gọi `AiService.DuDoanDuAnAsync`, rồi `PhanTichNguyenNhanDuAnAsync`.
- `LayNguCanhPhanTichAsync` kiểm tra lại claim, role Manager, Manager hiện tại của dự án và trạng thái trễ.
- Dự án đang thực hiện/chờ xác nhận hoàn thành, đã quá hạn: tạo snapshot tạm thời; không ghi `AI_DATASET`/`AI_KET_QUA`.
- Dự án hoàn thành trễ: lấy/làm mới dataset chính thức; gọi FastAPI; lưu append một `AI_KET_QUA`.
- Kết quả tạm thời trả lại view ngay; kết quả chính thức redirect về `/Ai/Predict?maDuAn=...`.

### 3.4. Chạy thử model

`POST /Ai/TestPredict`:

- Có anti-forgery và yêu cầu `AI.PhanTichNguyenNhan`.
- Cho chọn file model và gọi `/admin/model/test-reason`.
- Không ghi `AI_KET_QUA`.
- Đây là phần riêng có thật của màn hình cũ, nhưng về bản chất là công cụ kiểm thử model, phù hợp hơn với khu vực quản trị model/`AI.Train` chứ không phải luồng nghiệp vụ Manager.

### 3.5. Xác nhận

`POST /Ai/XacNhanNguyenNhan`:

- Có anti-forgery.
- Controller yêu cầu `AI.XacNhan`.
- `AiService.XacNhanNguyenNhanAsync` yêu cầu đồng thời:
  - Role Manager và không phải Admin.
  - Có claim `AI.XacNhan`.
  - Là `DU_AN.MaNguoiDung` hiện tại.
  - Dự án hoàn thành trễ.
  - Dataset mới nhất có `LaDuAnTre = true`.
  - Nguyên nhân tồn tại trong `DM_NGUYEN_NHAN`.
  - Có `AI_KET_QUA` chính thức của đúng dataset và đúng nguyên nhân được xác nhận.
- Service thêm/cập nhật `AI_NGUYEN_NHAN`, gán người/thời gian xác nhận và đồng bộ `AI_DATASET.MaDMNguyenNhan`.
- Redirect lại `/Ai/Predict?maDuAn=...`.

### 3.6. Lịch sử

- Màn hình `Predict` chỉ nạp **kết quả mới nhất**, không có danh sách lịch sử từng lần phân tích.
- `AI_KET_QUA` được append và dữ liệu lịch sử vẫn tồn tại.
- `Ai/Dashboard` đọc toàn bộ `AI_KET_QUA` theo scope để tạo tổng số, timeline tháng và danh sách cảnh báo/kết quả mới nhất theo dự án.
- Không có action/view `LichSuKetQua` riêng trong source.

## 4. Luồng AS-IS tại chi tiết dự án

### 4.1. Điểm vào và quyền xem

- GET `/DuAn/Details/{id}` yêu cầu `DuAn.Xem`.
- `DuAnController.GanNguCanhDetailsAsync` luôn gọi `AiService.LayPhanTichNguyenNhanDuAnAsync`.
- `Views/DuAn/Details.cshtml` luôn render section `#phan-tich-nguyen-nhan-tre`; khi không áp dụng chỉ hiển thị trạng thái/thông báo.
- Không có link từ chi tiết sang màn hình `Ai/Predict`; thao tác chạy và xác nhận được thực hiện ngay tại chi tiết.

### 4.2. Chạy/chạy lại

- Nút **Phân tích** hoặc **Phân tích lại** chỉ xuất hiện khi panel có `CoThePhanTich`.
- `CoThePhanTich` được tính bởi service từ:
  - `AI.PhanTichNguyenNhan`.
  - Đúng Manager hiện tại.
  - Dự án đang quá hạn theo luồng tạm thời hoặc hoàn thành trễ theo luồng chính thức.
- Form POST `/DuAn/PhanTichNguyenNhanTre` có anti-forgery tag helper.
- Controller kiểm tra `AI.PhanTichNguyenNhan`, gọi đúng pipeline chung `PhanTichNguyenNhanDuAnAsync`.
- Kết quả được gắn lại vào `DuAnChiTietViewModel` và trả `View("Details")`; lỗi redirect về Details.

### 4.3. Hiển thị và xác nhận

- Panel hiển thị badge tình trạng, loại phân tích tạm thời/chính thức, nguồn model/quy tắc, mức phù hợp, thời gian và trạng thái xác nhận.
- Chỉ hiển thị nút xác nhận khi `CoTheXacNhan`:
  - Có `AI.XacNhan`.
  - Đúng Manager hiện tại.
  - Là phân tích chính thức của dự án hoàn thành trễ.
  - Có mã nguyên nhân dự đoán.
- Form POST `/DuAn/XacNhanNguyenNhanTre` có anti-forgery.
- Controller và service tiếp tục kiểm tra backend như mục 3.5.
- Chi tiết dự án chỉ xác nhận đúng nguyên nhân AI gợi ý; không có select để Manager đổi sang nguyên nhân khác.

### 4.4. Đánh giá

Chi tiết dự án hiện là nơi bám đúng ngữ cảnh dự án nhất và có đầy đủ chạy/chạy lại/xem/xác nhận. Nó không hiển thị lịch sử nhiều bản ghi, nhưng màn hình độc lập cũng không có lịch sử này.

## 5. Luồng AS-IS tại đánh giá dự án

### 5.1. Workflow đánh giá

- `Index`: yêu cầu `DanhGiaDuAn.Xem`.
- `Form`: controller dùng `HasPermissionAsync(DanhGia, Sua)`, tức phép OR; service cũng dùng OR, chặn Admin tác nghiệp và yêu cầu Manager đang quản lý dự án.
- `Luu`: quyền `DanhGia` hoặc `Sua`; lưu phiếu/chi tiết tiêu chí, không tự ghi kết quả AI.
- `GuiDuyet`: quyền `DanhGia` hoặc `Sua`; service kiểm tra trạng thái.
- `Duyet`, `TuChoi`: quyền `DanhGiaDuAn.Duyet`; service giới hạn vai trò phù hợp.
- `ChiTiet`: quyền `DanhGiaDuAn.Xem`; chỉ xem phiếu, không chứa form chạy AI.

Phân tích AI không phải điều kiện bắt buộc được kiểm tra trong `LuuDanhGiaAsync`, `GuiDuyetAsync`, `DuyetAsync` hoặc `TuChoiAsync`. Kết quả AI là dữ liệu hỗ trợ.

### 5.2. Hiển thị AI

- `_Form.cshtml` chỉ render `_AiInsightCard` và khu xác nhận khi `duAnDangTre`.
- Card hiển thị kết quả mới nhất, mức phù hợp, thời gian, nguyên nhân Manager xác nhận và tối đa ba nguyên nhân liên quan.
- Card mang nhãn **Tham khảo**.
- Dữ liệu AI không tự ghi vào `DANH_GIA_DU_AN` hoặc `CT_DANH_GIA_DU_AN`.

### 5.3. Chạy AI bằng AJAX

- `_Form.cshtml` có form ẩn POST `PhanTichAiDuAn`, anti-forgery và JavaScript `fetch`.
- `DanhGiaDuAnController.PhanTichAiDuAn` yêu cầu `AI.PhanTichNguyenNhan`.
- `DanhGiaDuAnService.PhanTichAiDuAnAsync` gọi pipeline chung `AiService.PhanTichNguyenNhanDuAnAsync`, sau đó chỉ chuyển dữ liệu sang ViewModel JSON.
- Không có pipeline FastAPI hoặc logic lưu `AI_KET_QUA` thứ hai.

### 5.4. Điểm không thống nhất

`DanhGiaDuAnService.GetFormAsync` tính `thongKe.CoThePhanTichAi` bằng:

- Đúng scope.
- Role Manager.
- Có `DanhGiaDuAn.DanhGia` **hoặc** `DanhGiaDuAn.Sua`.

Nó không yêu cầu `AI.PhanTichNguyenNhan`. Vì vậy người chỉ có quyền đánh giá có thể nhìn thấy nút và JavaScript còn có thể tự động gọi POST, nhưng controller trả 403. Ngược lại, form đánh giá vốn đã yêu cầu quyền đánh giá nên người chỉ có quyền AI không thể vào form này; họ dùng chi tiết dự án.

### 5.5. Xác nhận trong đánh giá

- UI cho Manager chọn bất kỳ nguyên nhân trong `DM_NGUYEN_NHAN`, kể cả thay đổi xác nhận cũ.
- Controller yêu cầu `AI.XacNhan`.
- Service cuối cùng gọi `AiService.XacNhanNguyenNhanAsync`.
- `GetFormAsync` tính điều kiện hiển thị bằng biểu thức `(roleFlags.IsManager || có AI.XacNhan) && có DanhGiaDuAn.DanhGia`. Với Manager, nhánh OR làm UI có thể hiện dù thiếu `AI.XacNhan`; POST sau đó bị controller/service chặn.
- Service xác nhận hiện còn yêu cầu nguyên nhân chọn phải trùng với một `AI_KET_QUA` của dataset hiện tại. Do vậy UI cho chọn toàn bộ danh mục nhưng backend chỉ chấp nhận nguyên nhân đã từng là kết quả AI chính thức tương ứng. Đây là lệch UI/backend và hạn chế quyền quyết định độc lập của Manager.

## 6. Bảng so sánh ba điểm truy cập

| Tiêu chí | Màn hình AI độc lập | Chi tiết dự án | Đánh giá dự án |
| --- | --- | --- | --- |
| Chọn dự án | Có dropdown, phải chọn lại | Không; dự án là ngữ cảnh hiện tại | Không; dự án là ngữ cảnh phiếu |
| Chạy phân tích | Có, POST form | Có, POST form | Có, AJAX POST |
| Xem kết quả | Kết quả mới nhất hoặc kết quả tạm thời vừa chạy | Kết quả mới nhất/kết quả vừa chạy | Kết quả mới nhất trong card hỗ trợ |
| Xác nhận nguyên nhân | Có, chỉ xác nhận gợi ý hiện tại | Có, chỉ xác nhận gợi ý hiện tại | Có select, hỗ trợ thay đổi nhưng backend giới hạn |
| Kiểm tra permission | `AI.PhanTichNguyenNhan`; xác nhận `AI.XacNhan` | Cùng hai quyền AI | Backend đúng hai quyền AI; điều kiện hiển thị đang dựa một phần vào quyền đánh giá |
| Kiểm tra scope | Pipeline chung: Manager hiện tại | Pipeline chung: Manager hiện tại | Pipeline chung: Manager hiện tại; form cũng giới hạn Manager |
| Lưu `AI_KET_QUA` | Chỉ phân tích chính thức; test/tạm thời không lưu | Chỉ phân tích chính thức | Chỉ phân tích chính thức |
| Lưu `AI_NGUYEN_NHAN` | Qua service chung | Qua service chung | Qua service chung |
| Logic bị trùng | Trùng phần UI/form/redirect; nghiệp vụ lõi dùng chung | UI riêng; nghiệp vụ lõi dùng chung | Mapping card/JSON riêng; nghiệp vụ lõi dùng chung |

## 7. Phân tích permission hiện tại

### 7.1. Permission khai báo và seed

- `AI.Xem`: quyền cha/nhóm hiển thị AI, không được dùng trực tiếp để chạy phân tích.
- `AI.Dataset`: quản lý/tổng hợp dataset.
- `AI.Train`: train và quản trị model.
- `AI.PhanTichNguyenNhan`: chạy phân tích; `AI.DuDoan` chỉ là alias hằng số cùng giá trị.
- `AI.Dashboard`: xem tổng quan AI.
- `AI.XacNhan`: xác nhận ground truth.
- Runtime seed:
  - Admin: `AI.Xem`, `AI.Dataset`, `AI.Train`, `AI.Dashboard`; không seed quyền phân tích/xác nhận.
  - Manager: `AI.Xem`, `AI.PhanTichNguyenNhan`, `AI.Dashboard`, `AI.XacNhan`.
  - Employee: không thấy seed quyền AI.
- `PermissionDependencyProvider` tách `AIPredict` và `AIXacNhan` thành hai quyền con khác nhau; không có lý do kỹ thuật phải gộp chúng.

### 7.2. Ý nghĩa của helper OR

`PermissionHelper.HasPermissionAsync(User, params permissions)` trả `permissions.Any(...)`. Mọi lời gọi nhiều permission là OR, không phải AND. Điều này phù hợp với `DanhGia` hoặc `Sua` ở form, nhưng không được dùng để biểu diễn “AI.XacNhan và DanhGiaDuAn.DanhGia”.

### 7.3. Ma trận quyền thực tế

| Hành động | Quyền chức năng hiện tại | Điều kiện scope/backend | Vai trò thực tế |
| --- | --- | --- | --- |
| Xem khu vực AI của dự án | `DuAn.Xem` để mở Details; panel tự đọc claim AI | Scope xem dự án do `DuAnService`; panel có thể hiển thị read-only | Admin/Manager/Employee có quyền xem dự án theo scope |
| Chạy phân tích | `AI.PhanTichNguyenNhan` | Manager hiện tại; dự án quá hạn hợp lệ | Manager, không phải Admin |
| Chạy lại phân tích | Như chạy phân tích | Như chạy phân tích | Manager hiện tại |
| Xem lịch sử kết quả | Không có action lịch sử chi tiết; Dashboard cần `AI.Dashboard` | Dashboard lọc projectIds với non-Admin | Admin hoặc Manager được seed Dashboard |
| Xác nhận nguyên nhân | `AI.XacNhan` | Manager hiện tại; hoàn thành trễ; dataset/kết quả chính thức hợp lệ | Manager, không phải Admin |
| Thay đổi xác nhận | `AI.XacNhan` | Cùng scope; update row hiện hành | Chỉ Manager hiện tại; UI đánh giá có select nhưng backend yêu cầu khớp kết quả AI |
| Sử dụng kết quả trong đánh giá dự án | Quyền mở form `DanhGiaDuAn.DanhGia` hoặc `Sua` | Manager hiện tại | Manager; kết quả chỉ tham khảo |

### 7.4. Kết luận theo các tình huống bắt buộc

- **Có phép OR sai không:** Có ở điều kiện hiển thị xác nhận trong `DanhGiaDuAnService.GetFormAsync`: role Manager có thể làm nhánh `(Manager || AI.XacNhan)` đúng dù thiếu claim AI. Với phân tích, UI dùng quyền đánh giá thay vì quyền AI.
- **Chỉ kiểm tra UI mà backend không kiểm tra:** Không đối với ghi AI; cả ba controller và `AiService` đều kiểm tra lại. Rủi ro hiện tại là ngược lại: UI cho nút nhưng backend chặn.
- **Backend cho phép nhưng UI ẩn:** Trong form đánh giá, user phải có quyền đánh giá mới vào được; quyền AI đơn lẻ không phải quyền mở form. Đây là hợp lý nếu đánh giá chỉ là consumer. Ở Details, panel lấy đúng quyền từ service.
- **Admin có phân tích/xác nhận không:** Seed không cấp; kể cả cấp claim thủ công, `AiService` yêu cầu Manager và loại Admin, nên Admin không chạy/xác nhận.
- **Manager ngoài phạm vi truy cập URL trực tiếp:** Có thể mở GET tùy scope xem, nhưng POST pipeline kiểm tra `DU_AN.MaNguoiDung == currentUserId` và từ chối chạy/xác nhận.
- **Leader/Employee có thấy kết quả không:** Có thể thấy panel read-only nếu được phép xem chi tiết dự án. Không chạy/xác nhận vì service yêu cầu Manager hiện tại.
- **`AI.PhanTichNguyenNhan` và `AI.XacNhan` có bị gộp không:** Backend lõi tách đúng; UI đánh giá đang làm mờ ranh giới vì điều kiện hiển thị không dùng đúng hai claim.

### 7.5. Permission TO-BE

- Xem kết quả AI trong dự án: kế thừa quyền xem ngữ cảnh (`DuAn.Xem` hoặc `DanhGiaDuAn.Xem/DanhGia/Sua`) và scope dữ liệu; không cần biến quyền chạy AI thành quyền đọc.
- Chạy/chạy lại: bắt buộc `AI.PhanTichNguyenNhan` **và** Manager hiện tại.
- Xác nhận/thay đổi xác nhận: bắt buộc `AI.XacNhan` **và** Manager hiện tại.
- Quyền đánh giá chỉ cho phép đọc kết quả AI như dữ liệu hỗ trợ; không kích hoạt phân tích.
- Admin chỉ quản trị dataset/model/dashboard theo seed hiện tại; không xác nhận ground truth thay Manager.

## 8. Các lỗi hoặc rủi ro phát hiện

### 8.1. Cao — quyền hiển thị nút phân tích tại đánh giá không khớp backend

- **File:** `Services/Implementations/DanhGiaDuAnService.cs`.
- **Thành phần:** `GetFormAsync`.
- **Logic hiện tại:** `CoThePhanTichAi` dựa vào `DanhGiaDuAn.DanhGia` hoặc `DanhGiaDuAn.Sua`.
- **Rủi ro:** Nút hiện/tự động AJAX cho user thiếu `AI.PhanTichNguyenNhan`, nhận 403; quyền đánh giá vô tình được trình bày như quyền AI.
- **Đề xuất:** Tính quyền hiển thị từ `AI.PhanTichNguyenNhan` và scope; tốt hơn, bỏ hẳn hành động chạy khỏi đánh giá theo TO-BE.

### 8.2. Cao — UI xác nhận tại đánh giá dùng điều kiện OR sai ý nghĩa

- **File:** `Services/Implementations/DanhGiaDuAnService.cs`.
- **Thành phần:** `GetFormAsync`, biến `coQuyenXacNhanNguyenNhan`.
- **Logic hiện tại:** `(roleFlags.IsManager || CoQuyenTheoClaim(AI.XacNhan)) && DanhGiaDuAn.DanhGia`.
- **Rủi ro:** Manager thiếu `AI.XacNhan` vẫn thấy form nhưng POST bị 403.
- **Đề xuất:** Dùng `roleFlags.IsManager && AI.XacNhan && đúng scope`; không dùng quyền đánh giá thay quyền xác nhận.

### 8.3. Cao — Manager không thể xác nhận nguyên nhân khác gợi ý AI một cách độc lập

- **File:** `Services/Implementations/AiService.cs`, `Views/DanhGiaDuAn/_Form.cshtml`.
- **Thành phần:** `XacNhanNguyenNhanAsync`; select nguyên nhân.
- **Logic hiện tại:** View cho chọn toàn bộ danh mục, nhưng service yêu cầu tồn tại `AI_KET_QUA` của đúng dataset **và đúng mã nguyên nhân được chọn**.
- **Rủi ro:** AI đang gợi ý A nhưng Manager kết luận B sẽ bị từ chối, trái nguyên tắc AI chỉ hỗ trợ và Manager xác nhận cuối cùng.
- **Đề xuất:** Vẫn yêu cầu có kết quả phân tích chính thức của dataset hiện tại, nhưng không buộc mã xác nhận trùng mã dự đoán; validate B trong `DM_NGUYEN_NHAN`, rồi lưu B vào `AI_NGUYEN_NHAN`.

### 8.4. Trung bình — màn hình độc lập trộn nghiệp vụ Manager với kiểm thử model

- **File:** `Views/Ai/Predict.cshtml`, `AiController.cs`.
- **Thành phần:** `Predict`, `TestPredict`.
- **Logic hiện tại:** Cùng trang vừa chạy nghiệp vụ dự án vừa cho chọn model và chạy test.
- **Rủi ro:** Khó bỏ view ngay; quyền `AI.PhanTichNguyenNhan` đang cho tiếp cận công cụ test model dù quản trị model dùng `AI.Train`.
- **Đề xuất:** Khi di chuyển, đặt test model vào màn hình Models/Train và kiểm soát bằng `AI.Train`; route Predict chỉ giữ tương thích trong giai đoạn chuyển đổi.

### 8.5. Trung bình — ba UI riêng cho cùng nghiệp vụ

- **File:** `Views/Ai/Predict.cshtml`, `Views/DuAn/Details.cshtml`, `Views/DanhGiaDuAn/_AiInsightCard.cshtml`, `_Form.cshtml`.
- **Logic hiện tại:** Service lõi dùng chung nhưng markup, nhãn, form và JS riêng.
- **Rủi ro:** Quyền, thông báo, nguồn kết quả và trạng thái xác nhận dễ lệch.
- **Đề xuất:** Một ViewModel/panel read-only dùng chung; action chỉ đặt tại Details. Đánh giá dùng partial ở chế độ tham khảo.

### 8.6. Trung bình — tự động gọi AI trong form đánh giá

- **File:** `DanhGiaDuAnService.cs`, `Views/DanhGiaDuAn/_Form.cshtml`.
- **Thành phần:** `TuDongPhanTichAi`, JavaScript `submitAnalyze`.
- **Logic hiện tại:** Form có thể tự gọi phân tích khi thiếu/cũ.
- **Rủi ro:** Mở form đánh giá gây side effect ghi `AI_KET_QUA` chính thức mà người dùng không bấm chạy; lỗi AI chen vào workflow đánh giá.
- **Đề xuất:** Tắt auto-analyze và biến đánh giá thành nơi đọc dữ liệu AI.

### 8.7. Trung bình — màn hình cũ không có lịch sử chi tiết dù dữ liệu append

- **File:** `AiService.cs`, `AiPredictPageViewModel.cs`, `Views/Ai/Predict.cshtml`.
- **Thành phần:** `NapKetQuaPhanTichGanNhatAsync`.
- **Logic hiện tại:** Chỉ lấy bản ghi mới nhất.
- **Rủi ro:** Không thể coi view cũ là chức năng lịch sử cần giữ nguyên.
- **Đề xuất:** Giữ `Ai/Dashboard` cho tổng quan. Chỉ xây lịch sử chi tiết nếu có yêu cầu nghiệp vụ riêng; không dùng lý do “giữ lịch sử” để giữ menu Predict.

### 8.8. Trung bình — GET Predict có phạm vi rộng hơn quyền chạy

- **File:** `AiService.cs`.
- **Thành phần:** `KhoiTaoTrangPredictAsync`, `GetAccessibleProjectIdsAsync`.
- **Logic hiện tại:** Dropdown có thể chứa dự án user truy cập được, trong khi chạy chỉ cho Manager hiện tại.
- **Rủi ro:** User chọn được dự án nhưng POST bị từ chối.
- **Đề xuất:** Nếu route cũ còn hoạt động trong giai đoạn chuyển tiếp, lọc dropdown theo đúng tập có thể phân tích hoặc chuyển GET về Details.

### 8.9. Thấp — không có test tự động bảo vệ route/quyền AI

- **File:** toàn solution; không thấy test project tương ứng.
- **Rủi ro:** Bỏ menu/đổi route dễ gây hồi quy redirect, 403 và anti-forgery.
- **Đề xuất:** Bổ sung integration test theo ma trận mục 13 trong giai đoạn triển khai.

### 8.10. Thấp — tên `AI.DuDoan` là alias

- **File:** `Constants/Permissions.cs`.
- **Thành phần:** `Permissions.AI.DuDoan`.
- **Logic hiện tại:** Alias cùng giá trị với `AI.PhanTichNguyenNhan`, không phải permission độc lập.
- **Rủi ro:** Tài liệu hoặc code tương lai có thể hiểu nhầm là hai claim.
- **Đề xuất:** Giữ tương thích, nhưng chỉ dùng tên chuẩn `AI.PhanTichNguyenNhan` trong code mới.

## 9. Đề xuất luồng TO-BE

1. Người dùng mở `DuAn/Details/{id}`.
2. Backend kiểm tra `DuAn.Xem` và scope xem dự án.
3. Panel chung đọc trạng thái dự án, kết quả mới nhất và xác nhận hiện tại.
4. Chỉ Manager hiện tại có `AI.PhanTichNguyenNhan` mới thấy và gọi **Phân tích/Phân tích lại**.
5. `AiService.PhanTichNguyenNhanDuAnAsync` tiếp tục là cổng nghiệp vụ duy nhất:
   - Kiểm tra claim, role, scope và trạng thái.
   - Snapshot tạm thời cho dự án đang quá hạn; không ghi ground truth/kết quả chính thức.
   - Dataset chính thức cho dự án hoàn thành trễ.
   - Gửi đúng 22 feature đến FastAPI.
   - Áp hậu kiểm/fallback hiện có.
   - Chỉ lưu kết quả chính thức vào `AI_KET_QUA`.
6. Panel hiển thị AI dưới nhãn gợi ý/tham khảo.
7. Chỉ Manager hiện tại có `AI.XacNhan` mới xác nhận hoặc thay đổi nguyên nhân cuối cùng.
8. Xác nhận ghi `AI_NGUYEN_NHAN`; đồng bộ mã đã xác nhận sang `AI_DATASET.MaDMNguyenNhan`. Không lấy `AI_KET_QUA` làm label.
9. Form đánh giá dự án dùng lại panel read-only hoặc ViewModel chung:
   - Không chạy AI.
   - Không auto-analyze.
   - Hiển thị kết quả mới nhất và nguyên nhân Manager xác nhận.
   - Quyền đánh giá không thay thế quyền AI.
10. `Ai/Dashboard` tiếp tục phục vụ tổng quan/lịch sử tổng hợp, độc lập với menu chạy phân tích.
11. Bỏ link `Ai/Predict` khỏi sidebar.
12. Trong giai đoạn tương thích, GET `/Ai/Predict?maDuAn=x` redirect đến `/DuAn/Details/x#phan-tich-nguyen-nhan-tre`; nếu thiếu `maDuAn`, redirect về danh sách dự án phù hợp. POST cũ chỉ giữ đủ lâu để không đứt bookmark/form cũ, sau đó loại bỏ khi không còn consumer.

## 10. Danh sách file dự kiến cần chỉnh sửa

### 10.1. Bắt buộc sửa

| STT | File | Thành phần | Thay đổi dự kiến | Mức độ ảnh hưởng |
| --- | --- | --- | --- | --- |
| 1 | `Views/Shared/_Layout.cshtml` | Item `Ai/Predict` | Bỏ mục menu độc lập; giữ các item Dataset/Train/Models/Dashboard theo quyền | Thấp |
| 2 | `Controllers/AiController.cs` | GET/POST `Predict`, `TestPredict`, `XacNhanNguyenNhan` | Giữ GET redirect tương thích; xác định thời hạn deprecate POST; tách test model trước khi xóa view | Trung bình |
| 3 | `Services/Implementations/DanhGiaDuAnService.cs` | `GetFormAsync` | Không dùng quyền đánh giá để bật AI; bỏ auto-analyze; sửa điều kiện xác nhận theo `AI.XacNhan` | Cao |
| 4 | `Views/DanhGiaDuAn/_Form.cshtml` | Form ẩn và JavaScript AJAX | Bỏ hành động chạy/tự chạy AI; chỉ hiển thị tham khảo | Trung bình |
| 5 | `Views/DanhGiaDuAn/_AiInsightCard.cshtml` | Nút phân tích/card | Chuyển card sang read-only hoặc partial chung | Trung bình |
| 6 | `Services/Implementations/AiService.cs` | `XacNhanNguyenNhanAsync` | Giữ yêu cầu có phân tích chính thức nhưng cho Manager xác nhận mã hợp lệ khác dự đoán | Cao |

### 10.2. Có thể cần sửa

| STT | File | Thành phần | Thay đổi dự kiến | Mức độ ảnh hưởng |
| --- | --- | --- | --- | --- |
| 1 | `Views/DuAn/Details.cshtml` | Panel AI | Trích thành partial chung, bổ sung chọn nguyên nhân cuối cùng nếu nghiệp vụ cho phép | Trung bình |
| 2 | `ViewModels/Ai/AiProjectDelayAnalysisPanelViewModel.cs` | Panel model | Làm ViewModel chung cho Details và read-only evaluation | Trung bình |
| 3 | `ViewModels/DanhGiaDuAn/*` | Thuộc tính `CoThePhanTichAi`, `TuDongPhanTichAi` | Xóa thuộc tính chỉ phục vụ AJAX chạy AI sau khi hết consumer | Thấp |
| 4 | `Views/Ai/Predict.cshtml` | Toàn view | Xóa sau giai đoạn tương thích và sau khi tách `TestPredict` | Trung bình |
| 5 | `ViewModels/Ai/AiPredictPageViewModel.cs` | Page model | Xóa phần chỉ phục vụ view cũ sau khi không còn consumer | Trung bình |
| 6 | `wwwroot/css/AI/index.css` | Selector chỉ dùng Predict | Dọn selector không còn dùng, không ảnh hưởng Dashboard/Train/Models | Thấp |
| 7 | `Views/Ai/Models.cshtml` hoặc `Views/Ai/Train.cshtml` | Công cụ test model | Nhận phần `TestPredict` nếu vẫn cần quản trị | Trung bình |
| 8 | Tài liệu `docs/readme.md`, `docs/usecase.md`, `docs/workflow-he-thong.md`, `docs/phantichnguyennhantre.md`, `docs/danhgiaduan.md`, `docs/ctduan.md`, `docs/layoutmenu.md` | Link và mô tả AS-IS | Cập nhật sau triển khai | Thấp |
| 9 | Test project mới hoặc hiện có nếu được bổ sung | Integration tests | Bảo vệ route cũ, permission và scope | Trung bình |

### 10.3. Không nên sửa

| STT | File/thành phần | Lý do giữ |
| --- | --- | --- |
| 1 | `IAiService`, pipeline `AiService.PhanTichNguyenNhanDuAnAsync` | Cổng nghiệp vụ dùng chung đang đúng boundary MVC |
| 2 | `IAiApiService`, `AiApiService` và contract 22 feature | Không liên quan việc bỏ menu |
| 3 | FastAPI router/service/schema | FastAPI chỉ tính toán, không phụ thuộc menu MVC |
| 4 | Entity/DbContext/migration/schema AI | Không cần thay đổi dữ liệu hoặc schema |
| 5 | `AiDatasetService` và luồng train | Phải giữ nguồn label xác nhận |
| 6 | `AI.PhanTichNguyenNhan`, `AI.XacNhan`, `AI.Dashboard`, `AI.Dataset`, `AI.Train` | Bỏ menu không đồng nghĩa bỏ capability/claim |
| 7 | `Ai/Dashboard` | Còn chức năng tổng quan/lịch sử tổng hợp riêng |

## 11. Thành phần có thể xóa và thành phần phải giữ

### Có thể xóa

Chỉ sau khi hoàn tất chuyển đổi và xác nhận không còn consumer:

- Item `Ai/Predict` trong `_Layout.cshtml`.
- `Views/Ai/Predict.cshtml`.
- Các phần chỉ phục vụ page trong `AiPredictPageViewModel`.
- JavaScript đổi `maDuAn` inline trong `Predict.cshtml`.
- CSS selector chỉ phục vụ Predict.
- POST `AiController.Predict` và `AiController.XacNhanNguyenNhan` sau giai đoạn tương thích.
- `AiController.TestPredict` chỉ có thể xóa nếu chức năng test model được quyết định không còn cần; nếu còn cần thì phải chuyển sang Models/Train trước.

### Phải giữ

- `AiService.PhanTichNguyenNhanDuAnAsync`.
- `AiService.LayPhanTichNguyenNhanDuAnAsync`.
- `AiService.XacNhanNguyenNhanAsync`.
- `AiDatasetService`, 22 feature và quy tắc dataset tạm thời/chính thức.
- `AiApiService` và POST FastAPI `/analyze/delay-reason`.
- Lưu `AI_KET_QUA`.
- Lưu/soft-delete `AI_NGUYEN_NHAN`.
- Đồng bộ `AI_NGUYEN_NHAN` sang `AI_DATASET.MaDMNguyenNhan`.
- Các permission AI hiện có.
- `Ai/Dashboard` và dữ liệu lịch sử trong `AI_KET_QUA`.
- Luồng train chỉ lấy dòng `LaDuAnTre = true`, có `MaDMNguyenNhan` hợp lệ và đủ feature.
- Boundary MVC là system-of-record; FastAPI không ghi SQL Server.

## 12. Kế hoạch chỉnh sửa an toàn theo từng bước

1. Viết integration test cho permission, scope, trạng thái, anti-forgery và route hiện tại.
2. Chuẩn hóa một ViewModel/partial panel AI dùng chung; trước mắt giữ hành vi Details.
3. Sửa điều kiện quyền UI đánh giá cho khớp backend; tắt `TuDongPhanTichAi`.
4. Chuyển đánh giá sang read-only; loại bỏ form ẩn/JavaScript gọi `PhanTichAiDuAn`.
5. Nếu cần test model, chuyển `TestPredict` sang khu vực Models/Train và dùng `AI.Train`.
6. Bỏ item `Ai/Predict` khỏi `_Layout.cshtml`.
7. Chuyển mọi link nội bộ còn trỏ Predict sang `DuAn/Details/{id}#phan-tich-nguyen-nhan-tre`.
8. Giữ GET `/Ai/Predict` làm redirect tương thích:
   - Có `maDuAn`: về Details đúng dự án.
   - Không có `maDuAn`: về danh sách dự án.
9. Giữ POST cũ trong một phiên bản nếu còn bookmark/form ngoài hệ thống; log deprecation nếu pattern dự án có hỗ trợ.
10. Sửa xác nhận để Manager có thể chọn nguyên nhân cuối cùng khác AI nhưng vẫn yêu cầu dataset/kết quả chính thức hợp lệ.
11. Khi không còn consumer, xóa view/page ViewModel/action POST cũ và CSS/JS mồ côi.
12. Cập nhật tài liệu và chạy đầy đủ test/build/kiểm tra UTF-8.

Không xóa action/view cũ trước khi chuyển `TestPredict`, redirect, link nội bộ và test.

## 13. Tiêu chí kiểm thử sau chỉnh sửa

- Không còn item **Phân tích nguyên nhân trễ** độc lập trong menu.
- `/Ai/Predict?maDuAn=x` được redirect an toàn về chi tiết dự án và panel AI.
- URL cũ không tạo vòng lặp redirect, 404 hoặc mất query cần thiết.
- Manager đúng dự án, có `AI.PhanTichNguyenNhan`, chạy/chạy lại được.
- Manager ngoài phạm vi không chạy được kể cả POST URL trực tiếp.
- Người chỉ có quyền đánh giá nhưng không có quyền AI không thấy/không kích hoạt chạy AI; vẫn đọc được kết quả tham khảo theo scope.
- Người có quyền AI nhưng không đúng scope bị backend chặn.
- Admin không chạy hoặc xác nhận dù thử URL trực tiếp.
- Leader/Employee chỉ thấy dữ liệu read-only khi có quyền xem dự án; không có nút ghi.
- Dự án chưa quá hạn, không trễ hoặc thiếu dữ liệu không được phân tích.
- Dự án đang thực hiện quá hạn chỉ có kết quả tạm thời và không ghi `AI_KET_QUA`.
- Dự án hoàn thành trễ ghi đúng một bản kết quả mới vào `AI_KET_QUA` cho lần chạy hợp lệ.
- FastAPI lỗi không làm hỏng Details hoặc form đánh giá; hiển thị thông báo ngắn gọn.
- Xác nhận/thay đổi xác nhận yêu cầu `AI.XacNhan`, đúng Manager và ghi đúng `AI_NGUYEN_NHAN`.
- Nguyên nhân Manager chọn có thể khác gợi ý AI nếu thuộc danh mục hợp lệ.
- `AI_DATASET.MaDMNguyenNhan` nhận nhãn từ xác nhận Manager.
- `AI_KET_QUA` không bao giờ được dùng tự động làm nhãn train.
- Mở lại dự án vẫn soft-delete xác nhận và bỏ label như `DuAnService.MoLaiDuAnAsync`, không xóa lịch sử kết quả.
- Dashboard AI vẫn đọc được số lượt phân tích/lịch sử tổng hợp.
- Không có lỗi JavaScript console, AJAX 403 do nút hiển thị sai, route hoặc redirect.
- Mọi POST còn lại có anti-forgery và backend authorization.
- Build .NET 8 thành công.
- File/view tiếng Việt hiển thị đúng UTF-8, không có `PhÃ¢n tÃ­ch`, `NguyÃªn nhÃ¢n`, `Dá»± Ã¡n`.

## 14. Kết luận cuối cùng

Phương án duy nhất được khuyến nghị là:

**Bỏ mục `Ai/Predict` khỏi menu; lấy `DuAn/Details` làm nơi chính thức để chạy, chạy lại, xem và xác nhận nguyên nhân; biến khu AI trong đánh giá dự án thành dữ liệu chỉ đọc dùng cho quyết định đánh giá; giữ `Ai/Dashboard` cho tổng quan; giữ GET route cũ dưới dạng redirect tương thích trong một giai đoạn và chỉ xóa view/action cũ sau khi đã tách `TestPredict` cùng mọi link/consumer.**

Permission phải giữ ranh giới rõ:

- `DuAn.Xem`/quyền đánh giá + scope: đọc kết quả trong đúng ngữ cảnh.
- `AI.PhanTichNguyenNhan` + Manager hiện tại: chạy/chạy lại.
- `AI.XacNhan` + Manager hiện tại: xác nhận/thay đổi ground truth.
- `DanhGiaDuAn.*`: không thay thế quyền AI.

Phương án này không thay đổi schema, database, contract FastAPI hay boundary MVC/FastAPI; không làm mất `AI_KET_QUA`, `AI_NGUYEN_NHAN`, dataset, train hoặc dashboard.
