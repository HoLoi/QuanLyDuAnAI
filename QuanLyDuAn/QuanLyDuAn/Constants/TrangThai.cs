using System.Globalization;
using System.Text;

namespace QuanLyDuAn.Constants
{
    public static class TrangThai
    {
        public const string ChoDuyet = "ChoDuyet";
        public const string DaDuyet = "DaDuyet";
        public const string TuChoi = "TuChoi";
        public const string DaHuy = "DaHuy";

        public const string ChoDuyetHienThi = "Chờ duyệt";
        public const string DaDuyetHienThi = "Đã duyệt";
        public const string TuChoiHienThi = "Từ chối";
        public const string DaHuyHienThi = "Đã hủy";

        public const string ChuaBatDau = "ChuaBatDau";
        public const string ChuaBatDauHienThi = "Chưa bắt đầu";
        public const string DaThayThe = "DaThayThe";
        public const string DaThayTheHienThi = "Đã thay thế";
        public const string GiuNguyen = "GiuNguyen";
        public const string GiuNguyenHienThi = "Giữ nguyên";
        public const string DangThucHien = "DangThucHien";
        public const string DangThucHienHienThi = "Đang thực hiện";
        public const string HoanThanh = "HoanThanh";
        public const string HoanThanhHienThi = "Hoàn thành";
        public const string BiCanCan = "BiCanCan";
        public const string BiCanCanHienThi = "Bị cản cản";
        public const string KhoiTao = "KhoiTao";
        public const string KhoiTaoHienThi = "Khởi tạo";
        public const string ChoXacNhanHoanThanh = "ChoXacNhanHoanThanh";
        public const string ChoXacNhanHoanThanhHienThi = "Chờ xác nhận hoàn thành";
        public const string TamDung = "TamDung";
        public const string TamDungHienThi = "Tạm dừng";
        public const string LuuTru = "Archived";
        public const string LuuTruHienThi = "Lưu trữ";
        public const string HoatDong = "HoatDong";
        public const string HoatDongHienThi = "Hoạt động";
        public const string NgungHoatDong = "NgungHoatDong";
        public const string NgungHoatDongHienThi = "Ngừng hoạt động";
        public const string DangSuDung = "DangSuDung";
        public const string DangSuDungHienThi = "Đang sử dụng";
        public const string TreTienDo = "Tre";
        public const string TreTienDoHienThi = "Trễ tiến độ";
        public const string TaiKhoanHoatDong = "hoatdong";
        public const string TaiKhoanHoatDongHienThi = "Tài khoản hoạt động";
        public const string TaiKhoanKhoa = "khoa";
        public const string TaiKhoanKhoaHienThi = "Tài khoản bị khóa";
        public const string Done = "Done";
        public const string Completed = "Completed";

