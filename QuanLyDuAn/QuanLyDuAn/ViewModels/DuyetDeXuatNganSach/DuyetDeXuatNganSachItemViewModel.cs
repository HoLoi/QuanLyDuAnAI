namespace QuanLyDuAn.ViewModels.DuyetDeXuatNganSach
{
    public class DuyetDeXuatNganSachItemViewModel
    {
        public int MaDeXuatNS { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public decimal? NganSachCu { get; set; }
        public decimal? NganSachDeXuat { get; set; }
        public string LyDoDeXuat { get; set; } = string.Empty;
        public int MaNguoiDungDeXuat { get; set; }
        public string NguoiDungDeXuat { get; set; } = string.Empty;
        public DateTime? NgayDeXuat { get; set; }
        public string TrangThaiDeXuat { get; set; } = string.Empty;
    }
}
