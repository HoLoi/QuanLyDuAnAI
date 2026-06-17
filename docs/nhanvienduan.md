# Phân tích chức năng Nhân viên dự án (Thành viên dự án)

Tài liệu này mô tả trạng thái **AS-IS** của chức năng quản lý nhân viên/thành viên phụ trách dự án trong dự án ASP.NET Core MVC `QuanLyDuAn/QuanLyDuAn`, xác định vị trí lỗi runtime EF Core và đề xuất hướng sửa **chưa triển khai**.

> **Phạm vi tài liệu:** Chỉ phân tích source. Không sửa code, không migration, không thay đổi database/route/permission/UI.

---

## 1. Tổng quan chức năng Nhân viên dự án

### 1.1. Mục đích nghiệp vụ

Chức năng **Thành viên dự án** (permission namespace: `ThanhVienDuAn`, controller: `NhanVienDuAnController`, bảng: `NHAN_VIEN_DU_AN`) dùng để:

| Thao tác | Mô tả AS-IS |
| -------- | ----------- |
| Xem danh sách | Liệt kê nhân viên đang phụ trách một dự án, kèm vai trò, ngày tham gia, thông tin team phụ trách |
| Thêm nhân viên | Chọn trực tiếp các Employee hợp lệ chưa thuộc dự án |
| Cập nhật vai trò | Đổi `VaiTroTrongDuAn` giữa `Leader` (Trưởng nhóm) và `Member` (Thành viên) |
| Xóa/gỡ nhân viên | Hard delete bản ghi `NhanVienDuAn`, có ràng buộc nghiệp vụ |
| Thêm qua team | Khi gán team qua `TeamDuAnService`, thành viên team được đồng bộ vào `NhanVienDuAn` |

### 1.2. Quan hệ entity

```
DuAn (1) ──< NhanVienDuAn >── (N) NguoiDung
                │
                │ MaDuAn + MaNguoiDung (composite PK)
                │
Team (1) ──< TeamDuAn >── (N) DuAn
  │
  └──< NhanVienTeam >── NguoiDung
```

| Entity | Bảng | Khóa / Ghi chú |
| ------ | ---- | -------------- |
| `NhanVienDuAn` | `NHAN_VIEN_DU_AN` | PK `(MaDuAn, MaNguoiDung)`; không có `IsDeleted` |
| `NguoiDung` | `NGUOI_DUNG` | `HoTenNguoiDung` nullable (`string?`, max 255); có soft delete `IsDeleted` |
| `DuAn` | `DU_AN` | `MaNguoiDung` = người quản lý dự án; soft delete `IsDeleted` |
| `TeamDuAn` | `TEAM_DU_AN` | PK `(MaTeam, MaDuAn)` |
| `NhanVienTeam` | `NHAN_VIEN_TEAM` | PK `(MaNguoiDung, MaTeam)`; `IsLeader` xác định trưởng nhóm team |

Cấu hình EF Core: `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs` (dòng 563–598 cho `NguoiDung`, `NhanVienDuAn`).

### 1.3. Vai trò hệ thống và phân quyền

**Permission** (`QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`):

| Permission | Giá trị |
| ---------- | ------ |
| Xem | `ThanhVienDuAn.Xem` |
| Thêm / cập nhật vai trò | `ThanhVienDuAn.Them` |
| Xóa | `ThanhVienDuAn.Xoa` |

**Gán quyền mặc định** (`QuanLyDuAn/QuanLyDuAn/Data/KhoiTaoTaiKhoanMacDinh.cs`):

| Role ASP.NET Identity | Quyền ThanhVienDuAn |
| --------------------- | ------------------- |
| **Admin** | Không được gán `ThanhVienDuAn.*` trong seed |
| **Manager** | `Xem`, `Them`, `Xoa` |
| **Employee** | Không được gán `ThanhVienDuAn.*` trong seed |

**Kiểm tra tại Controller** (`NhanVienDuAnController`):

- `Index` → `ThanhVienDuAn.Xem`
- `ThemNhanVien`, `CapNhatVaiTro` → `ThanhVienDuAn.Them`
- `XoaNhanVien` → `ThanhVienDuAn.Xoa`

**Kiểm tra phạm vi thao tác ghi** (`NhanVienDuAnService.EnsureCanManageProjectAsync`):

- Chỉ **người quản lý dự án** (`DuAn.MaNguoiDung == MaNguoiDung hiện tại`) mới được `AddAsync`, `UpdateRoleAsync`, `RemoveAsync`.
- Điều kiện chỉnh sửa (`EvaluateEditableCondition`): dự án phải có đủ thông tin cơ bản (loại dự án, ngày bắt đầu/kết thúc hợp lệ) và **không** ở trạng thái `ChoXacNhanHoanThanh`, `HoanThanh`, `LuuTru`.

**Vai trò trong dự án** (`TrangThai.cs`):

- Giá trị lưu CSDL: `Leader`, `Member` (mặc định khi thêm: `Member`).
- `VaiTroLeader` trong dự án **khác** role ASP.NET `Manager`; trưởng nhóm dự án được gán khi thêm từ team nếu `NhanVienTeam.IsLeader == true`.

### 1.4. Lọc soft delete và tài khoản

| Đối tượng | Cơ chế xóa | Điều kiện lọc AS-IS |
| --------- | ---------- | ------------------- |
| `NhanVienDuAn` | Hard delete (`Remove` / `RemoveRange`) | Không có `IsDeleted` |
| `NguoiDung` | Soft delete | `nd.IsDeleted != true` trong hầu hết query join |
| `DuAn` | Soft delete | `duAn.IsDeleted != true` khi lấy dự án |
| `Team` | Soft delete | `team.IsDeleted != true` khi liên quan team |

