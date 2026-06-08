using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.PhanQuyen;

namespace QuanLyDuAn.Services.Implementations;

public class PhanQuyenService : IPhanQuyenService
{
    private readonly QuanLyDuAnDbContext _db;
    private readonly IPermissionDependencyProvider _permissionDependencyProvider;

    public PhanQuyenService(
        QuanLyDuAnDbContext db,
        IPermissionDependencyProvider permissionDependencyProvider)
    {
        _db = db;
        _permissionDependencyProvider = permissionDependencyProvider;
    }

    public Task<HashSet<string>> GetGrantedPermissionNamesAsync(ClaimsPrincipal user)
    {
        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (user?.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult(permissions);
        }

        foreach (var claim in user.Claims)
        {
            if (claim is null || string.IsNullOrWhiteSpace(claim.Value))
            {
                continue;
            }

            var type = claim.Type ?? string.Empty;
            if (type.Contains(Permissions.ClaimTypesCustom.Permission, StringComparison.OrdinalIgnoreCase)
                || type.Contains("claim", StringComparison.OrdinalIgnoreCase)
                || type.Contains("quyen", StringComparison.OrdinalIgnoreCase))
            {
                permissions.Add(claim.Value.Trim());
            }
        }

        return Task.FromResult(permissions);
    }

