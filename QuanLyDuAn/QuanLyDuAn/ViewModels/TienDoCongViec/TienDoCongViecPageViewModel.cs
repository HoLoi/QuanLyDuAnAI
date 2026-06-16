using QuanLyDuAn.ViewModels.Common;

namespace QuanLyDuAn.ViewModels.TienDoCongViec
{
    public class TienDoCongViecPageViewModel
    {
        public PaginationViewModel Pagination { get; set; } = new();
        public TienDoCongViecFilterViewModel Filter { get; set; } = new();
        public TienDoCongViecCapNhatViewModel Form { get; set; } = new();
        public List<TienDoCongViecItemViewModel> DanhSach { get; set; } = new();

        public List<TienDoCongViecDuAnOptionViewModel> DanhSachDuAn { get; set; } = new();
        public List<TienDoCongViecCongViecOptionViewModel> DanhSachCongViec { get; set; } = new();

        public HashSet<string> Permissions { get; set; } = new();
    }
}
