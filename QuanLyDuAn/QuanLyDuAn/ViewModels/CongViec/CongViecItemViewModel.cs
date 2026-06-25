namespace QuanLyDuAn.ViewModels.CongViec
{
    public class CongViecItemViewModel
    {
        public int MaCongViec { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public int MaDanhMucCV { get; set; }
        public string TenDanhMucCV { get; set; } = string.Empty;
        public int MaMucDo { get; set; }
        public string TenMucDo { get; set; } = string.Empty;
        public string TenCongViec { get; set; } = string.Empty;
        public string MoTaCongViec { get; set; } = string.Empty;
        public decimal ChiPhiDaChi { get; set; }
        public DateTime? NgayBatDauCongViec { get; set; }
        public DateTime? NgayKetThucCVDuKien { get; set; }
        public DateTime? NgayKetThucCVThucTe { get; set; }
        public DateTime? NgayTaoCongViec { get; set; }
        public string TrangThaiCongViec { get; set; } = string.Empty;
        public string TrangThaiHienThi { get; set; } = string.Empty;
        public string CssTrangThai { get; set; } = string.Empty;
        public string? ThongDiepWorkflow { get; set; }
        public bool IsQuaHan { get; set; }
        public bool IsHoanThanhTre { get; set; }
        public bool IsHoanThanhDungHan { get; set; }
        public bool IsKhongDanhGiaThoiHan { get; set; }
        public int SoNgayTre { get; set; }
        public string MaTinhTrangThoiHan { get; set; } = string.Empty;
        public string TinhTrangThoiHan { get; set; } = string.Empty;
        public string CssTinhTrangThoiHan { get; set; } = string.Empty;
        public int SoNguoiDuocPhanCong { get; set; }
        public bool DaPhanCong => SoNguoiDuocPhanCong > 0;
        public int SoLuongChiTietCongViec { get; set; }
        public bool DaCoChiTietCongViec => SoLuongChiTietCongViec > 0;
        public bool CoThePhanCongCongViec { get; set; }
        public bool CoTheXacNhanHoanThanh { get; set; }
        public bool CoTheMoLai { get; set; }
    }
}
