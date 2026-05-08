using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DuAn;

namespace QuanLyDuAn.Services.Implementations
{
    public class FileDuAnService : IFileDuAnService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public FileDuAnService(QuanLyDuAnDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<List<DuAnFileItemViewModel>> GetByDuAnAsync(int maDuAn)
        {
            return await _context.FileDuAn
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true)
                .OrderByDescending(x => x.NgayUploadFileDA ?? DateTime.MinValue)
                .Select(x => new DuAnFileItemViewModel
                {
                    MaFileDA = x.MaFileDA,
                    TenFileDA = x.TenFileDA ?? $"File {x.MaFileDA}",
                    DuongDanFileDA = x.DuongDanFileDA ?? string.Empty,
                    NgayUploadFileDA = x.NgayUploadFileDA
                })
                .ToListAsync();
        }

        public async Task<DuAnFileItemViewModel?> GetByIdAsync(int maFileDa)
        {
            return await _context.FileDuAn
                .Where(x => x.MaFileDA == maFileDa && x.IsDeleted != true)
                .Select(x => new DuAnFileItemViewModel
                {
                    MaFileDA = x.MaFileDA,
                    TenFileDA = x.TenFileDA ?? $"File {x.MaFileDA}",
                    DuongDanFileDA = x.DuongDanFileDA ?? string.Empty,
                    NgayUploadFileDA = x.NgayUploadFileDA
                })
                .FirstOrDefaultAsync();
        }

        public async Task UploadAsync(int maDuAn, IFormFile file, int currentUserId)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Vui lòng chọn tệp cần tải lên.");

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (duAn == null)
                throw new Exception("Không tìm thấy dự án.");

            if (duAn.MaNguoiDung != currentUserId)
                throw new Exception("Bạn không có quyền thao tác dự án này.");

            var fileName = Path.GetFileName(file.FileName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "tep_du_an";

            var extension = Path.GetExtension(fileName);
            var savedFileName = $"{Guid.NewGuid():N}{extension}";
            var folderPath = Path.Combine(GetWebRootPath(), "uploads", "duan", maDuAn.ToString());
            Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, savedFileName);
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var entity = new FileDuAn
            {
                MaDuAn = maDuAn,
                TenFileDA = fileName,
                DuongDanFileDA = $"/uploads/duan/{maDuAn}/{savedFileName}",
                NgayUploadFileDA = DateTime.Now,
                IsDeleted = false
            };

            _context.FileDuAn.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maFileDa, int currentUserId)
        {
            var entity = await _context.FileDuAn
                .FirstOrDefaultAsync(x => x.MaFileDA == maFileDa && x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy tệp cần xóa.");

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == entity.MaDuAn && x.IsDeleted != true);

            if (duAn == null)
                throw new Exception("Không tìm thấy dự án.");

            if (duAn.MaNguoiDung != currentUserId)
                throw new Exception("Bạn không có quyền thao tác dự án này.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;
            entity.DeletedBy = currentUserId;

            var physicalPath = GetPhysicalPath(entity.DuongDanFileDA);
            if (!string.IsNullOrWhiteSpace(physicalPath) && File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<(string FullPath, string FileName, int MaDuAn)> GetDownloadInfoAsync(int maFileDa)
        {
            var entity = await _context.FileDuAn
                .FirstOrDefaultAsync(x => x.MaFileDA == maFileDa && x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy tệp cần tải.");

            if (string.IsNullOrWhiteSpace(entity.DuongDanFileDA))
                throw new Exception("Tệp dự án không hợp lệ.");

            var physicalPath = GetPhysicalPath(entity.DuongDanFileDA);
            if (string.IsNullOrWhiteSpace(physicalPath) || !File.Exists(physicalPath))
                throw new Exception("Tệp không tồn tại trên hệ thống.");

            var downloadName = string.IsNullOrWhiteSpace(entity.TenFileDA)
                ? $"tep_du_an_{entity.MaFileDA}"
                : entity.TenFileDA;

            return (physicalPath, downloadName, entity.MaDuAn);
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
