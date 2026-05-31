namespace QuanLyDuAn.ViewModels.Common
{
    public class ExportDropdownViewModel
    {
        public string Action { get; set; } = "XuatFile";
        public Dictionary<string, string?> RouteValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public bool HienThiCsv { get; set; } = true;
    }
}
