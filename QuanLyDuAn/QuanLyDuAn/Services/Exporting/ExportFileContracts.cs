namespace QuanLyDuAn.Services.Exporting
{
    public enum ExportFileFormat
    {
        Excel,
        Pdf,
        Csv
    }

    public enum ExportColumnAlignment
    {
        Left,
        Center,
        Right
    }

    public class ExportColumnDefinition
    {
        public string Header { get; set; } = string.Empty;
        public Func<object, object?> ValueSelector { get; set; } = _ => null;
        public string? NumberFormat { get; set; }
        public ExportColumnAlignment Alignment { get; set; } = ExportColumnAlignment.Left;
        public bool WrapText { get; set; }
        public double MinWidth { get; set; } = 10;
        public double MaxWidth { get; set; } = 32;
        public float PdfRelativeWidth { get; set; } = 1;
        public bool ShowInExcel { get; set; } = true;
        public bool ShowInPdf { get; set; } = true;
        public bool ShowInCsv { get; set; } = true;
    }

    public class ExportSummaryDefinition
    {
        public string Label { get; set; } = string.Empty;
        public object? Value { get; set; }
        public string? NumberFormat { get; set; }
    }

    public sealed class ExportCellValue
    {
        public object? Value { get; init; }
        public string? NumberFormat { get; init; }

        public static ExportCellValue Create(object? value, string? numberFormat = null)
            => new() { Value = value, NumberFormat = numberFormat };
    }

    public class ExportSectionDefinition
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SheetName { get; set; } = "BaoCao";
        public bool IncludeRowNumber { get; set; }
        public bool EnableAutoFilter { get; set; } = true;
        public List<ExportColumnDefinition> Columns { get; set; } = new();
        public List<ExportSummaryDefinition> Summaries { get; set; } = new();
        public List<object> Rows { get; set; } = new();
    }

    public class ExportFileRequest
    {
        public string ReportTitle { get; set; } = string.Empty;
        public string? ExportedBy { get; set; }
        public DateTime ExportedAt { get; set; } = DateTime.Now;
        public string? AppliedFiltersText { get; set; }
        public string? DataScopeText { get; set; }
        public ExportFileFormat Format { get; set; } = ExportFileFormat.Excel;
        public string FileNamePrefix { get; set; } = "BaoCao";
        public string SheetName { get; set; } = "BaoCao";
        public bool IncludeRowNumber { get; set; }
        public bool PdfLandscape { get; set; }
        public int FreezeColumns { get; set; }
        public List<ExportColumnDefinition> Columns { get; set; } = new();
        public List<ExportSummaryDefinition> Summaries { get; set; } = new();
        public List<object> Rows { get; set; } = new();
        public List<ExportSectionDefinition> Sections { get; set; } = new();
    }

    public class ExportFileResult
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
        public string FileName { get; set; } = "bao-cao.bin";
    }
}
