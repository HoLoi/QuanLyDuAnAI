# Phân tích Dashboard Tổng quan AI

Ngày lập: 2026-07-03

Tài liệu này chỉ phân tích source hiện tại của màn hình `Tổng quan AI`. Đây không phải tài liệu giới thiệu hệ thống và không đề xuất thay đổi schema/workflow. Phạm vi đã đọc gồm MVC controller/service/interface/viewmodel/entity/view/js, FastAPI router/service/schema/model service/validation/prediction, DbContext/migration/SQL seed và các tài liệu nền:

- `docs/ai-he-thong-phan-tich.md`
- `docs/ai-train-dataset-review.md`
- `docs/mvc-ai-integration-rules.md`
- `docs/workflow-he-thong.md`

## 1. Kiến trúc Dashboard AI

Luồng render màn hình:

```text
AiController.Dashboard()
↓
IAiService.LayDashboardAsync()
↓
AiService.LayDashboardAsync()
↓
QuanLyDuAnDbContext + IAiApiService
↓
AiDashboardPageViewModel
↓
Views/Ai/Dashboard.cshtml
↓
Chart.js + wwwroot/js/ai/charts.js
```

Các hàm chính tham gia:

| Lớp/file | Hàm/property | Vai trò |
| --- | --- | --- |
| `Controllers/AiController.cs` | `Dashboard(CancellationToken)` | Kiểm tra quyền `AI.Dashboard`, gọi `_aiService.LayDashboardAsync`, trả view. |
| `Services/Interfaces/IAiService.cs` | `LayDashboardAsync` | Contract service tạo ViewModel Dashboard AI. |
| `Services/Implementations/AiService.cs` | `LayDashboardAsync` | Tổng hợp toàn bộ số liệu card, chart, bảng cảnh báo. |
| `Services/Implementations/AiService.cs` | `LayLuotPhanTichAiTheoThangAsync` | Tạo 12 mốc tháng cho line chart lượt phân tích/xác nhận. |
| `Services/Implementations/AiService.cs` | `LayNguyenNhanTheoQuanLyAsync` | Tạo bảng nguyên nhân theo quản lý. |
| `Services/Implementations/AiService.cs` | `LayNguyenNhanTheoTeamAsync` | Tạo bảng nguyên nhân theo team. |
| `Services/Implementations/AiService.cs` | `BuildModelQualityWarnings` | Tạo cảnh báo chất lượng model. |
| `Services/Implementations/AiApiService.cs` | `KiemTraSucKhoeAsync`, `LayChiTietModelAsync`, `LayTrangThaiAdminAsync`, `LayTongHopLogAsync`, `LayThongTinHeThongAsync` | Gọi FastAPI để lấy trạng thái service, model active, metadata model, logs/system info. |
| `Data/QuanLyDuAnDbContext.cs` | `AiModel`, `AiKetQua`, `AiNguyenNhan`, `AiDataset`, `DmNguyenNhan` | EF DbSet/tables được Dashboard đọc trực tiếp. |
| `ViewModels/Ai/AiDashboardPageViewModel.cs` | Các property `Tong...`, `NguyenNhan...`, `LichSuModel...` | DTO/ViewModel cấp dữ liệu cho Razor. |
| `Views/Ai/Dashboard.cshtml` | inline Razor + script Chart.js | Hiển thị card, bảng, serialize list sang JavaScript. |
| `wwwroot/js/ai/charts.js` | `AiChartHelper.render` | Helper tạo/destroy Chart.js, không tự tính nghiệp vụ. |

Luồng xuất file Dashboard AI:

```text
AiController.XuatFile()
↓
IAiService.LayDashboardAsync()
↓
BuildDashboardExportRows()
↓
IExportFileService.Export()
```

`XuatFile` yêu cầu đồng thời quyền `AI.Dashboard` và `ThongKe.XuatFile`. Nội dung export chỉ lấy một phần ViewModel: tổng lượt phân tích, tổng xác nhận, tỷ lệ xác nhận, dataset, top nguyên nhân, theo quản lý, theo team.

## 2. Luồng lấy dữ liệu

### 2.1 Phân quyền dữ liệu

Trong `AiService.LayDashboardAsync`:

- Lấy role hiện tại bằng `GetCurrentUserRoleFlagsAsync`.
- Lấy mã người dùng bằng `GetCurrentUserIdAsync`.
- Lấy danh sách dự án được phép bằng `GetAccessibleProjectIdsAsync`.
- Nếu không phải Admin thì giới hạn `AI_KET_QUA`, `AI_NGUYEN_NHAN`, `AI_DATASET` theo `projectIds`.
- `AI_MODEL` không giới hạn theo dự án; Dashboard đếm global các model `LoaiModel == "NguyenNhan"` và `IsDeleted != true`.

### 2.2 Nguồn DB và API

```text
Database SQL Server
  AI_MODEL
  AI_KET_QUA
  AI_NGUYEN_NHAN
  AI_DATASET
  DM_NGUYEN_NHAN
  DU_AN / NGUOI_DUNG / TEAM_DU_AN / TEAM
↓
EF Core trong AiService.LayDashboardAsync
↓
AiDashboardPageViewModel
↓
Razor card/table
↓
JsonSerializer.Serialize(...) trong Dashboard.cshtml
↓
Chart.js
```

FastAPI không đọc database. FastAPI chỉ cung cấp:

