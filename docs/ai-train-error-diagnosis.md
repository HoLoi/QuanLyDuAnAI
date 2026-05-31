# 1. Tóm tắt lỗi hiện tại

Nguồn hiển thị UI:
- `Đủ điều kiện huấn luyện: Không` lấy từ `Views/AiDataset/Index.cshtml` (field `Model.ChatLuongDatasetDb.DuDieuKienTrain`).
- Lý do `Mỗi nguyên nhân cần tối thiểu 5 dòng` sinh ra từ `AiDatasetService.KiemTraChatLuongDatasetAsync()`.

Số liệu thực tế từ DB (đã chạy query ngày 2026-05-27):
- Tổng số dòng `AI_DATASET`: **312**
- Số dòng hợp lệ để huấn luyện (LaDuAnTre=1 + có nhãn + đủ 10 feature): **230**
- Số dòng thiếu nhãn train (`TongSoDong - SoDongDuLabel` theo MVC): **82**
- Số dòng `LaDuAnTre = 1`: **230**
- Số dòng `LaDuAnTre = 0`: **76**
- Số dòng có `MaDMNguyenNhan`: **230**
- UI báo `Đủ điều kiện huấn luyện: Không`: **Đúng**
- Lý do UI báo `Mỗi nguyên nhân cần tối thiểu 5 dòng`: do phân bố lớp hiện tại có 2 nguyên nhân chỉ có **3 dòng**:
  - `MaDMNguyenNhan=7` (Thiếu dữ liệu hoặc tài liệu): 3
  - `MaDMNguyenNhan=10` (Khác): 3

# 2. Điều kiện train thật trong MVC

## 2.1 Method kiểm tra chất lượng dataset
- `AiDatasetService.KiemTraChatLuongDatasetAsync(...)`
- `AiDatasetService.KiemTraChatLuongDatasetNguyenNhanAsync(...)`

## 2.2 Method lấy dataset train
- `AiDatasetService.LayDatasetNguyenNhanHopLeDeTrainAsync(...)`
- `AiService.TrainAsync(...)` gọi method trên, rồi `BuildReasonTrainDataset(...)` để gửi FastAPI.

## 2.3 Checklist filter (đúng theo source)
- Có lọc `LaDuAnTre = true`: **Có**
- Có lọc `MaDMNguyenNhan != null`: **Có**
- Có lọc đủ 10 feature: **Có** (`HasDuFeature` / `HasRequiredReasonTrainFields`)
- Có bắt `MaDMNguyenNhan > 0`: **MVC không bắt tường minh** (chỉ `HasValue`; >0 được FastAPI kiểm tra)
- Có lọc `GhiChuDataset`: **Không**
- Có lấy latest snapshot theo dự án: **Không** (không `ROW_NUMBER/PARTITION` trong train query)
- Có join `AI_NGUYEN_NHAN` khi train: **Không**

## 2.4 Group theo tất cả DM hay chỉ label xuất hiện trong dataset
- MVC đang group theo **label xuất hiện trong dataset** (`GroupBy(MaDMNguyenNhan)` trên dữ liệu đã lọc), **không** left-join toàn bộ `DM_NGUYEN_NHAN`.

## 2.5 Có coi nguyên nhân `0 dòng` là fail không
- **Không**. Class không xuất hiện thì không vào `GroupBy`, nên không tự fail vì `0 dòng`.

# 3. Điều kiện train thật trong FastAPI

## 3.1 Hằng số
Từ `.env`, `.env.example`, `app/config.py`:
- `MIN_REASON_TRAIN_ROWS = 30`
- `MIN_REASON_CLASS_COUNT = 2`
- `MIN_REASON_ROWS_PER_CLASS = 5`

## 3.2 FastAPI filter lại dataset
Trong `model_service._validate_reason_dataset(...)`:
- Bỏ dòng không thuộc `LaDuAnTre=1`
- Bỏ dòng thiếu feature
- Bỏ dòng `MaDMNguyenNhan` null/không parse int/<=0
- Sau đó mới kiểm tra ngưỡng 30 dòng, >=2 lớp, và min mỗi lớp >=5

## 3.3 FastAPI có bỏ dòng MVC đã gửi không
- **Có thể có** (nếu MVC gửi label <=0 hoặc thiếu feature), nhưng với DB hiện tại thì không thấy ảnh hưởng vì 312/312 dòng đủ feature và label train hợp lệ đều >0.

