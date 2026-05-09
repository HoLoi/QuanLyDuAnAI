using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
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

        public TienDoCongViecController(
            ITienDoCongViecService service,
            IFileTienDoCongViecService fileService,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _fileService = fileService;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? locMaDuAn, int? locMaCongViec, int? locMaChiTietCv, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TienDo.Xem))
                return Forbid();

            try
            {
                var vm = await _service.GetPageAsync(locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa);
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
                        TuKhoa = tuKhoa
                    },
                    Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User)
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTienDo(TienDoCongViecCapNhatViewModel form, int? locMaDuAn, int? locMaCongViec, int? locMaChiTietCv, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TienDo.CapNhat))
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

                return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa });
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

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa });
        }

        [HttpPost]
        public async Task<IActionResult> DuyetBaoCaoTienDo(TienDoCongViecDuyetViewModel form, int? locMaDuAn, int? locMaCongViec, int? locMaChiTietCv, string? tuKhoa)
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

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa });
        }

        [HttpPost]
        public async Task<IActionResult> YeuCauBoSungBaoCaoTienDo(TienDoCongViecDuyetViewModel form, int? locMaDuAn, int? locMaCongViec, int? locMaChiTietCv, string? tuKhoa)
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

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa });
        }

        [HttpPost]
        public async Task<IActionResult> TuChoiBaoCaoTienDo(TienDoCongViecDuyetViewModel form, int? locMaDuAn, int? locMaCongViec, int? locMaChiTietCv, string? tuKhoa)
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

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa });
        }

        [HttpPost]
        public async Task<IActionResult> ThemFileTienDo(int maChiTietCv, IFormFile file, int? locMaDuAn, int? locMaCongViec, int? locMaChiTietCv, string? tuKhoa)
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

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa });
        }

        [HttpGet]
        public async Task<IActionResult> TaiFileTienDo(int maFileTdcv, int? locMaDuAn, int? locMaCongViec, int? locMaChiTietCv, string? tuKhoa)
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
                return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa });
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaFileTienDo(int maFileTdcv, int? locMaDuAn, int? locMaCongViec, int? locMaChiTietCv, string? tuKhoa)
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

            return RedirectToAction(nameof(Index), new { locMaDuAn, locMaCongViec, locMaChiTietCv, tuKhoa });
        }
    }
}