- `/health`: trạng thái service và model nguyên nhân đang nạp.
- `/admin/ai-status`: trạng thái admin, số model local, ngưỡng train.
- `/admin/logs/summary`: log train/analyze.
- `/admin/system-info`: Python/sklearn/pandas/uptime.
- `/admin/model/{model_file}`: metadata model, gồm accuracy/precision/recall/F1/feature importance/confusion matrix.

## 3. Phân tích từng Card thống kê

### 3.1 Trạng thái service

- View: `Views/Ai/Dashboard.cshtml`, card `Trạng thái service`.
- Property: `Model.Health.ServiceStatus`.
- Service: `AiService.LayDashboardAsync` gọi `_aiApiService.KiemTraSucKhoeAsync`.
- FastAPI: `health_router.health_check`.
- Endpoint: `GET /health`.
- Công thức: không có công thức, lấy string `serviceStatus`.
- Điều kiện: nếu call lỗi, `vm.CanhBao = health.ThongBao`.

### 3.2 Model active

- View: card `Model active`.
- Property: `Model.ModelNguyenNhanDangHoatDong`.
- Service: `AiService.LayDashboardAsync`.
- Nguồn: `health.DuLieu.LoadedReasonModel`.
- FastAPI: `model_service.get_loaded_models()` trả model `NguyenNhan` đang nạp trong memory.
- Lưu ý: không lấy trực tiếp từ `AI_MODEL.IsActive`. Vì vậy model active trên card có thể là file FastAPI đang load, còn lịch sử model lấy từ DB.

### 3.3 Tổng model nguyên nhân

- View: `@Model.TongModelTrongDb`.
- Service: `AiService.LayDashboardAsync`.
- Query tương đương:

```sql
SELECT COUNT(*)
FROM AI_MODEL
WHERE ISNULL(IsDeleted, 0) <> 1
  AND LoaiModel = N'NguyenNhan';
```

- Điều kiện lọc: `IsDeleted != true`, `LoaiModel == "NguyenNhan"`.
- Không giới hạn theo quyền dự án.

### 3.4 Tổng lượt phân tích AI

- View: `@Model.TongLanPhanTichTrongDb`.
- Service: `AiService.LayDashboardAsync`.
- Bảng: `AI_KET_QUA`.
- Query tương đương cho Admin:

```sql
SELECT COUNT(*) FROM AI_KET_QUA;
```

- Query tương đương cho non-admin:

```sql
SELECT COUNT(*)
FROM AI_KET_QUA
WHERE MaDuAn IN (...projectIds...);
```

- Điều kiện lọc: chỉ có scope dự án với non-admin. Không lọc `ReasonSource`, không lọc thời gian, không distinct theo dự án.
- Ý nghĩa hiện tại: số dòng kết quả phân tích/dự đoán đã lưu trong `AI_KET_QUA`.

### 3.5 Tổng xác nhận nguyên nhân

- View: `@Model.TongXacNhanNguyenNhanTrongDb`.
- Service: `AiService.LayDashboardAsync`.
- Bảng: `AI_NGUYEN_NHAN`.
- Query tương đương cho Admin:

```sql
SELECT COUNT(*)
FROM AI_NGUYEN_NHAN
WHERE ISNULL(IsDeleted, 0) <> 1;
```

- Query tương đương cho non-admin:

```sql
SELECT COUNT(*)
FROM AI_NGUYEN_NHAN
WHERE ISNULL(IsDeleted, 0) <> 1
  AND MaDuAn IN (...projectIds...);
```

- Ý nghĩa hiện tại: số dòng xác nhận nguyên nhân không bị soft-delete. Không distinct theo dự án.

### 3.6 Tỷ lệ xác nhận AI

- View: `@Model.TyLeXacNhanAi.ToString("0.##")%`.
- Ghi chú view: `Xác nhận nguyên nhân so với lượt phân tích.`
- Code hiện tại:

```csharp
vm.TyLeXacNhanAi = vm.TongLanPhanTichTrongDb > 0
    ? Math.Round((double)vm.TongXacNhanNguyenNhanTrongDb * 100d / vm.TongLanPhanTichTrongDb, 2)
    : 0d;
```

- Công thức toán học hiện tại:

```text
TyLeXacNhanAi = COUNT(AI_NGUYEN_NHAN where IsDeleted != true) * 100 / COUNT(AI_KET_QUA)
```

- Vấn đề: tử số và mẫu số không cùng đơn vị nghiệp vụ. `AI_NGUYEN_NHAN` là xác nhận/ground-truth của quản lý, có thể được seed hoặc tạo độc lập với `AI_KET_QUA`; `AI_KET_QUA` là kết quả phân tích/dự đoán đã lưu. Vì vậy tỷ lệ có thể lớn hơn 100%.
- Ví dụ nếu DB có 520 dòng `AI_NGUYEN_NHAN` và 1 dòng `AI_KET_QUA`: `520 * 100 / 1 = 52000%`.

### 3.7 Tổng dòng dataset

- View: `@Model.TongDongDataset`.
- Service: `AiService.LayDashboardAsync`.
- Code: lấy latest dataset theo từng `MaDuAn`, rồi `Count`.
- Query tương đương:

```sql
WITH LatestDataset AS (
    SELECT *,
           ROW_NUMBER() OVER (
               PARTITION BY MaDuAn
               ORDER BY ISNULL(NgayTongHop, '0001-01-01') DESC, MaData DESC
           ) AS rn
    FROM AI_DATASET
)
SELECT COUNT(*)
FROM LatestDataset
WHERE rn = 1;
```

- Ý nghĩa hiện tại: số dự án có snapshot dataset mới nhất, không phải tổng số dòng vật lý trong `AI_DATASET`.
- Vấn đề nhãn: card ghi `Tổng dòng dataset`, nhưng source đang đếm một snapshot mới nhất cho mỗi dự án.

