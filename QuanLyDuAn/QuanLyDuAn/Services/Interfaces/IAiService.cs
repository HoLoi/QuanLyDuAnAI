using QuanLyDuAn.ViewModels.Ai;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IAiService
    {
        Task<AiDashboardPageViewModel> LayDashboardAsync(CancellationToken cancellationToken = default);

        Task<AiTrainPageViewModel> LayTrangTrainAsync(CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiTrainResponseViewModel>> TrainAsync(string modelType, bool activateAfterTrain, string? trainNote, CancellationToken cancellationToken = default);

        Task<AiModelPageViewModel> LayTrangModelAsync(string? modelDangXem, string? boLocLoaiModel, CancellationToken cancellationToken = default, int pageNumber = 1, int pageSize = 20);
        Task<AiOperationResultViewModel<AiValidateModelResponseViewModel>> ValidateModelAsync(string modelFile, string modelType, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiCompareModelResponseViewModel>> CompareModelAsync(string currentModelFile, string newModelFile, string modelType, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiModelActivationResultViewModel>> DatModelHoatDongAsync(string modelFile, string modelType, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiModelReloadResultViewModel>> TaiLaiModelAsync(string modelType, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiModelDeleteResultViewModel>> XoaModelAsync(string modelFile, CancellationToken cancellationToken = default);

        Task<AiProjectDelayAnalysisPanelViewModel> LayPhanTichNguyenNhanDuAnAsync(int maDuAn, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiAnalyzeDelayReasonResponseViewModel>> PhanTichNguyenNhanDuAnAsync(int maDuAn, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<AiTestReasonResponseViewModel>> TestPredictAsync(AiPredictPageViewModel input, string? modelFile, CancellationToken cancellationToken = default);
        Task<AiOperationResultViewModel<bool>> XacNhanNguyenNhanAsync(int maDuAn, string maDmNguyenNhan, double? doTinCay, CancellationToken cancellationToken = default);
    }
}