Khi thêm nhân viên trực tiếp: chỉ chấp nhận user có role **Employee**, loại trừ **Admin/Manager**, tài khoản không bị khóa (`LockoutEnd` null hoặc đã hết hạn).

### 1.5. Điểm truy cập UI

- Trang chính: `Views/NhanVienDuAn/Index.cshtml`
- Partial: `_Form.cshtml`, `_Table.cshtml`
- Liên kết từ `Views/DuAn/_Table.cshtml`, `Views/DuAn/Details.cshtml` (khi có `ThanhVienDuAn.Xem`)

---

## 2. Luồng xử lý hiện tại

### 2.1. Xem danh sách (luồng gây lỗi)

```
NhanVienDuAnController.Index
  → INhanVienDuAnService.GetPageAsync
  → EF Core query (danhSachNhanVienQuery)
  → NhanVienDuAnPageViewModel
  → Views/NhanVienDuAn/Index.cshtml
```

| Hạng mục | Chi tiết |
| -------- | -------- |
| Controller | `NhanVienDuAnController` |
| Action | `Index` (GET) |
| File | `QuanLyDuAn/QuanLyDuAn/Controllers/NhanVienDuAnController.cs` |
| Interface | `INhanVienDuAnService` |
| Service | `NhanVienDuAnService` |
| Method | `GetPageAsync` |
| File service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanVienDuAnService.cs` |
| Input | `maDuAn`, `tuKhoa`, `locMaLoaiDuAn`, `locTrangThaiDuAn`, `pageNumber` (mặc định 1), `pageSize` (mặc định 20) |
| Kiểm tra Controller | `ThanhVienDuAn.Xem`; `maDuAn > 0` |
| Kiểm tra Service | Lấy `DuAn` hợp lệ; tính `CoTheChinhSua` theo trạng thái dự án |
| Query chính | Join `NhanVienDuAn` + `NguoiDung`, lọc `MaDuAn`, `IsDeleted != true`, project sang `NhanVienDuAnItemViewModel`, `CountAsync`, `OrderBy`, `Skip/Take`, `ToListAsync` |
| Dữ liệu trả về | `NhanVienDuAnPageViewModel` gồm `DanhSachNhanVienPhuTrach`, `DanhSachNhanVienCoTheThem`, `Pagination` |
| View | `Index.cshtml` + `_Table.cshtml` + `_Pagination.cshtml` |

**Lưu ý AS-IS:** Tham số `tuKhoa`, `locMaLoaiDuAn`, `locTrangThaiDuAn` được truyền qua redirect và hiển thị trên view, nhưng **không được dùng trong query** của `GetPageAsync`.

### 2.2. Thêm nhân viên trực tiếp

```
NhanVienDuAnController.ThemNhanVien (POST)
  → NhanVienDuAnService.AddAsync
  → SaveChangesAsync
  → ChatDuAnService.DongBoThanhVienPhongChatAsync
  → Redirect Index
```

| Hạng mục | Chi tiết |
| -------- | -------- |
| Permission | `ThanhVienDuAn.Them` |
| Input | `NhanVienDuAnPageViewModel.SelectedMaNguoiDung`, `MaDuAn` |
| Kiểm tra | `EnsureCanManageProjectAsync`; danh sách Employee hợp lệ; không trùng; không khóa |
| Ghi CSDL | Insert `NhanVienDuAn` (`VaiTroTrongDuAn = Member`); ghi `NhatKyPhuTrachDuAn` |
| Đồng bộ chat | Cập nhật thành viên phòng chat dự án |

### 2.3. Cập nhật vai trò

```
NhanVienDuAnController.CapNhatVaiTro (POST)
  → NhanVienDuAnService.UpdateRoleAsync
  → SaveChangesAsync + DongBoThanhVienPhongChatAsync
```

- Chỉ chấp nhận `Leader` hoặc `Member` (chuẩn hóa alias `Thành viên dự án` → `Member`).
- Ghi nhật ký `NhatKyPhuTrachDuAn`.

### 2.4. Xóa nhân viên

```
NhanVienDuAnController.XoaNhanVien (POST)
  → NhanVienDuAnService.RemoveAsync
```

Ràng buộc trước khi xóa:

1. Không xóa nếu nhân viên còn thuộc **team phụ trách** dự án (`TeamDuAn` + `NhanVienTeam`).
2. Không xóa nếu đang có **phân công công việc** trong dự án (`PhanCongCongViec` → `CongViec` → `DanhMucCongViec`).

Xóa bằng `_context.NhanVienDuAn.Remove(entity)` (hard delete).

### 2.5. Thêm nhân viên qua team phụ trách

```
TeamDuAnController (Save/Delete)
  → ITeamDuAnService.SaveAsync / DeleteAsync
  → Thao tác NhanVienDuAn + TeamDuAn
  → ChatDuAnService.DongBoThanhVienPhongChatAsync
```

File: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TeamDuAnService.cs`

**Khi gán team (`SaveAsync`):**

- Bắt buộc có trưởng nhóm team (`NhanVienTeam.IsLeader == true`).
- Trưởng nhóm **bắt buộc** được chọn.
- Thêm vào `NhanVienDuAn` các ID mới; vai trò `Leader` nếu là trưởng nhóm team, ngược lại `Member`.
- Gỡ khỏi `NhanVienDuAn` các thành viên team bị bỏ chọn, **trừ** những người được `protectedByOtherTeams`.

