namespace QuanLyDuAn.ViewModels.DuyetDeXuatCongViec
{
    public class DuyetDeXuatCongViecItemViewModel
    {
        public int MaDeXuatCV { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public int MaNguoiDungDeXuat { get; set; }
        public string NguoiDungDeXuat { get; set; } = string.Empty;
        public string TenCongViecDeXuat { get; set; } = string.Empty;
        public string MoTaCongViecDeXuat { get; set; } = string.Empty;
        public decimal? ChiPhiDeXuat { get; set; }
        public DateTime? NgayBatDauCongViecDeXuat { get; set; }
        public DateTime? NgayKetThucCVDeXuatDuKien { get; set; }
        public DateTime? NgayDeXuatCongViec { get; set; }
        public string TrangThaiCongViecDeXuat { get; set; } = string.Empty;
    }
}
