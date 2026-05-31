using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuanLyDuAn.Services.Implementations
{
    public class ExportFileService : IExportFileService
    {
        static ExportFileService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public ExportFileFormat ParseFormat(string? format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return ExportFileFormat.Excel;
            }

            return format.Trim().ToLowerInvariant() switch
            {
                "pdf" => ExportFileFormat.Pdf,
                "csv" => ExportFileFormat.Csv,
                _ => ExportFileFormat.Excel
            };
        }

        public ExportFileResult Export(ExportFileRequest request)
        {
            var safePrefix = NormalizeFileNamePrefix(request.FileNamePrefix);
            var dateSuffix = request.ExportedAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            return request.Format switch
            {
                ExportFileFormat.Pdf => new ExportFileResult
                {
                    Content = BuildPdf(request),
                    ContentType = "application/pdf",
                    FileName = $"{safePrefix}_{dateSuffix}.pdf"
                },
                ExportFileFormat.Csv => new ExportFileResult
                {
                    Content = BuildCsv(request),
                    ContentType = "text/csv; charset=utf-8",
                    FileName = $"{safePrefix}_{dateSuffix}.csv"
                },
                _ => new ExportFileResult
                {
                    Content = BuildExcel(request),
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = $"{safePrefix}_{dateSuffix}.xlsx"
                }
            };
        }

        private static byte[] BuildExcel(ExportFileRequest request)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("BaoCao");
            var totalColumns = Math.Max(1, request.Columns.Count);

            worksheet.Cell(1, 1).Value = request.ReportTitle;
            worksheet.Range(1, 1, 1, totalColumns).Merge().Style.Font.SetBold().Font.SetFontSize(14);

            worksheet.Cell(2, 1).Value = $"Ngay xuat: {request.ExportedAt:dd/MM/yyyy HH:mm}";
            worksheet.Range(2, 1, 2, totalColumns).Merge();
            worksheet.Cell(3, 1).Value = $"Nguoi xuat: {(string.IsNullOrWhiteSpace(request.ExportedBy) ? "Khong xac dinh" : request.ExportedBy)}";
            worksheet.Range(3, 1, 3, totalColumns).Merge();
            worksheet.Cell(4, 1).Value = $"Bo loc: {BuildAppliedFiltersText(request.AppliedFiltersText)}";
            worksheet.Range(4, 1, 4, totalColumns).Merge();
            worksheet.Range(2, 1, 4, totalColumns).Style.Font.SetFontColor(XLColor.DimGray);

            var headerRow = 6;
            for (var i = 0; i < request.Columns.Count; i++)
            {
                worksheet.Cell(headerRow, i + 1).Value = request.Columns[i].Header;
                worksheet.Cell(headerRow, i + 1).Style
                    .Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#eaf2ff"))
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            }

            for (var rowIndex = 0; rowIndex < request.Rows.Count; rowIndex++)
            {
                var row = request.Rows[rowIndex];
                for (var colIndex = 0; colIndex < request.Columns.Count; colIndex++)
                {
                    var value = request.Columns[colIndex].ValueSelector(row);
                    worksheet.Cell(headerRow + rowIndex + 1, colIndex + 1).Value = value;
                    worksheet.Cell(headerRow + rowIndex + 1, colIndex + 1).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                }
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static byte[] BuildPdf(ExportFileRequest request)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(24);
                    page.Size(PageSizes.A4.Landscape());
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Content().Column(column =>
                    {
                        column.Spacing(8);
                        column.Item().Text(request.ReportTitle).Bold().FontSize(15);
                        column.Item().Text($"Ngay xuat: {request.ExportedAt:dd/MM/yyyy HH:mm}");
                        column.Item().Text($"Nguoi xuat: {(string.IsNullOrWhiteSpace(request.ExportedBy) ? "Khong xac dinh" : request.ExportedBy)}");
                        column.Item().Text($"Bo loc: {BuildAppliedFiltersText(request.AppliedFiltersText)}");

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                for (var i = 0; i < Math.Max(1, request.Columns.Count); i++)
                                {
                                    columns.RelativeColumn();
                                }
                            });

                            table.Header(header =>
                            {
                                foreach (var col in request.Columns)
                                {
                                    header.Cell().Background("#eaf2ff").Padding(4).Text(col.Header).SemiBold();
                                }
                            });

                            foreach (var row in request.Rows)
                            {
                                foreach (var col in request.Columns)
                                {
                                    table.Cell().BorderBottom(1).BorderColor("#dddddd").PaddingVertical(3).PaddingHorizontal(4)
                                        .Text(col.ValueSelector(row) ?? string.Empty);
                                }
                            }
                        });
                    });
                });
            }).GeneratePdf();
        }

        private static byte[] BuildCsv(ExportFileRequest request)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Tieu de,{EscapeCsv(request.ReportTitle)}");
            sb.AppendLine($"Ngay xuat,{EscapeCsv(request.ExportedAt.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))}");
            sb.AppendLine($"Nguoi xuat,{EscapeCsv(string.IsNullOrWhiteSpace(request.ExportedBy) ? "Khong xac dinh" : request.ExportedBy)}");
            sb.AppendLine($"Bo loc,{EscapeCsv(BuildAppliedFiltersText(request.AppliedFiltersText))}");
            sb.AppendLine();
            sb.AppendLine(string.Join(",", request.Columns.Select(x => EscapeCsv(x.Header))));

            foreach (var row in request.Rows)
            {
                var cells = request.Columns.Select(column => EscapeCsv(column.ValueSelector(row))).ToArray();
                sb.AppendLine(string.Join(",", cells));
            }

            var content = Encoding.UTF8.GetBytes(sb.ToString());
            var bom = Encoding.UTF8.GetPreamble();
            var result = new byte[bom.Length + content.Length];
            Buffer.BlockCopy(bom, 0, result, 0, bom.Length);
            Buffer.BlockCopy(content, 0, result, bom.Length, content.Length);
            return result;
        }

        private static string BuildAppliedFiltersText(string? filters)
        {
            return string.IsNullOrWhiteSpace(filters) ? "Khong ap dung bo loc" : filters.Trim();
        }

        private static string EscapeCsv(string? value)
        {
            var input = value ?? string.Empty;
            if (input.Contains(',') || input.Contains('"') || input.Contains('\n') || input.Contains('\r'))
            {
                return $"\"{input.Replace("\"", "\"\"")}\"";
            }

            return input;
        }

        private static string NormalizeFileNamePrefix(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "bao-cao";
            }

            var clean = input.Trim().ToLowerInvariant();
            foreach (var ch in Path.GetInvalidFileNameChars())
            {
                clean = clean.Replace(ch.ToString(), "-");
            }

            return clean.Replace(" ", "-");
        }
    }
}
