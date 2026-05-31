using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiPredictPageViewModel
    {
        public string? CanhBao { get; set; }
        public string? LoiHeThong { get; set; }

        public int MaDuAn { get; set; }

        [Range(0, 10000)]
        public double SoNhanVienDuAn { get; set; }

        [Range(0, 1000000)]
        public double TongSoCongViec { get; set; }

        [Range(0, 1000000)]
        public double SoCongViecTre { get; set; }

        [Range(0, 100)]
        public double TyLeCongViecTre { get; set; }

        [Range(0, double.MaxValue)]
        public double ChiPhiDuKien { get; set; }

        [Range(0, double.MaxValue)]
        public double ChiPhiThucTe { get; set; }

        [Range(-double.MaxValue, double.MaxValue)]
        public double ChenhLechChiPhi { get; set; }

        [Range(0, 1000000)]
        public double SoLanThayDoiNhanSu { get; set; }

        [Range(0, 1000000)]
        public double SoLanThayDoiQuanLy { get; set; }

        [Range(0, 1000000)]
        public double SoNgayTreTienDo { get; set; }

        [Range(0, 1000000)]
        public double SoDeXuatCongViecChoDuyet { get; set; }

        [Range(0, 1000000)]
        public double SoDeXuatCongViecBiTuChoi { get; set; }

        [Range(0, 1000000)]
        public double ThoiGianDuyetCongViecTrungBinh { get; set; }

        [Range(0, 1000000)]
        public double SoDeXuatNganSachChoDuyet { get; set; }

        [Range(0, 1000000)]
        public double SoDeXuatNganSachBiTuChoi { get; set; }

        [Range(0, 1000000)]
        public double ThoiGianDuyetNganSachTrungBinh { get; set; }

        [Range(0, 1000000)]
        public double SoBaoCaoTienDoChoDuyet { get; set; }

        [Range(0, 1000000)]
        public double SoBaoCaoTienDoBiTuChoi { get; set; }

        [Range(0, 1000000)]
        public double SoBaoCaoTienDoYeuCauBoSung { get; set; }

        [Range(0, 100)]
        public double TyLeBaoCaoTienDoBiTuChoi { get; set; }

        [Range(0, 1000000)]
        public double SoLanCapNhatTienDo { get; set; }

        [Range(0, 1000000)]
        public double SoNgayChamCapNhatTienDo { get; set; }

        public bool? LaDuAnTre { get; set; }
        public string? ThongBaoKhongPhanTich { get; set; }
        public AiAnalyzeDelayReasonResponseViewModel? KetQuaPhanTich { get; set; }
        public AiTestReasonResponseViewModel? KetQuaTestPhanTich { get; set; }
        public List<AiDuAnOptionViewModel> DanhSachDuAn { get; set; } = [];
        public List<string> DanhSachReasonModel { get; set; } = [];
        public string? ModelNguyenNhanMacDinh { get; set; }
        public double? DoChinhXacKiemThuModelNguyenNhan { get; set; }
        public string? ModelTestDuocChon { get; set; }
        public bool CoModelNguyenNhanHoatDongHopLe { get; set; }
        public string? ThongBaoModelNguyenNhan { get; set; }
    }

    public class AiDuAnOptionViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
    }
}