**Cơ chế `protectedByOtherTeams`:**

```csharp
var otherTeamIds = await _context.TeamDuAn
    .Where(x => x.MaDuAn == maDuAn && x.MaTeam != maTeam.Value)
    .Select(x => x.MaTeam).ToListAsync();

var protectedByOtherTeams = await _context.NhanVienTeam
    .Where(x => otherTeamIds.Contains(x.MaTeam) && removeCandidates.Contains(x.MaNguoiDung))
    .Select(x => x.MaNguoiDung).Distinct().ToListAsync();

var removeIds = removeCandidates.Except(protectedByOtherTeams).ToList();
```

**Khi xóa team (`DeleteAsync`):**

- Tùy `xoaNhanVienThuocTeam`: có thể gỡ nhân viên thuộc team khỏi `NhanVienDuAn`.
- Vẫn áp dụng `protectedIds` (nhân viên còn thuộc team phụ trách khác được giữ).
- Kiểm tra phân công công việc trước khi gỡ.

### 2.6. Đăng ký DI

`QuanLyDuAn/QuanLyDuAn/Services/CauHinhDichVu.cs`:

```csharp
dichVu.AddScoped<INhanVienDuAnService, NhanVienDuAnService>();
```

---

## 3. Vị trí chính xác gây lỗi

### 3.1. Thông tin định vị

| Hạng mục | Giá trị |
| -------- | ------- |
| **File** | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanVienDuAnService.cs` |
| **Class** | `NhanVienDuAnService` |
| **Method** | `GetPageAsync` |
| **Dòng gây lỗi** | 79–84 (thực thi tại `ToListAsync()` dòng 84) |
| **Controller gọi** | `NhanVienDuAnController.Index` (dòng 38) |
| **Màn hình** | `Views/NhanVienDuAn/Index.cshtml` — "Danh sách thành viên phụ trách" |
| **Thao tác người dùng** | Mở trang quản lý thành viên dự án (từ danh sách/chi tiết dự án), hoặc quay lại Index sau POST thêm/sửa/xóa |
| **Điểm thực thi** | `ToListAsync()` — lỗi phát sinh khi EF Core cố dịch toàn bộ biểu thức LINQ sang SQL lúc execute |

### 3.2. Đoạn LINQ liên quan (AS-IS)

**Bước 1 — Khai báo query có projection chứa chuỗi mặc định:**

```csharp
var danhSachNhanVienQuery =
    from nvda in _context.NhanVienDuAn
    join nd in _context.NguoiDung on nvda.MaNguoiDung equals nd.MaNguoiDung
    where nvda.MaDuAn == maDuAn && nd.IsDeleted != true
    select new NhanVienDuAnItemViewModel
    {
        MaNguoiDung = nvda.MaNguoiDung,
        HoTenNguoiDung = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}",
        VaiTroTrongDuAn = nvda.VaiTroTrongDuAn ?? TrangThai.VaiTroMember,
        NgayThamGiaDuAn = nvda.NgayThamGiaDuAn,
        ThuocTeamPhuTrach = _context.NhanVienTeam.Any(nvt =>
            assignedTeamIds.Contains(nvt.MaTeam) && nvt.MaNguoiDung == nvda.MaNguoiDung),
        TenTeamPhuTrach = string.Join(
            ", ",
            (from nvt in _context.NhanVienTeam
             join team in _context.Team on nvt.MaTeam equals team.MaTeam
             where assignedTeamIds.Contains(nvt.MaTeam) && nvt.MaNguoiDung == nvda.MaNguoiDung
             select team.TenTeam ?? $"Team {team.MaTeam}").Distinct())
    };
```

**Bước 2 — Phân trang và sắp xếp (LỖI tại đây):**

```csharp
var totalItems = await danhSachNhanVienQuery.CountAsync();

vm.DanhSachNhanVienPhuTrach = await danhSachNhanVienQuery
    .OrderBy(x => x.HoTenNguoiDung)
    .ThenBy(x => x.MaNguoiDung)
    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
    .Take(pagination.PageSize)
    .ToListAsync();
