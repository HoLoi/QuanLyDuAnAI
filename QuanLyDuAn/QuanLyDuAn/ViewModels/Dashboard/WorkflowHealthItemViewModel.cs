namespace QuanLyDuAn.ViewModels.Dashboard
{
    public class WorkflowHealthItemViewModel
    {
        public string StepLabel { get; set; } = string.Empty;
        public int SoDuAnBiKet { get; set; }
        public string MucDoNghiemTrong { get; set; } = "Thấp";
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
    }
}
