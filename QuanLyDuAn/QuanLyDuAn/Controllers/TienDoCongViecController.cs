using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.TienDoCongViec;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class TienDoCongViecController : Controller
    {
        private readonly ITienDoCongViecService _service;
        private readonly IFileTienDoCongViecService _fileService;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;
        private readonly IExportFileService _exportFileService;

        public TienDoCongViecController(
            ITienDoCongViecService service,
            IFileTienDoCongViecService fileService,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService,
            IExportFileService exportFileService)
        {
            _service = service;
            _fileService = fileService;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
            _exportFileService = exportFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? locMaDuAn,
            int? locMaCongViec,
            int? locMaChiTietCv,
            string? tuKhoa,
            DateTime? tuNgayBaoCao,
            DateTime? denNgayBaoCao,
            int pageNumber = 1,
            int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TienDo.Xem))
                return Forbid();

            try
            {
                var vm = await _service.GetPageAsync(locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa, tuNgayBaoCao, denNgayBaoCao, pageNumber, pageSize);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new TienDoCongViecPageViewModel
                {
                    Filter = new TienDoCongViecFilterViewModel
                    {
                        LocMaDuAn = locMaDuAn,
                        LocMaCongViec = locMaCongViec,
                        LocMaChiTietCv = locMaChiTietCv,
                        TuKhoa = tuKhoa,
                        TuNgayBaoCao = tuNgayBaoCao,
                        DenNgayBaoCao = denNgayBaoCao
                    },
                    Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User)
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTienDo(
            TienDoCongViecCapNhatViewModel form,
            int? locMaDuAn,
            int? locMaCongViec,
            int? locMaChiTietCv,
            string? tuKhoa,
            DateTime? tuNgayBaoCao,
            DateTime? denNgayBaoCao)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TienDo.CapNhat))
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

                return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa, tuNgayBaoCao, denNgayBaoCao });
            }

            try
            {
                await _service.CapNhatTienDoAsync(form);
                TempData["Success"] = "Đã gửi báo cáo tiến độ chờ duyệt.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa, tuNgayBaoCao, denNgayBaoCao });
        }

        [HttpPost]
        public async Task<IActionResult> DuyetBaoCaoTienDo(
            TienDoCongViecDuyetViewModel form,
            int? locMaDuAn,
            int? locMaCongViec,
            int? locMaChiTietCv,
            string? tuKhoa,
            DateTime? tuNgayBaoCao,
            DateTime? denNgayBaoCao)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TienDo.Duyet))
                return Forbid();

            try
            {
                await _service.DuyetBaoCaoTienDoAsync(form);
                TempData["Success"] = "Đã duyệt báo cáo tiến độ.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa, tuNgayBaoCao, denNgayBaoCao });
        }

        [HttpPost]
        public async Task<IActionResult> YeuCauBoSungBaoCaoTienDo(
            TienDoCongViecDuyetViewModel form,
            int? locMaDuAn,
            int? locMaCongViec,
            int? locMaChiTietCv,
            string? tuKhoa,
            DateTime? tuNgayBaoCao,
            DateTime? denNgayBaoCao)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TienDo.Duyet))
                return Forbid();

            try
            {
                await _service.YeuCauBoSungBaoCaoTienDoAsync(form);
                TempData["Success"] = "Đã gửi yêu cầu bổ sung báo cáo tiến độ.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa, tuNgayBaoCao, denNgayBaoCao });
        }

        [HttpPost]
        public async Task<IActionResult> TuChoiBaoCaoTienDo(
            TienDoCongViecDuyetViewModel form,
            int? locMaDuAn,
            int? locMaCongViec,
            int? locMaChiTietCv,
            string? tuKhoa,
            DateTime? tuNgayBaoCao,
            DateTime? denNgayBaoCao)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TienDo.Duyet))
                return Forbid();

            try
            {
                await _service.TuChoiBaoCaoTienDoAsync(form);
                TempData["Success"] = "Đã từ chối báo cáo tiến độ.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa, tuNgayBaoCao, denNgayBaoCao });
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(
            string? format,
            int? locMaDuAn,
            int? locMaCongViec,
            int? locMaChiTietCv,
            string? tuKhoa,
            DateTime? tuNgayBaoCao,
            DateTime? denNgayBaoCao)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile)
                || !await _permission.HasPermissionAsync(User, Permissions.TienDo.Xem))
                return Forbid();

            var page = await _service.GetPageAsync(locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa, tuNgayBaoCao, denNgayBaoCao, paginate: false);
            var rows = page.DanhSach.Cast<object>().ToList();

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "Báo cáo tiến độ công việc",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Từ khóa", tuKhoa),
                    ("Mã dự án", locMaDuAn?.ToString()),
                    ("Mã công việc", locMaCongViec?.ToString()),
                    ("Mã chi tiết", locMaChiTietCv?.ToString()),
                    ("Từ ngày báo cáo", ExportSupport.FormatDate(tuNgayBaoCao)),
                    ("Đến ngày báo cáo", ExportSupport.FormatDate(denNgayBaoCao))),
                FileNamePrefix = "BaoCaoTienDoCongViec",
                SheetName = "TienDo",
                IncludeRowNumber = true,
                PdfLandscape = true,
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Dự án", ValueSelector = row => ((TienDoCongViecItemViewModel)row).TenDuAn, WrapText = true, MinWidth = 18, MaxWidth = 30, PdfRelativeWidth = 1.4f },
                    new() { Header = "Công việc", ValueSelector = row => ((TienDoCongViecItemViewModel)row).TenCongViec, WrapText = true, MinWidth = 18, MaxWidth = 30, PdfRelativeWidth = 1.4f },
                    new() { Header = "Chi tiết công việc", ValueSelector = row => ((TienDoCongViecItemViewModel)row).TenChiTietCongViec, WrapText = true, MinWidth = 20, MaxWidth = 34, PdfRelativeWidth = 1.5f },
                    new() { Header = "Người thực hiện", ValueSelector = row => ((TienDoCongViecItemViewModel)row).NguoiThucHien, MinWidth = 16, MaxWidth = 24 },
                    new() { Header = "Tiến độ hiện tại", ValueSelector = row => ((TienDoCongViecItemViewModel)row).PhanTramHienTai, NumberFormat = "0.##\"%\"", Alignment = ExportColumnAlignment.Right, MinWidth = 12, MaxWidth = 15 },
                    new() { Header = "Trạng thái", ValueSelector = row => TrangThai.ToDisplay(((TienDoCongViecItemViewModel)row).TrangThaiCTCV), Alignment = ExportColumnAlignment.Center, MinWidth = 14, MaxWidth = 20 },
                    new() { Header = "Báo cáo gần nhất", ValueSelector = row => ((TienDoCongViecItemViewModel)row).ThoiGianBaoCaoGanNhat, NumberFormat = "dd/MM/yyyy HH:mm", Alignment = ExportColumnAlignment.Center, MinWidth = 16, MaxWidth = 19 },
                    new() { Header = "Trạng thái duyệt", ValueSelector = row => TrangThai.ToDisplay(((TienDoCongViecItemViewModel)row).TrangThaiDuyetBaoCaoGanNhat), Alignment = ExportColumnAlignment.Center, MinWidth = 15, MaxWidth = 22 },
                    new() { Header = "Mã chi tiết", ValueSelector = row => ((TienDoCongViecItemViewModel)row).MaChiTietCV, Alignment = ExportColumnAlignment.Center, MinWidth = 10, MaxWidth = 13, ShowInPdf = false }
                },
                Rows = rows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
        }

        [HttpPost]
        public async Task<IActionResult> ThemFileTienDo(
            int maChiTietCv,
            IFormFile file,
            int? locMaDuAn,
            int? locMaCongViec,
            int? locMaChiTietCv,
            string? tuKhoa,
            DateTime? tuNgayBaoCao,
            DateTime? denNgayBaoCao)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TienDo.CapNhat))
                return Forbid();

            try
            {
                await _fileService.UploadAsync(maChiTietCv, file);
                TempData["Success"] = "Đã tải lên tệp minh chứng tiến độ.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa, tuNgayBaoCao, denNgayBaoCao });
        }

        [HttpGet]
        public async Task<IActionResult> TaiFileTienDo(
            int maFileTdcv,
            int? locMaDuAn,
            int? locMaCongViec,
            int? locMaChiTietCv,
            string? tuKhoa,
            DateTime? tuNgayBaoCao,
            DateTime? denNgayBaoCao)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TienDo.Xem))
                return Forbid();

            try
            {
                var (fullPath, fileName, _) = await _fileService.GetDownloadInfoAsync(maFileTdcv);
                return PhysicalFile(fullPath, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa, tuNgayBaoCao, denNgayBaoCao });
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaFileTienDo(
            int maFileTdcv,
            int? locMaDuAn,
            int? locMaCongViec,
            int? locMaChiTietCv,
            string? tuKhoa,
            DateTime? tuNgayBaoCao,
            DateTime? denNgayBaoCao)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TienDo.CapNhat))
                return Forbid();

            try
            {
                await _fileService.DeleteAsync(maFileTdcv);
                TempData["Success"] = "Đã xóa tệp minh chứng tiến độ.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa, tuNgayBaoCao, denNgayBaoCao });
        }
    }
}
