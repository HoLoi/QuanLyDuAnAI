# QUAN LY DU AN AI - TAI LIEU NGHIEP VU CHINH (AS-IS THEO SOURCE)

README này được cập nhật theo source code thực tế của dự án `ASP.NET MVC + EF Core + SQL Server + FastAPI AI` tại thời điểm hiện tại.

Mục tiêu:
- Làm nguồn sự thật nghiệp vụ để Copilot/Codex đọc vào và sinh code đúng style dự án.
- Bám đúng workflow, permission, AI flow, style xử lý hiện tại.
- Không áp kiến trúc trừu tượng ngoài source.

---

## I. PHAM VI SOURCE DA DOI CHIEU

### 1. Lõi hệ thống
- `QuanLyDuAn/QuanLyDuAn/Program.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/CauHinhDichVu.cs`
- `QuanLyDuAn/QuanLyDuAn/Data/QuanLyDuAnDbContext.cs`
- `QuanLyDuAn/QuanLyDuAn/Data/KhoiTaoTaiKhoanMacDinh.cs`
- `QuanLyDuAn/QuanLyDuAn/Constants/TrangThai.cs`
- `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`

### 2. Service/permission/workflow bắt buộc
- `AccountService.cs`
- `PermissionHelper.cs`
- `PhanQuyenService.cs`
- `DashboardService.cs`
- `DuAnService.cs`
- `CongViecService.cs`
- `ChiTietCongViecService.cs`
- `TienDoCongViecService.cs`
- `TrangThaiWorkflowService.cs`

### 3. AI integration
- MVC: `AiService.cs`, `AiDatasetService.cs`, `AiApiService.cs`, `AiController.cs`, `AiDatasetController.cs`
- FastAPI:
  - `QuanLyDuAnAIService/app/main.py`
  - `app/constants.py`, `app/config.py`, `app/schemas.py`
  - `app/services/model_service.py`, `prediction_service.py`, `validation_service.py`
  - `app/routers/health_router.py`, `model_router.py`, `prediction_router.py`, `admin_router.py`

### 4. SQL schema
- `quanlyduan.sql`

### 5. Controllers/Interfaces/ViewModels/Entities
- Toàn bộ `Controllers`, `Services/Interfaces`, `ViewModels`, `Models/Entities` đã được quét để đồng bộ naming/pattern/module scope.

---

## II. KIEN TRUC HE THONG (AS-IS)

### 1. Thành phần chính
- ASP.NET MVC (presentation + orchestration)
- Service Layer (nghiệp vụ chính)
- EF Core DbContext (truy cập dữ liệu trực tiếp)
- SQL Server (system-of-record)
- FastAPI AI service (compute-only)

### 2. Boundary
- MVC là nơi quyết định workflow, quyền, transaction, ghi DB.
- FastAPI chỉ train/validate/predict/model-local, không ghi DB nghiệp vụ.

### 3. Pattern đang dùng
- Controller gọi service, service xử lý business rule.
- Không dùng repository pattern riêng.
- Không tách microservice nghiệp vụ (ngoài AI compute service).

### 4. SignalR
- Trong source hiện tại chưa có cấu hình `AddSignalR`/`MapHub` và chưa có Hub class.
- Chat đang vận hành theo service + DB (`PHONG_CHAT`, `THANH_VIEN_PHONG_CHAT`, `TIN_NHAN`).

---

## III. DATABASE VA QUAN HE CHINH

### 1. Cụm bảng nghiệp vụ
- Dự án/công việc:
  - `DU_AN`
  - `DANH_MUC_CONG_VIEC`
  - `CONG_VIEC`
  - `CT_CONG_VIEC`
  - `TIEN_DO_CONG_VIEC`
- Ngân sách/chi phí:
  - `NGAN_SACH`
  - `CHI_PHI`
  - `DE_XUAT_NGAN_SACH`
