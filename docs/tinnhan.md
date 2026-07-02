# Phân tích chức năng chat/tin nhắn và nguyên nhân tải chậm

## 1. Mục tiêu và phạm vi

Tài liệu này phân tích trạng thái **AS-IS** của chức năng Chat dự án trong source hiện tại, tập trung vào thời gian tải danh sách phòng, lịch sử tin nhắn, cách render/gửi dữ liệu, quyền truy cập, truy vấn EF Core, index và phương án cải tiến phù hợp với cách triển khai hiện có.

Phạm vi là đọc source và lập phương án. Tài liệu này không thay đổi Controller, Service, View, JavaScript, CSS, cấu hình, CSDL hoặc migration; không tự bổ sung phân trang hay SignalR.

Nguồn chính đã đối chiếu:

- `QuanLyDuAn/QuanLyDuAn/Controllers/ChatDuAnController.cs`.
- `Services/Interfaces/IChatDuAnService.cs`, `Services/Implementations/ChatDuAnService.cs`.
- Các điểm tích hợp chat trong `DuAnService`, `NhanVienDuAnService`, `TeamDuAnService`, `CauHinhDichVu.cs`, permission helper/definition.
- Toàn bộ 7 file trong `Views/ChatDuAn/`, `_Layout.cshtml`, 4 ViewModel chat.
- 6 Entity liên quan, `QuanLyDuAnDbContext`, migration khởi tạo và model snapshot.
- Toàn bộ 7 file CSS trong `wwwroot/css/ChatDuAn/`; không có file JavaScript chat riêng, chỉ có một đoạn script nội tuyến ở `Views/ChatDuAn/Index.cshtml`.
- `Program.cs`, `.csproj`, hai file `appsettings`, `README.md`, `docs/kiemtratongquat.md`, hai tài liệu UI chat hiện có và các sơ đồ activity/deployment trong `uml/`.

Các nhận định về số query là suy ra từ các điểm thực thi `ToListAsync`, `FirstOrDefaultAsync`, `AnyAsync`, `SaveChangesAsync` trong source. Muốn kết luận thời gian tuyệt đối vẫn phải đo SQL và thời gian runtime.

## 2. Kiến trúc chat hiện tại

Chat là module Razor MVC đồng bộ:

1. Trình duyệt gọi `ChatDuAnController`.
2. Controller kiểm tra permission, gọi `IChatDuAnService`.
3. `ChatDuAnService` dùng trực tiếp `QuanLyDuAnDbContext` để kiểm tra scope, bảo đảm phòng, đồng bộ thành viên, đọc/ghi `PHONG_CHAT`, `THANH_VIEN_PHONG_CHAT`, `TIN_NHAN`.
4. Controller render Razor View/partial thành HTML.
5. SQL Server là nguồn sự thật.

Không có repository riêng cho chat. `CauHinhDichVu.AddTangDichVu()` đăng ký `IChatDuAnService` → `ChatDuAnService` theo scoped lifetime.

Hiện có ba action:

| Action | HTTP | Kết quả |
|---|---|---|
| `ChatDuAnController.Index(int? maDuAn, string? tuKhoa)` | GET | Dựng toàn bộ trang: danh sách phòng và tối đa 200 tin của phòng được chọn |
| `GuiTinNhan(...)` | POST | Ghi tin rồi `RedirectToAction(Index)`; trình duyệt tải lại toàn bộ trang |
| `TinNhan(int maPhongChat)` | GET | Trả partial `_MessageList`, nhưng giao diện hiện tại không có JavaScript gọi action này |

Source không có endpoint phân trang phòng, endpoint tải tin cũ, polling hoặc push realtime.

## 3. Entity và bảng dữ liệu liên quan

| Entity/bảng | Trường được chat dùng | Nhận xét |
|---|---|---|
| `PhongChat` / `PHONG_CHAT` | `MaPhongChat`, `MaDuAn`, `TenPhong`, `IsDeleted` | Mỗi dự án được code lấy phòng không xóa có `MaPhongChat` nhỏ nhất; DB không thể hiện unique một phòng/dự án |
| `TinNhan` / `TIN_NHAN` | `MaTinNhan`, `MaPhongChat`, `MaNguoiDung`, `NoiDungTinNhan`, `ThoiGianGui`, `IsDeleted` | Không có file đính kèm, trạng thái đọc hoặc delivery state |
| `ThanhVienPhongChat` / `THANH_VIEN_PHONG_CHAT` | khóa kép `MaPhongChat`, `MaNguoiDung`, `VaiTroTrongPhongChat` | Khóa kép vừa chống trùng vừa hỗ trợ lọc theo phòng |
| `DuAn` / `DU_AN` | `MaDuAn`, người quản lý, tên, trạng thái, `IsDeleted` | Scope gồm người quản lý hoặc thành viên trực tiếp |
| `NhanVienDuAn` / `NHAN_VIEN_DU_AN` | khóa dự án-người dùng, vai trò | Nguồn đồng bộ thành viên và vai trò phòng |
| `NguoiDung` / `NGUOI_DUNG` | tên, avatar, `IsDeleted`, liên kết Identity | Được join khi dựng tin nhắn và dùng để loại tài khoản đã xóa |
| Identity role tables | user-role-role name | Dùng để chặn Admin và loại Admin khỏi thành viên phòng |

Không có Entity/bảng cho “đã đọc”, số tin chưa đọc, typing, online/offline hoặc attachment. Vì vậy source hiện tại **không tính số tin chưa đọc**; `SoTinNhan` là tổng số tin chưa xóa của phòng.

`QuanLyDuAnDbContext` không có global query filter; chat tự ghi điều kiện `IsDeleted != true` ở các query chính.

## 4. Role, permission và scope chat

- Controller và Service đều kiểm tra `Chat.Xem` cho xem trang/tải tin.
- Controller và Service đều kiểm tra `Chat.Gui` cho gửi.
- `PermissionDependencyProvider` khai báo `Chat.Gui` phụ thuộc `Chat.Xem`.
- `KiemTraKhongChoAdminChatDuAn` được gọi trong các luồng trang, phòng, tin và gửi; Admin bị chặn dù có thể có claim `Chat.Xem`.
- `_Layout.cshtml` chỉ hiện menu khi có quyền chat và không phải Admin.
- Scope dự án trong `LayDanhSachMaDuAnTheoScopeAsync` là hợp của:
  - dự án chưa xóa do người dùng quản lý;
  - dự án chưa xóa có dòng `NhanVienDuAn` của người dùng.
- `CoQuyenVaoPhongChatAsync` không tin membership truyền từ client; nó suy ra dự án từ phòng rồi gọi `CoQuyenVaoDuAnAsync`.
- `GetTinNhanAsync` và `GuiTinNhanAsync` kiểm tra lại quyền ở backend. Gọi trực tiếp action partial vẫn không bỏ qua scope.
- Trước khi đọc/gửi, Service còn đồng bộ thành viên phòng. Khi gửi, sau đồng bộ phải tồn tại dòng `ThanhVienPhongChat`.
- Dự án `Đã hủy` hoặc `Lưu trữ` không được gửi; source không chặn đọc.

Điểm cần giữ nguyên nếu bổ sung endpoint: mọi endpoint tải thêm/tải cũ và mọi Hub đều phải kiểm tra `Chat.Xem`/`Chat.Gui`, scope dự án, membership và quy tắc chặn Admin; không chỉ dựa vào việc ẩn nút ở View.

## 5. Luồng mở màn hình chat

Luồng chính:

1. Người dùng mở `/ChatDuAn/Index`.
2. `ChatDuAnController.Index` kiểm tra `Permissions.Chat.Xem`.
3. `ChatDuAnService.GetPageAsync`:
   - kiểm permission lần nữa;
   - lấy `MaNguoiDung`, role và chặn Admin;
   - gọi `LayDanhSachMaDuAnTheoScopeAsync`;
   - gọi `GetPhongChatDuocThamGiaAsync(tuKhoa)`;
   - chọn phòng khớp `maDuAn`, nếu không truyền thì chọn phần tử đầu tiên;
   - gọi `GetTinNhanAsync` cho phòng đã chọn;
   - đánh dấu `DangChon`;
   - gọi `CoTheGuiTinNhanAsync`;
   - tạo `ChatDuAnPageViewModel`.
4. Controller tải tập permission cấp cho user rồi render `Views/ChatDuAn/Index.cshtml`.
5. `Index.cshtml` render `_RoomList`, `_ChatHeader`, `_MessageList`, `_MessageForm`.
6. JavaScript nội tuyến chỉ cuộn `#chat-message-list` xuống đáy; không tải thêm dữ liệu.

`GetPageAsync` gọi scope trước để kiểm tra rỗng, rồi `GetPhongChatDuocThamGiaAsync` lại lấy current user, role và scope. Đây là truy vấn lặp ở cấp lời gọi, chưa phải N+1 theo bản ghi nhưng làm tăng số round-trip.

## 6. Luồng tải danh sách phòng

`ChatDuAnService.GetPhongChatDuocThamGiaAsync` thực hiện:

1. Lấy user, role, scope dự án.
2. Chạy vòng lặp qua **toàn bộ mã dự án trong scope** và `await DamBaoPhongChatDuAnAsync(maDuAn)`.
3. Query tất cả phòng không xóa thuộc tất cả dự án không xóa trong scope; nếu có từ khóa thì lọc tên phòng/tên dự án trên SQL.
4. Sắp xếp `TenDuAn`, rồi `MaPhongChat`.
5. Với toàn bộ `roomIds`, chạy ba query tổng hợp:
   - số thành viên theo phòng;
   - tổng số tin chưa xóa theo phòng;
   - **toàn bộ các tin chưa xóa của các phòng**, sắp giảm dần theo thời gian/ID.
6. Query thứ ba được materialize bằng `ToListAsync`, sau đó mới `GroupBy` trong bộ nhớ để lấy dòng đầu mỗi phòng.

### Bảng hiện trạng danh sách phòng

| Nội dung | Hiện trạng source | Ảnh hưởng hiệu năng | Kết luận |
|---|---|---|---|
| Số phòng tải ban đầu | Tất cả phòng của tất cả dự án trong scope; không `Take` | Response/DOM tăng tuyến tính theo số phòng | Cần giới hạn |
| Cách sắp xếp phòng | `TenDuAn`, sau đó `MaPhongChat` | Ổn định nhưng không ưu tiên hoạt động mới | Chấp nhận nghiệp vụ hiện tại, chưa phù hợp sidebar chat lớn |
| Tìm kiếm phòng | GET server-side theo tên phòng hoặc tên dự án | Không tải toàn bộ phòng khớp ngoài kết quả, nhưng vẫn đồng bộ toàn bộ dự án trước khi lọc | Có lọc server; cần đưa giới hạn/paging vào query |
| Trạng thái dự án | Hiển thị; chỉ lọc `IsDeleted`, không lọc trạng thái | Bao gồm đang làm, hoàn thành, hủy, lưu trữ nếu không xóa | Cần xác nhận nghiệp vụ; không tự loại |
| Tin nhắn mới nhất | Tải mọi tin của mọi phòng rồi lấy `First` sau `GroupBy` client | Có thể rất lớn dù mỗi phòng chỉ cần một dòng | Cần tối ưu cao |
| Số tin chưa đọc | Không có | Không gây query; UI hiện không có unread | Không thuộc tối ưu ban đầu |
| Thành viên phòng | Một query `GroupBy` chung để đếm | Không N+1 ở bước đếm | Chấp nhận được |
| Phân trang | Không có `Skip/Take`, cursor hay page model | Tất cả phòng và card được tải/render | Có vấn đề khi nhiều phòng |
| Tải lười | Không có | Mở trang phải hoàn tất mọi query/phòng | Cần bổ sung ở bước sau |
| AJAX/partial | Danh sách phòng không có AJAX | Tìm/lọc/chọn phòng đều reload toàn trang | Cần cải thiện sau khi giới hạn query |
| Render HTML | `_RoomList` `foreach` mọi phòng | Hàng trăm card tạo nhiều node, cả mobile và desktop | CSS scroll không giảm dữ liệu/DOM |

### Trả lời các điểm bắt buộc

- Danh sách gồm tất cả dự án người dùng quản lý hoặc tham gia trực tiếp, miễn `DuAn.IsDeleted != true`; không lọc theo trạng thái.
- Dự án hoàn thành, hủy hoặc lưu trữ vẫn có thể xuất hiện; dự án xóa mềm bị loại. Source không có trạng thái “xóa mềm phòng nhưng vẫn hiện”.
- Không lọc phòng chưa có tin; chúng hiển thị “Chưa có tin nhắn”.
- Không sắp theo dự án mới nhất, tin mới nhất hoặc hoạt động gần nhất.
- Search là server-side khi submit. Không có debounce và không submit mỗi lần gõ.
- Nút “Lọc” là form GET, reload toàn trang.
- `.chat-room-items { overflow-y: auto; }` chỉ tạo vùng cuộn. Server vẫn trả và Razor vẫn render toàn bộ card.
- Ẩn/cắt text bằng CSS không làm response, số query hoặc số DOM node nhỏ hơn.

