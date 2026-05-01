using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.NhanSu;

namespace QuanLyDuAn.Services.Implementations
{
    public class NhanSuService : INhanSuService
    {
        private const int MaxAdminCount = 3;
        private readonly QuanLyDuAnDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<NhanSuService> _logger;
        private readonly PasswordHasher<Aspnetusers> _passwordHasher = new();

        public NhanSuService(QuanLyDuAnDbContext context, IEmailService emailService, ILogger<NhanSuService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<List<NhanSuViewModel>> GetAllAsync(string? tuKhoa, int? maChucDanh, string? trangThaiTaiKhoan)
        {
            var now = DateTime.UtcNow;

            var query = from nd in _context.NguoiDung
                        join cd in _context.ChucDanh on nd.MaChucDanh equals cd.MaChucDanh
                        join tk in _context.Aspnetusers on nd.Id equals tk.Id into tkJoin
                        from tk in tkJoin.DefaultIfEmpty()
                        where nd.IsDeleted != true
                        orderby nd.MaNguoiDung descending
                        select new NhanSuViewModel
                        {
                            MaNguoiDung = nd.MaNguoiDung,
                            HoTenNguoiDung = nd.HoTenNguoiDung ?? string.Empty,
                            DiaChiNguoiDung = nd.DiaChiNguoiDung,
                            SdtNguoiDung = nd.SdtNguoiDung,
                            NgaySinh = nd.NgaySinh,
                            MaChucDanh = nd.MaChucDanh,
                            TenChucDanh = cd.TenChucDanh ?? string.Empty,
                            UserName = tk != null ? tk.UserName : null,
                            Email = tk != null ? tk.Email : null,
                            CoTaiKhoan = tk != null,
                            TaiKhoanBiKhoa = tk != null && tk.LockoutEnd.HasValue && tk.LockoutEnd.Value > now
                        };

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    x.HoTenNguoiDung.ToLower().Contains(keyword) ||
                    (x.SdtNguoiDung != null && x.SdtNguoiDung.Contains(keyword)) ||
                    (x.UserName != null && x.UserName.ToLower().Contains(keyword)) ||
                    (x.Email != null && x.Email.ToLower().Contains(keyword)));
            }

            if (maChucDanh.HasValue && maChucDanh.Value > 0)
            {
                query = query.Where(x => x.MaChucDanh == maChucDanh.Value);
            }

            if (!string.IsNullOrWhiteSpace(trangThaiTaiKhoan))
            {
                switch (trangThaiTaiKhoan.Trim().ToLowerInvariant())
                {
                    //case "co":
                    //    query = query.Where(x => x.CoTaiKhoan);
                    //    break;
                    //case "chuaco":
                    //    query = query.Where(x => !x.CoTaiKhoan);
                    //    break;
                    case TrangThai.TaiKhoanKhoa:
                        query = query.Where(x => x.CoTaiKhoan && x.TaiKhoanBiKhoa);
                        break;
                    case TrangThai.TaiKhoanHoatDong:
                        query = query.Where(x => x.CoTaiKhoan && !x.TaiKhoanBiKhoa);
                        break;
                }
            }

            return await query.ToListAsync();
        }

        public async Task<NhanSuCreateUpdateViewModel?> GetByIdAsync(int id)
        {
            var data = await (from nd in _context.NguoiDung
                              join tk in _context.Aspnetusers on nd.Id equals tk.Id into tkJoin
                              from tk in tkJoin.DefaultIfEmpty()
                              join ur in _context.Aspnetuserroles on tk.Id equals ur.Asp_Id into urJoin
                              from ur in urJoin.DefaultIfEmpty()
                              where nd.MaNguoiDung == id && nd.IsDeleted != true
                              select new
                              {
                                  nd.MaNguoiDung,
                                  nd.HoTenNguoiDung,
                                  nd.DiaChiNguoiDung,
                                  nd.SdtNguoiDung,
                                  nd.NgaySinh,
                                  nd.MaChucDanh,
                                  UserName = tk != null ? tk.UserName : null,
                                  Email = tk != null ? tk.Email : null,
                                  RoleId = ur != null ? ur.Id : null
                              }).FirstOrDefaultAsync();

            if (data == null)
            {
                return null;
            }

            return new NhanSuCreateUpdateViewModel
            {
                MaNguoiDung = data.MaNguoiDung,
                HoTenNguoiDung = data.HoTenNguoiDung ?? string.Empty,
                DiaChiNguoiDung = data.DiaChiNguoiDung,
                SdtNguoiDung = data.SdtNguoiDung,
                NgaySinh = data.NgaySinh,
                MaChucDanh = data.MaChucDanh,
                UserName = data.UserName,
                Email = data.Email,
                RoleId = data.RoleId
            };
        }

