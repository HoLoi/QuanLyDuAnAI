using QuanLyDuAn.ViewModels.TeamDuAn;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface ITeamDuAnService
    {
        Task<TeamDuAnPageViewModel> GetPageAsync(
            int maDuAn,
            int? maTeamDangChon,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn);

        Task SaveAsync(int maDuAn, int? maTeam, List<int> maNguoiDungDuocChon);
        Task DeleteAsync(int maDuAn, int maTeam, bool xoaNhanVienThuocTeam);
    }
}
