# Phân tích chức năng huấn luyện model nguyên nhân trễ

## 1. Phạm vi source đã kiểm tra

Đã kiểm tra source MVC và FastAPI liên quan trực tiếp đến màn hình huấn luyện model nguyên nhân trễ:

- MVC controller: `QuanLyDuAn/QuanLyDuAn/Controllers/AiController.cs`, `AiDatasetController.cs`.
- MVC service/interface/config DI: `AiService.cs`, `AiDatasetService.cs`, `AiApiService.cs`, `IAiService.cs`, `IAiDatasetService.cs`, `IAiApiService.cs`, `AiApiOptions.cs`, `CauHinhDichVu.cs`, `Program.cs`.
- View/ViewModel: `Views/Ai/Train.cshtml`, `Views/AiDataset/Index.cshtml`, các file `ViewModels/Ai/*Train*`, `*Dataset*`, `*Model*`, `AiOperationResultViewModel.cs`.
- Entity/DbContext: `AiDataset.cs`, `AiNguyenNhan.cs`, `DmNguyenNhan.cs`, `AiModel.cs`, `AiKetQua.cs`, `QuanLyDuAnDbContext.cs`.
- Constants/cấu hình: `Permissions.cs`, `appsettings.json`, `appsettings.Development.json`.
- FastAPI: `main.py`, `constants.py`, `config.py`, `schemas.py`, `feature_builder.py`, `validation_service.py`, `model_service.py`, `decision_tree_model.py`, `model_router.py`, `prediction_router.py`, `admin_router.py`, `health_router.py`.

Không kết nối SQL Server, không chạy ứng dụng, không thay đổi source code, không thay đổi cơ sở dữ liệu.

## 2. Kiến trúc và trách nhiệm MVC - FastAPI

MVC là system-of-record: đọc `AI_DATASET`, `AI_NGUYEN_NHAN`, `DM_NGUYEN_NHAN`, lưu `AI_MODEL`, và quyết định màn hình Train có đủ điều kiện hay không.

FastAPI là compute-only: nhận payload dataset từ MVC, validate lại schema/chất lượng, train `DecisionTreeClassifier`, lưu file model/metadata local trong thư mục model của service, trả metadata về MVC. FastAPI không kết nối SQL Server.

`Program.cs` cấu hình `AiApiOptions` và `HttpClient<IAiApiService, AiApiService>` với `AiApi:BaseUrl`. `appsettings.json` và `appsettings.Development.json` đang trỏ `BaseUrl` tới `http://127.0.0.1:8001`. `DefaultConnection` trong `appsettings.json` trỏ tới catalog `QuanLyDuAnAI3`.

## 3. Luồng mở màn hình huấn luyện

1. Razor route vào `AiController.Train(GET)`.
   - File: `Controllers/AiController.cs`.
   - Method: `Train(CancellationToken)`.
   - Điều kiện: cần quyền `Permissions.AI.Train`.
   - Output: gọi `_aiService.LayTrangTrainAsync()`, render `Views/Ai/Train.cshtml`.

2. Service tạo ViewModel.
   - File: `Services/Implementations/AiService.cs`.
   - Method: `LayTrangTrainAsync`.
   - Input: không nhận dataset từ client; đọc DB qua `AiDatasetService`.
   - Gọi `KiemTraChatLuongDatasetAsync()` để lấy tổng quan thô, và `KiemTraChatLuongDatasetNguyenNhanAsync()` để lấy tập hợp lệ train nguyên nhân.
   - Nếu `qualityReason.DuDieuKienTrain == false`, gán `vm.CanhBao = "Chưa đủ dữ liệu để huấn luyện model nguyên nhân trễ."`.

3. Razor render.
   - File: `Views/Ai/Train.cshtml`.
   - Hiển thị tổng dòng `AI_DATASET`, dòng dự án trễ, dòng có nguyên nhân xác nhận, dòng hợp lệ để train.
   - Nút `Huấn luyện model nguyên nhân trễ` bị disable bởi `disabled="@(!Model.CoTheTrainNguyenNhan)"`.
   - Biểu đồ phân bố nguyên nhân lấy từ `Model.PhanBoNguyenNhanDataset`, tức phân bố sau lọc của `qualityReason`.

## 4. Luồng kiểm tra chất lượng dataset

Có hai luồng quality ở MVC:

- `KiemTraChatLuongDatasetAsync`: đọc toàn bộ `AI_DATASET`, đếm tổng dòng, dự án trễ, dòng có label, dòng đủ feature, và phân bố theo nguyên nhân trên các dòng `LaDuAnTre=true && MaDMNguyenNhan.HasValue`. Điểm cần chú ý: phân bố ở method này chưa lọc đủ feature, nên có thể khác phân bố train thật.
- `KiemTraChatLuongDatasetNguyenNhanAsync`: gọi `LayDatasetNguyenNhanHopLeDeTrainAsync()`, tức chỉ còn các dòng `LaDuAnTre=true`, có `MaDMNguyenNhan`, đủ 22 feature. Đây là nguồn quyết định màn Train có được train hay không.

FastAPI cũng có endpoint quality:

- `/dataset/validate`: `prediction_router.validate_dataset` gọi `ValidationService.validate_dataset`.
- `/admin/dataset/quality-report`: `admin_router.dataset_quality_report` gọi `ValidationService.quality_report`.
- `/admin/model/train-recommendation`: `admin_router.train_recommendation` gọi `ValidationService.train_recommendation`.

Tuy nhiên màn `Views/Ai/Train.cshtml` hiện tại không gọi các endpoint này để quyết định disable nút; nó dùng quality từ MVC.

## 5. Luồng huấn luyện model nguyên nhân trễ

1. Người dùng bấm submit form trên `Views/Ai/Train.cshtml`.
   - Form POST tới `AiController.Train(POST)`.
   - Có `modelType=NguyenNhan`, `ActivateAfterTrain`, `TrainNote`.

2. Controller gọi service.
   - File: `Controllers/AiController.cs`.
   - Method: `Train(AiTrainPageViewModel form, string modelType = "NguyenNhan", ...)`.
   - Nếu thành công: `TempData["Success"] = "Huấn luyện model nguyên nhân trễ thành công."`.
   - Nếu thất bại: `TempData["Warning"] = BuildDetailedWarning(result)`.

3. MVC service kiểm tra trước khi gọi FastAPI.
   - File: `Services/Implementations/AiService.cs`.
   - Method: `TrainAsync`.
   - Điều kiện: user phải là Admin; `modelType` bị normalize về `NguyenNhan`.
   - Gọi `KiemTraChatLuongDatasetNguyenNhanAsync()`. Nếu không đạt, trả `"Dataset nguyên nhân chưa đủ điều kiện train model."` và danh sách `quality.LyDoKhongDat`. FastAPI chưa được gọi ở nhánh này.
   - Gọi `LayDatasetNguyenNhanHopLeDeTrainAsync()`, rồi `BuildReasonTrainDataset`.

4. MVC tạo request.
   - File: `AiService.cs`.
   - Method: `BuildReasonTrainDataset`.
   - Payload: `AiTrainRequestViewModel.Dataset` gồm 22 feature và `MaDMNguyenNhan`; không gửi `LaDuAnTre`.
   - Các số nguyên được round từ `double?`; decimal giữ qua `decimal?`.

5. MVC gọi FastAPI.
   - File: `Services/Implementations/AiApiService.cs`.
   - Method: `TrainModelAsync`.
   - Endpoint: `POST /model/train`.

6. FastAPI validate và train.
   - File: `routers/model_router.py`.
   - Method: `train_model`.
   - Gọi `ModelService.train_model`.
   - `ModelService._validate_reason_dataset` lọc lại missing feature, label `MaDMNguyenNhan > 0`, class count và rows/class.
   - `feature_builder.build_training_frames` tạo `DataFrame` theo `FEATURE_COLUMNS`, label là `MaDMNguyenNhan`.
   - `decision_tree_model.train_decision_tree` chia `train_test_split(test_size=0.2, random_state=42, stratify=y nếu mỗi lớp >= 2)`, train `DecisionTreeClassifier(random_state=42)`.

7. MVC lưu model.
   - File: `AiService.cs`.
   - Method: `LuuThongTinModelAsync`.
   - Lưu/cập nhật bảng `AI_MODEL`: `TenModel`, `SoLuongDuLieu`, `DoChinhXac`, `TrainSize`, `TestSize`, `NgayTao`, `MoTaModel`, `LoaiModel=NguyenNhan`, `IsActive`.

## 6. Điều kiện một dòng AI_DATASET được dùng để train

| Điều kiện | Vị trí kiểm tra | Giá trị hợp lệ | Dữ liệu bị loại khi | Thông báo liên quan |
| --------- | --------------- | -------------- | ------------------- | ------------------- |
| `LaDuAnTre` | `AiDatasetService.LayDatasetNguyenNhanHopLeDeTrainAsync`, `HasDuFeature`; FastAPI `_validate_reason_dataset` | MVC: `true` rồi map sang `1`; FastAPI: `None`, `1`, `True` | MVC loại mọi dòng không `true`; FastAPI loại nếu có giá trị khác `None/1/True` | Có thể góp vào thiếu dòng hợp lệ |
| `MaDMNguyenNhan` | MVC query và `HasDuFeature`; FastAPI `_validate_reason_dataset` | MVC: có giá trị; FastAPI: parse int và `> 0` | Null, không parse được, `<=0` | `Mỗi nguyên nhân...` hoặc thiếu lớp/dòng |
| Đủ 22 feature | MVC `HasDuFeature`; FastAPI `get_missing_features` | Tất cả feature khác null | Bất kỳ feature null | `Số dòng hợp lệ train... nhỏ hơn 30` |
| Giá trị `0` | MVC chỉ kiểm `HasValue`; FastAPI chỉ kiểm `is None` | Hợp lệ | Không bị loại chỉ vì bằng 0 | Không có |
| Giá trị âm/ngoài phạm vi | Không thấy rule chặn trong MVC/FastAPI train | Vẫn có thể đi qua nếu không null | Không bị loại bởi source hiện tại | Không có |
| Tồn tại trong `DM_NGUYEN_NHAN` | Khi xác nhận Manager có `AnyAsync`; khi train không join danh mục | Train chỉ cần mã trong dataset | Nếu DB đã có mã mồ côi, train không tự loại | Không có trực tiếp |
| Xóa mềm/ngừng hoạt động danh mục | `DM_NGUYEN_NHAN` entity không có `IsDeleted`/`IsActive` | Không áp dụng | Không áp dụng | Không có |
| Trùng `MaDuAn` | Train không group theo dự án | Tất cả dòng hợp lệ được lấy | Không loại trùng; query chỉ order | Không có |
| Dòng mới nhất theo dự án | Dataset aggregation cập nhật bản ghi mới nhất, nhưng train đọc tất cả dòng hợp lệ | Không bắt buộc mới nhất | Không loại bản cũ nếu còn trong `AI_DATASET` | Không có |
| Trạng thái dự án | Khi tổng hợp dataset: chỉ `HoanThanh` hoặc `LuuTru`; khi train không join `DU_AN` | Dòng đã có trong `AI_DATASET` | Dự án đang thực hiện không được tổng hợp batch chính thức | Có thể thiếu dòng |
| `AI_NGUYEN_NHAN.IsDeleted` | Khi tổng hợp nhãn: lấy `IsDeleted != true`; khi train chỉ đọc `AI_DATASET` | Xác nhận chưa xóa mềm | Xác nhận bị xóa mềm không đồng bộ vào snapshot mới | Có thể thiếu label |
| Dataset stale | Có freshness trong phân tích dự án, không thấy train tự kiểm stale | Không áp dụng ở train | Không loại | Không có |

## 7. Danh sách feature và đối chiếu contract MVC - FastAPI

Hiện tại contract train sử dụng 22 feature. Không thấy dấu hiệu MVC dùng 22 nhưng FastAPI dùng contract cũ 10 feature; `FEATURE_COLUMNS` cũng có đủ 22.

