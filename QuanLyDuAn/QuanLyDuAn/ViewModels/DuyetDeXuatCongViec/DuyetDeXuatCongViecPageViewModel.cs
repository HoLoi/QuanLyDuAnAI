using QuanLyDuAn.ViewModels.Common;

namespace QuanLyDuAn.ViewModels.DuyetDeXuatCongViec
{
    public class DuyetDeXuatCongViecPageViewModel
    {
        public PaginationViewModel Pagination { get; set; } = new();
        public List<DuyetDeXuatCongViecItemViewModel> DanhSach { get; set; } = new();
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
