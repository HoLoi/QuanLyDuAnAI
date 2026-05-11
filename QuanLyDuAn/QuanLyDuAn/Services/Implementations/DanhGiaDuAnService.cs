using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DanhGiaDuAn;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class DanhGiaDuAnService : IDanhGiaDuAnService
    {
        private const string TrangThaiNhap = "Nhap";
        private const string TrangThaiDuLieuAiMacDinh = "Chưa có dữ liệu AI cho dự án này";
        private const int DiemToiThieu = 1;
        private const int DiemToiDa = 10;
        private const int DoDaiNhanXetToiDa = 500;

        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DanhGiaDuAnService(
            QuanLyDuAnDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DanhGiaDuAnPageViewModel> GetPageAsync(string? tuKhoa, string? trangThai, int? maDuAn)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaDuAn.Xem);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();

            var query =
                from dg in _context.DanhGiaDuAn
                join da in _context.DuAn on dg.MaDuAn equals da.MaDuAn
                join nguoiTao in _context.NguoiDung on dg.MaNguoiDung equals nguoiTao.MaNguoiDung
                join nguoiQuanLy in _context.NguoiDung on da.MaNguoiDung equals nguoiQuanLy.MaNguoiDung
                join nguoiDuyet in _context.NguoiDung on dg.MaNguoiDungDuyet equals nguoiDuyet.MaNguoiDung into reviewJoin
                from nguoiDuyet in reviewJoin.DefaultIfEmpty()
                where dg.IsDeleted != true
                      && da.IsDeleted != true
                select new
                {
                    dg.MaDanhGiaDuAn,
                    dg.MaDuAn,
                    TenDuAn = da.TenDuAn,
                    TrangThaiDuAn = da.TrangThaiDuAn,
                    da.PhanTramHoanThanh,
                    MaNguoiQuanLy = da.MaNguoiDung,
                    TenNguoiQuanLy = nguoiQuanLy.HoTenNguoiDung,
                    MaNguoiDanhGia = dg.MaNguoiDung,
                    TenNguoiDanhGia = nguoiTao.HoTenNguoiDung,
                    dg.DiemTongDanhGiaDA,
                    dg.NhanXetTongDuAn,
                    dg.NgayDanhGiaDA,
                    TrangThaiDanhGia = dg.TrangThaiDanhGiaDA,
                    TenNguoiDuyet = nguoiDuyet.HoTenNguoiDung,
                    NgayDuyet = dg.NgayDuyetDanhGiaDA,
                    LyDoTuChoi = dg.LyDoTuChoiDanhGiaDA
                };

            if (!roleFlags.IsAdmin)
            {
                query = query.Where(x =>
                    x.MaNguoiQuanLy == currentUserId
                    || _context.NhanVienDuAn.Any(nv =>
                        nv.MaDuAn == x.MaDuAn
                        && nv.MaNguoiDung == currentUserId));
            }

            if (maDuAn.HasValue)
            {
                query = query.Where(x => x.MaDuAn == maDuAn.Value);
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    (x.TenDuAn ?? string.Empty).ToLower().Contains(keyword)
                    || (x.TenNguoiQuanLy ?? string.Empty).ToLower().Contains(keyword)
                    || (x.NhanXetTongDuAn ?? string.Empty).ToLower().Contains(keyword));
            }

            var rows = await query
                .OrderByDescending(x => x.NgayDanhGiaDA ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaDanhGiaDuAn)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                rows = rows
                    .Where(x => LaTrangThaiDanhGia(ChuanHoaTrangThaiDanhGia(x.TrangThaiDanhGia), trangThai))
                    .ToList();
            }

            var items = rows.Select(x =>
            {
                var trangThaiCode = ChuanHoaTrangThaiDanhGia(x.TrangThaiDanhGia);
                var diemTong = x.DiemTongDanhGiaDA ?? 0;
                var xepLoai = TinhXepLoai(diemTong);
                var coTheSua = CoQuyenSuaDanhGiaDuAn(roleFlags, currentUserId, x.MaNguoiDanhGia, x.MaNguoiQuanLy, trangThaiCode);
                var coTheGuiDuyet = coTheSua && ChoPhepGuiDuyetTheoTrangThaiDuAn(x.TrangThaiDuAn);

                return new DanhGiaDuAnItemViewModel
                {
                    MaDanhGiaDuAn = x.MaDanhGiaDuAn,
                    MaDuAn = x.MaDuAn,
                    TenDuAn = x.TenDuAn ?? $"Du an {x.MaDuAn}",
                    TenNguoiQuanLy = x.TenNguoiQuanLy ?? $"Nguoi dung {x.MaNguoiQuanLy}",
                    TrangThaiDuAn = TrangThai.ToDisplay(x.TrangThaiDuAn),
                    TrangThaiDanhGia = trangThaiCode,
                    PhanTramHoanThanh = Math.Clamp(x.PhanTramHoanThanh ?? 0, 0, 100),
                    DiemTongKet = diemTong,
                    XepLoai = xepLoai,
                    NhanXet = x.NhanXetTongDuAn,
                    NgayDanhGia = x.NgayDanhGiaDA,
                    TenNguoiDanhGia = x.TenNguoiDanhGia ?? $"Nguoi dung {x.MaNguoiDanhGia}",
                    TenNguoiDuyet = x.TenNguoiDuyet,
                    NgayDuyet = x.NgayDuyet,
                    LyDoTuChoi = x.LyDoTuChoi,
                    CoTheDanhGia = roleFlags.IsManager && x.MaNguoiQuanLy == currentUserId,
                    CoTheSua = coTheSua,
                    CoTheGuiDuyet = coTheGuiDuyet,
                    CoTheDuyet = CoQuyenDuyetDanhGiaDuAn(roleFlags, trangThaiCode),
                    CoTheTuChoi = CoQuyenDuyetDanhGiaDuAn(roleFlags, trangThaiCode)
                };
            }).ToList();

            var danhSachDuAn = await LayDanhSachDuAnTheoScopeAsync(currentUserId, roleFlags);

            return new DanhGiaDuAnPageViewModel
            {
                TuKhoa = tuKhoa,
                TrangThai = trangThai,
                MaDuAn = maDuAn,
                DanhSach = items,
                DanhSachDuAn = danhSachDuAn,
                TongSo = items.Count,
                SoNhap = items.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThaiNhap)),
                SoChoDuyet = items.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThai.ChoDuyet)),
                SoDaDuyet = items.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThai.DaDuyet)),
                SoTuChoi = items.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThai.TuChoi))
            };
        }

        public async Task<DanhGiaDuAnFormViewModel> GetFormAsync(int maDuAn)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaDuAn.DanhGia, Permissions.DanhGiaDuAn.Sua);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminTacNghiep(roleFlags);

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (duAn == null)
            {
                throw new Exception("Khong tim thay du an.");
            }

            if (!roleFlags.IsManager || duAn.MaNguoiDung != currentUserId)
            {
                throw new Exception("Ban khong co quyen tao hoac sua danh gia du an nay.");
            }

            var tieuChi = await LayTieuChiDanhGiaDuAnAsync();

            var danhGia = await _context.DanhGiaDuAn
                .Where(x => x.MaDuAn == maDuAn && x.MaNguoiDung == currentUserId && x.IsDeleted != true)
                .OrderByDescending(x => x.NgayDanhGiaDA ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaDanhGiaDuAn)
                .FirstOrDefaultAsync();

            var chiTietRows = new List<CtDanhGiaDuAn>();
            if (danhGia != null)
            {
                chiTietRows = await _context.CtDanhGiaDuAn
                    .Where(x => x.MaDanhGiaDuAn == danhGia.MaDanhGiaDuAn && x.IsDeleted != true)
                    .ToListAsync();
            }

            var tieuChiMap = chiTietRows
                .Where(x => x.MaTieuChi.HasValue)
                .ToDictionary(x => x.MaTieuChi!.Value, x => x);

            var danhSachTieuChi = tieuChi.Select(tc =>
            {
                tieuChiMap.TryGetValue(tc.MaTieuChi, out var chiTiet);
                return new DanhGiaDuAnTieuChiViewModel
                {
                    MaChiTietDGDA = chiTiet?.MaChiTietDGDA,
                    MaTieuChi = tc.MaTieuChi,
                    TenTieuChi = tc.TenTieuChi ?? $"Tieu chi {tc.MaTieuChi}",
                    DiemDanhGiaDA = Math.Clamp(chiTiet?.DiemDanhGiaDA ?? DiemToiThieu, DiemToiThieu, DiemToiDa),
                    NhanXetDuAn = chiTiet?.NhanXetDuAn
                };
            }).ToList();

            var diemTong = TinhDiemTongKet(danhSachTieuChi.Select(x => x.DiemDanhGiaDA));
            var trangThaiDanhGia = ChuanHoaTrangThaiDanhGia(danhGia?.TrangThaiDanhGiaDA);
            var biKhoa = LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.DaDuyet) || LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.ChoDuyet);
            var thongKe = await XayDungThongKeDuAnAsync(maDuAn);

            return new DanhGiaDuAnFormViewModel
            {
                MaDanhGiaDuAn = danhGia?.MaDanhGiaDuAn,
                MaDuAn = maDuAn,
                TenDuAn = duAn.TenDuAn ?? $"Du an {duAn.MaDuAn}",
                TenNguoiQuanLy = thongKe.TenNguoiQuanLy,
                TrangThaiDuAn = TrangThai.ToDisplay(duAn.TrangThaiDuAn),
                PhanTramHoanThanh = thongKe.PhanTramHoanThanh,
                NgayBatDauDuAn = thongKe.NgayBatDauDuAn,
                NgayKetThucDuAn = thongKe.NgayKetThucDuAn,
                TongCongViec = thongKe.TongCongViec,
                CongViecHoanThanh = thongKe.CongViecHoanThanh,
                CongViecTreHan = thongKe.CongViecTreHan,
                TongChiTietCongViec = thongKe.TongChiTietCongViec,
                ChiTietHoanThanh = thongKe.ChiTietHoanThanh,
                ChiTietTreHan = thongKe.ChiTietTreHan,
                SoBaoCaoTienDo = thongKe.SoBaoCaoTienDo,
                SoBaoCaoMoiNhat = thongKe.SoBaoCaoMoiNhat,
                TongNganSach = thongKe.TongNganSach,
                TongChiPhi = thongKe.TongChiPhi,
                TyLeSuDungNganSach = thongKe.TyLeSuDungNganSach,
                CoDuLieuAi = thongKe.CoDuLieuAi,
                DuAnBiTreTheoAi = thongKe.DuAnBiTreTheoAi,
                TenNguyenNhanAiDuDoan = thongKe.TenNguyenNhanAiDuDoan,
                DoTinCayAi = thongKe.DoTinCayAi,
                ThoiGianDuDoanAi = thongKe.ThoiGianDuDoanAi,
                TenNguyenNhanManagerXacNhan = thongKe.TenNguyenNhanManagerXacNhan,
                DoTinCayManagerXacNhan = thongKe.DoTinCayManagerXacNhan,
                TrangThaiDuLieuAi = thongKe.TrangThaiDuLieuAi,
                TieuChi = danhSachTieuChi,
                NhanXetTongDuAn = danhGia?.NhanXetTongDuAn,
                DiemTongKet = diemTong,
                XepLoai = TinhXepLoai(diemTong),
                TrangThaiDanhGia = trangThaiDanhGia,
                CoTheLuu = !biKhoa,
                CoTheGuiDuyet = !biKhoa && ChoPhepGuiDuyetTheoTrangThaiDuAn(duAn.TrangThaiDuAn),
                CoTheDuyet = false,
                CoTheTuChoi = false,
                ThongKe = thongKe
            };
        }

        public async Task LuuDanhGiaAsync(DanhGiaDuAnFormViewModel form)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaDuAn.DanhGia, Permissions.DanhGiaDuAn.Sua);

            if (form.MaDuAn <= 0)
            {
                throw new Exception("Du an khong hop le.");
            }

            if (form.TieuChi.Count == 0)
            {
                throw new Exception("Danh gia du an phai co it nhat mot tieu chi.");
            }

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminTacNghiep(roleFlags);

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == form.MaDuAn && x.IsDeleted != true);
            if (duAn == null)
            {
                throw new Exception("Khong tim thay du an.");
            }

            if (!roleFlags.IsManager || duAn.MaNguoiDung != currentUserId)
            {
                throw new Exception("Ban khong co quyen tao hoac sua danh gia du an nay.");
            }

            KiemTraHopLeDuLieuTieuChi(form.TieuChi.Select(x => (x.DiemDanhGiaDA, x.NhanXetDuAn)).ToList());
            if (!string.IsNullOrWhiteSpace(form.NhanXetTongDuAn) && form.NhanXetTongDuAn.Trim().Length > DoDaiNhanXetToiDa)
            {
                throw new Exception($"Nhan xet tong du an toi da {DoDaiNhanXetToiDa} ky tu.");
            }

            DanhGiaDuAn? entity = null;
            if (form.MaDanhGiaDuAn.HasValue && form.MaDanhGiaDuAn.Value > 0)
            {
                entity = await _context.DanhGiaDuAn
                    .FirstOrDefaultAsync(x => x.MaDanhGiaDuAn == form.MaDanhGiaDuAn.Value && x.IsDeleted != true);
            }

            if (entity == null)
            {
                entity = new DanhGiaDuAn
                {
                    MaDuAn = form.MaDuAn,
                    MaNguoiDung = currentUserId,
                    IsDeleted = false
                };
                _context.DanhGiaDuAn.Add(entity);
            }
            else
            {
                if (entity.MaNguoiDung != currentUserId || entity.MaDuAn != form.MaDuAn)
                {
                    throw new Exception("Ban khong co quyen sua danh gia du an nay.");
                }

                var trangThaiDanhGia = ChuanHoaTrangThaiDanhGia(entity.TrangThaiDanhGiaDA);
                if (LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.DaDuyet))
                {
                    throw new Exception("Danh gia da duyet khong the chinh sua.");
                }

                if (LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.ChoDuyet))
                {
                    throw new Exception("Danh gia dang cho duyet, khong the chinh sua.");
                }
            }

            var diemTong = TinhDiemTongKet(form.TieuChi.Select(x => x.DiemDanhGiaDA));

            entity.NhanXetTongDuAn = form.NhanXetTongDuAn?.Trim();
            entity.DiemTongDanhGiaDA = (int)Math.Round(diemTong, MidpointRounding.AwayFromZero);
            entity.NgayDanhGiaDA = DateTime.Now;
            entity.TrangThaiDanhGiaDA = TrangThaiNhap;
            entity.MaNguoiDungDuyet = null;
            entity.NgayDuyetDanhGiaDA = null;
            entity.LyDoTuChoiDanhGiaDA = null;

            await _context.SaveChangesAsync();

            var chiTietCu = await _context.CtDanhGiaDuAn
                .Where(x => x.MaDanhGiaDuAn == entity.MaDanhGiaDuAn && x.IsDeleted != true)
                .ToListAsync();

            foreach (var item in chiTietCu)
            {
                item.IsDeleted = true;
                item.DeletedAt = DateTime.Now;
                item.DeletedBy = currentUserId;
            }

            foreach (var tieuChi in form.TieuChi)
            {
                _context.CtDanhGiaDuAn.Add(new CtDanhGiaDuAn
                {
                    MaDanhGiaDuAn = entity.MaDanhGiaDuAn,
                    MaTieuChi = tieuChi.MaTieuChi,
                    NhanXetDuAn = tieuChi.NhanXetDuAn?.Trim(),
                    DiemDanhGiaDA = tieuChi.DiemDanhGiaDA,
                    IsDeleted = false
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task GuiDuyetAsync(int maDanhGiaDuAn)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaDuAn.DanhGia, Permissions.DanhGiaDuAn.Sua);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminTacNghiep(roleFlags);

            var entity = await _context.DanhGiaDuAn
                .FirstOrDefaultAsync(x => x.MaDanhGiaDuAn == maDanhGiaDuAn && x.IsDeleted != true);
            if (entity == null)
            {
                throw new Exception("Khong tim thay ban danh gia du an.");
            }

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == entity.MaDuAn && x.IsDeleted != true);
            if (duAn == null)
            {
                throw new Exception("Du an khong ton tai hoac da bi xoa.");
            }

            if (!roleFlags.IsManager || duAn.MaNguoiDung != currentUserId || entity.MaNguoiDung != currentUserId)
            {
                throw new Exception("Ban khong co quyen gui duyet danh gia du an nay.");
            }

            var trangThaiDanhGia = ChuanHoaTrangThaiDanhGia(entity.TrangThaiDanhGiaDA);
            if (!LaTrangThaiDanhGia(trangThaiDanhGia, TrangThaiNhap) && !LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.TuChoi))
            {
                throw new Exception("Chi ban danh gia nhap hoac tu choi moi duoc gui duyet.");
            }

            if (!ChoPhepGuiDuyetTheoTrangThaiDuAn(duAn.TrangThaiDuAn))
            {
                throw new Exception("Chi du an da hoan thanh hoac cho xac nhan hoan thanh moi duoc gui duyet danh gia.");
            }

            var soTieuChi = await _context.CtDanhGiaDuAn
                .CountAsync(x => x.MaDanhGiaDuAn == maDanhGiaDuAn && x.IsDeleted != true);

            if (soTieuChi <= 0)
            {
                throw new Exception("Danh gia du an chua co chi tiet tieu chi.");
            }

            entity.TrangThaiDanhGiaDA = TrangThai.ChoDuyet;
            entity.MaNguoiDungDuyet = null;
            entity.NgayDuyetDanhGiaDA = null;
            entity.LyDoTuChoiDanhGiaDA = null;
            entity.NgayDanhGiaDA = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task DuyetAsync(int maDanhGiaDuAn)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaDuAn.Duyet);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            if (!roleFlags.IsAdmin)
            {
                throw new Exception("Chi Admin duoc phep duyet danh gia du an.");
            }

            var entity = await _context.DanhGiaDuAn
                .FirstOrDefaultAsync(x => x.MaDanhGiaDuAn == maDanhGiaDuAn && x.IsDeleted != true);
            if (entity == null)
            {
                throw new Exception("Khong tim thay ban danh gia du an.");
            }

            var trangThaiDanhGia = ChuanHoaTrangThaiDanhGia(entity.TrangThaiDanhGiaDA);
            if (!LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.ChoDuyet))
            {
                throw new Exception("Chi ban danh gia dang cho duyet moi duoc phe duyet.");
            }

            entity.TrangThaiDanhGiaDA = TrangThai.DaDuyet;
            entity.MaNguoiDungDuyet = currentUserId;
            entity.NgayDuyetDanhGiaDA = DateTime.Now;
            entity.LyDoTuChoiDanhGiaDA = null;
            await _context.SaveChangesAsync();
        }

        public async Task TuChoiAsync(int maDanhGiaDuAn, string lyDoTuChoi)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaDuAn.Duyet);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            if (!roleFlags.IsAdmin)
            {
                throw new Exception("Chi Admin duoc phep tu choi danh gia du an.");
            }

            if (string.IsNullOrWhiteSpace(lyDoTuChoi))
            {
                throw new Exception("Vui long nhap ly do tu choi.");
            }

            var lyDo = lyDoTuChoi.Trim();
            if (lyDo.Length > DoDaiNhanXetToiDa)
            {
                throw new Exception($"Ly do tu choi toi da {DoDaiNhanXetToiDa} ky tu.");
            }

            var entity = await _context.DanhGiaDuAn
                .FirstOrDefaultAsync(x => x.MaDanhGiaDuAn == maDanhGiaDuAn && x.IsDeleted != true);
            if (entity == null)
            {
                throw new Exception("Khong tim thay ban danh gia du an.");
            }

            var trangThaiDanhGia = ChuanHoaTrangThaiDanhGia(entity.TrangThaiDanhGiaDA);
            if (!LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.ChoDuyet))
            {
                throw new Exception("Chi ban danh gia dang cho duyet moi duoc tu choi.");
            }

            entity.TrangThaiDanhGiaDA = TrangThai.TuChoi;
            entity.MaNguoiDungDuyet = currentUserId;
            entity.NgayDuyetDanhGiaDA = DateTime.Now;
            entity.LyDoTuChoiDanhGiaDA = lyDo;
            await _context.SaveChangesAsync();
        }

        public async Task<DanhGiaDuAnChiTietViewModel> GetChiTietAsync(int maDanhGiaDuAn)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaDuAn.Xem);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();

            var data = await (
                from dg in _context.DanhGiaDuAn
                join da in _context.DuAn on dg.MaDuAn equals da.MaDuAn
                join nguoiTao in _context.NguoiDung on dg.MaNguoiDung equals nguoiTao.MaNguoiDung
                join nguoiQuanLy in _context.NguoiDung on da.MaNguoiDung equals nguoiQuanLy.MaNguoiDung
                join nguoiDuyet in _context.NguoiDung on dg.MaNguoiDungDuyet equals nguoiDuyet.MaNguoiDung into reviewJoin
                from nguoiDuyet in reviewJoin.DefaultIfEmpty()
                where dg.MaDanhGiaDuAn == maDanhGiaDuAn
                      && dg.IsDeleted != true
                      && da.IsDeleted != true
                select new
                {
                    dg.MaDanhGiaDuAn,
                    dg.MaDuAn,
                    TenDuAn = da.TenDuAn,
                    TrangThaiDuAn = da.TrangThaiDuAn,
                    MaNguoiQuanLy = da.MaNguoiDung,
                    TenNguoiQuanLy = nguoiQuanLy.HoTenNguoiDung,
                    MaNguoiDanhGia = dg.MaNguoiDung,
                    TenNguoiDanhGia = nguoiTao.HoTenNguoiDung,
                    dg.DiemTongDanhGiaDA,
                    dg.NhanXetTongDuAn,
                    dg.NgayDanhGiaDA,
                    TrangThaiDanhGia = dg.TrangThaiDanhGiaDA,
                    TenNguoiDuyet = nguoiDuyet.HoTenNguoiDung,
                    NgayDuyet = dg.NgayDuyetDanhGiaDA,
                    LyDoTuChoi = dg.LyDoTuChoiDanhGiaDA
                }).FirstOrDefaultAsync();

            if (data == null)
            {
                throw new Exception("Khong tim thay ban danh gia du an.");
            }

            var coQuyen = await CoQuyenXemDanhGiaDuAnAsync(data.MaDuAn, currentUserId, roleFlags);
            if (!coQuyen)
            {
                throw new Exception("Ban khong co quyen xem danh gia du an nay.");
            }

            var chiTietRows = await (
                from ct in _context.CtDanhGiaDuAn
                join tc in _context.TieuChiDanhGia on ct.MaTieuChi equals tc.MaTieuChi into tcJoin
                from tc in tcJoin.DefaultIfEmpty()
                where ct.MaDanhGiaDuAn == maDanhGiaDuAn && ct.IsDeleted != true
                orderby ct.MaChiTietDGDA
                select new DanhGiaDuAnTieuChiViewModel
                {
                    MaChiTietDGDA = ct.MaChiTietDGDA,
                    MaTieuChi = ct.MaTieuChi ?? 0,
                    TenTieuChi = tc != null && !string.IsNullOrWhiteSpace(tc.TenTieuChi)
                        ? tc.TenTieuChi!
                        : $"Tieu chi {ct.MaChiTietDGDA}",
                    DiemDanhGiaDA = Math.Clamp(ct.DiemDanhGiaDA ?? DiemToiThieu, DiemToiThieu, DiemToiDa),
                    NhanXetDuAn = ct.NhanXetDuAn
                }).ToListAsync();

            var diemTong = data.DiemTongDanhGiaDA ?? (int)Math.Round(TinhDiemTongKet(chiTietRows.Select(x => x.DiemDanhGiaDA)), MidpointRounding.AwayFromZero);
            var thongKe = await XayDungThongKeDuAnAsync(data.MaDuAn);

            return new DanhGiaDuAnChiTietViewModel
            {
                MaDanhGiaDuAn = data.MaDanhGiaDuAn,
                MaDuAn = data.MaDuAn,
                TenDuAn = data.TenDuAn ?? $"Du an {data.MaDuAn}",
                TenNguoiQuanLy = data.TenNguoiQuanLy ?? $"Nguoi dung {data.MaNguoiQuanLy}",
                TrangThaiDuAn = TrangThai.ToDisplay(data.TrangThaiDuAn),
                PhanTramHoanThanh = thongKe.PhanTramHoanThanh,
                NgayBatDauDuAn = thongKe.NgayBatDauDuAn,
                NgayKetThucDuAn = thongKe.NgayKetThucDuAn,
                TongCongViec = thongKe.TongCongViec,
                CongViecHoanThanh = thongKe.CongViecHoanThanh,
                CongViecTreHan = thongKe.CongViecTreHan,
                TongChiTietCongViec = thongKe.TongChiTietCongViec,
                ChiTietHoanThanh = thongKe.ChiTietHoanThanh,
                ChiTietTreHan = thongKe.ChiTietTreHan,
                SoBaoCaoTienDo = thongKe.SoBaoCaoTienDo,
                SoBaoCaoMoiNhat = thongKe.SoBaoCaoMoiNhat,
                TongNganSach = thongKe.TongNganSach,
                TongChiPhi = thongKe.TongChiPhi,
                TyLeSuDungNganSach = thongKe.TyLeSuDungNganSach,
                CoDuLieuAi = thongKe.CoDuLieuAi,
                DuAnBiTreTheoAi = thongKe.DuAnBiTreTheoAi,
                TenNguyenNhanAiDuDoan = thongKe.TenNguyenNhanAiDuDoan,
                DoTinCayAi = thongKe.DoTinCayAi,
                ThoiGianDuDoanAi = thongKe.ThoiGianDuDoanAi,
                TenNguyenNhanManagerXacNhan = thongKe.TenNguyenNhanManagerXacNhan,
                DoTinCayManagerXacNhan = thongKe.DoTinCayManagerXacNhan,
                TrangThaiDuLieuAi = thongKe.TrangThaiDuLieuAi,
                TrangThaiDanhGia = ChuanHoaTrangThaiDanhGia(data.TrangThaiDanhGia),
                DiemTongKet = diemTong,
                XepLoai = TinhXepLoai(diemTong),
                NhanXetTongDuAn = data.NhanXetTongDuAn,
                NgayDanhGia = data.NgayDanhGiaDA,
                TenNguoiDanhGia = data.TenNguoiDanhGia ?? $"Nguoi dung {data.MaNguoiDanhGia}",
                TenNguoiDuyet = data.TenNguoiDuyet,
                NgayDuyet = data.NgayDuyet,
                LyDoTuChoi = data.LyDoTuChoi,
                TieuChi = chiTietRows,
                ThongKe = thongKe
            };
        }

        private async Task<List<DanhGiaDuAnDuAnOptionViewModel>> LayDanhSachDuAnTheoScopeAsync(int currentUserId, (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags)
        {
            IQueryable<DuAn> query = _context.DuAn.Where(x => x.IsDeleted != true);

            if (!roleFlags.IsAdmin)
            {
                query = query.Where(x =>
                    x.MaNguoiDung == currentUserId
                    || _context.NhanVienDuAn.Any(nv => nv.MaDuAn == x.MaDuAn && nv.MaNguoiDung == currentUserId));
            }

            return await query
                .OrderBy(x => x.TenDuAn)
                .Select(x => new DanhGiaDuAnDuAnOptionViewModel
                {
                    MaDuAn = x.MaDuAn,
                    TenDuAn = x.TenDuAn ?? $"Du an {x.MaDuAn}"
                })
                .ToListAsync();
        }

        private async Task<List<TieuChiDanhGia>> LayTieuChiDanhGiaDuAnAsync()
        {
            var loaiHopLe = new[] { "DANHGIADUAN", "DUAN" };
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

        private async Task<DanhGiaDuAnThongKeViewModel> XayDungThongKeDuAnAsync(int maDuAn)
        {
            var duAn = await (
                from da in _context.DuAn
                join nd in _context.NguoiDung on da.MaNguoiDung equals nd.MaNguoiDung
                where da.MaDuAn == maDuAn && da.IsDeleted != true
                select new
                {
                    da.MaDuAn,
                    TenDuAn = da.TenDuAn,
                    TenNguoiQuanLy = nd.HoTenNguoiDung,
                    da.TrangThaiDuAn,
                    da.NgayBatDauDuAn,
                    da.NgayKetThucDuAn,
                    da.PhanTramHoanThanh
                }).FirstOrDefaultAsync();

            if (duAn == null)
            {
                var thongKeRong = new DanhGiaDuAnThongKeViewModel();
                GanDuLieuAiMacDinh(thongKeRong);
                return thongKeRong;
            }

            var congViecQuery =
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where dm.MaDuAn == maDuAn
                      && dm.IsDeleted != true
                      && cv.IsDeleted != true
                select cv;

            var trangThaiHoanThanhCongViec = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
            var soCongViec = await congViecQuery.CountAsync();
            var soCongViecHoanThanh = await congViecQuery.CountAsync(x =>
                x.TrangThaiCongViec != null
                && trangThaiHoanThanhCongViec.Contains(x.TrangThaiCongViec));
            var soCongViecTreHan = await congViecQuery.CountAsync(x =>
                x.NgayKetThucCVDuKien.HasValue
                && x.NgayKetThucCVDuKien.Value < DateTime.Now
                && (x.TrangThaiCongViec == null
                    || !trangThaiHoanThanhCongViec.Contains(x.TrangThaiCongViec)));

            var chiTietQuery =
                from ct in _context.CtCongViec
                join cv in _context.CongViec on ct.MaCongViec equals cv.MaCongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where dm.MaDuAn == maDuAn
                      && dm.IsDeleted != true
                      && cv.IsDeleted != true
                      && ct.IsDeleted != true
                select ct;

            var soChiTiet = await chiTietQuery.CountAsync();
            var maChiTietIds = await chiTietQuery.Select(x => x.MaChiTietCV).Distinct().ToListAsync();
            var soChiTietHoanThanh = await chiTietQuery.CountAsync(x =>
                x.TrangThaiCTCV != null
                && trangThaiHoanThanhCongViec.Contains(x.TrangThaiCTCV));
            var soChiTietTreHan = await chiTietQuery.CountAsync(x =>
                x.NgayKetThucCTCV.HasValue
                && x.NgayKetThucCTCV.Value < DateTime.Now
                && (x.TrangThaiCTCV == null
                    || !trangThaiHoanThanhCongViec.Contains(x.TrangThaiCTCV)));

            var soBaoCaoTienDo = await _context.TienDoCongViec
                .CountAsync(x => maChiTietIds.Contains(x.MaChiTietCV));
            var lanBaoCaoMoiNhat = await _context.TienDoCongViec
                .Where(x => maChiTietIds.Contains(x.MaChiTietCV))
                .MaxAsync(x => (DateTime?)x.ThoiGianCapNhat);
            var soBaoCaoMoiNhat = 0;
            if (lanBaoCaoMoiNhat.HasValue)
            {
                var batDauNgay = lanBaoCaoMoiNhat.Value.Date;
                var ketThucNgay = batDauNgay.AddDays(1);
                soBaoCaoMoiNhat = await _context.TienDoCongViec
                    .CountAsync(x =>
                        maChiTietIds.Contains(x.MaChiTietCV)
                        && x.ThoiGianCapNhat.HasValue
                        && x.ThoiGianCapNhat.Value >= batDauNgay
                        && x.ThoiGianCapNhat.Value < ketThucNgay);
            }

            var tyLe = soCongViec <= 0 ? 0d : Math.Round((double)soCongViecHoanThanh / soCongViec * 100d, 2);

            var trangThaiDaDuyetNganSach = TrangThai.GetCommonStatusVariants(TrangThai.DaDuyet);
            var tongNganSachDaDuyet = await _context.NganSach
                .Where(x =>
                    x.MaDuAn == maDuAn
                    && x.IsDeleted != true
                    && x.TrangThaiNganSach != null
                    && trangThaiDaDuyetNganSach.Contains(x.TrangThaiNganSach))
                .SumAsync(x => x.SoTienNganSach ?? 0m);

            var tongChiPhiDaDung = await (
                from cp in _context.ChiPhi
                join ns in _context.NganSach on cp.MaNganSach equals ns.MaNganSach
                where ns.MaDuAn == maDuAn
                      && cp.IsDeleted != true
                      && ns.IsDeleted != true
                select cp.SoTienDaChi ?? 0m
            ).SumAsync();
            var tyLeSuDungNganSach = tongNganSachDaDuyet <= 0m
                ? 0d
                : Math.Round((double)(tongChiPhiDaDung / tongNganSachDaDuyet) * 100d, 2);

            var soFileDuAn = await _context.FileDuAn.CountAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            var thongKe = new DanhGiaDuAnThongKeViewModel
            {
                TenDuAn = duAn.TenDuAn ?? $"Du an {duAn.MaDuAn}",
                TenNguoiQuanLy = duAn.TenNguoiQuanLy ?? $"Nguoi dung {duAn.MaDuAn}",
                TrangThaiDuAn = TrangThai.ToDisplay(duAn.TrangThaiDuAn),
                NgayBatDauDuAn = duAn.NgayBatDauDuAn,
                NgayKetThucDuAn = duAn.NgayKetThucDuAn,
                PhanTramHoanThanh = Math.Clamp(duAn.PhanTramHoanThanh ?? 0, 0, 100),
                TongCongViec = soCongViec,
                CongViecHoanThanh = soCongViecHoanThanh,
                CongViecTreHan = soCongViecTreHan,
                TongChiTietCongViec = soChiTiet,
                ChiTietHoanThanh = soChiTietHoanThanh,
                ChiTietTreHan = soChiTietTreHan,
                TyLeHoanThanh = tyLe,
                SoBaoCaoTienDo = soBaoCaoTienDo,
                SoBaoCaoMoiNhat = soBaoCaoMoiNhat,
                TongNganSach = tongNganSachDaDuyet,
                TongChiPhi = tongChiPhiDaDung,
                TyLeSuDungNganSach = tyLeSuDungNganSach,
                SoFileDuAn = soFileDuAn
            };

            await NapDuLieuAiThamKhaoAsync(thongKe, maDuAn);
            return thongKe;
        }

        private async Task NapDuLieuAiThamKhaoAsync(DanhGiaDuAnThongKeViewModel thongKe, int maDuAn)
        {
            GanDuLieuAiMacDinh(thongKe);

            var duLieuAi = await (
                from kq in _context.AiKetQua
                join dm in _context.DmNguyenNhan on kq.MaDMNguyenNhan equals dm.MaDMNguyenNhan into dmJoin
                from dm in dmJoin.DefaultIfEmpty()
                where kq.MaDuAn == maDuAn
                orderby kq.ThoiGianDuDoanKetQua descending, kq.MaAiKetQua descending
                select new
                {
                    kq.DoTinCayKetQua,
                    kq.ThoiGianDuDoanKetQua,
                    TenNguyenNhan = dm != null ? dm.TenNguyenNhan : null
                }).FirstOrDefaultAsync();

            if (duLieuAi == null)
            {
                return;
            }

            thongKe.CoDuLieuAi = true;
            thongKe.DuAnBiTreTheoAi = true;
            thongKe.TenNguyenNhanAiDuDoan = string.IsNullOrWhiteSpace(duLieuAi.TenNguyenNhan)
                ? "Chưa xác định nguyên nhân"
                : duLieuAi.TenNguyenNhan.Trim();
            thongKe.DoTinCayAi = ChuanHoaDoTinCay(duLieuAi.DoTinCayKetQua);
            thongKe.ThoiGianDuDoanAi = duLieuAi.ThoiGianDuDoanKetQua;
            thongKe.TrangThaiDuLieuAi = "Đã có dữ liệu AI tham khảo";

            var duLieuXacNhan = await (
                from nn in _context.AiNguyenNhan
                join dm in _context.DmNguyenNhan on nn.MaDMNguyenNhan equals dm.MaDMNguyenNhan into dmJoin
                from dm in dmJoin.DefaultIfEmpty()
                where nn.MaDuAn == maDuAn
                      && nn.IsDeleted != true
                orderby nn.MaAINguyenNhan descending
                select new
                {
                    nn.DoTinCay,
                    TenNguyenNhan = dm != null ? dm.TenNguyenNhan : null
                }).FirstOrDefaultAsync();

            if (duLieuXacNhan == null)
            {
                return;
            }

            thongKe.TenNguyenNhanManagerXacNhan = string.IsNullOrWhiteSpace(duLieuXacNhan.TenNguyenNhan)
                ? null
                : duLieuXacNhan.TenNguyenNhan.Trim();
            thongKe.DoTinCayManagerXacNhan = ChuanHoaDoTinCay(duLieuXacNhan.DoTinCay);
        }

        private static void GanDuLieuAiMacDinh(DanhGiaDuAnThongKeViewModel thongKe)
        {
            thongKe.CoDuLieuAi = false;
            thongKe.DuAnBiTreTheoAi = null;
            thongKe.TenNguyenNhanAiDuDoan = null;
            thongKe.DoTinCayAi = null;
            thongKe.ThoiGianDuDoanAi = null;
            thongKe.TenNguyenNhanManagerXacNhan = null;
            thongKe.DoTinCayManagerXacNhan = null;
            thongKe.TrangThaiDuLieuAi = TrangThaiDuLieuAiMacDinh;
        }

        private static double? ChuanHoaDoTinCay(double? doTinCay)
        {
            if (!doTinCay.HasValue)
            {
                return null;
            }

            var giaTri = doTinCay.Value;
            if (giaTri > 1d && giaTri <= 100d)
            {
                giaTri /= 100d;
            }

            return Math.Round(Math.Clamp(giaTri, 0d, 1d), 4);
        }

        private static bool ChoPhepGuiDuyetTheoTrangThaiDuAn(string? trangThaiDuAn)
        {
            return TrangThai.EqualsValue(trangThaiDuAn, TrangThai.HoanThanh)
                   || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.ChoXacNhanHoanThanh);
        }

        private static void KiemTraHopLeDuLieuTieuChi(List<(int Diem, string? NhanXet)> tieuChi)
        {
            foreach (var item in tieuChi)
            {
                if (item.Diem < DiemToiThieu || item.Diem > DiemToiDa)
                {
                    throw new Exception("Diem tung tieu chi phai nam trong khoang 1 den 10.");
                }

                if (!string.IsNullOrWhiteSpace(item.NhanXet) && item.NhanXet.Trim().Length > DoDaiNhanXetToiDa)
                {
                    throw new Exception($"Nhan xet tung tieu chi toi da {DoDaiNhanXetToiDa} ky tu.");
                }
            }
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

        private static bool CoQuyenSuaDanhGiaDuAn(
            (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags,
            int currentUserId,
            int maNguoiDanhGia,
            int maNguoiQuanLy,
            string trangThaiDanhGia)
        {
            if (roleFlags.IsAdmin)
            {
                return false;
            }

            if (!roleFlags.IsManager)
            {
                return false;
            }

            if (currentUserId != maNguoiDanhGia || currentUserId != maNguoiQuanLy)
            {
                return false;
            }

            return LaTrangThaiDanhGia(trangThaiDanhGia, TrangThaiNhap)
                   || LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.TuChoi);
        }

        private static bool CoQuyenDuyetDanhGiaDuAn((bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags, string trangThaiDanhGia)
        {
            return roleFlags.IsAdmin && LaTrangThaiDanhGia(trangThaiDanhGia, TrangThai.ChoDuyet);
        }

        private async Task<bool> CoQuyenXemDanhGiaDuAnAsync(
            int maDuAn,
            int currentUserId,
            (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags)
        {
            if (roleFlags.IsAdmin)
            {
                return true;
            }

            var laQuanLy = await _context.DuAn
                .AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true && x.MaNguoiDung == currentUserId);
            if (laQuanLy)
            {
                return true;
            }

            return await _context.NhanVienDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn && x.MaNguoiDung == currentUserId);
        }

        private static void KiemTraKhongChoAdminTacNghiep((bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags)
        {
            if (roleFlags.IsAdmin)
            {
                throw new Exception("Tai khoan Admin khong duoc tac nghiep danh gia du an.");
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
                throw new Exception("Ban khong co quyen truy cap chuc nang danh gia du an.");
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

