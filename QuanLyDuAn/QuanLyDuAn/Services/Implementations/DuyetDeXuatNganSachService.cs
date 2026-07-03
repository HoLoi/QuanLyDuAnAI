using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.DuyetDeXuatNganSach;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        public async Task<DuyetDeXuatNganSachPageViewModel> GetPageAsync(
            int? locMaDuAn,
            string? locTrangThai,
            int? locMaNguoiDungDeXuat,
            DateTime? tuNgay,
            DateTime? denNgay,
            decimal? tuSoTien,
            decimal? denSoTien,
            string? tuKhoa,
            int pageNumber = 1,
            int pageSize = PaginationViewModel.DefaultPageSize,
            bool paginate = true)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var (tuNgayLoc, denNgayLoc) = ChuanHoaKhoangNgay(tuNgay, denNgay);
            var (tuSoTienLoc, denSoTienLoc) = ChuanHoaKhoangTien(tuSoTien, denSoTien);
            var tuKhoaLoc = string.IsNullOrWhiteSpace(tuKhoa) ? null : tuKhoa.Trim();
            var danhSachDuAn = await GetManagedProjectOptionsAsync(currentUserId);
            var danhSachNguoiDeXuat = await GetRequesterOptionsAsync(currentUserId);

            var query =
                from dx in _context.DeXuatNganSach
                join da in _context.DuAn on dx.MaDuAn equals da.MaDuAn
                join nd in _context.NguoiDung on dx.MaNguoiDungDeXuat equals nd.MaNguoiDung
                join ndDuyetLeft in _context.NguoiDung on dx.MaNguoiDungDuyet equals ndDuyetLeft.MaNguoiDung into ndDuyetGroup
                from ndDuyet in ndDuyetGroup.DefaultIfEmpty()
                where dx.IsDeleted != true
                      && da.IsDeleted != true
                      && da.MaNguoiDung == currentUserId
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
                    NguoiDungDuyet = ndDuyet != null ? (ndDuyet.HoTenNguoiDung ?? $"Nhân viên {ndDuyet.MaNguoiDung}") : string.Empty,
                    NgayDeXuat = dx.NgayDeXuat,
                    NgayDuyet = dx.NgayDuyet,
                    TrangThaiDeXuat = dx.TrangThaiDeXuat ?? string.Empty
                };

            if (locMaDuAn.HasValue)
            {
                query = query.Where(x => x.MaDuAn == locMaDuAn.Value);
            }

            if (locMaNguoiDungDeXuat.HasValue)
            {
                query = query.Where(x => x.MaNguoiDungDeXuat == locMaNguoiDungDeXuat.Value);
            }

            if (!string.IsNullOrWhiteSpace(locTrangThai))
            {
                var filterValues = TrangThai.GetCommonStatusVariants(locTrangThai);
                if (filterValues.Length > 0)
                {
                    query = query.Where(x => filterValues.Contains(x.TrangThaiDeXuat));
                }
            }

            if (tuNgayLoc.HasValue)
            {
                query = query.Where(x =>
                    x.NgayDeXuat.HasValue &&
                    x.NgayDeXuat.Value >= tuNgayLoc.Value);
            }

            if (denNgayLoc.HasValue)
            {
                var denNgayDocQuyen = denNgayLoc.Value.AddDays(1);
                query = query.Where(x =>
                    x.NgayDeXuat.HasValue &&
                    x.NgayDeXuat.Value < denNgayDocQuyen);
            }

            if (tuSoTienLoc.HasValue)
            {
                query = query.Where(x =>
                    x.NganSachDeXuat.HasValue &&
                    x.NganSachDeXuat.Value >= tuSoTienLoc.Value);
            }

            if (denSoTienLoc.HasValue)
            {
                query = query.Where(x =>
                    x.NganSachDeXuat.HasValue &&
                    x.NganSachDeXuat.Value <= denSoTienLoc.Value);
            }

            if (!string.IsNullOrWhiteSpace(tuKhoaLoc))
            {
                var keyword = tuKhoaLoc.ToLower();
                query = query.Where(x =>
                    x.LyDoDeXuat.ToLower().Contains(keyword) ||
                    x.TenDuAn.ToLower().Contains(keyword) ||
                    x.NguoiDungDeXuat.ToLower().Contains(keyword));
            }

            var totalItems = await query.CountAsync();
            var pagination = PaginationViewModel.Create(pageNumber, pageSize, totalItems);
            IQueryable<DuyetDeXuatNganSachItemViewModel> danhSachQuery = query
                .OrderByDescending(x => x.NgayDeXuat)
                .ThenByDescending(x => x.MaDeXuatNS);

            if (paginate)
            {
                danhSachQuery = danhSachQuery
                    .Skip(pagination.Skip)
                    .Take(pagination.PageSize);
            }

            return new DuyetDeXuatNganSachPageViewModel
            {
                DanhSach = await danhSachQuery.ToListAsync(),
                DanhSachDuAn = danhSachDuAn,
                DanhSachNguoiDeXuat = danhSachNguoiDeXuat,
                Pagination = pagination,
                LocMaDuAn = locMaDuAn,
                LocTrangThai = locTrangThai,
                LocMaNguoiDungDeXuat = locMaNguoiDungDeXuat,
                TuNgay = tuNgayLoc,
                DenNgay = denNgayLoc,
                TuSoTien = tuSoTienLoc,
                DenSoTien = denSoTienLoc,
                TuKhoa = tuKhoaLoc
            };
        }

        private async Task<List<DuyetDeXuatNganSachSelectOptionViewModel>> GetManagedProjectOptionsAsync(int currentUserId)
        {
            var rawOptions = await _context.DuAn
                .Where(x => x.IsDeleted != true && x.MaNguoiDung == currentUserId)
                .OrderBy(x => x.TenDuAn)
                .Select(x => new
                {
                    x.MaDuAn,
                    x.TenDuAn
                })
                .ToListAsync();

            return rawOptions
                .Select(x => new DuyetDeXuatNganSachSelectOptionViewModel
                {
                    Value = x.MaDuAn,
                    Text = string.IsNullOrWhiteSpace(x.TenDuAn) ? $"Dự án {x.MaDuAn}" : x.TenDuAn
                })
                .OrderBy(x => x.Text)
                .ToList();
        }

        private async Task<List<DuyetDeXuatNganSachSelectOptionViewModel>> GetRequesterOptionsAsync(int currentUserId)
        {
            var rawOptions = await (
                from dx in _context.DeXuatNganSach
                join da in _context.DuAn on dx.MaDuAn equals da.MaDuAn
                join nd in _context.NguoiDung on dx.MaNguoiDungDeXuat equals nd.MaNguoiDung
                where dx.IsDeleted != true
                      && da.IsDeleted != true
                      && da.MaNguoiDung == currentUserId
                      && nd.IsDeleted != true
                select new
                {
                    nd.MaNguoiDung,
                    nd.HoTenNguoiDung
                })
                .Distinct()
                .ToListAsync();

            return rawOptions
                .Select(x => new DuyetDeXuatNganSachSelectOptionViewModel
                {
                    Value = x.MaNguoiDung,
                    Text = string.IsNullOrWhiteSpace(x.HoTenNguoiDung) ? $"Người dùng {x.MaNguoiDung}" : x.HoTenNguoiDung
                })
                .OrderBy(x => x.Text)
                .ToList();
        }

        public async Task ApproveAsync(int maDeXuatNs)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var deXuat = await _context.DeXuatNganSach
                .FirstOrDefaultAsync(x => x.MaDeXuatNS == maDeXuatNs && x.IsDeleted != true);

            if (deXuat == null)
                throw new Exception("Không tìm thấy đề xuất ngân sách.");

            if (!IsPending(deXuat.TrangThaiDeXuat))
                throw new Exception("Đề xuất ngân sách không còn ở trạng thái chờ duyệt.");

            await EnsureIsProjectManagerAsync(currentUserId, deXuat.MaDuAn);
            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == deXuat.MaDuAn && x.IsDeleted != true);

            if (duAn == null)
                throw new Exception("Không tìm thấy dự án của đề xuất ngân sách.");

            if (TrangThai.LaHoanThanhCongViec(duAn.TrangThaiDuAn)
                || TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.LuuTru)
                || TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.DaHuy))
            {
                throw new Exception("Dự án đã hoàn thành hoặc đã đóng, không thể duyệt đề xuất ngân sách.");
            }

            var tongChiPhiDaDung = await (
                from cp in _context.ChiPhi
                join ns in _context.NganSach on cp.MaNganSach equals ns.MaNganSach
                where cp.IsDeleted != true
                      && ns.IsDeleted != true
                      && ns.MaDuAn == deXuat.MaDuAn
                select cp.SoTienDaChi ?? 0
            ).SumAsync();

            var nganSachDeXuat = deXuat.NganSachDeXuat ?? 0;
            if (nganSachDeXuat < tongChiPhiDaDung)
                throw new Exception($"Ngân sách đề xuất không được nhỏ hơn tổng chi phí đã dùng ({tongChiPhiDaDung:N0} VNĐ).");

            var nganSachDangActive = await _context.NganSach
                .Where(x => x.MaDuAn == deXuat.MaDuAn && x.IsDeleted != true && x.IsActive == true)
                .ToListAsync();

            if (deXuat.MaNganSachCu.HasValue
                && !nganSachDangActive.Any(x => x.MaNganSach == deXuat.MaNganSachCu.Value))
            {
                throw new Exception("Không tìm thấy ngân sách hiện hành gắn với đề xuất.");
            }

            var maxVersion = await _context.NganSach
                .Where(x => x.MaDuAn == deXuat.MaDuAn && x.IsDeleted != true)
                .MaxAsync(x => (int?)x.Version) ?? 0;

            foreach (var nganSachCu in nganSachDangActive)
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

        public async Task RejectAsync(int maDeXuatNs)
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var deXuat = await _context.DeXuatNganSach
                .FirstOrDefaultAsync(x => x.MaDeXuatNS == maDeXuatNs && x.IsDeleted != true);

            if (deXuat == null)
                throw new Exception("Không tìm thấy đề xuất ngân sách.");

            if (!IsPending(deXuat.TrangThaiDeXuat))
                throw new Exception("Đề xuất ngân sách không còn ở trạng thái chờ duyệt.");

            await EnsureIsProjectManagerAsync(currentUserId, deXuat.MaDuAn);

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
                    HanhDongNKNS = $"Từ chối đề xuất ngân sách #{deXuat.MaDeXuatNS}",
                    ThoiGianNKNS = DateTime.Now
                });
            }

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = deXuat.MaDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Từ chối đề xuất ngân sách #{deXuat.MaDeXuatNS}",
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

        private static (DateTime? TuNgay, DateTime? DenNgay) ChuanHoaKhoangNgay(DateTime? tuNgay, DateTime? denNgay)
        {
            var tu = tuNgay?.Date;
            var den = denNgay?.Date;

            if (tu.HasValue && den.HasValue && tu.Value > den.Value)
            {
                (tu, den) = (den, tu);
            }

            return (tu, den);
        }

        private static (decimal? TuSoTien, decimal? DenSoTien) ChuanHoaKhoangTien(decimal? tuSoTien, decimal? denSoTien)
        {
            var tu = tuSoTien.HasValue && tuSoTien.Value >= 0 ? tuSoTien : null;
            var den = denSoTien.HasValue && denSoTien.Value >= 0 ? denSoTien : null;

            if (tu.HasValue && den.HasValue && tu.Value > den.Value)
            {
                (tu, den) = (den, tu);
            }

            return (tu, den);
        }

        private async Task EnsureIsProjectManagerAsync(int maNguoiDung, int maDuAn)
        {
            var isProjectManager = await _context.DuAn.AnyAsync(x =>
                x.MaDuAn == maDuAn &&
                x.IsDeleted != true &&
                x.MaNguoiDung == maNguoiDung);

            if (!isProjectManager)
                throw new Exception("Bạn không có quyền duyệt đề xuất cho dự án này.");
        }
    }
}
