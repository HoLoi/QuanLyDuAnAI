using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.Ai;
using QuanLyDuAn.ViewModels.DanhGiaDuAn;
using System.Security.Claims;
using System.Text.Json;

namespace QuanLyDuAn.Services.Implementations
{
    public class DanhGiaDuAnService : IDanhGiaDuAnService
    {
        private const string TrangThaiNhap = "Nhap";
        private const string TrangThaiDuLieuAiMacDinh = "Chưa có kết quả phân tích nguyên nhân trễ cho dự án này.";
        private const string TrangThaiDuLieuAiChuaDuLabel = "AI_DATASET đã có nhưng chưa đủ dữ liệu để xác định LaDuAnTre.";
        private const string CanhBaoKetQuaAiCoTheDaCu = "Kết quả AI có thể đã cũ so với AI_DATASET hoặc model đang hoạt động. Vui lòng phân tích lại.";
        private const string NguyenNhanKhongTre = "Không có nguyên nhân trễ";
        private const string NguyenNhanChuaDuDuLieu = "Chưa đủ dữ liệu để xác định nguyên nhân";
        private const string TrangThaiThoiHanChuaDenHan = "Chưa đến hạn";
        private const string TrangThaiThoiHanSapDenHan = "Sắp đến hạn";
        private const string TrangThaiThoiHanQuaHan = "Quá hạn";
        private const string TrangThaiThoiHanHoanThanhDungHan = "Hoàn thành đúng hạn";
        private const string TrangThaiThoiHanHoanThanhTreHan = "Hoàn thành trễ hạn";
        private const string TrangThaiThoiHanChuaCoMocKetThuc = "Chưa có mốc kết thúc dự kiến";
        private const string ThongBaoDuAnDungHanKhongCanPhanTich = "Dự án hoàn thành đúng hạn, không cần phân tích nguyên nhân trễ.";
        private const string CanhBaoKetQuaAiMauThuanThucTe = "Kết quả AI mâu thuẫn với trạng thái thực tế của dự án và đã bị bỏ qua.";
        private const string TrangThaiChuaDanhGia = "ChuaDanhGia";
        private const int DiemToiThieu = 1;
        private const int DiemToiDa = 10;
        private const int DoDaiNhanXetToiDa = 500;
        private const int SoNgayCanhBaoSapDenHan = 7;
        private const string AiRelatedReasonsPayloadMarker = "[[AI_RELATED_REASONS_V1]]";

        private readonly QuanLyDuAnDbContext _context;
        private readonly IAiService _aiService;
        private readonly IAiDatasetService _aiDatasetService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DanhGiaDuAnService> _logger;

        private sealed class AiRelatedReasonStoragePayload
        {
            public string? NoiDungPhanTich { get; set; }
            public string? MucPhuHop { get; set; }
            public List<DanhGiaDuAnRelatedReasonViewModel>? DanhSachNguyenNhanLienQuan { get; set; }
        }

        private enum TrangThaiThucTeTienDo
        {
            HoanThanhDungHan = 1,
            HoanThanhTreHan = 2,
            DangThucHienChuaQuaHan = 3,
            DangThucHienQuaHan = 4,
            ChuaDuDuLieuThoiHan = 5
        }

        public DanhGiaDuAnService(
            QuanLyDuAnDbContext context,
            IAiService aiService,
            IAiDatasetService aiDatasetService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DanhGiaDuAnService> logger)
        {
            _context = context;
            _aiService = aiService;
            _aiDatasetService = aiDatasetService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
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

        public async Task<DanhGiaDuAnPageViewModel> GetPageAsync(
            string? tuKhoa,
            string? trangThai,
            int? maDuAn,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia,
            int pageNumber = 1,
            int pageSize = 20,
            bool paginate = true)
        {
            KiemTraQuyenTheoClaim(Permissions.DanhGiaDuAn.Xem);
            var (tuNgayLoc, denNgayLoc) = ChuanHoaKhoangNgay(tuNgayDanhGia, denNgayDanhGia);
            var denNgayDocQuyen = denNgayLoc?.AddDays(1);
            var now = DateTime.Now;

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();

            if (!roleFlags.IsManager)
            {
                return new DanhGiaDuAnPageViewModel
                {
                    TuKhoa = tuKhoa,
                    TrangThai = trangThai,
                    MaDuAn = maDuAn,
                    TuNgayDanhGia = tuNgayLoc,
                    DenNgayDanhGia = denNgayLoc
                };
            }

            var danhSachDuAnTheoScope = await LayDanhSachDuAnTheoScopeAsync(currentUserId, roleFlags);
            var maDuAnTheoScope = danhSachDuAnTheoScope.Select(x => x.MaDuAn).ToHashSet();

            var query =
                from da in _context.DuAn
                join nguoiQuanLy in _context.NguoiDung on da.MaNguoiDung equals nguoiQuanLy.MaNguoiDung
                where da.IsDeleted != true
                select new
                {
                    da.MaDuAn,
                    TenDuAn = da.TenDuAn,
                    da.TrangThaiDuAn,
                    da.NgayKetThucDuAn,
                    da.NgayHoanThanhThucTeDuAn,
                    da.PhanTramHoanThanh,
                    MaNguoiQuanLy = da.MaNguoiDung,
                    TenNguoiQuanLy = nguoiQuanLy.HoTenNguoiDung
                };

            query = query.Where(x => maDuAnTheoScope.Contains(x.MaDuAn));

            if (maDuAn.HasValue)
            {
                query = query.Where(x => x.MaDuAn == maDuAn.Value);
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    (x.TenDuAn ?? string.Empty).ToLower().Contains(keyword)
                    || (x.TenNguoiQuanLy ?? string.Empty).ToLower().Contains(keyword));
            }

            if (tuNgayLoc.HasValue)
            {
                query = query.Where(x => _context.DanhGiaDuAn
                    .Where(dg => dg.IsDeleted != true && dg.MaDuAn == x.MaDuAn)
                    .OrderByDescending(dg => dg.NgayDanhGiaDA)
                    .ThenByDescending(dg => dg.MaDanhGiaDuAn)
                    .Select(dg => dg.NgayDanhGiaDA)
                    .FirstOrDefault() >= tuNgayLoc.Value);
            }

            if (denNgayDocQuyen.HasValue)
            {
                query = query.Where(x => _context.DanhGiaDuAn
                    .Where(dg => dg.IsDeleted != true && dg.MaDuAn == x.MaDuAn)
                    .OrderByDescending(dg => dg.NgayDanhGiaDA)
                    .ThenByDescending(dg => dg.MaDanhGiaDuAn)
                    .Select(dg => dg.NgayDanhGiaDA)
                    .FirstOrDefault() < denNgayDocQuyen.Value);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                if (LaTrangThaiDanhGia(TrangThaiChuaDanhGia, trangThai))
                {
                    query = query.Where(x => !_context.DanhGiaDuAn
                        .Any(dg => dg.IsDeleted != true && dg.MaDuAn == x.MaDuAn));
                }
                else
                {
                    var trangThaiFilterValues = TrangThai.GetCommonStatusVariants(trangThai);
                    if (trangThaiFilterValues.Length > 0)
                    {
                        query = query.Where(x => trangThaiFilterValues.Contains(_context.DanhGiaDuAn
                            .Where(dg => dg.IsDeleted != true && dg.MaDuAn == x.MaDuAn)
                            .OrderByDescending(dg => dg.NgayDanhGiaDA)
                            .ThenByDescending(dg => dg.MaDanhGiaDuAn)
                            .Select(dg => dg.TrangThaiDanhGiaDA ?? string.Empty)
                            .FirstOrDefault() ?? string.Empty));
                    }
                }
            }

            var totalItems = await query.CountAsync();
            var pagination = PaginationViewModel.Create(pageNumber, pageSize, totalItems);

            var summaryRows = await query
                .Select(x => new
                {
                    x.MaDuAn,
                    TrangThaiDanhGia = _context.DanhGiaDuAn
                        .Where(dg => dg.IsDeleted != true && dg.MaDuAn == x.MaDuAn)
                        .OrderByDescending(dg => dg.NgayDanhGiaDA)
                        .ThenByDescending(dg => dg.MaDanhGiaDuAn)
                        .Select(dg => dg.TrangThaiDanhGiaDA)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var orderedQuery = query
                .OrderBy(x => x.TenDuAn)
                .ThenBy(x => x.MaDuAn)
                .AsQueryable();

            if (paginate)
            {
                orderedQuery = orderedQuery
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize);
            }

            var duAnRows = await orderedQuery.ToListAsync();

            var duAnIds = duAnRows.Select(x => x.MaDuAn).Distinct().ToList();
            var trangThaiHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
            var trangThaiChoXacNhanHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.ChoXacNhanHoanThanh);

            var thongKeCongViecRows = await (
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where duAnIds.Contains(dm.MaDuAn)
                      && dm.IsDeleted != true
                      && cv.IsDeleted != true
                select new
                {
                    dm.MaDuAn,
                    cv.TrangThaiCongViec,
                    cv.NgayKetThucCVDuKien,
                    cv.NgayKetThucCVThucTe
                }).ToListAsync();

            var thongKeCongViec = thongKeCongViecRows
                .GroupBy(x => x.MaDuAn)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        TongCongViec = g.Count(),
                        NgayKetThucThucTeDuAn = g
                            .Where(x => x.NgayKetThucCVThucTe.HasValue)
                            .Select(x => x.NgayKetThucCVThucTe)
                            .OrderByDescending(x => x)
                            .FirstOrDefault(),
                        SoCongViecTre = g.Count(x =>
                            x.NgayKetThucCVDuKien.HasValue
                            && ((x.NgayKetThucCVThucTe.HasValue
                                    && x.NgayKetThucCVThucTe.Value > x.NgayKetThucCVDuKien.Value
                                    && x.TrangThaiCongViec != null
                                    && (trangThaiHoanThanh.Contains(x.TrangThaiCongViec)
                                        || trangThaiChoXacNhanHoanThanh.Contains(x.TrangThaiCongViec)))
                                || (x.NgayKetThucCVDuKien.Value < now
                                    && (x.TrangThaiCongViec == null
                                        || (!trangThaiHoanThanh.Contains(x.TrangThaiCongViec)
                                            && !trangThaiChoXacNhanHoanThanh.Contains(x.TrangThaiCongViec))))))
                    });

