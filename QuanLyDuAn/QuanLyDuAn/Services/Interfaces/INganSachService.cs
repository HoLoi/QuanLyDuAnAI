using QuanLyDuAn.ViewModels.NganSach;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface INganSachService
    {
        Task<NganSachPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai, int pageNumber = 1, int pageSize = 20, bool paginate = true);
    }
}