## 7. Luồng tải lịch sử tin nhắn

`ChatDuAnService.GetTinNhanAsync(maPhongChat)`:

1. Kiểm tra `Chat.Xem`, user, role và chặn Admin.
2. Query phòng không xóa để lấy `MaDuAn`.
3. Kiểm tra scope phòng qua `CoQuyenVaoPhongChatAsync` → `CoQuyenVaoDuAnAsync`.
4. Đồng bộ thành viên phòng.
5. Join `TIN_NHAN` với `NGUOI_DUNG`.
6. Lọc đúng phòng, tin chưa xóa, người gửi chưa xóa.
7. Sắp `ThoiGianGui DESC`, `MaTinNhan DESC`.
8. `Take(GioiHanTinNhan)` với hằng số `200`.
9. Projection trực tiếp sang `ChatDuAnTinNhanItemViewModel`, rồi `Reverse()` để hiển thị cũ → mới.

Kết luận chính:

- Không tải toàn bộ lịch sử không giới hạn; tải tối đa **200 tin mới nhất**.
- Không có `Skip`, page number, cursor, `hasMore` hoặc cơ chế tải tin cũ hơn.
- Khóa phụ `MaTinNhan DESC` làm thứ tự ổn định khi hai tin có cùng thời gian.
- Người gửi được lấy bằng một join, không query riêng từng tin.
- Không tải attachment vì schema/ViewModel hiện không có attachment.
- Không dùng `Include`; projection chỉ lấy đúng các trường hiển thị.
- Query không viết `.AsNoTracking()`, nhưng vì kết quả là DTO scalar projection nên EF không track Entity kết quả.
- Tin xóa mềm và tin của user đã xóa bị loại.

| Thành phần | Hiện trạng | Vấn đề | Hướng đề xuất |
|---|---|---|---|
| Lần tải đầu | Tối đa 200 tin của phòng được chọn, kèm toàn trang | 200 bubble/avatar có thể nặng; nhiều query đồng bộ trước đó | Ban đầu 30–50 tin |
| Tải tin cũ | Không có | Không xem được tin cũ hơn 200 | Cursor “cũ hơn” |
| Gửi tin | POST Controller → SaveChanges → redirect Index | Query/render lại toàn bộ phòng và 200 tin | Giai đoạn 2 dùng POST JSON/partial; giai đoạn 3 broadcast sau commit |
| Chuyển phòng | Link tới `Index?maDuAn=...` | Reload và dựng lại toàn bộ danh sách phòng | AJAX vùng chat, giữ sidebar và phòng chọn |
| Người gửi | Join `NguoiDung`, projection | Tốt, không N+1 | Giữ projection |
| File/ảnh | Chỉ URL avatar; không attachment | Avatar có thể lặp request tùy cache/server, nhưng source chưa chứng minh là nút thắt | Đo Network/cache trước khi tối ưu |
| Quyền truy cập | Kiểm permission, scope, Admin, đồng bộ membership | Nhiều query nhưng kiểm soát backend khá đầy đủ | Giữ nguyên rule trong endpoint mới |

Action `ChatDuAnController.TinNhan` có thể trả partial `_MessageList`, nhưng không có JavaScript sử dụng. Vì vậy nó không làm giao diện hiện tại thành AJAX.

## 8. Luồng gửi tin nhắn

1. `_MessageForm.cshtml` submit form POST `GuiTinNhan`.
2. Controller kiểm `Chat.Gui`; ModelState lỗi cũng redirect về `Index`.
3. `ChatDuAnService.GuiTinNhanAsync`:
   - kiểm permission, user, role, chặn Admin;
   - validate phòng, nội dung không rỗng và tối đa 2.000 ký tự;
   - join phòng-dự án, lọc soft delete;
   - kiểm scope phòng;
   - chặn gửi nếu dự án `Đã hủy` hoặc `Lưu trữ`;
   - đồng bộ thành viên;
   - kiểm membership còn hợp lệ;
   - insert `TinNhan` và `SaveChangesAsync`.
4. Controller redirect về `Index`.
5. GET mới tải lại danh sách phòng, tổng số tin, tin mới nhất và tối đa 200 tin lịch sử.

Không trả JSON, không append DOM, không partial sau gửi, không disable nút chống double-submit và không có loading indicator. `GuiTinNhan` cũng không thấy `[ValidateAntiForgeryToken]`; đây là vấn đề bảo vệ POST đã được tài liệu tổng quát lưu ý, nhưng không phải nguyên nhân chính của tốc độ tải.

## 9. Phân tích query EF Core

### 9.1 Các query chính

| File/method | Mục đích và bảng | Hình dạng query | Đánh giá |
|---|---|---|---|
| `ChatDuAnService.LayDanhSachMaDuAnTheoScopeAsync` | Scope từ `DU_AN`, `NHAN_VIEN_DU_AN` + `DU_AN` | Hai query `ToListAsync`, hợp/Distinct trong memory; không paging | Chấp nhận được với scope nhỏ; lặp lại nhiều lần trong một request |
| `GetPhongChatDuocThamGiaAsync` query `roomRows` | Phòng + dự án | Join, soft-delete, `Contains(dsMaDuAn)`, search, projection, order; không paging | Cần tối ưu vì lấy toàn bộ phòng |
| `GetPhongChatDuocThamGiaAsync` đếm thành viên | `THANH_VIEN_PHONG_CHAT` | `WHERE roomIds.Contains`, `GroupBy`, `Count DISTINCT` | Tốt; một query chung |
| `GetPhongChatDuocThamGiaAsync` đếm tin | `TIN_NHAN` | lọc phòng/soft-delete, `GroupBy`, `Count` | Chấp nhận được; cần đo khi bảng rất lớn |
| `GetPhongChatDuocThamGiaAsync` tin mới nhất | `TIN_NHAN` | lọc phòng/soft-delete, order toàn tập, projection, `ToListAsync`; `GroupBy` sau materialize | Cần tối ưu cao; tải entity scalar của mọi tin thay vì một tin/phòng |
| `GetTinNhanAsync` lịch sử | `TIN_NHAN` join `NGUOI_DUNG` | scope checks; order thời gian + ID; projection; `Take(200)` | Chấp nhận được nhưng page đầu quá lớn và không tải cũ |
| `DongBoThanhVienPhongChatAsync` | dự án, phòng, thành viên dự án, user, role, thành viên phòng | Nhiều query tuần tự và có thể `SaveChanges` | Hợp lý khi mutation staffing; không phù hợp chạy theo mọi dự án mỗi GET |
| `CoTheGuiTinNhanAsync` | phòng, dự án, scope, membership | Nhiều query nhỏ tuần tự | Đúng nghiệp vụ nhưng có thể gộp/cached trong page request |

### 9.2 N+1 và số query dự kiến

N+1 đã được chứng minh ở `GetPhongChatDuocThamGiaAsync`:

```text
foreach (var maDuAn in dsMaDuAn)
    await DamBaoPhongChatDuAnAsync(maDuAn);
```

Với phòng đã tồn tại, một lần `DamBaoPhongChatDuAnAsync` có thể thực hiện khoảng 8 query: đọc dự án, tìm phòng, rồi trong đồng bộ lại đọc dự án, tìm phòng, đọc thành viên dự án, user hợp lệ, Admin và thành viên phòng. Có thể có thêm `SaveChanges` nếu dữ liệu lệch. Vì vậy phần này tăng xấp xỉ theo `N` dự án và là N+1/chuỗi query theo từng bản ghi, không chỉ là rủi ro giả định.

Ngoài vòng lặp, một request `Index` còn lặp truy vấn user/role/scope giữa `GetPageAsync`, `GetPhongChatDuocThamGiaAsync`, `GetTinNhanAsync`, kiểm scope và `CoTheGuiTinNhanAsync`. Số chính xác phụ thuộc nhánh quyền, phòng có sẵn và có cần đồng bộ hay không.

Không phát hiện:

- `Include`/`ThenInclude` sâu;
- query DB trong vòng lặp mapping `roomRows.Select`;
- query riêng để lấy tin mới nhất hoặc người gửi cho từng phòng/tin.

Tuy nhiên query tin mới nhất là vấn đề theo **số dòng**: một query chung nhưng materialize tất cả tin của tất cả phòng.

### 9.3 Tracking và projection

- `DongBoThanhVienPhongChatAsync` dùng `AsNoTracking` khi đọc dự án; `CoQuyenVaoDuAnAsync` và `CoTheGuiTinNhanAsync` cũng có một số query `AsNoTracking`.
- Các query DTO/anonymous projection không viết `AsNoTracking`, nhưng EF không track Entity thuần khi projection chỉ có scalar.
- `DamBaoPhongChatDuAnAsync`/`DamBaoPhongChatNoiBoAsync` đọc Entity có tracking vì có thể tạo/sử dụng trong cùng unit of work. Không nên gắn nhãn đây là lỗi chỉ vì thiếu `AsNoTracking`.
- `XoaToanBoThanhVienPhongChatAsync` và tải `currentMembers` cần tracking để xóa/cập nhật.

### 9.4 Cách đo runtime cần thực hiện

Source chứng minh hình dạng tải, nhưng để định lượng cần:

- đo tổng thời gian `ChatDuAnController.Index`;
- bật EF Core command logging/diagnostic listener ở môi trường kiểm thử để đếm query và thời gian từng SQL;
- ghi số dự án scope, số phòng, số tin được đọc bởi query tin mới nhất và số tin lịch sử;
- đo kích thước HTML response;
- dùng browser Performance/Network đo TTFB, parse/render, số DOM node, ảnh avatar và cache;
- so sánh mốc 5, 50, 500 phòng và 10, 1.000, 100.000 tin/phòng.

Các kết luận về CPU SQL, execution plan, lock hoặc tốc độ máy chủ phải ghi: **Cần đo SQL và thời gian runtime**.

## 10. Phân tích nguyên nhân tải chậm

### Đã chứng minh từ source

1. Danh sách phòng không giới hạn, không phân trang và render toàn bộ card.
2. Mỗi GET chạy đồng bộ/bảo đảm phòng theo vòng lặp toàn bộ dự án, tạo nhiều round-trip DB và có thể ghi DB.
3. Query tin mới nhất đọc toàn bộ tin chưa xóa của mọi phòng rồi mới lấy một dòng/phòng trong bộ nhớ.
4. Query tổng số tin phải quét/aggregate tập tin theo mọi phòng đang hiển thị.
5. Mở, tìm, chọn phòng và gửi tin đều dựng lại toàn trang.
6. Lịch sử ban đầu lấy tối đa 200 tin và render tối đa 200 partial `_MessageItem`.
7. Mobile nhận cùng lượng dữ liệu như desktop; media query chỉ đổi bố cục.

### Rủi ro cần đo

- Index đơn `TIN_NHAN.MaPhongChat` có đủ cho lọc + soft-delete + order hay không.
- Avatar có tải chậm do cache/đường truyền hay không.
- SQL Server, IIS/Kestrel, RAM/CPU hoặc latency DB của môi trường deploy có phải nút thắt.
- Kích thước response và số DOM node thực tế ở dữ liệu production.

### Không phải nguyên nhân theo source hiện tại

- Không có `Include/ThenInclude` sâu.
- Không tải lịch sử của tất cả phòng khi mở trang; chỉ query lịch sử phòng được chọn.
- Không có query người gửi riêng cho từng tin.
- Không tính số chưa đọc.
- Không có polling hoặc SignalR đang tiêu thụ tài nguyên.

## 11. Phân tích danh sách phòng quá nhiều

Nguyên nhân chính là tổng hợp của ba lớp:

- **DB round-trip:** vòng lặp bảo đảm/đồng bộ theo từng dự án.
- **Khối lượng dữ liệu:** không giới hạn phòng và tải mọi tin để suy ra tin mới nhất.
- **Render:** mọi phòng thành một `<a class="chat-room-item">` có nhiều node con.

Việc sidebar có scrollbar không giải quyết ba lớp này. Với 500 phòng, server vẫn truy vấn dữ liệu cho 500 phòng và Razor vẫn tạo 500 card trước khi CSS cho cuộn.

Tình trạng phòng hiện tại:

