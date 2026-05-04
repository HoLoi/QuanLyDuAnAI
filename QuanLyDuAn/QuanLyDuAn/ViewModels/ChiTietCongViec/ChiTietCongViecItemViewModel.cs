namespace QuanLyDuAn.ViewModels.ChiTietCongViec
{
    public class ChiTietCongViecItemViewModel
    {
        public int MaChiTietCV { get; set; }
        public int MaCongViec { get; set; }
        public string NoiDungChiTietCV { get; set; } = string.Empty;
        public DateTime? NgayTaoCTCV { get; set; }
        public DateTime? NgayBaoCaoCTCV { get; set; }
        public double? PhanTramHoanThanhCTCV { get; set; }
        public string TrangThaiCTCV { get; set; } = string.Empty;
    }
}
