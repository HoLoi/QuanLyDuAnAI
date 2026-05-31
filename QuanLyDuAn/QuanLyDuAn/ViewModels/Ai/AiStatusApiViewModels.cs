namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiHealthResponseViewModel
    {
        public string ServiceStatus { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string? LoadedReasonModel { get; set; }
        public DateTime CheckedAt { get; set; }
    }

    public class AiAdminStatusViewModel
    {
        public string ServiceStatus { get; set; } = string.Empty;
        public string ServiceVersion { get; set; } = string.Empty;
        public string? DefaultReasonModelName { get; set; }
        public bool? DefaultReasonModelExists { get; set; }
        public int TotalLocalModels { get; set; }
        public string? LoadedReasonModel { get; set; }
        public bool LoadedReasonModelInLocalList { get; set; }
        public string ModelDir { get; set; } = string.Empty;
        public int MinReasonTrainRows { get; set; }
        public int MinReasonClassCount { get; set; }
        public int MinReasonRowsPerClass { get; set; }
        public double? ReasonConfidenceThreshold { get; set; }
        public DateTime CheckedAt { get; set; }
    }

    public class AiLogSummaryViewModel
    {
        public int TotalTrainSuccess { get; set; }
        public int TotalTrainFailed { get; set; }
        public int TotalAnalyzeSuccess { get; set; }
        public int TotalAnalyzeFailed { get; set; }
        public string? LatestError { get; set; }
        public string? LatestErrorTime { get; set; }
    }

    public class AiSystemInfoViewModel
    {
        public string PythonVersion { get; set; } = string.Empty;
        public string SklearnVersion { get; set; } = string.Empty;
        public string PandasVersion { get; set; } = string.Empty;
        public int UptimeSeconds { get; set; }
        public string? LoadedReasonModel { get; set; }
        public long? MemoryUsageBytes { get; set; }
    }
}
