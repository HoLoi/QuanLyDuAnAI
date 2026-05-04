using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;

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
            if (!await _permission.HasPermissionAsync(User, Permissions.PhanCong.Xem))
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
        public async Task<IActionResult> ThemPhanCong(int maCongViec, int maNhanVien, DateTime? tuNgay, DateTime? denNgay)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.PhanCong.ThucHien))
                return Forbid();

            try
            {
                await _service.AddAsync(maCongViec, maNhanVien, tuNgay, denNgay);
                TempData["Success"] = "Đã phân công công việc thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maCongViec });
        }

        [HttpPost]
        public async Task<IActionResult> XoaPhanCong(int maCongViec, int maNguoiDung)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.PhanCong.ThucHien))
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