### 3.8 Dòng đủ điều kiện train

- View: `@Model.TongDongDatasetHopLeTrain`.
- Service: `AiService.LayDashboardAsync`.
- Nguồn: `latestDatasetByProject`.
- Điều kiện:
  - `LaDuAnTre == true`
  - `MaDMNguyenNhan.HasValue`
  - đủ 22 feature không null.
- Vấn đề: card này không áp dụng filter lớp train thật (`MinReasonRowsPerClass = 5`, `MinReasonTrainRows = 30`, ít nhất 2 lớp). `AiDatasetService.PhanLoaiDatasetNguyenNhanDeTrainAsync` mới là nơi lọc lớp đủ điều kiện thật. Vì vậy card có thể lớn hơn số dòng thật sự được dùng để train.

### 3.9 Dự án trễ đã xác nhận

- View: `@Model.SoDuAnTreDaXacNhan`.
- Service: `AiService.LayDashboardAsync`.
- Công thức hiện tại:

```text
SoDuAnTreDaXacNhan = max(0, SoDuAnDuocXacDinhTre - SoDuAnTreChuaXacNhan)
```

- Trong đó:
  - `SoDuAnDuocXacDinhTre = latestDatasetByProject.Count(LaDuAnTre == true)`
  - `SoDuAnTreChuaXacNhan = latestDatasetByProject.Count(LaDuAnTre == true && MaDMNguyenNhan is null)`
- Ý nghĩa: số dự án trễ trong latest dataset có `MaDMNguyenNhan`.

### 3.10 Dự án trễ chưa xác nhận

- View: `@Model.SoDuAnTreChuaXacNhan`.
- Service: `AiService.LayDashboardAsync`.
- Bảng: `AI_DATASET`, snapshot mới nhất mỗi dự án.
- Điều kiện: `LaDuAnTre == true && !MaDMNguyenNhan.HasValue`.

### 3.11 Accuracy / Precision macro / Recall macro / F1 macro

- View: `AccuracyModelActive`, `PrecisionMacroModelActive`, `RecallMacroModelActive`, `F1MacroModelActive`.
- Service: `AiService.LayDashboardAsync`.
- Nguồn chính: FastAPI metadata active model qua `_aiApiService.LayChiTietModelAsync(vm.ModelNguyenNhanDangHoatDong)`.
- Keys đọc:
  - `accuracy`
  - `precision_macro` hoặc `precisionMacro`
  - `recall_macro` hoặc `recallMacro`
  - `f1_macro` hoặc `f1Macro`
- Format view: `ToString("P2")`, tức `0.85` hiển thị `85.00%`.
- Nếu metadata không có giá trị: hiển thị `Chưa có dữ liệu`.

## 4. Phân tích từng biểu đồ

### 4.1 Phân bố nguyên nhân đã xác nhận

- View: `reasonDistributionDashboardChart`.
- Chart type: `pie`.
- Source ViewModel: `Model.NguyenNhanPhoBien`.
- Service: `AiService.LayDashboardAsync`.
- Bảng đọc: `AI_NGUYEN_NHAN` join tên qua `DM_NGUYEN_NHAN`.
- Query source:

```csharp
var phanBoNguyenNhan = await aiNguyenNhanQuery
    .GroupBy(x => x.MaDMNguyenNhan)
    .Select(g => new { MaDm = g.Key, SoLan = g.Count() })
    .OrderByDescending(x => x.SoLan)
    .ToListAsync(cancellationToken);
var tongSoLan = phanBoNguyenNhan.Sum(x => x.SoLan);
vm.NguyenNhanPhoBien = phanBoNguyenNhan.Take(5).Select(x => new NguyenNhanThongKeItemViewModel
{
    NguyenNhan = ...,
    TyLePhanTram = tongSoLan > 0
        ? (int)Math.Round((double)x.SoLan * 100d / tongSoLan, MidpointRounding.AwayFromZero)
        : 0
}).ToList();
```

- SQL tương đương:

```sql
SELECT TOP (5)
       nn.MaDMNguyenNhan,
       dm.TenNguyenNhan,
       COUNT(*) AS SoLan
FROM AI_NGUYEN_NHAN nn
LEFT JOIN DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = nn.MaDMNguyenNhan
WHERE ISNULL(nn.IsDeleted, 0) <> 1
GROUP BY nn.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY COUNT(*) DESC;
```

- Dữ liệu đưa vào chart: `TyLePhanTram`, không phải `SoLan`.
- Vấn đề 1: `Take(5)` nhưng phần trăm được tính trên tổng tất cả nguyên nhân. Nếu có 10 nguyên nhân đều 10%, Dashboard chỉ đưa 5 giá trị `[10,10,10,10,10]` vào pie. Chart.js sẽ vẽ 5 lát bằng nhau và normalize 5 lát đó thành toàn bộ hình tròn, làm mất 50% còn lại.
- Vấn đề 2: ViewModel `NguyenNhanThongKeItemViewModel` chỉ có `TyLePhanTram`, không giữ `SoLan`, nên view không thể hiển thị count thật.
- Vấn đề 3: nếu seed phân bố đều theo `AI_NGUYEN_NHAN.MaDMNguyenNhan`, pie chia đều là đúng theo dữ liệu seed, nhưng vẫn thiếu lát `Khác/nhóm còn lại` nếu có hơn 5 nguyên nhân.

### 4.2 Dataset theo trạng thái train

