﻿using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.Ai;

namespace QuanLyDuAn.Services.Implementations
{
    public class AiService : IAiService
    {
        private const string ModelTypeNguyenNhan = "NguyenNhan";
        private const string ThongBaoDuAnKhongTre = "Dự án hiện chưa được xác định là trễ, không cần phân tích nguyên nhân trễ.";
        private const string AiRelatedReasonsPayloadMarker = "[[AI_RELATED_REASONS_V1]]";

        private readonly QuanLyDuAnDbContext _context;
        private readonly IAiApiService _aiApiService;
        private readonly IAiDatasetService _aiDatasetService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AiService> _logger;

        private sealed class AiRelatedReasonStoragePayload
        {
            public string? NoiDungPhanTich { get; set; }
            public string? MucPhuHop { get; set; }
            public List<AiRelatedReasonItemViewModel>? DanhSachNguyenNhanLienQuan { get; set; }
        }

        private sealed class AiDelayAnalysisContext
        {
            public int MaDuAn { get; set; }
            public string? TrangThaiDuAn { get; set; }
            public DateTime? NgayKetThucDuAn { get; set; }
            public DateTime? NgayHoanThanhThucTeDuAn { get; set; }
            public bool LaManagerHienTai { get; set; }
            public bool CoQuyenPhanTich { get; set; }
            public bool CoQuyenXacNhan { get; set; }
            public bool LaPhanTichTamThoi { get; set; }
            public bool LaPhanTichChinhThuc { get; set; }
            public bool DuAnKhongTre { get; set; }
            public string? LyDoKhongThePhanTich { get; set; }
        }

        public AiService(
            QuanLyDuAnDbContext context,
            IAiApiService aiApiService,
            IAiDatasetService aiDatasetService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AiService> logger)
        {
            _context = context;
            _aiApiService = aiApiService;
            _aiDatasetService = aiDatasetService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<AiDashboardPageViewModel> LayDashboardAsync(CancellationToken cancellationToken = default)
        {
            var roleFlags = await GetCurrentUserRoleFlagsAsync(cancellationToken);
            var currentUserId = await GetCurrentUserIdAsync(cancellationToken);
            var projectIds = await GetAccessibleProjectIdsAsync(currentUserId, roleFlags, cancellationToken);
            var gioiHanTheoScopeDuAn = !roleFlags.IsAdmin;

            var vm = new AiDashboardPageViewModel
            {
                LaQuanTriVien = roleFlags.IsAdmin,
                LaNguoiQuanLy = roleFlags.IsManager
            };

            var health = await _aiApiService.KiemTraSucKhoeAsync(cancellationToken);
            vm.Health = health.DuLieu;
            if (!health.ThanhCong)
            {
                vm.CanhBao = health.ThongBao;
            }
            else if (health.DuLieu != null)
            {
                vm.ModelNguyenNhanDangHoatDong = health.DuLieu.LoadedReasonModel;
            }

            if (roleFlags.IsAdmin)
            {
                var adminStatus = await _aiApiService.LayTrangThaiAdminAsync(cancellationToken);
                vm.AdminStatus = adminStatus.DuLieu;
                if (!adminStatus.ThanhCong)
                {
                    vm.CanhBao ??= adminStatus.ThongBao;
                }

                var logs = await _aiApiService.LayTongHopLogAsync(cancellationToken);
                vm.LogSummary = logs.DuLieu;
                if (!logs.ThanhCong)
                {
                    vm.CanhBao ??= logs.ThongBao;
                }

                var system = await _aiApiService.LayThongTinHeThongAsync(cancellationToken);
                vm.SystemInfo = system.DuLieu;
                if (!system.ThanhCong)
                {
                    vm.CanhBao ??= system.ThongBao;
                }
            }

            var aiModelQuery = _context.AiModel
                .Where(x => x.IsDeleted != true && x.LoaiModel == ModelTypeNguyenNhan);
            var aiKetQuaQuery = _context.AiKetQua.AsQueryable();
            var aiNguyenNhanQuery = _context.AiNguyenNhan.Where(x => x.IsDeleted != true);
            var aiDatasetQuery = _context.AiDataset.AsQueryable();

            if (gioiHanTheoScopeDuAn)
            {
                aiKetQuaQuery = aiKetQuaQuery.Where(x => projectIds.Contains(x.MaDuAn));
                aiNguyenNhanQuery = aiNguyenNhanQuery.Where(x => projectIds.Contains(x.MaDuAn));
                aiDatasetQuery = aiDatasetQuery.Where(x => projectIds.Contains(x.MaDuAn));
            }

            vm.TongModelTrongDb = await aiModelQuery.CountAsync(cancellationToken);
            vm.TongLanPhanTichTrongDb = await aiKetQuaQuery.CountAsync(cancellationToken);
            vm.TongXacNhanNguyenNhanTrongDb = await aiNguyenNhanQuery.CountAsync(cancellationToken);
            vm.TyLeXacNhanAi = vm.TongLanPhanTichTrongDb > 0
                ? Math.Round((double)vm.TongXacNhanNguyenNhanTrongDb * 100d / vm.TongLanPhanTichTrongDb, 2)
                : 0d;
            vm.LuotPhanTichTheoThang = await LayLuotPhanTichAiTheoThangAsync(
                aiKetQuaQuery,
                aiNguyenNhanQuery,
                cancellationToken);

            var latestDatasetByProject = await aiDatasetQuery
                .GroupBy(x => x.MaDuAn)
                .Select(g => g.OrderByDescending(x => x.NgayTongHop ?? DateTime.MinValue).ThenByDescending(x => x.MaData).First())
                .ToListAsync(cancellationToken);
            vm.TongDongDataset = latestDatasetByProject.Count;
            vm.SoDuAnDuocXacDinhTre = latestDatasetByProject.Count(x => x.LaDuAnTre == true);
            vm.SoDuAnTreChuaXacNhan = latestDatasetByProject.Count(x => x.LaDuAnTre == true && !x.MaDMNguyenNhan.HasValue);
            vm.SoDuAnTreDaXacNhan = Math.Max(0, vm.SoDuAnDuocXacDinhTre - vm.SoDuAnTreChuaXacNhan);
            vm.TongDongDatasetHopLeTrain = latestDatasetByProject.Count(x =>
                x.LaDuAnTre == true
                && x.MaDMNguyenNhan.HasValue
                && x.SoNhanVienDuAn.HasValue
                && x.TongSoCongViec.HasValue
                && x.SoCongViecTre.HasValue
                && x.TyLeCongViecTre.HasValue
                && x.ChiPhiDuKien.HasValue
                && x.ChiPhiThucTe.HasValue
                && x.ChenhLechChiPhi.HasValue
                && x.SoLanThayDoiNhanSu.HasValue
                && x.SoLanThayDoiQuanLy.HasValue
                && x.SoNgayTreTienDo.HasValue
                && x.SoDeXuatCongViecChoDuyet.HasValue
                && x.SoDeXuatCongViecBiTuChoi.HasValue
                && x.ThoiGianDuyetCongViecTrungBinh.HasValue
                && x.SoDeXuatNganSachChoDuyet.HasValue
                && x.SoDeXuatNganSachBiTuChoi.HasValue
                && x.ThoiGianDuyetNganSachTrungBinh.HasValue
                && x.SoBaoCaoTienDoChoDuyet.HasValue
                && x.SoBaoCaoTienDoBiTuChoi.HasValue
                && x.SoBaoCaoTienDoYeuCauBoSung.HasValue
                && x.TyLeBaoCaoTienDoBiTuChoi.HasValue
                && x.SoLanCapNhatTienDo.HasValue
                && x.SoNgayChamCapNhatTienDo.HasValue);

            var modelRows = await aiModelQuery
                .OrderByDescending(x => x.NgayTao ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaModel)
                .Select(x => new
                {
                    x.MaModel,
                    x.TenModel,
                    x.NgayTao,
                    x.DoChinhXac,
                    x.TrainSize,
                    x.TestSize,
                    x.IsActive
                })
                .ToListAsync(cancellationToken);
            vm.LichSuModelNguyenNhan = modelRows.Select(x => new AiModelVersionMetricViewModel
            {
                TenModel = string.IsNullOrWhiteSpace(x.TenModel) ? $"Model {x.MaModel}" : x.TenModel,
                CreatedAt = x.NgayTao ?? DateTime.MinValue,
                Accuracy = x.DoChinhXac,
                TrainSize = x.TrainSize,
                TestSize = x.TestSize,
                IsActive = x.IsActive == true
            }).ToList();

            if (!string.IsNullOrWhiteSpace(vm.ModelNguyenNhanDangHoatDong))
            {
                var detail = await _aiApiService.LayChiTietModelAsync(vm.ModelNguyenNhanDangHoatDong, cancellationToken);
                if (detail.ThanhCong && detail.DuLieu != null)
                {
                    vm.FeatureImportanceModelActive = ReadFeatureImportance(detail.DuLieu);
                    vm.ConfusionMatrixModelActive = ReadConfusionMatrix(detail.DuLieu);
                    vm.ConfusionMatrixLabelsModelActive = ReadConfusionMatrixLabels(detail.DuLieu);
                    vm.TrainSizeModelActive = TryReadInt(detail.DuLieu, "train_size", "trainSize");
                    vm.TestSizeModelActive = TryReadInt(detail.DuLieu, "test_size", "testSize");
                    vm.AccuracyModelActive = TryReadDouble(detail.DuLieu, "accuracy");
                    vm.PrecisionMacroModelActive = TryReadDouble(detail.DuLieu, "precision_macro", "precisionMacro");
                    vm.RecallMacroModelActive = TryReadDouble(detail.DuLieu, "recall_macro", "recallMacro");
                    vm.F1MacroModelActive = TryReadDouble(detail.DuLieu, "f1_macro", "f1Macro");
                    vm.DecisionTreeTextModelActive = TryReadString(detail.DuLieu, "decision_tree_text", "decisionTreeText");
                }
            }
            vm.CanhBaoChatLuongModel = BuildModelQualityWarnings(
                vm.LichSuModelNguyenNhan,
                vm.FeatureImportanceModelActive,
                vm.LichSuModelNguyenNhan.FirstOrDefault(x => x.IsActive)?.Accuracy,
                vm.TrainSizeModelActive,
                vm.TestSizeModelActive);

            var tenNguyenNhanRows = await _context.DmNguyenNhan
                .Select(x => new { x.MaDMNguyenNhan, x.TenNguyenNhan })
                .ToListAsync(cancellationToken);
            var tenNguyenNhanMap = tenNguyenNhanRows.ToDictionary(
                x => x.MaDMNguyenNhan,
                x => string.IsNullOrWhiteSpace(x.TenNguyenNhan) ? $"Nguyên nhân {x.MaDMNguyenNhan}" : x.TenNguyenNhan);
            vm.TenNguyenNhanTheoMa = tenNguyenNhanMap;

            var phanBoNguyenNhan = await aiNguyenNhanQuery
                .GroupBy(x => x.MaDMNguyenNhan)
                .Select(g => new { MaDm = g.Key, SoLan = g.Count() })
                .OrderByDescending(x => x.SoLan)
                .ToListAsync(cancellationToken);
            var tongSoLan = phanBoNguyenNhan.Sum(x => x.SoLan);
            vm.NguyenNhanPhoBien = phanBoNguyenNhan.Take(5).Select(x => new NguyenNhanThongKeItemViewModel
            {
                NguyenNhan = tenNguyenNhanMap.TryGetValue(x.MaDm, out var ten) ? ten : $"Nguyên nhân {x.MaDm}",
                TyLePhanTram = tongSoLan > 0 ? (int)Math.Round((double)x.SoLan * 100d / tongSoLan, MidpointRounding.AwayFromZero) : 0
            }).ToList();
            vm.NguyenNhanTheoQuanLy = await LayNguyenNhanTheoQuanLyAsync(
                aiNguyenNhanQuery,
                projectIds,
                gioiHanTheoScopeDuAn,
                cancellationToken);
            vm.NguyenNhanTheoTeam = await LayNguyenNhanTheoTeamAsync(
                aiNguyenNhanQuery,
                projectIds,
                gioiHanTheoScopeDuAn,
                cancellationToken);

            var tenDuAnRows = await _context.DuAn
                .Where(x => !gioiHanTheoScopeDuAn || projectIds.Contains(x.MaDuAn))
                .Select(x => new { x.MaDuAn, x.TenDuAn })
                .ToListAsync(cancellationToken);
            var tenDuAnMap = tenDuAnRows.ToDictionary(
                x => x.MaDuAn,
                x => string.IsNullOrWhiteSpace(x.TenDuAn) ? $"Dự án {x.MaDuAn}" : x.TenDuAn);

            var latestDatasetMapByProject = latestDatasetByProject
                .GroupBy(x => x.MaDuAn)
                .ToDictionary(x => x.Key, x => x.First());
            var latestKetQuaByProject = await aiKetQuaQuery
                .GroupBy(x => x.MaDuAn)
                .Select(g => g.OrderByDescending(x => x.ThoiGianDuDoanKetQua ?? DateTime.MinValue).ThenByDescending(x => x.MaAiKetQua).First())
                .ToListAsync(cancellationToken);
            vm.CanhBaoDuAnQuanLy = latestKetQuaByProject
                .Where(x =>
                    latestDatasetMapByProject.TryGetValue(x.MaDuAn, out var dataset)
                    && dataset.LaDuAnTre == true)
                .Select(x => new AiManagerRiskItemViewModel
                {
                    MaDuAn = x.MaDuAn,
                    TenDuAn = tenDuAnMap.TryGetValue(x.MaDuAn, out var ten) ? ten : $"Dự án {x.MaDuAn}",
                    TenNguyenNhan = tenNguyenNhanMap.TryGetValue(x.MaDMNguyenNhan, out var tenLyDo) ? tenLyDo : $"Nguyên nhân {x.MaDMNguyenNhan}",
                    DoTinCay = ChuanHoaDoTinCay(x.DoTinCayKetQua),
                    ThoiGianDuDoan = x.ThoiGianDuDoanKetQua,
                    NguonGoiY = string.IsNullOrWhiteSpace(x.ReasonSource) ? "RuleFallback" : x.ReasonSource,
                    KetQuaAiCoTheDaCu = latestDatasetMapByProject.TryGetValue(x.MaDuAn, out var dataset)
                                      && x.MaData != dataset.MaData,
                    CanhBao = latestDatasetMapByProject.TryGetValue(x.MaDuAn, out var datasetWarning)
                              && x.MaData != datasetWarning.MaData
                        ? "Kết quả AI có thể đã cũ hơn AI_DATASET mới nhất."
                        : null
                })
                .OrderByDescending(x => x.ThoiGianDuDoan ?? DateTime.MinValue)
                .Take(8)
                .ToList();

            return vm;
        }

        public async Task<AiTrainPageViewModel> LayTrangTrainAsync(CancellationToken cancellationToken = default)
        {
            var vm = new AiTrainPageViewModel();
            var qualitySummary = await _aiDatasetService.KiemTraChatLuongDatasetAsync(cancellationToken);
            var phanLoaiReason = await _aiDatasetService.PhanLoaiDatasetNguyenNhanDeTrainAsync(cancellationToken);
            var qualityReason = phanLoaiReason.ThongKe;

            vm.TongDongDataset = qualitySummary.TongSoDong;
            vm.TongDongDuAnTre = qualitySummary.SoMauDuAnTre;
            vm.TongDongCoNguyenNhanXacNhan = qualitySummary.SoDongCoNguyenNhan;
            vm.TongDongDatasetNguyenNhan = qualityReason.SoDongDuocDungTrain;
            vm.CoTheTrainNguyenNhan = qualityReason.DuDieuKienTrain;
            vm.BaoCaoDatasetNguyenNhanGanNhat = qualityReason;
            var tenNguyenNhanRows = await _context.DmNguyenNhan
                .Select(x => new { x.MaDMNguyenNhan, x.TenNguyenNhan })
                .ToListAsync(cancellationToken);
            var tenNguyenNhanTheoMa = tenNguyenNhanRows.ToDictionary(
                x => x.MaDMNguyenNhan,
                x => string.IsNullOrWhiteSpace(x.TenNguyenNhan) ? $"Nguyên nhân {x.MaDMNguyenNhan}" : x.TenNguyenNhan);
            vm.TenNguyenNhanTheoMa = tenNguyenNhanTheoMa;
            var phanBoTatCa = qualityReason.PhanBoLopDuDieuKien
                .Concat(qualityReason.PhanBoLopDangTichLuy)
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Sum(v => v.Value));
            vm.PhanBoNguyenNhanDataset = qualityReason.PhanBoLopDuDieuKien
                .Select(x =>
                {
                    var ten = int.TryParse(x.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var maDm)
                        && tenNguyenNhanTheoMa.TryGetValue(maDm, out var tenMap)
                            ? tenMap
                            : $"Nguyên nhân #{x.Key}";
                    return new { Ten = ten, SoLuong = x.Value };
                })
                .GroupBy(x => x.Ten)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.SoLuong));
            vm.PhanBoNguyenNhanTrain = phanBoTatCa
                .Select(x =>
                {
                    var maDm = int.TryParse(x.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                        ? parsed
                        : 0;
                    var ten = maDm > 0 && tenNguyenNhanTheoMa.TryGetValue(maDm, out var tenMap)
                        ? tenMap
                        : $"Nguyên nhân #{x.Key}";
                    return new AiReasonClassDistributionRowViewModel
                    {
                        MaDMNguyenNhan = maDm,
                        TenNguyenNhan = ten,
                        SoDong = x.Value,
                        DuDieuKien = qualityReason.PhanBoLopDuDieuKien.ContainsKey(x.Key)
                    };
                })
                .OrderByDescending(x => x.DuDieuKien)
                .ThenByDescending(x => x.SoDong)
                .ThenBy(x => x.MaDMNguyenNhan)
                .ToList();
            vm.KhuyenNghiTrainNguyenNhan = new AiTrainRecommendationViewModel
            {
                NenTrain = qualityReason.DuDieuKienTrain,
                LyDo = qualityReason.DuDieuKienTrain
                    ? "Dataset nguyên nhân đạt điều kiện train."
                    : "Dataset nguyên nhân chưa đạt điều kiện train.",
                CanhBaoThieuDuLieu = !qualityReason.DuDieuKienTrain,
                CanhBaoImbalance = qualityReason.ImbalanceRatio >= 3d,
                RecommendationMessages = qualityReason.DuDieuKienTrain
                    ? qualityReason.GhiChuChatLuongDuLieu.ToList()
                    : qualityReason.LyDoKhongDat.ToList()
            };

            if (!vm.CoTheTrainNguyenNhan)
            {
                vm.CanhBao = "Chưa đủ 30 dòng hoặc 2 nguyên nhân để huấn luyện.";
            }
            else if (qualityReason.SoLopDangTichLuy > 0)
            {
                vm.CanhBao = "Một số nguyên nhân đang tiếp tục tích lũy dữ liệu.";
            }

            var modelList = await _aiApiService.LayDanhSachModelAsync(ModelTypeNguyenNhan, cancellationToken);
            var latestModel = modelList.DuLieu?
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.UpdatedAt)
                .FirstOrDefault();
            if (latestModel != null)
            {
                vm.KetQuaTrain = new AiTrainResponseViewModel
                {
                    TenModel = latestModel.TenFile,
                    ModelFile = latestModel.TenFile,
                    SoLuongDuLieu = latestModel.TrainSize.GetValueOrDefault() + latestModel.TestSize.GetValueOrDefault(),
                    DoChinhXac = latestModel.Accuracy.GetValueOrDefault(),
                    TrainSize = latestModel.TrainSize.GetValueOrDefault(),
                    TestSize = latestModel.TestSize.GetValueOrDefault(),
                    NgayTao = latestModel.CreatedAt,
                    CreatedAt = latestModel.CreatedAt,
                    FeatureImportance = latestModel.FeatureImportance,
                    ConfusionMatrix = latestModel.ConfusionMatrix,
                    ConfusionMatrixLabels = latestModel.ConfusionMatrixLabels,
                    PrecisionMacro = latestModel.PrecisionMacro,
                    RecallMacro = latestModel.RecallMacro,
                    F1Macro = latestModel.F1Macro,
                    PrecisionWeighted = latestModel.PrecisionWeighted,
                    RecallWeighted = latestModel.RecallWeighted,
                    F1Weighted = latestModel.F1Weighted,
                    ClassDistribution = latestModel.ClassDistribution,
                    DecisionTreeText = latestModel.DecisionTreeText
                };
            }
            vm.CanhBaoChatLuongModel = BuildModelQualityWarnings(
                vm.KetQuaTrain == null ? [] : [new AiModelVersionMetricViewModel
                {
                    TenModel = vm.KetQuaTrain.ModelFile,
                    Accuracy = vm.KetQuaTrain.DoChinhXac,
                    TrainSize = vm.KetQuaTrain.TrainSize,
                    TestSize = vm.KetQuaTrain.TestSize,
                    IsActive = true,
                    CreatedAt = vm.KetQuaTrain.CreatedAt
                }],
                vm.KetQuaTrain?.FeatureImportance ?? [],
                vm.KetQuaTrain?.DoChinhXac,
                vm.KetQuaTrain?.TrainSize ?? 0,
                vm.KetQuaTrain?.TestSize ?? 0);

