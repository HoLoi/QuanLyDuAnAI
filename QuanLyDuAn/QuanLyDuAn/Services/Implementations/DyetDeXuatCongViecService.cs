using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.DuyetDeXuatCongViec;
using System.Data;
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

        public async Task<DuyetDeXuatCongViecPageViewModel> GetPageAsync(
            int? locMaDuAn,
            string? locTrangThai,
            int? locMaNguoiDungDeXuat,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? tuKhoa,
            int pageNumber = 1,
            int pageSize = PaginationViewModel.DefaultPageSize,
            bool paginate = true)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var (tuNgayLoc, denNgayLoc) = ChuanHoaKhoangNgay(tuNgay, denNgay);
            var tuKhoaLoc = string.IsNullOrWhiteSpace(tuKhoa) ? null : tuKhoa.Trim();
            var danhSachDuAn = await GetManagedProjectOptionsAsync(currentUserId);
            var danhSachNguoiDeXuat = await GetRequesterOptionsAsync(currentUserId);

            var query =
                from dx in _context.DeXuatCongViec
                join da in _context.DuAn on dx.MaDuAn equals da.MaDuAn
                join ndDeXuat in _context.NguoiDung on dx.MaNguoiDungDeXuat equals ndDeXuat.MaNguoiDung
                join ndDuyetLeft in _context.NguoiDung on dx.MaNguoiDungDuyet equals ndDuyetLeft.MaNguoiDung into ndDuyetGroup
                from ndDuyet in ndDuyetGroup.DefaultIfEmpty()
                join dmcvLeft in _context.DanhMucCongViec on dx.MaDanhMucCV equals dmcvLeft.MaDanhMucCV into dmcvGroup
                from dmcv in dmcvGroup.DefaultIfEmpty()
                join mdLeft in _context.MucDoUuTien on dx.MaMucDo equals mdLeft.MaMucDo into mdGroup
                from md in mdGroup.DefaultIfEmpty()
                where dx.IsDeleted != true
                      && da.IsDeleted != true
                      && da.MaNguoiDung == currentUserId
                select new DuyetDeXuatCongViecItemViewModel
                {
                    MaDeXuatCV = dx.MaDeXuatCV,
                    MaDuAn = dx.MaDuAn,
                    TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}",
                    MaNguoiDungDeXuat = dx.MaNguoiDungDeXuat,
                    NguoiDungDeXuat = ndDeXuat.HoTenNguoiDung ?? $"Nhân viên {dx.MaNguoiDungDeXuat}",
                    NguoiDungDuyet = ndDuyet != null ? (ndDuyet.HoTenNguoiDung ?? $"Nhân viên {ndDuyet.MaNguoiDung}") : string.Empty,
                    TenCongViecDeXuat = dx.TenCongViecDeXuat ?? string.Empty,
                    MoTaCongViecDeXuat = dx.MoTaCongViecDeXuat ?? string.Empty,
                    TenDanhMucCongViec = dmcv != null ? (dmcv.TenDanhMucCV ?? string.Empty) : string.Empty,
                    TenMucDoUuTien = md != null ? (md.TenMucDo ?? string.Empty) : string.Empty,
                    ChiPhiDeXuat = dx.ChiPhiDeXuat,
                    NgayBatDauCongViecDeXuat = dx.NgayBatDauCongViecDeXuat,
                    NgayKetThucCVDeXuatDuKien = dx.NgayKetThucCVDeXuatDuKien,
                    NgayDeXuatCongViec = dx.NgayDeXuatCongViec,
                    NgayDuyetDeXuatCongViec = dx.NgayDuyetDeXuatCongViec,
                    TrangThaiCongViecDeXuat = dx.TrangThaiCongViecDeXuat ?? string.Empty
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
                    query = query.Where(x => filterValues.Contains(x.TrangThaiCongViecDeXuat));
                }
            }

            if (tuNgayLoc.HasValue)
            {
                query = query.Where(x =>
                    x.NgayDeXuatCongViec.HasValue &&
                    x.NgayDeXuatCongViec.Value >= tuNgayLoc.Value);
            }

            if (denNgayLoc.HasValue)
            {
                var denNgayDocQuyen = denNgayLoc.Value.AddDays(1);
                query = query.Where(x =>
                    x.NgayDeXuatCongViec.HasValue &&
                    x.NgayDeXuatCongViec.Value < denNgayDocQuyen);
            }

            if (!string.IsNullOrWhiteSpace(tuKhoaLoc))
            {
                var keyword = tuKhoaLoc.ToLower();
                query = query.Where(x =>
                    x.TenCongViecDeXuat.ToLower().Contains(keyword) ||
                    x.MoTaCongViecDeXuat.ToLower().Contains(keyword) ||
                    x.TenDuAn.ToLower().Contains(keyword) ||
                    x.NguoiDungDeXuat.ToLower().Contains(keyword));
            }

            var totalItems = await query.CountAsync();
            var pagination = PaginationViewModel.Create(pageNumber, pageSize, totalItems);
            IQueryable<DuyetDeXuatCongViecItemViewModel> danhSachQuery = query
                .OrderByDescending(x => x.NgayDeXuatCongViec)
                .ThenByDescending(x => x.MaDeXuatCV);

            if (paginate)
            {
                danhSachQuery = danhSachQuery
                    .Skip(pagination.Skip)
                    .Take(pagination.PageSize);
            }

            return new DuyetDeXuatCongViecPageViewModel
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
                TuKhoa = tuKhoaLoc
            };
        }

        private async Task<List<DuyetDeXuatCongViecSelectOptionViewModel>> GetManagedProjectOptionsAsync(int currentUserId)
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
                .Select(x => new DuyetDeXuatCongViecSelectOptionViewModel
                {
                    Value = x.MaDuAn,
                    Text = string.IsNullOrWhiteSpace(x.TenDuAn) ? $"Dự án {x.MaDuAn}" : x.TenDuAn
                })
                .OrderBy(x => x.Text)
                .ToList();
        }

        private async Task<List<DuyetDeXuatCongViecSelectOptionViewModel>> GetRequesterOptionsAsync(int currentUserId)
        {
            var rawOptions = await (
                from dx in _context.DeXuatCongViec
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
                .Select(x => new DuyetDeXuatCongViecSelectOptionViewModel
                {
                    Value = x.MaNguoiDung,
                    Text = string.IsNullOrWhiteSpace(x.HoTenNguoiDung) ? $"Người dùng {x.MaNguoiDung}" : x.HoTenNguoiDung
                })
                .OrderBy(x => x.Text)
                .ToList();
        }

        public async Task ApproveAsync(int maDeXuatCv)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var deXuat = await _context.DeXuatCongViec
                .FirstOrDefaultAsync(x => x.MaDeXuatCV == maDeXuatCv && x.IsDeleted != true);

            if (deXuat == null)
                throw new Exception("Không tìm thấy đề xuất công việc.");

            if (!IsPending(deXuat.TrangThaiCongViecDeXuat))
                throw new Exception("Đề xuất công việc không còn ở trạng thái chờ duyệt.");

            await EnsureIsProjectManagerAsync(currentUserId, deXuat.MaDuAn);
            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == deXuat.MaDuAn && x.IsDeleted != true);

            if (duAn == null)
                throw new Exception("Không tìm thấy dự án của đề xuất công việc.");

            if (TrangThai.LaHoanThanhCongViec(duAn.TrangThaiDuAn))
                throw new Exception("Dự án đã hoàn thành. Vui lòng mở lại dự án trước khi duyệt đề xuất công việc.");

            if (TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.DaHuy)
                || TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.LuuTru))
            {
                throw new Exception("Dự án đã đóng, không thể duyệt đề xuất công việc.");
            }

            var canRollbackDuAn = TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.ChoXacNhanHoanThanh);

            var danhMucHopLe = await _context.DanhMucCongViec
                .AnyAsync(x => x.MaDanhMucCV == deXuat.MaDanhMucCV && x.MaDuAn == deXuat.MaDuAn && x.IsDeleted != true);

            if (!danhMucHopLe)
                throw new Exception("Danh mục công việc của đề xuất không còn hợp lệ.");

            var mucDoHopLe = await _context.MucDoUuTien
                .AnyAsync(x => x.MaMucDo == deXuat.MaMucDo);

            if (!mucDoHopLe)
                throw new Exception("Mức độ ưu tiên của đề xuất không còn hợp lệ.");

            var nganSachHienTai = await _context.NganSach
                .Where(x =>
                    x.MaDuAn == deXuat.MaDuAn
                    && x.IsDeleted != true
                    && x.IsActive == true
                    && (x.TrangThaiNganSach == TrangThai.DaDuyet || x.TrangThaiNganSach == TrangThai.DaDuyetHienThi))
                .OrderByDescending(x => x.Version)
                .ThenByDescending(x => x.NgayCapNhatNganSach)
                .FirstOrDefaultAsync();

            if (nganSachHienTai == null)
                throw new Exception("Dự án chưa có ngân sách hiện hành để duyệt đề xuất công việc.");

            var daCoCongViecTuDeXuat = await _context.CongViec.AnyAsync(x =>
                x.IsDeleted != true &&
                x.MaDeXuatCV == deXuat.MaDeXuatCV);

            if (daCoCongViecTuDeXuat)
                throw new Exception("Đề xuất công việc này đã được duyệt trước đó.");

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

            if (canRollbackDuAn)
            {
                duAn.TrangThaiDuAn = TrangThai.DangThucHien;
                _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
                {
                    MaDuAn = deXuat.MaDuAn,
                    MaNguoiDung = currentUserId,
                    NkHanhDongQLDA = $"Dự án tự chuyển về {TrangThai.ToDisplay(TrangThai.DangThucHien)} do duyệt thêm công việc mới từ đề xuất #{deXuat.MaDeXuatCV}.",
                    NkThoiGianQLDA = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task RejectAsync(int maDeXuatCv)
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var deXuat = await _context.DeXuatCongViec
                .FirstOrDefaultAsync(x => x.MaDeXuatCV == maDeXuatCv && x.IsDeleted != true);

            if (deXuat == null)
                throw new Exception("Không tìm thấy đề xuất công việc.");

            if (!IsPending(deXuat.TrangThaiCongViecDeXuat))
                throw new Exception("Đề xuất công việc không còn ở trạng thái chờ duyệt.");

            await EnsureIsProjectManagerAsync(currentUserId, deXuat.MaDuAn);

            deXuat.TrangThaiCongViecDeXuat = TrangThai.TuChoi;
            deXuat.MaNguoiDungDuyet = currentUserId;
            deXuat.NgayDuyetDeXuatCongViec = DateTime.Now;

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = deXuat.MaDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Từ chối đề xuất công việc #{deXuat.MaDeXuatCV}",
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
