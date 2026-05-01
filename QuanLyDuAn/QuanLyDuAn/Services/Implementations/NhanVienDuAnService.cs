using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.NhanVienDuAn;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class NhanVienDuAnService : INhanVienDuAnService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NhanVienDuAnService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<NhanVienDuAnPageViewModel> GetPageAsync(
            int maDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn)
        {
            var duAn = await GetProjectAsync(maDuAn);
            var (coTheChinhSua, lyDoKhongTheChinhSua) = EvaluateEditableCondition(duAn);

            var assignedTeamIds = await _context.TeamDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.MaTeam)
                .ToListAsync();

            var vm = new NhanVienDuAnPageViewModel
            {
                MaDuAn = duAn.MaDuAn,
                TenDuAn = duAn.TenDuAn ?? string.Empty,
                TrangThaiDuAn = duAn.TrangThaiDuAn ?? string.Empty,
                CoTheChinhSua = coTheChinhSua,
                LyDoKhongTheChinhSua = lyDoKhongTheChinhSua,
                TuKhoa = tuKhoa,
                LocMaLoaiDuAn = locMaLoaiDuAn,
                LocTrangThaiDuAn = locTrangThaiDuAn
            };

            vm.DanhSachNhanVienPhuTrach = await (
                from nvda in _context.NhanVienDuAn
                join nd in _context.NguoiDung on nvda.MaNguoiDung equals nd.MaNguoiDung
                where nvda.MaDuAn == maDuAn && nd.IsDeleted != true
                orderby nd.HoTenNguoiDung
                select new NhanVienDuAnItemViewModel
                {
                    MaNguoiDung = nvda.MaNguoiDung,
                    HoTenNguoiDung = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}",
                    VaiTroTrongDuAn = nvda.VaiTroTrongDuAn ?? "Thành viên dự án",
                    NgayThamGiaDuAn = nvda.NgayThamGiaDuAn,
                    ThuocTeamPhuTrach = _context.NhanVienTeam.Any(nvt =>
                        assignedTeamIds.Contains(nvt.MaTeam) && nvt.MaNguoiDung == nvda.MaNguoiDung),
                    TenTeamPhuTrach = string.Join(
                        ", ",
                        (from nvt in _context.NhanVienTeam
                         join team in _context.Team on nvt.MaTeam equals team.MaTeam
                         where assignedTeamIds.Contains(nvt.MaTeam) && nvt.MaNguoiDung == nvda.MaNguoiDung
                         select team.TenTeam ?? $"Team {team.MaTeam}").Distinct())
                }).ToListAsync();

            var assignedIds = vm.DanhSachNhanVienPhuTrach
                .Select(x => x.MaNguoiDung)
                .ToList();

            var employeeRoleIds = _context.Aspnetroles
                .Where(x => x.NormalizedName == "EMPLOYEE")
                .Select(x => x.Id);

            var blockedRoleIds = _context.Aspnetroles
                .Where(x => x.NormalizedName == "ADMIN" || x.NormalizedName == "MANAGER")
                .Select(x => x.Id);

            var employeeAspIds = _context.Aspnetuserroles
                .Where(x => employeeRoleIds.Contains(x.Id))
                .Select(x => x.Asp_Id);

            var blockedAspIds = _context.Aspnetuserroles
                .Where(x => blockedRoleIds.Contains(x.Id))
                .Select(x => x.Asp_Id);

            var danhSachUngVien = await (
                from nd in _context.NguoiDung
                join tk in _context.Aspnetusers on nd.Id equals tk.Id
                where nd.IsDeleted != true
                      && employeeAspIds.Contains(nd.Id)
                      && !blockedAspIds.Contains(nd.Id)
                      && !assignedIds.Contains(nd.MaNguoiDung)
                      && (!tk.LockoutEnd.HasValue || tk.LockoutEnd <= DateTime.UtcNow)
                orderby nd.HoTenNguoiDung
                select new
                {
                    MaNguoiDung = nd.MaNguoiDung,
                    HoTenNguoiDung = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}"
                }).ToListAsync();

            var ungVienIds = danhSachUngVien
                .Select(x => x.MaNguoiDung)
                .ToList();

            var danhSachTeamNhanVien = await (
                from nvt in _context.NhanVienTeam
                join team in _context.Team on nvt.MaTeam equals team.MaTeam
                where ungVienIds.Contains(nvt.MaNguoiDung) && team.IsDeleted != true
                select new
                {
                    nvt.MaNguoiDung,
                    TenTeam = team.TenTeam ?? $"Team {team.MaTeam}"
                }).ToListAsync();

            var teamByNhanVien = danhSachTeamNhanVien
                .GroupBy(x => x.MaNguoiDung)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(", ", g.Select(x => x.TenTeam).Distinct().OrderBy(x => x)));

            vm.DanhSachNhanVienCoTheThem = danhSachUngVien
                .Select(x => new NhanVienDuAnOptionViewModel
                {
                    MaNguoiDung = x.MaNguoiDung,
                    HoTenNguoiDung = x.HoTenNguoiDung,
                    TatCaTenTeamDangThamGia = teamByNhanVien.TryGetValue(x.MaNguoiDung, out var teamNames)
                        ? teamNames
                        : "Chưa thuộc team"
                })
                .ToList();

            return vm;
        }

        public async Task AddAsync(int maDuAn, List<int> maNguoiDungDuocChon)
        {
            var selectedIds = (maNguoiDungDuocChon ?? new List<int>())
                .Distinct()
                .ToList();

            if (selectedIds.Count == 0)
                throw new Exception("Vui lòng chọn ít nhất 1 nhân viên để thêm vào dự án.");

            var duAn = await GetProjectAsync(maDuAn);
            await EnsureCanManageProjectAsync(duAn);

            var eligibleIds = await GetEligibleEmployeeIdsAsync(selectedIds);
            if (eligibleIds.Count != selectedIds.Count)
                throw new Exception("Danh sách nhân viên được chọn có dữ liệu không hợp lệ hoặc tài khoản đang bị khóa.");

            var existedIds = await _context.NhanVienDuAn
                .Where(x => x.MaDuAn == maDuAn && selectedIds.Contains(x.MaNguoiDung))
                .Select(x => x.MaNguoiDung)
                .ToListAsync();

            var addIds = selectedIds.Except(existedIds).ToList();

            if (addIds.Count == 0)
                throw new Exception("Các nhân viên đã được gán cho dự án.");

            foreach (var id in addIds)
            {
                _context.NhanVienDuAn.Add(new NhanVienDuAn
                {
                    MaDuAn = maDuAn,
                    MaNguoiDung = id,
                    NgayThamGiaDuAn = DateTime.Now,
                    VaiTroTrongDuAn = "Thành viên dự án"
                });

                _context.NhatKyPhuTrachDuAn.Add(new NhatKyPhuTrachDuAn
                {
                    MaDuAn = maDuAn,
                    MaNguoiDung = id,
                    NkHanhDongPTDA = "Thêm nhân viên phụ trách dự án",
                    NkThoiGianPTDA = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoleAsync(int maDuAn, int maNguoiDung, string vaiTroTrongDuAn)
        {
            var duAn = await GetProjectAsync(maDuAn);
            await EnsureCanManageProjectAsync(duAn);

            var vaiTro = (vaiTroTrongDuAn ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(vaiTro))
                throw new Exception("Vai trò trong dự án không được để trống.");

            if (vaiTro.Length > 50)
                throw new Exception("Vai trò trong dự án tối đa 50 ký tự.");

            var entity = await _context.NhanVienDuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.MaNguoiDung == maNguoiDung);

            if (entity == null)
                throw new Exception("Không tìm thấy nhân viên phụ trách trong dự án.");

            entity.VaiTroTrongDuAn = vaiTro;

            _context.NhatKyPhuTrachDuAn.Add(new NhatKyPhuTrachDuAn
            {
                MaDuAn = maDuAn,
                MaNguoiDung = maNguoiDung,
                NkHanhDongPTDA = $"Cập nhật vai trò phụ trách: {vaiTro}",
                NkThoiGianPTDA = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int maDuAn, int maNguoiDung)
        {
            var duAn = await GetProjectAsync(maDuAn);
            await EnsureCanManageProjectAsync(duAn);

            var entity = await _context.NhanVienDuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.MaNguoiDung == maNguoiDung);

            if (entity == null)
                throw new Exception("Không tìm thấy nhân viên phụ trách trong dự án.");

            var assignedTeamIds = await _context.TeamDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.MaTeam)
                .ToListAsync();

            var isFromAssignedTeam = await _context.NhanVienTeam
                .AnyAsync(x => assignedTeamIds.Contains(x.MaTeam) && x.MaNguoiDung == maNguoiDung);

            if (isFromAssignedTeam)
                throw new Exception("Nhân viên đang thuộc team phụ trách dự án. Vui lòng bỏ khỏi team phụ trách trước khi xóa trực tiếp.");

            _context.NhanVienDuAn.Remove(entity);

            _context.NhatKyPhuTrachDuAn.Add(new NhatKyPhuTrachDuAn
            {
                MaDuAn = maDuAn,
                MaNguoiDung = maNguoiDung,
                NkHanhDongPTDA = "Xóa nhân viên phụ trách khỏi dự án",
                NkThoiGianPTDA = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        private async Task<DuAn> GetProjectAsync(int maDuAn)
        {
            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (duAn == null)
                throw new Exception("Không tìm thấy dự án.");

            return duAn;
        }

        private (bool CoTheChinhSua, string? LyDo) EvaluateEditableCondition(DuAn duAn)
        {
            if (duAn.MaLoaiDuAn <= 0 ||
                !duAn.NgayBatDauDuAn.HasValue ||
                !duAn.NgayKetThucDuAn.HasValue ||
                duAn.NgayKetThucDuAn < duAn.NgayBatDauDuAn)
            {
                return (false, "Dự án chưa đủ thông tin cơ bản, không thể cập nhật team/nhân viên phụ trách.");
            }

            var status = duAn.TrangThaiDuAn;
            if (TrangThai.EqualsValue(status, TrangThai.ChoXacNhanHoanThanh) ||
                TrangThai.EqualsValue(status, TrangThai.HoanThanh) ||
                TrangThai.EqualsValue(status, TrangThai.LuuTru))
            {
                return (false, "Trạng thái dự án hiện tại không cho phép thêm/sửa/xóa team hoặc nhân viên phụ trách.");
            }

            return (true, null);
        }

        private async Task EnsureCanManageProjectAsync(DuAn duAn)
        {
            var (coTheChinhSua, lyDoKhongTheChinhSua) = EvaluateEditableCondition(duAn);
            if (!coTheChinhSua)
                throw new Exception(lyDoKhongTheChinhSua ?? "Không thể thao tác dự án.");

            var currentUserId = await GetCurrentUserIdAsync();
            if (duAn.MaNguoiDung != currentUserId)
                throw new Exception("Bạn không có quyền thao tác dự án này.");
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

        private async Task<List<int>> GetEligibleEmployeeIdsAsync(List<int> ids)
        {
            var employeeRoleIds = _context.Aspnetroles
                .Where(x => x.NormalizedName == "EMPLOYEE")
                .Select(x => x.Id);

            var blockedRoleIds = _context.Aspnetroles
                .Where(x => x.NormalizedName == "ADMIN" || x.NormalizedName == "MANAGER")
                .Select(x => x.Id);

            var employeeAspIds = _context.Aspnetuserroles
                .Where(x => employeeRoleIds.Contains(x.Id))
                .Select(x => x.Asp_Id);

            var blockedAspIds = _context.Aspnetuserroles
                .Where(x => blockedRoleIds.Contains(x.Id))
                .Select(x => x.Asp_Id);

            return await (
                from nd in _context.NguoiDung
                join tk in _context.Aspnetusers on nd.Id equals tk.Id
                where ids.Contains(nd.MaNguoiDung)
                      && nd.IsDeleted != true
                      && employeeAspIds.Contains(nd.Id)
                      && !blockedAspIds.Contains(nd.Id)
                      && (!tk.LockoutEnd.HasValue || tk.LockoutEnd <= DateTime.UtcNow)
                select nd.MaNguoiDung
            ).ToListAsync();
        }
    }
}