- Đề xuất công việc:
  - `DE_XUAT_CONG_VIEC`
- Đánh giá:
  - `DANH_GIA_DU_AN`, `CT_DANH_GIA_DU_AN`
  - `DANH_GIA_NHAN_VIEN`, `CT_DANH_GIA_NHAN_VIEN`
- Chat:
  - `PHONG_CHAT`, `THANH_VIEN_PHONG_CHAT`, `TIN_NHAN`
- File:
  - `FILE_DU_AN`, `FILE_CONG_VIEC`, `FILE_CT_CONG_VIEC`, `FILE_TIEN_DO_CONG_VIEC`
- Phân công:
  - `PHAN_CONG_CONG_VIEC`, `PHAN_CONG_CT_CONG_VIEC`
- Nhật ký:
  - `NHAT_KY_QUAN_LY_DU_AN`, `NHAT_KY_NGAN_SACH`, `NHAT_KY_CHI_PHI`, ...

### 2. Cụm bảng AI
- `AI_DATASET`
- `AI_MODEL`
- `AI_KET_QUA`
- `AI_NGUYEN_NHAN`
- `DM_NGUYEN_NHAN`

### 3. Quan hệ nổi bật
- `AI_DATASET.MaDuAn -> DU_AN.MaDuAn`
- `AI_KET_QUA` FK tới `DU_AN`, `AI_DATASET`, `AI_MODEL`, `DM_NGUYEN_NHAN`
- `AI_NGUYEN_NHAN` FK tới `DU_AN`, `DM_NGUYEN_NHAN`
- Chuỗi workflow nghiệp vụ:
  - `DU_AN -> DANH_MUC_CONG_VIEC -> CONG_VIEC -> CT_CONG_VIEC -> TIEN_DO_CONG_VIEC`

---

## IV. TRANG THAI VA TERMINOLOGY CHUAN

Nguồn chuẩn: `Constants/TrangThai.cs`

### 1. Trạng thái dự án/công việc/chi tiết
- `KhoiTao`
- `DangThucHien`
- `BiCanCan`
- `ChoXacNhanHoanThanh`
- `HoanThanh`
- `TamDung`
- `DaHuy`
- `LuuTru` (code hiện tại: `Archived`)

### 2. Trạng thái duyệt
- `ChoDuyet`
- `DaDuyet`
- `YeuCauBoSung`
- `TuChoi`
- `Nhap` (đánh giá)

### 3. Trạng thái ngân sách phụ trợ
- `DaThayThe`
- `GiuNguyen`

### 4. Rule chuẩn hóa
- Dùng `TrangThai.Normalize`, `ToCode`, `ToDisplay`, `EqualsValue`, `GetCommonStatusVariants`.
- Không so sánh raw string trực tiếp nếu có thể dùng helper.

---

## V. PERMISSION LAYER (ROLE + CLAIM + SCOPE)

### 1. Nguồn permission
- `Constants/Permissions.cs` là danh mục permission chuẩn.
- Ví dụ AI permission:
  - `AI.Dataset`
  - `AI.Train`
  - `AI.PhanTichNguyenNhan`
  - `AI.Dashboard`
  - `AI.XacNhan`
  - `AI.DuDoan` chỉ giữ alias tương thích, không dùng làm quyền chính

### 2. Luồng check permission
- Controller gọi `IPermissionHelper.HasPermissionAsync`.
- `PermissionHelper` đọc tập permission được cấp từ `PhanQuyenService.GetGrantedPermissionNamesAsync`.
- `PhanQuyenService` đọc claim có type chứa `permission`/`claim`/`quyen`.

### 3. Role mặc định
- Runtime seed qua `KhoiTaoTaiKhoanMacDinh`:
  - `Admin`
  - `Manager`
  - `Employee`
- Gắn claim quyền tối thiểu cho từng role.

