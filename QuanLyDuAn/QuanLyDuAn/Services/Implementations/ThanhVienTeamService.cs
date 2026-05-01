using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.ThanhVienTeam;

namespace QuanLyDuAn.Services.Implementations
{
    public class ThanhVienTeamService : IThanhVienTeamService
    {
        private readonly QuanLyDuAnDbContext _context;

        public ThanhVienTeamService(QuanLyDuAnDbContext context)
        {
            _context = context;
        }

        public async Task<List<ThanhVienTeamViewModel>> GetAllAsync(string? tuKhoa, int? maTeam, bool? isLeader)
        {
            var query = from tv in _context.NhanVienTeam
                        join team in _context.Team on tv.MaTeam equals team.MaTeam
                        join ns in _context.NguoiDung on tv.MaNguoiDung equals ns.MaNguoiDung
                        where team.IsDeleted != true && ns.IsDeleted != true
                        orderby tv.MaTeam descending, ns.HoTenNguoiDung
                        select new ThanhVienTeamViewModel
                        {
                            MaTeam = tv.MaTeam,
                            TenTeam = team.TenTeam ?? string.Empty,
                            MaNguoiDung = tv.MaNguoiDung,
                            HoTenNguoiDung = ns.HoTenNguoiDung ?? string.Empty,
                            VaiTroTrongTeam = tv.VaiTroTrongTeam ?? string.Empty,
                            NgayThamGiaTeam = tv.NgayThamGiaTeam,
                            IsLeader = tv.IsLeader ?? false,
                            TeamDaCoTruongNhom = _context.NhanVienTeam.Any(x => x.MaTeam == tv.MaTeam && x.IsLeader == true)
                        };

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    x.TenTeam.ToLower().Contains(keyword) ||
                    x.HoTenNguoiDung.ToLower().Contains(keyword) ||
                    x.VaiTroTrongTeam.ToLower().Contains(keyword));
            }

            if (maTeam.HasValue && maTeam.Value > 0)
            {
                query = query.Where(x => x.MaTeam == maTeam.Value);
            }

