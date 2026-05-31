namespace QuanLyDuAn.ViewModels.DanhGiaNhanVien
{
    public class DanhGiaNhanVienItemViewModel
    {
        public int MaDanhGiaNhanVien { get; set; }
        public bool CoDanhGia { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public int MaNhanVien { get; set; }
        public string TenNhanVien { get; set; } = string.Empty;
        public string VaiTroTrongDuAn { get; set; } = string.Empty;
        public string TrangThaiDanhGia { get; set; } = string.Empty;
        public double DiemTongKet { get; set; }
        public string XepLoai { get; set; } = string.Empty;
        public string? NhanXet { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public string TenNguoiDanhGia { get; set; } = string.Empty;
        public string? TenNguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string? LyDoTuChoi { get; set; }

        public int TongChiTietDuocGiao { get; set; }
        public int ChiTietHoanThanh { get; set; }
        public int ChiTietTreHan { get; set; }
        public double TyLeHoanThanh { get; set; }

        public bool CoTheDanhGia { get; set; }
        public bool CoTheSua { get; set; }
        public bool CoTheGuiDuyet { get; set; }
        public bool CoTheDuyet { get; set; }
        public bool CoTheTuChoi { get; set; }
    }
}
