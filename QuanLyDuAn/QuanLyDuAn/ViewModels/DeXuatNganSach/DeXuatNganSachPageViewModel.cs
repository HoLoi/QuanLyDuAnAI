namespace QuanLyDuAn.ViewModels.DeXuatNganSach
{
    public class DeXuatNganSachPageViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public List<DeXuatNganSachItemViewModel> DanhSach { get; set; } = new();
        public DeXuatNganSachCreateViewModel Form { get; set; } = new();
        public List<DeXuatNganSachDuAnOptionViewModel> DanhSachDuAn { get; set; } = new();
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
