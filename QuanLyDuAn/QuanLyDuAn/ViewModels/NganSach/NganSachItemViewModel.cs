namespace QuanLyDuAn.ViewModels.NganSach
{
    public class NganSachItemViewModel
    {
        public int MaNganSach { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public decimal? SoTienNganSach { get; set; }
        public int? Version { get; set; }
        public bool IsActive { get; set; }
        public string MoTaNganSach { get; set; } = string.Empty;
        public DateTime? NgayCapNhatNganSach { get; set; }
        public DateTime? NgayDuyetNganSach { get; set; }
        public string TrangThaiNganSach { get; set; } = string.Empty;
        public string NguoiDungDeXuat { get; set; } = string.Empty;
        public string NguoiDungDuyet { get; set; } = string.Empty;
    }
}
