## 4.3.6 Ràng buộc & Index

Phần này trình bày cấu trúc các bảng dữ liệu, khóa chính, khóa ngoại và các index chính được xác định trực tiếp từ file `quanlyduan.sql`.

### Bảng: __EFMigrationsHistory

Mô tả:
Lưu lịch sử migration của Entity Framework trong cơ sở dữ liệu.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MigrationId | NVARCHAR(150) | PK | Mã migration |
| 2 | ProductVersion | NVARCHAR(32) |  | Phiên bản EF Core |

### Bảng: AI_DATASET

Mô tả:
Lưu dữ liệu đặc trưng theo từng dự án để phục vụ huấn luyện và dự đoán AI về trễ tiến độ.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaData | INT | PK | Mã dữ liệu AI |
| 2 | MaDuAn | INT | FK | Mã dự án |
| 3 | SoNhanVienDuAn | INT |  | Số nhan vien du an |
| 4 | TongSoCongViec | INT |  | Tổng so cong viec |
| 5 | SoCongViecTre | INT |  | Số cong viec tre |
| 6 | TyLeCongViecTre | FLOAT |  | Tỷ lệ cong viec tre |
| 7 | ChiPhiDuKien | DECIMAL(18, 2) |  | Chi phí du kien |
| 8 | ChiPhiThucTe | DECIMAL(18, 2) |  | Chi phí thuc te |
| 9 | ChenhLechChiPhi | DECIMAL(18, 2) |  | Chenh lech chi phi |
| 10 | SoLanThayDoiNhanSu | INT |  | Số lan thay doi nhan su |
| 11 | SoLanThayDoiQuanLy | INT |  | Số lan thay doi quan ly |
| 12 | SoNgayTreTienDo | INT |  | Số ngay tre tien do |
| 13 | IsTre | BIT |  | Nhãn trễ tiến độ |

### Bảng: AI_KET_QUA

Mô tả:
Lưu kết quả AI dự đoán nguyên nhân trễ tiến độ của dự án.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaAiKetQua | INT | PK | Mã ai ket qua |
| 2 | MaDMNguyenNhan | INT | FK | Mã d m nguyen nhan |
| 3 | MaModel | INT | FK | Mã model |
| 4 | MaData | INT | FK | Mã dữ liệu AI |
| 5 | MaDuAn | INT |  | Mã dự án |
| 6 | DoTinCayKetQua | FLOAT |  | Độ tin cậy kết quả AI |
| 7 | ThoiGianDuDoanKetQua | DATETIME2(7) |  | Thời gian du doan ket qua |

### Bảng: AI_MODEL

Mô tả:
Lưu thông tin metadata của các mô hình AI được huấn luyện và trạng thái kích hoạt.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaModel | INT | PK | Mã model |
| 2 | TenModel | NVARCHAR(255) |  | Tên model AI |
| 3 | SoLuongDuLieu | INT |  | Số luong du lieu |
| 4 | DoChinhXac | FLOAT |  | Do chinh xac |
| 5 | NgayTao | DATETIME2(7) |  | Ngày tao |
| 6 | MoTaModel | NVARCHAR(255) |  | Mô tả model |
| 7 | LoaiModel | NVARCHAR(50) |  | Loại model AI |
| 8 | IsActive | BIT |  | Trạng thái đang kích hoạt |
| 9 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 10 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 11 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: AI_NGUYEN_NHAN

Mô tả:
Lưu nguyên nhân trễ tiến độ được xác nhận để phục vụ phân tích và tái huấn luyện AI.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaAINguyenNhan | INT | PK | Mã a i nguyen nhan |
| 2 | MaDuAn | INT | FK | Mã dự án |
| 3 | MaDMNguyenNhan | INT |  | Mã d m nguyen nhan |
| 4 | DoTinCay | FLOAT |  | Độ tin cậy |
| 5 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 6 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 7 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: AspNetRoleClaims

Mô tả:
Lưu dữ liệu Identity phục vụ xác thực và phân quyền người dùng.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | Id | INT | PK | Mã định danh tài khoản/vai trò |
| 2 | Asp_Id | NVARCHAR(128) | FK | Mã tài khoản/vai trò Identity |
| 3 | MaDanhMucQuyen | INT | FK | Mã danh muc quyen |
| 4 | ClaimType | NVARCHAR(MAX) |  | Loại claim |
| 5 | ClaimValue | NVARCHAR(MAX) |  | Giá trị claim |

### Bảng: AspNetRoles

Mô tả:
Lưu dữ liệu Identity phục vụ xác thực và phân quyền người dùng.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | Id | NVARCHAR(128) | PK | Mã định danh tài khoản/vai trò |
| 2 | Name | NVARCHAR(256) |  | Tên |
| 3 | NormalizedName | NVARCHAR(256) |  | Tên chuẩn hóa |
| 4 | ConcurrencyStamp | NVARCHAR(MAX) |  | Mã kiểm soát đồng thời |

### Bảng: AspNetUserClaims

Mô tả:
Lưu dữ liệu Identity phục vụ xác thực và phân quyền người dùng.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | Id | INT | PK, FK | Mã định danh tài khoản/vai trò |
| 2 | Asp_Id | NVARCHAR(128) |  | Mã tài khoản/vai trò Identity |
| 3 | ClaimType | NVARCHAR(MAX) |  | Loại claim |
| 4 | ClaimValue | NVARCHAR(MAX) |  | Giá trị claim |

### Bảng: AspNetUserLogins

Mô tả:
Lưu dữ liệu Identity phục vụ xác thực và phân quyền người dùng.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | LoginProvider | NVARCHAR(128) | PK | Login provider |
| 2 | ProviderKey | NVARCHAR(128) | PK | Provider key |
| 3 | Id | NVARCHAR(128) | FK | Mã định danh tài khoản/vai trò |
| 4 | ProviderDisplayName | NVARCHAR(MAX) |  | Provider display name |

### Bảng: AspNetUserRoles

Mô tả:
Lưu dữ liệu Identity phục vụ xác thực và phân quyền người dùng.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | Asp_Id | NVARCHAR(128) | PK, FK | Mã tài khoản/vai trò Identity |
| 2 | Id | NVARCHAR(128) | PK | Mã định danh tài khoản/vai trò |

### Bảng: AspNetUsers

