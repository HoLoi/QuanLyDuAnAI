using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.YeuCauDoiQuanLy;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class YeuCauDoiQuanLyService : IYeuCauDoiQuanLyService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public YeuCauDoiQuanLyService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<YeuCauDoiQuanLyPageViewModel> GetPageAsync(int? maDuAn, string? trangThai, string? tuKhoa)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            await EnsureManagerCanCreateAsync(currentUserId);

            var managedProject = await _context.DuAn
                .Where(x => x.IsDeleted != true && x.MaNguoiDung == currentUserId)
                .OrderByDescending(x => x.NgayTaoDuAn)
                .ThenByDescending(x => x.MaDuAn)
                .FirstOrDefaultAsync(x => !maDuAn.HasValue || x.MaDuAn == maDuAn.Value);

            if (managedProject == null)
            {
                throw new Exception("Bạn chưa là quản lý của dự án nào để tạo yêu cầu đổi quản lý.");
            }

            var tenQuanLyHienTai = await _context.NguoiDung
                .Where(x => x.MaNguoiDung == managedProject.MaNguoiDung && x.IsDeleted != true)
                .Select(x => x.HoTenNguoiDung)
                .FirstOrDefaultAsync() ?? $"Nhân sự {managedProject.MaNguoiDung}";

            var managerOptions = await GetManagerOptionsAsync(managedProject.MaNguoiDung);
            var canCreateResult = await CanCreateFromProjectCoreAsync(managedProject.MaDuAn, currentUserId);
            var choDuyetStatuses = TrangThai.GetCommonStatusVariants(TrangThai.ChoDuyet);

            var query =
                from yc in _context.YeuCauDoiQuanLy
                join da in _context.DuAn on yc.MaDuAn equals da.MaDuAn
                join qlHienTai in _context.NguoiDung on yc.MaQuanLyHienTai equals qlHienTai.MaNguoiDung
                join qlDeXuat in _context.NguoiDung on yc.MaQuanLyDeXuat equals qlDeXuat.MaNguoiDung
                join nguoiDuyetLeft in _context.NguoiDung on yc.MaNguoiDungDuyet equals nguoiDuyetLeft.MaNguoiDung into nguoiDuyetGroup
                from nguoiDuyet in nguoiDuyetGroup.DefaultIfEmpty()
                where yc.IsDeleted != true
                      && da.IsDeleted != true
                      && yc.MaQuanLyHienTai == currentUserId
                select new YeuCauDoiQuanLyItemViewModel
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
                    CoTheHuy = choDuyetStatuses.Contains(yc.TrangThaiYeuCauDoiQuanLy ?? string.Empty)
                };

            query = query.Where(x => x.MaDuAn == managedProject.MaDuAn);

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

            var danhSachYeuCau = await query
                .OrderByDescending(x => x.NgayTaoYeuCauDoiQuanLy)
                .ThenByDescending(x => x.MaYeuCauDoiQuanLy)
                .ToListAsync();

            return new YeuCauDoiQuanLyPageViewModel
            {
                MaDuAn = managedProject.MaDuAn,
                TenDuAn = managedProject.TenDuAn ?? $"Dự án {managedProject.MaDuAn}",
                TrangThaiDuAn = managedProject.TrangThaiDuAn ?? string.Empty,
                MaQuanLyHienTai = managedProject.MaNguoiDung,
                TenQuanLyHienTai = tenQuanLyHienTai,
                TuKhoa = tuKhoa,
                TrangThai = trangThai,
                CoTheTaoYeuCauMoi = canCreateResult.CanCreate,
                LyDoKhongTheTaoYeuCau = canCreateResult.Reason,
                DanhSachQuanLy = managerOptions,
                DanhSachYeuCau = danhSachYeuCau,
                Form = new YeuCauDoiQuanLyCreateViewModel
                {
                    MaDuAn = managedProject.MaDuAn
                }
            };
        }

        public async Task CreateAsync(YeuCauDoiQuanLyCreateViewModel model)
        {
            if (model.MaDuAn <= 0)
            {
                throw new Exception("Vui lòng chọn dự án hợp lệ.");
            }

            if (!model.MaQuanLyDeXuat.HasValue || model.MaQuanLyDeXuat.Value <= 0)
            {
                throw new Exception("Vui lòng chọn quản lý đề xuất.");
            }

            var currentUserId = await GetCurrentUserIdAsync();
            await EnsureManagerCanCreateAsync(currentUserId);

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == model.MaDuAn && x.IsDeleted != true);

            if (duAn == null)
            {
                throw new Exception("Không tìm thấy dự án.");
            }

            if (duAn.MaNguoiDung != currentUserId)
            {
                throw new Exception("Chỉ quản lý hiện tại của dự án mới được tạo yêu cầu đổi quản lý.");
            }

            EnsureProjectAllowsManagerChangeRequest(duAn.TrangThaiDuAn);

            if (model.MaQuanLyDeXuat.Value == duAn.MaNguoiDung)
            {
                throw new Exception("Quản lý đề xuất mới không được trùng quản lý hiện tại.");
            }

            await EnsureNoPendingRequestAsync(duAn.MaDuAn);
            await EnsureManagerCandidateAsync(model.MaQuanLyDeXuat.Value);

            var now = DateTime.Now;
            var entity = new YeuCauDoiQuanLy
            {
                MaDuAn = duAn.MaDuAn,
                MaQuanLyHienTai = duAn.MaNguoiDung,
                MaQuanLyDeXuat = model.MaQuanLyDeXuat.Value,
                TrangThaiYeuCauDoiQuanLy = TrangThai.ChoDuyet,
                NgayTaoYeuCauDoiQuanLy = now,
                IsDeleted = false
            };

            _context.YeuCauDoiQuanLy.Add(entity);

            var lyDoPart = string.IsNullOrWhiteSpace(model.LyDoYeuCau)
                ? string.Empty
                : $". Lý do: {model.LyDoYeuCau.Trim()}";

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = duAn.MaDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Tạo yêu cầu đổi quản lý sang nhân sự #{model.MaQuanLyDeXuat.Value}{lyDoPart}",
                NkThoiGianQLDA = now
            });

            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(int id)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            await EnsureManagerCanCreateAsync(currentUserId);

            var entity = await _context.YeuCauDoiQuanLy
                .FirstOrDefaultAsync(x => x.MaYeuCauDoiQuanLy == id && x.IsDeleted != true);

            if (entity == null)
            {
                throw new Exception("Không tìm thấy yêu cầu đổi quản lý.");
            }

            if (entity.MaQuanLyHienTai != currentUserId)
            {
                throw new Exception("Bạn chỉ có thể hủy yêu cầu do chính bạn tạo.");
            }

            if (!TrangThai.EqualsValue(entity.TrangThaiYeuCauDoiQuanLy, TrangThai.ChoDuyet))
            {
                throw new Exception("Chỉ có thể hủy yêu cầu đang ở trạng thái chờ duyệt.");
            }

            entity.TrangThaiYeuCauDoiQuanLy = TrangThai.DaHuy;

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = entity.MaDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Hủy yêu cầu đổi quản lý #{entity.MaYeuCauDoiQuanLy}",
                NkThoiGianQLDA = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanCreateFromProjectAsync(int maDuAn)
        {
            if (maDuAn <= 0)
            {
                return false;
            }

            try
            {
                var currentUserId = await GetCurrentUserIdAsync();
                await EnsureManagerCanCreateAsync(currentUserId);
                var result = await CanCreateFromProjectCoreAsync(maDuAn, currentUserId);
                return result.CanCreate;
            }
            catch
            {
                return false;
            }
        }

        private async Task<(bool CanCreate, string? Reason)> CanCreateFromProjectCoreAsync(int maDuAn, int currentUserId)
        {
            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (duAn == null)
            {
                return (false, "Dự án không tồn tại hoặc đã bị xóa.");
            }

            if (duAn.MaNguoiDung != currentUserId)
            {
                return (false, "Bạn không phải quản lý hiện tại của dự án.");
            }

            if (!IsProjectStatusAllowedForRequest(duAn.TrangThaiDuAn))
            {
                return (false, "Dự án đang ở trạng thái không cho phép tạo yêu cầu đổi quản lý.");
            }

            var choDuyetStatuses = TrangThai.GetCommonStatusVariants(TrangThai.ChoDuyet);
            var hasPendingRequest = await _context.YeuCauDoiQuanLy.AnyAsync(x =>
                x.IsDeleted != true
                && x.MaDuAn == maDuAn
                && choDuyetStatuses.Contains(x.TrangThaiYeuCauDoiQuanLy ?? string.Empty));

            if (hasPendingRequest)
            {
                return (false, "Dự án đang có yêu cầu đổi quản lý chờ duyệt.");
            }

            return (true, null);
        }

        private async Task<List<YeuCauDoiQuanLyManagerOptionViewModel>> GetManagerOptionsAsync(int maQuanLyHienTai)
        {
            var items = await (
                from nd in _context.NguoiDung
                join cd in _context.ChucDanh on nd.MaChucDanh equals cd.MaChucDanh into chucDanhGroup
                from cd in chucDanhGroup.DefaultIfEmpty()
                where nd.IsDeleted != true && nd.MaNguoiDung != maQuanLyHienTai
                orderby nd.HoTenNguoiDung
                select new
                {
                    nd.MaNguoiDung,
                    TenNguoiDung = nd.HoTenNguoiDung ?? $"Nhân sự {nd.MaNguoiDung}",
                    TenChucDanh = cd != null && !string.IsNullOrWhiteSpace(cd.TenChucDanh)
                        ? cd.TenChucDanh
                        : "Chưa có chức danh"
                }
            ).ToListAsync();

            var options = new List<YeuCauDoiQuanLyManagerOptionViewModel>();
            foreach (var item in items)
            {
                if (await IsManagerCandidateAsync(item.MaNguoiDung, item.TenChucDanh))
                {
                    options.Add(new YeuCauDoiQuanLyManagerOptionViewModel
                    {
                        MaNguoiDung = item.MaNguoiDung,
                        TenNguoiDung = item.TenNguoiDung,
                        TenChucDanh = item.TenChucDanh
                    });
                }
            }

            return options;
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

            var isManager = await IsManagerCandidateAsync(nguoiDung.MaNguoiDung, nguoiDung.TenChucDanh);
            if (!isManager)
            {
                throw new Exception("Nhân sự đề xuất mới chưa có role/chức danh quản lý phù hợp.");
            }
        }

        private async Task<bool> IsManagerCandidateAsync(int maNguoiDung, string? tenChucDanh)
        {
            var aspUserId = await _context.Aspnetusers
                .Where(x => x.MaNguoiDung == maNguoiDung)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var hasManagerRole = false;
            if (!string.IsNullOrWhiteSpace(aspUserId))
            {
                hasManagerRole = await HasRoleAsync(aspUserId, "MANAGER");
            }

            var normalizedTitle = NormalizeKeyword(tenChucDanh);
            var hasManagerTitle = normalizedTitle.Contains("manager") || normalizedTitle.Contains("quanly");

            return hasManagerRole || hasManagerTitle;
        }

        private async Task EnsureNoPendingRequestAsync(int maDuAn)
        {
            var choDuyetStatuses = TrangThai.GetCommonStatusVariants(TrangThai.ChoDuyet);
            var hasPending = await _context.YeuCauDoiQuanLy.AnyAsync(x =>
                x.IsDeleted != true
                && x.MaDuAn == maDuAn
                && choDuyetStatuses.Contains(x.TrangThaiYeuCauDoiQuanLy ?? string.Empty));

            if (hasPending)
            {
                throw new Exception("Dự án đang có yêu cầu đổi quản lý chờ duyệt.");
            }
        }

        private static void EnsureProjectAllowsManagerChangeRequest(string? trangThaiDuAn)
        {
            if (!IsProjectStatusAllowedForRequest(trangThaiDuAn))
            {
                throw new Exception("Dự án đang ở trạng thái không cho phép tạo yêu cầu đổi quản lý.");
            }
        }

        private static bool IsProjectStatusAllowedForRequest(string? trangThaiDuAn)
        {
            if (TrangThai.LaHoanThanhCongViec(trangThaiDuAn)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.DaHuy)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.LuuTru))
            {
                return false;
            }

            return true;
        }

        private async Task EnsureManagerCanCreateAsync(int maNguoiDung)
        {
            var aspUserId = await _context.Aspnetusers
                .Where(x => x.MaNguoiDung == maNguoiDung)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(aspUserId))
            {
                throw new Exception("Không xác định được tài khoản đăng nhập của người dùng hiện tại.");
            }

            var hasManagerRole = await HasRoleAsync(aspUserId, "MANAGER");
            var hasAdminRole = await HasRoleAsync(aspUserId, "ADMIN");

            if (!hasManagerRole)
            {
                throw new Exception("Chỉ Manager mới được tạo yêu cầu đổi quản lý.");
            }

            if (hasAdminRole)
            {
                throw new Exception("Tài khoản Admin không được tạo yêu cầu đổi quản lý.");
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

        private async Task<int> GetCurrentUserIdAsync()
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

            return maNguoiDung;
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
