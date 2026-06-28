# Hướng dẫn cài đặt hệ thống QuanLyDuAnAI

Tài liệu này mô tả cách cài đặt, cấu hình, khởi chạy và kiểm tra hệ thống **QuanLyDuAnAI** trên Windows, dựa trên source code hiện tại trong repository. Mọi đường dẫn, cổng, tên file và cấu hình đều lấy từ source thực tế.

---

## 1. Giới thiệu

Hệ thống **QuanLyDuAnAI** gồm ba thành phần chính:

| Thành phần | Vai trò |
| ---------- | ------- |
| **ASP.NET Core MVC** (`QuanLyDuAn/QuanLyDuAn`) | Ứng dụng quản lý nghiệp vụ dự án: đăng nhập, phân quyền, workflow, dashboard, tổng hợp dữ liệu AI, gọi FastAPI và ghi kết quả vào SQL Server. |
| **SQL Server** | Lưu toàn bộ dữ liệu nghiệp vụ và dữ liệu AI (`AI_DATASET`, `AI_MODEL`, `AI_KET_QUA`, `AI_NGUYEN_NHAN`, …). |
| **FastAPI AI Service** (`QuanLyDuAnAIService`) | Dịch vụ tính toán AI độc lập: validate dataset, huấn luyện model, phân tích nguyên nhân trễ. **Không** truy cập hoặc ghi trực tiếp vào SQL Server. |

Luồng tích hợp:

```
Trình duyệt → ASP.NET MVC → SQL Server
                    ↓ REST API
              FastAPI (compute-only, model lưu cục bộ)
```

MVC giao tiếp FastAPI qua HTTP JSON theo cấu hình `AiApi:BaseUrl` trong `appsettings.json`.

---

## 2. Cấu trúc thư mục quan trọng

Cây thư mục rút gọn (chỉ liệt kê phần thực sự tồn tại):

```
<TEN_THU_MUC_DU_AN>/                 ← thư mục gốc repository
├── QuanLyDuAn/
│   ├── QuanLyDuAn.sln               ← file solution
│   └── QuanLyDuAn/
│       ├── QuanLyDuAn.csproj        ← project MVC
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Properties/launchSettings.json
│       ├── Data/
│       ├── Services/
│       ├── Controllers/
│       ├── Migrations/              ← có migration EF, nhưng startup không tự chạy migrate
│       └── wwwroot/                 ← static file + thư mục upload runtime
├── QuanLyDuAnAIService/             ← project FastAPI
│   ├── run.py                       ← script khởi chạy chính thức
│   ├── requirements.txt
│   ├── .env.example
│   ├── app/
│   │   ├── main.py
│   │   ├── config.py
│   │   ├── constants.py
│   │   └── routers/
│   ├── models/                      ← file .joblib và metadata AI
│   ├── logs/
│   └── sample_data/
├── quanlyduan.sql                   ← script SQL schema + dữ liệu mẫu chính
├── seed-demo-data-simple.sql        ← dữ liệu demo (tùy chọn)
├── seed-demo-data-ai-expanded.sql   ← dữ liệu AI mở rộng (tùy chọn)
├── seed-ai-training-extra.sql       ← dữ liệu huấn luyện AI bổ sung (tùy chọn)
├── data2.sql … data9.sql            ← snapshot DB khác tên (tham khảo/tùy chọn)
├── 520duan*.sql                     ← dữ liệu AI dung lượng lớn (tùy chọn)
└── docs/                            ← tài liệu nội bộ
    ├── readme.md
    ├── ai-he-thong-phan-tich.md
    ├── mvc-ai-integration-rules.md
    └── workflow-he-thong.md
```

---

## 3. Yêu cầu môi trường

| Thành phần | Phiên bản yêu cầu | Mục đích | Cách kiểm tra |
| ---------- | ----------------: | -------- | ------------- |
| .NET SDK | **8.0 trở lên** (`TargetFramework`: `net8.0` trong `QuanLyDuAn.csproj`) | Build và chạy ASP.NET Core MVC | `dotnet --info` |
| ASP.NET Core Runtime | 8.0 (đi kèm SDK 8+) | Chạy ứng dụng MVC | `dotnet --list-runtimes` |
| Entity Framework Core | **8.0.11** (package trong `.csproj`) | Truy cập SQL Server từ MVC | xem `QuanLyDuAn.csproj` |
| SQL Server | SQL Server 2016+ hoặc **SQL Server Express** (tương thích EF Core SqlServer 8.0.11) | Cơ sở dữ liệu nghiệp vụ | SSMS hoặc `Get-Service *SQL*` |
| SSMS | Khuyến nghị (không bắt buộc nếu dùng công cụ SQL khác) | Chạy script SQL, kiểm tra bảng | mở SQL Server Management Studio |
| Python | **3.10+** (môi trường hiện có dùng **3.10.11** theo `QuanLyDuAnAIService/.venv/pyvenv.cfg`) | Chạy FastAPI AI | `python --version` |
| pip | Phiên bản đi kèm Python | Cài dependency Python | `pip --version` |
| FastAPI | Khai báo trong `requirements.txt`; môi trường mẫu: **0.136.1** | Web API AI | `pip show fastapi` |
| Uvicorn | Khai báo trong `requirements.txt`; môi trường mẫu: **0.46.0** | ASGI server | `pip show uvicorn` |
| scikit-learn | `requirements.txt`; môi trường mẫu: **1.7.2** | Huấn luyện Decision Tree | `pip show scikit-learn` |
| pandas / NumPy / joblib / pydantic / python-dotenv | `requirements.txt` | Xử lý dữ liệu và cấu hình AI | `pip show pandas numpy joblib pydantic python-dotenv` |
| Node.js / npm | **Không cần** (repository không có `package.json`) | — | — |
| Visual Studio 2022 | **Không bắt buộc**; có thể dùng VS Code + CLI | Mở solution, debug MVC | mở `QuanLyDuAn.sln` |
| Visual Studio workload (nếu dùng VS) | **ASP.NET and web development** | Build/run MVC trên Visual Studio | Visual Studio Installer |

