using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
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
        private readonly IExportFileService _exportFileService;

        public DanhGiaNhanVienController(
            IDanhGiaNhanVienService service,
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
            int? maDuAn,
            int? maNhanVien,
            string? tuKhoa,
            string? trangThai,
            string? nguon,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.Xem))
            {
                return Forbid();
            }

            try
            {
                var vm = await _service.GetPageAsync(maDuAn, maNhanVien, tuKhoa, trangThai, tuNgayDanhGia, denNgayDanhGia);
                vm.Nguon = nguon;
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
                    TrangThai = trangThai,
                    Nguon = nguon,
                    TuNgayDanhGia = tuNgayDanhGia,
                    DenNgayDanhGia = denNgayDanhGia,
                    Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User)
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Form(
            int maDuAn,
            int maNhanVien,
            string? tuKhoa,
            string? trangThai,
            string? nguon,
            string? returnUrl,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.DanhGia, Permissions.DanhGiaNhanVien.Sua))
            {
                return Forbid();
            }

            try
            {
                var page = await _service.GetPageAsync(maDuAn, maNhanVien, tuKhoa, trangThai, tuNgayDanhGia, denNgayDanhGia);
                page.Form = await _service.GetFormAsync(maDuAn, maNhanVien);
                page.Nguon = nguon;
                page.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", page);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index), new { maDuAn, maNhanVien, tuKhoa, trangThai, nguon, tuNgayDanhGia, denNgayDanhGia });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Luu(
            DanhGiaNhanVienFormViewModel form,
            string? returnUrl,
            string? tuKhoa,
            string? trangThai,
            string? nguon,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
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
                return RedirectToAction(nameof(Form), new
                {
                    maDuAn = form.MaDuAn,
                    maNhanVien = form.MaNhanVien,
                    tuKhoa,
                    trangThai,
                    nguon,
                    returnUrl,
                    tuNgayDanhGia,
                    denNgayDanhGia
                });
            }

            try
            {
                await _service.LuuDanhGiaAsync(form);
                TempData["Success"] = "Đã lưu đánh giá nhân viên.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToReturnUrlOrIndex(returnUrl, new
            {
                maDuAn = form.MaDuAn,
                maNhanVien = form.MaNhanVien,
                tuKhoa,
                trangThai,
                nguon,
                tuNgayDanhGia,
                denNgayDanhGia
            });
        }

        [HttpPost]
        public async Task<IActionResult> GuiDuyet(
            int maDanhGiaNhanVien,
            int? maDuAn,
            int? maNhanVien,
            string? tuKhoa,
            string? trangThai,
            string? nguon,
            string? returnUrl,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.DanhGia, Permissions.DanhGiaNhanVien.Sua))
            {
                return Forbid();
            }

            try
            {
                await _service.GuiDuyetAsync(maDanhGiaNhanVien);
                TempData["Success"] = "Đã gửi duyệt đánh giá nhân viên.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToReturnUrlOrIndex(returnUrl, new { maDuAn, maNhanVien, tuKhoa, trangThai, nguon, tuNgayDanhGia, denNgayDanhGia });
        }

        [HttpPost]
        public async Task<IActionResult> Duyet(
            int maDanhGiaNhanVien,
            int? maDuAn,
            int? maNhanVien,
            string? tuKhoa,
            string? trangThai,
            string? nguon,
            string? returnUrl,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.Duyet))
            {
                return Forbid();
            }

            try
            {
                await _service.DuyetAsync(maDanhGiaNhanVien);
                TempData["Success"] = "Đã duyệt đánh giá nhân viên.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToReturnUrlOrIndex(returnUrl, new { maDuAn, maNhanVien, tuKhoa, trangThai, nguon, tuNgayDanhGia, denNgayDanhGia });
        }

        [HttpPost]
        public async Task<IActionResult> TuChoi(
            int maDanhGiaNhanVien,
            string lyDoTuChoi,
            int? maDuAn,
            int? maNhanVien,
            string? tuKhoa,
            string? trangThai,
            string? nguon,
            string? returnUrl,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DanhGiaNhanVien.Duyet))
            {
                return Forbid();
            }

            try
            {
                await _service.TuChoiAsync(maDanhGiaNhanVien, lyDoTuChoi);
                TempData["Success"] = "Đã từ chối đánh giá nhân viên.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToReturnUrlOrIndex(returnUrl, new { maDuAn, maNhanVien, tuKhoa, trangThai, nguon, tuNgayDanhGia, denNgayDanhGia });
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(
            string? format,
            int? maDuAn,
            int? maNhanVien,
            string? tuKhoa,
            string? trangThai,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile))
            {
                return Forbid();
            }

            var page = await _service.GetPageAsync(maDuAn, maNhanVien, tuKhoa, trangThai, tuNgayDanhGia, denNgayDanhGia);
            var rows = page.DanhSach.Cast<object>().ToList();

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "Báo cáo đánh giá nhân viên",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Từ khóa", tuKhoa),
                    ("Trạng thái đánh giá", trangThai),
                    ("Mã dự án", maDuAn?.ToString()),
                    ("Mã nhân viên", maNhanVien?.ToString()),
                    ("Từ ngày đánh giá", ExportSupport.FormatDate(tuNgayDanhGia)),
                    ("Đến ngày đánh giá", ExportSupport.FormatDate(denNgayDanhGia))),
                FileNamePrefix = "danh-gia-nhan-vien",
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Mã đánh giá", ValueSelector = row => ((DanhGiaNhanVienItemViewModel)row).CoDanhGia ? ((DanhGiaNhanVienItemViewModel)row).MaDanhGiaNhanVien.ToString() : "-" },
                    new() { Header = "Dự án", ValueSelector = row => ((DanhGiaNhanVienItemViewModel)row).TenDuAn },
                    new() { Header = "Nhân viên", ValueSelector = row => ((DanhGiaNhanVienItemViewModel)row).TenNhanVien },
                    new() { Header = "Vai trò", ValueSelector = row => ((DanhGiaNhanVienItemViewModel)row).VaiTroTrongDuAn },
                    new() { Header = "Trạng thái đánh giá", ValueSelector = row => string.Equals(((DanhGiaNhanVienItemViewModel)row).TrangThaiDanhGia, "ChuaDanhGia", StringComparison.OrdinalIgnoreCase) ? "Chưa đánh giá" : TrangThai.ToDisplay(((DanhGiaNhanVienItemViewModel)row).TrangThaiDanhGia) },
                    new() { Header = "Điểm tổng kết", ValueSelector = row => ((DanhGiaNhanVienItemViewModel)row).CoDanhGia ? ((DanhGiaNhanVienItemViewModel)row).DiemTongKet.ToString("0.##") : "-" },
                    new() { Header = "Xếp loại", ValueSelector = row => ((DanhGiaNhanVienItemViewModel)row).XepLoai },
                    new() { Header = "Tỷ lệ hoàn thành", ValueSelector = row => $"{((DanhGiaNhanVienItemViewModel)row).TyLeHoanThanh:0.##}%" },
                    new() { Header = "Ngày đánh giá", ValueSelector = row => ExportSupport.FormatDate(((DanhGiaNhanVienItemViewModel)row).NgayDanhGia) },
                    new() { Header = "Người đánh giá", ValueSelector = row => ((DanhGiaNhanVienItemViewModel)row).TenNguoiDanhGia }
                },
                Rows = rows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
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
