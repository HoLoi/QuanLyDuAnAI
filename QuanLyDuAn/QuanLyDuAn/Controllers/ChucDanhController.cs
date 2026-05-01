using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.ChucDanh;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class ChucDanhController : Controller
    {
        private readonly IChucDanhService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public ChucDanhController(
            IChucDanhService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ChucDanh.Xem))
                return Forbid();

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

            var vm = new ChucDanhPageViewModel
            {
                DanhSach = await _service.GetAllAsync(),
                Form = new(),
                Permissions = permissions
            };

            return View(vm);
        }

        public async Task<IActionResult> Sua(int id)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ChucDanh.Sua))
                return Forbid();

            var form = await _service.GetByIdAsync(id);

            if (form == null)
            {
                TempData["Error"] = "Không tìm thấy chức danh";
                return RedirectToAction("Index");
            }

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

            var vm = new ChucDanhPageViewModel
            {
                DanhSach = await _service.GetAllAsync(),
                Form = form,
                Permissions = permissions
            };

            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> LuuChucDanh(ChucDanhPageViewModel vm)
        {
            var model = vm.Form;

            if (!ModelState.IsValid)
            {
                vm.DanhSach = await _service.GetAllAsync();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }

            if (model.MaChucDanh == null)
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.ChucDanh.Them))
                    return Forbid();
            }
            else
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.ChucDanh.Sua))
                    return Forbid();
            }

            try
            {
                await _service.SaveAsync(model);
                TempData["Success"] = "Đã lưu chức danh";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form.TenChucDanh", ex.Message);

                vm.DanhSach = await _service.GetAllAsync();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

                return View("Index", vm);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> XoaChucDanh(int maChucDanh)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ChucDanh.Xoa))
                return Forbid();

            try
            {
                await _service.DeleteAsync(maChucDanh);
                TempData["Success"] = "Đã xóa chức danh";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}