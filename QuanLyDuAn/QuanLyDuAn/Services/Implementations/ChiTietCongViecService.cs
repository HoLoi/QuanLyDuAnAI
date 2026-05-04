using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.ChiTietCongViec;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class ChiTietCongViecService : IChiTietCongViecService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChiTietCongViecService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ChiTietCongViecPageViewModel> GetPageAsync(int maCongViec)
        {
            var congViec = await GetCongViecAsync(maCongViec);
            var chiTietMoiNhat = await GetChiTietMoiNhatAsync(maCongViec);

            var vm = new ChiTietCongViecPageViewModel
            {
                CoTheCapNhat = await CoTheCapNhatAsync(congViec),
                CongViec = new ChiTietCongViecSummaryViewModel
                {
                    MaCongViec = congViec.MaCongViec,
                    TenCongViec = congViec.TenCongViec ?? string.Empty,
                    TenTrangThai = TrangThai.ToDisplay(congViec.TrangThaiCongViec),
                    PhanTramHoanThanh = chiTietMoiNhat?.PhanTramHoanThanhCTCV ?? 0
                },
                Form = new ChiTietCongViecCreateUpdateViewModel
                {
                    MaCongViec = maCongViec,
                    NgayBaoCaoCTCV = DateTime.Today,
                    PhanTramHoanThanhCTCV = chiTietMoiNhat?.PhanTramHoanThanhCTCV ?? 0,
                    TrangThaiCTCV = chiTietMoiNhat?.TrangThaiCTCV ?? TrangThai.ChuaBatDau
                }
            };

            vm.DanhSach = await _context.CtCongViec
                .Where(x => x.MaCongViec == maCongViec)
                .OrderByDescending(x => x.NgayBaoCaoCTCV)
                .ThenByDescending(x => x.MaChiTietCV)
                .Select(x => new ChiTietCongViecItemViewModel
                {
                    MaChiTietCV = x.MaChiTietCV,
                    MaCongViec = x.MaCongViec,
                    NoiDungChiTietCV = x.NoiDungChiTietCV ?? string.Empty,
                    NgayTaoCTCV = x.NgayTaoCTCV,
                    NgayBaoCaoCTCV = x.NgayBaoCaoCTCV,
                    PhanTramHoanThanhCTCV = x.PhanTramHoanThanhCTCV,
                    TrangThaiCTCV = x.TrangThaiCTCV ?? string.Empty
                })
                .ToListAsync();

            return vm;
        }

        public async Task AddAsync(ChiTietCongViecCreateUpdateViewModel form)
        {
            ValidateForm(form);
            var congViec = await GetCongViecAsync(form.MaCongViec);
            await KiemTraQuyenCapNhatAsync(congViec);

            var entity = new CtCongViec
            {
                MaCongViec = form.MaCongViec,
                NoiDungChiTietCV = form.NoiDungChiTietCV.Trim(),
                NgayTaoCTCV = DateTime.Now,
                NgayBaoCaoCTCV = form.NgayBaoCaoCTCV,
                PhanTramHoanThanhCTCV = form.PhanTramHoanThanhCTCV,
                TrangThaiCTCV = TrangThai.ToCode(form.TrangThaiCTCV)
            };

            _context.CtCongViec.Add(entity);
            DongBoTrangThaiCongViec(congViec, entity.NgayBaoCaoCTCV, entity.PhanTramHoanThanhCTCV, entity.TrangThaiCTCV);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChiTietCongViecCreateUpdateViewModel form)
        {
            if (form.MaChiTietCV <= 0)
                throw new Exception("Không tìm thấy chi tiết công việc cần cập nhật.");

            ValidateForm(form);
            var congViec = await GetCongViecAsync(form.MaCongViec);
            await KiemTraQuyenCapNhatAsync(congViec);

            var entity = await _context.CtCongViec
                .FirstOrDefaultAsync(x => x.MaChiTietCV == form.MaChiTietCV && x.MaCongViec == form.MaCongViec);

            if (entity == null)
                throw new Exception("Không tìm thấy chi tiết công việc cần cập nhật.");

            entity.NoiDungChiTietCV = form.NoiDungChiTietCV.Trim();
            entity.NgayBaoCaoCTCV = form.NgayBaoCaoCTCV;
            entity.PhanTramHoanThanhCTCV = form.PhanTramHoanThanhCTCV;
            entity.TrangThaiCTCV = TrangThai.ToCode(form.TrangThaiCTCV);

            await DongBoTrangThaiTuBanGhiMoiNhatAsync(congViec, entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int maCongViec, int maChiTietCv)
        {
            var congViec = await GetCongViecAsync(maCongViec);
            await KiemTraQuyenCapNhatAsync(congViec);

            var entity = await _context.CtCongViec
                .FirstOrDefaultAsync(x => x.MaChiTietCV == maChiTietCv && x.MaCongViec == maCongViec);

            if (entity == null)
                throw new Exception("Không tìm thấy chi tiết công việc cần xóa.");

            _context.CtCongViec.Remove(entity);
            await _context.SaveChangesAsync();

            var chiTietMoiNhat = await GetChiTietMoiNhatAsync(maCongViec);
            if (chiTietMoiNhat == null)
            {
                congViec.TrangThaiCongViec = TrangThai.ChuaBatDau;
                congViec.NgayKetThucCVThucTe = null;
            }
            else
            {
                DongBoTrangThaiCongViec(
                    congViec,
                    chiTietMoiNhat.NgayBaoCaoCTCV,
                    chiTietMoiNhat.PhanTramHoanThanhCTCV,
                    chiTietMoiNhat.TrangThaiCTCV);
            }

            await _context.SaveChangesAsync();
        }

        private static void ValidateForm(ChiTietCongViecCreateUpdateViewModel form)
        {
            if (form.MaCongViec <= 0)
                throw new Exception("Mã công việc không hợp lệ.");

            if (string.IsNullOrWhiteSpace(form.NoiDungChiTietCV))
                throw new Exception("Nội dung chi tiết công việc không được để trống.");

            if (!form.NgayBaoCaoCTCV.HasValue)
                throw new Exception("Vui lòng chọn ngày báo cáo.");

            if (!form.PhanTramHoanThanhCTCV.HasValue)
                throw new Exception("Vui lòng nhập phần trăm hoàn thành.");

            if (form.PhanTramHoanThanhCTCV < 0 || form.PhanTramHoanThanhCTCV > 100)
                throw new Exception("Phần trăm hoàn thành phải nằm trong khoảng từ 0 đến 100.");

            if (string.IsNullOrWhiteSpace(form.TrangThaiCTCV))
                throw new Exception("Vui lòng chọn trạng thái chi tiết công việc.");
        }

        private async Task<CongViec> GetCongViecAsync(int maCongViec)
        {
            var congViec = await _context.CongViec
                .FirstOrDefaultAsync(x => x.MaCongViec == maCongViec && x.IsDeleted != true);

            if (congViec == null)
                throw new Exception("Không tìm thấy công việc.");

            return congViec;
        }

        private async Task<CtCongViec?> GetChiTietMoiNhatAsync(int maCongViec)
        {
            return await _context.CtCongViec
                .Where(x => x.MaCongViec == maCongViec)
                .OrderByDescending(x => x.NgayBaoCaoCTCV)
                .ThenByDescending(x => x.MaChiTietCV)
                .FirstOrDefaultAsync();
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(aspUserId))
                throw new Exception("Không xác định được người dùng hiện tại.");

            var maNguoiDung = await _context.Aspnetusers
                .Where(x => x.Id == aspUserId)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync();

            if (maNguoiDung <= 0)
                throw new Exception("Không xác định được nhân sự tương ứng của người dùng hiện tại.");

            return maNguoiDung;
        }

        private async Task<int> GetMaDuAnAsync(CongViec congViec)
        {
            var maDuAn = await _context.DanhMucCongViec
                .Where(x => x.MaDanhMucCV == congViec.MaDanhMucCV)
                .Select(x => x.MaDuAn)
                .FirstOrDefaultAsync();

            if (maDuAn <= 0)
                throw new Exception("Không tìm thấy dự án của công việc.");

            return maDuAn;
        }

        private async Task KiemTraQuyenCapNhatAsync(CongViec congViec)
        {
            if (await CoTheCapNhatAsync(congViec))
                return;

            var httpUser = _httpContextAccessor.HttpContext?.User;
            var isEmployee = httpUser?.IsInRole("Employee") == true;

            if (isEmployee)
            {
                throw new Exception("Nhân viên chỉ được cập nhật chi tiết công việc khi là trưởng team hoặc leader dự án.");
            }
            throw new Exception("Bạn không có quyền cập nhật chi tiết công việc này.");
        }

        private async Task<bool> CoTheCapNhatAsync(CongViec congViec)
        {
            var httpUser = _httpContextAccessor.HttpContext?.User;
            if (httpUser?.IsInRole("Admin") == true || httpUser?.IsInRole("Manager") == true)
                return true;

            var maNguoiDungHienTai = await GetCurrentUserIdAsync();
            var maDuAn = await GetMaDuAnAsync(congViec);
            var isEmployee = httpUser?.IsInRole("Employee") == true;

            if (isEmployee)
            {
                return await LaLeaderTeamHoacDuAnAsync(maDuAn, maNguoiDungHienTai);
            }

            var laLeaderDuAn = await _context.NhanVienDuAn
                .AnyAsync(x =>
                    x.MaDuAn == maDuAn &&
                    x.MaNguoiDung == maNguoiDungHienTai &&
                    TrangThai.EqualsValue(x.VaiTroTrongDuAn, TrangThai.VaiTroLeader));

            if (laLeaderDuAn)
                return true;

            return await _context.PhanCongCongViec
                .AnyAsync(x => x.MaCongViec == congViec.MaCongViec && x.MaNguoiDung == maNguoiDungHienTai);
        }

        private async Task<bool> LaLeaderTeamHoacDuAnAsync(int maDuAn, int maNguoiDung)
        {
            var teamDuAnIds = await _context.TeamDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.MaTeam)
                .ToListAsync();

            if (teamDuAnIds.Count > 0)
            {
                var laLeaderTeam = await _context.NhanVienTeam
                    .AnyAsync(x => teamDuAnIds.Contains(x.MaTeam)
                                && x.MaNguoiDung == maNguoiDung
                                && x.IsLeader == true);

                if (laLeaderTeam)
                    return true;
            }

            var vaiTroTrongDuAn = await _context.NhanVienDuAn
                .Where(x => x.MaDuAn == maDuAn && x.MaNguoiDung == maNguoiDung)
                .Select(x => x.VaiTroTrongDuAn)
                .FirstOrDefaultAsync();

            return TrangThai.EqualsValue(vaiTroTrongDuAn, TrangThai.VaiTroLeader);
        }

        private async Task DongBoTrangThaiTuBanGhiMoiNhatAsync(CongViec congViec, CtCongViec entityDangSua)
        {
            var chiTietMoiNhat = await _context.CtCongViec
                .Where(x => x.MaCongViec == congViec.MaCongViec)
                .OrderByDescending(x => x.NgayBaoCaoCTCV)
                .ThenByDescending(x => x.MaChiTietCV)
                .FirstOrDefaultAsync();

            if (chiTietMoiNhat == null || chiTietMoiNhat.MaChiTietCV == entityDangSua.MaChiTietCV)
            {
                DongBoTrangThaiCongViec(
                    congViec,
                    entityDangSua.NgayBaoCaoCTCV,
                    entityDangSua.PhanTramHoanThanhCTCV,
                    entityDangSua.TrangThaiCTCV);
                return;
            }

            DongBoTrangThaiCongViec(
                congViec,
                chiTietMoiNhat.NgayBaoCaoCTCV,
                chiTietMoiNhat.PhanTramHoanThanhCTCV,
                chiTietMoiNhat.TrangThaiCTCV);
        }

        private static void DongBoTrangThaiCongViec(
            CongViec congViec,
            DateTime? ngayBaoCao,
            double? phanTram,
            string? trangThai)
        {
            var phanTramValue = phanTram ?? 0;
            if (phanTramValue >= 100 || TrangThai.LaHoanThanhCongViec(trangThai))
            {
                congViec.TrangThaiCongViec = TrangThai.HoanThanh;
                congViec.NgayKetThucCVThucTe = ngayBaoCao ?? DateTime.Now;
                return;
            }

            congViec.TrangThaiCongViec = string.IsNullOrWhiteSpace(trangThai)
                ? TrangThai.DangThucHien
                : TrangThai.ToCode(trangThai);

            congViec.NgayKetThucCVThucTe = null;
        }
    }
}
