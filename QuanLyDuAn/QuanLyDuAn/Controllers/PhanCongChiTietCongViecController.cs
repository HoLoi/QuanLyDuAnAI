using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.PhanCongChiTietCongViec;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class PhanCongChiTietCongViecController : Controller
    {
        private readonly IPhanCongChiTietCongViecService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public PhanCongChiTietCongViecController(
            IPhanCongChiTietCongViecService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int maChiTietCv)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.PhanCongChiTietCongViec.Xem))
                return Forbid();

            try
            {
                var vm = await _service.GetPageAsync(maChiTietCv);
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
        public async Task<IActionResult> ThemPhanCong(PhanCongChiTietCongViecPageViewModel vm)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.PhanCongChiTietCongViec.ThucHien))
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu phân công không hợp lệ.";
                return RedirectToAction(nameof(Index), new { maChiTietCv = vm.Form.MaChiTietCV });
            }

            try
            {
                await _service.AddAsync(vm.Form);
                TempData["Success"] = "Đã phân công chi tiết công việc thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maChiTietCv = vm.Form.MaChiTietCV });
        }

        [HttpPost]
        public async Task<IActionResult> XoaPhanCong(int maChiTietCv, int maNguoiDung)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.PhanCongChiTietCongViec.ThucHien))
                return Forbid();

            try
            {
                await _service.RemoveAsync(maChiTietCv, maNguoiDung);
                TempData["Success"] = "Đã xóa phân công thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maChiTietCv });
        }
    }
}
