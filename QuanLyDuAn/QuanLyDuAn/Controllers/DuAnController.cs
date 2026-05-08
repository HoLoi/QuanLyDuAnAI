using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DuAn;
using System.Security.Claims;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class DuAnController : Controller
    {
        private readonly IDuAnService _service;
        private readonly IFileDuAnService _fileDuAnService;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public DuAnController(
            IDuAnService service,
            IFileDuAnService fileDuAnService,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _fileDuAnService = fileDuAnService;
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
        public async Task<IActionResult> Details(int id, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Xem))
                return Forbid();

            if (id <= 0)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction(nameof(Index), new
                {
                    tuKhoa,
                    locMaLoaiDuAn,
                    locTrangThaiDuAn
                });
            }

            var vm = await _service.GetChiTietAsync(id);
            if (vm == null)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction(nameof(Index), new
                {
                    tuKhoa,
                    locMaLoaiDuAn,
                    locTrangThaiDuAn
                });
            }

            vm.TuKhoa = tuKhoa;
            vm.LocMaLoaiDuAn = locMaLoaiDuAn;
            vm.LocTrangThaiDuAn = locTrangThaiDuAn;
            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            vm.Permissions = permissions;

            var currentUserId = TryGetCurrentUserId();
            vm.CoTheQuanLyFile = currentUserId.HasValue
                                 && currentUserId.Value == vm.MaNguoiDung
                                 && permissions.Contains(Permissions.DuAn.Sua);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            return await Details(id, tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn);
        }

        [HttpPost]
        public async Task<IActionResult> ThemFileDuAn(int maDuAn, IFormFile file, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                var currentUserId = GetCurrentUserId();
                await _fileDuAnService.UploadAsync(maDuAn, file, currentUserId);
                TempData["Success"] = "Đã tải lên tệp dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new
            {
                id = maDuAn,
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn
            });
        }

        [HttpPost]
        public async Task<IActionResult> XoaFileDuAn(int maFileDa, int maDuAn, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                var currentUserId = GetCurrentUserId();
                await _fileDuAnService.DeleteAsync(maFileDa, currentUserId);
                TempData["Success"] = "Đã xóa tệp dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new
            {
                id = maDuAn,
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn
            });
        }

        [HttpGet]
        public async Task<IActionResult> TaiFileDuAn(int maFileDa, int maDuAn, string? tuKhoa, int? locMaLoaiDuAn, string? locTrangThaiDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Xem))
                return Forbid();

            try
            {
                var (fullPath, fileName, projectId) = await _fileDuAnService.GetDownloadInfoAsync(maFileDa);
                if (projectId != maDuAn)
                    throw new Exception("Tệp không thuộc dự án hiện tại.");

                return PhysicalFile(fullPath, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new
                {
                    id = maDuAn,
                    tuKhoa,
                    locMaLoaiDuAn,
                    locTrangThaiDuAn
                });
            }
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

        private int? TryGetCurrentUserId()
        {
            var claimValue = User.FindFirstValue("MaNguoiDung");
            if (int.TryParse(claimValue, out var currentUserId) && currentUserId > 0)
            {
                return currentUserId;
            }

            return null;
        }

        private int GetCurrentUserId()
        {
            var currentUserId = TryGetCurrentUserId();
            if (!currentUserId.HasValue)
                throw new Exception("Khong xac dinh duoc nguoi dung hien tai.");

            return currentUserId.Value;
        }

    }
}
