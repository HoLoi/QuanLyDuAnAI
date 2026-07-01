using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.CongViec;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class CongViecController : Controller
    {
        private readonly ICongViecService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;
        private readonly IExportFileService _exportFileService;

        public CongViecController(
            ICongViecService service,
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
            int? locMaDuAn,
            string? locTrangThai,
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan,
            bool? treHan,
            int pageNumber = 1,
            int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.CongViec.Xem))
                return Forbid();

            if (treHan == true && string.IsNullOrWhiteSpace(locTinhTrangThoiHan))
            {
                locTinhTrangThoiHan = CongViecDeadlineStatus.FilterQuaHan;
            }

            var vm = await _service.GetPageAsync(locMaDuAn, locTrangThai, tuKhoa, tuNgay, denNgay, locTheoNgay, locTinhTrangThoiHan, pageNumber, pageSize);
            vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanHoanThanh(
            int maCongViec,
            int? locMaDuAn,
            string? locTrangThai,
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await CoQuyenXuLyWorkflowCongViecAsync())
                return Forbid();

            try
            {
                await _service.XacNhanHoanThanhCongViecAsync(maCongViec);
                TempData["Success"] = "Đã xác nhận hoàn thành công việc.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai, tuKhoa, tuNgay, denNgay, locTheoNgay, locTinhTrangThoiHan });
        }

        [HttpPost]
        public async Task<IActionResult> MoLai(
            int maCongViec,
            string lyDo,
            int? locMaDuAn,
            string? locTrangThai,
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await CoQuyenXuLyWorkflowCongViecAsync())
                return Forbid();

            try
            {
                await _service.MoLaiCongViecAsync(maCongViec, lyDo);
                TempData["Success"] = "Đã mở lại công việc.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai, tuKhoa, tuNgay, denNgay, locTheoNgay, locTinhTrangThoiHan });
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(
            string? format,
            int? locMaDuAn,
            string? locTrangThai,
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan,
            bool? treHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile)
                || !await _permission.HasPermissionAsync(User, Permissions.CongViec.Xem))
                return Forbid();

            if (treHan == true && string.IsNullOrWhiteSpace(locTinhTrangThoiHan))
            {
                locTinhTrangThoiHan = CongViecDeadlineStatus.FilterQuaHan;
            }

            var locTinhTrangThoiHanResolved = CongViecDeadlineStatus.NormalizeFilter(locTinhTrangThoiHan);
            var page = await _service.GetPageAsync(locMaDuAn, locTrangThai, tuKhoa, tuNgay, denNgay, locTheoNgay, locTinhTrangThoiHanResolved, paginate: false);
            var rows = page.DanhSach.Cast<object>().ToList();

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "Danh sách công việc",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Từ khóa", tuKhoa),
                    ("Dự án", locMaDuAn?.ToString()),
                    ("Trạng thái", TrangThai.ToDisplay(locTrangThai)),
                    ("Tình trạng thời hạn", string.IsNullOrWhiteSpace(locTinhTrangThoiHanResolved)
                        ? "Tất cả"
                        : CongViecDeadlineStatus.ToDisplayFilter(locTinhTrangThoiHanResolved)),
                    ("Lọc theo ngày", ExportSupport.ToDisplayFilterValue(locTheoNgay, "Ngày tạo")),
                    ("Từ ngày", ExportSupport.FormatDate(tuNgay)),
                    ("Đến ngày", ExportSupport.FormatDate(denNgay))),
                FileNamePrefix = "DanhSachCongViec",
                SheetName = "CongViec",
                IncludeRowNumber = true,
                PdfLandscape = true,
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Tên công việc", ValueSelector = row => ((CongViecItemViewModel)row).TenCongViec, WrapText = true, MinWidth = 20, MaxWidth = 36, PdfRelativeWidth = 1.7f },
                    new() { Header = "Dự án", ValueSelector = row => ((CongViecItemViewModel)row).TenDuAn, WrapText = true, MinWidth = 18, MaxWidth = 30, PdfRelativeWidth = 1.4f },
                    new() { Header = "Danh mục", ValueSelector = row => ((CongViecItemViewModel)row).TenDanhMucCV, MinWidth = 14, MaxWidth = 24 },
                    new() { Header = "Mức độ ưu tiên", ValueSelector = row => ((CongViecItemViewModel)row).TenMucDo, Alignment = ExportColumnAlignment.Center, MinWidth = 12, MaxWidth = 18 },
                    new() { Header = "Ngày bắt đầu", ValueSelector = row => ((CongViecItemViewModel)row).NgayBatDauCongViec, NumberFormat = "dd/MM/yyyy HH:mm", Alignment = ExportColumnAlignment.Center, MinWidth = 16, MaxWidth = 19 },
                    new() { Header = "Hạn hoàn thành", ValueSelector = row => ((CongViecItemViewModel)row).NgayKetThucCVDuKien, NumberFormat = "dd/MM/yyyy HH:mm", Alignment = ExportColumnAlignment.Center, MinWidth = 16, MaxWidth = 19 },
                    new() { Header = "Hoàn thành thực tế", ValueSelector = row => ((CongViecItemViewModel)row).NgayKetThucCVThucTe, NumberFormat = "dd/MM/yyyy HH:mm", Alignment = ExportColumnAlignment.Center, MinWidth = 16, MaxWidth = 19 },
                    new() { Header = "Tình trạng thời hạn", ValueSelector = row => ((CongViecItemViewModel)row).TinhTrangThoiHan, Alignment = ExportColumnAlignment.Center, MinWidth = 16, MaxWidth = 22 },
                    new() { Header = "Số ngày trễ", ValueSelector = row => ((CongViecItemViewModel)row).SoNgayTre, NumberFormat = "0", Alignment = ExportColumnAlignment.Right, MinWidth = 10, MaxWidth = 13 },
                    new() { Header = "Chi phí đã chi", ValueSelector = row => ((CongViecItemViewModel)row).ChiPhiDaChi, NumberFormat = "#,##0 \"VNĐ\"", Alignment = ExportColumnAlignment.Right, MinWidth = 16, MaxWidth = 22 },
                    new() { Header = "Trạng thái", ValueSelector = row => ((CongViecItemViewModel)row).TrangThaiHienThi, Alignment = ExportColumnAlignment.Center, MinWidth = 14, MaxWidth = 20 },
                    new() { Header = "Mã công việc", ValueSelector = row => ((CongViecItemViewModel)row).MaCongViec, Alignment = ExportColumnAlignment.Center, MinWidth = 11, MaxWidth = 14, ShowInPdf = false }
                },
                Rows = rows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
        }

        private async Task<bool> CoQuyenXuLyWorkflowCongViecAsync()
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.CongViec.Xem))
                return false;

            return await _permission.HasPermissionAsync(User, Permissions.TienDo.Duyet)
                   || await _permission.HasPermissionAsync(User, Permissions.DuyetDeXuatCongViec.Duyet);
        }
    }
}
