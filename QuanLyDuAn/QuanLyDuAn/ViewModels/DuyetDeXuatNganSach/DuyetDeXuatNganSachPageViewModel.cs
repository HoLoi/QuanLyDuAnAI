namespace QuanLyDuAn.ViewModels.DuyetDeXuatNganSach
{
    public class DuyetDeXuatNganSachPageViewModel
    {
        public List<DuyetDeXuatNganSachItemViewModel> DanhSach { get; set; } = new();
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
