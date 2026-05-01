using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DanhMucCongViec;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class DanhMucCongViecController : Controller
    {
        private readonly IDanhMucCongViecService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public DanhMucCongViecController(
            IDanhMucCongViecService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? tuKhoa, int? locMaDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhMucCongViec.Xem))
                return Forbid();

            if (!locMaDuAn.HasValue)
            {
                TempData["Error"] = "Vui lòng chọn dự án để quản lý danh mục công việc.";
                return RedirectToAction("Index", "DuAn");
            }

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            var duAnOptions = await _service.GetDuAnOptionsAsync();
            var selectedProject = duAnOptions.FirstOrDefault(x => x.MaDuAn == locMaDuAn.Value);

            if (selectedProject == null)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction("Index", "DuAn");
            }

            var vm = new DanhMucCongViecPageViewModel
            {
                MaDuAn = selectedProject.MaDuAn,
                TenDuAn = selectedProject.TenDuAn,
                DanhSach = await _service.GetAllAsync(tuKhoa, locMaDuAn),
                DanhSachDuAn = duAnOptions,
                Form = new DanhMucCongViecCreateUpdateViewModel
                {
                    MaDuAn = selectedProject.MaDuAn
                },
                TuKhoa = tuKhoa,
                LocMaDuAn = locMaDuAn,
                Permissions = permissions
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id, string? tuKhoa, int? locMaDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhMucCongViec.Sua))
                return Forbid();

            if (!locMaDuAn.HasValue)
            {
                TempData["Error"] = "Vui lòng chọn dự án để quản lý danh mục công việc.";
                return RedirectToAction("Index", "DuAn");
            }

            var form = await _service.GetByIdAsync(id);
            if (form == null)
            {
                TempData["Error"] = "Không tìm thấy danh mục công việc.";
                return RedirectToAction(nameof(Index), new { tuKhoa, locMaDuAn });
            }

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            var duAnOptions = await _service.GetDuAnOptionsAsync();
            var selectedProject = duAnOptions.FirstOrDefault(x => x.MaDuAn == locMaDuAn.Value);

            if (selectedProject == null)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction("Index", "DuAn");
            }

            form.MaDuAn = selectedProject.MaDuAn;

            var vm = new DanhMucCongViecPageViewModel
            {
                MaDuAn = selectedProject.MaDuAn,
                TenDuAn = selectedProject.TenDuAn,
                DanhSach = await _service.GetAllAsync(tuKhoa, locMaDuAn),
                DanhSachDuAn = duAnOptions,
                Form = form,
                TuKhoa = tuKhoa,
                LocMaDuAn = locMaDuAn,
                Permissions = permissions
            };

            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> LuuDanhMucCongViec(DanhMucCongViecPageViewModel vm)
        {
            var model = vm.Form;

            var selectedMaDuAn = vm.LocMaDuAn ?? model.MaDuAn;
            if (!selectedMaDuAn.HasValue)
            {
                TempData["Error"] = "Vui lòng chọn dự án để quản lý danh mục công việc.";
                return RedirectToAction("Index", "DuAn");
            }

            model.MaDuAn = selectedMaDuAn.Value;
            var duAnOptions = await _service.GetDuAnOptionsAsync();
            var selectedProject = duAnOptions.FirstOrDefault(x => x.MaDuAn == selectedMaDuAn.Value);

            if (selectedProject == null)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction("Index", "DuAn");
            }

            if (!ModelState.IsValid)
            {
                vm.MaDuAn = selectedProject.MaDuAn;
                vm.TenDuAn = selectedProject.TenDuAn;
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, selectedMaDuAn);
                vm.DanhSachDuAn = duAnOptions;
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }

            if (model.MaDanhMucCV == null)
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.DanhMucCongViec.Them))
                    return Forbid();
            }
            else
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.DanhMucCongViec.Sua))
                    return Forbid();
            }

            try
            {
                await _service.SaveAsync(model);
                TempData["Success"] = "Đã lưu danh mục công việc.";
                return RedirectToAction(nameof(Index), new
                {
                    tuKhoa = vm.TuKhoa,
                    locMaDuAn = selectedMaDuAn
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form.TenDanhMucCV", ex.Message);
                vm.MaDuAn = selectedProject.MaDuAn;
                vm.TenDuAn = selectedProject.TenDuAn;
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, selectedMaDuAn);
                vm.DanhSachDuAn = duAnOptions;
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaDanhMucCongViec(int maDanhMucCV, string? tuKhoa, int? locMaDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhMucCongViec.Xoa))
                return Forbid();

            try
            {
                await _service.DeleteAsync(maDanhMucCV);
                TempData["Success"] = "Đã xóa danh mục công việc.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaDuAn
            });
        }
    }
}
