using QuanLyDuAn.ViewModels.Dashboard;

namespace QuanLyDuAn.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync();
}
