using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.PhanCongChiTietCongViec;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class PhanCongChiTietCongViecService : IPhanCongChiTietCongViecService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PhanCongChiTietCongViecService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PhanCongChiTietCongViecPageViewModel> GetPageAsync(int maChiTietCv)
        {
            var (chiTietCongViec, congViec, maDuAn) = await GetChiTietCongViecVaDuAnAsync(maChiTietCv);
            var maNguoiDungHienTai = await GetCurrentUserIdAsync();
            var coThePhanCong = await KiemTraQuyenPhanCongAsync(maDuAn, maNguoiDungHienTai);

            var vm = new PhanCongChiTietCongViecPageViewModel
            {
                CoTheQuanLyCongViec = coThePhanCong,
                ChiTietCongViec = new PhanCongChiTietCongViecSummaryViewModel
                {
                    MaChiTietCV = chiTietCongViec.MaChiTietCV,
                    MaCongViec = congViec.MaCongViec,
                    TenChiTietCongViec = chiTietCongViec.TenCTCV ?? chiTietCongViec.NoiDungChiTietCV ?? string.Empty,
                    TenCongViec = congViec.TenCongViec ?? string.Empty,
                    TenTrangThai = TrangThai.ToDisplay(chiTietCongViec.TrangThaiCTCV)
                },
                Form = new PhanCongChiTietCongViecCreateViewModel
                {
                    MaChiTietCV = maChiTietCv
                }
            };

            vm.DanhSachPhanCong = await (
                from pcct in _context.PhanCongCtCongViec
                join nd in _context.NguoiDung on pcct.MaNguoiDung equals nd.MaNguoiDung
                where pcct.MaChiTietCV == maChiTietCv && nd.IsDeleted != true
                select new PhanCongChiTietCongViecItemViewModel
                {
                    MaNguoiDung = nd.MaNguoiDung,
                    TenNhanVien = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}",
                    VaiTro = pcct.VaiTroTrongCTCV ?? TrangThai.VaiTroThucHien,
                    NgayGiaoChiTietCongViec = pcct.NgayGiaoCTCV
                }).ToListAsync();

            var maDaPhanCong = vm.DanhSachPhanCong
                .Select(x => x.MaNguoiDung)
                .ToHashSet();

            var thanhVienRaw = await (
                from nd in _context.NguoiDung
                join nvda in _context.NhanVienDuAn on nd.MaNguoiDung equals nvda.MaNguoiDung
                join asp in _context.Aspnetusers on nd.Id equals asp.Id
                join pccv in _context.PhanCongCongViec on nd.MaNguoiDung equals pccv.MaNguoiDung
                where nvda.MaDuAn == maDuAn
                      && pccv.MaCongViec == congViec.MaCongViec
                      && nd.IsDeleted != true
                      && (!asp.LockoutEnd.HasValue || asp.LockoutEnd <= DateTime.UtcNow)
                      && !maDaPhanCong.Contains(nd.MaNguoiDung)
                select new
                {
                    MaNguoiDung = nd.MaNguoiDung,
                    nd.HoTenNguoiDung,
                    nvda.VaiTroTrongDuAn
                })
                .ToListAsync();

            vm.ThanhVienCoThePhanCong = thanhVienRaw
                .GroupBy(x => x.MaNguoiDung)
                .Select(x =>
                {
                    var first = x.First();
                    return new PhanCongChiTietCongViecOptionViewModel
                    {
                        MaNguoiDung = first.MaNguoiDung,
                        HoTenNguoiDung = first.HoTenNguoiDung ?? $"Nhân viên {first.MaNguoiDung}",
                        VaiTroTrongDuAn = TrangThai.ToDisplayVaiTroTrongDuAn(first.VaiTroTrongDuAn)
                    };
                })
                .ToList();

            return vm;
        }

        public async Task AddAsync(PhanCongChiTietCongViecCreateViewModel input)
        {
            var maChiTietCv = input.MaChiTietCV;
            var maNhanVien = input.MaNguoiDung ?? 0;

            if (maNhanVien <= 0)
                throw new Exception("Vui lòng chọn nhân viên để phân công.");

            var (chiTietCongViec, _, maDuAn) = await GetChiTietCongViecVaDuAnAsync(maChiTietCv);
            var maNguoiDungHienTai = await GetCurrentUserIdAsync();
            var thoiDiemGiao = DateTime.Now;

            KiemTraTrangThaiChiTietCongViec(chiTietCongViec);

            var coThePhanCong = await KiemTraQuyenPhanCongAsync(maDuAn, maNguoiDungHienTai);
            if (!coThePhanCong)
                throw new Exception("Bạn không có quyền phân công chi tiết công việc này.");

            await KiemTraNhanVienHopLeAsync(maDuAn, chiTietCongViec.MaCongViec, maNhanVien, thoiDiemGiao);

            var daTonTai = await _context.PhanCongCtCongViec
                .AnyAsync(x => x.MaChiTietCV == maChiTietCv && x.MaNguoiDung == maNhanVien);

            if (daTonTai)
                throw new Exception("Nhân viên này đã được phân công cho chi tiết công việc này.");

            _context.PhanCongCtCongViec.Add(new PhanCongCtCongViec
            {
                MaChiTietCV = maChiTietCv,
                MaNguoiDung = maNhanVien,
                NgayGiaoCTCV = thoiDiemGiao,
                VaiTroTrongCTCV = TrangThai.VaiTroThucHien
            });

            GhiNhatKy(maChiTietCv, maNhanVien, maNguoiDungHienTai, TrangThai.HanhDongThemPhanCongChiTiet);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int maChiTietCv, int maNguoiDung)
        {
            var (chiTietCongViec, _, maDuAn) = await GetChiTietCongViecVaDuAnAsync(maChiTietCv);
            var maNguoiDungHienTai = await GetCurrentUserIdAsync();

            KiemTraTrangThaiChiTietCongViec(chiTietCongViec);

            var coThePhanCong = await KiemTraQuyenPhanCongAsync(maDuAn, maNguoiDungHienTai);
            if (!coThePhanCong)
                throw new Exception("Bạn không có quyền xóa phân công chi tiết công việc này.");

            var entity = await _context.PhanCongCtCongViec
                .FirstOrDefaultAsync(x => x.MaChiTietCV == maChiTietCv && x.MaNguoiDung == maNguoiDung);

            if (entity == null)
                throw new Exception("Không tìm thấy phân công cần xóa.");

            _context.PhanCongCtCongViec.Remove(entity);
            GhiNhatKy(maChiTietCv, maNguoiDung, maNguoiDungHienTai, TrangThai.HanhDongXoaPhanCongChiTiet);

            await _context.SaveChangesAsync();
        }

        private async Task<(CtCongViec chiTietCongViec, CongViec congViec, int maDuAn)> GetChiTietCongViecVaDuAnAsync(int maChiTietCv)
        {
            var chiTietCongViec = await _context.CtCongViec
                .FirstOrDefaultAsync(x => x.MaChiTietCV == maChiTietCv && x.IsDeleted != true);

            if (chiTietCongViec == null)
                throw new Exception("Không tìm thấy chi tiết công việc.");

            var congViec = await _context.CongViec
                .FirstOrDefaultAsync(x => x.MaCongViec == chiTietCongViec.MaCongViec && x.IsDeleted != true);

            if (congViec == null)
                throw new Exception("Không tìm thấy công việc cha của chi tiết công việc.");

            var danhMuc = await _context.DanhMucCongViec
                .FirstOrDefaultAsync(x => x.MaDanhMucCV == congViec.MaDanhMucCV && x.IsDeleted != true);

            if (danhMuc == null)
                throw new Exception("Không tìm thấy danh mục công việc.");

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == danhMuc.MaDuAn && x.IsDeleted != true);

            if (duAn == null)
                throw new Exception("Không tìm thấy dự án liên quan.");

            return (chiTietCongViec, congViec, duAn.MaDuAn);
        }

        private static void KiemTraTrangThaiChiTietCongViec(CtCongViec chiTietCongViec)
        {
            var trangThaiChiTiet = TrangThai.ToCode(chiTietCongViec.TrangThaiCTCV);
            if (TrangThai.LaHoanThanhCongViec(trangThaiChiTiet)
                || TrangThai.EqualsValue(trangThaiChiTiet, TrangThai.DaHuy)
                || TrangThai.EqualsValue(trangThaiChiTiet, TrangThai.LuuTru)
                || TrangThai.EqualsValue(trangThaiChiTiet, TrangThai.TamDung))
            {
                throw new Exception("Chi tiết công việc đã đóng, không thể thực hiện phân công.");
            }
        }

        private async Task KiemTraNhanVienHopLeAsync(int maDuAn, int maCongViec, int maNhanVien, DateTime thoiDiemGiao)
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
                throw new Exception("Nhân viên không thuộc dự án này.");

            if (thamGiaDuAn.NgayThamGiaDuAn.HasValue && thamGiaDuAn.NgayThamGiaDuAn.Value > thoiDiemGiao)
                throw new Exception("Nhân viên chưa tham gia dự án tại thời điểm phân công chi tiết công việc.");

            var daPhanCongCongViecCha = await _context.PhanCongCongViec
                .AnyAsync(x => x.MaCongViec == maCongViec && x.MaNguoiDung == maNhanVien);

            if (!daPhanCongCongViecCha)
                throw new Exception("Nhân viên chưa được phân công công việc cha.");

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

            var vaiTroTrongDuAn = await _context.NhanVienDuAn
                .Where(x => x.MaDuAn == maDuAn && x.MaNguoiDung == maNguoiDungHienTai)
                .Select(x => x.VaiTroTrongDuAn)
                .FirstOrDefaultAsync();

            if (TrangThai.EqualsValue(vaiTroTrongDuAn, TrangThai.VaiTroMember)
                || TrangThai.EqualsValue(vaiTroTrongDuAn, TrangThai.VaiTroThanhVienDuAn))
            {
                return false;
            }

            if (TrangThai.EqualsValue(vaiTroTrongDuAn, TrangThai.VaiTroLeader))
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

            return false;
        }

        private void GhiNhatKy(int maChiTietCv, int maNguoiDung, int maNguoiDungGhi, string hanhDong)
        {
            _context.NhatKyPhanCongCtCongViec.Add(new NhatKyPhanCongCtCongViec
            {
                MaChiTietCV = maChiTietCv,
                MaNguoiDung = maNguoiDung,
                MaNguoiDungGhi = maNguoiDungGhi,
                HanhDongPCCTCV = hanhDong,
                ThoiGianPCCTCV = DateTime.Now
            });
        }
    }
}
