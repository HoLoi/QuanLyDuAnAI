using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DeXuatNganSach;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class DeXuatNganSachController : Controller
    {
        private readonly IDeXuatNganSachService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public DeXuatNganSachController(
            IDeXuatNganSachService service,
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
            if (!await _permission.HasPermissionAsync(User, Permissions.DeXuatNganSach.Xem))
                return Forbid();

            if (!locMaDuAn.HasValue)
            {
                TempData["Error"] = "Vui lòng chọn dự án để đề xuất ngân sách.";
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
        public async Task<IActionResult> TaoDeXuat(DeXuatNganSachPageViewModel vm)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DeXuatNganSach.Them))
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
                TempData["Success"] = "Đã tạo đề xuất ngân sách.";
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
        public async Task<IActionResult> HuyDeXuat(int maDeXuatNs, int? locMaDuAn, string? locTrangThai)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DeXuatNganSach.Them))
                return Forbid();

            try
            {
                await _service.CancelAsync(maDeXuatNs);
                TempData["Success"] = "Đã hủy đề xuất ngân sách.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai });
        }
    }
}
