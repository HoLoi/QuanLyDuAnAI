namespace QuanLyDuAn.ViewModels.DanhMucCongViec
{
    public class DanhMucCongViecPageViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public List<DanhMucCongViecViewModel> DanhSach { get; set; } = new();
        public DanhMucCongViecCreateUpdateViewModel Form { get; set; } = new();
        public List<DuAnOptionViewModel> DanhSachDuAn { get; set; } = new();
        public string? TuKhoa { get; set; }
        public int? LocMaDuAn { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