- Phòng được lazy-create trong GET nếu dự án chưa có phòng.
- `DuAnService` đã gọi `DamBaoPhongChatDuAnAsync` khi tạo dự án.
- `NhanVienDuAnService` và `TeamDuAnService` đã gọi đồng bộ khi thay đổi staffing/team.
- Vì đã có các điểm đồng bộ theo mutation, việc lặp lại toàn bộ đồng bộ mỗi lần tải danh sách là ứng viên tối ưu quan trọng. Tuy vậy vẫn cần giữ một cơ chế tự phục hồi có kiểm soát cho dữ liệu cũ/lệch, không đơn giản xóa bảo vệ.

## 12. Phân tích lịch sử tin nhắn quá nhiều

Source không tải vô hạn mà lấy 200 tin mới nhất. Do đó:

- Với phòng 1.000 hoặc 100.000 tin, page đầu vẫn chỉ trả 200 dòng lịch sử.
- Query vẫn cần tìm 200 dòng mới nhất theo phòng và thứ tự; hiệu năng phụ thuộc execution plan/index.
- 200 tin là khá nhiều cho lần render đầu và không có cách truy cập tin thứ 201 trở về trước.
- Sau gửi, toàn bộ 200 tin và toàn bộ sidebar được query/render lại.

Nguyên nhân chậm lịch sử vì vậy có khả năng là page đầu 200 + các query quyền/đồng bộ + full reload, không phải “tải toàn bộ lịch sử”. Cần đo SQL và thời gian runtime để tách chi phí query lịch sử khỏi chi phí chuẩn bị trang.

## 13. Kiểm tra index hiện tại

Mapping/migration hiện có:

| Bảng | Query thường dùng | Index hiện có | Có thể thiếu | Mức cần thiết |
|---|---|---|---|---|
| `PHONG_CHAT` | `MaDuAn`, `IsDeleted`, lấy phòng đầu | PK `MaPhongChat`; `IX_PHONG_CHAT_MaDuAn` | Composite/filtered theo `MaDuAn, IsDeleted`; unique một phòng/dự án chỉ khi nghiệp vụ xác nhận | Tùy execution plan; chưa thực hiện |
| `TIN_NHAN` | phòng + `IsDeleted` + order `ThoiGianGui, MaTinNhan` | PK `MaTinNhan`; index đơn `MaPhongChat`; index đơn `MaNguoiDung` | Composite phục vụ lọc/order, ví dụ bắt đầu `MaPhongChat, IsDeleted, ThoiGianGui, MaTinNhan` | Đáng đánh giá sau đo plan |
| `THANH_VIEN_PHONG_CHAT` | thành viên theo phòng; membership phòng-user | PK kép `(MaPhongChat, MaNguoiDung)`; index `MaNguoiDung` | Chưa thấy thiếu cho query hiện tại | Đủ về hình dạng hiện tại |
| `NHAN_VIEN_DU_AN` | scope theo user; thành viên theo dự án | PK bắt đầu bằng `MaDuAn`; index `MaNguoiDung` | Có thể cần composite theo user-dự án chỉ nếu plan cho thấy | Chưa đủ bằng chứng |
| `DU_AN` | quản lý + soft-delete; PK lookup | PK `MaDuAn`; các index do FK/migration hiện hành | Query scope quản lý có thể cần index theo `MaNguoiDung, IsDeleted` | Ngoài riêng chat; cần đo |
| `NGUOI_DUNG` | join PK, lọc `IsDeleted` | PK `MaNguoiDung` và các index Identity/FK | Không cần index riêng chỉ để join PK hiện tại | Thấp |

`TIN_NHAN` chưa có trường trạng thái đã đọc nên không có index unread. Không nên đề xuất index cho trạng thái không tồn tại.

Mọi index composite/filtered nêu trên là **Đề xuất tùy chọn, chưa thực hiện**. Chỉ quyết định sau khi lấy SQL sinh ra, execution plan, logical reads và dữ liệu gần production; query/application nên được sửa giảm khối lượng trước khi mặc định thêm index.

## 14. Đánh giá phân trang phòng chat

### Phương án A – phân trang số trang

- Dễ ghép với MVC GET và các mẫu paging khác.
- Search/filter server-side rõ ràng.
- Không tự nhiên với sidebar chat; đổi trang dễ mất ngữ cảnh phòng chọn.
- Nếu sắp theo hoạt động mới, tin mới có thể làm phòng nhảy trang và gây lệch offset.

Đánh giá: dễ triển khai nhất nhưng trải nghiệm kém hơn “tải thêm” cho sidebar.

### Phương án B – tải thêm phòng

- Ban đầu 20–30 phòng, nút “Tải thêm phòng” hoặc infinite scroll.
- Giữ được phòng đang chọn và sidebar hiện có.
- Phải chống request trùng, giữ cursor, merge theo `MaPhongChat`.
- Search phải reset danh sách và chạy server-side.
- Khi phòng có tin mới, client phải đưa/cập nhật phòng đúng vị trí mà không nhân đôi.

Đánh giá: phù hợp giao diện hiện tại nhất nếu thứ tự phòng được xác định ổn định.

### Phương án C – phòng hoạt động gần đây + search server

- Ban đầu 20–30 phòng có hoạt động gần nhất.
- Search server tìm cả phòng chưa nằm trong batch.
- Phòng chưa có tin có thể không có “thời gian hoạt động”; cần khóa sắp xếp phụ, ví dụ ưu tiên/ghép theo `MaPhongChat` hoặc ngày dự án.
- Nếu chỉ hiện phòng có tin, dự án mới sẽ khó tìm; vì vậy không được loại phòng chưa có tin.

Đánh giá: phù hợp nghiệp vụ chat lớn, nhưng cần định nghĩa rõ thứ tự cho phòng chưa có tin.

### Khuyến nghị

Chọn kết hợp **B + C**:

- page đầu 20–30 phòng theo hoạt động gần nhất;
- vẫn gồm phòng chưa có tin bằng khóa sắp xếp fallback rõ ràng;
- search server trên toàn scope;
- “Tải thêm phòng” trước, sau đó mới cân nhắc infinite scroll;
- cursor ổn định dựa trên thời gian tin mới nhất (nullable) + `MaPhongChat`, không dùng page number nếu danh sách thường đổi vì tin mới.

Trước khi làm UI tải thêm, phải loại vòng lặp đồng bộ theo từng dự án khỏi hot path và chuyển query tin mới nhất thành projection/subquery chỉ lấy một dòng mỗi phòng. Nếu không, endpoint 20 phòng vẫn có thể đồng bộ toàn bộ scope.

## 15. Đánh giá phân trang lịch sử tin nhắn

### Offset pagination

`Skip(page * pageSize).Take(pageSize)` dễ hiểu và dễ dựng. Tuy nhiên offset lớn tốn chi phí bỏ qua dòng; khi tin mới chèn vào, ranh giới page có thể dịch và gây trùng/thiếu.

### Keyset/cursor pagination

Phù hợp hơn cho chat. Cursor dùng cặp:

```text
(ThoiGianGui, MaTinNhan)
```

Tải tin cũ hơn bằng điều kiện khái niệm:

```text
ThoiGianGui < cursorTime
OR (ThoiGianGui == cursorTime AND MaTinNhan < cursorId)
```

Khuyến nghị:

- Page đầu lấy 30–50 tin mới nhất.
- SQL sắp `ThoiGianGui DESC, MaTinNhan DESC`, giới hạn server tối đa 50.
- Lấy thêm một dòng hoặc dựa vào batch đầy để xác định `hasMore`; cách chắc hơn là `Take(pageSize + 1)`.
- Trước render, đảo batch thành cũ → mới như source hiện tại.
- Khi người dùng cuộn lên đầu, hiện nút “Tải tin cũ hơn” ở bản đầu để kiểm soát request; infinite scroll là bước sau.
- Khi chèn tin cũ ở đầu DOM, ghi `scrollHeight`/`scrollTop` trước và bù chênh lệch sau để giữ vị trí đọc.
- Khóa request trong lúc tải, lưu cursor cũ nhất, loại trùng bằng `MaTinNhan`.
- Nếu `ThoiGianGui` nullable, cần chuẩn hóa quy tắc hiện tại `DateTime.MinValue`; tốt hơn là bảo đảm cursor/SQL dùng cùng semantics mà không đổi schema.

Keyset không cần thay đổi CSDL để hoạt động. Index phù hợp có thể cải thiện đáng kể nhưng là quyết định riêng sau đo plan.

## 16. Đánh giá SignalR

### 16.1 Hiện trạng SignalR

Đã kiểm tra `.csproj`, `Program.cs`, source C#, Razor và JavaScript:

- Không có package reference SignalR client/server riêng trong `.csproj`. ASP.NET Core shared framework có thể chứa assembly server, nhưng điều đó không có nghĩa ứng dụng đã cấu hình SignalR.
- Không có `AddSignalR()`.
- Không có `MapHub()`.
- Không có class kế thừa `Hub`, `IHubContext` hoặc `HubConnectionBuilder`.
- Không có JavaScript SignalR client.
- Không có polling định kỳ hoặc AJAX kiểm tra tin mới.
- Action partial `TinNhan` tồn tại nhưng không được JavaScript hiện tại gọi.

Kết luận: source ứng dụng hiện tại chưa triển khai realtime push hay polling.

### 16.2 SignalR có làm tải lần đầu nhanh hơn không?

Không. SignalR không thay thế:

- giới hạn danh sách phòng;
- tối ưu vòng lặp đồng bộ;
- query một tin mới nhất/phòng;
- page đầu 30–50 tin;
- giảm HTML/DOM.

Nếu thêm SignalR trước, GET đầu vẫn chậm như cũ và còn thêm kết nối realtime.

### 16.3 SignalR có cần cho trải nghiệm realtime không?

Có giá trị ở giai đoạn sau: hiện người dùng chỉ thấy tin mới sau khi reload, chọn lại phòng hoặc gửi tin khiến redirect. Với chat trao đổi dự án, push tới người đang mở phòng là cải thiện hợp lý.

Source không cung cấp số người đồng thời/phòng hoặc topology production thực tế. Sơ đồ deploy chỉ mô tả browser → ASP.NET MVC trên IIS/Kestrel → SQL Server, reverse proxy là khả năng/khuyến nghị; không có file Nginx trong repository. Vì vậy:

- Một instance: SignalR đơn giản, group theo phòng là vừa đủ; không cần sticky session/backplane.
- Nhiều instance: cần sticky session theo yêu cầu hosting hoặc Redis/Azure SignalR/backplane phù hợp; không thể kết luận từ source.
- Nếu dùng Nginx: phải proxy WebSocket/HTTP upgrade, timeout phù hợp và HTTPS/WSS. Repository không có cấu hình để xác nhận.
- `Program.cs` dùng HTTPS redirection ngoài Development, nhưng HTTPS production cuối cùng vẫn phụ thuộc host/reverse proxy.

### 16.4 Thiết kế đề xuất ở mức tài liệu

Ưu tiên giữ gửi qua Controller/Service, Hub chỉ broadcast:

```text
Client POST gửi tin
→ Controller kiểm Chat.Gui
→ Service kiểm scope, Admin, membership, trạng thái dự án
→ lưu TIN_NHAN
→ SaveChangesAsync thành công
→ publisher/IHubContext gửi sự kiện tới group phòng
→ client đang mở phòng append theo MaTinNhan
```

Lý do: ít phá vỡ nghiệp vụ hiện tại hơn gửi trực tiếp qua Hub; validation và transaction vẫn ở Service. Hub/group phải:

- chỉ cho người có `Chat.Xem`, đúng scope và membership join group;
- không tin `maPhongChat` client;
- người không có `Chat.Gui` không được gửi;
- Admin vẫn bị chặn;
- SQL Server là nguồn sự thật, không lưu tin chỉ trong memory;
- không broadcast trước khi commit;
- sự kiện có `MaTinNhan` server để chống trùng với response ở máy gửi;
- bật reconnect; sau reconnect tải bù từ DB theo ID/cursor;
- khi membership bị gỡ, endpoint/reconnect phải từ chối; muốn ngắt ngay kết nối đang sống cần cơ chế revalidate/kick riêng.

SignalR nên làm ở **giai đoạn 3**, sau tối ưu query và phân trang.

## 17. Ảnh hưởng của cách deploy hiện tại

Thông tin xác nhận được:

- ASP.NET Core MVC .NET 8, EF Core SQL Server.
- Có thể host Kestrel/IIS; sơ đồ production dự kiến có HTTPS/reverse proxy.
- MVC và FastAPI là hai tiến trình; chat không phụ thuộc FastAPI.
- Không có Docker Compose, Nginx config hoặc cấu hình load balancer trong repository.
- Không có bằng chứng source về số instance, CPU/RAM production, latency DB, sticky session hoặc WebSocket proxy.

