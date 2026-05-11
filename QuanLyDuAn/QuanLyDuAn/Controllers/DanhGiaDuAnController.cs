using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DanhGiaDuAn;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class DanhGiaDuAnController : Controller
    {
        private readonly IDanhGiaDuAnService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public DanhGiaDuAnController(
            IDanhGiaDuAnService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? tuKhoa, string? trangThai, int? maDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.Xem))
            {
                return Forbid();
            }

            try
            {
                var vm = await _service.GetPageAsync(tuKhoa, trangThai, maDuAn);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new DanhGiaDuAnPageViewModel
                {
                    TuKhoa = tuKhoa,
                    TrangThai = trangThai,
                    MaDuAn = maDuAn,
                    Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User)
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Form(int maDuAn, string? tuKhoa, string? trangThai)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.DanhGia, Permissions.DanhGiaDuAn.Sua))
            {
                return Forbid();
            }

            try
            {
                var page = await _service.GetPageAsync(tuKhoa, trangThai, maDuAn);
                page.Form = await _service.GetFormAsync(maDuAn);
                page.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", page);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index), new { tuKhoa, trangThai, maDuAn });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Luu(DanhGiaDuAnFormViewModel form)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.DanhGia, Permissions.DanhGiaDuAn.Sua))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
                return RedirectToAction(nameof(Form), new { maDuAn = form.MaDuAn });
            }

            try
            {
                await _service.LuuDanhGiaAsync(form);
                TempData["Success"] = "Da luu danh gia du an.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maDuAn = form.MaDuAn });
        }

        [HttpPost]
        public async Task<IActionResult> GuiDuyet(int maDanhGiaDuAn, int? maDuAn, string? tuKhoa, string? trangThai)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.DanhGia, Permissions.DanhGiaDuAn.Sua))
            {
                return Forbid();
            }

            try
            {
                await _service.GuiDuyetAsync(maDanhGiaDuAn);
                TempData["Success"] = "Da gui duyet danh gia du an.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maDuAn, tuKhoa, trangThai });
        }

        [HttpPost]
        public async Task<IActionResult> Duyet(int maDanhGiaDuAn, int? maDuAn, string? tuKhoa, string? trangThai)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.Duyet))
            {
                return Forbid();
            }

            try
            {
                await _service.DuyetAsync(maDanhGiaDuAn);
                TempData["Success"] = "Da duyet danh gia du an.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maDuAn, tuKhoa, trangThai });
        }

        [HttpPost]
        public async Task<IActionResult> TuChoi(int maDanhGiaDuAn, string lyDoTuChoi, int? maDuAn, string? tuKhoa, string? trangThai)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.Duyet))
            {
                return Forbid();
            }

            try
            {
                await _service.TuChoiAsync(maDanhGiaDuAn, lyDoTuChoi);
                TempData["Success"] = "Da tu choi danh gia du an.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maDuAn, tuKhoa, trangThai });
        }

        [HttpGet]
        public async Task<IActionResult> ChiTiet(int maDanhGiaDuAn)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.Xem))
            {
                return Forbid();
            }

            try
            {
                var vm = await _service.GetChiTietAsync(maDanhGiaDuAn);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
