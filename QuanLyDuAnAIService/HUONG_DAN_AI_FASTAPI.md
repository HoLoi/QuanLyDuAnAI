# HƯỚNG DẪN FASTAPI AI (Dual Model TreHan + NguyenNhan)

## 1. Vai trò hệ thống
- `QuanLyDuAn` (ASP.NET MVC): quản lý nghiệp vụ, quyền, workflow, ghi dữ liệu SQL Server.
- `QuanLyDuAnAIService` (FastAPI): compute-only cho validate/train/predict/model-local.
- FastAPI không ghi `AI_NGUYEN_NHAN`.

## 2. Chạy service
```powershell
cd e:\D\Luan Van\dvl9\QuanLyDuAnAI\QuanLyDuAnAIService
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
python run.py
```

Mặc định:
- Base URL: `http://localhost:8001`
- Swagger: `http://localhost:8001/docs`