**Lưu ý:** `requirements.txt` không ghim phiên bản cố định. Sau `pip install -r requirements.txt`, phiên bản thực tế phụ thuộc thời điểm cài. Bảng trên ghi thêm phiên bản đã quan sát trong `.venv` hiện có của dự án.

Ví dụ kiểm tra nhanh trên PowerShell:

```powershell
dotnet --info
dotnet --list-runtimes
python --version
pip --version
Get-Service *SQL* | Format-Table Name, Status -AutoSize
```

---

## 4. Tải source và mở đúng thư mục

1. Giải nén hoặc clone repository vào một thư mục, ví dụ `<TEN_THU_MUC_DU_AN>`.
2. File solution: `<TEN_THU_MUC_DU_AN>/QuanLyDuAn/QuanLyDuAn.sln`.
3. Thư mục chạy **MVC**:

```powershell
cd <TEN_THU_MUC_DU_AN>\QuanLyDuAn\QuanLyDuAn
```

4. Thư mục chạy **FastAPI**:

```powershell
cd <TEN_THU_MUC_DU_AN>\QuanLyDuAnAIService
```

Không dùng đường dẫn tuyệt đối gắn với máy của tác giả. Thay `<TEN_THU_MUC_DU_AN>` bằng vị trí thực tế trên máy bạn.

---

## 5. Cài đặt SQL Server

Dự án hiện triển khai database bằng **file SQL** (`quanlyduan.sql`), **không** tự chạy EF migration khi khởi động MVC (`Program.cs` không gọi `Database.Migrate()`).

### 5.1. Cài SQL Server và SSMS

1. Cài **SQL Server** (Developer/Express) trên Windows.
2. Khuyến nghị cài **SQL Server Management Studio (SSMS)** để chạy script và kiểm tra dữ liệu.
3. Đảm bảo dịch vụ SQL Server đang chạy:

```powershell
Get-Service *SQL* | Where-Object { $_.Status -eq 'Running' }
```

4. Xác định tên instance (ví dụ mặc định: `(localdb)\MSSQLLocalDB`, `.\SQLEXPRESS`, `.\SQLEXPRESS01`, hoặc tên máy `\SQLEXPRESS`).

### 5.2. Tạo cơ sở dữ liệu

File `quanlyduan.sql` **không** chứa lệnh `CREATE DATABASE`. Script bắt đầu bằng:

```sql
USE [QuanLyDuAnAI]
GO
```

**Bước bắt buộc:** tạo database trước khi chạy script, ví dụ trong SSMS:

```sql
CREATE DATABASE [QuanLyDuAnAI];
GO
```

Bạn có thể đặt tên database khác (ví dụ `QuanLyDuAnAI6` như trong `appsettings.json` mẫu), nhưng khi đó phải:

- Sửa dòng `USE [...]` trong script trước khi chạy, **hoặc**
- Chọn đúng database trong SSMS trước khi Execute, **và**
- Cập nhật `ConnectionStrings:DefaultConnection` cho khớp tên database.

### 5.3. Thứ tự chạy script SQL

#### Bắt buộc (cài mới lần đầu)

| Thứ tự | File | Mục đích | Ghi chú |
| ------ | ---- | -------- | ------- |
| 1 | `quanlyduan.sql` | Tạo toàn bộ schema, ràng buộc, dữ liệu danh mục và dữ liệu mẫu ban đầu | Database mặc định trong script: **`QuanLyDuAnAI`**. Script **không** tự tạo database. |

Sau khi chạy `quanlyduan.sql`, kiểm tra các bảng cốt lõi tồn tại:

```sql
SELECT name FROM sys.tables
WHERE name IN (
  N'DU_AN', N'CONG_VIEC', N'CT_CONG_VIEC', N'AI_DATASET',
  N'AI_MODEL', N'AI_KET_QUA', N'AI_NGUYEN_NHAN', N'DM_NGUYEN_NHAN',
  N'AspNetUsers', N'AspNetRoles', N'DANH_MUC_QUYEN', N'DANH_MUC_MAN_HINH'
)
ORDER BY name;
```

Kiểm tra vai trò và quyền:

