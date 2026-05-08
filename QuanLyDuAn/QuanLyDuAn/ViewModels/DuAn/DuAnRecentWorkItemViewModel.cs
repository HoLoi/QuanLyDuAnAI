namespace QuanLyDuAn.ViewModels.DuAn
{
    public class DuAnRecentWorkItemViewModel
    {
        public int MaCongViec { get; set; }
        public string TenCongViec { get; set; } = string.Empty;
        public string TenDanhMucCV { get; set; } = string.Empty;
        public string TrangThaiCongViec { get; set; } = string.Empty;
        public DateTime? NgayTaoCongViec { get; set; }
        public DateTime? NgayKetThucDuKien { get; set; }
    }
}
