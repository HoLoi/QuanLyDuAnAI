using System.Security.Claims;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDanhMucCongViecScopeService
    {
        Task<bool> CanAccessProjectAsync(ClaimsPrincipal user, int maDuAn, string permission);
        Task<HashSet<int>> GetAccessibleProjectIdsAsync(ClaimsPrincipal user, string permission);
    }
}