        public static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var normalized = value.Trim().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var ch in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    if (!char.IsWhiteSpace(ch))
                        builder.Append(char.ToLowerInvariant(ch));
                }
            }

            return builder.ToString();
        }

        public static bool EqualsValue(string? value, string expected)
        {
            return Normalize(value) == Normalize(expected);
        }

        public static string[] GetCommonStatusVariants(string? status)
        {
            var normalized = Normalize(status);

            if (normalized == Normalize(ChoDuyet))
                return new[] { ChoDuyet, ChoDuyetHienThi };

            if (normalized == Normalize(DaDuyet))
                return new[] { DaDuyet, DaDuyetHienThi };

            if (normalized == Normalize(TuChoi))
                return new[] { TuChoi, TuChoiHienThi };

            if (normalized == Normalize(DaHuy))
                return new[] { DaHuy, DaHuyHienThi };

            if (normalized == Normalize(ChuaBatDau))
                return new[] { ChuaBatDau, ChuaBatDauHienThi };

            if (normalized == Normalize(DaThayThe))
                return new[] { DaThayThe, DaThayTheHienThi };

            if (normalized == Normalize(GiuNguyen))
                return new[] { GiuNguyen, GiuNguyenHienThi };

            if (normalized == Normalize(DangThucHien))
                return new[] { DangThucHien, DangThucHienHienThi };

            if (normalized == Normalize(HoanThanh))
                return new[] { HoanThanh, HoanThanhHienThi };

            if (normalized == Normalize(BiCanCan))
                return new[] { BiCanCan, BiCanCanHienThi };

            if (normalized == Normalize(KhoiTao))
                return new[] { KhoiTao, KhoiTaoHienThi };

            if (normalized == Normalize(ChoXacNhanHoanThanh))
                return new[] { ChoXacNhanHoanThanh, ChoXacNhanHoanThanhHienThi };

            if (normalized == Normalize(TamDung))
                return new[] { TamDung, TamDungHienThi };

            if (normalized == Normalize(LuuTru))
                return new[] { LuuTru, LuuTruHienThi };

            if (normalized == Normalize(HoatDong))
                return new[] { HoatDong, HoatDongHienThi };

            if (normalized == Normalize(NgungHoatDong))
                return new[] { NgungHoatDong, NgungHoatDongHienThi };

            if (normalized == Normalize(DangSuDung))
                return new[] { DangSuDung, DangSuDungHienThi };

            if (normalized == Normalize(TreTienDo))
                return new[] { TreTienDo, TreTienDoHienThi };

            return Array.Empty<string>();
        }

        public static string ToCode(string? value)
        {
            var normalized = Normalize(value);

            if (normalized == Normalize(ChoDuyet))
                return ChoDuyet;

            if (normalized == Normalize(DaDuyet))
                return DaDuyet;

            if (normalized == Normalize(TuChoi))
                return TuChoi;

            if (normalized == Normalize(DaHuy))
                return DaHuy;

            if (normalized == Normalize(ChuaBatDau))
                return ChuaBatDau;

            if (normalized == Normalize(DaThayThe))
                return DaThayThe;

            if (normalized == Normalize(GiuNguyen))
                return GiuNguyen;

            if (normalized == Normalize(DangThucHien))
                return DangThucHien;

            if (normalized == Normalize(HoanThanh))
                return HoanThanh;

            if (normalized == Normalize(BiCanCan))
                return BiCanCan;

            if (normalized == Normalize(KhoiTao))
                return KhoiTao;

            if (normalized == Normalize(ChoXacNhanHoanThanh))
                return ChoXacNhanHoanThanh;

            if (normalized == Normalize(TamDung))
                return TamDung;

            if (normalized == Normalize(LuuTru))
                return LuuTru;

            if (normalized == Normalize(HoatDong))
                return HoatDong;

            if (normalized == Normalize(NgungHoatDong))
                return NgungHoatDong;

            if (normalized == Normalize(DangSuDung))
                return DangSuDung;

            if (normalized == Normalize(TreTienDo))
                return TreTienDo;

            if (normalized == Normalize(TaiKhoanHoatDong))
                return TaiKhoanHoatDong;

            if (normalized == Normalize(TaiKhoanKhoa))
                return TaiKhoanKhoa;

            if (normalized == Normalize(Done))
                return Done;

            if (normalized == Normalize(Completed))
                return Completed;

            return value ?? string.Empty;
        }

        public static string ToDisplay(string? value)
        {
            var normalized = Normalize(value);

            if (normalized == Normalize(ChoDuyet))
                return ChoDuyetHienThi;

            if (normalized == Normalize(DaDuyet))
                return DaDuyetHienThi;

            if (normalized == Normalize(TuChoi))
                return TuChoiHienThi;

            if (normalized == Normalize(DaHuy))
                return DaHuyHienThi;

            if (normalized == Normalize(ChuaBatDau))
                return ChuaBatDauHienThi;

            if (normalized == Normalize(DaThayThe))
                return DaThayTheHienThi;

            if (normalized == Normalize(GiuNguyen))
                return GiuNguyenHienThi;

            if (normalized == Normalize(DangThucHien))
                return DangThucHienHienThi;

            if (normalized == Normalize(HoanThanh))
                return HoanThanhHienThi;

            if (normalized == Normalize(BiCanCan))
                return BiCanCanHienThi;

            if (normalized == Normalize(KhoiTao))
                return KhoiTaoHienThi;

            if (normalized == Normalize(ChoXacNhanHoanThanh))
                return ChoXacNhanHoanThanhHienThi;

            if (normalized == Normalize(TamDung))
                return TamDungHienThi;

            if (normalized == Normalize(LuuTru))
                return LuuTruHienThi;

            if (normalized == Normalize(HoatDong))
                return HoatDongHienThi;

            if (normalized == Normalize(DangSuDung))
                return DangSuDungHienThi;

            if (normalized == Normalize(TreTienDo))
                return TreTienDoHienThi;

            return value ?? string.Empty;
        }
    }
}
