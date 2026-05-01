using QuanLyDuAn.ViewModels.DanhMucCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDanhMucCongViecService
    {
        Task<List<DanhMucCongViecViewModel>> GetAllAsync(string? tuKhoa, int? maDuAn);
        Task<DanhMucCongViecCreateUpdateViewModel?> GetByIdAsync(int id);
        Task<List<DuAnOptionViewModel>> GetDuAnOptionsAsync();
        Task SaveAsync(DanhMucCongViecCreateUpdateViewModel model);
        Task DeleteAsync(int id);
    }
}
