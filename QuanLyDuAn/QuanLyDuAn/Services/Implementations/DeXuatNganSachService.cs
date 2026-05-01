using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DeXuatNganSach;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class DeXuatNganSachService : IDeXuatNganSachService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeXuatNganSachService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DeXuatNganSachPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai)
        {
            if (!locMaDuAn.HasValue)
                throw new Exception("Vui lòng chọn dự án.");

            var currentUserId = await GetCurrentUserIdAsync();
            var projectOptions = await GetEligibleProjectOptionsAsync(currentUserId);
            var selectedProject = projectOptions.FirstOrDefault(x => x.MaDuAn == locMaDuAn.Value);

            if (selectedProject == null)
                throw new Exception("Bạn không có quyền truy cập dự án để đề xuất ngân sách.");

            var query =
                from dx in _context.DeXuatNganSach
                join da in _context.DuAn on dx.MaDuAn equals da.MaDuAn
                join ndDeXuat in _context.NguoiDung on dx.MaNguoiDungDeXuat equals ndDeXuat.MaNguoiDung
                join ndDuyet in _context.NguoiDung on dx.MaNguoiDungDuyet equals ndDuyet.MaNguoiDung into ndDuyetLeft
                from ndDuyet in ndDuyetLeft.DefaultIfEmpty()
                where dx.IsDeleted != true && da.IsDeleted != true
                select new DeXuatNganSachItemViewModel
                {
                    MaDeXuatNS = dx.MaDeXuatNS,
                    MaDuAn = dx.MaDuAn,
                    TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}",
                    MaNganSachCu = dx.MaNganSachCu,
                    NganSachCu = dx.NganSachCu,
                    NganSachDeXuat = dx.NganSachDeXuat,
                    LyDoDeXuat = dx.LyDoDeXuat ?? string.Empty,
                    MaNguoiDungDeXuat = dx.MaNguoiDungDeXuat,
                    NguoiDungDeXuat = ndDeXuat.HoTenNguoiDung ?? $"Nhân viên {dx.MaNguoiDungDeXuat}",
                    MaNguoiDungDuyet = dx.MaNguoiDungDuyet,
                    NguoiDungDuyet = ndDuyet != null ? (ndDuyet.HoTenNguoiDung ?? $"Nhân viên {ndDuyet.MaNguoiDung}") : null,
                    NgayDeXuat = dx.NgayDeXuat,
                    NgayDuyet = dx.NgayDuyet,
                    TrangThaiDeXuat = dx.TrangThaiDeXuat ?? string.Empty
                };

            query = query.Where(x => x.MaDuAn == locMaDuAn.Value);

            if (!string.IsNullOrWhiteSpace(locTrangThai))
            {
                var filterValues = TrangThai.GetCommonStatusVariants(locTrangThai);
                if (filterValues.Length > 0)
                {
                    query = query.Where(x => filterValues.Contains(x.TrangThaiDeXuat));
                }
            }

            return new DeXuatNganSachPageViewModel
            {
                MaDuAn = selectedProject.MaDuAn,
                TenDuAn = selectedProject.TenDuAn,
                DanhSach = await query
                    .OrderByDescending(x => x.NgayDeXuat)
                    .ThenByDescending(x => x.MaDeXuatNS)
                    .ToListAsync(),
                DanhSachDuAn = projectOptions,
                Form = new DeXuatNganSachCreateViewModel
                {
                    MaDuAn = selectedProject.MaDuAn
                },
                LocMaDuAn = locMaDuAn,
                LocTrangThai = locTrangThai
            };
        }

        public async Task CreateAsync(DeXuatNganSachCreateViewModel model)
        {
            if (!model.MaDuAn.HasValue || model.MaDuAn.Value <= 0)
                throw new Exception("Vui lòng chọn dự án hợp lệ.");

            if (!model.NganSachDeXuat.HasValue || model.NganSachDeXuat.Value <= 0)
                throw new Exception("Ngân sách đề xuất phải lớn hơn 0.");

            var maDuAn = model.MaDuAn.Value;
            var currentUserId = await GetCurrentUserIdAsync();
            await EnsureCanProposeForProjectAsync(currentUserId, maDuAn);

            var duAn = await _context.DuAn.FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);
            if (duAn == null)
                throw new Exception("Không tìm thấy dự án.");

            var pendingExists = await _context.DeXuatNganSach.AnyAsync(x =>
                x.IsDeleted != true &&
                x.MaDuAn == maDuAn &&
                (x.TrangThaiDeXuat == TrangThai.ChoDuyet || x.TrangThaiDeXuat == TrangThai.ChoDuyetHienThi));

            if (pendingExists)
                throw new Exception("Dự án đang có đề xuất ngân sách chờ duyệt.");

            var nganSachHienTai = await _context.NganSach
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true && x.IsActive == true)
                .OrderByDescending(x => x.Version)
                .ThenByDescending(x => x.NgayCapNhatNganSach)
                .FirstOrDefaultAsync();

            var entity = new DeXuatNganSach
            {
                MaDuAn = maDuAn,
                MaNganSachCu = nganSachHienTai?.MaNganSach,
                NganSachCu = nganSachHienTai?.SoTienNganSach,
                NganSachDeXuat = model.NganSachDeXuat,
                LyDoDeXuat = model.LyDoDeXuat.Trim(),
                MaNguoiDungDeXuat = currentUserId,
                NgayDeXuat = DateTime.Now,
                TrangThaiDeXuat = TrangThai.ChoDuyet,
                IsDeleted = false
            };

            _context.DeXuatNganSach.Add(entity);

            if (nganSachHienTai != null)
            {
                _context.NhatKyNganSach.Add(new NhatKyNganSach
                {
                    MaNganSach = nganSachHienTai.MaNganSach,
                    MaDuAn = maDuAn,
                    SoTienNKNS = entity.NganSachDeXuat,
                    NganSachTruoc = entity.NganSachCu,
                    NganSachSau = entity.NganSachDeXuat,
                    NkNgayCapNhatNS = DateTime.Now,
                    NkTrangThaiNganSach = nganSachHienTai.TrangThaiNganSach,
                    HanhDongNKNS = "Tạo đề xuất ngân sách mới",
                    ThoiGianNKNS = DateTime.Now
                });
            }

            var hanhDongNganSach = nganSachHienTai == null
                ? $"Tạo đề xuất ngân sách lần đầu: {entity.NganSachDeXuat:N0}"
                : $"Tạo đề xuất ngân sách từ {entity.NganSachCu:N0} lên {entity.NganSachDeXuat:N0}";

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = maDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = hanhDongNganSach,
                NkThoiGianQLDA = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(int maDeXuatNs)
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var entity = await _context.DeXuatNganSach
                .FirstOrDefaultAsync(x => x.MaDeXuatNS == maDeXuatNs && x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy đề xuất ngân sách.");

            await EnsureCanProposeForProjectAsync(currentUserId, entity.MaDuAn);

            if (entity.MaNguoiDungDeXuat != currentUserId)
                throw new Exception("Bạn chỉ có thể hủy đề xuất do chính bạn tạo.");

            if (!IsPending(entity.TrangThaiDeXuat))
                throw new Exception("Chỉ có thể hủy đề xuất đang ở trạng thái chờ duyệt.");

            entity.TrangThaiDeXuat = TrangThai.DaHuy;

            if (entity.MaNganSachCu.HasValue)
            {
                _context.NhatKyNganSach.Add(new NhatKyNganSach
                {
                    MaNganSach = entity.MaNganSachCu.Value,
                    MaDuAn = entity.MaDuAn,
                    SoTienNKNS = entity.NganSachDeXuat,
                    NganSachTruoc = entity.NganSachCu,
                    NganSachSau = entity.NganSachCu,
                    NkNgayCapNhatNS = DateTime.Now,
                    NkTrangThaiNganSach = TrangThai.GiuNguyen,
                    HanhDongNKNS = $"Hủy đề xuất ngân sách #{entity.MaDeXuatNS}",
                    ThoiGianNKNS = DateTime.Now
                });
            }

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = entity.MaDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Hủy đề xuất ngân sách #{entity.MaDeXuatNS}",
                NkThoiGianQLDA = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(aspUserId))
                throw new Exception("Không xác định được người dùng hiện tại.");

            var maNguoiDung = await _context.Aspnetusers
                .Where(x => x.Id == aspUserId)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync();

            if (maNguoiDung <= 0)
                throw new Exception("Không xác định được nhân sự tương ứng của người dùng hiện tại.");

            return maNguoiDung;
        }

        private async Task EnsureCanProposeForProjectAsync(int maNguoiDung, int maDuAn)
        {
            var coTeamPhuTrach = await _context.TeamDuAn.AnyAsync(x => x.MaDuAn == maDuAn);

            if (coTeamPhuTrach)
            {
                var isLeader = await (
                    from nvt in _context.NhanVienTeam
                    join td in _context.TeamDuAn on nvt.MaTeam equals td.MaTeam
                    where nvt.MaNguoiDung == maNguoiDung
                          && nvt.IsLeader == true
                          && td.MaDuAn == maDuAn
                    select nvt.MaTeam
                ).AnyAsync();

                if (!isLeader)
                    throw new Exception("Dự án đã có team phụ trách, chỉ trưởng team (isleader = true) mới được đề xuất.");

                return;
            }

            var laNhanVienDuAn = await _context.NhanVienDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn && x.MaNguoiDung == maNguoiDung);

            if (!laNhanVienDuAn)
                throw new Exception("Dự án chưa có team phụ trách, chỉ nhân viên thuộc dự án mới được đề xuất.");
        }

        private async Task<List<DeXuatNganSachDuAnOptionViewModel>> GetEligibleProjectOptionsAsync(int maNguoiDung)
        {
            var leaderProjectIds = await (
                from td in _context.TeamDuAn
                join nvt in _context.NhanVienTeam on td.MaTeam equals nvt.MaTeam
                where nvt.MaNguoiDung == maNguoiDung
                      && nvt.IsLeader == true
                select td.MaDuAn
            )
            .Distinct()
            .ToListAsync();

            var memberProjectIdsWithoutTeam = await (
                from nvda in _context.NhanVienDuAn
                where nvda.MaNguoiDung == maNguoiDung
                      && !_context.TeamDuAn.Any(td => td.MaDuAn == nvda.MaDuAn)
                select nvda.MaDuAn
            )
            .Distinct()
            .ToListAsync();

            var allowedProjectIds = leaderProjectIds
                .Concat(memberProjectIdsWithoutTeam)
                .Distinct()
                .ToList();

            var nganSachMoiNhatTheoDuAn = await _context.NganSach
                .Where(x => allowedProjectIds.Contains(x.MaDuAn) && x.IsDeleted != true && x.IsActive == true)
                .GroupBy(x => x.MaDuAn)
                .Select(g => g
                    .OrderByDescending(x => x.Version)
                    .ThenByDescending(x => x.NgayCapNhatNganSach)
                    .First())
                .ToListAsync();

            var nganSachTheoDuAn = nganSachMoiNhatTheoDuAn.ToDictionary(x => x.MaDuAn, x => x.SoTienNganSach);

            var danhSachDuAn = await _context.DuAn
                .Where(x => x.IsDeleted != true && allowedProjectIds.Contains(x.MaDuAn))
                .OrderBy(x => x.TenDuAn)
                .Select(x => new
                {
                    MaDuAn = x.MaDuAn,
                    TenDuAn = x.TenDuAn ?? $"Dự án {x.MaDuAn}"
                })
                .ToListAsync();

            return danhSachDuAn
                .Select(x => new DeXuatNganSachDuAnOptionViewModel
                {
                    MaDuAn = x.MaDuAn,
                    TenDuAn = x.TenDuAn,
                    NganSachHienTai = nganSachTheoDuAn.TryGetValue(x.MaDuAn, out var nganSach)
                        ? nganSach
                        : null
                })
                .ToList();
        }

        private static bool IsPending(string? status)
        {
            return TrangThai.EqualsValue(status, TrangThai.ChoDuyet);
        }
    }
}
