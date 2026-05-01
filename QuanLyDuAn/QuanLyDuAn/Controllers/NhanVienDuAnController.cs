using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.NhanVienDuAn;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class NhanVienDuAnController : Controller
    {
        private readonly INhanVienDuAnService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public NhanVienDuAnController(
            INhanVienDuAnService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int maDuAn, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThanhVienDuAn.Xem))
                return Forbid();

            if (maDuAn <= 0)
            {
                TempData["Error"] = "Vui lòng chọn dự án để quản lý nhân viên phụ trách.";
                return RedirectToAction("Index", "DuAn");
            }

            var vm = await _service.GetPageAsync(maDuAn, tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn);
            vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ThemNhanVien(NhanVienDuAnPageViewModel vm)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThanhVienDuAn.Them))
                return Forbid();

            try
            {
                await _service.AddAsync(vm.MaDuAn, vm.SelectedMaNguoiDung);
                TempData["Success"] = "Đã thêm nhân viên phụ trách dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                maDuAn = vm.MaDuAn,
                tuKhoa = vm.TuKhoa,
                locMaLoaiDuAn = vm.LocMaLoaiDuAn,
                locTrangThaiDuAn = vm.LocTrangThaiDuAn
            });
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatVaiTro(
            int maDuAn,
            int maNguoiDung,
            string vaiTroTrongDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThanhVienDuAn.Them))
                return Forbid();

            try
            {
                await _service.UpdateRoleAsync(maDuAn, maNguoiDung, vaiTroTrongDuAn);
                TempData["Success"] = "Đã cập nhật vai trò nhân viên phụ trách.";
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

        [HttpPost]
        public async Task<IActionResult> XoaNhanVien(
            int maDuAn,
            int maNguoiDung,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThanhVienDuAn.Xoa))
                return Forbid();

            try
            {
                await _service.RemoveAsync(maDuAn, maNguoiDung);
                TempData["Success"] = "Đã xóa nhân viên phụ trách khỏi dự án.";
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