- View: `datasetTrainStateChart`.
- Chart type: `bar`.
- Dữ liệu:

```javascript
data: [Model.TongDongDatasetHopLeTrain,
       Math.max(Model.TongDongDataset - Model.TongDongDatasetHopLeTrain, 0)]
```

- Nguồn: latest dataset mỗi dự án từ `AI_DATASET`.
- Ý nghĩa hiện tại: chia latest snapshot theo đủ/không đủ điều kiện cơ bản.
- Vấn đề: label chart là `Số dòng`, nhưng `TongDongDataset` trong Dashboard là số latest snapshot theo dự án, không phải toàn bộ dòng vật lý. Ngoài ra `TongDongDatasetHopLeTrain` chưa lọc lớp đủ điều kiện train thật.

### 4.3 Lượt phân tích và xác nhận theo tháng

- View: `aiAnalysisTimelineChart`.
- Chart type: `line`.
- Service: `LayLuotPhanTichAiTheoThangAsync`.
- Khoảng thời gian: từ đầu tháng hiện tại trừ 11 tháng đến hiện tại, tổng 12 tháng.
- Nguồn:
  - `AI_KET_QUA.ThoiGianDuDoanKetQua` cho lượt phân tích.
  - `AI_NGUYEN_NHAN.NgayXacNhan` cho lượt xác nhận.
- Grouping: load list DateTime về memory rồi count theo `Year` và `Month`.
- Điều kiện: bỏ dòng không có thời gian.
- Vấn đề nghiệp vụ: hai series vẫn là hai bảng khác đơn vị; có thể xác nhận nhiều hơn phân tích trong cùng tháng.

### 4.4 Accuracy các phiên bản model nguyên nhân

- View: `accuracyByVersionDashboardChart`.
- Chart type: `line`.
- Nguồn: `AI_MODEL`.
- Service: `AiService.LayDashboardAsync`, `modelRows`.
- Order: `NgayTao DESC`, sau đó `MaModel DESC`.
- Label: `CreatedAt` hoặc `TenModel`.
- Value: `Math.Round((x.Accuracy ?? 0d) * 100d, 2)`.
- Vấn đề: model thiếu `DoChinhXac` được hiển thị thành `0%`, không phân biệt “không có dữ liệu” với accuracy bằng 0.

### 4.5 Feature importance của model active

- View: `activeFeatureImportanceDashboardChart`.
- Chart type: horizontal bar (`indexAxis: 'y'`).
- Nguồn: FastAPI metadata active model.
- Service MVC: `ReadFeatureImportance`.
- FastAPI metadata key: `feature_importance` hoặc `featureImportance`.
- Label mapping: `wwwroot/js/ai/charts.js` `FEATURE_LABEL_MAP`.
- Vấn đề: không sort ở view. Thứ tự phụ thuộc thứ tự key trong metadata JSON.

### 4.6 Bảng nguyên nhân theo quản lý

- Không phải chart nhưng nằm trong Dashboard.
- Service: `LayNguyenNhanTheoQuanLyAsync`.
- Nguồn: `AI_NGUYEN_NHAN` join `DU_AN`, `NGUOI_DUNG`, `DM_NGUYEN_NHAN`.
- Group by: quản lý dự án + nguyên nhân.
- Order: `SoLan DESC`.
- Limit: `Take(8)`.

### 4.7 Bảng nguyên nhân theo team

- Không phải chart nhưng nằm trong Dashboard.
- Service: `LayNguyenNhanTheoTeamAsync`.
- Nguồn: `AI_NGUYEN_NHAN` join `TEAM_DU_AN`, `TEAM`, `DM_NGUYEN_NHAN`.
- Group by: team + nguyên nhân.
- Order: `SoLan DESC`.
- Limit: `Take(8)`.
- Vấn đề cần lưu ý: nếu một dự án thuộc nhiều team thì một xác nhận có thể xuất hiện ở nhiều team theo join `TEAM_DU_AN`.

### 4.8 Cảnh báo dự án trễ theo AI

- Không phải chart nhưng là bảng Dashboard.
- Service: `AiService.LayDashboardAsync`.
- Nguồn:
  - latest `AI_KET_QUA` theo `MaDuAn`.
  - latest `AI_DATASET` theo `MaDuAn`.
  - `DM_NGUYEN_NHAN`.
  - `DU_AN`.
- Điều kiện: chỉ lấy dự án latest dataset có `LaDuAnTre == true`.
- Order: `ThoiGianDuDoan DESC`.
- Limit: `Take(8)`.
- Cờ stale: `KetQuaAiCoTheDaCu = x.MaData != dataset.MaData`.

## 5. Phân tích các phép tính

