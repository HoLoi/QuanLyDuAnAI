using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.ViewModels.PhanQuyen;
using Microsoft.AspNetCore.Authorization;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class PhanQuyenController : Controller
    {
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public PhanQuyenController(IPermissionHelper permission, IPhanQuyenService phanQuyenService)
        {
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? roleId, CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.PhanQuyen.Xem))
                return Forbid();

            var model = await _phanQuyenService.GetPageViewModelAsync(roleId, cancellationToken);
            model.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(PhanQuyenSaveInputViewModel model, CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.PhanQuyen.Luu))
                return Forbid();

            if (!ModelState.IsValid)
            {
                var invalidVm = await _phanQuyenService.GetPageViewModelAsync(model.SelectedRoleId, cancellationToken);
                ApplySelections(invalidVm, model.SelectedPermissionIds);
                invalidVm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", invalidVm);
            }

            try
            {
                await _phanQuyenService.SaveRolePermissionsAsync(model, cancellationToken);
                TempData["Success"] = "Đã cập nhật phân quyền vai trò.";
                return RedirectToAction(nameof(Index), new { roleId = model.SelectedRoleId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var failedVm = await _phanQuyenService.GetPageViewModelAsync(model.SelectedRoleId, cancellationToken);
                ApplySelections(failedVm, model.SelectedPermissionIds);
                failedVm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", failedVm);
            }
        }

        private static void ApplySelections(PhanQuyenPageViewModel pageModel, IEnumerable<int> selectedPermissionIds)
        {
            var selectedSet = selectedPermissionIds
                .Where(x => x > 0)
                .ToHashSet();

            foreach (var group in pageModel.PermissionGroups)
            {
                foreach (var permission in group.DanhSachQuyen)
                {
                    permission.IsSelected = selectedSet.Contains(permission.MaDanhMucQuyen);
                }
            }
        }
    }
}
