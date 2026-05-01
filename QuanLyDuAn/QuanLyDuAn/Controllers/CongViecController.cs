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
    }
}
