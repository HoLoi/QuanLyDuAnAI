using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DuyetDeXuatNganSach;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class DuyetDeXuatNganSachService : IDuyetDeXuatNganSachService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DuyetDeXuatNganSachService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DuyetDeXuatNganSachPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai)
        {
            var query =
                from dx in _context.DeXuatNganSach
                join da in _context.DuAn on dx.MaDuAn equals da.MaDuAn
                join nd in _context.NguoiDung on dx.MaNguoiDungDeXuat equals nd.MaNguoiDung
                where dx.IsDeleted != true && da.IsDeleted != true
                select new DuyetDeXuatNganSachItemViewModel
                {
                    MaDeXuatNS = dx.MaDeXuatNS,
                    MaDuAn = dx.MaDuAn,
                    TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}",
                    NganSachCu = dx.NganSachCu,
                    NganSachDeXuat = dx.NganSachDeXuat,
                    LyDoDeXuat = dx.LyDoDeXuat ?? string.Empty,
                    MaNguoiDungDeXuat = dx.MaNguoiDungDeXuat,
                    NguoiDungDeXuat = nd.HoTenNguoiDung ?? $"Nhân viên {dx.MaNguoiDungDeXuat}",
                    NgayDeXuat = dx.NgayDeXuat,
                    TrangThaiDeXuat = dx.TrangThaiDeXuat ?? string.Empty
                };

            if (locMaDuAn.HasValue)
            {
                query = query.Where(x => x.MaDuAn == locMaDuAn.Value);
            }

            if (!string.IsNullOrWhiteSpace(locTrangThai))
            {
                var filterValues = TrangThai.GetCommonStatusVariants(locTrangThai);
                if (filterValues.Length > 0)
                {
                    query = query.Where(x => filterValues.Contains(x.TrangThaiDeXuat));
                }
            }

            return new DuyetDeXuatNganSachPageViewModel
            {
                DanhSach = await query
                    .OrderByDescending(x => x.NgayDeXuat)
                    .ThenByDescending(x => x.MaDeXuatNS)
                    .ToListAsync(),
                LocMaDuAn = locMaDuAn,
                LocTrangThai = locTrangThai
            };
        }

        public async Task ApproveAsync(int maDeXuatNs)
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var deXuat = await _context.DeXuatNganSach
                .FirstOrDefaultAsync(x => x.MaDeXuatNS == maDeXuatNs && x.IsDeleted != true);

            if (deXuat == null)
                throw new Exception("Không tìm thấy đề xuất ngân sách.");

            if (!IsPending(deXuat.TrangThaiDeXuat))
                throw new Exception("Đề xuất ngân sách không còn ở trạng thái chờ duyệt.");

            NganSach? nganSachCu = null;

            if (deXuat.MaNganSachCu.HasValue)
            {
                nganSachCu = await _context.NganSach
                    .FirstOrDefaultAsync(x => x.MaNganSach == deXuat.MaNganSachCu && x.IsDeleted != true);

                if (nganSachCu == null)
                    throw new Exception("Không tìm thấy ngân sách hiện hành gắn với đề xuất.");
            }

            var maxVersion = await _context.NganSach
                .Where(x => x.MaDuAn == deXuat.MaDuAn && x.IsDeleted != true)
                .MaxAsync(x => (int?)x.Version) ?? 0;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (nganSachCu != null)
            {
                nganSachCu.IsActive = false;
                nganSachCu.TrangThaiNganSach = TrangThai.DaThayThe;
                nganSachCu.NgayCapNhatNganSach = DateTime.Now;
            }

            var nganSachMoi = new NganSach
            {
                MaNguoiDungDuyet = currentUserId,
                MaNguoiDungDeXuat = deXuat.MaNguoiDungDeXuat,
                MaDuAn = deXuat.MaDuAn,
                SoTienNganSach = deXuat.NganSachDeXuat,
                Version = maxVersion + 1,
                IsActive = true,
                MoTaNganSach = $"Ngân sách từ duyệt đề xuất #{deXuat.MaDeXuatNS}",
                NgayCapNhatNganSach = DateTime.Now,
                NgayDuyetNganSach = DateTime.Now,
                TrangThaiNganSach = TrangThai.DaDuyet,
                IsDeleted = false
            };

            _context.NganSach.Add(nganSachMoi);

            deXuat.TrangThaiDeXuat = TrangThai.DaDuyet;
            deXuat.MaNguoiDungDuyet = currentUserId;
            deXuat.NgayDuyet = DateTime.Now;

            await _context.SaveChangesAsync();

            _context.NhatKyNganSach.Add(new NhatKyNganSach
            {
                MaNganSach = nganSachMoi.MaNganSach,
                MaDuAn = deXuat.MaDuAn,
                SoTienNKNS = deXuat.NganSachDeXuat,
                NganSachTruoc = deXuat.NganSachCu,
                NganSachSau = deXuat.NganSachDeXuat,
                NkNgayCapNhatNS = DateTime.Now,
                NkTrangThaiNganSach = TrangThai.DaDuyet,
                HanhDongNKNS = $"Duyệt đề xuất ngân sách #{deXuat.MaDeXuatNS}",
                ThoiGianNKNS = DateTime.Now
            });

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = deXuat.MaDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Duyệt đề xuất ngân sách #{deXuat.MaDeXuatNS}",
                NkThoiGianQLDA = DateTime.Now
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task RejectAsync(int maDeXuatNs, string? lyDo)
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var deXuat = await _context.DeXuatNganSach
                .FirstOrDefaultAsync(x => x.MaDeXuatNS == maDeXuatNs && x.IsDeleted != true);

            if (deXuat == null)
                throw new Exception("Không tìm thấy đề xuất ngân sách.");

            if (!IsPending(deXuat.TrangThaiDeXuat))
                throw new Exception("Đề xuất ngân sách không còn ở trạng thái chờ duyệt.");

            deXuat.TrangThaiDeXuat = TrangThai.TuChoi;
            deXuat.MaNguoiDungDuyet = currentUserId;
            deXuat.NgayDuyet = DateTime.Now;

            if (deXuat.MaNganSachCu.HasValue)
            {
                _context.NhatKyNganSach.Add(new NhatKyNganSach
                {
                    MaNganSach = deXuat.MaNganSachCu.Value,
                    MaDuAn = deXuat.MaDuAn,
                    SoTienNKNS = deXuat.NganSachDeXuat,
                    NganSachTruoc = deXuat.NganSachCu,
                    NganSachSau = deXuat.NganSachCu,
                    NkNgayCapNhatNS = DateTime.Now,
                    NkTrangThaiNganSach = TrangThai.TuChoi,
                    HanhDongNKNS = string.IsNullOrWhiteSpace(lyDo)
                        ? $"Từ chối đề xuất ngân sách #{deXuat.MaDeXuatNS}"
                        : $"Từ chối đề xuất ngân sách #{deXuat.MaDeXuatNS}. Lý do: {lyDo.Trim()}",
                    ThoiGianNKNS = DateTime.Now
                });
            }

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = deXuat.MaDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = string.IsNullOrWhiteSpace(lyDo)
                    ? $"Từ chối đề xuất ngân sách #{deXuat.MaDeXuatNS}"
                    : $"Từ chối đề xuất ngân sách #{deXuat.MaDeXuatNS}. Lý do: {lyDo.Trim()}",
                NkThoiGianQLDA = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(aspUserId))
                throw new Exception("Không xác định được người dùng hiện tại.");

            var maNguoiDung = await _context.Aspnetusers
                .Where(x => x.Id == aspUserId)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync();

            if (maNguoiDung <= 0)
                throw new Exception("Không xác định được nhân sự tương ứng của người dùng hiện tại.");

            return maNguoiDung;
        }

        private static bool IsPending(string? status)
        {
            return TrangThai.EqualsValue(status, TrangThai.ChoDuyet);
        }
    }
}
