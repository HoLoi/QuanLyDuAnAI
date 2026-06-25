using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.CongViec;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class CongViecService : ICongViecService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITrangThaiWorkflowService _trangThaiWorkflowService;

        public CongViecService(
            QuanLyDuAnDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ITrangThaiWorkflowService trangThaiWorkflowService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _trangThaiWorkflowService = trangThaiWorkflowService;
        }

        public async Task<CongViecPageViewModel> GetPageAsync(
            int? locMaDuAn,
            string? locTrangThai,
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan,
            int pageNumber = 1,
            int pageSize = PaginationViewModel.DefaultPageSize,
            bool paginate = true)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var (isAdmin, isManager, isEmployee) = await GetCurrentUserRoleFlagsAsync();
            var allowedProjectIds = await GetAccessibleProjectIdsAsync();
            var projectOptions = await GetProjectOptionsAsync(allowedProjectIds);
            var (tuNgayLoc, denNgayLoc) = ChuanHoaKhoangNgay(tuNgay, denNgay);
            var locTheoNgayResolved = string.IsNullOrWhiteSpace(locTheoNgay) ? "NgayTao" : locTheoNgay.Trim();
            var locTinhTrangThoiHanResolved = CongViecDeadlineStatus.NormalizeFilter(locTinhTrangThoiHan);
            var now = DateTime.Now;

            var query =
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                join da in _context.DuAn on dm.MaDuAn equals da.MaDuAn
                join md in _context.MucDoUuTien on cv.MaMucDo equals md.MaMucDo
                join cp in _context.ChiPhi.Where(x => x.IsDeleted != true) on cv.MaCongViec equals cp.MaCongViec into cpGroup
                where cv.IsDeleted != true && dm.IsDeleted != true && da.IsDeleted != true
                select new CongViecItemViewModel
                {
                    MaCongViec = cv.MaCongViec,
                    MaDanhMucCV = dm.MaDanhMucCV,
                    TenDanhMucCV = dm.TenDanhMucCV ?? string.Empty,
                    MaDuAn = da.MaDuAn,
                    TenDuAn = da.TenDuAn ?? string.Empty,
                    MaMucDo = md.MaMucDo,
                    TenMucDo = md.TenMucDo ?? string.Empty,
                    TenCongViec = cv.TenCongViec ?? string.Empty,
                    MoTaCongViec = cv.MoTaCongViec ?? string.Empty,
                    NgayBatDauCongViec = cv.NgayBatDauCongViec,
                    NgayKetThucCVDuKien = cv.NgayKetThucCVDuKien,
                    NgayKetThucCVThucTe = cv.NgayKetThucCVThucTe,
                    NgayTaoCongViec = cv.NgayTaoCongViec,
                    TrangThaiCongViec = cv.TrangThaiCongViec ?? string.Empty,
                    ChiPhiDaChi = cpGroup.Sum(x => x.SoTienDaChi ?? 0m)
                };

            if (allowedProjectIds.Count > 0)
            {
                query = query.Where(x => allowedProjectIds.Contains(x.MaDuAn));
            }

            if (locMaDuAn.HasValue)
            {
                query = query.Where(x => x.MaDuAn == locMaDuAn.Value);
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    x.TenCongViec.ToLower().Contains(keyword) ||
                    x.MoTaCongViec.ToLower().Contains(keyword) ||
                    x.TenDanhMucCV.ToLower().Contains(keyword) ||
                    x.TenDuAn.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(locTrangThai))
            {
                var filterValues = TrangThai.GetCommonStatusVariants(locTrangThai);
                if (filterValues.Length > 0)
                {
                    query = query.Where(x => filterValues.Contains(x.TrangThaiCongViec));
                }
            }

            if (tuNgayLoc.HasValue)
            {
                query = locTheoNgayResolved == "HanCongViec"
                    ? query.Where(x => x.NgayKetThucCVDuKien.HasValue && x.NgayKetThucCVDuKien.Value >= tuNgayLoc.Value)
                    : query.Where(x => x.NgayTaoCongViec.HasValue && x.NgayTaoCongViec.Value >= tuNgayLoc.Value);
            }

            if (denNgayLoc.HasValue)
            {
                var denNgayDocQuyen = denNgayLoc.Value.AddDays(1);
                query = locTheoNgayResolved == "HanCongViec"
                    ? query.Where(x => x.NgayKetThucCVDuKien.HasValue && x.NgayKetThucCVDuKien.Value < denNgayDocQuyen)
                    : query.Where(x => x.NgayTaoCongViec.HasValue && x.NgayTaoCongViec.Value < denNgayDocQuyen);
            }

            if (!string.IsNullOrWhiteSpace(locTinhTrangThoiHanResolved))
            {
                query = ApDungLocTinhTrangThoiHan(query, locTinhTrangThoiHanResolved, now);
            }

            if (isEmployee && !isManager && !isAdmin)
            {
                var duAnIds = await query
                    .Select(x => x.MaDuAn)
                    .Distinct()
                    .ToListAsync();

                var vaiTroTheoDuAn = await _context.NhanVienDuAn
                    .Where(x => x.MaNguoiDung == currentUserId && duAnIds.Contains(x.MaDuAn))
                    .Select(x => new
                    {
                        x.MaDuAn,
                        x.VaiTroTrongDuAn
                    })
                    .ToListAsync();

                var leaderDuAnSet = vaiTroTheoDuAn
                    .Where(x => TrangThai.EqualsValue(x.VaiTroTrongDuAn, TrangThai.VaiTroLeader))
                    .Select(x => x.MaDuAn)
                    .ToHashSet();

                query = query.Where(x =>
                    leaderDuAnSet.Contains(x.MaDuAn)
                    || _context.PhanCongCongViec.Any(pc => pc.MaCongViec == x.MaCongViec && pc.MaNguoiDung == currentUserId));
            }

            var totalItems = await query.CountAsync();
            var pagination = PaginationViewModel.Create(pageNumber, pageSize, totalItems);

            IQueryable<CongViecItemViewModel> danhSachQuery = query
                .OrderByDescending(x => x.NgayTaoCongViec)
                .ThenByDescending(x => x.MaCongViec);

            if (paginate)
            {
                danhSachQuery = danhSachQuery
                    .Skip(pagination.Skip)
                    .Take(pagination.PageSize);
            }

            var danhSach = await danhSachQuery.ToListAsync();

            GanTenHienThiMacDinh(danhSach);
            await GanSoNguoiDuocPhanCongAsync(danhSach);
            await GanSoLuongChiTietCongViecAsync(danhSach);
            await GanCoThePhanCongCongViecAsync(danhSach, currentUserId, isManager, isEmployee, isAdmin);
            await GanCoTheXuLyTrangThaiCongViecAsync(danhSach, currentUserId);
            GanThongTinWorkflowUi(danhSach);
            GanThongTinThoiHan(danhSach, now);

            return new CongViecPageViewModel
            {
                DanhSach = danhSach,
                Pagination = pagination,
                DanhSachDuAn = projectOptions,
                LocMaDuAn = locMaDuAn,
                LocTrangThai = locTrangThai,
                TuKhoa = tuKhoa,
                TuNgay = tuNgayLoc,
                DenNgay = denNgayLoc,
                LocTheoNgay = locTheoNgayResolved,
                LocTinhTrangThoiHan = locTinhTrangThoiHanResolved
            };
        }

        public async Task XacNhanHoanThanhCongViecAsync(int maCongViec)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var (congViec, duAn) = await LayThongTinCongViecVaDuAnAsync(maCongViec);

            await KiemTraQuyenXuLyTrangThaiCongViecAsync(congViec.MaCongViec, duAn.MaDuAn, currentUserId);

            if (!TrangThai.EqualsValue(congViec.TrangThaiCongViec, TrangThai.ChoXacNhanHoanThanh))
                throw new Exception("Công việc phải ở trạng thái chờ xác nhận hoàn thành.");

            congViec.TrangThaiCongViec = TrangThai.HoanThanh;
            if (!congViec.NgayKetThucCVThucTe.HasValue)
                congViec.NgayKetThucCVThucTe = DateTime.Now;

            await _context.SaveChangesAsync();

            await _trangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync(
                duAn.MaDuAn,
                currentUserId,
                "Xác nhận hoàn thành công việc");

            await _context.SaveChangesAsync();
        }

        public async Task MoLaiCongViecAsync(int maCongViec, string lyDo)
        {
            if (string.IsNullOrWhiteSpace(lyDo))
                throw new Exception("Vui lòng nhập lý do mở lại công việc.");

            var currentUserId = await GetCurrentUserIdAsync();
            var (congViec, duAn) = await LayThongTinCongViecVaDuAnAsync(maCongViec);

            await KiemTraQuyenXuLyTrangThaiCongViecAsync(congViec.MaCongViec, duAn.MaDuAn, currentUserId);

            if (!TrangThai.LaHoanThanhCongViec(congViec.TrangThaiCongViec))
                throw new Exception("Chỉ có thể mở lại công việc đã hoàn thành.");

            if (TrangThai.LaHoanThanhCongViec(duAn.TrangThaiDuAn))
                throw new Exception("Dự án đang ở trạng thái hoàn thành. Vui lòng mở lại dự án trước.");

            congViec.TrangThaiCongViec = TrangThai.DangThucHien;
            congViec.NgayKetThucCVThucTe = null;

            _context.NhatKyQuanLyDuAn.Add(new Models.Entities.NhatKyQuanLyDuAn
            {
                MaDuAn = duAn.MaDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Mở lại công việc #{congViec.MaCongViec}. Lý do: {lyDo.Trim()}",
                NkThoiGianQLDA = DateTime.Now
            });

            await _context.SaveChangesAsync();

            await _trangThaiWorkflowService.DongBoTrangThaiDuAnTheoCongViecAsync(
                duAn.MaDuAn,
                currentUserId,
                "Mở lại công việc");

            await _context.SaveChangesAsync();
        }

        private async Task GanSoNguoiDuocPhanCongAsync(List<CongViecItemViewModel> danhSach)
        {
            var maCongViecIds = danhSach
                .Select(x => x.MaCongViec)
                .Distinct()
                .ToList();

            if (maCongViecIds.Count == 0)
            {
                return;
            }

            var soNguoiTheoCongViec = await (
                from pc in _context.PhanCongCongViec
                join nd in _context.NguoiDung on pc.MaNguoiDung equals nd.MaNguoiDung
                where maCongViecIds.Contains(pc.MaCongViec)
                      && nd.IsDeleted != true
                group pc by pc.MaCongViec into g
                select new
                {
                    MaCongViec = g.Key,
                    SoNguoi = g.Select(x => x.MaNguoiDung).Distinct().Count()
                })
                .ToDictionaryAsync(x => x.MaCongViec, x => x.SoNguoi);

            foreach (var item in danhSach)
            {
                item.SoNguoiDuocPhanCong = soNguoiTheoCongViec.TryGetValue(item.MaCongViec, out var soNguoi)
                    ? soNguoi
                    : 0;
            }
        }

        private static void GanTenHienThiMacDinh(List<CongViecItemViewModel> danhSach)
        {
            foreach (var item in danhSach)
            {
                if (string.IsNullOrWhiteSpace(item.TenDanhMucCV))
                {
                    item.TenDanhMucCV = $"Danh mục {item.MaDanhMucCV}";
                }

                if (string.IsNullOrWhiteSpace(item.TenDuAn))
                {
                    item.TenDuAn = $"Dự án {item.MaDuAn}";
                }

                if (string.IsNullOrWhiteSpace(item.TenMucDo))
                {
                    item.TenMucDo = $"Mức độ {item.MaMucDo}";
                }
            }
        }

        private async Task GanSoLuongChiTietCongViecAsync(List<CongViecItemViewModel> danhSach)
        {
            var maCongViecIds = danhSach
                .Select(x => x.MaCongViec)
                .Distinct()
                .ToList();

            if (maCongViecIds.Count == 0)
            {
                return;
            }

            var soChiTietTheoCongViec = await _context.CtCongViec
                .Where(x => maCongViecIds.Contains(x.MaCongViec)
                            && x.IsDeleted != true)
                .GroupBy(x => x.MaCongViec)
                .Select(g => new
                {
                    MaCongViec = g.Key,
                    SoLuong = g.Count()
                })
                .ToDictionaryAsync(x => x.MaCongViec, x => x.SoLuong);

            foreach (var item in danhSach)
            {
                item.SoLuongChiTietCongViec = soChiTietTheoCongViec.TryGetValue(item.MaCongViec, out var soLuong)
                    ? soLuong
                    : 0;
            }
        }

        private async Task GanCoThePhanCongCongViecAsync(
            List<CongViecItemViewModel> danhSach,
            int currentUserId,
            bool isManager,
            bool isEmployee,
            bool isAdmin)
        {
            if (danhSach.Count == 0)
            {
                return;
            }

            // Manager/Admin: giữ nguyên luồng phân quyền hiện tại, không chặn theo vai trò dự án.
            if (!isEmployee || isManager || isAdmin)
            {
                foreach (var item in danhSach)
                {
                    item.CoThePhanCongCongViec = true;
                }
                return;
            }

            var duAnIds = danhSach
                .Select(x => x.MaDuAn)
                .Distinct()
                .ToList();

            var vaiTroTheoDuAn = await _context.NhanVienDuAn
                .Where(x => x.MaNguoiDung == currentUserId
                            && duAnIds.Contains(x.MaDuAn))
                .Select(x => new
                {
                    x.MaDuAn,
                    x.VaiTroTrongDuAn
                })
                .ToListAsync();

            var leaderDuAnIds = vaiTroTheoDuAn
                .Where(x => TrangThai.EqualsValue(x.VaiTroTrongDuAn, TrangThai.VaiTroLeader))
                .Select(x => x.MaDuAn)
                .Distinct()
                .ToList();

            var leaderDuAnSet = leaderDuAnIds.ToHashSet();

            foreach (var item in danhSach)
            {
                item.CoThePhanCongCongViec = leaderDuAnSet.Contains(item.MaDuAn);
            }
        }

        private async Task GanCoTheXuLyTrangThaiCongViecAsync(List<CongViecItemViewModel> danhSach, int currentUserId)
        {
            if (danhSach.Count == 0)
                return;

            var maDuAnTheoCongViec = danhSach.ToDictionary(x => x.MaCongViec, x => x.MaDuAn);
            var duAnIds = maDuAnTheoCongViec.Values.Distinct().ToList();

            var managerProjectIds = await _context.DuAn
                .Where(x => duAnIds.Contains(x.MaDuAn) && x.IsDeleted != true && x.MaNguoiDung == currentUserId)
                .Select(x => x.MaDuAn)
                .ToListAsync();

            var leaderVariants = new[] { TrangThai.VaiTroLeader, TrangThai.VaiTroLeaderHienThi };
            var leaderProjectIds = await _context.NhanVienDuAn
                .Where(x => x.MaNguoiDung == currentUserId && duAnIds.Contains(x.MaDuAn))
                .Where(x => leaderVariants.Contains(x.VaiTroTrongDuAn ?? string.Empty))
                .Select(x => x.MaDuAn)
                .ToListAsync();

            var teamLeaderIds = await _context.NhanVienTeam
                .Where(x => x.MaNguoiDung == currentUserId && x.IsLeader == true)
                .Select(x => x.MaTeam)
                .Distinct()
                .ToListAsync();

            var leaderTeamProjectIds = await _context.TeamDuAn
                .Where(x => teamLeaderIds.Contains(x.MaTeam) && duAnIds.Contains(x.MaDuAn))
                .Select(x => x.MaDuAn)
                .Distinct()
                .ToListAsync();

            var allowedProjectIds = managerProjectIds
                .Concat(leaderProjectIds)
                .Concat(leaderTeamProjectIds)
                .Distinct()
                .ToHashSet();

            foreach (var item in danhSach)
            {
                var trangThai = TrangThai.ToCode(item.TrangThaiCongViec);
                var coQuyen = allowedProjectIds.Contains(item.MaDuAn);
                item.CoTheXacNhanHoanThanh = coQuyen && TrangThai.EqualsValue(trangThai, TrangThai.ChoXacNhanHoanThanh);
                item.CoTheMoLai = coQuyen && TrangThai.LaHoanThanhCongViec(trangThai);
            }
        }

        private static void GanThongTinWorkflowUi(List<CongViecItemViewModel> danhSach)
        {
            foreach (var item in danhSach)
            {
                var trangThai = TrangThai.ToCode(item.TrangThaiCongViec);
                item.TrangThaiCongViec = trangThai;
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
                return "Công việc đã hoàn thành và đang ở trạng thái khóa tác nghiệp.";

            if (TrangThai.EqualsValue(trangThai, TrangThai.ChoXacNhanHoanThanh))
                return "Công việc đã đủ điều kiện và đang chờ xác nhận hoàn thành.";

            if (TrangThai.EqualsValue(trangThai, TrangThai.BiCanCan))
                return "Công việc đang bị cản trở, cần xử lý để tiếp tục.";

            return null;
        }

        private static IQueryable<CongViecItemViewModel> ApDungLocTinhTrangThoiHan(
            IQueryable<CongViecItemViewModel> query,
            string locTinhTrangThoiHan,
            DateTime now)
        {
            var trangThaiHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
            var trangThaiChoXacNhan = TrangThai.GetCommonStatusVariants(TrangThai.ChoXacNhanHoanThanh);
            var trangThaiDaHuy = TrangThai.GetCommonStatusVariants(TrangThai.DaHuy);

            return locTinhTrangThoiHan switch
            {
                CongViecDeadlineStatus.FilterQuaHan => query.Where(x =>
                    x.NgayKetThucCVDuKien.HasValue
                    && x.NgayKetThucCVDuKien.Value < now
                    && !trangThaiHoanThanh.Contains(x.TrangThaiCongViec)
                    && !trangThaiChoXacNhan.Contains(x.TrangThaiCongViec)
                    && !trangThaiDaHuy.Contains(x.TrangThaiCongViec)),

                CongViecDeadlineStatus.FilterChoXacNhanTre => query.Where(x =>
                    x.NgayKetThucCVDuKien.HasValue
                    && trangThaiChoXacNhan.Contains(x.TrangThaiCongViec)
                    && ((x.NgayKetThucCVThucTe.HasValue
                            && x.NgayKetThucCVThucTe.Value > x.NgayKetThucCVDuKien.Value)
                        || (!x.NgayKetThucCVThucTe.HasValue
                            && now > x.NgayKetThucCVDuKien.Value))),

                CongViecDeadlineStatus.FilterHoanThanhTre => query.Where(x =>
                    x.NgayKetThucCVDuKien.HasValue
                    && x.NgayKetThucCVThucTe.HasValue
                    && x.NgayKetThucCVThucTe.Value > x.NgayKetThucCVDuKien.Value
                    && trangThaiHoanThanh.Contains(x.TrangThaiCongViec)),

                CongViecDeadlineStatus.FilterHoanThanhDungHan => query.Where(x =>
                    x.NgayKetThucCVDuKien.HasValue
                    && x.NgayKetThucCVThucTe.HasValue
                    && x.NgayKetThucCVThucTe.Value <= x.NgayKetThucCVDuKien.Value
                    && trangThaiHoanThanh.Contains(x.TrangThaiCongViec)),

                CongViecDeadlineStatus.FilterConHan => query.Where(x =>
                    x.NgayKetThucCVDuKien.HasValue
                    && x.NgayKetThucCVDuKien.Value >= now
                    && !trangThaiHoanThanh.Contains(x.TrangThaiCongViec)
                    && !trangThaiChoXacNhan.Contains(x.TrangThaiCongViec)
                    && !trangThaiDaHuy.Contains(x.TrangThaiCongViec)),

                _ => query
            };
        }

        private static void GanThongTinThoiHan(List<CongViecItemViewModel> danhSach, DateTime now)
        {
            foreach (var item in danhSach)
            {
                GanThongTinThoiHan(item, now);
            }
        }

        private static void GanThongTinThoiHan(CongViecItemViewModel item, DateTime now)
        {
            item.IsQuaHan = false;
            item.IsHoanThanhTre = false;
            item.IsHoanThanhDungHan = false;
            item.IsKhongDanhGiaThoiHan = false;
            item.SoNgayTre = 0;

            var trangThai = TrangThai.ToCode(item.TrangThaiCongViec);

            if (TrangThai.EqualsValue(trangThai, TrangThai.DaHuy))
            {
                GanTinhTrangThoiHan(item, CongViecDeadlineStatus.KhongDanhGia, "Không đánh giá", "deadline-neutral");
                item.IsKhongDanhGiaThoiHan = true;
                return;
            }

            if (!item.NgayKetThucCVDuKien.HasValue)
            {
                GanTinhTrangThoiHan(item, CongViecDeadlineStatus.ChuaXacDinh, "Chưa xác định", "deadline-unknown");
                return;
            }

            var ngayDuKien = item.NgayKetThucCVDuKien.Value;

            if (TrangThai.LaHoanThanhCongViec(trangThai))
            {
                if (!item.NgayKetThucCVThucTe.HasValue)
                {
                    GanTinhTrangThoiHan(item, CongViecDeadlineStatus.ChuaXacDinh, "Chưa xác định ngày hoàn thành", "deadline-unknown");
                    return;
                }

                var ngayThucTe = item.NgayKetThucCVThucTe.Value;
                if (ngayThucTe <= ngayDuKien)
                {
                    GanTinhTrangThoiHan(item, CongViecDeadlineStatus.HoanThanhDungHan, "Hoàn thành đúng hạn", "deadline-on-time");
                    item.IsHoanThanhDungHan = true;
                    return;
                }

                item.SoNgayTre = TinhSoNgayTre(ngayDuKien, ngayThucTe);
                GanTinhTrangThoiHan(item, CongViecDeadlineStatus.HoanThanhTre, $"Hoàn thành trễ {item.SoNgayTre} ngày", "deadline-late");
                item.IsHoanThanhTre = true;
                return;
            }

            if (TrangThai.EqualsValue(trangThai, TrangThai.ChoXacNhanHoanThanh))
            {
                if (item.NgayKetThucCVThucTe.HasValue)
                {
                    var ngayHoanTat = item.NgayKetThucCVThucTe.Value;
                    if (ngayHoanTat <= ngayDuKien)
                    {
                        GanTinhTrangThoiHan(item, CongViecDeadlineStatus.HoanTatDungHan, "Hoàn tất đúng hạn", "deadline-on-time");
                        return;
                    }

                    item.SoNgayTre = TinhSoNgayTre(ngayDuKien, ngayHoanTat);
                    GanTinhTrangThoiHan(item, CongViecDeadlineStatus.HoanTatTre, $"Hoàn tất trễ {item.SoNgayTre} ngày", "deadline-pending-late");
                    item.IsQuaHan = true;
                    return;
                }

                if (ngayDuKien < now)
                {
                    item.SoNgayTre = TinhSoNgayTre(ngayDuKien, now);
                    GanTinhTrangThoiHan(item, CongViecDeadlineStatus.QuaHan, $"Quá hạn {item.SoNgayTre} ngày", "deadline-late");
                    item.IsQuaHan = true;
                    return;
                }

                GanTinhTrangThoiHan(item, CongViecDeadlineStatus.ConHan, "Còn hạn", "deadline-on-time");
                return;
            }

            if (ngayDuKien < now)
            {
                item.SoNgayTre = TinhSoNgayTre(ngayDuKien, now);
                var text = TrangThai.EqualsValue(trangThai, TrangThai.TamDung)
                    ? $"Quá hạn {item.SoNgayTre} ngày"
                    : $"Trễ {item.SoNgayTre} ngày";
                GanTinhTrangThoiHan(item, CongViecDeadlineStatus.QuaHan, text, "deadline-late");
                item.IsQuaHan = true;
                return;
            }

            GanTinhTrangThoiHan(item, CongViecDeadlineStatus.ConHan, "Còn hạn", "deadline-on-time");
        }

        private static void GanTinhTrangThoiHan(
            CongViecItemViewModel item,
            string maTinhTrang,
            string tinhTrang,
            string cssClass)
        {
            item.MaTinhTrangThoiHan = maTinhTrang;
            item.TinhTrangThoiHan = tinhTrang;
            item.CssTinhTrangThoiHan = cssClass;
        }

        private static int TinhSoNgayTre(DateTime han, DateTime thucTe)
        {
            var soNgay = (thucTe - han).TotalDays;
            return soNgay > 0 ? Math.Max(1, (int)Math.Ceiling(soNgay)) : 0;
        }

        private async Task<List<int>> GetAccessibleProjectIdsAsync()
        {
            var (isAdmin, isManager, isEmployee) = await GetCurrentUserRoleFlagsAsync();

            if (isAdmin || (!isManager && !isEmployee))
            {
                return await _context.DuAn
                    .Where(x => x.IsDeleted != true)
                    .Select(x => x.MaDuAn)
                    .ToListAsync();
            }

            var currentUserId = await GetCurrentUserIdAsync();
            var projectIds = new List<int>();

            if (isManager)
            {
                var managedIds = await _context.DuAn
                    .Where(x => x.IsDeleted != true && x.MaNguoiDung == currentUserId)
                    .Select(x => x.MaDuAn)
                    .ToListAsync();

                projectIds.AddRange(managedIds);
            }

            if (isEmployee)
            {
                var memberIds = await _context.NhanVienDuAn
                    .Where(x => x.MaNguoiDung == currentUserId)
                    .Select(x => x.MaDuAn)
                    .ToListAsync();

                projectIds.AddRange(memberIds);
            }

            return projectIds.Distinct().ToList();
        }

        private async Task<List<CongViecDuAnOptionViewModel>> GetProjectOptionsAsync(List<int> allowedProjectIds)
        {
            var query = _context.DuAn.Where(x => x.IsDeleted != true);

            if (allowedProjectIds.Count > 0)
            {
                query = query.Where(x => allowedProjectIds.Contains(x.MaDuAn));
            }

            var options = await query
                .OrderBy(x => x.TenDuAn)
                .Select(x => new CongViecDuAnOptionViewModel
                {
                    MaDuAn = x.MaDuAn,
                    TenDuAn = x.TenDuAn ?? string.Empty
                })
                .ToListAsync();

            foreach (var option in options)
            {
                if (string.IsNullOrWhiteSpace(option.TenDuAn))
                {
                    option.TenDuAn = $"Dự án {option.MaDuAn}";
                }
            }

            return options;
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

        private async Task<(bool IsAdmin, bool IsManager, bool IsEmployee)> GetCurrentUserRoleFlagsAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(aspUserId))
                throw new Exception("Không xác định được người dùng hiện tại.");

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

            return (normalizedRoles.Contains("ADMIN"), normalizedRoles.Contains("MANAGER"), normalizedRoles.Contains("EMPLOYEE"));
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

        private async Task<(Models.Entities.CongViec CongViec, Models.Entities.DuAn DuAn)> LayThongTinCongViecVaDuAnAsync(int maCongViec)
        {
            var duLieu = await (
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                join da in _context.DuAn on dm.MaDuAn equals da.MaDuAn
                where cv.MaCongViec == maCongViec
                      && cv.IsDeleted != true
                      && dm.IsDeleted != true
                      && da.IsDeleted != true
                select new { cv, da }
            ).FirstOrDefaultAsync();

            if (duLieu == null)
                throw new Exception("Không tìm thấy công việc.");

            return (duLieu.cv, duLieu.da);
        }

        private async Task KiemTraQuyenXuLyTrangThaiCongViecAsync(int maCongViec, int maDuAn, int currentUserId)
        {
            var coQuyenManager = await _context.DuAn
                .AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true && x.MaNguoiDung == currentUserId);

            if (coQuyenManager)
                return;

            var leaderVariants = new[] { TrangThai.VaiTroLeader, TrangThai.VaiTroLeaderHienThi };
            var laLeaderDuAn = await _context.NhanVienDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn
                            && x.MaNguoiDung == currentUserId
                            && leaderVariants.Contains(x.VaiTroTrongDuAn ?? string.Empty));

            if (laLeaderDuAn)
                return;

            var teamIds = await _context.TeamDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.MaTeam)
                .ToListAsync();

            if (teamIds.Count > 0)
            {
                var laLeaderTeam = await _context.NhanVienTeam
                    .AnyAsync(x => teamIds.Contains(x.MaTeam)
                                && x.MaNguoiDung == currentUserId
                                && x.IsLeader == true);
                if (laLeaderTeam)
                    return;
            }

            throw new Exception("Bạn không có quyền xác nhận hoặc mở lại công việc này.");
        }
    }
}
