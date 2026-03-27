namespace QuanLyDuAn.Models
{
    public class CongViec
    {
        public int MaCongViec { get; set; }
        public int MaDanhMucCv { get; set; }
        public int MaTrangThai { get; set; }
        public int MaMucDo { get; set; }
        public string? TenCongViec { get; set; }
        public string? MoTaCongViec { get; set; }
        public DateTime? NgayBatDauCongViec { get; set; }
        public DateTime? NgayKetThucCvDuKien { get; set; }
        public DateTime? NgayKetThucCvThucTe { get; set; }
        public DateTime? NgayTaoCongViec { get; set; }
    }
}

