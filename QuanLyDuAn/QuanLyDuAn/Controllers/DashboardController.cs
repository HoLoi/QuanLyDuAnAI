using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Ai;

namespace QuanLyDuAn.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IAiService _aiService;
        private readonly IPermissionHelper _permission;
        private readonly IExportFileService _exportFileService;

        public DashboardController(
            IDashboardService dashboardService,
            IAiService aiService,
            IPermissionHelper permission,
            IExportFileService exportFileService)
        {
            _dashboardService = dashboardService;
            _aiService = aiService;
            _permission = permission;
            _exportFileService = exportFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay, string? locNhanh)
        {
            var vm = await _dashboardService.GetDashboardAsync(tuNgay, denNgay, locNhanh);

            var aiDashboard = await _aiService.LayDashboardAsync();
            ViewBag.AiDashboard = new AiDashboardViewModel
            {
                SoCanhBaoDo = aiDashboard.SoDuAnTreChuaXacNhan,
                NguyenNhanThongKe = aiDashboard.NguyenNhanPhoBien
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(DateTime? tuNgay, DateTime? denNgay, string? locNhanh, string? format)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile))
            {
                return Forbid();
            }

            var vm = await _dashboardService.GetDashboardAsync(tuNgay, denNgay, locNhanh);
            var rows = new List<object>
            {
                new DashboardExportRow("Tổng quan", "Tổng dự án", vm.TongDuAn.ToString()),
                new DashboardExportRow("Tổng quan", "Tổng công việc", vm.TongCongViec.ToString()),
                new DashboardExportRow("Tổng quan", "Tổng nhân sự", vm.TongNhanVien.ToString()),
                new DashboardExportRow("Tổng quan", "Tổng ngân sách", ExportSupport.FormatCurrency(vm.TongNganSach)),
                new DashboardExportRow("Tổng quan", "Tổng chi phí", ExportSupport.FormatCurrency(vm.TongChiPhi)),
                new DashboardExportRow("Tổng quan", "Ngân sách còn lại", ExportSupport.FormatCurrency(vm.NganSachConLai)),
                new DashboardExportRow("Tổng quan", "Tỷ lệ sử dụng ngân sách", $"{vm.TyLeSuDungNganSach:0.##}%"),
                new DashboardExportRow("Cảnh báo", "Công việc trễ hạn", vm.CongViecTreHan.ToString()),
                new DashboardExportRow("Cảnh báo", "Nhân sự quá tải", vm.NhanSuQuaTai.ToString()),
                new DashboardExportRow("Cảnh báo", "Dự án vượt ngân sách", vm.DuAnVuotNganSach.ToString()),
                new DashboardExportRow("Cảnh báo", "Dự án thiếu dữ liệu AI", vm.DuAnThieuDatasetAi.ToString())
            };

            for (var i = 0; i < Math.Min(vm.TenDuAn.Count, Math.Min(vm.PhanTramTienDo.Count, vm.ChiPhiTheoDuAn.Count)); i++)
            {
                rows.Add(new DashboardExportRow(
                    "Theo dự án",
                    vm.TenDuAn[i],
                    $"Tiến độ: {vm.PhanTramTienDo[i]}% | Chi phí: {ExportSupport.FormatCurrency(vm.ChiPhiTheoDuAn[i])}"));
            }

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "Báo cáo thống kê tổng quan",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Từ ngày", ExportSupport.FormatDate(tuNgay)),
                    ("Đến ngày", ExportSupport.FormatDate(denNgay)),
                    ("Lọc nhanh", ExportSupport.ResolveTextOrDefault(locNhanh))),
                FileNamePrefix = "thong-ke-tong-quan",
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Nhóm dữ liệu", ValueSelector = row => ((DashboardExportRow)row).NhomDuLieu },
                    new() { Header = "Chỉ số", ValueSelector = row => ((DashboardExportRow)row).ChiSo },
                    new() { Header = "Giá trị", ValueSelector = row => ((DashboardExportRow)row).GiaTri }
                },
                Rows = rows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
        }

        private sealed record DashboardExportRow(string NhomDuLieu, string ChiSo, string GiaTri);
    }
}
