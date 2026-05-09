using Microsoft.AspNetCore.Http;
using QuanLyDuAn.ViewModels.TienDoCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IFileTienDoCongViecService
    {
        Task UploadAsync(int maChiTietCv, IFormFile file);

        Task DeleteAsync(int maFileTdcv);

        Task<(string FullPath, string FileName, int MaChiTietCV)> GetDownloadInfoAsync(int maFileTdcv);
    }
}