| Feature | Entity AI_DATASET | ViewModel MVC | JSON gửi đi | Schema FastAPI | FEATURE_COLUMNS | Nhận xét |
| ------- | ----------------- | ------------- | ----------- | -------------- | --------------- | -------- |
| SoNhanVienDuAn | `int?` | `double?`, train item `int?` | `SoNhanVienDuAn` | `float | None` | Có | MVC round sang int |
| TongSoCongViec | `int?` | `double?`, train item `int?` | `TongSoCongViec` | `float | None` | Có | 0 hợp lệ |
| SoCongViecTre | `int?` | `double?`, train item `int?` | `SoCongViecTre` | `float | None` | Có | 0 hợp lệ |
| TyLeCongViecTre | `double?` | `double?` | `TyLeCongViecTre` | `float | None` | Có | Không chặn ngoài 0-100 |
| ChiPhiDuKien | `decimal?` | `double?`, train item `decimal?` | `ChiPhiDuKien` | `float | None` | Có | Decimal sang JSON number |
| ChiPhiThucTe | `decimal?` | `double?`, train item `decimal?` | `ChiPhiThucTe` | `float | None` | Có | Decimal sang JSON number |
| ChenhLechChiPhi | `decimal?` | `double?`, train item `decimal?` | `ChenhLechChiPhi` | `float | None` | Có | Âm không bị chặn |
| SoLanThayDoiNhanSu | `int?` | `double?`, train item `int?` | `SoLanThayDoiNhanSu` | `float | None` | Có | MVC round sang int |
| SoLanThayDoiQuanLy | `int?` | `double?`, train item `int?` | `SoLanThayDoiQuanLy` | `float | None` | Có | MVC round sang int |
| SoNgayTreTienDo | `int?` | `double?`, train item `int?` | `SoNgayTreTienDo` | `float | None` | Có | 0 hợp lệ |
| SoDeXuatCongViecChoDuyet | `int?` | `double?`, train item `int?` | `SoDeXuatCongViecChoDuyet` | `float | None` | Có | Feature mới được tổng hợp |
| SoDeXuatCongViecBiTuChoi | `int?` | `double?`, train item `int?` | `SoDeXuatCongViecBiTuChoi` | `float | None` | Có | Feature mới được tổng hợp |
| ThoiGianDuyetCongViecTrungBinh | `double?` | `double?` | `ThoiGianDuyetCongViecTrungBinh` | `float | None` | Có | Null bị loại |
| SoDeXuatNganSachChoDuyet | `int?` | `double?`, train item `int?` | `SoDeXuatNganSachChoDuyet` | `float | None` | Có | Feature mới được tổng hợp |
| SoDeXuatNganSachBiTuChoi | `int?` | `double?`, train item `int?` | `SoDeXuatNganSachBiTuChoi` | `float | None` | Có | Feature mới được tổng hợp |
| ThoiGianDuyetNganSachTrungBinh | `double?` | `double?` | `ThoiGianDuyetNganSachTrungBinh` | `float | None` | Có | Null bị loại |
| SoBaoCaoTienDoChoDuyet | `int?` | `double?`, train item `int?` | `SoBaoCaoTienDoChoDuyet` | `float | None` | Có | Feature mới được tổng hợp |
| SoBaoCaoTienDoBiTuChoi | `int?` | `double?`, train item `int?` | `SoBaoCaoTienDoBiTuChoi` | `float | None` | Có | Feature mới được tổng hợp |
| SoBaoCaoTienDoYeuCauBoSung | `int?` | `double?`, train item `int?` | `SoBaoCaoTienDoYeuCauBoSung` | `float | None` | Có | Feature mới được tổng hợp |
| TyLeBaoCaoTienDoBiTuChoi | `double?` | `double?` | `TyLeBaoCaoTienDoBiTuChoi` | `float | None` | Có | Không chặn ngoài 0-100 |
| SoLanCapNhatTienDo | `int?` | `double?`, train item `int?` | `SoLanCapNhatTienDo` | `float | None` | Có | Feature mới được tổng hợp |
| SoNgayChamCapNhatTienDo | `int?` | `double?`, train item `int?` | `SoNgayChamCapNhatTienDo` | `float | None` | Có | Feature mới được tổng hợp |

`LaDuAnTre` không phải feature train và không phải label train nguyên nhân. Label train là `MaDMNguyenNhan`.

## 8. Cách hệ thống đồng bộ nhãn nguyên nhân

Có hai đường đồng bộ nhãn vào `AI_DATASET.MaDMNguyenNhan`:

1. Khi Manager xác nhận nguyên nhân:
   - File: `AiService.cs`.
   - Method: `XacNhanNguyenNhanAsync`.
   - Kiểm tra `DM_NGUYEN_NHAN` tồn tại, dự án trễ, có kết quả phân tích chính thức hợp lệ trong `AI_KET_QUA`.
   - Tạo hoặc cập nhật bản ghi `AI_NGUYEN_NHAN` mới nhất chưa xóa mềm.
   - Gán trực tiếp `datasetMoiNhat.MaDMNguyenNhan = maDmNguyenNhanInt`.

2. Khi tổng hợp dataset:
   - File: `AiDatasetService.cs`.
   - Method: `BuildFeatureSnapshotsAsync`.
   - Lấy `AI_NGUYEN_NHAN` chưa xóa mềm mới nhất theo `NgayXacNhan`, `MaAINguyenNhan`.
   - Chỉ gán `MaDMNguyenNhan` nếu snapshot xác định `laDuAnTre == true`.
   - `TongHopNoiBoAsync` cập nhật bản ghi `AI_DATASET` mới nhất theo từng `MaDuAn`.

Vì train đọc trực tiếp `AI_DATASET.MaDMNguyenNhan`, dữ liệu có trong `AI_NGUYEN_NHAN` nhưng chưa đồng bộ sang `AI_DATASET` sẽ không được dùng để train.

## 9. Cách tính phân bố lớp nguyên nhân

| Giai đoạn | Tổng số dòng | Số dòng hợp lệ | Số lớp | Số dòng nhỏ nhất mỗi lớp | Có đạt điều kiện không |
| --------- | -----------: | -------------: | -----: | -----------------------: | ---------------------- |
| MVC tổng quan `KiemTraChatLuongDatasetAsync` | Toàn bộ `AI_DATASET` | Đếm dòng `LaDuAnTre=true`, có label và đủ feature | Phân bố đang đếm trên dòng có label, chưa lọc đủ feature | Min trên phân bố chưa lọc feature | Có thể khác train thật |
| MVC train thật `KiemTraChatLuongDatasetNguyenNhanAsync` | Dòng đã qua `LayDatasetNguyenNhanHopLeDeTrainAsync` | Bằng tổng số dòng sau `HasDuFeature` | Group theo `MaDMNguyenNhan` | `minRows` sau lọc feature | Đây là điều kiện disable nút |
| FastAPI quality report | Payload được gửi tới FastAPI | Dòng không thiếu feature, `LaDuAnTre=1/True`, `MaDMNguyenNhan>0` | Group theo `MaDMNguyenNhan` | Min sau lọc | Chỉ dùng nếu endpoint được gọi |
| FastAPI train | Payload train từ MVC | Dòng không thiếu feature, label `>0`; `LaDuAnTre` vắng mặt được chấp nhận | Group theo `MaDMNguyenNhan` | Min sau lọc | Chặn lại nếu MVC lọt sai |

Không thể lấy số liệu thực tế vì không kết nối cơ sở dữ liệu. Xem mục 13 để chạy các truy vấn SELECT kiểm chứng.

## 10. Nguồn phát sinh hai thông báo lỗi

Thông báo `Mỗi nguyên nhân cần tối thiểu 5 dòng.`

- MVC tổng quan: `AiDatasetService.KiemTraChatLuongDatasetAsync` thêm chuỗi khi `phanBoTheoNguyenNhan.Count > 0 && phanBoTheoNguyenNhan.Values.Min() < MinReasonRowsPerClass`.
- MVC train thật: `AiDatasetService.KiemTraChatLuongDatasetNguyenNhanAsync` thêm chuỗi dài hơn: `Mỗi nguyên nhân cần tối thiểu 5 dòng, hiện nhỏ nhất là {minRows}.`
- FastAPI có thông điệp tương đương nhưng không giống nguyên văn: `Dataset mất cân bằng: mỗi nguyên nhân cần ít nhất 5 dòng.` hoặc `mỗi nguyên nhân cần >= 5 dòng`.
- Nếu giao diện hiển thị đúng chuỗi ngắn không có `hiện nhỏ nhất`, nhiều khả năng đến từ màn Dataset/tổng quan MVC; nếu hiển thị trong màn Train recommendation thì là chuỗi dài từ quality train thật.

Thông báo `Chưa đủ dữ liệu để huấn luyện model nguyên nhân trễ.`

- File: `AiService.cs`.
- Method: `LayTrangTrainAsync`.
- Điều kiện: `if (!vm.CoTheTrainNguyenNhan)`.
- Nguồn: MVC gán vào `vm.CanhBao`.
- Razor `Views/Ai/Train.cshtml` chỉ render `Model.CanhBao`; Razor không tự tạo chuỗi này.

Hai thông báo không cùng một dòng `if`, nhưng cùng phụ thuộc vào `qualityReason.DuDieuKienTrain=false` hoặc quality dataset không đạt. `Mỗi nguyên nhân...` là lý do chi tiết; `Chưa đủ dữ liệu...` là cảnh báo tổng trên màn Train.

Trường hợp FastAPI báo đủ nhưng MVC vẫn chặn: có thể xảy ra nếu gọi FastAPI bằng payload khác với tập MVC đang đọc, vì màn Train không hỏi FastAPI trước khi disable.

Trường hợp MVC báo đủ nhưng FastAPI vẫn từ chối: có thể xảy ra nếu FastAPI đang chạy source/config khác, env ngưỡng khác, hoặc payload bị 422 do schema `extra="forbid"`/contract khác. Với source hiện tại, payload train của MVC khớp schema FastAPI.

## 11. Ngưỡng và điều kiện chặn huấn luyện

MVC:

- File: `AiDatasetService.cs`.
- Constants: `MinReasonTrainRows = 30`, `MinReasonClasses = 2`, `MinReasonRowsPerClass = 5`.
- Dùng ở `KiemTraChatLuongDatasetAsync` và `KiemTraChatLuongDatasetNguyenNhanAsync`.

FastAPI:

- File: `config.py`.
- Env/default: `MIN_REASON_TRAIN_ROWS=30`, `MIN_REASON_CLASS_COUNT=2`, `MIN_REASON_ROWS_PER_CLASS=5`.
- Dùng ở `ValidationService` và `ModelService._validate_reason_dataset`.

Không thấy hard-code thêm trong Razor hoặc JavaScript ngoài việc nút disable theo `Model.CoTheTrainNguyenNhan`.

Khi một lớp chỉ có 1-4 dòng, toàn bộ train bị chặn; source không loại lớp nhỏ rồi train phần còn lại. Sau khi lọc feature, hệ thống đếm lại tổng dòng, số lớp, min mỗi lớp. `train_test_split` dùng `stratify=y` nếu có ít nhất 2 lớp và mỗi lớp có ít nhất 2 dòng; với min 5 dòng/lớp, source hiện tại đủ điều kiện stratify cơ bản. Không thấy xử lý riêng trường hợp số mẫu test nhỏ hơn số lớp; với ngưỡng 30 và 2 lớp thì thường không lỗi, nhưng nếu env FastAPI bị hạ ngưỡng thấp có thể phát sinh.

## 12. Phân tích dữ liệu có nhưng vẫn không đủ điều kiện train

Các trường hợp source cho phép xảy ra:

- Có nhiều dòng `AI_DATASET`, nhưng ít dòng `LaDuAnTre=true`.
- Có nhiều dự án trễ, nhưng `AI_DATASET.MaDMNguyenNhan` null.
- Có `AI_NGUYEN_NHAN`, nhưng chưa đồng bộ sang dòng `AI_DATASET` mới nhất hoặc dòng train.
- Có label nhưng thiếu một trong 22 feature mới, nên bị `HasDuFeature` loại.
- Có nhiều dòng tổng, nhưng phân bố sau lọc feature có một nguyên nhân dưới 5 dòng.
- Chỉ một nguyên nhân đạt dữ liệu, không đủ tối thiểu 2 lớp.
- Dữ liệu cũ được tạo trước khi thêm các feature đề xuất/tiến độ mới nên các cột mới null.
- Batch tổng hợp chỉ lấy dự án `HoanThanh` hoặc `LuuTru`; dự án đang thực hiện không được đưa vào batch train chính thức.
- MVC đang đọc catalog `QuanLyDuAnAI3` trong `DefaultConnection`, trong khi người dùng kiểm tra dữ liệu ở database khác.
- MVC gọi `http://127.0.0.1:8001`; nếu FastAPI chạy source cũ hoặc port khác, POST train có thể fail sau khi MVC đã cho phép.

## 13. Các truy vấn SQL SELECT để kiểm tra dữ liệu

Tất cả truy vấn dưới đây chỉ đọc dữ liệu.

```sql
-- 1. Tổng số dòng trong AI_DATASET
SELECT COUNT(*) AS [Tong so dong AI_DATASET]
FROM AI_DATASET;
```

```sql
-- 2. Tổng số dòng có LaDuAnTre = 1
SELECT COUNT(*) AS [Tong dong du an tre]
FROM AI_DATASET
WHERE LaDuAnTre = 1;
```

```sql
-- 3. Tổng số dòng có MaDMNguyenNhan hợp lệ theo danh mục
SELECT COUNT(*) AS [Dong co ma nguyen nhan hop le]
FROM AI_DATASET ds
JOIN DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = ds.MaDMNguyenNhan
WHERE ds.LaDuAnTre = 1
  AND ds.MaDMNguyenNhan IS NOT NULL;
```

```sql
-- 4. Phân bố số dòng theo nguyên nhân trước khi lọc feature
SELECT
    ds.MaDMNguyenNhan AS [Ma nguyen nhan],
    COALESCE(dm.TenNguyenNhan, N'(Khong co trong danh muc)') AS [Ten nguyen nhan],
    COUNT(*) AS [So dong]
FROM AI_DATASET ds
LEFT JOIN DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = ds.MaDMNguyenNhan
WHERE ds.LaDuAnTre = 1
  AND ds.MaDMNguyenNhan IS NOT NULL
GROUP BY ds.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY [So dong] DESC;
```

