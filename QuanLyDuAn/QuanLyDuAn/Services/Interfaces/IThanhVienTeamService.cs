using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.ThanhVienTeam;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IThanhVienTeamService
    {
        Task<List<ThanhVienTeamViewModel>> GetAllAsync(string? tuKhoa, int? maTeam, bool? isLeader);
        Task<PagedResultViewModel<ThanhVienTeamViewModel>> GetPagedAsync(string? tuKhoa, int? maTeam, bool? isLeader, int pageNumber = 1, int pageSize = 20);
        Task<ThanhVienTeamCreateUpdateViewModel?> GetByIdAsync(int maTeam, int maNguoiDung);
        Task<List<TeamOptionViewModel>> GetTeamOptionsAsync();
        Task<List<NhanSuOptionViewModel>> GetNhanSuOptionsAsync();
        Task SaveAsync(ThanhVienTeamCreateUpdateViewModel model);
        Task SetLeaderAsync(int maTeam, int maNguoiDung);
        Task DeleteAsync(int maTeam, int maNguoiDung);
    }
}
