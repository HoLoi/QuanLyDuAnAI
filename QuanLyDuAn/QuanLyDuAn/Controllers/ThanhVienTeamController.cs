using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.ThanhVienTeam;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class ThanhVienTeamController : Controller
    {
        private readonly IThanhVienTeamService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public ThanhVienTeamController(
            IThanhVienTeamService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? tuKhoa, int? locMaTeam, string? locVaiTroLanhDao, bool? cheDoGanTruongNhom)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThanhVienNhom.Xem))
                return Forbid();

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            var locLeader = ParseLeaderFilter(locVaiTroLanhDao);

            var vm = new ThanhVienTeamPageViewModel
            {
                DanhSach = await _service.GetAllAsync(tuKhoa, locMaTeam, locLeader),
                Form = new ThanhVienTeamCreateUpdateViewModel
                {
                    MaTeam = locMaTeam
                },
                DanhSachTeam = await _service.GetTeamOptionsAsync(),
                DanhSachNhanSu = await _service.GetNhanSuOptionsAsync(),
                TuKhoa = tuKhoa,
                LocMaTeam = locMaTeam,
                LocVaiTroLanhDao = locVaiTroLanhDao,
                CheDoGanTruongNhom = cheDoGanTruongNhom == true,
                Permissions = permissions
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int maTeam, int maNguoiDung, string? tuKhoa, int? locMaTeam, string? locVaiTroLanhDao, bool? cheDoGanTruongNhom)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThanhVienNhom.Them))
                return Forbid();

            var form = await _service.GetByIdAsync(maTeam, maNguoiDung);
            if (form == null)
            {
                TempData["Error"] = "Không tìm thấy thành viên team.";
                return RedirectToAction(nameof(Index));
            }

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            var locLeader = ParseLeaderFilter(locVaiTroLanhDao);

            var vm = new ThanhVienTeamPageViewModel
            {
                DanhSach = await _service.GetAllAsync(tuKhoa, locMaTeam, locLeader),
                Form = form,
                DanhSachTeam = await _service.GetTeamOptionsAsync(),
                DanhSachNhanSu = await _service.GetNhanSuOptionsAsync(),
                TuKhoa = tuKhoa,
                LocMaTeam = locMaTeam,
                LocVaiTroLanhDao = locVaiTroLanhDao,
                CheDoGanTruongNhom = cheDoGanTruongNhom == true,
                Permissions = permissions
            };

            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> LuuThanhVienTeam(ThanhVienTeamPageViewModel vm)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThanhVienNhom.Them))
                return Forbid();

            if (!ModelState.IsValid)
            {
                var locLeaderInvalid = ParseLeaderFilter(vm.LocVaiTroLanhDao);
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, vm.LocMaTeam, locLeaderInvalid);
                vm.DanhSachTeam = await _service.GetTeamOptionsAsync();
                vm.DanhSachNhanSu = await _service.GetNhanSuOptionsAsync();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }

            try
            {
                await _service.SaveAsync(vm.Form);
                TempData["Success"] = "Đã lưu thành viên team.";
                return RedirectToAction(nameof(Index), new
                {
                    tuKhoa = vm.TuKhoa,
                    locMaTeam = vm.Form.MaTeam ?? vm.LocMaTeam,
                    locVaiTroLanhDao = vm.LocVaiTroLanhDao,
                    cheDoGanTruongNhom = vm.CheDoGanTruongNhom
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form.VaiTroTrongTeam", ex.Message);
                var locLeaderFailed = ParseLeaderFilter(vm.LocVaiTroLanhDao);
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, vm.LocMaTeam, locLeaderFailed);
                vm.DanhSachTeam = await _service.GetTeamOptionsAsync();
                vm.DanhSachNhanSu = await _service.GetNhanSuOptionsAsync();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaThanhVienTeam(
            int maTeam,
            int maNguoiDung,
            string? tuKhoa,
            int? locMaTeam,
            string? locVaiTroLanhDao,
            bool? cheDoGanTruongNhom)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThanhVienNhom.Xoa))
                return Forbid();

            try
            {
                await _service.DeleteAsync(maTeam, maNguoiDung);
                TempData["Success"] = "Đã xóa thành viên khỏi team.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaTeam = locMaTeam ?? maTeam,
                locVaiTroLanhDao,
                cheDoGanTruongNhom
            });
        }

        [HttpPost]
        public async Task<IActionResult> GanTruongNhom(
            int maTeam,
            int maNguoiDung,
            string? tuKhoa,
            int? locMaTeam,
            string? locVaiTroLanhDao,
            bool? cheDoGanTruongNhom)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThanhVienNhom.Them))
                return Forbid();

            try
            {
                await _service.SetLeaderAsync(maTeam, maNguoiDung);
                TempData["Success"] = "Đã gán trưởng nhóm thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaTeam = locMaTeam ?? maTeam,
                locVaiTroLanhDao,
                cheDoGanTruongNhom = true
            });
        }

        private static bool? ParseLeaderFilter(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim().ToLowerInvariant() switch
            {
                "truongnhom" => true,
                "thanhvien" => false,
                _ => null
            };
        }
    }
}
