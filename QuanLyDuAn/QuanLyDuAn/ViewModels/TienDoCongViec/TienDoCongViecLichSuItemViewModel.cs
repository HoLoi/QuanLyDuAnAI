namespace QuanLyDuAn.ViewModels.TienDoCongViec
{
    public class TienDoCongViecLichSuItemViewModel
    {
        public int MaTienDo { get; set; }
        public int MaChiTietCV { get; set; }
        public int MaNguoiDung { get; set; }
        public string TenNguoiBaoCao { get; set; } = string.Empty;
        public int PhanTram { get; set; }
        public string TrangThaiCTCVDeXuatCode { get; set; } = string.Empty;
        public string TrangThaiCTCVDeXuat { get; set; } = string.Empty;
        public string TrangThaiDuyetBaoCaoCode { get; set; } = string.Empty;
        public string TrangThaiDuyetBaoCao { get; set; } = string.Empty;
        public string? GhiChuTienDo { get; set; }
        public DateTime? ThoiGianCapNhat { get; set; }
        public int? MaNguoiDungDuyet { get; set; }
        public string? TenNguoiDuyet { get; set; }
        public DateTime? ThoiGianDuyet { get; set; }
        public string? GhiChuDuyet { get; set; }
        public bool CoTheXuLyDuyet { get; set; }
        public List<TienDoCongViecFileItemViewModel> DanhSachFile { get; set; } = new();
    }
}
