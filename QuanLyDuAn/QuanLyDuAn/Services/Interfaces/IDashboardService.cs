using QuanLyDuAn.ViewModels.Dashboard;

namespace QuanLyDuAn.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(
        DateTime? tuNgay,
        DateTime? denNgay,
        string? locNhanh,
        int? locMaDuAn = null,
        int? locMaQuanLy = null,
        int? locMaTeam = null,
        string? locTrangThai = null,
        int? locMaLoaiDuAn = null,
        string? locTheoNgay = null);
}