Mô tả:
Lưu dữ liệu Identity phục vụ xác thực và phân quyền người dùng.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | Id | NVARCHAR(128) | PK, FK | Mã định danh tài khoản/vai trò |
| 2 | MaNguoiDung | INT |  | Mã người dùng |
| 3 | UserName | NVARCHAR(256) |  | User name |
| 4 | NormalizedUserName | NVARCHAR(256) |  | Normalized user name |
| 5 | Email | NVARCHAR(256) |  | Email |
| 6 | NormalizedEmail | NVARCHAR(256) |  | Normalized email |
| 7 | EmailConfirmed | BIT |  | Email confirmed |
| 8 | PasswordHash | NVARCHAR(MAX) |  | Password hash |
| 9 | SecurityStamp | NVARCHAR(MAX) |  | Security stamp |
| 10 | ConcurrencyStamp | NVARCHAR(MAX) |  | Mã kiểm soát đồng thời |
| 11 | PhoneNumber | NVARCHAR(MAX) |  | Phone number |
| 12 | PhoneNumberConfirmed | BIT |  | Phone number confirmed |
| 13 | TwoFactorEnabled | BIT |  | Two factor enabled |
| 14 | LockoutEnd | DATETIME2(7) |  | Lockout end |
| 15 | LockoutEnabled | BIT |  | Lockout enabled |
| 16 | AccessFailedCount | INT |  | Access failed count |

### Bảng: AspNetUserTokens

Mô tả:
Lưu dữ liệu Identity phục vụ xác thực và phân quyền người dùng.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | Id | NVARCHAR(128) | PK | Mã định danh tài khoản/vai trò |
| 2 | LoginProvider | NVARCHAR(128) | PK | Login provider |
| 3 | Name | NVARCHAR(128) | PK | Tên |
| 4 | Value | NVARCHAR(MAX) |  | Value |

### Bảng: CONG_VIEC

Mô tả:
Lưu thông tin cong viec trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaCongViec | INT | PK, FK | Mã cong viec |
| 2 | MaDeXuatCV | INT | FK | Mã de xuat c v |
| 3 | MaDanhMucCV | INT |  | Mã danh muc c v |
| 4 | MaMucDo | INT | FK | Mã muc do |
| 5 | TenCongViec | NVARCHAR(255) |  | Tên cong viec |
| 6 | MoTaCongViec | NVARCHAR(MAX) |  | Mô tả cong viec |
| 7 | NgayBatDauCongViec | DATETIME2(7) |  | Ngày bat dau cong viec |
| 8 | NgayKetThucCVDuKien | DATETIME2(7) |  | Ngày ket thuc c v du kien |
| 9 | NgayKetThucCVThucTe | DATETIME2(7) |  | Ngày ket thuc c v thuc te |
| 10 | NgayTaoCongViec | DATETIME2(7) |  | Ngày tao cong viec |
| 11 | TrangThaiCongViec | NVARCHAR(50) |  | Trạng thái cong viec |
| 12 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 13 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 14 | DeletedBy | INT | FK | Người thực hiện xóa mềm |

### Bảng: CT_CONG_VIEC

Mô tả:
Lưu thông tin ct cong viec trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaChiTietCV | INT | PK | Mã chi tiet c v |
| 2 | MaCongViec | INT |  | Mã cong viec |
| 3 | TenCTCV | NVARCHAR(MAX) |  | Tên c t c v |
| 4 | NoiDungChiTietCV | NVARCHAR(MAX) |  | Noi dung chi tiet c v |
| 5 | NgayTaoCTCV | DATETIME2(7) |  | Ngày tao c t c v |
| 6 | NgayBatDauCTCV | DATETIME2(7) |  | Ngày bat dau c t c v |
| 7 | NgayKetThucCTCV | DATETIME2(7) |  | Ngày ket thuc c t c v |
| 8 | TrangThaiCTCV | NVARCHAR(50) |  | Trạng thái c t c v |
| 9 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 10 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 11 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: CT_DANH_GIA_DU_AN

Mô tả:
Lưu thông tin ct danh gia du an trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaChiTietDGDA | INT | PK | Mã chi tiet d g d a |
| 2 | MaDanhGiaDuAn | INT |  | Mã danh gia du an |
| 3 | NhanXetDuAn | NVARCHAR(MAX) |  | Nhan xet du an |
| 4 | MaTieuChi | INT | FK | Mã tieu chi |
| 5 | DiemDanhGiaDA | INT |  | Điểm danh gia d a |
| 6 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 7 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 8 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: CT_DANH_GIA_NHAN_VIEN

Mô tả:
Lưu thông tin ct danh gia nhan vien trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaChiTietDGNV | INT | PK | Mã chi tiet d g n v |
| 2 | MaDanhGiaNhanVien | INT | FK | Mã danh gia nhan vien |
| 3 | MaTieuChi | INT | FK | Mã tieu chi |
| 4 | MaCongViec | INT | FK | Mã cong viec |
| 5 | NoiDungDanhGiaNhanVien | NVARCHAR(MAX) |  | Noi dung danh gia nhan vien |
| 6 | DiemDanhGiaNV | INT |  | Điểm danh gia n v |
| 7 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 8 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 9 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: CHI_PHI

Mô tả:
Lưu thông tin chi phi trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaChiPhi | INT | PK | Mã chi phi |
| 2 | MaCongViec | INT |  | Mã cong viec |
| 3 | MaNganSach | INT | FK | Mã ngan sach |
| 4 | NoiDungChiPhi | NVARCHAR(MAX) |  | Noi dung chi phi |
| 5 | SoTienDaChi | DECIMAL(18, 2) |  | Số tien da chi |
| 6 | NgayChi | DATETIME2(7) |  | Ngày chi |
| 7 | TrangThaiChiPhi | NVARCHAR(50) |  | Trạng thái chi phi |
| 8 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 9 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 10 | DeletedBy | INT | FK | Người thực hiện xóa mềm |

### Bảng: CHUC_DANH

Mô tả:
Lưu thông tin chuc danh trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaChucDanh | INT | PK | Mã chuc danh |
| 2 | TenChucDanh | NVARCHAR(255) |  | Tên chuc danh |
| 3 | MoTaChucDanh | NVARCHAR(255) |  | Mô tả chuc danh |

### Bảng: DANH_GIA_DU_AN

Mô tả:
Lưu thông tin danh gia du an trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaDanhGiaDuAn | INT | PK | Mã danh gia du an |
| 2 | MaDuAn | INT |  | Mã dự án |
| 3 | MaNguoiDung | INT | FK | Mã người dùng |
| 4 | DiemTongDanhGiaDA | INT |  | Điểm tong danh gia d a |
| 5 | NhanXetTongDuAn | NVARCHAR(255) |  | Nhan xet tong du an |
| 6 | NgayDanhGiaDA | DATETIME2(7) |  | Ngày danh gia d a |
| 7 | TrangThaiDanhGiaDA | NVARCHAR(50) |  | Trạng thái danh gia d a |
| 8 | MaNguoiDungDuyet | INT | FK | Mã nguoi dung duyet |
| 9 | NgayDuyetDanhGiaDA | DATETIME2(7) |  | Ngày duyet danh gia d a |
| 10 | LyDoTuChoiDanhGiaDA | NVARCHAR(500) |  | Ly do tu choi danh gia d a |
| 11 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 12 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 13 | DeletedBy | INT | FK | Người thực hiện xóa mềm |