### 4. Scope rule quan trọng
- Không chỉ dựa vào role global.
- Nhiều service chặn thêm theo phạm vi dữ liệu:
  - manager dự án
  - leader dự án
  - team leader
  - thành viên được phân công

---

## VI. WORKFLOW TONG THEO MODULE (AS-IS)

## 1. DU AN

### 1.1 Luồng chính
- `KhoiTao -> DangThucHien -> ChoXacNhanHoanThanh -> HoanThanh`
- Có `TamDung` và `MoLaiDuAn`.

### 1.2 Input -> Validate -> Xử lý -> Transaction -> Ghi DB -> Output
- Input:
  - thao tác từ `DuAnController` (save, chuyển trạng thái, hoàn thành, mở lại, tạm dừng).
- Validate:
  - `CheckManagerPermissionAsync`
  - `ValidateCompletionAsync` (đủ công việc hoàn thành, không blocked, có ngân sách).
- Xử lý:
  - đổi `TrangThaiDuAn`, set ghi chú/lý do khi cần.
  - auto start bằng `CheckAutoTransitionAsync` khi đủ điều kiện.
- Transaction:
  - `SaveAsync` dùng transaction khi lưu/cập nhật dự án và hậu xử lý liên quan.
- Ghi DB:
  - `SaveChangesAsync` theo từng bước logic.
- Output:
  - trạng thái dự án mới + trạng thái check UI (`ProjectStatusCheckViewModel`).

### 1.3 Auto rule
- Auto `KhoiTao -> DangThucHien` nếu có thành viên + danh mục + công việc.

---

## 2. CONG VIEC

### 2.1 Luồng trạng thái
- Auto sync từ chi tiết qua `TrangThaiWorkflowService`.
- Xác nhận hoàn thành: `ChoXacNhanHoanThanh -> HoanThanh`.
- Mở lại: `HoanThanh -> DangThucHien`.

### 2.2 Input -> Validate -> Xử lý -> Transaction -> Ghi DB -> Output
- Input:
  - từ `CongViecController` (xem, xác nhận, mở lại).
- Validate:
  - `KiemTraQuyenXuLyTrangThaiCongViecAsync`.
  - check trạng thái hợp lệ trước transition.
- Xử lý:
  - set trạng thái công việc.
  - ghi nhật ký mở lại.
  - sync trạng thái dự án liên quan.
- Transaction:
  - không mở transaction riêng lớn, dùng save theo nhịp nghiệp vụ.
- Ghi DB:
  - `SaveChangesAsync` trước và sau sync.
- Output:
  - danh sách công việc với cờ workflow UI (`CoTheXacNhanHoanThanh`, `CoTheMoLai`, `CssTrangThai`, `ThongDiepWorkflow`).

---

## 3. CHI TIET CONG VIEC

### 3.1 Luồng chính
- CRUD chi tiết công việc.
- Mọi thay đổi gọi sync chuỗi trạng thái cha.

### 3.2 Input -> Validate -> Xử lý -> Transaction -> Ghi DB -> Output
- Input:
  - `AddAsync`, `UpdateAsync`, `RemoveAsync`.
- Validate:
  - dữ liệu form, ngày tháng, trạng thái hợp lệ.
  - quyền cập nhật theo scope dự án/team.
  - chặn thêm/sửa khi công việc/dự án ở trạng thái khóa.
- Xử lý:
  - thêm/sửa/xóa mềm chi tiết.
  - gọi `DongBoChuoiTrangThaiTuCongViecAsync`.
- Transaction:
  - không mở transaction tách riêng ở service này.
- Ghi DB:
  - save chi tiết, save sau sync.
- Output:
  - danh sách chi tiết + tóm tắt trạng thái.

---

## 4. TIEN DO CONG VIEC

### 4.1 Luồng chính
- Nhân sự gửi báo cáo: luôn vào `ChoDuyet`.
- Người có scope duyệt xử lý: `DaDuyet` / `YeuCauBoSung` / `TuChoi`.
- Chỉ khi `DaDuyet` mới cập nhật trạng thái thật của `CT_CONG_VIEC`.