            var danhGiaRows = await (
                from dg in _context.DanhGiaDuAn
                join nguoiDanhGia in _context.NguoiDung on dg.MaNguoiDung equals nguoiDanhGia.MaNguoiDung
                join nguoiDuyet in _context.NguoiDung on dg.MaNguoiDungDuyet equals nguoiDuyet.MaNguoiDung into reviewJoin
                from nguoiDuyet in reviewJoin.DefaultIfEmpty()
                where dg.IsDeleted != true
                      && duAnIds.Contains(dg.MaDuAn)
                orderby dg.NgayDanhGiaDA descending, dg.MaDanhGiaDuAn descending
                select new
                {
                    dg.MaDanhGiaDuAn,
                    dg.MaDuAn,
                    dg.MaNguoiDung,
                    dg.DiemTongDanhGiaDA,
                    dg.NhanXetTongDuAn,
                    dg.NgayDanhGiaDA,
                    dg.TrangThaiDanhGiaDA,
                    TenNguoiDanhGia = nguoiDanhGia.HoTenNguoiDung,
                    TenNguoiDuyet = nguoiDuyet.HoTenNguoiDung,
                    NgayDuyet = dg.NgayDuyetDanhGiaDA,
                    LyDoTuChoi = dg.LyDoTuChoiDanhGiaDA
                }).ToListAsync();

            var danhGiaMoiNhatTheoDuAn = danhGiaRows
                .GroupBy(x => x.MaDuAn)
                .ToDictionary(g => g.Key, g => g.First());