### Bảng: DANH_GIA_NHAN_VIEN

Mô tả:
Lưu thông tin danh gia nhan vien trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaDanhGiaNhanVien | INT | PK | Mã danh gia nhan vien |
| 2 | MaNguoiDung | INT |  | Mã người dùng |
| 3 | MaDuAn | INT | FK | Mã dự án |
| 4 | MaNguoiDungDanhGia | INT | FK | Mã nguoi dung danh gia |
| 5 | DiemTongDanhGiaNV | INT |  | Điểm tong danh gia n v |
| 6 | NgayDanhGiaNV | DATETIME2(7) |  | Ngày danh gia n v |
| 7 | XepLoai | NVARCHAR(50) |  | Xep loai |
| 8 | NhanXetTongQuanNV | NVARCHAR(500) |  | Nhan xet tong quan n v |
| 9 | TrangThaiDanhGiaNV | NVARCHAR(50) |  | Trạng thái danh gia n v |
| 10 | MaNguoiDungDuyet | INT | FK | Mã nguoi dung duyet |
| 11 | NgayDuyetDanhGiaNV | DATETIME2(7) |  | Ngày duyet danh gia n v |
| 12 | LyDoTuChoiDanhGiaNV | NVARCHAR(500) |  | Ly do tu choi danh gia n v |
| 13 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 14 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 15 | DeletedBy | INT | FK | Người thực hiện xóa mềm |

### Bảng: DANH_MUC_CONG_VIEC

Mô tả:
Lưu thông tin danh muc cong viec trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaDanhMucCV | INT | PK | Mã danh muc c v |
| 2 | MaDuAn | INT |  | Mã dự án |
| 3 | TenDanhMucCV | NVARCHAR(255) |  | Tên danh muc c v |
| 4 | MoTaDanhMucCV | NVARCHAR(255) |  | Mô tả danh muc c v |
| 5 | NgayTaoDMCV | DATETIME2(7) |  | Ngày tao d m c v |
| 6 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 7 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 8 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: DANH_MUC_MAN_HINH

Mô tả:
Lưu thông tin danh muc man hinh trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaManHinh | INT | PK | Mã man hinh |
| 2 | TenManHinh | NVARCHAR(255) |  | Tên man hinh |
| 3 | MoTaManHinh | NVARCHAR(255) |  | Mô tả man hinh |

### Bảng: DANH_MUC_QUYEN

Mô tả:
Lưu thông tin danh muc quyen trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaDanhMucQuyen | INT | PK | Mã danh muc quyen |
| 2 | MaManHinh | INT |  | Mã man hinh |
| 3 | TenDanhMucQuyen | NVARCHAR(255) |  | Tên danh muc quyen |
| 4 | MoTaDanhMucQuyen | NVARCHAR(255) |  | Mô tả danh muc quyen |

### Bảng: DE_XUAT_CONG_VIEC

Mô tả:
Lưu thông tin de xuat cong viec trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaDeXuatCV | INT | PK | Mã de xuat c v |
| 2 | MaDuAn | INT | FK | Mã dự án |
| 3 | MaDanhMucCV | INT |  | Mã danh muc c v |
| 4 | MaMucDo | INT | FK | Mã muc do |
| 5 | MaNguoiDungDeXuat | INT | FK | Mã nguoi dung de xuat |
| 6 | MaNguoiDungDuyet | INT | FK | Mã nguoi dung duyet |
| 7 | TenCongViecDeXuat | NVARCHAR(255) |  | Tên cong viec de xuat |
| 8 | MoTaCongViecDeXuat | NVARCHAR(MAX) |  | Mô tả cong viec de xuat |
| 9 | ChiPhiDeXuat | DECIMAL(18, 2) |  | Chi phí de xuat |
| 10 | NgayBatDauCongViecDeXuat | DATETIME2(7) |  | Ngày bat dau cong viec de xuat |
| 11 | NgayKetThucCVDeXuatDuKien | DATETIME2(7) |  | Ngày ket thuc c v de xuat du kien |
| 12 | NgayDeXuatCongViec | DATETIME2(7) |  | Ngày de xuat cong viec |
| 13 | NgayDuyetDeXuatCongViec | DATETIME2(7) |  | Ngày duyet de xuat cong viec |
| 14 | TrangThaiCongViecDeXuat | NVARCHAR(50) |  | Trạng thái cong viec de xuat |
| 15 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 16 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 17 | DeletedBy | INT | FK | Người thực hiện xóa mềm |

### Bảng: DE_XUAT_NGAN_SACH

Mô tả:
Lưu thông tin de xuat ngan sach trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaDeXuatNS | INT | PK | Mã de xuat n s |
| 2 | MaDuAn | INT |  | Mã dự án |
| 3 | MaNganSachCu | INT | FK | Mã ngan sach cu |
| 4 | NganSachCu | DECIMAL(18, 2) |  | Ngan sach cu |
| 5 | NganSachDeXuat | DECIMAL(18, 2) |  | Ngan sach de xuat |
| 6 | LyDoDeXuat | NVARCHAR(MAX) |  | Ly do de xuat |
| 7 | MaNguoiDungDeXuat | INT | FK | Mã nguoi dung de xuat |
| 8 | MaNguoiDungDuyet | INT | FK | Mã nguoi dung duyet |
| 9 | NgayDeXuat | DATETIME2(7) |  | Ngày de xuat |
| 10 | NgayDuyet | DATETIME2(7) |  | Ngày duyet |
| 11 | TrangThaiDeXuat | NVARCHAR(50) |  | Trạng thái de xuat |
| 12 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 13 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 14 | DeletedBy | INT | FK | Người thực hiện xóa mềm |

### Bảng: DM_NGUYEN_NHAN

Mô tả:
Lưu danh mục nguyên nhân trễ tiến độ chuẩn hóa cho hệ thống AI.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaDMNguyenNhan | INT | PK | Mã d m nguyen nhan |
| 2 | TenNguyenNhan | NVARCHAR(255) |  | Tên nguyen nhan |

### Bảng: DU_AN

