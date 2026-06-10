# Phân tích giao diện Chat dự án

## 1. Phạm vi source đã đọc

### View/Razor Chat dự án

- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/Index.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_RoomList.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_ChatHeader.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_MessageList.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_MessageForm.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_EmptyState.cshtml`
- `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/_MessageItem.cshtml`

### CSS riêng của Chat dự án

- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/index.css`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/room-list.css`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/chat-header.css`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/message-list.css`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/message-item.css`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/message-form.css`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/empty-state.css`

### CSS dùng chung có ảnh hưởng đến khung trang

- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/site.css`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/shared/ui.css`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/layout/sidebar.css`

### Log sửa giao diện

- `docs/chat-du-an-ui-fix-log.md`

Ghi chú: bước này chỉ đọc source hiện tại và cập nhật tài liệu. Không sửa View, CSS, controller, service, model, route hay database.

## 2. Trạng thái giao diện sau lần sửa layout 1

- Layout desktop hiện không còn để danh sách phòng chat tự kéo dài toàn bộ trang như trước. Nguyên nhân là `.chat-layout` trong `index.css` đã đổi sang `height: calc(100vh - 150px)`, có `min-height: 0` và `overflow: hidden`.
- Danh sách phòng chat đã có vùng cuộn riêng theo cấu hình hiện tại: `.chat-room-list` là flex column, còn `.chat-room-items` có `flex: 1 1 auto`, `min-height: 0` và `overflow-y: auto`.
- Panel chat bên phải đã giữ chiều cao ổn định hơn vì `.chat-main` có `min-height: 0`, `overflow: hidden`, và chia 3 hàng bằng `grid-template-rows: auto minmax(0, 1fr) auto`.
- Vùng tin nhắn đã được đặt làm vùng scroll độc lập bằng `.chat-message-list { min-height: 0; overflow-y: auto; }`. Vùng này nằm ở hàng giữa `minmax(0, 1fr)` của `.chat-main`.
- Form gửi tin nhắn hiện nằm ở hàng cuối của panel chat. `.chat-message-form` đã là `position: static`, không còn phụ thuộc `position: sticky`.
- Trên màn nhỏ, `.chat-layout` chuyển sang một cột, `height: auto`, `overflow: visible`; `.chat-main` có `height: 70vh` và `min-height: 420px`. Điều này giúp không mất form gửi tin nhắn, nhưng cần kiểm tra thực tế trên mobile vì body/page có thể scroll lại.
- Kiểm tra bằng đọc byte UTF-8 cho thấy các file View/CSS/log đọc được UTF-8, không có ký tự thay thế. Một số output PowerShell có thể hiển thị sai dấu, nhưng kiểm tra chuỗi bằng UTF-8 vẫn nhận diện đúng các cụm như `Chat dự án`, `Dự án`, `Gửi`, `gửi`.

## 3. Những điểm đã sửa tốt

- Không còn dùng `min-height` làm ràng buộc chính cho `.chat-layout`; layout desktop đã có chiều cao hữu hạn theo viewport.
- `.chat-layout`, `.chat-sidebar`, `.chat-main`, `.chat-room-list`, `.chat-room-items` đều đã có các điểm khóa `min-height: 0` cần thiết cho layout grid/flex có vùng cuộn con.
- Cột phòng chat đã chuyển từ grid hai hàng sang flex column, phù hợp hơn với cấu trúc: form tìm kiếm ở trên, danh sách phòng co giãn và cuộn ở dưới.
- Panel chat bên phải đã chia rõ 3 vùng: header, danh sách tin nhắn, form gửi tin nhắn.
- Form gửi tin nhắn đã được đưa về footer bình thường của panel, không còn dùng sticky để giả lập cố định đáy.
- Vùng tin nhắn không còn phụ thuộc `height: 100%`; nó dựa vào hàng giữa của `.chat-main`, đúng hướng cho grid 3 hàng.
- Empty state tin nhắn vẫn nằm trong `.chat-message-list`, không làm phá cấu trúc header/body/footer.
- Không cần sửa CSS dùng chung ở lần fix 1, nên rủi ro ảnh hưởng màn hình khác thấp.

## 4. Những vấn đề còn tồn tại trên giao diện hiện tại

- Card phòng chat bên trái có thể tạo cảm giác bị bẹp hoặc thiếu thông tin khi viewport thấp hoặc khi có nhiều phòng, vì toàn bộ cột trái bị giới hạn trong chiều cao `.chat-layout` và từng item phải hiển thị nhiều nhóm thông tin trong một vùng hẹp 340px.
- `.chat-room-item` không có `min-height`; chiều cao item phụ thuộc nội dung, padding và các line-clamp. Về mặt CSS không bị ép thấp trực tiếp, nhưng cũng chưa có chiều cao tối thiểu để bảo đảm cảm giác card đủ thoáng.
- `.chat-room-item` có `overflow: hidden`. Nếu nội dung con hoặc hiệu ứng active/hover vượt ra ngoài card thì sẽ bị cắt. Thuộc tính này đang giúp bo góc và che hiệu ứng, nhưng cũng là điểm cần kiểm tra nếu card bị mất thông tin.
- `.room-title` đang dùng `display: -webkit-box`, `-webkit-line-clamp: 2`, `overflow: hidden`. Tên dự án dài sẽ bị cắt sau 2 dòng. Đây là cắt có chủ đích, nhưng nếu người dùng cần đọc đủ tên dự án thì cần cân nhắc tooltip hoặc tăng số dòng.
- `.room-latest` cũng dùng line-clamp 2 dòng và `min-height: 2.4em`. Tin nhắn mới nhất dài sẽ bị cắt sau 2 dòng; không phải lỗi layout chính, nhưng có thể là nguyên nhân người dùng thấy mô tả/tin nhắn không hiển thị đủ.
- `.room-status` có `max-width: 120px`, `white-space: nowrap`, `overflow: hidden`, `text-overflow: ellipsis`. Trạng thái dự án dài có thể bị rút gọn trong pill.
- `.room-meta` dùng `flex-wrap: wrap`; nếu có đủ `Số thành viên`, `Số tin`, và thời gian tin nhắn mới nhất thì metadata có thể xuống nhiều dòng. Không bị mất về CSS, nhưng trong card hẹp sẽ làm item cao hơn và giảm số room nhìn thấy trước khi scroll.
- `.chat-room-items` đã cuộn riêng, nhưng padding bên phải chỉ `0.25rem`; khi scrollbar xuất hiện có thể hơi sát nội dung. Nên kiểm tra trực quan xem scrollbar có đè cảm giác lên card không.
- Trên mobile, `.chat-room-list` có `max-height: 42vh`. Cấu hình này giúp danh sách phòng không chiếm toàn màn hình, nhưng nếu item phòng cao hoặc thông tin nhiều thì người dùng có thể thấy ít room và phải scroll nhiều.
- `.chat-message-empty` dùng `margin: auto` trong `.chat-message-list` đang là grid có `align-content: start`. Cần kiểm tra thực tế empty state có thật sự nằm cân đối giữa vùng tin nhắn hay bị dạt theo cách grid tính layout.
- `.message-input` có `resize: vertical` và `max-height: 130px`. Nếu người dùng kéo textarea cao, form có thể chiếm nhiều chiều cao footer và làm vùng tin nhắn nhỏ lại. Đây không phá layout nhưng cần kiểm tra trải nghiệm.
- `.message-submit` có `min-width: 96px`, `.message-form-row` desktop chia `1fr auto`; ổn trên desktop nhưng cần kiểm tra nút và textarea trên mobile vì media query đổi thành một cột.

## 5. Nguyên nhân kỹ thuật của các vấn đề còn lại

- `.chat-layout` trong `index.css`: hiện đã khóa chiều cao desktop bằng `height: calc(100vh - 150px)`. Đây là sửa đúng cho lỗi kéo dài trang, nhưng cũng làm toàn bộ nội dung chat phải sống trong chiều cao hữu hạn. Khi viewport thấp, cột phòng sẽ cuộn nhiều hơn và item có thể tạo cảm giác chật.
- `.chat-sidebar` trong `index.css`: có `overflow: hidden`, giúp nội dung không tràn khỏi panel. Nếu `.chat-room-list` hoặc scrollbar cần thêm không gian, overflow hidden ở cấp sidebar có thể che phần vượt ngoài.
- `.chat-room-list` trong `room-list.css`: đang `height: 100%`, `min-height: 0`, flex column và `overflow: hidden`. Cấu trúc này đúng cho scroll riêng, nhưng nếu cần khoảng breathing room thì phải chỉnh ở `.chat-room-items`/item thay vì bỏ overflow ở cha.
- `.chat-room-items` trong `room-list.css`: `flex: 1 1 auto`, `min-height: 0`, `overflow-y: auto` là đúng cho scroll. Tuy nhiên padding phải `0.25rem` hơi mỏng khi scrollbar xuất hiện; `gap: 0.95rem` khá ổn nhưng có thể làm danh sách ít item hơn trong viewport thấp.
- `.chat-room-item` trong `room-list.css`: `padding: 1.05rem 1rem`, `overflow: hidden`, không có `min-height`. Không có height cố định sai, nhưng thiếu `min-height` có thể làm card không có chuẩn kích thước ổn định giữa các trạng thái ít/nhiều nội dung.
- `.room-title`: line-clamp 2 dòng bảo vệ layout khỏi tên dự án quá dài, đồng thời là nguyên nhân trực tiếp khiến tên dài không hiển thị đủ.
- `.room-latest`: line-clamp 2 dòng và `overflow: hidden` bảo vệ card khỏi tin nhắn dài, nhưng là nguyên nhân trực tiếp khiến tin mới nhất dài bị cắt.
- `.room-meta`: flex-wrap cho phép đủ dữ liệu không mất, nhưng các pill có `white-space: nowrap`; khi cột hẹp, metadata xuống dòng nhiều và làm card cao hơn.
- `.room-status`: `max-width: 120px` và ellipsis có thể cắt trạng thái dài. Nếu trạng thái dự án tiếng Việt dài hơn 120px thì người dùng chỉ thấy một phần.
- `.chat-main`: grid 3 hàng hiện đúng. Không nên phá cơ chế này ở bước sau; nếu lỗi còn lại chỉ nằm ở card phòng thì không cần sửa `.chat-main`.
- `.chat-message-list`: `display: grid`, `align-content: start`, `overflow-y: auto`. Với nhiều tin nhắn thì ổn; với empty state cần kiểm tra căn giữa vì `.chat-message-empty { margin: auto }` trong grid có thể không tạo cảm giác giữa vùng nếu nội dung/track bị tính theo start.
- `.chat-message-form`: `position: static`, `flex-shrink: 0`, padding ổn. Vì nó đang nằm ở row auto cuối của `.chat-main`, không nên quay lại sticky nếu không có lý do rõ.

Không thấy selector `.room-main` trong source hiện tại. Các vùng tương ứng đang là `.room-card-header`, `.room-title`, `.room-subtitle`, `.room-latest`, `.room-meta`.

## 6. Đề xuất hướng sửa tiếp theo

- Giữ nguyên cơ chế scroll độc lập đã sửa ở lần 1: không bỏ `height`/`overflow` của `.chat-layout`, không bỏ `min-height: 0` ở các container chính.
- Nếu card phòng chat vẫn bị bẹp hoặc thiếu thông tin, ưu tiên tinh chỉnh `room-list.css`, không sửa controller/service/View.
- Có thể thêm `min-height` hợp lý cho `.chat-room-item` để card có nền kích thước ổn định hơn, nhưng tránh đặt height cố định làm cắt nội dung.
- Có thể rà lại `overflow: hidden` trên `.chat-room-item`; chỉ giữ nếu cần bo góc/hiệu ứng. Nếu nội dung thật sự bị cắt ngoài line-clamp có chủ đích, cần thay bằng overflow ở selector con cụ thể.
- Có thể điều chỉnh line-clamp cho `.room-title` và `.room-latest`: giữ 2 dòng nếu ưu tiên mật độ, tăng lên 3 dòng hoặc thêm tooltip nếu ưu tiên đọc đủ.
- Có thể tăng padding phải của `.chat-room-items` để scrollbar không sát card.
- Có thể tinh chỉnh `.room-meta` và `.meta-pill` để metadata gọn hơn, ví dụ giảm padding pill hoặc sắp xếp lại thời gian tin nhắn.
- Chỉ sửa `index.css` nếu kiểm tra thực tế cho thấy chiều cao tổng `calc(100vh - 150px)` chưa phù hợp với topbar/padding ở một số viewport.
- `message-list.css` chỉ cần chỉnh nhẹ nếu empty state chưa cân đối hoặc vùng tin nhắn chưa scroll đúng trong trường hợp nhiều tin nhắn.
- `message-form.css` chỉ cần chỉnh nhẹ nếu textarea/button chiếm quá nhiều chiều cao hoặc mobile hiển thị chưa tốt.
- Không sửa CSS dùng chung `site.css`, `shared/ui.css`, `layout/sidebar.css` nếu vấn đề còn lại chỉ nằm trong card phòng chat.

## 7. Danh sách file nên sửa ở bước tiếp theo

| File | Mức độ ưu tiên | Lý do sửa |
| --- | --- | --- |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/room-list.css` | Cao | Đây là nơi quyết định card phòng chat, scrollbar danh sách phòng, line-clamp tiêu đề/tin mới nhất, metadata và trạng thái. Nếu card bị bẹp/cắt, sửa ở đây trước. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/index.css` | Trung bình | Chỉ sửa nếu chiều cao tổng `height: calc(100vh - 150px)` chưa đúng với layout thật hoặc responsive một cột còn làm body scroll quá nhiều. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/message-list.css` | Thấp đến trung bình | Sửa nếu nhiều tin nhắn chưa cuộn mượt hoặc empty state chưa cân đối trong vùng message body. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/message-form.css` | Thấp | Sửa nếu textarea/button làm footer quá cao, mobile chưa đẹp, hoặc trạng thái không có quyền gửi chưa cân đối. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/ChatDuAn/chat-header.css` | Thấp | Chỉ sửa nếu header chat quá cao do subtitle pill wrap nhiều dòng, làm vùng tin nhắn bị thu hẹp. |
| `QuanLyDuAn/QuanLyDuAn/Views/ChatDuAn/Index.cshtml` | Rất thấp | Không cần sửa hiện tại; chỉ đụng nếu cần thêm wrapper/class scoped mà CSS hiện tại không thể xử lý. |
| `QuanLyDuAn/QuanLyDuAn/wwwroot/css/site.css`, `shared/ui.css`, `layout/sidebar.css` | Tránh sửa | CSS dùng chung, không nên sửa cho lỗi riêng của Chat dự án nếu không bắt buộc. |

