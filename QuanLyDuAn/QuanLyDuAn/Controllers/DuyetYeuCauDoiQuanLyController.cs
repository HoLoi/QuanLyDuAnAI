using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class DuyetYeuCauDoiQuanLyController : Controller
    {
        private readonly IDuyetYeuCauDoiQuanLyService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public DuyetYeuCauDoiQuanLyController(
            IDuyetYeuCauDoiQuanLyService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? trangThai, int? maDuAn, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuyetYeuCauDoiQuanLy.Xem))
            {
                return Forbid();
            }

            var vm = await _service.GetPageAsync(trangThai, maDuAn, tuKhoa);
            vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuyetYeuCauDoiQuanLy.Xem))
            {
                return Forbid();
            }

            try
            {
                var vm = await _service.GetDetailsAsync(id);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id, string? trangThai, int? maDuAn, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuyetYeuCauDoiQuanLy.Duyet))
            {
                return Forbid();
            }

            try
            {
                await _service.ApproveAsync(id);
                TempData["Success"] = "Đã duyệt yêu cầu đổi quản lý.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { trangThai, maDuAn, tuKhoa });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string? lyDoTuChoi, string? trangThai, int? maDuAn, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuyetYeuCauDoiQuanLy.Duyet))
            {
                return Forbid();
            }

            try
            {
                await _service.RejectAsync(id, lyDoTuChoi);
                TempData["Success"] = "Đã từ chối yêu cầu đổi quản lý.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { trangThai, maDuAn, tuKhoa });
        }
    }
}