Mô tả:
Lưu thông tin du an trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaDuAn | INT | PK | Mã dự án |
| 2 | MaNguoiDung | INT | FK | Mã người dùng |
| 3 | MaLoaiDuAn | INT |  | Mã loại dự án |
| 4 | TenDuAn | NVARCHAR(255) |  | Tên dự án |
| 5 | MoTaDuAn | NVARCHAR(MAX) |  | Mô tả dự án |
| 6 | NgayTaoDuAn | DATETIME2(7) |  | Ngày tao du an |
| 7 | NgayBatDauDuAn | DATETIME2(7) |  | Ngày bat dau du an |
| 8 | NgayKetThucDuAn | DATETIME2(7) |  | Ngày ket thuc du an |
| 9 | PhanTramHoanThanh | INT |  | Phan tram hoan thanh |
| 10 | TrangThaiDuAn | NVARCHAR(50) |  | Trạng thái du an |
| 11 | GhiChuDuAn | NVARCHAR(255) |  | Ghi chu du an |
| 12 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 13 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 14 | DeletedBy | INT | FK | Người thực hiện xóa mềm |

### Bảng: FILE_CONG_VIEC

Mô tả:
Lưu thông tin tệp đính kèm liên quan đến cong viec.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaFileCV | INT | PK | Mã file c v |
| 2 | MaCongViec | INT |  | Mã cong viec |
| 3 | TenFileCV | NVARCHAR(255) |  | Tên file c v |
| 4 | DuongDanFileCV | NVARCHAR(500) |  | Duong dan file c v |
| 5 | NgayUploadFileCV | DATETIME2(7) |  | Ngày upload file c v |
| 6 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 7 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 8 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: FILE_CT_CONG_VIEC

Mô tả:
Lưu thông tin tệp đính kèm liên quan đến ct cong viec.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaFileCTCV | INT | PK | Mã file c t c v |
| 2 | MaChiTietCV | INT |  | Mã chi tiet c v |
| 3 | TenFileCTCV | NVARCHAR(255) |  | Tên file c t c v |
| 4 | DuongDanFileCTCV | NVARCHAR(500) |  | Duong dan file c t c v |
| 5 | NgayUploadFileCTCV | DATETIME2(7) |  | Ngày upload file c t c v |
| 6 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 7 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 8 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: FILE_DU_AN

Mô tả:
Lưu thông tin tệp đính kèm liên quan đến du an.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaFileDA | INT | PK | Mã file d a |
| 2 | MaDuAn | INT |  | Mã dự án |
| 3 | TenFileDA | NVARCHAR(255) |  | Tên file d a |
| 4 | DuongDanFileDA | NVARCHAR(500) |  | Duong dan file d a |
| 5 | NgayUploadFileDA | DATETIME2(7) |  | Ngày upload file d a |
| 6 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 7 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 8 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: FILE_TIEN_DO_CONG_VIEC

Mô tả:
Lưu thông tin tệp đính kèm liên quan đến tien do cong viec.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaFileTDCV | INT | PK | Mã file t d c v |
| 2 | MaTienDo | INT |  | Mã tien do |
| 3 | TenFileTDCV | NVARCHAR(255) |  | Tên file t d c v |
| 4 | DuongDanFileTDCV | NVARCHAR(500) |  | Duong dan file t d c v |
| 5 | NgayUploadFileTDCV | DATETIME2(7) |  | Ngày upload file t d c v |
| 6 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 7 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 8 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: LOAI_DU_AN

Mô tả:
Lưu thông tin loai du an trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaLoaiDuAn | INT | PK | Mã loại dự án |
| 2 | TenLoai | NVARCHAR(255) |  | Tên loai |
| 3 | MoTaLoaiDuAn | NVARCHAR(255) |  | Mô tả loai du an |

### Bảng: MUC_DO_UU_TIEN

Mô tả:
Lưu thông tin muc do uu tien trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaMucDo | INT | PK | Mã muc do |
| 2 | TenMucDo | NVARCHAR(100) |  | Tên muc do |
| 3 | MoTaMucDo | NVARCHAR(255) |  | Mô tả muc do |

### Bảng: NGAN_SACH

Mô tả:
Lưu thông tin ngan sach trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNganSach | INT | PK | Mã ngan sach |
| 2 | MaNguoiDungDuyet | INT | FK | Mã nguoi dung duyet |
| 3 | MaNguoiDungDeXuat | INT | FK | Mã nguoi dung de xuat |
| 4 | MaDuAn | INT |  | Mã dự án |
| 5 | NganSach | DECIMAL(18, 2) |  | Ngan sach |
| 6 | Version | INT |  | Version |
| 7 | IsActive | BIT |  | Trạng thái đang kích hoạt |
| 8 | MoTaNganSach | NVARCHAR(255) |  | Mô tả ngan sach |
| 9 | NgayCapNhatNganSach | DATETIME2(7) |  | Ngày cap nhat ngan sach |
| 10 | NgayDuyetNganSach | DATETIME2(7) |  | Ngày duyet ngan sach |
| 11 | TrangThaiNganSach | NVARCHAR(50) |  | Trạng thái ngan sach |
| 12 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 13 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 14 | DeletedBy | INT | FK | Người thực hiện xóa mềm |

### Bảng: NGUOI_DUNG

Mô tả:
Lưu thông tin nguoi dung trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNguoiDung | INT | PK, FK | Mã người dùng |
| 2 | MaChucDanh | INT | FK | Mã chuc danh |
| 3 | Id | NVARCHAR(128) |  | Mã định danh tài khoản/vai trò |
| 4 | HoTenNguoiDung | NVARCHAR(255) |  | Ho ten nguoi dung |
| 5 | DiaChiNguoiDung | NVARCHAR(255) |  | Dia chi nguoi dung |
| 6 | SdtNguoiDung | NVARCHAR(20) |  | Sdt nguoi dung |
| 7 | NgaySinh | DATETIME2(7) |  | Ngày sinh |
| 8 | AnhDaiDien | NVARCHAR(255) |  | Anh dai dien |
| 9 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 10 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 11 | DeletedBy | INT | FK | Người thực hiện xóa mềm |

### Bảng: NHAN_VIEN_DU_AN

Mô tả:
Lưu thông tin nhan vien du an trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaDuAn | INT | PK, FK | Mã dự án |
| 2 | MaNguoiDung | INT | PK, FK | Mã người dùng |
| 3 | NgayThamGiaDuAn | DATETIME2(7) |  | Ngày tham gia du an |
| 4 | VaiTroTrongDuAn | NVARCHAR(50) |  | Vai tro trong du an |

### Bảng: NHAN_VIEN_TEAM

Mô tả:
Lưu thông tin nhan vien team trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNguoiDung | INT | PK | Mã người dùng |
| 2 | MaTeam | INT | PK, FK | Mã team |
| 3 | VaiTroTrongTeam | NVARCHAR(100) |  | Vai tro trong team |
| 4 | NgayThamGiaTeam | DATETIME2(7) |  | Ngày tham gia team |
| 5 | IsLeader | BIT |  | Is leader |

### Bảng: NHAT_KY_CHI_PHI

