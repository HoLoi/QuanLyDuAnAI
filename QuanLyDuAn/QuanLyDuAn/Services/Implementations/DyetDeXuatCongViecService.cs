using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DuyetDeXuatCongViec;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class DyetDeXuatCongViecService : IDuyetDeXuatCongViecService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DyetDeXuatCongViecService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DuyetDeXuatCongViecPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai)
        {
            var query =
                from dx in _context.DeXuatCongViec
                join da in _context.DuAn on dx.MaDuAn equals da.MaDuAn
                join ndDeXuat in _context.NguoiDung on dx.MaNguoiDungDeXuat equals ndDeXuat.MaNguoiDung
                where dx.IsDeleted != true && da.IsDeleted != true
                select new DuyetDeXuatCongViecItemViewModel
                {
                    MaDeXuatCV = dx.MaDeXuatCV,
                    MaDuAn = dx.MaDuAn,
                    TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}",
                    MaNguoiDungDeXuat = dx.MaNguoiDungDeXuat,
                    NguoiDungDeXuat = ndDeXuat.HoTenNguoiDung ?? $"Nhân viên {dx.MaNguoiDungDeXuat}",
                    TenCongViecDeXuat = dx.TenCongViecDeXuat ?? string.Empty,
                    MoTaCongViecDeXuat = dx.MoTaCongViecDeXuat ?? string.Empty,
                    ChiPhiDeXuat = dx.ChiPhiDeXuat,
                    NgayBatDauCongViecDeXuat = dx.NgayBatDauCongViecDeXuat,
                    NgayKetThucCVDeXuatDuKien = dx.NgayKetThucCVDeXuatDuKien,
                    NgayDeXuatCongViec = dx.NgayDeXuatCongViec,
                    TrangThaiCongViecDeXuat = dx.TrangThaiCongViecDeXuat ?? string.Empty
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
                    query = query.Where(x => filterValues.Contains(x.TrangThaiCongViecDeXuat));
                }
            }

            return new DuyetDeXuatCongViecPageViewModel
            {
                DanhSach = await query
                    .OrderByDescending(x => x.NgayDeXuatCongViec)
                    .ThenByDescending(x => x.MaDeXuatCV)
                    .ToListAsync(),
                LocMaDuAn = locMaDuAn,
                LocTrangThai = locTrangThai
            };
        }

        public async Task ApproveAsync(int maDeXuatCv)
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var deXuat = await _context.DeXuatCongViec
                .FirstOrDefaultAsync(x => x.MaDeXuatCV == maDeXuatCv && x.IsDeleted != true);

            if (deXuat == null)
                throw new Exception("Không tìm thấy đề xuất công việc.");

            if (!IsPending(deXuat.TrangThaiCongViecDeXuat))
                throw new Exception("Đề xuất công việc không còn ở trạng thái chờ duyệt.");

            var danhMucHopLe = await _context.DanhMucCongViec
                .AnyAsync(x => x.MaDanhMucCV == deXuat.MaDanhMucCV && x.MaDuAn == deXuat.MaDuAn && x.IsDeleted != true);

            if (!danhMucHopLe)
                throw new Exception("Danh mục công việc của đề xuất không còn hợp lệ.");

            var mucDoHopLe = await _context.MucDoUuTien
                .AnyAsync(x => x.MaMucDo == deXuat.MaMucDo);

            if (!mucDoHopLe)
                throw new Exception("Mức độ ưu tiên của đề xuất không còn hợp lệ.");

            var nganSachHienTai = await _context.NganSach
                .Where(x => x.MaDuAn == deXuat.MaDuAn && x.IsDeleted != true && x.IsActive == true)
                .OrderByDescending(x => x.Version)
                .ThenByDescending(x => x.NgayCapNhatNganSach)
                .FirstOrDefaultAsync();

            if (nganSachHienTai == null)
                throw new Exception("Dự án chưa có ngân sách hiện hành để duyệt đề xuất công việc.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var congViec = new CongViec
            {
                MaDeXuatCV = deXuat.MaDeXuatCV,
                MaDanhMucCV = deXuat.MaDanhMucCV,
                MaMucDo = deXuat.MaMucDo,
                TenCongViec = deXuat.TenCongViecDeXuat,
                MoTaCongViec = deXuat.MoTaCongViecDeXuat,
                NgayBatDauCongViec = deXuat.NgayBatDauCongViecDeXuat,
                NgayKetThucCVDuKien = deXuat.NgayKetThucCVDeXuatDuKien,
                NgayTaoCongViec = DateTime.Now,
                TrangThaiCongViec = TrangThai.ChuaBatDau,
                IsDeleted = false
            };

            _context.CongViec.Add(congViec);

            var chiPhi = new ChiPhi
            {
                MaCongViec = 0,
                MaNganSach = nganSachHienTai.MaNganSach,
                NoiDungChiPhi = $"Chi phí từ đề xuất công việc #{deXuat.MaDeXuatCV}",
                SoTienDaChi = deXuat.ChiPhiDeXuat,
                NgayChi = DateTime.Now,
                TrangThaiChiPhi = TrangThai.DaDuyet,
                IsDeleted = false
            };

            await _context.SaveChangesAsync();

            chiPhi.MaCongViec = congViec.MaCongViec;
            _context.ChiPhi.Add(chiPhi);

            deXuat.TrangThaiCongViecDeXuat = TrangThai.DaDuyet;
            deXuat.MaNguoiDungDuyet = currentUserId;
            deXuat.NgayDuyetDeXuatCongViec = DateTime.Now;

            await _context.SaveChangesAsync();

            _context.NhatKyChiPhi.Add(new NhatKyChiPhi
            {
                MaCongViec = congViec.MaCongViec,
                MaChiPhi = chiPhi.MaChiPhi,
                NkSoTienDaChi = chiPhi.SoTienDaChi,
                NkNgayChi = chiPhi.NgayChi,
                NkTrangThaiChiPhi = chiPhi.TrangThaiChiPhi,
                HanhDongNKCP = $"Duyệt đề xuất công việc #{deXuat.MaDeXuatCV} và tạo chi phí liên quan",
                ThoiGianNKCP = DateTime.Now
            });

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = deXuat.MaDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Duyệt đề xuất công việc #{deXuat.MaDeXuatCV}",
                NkThoiGianQLDA = DateTime.Now
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task RejectAsync(int maDeXuatCv, string? lyDo)
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var deXuat = await _context.DeXuatCongViec
                .FirstOrDefaultAsync(x => x.MaDeXuatCV == maDeXuatCv && x.IsDeleted != true);

            if (deXuat == null)
                throw new Exception("Không tìm thấy đề xuất công việc.");

            if (!IsPending(deXuat.TrangThaiCongViecDeXuat))
                throw new Exception("Đề xuất công việc không còn ở trạng thái chờ duyệt.");

            deXuat.TrangThaiCongViecDeXuat = TrangThai.TuChoi;
            deXuat.MaNguoiDungDuyet = currentUserId;
            deXuat.NgayDuyetDeXuatCongViec = DateTime.Now;

            var lyDoTuChoi = string.IsNullOrWhiteSpace(lyDo)
                ? string.Empty
                : $". Lý do: {lyDo.Trim()}";

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = deXuat.MaDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Từ chối đề xuất công việc #{deXuat.MaDeXuatCV}{lyDoTuChoi}",
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