```

### 3.3. Khớp với exception

Thông báo lỗi hệ thống khớp chính xác cấu trúc:

- `DbSet<NhanVienDuAn>().Join(DbSet<NguoiDung>(), ...)`
- `Where(ti => ti.Outer.MaDuAn == __maDuAn_0 && ti.Inner.IsDeleted != (bool?)True)`
- `OrderBy(ti => ti.Inner.HoTenNguoiDung ?? string.Format("Nhân viên {0}", ...))`

EF Core chuyển `$"Nhân viên {nd.MaNguoiDung}"` thành `string.Format(...)`, rồi **inline** biểu thức đó vào `OrderBy` vì `x.HoTenNguoiDung` là property đã được tính trong `Select`.

---

## 4. Phân tích nguyên nhân kỹ thuật

### 4.1. LINQ to Entities vs LINQ to Objects

| Giai đoạn | Mô tả |
| --------- | ----- |
| **LINQ to Entities** | Query gắn `IQueryable`, chưa chạy SQL cho đến khi gọi `ToListAsync`, `CountAsync`, ... |
| **LINQ to Objects** | Sau `ToListAsync()`, các `OrderBy`/`Select` tiếp theo chạy trên bộ nhớ |

Trong `GetPageAsync`, `danhSachNhanVienQuery` là `IQueryable<NhanVienDuAnItemViewModel>`. Chuỗi `.OrderBy(...).Skip(...).Take(...).ToListAsync()` vẫn thuộc LINQ to Entities.

### 4.2. Phần nào chạy phía SQL Server

Toàn bộ pipeline sau khi gọi `ToListAsync()` được EF Core 8.0.11 cố gắng dịch thành một câu SQL, gồm:

- `JOIN` `NHAN_VIEN_DU_AN` ↔ `NGUOI_DUNG`
- `WHERE` theo `MaDuAn`, `IsDeleted`
- `ORDER BY` theo biểu thức họ tên (đã mở rộng từ projection)
- `OFFSET/FETCH` (`Skip`/`Take`)

### 4.3. Vì sao `OrderBy` bị dịch kèm `string.Format`

`OrderBy(x => x.HoTenNguoiDung)` tham chiếu property của **kiểu projection** `NhanVienDuAnItemViewModel`, không phải cột `nd.HoTenNguoiDung` gốc.

EF Core phải thay thế `x.HoTenNguoiDung` bằng biểu thức đã khai báo trong `Select`:

```csharp
nd.HoTenNguoiDung ?? string.Format("Nhân viên {0}", nd.MaNguoiDung)
```

`string.Format` **không có** bản dịch SQL trong EF Core → `InvalidOperationException: Translation of method 'string.Format' failed`.

### 4.4. Toán tử `??` có phải nguyên nhân không?

**Không phải nguyên nhân chính.** `??` (COALESCE) thường được EF Core dịch được.

Nguyên nhân chính là **`string.Format` / string interpolation** nằm trong biểu thức mà `OrderBy` buộc phải đưa vào SQL.

### 4.5. Vì sao build thành công nhưng runtime lỗi

- `dotnet build` chỉ biên dịch C#; không execute LINQ query.
- EF Core kiểm tra khả năng dịch **lúc runtime** khi materialize query.
- Cú pháp `$"Nhân viên {nd.MaNguoiDung}"` hợp lệ với compiler nhưng không hợp lệ với EF translator khi nằm trong `OrderBy` như mô tả trên.

### 4.6. `CountAsync()` có thể vẫn chạy được

`CountAsync()` trên query có `Select` phức tạp thường được EF tối ưu thành `COUNT(*)` trên phần `FROM/WHERE`, **bỏ qua** projection và `OrderBy`. Do đó trang có thể tính được `totalItems` nhưng **vỡ khi tải danh sách** tại `ToListAsync()`.

---

## 5. Mục đích nghiệp vụ của biểu thức đang lỗi

### 5.1. Mục tiêu sắp xếp và hiển thị

| Mục đích | Chi tiết từ source |
| -------- | ------------------ |
| Sắp xếp danh sách | `OrderBy(x => x.HoTenNguoiDung).ThenBy(x => x.MaNguoiDung)` — alphabet theo tên hiển thị, tie-break theo mã |
| Tên hiển thị mặc định | `nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}"` trong `Select` |
| Hiển thị UI | `_Table.cshtml` dòng 35: `@item.HoTenNguoiDung` |

Biểu thức mặc định phục vụ **cả hiển thị lẫn sắp xếp** trong cùng một property projected.

### 5.2. `HoTenNguoiDung` có cho phép null không?

**Có.** Entity `NguoiDung.HoTenNguoiDung` khai báo `string?`. EF config `HasMaxLength(255)`, không bắt buộc `IsRequired`.

### 5.3. Dữ liệu khởi tạo

`KhoiTaoTaiKhoanMacDinh.TaoNguoiDungMacDinhAsync` luôn gán `HoTenNguoiDung = hoTen` (ví dụ admin: `"Quản trị hệ thống"`). Seed **không** tạo bản ghi `HoTenNguoiDung = null`, nhưng model và nghiệp vụ **cho phép** null — nhân sự thêm sau có thể có họ tên null/rỗng nếu dữ liệu thực tế như vậy.

### 5.4. Query ứng viên thêm (không gây lỗi hiện tại)

Trong cùng method, danh sách ứng viên dùng pattern an toàn hơn:

```csharp
orderby nd.HoTenNguoiDung   // sắp xếp theo cột DB gốc
select new {
    MaNguoiDung = nd.MaNguoiDung,
    HoTenNguoiDung = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}"
}
```

`OrderBy`/`orderby` trên cột gốc, chuỗi mặc định chỉ nằm trong `Select` — không trùng pattern lỗi của `danhSachNhanVienQuery`.

---

## 6. Những vị trí có nguy cơ lỗi tương tự

### 6.1. Bảng tổng hợp `$"Nhân viên {..."}` trong EF query

