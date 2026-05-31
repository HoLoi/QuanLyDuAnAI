using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Dashboard;

namespace QuanLyDuAn.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly QuanLyDuAnDbContext _db;

    public DashboardService(QuanLyDuAnDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(DateTime? tuNgay, DateTime? denNgay, string? locNhanh)
    {
        var (tuNgayLoc, denNgayLoc, locNhanhResolved) = ChuanHoaKhoangThoiGian(tuNgay, denNgay, locNhanh);
        var denNgayDocQuyen = denNgayLoc?.Date.AddDays(1);

        IQueryable<DuAn> duAnQuery = _db.DuAn.Where(x => x.IsDeleted != true);
        IQueryable<CongViec> congViecQuery = _db.CongViec.Where(x => x.IsDeleted != true);
        IQueryable<DeXuatCongViec> deXuatCongViecQuery = _db.DeXuatCongViec.Where(x => x.IsDeleted != true);
        IQueryable<DeXuatNganSach> deXuatNganSachQuery = _db.DeXuatNganSach.Where(x => x.IsDeleted != true);

        if (tuNgayLoc.HasValue)
        {
            duAnQuery = duAnQuery.Where(x => x.NgayTaoDuAn.HasValue && x.NgayTaoDuAn.Value >= tuNgayLoc.Value);
            congViecQuery = congViecQuery.Where(x => x.NgayTaoCongViec.HasValue && x.NgayTaoCongViec.Value >= tuNgayLoc.Value);
            deXuatCongViecQuery = deXuatCongViecQuery.Where(x => x.NgayDeXuatCongViec.HasValue && x.NgayDeXuatCongViec.Value >= tuNgayLoc.Value);
            deXuatNganSachQuery = deXuatNganSachQuery.Where(x => x.NgayDeXuat.HasValue && x.NgayDeXuat.Value >= tuNgayLoc.Value);
        }

        if (denNgayDocQuyen.HasValue)
        {
            duAnQuery = duAnQuery.Where(x => x.NgayTaoDuAn.HasValue && x.NgayTaoDuAn.Value < denNgayDocQuyen.Value);
            congViecQuery = congViecQuery.Where(x => x.NgayTaoCongViec.HasValue && x.NgayTaoCongViec.Value < denNgayDocQuyen.Value);
            deXuatCongViecQuery = deXuatCongViecQuery.Where(x => x.NgayDeXuatCongViec.HasValue && x.NgayDeXuatCongViec.Value < denNgayDocQuyen.Value);
            deXuatNganSachQuery = deXuatNganSachQuery.Where(x => x.NgayDeXuat.HasValue && x.NgayDeXuat.Value < denNgayDocQuyen.Value);
        }

        var projectsForChart = await duAnQuery
            .OrderByDescending(x => x.MaDuAn)
            .Take(12)
            .ToListAsync();

        var totalProjects = await duAnQuery.CountAsync();
        var totalTasks = await congViecQuery.CountAsync();
        var totalEmployees = await _db.NguoiDung.CountAsync(x => x.IsDeleted != true);

        var nganSachQuery = _db.NganSach
            .Where(x => x.IsDeleted != true && x.IsActive == true);
        if (tuNgayLoc.HasValue)
        {
            nganSachQuery = nganSachQuery.Where(x => x.NgayCapNhatNganSach.HasValue && x.NgayCapNhatNganSach.Value >= tuNgayLoc.Value);
        }
        if (denNgayDocQuyen.HasValue)
        {
            nganSachQuery = nganSachQuery.Where(x => x.NgayCapNhatNganSach.HasValue && x.NgayCapNhatNganSach.Value < denNgayDocQuyen.Value);
        }

        var totalBudget = await nganSachQuery.SumAsync(x => x.SoTienNganSach ?? 0);

        var expenseByProjectQuery =
            from cp in _db.ChiPhi
            join ns in _db.NganSach on cp.MaNganSach equals ns.MaNganSach
            where cp.IsDeleted != true && ns.IsDeleted != true
            select new { cp, ns.MaDuAn };

        if (tuNgayLoc.HasValue)
        {
            expenseByProjectQuery = expenseByProjectQuery.Where(x => x.cp.NgayChi.HasValue && x.cp.NgayChi.Value >= tuNgayLoc.Value);
        }
        if (denNgayDocQuyen.HasValue)
        {
            expenseByProjectQuery = expenseByProjectQuery.Where(x => x.cp.NgayChi.HasValue && x.cp.NgayChi.Value < denNgayDocQuyen.Value);
        }

        var expenseByProject = await expenseByProjectQuery
            .GroupBy(x => x.MaDuAn)
            .Select(g => new { ProjectId = g.Key, Total = g.Sum(x => x.cp.SoTienDaChi ?? 0) })
            .ToListAsync();

        var expenseMap = expenseByProject.ToDictionary(x => x.ProjectId, x => x.Total);
        var totalExpense = expenseByProject.Sum(x => x.Total);

        var duAnStatuses = await duAnQuery
            .Select(x => x.TrangThaiDuAn)
            .ToListAsync();

        var deXuatCongViecStatuses = await deXuatCongViecQuery
            .Select(x => x.TrangThaiCongViecDeXuat)
            .ToListAsync();

        var deXuatNganSachStatuses = await deXuatNganSachQuery
            .Select(x => x.TrangThaiDeXuat)
            .ToListAsync();

        var yeuCauDoiQuanLyStatuses = await _db.YeuCauDoiQuanLy
            .Where(x => x.IsDeleted != true)
            .Select(x => x.TrangThaiYeuCauDoiQuanLy)
            .ToListAsync();

        var trangThaiHoanThanhDuAn = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
        var trangThaiDuAnDaKetThuc = trangThaiHoanThanhDuAn
            .Concat(TrangThai.GetCommonStatusVariants(TrangThai.LuuTru))
            .ToArray();
        var duAnHoanThanhRows = await _db.DuAn
            .Where(x => x.IsDeleted != true && trangThaiDuAnDaKetThuc.Contains(x.TrangThaiDuAn ?? string.Empty))
            .Select(x => new
            {
                x.MaDuAn,
                x.NgayKetThucDuAn,
                x.NgayHoanThanhThucTeDuAn
            })
            .ToListAsync();
        var duAnHoanThanhIds = duAnHoanThanhRows.Select(x => x.MaDuAn).ToHashSet();
        var ngayHoanThanhFallbackTheoDuAn = await (
            from cv in _db.CongViec
            join dm in _db.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
            where cv.IsDeleted != true
                  && dm.IsDeleted != true
                  && cv.NgayKetThucCVThucTe.HasValue
                  && duAnHoanThanhIds.Contains(dm.MaDuAn)
            group cv by dm.MaDuAn
            into g
            select new { MaDuAn = g.Key, NgayHoanThanh = g.Max(x => x.NgayKetThucCVThucTe) }
        ).ToDictionaryAsync(x => x.MaDuAn, x => x.NgayHoanThanh);
        var duAnHoanThanhTimelineRows = duAnHoanThanhRows
            .Select(x => new
            {
                x.MaDuAn,
                x.NgayKetThucDuAn,
                NgayHoanThanhThucTe = x.NgayHoanThanhThucTeDuAn
                    ?? ngayHoanThanhFallbackTheoDuAn.GetValueOrDefault(x.MaDuAn)
            })
            .ToList();
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

        var congViecTreHanQuery = _db.CongViec.Where(x =>
            x.IsDeleted != true
            && x.NgayKetThucCVDuKien.HasValue
            && x.NgayKetThucCVDuKien.Value < DateTime.Now
            && x.TrangThaiCongViec != TrangThai.HoanThanh
            && x.TrangThaiCongViec != TrangThai.HoanThanhHienThi
            && x.TrangThaiCongViec != TrangThai.Done
            && x.TrangThaiCongViec != TrangThai.Completed);
        if (tuNgayLoc.HasValue)
        {
            congViecTreHanQuery = congViecTreHanQuery.Where(x => x.NgayTaoCongViec.HasValue && x.NgayTaoCongViec.Value >= tuNgayLoc.Value);
        }
        if (denNgayDocQuyen.HasValue)
        {
            congViecTreHanQuery = congViecTreHanQuery.Where(x => x.NgayTaoCongViec.HasValue && x.NgayTaoCongViec.Value < denNgayDocQuyen.Value);
        }

        var nhanSuQuaTaiQuery =
            from pc in _db.PhanCongCongViec
            join cv in _db.CongViec on pc.MaCongViec equals cv.MaCongViec
            where cv.IsDeleted != true
                  && cv.TrangThaiCongViec != TrangThai.HoanThanh
                  && cv.TrangThaiCongViec != TrangThai.HoanThanhHienThi
                  && cv.TrangThaiCongViec != TrangThai.Done
                  && cv.TrangThaiCongViec != TrangThai.Completed
            select new { pc.MaNguoiDung, cv.NgayTaoCongViec };
        if (tuNgayLoc.HasValue)
        {
            nhanSuQuaTaiQuery = nhanSuQuaTaiQuery.Where(x => x.NgayTaoCongViec.HasValue && x.NgayTaoCongViec.Value >= tuNgayLoc.Value);
        }
        if (denNgayDocQuyen.HasValue)
        {
            nhanSuQuaTaiQuery = nhanSuQuaTaiQuery.Where(x => x.NgayTaoCongViec.HasValue && x.NgayTaoCongViec.Value < denNgayDocQuyen.Value);
        }

        int CountDuAnByStatus(string expected)
            => duAnStatuses.Count(x => TrangThai.EqualsValue(x, expected));

        return new DashboardViewModel
        {
            TuNgay = tuNgayLoc,
            DenNgay = denNgayLoc,
            LocNhanh = locNhanhResolved,
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

            DuAnDungTienDo = duAnStatuses.Count(x => !TrangThai.EqualsValue(x, TrangThai.TreTienDo)),
            DuAnTreTienDo = duAnStatuses.Count(x => TrangThai.EqualsValue(x, TrangThai.TreTienDo)),

            CongViecTreHan = await congViecTreHanQuery.CountAsync(),

            NhanSuQuaTai = await nhanSuQuaTaiQuery
                .GroupBy(x => x.MaNguoiDung)
                .Where(g => g.Count() > 5)
                .CountAsync(),

            DuAnVuotNganSach = await (from da in _db.DuAn
                                      where da.IsDeleted != true
                                      let nganSach = _db.NganSach
                                          .Where(n => n.MaDuAn == da.MaDuAn && n.IsDeleted != true && n.IsActive == true)
                                          .Sum(n => n.SoTienNganSach ?? 0)
                                      let chiPhi = (from cp in _db.ChiPhi
                                                    join ns in _db.NganSach on cp.MaNganSach equals ns.MaNganSach
                                                    where cp.IsDeleted != true
                                                          && ns.IsDeleted != true
                                                          && ns.MaDuAn == da.MaDuAn
                                                    select cp.SoTienDaChi ?? 0).Sum()
                                      where chiPhi > nganSach && nganSach > 0
                                      select da.MaDuAn).CountAsync(),

            DuAnThieuDatasetAi = await (from da in _db.DuAn
                                        where da.IsDeleted != true
                                        join ds in _db.AiDataset on da.MaDuAn equals ds.MaDuAn into dsJoin
                                        from ds in dsJoin.DefaultIfEmpty()
                                        where ds == null
                                        select da.MaDuAn).CountAsync(),

            DeXuatCongViecChoDuyet = deXuatCongViecStatuses.Count(x => TrangThai.EqualsValue(x, TrangThai.ChoDuyet)),
            DeXuatNganSachChoDuyet = deXuatNganSachStatuses.Count(x => TrangThai.EqualsValue(x, TrangThai.ChoDuyet)),
            YeuCauDoiQuanLyChoDuyet = yeuCauDoiQuanLyStatuses.Count(x => TrangThai.EqualsValue(x, TrangThai.ChoDuyet)),

            WorkflowHealthItems = [],
            Suggestions = []
        };
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
}
