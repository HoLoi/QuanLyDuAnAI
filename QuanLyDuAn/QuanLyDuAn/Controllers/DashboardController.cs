using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Dashboard;
using QuanLyDuAn.ViewModels.Ai;

namespace QuanLyDuAn.Controllers
{
    public class DashboardController : Controller
    {

        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await _dashboardService.GetDashboardAsync();

            ViewBag.AiDashboard = new AiDashboardViewModel();

            return View(vm);
        }
    }
}