| File | Method | Đoạn / Ngữ cảnh | Chạy trên DB? | Nguy cơ | Liên quan NhanVienDuAn |
| ---- | ------ | --------------- | ------------- | ------- | ---------------------- |
| `NhanVienDuAnService.cs` | `GetPageAsync` | `OrderBy(x => x.HoTenNguoiDung)` sau `Select` có `?? $"Nhân viên..."` | Có | **Chắc chắn lỗi** | **Trực tiếp** |
| `NhanVienDuAnService.cs` | `GetPageAsync` | `Select` dòng 63 — interpolation trong projection | Có (khi materialize) | Phụ thuộc EF; kết hợp OrderBy → lỗi | **Trực tiếp** |
| `NhanVienDuAnService.cs` | `GetPageAsync` | `string.Join` + subquery trong `Select` (dòng 68–73) | Có | **Có thể lỗi** nếu EF không dịch được `string.Join` | **Trực tiếp** |
| `TeamDuAnService.cs` | `GetPageAsync` | `orderby nd.HoTenNguoiDung` rồi `Select` có `?? $"Nhân viên..."` | Có | Không lỗi (orderby cột gốc) | Gián tiếp (team → NVDA) |
| `TeamDuAnService.cs` | `DeleteAsync` | `OrderBy(x => x.HoTenNguoiDung).Select(x => x.HoTenNguoiDung ?? $"Nhân viên...")` | Có | Không lỗi (orderby cột entity gốc) | Gián tiếp |
| `DuAnService.cs` | `GetByIdAsync` (preview) | `orderby NgayThamGiaDuAn`; `Select` có interpolation | Có | Không lỗi hiện tại | Gián tiếp (preview thành viên) |
| `PhanCongCongViecService.cs` | `GetPageAsync` | Chỉ `Select`, không OrderBy trên computed name | Có | Thấp | Gián tiếp (dropdown NVDA) |
| `PhanCongChiTietCongViecService.cs` | `GetPageAsync` | Tương tự | Có | Thấp | Gián tiếp |
| `DeXuatCongViecService.cs` | `GetPageAsync` | `Select` tên người đề xuất/duyệt | Có | Thấp | Không trực tiếp |
| `DeXuatNganSachService.cs` | `GetPageAsync` | Tương tự | Có | Thấp | Không trực tiếp |
| `DuyetDeXuatCongViecService.cs` | `GetPageAsync` | Tương tự | Có | Thấp | Không trực tiếp |
| `DuyetDeXuatNganSachService.cs` | `GetPageAsync` | Tương tự | Có | Thấp | Không trực tiếp |
| `NganSachService.cs` | `GetPageAsync` | Tương tự | Có | Thấp | Gián tiếp (lọc theo NVDA) |
| `ChatDuAnService.cs` | `LayTinNhanGanNhatAsync` | `orderby ThoiGianGui`; `Select` có interpolation | Có | Thấp | Gián tiếp (đồng bộ chat NVDA) |
| `ThanhVienTeamService.cs` | `GetPagedAsync` | `OrderBy HoTenNguoiDung` (raw `?? string.Empty` trong Select) | Có | Thấp | Không trực tiếp |
| `DanhGiaNhanVienService.cs` | `LayDanhSachNhanVienTheoScopeAsync` | `OrderBy(x => x.Key.HoTenNguoiDung)` — key là cột DB; format sau `ToListAsync` | Có | Không lỗi | Gián tiếp (đọc NVDA) |
| `TienDoCongViecService.cs` | `GetPageAsync` | `ToListAsync` trước, rồi `Dictionary` với `$"Nhân viên..."` | Không (in-memory) | Không lỗi | Gián tiếp |
| `Helpers/ExportSupport.cs` | `FormatCurrency` | `string.Format` | Không | Không lỗi | Không |

### 6.2. `string.Format` trong codebase

Chỉ tìm thấy **một** vị trí `string.Format` trong C# application code: `Helpers/ExportSupport.cs` — chạy in-memory, **không liên quan** lỗi EF.

Lỗi runtime dùng `string.Format` vì compiler chuyển string interpolation thành `string.Format` trong expression tree EF nhìn thấy.

---

## 7. Các hướng sửa phù hợp (ĐỀ XUẤT — chưa triển khai)

### Hướng A: Sắp xếp bằng trường có thể dịch sang SQL (ĐỀ XUẤT — ưu tiên)

**Ý tưởng:** Tách sort khỏi display name.

```csharp
// Trước Select hoặc trên entity gốc:
.OrderBy(nd => nd.HoTenNguoiDung)
.ThenBy(nvda => nvda.MaNguoiDung)
// Sau đó Select / map:
HoTenNguoiDung = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}"
```

| Tiêu chí | Đánh giá |
| -------- | -------- |
| Giữ nghiệp vụ | Có — thứ tự gần đúng: null/empty xếp trước/sau theo SQL Server, tie-break `MaNguoiDung` |
| Thay đổi thứ tự | Nhẹ: record `HoTenNguoiDung = null` xếp theo COALESCE cột thật, không theo `"Nhân viên {id}"` |
| Phân trang server-side | **Giữ được** |
| Client evaluation sớm | Không |
| N+1 | Không phát sinh thêm nếu giữ một query |
| Phù hợp style hiện tại | Cao — giống `TeamDuAnService`, `ThanhVienTeamService` |
| Độ an toàn | **Cao nhất, ít thay đổi nhất** |

### Hướng B: Nối chuỗi SQL-translatable trong ORDER BY

EF Core 8 + SQL Server có thể dịch `+` hoặc `CONCAT` trong một số trường hợp, ví dụ:

```csharp
.OrderBy(nd => nd.HoTenNguoiDung ?? ("Nhân viên " + nd.MaNguoiDung.ToString()))
```

| Tiêu chí | Đánh giá |
| -------- | -------- |
| Giữ nghiệp vụ sort theo display name | Gần đúng hơn Hướng A |
| Rủi ro | Phụ thuộc translator; `ToString()` trên int cần kiểm tra thực tế |
| Phân trang | Giữ được |
| Phù hợp style | Trung bình |

### Hướng C: Materialize trước rồi sort in-memory

```csharp
var all = await danhSachNhanVienQuery.ToListAsync(); // vẫn có thể lỗi nếu Select không dịch được
var page = all.OrderBy(...).Skip(...).Take(...).ToList();
```

