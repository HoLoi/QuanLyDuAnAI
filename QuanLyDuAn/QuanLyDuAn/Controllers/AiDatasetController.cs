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
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile))
            {
                return Forbid();
            }

            var rows = await _aiDatasetService.LayDatasetHopLeDeTrainAsync(cancellationToken);
            var exportRows = rows.Cast<object>().ToList();

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "AI Dataset hợp lệ để huấn luyện",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Chỉ hiển thị thiếu dataset", thieuDataset ? "Có" : "Không"),
                    ("Điều kiện", "LaDuAnTre=1 và có MaDMNguyenNhan")),
                FileNamePrefix = "ai-dataset",
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Mã dự án", ValueSelector = row => ((AiDatasetRowViewModel)row).MaDuAn ?? string.Empty },
                    new() { Header = "Số nhân viên", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoNhanVienDuAn) },
                    new() { Header = "Tổng số công việc", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).TongSoCongViec) },
                    new() { Header = "Số công việc trễ", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoCongViecTre) },
                    new() { Header = "Tỷ lệ công việc trễ", ValueSelector = row => $"{ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).TyLeCongViecTre)}%" },
                    new() { Header = "Chi phí dự kiến", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).ChiPhiDuKien) },
                    new() { Header = "Chi phí thực tế", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).ChiPhiThucTe) },
                    new() { Header = "Chênh lệch chi phí", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).ChenhLechChiPhi) },
                    new() { Header = "Số lần thay đổi nhân sự", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoLanThayDoiNhanSu) },
                    new() { Header = "Số lần thay đổi quản lý", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoLanThayDoiQuanLy) },
                    new() { Header = "Số ngày trễ", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoNgayTreTienDo) },
                    new() { Header = "ĐX công việc chờ duyệt", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoDeXuatCongViecChoDuyet) },
                    new() { Header = "ĐX công việc bị từ chối", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoDeXuatCongViecBiTuChoi) },
                    new() { Header = "TG duyệt công việc TB (ngày)", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).ThoiGianDuyetCongViecTrungBinh) },
                    new() { Header = "ĐX ngân sách chờ duyệt", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoDeXuatNganSachChoDuyet) },
                    new() { Header = "ĐX ngân sách bị từ chối", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoDeXuatNganSachBiTuChoi) },
                    new() { Header = "TG duyệt ngân sách TB (ngày)", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).ThoiGianDuyetNganSachTrungBinh) },
                    new() { Header = "BC tiến độ chờ duyệt", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoBaoCaoTienDoChoDuyet) },
                    new() { Header = "BC tiến độ bị từ chối", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoBaoCaoTienDoBiTuChoi) },
                    new() { Header = "BC tiến độ yêu cầu bổ sung", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoBaoCaoTienDoYeuCauBoSung) },
                    new() { Header = "Tỷ lệ BC tiến độ bị từ chối", ValueSelector = row => $"{ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).TyLeBaoCaoTienDoBiTuChoi)}%" },
                    new() { Header = "Số lần cập nhật tiến độ", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoLanCapNhatTienDo) },
                    new() { Header = "Số ngày chậm cập nhật tiến độ", ValueSelector = row => ExportSupport.FormatNumber(((AiDatasetRowViewModel)row).SoNgayChamCapNhatTienDo) },
                    new() { Header = "Là dự án trễ", ValueSelector = row => ((AiDatasetRowViewModel)row).LaDuAnTre == 1 ? "Có" : "Không" },
                    new() { Header = "Mã nguyên nhân", ValueSelector = row => ((AiDatasetRowViewModel)row).MaDMNguyenNhan?.ToString() ?? string.Empty }
                },
                Rows = exportRows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
        }
    }
}
