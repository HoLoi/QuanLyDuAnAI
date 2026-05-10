using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.ChatDuAn;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class ChatDuAnController : Controller
    {
        private readonly IChatDuAnService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;

        public ChatDuAnController(
            IChatDuAnService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? maDuAn, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.Chat.Xem))
            {
                return Forbid();
            }

            try
            {
                var vm = await _service.GetPageAsync(maDuAn, tuKhoa);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new ChatDuAnPageViewModel
                {
                    TuKhoa = tuKhoa,
                    MaDuAnDangChon = maDuAn,
                    Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User)
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GuiTinNhan(ChatDuAnGuiTinNhanViewModel form, int? maDuAn, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.Chat.Gui))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

                return RedirectToAction(nameof(Index), new
                {
                    maDuAn = maDuAn ?? form.MaDuAn,
                    tuKhoa
                });
            }

            try
            {
                await _service.GuiTinNhanAsync(form);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                maDuAn = maDuAn ?? form.MaDuAn,
                tuKhoa
            });
        }

        [HttpGet]
        public async Task<IActionResult> TinNhan(int maPhongChat)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.Chat.Xem))
            {
                return Forbid();
            }

            try
            {
                var dsTinNhan = await _service.GetTinNhanAsync(maPhongChat);
                return PartialView("_MessageList", new ChatDuAnPageViewModel
                {
                    DanhSachTinNhan = dsTinNhan,
                    MaPhongChatDangChon = maPhongChat
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return PartialView("_MessageList", new ChatDuAnPageViewModel());
            }
        }
    }
}
