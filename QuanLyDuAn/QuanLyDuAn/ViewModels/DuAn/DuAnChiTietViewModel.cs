namespace QuanLyDuAn.ViewModels.DuAn
{
    public class DuAnChiTietViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public string? MoTaDuAn { get; set; }
        public int MaLoaiDuAn { get; set; }
        public string TenLoaiDuAn { get; set; } = string.Empty;
        public DateTime? NgayTaoDuAn { get; set; }
        public DateTime? NgayBatDauDuAn { get; set; }
        public DateTime? NgayKetThucDuAn { get; set; }
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public int PhanTramHoanThanh { get; set; }
        public string? GhiChuDuAn { get; set; }
        public int MaNguoiDung { get; set; }
        public string TenNguoiQuanLy { get; set; } = string.Empty;

        public int SoLuongTeam { get; set; }
        public int SoLuongThanhVien { get; set; }
        public int SoLuongCongViec { get; set; }
        public int SoLuongChiTietCongViec { get; set; }

        public int? MaCongViecDauTien { get; set; }

        public string? TuKhoa { get; set; }
        public int? LocMaLoaiDuAn { get; set; }
        public string? LocTrangThaiDuAn { get; set; }

        public HashSet<string> Permissions { get; set; } = new();
    }
}
