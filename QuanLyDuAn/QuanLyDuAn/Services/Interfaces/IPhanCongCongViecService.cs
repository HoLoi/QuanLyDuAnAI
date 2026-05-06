using QuanLyDuAn.ViewModels.PhanCongCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IPhanCongCongViecService
    {
        Task<PhanCongCongViecPageViewModel> GetPageAsync(int maCongViec);

        Task AddAsync(PhanCongCongViecCreateViewModel input);

        Task RemoveAsync(int maCongViec, int maNguoiDung);
    }
}