Phương án phù hợp nhất hiện tại là tối ưu trong MVC/EF/Razor trước, không thêm hạ tầng. Nếu sau này thêm SignalR trên một instance, group in-process đủ cho bước đầu. Chỉ thêm scale-out khi topology thực tế yêu cầu.

Nginx/server yếu có thể làm vấn đề nặng hơn, nhưng source đã có các nguyên nhân ứng dụng đủ cụ thể; không nên quy tốc độ chậm cho deploy khi chưa đo.

## 18. Phương án đề xuất theo giai đoạn

### Giai đoạn 1 – tối ưu tải ban đầu

1. Đo baseline query/time/rows/response/DOM.
2. Không đồng bộ toàn bộ dự án trên mỗi lần GET; tận dụng các điểm đồng bộ ở mutation và giữ một cơ chế repair có kiểm soát.
3. Loại truy vấn user/role/scope lặp trong cùng request.
4. Query phòng bằng projection gọn và giới hạn trước.
5. Lấy một tin mới nhất/phòng ngay trên SQL bằng subquery/group/window-compatible shape; không tải mọi tin.
6. Chỉ đếm dữ liệu thật sự cần hiện. Nếu tổng số tin không quan trọng, cân nhắc bỏ khỏi card ở yêu cầu sau.
7. Giảm page đầu tin xuống 30–50.
8. Thêm loading/disable submit khi đã chuyển sang request bất đồng bộ.

### Giai đoạn 2 – phân trang/tải lười

- 20–30 phòng gần nhất, search server, nút tải thêm.
- 30–50 tin mới nhất, cursor `(ThoiGianGui, MaTinNhan)`.
- Endpoint tải tin cũ và endpoint phòng phải kiểm quyền/scope.
- Chọn phòng bằng AJAX/partial hoặc JSON rồi cập nhật vùng chat; không reload sidebar.
- Gửi tin không redirect toàn trang; vẫn gọi nghiệp vụ Service hiện tại.

### Giai đoạn 3 – realtime

- SignalR group theo phòng.
- Controller/Service lưu DB trước, Hub broadcast sau commit.
- Reconnect, deduplicate bằng ID server, tải bù khi mất kết nối.
- Kiểm thử Nginx WebSocket/HTTPS nếu reverse proxy thực tế có dùng.

### Giai đoạn 4 – nâng cao, không bắt buộc ban đầu

- unread/đã xem;
- typing, online/offline;
- notification;
- scale-out SignalR.

Các chức năng này có thể cần mô hình dữ liệu/hạ tầng mới; không đưa vào lần tối ưu đầu.

## 19. File dự kiến cần sửa ở bước sau

Chỉ là danh sách dự kiến, chưa sửa:

| File/nhóm | Mục đích |
|---|---|
| `Services/Interfaces/IChatDuAnService.cs` | Hợp đồng page/cursor/tải thêm |
| `Services/Implementations/ChatDuAnService.cs` | Bỏ N+1 hot path, query latest gọn, cursor tin/phòng |
| `Controllers/ChatDuAnController.cs` | Endpoint tải thêm/tin cũ/gửi không full reload |
| `ViewModels/ChatDuAn/*` | Cursor, `HasMore`, page size, DTO response |
| `Views/ChatDuAn/Index.cshtml`, `_RoomList.cshtml`, `_MessageList.cshtml`, `_MessageForm.cshtml` | Placeholder/nút tải, vùng cập nhật |
| JavaScript chat mới hoặc script module scoped | Fetch, chống request trùng, giữ scroll, deduplicate |
| CSS `wwwroot/css/ChatDuAn/*` | Loading/nút tải, chỉ khi UI mới cần |
| `Program.cs` và Hub mới | Chỉ ở giai đoạn SignalR |
| Cấu hình reverse proxy ngoài repo | Chỉ nếu môi trường thật dùng SignalR qua Nginx |

`DuAnService`, `NhanVienDuAnService`, `TeamDuAnService` chỉ cần xem lại nếu thay đổi chiến lược đồng bộ/reconcile; không nên phá các điểm đồng bộ sau mutation đang có.

## 20. Có cần thay đổi CSDL hay không

Không cần thay đổi CSDL để:

- giới hạn phòng/tin;
- server-side search;
- tải thêm/cursor;
- giảm query lặp;
- query tin mới nhất gọn;
- AJAX/partial/JSON;
- SignalR group và broadcast.

Index composite cho `TIN_NHAN` hoặc index khác chỉ là **đề xuất tùy chọn, chưa thực hiện**, phụ thuộc execution plan/runtime. Unread/đã xem ở giai đoạn nâng cao có thể cần schema mới, nhưng không thuộc tối ưu bắt buộc.

## 21. Rủi ro khi chỉnh sửa

- Làm mất khả năng tự phục hồi phòng/thành viên nếu loại đồng bộ GET mà không có chiến lược repair.
- Phòng vừa có tin mới đổi vị trí trong khi user đang tải thêm, gây trùng/thiếu nếu dùng offset.
- Search + phòng đang chọn không cùng tập kết quả có thể làm mất ngữ cảnh.
- Cursor nullable/timezone không thống nhất gây bỏ sót tin; phải dùng thêm `MaTinNhan`.
- AJAX endpoint mới bỏ sót permission/scope.
- Append optimistic và event SignalR cùng đến gây tin trùng.
- Broadcast trước commit làm UI có tin không tồn tại trong DB.
- Nhiều instance không có scale-out làm mỗi client chỉ nhận event của một instance.
- Thay đổi card/scroll có thể phá layout desktop/mobile đã có vùng cuộn độc lập.
- Tối ưu index không dựa trên plan có thể tăng chi phí ghi và dung lượng mà không cải thiện query.

## 22. Test case đề xuất

### Danh sách phòng

- User có 5, 50 và 500 phòng.
- Có phòng chưa có tin.
- Có dự án hoàn thành, hủy, lưu trữ; đối chiếu đúng rule hiển thị đã thống nhất.
- Tìm theo tên/mã dự án và tên phòng; lưu ý source hiện tại chưa search theo mã số riêng nếu mã không nằm trong tên.
- Tải thêm nhiều lần, không trùng phòng.
- Phòng có tin mới được sắp đúng; phòng đang chọn không mất.
- Đo số query không tăng tuyến tính theo tổng dự án sau tối ưu.

### Tin nhắn

- Phòng có 0, 10, 1.000 và 100.000 tin.
- Page đầu đúng 30–50 tin mới nhất.
- Tải tin cũ nhiều lần, không trùng/thiếu.
- Giữ scroll khi prepend.
- Hai tin cùng thời gian sắp ổn định bằng `MaTinNhan`.
- Tin `IsDeleted = true` không hiện.
- User đã xóa: xác nhận nghiệp vụ có tiếp tục ẩn tin cũ như source hiện tại hay không.

### Quyền

- Người ngoài dự án không xem/tải thêm.
- Người bị gỡ khỏi dự án không tải thêm tin và không gửi.
- Employee chỉ thấy dự án tham gia; Manager chỉ đúng scope.
- Admin vẫn bị chặn.
- Gọi trực tiếp mọi endpoint cursor/AJAX vẫn bị kiểm quyền.
- Dự án hủy/lưu trữ đọc được nhưng không gửi theo rule hiện tại.

### SignalR

- Hai browser/profile gửi-nhận realtime.
- Không nhận tin phòng khác.
- Mất mạng/reconnect/tải bù.
- Không hiển thị trùng ở máy gửi.
- Người bị gỡ không join lại/không tiếp tục nhận theo thiết kế.
- Restart server và reconnect.
- Nginx WebSocket/HTTPS hoạt động nếu có trong deploy thực tế.
- Kiểm thử một instance và topology nhiều instance riêng.

### Hiệu năng/UI

- Ghi Controller time, SQL time/count/rows, response bytes, render time, DOM nodes.
- Desktop và mobile nhận batch bằng nhau nhưng render/scroll đúng.
- Nút tải/gửi bị disable trong request; không phát request trùng.
- Loading indicator hiển thị và lỗi mạng có thể thử lại.

## 23. Kết luận cuối cùng

| Nội dung | Hiện trạng | Có vấn đề không | Có nên sửa | Mức ưu tiên | Giải pháp đề xuất |
|---|---|---|---|---|---|
| Query danh sách phòng | Full scope + đồng bộ từng dự án + 4 query dữ liệu phòng | Có | Có | Rất cao | Bỏ N+1 hot path, projection/limit |
| Số phòng tải ban đầu | Toàn bộ | Có khi nhiều phòng | Có | Cao | 20–30 phòng, tải thêm |
| Search/filter phòng | Server-side nhưng full reload và đồng bộ toàn scope trước | Có một phần | Có | Cao | Search server + limit/cursor |
| Tin nhắn mới nhất | Một query chung nhưng tải toàn bộ tin rồi GroupBy memory | Có | Có | Rất cao | Subquery/projection một dòng/phòng |
| Query lịch sử | Join/projection tốt, `Take(200)` | Chấp nhận nhưng batch lớn | Có | Cao | 30–50 + keyset |
| Số tin tải ban đầu | Tối đa 200, không phải toàn bộ | Có thể nặng | Có | Cao | 30–50 |
| Tải tin cũ | Không có | Có | Có | Cao | Cursor thời gian + ID |
| Gửi tin nhắn | POST lưu DB đúng rule | Nghiệp vụ ổn | Giữ Service | Trung bình | Controller/AJAX rồi broadcast sau commit |
| Reload toàn trang | Tìm, chọn, gửi đều reload | Có | Có | Cao | Cập nhật vùng cần thiết |
| SignalR | Chưa triển khai, không polling | Thiếu realtime, không gây chậm page đầu | Có sau | Trung bình | Giai đoạn 3, group phòng |
| Quyền truy cập | Permission + scope + chặn Admin + membership | Tốt; nhiều query lặp | Giữ và tối ưu | Cao | Tái sử dụng context quyền trong request |
| Index CSDL | Có index FK đơn; chưa có composite lọc/order tin | Cần đo | Có thể | Sau query | Execution plan trước migration |
| Giao diện mobile | Cùng full payload; CSS một cột/scroll | Có về dữ liệu | Có | Trung bình | Dùng cùng batch giới hạn |

Trả lời trực tiếp:

1. **Nguyên nhân chính khiến danh sách phòng tải chậm:** tải toàn bộ phòng; vòng lặp bảo đảm/đồng bộ sinh nhiều query theo từng dự án; query tin mới nhất materialize toàn bộ tin của các phòng; render toàn bộ card.
2. **Nguyên nhân chính khiến lịch sử tin nhắn tải chậm:** page đầu tới 200 tin cộng các query quyền/đồng bộ và full page reload. Source không tải toàn bộ lịch sử.
3. **Có cần phân trang danh sách phòng không:** có khi scope có thể đạt hàng chục/hàng trăm phòng.
4. **Có cần phân trang lịch sử không:** có; hiện bị chặn 200 nhưng không tải được cũ hơn và page đầu còn lớn.
5. **Nên dùng gì:** phòng dùng “hoạt động gần đây + tải thêm” với cursor ổn định; tin dùng keyset/cursor `(ThoiGianGui, MaTinNhan)`.
6. **Có nên thêm SignalR không:** nên cho realtime nếu nghiệp vụ cần, nhưng không phải giải pháp tốc độ tải đầu.
7. **Khi nào làm SignalR:** giai đoạn 3, sau tối ưu query và phân trang/tải lười.
8. **Có cần sửa CSDL không:** không cho giải pháp chính; index chỉ xem xét sau runtime plan.
9. **File dự kiến chỉnh:** Controller, interface/implementation Chat service, ViewModel, View/partial, JavaScript chat; `Program.cs`/Hub chỉ ở giai đoạn SignalR; proxy config nếu deploy thật cần.
10. **Phương án ít rủi ro nhất:** giữ SQL Server và business rule hiện tại; trước hết bỏ N+1 GET, lấy latest một dòng/phòng, giới hạn 20–30 phòng và 30–50 tin; sau đó thêm endpoint cursor/AJAX; cuối cùng mới SignalR qua Controller/Service hiện có.

## 24. Trạng thái sau chỉnh sửa hiệu năng, phân trang và AJAX

Phần 1–23 ở trên là phân tích AS-IS trước đợt chỉnh sửa. Source hiện tại sau chỉnh sửa có trạng thái sau.

### 24.1 Kiến trúc và endpoint mới