            if (isLeader.HasValue)
            {
                query = query.Where(x => x.IsLeader == isLeader.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<ThanhVienTeamCreateUpdateViewModel?> GetByIdAsync(int maTeam, int maNguoiDung)
        {
            return await _context.NhanVienTeam
                .Where(x => x.MaTeam == maTeam && x.MaNguoiDung == maNguoiDung)
                .Select(x => new ThanhVienTeamCreateUpdateViewModel
                {
                    MaTeam = x.MaTeam,
                    MaNguoiDung = x.MaNguoiDung,
                    VaiTroTrongTeam = x.VaiTroTrongTeam ?? string.Empty,
                    IsLeader = x.IsLeader ?? false
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<TeamOptionViewModel>> GetTeamOptionsAsync()
        {
            return await _context.Team
                .Where(x => x.IsDeleted != true)
                .OrderBy(x => x.TenTeam)
                .Select(x => new TeamOptionViewModel
                {
                    MaTeam = x.MaTeam,
                    TenTeam = x.TenTeam ?? $"Team {x.MaTeam}"
                })
                .ToListAsync();
        }

        public async Task<List<NhanSuOptionViewModel>> GetNhanSuOptionsAsync()
        {
            var employeeRoleIds = _context.Aspnetroles
                .Where(x => x.NormalizedName == "EMPLOYEE")
                .Select(x => x.Id);

            var blockedRoleIds = _context.Aspnetroles
                .Where(x => x.NormalizedName == "ADMIN" || x.NormalizedName == "MANAGER")
                .Select(x => x.Id);

            var employeeUserIds = _context.Aspnetuserroles
                .Where(x => employeeRoleIds.Contains(x.Id))
                .Select(x => x.Asp_Id);

            var blockedUserIds = _context.Aspnetuserroles
                .Where(x => blockedRoleIds.Contains(x.Id))
                .Select(x => x.Asp_Id);

            return await _context.NguoiDung
                .Where(x => x.IsDeleted != true && employeeUserIds.Contains(x.Id) && !blockedUserIds.Contains(x.Id))
                .OrderBy(x => x.HoTenNguoiDung)
                .Select(x => new NhanSuOptionViewModel
                {
                    MaNguoiDung = x.MaNguoiDung,
                    HoTenNguoiDung = x.HoTenNguoiDung ?? $"Nhân sự {x.MaNguoiDung}",
                    SoTeamDaThamGia = _context.NhanVienTeam.Count(nvt => nvt.MaNguoiDung == x.MaNguoiDung)
                })
                .ToListAsync();
        }

        public async Task SaveAsync(ThanhVienTeamCreateUpdateViewModel model)
        {
            if (!model.MaTeam.HasValue)
                throw new Exception("Vui lòng chọn team.");

            if (!model.MaNguoiDung.HasValue)
                throw new Exception("Vui lòng chọn nhân sự.");

            var maTeam = model.MaTeam.Value;
            var maNguoiDung = model.MaNguoiDung.Value;
            var vaiTro = (model.VaiTroTrongTeam ?? string.Empty).Trim();

            var teamExists = await _context.Team
                .AnyAsync(x => x.MaTeam == maTeam && x.IsDeleted != true);
            if (!teamExists)
                throw new Exception("Team không tồn tại.");

            var nhanSuExists = await _context.NguoiDung
                .AnyAsync(x => x.MaNguoiDung == maNguoiDung && x.IsDeleted != true);
            if (!nhanSuExists)
                throw new Exception("Nhân sự không tồn tại.");

            var laNhanVienEmployee = await IsEligibleEmployeeAsync(maNguoiDung);
            if (!laNhanVienEmployee)
                throw new Exception("Chỉ nhân sự có vai trò Employee mới được thêm vào team.");

            var entity = await _context.NhanVienTeam
                .FirstOrDefaultAsync(x => x.MaTeam == maTeam && x.MaNguoiDung == maNguoiDung);

            var leaderHienTai = await _context.NhanVienTeam
                .Where(x => x.MaTeam == maTeam && x.IsLeader == true)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync();

            var teamDangPhuTrachDuAn = await _context.TeamDuAn.AnyAsync(x => x.MaTeam == maTeam);

            if (teamDangPhuTrachDuAn && leaderHienTai > 0)
            {
                var dangChuyenLeader =
                    (maNguoiDung == leaderHienTai && !model.IsLeader) ||
                    (maNguoiDung != leaderHienTai && model.IsLeader);

                if (dangChuyenLeader)
                {
                    throw new Exception("Team đang phụ trách dự án nên không thể chuyển đổi vai trò trưởng nhóm sang thành viên khác.");
                }
            }

            if (entity == null)
            {
                var taiKhoanDangBiKhoa = await _context.NguoiDung
                    .Where(x => x.MaNguoiDung == maNguoiDung && x.IsDeleted != true)
                    .Join(
                        _context.Aspnetusers,
                        nd => nd.Id,
                        tk => tk.Id,
                        (nd, tk) => tk.LockoutEnd)
                    .AnyAsync(lockoutEnd => lockoutEnd.HasValue && lockoutEnd.Value > DateTime.UtcNow);

                if (taiKhoanDangBiKhoa)
                    throw new Exception("Không thể thêm nhân sự bị khóa vào team.");
            }

            if (model.IsLeader && (entity == null || entity.IsLeader != true))
            {
                var daCoTruongNhomKhac = await _context.NhanVienTeam
                    .AnyAsync(x => x.MaTeam == maTeam && x.IsLeader == true && x.MaNguoiDung != maNguoiDung);

                if (daCoTruongNhomKhac)
                    throw new Exception("Mỗi team chỉ được có tối đa 1 trưởng nhóm.");
            }

            if (entity == null)
            {
                entity = new NhanVienTeam
                {
                    MaTeam = maTeam,
                    MaNguoiDung = maNguoiDung,
                    VaiTroTrongTeam = vaiTro,
                    NgayThamGiaTeam = DateTime.Now,
                    IsLeader = model.IsLeader
                };

                _context.NhanVienTeam.Add(entity);
            }
            else
            {
                entity.VaiTroTrongTeam = vaiTro;
                entity.IsLeader = model.IsLeader;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SetLeaderAsync(int maTeam, int maNguoiDung)
        {
            await KiemTraTaiKhoanBiKhoaAsync(maNguoiDung);

            var leaderHienTai = await _context.NhanVienTeam
                .Where(x => x.MaTeam == maTeam && x.IsLeader == true)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync();

            var teamDangPhuTrachDuAn = await _context.TeamDuAn.AnyAsync(x => x.MaTeam == maTeam);

            if (teamDangPhuTrachDuAn && leaderHienTai > 0 && leaderHienTai != maNguoiDung)
                throw new Exception("Team đang phụ trách dự án nên không thể chuyển đổi vai trò trưởng nhóm sang thành viên khác.");

            var target = await _context.NhanVienTeam
                .FirstOrDefaultAsync(x => x.MaTeam == maTeam && x.MaNguoiDung == maNguoiDung);

            if (target == null)
                throw new Exception("Không tìm thấy thành viên team để gán trưởng nhóm.");

            var danhSachTeam = await _context.NhanVienTeam
                .Where(x => x.MaTeam == maTeam)
                .ToListAsync();

            foreach (var item in danhSachTeam)
            {
                item.IsLeader = item.MaNguoiDung == maNguoiDung;
            }

            await _context.SaveChangesAsync();
        }

        private async Task KiemTraTaiKhoanBiKhoaAsync(int maNguoiDung)
        {
            var now = DateTime.UtcNow;

            var nhanSu = await (
                from nd in _context.NguoiDung
                join tk in _context.Aspnetusers on nd.Id equals tk.Id into tkJoin
                from tk in tkJoin.DefaultIfEmpty()
                where nd.MaNguoiDung == maNguoiDung
                select new
                {
                    nd.MaNguoiDung,
                    nd.IsDeleted,
                    LockoutEnd = tk != null ? tk.LockoutEnd : null
                }
            ).FirstOrDefaultAsync();

            if (nhanSu == null)
            {
                throw new Exception("Không thể gán trưởng nhóm: nhân sự không tồn tại.");
            }

            if (nhanSu.IsDeleted == true)
            {
                throw new Exception("Không thể gán trưởng nhóm: nhân sự đã bị xóa.");
            }

            if (nhanSu.LockoutEnd.HasValue && nhanSu.LockoutEnd.Value > now)
            {
                throw new Exception("Không thể gán trưởng nhóm cho tài khoản đang bị khóa.");
            }
        }

        public async Task DeleteAsync(int maTeam, int maNguoiDung)
        {
            var entity = await _context.NhanVienTeam
                .FirstOrDefaultAsync(x => x.MaTeam == maTeam && x.MaNguoiDung == maNguoiDung);

            if (entity == null)
                throw new Exception("Không tìm thấy thành viên team.");

            var teamDangPhuTrachDuAn = await _context.TeamDuAn.AnyAsync(x => x.MaTeam == maTeam);
            if (teamDangPhuTrachDuAn && entity.IsLeader == true)
                throw new Exception("Team đang phụ trách dự án nên không thể xóa trưởng nhóm khỏi team.");

            _context.NhanVienTeam.Remove(entity);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> IsEligibleEmployeeAsync(int maNguoiDung)
        {
            var userId = await _context.NguoiDung
                .Where(x => x.MaNguoiDung == maNguoiDung && x.IsDeleted != true)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(userId))
                return false;

            var roles = await (from ur in _context.Aspnetuserroles
                               join role in _context.Aspnetroles on ur.Id equals role.Id
                               where ur.Asp_Id == userId
                               select role.NormalizedName)
                               .ToListAsync();

            var laEmployee = roles.Contains("EMPLOYEE");
            var laRoleKhongHopLe = roles.Contains("ADMIN") || roles.Contains("MANAGER");

            return laEmployee && !laRoleKhongHopLe;
        }
    }
}
