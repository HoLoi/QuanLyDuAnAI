using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.TaiKhoanCaNhan;

namespace QuanLyDuAn.Services.Implementations
{
    public class TaiKhoanCaNhanService : ITaiKhoanCaNhanService
    {
        private const long MaxAvatarSizeBytes = 2 * 1024 * 1024;
        private static readonly HashSet<string> AllowedAvatarExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private readonly QuanLyDuAnDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly PasswordHasher<Aspnetusers> _passwordHasher = new();

        public TaiKhoanCaNhanService(QuanLyDuAnDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<TaiKhoanCaNhanViewModel> GetHoSoAsync(ClaimsPrincipal user)
        {
            var aspUserId = GetCurrentAspUserId(user);

            var profile = await (from tk in _context.Aspnetusers.AsNoTracking()
                                 join nd in _context.NguoiDung.AsNoTracking() on tk.Id equals nd.Id
                                 join cd in _context.ChucDanh.AsNoTracking() on nd.MaChucDanh equals cd.MaChucDanh into cdJoin
                                 from cd in cdJoin.DefaultIfEmpty()
                                 where tk.Id == aspUserId && nd.IsDeleted != true
                                 select new
                                 {
                                     tk.Id,
                                     tk.MaNguoiDung,
                                     tk.UserName,
                                     tk.Email,
                                     tk.PhoneNumber,
                                     tk.LockoutEnd,
                                     nd.HoTenNguoiDung,
                                     nd.DiaChiNguoiDung,
                                     nd.SdtNguoiDung,
                                     nd.NgaySinh,
                                     nd.AnhDaiDien,
                                     TenChucDanh = cd != null ? cd.TenChucDanh : null
                                 }).FirstOrDefaultAsync();

            if (profile == null)
            {
                throw new Exception("Không tìm thấy hồ sơ tài khoản hiện tại.");
            }

            var vaiTroHeThong = await (from ur in _context.Aspnetuserroles.AsNoTracking()
                                       join role in _context.Aspnetroles.AsNoTracking() on ur.Id equals role.Id
                                       where ur.Asp_Id == aspUserId
                                       orderby role.Name
                                       select role.Name ?? role.NormalizedName ?? ur.Id)
                .Distinct()
                .ToListAsync();

            var teamHienTai = await (from nvt in _context.NhanVienTeam.AsNoTracking()
                                     join t in _context.Team.AsNoTracking() on nvt.MaTeam equals t.MaTeam
                                     where nvt.MaNguoiDung == profile.MaNguoiDung && t.IsDeleted != true
                                     orderby t.TenTeam
                                     select t.TenTeam ?? $"Team {t.MaTeam}")
                .Distinct()
                .ToListAsync();

            var quanLyHienTai = await (from nvda in _context.NhanVienDuAn.AsNoTracking()
                                       join da in _context.DuAn.AsNoTracking() on nvda.MaDuAn equals da.MaDuAn
                                       join ql in _context.NguoiDung.AsNoTracking() on da.MaNguoiDung equals ql.MaNguoiDung
                                       where nvda.MaNguoiDung == profile.MaNguoiDung
                                             && da.IsDeleted != true
                                             && ql.IsDeleted != true
                                       orderby ql.HoTenNguoiDung
                                       select ql.HoTenNguoiDung ?? $"Người dùng {ql.MaNguoiDung}")
                .Distinct()
                .ToListAsync();

            var hoTen = (profile.HoTenNguoiDung ?? string.Empty).Trim();
            var chuCaiDau = !string.IsNullOrWhiteSpace(hoTen)
                ? hoTen[0].ToString().ToUpperInvariant()
                : "U";

            return new TaiKhoanCaNhanViewModel
            {
                MaNguoiDung = profile.MaNguoiDung,
                UserName = profile.UserName ?? string.Empty,
                HoTenNguoiDung = hoTen,
                Email = profile.Email,
                SdtNguoiDung = profile.SdtNguoiDung ?? profile.PhoneNumber,
                DiaChiNguoiDung = profile.DiaChiNguoiDung,
                NgaySinh = profile.NgaySinh,
                AnhDaiDien = profile.AnhDaiDien,
                TenChucDanh = profile.TenChucDanh,
                TaiKhoanBiKhoa = profile.LockoutEnd.HasValue && profile.LockoutEnd.Value > DateTime.UtcNow,
                VaiTroHeThong = vaiTroHeThong,
                TeamHienTai = teamHienTai,
                QuanLyHienTai = quanLyHienTai,
                ChuCaiDau = chuCaiDau
            };
        }

        public async Task<CapNhatTaiKhoanCaNhanViewModel> GetCapNhatAsync(ClaimsPrincipal user)
        {
            var hoSo = await GetHoSoAsync(user);

            return new CapNhatTaiKhoanCaNhanViewModel
            {
                UserName = hoSo.UserName,
                HoTenNguoiDung = hoSo.HoTenNguoiDung,
                Email = hoSo.Email ?? string.Empty,
                SdtNguoiDung = hoSo.SdtNguoiDung,
                DiaChiNguoiDung = hoSo.DiaChiNguoiDung ?? string.Empty,
                NgaySinh = hoSo.NgaySinh,
                AnhDaiDienHienTai = hoSo.AnhDaiDien,
                ChuCaiDau = hoSo.ChuCaiDau,
                TenChucDanh = hoSo.TenChucDanh,
                VaiTroHeThong = hoSo.VaiTroHeThong,
                TeamHienTai = hoSo.TeamHienTai
            };
        }

        public async Task CapNhatAsync(ClaimsPrincipal user, CapNhatTaiKhoanCaNhanViewModel model)
        {
            var aspUserId = GetCurrentAspUserId(user);

            var account = await _context.Aspnetusers
                .FirstOrDefaultAsync(x => x.Id == aspUserId);
            if (account == null)
            {
                throw new Exception("Không tìm thấy tài khoản hiện tại.");
            }

            var nguoiDung = await _context.NguoiDung
                .FirstOrDefaultAsync(x => x.MaNguoiDung == account.MaNguoiDung && x.IsDeleted != true);
            if (nguoiDung == null)
            {
                throw new Exception("Không tìm thấy hồ sơ nhân sự tương ứng.");
            }

            var hoTen = model.HoTenNguoiDung?.Trim() ?? string.Empty;
            var sdt = model.SdtNguoiDung?.Trim();
            var diaChi = model.DiaChiNguoiDung?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                throw new Exception("Họ tên không được để trống.");
            }

            if (model.NgaySinh.HasValue && model.NgaySinh.Value.Date > DateTime.Today)
            {
                throw new Exception("Ngày sinh không được lớn hơn ngày hiện tại.");
            }

            if (!string.IsNullOrWhiteSpace(sdt))
            {
                if (sdt.Length < 8 || sdt.Length > 20)
                {
                    throw new Exception("Số điện thoại phải từ 8 đến 20 ký tự.");
                }

                if (!sdt.All(char.IsDigit))
                {
                    throw new Exception("Số điện thoại chỉ được nhập số.");
                }

                var sdtTrung = await _context.NguoiDung
                    .AnyAsync(x => x.MaNguoiDung != nguoiDung.MaNguoiDung
                                   && x.IsDeleted != true
                                   && x.SdtNguoiDung == sdt);
                if (sdtTrung)
                {
                    throw new Exception("Số điện thoại đã tồn tại trong hệ thống.");
                }
            }

            var duongDanAnhMoi = nguoiDung.AnhDaiDien;
            if (model.AnhDaiDienFile != null)
            {
                duongDanAnhMoi = await LuuAnhDaiDienAsync(model.AnhDaiDienFile, nguoiDung.MaNguoiDung, nguoiDung.AnhDaiDien);
            }

            nguoiDung.HoTenNguoiDung = hoTen;
            nguoiDung.DiaChiNguoiDung = string.IsNullOrWhiteSpace(diaChi) ? null : diaChi;
            nguoiDung.SdtNguoiDung = string.IsNullOrWhiteSpace(sdt) ? null : sdt;
            nguoiDung.NgaySinh = model.NgaySinh;
            nguoiDung.AnhDaiDien = duongDanAnhMoi;

            account.PhoneNumber = string.IsNullOrWhiteSpace(sdt) ? null : sdt;

            await _context.SaveChangesAsync();
        }

