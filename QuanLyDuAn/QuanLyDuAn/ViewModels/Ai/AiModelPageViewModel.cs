using QuanLyDuAn.ViewModels.Common;

namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiModelPageViewModel
    {
        public PaginationViewModel Pagination { get; set; } = new();
        public string? CanhBao { get; set; }
        public string? LoiHeThong { get; set; }

        public List<AiModelInfoViewModel> DanhSachModel { get; set; } = [];
        public Dictionary<string, object?>? ChiTietModel { get; set; }
        public AiValidateModelResponseViewModel? KetQuaValidate { get; set; }
        public AiCompareModelResponseViewModel? KetQuaCompare { get; set; }
        public string? ModelDangXem { get; set; }
        public string? BoLocLoaiModel { get; set; }
        public string? CurrentModelFile { get; set; }
        public string? NewModelFile { get; set; }
        public string? LoadedModel { get; set; }
        public string? LoadedReasonModel { get; set; }
        public bool CoModelLocal { get; set; }
        public bool CoModelDangNap => !string.IsNullOrWhiteSpace(LoadedModel);
        public bool LoadedModelNamTrongDanhSachLocal { get; set; }
        public bool CoTheKiemTraModel { get; set; }
        public bool CoTheSoSanhModel { get; set; }
        public bool CoTheKichHoatModel { get; set; }
        public string? CanhBaoModelDangNapKhongHopLe { get; set; }
        public string? ThongBaoTinhTrangModel { get; set; }
        public List<AiModelVersionMetricViewModel> LichSuModelNguyenNhan { get; set; } = [];
        public List<string> CanhBaoChatLuongModel { get; set; } = [];
        public Dictionary<string, double> FeatureImportanceModelActive { get; set; } = [];
        public List<List<int>> ConfusionMatrixModelActive { get; set; } = [];
        public List<int> ConfusionMatrixLabelsModelActive { get; set; } = [];
        public Dictionary<int, string> TenNguyenNhanTheoMa { get; set; } = [];
        public double? AccuracyModelActive { get; set; }
        public double? PrecisionMacroModelActive { get; set; }
        public double? RecallMacroModelActive { get; set; }
        public double? F1MacroModelActive { get; set; }
        public string? DecisionTreeTextModelActive { get; set; }
        public string? TenModelPhanTich { get; set; }
        public List<AiDuAnOptionViewModel> DanhSachDuAnTest { get; set; } = [];
        public int? MaDuAnTest { get; set; }
        public string? ModelTestDuocChon { get; set; }
        public AiTestReasonResponseViewModel? KetQuaTestPhanTich { get; set; }
    }

    public class AiModelVersionMetricViewModel
    {
        public string TenModel { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public double? Accuracy { get; set; }
        public double? PrecisionMacro { get; set; }
        public double? RecallMacro { get; set; }
        public double? F1Macro { get; set; }
        public int? TrainSize { get; set; }
        public int? TestSize { get; set; }
        public bool IsActive { get; set; }
    }
}
