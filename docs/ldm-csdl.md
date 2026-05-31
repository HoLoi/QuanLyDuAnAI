## 4.3.4 Thiết kế LDM (Mức logic)

Lược đồ dữ liệu logic (LDM) mô tả các bảng dữ liệu chính và mối quan hệ giữa các bảng trong hệ thống. Các bảng được tổ chức theo từng nhóm chức năng nhằm phục vụ cho việc quản lý dữ liệu và xử lý nghiệp vụ.
Trong mục này chỉ trình bày các bảng nghiệp vụ và bảng Identity đang dùng; bảng kỹ thuật migration `__EFMigrationsHistory` không đưa vào mô tả LDM báo cáo.

### 1. Nhóm dữ liệu người dùng và phân quyền

NGUOI_DUNG(MaNguoiDung, MaChucDanh, Id, HoTenNguoiDung, SdtNguoiDung, IsDeleted)
CHUC_DANH(MaChucDanh, TenChucDanh, MoTaChucDanh)
AspNetUsers(Id, MaNguoiDung, UserName, Email, PhoneNumber, LockoutEnd)
AspNetRoles(Id, Name, NormalizedName)
AspNetUserRoles(PK: Asp_Id, Id)
AspNetRoleClaims(Id, Asp_Id, MaDanhMucQuyen, ClaimType, ClaimValue)
AspNetUserClaims(Id, Asp_Id, ClaimType, ClaimValue)
AspNetUserLogins(PK: LoginProvider, ProviderKey; Id, ProviderDisplayName)
AspNetUserTokens(PK: Id, LoginProvider, Name; Value)
DANH_MUC_MAN_HINH(MaManHinh, TenManHinh, MoTaManHinh)
DANH_MUC_QUYEN(MaDanhMucQuyen, MaManHinh, TenDanhMucQuyen, MoTaDanhMucQuyen)

### 2. Nhóm dữ liệu quản lý dự án

DU_AN(MaDuAn, MaNguoiDung, MaLoaiDuAn, TenDuAn, NgayBatDauDuAn, NgayKetThucDuAn, TrangThaiDuAn, PhanTramHoanThanh)
LOAI_DU_AN(MaLoaiDuAn, TenLoai, MoTaLoaiDuAn)
DANH_MUC_CONG_VIEC(MaDanhMucCV, MaDuAn, TenDanhMucCV, NgayTaoDMCV, IsDeleted)
MUC_DO_UU_TIEN(MaMucDo, TenMucDo, MoTaMucDo)

### 3. Nhóm dữ liệu team và nhân sự dự án

TEAM(MaTeam, TenTeam, NgayLapTeam, TrangThaiTeam, IsDeleted)
TEAM_DU_AN(PK: MaTeam, MaDuAn; NgayTeamThamGiaDA)
NHAN_VIEN_DU_AN(PK: MaDuAn, MaNguoiDung; NgayThamGiaDuAn, VaiTroTrongDuAn)
NHAN_VIEN_TEAM(PK: MaNguoiDung, MaTeam; VaiTroTrongTeam, NgayThamGiaTeam, IsLeader)

### 4. Nhóm dữ liệu công việc và chi tiết công việc

CONG_VIEC(MaCongViec, MaDeXuatCV, MaDanhMucCV, MaMucDo, TenCongViec, NgayBatDauCongViec, NgayKetThucCVDuKien, TrangThaiCongViec, IsDeleted)
CT_CONG_VIEC(MaChiTietCV, MaCongViec, TenCTCV, NgayBatDauCTCV, NgayKetThucCTCV, TrangThaiCTCV, IsDeleted)
FILE_CONG_VIEC(MaFileCV, MaCongViec, TenFileCV, DuongDanFileCV, NgayUploadFileCV)
FILE_CT_CONG_VIEC(MaFileCTCV, MaChiTietCV, TenFileCTCV, DuongDanFileCTCV, NgayUploadFileCTCV)

