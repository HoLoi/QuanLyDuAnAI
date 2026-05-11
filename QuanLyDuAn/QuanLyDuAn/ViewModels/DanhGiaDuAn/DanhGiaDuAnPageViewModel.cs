namespace QuanLyDuAn.ViewModels.DanhGiaDuAn
{
    public class DanhGiaDuAnPageViewModel
    {
        public string? TuKhoa { get; set; }
        public string? TrangThai { get; set; }
        public int? MaDuAn { get; set; }

        public List<DanhGiaDuAnItemViewModel> DanhSach { get; set; } = new();
        public List<DanhGiaDuAnDuAnOptionViewModel> DanhSachDuAn { get; set; } = new();

        public DanhGiaDuAnFormViewModel? Form { get; set; }

        public int TongSo { get; set; }
        public int SoNhap { get; set; }
        public int SoChoDuyet { get; set; }
        public int SoDaDuyet { get; set; }
        public int SoTuChoi { get; set; }

        public HashSet<string> Permissions { get; set; } = new();
    }

    public class DanhGiaDuAnDuAnOptionViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
    }
}
