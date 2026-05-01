using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.Constants;
using QuanLyDuAn.ViewModels.NhanSu;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class NhanSuController : Controller
    {
        private readonly INhanSuService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public NhanSuController(
            INhanSuService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? tuKhoa, int? locMaChucDanh, string? locTrangThaiTaiKhoan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Xem))
                return Forbid();

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

            var vm = new NhanSuPageViewModel
            {
                DanhSach = await _service.GetAllAsync(tuKhoa, locMaChucDanh, locTrangThaiTaiKhoan),
                Form = new(),
                DanhSachChucDanh = await _service.GetChucDanhOptionsAsync(),
                DanhSachVaiTroHeThong = await _service.GetVaiTroHeThongOptionsAsync(),
                TuKhoa = tuKhoa,
                LocMaChucDanh = locMaChucDanh,
                LocTrangThaiTaiKhoan = locTrangThaiTaiKhoan,
                Permissions = permissions
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Sua))
                return Forbid();

            var form = await _service.GetByIdAsync(id);
            if (form == null)
            {
                TempData["Error"] = "Không tìm thấy nhân sự";
                return RedirectToAction(nameof(Index));
            }

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

            var vm = new NhanSuPageViewModel
            {
                DanhSach = await _service.GetAllAsync(null, null, null),
                Form = form,
                DanhSachChucDanh = await _service.GetChucDanhOptionsAsync(),
                DanhSachVaiTroHeThong = await _service.GetVaiTroHeThongOptionsAsync(),
                Permissions = permissions
            };

            return View("Index", vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> LuuNhanSu(NhanSuPageViewModel vm)
        {
            var model = vm.Form;

            if (!ModelState.IsValid)
            {
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, vm.LocMaChucDanh, vm.LocTrangThaiTaiKhoan);
                vm.DanhSachChucDanh = await _service.GetChucDanhOptionsAsync();
                vm.DanhSachVaiTroHeThong = await _service.GetVaiTroHeThongOptionsAsync();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }

            if (model.MaNguoiDung == null)
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Them))
                    return Forbid();
            }
            else
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Sua))
                    return Forbid();
            }

            try
            {
                var laAdminDangThaoTac = User.IsInRole("ADMIN") || User.IsInRole("Admin");
                var warning = await _service.SaveAsync(model, laAdminDangThaoTac);
                TempData["Success"] = "Đã lưu nhân sự";
                if (!string.IsNullOrWhiteSpace(warning))
                {
                    TempData["Warning"] = warning;
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ModelState.AddModelError(string.Empty, ex.Message);
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, vm.LocMaChucDanh, vm.LocTrangThaiTaiKhoan);
                vm.DanhSachChucDanh = await _service.GetChucDanhOptionsAsync();
                vm.DanhSachVaiTroHeThong = await _service.GetVaiTroHeThongOptionsAsync();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaNhanSu(int maNguoiDung)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Xoa))
                return Forbid();

            try
            {
                await _service.DeleteAsync(maNguoiDung);
                TempData["Success"] = "Đã xóa nhân sự";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> KhoaTaiKhoan(int maNguoiDung)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Khoa))
                return Forbid();

            try
            {
                await _service.LockAccountAsync(maNguoiDung);
                TempData["Success"] = "Đã khóa tài khoản nhân sự";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MoKhoaTaiKhoan(int maNguoiDung)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.MoKhoa))
                return Forbid();

            try
            {
                await _service.UnlockAccountAsync(maNguoiDung);
                TempData["Success"] = "Đã mở khóa tài khoản nhân sự";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
