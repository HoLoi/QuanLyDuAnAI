using QuanLyDuAn.ViewModels.DeXuatCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDeXuatCongViecService
    {
        Task<DeXuatCongViecPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai);
        Task CreateAsync(DeXuatCongViecCreateViewModel model);
        Task CancelAsync(int maDeXuatCv);
    }
}
