using QuanLyDuAn.ViewModels.Common;

namespace QuanLyDuAn.ViewModels.NganSach
{
    public class NganSachPageViewModel
    {
        public PaginationViewModel Pagination { get; set; } = new();
        public List<NganSachItemViewModel> DanhSach { get; set; } = new();
        public List<NganSachDuAnOptionViewModel> DanhSachDuAn { get; set; } = new();
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public decimal TongNganSach { get; set; }
        public decimal TongNganSachDangHieuLuc { get; set; }
        public decimal TongDaSuDung { get; set; }
        public decimal TongConLai { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
