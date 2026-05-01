using QuanLyDuAn.ViewModels.CongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface ICongViecService
    {
        Task<CongViecPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai, string? tuKhoa);
    }
}
