using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.PhanCongCongViec;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class PhanCongCongViecController : Controller
    {
        private readonly IPhanCongCongViecService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public PhanCongCongViecController(
            IPhanCongCongViecService service,
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
            if (!await _permission.HasPermissionAsync(User, Permissions.PhanCongCongViec.Xem))
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
        public async Task<IActionResult> ThemPhanCong(PhanCongCongViecPageViewModel vm)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.PhanCongCongViec.ThucHien))
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu phân công không hợp lệ.";
                return RedirectToAction(nameof(Index), new { maCongViec = vm.Form.MaCongViec });
            }

            try
            {
                await _service.AddAsync(vm.Form);
                TempData["Success"] = "Đã phân công công việc thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maCongViec = vm.Form.MaCongViec });
        }

        [HttpPost]
        public async Task<IActionResult> XoaPhanCong(int maCongViec, int maNguoiDung)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.PhanCongCongViec.ThucHien))
                return Forbid();

            try
            {
                await _service.RemoveAsync(maCongViec, maNguoiDung);
                TempData["Success"] = "Đã xóa phân công thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maCongViec });
        }
    }
}