## 3.4 FastAPI có yêu cầu stratify không
- Có dùng `train_test_split(..., stratify=...)` trong `decision_tree_model.py`.
- Nhưng `stratify` chỉ bật khi mỗi lớp có >=2 dòng (`can_stratify`), nếu không thì `stratify=None`.

## 3.5 Nếu class <5 dòng thì lỗi ở MVC hay FastAPI
- **Cả hai đều chặn**.
- Lỗi đang thấy trên UI (`Mỗi nguyên nhân cần tối thiểu 5 dòng`) xuất phát từ **MVC** trước khi gọi FastAPI.

# 4. SQL kiểm tra thực tế trong DB

Ghi chú chạy query:
- Đã chạy trực tiếp SQL Server instance `LAPTOP-SI5JBDIU\SQLEXPRESS01`, DB `QuanLyDuAnAI`.
- Ban đầu môi trường sandbox không kết nối được SQL (SSPI/encryption), sau đó chạy ngoài sandbox để lấy số liệu thật.

## 4.1 Tổng quan AI_DATASET

```sql
SELECT
    COUNT(*) AS TongSoDong,
    SUM(CASE WHEN LaDuAnTre = 1 THEN 1 ELSE 0 END) AS SoDongTre,
    SUM(CASE WHEN LaDuAnTre = 0 THEN 1 ELSE 0 END) AS SoDongKhongTre,
    SUM(CASE WHEN LaDuAnTre IS NULL THEN 1 ELSE 0 END) AS SoDongChuaXacDinh,
    SUM(CASE WHEN MaDMNguyenNhan IS NOT NULL THEN 1 ELSE 0 END) AS SoDongCoNhan,
    SUM(CASE WHEN LaDuAnTre = 1 AND MaDMNguyenNhan IS NOT NULL THEN 1 ELSE 0 END) AS SoDongTreCoNhan
FROM dbo.AI_DATASET;
```

Kết quả:

| TongSoDong | SoDongTre | SoDongKhongTre | SoDongChuaXacDinh | SoDongCoNhan | SoDongTreCoNhan |
|---:|---:|---:|---:|---:|---:|
| 312 | 230 | 76 | 6 | 230 | 230 |

## 4.2 Dòng đủ 10 feature

```sql
SELECT
    COUNT(*) AS SoDongDu10Feature
FROM dbo.AI_DATASET
WHERE SoNhanVienDuAn IS NOT NULL
  AND TongSoCongViec IS NOT NULL
  AND SoCongViecTre IS NOT NULL
  AND TyLeCongViecTre IS NOT NULL
  AND ChiPhiDuKien IS NOT NULL
  AND ChiPhiThucTe IS NOT NULL
  AND ChenhLechChiPhi IS NOT NULL
  AND SoLanThayDoiNhanSu IS NOT NULL
  AND SoLanThayDoiQuanLy IS NOT NULL
  AND SoNgayTreTienDo IS NOT NULL;
```

Kết quả:

| SoDongDu10Feature |
|---:|
| 312 |

## 4.3 Dòng train hợp lệ theo AI_DATASET

```sql
SELECT
    dm.MaDMNguyenNhan,
    dm.TenNguyenNhan,
    COUNT(ds.MaData) AS SoDongTrainTheoDataset
FROM dbo.AI_DATASET ds
JOIN dbo.DM_NGUYEN_NHAN dm
    ON dm.MaDMNguyenNhan = ds.MaDMNguyenNhan
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
GROUP BY dm.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY SoDongTrainTheoDataset ASC, dm.MaDMNguyenNhan;
```

Kết quả:

| MaDMNguyenNhan | TenNguyenNhan | SoDongTrainTheoDataset |
|---:|---|---:|
| 7 | Thiếu dữ liệu hoặc tài liệu | 3 |
| 10 | Khác | 3 |
| 1 | Thiếu nhân sự | 28 |
| 2 | Thay đổi yêu cầu liên tục | 28 |
| 3 | Chậm phê duyệt | 28 |
| 4 | Vượt ngân sách | 28 |
| 5 | Rủi ro kỹ thuật | 28 |
| 6 | Công việc phụ thuộc bị chậm | 28 |
| 8 | Ước lượng thời gian chưa chính xác | 28 |
| 9 | Tiến độ cập nhật không đầy đủ | 28 |

## 4.4 Tất cả nguyên nhân, kể cả nguyên nhân 0 dòng

