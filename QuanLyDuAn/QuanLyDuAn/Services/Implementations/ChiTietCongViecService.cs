using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.ChiTietCongViec;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class ChiTietCongViecService : IChiTietCongViecService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChiTietCongViecService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ChiTietCongViecPageViewModel> GetPageAsync(int maCongViec)
        {
            var congViec = await LayCongViecAsync(maCongViec);
            var coTheCapNhat = await CoTheCapNhatAsync(congViec);
            var coThePhanCongChiTietCongViec = await CoThePhanCongChiTietCongViecAsync(congViec);

            var danhSach = await _context.CtCongViec
                .Where(x => x.MaCongViec == maCongViec && x.IsDeleted != true)
                .OrderByDescending(x => x.NgayTaoCTCV)
                .ThenByDescending(x => x.MaChiTietCV)
                .Select(x => new ChiTietCongViecItemViewModel
                {
                    MaChiTietCV = x.MaChiTietCV,
                    MaCongViec = x.MaCongViec,
                    TenCTCV = x.TenCTCV ?? string.Empty,
                    NoiDungChiTietCV = x.NoiDungChiTietCV ?? string.Empty,
                    NgayTaoCTCV = x.NgayTaoCTCV,
                    NgayBatDauCTCV = x.NgayBatDauCTCV,
                    NgayKetThucCTCV = x.NgayKetThucCTCV,
                    TrangThaiCTCV = x.TrangThaiCTCV ?? TrangThai.ChuaBatDau,
                    CoThePhanCongChiTietCongViec = coThePhanCongChiTietCongViec
                })
                .ToListAsync();

            return new ChiTietCongViecPageViewModel
            {
                CoTheCapNhat = coTheCapNhat,
                CongViec = new ChiTietCongViecSummaryViewModel
                {
                    MaCongViec = congViec.MaCongViec,
                    TenCongViec = congViec.TenCongViec ?? string.Empty,
                    TenTrangThai = TrangThai.ToDisplay(congViec.TrangThaiCongViec)
                },
                Form = new ChiTietCongViecCreateUpdateViewModel
                {
                    MaCongViec = maCongViec,
                    NgayBatDauCTCV = DateTime.Today,
                    TrangThaiCTCV = TrangThai.ChuaBatDau
                },
                DanhSach = danhSach
            };
        }

        public async Task AddAsync(ChiTietCongViecCreateUpdateViewModel form)
        {
            var congViec = await LayCongViecAsync(form.MaCongViec);
            KiemTraDuLieuDauVao(form, congViec);
            await KiemTraQuyenCapNhatAsync(congViec);

            var trangThai = TrangThai.ToCode(form.TrangThaiCTCV);

            var entity = new CtCongViec
            {
                MaCongViec = form.MaCongViec,
                TenCTCV = string.IsNullOrWhiteSpace(form.TenCTCV) ? null : form.TenCTCV.Trim(),
                NoiDungChiTietCV = form.NoiDungChiTietCV.Trim(),
                NgayTaoCTCV = DateTime.Now,
                NgayBatDauCTCV = form.NgayBatDauCTCV!.Value.Date,
                NgayKetThucCTCV = TrangThai.LaHoanThanhCongViec(trangThai) ? DateTime.Now : null,
                TrangThaiCTCV = trangThai,
                IsDeleted = false
            };

            _context.CtCongViec.Add(entity);
            await _context.SaveChangesAsync();

            await DongBoTrangThaiCongViecTheoChiTietAsync(congViec.MaCongViec);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChiTietCongViecCreateUpdateViewModel form)
        {
            if (form.MaChiTietCV <= 0)
                throw new Exception("Không tìm thấy chi tiết công việc cần cập nhật.");

            var congViec = await LayCongViecAsync(form.MaCongViec);
            KiemTraDuLieuDauVao(form, congViec);
            await KiemTraQuyenCapNhatAsync(congViec);

            var entity = await _context.CtCongViec
                .FirstOrDefaultAsync(x =>
                    x.MaChiTietCV == form.MaChiTietCV &&
                    x.MaCongViec == form.MaCongViec &&
                    x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy chi tiết công việc cần cập nhật.");

            var trangThai = TrangThai.ToCode(form.TrangThaiCTCV);

            entity.TenCTCV = string.IsNullOrWhiteSpace(form.TenCTCV) ? null : form.TenCTCV.Trim();
            entity.NoiDungChiTietCV = form.NoiDungChiTietCV.Trim();
            entity.NgayBatDauCTCV = form.NgayBatDauCTCV!.Value.Date;
            entity.NgayKetThucCTCV = TrangThai.LaHoanThanhCongViec(trangThai) ? DateTime.Now : null;
            entity.TrangThaiCTCV = trangThai;

            await _context.SaveChangesAsync();

            await DongBoTrangThaiCongViecTheoChiTietAsync(congViec.MaCongViec);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int maCongViec, int maChiTietCv)
        {
            var congViec = await LayCongViecAsync(maCongViec);
            await KiemTraQuyenCapNhatAsync(congViec);

            var entity = await _context.CtCongViec
                .FirstOrDefaultAsync(x =>
                    x.MaChiTietCV == maChiTietCv &&
                    x.MaCongViec == maCongViec &&
                    x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy chi tiết công việc cần xóa.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;
            entity.DeletedBy = await GetCurrentUserIdAsync();
            await _context.SaveChangesAsync();

            await DongBoTrangThaiCongViecTheoChiTietAsync(congViec.MaCongViec);
            await _context.SaveChangesAsync();
        }

        private static void KiemTraDuLieuDauVao(ChiTietCongViecCreateUpdateViewModel form, CongViec congViec)
        {
            if (form.MaCongViec <= 0)
                throw new Exception("Mã công việc không hợp lệ.");

            if (string.IsNullOrWhiteSpace(form.NoiDungChiTietCV))
                throw new Exception("Nội dung chi tiết công việc không được để trống.");

            if (form.NoiDungChiTietCV.Trim().Length > 255)
                throw new Exception("Nội dung chi tiết công việc tối đa 255 ký tự.");

            if (!string.IsNullOrWhiteSpace(form.TenCTCV) && form.TenCTCV.Trim().Length > 255)
                throw new Exception("Tên chi tiết công việc tối đa 255 ký tự.");

            if (!form.NgayBatDauCTCV.HasValue)
                throw new Exception("Vui lòng chọn ngày bắt đầu.");

            if (string.IsNullOrWhiteSpace(form.TrangThaiCTCV))
                throw new Exception("Vui lòng chọn trạng thái chi tiết công việc.");

            if (congViec.NgayBatDauCongViec.HasValue
                && form.NgayBatDauCTCV.Value.Date < congViec.NgayBatDauCongViec.Value.Date)
            {
                throw new Exception("Ngày bắt đầu chi tiết công việc không được trước ngày bắt đầu công việc.");
            }

            if (congViec.NgayKetThucCVDuKien.HasValue
                && form.NgayBatDauCTCV.Value.Date > congViec.NgayKetThucCVDuKien.Value.Date)
            {
                throw new Exception("Ngày bắt đầu chi tiết công việc không được sau ngày kết thúc dự kiến của công việc.");
            }

            var trangThai = TrangThai.ToCode(form.TrangThaiCTCV);
            var trangThaiHopLe = new[]
            {
                TrangThai.ChuaBatDau,
                TrangThai.DangThucHien,
                TrangThai.BiCanCan,
                TrangThai.TamDung,
                TrangThai.HoanThanh
            };

            if (!trangThaiHopLe.Any(x => TrangThai.EqualsValue(x, trangThai)))
                throw new Exception("Trạng thái chi tiết công việc không hợp lệ.");
        }

        private async Task<CongViec> LayCongViecAsync(int maCongViec)
        {
            var congViec = await _context.CongViec
                .FirstOrDefaultAsync(x => x.MaCongViec == maCongViec && x.IsDeleted != true);

            if (congViec == null)
                throw new Exception("Không tìm thấy công việc.");

            return congViec;
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

        private async Task<int> GetMaDuAnAsync(CongViec congViec)
        {
            var maDuAn = await _context.DanhMucCongViec
                .Where(x => x.MaDanhMucCV == congViec.MaDanhMucCV)
                .Select(x => x.MaDuAn)
                .FirstOrDefaultAsync();

            if (maDuAn <= 0)
                throw new Exception("Không tìm thấy dự án của công việc.");

            return maDuAn;
        }

        private async Task KiemTraQuyenCapNhatAsync(CongViec congViec)
        {
            if (await CoTheCapNhatAsync(congViec))
                return;

            var httpUser = _httpContextAccessor.HttpContext?.User;
            var isEmployee = httpUser?.IsInRole("Employee") == true;

            if (isEmployee)
                throw new Exception("Nhân viên chỉ được cập nhật chi tiết công việc khi là trưởng team hoặc leader dự án.");

            throw new Exception("Bạn không có quyền cập nhật chi tiết công việc này.");
        }

        private async Task<bool> CoTheCapNhatAsync(CongViec congViec)
        {
            var httpUser = _httpContextAccessor.HttpContext?.User;
            if (httpUser?.IsInRole("Admin") == true || httpUser?.IsInRole("Manager") == true)
                return true;

            var maNguoiDungHienTai = await GetCurrentUserIdAsync();
            var maDuAn = await GetMaDuAnAsync(congViec);
            var isEmployee = httpUser?.IsInRole("Employee") == true;

            if (isEmployee)
                return await LaLeaderTeamHoacDuAnAsync(maDuAn, maNguoiDungHienTai);

            var laLeaderDuAn = await _context.NhanVienDuAn
                .AnyAsync(x =>
                    x.MaDuAn == maDuAn &&
                    x.MaNguoiDung == maNguoiDungHienTai &&
                    TrangThai.EqualsValue(x.VaiTroTrongDuAn, TrangThai.VaiTroLeader));

            if (laLeaderDuAn)
                return true;

            return await _context.PhanCongCongViec
                .AnyAsync(x => x.MaCongViec == congViec.MaCongViec && x.MaNguoiDung == maNguoiDungHienTai);
        }

        private async Task<bool> LaLeaderTeamHoacDuAnAsync(int maDuAn, int maNguoiDung)
        {
            var teamDuAnIds = await _context.TeamDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.MaTeam)
                .ToListAsync();

            if (teamDuAnIds.Count > 0)
            {
                var laLeaderTeam = await _context.NhanVienTeam
                    .AnyAsync(x => teamDuAnIds.Contains(x.MaTeam)
                                && x.MaNguoiDung == maNguoiDung
                                && x.IsLeader == true);

                if (laLeaderTeam)
                    return true;
            }

            var vaiTroTrongDuAn = await _context.NhanVienDuAn
                .Where(x => x.MaDuAn == maDuAn && x.MaNguoiDung == maNguoiDung)
                .Select(x => x.VaiTroTrongDuAn)
                .FirstOrDefaultAsync();

            return TrangThai.EqualsValue(vaiTroTrongDuAn, TrangThai.VaiTroLeader);
        }

        private async Task<bool> CoThePhanCongChiTietCongViecAsync(CongViec congViec)
        {
            var httpUser = _httpContextAccessor.HttpContext?.User;
            if (httpUser?.IsInRole("Admin") == true || httpUser?.IsInRole("Manager") == true)
                return true;

            var maNguoiDungHienTai = await GetCurrentUserIdAsync();
            var maDuAn = await GetMaDuAnAsync(congViec);

            var teamDuAnIds = await _context.TeamDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.MaTeam)
                .ToListAsync();

            if (teamDuAnIds.Count > 0)
            {
                var laLeaderTeam = await _context.NhanVienTeam
                    .AnyAsync(x => teamDuAnIds.Contains(x.MaTeam)
                                && x.MaNguoiDung == maNguoiDungHienTai
                                && x.IsLeader == true);
                if (laLeaderTeam) return true;
            }

            var vaiTroTrongDuAn = await _context.NhanVienDuAn
                .Where(x => x.MaDuAn == maDuAn && x.MaNguoiDung == maNguoiDungHienTai)
                .Select(x => x.VaiTroTrongDuAn)
                .FirstOrDefaultAsync();

            return TrangThai.EqualsValue(vaiTroTrongDuAn, TrangThai.VaiTroLeader);
        }

        private async Task DongBoTrangThaiCongViecTheoChiTietAsync(int maCongViec)
        {
            var congViec = await LayCongViecAsync(maCongViec);
            var chiTietMoiNhat = await _context.CtCongViec
                .Where(x => x.MaCongViec == maCongViec && x.IsDeleted != true)
                .OrderByDescending(x => x.NgayBatDauCTCV)
                .ThenByDescending(x => x.NgayTaoCTCV)
                .ThenByDescending(x => x.MaChiTietCV)
                .FirstOrDefaultAsync();

            if (chiTietMoiNhat == null)
            {
                congViec.TrangThaiCongViec = TrangThai.ChuaBatDau;
                congViec.NgayKetThucCVThucTe = null;
                return;
            }

            var trangThaiChiTiet = TrangThai.ToCode(chiTietMoiNhat.TrangThaiCTCV);
            congViec.TrangThaiCongViec = string.IsNullOrWhiteSpace(trangThaiChiTiet)
                ? TrangThai.ChuaBatDau
                : trangThaiChiTiet;

            if (TrangThai.LaHoanThanhCongViec(trangThaiChiTiet))
                congViec.NgayKetThucCVThucTe = chiTietMoiNhat.NgayKetThucCTCV ?? DateTime.Now;
            else
                congViec.NgayKetThucCVThucTe = null;
        }
    }
}
