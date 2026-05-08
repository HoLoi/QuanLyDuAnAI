using Microsoft.AspNetCore.Http;
using QuanLyDuAn.ViewModels.DuAn;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IFileDuAnService
    {
        Task<List<DuAnFileItemViewModel>> GetByDuAnAsync(int maDuAn);
        Task<DuAnFileItemViewModel?> GetByIdAsync(int maFileDa);
        Task UploadAsync(int maDuAn, IFormFile file, int currentUserId);
        Task DeleteAsync(int maFileDa, int currentUserId);
        Task<(string FullPath, string FileName, int MaDuAn)> GetDownloadInfoAsync(int maFileDa);
    }
}
