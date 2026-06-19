# Duyệt đề xuất công việc

Tài liệu này ghi nhận kết quả đọc source thực tế ngày 18/06/2026 cho chức năng `DuyetDeXuatCongViec`, tập trung vào nhánh từ chối đề xuất công việc và ô nhập lý do từ chối trên giao diện. Phạm vi công việc chỉ là phân tích và tài liệu hóa, không sửa runtime source, không tạo migration và không thay đổi database.

## 1. Phạm vi source đã đọc

Các file đã đọc trực tiếp:

- `QuanLyDuAn/QuanLyDuAn/Controllers/DeXuatCongViecController.cs`
- `QuanLyDuAn/QuanLyDuAn/Controllers/DuyetDeXuatCongViecController.cs`
- `QuanLyDuAn/QuanLyDuAn/Controllers/DuyetDeXuatNganSachController.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDeXuatCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDuyetDeXuatCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDuyetDeXuatNganSachService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DeXuatCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DyetDeXuatCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DuyetDeXuatNganSachService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/CongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/CauHinhDichVu.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DeXuatCongViec.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/CongViec.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/ChiPhi.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/NganSach.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DanhMucCongViec.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DuAn.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/NguoiDung.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/NhatKyQuanLyDuAn.cs`
- `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`
- `QuanLyDuAn/QuanLyDuAn/Migrations/20260527125053_Init.cs`
- `QuanLyDuAn/QuanLyDuAn/Migrations/20260527125053_Init.Designer.cs`
- `QuanLyDuAn/QuanLyDuAn/Migrations/QuanLyDuAnDbContextModelSnapshot.cs`
- `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`
- `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs`
- `QuanLyDuAn/QuanLyDuAn/Program.cs`
- `QuanLyDuAn/QuanLyDuAn/appsettings.json`
- `QuanLyDuAn/QuanLyDuAn/appsettings.Development.json`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DeXuatCongViec/*.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DuyetDeXuatCongViec/*.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/DeXuatCongViec/Index.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DeXuatCongViec/_Form.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DeXuatCongViec/_Filter.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DeXuatCongViec/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DeXuatCongViec/DieuHuong.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatCongViec/Index.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatCongViec/_Filter.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatCongViec/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatNganSach/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/approval/index.js`
- `quanlyduan.sql`
- Các tài liệu hiện có có nhắc đến bảng/chức năng: `docs/usecase.md`, `docs/workflow-he-thong.md`, `docs/motacsdl.md`, `docs/rang-buoc-index-436.md`

Đã thử kiểm tra metadata runtime database bằng `sqlcmd` với connection string trong `appsettings.json`, chỉ chạy câu truy vấn đọc-only. Lệnh không truy vấn được vì lỗi kết nối/encryption của SQL client trước khi vào database. Vì vậy phần database runtime cần xác nhận lại bằng câu SQL đọc-only ở cuối tài liệu; các kết luận về cấu trúc hiện tại dựa trên Entity, DbContext, Migration, Model Snapshot và `quanlyduan.sql`.

## 2. Cấu trúc chức năng hiện tại

### Entity và quan hệ

`DeXuatCongViec` map bảng `DE_XUAT_CONG_VIEC`, gồm các dữ liệu chính:

- `MaDeXuatCV`
- `MaDuAn`
- `MaDanhMucCV`
- `MaMucDo`
- `MaNguoiDungDeXuat`
- `MaNguoiDungDuyet`
- `TenCongViecDeXuat`
- `MoTaCongViecDeXuat`
- `ChiPhiDeXuat`
- `NgayBatDauCongViecDeXuat`
- `NgayKetThucCVDeXuatDuKien`
- `NgayDeXuatCongViec`
- `NgayDuyetDeXuatCongViec`
- `TrangThaiCongViecDeXuat`
- Các cột xóa mềm: `IsDeleted`, `DeletedAt`, `DeletedBy`

Không có property hoặc cột `LyDoTuChoi`, `GhiChuTuChoi`, `NhanXet`, `GhiChu`, `NoiDung`, `LyDo` trong `DeXuatCongViec`.

`CongViec` có FK tùy chọn `MaDeXuatCV` trỏ lại `DE_XUAT_CONG_VIEC`. Khi duyệt đề xuất thành công, service tạo `CONG_VIEC` từ dữ liệu đề xuất.

`ChiPhi` có FK `MaCongViec` và `MaNganSach`. Khi duyệt đề xuất thành công, service luôn tạo một bản ghi `CHI_PHI` từ `ChiPhiDeXuat` và ngân sách active hiện tại.

`NhatKyQuanLyDuAn` map bảng `NHAT_KY_QUAN_LY_DU_AN`, có `NkHanhDongQLDA nvarchar(255)`. Nhánh từ chối đề xuất công việc ghi lý do vào chính chuỗi hành động này nếu người duyệt gửi lý do.

### Controller, service và DI

- Controller tạo/hủy đề xuất: `DeXuatCongViecController`.
- Controller duyệt/từ chối đề xuất: `DuyetDeXuatCongViecController`.
- Interface duyệt: `IDuyetDeXuatCongViecService`.
- Implementation duyệt: `DyetDeXuatCongViecService`. Tên class/file thiếu chữ `u` trong `Duyet`, nhưng được đăng ký DI đúng trong `CauHinhDichVu.cs`: `IDuyetDeXuatCongViecService -> DyetDeXuatCongViecService`.

### Route và HTTP method

Theo route mặc định trong `Program.cs`: `{controller=Dashboard}/{action=Index}/{id?}`.

- `GET /DuyetDeXuatCongViec/Index`
- `POST /DuyetDeXuatCongViec/Duyet`
- `POST /DuyetDeXuatCongViec/TuChoi`
- `GET /DeXuatCongViec/Index`
- `POST /DeXuatCongViec/TaoDeXuat`
- `POST /DeXuatCongViec/HuyDeXuat`

### Permission

`Permissions.cs` định nghĩa:

- `DeXuatCongViec.Xem`
- `DeXuatCongViec.Them`
- `DuyetDeXuatCongViec.Xem`
- `DuyetDeXuatCongViec.Duyet`

Controller kiểm tra:

- Xem danh sách duyệt: `DuyetDeXuatCongViec.Xem`.
- Duyệt/từ chối: `DuyetDeXuatCongViec.Duyet`.
- Xem màn hình đề xuất: `DeXuatCongViec.Xem`.
- Tạo/hủy đề xuất: `DeXuatCongViec.Them`.

### Trạng thái

`TrangThai.cs` có các trạng thái dùng cho đề xuất công việc:

- `ChoDuyet`
- `DaDuyet`
- `TuChoi`
- `DaHuy`

Service dùng `TrangThai.EqualsValue(...)` để so sánh, nên có hỗ trợ chuẩn hóa dấu/khoảng trắng và biến thể hiển thị.

### Nhật ký

Các nhánh liên quan ghi `NHAT_KY_QUAN_LY_DU_AN`:

- Tạo đề xuất: `Tạo đề xuất công việc: {TenCongViecDeXuat}`
- Hủy đề xuất: `Hủy đề xuất công việc #{MaDeXuatCV}`
- Duyệt đề xuất: `Duyệt đề xuất công việc #{MaDeXuatCV}`
- Từ chối đề xuất: `Từ chối đề xuất công việc #{MaDeXuatCV}` và nếu có lý do thì nối thêm `. Lý do: {lyDo.Trim()}`

Nhánh duyệt còn ghi `NHAT_KY_CHI_PHI` khi tạo chi phí.

## 3. Luồng tạo và hủy đề xuất AS-IS

### Ai được tạo đề xuất

`DeXuatCongViecService.EnsureCanProposeForProjectAsync(...)` quyết định quyền tạo:

- Nếu người hiện tại là Manager của chính dự án (`DU_AN.MaNguoiDung == currentUserId`) thì bị chặn: Manager chỉ duyệt, không tạo đề xuất.
- Nếu dự án đã có team phụ trách trong `TEAM_DU_AN`, chỉ nhân sự là leader trong `NHAN_VIEN_TEAM` của team phụ trách đó được tạo đề xuất.
- Nếu dự án chưa có team phụ trách, nhân viên thuộc dự án trong `NHAN_VIEN_DU_AN` được tạo đề xuất.

### Điều kiện dự án và ngân sách khi tạo

`CreateAsync(...)` kiểm tra:

- Có `MaDuAn`, `MaDanhMucCV`, `MaMucDo`.
- `ChiPhiDeXuat > 0`.
- Có ngày bắt đầu và ngày kết thúc dự kiến.
- Ngày kết thúc không trước ngày bắt đầu.
- Tên và mô tả công việc không rỗng.
- Dự án tồn tại và không bị xóa mềm.
- Dự án không ở trạng thái hoàn thành, chờ xác nhận hoàn thành, tạm dừng, lưu trữ hoặc đã hủy.
- Ngày đề xuất nằm trong khoảng ngày dự án nếu dự án có ngày bắt đầu/kết thúc.
- Có ngân sách hiện hành active, trạng thái đã duyệt.
- `ChiPhiDeXuat` không vượt ngân sách còn lại, tính từ ngân sách active trừ tổng `CHI_PHI` của dự án.
- Danh mục công việc thuộc đúng dự án và chưa bị xóa mềm.
- Mức độ ưu tiên tồn tại.
- Không trùng đề xuất đang chờ duyệt theo dự án, tên, ngày bắt đầu, ngày kết thúc.

### Dữ liệu được lưu khi tạo

`CreateAsync(...)` lưu vào `DE_XUAT_CONG_VIEC`:

- Dự án
- Danh mục công việc
- Mức độ ưu tiên
- Người đề xuất
- Tên công việc
- Mô tả công việc
- Chi phí đề xuất
- Ngày bắt đầu
- Ngày kết thúc dự kiến
- Ngày đề xuất
- Trạng thái ban đầu `ChoDuyet`

Source hiện tại của đề xuất công việc không có trường `LyDoDeXuat`. Trường `LyDoDeXuat` xuất hiện ở `DE_XUAT_NGAN_SACH`, không phải `DE_XUAT_CONG_VIEC`.

### Hủy đề xuất

`CancelAsync(int maDeXuatCv)`:

- Lấy người hiện tại.
- Tìm đề xuất chưa xóa mềm.
- Kiểm tra người hiện tại còn có quyền tạo đề xuất cho dự án.
- Chỉ người tạo đề xuất mới được hủy đề xuất của mình.
- Chỉ hủy khi đề xuất đang `ChoDuyet`.
- Cập nhật `TrangThaiCongViecDeXuat = DaHuy`.
- Ghi `NHAT_KY_QUAN_LY_DU_AN`.
- Gọi `SaveChangesAsync()` một lần.

Không có lý do hủy.

## 4. Luồng duyệt AS-IS

Luồng thực tế:

```text
DuyetDeXuatCongViecController.Duyet
→ DyetDeXuatCongViecService.ApproveAsync
→ kiểm tra đề xuất còn tồn tại và đang ChoDuyet
→ kiểm tra người duyệt là Manager đúng dự án
→ kiểm tra dự án chưa đóng / chưa hoàn thành
→ kiểm tra danh mục và mức độ ưu tiên còn hợp lệ
→ lấy ngân sách active đã duyệt
→ kiểm tra chưa có CONG_VIEC tạo từ MaDeXuatCV này
→ tạo CONG_VIEC
→ SaveChangesAsync để có MaCongViec
→ tạo CHI_PHI gắn MaCongViec và MaNganSach active
→ cập nhật DE_XUAT_CONG_VIEC
→ SaveChangesAsync
→ ghi NHAT_KY_CHI_PHI
→ ghi NHAT_KY_QUAN_LY_DU_AN
→ nếu dự án đang ChoXacNhanHoanThanh thì chuyển về DangThucHien và ghi log
→ SaveChangesAsync
→ Commit transaction
```

Chi tiết:

- Có transaction: `BeginTransactionAsync(IsolationLevel.Serializable)`.
- Có 3 lần `SaveChangesAsync()` trong transaction.
- Insert `CONG_VIEC`.
- Insert `CHI_PHI`.
- Update `DE_XUAT_CONG_VIEC`: trạng thái `DaDuyet`, người duyệt, ngày duyệt.
- Insert `NHAT_KY_CHI_PHI`.
- Insert `NHAT_KY_QUAN_LY_DU_AN`.
- Có thể update `DU_AN.TrangThaiDuAn` về `DangThucHien` nếu trước đó đang `ChoXacNhanHoanThanh`.
- Không gọi trực tiếp `ITrangThaiWorkflowService`; service tự cập nhật trạng thái dự án trong một nhánh cụ thể.

Lưu ý nghiệp vụ: `ApproveAsync` tạo `ChiPhi` kể cả khi `ChiPhiDeXuat` null, nhưng `CreateAsync` đã chặn tạo đề xuất nếu chi phí không có hoặc không lớn hơn 0.

## 5. Luồng từ chối AS-IS

Kết luận theo source hiện tại: lý do từ chối chỉ được gửi vào Controller, truyền xuống Service và được ghép vào chuỗi nhật ký `NHAT_KY_QUAN_LY_DU_AN.NkHanhDongQLDA`. Lý do không được lưu trong `DE_XUAT_CONG_VIEC`, không có ViewModel đọc lại lý do, và màn hình sau khi tải lại không hiển thị lại lý do từ chối.

Trường hợp khớp với danh sách cần kết luận:

- Service ghi nhật ký nhưng không lưu Entity.
- Nút từ chối nhanh luôn gửi lý do rỗng.
- Lý do không lưu trực tiếp trong `DE_XUAT_CONG_VIEC`.
- View sau khi từ chối không đọc lại lý do từ nhật ký.

Luồng thực tế:

```text
Người dùng nhập lý do
→ Views/DuyetDeXuatCongViec/_Table.cshtml, form chi tiết
→ không có AJAX; JavaScript chỉ confirm/disable submit
→ POST /DuyetDeXuatCongViec/TuChoi
→ DuyetDeXuatCongViecController.TuChoi(int maDeXuatCv, string? lyDo, ...)
→ IDuyetDeXuatCongViecService.RejectAsync(int maDeXuatCv, string? lyDo)
→ DyetDeXuatCongViecService.RejectAsync(...)
→ deXuat.TrangThaiCongViecDeXuat = TuChoi
→ deXuat.MaNguoiDungDuyet = currentUserId
→ deXuat.NgayDuyetDeXuatCongViec = DateTime.Now
→ lyDo được trim nếu không rỗng
→ thêm NhatKyQuanLyDuAn.NkHanhDongQLDA
→ SaveChangesAsync
→ NHAT_KY_QUAN_LY_DU_AN.NkHanhDongQLDA
→ View tải lại từ DeXuatCongViec/DuyetDeXuatCongViec query không đọc log
→ lý do không hiển thị lại
```

## 6. Bảng truy vết lý do từ chối

| Bước | File/method | Biến hoặc thuộc tính | Có lý do không | Xử lý tiếp |
| --- | --- | --- | ---: | --- |
| View, nút nhanh | `Views/DuyetDeXuatCongViec/_Table.cshtml` | hidden `name="lyDo" value=""` | Không | Gửi chuỗi rỗng khi bấm nút `Từ chối` ở dòng chính |
| View, form chi tiết | `Views/DuyetDeXuatCongViec/_Table.cshtml` | input text `name="lyDo"` | Có, nếu người dùng nhập | Gửi qua POST form thường |
| JavaScript | `wwwroot/js/approval/index.js` | Không đọc field lý do | Không xử lý | Chỉ `confirm`, disable submit, toggle chi tiết |
| HTTP request | `POST /DuyetDeXuatCongViec/TuChoi` | form-urlencoded `maDeXuatCv`, `lyDo`, filter | Có nếu submit form chi tiết | Model binding vào tham số action |
| Controller | `DuyetDeXuatCongViecController.TuChoi` | `string? lyDo` | Có | Truyền nguyên giá trị xuống service |
| Service | `DyetDeXuatCongViecService.RejectAsync` | `string? lyDo`, local `lyDoTuChoi` | Có | Nếu không rỗng thì `Trim()` và nối vào nhật ký |
| Entity đề xuất | `DeXuatCongViec` | Không có property lý do từ chối | Không | Chỉ cập nhật trạng thái, người duyệt, ngày duyệt |
| Database đề xuất | `DE_XUAT_CONG_VIEC` | Không có cột lý do từ chối | Không | Không lưu |
| Database nhật ký | `NHAT_KY_QUAN_LY_DU_AN.NkHanhDongQLDA` | Chuỗi hành động | Có, nếu form gửi lý do và còn trong giới hạn 255 | Lưu dưới dạng text log, không có FK riêng tới đề xuất |
| View sau khi từ chối | `GetPageAsync` + `_Table.cshtml` | Không có ViewModel lý do | Không | Hardcode hiển thị `Không có` ở màn hình duyệt; màn hình người đề xuất không có vùng lý do |

## 7. Kiểm tra giao diện từ chối

Trong `Views/DuyetDeXuatCongViec/_Table.cshtml` có hai form từ chối cho mỗi đề xuất đang `ChoDuyet` và user có quyền duyệt:

### Form 1: Nút từ chối nhanh ở dòng chính

Source rút gọn:

```cshtml
<form asp-action="TuChoi" method="post" class="js-confirm-submit">
    <input type="hidden" name="maDeXuatCv" value="@item.MaDeXuatCV" />
    <input type="hidden" name="locMaDuAn" value="@Model.LocMaDuAn" />
    <input type="hidden" name="locTrangThai" value="@Model.LocTrangThai" />
    <input type="hidden" name="lyDo" value="" />
    <button type="submit">Từ chối</button>
</form>
```

Đặc điểm:

- Gửi `lyDo=""`.
- Không có ô nhập lý do.
- Người dùng có thể bấm nút này mà không mở chi tiết, nên lý do không bao giờ được gửi.
- Có `asp-action`, `method="post"`; Form Tag Helper thường sinh anti-forgery token, nhưng action controller không có `[ValidateAntiForgeryToken]` và `Program.cs` không đăng ký global `AutoValidateAntiforgeryToken`.

### Form 2: Form từ chối trong dòng chi tiết

Source rút gọn:

```cshtml
<form asp-action="TuChoi" method="post" class="duyet-de-xuat-cong-viec-reject-form js-confirm-submit">
    <input type="hidden" name="maDeXuatCv" value="@item.MaDeXuatCV" />
    <input type="hidden" name="locMaDuAn" value="@Model.LocMaDuAn" />
    <input type="hidden" name="locTrangThai" value="@Model.LocTrangThai" />
    <input type="text" name="lyDo" class="form-control form-control-sm" placeholder="Nhập lý do từ chối (nếu có)" />
    <button type="submit">Gửi từ chối kèm lý do</button>
</form>
```

Đặc điểm:

- Input có `name="lyDo"`, khớp tham số controller `string? lyDo`.
- Lý do là tùy chọn theo placeholder `(nếu có)`.
- Không có `required`.
- Không có `maxlength`.
- Không có client validation riêng.
- Không có server validation bắt buộc.
- Service trim nếu có lý do không trắng.

### Hiển thị sau khi xử lý

Trong dòng chi tiết có nhãn `Lý do từ chối / ghi chú`, nhưng value hardcode `Không có`. ViewModel `DuyetDeXuatCongViecItemViewModel` không có property lý do. Query `GetPageAsync(...)` không đọc `NHAT_KY_QUAN_LY_DU_AN`, nên dù lý do có nằm trong log thì màn hình duyệt không hiển thị lại.

Màn hình người đề xuất `Views/DeXuatCongViec/_Table.cshtml` chỉ hiển thị mô tả, người duyệt, ngày duyệt; không hiển thị lý do từ chối.

## 8. Kiểm tra Controller từ chối

Action:

```csharp
[HttpPost]
public async Task<IActionResult> TuChoi(int maDeXuatCv, string? lyDo, int? locMaDuAn, string? locTrangThai)
```

Kết luận:

- Controller nhận tham số rời, không nhận ViewModel.
- Có tham số `string? lyDo`.
- Tên input `lyDo` trong view khớp tham số action.
- Có kiểm tra permission `Permissions.DuyetDeXuatCongViec.Duyet`.
- Không có `[ValidateAntiForgeryToken]`.
- Không kiểm tra `ModelState`.
- Không validate lý do rỗng.
- Không giới hạn độ dài.
- Có truyền `lyDo` xuống service: `_service.RejectAsync(maDeXuatCv, lyDo)`.
- Không dùng `TempData` để giữ lại lý do khi lỗi.
- Redirect giữ `locMaDuAn`, `locTrangThai`, nhưng không giữ `pageNumber`, `pageSize`.
- Kiểm tra Manager đúng dự án giao cho service.

## 9. Kiểm tra Service từ chối

Chữ ký hiện tại:

```csharp
Task RejectAsync(int maDeXuatCv, string? lyDo)
```

Implementation:

```csharp
public async Task RejectAsync(int maDeXuatCv, string? lyDo)
```

Kết luận:

- Có nhận lý do.
- Có kiểm tra đề xuất tồn tại và chưa xóa mềm.
- Có chặn nếu trạng thái không còn `ChoDuyet`.
- Có kiểm tra Manager đúng dự án qua `EnsureIsProjectManagerAsync(currentUserId, deXuat.MaDuAn)`.
- Có cập nhật `TrangThaiCongViecDeXuat = TuChoi`.
- Có cập nhật `MaNguoiDungDuyet`.
- Có cập nhật `NgayDuyetDeXuatCongViec`.
- Không cập nhật trường lý do trên `DeXuatCongViec` vì Entity không có property này.
- Có trim lý do nếu `!string.IsNullOrWhiteSpace(lyDo)`.
- Nếu lý do rỗng/trắng thì log không có phần lý do.
- Có ghi `NHAT_KY_QUAN_LY_DU_AN.NkHanhDongQLDA`.
- Nội dung log chính xác theo source: `Từ chối đề xuất công việc #{MaDeXuatCV}{lyDoTuChoi}`.
- Không có transaction riêng trong nhánh từ chối.
- Có một lần `SaveChangesAsync()`.
- Không sửa `CONG_VIEC`.
- Không sửa `CHI_PHI`.
- Không sửa `NGAN_SACH`.
- Có thể xảy ra trạng thái đã `TuChoi` nhưng lý do không nằm trong entity đề xuất; nếu bấm nút nhanh thì lý do rỗng ngay từ view.
- Vì `NkHanhDongQLDA` bị giới hạn 255 ký tự trong DbContext/Migration/SQL, lý do dài có nguy cơ gây lỗi lưu hoặc bị cắt tùy cấu hình SQL Server/EF, source không chủ động cắt.

## 10. Cấu trúc database liên quan

### `DE_XUAT_CONG_VIEC`

Theo Entity, DbContext, Migration, Snapshot và `quanlyduan.sql`, các cột liên quan:

- `MaDeXuatCV int identity`
- `MaDuAn int`
- `MaDanhMucCV int`
- `MaMucDo int`
- `MaNguoiDungDeXuat int`
- `MaNguoiDungDuyet int null`
- `TenCongViecDeXuat nvarchar(255) null`
- `MoTaCongViecDeXuat nvarchar(max) null`
- `ChiPhiDeXuat decimal(18,2) null`
- `NgayBatDauCongViecDeXuat datetime2 null`
- `NgayKetThucCVDeXuatDuKien datetime2 null`
- `NgayDeXuatCongViec datetime2 null`
- `NgayDuyetDeXuatCongViec datetime2 null`
- `TrangThaiCongViecDeXuat nvarchar(50) null`
- `IsDeleted bit null`
- `DeletedAt datetime2 null`
- `DeletedBy int null`

Không có `LyDoTuChoi`, `GhiChuTuChoi`, `NhanXet`, `GhiChu`, `NoiDung`, `LyDo`, `LyDoDeXuat`.

### `CONG_VIEC`

Cột liên quan nhánh duyệt:

- `MaCongViec`
- `MaDeXuatCV`
- `MaDanhMucCV`
- `MaMucDo`
- `TenCongViec`
- `MoTaCongViec`
- `NgayBatDauCongViec`
- `NgayKetThucCVDuKien`
- `NgayKetThucCVThucTe`
- `NgayTaoCongViec`
- `TrangThaiCongViec`

### `CHI_PHI`

Cột liên quan nhánh duyệt:

- `MaChiPhi`
- `MaCongViec`
- `MaNganSach`
- `NoiDungChiPhi`
- `SoTienDaChi`
- `NgayChi`
- `TrangThaiChiPhi`

### `NHAT_KY_QUAN_LY_DU_AN`

Cột liên quan:

- `MaNhatKyQLDA`
- `MaDuAn`
- `MaNguoiDung`
- `NkHanhDongQLDA nvarchar(255)`
- `NkThoiGianQLDA`
- `QLDATuNgay`
- `QLDADenNgay`

Không có `MaDeXuatCV`; mã đề xuất chỉ được nhúng trong chuỗi hành động.

### `NHAT_KY_CHI_PHI`

Chỉ dùng khi duyệt tạo chi phí:

- `MaNhatKyCP`
- `MaCongViec`
- `MaChiPhi`
- `NkSoTienDaChi`
- `NkNgayChi`
- `NkTrangThaiChiPhi`
- `HanhDongNKCP nvarchar(255)`
- `ThoiGianNKCP`

## 11. Bảng đối chiếu dữ liệu nghiệp vụ

| Dữ liệu nghiệp vụ | ViewModel | Entity property | Cột SQL | Được lưu không | Ghi chú |
| --- | --- | --- | --- | --- | --- |
| Mã đề xuất | `MaDeXuatCV` | `MaDeXuatCV` | `MaDeXuatCV` | Có | Khóa chính |
| Dự án | `MaDuAn`, `TenDuAn` | `MaDuAn` | `MaDuAn` | Có | FK tới `DU_AN` |
| Danh mục công việc | `MaDanhMucCV`, `TenDanhMucCongViec` | `MaDanhMucCV` | `MaDanhMucCV` | Có | FK tới `DANH_MUC_CONG_VIEC` |
| Tên công việc | `TenCongViecDeXuat` | `TenCongViecDeXuat` | `TenCongViecDeXuat` | Có | Tạo thành `CONG_VIEC.TenCongViec` khi duyệt |
| Mô tả | `MoTaCongViecDeXuat` | `MoTaCongViecDeXuat` | `MoTaCongViecDeXuat` | Có | Tạo thành `CONG_VIEC.MoTaCongViec` khi duyệt |
| Lý do đề xuất | Không có | Không có | Không có | Không | Không nhầm với `DE_XUAT_NGAN_SACH.LyDoDeXuat` |
| Chi phí đề xuất | `ChiPhiDeXuat` | `ChiPhiDeXuat` | `ChiPhiDeXuat` | Có | Tạo thành `CHI_PHI.SoTienDaChi` khi duyệt |
| Trạng thái | `TrangThaiCongViecDeXuat` | `TrangThaiCongViecDeXuat` | `TrangThaiCongViecDeXuat` | Có | `ChoDuyet`, `DaDuyet`, `TuChoi`, `DaHuy` |
| Người duyệt | `NguoiDungDuyet` | `MaNguoiDungDuyet` | `MaNguoiDungDuyet` | Có | Cập nhật khi duyệt/từ chối |
| Ngày duyệt | `NgayDuyetDeXuatCongViec` | `NgayDuyetDeXuatCongViec` | `NgayDuyetDeXuatCongViec` | Có | Cập nhật khi duyệt/từ chối |
| Lý do từ chối | Không có | Không có | Không có | Không trong đề xuất | Chỉ có thể nằm trong log chuỗi |
| Nội dung nhật ký | Không có ViewModel đọc lại | `NhatKyQuanLyDuAn.NkHanhDongQLDA` | `NHAT_KY_QUAN_LY_DU_AN.NkHanhDongQLDA` | Có, nếu gửi form chi tiết | Không có FK `MaDeXuatCV`, giới hạn 255 |

## 12. Đối chiếu Entity, Migration và SQL script

Kết quả đối chiếu:

- Entity `DeXuatCongViec` không có property lý do từ chối.
- `QuanLyDuAnDbContext` không cấu hình property/cột lý do từ chối cho `DE_XUAT_CONG_VIEC`.
- Migration khởi tạo `20260527125053_Init.cs` tạo `DE_XUAT_CONG_VIEC` không có cột lý do từ chối.
- Migration Designer và `QuanLyDuAnDbContextModelSnapshot.cs` cũng không có property lý do từ chối cho `DeXuatCongViec`.
- `quanlyduan.sql` không có cột lý do từ chối trong `DE_XUAT_CONG_VIEC`.
- `quanlyduan.sql` có `LyDoDeXuat` trong `DE_XUAT_NGAN_SACH`, không thuộc đề xuất công việc.

Kết luận: source/migration/snapshot/SQL script đang đồng bộ theo thiết kế hiện tại là không lưu lý do từ chối trong `DE_XUAT_CONG_VIEC`. Không có bằng chứng SQL script cũ hơn source về riêng cột lý do từ chối.

Runtime database chưa xác nhận được do lỗi kết nối `sqlcmd`. Câu SQL đọc-only cần chạy khi kết nối được:

```sql
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DE_XUAT_CONG_VIEC'
ORDER BY ORDINAL_POSITION;

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'NHAT_KY_QUAN_LY_DU_AN'
ORDER BY ORDINAL_POSITION;
```

## 13. Kiểm tra nhật ký và audit

Nhánh từ chối đề xuất công việc chỉ ghi `NHAT_KY_QUAN_LY_DU_AN`.

Đặc điểm audit:

- Có lưu nội dung lý do người dùng nhập nếu submit form chi tiết và `lyDo` không rỗng.
- Lý do nằm trong chuỗi `NkHanhDongQLDA`, không có cột riêng.
- Không có `MaDeXuatCV` FK trong bảng nhật ký.
- Mã đề xuất chỉ nhúng dạng text trong nội dung hành động.
- Có thể truy vấn tương đối bằng `LIKE`, nhưng không đảm bảo chính xác tuyệt đối như FK.
- Giới hạn `nvarchar(255)` áp dụng cho toàn chuỗi hành động, không chỉ phần lý do.
- Một đề xuất có thể có nhiều log chung theo dự án, dễ nhầm nếu chỉ tìm chuỗi.
- View hiện tại không đọc lại nhật ký để hiển thị lý do.
- Người đề xuất không có màn hình đọc lại lý do từ nhật ký trong module đề xuất công việc hiện tại.

Không được kết luận lý do đã là dữ liệu nghiệp vụ bền vững chỉ vì có log. Theo source, log là nơi duy nhất có thể giữ lý do.

## 14. Ma trận dữ liệu khi duyệt và từ chối

| Bảng | Khi duyệt | Khi từ chối |
| --- | --- | --- |
| `DE_XUAT_CONG_VIEC` | Update `TrangThaiCongViecDeXuat = DaDuyet`, `MaNguoiDungDuyet`, `NgayDuyetDeXuatCongViec` | Update `TrangThaiCongViecDeXuat = TuChoi`, `MaNguoiDungDuyet`, `NgayDuyetDeXuatCongViec` |
| `CONG_VIEC` | Insert công việc mới từ đề xuất, gắn `MaDeXuatCV` | Không thay đổi |
| `CHI_PHI` | Insert chi phí từ `ChiPhiDeXuat`, gắn ngân sách active | Không thay đổi |
| `NGAN_SACH` | Không tạo ngân sách mới; đọc ngân sách active. Có thể không đổi ngân sách active | Không thay đổi |
| `DU_AN` | Có thể chuyển `ChoXacNhanHoanThanh` về `DangThucHien` | Không thay đổi |
| `NHAT_KY_CHI_PHI` | Insert log tạo chi phí | Không thay đổi |
| `NHAT_KY_QUAN_LY_DU_AN` | Insert log duyệt đề xuất, và log chuyển trạng thái dự án nếu có | Insert log từ chối, có nối lý do nếu gửi |

## 15. Lỗi hoặc rủi ro

| Mức độ | File/vị trí | Hành vi hiện tại | Hậu quả |
| --- | --- | --- | --- |
| Cao | `Views/DuyetDeXuatCongViec/_Table.cshtml`, form từ chối nhanh | Có hidden `lyDo=""` | Người duyệt có thể từ chối không lưu lý do dù giao diện có form chi tiết nhập lý do |
| Cao | `DeXuatCongViec`, `DE_XUAT_CONG_VIEC` | Không có property/cột lý do từ chối | Lý do không phải dữ liệu nghiệp vụ của đề xuất, không hiển thị lại ổn định |
| Trung bình | `DyetDeXuatCongViecService.RejectAsync` | Lý do chỉ ghép vào `NkHanhDongQLDA` | Khó truy vấn chính xác theo mã đề xuất; không có FK |
| Trung bình | `NHAT_KY_QUAN_LY_DU_AN.NkHanhDongQLDA` | Giới hạn 255 ký tự | Lý do dài có nguy cơ lỗi lưu hoặc mất dữ liệu |
| Trung bình | `Views/DuyetDeXuatCongViec/_Table.cshtml` | Hiển thị hardcode `Không có` cho lý do từ chối/ghi chú | Manager nhìn lại sau reload không thấy lý do đã nhập |
| Trung bình | `Views/DeXuatCongViec/_Table.cshtml` | Không có vùng hiển thị lý do từ chối | Người đề xuất không xem được lý do từ chối trong màn hình đề xuất |
| Trung bình | `DuyetDeXuatCongViecController.TuChoi` | Không có `[ValidateAntiForgeryToken]`; `Program.cs` không bật global auto-validate | Form có thể sinh token nhưng action không bắt buộc validate token |
| Trung bình | `DyetDeXuatCongViecService.RejectAsync` | Không có transaction riêng | Nhánh hiện chỉ update đề xuất và thêm log trong một `SaveChangesAsync`; rủi ro thấp hơn duyệt nhưng không nhất quán với nhánh duyệt |
| Thấp | `DuyetDeXuatCongViecController.TuChoi` | Redirect không giữ `pageNumber`, `pageSize` | Sau thao tác có thể quay về trang mặc định thay vì trang đang xem |
| Thấp | `Views/DuyetDeXuatCongViec/_Table.cshtml`, `Index.cshtml`, `_Filter.cshtml`, `wwwroot/js/approval/index.js`, `Program.cs` | Một số file source đang có dấu hiệu ký tự tiếng Việt lỗi mã hóa khi đọc ra terminal | Cần xử lý riêng nếu sửa UI; tài liệu này không sửa các file đó |
| Thấp | `DyetDeXuatCongViecService` | Tên class/file thiếu chữ `u` trong `Duyet` | Không lỗi runtime do DI đúng, nhưng dễ gây nhầm khi tìm source |

Không thấy các rủi ro sau trong nhánh từ chối hiện tại:

- Từ chối tạo nhầm `CONG_VIEC`.
- Từ chối tạo nhầm `CHI_PHI`.
- Từ chối làm thay đổi ngân sách active.
- Từ chối ngoài phạm vi Manager đúng dự án: service đã kiểm tra `DU_AN.MaNguoiDung == currentUserId`.
- Từ chối lại đề xuất đã xử lý: service đã chặn nếu không còn `ChoDuyet`.
- Ghi đè lý do đề xuất: `DE_XUAT_CONG_VIEC` không có `LyDoDeXuat`, nên không có ghi đè trong bảng này.

## 16. Các phương án TO-BE

### Phương án A: Thêm cột `LyDoTuChoi`

Ưu điểm:

- Lý do từ chối là dữ liệu nghiệp vụ trực tiếp của đề xuất.
- Manager và người đề xuất có thể xem lại dễ dàng.
- Query/report rõ ràng, không phụ thuộc chuỗi log.
- Không nhầm với lý do đề xuất hoặc log chung.

Nhược điểm:

- Cần sửa schema, migration, script SQL.
- Cần xử lý dữ liệu cũ: các đề xuất đã từ chối trước đây chỉ có thể khôi phục lý do bằng cách parse log nếu muốn backfill, nhưng không đảm bảo chính xác.

File cần sửa:

- `Models/Entities/DeXuatCongViec.cs`
- `Data/QuanLyDuAnDbContext.cs`
- Migration mới
- `quanlyduan.sql` hoặc script cập nhật DB
- `ViewModels/DuyetDeXuatCongViec/DuyetDeXuatCongViecItemViewModel.cs`
- `ViewModels/DeXuatCongViec/DeXuatCongViecItemViewModel.cs`
- `Services/Implementations/DyetDeXuatCongViecService.cs`
- `Services/Implementations/DeXuatCongViecService.cs`
- `Views/DuyetDeXuatCongViec/_Table.cshtml`
- `Views/DeXuatCongViec/_Table.cshtml`

### Phương án B: Dùng cột sẵn có

Không khuyến nghị với source hiện tại vì `DE_XUAT_CONG_VIEC` không có cột chung phù hợp như `GhiChu`, `NoiDung`, `NhanXet`, `LyDo`. Dùng cột khác không tồn tại sẽ buộc vẫn phải sửa database. Không được dùng nhầm `DE_XUAT_NGAN_SACH.LyDoDeXuat`.

### Phương án C: Chỉ lưu trong nhật ký

Ưu điểm:

- Không cần sửa database nếu chấp nhận log hiện tại.
- Service đã có sẵn logic nối lý do vào `NHAT_KY_QUAN_LY_DU_AN`.

Nhược điểm:

- Không có FK `MaDeXuatCV`.
- Chỉ truy vấn bằng text.
- Giới hạn 255 ký tự.
- View hiện tại không đọc lại log.
- Người đề xuất không xem được lý do.
- Không phù hợp nếu lý do từ chối là dữ liệu nghiệp vụ cần hiển thị lại.

Nếu chọn phương án này vẫn cần sửa service/viewmodel/view để đọc log và hiển thị lại, đồng thời nên chuẩn hóa format log hoặc bổ sung mã đề xuất có thể parse được.

### Phương án D: Cột riêng và nhật ký

Ưu điểm:

- `DE_XUAT_CONG_VIEC.LyDoTuChoi` là source of truth.
- `NHAT_KY_QUAN_LY_DU_AN` giữ vai trò audit.
- View đọc từ cột riêng, không parse log.
- Có thể giới hạn độ dài rõ ràng, ví dụ `nvarchar(500)`.

Nhược điểm:

- Có trùng dữ liệu giữa entity và log.
- Cần transaction hoặc tối thiểu đảm bảo update đề xuất và ghi log cùng một `SaveChangesAsync`.
- Cần migration và cập nhật script.

Đây là phương án phù hợp nhất nếu nghiệp vụ yêu cầu giữ ô nhập lý do từ chối.

### Phương án E: Bỏ chức năng nhập lý do từ chối

Ưu điểm:

- Không sửa database.
- Không tạo migration.
- Không sửa Entity/DbContext/SQL.
- Loại bỏ hiểu nhầm UI có lý do nhưng không lưu bền vững.
- Đưa module công việc gần hơn với module duyệt ngân sách hiện tại, vốn chỉ có nút từ chối không lý do.

Hạn chế:

- Không còn dữ liệu giải thích cho người đề xuất.
- Không còn audit chi tiết lý do từ chối.
- Nếu tài liệu/use case đang mô tả "nhập lý do", cần cập nhật lại tài liệu nghiệp vụ.

Phạm vi tối thiểu nếu chọn phương án E:

- Bỏ input `name="lyDo"` trong form chi tiết.
- Bỏ form từ chối kèm lý do hoặc hợp nhất còn một nút từ chối.
- Bỏ hidden `lyDo=""` không cần thiết.
- Bỏ tham số `lyDo` khỏi controller.
- Bỏ tham số `lyDo` khỏi service interface và implementation.
- Log chỉ ghi nội dung chung `Từ chối đề xuất công việc #{id}`.
- Không sửa Entity.
- Không sửa DbContext.
- Không tạo migration.
- Không sửa SQL.

Phương án này phù hợp nếu quyết định nghiệp vụ là không cần lý do từ chối. Nếu giao diện vẫn muốn người duyệt nhập lý do và người đề xuất xem lại, phương án này không phù hợp.

## 17. Đề xuất phù hợp nhất

Nếu mục tiêu nghiệp vụ là giữ ô nhập lý do từ chối, phương án phù hợp nhất là Phương án D: thêm cột riêng `LyDoTuChoi` trong `DE_XUAT_CONG_VIEC` và vẫn ghi nhật ký audit. Cột riêng là source of truth để hiển thị lại; nhật ký chỉ là lịch sử thao tác.

Nếu mục tiêu là không sửa database ở bước sau, phương án phù hợp nhất là Phương án E: bỏ chức năng nhập lý do từ chối khỏi UI và hợp nhất còn một nút từ chối, vì source hiện tại không có nơi lưu lý do trong đề xuất.

Với source hiện tại, không nên giữ UI nhập lý do nếu không thêm nơi lưu rõ ràng hoặc không hiển thị lại từ log, vì người dùng dễ hiểu rằng lý do đã được lưu như dữ liệu nghiệp vụ.

## 18. File dự kiến cần sửa ở bước sau

### Nếu chọn Phương án D

- Controller: `Controllers/DuyetDeXuatCongViecController.cs`
- Service interface: `Services/Interfaces/IDuyetDeXuatCongViecService.cs`
- Service implementation: `Services/Implementations/DyetDeXuatCongViecService.cs`
- ViewModel duyệt: `ViewModels/DuyetDeXuatCongViec/DuyetDeXuatCongViecItemViewModel.cs`
- ViewModel người đề xuất: `ViewModels/DeXuatCongViec/DeXuatCongViecItemViewModel.cs`
- View duyệt: `Views/DuyetDeXuatCongViec/_Table.cshtml`
- View người đề xuất: `Views/DeXuatCongViec/_Table.cshtml`
- Entity: `Models/Entities/DeXuatCongViec.cs`
- DbContext: `Data/QuanLyDuAnDbContext.cs`
- Migration mới
- SQL script cập nhật database hoặc cập nhật `quanlyduan.sql`
- Tài liệu: `docs/duyetdexuatcongviec.md`, `docs/usecase.md` nếu cần đồng bộ use case

### Nếu chọn Phương án E, phạm vi tối thiểu không sửa database

- Controller: `Controllers/DuyetDeXuatCongViecController.cs`
- Service interface: `Services/Interfaces/IDuyetDeXuatCongViecService.cs`
- Service implementation: `Services/Implementations/DyetDeXuatCongViecService.cs`
- View: `Views/DuyetDeXuatCongViec/_Table.cshtml`
- Tài liệu: `docs/duyetdexuatcongviec.md`, `docs/usecase.md` nếu cần bỏ mô tả nhập lý do

Không cần sửa:

- `Models/Entities/DeXuatCongViec.cs`
- `Data/QuanLyDuAnDbContext.cs`
- Migration
- `quanlyduan.sql`
- Database runtime

## 19. Kết luận bắt buộc

- Giao diện hiện có ô nhập lý do từ chối: Có, trong form chi tiết của `Views/DuyetDeXuatCongViec/_Table.cshtml`.
- Có bao nhiêu form/nút từ chối: Có 2 form từ chối cho mỗi đề xuất đang chờ duyệt: một nút nhanh gửi `lyDo=""`, một form chi tiết có input `name="lyDo"`.
- Controller có nhận lý do không: Có, `DuyetDeXuatCongViecController.TuChoi(..., string? lyDo, ...)`.
- Service có nhận lý do không: Có, `IDuyetDeXuatCongViecService.RejectAsync(int maDeXuatCv, string? lyDo)` và `DyetDeXuatCongViecService.RejectAsync(...)`.
- Lý do được lưu ở đâu: Chỉ có thể được lưu trong `NHAT_KY_QUAN_LY_DU_AN.NkHanhDongQLDA` nếu submit form chi tiết có lý do. Không lưu trong `DE_XUAT_CONG_VIEC`.
- Nếu bấm nút từ chối nhanh: lý do bị mất ngay tại View vì form gửi hidden `lyDo` rỗng.
- Nếu nhập lý do ở form chi tiết: Controller nhận, Service nhận, Service trim và ghi vào chuỗi nhật ký; sau đó lý do không được đưa vào Entity đề xuất hoặc ViewModel hiển thị.
- Lý do có được hiển thị lại không: Không. View duyệt hardcode `Không có`; màn hình đề xuất không có vùng lý do.
- Người đề xuất có xem được lý do không: Không theo source hiện tại.
- SQL có thiếu cột hay đang đồng bộ đúng thiết kế: Entity, DbContext, Migration, Snapshot và `quanlyduan.sql` đồng bộ theo thiết kế hiện tại là không có cột lý do từ chối trong `DE_XUAT_CONG_VIEC`.
- Có cần sửa database không: Có nếu muốn lưu lý do từ chối như dữ liệu nghiệp vụ và hiển thị lại ổn định.
- Nếu không sửa database thì có thể bỏ lý do từ chối không: Có. Phương án E là bỏ input lý do và chỉ giữ nút từ chối, không cần sửa Entity/DbContext/Migration/SQL.
- File phải sửa nếu chọn phương án bỏ lý do: `DuyetDeXuatCongViecController.cs`, `IDuyetDeXuatCongViecService.cs`, `DyetDeXuatCongViecService.cs`, `Views/DuyetDeXuatCongViec/_Table.cshtml`, và tài liệu liên quan.
- Phương án phù hợp nhất với source hiện tại: Nếu giữ yêu cầu nhập lý do thì chọn Phương án D. Nếu không muốn sửa database thì chọn Phương án E để tránh UI đánh lừa người dùng.

## 20. Trạng thái kiểm chứng

- Đã đọc source thực tế và script database.
- Đã thử truy vấn metadata runtime DB bằng `sqlcmd`, nhưng chưa thành công do lỗi kết nối/encryption của client.
- Chưa sửa source runtime.
- Chưa sửa View.
- Chưa sửa Controller.
- Chưa sửa Service.
- Chưa sửa Entity.
- Chưa tạo migration.
- Chưa chạy `Update-Database`.
- Chưa sửa SQL.
- Chưa thay đổi dữ liệu database.

## 21. Trạng thái triển khai

Ngày triển khai: 18/06/2026.

Phương án được chọn: Phương án E - bỏ chức năng nhập lý do từ chối trong luồng duyệt đề xuất công việc.

Lý do không sửa database: source, Entity, DbContext, Migration, Model Snapshot và `quanlyduan.sql` hiện không có cột lưu lý do từ chối trong `DE_XUAT_CONG_VIEC`. Yêu cầu triển khai lần này là đơn giản hóa nghiệp vụ, không thêm cột, không tạo migration và không thay đổi database runtime.

File đã sửa:

- `QuanLyDuAn/QuanLyDuAn/Controllers/DuyetDeXuatCongViecController.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDuyetDeXuatCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DyetDeXuatCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/DuyetDeXuatCongViec/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/DuyetDeXuatCongViec/index.css`
- `docs/duyetdexuatcongviec.md`

Thay đổi giao diện:

- Đã bỏ input `name="lyDo"` trong form chi tiết.
- Đã bỏ hidden input `name="lyDo" value=""` trong nút từ chối nhanh.
- Đã bỏ form từ chối thứ hai trong phần chi tiết.
- Đã bỏ nút `Gửi từ chối kèm lý do`.
- Đã bỏ dòng `Lý do từ chối / ghi chú`.
- Đã bỏ nội dung hardcode `Không có` dùng cho lý do từ chối.
- Đã hợp nhất còn một nút `Từ chối` cho mỗi đề xuất đang `ChoDuyet` và user có quyền `DuyetDeXuatCongViec.Duyet`.
- Nút `Từ chối` vẫn dùng form POST thường, class `js-confirm-submit`, và thông báo xác nhận `Bạn có chắc chắn muốn từ chối đề xuất công việc này không?`.
- Đã giữ filter và pagination cho nhánh từ chối bằng `locMaDuAn`, `locTrangThai`, `pageNumber`, `pageSize`.
- Đã xóa CSS module-local chỉ phục vụ form nhập lý do: `.duyet-de-xuat-cong-viec-reject-form`.

Chữ ký Controller:

```csharp
// Trước
public async Task<IActionResult> TuChoi(int maDeXuatCv, string? lyDo, int? locMaDuAn, string? locTrangThai)

// Sau
public async Task<IActionResult> TuChoi(
    int maDeXuatCv,
    int? locMaDuAn,
    string? locTrangThai,
    int pageNumber = 1,
    int pageSize = 20)
```

Chữ ký Service:

```csharp
// Trước
Task RejectAsync(int maDeXuatCv, string? lyDo);
public async Task RejectAsync(int maDeXuatCv, string? lyDo)

// Sau
Task RejectAsync(int maDeXuatCv);
public async Task RejectAsync(int maDeXuatCv)
```

Nội dung nhật ký sau khi sửa:

```csharp
$"Từ chối đề xuất công việc #{deXuat.MaDeXuatCV}"
```

Không còn nối chuỗi `. Lý do: ...`.

Các kiểm tra đã thực hiện:

- Đã tìm trong các file thuộc module duyệt đề xuất công việc và không còn `lyDo`, `LyDoTuChoi`, `Nhập lý do từ chối`, `Gửi từ chối kèm lý do`, `Lý do từ chối / ghi chú`, `duyet-de-xuat-cong-viec-reject-form`.
- Đã tìm toàn project và không còn call site `RejectAsync(maDeXuatCv, lyDo)` hoặc chữ ký `RejectAsync(int maDeXuatCv, string? lyDo)` cho module `DuyetDeXuatCongViec`.
- Các `RejectAsync` còn nhận lý do trong module khác như `DuyetYeuCauDoiQuanLy` được giữ nguyên.
- Kiểm tra quyền vẫn nằm ở `DuyetDeXuatCongViecController.TuChoi`: thiếu `DuyetDeXuatCongViec.Duyet` thì `Forbid()`.
- Kiểm tra Manager đúng dự án vẫn nằm ở `DyetDeXuatCongViecService.EnsureIsProjectManagerAsync(...)`.
- Kiểm tra trạng thái vẫn dùng `IsPending(...)`, nên `DaDuyet`, `TuChoi`, `DaHuy` đều không được từ chối lại.
- Nhánh từ chối vẫn chỉ update `DE_XUAT_CONG_VIEC` và thêm `NHAT_KY_QUAN_LY_DU_AN`; không có code tạo `CONG_VIEC`, không có code tạo `CHI_PHI`, không sửa `NGAN_SACH`, không sửa `DU_AN`.
- `wwwroot/js/approval/index.js` không có logic phụ thuộc lý do từ chối nên không sửa.

Kết quả build:

```text
dotnet build QuanLyDuAn/QuanLyDuAn.sln
Build succeeded.
0 Error(s)
2 Warning(s)
```

Hai warning là `CS1998` trong `Services/Implementations/FileTienDoCongViecService.cs`, không thuộc phạm vi thay đổi.

Xác nhận phạm vi:

- Không sửa Entity `DeXuatCongViec`.
- Không sửa Entity `CongViec`, `ChiPhi`, `NganSach`.
- Không sửa `QuanLyDuAnDbContext`.
- Không sửa migration cũ.
- Không sửa Model Snapshot.
- Không tạo migration.
- Không chạy `Update-Database`.
- Không sửa `quanlyduan.sql`.
- Không chạy lệnh SQL thay đổi cấu trúc hoặc dữ liệu.
- Không thay đổi database runtime.
- Không sửa nhánh `ApproveAsync`.
- Không sửa chức năng tạo/hủy đề xuất công việc.
- Không phát sinh lỗi font tiếng Việt trong các file vừa sửa theo kiểm tra bằng `rg` với các mẫu mojibake đã nêu trong yêu cầu.
