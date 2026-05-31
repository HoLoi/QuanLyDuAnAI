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
            string? locTheoNgay)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.CongViec.Xem))
                return Forbid();

            var vm = await _service.GetPageAsync(locMaDuAn, locTrangThai, tuKhoa, tuNgay, denNgay, locTheoNgay);
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
            string? locTheoNgay)
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

            return RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai, tuKhoa, tuNgay, denNgay, locTheoNgay });
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
            string? locTheoNgay)
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

            return RedirectToAction(nameof(Index), new { locMaDuAn, locTrangThai, tuKhoa, tuNgay, denNgay, locTheoNgay });
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(
            string? format,
            int? locMaDuAn,
            string? locTrangThai,
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile))
                return Forbid();

            var page = await _service.GetPageAsync(locMaDuAn, locTrangThai, tuKhoa, tuNgay, denNgay, locTheoNgay);
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
                    ("Lọc theo ngày", ExportSupport.ResolveTextOrDefault(locTheoNgay, "Ngày tạo")),
                    ("Từ ngày", ExportSupport.FormatDate(tuNgay)),
                    ("Đến ngày", ExportSupport.FormatDate(denNgay))),
                FileNamePrefix = "cong-viec",
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Mã công việc", ValueSelector = row => ((CongViecItemViewModel)row).MaCongViec.ToString() },
                    new() { Header = "Tên công việc", ValueSelector = row => ((CongViecItemViewModel)row).TenCongViec },
                    new() { Header = "Dự án", ValueSelector = row => ((CongViecItemViewModel)row).TenDuAn },
                    new() { Header = "Danh mục", ValueSelector = row => ((CongViecItemViewModel)row).TenDanhMucCV },
                    new() { Header = "Mức độ", ValueSelector = row => ((CongViecItemViewModel)row).TenMucDo },
                    new() { Header = "Ngày bắt đầu", ValueSelector = row => ExportSupport.FormatDate(((CongViecItemViewModel)row).NgayBatDauCongViec) },
                    new() { Header = "Hạn kết thúc", ValueSelector = row => ExportSupport.FormatDate(((CongViecItemViewModel)row).NgayKetThucCVDuKien) },
                    new() { Header = "Chi phí đã chi", ValueSelector = row => ExportSupport.FormatCurrency(((CongViecItemViewModel)row).ChiPhiDaChi) },
                    new() { Header = "Trạng thái", ValueSelector = row => ((CongViecItemViewModel)row).TrangThaiHienThi }
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