```sql
-- 5. Phân bố số dòng theo nguyên nhân sau khi áp dụng đúng điều kiện MVC train
SELECT
    ds.MaDMNguyenNhan AS [Ma nguyen nhan],
    COALESCE(dm.TenNguyenNhan, N'(Khong co trong danh muc)') AS [Ten nguyen nhan],
    COUNT(*) AS [So dong hop le train]
FROM AI_DATASET ds
LEFT JOIN DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = ds.MaDMNguyenNhan
WHERE ds.LaDuAnTre = 1
  AND ds.MaDMNguyenNhan IS NOT NULL
  AND ds.SoNhanVienDuAn IS NOT NULL
  AND ds.TongSoCongViec IS NOT NULL
  AND ds.SoCongViecTre IS NOT NULL
  AND ds.TyLeCongViecTre IS NOT NULL
  AND ds.ChiPhiDuKien IS NOT NULL
  AND ds.ChiPhiThucTe IS NOT NULL
  AND ds.ChenhLechChiPhi IS NOT NULL
  AND ds.SoLanThayDoiNhanSu IS NOT NULL
  AND ds.SoLanThayDoiQuanLy IS NOT NULL
  AND ds.SoNgayTreTienDo IS NOT NULL
  AND ds.SoDeXuatCongViecChoDuyet IS NOT NULL
  AND ds.SoDeXuatCongViecBiTuChoi IS NOT NULL
  AND ds.ThoiGianDuyetCongViecTrungBinh IS NOT NULL
  AND ds.SoDeXuatNganSachChoDuyet IS NOT NULL
  AND ds.SoDeXuatNganSachBiTuChoi IS NOT NULL
  AND ds.ThoiGianDuyetNganSachTrungBinh IS NOT NULL
  AND ds.SoBaoCaoTienDoChoDuyet IS NOT NULL
  AND ds.SoBaoCaoTienDoBiTuChoi IS NOT NULL
  AND ds.SoBaoCaoTienDoYeuCauBoSung IS NOT NULL
  AND ds.TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
  AND ds.SoLanCapNhatTienDo IS NOT NULL
  AND ds.SoNgayChamCapNhatTienDo IS NOT NULL
GROUP BY ds.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY [So dong hop le train] DESC;
```

```sql
-- 6. Các dòng có nhãn nhưng thiếu một hoặc nhiều feature
SELECT
    ds.MaData,
    ds.MaDuAn,
    ds.MaDMNguyenNhan,
    CONCAT(
        CASE WHEN ds.SoNhanVienDuAn IS NULL THEN N'SoNhanVienDuAn; ' ELSE N'' END,
        CASE WHEN ds.TongSoCongViec IS NULL THEN N'TongSoCongViec; ' ELSE N'' END,
        CASE WHEN ds.SoCongViecTre IS NULL THEN N'SoCongViecTre; ' ELSE N'' END,
        CASE WHEN ds.TyLeCongViecTre IS NULL THEN N'TyLeCongViecTre; ' ELSE N'' END,
        CASE WHEN ds.ChiPhiDuKien IS NULL THEN N'ChiPhiDuKien; ' ELSE N'' END,
        CASE WHEN ds.ChiPhiThucTe IS NULL THEN N'ChiPhiThucTe; ' ELSE N'' END,
        CASE WHEN ds.ChenhLechChiPhi IS NULL THEN N'ChenhLechChiPhi; ' ELSE N'' END,
        CASE WHEN ds.SoLanThayDoiNhanSu IS NULL THEN N'SoLanThayDoiNhanSu; ' ELSE N'' END,
        CASE WHEN ds.SoLanThayDoiQuanLy IS NULL THEN N'SoLanThayDoiQuanLy; ' ELSE N'' END,
        CASE WHEN ds.SoNgayTreTienDo IS NULL THEN N'SoNgayTreTienDo; ' ELSE N'' END,
        CASE WHEN ds.SoDeXuatCongViecChoDuyet IS NULL THEN N'SoDeXuatCongViecChoDuyet; ' ELSE N'' END,
        CASE WHEN ds.SoDeXuatCongViecBiTuChoi IS NULL THEN N'SoDeXuatCongViecBiTuChoi; ' ELSE N'' END,
        CASE WHEN ds.ThoiGianDuyetCongViecTrungBinh IS NULL THEN N'ThoiGianDuyetCongViecTrungBinh; ' ELSE N'' END,
        CASE WHEN ds.SoDeXuatNganSachChoDuyet IS NULL THEN N'SoDeXuatNganSachChoDuyet; ' ELSE N'' END,
        CASE WHEN ds.SoDeXuatNganSachBiTuChoi IS NULL THEN N'SoDeXuatNganSachBiTuChoi; ' ELSE N'' END,
        CASE WHEN ds.ThoiGianDuyetNganSachTrungBinh IS NULL THEN N'ThoiGianDuyetNganSachTrungBinh; ' ELSE N'' END,
        CASE WHEN ds.SoBaoCaoTienDoChoDuyet IS NULL THEN N'SoBaoCaoTienDoChoDuyet; ' ELSE N'' END,
        CASE WHEN ds.SoBaoCaoTienDoBiTuChoi IS NULL THEN N'SoBaoCaoTienDoBiTuChoi; ' ELSE N'' END,
        CASE WHEN ds.SoBaoCaoTienDoYeuCauBoSung IS NULL THEN N'SoBaoCaoTienDoYeuCauBoSung; ' ELSE N'' END,
        CASE WHEN ds.TyLeBaoCaoTienDoBiTuChoi IS NULL THEN N'TyLeBaoCaoTienDoBiTuChoi; ' ELSE N'' END,
        CASE WHEN ds.SoLanCapNhatTienDo IS NULL THEN N'SoLanCapNhatTienDo; ' ELSE N'' END,
        CASE WHEN ds.SoNgayChamCapNhatTienDo IS NULL THEN N'SoNgayChamCapNhatTienDo; ' ELSE N'' END
    ) AS [Feature thieu]
FROM AI_DATASET ds
WHERE ds.LaDuAnTre = 1
  AND ds.MaDMNguyenNhan IS NOT NULL
  AND (
      ds.SoNhanVienDuAn IS NULL OR ds.TongSoCongViec IS NULL OR ds.SoCongViecTre IS NULL
      OR ds.TyLeCongViecTre IS NULL OR ds.ChiPhiDuKien IS NULL OR ds.ChiPhiThucTe IS NULL
      OR ds.ChenhLechChiPhi IS NULL OR ds.SoLanThayDoiNhanSu IS NULL OR ds.SoLanThayDoiQuanLy IS NULL
      OR ds.SoNgayTreTienDo IS NULL OR ds.SoDeXuatCongViecChoDuyet IS NULL OR ds.SoDeXuatCongViecBiTuChoi IS NULL
      OR ds.ThoiGianDuyetCongViecTrungBinh IS NULL OR ds.SoDeXuatNganSachChoDuyet IS NULL
      OR ds.SoDeXuatNganSachBiTuChoi IS NULL OR ds.ThoiGianDuyetNganSachTrungBinh IS NULL
      OR ds.SoBaoCaoTienDoChoDuyet IS NULL OR ds.SoBaoCaoTienDoBiTuChoi IS NULL
      OR ds.SoBaoCaoTienDoYeuCauBoSung IS NULL OR ds.TyLeBaoCaoTienDoBiTuChoi IS NULL
      OR ds.SoLanCapNhatTienDo IS NULL OR ds.SoNgayChamCapNhatTienDo IS NULL
  );
```

```sql
-- 7. Dự án có AI_NGUYEN_NHAN nhưng AI_DATASET.MaDMNguyenNhan chưa đồng bộ
WITH latest_nn AS (
    SELECT *,
           ROW_NUMBER() OVER (PARTITION BY MaDuAn ORDER BY ISNULL(NgayXacNhan, '19000101') DESC, MaAINguyenNhan DESC) AS rn
    FROM AI_NGUYEN_NHAN
    WHERE IsDeleted <> 1 OR IsDeleted IS NULL
),
latest_ds AS (
    SELECT *,
           ROW_NUMBER() OVER (PARTITION BY MaDuAn ORDER BY ISNULL(NgayTongHop, '19000101') DESC, MaData DESC) AS rn
    FROM AI_DATASET
)
SELECT
    nn.MaDuAn,
    nn.MaDMNguyenNhan AS [Ma nguyen nhan da xac nhan],
    ds.MaDMNguyenNhan AS [Ma nguyen nhan tren dataset],
    ds.LaDuAnTre,
    ds.NgayTongHop,
    nn.NgayXacNhan
FROM latest_nn nn
LEFT JOIN latest_ds ds ON ds.MaDuAn = nn.MaDuAn AND ds.rn = 1
WHERE nn.rn = 1
  AND (ds.MaData IS NULL OR ds.MaDMNguyenNhan IS NULL OR ds.MaDMNguyenNhan <> nn.MaDMNguyenNhan);
```

```sql
-- 8. Mã nguyên nhân không tồn tại trong DM_NGUYEN_NHAN
SELECT DISTINCT ds.MaDMNguyenNhan AS [Ma nguyen nhan khong ton tai]
FROM AI_DATASET ds
LEFT JOIN DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = ds.MaDMNguyenNhan
WHERE ds.MaDMNguyenNhan IS NOT NULL
  AND dm.MaDMNguyenNhan IS NULL;
```

```sql
-- 9. Dự án có nhiều dòng AI_DATASET
SELECT MaDuAn, COUNT(*) AS [So dong AI_DATASET]
FROM AI_DATASET
GROUP BY MaDuAn
HAVING COUNT(*) > 1
ORDER BY [So dong AI_DATASET] DESC;
```

```sql
-- 10. Dự án có nhiều lần xác nhận nguyên nhân
SELECT MaDuAn, COUNT(*) AS [So lan xac nhan]
FROM AI_NGUYEN_NHAN
WHERE IsDeleted <> 1 OR IsDeleted IS NULL
GROUP BY MaDuAn
HAVING COUNT(*) > 1
ORDER BY [So lan xac nhan] DESC;
```

```sql
-- 11. Nguyên nhân có ít hơn 5 dòng hợp lệ
WITH valid_rows AS (
    SELECT ds.*
    FROM AI_DATASET ds
    WHERE ds.LaDuAnTre = 1
      AND ds.MaDMNguyenNhan IS NOT NULL
      AND ds.SoNhanVienDuAn IS NOT NULL AND ds.TongSoCongViec IS NOT NULL
      AND ds.SoCongViecTre IS NOT NULL AND ds.TyLeCongViecTre IS NOT NULL
      AND ds.ChiPhiDuKien IS NOT NULL AND ds.ChiPhiThucTe IS NOT NULL
      AND ds.ChenhLechChiPhi IS NOT NULL AND ds.SoLanThayDoiNhanSu IS NOT NULL
      AND ds.SoLanThayDoiQuanLy IS NOT NULL AND ds.SoNgayTreTienDo IS NOT NULL
      AND ds.SoDeXuatCongViecChoDuyet IS NOT NULL AND ds.SoDeXuatCongViecBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetCongViecTrungBinh IS NOT NULL AND ds.SoDeXuatNganSachChoDuyet IS NOT NULL
      AND ds.SoDeXuatNganSachBiTuChoi IS NOT NULL AND ds.ThoiGianDuyetNganSachTrungBinh IS NOT NULL
      AND ds.SoBaoCaoTienDoChoDuyet IS NOT NULL AND ds.SoBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoBaoCaoTienDoYeuCauBoSung IS NOT NULL AND ds.TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoLanCapNhatTienDo IS NOT NULL AND ds.SoNgayChamCapNhatTienDo IS NOT NULL
)
SELECT v.MaDMNguyenNhan, COALESCE(dm.TenNguyenNhan, N'(Khong co trong danh muc)') AS [Ten nguyen nhan], COUNT(*) AS [So dong hop le]
FROM valid_rows v
LEFT JOIN DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = v.MaDMNguyenNhan
GROUP BY v.MaDMNguyenNhan, dm.TenNguyenNhan
HAVING COUNT(*) < 5;
```

```sql
-- 12. Tổng số dòng hợp lệ cuối cùng có thể gửi sang FastAPI
SELECT COUNT(*) AS [Tong dong hop le cuoi cung]
FROM AI_DATASET ds
WHERE ds.LaDuAnTre = 1
  AND ds.MaDMNguyenNhan IS NOT NULL
  AND ds.SoNhanVienDuAn IS NOT NULL AND ds.TongSoCongViec IS NOT NULL
  AND ds.SoCongViecTre IS NOT NULL AND ds.TyLeCongViecTre IS NOT NULL
  AND ds.ChiPhiDuKien IS NOT NULL AND ds.ChiPhiThucTe IS NOT NULL
  AND ds.ChenhLechChiPhi IS NOT NULL AND ds.SoLanThayDoiNhanSu IS NOT NULL
  AND ds.SoLanThayDoiQuanLy IS NOT NULL AND ds.SoNgayTreTienDo IS NOT NULL
  AND ds.SoDeXuatCongViecChoDuyet IS NOT NULL AND ds.SoDeXuatCongViecBiTuChoi IS NOT NULL
  AND ds.ThoiGianDuyetCongViecTrungBinh IS NOT NULL AND ds.SoDeXuatNganSachChoDuyet IS NOT NULL
  AND ds.SoDeXuatNganSachBiTuChoi IS NOT NULL AND ds.ThoiGianDuyetNganSachTrungBinh IS NOT NULL
  AND ds.SoBaoCaoTienDoChoDuyet IS NOT NULL AND ds.SoBaoCaoTienDoBiTuChoi IS NOT NULL
  AND ds.SoBaoCaoTienDoYeuCauBoSung IS NOT NULL AND ds.TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
  AND ds.SoLanCapNhatTienDo IS NOT NULL AND ds.SoNgayChamCapNhatTienDo IS NOT NULL;
```