| Tiêu chí | Đánh giá |
| -------- | -------- |
| Phân trang server-side | **Hỏng** nếu `ToListAsync` trước `Skip/Take` |
| Hiệu năng | Tải toàn bộ nhân viên dự án vào RAM |
| Chấp nhận được? | Chỉ khi số lượng nhỏ và chấp nhận mất pagination đúng nghĩa server-side |

**Không khuyến nghị** cho module đã có `PaginationViewModel` + `_Pagination.cshtml`.

### Hướng D: Tách `DisplayName` khỏi database query

Query chỉ lấy `nd.HoTenNguoiDung`, `nd.MaNguoiDung`; map ViewModel sau `ToListAsync`:

```csharp
HoTenNguoiDung = string.IsNullOrWhiteSpace(raw)
    ? $"Nhân viên {ma}"
    : raw
```

| Tiêu chí | Đánh giá |
| -------- | -------- |
| Giữ nghiệp vụ hiển thị | Có |
| Sort | Cần kết hợp Hướng A hoặc B cho sort |
| Phân trang | Giữ được nếu sort trên SQL |
| Phù hợp clean architecture nhẹ | Tốt |

### Kết luận hướng sửa

**Hướng A (+ D cho phần hiển thị)** là phù hợp nhất: sửa tối thiểu trong `NhanVienDuAnService.GetPageAsync`, giữ server-side pagination, không đổi Controller/View.

---

## 8. Kiểm tra phân trang

### 8.1. AS-IS trong `GetPageAsync`

```
CountAsync()
  → PaginationViewModel.Create(pageNumber, pageSize, totalItems)
  → OrderBy → ThenBy → Skip → Take → ToListAsync()
```

- `OrderBy` nằm **trước** `Skip`/`Take` — đúng server-side pagination.
- View dùng `Views/Shared/_Pagination.cshtml` với `Model.Pagination`.
- Page size cho phép: `10, 20, 50, 100`; mặc định `20` (`PaginationViewModel`).

### 8.2. Hậu quả nếu sửa sai

| Cách sửa | Hậu quả |
| -------- | ------- |
| `ToListAsync()` trước `Skip/Take` | Phân trang chỉ còn giả trên subset đã tải — **sai** |
| Bỏ `OrderBy` trên SQL, sort sau khi `Take` | Thứ tự trang sai — **sai** |
| Hướng A/B trên cột gốc trước `Skip/Take` | Phân trang **đúng** |

Theo `docs/phantrang.md`, module **Thành viên dự án** đã được thiết kế server-side; chỉ danh sách thành viên phân trang, danh sách ứng viên thêm **không** dùng chung page.

---

## 9. Kiểm tra quy tắc soft delete

| Đối tượng | Xóa khi gỡ khỏi dự án | Ghi chú |
| --------- | ---------------------- | ------- |
| `NhanVienDuAn` | Hard delete | `Remove` / `RemoveRange` |
| `NguoiDung` | Không xóa | Chỉ lọc `IsDeleted != true` khi join |
| Nhật ký | Ghi `NhatKyPhuTrachDuAn` | Hành động "Xóa nhân viên phụ trách khỏi dự án" |

Điều kiện `nd.IsDeleted != true` **phải được giữ** khi sửa query.

Chat đồng bộ sau thay đổi: `ChatDuAnService.DongBoThanhVienPhongChatAsync(maDuAn)` loại user đã soft-delete khỏi phòng chat.

---

## 10. Kiểm tra nghiệp vụ team và thành viên dự án

### 10.1. Chống trùng `MaDuAn + MaNguoiDung`

- PK composite trên `NhanVienDuAn` (`QuanLyDuAnDbContext`).
- `AddAsync`: kiểm tra `existedIds`, chỉ insert `addIds = selectedIds.Except(existedIds)`.
- `TeamDuAnService.SaveAsync`: `addIds = selectedIds.Except(projectMemberIds)`.

### 10.2. Trưởng nhóm

- Trưởng nhóm **team**: `NhanVienTeam.IsLeader == true`.
- Khi thêm qua team: nếu là leader → `VaiTroTrongDuAn = Leader`, ngược lại `Member`.
- Thêm trực tiếp qua `NhanVienDuAnService.AddAsync`: luôn `Member`.

### 10.3. Một nhân viên – nhiều team trong cùng dự án

**Có thể.** Nhiều `TeamDuAn` có thể cùng tham chiếu các team; cùng một `MaNguoiDung` có thể vào `NhanVienDuAn` một lần (PK). `protectedByOtherTeams` ngăn gỡ nhầm khi nhân viên còn được team khác phụ trách chọn.

### 10.4. Xóa trực tiếp vs xóa qua team

| Tình huống | Hành vi |
| ---------- | ------- |
| Xóa trực tiếp (`RemoveAsync`) | Chặn nếu còn trong team phụ trách hoặc có phân công CV |
| Bỏ chọn trong team (`SaveAsync`) | Gỡ NVDA trừ khi protected bởi team khác |
| Xóa team (`DeleteAsync`) | Có tùy chọn gỡ NVDA; kiểm tra phân công CV |

---

## 11. Ảnh hưởng đến các chức năng khác

### 11.1. Method `GetPageAsync` của `NhanVienDuAnService`

Chỉ được gọi từ `NhanVienDuAnController.Index`. **Không** dùng chung bởi service khác.

**Ảnh hưởng trực tiếp khi lỗi:**

- Màn hình **Quản lý thành viên dự án** không tải được.
- Redirect về `Index` sau **Thêm / Cập nhật vai trò / Xóa** cũng gọi lại `GetPageAsync` → có thể lỗi dù thao tác ghi đã thành công.

