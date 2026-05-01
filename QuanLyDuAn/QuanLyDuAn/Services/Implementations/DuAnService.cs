using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DuAn;
using System.Security.Claims;

namespace QuanLyDuAn.Services.Implementations
{
    public class DuAnService : IDuAnService
    {
        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DuAnService(QuanLyDuAnDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        #region CRUD Operations

        public async Task<List<DuAnViewModel>> GetAllAsync(string? tuKhoa, int? maLoaiDuAn, string? trangThaiDuAn)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var (isManager, isEmployee) = await GetCurrentUserRoleFlagsAsync();

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
                            PhanTramHoanThanh = da.PhanTramHoanThanh ?? 0,
                            TrangThaiDuAn = da.TrangThaiDuAn ?? string.Empty,
                            SoLuongTeam = _context.TeamDuAn.Count(x => x.MaDuAn == da.MaDuAn),
                            SoLuongThanhVien = _context.NhanVienDuAn.Count(x => x.MaDuAn == da.MaDuAn)
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

            return await query.ToListAsync();
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

                // Cannot edit if completed
                if (TrangThai.EqualsValue(existing.TrangThaiDuAn, TrangThai.HoanThanh))
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
                var entity = new DuAn
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

                _context.DuAn.Add(entity);
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
                }
            }

            await _context.SaveChangesAsync();

            // Auto-check and transition if conditions met
            if (model.MaDuAn == null)
            {
                var newProject = await _context.DuAn
                    .Where(x => x.MaNguoiDung == maNguoiDung && x.TenDuAn == tenDuAn && x.IsDeleted != true)
                    .OrderByDescending(x => x.MaDuAn)
                    .FirstOrDefaultAsync();

                if (newProject != null)
                {
                    _context.NhatKyQuanLyDuAn.Add(new NhatKyQuanLyDuAn
                    {
                        MaDuAn = newProject.MaDuAn,
                        MaNguoiDung = maNguoiDung,
                        NkHanhDongQLDA = $"Tạo dự án: {newProject.TenDuAn}",
                        NkThoiGianQLDA = DateTime.Now
                    });

                    await _context.SaveChangesAsync();
                    await CheckAutoTransitionAsync(newProject.MaDuAn);
                }
            }
            else
            {
                await CheckAutoTransitionAsync(model.MaDuAn.Value);
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
                            (x.cv.TrangThaiCongViec == TrangThai.HoanThanh || x.cv.TrangThaiCongViec == TrangThai.HoanThanhHienThi))
                .CountAsync();

            //result.AllTasksDone = totalTasks > 0 && completedTasks == totalTasks && project.PhanTramHoanThanh == 100;
            result.AllTasksDone = totalTasks > 0 && completedTasks == totalTasks;

            // Check ongoing tasks
            result.HasOngoingTasks = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && 
                               x.cv.IsDeleted != true &&
                               (x.cv.TrangThaiCongViec == TrangThai.DangThucHien || x.cv.TrangThaiCongViec == TrangThai.DangThucHienHienThi));

            // Check blocked tasks
            result.HasBlockedTasks = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && 
                               x.cv.IsDeleted != true &&
                               (x.cv.TrangThaiCongViec == TrangThai.BiCanCan || x.cv.TrangThaiCongViec == TrangThai.BiCanCanHienThi));

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
                            (x.cv.TrangThaiCongViec == TrangThai.HoanThanh || x.cv.TrangThaiCongViec == TrangThai.HoanThanhHienThi))
                .CountAsync();

            if (!(totalTasks > 0 && completedTasks == totalTasks))
                throw new Exception("Chưa hoàn thành tất cả công việc.");

            // Check no ongoing tasks
            var hasOngoing = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && 
                               x.cv.IsDeleted != true &&
                               (x.cv.TrangThaiCongViec == TrangThai.DangThucHien || x.cv.TrangThaiCongViec == TrangThai.DangThucHienHienThi));

            if (hasOngoing)
                throw new Exception("Còn công việc đang thực hiện.");

            // Check no blocked tasks
            var hasBlocked = await _context.CongViec
                .Join(_context.DanhMucCongViec, cv => cv.MaDanhMucCV, dmcv => dmcv.MaDanhMucCV, 
                      (cv, dmcv) => new { cv, dmcv })
                .AnyAsync(x => x.dmcv.MaDuAn == maDuAn && 
                               x.cv.IsDeleted != true &&
                               (x.cv.TrangThaiCongViec == TrangThai.BiCanCan || x.cv.TrangThaiCongViec == TrangThai.BiCanCanHienThi));

            if (hasBlocked)
                throw new Exception("Có công việc bị cản cản.");

            // Check 100% completion
            if (project.PhanTramHoanThanh != 100)
                throw new Exception("Tiến độ dự án chưa đạt 100%.");

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
