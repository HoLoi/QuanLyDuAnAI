using System.Linq;
using System.Text;
using System.Text.Json;
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
    public class AiController : Controller
    {
        private readonly IAiService _aiService;
        private readonly IPermissionHelper _permission;
        private readonly IExportFileService _exportFileService;

        public AiController(
            IAiService aiService,
            IPermissionHelper permission,
            IExportFileService exportFileService)
        {
            _aiService = aiService;
            _permission = permission;
            _exportFileService = exportFileService;
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
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                DataScopeText = "Theo phạm vi dự án được phép xem; AI chỉ hỗ trợ phân tích, không thay thế quyết định nghiệp vụ",
                Format = _exportFileService.ParseFormat(format),
                FileNamePrefix = "BaoCaoTongQuanAI",
                SheetName = "TongQuanAI",
                PdfLandscape = true,
                Rows = rows,
                Columns =
                [
                    new ExportColumnDefinition { Header = "Nhóm", ValueSelector = x => ((AiDashboardExportRow)x).Nhom, MinWidth = 18, MaxWidth = 28 },
                    new ExportColumnDefinition { Header = "Đối tượng", ValueSelector = x => ((AiDashboardExportRow)x).DoiTuong, MinWidth = 18, MaxWidth = 30 },
                    new ExportColumnDefinition { Header = "Chỉ tiêu / Nguyên nhân", ValueSelector = x => ((AiDashboardExportRow)x).ChiTieu, WrapText = true, MinWidth = 22, MaxWidth = 38, PdfRelativeWidth = 1.7f },
                    new ExportColumnDefinition { Header = "Giá trị", ValueSelector = x => ((AiDashboardExportRow)x).GiaTri, NumberFormat = "#,##0.##", Alignment = ExportColumnAlignment.Right },
                    new ExportColumnDefinition { Header = "Đơn vị", ValueSelector = x => ((AiDashboardExportRow)x).DonVi, Alignment = ExportColumnAlignment.Center }
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

            if (!maDuAn.HasValue || maDuAn.Value <= 0)
            {
                return RedirectToAction("Index", "DuAn");
            }

            var detailsUrl = Url.Action("Details", "DuAn", new { id = maDuAn.Value });
            return Redirect($"{detailsUrl}#phan-tich-nguyen-nhan-tre");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Predict(AiPredictPageViewModel form, CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.PhanTichNguyenNhan))
            {
                return Forbid();
            }

            if (form.MaDuAn <= 0)
            {
                TempData["Warning"] = "Dự án không hợp lệ.";
                return RedirectToAction("Index", "DuAn");
            }

            var result = await _aiService.PhanTichNguyenNhanDuAnAsync(form.MaDuAn, cancellationToken);
            if (!result.ThanhCong)
            {
                TempData["Warning"] = BuildDetailedWarning(result);
            }
            else
            {
                TempData["Success"] = "Phân tích nguyên nhân trễ thành công.";
            }

            var detailsUrl = Url.Action("Details", "DuAn", new { id = form.MaDuAn });
            return Redirect($"{detailsUrl}#phan-tich-nguyen-nhan-tre");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestPredict(int maDuAnTest, string? modelFile, CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.Train))
            {
                return Forbid();
            }

            var vm = await _aiService.LayTrangModelAsync(modelFile, "NguyenNhan", cancellationToken);
            vm.MaDuAnTest = maDuAnTest;
            vm.ModelTestDuocChon = string.IsNullOrWhiteSpace(modelFile) ? null : modelFile.Trim();
            var result = await _aiService.TestPredictAsync(
                new AiPredictPageViewModel { MaDuAn = maDuAnTest },
                modelFile,
                cancellationToken);
            vm.KetQuaTestPhanTich = result.DuLieu;
            if (!result.ThanhCong)
            {
                vm.CanhBao = BuildDetailedWarning(result);
            }

            return View("Models", vm);
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

            var detailsUrl = Url.Action("Details", "DuAn", new { id = maDuAn });
            return Redirect($"{detailsUrl}#phan-tich-nguyen-nhan-tre");
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
            var safeModelName = ExportSupport.NormalizeFileNamePart(
                Path.GetFileNameWithoutExtension(modelFile),
                "Model");
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return File(bytes, "application/json", $"MetadataModel_{safeModelName}_{timestamp}.json");
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

        private static List<object> BuildDashboardExportRows(AiDashboardPageViewModel vm)
        {
            var rows = new List<object>
            {
                new AiDashboardExportRow("Tổng quan", null, "Tổng lượt phân tích AI", vm.TongLanPhanTichTrongDb),
                new AiDashboardExportRow("Tổng quan", null, "Tổng xác nhận nguyên nhân", vm.TongXacNhanNguyenNhanTrongDb),
                new AiDashboardExportRow("Tỷ lệ xác nhận", "Dự án trễ đã xác nhận nguyên nhân / tổng dự án trễ", "Tỷ lệ xác nhận AI", vm.TyLeXacNhanAi, "%"),
                new AiDashboardExportRow("Dataset", null, "Số dự án có dataset mới nhất", vm.TongDongDataset),
                new AiDashboardExportRow("Dataset", null, "Dòng đủ feature và label", vm.TongDongDatasetHopLeTrain),
                new AiDashboardExportRow("Dataset", null, "Dự án trễ chưa xác nhận", vm.SoDuAnTreChuaXacNhan)
            };

            rows.AddRange(vm.NguyenNhanPhoBien.Select(x =>
                (object)new AiDashboardExportRow("Nguyên nhân phổ biến", $"{x.TyLePhanTram:0.##}%", x.NguyenNhan, x.SoLan, "lần")));
            rows.AddRange(vm.NguyenNhanTheoQuanLy.Select(x =>
                (object)new AiDashboardExportRow("Theo quản lý", x.Nhom, x.NguyenNhan, x.SoLan)));
            rows.AddRange(vm.NguyenNhanTheoTeam.Select(x =>
                (object)new AiDashboardExportRow("Theo team", x.Nhom, x.NguyenNhan, x.SoLan)));

            return rows;
        }

        private sealed record AiDashboardExportRow(
            string Nhom,
            string? DoiTuong,
            string ChiTieu,
            object? GiaTri,
            string? DonVi = null);
    }
}
