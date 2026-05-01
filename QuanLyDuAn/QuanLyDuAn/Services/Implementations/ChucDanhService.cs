using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.ChucDanh;

namespace QuanLyDuAn.Services.Implementations
{
    public class ChucDanhService : IChucDanhService
    {
        private readonly QuanLyDuAnDbContext _context;

        public ChucDanhService(QuanLyDuAnDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChucDanhViewModel>> GetAllAsync()
        {
            return await _context.ChucDanh
                .Select(x => new ChucDanhViewModel
                {
                    MaChucDanh = x.MaChucDanh,
                    TenChucDanh = x.TenChucDanh ?? string.Empty,
                    MoTaChucDanh = x.MoTaChucDanh
                })
                .ToListAsync();
        }

        public async Task<ChucDanhCreateUpdateViewModel?> GetByIdAsync(int id)
        {
            return await _context.ChucDanh
                .Where(x => x.MaChucDanh == id)
                .Select(x => new ChucDanhCreateUpdateViewModel
                {
                    MaChucDanh = x.MaChucDanh,
                    TenChucDanh = x.TenChucDanh ?? string.Empty,
                    MoTaChucDanh = x.MoTaChucDanh ?? string.Empty
                })
                .FirstOrDefaultAsync();
        }

        public async Task SaveAsync(ChucDanhCreateUpdateViewModel model)
        {
            var tenChucDanh = (model.TenChucDanh ?? string.Empty).Trim();
            var moTaChucDanh = (model.MoTaChucDanh ?? string.Empty).Trim();

            var tenChucDanhUpper = tenChucDanh.ToUpperInvariant();
            var biTrung = await _context.ChucDanh.AnyAsync(x =>
                x.TenChucDanh != null &&
                x.TenChucDanh.ToUpper() == tenChucDanhUpper &&
                (model.MaChucDanh == null || x.MaChucDanh != model.MaChucDanh.Value));

            if (biTrung)
                throw new Exception("Tên chức danh đã tồn tại.");

            if (model.MaChucDanh == null)
            {
                var entity = new ChucDanh
                {
                    TenChucDanh = tenChucDanh,
                    MoTaChucDanh = moTaChucDanh
                };

                _context.ChucDanh.Add(entity);
            }
            else
            {
                var entity = await _context.ChucDanh
                    .FirstOrDefaultAsync(x => x.MaChucDanh == model.MaChucDanh);

                if (entity == null)
                    throw new Exception("Không tìm thấy chức danh");

                entity.TenChucDanh = tenChucDanh;
                entity.MoTaChucDanh = moTaChucDanh;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int maChucDanh)
        {
            bool dangDuocDung = await _context.NguoiDung
                .AnyAsync(x => x.MaChucDanh == maChucDanh);

            if (dangDuocDung)
                throw new Exception("Không thể xóa: chức danh đang được sử dụng bởi nhân sự.");

            var entity = await _context.ChucDanh.FindAsync(maChucDanh);

            if (entity == null)
                throw new Exception("Không tìm thấy");

            _context.ChucDanh.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
