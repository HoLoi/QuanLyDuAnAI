namespace QuanLyDuAn.Models
{
    public class DanhGiaDuAn
    {
        public int MaDanhGiaDuAn { get; set; }
        public int MaDuAn { get; set; }
        public int MaNhanVien { get; set; }
        public int? DiemTongDanhGiaDa { get; set; }
        public string? NhanXetTongDuAn { get; set; }
        public DateTime? NgayDanhGiaDa { get; set; }
    }
}

