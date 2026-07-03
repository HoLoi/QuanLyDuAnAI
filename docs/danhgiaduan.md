# Đánh giá dự án - workflow Manager xác nhận

## 1. Tổng quan thay đổi

Module `DanhGiaDuAn` đã được đổi từ luồng cũ:

`Nhap -> ChoDuyet -> DaDuyet/TuChoi`

sang luồng nghiệp vụ mới:

`Nhap -> DaDuyet`

Trong giao diện, trạng thái DB `DaDuyet` được hiểu và hiển thị là **Đã xác nhận**. Các cột DB cũ vẫn giữ nguyên để không sửa schema:

- `DANH_GIA_DU_AN.MaNguoiDungDuyet` được dùng như **người xác nhận**.
- `DANH_GIA_DU_AN.NgayDuyetDanhGiaDA` được dùng như **ngày xác nhận**.
- `DANH_GIA_DU_AN.LyDoTuChoiDanhGiaDA` không còn được tạo mới trong workflow đánh giá dự án.

Lý do đổi nghiệp vụ: Admin chỉ quản trị hệ thống, không trực tiếp theo dõi tiến độ, chi phí, nhân sự và kết quả thực hiện dự án. Manager/phụ trách dự án là người có đủ ngữ cảnh để tạo và xác nhận đánh giá dự án.

## 2. Workflow mới

| Bước | Người thao tác | Điều kiện | Kết quả |
|---|---|---|---|
| Tạo/lưu nháp | Manager phụ trách dự án | User là Manager, `DuAn.MaNguoiDung == currentUserId`, có quyền `DanhGiaDuAn.DanhGia` hoặc `DanhGiaDuAn.Sua` | Lưu `DANH_GIA_DU_AN`, trạng thái `Nhap` |
| Sửa nháp | Manager đã tạo và phụ trách dự án | Trạng thái hiện tại là `Nhap` | Cập nhật điểm/nhận xét/chi tiết, vẫn là `Nhap` |
| Xác nhận đánh giá | Manager đã tạo và phụ trách dự án | Trạng thái `Nhap`, dự án đủ điều kiện, có chi tiết tiêu chí | Set `TrangThaiDanhGiaDA = DaDuyet`, lưu người/ngày xác nhận |
| Xem/giám sát | Admin có quyền xem | Có quyền `DanhGiaDuAn.Xem` | Xem danh sách/chi tiết, không có nút xác nhận/từ chối |

Luồng mới không tạo thêm trạng thái `ChoDuyet` hoặc `TuChoi`. Dữ liệu cũ nếu còn `ChoDuyet`/`TuChoi` vẫn hiển thị an toàn là **Chờ xác nhận cũ** hoặc **Từ chối cũ**.

## 3. Quyền và scope

| Vai trò | Xem danh sách | Tạo/sửa nháp | Xác nhận | Ghi chú |
|---|---|---|---|---|
| Manager | Có, theo dự án mình phụ trách | Có, nếu `DuAn.MaNguoiDung == currentUserId` | Có, chỉ bản `Nhap` do mình tạo trong dự án mình phụ trách | Không sửa bản đã xác nhận |
| Admin | Có, toàn bộ danh sách theo quyền xem | Không | Không | Admin chỉ giám sát, không duyệt nội dung |
| Employee | Không có luồng tạo/sửa/xác nhận | Không | Không | Không đổi nghiệp vụ nhân viên |

Permission dùng cho xác nhận là permission tác nghiệp hiện có `DanhGiaDuAn.DanhGia` hoặc `DanhGiaDuAn.Sua`, không dùng `DanhGiaDuAn.Duyet` cho workflow mới.

## 4. File đã sửa

