using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.DuAn;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class DuAnService : IDuAnService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChatDuAnService _chatDuAnService;

        public DuAnService(
            QuanLyDuAnDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IChatDuAnService chatDuAnService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _chatDuAnService = chatDuAnService;
        }

        #region CRUD Operations

        public async Task<List<DuAnViewModel>> GetAllAsync(
            string? tuKhoa,
            int? maLoaiDuAn,
            string? trangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var (isManager, isEmployee) = await GetCurrentUserRoleFlagsAsync();
            var (tuNgayLoc, denNgayLoc) = ChuanHoaKhoangNgay(tuNgay, denNgay);
            var locTheo = string.IsNullOrWhiteSpace(locTheoNgay) ? "NgayTao" : locTheoNgay.Trim();

            var query = from da in _context.DuAn
                        join loai in _context.LoaiDuAn on da.MaLoaiDuAn equals loai.MaLoaiDuAn
                        where da.IsDeleted != true
                        orderby da.MaDuAn descending
                        select new DuAnViewModel
                        {
                            MaDuAn = da.MaDuAn,
                            TenDuAn = da.TenDuAn ?? string.Empty,
                            MoTaDuAn = da.MoTaDuAn,
                            MaNguoiDung = da.MaNguoiDung,
                            TenNguoiQuanLy = string.Empty,
                            MaLoaiDuAn = da.MaLoaiDuAn,
                            TenLoaiDuAn = loai.TenLoai ?? string.Empty,
                            NgayTaoDuAn = da.NgayTaoDuAn,
                            NgayBatDauDuAn = da.NgayBatDauDuAn,
                            NgayKetThucDuAn = da.NgayKetThucDuAn,
                            NgayHoanThanhThucTeDuAn = da.NgayHoanThanhThucTeDuAn,
                            PhanTramHoanThanh = da.PhanTramHoanThanh ?? 0,
                            TrangThaiDuAn = da.TrangThaiDuAn ?? string.Empty,
                            SoLuongTeam = _context.TeamDuAn.Count(x => x.MaDuAn == da.MaDuAn),
                            SoLuongThanhVien = _context.NhanVienDuAn.Count(x => x.MaDuAn == da.MaDuAn),
                            HasApprovedBudget = _context.NganSach.Any(x =>
                                x.MaDuAn == da.MaDuAn
                                && x.IsDeleted != true
                                && x.IsActive == true
                                && (x.TrangThaiNganSach == TrangThai.DaDuyet || x.TrangThaiNganSach == TrangThai.DaDuyetHienThi))
                        };

            if (isManager)
            {
                query = query.Where(x => x.MaNguoiDung == currentUserId);
            }
            else if (isEmployee)
            {
                query = query.Where(x => _context.NhanVienDuAn.Any(nv =>
                    nv.MaDuAn == x.MaDuAn && nv.MaNguoiDung == currentUserId));
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    x.TenDuAn.ToLower().Contains(keyword) ||
                    x.TenLoaiDuAn.ToLower().Contains(keyword) ||
                    (x.MoTaDuAn != null && x.MoTaDuAn.ToLower().Contains(keyword)));
            }

            if (maLoaiDuAn.HasValue && maLoaiDuAn.Value > 0)
            {
                query = query.Where(x => x.MaLoaiDuAn == maLoaiDuAn.Value);
            }

            if (!string.IsNullOrWhiteSpace(trangThaiDuAn))
            {
                var filterValues = TrangThai.GetCommonStatusVariants(trangThaiDuAn);
                if (filterValues.Length > 0)
                {
                    query = query.Where(x => filterValues.Contains(x.TrangThaiDuAn));
                }
            }

            if (tuNgayLoc.HasValue)
            {
                query = locTheo switch
                {
                    "NgayBatDau" => query.Where(x => x.NgayBatDauDuAn.HasValue && x.NgayBatDauDuAn.Value >= tuNgayLoc.Value),
                    "NgayKetThuc" => query.Where(x => x.NgayKetThucDuAn.HasValue && x.NgayKetThucDuAn.Value >= tuNgayLoc.Value),
                    _ => query.Where(x => x.NgayTaoDuAn.HasValue && x.NgayTaoDuAn.Value >= tuNgayLoc.Value)
                };
            }

            if (denNgayLoc.HasValue)
            {
                var denNgayDocQuyen = denNgayLoc.Value.AddDays(1);
                query = locTheo switch
                {
                    "NgayBatDau" => query.Where(x => x.NgayBatDauDuAn.HasValue && x.NgayBatDauDuAn.Value < denNgayDocQuyen),
                    "NgayKetThuc" => query.Where(x => x.NgayKetThucDuAn.HasValue && x.NgayKetThucDuAn.Value < denNgayDocQuyen),
                    _ => query.Where(x => x.NgayTaoDuAn.HasValue && x.NgayTaoDuAn.Value < denNgayDocQuyen)
                };
            }

            query = ApplyDeadlineFilter(query, locTinhTrangThoiHan, DateTime.Now);

            var items = await query.ToListAsync();
            await GanTinhTrangThoiHanAsync(items);
            return items;
        }

        public async Task<PagedResultViewModel<DuAnViewModel>> GetPagedAsync(
            string? tuKhoa,
            int? maLoaiDuAn,
            string? trangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan,
            int pageNumber = 1,
            int pageSize = PaginationViewModel.DefaultPageSize)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var (isManager, isEmployee) = await GetCurrentUserRoleFlagsAsync();
            var (tuNgayLoc, denNgayLoc) = ChuanHoaKhoangNgay(tuNgay, denNgay);
            var locTheo = string.IsNullOrWhiteSpace(locTheoNgay) ? "NgayTao" : locTheoNgay.Trim();

            var query = from da in _context.DuAn
                        join loai in _context.LoaiDuAn on da.MaLoaiDuAn equals loai.MaLoaiDuAn
                        where da.IsDeleted != true
                        select new DuAnViewModel
                        {
                            MaDuAn = da.MaDuAn,
                            TenDuAn = da.TenDuAn ?? string.Empty,
                            MoTaDuAn = da.MoTaDuAn,
                            MaNguoiDung = da.MaNguoiDung,
                            TenNguoiQuanLy = string.Empty,
                            MaLoaiDuAn = da.MaLoaiDuAn,
                            TenLoaiDuAn = loai.TenLoai ?? string.Empty,
                            NgayTaoDuAn = da.NgayTaoDuAn,
                            NgayBatDauDuAn = da.NgayBatDauDuAn,
                            NgayKetThucDuAn = da.NgayKetThucDuAn,
                            NgayHoanThanhThucTeDuAn = da.NgayHoanThanhThucTeDuAn,
                            PhanTramHoanThanh = da.PhanTramHoanThanh ?? 0,
                            TrangThaiDuAn = da.TrangThaiDuAn ?? string.Empty,
                            SoLuongTeam = _context.TeamDuAn.Count(x => x.MaDuAn == da.MaDuAn),
                            SoLuongThanhVien = _context.NhanVienDuAn.Count(x => x.MaDuAn == da.MaDuAn),
                            HasApprovedBudget = _context.NganSach.Any(x =>
                                x.MaDuAn == da.MaDuAn
                                && x.IsDeleted != true
                                && x.IsActive == true
                                && (x.TrangThaiNganSach == TrangThai.DaDuyet || x.TrangThaiNganSach == TrangThai.DaDuyetHienThi))
                        };

            if (isManager)
            {
                query = query.Where(x => x.MaNguoiDung == currentUserId);
            }
            else if (isEmployee)
            {
                query = query.Where(x => _context.NhanVienDuAn.Any(nv =>
                    nv.MaDuAn == x.MaDuAn && nv.MaNguoiDung == currentUserId));
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    x.TenDuAn.ToLower().Contains(keyword) ||
                    x.TenLoaiDuAn.ToLower().Contains(keyword) ||
                    (x.MoTaDuAn != null && x.MoTaDuAn.ToLower().Contains(keyword)));
            }

            if (maLoaiDuAn.HasValue && maLoaiDuAn.Value > 0)
            {
                query = query.Where(x => x.MaLoaiDuAn == maLoaiDuAn.Value);
            }

            if (!string.IsNullOrWhiteSpace(trangThaiDuAn))
            {
                var filterValues = TrangThai.GetCommonStatusVariants(trangThaiDuAn);
                if (filterValues.Length > 0)
                {
                    query = query.Where(x => filterValues.Contains(x.TrangThaiDuAn));
                }
            }

            if (tuNgayLoc.HasValue)
            {
                query = locTheo switch
                {
                    "NgayBatDau" => query.Where(x => x.NgayBatDauDuAn.HasValue && x.NgayBatDauDuAn.Value >= tuNgayLoc.Value),
                    "NgayKetThuc" => query.Where(x => x.NgayKetThucDuAn.HasValue && x.NgayKetThucDuAn.Value >= tuNgayLoc.Value),
                    _ => query.Where(x => x.NgayTaoDuAn.HasValue && x.NgayTaoDuAn.Value >= tuNgayLoc.Value)
                };
            }

            if (denNgayLoc.HasValue)
            {
                var denNgayDocQuyen = denNgayLoc.Value.AddDays(1);
                query = locTheo switch
                {
                    "NgayBatDau" => query.Where(x => x.NgayBatDauDuAn.HasValue && x.NgayBatDauDuAn.Value < denNgayDocQuyen),
                    "NgayKetThuc" => query.Where(x => x.NgayKetThucDuAn.HasValue && x.NgayKetThucDuAn.Value < denNgayDocQuyen),
                    _ => query.Where(x => x.NgayTaoDuAn.HasValue && x.NgayTaoDuAn.Value < denNgayDocQuyen)
                };
            }

            query = ApplyDeadlineFilter(query, locTinhTrangThoiHan, DateTime.Now);

            var totalItems = await query.CountAsync();
            var pagination = PaginationViewModel.Create(pageNumber, pageSize, totalItems);
            var items = await query
                .OrderByDescending(x => x.MaDuAn)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            await GanTinhTrangThoiHanAsync(items);

            return new PagedResultViewModel<DuAnViewModel>
            {
                Items = items,
                Pagination = pagination
            };
        }

        public async Task<bool> CanAccessAsync(int maDuAn)
        {
            if (maDuAn <= 0)
            {
                return false;
            }

            var currentUserId = await GetCurrentUserIdAsync();
            var (isManager, isEmployee) = await GetCurrentUserRoleFlagsAsync();

            var query = _context.DuAn
                .AsNoTracking()
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (isManager)
            {
                query = query.Where(x => x.MaNguoiDung == currentUserId);
            }
            else if (isEmployee)
            {
                query = query.Where(x => _context.NhanVienDuAn.Any(nv =>
                    nv.MaDuAn == x.MaDuAn && nv.MaNguoiDung == currentUserId));
            }

            return await query.AnyAsync();
        }

        public async Task<DuAnChiTietViewModel?> GetChiTietAsync(int id)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var (isManager, isEmployee) = await GetCurrentUserRoleFlagsAsync();

            var duAnQuery = from da in _context.DuAn.AsNoTracking()
                              join loai in _context.LoaiDuAn on da.MaLoaiDuAn equals loai.MaLoaiDuAn
                              where da.MaDuAn == id && da.IsDeleted != true
                              select new
                              {
                                  da.MaDuAn,
                                  da.TenDuAn,
                                  da.MoTaDuAn,
                                  da.MaLoaiDuAn,
                                  TenLoaiDuAn = loai.TenLoai,
                                  da.NgayTaoDuAn,
                                  da.NgayBatDauDuAn,
                                  da.NgayKetThucDuAn,
                                  da.NgayHoanThanhThucTeDuAn,
                                  da.TrangThaiDuAn,
                                  da.PhanTramHoanThanh,
                                  da.GhiChuDuAn,
                                  da.MaNguoiDung
                              };

            if (isManager)
            {
                duAnQuery = duAnQuery.Where(x => x.MaNguoiDung == currentUserId);
            }
            else if (isEmployee)
            {
                duAnQuery = duAnQuery.Where(x => _context.NhanVienDuAn.Any(nv =>
                    nv.MaDuAn == x.MaDuAn && nv.MaNguoiDung == currentUserId));
            }

            var duAn = await duAnQuery.FirstOrDefaultAsync();

            if (duAn == null)
                return null;

            var tenNguoiQuanLy = await _context.NguoiDung
                .Where(x => x.MaNguoiDung == duAn.MaNguoiDung && x.IsDeleted != true)
                .Select(x => x.HoTenNguoiDung)
                .FirstOrDefaultAsync();

            var queryCongViec = _context.CongViec
                .AsNoTracking()
                .Join(_context.DanhMucCongViec.AsNoTracking(), cv => cv.MaDanhMucCV, dm => dm.MaDanhMucCV, (cv, dm) => new { cv, dm })
                .Where(x => x.dm.MaDuAn == id && x.dm.IsDeleted != true && x.cv.IsDeleted != true);

            var now = DateTime.Now;
            var congViecThongKe = await queryCongViec
                .Select(x => new
                {
                    x.cv.MaCongViec,
                    x.cv.TenCongViec,
                    x.dm.TenDanhMucCV,
                    x.dm.MaDanhMucCV,
                    x.cv.TrangThaiCongViec,
                    x.cv.NgayTaoCongViec,
                    x.cv.NgayKetThucCVDuKien,
                    x.cv.NgayKetThucCVThucTe
                })
                .ToListAsync();

            var soLuongCongViec = congViecThongKe.Count;

            var soLuongChiTietCongViec = await _context.CtCongViec
                .Join(queryCongViec, ct => ct.MaCongViec, x => x.cv.MaCongViec, (ct, x) => new { ct })
                .CountAsync(x => x.ct.IsDeleted != true);

            var maCongViecDauTien = congViecThongKe
                .OrderBy(x => x.MaCongViec)
                .Select(x => (int?)x.MaCongViec)
                .FirstOrDefault();

            var danhSachFile = await _context.FileDuAn
                .Where(x => x.MaDuAn == id && x.IsDeleted != true)
                .OrderByDescending(x => x.NgayUploadFileDA ?? DateTime.MinValue)
                .Select(x => new DuAnFileItemViewModel
                {
                    MaFileDA = x.MaFileDA,
                    TenFileDA = x.TenFileDA ?? $"File {x.MaFileDA}",
                    DuongDanFileDA = x.DuongDanFileDA ?? string.Empty,
                    NgayUploadFileDA = x.NgayUploadFileDA
                })
                .ToListAsync();

            var statusCheck = await CheckProjectStatusAsync(id);

            var hasApprovedBudget = await _context.NganSach.AnyAsync(x =>
                x.MaDuAn == id
                && x.IsDeleted != true
                && x.IsActive == true
                && (x.TrangThaiNganSach == TrangThai.DaDuyet || x.TrangThaiNganSach == TrangThai.DaDuyetHienThi));

            int? soNgayConLai = null;
            var isSapDenHan = false;
            if (duAn.NgayKetThucDuAn.HasValue)
            {
                var diffDays = (int)Math.Floor((duAn.NgayKetThucDuAn.Value - now).TotalDays);
                soNgayConLai = diffDays;

                var isCompleted = TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.HoanThanh)
                                  || TrangThai.EqualsValue(duAn.TrangThaiDuAn, TrangThai.LuuTru);

                if (!isCompleted && diffDays >= 0 && diffDays <= 7)
                {
                    isSapDenHan = true;
                }
            }

            var congViecHoanThanh = 0;
            var congViecDangThucHien = 0;
            var congViecTamDung = 0;
            var congViecChuaBatDau = 0;
            var congViecBiCanTro = 0;
            var congViecChoXacNhan = 0;
            var congViecDangQuaHan = 0;
            var congViecHoanThanhDungHan = 0;
            var congViecHoanThanhTre = 0;
            var congViecHoanThanhThieuDuLieuNgay = 0;

            foreach (var congViec in congViecThongKe)
            {
                var trangThaiCongViec = congViec.TrangThaiCongViec;
                var laHoanThanh = TrangThai.LaHoanThanhCongViec(trangThaiCongViec);
                var laDaHuy = TrangThai.EqualsValue(trangThaiCongViec, TrangThai.DaHuy);
                var laChoXacNhan = TrangThai.EqualsValue(trangThaiCongViec, TrangThai.ChoXacNhanHoanThanh);

                if (laHoanThanh)
                {
                    congViecHoanThanh++;

                    if (!congViec.NgayKetThucCVDuKien.HasValue || !congViec.NgayKetThucCVThucTe.HasValue)
                    {
                        congViecHoanThanhThieuDuLieuNgay++;
                    }
                    else if (congViec.NgayKetThucCVThucTe.Value <= congViec.NgayKetThucCVDuKien.Value)
                    {
                        congViecHoanThanhDungHan++;
                    }
                    else
                    {
                        congViecHoanThanhTre++;
                    }
                }

                if (TrangThai.EqualsValue(trangThaiCongViec, TrangThai.DangThucHien))
                {
                    congViecDangThucHien++;
                }
                else if (TrangThai.EqualsValue(trangThaiCongViec, TrangThai.TamDung))
                {
                    congViecTamDung++;
                }
                else if (TrangThai.EqualsValue(trangThaiCongViec, TrangThai.ChuaBatDau))
                {
                    congViecChuaBatDau++;
                }
                else if (TrangThai.EqualsValue(trangThaiCongViec, TrangThai.BiCanCan))
                {
                    congViecBiCanTro++;
                }
                else if (laChoXacNhan)
                {
                    congViecChoXacNhan++;
                }

                if (!laHoanThanh
                    && !laDaHuy
                    && congViec.NgayKetThucCVDuKien.HasValue
                    && congViec.NgayKetThucCVDuKien.Value < now
                    && !(laChoXacNhan && congViec.NgayKetThucCVThucTe.HasValue))
                {
                    congViecDangQuaHan++;
                }
            }

            var congViecTreHan = congViecDangQuaHan + congViecHoanThanhTre;

            decimal? tiLeHoanThanh = null;
            var tiLeHoanThanhCap = 0m;
            if (soLuongCongViec > 0)
            {
                tiLeHoanThanh = Math.Round((decimal)congViecHoanThanh / soLuongCongViec * 100m, 0);
                tiLeHoanThanhCap = Math.Min(tiLeHoanThanh.Value, 100m);
            }

            decimal? tyLeHoanThanhDungHan = null;
            var tyLeHoanThanhDungHanCap = 0m;
            var tongHoanThanhCoDuNgay = congViecHoanThanhDungHan + congViecHoanThanhTre;
            if (tongHoanThanhCoDuNgay > 0)
            {
                tyLeHoanThanhDungHan = Math.Round((decimal)congViecHoanThanhDungHan / tongHoanThanhCoDuNgay * 100m, 1);
                tyLeHoanThanhDungHanCap = Math.Min(tyLeHoanThanhDungHan.Value, 100m);
            }

            var congViecGanDay = congViecThongKe
                .OrderByDescending(x => x.NgayTaoCongViec ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaCongViec)
                .Take(5)
                .Select(x => new DuAnRecentWorkItemViewModel
                {
                    MaCongViec = x.MaCongViec,
                    TenCongViec = x.TenCongViec ?? string.Empty,
                    TenDanhMucCV = x.TenDanhMucCV ?? $"Danh mục {x.MaDanhMucCV}",
                    TrangThaiCongViec = x.TrangThaiCongViec ?? string.Empty,
                    NgayTaoCongViec = x.NgayTaoCongViec,
                    NgayKetThucDuKien = x.NgayKetThucCVDuKien,
                    NgayKetThucThucTe = x.NgayKetThucCVThucTe
                })
                .ToList();

            foreach (var congViec in congViecGanDay)
            {
                GanTinhTrangThoiHanCongViec(congViec, now);
            }

            var trangThaiHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
            var deadlineGanNhat = await queryCongViec
                .Where(x => x.cv.NgayKetThucCVDuKien.HasValue
                            && !trangThaiHoanThanh.Contains(x.cv.TrangThaiCongViec ?? string.Empty))
                .OrderBy(x => x.cv.NgayKetThucCVDuKien)
                .Select(x => new DuAnDeadlinePreviewViewModel
                {
                    MaCongViec = x.cv.MaCongViec,
                    TenCongViec = x.cv.TenCongViec ?? string.Empty,
                    TrangThaiCongViec = x.cv.TrangThaiCongViec ?? string.Empty,
                    NgayKetThucDuKien = x.cv.NgayKetThucCVDuKien
                })
                .FirstOrDefaultAsync();

            if (deadlineGanNhat?.NgayKetThucDuKien != null)
            {
                deadlineGanNhat.SoNgayConLai = (int)Math.Floor((deadlineGanNhat.NgayKetThucDuKien.Value - now).TotalDays);
            }

            var tongNganSachDaDuyet = await _context.NganSach
                .Where(x => x.MaDuAn == id
                            && x.IsDeleted != true
                            && x.IsActive == true
                            && (x.TrangThaiNganSach == TrangThai.DaDuyet || x.TrangThaiNganSach == TrangThai.DaDuyetHienThi))
                .SumAsync(x => x.SoTienNganSach ?? 0m);

            var tongChiPhiDaDung = await (
                from cp in _context.ChiPhi
                join ns in _context.NganSach on cp.MaNganSach equals ns.MaNganSach
                where cp.IsDeleted != true
                      && ns.IsDeleted != true
                      && ns.MaDuAn == id
                select cp.SoTienDaChi ?? 0m
            ).SumAsync();

            var coChiPhi = await (
                from cp in _context.ChiPhi
                join ns in _context.NganSach on cp.MaNganSach equals ns.MaNganSach
                where cp.IsDeleted != true
                      && ns.IsDeleted != true
                      && ns.MaDuAn == id
                select cp.MaChiPhi
            ).AnyAsync();

            decimal? tongNganSachDaDuyetDisplay = hasApprovedBudget ? tongNganSachDaDuyet : null;
            decimal? soTienConLai = tongNganSachDaDuyetDisplay.HasValue
                ? tongNganSachDaDuyetDisplay.Value - tongChiPhiDaDung
                : null;

            decimal? phanTramSuDung = tongNganSachDaDuyetDisplay.HasValue && tongNganSachDaDuyetDisplay.Value > 0
                ? Math.Round((tongChiPhiDaDung / tongNganSachDaDuyetDisplay.Value) * 100m, 0)
                : null;

            var phanTramCap = phanTramSuDung.HasValue
                ? Math.Min(phanTramSuDung.Value, 100m)
                : 0m;

            var vuotNganSach = tongNganSachDaDuyetDisplay.HasValue && tongChiPhiDaDung > tongNganSachDaDuyetDisplay.Value;
            var sapVuotNganSach = phanTramSuDung.HasValue && phanTramSuDung.Value >= 80m && phanTramSuDung.Value <= 100m;

            var trangThaiNganSachHienThi = "Bình thường";
            var trangThaiNganSachCss = "normal";

            if (vuotNganSach)
            {
                trangThaiNganSachHienThi = "Vượt ngân sách";
                trangThaiNganSachCss = "over";
            }
            else if (sapVuotNganSach)
            {
                trangThaiNganSachHienThi = "Sắp vượt";
                trangThaiNganSachCss = "near";
            }

            var tepGanDay = danhSachFile
                .OrderByDescending(x => x.NgayUploadFileDA ?? DateTime.MinValue)
                .Take(5)
                .Select(x => new DuAnRecentFileViewModel
                {
                    MaFileDA = x.MaFileDA,
                    TenFileDA = x.TenFileDA,
                    NgayUploadFileDA = x.NgayUploadFileDA
                })
                .ToList();

            var thanhVienNoiBat = await (
                from nvda in _context.NhanVienDuAn
                join nd in _context.NguoiDung on nvda.MaNguoiDung equals nd.MaNguoiDung
                where nvda.MaDuAn == id && nd.IsDeleted != true
                orderby nvda.NgayThamGiaDuAn descending
                select new DuAnMemberPreviewViewModel
                {
                    MaNguoiDung = nvda.MaNguoiDung,
                    HoTenNguoiDung = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}",
                    VaiTroTrongDuAn = nvda.VaiTroTrongDuAn ?? TrangThai.VaiTroMember,
                    NgayThamGiaDuAn = nvda.NgayThamGiaDuAn
                })
                .Take(5)
                .ToListAsync();

            var hoatDongQuanLy = await (
                from nk in _context.NhatKyQuanLyDuAn
                join nd in _context.NguoiDung on nk.MaNguoiDung equals nd.MaNguoiDung
                where nk.MaDuAn == id
                orderby nk.NkThoiGianQLDA descending
                select new DuAnActivityPreviewViewModel
                {
                    NoiDung = nk.NkHanhDongQLDA ?? string.Empty,
                    NguoiThucHien = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}",
                    ThoiGian = nk.NkThoiGianQLDA,
                    LoaiHanhDong = "Quản lý dự án"
                })
                .Take(5)
                .ToListAsync();

            var hoatDongPhuTrach = await (
                from nk in _context.NhatKyPhuTrachDuAn
                join nd in _context.NguoiDung on nk.MaNguoiDung equals nd.MaNguoiDung
                where nk.MaDuAn == id
                orderby nk.NkThoiGianPTDA descending
                select new DuAnActivityPreviewViewModel
                {
                    NoiDung = nk.NkHanhDongPTDA ?? string.Empty,
                    NguoiThucHien = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}",
                    ThoiGian = nk.NkThoiGianPTDA,
                    LoaiHanhDong = "Phụ trách dự án"
                })
                .Take(5)
                .ToListAsync();

            var hoatDongGanDay = hoatDongQuanLy
                .Concat(hoatDongPhuTrach)
                .OrderByDescending(x => x.ThoiGian ?? DateTime.MinValue)
                .Take(5)
                .ToList();

            var coTheYeuCauDoiQuanLy = false;
            string? lyDoKhongTheYeuCauDoiQuanLy = null;
            var currentUserIdFromClaim = TryGetCurrentUserIdFromClaims();

            if (!HasCurrentUserPermission(Permissions.YeuCauDoiQuanLy.Them))
            {
                lyDoKhongTheYeuCauDoiQuanLy = "Bạn chưa có quyền tạo yêu cầu đổi quản lý.";
            }
            else if (!currentUserIdFromClaim.HasValue || currentUserIdFromClaim.Value <= 0)
            {
                lyDoKhongTheYeuCauDoiQuanLy = "Không xác định được người dùng hiện tại.";
            }
            else if (IsCurrentUserInRole("Admin"))
            {
                lyDoKhongTheYeuCauDoiQuanLy = "Tài khoản Admin không được tạo yêu cầu đổi quản lý.";
            }
            else if (!IsCurrentUserInRole("Manager"))
            {
                lyDoKhongTheYeuCauDoiQuanLy = "Chỉ Manager mới được tạo yêu cầu đổi quản lý.";
            }
            else if (duAn.MaNguoiDung != currentUserIdFromClaim.Value)
            {
                lyDoKhongTheYeuCauDoiQuanLy = "Bạn không phải quản lý hiện tại của dự án.";
            }
            else if (!IsProjectStatusAllowedForManagerRequest(duAn.TrangThaiDuAn))
            {
                lyDoKhongTheYeuCauDoiQuanLy = "Dự án đang ở trạng thái không cho phép tạo yêu cầu đổi quản lý.";
            }
            else
            {
                var choDuyetStatuses = TrangThai.GetCommonStatusVariants(TrangThai.ChoDuyet);
                var hasPendingRequest = await _context.YeuCauDoiQuanLy.AnyAsync(x =>
                    x.IsDeleted != true
                    && x.MaDuAn == id
                    && choDuyetStatuses.Contains(x.TrangThaiYeuCauDoiQuanLy ?? string.Empty));

                if (hasPendingRequest)
                {
                    lyDoKhongTheYeuCauDoiQuanLy = "Dự án đang có yêu cầu đổi quản lý chờ duyệt.";
                }
                else
                {
                    coTheYeuCauDoiQuanLy = true;
                }
            }

            var congViecVuotHanDuAn = duAn.NgayKetThucDuAn.HasValue
                ? congViecThongKe.Count(x =>
                    TrangThai.LaHoanThanhCongViec(x.TrangThaiCongViec)
                    && x.NgayKetThucCVThucTe.HasValue
                    && x.NgayKetThucCVThucTe.Value > duAn.NgayKetThucDuAn.Value)
                : 0;

            var deadlineSource = new DuAnViewModel
            {
                TrangThaiDuAn = duAn.TrangThaiDuAn ?? string.Empty,
                NgayKetThucDuAn = duAn.NgayKetThucDuAn,
                NgayHoanThanhThucTeDuAn = duAn.NgayHoanThanhThucTeDuAn,
                SoCongViecTre = congViecTreHan,
                SoCongViecVuotHanDuAn = congViecVuotHanDuAn
            };
            DuAnDeadlineStatusHelper.Apply(deadlineSource, now);

            return new DuAnChiTietViewModel
            {
                MaDuAn = duAn.MaDuAn,
                TenDuAn = duAn.TenDuAn ?? string.Empty,
                MoTaDuAn = duAn.MoTaDuAn,
                MaLoaiDuAn = duAn.MaLoaiDuAn,
                TenLoaiDuAn = duAn.TenLoaiDuAn ?? string.Empty,
                NgayTaoDuAn = duAn.NgayTaoDuAn,
                NgayBatDauDuAn = duAn.NgayBatDauDuAn,
                NgayKetThucDuAn = duAn.NgayKetThucDuAn,
                NgayHoanThanhThucTeDuAn = duAn.NgayHoanThanhThucTeDuAn,
                TrangThaiDuAn = duAn.TrangThaiDuAn ?? string.Empty,
                PhanTramHoanThanh = duAn.PhanTramHoanThanh ?? 0,
                GhiChuDuAn = duAn.GhiChuDuAn,
                MaNguoiDung = duAn.MaNguoiDung,
                TenNguoiQuanLy = tenNguoiQuanLy ?? string.Empty,
                SoLuongTeam = await _context.TeamDuAn.CountAsync(x => x.MaDuAn == id),
                SoLuongThanhVien = await _context.NhanVienDuAn.CountAsync(x => x.MaDuAn == id),
                SoLuongCongViec = soLuongCongViec,
                SoLuongChiTietCongViec = soLuongChiTietCongViec,
                MaCongViecDauTien = maCongViecDauTien,
                DanhSachFile = danhSachFile,
                StatusCheck = statusCheck,
                HasApprovedBudget = hasApprovedBudget,
                SoNgayConLai = soNgayConLai,
                IsSapDenHan = isSapDenHan,
                IsQuaHan = deadlineSource.IsQuaHan,
                IsHoanThanhTre = deadlineSource.IsHoanThanhTre,
                IsHoanThanhDungHan = deadlineSource.IsHoanThanhDungHan,
                CoCongViecTre = deadlineSource.CoCongViecTre,
                IsConHan = deadlineSource.IsConHan,
                IsChuaXacDinh = deadlineSource.IsChuaXacDinh,
                IsKhongDanhGia = deadlineSource.IsKhongDanhGia,
                SoNgayTre = deadlineSource.SoNgayTre,
                SoCongViecTre = deadlineSource.SoCongViecTre,
                MaTinhTrangThoiHan = deadlineSource.MaTinhTrangThoiHan,
                TinhTrangThoiHan = deadlineSource.TinhTrangThoiHan,
                CssTinhTrangThoiHan = deadlineSource.CssTinhTrangThoiHan,
                TienDoCongViec = new DuAnWorkStatusSummaryViewModel
                {
                    TongCongViec = soLuongCongViec,
                    CongViecHoanThanh = congViecHoanThanh,
                    CongViecDangThucHien = congViecDangThucHien,
                    CongViecTreHan = congViecTreHan,
                    CongViecDangQuaHan = congViecDangQuaHan,
                    CongViecHoanThanhDungHan = congViecHoanThanhDungHan,
                    CongViecHoanThanhTre = congViecHoanThanhTre,
                    CongViecHoanThanhThieuDuLieuNgay = congViecHoanThanhThieuDuLieuNgay,
                    CongViecBiCanTro = congViecBiCanTro,
                    CongViecChoXacNhan = congViecChoXacNhan,
                    CongViecTamDung = congViecTamDung,
                    CongViecChuaBatDau = congViecChuaBatDau,
                    TiLeHoanThanh = tiLeHoanThanh,
                    TiLeHoanThanhCap = tiLeHoanThanhCap,
                    TyLeHoanThanhDungHan = tyLeHoanThanhDungHan,
                    TyLeHoanThanhDungHanCap = tyLeHoanThanhDungHanCap
                },
                NganSachTongHop = new DuAnBudgetSummaryViewModel
                {
                    TongNganSachDaDuyet = tongNganSachDaDuyetDisplay,
                    TongChiPhiDaDung = tongChiPhiDaDung,
                    SoTienConLai = soTienConLai,
                    PhanTramSuDung = phanTramSuDung,
                    PhanTramSuDungCap = phanTramCap,
                    TrangThaiHienThi = trangThaiNganSachHienThi,
                    TrangThaiCss = trangThaiNganSachCss,
                    CoNganSachDaDuyet = hasApprovedBudget,
                    CoChiPhi = coChiPhi,
                    VuotNganSach = vuotNganSach,
                    SapVuotNganSach = sapVuotNganSach
                },
                DeadlineGanNhat = deadlineGanNhat,
                CongViecGanDay = congViecGanDay,
                TepGanDay = tepGanDay,
                ThanhVienNoiBat = thanhVienNoiBat,
                HoatDongGanDay = hoatDongGanDay,
                CoTheYeuCauDoiQuanLy = coTheYeuCauDoiQuanLy,
                LyDoKhongTheYeuCauDoiQuanLy = lyDoKhongTheYeuCauDoiQuanLy
            };
        }

        public async Task<DuAnCreateUpdateViewModel?> GetByIdAsync(int id)
        {
            var entity = await _context.DuAn
                .Where(x => x.MaDuAn == id && x.IsDeleted != true)
                .FirstOrDefaultAsync();

            if (entity == null)
                return null;

            var tenNguoiQuanLy = await _context.NguoiDung
                .Where(x => x.MaNguoiDung == entity.MaNguoiDung && x.IsDeleted != true)
                .Select(x => x.HoTenNguoiDung)
                .FirstOrDefaultAsync();

            var vm = new DuAnCreateUpdateViewModel
            {
                MaDuAn = entity.MaDuAn,
                TenDuAn = entity.TenDuAn ?? string.Empty,
                MoTaDuAn = entity.MoTaDuAn,
                MaNguoiDung = entity.MaNguoiDung,
                TenNguoiQuanLy = tenNguoiQuanLy ?? string.Empty,
                MaLoaiDuAn = entity.MaLoaiDuAn,
                NgayBatDauDuAn = entity.NgayBatDauDuAn,
                NgayKetThucDuAn = entity.NgayKetThucDuAn,
                NgayHoanThanhThucTeDuAn = entity.NgayHoanThanhThucTeDuAn,
                TrangThaiDuAn = TrangThai.ToCode(entity.TrangThaiDuAn ?? TrangThai.KhoiTao),
                GhiChuDuAn = entity.GhiChuDuAn
            };

            // Get status check info for UI
            vm.StatusCheck = await CheckProjectStatusAsync(id);

            return vm;
        }

        public async Task<List<LoaiDuAnOptionViewModel>> GetLoaiDuAnOptionsAsync()
        {
            return await _context.LoaiDuAn
                .OrderBy(x => x.TenLoai)
                .Select(x => new LoaiDuAnOptionViewModel
                {
                    MaLoaiDuAn = x.MaLoaiDuAn,
                    TenLoai = x.TenLoai ?? $"Loại dự án {x.MaLoaiDuAn}"
                })
                .ToListAsync();
        }

        public async Task SaveAsync(DuAnCreateUpdateViewModel model)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (!model.MaLoaiDuAn.HasValue)
                    throw new Exception("Vui lòng chọn loại dự án.");

                var maLoaiDuAn = model.MaLoaiDuAn.Value;
                var tenDuAn = (model.TenDuAn ?? string.Empty).Trim();
                var moTaDuAn = string.IsNullOrWhiteSpace(model.MoTaDuAn)
                    ? null
                    : model.MoTaDuAn.Trim();
                var trangThai = TrangThai.ToCode((model.TrangThaiDuAn ?? string.Empty).Trim());
                var maNguoiDung = model.MaNguoiDung ?? 0;
                var ghiChuDuAn = string.IsNullOrWhiteSpace(model.GhiChuDuAn) ? null : model.GhiChuDuAn.Trim();
                DuAn? duAnMoi = null;

                // Create mode
                if (model.MaDuAn == null)
                {
                    // Validate start date >= today
                    if (model.NgayBatDauDuAn.HasValue && model.NgayBatDauDuAn.Value.Date < DateTime.Today)
                        throw new Exception("Ngày bắt đầu không được nhỏ hơn ngày hôm nay.");

                    var currentUserId = await GetCurrentUserIdAsync();
                    model.MaNguoiDung = currentUserId;
                    maNguoiDung = currentUserId;
                    trangThai = TrangThai.KhoiTao;
                }
                else
                {
                    // Edit mode - validate manager permission and not completed
                    var existing = await _context.DuAn
                        .FirstOrDefaultAsync(x => x.MaDuAn == model.MaDuAn && x.IsDeleted != true);

                    if (existing == null)
                        throw new Exception("Không tìm thấy dự án.");

                    // Check manager permission
                    var currentUserId = await GetCurrentUserIdAsync();
                    await CheckManagerPermissionAsync(model.MaDuAn.Value, currentUserId);

                    // Cannot edit if completed, except the explicit safe archive transition.
                    if (TrangThai.EqualsValue(existing.TrangThaiDuAn, TrangThai.HoanThanh)
                        && !TrangThai.EqualsValue(trangThai, TrangThai.LuuTru))
                        throw new Exception("Dự án đã hoàn thành, không thể chỉnh sửa.");

                    maNguoiDung = existing.MaNguoiDung;

                    // Preserve status if not being changed
                    if (string.IsNullOrWhiteSpace(trangThai))
                        trangThai = TrangThai.ToCode(existing.TrangThaiDuAn ?? TrangThai.KhoiTao);

                    await ValidateStatusTransitionAsync(existing, trangThai, ghiChuDuAn);
                }

                // Validate loai exists
                var loaiExists = await _context.LoaiDuAn
                    .AnyAsync(x => x.MaLoaiDuAn == maLoaiDuAn);
                if (!loaiExists)
                    throw new Exception("Loại dự án không tồn tại.");

                // Validate GhiChuDuAn required for TamDung
                if (TrangThai.EqualsValue(trangThai, TrangThai.TamDung))
                {
                    if (string.IsNullOrWhiteSpace(ghiChuDuAn))
                        throw new Exception("Ghi chú lý do tạm dừng không được để trống.");
                }

                // Create or update entity
                if (model.MaDuAn == null)
                {
                    duAnMoi = new DuAn
                    {
                        MaNguoiDung = maNguoiDung,
                        MaLoaiDuAn = maLoaiDuAn,
                        TenDuAn = tenDuAn,
                        MoTaDuAn = moTaDuAn,
                        NgayTaoDuAn = DateTime.Now,
                        NgayBatDauDuAn = model.NgayBatDauDuAn,
                        NgayKetThucDuAn = model.NgayKetThucDuAn,
                        PhanTramHoanThanh = 0,
                        TrangThaiDuAn = trangThai,
                        GhiChuDuAn = ghiChuDuAn,
                        IsDeleted = false
                    };

                    _context.DuAn.Add(duAnMoi);
                }
                else
                {
                    var entity = await _context.DuAn
                        .FirstOrDefaultAsync(x => x.MaDuAn == model.MaDuAn && x.IsDeleted != true);

                    if (entity == null)
                        throw new Exception("Không tìm thấy dự án.");

                    entity.MaLoaiDuAn = maLoaiDuAn;
                    entity.TenDuAn = tenDuAn;
                    entity.MoTaDuAn = moTaDuAn;
                    entity.NgayBatDauDuAn = model.NgayBatDauDuAn;
                    entity.NgayKetThucDuAn = model.NgayKetThucDuAn;
                    entity.TrangThaiDuAn = trangThai;
                    entity.GhiChuDuAn = ghiChuDuAn;

                    if (TrangThai.EqualsValue(trangThai, TrangThai.HoanThanh))
                    {
                        entity.PhanTramHoanThanh = 100;
                        entity.NgayHoanThanhThucTeDuAn ??= DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();

                // Auto-check and transition if conditions met
                if (model.MaDuAn == null && duAnMoi != null)
                {
                    await _chatDuAnService.DamBaoPhongChatDuAnAsync(duAnMoi.MaDuAn);

                    _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
                    {
                        MaDuAn = duAnMoi.MaDuAn,
                        MaNguoiDung = maNguoiDung,
                        NkHanhDongQLDA = $"Tạo dự án: {duAnMoi.TenDuAn}",
                        NkThoiGianQLDA = DateTime.Now
                    });

                    await _context.SaveChangesAsync();
                    await CheckAutoTransitionAsync(duAnMoi.MaDuAn);
                }
                else if (model.MaDuAn.HasValue)
                {
                    await CheckAutoTransitionAsync(model.MaDuAn.Value);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task ValidateStatusTransitionAsync(DuAn existing, string trangThaiMoi, string? ghiChuDuAn)
        {
            var trangThaiHienTai = (existing.TrangThaiDuAn ?? string.Empty).Trim();
            var trangThaiMucTieu = (trangThaiMoi ?? string.Empty).Trim();

            if (string.Equals(trangThaiHienTai, trangThaiMucTieu, StringComparison.OrdinalIgnoreCase))
                return;

            if (TrangThai.EqualsValue(trangThaiHienTai, TrangThai.HoanThanh))
            {
                throw new Exception("Dự án đã hoàn thành, không thể thay đổi trạng thái.");
            }

            if (TrangThai.EqualsValue(trangThaiMucTieu, TrangThai.KhoiTao))
            {
                throw new Exception("Không thể chuyển dự án về trạng thái khởi tạo bằng nút Lưu.");
            }

            if (TrangThai.EqualsValue(trangThaiMucTieu, TrangThai.DangThucHien))
            {
                await ValidateCanStartProjectAsync(existing.MaDuAn);
                return;
            }

            if (TrangThai.EqualsValue(trangThaiMucTieu, TrangThai.ChoXacNhanHoanThanh))
            {
                await ValidateCompletionAsync(existing.MaDuAn);
                return;
            }

            if (TrangThai.EqualsValue(trangThaiMucTieu, TrangThai.HoanThanh))
            {
                await ValidateCompletionAsync(existing.MaDuAn);

                if (!TrangThai.EqualsValue(trangThaiHienTai, TrangThai.ChoXacNhanHoanThanh))
                {
                    throw new Exception("Chỉ có thể hoàn thành dự án khi đang ở trạng thái chờ xác nhận hoàn thành.");
                }

                return;
            }

            if (TrangThai.EqualsValue(trangThaiMucTieu, TrangThai.TamDung))
            {
                if (string.IsNullOrWhiteSpace(ghiChuDuAn))
                    throw new Exception("Ghi chú lý do tạm dừng không được để trống.");

                return;
            }

            if (TrangThai.EqualsValue(trangThaiMucTieu, TrangThai.LuuTru))
            {
                if (!TrangThai.EqualsValue(trangThaiHienTai, TrangThai.HoanThanh))
                {
                    throw new Exception("Chỉ có thể lưu trữ dự án đã hoàn thành.");
                }

                if (!DuAnHoanThanhDungHan(existing))
                {
                    throw new Exception("Chỉ có thể lưu trữ dự án hoàn thành đúng hạn và đã có ngày hoàn thành thực tế.");
                }

                return;
            }

            throw new Exception("Trạng thái dự án không hợp lệ.");
        }

        private async Task ValidateCanStartProjectAsync(int maDuAn)
        {
            var hasMembers = await _context.NhanVienDuAn.AnyAsync(x => x.MaDuAn == maDuAn);
            var hasCategories = await _context.DanhMucCongViec.AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);
            var hasWorkItems = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV,
                    (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && x.cv.IsDeleted != true);

            if (!hasMembers)
                throw new Exception("Dự án phải có ít nhất 1 thành viên.");

            if (!hasCategories)
                throw new Exception("Dự án phải có danh mục công việc.");

            if (!hasWorkItems)
                throw new Exception("Dự án phải có ít nhất 1 công việc.");
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == id && x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy dự án.");

            // Check manager permission
            var currentUserId = await GetCurrentUserIdAsync();
            await CheckManagerPermissionAsync(id, currentUserId);

            // Validate can delete
            await ValidateDeleteAsync(id);

            // Soft delete
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;
            entity.DeletedBy = currentUserId;

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Status Workflow Methods

        public async Task<ProjectStatusCheckViewModel> CheckProjectStatusAsync(int maDuAn)
        {
            var project = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (project == null)
                throw new Exception("Không tìm thấy dự án.");

            var result = new ProjectStatusCheckViewModel
            {
                MaDuAn = maDuAn,
                TrangThaiDuAn = TrangThai.ToCode(project.TrangThaiDuAn ?? TrangThai.KhoiTao),
                PhanTramHoanThanh = project.PhanTramHoanThanh ?? 0
            };

            // Check conditions
            result.HasMembers = await _context.NhanVienDuAn.AnyAsync(x => x.MaDuAn == maDuAn);
            result.HasCategories = await _context.DanhMucCongViec.AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);
            result.HasWorkItems = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && x.cv.IsDeleted != true);

            var trangThaiHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
            var trangThaiDangThucHien = TrangThai.GetCommonStatusVariants(TrangThai.DangThucHien);
            var trangThaiBiCanCan = TrangThai.GetCommonStatusVariants(TrangThai.BiCanCan);

            // Check all tasks done
            var totalTasks = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .Where(x => x.dmcv.MaDuAn == maDuAn && x.cv.IsDeleted != true)
                .CountAsync();

            var completedTasks = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .Where(x => x.dmcv.MaDuAn == maDuAn && 
                            x.cv.IsDeleted != true &&
                            trangThaiHoanThanh.Contains(x.cv.TrangThaiCongViec ?? string.Empty))
                .CountAsync();

            //result.AllTasksDone = totalTasks > 0 && completedTasks == totalTasks && project.PhanTramHoanThanh == 100;
            result.AllTasksDone = totalTasks > 0 && completedTasks == totalTasks;

            // Check ongoing tasks
            result.HasOngoingTasks = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && 
                               x.cv.IsDeleted != true &&
                               trangThaiDangThucHien.Contains(x.cv.TrangThaiCongViec ?? string.Empty));

            // Check blocked tasks
            result.HasBlockedTasks = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && 
                               x.cv.IsDeleted != true &&
                               trangThaiBiCanCan.Contains(x.cv.TrangThaiCongViec ?? string.Empty));

            // Determine allowed transitions based on current status
            var currentStatus = project.TrangThaiDuAn ?? TrangThai.KhoiTao;

            if (TrangThai.EqualsValue(currentStatus, TrangThai.KhoiTao))
            {
                result.CanTransitionToDangThucHien = result.HasMembers && result.HasCategories && result.HasWorkItems;
                result.CanDelete = await CanDeleteNoRelatedDataAsync(maDuAn);
                result.CanPause = false;
                result.CanRequestCompletion = false;
            }
            else if (TrangThai.EqualsValue(currentStatus, TrangThai.DangThucHien))
            {
                result.CanTransitionToDangThucHien = false;
                result.CanDelete = false;
                result.CanPause = true;
                result.CanRequestCompletion = result.AllTasksDone && !result.HasOngoingTasks && !result.HasBlockedTasks;
            }
            else if (TrangThai.EqualsValue(currentStatus, TrangThai.ChoXacNhanHoanThanh))
            {
                result.IsInChoXacNhanHoanThanh = true;
                result.CanConfirmCompletion = result.AllTasksDone && !result.HasOngoingTasks && !result.HasBlockedTasks;
                result.CanDelete = false;
                result.CanPause = false;
            }
            else if (TrangThai.EqualsValue(currentStatus, TrangThai.HoanThanh))
            {
                result.IsCompleted = true;
                result.CanReopen = true;
                result.CanArchive = DuAnHoanThanhDungHan(project);
                result.CanDelete = false;
                result.CanPause = false;
                result.CanRequestCompletion = false;
            }
            else if (TrangThai.EqualsValue(currentStatus, TrangThai.TamDung))
            {
                result.CanDelete = false;
                result.CanPause = false;
                result.CanTransitionToDangThucHien = true;
            }

            return result;
        }

        public async Task CheckAutoTransitionAsync(int maDuAn)
        {
            var project = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (project == null)
                throw new Exception("Không tìm thấy dự án.");

            // Only auto-transition from KhoiTao to DangThucHien
            if (!TrangThai.EqualsValue(project.TrangThaiDuAn, TrangThai.KhoiTao))
                return;

            var hasMembers = await _context.NhanVienDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn);

            var hasCategories = await _context.DanhMucCongViec
                .AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            var hasWorkItems = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && x.cv.IsDeleted != true);

            if (hasMembers && hasCategories && hasWorkItems)
            {
                project.TrangThaiDuAn = TrangThai.DangThucHien;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CheckManagerPermissionAsync(int maDuAn, int currentUserId)
        {
            var project = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (project == null)
                throw new Exception("Không tìm thấy dự án.");

            if (project.MaNguoiDung != currentUserId)
                throw new Exception("Bạn không có quyền thao tác dự án này.");
        }

        public async Task ValidateDeleteAsync(int maDuAn)
        {
            var project = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (project == null)
                throw new Exception("Không tìm thấy dự án.");

            // Only allow delete if status is KhoiTao
            if (!TrangThai.EqualsValue(project.TrangThaiDuAn, TrangThai.KhoiTao))
                throw new Exception("Chỉ có thể xóa dự án ở trạng thái 'Khởi tạo'.");

            // Check for related data
            if (!await CanDeleteNoRelatedDataAsync(maDuAn))
                throw new Exception("Dự án đã phát sinh dữ liệu, không thể xóa.");
        }

        public async Task ValidateCompletionAsync(int maDuAn)
        {
            var project = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (project == null)
                throw new Exception("Không tìm thấy dự án.");

            var trangThaiHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
            var trangThaiDangThucHien = TrangThai.GetCommonStatusVariants(TrangThai.DangThucHien);
            var trangThaiBiCanCan = TrangThai.GetCommonStatusVariants(TrangThai.BiCanCan);

            // Check all tasks done
            var totalTasks = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .Where(x => x.dmcv.MaDuAn == maDuAn && x.cv.IsDeleted != true)
                .CountAsync();

            var completedTasks = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .Where(x => x.dmcv.MaDuAn == maDuAn && 
                            x.cv.IsDeleted != true &&
                            trangThaiHoanThanh.Contains(x.cv.TrangThaiCongViec ?? string.Empty))
                .CountAsync();

            if (!(totalTasks > 0 && completedTasks == totalTasks))
                throw new Exception("Chưa hoàn thành tất cả công việc.");

            // Check no ongoing tasks
            var hasOngoing = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && 
                               x.cv.IsDeleted != true &&
                               trangThaiDangThucHien.Contains(x.cv.TrangThaiCongViec ?? string.Empty));

            if (hasOngoing)
                throw new Exception("Còn công việc đang thực hiện.");

            // Check no blocked tasks
            var hasBlocked = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && 
                               x.cv.IsDeleted != true &&
                               trangThaiBiCanCan.Contains(x.cv.TrangThaiCongViec ?? string.Empty));

            if (hasBlocked)
                throw new Exception("Có công việc bị cản cản.");

            var hasBudget = await _context.NganSach
                .AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (!hasBudget)
                throw new Exception("Dự án chưa có ngân sách, không thể hoàn thành.");
        }

        public async Task TransitionToDangThucHienAsync(int maDuAn)
        {
            var project = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (project == null)
                throw new Exception("Không tìm thấy dự án.");

            // Check manager permission
            var currentUserId = await GetCurrentUserIdAsync();
            await CheckManagerPermissionAsync(maDuAn, currentUserId);

            // Validate current status is KhoiTao
            if (!TrangThai.EqualsValue(project.TrangThaiDuAn, TrangThai.KhoiTao))
                throw new Exception("Chỉ có thể bắt đầu dự án ở trạng thái 'Khởi tạo'.");

            // Validate conditions
            var hasMembers = await _context.NhanVienDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn);

            var hasCategories = await _context.DanhMucCongViec
                .AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            var hasWorkItems = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && x.cv.IsDeleted != true);

            if (!hasMembers)
                throw new Exception("Dự án phải có ít nhất 1 thành viên.");

            if (!hasCategories)
                throw new Exception("Dự án phải có danh mục công việc.");

            if (!hasWorkItems)
                throw new Exception("Dự án phải có ít nhất 1 công việc.");

            project.TrangThaiDuAn = TrangThai.DangThucHien;
            await _context.SaveChangesAsync();
        }

        public async Task RequestCompletionAsync(int maDuAn)
        {
            var project = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (project == null)
                throw new Exception("Không tìm thấy dự án.");

            // Check manager permission
            var currentUserId = await GetCurrentUserIdAsync();
            await CheckManagerPermissionAsync(maDuAn, currentUserId);

            if (!TrangThai.EqualsValue(project.TrangThaiDuAn, TrangThai.DangThucHien))
                throw new Exception("Chỉ có thể yêu cầu hoàn thành khi dự án đang ở trạng thái đang thực hiện.");

            // Validate completion conditions
            await ValidateCompletionAsync(maDuAn);

            // Transition to intermediate state
            project.TrangThaiDuAn = TrangThai.ChoXacNhanHoanThanh;
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmCompletionAsync(int maDuAn)
        {
            var project = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (project == null)
                throw new Exception("Không tìm thấy dự án.");

            // Check manager permission
            var currentUserId = await GetCurrentUserIdAsync();
            await CheckManagerPermissionAsync(maDuAn, currentUserId);

            // Must be in ChoXacNhanHoanThanh state
            if (!TrangThai.EqualsValue(project.TrangThaiDuAn, TrangThai.ChoXacNhanHoanThanh))
                throw new Exception("Dự án phải ở trạng thái chờ xác nhận hoàn thành.");

            // Validate completion conditions again
            await ValidateCompletionAsync(maDuAn);

            // Transition to completed
            project.TrangThaiDuAn = TrangThai.HoanThanh;
            project.NgayHoanThanhThucTeDuAn = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task MoLaiDuAnAsync(int maDuAn, string lyDo)
        {
            if (string.IsNullOrWhiteSpace(lyDo))
                throw new Exception("Vui lòng nhập lý do mở lại dự án.");

            var project = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (project == null)
                throw new Exception("Không tìm thấy dự án.");

            var currentUserId = await GetCurrentUserIdAsync();
            await CheckManagerPermissionAsync(maDuAn, currentUserId);

            if (!TrangThai.LaHoanThanhCongViec(project.TrangThaiDuAn))
                throw new Exception("Chỉ có thể mở lại dự án khi dự án đang ở trạng thái hoàn thành.");

            project.TrangThaiDuAn = TrangThai.DangThucHien;
            project.NgayHoanThanhThucTeDuAn = null;
            project.GhiChuDuAn = lyDo.Trim();

            var now = DateTime.Now;
            var xacNhanNguyenNhanCu = await _context.AiNguyenNhan
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true)
                .ToListAsync();
            foreach (var item in xacNhanNguyenNhanCu)
            {
                item.IsDeleted = true;
                item.DeletedAt = now;
                item.DeletedBy = currentUserId;
            }

            var datasets = await _context.AiDataset
                .Where(x => x.MaDuAn == maDuAn && x.MaDMNguyenNhan.HasValue)
                .ToListAsync();
            foreach (var dataset in datasets)
            {
                dataset.MaDMNguyenNhan = null;
                dataset.GhiChuDataset = "Đã bỏ nhãn xác nhận do dự án được mở lại.";
            }

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = maDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Mở lại dự án từ trạng thái hoàn thành. Lý do: {lyDo.Trim()}",
                NkThoiGianQLDA = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        public async Task PauseProjectAsync(int maDuAn, string ghiChuDuAn)
        {
            if (string.IsNullOrWhiteSpace(ghiChuDuAn))
                throw new Exception("Ghi chú lý do tạm dừng không được để trống.");

            var project = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (project == null)
                throw new Exception("Không tìm thấy dự án.");

            // Check manager permission
            var currentUserId = await GetCurrentUserIdAsync();
            await CheckManagerPermissionAsync(maDuAn, currentUserId);

            // Cannot pause if already completed
            if (TrangThai.EqualsValue(project.TrangThaiDuAn, TrangThai.HoanThanh))
                throw new Exception("Không thể tạm dừng dự án đã hoàn thành.");

            project.TrangThaiDuAn = TrangThai.TamDung;
            project.GhiChuDuAn = ghiChuDuAn.Trim();
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Helper Methods

        private static void GanTinhTrangThoiHanCongViec(DuAnRecentWorkItemViewModel congViec, DateTime homNay)
        {
            var laHoanThanh = TrangThai.LaHoanThanhCongViec(congViec.TrangThaiCongViec);
            var laChoXacNhan = TrangThai.EqualsValue(congViec.TrangThaiCongViec, TrangThai.ChoXacNhanHoanThanh);

            if (TrangThai.EqualsValue(congViec.TrangThaiCongViec, TrangThai.DaHuy))
            {
                congViec.MaTinhTrangThoiHan = "khong-danh-gia";
                congViec.TinhTrangThoiHan = "Không đánh giá";
                congViec.CssTinhTrangThoiHan = "is-neutral";
                return;
            }

            if (laHoanThanh)
            {
                GanKetQuaHoanThanhCongViec(congViec, "Hoàn thành");
                return;
            }

            if (laChoXacNhan && congViec.NgayKetThucThucTe.HasValue)
            {
                GanKetQuaHoanThanhCongViec(congViec, "Hoàn tất");
                if (congViec.CssTinhTrangThoiHan == "is-completed-on-time")
                {
                    congViec.TinhTrangThoiHan = "Hoàn tất đúng hạn, chờ xác nhận";
                    congViec.CssTinhTrangThoiHan = "is-pending";
                }
                else if (congViec.CssTinhTrangThoiHan == "is-completed-late")
                {
                    congViec.TinhTrangThoiHan = $"Hoàn tất trễ {congViec.SoNgayTre} ngày, chờ xác nhận";
                    congViec.CssTinhTrangThoiHan = "is-pending-late";
                }

                return;
            }

            if (!congViec.NgayKetThucDuKien.HasValue)
            {
                congViec.MaTinhTrangThoiHan = "thieu-du-lieu";
                congViec.TinhTrangThoiHan = "Chưa có hạn";
                congViec.CssTinhTrangThoiHan = "is-missing";
                return;
            }

            var soGioConLai = (congViec.NgayKetThucDuKien.Value - homNay).TotalHours;
            if (soGioConLai < 0)
            {
                congViec.SoNgayTre = TinhSoNgayTre(congViec.NgayKetThucDuKien.Value, homNay);
                congViec.MaTinhTrangThoiHan = "dang-qua-han";
                congViec.TinhTrangThoiHan = $"Đang quá hạn {congViec.SoNgayTre} ngày";
                congViec.CssTinhTrangThoiHan = "is-overdue";
                return;
            }

            if (soGioConLai < 24)
            {
                congViec.MaTinhTrangThoiHan = "den-han-hom-nay";
                congViec.TinhTrangThoiHan = "Đến hạn hôm nay";
                congViec.CssTinhTrangThoiHan = "is-near";
                return;
            }

            congViec.MaTinhTrangThoiHan = "con-han";
            congViec.TinhTrangThoiHan = "Còn hạn";
            congViec.CssTinhTrangThoiHan = "is-on-track";
        }

        private static void GanKetQuaHoanThanhCongViec(DuAnRecentWorkItemViewModel congViec, string prefix)
        {
            if (!congViec.NgayKetThucDuKien.HasValue || !congViec.NgayKetThucThucTe.HasValue)
            {
                congViec.MaTinhTrangThoiHan = "thieu-du-lieu";
                congViec.TinhTrangThoiHan = "Chưa đủ dữ liệu";
                congViec.CssTinhTrangThoiHan = "is-missing";
                return;
            }

            var soNgayTre = TinhSoNgayTre(congViec.NgayKetThucDuKien.Value, congViec.NgayKetThucThucTe.Value);
            if (soNgayTre <= 0)
            {
                congViec.MaTinhTrangThoiHan = "hoan-thanh-dung-han";
                congViec.TinhTrangThoiHan = $"{prefix} đúng hạn";
                congViec.CssTinhTrangThoiHan = "is-completed-on-time";
                return;
            }

            congViec.SoNgayTre = soNgayTre;
            congViec.MaTinhTrangThoiHan = "hoan-thanh-tre";
            congViec.TinhTrangThoiHan = $"{prefix} trễ {soNgayTre} ngày";
            congViec.CssTinhTrangThoiHan = "is-completed-late";
        }

        private static int TinhSoNgayTre(DateTime han, DateTime thucTe)
        {
            var soNgay = (thucTe - han).TotalDays;
            return soNgay > 0 ? Math.Max(1, (int)Math.Ceiling(soNgay)) : 0;
        }

        private IQueryable<DuAnViewModel> ApplyDeadlineFilter(
            IQueryable<DuAnViewModel> query,
            string? locTinhTrangThoiHan,
            DateTime now)
        {
            var filter = DuAnDeadlineStatusHelper.NormalizeFilter(locTinhTrangThoiHan);
            if (filter == null)
            {
                return query;
            }

            var trangThaiHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);
            var trangThaiLuuTru = TrangThai.GetCommonStatusVariants(TrangThai.LuuTru);
            var trangThaiDaHuy = TrangThai.GetCommonStatusVariants(TrangThai.DaHuy);
            var maDuAnCoCongViecTre = TaoQueryMaDuAnCoCongViecTre(now);
            var maDuAnCoCongViecVuotHanDuAn = TaoQueryMaDuAnCoCongViecVuotHanDuAn();

            return filter switch
            {
                DuAnDeadlineStatusHelper.FilterDangQuaHan => query.Where(x =>
                    x.NgayKetThucDuAn.HasValue
                    && x.NgayKetThucDuAn.Value < now
                    && !trangThaiHoanThanh.Contains(x.TrangThaiDuAn)
                    && !trangThaiLuuTru.Contains(x.TrangThaiDuAn)
                    && !trangThaiDaHuy.Contains(x.TrangThaiDuAn)),

                DuAnDeadlineStatusHelper.FilterCoCongViecTre => query.Where(x =>
                    x.NgayKetThucDuAn.HasValue
                    && x.NgayKetThucDuAn.Value >= now
                    && !trangThaiHoanThanh.Contains(x.TrangThaiDuAn)
                    && !trangThaiLuuTru.Contains(x.TrangThaiDuAn)
                    && !trangThaiDaHuy.Contains(x.TrangThaiDuAn)
                    && maDuAnCoCongViecTre.Contains(x.MaDuAn)),

                DuAnDeadlineStatusHelper.FilterHoanThanhTre => query.Where(x =>
                    x.NgayKetThucDuAn.HasValue
                    && x.NgayHoanThanhThucTeDuAn.HasValue
                    && x.NgayHoanThanhThucTeDuAn.Value > x.NgayKetThucDuAn.Value
                    && (trangThaiHoanThanh.Contains(x.TrangThaiDuAn)
                        || trangThaiLuuTru.Contains(x.TrangThaiDuAn))
                    && maDuAnCoCongViecTre.Contains(x.MaDuAn)
                    && maDuAnCoCongViecVuotHanDuAn.Contains(x.MaDuAn)),

                DuAnDeadlineStatusHelper.FilterHoanThanhDungHan => query.Where(x =>
                    x.NgayKetThucDuAn.HasValue
                    && x.NgayHoanThanhThucTeDuAn.HasValue
                    && x.NgayHoanThanhThucTeDuAn.Value <= x.NgayKetThucDuAn.Value
                    && (trangThaiHoanThanh.Contains(x.TrangThaiDuAn)
                        || trangThaiLuuTru.Contains(x.TrangThaiDuAn))),

                DuAnDeadlineStatusHelper.FilterConHan => query.Where(x =>
                    x.NgayKetThucDuAn.HasValue
                    && x.NgayKetThucDuAn.Value >= now
                    && !trangThaiHoanThanh.Contains(x.TrangThaiDuAn)
                    && !trangThaiLuuTru.Contains(x.TrangThaiDuAn)
                    && !trangThaiDaHuy.Contains(x.TrangThaiDuAn)
                    && !maDuAnCoCongViecTre.Contains(x.MaDuAn)),

                _ => query
            };
        }

        private IQueryable<int> TaoQueryMaDuAnCoCongViecTre(DateTime now)
        {
            var trangThaiHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);

            return (from cv in _context.CongViec
                    join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                    where cv.IsDeleted != true
                          && dm.IsDeleted != true
                          && cv.NgayKetThucCVDuKien.HasValue
                          && ((trangThaiHoanThanh.Contains(cv.TrangThaiCongViec ?? string.Empty)
                               && cv.NgayKetThucCVThucTe.HasValue
                               && cv.NgayKetThucCVThucTe.Value > cv.NgayKetThucCVDuKien.Value)
                              || (!trangThaiHoanThanh.Contains(cv.TrangThaiCongViec ?? string.Empty)
                                  && cv.NgayKetThucCVDuKien.Value < now))
                    select dm.MaDuAn).Distinct();
        }

        private IQueryable<int> TaoQueryMaDuAnCoCongViecVuotHanDuAn()
        {
            var trangThaiHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);

            return (from cv in _context.CongViec
                    join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                    join da in _context.DuAn on dm.MaDuAn equals da.MaDuAn
                    where cv.IsDeleted != true
                          && dm.IsDeleted != true
                          && da.IsDeleted != true
                          && da.NgayKetThucDuAn.HasValue
                          && cv.NgayKetThucCVThucTe.HasValue
                          && trangThaiHoanThanh.Contains(cv.TrangThaiCongViec ?? string.Empty)
                          && cv.NgayKetThucCVThucTe.Value > da.NgayKetThucDuAn.Value
                    select dm.MaDuAn).Distinct();
        }

        private async Task GanTinhTrangThoiHanAsync(List<DuAnViewModel> items)
        {
            if (items.Count == 0)
            {
                return;
            }

            var now = DateTime.Now;
            var maDuAn = items.Select(x => x.MaDuAn).Distinct().ToList();
            var trangThaiHoanThanh = TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh);

            var soCongViecTreTheoDuAn = await (from cv in _context.CongViec
                                               join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                                               where maDuAn.Contains(dm.MaDuAn)
                                                     && cv.IsDeleted != true
                                                     && dm.IsDeleted != true
                                                     && cv.NgayKetThucCVDuKien.HasValue
                                                     && ((trangThaiHoanThanh.Contains(cv.TrangThaiCongViec ?? string.Empty)
                                                          && cv.NgayKetThucCVThucTe.HasValue
                                                          && cv.NgayKetThucCVThucTe.Value > cv.NgayKetThucCVDuKien.Value)
                                                         || (!trangThaiHoanThanh.Contains(cv.TrangThaiCongViec ?? string.Empty)
                                                             && cv.NgayKetThucCVDuKien.Value < now))
                                               group cv by dm.MaDuAn into g
                                               select new
                                               {
                                                   MaDuAn = g.Key,
                                                   SoCongViecTre = g.Count()
                                               })
                .ToDictionaryAsync(x => x.MaDuAn, x => x.SoCongViecTre);

            var soCongViecVuotHanTheoDuAn = await (from cv in _context.CongViec
                                                   join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                                                   join da in _context.DuAn on dm.MaDuAn equals da.MaDuAn
                                                   where maDuAn.Contains(dm.MaDuAn)
                                                         && cv.IsDeleted != true
                                                         && dm.IsDeleted != true
                                                         && da.IsDeleted != true
                                                         && da.NgayKetThucDuAn.HasValue
                                                         && cv.NgayKetThucCVThucTe.HasValue
                                                         && trangThaiHoanThanh.Contains(cv.TrangThaiCongViec ?? string.Empty)
                                                         && cv.NgayKetThucCVThucTe.Value > da.NgayKetThucDuAn.Value
                                                   group cv by dm.MaDuAn into g
                                                   select new
                                                   {
                                                       MaDuAn = g.Key,
                                                       SoCongViecVuotHanDuAn = g.Count()
                                                   })
                .ToDictionaryAsync(x => x.MaDuAn, x => x.SoCongViecVuotHanDuAn);

            foreach (var item in items)
            {
                item.SoCongViecTre = soCongViecTreTheoDuAn.TryGetValue(item.MaDuAn, out var soCongViecTre)
                    ? soCongViecTre
                    : 0;
                item.SoCongViecVuotHanDuAn = soCongViecVuotHanTheoDuAn.TryGetValue(item.MaDuAn, out var soCongViecVuotHanDuAn)
                    ? soCongViecVuotHanDuAn
                    : 0;
                DuAnDeadlineStatusHelper.Apply(item, now);
            }
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var userId = GetCurrentAspUserId();

            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("Không xác định được người dùng hiện tại.");

            var currentUser = await _context.Aspnetusers
                .Where(x => x.Id == userId)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync();

            if (currentUser <= 0)
                throw new Exception("Không xác định được nhân sự tương ứng của người dùng hiện tại.");

            return currentUser;
        }

        private int? TryGetCurrentUserIdFromClaims()
        {
            var claimValue = _httpContextAccessor.HttpContext?.User?.FindFirst("MaNguoiDung")?.Value;
            if (int.TryParse(claimValue, out var currentUserId) && currentUserId > 0)
            {
                return currentUserId;
            }

            return null;
        }

        private bool HasCurrentUserPermission(string permission)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(permission))
            {
                return false;
            }

            foreach (var claim in user.Claims)
            {
                if (claim is null || string.IsNullOrWhiteSpace(claim.Value))
                {
                    continue;
                }

                var claimType = claim.Type ?? string.Empty;
                if (!claimType.Contains(Permissions.ClaimTypesCustom.Permission, StringComparison.OrdinalIgnoreCase)
                    && !claimType.Contains("claim", StringComparison.OrdinalIgnoreCase)
                    && !claimType.Contains("quyen", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.Equals(claim.Value.Trim(), permission, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsCurrentUserInRole(string roleName)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(roleName))
            {
                return false;
            }

            return user.IsInRole(roleName) || user.IsInRole(roleName.ToUpperInvariant());
        }

        private string GetCurrentAspUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("Không xác định được người dùng hiện tại.");

            return userId;
        }

        private async Task<(bool IsManager, bool IsEmployee)> GetCurrentUserRoleFlagsAsync()
        {
            var aspUserId = GetCurrentAspUserId();

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

            return (normalizedRoles.Contains("MANAGER"), normalizedRoles.Contains("EMPLOYEE"));
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

        private static bool IsProjectStatusAllowedForManagerRequest(string? trangThaiDuAn)
        {
            if (TrangThai.LaHoanThanhCongViec(trangThaiDuAn)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.DaHuy)
                || TrangThai.EqualsValue(trangThaiDuAn, TrangThai.LuuTru))
            {
                return false;
            }

            return true;
        }

        private static bool DuAnHoanThanhDungHan(DuAn duAn)
        {
            return TrangThai.LaHoanThanhCongViec(duAn.TrangThaiDuAn)
                && duAn.NgayKetThucDuAn.HasValue
                && duAn.NgayHoanThanhThucTeDuAn.HasValue
                && duAn.NgayHoanThanhThucTeDuAn.Value <= duAn.NgayKetThucDuAn.Value;
        }

        private async Task<bool> CanDeleteNoRelatedDataAsync(int maDuAn)
        {
            // Check for any related data
            var hasCongViec = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && x.cv.IsDeleted != true);

            if (hasCongViec)
                return false;

            var hasDeXuat = await _context.DeXuatCongViec
                .AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (hasDeXuat)
                return false;

            var hasNganSach = await _context.NganSach
                .AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (hasNganSach)
                return false;

            // ChiPhi is related through CongViec → DanhMucCongViec → DuAn
            var hasChiPhi = await _context.ChiPhi
                .Join(_context.CongViec, cp => cp.MaCongViec, cv => cv.MaCongViec,
                      (cp, cv) => new { cp, cv })
                .Join(_context.DanhMucCongViec, x => x.cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV,
                      (x, dmcv) => new { x.cp, x.cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && x.cp.IsDeleted != true);

            if (hasChiPhi)
                return false;

            var hasAiDataset = await _context.AiDataset
                .AnyAsync(x => x.MaDuAn == maDuAn);

            if (hasAiDataset)
                return false;

            var hasNhatKy = await _context.NhatKyDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn);

            if (hasNhatKy)
                return false;

            // NhanVienDuAn, TeamDuAn, DanhMucCongViec don't have IsDeleted
            var hasNhanVien = await _context.NhanVienDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn);

            if (hasNhanVien)
                return false;

            var hasTeam = await _context.TeamDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn);

            if (hasTeam)
                return false;

            var hasDanhMuc = await _context.DanhMucCongViec
                .AnyAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (hasDanhMuc)
                return false;

            return true;
        }

        #endregion
    }
}

