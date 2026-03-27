namespace QuanLyDuAn.Models
{
    public class NhanVien
    {
        public int MaNhanVien { get; set; }
        public int MaVaiTro { get; set; }
        public string? UserId { get; set; }
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Cccd { get; set; }

        public QuanLyDuAn.Data.ApplicationUser? User { get; set; }
    }
}