Mô tả:
Lưu nhật ký thay đổi dữ liệu liên quan đến chi phi.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNhatKyCP | INT | PK | Mã nhat ky c p |
| 2 | MaCongViec | INT |  | Mã cong viec |
| 3 | MaChiPhi | INT | FK | Mã chi phi |
| 4 | NkSoTienDaChi | DECIMAL(18, 2) |  | Nk so tien da chi |
| 5 | NkNgayChi | DATETIME2(7) |  | Nk ngay chi |
| 6 | NkTrangThaiChiPhi | NVARCHAR(50) |  | Nk trang thai chi phi |
| 7 | HanhDongNKCP | NVARCHAR(255) |  | Hanh dong n k c p |
| 8 | ThoiGianNKCP | DATETIME2(7) |  | Thời gian n k c p |

### Bảng: NHAT_KY_DU_AN

Mô tả:
Lưu nhật ký thay đổi dữ liệu liên quan đến du an.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNhatKyTeamDA | INT | PK | Mã nhat ky team d a |
| 2 | MaTeam | INT | FK | Mã team |
| 3 | MaDuAn | INT | FK | Mã dự án |
| 4 | TeamCuPhuTrach | INT |  | Team cu phu trach |
| 5 | TeamMoiPhuTrach | INT |  | Team moi phu trach |
| 6 | HanhDongNKDA | NVARCHAR(255) |  | Hanh dong n k d a |
| 7 | ThoiGianNKDA | DATETIME2(7) |  | Thời gian n k d a |

### Bảng: NHAT_KY_NGAN_SACH

Mô tả:
Lưu nhật ký thay đổi dữ liệu liên quan đến ngan sach.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNhatKyNS | INT | PK | Mã nhat ky n s |
| 2 | MaNganSach | INT | FK | Mã ngan sach |
| 3 | MaDuAn | INT |  | Mã dự án |
| 4 | SoTienNKNS | DECIMAL(18, 2) |  | Số tien n k n s |
| 5 | NganSachTruoc | DECIMAL(18, 2) |  | Ngan sach truoc |
| 6 | NganSachSau | DECIMAL(18, 2) |  | Ngan sach sau |
| 7 | NkNgayCapNhatNS | DATETIME2(7) |  | Nk ngay cap nhat n s |
| 8 | NkTrangThaiNganSach | NVARCHAR(50) |  | Nk trang thai ngan sach |
| 9 | HanhDongNKNS | NVARCHAR(255) |  | Hanh dong n k n s |
| 10 | ThoiGianNKNS | DATETIME2(7) |  | Thời gian n k n s |

### Bảng: NHAT_KY_PHAN_CONG_CONG_VIEC

Mô tả:
Lưu nhật ký thay đổi dữ liệu liên quan đến phan cong cong viec.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNhatKyPCCV | INT | PK | Mã nhat ky p c c v |
| 2 | MaNguoiDung | INT | FK | Mã người dùng |
| 3 | MaCongViec | INT |  | Mã cong viec |
| 4 | MaNguoiDungGhi | INT | FK | Mã nguoi dung ghi |
| 5 | HanhDongPCCV | NVARCHAR(255) |  | Hanh dong p c c v |
| 6 | ThoiGianPCCV | DATETIME2(7) |  | Thời gian p c c v |

### Bảng: NHAT_KY_PHAN_CONG_CT_CONG_VIEC

Mô tả:
Lưu nhật ký thay đổi dữ liệu liên quan đến phan cong ct cong viec.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNhatKyPCCTCV | INT | PK | Mã nhat ky p c c t c v |
| 2 | MaNguoiDung | INT | FK | Mã người dùng |
| 3 | MaChiTietCV | INT |  | Mã chi tiet c v |
| 4 | MaNguoiDungGhi | INT | FK | Mã nguoi dung ghi |
| 5 | HanhDongPCCTCV | NVARCHAR(255) |  | Hanh dong p c c t c v |
| 6 | ThoiGianPCCTCV | DATETIME2(7) |  | Thời gian p c c t c v |

### Bảng: NHAT_KY_PHU_TRACH_DU_AN

Mô tả:
Lưu nhật ký thay đổi dữ liệu liên quan đến phu trach du an.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNhatKyPTDA | INT | PK | Mã nhat ky p t d a |
| 2 | MaNguoiDung | INT | FK | Mã người dùng |
| 3 | MaDuAn | INT |  | Mã dự án |
| 4 | NkHanhDongPTDA | NVARCHAR(255) |  | Nk hanh dong p t d a |
| 5 | NkThoiGianPTDA | DATETIME2(7) |  | Nk thoi gian p t d a |

### Bảng: NHAT_KY_QUAN_LY_DU_AN

Mô tả:
Lưu nhật ký thay đổi dữ liệu liên quan đến quan ly du an.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNhatKyQLDA | INT | PK | Mã nhat ky q l d a |
| 2 | MaDuAn | INT | FK | Mã dự án |
| 3 | MaNguoiDung | INT |  | Mã người dùng |
| 4 | NkHanhDongQLDA | NVARCHAR(255) |  | Nk hanh dong q l d a |
| 5 | NkThoiGianQLDA | DATETIME2(7) |  | Nk thoi gian q l d a |
| 6 | QLDATuNgay | DATETIME2(7) |  | Q l d a tu ngay |
| 7 | QLDADenNgay | DATETIME2(7) |  | Q l d a den ngay |

### Bảng: PHAN_CONG_CONG_VIEC

Mô tả:
Lưu thông tin phan cong cong viec trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNguoiDung | INT | PK, FK | Mã người dùng |
| 2 | MaCongViec | INT | PK | Mã cong viec |
| 3 | NgayGiaoViec | DATETIME2(7) |  | Ngày giao viec |

### Bảng: PHAN_CONG_CT_CONG_VIEC

Mô tả:
Lưu thông tin phan cong ct cong viec trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaNguoiDung | INT | PK, FK | Mã người dùng |
| 2 | MaChiTietCV | INT | PK | Mã chi tiet c v |
| 3 | NgayGiaoCTCV | DATETIME2(7) |  | Ngày giao c t c v |
| 4 | VaiTroTrongCTCV | NVARCHAR(MAX) |  | Vai tro trong c t c v |

### Bảng: PHONG_CHAT

Mô tả:
Lưu thông tin phong chat trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaPhongChat | INT | PK | Mã phong chat |
| 2 | MaDuAn | INT |  | Mã dự án |
| 3 | TenPhong | NVARCHAR(255) |  | Tên phong |
| 4 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 5 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 6 | DeletedBy | INT | FK | Người thực hiện xóa mềm |

### Bảng: TEAM

