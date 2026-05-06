using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.ChiTietCongViec;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class ChiTietCongViecController : Controller
    {
        private readonly IChiTietCongViecService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public ChiTietCongViecController(
            IChiTietCongViecService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int maCongViec)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ChiTietCongViec.Xem))
                return Forbid();

            try
            {
                var vm = await _service.GetPageAsync(maCongViec);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "CongViec");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Them(ChiTietCongViecCreateUpdateViewModel form)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ChiTietCongViec.Them))
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
                return await RenderIndexWithFormAsync(form.MaCongViec, form);
            }

            try
            {
                await _service.AddAsync(form);
                TempData["Success"] = "Đã thêm chi tiết công việc thành công.";
                return RedirectToAction(nameof(Index), new { maCongViec = form.MaCongViec });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return await RenderIndexWithFormAsync(form.MaCongViec, form);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Sua(ChiTietCongViecCreateUpdateViewModel form)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ChiTietCongViec.Sua))
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
                return await RenderIndexWithFormAsync(form.MaCongViec, form);
            }

            try
            {
                await _service.UpdateAsync(form);
                TempData["Success"] = "Đã cập nhật chi tiết công việc thành công.";
                return RedirectToAction(nameof(Index), new { maCongViec = form.MaCongViec });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return await RenderIndexWithFormAsync(form.MaCongViec, form);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int maCongViec, int maChiTietCv)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ChiTietCongViec.Xoa))
                return Forbid();

            try
            {
                await _service.RemoveAsync(maCongViec, maChiTietCv);
                TempData["Success"] = "Đã xóa chi tiết công việc thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maCongViec });
        }

        private async Task<IActionResult> RenderIndexWithFormAsync(int maCongViec, ChiTietCongViecCreateUpdateViewModel form)
        {
            try
            {
                var vm = await _service.GetPageAsync(maCongViec);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                vm.Form = form;
                return View(nameof(Index), vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "CongViec");
            }
        }
    }
}
