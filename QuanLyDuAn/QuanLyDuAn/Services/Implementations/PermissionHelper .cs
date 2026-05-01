using QuanLyDuAn.Services.Interfaces;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class PermissionHelper : IPermissionHelper
    {
        private readonly IPhanQuyenService _service;

        public PermissionHelper(IPhanQuyenService service)
        {
            _service = service;
        }

        public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, params string[] permissions)
        {
            var granted = await _service.GetGrantedPermissionNamesAsync(user);
            return permissions.Any(p => granted.Contains(p));
        }
    }
}