### 4.2 Input -> Validate -> Xử lý -> Transaction -> Ghi DB -> Output
- Input:
  - `CapNhatTienDoAsync`, `DuyetBaoCaoTienDoAsync`, `YeuCauBoSungBaoCaoTienDoAsync`, `TuChoiBaoCaoTienDoAsync`.
- Validate:
  - quyền theo scope.
  - không cho Admin tác nghiệp tiến độ.
  - không lùi trạng thái (`KiemTraKhongDuocLuiTrangThai`).
  - khóa theo trạng thái dự án/công việc/chi tiết (`BiKhoaCapNhatTheoTrangThai`).
  - rule file minh chứng cho đề xuất hoàn thành.
- Xử lý:
  - tạo báo cáo hoặc duyệt báo cáo.
  - khi duyệt `DaDuyet`, cập nhật trạng thái thật chi tiết.
  - sync cha qua workflow service.
- Transaction:
  - có transaction rõ ràng ở cả gửi và duyệt báo cáo.
- Ghi DB:
  - save tiến độ, save file, save sync.
- Output:
  - lịch sử tiến độ + trạng thái duyệt + quyền hành động.

---

## 5. DE XUAT CONG VIEC

### 5.1 Luồng trạng thái
- Tạo đề xuất: `ChoDuyet`.
- Người tạo có thể hủy khi pending: `DaHuy`.
- Duyệt: `DaDuyet` (sinh `CONG_VIEC` + `CHI_PHI`).
- Từ chối: `TuChoi`.

### 5.2 Input -> Validate -> Xử lý -> Transaction -> Ghi DB -> Output
- Input:
  - tạo/hủy ở `DeXuatCongViecService`.
  - duyệt/từ chối ở `DyetDeXuatCongViecService`.
- Validate:
  - scope đề xuất theo leader/team/member rule.
  - ngân sách active phải đủ.
  - chống trùng đề xuất pending.
  - chỉ duyệt khi trạng thái pending.
- Xử lý:
  - tạo đề xuất hoặc chuyển trạng thái.
  - khi duyệt tạo công việc thật và bản ghi chi phí liên quan.
- Transaction:
  - duyệt dùng transaction `Serializable`.
- Ghi DB:
  - ghi đề xuất + công việc + chi phí + nhật ký.
- Output:
  - danh sách đề xuất theo dự án.

---

## 6. DE XUAT NGAN SACH + NGAN SACH

### 6.1 Luồng trạng thái
- Tạo đề xuất ngân sách: `ChoDuyet`.
- Hủy đề xuất pending: `DaHuy`.
- Duyệt:
  - ngân sách active cũ -> `DaThayThe`
  - tạo ngân sách version mới -> `DaDuyet`, `IsActive=true`
  - đề xuất -> `DaDuyet`
- Từ chối: `TuChoi`.

### 6.2 Input -> Validate -> Xử lý -> Transaction -> Ghi DB -> Output
- Input:
  - `DeXuatNganSachService` tạo/hủy.
  - `DuyetDeXuatNganSachService` duyệt/từ chối.
- Validate:
  - scope leader/team/member cho tạo đề xuất.
  - manager dự án mới được duyệt.
  - pending check để tránh duyệt lặp.
  - ngân sách đề xuất không nhỏ hơn chi phí đã dùng.
- Xử lý:
  - version hóa ngân sách.
  - cập nhật active flag.
- Transaction:
  - duyệt dùng `Serializable`.
- Ghi DB:
  - `NGAN_SACH`, `DE_XUAT_NGAN_SACH`, nhật ký ngân sách và nhật ký dự án.
- Output:
  - ngân sách active mới theo dự án.

---

## 7. DANH GIA DU AN

