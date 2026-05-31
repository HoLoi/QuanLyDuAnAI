using QuanLyDuAn.ViewModels.Ai;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IAiApiService
    {
        Task<AiOperationResultViewModel<AiHealthResponseViewModel>> KiemTraSucKhoeAsync(CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiAdminStatusViewModel>> LayTrangThaiAdminAsync(CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiSystemInfoViewModel>> LayThongTinHeThongAsync(CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiLogSummaryViewModel>> LayTongHopLogAsync(CancellationToken cancellationToken = default);

        Task<AiOperationResultViewModel<AiDatasetValidateResponseViewModel>> ValidateDatasetAsync(AiDatasetValidateRequestViewModel request, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiDatasetQualityReportViewModel>> BaoCaoChatLuongDatasetAsync(AiDatasetValidateRequestViewModel request, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiTrainRecommendationViewModel>> KhuyenNghiTrainAsync(AiDatasetValidateRequestViewModel request, CancellationToken cancellationToken = default);

        Task<AiOperationResultViewModel<AiTrainResponseViewModel>> TrainModelAsync(AiTrainRequestViewModel request, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<List<AiModelInfoViewModel>>> LayDanhSachModelAsync(string? modelType = null, CancellationToken cancellationToken = default, bool includeAliases = false);
        Task<AiOperationResultViewModel<Dictionary<string, object?>>> LayChiTietModelAsync(string modelFile, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiValidateModelResponseViewModel>> ValidateModelAsync(string modelFile, string? modelType, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiCompareModelResponseViewModel>> CompareModelAsync(AiCompareModelRequestViewModel request, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiModelActivationResultViewModel>> DatModelHoatDongAsync(string modelFile, string modelType, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiModelReloadResultViewModel>> TaiLaiModelHoatDongAsync(string modelType, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiModelDeleteResultViewModel>> XoaModelAsync(string modelFile, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<Dictionary<string, object?>>> ExportMetadataAsync(string modelFile, CancellationToken cancellationToken = default);

        Task<AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>> DuDoanDuAnAsync(AiAnalyzeDelayReasonRequestViewModel request, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiTestReasonResponseViewModel>> TestPredictAsync(AiTestReasonRequestViewModel request, CancellationToken cancellationToken = default);
    }
}