```sql
-- 13. So sánh phân bố trước và sau khi lọc feature
WITH before_filter AS (
    SELECT MaDMNguyenNhan, COUNT(*) AS SoDongTruocLoc
    FROM AI_DATASET
    WHERE LaDuAnTre = 1 AND MaDMNguyenNhan IS NOT NULL
    GROUP BY MaDMNguyenNhan
),
after_filter AS (
    SELECT MaDMNguyenNhan, COUNT(*) AS SoDongSauLoc
    FROM AI_DATASET ds
    WHERE ds.LaDuAnTre = 1
      AND ds.MaDMNguyenNhan IS NOT NULL
      AND ds.SoNhanVienDuAn IS NOT NULL AND ds.TongSoCongViec IS NOT NULL
      AND ds.SoCongViecTre IS NOT NULL AND ds.TyLeCongViecTre IS NOT NULL
      AND ds.ChiPhiDuKien IS NOT NULL AND ds.ChiPhiThucTe IS NOT NULL
      AND ds.ChenhLechChiPhi IS NOT NULL AND ds.SoLanThayDoiNhanSu IS NOT NULL
      AND ds.SoLanThayDoiQuanLy IS NOT NULL AND ds.SoNgayTreTienDo IS NOT NULL
      AND ds.SoDeXuatCongViecChoDuyet IS NOT NULL AND ds.SoDeXuatCongViecBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetCongViecTrungBinh IS NOT NULL AND ds.SoDeXuatNganSachChoDuyet IS NOT NULL
      AND ds.SoDeXuatNganSachBiTuChoi IS NOT NULL AND ds.ThoiGianDuyetNganSachTrungBinh IS NOT NULL
      AND ds.SoBaoCaoTienDoChoDuyet IS NOT NULL AND ds.SoBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoBaoCaoTienDoYeuCauBoSung IS NOT NULL AND ds.TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoLanCapNhatTienDo IS NOT NULL AND ds.SoNgayChamCapNhatTienDo IS NOT NULL
    GROUP BY MaDMNguyenNhan
)
SELECT
    COALESCE(b.MaDMNguyenNhan, a.MaDMNguyenNhan) AS [Ma nguyen nhan],
    dm.TenNguyenNhan AS [Ten nguyen nhan],
    ISNULL(b.SoDongTruocLoc, 0) AS [So dong truoc loc],
    ISNULL(a.SoDongSauLoc, 0) AS [So dong sau loc],
    ISNULL(b.SoDongTruocLoc, 0) - ISNULL(a.SoDongSauLoc, 0) AS [So dong bi loai]
FROM before_filter b
FULL JOIN after_filter a ON a.MaDMNguyenNhan = b.MaDMNguyenNhan
LEFT JOIN DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = COALESCE(b.MaDMNguyenNhan, a.MaDMNguyenNhan)
ORDER BY [So dong bi loai] DESC;
```

```sql
-- 14. Kiểm tra feature mới bị NULL
SELECT
    SUM(CASE WHEN SoDeXuatCongViecChoDuyet IS NULL THEN 1 ELSE 0 END) AS [Null SoDeXuatCongViecChoDuyet],
    SUM(CASE WHEN SoDeXuatCongViecBiTuChoi IS NULL THEN 1 ELSE 0 END) AS [Null SoDeXuatCongViecBiTuChoi],
    SUM(CASE WHEN ThoiGianDuyetCongViecTrungBinh IS NULL THEN 1 ELSE 0 END) AS [Null ThoiGianDuyetCongViecTrungBinh],
    SUM(CASE WHEN SoDeXuatNganSachChoDuyet IS NULL THEN 1 ELSE 0 END) AS [Null SoDeXuatNganSachChoDuyet],
    SUM(CASE WHEN SoDeXuatNganSachBiTuChoi IS NULL THEN 1 ELSE 0 END) AS [Null SoDeXuatNganSachBiTuChoi],
    SUM(CASE WHEN ThoiGianDuyetNganSachTrungBinh IS NULL THEN 1 ELSE 0 END) AS [Null ThoiGianDuyetNganSachTrungBinh],
    SUM(CASE WHEN SoBaoCaoTienDoChoDuyet IS NULL THEN 1 ELSE 0 END) AS [Null SoBaoCaoTienDoChoDuyet],
    SUM(CASE WHEN SoBaoCaoTienDoBiTuChoi IS NULL THEN 1 ELSE 0 END) AS [Null SoBaoCaoTienDoBiTuChoi],
    SUM(CASE WHEN SoBaoCaoTienDoYeuCauBoSung IS NULL THEN 1 ELSE 0 END) AS [Null SoBaoCaoTienDoYeuCauBoSung],
    SUM(CASE WHEN TyLeBaoCaoTienDoBiTuChoi IS NULL THEN 1 ELSE 0 END) AS [Null TyLeBaoCaoTienDoBiTuChoi],
    SUM(CASE WHEN SoLanCapNhatTienDo IS NULL THEN 1 ELSE 0 END) AS [Null SoLanCapNhatTienDo],
    SUM(CASE WHEN SoNgayChamCapNhatTienDo IS NULL THEN 1 ELSE 0 END) AS [Null SoNgayChamCapNhatTienDo]
FROM AI_DATASET;
```

```sql
-- 15. Kiểm tra dữ liệu feature có giá trị bất hợp lý theo miền cơ bản
SELECT MaData, MaDuAn, MaDMNguyenNhan,
       SoNhanVienDuAn, TongSoCongViec, SoCongViecTre, TyLeCongViecTre,
       ChiPhiDuKien, ChiPhiThucTe, ChenhLechChiPhi,
       TyLeBaoCaoTienDoBiTuChoi, SoNgayTreTienDo, SoNgayChamCapNhatTienDo
FROM AI_DATASET
WHERE SoNhanVienDuAn < 0
   OR TongSoCongViec < 0
   OR SoCongViecTre < 0
   OR SoCongViecTre > TongSoCongViec
   OR TyLeCongViecTre < 0 OR TyLeCongViecTre > 100
   OR TyLeBaoCaoTienDoBiTuChoi < 0 OR TyLeBaoCaoTienDoBiTuChoi > 100
   OR SoNgayTreTienDo < 0
   OR SoNgayChamCapNhatTienDo < 0;
```

## 14. Các điểm không đồng bộ hoặc lỗi tiềm ẩn

- `KiemTraChatLuongDatasetAsync` đếm phân bố nguyên nhân trước khi lọc đủ feature, còn `KiemTraChatLuongDatasetNguyenNhanAsync` đếm sau lọc. Nếu hai màn hình dùng hai nguồn này, số liệu có thể nhìn khác nhau.
- Train đọc toàn bộ dòng hợp lệ trong `AI_DATASET`, không lấy latest theo `MaDuAn`; nếu tồn tại nhiều dòng cũ cùng dự án, chúng vẫn có thể được train.
- `DM_NGUYEN_NHAN` không có `IsDeleted/IsActive`, nên không có rule loại danh mục ngừng hoạt động ở train.
- FastAPI schema `extra="forbid"`; nếu MVC hoặc client gửi thêm field ngoài schema, sẽ 422. Payload train hiện tại không gửi thừa.
- `AiApiService.BuildErrorPayload` thông báo 422 vẫn liệt kê chuỗi feature cũ 10 feature trong biến `featureList`; đây là thông báo hỗ trợ lỗi, không phải contract train thật.
- Nếu `.env` FastAPI override `MIN_REASON_*`, MVC và FastAPI có thể lệch ngưỡng.
- Nếu FastAPI chưa restart sau khi đổi constants/config, MVC có thể tưởng source đã đồng bộ nhưng service runtime vẫn dùng bản cũ.

## 15. Nguyên nhân có khả năng cao nhất

Rất có khả năng: có dữ liệu/xác nhận nguyên nhân nhưng phân bố sau lọc feature không đạt tối thiểu 5 dòng mỗi nguyên nhân.

Bằng chứng:
- `AiDatasetService.KiemTraChatLuongDatasetNguyenNhanAsync` lấy dữ liệu từ `LayDatasetNguyenNhanHopLeDeTrainAsync`, tức sau khi lọc đủ 22 feature.
- Method này chặn khi `minRows < MinReasonRowsPerClass` và sinh thông báo `Mỗi nguyên nhân cần tối thiểu 5 dòng, hiện nhỏ nhất là {minRows}.`
- Màn Train disable theo `Model.CoTheTrainNguyenNhan`.

Cách kiểm chứng:
- Chạy truy vấn số 5, 11, 13 ở mục 13.

Rất có khả năng: `AI_NGUYEN_NHAN` đã có xác nhận nhưng `AI_DATASET.MaDMNguyenNhan` chưa đồng bộ hoặc đang null ở dòng train.

Bằng chứng:
- Train chỉ đọc `AI_DATASET.MaDMNguyenNhan`; không join `AI_NGUYEN_NHAN`.
- Tổng hợp dataset lấy `AI_NGUYEN_NHAN` mới nhất chưa xóa mềm để gán vào snapshot.
- Nếu người dùng thêm xác nhận sau lần tổng hợp hoặc dòng dataset mới nhất chưa cập nhật, quality train không tính nhãn đó.

Cách kiểm chứng:
- Chạy truy vấn số 7 và số 3 ở mục 13.

Có khả năng: dữ liệu cũ thiếu các feature mới trong contract 22 feature.

Bằng chứng:
- `HasDuFeature` yêu cầu đủ các feature đề xuất công việc, ngân sách, báo cáo tiến độ.
- Dữ liệu cũ tạo theo contract ít feature hơn sẽ bị loại dù có `LaDuAnTre=1` và `MaDMNguyenNhan`.

Cách kiểm chứng:
- Chạy truy vấn số 6 và 14.

Có khả năng: người dùng đang nhìn dữ liệu ở database khác với MVC.

Bằng chứng:
- `appsettings.json` đang dùng `DefaultConnection` tới `QuanLyDuAnAI3`.
- Nếu SQL đang kiểm tra là catalog khác, màn Train vẫn dựa trên `QuanLyDuAnAI3`.

Cách kiểm chứng:
- Kiểm tra connection string runtime và chạy SELECT trên đúng database.

Ít có khả năng: FastAPI từ chối vì contract feature cũ.

Bằng chứng:
- `constants.py.FEATURE_COLUMNS` có đủ 22 feature.
- `AiReasonTrainDatasetItemViewModel` có đủ 22 field và JSON name trùng schema.
- Màn Train bị disable trước khi gọi FastAPI nếu MVC chưa đủ dữ liệu.

Không phải nguyên nhân chính của hai thông báo đang hỏi: `AI_KET_QUA` chưa đủ dữ liệu.

Bằng chứng:
- Train nguyên nhân không dùng `AI_KET_QUA` làm label.
- `AI_KET_QUA` chỉ liên quan kết quả phân tích/chứng cứ xác nhận chính thức, không phải nguồn phân bố train.

## 16. Đề xuất hướng chỉnh sửa ở bước tiếp theo

- Không hạ ngưỡng 30/2/5, không tạo nhãn giả, không nhân bản dữ liệu.
- Nếu truy vấn cho thấy `AI_NGUYEN_NHAN` có nhãn nhưng `AI_DATASET.MaDMNguyenNhan` null, ưu tiên sửa hoặc chạy lại luồng đồng bộ/tổng hợp hiện có để cập nhật `AI_DATASET`.
- Nếu thiếu feature mới, ưu tiên tổng hợp lại dataset bằng logic hiện có để điền 22 feature.
- Nếu màn hình gây nhầm lẫn, chỉ chỉnh UI ngắn gọn để hiển thị phân bố sau lọc feature, min dòng mỗi lớp, số lớp hợp lệ, và lý do chặn.
- Nếu MVC và FastAPI lệch ngưỡng do env, đồng bộ cấu hình runtime hoặc hiển thị giá trị ngưỡng từ đúng nguồn.
- Nếu cần chống dữ liệu trùng theo dự án, phân tích riêng trước khi sửa vì hiện source train tất cả dòng hợp lệ.

## 17. Danh sách file có thể cần chỉnh sửa

Chỉ đề xuất cho bước sau, chưa sửa ở bước này:

- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs`: nếu cần đồng bộ lại label hoặc thống nhất quality count.
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiService.cs`: nếu cần làm rõ cảnh báo màn Train hoặc kiểm tra stale trước train.
- `QuanLyDuAn/QuanLyDuAn/Views/Ai/Train.cshtml`: nếu cần hiển thị rõ phân bố sau lọc feature và lý do chặn.
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiTrainPageViewModel.cs`: nếu cần bổ sung trường thống kê chi tiết.
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiApiService.cs`: nếu cần sửa thông báo 422 đang liệt kê feature cũ.
- `QuanLyDuAnAIService/app/config.py`: chỉ khi cần chuẩn hóa đọc ngưỡng runtime, không hạ ngưỡng.

## 18. Quy tắc đề xuất đối với nguyên nhân dưới 5 dòng

Quy tắc nghiệp vụ mới đang xem xét không hạ ngưỡng `5` dòng mỗi nguyên nhân. Điểm thay đổi là phạm vi áp dụng ngưỡng: chỉ các nguyên nhân được đưa vào lần huấn luyện hiện tại phải có ít nhất `5` dòng hợp lệ; nguyên nhân dưới `5` dòng được loại tạm khỏi payload train và tiếp tục tích lũy.

Quy tắc sau lọc cần giữ nguyên:

- Tổng dòng được dùng train sau khi loại lớp nhỏ phải `>= 30`.
- Số lớp nguyên nhân đủ điều kiện sau khi loại lớp nhỏ phải `>= 2`.
- Mỗi lớp còn lại trong payload train phải `>= 5` dòng.
- Không xóa, sửa nhãn, nhân bản, đổi sang `Khác`, hoặc dùng `AI_KET_QUA` làm nhãn.
- Ground truth vẫn là nguyên nhân Manager xác nhận trong `AI_NGUYEN_NHAN`, được đồng bộ sang `AI_DATASET.MaDMNguyenNhan`.

Theo source hiện tại, nguyên nhân dưới `5` dòng đang chặn toàn bộ train vì cả MVC và FastAPI đều tính `minRows` trên toàn bộ lớp có mặt trong tập hợp lệ:

- MVC: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs`, method `KiemTraChatLuongDatasetNguyenNhanAsync`, biến `minRows`.
- FastAPI: `QuanLyDuAnAIService/app/services/model_service.py`, method `_validate_reason_dataset`, biến `min_rows`.
- FastAPI quality report: `QuanLyDuAnAIService/app/services/validation_service.py`, method `quality_report`, điều kiện `min(class_counts.values()) < settings.min_reason_rows_per_class`.

## 19. Tập dữ liệu trước và sau khi lọc lớp

Thứ tự hợp lý, bám theo pipeline hiện tại, nên là:

```text
Đọc AI_DATASET
→ chỉ lấy LaDuAnTre = 1
→ chỉ lấy MaDMNguyenNhan có giá trị hợp lệ
→ chỉ lấy dòng đủ 22 feature
→ group theo MaDMNguyenNhan
→ xác định lớp có số dòng >= 5
→ loại tạm lớp dưới 5 dòng khỏi lần train
→ đếm lại tổng dòng được dùng train
→ đếm lại số lớp được dùng train
→ nếu đạt 30 dòng + 2 lớp thì gửi payload sang FastAPI
```

Thứ tự này phù hợp với source hiện tại ở phần lọc nền:

- `LayDatasetNguyenNhanHopLeDeTrainAsync` đã đọc `AI_DATASET` với `LaDuAnTre == true && MaDMNguyenNhan.HasValue`, sau đó gọi `HasDuFeature`.
- `BuildReasonTrainDataset` trong `AiService.cs` kiểm tra lại `HasRequiredReasonTrainFields`, `LaDuAnTre == 1`, `MaDMNguyenNhan.HasValue` rồi map 22 feature sang payload.
- Điểm cần bổ sung là bước tách lớp đủ điều kiện và lớp đang tích lũy sau khi đã có tập hợp lệ nền.

Các câu trả lời cần chốt cho bước sửa sau:

- Lọc lớp dưới `5` dòng phải thực hiện trước khi kiểm tra tổng `30` dòng cuối cùng, vì tổng `30` phải phản ánh số dòng thật sự train.
- Số lớp tối thiểu `2` phải tính sau khi loại lớp nhỏ, vì model chỉ học các lớp còn lại.
- Phân bố UI nên hiển thị cả trước và sau lọc lớp: trước lọc để thấy dữ liệu đang tích lũy, sau lọc để thấy payload train.
- Một lớp đúng `5` dòng vẫn qua ngưỡng hiện tại. Với `train_test_split(test_size=0.2, stratify=y)`, lớp `5` dòng thường được chia khoảng `4` train và `1` test, vì `decision_tree_model.py` bật `stratify` khi mỗi lớp có ít nhất `2` dòng.
- Source chưa xử lý riêng trường hợp tập test nhỏ hơn số lớp. Với ngưỡng hiện tại `30` dòng, `2` lớp, `5` dòng/lớp, rủi ro này thấp nếu số lớp không vượt quá số mẫu test, nhưng vẫn nên ghi nhận trong kiểm thử biên.

| Chỉ tiêu | Source hiện tại | Cách hiểu sau quy tắc mới |
| -------- | --------------- | -------------------------- |
| Dòng hợp lệ trước lọc lớp | `LayDatasetNguyenNhanHopLeDeTrainAsync` trả về dòng trễ, có nhãn, đủ 22 feature | Dùng để hiển thị dữ liệu nền và phân lớp đang tích lũy |
| Dòng được dùng train | Hiện bằng toàn bộ dòng hợp lệ trước lọc lớp | Chỉ gồm lớp có `count >= 5` |
| Số lớp | Hiện bằng mọi `MaDMNguyenNhan` có trong tập hợp lệ | Chỉ lớp đủ `5` dòng |
| `minRows` | Hiện lấy min của mọi lớp, nên 1 lớp có 1 dòng chặn tất cả | Lấy min trên lớp được train; lớp dưới 5 nằm ở nhóm tích lũy |
| Nút Train | Hiện dựa trên `qualityReason.DuDieuKienTrain` trước lọc lớp | Nên dựa trên kết quả sau lọc lớp |

## 20. Vị trí cần đồng bộ giữa MVC và FastAPI

| Tầng | File | Method | Logic hiện tại | Logic dự kiến | Rủi ro |
| ---- | ---- | ------ | -------------- | ------------- | ------ |
| MVC dataset | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs` | `KiemTraChatLuongDatasetNguyenNhanAsync` | Group theo `MaDMNguyenNhan`, tính `minRows` trên mọi lớp; nếu `minRows < 5` thì `DuDieuKienTrain=false` | Sau khi có tập hợp lệ nền, tách lớp đủ `>= 5` và lớp tích lũy `< 5`; đếm lại tổng dòng/số lớp trên tập đủ điều kiện | Nếu chỉ sửa ở đây mà không sửa payload, UI báo đủ nhưng train vẫn gửi cả lớp nhỏ |
| MVC dataset | `AiDatasetService.cs` | `LayDatasetNguyenNhanHopLeDeTrainAsync` | Trả toàn bộ dòng trễ, có nhãn, đủ 22 feature | Có thể giữ nguyên là hàm lấy tập nền; thêm helper mới để lấy tập train sau lọc lớp | Đổi trực tiếp method này có thể ảnh hưởng compare model và thống kê cũ |
| MVC service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiService.cs` | `LayTrangTrainAsync` | Dùng `qualityReason.SoDongHopLeTrain`, `DuDieuKienTrain`, `PhanBoTheoNguyenNhan` cho UI | Hiển thị dòng trước lọc, dòng được train, lớp đủ điều kiện, lớp đang tích lũy | ViewModel hiện thiếu field cho hai nhóm phân bố |
| MVC service | `AiService.cs` | `TrainAsync` | Nếu `quality.DuDieuKienTrain=false` thì chặn trước FastAPI; nếu đạt thì gọi `LayDatasetNguyenNhanHopLeDeTrainAsync` rồi build payload | Chặn theo kết quả sau lọc lớp; payload chỉ gồm lớp đủ điều kiện | Nếu quality và payload dùng hai helper khác nhau có thể lệch số dòng |
| MVC service | `AiService.cs` | `BuildReasonTrainDataset` | Map tất cả dòng đầu vào thành payload train | Giữ map feature, nhưng đầu vào phải là tập đã loại lớp nhỏ hoặc helper tự lọc lại | Không nên đưa lớp nhỏ vào payload vì FastAPI mới nên loại/validate lại |
| MVC ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiDatasetManagementViewModels.cs` | `AiReasonTrainingQualitySummaryViewModel` | Có `SoDongHopLeTrain`, `SoLoaiNguyenNhan`, `PhanBoTheoNguyenNhan`, chưa có nhóm tích lũy | Bổ sung field cho `SoDongHopLeTruocLocLop`, `SoDongDuocDungTrain`, phân bố đủ điều kiện/tích lũy | Thiếu field sẽ khiến UI phải suy diễn từ một dictionary |
| MVC UI | `QuanLyDuAn/QuanLyDuAn/Views/Ai/Train.cshtml` | Razor render | Disable nút theo `Model.CoTheTrainNguyenNhan` | Disable theo kết quả sau lọc lớp; hiển thị ngắn gọn hai nhóm | Nếu chỉ đổi text mà không đổi data sẽ tiếp tục gây hiểu nhầm |
| FastAPI train | `QuanLyDuAnAIService/app/services/model_service.py` | `_validate_reason_dataset` | Tự lọc dòng thiếu feature/label, sau đó reject nếu `min_rows < 5` | Nên lọc lại lớp `< 5` để bảo vệ contract, thêm warning, rồi kiểm tra lại `30` dòng và `2` lớp | Nếu chỉ validate mà không lọc, client khác gửi lớp nhỏ sẽ bị từ chối theo rule cũ |
| FastAPI quality | `QuanLyDuAnAIService/app/services/validation_service.py` | `quality_report`, `train_recommendation` | `quality_report` reject khi bất kỳ lớp nào `< 5` | Cùng cách tính với train: báo lớp đủ điều kiện và lớp tích lũy, trainable dựa trên tập sau lọc | Màn MVC có thể báo đủ nhưng endpoint admin báo không đủ nếu lệch logic |
| FastAPI train/test | `QuanLyDuAnAIService/app/ml/decision_tree_model.py` | `train_decision_tree` | Split `test_size=0.2`, `stratify=y` nếu min class count `>= 2` | Có thể giữ nguyên nếu đầu vào đã đảm bảo mỗi lớp `>= 5` | Cần test biên khi số lớp nhiều làm số mẫu test nhỏ hơn số lớp |

Kết luận vị trí lọc: nên áp dụng ở cả MVC và FastAPI. MVC lọc để thống kê, enable/disable UI và tạo payload đúng; FastAPI lọc/validate lại để bảo vệ contract khi có client khác hoặc MVC/FastAPI chạy lệch phiên bản.

## 21. Phạm vi nguyên nhân model thực tế hỗ trợ

Source hiện tại không có trường riêng trong bảng `AI_MODEL` để lưu danh sách lớp model hỗ trợ. Entity `QuanLyDuAn/QuanLyDuAn/Models/Entities/AiModel.cs` chỉ có các field như `SoLuongDuLieu`, `DoChinhXac`, `TrainSize`, `TestSize`, `MoTaModel`, `LoaiModel`, `IsActive`.

Tuy vậy, FastAPI đã có dữ liệu lớp trong file metadata và trong model `.joblib`:

- `DecisionTreeClassifier` tự có thuộc tính `classes_` sau khi fit.
- `QuanLyDuAnAIService/app/services/model_service.py`, method `train_model`, lưu `class_distribution` và `confusion_matrix_labels` vào metadata.
- `QuanLyDuAnAIService/app/ml/model_storage.py`, method `list_models`, đọc lại `class_distribution`/`classDistribution` và `confusion_matrix_labels`/`confusionMatrixLabels`.
- `QuanLyDuAnAIService/app/schemas.py`, `ModelInfoResponse` đã có `classDistribution` và `confusionMatrixLabels`.
- Metadata hiện có `QuanLyDuAnAIService/models/reason_active.metadata.json` đang có `class_distribution`, `confusion_matrix_labels`, `feature_list`, `label_column`, `train_size`, `test_size`.

Phân biệt phạm vi:

| Phạm vi | Nguồn hiện tại | Ý nghĩa sau quy tắc mới |
| ------- | -------------- | ------------------------ |
| Danh mục nguyên nhân toàn hệ thống | `DM_NGUYEN_NHAN`, MVC đọc qua `_context.DmNguyenNhan` | Manager vẫn có thể chọn/xác nhận toàn bộ danh mục |
| Nguyên nhân có dữ liệu trong dataset | `AI_DATASET.MaDMNguyenNhan` sau khi đồng bộ | Bao gồm cả lớp đủ và lớp đang tích lũy |
| Nguyên nhân đủ điều kiện lần train | Group tập hợp lệ theo `MaDMNguyenNhan`, `count >= 5` | Chỉ nhóm này được đưa vào payload train |
| Nguyên nhân model đã học | `model.classes_`, `metadata.class_distribution`, `metadata.confusion_matrix_labels` | Phạm vi dự đoán bằng model |
| Nguyên nhân Manager xác nhận thủ công | `AI_NGUYEN_NHAN.MaDMNguyenNhan` | Không bị giới hạn bởi lớp model đã học |