| File | Thay đổi chính |
|---|---|
| `QuanLyDuAn/QuanLyDuAn/Controllers/DanhGiaDuAnController.cs` | Thêm action POST `XacNhan`; action cũ `GuiDuyet`, `Duyet`, `TuChoi` chỉ redirect về Index với thông báo workflow đã thay đổi; export đổi nhãn `Người xác nhận` và trạng thái `Đã xác nhận`. |
| `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDanhGiaDuAnService.cs` | Thêm `Task XacNhanAsync(int maDanhGiaDuAn)`. |
| `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs` | Thêm nghiệp vụ xác nhận Manager; Admin được xem danh sách nhưng không xác nhận; bỏ tạo mới `ChoDuyet`; bản xác nhận set `DaDuyet`, `MaNguoiDungDuyet`, `NgayDuyetDanhGiaDA`. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnItemViewModel.cs` | Thêm cờ `CoTheXacNhan`. |
| `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnFormViewModel.cs` | Thêm cờ `CoTheXacNhan`. |
| `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_Table.cshtml` | Bỏ nút gửi duyệt/duyệt/từ chối; thêm nút `Xác nhận đánh giá` gọi action `XacNhan`. |
| `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_Form.cshtml` | Đổi nút lưu thành `Lưu nháp`; đổi nút gửi duyệt thành `Xác nhận đánh giá`; thêm nhắc lưu nháp trước khi xác nhận. |
| `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_TrangThaiDanhGiaBadge.cshtml` | Hiển thị `DaDuyet` là `Đã xác nhận`, `ChoDuyet` là `Chờ xác nhận cũ`, `TuChoi` là `Từ chối cũ`. |
| `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_Filter.cshtml` | Đổi label trạng thái trong bộ lọc sang ngôn ngữ xác nhận. |
| `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_SummaryCards.cshtml` | Đổi summary `Đã duyệt` thành `Đã xác nhận`, trạng thái cũ hiển thị rõ là dữ liệu cũ. |
| `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/ChiTiet.cshtml` | Hiển thị thêm `Người đánh giá`, `Người xác nhận`, `Ngày xác nhận`. |
| `docs/danhgiaduan.md` | Cập nhật tài liệu sau triển khai. |

Không sửa database, không tạo migration, không đổi schema, không sửa file SQL, không seed lại role/permission.

## 5. Chi tiết nghiệp vụ service

Method mới: `DanhGiaDuAnService.XacNhanAsync(int maDanhGiaDuAn)`.

Điều kiện bắt buộc:

- User đã đăng nhập.
- User có `Permissions.DanhGiaDuAn.DanhGia` hoặc `Permissions.DanhGiaDuAn.Sua`.
- User là Manager.
- User không phải Admin tác nghiệp.
- Dự án tồn tại và chưa xóa.
- `DuAn.MaNguoiDung == currentUserId`.
- Bản đánh giá thuộc dự án đó.
- Bản đánh giá do chính Manager đó tạo: `DanhGiaDuAn.MaNguoiDung == currentUserId`.
- Trạng thái hiện tại là `Nhap`.
- Dự án đủ điều kiện xác nhận theo rule hiện có: `HoanThanh`, `ChoXacNhanHoanThanh`, hoặc `Archived`.
- Có ít nhất một chi tiết tiêu chí chưa xóa trong `CT_DANH_GIA_DU_AN`.

Khi xác nhận:

- `TrangThaiDanhGiaDA = TrangThai.DaDuyet`.
- `MaNguoiDungDuyet = currentUserId`.
- `NgayDuyetDanhGiaDA = DateTime.Now`.
- `LyDoTuChoiDanhGiaDA = null`.
- `NgayDanhGiaDA = DateTime.Now`.

## 6. Xử lý dữ liệu cũ

Không tự động cập nhật dữ liệu cũ.

| Trạng thái DB cũ | Hiển thị mới | Thao tác mới |
|---|---|---|
| `ChoDuyet` | `Chờ xác nhận cũ` | Không tạo thêm; không xác nhận trực tiếp trong luồng mới vì chỉ xác nhận bản `Nhap` |
| `TuChoi` | `Từ chối cũ` | Không tạo thêm; không còn workflow từ chối |
| `DaDuyet` | `Đã xác nhận` | Khóa sửa |
| `Nhap` | `Nháp` | Manager có thể sửa hoặc xác nhận |

## 7. Test case cần chạy

| Mã test | Tình huống | Kết quả mong đợi |
|---|---|---|
| DGDA-01 | Manager tạo đánh giá dự án mình quản lý | Trạng thái `Nhap`, có chi tiết tiêu chí |
| DGDA-02 | Manager sửa bản nháp | Lưu thành công, vẫn `Nhap` |
| DGDA-03 | Manager xác nhận bản nháp | DB lưu `DaDuyet`, UI hiển thị `Đã xác nhận`, có người/ngày xác nhận |
| DGDA-04 | Manager sửa bản đã xác nhận | Bị chặn, không hiện nút sửa/xác nhận |
| DGDA-05 | Manager xem danh sách | Không còn nút `Gửi duyệt` cũ |
| DGDA-06 | Admin xem danh sách | Không có nút duyệt/từ chối/xác nhận |
| DGDA-07 | Admin tạo/sửa/xác nhận | Bị chặn bởi permission/service |
| DGDA-08 | Employee tạo/sửa/xác nhận | Bị chặn |
| DGDA-09 | Dữ liệu cũ `ChoDuyet` hoặc `TuChoi` | Index/ChiTiet không lỗi, hiển thị trạng thái cũ rõ ràng |
| DGDA-10 | Font tiếng Việt | View và docs hiển thị đúng UTF-8 |
| DGDA-11 | Export/chi tiết | Nhãn là `Người xác nhận`, trạng thái `Đã xác nhận` |
| DGDA-12 | Đánh giá nhân viên | Không bị đổi source/module |

## 8. Kết quả kiểm tra

Đã chạy:

```powershell
dotnet build .\QuanLyDuAn\QuanLyDuAn.sln
```

Kết quả:

- Build succeeded.
- `0 Error(s)`.
- `2 Warning(s)` hiện có ở `FileTienDoCongViecService.cs` về async method thiếu `await`, không thuộc phạm vi thay đổi `DanhGiaDuAn`.

Lần build đầu trong sandbox bị chặn restore NuGet (`api.nuget.org:443`), sau đó chạy lại ngoài sandbox theo quyền được cấp và build thành công.

## 9. Lưu ý

- Các action cũ `GuiDuyet`, `Duyet`, `TuChoi` được giữ để tránh lỗi route cũ, nhưng chỉ redirect về Index và không còn tạo/cập nhật workflow duyệt cũ.
- Các property cũ `CoTheGuiDuyet`, `CoTheDuyet`, `CoTheTuChoi` vẫn còn trong ViewModel để tránh phá vỡ contract nội bộ, nhưng service set `false` và view `DanhGiaDuAn` không dùng chúng cho nút thao tác mới.
- Không đổi module `DanhGiaNhanVien`, AI phân tích nguyên nhân trễ, đề xuất công việc, đề xuất ngân sách hoặc tiến độ.

## 10. Cập nhật ràng buộc điểm đánh giá 1-10

Ngày cập nhật: 2026-07-04.

Phạm vi: module `DanhGiaDuAn`, không sửa database, không tạo migration, không đổi schema và không đổi workflow `Nhap -> DaDuyet` (UI hiểu là `Nháp -> Đã xác nhận`).

Các lớp đã ràng buộc:

| Lớp | File/method | Nội dung |
|---|---|---|
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnTieuChiViewModel.cs` | `DiemDanhGiaDA` có `[Required(ErrorMessage = "Vui lòng nhập điểm đánh giá.")]` và `[Range(1, 10, ErrorMessage = "Điểm đánh giá phải nằm trong khoảng từ 1 đến 10.")]`. |
| ViewModel | `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnFormViewModel.cs` | `TieuChi` có `[MinLength(1, ErrorMessage = "Vui lòng nhập ít nhất một tiêu chí đánh giá.")]`. |
| Razor View | `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_Form.cshtml` | Input điểm dùng `type="number"`, `min="1"`, `max="10"`, `step="1"`, `required` và hiển thị lỗi bằng `asp-validation-for`. |
| Controller | `QuanLyDuAn/QuanLyDuAn/Controllers/DanhGiaDuAnController.cs`, action `Luu` | Đã có kiểm tra `ModelState.IsValid`; khi invalid không gọi service lưu. |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs`, method `LuuDanhGiaAsync` | Gọi `KiemTraHopLeDuLieuTieuChi(...)` trước khi lưu; điểm ngoài 1-10 bị chặn server-side. |
| Service | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs`, method `XacNhanAsync` | Kiểm tra lại điểm đã lưu trong `CT_DANH_GIA_DU_AN`; nếu có điểm null/ngoài 1-10 thì không cho xác nhận và báo: `Không thể xác nhận vì có điểm đánh giá không hợp lệ. Vui lòng chỉnh điểm trong khoảng từ 1 đến 10.` |