Mô tả:
Lưu thông tin team trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaTeam | INT | PK | Mã team |
| 2 | TenTeam | NVARCHAR(255) |  | Tên team |
| 3 | MoTaTeam | NVARCHAR(255) |  | Mô tả team |
| 4 | NgayLapTeam | DATETIME2(7) |  | Ngày lap team |
| 5 | TrangThaiTeam | NVARCHAR(50) |  | Trạng thái team |
| 6 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 7 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 8 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: TEAM_DU_AN

Mô tả:
Lưu thông tin team du an trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaTeam | INT | PK, FK | Mã team |
| 2 | MaDuAn | INT | PK | Mã dự án |
| 3 | NgayTeamThamGiaDA | DATETIME2(7) |  | Ngày team tham gia d a |

### Bảng: TIEN_DO_CONG_VIEC

Mô tả:
Lưu thông tin tien do cong viec trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaTienDo | INT | PK | Mã tien do |
| 2 | MaChiTietCV | INT |  | Mã chi tiet c v |
| 3 | MaNguoiDung | INT | FK | Mã người dùng |
| 4 | MaNguoiDungDuyet | INT | FK | Mã nguoi dung duyet |
| 5 | ThoiGianDuyet | DATETIME2(7) |  | Thời gian duyet |
| 6 | GhiChuDuyet | NVARCHAR(255) |  | Ghi chu duyet |
| 7 | PhanTram | INT |  | Phan tram |
| 8 | GhiChuTienDo | NVARCHAR(255) |  | Ghi chu tien do |
| 9 | ThoiGianCapNhat | DATETIME2(7) |  | Thời gian cap nhat |
| 10 | TrangThaiCTCVDeXuat | NVARCHAR(50) |  | Trạng thái c t c v de xuat |
| 11 | TrangThaiTienDo | NVARCHAR(50) |  | Trạng thái tien do |

### Bảng: TIEU_CHI_DANH_GIA

Mô tả:
Lưu thông tin tieu chi danh gia trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaTieuChi | INT | PK | Mã tieu chi |
| 2 | TenTieuChi | NVARCHAR(255) |  | Tên tieu chi |
| 3 | DiemTieuChi | FLOAT |  | Điểm tieu chi |
| 4 | MoTa | NVARCHAR(255) |  | Mô tả  |
| 5 | LoaiTieuChi | NVARCHAR(255) |  | Loai tieu chi |
| 6 | TrangThaiTieuChi | NVARCHAR(50) |  | Trạng thái tieu chi |

### Bảng: TIN_NHAN

Mô tả:
Lưu thông tin tin nhan trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaTinNhan | INT | PK | Mã tin nhan |
| 2 | MaPhongChat | INT |  | Mã phong chat |
| 3 | MaNguoiDung | INT | FK | Mã người dùng |
| 4 | NoiDungTinNhan | NVARCHAR(MAX) |  | Noi dung tin nhan |
| 5 | ThoiGianGui | DATETIME2(7) |  | Thời gian gui |
| 6 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 7 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 8 | DeletedBy | INT |  | Người thực hiện xóa mềm |

### Bảng: THANH_VIEN_PHONG_CHAT

Mô tả:
Lưu thông tin thanh vien phong chat trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaPhongChat | INT | PK, FK | Mã phong chat |
| 2 | MaNguoiDung | INT | PK | Mã người dùng |
| 3 | VaiTroTrongPhongChat | NVARCHAR(50) |  | Vai tro trong phong chat |

### Bảng: YEU_CAU_DOI_QUAN_LY

Mô tả:
Lưu thông tin yeu cau doi quan ly trong hệ thống.

| STT | Tên thuộc tính | Kiểu dữ liệu | Khóa | Mô tả |
|-----|----------------|-------------|------|-------|
| 1 | MaYeuCauDoiQuanLy | INT | PK | Mã yeu cau doi quan ly |
| 2 | MaQuanLyDeXuat | INT | FK | Mã quan ly de xuat |
| 3 | MaDuAn | INT |  | Mã dự án |
| 4 | MaNguoiDungDuyet | INT | FK | Mã nguoi dung duyet |
| 5 | MaQuanLyHienTai | INT | FK | Mã quan ly hien tai |
| 6 | TrangThaiYeuCauDoiQuanLy | NVARCHAR(255) |  | Trạng thái yeu cau doi quan ly |
| 7 | NgayTaoYeuCauDoiQuanLy | DATETIME2(7) |  | Ngày tao yeu cau doi quan ly |
| 8 | NgayDuyetYeuCauDoiQuanLy | DATETIME2(7) |  | Ngày duyet yeu cau doi quan ly |
| 9 | IsDeleted | BIT |  | Trạng thái xóa mềm |
| 10 | DeletedAt | DATETIME2(7) |  | Thời điểm xóa mềm |
| 11 | DeletedBy | INT | FK | Người thực hiện xóa mềm |

### Các ràng buộc và Index chính

#### Khóa chính

