namespace QuanLyDuAn.ViewModels.DuyetDeXuatCongViec
{
    public class DuyetDeXuatCongViecPageViewModel
    {
        public List<DuyetDeXuatCongViecItemViewModel> DanhSach { get; set; } = new();
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