        public async Task DoiMatKhauAsync(ClaimsPrincipal user, DoiMatKhauViewModel model)
        {
            var aspUserId = GetCurrentAspUserId(user);

            var account = await _context.Aspnetusers
                .FirstOrDefaultAsync(x => x.Id == aspUserId);
            if (account == null)
            {
                throw new Exception("Không tìm thấy tài khoản hiện tại.");
            }

            if (string.IsNullOrWhiteSpace(account.PasswordHash))
            {
                throw new Exception("Tài khoản chưa có mật khẩu hợp lệ.");
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(
                account,
                account.PasswordHash,
                model.MatKhauHienTai);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                throw new Exception("Mật khẩu hiện tại không chính xác.");
            }

            if (string.Equals(model.MatKhauHienTai, model.MatKhauMoi, StringComparison.Ordinal))
            {
                throw new Exception("Mật khẩu mới không được trùng mật khẩu hiện tại.");
            }

            account.PasswordHash = _passwordHasher.HashPassword(account, model.MatKhauMoi);
            account.SecurityStamp = Guid.NewGuid().ToString("N");
            account.ConcurrencyStamp = Guid.NewGuid().ToString("N");
            account.AccessFailedCount = 0;

            await _context.SaveChangesAsync();
        }

        private static string GetCurrentAspUserId(ClaimsPrincipal user)
        {
            var aspUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(aspUserId))
            {
                throw new Exception("Không xác định được người dùng hiện tại.");
            }

