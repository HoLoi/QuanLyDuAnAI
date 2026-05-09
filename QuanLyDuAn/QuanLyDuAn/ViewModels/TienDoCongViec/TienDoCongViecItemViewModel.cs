namespace QuanLyDuAn.ViewModels.TienDoCongViec
{
    public class TienDoCongViecItemViewModel
    {
        public int MaChiTietCV { get; set; }
        public int MaCongViec { get; set; }
        public int MaDuAn { get; set; }
        public int? MaTienDoMoiNhat { get; set; }

        public string TenDuAn { get; set; } = string.Empty;
        public string TenCongViec { get; set; } = string.Empty;
        public string TenChiTietCongViec { get; set; } = string.Empty;
        public string NguoiThucHien { get; set; } = string.Empty;

        public int PhanTramHienTai { get; set; }
        public string TrangThaiCTCVCode { get; set; } = string.Empty;
        public string TrangThaiCTCV { get; set; } = string.Empty;
        public DateTime? ThoiGianBaoCaoGanNhat { get; set; }
        public string? GhiChuBaoCaoGanNhat { get; set; }
        public string? TenNguoiBaoCaoGanNhat { get; set; }
        public string? TrangThaiCTCVDeXuatGanNhatCode { get; set; }
        public string? TrangThaiCTCVDeXuatGanNhat { get; set; }
        public string? TrangThaiDuyetBaoCaoGanNhatCode { get; set; }
        public string? TrangThaiDuyetBaoCaoGanNhat { get; set; }
        public bool DangCoBaoCaoChoDuyet { get; set; }

        public bool CoTheCapNhatTienDo { get; set; }
        public bool CoTheDuyetBaoCao { get; set; }
        public List<string> TrangThaiDeXuatOptions { get; set; } = new();
        public List<TienDoCongViecFileItemViewModel> DanhSachFile { get; set; } = new();
        public List<TienDoCongViecLichSuItemViewModel> LichSuTienDo { get; set; } = new();
    }
}

