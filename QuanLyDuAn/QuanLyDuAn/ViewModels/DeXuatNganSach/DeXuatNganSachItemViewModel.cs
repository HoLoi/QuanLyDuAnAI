namespace QuanLyDuAn.ViewModels.DeXuatNganSach
{
    public class DeXuatNganSachItemViewModel
    {
        public int MaDeXuatNS { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public int? MaNganSachCu { get; set; }
        public decimal? NganSachCu { get; set; }
        public decimal? NganSachDeXuat { get; set; }
        public string LyDoDeXuat { get; set; } = string.Empty;
        public int MaNguoiDungDeXuat { get; set; }
        public string NguoiDungDeXuat { get; set; } = string.Empty;
        public int? MaNguoiDungDuyet { get; set; }
        public string? NguoiDungDuyet { get; set; }
        public DateTime? NgayDeXuat { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string TrangThaiDeXuat { get; set; } = string.Empty;
    }
}
