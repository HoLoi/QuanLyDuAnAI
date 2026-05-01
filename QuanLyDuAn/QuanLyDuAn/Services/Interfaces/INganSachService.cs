using QuanLyDuAn.ViewModels.NganSach;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface INganSachService
    {
        Task<NganSachPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai);
    }
}
