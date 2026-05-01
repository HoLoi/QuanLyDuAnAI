using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DeXuatCongViec;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class DeXuatCongViecService : IDeXuatCongViecService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeXuatCongViecService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DeXuatCongViecPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai)
        {
            if (!locMaDuAn.HasValue)
                throw new Exception("Vui lòng chọn dự án.");

            var currentUserId = await GetCurrentUserIdAsync();
            var projectOptions = await GetEligibleProjectOptionsAsync(currentUserId);
            var selectedProject = projectOptions.FirstOrDefault(x => x.MaDuAn == locMaDuAn.Value);

            if (selectedProject == null)
                throw new Exception("Bạn không có quyền truy cập dự án để đề xuất công việc.");

            var danhSachDanhMuc = await _context.DanhMucCongViec
                .Where(x => x.MaDuAn == locMaDuAn.Value && x.IsDeleted != true)
                .OrderBy(x => x.TenDanhMucCV)
                .Select(x => new DeXuatCongViecDanhMucOptionViewModel
                {
                    MaDanhMucCV = x.MaDanhMucCV,
                    TenDanhMucCV = x.TenDanhMucCV ?? $"Danh mục {x.MaDanhMucCV}"
                })
                .ToListAsync();

            var danhSachMucDo = await _context.MucDoUuTien
                .OrderBy(x => x.MaMucDo)
                .Select(x => new DeXuatCongViecMucDoOptionViewModel
                {
                    MaMucDo = x.MaMucDo,
                    TenMucDo = x.TenMucDo ?? $"Mức độ {x.MaMucDo}"
                })
                .ToListAsync();

            var query =
                from dx in _context.DeXuatCongViec
                join da in _context.DuAn on dx.MaDuAn equals da.MaDuAn
                join ndDeXuat in _context.NguoiDung on dx.MaNguoiDungDeXuat equals ndDeXuat.MaNguoiDung
                join ndDuyet in _context.NguoiDung on dx.MaNguoiDungDuyet equals ndDuyet.MaNguoiDung into ndDuyetLeft
                from ndDuyet in ndDuyetLeft.DefaultIfEmpty()
                where dx.IsDeleted != true && da.IsDeleted != true
                select new DeXuatCongViecItemViewModel
                {
                    MaDeXuatCV = dx.MaDeXuatCV,
                    MaDuAn = dx.MaDuAn,
                    TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}",
                    MaNguoiDungDeXuat = dx.MaNguoiDungDeXuat,
                    NguoiDungDeXuat = ndDeXuat.HoTenNguoiDung ?? $"Nhân viên {dx.MaNguoiDungDeXuat}",
                    MaNguoiDungDuyet = dx.MaNguoiDungDuyet,
                    NguoiDungDuyet = ndDuyet != null ? (ndDuyet.HoTenNguoiDung ?? $"Nhân viên {ndDuyet.MaNguoiDung}") : null,
                    TenCongViecDeXuat = dx.TenCongViecDeXuat ?? string.Empty,
                    MoTaCongViecDeXuat = dx.MoTaCongViecDeXuat ?? string.Empty,
                    ChiPhiDeXuat = dx.ChiPhiDeXuat,
                    NgayBatDauCongViecDeXuat = dx.NgayBatDauCongViecDeXuat,
                    NgayKetThucCVDeXuatDuKien = dx.NgayKetThucCVDeXuatDuKien,
                    NgayDeXuatCongViec = dx.NgayDeXuatCongViec,
                    NgayDuyetDeXuatCongViec = dx.NgayDuyetDeXuatCongViec,
                    TrangThaiCongViecDeXuat = dx.TrangThaiCongViecDeXuat ?? string.Empty
                };

            query = query.Where(x => x.MaDuAn == locMaDuAn.Value);

            if (!string.IsNullOrWhiteSpace(locTrangThai))
            {
                var filterValues = TrangThai.GetCommonStatusVariants(locTrangThai);
                if (filterValues.Length > 0)
                {
                    query = query.Where(x => filterValues.Contains(x.TrangThaiCongViecDeXuat));
                }
            }

            return new DeXuatCongViecPageViewModel
            {
                MaDuAn = selectedProject.MaDuAn,
                TenDuAn = selectedProject.TenDuAn,
                DanhSach = await query
                    .OrderByDescending(x => x.NgayDeXuatCongViec)
                    .ThenByDescending(x => x.MaDeXuatCV)
                    .ToListAsync(),
                DanhSachDuAn = projectOptions,
                DanhSachDanhMuc = danhSachDanhMuc,
                DanhSachMucDo = danhSachMucDo,
                Form = new DeXuatCongViecCreateViewModel
                {
                    MaDuAn = selectedProject.MaDuAn
                },
                LocMaDuAn = locMaDuAn,
                LocTrangThai = locTrangThai
            };
        }

        public async Task CreateAsync(DeXuatCongViecCreateViewModel model)
        {
            if (!model.MaDuAn.HasValue || model.MaDuAn.Value <= 0)
                throw new Exception("Vui lòng chọn dự án hợp lệ.");

            if (!model.MaDanhMucCV.HasValue || model.MaDanhMucCV.Value <= 0)
                throw new Exception("Vui lòng chọn danh mục công việc.");

            if (!model.MaMucDo.HasValue || model.MaMucDo.Value <= 0)
                throw new Exception("Vui lòng chọn mức độ ưu tiên.");

            if (!model.ChiPhiDeXuat.HasValue || model.ChiPhiDeXuat.Value <= 0)
                throw new Exception("Chi phí đề xuất phải lớn hơn 0.");

            if (!model.NgayBatDauCongViecDeXuat.HasValue || !model.NgayKetThucCVDeXuatDuKien.HasValue)
                throw new Exception("Vui lòng nhập đầy đủ ngày bắt đầu và ngày kết thúc dự kiến.");

            if (model.NgayKetThucCVDeXuatDuKien < model.NgayBatDauCongViecDeXuat)
                throw new Exception("Ngày kết thúc dự kiến phải lớn hơn hoặc bằng ngày bắt đầu.");

            var maDuAn = model.MaDuAn.Value;
            var maDanhMucCv = model.MaDanhMucCV.Value;
            var maMucDo = model.MaMucDo.Value;
            var currentUserId = await GetCurrentUserIdAsync();
            await EnsureCanProposeForProjectAsync(currentUserId, maDuAn);

            var duAn = await _context.DuAn.FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);
            if (duAn == null)
                throw new Exception("Không tìm thấy dự án.");

            var nganSachHienTai = await _context.NganSach
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true && x.IsActive == true)
                .OrderByDescending(x => x.Version)
                .ThenByDescending(x => x.NgayCapNhatNganSach)
                .FirstOrDefaultAsync();

            if (nganSachHienTai == null)
                throw new Exception("Dự án chưa có ngân sách hiện hành. Vui lòng đề xuất ngân sách trước.");

            var danhMucExists = await _context.DanhMucCongViec
                .AnyAsync(x => x.MaDanhMucCV == maDanhMucCv && x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (!danhMucExists)
                throw new Exception("Danh mục công việc không tồn tại hoặc không thuộc dự án.");

            var mucDoExists = await _context.MucDoUuTien
                .AnyAsync(x => x.MaMucDo == maMucDo);

            if (!mucDoExists)
                throw new Exception("Mức độ ưu tiên không tồn tại.");

            var entity = new DeXuatCongViec
            {
                MaDuAn = maDuAn,
                MaDanhMucCV = maDanhMucCv,
                MaMucDo = maMucDo,
                MaNguoiDungDeXuat = currentUserId,
                TenCongViecDeXuat = model.TenCongViecDeXuat.Trim(),
                MoTaCongViecDeXuat = model.MoTaCongViecDeXuat.Trim(),
                ChiPhiDeXuat = model.ChiPhiDeXuat,
                NgayBatDauCongViecDeXuat = model.NgayBatDauCongViecDeXuat,
                NgayKetThucCVDeXuatDuKien = model.NgayKetThucCVDeXuatDuKien,
                NgayDeXuatCongViec = DateTime.Now,
                TrangThaiCongViecDeXuat = TrangThai.ChoDuyet,
                IsDeleted = false
            };

            _context.DeXuatCongViec.Add(entity);

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = maDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Tạo đề xuất công việc: {entity.TenCongViecDeXuat}",
                NkThoiGianQLDA = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(int maDeXuatCv)
        {
            var currentUserId = await GetCurrentUserIdAsync();

            var entity = await _context.DeXuatCongViec
                .FirstOrDefaultAsync(x => x.MaDeXuatCV == maDeXuatCv && x.IsDeleted != true);

            if (entity == null)
                throw new Exception("Không tìm thấy đề xuất công việc.");

            await EnsureCanProposeForProjectAsync(currentUserId, entity.MaDuAn);

            if (entity.MaNguoiDungDeXuat != currentUserId)
                throw new Exception("Bạn chỉ có thể hủy đề xuất do chính bạn tạo.");

            if (!IsPending(entity.TrangThaiCongViecDeXuat))
                throw new Exception("Chỉ có thể hủy đề xuất đang ở trạng thái chờ duyệt.");

            entity.TrangThaiCongViecDeXuat = TrangThai.DaHuy;

            _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
            {
                MaDuAn = entity.MaDuAn,
                MaNguoiDung = currentUserId,
                NkHanhDongQLDA = $"Hủy đề xuất công việc #{entity.MaDeXuatCV}",
                NkThoiGianQLDA = DateTime.Now
            });

            await _context.SaveChangesAsync();
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

        private async Task EnsureCanProposeForProjectAsync(int maNguoiDung, int maDuAn)
        {
            var coTeamPhuTrach = await _context.TeamDuAn.AnyAsync(x => x.MaDuAn == maDuAn);

            if (coTeamPhuTrach)
            {
                var isLeader = await (
                    from nvt in _context.NhanVienTeam
                    join td in _context.TeamDuAn on nvt.MaTeam equals td.MaTeam
                    where nvt.MaNguoiDung == maNguoiDung
                          && nvt.IsLeader == true
                          && td.MaDuAn == maDuAn
                    select nvt.MaTeam
                ).AnyAsync();

                if (!isLeader)
                    throw new Exception("Dự án đã có team phụ trách, chỉ trưởng team (isleader = true) mới được đề xuất.");

                return;
            }

            var laNhanVienDuAn = await _context.NhanVienDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn && x.MaNguoiDung == maNguoiDung);

            if (!laNhanVienDuAn)
                throw new Exception("Dự án chưa có team phụ trách, chỉ nhân viên thuộc dự án mới được đề xuất.");
        }

        private async Task<List<DeXuatCongViecDuAnOptionViewModel>> GetEligibleProjectOptionsAsync(int maNguoiDung)
        {
            var leaderProjectIds = await (
                from td in _context.TeamDuAn
                join nvt in _context.NhanVienTeam on td.MaTeam equals nvt.MaTeam
                where nvt.MaNguoiDung == maNguoiDung
                      && nvt.IsLeader == true
                select td.MaDuAn
            )
            .Distinct()
            .ToListAsync();

            var memberProjectIdsWithoutTeam = await (
                from nvda in _context.NhanVienDuAn
                where nvda.MaNguoiDung == maNguoiDung
                      && !_context.TeamDuAn.Any(td => td.MaDuAn == nvda.MaDuAn)
                select nvda.MaDuAn
            )
            .Distinct()
            .ToListAsync();

            var allowedProjectIds = leaderProjectIds
                .Concat(memberProjectIdsWithoutTeam)
                .Distinct()
                .ToList();

            return await _context.DuAn
                .Where(x => x.IsDeleted != true && allowedProjectIds.Contains(x.MaDuAn))
                .OrderBy(x => x.TenDuAn)
                .Select(x => new DeXuatCongViecDuAnOptionViewModel
                {
                    MaDuAn = x.MaDuAn,
                    TenDuAn = x.TenDuAn ?? $"Dự án {x.MaDuAn}"
                })
                .ToListAsync();
        }

        private static bool IsPending(string? status)
        {
            return TrangThai.EqualsValue(status, TrangThai.ChoDuyet);
        }
    }
}
