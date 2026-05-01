using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DeXuatCongViec;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class DeXuatCongViecController : Controller
    {
        private readonly IDeXuatCongViecService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public DeXuatCongViecController(
            IDeXuatCongViecService service,
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
            if (!await _permission.HasPermissionAsync(User, Permissions.DeXuatCongViec.Xem))
                return Forbid();

            if (!locMaDuAn.HasValue)
            {
                TempData["Error"] = "Vui lòng chọn dự án để đề xuất công việc.";
                return RedirectToAction("Index", "DuAn");
            }

            try
            {
                var vm = await _service.GetPageAsync(locMaDuAn, locTrangThai);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "DuAn");
            }
        }

        [HttpPost]
        public async Task<IActionResult> TaoDeXuat(DeXuatCongViecPageViewModel vm)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DeXuatCongViec.Them))
                return Forbid();

            if (!ModelState.IsValid)
            {
                var selectedMaDuAn = vm.LocMaDuAn ?? vm.Form.MaDuAn;
                var invalidVm = await _service.GetPageAsync(selectedMaDuAn, vm.LocTrangThai);
                invalidVm.Form = vm.Form;
                invalidVm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", invalidVm);
            }

            try
            {
                await _service.CreateAsync(vm.Form);
                TempData["Success"] = "Đã tạo đề xuất công việc.";
                return RedirectToAction(nameof(Index), new
                {
                    locMaDuAn = vm.LocMaDuAn ?? vm.Form.MaDuAn,
                    locTrangThai = vm.LocTrangThai
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var selectedMaDuAn = vm.LocMaDuAn ?? vm.Form.MaDuAn;
                var failedVm = await _service.GetPageAsync(selectedMaDuAn, vm.LocTrangThai);
                failedVm.Form = vm.Form;
                failedVm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", failedVm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> HuyDeXuat(int maDeXuatCv, int? locMaDuAn, string? locTrangThai)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DeXuatCongViec.Them))
                return Forbid();

            try
            {
                await _service.CancelAsync(maDeXuatCv);
                TempData["Success"] = "Đã hủy đề xuất công việc.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                locMaDuAn,
                locTrangThai
            });
        }
    }
}
