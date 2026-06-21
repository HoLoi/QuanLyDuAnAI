namespace QuanLyDuAn.ViewModels.DuAn
{
    public class DuAnViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public string? MoTaDuAn { get; set; }
        public int MaNguoiDung { get; set; }
        public string TenNguoiQuanLy { get; set; } = string.Empty;
        public int MaLoaiDuAn { get; set; }
        public string TenLoaiDuAn { get; set; } = string.Empty;
        public DateTime? NgayTaoDuAn { get; set; }
        public DateTime? NgayBatDauDuAn { get; set; }
        public DateTime? NgayKetThucDuAn { get; set; }
        public DateTime? NgayHoanThanhThucTeDuAn { get; set; }
        public int PhanTramHoanThanh { get; set; }
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public int SoLuongTeam { get; set; }
        public int SoLuongThanhVien { get; set; }
        public bool HasApprovedBudget { get; set; }
        public bool IsQuaHan { get; set; }
        public bool IsHoanThanhTre { get; set; }
        public bool IsHoanThanhDungHan { get; set; }
        public bool CoCongViecTre { get; set; }
        public bool IsConHan { get; set; }
        public bool IsChuaXacDinh { get; set; }
        public bool IsKhongDanhGia { get; set; }
        public int SoNgayTre { get; set; }
        public int SoCongViecTre { get; set; }
        public string MaTinhTrangThoiHan { get; set; } = string.Empty;
        public string TinhTrangThoiHan { get; set; } = string.Empty;
        public string CssTinhTrangThoiHan { get; set; } = string.Empty;
    }
}
