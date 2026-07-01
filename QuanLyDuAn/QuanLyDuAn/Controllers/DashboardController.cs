using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Dashboard;

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
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile)
                || !await _permission.HasPermissionAsync(User, Permissions.ThongKe.Xem))
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
            var tongQuanRows = new List<object>
            {
                new DashboardKpiExportRow("Tổng dự án", ExportCellValue.Create(vm.TongDuAn), string.Empty),
                new DashboardKpiExportRow("Tổng công việc", ExportCellValue.Create(vm.TongCongViec), string.Empty),
                new DashboardKpiExportRow("Tổng nhân sự", ExportCellValue.Create(vm.TongNhanVien), string.Empty),
                new DashboardKpiExportRow("Tổng ngân sách", ExportCellValue.Create(vm.TongNganSach, "#,##0 \"VNĐ\""), "VNĐ"),
                new DashboardKpiExportRow("Tổng chi phí", ExportCellValue.Create(vm.TongChiPhi, "#,##0 \"VNĐ\""), "VNĐ"),
                new DashboardKpiExportRow("Ngân sách còn lại", ExportCellValue.Create(vm.NganSachConLai, "#,##0 \"VNĐ\""), "VNĐ"),
                new DashboardKpiExportRow("Tỷ lệ sử dụng ngân sách", ExportCellValue.Create(vm.TyLeSuDungNganSach, "0.##\"%\""), "%")
            };
            var canhBaoRows = new List<object>
            {
                new DashboardKpiExportRow("Công việc trễ hạn", ExportCellValue.Create(vm.CongViecTreHan), string.Empty),
                new DashboardKpiExportRow("Nhân sự quá tải", ExportCellValue.Create(vm.NhanSuQuaTai), string.Empty),
                new DashboardKpiExportRow("Dự án vượt ngân sách", ExportCellValue.Create(vm.DuAnVuotNganSach), string.Empty)
            };
            var theoDuAnRows = vm.DuAnTheoDoi.Cast<object>().ToList();
            var duAnTreRows = vm.TopDuAnTre.Cast<object>().ToList();
            var vuotNganSachRows = vm.TopDuAnVuotNganSach.Cast<object>().ToList();

            var csvRows = new List<object>();
            csvRows.AddRange(tongQuanRows.Select(x =>
            {
                var row = (DashboardKpiExportRow)x;
                return (object)new DashboardCsvExportRow("Tổng quan", string.Empty, row.ChiTieu, row.GiaTri.Value, row.DonVi);
            }));
            csvRows.AddRange(canhBaoRows.Select(x =>
            {
                var row = (DashboardKpiExportRow)x;
                return (object)new DashboardCsvExportRow("Cảnh báo", string.Empty, row.ChiTieu, row.GiaTri.Value, row.DonVi);
            }));
            foreach (var item in vm.DuAnTheoDoi)
            {
                csvRows.Add(new DashboardCsvExportRow("Theo dự án", item.TenDuAn, "Quản lý", item.TenQuanLy, string.Empty));
                csvRows.Add(new DashboardCsvExportRow("Theo dự án", item.TenDuAn, "Tiến độ", item.PhanTramTienDo, "%"));
                csvRows.Add(new DashboardCsvExportRow("Theo dự án", item.TenDuAn, "Ngân sách", item.NganSach, "VNĐ"));
                csvRows.Add(new DashboardCsvExportRow("Theo dự án", item.TenDuAn, "Chi phí", item.ChiPhi, "VNĐ"));
                csvRows.Add(new DashboardCsvExportRow("Theo dự án", item.TenDuAn, "Chênh lệch", item.ChenhLech, "VNĐ"));
            }
            foreach (var item in vm.TopDuAnTre)
            {
                csvRows.Add(new DashboardCsvExportRow("Dự án trễ", item.TenDuAn, "Quản lý", item.TenQuanLy, string.Empty));
                csvRows.Add(new DashboardCsvExportRow("Dự án trễ", item.TenDuAn, "Số ngày trễ", item.SoNgayTre, "ngày"));
                csvRows.Add(new DashboardCsvExportRow("Dự án trễ", item.TenDuAn, "Tiến độ", item.PhanTramHoanThanh, "%"));
                csvRows.Add(new DashboardCsvExportRow("Dự án trễ", item.TenDuAn, "Trạng thái", "Trễ tiến độ", string.Empty));
            }
            foreach (var item in vm.TopDuAnVuotNganSach)
            {
                csvRows.Add(new DashboardCsvExportRow("Dự án vượt ngân sách", item.TenDuAn, "Quản lý", item.TenQuanLy, string.Empty));
                csvRows.Add(new DashboardCsvExportRow("Dự án vượt ngân sách", item.TenDuAn, "Ngân sách", item.NganSach, "VNĐ"));
                csvRows.Add(new DashboardCsvExportRow("Dự án vượt ngân sách", item.TenDuAn, "Chi phí", item.ChiPhi, "VNĐ"));
                csvRows.Add(new DashboardCsvExportRow("Dự án vượt ngân sách", item.TenDuAn, "Vượt ngân sách", item.ChenhLech, "VNĐ"));
            }

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "Báo cáo thống kê tổng quan",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Từ ngày", ExportSupport.FormatDate(tuNgay)),
                    ("Đến ngày", ExportSupport.FormatDate(denNgay)),
                    ("Lọc nhanh", ExportSupport.ToDisplayFilterValue(locNhanh)),
                    ("Dự án", ResolveOptionText(vm.DuAnOptions, locMaDuAn)),
                    ("Quản lý", ResolveOptionText(vm.QuanLyOptions, locMaQuanLy)),
                    ("Team", ResolveOptionText(vm.TeamOptions, locMaTeam)),
                    ("Trạng thái", TrangThai.ToDisplay(locTrangThai)),
                    ("Loại dự án", ResolveOptionText(vm.LoaiDuAnOptions, locMaLoaiDuAn)),
                    ("Loại mốc ngày", ExportSupport.ToDisplayFilterValue(locTheoNgay, "Ngày tạo"))),
                DataScopeText = "Theo phạm vi dự án được phép xem; bảng theo dự án lấy tối đa 12 dự án",
                FileNamePrefix = "BaoCaoTongQuan",
                SheetName = "TongQuan",
                PdfLandscape = true,
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Nhóm", ValueSelector = row => ((DashboardCsvExportRow)row).Nhom },
                    new() { Header = "Đối tượng", ValueSelector = row => ((DashboardCsvExportRow)row).DoiTuong },
                    new() { Header = "Chỉ tiêu", ValueSelector = row => ((DashboardCsvExportRow)row).ChiTieu },
                    new() { Header = "Giá trị", ValueSelector = row => ((DashboardCsvExportRow)row).GiaTri },
                    new() { Header = "Đơn vị", ValueSelector = row => ((DashboardCsvExportRow)row).DonVi }
                },
                Rows = csvRows,
                Sections =
                [
                    BuildKpiSection("TỔNG QUAN", "TongQuan", tongQuanRows),
                    BuildKpiSection("CẢNH BÁO", "TongQuan", canhBaoRows),
                    new ExportSectionDefinition
                    {
                        Title = "THỐNG KÊ THEO DỰ ÁN",
                        Description = "Danh sách theo dõi tối đa 12 dự án",
                        SheetName = "TheoDuAn",
                        IncludeRowNumber = true,
                        Rows = theoDuAnRows,
                        Columns =
                        [
                            new() { Header = "Dự án", ValueSelector = row => ((DashboardProjectTrackingItemViewModel)row).TenDuAn, WrapText = true, MinWidth = 22, MaxWidth = 38, PdfRelativeWidth = 1.8f },
                            new() { Header = "Quản lý", ValueSelector = row => ((DashboardProjectTrackingItemViewModel)row).TenQuanLy, MinWidth = 17, MaxWidth = 25, PdfRelativeWidth = 1.2f },
                            new() { Header = "Tiến độ", ValueSelector = row => ((DashboardProjectTrackingItemViewModel)row).PhanTramTienDo, NumberFormat = "0.##\"%\"", Alignment = ExportColumnAlignment.Right },
                            new() { Header = "Ngân sách", ValueSelector = row => ((DashboardProjectTrackingItemViewModel)row).NganSach, NumberFormat = "#,##0 \"VNĐ\"", Alignment = ExportColumnAlignment.Right },
                            new() { Header = "Chi phí", ValueSelector = row => ((DashboardProjectTrackingItemViewModel)row).ChiPhi, NumberFormat = "#,##0 \"VNĐ\"", Alignment = ExportColumnAlignment.Right },
                            new() { Header = "Chênh lệch", ValueSelector = row => ((DashboardProjectTrackingItemViewModel)row).ChenhLech, NumberFormat = "#,##0 \"VNĐ\"", Alignment = ExportColumnAlignment.Right }
                        ]
                    },
                    new ExportSectionDefinition
                    {
                        Title = "DỰ ÁN TRỄ",
                        SheetName = "CanhBao",
                        IncludeRowNumber = true,
                        EnableAutoFilter = false,
                        Rows = duAnTreRows,
                        Columns =
                        [
                            new() { Header = "Dự án", ValueSelector = row => ((DashboardDelayedProjectItemViewModel)row).TenDuAn, WrapText = true, MinWidth = 22, MaxWidth = 38, PdfRelativeWidth = 1.8f },
                            new() { Header = "Quản lý", ValueSelector = row => ((DashboardDelayedProjectItemViewModel)row).TenQuanLy, MinWidth = 17, MaxWidth = 25 },
                            new() { Header = "Số ngày trễ", ValueSelector = row => ((DashboardDelayedProjectItemViewModel)row).SoNgayTre, Alignment = ExportColumnAlignment.Right },
                            new() { Header = "Tiến độ", ValueSelector = row => ((DashboardDelayedProjectItemViewModel)row).PhanTramHoanThanh, NumberFormat = "0.##\"%\"", Alignment = ExportColumnAlignment.Right },
                            new() { Header = "Trạng thái", ValueSelector = _ => "Trễ tiến độ", Alignment = ExportColumnAlignment.Center }
                        ]
                    },
                    new ExportSectionDefinition
                    {
                        Title = "DỰ ÁN VƯỢT NGÂN SÁCH",
                        SheetName = "CanhBao",
                        IncludeRowNumber = true,
                        EnableAutoFilter = false,
                        Rows = vuotNganSachRows,
                        Columns =
                        [
                            new() { Header = "Dự án", ValueSelector = row => ((DashboardBudgetOverrunItemViewModel)row).TenDuAn, WrapText = true, MinWidth = 22, MaxWidth = 38, PdfRelativeWidth = 1.8f },
                            new() { Header = "Ngân sách", ValueSelector = row => ((DashboardBudgetOverrunItemViewModel)row).NganSach, NumberFormat = "#,##0 \"VNĐ\"", Alignment = ExportColumnAlignment.Right },
                            new() { Header = "Chi phí", ValueSelector = row => ((DashboardBudgetOverrunItemViewModel)row).ChiPhi, NumberFormat = "#,##0 \"VNĐ\"", Alignment = ExportColumnAlignment.Right },
                            new() { Header = "Vượt ngân sách", ValueSelector = row => ((DashboardBudgetOverrunItemViewModel)row).ChenhLech, NumberFormat = "#,##0 \"VNĐ\"", Alignment = ExportColumnAlignment.Right },
                            new() { Header = "Quản lý", ValueSelector = row => ((DashboardBudgetOverrunItemViewModel)row).TenQuanLy, MinWidth = 17, MaxWidth = 25 }
                        ]
                    }
                ]
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
        }

        private static ExportSectionDefinition BuildKpiSection(
            string title,
            string sheetName,
            List<object> rows)
            => new()
            {
                Title = title,
                SheetName = sheetName,
                EnableAutoFilter = false,
                Rows = rows,
                Columns =
                [
                    new() { Header = "Chỉ tiêu", ValueSelector = row => ((DashboardKpiExportRow)row).ChiTieu, MinWidth = 24, MaxWidth = 38, PdfRelativeWidth = 2 },
                    new() { Header = "Giá trị", ValueSelector = row => ((DashboardKpiExportRow)row).GiaTri, Alignment = ExportColumnAlignment.Right, MinWidth = 16, MaxWidth = 24 },
                    new() { Header = "Đơn vị", ValueSelector = row => ((DashboardKpiExportRow)row).DonVi, Alignment = ExportColumnAlignment.Center, MinWidth = 10, MaxWidth = 14 }
                ]
            };

        private static string? ResolveOptionText(
            IEnumerable<DashboardFilterOptionViewModel> options,
            int? selectedValue)
        {
            if (!selectedValue.HasValue)
            {
                return null;
            }

            return options.FirstOrDefault(x => x.Value == selectedValue.Value)?.Text
                ?? selectedValue.Value.ToString();
        }

        private sealed record DashboardKpiExportRow(
            string ChiTieu,
            ExportCellValue GiaTri,
            string DonVi);

        private sealed record DashboardCsvExportRow(
            string Nhom,
            string DoiTuong,
            string ChiTieu,
            object? GiaTri,
            string DonVi);
    }
}
