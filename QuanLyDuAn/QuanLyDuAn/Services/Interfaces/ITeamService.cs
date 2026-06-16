using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.Team;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface ITeamService
    {
        Task<List<TeamViewModel>> GetAllAsync(string? tuKhoa, string? trangThaiTeam);
        Task<PagedResultViewModel<TeamViewModel>> GetPagedAsync(string? tuKhoa, string? trangThaiTeam, int pageNumber = 1, int pageSize = 20);
        Task<TeamCreateUpdateViewModel?> GetByIdAsync(int id);
        Task SaveAsync(TeamCreateUpdateViewModel model);
        Task DeleteAsync(int id);
    }
}
