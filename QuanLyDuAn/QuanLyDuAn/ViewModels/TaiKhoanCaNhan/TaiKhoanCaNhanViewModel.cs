namespace QuanLyDuAn.ViewModels.TaiKhoanCaNhan
{
    public class TaiKhoanCaNhanViewModel
    {
        public int MaNguoiDung { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string HoTenNguoiDung { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? SdtNguoiDung { get; set; }
        public string? DiaChiNguoiDung { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? AnhDaiDien { get; set; }
        public string? TenChucDanh { get; set; }
        public bool TaiKhoanBiKhoa { get; set; }
        public string ChuCaiDau { get; set; } = "U";
        public List<string> VaiTroHeThong { get; set; } = new();
        public List<string> TeamHienTai { get; set; } = new();
        public List<string> QuanLyHienTai { get; set; } = new();
    }
}