        public async Task<List<ChucDanhOptionViewModel>> GetChucDanhOptionsAsync()
        {
            return await _context.ChucDanh
                .OrderBy(x => x.TenChucDanh)
                .Select(x => new ChucDanhOptionViewModel
                {
                    MaChucDanh = x.MaChucDanh,
                    TenChucDanh = x.TenChucDanh ?? $"Chức danh {x.MaChucDanh}"
                })
                .ToListAsync();
        }

        public async Task<List<VaiTroHeThongOptionViewModel>> GetVaiTroHeThongOptionsAsync()
        {
            return await _context.Aspnetroles
                .OrderBy(x => x.Name)
                .Select(x => new VaiTroHeThongOptionViewModel
                {
                    RoleId = x.Id,
                    RoleName = x.Name ?? x.NormalizedName ?? x.Id
                })
                .ToListAsync();
        }

        public async Task<string?> SaveAsync(NhanSuCreateUpdateViewModel model, bool laAdminDangThaoTac)
        {
            string? warningMessage = null;
            if (!model.MaChucDanh.HasValue)
            {
                throw new Exception("Vui lòng chọn chức danh.");
            }

            var maChucDanh = model.MaChucDanh.Value;

            var chucDanhExists = await _context.ChucDanh
                .AnyAsync(x => x.MaChucDanh == maChucDanh);

            if (!chucDanhExists)
            {
                throw new Exception("Chức danh không tồn tại.");
            }

            if (model.MaNguoiDung == null)
            {
                var userName = model.UserName.Trim();
                var normalizedUserName = userName.ToUpperInvariant();
                var email = model.Email.Trim();
                var normalizedEmail = email.ToUpperInvariant();

                var userNameExists = await _context.Aspnetusers
                    .AnyAsync(x => x.NormalizedUserName == normalizedUserName);
                if (userNameExists)
                {
                    throw new Exception("Tên đăng nhập đã tồn tại.");
                }

                var emailExists = await _context.Aspnetusers
                    .AnyAsync(x => x.NormalizedEmail == normalizedEmail);
                if (emailExists)
                {
                    throw new Exception("Email đã tồn tại.");
                }

                var sdt = model.SdtNguoiDung.Trim();

                var sdtExists = await _context.NguoiDung
                    .AnyAsync(x => x.SdtNguoiDung == sdt && x.IsDeleted != true);

                if (sdtExists)
                {
                    throw new Exception("Số điện thoại đã tồn tại.");
                }

                var roleExists = await _context.Aspnetroles
                    .AnyAsync(x => x.Id == model.RoleId);
                if (!roleExists)
                {
                    throw new Exception("Vai trò hệ thống không tồn tại.");
                }

                var roleMoiLaAdmin = await IsAdminRoleAsync(model.RoleId);
                if (roleMoiLaAdmin && !laAdminDangThaoTac)
                {
                    throw new Exception("Chỉ Admin mới được tạo tài khoản Admin.");
                }

                if (roleMoiLaAdmin)
                {
                    var soLuongAdmin = await CountAdminUsersAsync();
                    if (soLuongAdmin >= MaxAdminCount)
                    {
                        throw new Exception($"Hệ thống chỉ cho phép tối đa {MaxAdminCount} tài khoản Admin.");
                    }
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();
                var entity = new NguoiDung
                {
                    MaChucDanh = maChucDanh,
                    HoTenNguoiDung = model.HoTenNguoiDung,
                    DiaChiNguoiDung = model.DiaChiNguoiDung,
                    SdtNguoiDung = model.SdtNguoiDung,
                    NgaySinh = model.NgaySinh,
                    IsDeleted = false
                };

                _context.NguoiDung.Add(entity);
                await _context.SaveChangesAsync();

                var userId = Guid.NewGuid().ToString("N");
                var account = new Aspnetusers
                {
                    Id = userId,
                    MaNguoiDung = entity.MaNguoiDung,
                    UserName = userName,
                    NormalizedUserName = normalizedUserName,
                    Email = email,
                    NormalizedEmail = normalizedEmail,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("N"),
                    ConcurrencyStamp = Guid.NewGuid().ToString("N"),
                    PhoneNumber = model.SdtNguoiDung,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                };
                account.PasswordHash = _passwordHasher.HashPassword(account, model.Password!);

                _context.Aspnetusers.Add(account);
                _context.Aspnetuserroles.Add(new Aspnetuserroles
                {
                    Asp_Id = userId,
                    Id = model.RoleId
                });

                entity.Id = userId;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return warningMessage;
            }
            else
            {
                var maNguoiDung = model.MaNguoiDung.Value;
                var entity = await _context.NguoiDung
                    .FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung && x.IsDeleted != true);

                if (entity == null)
                {
                    throw new Exception("Không tìm thấy nhân sự.");
                }

                if (string.IsNullOrWhiteSpace(entity.Id))
                {
                    throw new Exception("Dữ liệu nhân sự chưa có tài khoản đi kèm. Vui lòng chuẩn hóa dữ liệu trước khi sửa.");
                }

                var account = await _context.Aspnetusers.FirstOrDefaultAsync(x => x.Id == entity.Id);
                if (account == null)
                {
                    throw new Exception("Không tìm thấy tài khoản hệ thống đi kèm nhân sự.");
                }

                var userRole = await _context.Aspnetuserroles.FirstOrDefaultAsync(x => x.Asp_Id == account.Id);
                if (userRole == null)
                {
                    throw new Exception("Không tìm thấy phân quyền vai trò của tài khoản.");
                }

                var roleExists = await _context.Aspnetroles.AnyAsync(x => x.Id == model.RoleId);
                if (!roleExists)
                {
                    throw new Exception("Vai trò hệ thống không tồn tại.");
                }

                var sdt = model.SdtNguoiDung.Trim();

                var sdtExists = await _context.NguoiDung
                    .AnyAsync(x => x.SdtNguoiDung == sdt
                                && x.MaNguoiDung != model.MaNguoiDung
                                && x.IsDeleted != true);

                if (sdtExists)
                {
                    throw new Exception("Số điện thoại đã tồn tại.");
                }

                var coDoiRole = !string.Equals(userRole.Id, model.RoleId, StringComparison.Ordinal);
                if (coDoiRole)
                {
                    var roleMoiLaAdmin = await IsAdminRoleAsync(model.RoleId);
                    if (roleMoiLaAdmin && !laAdminDangThaoTac)
                    {
                        throw new Exception("Chỉ Admin mới được gán vai trò Admin.");
                    }

                    if (roleMoiLaAdmin)
                    {
                        var soLuongAdmin = await CountAdminUsersAsync();
                        if (soLuongAdmin >= MaxAdminCount)
                        {
                            throw new Exception($"Hệ thống chỉ cho phép tối đa {MaxAdminCount} tài khoản Admin.");
                        }
                    }

                    var lyDoChan = await KiemTraRangBuocDoiRoleAsync(entity.MaNguoiDung);
                    if (!string.IsNullOrEmpty(lyDoChan))
                    {
                        throw new Exception(lyDoChan);
                    }

                    userRole.Id = model.RoleId;
                }

                entity.MaChucDanh = maChucDanh;
                entity.HoTenNguoiDung = model.HoTenNguoiDung;
                entity.DiaChiNguoiDung = model.DiaChiNguoiDung;
                entity.SdtNguoiDung = model.SdtNguoiDung;
                entity.NgaySinh = model.NgaySinh;

                var emailMoi = model.Email.Trim();
                var normalizedEmailMoi = emailMoi.ToUpperInvariant();
                var normalizedEmailHienTai = (account.Email ?? string.Empty).Trim().ToUpperInvariant();
                if (!string.Equals(normalizedEmailMoi, normalizedEmailHienTai, StringComparison.Ordinal))
                {
                    throw new Exception("Không được thay đổi email khi chỉnh sửa nhân sự.");
                }

                account.PhoneNumber = model.SdtNguoiDung;

                if (!string.IsNullOrWhiteSpace(model.ResetPassword))
                {
                    account.PasswordHash = _passwordHasher.HashPassword(account, model.ResetPassword);
                    account.SecurityStamp = Guid.NewGuid().ToString("N");
                }

                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(model.ResetPassword))
                {
                    if (string.IsNullOrWhiteSpace(account.Email))
                    {
                        warningMessage = "Đã reset mật khẩu nhưng tài khoản chưa có email để gửi thông báo.";
                    }
                    else
                    {
                        try
                        {
                            var body = $"Xin chào {entity.HoTenNguoiDung},\n\nMật khẩu tài khoản của bạn đã được đặt lại trong hệ thống QuanLyDuAn AI.\nNếu bạn không yêu cầu thao tác này, vui lòng liên hệ quản trị viên ngay.\n\nTrân trọng.";
                            await _emailService.SendAsync(account.Email, "Thong bao reset mat khau", body);
                        }
                        catch (Exception ex)
                        {
                            warningMessage = $"Đã reset mật khẩu nhưng gửi email thất bại: {ex.Message}";
                        }
                    }
                }

                return warningMessage;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.NguoiDung
                .FirstOrDefaultAsync(x => x.MaNguoiDung == id && x.IsDeleted != true);

            if (entity == null)
            {
                throw new Exception("Không tìm thấy nhân sự.");
            }

            await KiemTraRangBuocXoaAsync(id);

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private async Task KiemTraRangBuocXoaAsync(int maNguoiDung)
        {
            var dangQuanLyDuAn = await _context.DuAn
                .AnyAsync(x => x.MaNguoiDung == maNguoiDung && x.IsDeleted != true);
            if (dangQuanLyDuAn)
            {
                throw new Exception("Không thể xóa: nhân sự đang quản lý dự án.");
            }

            var dangLaLeader = await _context.NhanVienTeam
                .AnyAsync(x => x.MaNguoiDung == maNguoiDung && x.IsLeader == true);
            if (dangLaLeader)
            {
                throw new Exception("Không thể xóa: nhân sự đang là trưởng nhóm.");
            }

            var dangThuocTeam = await _context.NhanVienTeam
                .AnyAsync(x => x.MaNguoiDung == maNguoiDung);
            if (dangThuocTeam)
            {
                throw new Exception("Không thể xóa: nhân sự còn thuộc team.");
            }

            var dangThuocDuAn = await _context.NhanVienDuAn
                .AnyAsync(x => x.MaNguoiDung == maNguoiDung);
            if (dangThuocDuAn)
            {
                throw new Exception("Không thể xóa: nhân sự còn thuộc dự án.");
            }

            var dangThuocTeamDangThamGiaDuAn = await (
                from nvt in _context.NhanVienTeam
                join tda in _context.TeamDuAn on nvt.MaTeam equals tda.MaTeam
                where nvt.MaNguoiDung == maNguoiDung
                select tda.MaDuAn
            ).AnyAsync();
            if (dangThuocTeamDangThamGiaDuAn)
            {
                throw new Exception("Không thể xóa: nhân sự thuộc team đang tham gia dự án.");
            }

            var hoanThanh = TrangThai.HoanThanh.ToUpperInvariant();
            var hoanThanhHienThi = TrangThai.HoanThanhHienThi.ToUpperInvariant();
            var done = TrangThai.Done.ToUpperInvariant();
            var completed = TrangThai.Completed.ToUpperInvariant();

            var conCongViecChuaHoanThanh = await (
                from pc in _context.PhanCongCongViec
                join cv in _context.CongViec on pc.MaCongViec equals cv.MaCongViec
                where pc.MaNguoiDung == maNguoiDung
                      && cv.IsDeleted != true
                      && (
                          string.IsNullOrWhiteSpace(cv.TrangThaiCongViec)
                          || (
                              cv.TrangThaiCongViec.Trim().ToUpper() != hoanThanh
                              && cv.TrangThaiCongViec.Trim().ToUpper() != hoanThanhHienThi
                              && cv.TrangThaiCongViec.Trim().ToUpper() != done
                              && cv.TrangThaiCongViec.Trim().ToUpper() != completed
                          )
                      )
                select cv.MaCongViec
            ).AnyAsync();
            if (conCongViecChuaHoanThanh)
            {
                throw new Exception("Không thể xóa: nhân sự còn công việc chưa hoàn thành.");
            }
        }

        public async Task LockAccountAsync(int id)
        {
            await KiemTraRangBuocKhoaAsync(id);

            var userId = await _context.NguoiDung
                .Where(x => x.MaNguoiDung == id && x.IsDeleted != true)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("Nhân sự chưa có tài khoản hệ thống.");
            }

            var account = await _context.Aspnetusers.FirstOrDefaultAsync(x => x.Id == userId);
            if (account == null)
            {
                throw new Exception("Không tìm thấy tài khoản hệ thống.");
            }

            account.LockoutEnabled = true;
            account.LockoutEnd = DateTime.UtcNow.AddYears(100);

            await _context.SaveChangesAsync();
        }

