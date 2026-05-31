using System.Text.Json.Serialization;

namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiReasonTrainDatasetItemViewModel
    {
        [JsonPropertyName("SoNhanVienDuAn")]
        public int? SoNhanVienDuAn { get; set; }

        [JsonPropertyName("TongSoCongViec")]
        public int? TongSoCongViec { get; set; }

        [JsonPropertyName("SoCongViecTre")]
        public int? SoCongViecTre { get; set; }

        [JsonPropertyName("TyLeCongViecTre")]
        public double? TyLeCongViecTre { get; set; }

        [JsonPropertyName("ChiPhiDuKien")]
        public decimal? ChiPhiDuKien { get; set; }

        [JsonPropertyName("ChiPhiThucTe")]
        public decimal? ChiPhiThucTe { get; set; }

        [JsonPropertyName("ChenhLechChiPhi")]
        public decimal? ChenhLechChiPhi { get; set; }

        [JsonPropertyName("SoLanThayDoiNhanSu")]
        public int? SoLanThayDoiNhanSu { get; set; }

        [JsonPropertyName("SoLanThayDoiQuanLy")]
        public int? SoLanThayDoiQuanLy { get; set; }

        [JsonPropertyName("SoNgayTreTienDo")]
        public int? SoNgayTreTienDo { get; set; }

        [JsonPropertyName("SoDeXuatCongViecChoDuyet")]
        public int? SoDeXuatCongViecChoDuyet { get; set; }

        [JsonPropertyName("SoDeXuatCongViecBiTuChoi")]
        public int? SoDeXuatCongViecBiTuChoi { get; set; }

        [JsonPropertyName("ThoiGianDuyetCongViecTrungBinh")]
        public double? ThoiGianDuyetCongViecTrungBinh { get; set; }

        [JsonPropertyName("SoDeXuatNganSachChoDuyet")]
        public int? SoDeXuatNganSachChoDuyet { get; set; }

        [JsonPropertyName("SoDeXuatNganSachBiTuChoi")]
        public int? SoDeXuatNganSachBiTuChoi { get; set; }

        [JsonPropertyName("ThoiGianDuyetNganSachTrungBinh")]
        public double? ThoiGianDuyetNganSachTrungBinh { get; set; }

        [JsonPropertyName("SoBaoCaoTienDoChoDuyet")]
        public int? SoBaoCaoTienDoChoDuyet { get; set; }

        [JsonPropertyName("SoBaoCaoTienDoBiTuChoi")]
        public int? SoBaoCaoTienDoBiTuChoi { get; set; }

        [JsonPropertyName("SoBaoCaoTienDoYeuCauBoSung")]
        public int? SoBaoCaoTienDoYeuCauBoSung { get; set; }

        [JsonPropertyName("TyLeBaoCaoTienDoBiTuChoi")]
        public double? TyLeBaoCaoTienDoBiTuChoi { get; set; }

        [JsonPropertyName("SoLanCapNhatTienDo")]
        public int? SoLanCapNhatTienDo { get; set; }

        [JsonPropertyName("SoNgayChamCapNhatTienDo")]
        public int? SoNgayChamCapNhatTienDo { get; set; }

        [JsonPropertyName("MaDMNguyenNhan")]
        public int MaDMNguyenNhan { get; set; }
    }
}