| Luồng | Trạng thái sau chỉnh sửa |
|---|---|
| Mở trang | `Index` chỉ lấy batch 20 phòng và 30 tin mới nhất của phòng chọn |
| Tải thêm phòng | `ChatDuAnController.PhongBatch` trả partial `_RoomBatch`, mỗi batch 20, server chặn tối đa 50 |
| Chọn phòng | `ChatDuAnController.Phong` trả `_ChatContent`; JavaScript chỉ thay vùng `#chat-main`, không tải lại sidebar/trang |
| Tải tin cũ | `ChatDuAnController.TinNhanCu` dùng cursor `(ThoiGianGui, MaTinNhan)`, mặc định 30, tối đa 50 |
| Gửi tin | `GuiTinNhan` lưu qua Service và trả partial `_MessageItem`; JavaScript append đúng tin có `MaTinNhan` server |
| Search | Submit bằng fetch tới `PhongBatch`, reset cursor và batch phòng |
| Realtime | Chưa triển khai; không có SignalR hoặc polling |

Các endpoint mới vẫn kiểm tra `Chat.Xem`, chặn Admin và gọi Service kiểm scope/membership. `GuiTinNhan` vẫn kiểm `Chat.Gui`, scope, membership, trạng thái dự án, nội dung và độ dài trong `ChatDuAnService`.

### 24.2 Loại bỏ N+1 và query preview

- `GetPhongChatDuocThamGiaAsync` không còn vòng lặp `foreach` gọi `DamBaoPhongChatDuAnAsync` cho toàn bộ scope.
- Tạo phòng khi tạo dự án trong `DuAnService` và đồng bộ sau mutation trong `NhanVienDuAnService`/`TeamDuAnService` được giữ nguyên.
- Khi URL mở đúng một dự án cũ chưa có phòng, `GetPageAsync` chỉ bảo đảm phòng cho dự án đó.
- Query batch phòng dùng projection SQL, `Take(pageSize + 1)` và các correlated subquery có `OrderByDescending(ThoiGianGui)` + `ThenByDescending(MaTinNhan)` để lấy preview. Không còn `ToListAsync` toàn bộ `TIN_NHAN` rồi `GroupBy` trong bộ nhớ.
- Thứ tự phòng: có tin trước, thời gian tin mới nhất giảm dần, `MaPhongChat` giảm dần. Phòng chưa có tin vẫn được giữ.
- Cursor phòng gồm cờ có tin, thời gian tin mới nhất và `MaPhongChat`; `HasMore` được xác định bằng bản ghi dư.

Số query mở trang không còn tăng theo công thức khoảng `8 × số dự án` chỉ để đồng bộ. Query scope vẫn đọc danh sách mã dự án hợp lệ, nhưng không thực thi một chuỗi query riêng cho từng dự án.

### 24.3 Phân trang lịch sử

- `SoTinNhanTaiBanDau = 30`.
- `SoTinNhanToiDaMoiLanTai = 50`.
- Query lọc phòng, `IsDeleted`, user chưa xóa; sắp `ThoiGianGui DESC`, `MaTinNhan DESC`; lấy `pageSize + 1`.
- Batch được bỏ dòng dư, xác định `HasMore`, rồi đảo thành cũ → mới để render.
- Cursor tin cũ dùng đúng cặp `(ThoiGianGui, MaTinNhan)`, không dùng `Skip`.
- JavaScript lưu chiều cao/scrollTop trước khi prepend, bù chênh lệch sau khi chèn và loại trùng theo `MaTinNhan`.

### 24.4 JavaScript và giao diện

File mới `wwwroot/js/ChatDuAn/chat.js` xử lý:

- chọn phòng bằng fetch và hủy request chọn phòng cũ nếu người dùng chọn nhanh;
- tải thêm phòng, search server-side và chống request trùng;
- tải tin cũ, giữ vị trí cuộn và loại trùng;
- gửi tin bằng `FormData`, disable nút khi đang gửi, giữ nội dung nếu lỗi;
- append tin server trả về, xóa empty state, cập nhật preview và cuộn xuống cuối;
- cập nhật URL bằng `history.pushState`.

Các partial mới:

- `_RoomItem.cshtml`;
- `_RoomBatch.cshtml`;
- `_MessageBatch.cshtml`;
- `_ChatContent.cshtml`.

CSS chỉ bổ sung layout cho wrapper content/batch, nút tải và trạng thái request; cơ chế scroll độc lập hiện tại được giữ.

### 24.5 Anti-forgery

`ChatDuAnController.GuiTinNhan` có `[ValidateAntiForgeryToken]`. Form Razor POST sinh token; JavaScript gửi nguyên `FormData`, bao gồm `__RequestVerificationToken`. Không tắt anti-forgery và không mở rộng thay đổi sang POST của module khác.

### 24.6 File đã thay đổi

- `Controllers/ChatDuAnController.cs`.
- `Services/Interfaces/IChatDuAnService.cs`.
- `Services/Implementations/ChatDuAnService.cs`.
- `ViewModels/ChatDuAn/ChatDuAnPageViewModel.cs`.
- `ViewModels/ChatDuAn/ChatDuAnPhongBatchViewModel.cs`.
- `ViewModels/ChatDuAn/ChatDuAnTinNhanBatchViewModel.cs`.
- `Views/ChatDuAn/Index.cshtml`, `_RoomList.cshtml`, `_MessageList.cshtml`, `_MessageItem.cshtml`, `_MessageForm.cshtml`.
- Bốn partial mới nêu tại mục 24.4.
- `wwwroot/js/ChatDuAn/chat.js`.
- `wwwroot/css/ChatDuAn/index.css`, `room-list.css`, `message-list.css`.
- `docs/tinnhan.md`.

Không sửa Entity, DbContext mapping, migration, CSDL, module AI, FastAPI, permission definition hoặc workflow dự án.

### 24.7 Kiểm tra đã thực hiện

| Kiểm tra | Kết quả |
|---|---|
| `dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore -p:UseAppHost=false` | Thành công, 0 lỗi |
| Razor compile | Thành công trong build |
| `node --check wwwroot/js/ChatDuAn/chat.js` | Thành công |
| Quét source vòng lặp đồng bộ toàn scope | Đã loại khỏi GET danh sách |
| Quét SignalR | Không thêm `AddSignalR`, `MapHub`, Hub hoặc client SignalR |
| Runtime SQL/browser | Chưa thực hiện được: ứng dụng dừng lúc seed do kết nối SQL Server không khả dụng và tiến trình không ghi được Windows Event Log |
| Đo query/time trước–sau | Chưa có số runtime; về cấu trúc, query không còn tăng theo từng dự án và preview không materialize toàn bộ tin |

Do runtime bị chặn trước khi host khởi động, cần chạy bổ sung trên môi trường có SQL Server:

1. xác nhận EF Core dịch query batch phòng hoàn toàn sang SQL Server;
2. chạy test 0/5/20/21/50/500 phòng và 0/10/30/31/1.000 tin;
3. kiểm tra Network không có document reload khi chọn/gửi;
4. kiểm tra anti-forgery, quyền, Admin và user vừa bị gỡ;
5. đo query count, SQL time, response size và DOM trước–sau.

### 24.8 Nội dung để lại cho đợt SignalR

Chưa thực hiện SignalR, Hub, WebSocket, polling, Redis hoặc scale-out. SQL Server vẫn là nguồn sự thật. Đợt sau có thể broadcast sau `SaveChangesAsync`, nhưng không thay đổi này trong đợt tối ưu hiện tại.

### 24.9 Xác nhận CSDL

- Không sửa CSDL.
- Không tạo hoặc sửa migration.
- Không thêm index, bảng, cột, khóa hoặc trường `LastMessageTime`.
- Không sửa Entity/DbContext mapping.

## 25. Lỗi empty state của phân trang phòng và cách sửa

### 25.1 Hiện tượng đã kiểm tra lại

Sau khi chuyển danh sách phòng sang AJAX/batch, sidebar có thể hiển thị đúng nhiều phòng chat nhưng cuối danh sách lại vẫn hiện thêm dòng:

`Không có phòng phù hợp.`

Trạng thái này sai khi danh sách thực tế vẫn còn các card phòng trên DOM.

### 25.2 Nguyên nhân chính xác

Nguyên nhân nằm ở cách suy diễn trạng thái rỗng trong JavaScript, không nằm ở query EF Core hay `HasMore`.

- File `QuanLyDuAn/QuanLyDuAn/wwwroot/js/ChatDuAn/chat.js`.
- Trong `loadRoomBatch(reset)`, code cũ parse partial `_RoomBatch`, rồi gọi:

```javascript
setStatus(status, batch.querySelector(".chat-room-item") ? "" : "Không có phòng phù hợp.", false);
```

- Điều kiện trên chỉ nhìn vào **batch vừa trả về**, không nhìn vào **tổng số phòng đang hiển thị trên DOM**.
- Vì vậy, khi người dùng bấm `Tải thêm phòng` và request cuối trả về batch rỗng:
  - danh sách cũ vẫn còn nhiều `.chat-room-item`;
  - nhưng batch mới không có `.chat-room-item`;
  - code cũ kết luận sai là “không có phòng phù hợp”.

Nói ngắn gọn: code cũ đã nhầm giữa `batch rỗng` và `toàn bộ danh sách rỗng`.

### 25.3 Điều kiện cũ bị sai ở đâu

- `_RoomBatch.cshtml` chỉ đại diện cho **một batch**.
- `HasMore == false` chỉ có nghĩa là **không còn batch tiếp theo**.
- Batch cuối rỗng hoặc batch cuối không còn dữ liệu không đồng nghĩa toàn bộ sidebar rỗng.
- Do đó không được dùng:
  - `batch.querySelector(".chat-room-item") == null`;
  - hoặc `HasMore == false`;
  - hoặc partial batch rỗng

để quyết định hiển thị empty state toàn danh sách.

### 25.4 Cách sửa đã áp dụng

Các file đã chỉnh:

- `QuanLyDuAn/QuanLyDuAn/ViewModels/ChatDuAn/ChatDuAnPhongBatchViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_RoomBatch.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_RoomList.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/Index.cshtml`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/ChatDuAn/chat.js`

Chi tiết sửa:

- `_RoomBatch.cshtml` tiếp tục chỉ render item của batch, không tự render empty state.
- `ChatDuAnPhongBatchViewModel` bổ sung `SoLuongPhong => DanhSachPhong.Count`; `_RoomBatch.cshtml` xuất `data-room-count`.
- `_RoomList.cshtml` tạo riêng một phần tử `#chat-room-empty` cho empty state toàn danh sách và không dùng `#chat-room-status` để hiển thị trạng thái rỗng.
- `Index.cshtml` truyền chuỗi chuẩn `Không có phòng phù hợp.` qua `data-room-empty-text`.
- `chat.js` tách rõ helper:
  - đếm tổng số room hiện có trên DOM;
  - xóa/thêm empty state riêng;
  - cập nhật nút `Tải thêm phòng` chỉ theo `HasMore`;
  - không dùng batch rỗng để kết luận danh sách rỗng.

### 25.5 Điều kiện mới để hiển thị empty state

Sau khi sửa, sidebar chỉ hiện `Không có phòng phù hợp.` khi đồng thời thỏa cả hai điều kiện:

1. request đầu hoặc request search/reset đã hoàn thành;
2. tổng số phần tử `[data-room-id]` đang có trên `#chat-room-items` bằng `0`.

Nếu DOM đang có ít nhất một room item thì empty state bị xóa và không được render cùng lúc.

### 25.6 Cách xử lý batch cuối

Luồng `Tải thêm phòng` hiện xử lý như sau:

- giữ nguyên các room hiện có;
- nếu batch mới sau khi parse/loại trùng không còn item nào:
  - không append empty state;
  - không thay danh sách cũ;
  - chỉ cập nhật `HasMore`;
  - ẩn nút `Tải thêm phòng` nếu `HasMore == false`.

Điều này đúng với yêu cầu: batch cuối rỗng không được biến thành trạng thái “không có phòng”.

### 25.7 Kết quả suy ra từ source sau sửa

Các trường hợp dưới đây mô tả hành vi **kỳ vọng theo code**, không phải kết quả kiểm thử runtime:

- Trường hợp 20 phòng:
  - batch đầu hiển thị 20 phòng;
  - `HasMore = false` thì nút `Tải thêm phòng` bị ẩn;
  - không còn hiện `Không có phòng phù hợp.` nếu DOM vẫn có room.
- Trường hợp 21 phòng:
  - batch đầu hiển thị 20 phòng;
  - batch sau hiển thị thêm 1 phòng;
  - sau batch cuối không xuất hiện empty state.
- Trường hợp 40 phòng:
  - tải tiếp nhiều batch vẫn giữ các room đã có;
  - batch cuối chỉ làm `HasMore = false`;
  - không render thông báo rỗng sai.
- Search có kết quả:
  - xóa batch cũ;
  - render batch mới;
  - nếu có ít nhất một room thì xóa empty state.
