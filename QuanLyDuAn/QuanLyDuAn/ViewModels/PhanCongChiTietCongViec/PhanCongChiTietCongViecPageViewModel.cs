namespace QuanLyDuAn.ViewModels.PhanCongChiTietCongViec
{
    public class PhanCongChiTietCongViecPageViewModel
    {
        public bool CoTheQuanLyCongViec { get; set; }

        public PhanCongChiTietCongViecSummaryViewModel ChiTietCongViec { get; set; } = new();

        public PhanCongChiTietCongViecCreateViewModel Form { get; set; } = new();

        public List<PhanCongChiTietCongViecItemViewModel> DanhSachPhanCong { get; set; } = new();

        public List<PhanCongChiTietCongViecOptionViewModel> ThanhVienCoThePhanCong { get; set; } = new();

        public HashSet<string> Permissions { get; set; } = new();
    }
}
