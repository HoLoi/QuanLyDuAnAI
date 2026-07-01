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
        private const string HeaderColor = "#DCE9F8";
        private const string NavyColor = "#17365D";
        private const string BorderColor = "#D6DEE8";

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
            ArgumentNullException.ThrowIfNull(request);
            var safePrefix = NormalizeFileNamePart(request.FileNamePrefix, "BaoCao");
            var timestamp = request.ExportedAt.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);

            return request.Format switch
            {
                ExportFileFormat.Pdf => new ExportFileResult
                {
                    Content = BuildPdf(request),
                    ContentType = "application/pdf",
                    FileName = $"{safePrefix}_{timestamp}.pdf"
                },
                ExportFileFormat.Csv => new ExportFileResult
                {
                    Content = BuildCsv(request),
                    ContentType = "text/csv; charset=utf-8",
                    FileName = $"{safePrefix}_{timestamp}.csv"
                },
                _ => new ExportFileResult
                {
                    Content = BuildExcel(request),
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FileName = $"{safePrefix}_{timestamp}.xlsx"
                }
            };
        }

        private static byte[] BuildExcel(ExportFileRequest request)
        {
            using var workbook = new XLWorkbook();
            var sections = GetSections(request);

            foreach (var sheetGroup in sections.GroupBy(x => NormalizeSheetName(x.SheetName)))
            {
                var worksheet = workbook.Worksheets.Add(sheetGroup.Key);
                var sheetSections = sheetGroup.ToList();
                var totalColumns = Math.Max(1, sheetSections.Max(GetExcelColumnCount));

                WriteExcelReportMetadata(worksheet, request, totalColumns);
                var currentRow = 7;
                var firstHeaderRow = 6;

                foreach (var section in sheetSections)
                {
                    var columns = GetColumns(section.Columns, ExportFileFormat.Excel);
                    var sectionColumnCount = Math.Max(1, columns.Count + (section.IncludeRowNumber ? 1 : 0));

                    worksheet.Cell(currentRow, 1).Value = section.Title;
                    worksheet.Range(currentRow, 1, currentRow, sectionColumnCount).Merge();
                    worksheet.Range(currentRow, 1, currentRow, sectionColumnCount).Style
                        .Font.SetBold()
                        .Font.SetFontSize(12)
                        .Font.SetFontColor(XLColor.FromHtml(NavyColor));
                    currentRow++;

                    if (!string.IsNullOrWhiteSpace(section.Description))
                    {
                        worksheet.Cell(currentRow, 1).Value = section.Description;
                        worksheet.Range(currentRow, 1, currentRow, sectionColumnCount).Merge();
                        worksheet.Range(currentRow, 1, currentRow, sectionColumnCount).Style
                            .Font.SetItalic()
                            .Font.SetFontColor(XLColor.DimGray)
                            .Alignment.SetWrapText();
                        currentRow++;
                    }

                    var headerRow = currentRow;
                    if (firstHeaderRow == 6)
                    {
                        firstHeaderRow = headerRow;
                    }
                    var columnIndex = 1;
                    if (section.IncludeRowNumber)
                    {
                        ApplyExcelHeader(worksheet.Cell(headerRow, columnIndex++), "STT");
                    }
                    foreach (var column in columns)
                    {
                        ApplyExcelHeader(worksheet.Cell(headerRow, columnIndex++), column.Header);
                    }

                    for (var rowIndex = 0; rowIndex < section.Rows.Count; rowIndex++)
                    {
                        var sheetRow = headerRow + rowIndex + 1;
                        columnIndex = 1;
                        if (section.IncludeRowNumber)
                        {
                            var numberCell = worksheet.Cell(sheetRow, columnIndex++);
                            numberCell.Value = rowIndex + 1;
                            ApplyExcelDataStyle(numberCell, ExportColumnAlignment.Center, false);
                        }

                        foreach (var column in columns)
                        {
                            var cell = worksheet.Cell(sheetRow, columnIndex++);
                            var selectedValue = column.ValueSelector(section.Rows[rowIndex]);
                            var (value, numberFormat) = ResolveCellValue(selectedValue, column.NumberFormat);
                            SetCellValue(cell, NormalizeMoneyForDisplay(value, numberFormat));
                            if (!string.IsNullOrWhiteSpace(numberFormat))
                            {
                                cell.Style.NumberFormat.Format = numberFormat;
                            }
                            ApplyExcelDataStyle(cell, column.Alignment, column.WrapText);
                        }
                    }

                    var lastDataRow = headerRow + section.Rows.Count;
                    if (section.Rows.Count > 0
                        && section.EnableAutoFilter
                        && sheetSections.Count == 1)
                    {
                        worksheet.Range(headerRow, 1, lastDataRow, sectionColumnCount).SetAutoFilter();
                    }

                    ApplyExcelColumnWidths(worksheet, columns, section.IncludeRowNumber, headerRow, lastDataRow);

                    currentRow = Math.Max(headerRow + 1, lastDataRow + 1);
                    foreach (var summary in section.Summaries)
                    {
                        worksheet.Cell(currentRow, 1).Value = summary.Label;
                        worksheet.Cell(currentRow, 1).Style.Font.SetBold();
                        var valueCell = worksheet.Cell(currentRow, Math.Min(2, sectionColumnCount));
                        SetCellValue(valueCell, NormalizeMoneyForDisplay(summary.Value, summary.NumberFormat));
                        if (!string.IsNullOrWhiteSpace(summary.NumberFormat))
                        {
                            valueCell.Style.NumberFormat.Format = summary.NumberFormat;
                        }
                        valueCell.Style.Font.SetBold();
                        valueCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        currentRow++;
                    }

                    currentRow += 2;
                }

                worksheet.SheetView.FreezeRows(sheetSections.Count == 1 ? firstHeaderRow : 6);
                if (request.FreezeColumns > 0)
                {
                    worksheet.SheetView.FreezeColumns(request.FreezeColumns);
                }
                if (sheetSections.Count == 1)
                {
                    var headerRow = string.IsNullOrWhiteSpace(sheetSections[0].Description) ? 8 : 9;
                    worksheet.PageSetup.SetRowsToRepeatAtTop(headerRow, headerRow);
                }
                worksheet.PageSetup.PageOrientation = request.PdfLandscape
                    ? XLPageOrientation.Landscape
                    : XLPageOrientation.Portrait;
                worksheet.PageSetup.FitToPages(1, 0);
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static byte[] BuildPdf(ExportFileRequest request)
        {
            var sections = GetSections(request);
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(24);
                    page.Size(request.PdfLandscape ? PageSizes.A4.Landscape() : PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(request.PdfLandscape ? 8 : 9));

                    page.Header().Column(header =>
                    {
                        header.Spacing(3);
                        header.Item().AlignCenter().Text(request.ReportTitle).Bold().FontSize(15).FontColor(NavyColor);
                        header.Item().Text($"Thời gian xuất: {request.ExportedAt:dd/MM/yyyy HH:mm:ss}");
                        header.Item().Text($"Người xuất: {ResolveExporter(request.ExportedBy)}");
                        header.Item().Text($"Bộ lọc: {BuildAppliedFiltersText(request.AppliedFiltersText)}");
                        header.Item().Text($"Phạm vi: {BuildDataScopeText(request.DataScopeText)} | Tổng số dòng: {GetTotalRows(request):N0}");
                        header.Item().PaddingBottom(6).LineHorizontal(1).LineColor(BorderColor);
                    });

                    page.Content().PaddingTop(8).Column(content =>
                    {
                        content.Spacing(10);
                        foreach (var section in sections)
                        {
                            var columns = GetColumns(section.Columns, ExportFileFormat.Pdf);
                            content.Item().PaddingTop(4).Text(section.Title).Bold().FontSize(11).FontColor(NavyColor);
                            if (!string.IsNullOrWhiteSpace(section.Description))
                            {
                                content.Item().Text(section.Description).Italic().FontColor(Colors.Grey.Darken1);
                            }

                            content.Item().Table(table =>
                            {
                                table.ColumnsDefinition(definition =>
                                {
                                    definition.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.White).Element(cell =>
                                        ComposePdfTableRow(
                                            cell,
                                            columns,
                                            section.IncludeRowNumber,
                                            true,
                                            null,
                                            0));
                                });

                                for (var rowIndex = 0; rowIndex < section.Rows.Count; rowIndex++)
                                {
                                    var currentRow = section.Rows[rowIndex];
                                    var currentRowNumber = rowIndex + 1;
                                    table.Cell()
                                        .Background(Colors.White)
                                        .ShowEntire()
                                        .Element(cell => ComposePdfTableRow(
                                            cell,
                                            columns,
                                            section.IncludeRowNumber,
                                            false,
                                            currentRow,
                                            currentRowNumber));
                                }
                            });

                            foreach (var summary in section.Summaries)
                            {
                                content.Item().AlignRight().Text(
                                    $"{summary.Label}: {FormatDisplayValue(summary.Value, summary.NumberFormat)}").SemiBold();
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Trang ");
                        text.CurrentPageNumber();
                        text.Span("/");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        private static byte[] BuildCsv(ExportFileRequest request)
        {
            var columns = GetColumns(request.Columns, ExportFileFormat.Csv);
            var sb = new StringBuilder();
            var headers = new List<string>();
            if (request.IncludeRowNumber)
            {
                headers.Add("STT");
            }
            headers.AddRange(columns.Select(x => x.Header));
            sb.AppendLine(string.Join(",", headers.Select(EscapeCsv)));

            for (var rowIndex = 0; rowIndex < request.Rows.Count; rowIndex++)
            {
                var cells = new List<string>();
                if (request.IncludeRowNumber)
                {
                    cells.Add((rowIndex + 1).ToString(CultureInfo.InvariantCulture));
                }
                cells.AddRange(columns.Select(column =>
                    FormatCsvValue(UnwrapCellValue(column.ValueSelector(request.Rows[rowIndex])))));
                sb.AppendLine(string.Join(",", cells.Select(EscapeCsv)));
            }

            var content = Encoding.UTF8.GetBytes(sb.ToString());
            var bom = Encoding.UTF8.GetPreamble();
            var result = new byte[bom.Length + content.Length];
            Buffer.BlockCopy(bom, 0, result, 0, bom.Length);
            Buffer.BlockCopy(content, 0, result, bom.Length, content.Length);
            return result;
        }

        private static List<ExportSectionDefinition> GetSections(ExportFileRequest request)
        {
            if (request.Sections.Count > 0)
            {
                return request.Sections;
            }

            return
            [
                new ExportSectionDefinition
                {
                    Title = request.ReportTitle,
                    SheetName = request.SheetName,
                    IncludeRowNumber = request.IncludeRowNumber,
                    Columns = request.Columns,
                    Summaries = request.Summaries,
                    Rows = request.Rows
                }
            ];
        }

        private static List<ExportColumnDefinition> GetColumns(
            IEnumerable<ExportColumnDefinition> columns,
            ExportFileFormat format)
            => columns.Where(column => format switch
            {
                ExportFileFormat.Excel => column.ShowInExcel,
                ExportFileFormat.Pdf => column.ShowInPdf,
                ExportFileFormat.Csv => column.ShowInCsv,
                _ => true
            }).ToList();

        private static int GetExcelColumnCount(ExportSectionDefinition section)
            => Math.Max(1,
                GetColumns(section.Columns, ExportFileFormat.Excel).Count + (section.IncludeRowNumber ? 1 : 0));

        private static int GetTotalRows(ExportFileRequest request)
            => request.Sections.Count > 0
                ? request.Sections.Sum(x => x.Rows.Count)
                : request.Rows.Count;

        private static void WriteExcelReportMetadata(
            IXLWorksheet worksheet,
            ExportFileRequest request,
            int totalColumns)
        {
            worksheet.Cell(1, 1).Value = request.ReportTitle;
            worksheet.Range(1, 1, 1, totalColumns).Merge();
            worksheet.Range(1, 1, 1, totalColumns).Style
                .Font.SetBold()
                .Font.SetFontSize(16)
                .Font.SetFontColor(XLColor.FromHtml(NavyColor))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Row(1).Height = 26;

            SetMetadataRow(worksheet, 2, totalColumns, $"Thời gian xuất: {request.ExportedAt:dd/MM/yyyy HH:mm:ss}");
            SetMetadataRow(worksheet, 3, totalColumns, $"Người xuất: {ResolveExporter(request.ExportedBy)}");
            SetMetadataRow(worksheet, 4, totalColumns, $"Bộ lọc: {BuildAppliedFiltersText(request.AppliedFiltersText)}");
            SetMetadataRow(worksheet, 5, totalColumns,
                $"Phạm vi: {BuildDataScopeText(request.DataScopeText)} | Tổng số dòng: {GetTotalRows(request):N0}");
        }

        private static void SetMetadataRow(IXLWorksheet worksheet, int row, int totalColumns, string value)
        {
            worksheet.Cell(row, 1).Value = value;
            worksheet.Range(row, 1, row, totalColumns).Merge();
            worksheet.Range(row, 1, row, totalColumns).Style
                .Font.SetFontColor(XLColor.DimGray)
                .Alignment.SetWrapText()
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        }

        private static void ApplyExcelHeader(IXLCell cell, string header)
        {
            cell.Value = header;
            cell.Style
                .Font.SetBold()
                .Font.SetFontColor(XLColor.FromHtml(NavyColor))
                .Fill.SetBackgroundColor(XLColor.FromHtml(HeaderColor))
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.FromHtml(BorderColor))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Alignment.SetWrapText();
        }

        private static void ApplyExcelDataStyle(
            IXLCell cell,
            ExportColumnAlignment alignment,
            bool wrapText)
        {
            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            cell.Style.Border.SetOutsideBorderColor(XLColor.FromHtml(BorderColor));
            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            cell.Style.Alignment.SetWrapText(wrapText);
            cell.Style.Alignment.SetHorizontal(alignment switch
            {
                ExportColumnAlignment.Center => XLAlignmentHorizontalValues.Center,
                ExportColumnAlignment.Right => XLAlignmentHorizontalValues.Right,
                _ => XLAlignmentHorizontalValues.Left
            });
        }

        private static void ApplyExcelColumnWidths(
            IXLWorksheet worksheet,
            IReadOnlyList<ExportColumnDefinition> columns,
            bool includeRowNumber,
            int headerRow,
            int lastDataRow)
        {
            var columnIndex = 1;
            if (includeRowNumber)
            {
                worksheet.Column(columnIndex++).Width = 7;
            }

            foreach (var column in columns)
            {
                var worksheetColumn = worksheet.Column(columnIndex++);
                worksheetColumn.AdjustToContents(headerRow, Math.Max(headerRow, lastDataRow));
                worksheetColumn.Width = Math.Clamp(worksheetColumn.Width, column.MinWidth, column.MaxWidth);
            }
        }

        private static void SetCellValue(IXLCell cell, object? value)
        {
            if (value == null)
            {
                cell.Clear(XLClearOptions.Contents);
                return;
            }

            switch (value)
            {
                case DateTime dateTime:
                    cell.Value = dateTime;
                    break;
                case DateOnly dateOnly:
                    cell.Value = dateOnly.ToDateTime(TimeOnly.MinValue);
                    break;
                case decimal decimalValue:
                    cell.Value = decimalValue;
                    break;
                case double doubleValue:
                    cell.Value = doubleValue;
                    break;
                case float floatValue:
                    cell.Value = floatValue;
                    break;
                case long longValue:
                    cell.Value = longValue;
                    break;
                case int intValue:
                    cell.Value = intValue;
                    break;
                case short shortValue:
                    cell.Value = shortValue;
                    break;
                case bool boolValue:
                    cell.Value = boolValue;
                    break;
                default:
                    cell.Value = Convert.ToString(value, CultureInfo.GetCultureInfo("vi-VN")) ?? string.Empty;
                    break;
            }
        }

        private static object? NormalizeMoneyForDisplay(object? value, string? numberFormat)
        {
            if (!IsCurrencyFormat(numberFormat) || value == null)
            {
                return value;
            }

            return value switch
            {
                decimal number => Math.Round(number, 0, MidpointRounding.AwayFromZero),
                double number => Math.Round(number, 0, MidpointRounding.AwayFromZero),
                float number => MathF.Round(number, 0, MidpointRounding.AwayFromZero),
                _ => value
            };
        }

        private static (object? Value, string? NumberFormat) ResolveCellValue(
            object? selectedValue,
            string? columnNumberFormat)
            => selectedValue is ExportCellValue cellValue
                ? (cellValue.Value, cellValue.NumberFormat ?? columnNumberFormat)
                : (selectedValue, columnNumberFormat);

        private static object? UnwrapCellValue(object? selectedValue)
            => selectedValue is ExportCellValue cellValue ? cellValue.Value : selectedValue;

        private static void ComposePdfHeader(
            IContainer container,
            string value,
            ExportColumnAlignment alignment)
        {
            var styled = container
                .Background(HeaderColor)
                .Border(0.5f)
                .BorderColor(BorderColor)
                .Padding(4);
            AlignPdf(styled, alignment).Text(value).SemiBold().FontColor(NavyColor);
        }

        private static void ComposePdfDataCell(
            IContainer container,
            string value,
            ExportColumnAlignment alignment)
        {
            var styled = container
                .Background(Colors.White)
                .BorderBottom(0.5f)
                .BorderColor(BorderColor)
                .PaddingVertical(3)
                .PaddingHorizontal(4);
            AlignPdf(styled, alignment).Text(value);
        }

        private static void ComposePdfTableRow(
            IContainer container,
            IReadOnlyList<ExportColumnDefinition> columns,
            bool includeRowNumber,
            bool isHeader,
            object? row,
            int rowNumber)
        {
            container.Table(innerTable =>
            {
                innerTable.ColumnsDefinition(definition =>
                {
                    if (includeRowNumber)
                    {
                        definition.ConstantColumn(28);
                    }
                    foreach (var column in columns)
                    {
                        definition.RelativeColumn(Math.Max(0.25f, column.PdfRelativeWidth));
                    }
                });

                if (includeRowNumber)
                {
                    if (isHeader)
                    {
                        ComposePdfHeader(innerTable.Cell(), "STT", ExportColumnAlignment.Center);
                    }
                    else
                    {
                        ComposePdfDataCell(
                            innerTable.Cell(),
                            rowNumber.ToString(CultureInfo.InvariantCulture),
                            ExportColumnAlignment.Center);
                    }
                }

                foreach (var column in columns)
                {
                    if (isHeader)
                    {
                        ComposePdfHeader(innerTable.Cell(), column.Header, column.Alignment);
                        continue;
                    }

                    var selectedValue = column.ValueSelector(row!);
                    var (value, numberFormat) = ResolveCellValue(selectedValue, column.NumberFormat);
                    ComposePdfDataCell(
                        innerTable.Cell(),
                        FormatDisplayValue(value, numberFormat),
                        column.Alignment);
                }
            });
        }

        private static IContainer AlignPdf(IContainer container, ExportColumnAlignment alignment)
            => alignment switch
            {
                ExportColumnAlignment.Center => container.AlignCenter(),
                ExportColumnAlignment.Right => container.AlignRight(),
                _ => container.AlignLeft()
            };

        private static string FormatDisplayValue(object? value, string? numberFormat)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is DateTime dateTime)
            {
                return string.Equals(numberFormat, "dd/MM/yyyy", StringComparison.Ordinal)
                    ? dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                    : dateTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            }

            var displayValue = NormalizeMoneyForDisplay(value, numberFormat);
            if (displayValue is IFormattable formattable && !string.IsNullOrWhiteSpace(numberFormat))
            {
                if (IsCurrencyFormat(numberFormat))
                {
                    return $"{formattable.ToString("#,##0", CultureInfo.GetCultureInfo("vi-VN"))} VNĐ";
                }
                if (numberFormat.Contains('%'))
                {
                    return $"{formattable.ToString("0.##", CultureInfo.InvariantCulture)}%";
                }
                return formattable.ToString(numberFormat, CultureInfo.GetCultureInfo("vi-VN")) ?? string.Empty;
            }

            return Convert.ToString(displayValue, CultureInfo.GetCultureInfo("vi-VN")) ?? string.Empty;
        }

        private static string FormatCsvValue(object? value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value switch
            {
                DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateOnly dateOnly => dateOnly.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                decimal number => number.ToString("0.############################", CultureInfo.InvariantCulture),
                double number => number.ToString("R", CultureInfo.InvariantCulture),
                float number => number.ToString("R", CultureInfo.InvariantCulture),
                byte or sbyte or short or ushort or int or uint or long or ulong
                    => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty,
                bool boolean => boolean ? "true" : "false",
                _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
            };
        }

        private static bool IsCurrencyFormat(string? numberFormat)
            => !string.IsNullOrWhiteSpace(numberFormat)
               && numberFormat.Contains("VNĐ", StringComparison.OrdinalIgnoreCase);

        private static string BuildAppliedFiltersText(string? filters)
            => string.IsNullOrWhiteSpace(filters) ? "Không áp dụng bộ lọc" : filters.Trim();

        private static string BuildDataScopeText(string? scope)
            => string.IsNullOrWhiteSpace(scope) ? "Theo quyền truy cập hiện tại" : scope.Trim();

        private static string ResolveExporter(string? exportedBy)
            => string.IsNullOrWhiteSpace(exportedBy) ? "Không xác định" : exportedBy.Trim();

        private static string EscapeCsv(string? value)
        {
            var input = value ?? string.Empty;
            if (input.Contains(',') || input.Contains('"') || input.Contains('\n') || input.Contains('\r'))
            {
                return $"\"{input.Replace("\"", "\"\"")}\"";
            }
            return input;
        }

        private static string NormalizeSheetName(string? input)
        {
            var name = string.IsNullOrWhiteSpace(input) ? "BaoCao" : input.Trim();
            foreach (var invalid in new[] { ':', '\\', '/', '?', '*', '[', ']' })
            {
                name = name.Replace(invalid, '-');
            }
            return name.Length > 31 ? name[..31] : name;
        }

        private static string NormalizeFileNamePart(string? input, string fallback)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return fallback;
            }

            var normalized = input.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);
            foreach (var character in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(character);
                if (category == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (character == 'đ' || character == 'Đ')
                {
                    builder.Append(character == 'đ' ? 'd' : 'D');
                }
                else if (char.IsLetterOrDigit(character) || character is '_' or '-')
                {
                    builder.Append(character);
                }
                else if (char.IsWhiteSpace(character))
                {
                    builder.Append('_');
                }
            }

            var result = builder.ToString().Trim('_', '-');
            return string.IsNullOrWhiteSpace(result) ? fallback : result;
        }
    }
}
