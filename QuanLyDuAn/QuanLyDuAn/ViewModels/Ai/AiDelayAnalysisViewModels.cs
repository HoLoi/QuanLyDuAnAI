namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiProjectFeatureSnapshotViewModel
    {
        public int MaDuAn { get; set; }
        public string? TrangThaiDuAn { get; set; }
        public int? PhanTramHoanThanh { get; set; }
        public DateTime? NgayKetThucDuAn { get; set; }
        public DateTime? NgayHoanThanhThucTeDuAn { get; set; }
        public DateTime ThoiDiemTongHop { get; set; } = DateTime.Now;
        public bool LaDuAnTre { get; set; }
        public int? MaDMNguyenNhan { get; set; }

        public int SoNhanVienDuAn { get; set; }
        public int TongSoCongViec { get; set; }
        public int SoCongViecTre { get; set; }
        public double TyLeCongViecTre { get; set; }
        public decimal ChiPhiDuKien { get; set; }
        public decimal ChiPhiThucTe { get; set; }
        public decimal ChenhLechChiPhi { get; set; }
        public int SoLanThayDoiNhanSu { get; set; }
        public int SoLanThayDoiQuanLy { get; set; }
        public int SoNgayTreTienDo { get; set; }
        public int SoDeXuatCongViecChoDuyet { get; set; }
        public int SoDeXuatCongViecBiTuChoi { get; set; }
        public double ThoiGianDuyetCongViecTrungBinh { get; set; }
        public int SoDeXuatNganSachChoDuyet { get; set; }
        public int SoDeXuatNganSachBiTuChoi { get; set; }
        public double ThoiGianDuyetNganSachTrungBinh { get; set; }
        public int SoBaoCaoTienDoChoDuyet { get; set; }
        public int SoBaoCaoTienDoBiTuChoi { get; set; }
        public int SoBaoCaoTienDoYeuCauBoSung { get; set; }
        public double TyLeBaoCaoTienDoBiTuChoi { get; set; }
        public int SoLanCapNhatTienDo { get; set; }
        public int SoNgayChamCapNhatTienDo { get; set; }
    }

    public class AiProjectDelayAnalysisPanelViewModel
    {
        public int MaDuAn { get; set; }
        public bool CoThePhanTich { get; set; }
        public bool CoTheXacNhan { get; set; }
        public bool LaPhanTichTamThoi { get; set; }
        public bool LaPhanTichChinhThuc { get; set; }
        public string BadgeTinhTrang { get; set; } = "Chưa áp dụng";
        public string BadgeLoaiPhanTich { get; set; } = string.Empty;
        public string? ThongBao { get; set; }
        public AiAnalyzeDelayReasonResponseViewModel? KetQua { get; set; }
        public DateTime? ThoiGianPhanTich { get; set; }
        public int? MaDMNguyenNhanDaXacNhan { get; set; }
        public string? NguyenNhanDaXacNhan { get; set; }
        public double? DoTinCayDaXacNhan { get; set; }
        public DateTime? ThoiGianXacNhan { get; set; }
        public List<AiReasonOptionViewModel> DanhSachNguyenNhan { get; set; } = [];
    }

    public class AiReasonOptionViewModel
    {
        public int MaDMNguyenNhan { get; set; }
        public string TenNguyenNhan { get; set; } = string.Empty;
    }
}