### 7.1 Luồng trạng thái
- `Nhap -> ChoDuyet -> DaDuyet / TuChoi`.
- Có flow xác nhận nguyên nhân AI tham khảo trong màn đánh giá.

### 7.2 Input -> Validate -> Xử lý -> Transaction -> Ghi DB -> Output
- Input:
  - lưu form, gửi duyệt, duyệt, từ chối.
- Validate:
  - quyền theo claim/scope.
  - ràng buộc theo trạng thái dự án.
- Xử lý:
  - cập nhật trạng thái đánh giá.
  - tổng hợp insight AI + timeline thực tế.
- Transaction:
  - theo đơn vị thao tác save logic.
- Ghi DB:
  - `DANH_GIA_DU_AN`, `CT_DANH_GIA_DU_AN`, dữ liệu xác nhận AI nếu thao tác.
- Output:
  - page đánh giá với insight AI, trạng thái review.

---

## 8. CHAT DU AN

### 8.1 Luồng chính
- Mỗi dự án có phòng chat nội bộ.
- Service tự đảm bảo phòng chat tồn tại và đồng bộ thành viên.
- Chặn gửi tin khi dự án `DaHuy` hoặc `LuuTru`.

### 8.2 Input -> Validate -> Xử lý -> Transaction -> Ghi DB -> Output
- Input:
  - xem phòng, xem tin, gửi tin.
- Validate:
  - quyền vào phòng theo scope dự án.
  - user phải là thành viên phòng hợp lệ.
  - giới hạn nội dung tin nhắn.
- Xử lý:
  - đồng bộ membership trước khi gửi.
  - ghi tin mới.
- Transaction:
  - chủ yếu theo thao tác DB đơn lẻ.
- Ghi DB:
  - `PHONG_CHAT`, `THANH_VIEN_PHONG_CHAT`, `TIN_NHAN`.
- Output:
  - danh sách phòng + lịch sử tin.

---

## 9. AI WORKFLOW

### 9.1 Bảng AI và vai trò
- `AI_DATASET`: snapshot 10 feature theo dự án để train/phân tích nguyên nhân.
- `LaDuAnTre`: cờ trễ do rule nghiệp vụ MVC quyết định (không phải output AI).
- `MaDMNguyenNhan`: nhãn train nguyên nhân được đồng bộ từ xác nhận manager.
- `AI_MODEL`: metadata model, `LoaiModel`, `IsActive`.
- `AI_KET_QUA`: kết quả AI gợi ý nguyên nhân trễ tại thời điểm phân tích.
- `AI_NGUYEN_NHAN`: xác nhận nguyên nhân thực tế từ manager.

### 9.2 Model runtime
- Chỉ còn model loại `NguyenNhan`.
- Active alias chuẩn: `reason_active.joblib`.
- Không còn pipeline dự đoán trễ hạn trong luồng chính.

### 9.3 Analyze flow thật
- `AiController.Predict` (màn "Phân tích nguyên nhân trễ") -> `AiService.DuDoanDuAnAsync` -> FastAPI `/analyze/delay-reason`.
- MVC chỉ gọi AI khi `LaDuAnTre = 1`.
- MVC luôn lấy feature từ `AI_DATASET` mới nhất của dự án, không dùng dữ liệu form UI làm nguồn chính.
- Nếu `AI_DATASET.NgayTongHop` cũ hơn dữ liệu nghiệp vụ mới nhất thì yêu cầu tổng hợp lại trước khi gọi AI.
- Nếu model nguyên nhân không đủ tin cậy/lỗi/chưa có: fallback rule.
- Hậu kiểm reason mâu thuẫn:
  - nếu AI trả `Vượt ngân sách` nhưng `ChiPhiThucTe <= ChiPhiDuKien` hoặc `ChenhLechChiPhi <= 0`, hệ thống reject kết quả này và chuyển fallback hợp lệ hơn.
- Lưu kết quả vào `AI_KET_QUA`.

