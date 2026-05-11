using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
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

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var projectsForChart = await _db.DuAn
            .Where(x => x.IsDeleted != true)
            .OrderByDescending(x => x.MaDuAn)
            .Take(12)
            .ToListAsync();

        var totalProjects = await _db.DuAn.CountAsync(x => x.IsDeleted != true);
        var totalTasks = await _db.CongViec.CountAsync(x => x.IsDeleted != true);
        var totalEmployees = await _db.NguoiDung.CountAsync(x => x.IsDeleted != true);

        var totalBudget = await _db.NganSach
            .Where(x => x.IsDeleted != true && x.IsActive == true)
            .SumAsync(x => x.SoTienNganSach ?? 0);

        var expenseByProject = await (
            from cp in _db.ChiPhi
            join ns in _db.NganSach on cp.MaNganSach equals ns.MaNganSach
            where cp.IsDeleted != true && ns.IsDeleted != true
            group cp by ns.MaDuAn into g
            select new { ProjectId = g.Key, Total = g.Sum(x => x.SoTienDaChi ?? 0) }
        ).ToListAsync();

        var expenseMap = expenseByProject.ToDictionary(x => x.ProjectId, x => x.Total);
        var totalExpense = expenseByProject.Sum(x => x.Total);

        var duAnStatuses = await _db.DuAn
            .Where(x => x.IsDeleted != true)
            .Select(x => x.TrangThaiDuAn)
            .ToListAsync();

        var deXuatCongViecStatuses = await _db.DeXuatCongViec
            .Where(x => x.IsDeleted != true)
            .Select(x => x.TrangThaiCongViecDeXuat)
            .ToListAsync();

        var deXuatNganSachStatuses = await _db.DeXuatNganSach
            .Where(x => x.IsDeleted != true)
            .Select(x => x.TrangThaiDeXuat)
            .ToListAsync();

        var yeuCauDoiQuanLyStatuses = await _db.YeuCauDoiQuanLy
            .Where(x => x.IsDeleted != true)
            .Select(x => x.TrangThaiYeuCauDoiQuanLy)
            .ToListAsync();

        int CountDuAnByStatus(string expected)
            => duAnStatuses.Count(x => TrangThai.EqualsValue(x, expected));

        return new DashboardViewModel
        {
            TongDuAn = totalProjects,
            TongCongViec = totalTasks,
            TongNhanVien = totalEmployees,
            TongNganSach = totalBudget,
            TongChiPhi = totalExpense,
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

            DuAnDungTienDo = duAnStatuses.Count(x => !TrangThai.EqualsValue(x, TrangThai.TreTienDo)),
            DuAnTreTienDo = duAnStatuses.Count(x => TrangThai.EqualsValue(x, TrangThai.TreTienDo)),

            CongViecTreHan = await _db.CongViec.CountAsync(x =>
                x.IsDeleted != true
                && x.NgayKetThucCVDuKien.HasValue
                && x.NgayKetThucCVDuKien.Value < DateTime.Now
                && x.TrangThaiCongViec != TrangThai.HoanThanh
                && x.TrangThaiCongViec != TrangThai.HoanThanhHienThi
                && x.TrangThaiCongViec != TrangThai.Done
                && x.TrangThaiCongViec != TrangThai.Completed),

            NhanSuQuaTai = await (from pc in _db.PhanCongCongViec
                                  join cv in _db.CongViec on pc.MaCongViec equals cv.MaCongViec
                                  where cv.IsDeleted != true
                                        && cv.TrangThaiCongViec != TrangThai.HoanThanh
                                        && cv.TrangThaiCongViec != TrangThai.HoanThanhHienThi
                                        && cv.TrangThaiCongViec != TrangThai.Done
                                        && cv.TrangThaiCongViec != TrangThai.Completed
                                  group pc by pc.MaNguoiDung into g
                                  where g.Count() > 5
                                  select g.Key).CountAsync(),

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
}