```sql
SELECT
    dm.MaDMNguyenNhan,
    dm.TenNguyenNhan,
    COUNT(ds.MaData) AS SoDongTrain
FROM dbo.DM_NGUYEN_NHAN dm
LEFT JOIN dbo.AI_DATASET ds
    ON ds.MaDMNguyenNhan = dm.MaDMNguyenNhan
   AND ds.LaDuAnTre = 1
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
GROUP BY dm.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY SoDongTrain ASC, dm.MaDMNguyenNhan;
```

Kết quả (không có lớp 0 dòng):

| MaDMNguyenNhan | TenNguyenNhan | SoDongTrain |
|---:|---|---:|
| 7 | Thiếu dữ liệu hoặc tài liệu | 3 |
| 10 | Khác | 3 |
| 1 | Thiếu nhân sự | 28 |
| 2 | Thay đổi yêu cầu liên tục | 28 |
| 3 | Chậm phê duyệt | 28 |
| 4 | Vượt ngân sách | 28 |
| 5 | Rủi ro kỹ thuật | 28 |
| 6 | Công việc phụ thuộc bị chậm | 28 |
| 8 | Ước lượng thời gian chưa chính xác | 28 |
| 9 | Tiến độ cập nhật không đầy đủ | 28 |

## 4.5 Kiểm tra nguyên nhân dưới 5 dòng

```sql
SELECT *
FROM
(
    SELECT
        dm.MaDMNguyenNhan,
        dm.TenNguyenNhan,
        COUNT(ds.MaData) AS SoDongTrain
    FROM dbo.DM_NGUYEN_NHAN dm
    LEFT JOIN dbo.AI_DATASET ds
        ON ds.MaDMNguyenNhan = dm.MaDMNguyenNhan
       AND ds.LaDuAnTre = 1
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
    GROUP BY dm.MaDMNguyenNhan, dm.TenNguyenNhan
) x
WHERE x.SoDongTrain < 5
ORDER BY x.SoDongTrain ASC, x.MaDMNguyenNhan;
```

Kết quả:

| MaDMNguyenNhan | TenNguyenNhan | SoDongTrain |
|---:|---|---:|
| 7 | Thiếu dữ liệu hoặc tài liệu | 3 |
| 10 | Khác | 3 |

## 4.6 Nếu code train dùng latest snapshot theo dự án

```sql
;WITH latest AS
(
    SELECT
        ds.*,
        ROW_NUMBER() OVER (
            PARTITION BY ds.MaDuAn
            ORDER BY ds.NgayTongHop DESC, ds.MaData DESC
        ) AS rn
    FROM dbo.AI_DATASET ds
)
SELECT
    dm.MaDMNguyenNhan,
    dm.TenNguyenNhan,
    COUNT(*) AS SoDongTrainLatest
FROM latest ds
JOIN dbo.DM_NGUYEN_NHAN dm
    ON dm.MaDMNguyenNhan = ds.MaDMNguyenNhan
WHERE ds.rn = 1
  AND ds.LaDuAnTre = 1
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
GROUP BY dm.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY SoDongTrainLatest ASC, dm.MaDMNguyenNhan;
```

Kết quả: giống 4.3 (7 và 10 vẫn chỉ 3 dòng).

## 4.7 Nếu code train join AI_NGUYEN_NHAN

```sql
SELECT
    dm.MaDMNguyenNhan,
    dm.TenNguyenNhan,
    COUNT(*) AS SoDongTrainTheoXacNhan
FROM dbo.AI_DATASET ds
JOIN dbo.AI_NGUYEN_NHAN ann
    ON ann.MaDuAn = ds.MaDuAn
   AND ann.MaDMNguyenNhan = ds.MaDMNguyenNhan
   AND ISNULL(ann.IsDeleted, 0) = 0
JOIN dbo.DM_NGUYEN_NHAN dm
    ON dm.MaDMNguyenNhan = ann.MaDMNguyenNhan
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
GROUP BY dm.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY SoDongTrainTheoXacNhan ASC, dm.MaDMNguyenNhan;
```

Kết quả: giống 4.3 (7 và 10 vẫn chỉ 3 dòng).

## 4.8 Kiểm tra seed expanded