Không cần thay đổi schema để lưu lớp hỗ trợ nếu bước sau dùng metadata hiện có. Có thể bổ sung field response/viewmodel lấy từ `classDistribution.Keys` hoặc `confusionMatrixLabels`; nếu cần ghi mô tả ngắn vào `MoTaModel` thì phải lưu ý giới hạn `CatChuoi(..., 250)` trong `AiService.LuuThongTinModelAsync`.

## 22. Ảnh hưởng đến luồng phân tích và RuleFallback

Mapping lớp khi model chỉ học một phần nguyên nhân hiện không có dấu hiệu lệch index:

- `QuanLyDuAnAIService/app/services/prediction_service.py`, `_predict_label_with_confidence`, dùng `int(model.predict(frame)[0])`. Với scikit-learn, `predict()` trả label thật đã train, ví dụ model học `[1, 3, 5, 7]` thì kết quả là `1`, `3`, `5` hoặc `7`, không phải vị trí `0..3`.
- `_get_ranked_probabilities` đọc `model.classes_` rồi map `probabilities[idx]` về `reason_code = int(reason_class)`.
- `_build_related_reasons` cũng dùng mã nguyên nhân thật từ ranked probability.
- `_map_reason_code_to_catalog` đối chiếu mã thật với danh mục MVC gửi sang, không giả định mã liên tục.

Luồng phân tích hiện gửi toàn bộ danh mục nguyên nhân:

- MVC `AiService` gọi FastAPI `/analyze/delay-reason` với `DanhMucNguyenNhan` lấy từ `DM_NGUYEN_NHAN`.
- FastAPI dùng danh mục này để map mã dự đoán sang tên. Nếu model không học mã `2`, model không thể dự đoán mã `2`; nhưng RuleFallback vẫn có thể map sang mã `2` nếu rule chọn tên tương ứng trong danh mục.

RuleFallback cần được hiểu đúng:

- `PredictionService.suggest_reason` dùng toàn bộ `danh_muc_nguyen_nhan`, nên có thể trả nguyên nhân chưa được model học.
- Điều này không phá vỡ training vì kết quả fallback có `reasonSource = "RuleFallback"`, không phải nhãn train.
- Manager vẫn có thể xác nhận nguyên nhân chưa học; xác nhận đó lưu vào `AI_NGUYEN_NHAN` và sau khi tổng hợp lại sẽ tích lũy trong `AI_DATASET`.
- Không nên giới hạn danh mục xác nhận của Manager theo lớp model. Nếu UI cần hiển thị, chỉ nên phân biệt nguồn gợi ý `NguyenNhanModel` và `RuleFallback`.

Rủi ro còn lại:

- UI/model management chưa hiển thị rõ model đang hỗ trợ bao nhiêu lớp.
- Nếu người dùng nhìn thấy fallback trả nguyên nhân ngoài lớp model, cần tránh hiểu nhầm đó là dự đoán trực tiếp từ model.
- `ModelCompareService.compare_models` đánh giá trên `testDataset` được gửi vào; nếu test set chứa lớp mà model mới không học, accuracy vẫn tính được nhưng phép so sánh không phản ánh cùng phạm vi lớp.

## 23. Ảnh hưởng đến metadata và đánh giá model

`SoLuongDuLieu`, `TrainSize`, `TestSize` nên phản ánh tập dữ liệu thật sự dùng để train sau khi loại lớp nhỏ:

- FastAPI `TrainResponse.soLuongDuLieu` hiện trả `int(len(x))` sau `_validate_reason_dataset` và `build_training_frames`.
- MVC `LuuThongTinModelAsync` lưu `trainResult.SoLuongDuLieu`, `TrainSize`, `TestSize` vào `AI_MODEL`.
- Vì vậy nếu FastAPI lọc lại lớp nhỏ, các số này phải là số sau lọc, không phải số hợp lệ trước lọc.

Đánh giá model khi train một phần lớp:

- `accuracy`, `precision_macro`, `recall_macro`, `f1_macro` trong `decision_tree_model.py` chỉ đo trên các lớp có trong `y_test`.
- `classification_report` hiện không dùng danh sách label cố định toàn bộ danh mục; nó dựa trên lớp thực tế trong test/predict.
- `confusion_matrix` dùng `labels_sorted = sorted(int(v) for v in y.unique().tolist())`, tức là chỉ lớp có trong lần train.
- `confusion_matrix_labels` đã phản ánh lớp model đánh giá, không phải toàn bộ `DM_NGUYEN_NHAN`.

Hàm quản lý/kích hoạt model hiện tại:

- `model_storage.list_models` trả `classDistribution` và `confusionMatrixLabels`, nhưng `Views/Ai/Models.cshtml` chưa có cột/tóm tắt số lớp hỗ trợ.
- `model_inspector.validate_model_file` chỉ kiểm tra feature schema, chưa kiểm tra số lớp hoặc danh sách lớp.
- `ModelService.set_active_model` validate load/schema rồi copy alias active, chưa cảnh báo nếu model mới hỗ trợ ít lớp hơn model đang active.
- Model cũ vẫn tương thích nếu metadata có `class_distribution` hoặc `confusion_matrix_labels`. Nếu metadata cũ thiếu hai field này, có thể load `.joblib` để đọc `classes_`, nhưng source hiện chưa expose đường này qua API.

Không nên báo accuracy như thể model hỗ trợ toàn bộ danh mục. Nếu sau này hiển thị, nên ghi ngắn: `Hỗ trợ 7 nguyên nhân`, lấy từ `classDistribution.Count` hoặc `confusionMatrixLabels.Count`.

## 24. Ảnh hưởng đến giao diện huấn luyện

`Views/Ai/Train.cshtml` hiện hiển thị:

- `Tổng dòng AI_DATASET`
- `Dòng dự án trễ`
- `Dòng có nguyên nhân xác nhận`
- `Dòng hợp lệ để train`
- `Đủ điều kiện train`
- Chart `Phân bố nguyên nhân trong dataset`

ViewModel `AiTrainPageViewModel` hiện có:

- `TongDongDataset`
- `TongDongDuAnTre`
- `TongDongCoNguyenNhanXacNhan`
- `TongDongDatasetNguyenNhan`
- `CoTheTrainNguyenNhan`
- `PhanBoNguyenNhanDataset`
- `BaoCaoDatasetNguyenNhanGanNhat`

Chưa có field để tách:

- Dòng hợp lệ trước lọc lớp.
- Dòng được dùng để train sau lọc lớp.
- Số nguyên nhân đủ điều kiện.
- Số nguyên nhân đang tích lũy.
- Phân bố đủ điều kiện và phân bố đang tích lũy.

Gợi ý hiển thị ngắn gọn cho bước sửa sau:

```text
Dòng hợp lệ trước lọc lớp: 42
Dòng được dùng để train: 38
Nguyên nhân đủ điều kiện: 6/10
Nguyên nhân đang tích lũy: 4
```

Với từng nguyên nhân:

| Nguyên nhân | Số dòng hợp lệ | Trạng thái |
| ----------- | -------------: | ---------- |
| Thiếu nhân sự | 8 | Đủ điều kiện |
| Vượt ngân sách | 2 | Đang tích lũy |

Nút Train nên disable theo tập sau lọc. Cảnh báo `Mỗi nguyên nhân cần tối thiểu 5 dòng` không còn nên hiểu là mọi nguyên nhân trong dataset đều phải đủ 5 dòng; nên đổi ngắn thành một trong các thông báo:

- `Các nguyên nhân dưới 5 dòng sẽ tiếp tục tích lũy.`
- `Chưa đủ 30 dòng hoặc 2 nguyên nhân sau khi lọc lớp dưới 5 dòng.`

Không nên thêm đoạn giải thích dài vào màn hình.

## 25. Các trường hợp biên

| Trường hợp | Hành vi hiện tại | Hành vi hợp lý | File/method liên quan |
| ---------- | ---------------- | -------------- | --------------------- |
| Có 10 nguyên nhân nhưng chỉ 1 nguyên nhân đủ 5 dòng | Bị chặn vì có lớp dưới 5 và/hoặc lớp hợp lệ không đạt | Sau lọc còn 1 lớp, vẫn chặn vì cần ít nhất 2 lớp | `AiDatasetService.KiemTraChatLuongDatasetNguyenNhanAsync`, `ModelService._validate_reason_dataset` |
| Có 2 nguyên nhân đủ 5 dòng nhưng tổng chỉ 12 dòng | Hiện bị chặn vì tổng `<30` | Sau lọc vẫn chặn vì dòng train `<30` | Cùng hai method validate |
| Có 5 nguyên nhân đủ điều kiện và tổng 31 dòng | Hiện vẫn có thể bị chặn nếu còn một lớp 1-4 dòng | Sau lọc được train nếu 31 dòng thuộc lớp đủ điều kiện | MVC quality + payload, FastAPI validate |
| Có một nguyên nhân đúng 5 dòng | Hiện đạt nếu mọi lớp khác cũng `>=5` | Vẫn được giữ trong tập train | `decision_tree_model.train_decision_tree` |
| Sau khi lọc còn đúng 2 nguyên nhân | Đạt nếu không có lớp nhỏ | Đạt nếu tổng sau lọc `>=30` | MVC/FastAPI ngưỡng `2` lớp |
| Một lớp đủ 5 dòng ở MVC nhưng FastAPI chỉ nhận 4 do một dòng lỗi schema | Có thể FastAPI từ chối vì `min_rows <5` | FastAPI phải validate/lọc lại và trả lỗi/warning rõ; MVC cần giữ payload đúng 22 feature | `schemas.py`, `_validate_reason_dataset`, `AiApiService.BuildErrorPayload` |
| Model cũ hỗ trợ 8 lớp, model mới chỉ hỗ trợ 5 lớp | Kích hoạt không cảnh báo riêng nếu schema đúng | Cho phép nhưng nên hiển thị/cảnh báo số lớp hỗ trợ trước khi activate | `ModelService.set_active_model`, `Views/Ai/Models.cshtml` |
| Nguyên nhân từng được train nhưng lần train mới không còn đủ 5 dòng | Hiện khó xảy ra nếu dataset không giảm, nhưng source không bảo vệ riêng | Lần train mới không học lớp đó; metadata phải thể hiện phạm vi mới | Metadata `class_distribution`, `confusion_matrix_labels` |
| Mã nguyên nhân không liên tục `[1,2,5,8]` | Train/predict vẫn dùng mã thật | Vẫn an toàn, vì mapping dùng `model.classes_` và mã thật | `PredictionService._get_ranked_probabilities` |
| Một nguyên nhân đổi tên nhưng giữ nguyên mã | Model dự đoán mã; UI map tên mới từ danh mục | An toàn về mã, cần chấp nhận tên hiển thị mới | `PredictionService._map_reason_code_to_catalog`, MVC `TenNguyenNhanTheoMa` |
| Manager xác nhận nguyên nhân model chưa học | Vẫn có thể lưu xác nhận nếu danh mục hợp lệ | Phải tiếp tục cho phép để tích lũy nhãn thật | `AiController.XacNhanNguyenNhan`, `AiService.XacNhanNguyenNhanAsync` |
| RuleFallback trả nguyên nhân model chưa học | Có thể xảy ra vì fallback dùng toàn bộ danh mục | Cho phép, nhưng UI cần phân biệt `RuleFallback` với model | `PredictionService.suggest_reason`, `PredictProjectResponse.reasonSource` |
| Tập test nhỏ hơn số lớp | Source chưa xử lý riêng; sklearn có thể lỗi nếu stratify không chia được | Cần kiểm thử; không đổi `test_size` nếu ngưỡng hiện tại vẫn đảm bảo | `decision_tree_model.train_decision_tree` |
| Một lớp có 5 dòng nhưng sau split chỉ có 1 dòng test | Hiện được stratify, thường 4 train/1 test | Chấp nhận theo ngưỡng hiện tại | `train_test_split(..., stratify=y)` |
| FastAPI chạy cấu hình ngưỡng khác MVC | Có thể MVC báo đủ nhưng FastAPI từ chối hoặc ngược lại | Cần đồng bộ cấu hình/ngưỡng và hiển thị nguồn ngưỡng khi debug | `AiDatasetService` constants, `config.py` env settings |

## 26. Truy vấn SQL kiểm tra tập sau lọc

Các truy vấn dưới đây chỉ dùng `SELECT`. Điều kiện feature bám theo 22 cột hiện có trong source.

