using System.Text.Json.Serialization;

namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiReasonCatalogItemViewModel
    {
        [JsonPropertyName("maDMNguyenNhan")]
        public string MaDMNguyenNhan { get; set; } = string.Empty;

        [JsonPropertyName("tenNguyenNhan")]
        public string TenNguyenNhan { get; set; } = string.Empty;
    }

    public class AiProjectFeatureViewModel
    {
        public double SoNhanVienDuAn { get; set; }
        public double TongSoCongViec { get; set; }
        public double SoCongViecTre { get; set; }
        public double TyLeCongViecTre { get; set; }
        public double ChiPhiDuKien { get; set; }
        public double ChiPhiThucTe { get; set; }
        public double ChenhLechChiPhi { get; set; }
        public double SoLanThayDoiNhanSu { get; set; }
        public double SoLanThayDoiQuanLy { get; set; }
        public double SoNgayTreTienDo { get; set; }
        public double SoDeXuatCongViecChoDuyet { get; set; }
        public double SoDeXuatCongViecBiTuChoi { get; set; }
        public double ThoiGianDuyetCongViecTrungBinh { get; set; }
        public double SoDeXuatNganSachChoDuyet { get; set; }
        public double SoDeXuatNganSachBiTuChoi { get; set; }
        public double ThoiGianDuyetNganSachTrungBinh { get; set; }
        public double SoBaoCaoTienDoChoDuyet { get; set; }
        public double SoBaoCaoTienDoBiTuChoi { get; set; }
        public double SoBaoCaoTienDoYeuCauBoSung { get; set; }
        public double TyLeBaoCaoTienDoBiTuChoi { get; set; }
        public double SoLanCapNhatTienDo { get; set; }
        public double SoNgayChamCapNhatTienDo { get; set; }
    }

    public class AiAnalyzeDelayReasonRequestViewModel
    {
        [JsonPropertyName("maDuAn")]
        public int MaDuAn { get; set; }

        [JsonPropertyName("feature")]
        public AiProjectFeatureViewModel Feature { get; set; } = new();

        [JsonPropertyName("danhMucNguyenNhan")]
        public List<AiReasonCatalogItemViewModel> DanhMucNguyenNhan { get; set; } = [];

        [JsonPropertyName("reasonConfidenceThreshold")]
        public double ReasonConfidenceThreshold { get; set; } = 0.6;
    }

    public class AiAnalyzeDelayReasonResponseViewModel
    {
        public int? MaDMNguyenNhanDuDoan { get; set; }
        public string? TenNguyenNhanDuDoan { get; set; }
        public double DoTinCayKetQua { get; set; }
        [JsonPropertyName("mucPhuHop")]
        public string? MucPhuHop { get; set; }
        [JsonPropertyName("danhSachNguyenNhanLienQuan")]
        public List<AiRelatedReasonItemViewModel>? DanhSachNguyenNhanLienQuan { get; set; }
        public string ReasonSource { get; set; } = "RuleFallback";
        public string? ModelNguyenNhanUsed { get; set; }
        public string? CanhBaoNguyenNhan { get; set; }
        public string? NoiDungPhanTich { get; set; }
    }

    public class AiRelatedReasonItemViewModel
    {
        [JsonPropertyName("maDMNguyenNhan")]
        public int? MaDMNguyenNhan { get; set; }

        [JsonPropertyName("tenNguyenNhan")]
        public string? TenNguyenNhan { get; set; }

        [JsonPropertyName("score")]
        public double? Score { get; set; }

        [JsonPropertyName("mucPhuHop")]
        public string? MucPhuHop { get; set; }
    }

    public class AiTestReasonRequestViewModel
    {
        [JsonPropertyName("modelFile")]
        public string? ModelFile { get; set; }

        [JsonPropertyName("feature")]
        public AiProjectFeatureViewModel Feature { get; set; } = new();

        [JsonPropertyName("danhMucNguyenNhan")]
        public List<AiReasonCatalogItemViewModel>? DanhMucNguyenNhan { get; set; }
    }

    public class AiTestReasonResponseViewModel
    {
        public double Confidence { get; set; }
        public int? SuggestedReasonCode { get; set; }
        public string SuggestedReason { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string ModelUsed { get; set; } = string.Empty;
        public string ReasonSource { get; set; } = "RuleFallback";
    }
}
