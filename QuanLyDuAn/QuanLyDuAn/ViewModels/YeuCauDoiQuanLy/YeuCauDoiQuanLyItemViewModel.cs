namespace QuanLyDuAn.ViewModels.YeuCauDoiQuanLy
{
    public class YeuCauDoiQuanLyItemViewModel
    {
        public int MaYeuCauDoiQuanLy { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public int MaQuanLyHienTai { get; set; }
        public string TenQuanLyHienTai { get; set; } = string.Empty;
        public int MaQuanLyDeXuat { get; set; }
        public string TenQuanLyDeXuat { get; set; } = string.Empty;
        public int? MaNguoiDungDuyet { get; set; }
        public string? TenNguoiDungDuyet { get; set; }
        public string TrangThaiYeuCauDoiQuanLy { get; set; } = string.Empty;
        public DateTime? NgayTaoYeuCauDoiQuanLy { get; set; }
        public DateTime? NgayDuyetYeuCauDoiQuanLy { get; set; }
        public bool CoTheHuy { get; set; }
    }
}
