namespace QuanLyDuAn.ViewModels.NhanVienDuAn
{
    public class NhanVienDuAnItemViewModel
    {
        public int MaNguoiDung { get; set; }
        public string HoTenNguoiDung { get; set; } = string.Empty;
        public string VaiTroTrongDuAn { get; set; } = string.Empty;
        public DateTime? NgayThamGiaDuAn { get; set; }
        public bool ThuocTeamPhuTrach { get; set; }
        public string TenTeamPhuTrach { get; set; } = string.Empty;
    }
}
