using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DanhGiaNhanVien;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class DanhGiaNhanVienController : Controller
    {
        private readonly IDanhGiaNhanVienService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public DanhGiaNhanVienController(
            IDanhGiaNhanVienService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? maDuAn, int? maNhanVien, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.Xem))
            {
                return Forbid();
            }

            try
            {
                var vm = await _service.GetPageAsync(maDuAn, maNhanVien, tuKhoa);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new DanhGiaNhanVienPageViewModel
                {
                    MaDuAn = maDuAn,
                    MaNhanVien = maNhanVien,
                    TuKhoa = tuKhoa,
                    Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User)
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Form(int maDuAn, int maNhanVien, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.DanhGia, Permissions.DanhGiaNhanVien.Sua))
            {
                return Forbid();
            }

            try
            {
                var page = await _service.GetPageAsync(maDuAn, maNhanVien, tuKhoa);
                page.Form = await _service.GetFormAsync(maDuAn, maNhanVien);
                page.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", page);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index), new { maDuAn, maNhanVien, tuKhoa });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Luu(DanhGiaNhanVienFormViewModel form)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.DanhGia, Permissions.DanhGiaNhanVien.Sua))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
                return RedirectToAction(nameof(Form), new { maDuAn = form.MaDuAn, maNhanVien = form.MaNhanVien });
            }

            try
            {
                await _service.LuuDanhGiaAsync(form);
                TempData["Success"] = "Da luu danh gia nhan vien.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maDuAn = form.MaDuAn, maNhanVien = form.MaNhanVien });
        }

        [HttpPost]
        public async Task<IActionResult> GuiDuyet(int maDanhGiaNhanVien, int? maDuAn, int? maNhanVien, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.DanhGia, Permissions.DanhGiaNhanVien.Sua))
            {
                return Forbid();
            }

            try
            {
                await _service.GuiDuyetAsync(maDanhGiaNhanVien);
                TempData["Success"] = "Da gui duyet danh gia nhan vien.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maDuAn, maNhanVien, tuKhoa });
        }

        [HttpPost]
        public async Task<IActionResult> Duyet(int maDanhGiaNhanVien, int? maDuAn, int? maNhanVien, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.Duyet))
            {
                return Forbid();
            }

            try
            {
                await _service.DuyetAsync(maDanhGiaNhanVien);
                TempData["Success"] = "Da duyet danh gia nhan vien.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maDuAn, maNhanVien, tuKhoa });
        }

        [HttpPost]
        public async Task<IActionResult> TuChoi(int maDanhGiaNhanVien, string lyDoTuChoi, int? maDuAn, int? maNhanVien, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.Duyet))
            {
                return Forbid();
            }

            try
            {
                await _service.TuChoiAsync(maDanhGiaNhanVien, lyDoTuChoi);
                TempData["Success"] = "Da tu choi danh gia nhan vien.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maDuAn, maNhanVien, tuKhoa });
        }

        [HttpGet]
        public async Task<IActionResult> ChiTiet(int maDanhGiaNhanVien)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.Xem))
            {
                return Forbid();
            }

            try
            {
                var vm = await _service.GetChiTietAsync(maDanhGiaNhanVien);
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
