using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.NganSach;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class NganSachController : Controller
    {
        private readonly INganSachService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;
        private readonly IExportFileService _exportFileService;

        public NganSachController(
            INganSachService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService,
            IExportFileService exportFileService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
            _exportFileService = exportFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? locMaDuAn, string? locTrangThai, int pageNumber = 1, int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NganSach.Xem))
                return Forbid();

            var vm = await _service.GetPageAsync(locMaDuAn, locTrangThai, pageNumber, pageSize);
            vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(string? format, int? locMaDuAn, string? locTrangThai)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile)
                || !await _permission.HasPermissionAsync(User, Permissions.NganSach.Xem))
                return Forbid();

            var page = await _service.GetPageAsync(locMaDuAn, locTrangThai, paginate: false);
            var rows = page.DanhSach.Cast<object>().ToList();

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "Báo cáo ngân sách dự án",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Mã dự án", locMaDuAn?.ToString()),
                    ("Trạng thái", TrangThai.ToDisplay(locTrangThai))),
                FileNamePrefix = "BaoCaoNganSach",
                SheetName = "NganSach",
                IncludeRowNumber = true,
                PdfLandscape = true,
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Dự án", ValueSelector = row => ((NganSachItemViewModel)row).TenDuAn, WrapText = true, MinWidth = 20, MaxWidth = 34, PdfRelativeWidth = 1.6f },
                    new() { Header = "Phiên bản", ValueSelector = row => ((NganSachItemViewModel)row).Version, NumberFormat = "0", Alignment = ExportColumnAlignment.Center, MinWidth = 9, MaxWidth = 11 },
                    new() { Header = "Số tiền ngân sách", ValueSelector = row => ((NganSachItemViewModel)row).SoTienNganSach, NumberFormat = "#,##0 \"VNĐ\"", Alignment = ExportColumnAlignment.Right, MinWidth = 18, MaxWidth = 24 },
                    new() { Header = "Ngày duyệt", ValueSelector = row => ((NganSachItemViewModel)row).NgayDuyetNganSach, NumberFormat = "dd/MM/yyyy HH:mm", Alignment = ExportColumnAlignment.Center, MinWidth = 16, MaxWidth = 19 },
                    new() { Header = "Ngày cập nhật", ValueSelector = row => ((NganSachItemViewModel)row).NgayCapNhatNganSach, NumberFormat = "dd/MM/yyyy HH:mm", Alignment = ExportColumnAlignment.Center, MinWidth = 16, MaxWidth = 19 },
                    new() { Header = "Trạng thái", ValueSelector = row => TrangThai.ToDisplay(((NganSachItemViewModel)row).TrangThaiNganSach), Alignment = ExportColumnAlignment.Center, MinWidth = 14, MaxWidth = 20 },
                    new() { Header = "Người đề xuất", ValueSelector = row => ((NganSachItemViewModel)row).NguoiDungDeXuat, MinWidth = 16, MaxWidth = 24 },
                    new() { Header = "Người duyệt", ValueSelector = row => ((NganSachItemViewModel)row).NguoiDungDuyet, MinWidth = 16, MaxWidth = 24 },
                    new() { Header = "Mã ngân sách", ValueSelector = row => ((NganSachItemViewModel)row).MaNganSach, Alignment = ExportColumnAlignment.Center, MinWidth = 11, MaxWidth = 14, ShowInPdf = false }
                },
                Summaries = new List<ExportSummaryDefinition>
                {
                    new() { Label = "Tổng ngân sách", Value = page.TongNganSach, NumberFormat = "#,##0 \"VNĐ\"" },
                    new() { Label = "Ngân sách đang hiệu lực", Value = page.TongNganSachDangHieuLuc, NumberFormat = "#,##0 \"VNĐ\"" },
                    new() { Label = "Đã sử dụng", Value = page.TongDaSuDung, NumberFormat = "#,##0 \"VNĐ\"" },
                    new() { Label = "Còn lại", Value = page.TongConLai, NumberFormat = "#,##0 \"VNĐ\"" }
                },
                Rows = rows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
        }
    }
}
