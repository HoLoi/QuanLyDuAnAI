using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Services.Interfaces;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class DanhMucCongViecScopeService : IDanhMucCongViecScopeService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IPhanQuyenService _phanQuyenService;

        public DanhMucCongViecScopeService(
            QuanLyDuAnDbContext context,
            IPhanQuyenService phanQuyenService)
        {
            _context = context;
            _phanQuyenService = phanQuyenService;
        }

        public async Task<bool> CanAccessProjectAsync(ClaimsPrincipal user, int maDuAn, string permission)
        {
            if (maDuAn <= 0 || string.IsNullOrWhiteSpace(permission))
                return false;

            if (!await HasPermissionAsync(user, permission))
                return false;

            if (IsAdminOrManager(user))
                return true;

            if (!IsEmployee(user))
                return false;

            var maNguoiDung = await GetCurrentUserIdAsync(user);
            if (!maNguoiDung.HasValue)
                return false;

            return await IsLeaderOfActiveProjectTeamAsync(maNguoiDung.Value, maDuAn);
        }

        public async Task<HashSet<int>> GetAccessibleProjectIdsAsync(ClaimsPrincipal user, string permission)
        {
            if (string.IsNullOrWhiteSpace(permission) || !await HasPermissionAsync(user, permission))
                return new HashSet<int>();

            if (IsAdminOrManager(user))
            {
                var allProjectIds = await _context.DuAn
                    .Where(x => x.IsDeleted != true)
                    .Select(x => x.MaDuAn)
                    .ToListAsync();

                return allProjectIds.ToHashSet();
            }

            if (!IsEmployee(user))
                return new HashSet<int>();

            var maNguoiDung = await GetCurrentUserIdAsync(user);
            if (!maNguoiDung.HasValue)
                return new HashSet<int>();

            var activeTeamStatuses = TrangThai.GetCommonStatusVariants(TrangThai.HoatDong);
            var projectIds = await (
                from nvt in _context.NhanVienTeam
                join team in _context.Team on nvt.MaTeam equals team.MaTeam
                join tda in _context.TeamDuAn on team.MaTeam equals tda.MaTeam
                join duAn in _context.DuAn on tda.MaDuAn equals duAn.MaDuAn
                where nvt.MaNguoiDung == maNguoiDung.Value
                      && nvt.IsLeader == true
                      && team.IsDeleted != true
                      && activeTeamStatuses.Contains(team.TrangThaiTeam ?? string.Empty)
                      && duAn.IsDeleted != true
                select duAn.MaDuAn
            )
            .Distinct()
            .ToListAsync();

            return projectIds.ToHashSet();
        }

        private async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission)
        {
            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(user);
            return permissions.Contains(permission);
        }

        private static bool IsAdminOrManager(ClaimsPrincipal user)
        {
            return user.IsInRole("Admin")
                || user.IsInRole("ADMIN")
                || user.IsInRole("Manager")
                || user.IsInRole("MANAGER");
        }

        private static bool IsEmployee(ClaimsPrincipal user)
        {
            return user.IsInRole("Employee") || user.IsInRole("EMPLOYEE");
        }

        private async Task<int?> GetCurrentUserIdAsync(ClaimsPrincipal user)
        {
            var maNguoiDungClaim = user.FindFirst("MaNguoiDung")?.Value;
            if (int.TryParse(maNguoiDungClaim, out var maNguoiDung) && maNguoiDung > 0)
                return maNguoiDung;

            var aspUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(aspUserId))
                return null;

            var maNguoiDungFromDb = await _context.Aspnetusers
                .Where(x => x.Id == aspUserId)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync();

            return maNguoiDungFromDb > 0 ? maNguoiDungFromDb : null;
        }

        private async Task<bool> IsLeaderOfActiveProjectTeamAsync(int maNguoiDung, int maDuAn)
        {
            var activeTeamStatuses = TrangThai.GetCommonStatusVariants(TrangThai.HoatDong);
            return await (
                from nvt in _context.NhanVienTeam
                join team in _context.Team on nvt.MaTeam equals team.MaTeam
                join tda in _context.TeamDuAn on team.MaTeam equals tda.MaTeam
                join duAn in _context.DuAn on tda.MaDuAn equals duAn.MaDuAn
                where nvt.MaNguoiDung == maNguoiDung
                      && nvt.IsLeader == true
                      && tda.MaDuAn == maDuAn
                      && team.IsDeleted != true
                      && activeTeamStatuses.Contains(team.TrangThaiTeam ?? string.Empty)
                      && duAn.IsDeleted != true
                select nvt.MaTeam
            ).AnyAsync();
        }
    }
}
