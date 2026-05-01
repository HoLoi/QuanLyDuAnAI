using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DanhMucCongViec;

namespace QuanLyDuAn.Services.Implementations
{
    public class DanhMucCongViecService : IDanhMucCongViecService
    {
        private readonly QuanLyDuAnDbContext _context;

        public DanhMucCongViecService(QuanLyDuAnDbContext context)
        {
            _context = context;
        }

        public async Task<List<DanhMucCongViecViewModel>> GetAllAsync(string? tuKhoa, int? maDuAn)
        {
            var query = (
                from dm in _context.DanhMucCongViec
                join da in _context.DuAn on dm.MaDuAn equals da.MaDuAn
                where dm.IsDeleted != true && da.IsDeleted != true
                select new DanhMucCongViecViewModel
                {
                    MaDanhMucCV = dm.MaDanhMucCV,
                    MaDuAn = dm.MaDuAn,
                    TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}",
                    TenDanhMucCV = dm.TenDanhMucCV ?? string.Empty,
                    MoTaDanhMucCV = dm.MoTaDanhMucCV,
                    NgayTaoDMCV = dm.NgayTaoDMCV,
                    SoLuongCongViec = _context.CongViec.Count(cv =>
                        cv.MaDanhMucCV == dm.MaDanhMucCV && cv.IsDeleted != true)
                })
                .OrderByDescending(x => x.MaDanhMucCV)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLowerInvariant();
                query = query.Where(x =>
                    x.TenDanhMucCV.ToLower().Contains(keyword) ||
                    (x.MoTaDanhMucCV != null && x.MoTaDanhMucCV.ToLower().Contains(keyword)) ||
                    x.TenDuAn.ToLower().Contains(keyword));
            }

            if (maDuAn.HasValue)
            {
                query = query.Where(x => x.MaDuAn == maDuAn.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<DanhMucCongViecCreateUpdateViewModel?> GetByIdAsync(int id)
        {
            return await _context.DanhMucCongViec
                .Where(x => x.MaDanhMucCV == id && x.IsDeleted != true)
                .Select(x => new DanhMucCongViecCreateUpdateViewModel
                {
                    MaDanhMucCV = x.MaDanhMucCV,
                    MaDuAn = x.MaDuAn,
                    TenDanhMucCV = x.TenDanhMucCV ?? string.Empty,
                    MoTaDanhMucCV = x.MoTaDanhMucCV ?? string.Empty
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<DuAnOptionViewModel>> GetDuAnOptionsAsync()
        {
            return await _context.DuAn
                .Where(x => x.IsDeleted != true)
                .OrderBy(x => x.TenDuAn)
                .Select(x => new DuAnOptionViewModel
                {
                    MaDuAn = x.MaDuAn,
                    TenDuAn = x.TenDuAn ?? $"Dự án {x.MaDuAn}"
                })
                .ToListAsync();
        }

        public async Task SaveAsync(DanhMucCongViecCreateUpdateViewModel model)
        {
            if (!model.MaDuAn.HasValue || model.MaDuAn.Value <= 0)
                throw new Exception("Vui lòng chọn dự án hợp lệ.");

            var maDuAn = model.MaDuAn.Value;
            var tenDanhMuc = (model.TenDanhMucCV ?? string.Empty).Trim();
            var moTaDanhMuc = (model.MoTaDanhMucCV ?? string.Empty).Trim();

            var duAnTonTai = await _context.DuAn
                .AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (!duAnTonTai)
                throw new Exception("Dự án không tồn tại hoặc đã bị xóa.");

            var tenDanhMucUpper = tenDanhMuc.ToUpperInvariant();
            var biTrung = await _context.DanhMucCongViec.AnyAsync(x =>
                x.IsDeleted != true &&
                x.MaDuAn == maDuAn &&
                x.TenDanhMucCV != null &&
                x.TenDanhMucCV.ToUpper() == tenDanhMucUpper &&
                (model.MaDanhMucCV == null || x.MaDanhMucCV != model.MaDanhMucCV.Value));

            if (biTrung)
                throw new Exception("Tên danh mục công việc đã tồn tại trong dự án này.");

            if (model.MaDanhMucCV == null)
            {
                var entity = new DanhMucCongViec
                {
                    MaDuAn = maDuAn,
                    TenDanhMucCV = tenDanhMuc,
                    MoTaDanhMucCV = moTaDanhMuc,
                    NgayTaoDMCV = DateTime.Now,
                    IsDeleted = false
                };

                _context.DanhMucCongViec.Add(entity);
            }
            else
            {
                var entity = await _context.DanhMucCongViec
                    .FirstOrDefaultAsync(x => x.MaDanhMucCV == model.MaDanhMucCV && x.IsDeleted != true);

                if (entity == null)
                    throw new Exception("Không tìm thấy danh mục công việc.");

                if (entity.MaDuAn != maDuAn)
                {
                    var dangCoCongViec = await _context.CongViec
                        .AnyAsync(x => x.MaDanhMucCV == entity.MaDanhMucCV && x.IsDeleted != true);

                    if (dangCoCongViec)
                        throw new Exception("Không thể đổi dự án vì danh mục đã có công việc phát sinh.");
                }

                entity.MaDuAn = maDuAn;
                entity.TenDanhMucCV = tenDanhMuc;
                entity.MoTaDanhMucCV = moTaDanhMuc;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dangDuocSuDung = await _context.CongViec
                .AnyAsync(x => x.MaDanhMucCV == id && x.IsDeleted != true);

            if (dangDuocSuDung)
                throw new Exception("Không thể xóa: danh mục công việc đang có công việc liên kết.");

            var entity = await _context.DanhMucCongViec
                .FirstOrDefaultAsync(x => x.MaDanhMucCV == id && x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy danh mục công việc.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