        private async Task KiemTraRangBuocKhoaAsync(int maNguoiDung)
        {
            var tonTaiNhanSu = await _context.NguoiDung
                .AnyAsync(x => x.MaNguoiDung == maNguoiDung && x.IsDeleted != true);
            if (!tonTaiNhanSu)
            {
                throw new Exception("Không tìm thấy nhân sự.");
            }

            var dangQuanLyDuAn = await _context.DuAn
                .AnyAsync(x => x.MaNguoiDung == maNguoiDung && x.IsDeleted != true);
            if (dangQuanLyDuAn)
            {
                throw new Exception("Không thể khóa: nhân sự đang quản lý dự án. Hãy chuyển dự án trước.");
            }

            var dangLaLeader = await _context.NhanVienTeam
                .AnyAsync(x => x.MaNguoiDung == maNguoiDung && x.IsLeader == true);
            if (dangLaLeader)
            {
                throw new Exception("Không thể khóa: nhân sự đang là trưởng nhóm. Hãy gỡ leader trước.");
            }

            // Bonus: chỉ cảnh báo nghiệp vụ nội bộ, không chặn thao tác khóa tài khoản.
            var hoanThanh = TrangThai.HoanThanh.ToUpperInvariant();
            var hoanThanhHienThi = TrangThai.HoanThanhHienThi.ToUpperInvariant();
            var done = TrangThai.Done.ToUpperInvariant();
            var completed = TrangThai.Completed.ToUpperInvariant();

            var conTaskChuaHoanThanh = await (
                from pc in _context.PhanCongCongViec
                join cv in _context.CongViec on pc.MaCongViec equals cv.MaCongViec
                where pc.MaNguoiDung == maNguoiDung
                      && cv.IsDeleted != true
                      && (
                          string.IsNullOrWhiteSpace(cv.TrangThaiCongViec)
                          || (
                              cv.TrangThaiCongViec.Trim().ToUpper() != hoanThanh
                              && cv.TrangThaiCongViec.Trim().ToUpper() != hoanThanhHienThi
                              && cv.TrangThaiCongViec.Trim().ToUpper() != done
                              && cv.TrangThaiCongViec.Trim().ToUpper() != completed
                          )
                      )
                select cv.MaCongViec
            ).AnyAsync();

            if (conTaskChuaHoanThanh)
            {
                _logger.LogWarning("Nhan su {MaNguoiDung} con cong viec chua hoan thanh khi khoa tai khoan.", maNguoiDung);
            }
        }

