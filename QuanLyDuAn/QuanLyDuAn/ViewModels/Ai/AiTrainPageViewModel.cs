using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiTrainPageViewModel
    {
        public string? CanhBao { get; set; }
        public string? LoiHeThong { get; set; }

        public bool ActivateAfterTrain { get; set; } = true;

        [StringLength(255)]
        public string? TrainNote { get; set; }

        public int TongDongDataset { get; set; }
        public int TongDongDuAnTre { get; set; }
        public int TongDongCoNguyenNhanXacNhan { get; set; }
        public int TongDongDatasetNguyenNhan { get; set; }
        public AiTrainRecommendationViewModel? KhuyenNghiTrainNguyenNhan { get; set; }
        public AiTrainResponseViewModel? KetQuaTrain { get; set; }
        public AiReasonTrainingQualitySummaryViewModel? BaoCaoDatasetNguyenNhanGanNhat { get; set; }
        public List<string> CanhBaoChatLuongModel { get; set; } = [];
        public bool CoTheTrainNguyenNhan { get; set; }
        public Dictionary<int, string> TenNguyenNhanTheoMa { get; set; } = [];
        public Dictionary<string, int> PhanBoNguyenNhanDataset { get; set; } = [];
    }
}
