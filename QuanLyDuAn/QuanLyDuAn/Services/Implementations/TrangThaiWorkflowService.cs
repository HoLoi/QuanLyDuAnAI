using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Services.Implementations
{
    public class TrangThaiWorkflowService : ITrangThaiWorkflowService
    {
        private readonly QuanLyDuAnDbContext _context;

        public TrangThaiWorkflowService(QuanLyDuAnDbContext context)
        {
            _context = context;
        }

        public async Task DongBoChuoiTrangThaiTuCongViecAsync(int maCongViec, int? maNguoiDungThucHien = null, string? nguonCapNhat = null)
        {
            await DongBoTrangThaiCongViecTheoChiTietAsync(maCongViec, maNguoiDungThucHien, nguonCapNhat);

            var maDuAn = await LayMaDuAnTheoCongViecAsync(maCongViec);
            if (maDuAn > 0)
            {
                await DongBoTrangThaiDuAnTheoCongViecAsync(maDuAn, maNguoiDungThucHien, nguonCapNhat);
            }
        }

        public async Task DongBoTrangThaiCongViecTheoChiTietAsync(int maCongViec, int? maNguoiDungThucHien = null, string? nguonCapNhat = null)
        {
            var congViec = await _context.CongViec
                .FirstOrDefaultAsync(x => x.MaCongViec == maCongViec && x.IsDeleted != true);

            if (congViec == null)
                return;

            var trangThaiHienTai = TrangThai.ToCode(congViec.TrangThaiCongViec);
            if (TrangThai.LaHoanThanhCongViec(trangThaiHienTai)
                || TrangThai.EqualsValue(trangThaiHienTai, TrangThai.TamDung)
                || TrangThai.EqualsValue(trangThaiHienTai, TrangThai.DaHuy))
            {
                return;
            }

            var trangThaiChiTiet = await _context.CtCongViec
                .Where(x => x.MaCongViec == maCongViec && x.IsDeleted != true)
                .Select(x => TrangThai.ToCode(x.TrangThaiCTCV))
                .ToListAsync();

            var trangThaiMoi = TinhTrangThaiCongViecTheoChiTiet(trangThaiChiTiet);

            if (!TrangThai.EqualsValue(trangThaiHienTai, trangThaiMoi))
            {
                if (TrangThai.EqualsValue(trangThaiHienTai, TrangThai.ChoXacNhanHoanThanh)
                    && !TrangThai.EqualsValue(trangThaiMoi, TrangThai.ChoXacNhanHoanThanh))
                {
                    await ThemNhatKyDuAnNeuCanAsync(
                        maCongViec,
                        maNguoiDungThucHien,
                        $"Tự động đưa công việc #{maCongViec} về trạng thái {TrangThai.ToDisplay(trangThaiMoi)} do phát sinh thay đổi chi tiết. Nguồn: {nguonCapNhat ?? "Hệ thống"}");
                }

                congViec.TrangThaiCongViec = trangThaiMoi;
            }

            if (TrangThai.EqualsValue(trangThaiMoi, TrangThai.ChoXacNhanHoanThanh) || TrangThai.LaHoanThanhCongViec(trangThaiMoi))
            {
                if (!congViec.NgayKetThucCVThucTe.HasValue)
                    congViec.NgayKetThucCVThucTe = DateTime.Now;
            }
            else
            {
                congViec.NgayKetThucCVThucTe = null;
            }
        }

        public async Task DongBoTrangThaiDuAnTheoCongViecAsync(int maDuAn, int? maNguoiDungThucHien = null, string? nguonCapNhat = null)
        {
            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (duAn == null)
                return;

            var trangThaiDuAnHienTai = TrangThai.ToCode(duAn.TrangThaiDuAn);
            var trangThaiCongViec = await (
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where cv.IsDeleted != true
                      && dm.IsDeleted != true
                      && dm.MaDuAn == maDuAn
                select TrangThai.ToCode(cv.TrangThaiCongViec)
            ).ToListAsync();

            duAn.PhanTramHoanThanh = TinhPhanTramDuAnTheoCongViec(trangThaiCongViec);

            if (TrangThai.LaHoanThanhCongViec(trangThaiDuAnHienTai)
                || TrangThai.EqualsValue(trangThaiDuAnHienTai, TrangThai.LuuTru)
                || TrangThai.EqualsValue(trangThaiDuAnHienTai, TrangThai.DaHuy)
                || TrangThai.EqualsValue(trangThaiDuAnHienTai, TrangThai.TamDung))
            {
                return;
            }

            var tatCaCongViecHoanThanh = trangThaiCongViec.Count > 0 && trangThaiCongViec.All(TrangThai.LaHoanThanhCongViec);
            var canRollbackChoXacNhan = TrangThai.EqualsValue(trangThaiDuAnHienTai, TrangThai.ChoXacNhanHoanThanh) && !tatCaCongViecHoanThanh;
            var canLenChoXacNhan = !TrangThai.EqualsValue(trangThaiDuAnHienTai, TrangThai.ChoXacNhanHoanThanh) && tatCaCongViecHoanThanh;

            if (canRollbackChoXacNhan)
            {
                duAn.TrangThaiDuAn = TrangThai.DangThucHien;
                if (maNguoiDungThucHien.HasValue && maNguoiDungThucHien.Value > 0)
                {
                    _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
                    {
                        MaDuAn = maDuAn,
                        MaNguoiDung = maNguoiDungThucHien.Value,
                        NkHanhDongQLDA = $"Tự động đưa dự án về trạng thái {TrangThai.ToDisplay(TrangThai.DangThucHien)} do phát sinh công việc chưa hoàn thành. Nguồn: {nguonCapNhat ?? "Hệ thống"}",
                        NkThoiGianQLDA = DateTime.Now
                    });
                }
            }
            else if (canLenChoXacNhan)
            {
                duAn.TrangThaiDuAn = TrangThai.ChoXacNhanHoanThanh;
            }
        }

        private static string TinhTrangThaiCongViecTheoChiTiet(List<string> trangThaiChiTiet)
        {
            if (trangThaiChiTiet.Count == 0)
                return TrangThai.ChuaBatDau;

            if (trangThaiChiTiet.All(TrangThai.LaHoanThanhCongViec))
                return TrangThai.ChoXacNhanHoanThanh;

            if (trangThaiChiTiet.Any(x => TrangThai.EqualsValue(x, TrangThai.BiCanCan)))
                return TrangThai.BiCanCan;

            var coTienTrien = trangThaiChiTiet.Any(x =>
                TrangThai.EqualsValue(x, TrangThai.DangThucHien)
                || TrangThai.EqualsValue(x, TrangThai.ChoXacNhanHoanThanh)
                || TrangThai.EqualsValue(x, TrangThai.TamDung)
                || TrangThai.EqualsValue(x, TrangThai.DaHuy)
                || !TrangThai.EqualsValue(x, TrangThai.ChuaBatDau));

            return coTienTrien ? TrangThai.DangThucHien : TrangThai.ChuaBatDau;
        }

        private static int TinhPhanTramDuAnTheoCongViec(List<string> trangThaiCongViec)
        {
            if (trangThaiCongViec.Count == 0)
                return 0;

            var phanTram = trangThaiCongViec
                .Select(TinhPhanTramTheoTrangThaiCongViec)
                .ToList();

            return phanTram.Count == 0 ? 0 : (int)Math.Round(phanTram.Average());
        }

        private static int TinhPhanTramTheoTrangThaiCongViec(string? trangThai)
        {
            if (TrangThai.LaHoanThanhCongViec(trangThai))
                return 100;

            if (TrangThai.EqualsValue(trangThai, TrangThai.ChoXacNhanHoanThanh))
                return 90;

            if (TrangThai.EqualsValue(trangThai, TrangThai.DangThucHien)
                || TrangThai.EqualsValue(trangThai, TrangThai.BiCanCan)
                || TrangThai.EqualsValue(trangThai, TrangThai.TamDung))
                return 50;

            return 0;
        }

        private async Task<int> LayMaDuAnTheoCongViecAsync(int maCongViec)
        {
            return await (
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where cv.MaCongViec == maCongViec
                      && cv.IsDeleted != true
                      && dm.IsDeleted != true
                select dm.MaDuAn
            ).FirstOrDefaultAsync();
        }

        private async Task ThemNhatKyDuAnNeuCanAsync(int maCongViec, int? maNguoiDungThucHien, string noiDung)
        {
            if (!maNguoiDungThucHien.HasValue || maNguoiDungThucHien.Value <= 0)
                return;

            var maDuAn = await LayMaDuAnTheoCongViecAsync(maCongViec);
            if (maDuAn <= 0)
                return;

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = maDuAn,
                MaNguoiDung = maNguoiDungThucHien.Value,
                NkHanhDongQLDA = noiDung,
                NkThoiGianQLDA = DateTime.Now
            });
        }
    }
}
