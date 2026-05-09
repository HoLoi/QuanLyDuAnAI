using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Services.Implementations
{
    public class FileTienDoCongViecService : IFileTienDoCongViecService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ITienDoCongViecService _tienDoCongViecService;

        public FileTienDoCongViecService(
            QuanLyDuAnDbContext context,
            IWebHostEnvironment environment,
            ITienDoCongViecService tienDoCongViecService)
        {
            _context = context;
            _environment = environment;
            _tienDoCongViecService = tienDoCongViecService;
        }

        public async Task UploadAsync(int maChiTietCv, IFormFile file)
        {
            throw new Exception("Tệp minh chứng chỉ được tải lên cùng lúc gửi báo cáo tiến độ. Vui lòng tạo báo cáo mới.");
        }

        public async Task DeleteAsync(int maFileTdcv)
        {
            throw new Exception("Không cho phép xóa tệp minh chứng của báo cáo tiến độ đã gửi.");
        }

        public async Task<(string FullPath, string FileName, int MaChiTietCV)> GetDownloadInfoAsync(int maFileTdcv)
        {
            var entity = await _context.FileTienDoCongViec
                .FirstOrDefaultAsync(x => x.MaFileTDCV == maFileTdcv && x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy tệp tiến độ cần tải.");

            var tienDo = await _context.TienDoCongViec
                .FirstOrDefaultAsync(x => x.MaTienDo == entity.MaTienDo);

            if (tienDo == null)
                throw new Exception("Không tìm thấy báo cáo tiến độ liên quan.");

            var currentUserId = await GetCurrentUserIdAsync();
            var coTheXem = await CoTheXemAsync(tienDo.MaChiTietCV, currentUserId);
            if (!coTheXem)
                throw new Exception("Bạn không có quyền tải tệp minh chứng này.");

            if (string.IsNullOrWhiteSpace(entity.DuongDanFileTDCV))
                throw new Exception("Tệp tiến độ không hợp lệ.");

            var physicalPath = GetPhysicalPath(entity.DuongDanFileTDCV);
            if (string.IsNullOrWhiteSpace(physicalPath) || !File.Exists(physicalPath))
                throw new Exception("Tệp không tồn tại trên hệ thống.");

            var fileName = string.IsNullOrWhiteSpace(entity.TenFileTDCV)
                ? $"tep_tien_do_{entity.MaFileTDCV}"
                : entity.TenFileTDCV;

            return (physicalPath, fileName, tienDo.MaChiTietCV);
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            if (_tienDoCongViecService is TienDoCongViecService typed)
                return await typed.GetCurrentUserIdForFileAsync();

            throw new Exception("Không xác định được dịch vụ tiến độ hiện tại.");
        }

        private async Task<bool> CoTheXemAsync(int maChiTietCv, int currentUserId)
        {
            if (_tienDoCongViecService is TienDoCongViecService typed)
                return await typed.CoTheXemTienDoChiTietAsync(maChiTietCv, currentUserId);

            return false;
        }

        private string GetWebRootPath()
        {
            if (!string.IsNullOrWhiteSpace(_environment.WebRootPath))
                return _environment.WebRootPath;

            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        private string GetPhysicalPath(string? duongDan)
        {
            if (string.IsNullOrWhiteSpace(duongDan))
                return string.Empty;

            var relativePath = duongDan.TrimStart('~').TrimStart('/', '\\');
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(GetWebRootPath(), relativePath);
        }
    }
}
