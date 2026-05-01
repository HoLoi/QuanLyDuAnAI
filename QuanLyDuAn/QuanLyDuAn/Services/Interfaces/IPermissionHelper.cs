using System.Security.Claims;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IPermissionHelper
    {
        Task<bool> HasPermissionAsync(ClaimsPrincipal user, params string[] permissions);
    }
}