            var items = duAnRows.Select(x =>
            {
                thongKeCongViec.TryGetValue(x.MaDuAn, out var tk);
                var duDieuKienDanhGia = ChoPhepGuiDuyetTheoTrangThaiDuAn(x.TrangThaiDuAn);
                var coTheDanhGia = roleFlags.IsManager && x.MaNguoiQuanLy == currentUserId && duDieuKienDanhGia;
                var timelineThongKe = new DanhGiaDuAnThongKeViewModel
                {
                    TrangThaiDuAn = x.TrangThaiDuAn ?? string.Empty,
                    NgayKetThucDuAn = x.NgayKetThucDuAn,
                    NgayKetThucThucTeDuAn = x.NgayHoanThanhThucTeDuAn ?? tk?.NgayKetThucThucTeDuAn,
                    PhanTramHoanThanh = Math.Clamp(x.PhanTramHoanThanh ?? 0, 0, 100)
                };
                BuildTimelineInsightAsync(timelineThongKe);
                var duAnTreTienDo =
                    !timelineThongKe.ChuaCoMocKetThucDuKien
                    && (string.Equals(timelineThongKe.TrangThaiThoiHanDuAn, TrangThaiThoiHanQuaHan, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(timelineThongKe.TrangThaiThoiHanDuAn, TrangThaiThoiHanHoanThanhTreHan, StringComparison.OrdinalIgnoreCase));

                if (!danhGiaMoiNhatTheoDuAn.TryGetValue(x.MaDuAn, out var dg))
                {
                    return new DanhGiaDuAnItemViewModel
                    {
                        CoDanhGia = false,
                        MaDanhGiaDuAn = 0,
                        MaDuAn = x.MaDuAn,
                        TenDuAn = x.TenDuAn ?? $"Du an {x.MaDuAn}",
                        TenNguoiQuanLy = x.TenNguoiQuanLy ?? $"Nguoi dung {x.MaNguoiQuanLy}",
                        TrangThaiDuAn = TrangThai.ToDisplay(x.TrangThaiDuAn),
                        DuDieuKienDanhGia = duDieuKienDanhGia,
                        TrangThaiDanhGia = TrangThaiChuaDanhGia,
                        PhanTramHoanThanh = Math.Clamp(x.PhanTramHoanThanh ?? 0, 0, 100),
                        TongCongViec = tk?.TongCongViec ?? 0,
                        CongViecTreHan = tk?.SoCongViecTre ?? 0,
                        TrangThaiThoiHanDuAn = timelineThongKe.TrangThaiThoiHanDuAn,
                        SoNgayQuaHan = timelineThongKe.SoNgayQuaHan,
                        DuAnTreTienDo = duAnTreTienDo,
                        ChuaDuDuLieuTreTienDo = timelineThongKe.ChuaCoMocKetThucDuKien,
                        DiemTongKet = 0,
                        XepLoai = "-",
                        CoTheDanhGia = coTheDanhGia
                    };
                }

                var trangThaiCode = ChuanHoaTrangThaiDanhGia(dg.TrangThaiDanhGiaDA);
                var diemTong = dg.DiemTongDanhGiaDA ?? 0;
                var coTheSua = CoQuyenSuaDanhGiaDuAn(roleFlags, currentUserId, dg.MaNguoiDung, x.MaNguoiQuanLy, trangThaiCode);
                var coTheGuiDuyet = coTheSua && duDieuKienDanhGia;
                var coTheDuyet = CoQuyenDuyetDanhGiaDuAn(roleFlags, trangThaiCode);

                return new DanhGiaDuAnItemViewModel
                {
                    CoDanhGia = true,
                    MaDanhGiaDuAn = dg.MaDanhGiaDuAn,
                    MaDuAn = x.MaDuAn,
                    TenDuAn = x.TenDuAn ?? $"Du an {x.MaDuAn}",
                    TenNguoiQuanLy = x.TenNguoiQuanLy ?? $"Nguoi dung {x.MaNguoiQuanLy}",
                    TrangThaiDuAn = TrangThai.ToDisplay(x.TrangThaiDuAn),
                    DuDieuKienDanhGia = duDieuKienDanhGia,
                    TrangThaiDanhGia = trangThaiCode,
                    PhanTramHoanThanh = Math.Clamp(x.PhanTramHoanThanh ?? 0, 0, 100),
                    TongCongViec = tk?.TongCongViec ?? 0,
                    CongViecTreHan = tk?.SoCongViecTre ?? 0,
                    TrangThaiThoiHanDuAn = timelineThongKe.TrangThaiThoiHanDuAn,
                    SoNgayQuaHan = timelineThongKe.SoNgayQuaHan,
                    DuAnTreTienDo = duAnTreTienDo,
                    ChuaDuDuLieuTreTienDo = timelineThongKe.ChuaCoMocKetThucDuKien,
                    DiemTongKet = diemTong,
                    XepLoai = TinhXepLoai(diemTong),
                    NhanXet = dg.NhanXetTongDuAn,
                    NgayDanhGia = dg.NgayDanhGiaDA,
                    TenNguoiDanhGia = dg.TenNguoiDanhGia ?? $"Nguoi dung {dg.MaNguoiDung}",
                    TenNguoiDuyet = dg.TenNguoiDuyet,
                    NgayDuyet = dg.NgayDuyet,
                    LyDoTuChoi = dg.LyDoTuChoi,
                    CoTheDanhGia = coTheDanhGia,
                    CoTheSua = coTheSua && duDieuKienDanhGia,
                    CoTheGuiDuyet = coTheGuiDuyet,
                    CoTheDuyet = coTheDuyet,
                    CoTheTuChoi = coTheDuyet
                };
            }).ToList();

            return new DanhGiaDuAnPageViewModel
            {
                TuKhoa = tuKhoa,
                TrangThai = trangThai,
                MaDuAn = maDuAn,
                TuNgayDanhGia = tuNgayLoc,
                DenNgayDanhGia = denNgayLoc,
                DanhSach = items,
                DanhSachDuAn = danhSachDuAnTheoScope,
                TongSo = totalItems,
                SoChuaDanhGia = summaryRows.Count(x => string.IsNullOrWhiteSpace(x.TrangThaiDanhGia)),
                SoNhap = summaryRows.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThaiNhap)),
                SoChoDuyet = summaryRows.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThai.ChoDuyet)),
                SoDaDuyet = summaryRows.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThai.DaDuyet)),
                SoTuChoi = summaryRows.Count(x => LaTrangThaiDanhGia(x.TrangThaiDanhGia, TrangThai.TuChoi)),
                Pagination = pagination
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
            var nguyenNhanRows = await _context.DmNguyenNhan
                .OrderBy(x => x.MaDMNguyenNhan)
                .Select(x => new
                {
                    x.MaDMNguyenNhan,
                    x.TenNguyenNhan
                })
                .ToListAsync();
            var danhSachNguyenNhan = nguyenNhanRows
                .Select(x => new DanhGiaDuAnNguyenNhanOptionViewModel
                {
                    MaDMNguyenNhan = x.MaDMNguyenNhan,
                    TenNguyenNhan = string.IsNullOrWhiteSpace(x.TenNguyenNhan)
                        ? $"Nguyên nhân {x.MaDMNguyenNhan}"
                        : x.TenNguyenNhan!
                })
                .ToList();
            var coDuAnTheoScope = duAn.MaNguoiDung == currentUserId
                                  || await _context.NhanVienDuAn.AnyAsync(x =>
                                      x.MaDuAn == maDuAn && x.MaNguoiDung == currentUserId);
            var trangThaiThucTe = XacDinhTrangThaiThucTeTienDo(thongKe);
            var coThePhanTichAi = coDuAnTheoScope
                                  && roleFlags.IsManager
                                  && CoQuyenTheoClaim(Permissions.DanhGiaDuAn.DanhGia, Permissions.DanhGiaDuAn.Sua);
            var canPhanTichAi = !thongKe.CoDuLieuAi.GetValueOrDefault()
                                || thongKe.KetQuaAiCoTheDaCu;
            var lyDoCanPhanTichAi = !thongKe.CoDuLieuAi.GetValueOrDefault()
                ? "Chưa có kết quả AI mới cho dự án này."
                : thongKe.KetQuaAiCoTheDaCu
                    ? "Kết quả AI hiện tại có thể đã cũ so với dữ liệu mới."
                    : null;
            if (trangThaiThucTe == TrangThaiThucTeTienDo.HoanThanhDungHan)
            {
                coThePhanTichAi = false;
                canPhanTichAi = false;
                lyDoCanPhanTichAi = ThongBaoDuAnDungHanKhongCanPhanTich;
            }
            thongKe.CoThePhanTichAi = coThePhanTichAi;
            thongKe.CanPhanTichAi = canPhanTichAi;
            thongKe.TuDongPhanTichAi = coThePhanTichAi && canPhanTichAi;
            thongKe.LyDoCanPhanTichAi = lyDoCanPhanTichAi;
            var coQuyenXacNhanNguyenNhan = coDuAnTheoScope
                                           && (roleFlags.IsManager || CoQuyenTheoClaim(Permissions.AI.XacNhan))
                                           && CoQuyenTheoClaim(Permissions.DanhGiaDuAn.DanhGia);
            var duAnCanXacNhanNguyenNhan = thongKe.DuAnBiTreTheoAi == true;
            var thongBaoXacNhan = duAnCanXacNhanNguyenNhan
                ? null
                : "Dự án không trễ, không cần xác nhận nguyên nhân.";

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
                MaDmNguyenNhanAiDuDoan = thongKe.MaDmNguyenNhanAiDuDoan,
                MaDmNguyenNhanManagerXacNhan = thongKe.MaDmNguyenNhanManagerXacNhan,
                TenNguyenNhanManagerXacNhan = thongKe.TenNguyenNhanManagerXacNhan,
                DoTinCayManagerXacNhan = thongKe.DoTinCayManagerXacNhan,
                ThoiGianManagerXacNhan = thongKe.ThoiGianManagerXacNhan,
                TrangThaiDuLieuAi = thongKe.TrangThaiDuLieuAi,
                CoTheXacNhanNguyenNhan = coQuyenXacNhanNguyenNhan,
                HienThiKhuXacNhanNguyenNhan = coQuyenXacNhanNguyenNhan && duAnCanXacNhanNguyenNhan,
                ThongBaoXacNhanNguyenNhan = thongBaoXacNhan,
                DanhSachNguyenNhan = danhSachNguyenNhan,
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

        public async Task<DanhGiaDuAnAiInsightViewModel> PhanTichAiDuAnAsync(int maDuAn, CancellationToken cancellationToken = default)
        {
            if (maDuAn <= 0)
            {
                throw new Exception("Dự án không hợp lệ.");
            }

            var predictResult = await _aiService.PhanTichNguyenNhanDuAnAsync(maDuAn, cancellationToken);
            if (!predictResult.ThanhCong)
            {
                var chiTiet = predictResult.Loi.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                var thongBao = !string.IsNullOrWhiteSpace(predictResult.ThongBao)
                    ? predictResult.ThongBao
                    : "Không thể gọi dịch vụ AI.";
                _logger.LogWarning(
                    "Phan tich AI that bai cho du an {MaDuAn}. ThongBao: {ThongBao}. ChiTiet: {ChiTiet}",
                    maDuAn,
                    thongBao,
                    string.Join(" | ", chiTiet));
                throw new Exception(chiTiet.Count > 0 ? $"{thongBao} {string.Join(" | ", chiTiet)}" : thongBao);
            }
            var duLieuPhanTich = predictResult.DuLieu;

            var thongKe = await XayDungThongKeDuAnAsync(maDuAn, cancellationToken);
            if (duLieuPhanTich != null)
            {
                thongKe.CoDuLieuAi = true;
                thongKe.DuAnBiTreTheoAi = true;
                thongKe.MaDmNguyenNhanAiDuDoan = duLieuPhanTich.MaDMNguyenNhanDuDoan;
                thongKe.TenNguyenNhanAiDuDoan = duLieuPhanTich.TenNguyenNhanDuDoan;
                thongKe.DoTinCayAi = duLieuPhanTich.DoTinCayKetQua;
                thongKe.ThoiGianDuDoanAi = duLieuPhanTich.ThoiGianPhanTich ?? DateTime.Now;
                thongKe.NguonNguyenNhanAi = duLieuPhanTich.ReasonSource == "NguyenNhanModel" ? "Mô hình" : "Quy tắc";
                thongKe.TenModelNguyenNhanAi = string.IsNullOrWhiteSpace(duLieuPhanTich.ModelNguyenNhanUsed)
                    ? "Quy tắc"
                    : duLieuPhanTich.ModelNguyenNhanUsed;
                thongKe.TrangThaiDuLieuAi = "Đã phân tích.";
                thongKe.MucPhuHopAi = string.IsNullOrWhiteSpace(duLieuPhanTich.MucPhuHop)
                    ? XacDinhMucPhuHopTuDoTinCay(thongKe.DoTinCayAi)
                    : duLieuPhanTich.MucPhuHop.Trim();
                thongKe.DanhSachNguyenNhanLienQuan = ChuanHoaDanhSachNguyenNhanLienQuan(
                    duLieuPhanTich.DanhSachNguyenNhanLienQuan?
                        .Select(x => new DanhGiaDuAnRelatedReasonViewModel
                        {
                            MaDMNguyenNhan = x.MaDMNguyenNhan,
                            TenNguyenNhan = x.TenNguyenNhan,
                            Score = x.Score,
                            MucPhuHop = x.MucPhuHop
                        })
                        .ToList(),
                    thongKe.MaDmNguyenNhanAiDuDoan,
                    thongKe.TenNguyenNhanAiDuDoan);
            }
            thongKe.CoThePhanTichAi = true;
            thongKe.CanPhanTichAi = false;
            thongKe.TuDongPhanTichAi = false;
            thongKe.LyDoCanPhanTichAi = null;
            return BuildAiInsightViewModelAsync(thongKe);
        }

        public async Task XacNhanNguyenNhanAsync(int maDuAn, int maDmNguyenNhan, double? doTinCay)
        {
            if (maDuAn <= 0)
            {
                throw new Exception("Dự án không hợp lệ.");
            }

            var result = await _aiService.XacNhanNguyenNhanAsync(
                maDuAn,
                maDmNguyenNhan.ToString(System.Globalization.CultureInfo.InvariantCulture),
                doTinCay);
            if (!result.ThanhCong)
            {
                throw new Exception(string.IsNullOrWhiteSpace(result.ThongBao)
                    ? "Bạn không có quyền thực hiện."
                    : result.ThongBao);
            }
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
            if (!roleFlags.IsManager)
            {
                return new List<DanhGiaDuAnDuAnOptionViewModel>();
            }

            IQueryable<DuAn> query = _context.DuAn.Where(x =>
                x.IsDeleted != true
                && x.MaNguoiDung == currentUserId);

            var rows = await query
                .OrderBy(x => x.TenDuAn)
                .Select(x => new
                {
                    x.MaDuAn,
                    x.TenDuAn
                })
                .ToListAsync();
            return rows.Select(x => new DanhGiaDuAnDuAnOptionViewModel
            {
                MaDuAn = x.MaDuAn,
                TenDuAn = string.IsNullOrWhiteSpace(x.TenDuAn) ? $"Du an {x.MaDuAn}" : x.TenDuAn
            }).ToList();
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

        private async Task<DanhGiaDuAnThongKeViewModel> XayDungThongKeDuAnAsync(int maDuAn, CancellationToken cancellationToken = default)
        {
            var thongKe = await LoadThongTinHoTroDanhGiaAsync(maDuAn, cancellationToken);
            BuildTimelineInsightAsync(thongKe);
            var trangThaiThucTe = XacDinhTrangThaiThucTeTienDo(thongKe);
            await LoadKetQuaAiMoiNhatAsync(thongKe, maDuAn, trangThaiThucTe, cancellationToken);
            return thongKe;
        }

        private async Task<DanhGiaDuAnThongKeViewModel> LoadThongTinHoTroDanhGiaAsync(int maDuAn, CancellationToken cancellationToken = default)
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
                    da.NgayHoanThanhThucTeDuAn,
                    da.PhanTramHoanThanh
                }).FirstOrDefaultAsync(cancellationToken);

            if (duAn == null)
            {
                var thongKeRong = new DanhGiaDuAnThongKeViewModel();
                GanDuLieuAiMacDinh(thongKeRong);
                BuildTimelineInsightAsync(thongKeRong);
                return thongKeRong;
            }

            var now = DateTime.Now;
            var trangThaiHoanThanhCongViec = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
            var trangThaiChoXacNhanHoanThanhCongViec = TrangThai.GetCommonStatusVariants(TrangThai.ChoXacNhanHoanThanh);
            var trangThaiBiCanTro = TrangThai.GetCommonStatusVariants(TrangThai.BiCanCan);
            var trangThaiChoDuyet = TrangThai.GetCommonStatusVariants(TrangThai.ChoDuyet);
            var trangThaiDaDuyet = TrangThai.GetCommonStatusVariants(TrangThai.DaDuyet);
            var trangThaiTuChoi = TrangThai.GetCommonStatusVariants(TrangThai.TuChoi);
            var trangThaiYeuCauBoSung = TrangThai.GetCommonStatusVariants(TrangThai.YeuCauBoSung);

            var congViecRows = await (
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where dm.MaDuAn == maDuAn
                      && dm.IsDeleted != true
                      && cv.IsDeleted != true
                select new
                {
                    cv.MaCongViec,
                    cv.TrangThaiCongViec,
                    cv.NgayKetThucCVDuKien,
                    cv.NgayKetThucCVThucTe
                }).ToListAsync(cancellationToken);

            var soCongViec = congViecRows.Count;
            var soCongViecHoanThanh = congViecRows.Count(x =>
                x.TrangThaiCongViec != null
                && trangThaiHoanThanhCongViec.Contains(x.TrangThaiCongViec));
            var soCongViecChuaHoanThanh = Math.Max(0, soCongViec - soCongViecHoanThanh);
            var soCongViecDangTreHan = congViecRows.Count(x =>
                x.NgayKetThucCVDuKien.HasValue
                && x.NgayKetThucCVDuKien.Value < now
                && (x.TrangThaiCongViec == null
                    || (!trangThaiHoanThanhCongViec.Contains(x.TrangThaiCongViec)
                        && !trangThaiChoXacNhanHoanThanhCongViec.Contains(x.TrangThaiCongViec))));
            var soCongViecHoanThanhTreHan = congViecRows.Count(x =>
                x.NgayKetThucCVDuKien.HasValue
                && x.NgayKetThucCVThucTe.HasValue
                && x.NgayKetThucCVThucTe.Value > x.NgayKetThucCVDuKien.Value
                && x.TrangThaiCongViec != null
                && (trangThaiHoanThanhCongViec.Contains(x.TrangThaiCongViec)
                    || trangThaiChoXacNhanHoanThanhCongViec.Contains(x.TrangThaiCongViec)));
            var tongCongViecTreHan = soCongViecDangTreHan + soCongViecHoanThanhTreHan;
            var tyLeCongViecTreHan = TinhTyLe(tongCongViecTreHan, soCongViec);
            var ngayKetThucThucTeFallback = congViecRows
                .Where(x => x.NgayKetThucCVThucTe.HasValue)
                .Select(x => x.NgayKetThucCVThucTe)
                .OrderByDescending(x => x)
                .FirstOrDefault();
            var ngayKetThucThucTeDuAn = duAn.NgayHoanThanhThucTeDuAn ?? ngayKetThucThucTeFallback;

            var chiTietRows = await (
                from ct in _context.CtCongViec
                join cv in _context.CongViec on ct.MaCongViec equals cv.MaCongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where dm.MaDuAn == maDuAn
                      && dm.IsDeleted != true
                      && cv.IsDeleted != true
                      && ct.IsDeleted != true
                select new
                {
                    ct.MaChiTietCV,
                    ct.TrangThaiCTCV,
                    ct.NgayKetThucCTCV
                }).ToListAsync(cancellationToken);

            var soChiTiet = chiTietRows.Count;
            var maChiTietIds = chiTietRows.Select(x => x.MaChiTietCV).Distinct().ToList();
            var soChiTietHoanThanh = chiTietRows.Count(x =>
                x.TrangThaiCTCV != null
                && trangThaiHoanThanhCongViec.Contains(x.TrangThaiCTCV));
            var soChiTietChuaHoanThanh = Math.Max(0, soChiTiet - soChiTietHoanThanh);
            var soChiTietBiCanTro = chiTietRows.Count(x =>
                x.TrangThaiCTCV != null
                && trangThaiBiCanTro.Contains(x.TrangThaiCTCV));
            var soChiTietTreHan = 0;
            var tyLeChiTietHoanThanh = TinhTyLe(soChiTietHoanThanh, soChiTiet);

            var baoCaoTienDoRows = await _context.TienDoCongViec
                .Where(x => maChiTietIds.Contains(x.MaChiTietCV))
                .Select(x => new
                {
                    x.TrangThaiTienDo,
                    x.ThoiGianCapNhat,
                    x.MaTienDo
                })
                .ToListAsync(cancellationToken);
            var soBaoCaoTienDo = baoCaoTienDoRows.Count;
            var soBaoCaoTienDoChoDuyet = baoCaoTienDoRows.Count(x => trangThaiChoDuyet.Contains(x.TrangThaiTienDo ?? string.Empty));
            var soBaoCaoTienDoDaDuyet = baoCaoTienDoRows.Count(x => trangThaiDaDuyet.Contains(x.TrangThaiTienDo ?? string.Empty));
            var soBaoCaoTienDoBiTuChoi = baoCaoTienDoRows.Count(x => trangThaiTuChoi.Contains(x.TrangThaiTienDo ?? string.Empty));
            var soBaoCaoTienDoYeuCauBoSung = baoCaoTienDoRows.Count(x => trangThaiYeuCauBoSung.Contains(x.TrangThaiTienDo ?? string.Empty));
            var tyLeBaoCaoTienDoBiTuChoi = TinhTyLe(soBaoCaoTienDoBiTuChoi, soBaoCaoTienDo);
            var lanBaoCaoMoiNhat = baoCaoTienDoRows
                .Where(x => x.ThoiGianCapNhat.HasValue)
                .OrderByDescending(x => x.ThoiGianCapNhat)
                .ThenByDescending(x => x.MaTienDo)
                .Select(x => x.ThoiGianCapNhat)
                .FirstOrDefault();
            var soBaoCaoMoiNhat = 0;
            if (lanBaoCaoMoiNhat.HasValue)
            {
                soBaoCaoMoiNhat = baoCaoTienDoRows.Count(x =>
                    x.ThoiGianCapNhat.HasValue
                    && x.ThoiGianCapNhat.Value == lanBaoCaoMoiNhat.Value);
            }
            var mocChamCapNhatTienDo = duAn.NgayKetThucDuAn ?? now;
            var soNgayChamCapNhatTienDo = lanBaoCaoMoiNhat.HasValue
                ? TinhSoNgayTre(lanBaoCaoMoiNhat.Value, mocChamCapNhatTienDo)
                : (int?)null;

            var tyLe = soCongViec <= 0 ? 0d : Math.Round((double)soCongViecHoanThanh / soCongViec * 100d, 2);

            var tongNganSachDaDuyet = await _context.NganSach
                .Where(x =>
                    x.MaDuAn == maDuAn
                    && x.IsDeleted != true
                    && x.TrangThaiNganSach != null
                    && trangThaiDaDuyet.Contains(x.TrangThaiNganSach))
                .SumAsync(x => x.SoTienNganSach ?? 0m, cancellationToken);

            var tongChiPhiDaDung = await (
                from cp in _context.ChiPhi
                join ns in _context.NganSach on cp.MaNganSach equals ns.MaNganSach
                where ns.MaDuAn == maDuAn
                      && cp.IsDeleted != true
                      && ns.IsDeleted != true
                select cp.SoTienDaChi ?? 0m
            ).SumAsync(cancellationToken);
            var tyLeSuDungNganSach = tongNganSachDaDuyet <= 0m
                ? 0d
                : Math.Round((double)(tongChiPhiDaDung / tongNganSachDaDuyet) * 100d, 2);
            var nganSachConLai = tongNganSachDaDuyet > tongChiPhiDaDung
                ? tongNganSachDaDuyet - tongChiPhiDaDung
                : 0m;
            var soTienVuotNganSach = tongChiPhiDaDung > tongNganSachDaDuyet
                ? tongChiPhiDaDung - tongNganSachDaDuyet
                : 0m;
            var tyLeVuotNganSach = tongNganSachDaDuyet <= 0m || soTienVuotNganSach <= 0m
                ? 0d
                : Math.Round((double)(soTienVuotNganSach / tongNganSachDaDuyet) * 100d, 2);
            var trangThaiNganSach = tongNganSachDaDuyet <= 0m
                ? "Chưa có ngân sách"
                : soTienVuotNganSach > 0m
                    ? "Vượt ngân sách"
                    : "Trong ngân sách";

            var deXuatCongViecRows = await _context.DeXuatCongViec
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true)
                .Select(x => new
                {
                    x.TrangThaiCongViecDeXuat,
                    x.NgayDeXuatCongViec,
                    x.NgayDuyetDeXuatCongViec
                })
                .ToListAsync(cancellationToken);
            var soDeXuatCongViecChoDuyet = deXuatCongViecRows.Count(x => trangThaiChoDuyet.Contains(x.TrangThaiCongViecDeXuat ?? string.Empty));
            var soDeXuatCongViecBiTuChoi = deXuatCongViecRows.Count(x => trangThaiTuChoi.Contains(x.TrangThaiCongViecDeXuat ?? string.Empty));
            var thoiGianDuyetCongViecTrungBinh = deXuatCongViecRows
                .Where(x => x.NgayDeXuatCongViec.HasValue && x.NgayDuyetDeXuatCongViec.HasValue)
                .Select(x => Math.Max(0d, (x.NgayDuyetDeXuatCongViec!.Value - x.NgayDeXuatCongViec!.Value).TotalDays))
                .DefaultIfEmpty(0d)
                .Average();
            thoiGianDuyetCongViecTrungBinh = Math.Round(thoiGianDuyetCongViecTrungBinh, 2);

            var deXuatNganSachRows = await _context.DeXuatNganSach
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true)
                .Select(x => new
                {
                    x.TrangThaiDeXuat,
                    x.NgayDeXuat,
                    x.NgayDuyet
                })
                .ToListAsync(cancellationToken);
            var soDeXuatNganSachChoDuyet = deXuatNganSachRows.Count(x => trangThaiChoDuyet.Contains(x.TrangThaiDeXuat ?? string.Empty));
            var soDeXuatNganSachBiTuChoi = deXuatNganSachRows.Count(x => trangThaiTuChoi.Contains(x.TrangThaiDeXuat ?? string.Empty));
            var thoiGianDuyetNganSachTrungBinh = deXuatNganSachRows
                .Where(x => x.NgayDeXuat.HasValue && x.NgayDuyet.HasValue)
                .Select(x => Math.Max(0d, (x.NgayDuyet!.Value - x.NgayDeXuat!.Value).TotalDays))
                .DefaultIfEmpty(0d)
                .Average();
            thoiGianDuyetNganSachTrungBinh = Math.Round(thoiGianDuyetNganSachTrungBinh, 2);

            var soNhanVienThamGia = await _context.NhanVienDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.MaNguoiDung)
                .Distinct()
                .CountAsync(cancellationToken);
            var nhatKyPhuTrachRows = await _context.NhatKyPhuTrachDuAn
                .Where(x => x.MaDuAn == maDuAn)
                .Select(x => x.NkHanhDongPTDA)
                .ToListAsync(cancellationToken);
            var soLanThayDoiNhanSu = nhatKyPhuTrachRows.Count(LaHanhDongThayDoiNhanSu);
            var soLanThayDoiQuanLy = await _context.YeuCauDoiQuanLy
                .CountAsync(x =>
                    x.MaDuAn == maDuAn
                    && x.IsDeleted != true
                    && x.NgayDuyetYeuCauDoiQuanLy.HasValue
                    && x.MaQuanLyHienTai != x.MaQuanLyDeXuat
                    && x.TrangThaiYeuCauDoiQuanLy != null
                    && trangThaiDaDuyet.Contains(x.TrangThaiYeuCauDoiQuanLy), cancellationToken);

            var soFileDuAn = await _context.FileDuAn.CountAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true, cancellationToken);

            var thongKe = new DanhGiaDuAnThongKeViewModel
            {
                TenDuAn = duAn.TenDuAn ?? $"Du an {duAn.MaDuAn}",
                TenNguoiQuanLy = duAn.TenNguoiQuanLy ?? $"Nguoi dung {duAn.MaDuAn}",
                TrangThaiDuAn = TrangThai.ToDisplay(duAn.TrangThaiDuAn),
                NgayBatDauDuAn = duAn.NgayBatDauDuAn,
                NgayKetThucDuAn = duAn.NgayKetThucDuAn,
                NgayKetThucThucTeDuAn = ngayKetThucThucTeDuAn,
                PhanTramHoanThanh = Math.Clamp(duAn.PhanTramHoanThanh ?? 0, 0, 100),
                TongCongViec = soCongViec,
                CongViecHoanThanh = soCongViecHoanThanh,
                CongViecTreHan = tongCongViecTreHan,
                CongViecChuaHoanThanh = soCongViecChuaHoanThanh,
                CongViecDangTreHan = soCongViecDangTreHan,
                CongViecHoanThanhTreHan = soCongViecHoanThanhTreHan,
                TyLeCongViecTreHan = tyLeCongViecTreHan,
                TongChiTietCongViec = soChiTiet,
                ChiTietHoanThanh = soChiTietHoanThanh,
                ChiTietChuaHoanThanh = soChiTietChuaHoanThanh,
                ChiTietBiCanTro = soChiTietBiCanTro,
                ChiTietTreHan = soChiTietTreHan,
                TyLeChiTietHoanThanh = tyLeChiTietHoanThanh,
                TyLeHoanThanh = tyLe,
                SoBaoCaoTienDo = soBaoCaoTienDo,
                SoBaoCaoMoiNhat = soBaoCaoMoiNhat,
                SoBaoCaoTienDoChoDuyet = soBaoCaoTienDoChoDuyet,
                SoBaoCaoTienDoDaDuyet = soBaoCaoTienDoDaDuyet,
                SoBaoCaoTienDoBiTuChoi = soBaoCaoTienDoBiTuChoi,
                SoBaoCaoTienDoYeuCauBoSung = soBaoCaoTienDoYeuCauBoSung,
                TyLeBaoCaoTienDoBiTuChoi = tyLeBaoCaoTienDoBiTuChoi,
                LanCapNhatTienDoGanNhat = lanBaoCaoMoiNhat,
                SoNgayChamCapNhatTienDo = soNgayChamCapNhatTienDo,
                TongNganSach = tongNganSachDaDuyet,
                TongChiPhi = tongChiPhiDaDung,
                TyLeSuDungNganSach = tyLeSuDungNganSach,
                NganSachConLai = nganSachConLai,
                SoTienVuotNganSach = soTienVuotNganSach,
                TyLeVuotNganSach = tyLeVuotNganSach,
                TrangThaiNganSach = trangThaiNganSach,
                SoDeXuatCongViecChoDuyet = soDeXuatCongViecChoDuyet,
                SoDeXuatCongViecBiTuChoi = soDeXuatCongViecBiTuChoi,
                ThoiGianDuyetCongViecTrungBinh = thoiGianDuyetCongViecTrungBinh,
                SoDeXuatNganSachChoDuyet = soDeXuatNganSachChoDuyet,
                SoDeXuatNganSachBiTuChoi = soDeXuatNganSachBiTuChoi,
                ThoiGianDuyetNganSachTrungBinh = thoiGianDuyetNganSachTrungBinh,
                SoNhanVienThamGia = soNhanVienThamGia,
                SoLanThayDoiNhanSu = soLanThayDoiNhanSu,
                SoLanThayDoiQuanLy = soLanThayDoiQuanLy,
                SoFileDuAn = soFileDuAn
            };
            return thongKe;
        }

        private async Task LoadKetQuaAiMoiNhatAsync(
            DanhGiaDuAnThongKeViewModel thongKe,
            int maDuAn,
            TrangThaiThucTeTienDo trangThaiThucTe,
            CancellationToken cancellationToken = default)
        {
            GanDuLieuAiMacDinh(thongKe);

            var datasetMoiNhat = await _context.AiDataset
                .Where(x => x.MaDuAn == maDuAn)
                .OrderByDescending(x => x.NgayTongHop ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaData)
                .FirstOrDefaultAsync(cancellationToken);

            var duLieuAi = await (
                from kq in _context.AiKetQua
                join dm in _context.DmNguyenNhan on kq.MaDMNguyenNhan equals dm.MaDMNguyenNhan into dmJoin
                from dm in dmJoin.DefaultIfEmpty()
                join model in _context.AiModel on kq.MaModel equals model.MaModel into modelJoin
                from model in modelJoin.DefaultIfEmpty()
                where kq.MaDuAn == maDuAn
                orderby kq.ThoiGianDuDoanKetQua descending, kq.MaAiKetQua descending
                select new
                {
                    kq.MaData,
                    kq.MaDMNguyenNhan,
                    kq.DoTinCayKetQua,
                    kq.ThoiGianDuDoanKetQua,
                    kq.NoiDungPhanTich,
                    TenNguyenNhan = dm != null ? dm.TenNguyenNhan : null,
                    TenModel = model != null ? model.TenModel : null,
                    LoaiModel = model != null ? model.LoaiModel : null
                }).FirstOrDefaultAsync(cancellationToken);

            var modelNguyenNhanHoatDong = await _context.AiModel
                .Where(x => x.IsDeleted != true
                            && x.IsActive == true
                            && x.LoaiModel == "NguyenNhan")
                .OrderByDescending(x => x.NgayTao ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaModel)
                .Select(x => x.TenModel)
                .FirstOrDefaultAsync(cancellationToken);

            if (trangThaiThucTe == TrangThaiThucTeTienDo.HoanThanhDungHan)
            {
                thongKe.CoDuLieuAi = false;
                thongKe.DuAnBiTreTheoAi = false;
                thongKe.TenNguyenNhanAiDuDoan = NguyenNhanKhongTre;
                thongKe.DoTinCayAi = null;
                thongKe.MucPhuHopAi = null;
                thongKe.DanhSachNguyenNhanLienQuan = null;
                thongKe.ThoiGianDuDoanAi = null;
                thongKe.TrangThaiDuLieuAi = ThongBaoDuAnDungHanKhongCanPhanTich;
                thongKe.CanPhanTichAi = false;
                thongKe.LyDoCanPhanTichAi = ThongBaoDuAnDungHanKhongCanPhanTich;

                if (duLieuAi != null)
                {
                    var tenNguyenNhanCu = ChuanHoaTenNguyenNhanHienThi(duLieuAi.TenNguyenNhan);
                    if (!LaNguyenNhanKhongTre(tenNguyenNhanCu))
                    {
                        thongKe.KetQuaAiCoTheDaCu = true;
                        thongKe.CanhBaoDuLieuAi = CanhBaoKetQuaAiMauThuanThucTe;
                    }
                }

                return;
            }

            if (duLieuAi != null)
            {
                var duLieuNoiDung = ParseStoredNoiDungPhanTich(duLieuAi.NoiDungPhanTich);
                thongKe.CoDuLieuAi = true;
                thongKe.DoTinCayAi = ChuanHoaDoTinCay(duLieuAi.DoTinCayKetQua);
                thongKe.MucPhuHopAi = XacDinhMucPhuHop(duLieuNoiDung.MucPhuHop, thongKe.DoTinCayAi);
                thongKe.DanhSachNguyenNhanLienQuan = null;
                thongKe.ThoiGianDuDoanAi = duLieuAi.ThoiGianDuDoanKetQua;
                thongKe.MaDmNguyenNhanAiDuDoan = duLieuAi.MaDMNguyenNhan;
                thongKe.TrangThaiDuLieuAi = "Đã có kết quả phân tích nguyên nhân trễ cho dự án này.";

                var suDungModelNguyenNhan = string.Equals(duLieuAi.LoaiModel, "NguyenNhan", StringComparison.OrdinalIgnoreCase);
                thongKe.TenModelTreHanAi = null;
                thongKe.TenModelNguyenNhanAi = suDungModelNguyenNhan
                    ? duLieuAi.TenModel
                    : modelNguyenNhanHoatDong;
                thongKe.NguonNguyenNhanAi = suDungModelNguyenNhan ? "NguyenNhanModel" : "RuleFallback";

                var duLieuAiDaCu = duLieuAi.MaData != datasetMoiNhat?.MaData;
                if (!duLieuAiDaCu)
                {
                    if (suDungModelNguyenNhan)
                    {
                        duLieuAiDaCu = !string.IsNullOrWhiteSpace(modelNguyenNhanHoatDong)
                                       && !string.Equals(duLieuAi.TenModel, modelNguyenNhanHoatDong, StringComparison.OrdinalIgnoreCase);
                    }
                }

                thongKe.KetQuaAiCoTheDaCu = duLieuAiDaCu;
                thongKe.CanPhanTichAi = duLieuAiDaCu;
                if (duLieuAiDaCu)
                {
                    thongKe.CanhBaoDuLieuAi = CanhBaoKetQuaAiCoTheDaCu;
                }

                var tenNguyenNhanTuAi = ChuanHoaTenNguyenNhanHienThi(duLieuAi.TenNguyenNhan);
                if (LaNguyenNhanKhongTre(tenNguyenNhanTuAi))
                {
                    thongKe.DuAnBiTreTheoAi = false;
                    thongKe.TenNguyenNhanAiDuDoan = NguyenNhanKhongTre;
                }
                else
                {
                    thongKe.DuAnBiTreTheoAi = true;
                    if (string.IsNullOrWhiteSpace(tenNguyenNhanTuAi)
                        || LaNguyenNhanPheDuyetMacDinh(tenNguyenNhanTuAi))
                    {
                        thongKe.TenNguyenNhanAiDuDoan = datasetMoiNhat?.LaDuAnTre == true
                            ? GoiYNguyenNhanTre(datasetMoiNhat)
                            : NguyenNhanChuaDuDuLieu;
                    }
                    else
                    {
                        thongKe.TenNguyenNhanAiDuDoan = tenNguyenNhanTuAi;
                    }

                    thongKe.TenNguyenNhanAiDuDoan = DieuChinhNguyenNhanTheoDuLieuThucTe(
                        thongKe.TenNguyenNhanAiDuDoan,
                        thongKe,
                        datasetMoiNhat);
                }

                thongKe.DanhSachNguyenNhanLienQuan = thongKe.DuAnBiTreTheoAi == true
                    ? ChuanHoaDanhSachNguyenNhanLienQuan(
                        duLieuNoiDung.DanhSachNguyenNhanLienQuan,
                        thongKe.MaDmNguyenNhanAiDuDoan,
                        thongKe.TenNguyenNhanAiDuDoan)
                    : null;
            }
            else
            {
                thongKe.CanPhanTichAi = true;
                if (datasetMoiNhat != null && !datasetMoiNhat.LaDuAnTre.HasValue)
                {
                    thongKe.TrangThaiDuLieuAi = TrangThaiDuLieuAiChuaDuLabel;
                }
                else if (datasetMoiNhat?.LaDuAnTre == false)
                {
                    thongKe.DuAnBiTreTheoAi = false;
                    thongKe.TenNguyenNhanAiDuDoan = NguyenNhanKhongTre;
                }
                else if (datasetMoiNhat?.LaDuAnTre == true)
                {
                    thongKe.DuAnBiTreTheoAi = true;
                }
            }

            ApDungRangBuocTheoTrangThaiThucTe(thongKe, trangThaiThucTe, datasetMoiNhat);

            var duLieuXacNhan = await (
                from nn in _context.AiNguyenNhan
                join dm in _context.DmNguyenNhan on nn.MaDMNguyenNhan equals dm.MaDMNguyenNhan into dmJoin
                from dm in dmJoin.DefaultIfEmpty()
                where nn.MaDuAn == maDuAn
                      && nn.IsDeleted != true
                orderby nn.MaAINguyenNhan descending
                select new
                {
                    nn.MaDMNguyenNhan,
                    nn.DoTinCay,
                    TenNguyenNhan = dm != null ? dm.TenNguyenNhan : null
                }).FirstOrDefaultAsync(cancellationToken);

            if (duLieuXacNhan == null)
            {
                return;
            }

            thongKe.TenNguyenNhanManagerXacNhan = string.IsNullOrWhiteSpace(duLieuXacNhan.TenNguyenNhan)
                ? null
                : ChuanHoaTenNguyenNhanHienThi(duLieuXacNhan.TenNguyenNhan);
            thongKe.MaDmNguyenNhanManagerXacNhan = duLieuXacNhan.MaDMNguyenNhan;
            thongKe.DoTinCayManagerXacNhan = ChuanHoaDoTinCay(duLieuXacNhan.DoTinCay);
        }

        private static void BuildTimelineInsightAsync(DanhGiaDuAnThongKeViewModel thongKe)
        {
            thongKe.SoNgayConLai = null;
            thongKe.SoNgayQuaHan = null;

            if (!thongKe.NgayKetThucDuAn.HasValue)
            {
                thongKe.ChuaCoMocKetThucDuKien = true;
                thongKe.TrangThaiThoiHanDuAn = TrangThaiThoiHanChuaCoMocKetThuc;
                return;
            }

            thongKe.ChuaCoMocKetThucDuKien = false;
            var now = DateTime.Now;
            var ngayDuKien = thongKe.NgayKetThucDuAn.Value;
            var laDuAnHoanThanh = TrangThai.EqualsValue(thongKe.TrangThaiDuAn, TrangThai.HoanThanh)
                                  || TrangThai.EqualsValue(thongKe.TrangThaiDuAn, TrangThai.ChoXacNhanHoanThanh)
                                  || thongKe.PhanTramHoanThanh >= 100;

            if (laDuAnHoanThanh && thongKe.NgayKetThucThucTeDuAn.HasValue)
            {
                var soNgayTre = TinhSoNgayTre(ngayDuKien, thongKe.NgayKetThucThucTeDuAn.Value);
                if (soNgayTre > 0)
                {
                    thongKe.SoNgayQuaHan = soNgayTre;
                    thongKe.TrangThaiThoiHanDuAn = TrangThaiThoiHanHoanThanhTreHan;
                }
                else
                {
                    thongKe.SoNgayConLai = Math.Max(0, (int)Math.Floor((ngayDuKien - thongKe.NgayKetThucThucTeDuAn.Value).TotalDays));
                    thongKe.TrangThaiThoiHanDuAn = TrangThaiThoiHanHoanThanhDungHan;
                }

                return;
            }

            if (now > ngayDuKien)
            {
                thongKe.SoNgayQuaHan = TinhSoNgayTre(ngayDuKien, now);
                thongKe.TrangThaiThoiHanDuAn = TrangThaiThoiHanQuaHan;
                return;
            }

            var soNgayConLai = Math.Max(0, (int)Math.Floor((ngayDuKien - now).TotalDays));
            thongKe.SoNgayConLai = soNgayConLai;
            thongKe.TrangThaiThoiHanDuAn = soNgayConLai <= SoNgayCanhBaoSapDenHan
                ? TrangThaiThoiHanSapDenHan
                : TrangThaiThoiHanChuaDenHan;
        }

        private static int TinhSoNgayTre(DateTime han, DateTime thucTe)
        {
            var soNgay = (thucTe - han).TotalDays;
            return soNgay > 0 ? Math.Max(1, (int)Math.Ceiling(soNgay)) : 0;
        }

        private static TrangThaiThucTeTienDo XacDinhTrangThaiThucTeTienDo(DanhGiaDuAnThongKeViewModel thongKe)
        {
            if (thongKe.ChuaCoMocKetThucDuKien)
            {
                return TrangThaiThucTeTienDo.ChuaDuDuLieuThoiHan;
            }

            if (string.Equals(thongKe.TrangThaiThoiHanDuAn, TrangThaiThoiHanHoanThanhDungHan, StringComparison.OrdinalIgnoreCase))
            {
                return TrangThaiThucTeTienDo.HoanThanhDungHan;
            }

            if (string.Equals(thongKe.TrangThaiThoiHanDuAn, TrangThaiThoiHanHoanThanhTreHan, StringComparison.OrdinalIgnoreCase))
            {
                return TrangThaiThucTeTienDo.HoanThanhTreHan;
            }

            if (string.Equals(thongKe.TrangThaiThoiHanDuAn, TrangThaiThoiHanQuaHan, StringComparison.OrdinalIgnoreCase))
            {
                return TrangThaiThucTeTienDo.DangThucHienQuaHan;
            }

            return TrangThaiThucTeTienDo.DangThucHienChuaQuaHan;
        }

        private static string DieuChinhNguyenNhanTheoDuLieuThucTe(
            string? nguyenNhanAi,
            DanhGiaDuAnThongKeViewModel thongKe,
            AiDataset? datasetMoiNhat)
        {
            if (string.IsNullOrWhiteSpace(nguyenNhanAi))
            {
                return NguyenNhanChuaDuDuLieu;
            }

            var daChuanHoa = ChuanHoaTenNguyenNhanHienThi(nguyenNhanAi);
            var soCongViecTre = datasetMoiNhat?.SoCongViecTre ?? thongKe.CongViecTreHan;
            var tyLeCongViecTre = datasetMoiNhat?.TyLeCongViecTre ?? 0d;
            var chiPhiDuKien = datasetMoiNhat?.ChiPhiDuKien ?? thongKe.TongNganSach;
            var chiPhiThucTe = datasetMoiNhat?.ChiPhiThucTe ?? thongKe.TongChiPhi;
            var chenhLechChiPhi = (datasetMoiNhat?.ChenhLechChiPhi)
                                  ?? (chiPhiThucTe - chiPhiDuKien);

            if (string.Equals(daChuanHoa, "Nhiều công việc trễ hạn", StringComparison.OrdinalIgnoreCase)
                && soCongViecTre <= 0
                && tyLeCongViecTre <= 0d
                && thongKe.CongViecTreHan <= 0
                && thongKe.ChiTietTreHan <= 0)
            {
                return GoiYNguyenNhanTre(datasetMoiNhat ?? new AiDataset
                {
                    LaDuAnTre = true,
                    SoCongViecTre = soCongViecTre,
                    TyLeCongViecTre = tyLeCongViecTre,
                    ChiPhiDuKien = chiPhiDuKien,
                    ChiPhiThucTe = chiPhiThucTe,
                    ChenhLechChiPhi = chenhLechChiPhi,
                    SoLanThayDoiNhanSu = datasetMoiNhat?.SoLanThayDoiNhanSu ?? 0,
                    SoLanThayDoiQuanLy = datasetMoiNhat?.SoLanThayDoiQuanLy ?? 0,
                    SoNgayTreTienDo = datasetMoiNhat?.SoNgayTreTienDo ?? 0
                });
            }

            if (LaNguyenNhanVuotNganSach(daChuanHoa)
                && !LaVuotNganSachHopLe(chiPhiDuKien, chiPhiThucTe, chenhLechChiPhi))
            {
                return GoiYNguyenNhanTre(datasetMoiNhat ?? new AiDataset
                {
                    LaDuAnTre = true,
                    SoCongViecTre = soCongViecTre,
                    TyLeCongViecTre = tyLeCongViecTre,
                    ChiPhiDuKien = chiPhiDuKien,
                    ChiPhiThucTe = chiPhiThucTe,
                    ChenhLechChiPhi = chenhLechChiPhi,
                    SoLanThayDoiNhanSu = datasetMoiNhat?.SoLanThayDoiNhanSu ?? 0,
                    SoLanThayDoiQuanLy = datasetMoiNhat?.SoLanThayDoiQuanLy ?? 0,
                    SoNgayTreTienDo = datasetMoiNhat?.SoNgayTreTienDo ?? 0
                });
            }

            return daChuanHoa;
        }

        private static void ApDungRangBuocTheoTrangThaiThucTe(
            DanhGiaDuAnThongKeViewModel thongKe,
            TrangThaiThucTeTienDo trangThaiThucTe,
            AiDataset? datasetMoiNhat)
        {
            if (trangThaiThucTe == TrangThaiThucTeTienDo.HoanThanhTreHan
                || trangThaiThucTe == TrangThaiThucTeTienDo.DangThucHienQuaHan)
            {
                if (thongKe.DuAnBiTreTheoAi != true)
                {
                    thongKe.DuAnBiTreTheoAi = true;
                    thongKe.TenNguyenNhanAiDuDoan = GoiYNguyenNhanTre(datasetMoiNhat ?? new AiDataset
                    {
                        LaDuAnTre = true,
                        SoCongViecTre = thongKe.CongViecTreHan,
                        TyLeCongViecTre = thongKe.TongCongViec > 0
                            ? Math.Round((double)thongKe.CongViecTreHan * 100d / thongKe.TongCongViec, 2)
                            : 0d,
                        ChiPhiDuKien = thongKe.TongNganSach,
                        ChiPhiThucTe = thongKe.TongChiPhi,
                        SoLanThayDoiNhanSu = 0,
                        SoLanThayDoiQuanLy = 0,
                        SoNgayTreTienDo = thongKe.SoNgayQuaHan ?? 0
                    });
                    thongKe.KetQuaAiCoTheDaCu = true;
                    thongKe.CanhBaoDuLieuAi = CanhBaoKetQuaAiMauThuanThucTe;
                    thongKe.CanPhanTichAi = true;
                }
            }
        }

        private static void GanDuLieuAiMacDinh(DanhGiaDuAnThongKeViewModel thongKe)
        {
            thongKe.CoDuLieuAi = false;
            thongKe.DuAnBiTreTheoAi = null;
            thongKe.MaDmNguyenNhanAiDuDoan = null;
            thongKe.TenNguyenNhanAiDuDoan = null;
            thongKe.NguonNguyenNhanAi = null;
            thongKe.TenModelTreHanAi = null;
            thongKe.TenModelNguyenNhanAi = null;
            thongKe.DoTinCayAi = null;
            thongKe.MucPhuHopAi = null;
            thongKe.DanhSachNguyenNhanLienQuan = null;
            thongKe.ThoiGianDuDoanAi = null;
            thongKe.MaDmNguyenNhanManagerXacNhan = null;
            thongKe.TenNguyenNhanManagerXacNhan = null;
            thongKe.DoTinCayManagerXacNhan = null;
            thongKe.ThoiGianManagerXacNhan = null;
            thongKe.TrangThaiDuLieuAi = TrangThaiDuLieuAiMacDinh;
            thongKe.KetQuaAiCoTheDaCu = false;
            thongKe.CanhBaoDuLieuAi = null;
            thongKe.CanPhanTichAi = false;
            thongKe.LyDoCanPhanTichAi = null;
        }

        private static DanhGiaDuAnAiInsightViewModel BuildAiInsightViewModelAsync(DanhGiaDuAnThongKeViewModel thongKe)
        {
            var trangThaiThucTe = XacDinhTrangThaiThucTeTienDo(thongKe);
            var coDuLieuAi = thongKe.CoDuLieuAi == true;
            var laDuAnTre = thongKe.DuAnBiTreTheoAi == true;
            var tinhTrangTienDo = "Chưa đủ dữ liệu";
            switch (trangThaiThucTe)
            {
                case TrangThaiThucTeTienDo.HoanThanhDungHan:
                    tinhTrangTienDo = "Không trễ";
                    break;
                case TrangThaiThucTeTienDo.HoanThanhTreHan:
                    tinhTrangTienDo = "Trễ hạn";
                    break;
                case TrangThaiThucTeTienDo.DangThucHienQuaHan:
                    tinhTrangTienDo = laDuAnTre ? "Trễ hạn" : "Quá hạn";
                    break;
                case TrangThaiThucTeTienDo.DangThucHienChuaQuaHan:
                    tinhTrangTienDo = laDuAnTre ? "Nguy cơ trễ" : "Đúng tiến độ";
                    break;
                case TrangThaiThucTeTienDo.ChuaDuDuLieuThoiHan:
                    tinhTrangTienDo = laDuAnTre ? "Nguy cơ trễ" : "Chưa đủ dữ liệu";
                    break;
            }

            var nguyenNhanAi = thongKe.DuAnBiTreTheoAi == false || trangThaiThucTe == TrangThaiThucTeTienDo.HoanThanhDungHan
                ? NguyenNhanKhongTre
                : (laDuAnTre
                    ? (string.IsNullOrWhiteSpace(thongKe.TenNguyenNhanAiDuDoan) ? "Chưa có gợi ý AI" : thongKe.TenNguyenNhanAiDuDoan)
                    : "Chưa có gợi ý AI");
            var doTinCayAi = thongKe.DuAnBiTreTheoAi == false || trangThaiThucTe == TrangThaiThucTeTienDo.HoanThanhDungHan
                ? "Không áp dụng"
                : (laDuAnTre ? XacDinhMucPhuHop(thongKe.MucPhuHopAi, thongKe.DoTinCayAi) : "Chưa có");
            var thoiGianDuDoanAi = thongKe.DuAnBiTreTheoAi == false || trangThaiThucTe == TrangThaiThucTeTienDo.HoanThanhDungHan
                ? "Không áp dụng"
                : (laDuAnTre ? (thongKe.ThoiGianDuDoanAi?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có") : "Chưa có");
            var nguonNguyenNhanAi = thongKe.DuAnBiTreTheoAi == false || trangThaiThucTe == TrangThaiThucTeTienDo.HoanThanhDungHan
                ? "Không áp dụng"
                : (laDuAnTre
                    ? (string.IsNullOrWhiteSpace(thongKe.NguonNguyenNhanAi) ? "RuleFallback" : thongKe.NguonNguyenNhanAi)
                    : "Chưa có");
            var thongBaoKhongTre = trangThaiThucTe == TrangThaiThucTeTienDo.HoanThanhDungHan
                ? ThongBaoDuAnDungHanKhongCanPhanTich
                : "Dự án hiện chưa được xác định là trễ, không cần phân tích nguyên nhân trễ.";

            return new DanhGiaDuAnAiInsightViewModel
            {
                CoDuLieuAi = coDuLieuAi,
                DuAnBiTreTheoAi = thongKe.DuAnBiTreTheoAi,
                TinhTrangTienDo = tinhTrangTienDo,
                NguyenNhanAiDuDoan = nguyenNhanAi ?? "Chưa có gợi ý AI",
                DoTinCayAi = doTinCayAi,
                ThoiGianDuDoanAi = thoiGianDuDoanAi,
                NguonNguyenNhanAi = nguonNguyenNhanAi,
                ModelTreHan = "Không dùng",
                ModelNguyenNhan = string.IsNullOrWhiteSpace(thongKe.TenModelNguyenNhanAi) ? "Fallback rule" : thongKe.TenModelNguyenNhanAi,
                NguyenNhanManagerXacNhan = thongKe.DuAnBiTreTheoAi == true
                    ? (string.IsNullOrWhiteSpace(thongKe.TenNguyenNhanManagerXacNhan) ? "Chưa xác nhận" : thongKe.TenNguyenNhanManagerXacNhan)
                    : "Không cần xác nhận",
                DoTinCayManagerXacNhan = DinhDangPhanTram(thongKe.DoTinCayManagerXacNhan, "Chưa xác nhận"),
                HienThiThongBaoKhongTre = thongKe.DuAnBiTreTheoAi == false,
                ThongBaoKhongTre = thongBaoKhongTre,
                KetQuaAiCoTheDaCu = thongKe.KetQuaAiCoTheDaCu,
                CanhBaoDuLieuAi = thongKe.CanhBaoDuLieuAi,
                TrangThaiDuLieuAi = string.IsNullOrWhiteSpace(thongKe.TrangThaiDuLieuAi) ? TrangThaiDuLieuAiMacDinh : thongKe.TrangThaiDuLieuAi!,
                CoThePhanTichAi = thongKe.CoThePhanTichAi,
                CanPhanTichAi = thongKe.CanPhanTichAi,
                TuDongPhanTichAi = thongKe.TuDongPhanTichAi,
                LyDoCanPhanTichAi = thongKe.LyDoCanPhanTichAi,
                NutPhanTichText = coDuLieuAi ? "Phân tích lại nguyên nhân" : "Phân tích nguyên nhân trễ",
                DanhSachNguyenNhanLienQuan = ChuanHoaDanhSachNguyenNhanLienQuan(
                    thongKe.DanhSachNguyenNhanLienQuan,
                    thongKe.MaDmNguyenNhanAiDuDoan,
                    thongKe.TenNguyenNhanAiDuDoan)
            };
        }

        private static string XacDinhMucPhuHop(string? mucPhuHop, double? doTinCay)
        {
            if (!string.IsNullOrWhiteSpace(mucPhuHop))
            {
                var normalized = mucPhuHop.Trim();
                if (normalized.Equals("Cao", StringComparison.OrdinalIgnoreCase))
                {
                    return "Cao";
                }

                if (normalized.Equals("Trung bình", StringComparison.OrdinalIgnoreCase)
                    || normalized.Equals("Trung binh", StringComparison.OrdinalIgnoreCase))
                {
                    return "Trung bình";
                }

                if (normalized.Equals("Thấp", StringComparison.OrdinalIgnoreCase)
                    || normalized.Equals("Thap", StringComparison.OrdinalIgnoreCase))
                {
                    return "Thấp";
                }
            }

            return XacDinhMucPhuHopTuDoTinCay(doTinCay);
        }

        private static string XacDinhMucPhuHopTuDoTinCay(double? doTinCay)
        {
            if (!doTinCay.HasValue)
            {
                return "Chưa có";
            }

            if (doTinCay.Value >= 0.75d)
            {
                return "Cao";
            }

            if (doTinCay.Value >= 0.5d)
            {
                return "Trung bình";
            }

            return "Thấp";
        }

        private static string XacDinhMucPhuHopTuScore(double? score)
        {
            if (!score.HasValue || score.Value <= 0d)
            {
                return "Chưa có";
            }

            if (score.Value >= 0.75d)
            {
                return "Cao";
            }

            if (score.Value >= 0.5d)
            {
                return "Trung bình";
            }

            return "Thấp";
        }

        private static List<DanhGiaDuAnRelatedReasonViewModel>? ChuanHoaDanhSachNguyenNhanLienQuan(
            IEnumerable<DanhGiaDuAnRelatedReasonViewModel>? source,
            int? maNguyenNhanChinh,
            string? tenNguyenNhanChinh)
        {
            if (source == null)
            {
                return null;
            }

            var normalizedMainName = ChuanHoaTenSoSanh(tenNguyenNhanChinh);
            var result = source
                .Where(x => !string.IsNullOrWhiteSpace(x.TenNguyenNhan))
                .Select(x =>
                {
                    var score = x.Score.GetValueOrDefault();
                    var tenNguyenNhan = x.TenNguyenNhan!.Trim();
                    var laTrungMaNguyenNhanChinh = maNguyenNhanChinh.HasValue
                        && x.MaDMNguyenNhan.HasValue
                        && maNguyenNhanChinh.Value == x.MaDMNguyenNhan.Value;
                    var laTrungTenNguyenNhanChinh = ChuanHoaTenSoSanh(tenNguyenNhan) == normalizedMainName;
                    var laTrungNguyenNhanChinh = laTrungMaNguyenNhanChinh || laTrungTenNguyenNhanChinh;
                    var diemHopLe = score > 0.05d || (score > 0d && !laTrungTenNguyenNhanChinh);

                    return new
                    {
                        Item = x,
                        Score = score,
                        TenNguyenNhan = tenNguyenNhan,
                        LaTrungNguyenNhanChinh = laTrungNguyenNhanChinh,
                        DiemHopLe = diemHopLe
                    };
                })
                .Where(x => !x.LaTrungNguyenNhanChinh)
                .Where(x => x.DiemHopLe)
                .OrderByDescending(x => x.Score)
                .Take(3)
                .Select(x => new DanhGiaDuAnRelatedReasonViewModel
                {
                    MaDMNguyenNhan = x.Item.MaDMNguyenNhan,
                    TenNguyenNhan = x.TenNguyenNhan,
                    Score = x.Score,
                    MucPhuHop = string.IsNullOrWhiteSpace(x.Item.MucPhuHop)
                        ? XacDinhMucPhuHopTuScore(x.Score)
                        : XacDinhMucPhuHop(x.Item.MucPhuHop, x.Score)
                })
                .ToList();

            return result.Count > 0 ? result : null;
        }

        private static string ChuanHoaTenSoSanh(string? text)
            => string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : text.Trim().ToUpperInvariant();

        private static AiRelatedReasonStoragePayload ParseStoredNoiDungPhanTich(string? rawNoiDungPhanTich)
        {
            if (string.IsNullOrWhiteSpace(rawNoiDungPhanTich))
            {
                return new AiRelatedReasonStoragePayload();
            }

            if (!rawNoiDungPhanTich.StartsWith(AiRelatedReasonsPayloadMarker, StringComparison.Ordinal))
            {
                return new AiRelatedReasonStoragePayload
                {
                    NoiDungPhanTich = rawNoiDungPhanTich
                };
            }

            try
            {
                var rawJson = rawNoiDungPhanTich[AiRelatedReasonsPayloadMarker.Length..];
                var payload = JsonSerializer.Deserialize<AiRelatedReasonStoragePayload>(
                    rawJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (payload == null)
                {
                    return new AiRelatedReasonStoragePayload
                    {
                        NoiDungPhanTich = rawNoiDungPhanTich
                    };
                }

                return payload;
            }
            catch
            {
                return new AiRelatedReasonStoragePayload
                {
                    NoiDungPhanTich = rawNoiDungPhanTich
                };
            }
        }

        private static string DinhDangPhanTram(double? giaTri, string fallback = "Chưa có")
        {
            if (!giaTri.HasValue)
            {
                return fallback;
            }

            return $"{(giaTri.Value * 100d):0.##}%";
        }

        private static string GoiYNguyenNhanTre(AiDataset dataset)
        {
            if (dataset.LaDuAnTre != true)
            {
                return NguyenNhanKhongTre;
            }

            var tyLeCongViecTre = NormalizeDelayRatio(dataset.TyLeCongViecTre ?? 0d);
            var soCongViecTre = dataset.SoCongViecTre ?? 0;
            if (soCongViecTre > 0 && (soCongViecTre >= AiReasonHeuristic.SevereOverdueTasksThreshold || tyLeCongViecTre >= AiReasonHeuristic.SevereDelayRatioThreshold))
            {
                return "Nhiều công việc trễ hạn";
            }

            var chiPhiDuKien = dataset.ChiPhiDuKien ?? 0m;
            var chiPhiThucTe = dataset.ChiPhiThucTe ?? 0m;
            var chenhLechChiPhi = dataset.ChenhLechChiPhi ?? (chiPhiThucTe - chiPhiDuKien);
            if (LaVuotNganSachHopLe(chiPhiDuKien, chiPhiThucTe, chenhLechChiPhi))
            {
                return "Vượt ngân sách";
            }

            if ((dataset.SoLanThayDoiNhanSu ?? 0) >= AiReasonHeuristic.HighStaffChangeThreshold)
            {
                return "Biến động nhân sự";
            }

            if ((dataset.SoLanThayDoiQuanLy ?? 0) >= AiReasonHeuristic.HighManagerChangeThreshold)
            {
                return "Thay đổi quản lý";
            }

            if ((dataset.SoNgayTreTienDo ?? 0) >= AiReasonHeuristic.LongDelayDaysThreshold)
            {
                return "Trễ tiến độ kéo dài";
            }

            if (tyLeCongViecTre >= AiReasonHeuristic.HighDelayRatioThreshold)
            {
                return "Tiến độ cập nhật không đầy đủ";
            }

            return NguyenNhanChuaDuDuLieu;
        }

        private static double NormalizeDelayRatio(double value)
        {
            if (value <= 0d)
            {
                return 0d;
            }

            var ratio = value > 1d ? value / 100d : value;
            return Math.Clamp(ratio, 0d, 1d);
        }

        private static bool LaNguyenNhanVuotNganSach(string? tenNguyenNhan)
        {
            var normalized = TrangThai.Normalize(tenNguyenNhan).Replace(" ", string.Empty);
            return normalized.Contains("vuotngansach")
                   || normalized.Contains("ngansach")
                   || normalized.Contains("chiphi");
        }

        private static bool LaVuotNganSachHopLe(decimal chiPhiDuKien, decimal chiPhiThucTe, decimal chenhLechChiPhi)
        {
            if (chiPhiThucTe <= chiPhiDuKien || chenhLechChiPhi <= 0m)
            {
                return false;
            }

            if (chiPhiDuKien <= 0m)
            {
                return chenhLechChiPhi > 0m;
            }

            var tyLeVuot = (double)((chiPhiThucTe - chiPhiDuKien) / chiPhiDuKien);
            return tyLeVuot >= AiReasonHeuristic.HighCostOverrunThreshold;
        }

        private static bool LaNguyenNhanPheDuyetMacDinh(string tenNguyenNhan)
        {
            var normalized = tenNguyenNhan.Trim().ToLowerInvariant();
            return normalized == "cham phe duyet" || normalized == "chậm phê duyệt";
        }

        private static bool LaNguyenNhanKhongTre(string tenNguyenNhan)
        {
            var normalized = tenNguyenNhan.Trim().ToLowerInvariant();
            return normalized == "khong co nguyen nhan tre"
                   || normalized == "không có nguyên nhân trễ";
        }

        private static string ChuanHoaTenNguyenNhanHienThi(string? tenNguyenNhan)
        {
            if (string.IsNullOrWhiteSpace(tenNguyenNhan))
            {
                return NguyenNhanChuaDuDuLieu;
            }

            var normalized = tenNguyenNhan.Trim().ToLowerInvariant();
            return normalized switch
            {
                "cham phe duyet" => "Chậm phê duyệt",
                "bien dong nhan su" => "Biến động nhân sự",
                "thay doi quan ly" => "Thay đổi quản lý",
                "vuot ngan sach" => "Vượt ngân sách",
                "nhieu cong viec tre han" => "Nhiều công việc trễ hạn",
                "tre tien do keo dai" => "Trễ tiến độ kéo dài",
                _ => tenNguyenNhan.Trim()
            };
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
                   || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.ChoXacNhanHoanThanh)
                   || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.LuuTru);
        }

        private static double TinhTyLe(int tuSo, int mauSo)
        {
            if (mauSo <= 0)
            {
                return 0d;
            }

            return Math.Round((double)tuSo * 100d / mauSo, 2);
        }

        private static bool LaHanhDongThayDoiNhanSu(string? hanhDong)
        {
            var normalized = TrangThai.Normalize(hanhDong).Replace(" ", string.Empty);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            return normalized.Contains("themnhansu")
                   || normalized.Contains("themnhanvien")
                   || normalized.Contains("themthanhvien")
                   || normalized.Contains("xoanhansu")
                   || normalized.Contains("xoanhanvien")
                   || normalized.Contains("xoathanhvien")
                   || normalized.Contains("gonhanvien")
                   || normalized.Contains("gonhansu")
                   || normalized.Contains("bochon")
                   || normalized.Contains("capnhatvaitrophutrach")
                   || normalized.Contains("thaydoiphutrach")
                   || normalized.Contains("doiphutrach")
                   || normalized.Contains("dieuchuyennhansu")
                   || normalized.Contains("ganthanhvien");
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
            if (!CoQuyenTheoClaim(tenQuyen))
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user?.Identity?.IsAuthenticated != true)
                {
                    throw new Exception("Ban chua dang nhap.");
                }

                throw new Exception("Ban khong co quyen truy cap chuc nang danh gia du an.");
            }
        }

        private bool CoQuyenTheoClaim(params string[] tenQuyen)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var granted = user.Claims
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => x.Value.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return tenQuyen.Any(granted.Contains);
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


