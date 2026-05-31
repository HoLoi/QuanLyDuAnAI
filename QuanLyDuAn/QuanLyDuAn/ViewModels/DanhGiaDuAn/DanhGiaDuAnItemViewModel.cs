namespace QuanLyDuAn.ViewModels.DanhGiaDuAn
{
    public class DanhGiaDuAnItemViewModel
    {
        public int MaDanhGiaDuAn { get; set; }
        public bool CoDanhGia { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public string TenNguoiQuanLy { get; set; } = string.Empty;
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public bool DuDieuKienDanhGia { get; set; }
        public string TrangThaiDanhGia { get; set; } = string.Empty;
        public int PhanTramHoanThanh { get; set; }
        public int TongCongViec { get; set; }
        public int CongViecTreHan { get; set; }
        public string TrangThaiThoiHanDuAn { get; set; } = string.Empty;
        public int? SoNgayQuaHan { get; set; }
        public bool DuAnTreTienDo { get; set; }
        public bool ChuaDuDuLieuTreTienDo { get; set; }
        public double DiemTongKet { get; set; }
        public string XepLoai { get; set; } = string.Empty;
        public string? NhanXet { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public string TenNguoiDanhGia { get; set; } = string.Empty;
        public string? TenNguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string? LyDoTuChoi { get; set; }

        public bool CoTheDanhGia { get; set; }
        public bool CoTheSua { get; set; }
        public bool CoTheGuiDuyet { get; set; }
        public bool CoTheDuyet { get; set; }
        public bool CoTheTuChoi { get; set; }
    }
}