Test case cần chạy/bảo đảm:

| Mã test | Tình huống | Kết quả mong đợi |
|---|---|---|
| DGDA-DIEM-01 | Nhập điểm 1 | Lưu được |
| DGDA-DIEM-02 | Nhập điểm 10 | Lưu được |
| DGDA-DIEM-03 | Nhập điểm 0 | Bị chặn bởi ViewModel/Service |
| DGDA-DIEM-04 | Nhập điểm 11 | Bị chặn bởi ViewModel/Service |
| DGDA-DIEM-05 | Nhập điểm âm | Bị chặn bởi ViewModel/Service |
| DGDA-DIEM-06 | Bỏ trống điểm | Bị chặn bởi `required`/ModelState hoặc service không nhận điểm hợp lệ |
| DGDA-DIEM-07 | Sửa HTML/request gửi điểm 99 | Service `LuuDanhGiaAsync` vẫn chặn |
| DGDA-DIEM-08 | Bản nháp có điểm sai trong DB cũ | `XacNhanAsync` không cho xác nhận |
| DGDA-DIEM-09 | Toàn bộ điểm hợp lệ | Lưu và xác nhận đúng workflow Manager xác nhận |
| DIEM-UTF8-01 | Kiểm tra font tiếng Việt | View và docs giữ UTF-8 |
| DIEM-DB-01 | Kiểm tra migration/schema | Không có migration mới, không sửa database |

Kết quả kiểm tra sau cập nhật điểm:

- Đã chạy `dotnet build .\QuanLyDuAn\QuanLyDuAn.sln`: build thành công, `0 Error(s)`, còn `2 Warning(s)` cũ ở `FileTienDoCongViecService.cs`.
- Đã kiểm tra UTF-8 cho các file đã sửa trong module `DanhGiaDuAn`: không có ký tự replacement `U+FFFD`.
- Không tạo migration mới; thư mục `QuanLyDuAn/QuanLyDuAn/Migrations` chỉ có migration cũ.