```sql
SELECT Name FROM dbo.AspNetRoles;
SELECT COUNT(*) AS SoQuyen FROM dbo.DANH_MUC_QUYEN;
SELECT COUNT(*) AS SoManHinh FROM dbo.DANH_MUC_MAN_HINH;
```

#### Tùy chọn — dữ liệu demo / huấn luyện AI

Chỉ chạy **sau** `quanlyduan.sql` và khi đã chọn đúng database (`USE [QuanLyDuAnAI]`):

| File | Mục đích | Cảnh báo |
| ---- | -------- | -------- |
| `seed-demo-data-simple.sql` | Thêm bộ dữ liệu demo nghiệp vụ | Có kiểm tra bảng bắt buộc; không nên chạy lặp nếu đã có dữ liệu demo tương tự |
| `seed-demo-data-ai-expanded.sql` | Mở rộng dữ liệu AI reason-only | Có thể ALTER thêm cột `AI_DATASET` nếu thiếu; prefix `SEED_AI_REASON_EXPANDED` |
| `seed-ai-training-extra.sql` | Thêm dữ liệu huấn luyện nguyên nhân | Prefix `SEED_AI_TRAIN_EXTRA`; dùng khi cần đủ ngưỡng train |

#### Tùy chọn — Dữ liệu AI dung lượng lớn (trình diễn / huấn luyện)

Các file sau là snapshot hoặc bổ sung dữ liệu lớn, **không** bắt buộc cho cài đặt tối thiểu:

| File | Database trong script |
| ---- | --------------------- |
| `520duantre.sql`, `520duanhoanthanhtre.sql` | `QuanLyDuAnAI1` |
| `520duanhoanthanhtrecodataset.sql`, `520duanhoanthanhtrecodatasetvanguyennhan*.sql` | `QuanLyDuAnAI2` (bản `fixver3` là bản sửa mới nhất trong repo) |
| `data2.sql` … `data9.sql`, `10duan.sql` | `QuanLyDuAnAI1` |
| `insert-du-lieu-10-du-an-tre-moi.sql`, `insert-du-lieu-100-du-an-tre-moi.sql`, `insert-du-lieu-400-du-an-tre-moi.sql` | **Không có** `USE`; phải chọn database thủ công trước khi chạy |

**Không chạy máy móc toàn bộ file SQL** nếu chưa đọc mục đích: nhiều file tạo snapshot riêng hoặc dữ liệu trùng prefix (`DATA8-`, `SEED_…`).

#### Script bổ trợ trong `docs/` (chỉ khi schema/dữ liệu cũ thiếu cột)

| File | Mục đích |
| ---- | -------- |
| `docs/update-schema-ngay-hoan-thanh-thuc-te-du-an.sql` | Cập nhật schema cột ngày hoàn thành thực tế |
| `docs/backfill-ngay-hoan-thanh-thuc-te-du-an.sql` | Backfill dữ liệu cho cột trên |
| `docs/insert-du-lieu-van-hanh-tien-do-du-an-tre.sql` và bản `*-fixed.sql` | Dữ liệu kiểm thử tiến độ dự án trễ |

Chỉ chạy khi tài liệu/schema hiện tại yêu cầu; `quanlyduan.sql` mới thường đã đủ cho cài mới.

### 5.4. Migration EF Core (phương án phụ, không phải luồng chính)

Repository có thư mục `QuanLyDuAn/QuanLyDuAn/Migrations/` (ví dụ migration `20260527125053_Init`), nhưng:

- `Program.cs` **không** gọi `Database.Migrate()`.
- `quanlyduan.sql` ghi migration history `20260505133317_Init` — **khác tên** với migration trong source hiện tại.

**Khuyến nghị:** dùng `quanlyduan.sql` cho cài đặt mới. Chỉ cân nhắc `dotnet ef database update` khi bạn chủ động chọn luồng migration và chấp nhận đối chiếu schema thủ công.

---

## 6. Cấu hình connection string

Key thực tế trong `QuanLyDuAn/QuanLyDuAn/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=<TEN_SQL_SERVER>;Initial Catalog=<TEN_DATABASE>;Integrated Security=True;Trust Server Certificate=True"
  }
}
```

### 6.1. Giải thích các thành phần

| Thành phần | Ý nghĩa |
| ---------- | ------- |
| `Data Source` | Tên server/instance SQL Server (ví dụ `.\SQLEXPRESS`, `(localdb)\MSSQLLocalDB`, `TEN_MAY\SQLEXPRESS01`) |
| `Initial Catalog` | Tên database (script chính dùng `QuanLyDuAnAI`; file mẫu hiện tại dùng `QuanLyDuAnAI6` — **phải khớp database bạn đã tạo**) |
| `Integrated Security=True` | Windows Authentication |
| `Trust Server Certificate=True` | Bỏ qua lỗi chứng chỉ khi kết nối local/dev |

### 6.2. SQL Server Authentication (nếu cần)

Source mẫu dùng Windows Authentication. Nếu SQL Server dùng login SQL, đổi connection string theo dạng:

