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
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile))
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
                FileNamePrefix = "ngan-sach",
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Mã ngân sách", ValueSelector = row => ((NganSachItemViewModel)row).MaNganSach.ToString() },
                    new() { Header = "Dự án", ValueSelector = row => ((NganSachItemViewModel)row).TenDuAn },
                    new() { Header = "Version", ValueSelector = row => ((NganSachItemViewModel)row).Version?.ToString() ?? string.Empty },
                    new() { Header = "Số tiền ngân sách", ValueSelector = row => ExportSupport.FormatCurrency(((NganSachItemViewModel)row).SoTienNganSach) },
                    new() { Header = "Ngày duyệt", ValueSelector = row => ExportSupport.FormatDate(((NganSachItemViewModel)row).NgayDuyetNganSach) },
                    new() { Header = "Ngày cập nhật", ValueSelector = row => ExportSupport.FormatDate(((NganSachItemViewModel)row).NgayCapNhatNganSach) },
                    new() { Header = "Trạng thái", ValueSelector = row => TrangThai.ToDisplay(((NganSachItemViewModel)row).TrangThaiNganSach) },
                    new() { Header = "Người đề xuất", ValueSelector = row => ((NganSachItemViewModel)row).NguoiDungDeXuat },
                    new() { Header = "Người duyệt", ValueSelector = row => ((NganSachItemViewModel)row).NguoiDungDuyet }
                },
                Rows = rows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
        }
    }
}
