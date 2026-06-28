# Hướng dẫn cài đặt hệ thống QuanLyDuAnAI

## 1. Tổng quan

Hệ thống gồm ba thành phần:

- **ASP.NET Core MVC**: xử lý giao diện, nghiệp vụ, phân quyền và lưu dữ liệu.
- **SQL Server**: lưu dữ liệu dự án và dữ liệu AI.
- **FastAPI**: huấn luyện model và phân tích nguyên nhân trễ.

MVC giao tiếp với FastAPI qua REST API. FastAPI không ghi trực tiếp vào SQL Server.

---

## 2. Phần mềm cần cài

- **.NET SDK 8.0 trở lên**
- **SQL Server** và **SQL Server Management Studio**
- **Python 3.10 trở lên**
- Visual Studio 2022 hoặc VS Code nếu cần chỉnh sửa source

Kiểm tra nhanh:

```powershell
dotnet --info
python --version
pip --version
```

---

## 3. Cài đặt cơ sở dữ liệu

### Bước 1: Tạo database

Mở SQL Server Management Studio và chạy:

```sql
CREATE DATABASE [QuanLyDuAnAI];
GO
```

### Bước 2: Chạy script chính

Mở file:

```text
quanlyduan.sql
```

Kiểm tra dòng đầu của script đang sử dụng đúng database:

```sql
USE [QuanLyDuAnAI]
GO
```

Sau đó chạy toàn bộ script để tạo bảng, khóa ngoại, dữ liệu danh mục và dữ liệu mẫu.

> Chỉ `quanlyduan.sql` là bắt buộc khi cài mới. Các file `seed-*.sql`, `data*.sql` hoặc `520duan*.sql` chỉ dùng khi cần bổ sung dữ liệu demo hoặc dữ liệu huấn luyện AI. Không chạy toàn bộ các file này cùng lúc vì có thể gây trùng dữ liệu.

### Bước 3: Cấu hình kết nối

Mở:

```text
QuanLyDuAn/QuanLyDuAn/appsettings.json
```

Cập nhật:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=<TEN_SQL_SERVER>;Initial Catalog=QuanLyDuAnAI;Integrated Security=True;Trust Server Certificate=True"
}
```

Thay `<TEN_SQL_SERVER>` bằng SQL Server instance trên máy, ví dụ:

```text
.\SQLEXPRESS
```

Tên database trong connection string phải trùng với database đã tạo.

---

## 4. Cấu hình ASP.NET Core MVC

Trong `appsettings.json`, kiểm tra các cấu hình chính.

### Kết nối FastAPI

```json
"AiApi": {
  "BaseUrl": "http://127.0.0.1:8001",
  "TimeoutSeconds": 10,
  "RetryCount": 1,
  "RetryDelayMilliseconds": 300
}
```

### Địa chỉ ứng dụng

```json
"AccountActivation": {
  "TokenLifetimeHours": 24,
  "ResendCooldownSeconds": 60,
  "AppBaseUrl": "http://<IP_MAY>:5037"
}
```

`AppBaseUrl` phải là địa chỉ HTTP/HTTPS hợp lệ. Source hiện tại không chấp nhận `localhost` hoặc `127.0.0.1`, vì vậy cần dùng IP LAN của máy, ví dụ:

```text
http://192.168.1.10:5037
```

Có thể kiểm tra IP bằng:

```powershell
ipconfig
```

Cấu hình email chỉ cần thiết khi sử dụng kích hoạt tài khoản hoặc quên mật khẩu.

---

## 5. Cài đặt và chạy FastAPI

Mở PowerShell tại thư mục gốc dự án và chạy:

```powershell
cd QuanLyDuAnAIService
python -m venv .venv
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\.venv\Scripts\Activate.ps1
python -m pip install --upgrade pip
pip install -r requirements.txt
```

Tạo file `.env` từ file mẫu:

```powershell
Copy-Item .env.example .env
```

Khởi động FastAPI:

```powershell
python run.py
```

FastAPI chạy tại:

```text
http://127.0.0.1:8001
```

Kiểm tra:

```text
http://127.0.0.1:8001/health
http://127.0.0.1:8001/docs
```

Nếu thư mục `models` đã có `reason_active.joblib`, hệ thống sẽ nạp model khi khởi động. Nếu chưa có model, cần huấn luyện model từ chức năng AI trong MVC.

---

## 6. Cài đặt và chạy ASP.NET Core MVC

Mở một PowerShell khác:

```powershell
cd QuanLyDuAn\QuanLyDuAn
dotnet restore
dotnet build
dotnet run --launch-profile http
```

Mở trình duyệt tại:

```text
http://localhost:5037
```

Giữ cửa sổ chạy MVC và FastAPI mở trong suốt quá trình sử dụng.

### Tài khoản mặc định

Nếu database chưa có tài khoản `admin`, hệ thống có thể tự tạo:

```text
Tài khoản: admin
Mật khẩu: 111111
```

Nên đổi mật khẩu sau lần đăng nhập đầu tiên.

---

## 7. Thứ tự khởi động hệ thống

Mỗi lần sử dụng, thực hiện theo thứ tự:

1. Khởi động SQL Server.
2. Chạy FastAPI:

```powershell
cd QuanLyDuAnAIService
.\.venv\Scripts\Activate.ps1
python run.py
```

3. Kiểm tra `http://127.0.0.1:8001/health`.
4. Chạy MVC:

