using QuanLyDuAn.ViewModels.Ai;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IAiDatasetService
    {
        Task<AiDatasetPageViewModel> KhoiTaoTrangDatasetAsync(
            bool dangThieuDataset,
            bool baoGomBaoCaoChatLuong,
            CancellationToken cancellationToken = default);

        Task<AiDatasetTongHopResultViewModel> TongHopDatasetAsync(CancellationToken cancellationToken = default);
        Task<AiDatasetTongHopResultViewModel> TongHopDatasetChoDuAnAsync(int maDuAn, CancellationToken cancellationToken = default);
        Task<AiProjectFeatureSnapshotViewModel?> BuildFeatureSnapshotAsync(int maDuAn, CancellationToken cancellationToken = default);
        Task<AiDatasetQualitySummaryViewModel> KiemTraChatLuongDatasetAsync(CancellationToken cancellationToken = default);
        Task<List<AiDatasetRowViewModel>> LayDatasetHopLeDeTrainAsync(CancellationToken cancellationToken = default);
        Task<AiReasonTrainingQualitySummaryViewModel> KiemTraChatLuongDatasetNguyenNhanAsync(CancellationToken cancellationToken = default);
        Task<List<AiDatasetRowViewModel>> LayDatasetNguyenNhanHopLeDeTrainAsync(CancellationToken cancellationToken = default);
    }
}
