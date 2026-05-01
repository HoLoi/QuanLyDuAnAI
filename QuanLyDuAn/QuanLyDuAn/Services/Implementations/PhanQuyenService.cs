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

    public PhanQuyenService(QuanLyDuAnDbContext db)
    {
        _db = db;
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

        var groupedPermissions = permissionRows
            .GroupBy(x => new { x.MaManHinh, x.TenManHinh })
            .Select(group => new PhanQuyenNhomManHinhViewModel
            {
                MaManHinh = group.Key.MaManHinh,
                TenManHinh = group.Key.TenManHinh,
                DanhSachQuyen = group.Select(permission => new PhanQuyenItemViewModel
                {
                    MaDanhMucQuyen = permission.MaDanhMucQuyen,
                    TenDanhMucQuyen = permission.TenDanhMucQuyen ?? $"Quyen {permission.MaDanhMucQuyen}",
                    MoTaDanhMucQuyen = permission.MoTaDanhMucQuyen,
                    IsSelected = selectedPermissionSet.Contains(permission.MaDanhMucQuyen)
                }).ToList()
            }).ToList();

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

        var validPermissions = await _db.DanhMucQuyen
            .AsNoTracking()
            .Where(x => selectedPermissionIds.Contains(x.MaDanhMucQuyen))
            .Select(x => new { x.MaDanhMucQuyen, x.TenDanhMucQuyen })
            .ToListAsync(cancellationToken);

        var roleName = role.Name ?? role.NormalizedName ?? string.Empty;
        if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            var requiredPermissions = new[]
            {
                Permissions.PhanQuyen.Xem,
                Permissions.PhanQuyen.Luu
            };

            var validPermissionNames = validPermissions
                .Select(x => x.TenDanhMucQuyen)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!requiredPermissions.All(validPermissionNames.Contains))
            {
                throw new Exception("Admin bắt buộc phải có quyền xem và lưu phân quyền.");
            }
        }

        var validPermissionMap = validPermissions
            .ToDictionary(x => x.MaDanhMucQuyen, x => x.TenDanhMucQuyen ?? $"Quyen {x.MaDanhMucQuyen}");
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

