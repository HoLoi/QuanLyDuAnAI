using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.NganSach;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class NganSachService : INganSachService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NganSachService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<NganSachPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai)
        {
            var allowedProjectIds = await GetAccessibleProjectIdsAsync();
            var projectOptions = await GetProjectOptionsAsync(allowedProjectIds);

            var query =
                from ns in _context.NganSach
                join da in _context.DuAn on ns.MaDuAn equals da.MaDuAn
                join ndDuyet in _context.NguoiDung on ns.MaNguoiDungDuyet equals ndDuyet.MaNguoiDung into ndDuyetLeft
                from ndDuyet in ndDuyetLeft.DefaultIfEmpty()
                join ndDeXuat in _context.NguoiDung on ns.MaNguoiDungDeXuat equals ndDeXuat.MaNguoiDung into ndDeXuatLeft
                from ndDeXuat in ndDeXuatLeft.DefaultIfEmpty()
                where ns.IsDeleted != true && da.IsDeleted != true
                select new NganSachItemViewModel
                {
                    MaNganSach = ns.MaNganSach,
                    MaDuAn = da.MaDuAn,
                    TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}",
                    SoTienNganSach = ns.SoTienNganSach,
                    Version = ns.Version,
                    IsActive = ns.IsActive == true,
                    MoTaNganSach = ns.MoTaNganSach ?? string.Empty,
                    NgayCapNhatNganSach = ns.NgayCapNhatNganSach,
                    NgayDuyetNganSach = ns.NgayDuyetNganSach,
                    TrangThaiNganSach = ns.TrangThaiNganSach ?? string.Empty,
                    NguoiDungDeXuat = ndDeXuat != null
                        ? (ndDeXuat.HoTenNguoiDung ?? $"Nhân viên {ndDeXuat.MaNguoiDung}")
                        : string.Empty,
                    NguoiDungDuyet = ndDuyet != null
                        ? (ndDuyet.HoTenNguoiDung ?? $"Nhân viên {ndDuyet.MaNguoiDung}")
                        : string.Empty
                };

            if (allowedProjectIds.Count > 0)
            {
                query = query.Where(x => allowedProjectIds.Contains(x.MaDuAn));
            }

            if (locMaDuAn.HasValue)
            {
                query = query.Where(x => x.MaDuAn == locMaDuAn.Value);
            }

            if (!string.IsNullOrWhiteSpace(locTrangThai))
            {
                var filterValues = TrangThai.GetCommonStatusVariants(locTrangThai);
                if (filterValues.Length > 0)
                {
                    query = query.Where(x => filterValues.Contains(x.TrangThaiNganSach));
                }
            }

            return new NganSachPageViewModel
            {
                DanhSach = await query
                    .OrderByDescending(x => x.NgayDuyetNganSach)
                    .ThenByDescending(x => x.MaNganSach)
                    .ToListAsync(),
                DanhSachDuAn = projectOptions,
                LocMaDuAn = locMaDuAn,
                LocTrangThai = locTrangThai
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

        private async Task<List<NganSachDuAnOptionViewModel>> GetProjectOptionsAsync(List<int> allowedProjectIds)
        {
            var query = _context.DuAn.Where(x => x.IsDeleted != true);

            if (allowedProjectIds.Count > 0)
            {
                query = query.Where(x => allowedProjectIds.Contains(x.MaDuAn));
            }

            return await query
                .OrderBy(x => x.TenDuAn)
                .Select(x => new NganSachDuAnOptionViewModel
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
