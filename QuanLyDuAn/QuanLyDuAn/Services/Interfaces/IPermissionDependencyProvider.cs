using QuanLyDuAn.ViewModels.PhanQuyen;

namespace QuanLyDuAn.Services.Interfaces;

public interface IPermissionDependencyProvider
{
    IReadOnlyList<PermissionDefinition> GetPermissionDefinitions();
    PermissionDefinition GetPermissionDefinition(string permissionName, string? screenKey = null);
    string? GetParentPermission(string permissionName);
    IReadOnlyCollection<string> GetRequiredPermissionsForRole(string roleName);
    IReadOnlyCollection<string> GetDeniedPermissionsForRole(string roleName);
    HashSet<string> NormalizeDependencies(IEnumerable<string> permissionNames);
    List<string> ValidateDependencies(IEnumerable<string> permissionNames);
    List<string> ValidateRoleConstraints(string roleName, IEnumerable<string> permissionNames);
}

public sealed record PermissionDefinition(
    string PermissionName,
    string DisplayName,
    string Description,
    string ScreenKey,
    string ScreenDisplayName,
    string GroupKey,
    string GroupName,
    string? ParentPermission,
    int GroupOrder,
    int ScreenOrder,
    int PermissionOrder);
