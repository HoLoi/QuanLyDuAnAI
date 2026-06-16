using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Team;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class TeamController : Controller
    {
        private readonly ITeamService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public TeamController(
            ITeamService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? tuKhoa, string? locTrangThaiTeam, int pageNumber = 1, int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.Nhom.Xem))
                return Forbid();

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            var paged = await _service.GetPagedAsync(tuKhoa, locTrangThaiTeam, pageNumber, pageSize);

            var vm = new TeamPageViewModel
            {
                DanhSach = paged.Items,
                Form = new(),
                TuKhoa = tuKhoa,
                LocTrangThaiTeam = locTrangThaiTeam,
                Pagination = paged.Pagination,
                Permissions = permissions
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id, string? tuKhoa, string? locTrangThaiTeam, int pageNumber = 1, int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.Nhom.Sua))
                return Forbid();

            var form = await _service.GetByIdAsync(id);
            if (form == null)
            {
                TempData["Error"] = "Không tìm thấy team.";
                return RedirectToAction(nameof(Index));
            }

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            var paged = await _service.GetPagedAsync(tuKhoa, locTrangThaiTeam, pageNumber, pageSize);

            var vm = new TeamPageViewModel
            {
                DanhSach = paged.Items,
                Form = form,
                TuKhoa = tuKhoa,
                LocTrangThaiTeam = locTrangThaiTeam,
                Pagination = paged.Pagination,
                Permissions = permissions
            };

            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> LuuTeam(TeamPageViewModel vm)
        {
            var model = vm.Form;

            if (!ModelState.IsValid)
            {
                var paged = await _service.GetPagedAsync(vm.TuKhoa, vm.LocTrangThaiTeam);
                vm.DanhSach = paged.Items;
                vm.Pagination = paged.Pagination;
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }

            if (model.MaTeam == null)
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.Nhom.Them))
                    return Forbid();
            }
            else
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.Nhom.Sua))
                    return Forbid();
            }

            try
            {
                await _service.SaveAsync(model);
                TempData["Success"] = "Đã lưu team.";
                return RedirectToAction(nameof(Index), new
                {
                    tuKhoa = vm.TuKhoa,
                    locTrangThaiTeam = vm.LocTrangThaiTeam
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form.TenTeam", ex.Message);
                var paged = await _service.GetPagedAsync(vm.TuKhoa, vm.LocTrangThaiTeam);
                vm.DanhSach = paged.Items;
                vm.Pagination = paged.Pagination;
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaTeam(int maTeam, string? tuKhoa, string? locTrangThaiTeam)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.Nhom.Xoa))
                return Forbid();

            try
            {
                await _service.DeleteAsync(maTeam);
                TempData["Success"] = "Đã xóa team.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locTrangThaiTeam
            });
        }
    }
}
