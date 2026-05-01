using QuanLyDuAn.ViewModels.ChucDanh;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IChucDanhService
    {
        Task<List<ChucDanhViewModel>> GetAllAsync();
        Task<ChucDanhCreateUpdateViewModel?> GetByIdAsync(int id);
        Task SaveAsync(ChucDanhCreateUpdateViewModel model);
        Task DeleteAsync(int id);
    }
}
