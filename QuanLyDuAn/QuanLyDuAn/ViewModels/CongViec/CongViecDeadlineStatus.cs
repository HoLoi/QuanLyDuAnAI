namespace QuanLyDuAn.ViewModels.CongViec
{
    public static class CongViecDeadlineStatus
    {
        public const string FilterQuaHan = "QuaHan";
        public const string FilterChoXacNhanTre = "ChoXacNhanTre";
        public const string FilterHoanThanhTre = "HoanThanhTre";
        public const string FilterHoanThanhDungHan = "HoanThanhDungHan";
        public const string FilterConHan = "ConHan";

        public const string ConHan = "ConHan";
        public const string QuaHan = "QuaHan";
        public const string HoanTatDungHan = "HoanTatDungHan";
        public const string HoanTatTre = "HoanTatTre";
        public const string HoanThanhDungHan = "HoanThanhDungHan";
        public const string HoanThanhTre = "HoanThanhTre";
        public const string KhongDanhGia = "KhongDanhGia";
        public const string ChuaXacDinh = "ChuaXacDinh";

        public static string? NormalizeFilter(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim() switch
            {
                FilterQuaHan => FilterQuaHan,
                FilterChoXacNhanTre => FilterChoXacNhanTre,
                FilterHoanThanhTre => FilterHoanThanhTre,
                FilterHoanThanhDungHan => FilterHoanThanhDungHan,
                FilterConHan => FilterConHan,
                _ => null
            };
        }

        public static string ToDisplayFilter(string? value)
        {
            return NormalizeFilter(value) switch
            {
                FilterQuaHan => "Đang quá hạn",
                FilterChoXacNhanTre => "Chờ xác nhận trễ",
                FilterHoanThanhTre => "Hoàn thành trễ",
                FilterHoanThanhDungHan => "Hoàn thành đúng hạn",
                FilterConHan => "Còn hạn",
                _ => "Tất cả tình trạng"
            };
        }
    }
}
