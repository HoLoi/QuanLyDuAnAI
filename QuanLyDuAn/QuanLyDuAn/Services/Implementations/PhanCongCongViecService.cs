using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.PhanCongCongViec;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class PhanCongCongViecService : IPhanCongCongViecService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PhanCongCongViecService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PhanCongCongViecPageViewModel> GetPageAsync(int maCongViec)
        {
            var (congViec, maDuAn) = await GetCongViecVaDuAnAsync(maCongViec);
            var maNguoiDungHienTai = await GetCurrentUserIdAsync();
            var coThePhanCong = await KiemTraQuyenPhanCongAsync(maDuAn, maNguoiDungHienTai);

            var tienDo = await _context.TienDoCongViec
                .Where(x => x.MaCongViec == maCongViec)
                .Select(x => x.PhanTram)
                .MaxAsync(x => (int?)x) ?? 0;

            var vm = new PhanCongCongViecPageViewModel
            {
                CoTheQuanLyCongViec = coThePhanCong,
                CongViec = new PhanCongCongViecSummaryViewModel
                {
                    MaCongViec = congViec.MaCongViec,
                    TenCongViec = congViec.TenCongViec ?? string.Empty,
                    TenTrangThai = TrangThai.ToDisplay(congViec.TrangThaiCongViec),
                    PhanTramTienDo = tienDo
                }
            };

            vm.DanhSachPhanCong = await (
                from pccv in _context.PhanCongCongViec
                join nd in _context.NguoiDung on pccv.MaNguoiDung equals nd.MaNguoiDung
                where pccv.MaCongViec == maCongViec
                select new PhanCongCongViecItemViewModel
                {
                    MaNguoiDung = nd.MaNguoiDung,
                    TenNhanVien = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}",
                    VaiTro = TrangThai.VaiTroThucHien,
                    NgayGiaoViec = pccv.NgayGiaoViec
                }).ToListAsync();

            var maDaPhanCong = vm.DanhSachPhanCong
                .Select(x => x.MaNguoiDung)
                .ToHashSet();

            vm.ThanhVienCoThePhanCong = await (
                from nd in _context.NguoiDung
                join nvda in _context.NhanVienDuAn on nd.MaNguoiDung equals nvda.MaNguoiDung
                where nvda.MaDuAn == maDuAn
                      && !maDaPhanCong.Contains(nd.MaNguoiDung)
                select new PhanCongCongViecOptionViewModel
                {
                    MaNguoiDung = nd.MaNguoiDung,
                    HoTenNguoiDung = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}",
                    VaiTroTrongDuAn = TrangThai.ToDisplayVaiTroTrongDuAn(nvda.VaiTroTrongDuAn)
                }).ToListAsync();

            return vm;
        }

        public async Task AddAsync(int maCongViec, int maNhanVien, DateTime? tuNgay, DateTime? denNgay)
        {
            var (congViec, maDuAn) = await GetCongViecVaDuAnAsync(maCongViec);
            var maNguoiDungHienTai = await GetCurrentUserIdAsync();

            KiemTraTrangThaiCongViec(congViec);

            var coThePhanCong = await KiemTraQuyenPhanCongAsync(maDuAn, maNguoiDungHienTai);
            if (!coThePhanCong)
                throw new Exception("Bạn không có quyền phân công công việc này.");

            var thuocDuAn = await _context.NhanVienDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn && x.MaNguoiDung == maNhanVien);

            if (!thuocDuAn)
                throw new Exception("Nhân viên không thuộc dự án này. Vui lòng thêm nhân viên vào dự án trước.");

            var daTonTai = await _context.PhanCongCongViec
                .AnyAsync(x => x.MaCongViec == maCongViec && x.MaNguoiDung == maNhanVien);

            if (daTonTai)
                throw new Exception("Nhân viên này đã được phân công cho công việc này.");

            _context.PhanCongCongViec.Add(new PhanCongCongViec
            {
                MaCongViec = maCongViec,
                MaNguoiDung = maNhanVien,
                NgayGiaoViec = DateTime.Now
            });

            if (tuNgay.HasValue) congViec.NgayBatDauCongViec = tuNgay.Value;
            if (denNgay.HasValue) congViec.NgayKetThucCVDuKien = denNgay.Value;

            GhiNhatKy(maCongViec, maNhanVien, maNguoiDungHienTai, TrangThai.HanhDongThemPhanCong);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int maCongViec, int maNguoiDung)
        {
            var (congViec, maDuAn) = await GetCongViecVaDuAnAsync(maCongViec);
            var maNguoiDungHienTai = await GetCurrentUserIdAsync();

            KiemTraTrangThaiCongViec(congViec);

            var coThePhanCong = await KiemTraQuyenPhanCongAsync(maDuAn, maNguoiDungHienTai);
            if (!coThePhanCong)
                throw new Exception("Bạn không có quyền xóa phân công công việc này.");

            var entity = await _context.PhanCongCongViec
                .FirstOrDefaultAsync(x => x.MaCongViec == maCongViec && x.MaNguoiDung == maNguoiDung);

            if (entity == null)
                throw new Exception("Không tìm thấy phân công cần xóa.");

            _context.PhanCongCongViec.Remove(entity);
            GhiNhatKy(maCongViec, maNguoiDung, maNguoiDungHienTai, TrangThai.HanhDongXoaPhanCong);

            await _context.SaveChangesAsync();
        }

        private async Task<(CongViec congViec, int maDuAn)> GetCongViecVaDuAnAsync(int maCongViec)
        {
            var congViec = await _context.CongViec
                .FirstOrDefaultAsync(x => x.MaCongViec == maCongViec && x.IsDeleted != true);

            if (congViec == null)
                throw new Exception("Không tìm thấy công việc.");

            var danhMuc = await _context.DanhMucCongViec
                .FirstOrDefaultAsync(x => x.MaDanhMucCV == congViec.MaDanhMucCV);

            if (danhMuc == null)
                throw new Exception("Không tìm thấy danh mục công việc.");

            return (congViec, danhMuc.MaDuAn);
        }

        private static void KiemTraTrangThaiCongViec(CongViec congViec)
        {
            if (TrangThai.LaHoanThanhCongViec(congViec.TrangThaiCongViec))
                throw new Exception("Công việc đã hoàn thành, không thể thực hiện phân công.");
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

        private async Task<bool> KiemTraQuyenPhanCongAsync(int maDuAn, int maNguoiDungHienTai)
        {
            var httpUser = _httpContextAccessor.HttpContext?.User;
            if (httpUser?.IsInRole("Admin") == true || httpUser?.IsInRole("Manager") == true)
                return true;

            var teamDuAnIds = await _context.TeamDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.MaTeam)
                .ToListAsync();

            if (teamDuAnIds.Count > 0)
            {
                var isTeamLeader = await _context.NhanVienTeam
                    .AnyAsync(x => teamDuAnIds.Contains(x.MaTeam)
                                && x.MaNguoiDung == maNguoiDungHienTai
                                && x.IsLeader == true);

                if (isTeamLeader) return true;
            }

            var maNvda = await _context.NhanVienDuAn
                .Where(x => x.MaDuAn == maDuAn && x.MaNguoiDung == maNguoiDungHienTai)
                .Select(x => x.VaiTroTrongDuAn)
                .FirstOrDefaultAsync();

            return TrangThai.EqualsValue(maNvda, TrangThai.VaiTroLeader);
        }

        private void GhiNhatKy(int maCongViec, int maNguoiDung, int maNguoiDungGhi, string hanhDong)
        {
            _context.NhatKyPhanCongCongViec.Add(new NhatKyPhanCongCongViec
            {
                MaCongViec = maCongViec,
                MaNguoiDung = maNguoiDung,
                MaNguoiDungGhi = maNguoiDungGhi,
                HanhDongPCCV = hanhDong,
                ThoiGianPCCV = DateTime.Now
            });
        }
    }
}