```json
"DefaultConnection": "Data Source=<TEN_SQL_SERVER>;Initial Catalog=<TEN_DATABASE>;User ID=<TEN_DANG_NHAP_SQL>;Password=<MAT_KHAU_SQL>;Trust Server Certificate=True"
```

Không commit mật khẩu thật vào Git.

### 6.3. Kiểm tra kết nối

1. Mở SSMS, kết nối bằng cùng server/instance như connection string.
2. Xác nhận database `<TEN_DATABASE>` tồn tại và có bảng `DU_AN`.
3. Khởi chạy MVC (mục 7); nếu connection string sai, ứng dụng sẽ lỗi khi `KhoiTaoTaiKhoanMacDinh.DamBaoDuLieuAsync` chạy lúc startup.

### 6.4. Lỗi chứng chỉ thường gặp

Nếu gặp lỗi SSL/certificate khi kết nối SQL Server local, thêm hoặc giữ `Trust Server Certificate=True` như connection string mẫu.

---

## 7. Cài đặt ASP.NET Core MVC

### 7.1. Restore, build và chạy bằng CLI (hướng dẫn chính)

```powershell
cd <TEN_THU_MUC_DU_AN>\QuanLyDuAn\QuanLyDuAn
dotnet restore
dotnet build
dotnet run --launch-profile http
```

Project file: `QuanLyDuAn.csproj`  
Profile khuyến nghị: **`http`** (trong `Properties/launchSettings.json`).

### 7.2. Cổng và URL

Theo `Properties/launchSettings.json`:

| Profile | URL |
| ------- | --- |
| `http` (mặc định khuyến nghị) | `http://0.0.0.0:5037` |
| `https` | `https://localhost:7298` và `http://0.0.0.0:5037` |
| IIS Express | `http://localhost:11893` |

Trên máy local, mở trình duyệt:

- `http://localhost:5037`
- hoặc `http://127.0.0.1:5037`

Trang đăng nhập mặc định: `/Account/Login` (cấu hình trong `Program.cs`).

### 7.3. Hành vi khi khởi động MVC

| Hạng mục | Hành vi thực tế |
| -------- | ---------------- |
| EF Migration | **Không** tự chạy |
| Khởi tạo dữ liệu | **Có** — `KhoiTaoTaiKhoanMacDinh.DamBaoDuLieuAsync()` chạy mỗi lần startup |
| Role mặc định | Tạo/bổ sung `Admin`, `Manager`, `Employee` nếu thiếu |
| Tài khoản admin | Tạo `admin` nếu chưa có user `ADMIN` (xem mục 11) |
| Danh mục | Bổ sung màn hình, quyền, chức danh, loại dự án, mức ưu tiên, tiêu chí đánh giá, `DM_NGUYEN_NHAN` |
| Thư mục upload | Tự tạo khi upload lần đầu dưới `wwwroot/uploads/...` |
| Email | **Không bắt buộc** để đăng nhập; cần cấu hình nếu dùng kích hoạt tài khoản / quên mật khẩu qua email |

### 7.4. Cấu hình bắt buộc khác trong `appsettings.json`

#### Email (`EmailSettings`)

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "SenderEmail": "",
  "SenderName": "QuanLyDuAn AI",
  "Username": "",
  "AppPassword": ""
}
```

Điền `SenderEmail`, `Username`, `AppPassword` tại vị trí cấu hình trên máy bạn. **Không** ghi secret thật vào tài liệu hoặc commit Git.

#### Kích hoạt tài khoản (`AccountActivation`)

```json
"AccountActivation": {
  "TokenLifetimeHours": 24,
  "ResendCooldownSeconds": 60,
  "AppBaseUrl": "http://<IP_LAN_HOAC_IP_MAY>:5037"
}
```

`Program.cs` **validate khi startup**:

- `AppBaseUrl` phải là URL HTTP/HTTPS tuyệt đối.
- **Không được** dùng `localhost` hoặc IP loopback (`127.0.0.1`).

Vì vậy, ngay cả khi chỉ chạy trên một máy, bạn vẫn cần đặt URL bằng **địa chỉ IP LAN** của máy (ví dụ `http://192.168.x.x:5037`), không dùng `http://localhost:5037`.

Tra IP LAN trên Windows:

```powershell
Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.IPAddress -notlike '127.*' } | Select-Object IPAddress, InterfaceAlias
```

### 7.5. Chạy bằng Visual Studio (tùy chọn)

1. Mở `QuanLyDuAn/QuanLyDuAn.sln`.
2. Chọn project `QuanLyDuAn` làm startup project.
3. Chọn profile **`http`**.
4. Nhấn F5.

---

## 8. Cài đặt FastAPI

### 8.1. Tạo môi trường ảo và cài dependency

```powershell
cd <TEN_THU_MUC_DU_AN>\QuanLyDuAnAIService
python -m venv .venv
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\.venv\Scripts\Activate.ps1
python -m pip install --upgrade pip
pip install -r requirements.txt
```

File dependency: `QuanLyDuAnAIService/requirements.txt`:

```
fastapi
uvicorn
pandas
numpy
scikit-learn
joblib
python-dotenv
pydantic
```

### 8.2. Biến môi trường (`.env`)

Sao chép mẫu:

```powershell
cd <TEN_THU_MUC_DU_AN>\QuanLyDuAnAIService
Copy-Item .env.example .env
```

Nội dung mẫu từ `.env.example`:

```env
MODEL_DIR=models
DEFAULT_REASON_MODEL_ALIAS=reason_active.joblib
ALLOW_ORIGINS=http://localhost:5000,http://localhost:5001
MIN_REASON_TRAIN_ROWS=30
MIN_REASON_CLASS_COUNT=2
MIN_REASON_ROWS_PER_CLASS=5
REASON_CONFIDENCE_THRESHOLD=0.6
HIGH_DELAY_RATIO_THRESHOLD=0.2
HIGH_COST_OVERRUN_THRESHOLD=0.15
HIGH_STAFF_CHANGE_THRESHOLD=2
HIGH_MANAGER_CHANGE_THRESHOLD=1
```

| Biến | Mặc định | Ý nghĩa |
| ---- | -------- | ------- |
| `MODEL_DIR` | `models` | Thư mục lưu file `.joblib` (tương đối so với `QuanLyDuAnAIService`) |
| `DEFAULT_REASON_MODEL_ALIAS` | `reason_active.joblib` | Model nguyên nhân active |
| `MIN_REASON_*` | 30 / 2 / 5 | Ngưỡng tối thiểu để train model nguyên nhân |
| `ALLOW_ORIGINS` | localhost:5000,5001 | CORS (MVC gọi server-side nên ít ảnh hưởng trực tiếp) |

### 8.3. Khởi chạy FastAPI

Cách chính thức theo source (`run.py`):

```powershell
cd <TEN_THU_MUC_DU_AN>\QuanLyDuAnAIService
.\.venv\Scripts\Activate.ps1
python run.py
```

`run.py` gọi:

```python
uvicorn.run("app.main:app", host="0.0.0.0", port=8001, reload=True)
```

Tương đương:

```powershell
python -m uvicorn app.main:app --host 0.0.0.0 --port 8001 --reload
```

| Mục | Giá trị |
| --- | ------- |
| Module app | `app.main:app` |
| Host | `0.0.0.0` (lắng nghe mọi interface) |
| Port | **8001** |
| Swagger | `http://127.0.0.1:8001/docs` |
| Health check | `GET http://127.0.0.1:8001/health` |

### 8.4. Model AI có sẵn

Repository hiện có sẵn trong `QuanLyDuAnAIService/models/`:

- `reason_active.joblib` (+ `reason_active.metadata.json`) — alias model đang active
- `decision_tree_reason_20260528_061957.joblib`
- `decision_tree_reason_20260628_140040.joblib`

Khi startup, FastAPI nạp model nguyên nhân (`NguyenNhan`) qua `model_service.startup_load_default_models()`.

**Nếu chưa có model:** service vẫn chạy; health trả về `loadedReasonModel` có thể null; phân tích sẽ fallback theo rule hoặc báo chưa sẵn sàng cho đến khi train và set active.

### 8.5. Kiểm tra health

```powershell
Invoke-RestMethod -Uri "http://127.0.0.1:8001/health" -Method Get
```

Hoặc mở trình duyệt: `http://127.0.0.1:8001/health`

### 8.6. Thoát môi trường ảo

```powershell
deactivate
```

---

## 9. Cấu hình MVC kết nối FastAPI

Key cấu hình trong `appsettings.json` và `appsettings.Development.json`:

```json
"AiApi": {
  "BaseUrl": "http://127.0.0.1:8001",
  "TimeoutSeconds": 10,
  "RetryCount": 1,
  "RetryDelayMilliseconds": 300
}
```

| Key | Giá trị mặc định | Ý nghĩa |
| --- | ----------------- | ------- |
| `BaseUrl` | `http://127.0.0.1:8001` | URL gốc FastAPI — **phải trùng port FastAPI đang chạy** |
| `TimeoutSeconds` | 10 | Timeout HTTP client |
| `RetryCount` | 1 | Số lần thử lại |
| `RetryDelayMilliseconds` | 300 | Delay giữa các lần thử |

MVC gọi các endpoint (qua `AiApiService.cs`), ví dụ:

| Chức năng | Endpoint FastAPI |
| --------- | ---------------- |
| Health | `GET /health` |
| Validate dataset | `POST /dataset/validate` |
| Train model | `POST /model/train` |
| Danh sách model | `GET /model/list` |
| Phân tích nguyên nhân trễ | `POST /analyze/delay-reason` |

**Quan trọng:** nếu `AiApi:BaseUrl` không trùng địa chỉ/port FastAPI thực tế, màn hình AI sẽ báo không kết nối được dịch vụ AI.

Khi MVC và FastAPI cùng chạy trên một máy, giữ `http://127.0.0.1:8001` là phù hợp.

---

## 10. Thứ tự chạy toàn bộ hệ thống

```
1. Khởi động SQL Server
2. Kiểm tra database đã có schema + dữ liệu cần thiết
3. Terminal 1: FastAPI (giữ mở)
4. Kiểm tra GET /health
5. Terminal 2: ASP.NET MVC (giữ mở)
6. Mở http://localhost:5037
7. Đăng nhập
8. Kiểm tra module AI (Dashboard AI / health / train / phân tích)
```

