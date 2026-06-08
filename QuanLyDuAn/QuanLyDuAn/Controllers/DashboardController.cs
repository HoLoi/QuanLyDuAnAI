using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IPermissionHelper _permission;
        private readonly IExportFileService _exportFileService;

        public DashboardController(
            IDashboardService dashboardService,
            IPermissionHelper permission,
            IExportFileService exportFileService)
        {
            _dashboardService = dashboardService;
            _permission = permission;
            _exportFileService = exportFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locNhanh,
            int? locMaDuAn,
            int? locMaQuanLy,
            int? locMaTeam,
            string? locTrangThai,
            int? locMaLoaiDuAn,
            string? locTheoNgay)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.Xem))
            {
                return Forbid();
            }

            var vm = await _dashboardService.GetDashboardAsync(
                tuNgay,
                denNgay,
                locNhanh,
                locMaDuAn,
                locMaQuanLy,
                locMaTeam,
                locTrangThai,
                locMaLoaiDuAn,
                locTheoNgay);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locNhanh,
            string? format,
            int? locMaDuAn,
            int? locMaQuanLy,
            int? locMaTeam,
            string? locTrangThai,
            int? locMaLoaiDuAn,
            string? locTheoNgay)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile))
            {
                return Forbid();
            }

            var vm = await _dashboardService.GetDashboardAsync(
                tuNgay,
                denNgay,
                locNhanh,
                locMaDuAn,
                locMaQuanLy,
                locMaTeam,
                locTrangThai,
                locMaLoaiDuAn,
                locTheoNgay);
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
                new DashboardExportRow("Cảnh báo", "Dự án vượt ngân sách", vm.DuAnVuotNganSach.ToString())
            };

            for (var i = 0; i < Math.Min(vm.TenDuAn.Count, Math.Min(vm.PhanTramTienDo.Count, vm.ChiPhiTheoDuAn.Count)); i++)
            {
                rows.Add(new DashboardExportRow(
                    "Theo dự án",
                    vm.TenDuAn[i],
                    $"Tiến độ: {vm.PhanTramTienDo[i]}% | Chi phí: {ExportSupport.FormatCurrency(vm.ChiPhiTheoDuAn[i])}"));
            }

            foreach (var item in vm.TopDuAnTre)
            {
                rows.Add(new DashboardExportRow(
                    "Top dự án trễ",
                    item.TenDuAn,
                    $"Quản lý: {item.TenQuanLy} | Số ngày trễ: {item.SoNgayTre} | Tiến độ: {item.PhanTramHoanThanh}%"));
            }

            foreach (var item in vm.TopDuAnVuotNganSach)
            {
                rows.Add(new DashboardExportRow(
                    "Top dự án vượt ngân sách",
                    item.TenDuAn,
                    $"Ngân sách: {ExportSupport.FormatCurrency(item.NganSach)} | Chi phí: {ExportSupport.FormatCurrency(item.ChiPhi)} | Chênh lệch: {ExportSupport.FormatCurrency(item.ChenhLech)}"));
            }

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "Báo cáo thống kê tổng quan",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Từ ngày", ExportSupport.FormatDate(tuNgay)),
                    ("Đến ngày", ExportSupport.FormatDate(denNgay)),
                    ("Lọc nhanh", ExportSupport.ResolveTextOrDefault(locNhanh)),
                    ("Dự án", locMaDuAn?.ToString()),
                    ("Quản lý", locMaQuanLy?.ToString()),
                    ("Team", locMaTeam?.ToString()),
                    ("Trạng thái", TrangThai.ToDisplay(locTrangThai)),
                    ("Loại dự án", locMaLoaiDuAn?.ToString()),
                    ("Loại mốc ngày", ExportSupport.ResolveTextOrDefault(locTheoNgay, "Ngày tạo"))),
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
