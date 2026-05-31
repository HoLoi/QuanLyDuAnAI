namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiDashboardPageViewModel
    {
        public bool LaQuanTriVien { get; set; }
        public bool LaNguoiQuanLy { get; set; }

        public string? CanhBao { get; set; }
        public string? LoiHeThong { get; set; }

        public AiHealthResponseViewModel? Health { get; set; }
        public AiAdminStatusViewModel? AdminStatus { get; set; }
        public AiLogSummaryViewModel? LogSummary { get; set; }
        public AiSystemInfoViewModel? SystemInfo { get; set; }
        public string? ModelNguyenNhanDangHoatDong { get; set; }

        public int TongModelTrongDb { get; set; }
        public int TongLanPhanTichTrongDb { get; set; }
        public int TongXacNhanNguyenNhanTrongDb { get; set; }
        public int SoDuAnDuocXacDinhTre { get; set; }
        public int SoDuAnTreChuaXacNhan { get; set; }
        public int TongDongDataset { get; set; }
        public int TongDongDatasetHopLeTrain { get; set; }
        public int SoDuAnTreDaXacNhan { get; set; }
        public List<NguyenNhanThongKeItemViewModel> NguyenNhanPhoBien { get; set; } = [];
        public List<AiManagerRiskItemViewModel> CanhBaoDuAnQuanLy { get; set; } = [];
        public List<string> CanhBaoChatLuongModel { get; set; } = [];
        public List<AiModelVersionMetricViewModel> LichSuModelNguyenNhan { get; set; } = [];
        public Dictionary<string, double> FeatureImportanceModelActive { get; set; } = [];
        public List<List<int>> ConfusionMatrixModelActive { get; set; } = [];
        public List<int> ConfusionMatrixLabelsModelActive { get; set; } = [];
        public Dictionary<int, string> TenNguyenNhanTheoMa { get; set; } = [];
        public double? AccuracyModelActive { get; set; }
        public double? PrecisionMacroModelActive { get; set; }
        public double? RecallMacroModelActive { get; set; }
        public double? F1MacroModelActive { get; set; }
        public string? DecisionTreeTextModelActive { get; set; }
        public int TrainSizeModelActive { get; set; }
        public int TestSizeModelActive { get; set; }
    }

    public class AiManagerRiskItemViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public string TenNguyenNhan { get; set; } = string.Empty;
        public double DoTinCay { get; set; }
        public DateTime? ThoiGianDuDoan { get; set; }
        public string NguonGoiY { get; set; } = "RuleFallback";
        public bool KetQuaAiCoTheDaCu { get; set; }
        public string? CanhBao { get; set; }
    }
}
