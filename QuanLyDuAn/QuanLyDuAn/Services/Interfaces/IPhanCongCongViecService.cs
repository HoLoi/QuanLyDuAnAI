using QuanLyDuAn.ViewModels.PhanCongCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IPhanCongCongViecService
    {
        Task<PhanCongCongViecPageViewModel> GetPageAsync(int maCongViec);

        Task AddAsync(int maCongViec, int maNhanVien, DateTime? tuNgay, DateTime? denNgay);

        Task RemoveAsync(int maCongViec, int maNguoiDung);
    }
}
