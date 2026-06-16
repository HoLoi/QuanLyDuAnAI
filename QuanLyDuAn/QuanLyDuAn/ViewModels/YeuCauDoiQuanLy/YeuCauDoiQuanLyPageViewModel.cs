using QuanLyDuAn.ViewModels.Common;

namespace QuanLyDuAn.ViewModels.YeuCauDoiQuanLy
{
    public class YeuCauDoiQuanLyPageViewModel
    {
        public PaginationViewModel Pagination { get; set; } = new();
        public int? MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public int? MaQuanLyHienTai { get; set; }
        public string TenQuanLyHienTai { get; set; } = string.Empty;

        public string? TuKhoa { get; set; }
        public string? TrangThai { get; set; }

        public bool CoTheTaoYeuCauMoi { get; set; }
        public string? LyDoKhongTheTaoYeuCau { get; set; }

        public YeuCauDoiQuanLyCreateViewModel Form { get; set; } = new();
        public List<YeuCauDoiQuanLyManagerOptionViewModel> DanhSachQuanLy { get; set; } = new();
        public List<YeuCauDoiQuanLyItemViewModel> DanhSachYeuCau { get; set; } = new();

        public HashSet<string> Permissions { get; set; } = new();
    }
}
