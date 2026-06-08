using System.Security.Claims;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Dashboard;

namespace QuanLyDuAn.Services.Implementations;

public class DashboardService : IDashboardService
{
    private const int MaxDuAnFilterOptions = 500;

    private readonly QuanLyDuAnDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        QuanLyDuAnDbContext db,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DashboardService> logger)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(
        DateTime? tuNgay,
        DateTime? denNgay,
        string? locNhanh,
        int? locMaDuAn = null,
        int? locMaQuanLy = null,
        int? locMaTeam = null,
        string? locTrangThai = null,
        int? locMaLoaiDuAn = null,
        string? locTheoNgay = null)
    {
        var (tuNgayLoc, denNgayLoc, locNhanhResolved) = ChuanHoaKhoangThoiGian(tuNgay, denNgay, locNhanh);
        var denNgayDocQuyen = denNgayLoc?.Date.AddDays(1);
        var locTheo = string.IsNullOrWhiteSpace(locTheoNgay) ? "NgayTao" : locTheoNgay.Trim();
        var scope = await LogQueryAsync("PhamViDuAn", () => LayPhamViDuAnHienTaiAsync());
        var filterOptions = await LogQueryAsync("Load filter options", () => LayFilterOptionsAsync(scope));

        var duAnScopeQuery = TaoDuAnTheoScopeQuery(scope);
        var duAnThongKeQuery = duAnScopeQuery;
        if (locMaDuAn.HasValue && locMaDuAn.Value > 0)
        {
            duAnThongKeQuery = duAnThongKeQuery.Where(x => x.MaDuAn == locMaDuAn.Value);
        }

        if (locMaQuanLy.HasValue && locMaQuanLy.Value > 0)
        {
            duAnThongKeQuery = duAnThongKeQuery.Where(x => x.MaNguoiDung == locMaQuanLy.Value);
        }

        if (locMaTeam.HasValue && locMaTeam.Value > 0)
        {
            duAnThongKeQuery = duAnThongKeQuery.Where(x => _db.TeamDuAn.AsNoTracking().Any(td => td.MaDuAn == x.MaDuAn && td.MaTeam == locMaTeam.Value));
        }

        if (locMaLoaiDuAn.HasValue && locMaLoaiDuAn.Value > 0)
        {
            duAnThongKeQuery = duAnThongKeQuery.Where(x => x.MaLoaiDuAn == locMaLoaiDuAn.Value);
        }

        if (!string.IsNullOrWhiteSpace(locTrangThai))
        {
            var trangThaiFilter = TrangThai.GetCommonStatusVariants(locTrangThai);
            if (trangThaiFilter.Length > 0)
            {
                duAnThongKeQuery = duAnThongKeQuery.Where(x => trangThaiFilter.Contains(x.TrangThaiDuAn ?? string.Empty));
            }
        }

        duAnThongKeQuery = ApDungLocNgayDuAn(duAnThongKeQuery, tuNgayLoc, denNgayDocQuyen, locTheo);
        var maDuAnDaLoc = await LogQueryAsync("MaDuAnDaLoc", () => duAnThongKeQuery.Select(x => x.MaDuAn).ToListAsync());
        if (maDuAnDaLoc.Count == 0)
        {
            return TaoDashboardRong(
                filterOptions,
                tuNgayLoc,
                denNgayLoc,
                locNhanhResolved,
                locMaDuAn,
                locMaQuanLy,
                locMaTeam,
                locTrangThai,
                locMaLoaiDuAn,
                locTheo);
        }

        IQueryable<CongViec> congViecQuery =
            from cv in _db.CongViec.AsNoTracking()
            join dm in _db.DanhMucCongViec.AsNoTracking() on cv.MaDanhMucCV equals dm.MaDanhMucCV
            where cv.IsDeleted != true
                  && dm.IsDeleted != true
                  && maDuAnDaLoc.Contains(dm.MaDuAn)
            select cv;

        var deXuatCongViecQuery = _db.DeXuatCongViec
            .AsNoTracking()
            .Where(x => x.IsDeleted != true && maDuAnDaLoc.Contains(x.MaDuAn));
        var deXuatNganSachQuery = _db.DeXuatNganSach
            .AsNoTracking()
            .Where(x => x.IsDeleted != true && maDuAnDaLoc.Contains(x.MaDuAn));

        if (tuNgayLoc.HasValue)
        {
            deXuatCongViecQuery = deXuatCongViecQuery.Where(x => x.NgayDeXuatCongViec.HasValue && x.NgayDeXuatCongViec.Value >= tuNgayLoc.Value);
            deXuatNganSachQuery = deXuatNganSachQuery.Where(x => x.NgayDeXuat.HasValue && x.NgayDeXuat.Value >= tuNgayLoc.Value);
        }

        if (denNgayDocQuyen.HasValue)
        {
            deXuatCongViecQuery = deXuatCongViecQuery.Where(x => x.NgayDeXuatCongViec.HasValue && x.NgayDeXuatCongViec.Value < denNgayDocQuyen.Value);
            deXuatNganSachQuery = deXuatNganSachQuery.Where(x => x.NgayDeXuat.HasValue && x.NgayDeXuat.Value < denNgayDocQuyen.Value);
        }

        var nganSachQuery = _db.NganSach
            .AsNoTracking()
            .Where(x => x.IsDeleted != true && x.IsActive == true && maDuAnDaLoc.Contains(x.MaDuAn));

        var expenseByProjectQuery =
            from cp in _db.ChiPhi.AsNoTracking()
            join ns in _db.NganSach.AsNoTracking() on cp.MaNganSach equals ns.MaNganSach
            where cp.IsDeleted != true
                  && ns.IsDeleted != true
                  && maDuAnDaLoc.Contains(ns.MaDuAn)
            select new { cp.NgayChi, cp.SoTienDaChi, ns.MaDuAn };
        if (tuNgayLoc.HasValue)
        {
            expenseByProjectQuery = expenseByProjectQuery.Where(x => x.NgayChi.HasValue && x.NgayChi.Value >= tuNgayLoc.Value);
        }
        if (denNgayDocQuyen.HasValue)
        {
            expenseByProjectQuery = expenseByProjectQuery.Where(x => x.NgayChi.HasValue && x.NgayChi.Value < denNgayDocQuyen.Value);
        }

        var projectsForChart = await LogQueryAsync("BieuDoDuAn", () => duAnThongKeQuery
            .OrderByDescending(x => x.MaDuAn)
            .Take(12)
            .ToListAsync());

        var totalProjects = maDuAnDaLoc.Count;
        var totalTasks = await LogQueryAsync("TongCongViec", () => congViecQuery.CountAsync());
        var totalEmployees = await LogQueryAsync("TongNhanSu", () => _db.NguoiDung.AsNoTracking().CountAsync(x => x.IsDeleted != true));
        var totalBudget = await LogQueryAsync("TongNganSach", () => nganSachQuery.SumAsync(x => x.SoTienNganSach ?? 0));

        var expenseByProject = await LogQueryAsync("TongChiPhiTheoDuAn", () => expenseByProjectQuery
            .GroupBy(x => x.MaDuAn)
            .Select(g => new { ProjectId = g.Key, Total = g.Sum(x => x.SoTienDaChi ?? 0) })
            .ToListAsync());
        var expenseMap = expenseByProject.ToDictionary(x => x.ProjectId, x => x.Total);
        var totalExpense = expenseByProject.Sum(x => x.Total);

        var duAnStatuses = await LogQueryAsync("BieuDoTrangThaiDuAn", () => duAnThongKeQuery.Select(x => x.TrangThaiDuAn).ToListAsync());
        var congViecStatuses = await LogQueryAsync("BieuDoTrangThaiCongViec", () => congViecQuery.Select(x => x.TrangThaiCongViec).ToListAsync());
        var deXuatCongViecStatuses = await LogQueryAsync("DeXuatCongViecChoDuyet", () => deXuatCongViecQuery.Select(x => x.TrangThaiCongViecDeXuat).ToListAsync());
        var deXuatNganSachStatuses = await LogQueryAsync("DeXuatNganSachChoDuyet", () => deXuatNganSachQuery.Select(x => x.TrangThaiDeXuat).ToListAsync());
        var yeuCauDoiQuanLyStatuses = await LogQueryAsync("YeuCauDoiQuanLyChoDuyet", () => _db.YeuCauDoiQuanLy
            .AsNoTracking()
            .Where(x => x.IsDeleted != true && maDuAnDaLoc.Contains(x.MaDuAn))
            .Select(x => x.TrangThaiYeuCauDoiQuanLy)
            .ToListAsync());

        var duAnHoanThanhTimelineRows = await LogQueryAsync("DuAnHoanThanhTheoPhamVi", () => LayDuAnHoanThanhTheoPhamViAsync(duAnThongKeQuery, maDuAnDaLoc));
        var duAnHoanThanhTrongKy = duAnHoanThanhTimelineRows.Count(x =>
            x.NgayHoanThanhThucTe.HasValue
            && (!tuNgayLoc.HasValue || x.NgayHoanThanhThucTe.Value >= tuNgayLoc.Value)
            && (!denNgayDocQuyen.HasValue || x.NgayHoanThanhThucTe.Value < denNgayDocQuyen.Value));
        var duAnHoanThanhDungHan = duAnHoanThanhTimelineRows.Count(x =>
            x.NgayKetThucDuAn.HasValue
            && x.NgayHoanThanhThucTe.HasValue
            && x.NgayHoanThanhThucTe.Value.Date <= x.NgayKetThucDuAn.Value.Date);
        var duAnHoanThanhTreHan = duAnHoanThanhTimelineRows.Count(x =>
            x.NgayKetThucDuAn.HasValue
            && x.NgayHoanThanhThucTe.HasValue
            && x.NgayHoanThanhThucTe.Value.Date > x.NgayKetThucDuAn.Value.Date);

        var congViecTreHanQuery = congViecQuery.Where(x =>
            x.NgayKetThucCVDuKien.HasValue
            && x.NgayKetThucCVDuKien.Value < DateTime.Now
            && !TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh).Contains(x.TrangThaiCongViec ?? string.Empty));

        var congViecTreHan = await LogQueryAsync("CongViecTreHan", () => congViecTreHanQuery.CountAsync());
        var topDuAnTre = await LogQueryAsync("TopDuAnTre", () => LayTopDuAnTreAsync(duAnThongKeQuery));
        var topDuAnVuotNganSach = await LogQueryAsync("TopDuAnVuotNganSach", () => LayTopDuAnVuotNganSachAsync(duAnThongKeQuery, maDuAnDaLoc));
        var topNhanSuQuaTai = await LogQueryAsync("TopNhanSuQuaTai", () => LayTopNhanSuQuaTaiAsync(maDuAnDaLoc));
        var topCongViecTre = await LogQueryAsync("TopCongViecTre", () => LayTopCongViecTreAsync(maDuAnDaLoc));
        var thongKeTheoThang = await LogQueryAsync("BieuDoThoiGian", () => LayThongKeTheoThangAsync(duAnThongKeQuery, maDuAnDaLoc));

        int CountDuAnByStatus(string expected)
            => duAnStatuses.Count(x => TrangThai.EqualsValue(x, expected));

        int CountCongViecByStatus(string expected)
            => congViecStatuses.Count(x => TrangThai.EqualsValue(x, expected));

        var duAnTreTienDo = topDuAnTre.Count;

        return new DashboardViewModel
        {
            TuNgay = tuNgayLoc,
            DenNgay = denNgayLoc,
            LocNhanh = locNhanhResolved,
            LocMaDuAn = locMaDuAn,
            LocMaQuanLy = locMaQuanLy,
            LocMaTeam = locMaTeam,
            LocTrangThai = locTrangThai,
            LocMaLoaiDuAn = locMaLoaiDuAn,
            LocTheoNgay = locTheo,
            DuAnOptions = filterOptions.DuAnOptions,
            QuanLyOptions = filterOptions.QuanLyOptions,
            TeamOptions = filterOptions.TeamOptions,
            LoaiDuAnOptions = filterOptions.LoaiDuAnOptions,

            TongDuAn = totalProjects,
            TongCongViec = totalTasks,
            TongNhanVien = totalEmployees,
            TongNganSach = totalBudget,
            TongChiPhi = totalExpense,
            TongDeXuat = deXuatCongViecStatuses.Count + deXuatNganSachStatuses.Count,
            DuAnHoanThanhTrongKy = duAnHoanThanhTrongKy,
            NganSachConLai = totalBudget - totalExpense,
            TyLeSuDungNganSach = totalBudget > 0
                ? Math.Round((totalExpense / totalBudget) * 100m, 2, MidpointRounding.AwayFromZero)
                : 0,

            TenDuAn = projectsForChart.Select(x => x.TenDuAn ?? $"Dự án #{x.MaDuAn}").ToList(),
            PhanTramTienDo = projectsForChart.Select(x => x.PhanTramHoanThanh ?? 0).ToList(),
            ChiPhiTheoDuAn = projectsForChart.Select(x => expenseMap.TryGetValue(x.MaDuAn, out var value) ? value : 0).ToList(),

            DuAnKhoiTao = CountDuAnByStatus(TrangThai.KhoiTao),
            DuAnDangThucHien = CountDuAnByStatus(TrangThai.DangThucHien),
            DuAnTamDung = CountDuAnByStatus(TrangThai.TamDung),
            DuAnChoXacNhanHoanThanh = CountDuAnByStatus(TrangThai.ChoXacNhanHoanThanh),
            DuAnHoanThanh = duAnStatuses.Count(TrangThai.LaHoanThanhCongViec),
            DuAnHoanThanhDungHan = duAnHoanThanhDungHan,
            DuAnHoanThanhTreHan = duAnHoanThanhTreHan,
            DuAnLuuTru = CountDuAnByStatus(TrangThai.LuuTru),
            DuAnDungTienDo = Math.Max(0, totalProjects - duAnTreTienDo),
            DuAnTreTienDo = duAnTreTienDo,

            CongViecTreHan = congViecTreHan,
            NhanSuQuaTai = topNhanSuQuaTai.Count,
            DuAnVuotNganSach = topDuAnVuotNganSach.Count,

            DeXuatCongViecChoDuyet = deXuatCongViecStatuses.Count(x => TrangThai.EqualsValue(x, TrangThai.ChoDuyet)),
            DeXuatNganSachChoDuyet = deXuatNganSachStatuses.Count(x => TrangThai.EqualsValue(x, TrangThai.ChoDuyet)),
            YeuCauDoiQuanLyChoDuyet = yeuCauDoiQuanLyStatuses.Count(x => TrangThai.EqualsValue(x, TrangThai.ChoDuyet)),

            TrangThaiDuAnChart = [
                new() { Label = "Khởi tạo", Value = CountDuAnByStatus(TrangThai.KhoiTao) },
                new() { Label = "Đang thực hiện", Value = CountDuAnByStatus(TrangThai.DangThucHien) },
                new() { Label = "Tạm dừng", Value = CountDuAnByStatus(TrangThai.TamDung) },
                new() { Label = "Chờ xác nhận", Value = CountDuAnByStatus(TrangThai.ChoXacNhanHoanThanh) },
                new() { Label = "Hoàn thành", Value = duAnStatuses.Count(TrangThai.LaHoanThanhCongViec) },
                new() { Label = "Lưu trữ", Value = CountDuAnByStatus(TrangThai.LuuTru) }
            ],
            TrangThaiCongViecChart = [
                new() { Label = "Chưa bắt đầu", Value = CountCongViecByStatus(TrangThai.ChuaBatDau) },
                new() { Label = "Đang thực hiện", Value = CountCongViecByStatus(TrangThai.DangThucHien) },
                new() { Label = "Bị cản", Value = CountCongViecByStatus(TrangThai.BiCanCan) },
                new() { Label = "Chờ xác nhận", Value = CountCongViecByStatus(TrangThai.ChoXacNhanHoanThanh) },
                new() { Label = "Hoàn thành", Value = congViecStatuses.Count(TrangThai.LaHoanThanhCongViec) },
                new() { Label = "Tạm dừng", Value = CountCongViecByStatus(TrangThai.TamDung) }
            ],
            ThongKeTheoThang = thongKeTheoThang,
            TopDuAnTre = topDuAnTre,
            TopDuAnVuotNganSach = topDuAnVuotNganSach,
            TopNhanSuQuaTai = topNhanSuQuaTai,
            TopCongViecTre = topCongViecTre,
            WorkflowHealthItems = [],
            Suggestions = []
        };
    }

    private async Task<DashboardScope> LayPhamViDuAnHienTaiAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return new DashboardScope(false, []);
        }

        var isAdmin = user.IsInRole("Admin") || user.IsInRole("ADMIN");
        if (isAdmin)
        {
            return new DashboardScope(true, []);
        }

        var currentUserId = await LayMaNguoiDungHienTaiAsync(user);
        if (currentUserId <= 0)
        {
            return new DashboardScope(false, []);
        }

        var projectIds = new HashSet<int>();
        if (user.IsInRole("Manager") || user.IsInRole("MANAGER"))
        {
            var managed = await _db.DuAn
                .AsNoTracking()
                .Where(x => x.IsDeleted != true && x.MaNguoiDung == currentUserId)
                .Select(x => x.MaDuAn)
                .ToListAsync();
            foreach (var id in managed)
            {
                projectIds.Add(id);
            }

            return new DashboardScope(false, projectIds.ToList());
        }

        var joined = await _db.NhanVienDuAn
            .AsNoTracking()
            .Where(x => x.MaNguoiDung == currentUserId)
            .Select(x => x.MaDuAn)
            .ToListAsync();
        foreach (var id in joined)
        {
            projectIds.Add(id);
        }

        var teamProjects = await (
            from nt in _db.NhanVienTeam.AsNoTracking()
            join td in _db.TeamDuAn.AsNoTracking() on nt.MaTeam equals td.MaTeam
            where nt.MaNguoiDung == currentUserId
            select td.MaDuAn)
            .ToListAsync();
        foreach (var id in teamProjects)
        {
            projectIds.Add(id);
        }

        return new DashboardScope(false, projectIds.ToList());
    }

    private IQueryable<DuAn> TaoDuAnTheoScopeQuery(DashboardScope scope)
    {
        var query = _db.DuAn
            .AsNoTracking()
            .Where(x => x.IsDeleted != true);

        if (!scope.LaAdmin)
        {
            query = query.Where(x => scope.MaDuAnDuocXem.Contains(x.MaDuAn));
        }

        return query;
    }

    private async Task<DashboardFilterOptions> LayFilterOptionsAsync(DashboardScope scope)
    {
        var duAnScopeQuery = TaoDuAnTheoScopeQuery(scope);

        var duAnOptionRows = await duAnScopeQuery
            .OrderByDescending(x => x.NgayTaoDuAn ?? DateTime.MinValue)
            .ThenBy(x => x.TenDuAn)
            .Select(x => new
            {
                x.MaDuAn,
                x.TenDuAn
            })
            .Take(MaxDuAnFilterOptions)
            .ToListAsync();
        var duAnOptions = duAnOptionRows
            .Select(x => new DashboardFilterOptionViewModel
            {
                Value = x.MaDuAn,
                Text = string.IsNullOrWhiteSpace(x.TenDuAn) ? $"Dự án #{x.MaDuAn}" : x.TenDuAn
            })
            .OrderBy(x => x.Text)
            .ToList();

        var quanLyOptionRows = await (
            from da in duAnScopeQuery
            join nd in _db.NguoiDung.AsNoTracking() on da.MaNguoiDung equals nd.MaNguoiDung
            where nd.IsDeleted != true
            select new
            {
                nd.MaNguoiDung,
                nd.HoTenNguoiDung
            })
            .Distinct()
            .ToListAsync();
        var quanLyOptions = quanLyOptionRows
            .Select(x => new DashboardFilterOptionViewModel
            {
                Value = x.MaNguoiDung,
                Text = string.IsNullOrWhiteSpace(x.HoTenNguoiDung) ? $"Nhân sự {x.MaNguoiDung}" : x.HoTenNguoiDung
            })
            .OrderBy(x => x.Text)
            .ToList();

        var teamOptionRows = await (
            from da in duAnScopeQuery
            join td in _db.TeamDuAn.AsNoTracking() on da.MaDuAn equals td.MaDuAn
            join t in _db.Team.AsNoTracking() on td.MaTeam equals t.MaTeam
            where t.IsDeleted != true
            select new
            {
                t.MaTeam,
                t.TenTeam
            })
            .Distinct()
            .ToListAsync();
        var teamOptions = teamOptionRows
            .Select(x => new DashboardFilterOptionViewModel
            {
                Value = x.MaTeam,
                Text = string.IsNullOrWhiteSpace(x.TenTeam) ? $"Team {x.MaTeam}" : x.TenTeam
            })
            .OrderBy(x => x.Text)
            .ToList();

        var loaiDuAnOptionRows = await (
            from da in duAnScopeQuery
            join loai in _db.LoaiDuAn.AsNoTracking() on da.MaLoaiDuAn equals loai.MaLoaiDuAn
            select new
            {
                loai.MaLoaiDuAn,
                loai.TenLoai
            })
            .Distinct()
            .ToListAsync();
        var loaiDuAnOptions = loaiDuAnOptionRows
            .Select(x => new DashboardFilterOptionViewModel
            {
                Value = x.MaLoaiDuAn,
                Text = string.IsNullOrWhiteSpace(x.TenLoai) ? $"Loại dự án {x.MaLoaiDuAn}" : x.TenLoai
            })
            .OrderBy(x => x.Text)
            .ToList();

        return new DashboardFilterOptions(duAnOptions, quanLyOptions, teamOptions, loaiDuAnOptions);
    }

    private static DashboardViewModel TaoDashboardRong(
        DashboardFilterOptions filterOptions,
        DateTime? tuNgay,
        DateTime? denNgay,
        string? locNhanh,
        int? locMaDuAn,
        int? locMaQuanLy,
        int? locMaTeam,
        string? locTrangThai,
        int? locMaLoaiDuAn,
        string locTheoNgay)
    {
        return new DashboardViewModel
        {
            TuNgay = tuNgay,
            DenNgay = denNgay,
            LocNhanh = locNhanh,
            LocMaDuAn = locMaDuAn,
            LocMaQuanLy = locMaQuanLy,
            LocMaTeam = locMaTeam,
            LocTrangThai = locTrangThai,
            LocMaLoaiDuAn = locMaLoaiDuAn,
            LocTheoNgay = locTheoNgay,
            DuAnOptions = filterOptions.DuAnOptions,
            QuanLyOptions = filterOptions.QuanLyOptions,
            TeamOptions = filterOptions.TeamOptions,
            LoaiDuAnOptions = filterOptions.LoaiDuAnOptions,
            TenDuAn = [],
            PhanTramTienDo = [],
            ChiPhiTheoDuAn = [],
            TrangThaiDuAnChart = [],
            TrangThaiCongViecChart = [],
            ThongKeTheoThang = [],
            TopDuAnTre = [],
            TopDuAnVuotNganSach = [],
            TopNhanSuQuaTai = [],
            TopCongViecTre = [],
            WorkflowHealthItems = [],
            Suggestions = []
        };
    }

    private async Task<int> LayMaNguoiDungHienTaiAsync(ClaimsPrincipal user)
    {
        var claimValue = user.FindFirstValue("MaNguoiDung");
        if (int.TryParse(claimValue, out var maNguoiDung) && maNguoiDung > 0)
        {
            return maNguoiDung;
        }

        var aspUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(aspUserId))
        {
            return 0;
        }

        return await _db.Aspnetusers
            .AsNoTracking()
            .Where(x => x.Id == aspUserId)
            .Select(x => x.MaNguoiDung)
            .FirstOrDefaultAsync();
    }

    private async Task<T> LogQueryAsync<T>(string name, Func<Task<T>> query)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var result = await query();
            return result;
        }
        finally
        {
            sw.Stop();
            _logger.LogInformation("Dashboard query {Name} took {Elapsed} ms", name, sw.ElapsedMilliseconds);
        }
    }

    private static IQueryable<DuAn> ApDungLocNgayDuAn(IQueryable<DuAn> query, DateTime? tuNgay, DateTime? denNgayDocQuyen, string locTheo)
    {
        if (tuNgay.HasValue)
        {
            query = locTheo switch
            {
                "NgayBatDau" => query.Where(x => x.NgayBatDauDuAn.HasValue && x.NgayBatDauDuAn.Value >= tuNgay.Value),
                "NgayKetThuc" => query.Where(x => x.NgayKetThucDuAn.HasValue && x.NgayKetThucDuAn.Value >= tuNgay.Value),
                "NgayHoanThanh" => query.Where(x => x.NgayHoanThanhThucTeDuAn.HasValue && x.NgayHoanThanhThucTeDuAn.Value >= tuNgay.Value),
                _ => query.Where(x => x.NgayTaoDuAn.HasValue && x.NgayTaoDuAn.Value >= tuNgay.Value)
            };
        }

        if (denNgayDocQuyen.HasValue)
        {
            query = locTheo switch
            {
                "NgayBatDau" => query.Where(x => x.NgayBatDauDuAn.HasValue && x.NgayBatDauDuAn.Value < denNgayDocQuyen.Value),
                "NgayKetThuc" => query.Where(x => x.NgayKetThucDuAn.HasValue && x.NgayKetThucDuAn.Value < denNgayDocQuyen.Value),
                "NgayHoanThanh" => query.Where(x => x.NgayHoanThanhThucTeDuAn.HasValue && x.NgayHoanThanhThucTeDuAn.Value < denNgayDocQuyen.Value),
                _ => query.Where(x => x.NgayTaoDuAn.HasValue && x.NgayTaoDuAn.Value < denNgayDocQuyen.Value)
            };
        }

        return query;
    }

    private async Task<List<DashboardCompletionProjectRow>> LayDuAnHoanThanhTheoPhamViAsync(IQueryable<DuAn> duAnQuery, List<int> maDuAnDaLoc)
    {
        var trangThaiDuAnDaKetThuc = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh)
            .Concat(TrangThai.GetCommonStatusVariants(TrangThai.LuuTru))
            .ToArray();
        var rows = await duAnQuery
            .Where(x => trangThaiDuAnDaKetThuc.Contains(x.TrangThaiDuAn ?? string.Empty))
            .Select(x => new
            {
                x.MaDuAn,
                x.NgayKetThucDuAn,
                x.NgayHoanThanhThucTeDuAn
            })
            .ToListAsync();

        if (rows.Count == 0)
        {
            return [];
        }

        var maDuAnHoanThanh = rows
            .Select(x => x.MaDuAn)
            .Where(maDuAnDaLoc.Contains)
            .ToList();
        var fallback = await (
            from cv in _db.CongViec.AsNoTracking()
            join dm in _db.DanhMucCongViec.AsNoTracking() on cv.MaDanhMucCV equals dm.MaDanhMucCV
            where cv.IsDeleted != true
                  && dm.IsDeleted != true
                  && cv.NgayKetThucCVThucTe.HasValue
                  && maDuAnHoanThanh.Contains(dm.MaDuAn)
            group cv by dm.MaDuAn
            into g
            select new { MaDuAn = g.Key, NgayHoanThanh = g.Max(x => x.NgayKetThucCVThucTe) }
        ).ToDictionaryAsync(x => x.MaDuAn, x => x.NgayHoanThanh);

        return rows.Select(x => new DashboardCompletionProjectRow(
            x.MaDuAn,
            x.NgayKetThucDuAn,
            x.NgayHoanThanhThucTeDuAn ?? fallback.GetValueOrDefault(x.MaDuAn))).ToList();
    }

    private async Task<List<DashboardDelayedProjectItemViewModel>> LayTopDuAnTreAsync(IQueryable<DuAn> duAnQuery)
    {
        var today = DateTime.Today;
        var rows = await (
            from da in duAnQuery
            join nd in _db.NguoiDung.AsNoTracking() on da.MaNguoiDung equals nd.MaNguoiDung
            select new
            {
                da.TenDuAn,
                da.NgayKetThucDuAn,
                da.NgayHoanThanhThucTeDuAn,
                da.PhanTramHoanThanh,
                da.TrangThaiDuAn,
                TenQuanLy = nd.HoTenNguoiDung
            })
            .Where(x => x.NgayKetThucDuAn.HasValue
                && (x.NgayKetThucDuAn.Value < today
                    || (x.NgayHoanThanhThucTeDuAn.HasValue && x.NgayHoanThanhThucTeDuAn.Value > x.NgayKetThucDuAn.Value)))
            .OrderBy(x => x.NgayKetThucDuAn)
            .Take(30)
            .ToListAsync();

        return rows
            .Select(x =>
            {
                var daHoanThanh = TrangThai.LaHoanThanhCongViec(x.TrangThaiDuAn);
                var mocSoSanh = daHoanThanh
                    ? x.NgayHoanThanhThucTeDuAn?.Date
                    : today;
                var soNgayTre = x.NgayKetThucDuAn.HasValue && mocSoSanh.HasValue
                    ? Math.Max(0, (mocSoSanh.Value - x.NgayKetThucDuAn.Value.Date).Days)
                    : 0;
                return new DashboardDelayedProjectItemViewModel
                {
                    TenDuAn = x.TenDuAn ?? "Dự án chưa đặt tên",
                    TenQuanLy = x.TenQuanLy ?? "Chưa xác định",
                    NgayKetThuc = x.NgayKetThucDuAn,
                    SoNgayTre = soNgayTre,
                    PhanTramHoanThanh = x.PhanTramHoanThanh ?? 0
                };
            })
            .Where(x => x.SoNgayTre > 0)
            .OrderByDescending(x => x.SoNgayTre)
            .ThenBy(x => x.NgayKetThuc)
            .Take(5)
            .ToList();
    }

    private async Task<List<DashboardBudgetOverrunItemViewModel>> LayTopDuAnVuotNganSachAsync(IQueryable<DuAn> duAnQuery, List<int> maDuAnDaLoc)
    {
        var budgetRows = await _db.NganSach
            .AsNoTracking()
            .Where(x => x.IsDeleted != true
                && x.IsActive == true
                && maDuAnDaLoc.Contains(x.MaDuAn))
            .GroupBy(x => x.MaDuAn)
            .Select(g => new { MaDuAn = g.Key, NganSach = g.Sum(x => x.SoTienNganSach ?? 0) })
            .ToListAsync();

        var expenseRows = await (
            from cp in _db.ChiPhi.AsNoTracking()
            join ns in _db.NganSach.AsNoTracking() on cp.MaNganSach equals ns.MaNganSach
            where cp.IsDeleted != true
                  && ns.IsDeleted != true
                  && maDuAnDaLoc.Contains(ns.MaDuAn)
            group cp by ns.MaDuAn into g
            select new { MaDuAn = g.Key, ChiPhi = g.Sum(x => x.SoTienDaChi ?? 0) })
            .ToListAsync();

        var budgetMap = budgetRows.ToDictionary(x => x.MaDuAn, x => x.NganSach);
        var topIds = expenseRows
            .Select(x => new
            {
                x.MaDuAn,
                NganSach = budgetMap.GetValueOrDefault(x.MaDuAn),
                x.ChiPhi,
                ChenhLech = x.ChiPhi - budgetMap.GetValueOrDefault(x.MaDuAn)
            })
            .Where(x => x.NganSach > 0 && x.ChenhLech > 0)
            .OrderByDescending(x => x.ChenhLech)
            .Take(5)
            .ToList();

        var projectIds = topIds.Select(x => x.MaDuAn).ToList();
        var projectNames = await duAnQuery
            .Where(x => projectIds.Contains(x.MaDuAn))
            .Select(x => new { x.MaDuAn, x.TenDuAn })
            .ToListAsync();
        var nameMap = projectNames.ToDictionary(
            x => x.MaDuAn,
            x => string.IsNullOrWhiteSpace(x.TenDuAn) ? "Dự án chưa đặt tên" : x.TenDuAn);

        return topIds.Select(x => new DashboardBudgetOverrunItemViewModel
        {
            TenDuAn = nameMap.GetValueOrDefault(x.MaDuAn, "Dự án chưa đặt tên"),
            NganSach = x.NganSach,
            ChiPhi = x.ChiPhi,
            ChenhLech = x.ChenhLech
        }).ToList();
    }

    private async Task<List<DashboardOverloadedEmployeeItemViewModel>> LayTopNhanSuQuaTaiAsync(List<int> maDuAnDaLoc)
    {
        var completionStatuses = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
        var today = DateTime.Today;
        var workRows = await (
            from pc in _db.PhanCongCongViec.AsNoTracking()
            join cv in _db.CongViec.AsNoTracking() on pc.MaCongViec equals cv.MaCongViec
            join dm in _db.DanhMucCongViec.AsNoTracking() on cv.MaDanhMucCV equals dm.MaDanhMucCV
            where cv.IsDeleted != true
                  && dm.IsDeleted != true
                  && maDuAnDaLoc.Contains(dm.MaDuAn)
                  && !completionStatuses.Contains(cv.TrangThaiCongViec ?? string.Empty)
            group new { pc, cv } by pc.MaNguoiDung into g
            select new
            {
                MaNguoiDung = g.Key,
                SoCongViec = g.Select(x => x.pc.MaCongViec).Distinct().Count(),
                CongViecTre = g.Count(x => x.cv.NgayKetThucCVDuKien.HasValue && x.cv.NgayKetThucCVDuKien.Value < today)
            })
            .ToListAsync();

        var detailRows = await (
            from pc in _db.PhanCongCtCongViec.AsNoTracking()
            join ct in _db.CtCongViec.AsNoTracking() on pc.MaChiTietCV equals ct.MaChiTietCV
            join cv in _db.CongViec.AsNoTracking() on ct.MaCongViec equals cv.MaCongViec
            join dm in _db.DanhMucCongViec.AsNoTracking() on cv.MaDanhMucCV equals dm.MaDanhMucCV
            where ct.IsDeleted != true
                  && cv.IsDeleted != true
                  && dm.IsDeleted != true
                  && maDuAnDaLoc.Contains(dm.MaDuAn)
                  && !completionStatuses.Contains(ct.TrangThaiCTCV ?? string.Empty)
            group pc by pc.MaNguoiDung into g
            select new
            {
                MaNguoiDung = g.Key,
                SoChiTietCongViec = g.Select(x => x.MaChiTietCV).Distinct().Count()
            })
            .ToListAsync();

        var userIds = workRows.Select(x => x.MaNguoiDung)
            .Concat(detailRows.Select(x => x.MaNguoiDung))
            .Distinct()
            .ToList();
        var nameRows = await _db.NguoiDung
            .AsNoTracking()
            .Where(x => userIds.Contains(x.MaNguoiDung) && x.IsDeleted != true)
            .Select(x => new { x.MaNguoiDung, x.HoTenNguoiDung })
            .ToListAsync();
        var names = nameRows.ToDictionary(
            x => x.MaNguoiDung,
            x => string.IsNullOrWhiteSpace(x.HoTenNguoiDung) ? $"Nhân sự {x.MaNguoiDung}" : x.HoTenNguoiDung);

        return userIds
            .Select(userId => new DashboardOverloadedEmployeeItemViewModel
            {
                TenNhanVien = names.GetValueOrDefault(userId, $"Nhân sự {userId}"),
                SoCongViec = workRows.FirstOrDefault(x => x.MaNguoiDung == userId)?.SoCongViec ?? 0,
                SoChiTietCongViec = detailRows.FirstOrDefault(x => x.MaNguoiDung == userId)?.SoChiTietCongViec ?? 0,
                CongViecTre = workRows.FirstOrDefault(x => x.MaNguoiDung == userId)?.CongViecTre ?? 0
            })
            .Where(x => x.SoCongViec > 5 || x.SoChiTietCongViec > 8 || x.CongViecTre > 0)
            .OrderByDescending(x => x.SoCongViec + x.SoChiTietCongViec)
            .ThenByDescending(x => x.CongViecTre)
            .Take(5)
            .ToList();
    }

    private async Task<List<DashboardDelayedTaskItemViewModel>> LayTopCongViecTreAsync(List<int> maDuAnDaLoc)
    {
        var completionStatuses = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
        var today = DateTime.Today;
        var rows = await (
            from cv in _db.CongViec.AsNoTracking()
            join dm in _db.DanhMucCongViec.AsNoTracking() on cv.MaDanhMucCV equals dm.MaDanhMucCV
            join da in _db.DuAn.AsNoTracking() on dm.MaDuAn equals da.MaDuAn
            where cv.IsDeleted != true
                  && dm.IsDeleted != true
                  && da.IsDeleted != true
                  && maDuAnDaLoc.Contains(dm.MaDuAn)
                  && cv.NgayKetThucCVDuKien.HasValue
                  && cv.NgayKetThucCVDuKien.Value < today
                  && !completionStatuses.Contains(cv.TrangThaiCongViec ?? string.Empty)
            select new
            {
                cv.MaCongViec,
                cv.TenCongViec,
                da.TenDuAn,
                cv.NgayKetThucCVDuKien
            })
            .OrderBy(x => x.NgayKetThucCVDuKien)
            .Take(5)
            .ToListAsync();

        var workIds = rows.Select(x => x.MaCongViec).ToList();
        var assignees = await (
            from pc in _db.PhanCongCongViec.AsNoTracking()
            join nd in _db.NguoiDung.AsNoTracking() on pc.MaNguoiDung equals nd.MaNguoiDung
            where workIds.Contains(pc.MaCongViec)
                  && nd.IsDeleted != true
            select new
            {
                pc.MaCongViec,
                nd.MaNguoiDung,
                nd.HoTenNguoiDung
            })
            .ToListAsync();
        var assigneeNames = assignees
            .Select(x => new
            {
                x.MaCongViec,
                TenNhanVien = string.IsNullOrWhiteSpace(x.HoTenNguoiDung) ? $"Nhân sự {x.MaNguoiDung}" : x.HoTenNguoiDung
            })
            .ToList();

        return rows
            .Select(x => new DashboardDelayedTaskItemViewModel
            {
                TenCongViec = x.TenCongViec ?? "Công việc chưa đặt tên",
                TenDuAn = x.TenDuAn ?? "Dự án chưa đặt tên",
                NguoiPhuTrach = string.Join(", ", assigneeNames.Where(a => a.MaCongViec == x.MaCongViec).Select(a => a.TenNhanVien).Distinct().Take(3)),
                SoNgayTre = Math.Max(0, (today - x.NgayKetThucCVDuKien!.Value.Date).Days)
            })
            .Select(x =>
            {
                if (string.IsNullOrWhiteSpace(x.NguoiPhuTrach))
                {
                    x.NguoiPhuTrach = "Chưa phân công";
                }
                return x;
            })
            .OrderByDescending(x => x.SoNgayTre)
            .Take(5)
            .ToList();
    }

    private async Task<List<DashboardTimelineItemViewModel>> LayThongKeTheoThangAsync(IQueryable<DuAn> duAnQuery, List<int> maDuAnDaLoc)
    {
        var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var startMonth = currentMonth.AddMonths(-11);

        var duAnRows = await duAnQuery
            .Where(x => (x.NgayTaoDuAn.HasValue && x.NgayTaoDuAn.Value >= startMonth)
                        || (x.NgayHoanThanhThucTeDuAn.HasValue && x.NgayHoanThanhThucTeDuAn.Value >= startMonth))
            .Select(x => new
            {
                x.NgayTaoDuAn,
                x.NgayHoanThanhThucTeDuAn,
                x.TrangThaiDuAn
            })
            .ToListAsync();

        var congViecRows = await (
            from cv in _db.CongViec.AsNoTracking()
            join dm in _db.DanhMucCongViec.AsNoTracking() on cv.MaDanhMucCV equals dm.MaDanhMucCV
            where cv.IsDeleted != true
                  && dm.IsDeleted != true
                  && maDuAnDaLoc.Contains(dm.MaDuAn)
                  && ((cv.NgayKetThucCVThucTe.HasValue && cv.NgayKetThucCVThucTe.Value >= startMonth)
                      || (cv.NgayKetThucCVDuKien.HasValue && cv.NgayKetThucCVDuKien.Value >= startMonth))
            select new
            {
                cv.NgayKetThucCVThucTe,
                cv.NgayKetThucCVDuKien,
                cv.TrangThaiCongViec
            })
            .ToListAsync();

        var chiPhiRows = await (
            from cp in _db.ChiPhi.AsNoTracking()
            join ns in _db.NganSach.AsNoTracking() on cp.MaNganSach equals ns.MaNganSach
            where cp.IsDeleted != true
                  && ns.IsDeleted != true
                  && maDuAnDaLoc.Contains(ns.MaDuAn)
                  && cp.NgayChi.HasValue
                  && cp.NgayChi.Value >= startMonth
            select new
            {
                cp.NgayChi,
                SoTien = cp.SoTienDaChi ?? 0
            })
            .ToListAsync();

        var result = new List<DashboardTimelineItemViewModel>();
        for (var i = 0; i < 12; i++)
        {
            var month = startMonth.AddMonths(i);
            var nextMonth = month.AddMonths(1);
            result.Add(new DashboardTimelineItemViewModel
            {
                Label = month.ToString("MM/yyyy"),
                DuAnTaoMoi = duAnRows.Count(x => x.NgayTaoDuAn.HasValue && x.NgayTaoDuAn.Value >= month && x.NgayTaoDuAn.Value < nextMonth),
                DuAnHoanThanh = duAnRows.Count(x =>
                    TrangThai.LaHoanThanhCongViec(x.TrangThaiDuAn)
                    && x.NgayHoanThanhThucTeDuAn.HasValue
                    && x.NgayHoanThanhThucTeDuAn.Value >= month
                    && x.NgayHoanThanhThucTeDuAn.Value < nextMonth),
                CongViecHoanThanh = congViecRows.Count(x =>
                    TrangThai.LaHoanThanhCongViec(x.TrangThaiCongViec)
                    && x.NgayKetThucCVThucTe.HasValue
                    && x.NgayKetThucCVThucTe.Value >= month
                    && x.NgayKetThucCVThucTe.Value < nextMonth),
                CongViecTre = congViecRows.Count(x =>
                    x.NgayKetThucCVDuKien.HasValue
                    && x.NgayKetThucCVDuKien.Value >= month
                    && x.NgayKetThucCVDuKien.Value < nextMonth
                    && (!TrangThai.LaHoanThanhCongViec(x.TrangThaiCongViec)
                        || (x.NgayKetThucCVThucTe.HasValue && x.NgayKetThucCVThucTe.Value.Date > x.NgayKetThucCVDuKien.Value.Date))),
                ChiPhi = chiPhiRows
                    .Where(x => x.NgayChi.HasValue && x.NgayChi.Value >= month && x.NgayChi.Value < nextMonth)
                    .Sum(x => x.SoTien)
            });
        }

        return result;
    }

    private static (DateTime? TuNgay, DateTime? DenNgay, string? LocNhanh) ChuanHoaKhoangThoiGian(
        DateTime? tuNgay,
        DateTime? denNgay,
        string? locNhanh)
    {
        var homNay = DateTime.Today;
        var nhanh = string.IsNullOrWhiteSpace(locNhanh) ? null : locNhanh.Trim().ToLowerInvariant();
        DateTime? tu = tuNgay?.Date;
        DateTime? den = denNgay?.Date;

        if (!string.IsNullOrWhiteSpace(nhanh))
        {
            switch (nhanh)
            {
                case "homnay":
                    tu = homNay;
                    den = homNay;
                    break;
                case "7ngay":
                    tu = homNay.AddDays(-6);
                    den = homNay;
                    break;
                case "thangnay":
                    tu = new DateTime(homNay.Year, homNay.Month, 1);
                    den = tu.Value.AddMonths(1).AddDays(-1);
                    break;
                case "quynay":
                    var quarter = ((homNay.Month - 1) / 3) + 1;
                    var quarterStartMonth = ((quarter - 1) * 3) + 1;
                    tu = new DateTime(homNay.Year, quarterStartMonth, 1);
                    den = tu.Value.AddMonths(3).AddDays(-1);
                    break;
                case "namnay":
                    tu = new DateTime(homNay.Year, 1, 1);
                    den = new DateTime(homNay.Year, 12, 31);
                    break;
                case "tatca":
                    tu = null;
                    den = null;
                    break;
                default:
                    nhanh = null;
                    break;
            }
        }

        if (tu.HasValue && den.HasValue && tu.Value > den.Value)
        {
            (tu, den) = (den, tu);
        }

        if (string.IsNullOrWhiteSpace(nhanh) && !tu.HasValue && !den.HasValue)
        {
            nhanh = "tatca";
        }

        return (tu, den, nhanh);
    }

    private sealed record DashboardScope(bool LaAdmin, List<int> MaDuAnDuocXem);
    private sealed record DashboardFilterOptions(
        List<DashboardFilterOptionViewModel> DuAnOptions,
        List<DashboardFilterOptionViewModel> QuanLyOptions,
        List<DashboardFilterOptionViewModel> TeamOptions,
        List<DashboardFilterOptionViewModel> LoaiDuAnOptions);
    private sealed record DashboardCompletionProjectRow(int MaDuAn, DateTime? NgayKetThucDuAn, DateTime? NgayHoanThanhThucTe);
}
