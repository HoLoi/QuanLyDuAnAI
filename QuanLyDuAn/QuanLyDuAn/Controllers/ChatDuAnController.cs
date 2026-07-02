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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiTinNhan(ChatDuAnGuiTinNhanViewModel form, int? maDuAn, string? tuKhoa)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.Chat.Gui))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                var message = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
                return BadRequest(new { message });
            }

            try
            {
                var tinNhan = await _service.GuiTinNhanAsync(form);
                return PartialView("_MessageItem", tinNhan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
                var batch = await _service.GetTinNhanBatchAsync(maPhongChat);
                return PartialView("_MessageBatch", batch);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Phong(int maPhongChat)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.Chat.Xem))
            {
                return Forbid();
            }

            try
            {
                var vm = await _service.GetPhongContentAsync(maPhongChat);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return PartialView("_ChatContent", vm);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PhongBatch(
            string? tuKhoa,
            int? lastRoomId,
            int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.Chat.Xem))
            {
                return Forbid();
            }

            try
            {
                var batch = await _service.GetPhongChatBatchAsync(
                    tuKhoa,
                    lastRoomId,
                    pageSize);
                return PartialView("_RoomBatch", batch);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TinNhanCu(
            int maPhongChat,
            DateTime cursorThoiGianGui,
            int cursorMaTinNhan,
            int pageSize = 30)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.Chat.Xem))
            {
                return Forbid();
            }

            try
            {
                var batch = await _service.GetTinNhanBatchAsync(
                    maPhongChat,
                    cursorThoiGianGui,
                    cursorMaTinNhan,
                    pageSize);
                return PartialView("_MessageBatch", batch);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TinNhanMoi(
            int maPhongChat,
            int afterMessageId,
            int pageSize = 50)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.Chat.Xem))
            {
                return Forbid();
            }

            try
            {
                return Json(await _service.GetTinNhanMoiAsync(
                    maPhongChat,
                    afterMessageId,
                    pageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