```sql
SELECT
    COUNT(*) AS TongDongExpanded,
    SUM(CASE WHEN LaDuAnTre = 1 THEN 1 ELSE 0 END) AS DongTreExpanded,
    SUM(CASE WHEN LaDuAnTre = 0 THEN 1 ELSE 0 END) AS DongKhongTreExpanded,
    SUM(CASE WHEN LaDuAnTre = 1 AND MaDMNguyenNhan IS NOT NULL THEN 1 ELSE 0 END) AS DongTrainExpanded
FROM dbo.AI_DATASET
WHERE GhiChuDataset LIKE N'SEED_AI_REASON_EXPANDED%';
```

Kết quả:

| TongDongExpanded | DongTreExpanded | DongKhongTreExpanded | DongTrainExpanded |
|---:|---:|---:|---:|
| 260 | 200 | 60 | 200 |

## 4.9 Phân bố expanded theo nguyên nhân

```sql
SELECT
    dm.MaDMNguyenNhan,
    dm.TenNguyenNhan,
    COUNT(ds.MaData) AS SoDongExpanded
FROM dbo.DM_NGUYEN_NHAN dm
LEFT JOIN dbo.AI_DATASET ds
    ON ds.MaDMNguyenNhan = dm.MaDMNguyenNhan
   AND ds.GhiChuDataset LIKE N'SEED_AI_REASON_EXPANDED%'
   AND ds.LaDuAnTre = 1
GROUP BY dm.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY SoDongExpanded ASC, dm.MaDMNguyenNhan;
```

Kết quả:

| MaDMNguyenNhan | TenNguyenNhan | SoDongExpanded |
|---:|---|---:|
| 7 | Thiếu dữ liệu hoặc tài liệu | 0 |
| 10 | Khác | 0 |
| 1 | Thiếu nhân sự | 25 |
| 2 | Thay đổi yêu cầu liên tục | 25 |
| 3 | Chậm phê duyệt | 25 |
| 4 | Vượt ngân sách | 25 |
| 5 | Rủi ro kỹ thuật | 25 |
| 6 | Công việc phụ thuộc bị chậm | 25 |
| 8 | Ước lượng thời gian chưa chính xác | 25 |
| 9 | Tiến độ cập nhật không đầy đủ | 25 |

# 5. So sánh SQL seed và điều kiện train code

## 5.1 seed-demo-data-ai-expanded.sql
- Tạo **260** dòng `AI_DATASET` (`@SeedDatasetRows <> 260` sẽ throw).
- Trong đó **200** dòng delay (`@DelayProjects <> 200` sẽ throw), **60** on-time (`@OnTimeProjects <> 60` sẽ throw).
- Tạo **200** dòng `AI_NGUYEN_NHAN` (`@SeedReasonRows <> 200` sẽ throw).
- Với delay của expanded: có `MaDMNguyenNhan` đầy đủ (query 4.8: `DongTrainExpanded=200`).
- Expanded chỉ map **8 nguyên nhân** (1,2,3,4,5,6,8,9), không dùng 7 và 10.

## 5.2 seed-demo-data-simple.sql
- Có danh mục required **10 nguyên nhân** (bao gồm `Thiếu dữ liệu hoặc tài liệu` và `Khác`).
- `@ProjectPlan` có 48 dự án: **30 Tre**, **15 DungHan**, **3 Predict**.
- Theo logic insert AI_DATASET: 30 Tre có `MaDMNguyenNhan`, 15 DungHan không có nhãn, 3 Predict `LaDuAnTre = NULL`.
- `AI_NGUYEN_NHAN` seed theo tag `SEED_AI_NGUYEN_NHAN_CONFIRMED_DVL11`: thực tế DB hiện có **30** dòng.

## 5.3 Code train có bắt tất cả 10 nguyên nhân đều >=5 không
- **Không**. Code chỉ xét các lớp xuất hiện trong dataset train hợp lệ.

## 5.4 Nếu seed chỉ tạo 8 nguyên nhân nhưng DM có 10 thì có gây lỗi không
- **Không tự động gây lỗi**, nếu 2 nguyên nhân còn lại thực sự không xuất hiện trong dataset train.
- Nhưng DB hiện tại đang **trộn** expanded + simple:
  - Expanded: 8 nguyên nhân, mỗi nguyên nhân 25 dòng.
  - Simple: thêm 10 nguyên nhân, mỗi nguyên nhân 3 dòng delay.
  - Hệ quả gộp: nguyên nhân 7 và 10 tồn tại nhưng chỉ có 3 dòng -> fail điều kiện >=5.