| Tên | Code đang tính | Công thức toán học | Nhận xét |
| --- | --- | --- | --- |
| Tổng model nguyên nhân | `aiModelQuery.CountAsync()` | `COUNT(AI_MODEL WHERE IsDeleted != true AND LoaiModel='NguyenNhan')` | Global DB, không theo project scope. |
| Tổng lượt phân tích AI | `aiKetQuaQuery.CountAsync()` | `COUNT(AI_KET_QUA)` theo scope dự án | Đếm dòng kết quả, không distinct dự án. |
| Tổng xác nhận nguyên nhân | `aiNguyenNhanQuery.CountAsync()` | `COUNT(AI_NGUYEN_NHAN WHERE IsDeleted != true)` theo scope dự án | Đếm dòng xác nhận, không distinct dự án. |
| Tỷ lệ xác nhận AI | `TongXacNhan * 100 / TongLanPhanTich` | `COUNT(AI_NGUYEN_NHAN) * 100 / COUNT(AI_KET_QUA)` | Có thể >100% vì tử/mẫu khác đơn vị. |
| Tổng dòng dataset | `latestDatasetByProject.Count` | `COUNT(latest AI_DATASET per MaDuAn)` | Nhãn “dòng dataset” dễ hiểu nhầm là tổng dòng vật lý. |
| Dự án trễ | `Count(LaDuAnTre == true)` | `COUNT(latest dataset WHERE LaDuAnTre=1)` | Dựa trên snapshot mới nhất từng dự án. |
| Dự án trễ chưa xác nhận | `Count(LaDuAnTre && !MaDMNguyenNhan.HasValue)` | `COUNT(latest dataset WHERE LaDuAnTre=1 AND MaDMNguyenNhan IS NULL)` | Hợp lý theo snapshot. |
| Dự án trễ đã xác nhận | `Max(0, SoDuAnTre - SoDuAnTreChuaXacNhan)` | `COUNT(latest dataset WHERE LaDuAnTre=1 AND MaDMNguyenNhan IS NOT NULL)` | Công thức gián tiếp, tương đương nếu hai biến trước đúng. |
| Dòng đủ điều kiện train | count đủ 22 feature + label trên latest dataset | `COUNT(latest dataset WHERE LaDuAnTre=1 AND MaDMNguyenNhan IS NOT NULL AND all features not null)` | Thiếu filter lớp train thật. |
| Phân bố nguyên nhân | `Round(SoLan * 100 / tongSoLan)` rồi `Take(5)` | phần trăm mỗi nguyên nhân trên tổng `AI_NGUYEN_NHAN` | Đưa phần trăm top 5 vào pie gây méo tỷ trọng hiển thị. |
| Timeline | count theo tháng | `COUNT(AI_KET_QUA by month)`, `COUNT(AI_NGUYEN_NHAN by month)` | Hai series khác nguồn. |
| Accuracy chart | `(x.Accuracy ?? 0) * 100` | accuracy DB thành % | Null bị biến thành 0%. |
| Accuracy active | `TryReadDouble(metadata,"accuracy")` + `ToString("P2")` | metadata value dạng 0..1 | Lấy từ FastAPI metadata, không từ DB. |
| Precision/Recall/F1 active | đọc metadata macro keys + `ToString("P2")` | metric macro dạng 0..1 | Không tự tính trong MVC. |
| Confidence cảnh báo | `ChuanHoaDoTinCay` | nếu >1 và <=100 thì chia 100, clamp 0..1 | Chống dữ liệu confidence lưu dạng %. |

## 6. Đối chiếu Database

| Bảng | Dashboard đọc cột nào | Mục đích |
| --- | --- | --- |
| `AI_MODEL` | `MaModel`, `TenModel`, `NgayTao`, `DoChinhXac`, `TrainSize`, `TestSize`, `IsActive`, `IsDeleted`, `LoaiModel` | Tổng model, lịch sử accuracy, cảnh báo model. |
| `AI_KET_QUA` | `MaAiKetQua`, `MaDuAn`, `MaData`, `MaModel`, `MaDMNguyenNhan`, `DoTinCayKetQua`, `ThoiGianDuDoanKetQua`, `ReasonSource` | Tổng lượt phân tích, timeline, cảnh báo dự án trễ theo AI. |
| `AI_NGUYEN_NHAN` | `MaAINguyenNhan`, `MaDuAn`, `MaDMNguyenNhan`, `NgayXacNhan`, `IsDeleted` | Tổng xác nhận, tỷ lệ xác nhận, phân bố nguyên nhân, theo quản lý/team, timeline xác nhận. |
| `AI_DATASET` | `MaData`, `MaDuAn`, `NgayTongHop`, `LaDuAnTre`, `MaDMNguyenNhan`, toàn bộ 22 feature | Dataset cards, trạng thái train, xác định dự án trễ/chưa xác nhận, stale check cho `AI_KET_QUA`. |
| `DM_NGUYEN_NHAN` | `MaDMNguyenNhan`, `TenNguyenNhan` | Map mã nguyên nhân sang tên. |
| `DU_AN` | `MaDuAn`, `TenDuAn`, `MaNguoiDung`, `IsDeleted` | Scope dự án, tên dự án, quản lý dự án. |
| `NGUOI_DUNG` | `MaNguoiDung`, `HoTenNguoiDung`, `IsDeleted` | Bảng nguyên nhân theo quản lý. |
| `TEAM_DU_AN` | `MaDuAn`, `MaTeam` | Bảng nguyên nhân theo team. |
| `TEAM` | `MaTeam`, `TenTeam`, `IsDeleted` | Tên team. |
| `NHAN_VIEN_DU_AN` | `MaNguoiDung`, `MaDuAn` | Scope dự án của employee/manager. |

DbContext/migration hiện có các bảng AI chính. Một số SQL cũ như `quanlyduan.sql` chỉ có schema/dữ liệu AI cũ hơn, trong khi `data2.sql`, `data3.sql`, `data7.sql`, `10duan.sql` đã seed tên nguyên nhân chuẩn mới. Các script seed lớn có thêm/kiểm tra các cột AI_DATASET mở rộng.

## 7. Kiểm tra tính hợp lý Dashboard

Kết luận theo source:

- `Tỷ lệ xác nhận AI` có thể hiển thị lớn hơn 100% vì lấy `COUNT(AI_NGUYEN_NHAN)` chia cho `COUNT(AI_KET_QUA)`. Đây là lỗi logic/định nghĩa chỉ số, không phải lỗi format.
- Pie chart `Phân bố nguyên nhân đã xác nhận` đang đọc đúng bảng `AI_NGUYEN_NHAN`, nhưng dữ liệu đưa vào chart là phần trăm top 5, không phải số lần. Khi thiếu nhóm “còn lại”, hình pie không phản ánh đúng tỷ trọng toàn bộ.
- `Tổng dòng dataset` thực tế là số latest snapshot theo dự án. Nếu DB có nhiều snapshot cho một dự án, card không phải tổng dòng `AI_DATASET`.
- `Dòng đủ điều kiện train` trên Dashboard không trùng hoàn toàn với “số dòng dùng train” thật của `AiDatasetService`, vì chưa áp dụng điều kiện tối thiểu theo lớp.
- `Tổng model nguyên nhân` lấy từ DB, còn `Model active` lấy từ FastAPI memory. Hai nguồn có thể lệch nếu DB và thư mục model chưa đồng bộ.
- Accuracy chart biến `null` thành `0%`, có thể gây hiểu nhầm “model rất kém” thay vì “thiếu dữ liệu”.
- Bảng theo team có thể nhân số lần nếu một dự án liên kết nhiều team.

## 8. Danh sách BUG

| STT | Tên lỗi | Mức độ | Nguyên nhân | File | Method | Biến liên quan | Công thức hiện tại | Công thức đúng nên là gì |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | `Tỷ lệ xác nhận AI` có thể >100% | Cao | Tử số là số dòng `AI_NGUYEN_NHAN`, mẫu số là số dòng `AI_KET_QUA`; hai bảng không cùng đơn vị đo. | `Services/Implementations/AiService.cs` | `LayDashboardAsync` | `TongXacNhanNguyenNhanTrongDb`, `TongLanPhanTichTrongDb`, `TyLeXacNhanAi` | `COUNT(AI_NGUYEN_NHAN) * 100 / COUNT(AI_KET_QUA)` | Chọn một định nghĩa thống nhất: tỷ lệ dự án trễ đã xác nhận trên tổng dự án trễ trong latest dataset, hoặc tỷ lệ kết quả phân tích đã có xác nhận theo cùng `MaDuAn`; không trộn tổng dòng hai bảng độc lập. |
| 2 | Pie chart top 5 dùng phần trăm nhưng không có nhóm còn lại | Cao | Service tính phần trăm trên tổng tất cả nguyên nhân, sau đó `Take(5)`; Chart.js pie normalize các giá trị được đưa vào thành 100% hình tròn. | `Services/Implementations/AiService.cs`, `Views/Ai/Dashboard.cshtml` | `LayDashboardAsync`, inline chart script | `NguyenNhanPhoBien`, `TyLePhanTram`, `reasonValues` | `Top5(Round(SoLan*100/TongSoLan))` đưa vào pie | Dùng `SoLan` làm data pie, hoặc nếu dùng phần trăm thì thêm nhóm “Khác/còn lại” để tổng các lát bằng 100% thật. |
| 3 | `Dòng đủ điều kiện train` không phản ánh số dòng train thật | Trung bình | Dashboard chỉ check đủ feature/label trên latest dataset; service train còn lọc lớp tối thiểu 5 dòng/lớp, 30 dòng, 2 lớp. | `Services/Implementations/AiService.cs`, `Services/Implementations/AiDatasetService.cs` | `LayDashboardAsync`, `PhanLoaiDatasetNguyenNhanDeTrainAsync` | `TongDongDatasetHopLeTrain`, `MinReasonRowsPerClass`, `MinReasonTrainRows` | Count đủ 22 feature + label | Gọi hoặc tái dùng kết quả phân loại train để hiển thị “dòng được dùng train” thật; nếu giữ công thức hiện tại thì đổi nhãn thành “Dòng đủ feature và label”. |
| 4 | Nhãn `Tổng dòng dataset` dễ sai nghĩa | Trung bình | Source đếm latest snapshot theo `MaDuAn`, không đếm toàn bộ `AI_DATASET`. | `Services/Implementations/AiService.cs`, `Views/Ai/Dashboard.cshtml` | `LayDashboardAsync` | `latestDatasetByProject`, `TongDongDataset` | `COUNT(latest dataset per project)` | Đổi nhãn thành “Số dự án có dataset mới nhất” hoặc đổi query sang `COUNT(AI_DATASET)` nếu muốn tổng dòng vật lý. |
| 5 | Model active và tổng/lịch sử model lấy từ hai nguồn khác nhau | Trung bình | Active model lấy từ FastAPI health, tổng/lịch sử lấy từ DB `AI_MODEL`. | `Services/Implementations/AiService.cs`, `Services/Implementations/AiApiService.cs` | `LayDashboardAsync`, `KiemTraSucKhoeAsync` | `ModelNguyenNhanDangHoatDong`, `TongModelTrongDb`, `LichSuModelNguyenNhan` | Nguồn mixed FastAPI memory + DB | Đồng bộ rõ nguồn, hoặc hiển thị thêm trạng thái “active file có/không có trong DB”. |
| 6 | Accuracy phiên bản model thiếu dữ liệu bị vẽ 0% | Thấp | View dùng `(x.Accuracy ?? 0d) * 100d`. | `Views/Ai/Dashboard.cshtml` | inline chart script | `modelAccuracy` | Null => 0 | Bỏ điểm null khỏi chart hoặc hiển thị trạng thái thiếu dữ liệu riêng. |
| 7 | Bảng nguyên nhân theo team có thể nhân dòng | Thấp/Trung bình | Join `AI_NGUYEN_NHAN` với `TEAM_DU_AN`; một dự án nhiều team tạo nhiều dòng nhóm. | `Services/Implementations/AiService.cs` | `LayNguyenNhanTheoTeamAsync` | `NguyenNhanTheoTeam`, `TeamDuAn` | Count theo join team-dự án | Nếu cần thống kê xác nhận theo dự án duy nhất, phải định nghĩa lại cách phân bổ cho nhiều team. |

