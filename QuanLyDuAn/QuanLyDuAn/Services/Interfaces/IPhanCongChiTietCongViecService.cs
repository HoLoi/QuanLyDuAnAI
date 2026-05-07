using QuanLyDuAn.ViewModels.PhanCongChiTietCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IPhanCongChiTietCongViecService
    {
        Task<PhanCongChiTietCongViecPageViewModel> GetPageAsync(int maChiTietCv);

        Task AddAsync(PhanCongChiTietCongViecCreateViewModel input);

        Task RemoveAsync(int maChiTietCv, int maNguoiDung);
    }
}
