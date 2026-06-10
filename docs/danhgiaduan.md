# 1. Tổng quan chức năng Đánh giá dự án

Chức năng Đánh giá dự án nằm trong module `DanhGiaDuAn` của ASP.NET Core MVC. Source chính đã rà soát:

- `QuanLyDuAn/QuanLyDuAn/Controllers/DanhGiaDuAnController.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDanhGiaDuAnService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/*`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/*`

Theo source hiện tại, chức năng này dùng để Manager đánh giá tổng kết dự án khi dự án đã đủ điều kiện đánh giá. Điều kiện gửi duyệt đánh giá dự án đang dựa trên trạng thái dự án: `HoanThanh`, `ChoXacNhanHoanThanh`, hoặc `Archived`/`LuuTru`.

Người thao tác chính:

- Manager: xem danh sách dự án thuộc phạm vi mình quản lý, tạo/sửa đánh giá, lưu nháp, gửi duyệt, phân tích AI, xác nhận nguyên nhân thực tế nếu dự án bị trễ.
- Admin: duyệt hoặc từ chối đánh giá đang ở trạng thái chờ duyệt.
- Người có quyền xem và thuộc phạm vi dự án: xem chi tiết đánh giá.

Dữ liệu đánh giá chính được lưu vào:

- `DANH_GIA_DU_AN`: thông tin đầu đánh giá, điểm tổng, nhận xét tổng, trạng thái duyệt, người duyệt.
- `CT_DANH_GIA_DU_AN`: điểm và nhận xét theo từng tiêu chí.
- `TIEU_CHI_DANH_GIA`: danh mục tiêu chí dùng để sinh form đánh giá dự án.

Phần "Dữ liệu hỗ trợ" không lưu riêng trong bảng đánh giá. Nó được tính động trong `DanhGiaDuAnService.LoadThongTinHoTroDanhGiaAsync(...)` từ `DU_AN`, `CONG_VIEC`, `CT_CONG_VIEC`, `TIEN_DO_CONG_VIEC`, `NGAN_SACH`, `CHI_PHI`, `FILE_DU_AN`, `AI_DATASET`, `AI_KET_QUA`, `AI_NGUYEN_NHAN`, `DM_NGUYEN_NHAN`.

# 2. Luồng xử lý hiện tại theo source

## 2.1 Vào màn hình đánh giá dự án

Người dùng vào `DanhGiaDuAn/Index`. Controller kiểm tra quyền `DanhGiaDuAn.Xem`, sau đó gọi `IDanhGiaDuAnService.GetPageAsync(...)`.

Trong `GetPageAsync(...)`:

- Lấy mã người dùng hiện tại từ `AspNetUsers`.
- Lấy vai trò hiện tại từ `AspNetUserRoles` và `AspNetRoles`.
- Nếu không phải Manager thì trả về trang rỗng.
- Nếu là Manager thì lấy danh sách dự án mà `DU_AN.MaNguoiDung` là người dùng hiện tại.
- Lấy đánh giá mới nhất của từng dự án từ `DANH_GIA_DU_AN`.
- Tính dữ liệu danh sách: trạng thái dự án, phần trăm hoàn thành, tổng công việc, số công việc trễ, trạng thái thời hạn, số ngày quá hạn, trạng thái đánh giá, quyền thao tác.

View `Views/DanhGiaDuAn/Index.cshtml` hiển thị bộ lọc, thẻ thống kê, bảng danh sách và form nếu đang mở form.

## 2.2 Mở form đánh giá dự án

Action `Form` nhận `maDuAn`. Controller kiểm tra quyền `DanhGiaDuAn.DanhGia` hoặc `DanhGiaDuAn.Sua`, gọi:

- `GetPageAsync(...)` để dựng lại trang danh sách theo bộ lọc.
- `GetFormAsync(maDuAn)` để dựng form.

Trong `GetFormAsync(...)`:

- Chặn Admin thao tác nghiệp vụ đánh giá.
- Chỉ cho Manager đang quản lý dự án mở form.
- Lấy tiêu chí loại `DANHGIADUAN` hoặc `DUAN`, trạng thái rỗng hoặc `DangSuDung`.
- Lấy đánh giá mới nhất của chính Manager hiện tại cho dự án.
- Nếu đã có chi tiết đánh giá thì map lại điểm/nhận xét theo tiêu chí; nếu chưa có thì điểm mặc định từng tiêu chí là 1.
- Tính điểm tổng bằng trung bình điểm tiêu chí.
- Khóa form nếu đánh giá ở trạng thái `ChoDuyet` hoặc `DaDuyet`.
- Gọi `XayDungThongKeDuAnAsync(...)` để lấy "Dữ liệu hỗ trợ" và dữ liệu AI.

View `_Form.cshtml` hiển thị:

- Khối "Dữ liệu hỗ trợ" qua partial `_ProjectStats.cshtml`.
- Khối "Gợi ý AI" qua partial `_AiInsightCard.cshtml` nếu `Model.ThongKe.DuAnBiTreTheoAi == true`.
- Khối xác nhận nguyên nhân thực tế nếu dự án trễ và người dùng đủ quyền.
- Các dòng tiêu chí: điểm 1-10 và nhận xét từng tiêu chí.
- Nhận xét tổng dự án.
- Nút "Lưu đánh giá" nếu `CoTheLuu == true`.
- Nút "Gửi duyệt" nếu `CoTheGuiDuyet == true` và đã có `MaDanhGiaDuAn`.

## 2.3 Lưu nháp

Action `Luu` nhận `DanhGiaDuAnFormViewModel`. Controller kiểm tra quyền `DanhGiaDuAn.DanhGia` hoặc `DanhGiaDuAn.Sua`, kiểm tra `ModelState`, rồi gọi `LuuDanhGiaAsync(form)`.

Trong service:

- Kiểm tra dự án hợp lệ.
- Kiểm tra phải có ít nhất một tiêu chí.
- Chỉ Manager đang quản lý dự án và là người tạo đánh giá được lưu.
- Điểm từng tiêu chí phải từ 1 đến 10.
- Nhận xét từng tiêu chí và nhận xét tổng tối đa 500 ký tự theo rule service.
- Nếu tạo mới thì thêm bản ghi `DANH_GIA_DU_AN`.
- Nếu sửa thì không cho sửa khi trạng thái là `DaDuyet` hoặc `ChoDuyet`.
- Tính điểm tổng bằng trung bình điểm tiêu chí, sau đó làm tròn về số nguyên khi lưu vào `DiemTongDanhGiaDA`.
- Gán `TrangThaiDanhGiaDA = "Nhap"`.
- Xóa mềm các chi tiết cũ trong `CT_DANH_GIA_DU_AN`, rồi thêm lại chi tiết mới.

Lưu ý: service gọi `SaveChangesAsync()` một lần sau khi tạo/cập nhật đầu đánh giá để có `MaDanhGiaDuAn`, sau đó gọi tiếp lần nữa sau khi xóa mềm/thêm chi tiết. Không thấy dùng transaction tường minh.

## 2.4 Gửi duyệt

Action `GuiDuyet` kiểm tra quyền `DanhGiaDuAn.DanhGia` hoặc `DanhGiaDuAn.Sua`, rồi gọi `GuiDuyetAsync(maDanhGiaDuAn)`.

Rule trong service:

- Chặn Admin thao tác.
- Chỉ Manager quản lý dự án và là người tạo đánh giá được gửi duyệt.
- Chỉ gửi duyệt khi đánh giá đang `Nhap` hoặc `TuChoi`.
- Chỉ gửi duyệt khi dự án ở `HoanThanh`, `ChoXacNhanHoanThanh`, hoặc `LuuTru`.
- Phải có ít nhất một chi tiết tiêu chí.
- Cập nhật `TrangThaiDanhGiaDA = "ChoDuyet"`, xóa thông tin người duyệt/lý do từ chối cũ, cập nhật `NgayDanhGiaDA`.

## 2.5 Duyệt hoặc từ chối

Action `Duyet` kiểm tra quyền `DanhGiaDuAn.Duyet`, sau đó service chỉ cho Admin duyệt đánh giá đang `ChoDuyet`. Khi duyệt:

- `TrangThaiDanhGiaDA = "DaDuyet"`
- `MaNguoiDungDuyet = currentUserId`
- `NgayDuyetDanhGiaDA = DateTime.Now`
- `LyDoTuChoiDanhGiaDA = null`

Action `TuChoi` cũng kiểm tra quyền `DanhGiaDuAn.Duyet`, service chỉ cho Admin từ chối đánh giá đang `ChoDuyet`. Lý do từ chối bắt buộc và tối đa 500 ký tự. Khi từ chối:

- `TrangThaiDanhGiaDA = "TuChoi"`
- `MaNguoiDungDuyet = currentUserId`
- `NgayDuyetDanhGiaDA = DateTime.Now`
- `LyDoTuChoiDanhGiaDA = lyDo`

# 3. Controller và action đang dùng

Source: `QuanLyDuAn/QuanLyDuAn/Controllers/DanhGiaDuAnController.cs`.

| Action | Method | Route thực tế | Quyền kiểm tra ở controller | Service gọi | View/kết quả |
|---|---|---|---|---|---|
| `Index` | GET | `/DanhGiaDuAn/Index` | `DanhGiaDuAn.Xem` | `GetPageAsync(...)` | `Views/DanhGiaDuAn/Index.cshtml` với `DanhGiaDuAnPageViewModel` |
| `Form` | GET | `/DanhGiaDuAn/Form?maDuAn=...` | `DanhGiaDuAn.DanhGia` hoặc `DanhGiaDuAn.Sua` | `GetPageAsync(...)`, `GetFormAsync(maDuAn)` | Trả về view `Index` kèm `page.Form` |
| `Luu` | POST | `/DanhGiaDuAn/Luu` | `DanhGiaDuAn.DanhGia` hoặc `DanhGiaDuAn.Sua` | `LuuDanhGiaAsync(form)` | Redirect về `returnUrl` hợp lệ hoặc `Index` |
| `XacNhanNguyenNhan` | POST | `/DanhGiaDuAn/XacNhanNguyenNhan` | `AI.XacNhan` hoặc `DanhGiaDuAn.DanhGia` | `XacNhanNguyenNhanAsync(...)` | Redirect về `returnUrl` hợp lệ hoặc `Index` |
| `PhanTichAiDuAn` | POST | `/DanhGiaDuAn/PhanTichAiDuAn` | `DanhGiaDuAn.DanhGia` hoặc `DanhGiaDuAn.Sua` | `PhanTichAiDuAnAsync(...)` | JSON thành công/thất bại |
| `GuiDuyet` | POST | `/DanhGiaDuAn/GuiDuyet` | `DanhGiaDuAn.DanhGia` hoặc `DanhGiaDuAn.Sua` | `GuiDuyetAsync(maDanhGiaDuAn)` | Redirect |
| `Duyet` | POST | `/DanhGiaDuAn/Duyet` | `DanhGiaDuAn.Duyet` | `DuyetAsync(maDanhGiaDuAn)` | Redirect |
| `TuChoi` | POST | `/DanhGiaDuAn/TuChoi` | `DanhGiaDuAn.Duyet` | `TuChoiAsync(maDanhGiaDuAn, lyDoTuChoi)` | Redirect |
| `XuatFile` | GET | `/DanhGiaDuAn/XuatFile` | `ThongKe.XuatFile` | `GetPageAsync(...)` | File báo cáo |
| `ChiTiet` | GET | `/DanhGiaDuAn/ChiTiet?maDanhGiaDuAn=...` | `DanhGiaDuAn.Xem` | `GetChiTietAsync(maDanhGiaDuAn)` | `Views/DanhGiaDuAn/ChiTiet.cshtml` |

Không thấy attribute route riêng trong controller; route đang theo convention MVC.

# 4. Service và business rule

Source: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs`.

## 4.1 Các hàm chính

| Hàm | Vai trò |
|---|---|
| `GetPageAsync(...)` | Dựng trang danh sách, bộ lọc, thống kê trạng thái đánh giá, quyền thao tác từng dòng |
| `GetFormAsync(int maDuAn)` | Dựng form đánh giá, tiêu chí, dữ liệu hỗ trợ, AI insight, trạng thái khóa/mở form |
| `LuuDanhGiaAsync(DanhGiaDuAnFormViewModel form)` | Tạo/cập nhật bản nháp đánh giá và chi tiết tiêu chí |
| `GuiDuyetAsync(int maDanhGiaDuAn)` | Chuyển trạng thái đánh giá từ nháp/từ chối sang chờ duyệt |
| `DuyetAsync(int maDanhGiaDuAn)` | Admin duyệt đánh giá |
| `TuChoiAsync(int maDanhGiaDuAn, string lyDoTuChoi)` | Admin từ chối đánh giá |
| `GetChiTietAsync(int maDanhGiaDuAn)` | Dựng màn hình xem chi tiết đánh giá đã có |
| `XayDungThongKeDuAnAsync(...)` | Tạo toàn bộ dữ liệu hỗ trợ và dữ liệu AI cho form/chi tiết |
| `LoadThongTinHoTroDanhGiaAsync(...)` | Tính dữ liệu hỗ trợ nghiệp vụ từ bảng dự án, công việc, chi phí, tiến độ |
| `LoadKetQuaAiMoiNhatAsync(...)` | Gắn kết quả AI mới nhất và nguyên nhân Manager xác nhận |
| `PhanTichAiDuAnAsync(...)` | Tổng hợp dataset nếu cần, gọi AI service phân tích nguyên nhân trễ |
| `XacNhanNguyenNhanAsync(...)` | Lưu nguyên nhân thực tế do Manager xác nhận vào `AI_NGUYEN_NHAN` |

## 4.2 Rule quyền và phạm vi dữ liệu

- `GetPageAsync(...)` yêu cầu claim `DanhGiaDuAn.Xem`.
- Danh sách chỉ trả dữ liệu cho vai trò Manager; nếu không phải Manager thì danh sách rỗng.
- Danh sách dự án theo scope chỉ lấy `DU_AN.MaNguoiDung == currentUserId`.
- `GetFormAsync(...)`, `LuuDanhGiaAsync(...)`, `GuiDuyetAsync(...)`, `PhanTichAiDuAnAsync(...)` chặn Admin thao tác bằng `KiemTraKhongChoAdminTacNghiep(...)`.
- Form và lưu đánh giá chỉ cho Manager đang quản lý dự án.
- `DuyetAsync(...)` và `TuChoiAsync(...)` yêu cầu role Admin trong service, ngoài claim `DanhGiaDuAn.Duyet`.
- `GetChiTietAsync(...)` cho Admin xem, Manager quản lý dự án xem, hoặc người có trong `NhanVienDuAn` xem.

## 4.3 Rule trạng thái đánh giá

Trạng thái đánh giá đang dùng trong source:

- `Nhap`
- `ChoDuyet`
- `DaDuyet`
- `TuChoi`
- `ChuaDanhGia` là trạng thái hiển thị ở danh sách khi chưa có bản ghi đánh giá, không phải trạng thái lưu trong `DANH_GIA_DU_AN`.

Rule:

- Đánh giá mới hoặc lưu lại luôn chuyển về `Nhap`.
- Đánh giá `ChoDuyet` hoặc `DaDuyet` bị khóa không cho sửa.
- Đánh giá `Nhap` hoặc `TuChoi` có thể gửi duyệt nếu dự án đủ điều kiện.
- Chỉ đánh giá `ChoDuyet` mới được Admin duyệt/từ chối.

## 4.4 Rule trạng thái dự án

Hàm `ChoPhepGuiDuyetTheoTrangThaiDuAn(...)` cho phép gửi duyệt khi dự án:

- `HoanThanh`
- `ChoXacNhanHoanThanh`
- `LuuTru` (`Archived`)

Ở danh sách, `DuDieuKienDanhGia` cũng dựa trên rule này. Nếu dự án chưa đủ điều kiện, UI hiện "Chưa đủ điều kiện" hoặc "Chờ dự án hoàn thành/lưu trữ."

## 4.5 Rule tính điểm và xếp loại

- Điểm từng tiêu chí từ 1 đến 10.
- `TinhDiemTongKet(...)` tính trung bình điểm tiêu chí, làm tròn 2 chữ số.
- Khi lưu vào `DANH_GIA_DU_AN.DiemTongDanhGiaDA`, service làm tròn về số nguyên bằng `Math.Round(..., MidpointRounding.AwayFromZero)`.
- Xếp loại đang hiển thị không dấu:
  - `>= 8.5`: `Xuat sac`
  - `>= 7`: `Tot`
  - `>= 5.5`: `Kha`
  - `>= 4`: `Trung binh`
  - còn lại: `Kem`

## 4.6 Rule tạo chi tiết tiêu chí

- Tiêu chí lấy từ `TIEU_CHI_DANH_GIA` với `LoaiTieuChi` là `DANHGIADUAN` hoặc `DUAN`.
- Chỉ lấy tiêu chí đang dùng: `TrangThaiTieuChi` rỗng hoặc thuộc biến thể `DangSuDung`.
- Khi lưu, chi tiết cũ trong `CT_DANH_GIA_DU_AN` bị xóa mềm, sau đó thêm chi tiết mới cho toàn bộ tiêu chí trên form.

## 4.7 Lưu ý transaction và SaveChanges

`LuuDanhGiaAsync(...)` gọi `SaveChangesAsync()` hai lần:

- Lần 1 sau khi tạo/cập nhật bản ghi `DANH_GIA_DU_AN`.
- Lần 2 sau khi xóa mềm chi tiết cũ và thêm chi tiết mới.

Không thấy transaction tường minh. Nếu lỗi xảy ra sau lần lưu đầu nhưng trước lần lưu chi tiết, có khả năng dữ liệu đầu đánh giá đã đổi còn chi tiết chưa đồng bộ.

# 5. Cấu trúc dữ liệu đánh giá dự án

Source entity và mapping:

- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DanhGiaDuAn.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/CtDanhGiaDuAn.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/TieuChiDanhGia.cs`
- `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`

## 5.1 `DANH_GIA_DU_AN`

Lưu đầu đánh giá dự án:

| Trường entity | Ý nghĩa theo source |
|---|---|
| `MaDanhGiaDuAn` | Khóa chính bản đánh giá |
| `MaDuAn` | Dự án được đánh giá |
| `MaNguoiDung` | Người tạo/người đánh giá |
| `DiemTongDanhGiaDA` | Điểm tổng đã làm tròn về số nguyên khi lưu |
| `NhanXetTongDuAn` | Nhận xét tổng dự án |
| `NgayDanhGiaDA` | Ngày lưu/gửi đánh giá |
| `TrangThaiDanhGiaDA` | `Nhap`, `ChoDuyet`, `DaDuyet`, `TuChoi` |
| `MaNguoiDungDuyet` | Người duyệt/từ chối |
| `NgayDuyetDanhGiaDA` | Thời điểm duyệt/từ chối |
| `LyDoTuChoiDanhGiaDA` | Lý do từ chối |
| `IsDeleted`, `DeletedAt`, `DeletedBy` | Xóa mềm |

## 5.2 `CT_DANH_GIA_DU_AN`

Lưu chi tiết theo tiêu chí:

| Trường entity | Ý nghĩa theo source |
|---|---|
| `MaChiTietDGDA` | Khóa chính chi tiết |
| `MaDanhGiaDuAn` | Liên kết đầu đánh giá |
| `MaTieuChi` | Tiêu chí đánh giá |
| `NhanXetDuAn` | Nhận xét theo tiêu chí |
| `DiemDanhGiaDA` | Điểm tiêu chí |
| `IsDeleted`, `DeletedAt`, `DeletedBy` | Xóa mềm |

## 5.3 `TIEU_CHI_DANH_GIA`

Lưu danh mục tiêu chí. Form dự án chỉ lấy tiêu chí có `LoaiTieuChi` là `DUAN` hoặc `DANHGIADUAN`, đang dùng. Seed mặc định trong `KhoiTaoTaiKhoanMacDinh.cs` có các tiêu chí dự án:

- `Tien do`
- `Chat luong`
- `Chi phi ngan sach`
- `Phoi hop`
- `Hieu qua tong the`

Các tiêu chí seed hiện là không dấu.

## 5.4 Quan hệ với `DU_AN`

`DANH_GIA_DU_AN.MaDuAn` liên kết `DU_AN.MaDuAn`. Service dùng thêm:

- `DU_AN.MaNguoiDung`: Manager quản lý dự án.
- `DU_AN.TrangThaiDuAn`: xác định đủ điều kiện đánh giá.
- `DU_AN.NgayBatDauDuAn`, `NgayKetThucDuAn`, `NgayHoanThanhThucTeDuAn`, `PhanTramHoanThanh`: tính dữ liệu hỗ trợ.

# 6. Phân tích phần “Dữ liệu hỗ trợ” hiện tại

Source tính dữ liệu: `DanhGiaDuAnService.LoadThongTinHoTroDanhGiaAsync(...)`.

Source hiển thị: `Views/DanhGiaDuAn/_ProjectStats.cshtml`.

## 6.1 Bảng chỉ số đang hiển thị

| Chỉ số UI | Biến ViewModel | Cách tính trong service | Bảng nguồn | Ý nghĩa nghiệp vụ | Nhận xét |
|---|---|---|---|---|---|
| Ngày bắt đầu dự án | `NgayBatDauDuAn` | Lấy trực tiếp từ `DU_AN.NgayBatDauDuAn` | `DU_AN` | Mốc bắt đầu dự án | Đúng theo dữ liệu dự án, không có tính toán thêm |
| Ngày kết thúc dự kiến | `NgayKetThucDuAn` | Lấy trực tiếp từ `DU_AN.NgayKetThucDuAn` | `DU_AN` | Mốc kế hoạch để so sánh trễ hạn | Nếu null thì trạng thái thời hạn là chưa có mốc kết thúc |
| Ngày kết thúc thực tế | `NgayKetThucThucTeDuAn` | Ưu tiên `DU_AN.NgayHoanThanhThucTeDuAn`, nếu null thì fallback `MAX(CONG_VIEC.NgayKetThucCVThucTe)` | `DU_AN`, `CONG_VIEC`, `DANH_MUC_CONG_VIEC` | Mốc hoàn thành thực tế để xét hoàn thành đúng/trễ | Có thể gây hiểu nhầm vì nếu dự án chưa có ngày hoàn thành thật nhưng có công việc đã hoàn thành, UI vẫn có thể hiện ngày kết thúc thực tế theo fallback |
| Trạng thái thời hạn | `TrangThaiThoiHanDuAn` | `BuildTimelineInsightAsync(...)`: so sánh ngày dự kiến với ngày thực tế hoặc ngày hiện tại | `DU_AN`, fallback từ `CONG_VIEC` | Cho biết chưa đến hạn, sắp đến hạn, quá hạn, hoàn thành đúng hạn, hoàn thành trễ hạn | Rule tính khá rõ, nhưng phụ thuộc fallback ngày kết thúc thực tế nên cần diễn giải nếu hiển thị cho Manager |
| Số ngày còn lại | `SoNgayConLai` | Nếu có ngày kết thúc dự kiến và chưa quá hạn thì `ngayDuKien - homNay`; nếu hoàn thành đúng hạn thì `Abs(ngayThucTe - ngayDuKien)` | `DU_AN`, `CONG_VIEC` | Cho biết còn bao nhiêu ngày hoặc hoàn thành sớm/đúng mốc bao nhiêu ngày | Với dự án đã hoàn thành đúng hạn, nhãn "Số ngày còn lại" dễ hiểu sai vì thực chất là số ngày không trễ/sớm so với kế hoạch |
| Số ngày quá hạn | `SoNgayQuaHan` | Nếu hoàn thành trễ: `NgayKetThucThucTeDuAn - NgayKetThucDuAn`; nếu chưa hoàn thành và đã quá hạn: `homNay - NgayKetThucDuAn` | `DU_AN`, `CONG_VIEC` | Độ trễ theo ngày | Đúng theo rule hiện tại; cần chú thích khi dự án hoàn thành trễ nhưng công việc trễ hạn bằng 0 |
| Tổng công việc | `TongCongViec` | Đếm `CONG_VIEC` thuộc danh mục của dự án, chưa xóa | `CONG_VIEC`, `DANH_MUC_CONG_VIEC` | Quy mô công việc cấp công việc | Đúng với source |
| Công việc hoàn thành | `CongViecHoanThanh` | Đếm công việc có `TrangThaiCongViec` thuộc biến thể `HoanThanh` | `CONG_VIEC` | Số công việc đã hoàn thành | Đúng theo trạng thái, không xét ngày hoàn thành |
| Công việc trễ hạn | `CongViecTreHan` | Đếm công việc có `NgayKetThucCVDuKien < DateTime.Now` và chưa hoàn thành | `CONG_VIEC` | Số công việc chưa hoàn thành quá hạn | Chưa tính trường hợp công việc đã hoàn thành nhưng hoàn thành sau hạn; khác với cách `AI_DATASET` tính công việc trễ |
| Tỷ lệ hoàn thành | `TyLeHoanThanh` | `CongViecHoanThanh / TongCongViec * 100`, làm tròn 2 chữ số | `CONG_VIEC` | Tỷ lệ hoàn thành theo số công việc | Không dùng `DU_AN.PhanTramHoanThanh`, nên có thể lệch với phần trăm dự án ở danh sách |
| Ngân sách được duyệt | `TongNganSach` | Tổng `NGAN_SACH.SoTienNganSach` của dự án có trạng thái `DaDuyet`, chưa xóa | `NGAN_SACH` | Tổng ngân sách hợp lệ để so với chi phí | Đúng theo rule ngân sách duyệt |
| Chi phí đã dùng | `TongChiPhi` | Tổng `CHI_PHI.SoTienDaChi` join qua `NGAN_SACH` thuộc dự án, `CHI_PHI` và `NGAN_SACH` chưa xóa | `CHI_PHI`, `NGAN_SACH` | Tổng tiền đã chi | Không lọc trạng thái chi phí; nếu có trạng thái chi phí nháp/chờ duyệt trong dữ liệu thì có thể bị tính chung |
| Tỷ lệ sử dụng ngân sách | `TyLeSuDungNganSach` | `TongChiPhi / TongNganSach * 100`, nếu ngân sách <= 0 thì 0 | `CHI_PHI`, `NGAN_SACH` | Mức sử dụng ngân sách | Có thể >100%, UI chưa cảnh báo rõ |
| Số báo cáo tiến độ | `SoBaoCaoTienDo` | Đếm `TIEN_DO_CONG_VIEC` thuộc các chi tiết công việc của dự án | `TIEN_DO_CONG_VIEC`, `CT_CONG_VIEC`, `CONG_VIEC`, `DANH_MUC_CONG_VIEC` | Số lần cập nhật/báo cáo tiến độ | Chỉ là tổng số báo cáo, chưa tách chờ duyệt/đã duyệt/từ chối/yêu cầu bổ sung |
| Chênh lệch ngân sách | Không có biến riêng; view tính `TongNganSach - TongChiPhi` | Tính trực tiếp trong `_ProjectStats.cshtml` | `NGAN_SACH`, `CHI_PHI` | Số tiền còn lại nếu dương, vượt nếu âm | Dễ hiểu nhầm vì nhãn "chênh lệch" không nói rõ dương là còn lại, âm là vượt |

## 6.2 Chỉ số có trong ViewModel nhưng chưa hiển thị ở `_ProjectStats`

`DanhGiaDuAnThongKeViewModel` có thêm các trường sau nhưng `_ProjectStats.cshtml` hiện chưa hiển thị:

- `TongChiTietCongViec`
- `ChiTietHoanThanh`
- `ChiTietTreHan`
- `SoBaoCaoMoiNhat`
- `SoFileDuAn`
- Một số trường AI như nguồn nguyên nhân, model, kết quả cũ, cảnh báo dữ liệu.

## 6.3 Phần AI trong dữ liệu hỗ trợ

Trong form, `_AiInsightCard.cshtml` chỉ hiển thị khi `Model.ThongKe.DuAnBiTreTheoAi == true`.

Các chỉ số AI đang hiển thị:

| Chỉ số UI | Biến ViewModel | Nguồn/cách tính |
|---|---|---|
| Tình trạng AI | `DuAnBiTreTheoAi`, `TrangThaiThoiHanDuAn`, `SoNgayQuaHan` | Kết hợp trạng thái thực tế và kết quả AI |
| Gợi ý AI | `TenNguyenNhanAiDuDoan` | Từ `AI_KET_QUA` join `DM_NGUYEN_NHAN`, hoặc fallback từ `AI_DATASET` |
| Mức phù hợp | `MucPhuHopAi` hoặc `DoTinCayAi` | Từ payload `NoiDungPhanTich` hoặc quy đổi độ tin cậy |
| Thời gian phân tích | `ThoiGianDuDoanAi` | `AI_KET_QUA.ThoiGianDuDoanKetQua` |
| Nguyên nhân Manager xác nhận | `TenNguyenNhanManagerXacNhan` | `AI_NGUYEN_NHAN` join `DM_NGUYEN_NHAN` |
| Nguyên nhân liên quan | `DanhSachNguyenNhanLienQuan` | Parse từ `AI_KET_QUA.NoiDungPhanTich` nếu có marker payload |

Nếu dự án không trễ, form hiện thông báo "Dự án không trễ nên không cần xác nhận nguyên nhân trễ."

# 7. Đánh giá dữ liệu hỗ trợ còn thiếu

Các đề xuất dưới đây chỉ dựa trên dữ liệu đã có trong source/entity/service hiện tại, không yêu cầu thêm bảng mới.

## 7.1 Dữ liệu tiến độ

Đang thiếu trên form:

- Số báo cáo tiến độ chờ duyệt.
- Số báo cáo tiến độ bị từ chối.
- Số báo cáo tiến độ yêu cầu bổ sung.
- Tỷ lệ báo cáo tiến độ bị từ chối.
- Lần cập nhật tiến độ gần nhất.
- Số ngày chậm cập nhật tiến độ.

Dữ liệu này đã có hoặc có thể tính từ:

- `TIEN_DO_CONG_VIEC.TrangThaiTienDo`
- `TIEN_DO_CONG_VIEC.ThoiGianCapNhat`
- `AI_DATASET.SoBaoCaoTienDoChoDuyet`
- `AI_DATASET.SoBaoCaoTienDoBiTuChoi`
- `AI_DATASET.SoBaoCaoTienDoYeuCauBoSung`
- `AI_DATASET.TyLeBaoCaoTienDoBiTuChoi`
- `AI_DATASET.SoLanCapNhatTienDo`
- `AI_DATASET.SoNgayChamCapNhatTienDo`

`AiDatasetService` hiện đã tổng hợp các chỉ số này, nhưng `DanhGiaDuAnService.LoadThongTinHoTroDanhGiaAsync(...)` chưa đưa vào `DanhGiaDuAnThongKeViewModel` và `_ProjectStats.cshtml` chưa hiển thị.

## 7.2 Dữ liệu công việc

Đang thiếu trên form:

- Số công việc chưa hoàn thành.
- Số công việc đang bị cản trở.
- Số công việc hoàn thành trễ.
- Tỷ lệ công việc trễ.

Dữ liệu có thể lấy từ `CONG_VIEC.TrangThaiCongViec`, `NgayKetThucCVDuKien`, `NgayKetThucCVThucTe`.

Điểm cần chú ý: `DanhGiaDuAnService` hiện tính `CongViecTreHan` là công việc chưa hoàn thành và đã quá hạn. Trong khi `AiDatasetService` tính công việc trễ bao gồm cả công việc đã hoàn thành sau hạn. Nếu đưa thêm chỉ số, cần ghi rõ "đang trễ" và "hoàn thành trễ" để tránh nhầm.

## 7.3 Dữ liệu chi tiết công việc

Đang có trong service nhưng chưa hiển thị:

- Tổng chi tiết công việc.
- Chi tiết hoàn thành.
- Chi tiết trễ hạn.

Đang thiếu:

- Chi tiết chưa hoàn thành.
- Chi tiết bị cản trở.
- Tỷ lệ chi tiết hoàn thành.

Dữ liệu lấy từ `CT_CONG_VIEC.TrangThaiCTCV`, `NgayKetThucCTCV`.

## 7.4 Dữ liệu ngân sách/chi phí

Đang thiếu hoặc chưa rõ:

- Vượt ngân sách bao nhiêu: có thể tính từ `TongChiPhi - TongNganSach` khi dương.
- Tỷ lệ vượt ngân sách: `(TongChiPhi - TongNganSach) / TongNganSach * 100`.
- Số tiền còn lại: `TongNganSach - TongChiPhi` khi dương.
- Trạng thái "còn lại" hoặc "vượt mức" thay vì nhãn chung "Chênh lệch ngân sách".

Dữ liệu hiện có từ `NGAN_SACH`, `CHI_PHI`, và đã có thêm trong `AI_DATASET.ChenhLechChiPhi`.

## 7.5 Dữ liệu đề xuất

Đang thiếu trên form:

- Số đề xuất công việc chờ duyệt.
- Số đề xuất công việc bị từ chối.
- Thời gian duyệt đề xuất công việc trung bình.
- Số đề xuất ngân sách chờ duyệt.
- Số đề xuất ngân sách bị từ chối.
- Thời gian duyệt đề xuất ngân sách trung bình.

Dữ liệu đã được tổng hợp trong `AI_DATASET` bởi `AiDatasetService`:

- `SoDeXuatCongViecChoDuyet`
- `SoDeXuatCongViecBiTuChoi`
- `ThoiGianDuyetCongViecTrungBinh`
- `SoDeXuatNganSachChoDuyet`
- `SoDeXuatNganSachBiTuChoi`
- `ThoiGianDuyetNganSachTrungBinh`

Nguồn gốc nghiệp vụ nằm ở `DE_XUAT_CONG_VIEC` và `DE_XUAT_NGAN_SACH`.

## 7.6 Dữ liệu nhân sự

Đang thiếu trên form:

- Số nhân viên tham gia dự án.
- Số lần thay đổi nhân sự.
- Số lần thay đổi quản lý.

Dữ liệu đã có:

- `NHAN_VIEN_DU_AN` có thể đếm nhân viên tham gia.
- `AI_DATASET.SoNhanVienDuAn`
- `AI_DATASET.SoLanThayDoiNhanSu`
- `AI_DATASET.SoLanThayDoiQuanLy`
- Entity nhật ký như `NhatKyPhuTrachDuAn`, `NhatKyQuanLyDuAn`, `NhatKyPhanCong...` đang tồn tại trong DbContext, và `AiDatasetService` đã có rule nhận diện hành động thay đổi nhân sự.

## 7.7 Dữ liệu AI

Đang hiển thị một phần:

- Gợi ý nguyên nhân AI.
- Mức phù hợp.
- Thời gian phân tích.
- Nguyên nhân Manager xác nhận.
- Nguyên nhân liên quan.

Đang thiếu hoặc chưa rõ trên form:

- Nguồn nguyên nhân AI: model nguyên nhân hay rule fallback.
- Tên model đang dùng.
- Cảnh báo kết quả AI cũ so với dataset/model mới có thể chưa đủ nổi bật.
- Dữ liệu đầu vào AI quan trọng: tỷ lệ công việc trễ, chênh lệch chi phí, số ngày trễ, biến động nhân sự.
- Thời điểm Manager xác nhận nguyên nhân. Entity `AI_NGUYEN_NHAN` có `NgayXacNhan` và `MaNguoiDungXacNhan`, nhưng `XacNhanNguyenNhanAsync(...)` hiện không gán hai trường này.

## 7.8 Dữ liệu đánh giá trước đó

Đang có một phần:

- Danh sách lấy đánh giá mới nhất theo dự án.
- Form mở đánh giá mới nhất của Manager hiện tại cho dự án.
- Trạng thái đánh giá hiện tại hiển thị ở danh sách/form.

Đang thiếu:

- Có bao nhiêu lần đánh giá cũ cho dự án.
- Lần đánh giá gần nhất trước đó là khi nào.
- Ai đã đánh giá trước đó nếu nhiều bản ghi.
- Lý do từ chối gần nhất trong form, nếu đang sửa lại bản bị từ chối.

# 8. Những điểm bất hợp lý hoặc chưa rõ

1. `Ngày kết thúc thực tế` có fallback từ `MAX(CONG_VIEC.NgayKetThucCVThucTe)` nếu `DU_AN.NgayHoanThanhThucTeDuAn` null. UI không nói rõ đây có thể là ngày hoàn thành công việc cuối cùng, nên Manager có thể hiểu nhầm là ngày hoàn thành chính thức của dự án.

2. `Số ngày còn lại` với dự án đã hoàn thành đúng hạn đang dùng `Abs(ngayThucTe - ngayDuKien)`. Nhãn này dễ hiểu sai vì dự án đã kết thúc thì không còn "ngày còn lại"; ý nghĩa thực tế là số ngày hoàn thành sớm/không trễ.

3. `Công việc trễ hạn` trong `DanhGiaDuAnService` chỉ tính công việc chưa hoàn thành và quá hạn. Công việc đã hoàn thành nhưng trễ hạn không được tính ở chỉ số này, trong khi `AI_DATASET` lại có rule tính công việc hoàn thành sau hạn là trễ. Đây là điểm dễ gây mâu thuẫn.

4. `Tỷ lệ hoàn thành` trong dữ liệu hỗ trợ tính theo số công việc hoàn thành, không phải `DU_AN.PhanTramHoanThanh`. Trong danh sách lại hiển thị `PhanTramHoanThanh` từ bảng dự án. Hai tỷ lệ này có thể khác nhau.

5. `Chi phí đã dùng` cộng toàn bộ `CHI_PHI.SoTienDaChi` của ngân sách thuộc dự án, không lọc `TrangThaiChiPhi`. Nếu bảng chi phí có quy trình trạng thái, chỉ số có thể bao gồm chi phí chưa duyệt.

6. `Chênh lệch ngân sách` đang tính `TongNganSach - TongChiPhi`. Giá trị dương là còn lại, giá trị âm là vượt, nhưng UI chỉ ghi "Chênh lệch ngân sách" nên dễ hiểu nhầm chiều âm/dương.

7. `Tỷ lệ sử dụng ngân sách` có thể vượt 100%, nhưng UI hiện chưa có badge hoặc cảnh báo riêng.

8. Dự án có thể "Hoàn thành trễ hạn" theo ngày kết thúc thực tế, nhưng `Công việc trễ hạn` bằng 0 vì công việc đã hoàn thành. Điều này đúng theo rule hiện tại nhưng cần nhãn rõ để Manager không nghĩ dữ liệu mâu thuẫn.

9. `NhanXetTongDuAn` trong ViewModel cho phép 500 ký tự, service cũng kiểm tra 500 ký tự, nhưng mapping DbContext đang cấu hình `NhanXetTongDuAn` tối đa 255 ký tự. Đây là điểm không nhất quán giữa validation và mapping.

10. `XacNhanNguyenNhanAsync(...)` cập nhật `AI_NGUYEN_NHAN.MaDMNguyenNhan` và `DoTinCay`, nhưng không gán `NgayXacNhan`, `MaNguoiDungXacNhan`, `GhiChuXacNhan`. Vì vậy dữ liệu "ai xác nhận/lúc nào" chưa đầy đủ dù entity đã có trường.

11. Form có nút `Gửi duyệt` dùng chung form `Luu` nhưng đổi `formaction` sang `GuiDuyet`. Các hidden filter có trong form, nhưng action `GuiDuyet` chỉ cần `maDanhGiaDuAn`; nếu người dùng bấm gửi duyệt mà chưa lưu thay đổi mới nhất trên form thì service sẽ gửi duyệt bản đang lưu trong DB, không lưu các thay đổi vừa nhập trên màn hình.

12. Xếp loại đang hiển thị không dấu (`Xuat sac`, `Tot`, `Kha`, `Trung binh`, `Kem`) trong source. Đây là vấn đề trình bày, không phải rule nghiệp vụ.

# 9. Đề xuất hướng sửa ở mức phân tích, chưa sửa code

Các đề xuất dưới đây chỉ là hướng phân tích cho bước sau, chưa sửa code.

## 9.1 Bổ sung chỉ số vào “Dữ liệu hỗ trợ”

Nên bổ sung theo mức ưu tiên:

1. Tiến độ báo cáo:
   - Báo cáo chờ duyệt.
   - Báo cáo bị từ chối.
   - Báo cáo yêu cầu bổ sung.
   - Lần cập nhật gần nhất.
   - Số ngày chậm cập nhật.

2. Công việc:
   - Công việc chưa hoàn thành.
   - Công việc đang trễ.
   - Công việc hoàn thành trễ.
   - Tỷ lệ công việc trễ.

3. Chi tiết công việc:
   - Tổng chi tiết.
   - Chi tiết hoàn thành.
   - Chi tiết chưa hoàn thành.
   - Chi tiết trễ hạn hoặc bị cản trở.

4. Ngân sách:
   - Còn lại ngân sách.
   - Vượt ngân sách.
   - Tỷ lệ vượt ngân sách.

5. Đề xuất:
   - Đề xuất công việc chờ duyệt/bị từ chối.
   - Đề xuất ngân sách chờ duyệt/bị từ chối.

6. Nhân sự:
   - Số nhân viên tham gia.
   - Số lần thay đổi nhân sự.
   - Số lần thay đổi quản lý.

7. AI:
   - Nguồn gợi ý AI.
   - Model nguyên nhân.
   - Dataset/kết quả có cũ không.
   - Ngày giờ Manager xác nhận nếu source lưu đầy đủ.

## 9.2 Đổi tên nhãn cho rõ hơn

Nên cân nhắc đổi:

| Nhãn hiện tại | Hướng nhãn rõ hơn |
|---|---|
| Số ngày còn lại | Số ngày còn lại / hoàn thành sớm |
| Công việc trễ hạn | Công việc đang trễ hạn |
| Chênh lệch ngân sách | Ngân sách còn lại/vượt |
| Tỷ lệ hoàn thành | Tỷ lệ công việc hoàn thành |
| Ngày kết thúc thực tế | Ngày hoàn thành thực tế dự án |

Nếu vẫn dùng fallback từ công việc, nhãn ngày kết thúc thực tế nên có chú thích hoặc tách thành "Ngày hoàn thành dự án" và "Ngày hoàn thành công việc cuối cùng".

## 9.3 Nhóm lại dữ liệu

Nên nhóm `Dữ liệu hỗ trợ` thành các nhóm:

- Thời hạn dự án.
- Công việc.
- Chi tiết công việc.
- Tiến độ báo cáo.
- Ngân sách/chi phí.
- Đề xuất.
- Nhân sự.
- AI.

Nhóm này giúp Manager nhìn nhanh nguyên nhân điểm đánh giá thay vì đọc một lưới chỉ số phẳng.

## 9.4 Badge và màu cảnh báo

Nên có cảnh báo rõ cho:

- `Trạng thái thời hạn = Quá hạn` hoặc `Hoàn thành trễ hạn`.
- `Số ngày quá hạn > 0`.
- `Tỷ lệ sử dụng ngân sách > 100%`.
- `Chênh lệch ngân sách < 0`.
- `Báo cáo tiến độ chờ duyệt > 0`.
- `Báo cáo tiến độ bị từ chối > 0` hoặc `Yêu cầu bổ sung > 0`.
- `Công việc đang trễ > 0`.
- `Công việc hoàn thành trễ > 0`.
- `Kết quả AI có thể đã cũ`.

## 9.5 Chỉ số nên giữ nguyên

Nên giữ các chỉ số nền tảng hiện có:

- Ngày bắt đầu dự án.
- Ngày kết thúc dự kiến.
- Trạng thái thời hạn.
- Số ngày quá hạn.
- Tổng công việc.
- Công việc hoàn thành.
- Ngân sách được duyệt.
- Chi phí đã dùng.
- Tỷ lệ sử dụng ngân sách.
- Số báo cáo tiến độ.

Tuy nhiên cần làm rõ nhãn/cách diễn giải cho các chỉ số dễ nhầm như công việc trễ, số ngày còn lại, ngày kết thúc thực tế và chênh lệch ngân sách.

# 10. Kết luận

Chức năng Đánh giá dự án hiện tại đã có đủ luồng cơ bản: Manager tạo/sửa đánh giá theo tiêu chí, lưu nháp, gửi duyệt; Admin duyệt/từ chối; có màn hình chi tiết; có dữ liệu hỗ trợ và gợi ý AI cho dự án trễ.

Phần dữ liệu hỗ trợ hiện mạnh ở các nhóm cơ bản: thời hạn dự án, số lượng công việc, tiến độ hoàn thành, ngân sách/chi phí, tổng số báo cáo tiến độ và AI nguyên nhân trễ.

Thiếu quan trọng nhất hiện nay là dữ liệu phân rã theo trạng thái: báo cáo tiến độ chờ duyệt/từ chối/yêu cầu bổ sung, công việc hoàn thành trễ so với đang trễ, chi tiết công việc, đề xuất công việc/ngân sách, nhân sự và các tín hiệu AI/dataset đã có nhưng chưa đưa vào form.

Bước tiếp theo nên sửa theo hướng mở rộng `DanhGiaDuAnThongKeViewModel` và `LoadThongTinHoTroDanhGiaAsync(...)` để đưa thêm chỉ số đã có trong source/database, sau đó nhóm lại `_ProjectStats.cshtml` theo Tiến độ / Công việc / Ngân sách / Báo cáo / Đề xuất / Nhân sự / AI. Chưa nên đổi workflow, quyền, controller route hoặc database nếu chỉ cần cải thiện dữ liệu hỗ trợ.

# Danh sách file source đã rà soát

- `QuanLyDuAn/QuanLyDuAn/Controllers/DanhGiaDuAnController.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IDanhGiaDuAnService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiDatasetService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/AiApiService.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/TienDoCongViecService.cs`
- `QuanLyDuAn/QuanLyDuAn/Controllers/TienDoCongViecController.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnPageViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnItemViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnFormViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnThongKeViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnTieuChiViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnChiTietViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnAiInsightViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiPredictPageViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/ViewModels/Ai/AiDatasetApiViewModels.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/Index.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_Filter.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_SummaryCards.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_Table.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_Form.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_ProjectStats.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_AiInsightCard.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_ScoreBadge.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_TrangThaiDanhGiaBadge.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_EmptyState.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/ChiTiet.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DanhGiaDuAn.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/CtDanhGiaDuAn.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/TieuChiDanhGia.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DuAn.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/CongViec.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/CtCongViec.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/TienDoCongViec.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/NganSach.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/ChiPhi.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/AiDataset.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/AiKetQua.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/AiNguyenNhan.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DmNguyenNhan.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DeXuatCongViec.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/DeXuatNganSach.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/NhanVienDuAn.cs`
- `QuanLyDuAn/QuanLyDuAn/Models/Entities/YeuCauDoiQuanLy.cs`
- `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`
- `QuanLyDuAn/QuanLyDuAn/Data/KhoiTaoTaiKhoanMacDinh.cs`
- `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs`
- `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`
