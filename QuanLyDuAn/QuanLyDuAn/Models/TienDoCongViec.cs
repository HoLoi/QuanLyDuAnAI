namespace QuanLyDuAn.Models
{
    public class TienDoCongViec
    {
        public int MaTienDo { get; set; }
        public int MaCongViec { get; set; }
        public int MaNhanVien { get; set; }
        public int? PhanTram { get; set; }
        public string? GhiChuTienDo { get; set; }
        public DateTime? ThoiGianCapNhat { get; set; }
        public string? TrangThaiTienDo { get; set; }
    }
}