```powershell
cd QuanLyDuAn\QuanLyDuAn
dotnet run --launch-profile http
```

5. Truy cập `http://localhost:5037` và đăng nhập.

---

## 8. Kiểm tra sau khi cài đặt

Hệ thống được cài đúng khi:

- MVC build và khởi động thành công.
- Không xuất hiện lỗi kết nối SQL Server.
- Trang đăng nhập hiển thị.
- Đăng nhập được bằng tài khoản hợp lệ.
- Dashboard tải được dữ liệu.
- FastAPI `/health` trả về trạng thái hoạt động.
- MVC kết nối được FastAPI.
- Danh sách model AI hiển thị được.
- Chức năng tổng hợp dữ liệu và phân tích nguyên nhân trễ hoạt động đúng quyền.

---

## 9. Một số lỗi thường gặp

### Không kết nối được SQL Server

Kiểm tra:

- SQL Server service đã chạy chưa.
- `Data Source` có đúng instance không.
- `Initial Catalog` có đúng tên database không.
- Đã chạy `quanlyduan.sql` chưa.

### MVC lỗi `AccountActivation:AppBaseUrl`

Không dùng `localhost` hoặc `127.0.0.1`. Thay bằng IP LAN:

```text
http://<IP_MAY>:5037
```

### FastAPI báo thiếu thư viện

Kích hoạt môi trường ảo và cài lại:

```powershell
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
```

### MVC không kết nối được FastAPI

Kiểm tra FastAPI đã chạy và mở được:

```text
http://127.0.0.1:8001/health
```

Đồng thời kiểm tra:

```json
"AiApi": {
  "BaseUrl": "http://127.0.0.1:8001"
}
```

### Cổng đang được sử dụng

Kiểm tra cổng:

```powershell
netstat -ano | findstr :5037
netstat -ano | findstr :8001
```

Dừng tiến trình đang chiếm cổng hoặc cấu hình lại đồng bộ giữa các thành phần.

### Không có model AI

Kiểm tra thư mục:

```text
QuanLyDuAnAIService/models
```

Nếu chưa có `reason_active.joblib`, sử dụng chức năng huấn luyện model trong hệ thống sau khi dữ liệu AI đáp ứng điều kiện.

---

## 10. Dừng hệ thống

Tại cửa sổ MVC và FastAPI, nhấn:

```text
Ctrl + C
```

Thoát môi trường Python:

```powershell
deactivate
```
