using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class DuyetDeXuatNganSachController : Controller
    {
        private readonly IDuyetDeXuatNganSachService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public DuyetDeXuatNganSachController(
            IDuyetDeXuatNganSachService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? locMaDuAn,
            string? locTrangThai,
            int? locMaNguoiDungDeXuat,
            DateTime? tuNgay,
            DateTime? denNgay,
            decimal? tuSoTien,
            decimal? denSoTien,
            string? tuKhoa,
            int pageNumber = 1,
            int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuyetNganSach.Xem))
                return Forbid();

            var vm = await _service.GetPageAsync(
                locMaDuAn,
                locTrangThai,
                locMaNguoiDungDeXuat,
                tuNgay,
                denNgay,
                tuSoTien,
                denSoTien,
                tuKhoa,
                pageNumber,
                pageSize);
            vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Duyet(
            int maDeXuatNs,
            int? locMaDuAn,
            string? locTrangThai,
            int? locMaNguoiDungDeXuat,
            DateTime? tuNgay,
            DateTime? denNgay,
            decimal? tuSoTien,
            decimal? denSoTien,
            string? tuKhoa,
            int pageNumber = 1,
            int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuyetNganSach.Duyet))
                return Forbid();

            try
            {
                await _service.ApproveAsync(maDeXuatNs);
                TempData["Success"] = "Đã duyệt đề xuất ngân sách.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                locMaDuAn,
                locTrangThai,
                locMaNguoiDungDeXuat,
                tuNgay,
                denNgay,
                tuSoTien,
                denSoTien,
                tuKhoa,
                pageNumber,
                pageSize
            });
        }

        [HttpPost]
        public async Task<IActionResult> TuChoi(
            int maDeXuatNs,
            int? locMaDuAn,
            string? locTrangThai,
            int? locMaNguoiDungDeXuat,
            DateTime? tuNgay,
            DateTime? denNgay,
            decimal? tuSoTien,
            decimal? denSoTien,
            string? tuKhoa,
            int pageNumber = 1,
            int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuyetNganSach.Duyet))
                return Forbid();

            try
            {
                await _service.RejectAsync(maDeXuatNs);
                TempData["Success"] = "Đã từ chối đề xuất ngân sách.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                locMaDuAn,
                locTrangThai,
                locMaNguoiDungDeXuat,
                tuNgay,
                denNgay,
                tuSoTien,
                denSoTien,
                tuKhoa,
                pageNumber,
                pageSize
            });
        }
    }
}
