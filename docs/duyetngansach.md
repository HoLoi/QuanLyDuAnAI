# Phân tích chức năng Duyệt đề xuất ngân sách dự án (AS-IS)

Tài liệu này mô tả luồng nghiệp vụ **duyệt / từ chối đề xuất ngân sách** dựa trên source code và cấu trúc database thực tế của dự án. Phạm vi tập trung vào nhánh **Từ chối đề xuất ngân sách**, đặc biệt việc lý do từ chối có được lưu hay không và ở đâu.

> **Phạm vi lần phân tích này:** chỉ đọc source và ghi tài liệu. Không sửa code, không tạo migration, không thay đổi database.

---

## 10.1 Phạm vi source đã đọc

### Controller

| File | Nội dung đã đọc |
|------|-----------------|
| `QuanLyDuAn/QuanLyDuAn/Controllers/DuyetDeXuatNganSachController.cs` | `Index`, `Duyet`, `TuChoi` |
| `QuanLyDuAn/QuanLyDuAn/Controllers/DeXuatNganSachController.cs` | `Index`, `TaoDeXuat`, `HuyDeXuat` |

### Service

| File | Nội dung đã đọc |
|------|-----------------|
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDuyetDeXuatNganSachService.cs` | Interface duyệt |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuyetDeXuatNganSachService.cs` | `GetPageAsync`, `ApproveAsync`, `RejectAsync` |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDeXuatNganSachService.cs` | Interface đề xuất |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DeXuatNganSachService.cs` | `GetPageAsync`, `CreateAsync`, `CancelAsync` |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NganSachService.cs` | `GetPageAsync` (xem có đọc nhật ký không) |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuAnService.cs` | Đoạn đọc `NhatKyQuanLyDuAn` cho hoạt động dự án |

### Entity

| File |
|------|
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/DeXuatNganSach.cs` |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/NganSach.cs` |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/NhatKyNganSach.cs` |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/NhatKyQuanLyDuAn.cs` |
| `QuanLyDuAn/QuanLyDuAn/Models/Entities/DanhGiaDuAn.cs` (đối chiếu pattern `LyDoTuChoi`) |

### ViewModel

| File |
|------|
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuyetDeXuatNganSach/DuyetDeXuatNganSachPageViewModel.cs` |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DuyetDeXuatNganSach/DuyetDeXuatNganSachItemViewModel.cs` |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DeXuatNganSach/DeXuatNganSachPageViewModel.cs` |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DeXuatNganSach/DeXuatNganSachItemViewModel.cs` |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DeXuatNganSach/DeXuatNganSachCreateViewModel.cs` |

### View / JavaScript

| File |
|------|
| `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatNganSach/Index.cshtml` |
| `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatNganSach/_Filter.cshtml` |
| `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatNganSach/_Table.cshtml` |
| `QuanLyDuAn/QuanLyDuAn/Views/DeXuatNganSach/Index.cshtml` |
| `QuanLyDuAn/QuanLyDuAn/Views/DeXuatNganSach/_Table.cshtml` |
| `QuanLyDuAn/QuanLyDuAn/Views/DeXuatNganSach/_Form.cshtml` |
| `QuanLyDuAn/QuanLyDuAn/Views/NganSach/Index.cshtml` |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/js/approval/index.js` |

### Database / cấu hình

| File |
|------|
| `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs` (cấu hình `DeXuatNganSach`, `NganSach`, `NhatKyNganSach`, `NhatKyQuanLyDuAn`) |
| `QuanLyDuAn/QuanLyDuAn/Migrations/20260527125053_Init.cs` |
| `QuanLyDuAn/QuanLyDuAn/Migrations/20260527125053_Init.Designer.cs` |
| `QuanLyDuAn/QuanLyDuAn/Migrations/QuanLyDuAnDbContextModelSnapshot.cs` |
| `quanlyduan.sql` |

### Constants / tham chiếu chéo

