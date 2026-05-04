using QuanLyDuAn.ViewModels.ChiTietCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IChiTietCongViecService
    {
        Task<ChiTietCongViecPageViewModel> GetPageAsync(int maCongViec);
        Task AddAsync(ChiTietCongViecCreateUpdateViewModel form);
        Task UpdateAsync(ChiTietCongViecCreateUpdateViewModel form);
        Task RemoveAsync(int maCongViec, int maChiTietCv);
    }
}
