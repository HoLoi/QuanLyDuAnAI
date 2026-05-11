namespace QuanLyDuAn.ViewModels.DanhGiaNhanVien
{
    public class DanhGiaNhanVienPageViewModel
    {
        public int? MaDuAn { get; set; }
        public int? MaNhanVien { get; set; }
        public string? TuKhoa { get; set; }

        public List<DanhGiaNhanVienItemViewModel> DanhSach { get; set; } = new();
        public List<DanhGiaNhanVienDuAnOptionViewModel> DanhSachDuAn { get; set; } = new();
        public List<DanhGiaNhanVienNhanVienOptionViewModel> DanhSachNhanVien { get; set; } = new();

        public DanhGiaNhanVienFormViewModel? Form { get; set; }

        public int TongSo { get; set; }
        public int SoNhap { get; set; }
        public int SoChoDuyet { get; set; }
        public int SoDaDuyet { get; set; }
        public int SoTuChoi { get; set; }

        public HashSet<string> Permissions { get; set; } = new();
    }

    public class DanhGiaNhanVienDuAnOptionViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
    }

    public class DanhGiaNhanVienNhanVienOptionViewModel
    {
        public int MaNhanVien { get; set; }
        public string TenNhanVien { get; set; } = string.Empty;
    }
}