```sql
-- 1. Nguyên nhân đủ từ 5 dòng hợp lệ trở lên.
WITH DongHopLe AS (
    SELECT ds.MaData, ds.MaDuAn, ds.MaDMNguyenNhan
    FROM AI_DATASET ds
    WHERE ds.LaDuAnTre = 1
      AND ds.MaDMNguyenNhan IS NOT NULL
      AND ds.SoNhanVienDuAn IS NOT NULL
      AND ds.TongSoCongViec IS NOT NULL
      AND ds.SoCongViecTre IS NOT NULL
      AND ds.TyLeCongViecTre IS NOT NULL
      AND ds.ChiPhiDuKien IS NOT NULL
      AND ds.ChiPhiThucTe IS NOT NULL
      AND ds.ChenhLechChiPhi IS NOT NULL
      AND ds.SoLanThayDoiNhanSu IS NOT NULL
      AND ds.SoLanThayDoiQuanLy IS NOT NULL
      AND ds.SoNgayTreTienDo IS NOT NULL
      AND ds.SoDeXuatCongViecChoDuyet IS NOT NULL
      AND ds.SoDeXuatCongViecBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetCongViecTrungBinh IS NOT NULL
      AND ds.SoDeXuatNganSachChoDuyet IS NOT NULL
      AND ds.SoDeXuatNganSachBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetNganSachTrungBinh IS NOT NULL
      AND ds.SoBaoCaoTienDoChoDuyet IS NOT NULL
      AND ds.SoBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoBaoCaoTienDoYeuCauBoSung IS NOT NULL
      AND ds.TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoLanCapNhatTienDo IS NOT NULL
      AND ds.SoNgayChamCapNhatTienDo IS NOT NULL
)
SELECT d.MaDMNguyenNhan AS [Mã nguyên nhân],
       COALESCE(dm.TenNguyenNhan, N'Không có trong danh mục') AS [Tên nguyên nhân],
       COUNT(*) AS [Số dòng hợp lệ]
FROM DongHopLe d
LEFT JOIN DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = d.MaDMNguyenNhan
GROUP BY d.MaDMNguyenNhan, dm.TenNguyenNhan
HAVING COUNT(*) >= 5
ORDER BY [Số dòng hợp lệ] DESC, [Mã nguyên nhân];
```

```sql
-- 2. Nguyên nhân dưới 5 dòng hợp lệ, sẽ tiếp tục tích lũy.
WITH DongHopLe AS (
    SELECT ds.MaData, ds.MaDuAn, ds.MaDMNguyenNhan
    FROM AI_DATASET ds
    WHERE ds.LaDuAnTre = 1
      AND ds.MaDMNguyenNhan IS NOT NULL
      AND ds.SoNhanVienDuAn IS NOT NULL
      AND ds.TongSoCongViec IS NOT NULL
      AND ds.SoCongViecTre IS NOT NULL
      AND ds.TyLeCongViecTre IS NOT NULL
      AND ds.ChiPhiDuKien IS NOT NULL
      AND ds.ChiPhiThucTe IS NOT NULL
      AND ds.ChenhLechChiPhi IS NOT NULL
      AND ds.SoLanThayDoiNhanSu IS NOT NULL
      AND ds.SoLanThayDoiQuanLy IS NOT NULL
      AND ds.SoNgayTreTienDo IS NOT NULL
      AND ds.SoDeXuatCongViecChoDuyet IS NOT NULL
      AND ds.SoDeXuatCongViecBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetCongViecTrungBinh IS NOT NULL
      AND ds.SoDeXuatNganSachChoDuyet IS NOT NULL
      AND ds.SoDeXuatNganSachBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetNganSachTrungBinh IS NOT NULL
      AND ds.SoBaoCaoTienDoChoDuyet IS NOT NULL
      AND ds.SoBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoBaoCaoTienDoYeuCauBoSung IS NOT NULL
      AND ds.TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoLanCapNhatTienDo IS NOT NULL
      AND ds.SoNgayChamCapNhatTienDo IS NOT NULL
)
SELECT d.MaDMNguyenNhan AS [Mã nguyên nhân],
       COALESCE(dm.TenNguyenNhan, N'Không có trong danh mục') AS [Tên nguyên nhân],
       COUNT(*) AS [Số dòng hợp lệ]
FROM DongHopLe d
LEFT JOIN DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = d.MaDMNguyenNhan
GROUP BY d.MaDMNguyenNhan, dm.TenNguyenNhan
HAVING COUNT(*) < 5
ORDER BY [Số dòng hợp lệ], [Mã nguyên nhân];
```

```sql
-- 3, 4, 5. Tổng dòng sau lọc, số lớp đủ điều kiện và kết luận 30 dòng + 2 lớp.
WITH DongHopLe AS (
    SELECT ds.MaData, ds.MaDuAn, ds.MaDMNguyenNhan
    FROM AI_DATASET ds
    WHERE ds.LaDuAnTre = 1
      AND ds.MaDMNguyenNhan IS NOT NULL
      AND ds.SoNhanVienDuAn IS NOT NULL
      AND ds.TongSoCongViec IS NOT NULL
      AND ds.SoCongViecTre IS NOT NULL
      AND ds.TyLeCongViecTre IS NOT NULL
      AND ds.ChiPhiDuKien IS NOT NULL
      AND ds.ChiPhiThucTe IS NOT NULL
      AND ds.ChenhLechChiPhi IS NOT NULL
      AND ds.SoLanThayDoiNhanSu IS NOT NULL
      AND ds.SoLanThayDoiQuanLy IS NOT NULL
      AND ds.SoNgayTreTienDo IS NOT NULL
      AND ds.SoDeXuatCongViecChoDuyet IS NOT NULL
      AND ds.SoDeXuatCongViecBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetCongViecTrungBinh IS NOT NULL
      AND ds.SoDeXuatNganSachChoDuyet IS NOT NULL
      AND ds.SoDeXuatNganSachBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetNganSachTrungBinh IS NOT NULL
      AND ds.SoBaoCaoTienDoChoDuyet IS NOT NULL
      AND ds.SoBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoBaoCaoTienDoYeuCauBoSung IS NOT NULL
      AND ds.TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoLanCapNhatTienDo IS NOT NULL
      AND ds.SoNgayChamCapNhatTienDo IS NOT NULL
),
LopDuDieuKien AS (
    SELECT MaDMNguyenNhan
    FROM DongHopLe
    GROUP BY MaDMNguyenNhan
    HAVING COUNT(*) >= 5
),
TapTrain AS (
    SELECT d.*
    FROM DongHopLe d
    INNER JOIN LopDuDieuKien l ON l.MaDMNguyenNhan = d.MaDMNguyenNhan
)
SELECT COUNT(*) AS [Tổng dòng được dùng train],
       COUNT(DISTINCT MaDMNguyenNhan) AS [Số lớp đủ điều kiện],
       CASE
           WHEN COUNT(*) >= 30 AND COUNT(DISTINCT MaDMNguyenNhan) >= 2 THEN N'Đạt'
           ELSE N'Chưa đạt'
       END AS [Kết luận sau lọc];
```

```sql
-- 6. Dự án bị loại khỏi lần train vì thuộc lớp dưới 5 dòng.
WITH DongHopLe AS (
    SELECT ds.MaData, ds.MaDuAn, ds.MaDMNguyenNhan, ds.NgayTongHop
    FROM AI_DATASET ds
    WHERE ds.LaDuAnTre = 1
      AND ds.MaDMNguyenNhan IS NOT NULL
      AND ds.SoNhanVienDuAn IS NOT NULL
      AND ds.TongSoCongViec IS NOT NULL
      AND ds.SoCongViecTre IS NOT NULL
      AND ds.TyLeCongViecTre IS NOT NULL
      AND ds.ChiPhiDuKien IS NOT NULL
      AND ds.ChiPhiThucTe IS NOT NULL
      AND ds.ChenhLechChiPhi IS NOT NULL
      AND ds.SoLanThayDoiNhanSu IS NOT NULL
      AND ds.SoLanThayDoiQuanLy IS NOT NULL
      AND ds.SoNgayTreTienDo IS NOT NULL
      AND ds.SoDeXuatCongViecChoDuyet IS NOT NULL
      AND ds.SoDeXuatCongViecBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetCongViecTrungBinh IS NOT NULL
      AND ds.SoDeXuatNganSachChoDuyet IS NOT NULL
      AND ds.SoDeXuatNganSachBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetNganSachTrungBinh IS NOT NULL
      AND ds.SoBaoCaoTienDoChoDuyet IS NOT NULL
      AND ds.SoBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoBaoCaoTienDoYeuCauBoSung IS NOT NULL
      AND ds.TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoLanCapNhatTienDo IS NOT NULL
      AND ds.SoNgayChamCapNhatTienDo IS NOT NULL
),
DemLop AS (
    SELECT MaDMNguyenNhan, COUNT(*) AS SoDongHopLe
    FROM DongHopLe
    GROUP BY MaDMNguyenNhan
)
SELECT d.MaDuAn AS [Mã dự án],
       d.MaData AS [Mã dòng dataset],
       d.MaDMNguyenNhan AS [Mã nguyên nhân],
       COALESCE(dm.TenNguyenNhan, N'Không có trong danh mục') AS [Tên nguyên nhân],
       c.SoDongHopLe AS [Số dòng hợp lệ của lớp],
       d.NgayTongHop AS [Ngày tổng hợp]
FROM DongHopLe d
INNER JOIN DemLop c ON c.MaDMNguyenNhan = d.MaDMNguyenNhan
LEFT JOIN DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = d.MaDMNguyenNhan
WHERE c.SoDongHopLe < 5
ORDER BY c.SoDongHopLe, d.MaDMNguyenNhan, d.MaDuAn;
```

```sql
-- 7. So sánh tổng dòng hợp lệ trước lọc, dòng dùng train, dòng đang tích lũy.
WITH DongHopLe AS (
    SELECT ds.MaData, ds.MaDuAn, ds.MaDMNguyenNhan
    FROM AI_DATASET ds
    WHERE ds.LaDuAnTre = 1
      AND ds.MaDMNguyenNhan IS NOT NULL
      AND ds.SoNhanVienDuAn IS NOT NULL
      AND ds.TongSoCongViec IS NOT NULL
      AND ds.SoCongViecTre IS NOT NULL
      AND ds.TyLeCongViecTre IS NOT NULL
      AND ds.ChiPhiDuKien IS NOT NULL
      AND ds.ChiPhiThucTe IS NOT NULL
      AND ds.ChenhLechChiPhi IS NOT NULL
      AND ds.SoLanThayDoiNhanSu IS NOT NULL
      AND ds.SoLanThayDoiQuanLy IS NOT NULL
      AND ds.SoNgayTreTienDo IS NOT NULL
      AND ds.SoDeXuatCongViecChoDuyet IS NOT NULL
      AND ds.SoDeXuatCongViecBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetCongViecTrungBinh IS NOT NULL
      AND ds.SoDeXuatNganSachChoDuyet IS NOT NULL
      AND ds.SoDeXuatNganSachBiTuChoi IS NOT NULL
      AND ds.ThoiGianDuyetNganSachTrungBinh IS NOT NULL
      AND ds.SoBaoCaoTienDoChoDuyet IS NOT NULL
      AND ds.SoBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoBaoCaoTienDoYeuCauBoSung IS NOT NULL
      AND ds.TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
      AND ds.SoLanCapNhatTienDo IS NOT NULL
      AND ds.SoNgayChamCapNhatTienDo IS NOT NULL
),
DemLop AS (
    SELECT MaDMNguyenNhan, COUNT(*) AS SoDongHopLe
    FROM DongHopLe
    GROUP BY MaDMNguyenNhan
)
SELECT COUNT(*) AS [Dòng hợp lệ trước lọc lớp],
       SUM(CASE WHEN c.SoDongHopLe >= 5 THEN 1 ELSE 0 END) AS [Dòng được dùng train],
       SUM(CASE WHEN c.SoDongHopLe < 5 THEN 1 ELSE 0 END) AS [Dòng đang tích lũy]
FROM DongHopLe d
INNER JOIN DemLop c ON c.MaDMNguyenNhan = d.MaDMNguyenNhan;
```

```sql
-- 8. Kiểm tra mã nguyên nhân không liên tục trong danh mục.
WITH OrderedReasons AS (
    SELECT MaDMNguyenNhan,
           TenNguyenNhan,
           LAG(MaDMNguyenNhan) OVER (ORDER BY MaDMNguyenNhan) AS MaTruoc
    FROM DM_NGUYEN_NHAN
)
SELECT MaDMNguyenNhan AS [Mã nguyên nhân],
       TenNguyenNhan AS [Tên nguyên nhân],
       MaTruoc AS [Mã trước đó],
       MaDMNguyenNhan - MaTruoc AS [Khoảng cách mã]
FROM OrderedReasons
WHERE MaTruoc IS NOT NULL
  AND MaDMNguyenNhan - MaTruoc > 1
ORDER BY MaDMNguyenNhan;
```

```sql
-- 9. Kiểm tra khả năng cùng tên sau chuẩn hóa đơn giản nhưng khác mã.
SELECT TenNguyenNhan AS [Tên nguyên nhân],
       COUNT(*) AS [Số mã cùng tên],
       STRING_AGG(CAST(MaDMNguyenNhan AS nvarchar(20)), N', ') AS [Danh sách mã]
FROM DM_NGUYEN_NHAN
GROUP BY TenNguyenNhan
HAVING COUNT(*) > 1
ORDER BY [Số mã cùng tên] DESC, [Tên nguyên nhân];
```

```sql
-- 10. Kiểm tra số lớp model mới nhất đã lưu trong AI_MODEL nếu MoTaModel có ghi lớp hỗ trợ.
SELECT TOP (20)
       MaModel AS [Mã model],
       TenModel AS [Tên model],
       SoLuongDuLieu AS [Số lượng dữ liệu],
       TrainSize AS [Train size],
       TestSize AS [Test size],
       DoChinhXac AS [Độ chính xác],
       MoTaModel AS [Mô tả model],
       IsActive AS [Đang kích hoạt],
       NgayTao AS [Ngày tạo]
FROM AI_MODEL
WHERE IsDeleted <> 1 OR IsDeleted IS NULL
ORDER BY NgayTao DESC, MaModel DESC;
```