- Search không có kết quả:
  - xóa danh sách cũ;
  - không chèn room item;
  - hiển thị đúng một empty state `Không có phòng phù hợp.`
- Request tải thêm lỗi:
  - giữ nguyên danh sách cũ;
  - không thay bằng empty state;
  - chỉ hiển thị thông báo lỗi trên `#chat-room-status`.

### 25.8 Giới hạn xác minh hiện tại

- `node --check` và `dotnet build` có thể xác nhận cú pháp và Razor compile.
- Kiểm thử trình duyệt thực tế vẫn phụ thuộc môi trường chạy được ứng dụng và SQL Server.
- Vì vậy các case 20/21/40 phòng hiện được xác nhận theo logic source sau sửa và qua build tĩnh; nếu host/runtime khả dụng cần chạy thêm test tay trên browser để chốt hành vi UI cuối cùng.

## 26. Lỗi không hiển thị nút Tải thêm phòng

### 26.1 Hiện tượng

Sau khi sửa lỗi empty state ở mục 25, sidebar chat vẫn còn một lỗi mới:

- batch đầu chỉ hiển thị một phần danh sách phòng;
- trong scope người dùng vẫn còn phòng khác;
- nhưng không xuất hiện nút `Tải thêm phòng`;
- vì vậy người dùng không thể tải các batch tiếp theo.

### 26.2 Giả thuyết và lần sửa trước

Lần sửa trước tập trung vào lớp hiển thị View/JavaScript của nút tải thêm. Sau khi đối chiếu lại toàn bộ chuỗi source và việc lỗi vẫn tái hiện trên web, nội dung này **không còn được xem là kết luận nguyên nhân chính xác**. Đây chỉ là giả thuyết/lần gia cố trạng thái hiển thị ban đầu, vì chưa có bằng chứng chứng minh thuộc tính `hidden` là nguyên nhân làm request tải thêm không hoạt động.

Điểm lỗi chính:

- File `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_RoomList.cshtml`.
- Nút tải thêm đang render:

```cshtml
hidden="@(!Model.PhongBatch.HasMore)"
```

- `HasMore` của server vẫn được đưa ra `ViewModel` và `_RoomBatch.cshtml` vẫn xuất `data-has-more` cho batch.
- Tuy nhiên trạng thái hiển thị ban đầu của nút lại phụ thuộc trực tiếp vào thuộc tính HTML `hidden` render từ Razor.
- Để tránh phụ thuộc vào cách trình duyệt xử lý thuộc tính boolean render sẵn và để thống nhất với dữ liệu batch, cần chỉ render `hidden` khi thực sự hết dữ liệu, đồng thời đồng bộ lại nút từ `data-has-more` của batch đầu ngay khi JavaScript khởi tạo.

Những điều source cho phép xác nhận:

- Service/cursor hiện tại vẫn tính `HasMore` theo cơ chế `Take(pageSize + 1)`.
- Lần sửa render state giúp nút đồng bộ theo metadata, nhưng không chứng minh được nguyên nhân khiến batch tiếp theo không được thêm.

### 26.3 File và method liên quan

- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChatDuAnService.cs`
  - `GetPageAsync`
  - `GetPhongChatBatchAsync`
  - `GetPhongChatBatchNoAuthAsync`
- `QuanLyDuAn/QuanLyDuAn/Controllers/ChatDuAnController.cs`
  - `Index`
  - `PhongBatch`
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_RoomList.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_RoomBatch.cshtml`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/ChatDuAn/chat.js`

### 26.4 HasMore và cursor trước sửa

Theo source trước sửa:

- Query phòng dùng:
  - lọc scope;
  - lọc từ khóa;
  - thứ tự `co tin` → `thời gian tin mới nhất giảm dần` → `MaPhongChat giảm dần`;
  - `Take(pageSize + 1)`;
  - `hasMore = rows.Count > pageSize`;
  - bỏ đúng bản ghi dư;
  - cursor lấy từ item cuối thực trả về.
- `_RoomBatch.cshtml` đã xuất:
  - `data-has-more`
  - `data-cursor-has-message`
  - `data-cursor-time`
  - `data-cursor-room-id`

Vì vậy ở mức source:

- `HasMore` không bị mất trong Controller.
- Cursor không bị mất trong partial batch.
- JavaScript tải thêm vẫn đọc cursor từ batch cuối.

### 26.5 Cách sửa đã áp dụng

#### View

Trong `_RoomList.cshtml`:

- bỏ kiểu render `hidden="@(...)"`;
- thay bằng chỉ render thuộc tính `hidden` khi `HasMore == false`.

Tức là nút chỉ nhận boolean attribute `hidden` khi server thật sự kết luận hết batch.

#### JavaScript

Trong `wwwroot/js/ChatDuAn/chat.js`:

- bổ sung `syncRoomLoadButtonFromBatch()`;
- khi trang khởi tạo, JavaScript đọc batch cuối hiện có trên DOM;
- lấy `data-has-more`;
- gọi `updateRoomLoadButton(batch.dataset.hasMore === "true")`.

Việc này làm trạng thái ban đầu của nút bám đúng metadata server thay vì phụ thuộc hoàn toàn vào HTML render tĩnh.

### 26.6 Query/cursor sau sửa

Không sửa query phân trang phòng.

Không sửa thứ tự.

Không sửa điều kiện cursor.

Không sửa `HasMore = rows.Count > pageSize`.

Không sửa `Take(pageSize + 1)`.

Không sửa scope permission.

Đợt này chỉ sửa lớp hiển thị của nút tải thêm để UI phản ánh đúng `HasMore` vốn đã có trong batch.

### 26.7 Kết quả test theo dữ liệu

Hiện tại chưa thể xác nhận các mốc dữ liệu `5/20/21/40/41 phòng` bằng runtime thật vì môi trường tại phiên làm việc này chưa truy cập được SQL Server/app host. Do đó:

- chưa khẳng định số phòng thực tế theo scope trong môi trường kiểm thử;
- chưa khẳng định kết quả browser của case `21/40/41 phòng`;
- chưa khẳng định Network response runtime của `PhongBatch`.

Những gì đã xác nhận được từ source sau sửa:

- nếu batch đầu có `data-has-more=\"true\"` thì JavaScript sẽ bật nút ngay khi khởi tạo;
- nếu batch đầu có `data-has-more=\"false\"` thì nút vẫn ẩn;
- khi tải thêm thành công, nút tiếp tục chỉ phụ thuộc `HasMore` của server;
- khi request lỗi, JavaScript không ép `HasMore = false` và vẫn enable lại nút.

### 26.8 Kết quả search

Theo source sau sửa:

- search/reset vẫn gọi `PhongBatch` với `cursor = null`;
- batch mới trả `data-has-more`;
- `loadRoomBatch(true)` vẫn dùng `updateRoomLoadButton(hasMore)` từ server;
- vì vậy search có hơn 20 kết quả sẽ có thể hiện nút tải thêm đúng theo `HasMore`.

### 26.9 Kết quả quyền và scope

Không thay đổi:

- `Chat.Xem`;
- chặn Admin;
- scope dự án;
- membership phòng chat;
- điều kiện soft delete.

Nếu trong môi trường thật có “phòng còn lại” nhưng không thuộc scope thì đó không phải lỗi phân trang; tuy nhiên ở phiên này chưa thể đo tổng số phòng theo scope bằng DB runtime vì kết nối SQL hiện không khả dụng.

### 26.10 Kết quả browser/Network

Chưa thể xác nhận bằng browser/Network thật trong phiên này vì:

- không khởi chạy được ứng dụng đầy đủ để mở trang chat;
- không truy cập được SQL Server để dựng dữ liệu thật;
- browser runtime vì thế chưa có đích localhost khả dụng để kiểm tra DOM/Network của trang chat.

### 26.11 Phần chưa thể kiểm thử

- Tổng số phòng theo scope người dùng đang test.
- Batch đầu thực tế trả bao nhiêu phòng.
- `HasMore` runtime trước/sau sửa cho case `21/40/41`.
- Cursor runtime trước/sau sửa trên dữ liệu có tin và chưa có tin.
- Trạng thái DOM/Computed style của nút trên browser thật.

### 26.12 Trạng thái sau khi thay cursor

Mục 26 được giữ lại như lịch sử của lần sửa trước. Kể từ thay đổi tại mục 28, cursor ba thành phần và metadata tương ứng đã bị loại bỏ; nội dung kỹ thuật về cursor tại mục này không còn là cơ chế hiện hành.

## 27. Phân tích tĩnh lỗi không tải thêm được phòng (lịch sử trước `lastRoomId`)

### 27.1 Phạm vi và kết luận

Mục này ghi lại kết quả phân tích source **trước khi** chuyển sang `lastRoomId`. Kết luận dưới đây giải thích vì sao lần rà soát đó không tìm được lỗi tĩnh chắc chắn trong cursor ba thành phần; cơ chế này đã được thay thế tại mục 28 và không còn là hiện trạng source.

- cơ chế phân trang phòng hiện tại là keyset/cursor phía server kết hợp AJAX partial HTML;
- chuỗi tên trường từ ViewModel, Razor, JavaScript, query string, Controller đến Service đang thống nhất;
- điều kiện cursor của nhóm có tin, chuyển sang nhóm chưa có tin và tiếp tục trong nhóm chưa có tin tương thích với thứ tự sắp xếp;
- logic `pageSize + 1`, `HasMore`, bỏ dòng dư và lấy cursor từ item cuối thực trả về đúng thứ tự;
- JavaScript gắn sự kiện bằng delegation, parse partial, loại trùng bằng `MaPhongChat` và chèn batch vào DOM trước vùng nút tải thêm;
- không có lỗi trực tiếp nào trong source hiện tại đủ để chứng minh là nguyên nhân khiến thao tác tải thêm thất bại.

Vì vậy đợt này **không sửa thêm Controller, Service, ViewModel, View, JavaScript hoặc CSS**. Sửa cursor khi điều kiện hiện tại đã đúng sẽ là sửa theo suy đoán và có nguy cơ tạo lỗi thiếu/trùng phòng.

### 27.2 Luồng dữ liệu hiện tại

1. `ChatDuAnController.Index(int? maDuAn, string? tuKhoa)` gọi `ChatDuAnService.GetPageAsync`.
2. `GetPageAsync` gọi `GetPhongChatBatchNoAuthAsync(currentUserId, tuKhoa, null, null, null, 20)`. Ba giá trị cursor `null` thể hiện batch đầu chưa có cursor.
3. Service trả `ChatDuAnPhongBatchViewModel` gồm `DanhSachPhong`, `HasMore`, `CursorCoTinNhan`, `CursorThoiGianTinNhan`, `CursorMaPhongChat`, `TuKhoa`.
4. `Index.cshtml` render `_RoomList`; `_RoomList.cshtml` render `_RoomBatch` đầu và một nút duy nhất `#chat-load-more-rooms`.
5. `_RoomBatch.cshtml` xuất:
   - `data-room-count`;
   - `data-has-more`;
   - `data-cursor-has-message`;
   - `data-cursor-time`;
   - `data-cursor-room-id`.
6. Khi người dùng bấm nút, event delegation trong `chat.js` gọi `loadRoomBatch(false)`.
7. `getLastRoomBatch()` lấy batch cuối trong `#chat-room-items`; JavaScript đọc cursor từ `dataset.cursorHasMessage`, `dataset.cursorTime`, `dataset.cursorRoomId`.
8. `buildUrl` gửi `tuKhoa`, ba thành phần cursor và `pageSize=20` đến URL `PhongBatch` lấy từ `data-room-batch-url`.
9. `ChatDuAnController.PhongBatch` bind lần lượt thành `string?`, `bool?`, `DateTime?`, `int?`, `int`, rồi truyền nguyên giá trị sang `GetPhongChatBatchAsync`.
10. Service áp scope, từ khóa và cursor, lấy batch kế tiếp, Controller trả partial `_RoomBatch`.
11. JavaScript parse `.chat-room-batch`, loại trùng, rồi `insertBefore(batch, loadArea)` để chèn batch vào `#chat-room-items`.
12. Nút được ẩn khi và chỉ khi metadata batch trả về có `data-has-more != "true"`.

### 27.3 Page size, thứ tự, HasMore và cursor

- Batch đầu: `SoPhongTaiBanDau = 20`.
- Mặc định action/interface tải thêm: `pageSize = 20`.
- Giới hạn Service: `Math.Clamp(pageSize, 1, SoPhongToiDaMoiLanTai)` với tối đa `50`.
- Thứ tự:
  1. `MaTinNhanMoiNhat.HasValue DESC`;
  2. `ThoiGianTinNhanMoiNhat ?? DateTime.MinValue DESC`;
  3. `MaPhongChat DESC`.