### 5. Nhóm dữ liệu phân công và tiến độ

PHAN_CONG_CONG_VIEC(PK: MaNguoiDung, MaCongViec; NgayGiaoViec)
PHAN_CONG_CT_CONG_VIEC(PK: MaNguoiDung, MaChiTietCV; NgayGiaoCTCV, VaiTroTrongCTCV)
TIEN_DO_CONG_VIEC(MaTienDo, MaChiTietCV, MaNguoiDung, MaNguoiDungDuyet, ThoiGianCapNhat, PhanTram, TrangThaiCTCVDeXuat, TrangThaiTienDo)
FILE_TIEN_DO_CONG_VIEC(MaFileTDCV, MaTienDo, TenFileTDCV, DuongDanFileTDCV, NgayUploadFileTDCV)

### 6. Nhóm dữ liệu đề xuất và duyệt

DE_XUAT_CONG_VIEC(MaDeXuatCV, MaDuAn, MaDanhMucCV, MaMucDo, MaNguoiDungDeXuat, MaNguoiDungDuyet, TenCongViecDeXuat, ChiPhiDeXuat, TrangThaiCongViecDeXuat)
DE_XUAT_NGAN_SACH(MaDeXuatNS, MaDuAn, MaNganSachCu, NganSachCu, NganSachDeXuat, MaNguoiDungDeXuat, MaNguoiDungDuyet, TrangThaiDeXuat)
YEU_CAU_DOI_QUAN_LY(MaYeuCauDoiQuanLy, MaDuAn, MaQuanLyHienTai, MaQuanLyDeXuat, MaNguoiDungDuyet, TrangThaiYeuCauDoiQuanLy)

### 7. Nhóm dữ liệu ngân sách và chi phí

NGAN_SACH(MaNganSach, MaDuAn, MaNguoiDungDeXuat, MaNguoiDungDuyet, NganSach, Version, IsActive, TrangThaiNganSach, IsDeleted)
CHI_PHI(MaChiPhi, MaCongViec, MaNganSach, NoiDungChiPhi, SoTienDaChi, NgayChi, TrangThaiChiPhi, IsDeleted)

### 8. Nhóm dữ liệu đánh giá

TIEU_CHI_DANH_GIA(MaTieuChi, TenTieuChi, DiemTieuChi, LoaiTieuChi, TrangThaiTieuChi)
DANH_GIA_DU_AN(MaDanhGiaDuAn, MaDuAn, MaNguoiDung, DiemTongDanhGiaDA, TrangThaiDanhGiaDA, MaNguoiDungDuyet)
CT_DANH_GIA_DU_AN(MaChiTietDGDA, MaDanhGiaDuAn, MaTieuChi, DiemDanhGiaDA, IsDeleted)
DANH_GIA_NHAN_VIEN(MaDanhGiaNhanVien, MaNguoiDung, MaDuAn, MaNguoiDungDanhGia, DiemTongDanhGiaNV, XepLoai, TrangThaiDanhGiaNV, MaNguoiDungDuyet)
CT_DANH_GIA_NHAN_VIEN(MaChiTietDGNV, MaDanhGiaNhanVien, MaTieuChi, MaCongViec, DiemDanhGiaNV, IsDeleted)

### 9. Nhóm dữ liệu AI

AI_MODEL(MaModel, TenModel, LoaiModel, SoLuongDuLieu, DoChinhXac, IsActive, IsDeleted)
DM_NGUYEN_NHAN(MaDMNguyenNhan, TenNguyenNhan)
AI_DATASET(MaData, MaDuAn, SoNhanVienDuAn, TongSoCongViec, SoCongViecTre, TyLeCongViecTre, ChiPhiDuKien, ChiPhiThucTe, ChenhLechChiPhi, SoLanThayDoiNhanSu, SoLanThayDoiQuanLy, SoNgayTreTienDo, IsTre)
AI_KET_QUA(MaAiKetQua, MaDuAn, MaData, MaModel, MaDMNguyenNhan, DoTinCayKetQua, ThoiGianDuDoanKetQua)
AI_NGUYEN_NHAN(MaAINguyenNhan, MaDuAn, MaDMNguyenNhan, DoTinCay, IsDeleted)