| STT | Bảng | Cột khóa chính |
|-----|------|----------------|
| 1 | __EFMigrationsHistory | MigrationId |
| 2 | AI_DATASET | MaData |
| 3 | AI_KET_QUA | MaAiKetQua |
| 4 | AI_MODEL | MaModel |
| 5 | AI_NGUYEN_NHAN | MaAINguyenNhan |
| 6 | AspNetRoleClaims | Id |
| 7 | AspNetRoles | Id |
| 8 | AspNetUserClaims | Id |
| 9 | AspNetUserLogins | LoginProvider, ProviderKey |
| 10 | AspNetUserRoles | Asp_Id, Id |
| 11 | AspNetUsers | Id |
| 12 | AspNetUserTokens | Id, LoginProvider, Name |
| 13 | CONG_VIEC | MaCongViec |
| 14 | CT_CONG_VIEC | MaChiTietCV |
| 15 | CT_DANH_GIA_DU_AN | MaChiTietDGDA |
| 16 | CT_DANH_GIA_NHAN_VIEN | MaChiTietDGNV |
| 17 | CHI_PHI | MaChiPhi |
| 18 | CHUC_DANH | MaChucDanh |
| 19 | DANH_GIA_DU_AN | MaDanhGiaDuAn |
| 20 | DANH_GIA_NHAN_VIEN | MaDanhGiaNhanVien |
| 21 | DANH_MUC_CONG_VIEC | MaDanhMucCV |
| 22 | DANH_MUC_MAN_HINH | MaManHinh |
| 23 | DANH_MUC_QUYEN | MaDanhMucQuyen |
| 24 | DE_XUAT_CONG_VIEC | MaDeXuatCV |
| 25 | DE_XUAT_NGAN_SACH | MaDeXuatNS |
| 26 | DM_NGUYEN_NHAN | MaDMNguyenNhan |
| 27 | DU_AN | MaDuAn |
| 28 | FILE_CONG_VIEC | MaFileCV |
| 29 | FILE_CT_CONG_VIEC | MaFileCTCV |
| 30 | FILE_DU_AN | MaFileDA |
| 31 | FILE_TIEN_DO_CONG_VIEC | MaFileTDCV |
| 32 | LOAI_DU_AN | MaLoaiDuAn |
| 33 | MUC_DO_UU_TIEN | MaMucDo |
| 34 | NGAN_SACH | MaNganSach |
| 35 | NGUOI_DUNG | MaNguoiDung |
| 36 | NHAN_VIEN_DU_AN | MaDuAn, MaNguoiDung |
| 37 | NHAN_VIEN_TEAM | MaNguoiDung, MaTeam |
| 38 | NHAT_KY_CHI_PHI | MaNhatKyCP |
| 39 | NHAT_KY_DU_AN | MaNhatKyTeamDA |
| 40 | NHAT_KY_NGAN_SACH | MaNhatKyNS |
| 41 | NHAT_KY_PHAN_CONG_CONG_VIEC | MaNhatKyPCCV |
| 42 | NHAT_KY_PHAN_CONG_CT_CONG_VIEC | MaNhatKyPCCTCV |
| 43 | NHAT_KY_PHU_TRACH_DU_AN | MaNhatKyPTDA |
| 44 | NHAT_KY_QUAN_LY_DU_AN | MaNhatKyQLDA |
| 45 | PHAN_CONG_CONG_VIEC | MaNguoiDung, MaCongViec |
| 46 | PHAN_CONG_CT_CONG_VIEC | MaNguoiDung, MaChiTietCV |
| 47 | PHONG_CHAT | MaPhongChat |
| 48 | TEAM | MaTeam |
| 49 | TEAM_DU_AN | MaTeam, MaDuAn |
| 50 | TIEN_DO_CONG_VIEC | MaTienDo |
| 51 | TIEU_CHI_DANH_GIA | MaTieuChi |
| 52 | TIN_NHAN | MaTinNhan |
| 53 | THANH_VIEN_PHONG_CHAT | MaPhongChat, MaNguoiDung |
| 54 | YEU_CAU_DOI_QUAN_LY | MaYeuCauDoiQuanLy |

#### Khóa ngoại

