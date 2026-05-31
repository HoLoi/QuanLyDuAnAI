using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DuyetYeuCauDoiQuanLy;
using System.Data;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class DuyetYeuCauDoiQuanLyService : IDuyetYeuCauDoiQuanLyService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DuyetYeuCauDoiQuanLyService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DuyetYeuCauDoiQuanLyPageViewModel> GetPageAsync(string? trangThai, int? maDuAn, string? tuKhoa)
        {
            var currentUser = await GetCurrentUserContextAsync();
            await EnsureCanReviewAsync(currentUser.AspUserId);

            var query = BuildBaseQuery();

            if (maDuAn.HasValue && maDuAn.Value > 0)
            {
                query = query.Where(x => x.MaDuAn == maDuAn.Value);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                var statuses = TrangThai.GetCommonStatusVariants(trangThai);
                if (statuses.Length > 0)
                {
                    query = query.Where(x => statuses.Contains(x.TrangThaiYeuCauDoiQuanLy));
                }
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    x.TenDuAn.ToLower().Contains(keyword)
                    || x.TenQuanLyHienTai.ToLower().Contains(keyword)
                    || x.TenQuanLyDeXuat.ToLower().Contains(keyword));
            }

            var danhSach = await query
                .OrderByDescending(x => x.NgayTaoYeuCauDoiQuanLy)
                .ThenByDescending(x => x.MaYeuCauDoiQuanLy)
                .ToListAsync();

            return new DuyetYeuCauDoiQuanLyPageViewModel
            {
                TrangThai = trangThai,
                MaDuAn = maDuAn,
                TuKhoa = tuKhoa,
                DanhSachYeuCau = danhSach
            };
        }

        public async Task<DuyetYeuCauDoiQuanLyDetailsViewModel> GetDetailsAsync(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Yêu cầu đổi quản lý không hợp lệ.");
            }

            var currentUser = await GetCurrentUserContextAsync();
            await EnsureCanReviewAsync(currentUser.AspUserId);

            var item = await BuildBaseQuery()
                .FirstOrDefaultAsync(x => x.MaYeuCauDoiQuanLy == id);

            if (item == null)
            {
                throw new Exception("Không tìm thấy yêu cầu đổi quản lý.");
            }

            return new DuyetYeuCauDoiQuanLyDetailsViewModel
            {
                YeuCau = item
            };
        }

        public async Task ApproveAsync(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Yêu cầu đổi quản lý không hợp lệ.");
            }

            var currentUser = await GetCurrentUserContextAsync();
            await EnsureCanReviewAsync(currentUser.AspUserId);

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var yeuCau = await _context.YeuCauDoiQuanLy
                .FirstOrDefaultAsync(x => x.MaYeuCauDoiQuanLy == id && x.IsDeleted != true);

            if (yeuCau == null)
            {
                throw new Exception("Không tìm thấy yêu cầu đổi quản lý.");
            }

            if (!TrangThai.EqualsValue(yeuCau.TrangThaiYeuCauDoiQuanLy, TrangThai.ChoDuyet))
            {
                throw new Exception("Yêu cầu đã được xử lý trước đó.");
            }

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == yeuCau.MaDuAn && x.IsDeleted != true);

            if (duAn == null)
            {
                throw new Exception("Không tìm thấy dự án của yêu cầu.");
            }

            if (duAn.MaNguoiDung != yeuCau.MaQuanLyHienTai)
            {
                throw new Exception("Quản lý hiện tại của dự án đã thay đổi. Không thể duyệt yêu cầu này.");
            }

            await EnsureManagerCandidateAsync(yeuCau.MaQuanLyDeXuat);

            var now = DateTime.Now;
            yeuCau.TrangThaiYeuCauDoiQuanLy = TrangThai.DaDuyet;
            yeuCau.MaNguoiDungDuyet = currentUser.MaNguoiDung;
            yeuCau.NgayDuyetYeuCauDoiQuanLy = now;

            duAn.MaNguoiDung = yeuCau.MaQuanLyDeXuat;

            var tenQuanLyCu = await _context.NguoiDung
                .Where(x => x.MaNguoiDung == yeuCau.MaQuanLyHienTai)
                .Select(x => x.HoTenNguoiDung)
                .FirstOrDefaultAsync() ?? $"Nhân sự #{yeuCau.MaQuanLyHienTai}";

            var tenQuanLyMoi = await _context.NguoiDung
                .Where(x => x.MaNguoiDung == yeuCau.MaQuanLyDeXuat)
                .Select(x => x.HoTenNguoiDung)
                .FirstOrDefaultAsync() ?? $"Nhân sự #{yeuCau.MaQuanLyDeXuat}";

            var logQuanLyCuDangMo = await _context.NhatKyQuanLyDuAn
                .Where(x => x.MaDuAn == yeuCau.MaDuAn
                            && x.MaNguoiDung == yeuCau.MaQuanLyHienTai
                            && x.QLDADenNgay == null)
                .OrderByDescending(x => x.QLDATuNgay ?? x.NkThoiGianQLDA ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaNhatKyQLDA)
                .FirstOrDefaultAsync();

            if (logQuanLyCuDangMo != null)
            {
                logQuanLyCuDangMo.QLDADenNgay = now;
            }

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = yeuCau.MaDuAn,
                MaNguoiDung = yeuCau.MaQuanLyDeXuat,
                NkHanhDongQLDA = $"Duyệt yêu cầu đổi quản lý: {tenQuanLyCu} -> {tenQuanLyMoi}",
                NkThoiGianQLDA = now,
                QLDATuNgay = now,
                QLDADenNgay = null
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task RejectAsync(int id, string? lyDoTuChoi)
        {
            if (id <= 0)
            {
                throw new Exception("Yêu cầu đổi quản lý không hợp lệ.");
            }

            var currentUser = await GetCurrentUserContextAsync();
            await EnsureCanReviewAsync(currentUser.AspUserId);

            var yeuCau = await _context.YeuCauDoiQuanLy
                .FirstOrDefaultAsync(x => x.MaYeuCauDoiQuanLy == id && x.IsDeleted != true);

            if (yeuCau == null)
            {
                throw new Exception("Không tìm thấy yêu cầu đổi quản lý.");
            }

            if (!TrangThai.EqualsValue(yeuCau.TrangThaiYeuCauDoiQuanLy, TrangThai.ChoDuyet))
            {
                throw new Exception("Yêu cầu đã được xử lý trước đó.");
            }

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == yeuCau.MaDuAn && x.IsDeleted != true);

            if (duAn == null)
            {
                throw new Exception("Không tìm thấy dự án của yêu cầu.");
            }

            var now = DateTime.Now;
            yeuCau.TrangThaiYeuCauDoiQuanLy = TrangThai.TuChoi;
            yeuCau.MaNguoiDungDuyet = currentUser.MaNguoiDung;
            yeuCau.NgayDuyetYeuCauDoiQuanLy = now;

            var lyDoPart = string.IsNullOrWhiteSpace(lyDoTuChoi)
                ? string.Empty
                : $". Lý do: {lyDoTuChoi.Trim()}";

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = yeuCau.MaDuAn,
                MaNguoiDung = currentUser.MaNguoiDung,
                NkHanhDongQLDA = $"Từ chối yêu cầu đổi quản lý #{yeuCau.MaYeuCauDoiQuanLy}{lyDoPart}",
                NkThoiGianQLDA = now
            });

            await _context.SaveChangesAsync();
        }

        private IQueryable<DuyetYeuCauDoiQuanLyItemViewModel> BuildBaseQuery()
        {
            var choDuyetStatuses = TrangThai.GetCommonStatusVariants(TrangThai.ChoDuyet);
            return
                from yc in _context.YeuCauDoiQuanLy
                join da in _context.DuAn on yc.MaDuAn equals da.MaDuAn
                join qlHienTai in _context.NguoiDung on yc.MaQuanLyHienTai equals qlHienTai.MaNguoiDung
                join qlDeXuat in _context.NguoiDung on yc.MaQuanLyDeXuat equals qlDeXuat.MaNguoiDung
                join nguoiDuyetLeft in _context.NguoiDung on yc.MaNguoiDungDuyet equals nguoiDuyetLeft.MaNguoiDung into nguoiDuyetGroup
                from nguoiDuyet in nguoiDuyetGroup.DefaultIfEmpty()
                where yc.IsDeleted != true
                      && da.IsDeleted != true
                select new DuyetYeuCauDoiQuanLyItemViewModel
                {
                    MaYeuCauDoiQuanLy = yc.MaYeuCauDoiQuanLy,
                    MaDuAn = yc.MaDuAn,
                    TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}",
                    MaQuanLyHienTai = yc.MaQuanLyHienTai,
                    TenQuanLyHienTai = qlHienTai.HoTenNguoiDung ?? $"Nhân sự {yc.MaQuanLyHienTai}",
                    MaQuanLyDeXuat = yc.MaQuanLyDeXuat,
                    TenQuanLyDeXuat = qlDeXuat.HoTenNguoiDung ?? $"Nhân sự {yc.MaQuanLyDeXuat}",
                    MaNguoiDungDuyet = yc.MaNguoiDungDuyet,
                    TenNguoiDungDuyet = nguoiDuyet != null ? (nguoiDuyet.HoTenNguoiDung ?? $"Nhân sự {nguoiDuyet.MaNguoiDung}") : null,
                    TrangThaiYeuCauDoiQuanLy = yc.TrangThaiYeuCauDoiQuanLy ?? string.Empty,
                    NgayTaoYeuCauDoiQuanLy = yc.NgayTaoYeuCauDoiQuanLy,
                    NgayDuyetYeuCauDoiQuanLy = yc.NgayDuyetYeuCauDoiQuanLy,
                    CoTheXuLy = choDuyetStatuses.Contains(yc.TrangThaiYeuCauDoiQuanLy ?? string.Empty)
                };
        }

        private async Task EnsureCanReviewAsync(string aspUserId)
        {
            var isAdmin = await HasRoleAsync(aspUserId, "ADMIN");
            var hasApprovePermission = await HasPermissionAsync(aspUserId, Permissions.DuyetYeuCauDoiQuanLy.Duyet);

            if (!isAdmin && !hasApprovePermission)
            {
                throw new Exception("Bạn không có quyền duyệt/từ chối yêu cầu đổi quản lý.");
            }
        }

        private async Task EnsureManagerCandidateAsync(int maNguoiDung)
        {
            var nguoiDung = await (
                from nd in _context.NguoiDung
                join cd in _context.ChucDanh on nd.MaChucDanh equals cd.MaChucDanh into chucDanhGroup
                from cd in chucDanhGroup.DefaultIfEmpty()
                where nd.MaNguoiDung == maNguoiDung && nd.IsDeleted != true
                select new
                {
                    nd.MaNguoiDung,
                    TenChucDanh = cd != null ? cd.TenChucDanh : null
                }
            ).FirstOrDefaultAsync();

            if (nguoiDung == null)
            {
                throw new Exception("Quản lý đề xuất không tồn tại hoặc đã bị xóa.");
            }

            var aspUserId = await _context.Aspnetusers
                .Where(x => x.MaNguoiDung == maNguoiDung)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var hasManagerRole = false;
            if (!string.IsNullOrWhiteSpace(aspUserId))
            {
                hasManagerRole = await HasRoleAsync(aspUserId, "MANAGER");
            }

            var normalizedTitle = NormalizeKeyword(nguoiDung.TenChucDanh);
            var hasManagerTitle = normalizedTitle.Contains("manager") || normalizedTitle.Contains("quanly");

            if (!hasManagerRole && !hasManagerTitle)
            {
                throw new Exception("Nhân sự đề xuất mới chưa có role/chức danh quản lý phù hợp.");
            }
        }

        private async Task<bool> HasRoleAsync(string aspUserId, string normalizedRole)
        {
            return await (
                from ur in _context.Aspnetuserroles
                join r in _context.Aspnetroles on ur.Id equals r.Id
                where ur.Asp_Id == aspUserId
                select (r.NormalizedName ?? r.Name ?? string.Empty).ToUpper()
            ).AnyAsync(x => x == normalizedRole);
        }

        private async Task<bool> HasPermissionAsync(string aspUserId, string permission)
        {
            var permissionLower = permission.ToLower();
            return await (
                from ur in _context.Aspnetuserroles
                join rc in _context.Aspnetroleclaims on ur.Id equals rc.Asp_Id
                where ur.Asp_Id == aspUserId
                select rc.ClaimValue
            ).AnyAsync(x => x != null && x.ToLower() == permissionLower);
        }

        private async Task<(int MaNguoiDung, string AspUserId)> GetCurrentUserContextAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(aspUserId))
            {
                throw new Exception("Không xác định được người dùng hiện tại.");
            }

            var maNguoiDung = await _context.Aspnetusers
                .Where(x => x.Id == aspUserId)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync();

            if (maNguoiDung <= 0)
            {
                throw new Exception("Không xác định được nhân sự tương ứng của người dùng hiện tại.");
            }

            return (maNguoiDung, aspUserId);
        }

        private static string NormalizeKeyword(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            var normalized = TrangThai.Normalize(input);
            return normalized.Replace(" ", string.Empty);
        }
    }
}
