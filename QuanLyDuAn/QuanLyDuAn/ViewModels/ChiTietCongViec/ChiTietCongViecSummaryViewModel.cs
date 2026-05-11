namespace QuanLyDuAn.ViewModels.ChiTietCongViec
{
    public class ChiTietCongViecSummaryViewModel
    {
        public int MaCongViec { get; set; }
        public string TenCongViec { get; set; } = string.Empty;
        public string TenTrangThai { get; set; } = string.Empty;
        public string TrangThaiCongViec { get; set; } = string.Empty;
        public string CssTrangThai { get; set; } = string.Empty;
        public string? ThongDiepWorkflow { get; set; }
        public int TongSoChiTiet { get; set; }
        public int SoChiTietHoanThanh { get; set; }
        public int PhanTramTienDo { get; set; }
    }
}