Chi tiết từng bước:

### Bước 1–2: SQL Server

Đảm bảo service SQL chạy và database đã restore từ `quanlyduan.sql` (hoặc tương đương).

### Bước 3–4: Terminal 1 — FastAPI

```powershell
cd <TEN_THU_MUC_DU_AN>\QuanLyDuAnAIService
.\.venv\Scripts\Activate.ps1
python run.py
```

Kiểm tra:

```powershell
Invoke-RestMethod http://127.0.0.1:8001/health
```

### Bước 5–8: Terminal 2 — MVC

```powershell
cd <TEN_THU_MUC_DU_AN>\QuanLyDuAn\QuanLyDuAn
dotnet run --launch-profile http
```

Mở trình duyệt → đăng nhập → vào menu AI (cần quyền tương ứng).

**Giữ cả hai terminal mở** trong suốt phiên làm việc.

---

## 11. Tài khoản đăng nhập mặc định

### 11.1. Khi database mới, chưa có user `admin`

`KhoiTaoTaiKhoanMacDinh.cs` tạo tài khoản nếu chưa tồn tại user có `NormalizedUserName == "ADMIN"`:

| Vai trò | Tài khoản | Mật khẩu ban đầu | Ghi chú |
| ------- | --------- | ---------------- | ------- |
| Admin | `admin` | `111111` | Khai báo rõ trong source (`HashPassword(user, "111111")`) |

Runtime **không** tự tạo sẵn tài khoản Manager/Employee mặc định — chỉ tạo role và gán claim tối thiểu.

### 11.2. Khi đã chạy `quanlyduan.sql`

Script chứa nhiều user mẫu (`admin`, `manager`, `employee`, …) với `PasswordHash` đã mã hóa. Source **không** khai báo plaintext cho các user này.

| Tình huống | Hướng xử lý |
| ---------- | ----------- |
| Biết mật khẩu từ người cung cấp source | Dùng tài khoản tương ứng |
| Không biết mật khẩu | Đăng nhập `admin` nếu hash khớp mật khẩu demo; hoặc reset mật khẩu qua chức năng Quên mật khẩu (cần email); hoặc tạo DB mới không import user cũ để runtime tạo `admin/111111` |

### 11.3. Cảnh báo bảo mật

**Bắt buộc đổi mật khẩu mặc định** khi triển khai ngoài môi trường học tập/demo.

---

## 12. Kiểm tra sau khi cài đặt

Checklist:

- [ ] `dotnet build` thành công cho `QuanLyDuAn.csproj`
- [ ] FastAPI import `app.main:app` không lỗi
- [ ] MVC khởi động không lỗi validation `AccountActivation:AppBaseUrl`
- [ ] Kết nối SQL Server thành công (startup không exception DB)
- [ ] Trang `/Account/Login` hiển thị
- [ ] Đăng nhập được bằng tài khoản hợp lệ
- [ ] Dashboard tải dữ liệu thống kê
- [ ] `GET http://127.0.0.1:8001/health` trả về `serviceStatus: healthy`
- [ ] MVC gọi được FastAPI (màn AI Dashboard / kiểm tra kết nối)
- [ ] Danh sách model AI hiển thị (`/model/list` hoặc giao diện quản trị AI)
- [ ] Có model active `reason_active.joblib` **hoặc** hệ thống hiển thị đúng trạng thái chưa có model
- [ ] Chức năng **Tổng hợp AI Dataset** chạy (quyền `AI.Dataset`, thường Admin)
- [ ] Chức năng **Phân tích nguyên nhân** chỉ chạy với dự án đủ điều kiện trễ (`LaDuAnTre = 1`)
- [ ] Upload file dự án / tiến độ / avatar tạo file dưới `wwwroot/uploads/...`

---

## 13. Kiểm tra module AI

Luồng hiện tại (reason-only):

1. MVC tổng hợp dữ liệu nghiệp vụ vào **`AI_DATASET`** (`AiDatasetService`).
2. Chỉ dự án **trễ** (`LaDuAnTre = 1`) và có nguyên nhân được **Manager xác nhận** (`AI_NGUYEN_NHAN`) mới tạo nhãn huấn luyện đáng tin cậy (đồng bộ `MaDMNguyenNhan` vào dataset).
3. MVC gửi dataset hợp lệ sang FastAPI **`POST /model/train`**.
4. FastAPI huấn luyện Decision Tree, lưu file `.joblib` cục bộ trong `QuanLyDuAnAIService/models/`.
5. MVC gửi feature dự án trễ sang FastAPI **`POST /analyze/delay-reason`**.
6. Kết quả gợi ý lưu vào **`AI_KET_QUA`**, hiển thị trên UI.
7. Manager **xác nhận** nguyên nhân cuối vào **`AI_NGUYEN_NHAN`**.

Điểm cần nhớ khi kiểm tra:

