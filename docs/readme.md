# README nội bộ cho dự án QuanLyDuAnAI

## 1. Kiến trúc tổng thể

- Frontend: ASP.NET MVC (Razor).
- Backend nghiệp vụ: ASP.NET MVC.
- CSDL: SQL Server.
- AI compute service: FastAPI (Python).

## 2. Nguyên tắc kiến trúc bắt buộc

- MVC là system-of-record cho toàn bộ dữ liệu nghiệp vụ và dữ liệu AI trong SQL Server.
- FastAPI chỉ thực hiện tính toán AI, không ghi trực tiếp SQL Server.
- Quyết định nghiệp vụ cuối cùng thuộc MVC.

## 3. Phạm vi AI hiện tại (reason-only)

- AI chỉ phân tích nguyên nhân trễ dự án.
- Không còn AI dự đoán dự án trễ hay không.
- Không còn model `TreHan` trong luồng chính.
- Không còn `delay_active.joblib`.
- Model chính duy nhất là `NguyenNhan`.

## 4. Luồng dữ liệu AI

1. MVC tổng hợp dữ liệu vào `AI_DATASET`.
2. MVC xác định `LaDuAnTre` bằng business rule.
3. Chỉ khi `LaDuAnTre = 1` mới gọi FastAPI phân tích nguyên nhân.
4. MVC lưu kết quả gợi ý vào `AI_KET_QUA`.
5. Manager xác nhận nguyên nhân thật vào `AI_NGUYEN_NHAN`.

## 5. Quy tắc train model nguyên nhân

- Feature cố định gồm 10 trường:
  - `SoNhanVienDuAn`
  - `TongSoCongViec`
  - `SoCongViecTre`
  - `TyLeCongViecTre`
  - `ChiPhiDuKien`
  - `ChiPhiThucTe`
  - `ChenhLechChiPhi`
  - `SoLanThayDoiNhanSu`
  - `SoLanThayDoiQuanLy`
  - `SoNgayTreTienDo`
- Label train: `MaDMNguyenNhan`.
- Chỉ lấy dòng `LaDuAnTre = 1`.
- Không dùng `AI_KET_QUA` làm ground truth train.
- Không dùng `IsTre` làm label AI.

## 6. Quyền AI chuẩn

- `AI.Dataset`
- `AI.Train`
- `AI.PhanTichNguyenNhan`
- `AI.Dashboard`
- `AI.XacNhan`

`AI.DuDoan` chỉ giữ tương thích alias nếu cần.

## 7. Chuẩn kỹ thuật

- Không dùng `StringComparison` trong LINQ query chạy trên EF Core.
- Chuỗi tiếng Việt SQL phải dùng `N'...'`.
- Tất cả file code/docs/script giữ UTF-8.
- Không để chuỗi lỗi font/mojibake trong UI, SQL, tài liệu.
