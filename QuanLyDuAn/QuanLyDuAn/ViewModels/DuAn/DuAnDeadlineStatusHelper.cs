using QuanLyDuAn.Constants;

namespace QuanLyDuAn.ViewModels.DuAn
{
    public static class DuAnDeadlineStatusHelper
    {
        public const string FilterDangQuaHan = "DangQuaHan";
        public const string FilterCoCongViecTre = "CoCongViecTre";
        public const string FilterHoanThanhTre = "HoanThanhTre";
        public const string FilterHoanThanhDungHan = "HoanThanhDungHan";
        public const string FilterConHan = "ConHan";

        public const string StatusQuaHan = "QuaHan";
        public const string StatusCongViecTre = "CongViecTre";
        public const string StatusHoanThanhTre = "HoanThanhTre";
        public const string StatusHoanThanhDungHan = "HoanThanhDungHan";
        public const string StatusConHan = "ConHan";
        public const string StatusChuaXacDinh = "ChuaXacDinh";
        public const string StatusKhongDanhGia = "KhongDanhGia";

        public static string? NormalizeFilter(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim() switch
            {
                FilterDangQuaHan => FilterDangQuaHan,
                FilterCoCongViecTre => FilterCoCongViecTre,
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
                FilterDangQuaHan => "Đang quá hạn",
                FilterCoCongViecTre => "Có công việc trễ",
                FilterHoanThanhTre => "Hoàn thành trễ",
                FilterHoanThanhDungHan => "Hoàn thành đúng hạn",
                FilterConHan => "Còn hạn",
                _ => "Tất cả tình trạng"
            };
        }

        public static void Apply(DuAnViewModel duAn, DateTime today)
        {
            duAn.IsQuaHan = false;
            duAn.IsHoanThanhTre = false;
            duAn.IsHoanThanhDungHan = false;
            duAn.CoCongViecTre = duAn.SoCongViecTre > 0;
            duAn.IsConHan = false;
            duAn.IsChuaXacDinh = false;
            duAn.IsKhongDanhGia = false;
            duAn.SoNgayTre = 0;

            if (TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.DaHuy))
            {
                SetStatus(duAn, StatusKhongDanhGia, "Không đánh giá", "is-neutral");
                duAn.IsKhongDanhGia = true;
                return;
            }

            if (!duAn.NgayKetThucDuAn.HasValue)
            {
                SetStatus(duAn, StatusChuaXacDinh, "Chưa xác định", "is-unknown");
                duAn.IsChuaXacDinh = true;
                return;
            }

            var ngayKetThuc = duAn.NgayKetThucDuAn.Value.Date;
            var laTrangThaiDaKetThuc = TrangThai.LaHoanThanhCongViec(duAn.TrangThaiDuAn)
                                      || TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.LuuTru);

            if (laTrangThaiDaKetThuc)
            {
                if (duAn.NgayHoanThanhThucTeDuAn.HasValue)
                {
                    var ngayHoanThanh = duAn.NgayHoanThanhThucTeDuAn.Value.Date;
                    if (ngayHoanThanh > ngayKetThuc)
                    {
                        duAn.IsHoanThanhTre = true;
                        duAn.SoNgayTre = (ngayHoanThanh - ngayKetThuc).Days;
                        SetStatus(duAn, StatusHoanThanhTre, $"Hoàn thành trễ {duAn.SoNgayTre} ngày", "is-late");
                        return;
                    }

                    duAn.IsHoanThanhDungHan = true;
                    SetStatus(duAn, StatusHoanThanhDungHan, "Hoàn thành đúng hạn", "is-on-time");
                    return;
                }

                if (duAn.SoCongViecTre > 0)
                {
                    SetWorkLateStatus(duAn);
                    return;
                }

                SetStatus(duAn, StatusChuaXacDinh, "Chưa xác định", "is-unknown");
                duAn.IsChuaXacDinh = true;
                return;
            }

            if (today.Date > ngayKetThuc)
            {
                duAn.IsQuaHan = true;
                duAn.SoNgayTre = (today.Date - ngayKetThuc).Days;
                var prefix = TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.TamDung)
                             || TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.ChoXacNhanHoanThanh)
                    ? "Quá hạn"
                    : "Trễ";
                SetStatus(duAn, StatusQuaHan, $"{prefix} {duAn.SoNgayTre} ngày", "is-late");
                return;
            }

            if (duAn.SoCongViecTre > 0)
            {
                SetWorkLateStatus(duAn);
                return;
            }

            duAn.IsConHan = true;
            SetStatus(duAn, StatusConHan, "Còn hạn", "is-on-time");
        }

        private static void SetWorkLateStatus(DuAnViewModel duAn)
        {
            duAn.CoCongViecTre = true;
            var text = duAn.SoCongViecTre == 1 ? "1 công việc trễ" : $"{duAn.SoCongViecTre} công việc trễ";
            SetStatus(duAn, StatusCongViecTre, text, "is-work-late");
        }

        private static void SetStatus(DuAnViewModel duAn, string code, string text, string css)
        {
            duAn.MaTinhTrangThoiHan = code;
            duAn.TinhTrangThoiHan = text;
            duAn.CssTinhTrangThoiHan = css;
        }
    }
}
