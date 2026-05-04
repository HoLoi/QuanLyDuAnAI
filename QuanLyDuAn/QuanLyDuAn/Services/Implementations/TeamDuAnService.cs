using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.TeamDuAn;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class TeamDuAnService : ITeamDuAnService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TeamDuAnService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TeamDuAnPageViewModel> GetPageAsync(
            int maDuAn,
            int? maTeamDangChon,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn)
        {
            var duAn = await GetProjectAsync(maDuAn);
            var (coTheChinhSua, lyDoKhongTheChinhSua) = EvaluateEditableCondition(duAn);

            var danhSachTeamDuAn = await (
                from td in _context.TeamDuAn
                join team in _context.Team on td.MaTeam equals team.MaTeam
                where td.MaDuAn == maDuAn && team.IsDeleted != true
                orderby td.NgayTeamThamGiaDA descending
                select new TeamDuAnItemViewModel
                {
                    MaTeam = td.MaTeam,
                    TenTeam = team.TenTeam ?? $"Team {team.MaTeam}",
                    NgayTeamThamGiaDuAn = td.NgayTeamThamGiaDA,
                    SoThanhVienPhuTrach = _context.NhanVienDuAn.Count(nvda =>
                        nvda.MaDuAn == maDuAn &&
                        _context.NhanVienTeam.Any(nvt => nvt.MaTeam == td.MaTeam && nvt.MaNguoiDung == nvda.MaNguoiDung))
                }).ToListAsync();

            var danhSachTeam = await _context.Team
                .Where(x => x.IsDeleted != true)
                .OrderBy(x => x.TenTeam)
                .Select(x => new TeamDuAnTeamOptionViewModel
                {
                    MaTeam = x.MaTeam,
                    TenTeam = x.TenTeam ?? $"Team {x.MaTeam}",
                    TrangThaiTeam = x.TrangThaiTeam ?? string.Empty
                })
                .ToListAsync();

            var vm = new TeamDuAnPageViewModel
            {
                MaDuAn = duAn.MaDuAn,
                TenDuAn = duAn.TenDuAn ?? string.Empty,
                TrangThaiDuAn = duAn.TrangThaiDuAn ?? string.Empty,
                CoTheChinhSua = coTheChinhSua,
                LyDoKhongTheChinhSua = lyDoKhongTheChinhSua,
                MaTeamDangChon = maTeamDangChon,
                DanhSachTeamPhuTrach = danhSachTeamDuAn,
                DanhSachTeamCoTheGan = danhSachTeam,
                TuKhoa = tuKhoa,
                LocMaLoaiDuAn = locMaLoaiDuAn,
                LocTrangThaiDuAn = locTrangThaiDuAn
            };

            if (maTeamDangChon.HasValue)
            {
                var maTeam = maTeamDangChon.Value;
                var selectedIds = await _context.NhanVienDuAn
                    .Where(x => x.MaDuAn == maDuAn)
                    .Select(x => x.MaNguoiDung)
                    .ToListAsync();

                vm.DanhSachThanhVienTeam = await (
                    from nvt in _context.NhanVienTeam
                    join nd in _context.NguoiDung on nvt.MaNguoiDung equals nd.MaNguoiDung
                    where nvt.MaTeam == maTeam && nd.IsDeleted != true
                    orderby (nvt.IsLeader == true) descending, nd.HoTenNguoiDung
                    select new TeamDuAnMemberOptionViewModel
                    {
                        MaNguoiDung = nvt.MaNguoiDung,
                        HoTenNguoiDung = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}",
                        IsLeader = nvt.IsLeader == true,
                        VaiTroTrongTeam = nvt.IsLeader == true
                            ? "Trưởng nhóm"
                            : (nvt.VaiTroTrongTeam ?? "Thành viên"),
                        IsSelected = nvt.IsLeader == true || selectedIds.Contains(nvt.MaNguoiDung)
                    }).ToListAsync();

                vm.SelectedMaNguoiDung = vm.DanhSachThanhVienTeam
                    .Where(x => x.IsSelected)
                    .Select(x => x.MaNguoiDung)
                    .ToList();
            }

            return vm;
        }

        public async Task SaveAsync(int maDuAn, int? maTeam, List<int> maNguoiDungDuocChon)
        {
            if (!maTeam.HasValue)
                throw new Exception("Vui lòng chọn team phụ trách.");

            var selectedIds = (maNguoiDungDuocChon ?? new List<int>())
                .Distinct()
                .ToList();

            var duAn = await GetProjectAsync(maDuAn);
            await EnsureCanManageProjectAsync(duAn);

            var team = await _context.Team
                .FirstOrDefaultAsync(x => x.MaTeam == maTeam.Value && x.IsDeleted != true);

            if (team == null)
                throw new Exception("Team không tồn tại.");

            var teamMemberIds = await _context.NhanVienTeam
                .Where(x => x.MaTeam == maTeam.Value)
                .Select(x => x.MaNguoiDung)
                .ToListAsync();

            if (teamMemberIds.Count == 0)
                throw new Exception("Team chưa có thành viên để phụ trách dự án.");

            var leaderId = await _context.NhanVienTeam
                .Where(x => x.MaTeam == maTeam.Value && x.IsLeader == true)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync();

            if (leaderId <= 0)
                throw new Exception("Team chưa có trưởng nhóm. Vui lòng gán trưởng nhóm trước khi phân công phụ trách dự án.");

            if (!selectedIds.Contains(leaderId))
                throw new Exception("Trưởng nhóm của team bắt buộc phải được phân công phụ trách dự án và không được bỏ chọn.");

            if (selectedIds.Count == 0)
                throw new Exception("Vui lòng chọn ít nhất 1 thành viên trong team.");

            var invalidSelection = selectedIds.Except(teamMemberIds).ToList();
            if (invalidSelection.Count > 0)
                throw new Exception("Danh sách nhân viên được chọn không hợp lệ.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var teamDuAn = await _context.TeamDuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.MaTeam == maTeam.Value);

            var isCreate = false;
            if (teamDuAn == null)
            {
                isCreate = true;
                teamDuAn = new TeamDuAn
                {
                    MaDuAn = maDuAn,
                    MaTeam = maTeam.Value,
                    NgayTeamThamGiaDA = DateTime.Now
                };
                _context.TeamDuAn.Add(teamDuAn);
            }

            var projectMemberIds = await _context.NhanVienDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.MaNguoiDung)
                .ToListAsync();

            var currentTeamMembersInProject = projectMemberIds
                .Where(x => teamMemberIds.Contains(x))
                .ToList();

            var addIds = selectedIds.Except(projectMemberIds).ToList();

            var removeCandidates = currentTeamMembersInProject
                .Except(selectedIds)
                .ToList();

            var otherTeamIds = await _context.TeamDuAn
                .Where(x => x.MaDuAn == maDuAn && x.MaTeam != maTeam.Value)
                .Select(x => x.MaTeam)
                .ToListAsync();

            var protectedByOtherTeams = await _context.NhanVienTeam
                .Where(x => otherTeamIds.Contains(x.MaTeam) && removeCandidates.Contains(x.MaNguoiDung))
                .Select(x => x.MaNguoiDung)
                .Distinct()
                .ToListAsync();

            var removeIds = removeCandidates.Except(protectedByOtherTeams).ToList();

            if (addIds.Count > 0)
            {
                var leaderIds = (await _context.NhanVienTeam
                    .Where(x => x.MaTeam == maTeam.Value && addIds.Contains(x.MaNguoiDung) && x.IsLeader == true)
                    .Select(x => x.MaNguoiDung)
                    .ToListAsync())
                    .ToHashSet();

                foreach (var id in addIds)
                {
                    _context.NhanVienDuAn.Add(new NhanVienDuAn
                    {
                        MaDuAn = maDuAn,
                        MaNguoiDung = id,
                        NgayThamGiaDuAn = DateTime.Now,
                        VaiTroTrongDuAn = leaderIds.Contains(id)
                            ? TrangThai.VaiTroLeader
                            : TrangThai.VaiTroMember
                    });
                }
            }

            if (removeIds.Count > 0)
            {
                var removeEntities = await _context.NhanVienDuAn
                    .Where(x => x.MaDuAn == maDuAn && removeIds.Contains(x.MaNguoiDung))
                    .ToListAsync();

                _context.NhanVienDuAn.RemoveRange(removeEntities);
            }

            if (isCreate)
            {
                _context.NhatKyDuAn.Add(new NhatKyDuAn
                {
                    MaDuAn = maDuAn,
                    MaTeam = maTeam.Value,
                    TeamCuPhuTrach = null,
                    TeamMoiPhuTrach = maTeam.Value,
                    HanhDongNKDA = $"Thêm team {team.TenTeam ?? team.MaTeam.ToString()} phụ trách dự án",
                    ThoiGianNKDA = DateTime.Now
                });
            }
            else
            {
                _context.NhatKyDuAn.Add(new NhatKyDuAn
                {
                    MaDuAn = maDuAn,
                    MaTeam = maTeam.Value,
                    TeamCuPhuTrach = maTeam.Value,
                    TeamMoiPhuTrach = maTeam.Value,
                    HanhDongNKDA = $"Cập nhật danh sách nhân viên phụ trách của team {team.TenTeam ?? team.MaTeam.ToString()}",
                    ThoiGianNKDA = DateTime.Now
                });
            }

            foreach (var id in addIds)
            {
                _context.NhatKyPhuTrachDuAn.Add(new NhatKyPhuTrachDuAn
                {
                    MaDuAn = maDuAn,
                    MaNguoiDung = id,
                    NkHanhDongPTDA = $"Thêm nhân viên vào dự án từ team {team.TenTeam ?? team.MaTeam.ToString()}",
                    NkThoiGianPTDA = DateTime.Now
                });
            }

            foreach (var id in removeIds)
            {
                _context.NhatKyPhuTrachDuAn.Add(new NhatKyPhuTrachDuAn
                {
                    MaDuAn = maDuAn,
                    MaNguoiDung = id,
                    NkHanhDongPTDA = $"Gỡ nhân viên khỏi dự án khi bỏ chọn từ team {team.TenTeam ?? team.MaTeam.ToString()}",
                    NkThoiGianPTDA = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task DeleteAsync(int maDuAn, int maTeam, bool xoaNhanVienThuocTeam)
        {
            var duAn = await GetProjectAsync(maDuAn);
            await EnsureCanManageProjectAsync(duAn);

            var teamDuAn = await _context.TeamDuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.MaTeam == maTeam);

            if (teamDuAn == null)
                throw new Exception("Không tìm thấy team phụ trách trong dự án.");

            var team = await _context.Team
                .FirstOrDefaultAsync(x => x.MaTeam == maTeam);

            var teamMemberIds = await _context.NhanVienTeam
                .Where(x => x.MaTeam == maTeam)
                .Select(x => x.MaNguoiDung)
                .ToListAsync();

            var removeIds = await _context.NhanVienDuAn
                .Where(x => x.MaDuAn == maDuAn && teamMemberIds.Contains(x.MaNguoiDung))
                .Select(x => x.MaNguoiDung)
                .Distinct()
                .ToListAsync();

            var otherTeamIds = await _context.TeamDuAn
                .Where(x => x.MaDuAn == maDuAn && x.MaTeam != maTeam)
                .Select(x => x.MaTeam)
                .ToListAsync();

            var protectedIds = await _context.NhanVienTeam
                .Where(x => otherTeamIds.Contains(x.MaTeam) && teamMemberIds.Contains(x.MaNguoiDung))
                .Select(x => x.MaNguoiDung)
                .Distinct()
                .ToListAsync();

            removeIds = removeIds.Except(protectedIds).ToList();

            if (xoaNhanVienThuocTeam && removeIds.Count > 0)
            {
                var assignedInProjectIds = await (
                    from pccv in _context.PhanCongCongViec
                    join cv in _context.CongViec on pccv.MaCongViec equals cv.MaCongViec
                    join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                    where dm.MaDuAn == maDuAn
                          && cv.IsDeleted != true
                          && removeIds.Contains(pccv.MaNguoiDung)
                    select pccv.MaNguoiDung
                ).Distinct().ToListAsync();

                if (assignedInProjectIds.Count > 0)
                {
                    var tenNhanVien = await _context.NguoiDung
                        .Where(x => assignedInProjectIds.Contains(x.MaNguoiDung))
                        .OrderBy(x => x.HoTenNguoiDung)
                        .Select(x => x.HoTenNguoiDung ?? $"Nhân viên {x.MaNguoiDung}")
                        .ToListAsync();

                    throw new Exception(
                        "Không thể gỡ nhân viên thuộc team này vì đang có phân công công việc: "
                        + string.Join(", ", tenNhanVien));
                }
            }

            if (xoaNhanVienThuocTeam && removeIds.Count == 0)
            {
                throw new Exception("Không thể xóa team vì tất cả nhân viên đang có công việc hoặc còn thuộc team phụ trách khác.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            _context.TeamDuAn.Remove(teamDuAn);

            if (xoaNhanVienThuocTeam && removeIds.Count > 0)
            {
                var removeEntities = await _context.NhanVienDuAn
                    .Where(x => x.MaDuAn == maDuAn && removeIds.Contains(x.MaNguoiDung))
                    .ToListAsync();

                _context.NhanVienDuAn.RemoveRange(removeEntities);
            }

            _context.NhatKyDuAn.Add(new NhatKyDuAn
            {
                MaDuAn = maDuAn,
                MaTeam = maTeam,
                TeamCuPhuTrach = maTeam,
                TeamMoiPhuTrach = null,
                HanhDongNKDA = xoaNhanVienThuocTeam
                    ? $"Xóa team {team?.TenTeam ?? maTeam.ToString()} khỏi dự án và gỡ nhân viên thuộc team"
                    : $"Xóa team {team?.TenTeam ?? maTeam.ToString()} khỏi dự án, giữ nguyên nhân viên phụ trách",
                ThoiGianNKDA = DateTime.Now
            });

            if (xoaNhanVienThuocTeam)
            {
                foreach (var id in removeIds)
                {
                    _context.NhatKyPhuTrachDuAn.Add(new NhatKyPhuTrachDuAn
                    {
                        MaDuAn = maDuAn,
                        MaNguoiDung = id,
                        NkHanhDongPTDA = $"Gỡ nhân viên khỏi dự án do xóa team {team?.TenTeam ?? maTeam.ToString()} phụ trách",
                        NkThoiGianPTDA = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
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
    }
}
