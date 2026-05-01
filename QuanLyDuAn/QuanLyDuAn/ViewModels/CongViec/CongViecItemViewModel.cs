namespace QuanLyDuAn.ViewModels.CongViec
{
    public class CongViecItemViewModel
    {
        public int MaCongViec { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public int MaDanhMucCV { get; set; }
        public string TenDanhMucCV { get; set; } = string.Empty;
        public int MaMucDo { get; set; }
        public string TenMucDo { get; set; } = string.Empty;
        public string TenCongViec { get; set; } = string.Empty;
        public string MoTaCongViec { get; set; } = string.Empty;
        public decimal ChiPhiDaChi { get; set; }
        public DateTime? NgayBatDauCongViec { get; set; }
        public DateTime? NgayKetThucCVDuKien { get; set; }
        public DateTime? NgayKetThucCVThucTe { get; set; }
        public DateTime? NgayTaoCongViec { get; set; }
        public string TrangThaiCongViec { get; set; } = string.Empty;
    }
}
