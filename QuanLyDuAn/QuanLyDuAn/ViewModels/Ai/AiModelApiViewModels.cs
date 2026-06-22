using System.Text.Json.Serialization;

namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiTrainRequestViewModel
    {
        [JsonPropertyName("dataset")]
        public List<AiReasonTrainDatasetItemViewModel> Dataset { get; set; } = [];
        [JsonPropertyName("requestedByUserId")]
        public string? RequestedByUserId { get; set; }
        [JsonPropertyName("requestedByUserName")]
        public string? RequestedByUserName { get; set; }
        [JsonPropertyName("trainNote")]
        public string? TrainNote { get; set; }
        [JsonPropertyName("activateAfterTrain")]
        public bool ActivateAfterTrain { get; set; }
        [JsonPropertyName("modelType")]
        public string ModelType { get; set; } = "NguyenNhan";
    }

    public class AiTrainResponseViewModel
    {
        public string TenModel { get; set; } = string.Empty;
        public string ModelFile { get; set; } = string.Empty;
        public string ModelPath { get; set; } = string.Empty;
        public string MetadataFile { get; set; } = string.Empty;
        public int SoLuongDuLieu { get; set; }
        public double DoChinhXac { get; set; }
        public int TrainSize { get; set; }
        public int TestSize { get; set; }
        public DateTime NgayTao { get; set; }
        public string MoTaModel { get; set; } = string.Empty;
        public string ModelType { get; set; } = "NguyenNhan";
        public List<string> FeatureList { get; set; } = [];
        public string LabelColumn { get; set; } = "MaDMNguyenNhan";
        public Dictionary<string, double> FeatureImportance { get; set; } = [];
        public List<List<int>> ConfusionMatrix { get; set; } = [];
        public List<int> ConfusionMatrixLabels { get; set; } = [];
        public Dictionary<string, object> ClassificationReport { get; set; } = [];
        public double? PrecisionMacro { get; set; }
        public double? RecallMacro { get; set; }
        public double? F1Macro { get; set; }
        public double? PrecisionWeighted { get; set; }
        public double? RecallWeighted { get; set; }
        public double? F1Weighted { get; set; }
        public Dictionary<string, int> ClassDistribution { get; set; } = [];
        public int ValidRowsBeforeClassFilter { get; set; }
        public int UsedRows { get; set; }
        public int AccumulatingRows { get; set; }
        public int EligibleClassCount { get; set; }
        public int AccumulatingClassCount { get; set; }
        public Dictionary<string, int> UsedClassDistribution { get; set; } = [];
        public Dictionary<string, int> DroppedClassDistribution { get; set; } = [];
        public string? DecisionTreeText { get; set; }
        public List<string> WarningMessages { get; set; } = [];
        public bool SuggestedIsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? TrainNote { get; set; }
        public string LoaiModel { get; set; } = "NguyenNhan";
    }

    public class AiModelInfoViewModel
    {
        public string TenFile { get; set; } = string.Empty;
        public int DungLuong { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDefault { get; set; }
        public bool CanLoad { get; set; }
        public string? Algorithm { get; set; }
        public List<string> ExpectedFeatures { get; set; } = [];
        public bool SchemaValid { get; set; }
        public bool CanActivate { get; set; }
        public string? ValidationMessage { get; set; }
        public double? Accuracy { get; set; }
        public int? TrainSize { get; set; }
        public int? TestSize { get; set; }
        public Dictionary<string, int> ClassDistribution { get; set; } = [];
        public Dictionary<string, double> FeatureImportance { get; set; } = [];
        public List<List<int>> ConfusionMatrix { get; set; } = [];
        public List<int> ConfusionMatrixLabels { get; set; } = [];
        public double? PrecisionMacro { get; set; }
        public double? RecallMacro { get; set; }
        public double? F1Macro { get; set; }
        public double? PrecisionWeighted { get; set; }
        public double? RecallWeighted { get; set; }
        public double? F1Weighted { get; set; }
        public string? DecisionTreeText { get; set; }
        public string LoaiModel { get; set; } = "NguyenNhan";
    }

    public class AiValidateModelRequestViewModel
    {
        [JsonPropertyName("modelFile")]
        public string ModelFile { get; set; } = string.Empty;
        [JsonPropertyName("modelType")]
        public string? ModelType { get; set; }
    }

    public class AiValidateModelResponseViewModel
    {
        public bool ModelExists { get; set; }
        public bool CanLoad { get; set; }
        public List<string> ExpectedFeatures { get; set; } = [];
        public bool SchemaValid { get; set; }
        public List<string> Errors { get; set; } = [];
    }

    public class AiCompareModelRequestViewModel
    {
        [JsonPropertyName("currentModelFile")]
        public string CurrentModelFile { get; set; } = string.Empty;
        [JsonPropertyName("newModelFile")]
        public string NewModelFile { get; set; } = string.Empty;
        [JsonPropertyName("testDataset")]
        public List<AiReasonTrainDatasetItemViewModel> TestDataset { get; set; } = [];
        [JsonPropertyName("modelType")]
        public string ModelType { get; set; } = "NguyenNhan";
    }

    public class AiCompareModelResponseViewModel
    {
        public double CurrentAccuracy { get; set; }
        public double NewAccuracy { get; set; }
        public double DifferenceAccuracy { get; set; }
        public List<List<int>> ConfusionMatrixCurrent { get; set; } = [];
        public List<List<int>> ConfusionMatrixNew { get; set; } = [];
        public string Recommendation { get; set; } = string.Empty;
    }

    public class AiModelActivationResultViewModel
    {
        public string ActiveModel { get; set; } = string.Empty;
        public string? LoadedModel { get; set; }
        public string? DefaultAliasModel { get; set; }
        public DateTime ActivatedAt { get; set; }
        public string? PreviousModel { get; set; }
        public string ModelType { get; set; } = "NguyenNhan";
    }

    public class AiModelReloadResultViewModel
    {
        public string? LoadedModel { get; set; }
        public DateTime LoadedAt { get; set; }
        public bool Success { get; set; }
        public string ModelType { get; set; } = "NguyenNhan";
    }

    public class AiModelDeleteResultViewModel
    {
        public string DeletedModel { get; set; } = string.Empty;
    }
}