Lưu ý: danh sách lớp hỗ trợ hiện đầy đủ hơn trong file metadata FastAPI (`class_distribution`, `confusion_matrix_labels`) chứ không nằm thành cột riêng trong `AI_MODEL`.

## 27. Phương án kiến trúc được đề xuất

Phương án phù hợp nhất với kiến trúc hiện tại là **Phương án C: MVC và FastAPI cùng áp dụng helper/quy tắc tương đương**.

Lý do theo source:

- MVC là system-of-record, đọc `AI_DATASET`, quyết định enable/disable nút Train và tạo payload. Nếu MVC không lọc lớp nhỏ, UI vẫn sai và payload vẫn chứa lớp đang tích lũy.
- FastAPI là compute-only nhưng là điểm train thật. Nếu FastAPI không validate/lọc lại, payload từ client khác hoặc phiên bản MVC cũ có thể phá rule.
- Hai tầng hiện đã có ngưỡng riêng: MVC hard-code `MinReasonTrainRows`, `MinReasonClasses`, `MinReasonRowsPerClass`; FastAPI đọc `MIN_REASON_TRAIN_ROWS`, `MIN_REASON_CLASS_COUNT`, `MIN_REASON_ROWS_PER_CLASS` qua `config.py`. Vì vậy bước sửa sau phải đồng bộ logic và kiểm tra lệch cấu hình.

Quy ước đề xuất:

- MVC lọc để thống kê, hiển thị và tạo payload.
- FastAPI lọc lại lớp `< MIN_REASON_ROWS_PER_CLASS`, trả warning về số dòng/lớp bị loại, rồi kiểm tra lại tổng dòng và số lớp.
- Metadata FastAPI lưu/giữ `class_distribution` và `confusion_matrix_labels` như danh sách lớp model đã học.
- MVC khi lưu `AI_MODEL` giữ `SoLuongDuLieu`, `TrainSize`, `TestSize` theo tập sau lọc; nếu cần hiển thị số lớp hỗ trợ thì lấy từ metadata/API, không đổi schema.

## 28. Danh sách file và method có thể cần chỉnh sửa

Chỉ là danh sách chuẩn bị cho bước sau, chưa sửa code ở bước tài liệu này.

| File | Method/class | Lý do có thể cần chỉnh sửa |
| ---- | ------------ | -------------------------- |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs` | `KiemTraChatLuongDatasetNguyenNhanAsync` | Tính thống kê trước/sau lọc lớp, không để một lớp `<5` chặn toàn bộ nếu tập còn lại đạt |
| `AiDatasetService.cs` | helper mới, ví dụ `LocLopNguyenNhanDuDieuKienTrain` | Dùng chung giữa quality và lấy payload để tránh lệch logic |
| `AiDatasetService.cs` | `LayDatasetNguyenNhanHopLeDeTrainAsync` | Nên cân nhắc giữ là tập hợp lệ nền; nếu đổi trực tiếp phải rà tác động compare/thống kê |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IAiDatasetService.cs` | interface dataset | Nếu thêm method trả tập sau lọc hoặc summary mới |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiDatasetManagementViewModels.cs` | `AiReasonTrainingQualitySummaryViewModel` | Bổ sung field trước lọc/sau lọc, phân bố đủ điều kiện/tích lũy |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiTrainPageViewModel.cs` | `AiTrainPageViewModel` | Bổ sung số liệu UI ngắn gọn |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiService.cs` | `LayTrangTrainAsync` | Map summary mới ra UI, cảnh báo ngắn |
| `AiService.cs` | `TrainAsync` | Lấy dataset đã lọc lớp để build payload; chặn theo kết quả sau lọc |
| `AiService.cs` | `CompareModelAsync` | Cân nhắc dùng tập test cùng phạm vi lớp hoặc ghi rõ so sánh khi hai model hỗ trợ lớp khác nhau |
| `AiService.cs` | `LuuThongTinModelAsync` | Nếu muốn ghi số lớp hỗ trợ vào mô tả ngắn, không đổi schema |
| `QuanLyDuAn/QuanLyDuAn/Views/Ai/Train.cshtml` | Razor Train | Hiển thị dòng trước lọc, dòng dùng train, nguyên nhân đủ điều kiện/đang tích lũy; disable theo sau lọc |
| `QuanLyDuAn/QuanLyDuAn/Views/Ai/Models.cshtml` | Razor Models | Nếu cần hiển thị số lớp model hỗ trợ từ metadata |
| `QuanLyDuAnAIService/app/services/model_service.py` | `_validate_reason_dataset` | Lọc/validate lại lớp `<5`, cảnh báo lớp bị loại, kiểm tra lại `30` và `2` |
| `QuanLyDuAnAIService/app/services/validation_service.py` | `quality_report`, `train_recommendation` | Đồng bộ cách tính với train endpoint |
| `QuanLyDuAnAIService/app/schemas.py` | response schema | Nếu cần trả thêm `eligibleClassCount`, `droppedClassDistribution`, `usedClassDistribution` |
| `QuanLyDuAnAIService/app/ml/decision_tree_model.py` | `train_decision_tree` | Có thể giữ nguyên; chỉ cần test biên split/stratify |
| `QuanLyDuAnAIService/app/ml/model_storage.py` | `list_models` | Nếu cần expose số lớp hỗ trợ rõ hơn từ `class_distribution` |
| `QuanLyDuAnAIService/app/services/prediction_service.py` | `_get_ranked_probabilities`, `suggest_reason` | Không cần sửa mapping lớp; chỉ cân nhắc cảnh báo nguồn `RuleFallback` nếu UI yêu cầu |

## 29. Các thay đổi đã triển khai

Đã sửa luồng huấn luyện model nguyên nhân trễ để một nguyên nhân dưới 5 dòng không còn chặn toàn bộ lần train nếu các nguyên nhân còn lại đã đủ điều kiện.

Phía MVC:

- Giữ `LayDatasetNguyenNhanHopLeDeTrainAsync` là tập hợp lệ nền: dự án trễ, có `MaDMNguyenNhan`, đủ 22 feature.
- Thêm `PhanLoaiDatasetNguyenNhanDeTrainAsync` để tách `TapHopLeTruocLocLop`, `TapDuocDungTrain`, `TapDangTichLuy`.
- `KiemTraChatLuongDatasetNguyenNhanAsync` dùng thống kê sau lọc lớp để quyết định `DuDieuKienTrain`.
- `AiService.TrainAsync` build payload từ `TapDuocDungTrain`, không gửi lớp dưới 5 dòng sang FastAPI.
- `Views/Ai/Train.cshtml` hiển thị dòng hợp lệ, dòng dùng train, số nguyên nhân đủ điều kiện và số nguyên nhân đang tích lũy.

Phía FastAPI:

- Thêm helper `reason_dataset_policy.py` dùng chung cho train, quality report và train recommendation.
- `_validate_reason_dataset` lọc dòng thiếu feature, label không hợp lệ, lớp dưới 5 dòng, sau đó kiểm tra lại 30 dòng và 2 lớp.
- `ValidationService.quality_report`, `train_recommendation`, `validate_dataset` dùng cùng quy tắc với `/model/train`.
- `TrainResponse` và quality response có thêm metadata trước/sau lọc lớp.
- Metadata model mới ghi `class_distribution`, `confusion_matrix_labels`, `used_class_distribution`, `dropped_class_distribution` theo tập thực sự dùng train.

Không thay đổi database, không tạo migration, không chỉnh schema, không sửa seed SQL.

## 30. Luồng huấn luyện sau chỉnh sửa

Luồng MVC hiện tại:

```text
Đọc AI_DATASET
-> chỉ lấy dự án trễ
-> chỉ lấy dòng có MaDMNguyenNhan
-> chỉ lấy dòng đủ 22 feature
-> nhóm theo MaDMNguyenNhan
-> lớp >= 5 dòng đưa vào TapDuocDungTrain
-> lớp < 5 dòng đưa vào TapDangTichLuy
-> kiểm tra TapDuocDungTrain >= 30 dòng và >= 2 lớp
-> BuildReasonTrainDataset từ TapDuocDungTrain
-> gọi FastAPI /model/train
```

Luồng FastAPI hiện tại:

```text
Validate schema Pydantic
-> loại dòng không phải dự án trễ nếu có LaDuAnTre khác 1/true
-> loại dòng thiếu feature
-> loại dòng thiếu hoặc sai MaDMNguyenNhan
-> nhóm theo MaDMNguyenNhan
-> loại tạm lớp < MIN_REASON_ROWS_PER_CLASS
-> kiểm tra lại MIN_REASON_TRAIN_ROWS và MIN_REASON_CLASS_COUNT
-> train DecisionTreeClassifier bằng mã MaDMNguyenNhan thật
```

## 31. Quy tắc lọc lớp đang áp dụng

Ngưỡng hiện tại:

```text
MinReasonTrainRows = 30
MinReasonClasses = 2
MinReasonRowsPerClass = 5
```

Điều kiện cuối cùng:

```text
SoDongDuocDungTrain >= 30
SoLopDuDieuKien >= 2
Mỗi lớp trong TapDuocDungTrain >= 5 dòng
```

Các lớp dưới 5 dòng:

- Không bị xóa.
- Không đổi nhãn.
- Không chuyển sang `Khác`.
- Không nhân bản hoặc tạo dữ liệu giả.
- Không gửi vào payload train hiện tại.
- Vẫn nằm trong `AI_DATASET` và tiếp tục tích lũy cho lần train sau.

Nhãn train vẫn là `MaDMNguyenNhan`. MVC và FastAPI không map mã nguyên nhân sang index `0..n-1`. Prediction vẫn dùng `model.predict(frame)[0]`, còn xác suất được map theo `model.classes_`.

## 32. Các file đã chỉnh sửa

MVC:

- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IAiDatasetService.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiDatasetManagementViewModels.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiTrainPageViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiDatasetApiViewModels.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiModelApiViewModels.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/Ai/Train.cshtml`

FastAPI:

- `QuanLyDuAnAIService/app/services/reason_dataset_policy.py`
- `QuanLyDuAnAIService/app/services/model_service.py`
- `QuanLyDuAnAIService/app/services/validation_service.py`
- `QuanLyDuAnAIService/app/schemas.py`
- `QuanLyDuAnAIService/app/ml/decision_tree_model.py`

Tài liệu:

- `docs/huanluyenmodelnguyennhantre.md`

## 33. Kết quả build và kiểm thử

Đã chạy:

```text
dotnet build QuanLyDuAn/QuanLyDuAn.sln
```

Kết quả: build thành công. Có 2 warning CS1998 sẵn có trong `FileTienDoCongViecService.cs`, không thuộc phạm vi thay đổi này.

Đã chạy:

```text
python -m compileall app
```

Kết quả: compile Python thành công.

Đã kiểm thử FastAPI bằng route handler trực tiếp với `MODEL_DIR` tạm, không ghi database và không dùng thư mục model thật:

- Case 1: `10, 8, 7, 6, 2, 1` -> dùng train 31 dòng, 4 lớp, train được, lớp 2 và 1 dòng bị đưa vào `droppedClassDistribution`.
- Case 2: `6, 6, 2` -> sau lọc còn 12 dòng, không train.
- Case 3: `35, 2` -> sau lọc còn 1 lớp, không train.
- Case 4: `15, 15, 1` -> sau lọc còn 30 dòng, 2 lớp, train được.
- Case 5: mã lớp không liên tục `[1, 3, 5, 8]` -> metadata `confusionMatrixLabels` giữ `[1, 3, 5, 8]`, `classDistribution` chỉ có các lớp được train.
- Case 6: một lớp đúng 5 dòng -> lớp được đưa vào train, split không lỗi.
- Case 7: payload có một dòng thiếu feature -> FastAPI loại dòng lỗi, lớp còn dưới 5 bị loại, kiểm tra lại và trả lỗi rõ.
- Kiểm tra `quality-report`, `train-recommendation`, `/model/train`, `test-reason` dùng cùng logic sau lọc.

Không chạy được `fastapi.testclient.TestClient` vì môi trường Python hiện thiếu package `httpx`. Đã thay bằng gọi trực tiếp route handler để đi qua cùng code xử lý endpoint.

## 34. Các giới hạn còn lại

- Chưa kiểm thử runtime qua trình duyệt MVC thật và chưa gọi HTTP thật tới FastAPI vì không khởi động server trong lượt sửa này.
- Chưa kiểm thử với dữ liệu SQL thật; toàn bộ kiểm thử nghiệp vụ dùng dữ liệu giả trong bộ nhớ, đúng ràng buộc không tạo hoặc sửa dữ liệu database.
- Màn quản lý model chưa bổ sung hiển thị "Hỗ trợ N nguyên nhân" để tránh mở rộng phạm vi UI không bắt buộc.
- Model cũ thiếu metadata mới vẫn được giữ tương thích qua các field default/optional; số liệu lớp hỗ trợ đầy đủ nhất vẫn nằm trong metadata model mới.