# 6. Kết luận nguyên nhân không train được

Kết luận chính (xác suất cao nhất, đã có số liệu DB chứng minh):
- **Thiếu dòng ở một số nguyên nhân thật**: Có, nguyên nhân 7 và 10 chỉ 3 dòng.
- **Code đang kiểm tra cả nguyên nhân 0 dòng trong DM_NGUYEN_NHAN**: Không.
- **Query UI và query train không giống nhau**: Có khác nhẹ (UI dataset page dùng `KiemTraChatLuongDatasetAsync`, train page dùng `KiemTraChatLuongDatasetNguyenNhanAsync`), nhưng với DB hiện tại kết quả fail giống nhau.
- **Code train lấy latest snapshot làm mất dữ liệu**: Không (không dùng latest per project trong train query).
- **Code train join AI_NGUYEN_NHAN làm mất dòng**: Không.
- **Seed chỉ tạo 8/10 nguyên nhân nên 2 nguyên nhân còn lại 0 dòng**: Đúng với expanded riêng lẻ, nhưng DB hiện tại có thêm simple nên 2 nguyên nhân đó không phải 0 mà là 3 dòng.
- **Feature NULL làm mất dòng**: Không (312/312 đủ 10 feature).
- **Có dữ liệu cũ làm nhiễu thống kê**: Có (4 dòng `OTHER_OR_NULL`), nhưng không phải nguyên nhân chính của lỗi min 5.

# 7. Đề xuất sửa

Không sửa code ngay, chỉ đề xuất:
- Nếu muốn giữ rule hiện tại (`>=5` mỗi lớp xuất hiện), cần **sửa seed/data** để nguyên nhân 7 và 10 có ít nhất 5 dòng (thêm >=2 dòng mỗi nguyên nhân).
- Nếu muốn train theo tập lớp thực sự có ý nghĩa nghiệp vụ trong từng đợt, có thể **sửa validation** để:
  - Chỉ kiểm tra các class đủ điều kiện business và được chọn train trong đợt đó.
  - Hoặc cho phép exclude class hiếm qua config trước train.
- Nếu muốn train đủ 10 nguyên nhân: seed phải tạo >=5 dòng cho cả 10.
- Nếu muốn chỉ train 8 nguyên nhân: validation không nên fail vì 2 nguyên nhân không xuất hiện.
- Với 2 nguyên nhân `Thiếu dữ liệu hoặc tài liệu` và `Khác`:
  - Nên quyết định rõ: giữ để train (thì phải có dữ liệu đủ), hay loại khỏi train-set (mapping sang rule/manual, không dùng làm class model).

# 8. Báo cáo cuối

- File đã tạo/cập nhật: `docs/ai-train-error-diagnosis.md`
- Đã đọc source bắt buộc:
  - `quanlyduan.sql`
  - `seed-demo-data-simple.sql`
  - `seed-demo-data-ai-expanded.sql`
  - `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs`
  - `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiService.cs`
  - `QuanLyDuAn/QuanLyDuAn/Controllers/AiController.cs`
  - ViewModel AI train/dataset (`AiTrainPageViewModel.cs`, `AiDatasetManagementViewModels.cs`, `AiDatasetApiViewModels.cs`, `AiReasonTrainDatasetItemViewModel.cs`, `AiDatasetPageViewModel.cs`)
  - FastAPI: `schemas.py`, `model_service.py`, `validation_service.py`, `constants.py` (đọc thêm `config.py`, `ml/decision_tree_model.py`, `ml/feature_builder.py` để xác định stratify/filter)
  - `.env`, `.env.example`
- Query đã chạy:
  - Đầy đủ nhóm 4.1 -> 4.9
  - Query bổ sung phân tích theo seed tag để xác nhận nguồn gây lệch lớp
- Nguyên nhân nghi ngờ chính khiến không train được:
  - Dữ liệu train hiện có 2 lớp (MaDMNguyenNhan 7 và 10) chỉ 3 dòng, thấp hơn ngưỡng 5.
  - Gốc dữ liệu là trộn `SEED_AI_REASON_EXPANDED` (8 lớp x 25) với `SEED_AI_DATASET_REASON_ONLY_DVL11` (10 lớp x 3), làm min class = 3.
- Trạng thái bàn giao:
  - Nên gửi file `.md` này cho AI/phân tích tiếp theo để quyết định hướng xử lý (sửa seed hoặc sửa policy validation).
