namespace QuanLyDuAn.ViewModels.PhanCongCongViec
{
    public class PhanCongCongViecPageViewModel
    {
        public bool CoTheQuanLyCongViec { get; set; }

        public PhanCongCongViecSummaryViewModel CongViec { get; set; } = new();

        public List<PhanCongCongViecItemViewModel> DanhSachPhanCong { get; set; } = new();

        public List<PhanCongCongViecOptionViewModel> ThanhVienCoThePhanCong { get; set; } = new();

        public HashSet<string> Permissions { get; set; } = new();
    }
}