| File |
|------|
| `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs` |
| `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs` |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DyetDeXuatCongViecService.cs` (đối chiếu pattern từ chối module khác) |

---

## 10.2 Cấu trúc nghiệp vụ duyệt ngân sách

### Tạo đề xuất ngân sách

- **Màn hình:** `DeXuatNganSach/Index` (bắt buộc có `locMaDuAn`).
- **Quyền:** `DeXuatNganSach.Xem` (xem), `DeXuatNganSach.Them` (tạo/hủy).
- **Ai được đề xuất:** Trưởng team (nếu dự án có team) hoặc nhân viên thuộc dự án (nếu chưa có team). **Quản lý dự án không được tạo đề xuất.**
- **Service:** `DeXuatNganSachService.CreateAsync`.
- **Lưu:** bản ghi mới `DE_XUAT_NGAN_SACH` với `TrangThaiDeXuat = ChoDuyet`, `LyDoDeXuat` (lý do đề xuất — khác với lý do từ chối).
- **Nhật ký:** ghi `NHAT_KY_NGAN_SACH` (nếu đã có ngân sách hiện hành) và `NHAT_KY_QUAN_LY_DU_AN`.

### Xem danh sách chờ duyệt

- **Màn hình:** `DuyetDeXuatNganSach/Index`.
- **Quyền:** `DuyetNganSach.Xem`.
- **Phạm vi dữ liệu:** chỉ đề xuất thuộc dự án mà người dùng hiện tại là **quản lý** (`DuAn.MaNguoiDung == currentUserId`).
- **Lọc:** `locMaDuAn`, `locTrangThai` (`ChoDuyet`, `DaDuyet`, `TuChoi`, `DaHuy`).

### Xem chi tiết

- Không có action `Details` riêng. Chi tiết hiển thị **inline** trong `_Table.cshtml` (nút "Xem chi tiết" toggle hàng mở rộng).

### Duyệt đề xuất

- **Action:** `POST DuyetDeXuatNganSach/Duyet`.
- **Quyền:** `DuyetNganSach.Duyet`.
- **Service:** `DuyetDeXuatNganSachService.ApproveAsync`.
- **Transaction:** có (`IsolationLevel.Serializable`).
- **Version hóa ngân sách:** deactivate ngân sách `IsActive = true` cũ → tạo `NGAN_SACH` mới `Version + 1`, `IsActive = true`.
- **Cập nhật đề xuất:** `TrangThaiDeXuat = DaDuyet`, `MaNguoiDungDuyet`, `NgayDuyet`.
- **Nhật ký:** `NHAT_KY_NGAN_SACH` + `NHAT_KY_QUAN_LY_DU_AN`.

### Từ chối đề xuất

- **Action:** `POST DuyetDeXuatNganSach/TuChoi`.
- **Tham số lý do:** `string? lyDo` (tham số rời, không qua ViewModel).
- **Service:** `DuyetDeXuatNganSachService.RejectAsync`.
- **Không tạo ngân sách mới**, không đổi ngân sách đang active.
- **Cập nhật đề xuất:** `TrangThaiDeXuat = TuChoi`, `MaNguoiDungDuyet`, `NgayDuyet`.
- **Lý do từ chối:** không ghi vào `DE_XUAT_NGAN_SACH`; nhúng vào chuỗi nhật ký (xem mục 10.4).

### Hủy đề xuất

- **Action:** `POST DeXuatNganSach/HuyDeXuat`.
- **Quyền:** `DeXuatNganSach.Them`.
- **Điều kiện:** chỉ người tạo, trạng thái `ChoDuyet`.
- **Service:** `DeXuatNganSachService.CancelAsync` → `TrangThaiDeXuat = DaHuy`.

### Nhật ký ngân sách

- Bảng `NHAT_KY_NGAN_SACH`: ghi khi tạo / duyệt / từ chối / hủy đề xuất (từ chối chỉ khi `MaNganSachCu` có giá trị).
- Bảng `NHAT_KY_QUAN_LY_DU_AN`: luôn ghi khi tạo / duyệt / từ chối / hủy đề xuất.
- **Không có cột FK `MaDeXuatNS`** trên bảng nhật ký; mã đề xuất chỉ xuất hiện trong nội dung text `HanhDongNKNS` / `NkHanhDongQLDA`.

---

## 10.3 Luồng duyệt AS-IS (thành công)

```
POST DuyetDeXuatNganSach/Duyet(maDeXuatNs)
  → DuyetDeXuatNganSachController.Duyet
  → DuyetDeXuatNganSachService.ApproveAsync
  → BeginTransaction (Serializable)
  → Đọc DE_XUAT_NGAN_SACH (IsDeleted != true)
  → Kiểm tra TrangThaiDeXuat == ChoDuyet
  → EnsureIsProjectManagerAsync (DuAn.MaNguoiDung == currentUserId)
  → Kiểm tra dự án chưa hoàn thành/đóng
  → Kiểm tra NganSachDeXuat >= tổng chi phí đã dùng
  → Deactivate NGAN_SACH IsActive=true (TrangThaiNganSach = DaThayThe)
  → Insert NGAN_SACH mới (Version+1, IsActive=true, TrangThaiNganSach=DaDuyet)
  → Update DE_XUAT_NGAN_SACH: TrangThaiDeXuat=DaDuyet, MaNguoiDungDuyet, NgayDuyet
  → SaveChangesAsync (lấy MaNganSach mới)
  → Insert NHAT_KY_NGAN_SACH
  → Insert NHAT_KY_QUAN_LY_DU_AN
  → SaveChangesAsync
  → Commit transaction
```

### Bảng và trường được cập nhật khi duyệt

| Bảng | Trường | Giá trị |
|------|--------|---------|
| `NGAN_SACH` (cũ) | `IsActive` | `false` |
| `NGAN_SACH` (cũ) | `TrangThaiNganSach` | `DaThayThe` |
| `NGAN_SACH` (cũ) | `NgayCapNhatNganSach` | `DateTime.Now` |
| `NGAN_SACH` (mới) | toàn bộ bản ghi mới | từ đề xuất |
| `DE_XUAT_NGAN_SACH` | `TrangThaiDeXuat` | `DaDuyet` |
| `DE_XUAT_NGAN_SACH` | `MaNguoiDungDuyet` | người duyệt hiện tại |
| `DE_XUAT_NGAN_SACH` | `NgayDuyet` | `DateTime.Now` |
| `NHAT_KY_NGAN_SACH` | `HanhDongNKNS` | `"Duyệt đề xuất ngân sách #{MaDeXuatNS}"` |
| `NHAT_KY_QUAN_LY_DU_AN` | `NkHanhDongQLDA` | `"Duyệt đề xuất ngân sách #{MaDeXuatNS}"` |

---

## 10.4 Luồng từ chối AS-IS

### Kết luận phân loại

**Trường hợp 3 kết hợp 4:** Lý do từ chối **không** được lưu trong `DE_XUAT_NGAN_SACH`. Lý do **được nhúng vào nội dung nhật ký** (audit text), cụ thể:

1. **Luôn** ghi vào `NHAT_KY_QUAN_LY_DU_AN.NkHanhDongQLDA` (nếu có nhập lý do).
2. **Chỉ khi** `MaNganSachCu` có giá trị mới ghi thêm vào `NHAT_KY_NGAN_SACH.HanhDongNKNS`.

Ngoài ra có **lỗi triển khai giao diện**: nút "Từ chối" nhanh trên hàng bảng luôn gửi `lyDo=""`, nên lý do người dùng nhập ở form chi tiết **không được dùng** nếu họ bấm nút sai form.

### Luồng chi tiết

```
Người dùng nhập lý do (form chi tiết, name="lyDo")
  → POST /DuyetDeXuatNganSach/TuChoi
  → DuyetDeXuatNganSachController.TuChoi(maDeXuatNs, lyDo, ...)
      • Kiểm tra permission DuyetNganSach.Duyet
      • Không kiểm tra ModelState
      • Không validate bắt buộc lyDo
      • Gọi _service.RejectAsync(maDeXuatNs, lyDo)
  → DuyetDeXuatNganSachService.RejectAsync
      • Không có transaction
      • Kiểm tra TrangThaiDeXuat == ChoDuyet
      • EnsureIsProjectManagerAsync
      • deXuat.TrangThaiDeXuat = TuChoi
      • deXuat.MaNguoiDungDuyet = currentUserId
      • deXuat.NgayDuyet = DateTime.Now
      • Nếu MaNganSachCu.HasValue:
          HanhDongNKNS = "Từ chối đề xuất ngân sách #{id}" hoặc "... Lý do: {lyDo.Trim()}"
      • NkHanhDongQLDA = tương tự (luôn ghi)
      • SaveChangesAsync
  → Redirect Index + TempData Success (không chứa lý do)
