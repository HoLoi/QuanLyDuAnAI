using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.TienDoCongViec;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class TienDoCongViecService : ITienDoCongViecService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;
        private readonly ITrangThaiWorkflowService _trangThaiWorkflowService;
        private static readonly HashSet<string> AllowedEvidenceExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg", ".zip"
        };
        private const long MaxEvidenceFileSize = 10 * 1024 * 1024;

        public TienDoCongViecService(
            QuanLyDuAnDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment environment,
            ITrangThaiWorkflowService trangThaiWorkflowService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
            _trangThaiWorkflowService = trangThaiWorkflowService;
        }

        public async Task<TienDoCongViecPageViewModel> GetPageAsync(
            int? locMaDuAn,
            int? locMaCongViec,
            int? locMaChiTietCv,
            string? tuKhoa,
            DateTime? tuNgayBaoCao,
            DateTime? denNgayBaoCao,
            int pageNumber = 1,
            int pageSize = 20,
            bool paginate = true)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            var (tuNgayLoc, denNgayLoc) = ChuanHoaKhoangNgay(tuNgayBaoCao, denNgayBaoCao);
            var denNgayDocQuyen = denNgayLoc?.Date.AddDays(1);
            var accessibleDetailIds = roleFlags.IsAdmin
                ? new List<int>()
                : await GetAccessibleChiTietCongViecIdsAsync(currentUserId, roleFlags);
            var reviewProjectIds = await GetProjectIdsForReviewAsync(currentUserId, roleFlags);

            var baseQuery =
                from ct in _context.CtCongViec
                join cv in _context.CongViec on ct.MaCongViec equals cv.MaCongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                join da in _context.DuAn on dm.MaDuAn equals da.MaDuAn
                where ct.IsDeleted != true
                      && cv.IsDeleted != true
                      && dm.IsDeleted != true
                      && da.IsDeleted != true
                      && (roleFlags.IsAdmin || accessibleDetailIds.Contains(ct.MaChiTietCV))
                select new
                {
                    ct.MaChiTietCV,
                    ct.MaCongViec,
                    dm.MaDuAn,
                    TenDuAn = da.TenDuAn,
                    TenCongViec = cv.TenCongViec,
                    TenChiTietCongViec = ct.TenCTCV,
                    ct.NoiDungChiTietCV,
                    ct.TrangThaiCTCV,
                    TrangThaiCongViec = cv.TrangThaiCongViec,
                    TrangThaiDuAn = da.TrangThaiDuAn
                };

            if (locMaDuAn.HasValue)
                baseQuery = baseQuery.Where(x => x.MaDuAn == locMaDuAn.Value);

            if (locMaCongViec.HasValue)
                baseQuery = baseQuery.Where(x => x.MaCongViec == locMaCongViec.Value);

            if (locMaChiTietCv.HasValue)
                baseQuery = baseQuery.Where(x => x.MaChiTietCV == locMaChiTietCv.Value);

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                baseQuery = baseQuery.Where(x =>
                    (x.TenDuAn ?? string.Empty).ToLower().Contains(keyword)
                    || (x.TenCongViec ?? string.Empty).ToLower().Contains(keyword)
                    || (x.TenChiTietCongViec ?? string.Empty).ToLower().Contains(keyword)
                    || (x.NoiDungChiTietCV ?? string.Empty).ToLower().Contains(keyword));
            }

            if (tuNgayLoc.HasValue || denNgayDocQuyen.HasValue)
            {
                baseQuery = baseQuery.Where(x => _context.TienDoCongViec.Any(td =>
                    td.MaChiTietCV == x.MaChiTietCV &&
                    td.ThoiGianCapNhat.HasValue &&
                    (!tuNgayLoc.HasValue || td.ThoiGianCapNhat.Value >= tuNgayLoc.Value) &&
                    (!denNgayDocQuyen.HasValue || td.ThoiGianCapNhat.Value < denNgayDocQuyen.Value)));
            }

            var totalItems = await baseQuery.CountAsync();
            var pagination = PaginationViewModel.Create(pageNumber, pageSize, totalItems);

            var dsMaDuAn = await baseQuery
                .Select(x => x.MaDuAn)
                .Distinct()
                .ToListAsync();
            var dsMaCongViec = await baseQuery
                .Select(x => x.MaCongViec)
                .Distinct()
                .ToListAsync();

            var orderedDetailQuery = baseQuery
                .OrderByDescending(x => x.MaChiTietCV)
                .AsQueryable();

            if (paginate)
            {
                orderedDetailQuery = orderedDetailQuery
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize);
            }

            var detailRows = await orderedDetailQuery.ToListAsync();

            var detailIds = detailRows.Select(x => x.MaChiTietCV).Distinct().ToList();

            var progressQuery = _context.TienDoCongViec
                .Where(x => detailIds.Contains(x.MaChiTietCV));
            if (tuNgayLoc.HasValue)
            {
                progressQuery = progressQuery.Where(x => x.ThoiGianCapNhat.HasValue && x.ThoiGianCapNhat.Value >= tuNgayLoc.Value);
            }
            if (denNgayDocQuyen.HasValue)
            {
                progressQuery = progressQuery.Where(x => x.ThoiGianCapNhat.HasValue && x.ThoiGianCapNhat.Value < denNgayDocQuyen.Value);
            }

            var progressRows = await progressQuery
                .OrderByDescending(x => x.ThoiGianCapNhat ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaTienDo)
                .ToListAsync();

            var progressByDetail = progressRows
                .GroupBy(x => x.MaChiTietCV)
                .ToDictionary(x => x.Key, x => x.ToList());

            var detailById = detailRows.ToDictionary(x => x.MaChiTietCV, x => x);

            var latestProgressByDetail = progressByDetail
                .ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());

            var latestApprovedByDetail = progressByDetail
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.FirstOrDefault(i => TrangThai.EqualsValue(i.TrangThaiTienDo, TrangThai.DaDuyet)));

            var detailIdsDangChoDuyet = progressByDetail
                .Where(x => x.Value.Any(i => TrangThai.EqualsValue(i.TrangThaiTienDo, TrangThai.ChoDuyet)))
                .Select(x => x.Key)
                .ToHashSet();

            var progressIds = progressRows
                .Select(x => x.MaTienDo)
                .Distinct()
                .ToList();

            var fileRows = await _context.FileTienDoCongViec
                .Where(x => progressIds.Contains(x.MaTienDo) && x.IsDeleted != true)
                .OrderByDescending(x => x.NgayUploadFileTDCV ?? DateTime.MinValue)
                .ToListAsync();

            var reporterIds = progressRows
                .Select(x => x.MaNguoiDung)
                .Distinct()
                .ToList();

            var approverIds = progressRows
                .Where(x => x.MaNguoiDungDuyet.HasValue)
                .Select(x => x.MaNguoiDungDuyet!.Value)
                .Distinct()
                .ToList();

            var userIds = reporterIds.Concat(approverIds).Distinct().ToList();
            var userNameRows = await _context.NguoiDung
                .Where(x => userIds.Contains(x.MaNguoiDung) && x.IsDeleted != true)
                .Select(x => new { x.MaNguoiDung, x.HoTenNguoiDung })
                .ToListAsync();

            var assignedUserNames = await (
                from pc in _context.PhanCongCtCongViec
                join nd in _context.NguoiDung on pc.MaNguoiDung equals nd.MaNguoiDung
                where detailIds.Contains(pc.MaChiTietCV) && nd.IsDeleted != true
                select new { pc.MaChiTietCV, nd.HoTenNguoiDung, nd.MaNguoiDung }
            ).ToListAsync();

            var assignedSelfIds = await _context.PhanCongCtCongViec
                .Where(x => x.MaNguoiDung == currentUserId && detailIds.Contains(x.MaChiTietCV))
                .Select(x => x.MaChiTietCV)
                .ToListAsync();
            var assignedSelfSet = assignedSelfIds.ToHashSet();

            var userNameById = userNameRows.ToDictionary(
                x => x.MaNguoiDung,
                x => string.IsNullOrWhiteSpace(x.HoTenNguoiDung) ? $"Nhân viên {x.MaNguoiDung}" : x.HoTenNguoiDung!.Trim());

            var fileByProgress = fileRows
                .GroupBy(x => x.MaTienDo)
                .ToDictionary(x => x.Key, x => x.ToList());

            var assignedNameByDetail = assignedUserNames
                .GroupBy(x => x.MaChiTietCV)
                .ToDictionary(
                    x => x.Key,
                    x => string.Join(", ",
                        x.Select(i => string.IsNullOrWhiteSpace(i.HoTenNguoiDung)
                            ? $"Nhân viên {i.MaNguoiDung}"
                            : i.HoTenNguoiDung)
                        .Distinct()));

            var lichSuByDetail = new Dictionary<int, List<TienDoCongViecLichSuItemViewModel>>();
            foreach (var detailId in detailIds)
            {
                if (!progressByDetail.TryGetValue(detailId, out var detailProgress))
                {
                    lichSuByDetail[detailId] = new List<TienDoCongViecLichSuItemViewModel>();
                    continue;
                }

                var row = detailById[detailId];
                var detailBiKhoa = BiKhoaCapNhatTheoTrangThai(row.TrangThaiDuAn, row.TrangThaiCongViec, row.TrangThaiCTCV);
                var coTheDuyetTheoScope = reviewProjectIds.Contains(row.MaDuAn);

                lichSuByDetail[detailId] = detailProgress
                    .OrderByDescending(i => i.ThoiGianCapNhat ?? DateTime.MinValue)
                    .ThenByDescending(i => i.MaTienDo)
                    .Select(i =>
                    {
                        var trangThaiDeXuat = TrangThai.ToCode(string.IsNullOrWhiteSpace(i.TrangThaiCTCVDeXuat)
                            ? i.TrangThaiTienDo
                            : i.TrangThaiCTCVDeXuat);
                        var trangThaiDuyet = TrangThai.ToCode(i.TrangThaiTienDo);

                        return new TienDoCongViecLichSuItemViewModel
                        {
                            MaTienDo = i.MaTienDo,
                            MaChiTietCV = i.MaChiTietCV,
                            MaNguoiDung = i.MaNguoiDung,
                            TenNguoiBaoCao = userNameById.TryGetValue(i.MaNguoiDung, out var tenNguoiBaoCao)
                                ? tenNguoiBaoCao
                                : $"Nhân viên {i.MaNguoiDung}",
                            PhanTram = Math.Clamp(i.PhanTram ?? 0, 0, 100),
                            TrangThaiCTCVDeXuatCode = trangThaiDeXuat,
                            TrangThaiCTCVDeXuat = TrangThai.ToDisplay(trangThaiDeXuat),
                            TrangThaiDuyetBaoCaoCode = trangThaiDuyet,
                            TrangThaiDuyetBaoCao = TrangThai.ToDisplay(trangThaiDuyet),
                            GhiChuTienDo = i.GhiChuTienDo,
                            ThoiGianCapNhat = i.ThoiGianCapNhat,
                            MaNguoiDungDuyet = i.MaNguoiDungDuyet,
                            TenNguoiDuyet = i.MaNguoiDungDuyet.HasValue && userNameById.TryGetValue(i.MaNguoiDungDuyet.Value, out var tenNguoiDuyet)
                                ? tenNguoiDuyet
                                : null,
                            ThoiGianDuyet = i.ThoiGianDuyet,
                            GhiChuDuyet = i.GhiChuDuyet,
                            CoTheXuLyDuyet = !roleFlags.IsAdmin
                                && !detailBiKhoa
                                && coTheDuyetTheoScope
                                && i.MaNguoiDung != currentUserId
                                && TrangThai.EqualsValue(trangThaiDuyet, TrangThai.ChoDuyet),
                            DanhSachFile = fileByProgress.TryGetValue(i.MaTienDo, out var files)
                                ? files.Select(f => new TienDoCongViecFileItemViewModel
                                {
                                    MaFileTDCV = f.MaFileTDCV,
                                    MaTienDo = f.MaTienDo,
                                    TenFileTDCV = f.TenFileTDCV ?? $"Tệp {f.MaFileTDCV}",
                                    NgayUploadFileTDCV = f.NgayUploadFileTDCV,
                                    CoTheXoa = false
                                }).ToList()
                                : new List<TienDoCongViecFileItemViewModel>()
                        };
                    })
                    .ToList();
            }

            var danhSach = new List<TienDoCongViecItemViewModel>();
            foreach (var row in detailRows)
            {
                latestProgressByDetail.TryGetValue(row.MaChiTietCV, out var latest);
                latestApprovedByDetail.TryGetValue(row.MaChiTietCV, out var latestApproved);

                var trangThaiChiTietCode = TrangThai.ToCode(row.TrangThaiCTCV);
                var phanTram = TinhPhanTramTheoTrangThai(trangThaiChiTietCode);
                var biKhoa = BiKhoaCapNhatTheoTrangThai(row.TrangThaiDuAn, row.TrangThaiCongViec, row.TrangThaiCTCV);
                var dangCoBaoCaoChoDuyet = detailIdsDangChoDuyet.Contains(row.MaChiTietCV);
                var coTheCapNhatTienDo = !roleFlags.IsAdmin
                                         && assignedSelfSet.Contains(row.MaChiTietCV)
                                         && !biKhoa
                                         && !dangCoBaoCaoChoDuyet;
                var coTheDuyetBaoCao = !roleFlags.IsAdmin && reviewProjectIds.Contains(row.MaDuAn) && !biKhoa;

                var latestDeXuatCode = TrangThai.ToCode(string.IsNullOrWhiteSpace(latest?.TrangThaiCTCVDeXuat)
                    ? latest?.TrangThaiTienDo
                    : latest!.TrangThaiCTCVDeXuat);
                var latestDuyetCode = TrangThai.ToCode(latest?.TrangThaiTienDo);

                var item = new TienDoCongViecItemViewModel
                {
                    MaChiTietCV = row.MaChiTietCV,
                    MaCongViec = row.MaCongViec,
                    MaDuAn = row.MaDuAn,
                    MaTienDoMoiNhat = latest?.MaTienDo,
                    TenDuAn = row.TenDuAn ?? $"Dự án {row.MaDuAn}",
                    TenCongViec = row.TenCongViec ?? $"Công việc {row.MaCongViec}",
                    TenChiTietCongViec = !string.IsNullOrWhiteSpace(row.TenChiTietCongViec)
                        ? row.TenChiTietCongViec
                        : (row.NoiDungChiTietCV ?? $"Chi tiết {row.MaChiTietCV}"),
                    NguoiThucHien = assignedNameByDetail.TryGetValue(row.MaChiTietCV, out var names) ? names : "Chưa phân công",
                    PhanTramHienTai = Math.Clamp(phanTram, 0, 100),
                    TrangThaiCTCVCode = trangThaiChiTietCode,
                    TrangThaiCTCV = TrangThai.ToDisplay(row.TrangThaiCTCV),
                    ThoiGianBaoCaoGanNhat = latest?.ThoiGianCapNhat,
                    GhiChuBaoCaoGanNhat = latest?.GhiChuTienDo,
                    TenNguoiBaoCaoGanNhat = latest != null && userNameById.TryGetValue(latest.MaNguoiDung, out var tenNguoiBaoCao)
                        ? tenNguoiBaoCao
                        : null,
                    TrangThaiCTCVDeXuatGanNhatCode = string.IsNullOrWhiteSpace(latestDeXuatCode) ? null : latestDeXuatCode,
                    TrangThaiCTCVDeXuatGanNhat = string.IsNullOrWhiteSpace(latestDeXuatCode) ? null : TrangThai.ToDisplay(latestDeXuatCode),
                    TrangThaiDuyetBaoCaoGanNhatCode = string.IsNullOrWhiteSpace(latestDuyetCode) ? null : latestDuyetCode,
                    TrangThaiDuyetBaoCaoGanNhat = string.IsNullOrWhiteSpace(latestDuyetCode) ? null : TrangThai.ToDisplay(latestDuyetCode),
                    DangCoBaoCaoChoDuyet = dangCoBaoCaoChoDuyet,
                    CoTheCapNhatTienDo = coTheCapNhatTienDo,
                    CoTheDuyetBaoCao = coTheDuyetBaoCao,
                    TrangThaiDeXuatOptions = LayDanhSachTrangThaiDeXuatCoTheChon(
                        row.TrangThaiCTCV,
                        latestApproved?.TrangThaiCTCVDeXuat ?? latestApproved?.TrangThaiTienDo),
                    LichSuTienDo = lichSuByDetail.TryGetValue(row.MaChiTietCV, out var lichSu)
                        ? lichSu
                        : new List<TienDoCongViecLichSuItemViewModel>()
                };

                if (latest != null && fileByProgress.TryGetValue(latest.MaTienDo, out var files))
                {
                    item.DanhSachFile = files.Select(x => new TienDoCongViecFileItemViewModel
                    {
                        MaFileTDCV = x.MaFileTDCV,
                        MaTienDo = x.MaTienDo,
                        TenFileTDCV = x.TenFileTDCV ?? $"Tệp {x.MaFileTDCV}",
                        NgayUploadFileTDCV = x.NgayUploadFileTDCV,
                        CoTheXoa = false
                    }).ToList();
                }

                danhSach.Add(item);
            }

            var duAnOptions = await (
                from da in _context.DuAn
                where da.IsDeleted != true && dsMaDuAn.Contains(da.MaDuAn)
                orderby da.TenDuAn
                select new TienDoCongViecDuAnOptionViewModel
                {
                    MaDuAn = da.MaDuAn,
                    TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}"
                }).ToListAsync();

            var congViecOptions = await (
                from cv in _context.CongViec
                where cv.IsDeleted != true && dsMaCongViec.Contains(cv.MaCongViec)
                orderby cv.TenCongViec
                select new TienDoCongViecCongViecOptionViewModel
                {
                    MaCongViec = cv.MaCongViec,
                    TenCongViec = cv.TenCongViec ?? $"Công việc {cv.MaCongViec}"
                }).ToListAsync();

            return new TienDoCongViecPageViewModel
            {
                Filter = new TienDoCongViecFilterViewModel
                {
                    LocMaDuAn = locMaDuAn,
                    LocMaCongViec = locMaCongViec,
                    LocMaChiTietCv = locMaChiTietCv,
                    TuKhoa = tuKhoa,
                    TuNgayBaoCao = tuNgayLoc,
                    DenNgayBaoCao = denNgayLoc
                },
                DanhSach = danhSach,
                DanhSachDuAn = duAnOptions,
                DanhSachCongViec = congViecOptions,
                Pagination = pagination
            };
        }

        public async Task CapNhatTienDoAsync(TienDoCongViecCapNhatViewModel form)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminTacNghiep(roleFlags);

            if (form.MaChiTietCV <= 0)
                throw new Exception("Mã chi tiết công việc không hợp lệ.");

            if (string.IsNullOrWhiteSpace(form.TrangThaiCTCVMoi))
                throw new Exception("Vui lòng chọn trạng thái đề xuất.");

            if (!string.IsNullOrWhiteSpace(form.GhiChuTienDo) && form.GhiChuTienDo.Trim().Length > 255)
                throw new Exception("Ghi chú tiến độ tối đa 255 ký tự.");

            var contextInfo = await GetContextByChiTietCvAsync(form.MaChiTietCV);

            if (BiKhoaCapNhatTheoTrangThai(contextInfo.TrangThaiDuAn, contextInfo.TrangThaiCongViec, contextInfo.TrangThaiCTCV))
                throw new Exception("Trạng thái hiện tại không cho phép cập nhật tiến độ.");

            var duocPhanCong = await _context.PhanCongCtCongViec
                .AnyAsync(x => x.MaChiTietCV == form.MaChiTietCV && x.MaNguoiDung == currentUserId);

            if (!duocPhanCong)
                throw new Exception("Bạn không được phân công chi tiết công việc này.");

            var trangThaiChoDuyetVariants = TrangThai.GetCommonStatusVariants(TrangThai.ChoDuyet);
            var dangCoBaoCaoChoDuyet = await _context.TienDoCongViec
                .AnyAsync(x => x.MaChiTietCV == form.MaChiTietCV && trangThaiChoDuyetVariants.Contains(x.TrangThaiTienDo ?? string.Empty));
            if (dangCoBaoCaoChoDuyet)
                throw new Exception("Chi tiết công việc đang có báo cáo chờ duyệt, vui lòng chờ xử lý trước khi gửi báo cáo mới.");

            var trangThaiDaDuyetVariants = TrangThai.GetCommonStatusVariants(TrangThai.DaDuyet);
            var baoCaoDaDuyetMoiNhat = await _context.TienDoCongViec
                .Where(x => x.MaChiTietCV == form.MaChiTietCV && trangThaiDaDuyetVariants.Contains(x.TrangThaiTienDo ?? string.Empty))
                .OrderByDescending(x => x.ThoiGianCapNhat ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaTienDo)
                .FirstOrDefaultAsync();

            var trangThaiDeXuat = TrangThai.ToCode(form.TrangThaiCTCVMoi);
            KiemTraTrangThaiChiTietHopLe(trangThaiDeXuat);
            KiemTraKhongDuocLuiTrangThai(
                contextInfo.TrangThaiCTCV,
                trangThaiDeXuat,
                baoCaoDaDuyetMoiNhat?.TrangThaiCTCVDeXuat ?? baoCaoDaDuyetMoiNhat?.TrangThaiTienDo);

            var files = (form.Files ?? new List<IFormFile>())
                .Where(x => x != null)
                .ToList();
            KiemTraFileMinhChungHopLe(files, trangThaiDeXuat);
            KiemTraBaoCaoKhongRong(contextInfo.TrangThaiCTCV, trangThaiDeXuat, form.GhiChuTienDo, files.Count);

            var phanTram = TinhPhanTramTheoTrangThai(trangThaiDeXuat);
            var entity = new TienDoCongViec
            {
                MaChiTietCV = form.MaChiTietCV,
                MaNguoiDung = currentUserId,
                PhanTram = phanTram,
                GhiChuTienDo = string.IsNullOrWhiteSpace(form.GhiChuTienDo) ? null : form.GhiChuTienDo.Trim(),
                ThoiGianCapNhat = DateTime.Now,
                TrangThaiCTCVDeXuat = trangThaiDeXuat,
                TrangThaiTienDo = TrangThai.ChoDuyet
            };

            var savedPhysicalPaths = new List<string>();
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.TienDoCongViec.Add(entity);
                await _context.SaveChangesAsync();

                if (files.Count > 0)
                {
                    var createdPaths = await LuuFileMinhChungChoTienDoAsync(entity.MaTienDo, form.MaChiTietCV, files);
                    savedPhysicalPaths.AddRange(createdPaths);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                XoaFileVatLyDaLuu(savedPhysicalPaths);
                throw;
            }
        }

        public async Task DuyetBaoCaoTienDoAsync(TienDoCongViecDuyetViewModel form)
        {
            await XuLyBaoCaoTienDoAsync(form, TrangThai.DaDuyet);
        }

        public async Task YeuCauBoSungBaoCaoTienDoAsync(TienDoCongViecDuyetViewModel form)
        {
            await XuLyBaoCaoTienDoAsync(form, TrangThai.YeuCauBoSung);
        }

        public async Task TuChoiBaoCaoTienDoAsync(TienDoCongViecDuyetViewModel form)
        {
            await XuLyBaoCaoTienDoAsync(form, TrangThai.TuChoi);
        }

        private async Task XuLyBaoCaoTienDoAsync(TienDoCongViecDuyetViewModel form, string trangThaiDich)
        {
            if (form.MaTienDo <= 0)
                throw new Exception("Mã tiến độ không hợp lệ.");

            if (!string.IsNullOrWhiteSpace(form.GhiChuDuyet) && form.GhiChuDuyet.Trim().Length > 255)
                throw new Exception("Ghi chú duyệt tối đa 255 ký tự.");

            if ((TrangThai.EqualsValue(trangThaiDich, TrangThai.YeuCauBoSung) || TrangThai.EqualsValue(trangThaiDich, TrangThai.TuChoi))
                && string.IsNullOrWhiteSpace(form.GhiChuDuyet))
            {
                throw new Exception("Vui lòng nhập ghi chú duyệt khi yêu cầu bổ sung hoặc từ chối.");
            }

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminTacNghiep(roleFlags);

            var tienDo = await _context.TienDoCongViec
                .FirstOrDefaultAsync(x => x.MaTienDo == form.MaTienDo);
            if (tienDo == null)
                throw new Exception("Không tìm thấy báo cáo tiến độ.");

            var contextInfo = await GetContextByChiTietCvAsync(tienDo.MaChiTietCV);
            if (BiKhoaCapNhatTheoTrangThai(contextInfo.TrangThaiDuAn, contextInfo.TrangThaiCongViec, contextInfo.TrangThaiCTCV))
                throw new Exception("Dữ liệu dự án/công việc đã khóa, không thể xử lý báo cáo.");

            if (!TrangThai.EqualsValue(tienDo.TrangThaiTienDo, TrangThai.ChoDuyet))
                throw new Exception("Báo cáo tiến độ này đã được xử lý.");

            if (tienDo.MaNguoiDung == currentUserId)
                throw new Exception("Bạn không thể tự duyệt báo cáo của chính mình.");

            var coTheDuyet = await CoTheDuyetBaoCaoTheoScopeAsync(contextInfo.MaDuAn, currentUserId, roleFlags);
            if (!coTheDuyet)
                throw new Exception("Bạn không có quyền duyệt báo cáo tiến độ trong phạm vi dự án này.");

            var trangThaiDeXuat = TrangThai.ToCode(string.IsNullOrWhiteSpace(tienDo.TrangThaiCTCVDeXuat)
                ? tienDo.TrangThaiTienDo
                : tienDo.TrangThaiCTCVDeXuat);
            KiemTraTrangThaiChiTietHopLe(trangThaiDeXuat);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                tienDo.TrangThaiTienDo = trangThaiDich;
                tienDo.MaNguoiDungDuyet = currentUserId;
                tienDo.ThoiGianDuyet = DateTime.Now;
                tienDo.GhiChuDuyet = string.IsNullOrWhiteSpace(form.GhiChuDuyet) ? null : form.GhiChuDuyet.Trim();

                if (TrangThai.EqualsValue(trangThaiDich, TrangThai.DaDuyet))
                {
                    var trangThaiDaDuyetVariants = TrangThai.GetCommonStatusVariants(TrangThai.DaDuyet);
                    var baoCaoDaDuyetMoiNhat = await _context.TienDoCongViec
                        .Where(x => x.MaChiTietCV == tienDo.MaChiTietCV
                                    && x.MaTienDo != tienDo.MaTienDo
                                    && trangThaiDaDuyetVariants.Contains(x.TrangThaiTienDo ?? string.Empty))
                        .OrderByDescending(x => x.ThoiGianCapNhat ?? DateTime.MinValue)
                        .ThenByDescending(x => x.MaTienDo)
                        .FirstOrDefaultAsync();

                    KiemTraKhongDuocLuiTrangThai(
                        contextInfo.TrangThaiCTCV,
                        trangThaiDeXuat,
                        baoCaoDaDuyetMoiNhat?.TrangThaiCTCVDeXuat ?? baoCaoDaDuyetMoiNhat?.TrangThaiTienDo);

                    var soFileMinhChung = await _context.FileTienDoCongViec
                        .CountAsync(x => x.MaTienDo == tienDo.MaTienDo && x.IsDeleted != true);
                    KiemTraFileMinhChungTonTaiTheoDeXuat(trangThaiDeXuat, soFileMinhChung);

                    contextInfo.ChiTietCongViec.TrangThaiCTCV = trangThaiDeXuat;
                    contextInfo.ChiTietCongViec.NgayKetThucCTCV = TrangThai.LaHoanThanhCongViec(trangThaiDeXuat) ? DateTime.Now : null;
                    await _context.SaveChangesAsync();

                    await _trangThaiWorkflowService.DongBoChuoiTrangThaiTuCongViecAsync(
                        contextInfo.MaCongViec,
                        currentUserId,
                        "Duyệt báo cáo tiến độ");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        internal async Task<bool> CoTheTacNghiepTienDoChiTietAsync(int maChiTietCv, int currentUserId)
        {
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            if (roleFlags.IsAdmin)
                return false;

            var contextInfo = await GetContextByChiTietCvAsync(maChiTietCv);
            if (BiKhoaCapNhatTheoTrangThai(contextInfo.TrangThaiDuAn, contextInfo.TrangThaiCongViec, contextInfo.TrangThaiCTCV))
                return false;

            var trangThaiChoDuyetVariants = TrangThai.GetCommonStatusVariants(TrangThai.ChoDuyet);
            var dangCoBaoCaoChoDuyet = await _context.TienDoCongViec
                .AnyAsync(x => x.MaChiTietCV == maChiTietCv && trangThaiChoDuyetVariants.Contains(x.TrangThaiTienDo ?? string.Empty));
            if (dangCoBaoCaoChoDuyet)
                return false;

            return await _context.PhanCongCtCongViec
                .AnyAsync(x => x.MaChiTietCV == maChiTietCv && x.MaNguoiDung == currentUserId);
        }

        internal async Task<bool> CoTheXemTienDoChiTietAsync(int maChiTietCv, int currentUserId)
        {
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            if (roleFlags.IsAdmin)
                return true;

            var accessibleDetailIds = await GetAccessibleChiTietCongViecIdsAsync(currentUserId, roleFlags);
            return accessibleDetailIds.Contains(maChiTietCv);
        }

        internal async Task<int> GetCurrentUserIdForFileAsync()
        {
            return await GetCurrentUserIdAsync();
        }

        private async Task<(CtCongViec ChiTietCongViec, CongViec CongViec, DuAn DuAn, int MaDuAn, int MaCongViec, string? TrangThaiDuAn, string? TrangThaiCongViec, string? TrangThaiCTCV)> GetContextByChiTietCvAsync(int maChiTietCv)
        {
            var chiTiet = await _context.CtCongViec
                .FirstOrDefaultAsync(x => x.MaChiTietCV == maChiTietCv && x.IsDeleted != true);

            if (chiTiet == null)
                throw new Exception("Không tìm thấy chi tiết công việc.");

            var congViec = await _context.CongViec
                .FirstOrDefaultAsync(x => x.MaCongViec == chiTiet.MaCongViec && x.IsDeleted != true);

            if (congViec == null)
                throw new Exception("Không tìm thấy công việc cha.");

            var danhMuc = await _context.DanhMucCongViec
                .FirstOrDefaultAsync(x => x.MaDanhMucCV == congViec.MaDanhMucCV && x.IsDeleted != true);

            if (danhMuc == null)
                throw new Exception("Không tìm thấy danh mục công việc.");

            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == danhMuc.MaDuAn && x.IsDeleted != true);

            if (duAn == null)
                throw new Exception("Không tìm thấy dự án liên quan.");

            return (chiTiet, congViec, duAn, duAn.MaDuAn, congViec.MaCongViec, duAn.TrangThaiDuAn, congViec.TrangThaiCongViec, chiTiet.TrangThaiCTCV);
        }

        private async Task<List<int>> GetAccessibleChiTietCongViecIdsAsync(int currentUserId, (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags)
        {
            if (roleFlags.IsAdmin)
                return new List<int>();

            var query =
                from ct in _context.CtCongViec
                join cv in _context.CongViec on ct.MaCongViec equals cv.MaCongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where ct.IsDeleted != true && cv.IsDeleted != true && dm.IsDeleted != true
                select new { ct.MaChiTietCV, dm.MaDuAn };

            if (roleFlags.IsManager)
            {
                var managedProjectDetailIds = await query
                    .Where(x => _context.DuAn.Any(da => da.MaDuAn == x.MaDuAn && da.IsDeleted != true && da.MaNguoiDung == currentUserId))
                    .Select(x => x.MaChiTietCV)
                    .Distinct()
                    .ToListAsync();

                var managerAssignedDetailIds = await _context.PhanCongCtCongViec
                    .Where(x => x.MaNguoiDung == currentUserId)
                    .Select(x => x.MaChiTietCV)
                    .Distinct()
                    .ToListAsync();

                return managedProjectDetailIds
                    .Concat(managerAssignedDetailIds)
                    .Distinct()
                    .ToList();
            }

            var leaderVariants = new[] { TrangThai.VaiTroLeader, TrangThai.VaiTroLeaderHienThi };
            var leaderProjectIds = await _context.NhanVienDuAn
                .Where(x => x.MaNguoiDung == currentUserId && leaderVariants.Contains(x.VaiTroTrongDuAn ?? string.Empty))
                .Select(x => x.MaDuAn)
                .Distinct()
                .ToListAsync();

            var teamLeaderIds = await _context.NhanVienTeam
                .Where(x => x.MaNguoiDung == currentUserId && x.IsLeader == true)
                .Select(x => x.MaTeam)
                .Distinct()
                .ToListAsync();

            var teamLeaderProjectIds = await _context.TeamDuAn
                .Where(x => teamLeaderIds.Contains(x.MaTeam))
                .Select(x => x.MaDuAn)
                .Distinct()
                .ToListAsync();

            var assignedDetailIds = await _context.PhanCongCtCongViec
                .Where(x => x.MaNguoiDung == currentUserId)
                .Select(x => x.MaChiTietCV)
                .Distinct()
                .ToListAsync();

            var visibleProjectIds = leaderProjectIds.Concat(teamLeaderProjectIds).Distinct().ToList();

            if (visibleProjectIds.Count == 0)
                return assignedDetailIds;

            return await query
                .Where(x => visibleProjectIds.Contains(x.MaDuAn) || assignedDetailIds.Contains(x.MaChiTietCV))
                .Select(x => x.MaChiTietCV)
                .Distinct()
                .ToListAsync();
        }

        private async Task<HashSet<int>> GetProjectIdsForReviewAsync(int currentUserId, (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags)
        {
            if (roleFlags.IsAdmin)
                return new HashSet<int>();

            var projectIds = new HashSet<int>();

            if (roleFlags.IsManager)
            {
                var managedProjectIds = await _context.DuAn
                    .Where(x => x.IsDeleted != true && x.MaNguoiDung == currentUserId)
                    .Select(x => x.MaDuAn)
                    .Distinct()
                    .ToListAsync();
                foreach (var id in managedProjectIds)
                    projectIds.Add(id);
            }

            var leaderVariants = new[] { TrangThai.VaiTroLeader, TrangThai.VaiTroLeaderHienThi };
            var leaderProjectIds = await _context.NhanVienDuAn
                .Where(x => x.MaNguoiDung == currentUserId && leaderVariants.Contains(x.VaiTroTrongDuAn ?? string.Empty))
                .Select(x => x.MaDuAn)
                .Distinct()
                .ToListAsync();
            foreach (var id in leaderProjectIds)
                projectIds.Add(id);

            var teamLeaderIds = await _context.NhanVienTeam
                .Where(x => x.MaNguoiDung == currentUserId && x.IsLeader == true)
                .Select(x => x.MaTeam)
                .Distinct()
                .ToListAsync();

            var teamLeaderProjectIds = await _context.TeamDuAn
                .Where(x => teamLeaderIds.Contains(x.MaTeam))
                .Select(x => x.MaDuAn)
                .Distinct()
                .ToListAsync();
            foreach (var id in teamLeaderProjectIds)
                projectIds.Add(id);

            return projectIds;
        }

        private async Task<bool> CoTheDuyetBaoCaoTheoScopeAsync(int maDuAn, int currentUserId, (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags)
        {
            if (roleFlags.IsAdmin)
                return false;

            var reviewProjectIds = await GetProjectIdsForReviewAsync(currentUserId, roleFlags);
            return reviewProjectIds.Contains(maDuAn);
        }

        private static void KiemTraTrangThaiChiTietHopLe(string trangThaiChiTiet)
        {
            var status = TrangThai.ToCode(trangThaiChiTiet);
            var trangThaiHopLe = new[]
            {
                TrangThai.ChuaBatDau,
                TrangThai.DangThucHien,
                TrangThai.BiCanCan,
                TrangThai.ChoXacNhanHoanThanh,
                TrangThai.HoanThanh,
                TrangThai.TamDung,
                TrangThai.DaHuy
            };

            if (!trangThaiHopLe.Any(x => TrangThai.EqualsValue(x, status)))
                throw new Exception("Trạng thái đề xuất chi tiết công việc không hợp lệ.");
        }

        private static List<string> LayDanhSachTrangThaiDeXuatCoTheChon(string? trangThaiHienTai, string? trangThaiDaDuyetMoiNhat)
        {
            var trangThaiMoc = !string.IsNullOrWhiteSpace(trangThaiDaDuyetMoiNhat)
                ? TrangThai.ToCode(trangThaiDaDuyetMoiNhat)
                : TrangThai.ToCode(trangThaiHienTai);

            if (TrangThai.LaHoanThanhCongViec(trangThaiMoc))
                return new List<string>();

            var danhSachCoTheChon = new[]
            {
                TrangThai.ChuaBatDau,
                TrangThai.DangThucHien,
                TrangThai.BiCanCan,
                TrangThai.ChoXacNhanHoanThanh,
                TrangThai.HoanThanh,
                TrangThai.TamDung,
                TrangThai.DaHuy
            };

            var thuTuMoc = LayThuTuTrangThaiChiTiet(trangThaiMoc);
            if (thuTuMoc < 0)
                return danhSachCoTheChon.ToList();

            return danhSachCoTheChon
                .Where(x => LayThuTuTrangThaiChiTiet(x) >= thuTuMoc)
                .ToList();
        }

        private static void KiemTraKhongDuocLuiTrangThai(string? trangThaiHienTai, string? trangThaiMoi, string? trangThaiDaDuyetMoiNhat)
        {
            var currentCode = TrangThai.ToCode(trangThaiHienTai);
            var newCode = TrangThai.ToCode(trangThaiMoi);
            var latestApprovedCode = TrangThai.ToCode(trangThaiDaDuyetMoiNhat);

            if (TrangThai.LaHoanThanhCongViec(currentCode) && !TrangThai.LaHoanThanhCongViec(newCode))
                throw new Exception("Không thể cập nhật trạng thái lùi so với trạng thái hiện tại.");

            var thuTuTrangThaiMoi = LayThuTuTrangThaiChiTiet(newCode);
            var thuTuTrangThaiHienTai = LayThuTuTrangThaiChiTiet(currentCode);
            var thuTuTrangThaiDaDuyetMoiNhat = LayThuTuTrangThaiChiTiet(latestApprovedCode);

            if (thuTuTrangThaiHienTai >= 0 && thuTuTrangThaiMoi < thuTuTrangThaiHienTai)
                throw new Exception("Không thể cập nhật trạng thái lùi so với trạng thái chi tiết công việc hiện tại.");

            if (thuTuTrangThaiDaDuyetMoiNhat >= 0 && thuTuTrangThaiMoi < thuTuTrangThaiDaDuyetMoiNhat)
                throw new Exception("Không thể cập nhật trạng thái lùi so với báo cáo đã duyệt gần nhất.");
        }

        private static void KiemTraBaoCaoKhongRong(string? trangThaiHienTai, string? trangThaiDeXuat, string? ghiChuTienDo, int soLuongFile)
        {
            var currentCode = TrangThai.ToCode(trangThaiHienTai);
            var deXuatCode = TrangThai.ToCode(trangThaiDeXuat);
            var khongDoiTrangThai = TrangThai.EqualsValue(currentCode, deXuatCode);

            if (khongDoiTrangThai && string.IsNullOrWhiteSpace(ghiChuTienDo) && soLuongFile == 0)
            {
                throw new Exception("Không thể gửi báo cáo rỗng khi trạng thái đề xuất không thay đổi.");
            }
        }

        private static int LayThuTuTrangThaiChiTiet(string? trangThai)
        {
            var code = TrangThai.ToCode(trangThai);

            if (TrangThai.EqualsValue(code, TrangThai.ChuaBatDau)
                || TrangThai.EqualsValue(code, TrangThai.KhoiTao))
                return 0;

            if (TrangThai.EqualsValue(code, TrangThai.DangThucHien))
                return 1;

            if (TrangThai.EqualsValue(code, TrangThai.BiCanCan))
                return 2;

            if (TrangThai.EqualsValue(code, TrangThai.ChoXacNhanHoanThanh))
                return 3;

            if (TrangThai.LaHoanThanhCongViec(code))
                return 4;

            if (TrangThai.EqualsValue(code, TrangThai.TamDung))
                return 5;

            if (TrangThai.EqualsValue(code, TrangThai.DaHuy))
                return 6;

            return -1;
        }

        private static int TinhPhanTramTheoTrangThai(string? trangThai)
        {
            if (TrangThai.LaHoanThanhCongViec(trangThai))
                return 100;

            if (TrangThai.EqualsValue(trangThai, TrangThai.ChoXacNhanHoanThanh))
                return 90;

            if (TrangThai.EqualsValue(trangThai, TrangThai.DangThucHien)
                || TrangThai.EqualsValue(trangThai, TrangThai.BiCanCan)
                || TrangThai.EqualsValue(trangThai, TrangThai.TamDung))
                return 50;

            return 0;
        }

        private static bool BiKhoaCapNhatTheoTrangThai(string? trangThaiDuAn, string? trangThaiCongViec, string? trangThaiCtCongViec)
        {
            if (TrangThai.LaHoanThanhCongViec(trangThaiDuAn)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.DaHuy)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.TamDung)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.LuuTru))
                return true;

            if (TrangThai.LaHoanThanhCongViec(trangThaiCongViec)
                || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.DaHuy)
                || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.TamDung)
                || TrangThai.EqualsValue(trangThaiCongViec, TrangThai.LuuTru))
                return true;

            if (TrangThai.LaHoanThanhCongViec(trangThaiCtCongViec)
                || TrangThai.EqualsValue(trangThaiCtCongViec, TrangThai.DaHuy)
                || TrangThai.EqualsValue(trangThaiCtCongViec, TrangThai.TamDung)
                || TrangThai.EqualsValue(trangThaiCtCongViec, TrangThai.LuuTru))
                return true;

            return false;
        }

        private static void KiemTraFileMinhChungHopLe(List<IFormFile> files, string trangThaiDeXuat)
        {
            if ((TrangThai.EqualsValue(trangThaiDeXuat, TrangThai.ChoXacNhanHoanThanh)
                || TrangThai.LaHoanThanhCongViec(trangThaiDeXuat))
                && files.Count == 0)
            {
                throw new Exception("Vui lòng tải tệp minh chứng khi đề xuất chờ xác nhận hoặc hoàn thành.");
            }

            foreach (var file in files)
            {
                if (file.Length <= 0)
                    throw new Exception("Không cho phép tệp minh chứng rỗng.");

                if (file.Length > MaxEvidenceFileSize)
                    throw new Exception("Dung lượng mỗi tệp minh chứng không được vượt quá 10MB.");

                var extension = Path.GetExtension(file.FileName ?? string.Empty);
                if (string.IsNullOrWhiteSpace(extension) || !AllowedEvidenceExtensions.Contains(extension))
                    throw new Exception("Định dạng tệp minh chứng không hợp lệ.");
            }
        }

        private static void KiemTraFileMinhChungTonTaiTheoDeXuat(string trangThaiDeXuat, int soLuongFile)
        {
            if ((TrangThai.EqualsValue(trangThaiDeXuat, TrangThai.ChoXacNhanHoanThanh)
                || TrangThai.LaHoanThanhCongViec(trangThaiDeXuat))
                && soLuongFile <= 0)
            {
                throw new Exception("Báo cáo đề xuất hoàn thành phải có tệp minh chứng.");
            }
        }

        private async Task<List<string>> LuuFileMinhChungChoTienDoAsync(int maTienDo, int maChiTietCv, List<IFormFile> files)
        {
            var savedPhysicalPaths = new List<string>();
            var folderPath = Path.Combine(GetWebRootPath(), "uploads", "tiendocongviec", maChiTietCv.ToString());
            Directory.CreateDirectory(folderPath);

            foreach (var file in files)
            {
                var originalFileName = Path.GetFileName(file.FileName ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(originalFileName))
                    originalFileName = "tep_tien_do";

                var extension = Path.GetExtension(originalFileName);
                var savedFileName = $"{Guid.NewGuid():N}{extension}";
                var physicalPath = Path.Combine(folderPath, savedFileName);

                await using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                savedPhysicalPaths.Add(physicalPath);

                _context.FileTienDoCongViec.Add(new FileTienDoCongViec
                {
                    MaTienDo = maTienDo,
                    TenFileTDCV = originalFileName,
                    DuongDanFileTDCV = $"/uploads/tiendocongviec/{maChiTietCv}/{savedFileName}",
                    NgayUploadFileTDCV = DateTime.Now,
                    IsDeleted = false
                });
            }

            return savedPhysicalPaths;
        }

        private static void XoaFileVatLyDaLuu(List<string> savedPhysicalPaths)
        {
            foreach (var path in savedPhysicalPaths.Distinct())
            {
                try
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
                catch
                {
                    // No-op: giữ nguyên lỗi nghiệp vụ gốc.
                }
            }
        }

        private string GetWebRootPath()
        {
            if (!string.IsNullOrWhiteSpace(_environment.WebRootPath))
                return _environment.WebRootPath;

            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        private static void KiemTraKhongChoAdminTacNghiep((bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags)
        {
            if (roleFlags.IsAdmin)
                throw new Exception("Tài khoản Admin không thao tác nghiệp vụ tiến độ.");
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
    }
}
