namespace QuanLyDuAn.ViewModels.CongViec
{
    public class CongViecPageViewModel
    {
        public List<CongViecItemViewModel> DanhSach { get; set; } = new();
        public List<CongViecDuAnOptionViewModel> DanhSachDuAn { get; set; } = new();
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public string? TuKhoa { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
