using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace QuanLyDuAn.Helpers
{
    public static class ExportSupport
    {
        public static string BuildFiltersText(params (string Label, string? Value)[] filters)
        {
            var parts = filters
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => $"{x.Label}: {x.Value!.Trim()}");

            return string.Join("; ", parts);
        }

        public static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : string.Empty;
        }

        public static string FormatDateTime(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) : string.Empty;
        }

        public static string FormatCurrency(decimal? value)
        {
            return value.HasValue ? string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0} VNĐ", value.Value) : string.Empty;
        }

        public static string FormatNumber(double? value, string format = "0.##")
        {
            return value.HasValue ? value.Value.ToString(format, CultureInfo.InvariantCulture) : string.Empty;
        }

        public static string ResolveExporterName(ClaimsPrincipal user)
        {
            return user?.FindFirstValue(ClaimTypes.Name)
                ?? user?.Identity?.Name
                ?? "Không xác định";
        }

        public static string ResolveTextOrDefault(string? value, string fallback = "Tất cả")
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        public static string ToDisplayFilterValue(string? value, string fallback = "Tất cả")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            return value.Trim().ToLowerInvariant() switch
            {
                "tatca" => "Tất cả",
                "homnay" => "Hôm nay",
                "7ngay" => "7 ngày gần đây",
                "thangnay" => "Tháng này",
                "quynay" => "Quý này",
                "namnay" => "Năm nay",
                "ngaytao" => "Ngày tạo",
                "ngaybatdau" => "Ngày bắt đầu",
                "ngayketthuc" => "Ngày kết thúc",
                "hancongviec" => "Hạn công việc",
                _ => value.Trim()
            };
        }

        public static string NormalizeFileNamePart(string? value, string fallback = "File")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            var normalized = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);
            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
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
