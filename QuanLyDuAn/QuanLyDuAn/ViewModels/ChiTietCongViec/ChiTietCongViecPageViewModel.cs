namespace QuanLyDuAn.ViewModels.ChiTietCongViec
{
    public class ChiTietCongViecPageViewModel
    {
        public ChiTietCongViecSummaryViewModel CongViec { get; set; } = new();
        public ChiTietCongViecCreateUpdateViewModel Form { get; set; } = new();
        public List<ChiTietCongViecItemViewModel> DanhSach { get; set; } = new();
        public bool CoTheCapNhat { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
