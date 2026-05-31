namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiDatasetPageViewModel
    {
        public bool DangThieuDataset { get; set; }
        public string? CanhBao { get; set; }
        public string? LoiHeThong { get; set; }
        public bool DaKiemTraChatLuongDataset { get; set; }

        public string TenTapTin { get; set; } = string.Empty;
        public int TongDongDaTaiLen { get; set; }
        public List<AiDatasetRowViewModel> DuLieuPreview { get; set; } = [];
        public int TongSoDongDataset { get; set; }
        public int SoDongDuLabel { get; set; }
        public int SoDongThieuLabel { get; set; }
        public AiDatasetTongHopResultViewModel? KetQuaTongHop { get; set; }
        public AiDatasetQualitySummaryViewModel? ChatLuongDatasetDb { get; set; }
        public List<AiDatasetProjectOptionViewModel> DanhSachDuAn { get; set; } = [];
        public int? MaDuAnTongHop { get; set; }

        public AiDatasetValidateResponseViewModel? KetQuaValidate { get; set; }
        public AiDatasetQualityReportViewModel? BaoCaoChatLuong { get; set; }
        public AiTrainRecommendationViewModel? KhuyenNghiTrain { get; set; }
    }
}
