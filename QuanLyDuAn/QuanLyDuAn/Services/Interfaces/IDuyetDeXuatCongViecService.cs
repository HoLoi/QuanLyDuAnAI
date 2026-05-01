using QuanLyDuAn.ViewModels.DuyetDeXuatCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDuyetDeXuatCongViecService
    {
        Task<DuyetDeXuatCongViecPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai);
        Task ApproveAsync(int maDeXuatCv);
        Task RejectAsync(int maDeXuatCv, string? lyDo);
    }
}