            return vm;
        }

        public async Task<AiOperationResultViewModel<AiTrainResponseViewModel>> TrainAsync(string modelType, bool activateAfterTrain, string? trainNote, CancellationToken cancellationToken = default)
        {
            var roleFlags = await GetCurrentUserRoleFlagsAsync(cancellationToken);
            if (!roleFlags.IsAdmin)
            {
                return new AiOperationResultViewModel<AiTrainResponseViewModel>
                {
                    ThanhCong = false,
                    ThongBao = "Chỉ quản trị viên được phép train model AI.",
                    Loi = ["Không đủ quyền thao tác train model."]
                };
            }

            var modelTypeNormalized = NormalizeModelType(modelType);
            if (!string.Equals(modelTypeNormalized, ModelTypeNguyenNhan, StringComparison.OrdinalIgnoreCase))
            {
                return new AiOperationResultViewModel<AiTrainResponseViewModel>
                {
                    ThanhCong = false,
                    ThongBao = "Hệ thống chỉ hỗ trợ train model nguyên nhân trễ.",
                    Loi = ["modelType phải là NguyenNhan."]
                };
            }

            var phanLoai = await _aiDatasetService.PhanLoaiDatasetNguyenNhanDeTrainAsync(cancellationToken);
            var quality = phanLoai.ThongKe;
            if (!quality.DuDieuKienTrain)
            {
                return new AiOperationResultViewModel<AiTrainResponseViewModel>
                {
                    ThanhCong = false,
                    ThongBao = "Dataset nguyên nhân chưa đủ điều kiện train model.",
                    Loi = quality.LyDoKhongDat
                };
            }

            var trainDataset = BuildReasonTrainDataset(phanLoai.TapDuocDungTrain);
            if (trainDataset.Count == 0)
            {
                return new AiOperationResultViewModel<AiTrainResponseViewModel>
                {
                    ThanhCong = false,
                    ThongBao = "Dataset nguyên nhân chưa có dòng hợp lệ để train.",
                    Loi = ["Cần dữ liệu có LaDuAnTre=1, MaDMNguyenNhan hợp lệ và đủ toàn bộ feature train."]
                };
            }

            var user = _httpContextAccessor.HttpContext?.User;
            var request = new AiTrainRequestViewModel
            {
                Dataset = trainDataset,
                RequestedByUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier),
                RequestedByUserName = user?.Identity?.Name,
                TrainNote = trainNote?.Trim(),
                ActivateAfterTrain = activateAfterTrain,
                ModelType = ModelTypeNguyenNhan
            };

            var trainResult = await _aiApiService.TrainModelAsync(request, cancellationToken);
            if (!trainResult.ThanhCong || trainResult.DuLieu == null)
            {
                return trainResult;
            }

            await LuuThongTinModelAsync(trainResult.DuLieu, activateAfterTrain, cancellationToken);
            return trainResult;
        }

        public async Task<AiModelPageViewModel> LayTrangModelAsync(string? modelDangXem, string? boLocLoaiModel, CancellationToken cancellationToken = default, int pageNumber = 1, int pageSize = 20)
        {
            var vm = new AiModelPageViewModel
            {
                ModelDangXem = modelDangXem,
                BoLocLoaiModel = ModelTypeNguyenNhan
            };

            var list = await _aiApiService.LayDanhSachModelAsync(ModelTypeNguyenNhan, cancellationToken);
            var tatCaModel = list.DuLieu ?? [];
            vm.LichSuModelNguyenNhan = tatCaModel
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.UpdatedAt)
                .ThenByDescending(x => x.TenFile)
                .Select(x => new AiModelVersionMetricViewModel
                {
                    TenModel = x.TenFile,
                    CreatedAt = x.CreatedAt,
                    Accuracy = x.Accuracy,
                    PrecisionMacro = x.PrecisionMacro,
                    RecallMacro = x.RecallMacro,
                    F1Macro = x.F1Macro,
                    TrainSize = x.TrainSize,
                    TestSize = x.TestSize,
                    IsActive = x.IsDefault
                })
                .ToList();
            vm.Pagination = PaginationViewModel.Create(pageNumber, pageSize, tatCaModel.Count);
            vm.DanhSachModel = tatCaModel
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.UpdatedAt)
                .ThenByDescending(x => x.TenFile)
                .Skip((vm.Pagination.PageNumber - 1) * vm.Pagination.PageSize)
                .Take(vm.Pagination.PageSize)
                .ToList();
            var tenNguyenNhanRows = await _context.DmNguyenNhan
                .Select(x => new { x.MaDMNguyenNhan, x.TenNguyenNhan })
                .ToListAsync(cancellationToken);
            vm.TenNguyenNhanTheoMa = tenNguyenNhanRows.ToDictionary(
                x => x.MaDMNguyenNhan,
                x => string.IsNullOrWhiteSpace(x.TenNguyenNhan) ? $"Nguyên nhân {x.MaDMNguyenNhan}" : x.TenNguyenNhan);
            vm.DanhSachDuAnTest = await _context.DuAn
                .Where(x => x.IsDeleted != true && _context.AiDataset.Any(d => d.MaDuAn == x.MaDuAn))
                .OrderBy(x => x.TenDuAn)
                .Select(x => new AiDuAnOptionViewModel
                {
                    MaDuAn = x.MaDuAn,
                    TenDuAn = string.IsNullOrWhiteSpace(x.TenDuAn) ? $"Dự án {x.MaDuAn}" : x.TenDuAn
                })
                .ToListAsync(cancellationToken);
            vm.CoModelLocal = tatCaModel.Count > 0;
            vm.CoTheKiemTraModel = vm.CoModelLocal;
            vm.CoTheKichHoatModel = tatCaModel.Any(x => x.CanActivate);
            vm.CoTheSoSanhModel = tatCaModel.Count(x => x.CanActivate) >= 2;
            if (!list.ThanhCong)
            {
                vm.CanhBao = list.ThongBao;
            }

            var health = await _aiApiService.KiemTraSucKhoeAsync(cancellationToken);
            if (health.ThanhCong && health.DuLieu != null)
            {
                vm.LoadedReasonModel = health.DuLieu.LoadedReasonModel;
                vm.LoadedModel = health.DuLieu.LoadedReasonModel;
            }

            var danhSachModelSet = tatCaModel
                .Select(x => x.TenFile)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            vm.LoadedModelNamTrongDanhSachLocal =
                !string.IsNullOrWhiteSpace(vm.LoadedReasonModel)
                && danhSachModelSet.Contains(vm.LoadedReasonModel.Trim());
            if (!string.IsNullOrWhiteSpace(vm.LoadedReasonModel))
            {
                foreach (var item in vm.LichSuModelNguyenNhan)
                {
                    item.IsActive = string.Equals(item.TenModel, vm.LoadedReasonModel, StringComparison.OrdinalIgnoreCase);
                }
            }

            vm.CurrentModelFile = vm.LoadedModelNamTrongDanhSachLocal
                ? vm.LoadedReasonModel
                : tatCaModel.FirstOrDefault(x => x.CanActivate)?.TenFile;
            vm.NewModelFile = tatCaModel
                .Where(x => x.CanActivate)
                .Select(x => x.TenFile)
                .FirstOrDefault(x => !string.Equals(x, vm.CurrentModelFile, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(modelDangXem))
            {
                var detail = await _aiApiService.LayChiTietModelAsync(modelDangXem.Trim(), cancellationToken);
                vm.ChiTietModel = detail.DuLieu;
                vm.TenModelPhanTich = modelDangXem.Trim();
                if (!detail.ThanhCong)
                {
                    vm.CanhBao ??= detail.ThongBao;
                }
                else if (detail.DuLieu != null)
                {
                    vm.FeatureImportanceModelActive = ReadFeatureImportance(detail.DuLieu);
                    vm.ConfusionMatrixModelActive = ReadConfusionMatrix(detail.DuLieu);
                    vm.ConfusionMatrixLabelsModelActive = ReadConfusionMatrixLabels(detail.DuLieu);
                    vm.AccuracyModelActive = TryReadDouble(detail.DuLieu, "accuracy");
                    vm.PrecisionMacroModelActive = TryReadDouble(detail.DuLieu, "precision_macro", "precisionMacro");
                    vm.RecallMacroModelActive = TryReadDouble(detail.DuLieu, "recall_macro", "recallMacro");
                    vm.F1MacroModelActive = TryReadDouble(detail.DuLieu, "f1_macro", "f1Macro");
                    vm.DecisionTreeTextModelActive = TryReadString(detail.DuLieu, "decision_tree_text", "decisionTreeText");
                }
            }
            else if (!string.IsNullOrWhiteSpace(vm.LoadedReasonModel))
            {
                var detail = await _aiApiService.LayChiTietModelAsync(vm.LoadedReasonModel.Trim(), cancellationToken);
                if (detail.ThanhCong && detail.DuLieu != null)
                {
                    vm.TenModelPhanTich = vm.LoadedReasonModel.Trim();
                    vm.FeatureImportanceModelActive = ReadFeatureImportance(detail.DuLieu);
                    vm.ConfusionMatrixModelActive = ReadConfusionMatrix(detail.DuLieu);
                    vm.ConfusionMatrixLabelsModelActive = ReadConfusionMatrixLabels(detail.DuLieu);
                    vm.AccuracyModelActive = TryReadDouble(detail.DuLieu, "accuracy");
                    vm.PrecisionMacroModelActive = TryReadDouble(detail.DuLieu, "precision_macro", "precisionMacro");
                    vm.RecallMacroModelActive = TryReadDouble(detail.DuLieu, "recall_macro", "recallMacro");
                    vm.F1MacroModelActive = TryReadDouble(detail.DuLieu, "f1_macro", "f1Macro");
                    vm.DecisionTreeTextModelActive = TryReadString(detail.DuLieu, "decision_tree_text", "decisionTreeText");
                }
            }

            var modelDangXemHoacActive = tatCaModel.FirstOrDefault(x =>
                string.Equals(x.TenFile, vm.TenModelPhanTich, StringComparison.OrdinalIgnoreCase))
                ?? tatCaModel.FirstOrDefault(x =>
                    string.Equals(x.TenFile, vm.LoadedReasonModel, StringComparison.OrdinalIgnoreCase));
            if (modelDangXemHoacActive != null)
            {
                vm.AccuracyModelActive ??= modelDangXemHoacActive.Accuracy;
                vm.PrecisionMacroModelActive ??= modelDangXemHoacActive.PrecisionMacro;
                vm.RecallMacroModelActive ??= modelDangXemHoacActive.RecallMacro;
                vm.F1MacroModelActive ??= modelDangXemHoacActive.F1Macro;
                if (vm.ConfusionMatrixLabelsModelActive.Count == 0)
                {
                    vm.ConfusionMatrixLabelsModelActive = modelDangXemHoacActive.ConfusionMatrixLabels;
                }

                vm.DecisionTreeTextModelActive ??= modelDangXemHoacActive.DecisionTreeText;
            }
            vm.CanhBaoChatLuongModel = BuildModelQualityWarnings(
                vm.LichSuModelNguyenNhan,
                vm.FeatureImportanceModelActive,
                vm.LichSuModelNguyenNhan.FirstOrDefault(x =>
                    string.Equals(x.TenModel, vm.TenModelPhanTich, StringComparison.OrdinalIgnoreCase)
                    || x.IsActive)?.Accuracy,
                vm.LichSuModelNguyenNhan.FirstOrDefault(x =>
                    string.Equals(x.TenModel, vm.TenModelPhanTich, StringComparison.OrdinalIgnoreCase)
                    || x.IsActive)?.TrainSize ?? 0,
                vm.LichSuModelNguyenNhan.FirstOrDefault(x =>
                    string.Equals(x.TenModel, vm.TenModelPhanTich, StringComparison.OrdinalIgnoreCase)
                    || x.IsActive)?.TestSize ?? 0);

            return vm;
        }

        public Task<AiOperationResultViewModel<AiValidateModelResponseViewModel>> ValidateModelAsync(string modelFile, string modelType, CancellationToken cancellationToken = default)
            => _aiApiService.ValidateModelAsync(modelFile.Trim(), ModelTypeNguyenNhan, cancellationToken);

        public async Task<AiOperationResultViewModel<AiCompareModelResponseViewModel>> CompareModelAsync(string currentModelFile, string newModelFile, string modelType, CancellationToken cancellationToken = default)
        {
            var datasetHopLe = await _aiDatasetService.LayDatasetNguyenNhanHopLeDeTrainAsync(cancellationToken);
            var testDataset = BuildReasonTrainDataset(datasetHopLe);
            if (testDataset.Count == 0)
            {
                return new AiOperationResultViewModel<AiCompareModelResponseViewModel>
                {
                    ThanhCong = false,
                    ThongBao = "Không có dữ liệu test hợp lệ để so sánh model.",
                    Loi = ["Dataset test loại NguyenNhan đang trống."]
                };
            }

            return await _aiApiService.CompareModelAsync(new AiCompareModelRequestViewModel
            {
                CurrentModelFile = currentModelFile.Trim(),
                NewModelFile = newModelFile.Trim(),
                TestDataset = testDataset,
                ModelType = ModelTypeNguyenNhan
            }, cancellationToken);
        }

        public async Task<AiOperationResultViewModel<AiModelActivationResultViewModel>> DatModelHoatDongAsync(string modelFile, string modelType, CancellationToken cancellationToken = default)
        {
            var result = await _aiApiService.DatModelHoatDongAsync(modelFile.Trim(), ModelTypeNguyenNhan, cancellationToken);
            if (!result.ThanhCong || result.DuLieu == null)
            {
                return result;
            }

            var models = await _context.AiModel
                .Where(x => x.IsDeleted != true && x.LoaiModel == ModelTypeNguyenNhan)
                .ToListAsync(cancellationToken);
            foreach (var model in models)
            {
                model.IsActive = string.Equals(model.TenModel, modelFile, StringComparison.OrdinalIgnoreCase);
            }

            await _context.SaveChangesAsync(cancellationToken);
            result.DuLieu.ModelType = ModelTypeNguyenNhan;
            return result;
        }

        public Task<AiOperationResultViewModel<AiModelReloadResultViewModel>> TaiLaiModelAsync(string modelType, CancellationToken cancellationToken = default)
            => _aiApiService.TaiLaiModelHoatDongAsync(ModelTypeNguyenNhan, cancellationToken);

        public async Task<AiOperationResultViewModel<AiModelDeleteResultViewModel>> XoaModelAsync(string modelFile, CancellationToken cancellationToken = default)
        {
            var result = await _aiApiService.XoaModelAsync(modelFile, cancellationToken);
            if (result.ThanhCong)
            {
                var model = await _context.AiModel
                    .FirstOrDefaultAsync(x => x.IsDeleted != true && x.TenModel == modelFile, cancellationToken);
                if (model != null)
                {
                    model.IsDeleted = true;
                    model.DeletedAt = DateTime.Now;
                    model.DeletedBy = await GetCurrentUserIdAsync(cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            return result;
        }

        public async Task<AiProjectDelayAnalysisPanelViewModel> LayPhanTichNguyenNhanDuAnAsync(int maDuAn, CancellationToken cancellationToken = default)
        {
            var panel = new AiProjectDelayAnalysisPanelViewModel
            {
                MaDuAn = maDuAn,
                BadgeTinhTrang = "Chưa áp dụng"
            };

            var context = await LayNguCanhPhanTichAsync(maDuAn, cancellationToken);
            panel.CoThePhanTich = context.CoQuyenPhanTich
                                  && context.LaManagerHienTai
                                  && (context.LaPhanTichTamThoi || context.LaPhanTichChinhThuc);
            panel.LaPhanTichTamThoi = context.LaPhanTichTamThoi;
            panel.LaPhanTichChinhThuc = context.LaPhanTichChinhThuc;
            panel.BadgeLoaiPhanTich = context.LaPhanTichTamThoi ? "Tạm thời" : context.LaPhanTichChinhThuc ? "Chính thức" : string.Empty;
            panel.BadgeTinhTrang = context.LaPhanTichTamThoi
                ? "Đang trễ"
                : context.LaPhanTichChinhThuc
                    ? "Hoàn thành trễ"
                    : context.DuAnKhongTre
                        ? "Dự án không trễ"
                        : "Chưa áp dụng";
            panel.ThongBao = context.LyDoKhongThePhanTich;

            await NapKetQuaPhanTichGanNhatChoPanelAsync(panel, maDuAn, cancellationToken);
            await NapXacNhanNguyenNhanChoPanelAsync(panel, maDuAn, cancellationToken);
            panel.DanhSachNguyenNhan = await _context.DmNguyenNhan
                .OrderBy(x => x.MaDMNguyenNhan)
                .Select(x => new AiReasonOptionViewModel
                {
                    MaDMNguyenNhan = x.MaDMNguyenNhan,
                    TenNguyenNhan = string.IsNullOrWhiteSpace(x.TenNguyenNhan)
                        ? $"Nguyên nhân {x.MaDMNguyenNhan}"
                        : x.TenNguyenNhan
                })
                .ToListAsync(cancellationToken);

            var maDatasetMoiNhat = await _context.AiDataset
                .Where(x => x.MaDuAn == maDuAn && x.LaDuAnTre == true)
                .OrderByDescending(x => x.NgayTongHop ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaData)
                .Select(x => (int?)x.MaData)
                .FirstOrDefaultAsync(cancellationToken);
            var coKetQuaChinhThucHienTai = maDatasetMoiNhat.HasValue
                && await _context.AiKetQua.AnyAsync(
                    x => x.MaDuAn == maDuAn && x.MaData == maDatasetMoiNhat.Value,
                    cancellationToken);
            panel.CoTheXacNhan = context.CoQuyenXacNhan
                                 && context.LaManagerHienTai
                                 && context.LaPhanTichChinhThuc
                                 && coKetQuaChinhThucHienTai;
            return panel;
        }

        public async Task<AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>> PhanTichNguyenNhanDuAnAsync(int maDuAn, CancellationToken cancellationToken = default)
        {
            var context = await LayNguCanhPhanTichAsync(maDuAn, cancellationToken);
            var permissionError = KiemTraQuyenPhanTich(context);
            if (permissionError != null)
            {
                return permissionError;
            }

            AiDataset? dataset = null;
            AiProjectFeatureSnapshotViewModel? snapshot = null;
            if (context.LaPhanTichChinhThuc)
            {
                dataset = await LayHoacLamMoiDatasetChinhThucAsync(maDuAn, cancellationToken);
                if (dataset?.LaDuAnTre != true)
                {
                    return new AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>
                    {
                        ThanhCong = false,
                        ThongBao = "Dự án không trễ.",
                        Loi = []
                    };
                }
            }
            else if (context.LaPhanTichTamThoi)
            {
                snapshot = await _aiDatasetService.BuildFeatureSnapshotAsync(maDuAn, cancellationToken);
                if (snapshot == null)
                {
                    return new AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>
                    {
                        ThanhCong = false,
                        ThongBao = "Chưa đủ dữ liệu phân tích.",
                        Loi = []
                    };
                }
            }
            else
            {
                return new AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>
                {
                    ThanhCong = false,
                    ThongBao = context.LyDoKhongThePhanTich ?? "Dự án không trễ.",
                    Loi = []
                };
            }

            var input = context.LaPhanTichChinhThuc
                ? TaoInputTuDataset(maDuAn, dataset!)
                : TaoInputTuSnapshot(snapshot!);

            var result = await GoiFastApiPhanTichAsync(input, cancellationToken);
            if (!result.ThanhCong || result.DuLieu == null)
            {
                return result;
            }

            result.DuLieu.LaKetQuaTamThoi = context.LaPhanTichTamThoi;
            result.DuLieu.LaKetQuaChinhThuc = context.LaPhanTichChinhThuc;
            result.DuLieu.ThoiGianPhanTich = DateTime.Now;

            if (!await LaNguyenNhanDuDoanHopLeAsync(result.DuLieu.MaDMNguyenNhanDuDoan, cancellationToken))
            {
                return new AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>
                {
                    ThanhCong = false,
                    ThongBao = "Không xác định được nguyên nhân phù hợp.",
                    Loi = []
                };
            }

            if (context.LaPhanTichChinhThuc)
            {
                await LuuKetQuaPhanTichAsync(maDuAn, dataset!, result.DuLieu, cancellationToken);
            }

            return result;
        }

        public async Task<AiOperationResultViewModel<AiTestReasonResponseViewModel>> TestPredictAsync(AiPredictPageViewModel input, string? modelFile, CancellationToken cancellationToken = default)
        {
            input = await BoSungFeatureTuDatasetNeuCanAsync(input, cancellationToken);
            if (input.MaDuAn > 0)
            {
                var dataset = await _context.AiDataset
                    .Where(x => x.MaDuAn == input.MaDuAn)
                    .OrderByDescending(x => x.NgayTongHop ?? DateTime.MinValue)
                    .ThenByDescending(x => x.MaData)
                    .FirstOrDefaultAsync(cancellationToken);
                if (dataset != null)
                {
                    GanFeatureTuDataset(input, dataset);
                    input.LaDuAnTre = dataset.LaDuAnTre;
                    if (dataset.LaDuAnTre != true)
                    {
                        return new AiOperationResultViewModel<AiTestReasonResponseViewModel>
                        {
                            ThanhCong = false,
                            ThongBao = ThongBaoDuAnKhongTre,
                            Loi = []
                        };
                    }
                }
            }

            var danhMucRows = await _context.DmNguyenNhan
                .OrderBy(x => x.MaDMNguyenNhan)
                .Select(x => new
                {
                    x.MaDMNguyenNhan,
                    x.TenNguyenNhan
                })
                .ToListAsync(cancellationToken);
            var danhMuc = danhMucRows.Select(x => new AiReasonCatalogItemViewModel
            {
                MaDMNguyenNhan = x.MaDMNguyenNhan.ToString(CultureInfo.InvariantCulture),
                TenNguyenNhan = string.IsNullOrWhiteSpace(x.TenNguyenNhan) ? $"Nguyên nhân {x.MaDMNguyenNhan}" : x.TenNguyenNhan
            }).ToList();
            var request = new AiTestReasonRequestViewModel
            {
                ModelFile = string.IsNullOrWhiteSpace(modelFile) ? null : modelFile.Trim(),
                Feature = MapFeature(input),
                DanhMucNguyenNhan = danhMuc
            };

            var result = await _aiApiService.TestPredictAsync(request, cancellationToken);
            if (result.ThanhCong && result.DuLieu != null)
            {
                result.DuLieu = HauKiemKetQuaTestNguyenNhan(result.DuLieu, request.Feature, danhMuc);
            }

            return result;
        }

        public async Task<AiOperationResultViewModel<bool>> XacNhanNguyenNhanAsync(int maDuAn, string maDmNguyenNhan, double? doTinCay, CancellationToken cancellationToken = default)
        {
            if (!int.TryParse(maDmNguyenNhan, NumberStyles.Integer, CultureInfo.InvariantCulture, out var maDmNguyenNhanInt))
            {
                return new AiOperationResultViewModel<bool>
                {
                    ThanhCong = false,
                    ThongBao = "Mã nguyên nhân không hợp lệ.",
                    Loi = ["Không thể lưu xác nhận nguyên nhân AI."]
                };
            }

            var roleFlags = await GetCurrentUserRoleFlagsAsync(cancellationToken);
            var currentUserId = await GetCurrentUserIdAsync(cancellationToken);
            var coQuyenXacNhanTheoClaim = HasPermissionClaim(Permissions.AI.XacNhan);

            if (!roleFlags.IsManager || roleFlags.IsAdmin || !coQuyenXacNhanTheoClaim)
            {
                return new AiOperationResultViewModel<bool>
                {
                    ThanhCong = false,
                    ThongBao = "Bạn không có quyền xác nhận nguyên nhân AI.",
                    Loi = ["Không đủ quyền xác nhận."]
                };
            }

            var duAn = await _context.DuAn
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true)
                .Select(x => new
                {
                    x.MaDuAn,
                    x.MaNguoiDung,
                    x.TrangThaiDuAn,
                    x.NgayKetThucDuAn,
                    x.NgayHoanThanhThucTeDuAn
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (duAn == null)
            {
                return new AiOperationResultViewModel<bool>
                {
                    ThanhCong = false,
                    ThongBao = "Không tìm thấy dự án cần xác nhận nguyên nhân.",
                    Loi = ["Dự án không tồn tại hoặc đã bị xóa."]
                };
            }

            if (duAn.MaNguoiDung != currentUserId)
            {
                return new AiOperationResultViewModel<bool>
                {
                    ThanhCong = false,
                    ThongBao = "Bạn không có quyền xác nhận nguyên nhân cho dự án này.",
                    Loi = ["Chỉ Manager hiện tại của dự án được xác nhận."]
                };
            }

            var duAnHoanThanhTre = await LaDuAnHoanThanhTreHopLeAsync(
                maDuAn,
                duAn.TrangThaiDuAn,
                duAn.NgayKetThucDuAn,
                duAn.NgayHoanThanhThucTeDuAn,
                cancellationToken);
            if (!duAnHoanThanhTre)
            {
                return new AiOperationResultViewModel<bool>
                {
                    ThanhCong = false,
                    ThongBao = "Dự án không trễ.",
                    Loi = []
                };
            }

            var tonTaiDanhMuc = await _context.DmNguyenNhan.AnyAsync(x => x.MaDMNguyenNhan == maDmNguyenNhanInt, cancellationToken);
            if (!tonTaiDanhMuc)
            {
                return new AiOperationResultViewModel<bool>
                {
                    ThanhCong = false,
                    ThongBao = "Nguyên nhân xác nhận không hợp lệ.",
                    Loi = ["Danh mục nguyên nhân không tồn tại."]
                };
            }

            var datasetMoiNhat = await _context.AiDataset
                .Where(x => x.MaDuAn == maDuAn)
                .OrderByDescending(x => x.NgayTongHop ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaData)
                .FirstOrDefaultAsync(cancellationToken);

            if (datasetMoiNhat?.LaDuAnTre != true)
            {
                return new AiOperationResultViewModel<bool>
                {
                    ThanhCong = false,
                    ThongBao = "Dự án hiện không trễ, không cần xác nhận nguyên nhân.",
                    Loi = ["LaDuAnTre=0 nên hệ thống không cho xác nhận nguyên nhân trễ."]
                };
            }

            var coKetQuaChinhThucHopLe = await _context.AiKetQua
                .AnyAsync(x => x.MaDuAn == maDuAn
                               && x.MaData == datasetMoiNhat.MaData,
                    cancellationToken);
            if (!coKetQuaChinhThucHopLe)
            {
                return new AiOperationResultViewModel<bool>
                {
                    ThanhCong = false,
                    ThongBao = "Chưa có kết quả phân tích chính thức hợp lệ.",
                    Loi = []
                };
            }

            var row = await _context.AiNguyenNhan
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true)
                .OrderByDescending(x => x.NgayXacNhan ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaAINguyenNhan)
                .FirstOrDefaultAsync(cancellationToken);

            if (row == null)
            {
                row = new AiNguyenNhan
                {
                    MaDuAn = maDuAn,
                    MaDMNguyenNhan = maDmNguyenNhanInt,
                    DoTinCay = doTinCay,
                    NgayXacNhan = DateTime.Now,
                    MaNguoiDungXacNhan = currentUserId,
                    IsDeleted = false
                };
                _context.AiNguyenNhan.Add(row);
            }
            else
            {
                row.MaDMNguyenNhan = maDmNguyenNhanInt;
                row.DoTinCay = doTinCay;
                row.NgayXacNhan = DateTime.Now;
                row.MaNguoiDungXacNhan = currentUserId;
                row.IsDeleted = false;
                row.DeletedAt = null;
                row.DeletedBy = null;
            }

            datasetMoiNhat.MaDMNguyenNhan = maDmNguyenNhanInt;
            datasetMoiNhat.GhiChuDataset = "Đã đồng bộ nguyên nhân xác nhận từ Manager.";

            await _context.SaveChangesAsync(cancellationToken);
            return new AiOperationResultViewModel<bool>
            {
                ThanhCong = true,
                DuLieu = true,
                ThongBao = "Đã lưu xác nhận nguyên nhân từ Manager."
            };
        }

        private async Task<AiDelayAnalysisContext> LayNguCanhPhanTichAsync(int maDuAn, CancellationToken cancellationToken)
        {
            var roleFlags = await GetCurrentUserRoleFlagsAsync(cancellationToken);
            var currentUserId = await GetCurrentUserIdAsync(cancellationToken);
            var coQuyenPhanTich = HasPermissionClaim(Permissions.AI.PhanTichNguyenNhan);
            var coQuyenXacNhan = HasPermissionClaim(Permissions.AI.XacNhan);

            var duAn = await _context.DuAn
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true)
                .Select(x => new
                {
                    x.MaDuAn,
                    x.MaNguoiDung,
                    x.TrangThaiDuAn,
                    x.NgayKetThucDuAn,
                    x.NgayHoanThanhThucTeDuAn
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (duAn == null)
            {
                return new AiDelayAnalysisContext
                {
                    MaDuAn = maDuAn,
                    LyDoKhongThePhanTich = "Không tìm thấy dự án."
                };
            }

            var laManagerHienTai = roleFlags.IsManager && !roleFlags.IsAdmin && duAn.MaNguoiDung == currentUserId;
            var now = DateTime.Now;
            var coNgayKetThuc = duAn.NgayKetThucDuAn.HasValue;
            var daQuaHan = coNgayKetThuc && now > duAn.NgayKetThucDuAn!.Value;
            var daHoanThanh = TrangThai.LaHoanThanhCongViec(duAn.TrangThaiDuAn);
            var laLuuTru = TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.LuuTru);
            var hoanThanhTre = await LaDuAnHoanThanhTreHopLeAsync(
                maDuAn,
                duAn.TrangThaiDuAn,
                duAn.NgayKetThucDuAn,
                duAn.NgayHoanThanhThucTeDuAn,
                cancellationToken);
            var trangThaiTamThoiHopLe = TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.DangThucHien)
                                        || TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.ChoXacNhanHoanThanh);
            var tamThoi = !daHoanThanh && !laLuuTru && trangThaiTamThoiHopLe && daQuaHan;

            var context = new AiDelayAnalysisContext
            {
                MaDuAn = duAn.MaDuAn,
                TrangThaiDuAn = duAn.TrangThaiDuAn,
                NgayKetThucDuAn = duAn.NgayKetThucDuAn,
                NgayHoanThanhThucTeDuAn = duAn.NgayHoanThanhThucTeDuAn,
                LaManagerHienTai = laManagerHienTai,
                CoQuyenPhanTich = coQuyenPhanTich,
                CoQuyenXacNhan = coQuyenXacNhan,
                LaPhanTichTamThoi = tamThoi,
                LaPhanTichChinhThuc = hoanThanhTre,
                DuAnKhongTre = (daHoanThanh || laLuuTru) && !hoanThanhTre
            };

            context.LyDoKhongThePhanTich = XacDinhThongBaoKhongThePhanTich(context, coNgayKetThuc, daQuaHan, daHoanThanh, laLuuTru);
            return context;
        }

        private static string? XacDinhThongBaoKhongThePhanTich(
            AiDelayAnalysisContext context,
            bool coNgayKetThuc,
            bool daQuaHan,
            bool daHoanThanh,
            bool laLuuTru)
        {
            if (context.LaPhanTichTamThoi || context.LaPhanTichChinhThuc)
            {
                return null;
            }

            if (!coNgayKetThuc)
            {
                return "Chưa đủ dữ liệu phân tích.";
            }

            if (context.DuAnKhongTre)
            {
                return "Dự án không trễ.";
            }

            if (!daQuaHan && !daHoanThanh && !laLuuTru)
            {
                return "Dự án chưa quá hạn.";
            }

            return "Dự án không trễ.";
        }

        private static AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>? KiemTraQuyenPhanTich(AiDelayAnalysisContext context)
        {
            if (!context.CoQuyenPhanTich)
            {
                return new AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>
                {
                    ThanhCong = false,
                    ThongBao = "Bạn không có quyền phân tích dự án này.",
                    Loi = []
                };
            }

            if (!context.LaManagerHienTai)
            {
                return new AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>
                {
                    ThanhCong = false,
                    ThongBao = "Bạn không có quyền phân tích dự án này.",
                    Loi = []
                };
            }

            return null;
        }

        private async Task<AiDataset?> LayHoacLamMoiDatasetChinhThucAsync(int maDuAn, CancellationToken cancellationToken)
        {
            var dataset = await LayDatasetMoiNhatAsync(maDuAn, cancellationToken);
            var canRefresh = dataset == null;
            if (dataset != null)
            {
                var freshness = await KiemTraCanTongHopLaiDatasetAsync(maDuAn, dataset.NgayTongHop, cancellationToken);
                canRefresh = freshness.CanTongHopLai;
            }

            if (canRefresh)
            {
                await _aiDatasetService.TongHopDatasetChoDuAnAsync(maDuAn, cancellationToken);
                dataset = await LayDatasetMoiNhatAsync(maDuAn, cancellationToken);
            }

            return dataset;
        }

        private Task<AiDataset?> LayDatasetMoiNhatAsync(int maDuAn, CancellationToken cancellationToken)
            => _context.AiDataset
                .Where(x => x.MaDuAn == maDuAn)
                .OrderByDescending(x => x.NgayTongHop ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaData)
                .FirstOrDefaultAsync(cancellationToken);

        private async Task<bool> LaDuAnHoanThanhTreHopLeAsync(
            int maDuAn,
            string? trangThaiDuAn,
            DateTime? ngayKetThucDuAn,
            DateTime? ngayHoanThanhThucTeDuAn,
            CancellationToken cancellationToken)
        {
            var daKetThuc = TrangThai.LaHoanThanhCongViec(trangThaiDuAn)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.LuuTru);
            if (!daKetThuc
                || !ngayKetThucDuAn.HasValue
                || !ngayHoanThanhThucTeDuAn.HasValue
                || ngayHoanThanhThucTeDuAn.Value <= ngayKetThucDuAn.Value)
            {
                return false;
            }

            var trangThaiHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
            var coCongViecTre = await (
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where dm.MaDuAn == maDuAn
                      && cv.IsDeleted != true
                      && dm.IsDeleted != true
                      && cv.NgayKetThucCVDuKien.HasValue
                      && cv.NgayKetThucCVThucTe.HasValue
                      && trangThaiHoanThanh.Contains(cv.TrangThaiCongViec ?? string.Empty)
                      && cv.NgayKetThucCVThucTe.Value > cv.NgayKetThucCVDuKien.Value
                select cv.MaCongViec)
                .AnyAsync(cancellationToken);
            if (!coCongViecTre)
            {
                return false;
            }

            return await (
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where dm.MaDuAn == maDuAn
                      && cv.IsDeleted != true
                      && dm.IsDeleted != true
                      && cv.NgayKetThucCVThucTe.HasValue
                      && trangThaiHoanThanh.Contains(cv.TrangThaiCongViec ?? string.Empty)
                      && cv.NgayKetThucCVThucTe.Value > ngayKetThucDuAn.Value
                select cv.MaCongViec)
                .AnyAsync(cancellationToken);
        }

        private static AiPredictPageViewModel TaoInputTuDataset(int maDuAn, AiDataset dataset)
        {
            var input = new AiPredictPageViewModel { MaDuAn = maDuAn, LaDuAnTre = dataset.LaDuAnTre };
            GanFeatureTuDataset(input, dataset);
            return input;
        }

        private static AiPredictPageViewModel TaoInputTuSnapshot(AiProjectFeatureSnapshotViewModel snapshot)
        {
            return new AiPredictPageViewModel
            {
                MaDuAn = snapshot.MaDuAn,
                LaDuAnTre = snapshot.LaDuAnTre,
                SoNhanVienDuAn = snapshot.SoNhanVienDuAn,
                TongSoCongViec = snapshot.TongSoCongViec,
                SoCongViecTre = snapshot.SoCongViecTre,
                TyLeCongViecTre = snapshot.TyLeCongViecTre,
                ChiPhiDuKien = (double)snapshot.ChiPhiDuKien,
                ChiPhiThucTe = (double)snapshot.ChiPhiThucTe,
                ChenhLechChiPhi = (double)snapshot.ChenhLechChiPhi,
                SoLanThayDoiNhanSu = snapshot.SoLanThayDoiNhanSu,
                SoLanThayDoiQuanLy = snapshot.SoLanThayDoiQuanLy,
                SoNgayTreTienDo = snapshot.SoNgayTreTienDo,
                SoDeXuatCongViecChoDuyet = snapshot.SoDeXuatCongViecChoDuyet,
                SoDeXuatCongViecBiTuChoi = snapshot.SoDeXuatCongViecBiTuChoi,
                ThoiGianDuyetCongViecTrungBinh = snapshot.ThoiGianDuyetCongViecTrungBinh,
                SoDeXuatNganSachChoDuyet = snapshot.SoDeXuatNganSachChoDuyet,
                SoDeXuatNganSachBiTuChoi = snapshot.SoDeXuatNganSachBiTuChoi,
                ThoiGianDuyetNganSachTrungBinh = snapshot.ThoiGianDuyetNganSachTrungBinh,
                SoBaoCaoTienDoChoDuyet = snapshot.SoBaoCaoTienDoChoDuyet,
                SoBaoCaoTienDoBiTuChoi = snapshot.SoBaoCaoTienDoBiTuChoi,
                SoBaoCaoTienDoYeuCauBoSung = snapshot.SoBaoCaoTienDoYeuCauBoSung,
                TyLeBaoCaoTienDoBiTuChoi = snapshot.TyLeBaoCaoTienDoBiTuChoi,
                SoLanCapNhatTienDo = snapshot.SoLanCapNhatTienDo,
                SoNgayChamCapNhatTienDo = snapshot.SoNgayChamCapNhatTienDo
            };
        }

        private async Task<AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>> GoiFastApiPhanTichAsync(
            AiPredictPageViewModel input,
            CancellationToken cancellationToken)
        {
            var danhMuc = await LayDanhMucNguyenNhanAsync(cancellationToken);
            var request = new AiAnalyzeDelayReasonRequestViewModel
            {
                MaDuAn = input.MaDuAn,
                Feature = MapFeature(input),
                DanhMucNguyenNhan = danhMuc,
                ReasonConfidenceThreshold = 0.6
            };

            var result = await _aiApiService.DuDoanDuAnAsync(request, cancellationToken);
            if (result.ThanhCong && result.DuLieu != null)
            {
                result.DuLieu = HauKiemKetQuaNguyenNhan(result.DuLieu, request.Feature, danhMuc);
            }

            return result;
        }

        private async Task<List<AiReasonCatalogItemViewModel>> LayDanhMucNguyenNhanAsync(CancellationToken cancellationToken)
        {
            var danhMucRows = await _context.DmNguyenNhan
                .OrderBy(x => x.MaDMNguyenNhan)
                .Select(x => new
                {
                    x.MaDMNguyenNhan,
                    x.TenNguyenNhan
                })
                .ToListAsync(cancellationToken);

            return danhMucRows.Select(x => new AiReasonCatalogItemViewModel
            {
                MaDMNguyenNhan = x.MaDMNguyenNhan.ToString(CultureInfo.InvariantCulture),
                TenNguyenNhan = string.IsNullOrWhiteSpace(x.TenNguyenNhan) ? $"Nguyên nhân {x.MaDMNguyenNhan}" : x.TenNguyenNhan
            }).ToList();
        }

        private Task<bool> LaNguyenNhanDuDoanHopLeAsync(int? maDmNguyenNhan, CancellationToken cancellationToken)
        {
            if (!maDmNguyenNhan.HasValue || maDmNguyenNhan.Value <= 0)
            {
                return Task.FromResult(false);
            }

            return _context.DmNguyenNhan.AnyAsync(x => x.MaDMNguyenNhan == maDmNguyenNhan.Value, cancellationToken);
        }

        private async Task NapKetQuaPhanTichGanNhatChoPanelAsync(
            AiProjectDelayAnalysisPanelViewModel panel,
            int maDuAn,
            CancellationToken cancellationToken)
        {
            var duLieuAi = await (
                from kq in _context.AiKetQua
                join dm in _context.DmNguyenNhan on kq.MaDMNguyenNhan equals dm.MaDMNguyenNhan into dmJoin
                from dm in dmJoin.DefaultIfEmpty()
                join model in _context.AiModel on kq.MaModel equals model.MaModel into modelJoin
                from model in modelJoin.DefaultIfEmpty()
                where kq.MaDuAn == maDuAn
                orderby kq.ThoiGianDuDoanKetQua descending, kq.MaAiKetQua descending
                select new
                {
                    kq.MaDMNguyenNhan,
                    TenNguyenNhan = dm != null ? dm.TenNguyenNhan : null,
                    kq.DoTinCayKetQua,
                    kq.ThoiGianDuDoanKetQua,
                    kq.ReasonSource,
                    ModelNguyenNhan = model != null ? model.TenModel : null,
                    kq.CanhBaoNguyenNhan,
                    kq.NoiDungPhanTich
                }).FirstOrDefaultAsync(cancellationToken);

            if (duLieuAi == null)
            {
                return;
            }

            var duLieuNoiDung = ParseStoredNoiDungPhanTich(duLieuAi.NoiDungPhanTich);
            panel.ThoiGianPhanTich = duLieuAi.ThoiGianDuDoanKetQua;
            panel.KetQua = new AiAnalyzeDelayReasonResponseViewModel
            {
                MaDMNguyenNhanDuDoan = duLieuAi.MaDMNguyenNhan,
                TenNguyenNhanDuDoan = string.IsNullOrWhiteSpace(duLieuAi.TenNguyenNhan) ? null : duLieuAi.TenNguyenNhan,
                DoTinCayKetQua = ChuanHoaDoTinCay(duLieuAi.DoTinCayKetQua),
                MucPhuHop = duLieuNoiDung.MucPhuHop,
                DanhSachNguyenNhanLienQuan = duLieuNoiDung.DanhSachNguyenNhanLienQuan,
                ReasonSource = string.IsNullOrWhiteSpace(duLieuAi.ReasonSource) ? "RuleFallback" : duLieuAi.ReasonSource,
                ModelNguyenNhanUsed = duLieuAi.ModelNguyenNhan,
                CanhBaoNguyenNhan = duLieuAi.CanhBaoNguyenNhan,
                NoiDungPhanTich = duLieuNoiDung.NoiDungPhanTich,
                LaKetQuaChinhThuc = true,
                ThoiGianPhanTich = duLieuAi.ThoiGianDuDoanKetQua
            };
        }

        private async Task NapXacNhanNguyenNhanChoPanelAsync(
            AiProjectDelayAnalysisPanelViewModel panel,
            int maDuAn,
            CancellationToken cancellationToken)
        {
            var xacNhan = await (
                from nn in _context.AiNguyenNhan
                join dm in _context.DmNguyenNhan on nn.MaDMNguyenNhan equals dm.MaDMNguyenNhan into dmJoin
                from dm in dmJoin.DefaultIfEmpty()
                where nn.MaDuAn == maDuAn && nn.IsDeleted != true
                orderby nn.NgayXacNhan descending, nn.MaAINguyenNhan descending
                select new
                {
                    nn.MaDMNguyenNhan,
                    TenNguyenNhan = dm != null ? dm.TenNguyenNhan : null,
                    nn.DoTinCay,
                    nn.NgayXacNhan
                }).FirstOrDefaultAsync(cancellationToken);

            if (xacNhan == null)
            {
                return;
            }

            panel.MaDMNguyenNhanDaXacNhan = xacNhan.MaDMNguyenNhan;
            panel.NguyenNhanDaXacNhan = xacNhan.TenNguyenNhan;
            panel.DoTinCayDaXacNhan = xacNhan.DoTinCay;
            panel.ThoiGianXacNhan = xacNhan.NgayXacNhan;
        }

        private bool HasPermissionClaim(string permission)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => x.Value.Trim())
                .Contains(permission, StringComparer.OrdinalIgnoreCase) == true;
        }

        private async Task LuuThongTinModelAsync(AiTrainResponseViewModel trainResult, bool activateAfterTrain, CancellationToken cancellationToken)
        {
            var existed = await _context.AiModel
                .FirstOrDefaultAsync(x =>
                    x.IsDeleted != true
                    && x.TenModel == trainResult.ModelFile
                    && x.LoaiModel == ModelTypeNguyenNhan, cancellationToken);

            if (activateAfterTrain)
            {
                var activeModels = await _context.AiModel
                    .Where(x => x.IsDeleted != true && x.LoaiModel == ModelTypeNguyenNhan)
                    .ToListAsync(cancellationToken);
                foreach (var model in activeModels)
                {
                    model.IsActive = false;
                }
            }

            if (existed == null)
            {
                existed = new AiModel
                {
                    TenModel = trainResult.ModelFile,
                    SoLuongDuLieu = trainResult.SoLuongDuLieu,
                    DoChinhXac = trainResult.DoChinhXac,
                    TrainSize = trainResult.TrainSize,
                    TestSize = trainResult.TestSize,
                    NgayTao = trainResult.NgayTao.ToLocalTime(),
                    MoTaModel = CatChuoi(trainResult.MoTaModel, 250),
                    LoaiModel = ModelTypeNguyenNhan,
                    IsActive = activateAfterTrain,
                    IsDeleted = false
                };
                _context.AiModel.Add(existed);
            }
            else
            {
                existed.SoLuongDuLieu = trainResult.SoLuongDuLieu;
                existed.DoChinhXac = trainResult.DoChinhXac;
                existed.TrainSize = trainResult.TrainSize;
                existed.TestSize = trainResult.TestSize;
                existed.NgayTao = trainResult.NgayTao.ToLocalTime();
                existed.MoTaModel = CatChuoi(trainResult.MoTaModel, 250);
                existed.LoaiModel = ModelTypeNguyenNhan;
                existed.IsActive = activateAfterTrain || existed.IsActive == true;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task LuuKetQuaPhanTichAsync(
            int maDuAn,
            AiDataset dataset,
            AiAnalyzeDelayReasonResponseViewModel phanTich,
            CancellationToken cancellationToken)
        {
            var tenModel = string.IsNullOrWhiteSpace(phanTich.ModelNguyenNhanUsed)
                ? await _context.AiModel
                    .Where(x => x.IsDeleted != true && x.IsActive == true && x.LoaiModel == ModelTypeNguyenNhan)
                    .OrderByDescending(x => x.NgayTao ?? DateTime.MinValue)
                    .ThenByDescending(x => x.MaModel)
                    .Select(x => x.TenModel)
                    .FirstOrDefaultAsync(cancellationToken)
                : phanTich.ModelNguyenNhanUsed.Trim();

            var model = await _context.AiModel
                .FirstOrDefaultAsync(x =>
                    x.IsDeleted != true
                    && x.TenModel == tenModel
                    && x.LoaiModel == ModelTypeNguyenNhan, cancellationToken);
            if (model == null)
            {
                model = new AiModel
                {
                    TenModel = tenModel ?? "reason_fallback",
                    LoaiModel = ModelTypeNguyenNhan,
                    IsActive = false,
                    IsDeleted = false,
                    NgayTao = DateTime.Now,
                    MoTaModel = "Đồng bộ từ kết quả phân tích nguyên nhân."
                };
                _context.AiModel.Add(model);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var maDmNguyenNhan = phanTich.MaDMNguyenNhanDuDoan ?? 0;
            if (maDmNguyenNhan <= 0)
            {
                return;
            }

            _context.AiKetQua.Add(new AiKetQua
            {
                MaDuAn = maDuAn,
                MaData = dataset.MaData,
                MaModel = model.MaModel,
                MaDMNguyenNhan = maDmNguyenNhan,
                DoTinCayKetQua = phanTich.DoTinCayKetQua,
                ThoiGianDuDoanKetQua = DateTime.Now,
                ReasonSource = string.IsNullOrWhiteSpace(phanTich.ReasonSource) ? "RuleFallback" : phanTich.ReasonSource,
                CanhBaoNguyenNhan = CatChuoi(phanTich.CanhBaoNguyenNhan, 500),
                NoiDungPhanTich = BuildStoredNoiDungPhanTich(phanTich)
            });
            await _context.SaveChangesAsync(cancellationToken);
        }

        private static string? BuildStoredNoiDungPhanTich(AiAnalyzeDelayReasonResponseViewModel phanTich)
        {
            var relatedReasons = phanTich.DanhSachNguyenNhanLienQuan?
                .Where(x => !string.IsNullOrWhiteSpace(x.TenNguyenNhan))
                .Select(x => new AiRelatedReasonItemViewModel
                {
                    MaDMNguyenNhan = x.MaDMNguyenNhan,
                    TenNguyenNhan = x.TenNguyenNhan?.Trim(),
                    Score = x.Score,
                    MucPhuHop = x.MucPhuHop
                })
                .ToList();

            var coDuLieuBoSung = !string.IsNullOrWhiteSpace(phanTich.MucPhuHop)
                || (relatedReasons?.Count > 0);
            if (!coDuLieuBoSung)
            {
                return phanTich.NoiDungPhanTich;
            }

            var payload = new AiRelatedReasonStoragePayload
            {
                NoiDungPhanTich = phanTich.NoiDungPhanTich,
                MucPhuHop = phanTich.MucPhuHop,
                DanhSachNguyenNhanLienQuan = relatedReasons
            };
            return AiRelatedReasonsPayloadMarker + JsonSerializer.Serialize(payload);
        }

        private static AiRelatedReasonStoragePayload ParseStoredNoiDungPhanTich(string? rawNoiDungPhanTich)
        {
            if (string.IsNullOrWhiteSpace(rawNoiDungPhanTich))
            {
                return new AiRelatedReasonStoragePayload();
            }

            if (!rawNoiDungPhanTich.StartsWith(AiRelatedReasonsPayloadMarker, StringComparison.Ordinal))
            {
                return new AiRelatedReasonStoragePayload
                {
                    NoiDungPhanTich = rawNoiDungPhanTich
                };
            }

            try
            {
                var rawJson = rawNoiDungPhanTich[AiRelatedReasonsPayloadMarker.Length..];
                var payload = JsonSerializer.Deserialize<AiRelatedReasonStoragePayload>(rawJson);
                if (payload == null)
                {
                    return new AiRelatedReasonStoragePayload
                    {
                        NoiDungPhanTich = rawNoiDungPhanTich
                    };
                }

                return payload;
            }
            catch
            {
                return new AiRelatedReasonStoragePayload
                {
                    NoiDungPhanTich = rawNoiDungPhanTich
                };
            }
        }

        private static void GanFeatureTuDataset(AiPredictPageViewModel vm, AiDataset dataset)
        {
            vm.SoNhanVienDuAn = dataset.SoNhanVienDuAn ?? 0;
            vm.TongSoCongViec = dataset.TongSoCongViec ?? 0;
            vm.SoCongViecTre = dataset.SoCongViecTre ?? 0;
            vm.TyLeCongViecTre = dataset.TyLeCongViecTre ?? 0;
            vm.ChiPhiDuKien = (double)(dataset.ChiPhiDuKien ?? 0);
            vm.ChiPhiThucTe = (double)(dataset.ChiPhiThucTe ?? 0);
            vm.ChenhLechChiPhi = (double)(dataset.ChenhLechChiPhi ?? 0);
            vm.SoLanThayDoiNhanSu = dataset.SoLanThayDoiNhanSu ?? 0;
            vm.SoLanThayDoiQuanLy = dataset.SoLanThayDoiQuanLy ?? 0;
            vm.SoNgayTreTienDo = dataset.SoNgayTreTienDo ?? 0;
            vm.SoDeXuatCongViecChoDuyet = dataset.SoDeXuatCongViecChoDuyet ?? 0;
            vm.SoDeXuatCongViecBiTuChoi = dataset.SoDeXuatCongViecBiTuChoi ?? 0;
            vm.ThoiGianDuyetCongViecTrungBinh = dataset.ThoiGianDuyetCongViecTrungBinh ?? 0;
            vm.SoDeXuatNganSachChoDuyet = dataset.SoDeXuatNganSachChoDuyet ?? 0;
            vm.SoDeXuatNganSachBiTuChoi = dataset.SoDeXuatNganSachBiTuChoi ?? 0;
            vm.ThoiGianDuyetNganSachTrungBinh = dataset.ThoiGianDuyetNganSachTrungBinh ?? 0;
            vm.SoBaoCaoTienDoChoDuyet = dataset.SoBaoCaoTienDoChoDuyet ?? 0;
            vm.SoBaoCaoTienDoBiTuChoi = dataset.SoBaoCaoTienDoBiTuChoi ?? 0;
            vm.SoBaoCaoTienDoYeuCauBoSung = dataset.SoBaoCaoTienDoYeuCauBoSung ?? 0;
            vm.TyLeBaoCaoTienDoBiTuChoi = dataset.TyLeBaoCaoTienDoBiTuChoi ?? 0;
            vm.SoLanCapNhatTienDo = dataset.SoLanCapNhatTienDo ?? 0;
            vm.SoNgayChamCapNhatTienDo = dataset.SoNgayChamCapNhatTienDo ?? 0;
        }

        private async Task<AiPredictPageViewModel> BoSungFeatureTuDatasetNeuCanAsync(AiPredictPageViewModel input, CancellationToken cancellationToken)
        {
            if (input.MaDuAn <= 0)
            {
                return input;
            }

            var canBoSung =
                input.SoNhanVienDuAn == 0
                && input.TongSoCongViec == 0
                && input.SoCongViecTre == 0
                && input.TyLeCongViecTre == 0
                && input.ChiPhiDuKien == 0
                && input.ChiPhiThucTe == 0
                && input.ChenhLechChiPhi == 0
                && input.SoLanThayDoiNhanSu == 0
                && input.SoLanThayDoiQuanLy == 0
                && input.SoNgayTreTienDo == 0
                && input.SoDeXuatCongViecChoDuyet == 0
                && input.SoDeXuatCongViecBiTuChoi == 0
                && input.ThoiGianDuyetCongViecTrungBinh == 0
                && input.SoDeXuatNganSachChoDuyet == 0
                && input.SoDeXuatNganSachBiTuChoi == 0
                && input.ThoiGianDuyetNganSachTrungBinh == 0
                && input.SoBaoCaoTienDoChoDuyet == 0
                && input.SoBaoCaoTienDoBiTuChoi == 0
                && input.SoBaoCaoTienDoYeuCauBoSung == 0
                && input.TyLeBaoCaoTienDoBiTuChoi == 0
                && input.SoLanCapNhatTienDo == 0
                && input.SoNgayChamCapNhatTienDo == 0;

            if (!canBoSung)
            {
                return input;
            }

            var dataset = await _context.AiDataset
                .Where(x => x.MaDuAn == input.MaDuAn)
                .OrderByDescending(x => x.NgayTongHop ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaData)
                .FirstOrDefaultAsync(cancellationToken);
            if (dataset == null)
            {
                return input;
            }

            GanFeatureTuDataset(input, dataset);
            input.LaDuAnTre = dataset.LaDuAnTre;
            return input;
        }

        private static AiProjectFeatureViewModel MapFeature(AiPredictPageViewModel input)
        {
            return new AiProjectFeatureViewModel
            {
                SoNhanVienDuAn = input.SoNhanVienDuAn,
                TongSoCongViec = input.TongSoCongViec,
                SoCongViecTre = input.SoCongViecTre,
                TyLeCongViecTre = ChuanHoaTyLeCongViecTre(input.TyLeCongViecTre),
                ChiPhiDuKien = input.ChiPhiDuKien,
                ChiPhiThucTe = input.ChiPhiThucTe,
                ChenhLechChiPhi = input.ChenhLechChiPhi,
                SoLanThayDoiNhanSu = input.SoLanThayDoiNhanSu,
                SoLanThayDoiQuanLy = input.SoLanThayDoiQuanLy,
                SoNgayTreTienDo = input.SoNgayTreTienDo,
                SoDeXuatCongViecChoDuyet = input.SoDeXuatCongViecChoDuyet,
                SoDeXuatCongViecBiTuChoi = input.SoDeXuatCongViecBiTuChoi,
                ThoiGianDuyetCongViecTrungBinh = input.ThoiGianDuyetCongViecTrungBinh,
                SoDeXuatNganSachChoDuyet = input.SoDeXuatNganSachChoDuyet,
                SoDeXuatNganSachBiTuChoi = input.SoDeXuatNganSachBiTuChoi,
                ThoiGianDuyetNganSachTrungBinh = input.ThoiGianDuyetNganSachTrungBinh,
                SoBaoCaoTienDoChoDuyet = input.SoBaoCaoTienDoChoDuyet,
                SoBaoCaoTienDoBiTuChoi = input.SoBaoCaoTienDoBiTuChoi,
                SoBaoCaoTienDoYeuCauBoSung = input.SoBaoCaoTienDoYeuCauBoSung,
                TyLeBaoCaoTienDoBiTuChoi = input.TyLeBaoCaoTienDoBiTuChoi,
                SoLanCapNhatTienDo = input.SoLanCapNhatTienDo,
                SoNgayChamCapNhatTienDo = input.SoNgayChamCapNhatTienDo
            };
        }

        private static double NormalizeDelayRatio(double value)
        {
            if (value <= 0d)
            {
                return 0d;
            }

            var ratio = value > 1d ? value / 100d : value;
            return Math.Clamp(ratio, 0d, 1d);
        }

        private static string NormalizeReasonText(string? value)
            => TrangThai.Normalize(value).Replace(" ", string.Empty);

        private static bool LaNguyenNhanVuotNganSach(string? tenNguyenNhan)
        {
            var normalized = NormalizeReasonText(tenNguyenNhan);
            return normalized.Contains("vuotngansach")
                   || normalized.Contains("ngansach")
                   || normalized.Contains("chiphi");
        }

        private static bool LaVuotNganSachHopLe(AiProjectFeatureViewModel feature)
        {
            if (feature.ChiPhiThucTe <= feature.ChiPhiDuKien || feature.ChenhLechChiPhi <= 0d)
            {
                return false;
            }

            if (feature.ChiPhiDuKien <= 0d)
            {
                return feature.ChenhLechChiPhi > 0d;
            }

            var overrunRatio = (feature.ChiPhiThucTe - feature.ChiPhiDuKien) / feature.ChiPhiDuKien;
            return overrunRatio >= AiReasonHeuristic.HighCostOverrunThreshold;
        }

        private static AiReasonCatalogItemViewModel? TimNguyenNhanTheoTuKhoa(
            IReadOnlyCollection<AiReasonCatalogItemViewModel> danhMuc,
            params string[] tuKhoa)
        {
            foreach (var item in danhMuc)
            {
                var normalized = NormalizeReasonText(item.TenNguyenNhan);
                if (tuKhoa.Any(keyword => normalized.Contains(NormalizeReasonText(keyword))))
                {
                    return item;
                }
            }

            return null;
        }

        private static (AiReasonCatalogItemViewModel? Item, string DefaultReason, string NoiDungPhanTich) GoiYNguyenNhanTheoHeuristic(
            AiProjectFeatureViewModel feature,
            IReadOnlyCollection<AiReasonCatalogItemViewModel> danhMuc)
        {
            var tyLeTre = NormalizeDelayRatio(feature.TyLeCongViecTre);
            if (LaVuotNganSachHopLe(feature))
            {
                return (
                    TimNguyenNhanTheoTuKhoa(danhMuc, "Vượt ngân sách", "ngân sách", "chi phí"),
                    "Vượt ngân sách",
                    "Chi phí thực tế đang vượt ngưỡng ngân sách theo luật hậu kiểm."
                );
            }

            if (feature.SoLanThayDoiNhanSu >= AiReasonHeuristic.HighStaffChangeThreshold)
            {
                return (
                    TimNguyenNhanTheoTuKhoa(danhMuc, "Thiếu nhân sự", "Biến động nhân sự", "nhân sự"),
                    "Thiếu nhân sự",
                    "Số lần thay đổi nhân sự cao, hệ thống ưu tiên nhóm nguyên nhân nhân sự."
                );
            }

            if (feature.SoLanThayDoiQuanLy >= AiReasonHeuristic.HighManagerChangeThreshold)
            {
                return (
                    TimNguyenNhanTheoTuKhoa(danhMuc, "Quy trình xử lý chậm", "Thay đổi quản lý", "phê duyệt", "quản lý", "xử lý"),
                    "Quy trình xử lý chậm",
                    "Số lần thay đổi quản lý/phê duyệt cao, hệ thống ưu tiên nhóm nguyên nhân xử lý chậm."
                );
            }

            if (tyLeTre >= AiReasonHeuristic.SevereDelayRatioThreshold
                || feature.SoCongViecTre >= AiReasonHeuristic.SevereOverdueTasksThreshold)
            {
                return (
                    TimNguyenNhanTheoTuKhoa(danhMuc, "Phối hợp công việc chưa tốt", "Nhiều công việc trễ hạn", "công việc trễ", "phối hợp"),
                    "Nhiều công việc trễ hạn",
                    "Tỷ lệ công việc trễ cao, hệ thống ưu tiên nhóm nguyên nhân phụ thuộc/công việc trễ."
                );
            }

            if (feature.SoNgayTreTienDo >= AiReasonHeuristic.LongDelayDaysThreshold)
            {
                return (
                    TimNguyenNhanTheoTuKhoa(danhMuc, "Ước lượng thời gian chưa chính xác", "Trễ tiến độ kéo dài", "tiến độ"),
                    "Trễ tiến độ kéo dài",
                    "Số ngày trễ tiến độ cao, hệ thống ưu tiên nhóm nguyên nhân ước lượng/thời gian."
                );
            }

            if (tyLeTre >= AiReasonHeuristic.HighDelayRatioThreshold)
            {
                return (
                    TimNguyenNhanTheoTuKhoa(danhMuc, "Tiến độ cập nhật không đầy đủ", "cập nhật tiến độ", "tiến độ"),
                    "Tiến độ cập nhật không đầy đủ",
                    "Có tín hiệu trễ theo tỷ lệ công việc trễ, hệ thống ưu tiên nhóm nguyên nhân tiến độ."
                );
            }

            if (feature.SoCongViecTre > 0d)
            {
                return (
                    TimNguyenNhanTheoTuKhoa(danhMuc, "Thay đổi yêu cầu liên tục", "yêu cầu"),
                    "Thay đổi yêu cầu liên tục",
                    "Có công việc trễ cục bộ, hệ thống ưu tiên nhóm nguyên nhân thay đổi yêu cầu."
                );
            }

            return (
                TimNguyenNhanTheoTuKhoa(danhMuc, "Khác"),
                "Khác",
                "Kết quả AI không phù hợp với dữ liệu hiện tại."
            );
        }

        private static AiAnalyzeDelayReasonResponseViewModel HauKiemKetQuaNguyenNhan(
            AiAnalyzeDelayReasonResponseViewModel response,
            AiProjectFeatureViewModel feature,
            IReadOnlyCollection<AiReasonCatalogItemViewModel> danhMuc)
        {
            if (!LaNguyenNhanVuotNganSach(response.TenNguyenNhanDuDoan))
            {
                return response;
            }

            if (LaVuotNganSachHopLe(feature))
            {
                return response;
            }

            var fallback = GoiYNguyenNhanTheoHeuristic(feature, danhMuc);
            response.MaDMNguyenNhanDuDoan = fallback.Item != null
                && int.TryParse(fallback.Item.MaDMNguyenNhan, out var maDm)
                ? maDm
                : null;
            response.TenNguyenNhanDuDoan = fallback.Item?.TenNguyenNhan ?? fallback.DefaultReason;
            response.ReasonSource = "RuleFallback";
            response.CanhBaoNguyenNhan = string.IsNullOrWhiteSpace(response.CanhBaoNguyenNhan)
                ? "Kết quả AI không phù hợp với dữ liệu hiện tại, hệ thống đã chuyển sang luật gợi ý."
                : $"{response.CanhBaoNguyenNhan} Kết quả AI không phù hợp với dữ liệu hiện tại, hệ thống đã chuyển sang luật gợi ý.";
            response.NoiDungPhanTich = fallback.NoiDungPhanTich;
            return response;
        }

        private static AiTestReasonResponseViewModel HauKiemKetQuaTestNguyenNhan(
            AiTestReasonResponseViewModel response,
            AiProjectFeatureViewModel feature,
            IReadOnlyCollection<AiReasonCatalogItemViewModel> danhMuc)
        {
            if (!LaNguyenNhanVuotNganSach(response.SuggestedReason))
            {
                return response;
            }

            if (LaVuotNganSachHopLe(feature))
            {
                return response;
            }

            var fallback = GoiYNguyenNhanTheoHeuristic(feature, danhMuc);
            response.SuggestedReasonCode = fallback.Item != null
                && int.TryParse(fallback.Item.MaDMNguyenNhan, out var maDm)
                ? maDm
                : null;
            response.SuggestedReason = fallback.Item?.TenNguyenNhan ?? fallback.DefaultReason;
            response.ReasonSource = "RuleFallback";
            response.Explanation = "Kết quả AI không phù hợp với dữ liệu hiện tại. " + fallback.NoiDungPhanTich;
            return response;
        }

        private static List<string> BuildModelQualityWarnings(
            IReadOnlyCollection<AiModelVersionMetricViewModel> modelHistory,
            IReadOnlyDictionary<string, double> featureImportance,
            double? activeAccuracy,
            int activeTrainSize,
            int activeTestSize)
        {
            var warnings = new List<string>();
            var hasAnyModel = modelHistory.Count > 0 || activeTrainSize > 0 || activeTestSize > 0;
            if (!hasAnyModel)
            {
                return warnings;
            }

            if (activeTestSize > 0 && activeTestSize < 20)
            {
                warnings.Add("Kích thước tập kiểm thử đang nhỏ (TestSize < 20), cần thận trọng khi diễn giải chất lượng model.");
            }

            if (activeTestSize > 0 && activeTestSize < 20 && activeAccuracy.HasValue && activeAccuracy.Value >= 0.9999d)
            {
                warnings.Add("Độ chính xác chỉ mang tính tham khảo vì tập kiểm thử còn ít.");
            }

            if (featureImportance.Count > 0 && featureImportance.Values.Max() >= 0.8d)
            {
                warnings.Add("Model có thể đang phụ thuộc quá nhiều vào một đặc trưng.");
            }

            return warnings;
        }

        private async Task<(bool CanTongHopLai, DateTime? ThoiDiemDuLieuNghiepVuMoiNhat)> KiemTraCanTongHopLaiDatasetAsync(
            int maDuAn,
            DateTime? ngayTongHopDataset,
            CancellationToken cancellationToken)
        {
            var mocNghiepVuMoiNhat = await LayThoiDiemDuLieuNghiepVuMoiNhatAsync(maDuAn, cancellationToken);
            if (!mocNghiepVuMoiNhat.HasValue)
            {
                return (false, null);
            }

            if (!ngayTongHopDataset.HasValue)
            {
                return (true, mocNghiepVuMoiNhat);
            }

            return (mocNghiepVuMoiNhat.Value > ngayTongHopDataset.Value, mocNghiepVuMoiNhat);
        }

        private async Task<DateTime?> LayThoiDiemDuLieuNghiepVuMoiNhatAsync(int maDuAn, CancellationToken cancellationToken)
        {
            var mocDuAn = await _context.DuAn
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true)
                .Select(x => x.NgayTaoDuAn)
                .MaxAsync(cancellationToken);

            var mocNhatKyQuanLy = await _context.NhatKyQuanLyDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.NkThoiGianQLDA)
                .MaxAsync(cancellationToken);

            var mocNhatKyPhuTrach = await _context.NhatKyPhuTrachDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.NkThoiGianPTDA)
                .MaxAsync(cancellationToken);

            var mocNhatKyTeam = await _context.NhatKyDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.ThoiGianNKDA)
                .MaxAsync(cancellationToken);

            var mocYeuCauDoiQuanLy = await _context.YeuCauDoiQuanLy
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true)
                .Select(x => x.NgayDuyetYeuCauDoiQuanLy ?? x.NgayTaoYeuCauDoiQuanLy)
                .MaxAsync(cancellationToken);

            var mocCongViec = await (
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where cv.IsDeleted != true && dm.IsDeleted != true && dm.MaDuAn == maDuAn
                select cv.NgayKetThucCVThucTe ?? cv.NgayTaoCongViec ?? cv.NgayKetThucCVDuKien
            ).MaxAsync(cancellationToken);

            var mocTienDoCongViec = await (
                from td in _context.TienDoCongViec
                join ct in _context.CtCongViec on td.MaChiTietCV equals ct.MaChiTietCV
                join cv in _context.CongViec on ct.MaCongViec equals cv.MaCongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where ct.IsDeleted != true && cv.IsDeleted != true && dm.IsDeleted != true && dm.MaDuAn == maDuAn
                select td.ThoiGianDuyet ?? td.ThoiGianCapNhat
            ).MaxAsync(cancellationToken);

            var mocDeXuatCongViec = await _context.DeXuatCongViec
                .Where(x => x.IsDeleted != true && x.MaDuAn == maDuAn)
                .Select(x => x.NgayDuyetDeXuatCongViec ?? x.NgayDeXuatCongViec)
                .MaxAsync(cancellationToken);

            var mocDeXuatNganSach = await _context.DeXuatNganSach
                .Where(x => x.IsDeleted != true && x.MaDuAn == maDuAn)
                .Select(x => x.NgayDuyet ?? x.NgayDeXuat)
                .MaxAsync(cancellationToken);

            var mocNganSach = await _context.NganSach
                .Where(x => x.IsDeleted != true && x.MaDuAn == maDuAn)
                .Select(x => x.NgayCapNhatNganSach ?? x.NgayDuyetNganSach)
                .MaxAsync(cancellationToken);

            var mocChiPhi = await (
                from cp in _context.ChiPhi
                join ns in _context.NganSach on cp.MaNganSach equals ns.MaNganSach
                where cp.IsDeleted != true && ns.IsDeleted != true && ns.MaDuAn == maDuAn
                select cp.NgayChi
            ).MaxAsync(cancellationToken);

            var all = new DateTime?[]
            {
                mocDuAn,
                mocNhatKyQuanLy,
                mocNhatKyPhuTrach,
                mocNhatKyTeam,
                mocYeuCauDoiQuanLy,
                mocCongViec,
                mocTienDoCongViec,
                mocDeXuatCongViec,
                mocDeXuatNganSach,
                mocNganSach,
                mocChiPhi
            };

            var coGiaTri = all.Where(x => x.HasValue).Select(x => x!.Value).ToList();
            if (coGiaTri.Count == 0)
            {
                return null;
            }

            return coGiaTri.Max();
        }

        private static string DinhDangMocThoiGian(DateTime? value)
            => value.HasValue ? value.Value.ToString("yyyy-MM-dd HH:mm:ss") : "(không có)";

        private static List<AiReasonTrainDatasetItemViewModel> BuildReasonTrainDataset(IEnumerable<AiDatasetRowViewModel> rows)
        {
            return rows
                .Where(HasRequiredReasonTrainFields)
                .Where(x => x.LaDuAnTre == 1 && x.MaDMNguyenNhan.HasValue)
                .Select(x => new AiReasonTrainDatasetItemViewModel
                {
                    SoNhanVienDuAn = ToNullableInt(x.SoNhanVienDuAn),
                    TongSoCongViec = ToNullableInt(x.TongSoCongViec),
                    SoCongViecTre = ToNullableInt(x.SoCongViecTre),
                    TyLeCongViecTre = x.TyLeCongViecTre,
                    ChiPhiDuKien = ToNullableDecimal(x.ChiPhiDuKien),
                    ChiPhiThucTe = ToNullableDecimal(x.ChiPhiThucTe),
                    ChenhLechChiPhi = ToNullableDecimal(x.ChenhLechChiPhi),
                    SoLanThayDoiNhanSu = ToNullableInt(x.SoLanThayDoiNhanSu),
                    SoLanThayDoiQuanLy = ToNullableInt(x.SoLanThayDoiQuanLy),
                    SoNgayTreTienDo = ToNullableInt(x.SoNgayTreTienDo),
                    SoDeXuatCongViecChoDuyet = ToNullableInt(x.SoDeXuatCongViecChoDuyet),
                    SoDeXuatCongViecBiTuChoi = ToNullableInt(x.SoDeXuatCongViecBiTuChoi),
                    ThoiGianDuyetCongViecTrungBinh = x.ThoiGianDuyetCongViecTrungBinh,
                    SoDeXuatNganSachChoDuyet = ToNullableInt(x.SoDeXuatNganSachChoDuyet),
                    SoDeXuatNganSachBiTuChoi = ToNullableInt(x.SoDeXuatNganSachBiTuChoi),
                    ThoiGianDuyetNganSachTrungBinh = x.ThoiGianDuyetNganSachTrungBinh,
                    SoBaoCaoTienDoChoDuyet = ToNullableInt(x.SoBaoCaoTienDoChoDuyet),
                    SoBaoCaoTienDoBiTuChoi = ToNullableInt(x.SoBaoCaoTienDoBiTuChoi),
                    SoBaoCaoTienDoYeuCauBoSung = ToNullableInt(x.SoBaoCaoTienDoYeuCauBoSung),
                    TyLeBaoCaoTienDoBiTuChoi = x.TyLeBaoCaoTienDoBiTuChoi,
                    SoLanCapNhatTienDo = ToNullableInt(x.SoLanCapNhatTienDo),
                    SoNgayChamCapNhatTienDo = ToNullableInt(x.SoNgayChamCapNhatTienDo),
                    MaDMNguyenNhan = x.MaDMNguyenNhan!.Value
                })
                .ToList();
        }

        private async Task<List<AiTimelineItemViewModel>> LayLuotPhanTichAiTheoThangAsync(
            IQueryable<AiKetQua> aiKetQuaQuery,
            IQueryable<AiNguyenNhan> aiNguyenNhanQuery,
            CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var fromMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-11);
            var analysisRows = await aiKetQuaQuery
                .Where(x => x.ThoiGianDuDoanKetQua.HasValue && x.ThoiGianDuDoanKetQua.Value >= fromMonth)
                .Select(x => x.ThoiGianDuDoanKetQua!.Value)
                .ToListAsync(cancellationToken);
            var confirmRows = await aiNguyenNhanQuery
                .Where(x => x.NgayXacNhan.HasValue && x.NgayXacNhan.Value >= fromMonth)
                .Select(x => x.NgayXacNhan!.Value)
                .ToListAsync(cancellationToken);

            var result = new List<AiTimelineItemViewModel>();
            for (var i = 0; i < 12; i++)
            {
                var month = fromMonth.AddMonths(i);
                result.Add(new AiTimelineItemViewModel
                {
                    Label = month.ToString("MM/yyyy", CultureInfo.InvariantCulture),
                    SoLuotPhanTich = analysisRows.Count(x => x.Year == month.Year && x.Month == month.Month),
                    SoXacNhan = confirmRows.Count(x => x.Year == month.Year && x.Month == month.Month)
                });
            }

            return result;
        }

        private async Task<List<AiReasonGroupItemViewModel>> LayNguyenNhanTheoQuanLyAsync(
            IQueryable<AiNguyenNhan> aiNguyenNhanQuery,
            List<int> projectIds,
            bool gioiHanTheoScopeDuAn,
            CancellationToken cancellationToken)
        {
            var rows = await (
                from nn in aiNguyenNhanQuery
                join da in _context.DuAn on nn.MaDuAn equals da.MaDuAn
                join ql in _context.NguoiDung on da.MaNguoiDung equals ql.MaNguoiDung
                join dm in _context.DmNguyenNhan on nn.MaDMNguyenNhan equals dm.MaDMNguyenNhan
                where da.IsDeleted != true
                    && ql.IsDeleted != true
                    && (!gioiHanTheoScopeDuAn || projectIds.Contains(da.MaDuAn))
                group nn by new
                {
                    QuanLy = ql.HoTenNguoiDung,
                    ql.MaNguoiDung,
                    NguyenNhan = dm.TenNguyenNhan,
                    dm.MaDMNguyenNhan
                } into g
                select new
                {
                    g.Key.QuanLy,
                    g.Key.MaNguoiDung,
                    g.Key.NguyenNhan,
                    g.Key.MaDMNguyenNhan,
                    SoLan = g.Count()
                })
                .OrderByDescending(x => x.SoLan)
                .Take(8)
                .ToListAsync(cancellationToken);

            return rows.Select(x => new AiReasonGroupItemViewModel
            {
                Nhom = string.IsNullOrWhiteSpace(x.QuanLy) ? $"Quản lý {x.MaNguoiDung}" : x.QuanLy,
                NguyenNhan = string.IsNullOrWhiteSpace(x.NguyenNhan) ? $"Nguyên nhân {x.MaDMNguyenNhan}" : x.NguyenNhan,
                SoLan = x.SoLan
            }).ToList();
        }

        private async Task<List<AiReasonGroupItemViewModel>> LayNguyenNhanTheoTeamAsync(
            IQueryable<AiNguyenNhan> aiNguyenNhanQuery,
            List<int> projectIds,
            bool gioiHanTheoScopeDuAn,
            CancellationToken cancellationToken)
        {
            var rows = await (
                from nn in aiNguyenNhanQuery
                join td in _context.TeamDuAn on nn.MaDuAn equals td.MaDuAn
                join team in _context.Team on td.MaTeam equals team.MaTeam
                join dm in _context.DmNguyenNhan on nn.MaDMNguyenNhan equals dm.MaDMNguyenNhan
                where team.IsDeleted != true
                    && (!gioiHanTheoScopeDuAn || projectIds.Contains(nn.MaDuAn))
                group nn by new
                {
                    Team = team.TenTeam,
                    team.MaTeam,
                    NguyenNhan = dm.TenNguyenNhan,
                    dm.MaDMNguyenNhan
                } into g
                select new
                {
                    g.Key.Team,
                    g.Key.MaTeam,
                    g.Key.NguyenNhan,
                    g.Key.MaDMNguyenNhan,
                    SoLan = g.Count()
                })
                .OrderByDescending(x => x.SoLan)
                .Take(8)
                .ToListAsync(cancellationToken);

            return rows.Select(x => new AiReasonGroupItemViewModel
            {
                Nhom = string.IsNullOrWhiteSpace(x.Team) ? $"Team {x.MaTeam}" : x.Team,
                NguyenNhan = string.IsNullOrWhiteSpace(x.NguyenNhan) ? $"Nguyên nhân {x.MaDMNguyenNhan}" : x.NguyenNhan,
                SoLan = x.SoLan
            }).ToList();
        }

        private static bool HasRequiredReasonTrainFields(AiDatasetRowViewModel row)
        {
            return row.SoNhanVienDuAn.HasValue
                && row.TongSoCongViec.HasValue
                && row.SoCongViecTre.HasValue
                && row.TyLeCongViecTre.HasValue
                && row.ChiPhiDuKien.HasValue
                && row.ChiPhiThucTe.HasValue
                && row.ChenhLechChiPhi.HasValue
                && row.SoLanThayDoiNhanSu.HasValue
                && row.SoLanThayDoiQuanLy.HasValue
                && row.SoNgayTreTienDo.HasValue
                && row.SoDeXuatCongViecChoDuyet.HasValue
                && row.SoDeXuatCongViecBiTuChoi.HasValue
                && row.ThoiGianDuyetCongViecTrungBinh.HasValue
                && row.SoDeXuatNganSachChoDuyet.HasValue
                && row.SoDeXuatNganSachBiTuChoi.HasValue
                && row.ThoiGianDuyetNganSachTrungBinh.HasValue
                && row.SoBaoCaoTienDoChoDuyet.HasValue
                && row.SoBaoCaoTienDoBiTuChoi.HasValue
                && row.SoBaoCaoTienDoYeuCauBoSung.HasValue
                && row.TyLeBaoCaoTienDoBiTuChoi.HasValue
                && row.SoLanCapNhatTienDo.HasValue
                && row.SoNgayChamCapNhatTienDo.HasValue
                && row.MaDMNguyenNhan.HasValue;
        }

        private static int? ToNullableInt(double? value)
            => value.HasValue
                ? (int)Math.Round(value.Value, MidpointRounding.AwayFromZero)
                : null;

        private static decimal? ToNullableDecimal(double? value)
            => value.HasValue
                ? Convert.ToDecimal(value.Value, CultureInfo.InvariantCulture)
                : null;

        private async Task<List<int>> GetAccessibleProjectIdsAsync(
            int currentUserId,
            (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags,
            CancellationToken cancellationToken)
        {
            if (roleFlags.IsAdmin)
            {
                return [];
            }

            var projectIds = new HashSet<int>();
            if (roleFlags.IsManager)
            {
                var managed = await _context.DuAn
                    .Where(x => x.IsDeleted != true && x.MaNguoiDung == currentUserId)
                    .Select(x => x.MaDuAn)
                    .ToListAsync(cancellationToken);
                foreach (var id in managed)
                {
                    projectIds.Add(id);
                }
            }

            if (roleFlags.IsEmployee || roleFlags.IsManager)
            {
                var joined = await _context.NhanVienDuAn
                    .Where(x => x.MaNguoiDung == currentUserId)
                    .Select(x => x.MaDuAn)
                    .ToListAsync(cancellationToken);
                foreach (var id in joined)
                {
                    projectIds.Add(id);
                }
            }

            return projectIds.ToList();
        }

        private async Task<int> GetCurrentUserIdAsync(CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(aspUserId))
            {
                throw new Exception("Không xác định được người dùng hiện tại.");
            }

            var maNguoiDung = await _context.Aspnetusers
                .Where(x => x.Id == aspUserId)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync(cancellationToken);
            if (maNguoiDung <= 0)
            {
                throw new Exception("Không xác định được nhân sự hiện tại.");
            }

            return maNguoiDung;
        }

        private async Task<(bool IsAdmin, bool IsManager, bool IsEmployee)> GetCurrentUserRoleFlagsAsync(CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(aspUserId))
            {
                throw new Exception("Không xác định được người dùng hiện tại.");
            }

            var roleNames = await (
                from ur in _context.Aspnetuserroles
                join r in _context.Aspnetroles on ur.Id equals r.Id
                where ur.Asp_Id == aspUserId
                select (r.NormalizedName ?? r.Name) ?? string.Empty
            ).ToListAsync(cancellationToken);

            var normalized = roleNames
                .Select(x => x.Trim().ToUpperInvariant())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet();

            return (
                normalized.Contains("ADMIN"),
                normalized.Contains("MANAGER"),
                normalized.Contains("EMPLOYEE"));
        }

        private static string NormalizeModelType(string? modelType)
            => string.Equals(modelType?.Trim(), ModelTypeNguyenNhan, StringComparison.OrdinalIgnoreCase)
                ? ModelTypeNguyenNhan
                : ModelTypeNguyenNhan;

        private static double ChuanHoaDoTinCay(double? doTinCay)
        {
            if (!doTinCay.HasValue)
            {
                return 0;
            }

            var value = doTinCay.Value;
            if (value > 1d && value <= 100d)
            {
                value /= 100d;
            }

            return Math.Round(Math.Clamp(value, 0d, 1d), 4);
        }

        private static double ChuanHoaTyLeCongViecTre(double value)
        {
            var normalized = value <= 1d ? value * 100d : value;
            return Math.Round(Math.Clamp(normalized, 0d, 100d), 4);
        }

        private static Dictionary<string, double> ReadFeatureImportance(Dictionary<string, object?> metadata)
        {
            if (!metadata.TryGetValue("feature_importance", out var raw)
                && !metadata.TryGetValue("featureImportance", out raw))
            {
                return [];
            }

            if (raw is not JsonElement element || element.ValueKind != JsonValueKind.Object)
            {
                return [];
            }

            var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Number && prop.Value.TryGetDouble(out var number))
                {
                    result[prop.Name] = number;
                }
            }

            return result;
        }

        private static List<List<int>> ReadConfusionMatrix(Dictionary<string, object?> metadata)
        {
            if (!metadata.TryGetValue("confusion_matrix", out var raw)
                && !metadata.TryGetValue("confusionMatrix", out raw))
            {
                return [];
            }

            if (raw is not JsonElement element || element.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            var matrix = new List<List<int>>();
            foreach (var row in element.EnumerateArray())
            {
                if (row.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                var rowValues = new List<int>();
                foreach (var value in row.EnumerateArray())
                {
                    if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var number))
                    {
                        rowValues.Add(number);
                    }
                }

                if (rowValues.Count > 0)
                {
                    matrix.Add(rowValues);
                }
            }

            return matrix;
        }

        private static List<int> ReadConfusionMatrixLabels(Dictionary<string, object?> metadata)
        {
            if (!metadata.TryGetValue("confusion_matrix_labels", out var raw)
                && !metadata.TryGetValue("confusionMatrixLabels", out raw))
            {
                return [];
            }

            if (raw is not JsonElement element || element.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            var labels = new List<int>();
            foreach (var value in element.EnumerateArray())
            {
                if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var number))
                {
                    labels.Add(number);
                    continue;
                }

                if (value.ValueKind == JsonValueKind.String
                    && int.TryParse(value.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                {
                    labels.Add(parsed);
                }
            }

            return labels;
        }

        private static int TryReadInt(Dictionary<string, object?> metadata, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (!metadata.TryGetValue(key, out var raw) || raw is not JsonElement element)
                {
                    continue;
                }

                if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var number))
                {
                    return number;
                }
            }

            return 0;
        }

        private static double? TryReadDouble(Dictionary<string, object?> metadata, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (!metadata.TryGetValue(key, out var raw) || raw is not JsonElement element)
                {
                    continue;
                }

                if (element.ValueKind == JsonValueKind.Number && element.TryGetDouble(out var number))
                {
                    return number;
                }

                if (element.ValueKind == JsonValueKind.String
                    && double.TryParse(element.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
                {
                    return parsed;
                }
            }

            return null;
        }

        private static string? TryReadString(Dictionary<string, object?> metadata, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (!metadata.TryGetValue(key, out var raw) || raw is not JsonElement element)
                {
                    continue;
                }

                if (element.ValueKind == JsonValueKind.String)
                {
                    var value = element.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return value.Trim();
                    }
                }
            }

            return null;
        }

        private static string CatChuoi(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var text = value.Trim();
            return text.Length <= maxLength ? text : text[..maxLength];
        }
    }
}