## 8. Checklist kiểm tra sau lần sửa tiếp theo

- Nhiều phòng chat nhưng toàn trang không bị kéo dài.
- Card phòng chat hiển thị đủ tên dự án, tên phòng, trạng thái, số thành viên, số tin và thời gian tin mới nhất khi có.
- Tên dự án dài được xử lý có chủ đích: hoặc line-clamp đẹp, hoặc có cách xem đủ.
- Tin nhắn mới nhất dài được xử lý có chủ đích: không phá layout và không tạo cảm giác bị cắt lỗi.
- Danh sách phòng chat cuộn riêng trong `.chat-room-items`.
- Thanh cuộn danh sách phòng nằm đúng trong vùng danh sách, không đè khó chịu lên card.
- Nhiều tin nhắn thì vùng `.chat-message-list` cuộn riêng.
- Form gửi tin nhắn luôn nằm dưới panel chat.
- Phòng chưa có tin nhắn hiển thị empty state cân đối.
- Người không có quyền gửi thấy trạng thái disabled gọn, không làm footer quá cao.
- Desktop không vỡ layout hai cột.
- Mobile không mất form gửi tin nhắn và danh sách phòng không chiếm toàn bộ màn hình.
- Không lỗi font tiếng Việt trong View, CSS và tài liệu.
