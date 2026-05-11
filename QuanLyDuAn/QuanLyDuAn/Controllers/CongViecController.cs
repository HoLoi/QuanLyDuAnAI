using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class CongViecController : Controller
    {
        private readonly ICongViecService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public CongViecController(
            ICongViecService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? locMaDuAn, string? locTrangThai, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.CongViec.Xem))
                return Forbid();

            var vm = await _service.GetPageAsync(locMaDuAn, locTrangThai, tuKhoa);
            vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanHoanThanh(int maCongViec, int? locMaDuAn, string? locTrangThai, string? tuKhoa)
        {
            if (!await CoQuyenXuLyWorkflowCongViecAsync())
                return Forbid();

            try
            {
                await _service.XacNhanHoanThanhCongViecAsync(maCongViec);
                TempData["Success"] = "Đã xác nhận hoàn thành công việc.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai, tuKhoa });
        }

        [HttpPost]
        public async Task<IActionResult> MoLai(int maCongViec, string lyDo, int? locMaDuAn, string? locTrangThai, string? tuKhoa)
        {
            if (!await CoQuyenXuLyWorkflowCongViecAsync())
                return Forbid();

            try
            {
                await _service.MoLaiCongViecAsync(maCongViec, lyDo);
                TempData["Success"] = "Đã mở lại công việc.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai, tuKhoa });
        }

        private async Task<bool> CoQuyenXuLyWorkflowCongViecAsync()
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.CongViec.Xem))
                return false;

            return await _permission.HasPermissionAsync(User, Permissions.TienDo.Duyet)
                   || await _permission.HasPermissionAsync(User, Permissions.DuyetDeXuatCongViec.Duyet);
        }
    }
}
