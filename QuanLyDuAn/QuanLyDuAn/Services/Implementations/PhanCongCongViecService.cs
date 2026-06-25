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
                .Join(
                    _context.CtCongViec.Where(ct => ct.MaCongViec == maCongViec && ct.IsDeleted != true),
                    td => td.MaChiTietCV,
                    ct => ct.MaChiTietCV,
                    (td, ct) => td.PhanTram)
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
                },
                Form = new PhanCongCongViecCreateViewModel
                {
                    MaCongViec = maCongViec
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
                join asp in _context.Aspnetusers on nd.Id equals asp.Id
                where nvda.MaDuAn == maDuAn
                      && nd.IsDeleted != true
                      && (!asp.LockoutEnd.HasValue || asp.LockoutEnd <= DateTime.UtcNow)
                      && !maDaPhanCong.Contains(nd.MaNguoiDung)
                select new PhanCongCongViecOptionViewModel
                {
                    MaNguoiDung = nd.MaNguoiDung,
                    HoTenNguoiDung = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}",
                    VaiTroTrongDuAn = TrangThai.ToDisplayVaiTroTrongDuAn(nvda.VaiTroTrongDuAn)
                }).ToListAsync();

            return vm;
        }

        public async Task AddAsync(PhanCongCongViecCreateViewModel input)
        {
            var maCongViec = input.MaCongViec;
            var maNhanVien = input.MaNguoiDung ?? 0;

            if (maNhanVien <= 0)
                throw new Exception("Vui lòng chọn nhân viên để phân công.");

            var (congViec, maDuAn) = await GetCongViecVaDuAnAsync(maCongViec);
            var maNguoiDungHienTai = await GetCurrentUserIdAsync();
            var thoiDiemGiao = DateTime.Now;

            KiemTraTrangThaiCongViec(congViec);

            var coThePhanCong = await KiemTraQuyenPhanCongAsync(maDuAn, maNguoiDungHienTai);
            if (!coThePhanCong)
                throw new Exception("Bạn không có quyền phân công công việc này.");

            await KiemTraNhanVienHopLeAsync(maDuAn, maNhanVien, thoiDiemGiao);

            var daTonTai = await _context.PhanCongCongViec
                .AnyAsync(x => x.MaCongViec == maCongViec && x.MaNguoiDung == maNhanVien);

            if (daTonTai)
                throw new Exception("Nhân viên này đã được phân công cho công việc này.");

            _context.PhanCongCongViec.Add(new PhanCongCongViec
            {
                MaCongViec = maCongViec,
                MaNguoiDung = maNhanVien,
                NgayGiaoViec = thoiDiemGiao
            });

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
            var trangThaiCongViec = TrangThai.ToCode(congViec.TrangThaiCongViec);
            if (TrangThai.LaHoanThanhCongViec(trangThaiCongViec)
                || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.DaHuy)
                || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.LuuTru)
                || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.TamDung))
            {
                throw new Exception("Công việc đã đóng, không thể thực hiện phân công.");
            }
        }

        private async Task KiemTraNhanVienHopLeAsync(int maDuAn, int maNhanVien, DateTime thoiDiemGiao)
        {
            var nhanVien = await (
                from nd in _context.NguoiDung
                join asp in _context.Aspnetusers on nd.Id equals asp.Id
                where nd.MaNguoiDung == maNhanVien && nd.IsDeleted != true
                select new
                {
                    nd.MaNguoiDung,
                    asp.LockoutEnd
                }).FirstOrDefaultAsync();

            if (nhanVien == null)
                throw new Exception("Người được phân công không tồn tại hoặc đã bị xóa.");

            if (nhanVien.LockoutEnd.HasValue && nhanVien.LockoutEnd.Value > DateTime.UtcNow)
                throw new Exception("Tài khoản của người được phân công đang bị khóa.");

            var thamGiaDuAn = await _context.NhanVienDuAn
                .Where(x => x.MaDuAn == maDuAn && x.MaNguoiDung == maNhanVien)
                .Select(x => new { x.NgayThamGiaDuAn })
                .FirstOrDefaultAsync();

            if (thamGiaDuAn == null)
                throw new Exception("Nhân viên không thuộc dự án này. Vui lòng thêm nhân viên vào dự án trước.");

            if (thamGiaDuAn.NgayThamGiaDuAn.HasValue && thamGiaDuAn.NgayThamGiaDuAn.Value > thoiDiemGiao)
                throw new Exception("Nhân viên chưa tham gia dự án tại thời điểm phân công.");
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
            if (httpUser?.IsInRole("Admin") == true)
                return false;

            if (httpUser?.IsInRole("Manager") == true)
            {
                var laQuanLyDuAn = await _context.DuAn
                    .AnyAsync(x => x.MaDuAn == maDuAn
                                   && x.MaNguoiDung == maNguoiDungHienTai
                                   && x.IsDeleted != true);

                if (laQuanLyDuAn)
                    return true;
            }

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
