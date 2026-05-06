namespace QuanLyDuAn.ViewModels.DeXuatCongViec
{
    public class DeXuatCongViecPageViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public List<DeXuatCongViecItemViewModel> DanhSach { get; set; } = new();
        public DeXuatCongViecCreateViewModel Form { get; set; } = new();
        public List<DeXuatCongViecDuAnOptionViewModel> DanhSachDuAn { get; set; } = new();
        public List<DeXuatCongViecDanhMucOptionViewModel> DanhSachDanhMuc { get; set; } = new();
        public List<DeXuatCongViecMucDoOptionViewModel> DanhSachMucDo { get; set; } = new();
        public decimal? NganSachDuAn { get; set; }
        public decimal TongChiPhiDaDung { get; set; }
        public bool HasApprovedBudget { get; set; }
        public int? LocMaDuAn { get; set; }
        public string? LocTrangThai { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
