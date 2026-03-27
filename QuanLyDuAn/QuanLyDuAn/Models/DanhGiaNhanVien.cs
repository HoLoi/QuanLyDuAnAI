namespace QuanLyDuAn.Models
{
    public class DanhGiaNhanVien
    {
        public int MaDanhGiaNhanVien { get; set; }
        public int MaNhanVien { get; set; }
        public int MaDuAn { get; set; }
        public int NhaMaNhanVien { get; set; }
        public int? DiemTongDanhGiaNv { get; set; }
        public DateTime? NgayDanhGiaNv { get; set; }
        public string? XepLoai { get; set; }
    }
}