### 11.2. Các chức năng dùng bảng `NhanVienDuAn` (không qua `GetPageAsync`)

| Chức năng | Service | Ảnh hưởng bởi lỗi hiện tại |
| --------- | ------- | --------------------------- |
| Danh mục công việc | `DanhMucCongViecScopeService` | Không (query riêng) |
| Phân công công việc | `PhanCongCongViecService` | Không trực tiếp |
| Phân công chi tiết | `PhanCongChiTietCongViecService` | Không trực tiếp |
| Đề xuất công việc | `DeXuatCongViecService` | Không trực tiếp |
| Đề xuất ngân sách | `DeXuatNganSachService`, `NganSachService` | Không trực tiếp |
| Đánh giá nhân viên | `DanhGiaNhanVienService` | Không trực tiếp |
| Chat dự án | `ChatDuAnService` | Không trực tiếp; đồng bộ thành viên vẫn chạy sau Add/Remove |
| Dashboard | `DashboardService` | Không trực tiếp |
| Chi tiết dự án | `DuAnService.GetByIdAsync` | Preview thành viên dùng query khác — **không** dùng `OrderBy` trên computed name |
| Xuất báo cáo | Các export module khác | Không có export riêng cho NVDA |
| AI Dataset | `AiService` | Đếm `NhanVienDuAn` — không qua `GetPageAsync` |

---

## 12. Kiểm tra encoding

| Kiểm tra | Kết quả |
| -------- | ------- |
| Chuỗi source `"Nhân viên {MaNguoiDung}"` | Unicode tiếng Việt có dấu đầy đủ trong `NhanVienDuAnService.cs` |
| Thông báo UI (_Table, Index) | Tiếng Việt có dấu: "Quản lý thành viên dự án", "Danh sách thành viên phụ trách", ... |
| File tài liệu này | UTF-8, tiếng Việt có dấu |
| Mojibake | Không phát hiện trong các file đã đọc |

Không cần chuyển sang tiếng Việt không dấu. Không sửa encoding hàng loạt dự án.

---

## 13. Kết quả build

```text
dotnet build QuanLyDuAn/QuanLyDuAn/QuanLyDuAn.csproj
→ Build succeeded (0 Error, 2 Warning CS1998 unrelated)
Target: net8.0
EF Core: 8.0.11
```

**Vì sao build thành công vẫn lỗi runtime:** Lỗi thuộc EF Core query translation, chỉ xuất hiện khi execute SQL.

### Tái hiện lỗi (runtime)

1. Đăng nhập tài khoản **Manager** có `ThanhVienDuAn.Xem`.
2. Mở dự án do Manager đó quản lý (`DuAn.MaNguoiDung` = user hiện tại).
3. Truy cập `/NhanVienDuAn/Index?maDuAn={id}` (hoặc link "Thành viên dự án" từ danh sách/chi tiết dự án).
4. Quan sát `InvalidOperationException` tại `GetPageAsync` → `ToListAsync()` khi dự án có ít nhất một bản ghi `NhanVienDuAn` join được `NguoiDung` (điều kiện lỗi translation, không phụ thuộc họ tên null).

---

## 14. Kết luận AS-IS

### Chức năng đang hoạt động theo nghiệp vụ nào

Quản lý tập nhân viên phụ trách dự án: xem danh sách có phân trang, thêm Employee trực tiếp, cập nhật vai trò Leader/Member, xóa có ràng buộc team/phân công; đồng bộ với team phụ trách qua `TeamDuAnService`; chỉ Manager sở hữu dự án được ghi; đồng bộ chat sau thay đổi thành viên.

### Method đang làm hệ thống lỗi

`NhanVienDuAnService.GetPageAsync` — cụ thể chuỗi `OrderBy(x => x.HoTenNguoiDung)` sau projection chứa `$"Nhân viên {nd.MaNguoiDung}"`, thực thi tại `ToListAsync()` dòng 84.

### Nguyên nhân kỹ thuật chính

EF Core 8 không dịch được `string.Format` (sinh ra từ string interpolation) khi biểu thức đó được **inline vào `ORDER BY`** qua property computed trong `Select`.

### Phạm vi ảnh hưởng

- **Trực tiếp:** Màn hình `NhanVienDuAn/Index` và mọi redirect về Index sau POST.
- **Gián tiếp:** Không block thêm/sửa/xóa ở tầng service, nhưng người dùng không xem lại được danh sách sau thao tác.

### Hướng sửa đề xuất phù hợp nhất

**Hướng A + D:** `OrderBy`/`ThenBy` trên `nd.HoTenNguoiDung` và `nvda.MaNguoiDung` (hoặc tương đương trước `Skip/Take`); giữ chuỗi hiển thị `"Nhân viên {MaNguoiDung}"` trong map ViewModel sau query hoặc trong `Select` không tham gia sort.

### File dự kiến phải sửa (bước tiếp theo)

