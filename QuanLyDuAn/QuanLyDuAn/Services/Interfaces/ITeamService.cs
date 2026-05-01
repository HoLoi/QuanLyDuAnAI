using QuanLyDuAn.ViewModels.Team;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface ITeamService
    {
        Task<List<TeamViewModel>> GetAllAsync(string? tuKhoa, string? trangThaiTeam);
        Task<TeamCreateUpdateViewModel?> GetByIdAsync(int id);
        Task SaveAsync(TeamCreateUpdateViewModel model);
        Task DeleteAsync(int id);
    }
}