```

### Ô lý do từ chối bind vào đâu?

| Tầng | Thuộc tính / tham số | Ghi chú |
|------|----------------------|---------|
| View (form chi tiết) | `name="lyDo"` | Input text, không có ViewModel |
| View (nút nhanh) | `name="lyDo"` hidden `value=""` | Luôn rỗng |
| Controller | `string? lyDo` | Tham số action, không phải ViewModel property |
| Service | `string? lyDo` | Dùng để ghép chuỗi nhật ký |
| Entity `DeXuatNganSach` | **Không có** | Không map lý do từ chối |
| ViewModel item | **Không có** | `DuyetDeXuatNganSachItemViewModel` không có `LyDoTuChoi` |

**Lưu ý:** Cột `LyDoDeXuat` trên `DE_XUAT_NGAN_SACH` là **lý do đề xuất** (người đề xuất nhập khi tạo), **không** phải lý do từ chối.

### Validation và xử lý giá trị rỗng

| Kiểm tra | View | Controller | Service |
|----------|------|------------|---------|
| Bắt buộc nhập lý do | Không (`placeholder="... (nếu có)"`) | Không | Không |
| Trim khoảng trắng | Không (browser gửi raw) | Không | Có (`lyDo.Trim()` khi không `IsNullOrWhiteSpace`) |
| Giới hạn độ dài | Không | Không | Không (rủi ro với cột `nvarchar(255)`) |
| Chỉ từ chối khi `ChoDuyet` | Chỉ hiện nút khi pending | Không kiểm tra trực tiếp | `IsPending` → `TrangThai.ChoDuyet` |
| Kiểm tra Manager đúng dự án | Không | Không | `EnsureIsProjectManagerAsync` |
| Transaction | — | — | **Không** (khác với `ApproveAsync`) |

### Sau khi tải lại trang

- Màn **DuyetDeXuatNganSach**: trường "Lý do từ chối / ghi chú" **hardcode** `"Không có"` — không đọc database hay nhật ký.
- Màn **DeXuatNganSach** (người đề xuất): không hiển thị lý do từ chối.
- Hoạt động dự án (`DuAn/Details`): có thể thấy dòng log dạng `"Từ chối đề xuất ngân sách #N. Lý do: ..."` trong preview `NhatKyQuanLyDuAn` — **chỉ nếu** người dùng có quyền xem nhật ký và lý do đã được gửi lên server.

---

## 10.5 Sơ đồ truy vết dữ liệu lý do từ chối

| Bước | File / method | Tên biến hoặc thuộc tính | Có giá trị lý do không | Hành động tiếp theo |
|------|---------------|--------------------------|:----------------------:|---------------------|
| View (form chi tiết) | `Views/DuyetDeXuatNganSach/_Table.cshtml` | `<input name="lyDo">` | Có (nếu người dùng nhập) | Submit POST `TuChoi` |
| View (nút nhanh) | `Views/DuyetDeXuatNganSach/_Table.cshtml` | `<input type="hidden" name="lyDo" value="">` | **Không** (luôn rỗng) | Submit POST `TuChoi` không kèm lý do |
| JavaScript | `wwwroot/js/approval/index.js` | — | Không xử lý `lyDo` | Chỉ `confirm()` và disable nút submit |
| Controller | `DuyetDeXuatNganSachController.TuChoi` | `string? lyDo` | Có (nếu form gửi) | `_service.RejectAsync(maDeXuatNs, lyDo)` |
| Service | `DuyetDeXuatNganSachService.RejectAsync` | `string? lyDo` | Có | Ghép vào `HanhDongNKNS` / `NkHanhDongQLDA`; **không** gán Entity |
| Entity | `DeXuatNganSach` | — | **Không có property** | Chỉ cập nhật trạng thái / người duyệt / ngày |
| Database `DE_XUAT_NGAN_SACH` | — | — | **Không lưu** | — |
| Database nhật ký | `NHAT_KY_QUAN_LY_DU_AN` | `NkHanhDongQLDA` | Có (nhúng text) | Luôn insert khi từ chối |
| Database nhật ký | `NHAT_KY_NGAN_SACH` | `HanhDongNKNS` | Có (nhúng text) | Chỉ khi `MaNganSachCu` có giá trị |
| View chi tiết sau từ chối | `Views/DuyetDeXuatNganSach/_Table.cshtml` | hardcode | **Không hiển thị** | Luôn `"Không có"` |

---

## Phân tích giao diện từ chối

### Hai form từ chối trên cùng một đề xuất

**Form 1 — Nút "Từ chối" trên hàng bảng (dòng 94–100 `_Table.cshtml`):**

```html
<form asp-action="TuChoi" method="post" class="js-confirm-submit" ...>
    <input type="hidden" name="maDeXuatNs" value="@item.MaDeXuatNS" />
    ...
    <input type="hidden" name="lyDo" value="" />
    <button type="submit">Từ chối</button>
