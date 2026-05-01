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
        public int PhanTramHoanThanh { get; set; }
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public int SoLuongTeam { get; set; }
        public int SoLuongThanhVien { get; set; }
    }
}
