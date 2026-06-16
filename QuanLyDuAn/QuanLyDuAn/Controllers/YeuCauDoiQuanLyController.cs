using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.YeuCauDoiQuanLy;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class YeuCauDoiQuanLyController : Controller
    {
        private readonly IYeuCauDoiQuanLyService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public YeuCauDoiQuanLyController(
            IYeuCauDoiQuanLyService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? maDuAn,
            string? trangThai,
            string? tuKhoa,
            int pageNumber = 1,
            int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.YeuCauDoiQuanLy.Xem))
            {
                return Forbid();
            }

            try
            {
                var vm = await _service.GetPageAsync(maDuAn, trangThai, tuKhoa, pageNumber, pageSize);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "DuAn");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(int maDuAn, string? trangThai, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.YeuCauDoiQuanLy.Them))
            {
                return Forbid();
            }

            try
            {
                var vm = await _service.GetPageAsync(maDuAn, trangThai, tuKhoa);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index), new { maDuAn, trangThai, tuKhoa });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(YeuCauDoiQuanLyPageViewModel vm)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.YeuCauDoiQuanLy.Them))
            {
                return Forbid();
            }

            var model = vm.Form;
            var trangThai = vm.TrangThai;
            var tuKhoa = vm.TuKhoa;

            if (!ModelState.IsValid)
            {
                var invalidVm = await _service.GetPageAsync(model.MaDuAn, trangThai, tuKhoa);
                invalidVm.Form = model;
                invalidVm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(invalidVm);
            }

            try
            {
                await _service.CreateAsync(model);
                TempData["Success"] = "Đã tạo yêu cầu đổi quản lý.";
                return RedirectToAction(nameof(Create), new { maDuAn = model.MaDuAn, trangThai, tuKhoa });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var failedVm = await _service.GetPageAsync(model.MaDuAn, trangThai, tuKhoa);
                failedVm.Form = model;
                failedVm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(failedVm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id, int? maDuAn, string? trangThai, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.YeuCauDoiQuanLy.Them))
            {
                return Forbid();
            }

            try
            {
                await _service.CancelAsync(id);
                TempData["Success"] = "Đã hủy yêu cầu đổi quản lý.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            if (maDuAn.HasValue)
            {
                return RedirectToAction(nameof(Create), new { maDuAn, trangThai, tuKhoa });
            }

            return RedirectToAction(nameof(Index), new { maDuAn, trangThai, tuKhoa });
        }
    }
}
