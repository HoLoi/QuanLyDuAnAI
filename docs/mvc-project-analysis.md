# Phân tích dự án MVC cho tích hợp AI

## 1. Kiến trúc MVC hiện tại

- `Program.cs` cấu hình xác thực/ủy quyền toàn cục.
- `DbContext` là nguồn ghi chính cho dữ liệu nghiệp vụ và dữ liệu AI.
- Service layer xử lý business rule, controller điều phối request/response.

## 2. Boundary MVC - FastAPI

- MVC: workflow, permission, scope dữ liệu, ghi DB.
- FastAPI: compute AI, không ghi SQL Server.

## 3. Trạng thái module AI sau triển khai

- Chỉ còn bài toán phân tích nguyên nhân trễ dự án.
- Không còn model `TreHan` trong luồng chính.
- Không còn dùng `delay_active.joblib`.
- Model duy nhất cho pipeline chính: `NguyenNhan`.

## 4. Quyền AI trong MVC

- `AI.Dataset`
- `AI.Train`
- `AI.PhanTichNguyenNhan`
- `AI.Dashboard`
- `AI.XacNhan`

`AI.DuDoan` chỉ còn alias tương thích nếu có.

## 5. Quy tắc dữ liệu AI trong MVC

- `LaDuAnTre` là trạng thái nghiệp vụ do MVC xác định.
- Khi `LaDuAnTre = 1`, MVC mới gọi FastAPI phân tích nguyên nhân.
- `AI_KET_QUA` lưu kết quả gợi ý AI.
- `AI_NGUYEN_NHAN` lưu xác nhận nguyên nhân thật của Manager.
- Train dùng label `MaDMNguyenNhan`, không dùng `IsTre`.

## 6. Chuẩn triển khai code

- Query EF Core không dùng `StringComparison` trong biểu thức DB.
- Chuỗi tiếng Việt trong UI/docs hiển thị đúng dấu UTF-8.
- SQL Unicode dùng `N'...'`.

> Cập nhật 2026-05-26: Luồng AI đã chuẩn hóa reason-only (NguyenNhan), train/test split trong RAM, metadata train/model phục vụ biểu đồ theo phiên bản model.
