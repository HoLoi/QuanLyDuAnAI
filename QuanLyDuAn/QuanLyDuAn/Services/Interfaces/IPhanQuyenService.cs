using System.Security.Claims;
using QuanLyDuAn.ViewModels.PhanQuyen;

namespace QuanLyDuAn.Services.Interfaces;

public interface IPhanQuyenService
{
    Task<HashSet<string>> GetGrantedPermissionNamesAsync(ClaimsPrincipal user);
    Task<PhanQuyenPageViewModel> GetPageViewModelAsync(string? roleId, CancellationToken cancellationToken);
    Task SaveRolePermissionsAsync(PhanQuyenSaveInputViewModel model, CancellationToken cancellationToken);
}