| Nội dung | Chi tiết |
| -------- | -------- |
| Bài toán AI | Phân tích **nguyên nhân trễ**, không phải dự đoán dự án có trễ hay không |
| Model chính | Loại **`NguyenNhan`**, alias active **`reason_active.joblib`** |
| Ngưỡng train | Tối thiểu 30 dòng, 2 lớp nguyên nhân, 5 dòng/lớp (MVC + FastAPI) |
| Không dùng | `AI_KET_QUA` làm nhãn huấn luyện |
| FastAPI | Không ghi trực tiếp SQL Server |

Chi tiết 22 feature và rule nghiệp vụ: xem `docs/ai-he-thong-phan-tich.md` và `docs/mvc-ai-integration-rules.md`.

---

## 14. Lỗi thường gặp và cách xử lý

| Lỗi | Nguyên nhân có thể | Cách kiểm tra | Cách xử lý |
| --- | ------------------ | ------------- | ---------- |
| `'dotnet' is not recognized` | Chưa cài .NET SDK | `dotnet --info` | Cài .NET SDK 8+ và mở lại terminal |
| Sai phiên bản SDK | Thiếu SDK 8 cho `net8.0` | `dotnet --list-sdks` | Cài SDK 8.0.x (SDK 9 vẫn build được `net8.0` nếu có) |
| NuGet restore lỗi | Mạng/proxy/nuget.org | `dotnet restore` | Kiểm tra Internet, proxy, chạy lại restore |
| Không kết nối SQL Server | Service tắt, sai instance | SSMS, `Get-Service *SQL*` | Bật SQL Server, sửa `Data Source` |
| Sai tên server/instance | Copy connection string máy khác | SSMS Connect dialog | Dùng đúng `.\SQLEXPRESS` hoặc instance thực tế |
| Login failed for user | Sai authentication | SSMS với cùng login | Dùng Windows Auth hoặc sửa User/Password |
| Lỗi chứng chỉ SQL | SSL local | Thông báo chi tiết connection | Thêm `Trust Server Certificate=True` |
| Database chưa tồn tại | Chưa CREATE DATABASE | SSMS → Databases | Tạo DB trước khi chạy script |
| Thiếu bảng | Chưa chạy `quanlyduan.sql` | `SELECT * FROM sys.tables` | Chạy script schema chính |
| Chạy sai script / sai DB | `USE` khác connection string | So sánh tên DB | Chọn đúng DB, đồng bộ tên với appsettings |
| Cổng MVC bị chiếm | Port 5037 đang dùng | `netstat -ano \| findstr :5037` | Dừng process chiếm cổng hoặc đổi port trong `launchSettings.json` |
| Cổng FastAPI bị chiếm | Port 8001 đang dùng | `netstat -ano \| findstr :8001` | Dừng process hoặc đổi port trong `run.py` **và** `AiApi:BaseUrl` |
| Không Activate venv | ExecutionPolicy | PowerShell báo lỗi script | `Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass` |
| Module `app` not found | Chạy sai thư mục | `pwd` | `cd QuanLyDuAnAIService` rồi chạy lại |
| Thiếu thư viện Python | Chưa pip install | `pip list` | `pip install -r requirements.txt` trong venv |
| Không load model | Thiếu file `.joblib` | Liệt kê `models/` | Train model hoặc copy model có sẵn; kiểm tra `DEFAULT_REASON_MODEL_ALIAS` |
| Chưa có model active | Chưa train/set active | `GET /health` | Train qua UI AI hoặc API `/model/train` + `/admin/model/set-active` |
| MVC không gọi được FastAPI | FastAPI chưa chạy / sai URL | `Invoke-RestMethod http://127.0.0.1:8001/health` | Khởi động FastAPI; sửa `AiApi:BaseUrl` |
| Sai Base URL AI | Port/host không khớp | So sánh `run.py` và appsettings | Đồng bộ port 8001 |
| Health lỗi 500 | Import/model lỗi | Log terminal FastAPI | Xem traceback; cài lại dependency |
| Lỗi quyền ghi model/upload | Quyền NTFS | Ghi thử vào `models/`, `wwwroot/uploads/` | Cấp quyền ghi cho user chạy app |
| Startup MVC lỗi AppBaseUrl | Dùng localhost | Đọc exception startup | Đặt `AccountActivation:AppBaseUrl` = `http://<IP_LAN>:5037` |
| Tài khoản mặc định không vào được | DB import user hash khác | Query `AspNetUsers` | Dùng mục 11.2 để reset/tạo lại |
| Font tiếng Việt lỗi | Encoding file/SQL/browser | Kiểm tra file UTF-8 | Lưu file UTF-8; SQL dùng prefix `N'...'` |

---

## 15. Cách dừng hệ thống

1. Terminal MVC: nhấn **`Ctrl + C`**
2. Terminal FastAPI: nhấn **`Ctrl + C`**
3. Thoát venv Python:

```powershell
deactivate
```

4. **Không cần** dừng SQL Server nếu máy còn dùng cho việc khác.

---

## 16. Chạy lại trong những lần sau

**Terminal FastAPI:**

```powershell
cd <TEN_THU_MUC_DU_AN>\QuanLyDuAnAIService
.\.venv\Scripts\Activate.ps1
python run.py
```

