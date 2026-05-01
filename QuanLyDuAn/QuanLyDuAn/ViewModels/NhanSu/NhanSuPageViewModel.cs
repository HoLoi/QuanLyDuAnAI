namespace QuanLyDuAn.ViewModels.NhanSu
{
    public class NhanSuPageViewModel
    {
        public List<NhanSuViewModel> DanhSach { get; set; } = new();
        public NhanSuCreateUpdateViewModel Form { get; set; } = new();
        public List<ChucDanhOptionViewModel> DanhSachChucDanh { get; set; } = new();
        public List<VaiTroHeThongOptionViewModel> DanhSachVaiTroHeThong { get; set; } = new();
        public string? TuKhoa { get; set; }
        public int? LocMaChucDanh { get; set; }
        public string? LocTrangThaiTaiKhoan { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