- Service gọi `Take(pageSize + 1)` trước `ToListAsync`.
- `HasMore = rows.Count > pageSize` được tính trước khi bỏ dòng dư.
- Nếu có dư, Service xóa đúng phần tử cuối của `rows`.
- Cursor lấy từ item cuối cùng trong danh sách thực trả về, không lấy từ dòng dư.
- Cursor gồm:
  - `CursorCoTinNhan: bool?`;
  - `CursorThoiGianTinNhan: DateTime?`;
  - `CursorMaPhongChat: int?`.
- Việc cursor có tồn tại được xác định bằng `cursorCoTinNhan.HasValue && cursorMaPhongChat.HasValue`; do đó `false` khác `null` và không bị hiểu nhầm thành “không có cursor”.

### 27.4 Bảng chân trị điều kiện cursor

| Trạng thái cursor | Điều kiện Service hiện tại | Đánh giá |
|---|---|---|
| Không có cursor | Một hoặc cả hai giá trị `cursorCoTinNhan`, `cursorMaPhongChat` không có | Không áp `Where` cursor; đúng cho batch đầu |
| Cursor có tin | Lấy phòng không có tin **hoặc** thời gian nhỏ hơn cursor **hoặc** cùng thời gian và `MaPhongChat` nhỏ hơn | Tiếp tục nhóm có tin và chuyển được sang toàn bộ nhóm chưa có tin |
| Cursor chưa có tin | `!MaTinNhanMoiNhat.HasValue && MaPhongChat < cursorRoomId` | Tiếp tục đúng chiều giảm dần trong nhóm chưa có tin; không yêu cầu thời gian |

Trường hợp 170 phòng mà phần lớn chưa có tin không bị loại theo điều kiện source: nếu item cuối batch đầu chưa có tin, batch sau lọc các phòng chưa có tin có `MaPhongChat` nhỏ hơn cursor.

`CursorThoiGianTinNhan` hiện được gán `DateTime.MinValue` khi item cuối không có tin. Giá trị này được render/gửi, nhưng nhánh cursor `false` của Service không sử dụng thời gian. Đây là dữ liệu thừa, chưa phải lỗi tải thêm được chứng minh.

### 27.5 Query EF Core

`GetPhongChatBatchNoAuthAsync` giữ toàn bộ lọc và giới hạn trên `IQueryable` cho tới `ToListAsync`:

- `AsNoTracking` cho `PhongChat` và `DuAn`;
- join phòng với dự án;
- lọc soft delete và `dsMaDuAn.Contains`;
- projection preview/count bằng correlated subquery;
- lọc từ khóa trên SQL;
- áp cursor trước sắp xếp;
- sắp xếp và `Take(pageSize + 1)` trước materialize.

Không có `AsEnumerable()` hoặc `ToListAsync()` trước cursor/`Take`; không tải toàn bộ khoảng 170 phòng để phân trang trong bộ nhớ; không có vòng lặp query riêng cho từng item sau materialize. Các subquery preview/count có thể cần đo hiệu năng, nhưng không chứng minh được lỗi “không append batch”.

### 27.6 ViewModel, Razor và model binding

Tên trường đang khớp:

| ViewModel | Razor `data-*` | JavaScript/query string | Controller |
|---|---|---|---|
| `CursorCoTinNhan` | `data-cursor-has-message` | `cursorCoTinNhan` | `bool? cursorCoTinNhan` |
| `CursorThoiGianTinNhan` | `data-cursor-time` | `cursorThoiGianTinNhan` | `DateTime? cursorThoiGianTinNhan` |
| `CursorMaPhongChat` | `data-cursor-room-id` | `cursorMaPhongChat` | `int? cursorMaPhongChat` |
| `HasMore` | `data-has-more` | đọc bằng `=== "true"` | không gửi ngược lên |

Razor render boolean chữ thường. Thời gian dùng định dạng round-trip `"O"` và `URLSearchParams` mã hóa query string. `buildUrl` bỏ `null`, `undefined` và chuỗi rỗng nhưng không bỏ chuỗi `"false"`, nên cursor thuộc nhóm chưa có tin vẫn được gửi.

### 27.7 JavaScript và partial

- Event listener không gắn trực tiếp vào nút mà dùng `document.addEventListener("click", ...)`; việc thay/append batch không làm mất listener.
- `getLastRoomBatch()` lấy đúng batch cuối bằng danh sách `#chat-room-items .chat-room-batch`.
- `_RoomBatch.cshtml` chỉ render wrapper, metadata và `_RoomItem`; không render nút hoặc empty state.
- `_RoomItem.cshtml` đặt `data-room-id="@Model.MaPhongChat"`, không dùng `MaDuAn`.
- `removeDuplicateRooms` dùng tập `data-room-id` hiện có và loại trùng theo `MaPhongChat`.
- Batch mới được chèn vào DOM thật bằng `roomItems.insertBefore(batch, loadArea)`.
- `updateRoomLoadButton` chỉ dùng `HasMore` từ server; lỗi request không tự đặt `HasMore=false`.
- Search gọi `loadRoomBatch(true)`, không gửi cursor, xóa các batch cũ sau khi nhận response thành công và giữ cùng từ khóa hiện tại cho các lần tải thêm sau.

Không thấy selector, ID hoặc tên metadata lệch nhau trong source hiện tại.

### 27.8 Nội dung kết luận trước đây chưa đủ bằng chứng

- Mục 25 mô tả đúng lỗi empty state trong code cũ, nhưng các case `20/21/40` chỉ là hành vi suy ra từ source; không được gọi là kết quả runtime.
- Mục 26 từng gọi render thuộc tính `hidden` là “nguyên nhân chính xác”. Sau khi rà lại, kết luận đó quá mạnh: thay đổi chỉ đồng bộ trạng thái nút theo `data-has-more`, không chứng minh được nguyên nhân khiến request/batch tiếp theo không hoạt động. Mục 26 đã được đánh dấu lại là giả thuyết/lần sửa trước.
- Source hiện tại đã có `asp-append-version="true"` cho `chat.js`; cache busting được cấu hình ở View. Việc bản deploy có dùng đúng asset mới hay không vẫn chỉ xác nhận được ở runtime.

### 27.9 Phần chỉ có thể xác nhận bằng runtime

Phân tích tĩnh không cho biết:

- browser có thực sự tải đúng phiên bản `chat.js` hay không;
- click có phát request `PhongBatch` hay bị JavaScript/runtime khác chặn;
- query string thực tế và HTTP status;
- HTML/metadata thực tế của batch đầu và batch sau;
- exception EF/model binding nếu có ở runtime;
- batch tiếp theo có item nhưng bị code/runtime khác ngoài source hiện tại can thiệp hay không;
- DLL/View/static asset của bản deploy có đồng bộ với workspace hiện tại hay không.

Dữ liệu đã biết theo yêu cầu hiện tại là 520 phòng chưa xóa toàn CSDL và khoảng 170 phòng trong scope của Manager `hophuocloi108`. Hai con số này không tự chứng minh lỗi nằm ở cursor, vì vẫn cần đối chiếu tập khớp từ khóa, metadata và response của request cụ thể.

### 27.10 File thay đổi và giới hạn

Đợt phân tích tĩnh này chỉ cập nhật `docs/tinnhan.md`; không có đủ bằng chứng source để sửa code phân trang.

- Không sửa CSDL hoặc migration.
- Không sửa Entity, permission, role, scope hoặc quy tắc chặn Admin.
- Không tải toàn bộ phòng và không thay page size.
- Không triển khai SignalR hoặc polling.

### 27.11 Trạng thái sau thay đổi

Các mô tả tại mục 27 về `CursorCoTinNhan`, `CursorThoiGianTinNhan`, `CursorMaPhongChat`, thứ tự theo tin mới nhất và metadata cursor ba thành phần chỉ còn giá trị lịch sử. Cơ chế hiện hành được mô tả tại mục 28.

## 28. Thay cursor phòng ba thành phần bằng `lastRoomId`

### 28.1 Lý do thay đổi

Cursor phòng cũ kết hợp ba giá trị:

- phòng có tin nhắn hay chưa;
- thời gian tin nhắn mới nhất;
- `MaPhongChat`.

Ba giá trị phải đi qua Service → ViewModel → Razor data attribute → JavaScript → query string → Controller → Service. Dù phân tích tĩnh ở mục 27 không chỉ ra điều kiện sai chắc chắn, cơ chế này phức tạp, phụ thuộc trường nullable và tạo nhiều điểm truyền dữ liệu không cần thiết. Với phần lớn phòng chưa có tin nhắn, lợi ích sắp xếp theo hoạt động không bù được rủi ro vận hành.

Thay đổi mới ưu tiên tính đúng và ổn định: cursor phòng chỉ còn `lastRoomId`, dựa trên khóa chính `MaPhongChat`.

### 28.2 Cơ chế `lastRoomId` mới

- Batch đầu gọi Service với `lastRoomId = null`.
- Danh sách được sắp xếp `MaPhongChat DESC`.
- Batch tiếp theo lọc `MaPhongChat < lastRoomId`.
- `lastRoomId` của request kế tiếp là `MaPhongChat` của item cuối cùng thực sự trả về trong batch trước.
- Phòng có tin và chưa có tin dùng chung một thứ tự; không còn nhóm nullable, `DateTime.MinValue` hoặc cursor thời gian.
- Mỗi batch mặc định 20 phòng; Service kẹp `pageSize` trong khoảng 1–50.

### 28.3 Query batch

`ChatDuAnService.GetPhongChatBatchNoAuthAsync` thực hiện:

1. Lấy danh sách mã dự án theo scope người dùng.
2. Join `PHONG_CHAT` với `DU_AN`.
3. Lọc phòng chưa xóa, dự án chưa xóa và dự án thuộc scope.
4. Lọc từ khóa theo tên phòng hoặc tên dự án nếu có.
5. Nếu `lastRoomId.HasValue`, thêm điều kiện `MaPhongChat < lastRoomId.Value`.
6. `OrderByDescending(MaPhongChat)`.
7. `Take(pageSize + 1)`.
8. `ToListAsync`.
9. Tính `HasMore = rows.Count > pageSize`.
10. Nếu có dòng dư, bỏ đúng dòng cuối.
11. Tạo `NextRoomId` từ item cuối cùng trong danh sách thực trả về.

Không dùng `Skip`, không `AsEnumerable`, không tải toàn bộ phòng để phân trang trong bộ nhớ.

### 28.4 Preview tin nhắn vẫn được giữ

Việc bỏ tin nhắn mới nhất khỏi thứ tự/cursor không bỏ dữ liệu hiển thị. Query vẫn projection:

- nội dung tin nhắn mới nhất;
- thời gian tin nhắn mới nhất;
- số thành viên;
- số tin nhắn.

Nội dung và thời gian mới nhất tiếp tục lấy bằng correlated subquery trên SQL Server, sắp theo `ThoiGianGui DESC`, `MaTinNhan DESC`. Không materialize toàn bộ `TIN_NHAN` và không query riêng trong vòng lặp từng phòng.

Subquery `MaTinNhanMoiNhat` cũ chỉ phục vụ phân nhóm/sắp xếp cursor phòng nên đã được loại khỏi projection.

### 28.5 Luồng Service đến JavaScript

| Lớp | Cơ chế mới |
|---|---|
| `IChatDuAnService` | `GetPhongChatBatchAsync(string? tuKhoa, int? lastRoomId, int pageSize = 20)` |
| `ChatDuAnController.PhongBatch` | Nhận `tuKhoa`, `lastRoomId`, `pageSize`; vẫn kiểm tra `Chat.Xem` |
| `ChatDuAnPhongBatchViewModel` | Giữ `DanhSachPhong`, `SoLuongPhong`, `HasMore`, `NextRoomId`, `TuKhoa` |
| `_RoomBatch.cshtml` | Render `data-room-count`, `data-has-more`, `data-next-room-id` |
| `chat.js` | Batch đầu/search không gửi cursor; tải thêm đọc `dataset.nextRoomId` của batch cuối và gửi `lastRoomId` |

Boolean `HasMore` tiếp tục render chữ thường và JavaScript đọc bằng `value === "true"`.

### 28.6 Nút tải thêm, search và loại trùng

