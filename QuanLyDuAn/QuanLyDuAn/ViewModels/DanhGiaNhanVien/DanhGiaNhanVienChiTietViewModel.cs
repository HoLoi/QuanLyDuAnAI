namespace QuanLyDuAn.ViewModels.DanhGiaNhanVien
{
    public class DanhGiaNhanVienChiTietViewModel
    {
        public int MaDanhGiaNhanVien { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public int MaNhanVien { get; set; }
        public string TenNhanVien { get; set; } = string.Empty;
        public string VaiTroTrongDuAn { get; set; } = string.Empty;
        public int TongChiTietDuocGiao { get; set; }
        public int ChiTietHoanThanh { get; set; }
        public int ChiTietDangLam { get; set; }
        public int ChiTietTreHan { get; set; }
        public double TyLeHoanThanh { get; set; }
        public int SoLanCapNhatTienDo { get; set; }
        public DateTime? LanCapNhatGanNhat { get; set; }
        public int SoFileMinhChung { get; set; }
        public double DiemTrungBinhTienDo { get; set; }
        public string TrangThaiDanhGia { get; set; } = string.Empty;

        public double DiemTongKet { get; set; }
        public string XepLoai { get; set; } = string.Empty;
        public string? NhanXetTongQuan { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public string TenNguoiDanhGia { get; set; } = string.Empty;
        public string? TenNguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string? LyDoTuChoi { get; set; }

        public List<DanhGiaNhanVienTieuChiViewModel> TieuChi { get; set; } = new();
        public DanhGiaNhanVienThongKeViewModel ThongKe { get; set; } = new();
    }
}