**Terminal MVC:**

```powershell
cd <TEN_THU_MUC_DU_AN>\QuanLyDuAn\QuanLyDuAn
dotnet run --launch-profile http
```

Đảm bảo SQL Server đang chạy trước khi mở MVC.

---

## 17. Triển khai trong mạng LAN

Source **đã hỗ trợ** lắng nghe LAN một phần:

| Thành phần | Bind | Port |
| ---------- | ---- | ---- |
| MVC (`launchSettings.json`, profile `http`) | `0.0.0.0` | **5037** |
| FastAPI (`run.py`) | `0.0.0.0` | **8001** |

### Cấu hình cần chỉnh

1. **`AccountActivation:AppBaseUrl`**: URL mà máy khác truy cập MVC, ví dụ `http://192.168.x.x:5037` (không dùng localhost).
2. **`AiApi:BaseUrl`**: nếu MVC và FastAPI cùng máy chủ, giữ `http://127.0.0.1:8001`. MVC gọi FastAPI từ server, không từ trình duyệt client.
3. **Windows Firewall**: mở inbound TCP **5037** (MVC) nếu máy khác cần truy cập web.

### Truy cập từ máy khác

- MVC: `http://<IP_MAY_CHU>:5037`
- Swagger AI (debug): `http://<IP_MAY_CHU>:8001/docs` — chỉ nên bật trong môi trường tin cậy

### Rủi ro

- Cấu hình hiện tại dùng **HTTP** thuần, không HTTPS.
- Không có reverse proxy, hardening production hay bảo vệ secret đầy đủ.
- **Không** coi cấu hình mặc định là sẵn sàng Internet/public.

---

## 18. Lưu ý triển khai production

Tách khỏi hướng dẫn demo local:

- Không dùng mật khẩu mặc định (`admin/111111`).
- Không lưu secret (SQL password, Gmail App Password, connection string) trong Git.
- Không dùng Uvicorn `--reload` (tắt reload trong `run.py` hoặc dùng process manager).
- Không dùng tài khoản SQL quyền sysadmin cho ứng dụng.
- Bắt buộc **HTTPS** (reverse proxy: IIS, Nginx, Caddy, …).
- Sao lưu database định kỳ.
- Phân quyền ghi thư mục `wwwroot/uploads` và `QuanLyDuAnAIService/models`.
- Cấu hình logging tập trung (MVC + FastAPI `logs/ai_service.log`).
- Giới hạn kích thước upload ở reverse proxy/Kestrel nếu triển khai thực tế.
- Dự án hiện tại **chưa được khẳng định production-ready** chỉ vì chạy được trên môi trường dev/LAN.

---

## Trạng thái xác minh

Các kiểm tra đã thực hiện trên môi trường có source (không ghi dữ liệu DB):

| Kiểm tra | Kết quả |
| -------- | ------- |
| `dotnet restore` tại `QuanLyDuAn/QuanLyDuAn` | Thành công |
| `dotnet build` tại `QuanLyDuAn/QuanLyDuAn` | Thành công (2 warning CS1998 ở `FileTienDoCongViecService.cs`, không chặn build) |
| Import `from app.main import app` | Thành công |
| Endpoint `/health` trong source | Có tại `app/routers/health_router.py` |
| Lệnh Uvicorn/`run.py` | Trỏ đúng `app.main:app`, port **8001** |
| `requirements.txt` | Hợp lệ, 8 package |

**Chưa xác minh bằng chạy thực tế** (phụ thuộc môi trường người đọc):

- Kết nối SQL Server với connection string mẫu (`QuanLyDuAnAI6` trên instance tác giả).
- Đăng nhập UI end-to-end.
- Gọi `/health` khi Uvicorn đang chạy (chưa khởi chạy server trong phiên viết tài liệu).
- Train/phân tích AI với dữ liệu thật.

### Cấu hình nhạy cảm cần người cài tự thay

| Vị trí | Nội dung cần thay |
| ------ | ----------------- |
| `appsettings.json` → `ConnectionStrings:DefaultConnection` | Server/instance và tên database máy bạn |
| `appsettings.json` → `AccountActivation:AppBaseUrl` | IP/URL LAN thực tế (không localhost) |
| `appsettings.json` → `EmailSettings` | Email và App Password SMTP |
| `appsettings.json` → `AiApi:BaseUrl` | Chỉ khi đổi port/host FastAPI |
| `QuanLyDuAnAIService/.env` | Tùy chọn override biến môi trường AI |

File mẫu hiện tại chứa giá trị gắn máy tác giả (`LAPTOP-SI5JBDIU\SQLEXPRESS01`, `QuanLyDuAnAI6`, `192.168.2.27`) — **không** dùng nguyên xi trên máy mới.

---

*Tài liệu tham khảo nội bộ: `docs/readme.md`, `docs/ai-he-thong-phan-tich.md`, `docs/mvc-ai-integration-rules.md`, `docs/workflow-he-thong.md`, `QuanLyDuAnAIService/HUONG_DAN_AI_FASTAPI.md`. Khi tài liệu cũ và source khác nhau, lấy source hiện tại làm căn cứ.*
