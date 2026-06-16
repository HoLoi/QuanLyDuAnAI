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
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile))
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
                FileNamePrefix = "tien-do-cong-viec",
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Mã chi tiết", ValueSelector = row => ((TienDoCongViecItemViewModel)row).MaChiTietCV.ToString() },
                    new() { Header = "Dự án", ValueSelector = row => ((TienDoCongViecItemViewModel)row).TenDuAn },
                    new() { Header = "Công việc", ValueSelector = row => ((TienDoCongViecItemViewModel)row).TenCongViec },
                    new() { Header = "Chi tiết công việc", ValueSelector = row => ((TienDoCongViecItemViewModel)row).TenChiTietCongViec },
                    new() { Header = "Người thực hiện", ValueSelector = row => ((TienDoCongViecItemViewModel)row).NguoiThucHien },
                    new() { Header = "Tiến độ hiện tại", ValueSelector = row => $"{((TienDoCongViecItemViewModel)row).PhanTramHienTai}%" },
                    new() { Header = "Trạng thái", ValueSelector = row => ((TienDoCongViecItemViewModel)row).TrangThaiCTCV },
                    new() { Header = "Báo cáo gần nhất", ValueSelector = row => ExportSupport.FormatDateTime(((TienDoCongViecItemViewModel)row).ThoiGianBaoCaoGanNhat) },
                    new() { Header = "Trạng thái duyệt", ValueSelector = row => ((TienDoCongViecItemViewModel)row).TrangThaiDuyetBaoCaoGanNhat ?? string.Empty }
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
