using QuanLyDuAn.ViewModels.YeuCauDoiQuanLy;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IYeuCauDoiQuanLyService
    {
        Task<YeuCauDoiQuanLyPageViewModel> GetPageAsync(
            int? maDuAn,
            string? trangThai,
            string? tuKhoa,
            int pageNumber = 1,
            int pageSize = 20,
            bool paginate = true);
        Task CreateAsync(YeuCauDoiQuanLyCreateViewModel model);
        Task CancelAsync(int id);
        Task<bool> CanCreateFromProjectAsync(int maDuAn);
    }
}
