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
        private readonly IDanhMucCongViecScopeService _scopeService;

        public DanhMucCongViecController(
            IDanhMucCongViecService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService,
            IDanhMucCongViecScopeService scopeService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
            _scopeService = scopeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? tuKhoa, int? locMaDuAn, int pageNumber = 1, int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhMucCongViec.Xem))
                return Forbid();

            if (!locMaDuAn.HasValue)
            {
                TempData["Error"] = "Vui lòng chọn dự án để quản lý danh mục công việc.";
                return RedirectToAction("Index", "DuAn");
            }

            if (!await _scopeService.CanAccessProjectAsync(User, locMaDuAn.Value, Permissions.DanhMucCongViec.Xem))
                return Forbid();

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            var duAnOptions = await _service.GetDuAnOptionsAsync(User, Permissions.DanhMucCongViec.Xem);
            var selectedProject = duAnOptions.FirstOrDefault(x => x.MaDuAn == locMaDuAn.Value);

            if (selectedProject == null)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction("Index", "DuAn");
            }

            var paged = await _service.GetPagedAsync(User, tuKhoa, locMaDuAn.Value, pageNumber, pageSize);
            var canCreate = await _scopeService.CanAccessProjectAsync(User, selectedProject.MaDuAn, Permissions.DanhMucCongViec.Them);
            var canEdit = await _scopeService.CanAccessProjectAsync(User, selectedProject.MaDuAn, Permissions.DanhMucCongViec.Sua);
            var canDelete = await _scopeService.CanAccessProjectAsync(User, selectedProject.MaDuAn, Permissions.DanhMucCongViec.Xoa);

            var vm = new DanhMucCongViecPageViewModel
            {
                MaDuAn = selectedProject.MaDuAn,
                TenDuAn = selectedProject.TenDuAn,
                DanhSach = paged.Items,
                DanhSachDuAn = duAnOptions,
                Form = new DanhMucCongViecCreateUpdateViewModel
                {
                    MaDuAn = selectedProject.MaDuAn
                },
                TuKhoa = tuKhoa,
                LocMaDuAn = locMaDuAn,
                Pagination = paged.Pagination,
                Permissions = permissions,
                CoTheXemDanhMuc = true,
                CoTheThemDanhMuc = canCreate,
                CoTheSuaDanhMuc = canEdit,
                CoTheXoaDanhMuc = canDelete
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id, string? tuKhoa, int? locMaDuAn, int pageNumber = 1, int pageSize = 20)
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

            var maDuAnThucTe = form.MaDuAn;
            if (!maDuAnThucTe.HasValue || maDuAnThucTe.Value <= 0)
                return NotFound();

            if (locMaDuAn.Value != maDuAnThucTe.Value)
                return Forbid();

            if (!await _scopeService.CanAccessProjectAsync(User, maDuAnThucTe.Value, Permissions.DanhMucCongViec.Sua))
                return Forbid();

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            var duAnOptions = await _service.GetDuAnOptionsAsync(User, Permissions.DanhMucCongViec.Xem);
            var selectedProject = duAnOptions.FirstOrDefault(x => x.MaDuAn == maDuAnThucTe.Value);

            if (selectedProject == null)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction("Index", "DuAn");
            }

            form.MaDuAn = selectedProject.MaDuAn;
            var paged = await _service.GetPagedAsync(User, tuKhoa, maDuAnThucTe.Value, pageNumber, pageSize);
            var canCreate = await _scopeService.CanAccessProjectAsync(User, selectedProject.MaDuAn, Permissions.DanhMucCongViec.Them);
            var canDelete = await _scopeService.CanAccessProjectAsync(User, selectedProject.MaDuAn, Permissions.DanhMucCongViec.Xoa);

            var vm = new DanhMucCongViecPageViewModel
            {
                MaDuAn = selectedProject.MaDuAn,
                TenDuAn = selectedProject.TenDuAn,
                DanhSach = paged.Items,
                DanhSachDuAn = duAnOptions,
                Form = form,
                TuKhoa = tuKhoa,
                LocMaDuAn = maDuAnThucTe,
                Pagination = paged.Pagination,
                Permissions = permissions,
                CoTheXemDanhMuc = true,
                CoTheThemDanhMuc = canCreate,
                CoTheSuaDanhMuc = true,
                CoTheXoaDanhMuc = canDelete
            };

            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> LuuDanhMucCongViec(DanhMucCongViecPageViewModel vm)
        {
            var model = vm.Form;
            if (model == null)
                return BadRequest();

            var isCreate = model.MaDanhMucCV == null;
            var permission = isCreate ? Permissions.DanhMucCongViec.Them : Permissions.DanhMucCongViec.Sua;
            if (!await _permission.HasPermissionAsync(User, permission))
                return Forbid();

            int? originalMaDuAn = null;
            if (!isCreate)
            {
                var maDanhMucCv = model.MaDanhMucCV!.Value;
                originalMaDuAn = await _service.GetMaDuAnByDanhMucIdAsync(maDanhMucCv);
                if (!originalMaDuAn.HasValue)
                    return NotFound();

                if (vm.LocMaDuAn.HasValue && vm.LocMaDuAn.Value != originalMaDuAn.Value)
                    return Forbid();

                if (!await _scopeService.CanAccessProjectAsync(User, originalMaDuAn.Value, permission))
                    return Forbid();
            }

            var selectedMaDuAn = isCreate
                ? (vm.LocMaDuAn ?? model.MaDuAn)
                : (model.MaDuAn ?? originalMaDuAn);

            if (!selectedMaDuAn.HasValue)
            {
                TempData["Error"] = "Vui lòng chọn dự án để quản lý danh mục công việc.";
                return RedirectToAction("Index", "DuAn");
            }

            model.MaDuAn = selectedMaDuAn.Value;
            if (!await _scopeService.CanAccessProjectAsync(User, selectedMaDuAn.Value, permission))
                return Forbid();

            var duAnOptions = await _service.GetDuAnOptionsAsync(User, Permissions.DanhMucCongViec.Xem);
            var selectedProject = duAnOptions.FirstOrDefault(x => x.MaDuAn == selectedMaDuAn.Value);

            if (selectedProject == null)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                var paged = await _service.GetPagedAsync(User, vm.TuKhoa, selectedMaDuAn.Value);
                vm.MaDuAn = selectedProject.MaDuAn;
                vm.TenDuAn = selectedProject.TenDuAn;
                vm.DanhSach = paged.Items;
                vm.DanhSachDuAn = duAnOptions;
                vm.Pagination = paged.Pagination;
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                await GanCoQuyenThaoTacAsync(vm, selectedMaDuAn.Value);
                return View("Index", vm);
            }

            try
            {
                await _service.SaveAsync(User, model);
                TempData["Success"] = "Đã lưu danh mục công việc.";
                return RedirectToAction(nameof(Index), new
                {
                    tuKhoa = vm.TuKhoa,
                    locMaDuAn = selectedMaDuAn
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form.TenDanhMucCV", ex.Message);
                var paged = await _service.GetPagedAsync(User, vm.TuKhoa, selectedMaDuAn.Value);
                vm.MaDuAn = selectedProject.MaDuAn;
                vm.TenDuAn = selectedProject.TenDuAn;
                vm.DanhSach = paged.Items;
                vm.DanhSachDuAn = duAnOptions;
                vm.Pagination = paged.Pagination;
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                await GanCoQuyenThaoTacAsync(vm, selectedMaDuAn.Value);
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
                await _service.DeleteAsync(User, maDanhMucCV);
                TempData["Success"] = "Đã xóa danh mục công việc.";
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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

        private async Task GanCoQuyenThaoTacAsync(DanhMucCongViecPageViewModel vm, int maDuAn)
        {
            vm.CoTheXemDanhMuc = await _scopeService.CanAccessProjectAsync(User, maDuAn, Permissions.DanhMucCongViec.Xem);
            vm.CoTheThemDanhMuc = await _scopeService.CanAccessProjectAsync(User, maDuAn, Permissions.DanhMucCongViec.Them);
            vm.CoTheSuaDanhMuc = await _scopeService.CanAccessProjectAsync(User, maDuAn, Permissions.DanhMucCongViec.Sua);
            vm.CoTheXoaDanhMuc = await _scopeService.CanAccessProjectAsync(User, maDuAn, Permissions.DanhMucCongViec.Xoa);
        }
    }
}
