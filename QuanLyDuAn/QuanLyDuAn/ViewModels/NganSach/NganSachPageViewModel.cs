namespace QuanLyDuAn.ViewModels.NganSach
{
    public class NganSachPageViewModel
    {
        public List<NganSachItemViewModel> DanhSach { get; set; } = new();
        public List<NganSachDuAnOptionViewModel> DanhSachDuAn { get; set; } = new();
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
