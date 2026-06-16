using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.ChiTietCongViec;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class ChiTietCongViecController : Controller
    {
        private readonly IChiTietCongViecService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;
        private readonly IExportFileService _exportFileService;

        public ChiTietCongViecController(
            IChiTietCongViecService service,
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
        public async Task<IActionResult> Index(int maCongViec, int pageNumber = 1, int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ChiTietCongViec.Xem))
                return Forbid();

            try
            {
                var vm = await _service.GetPageAsync(maCongViec, pageNumber, pageSize);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "CongViec");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Them(ChiTietCongViecCreateUpdateViewModel form)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ChiTietCongViec.Them))
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
                return await RenderIndexWithFormAsync(form.MaCongViec, form);
            }

            try
            {
                await _service.AddAsync(form);
                TempData["Success"] = "Đã thêm chi tiết công việc thành công.";
                return RedirectToAction(nameof(Index), new { maCongViec = form.MaCongViec });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return await RenderIndexWithFormAsync(form.MaCongViec, form);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Sua(ChiTietCongViecCreateUpdateViewModel form)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ChiTietCongViec.Sua))
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
                return await RenderIndexWithFormAsync(form.MaCongViec, form);
            }

            try
            {
                await _service.UpdateAsync(form);
                TempData["Success"] = "Đã cập nhật chi tiết công việc thành công.";
                return RedirectToAction(nameof(Index), new { maCongViec = form.MaCongViec });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return await RenderIndexWithFormAsync(form.MaCongViec, form);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int maCongViec, int maChiTietCv)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ChiTietCongViec.Xoa))
                return Forbid();

            try
            {
                await _service.RemoveAsync(maCongViec, maChiTietCv);
                TempData["Success"] = "Đã xóa chi tiết công việc thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { maCongViec });
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(string? format, int maCongViec)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile))
                return Forbid();

            var page = await _service.GetPageAsync(maCongViec, paginate: false);
            var rows = page.DanhSach.Cast<object>().ToList();

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = $"Danh sách chi tiết công việc #{maCongViec}",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Mã công việc", maCongViec.ToString()),
                    ("Tên công việc", page.CongViec.TenCongViec)),
                FileNamePrefix = "chi-tiet-cong-viec",
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Mã chi tiết", ValueSelector = row => ((ChiTietCongViecItemViewModel)row).MaChiTietCV.ToString() },
                    new() { Header = "Tên chi tiết", ValueSelector = row => ((ChiTietCongViecItemViewModel)row).TenCTCV },
                    new() { Header = "Nội dung", ValueSelector = row => ((ChiTietCongViecItemViewModel)row).NoiDungChiTietCV },
                    new() { Header = "Ngày bắt đầu", ValueSelector = row => ExportSupport.FormatDate(((ChiTietCongViecItemViewModel)row).NgayBatDauCTCV) },
                    new() { Header = "Ngày kết thúc", ValueSelector = row => ExportSupport.FormatDate(((ChiTietCongViecItemViewModel)row).NgayKetThucCTCV) },
                    new() { Header = "Trạng thái", ValueSelector = row => ((ChiTietCongViecItemViewModel)row).TrangThaiHienThi }
                },
                Rows = rows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
        }

        private async Task<IActionResult> RenderIndexWithFormAsync(int maCongViec, ChiTietCongViecCreateUpdateViewModel form)
        {
            try
            {
                var vm = await _service.GetPageAsync(maCongViec);
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                vm.Form = form;
                return View(nameof(Index), vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "CongViec");
            }
        }
    }
}