### 9.3.1 Rule feature nhạy cảm
- `SoLanThayDoiNhanSu`: đếm từ `NHAT_KY_PHU_TRACH_DU_AN` sau khi lọc hành động thay đổi nhân sự/phụ trách hợp lệ.
- `SoLanThayDoiQuanLy`: đếm từ `YEU_CAU_DOI_QUAN_LY` đã duyệt thực sự (`DaDuyet/Đã duyệt`).
- `SoNgayTreTienDo`: phiên bản hiện tại lấy theo `max` ngày trễ công việc trong dự án.

### 9.4 Predict thử
- `AiController.TestPredict` chỉ test, không lưu `AI_KET_QUA`.
- Nếu dự án không trễ (`LaDuAnTre != 1`) thì không gọi FastAPI.

### 9.5 Train flow
- Train `NguyenNhan`:
  - chỉ lấy dòng `LaDuAnTre = 1`.
  - bắt buộc có `MaDMNguyenNhan`.
  - đủ 10 feature chuẩn.
- Ground truth train lấy từ `AI_NGUYEN_NHAN` (manager xác nhận), không lấy từ `AI_KET_QUA`.
- FastAPI chia train/test trong RAM bằng `train_test_split`, không tách bảng train/test và không tách CSV trong luồng chính.
- Gate mặc định:
  - `MIN_REASON_TRAIN_ROWS=30`
  - `MIN_REASON_CLASS_COUNT=2`
  - `MIN_REASON_ROWS_PER_CLASS=5`

### 9.6 Guardrail AI
- FastAPI schema strict (`extra=forbid`).
- MVC parser chi tiết lỗi 422 để chỉ rõ field mismatch.
- Manager confirm có scope + quyền + check `LaDuAnTre = 1` mới cho xác nhận.
- FastAPI không ghi `AI_NGUYEN_NHAN`.

### 9.7 Ngưỡng heuristic reason-only (đồng bộ MVC/FastAPI)
- `HighStaffChangeThreshold = 2`
- `HighManagerChangeThreshold = 1`
- `HighDelayRatioThreshold = 0.2`
- `HighCostOverrunThreshold = 0.15`
- `LongDelayDaysThreshold = 14`

---

## VII. DASHBOARD LOGIC (AS-IS)

Nguồn chính: `DashboardService.GetDashboardAsync`

### 1. Dữ liệu tổng hợp
- tổng dự án, tổng công việc, tổng nhân viên
- tổng ngân sách, tổng chi phí, ngân sách còn lại, tỷ lệ sử dụng ngân sách
- phân bố trạng thái dự án
- công việc trễ hạn
- nhân sự quá tải
- dự án vượt ngân sách
- đề xuất chờ duyệt
- số dự án thiếu `AI_DATASET`

### 2. Rule cảnh báo
- over-budget dựa trên `chiPhi > nganSachActive`.
- overdue task dựa trên deadline + trạng thái chưa hoàn thành.

### 3. AI suggestion ở dashboard
- mức tổng quan hiện tại là thống kê dữ liệu AI và độ phủ dataset.
- insight chi tiết theo từng dự án nằm ở module AI dashboard và đánh giá dự án.

---

## VIII. AUTO SYNC / AUTO CALCULATE / AUTO COMPLETE

### 1. Auto sync trạng thái
- `ITrangThaiWorkflowService` là đầu mối:
  - `DongBoTrangThaiCongViecTheoChiTietAsync`
  - `DongBoTrangThaiDuAnTheoCongViecAsync`
  - `DongBoChuoiTrangThaiTuCongViecAsync`

### 2. Auto calculate
- `DuAn.PhanTramHoanThanh` được tính theo trạng thái công việc trong workflow service.
- Dashboard tự tổng hợp KPI từ nhiều bảng.
- AI dataset tự tổng hợp từ dữ liệu nghiệp vụ qua `AiDatasetService`.

