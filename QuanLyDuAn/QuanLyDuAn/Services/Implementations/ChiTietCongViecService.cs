using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.ChiTietCongViec;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class ChiTietCongViecService : IChiTietCongViecService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITrangThaiWorkflowService _trangThaiWorkflowService;

        public ChiTietCongViecService(
            QuanLyDuAnDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ITrangThaiWorkflowService trangThaiWorkflowService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _trangThaiWorkflowService = trangThaiWorkflowService;
        }

        public async Task<ChiTietCongViecPageViewModel> GetPageAsync(int maCongViec, int pageNumber = 1, int pageSize = 20, bool paginate = true)
        {
            var congViec = await LayCongViecAsync(maCongViec);
            var coTheCapNhat = await CoTheCapNhatAsync(congViec)
                               && !BiKhoaCapNhatTheoTrangThaiCongViec(congViec.TrangThaiCongViec);
            var coThePhanCongChiTietCongViec = await CoThePhanCongChiTietCongViecAsync(congViec);
            var coTheMoLaiTheoCapTren = coTheCapNhat && await CapTrenChoPhepMoLaiChiTietAsync(congViec);

            var query = _context.CtCongViec
                .Where(x => x.MaCongViec == maCongViec && x.IsDeleted != true);

            var tongSoChiTiet = await query.CountAsync();
            var trangThaiHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
            var soChiTietHoanThanh = await query.CountAsync(x =>
                trangThaiHoanThanh.Contains(x.TrangThaiCTCV ?? string.Empty));
            var pagination = paginate
                ? PaginationViewModel.Create(pageNumber, pageSize, tongSoChiTiet)
                : PaginationViewModel.Create(1, tongSoChiTiet > 0 ? tongSoChiTiet : PaginationViewModel.DefaultPageSize, tongSoChiTiet);

            var pagedQuery = query
                .OrderByDescending(x => x.NgayTaoCTCV)
                .ThenByDescending(x => x.MaChiTietCV)
                .AsQueryable();

            if (paginate)
            {
                pagedQuery = pagedQuery
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize);
            }

            var danhSach = await pagedQuery
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
                    CoThePhanCongChiTietCongViec = coThePhanCongChiTietCongViec,
                    CoTheMoLai = coTheMoLaiTheoCapTren
                        && (TrangThai.EqualsValue(x.TrangThaiCTCV, TrangThai.BiCanCan)
                            || TrangThai.EqualsValue(x.TrangThaiCTCV, TrangThai.TamDung))
                })
                .ToListAsync();

            await GanSoNguoiDuocPhanCongAsync(danhSach);
            GanThongTinWorkflowUi(danhSach);

            var trangThaiCongViec = TrangThai.ToCode(congViec.TrangThaiCongViec);
            var phanTramTienDo = tongSoChiTiet == 0
                ? 0
                : (int)Math.Round((double)soChiTietHoanThanh * 100 / tongSoChiTiet);

            return new ChiTietCongViecPageViewModel
            {
                CoTheCapNhat = coTheCapNhat,
                CongViec = new ChiTietCongViecSummaryViewModel
                {
                    MaCongViec = congViec.MaCongViec,
                    TenCongViec = congViec.TenCongViec ?? string.Empty,
                    TenTrangThai = TrangThai.ToDisplay(trangThaiCongViec),
                    TrangThaiCongViec = trangThaiCongViec,
                    CssTrangThai = LayCssTrangThai(trangThaiCongViec),
                    ThongDiepWorkflow = LayThongDiepWorkflow(trangThaiCongViec),
                    TongSoChiTiet = tongSoChiTiet,
                    SoChiTietHoanThanh = soChiTietHoanThanh,
                    PhanTramTienDo = phanTramTienDo
                },
                Form = new ChiTietCongViecCreateUpdateViewModel
                {
                    MaCongViec = maCongViec,
                    NgayBatDauCTCV = DateTime.Today,
                    TrangThaiCTCV = TrangThai.ChuaBatDau
                },
                DanhSach = danhSach,
                Pagination = pagination
            };
        }

        private async Task GanSoNguoiDuocPhanCongAsync(List<ChiTietCongViecItemViewModel> danhSach)
        {
            var maChiTietIds = danhSach
                .Select(x => x.MaChiTietCV)
                .Distinct()
                .ToList();

            if (maChiTietIds.Count == 0)
            {
                return;
            }

            var soNguoiTheoChiTiet = await (
                from pc in _context.PhanCongCtCongViec
                join nd in _context.NguoiDung on pc.MaNguoiDung equals nd.MaNguoiDung
                where maChiTietIds.Contains(pc.MaChiTietCV)
                      && nd.IsDeleted != true
                group pc by pc.MaChiTietCV into g
                select new
                {
                    MaChiTietCV = g.Key,
                    SoNguoi = g.Select(x => x.MaNguoiDung).Distinct().Count()
                })
                .ToDictionaryAsync(x => x.MaChiTietCV, x => x.SoNguoi);

            foreach (var item in danhSach)
            {
                item.SoNguoiDuocPhanCong = soNguoiTheoChiTiet.TryGetValue(item.MaChiTietCV, out var soNguoi)
                    ? soNguoi
                    : 0;
            }
        }

        public async Task AddAsync(ChiTietCongViecCreateUpdateViewModel form)
        {
            var congViec = await LayCongViecAsync(form.MaCongViec);
            KiemTraDuLieuDauVao(form, congViec);
            await KiemTraQuyenCapNhatAsync(congViec);
            await KiemTraTrangThaiCongViecTruocKhiThemAsync(congViec);
            var currentUserId = await GetCurrentUserIdAsync();

            var entity = new CtCongViec
            {
                MaCongViec = form.MaCongViec,
                TenCTCV = string.IsNullOrWhiteSpace(form.TenCTCV) ? null : form.TenCTCV.Trim(),
                NoiDungChiTietCV = form.NoiDungChiTietCV.Trim(),
                NgayTaoCTCV = DateTime.Now,
                NgayBatDauCTCV = form.NgayBatDauCTCV!.Value.Date,
                NgayKetThucCTCV = null,
                TrangThaiCTCV = TrangThai.ChuaBatDau,
                IsDeleted = false
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.CtCongViec.Add(entity);
                await _context.SaveChangesAsync();

                await _trangThaiWorkflowService.DongBoChuoiTrangThaiTuCongViecAsync(
                    congViec.MaCongViec,
                    currentUserId,
                    "Thêm chi tiết công việc");
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(ChiTietCongViecCreateUpdateViewModel form)
        {
            if (form.MaChiTietCV <= 0)
                throw new Exception("Không tìm thấy chi tiết công việc cần cập nhật.");

            var congViec = await LayCongViecAsync(form.MaCongViec);
            KiemTraDuLieuDauVao(form, congViec);
            await KiemTraQuyenCapNhatAsync(congViec);
            KiemTraTrangThaiCongViecChoCapNhat(congViec, "sửa");
            var currentUserId = await GetCurrentUserIdAsync();

            var entity = await _context.CtCongViec
                .FirstOrDefaultAsync(x =>
                    x.MaChiTietCV == form.MaChiTietCV &&
                    x.MaCongViec == form.MaCongViec &&
                    x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy chi tiết công việc cần cập nhật.");

            entity.TenCTCV = string.IsNullOrWhiteSpace(form.TenCTCV) ? null : form.TenCTCV.Trim();
            entity.NoiDungChiTietCV = form.NoiDungChiTietCV.Trim();
            entity.NgayBatDauCTCV = form.NgayBatDauCTCV!.Value.Date;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.SaveChangesAsync();

                await _trangThaiWorkflowService.DongBoChuoiTrangThaiTuCongViecAsync(
                    congViec.MaCongViec,
                    currentUserId,
                    "Cập nhật chi tiết công việc");
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveAsync(int maCongViec, int maChiTietCv)
        {
            var congViec = await LayCongViecAsync(maCongViec);
            await KiemTraQuyenCapNhatAsync(congViec);
            KiemTraTrangThaiCongViecChoCapNhat(congViec, "xóa");
            var currentUserId = await GetCurrentUserIdAsync();

            var entity = await _context.CtCongViec
                .FirstOrDefaultAsync(x =>
                    x.MaChiTietCV == maChiTietCv &&
                    x.MaCongViec == maCongViec &&
                    x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy chi tiết công việc cần xóa.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.Now;
                entity.DeletedBy = currentUserId;
                await _context.SaveChangesAsync();

                await _trangThaiWorkflowService.DongBoChuoiTrangThaiTuCongViecAsync(
                    congViec.MaCongViec,
                    currentUserId,
                    "Xóa chi tiết công việc");
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task MoLaiChiTietCongViecAsync(int maCongViec, int maChiTietCv, string lyDo)
        {
            if (string.IsNullOrWhiteSpace(lyDo))
                throw new Exception("Vui lòng nhập lý do mở lại chi tiết công việc.");

            var lyDoMoLai = lyDo.Trim();
            if (lyDoMoLai.Length > 255)
                throw new Exception("Lý do mở lại tối đa 255 ký tự.");

            var congViec = await LayCongViecAsync(maCongViec);
            await KiemTraQuyenCapNhatAsync(congViec);
            KiemTraTrangThaiCongViecChoMoLaiChiTiet(congViec);
            var currentUserId = await GetCurrentUserIdAsync();
            var (maDuAn, trangThaiDuAn) = await LayThongTinDuAnTheoCongViecAsync(congViec);

            KiemTraTrangThaiDuAnChoMoLaiChiTiet(trangThaiDuAn);

            var chiTiet = await _context.CtCongViec
                .FirstOrDefaultAsync(x =>
                    x.MaChiTietCV == maChiTietCv &&
                    x.MaCongViec == maCongViec &&
                    x.IsDeleted != true);

            if (chiTiet == null)
                throw new Exception("Không tìm thấy chi tiết công việc cần mở lại.");

            var trangThaiCu = TrangThai.ToCode(chiTiet.TrangThaiCTCV);
            if (TrangThai.LaHoanThanhCongViec(trangThaiCu))
                throw new Exception("Chi tiết công việc đã hoàn thành. Vui lòng mở lại công việc hoặc xử lý theo quy trình hoàn thành.");

            if (TrangThai.EqualsValue(trangThaiCu, TrangThai.DaHuy)
                || TrangThai.EqualsValue(trangThaiCu, TrangThai.LuuTru))
            {
                throw new Exception("Chi tiết công việc đã đóng, không thể mở lại.");
            }

            if (!TrangThai.EqualsValue(trangThaiCu, TrangThai.BiCanCan)
                && !TrangThai.EqualsValue(trangThaiCu, TrangThai.TamDung))
            {
                throw new Exception("Chỉ có thể mở lại chi tiết công việc đang bị cản trở hoặc tạm dừng.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                chiTiet.TrangThaiCTCV = TrangThai.DangThucHien;
                chiTiet.NgayKetThucCTCV = null;

                _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
                {
                    MaDuAn = maDuAn,
                    MaNguoiDung = currentUserId,
                    NkHanhDongQLDA = RutGonNhatKy(
                        $"Mở lại chi tiết công việc #{chiTiet.MaChiTietCV} ({LayTenChiTietCongViec(chiTiet)}) từ trạng thái {TrangThai.ToDisplay(trangThaiCu)} sang {TrangThai.ToDisplay(TrangThai.DangThucHien)}. Người thực hiện #{currentUserId}. Lý do: {lyDoMoLai}"),
                    NkThoiGianQLDA = DateTime.Now
                });

                await _context.SaveChangesAsync();

                await _trangThaiWorkflowService.DongBoChuoiTrangThaiTuCongViecAsync(
                    maCongViec,
                    currentUserId,
                    "Mở lại chi tiết công việc");
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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

        private async Task<(int MaDuAn, string? TrangThaiDuAn)> LayThongTinDuAnTheoCongViecAsync(CongViec congViec)
        {
            var duAn = await (
                from dm in _context.DanhMucCongViec
                join da in _context.DuAn on dm.MaDuAn equals da.MaDuAn
                where dm.MaDanhMucCV == congViec.MaDanhMucCV
                      && dm.IsDeleted != true
                      && da.IsDeleted != true
                select new { da.MaDuAn, da.TrangThaiDuAn }
            ).FirstOrDefaultAsync();

            if (duAn == null)
                throw new Exception("Không tìm thấy dự án của công việc.");

            return (duAn.MaDuAn, duAn.TrangThaiDuAn);
        }

        private async Task<bool> CapTrenChoPhepMoLaiChiTietAsync(CongViec congViec)
        {
            if (TrangThai.LaHoanThanhCongViec(congViec.TrangThaiCongViec)
                || TrangThai.EqualsValue(congViec.TrangThaiCongViec, TrangThai.TamDung)
                || TrangThai.EqualsValue(congViec.TrangThaiCongViec, TrangThai.DaHuy)
                || TrangThai.EqualsValue(congViec.TrangThaiCongViec, TrangThai.LuuTru))
            {
                return false;
            }

            var (_, trangThaiDuAn) = await LayThongTinDuAnTheoCongViecAsync(congViec);
            return !TrangThai.LaHoanThanhCongViec(trangThaiDuAn)
                   && !TrangThai.EqualsValue(trangThaiDuAn, TrangThai.TamDung)
                   && !TrangThai.EqualsValue(trangThaiDuAn, TrangThai.DaHuy)
                   && !TrangThai.EqualsValue(trangThaiDuAn, TrangThai.LuuTru);
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

            var leaderVariants = new[] { TrangThai.VaiTroLeader, TrangThai.VaiTroLeaderHienThi };
            var laLeaderDuAn = await _context.NhanVienDuAn
                .AnyAsync(x =>
                    x.MaDuAn == maDuAn &&
                    x.MaNguoiDung == maNguoiDungHienTai &&
                    leaderVariants.Contains(x.VaiTroTrongDuAn ?? string.Empty));

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

        private async Task KiemTraTrangThaiCongViecTruocKhiThemAsync(CongViec congViec)
        {
            var trangThaiCongViec = TrangThai.ToCode(congViec.TrangThaiCongViec);
            if (TrangThai.LaHoanThanhCongViec(trangThaiCongViec))
            {
                throw new Exception("Công việc đã hoàn thành. Vui lòng mở lại công việc trước khi thêm chi tiết mới.");
            }

            if (TrangThai.EqualsValue(trangThaiCongViec, TrangThai.TamDung)
                || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.DaHuy))
            {
                throw new Exception("Công việc đang tạm dừng hoặc đã hủy, không thể thêm chi tiết mới.");
            }

            var maDuAn = await GetMaDuAnAsync(congViec);
            var trangThaiDuAn = await _context.DuAn
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true)
                .Select(x => x.TrangThaiDuAn)
                .FirstOrDefaultAsync();

            if (TrangThai.LaHoanThanhCongViec(trangThaiDuAn)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.LuuTru)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.DaHuy))
            {
                throw new Exception("Dự án đã hoàn thành hoặc đã đóng. Vui lòng mở lại dự án trước khi phát sinh thêm chi tiết.");
            }
        }

        private static void GanThongTinWorkflowUi(List<ChiTietCongViecItemViewModel> danhSach)
        {
            foreach (var item in danhSach)
            {
                var trangThai = TrangThai.ToCode(item.TrangThaiCTCV);
                item.TrangThaiCTCV = trangThai;
                item.TrangThaiHienThi = TrangThai.ToDisplay(trangThai);
                item.CssTrangThai = LayCssTrangThai(trangThai);
                item.ThongDiepWorkflow = LayThongDiepWorkflow(trangThai);
            }
        }

        private static string LayCssTrangThai(string? trangThai)
        {
            if (TrangThai.LaHoanThanhCongViec(trangThai))
                return "workflow-success";

            if (TrangThai.EqualsValue(trangThai, TrangThai.ChoXacNhanHoanThanh))
                return "workflow-pending";

            if (TrangThai.EqualsValue(trangThai, TrangThai.BiCanCan))
                return "workflow-blocked";

            if (TrangThai.EqualsValue(trangThai, TrangThai.TamDung))
                return "workflow-paused";

            if (TrangThai.EqualsValue(trangThai, TrangThai.DaHuy))
                return "workflow-cancelled";

            if (TrangThai.EqualsValue(trangThai, TrangThai.ChuaBatDau))
                return "workflow-idle";

            return "workflow-active";
        }

        private static string? LayThongDiepWorkflow(string? trangThai)
        {
            if (TrangThai.LaHoanThanhCongViec(trangThai))
                return "Công việc đã hoàn thành, các cập nhật thường sẽ bị khóa.";

            if (TrangThai.EqualsValue(trangThai, TrangThai.ChoXacNhanHoanThanh))
                return "Đang chờ xác nhận hoàn thành, vui lòng xử lý trước khi mở rộng thêm chi tiết.";

            if (TrangThai.EqualsValue(trangThai, TrangThai.BiCanCan))
                return "Công việc đang bị cản trở, cần xử lý blocker để tiếp tục.";

            return null;
        }

        private static void KiemTraTrangThaiCongViecChoCapNhat(CongViec congViec, string thaoTac)
        {
            var trangThaiCongViec = TrangThai.ToCode(congViec.TrangThaiCongViec);
            if (TrangThai.LaHoanThanhCongViec(trangThaiCongViec))
            {
                throw new Exception($"Công việc đã hoàn thành. Vui lòng mở lại công việc trước khi {thaoTac} chi tiết.");
            }

            if (TrangThai.EqualsValue(trangThaiCongViec, TrangThai.TamDung)
                || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.DaHuy))
            {
                throw new Exception("Công việc đang tạm dừng hoặc đã hủy, không thể cập nhật chi tiết.");
            }
        }

        private static void KiemTraTrangThaiCongViecChoMoLaiChiTiet(CongViec congViec)
        {
            var trangThaiCongViec = TrangThai.ToCode(congViec.TrangThaiCongViec);
            if (TrangThai.LaHoanThanhCongViec(trangThaiCongViec))
            {
                throw new Exception("Công việc đã hoàn thành. Vui lòng mở lại công việc trước khi mở lại chi tiết.");
            }

            if (TrangThai.EqualsValue(trangThaiCongViec, TrangThai.TamDung)
                || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.DaHuy)
                || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.LuuTru))
            {
                throw new Exception("Công việc đang tạm dừng hoặc đã đóng, không thể mở lại chi tiết.");
            }
        }

        private static void KiemTraTrangThaiDuAnChoMoLaiChiTiet(string? trangThaiDuAn)
        {
            if (TrangThai.LaHoanThanhCongViec(trangThaiDuAn)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.DaHuy)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.LuuTru)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.TamDung))
            {
                throw new Exception("Dự án đã hoàn thành, tạm dừng hoặc đã đóng, không thể mở lại chi tiết công việc.");
            }
        }

        private static string LayTenChiTietCongViec(CtCongViec chiTiet)
        {
            if (!string.IsNullOrWhiteSpace(chiTiet.TenCTCV))
                return chiTiet.TenCTCV.Trim();

            if (!string.IsNullOrWhiteSpace(chiTiet.NoiDungChiTietCV))
                return chiTiet.NoiDungChiTietCV.Trim();

            return "Không có tên";
        }

        private static string RutGonNhatKy(string noiDung)
        {
            const int maxLength = 255;
            if (noiDung.Length <= maxLength)
                return noiDung;

            return noiDung[..maxLength].TrimEnd();
        }

        private static bool BiKhoaCapNhatTheoTrangThaiCongViec(string? trangThaiCongViec)
        {
            return TrangThai.LaHoanThanhCongViec(trangThaiCongViec)
                   || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.TamDung)
                   || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.DaHuy);
        }
    }
}
