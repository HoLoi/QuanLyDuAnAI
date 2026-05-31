using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Data;
using QuanLyDuAn.Services.Implementations;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.TaiKhoanCaNhan;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class TaiKhoanCaNhanController : Controller
    {
        private readonly ITaiKhoanCaNhanService _service;

        public TaiKhoanCaNhanController(QuanLyDuAnDbContext context, IWebHostEnvironment environment)
        {
            _service = new TaiKhoanCaNhanService(context, environment);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await _service.GetHoSoAsync(User);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CapNhat()
        {
            var vm = await _service.GetCapNhatAsync(User);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhat(CapNhatTaiKhoanCaNhanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var baseVm = await _service.GetCapNhatAsync(User);
                model.UserName = baseVm.UserName;
                model.Email = baseVm.Email;
                model.TenChucDanh = baseVm.TenChucDanh;
                model.VaiTroHeThong = baseVm.VaiTroHeThong;
                model.TeamHienTai = baseVm.TeamHienTai;
                model.AnhDaiDienHienTai = baseVm.AnhDaiDienHienTai;
                model.ChuCaiDau = baseVm.ChuCaiDau;
                return View(model);
            }

            try
            {
                await _service.CapNhatAsync(User, model);
                TempData["Success"] = "Đã cập nhật hồ sơ cá nhân.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var baseVm = await _service.GetCapNhatAsync(User);
                model.UserName = baseVm.UserName;
                model.Email = baseVm.Email;
                model.TenChucDanh = baseVm.TenChucDanh;
                model.VaiTroHeThong = baseVm.VaiTroHeThong;
                model.TeamHienTai = baseVm.TeamHienTai;
                model.AnhDaiDienHienTai = baseVm.AnhDaiDienHienTai;
                model.ChuCaiDau = baseVm.ChuCaiDau;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            return View(new DoiMatKhauViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _service.DoiMatKhauAsync(User, model);
                TempData["Success"] = "Đổi mật khẩu thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}
