using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DanhGiaNhanVien;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class DanhGiaNhanVienService : IDanhGiaNhanVienService
    {
        private const string TrangThaiNhap = "Nhap";
        private const int DiemToiThieu = 1;
        private const int DiemToiDa = 10;
        private const int DoDaiNhanXetToiDa = 500;

        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DanhGiaNhanVienService(
            QuanLyDuAnDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DanhGiaNhanVienPageViewModel> GetPageAsync(int? maDuAn, int? maNhanVien, string? tuKhoa)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaNhanVien.Xem);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            var leaderTeamIds = await LayDanhSachTeamLeaderAsync(currentUserId);
            var leaderProjectIds = await LayDanhSachDuAnLeaderScopeAsync(currentUserId, leaderTeamIds);

            var query =
                from dg in _context.DanhGiaNhanVien
                join da in _context.DuAn on dg.MaDuAn equals da.MaDuAn
                join nv in _context.NguoiDung on dg.MaNguoiDung equals nv.MaNguoiDung
                join ngDanhGia in _context.NguoiDung on dg.MaNguoiDungDanhGia equals ngDanhGia.MaNguoiDung
                join ngDuyet in _context.NguoiDung on dg.MaNguoiDungDuyet equals ngDuyet.MaNguoiDung into reviewJoin
                from ngDuyet in reviewJoin.DefaultIfEmpty()
                join nvda in _context.NhanVienDuAn on new { dg.MaDuAn, MaNguoiDung = dg.MaNguoiDung }
                    equals new { nvda.MaDuAn, nvda.MaNguoiDung } into projectRoleJoin
                from projectRole in projectRoleJoin.DefaultIfEmpty()
                where dg.IsDeleted != true
                      && da.IsDeleted != true
                      && nv.IsDeleted != true
                select new
                {
                    dg.MaDanhGiaNhanVien,
                    dg.MaDuAn,
                    TenDuAn = da.TenDuAn,
                    MaNguoiQuanLy = da.MaNguoiDung,
                    dg.MaNguoiDung,
                    TenNhanVien = nv.HoTenNguoiDung,
                    VaiTroTrongDuAn = projectRole.VaiTroTrongDuAn,
                    TrangThaiDanhGia = dg.TrangThaiDanhGiaNV,
                    dg.DiemTongDanhGiaNV,
                    dg.XepLoai,
                    NhanXetTongQuan = dg.NhanXetTongQuanNV,
                    dg.NgayDanhGiaNV,
                    dg.MaNguoiDungDanhGia,
                    TenNguoiDanhGia = ngDanhGia.HoTenNguoiDung,
                    TenNguoiDuyet = ngDuyet.HoTenNguoiDung,
                    NgayDuyet = dg.NgayDuyetDanhGiaNV,
                    LyDoTuChoi = dg.LyDoTuChoiDanhGiaNV
                };

            if (!roleFlags.IsAdmin)
            {
                if (roleFlags.IsManager)
                {
                    query = query.Where(x => x.MaNguoiQuanLy == currentUserId);
                }
                else
                {
                    query = query.Where(x =>
                        x.MaNguoiDung == currentUserId
                        || x.MaNguoiDungDanhGia == currentUserId
                        || (leaderProjectIds.Contains(x.MaDuAn)
                            && _context.TeamDuAn.Any(td => td.MaDuAn == x.MaDuAn && leaderTeamIds.Contains(td.MaTeam))
                            && _context.NhanVienTeam.Any(nt => nt.MaNguoiDung == x.MaNguoiDung && leaderTeamIds.Contains(nt.MaTeam))));
                }
            }

            if (maDuAn.HasValue)
            {
                query = query.Where(x => x.MaDuAn == maDuAn.Value);
            }

            if (maNhanVien.HasValue)
            {
                query = query.Where(x => x.MaNguoiDung == maNhanVien.Value);
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    (x.TenDuAn ?? string.Empty).ToLower().Contains(keyword)
                    || (x.TenNhanVien ?? string.Empty).ToLower().Contains(keyword)
                    || (x.NhanXetTongQuan ?? string.Empty).ToLower().Contains(keyword));
            }

            var rows = await query
                .OrderByDescending(x => x.NgayDanhGiaNV ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaDanhGiaNhanVien)
                .ToListAsync();

            var danhGiaIds = rows.Select(x => x.MaDanhGiaNhanVien).ToList();
            var statsByDanhGia = await XayDungThongKeNhanVienTheoDanhGiaAsync(danhGiaIds);

            var items = rows.Select(x =>
            {
                var trangThaiDanhGia = ChuanHoaTrangThaiDanhGia(x.TrangThaiDanhGia);
                statsByDanhGia.TryGetValue(x.MaDanhGiaNhanVien, out var thongKe);

                var coTheSua = CoQuyenSuaDanhGiaNhanVien(roleFlags, currentUserId, x.MaNguoiDungDanhGia, trangThaiDanhGia);
                var coTheGuiDuyet = coTheSua && !roleFlags.IsManager;
                var coTheDuyet = CoQuyenDuyetDanhGiaNhanVien(roleFlags, currentUserId, x.MaNguoiQuanLy, trangThaiDanhGia);

                var diemTong = x.DiemTongDanhGiaNV ?? 0;
                return new DanhGiaNhanVienItemViewModel
                {
                    MaDanhGiaNhanVien = x.MaDanhGiaNhanVien,
                    MaDuAn = x.MaDuAn,
                    TenDuAn = x.TenDuAn ?? $"Du an {x.MaDuAn}",
                    MaNhanVien = x.MaNguoiDung,
                    TenNhanVien = x.TenNhanVien ?? $"Nguoi dung {x.MaNguoiDung}",
                    VaiTroTrongDuAn = TrangThai.ToDisplayVaiTroTrongDuAn(x.VaiTroTrongDuAn),
                    TrangThaiDanhGia = trangThaiDanhGia,
                    DiemTongKet = diemTong,
                    XepLoai = !string.IsNullOrWhiteSpace(x.XepLoai) ? x.XepLoai! : TinhXepLoai(diemTong),
                    NhanXet = x.NhanXetTongQuan,
                    NgayDanhGia = x.NgayDanhGiaNV,
                    TenNguoiDanhGia = x.TenNguoiDanhGia ?? $"Nguoi dung {x.MaNguoiDungDanhGia}",
                    TenNguoiDuyet = x.TenNguoiDuyet,
                    NgayDuyet = x.NgayDuyet,
                    LyDoTuChoi = x.LyDoTuChoi,
                    TongChiTietDuocGiao = thongKe?.TongChiTietDuocGiao ?? 0,
                    ChiTietHoanThanh = thongKe?.ChiTietHoanThanh ?? 0,
                    ChiTietTreHan = thongKe?.ChiTietTreHan ?? 0,
                    TyLeHoanThanh = thongKe?.TyLeHoanThanh ?? 0,
                    CoTheDanhGia = CoQuyenTaoDanhGiaNhanVien(roleFlags, currentUserId, x.MaNguoiQuanLy, x.MaDuAn, x.MaNguoiDung, leaderTeamIds, leaderProjectIds),
                    CoTheSua = coTheSua,
                    CoTheGuiDuyet = coTheGuiDuyet,
                    CoTheDuyet = coTheDuyet,
                    CoTheTuChoi = coTheDuyet
                };
            }).ToList();

            return new DanhGiaNhanVienPageViewModel
            {
                MaDuAn = maDuAn,
                MaNhanVien = maNhanVien,
                TuKhoa = tuKhoa,
                DanhSach = items,
                DanhSachDuAn = await LayDanhSachDuAnTheoScopeAsync(currentUserId, roleFlags, leaderProjectIds),
                DanhSachNhanVien = await LayDanhSachNhanVienTheoScopeAsync(currentUserId, roleFlags, maDuAn, leaderProjectIds, leaderTeamIds),
                TongSo = items.Count,
                SoNhap = items.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThaiNhap)),
                SoChoDuyet = items.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThai.ChoDuyet)),
                SoDaDuyet = items.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThai.DaDuyet)),
                SoTuChoi = items.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThai.TuChoi))
            };
        }

        public async Task<DanhGiaNhanVienFormViewModel> GetFormAsync(int maDuAn, int maNhanVien)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaNhanVien.DanhGia, Permissions.DanhGiaNhanVien.Sua);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminTacNghiep(roleFlags);

            if (currentUserId == maNhanVien)
            {
                throw new Exception("Khong duoc tu danh gia chinh minh.");
            }

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);
            if (duAn == null)
            {
                throw new Exception("Khong tim thay du an.");
            }

            var thanhVien = await (
                from nvda in _context.NhanVienDuAn
                join nd in _context.NguoiDung on nvda.MaNguoiDung equals nd.MaNguoiDung
                where nvda.MaDuAn == maDuAn
                      && nvda.MaNguoiDung == maNhanVien
                      && nd.IsDeleted != true
                select new
                {
                    nvda.MaNguoiDung,
                    TenNhanVien = nd.HoTenNguoiDung,
                    nvda.VaiTroTrongDuAn
                }).FirstOrDefaultAsync();

            if (thanhVien == null)
            {
                throw new Exception("Nhan vien khong thuoc du an.");
            }

            var leaderTeamIds = await LayDanhSachTeamLeaderAsync(currentUserId);
            var leaderProjectIds = await LayDanhSachDuAnLeaderScopeAsync(currentUserId, leaderTeamIds);

            if (!CoQuyenTaoDanhGiaNhanVien(roleFlags, currentUserId, duAn.MaNguoiDung, maDuAn, maNhanVien, leaderTeamIds, leaderProjectIds))
            {
                throw new Exception("Ban khong co quyen tao hoac sua danh gia nhan vien nay.");
            }

            if (!(roleFlags.IsManager && duAn.MaNguoiDung == currentUserId))
            {
                var coScopeLeader = await CoThuocScopeLeaderAsync(maDuAn, maNhanVien, leaderTeamIds);
                if (!coScopeLeader)
                {
                    throw new Exception("Leader chi duoc danh gia nhan vien trong pham vi team duoc phan quyen.");
                }
            }

            var tieuChi = await LayTieuChiDanhGiaNhanVienAsync();
            var congViecOptions = await LayCongViecOptionsTheoDuAnAsync(maDuAn);

            var danhGia = await _context.DanhGiaNhanVien
                .Where(x =>
                    x.MaDuAn == maDuAn
                    && x.MaNguoiDung == maNhanVien
                    && x.MaNguoiDungDanhGia == currentUserId
                    && x.IsDeleted != true)
                .OrderByDescending(x => x.NgayDanhGiaNV ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaDanhGiaNhanVien)
                .FirstOrDefaultAsync();

            var chiTiet = new List<CtDanhGiaNhanVien>();
            if (danhGia != null)
            {
                chiTiet = await _context.CtDanhGiaNhanVien
                    .Where(x => x.MaDanhGiaNhanVien == danhGia.MaDanhGiaNhanVien && x.IsDeleted != true)
                    .ToListAsync();
            }

            var chiTietByTieuChi = chiTiet
                .Where(x => x.MaTieuChi.HasValue)
                .ToDictionary(x => x.MaTieuChi!.Value, x => x);

            var tieuChiVm = tieuChi.Select(tc =>
            {
                chiTietByTieuChi.TryGetValue(tc.MaTieuChi, out var row);
                var maCongViec = row?.MaCongViec;
                return new DanhGiaNhanVienTieuChiViewModel
                {
                    MaChiTietDGNV = row?.MaChiTietDGNV,
                    MaTieuChi = tc.MaTieuChi,
                    TenTieuChi = tc.TenTieuChi ?? $"Tieu chi {tc.MaTieuChi}",
                    MaCongViec = maCongViec,
                    TenCongViec = maCongViec.HasValue && congViecOptions.TryGetValue(maCongViec.Value, out var tenCongViec)
                        ? tenCongViec
                        : null,
                    DiemDanhGiaNV = Math.Clamp(row?.DiemDanhGiaNV ?? DiemToiThieu, DiemToiThieu, DiemToiDa),
                    NoiDungDanhGiaNhanVien = row?.NoiDungDanhGiaNhanVien
                };
            }).ToList();

            var trangThaiDanhGia = ChuanHoaTrangThaiDanhGia(danhGia?.TrangThaiDanhGiaNV);
            var biKhoa = LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.DaDuyet)
                         || LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.ChoDuyet);
            var laManagerDuAn = roleFlags.IsManager && duAn.MaNguoiDung == currentUserId;

            var diemTong = TinhDiemTongKet(tieuChiVm.Select(x => x.DiemDanhGiaNV));
            var thongKe = await XayDungThongKeNhanVienAsync(maDuAn, maNhanVien);
            return new DanhGiaNhanVienFormViewModel
            {
                MaDanhGiaNhanVien = danhGia?.MaDanhGiaNhanVien,
                MaDuAn = maDuAn,
                TenDuAn = duAn.TenDuAn ?? $"Du an {maDuAn}",
                MaNhanVien = maNhanVien,
                TenNhanVien = thanhVien.TenNhanVien ?? $"Nguoi dung {maNhanVien}",
                VaiTroTrongDuAn = TrangThai.ToDisplayVaiTroTrongDuAn(thanhVien.VaiTroTrongDuAn),
                TongChiTietDuocGiao = thongKe.TongChiTietDuocGiao,
                ChiTietHoanThanh = thongKe.ChiTietHoanThanh,
                ChiTietDangLam = thongKe.ChiTietDangLam,
                ChiTietTreHan = thongKe.ChiTietTreHan,
                TyLeHoanThanh = thongKe.TyLeHoanThanh,
                SoLanCapNhatTienDo = thongKe.SoLanCapNhatTienDo,
                LanCapNhatGanNhat = thongKe.LanCapNhatGanNhat,
                SoFileMinhChung = thongKe.SoFileMinhChung,
                DiemTrungBinhTienDo = thongKe.DiemTrungBinhTienDo,
                TieuChi = tieuChiVm,
                NhanXetTongQuan = danhGia?.NhanXetTongQuanNV,
                DiemTongKet = diemTong,
                XepLoai = TinhXepLoai(diemTong),
                TrangThaiDanhGia = trangThaiDanhGia,
                CoTheLuu = !biKhoa,
                CoTheGuiDuyet = !biKhoa && !laManagerDuAn,
                CoTheDuyet = false,
                CoTheTuChoi = false,
                ThongKe = thongKe
            };
        }

        public async Task LuuDanhGiaAsync(DanhGiaNhanVienFormViewModel form)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaNhanVien.DanhGia, Permissions.DanhGiaNhanVien.Sua);

            if (form.MaDuAn <= 0 || form.MaNhanVien <= 0)
            {
                throw new Exception("Thong tin danh gia nhan vien khong hop le.");
            }

            if (form.TieuChi.Count == 0)
            {
                throw new Exception("Danh gia nhan vien phai co it nhat mot tieu chi.");
            }

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminTacNghiep(roleFlags);

            if (form.MaNhanVien == currentUserId)
            {
                throw new Exception("Khong duoc tu danh gia chinh minh.");
            }

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == form.MaDuAn && x.IsDeleted != true);
            if (duAn == null)
            {
                throw new Exception("Khong tim thay du an.");
            }

            var thanhVienHopLe = await _context.NhanVienDuAn
                .AnyAsync(x => x.MaDuAn == form.MaDuAn && x.MaNguoiDung == form.MaNhanVien);
            if (!thanhVienHopLe)
            {
                throw new Exception("Nhan vien khong thuoc du an.");
            }

            var leaderTeamIds = await LayDanhSachTeamLeaderAsync(currentUserId);
            var leaderProjectIds = await LayDanhSachDuAnLeaderScopeAsync(currentUserId, leaderTeamIds);
            if (!CoQuyenTaoDanhGiaNhanVien(roleFlags, currentUserId, duAn.MaNguoiDung, form.MaDuAn, form.MaNhanVien, leaderTeamIds, leaderProjectIds))
            {
                throw new Exception("Ban khong co quyen tac nghiep danh gia nhan vien nay.");
            }

            if (!(roleFlags.IsManager && duAn.MaNguoiDung == currentUserId))
            {
                var coScopeLeader = await CoThuocScopeLeaderAsync(form.MaDuAn, form.MaNhanVien, leaderTeamIds);
                if (!coScopeLeader)
                {
                    throw new Exception("Leader chi duoc danh gia nhan vien trong pham vi team duoc phan quyen.");
                }
            }

            if (!string.IsNullOrWhiteSpace(form.NhanXetTongQuan) && form.NhanXetTongQuan.Trim().Length > DoDaiNhanXetToiDa)
            {
                throw new Exception($"Nhan xet tong quan toi da {DoDaiNhanXetToiDa} ky tu.");
            }

            foreach (var item in form.TieuChi)
            {
                if (item.DiemDanhGiaNV < DiemToiThieu || item.DiemDanhGiaNV > DiemToiDa)
                {
                    throw new Exception("Diem tung tieu chi phai nam trong khoang 1 den 10.");
                }

                if (!string.IsNullOrWhiteSpace(item.NoiDungDanhGiaNhanVien) && item.NoiDungDanhGiaNhanVien.Trim().Length > DoDaiNhanXetToiDa)
                {
                    throw new Exception($"Noi dung tung tieu chi toi da {DoDaiNhanXetToiDa} ky tu.");
                }
            }

            DanhGiaNhanVien? entity = null;
            if (form.MaDanhGiaNhanVien.HasValue && form.MaDanhGiaNhanVien.Value > 0)
            {
                entity = await _context.DanhGiaNhanVien
                    .FirstOrDefaultAsync(x => x.MaDanhGiaNhanVien == form.MaDanhGiaNhanVien.Value && x.IsDeleted != true);
            }

            if (entity == null)
            {
                entity = new DanhGiaNhanVien
                {
                    MaDuAn = form.MaDuAn,
                    MaNguoiDung = form.MaNhanVien,
                    MaNguoiDungDanhGia = currentUserId,
                    IsDeleted = false
                };
                _context.DanhGiaNhanVien.Add(entity);
            }
            else
            {
                if (entity.MaNguoiDungDanhGia != currentUserId)
                {
                    throw new Exception("Ban khong co quyen sua ban danh gia nay.");
                }

                var trangThaiCu = ChuanHoaTrangThaiDanhGia(entity.TrangThaiDanhGiaNV);
                if (LaTrangThaiDanhGia(trangThaiCu, TrangThai.DaDuyet))
                {
                    throw new Exception("Danh gia da duyet khong the chinh sua.");
                }

                if (LaTrangThaiDanhGia(trangThaiCu, TrangThai.ChoDuyet))
                {
                    throw new Exception("Danh gia dang cho duyet, khong the chinh sua.");
                }
            }

            var diemTong = TinhDiemTongKet(form.TieuChi.Select(x => x.DiemDanhGiaNV));
            var xepLoai = TinhXepLoai(diemTong);
            var laManagerDuAn = roleFlags.IsManager && duAn.MaNguoiDung == currentUserId;

            entity.MaDuAn = form.MaDuAn;
            entity.MaNguoiDung = form.MaNhanVien;
            entity.MaNguoiDungDanhGia = currentUserId;
            entity.DiemTongDanhGiaNV = (int)Math.Round(diemTong, MidpointRounding.AwayFromZero);
            entity.XepLoai = xepLoai;
            entity.NhanXetTongQuanNV = form.NhanXetTongQuan?.Trim();
            entity.NgayDanhGiaNV = DateTime.Now;

            if (laManagerDuAn)
            {
                entity.TrangThaiDanhGiaNV = TrangThai.DaDuyet;
                entity.MaNguoiDungDuyet = currentUserId;
                entity.NgayDuyetDanhGiaNV = DateTime.Now;
                entity.LyDoTuChoiDanhGiaNV = null;
            }
            else
            {
                entity.TrangThaiDanhGiaNV = TrangThaiNhap;
                entity.MaNguoiDungDuyet = null;
                entity.NgayDuyetDanhGiaNV = null;
                entity.LyDoTuChoiDanhGiaNV = null;
            }

            await _context.SaveChangesAsync();

            var chiTietCu = await _context.CtDanhGiaNhanVien
                .Where(x => x.MaDanhGiaNhanVien == entity.MaDanhGiaNhanVien && x.IsDeleted != true)
                .ToListAsync();

            foreach (var old in chiTietCu)
            {
                old.IsDeleted = true;
                old.DeletedAt = DateTime.Now;
                old.DeletedBy = currentUserId;
            }

            foreach (var item in form.TieuChi)
            {
                _context.CtDanhGiaNhanVien.Add(new CtDanhGiaNhanVien
                {
                    MaDanhGiaNhanVien = entity.MaDanhGiaNhanVien,
                    MaTieuChi = item.MaTieuChi,
                    MaCongViec = item.MaCongViec,
                    NoiDungDanhGiaNhanVien = item.NoiDungDanhGiaNhanVien?.Trim(),
                    DiemDanhGiaNV = item.DiemDanhGiaNV,
                    IsDeleted = false
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task GuiDuyetAsync(int maDanhGiaNhanVien)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaNhanVien.DanhGia, Permissions.DanhGiaNhanVien.Sua);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminTacNghiep(roleFlags);

            var entity = await _context.DanhGiaNhanVien
                .FirstOrDefaultAsync(x => x.MaDanhGiaNhanVien == maDanhGiaNhanVien && x.IsDeleted != true);
            if (entity == null)
            {
                throw new Exception("Khong tim thay ban danh gia nhan vien.");
            }

            if (entity.MaNguoiDungDanhGia != currentUserId)
            {
                throw new Exception("Ban khong co quyen gui duyet ban danh gia nay.");
            }

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == entity.MaDuAn && x.IsDeleted != true);
            if (duAn == null)
            {
                throw new Exception("Du an khong ton tai hoac da bi xoa.");
            }

            if (duAn.MaNguoiDung == currentUserId)
            {
                throw new Exception("Danh gia do Manager tao da duoc duyet ngay khi luu.");
            }

            var trangThai = ChuanHoaTrangThaiDanhGia(entity.TrangThaiDanhGiaNV);
            if (!LaTrangThaiDanhGia(trangThai, TrangThaiNhap) && !LaTrangThaiDanhGia(trangThai, TrangThai.TuChoi))
            {
                throw new Exception("Chi ban danh gia nhap hoac tu choi moi duoc gui duyet.");
            }

            var soChiTiet = await _context.CtDanhGiaNhanVien
                .CountAsync(x => x.MaDanhGiaNhanVien == maDanhGiaNhanVien && x.IsDeleted != true);
            if (soChiTiet <= 0)
            {
                throw new Exception("Danh gia nhan vien chua co chi tiet tieu chi.");
            }

            entity.TrangThaiDanhGiaNV = TrangThai.ChoDuyet;
            entity.MaNguoiDungDuyet = null;
            entity.NgayDuyetDanhGiaNV = null;
            entity.LyDoTuChoiDanhGiaNV = null;
            entity.NgayDanhGiaNV = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task DuyetAsync(int maDanhGiaNhanVien)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaNhanVien.Duyet);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminTacNghiep(roleFlags);

            var entity = await _context.DanhGiaNhanVien
                .FirstOrDefaultAsync(x => x.MaDanhGiaNhanVien == maDanhGiaNhanVien && x.IsDeleted != true);
            if (entity == null)
            {
                throw new Exception("Khong tim thay ban danh gia nhan vien.");
            }

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == entity.MaDuAn && x.IsDeleted != true);
            if (duAn == null)
            {
                throw new Exception("Du an khong ton tai hoac da bi xoa.");
            }

            if (duAn.MaNguoiDung != currentUserId)
            {
                throw new Exception("Chi Manager cua du an duoc phep duyet danh gia nhan vien.");
            }

            var trangThai = ChuanHoaTrangThaiDanhGia(entity.TrangThaiDanhGiaNV);
            if (!LaTrangThaiDanhGia(trangThai, TrangThai.ChoDuyet))
            {
                throw new Exception("Chi ban danh gia dang cho duyet moi duoc phe duyet.");
            }

            entity.TrangThaiDanhGiaNV = TrangThai.DaDuyet;
            entity.MaNguoiDungDuyet = currentUserId;
            entity.NgayDuyetDanhGiaNV = DateTime.Now;
            entity.LyDoTuChoiDanhGiaNV = null;
            await _context.SaveChangesAsync();
        }

        public async Task TuChoiAsync(int maDanhGiaNhanVien, string lyDoTuChoi)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaNhanVien.Duyet);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminTacNghiep(roleFlags);

            if (string.IsNullOrWhiteSpace(lyDoTuChoi))
            {
                throw new Exception("Vui long nhap ly do tu choi.");
            }

            var lyDo = lyDoTuChoi.Trim();
            if (lyDo.Length > DoDaiNhanXetToiDa)
            {
                throw new Exception($"Ly do tu choi toi da {DoDaiNhanXetToiDa} ky tu.");
            }

            var entity = await _context.DanhGiaNhanVien
                .FirstOrDefaultAsync(x => x.MaDanhGiaNhanVien == maDanhGiaNhanVien && x.IsDeleted != true);
            if (entity == null)
            {
                throw new Exception("Khong tim thay ban danh gia nhan vien.");
            }

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == entity.MaDuAn && x.IsDeleted != true);
            if (duAn == null)
            {
                throw new Exception("Du an khong ton tai hoac da bi xoa.");
            }

            if (duAn.MaNguoiDung != currentUserId)
            {
                throw new Exception("Chi Manager cua du an duoc phep tu choi danh gia nhan vien.");
            }

            var trangThai = ChuanHoaTrangThaiDanhGia(entity.TrangThaiDanhGiaNV);
            if (!LaTrangThaiDanhGia(trangThai, TrangThai.ChoDuyet))
            {
                throw new Exception("Chi ban danh gia dang cho duyet moi duoc tu choi.");
            }

            entity.TrangThaiDanhGiaNV = TrangThai.TuChoi;
            entity.MaNguoiDungDuyet = currentUserId;
            entity.NgayDuyetDanhGiaNV = DateTime.Now;
            entity.LyDoTuChoiDanhGiaNV = lyDo;
            await _context.SaveChangesAsync();
        }

        public async Task<DanhGiaNhanVienChiTietViewModel> GetChiTietAsync(int maDanhGiaNhanVien)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaNhanVien.Xem);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            var leaderTeamIds = await LayDanhSachTeamLeaderAsync(currentUserId);
            var leaderProjectIds = await LayDanhSachDuAnLeaderScopeAsync(currentUserId, leaderTeamIds);

            var data = await (
                from dg in _context.DanhGiaNhanVien
                join da in _context.DuAn on dg.MaDuAn equals da.MaDuAn
                join nv in _context.NguoiDung on dg.MaNguoiDung equals nv.MaNguoiDung
                join ngDanhGia in _context.NguoiDung on dg.MaNguoiDungDanhGia equals ngDanhGia.MaNguoiDung
                join ngDuyet in _context.NguoiDung on dg.MaNguoiDungDuyet equals ngDuyet.MaNguoiDung into reviewJoin
                from ngDuyet in reviewJoin.DefaultIfEmpty()
                join nvda in _context.NhanVienDuAn on new { dg.MaDuAn, MaNguoiDung = dg.MaNguoiDung }
                    equals new { nvda.MaDuAn, nvda.MaNguoiDung } into roleJoin
                from role in roleJoin.DefaultIfEmpty()
                where dg.MaDanhGiaNhanVien == maDanhGiaNhanVien
                      && dg.IsDeleted != true
                      && da.IsDeleted != true
                      && nv.IsDeleted != true
                select new
                {
                    dg.MaDanhGiaNhanVien,
                    dg.MaDuAn,
                    TenDuAn = da.TenDuAn,
                    MaNguoiQuanLy = da.MaNguoiDung,
                    dg.MaNguoiDung,
                    TenNhanVien = nv.HoTenNguoiDung,
                    VaiTroTrongDuAn = role.VaiTroTrongDuAn,
                    TrangThaiDanhGia = dg.TrangThaiDanhGiaNV,
                    dg.DiemTongDanhGiaNV,
                    dg.XepLoai,
                    NhanXetTongQuan = dg.NhanXetTongQuanNV,
                    dg.NgayDanhGiaNV,
                    dg.MaNguoiDungDanhGia,
                    TenNguoiDanhGia = ngDanhGia.HoTenNguoiDung,
                    TenNguoiDuyet = ngDuyet.HoTenNguoiDung,
                    NgayDuyet = dg.NgayDuyetDanhGiaNV,
                    LyDoTuChoi = dg.LyDoTuChoiDanhGiaNV
                }).FirstOrDefaultAsync();

            if (data == null)
            {
                throw new Exception("Khong tim thay ban danh gia nhan vien.");
            }

            var coQuyen = roleFlags.IsAdmin
                          || (roleFlags.IsManager && data.MaNguoiQuanLy == currentUserId)
                          || data.MaNguoiDung == currentUserId
                          || data.MaNguoiDungDanhGia == currentUserId
                          || (leaderProjectIds.Contains(data.MaDuAn)
                              && _context.TeamDuAn.Any(td => td.MaDuAn == data.MaDuAn && leaderTeamIds.Contains(td.MaTeam))
                              && _context.NhanVienTeam.Any(nt => nt.MaNguoiDung == data.MaNguoiDung && leaderTeamIds.Contains(nt.MaTeam)));
            if (!coQuyen)
            {
                throw new Exception("Ban khong co quyen xem danh gia nhan vien nay.");
            }

            var chiTietRows = await (
                from ct in _context.CtDanhGiaNhanVien
                join tc in _context.TieuChiDanhGia on ct.MaTieuChi equals tc.MaTieuChi into tcJoin
                from tc in tcJoin.DefaultIfEmpty()
                join cv in _context.CongViec on ct.MaCongViec equals cv.MaCongViec into cvJoin
                from cv in cvJoin.DefaultIfEmpty()
                where ct.MaDanhGiaNhanVien == maDanhGiaNhanVien && ct.IsDeleted != true
                orderby ct.MaChiTietDGNV
                select new DanhGiaNhanVienTieuChiViewModel
                {
                    MaChiTietDGNV = ct.MaChiTietDGNV,
                    MaTieuChi = ct.MaTieuChi ?? 0,
                    TenTieuChi = tc != null && !string.IsNullOrWhiteSpace(tc.TenTieuChi)
                        ? tc.TenTieuChi!
                        : $"Tieu chi {ct.MaChiTietDGNV}",
                    MaCongViec = ct.MaCongViec,
                    TenCongViec = cv.TenCongViec,
                    DiemDanhGiaNV = Math.Clamp(ct.DiemDanhGiaNV ?? DiemToiThieu, DiemToiThieu, DiemToiDa),
                    NoiDungDanhGiaNhanVien = ct.NoiDungDanhGiaNhanVien
                }).ToListAsync();

            var diemTong = data.DiemTongDanhGiaNV ?? (int)Math.Round(TinhDiemTongKet(chiTietRows.Select(x => x.DiemDanhGiaNV)), MidpointRounding.AwayFromZero);
            var thongKe = await XayDungThongKeNhanVienAsync(data.MaDuAn, data.MaNguoiDung);
            return new DanhGiaNhanVienChiTietViewModel
            {
                MaDanhGiaNhanVien = data.MaDanhGiaNhanVien,
                MaDuAn = data.MaDuAn,
                TenDuAn = data.TenDuAn ?? $"Du an {data.MaDuAn}",
                MaNhanVien = data.MaNguoiDung,
                TenNhanVien = data.TenNhanVien ?? $"Nguoi dung {data.MaNguoiDung}",
                VaiTroTrongDuAn = TrangThai.ToDisplayVaiTroTrongDuAn(data.VaiTroTrongDuAn),
                TongChiTietDuocGiao = thongKe.TongChiTietDuocGiao,
                ChiTietHoanThanh = thongKe.ChiTietHoanThanh,
                ChiTietDangLam = thongKe.ChiTietDangLam,
                ChiTietTreHan = thongKe.ChiTietTreHan,
                TyLeHoanThanh = thongKe.TyLeHoanThanh,
                SoLanCapNhatTienDo = thongKe.SoLanCapNhatTienDo,
                LanCapNhatGanNhat = thongKe.LanCapNhatGanNhat,
                SoFileMinhChung = thongKe.SoFileMinhChung,
                DiemTrungBinhTienDo = thongKe.DiemTrungBinhTienDo,
                TrangThaiDanhGia = ChuanHoaTrangThaiDanhGia(data.TrangThaiDanhGia),
                DiemTongKet = diemTong,
                XepLoai = !string.IsNullOrWhiteSpace(data.XepLoai) ? data.XepLoai! : TinhXepLoai(diemTong),
                NhanXetTongQuan = data.NhanXetTongQuan,
                NgayDanhGia = data.NgayDanhGiaNV,
                TenNguoiDanhGia = data.TenNguoiDanhGia ?? $"Nguoi dung {data.MaNguoiDungDanhGia}",
                TenNguoiDuyet = data.TenNguoiDuyet,
                NgayDuyet = data.NgayDuyet,
                LyDoTuChoi = data.LyDoTuChoi,
                TieuChi = chiTietRows,
                ThongKe = thongKe
            };
        }

        private async Task<Dictionary<int, DanhGiaNhanVienThongKeViewModel>> XayDungThongKeNhanVienTheoDanhGiaAsync(List<int> maDanhGiaIds)
        {
            var map = new Dictionary<int, DanhGiaNhanVienThongKeViewModel>();
            if (maDanhGiaIds.Count == 0)
            {
                return map;
            }

            var rows = await _context.DanhGiaNhanVien
                .Where(x => maDanhGiaIds.Contains(x.MaDanhGiaNhanVien) && x.IsDeleted != true)
                .Select(x => new
                {
                    x.MaDanhGiaNhanVien,
                    x.MaDuAn,
                    x.MaNguoiDung
                })
                .ToListAsync();

            foreach (var row in rows)
            {
                map[row.MaDanhGiaNhanVien] = await XayDungThongKeNhanVienAsync(row.MaDuAn, row.MaNguoiDung);
            }

            return map;
        }

        private async Task<DanhGiaNhanVienThongKeViewModel> XayDungThongKeNhanVienAsync(int maDuAn, int maNhanVien)
        {
            var thongTin = await (
                from da in _context.DuAn
                join nd in _context.NguoiDung on maNhanVien equals nd.MaNguoiDung
                join nvda in _context.NhanVienDuAn on new { da.MaDuAn, MaNguoiDung = nd.MaNguoiDung }
                    equals new { nvda.MaDuAn, nvda.MaNguoiDung }
                where da.MaDuAn == maDuAn
                      && da.IsDeleted != true
                      && nd.IsDeleted != true
                select new
                {
                    da.MaDuAn,
                    TenDuAn = da.TenDuAn,
                    nd.MaNguoiDung,
                    TenNhanVien = nd.HoTenNguoiDung,
                    nvda.VaiTroTrongDuAn
                }).FirstOrDefaultAsync();

            if (thongTin == null)
            {
                return new DanhGiaNhanVienThongKeViewModel();
            }

            var chiTietIds = await (
                from pc in _context.PhanCongCtCongViec
                join ct in _context.CtCongViec on pc.MaChiTietCV equals ct.MaChiTietCV
                join cv in _context.CongViec on ct.MaCongViec equals cv.MaCongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where pc.MaNguoiDung == maNhanVien
                      && dm.MaDuAn == maDuAn
                      && dm.IsDeleted != true
                      && cv.IsDeleted != true
                      && ct.IsDeleted != true
                select ct.MaChiTietCV
            ).Distinct().ToListAsync();

            var tongChiTiet = chiTietIds.Count;

            var trangThaiHoanThanhCongViec = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
            var completedSet = await _context.CtCongViec
                .Where(x =>
                    chiTietIds.Contains(x.MaChiTietCV)
                    && x.TrangThaiCTCV != null
                    && trangThaiHoanThanhCongViec.Contains(x.TrangThaiCTCV))
                .Select(x => x.MaChiTietCV)
                .ToListAsync();

            var treHanSet = await _context.CtCongViec
                .Where(x =>
                    chiTietIds.Contains(x.MaChiTietCV)
                    && x.NgayKetThucCTCV.HasValue
                    && x.NgayKetThucCTCV.Value < DateTime.Now
                    && (x.TrangThaiCTCV == null
                        || !trangThaiHoanThanhCongViec.Contains(x.TrangThaiCTCV)))
                .Select(x => x.MaChiTietCV)
                .ToListAsync();

            var soBaoCaoDaGui = await _context.TienDoCongViec
                .CountAsync(x => x.MaNguoiDung == maNhanVien && chiTietIds.Contains(x.MaChiTietCV));

            var trangThaiDaDuyetTienDo = TrangThai.GetCommonStatusVariants(TrangThai.DaDuyet);
            var soBaoCaoDaDuyet = await _context.TienDoCongViec
                .CountAsync(x =>
                    x.MaNguoiDung == maNhanVien
                    && chiTietIds.Contains(x.MaChiTietCV)
                    && x.TrangThaiTienDo != null
                    && trangThaiDaDuyetTienDo.Contains(x.TrangThaiTienDo));

            var trangThaiTuChoiTienDo = TrangThai.GetCommonStatusVariants(TrangThai.TuChoi);
            var trangThaiYeuCauBoSungTienDo = TrangThai.GetCommonStatusVariants(TrangThai.YeuCauBoSung);
            var soBaoCaoBiTuChoiHoacBoSung = await _context.TienDoCongViec
                .CountAsync(x =>
                    x.MaNguoiDung == maNhanVien
                    && chiTietIds.Contains(x.MaChiTietCV)
                    && x.TrangThaiTienDo != null
                    && (trangThaiTuChoiTienDo.Contains(x.TrangThaiTienDo)
                        || trangThaiYeuCauBoSungTienDo.Contains(x.TrangThaiTienDo)));

            var maTienDoIds = await _context.TienDoCongViec
                .Where(x => x.MaNguoiDung == maNhanVien && chiTietIds.Contains(x.MaChiTietCV))
                .Select(x => x.MaTienDo)
                .Distinct()
                .ToListAsync();

            var soFile = await _context.FileTienDoCongViec
                .CountAsync(x => maTienDoIds.Contains(x.MaTienDo) && x.IsDeleted != true);

            var lanCapNhatGanNhat = await _context.TienDoCongViec
                .Where(x => x.MaNguoiDung == maNhanVien && chiTietIds.Contains(x.MaChiTietCV))
                .MaxAsync(x => (DateTime?)x.ThoiGianCapNhat);
            var diemTrungBinhTienDo = await _context.TienDoCongViec
                .Where(x =>
                    x.MaNguoiDung == maNhanVien
                    && chiTietIds.Contains(x.MaChiTietCV)
                    && x.PhanTram.HasValue)
                .Select(x => (double?)x.PhanTram)
                .AverageAsync() ?? 0d;

            var tyLe = tongChiTiet <= 0 ? 0d : Math.Round((double)completedSet.Count / tongChiTiet * 100d, 2);
            var chiTietDangLam = Math.Max(0, tongChiTiet - completedSet.Count);
            return new DanhGiaNhanVienThongKeViewModel
            {
                MaDuAn = thongTin.MaDuAn,
                TenDuAn = thongTin.TenDuAn ?? $"Du an {thongTin.MaDuAn}",
                MaNhanVien = thongTin.MaNguoiDung,
                TenNhanVien = thongTin.TenNhanVien ?? $"Nguoi dung {thongTin.MaNguoiDung}",
                VaiTroTrongDuAn = TrangThai.ToDisplayVaiTroTrongDuAn(thongTin.VaiTroTrongDuAn),
                TongChiTietDuocGiao = tongChiTiet,
                ChiTietHoanThanh = completedSet.Count,
                ChiTietDangLam = chiTietDangLam,
                ChiTietTreHan = treHanSet.Count,
                TyLeHoanThanh = tyLe,
                SoLanCapNhatTienDo = soBaoCaoDaGui,
                SoBaoCaoDaDuyet = soBaoCaoDaDuyet,
                SoBaoCaoTuChoiHoacBoSung = soBaoCaoBiTuChoiHoacBoSung,
                SoFileMinhChung = soFile,
                LanCapNhatGanNhat = lanCapNhatGanNhat,
                DiemTrungBinhTienDo = Math.Round(diemTrungBinhTienDo, 2)
            };
        }

        private async Task<List<DanhGiaNhanVienDuAnOptionViewModel>> LayDanhSachDuAnTheoScopeAsync(
            int currentUserId,
            (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags,
            List<int> leaderProjectIds)
        {
            IQueryable<DuAn> query = _context.DuAn.Where(x => x.IsDeleted != true);

            if (!roleFlags.IsAdmin)
            {
                if (roleFlags.IsManager)
                {
                    query = query.Where(x => x.MaNguoiDung == currentUserId);
                }
                else
                {
                    query = query.Where(x =>
                        leaderProjectIds.Contains(x.MaDuAn)
                        || _context.NhanVienDuAn.Any(nv => nv.MaDuAn == x.MaDuAn && nv.MaNguoiDung == currentUserId));
                }
            }

            return await query
                .OrderBy(x => x.TenDuAn)
                .Select(x => new DanhGiaNhanVienDuAnOptionViewModel
                {
                    MaDuAn = x.MaDuAn,
                    TenDuAn = x.TenDuAn ?? $"Du an {x.MaDuAn}"
                }).ToListAsync();
        }

        private async Task<List<DanhGiaNhanVienNhanVienOptionViewModel>> LayDanhSachNhanVienTheoScopeAsync(
            int currentUserId,
            (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags,
            int? maDuAn,
            List<int> leaderProjectIds,
            List<int> leaderTeamIds)
        {
            var query =
                from nvda in _context.NhanVienDuAn
                join nd in _context.NguoiDung on nvda.MaNguoiDung equals nd.MaNguoiDung
                where nd.IsDeleted != true
                select new
                {
                    nvda.MaDuAn,
                    nvda.MaNguoiDung,
                    nd.HoTenNguoiDung
                };

            if (maDuAn.HasValue)
            {
                query = query.Where(x => x.MaDuAn == maDuAn.Value);
            }

            if (!roleFlags.IsAdmin)
            {
                if (roleFlags.IsManager)
                {
                    query = query.Where(x => _context.DuAn.Any(da => da.MaDuAn == x.MaDuAn && da.MaNguoiDung == currentUserId && da.IsDeleted != true));
                }
                else
                {
                    query = query.Where(x =>
                        leaderProjectIds.Contains(x.MaDuAn)
                        && _context.TeamDuAn.Any(td => td.MaDuAn == x.MaDuAn && leaderTeamIds.Contains(td.MaTeam))
                        && _context.NhanVienTeam.Any(nt => nt.MaNguoiDung == x.MaNguoiDung && leaderTeamIds.Contains(nt.MaTeam))
                        || x.MaNguoiDung == currentUserId);
                }
            }

            var rows = await query
                .GroupBy(x => new { x.MaNguoiDung, x.HoTenNguoiDung })
                .OrderBy(x => x.Key.HoTenNguoiDung)
                .Select(x => new DanhGiaNhanVienNhanVienOptionViewModel
                {
                    MaNhanVien = x.Key.MaNguoiDung,
                    TenNhanVien = x.Key.HoTenNguoiDung ?? $"Nguoi dung {x.Key.MaNguoiDung}"
                }).ToListAsync();

            return rows;
        }

        private async Task<List<TieuChiDanhGia>> LayTieuChiDanhGiaNhanVienAsync()
        {
            var loaiHopLe = new[] { "DANHGIANHANVIEN", "NHANVIEN" };
            var trangThaiDangSuDung = TrangThai.GetCommonStatusVariants(TrangThai.DangSuDung);

            return await _context.TieuChiDanhGia
                .Where(x =>
                    x.MaTieuChi > 0
                    && x.TenTieuChi != null
                    && x.LoaiTieuChi != null
                    && loaiHopLe.Contains(x.LoaiTieuChi.ToUpper())
                    && (string.IsNullOrWhiteSpace(x.TrangThaiTieuChi)
                        || trangThaiDangSuDung.Contains(x.TrangThaiTieuChi)))
                .OrderBy(x => x.MaTieuChi)
                .ToListAsync();
        }

        private async Task<Dictionary<int, string>> LayCongViecOptionsTheoDuAnAsync(int maDuAn)
        {
            var rows = await (
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where dm.MaDuAn == maDuAn
                      && dm.IsDeleted != true
                      && cv.IsDeleted != true
                select new
                {
                    cv.MaCongViec,
                    cv.TenCongViec
                }).ToListAsync();

            return rows.ToDictionary(
                x => x.MaCongViec,
                x => x.TenCongViec ?? $"Cong viec {x.MaCongViec}");
        }

        private static bool CoQuyenTaoDanhGiaNhanVien(
            (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags,
            int currentUserId,
            int maNguoiQuanLyDuAn,
            int maDuAn,
            int maNhanVien,
            List<int> leaderTeamIds,
            List<int> leaderProjectIds)
        {
            if (roleFlags.IsAdmin || currentUserId == maNhanVien)
            {
                return false;
            }

            if (roleFlags.IsManager && maNguoiQuanLyDuAn == currentUserId)
            {
                return true;
            }

            if (leaderProjectIds.Contains(maDuAn) && leaderTeamIds.Count > 0)
            {
                return true;
            }

            return false;
        }

        private static bool CoQuyenSuaDanhGiaNhanVien(
            (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags,
            int currentUserId,
            int maNguoiDanhGia,
            string trangThaiDanhGia)
        {
            if (roleFlags.IsAdmin)
            {
                return false;
            }

            if (currentUserId != maNguoiDanhGia)
            {
                return false;
            }

            return LaTrangThaiDanhGia(trangThaiDanhGia, TrangThaiNhap)
                   || LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.TuChoi);
        }

        private static bool CoQuyenDuyetDanhGiaNhanVien(
            (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags,
            int currentUserId,
            int maNguoiQuanLyDuAn,
            string trangThaiDanhGia)
        {
            return !roleFlags.IsAdmin
                   && roleFlags.IsManager
                   && maNguoiQuanLyDuAn == currentUserId
                   && LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.ChoDuyet);
        }

        private async Task<List<int>> LayDanhSachTeamLeaderAsync(int currentUserId)
        {
            return await _context.NhanVienTeam
                .Where(x => x.MaNguoiDung == currentUserId && x.IsLeader == true)
                .Select(x => x.MaTeam)
                .Distinct()
                .ToListAsync();
        }

        private async Task<List<int>> LayDanhSachDuAnLeaderScopeAsync(int currentUserId, List<int> teamLeaderIds)
        {
            var leaderRoleVariants = new[] { TrangThai.VaiTroLeader, TrangThai.VaiTroLeaderHienThi };
            var leaderInProjectIds = await _context.NhanVienDuAn
                .Where(x =>
                    x.MaNguoiDung == currentUserId
                    && x.VaiTroTrongDuAn != null
                    && leaderRoleVariants.Contains(x.VaiTroTrongDuAn))
                .Select(x => x.MaDuAn)
                .Distinct()
                .ToListAsync();

            if (teamLeaderIds.Count == 0)
            {
                return leaderInProjectIds;
            }

            var projectByTeamLeader = await _context.TeamDuAn
                .Where(x => teamLeaderIds.Contains(x.MaTeam))
                .Select(x => x.MaDuAn)
                .Distinct()
                .ToListAsync();

            return leaderInProjectIds.Concat(projectByTeamLeader).Distinct().ToList();
        }

        private async Task<bool> CoThuocScopeLeaderAsync(int maDuAn, int maNhanVien, List<int> teamLeaderIds)
        {
            if (teamLeaderIds.Count == 0)
            {
                return false;
            }

            var coTeamTrongDuAn = await _context.TeamDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn && teamLeaderIds.Contains(x.MaTeam));
            if (!coTeamTrongDuAn)
            {
                return false;
            }

            return await _context.NhanVienTeam
                .AnyAsync(x => x.MaNguoiDung == maNhanVien && teamLeaderIds.Contains(x.MaTeam));
        }

        private static string ChuanHoaTrangThaiDanhGia(string? trangThai)
        {
            if (string.IsNullOrWhiteSpace(trangThai))
            {
                return TrangThaiNhap;
            }

            if (LaTrangThaiDanhGia(trangThai, TrangThai.ChoDuyet))
            {
                return TrangThai.ChoDuyet;
            }

            if (LaTrangThaiDanhGia(trangThai, TrangThai.DaDuyet))
            {
                return TrangThai.DaDuyet;
            }

            if (LaTrangThaiDanhGia(trangThai, TrangThai.TuChoi))
            {
                return TrangThai.TuChoi;
            }

            if (LaTrangThaiDanhGia(trangThai, TrangThaiNhap))
            {
                return TrangThaiNhap;
            }

            return trangThai.Trim();
        }

        private static bool LaTrangThaiDanhGia(string? value, string expected)
        {
            return string.Equals(TrangThai.Normalize(value), TrangThai.Normalize(expected), StringComparison.OrdinalIgnoreCase);
        }

        private static double TinhDiemTongKet(IEnumerable<int> diemTieuChi)
        {
            var ds = diemTieuChi.ToList();
            if (ds.Count == 0)
            {
                return 0d;
            }

            return Math.Round(ds.Average(), 2);
        }

        private static string TinhXepLoai(double diem)
        {
            if (diem >= 8.5d) return "Xuat sac";
            if (diem >= 7d) return "Tot";
            if (diem >= 5.5d) return "Kha";
            if (diem >= 4d) return "Trung binh";
            return "Kem";
        }

        private static void KiemTraKhongChoAdminTacNghiep((bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags)
        {
            if (roleFlags.IsAdmin)
            {
                throw new Exception("Tai khoan Admin khong duoc tac nghiep danh gia nhan vien.");
            }
        }

        private void KiemTraQuyenTheoClaim(params string[] tenQuyen)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                throw new Exception("Ban chua dang nhap.");
            }

            var granted = user.Claims
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => x.Value.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!tenQuyen.Any(granted.Contains))
            {
                throw new Exception("Ban khong co quyen truy cap chuc nang danh gia nhan vien.");
            }
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(aspUserId))
            {
                throw new Exception("Khong xac dinh duoc nguoi dung hien tai.");
            }

            var maNguoiDung = await _context.Aspnetusers
                .Where(x => x.Id == aspUserId)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync();

            if (maNguoiDung <= 0)
            {
                throw new Exception("Khong xac dinh duoc nhan su tuong ung cua nguoi dung hien tai.");
            }

            return maNguoiDung;
        }

        private async Task<(bool IsAdmin, bool IsManager, bool IsEmployee)> GetCurrentUserRoleFlagsAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(aspUserId))
            {
                throw new Exception("Khong xac dinh duoc nguoi dung hien tai.");
            }

            var roleNames = await (
                from ur in _context.Aspnetuserroles
                join r in _context.Aspnetroles on ur.Id equals r.Id
                where ur.Asp_Id == aspUserId
                select (r.NormalizedName ?? r.Name) ?? string.Empty
            ).ToListAsync();

            var normalizedRoles = roleNames
                .Select(x => x.Trim().ToUpperInvariant())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet();

            return (
                normalizedRoles.Contains("ADMIN"),
                normalizedRoles.Contains("MANAGER"),
                normalizedRoles.Contains("EMPLOYEE"));
        }
    }
}

