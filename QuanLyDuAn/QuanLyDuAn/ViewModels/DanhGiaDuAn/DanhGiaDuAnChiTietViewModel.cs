namespace QuanLyDuAn.ViewModels.DanhGiaDuAn
{
    public class DanhGiaDuAnChiTietViewModel
    {
        public int MaDanhGiaDuAn { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public string TenNguoiQuanLy { get; set; } = string.Empty;
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public int PhanTramHoanThanh { get; set; }
        public DateTime? NgayBatDauDuAn { get; set; }
        public DateTime? NgayKetThucDuAn { get; set; }
        public int TongCongViec { get; set; }
        public int CongViecHoanThanh { get; set; }
        public int CongViecTreHan { get; set; }
        public int TongChiTietCongViec { get; set; }
        public int ChiTietHoanThanh { get; set; }
        public int ChiTietTreHan { get; set; }
        public int SoBaoCaoTienDo { get; set; }
        public int SoBaoCaoMoiNhat { get; set; }
        public decimal TongNganSach { get; set; }
        public decimal TongChiPhi { get; set; }
        public double TyLeSuDungNganSach { get; set; }
        public bool? CoDuLieuAi { get; set; }
        public bool? DuAnBiTreTheoAi { get; set; }
        public string? TenNguyenNhanAiDuDoan { get; set; }
        public double? DoTinCayAi { get; set; }
        public DateTime? ThoiGianDuDoanAi { get; set; }
        public string? TenNguyenNhanManagerXacNhan { get; set; }
        public double? DoTinCayManagerXacNhan { get; set; }
        public string? TrangThaiDuLieuAi { get; set; }
        public string TrangThaiDanhGia { get; set; } = string.Empty;

        public double DiemTongKet { get; set; }
        public string XepLoai { get; set; } = string.Empty;
        public string? NhanXetTongDuAn { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public string TenNguoiDanhGia { get; set; } = string.Empty;
        public string? TenNguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string? LyDoTuChoi { get; set; }

        public List<DanhGiaDuAnTieuChiViewModel> TieuChi { get; set; } = new();
        public DanhGiaDuAnThongKeViewModel ThongKe { get; set; } = new();
    }
}
