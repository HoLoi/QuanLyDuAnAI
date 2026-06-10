# Log sửa giao diện Chat dự án

## 1. Mục tiêu sửa

Lỗi cũ: khi có nhiều phòng chat ở cột trái, danh sách phòng làm `.chat-layout` nở cao theo nội dung vì layout chỉ dùng `min-height`. Panel chat bên phải bị kéo theo, vùng tin nhắn không còn là vùng cuộn độc lập ổn định và form gửi tin nhắn không giữ đúng vai trò footer ở đáy panel.

Mục tiêu sau sửa: giữ nguyên nghiệp vụ và chỉ chỉnh CSS riêng của Chat dự án để layout có chiều cao hữu hạn theo viewport, danh sách phòng cuộn riêng, vùng tin nhắn cuộn riêng, còn form gửi tin nhắn nằm ở đáy panel chat.

## 2. File đã sửa

| File | Nội dung sửa | Lý do |
| --- | --- | --- |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/index.css` | Đổi `.chat-layout` từ `min-height` sang `height: calc(100vh - 150px)`, thêm `min-height: 0`, `overflow: hidden`; thêm `min-height: 0` cho `.chat-sidebar`, `.chat-main`; chỉnh responsive một cột. | Khóa chiều cao layout chat theo viewport để cột trái không kéo dài toàn trang và panel phải giữ cấu trúc 3 vùng. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/room-list.css` | Đổi `.chat-room-list` sang flex column, thêm `min-height: 0`; thêm `flex: 1 1 auto`, `min-height: 0` cho `.chat-room-items`; giới hạn chiều cao danh sách phòng trên màn nhỏ. | Đảm bảo danh sách phòng là vùng cuộn độc lập khi số phòng tăng. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/message-list.css` | Giữ `.chat-message-list` là vùng scroll, bỏ phụ thuộc `height: 100%`, thêm `align-content: start`. | Vùng tin nhắn nằm ở hàng giữa của `.chat-main` và tự cuộn trong chiều cao panel đã khóa. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/message-form.css` | Đổi `.chat-message-form` từ `position: sticky` sang `position: static`, thêm `flex-shrink: 0`. | Form gửi tin nhắn trở thành footer bình thường của panel 3 hàng, không phụ thuộc sticky. |

## 3. CSS/class đã thêm hoặc chỉnh

- `.chat-du-an-page`
- `.chat-layout`
- `.chat-sidebar`
- `.chat-main`
- `.chat-room-list`
- `.chat-room-items`
- `.chat-message-list`
- `.chat-message-form`
- Media query `@media (max-width: 992px)` trong `index.css`
- Media query `@media (max-width: 768px)` trong `room-list.css`

## 4. Kết quả sau khi sửa

- Danh sách phòng chat đã được cấu hình cuộn độc lập qua `.chat-room-items { flex: 1 1 auto; min-height: 0; overflow-y: auto; }`.
- Vùng tin nhắn đã được cấu hình cuộn độc lập qua `.chat-main` 3 hàng và `.chat-message-list { min-height: 0; overflow-y: auto; }`.
- Form gửi tin nhắn nằm ở đáy panel theo hàng `auto` cuối của `.chat-main`, không còn phụ thuộc `position: sticky`.
- Toàn trang không còn bị kéo dài bất thường chỉ vì có nhiều phòng chat trên desktop, vì `.chat-layout` đã có `height: calc(100vh - 150px)` và `overflow: hidden`.
- Font chữ tiếng Việt trong các file đã sửa/log vẫn đúng dấu khi kiểm tra bằng đọc UTF-8; không phát hiện chuỗi mojibake tiếng Việt.

## 5. Kiểm tra build

Lệnh đã chạy:

```powershell
dotnet build QuanLyDuAn.sln --no-restore -p:UseAppHost=false -p:CopyBuildOutputToOutputDirectory=false
```

Kết quả: build thành công, 0 lỗi. Có 2 warning CS1998 sẵn có ở `FileTienDoCongViecService.cs` về async method không có `await`, không liên quan đến thay đổi giao diện Chat dự án.

