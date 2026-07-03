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
        public async Task<IActionResult> XacNhan(
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
                await _service.XacNhanAsync(maDanhGiaDuAn);
                TempData["Success"] = "Đã xác nhận đánh giá dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToReturnUrlOrIndex(returnUrl, new { maDuAn, tuKhoa, trangThai, tuNgayDanhGia, denNgayDanhGia });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuiDuyet(
            int maDanhGiaDuAn,
            int? maDuAn,
            string? tuKhoa,
            string? trangThai,
            string? returnUrl,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            TempData["Error"] = "Luồng gửi duyệt đã được thay bằng xác nhận đánh giá.";
            return RedirectToReturnUrlOrIndex(returnUrl, new { maDuAn, tuKhoa, trangThai, tuNgayDanhGia, denNgayDanhGia });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Duyet(
            int maDanhGiaDuAn,
            int? maDuAn,
            string? tuKhoa,
            string? trangThai,
            string? returnUrl,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            TempData["Error"] = "Luồng duyệt bởi Admin đã được thay bằng xác nhận đánh giá của Manager.";
            return RedirectToReturnUrlOrIndex(returnUrl, new { maDuAn, tuKhoa, trangThai, tuNgayDanhGia, denNgayDanhGia });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TuChoi(
            int maDanhGiaDuAn,
            string lyDoTuChoi,
            int? maDuAn,
            string? tuKhoa,
            string? trangThai,
            string? returnUrl,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            TempData["Error"] = "Luồng từ chối bởi Admin đã được thay bằng xác nhận đánh giá của Manager.";
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
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile)
                || !await _permission.HasPermissionAsync(User, Permissions.DanhGiaDuAn.Xem))
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
                FileNamePrefix = "DanhGiaDuAn",
                SheetName = "DanhGiaDuAn",
                IncludeRowNumber = true,
                PdfLandscape = true,
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Dự án", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).TenDuAn, WrapText = true, MinWidth = 20, MaxWidth = 34, PdfRelativeWidth = 1.6f },
                    new() { Header = "Người quản lý", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).TenNguoiQuanLy, MinWidth = 16, MaxWidth = 24 },
                    new() { Header = "Trạng thái dự án", ValueSelector = row => TrangThai.ToDisplay(((DanhGiaDuAnItemViewModel)row).TrangThaiDuAn), Alignment = ExportColumnAlignment.Center, MinWidth = 15, MaxWidth = 21 },
                    new() { Header = "Số công việc", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).TongCongViec, NumberFormat = "0", Alignment = ExportColumnAlignment.Right, MinWidth = 10, MaxWidth = 13, ShowInPdf = false },
                    new() { Header = "Số công việc trễ", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).CongViecTreHan, NumberFormat = "0", Alignment = ExportColumnAlignment.Right, MinWidth = 12, MaxWidth = 15, ShowInPdf = false },
                    new() { Header = "Trạng thái đánh giá", ValueSelector = row => ToDisplayTrangThaiDanhGia(((DanhGiaDuAnItemViewModel)row).TrangThaiDanhGia), Alignment = ExportColumnAlignment.Center, MinWidth = 16, MaxWidth = 22 },
                    new() { Header = "Điểm tổng kết", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).CoDanhGia ? ((DanhGiaDuAnItemViewModel)row).DiemTongKet : null, NumberFormat = "0.00", Alignment = ExportColumnAlignment.Right, MinWidth = 11, MaxWidth = 14 },
                    new() { Header = "Xếp loại", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).CoDanhGia ? ((DanhGiaDuAnItemViewModel)row).XepLoai : "Chưa đánh giá", Alignment = ExportColumnAlignment.Center, MinWidth = 12, MaxWidth = 18 },
                    new() { Header = "Ngày đánh giá", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).NgayDanhGia, NumberFormat = "dd/MM/yyyy HH:mm", Alignment = ExportColumnAlignment.Center, MinWidth = 16, MaxWidth = 19 },
                    new() { Header = "Người đánh giá", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).TenNguoiDanhGia, MinWidth = 16, MaxWidth = 24 },
                    new() { Header = "Người xác nhận", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).TenNguoiDuyet, MinWidth = 16, MaxWidth = 24 },
                    new() { Header = "Mã đánh giá", ValueSelector = row => ((DanhGiaDuAnItemViewModel)row).CoDanhGia ? ((DanhGiaDuAnItemViewModel)row).MaDanhGiaDuAn : null, Alignment = ExportColumnAlignment.Center, MinWidth = 11, MaxWidth = 14, ShowInPdf = false }
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

        private static string ToDisplayTrangThaiDanhGia(string? trangThai)
        {
            if (string.Equals(trangThai, "ChuaDanhGia", StringComparison.OrdinalIgnoreCase)) return "Chưa đánh giá";
            if (string.Equals(trangThai, TrangThai.Nhap, StringComparison.OrdinalIgnoreCase)) return "Nháp";
            if (string.Equals(trangThai, TrangThai.DaDuyet, StringComparison.OrdinalIgnoreCase)) return "Đã xác nhận";
            if (string.Equals(trangThai, TrangThai.ChoDuyet, StringComparison.OrdinalIgnoreCase)) return "Chờ xác nhận cũ";
            if (string.Equals(trangThai, TrangThai.TuChoi, StringComparison.OrdinalIgnoreCase)) return "Từ chối cũ";
            return trangThai ?? string.Empty;
        }
    }
}
