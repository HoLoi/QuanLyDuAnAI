using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.TeamDuAn;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class TeamDuAnController : Controller
    {
        private readonly ITeamDuAnService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public TeamDuAnController(
            ITeamDuAnService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int maDuAn,
            int? maTeamDangChon,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TeamDuAn.Xem))
                return Forbid();

            if (maDuAn <= 0)
            {
                TempData["Error"] = "Vui lòng chọn dự án để quản lý team phụ trách.";
                return RedirectToAction("Index", "DuAn");
            }

            var vm = await _service.GetPageAsync(maDuAn, maTeamDangChon, tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn);
            vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> LuuTeamPhuTrach(TeamDuAnPageViewModel vm)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TeamDuAn.Them))
                return Forbid();

            try
            {
                await _service.SaveAsync(vm.MaDuAn, vm.MaTeamDangChon, vm.SelectedMaNguoiDung);
                TempData["Success"] = "Đã lưu team phụ trách và thành viên dự án tương ứng.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                maDuAn = vm.MaDuAn,
                maTeamDangChon = vm.MaTeamDangChon,
                tuKhoa = vm.TuKhoa,
                locMaLoaiDuAn = vm.LocMaLoaiDuAn,
                locTrangThaiDuAn = vm.LocTrangThaiDuAn
            });
        }

        [HttpPost]
        public async Task<IActionResult> XoaTeamKhoiDuAn(
            int maDuAn,
            int maTeam,
            int luaChonXuLyNhanVien,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.TeamDuAn.Xoa))
                return Forbid();

            try
            {
                var xoaNhanVien = luaChonXuLyNhanVien == 2;
                await _service.DeleteAsync(maDuAn, maTeam, xoaNhanVien);
                TempData["Success"] = xoaNhanVien
                    ? "Đã xóa team phụ trách và gỡ nhân viên thuộc team khỏi dự án."
                    : "Đã xóa team phụ trách khỏi dự án và giữ nguyên nhân viên.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                maDuAn,
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn
            });
        }
    }
}
