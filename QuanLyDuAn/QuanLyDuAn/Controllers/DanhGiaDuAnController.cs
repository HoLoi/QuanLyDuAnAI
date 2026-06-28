using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
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
        private readonly IExportFileService _exportFileService;

        public DanhGiaDuAnController(
            IDanhGiaDuAnService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService,
            IExportFileService exportFileService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
            _exportFileService = exportFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? trangThai,
            int? maDuAn,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia,
            int pageNumber = 1,
            int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.Xem))
            {
                return Forbid();
            }

            try
            {
                var vm = await _service.GetPageAsync(tuKhoa, trangThai, maDuAn, tuNgayDanhGia, denNgayDanhGia, pageNumber, pageSize);
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
                    TuNgayDanhGia = tuNgayDanhGia,
                    DenNgayDanhGia = denNgayDanhGia,
                    Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User)
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Form(
            int maDuAn,
            string? tuKhoa,
            string? trangThai,
            string? returnUrl,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.DanhGia, Permissions.DanhGiaDuAn.Sua))
            {
                return Forbid();
            }

            try
            {
                var page = await _service.GetPageAsync(tuKhoa, trangThai, maDuAn, tuNgayDanhGia, denNgayDanhGia);
                page.Form = await _service.GetFormAsync(maDuAn);

                page.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", page);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index), new { tuKhoa, trangThai, maDuAn, tuNgayDanhGia, denNgayDanhGia });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Luu(
            DanhGiaDuAnFormViewModel form,
            string? returnUrl,
            string? tuKhoa,
            string? trangThai,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
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
                return RedirectToAction(nameof(Form), new
                {
                    maDuAn = form.MaDuAn,
                    tuKhoa,
                    trangThai,
                    returnUrl,
                    tuNgayDanhGia,
                    denNgayDanhGia
                });
            }

            try
            {
                await _service.LuuDanhGiaAsync(form);
                TempData["Success"] = "Đã lưu đánh giá dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToReturnUrlOrIndex(returnUrl, new
            {
                maDuAn = form.MaDuAn,
                tuKhoa,
                trangThai,
                tuNgayDanhGia,
                denNgayDanhGia
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiDuyet(
            int maDanhGiaDuAn,
            int? maDuAn,
            string? tuKhoa,
            string? trangThai,
            string? returnUrl,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.DanhGia, Permissions.DanhGiaDuAn.Sua))
            {
                return Forbid();
            }

            try
            {
                await _service.GuiDuyetAsync(maDanhGiaDuAn);
                TempData["Success"] = "Đã gửi duyệt đánh giá dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToReturnUrlOrIndex(returnUrl, new { maDuAn, tuKhoa, trangThai, tuNgayDanhGia, denNgayDanhGia });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duyet(
            int maDanhGiaDuAn,
            int? maDuAn,
            string? tuKhoa,
            string? trangThai,
            string? returnUrl,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.Duyet))
            {
                return Forbid();
            }

            try
            {
                await _service.DuyetAsync(maDanhGiaDuAn);
                TempData["Success"] = "Đã duyệt đánh giá dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToReturnUrlOrIndex(returnUrl, new { maDuAn, tuKhoa, trangThai, tuNgayDanhGia, denNgayDanhGia });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoi(
            int maDanhGiaDuAn,
            string lyDoTuChoi,
            int? maDuAn,
            string? tuKhoa,
            string? trangThai,
            string? returnUrl,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.Duyet))
            {
                return Forbid();
            }

            try
            {
                await _service.TuChoiAsync(maDanhGiaDuAn, lyDoTuChoi);
                TempData["Success"] = "Đã từ chối đánh giá dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToReturnUrlOrIndex(returnUrl, new { maDuAn, tuKhoa, trangThai, tuNgayDanhGia, denNgayDanhGia });
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(
            string? format,
            string? tuKhoa,
            string? trangThai,
            int? maDuAn,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile))
            {
                return Forbid();
            }

            var page = await _service.GetPageAsync(tuKhoa, trangThai, maDuAn, tuNgayDanhGia, denNgayDanhGia, paginate: false);
            var rows = page.DanhSach.Cast<object>().ToList();

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "Báo cáo đánh giá dự án",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Từ khóa", tuKhoa),
                    ("Trạng thái đánh giá", trangThai),
                    ("Mã dự án", maDuAn?.ToString()),
                    ("Từ ngày đánh giá", ExportSupport.FormatDate(tuNgayDanhGia)),
                    ("Đến ngày đánh giá", ExportSupport.FormatDate(denNgayDanhGia))),
                FileNamePrefix = "danh-gia-du-an",
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Mã đánh giá", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).CoDanhGia ? ((DanhGiaDuAnItemViewModel)row).MaDanhGiaDuAn.ToString() : "-" },
                    new() { Header = "Dự án", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).TenDuAn },
                    new() { Header = "Người quản lý", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).TenNguoiQuanLy },
                    new() { Header = "Trạng thái dự án", ValueSelector = row => TrangThai.ToDisplay(((DanhGiaDuAnItemViewModel)row).TrangThaiDuAn) },
                    new() { Header = "Số công việc", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).TongCongViec.ToString() },
                    new() { Header = "Số công việc trễ", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).CongViecTreHan.ToString() },
                    new() { Header = "Trạng thái đánh giá", ValueSelector = row => string.Equals(((DanhGiaDuAnItemViewModel)row).TrangThaiDanhGia, "ChuaDanhGia", StringComparison.OrdinalIgnoreCase) ? "Chưa đánh giá" : TrangThai.ToDisplay(((DanhGiaDuAnItemViewModel)row).TrangThaiDanhGia) },
                    new() { Header = "Điểm tổng kết", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).CoDanhGia ? ((DanhGiaDuAnItemViewModel)row).DiemTongKet.ToString("0.##") : "-" },
                    new() { Header = "Xếp loại", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).XepLoai },
                    new() { Header = "Ngày đánh giá", ValueSelector = row => ExportSupport.FormatDateTime(((DanhGiaDuAnItemViewModel)row).NgayDanhGia) },
                    new() { Header = "Người đánh giá", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).TenNguoiDanhGia },
                    new() { Header = "Người duyệt", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).TenNguoiDuyet ?? string.Empty }
                },
                Rows = rows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
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

        private IActionResult RedirectToReturnUrlOrIndex(string? returnUrl, object fallbackRouteValues)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index), fallbackRouteValues);
        }
    }
}
