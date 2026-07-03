using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Account;
using QuanLyDuAn.ViewModels.Auth;

namespace QuanLyDuAn.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View(new DangNhapViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(DangNhapViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var principal = await _accountService.AuthenticateAsync(model.TenDangNhap, model.MatKhau);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.GhiNhoDangNhap,
                    AllowRefresh = true
                };

                if (model.GhiNhoDangNhap)
                {
                    authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);
                }

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Activate(string? userId, string? token)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Liên kết kích hoạt không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction(nameof(Login));
            }

            try
            {
                var model = await _accountService.TaoFormKichHoatAsync(userId, token);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Login));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(ActivateAccountViewModel model)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _accountService.KichHoatTaiKhoanAsync(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }

            TempData["Success"] = "Kích hoạt tài khoản thành công. Vui lòng đăng nhập bằng mật khẩu mới.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string maPhien;
            try
            {
                maPhien = await _accountService.KhoiTaoQuenMatKhauAsync(model.EmailHoacTenDangNhap);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }

            return RedirectToAction(
                nameof(VerifyOtp),
                new
                {
                    maPhien,
                    thongBao = "Nếu thông tin hợp lệ, hệ thống đã gửi mã OTP đến email đã đăng ký."
                });
        }

        [HttpGet]
        public IActionResult VerifyOtp(string? maPhien, string? thongBao = null)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrWhiteSpace(maPhien))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            return View(new VerifyOtpViewModel
            {
                MaPhien = maPhien.Trim(),
                ThongBao = thongBao
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var hopLe = await _accountService.XacNhanOtpDatLaiMatKhauAsync(model.MaPhien, model.MaOtp);
            if (!hopLe)
            {
                ModelState.AddModelError(string.Empty, "Mã OTP không hợp lệ hoặc đã hết hạn.");
                model.ThongBao = "Mã OTP có hiệu lực trong 3 phút.";
                return View(model);
            }

            return RedirectToAction(nameof(ResetPassword), new { maPhien = model.MaPhien });
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string? maPhien)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrWhiteSpace(maPhien))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            var hopLe = await _accountService.CoPhienDatLaiMatKhauHopLeAsync(maPhien);
            if (!hopLe)
            {
                TempData["Error"] = "Phiên đặt lại mật khẩu không hợp lệ hoặc đã hết hạn. Vui lòng thực hiện lại.";
                return RedirectToAction(nameof(ForgotPassword));
            }

            return View(new ResetPasswordViewModel
            {
                MaPhien = maPhien.Trim()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _accountService.DatLaiMatKhauBangOtpAsync(model.MaPhien, model.MatKhauMoi);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }

            TempData["Success"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập bằng mật khẩu mới.";
            return RedirectToAction(nameof(Login));
        }
    }
}
