namespace QuanLyDuAn.Services.Exporting
{
    public enum ExportFileFormat
    {
        Excel,
        Pdf,
        Csv
    }

    public class ExportColumnDefinition
    {
        public string Header { get; set; } = string.Empty;
        public Func<object, string> ValueSelector { get; set; } = _ => string.Empty;
    }

    public class ExportFileRequest
    {
        public string ReportTitle { get; set; } = string.Empty;
        public string? ExportedBy { get; set; }
        public DateTime ExportedAt { get; set; } = DateTime.Now;
        public string? AppliedFiltersText { get; set; }
        public ExportFileFormat Format { get; set; } = ExportFileFormat.Excel;
        public string FileNamePrefix { get; set; } = "bao-cao";
        public List<ExportColumnDefinition> Columns { get; set; } = new();
        public List<object> Rows { get; set; } = new();
    }

    public class ExportFileResult
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
        public string FileName { get; set; } = "bao-cao.bin";
    }
}
