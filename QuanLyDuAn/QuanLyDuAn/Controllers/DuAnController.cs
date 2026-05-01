using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DuAn;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class DuAnController : Controller
    {
        private readonly IDuAnService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public DuAnController(
            IDuAnService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Xem))
                return Forbid();

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

            var vm = new DuAnPageViewModel
            {
                DanhSach = await _service.GetAllAsync(tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn),
                Form = new(),
                DanhSachLoaiDuAn = await _service.GetLoaiDuAnOptionsAsync(),
                TuKhoa = tuKhoa,
                LocMaLoaiDuAn = locMaLoaiDuAn,
                LocTrangThaiDuAn = locTrangThaiDuAn,
                Permissions = permissions
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Xem))
                return Forbid();

            var form = await _service.GetByIdAsync(id);
            if (form == null)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction(nameof(Index), new
                {
                    tuKhoa,
                    locMaLoaiDuAn,
                    locTrangThaiDuAn
                });
            }

            var loaiDuAnOptions = await _service.GetLoaiDuAnOptionsAsync();
            ViewBag.TenLoaiDuAn = loaiDuAnOptions
                .FirstOrDefault(x => x.MaLoaiDuAn == form.MaLoaiDuAn)?.TenLoai;

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.LocMaLoaiDuAn = locMaLoaiDuAn;
            ViewBag.LocTrangThaiDuAn = locTrangThaiDuAn;

            return View(form);
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            var form = await _service.GetByIdAsync(id);
            if (form == null)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction(nameof(Index));
            }

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

            var vm = new DuAnPageViewModel
            {
                DanhSach = await _service.GetAllAsync(tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn),
                Form = form,
                DanhSachLoaiDuAn = await _service.GetLoaiDuAnOptionsAsync(),
                TuKhoa = tuKhoa,
                LocMaLoaiDuAn = locMaLoaiDuAn,
                LocTrangThaiDuAn = locTrangThaiDuAn,
                Permissions = permissions
            };

            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> LuuDuAn(DuAnPageViewModel vm)
        {
            var model = vm.Form;

            if (!ModelState.IsValid)
            {
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, vm.LocMaLoaiDuAn, vm.LocTrangThaiDuAn);
                vm.DanhSachLoaiDuAn = await _service.GetLoaiDuAnOptionsAsync();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }

            if (model.MaDuAn == null)
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Them))
                    return Forbid();
            }
            else
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                    return Forbid();
            }

            try
            {
                await _service.SaveAsync(model);
                TempData["Success"] = "Đã lưu dự án.";
                return RedirectToAction(nameof(Index), new
                {
                    tuKhoa = vm.TuKhoa,
                    locMaLoaiDuAn = vm.LocMaLoaiDuAn,
                    locTrangThaiDuAn = vm.LocTrangThaiDuAn
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, vm.LocMaLoaiDuAn, vm.LocTrangThaiDuAn);
                vm.DanhSachLoaiDuAn = await _service.GetLoaiDuAnOptionsAsync();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaDuAn(int maDuAn, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Xoa))
                return Forbid();

            try
            {
                await _service.DeleteAsync(maDuAn);
                TempData["Success"] = "Đã xóa dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn
            });
        }

        [HttpPost]
        public async Task<IActionResult> BatDauDuAn(int maDuAn, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                await _service.TransitionToDangThucHienAsync(maDuAn);
                TempData["Success"] = "Đã bắt đầu dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn
            });
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanHoanThanh(int maDuAn, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                await _service.ConfirmCompletionAsync(maDuAn);
                TempData["Success"] = "Đã xác nhận hoàn thành dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn
            });
        }

        [HttpPost]
        public async Task<IActionResult> TamDungDuAn(int maDuAn, string ghiChuDuAn, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                await _service.PauseProjectAsync(maDuAn, ghiChuDuAn);
                TempData["Success"] = "Đã tạm dừng dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn
            });
        }

        [HttpPost]
        public async Task<IActionResult> YeuCauHoanThanh(int maDuAn, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                await _service.RequestCompletionAsync(maDuAn);
                TempData["Success"] = "Đã gửi yêu cầu hoàn thành dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn
            });
        }
    }
}
