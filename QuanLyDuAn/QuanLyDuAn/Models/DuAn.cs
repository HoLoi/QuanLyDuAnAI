namespace QuanLyDuAn.Models
{
    public class DuAn
    {
        public int MaDuAn { get; set; }
        public int MaNhanVien { get; set; }
        public int MaTrangThai { get; set; }
        public int MaLoaiDuAn { get; set; }
        public string? TenDuAn { get; set; }
        public string? MoTaDuAn { get; set; }
        public DateTime? NgayBatDauDuAn { get; set; }
        public DateTime? NgayKetThucDuAn { get; set; }
        public int? PhanTramHoanThanh { get; set; }
    }
}