            return aspUserId;
        }

        private async Task<string> LuuAnhDaiDienAsync(IFormFile file, int maNguoiDung, string? duongDanAnhCu)
        {
            var extension = Path.GetExtension(file.FileName ?? string.Empty).ToLowerInvariant();
            if (!AllowedAvatarExtensions.Contains(extension))
            {
                throw new Exception("Ảnh đại diện chỉ hỗ trợ định dạng JPG, JPEG, PNG hoặc WEBP.");
            }

            if (file.Length <= 0)
            {
                throw new Exception("Tệp ảnh đại diện không hợp lệ.");
            }

            if (file.Length > MaxAvatarSizeBytes)
            {
                throw new Exception("Ảnh đại diện vượt quá dung lượng tối đa 2MB.");
            }

            var uploadFolder = Path.Combine(GetWebRootPath(), "uploads", "avatars");
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"avatar_{maNguoiDung}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
            var physicalPath = Path.Combine(uploadFolder, fileName);

            await using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            XoaAnhCuNeuCan(duongDanAnhCu);

            return $"/uploads/avatars/{fileName}";
        }

        private string GetWebRootPath()
        {
            if (!string.IsNullOrWhiteSpace(_environment.WebRootPath))
            {
                return _environment.WebRootPath;
            }

            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        private void XoaAnhCuNeuCan(string? duongDanAnhCu)
        {
            if (string.IsNullOrWhiteSpace(duongDanAnhCu))
            {
                return;
            }

            var startsWithAvatarFolder = duongDanAnhCu.StartsWith("/uploads/avatars/", StringComparison.OrdinalIgnoreCase)
                || duongDanAnhCu.StartsWith("~/uploads/avatars/", StringComparison.OrdinalIgnoreCase);
            if (!startsWithAvatarFolder)
            {
                return;
            }

            var relativePath = duongDanAnhCu.TrimStart('~').TrimStart('/', '\\');
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(GetWebRootPath(), relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