        public async Task UnlockAccountAsync(int id)
        {
            var userId = await _context.NguoiDung
                .Where(x => x.MaNguoiDung == id && x.IsDeleted != true)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("Nhân sự chưa có tài khoản hệ thống.");
            }

            var account = await _context.Aspnetusers.FirstOrDefaultAsync(x => x.Id == userId);
            if (account == null)
            {
                throw new Exception("Không tìm thấy tài khoản hệ thống.");
            }

            account.LockoutEnd = null;
            account.AccessFailedCount = 0;

            await _context.SaveChangesAsync();
        }

        private async Task<bool> IsAdminRoleAsync(string? roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                return false;
            }

            var roleName = await _context.Aspnetroles
                .Where(x => x.Id == roleId)
                .Select(x => x.Name ?? x.NormalizedName ?? string.Empty)
                .FirstOrDefaultAsync();

            return IsAdminRoleName(roleName);
        }

        private async Task<int> CountAdminUsersAsync()
        {
            var adminRoleIds = await _context.Aspnetroles
                .Where(x => (x.Name != null && x.Name.ToUpper() == "ADMIN")
                            || (x.NormalizedName != null && x.NormalizedName.ToUpper() == "ADMIN"))
                .Select(x => x.Id)
                .ToListAsync();

            if (adminRoleIds.Count == 0)
            {
                return 0;
            }

            return await _context.Aspnetuserroles
                .CountAsync(x => adminRoleIds.Contains(x.Id));
        }

        private static bool IsAdminRoleName(string? roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return false;
            }

            var normalized = roleName.Trim().ToUpperInvariant();
            return normalized == "ADMIN";
        }

        private async Task<string?> KiemTraRangBuocDoiRoleAsync(int maNguoiDung)
        {
            var dangQuanLyDuAn = await _context.DuAn
                .AnyAsync(x => x.MaNguoiDung == maNguoiDung && x.IsDeleted != true);
            if (dangQuanLyDuAn)
            {
                return "Không thể đổi role: nhân sự đang quản lý dự án. Hãy chuyển dự án sang người khác trước.";
            }

            var dangLaLeader = await _context.NhanVienTeam
                .AnyAsync(x => x.MaNguoiDung == maNguoiDung && x.IsLeader == true);
            if (dangLaLeader)
            {
                return "Không thể đổi role: nhân sự vẫn đang là leader của team. Hãy gỡ vai trò leader trước.";
            }

            var dangThuocNhanVienDuAn = await _context.NhanVienDuAn
                .AnyAsync(x => x.MaNguoiDung == maNguoiDung);
            if (dangThuocNhanVienDuAn)
            {
                return "Không thể đổi role: nhân sự vẫn còn trong NHAN_VIEN_DU_AN. Hãy gỡ khỏi dự án trước.";
            }

            var dangThuocTeamDuAn = await (
                from nvt in _context.NhanVienTeam
                join tda in _context.TeamDuAn on nvt.MaTeam equals tda.MaTeam
                where nvt.MaNguoiDung == maNguoiDung
                select tda.MaDuAn
            ).AnyAsync();
            if (dangThuocTeamDuAn)
            {
                return "Không thể đổi role: nhân sự thuộc team đang tham gia dự án (TEAM_DU_AN). Hãy gỡ khỏi team trước.";
            }

            var hoanThanh = TrangThai.HoanThanh.ToUpperInvariant();
            var hoanThanhHienThi = TrangThai.HoanThanhHienThi.ToUpperInvariant();
            var done = TrangThai.Done.ToUpperInvariant();
            var completed = TrangThai.Completed.ToUpperInvariant();

            var conCongViecChuaHoanThanh = await (
                from pc in _context.PhanCongCongViec
                join cv in _context.CongViec on pc.MaCongViec equals cv.MaCongViec
                where pc.MaNguoiDung == maNguoiDung
                      && cv.IsDeleted != true
                      && (
                          string.IsNullOrWhiteSpace(cv.TrangThaiCongViec)
                          || (
                              cv.TrangThaiCongViec.Trim().ToUpper() != hoanThanh
                              && cv.TrangThaiCongViec.Trim().ToUpper() != hoanThanhHienThi
                              && cv.TrangThaiCongViec.Trim().ToUpper() != done
                              && cv.TrangThaiCongViec.Trim().ToUpper() != completed
                          )
                      )
                select cv.MaCongViec
            ).AnyAsync();
            if (conCongViecChuaHoanThanh)
            {
                return "Không thể đổi role: nhân sự còn công việc chưa hoàn thành trong PHAN_CONG_CONG_VIEC. Hãy clear phân công trước.";
            }

            return null;
        }

    }
}
