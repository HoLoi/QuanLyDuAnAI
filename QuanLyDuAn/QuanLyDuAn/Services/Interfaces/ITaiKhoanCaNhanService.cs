using System.Security.Claims;
using QuanLyDuAn.ViewModels.TaiKhoanCaNhan;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface ITaiKhoanCaNhanService
    {
        Task<TaiKhoanCaNhanViewModel> GetHoSoAsync(ClaimsPrincipal user);
        Task<CapNhatTaiKhoanCaNhanViewModel> GetCapNhatAsync(ClaimsPrincipal user);
        Task CapNhatAsync(ClaimsPrincipal user, CapNhatTaiKhoanCaNhanViewModel model);
        Task DoiMatKhauAsync(ClaimsPrincipal user, DoiMatKhauViewModel model);
    }
}
