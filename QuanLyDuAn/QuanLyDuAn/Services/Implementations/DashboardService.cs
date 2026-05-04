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
        var projects = await _db.DuAn
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

        return new DashboardViewModel
        {
            TongDuAn = totalProjects,
            TongCongViec = totalTasks,
            TongNhanVien = totalEmployees,
            TongNganSach = totalBudget,
            TenDuAn = projects.Select(x => x.TenDuAn ?? $"Du an #{x.MaDuAn}").ToList(),
            PhanTramTienDo = projects.Select(x => x.PhanTramHoanThanh ?? 0).ToList(),
            ChiPhiTheoDuAn = projects.Select(x => expenseMap.TryGetValue(x.MaDuAn, out var value) ? value : 0).ToList(),
            DuAnDungTienDo = projects.Count(x =>
                x.TrangThaiDuAn != null && !x.TrangThaiDuAn.Contains(TrangThai.TreTienDo, StringComparison.OrdinalIgnoreCase)),
            DuAnTreTienDo = projects.Count(x =>
                x.TrangThaiDuAn != null && x.TrangThaiDuAn.Contains(TrangThai.TreTienDo, StringComparison.OrdinalIgnoreCase)),
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
            WorkflowHealthItems = [],
            Suggestions = []
        };
    }
}
