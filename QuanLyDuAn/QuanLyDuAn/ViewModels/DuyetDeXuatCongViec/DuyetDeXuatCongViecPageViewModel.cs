using QuanLyDuAn.ViewModels.Common;

namespace QuanLyDuAn.ViewModels.DuyetDeXuatCongViec
{
    public class DuyetDeXuatCongViecPageViewModel
    {
        public PaginationViewModel Pagination { get; set; } = new();
        public List<DuyetDeXuatCongViecItemViewModel> DanhSach { get; set; } = new();
        public List<DuyetDeXuatCongViecSelectOptionViewModel> DanhSachDuAn { get; set; } = new();
        public List<DuyetDeXuatCongViecSelectOptionViewModel> DanhSachNguoiDeXuat { get; set; } = new();
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public int? LocMaNguoiDungDeXuat { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? TuKhoa { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }

    public class DuyetDeXuatCongViecSelectOptionViewModel
    {
        public int Value { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