### 3. Auto complete
- Công việc auto lên `ChoXacNhanHoanThanh` khi toàn bộ chi tiết hoàn thành.
- Dự án auto lên `ChoXacNhanHoanThanh` khi toàn bộ công việc hoàn thành.
- Không auto chuyển thẳng `HoanThanh`; cần bước xác nhận thủ công.

### 4. Reopen
- `CongViecService.MoLaiCongViecAsync`.
- `DuAnService.MoLaiDuAnAsync`.

---

## IX. BUSINESS RULE CHINH

### 1. Soft delete
- Pattern chuẩn: lọc `IsDeleted != true` ở query.
- Xóa thực tế chủ yếu là update cờ xóa + metadata xóa.

### 2. Transaction rule
- Dùng transaction ở các flow đa bảng/đa bước quan trọng:
  - duyệt đề xuất công việc
  - duyệt đề xuất ngân sách
  - duyệt yêu cầu đổi quản lý
  - save dự án
  - tiến độ (gửi báo cáo + duyệt báo cáo)
  - một số flow nhân sự/team dự án

### 3. Idempotent theo trạng thái
- Nhiều thao tác kiểm tra trạng thái pending trước khi duyệt/hủy để tránh xử lý lặp.

### 4. Editable/readonly status
- Nhiều module khóa cập nhật khi thực thể ở `HoanThanh`, `TamDung`, `DaHuy`, `LuuTru`.
- Tiến độ có kiểm soát không cho lùi trạng thái.

### 5. Nhật ký nghiệp vụ
- Ghi nhật ký ở nhiều flow:
  - quản lý dự án
  - ngân sách
  - chi phí
  - phân công

---

## X. CODING CONVENTION (AS-IS)

### 1. Naming
- Service: `*Service` + interface `I*Service`.
- Controller: `*Controller`.
- ViewModel: theo module (`ViewModels/<Module>`).
- Entity: ánh xạ bảng SQL tương ứng.

### 2. Async naming
- Method bất đồng bộ đa phần kết thúc `Async`.

### 3. Service style
- Xử lý nghiệp vụ ở service, controller mỏng.
- Query trực tiếp qua DbContext (không repository layer riêng).

### 4. Validation style
- Validate sớm bằng `if (...) throw new Exception("...")`.
- Chặn quyền + chặn scope + chặn trạng thái + chặn dữ liệu đầu vào.

### 5. Exception style
- Throw `Exception` message nghiệp vụ tiếng Việt (một số chỗ không dấu hoặc lỗi mã hóa ký tự).

### 6. SaveChangesAsync style
- Gọi `SaveChangesAsync` theo nhịp xử lý.
- Với flow nhiều bước dùng transaction + commit/rollback rõ ràng.

### 7. IsDeleted filter
- Là quy tắc mặc định trong hầu hết service query.

---

## XI. LUU Y QUAN TRONG CHO COPILOT/CODEX

1. Không đổi terminology `TrangThai`, `Permissions`, tên bảng/entity hiện có.
2. Không tự thêm kiến trúc mới (repository/unit-of-work riêng) nếu không có yêu cầu.
3. Khi sửa workflow, luôn đối chiếu `TrangThaiWorkflowService` trước.
4. Khi sửa AI contract, phải đồng bộ cùng lúc:
- MVC ViewModel/DTO
- `AiApiService` request builder
- FastAPI `schemas.py`
- logic validate/train/predict
5. Khi viết query mới, mặc định xét `IsDeleted`.
6. Khi thêm thao tác duyệt/chuyển trạng thái, kiểm tra lại rule idempotent theo trạng thái hiện tại.

---

## XII. GHI CHU AS-IS

- README này phản ánh code thực tế hiện tại.
- Nếu source thay đổi workflow/permission/AI contract, cần cập nhật README ngay trong cùng đợt sửa code để tránh lệch tài liệu.
