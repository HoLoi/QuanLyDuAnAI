namespace QuanLyDuAn.ViewModels.DuAn
{
    public class DuAnDeadlinePreviewViewModel
    {
        public int MaCongViec { get; set; }
        public string TenCongViec { get; set; } = string.Empty;
        public string TrangThaiCongViec { get; set; } = string.Empty;
        public DateTime? NgayKetThucDuKien { get; set; }
        public int? SoNgayConLai { get; set; }
    }
}
