using QuanLyDuAn.ViewModels.Common;

namespace QuanLyDuAn.ViewModels.DuAn
{
    public class DuAnPageViewModel
    {
        public PaginationViewModel Pagination { get; set; } = new();
        public List<DuAnViewModel> DanhSach { get; set; } = new();
        public DuAnCreateUpdateViewModel Form { get; set; } = new();
        public List<LoaiDuAnOptionViewModel> DanhSachLoaiDuAn { get; set; } = new();
        public string? TuKhoa { get; set; }
        public int? LocMaLoaiDuAn { get; set; }
        public string? LocTrangThaiDuAn { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? LocTheoNgay { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