## 6. Kiểm tra encoding/font chữ

- Các file đã sửa hoặc tạo mới vẫn đọc được bằng UTF-8.
- Không phát hiện lỗi font tiếng Việt trong View/CSS/log.
- Không phát hiện các chuỗi mojibake tiếng Việt phổ biến trong nội dung file.

## 7. Lưu ý kiểm tra thủ công

- Kiểm tra nhiều phòng chat ở cột trái.
- Kiểm tra nhiều tin nhắn trong một phòng.
- Kiểm tra phòng chưa có tin nhắn.
- Kiểm tra người không có quyền gửi tin nhắn.
- Kiểm tra desktop và mobile.
- Kiểm tra chữ tiếng Việt trên giao diện chat.

## 8. Sửa tinh chỉnh danh sách phòng chat sau lần fix 1

### Vấn đề còn lại trước khi sửa

Sau lần fix 1, layout chính đã ổn hơn nhưng card phòng chat ở cột trái vẫn có cảm giác hơi chật. Các phần tên dự án, tên phòng, tin nhắn mới nhất, trạng thái và metadata cùng nằm trong cột 340px nên khi nội dung dài có thể tạo cảm giác bị bẹp hoặc bị cắt quá sớm. Scrollbar danh sách phòng cũng còn khá sát card.

### File đã sửa

| File | Nội dung sửa | Lý do |
| --- | --- | --- |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/room-list.css` | Tinh chỉnh padding, gap, min-height, line-clamp, status pill và metadata của danh sách phòng. | Vấn đề còn lại nằm ở card phòng chat bên trái, không cần sửa View hoặc CSS dùng chung. |

### Selector CSS đã chỉnh

- `.chat-room-items`: giữ `flex: 1 1 auto`, `min-height: 0`, `overflow-y: auto`; giảm nhẹ `gap`, tăng padding phải và thêm `scrollbar-gutter: stable`.
- `.chat-room-item`: thêm `min-height` hợp lý, tăng nhẹ padding dọc, không đặt height cố định.
- `.room-title`: tăng line-clamp từ 2 lên 3 dòng để tên dự án dài ít bị cắt quá sớm.
- `.room-status`: tăng nhẹ `max-width` để pill trạng thái cân đối hơn.
- `.room-subtitle`: thêm line-clamp 2 dòng để tên phòng dài được rút gọn có chủ đích.
- `.room-latest`: tăng line-clamp từ 2 lên 3 dòng và tăng `min-height` để phần tin mới nhất đỡ bị bẹp.
- `.room-meta`, `.meta-pill`: giảm nhẹ gap, padding và font-size để metadata gọn hơn nhưng vẫn đọc rõ.
- Responsive `@media (max-width: 768px)`: tăng `max-height` của `.chat-room-list` lên 46vh và đặt `min-height` thấp hơn cho item trên mobile.

### Kết quả sau khi chỉnh

- Card phòng chat có chiều cao tối thiểu nên đỡ chật hơn, nhưng không bị khóa bằng height cố định.
- Nội dung dài vẫn được xử lý có chủ đích bằng line-clamp/ellipsis, không để phá layout.
- Thanh cuộn danh sách phòng vẫn nằm trong `.chat-room-items`, có thêm khoảng đệm phải để không quá sát card.
- Cơ chế scroll độc lập của danh sách phòng vẫn được giữ nguyên.
- Không ảnh hưởng panel chat bên phải, vùng tin nhắn hoặc form gửi tin nhắn.
- Không sửa controller, service, route, model, entity, database, permission hoặc workflow.
- Không phát hiện lỗi font tiếng Việt khi kiểm tra UTF-8 trên các file đã sửa.

### Kiểm tra build

Lệnh đã chạy:

```powershell
dotnet build QuanLyDuAn.sln --no-restore -p:UseAppHost=false -p:CopyBuildOutputToOutputDirectory=false
```

Kết quả: build thành công, 0 lỗi. Có 2 warning CS1998 sẵn có ở `FileTienDoCongViecService.cs`, không liên quan đến thay đổi giao diện Chat dự án.