    public async Task<PhanQuyenPageViewModel> GetPageViewModelAsync(string? roleId, CancellationToken cancellationToken)
    {
        var roles = await _db.Aspnetroles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var selectedRoleId = roleId;
        if (string.IsNullOrWhiteSpace(selectedRoleId))
        {
            selectedRoleId = roles.FirstOrDefault()?.Id;
        }

        var selectedRoleName = roles
            .FirstOrDefault(x => string.Equals(x.Id, selectedRoleId, StringComparison.OrdinalIgnoreCase))?
            .Name ?? string.Empty;

        var selectedPermissionSet = new HashSet<int>();
        if (!string.IsNullOrWhiteSpace(selectedRoleId))
        {
            var selectedPermissionIds = await _db.Aspnetroleclaims
                .AsNoTracking()
                .Where(x => x.Asp_Id == selectedRoleId)
                .Select(x => x.MaDanhMucQuyen)
                .Distinct()
                .ToListAsync(cancellationToken);

            selectedPermissionSet = selectedPermissionIds.ToHashSet();
        }

        var permissionRows = await (from permission in _db.DanhMucQuyen.AsNoTracking()
                                    join screen in _db.DanhMucManHinh.AsNoTracking()
                                        on permission.MaManHinh equals screen.MaManHinh into screenJoin
                                    from screen in screenJoin.DefaultIfEmpty()
                                    orderby screen.TenManHinh, permission.TenDanhMucQuyen
                                    select new
                                    {
                                        permission.MaManHinh,
                                        TenManHinh = screen != null && !string.IsNullOrWhiteSpace(screen.TenManHinh)
                                            ? screen.TenManHinh
                                            : "Khac",
                                        permission.MaDanhMucQuyen,
                                        permission.TenDanhMucQuyen,
                                        permission.MoTaDanhMucQuyen
                                    }).ToListAsync(cancellationToken);

        var requiredByRole = _permissionDependencyProvider.GetRequiredPermissionsForRole(selectedRoleName);
        var deniedByRole = _permissionDependencyProvider.GetDeniedPermissionsForRole(selectedRoleName);
        var parentPermissions = _permissionDependencyProvider.GetPermissionDefinitions()
            .Where(x => !string.IsNullOrWhiteSpace(x.ParentPermission))
            .Select(x => x.ParentPermission!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var enrichedPermissions = permissionRows
            .Select(permission =>
            {
                var permissionName = permission.TenDanhMucQuyen ?? $"Quyen {permission.MaDanhMucQuyen}";
                var definition = _permissionDependencyProvider.GetPermissionDefinition(permissionName, permission.TenManHinh);
                return new
                {
                    permission.MaManHinh,
                    permission.TenManHinh,
                    permission.MaDanhMucQuyen,
                    PermissionName = permissionName,
                    permission.MoTaDanhMucQuyen,
                    Definition = definition
                };
            })
            .OrderBy(x => x.Definition.GroupOrder)
            .ThenBy(x => x.Definition.ScreenOrder)
            .ThenBy(x => x.Definition.PermissionOrder)
            .ThenBy(x => x.PermissionName)
            .ToList();

        var groupedPermissions = enrichedPermissions
            .GroupBy(x => new
            {
                x.MaManHinh,
                x.TenManHinh,
                x.Definition.ScreenDisplayName,
                x.Definition.GroupKey,
                x.Definition.GroupName,
                x.Definition.GroupOrder,
                x.Definition.ScreenOrder
            })
            .Select(group => new PhanQuyenNhomManHinhViewModel
            {
                MaManHinh = group.Key.MaManHinh,
                TenManHinh = group.Key.TenManHinh,
                DisplayName = group.Key.ScreenDisplayName,
                GroupKey = group.Key.GroupKey,
                GroupName = group.Key.GroupName,
                GroupOrder = group.Key.GroupOrder,
                ScreenOrder = group.Key.ScreenOrder,
                SearchText = string.Join(' ', group.Select(x =>
                    $"{x.TenManHinh} {x.Definition.ScreenDisplayName} {x.Definition.GroupName} {x.PermissionName} {x.Definition.DisplayName}")),
                DanhSachQuyen = group.Select(permission => new PhanQuyenItemViewModel
                {
                    MaDanhMucQuyen = permission.MaDanhMucQuyen,
                    TenDanhMucQuyen = permission.PermissionName,
                    MoTaDanhMucQuyen = permission.MoTaDanhMucQuyen,
                    IsSelected = selectedPermissionSet.Contains(permission.MaDanhMucQuyen),
                    DisplayName = permission.Definition.DisplayName,
                    GroupKey = permission.Definition.GroupKey,
                    GroupName = permission.Definition.GroupName,
                    ScreenKey = permission.Definition.ScreenKey,
                    ScreenDisplayName = permission.Definition.ScreenDisplayName,
                    ParentPermission = permission.Definition.ParentPermission,
                    IsParent = parentPermissions.Contains(permission.PermissionName),
                    DependencyMessage = permission.Definition.ParentPermission is null ? string.Empty : "Cần quyền Xem.",
                    IsDeniedByRole = deniedByRole.Contains(permission.PermissionName),
                    IsRequiredByRole = requiredByRole.Contains(permission.PermissionName),
                    Description = permission.Definition.Description,
                    GroupOrder = permission.Definition.GroupOrder,
                    ScreenOrder = permission.Definition.ScreenOrder,
                    PermissionOrder = permission.Definition.PermissionOrder
                })
                .OrderBy(x => x.PermissionOrder)
                .ThenBy(x => x.TenDanhMucQuyen)
                .ToList()
            })
            .OrderBy(x => x.GroupOrder)
            .ThenBy(x => x.ScreenOrder)
            .ThenBy(x => x.DisplayName)
            .ToList();

        return new PhanQuyenPageViewModel
        {
            SelectedRoleId = selectedRoleId,
            Roles = roles.Select(x => new PhanQuyenRoleViewModel
            {
                RoleId = x.Id,
                RoleName = x.Name ?? x.NormalizedName ?? x.Id
            }).ToList(),
            PermissionGroups = groupedPermissions
        };
    }

    public async Task SaveRolePermissionsAsync(PhanQuyenSaveInputViewModel model, CancellationToken cancellationToken)
    {
        var selectedRoleId = model.SelectedRoleId?.Trim();
        if (string.IsNullOrWhiteSpace(selectedRoleId))
        {
            throw new Exception("Vui lòng chọn vai trò hệ thống.");
        }

        var role = await _db.Aspnetroles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == selectedRoleId, cancellationToken);
        if (role == null)
        {
            throw new Exception("Vai trò không tồn tại.");
        }

        var selectedPermissionIds = (model.SelectedPermissionIds ?? new List<int>())
            .Where(x => x > 0)
            .Distinct()
            .ToList();

        var allPermissions = await _db.DanhMucQuyen
            .AsNoTracking()
            .Select(x => new { x.MaDanhMucQuyen, x.TenDanhMucQuyen })
            .ToListAsync(cancellationToken);

        var selectedPermissionNames = allPermissions
            .Where(x => selectedPermissionIds.Contains(x.MaDanhMucQuyen))
            .Select(x => x.TenDanhMucQuyen)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var roleName = role.Name ?? role.NormalizedName ?? string.Empty;
        if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var requiredPermission in _permissionDependencyProvider.GetRequiredPermissionsForRole(roleName))
            {
                selectedPermissionNames.Add(requiredPermission);
            }
        }

        var normalizedPermissionNames = _permissionDependencyProvider.NormalizeDependencies(selectedPermissionNames);
        var validationErrors = new List<string>();
        validationErrors.AddRange(_permissionDependencyProvider.ValidateDependencies(normalizedPermissionNames));
        validationErrors.AddRange(_permissionDependencyProvider.ValidateRoleConstraints(roleName, normalizedPermissionNames));

        if (validationErrors.Count > 0)
        {
            throw new Exception(string.Join(" ", validationErrors.Distinct(StringComparer.OrdinalIgnoreCase)));
        }

        var validPermissionMap = allPermissions
            .Where(x => !string.IsNullOrWhiteSpace(x.TenDanhMucQuyen)
                        && normalizedPermissionNames.Contains(x.TenDanhMucQuyen))
            .ToDictionary(x => x.MaDanhMucQuyen, x => x.TenDanhMucQuyen ?? $"Quyen {x.MaDanhMucQuyen}");

        var missingPermissions = normalizedPermissionNames
            .Where(x => !validPermissionMap.Values.Contains(x, StringComparer.OrdinalIgnoreCase))
            .ToList();
        if (missingPermissions.Count > 0)
        {
            throw new Exception($"Danh mục quyền chưa được seed đầy đủ: {string.Join(", ", missingPermissions)}.");
        }

        var permissionSet = validPermissionMap.Keys.ToHashSet();

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var currentClaims = await _db.Aspnetroleclaims
            .Where(x => x.Asp_Id == selectedRoleId)
            .ToListAsync(cancellationToken);

        var claimToRemove = currentClaims
            .Where(x => !permissionSet.Contains(x.MaDanhMucQuyen))
            .ToList();

        if (claimToRemove.Count > 0)
        {
            _db.Aspnetroleclaims.RemoveRange(claimToRemove);
        }

        var existingPermissionSet = currentClaims.Select(x => x.MaDanhMucQuyen).ToHashSet();
        var claimToAdd = permissionSet.Where(x => !existingPermissionSet.Contains(x)).ToList();

        foreach (var permissionId in claimToAdd)
        {
            _db.Aspnetroleclaims.Add(new Aspnetroleclaims
            {
                Asp_Id = selectedRoleId,
                MaDanhMucQuyen = permissionId,
                ClaimType = Permissions.ClaimTypesCustom.Permission,
                ClaimValue = validPermissionMap[permissionId]
            });
        }

        foreach (var claim in currentClaims.Where(x => permissionSet.Contains(x.MaDanhMucQuyen)))
        {
            claim.ClaimType = Permissions.ClaimTypesCustom.Permission;
            claim.ClaimValue = validPermissionMap[claim.MaDanhMucQuyen];
        }

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
