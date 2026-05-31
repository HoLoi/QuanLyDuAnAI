using System.Text.Json.Serialization;

namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiDatasetRowViewModel
    {
        public string? MaDuAn { get; set; }
        public string? TenDuAn { get; set; }
        public double? SoNhanVienDuAn { get; set; }
        public double? TongSoCongViec { get; set; }
        public double? SoCongViecTre { get; set; }
        public double? TyLeCongViecTre { get; set; }
        public double? ChiPhiDuKien { get; set; }
        public double? ChiPhiThucTe { get; set; }
        public double? ChenhLechChiPhi { get; set; }
        public double? SoLanThayDoiNhanSu { get; set; }
        public double? SoLanThayDoiQuanLy { get; set; }
        public double? SoNgayTreTienDo { get; set; }
        public double? SoDeXuatCongViecChoDuyet { get; set; }
        public double? SoDeXuatCongViecBiTuChoi { get; set; }
        public double? ThoiGianDuyetCongViecTrungBinh { get; set; }
        public double? SoDeXuatNganSachChoDuyet { get; set; }
        public double? SoDeXuatNganSachBiTuChoi { get; set; }
        public double? ThoiGianDuyetNganSachTrungBinh { get; set; }
        public double? SoBaoCaoTienDoChoDuyet { get; set; }
        public double? SoBaoCaoTienDoBiTuChoi { get; set; }
        public double? SoBaoCaoTienDoYeuCauBoSung { get; set; }
        public double? TyLeBaoCaoTienDoBiTuChoi { get; set; }
        public double? SoLanCapNhatTienDo { get; set; }
        public double? SoNgayChamCapNhatTienDo { get; set; }
        public int? LaDuAnTre { get; set; }
        public int? MaDMNguyenNhan { get; set; }
        public string? TenNguyenNhanXacNhan { get; set; }
        public bool DuDieuKienTrain { get; set; }
        public DateTime? NgayTongHop { get; set; }
        public string? GhiChuDataset { get; set; }
    }

    public class AiDatasetValidateRequestViewModel
    {
        [JsonPropertyName("dataset")]
        public List<AiDatasetRowViewModel> Dataset { get; set; } = [];
    }

    public class AiDatasetValidateResponseViewModel
    {
        public int TongSoDong { get; set; }
        public int SoDongHopLe { get; set; }
        public int SoDongThieuDuLieu { get; set; }
        public int SoDongDuAnTre { get; set; }
        public int SoDongCoNguyenNhan { get; set; }
        public List<string> CanhBaoDataset { get; set; } = [];
    }

    public class AiDatasetQualityReportViewModel
    {
        public int TongSoDong { get; set; }
        public int SoDongHopLe { get; set; }
        public int SoDongThieuLabel { get; set; }
        public int SoDongThieuFeature { get; set; }
        public Dictionary<string, double> TyLeNullTungFeature { get; set; } = [];
        public Dictionary<string, double> TyLeClass { get; set; } = [];
        public bool DuTrainHayKhong { get; set; }
        public List<string> WarningMessages { get; set; } = [];
        public List<string> BlockingErrors { get; set; } = [];
    }

    public class AiTrainRecommendationViewModel
    {
        public bool NenTrain { get; set; }
        public string LyDo { get; set; } = string.Empty;
        public bool CanhBaoImbalance { get; set; }
        public bool CanhBaoThieuDuLieu { get; set; }
        public List<string> RecommendationMessages { get; set; } = [];
    }
}
