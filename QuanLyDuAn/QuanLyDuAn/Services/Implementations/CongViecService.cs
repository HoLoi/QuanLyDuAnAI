using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.CongViec;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class CongViecService : ICongViecService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CongViecService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CongViecPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai, string? tuKhoa)
        {
            var allowedProjectIds = await GetAccessibleProjectIdsAsync();
            var projectOptions = await GetProjectOptionsAsync(allowedProjectIds);

            var query =
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                join da in _context.DuAn on dm.MaDuAn equals da.MaDuAn
                join md in _context.MucDoUuTien on cv.MaMucDo equals md.MaMucDo
                join cp in _context.ChiPhi.Where(x => x.IsDeleted != true) on cv.MaCongViec equals cp.MaCongViec into cpGroup
                where cv.IsDeleted != true && dm.IsDeleted != true && da.IsDeleted != true
                select new CongViecItemViewModel
                {
                    MaCongViec = cv.MaCongViec,
                    MaDanhMucCV = dm.MaDanhMucCV,
                    TenDanhMucCV = dm.TenDanhMucCV ?? $"Danh mục {dm.MaDanhMucCV}",
                    MaDuAn = da.MaDuAn,
                    TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}",
                    MaMucDo = md.MaMucDo,
                    TenMucDo = md.TenMucDo ?? $"Mức độ {md.MaMucDo}",
                    TenCongViec = cv.TenCongViec ?? string.Empty,
                    MoTaCongViec = cv.MoTaCongViec ?? string.Empty,
                    NgayBatDauCongViec = cv.NgayBatDauCongViec,
                    NgayKetThucCVDuKien = cv.NgayKetThucCVDuKien,
                    NgayKetThucCVThucTe = cv.NgayKetThucCVThucTe,
                    NgayTaoCongViec = cv.NgayTaoCongViec,
                    TrangThaiCongViec = cv.TrangThaiCongViec ?? string.Empty,
                    ChiPhiDaChi = cpGroup.Sum(x => x.SoTienDaChi ?? 0m)
                };

            if (allowedProjectIds.Count > 0)
            {
                query = query.Where(x => allowedProjectIds.Contains(x.MaDuAn));
            }

            if (locMaDuAn.HasValue)
            {
                query = query.Where(x => x.MaDuAn == locMaDuAn.Value);
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    x.TenCongViec.ToLower().Contains(keyword) ||
                    x.MoTaCongViec.ToLower().Contains(keyword) ||
                    x.TenDanhMucCV.ToLower().Contains(keyword) ||
                    x.TenDuAn.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(locTrangThai))
            {
                var filterValues = TrangThai.GetCommonStatusVariants(locTrangThai);
                if (filterValues.Length > 0)
                {
                    query = query.Where(x => filterValues.Contains(x.TrangThaiCongViec));
                }
            }

            return new CongViecPageViewModel
            {
                DanhSach = await query
                    .OrderByDescending(x => x.NgayTaoCongViec)
                    .ThenByDescending(x => x.MaCongViec)
                    .ToListAsync(),
                DanhSachDuAn = projectOptions,
                LocMaDuAn = locMaDuAn,
                LocTrangThai = locTrangThai,
                TuKhoa = tuKhoa
            };
        }

        private async Task<List<int>> GetAccessibleProjectIdsAsync()
        {
            var (isManager, isEmployee) = await GetCurrentUserRoleFlagsAsync();

            if (!isManager && !isEmployee)
            {
                return await _context.DuAn
                    .Where(x => x.IsDeleted != true)
                    .Select(x => x.MaDuAn)
                    .ToListAsync();
            }

            var currentUserId = await GetCurrentUserIdAsync();
            var projectIds = new List<int>();

            if (isManager)
            {
                var managedIds = await _context.DuAn
                    .Where(x => x.IsDeleted != true && x.MaNguoiDung == currentUserId)
                    .Select(x => x.MaDuAn)
                    .ToListAsync();

                projectIds.AddRange(managedIds);
            }

            if (isEmployee)
            {
                var memberIds = await _context.NhanVienDuAn
                    .Where(x => x.MaNguoiDung == currentUserId)
                    .Select(x => x.MaDuAn)
                    .ToListAsync();

                projectIds.AddRange(memberIds);
            }

            return projectIds.Distinct().ToList();
        }

        private async Task<List<CongViecDuAnOptionViewModel>> GetProjectOptionsAsync(List<int> allowedProjectIds)
        {
            var query = _context.DuAn.Where(x => x.IsDeleted != true);

            if (allowedProjectIds.Count > 0)
            {
                query = query.Where(x => allowedProjectIds.Contains(x.MaDuAn));
            }

            return await query
                .OrderBy(x => x.TenDuAn)
                .Select(x => new CongViecDuAnOptionViewModel
                {
                    MaDuAn = x.MaDuAn,
                    TenDuAn = x.TenDuAn ?? $"Dự án {x.MaDuAn}"
                })
                .ToListAsync();
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

        private async Task<(bool IsManager, bool IsEmployee)> GetCurrentUserRoleFlagsAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(aspUserId))
                throw new Exception("Không xác định được người dùng hiện tại.");

            var roleNames = await (
                from ur in _context.Aspnetuserroles
                join r in _context.Aspnetroles on ur.Id equals r.Id
                where ur.Asp_Id == aspUserId
                select (r.NormalizedName ?? r.Name) ?? string.Empty
            ).ToListAsync();

            var normalizedRoles = roleNames
                .Select(x => x.Trim().ToUpperInvariant())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet();

            return (normalizedRoles.Contains("MANAGER"), normalizedRoles.Contains("EMPLOYEE"));
        }
    }
}
