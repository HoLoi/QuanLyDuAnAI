using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.DanhMucCongViec;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDanhMucCongViecService
    {
        Task<PagedResultViewModel<DanhMucCongViecViewModel>> GetPagedAsync(ClaimsPrincipal user, string? tuKhoa, int maDuAn, int pageNumber = 1, int pageSize = 20);
        Task<DanhMucCongViecCreateUpdateViewModel?> GetByIdAsync(int id);
        Task<int?> GetMaDuAnByDanhMucIdAsync(int id);
        Task<List<DuAnOptionViewModel>> GetDuAnOptionsAsync(ClaimsPrincipal user, string permission);
        Task SaveAsync(ClaimsPrincipal user, DanhMucCongViecCreateUpdateViewModel model);
        Task DeleteAsync(ClaimsPrincipal user, int id);
    }
}