</form>
```

**Form 2 — Form trong vùng chi tiết (dòng 164–170):**

```html
<form asp-action="TuChoi" method="post" class="... js-confirm-submit" ...>
    <input type="hidden" name="maDeXuatNs" value="@item.MaDeXuatNS" />
    ...
    <input type="text" name="lyDo" class="form-control form-control-sm" placeholder="Nhập lý do từ chối (nếu có)" />
    <button type="submit">Gửi từ chối kèm lý do</button>
</form>
```

| Tiêu chí | Giá trị thực tế |
|----------|-----------------|
| Route / action | `POST DuyetDeXuatNganSach/TuChoi` |
| Method | `POST` form HTML (không AJAX) |
| Anti-forgery token | **Không** có `[ValidateAntiForgeryToken]` trên action (khác với `DanhGiaDuAnController.TuChoi`) |
| Mã đề xuất | `name="maDeXuatNs"` → khớp tham số Controller |
| Lý do từ chối | `name="lyDo"` → khớp tham số Controller |
| Bind Controller | Khớp tên tham số (`lyDo` camelCase) |
| Client validation | Chỉ `window.confirm()` qua `js-confirm-submit` |
| Server validation lý do | Không |
| Sau từ chối thành công | Redirect + `TempData["Success"]`; **không** hiển thị lại lý do |

**Hành vi quan trọng:** Nút "Từ chối" nhanh **không đọc** ô nhập lý do trong form chi tiết (hai form độc lập). Đây là lỗi triển khai UX: người dùng có thể nhập lý do, bấm nút sai, và hệ thống vẫn từ chối thành công **không có lý do**.

---

## Phân tích Controller từ chối

**File:** `DuyetDeXuatNganSachController.cs`, action `TuChoi`.

| Tiêu chí | Kết quả |
|----------|---------|
| Nhận ViewModel hay tham số rời | Tham số rời: `int maDeXuatNs`, `string? lyDo`, filter params |
| Nhận lý do từ chối | **Có** — `string? lyDo` |
| Kiểm tra `ModelState` | **Không** |
| Permission | `Permissions.DuyetNganSach.Duyet` |
| Kiểm tra Manager dự án | Ủy quyền cho Service (`EnsureIsProjectManagerAsync`) |
| Gọi service | `_service.RejectAsync(maDeXuatNs, lyDo)` — **có truyền lý do** |
| Lỗi service | `TempData["Error"] = ex.Message`; lý do **không** được giữ lại |
| TempData | Chỉ thông báo chung, không lưu lý do |
| Nhầm lẫn GET/POST | GET `Index`, POST `TuChoi` — tách rõ |

---

## Phân tích Service từ chối

**File:** `DuyetDeXuatNganSachService.RejectAsync(int maDeXuatNs, string? lyDo)`.

| Tiêu chí | Kết quả |
|----------|---------|
| Nhận `lyDo` | **Có** |
| Xử lý `lyDo` | Ghép vào chuỗi nhật ký; trim khi không rỗng |
| Cập nhật `TrangThaiDeXuat` | `TuChoi` |
| Cập nhật `NgayDuyet` | `DateTime.Now` |
| Cập nhật người duyệt | `MaNguoiDungDuyet = currentUserId` |
| Cập nhật lý do trên Entity | **Không** |
| Tạo ngân sách mới | **Không** |
| Thay đổi ngân sách active | **Không** |
| Transaction | **Không** |
| `SaveChangesAsync` | **Có** (một lần) |
| Từ chối lại đề xuất đã xử lý | **Chặn** — `IsPending` kiểm tra `ChoDuyet` |
| Trạng thái `TuChoi` nhưng mất lý do | **Có thể** — khi gửi `lyDo` rỗng hoặc dùng nút từ chối nhanh |

**Định dạng chuỗi nhật ký khi có lý do:**

```csharp
$"Từ chối đề xuất ngân sách #{deXuat.MaDeXuatNS}. Lý do: {lyDo.Trim()}"
```

---

## Bảng đối chiếu dữ liệu nghiệp vụ

| Dữ liệu nghiệp vụ | ViewModel | Entity property | Cột SQL | Được lưu không | Ghi chú |
|-------------------|-----------|-----------------|---------|:--------------:|---------|
| Mã đề xuất | `MaDeXuatNS` | `MaDeXuatNS` | `MaDeXuatNS` | Có (sẵn có) | PK |
| Trạng thái | `TrangThaiDeXuat` | `TrangThaiDeXuat` | `TrangThaiDeXuat` | Có | Cập nhật `TuChoi` |
| Người duyệt | `NguoiDungDuyet` (join) | `MaNguoiDungDuyet` | `MaNguoiDungDuyet` | Có | FK `NGUOI_DUNG` |
| Ngày duyệt | `NgayDuyet` | `NgayDuyet` | `NgayDuyet` | Có | `DateTime.Now` |
| Lý do từ chối | **Không có** | **Không có** | **Không có cột riêng** | **Không** trên đề xuất | Chỉ nhúng nhật ký |
| Ghi chú đề xuất | `LyDoDeXuat` | `LyDoDeXuat` | `LyDoDeXuat` | Có (khi tạo) | Không bị ghi đè khi từ chối |
| Nội dung nhật ký | — | `HanhDongNKNS` / `NkHanhDongQLDA` | cùng tên | Có (text) | Không có FK `MaDeXuatNS` |

### Kiểm tra cột tương tự trên `DE_XUAT_NGAN_SACH`

| Cột tìm kiếm | Tồn tại trong SQL / Entity? |
|--------------|----------------------------|
| `LyDoTuChoi` | **Không** |
| `GhiChu` | **Không** |
| `NoiDung` | **Không** |
| `NhanXet` | **Không** |
| `LyDoDuyet` | **Không** |
| `LyDoDeXuat` | **Có** — lý do **đề xuất**, không phải từ chối |
| `MaNguoiDungDuyet` / `NguoiDuyet` | **Có** (`MaNguoiDungDuyet`) |
| `NgayDuyet` | **Có** |

---

## 10.6 Cấu trúc database hiện tại

### `DE_XUAT_NGAN_SACH`

| Cột | Kiểu | Liên quan duyệt/từ chối |
|-----|------|--------------------------|
| `MaDeXuatNS` | int IDENTITY PK | Mã đề xuất |
| `MaDuAn` | int NOT NULL | FK dự án |
| `MaNganSachCu` | int NULL | Ngân sách hiện hành lúc đề xuất |
| `NganSachCu` | decimal(18,2) NULL | Số tiền cũ |
| `NganSachDeXuat` | decimal(18,2) NULL | Số tiền đề xuất |
| `LyDoDeXuat` | nvarchar(max) NULL | **Lý do đề xuất** |
| `MaNguoiDungDeXuat` | int NOT NULL | Người đề xuất |
| `MaNguoiDungDuyet` | int NULL | Người duyệt/từ chối |
| `NgayDeXuat` | datetime2 NULL | Ngày tạo đề xuất |
| `NgayDuyet` | datetime2 NULL | Ngày duyệt/từ chối |
| `TrangThaiDeXuat` | nvarchar(50) NULL | `ChoDuyet` / `DaDuyet` / `TuChoi` / `DaHuy` |
| `IsDeleted`, `DeletedAt`, `DeletedBy` | soft delete | — |

**Không có cột lý do từ chối.**

### `NGAN_SACH`

| Cột | Kiểu | Ghi chú |
|-----|------|---------|
| `MaNganSach` | int PK | — |
| `MaNguoiDungDuyet` | int NOT NULL | Người duyệt phiên bản |
| `MaNguoiDungDeXuat` | int NOT NULL | Người đề xuất phiên bản |
| `MaDuAn` | int NOT NULL | FK dự án |
| `NganSach` | decimal(18,2) NULL | Entity map `SoTienNganSach` → cột `NganSach` |
| `Version` | int NULL | Version hóa khi duyệt |
| `IsActive` | bit NULL | Phiên bản hiệu lực |
| `MoTaNganSach` | nvarchar(255) NULL | Mô tả |
| `NgayCapNhatNganSach` | datetime2 NULL | — |
| `NgayDuyetNganSach` | datetime2 NULL | — |
| `TrangThaiNganSach` | nvarchar(50) NULL | — |
| `IsDeleted`, `DeletedAt`, `DeletedBy` | soft delete | — |

**Không có cột lý do từ chối.** Từ chối đề xuất **không** sửa bảng này.

### `NHAT_KY_NGAN_SACH`

| Cột | Kiểu | Ghi chú |
|-----|------|---------|
| `MaNhatKyNS` | int PK | — |
| `MaNganSach` | int NOT NULL | FK ngân sách (không FK đề xuất) |
| `MaDuAn` | int NOT NULL | FK dự án |
| `SoTienNKNS` | decimal(18,2) NULL | — |
| `NganSachTruoc` / `NganSachSau` | decimal(18,2) NULL | — |
| `NkNgayCapNhatNS` | datetime2 NULL | — |
| `NkTrangThaiNganSach` | nvarchar(50) NULL | `TuChoi` khi từ chối |
| `HanhDongNKNS` | **nvarchar(255) NULL** | **Chứa lý do nhúng text** |
| `ThoiGianNKNS` | datetime2 NULL | — |

**Không có `MaDeXuatNS`.** Max 255 ký tự — lý do dài có thể bị cắt khi lưu.

### `NHAT_KY_QUAN_LY_DU_AN`

| Cột | Kiểu | Ghi chú |
|-----|------|---------|
| `MaNhatKyQLDA` | int PK | — |
| `MaDuAn` | int NOT NULL | — |
| `MaNguoiDung` | int NOT NULL | Người thực hiện từ chối |
| `NkHanhDongQLDA` | **nvarchar(255) NULL** | **Chứa lý do nhúng text** |
| `NkThoiGianQLDA` | datetime2 NULL | — |
| `QLDATuNgay` / `QLDADenNgay` | datetime2 NULL | Không dùng khi từ chối ngân sách |

---

## 10.7 Đối chiếu Entity, Migration và SQL script

### `DE_XUAT_NGAN_SACH`

| Nguồn | Trạng thái |
|-------|------------|
| Entity `DeXuatNganSach.cs` | 14 property, không có lý do từ chối |
| Migration `20260527125053_Init` | Khớp Entity |
| Model snapshot | Khớp Entity |
| `quanlyduan.sql` | Khớp — **không thiếu cột** so với Entity |

**Kết luận:** SQL script **không cũ hơn** Entity/Migration về bảng đề xuất ngân sách. Việc không thấy cột lý do từ chối trong SQL là **đúng với thiết kế hiện tại**, không phải lệch đồng bộ.

### Migration

- Dự án chỉ có **một** migration: `20260527125053_Init`.
- Không có migration bổ sung cột `LyDoTuChoi` sau Init.

### Khác biệt có chủ đích (không phải lỗi đồng bộ)

| Entity | SQL | Ghi chú |
|--------|-----|---------|
| `NganSach.SoTienNganSach` | `NGAN_SACH.NganSach` | Map qua `HasColumnName("NganSach")` trong DbContext |

### Câu SQL kiểm tra runtime (chỉ đọc, không thay đổi)

```sql
-- Kiểm tra cột bảng đề xuất
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DE_XUAT_NGAN_SACH'
ORDER BY ORDINAL_POSITION;

