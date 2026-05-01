using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Team;

namespace QuanLyDuAn.Services.Implementations
{
    public class TeamService : ITeamService
    {
        private readonly QuanLyDuAnDbContext _context;

        public TeamService(QuanLyDuAnDbContext context)
        {
            _context = context;
        }

        public async Task<List<TeamViewModel>> GetAllAsync(string? tuKhoa, string? trangThaiTeam)
        {
            var query = _context.Team
                .Where(x => x.IsDeleted != true)
                .Select(x => new TeamViewModel
                {
                    MaTeam = x.MaTeam,
                    TenTeam = x.TenTeam ?? string.Empty,
                    MoTaTeam = x.MoTaTeam,
                    NgayLapTeam = x.NgayLapTeam,
                    TrangThaiTeam = x.TrangThaiTeam ?? string.Empty,
                    SoLuongThanhVien = _context.NhanVienTeam.Count(nvt => nvt.MaTeam == x.MaTeam),
                    CoTruongNhom = _context.NhanVienTeam.Any(nvt => nvt.MaTeam == x.MaTeam && nvt.IsLeader == true)
                })
                .OrderByDescending(x => x.MaTeam)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    x.TenTeam.ToLower().Contains(keyword) ||
                    (x.MoTaTeam != null && x.MoTaTeam.ToLower().Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(trangThaiTeam))
            {
                var filterValues = TrangThai.GetCommonStatusVariants(trangThaiTeam);
                if (filterValues.Length > 0)
                {
                    query = query.Where(x => filterValues.Contains(x.TrangThaiTeam));
                }
            }

            return await query.ToListAsync();
        }

        public async Task<TeamCreateUpdateViewModel?> GetByIdAsync(int id)
        {
            return await _context.Team
                .Where(x => x.MaTeam == id && x.IsDeleted != true)
                .Select(x => new TeamCreateUpdateViewModel
                {
                    MaTeam = x.MaTeam,
                    TenTeam = x.TenTeam ?? string.Empty,
                    MoTaTeam = x.MoTaTeam ?? string.Empty,
                    TrangThaiTeam = TrangThai.ToCode(x.TrangThaiTeam ?? TrangThai.HoatDong)
                })
                .FirstOrDefaultAsync();
        }

        public async Task SaveAsync(TeamCreateUpdateViewModel model)
        {
            var tenTeam = (model.TenTeam ?? string.Empty).Trim();
            var moTaTeam = (model.MoTaTeam ?? string.Empty).Trim();
            var trangThaiTeam = TrangThai.ToCode((model.TrangThaiTeam ?? string.Empty).Trim());

            var tenTeamUpper = tenTeam.ToUpperInvariant();
            var biTrung = await _context.Team.AnyAsync(x =>
                x.IsDeleted != true &&
                x.TenTeam != null &&
                x.TenTeam.ToUpper() == tenTeamUpper &&
                (model.MaTeam == null || x.MaTeam != model.MaTeam.Value));

            if (biTrung)
                throw new Exception("Tên team đã tồn tại.");

            if (model.MaTeam == null)
            {
                var entity = new Team
                {
                    TenTeam = tenTeam,
                    MoTaTeam = moTaTeam,
                    NgayLapTeam = DateTime.Now,
                    TrangThaiTeam = trangThaiTeam,
                    IsDeleted = false
                };

                _context.Team.Add(entity);
            }
            else
            {
                var entity = await _context.Team
                    .FirstOrDefaultAsync(x => x.MaTeam == model.MaTeam && x.IsDeleted != true);

                if (entity == null)
                    throw new Exception("Không tìm thấy team.");

                entity.TenTeam = tenTeam;
                entity.MoTaTeam = moTaTeam;
                entity.TrangThaiTeam = trangThaiTeam;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dangCoThanhVien = await _context.NhanVienTeam.AnyAsync(x => x.MaTeam == id);
            if (dangCoThanhVien)
                throw new Exception("Không thể xóa: team đang có thành viên.");

            var dangGanDuAn = await _context.TeamDuAn.AnyAsync(x => x.MaTeam == id);
            if (dangGanDuAn)
                throw new Exception("Không thể xóa: team đang được gán cho dự án.");

            var entity = await _context.Team
                .FirstOrDefaultAsync(x => x.MaTeam == id && x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy team.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
