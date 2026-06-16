using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Ai;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class AiController : Controller
    {
        private readonly IAiService _aiService;
        private readonly IPermissionHelper _permission;
        private readonly IExportFileService _exportFileService;
        private readonly ILogger<AiController> _logger;

        public AiController(
            IAiService aiService,
            IPermissionHelper permission,
            IExportFileService exportFileService,
            ILogger<AiController> logger)
        {
            _aiService = aiService;
            _permission = permission;
            _exportFileService = exportFileService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Dashboard))
            {
                return Forbid();
            }

            var vm = await _aiService.LayDashboardAsync(cancellationToken);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(string? format, CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Dashboard)
                || !await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile))
            {
                return Forbid();
            }

            var vm = await _aiService.LayDashboardAsync(cancellationToken);
            var rows = BuildDashboardExportRows(vm);
            var request = new ExportFileRequest
            {
                ReportTitle = "Báo cáo tổng quan AI",
                ExportedBy = User?.Identity?.Name,
                AppliedFiltersText = "Dữ liệu theo phạm vi quyền xem hiện tại.",
                Format = ParseExportFormat(format),
                FileNamePrefix = "bao-cao-ai-dashboard",
                Rows = rows,
                Columns =
                [
                    new ExportColumnDefinition { Header = "Nhóm", ValueSelector = x => ((AiDashboardExportRow)x).Nhom },
                    new ExportColumnDefinition { Header = "Chỉ tiêu", ValueSelector = x => ((AiDashboardExportRow)x).ChiTieu },
                    new ExportColumnDefinition { Header = "Giá trị", ValueSelector = x => ((AiDashboardExportRow)x).GiaTri }
                ]
            };
            var exported = _exportFileService.Export(request);
            return File(exported.Content, exported.ContentType, exported.FileName);
        }

        [HttpGet]
        public async Task<IActionResult> Train(CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Train))
            {
                return Forbid();
            }

            var vm = await _aiService.LayTrangTrainAsync(cancellationToken);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Train(AiTrainPageViewModel form, string modelType = "NguyenNhan", CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Train))
            {
                return Forbid();
            }

            var result = await _aiService.TrainAsync(modelType, form.ActivateAfterTrain, form.TrainNote, cancellationToken);
            if (result.ThanhCong)
            {
                TempData["Success"] = "Huấn luyện model nguyên nhân trễ thành công.";
            }
            else
            {
                TempData["Warning"] = BuildDetailedWarning(result);
            }

            return RedirectToAction(nameof(Train));
        }

        [HttpGet]
        public async Task<IActionResult> Predict(int? maDuAn, CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.PhanTichNguyenNhan))
            {
                return Forbid();
            }

            var vm = await _aiService.KhoiTaoTrangPredictAsync(maDuAn, cancellationToken);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Predict(AiPredictPageViewModel form, CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.PhanTichNguyenNhan))
            {
                return Forbid();
            }

            var vm = await _aiService.KhoiTaoTrangPredictAsync(form.MaDuAn > 0 ? form.MaDuAn : null, cancellationToken);
            GanFeatureNhap(vm, form);
            vm.ModelTestDuocChon = form.ModelTestDuocChon;

            if (form.MaDuAn <= 0)
            {
                vm.LoiHeThong = "Thiếu mã dự án. Vui lòng chọn dự án trước khi bấm Phân tích nguyên nhân trễ.";
                return View(vm);
            }

            if (!ModelState.IsValid)
            {
                var modelStateErrors = BuildModelStateErrorDetails(ModelState);
                _logger.LogWarning(
                    "ModelState invalid khi phan tich nguyen nhan tre. MaDuAn={MaDuAn}. Errors: {Errors}",
                    form.MaDuAn,
                    string.Join(" | ", modelStateErrors));
                vm.LoiHeThong = $"Dữ liệu nhập chưa hợp lệ: {string.Join("; ", modelStateErrors)}";
                return View(vm);
            }

            var result = await _aiService.DuDoanDuAnAsync(form, cancellationToken);
            if (!result.ThanhCong)
            {
                vm.CanhBao = BuildDetailedWarning(result);
                return View(vm);
            }

            TempData["Success"] = "Đã phân tích nguyên nhân trễ và lưu kết quả AI_KET_QUA.";
            if (result.Loi.Count > 0)
            {
                TempData["Warning"] = string.Join(" ", result.Loi);
            }

            return RedirectToAction(nameof(Predict), new { maDuAn = form.MaDuAn });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestPredict(AiPredictPageViewModel form, string? modelFile, CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.PhanTichNguyenNhan))
            {
                return Forbid();
            }

            var vm = await _aiService.KhoiTaoTrangPredictAsync(form.MaDuAn > 0 ? form.MaDuAn : null, cancellationToken);
            GanFeatureNhap(vm, form);
            vm.ModelTestDuocChon = string.IsNullOrWhiteSpace(modelFile) ? null : modelFile.Trim();

            var result = await _aiService.TestPredictAsync(form, modelFile, cancellationToken);
            vm.KetQuaTestPhanTich = result.DuLieu;
            if (!result.ThanhCong)
            {
                vm.CanhBao = BuildDetailedWarning(result);
            }

            return View("Predict", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanNguyenNhan(int maDuAn, string maDmNguyenNhan, double? doTinCay, CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.XacNhan))
            {
                return Forbid();
            }

            var result = await _aiService.XacNhanNguyenNhanAsync(maDuAn, maDmNguyenNhan, doTinCay, cancellationToken);
            if (result.ThanhCong)
            {
                TempData["Success"] = "Đã lưu xác nhận nguyên nhân AI.";
            }
            else
            {
                TempData["Warning"] = BuildDetailedWarning(result);
            }

            return RedirectToAction(nameof(Predict), new { maDuAn });
        }

        [HttpGet]
        public async Task<IActionResult> Models(string? model, string? loaiModel, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Train))
            {
                return Forbid();
            }

            var vm = await _aiService.LayTrangModelAsync(model, loaiModel, cancellationToken, pageNumber, pageSize);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateModel(string modelFile, string modelType = "NguyenNhan", CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Train))
            {
                return Forbid();
            }

            var vm = await _aiService.LayTrangModelAsync(modelFile, modelType, cancellationToken);
            vm.CurrentModelFile = modelFile;
            var result = await _aiService.ValidateModelAsync(modelFile, modelType, cancellationToken);
            vm.KetQuaValidate = result.DuLieu;
            if (!result.ThanhCong)
            {
                vm.CanhBao = BuildDetailedWarning(result);
            }

            return View("Models", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompareModel(string currentModelFile, string newModelFile, string modelType = "NguyenNhan", CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Train))
            {
                return Forbid();
            }

            var vm = await _aiService.LayTrangModelAsync(newModelFile, modelType, cancellationToken);
            vm.CurrentModelFile = currentModelFile;
            vm.NewModelFile = newModelFile;

            var result = await _aiService.CompareModelAsync(currentModelFile, newModelFile, modelType, cancellationToken);
            vm.KetQuaCompare = result.DuLieu;
            if (!result.ThanhCong)
            {
                vm.CanhBao = BuildDetailedWarning(result);
            }

            return View("Models", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActiveModel(string modelFile, string modelType = "NguyenNhan", CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Train))
            {
                return Forbid();
            }

            var result = await _aiService.DatModelHoatDongAsync(modelFile, modelType, cancellationToken);
            if (result.ThanhCong)
            {
                var loadedModel = result.DuLieu?.LoadedModel;
                TempData["Success"] = string.IsNullOrWhiteSpace(loadedModel)
                    ? $"Đã kích hoạt model {modelFile} ({modelType})."
                    : $"Đã kích hoạt model {modelFile} ({modelType}) và dịch vụ AI đang sử dụng model này ({loadedModel}).";
            }
            else
            {
                TempData["Warning"] = BuildDetailedWarning(result);
            }

            return RedirectToAction(nameof(Models), new { model = modelFile, loaiModel = modelType });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReloadModel(string modelType = "NguyenNhan", CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Train))
            {
                return Forbid();
            }

            var result = await _aiService.TaiLaiModelAsync(modelType, cancellationToken);
            if (result.ThanhCong)
            {
                TempData["Success"] = $"Đã tải lại model hoạt động ({modelType}).";
            }
            else
            {
                TempData["Warning"] = BuildDetailedWarning(result);
            }

            return RedirectToAction(nameof(Models), new { loaiModel = modelType });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteModel(string modelFile, string modelType = "NguyenNhan", CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Train))
            {
                return Forbid();
            }

            var result = await _aiService.XoaModelAsync(modelFile, cancellationToken);
            if (result.ThanhCong)
            {
                TempData["Success"] = $"Đã xóa model: {modelFile}.";
            }
            else
            {
                TempData["Warning"] = BuildDetailedWarning(result);
            }

            return RedirectToAction(nameof(Models), new { loaiModel = modelType });
        }

        [HttpGet]
        public async Task<IActionResult> ExportMetadata(string modelFile, string modelType = "NguyenNhan", CancellationToken cancellationToken = default)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Train))
            {
                return Forbid();
            }

            var result = await _aiService.LayTrangModelAsync(modelFile, modelType, cancellationToken);
            if (result.ChiTietModel == null)
            {
                TempData["Warning"] = "Không lấy được metadata model.";
                return RedirectToAction(nameof(Models), new { model = modelFile, loaiModel = modelType });
            }

            var json = JsonSerializer.Serialize(result.ChiTietModel, new JsonSerializerOptions { WriteIndented = true });
            var bytes = Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", $"metadata-{modelFile}.json");
        }

        private static void GanFeatureNhap(AiPredictPageViewModel vm, AiPredictPageViewModel form)
        {
            vm.MaDuAn = form.MaDuAn;
            vm.SoNhanVienDuAn = form.SoNhanVienDuAn;
            vm.TongSoCongViec = form.TongSoCongViec;
            vm.SoCongViecTre = form.SoCongViecTre;
            vm.TyLeCongViecTre = form.TyLeCongViecTre;
            vm.ChiPhiDuKien = form.ChiPhiDuKien;
            vm.ChiPhiThucTe = form.ChiPhiThucTe;
            vm.ChenhLechChiPhi = form.ChenhLechChiPhi;
            vm.SoLanThayDoiNhanSu = form.SoLanThayDoiNhanSu;
            vm.SoLanThayDoiQuanLy = form.SoLanThayDoiQuanLy;
            vm.SoNgayTreTienDo = form.SoNgayTreTienDo;
            vm.SoDeXuatCongViecChoDuyet = form.SoDeXuatCongViecChoDuyet;
            vm.SoDeXuatCongViecBiTuChoi = form.SoDeXuatCongViecBiTuChoi;
            vm.ThoiGianDuyetCongViecTrungBinh = form.ThoiGianDuyetCongViecTrungBinh;
            vm.SoDeXuatNganSachChoDuyet = form.SoDeXuatNganSachChoDuyet;
            vm.SoDeXuatNganSachBiTuChoi = form.SoDeXuatNganSachBiTuChoi;
            vm.ThoiGianDuyetNganSachTrungBinh = form.ThoiGianDuyetNganSachTrungBinh;
            vm.SoBaoCaoTienDoChoDuyet = form.SoBaoCaoTienDoChoDuyet;
            vm.SoBaoCaoTienDoBiTuChoi = form.SoBaoCaoTienDoBiTuChoi;
            vm.SoBaoCaoTienDoYeuCauBoSung = form.SoBaoCaoTienDoYeuCauBoSung;
            vm.TyLeBaoCaoTienDoBiTuChoi = form.TyLeBaoCaoTienDoBiTuChoi;
            vm.SoLanCapNhatTienDo = form.SoLanCapNhatTienDo;
            vm.SoNgayChamCapNhatTienDo = form.SoNgayChamCapNhatTienDo;
        }

        private static string BuildDetailedWarning<T>(AiOperationResultViewModel<T> result)
        {
            if (result.Loi == null || result.Loi.Count == 0)
            {
                return result.ThongBao;
            }

            var details = string.Join(Environment.NewLine, result.Loi.Select(x => $"- {x}"));
            return $"{result.ThongBao}{Environment.NewLine}{details}";
        }

        private static List<string> BuildModelStateErrorDetails(ModelStateDictionary modelState)
        {
            return modelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x =>
                {
                    var field = string.IsNullOrWhiteSpace(x.Key) ? "(tổng quát)" : x.Key;
                    return x.Value!.Errors.Select(error =>
                    {
                        var message = string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "Giá trị không hợp lệ."
                            : error.ErrorMessage;
                        return $"{field}: {message}";
                    });
                })
                .ToList();
        }

        private static List<object> BuildDashboardExportRows(AiDashboardPageViewModel vm)
        {
            var rows = new List<object>
            {
                new AiDashboardExportRow("Tổng quan", "Tổng lượt phân tích AI", vm.TongLanPhanTichTrongDb.ToString("N0")),
                new AiDashboardExportRow("Tổng quan", "Tổng xác nhận nguyên nhân", vm.TongXacNhanNguyenNhanTrongDb.ToString("N0")),
                new AiDashboardExportRow("Tổng quan", "Tỷ lệ xác nhận AI", $"{vm.TyLeXacNhanAi:0.##}%"),
                new AiDashboardExportRow("Dataset", "Tổng dòng dataset", vm.TongDongDataset.ToString("N0")),
                new AiDashboardExportRow("Dataset", "Dòng đủ điều kiện train", vm.TongDongDatasetHopLeTrain.ToString("N0")),
                new AiDashboardExportRow("Dataset", "Dự án trễ chưa xác nhận", vm.SoDuAnTreChuaXacNhan.ToString("N0"))
            };

            rows.AddRange(vm.NguyenNhanPhoBien.Select(x =>
                (object)new AiDashboardExportRow("Nguyên nhân phổ biến", x.NguyenNhan, $"{x.TyLePhanTram}%")));
            rows.AddRange(vm.NguyenNhanTheoQuanLy.Select(x =>
                (object)new AiDashboardExportRow("Nguyên nhân theo quản lý", $"{x.Nhom} - {x.NguyenNhan}", x.SoLan.ToString("N0"))));
            rows.AddRange(vm.NguyenNhanTheoTeam.Select(x =>
                (object)new AiDashboardExportRow("Nguyên nhân theo team", $"{x.Nhom} - {x.NguyenNhan}", x.SoLan.ToString("N0"))));

            return rows;
        }

        private static ExportFileFormat ParseExportFormat(string? format)
        {
            return format?.Trim().ToLowerInvariant() switch
            {
                "pdf" => ExportFileFormat.Pdf,
                "csv" => ExportFileFormat.Csv,
                _ => ExportFileFormat.Excel
            };
        }

        private sealed record AiDashboardExportRow(string Nhom, string ChiTieu, string GiaTri);
    }
}