-- Tìm lý do từ chối đã nhúng trong nhật ký quản lý dự án
SELECT MaNhatKyQLDA, MaDuAn, NkHanhDongQLDA, NkThoiGianQLDA
FROM NHAT_KY_QUAN_LY_DU_AN
WHERE NkHanhDongQLDA LIKE N'Từ chối đề xuất ngân sách #%'
ORDER BY NkThoiGianQLDA DESC;

-- Tìm trong nhật ký ngân sách (nếu có MaNganSachCu lúc từ chối)
SELECT MaNhatKyNS, MaDuAn, MaNganSach, HanhDongNKNS, ThoiGianNKNS
FROM NHAT_KY_NGAN_SACH
WHERE HanhDongNKNS LIKE N'Từ chối đề xuất ngân sách #%'
ORDER BY ThoiGianNKNS DESC;
```

---

## 10.7 Các lỗi hoặc rủi ro phát hiện

| # | Mức độ | File | Method / vị trí | Hành vi hiện tại | Hậu quả |
|---|--------|------|-----------------|------------------|---------|
| 1 | **Cao** | `Views/DuyetDeXuatNganSach/_Table.cshtml` | Form nút "Từ chối" nhanh | `lyDo` hidden luôn rỗng | Từ chối thành công nhưng **mất lý do** dù người dùng đã nhập ở form chi tiết |
| 2 | **Cao** | `Views/DuyetDeXuatNganSach/_Table.cshtml` | Dòng "Lý do từ chối / ghi chú" | Hardcode `"Không có"` | **Không xem lại** lý do trên màn duyệt dù DB có trong nhật ký |
| 3 | **Cao** | `DeXuatNganSach/_Table.cshtml` | Chi tiết đề xuất | Không hiển thị lý do từ chối | Người đề xuất không thấy lý do bị từ chối |
| 4 | **Trung bình** | `DuyetDeXuatNganSachService.RejectAsync` | Ghép `lyDo` vào nhật ký | Không validate độ dài | `HanhDongNKNS` / `NkHanhDongQLDA` max **255** — lý do dài có thể lỗi DB hoặc bị cắt |
| 5 | **Trung bình** | `DuyetDeXuatNganSachService.RejectAsync` | Nhánh `MaNganSachCu` | Không ghi `NHAT_KY_NGAN_SACH` khi đề xuất lần đầu (chưa có ngân sách) | Lý do chỉ còn ở `NHAT_KY_QUAN_LY_DU_AN`, không có bản ghi nhật ký ngân sách |
| 6 | **Trung bình** | `NHAT_KY_*` | Thiết kế schema | Không FK `MaDeXuatNS` | Khó truy vấn / báo cáo lý do theo từng đề xuất chính xác |
| 7 | **Trung bình** | `DuyetDeXuatNganSachController.TuChoi` | POST action | Không `[ValidateAntiForgeryToken]` | Rủi ro CSRF (cùng pattern với `Duyet` / `DeXuatNganSach`) |
| 8 | **Thấp** | Controller + Service | Từ chối | Lý do không bắt buộc (cả View lẫn backend) | Có thể từ chối không giải thích — đúng placeholder "nếu có" |
| 9 | **Thấp** | `RejectAsync` vs `ApproveAsync` | Transaction | Từ chối không bọc transaction | Ít rủi ro hơn duyệt (không tạo ngân sách), nhưng không atomic nếu mở rộng logic |
| 10 | **Thấp** | View hiển thị lý do | Razor | `@item.LyDoDeXuat` không encode đặc biệt | ASP.NET Core Razor mặc định HTML-encode; rủi ro XSS thấp nếu không dùng `Html.Raw` |

**Không phát hiện:** lý do từ chối ghi đè `LyDoDeXuat`; lý do chỉ lưu `TempData`.

---

## 10.8 Các phương án TO-BE có thể áp dụng

### Phương án A: Thêm cột riêng vào `DE_XUAT_NGAN_SACH`

Ví dụ: `LyDoTuChoi nvarchar(500) NULL`.

**Ưu điểm**

- Truy vấn trực tiếp theo đề xuất; hiển thị danh sách/chi tiết đơn giản.
- Nhất quán với module **Đánh giá dự án** (`LyDoTuChoiDanhGiaDA`) và **Đánh giá nhân viên** (`LyDoTuChoiDanhGiaNV`).
- Tách bạch `LyDoDeXuat` (đề xuất) và `LyDoTuChoi` (từ chối).

**Nhược điểm**

- Cần migration + cập nhật `quanlyduan.sql`.
- Dữ liệu cũ không có lý do (có thể backfill từ nhật ký nếu cần).

**File cần sửa:** Entity, DbContext, Migration, SQL script, Service `RejectAsync`, ViewModel item, View `_Table.cshtml` (cả duyệt và đề xuất).

**Migration:** Có. **Ảnh hưởng dữ liệu cũ:** Cột NULL cho bản ghi cũ.

---

### Phương án B: Dùng cột ghi chú sẵn có

**Không khuyến nghị.** Bảng `DE_XUAT_NGAN_SACH` chỉ có `LyDoDeXuat` — đây là lý do **đề xuất**, đang được dùng và hiển thị. Ghi đè sẽ **mất lý do đề xuất gốc** và không phân biệt được hai mục đích.

---

### Phương án C: Chỉ lưu vào nhật ký

**Hiện trạng gần với phương án này** nhưng chưa đủ cho nghiệp vụ chính.

**Ưu điểm:** Không đổi schema đề xuất.

**Nhược điểm**

- Không FK `MaDeXuatNS`; phải parse text `LIKE '%#123%'`.
- Giới hạn 255 ký tự.
- Giao diện không đọc nhật ký → người dùng tưởng lý do không lưu.
- Đề xuất ngân sách lần đầu thiếu bản ghi `NHAT_KY_NGAN_SACH`.

**Giao diện cần đọc từ:** `NHAT_KY_QUAN_LY_DU_AN` (và/hoặc `NHAT_KY_NGAN_SACH`) — cần logic parse hoặc query theo pattern.

---

### Phương án D: Cột riêng + ghi nhật ký (audit)

**Ưu điểm**

- Cột riêng phục vụ hiển thị nghiệp vụ và báo cáo.
- Nhật ký giữ lịch sử audit (có thể rút gọn message, không nhúng full lý do dài).
- Cùng transaction khi cập nhật (nên thêm transaction như `ApproveAsync`).

**Nhược điểm**

- Trùng dữ liệu nếu không quy ước rõ (cột = source of truth, log = tóm tắt).

**Đảm bảo transaction:** bọc cập nhật `DE_XUAT_NGAN_SACH` + insert nhật ký trong một transaction.

---

### Phương án đề xuất

**Phương án D** (cột `LyDoTuChoi` + nhật ký rút gọn) — **phù hợp nhất** với kiến trúc hiện tại vì:

1. Dự án đã có precedent cột `LyDoTuChoi*` trên bảng đánh giá.
2. Giao diện duyệt đã có placeholder field "Lý do từ chối" nhưng chưa nối dữ liệu.
3. Nhật ký vẫn hữu ích cho timeline dự án (`DuAn/Details`).

**Bổ sung bắt buộc kèm theo (không thuộc phương án DB):**

- Sửa UX: **một** form từ chối (bỏ nút nhanh gửi `lyDo` rỗng, hoặc bắt buộc mở chi tiết).
- Map hiển thị lý do trên `_Table.cshtml` cả hai phía duyệt và đề xuất.

---

## 10.9 File dự kiến cần sửa ở bước sau (theo phương án D)

| Nhóm | File |
|------|------|
| Entity | `Models/Entities/DeXuatNganSach.cs` |
| DbContext | `Data/QuanLyDuAnDbContext.cs` |
| Migration | Tạo migration mới (ví dụ `AddLyDoTuChoiDeXuatNganSach`) |
| SQL script | `quanlyduan.sql` |
| ViewModel | `ViewModels/DuyetDeXuatNganSach/DuyetDeXuatNganSachItemViewModel.cs` |
| ViewModel | `ViewModels/DeXuatNganSach/DeXuatNganSachItemViewModel.cs` |
| Service interface | `Services/Interfaces/IDuyetDeXuatNganSachService.cs` (nếu đổi chữ ký / thêm validate) |
| Service | `Services/Implementations/DuyetDeXuatNganSachService.cs` (`RejectAsync`, `GetPageAsync`) |
| Service | `Services/Implementations/DeXuatNganSachService.cs` (`GetPageAsync` map thêm field) |
| Controller | `Controllers/DuyetDeXuatNganSachController.cs` (validation, antiforgery tùy chuẩn dự án) |
| View | `Views/DuyetDeXuatNganSach/_Table.cshtml` |
| View | `Views/DeXuatNganSach/_Table.cshtml` |
| Tài liệu | `docs/duyetngansach.md`, `docs/motacsdl.md` (nếu cần cập nhật mô tả CSDL) |

---

## 10.10 Kết luận

| Câu hỏi | Trả lời |
|---------|---------|
| Ô lý do từ chối có hoạt động trên giao diện không? | **Có** — input `name="lyDo"` trong form chi tiết submit được. Nút "Từ chối" nhanh **không** gửi lý do. |
| Controller có nhận được không? | **Có** — tham số `string? lyDo`. |
| Service có nhận được không? | **Có** — `RejectAsync(maDeXuatNs, lyDo)`. |
| Lý do hiện được lưu ở đâu? | **Không** lưu trên `DE_XUAT_NGAN_SACH`. **Có** nhúng trong `NHAT_KY_QUAN_LY_DU_AN.NkHanhDongQLDA`; **có thể thêm** trong `NHAT_KY_NGAN_SACH.HanhDongNKNS` nếu `MaNganSachCu` có giá trị. |
| Nếu không lưu (trên đề xuất) thì bị mất ở bước nào? | Không bị mất ở Controller/Service — **thiết kế không gán Entity**. Mất trên UI do nút từ chối nhanh gửi `lyDo=""`. |
| File SQL thiếu cột hay chỉ cũ hơn Entity? | **Không thiếu, đồng bộ** với Entity/Migration. Không có cột lý do từ chối là **đúng thiết kế hiện tại**. |
| Có cần thay đổi database không? | **Không bắt buộc** để lưu lý do (đã lưu nhật ký). **Nên có** nếu muốn nghiệp vụ chính rõ ràng và hiển thị trực tiếp. |
| Có cần migration không? | Chỉ khi triển khai phương án A/D. |
| Có cần hiển thị lý do ở danh sách/chi tiết không? | **Có** — hiện UI cam kết field "Lý do từ chối" nhưng luôn hiển thị "Không có". |
| Phương án sửa phù hợp nhất? | **Phương án D** — thêm `LyDoTuChoi` + giữ nhật ký audit + sửa UX form từ chối. |

---

## Xác nhận phạm vi công việc lần này

- [x] Đã tạo `docs/duyetngansach.md`
- [x] Chưa sửa source code
- [x] Chưa tạo migration
- [x] Chưa thay đổi database
- [x] Tài liệu ghi bằng tiếng Việt UTF-8

---

## Trạng thái triển khai

### Ngày triển khai

- 2026-06-17

### Phương án đã chọn

Đã triển khai phương án bỏ lý do từ chối khỏi nhánh **Duyệt đề xuất ngân sách / Từ chối**. Lý do chọn phương án này là database hiện không có cột lưu lý do từ chối, yêu cầu nghiệp vụ mới không cần nhập hoặc lưu lý do, và việc giữ lý do trong text nhật ký gây hiểu nhầm rằng hệ thống có dữ liệu lý do riêng cho từng đề xuất.

### File đã sửa

- `QuanLyDuAn/QuanLyDuAn/Controllers/DuyetDeXuatNganSachController.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDuyetDeXuatNganSachService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuyetDeXuatNganSachService.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatNganSach/_Table.cshtml`
- `docs/duyetngansach.md`

Không sửa `wwwroot/js/approval/index.js` vì file này chỉ xử lý `confirm`, disable nút submit và toggle chi tiết; không có logic riêng cho `lyDo`, `reason` hoặc lý do từ chối.

### Thay đổi giao diện

- Đã bỏ ô nhập lý do từ chối.
- Đã bỏ hidden input `name="lyDo"`.
- Đã bỏ placeholder `Nhập lý do từ chối`.
- Đã bỏ nút `Gửi từ chối kèm lý do`.
- Đã bỏ form từ chối thứ hai trong vùng chi tiết.
- Đã bỏ dòng `Lý do từ chối / ghi chú`.
- Đã bỏ nội dung hardcode `Không có` dùng cho lý do từ chối.
- Mỗi đề xuất `ChoDuyet` hiện chỉ còn một form từ chối.
- Form từ chối chỉ gửi `maDeXuatNs`, `locMaDuAn`, `locTrangThai`, `pageNumber`, `pageSize`.
- Nút từ chối giữ tên `Từ chối`, dùng `POST`, giữ cơ chế xác nhận trước khi submit.
- Nội dung xác nhận mới: `Bạn có chắc chắn muốn từ chối đề xuất ngân sách này không?`

### Thay đổi Controller

- Đã bỏ tham số `string? lyDo` khỏi action `TuChoi`.
- Action vẫn giữ tên `TuChoi`, HTTP method `POST`, permission `DuyetNganSach.Duyet`, cách lấy lỗi qua `TempData["Error"]`, thông báo thành công qua `TempData["Success"]`, và redirect về `Index`.
- Action `TuChoi` hiện nhận thêm `pageNumber` và `pageSize` để giữ phân trang sau khi xử lý.
- Lời gọi service đổi từ `_service.RejectAsync(maDeXuatNs, lyDo)` thành `_service.RejectAsync(maDeXuatNs)`.

### Thay đổi Service interface

- Đã đổi chữ ký từ `Task RejectAsync(int maDeXuatNs, string? lyDo)` thành `Task RejectAsync(int maDeXuatNs)`.
- Không thay đổi các method duyệt, lấy danh sách hoặc nghiệp vụ khác.

### Thay đổi Service implementation

- Đã đổi chữ ký `RejectAsync` sang `RejectAsync(int maDeXuatNs)`.
- Giữ nguyên kiểm tra đề xuất tồn tại, trạng thái `ChoDuyet`, kiểm tra Manager đúng dự án, cập nhật `TrangThaiDeXuat = TuChoi`, `MaNguoiDungDuyet`, `NgayDuyet`, ghi nhật ký và `SaveChangesAsync`.
- Giữ nguyên rule chỉ ghi `NHAT_KY_NGAN_SACH` khi `MaNganSachCu` có giá trị.
- Đã bỏ toàn bộ xử lý `string? lyDo`, `string.IsNullOrWhiteSpace(lyDo)`, `lyDo.Trim()` và chuỗi `. Lý do: ...`.
- Không thay đổi `ApproveAsync`.

### Nội dung nhật ký sau khi sửa

Áp dụng thống nhất cho `NHAT_KY_NGAN_SACH.HanhDongNKNS` và `NHAT_KY_QUAN_LY_DU_AN.NkHanhDongQLDA`:

```csharp
$"Từ chối đề xuất ngân sách #{deXuat.MaDeXuatNS}"
```

Nhật ký không còn ghi thêm `Lý do: ...`.

### Kết quả build

Đã chạy:

```powershell
dotnet build QuanLyDuAn\QuanLyDuAn.sln
```

Kết quả: build thành công, `0 Error(s)`, `2 Warning(s)`.

Hai warning hiện có nằm ở `FileTienDoCongViecService.cs` về async method không có `await`, không phát sinh từ thay đổi nhánh từ chối đề xuất ngân sách.

### Kiểm thử và rà soát sau triển khai

- Đã kiểm tra source `_Table.cshtml`: đề xuất `ChoDuyet` và người dùng có `DuyetNganSach.Duyet` chỉ còn một nút `Từ chối`.
- Đã kiểm tra source `_Table.cshtml`: không còn ô nhập lý do từ chối.
- Đã kiểm tra source `_Table.cshtml`: không còn nút `Gửi từ chối kèm lý do`.
- Đã kiểm tra source `_Table.cshtml`: không còn dòng `Lý do từ chối / ghi chú: Không có`.
- Đã kiểm tra chữ ký đồng bộ giữa Controller, interface và service implementation.
- Đã kiểm tra không còn lời gọi `RejectAsync(maDeXuatNs, lyDo)` trong module duyệt đề xuất ngân sách.
- Đã kiểm tra nhánh service: Manager đúng dự án và đề xuất `ChoDuyet` sẽ được chuyển sang `TuChoi`, lưu `MaNguoiDungDuyet`, lưu `NgayDuyet`, không tạo `NGAN_SACH` mới và không thay đổi ngân sách active.
- Đã kiểm tra nhánh service: người không phải Manager đúng dự án bị chặn bởi `EnsureIsProjectManagerAsync`.
- Đã kiểm tra nhánh service: đề xuất không còn `ChoDuyet` bị chặn bởi `IsPending`, bao gồm `DaDuyet`, `TuChoi`, `DaHuy`.
- Đã kiểm tra redirect sau từ chối giữ `locMaDuAn`, `locTrangThai`, `pageNumber`, `pageSize`.

### Xác nhận phạm vi database

- Không sửa Entity.
- Không sửa `QuanLyDuAnDbContext`.
- Không tạo migration.
- Không sửa migration cũ.
- Không sửa model snapshot.
- Không sửa `quanlyduan.sql`.
- Không sửa bảng `DE_XUAT_NGAN_SACH`.
- Không sửa bảng `NGAN_SACH`.
- Không sửa bảng `NHAT_KY_NGAN_SACH`.
- Không sửa bảng `NHAT_KY_QUAN_LY_DU_AN`.
- Không thay đổi cấu trúc hoặc dữ liệu database.

### UTF-8 và lỗi font

- Các file đã sửa được giữ ở UTF-8.
- Đã rà các file đã sửa để không phát sinh các dấu hiệu mojibake đã nêu trong yêu cầu.