| File | Lý do |
| ---- | ----- |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanVienDuAnService.cs` | Sửa query `GetPageAsync` |

### File không cần sửa (cho lỗi này)

| File | Lý do |
| ---- | ----- |
| `NhanVienDuAnController.cs` | Logic đúng; không phải nguồn lỗi translation |
| `Views/NhanVienDuAn/*` | Chỉ hiển thị dữ liệu |
| `ViewModels/NhanVienDuAn/*` | Cấu trúc phù hợp |
| `QuanLyDuAnDbContext.cs` | Không liên quan translation |
| Entity models | Không đổi schema |
| `TeamDuAnService.cs` | Pattern sort đã an toàn hơn |

### Migration / database / View / Controller

| Hạng mục | Cần thiết? |
| -------- | ---------- |
| Migration / DB change | **Không** |
| Thay đổi Controller / route / permission | **Không** |
| Thay đổi View | **Không** (trừ khi muốn hiển thị thêm — không bắt buộc cho fix lỗi) |
| Sửa Service | **Có** — chỉ `NhanVienDuAnService.cs` |

---

*Tài liệu được tạo từ source thực tế. Chưa có thay đổi code.*

---

## 15. Kết quả triển khai sửa lỗi

### File đã sửa

- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanVienDuAnService.cs`
- `docs/nhanvienduan.md`

### Method đã sửa

- `NhanVienDuAnService.GetPageAsync`

### Nguyên nhân trước khi sửa

Query danh sách thành viên dự án project trực tiếp `HoTenNguoiDung` bằng:

```csharp
nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}"
```

Sau đó lại sắp xếp bằng:

```csharp
OrderBy(x => x.HoTenNguoiDung)
```

EF Core inline biểu thức tên hiển thị vào `ORDER BY`, biến string interpolation thành `string.Format(...)` và phát sinh lỗi runtime:

```text
Translation of method 'string.Format' failed.
```

### Cách sửa thực tế

`GetPageAsync` đã được tách thành query dữ liệu gốc từ `NhanVienDuAn` join `NguoiDung`, giữ điều kiện:

```csharp
nvda.MaDuAn == maDuAn && nd.IsDeleted != true
```

Sau đó sắp xếp trực tiếp trên cột dữ liệu gốc trước khi phân trang:

```csharp
.OrderBy(x => x.NguoiDung.HoTenNguoiDung)
.ThenBy(x => x.NguoiDung.MaNguoiDung)
.Skip((pagination.PageNumber - 1) * pagination.PageSize)
.Take(pagination.PageSize)
```

Projection sang `NhanVienDuAnItemViewModel` vẫn giữ các dữ liệu:

- `MaNguoiDung`
- `HoTenNguoiDung`
- `VaiTroTrongDuAn`
- `NgayThamGiaDuAn`
- `ThuocTeamPhuTrach`
- `TenTeamPhuTrach`

Sau khi `ToListAsync()` hoàn tất, mới chuẩn hóa tên hiển thị mặc định:

```csharp
item.HoTenNguoiDung = string.IsNullOrWhiteSpace(item.HoTenNguoiDung)
    ? $"Nhân viên {item.MaNguoiDung}"
    : item.HoTenNguoiDung;
```

### Server-side pagination

Server-side pagination vẫn được giữ. Thứ tự thực thi query danh sách chính vẫn là:

```text
OrderBy
→ ThenBy
→ Skip
→ Take
→ ToListAsync
```

`CountAsync()` vẫn chạy trên base query đã lọc theo dự án và người dùng chưa soft delete. `PaginationViewModel.Create(pageNumber, pageSize, totalItems)` không thay đổi, không thay đổi các lựa chọn page size `10, 20, 50, 100`.

### Thay đổi Controller/View/ViewModel/database

Không thay đổi:

- Controller
- action hoặc route
- interface service
- View
- ViewModel
- entity
- `QuanLyDuAnDbContext`
- database
- migration
- permission `ThanhVienDuAn.*`
- workflow thêm, sửa vai trò hoặc xóa nhân viên

### Kết quả build

Đã chạy:

```text
dotnet build QuanLyDuAn/QuanLyDuAn/QuanLyDuAn.csproj
```

Kết quả:

```text
Build succeeded.
0 Error(s)
2 Warning(s)
```

Hai warning còn lại là `CS1998` sẵn có tại `FileTienDoCongViecService.cs`, không phát sinh từ thay đổi `NhanVienDuAnService`.

### Kết quả runtime test

Đã chạy ứng dụng bằng:

```text
dotnet run --project QuanLyDuAn/QuanLyDuAn/QuanLyDuAn.csproj --no-build --no-launch-profile --urls http://127.0.0.1:5037
```

Kết quả môi trường:

- Ứng dụng start thành công trên `http://127.0.0.1:5037`.
- Kết nối SQL Server và seed startup chạy được khi chạy ngoài sandbox.
- Request chưa đăng nhập tới `/NhanVienDuAn/Index?maDuAn=1` trả về `302 Found` về `/Account/Login?ReturnUrl=...`, đúng cơ chế bảo vệ đăng nhập.
- Chưa thể kiểm thử đầy đủ màn hình `NhanVienDuAn/Index` bằng tài khoản Manager trong phiên này vì seed mặc định chỉ tạo `admin/111111`, không tạo tài khoản Manager mặc định; request không có session đăng nhập chưa đi vào `NhanVienDuAnService.GetPageAsync`.

Do đó, runtime đã xác nhận app start và route được bảo vệ bởi login, nhưng chưa xác nhận được bằng giao diện Manager các bước xem trang đầu/trang sau/page size/thêm/sửa/xóa.

### Kiểm tra `TenTeamPhuTrach`

Chưa phát sinh lỗi runtime mới tại `TenTeamPhuTrach` trong phạm vi kiểm thử hiện có. Vì chưa có bằng chứng lỗi dịch SQL ở `string.Join`, phần này được giữ nguyên theo yêu cầu, không refactor sang query nhóm riêng.

### Danh sách file thực tế đã thay đổi

- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/NhanVienDuAnService.cs`
- `docs/nhanvienduan.md`
