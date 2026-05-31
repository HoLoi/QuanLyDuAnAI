using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Services.Implementations;

public class AccountService : IAccountService
{
    private const string ForgotPasswordProvider = "QUANLYDUAN_FORGOT_PASSWORD";
    private const string OtpTokenPrefix = "OTP:";
    private const string VerifiedTokenPrefix = "VERIFIED:";
    private static readonly TimeSpan OtpExpiredAfter = TimeSpan.FromMinutes(3);
    private static readonly TimeSpan VerifiedExpiredAfter = TimeSpan.FromMinutes(3);

    private readonly QuanLyDuAnDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly PasswordHasher<Aspnetusers> _passwordHasher = new();

    public AccountService(QuanLyDuAnDbContext dbContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
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
            throw new Exception("Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        var taiKhoanBiKhoa = account.LockoutEnabled
                           && account.LockoutEnd.HasValue
                           && account.LockoutEnd.Value > DateTime.UtcNow;
        if (taiKhoanBiKhoa)
        {
            throw new Exception("Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên.");
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            throw new Exception("Tên đăng nhập hoặc mật khẩu không đúng.");
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

    public async Task<string> KhoiTaoQuenMatKhauAsync(string tenDangNhapHoacEmail)
    {
        var maPhien = TaoMaPhien();
        if (string.IsNullOrWhiteSpace(tenDangNhapHoacEmail))
        {
            return maPhien;
        }

        var thongTinDangNhap = tenDangNhapHoacEmail.Trim();
        var normalizedInput = thongTinDangNhap.ToUpperInvariant();

        var account = await _dbContext.Aspnetusers
            .FirstOrDefaultAsync(x =>
                x.NormalizedUserName == normalizedInput
                || x.UserName == thongTinDangNhap
                || x.NormalizedEmail == normalizedInput
                || x.Email == thongTinDangNhap);

        if (account is null || string.IsNullOrWhiteSpace(account.Email))
        {
            return maPhien;
        }

        await XoaTokenQuenMatKhauAsync(account.Id);

        var otp = TaoMaOtp();
        var otpPayload = new OtpTokenPayload
        {
            OtpHash = BamSha256(otp),
            ExpiresUtc = DateTime.UtcNow.Add(OtpExpiredAfter),
            FailedAttempts = 0
        };

        var otpToken = new Aspnetusertokens
        {
            Id = account.Id,
            LoginProvider = ForgotPasswordProvider,
            Name = OtpTokenPrefix + maPhien,
            Value = JsonSerializer.Serialize(otpPayload)
        };

        _dbContext.Aspnetusertokens.Add(otpToken);
        await _dbContext.SaveChangesAsync();

        try
        {
            await _emailService.SendAsync(
                account.Email,
                "Mã OTP đặt lại mật khẩu",
                TaoNoiDungOtp(otp));
        }
        catch
        {
            _dbContext.Aspnetusertokens.Remove(otpToken);
            await _dbContext.SaveChangesAsync();
            throw new Exception("Hệ thống chưa thể gửi mã OTP lúc này. Vui lòng thử lại sau.");
        }

        return maPhien;
    }

    public async Task<bool> XacNhanOtpDatLaiMatKhauAsync(string maPhien, string maOtp)
    {
        if (string.IsNullOrWhiteSpace(maPhien) || string.IsNullOrWhiteSpace(maOtp))
        {
            return false;
        }

        var normalizedSession = maPhien.Trim();
        var tokenName = OtpTokenPrefix + normalizedSession;
        var token = await _dbContext.Aspnetusertokens
            .FirstOrDefaultAsync(x => x.LoginProvider == ForgotPasswordProvider && x.Name == tokenName);

        if (token is null || string.IsNullOrWhiteSpace(token.Value))
        {
            return false;
        }

        var payload = DocPayload<OtpTokenPayload>(token.Value);
        if (payload is null || payload.ExpiresUtc <= DateTime.UtcNow)
        {
            _dbContext.Aspnetusertokens.Remove(token);
            await _dbContext.SaveChangesAsync();
            return false;
        }

        var otpHopLe = SoSanhAnToan(payload.OtpHash, BamSha256(maOtp.Trim()));
        if (!otpHopLe)
        {
            payload.FailedAttempts++;
            if (payload.FailedAttempts >= 5)
            {
                _dbContext.Aspnetusertokens.Remove(token);
            }
            else
            {
                token.Value = JsonSerializer.Serialize(payload);
            }

            await _dbContext.SaveChangesAsync();
            return false;
        }

        _dbContext.Aspnetusertokens.Remove(token);

        var verifyTokenName = VerifiedTokenPrefix + normalizedSession;
        var verifyToken = await _dbContext.Aspnetusertokens
            .FirstOrDefaultAsync(x =>
                x.Id == token.Id
                && x.LoginProvider == ForgotPasswordProvider
                && x.Name == verifyTokenName);
        if (verifyToken is not null)
        {
            _dbContext.Aspnetusertokens.Remove(verifyToken);
        }

        _dbContext.Aspnetusertokens.Add(new Aspnetusertokens
        {
            Id = token.Id,
            LoginProvider = ForgotPasswordProvider,
            Name = verifyTokenName,
            Value = JsonSerializer.Serialize(new VerifiedTokenPayload
            {
                ExpiresUtc = DateTime.UtcNow.Add(VerifiedExpiredAfter)
            })
        });

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CoPhienDatLaiMatKhauHopLeAsync(string maPhien)
    {
        if (string.IsNullOrWhiteSpace(maPhien))
        {
            return false;
        }

        var tokenName = VerifiedTokenPrefix + maPhien.Trim();
        var token = await _dbContext.Aspnetusertokens
            .FirstOrDefaultAsync(x => x.LoginProvider == ForgotPasswordProvider && x.Name == tokenName);

        if (token is null || string.IsNullOrWhiteSpace(token.Value))
        {
            return false;
        }

        var payload = DocPayload<VerifiedTokenPayload>(token.Value);
        if (payload is null || payload.ExpiresUtc <= DateTime.UtcNow)
        {
            _dbContext.Aspnetusertokens.Remove(token);
            await _dbContext.SaveChangesAsync();
            return false;
        }

        return true;
    }

    public async Task DatLaiMatKhauBangOtpAsync(string maPhien, string matKhauMoi)
    {
        if (string.IsNullOrWhiteSpace(maPhien))
        {
            throw new Exception("Phiên đặt lại mật khẩu không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(matKhauMoi))
        {
            throw new Exception("Mật khẩu mới không được để trống.");
        }

        var tokenName = VerifiedTokenPrefix + maPhien.Trim();
        var verifyToken = await _dbContext.Aspnetusertokens
            .FirstOrDefaultAsync(x => x.LoginProvider == ForgotPasswordProvider && x.Name == tokenName);

        if (verifyToken is null || string.IsNullOrWhiteSpace(verifyToken.Value))
        {
            throw new Exception("Phiên đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");
        }

        var payload = DocPayload<VerifiedTokenPayload>(verifyToken.Value);
        if (payload is null || payload.ExpiresUtc <= DateTime.UtcNow)
        {
            _dbContext.Aspnetusertokens.Remove(verifyToken);
            await _dbContext.SaveChangesAsync();
            throw new Exception("Phiên đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");
        }

        var account = await _dbContext.Aspnetusers
            .FirstOrDefaultAsync(x => x.Id == verifyToken.Id);
        if (account is null)
        {
            _dbContext.Aspnetusertokens.Remove(verifyToken);
            await _dbContext.SaveChangesAsync();
            throw new Exception("Không thể cập nhật mật khẩu. Vui lòng thử lại.");
        }

        account.PasswordHash = _passwordHasher.HashPassword(account, matKhauMoi.Trim());
        account.SecurityStamp = Guid.NewGuid().ToString("N");
        account.ConcurrencyStamp = Guid.NewGuid().ToString("N");
        account.AccessFailedCount = 0;
        account.LockoutEnd = null;

        await XoaTokenQuenMatKhauAsync(account.Id);
        await _dbContext.SaveChangesAsync();
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

    private async Task XoaTokenQuenMatKhauAsync(string userId)
    {
        var tokens = await _dbContext.Aspnetusertokens
            .Where(x =>
                x.Id == userId
                && x.LoginProvider == ForgotPasswordProvider
                && (x.Name.StartsWith(OtpTokenPrefix) || x.Name.StartsWith(VerifiedTokenPrefix)))
            .ToListAsync();

        if (tokens.Count > 0)
        {
            _dbContext.Aspnetusertokens.RemoveRange(tokens);
        }
    }

    private static string TaoMaPhien() => Guid.NewGuid().ToString("N");

    private static string TaoMaOtp()
    {
        var number = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return number.ToString("D6");
    }

    private static string TaoNoiDungOtp(string otp)
    {
        return $"""
Xin chào,

Bạn vừa yêu cầu đặt lại mật khẩu cho tài khoản hệ thống quản lý dự án.
Mã OTP của bạn là: {otp}
Mã có hiệu lực trong 3 phút.

Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.
""";
    }

    private static string BamSha256(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    private static bool SoSanhAnToan(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);
        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }

    private static TPayload? DocPayload<TPayload>(string json)
        where TPayload : class
    {
        try
        {
            return JsonSerializer.Deserialize<TPayload>(json);
        }
        catch
        {
            return null;
        }
    }

    private sealed class OtpTokenPayload
    {
        public string OtpHash { get; set; } = string.Empty;
        public DateTime ExpiresUtc { get; set; }
        public int FailedAttempts { get; set; }
    }

    private sealed class VerifiedTokenPayload
    {
        public DateTime ExpiresUtc { get; set; }
    }
}
