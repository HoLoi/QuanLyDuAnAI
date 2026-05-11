namespace QuanLyDuAn.ViewModels.ChiTietCongViec
{
    public class ChiTietCongViecItemViewModel
    {
        public int MaChiTietCV { get; set; }
        public int MaCongViec { get; set; }
        public string TenCTCV { get; set; } = string.Empty;
        public string NoiDungChiTietCV { get; set; } = string.Empty;
        public DateTime? NgayTaoCTCV { get; set; }
        public DateTime? NgayBatDauCTCV { get; set; }
        public DateTime? NgayKetThucCTCV { get; set; }
        public string TrangThaiCTCV { get; set; } = string.Empty;
        public string TrangThaiHienThi { get; set; } = string.Empty;
        public string CssTrangThai { get; set; } = string.Empty;
        public string? ThongDiepWorkflow { get; set; }
        public bool CoThePhanCongChiTietCongViec { get; set; }
    }
}
