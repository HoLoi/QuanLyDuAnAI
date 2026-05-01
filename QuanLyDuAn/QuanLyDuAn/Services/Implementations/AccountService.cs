using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly QuanLyDuAnDbContext _dbContext;
    private readonly PasswordHasher<Aspnetusers> _passwordHasher = new();

    public AccountService(QuanLyDuAnDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClaimsPrincipal> AuthenticateAsync(string userName, string password)
    {
        var normalizedUserName = userName.Trim().ToUpperInvariant();

        var account = await _dbContext.Aspnetusers
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.NormalizedUserName == normalizedUserName
                || x.UserName == userName.Trim());

        if (account is null || string.IsNullOrWhiteSpace(account.PasswordHash))
        {
            throw new Exception("Ten dang nhap hoac mat khau khong dung.");
        }

        var taiKhoanBiKhoa = account.LockoutEnabled
                           && account.LockoutEnd.HasValue
                           && account.LockoutEnd.Value > DateTime.UtcNow;
        if (taiKhoanBiKhoa)
        {
            throw new Exception("Tai khoan da bi khoa. Vui long lien he quan tri vien.");
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            throw new Exception("Ten dang nhap hoac mat khau khong dung.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id),
            new(ClaimTypes.Name, account.UserName ?? userName.Trim()),
            new("MaNguoiDung", account.MaNguoiDung.ToString())
        };

        var roleAssignments = await (from ur in _dbContext.Aspnetuserroles.AsNoTracking()
                                     join r in _dbContext.Aspnetroles.AsNoTracking() on ur.Id equals r.Id
                                     where ur.Asp_Id == account.Id
                                     select new { ur.Id, RoleName = r.Name })
            .ToListAsync();

        foreach (var role in roleAssignments)
        {
            if (string.IsNullOrWhiteSpace(role.RoleName))
            {
                continue;
            }

            claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
            claims.Add(new Claim("role", role.RoleName));

            foreach (var aliasRole in GetAliasRoles(role.RoleName))
            {
                claims.Add(new Claim(ClaimTypes.Role, aliasRole));
                claims.Add(new Claim("role", aliasRole));
            }
        }

        var isLeader = await _dbContext.NhanVienTeam
            .AsNoTracking()
            .AnyAsync(x => x.MaNguoiDung == account.MaNguoiDung && x.IsLeader == true);
        claims.Add(new Claim("IsLeader", isLeader ? "true" : "false"));

        var roleIds = roleAssignments.Select(x => x.Id).Distinct().ToList();
        if (roleIds.Count > 0)
        {
            var roleClaims = await (from rc in _dbContext.Aspnetroleclaims.AsNoTracking()
                                    join dmq in _dbContext.DanhMucQuyen.AsNoTracking()
                                        on rc.MaDanhMucQuyen equals dmq.MaDanhMucQuyen into dmqJoin
                                    from dmq in dmqJoin.DefaultIfEmpty()
                                    where roleIds.Contains(rc.Asp_Id)
                                    select new
                                    {
                                        rc.ClaimType,
                                        ClaimValue = string.IsNullOrWhiteSpace(rc.ClaimValue)
                                            ? dmq.TenDanhMucQuyen
                                            : rc.ClaimValue
                                    })
                .ToListAsync();

            foreach (var roleClaim in roleClaims)
            {
                if (string.IsNullOrWhiteSpace(roleClaim.ClaimType) || string.IsNullOrWhiteSpace(roleClaim.ClaimValue))
                {
                    continue;
                }

                if (!claims.Any(x => x.Type == roleClaim.ClaimType && x.Value == roleClaim.ClaimValue))
                {
                    claims.Add(new Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
                }
            }
        }

        var userClaims = await _dbContext.Aspnetuserclaims
            .AsNoTracking()
            .Where(x => x.Asp_Id == account.Id)
            .ToListAsync();

        foreach (var userClaim in userClaims)
        {
            if (string.IsNullOrWhiteSpace(userClaim.ClaimType) || string.IsNullOrWhiteSpace(userClaim.ClaimValue))
            {
                continue;
            }

            if (!claims.Any(x => x.Type == userClaim.ClaimType && x.Value == userClaim.ClaimValue))
            {
                claims.Add(new Claim(userClaim.ClaimType, userClaim.ClaimValue));
            }
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }

    private static IEnumerable<string> GetAliasRoles(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            yield break;
        }

        if (roleName.Equals("USER", StringComparison.OrdinalIgnoreCase)
            || roleName.Equals("USER_LEADER", StringComparison.OrdinalIgnoreCase))
        {
            yield return "Employee";
        }

        if (roleName.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
        {
            yield return "Admin";
        }

        if (roleName.Equals("MANAGER", StringComparison.OrdinalIgnoreCase))
        {
            yield return "Manager";
        }
    }
}
