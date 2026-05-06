using QuanLyDuAn.ViewModels.ChiTietCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IChiTietCongViecService
    {
        Task<ChiTietCongViecPageViewModel> GetPageAsync(int maCongViec);
        Task AddAsync(ChiTietCongViecCreateUpdateViewModel model);
        Task UpdateAsync(ChiTietCongViecCreateUpdateViewModel model);
        Task RemoveAsync(int maCongViec, int maChiTietCv);
    }
}
