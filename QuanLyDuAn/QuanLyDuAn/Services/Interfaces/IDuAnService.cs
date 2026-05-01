using QuanLyDuAn.ViewModels.DuAn;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDuAnService
    {
        Task<List<DuAnViewModel>> GetAllAsync(string? tuKhoa, int? maLoaiDuAn, string? trangThaiDuAn);
        Task<DuAnCreateUpdateViewModel?> GetByIdAsync(int id);
        Task<List<LoaiDuAnOptionViewModel>> GetLoaiDuAnOptionsAsync();
        Task SaveAsync(DuAnCreateUpdateViewModel model);
        Task DeleteAsync(int id);

        // Status workflow methods
        Task<ProjectStatusCheckViewModel> CheckProjectStatusAsync(int maDuAn);
        Task CheckAutoTransitionAsync(int maDuAn);
        Task CheckManagerPermissionAsync(int maDuAn, int currentUserId);
        Task ValidateDeleteAsync(int maDuAn);
        Task ValidateCompletionAsync(int maDuAn);
        Task TransitionToDangThucHienAsync(int maDuAn);
        Task RequestCompletionAsync(int maDuAn);
        Task ConfirmCompletionAsync(int maDuAn);
        Task PauseProjectAsync(int maDuAn, string ghiChuDuAn);
    }
}