- `_RoomList.cshtml` vẫn chỉ có một nút `#chat-load-more-rooms`, nằm ngoài `_RoomBatch`.
- Nút disable trong khi request chạy và được enable lại trong `finally`.
- Nút chỉ ẩn theo `HasMore` của response thành công; lỗi request không ép `HasMore=false`.
- Search gọi `loadRoomBatch(true)`, nên `cursorBatch = null` và không gửi `lastRoomId`.
- Sau response search thành công, các batch cũ được xóa và batch đầu của tập kết quả mới được chèn.
- Những lần tải thêm sau search đọc `NextRoomId` từ batch cuối mới và giữ từ khóa hiện tại.
- `_RoomItem.cshtml` tiếp tục render `data-room-id="@Model.MaPhongChat"`.
- `removeDuplicateRooms` tiếp tục loại trùng theo `MaPhongChat`, không dùng `MaDuAn`, tên phòng hoặc tên dự án.

### 28.7 Code cursor phòng cũ đã loại bỏ

Đã loại khỏi source thực thi:

- `CursorCoTinNhan`;
- `CursorThoiGianTinNhan`;
- `CursorMaPhongChat`;
- `cursorCoTinNhan`;
- `cursorThoiGianTinNhan`;
- `cursorMaPhongChat`;
- `data-cursor-has-message`;
- `data-cursor-room-id`;
- `data-cursor-time` của `_RoomBatch`.

`data-cursor-time` trong `_MessageBatch.cshtml` vẫn được giữ vì thuộc phân trang lịch sử tin nhắn, không phải cursor phòng.

### 28.8 File đã sửa

- `QuanLyDuAn/QuanLyDuAn/Controllers/ChatDuAnController.cs`.
- `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IChatDuAnService.cs`.
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/ChatDuAnService.cs`.
- `QuanLyDuAn/QuanLyDuAn/ViewModels/ChatDuAn/ChatDuAnPhongBatchViewModel.cs`.
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_RoomBatch.cshtml`.
- `QuanLyDuAn/QuanLyDuAn/wwwroot/js/ChatDuAn/chat.js`.
- `docs/tinnhan.md`.

Không cần sửa `_RoomList.cshtml`, `_RoomItem.cshtml`, `Index.cshtml` hoặc CSS vì cấu trúc nút, khóa `data-room-id`, cache busting và vùng cuộn hiện tại đã phù hợp.

### 28.9 Phần giữ nguyên

- Phân trang lịch sử tin nhắn vẫn dùng cursor `(ThoiGianGui, MaTinNhan)`, mặc định 30 và tối đa 50.
- Permission `Chat.Xem`, `Chat.Gui`, scope dự án, membership và quy tắc chặn Admin không đổi.
- Phòng/dự án xóa mềm vẫn bị loại.
- Dự án đã hủy/lưu trữ tiếp tục theo quy tắc đọc/gửi hiện hành.
- Không triển khai SignalR, WebSocket hoặc polling.
- Không sửa CSDL, migration, Entity, index, bảng hoặc cột.

### 28.10 Kiểm tra và giới hạn xác minh

Build solution và `node --check` được dùng để xác nhận compile/Razor/interface và cú pháp JavaScript. Toàn repository được quét để bảo đảm cursor phòng cũ không còn trong source thực thi; các từ khóa còn trong tài liệu chỉ là lịch sử phân tích.

- `dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore -p:UseAppHost=false`: thành công, 0 lỗi; còn 2 cảnh báo `CS1998` có sẵn tại `FileTienDoCongViecService.cs`, không liên quan Chat.
- `node --check QuanLyDuAn/QuanLyDuAn/wwwroot/js/ChatDuAn/chat.js`: thành công.
- Kiểm tra UTF-8 trên toàn bộ file đã sửa: hợp lệ, không BOM và không phát hiện chuỗi mojibake.

Đợt này không chạy browser/SQL runtime theo phạm vi yêu cầu. Vì vậy chưa tuyên bố hành vi tải đủ khoảng 170 phòng đã được xác nhận trên bản deploy; kết quả runtime vẫn cần kiểm tra riêng sau khi publish.

## 29. Triển khai realtime bằng SignalR

### 29.1 Kiến trúc và ranh giới nghiệp vụ

Luồng gửi vẫn là `fetch POST` → `ChatDuAnController.GuiTinNhan` → `ChatDuAnService.GuiTinNhanAsync` → insert `TIN_NHAN` → `SaveChangesAsync` thành công → `IChatRealtimePublisher` → SignalR. Hub không có method gửi/tạo tin, không dùng `DbContext` và không nhận phòng, nội dung hoặc recipient từ client. SQL Server tiếp tục là nguồn dữ liệu chính; lỗi phát realtime không rollback tin đã lưu và không làm HTTP gửi tin thất bại.

`Program.cs` đăng ký `AddSignalR()` và map Hub `[Authorize]` tại `/hubs/chat-du-an` sau authentication/authorization. `ChatDuAnHub.OnConnectedAsync` từ chối Admin và principal không có `Chat.Xem`; Hub chỉ quản lý kết nối, không thực hiện nghiệp vụ chat.

### 29.2 UserIdentifier và recipient

Cookie principal do `AccountService` tạo có `ClaimTypes.NameIdentifier = AspNetUsers.Id`. Đây là khóa tài khoản Identity ổn định và đúng giá trị SignalR mặc định dùng làm `UserIdentifier`, vì vậy không tạo `IUserIdProvider` và không query CSDL cho mỗi kết nối.

Sau khi lưu từng tin, Service tính lại recipient phía server từ:

- membership hiện hành trong `THANH_VIEN_PHONG_CHAT`;
- scope người quản lý dự án hoặc dòng `NHAN_VIEN_DU_AN` hiện hành;
- dự án/người dùng chưa xóa;
- mapping `AspNetUsers.Id` hợp lệ và tài khoản không đang bị khóa;
- quyền `Chat.Xem` từ user claim hoặc role claim;
- loại tài khoản thuộc role Admin.

Danh sách `AspNetUsers.Id` này được truyền vào `Clients.Users(...)`. Không dùng `Clients.All`, không lấy recipient từ client và không join group cho toàn bộ khoảng 170 phòng khi connect/reconnect. Cách tính lại recipient trên mỗi tin bảo đảm người vừa bị gỡ khỏi dự án không nhận các tin phát sau đó, đồng thời tránh N+1 `Groups.AddToGroupAsync`.

### 29.3 Publisher, event và payload

`IChatRealtimePublisher`/`ChatRealtimePublisher` dùng `IHubContext<ChatDuAnHub>` và event duy nhất `MessageCreated`. Publisher không ghi CSDL. Nó log message ID, room ID và số recipient nhưng không log nội dung hoặc danh sách ID; exception SignalR được log và xử lý tại publisher để HTTP vẫn trả thành công.

DTO `ChatRealtimeMessageDto` chỉ gồm `MaTinNhan`, `MaPhongChat`, `MaDuAn`, `MaNguoiDung`, `TenNguoiGui`, `AvatarUrl`, `NoiDungTinNhan`, `ThoiGianGui`, `TenDuAn`. Không trả Entity, password hash, security stamp, claim hoặc permission. JSON datetime dùng định dạng chuẩn của ASP.NET Core.

### 29.4 SignalR client, append và chống trùng

Client `@microsoft/signalr` 8.0.17 được lưu local tại `wwwroot/lib/microsoft/signalr/signalr.min.js`, tải trước `chat.js` và có `asp-append-version="true"`. Razor truyền `data-chat-hub-url`, `data-new-messages-url` và mã người dùng nghiệp vụ hiện tại; JavaScript không hard-code base path.

Trang chỉ tạo một `HubConnection` với `withAutomaticReconnect()`. `MessageCreated` được kiểm tra payload và chống trùng bằng `MaTinNhan`/`data-message-id`. HTTP response và SignalR có thể đến theo bất kỳ thứ tự nào; đường đến sau bỏ qua. DOM realtime được tạo bằng `document.createElement`, `textContent` và URL avatar chỉ chấp nhận HTTP/HTTPS, không chèn nội dung người dùng vào `innerHTML`.

Nếu event thuộc phòng đang mở, client append đúng kiểu tin của mình/người khác. Chỉ tự cuộn khi người dùng đang gần cuối; nếu đang đọc tin cũ, vị trí được giữ và hiện nút `Có tin nhắn mới`. Nếu event thuộc phòng khác, không append vào phòng hiện tại.

Preview chỉ được cập nhật khi card phòng đã có trong sidebar: nội dung, thời gian và tổng số tin. Card không được di chuyển, không tạo card cho phòng chưa tải, không đổi `lastRoomId` và không tác động nút tải thêm. Hành vi cũ đưa card vừa gửi lên đầu đã được loại bỏ để DOM tiếp tục khớp `MaPhongChat DESC`.

### 29.5 Reconnect và tải bù

Client hiển thị ngắn gọn các trạng thái `Đang kết nối...`, `Mất kết nối, đang thử lại...`, `Đã kết nối lại.`. Nếu không kết nối được, gửi HTTP vẫn hoạt động và form không bị khóa bởi SignalR.

Sau `onreconnected`, client lấy `MaTinNhan` lớn nhất đang có của phòng mở và gọi `GET /ChatDuAn/TinNhanMoi?maPhongChat=...&afterMessageId=...&pageSize=50`. Endpoint riêng này kiểm tra `Chat.Xem`, chặn Admin, kiểm tra phòng, scope và membership; lọc soft delete, `MaTinNhan > afterMessageId`, sắp `MaTinNhan ASC`, giới hạn tối đa 50 và trả `HasMore`. Client tải tiếp tối đa 20 batch mỗi lần reconnect, append tăng dần và deduplicate; không reload trang, không reset phòng và không thay endpoint/cursor tải tin cũ.

### 29.6 Nginx, HTTPS và giới hạn triển khai

Repository không có file cấu hình Nginx/systemd và không xác nhận port Kestrel production, nên không tự tạo hoặc sửa một file deploy giả định. Nếu Nginx thực tế đang dùng, thêm location sau vào server HTTPS hiện hành và trỏ `quanlyduan_mvc` tới đúng upstream/port MVC của service đang triển khai:

```nginx
location /hubs/chat-du-an {
    proxy_pass http://quanlyduan_mvc;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection "upgrade";
    proxy_set_header Host $host;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
    proxy_read_timeout 3600s;
}
```

Khi trang ngoài dùng HTTPS, URL Hub tương đối cùng origin tự dùng WSS. Thiết kế hiện tại giả định một instance MVC; chưa thêm Redis backplane, Azure SignalR, sticky session hoặc cấu hình scale-out.

### 29.7 File thay đổi

- `Program.cs`, `Hubs/ChatDuAnHub.cs`.
- `Services/CauHinhDichVu.cs`, `IChatDuAnService.cs`, `ChatDuAnService.cs`.
- `IChatRealtimePublisher.cs`, `ChatRealtimePublisher.cs`.
- `ChatRealtimeMessageDto.cs`, `ChatDuAnTinNhanMoiBatchViewModel.cs`.
- `ChatDuAnController.cs`.
- `Views/ChatDuAn/Index.cshtml`, `_RoomItem.cshtml`.
- `wwwroot/js/ChatDuAn/chat.js`, client SignalR local.
- `wwwroot/css/ChatDuAn/index.css`, `message-list.css`.
- `docs/tinnhan.md`.

### 29.8 Build, kiểm tra và giới hạn xác minh

- `dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore -p:UseAppHost=false`: thành công, 0 lỗi; còn 2 cảnh báo `CS1998` có sẵn trong `FileTienDoCongViecService.cs`, không liên quan Chat.
- `node --check QuanLyDuAn/QuanLyDuAn/wwwroot/js/ChatDuAn/chat.js`: thành công.
- Razor, Hub mapping và DI compile thành công; không phát hiện circular dependency.
- Thử start tại `http://127.0.0.1:5117` không tới trạng thái listen vì startup seed không kết nối được SQL Server `LAPTOP-SI5JBDIU\SQLEXPRESS01/QuanLyDuAnAI7`; logger Event Log đồng thời bị từ chối quyền. Vì vậy chưa thể kiểm tra `/hubs/chat-du-an/negotiate`, Console/Network, hai tài khoản, reconnect, permission runtime hoặc Nginx/WSS trong môi trường này.
- Các kịch bản hai profile Manager/Employee, chống trùng, phòng khác, gỡ thành viên, mất mạng/tải bù và Hub lỗi đã được triển khai theo source nhưng vẫn phải chạy test browser tích hợp với SQL Server khả dụng trước khi xác nhận runtime production.

Đợt này không sửa CSDL, Entity, migration, schema hoặc dữ liệu nghiệp vụ. Phân trang phòng vẫn dùng batch đầu 20, tối đa 50, `lastRoomId`, `MaPhongChat DESC`, `Take(pageSize + 1)`, search reset cursor và deduplicate theo `MaPhongChat`. Phân trang tin cũ vẫn dùng batch đầu 30, tối đa 50 và cursor `(ThoiGianGui, MaTinNhan)`; endpoint tải bù là luồng riêng.