## 9. Đề xuất hướng sửa

Không viết code trong tài liệu này. Hướng sửa nên đi theo thứ tự:

1. Sửa `AiService.LayDashboardAsync` cho card tỷ lệ:
   - Nếu muốn tỷ lệ xác nhận của dataset: dùng `SoDuAnTreDaXacNhan / SoDuAnDuocXacDinhTre * 100`.
   - Nếu muốn tỷ lệ phân tích đã xác nhận: join/deduplicate theo cùng `MaDuAn` giữa latest `AI_KET_QUA` và latest `AI_NGUYEN_NHAN`, rồi chia trên cùng tập dự án có phân tích.
   - Nếu muốn giữ hai tổng độc lập, bỏ phần trăm hoặc đổi thành hai card riêng.

2. Sửa dữ liệu pie chart:
   - ViewModel nên có `SoLan` và `TyLePhanTram`.
   - Chart nên dùng `SoLan` làm `data`.
   - Nếu vẫn chỉ top 5, thêm lát “Khác” bằng tổng count còn lại.
   - Tooltip/label có thể hiển thị cả số lần và phần trăm.

3. Sửa card dataset:
   - Đổi nhãn `Tổng dòng dataset` thành nhãn phản ánh latest per project, hoặc đổi query.
   - Đổi nhãn `Dòng đủ điều kiện train` thành “Dòng đủ feature và label” nếu không lọc lớp.
   - Nếu muốn số train thật, gọi `AiDatasetService.PhanLoaiDatasetNguyenNhanDeTrainAsync`.

4. Sửa chart accuracy:
   - Không map null thành 0.
   - Dùng metadata FastAPI hoặc DB nhất quán, tránh active model một nguồn, history một nguồn mà không giải thích.

5. Sửa bảng theo team nếu nghiệp vụ cần:
   - Quy định mỗi dự án thuộc một team chính, hoặc chia/đếm distinct theo `MaDuAn + MaDMNguyenNhan`, hoặc ghi rõ đây là thống kê theo quan hệ team-dự án.

6. Controller cần sửa:
   - `AiController.BuildDashboardExportRows` nếu đổi ViewModel hoặc công thức card/chart, để export không còn ghi tỷ lệ/nhãn cũ.

7. View cần sửa:
   - `Views/Ai/Dashboard.cshtml` cho nhãn card, dữ liệu chart, tooltip, label.
   - `wwwroot/js/ai/charts.js` chỉ cần sửa nếu muốn helper tooltip/format chung; hiện helper không gây lỗi nghiệp vụ.

## 10. SQL kiểm chứng read-only đề xuất

Đếm tổng nguồn:

```sql
SELECT COUNT(*) AS TongKetQuaPhanTich
FROM dbo.AI_KET_QUA;

SELECT COUNT(*) AS TongXacNhanNguyenNhan
FROM dbo.AI_NGUYEN_NHAN
WHERE ISNULL(IsDeleted, 0) = 0;

SELECT COUNT(DISTINCT MaDuAn) AS SoDuAnCoKetQuaPhanTich
FROM dbo.AI_KET_QUA;

SELECT COUNT(DISTINCT MaDuAn) AS SoDuAnCoXacNhan
FROM dbo.AI_NGUYEN_NHAN
WHERE ISNULL(IsDeleted, 0) = 0;
```

Kiểm tra tỷ lệ hiện tại:

```sql
SELECT
    kq.TongKetQuaPhanTich,
    nn.TongXacNhanNguyenNhan,
    CASE
        WHEN kq.TongKetQuaPhanTich > 0
            THEN CAST(nn.TongXacNhanNguyenNhan AS decimal(18, 4)) * 100 / kq.TongKetQuaPhanTich
        ELSE 0
    END AS TyLeHienTai
FROM (SELECT COUNT(*) AS TongKetQuaPhanTich FROM dbo.AI_KET_QUA) kq
CROSS JOIN (
    SELECT COUNT(*) AS TongXacNhanNguyenNhan
    FROM dbo.AI_NGUYEN_NHAN
    WHERE ISNULL(IsDeleted, 0) = 0
) nn;
```

Phân bố `AI_NGUYEN_NHAN`:

```sql
SELECT
    nn.MaDMNguyenNhan,
    dm.TenNguyenNhan,
    COUNT(*) AS SoLan,
    CAST(COUNT(*) AS decimal(18, 4)) * 100
        / NULLIF(SUM(COUNT(*)) OVER (), 0) AS TyLePhanTram
FROM dbo.AI_NGUYEN_NHAN nn
LEFT JOIN dbo.DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = nn.MaDMNguyenNhan
WHERE ISNULL(nn.IsDeleted, 0) = 0
GROUP BY nn.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY SoLan DESC, nn.MaDMNguyenNhan;
```

Phân bố latest `AI_DATASET.MaDMNguyenNhan` theo cách Dashboard lấy dataset:

```sql
WITH LatestDataset AS (
    SELECT
        ds.*,
        ROW_NUMBER() OVER (
            PARTITION BY ds.MaDuAn
            ORDER BY ISNULL(ds.NgayTongHop, '0001-01-01') DESC, ds.MaData DESC
        ) AS rn
    FROM dbo.AI_DATASET ds
)
SELECT
    ds.MaDMNguyenNhan,
    dm.TenNguyenNhan,
    COUNT(*) AS SoDuAnLatestDataset
FROM LatestDataset ds
LEFT JOIN dbo.DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = ds.MaDMNguyenNhan
WHERE ds.rn = 1
  AND ds.LaDuAnTre = 1
GROUP BY ds.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY SoDuAnLatestDataset DESC, ds.MaDMNguyenNhan;
```

So sánh phân tích và xác nhận theo dự án:

```sql
WITH LatestKetQua AS (
    SELECT
        kq.*,
        ROW_NUMBER() OVER (
            PARTITION BY kq.MaDuAn
            ORDER BY ISNULL(kq.ThoiGianDuDoanKetQua, '0001-01-01') DESC, kq.MaAiKetQua DESC
        ) AS rn
    FROM dbo.AI_KET_QUA kq
),
LatestXacNhan AS (
    SELECT
        nn.*,
        ROW_NUMBER() OVER (
            PARTITION BY nn.MaDuAn
            ORDER BY ISNULL(nn.NgayXacNhan, '0001-01-01') DESC, nn.MaAINguyenNhan DESC
        ) AS rn
    FROM dbo.AI_NGUYEN_NHAN nn
    WHERE ISNULL(nn.IsDeleted, 0) = 0
)
SELECT
    COALESCE(kq.MaDuAn, nn.MaDuAn) AS MaDuAn,
    kq.MaAiKetQua,
    kq.MaDMNguyenNhan AS MaNguyenNhanAiDuDoan,
    nn.MaAINguyenNhan,
    nn.MaDMNguyenNhan AS MaNguyenNhanQuanLyXacNhan,
    kq.ThoiGianDuDoanKetQua,
    nn.NgayXacNhan
FROM LatestKetQua kq
FULL OUTER JOIN LatestXacNhan nn ON nn.MaDuAn = kq.MaDuAn AND nn.rn = 1
WHERE (kq.rn = 1 OR kq.rn IS NULL)
ORDER BY COALESCE(kq.MaDuAn, nn.MaDuAn);
```

Kiểm tra số “dòng đủ điều kiện train” theo Dashboard và số dòng train thật theo lớp:

```sql
WITH LatestDataset AS (
    SELECT
        ds.*,
        ROW_NUMBER() OVER (
            PARTITION BY ds.MaDuAn
            ORDER BY ISNULL(ds.NgayTongHop, '0001-01-01') DESC, ds.MaData DESC
        ) AS rn
    FROM dbo.AI_DATASET ds
),
DuFeature AS (
    SELECT *
    FROM LatestDataset
    WHERE rn = 1
      AND LaDuAnTre = 1
      AND MaDMNguyenNhan IS NOT NULL
      AND SoNhanVienDuAn IS NOT NULL
      AND TongSoCongViec IS NOT NULL
      AND SoCongViecTre IS NOT NULL
      AND TyLeCongViecTre IS NOT NULL
      AND ChiPhiDuKien IS NOT NULL
      AND ChiPhiThucTe IS NOT NULL
      AND ChenhLechChiPhi IS NOT NULL
      AND SoLanThayDoiNhanSu IS NOT NULL
      AND SoLanThayDoiQuanLy IS NOT NULL
      AND SoNgayTreTienDo IS NOT NULL
      AND SoDeXuatCongViecChoDuyet IS NOT NULL
      AND SoDeXuatCongViecBiTuChoi IS NOT NULL
      AND ThoiGianDuyetCongViecTrungBinh IS NOT NULL
      AND SoDeXuatNganSachChoDuyet IS NOT NULL
      AND SoDeXuatNganSachBiTuChoi IS NOT NULL
      AND ThoiGianDuyetNganSachTrungBinh IS NOT NULL
      AND SoBaoCaoTienDoChoDuyet IS NOT NULL
      AND SoBaoCaoTienDoBiTuChoi IS NOT NULL
      AND SoBaoCaoTienDoYeuCauBoSung IS NOT NULL
      AND TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
      AND SoLanCapNhatTienDo IS NOT NULL
      AND SoNgayChamCapNhatTienDo IS NOT NULL
),
ClassCount AS (
    SELECT MaDMNguyenNhan, COUNT(*) AS SoDong
    FROM DuFeature
    GROUP BY MaDMNguyenNhan
)
SELECT
    (SELECT COUNT(*) FROM DuFeature) AS SoDongDuFeatureVaLabel,
    (SELECT SUM(SoDong) FROM ClassCount WHERE SoDong >= 5) AS SoDongQuaLocLop,
    (SELECT COUNT(*) FROM ClassCount WHERE SoDong >= 5) AS SoLopDuDieuKien;
```

## 11. Phạm vi chưa xác minh

- Chưa chạy ứng dụng MVC/FastAPI runtime.
- Chưa kết nối SQL Server để kiểm tra dữ liệu thật hiện tại.
- Chưa xác nhận ảnh màn hình runtime có đúng 520 xác nhận / 1 phân tích hay không; phân tích `52000%` ở trên dựa trên công thức source và ví dụ số liệu người dùng nêu.
- Chưa kiểm tra trực tiếp các file SQL rất lớn `520duanhoanthanhtrecodatasetvanguyennhan*.sql` bằng parser đếm phân bố đầy đủ; tài liệu đã cung cấp SQL read-only để xác minh phân bố thật trong DB.
