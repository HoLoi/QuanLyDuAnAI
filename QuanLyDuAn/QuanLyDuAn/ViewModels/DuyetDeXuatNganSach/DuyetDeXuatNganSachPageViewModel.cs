using QuanLyDuAn.ViewModels.Common;

namespace QuanLyDuAn.ViewModels.DuyetDeXuatNganSach
{
    public class DuyetDeXuatNganSachPageViewModel
    {
        public PaginationViewModel Pagination { get; set; } = new();
        public List<DuyetDeXuatNganSachItemViewModel> DanhSach { get; set; } = new();
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
