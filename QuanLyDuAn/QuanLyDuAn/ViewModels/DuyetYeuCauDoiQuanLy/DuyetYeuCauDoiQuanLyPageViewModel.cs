using QuanLyDuAn.ViewModels.Common;

namespace QuanLyDuAn.ViewModels.DuyetYeuCauDoiQuanLy
{
    public class DuyetYeuCauDoiQuanLyPageViewModel
    {
        public PaginationViewModel Pagination { get; set; } = new();
        public string? TrangThai { get; set; }
        public int? MaDuAn { get; set; }
        public string? TuKhoa { get; set; }

        public List<DuyetYeuCauDoiQuanLyItemViewModel> DanhSachYeuCau { get; set; } = new();
        public HashSet<string> Permissions { get; set; } = new();
    }
}