### 10. Nhóm dữ liệu chat và file

PHONG_CHAT(MaPhongChat, MaDuAn, TenPhong, IsDeleted)
THANH_VIEN_PHONG_CHAT(PK: MaPhongChat, MaNguoiDung; VaiTroTrongPhongChat)
TIN_NHAN(MaTinNhan, MaPhongChat, MaNguoiDung, NoiDungTinNhan, ThoiGianGui, IsDeleted)
FILE_DU_AN(MaFileDA, MaDuAn, TenFileDA, DuongDanFileDA, NgayUploadFileDA)

### 11. Nhóm dữ liệu nhật ký/audit

NHAT_KY_DU_AN(MaNhatKyTeamDA, MaTeam, MaDuAn, TeamCuPhuTrach, TeamMoiPhuTrach, HanhDongNKDA, ThoiGianNKDA)
NHAT_KY_PHU_TRACH_DU_AN(MaNhatKyPTDA, MaNguoiDung, MaDuAn, NkHanhDongPTDA, NkThoiGianPTDA)
NHAT_KY_QUAN_LY_DU_AN(MaNhatKyQLDA, MaDuAn, MaNguoiDung, NkHanhDongQLDA, NkThoiGianQLDA, QLDATuNgay, QLDADenNgay)
NHAT_KY_NGAN_SACH(MaNhatKyNS, MaNganSach, MaDuAn, NganSachTruoc, NganSachSau, HanhDongNKNS, ThoiGianNKNS)
NHAT_KY_CHI_PHI(MaNhatKyCP, MaCongViec, MaChiPhi, NkSoTienDaChi, NkTrangThaiChiPhi, HanhDongNKCP, ThoiGianNKCP)
NHAT_KY_PHAN_CONG_CONG_VIEC(MaNhatKyPCCV, MaNguoiDung, MaCongViec, MaNguoiDungGhi, HanhDongPCCV, ThoiGianPCCV)
NHAT_KY_PHAN_CONG_CT_CONG_VIEC(MaNhatKyPCCTCV, MaNguoiDung, MaChiTietCV, MaNguoiDungGhi, HanhDongPCCTCV, ThoiGianPCCTCV)

### Quan hệ logic chính

- DU_AN liên kết LOAI_DU_AN qua `MaLoaiDuAn` và liên kết NGUOI_DUNG qua `MaNguoiDung`.
- DU_AN có nhiều DANH_MUC_CONG_VIEC qua `DANH_MUC_CONG_VIEC.MaDuAn`.
- DANH_MUC_CONG_VIEC có nhiều CONG_VIEC qua `CONG_VIEC.MaDanhMucCV`.
- CONG_VIEC có nhiều CT_CONG_VIEC qua `CT_CONG_VIEC.MaCongViec`.
- CT_CONG_VIEC có nhiều TIEN_DO_CONG_VIEC qua `TIEN_DO_CONG_VIEC.MaChiTietCV`.
- DU_AN liên kết TEAM qua bảng trung gian TEAM_DU_AN (PK ghép `MaTeam`, `MaDuAn`).
- DU_AN liên kết NGUOI_DUNG qua bảng trung gian NHAN_VIEN_DU_AN (PK ghép `MaDuAn`, `MaNguoiDung`).
- NGAN_SACH liên kết CHI_PHI qua `CHI_PHI.MaNganSach`.
- AI_DATASET, AI_KET_QUA, AI_NGUYEN_NHAN liên kết DU_AN qua `MaDuAn`.
- PHONG_CHAT liên kết DU_AN qua `MaDuAn`; TIN_NHAN liên kết PHONG_CHAT qua `MaPhongChat`.
