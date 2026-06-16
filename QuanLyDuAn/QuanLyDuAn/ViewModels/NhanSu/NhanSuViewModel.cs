using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.NhanSu
{
    public class NhanSuViewModel
    {
        public int MaNguoiDung { get; set; }
        public string HoTenNguoiDung { get; set; } = string.Empty;
        public string? DiaChiNguoiDung { get; set; }
        public string? SdtNguoiDung { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int MaChucDanh { get; set; }
        public string TenChucDanh { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool CoTaiKhoan { get; set; }
        public bool TaiKhoanBiKhoa { get; set; }
        public bool ChoKichHoat { get; set; }
        public bool CoTheGuiLaiKichHoat { get; set; }
        public string TrangThaiTaiKhoan { get; set; } = string.Empty;
    }
}
