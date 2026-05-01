using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class DuyetDeXuatCongViecController : Controller
    {
        private readonly IDuyetDeXuatCongViecService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public DuyetDeXuatCongViecController(
            IDuyetDeXuatCongViecService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? locMaDuAn, string? locTrangThai)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuyetDeXuatCongViec.Xem))
                return Forbid();

            var vm = await _service.GetPageAsync(locMaDuAn, locTrangThai);
            vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Duyet(int maDeXuatCv, int? locMaDuAn, string? locTrangThai)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuyetDeXuatCongViec.Duyet))
                return Forbid();

            try
            {
                await _service.ApproveAsync(maDeXuatCv);
                TempData["Success"] = "Đã duyệt đề xuất công việc.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai });
        }

        [HttpPost]
        public async Task<IActionResult> TuChoi(int maDeXuatCv, string? lyDo, int? locMaDuAn, string? locTrangThai)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuyetDeXuatCongViec.Duyet))
                return Forbid();

            try
            {
                await _service.RejectAsync(maDeXuatCv, lyDo);
                TempData["Success"] = "Đã từ chối đề xuất công việc.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai });
        }
    }
}
