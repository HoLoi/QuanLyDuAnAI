using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Ai;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class AiDatasetController : Controller
    {
        private readonly IAiDatasetService _aiDatasetService;
        private readonly IPermissionHelper _permission;
        private readonly IExportFileService _exportFileService;

        public AiDatasetController(
            IAiDatasetService aiDatasetService,
            IPermissionHelper permission,
            IExportFileService exportFileService)
        {
            _aiDatasetService = aiDatasetService;
            _permission = permission;
            _exportFileService = exportFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(bool thieuDataset = false, CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Dataset))
            {
                return Forbid();
            }

            var vm = await _aiDatasetService.KhoiTaoTrangDatasetAsync(
                thieuDataset,
                baoGomBaoCaoChatLuong: true,
                cancellationToken);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TongHopDataset(CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Dataset))
            {
                return Forbid();
            }

            var result = await _aiDatasetService.TongHopDatasetAsync(cancellationToken);
            TempData["Success"] = $"Đã tổng hợp dataset: xử lý {result.TongSoDuAnXuLy} dự án, tạo mới {result.SoDuAnTaoMoi}, cập nhật {result.SoDuAnCapNhat}.";
            if (result.ThongBao.Any())
            {
                TempData["Warning"] = string.Join(" ", result.ThongBao);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TongHopDatasetChoDuAn(int maDuAn, CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Dataset))
            {
                return Forbid();
            }

            var result = await _aiDatasetService.TongHopDatasetChoDuAnAsync(maDuAn, cancellationToken);
            if (result.SoDuAnTaoMoi > 0 || result.SoDuAnCapNhat > 0)
            {
                TempData["Success"] = $"Đã tổng hợp AI_DATASET cho dự án #{maDuAn}.";
            }
            else
            {
                TempData["Warning"] = result.ThongBao.FirstOrDefault() ?? $"Không thể tổng hợp dữ liệu cho dự án #{maDuAn}.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KiemTraChatLuongDataset(CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Dataset))
            {
                return Forbid();
            }

            var vm = await _aiDatasetService.KhoiTaoTrangDatasetAsync(
                dangThieuDataset: false,
                baoGomBaoCaoChatLuong: true,
                cancellationToken);
            vm.DaKiemTraChatLuongDataset = true;
            return View(nameof(Index), vm);
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(string? format, bool thieuDataset = false, CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile)
                || !await _permission.HasPermissionAsync(User, Permissions.AI.Dataset))
            {
                return Forbid();
            }

            if (_exportFileService.ParseFormat(format) == ExportFileFormat.Pdf)
            {
                return BadRequest("AI Dataset chỉ hỗ trợ xuất Excel và CSV.");
            }

            var rows = await _aiDatasetService.LayDatasetHopLeDeTrainAsync(cancellationToken);
            var exportRows = rows.Cast<object>().ToList();

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "AI Dataset hợp lệ để huấn luyện",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Điều kiện", "Dự án trễ và đã có nguyên nhân xác nhận")),
                FileNamePrefix = "AIDatasetHopLe",
                SheetName = "AIDataset",
                IncludeRowNumber = true,
                PdfLandscape = true,
                FreezeColumns = 2,
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Mã dự án", ValueSelector = row => ((AiDatasetRowViewModel)row).MaDuAn, Alignment = ExportColumnAlignment.Center, MinWidth = 10, MaxWidth = 13 },
                    new() { Header = "Số nhân viên", ValueSelector = row => ((AiDatasetRowViewModel)row).SoNhanVienDuAn, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "Tổng số công việc", ValueSelector = row => ((AiDatasetRowViewModel)row).TongSoCongViec, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "Số công việc trễ", ValueSelector = row => ((AiDatasetRowViewModel)row).SoCongViecTre, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "Tỷ lệ công việc trễ", ValueSelector = row => ((AiDatasetRowViewModel)row).TyLeCongViecTre, NumberFormat = "0.##\"%\"", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "Chi phí dự kiến", ValueSelector = row => ((AiDatasetRowViewModel)row).ChiPhiDuKien, NumberFormat = "#,##0 \"VNĐ\"", Alignment = ExportColumnAlignment.Right, MinWidth = 16, MaxWidth = 22 },
                    new() { Header = "Chi phí thực tế", ValueSelector = row => ((AiDatasetRowViewModel)row).ChiPhiThucTe, NumberFormat = "#,##0 \"VNĐ\"", Alignment = ExportColumnAlignment.Right, MinWidth = 16, MaxWidth = 22 },
                    new() { Header = "Chênh lệch chi phí", ValueSelector = row => ((AiDatasetRowViewModel)row).ChenhLechChiPhi, NumberFormat = "#,##0 \"VNĐ\"", Alignment = ExportColumnAlignment.Right, MinWidth = 16, MaxWidth = 22 },
                    new() { Header = "Số lần thay đổi nhân sự", ValueSelector = row => ((AiDatasetRowViewModel)row).SoLanThayDoiNhanSu, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "Số lần thay đổi quản lý", ValueSelector = row => ((AiDatasetRowViewModel)row).SoLanThayDoiQuanLy, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "Số ngày trễ", ValueSelector = row => ((AiDatasetRowViewModel)row).SoNgayTreTienDo, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "ĐX công việc chờ duyệt", ValueSelector = row => ((AiDatasetRowViewModel)row).SoDeXuatCongViecChoDuyet, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "ĐX công việc bị từ chối", ValueSelector = row => ((AiDatasetRowViewModel)row).SoDeXuatCongViecBiTuChoi, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "TG duyệt công việc TB (ngày)", ValueSelector = row => ((AiDatasetRowViewModel)row).ThoiGianDuyetCongViecTrungBinh, NumberFormat = "0.00", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "ĐX ngân sách chờ duyệt", ValueSelector = row => ((AiDatasetRowViewModel)row).SoDeXuatNganSachChoDuyet, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "ĐX ngân sách bị từ chối", ValueSelector = row => ((AiDatasetRowViewModel)row).SoDeXuatNganSachBiTuChoi, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "TG duyệt ngân sách TB (ngày)", ValueSelector = row => ((AiDatasetRowViewModel)row).ThoiGianDuyetNganSachTrungBinh, NumberFormat = "0.00", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "BC tiến độ chờ duyệt", ValueSelector = row => ((AiDatasetRowViewModel)row).SoBaoCaoTienDoChoDuyet, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "BC tiến độ bị từ chối", ValueSelector = row => ((AiDatasetRowViewModel)row).SoBaoCaoTienDoBiTuChoi, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "BC tiến độ yêu cầu bổ sung", ValueSelector = row => ((AiDatasetRowViewModel)row).SoBaoCaoTienDoYeuCauBoSung, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "Tỷ lệ BC tiến độ bị từ chối", ValueSelector = row => ((AiDatasetRowViewModel)row).TyLeBaoCaoTienDoBiTuChoi, NumberFormat = "0.##\"%\"", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "Số lần cập nhật tiến độ", ValueSelector = row => ((AiDatasetRowViewModel)row).SoLanCapNhatTienDo, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "Số ngày chậm cập nhật tiến độ", ValueSelector = row => ((AiDatasetRowViewModel)row).SoNgayChamCapNhatTienDo, NumberFormat = "0.##", Alignment = ExportColumnAlignment.Right },
                    new() { Header = "Là dự án trễ", ValueSelector = row => ((AiDatasetRowViewModel)row).LaDuAnTre == 1 ? "Có" : "Không", Alignment = ExportColumnAlignment.Center },
                    new() { Header = "Mã nguyên nhân", ValueSelector = row => ((AiDatasetRowViewModel)row).MaDMNguyenNhan, Alignment = ExportColumnAlignment.Center }
                },
                Rows = exportRows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
        }
    }
}
