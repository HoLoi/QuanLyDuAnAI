using QuanLyDuAn.ViewModels.Common;

namespace QuanLyDuAn.ViewModels.CongViec
{
    public class CongViecPageViewModel
    {
        public PaginationViewModel Pagination { get; set; } = new();
        public List<CongViecItemViewModel> DanhSach { get; set; } = new();
        public List<CongViecDuAnOptionViewModel> DanhSachDuAn { get; set; } = new();
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public string? TuKhoa { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? LocTheoNgay { get; set; }
        public string? LocTinhTrangThoiHan { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
