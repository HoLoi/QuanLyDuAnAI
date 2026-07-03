using QuanLyDuAn.ViewModels.Common;

namespace QuanLyDuAn.ViewModels.DuyetDeXuatNganSach
{
    public class DuyetDeXuatNganSachPageViewModel
    {
        public PaginationViewModel Pagination { get; set; } = new();
        public List<DuyetDeXuatNganSachItemViewModel> DanhSach { get; set; } = new();
        public List<DuyetDeXuatNganSachSelectOptionViewModel> DanhSachDuAn { get; set; } = new();
        public List<DuyetDeXuatNganSachSelectOptionViewModel> DanhSachNguoiDeXuat { get; set; } = new();
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public int? LocMaNguoiDungDeXuat { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? TuKhoa { get; set; }
        public decimal? TuSoTien { get; set; }
        public decimal? DenSoTien { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }

    public class DuyetDeXuatNganSachSelectOptionViewModel
    {
        public int Value { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