| STT | Bảng | Cột khóa ngoại | Bảng tham chiếu | Cột tham chiếu |
|-----|------|---------------|----------------|---------------|
| 1 | AI_DATASET | MaDuAn | DU_AN | MaDuAn |
| 2 | AI_KET_QUA | MaData | AI_DATASET | MaData |
| 3 | AI_KET_QUA | MaModel | AI_MODEL | MaModel |
| 4 | AI_KET_QUA | MaDMNguyenNhan | DM_NGUYEN_NHAN | MaDMNguyenNhan |
| 5 | AI_NGUYEN_NHAN | MaDuAn | DU_AN | MaDuAn |
| 6 | AI_NGUYEN_NHAN | Asp_Id | AspNetRoles | Id |
| 7 | AspNetRoleClaims | MaDanhMucQuyen | DANH_MUC_QUYEN | MaDanhMucQuyen |
| 8 | AspNetRoleClaims | Asp_Id | AspNetUsers | Id |
| 9 | AspNetUserClaims | Id | AspNetUsers | Id |
| 10 | AspNetUserLogins | Id | AspNetRoles | Id |
| 11 | AspNetUserRoles | Asp_Id | AspNetUsers | Id |
| 12 | AspNetUserRoles | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 13 | AspNetUsers | Id | AspNetUsers | Id |
| 14 | AspNetUserTokens | MaDanhMucCV | DANH_MUC_CONG_VIEC | MaDanhMucCV |
| 15 | CONG_VIEC | MaMucDo | MUC_DO_UU_TIEN | MaMucDo |
| 16 | CONG_VIEC | MaDeXuatCV | DE_XUAT_CONG_VIEC | MaDeXuatCV |
| 17 | CONG_VIEC | DeletedBy | NGUOI_DUNG | MaNguoiDung |
| 18 | CONG_VIEC | MaCongViec | CONG_VIEC | MaCongViec |
| 19 | CT_CONG_VIEC | MaDanhGiaDuAn | DANH_GIA_DU_AN | MaDanhGiaDuAn |
| 20 | CT_DANH_GIA_DU_AN | MaTieuChi | TIEU_CHI_DANH_GIA | MaTieuChi |
| 21 | CT_DANH_GIA_DU_AN | MaCongViec | CONG_VIEC | MaCongViec |
| 22 | CT_DANH_GIA_NHAN_VIEN | MaDanhGiaNhanVien | DANH_GIA_NHAN_VIEN | MaDanhGiaNhanVien |
| 23 | CT_DANH_GIA_NHAN_VIEN | MaTieuChi | TIEU_CHI_DANH_GIA | MaTieuChi |
| 24 | CT_DANH_GIA_NHAN_VIEN | MaCongViec | CONG_VIEC | MaCongViec |
| 25 | CHI_PHI | MaNganSach | NGAN_SACH | MaNganSach |
| 26 | CHI_PHI | DeletedBy | NGUOI_DUNG | MaNguoiDung |
| 27 | CHI_PHI | MaDuAn | DU_AN | MaDuAn |
| 28 | DANH_GIA_DU_AN | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 29 | DANH_GIA_DU_AN | DeletedBy | NGUOI_DUNG | MaNguoiDung |
| 30 | DANH_GIA_DU_AN | MaNguoiDungDuyet | NGUOI_DUNG | MaNguoiDung |
| 31 | DANH_GIA_NHAN_VIEN | MaDuAn | DU_AN | MaDuAn |
| 32 | DANH_GIA_NHAN_VIEN | MaNguoiDungDanhGia | NGUOI_DUNG | MaNguoiDung |
| 33 | DANH_GIA_NHAN_VIEN | DeletedBy | NGUOI_DUNG | MaNguoiDung |
| 34 | DANH_GIA_NHAN_VIEN | MaNguoiDungDuyet | NGUOI_DUNG | MaNguoiDung |
| 35 | DANH_MUC_CONG_VIEC | MaManHinh | DANH_MUC_MAN_HINH | MaManHinh |
| 36 | DANH_MUC_QUYEN | MaDanhMucCV | DANH_MUC_CONG_VIEC | MaDanhMucCV |
| 37 | DE_XUAT_CONG_VIEC | MaDuAn | DU_AN | MaDuAn |
| 38 | DE_XUAT_CONG_VIEC | MaMucDo | MUC_DO_UU_TIEN | MaMucDo |
| 39 | DE_XUAT_CONG_VIEC | MaNguoiDungDeXuat | NGUOI_DUNG | MaNguoiDung |
| 40 | DE_XUAT_CONG_VIEC | MaNguoiDungDuyet | NGUOI_DUNG | MaNguoiDung |
| 41 | DE_XUAT_CONG_VIEC | DeletedBy | NGUOI_DUNG | MaNguoiDung |
| 42 | DE_XUAT_NGAN_SACH | MaNguoiDungDeXuat | NGUOI_DUNG | MaNguoiDung |
| 43 | DE_XUAT_NGAN_SACH | DeletedBy | NGUOI_DUNG | MaNguoiDung |
| 44 | DE_XUAT_NGAN_SACH | MaNguoiDungDuyet | NGUOI_DUNG | MaNguoiDung |
| 45 | DE_XUAT_NGAN_SACH | MaNganSachCu | NGAN_SACH | MaNganSach |
| 46 | DE_XUAT_NGAN_SACH | MaLoaiDuAn | LOAI_DU_AN | MaLoaiDuAn |
| 47 | DU_AN | DeletedBy | NGUOI_DUNG | MaNguoiDung |
| 48 | DU_AN | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 49 | DU_AN | MaCongViec | CONG_VIEC | MaCongViec |
| 50 | FILE_CONG_VIEC | MaChiTietCV | CT_CONG_VIEC | MaChiTietCV |
| 51 | FILE_CT_CONG_VIEC | MaDuAn | DU_AN | MaDuAn |
| 52 | FILE_DU_AN | MaTienDo | TIEN_DO_CONG_VIEC | MaTienDo |
| 53 | FILE_TIEN_DO_CONG_VIEC | MaDuAn | DU_AN | MaDuAn |
| 54 | NGAN_SACH | MaNguoiDungDeXuat | NGUOI_DUNG | MaNguoiDung |
| 55 | NGAN_SACH | MaNguoiDungDuyet | NGUOI_DUNG | MaNguoiDung |
| 56 | NGAN_SACH | DeletedBy | NGUOI_DUNG | MaNguoiDung |
| 57 | NGAN_SACH | Id | AspNetUsers | Id |
| 58 | NGUOI_DUNG | MaChucDanh | CHUC_DANH | MaChucDanh |
| 59 | NGUOI_DUNG | DeletedBy | NGUOI_DUNG | MaNguoiDung |
| 60 | NGUOI_DUNG | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 61 | NHAN_VIEN_DU_AN | MaDuAn | DU_AN | MaDuAn |
| 62 | NHAN_VIEN_DU_AN | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 63 | NHAN_VIEN_TEAM | MaTeam | TEAM | MaTeam |
| 64 | NHAN_VIEN_TEAM | MaCongViec | CONG_VIEC | MaCongViec |
| 65 | NHAT_KY_CHI_PHI | MaChiPhi | CHI_PHI | MaChiPhi |
| 66 | NHAT_KY_CHI_PHI | MaDuAn | DU_AN | MaDuAn |
| 67 | NHAT_KY_DU_AN | MaTeam | TEAM | MaTeam |
| 68 | NHAT_KY_DU_AN | MaDuAn | DU_AN | MaDuAn |
| 69 | NHAT_KY_NGAN_SACH | MaNganSach | NGAN_SACH | MaNganSach |
| 70 | NHAT_KY_NGAN_SACH | MaCongViec | CONG_VIEC | MaCongViec |
| 71 | NHAT_KY_PHAN_CONG_CONG_VIEC | MaNguoiDungGhi | NGUOI_DUNG | MaNguoiDung |
| 72 | NHAT_KY_PHAN_CONG_CONG_VIEC | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 73 | NHAT_KY_PHAN_CONG_CONG_VIEC | MaChiTietCV | CT_CONG_VIEC | MaChiTietCV |
| 74 | NHAT_KY_PHAN_CONG_CT_CONG_VIEC | MaNguoiDungGhi | NGUOI_DUNG | MaNguoiDung |
| 75 | NHAT_KY_PHAN_CONG_CT_CONG_VIEC | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 76 | NHAT_KY_PHAN_CONG_CT_CONG_VIEC | MaDuAn | DU_AN | MaDuAn |
| 77 | NHAT_KY_PHU_TRACH_DU_AN | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 78 | NHAT_KY_QUAN_LY_DU_AN | MaDuAn | DU_AN | MaDuAn |
| 79 | NHAT_KY_QUAN_LY_DU_AN | MaCongViec | CONG_VIEC | MaCongViec |
| 80 | PHAN_CONG_CONG_VIEC | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 81 | PHAN_CONG_CONG_VIEC | MaChiTietCV | CT_CONG_VIEC | MaChiTietCV |
| 82 | PHAN_CONG_CT_CONG_VIEC | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 83 | PHAN_CONG_CT_CONG_VIEC | MaDuAn | DU_AN | MaDuAn |
| 84 | PHONG_CHAT | DeletedBy | NGUOI_DUNG | MaNguoiDung |
| 85 | TEAM | MaDuAn | DU_AN | MaDuAn |
| 86 | TEAM_DU_AN | MaTeam | TEAM | MaTeam |
| 87 | TEAM_DU_AN | MaChiTietCV | CT_CONG_VIEC | MaChiTietCV |
| 88 | TIEN_DO_CONG_VIEC | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 89 | TIEN_DO_CONG_VIEC | MaNguoiDungDuyet | NGUOI_DUNG | MaNguoiDung |
| 90 | TIEN_DO_CONG_VIEC | MaPhongChat | PHONG_CHAT | MaPhongChat |
| 91 | TIN_NHAN | MaNguoiDung | NGUOI_DUNG | MaNguoiDung |
| 92 | THANH_VIEN_PHONG_CHAT | MaPhongChat | PHONG_CHAT | MaPhongChat |
| 93 | THANH_VIEN_PHONG_CHAT | MaDuAn | DU_AN | MaDuAn |
| 94 | YEU_CAU_DOI_QUAN_LY | MaQuanLyHienTai | NGUOI_DUNG | MaNguoiDung |
| 95 | YEU_CAU_DOI_QUAN_LY | MaQuanLyDeXuat | NGUOI_DUNG | MaNguoiDung |
| 96 | YEU_CAU_DOI_QUAN_LY | MaNguoiDungDuyet | NGUOI_DUNG | MaNguoiDung |
| 97 | YEU_CAU_DOI_QUAN_LY | DeletedBy | NGUOI_DUNG | MaNguoiDung |

#### Unique và Index quan trọng

Trong file `quanlyduan.sql`, các index quan trọng được thể hiện chủ yếu thông qua các khóa chính dạng `PRIMARY KEY CLUSTERED`. Không ghi nhận thêm câu lệnh tạo `UNIQUE INDEX` hoặc `CREATE INDEX` độc lập ngoài các khóa chính trong script.