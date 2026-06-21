using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.DuAn;
using System.Security.Claims;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class DuAnController : Controller
    {
        private readonly IDuAnService _service;
        private readonly IFileDuAnService _fileDuAnService;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;
        private readonly IExportFileService _exportFileService;
        private readonly IAiService _aiService;

        public DuAnController(
            IDuAnService service,
            IFileDuAnService fileDuAnService,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService,
            IExportFileService exportFileService,
            IAiService aiService)
        {
            _service = service;
            _fileDuAnService = fileDuAnService;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
            _exportFileService = exportFileService;
            _aiService = aiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan,
            int pageNumber = 1,
            int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Xem))
                return Forbid();

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

            var paged = await _service.GetPagedAsync(tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn, tuNgay, denNgay, locTheoNgay, locTinhTrangThoiHan, pageNumber, pageSize);

            var vm = new DuAnPageViewModel
            {
                DanhSach = paged.Items,
                Pagination = paged.Pagination,
                Form = new(),
                DanhSachLoaiDuAn = await _service.GetLoaiDuAnOptionsAsync(),
                TuKhoa = tuKhoa,
                LocMaLoaiDuAn = locMaLoaiDuAn,
                LocTrangThaiDuAn = locTrangThaiDuAn,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                LocTheoNgay = locTheoNgay,
                LocTinhTrangThoiHan = locTinhTrangThoiHan,
                Permissions = permissions
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(
            int id,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Xem))
                return Forbid();

            if (id <= 0)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction(nameof(Index), new
                {
                    tuKhoa,
                    locMaLoaiDuAn,
                    locTrangThaiDuAn,
                    tuNgay,
                    denNgay,
                    locTheoNgay,
                    locTinhTrangThoiHan
                });
            }

            var vm = await _service.GetChiTietAsync(id);
            if (vm == null)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction(nameof(Index), new
                {
                    tuKhoa,
                    locMaLoaiDuAn,
                    locTrangThaiDuAn,
                    tuNgay,
                    denNgay,
                    locTheoNgay,
                    locTinhTrangThoiHan
                });
            }

            vm.TuKhoa = tuKhoa;
            vm.LocMaLoaiDuAn = locMaLoaiDuAn;
            vm.LocTrangThaiDuAn = locTrangThaiDuAn;
            vm.TuNgay = tuNgay;
            vm.DenNgay = denNgay;
            vm.LocTheoNgay = locTheoNgay;
            vm.LocTinhTrangThoiHan = locTinhTrangThoiHan;
            await GanNguCanhDetailsAsync(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PhanTichNguyenNhanTre(
            int maDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan,
            CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.PhanTichNguyenNhan))
                return Forbid();

            var result = await _aiService.PhanTichNguyenNhanDuAnAsync(maDuAn, cancellationToken);
            if (!result.ThanhCong)
            {
                TempData["Error"] = string.IsNullOrWhiteSpace(result.ThongBao)
                    ? "Không kết nối được dịch vụ AI."
                    : result.ThongBao;
                return RedirectToAction(nameof(Details), new { id = maDuAn, tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn, tuNgay, denNgay, locTheoNgay, locTinhTrangThoiHan });
            }

            var vm = await _service.GetChiTietAsync(maDuAn);
            if (vm == null)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction(nameof(Index));
            }

            vm.TuKhoa = tuKhoa;
            vm.LocMaLoaiDuAn = locMaLoaiDuAn;
            vm.LocTrangThaiDuAn = locTrangThaiDuAn;
            vm.TuNgay = tuNgay;
            vm.DenNgay = denNgay;
            vm.LocTheoNgay = locTheoNgay;
            vm.LocTinhTrangThoiHan = locTinhTrangThoiHan;
            await GanNguCanhDetailsAsync(vm);
            if (vm.PhanTichNguyenNhanTre != null)
            {
                vm.PhanTichNguyenNhanTre.KetQua = result.DuLieu;
                vm.PhanTichNguyenNhanTre.ThoiGianPhanTich = result.DuLieu?.ThoiGianPhanTich ?? DateTime.Now;
            }

            TempData["Success"] = "Đã phân tích.";
            return View("Details", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanNguyenNhanTre(
            int maDuAn,
            string maDmNguyenNhan,
            double? doTinCay,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan,
            CancellationToken cancellationToken)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.AI.XacNhan))
                return Forbid();

            var result = await _aiService.XacNhanNguyenNhanAsync(maDuAn, maDmNguyenNhan, doTinCay, cancellationToken);
            if (result.ThanhCong)
            {
                TempData["Success"] = "Đã xác nhận nguyên nhân.";
            }
            else
            {
                TempData["Error"] = string.IsNullOrWhiteSpace(result.ThongBao)
                    ? "Bạn không có quyền thực hiện."
                    : result.ThongBao;
            }

            return RedirectToAction(nameof(Details), new { id = maDuAn, tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn, tuNgay, denNgay, locTheoNgay, locTinhTrangThoiHan });
        }

        [HttpGet]
        public async Task<IActionResult> ChiTiet(
            int id,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            return await Details(id, tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn, tuNgay, denNgay, locTheoNgay, locTinhTrangThoiHan);
        }

        [HttpPost]
        public async Task<IActionResult> ThemFileDuAn(
            int maDuAn,
            IFormFile file,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                var currentUserId = GetCurrentUserId();
                await _fileDuAnService.UploadAsync(maDuAn, file, currentUserId);
                TempData["Success"] = "Đã tải lên tệp dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new
            {
                id = maDuAn,
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn,
                tuNgay,
                denNgay,
                locTheoNgay,
                locTinhTrangThoiHan
            });
        }

        [HttpPost]
        public async Task<IActionResult> XoaFileDuAn(
            int maFileDa,
            int maDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                var currentUserId = GetCurrentUserId();
                await _fileDuAnService.DeleteAsync(maFileDa, currentUserId);
                TempData["Success"] = "Đã xóa tệp dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new
            {
                id = maDuAn,
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn,
                tuNgay,
                denNgay,
                locTheoNgay,
                locTinhTrangThoiHan
            });
        }

        [HttpGet]
        public async Task<IActionResult> TaiFileDuAn(
            int maFileDa,
            int maDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Xem))
                return Forbid();

            try
            {
                var (fullPath, fileName, projectId) = await _fileDuAnService.GetDownloadInfoAsync(maFileDa);
                if (projectId != maDuAn)
                    throw new Exception("Tệp không thuộc dự án hiện tại.");

                return PhysicalFile(fullPath, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new
                {
                    id = maDuAn,
                    tuKhoa,
                    locMaLoaiDuAn,
                    locTrangThaiDuAn,
                    tuNgay,
                    denNgay,
                    locTheoNgay,
                    locTinhTrangThoiHan
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Sua(
            int id,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            var form = await _service.GetByIdAsync(id);
            if (form == null)
            {
                TempData["Error"] = "Không tìm thấy dự án.";
                return RedirectToAction(nameof(Index), new { tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn, tuNgay, denNgay, locTheoNgay, locTinhTrangThoiHan });
            }

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

            var vm = new DuAnPageViewModel
            {
                DanhSach = (await _service.GetPagedAsync(tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn, tuNgay, denNgay, locTheoNgay, locTinhTrangThoiHan)).Items,
                Form = form,
                DanhSachLoaiDuAn = await _service.GetLoaiDuAnOptionsAsync(),
                TuKhoa = tuKhoa,
                LocMaLoaiDuAn = locMaLoaiDuAn,
                LocTrangThaiDuAn = locTrangThaiDuAn,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                LocTheoNgay = locTheoNgay,
                LocTinhTrangThoiHan = locTinhTrangThoiHan,
                Permissions = permissions
            };

            return View("Index", vm);
        }

        [HttpPost]
        public async Task<IActionResult> LuuDuAn(DuAnPageViewModel vm)
        {
            var model = vm.Form;

            if (!ModelState.IsValid)
            {
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, vm.LocMaLoaiDuAn, vm.LocTrangThaiDuAn, vm.TuNgay, vm.DenNgay, vm.LocTheoNgay, vm.LocTinhTrangThoiHan);
                vm.DanhSachLoaiDuAn = await _service.GetLoaiDuAnOptionsAsync();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }

            if (model.MaDuAn == null)
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Them))
                    return Forbid();
            }
            else
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                    return Forbid();
            }

            try
            {
                await _service.SaveAsync(model);
                TempData["Success"] = "Đã lưu dự án.";
                return RedirectToAction(nameof(Index), new
                {
                    tuKhoa = vm.TuKhoa,
                    locMaLoaiDuAn = vm.LocMaLoaiDuAn,
                    locTrangThaiDuAn = vm.LocTrangThaiDuAn,
                    tuNgay = vm.TuNgay,
                    denNgay = vm.DenNgay,
                    locTheoNgay = vm.LocTheoNgay,
                    locTinhTrangThoiHan = vm.LocTinhTrangThoiHan
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, vm.LocMaLoaiDuAn, vm.LocTrangThaiDuAn, vm.TuNgay, vm.DenNgay, vm.LocTheoNgay, vm.LocTinhTrangThoiHan);
                vm.DanhSachLoaiDuAn = await _service.GetLoaiDuAnOptionsAsync();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> XoaDuAn(
            int maDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Xoa))
                return Forbid();

            try
            {
                await _service.DeleteAsync(maDuAn);
                TempData["Success"] = "Đã xóa dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn,
                tuNgay,
                denNgay,
                locTheoNgay,
                locTinhTrangThoiHan
            });
        }

        [HttpPost]
        public async Task<IActionResult> BatDauDuAn(
            int maDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                await _service.TransitionToDangThucHienAsync(maDuAn);
                TempData["Success"] = "Đã bắt đầu dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn,
                tuNgay,
                denNgay,
                locTheoNgay,
                locTinhTrangThoiHan
            });
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanHoanThanh(
            int maDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                await _service.ConfirmCompletionAsync(maDuAn);
                TempData["Success"] = "Đã xác nhận hoàn thành dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn,
                tuNgay,
                denNgay,
                locTheoNgay,
                locTinhTrangThoiHan
            });
        }

        [HttpPost]
        public async Task<IActionResult> MoLaiDuAn(
            int maDuAn,
            string lyDo,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                await _service.MoLaiDuAnAsync(maDuAn, lyDo);
                TempData["Success"] = "Đã mở lại dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn,
                tuNgay,
                denNgay,
                locTheoNgay,
                locTinhTrangThoiHan
            });
        }

        [HttpPost]
        public async Task<IActionResult> TamDungDuAn(
            int maDuAn,
            string ghiChuDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                await _service.PauseProjectAsync(maDuAn, ghiChuDuAn);
                TempData["Success"] = "Đã tạm dừng dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn,
                tuNgay,
                denNgay,
                locTheoNgay,
                locTinhTrangThoiHan
            });
        }

        [HttpPost]
        public async Task<IActionResult> YeuCauHoanThanh(
            int maDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.DuAn.Sua))
                return Forbid();

            try
            {
                await _service.RequestCompletionAsync(maDuAn);
                TempData["Success"] = "Đã gửi yêu cầu hoàn thành dự án.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new
            {
                tuKhoa,
                locMaLoaiDuAn,
                locTrangThaiDuAn,
                tuNgay,
                denNgay,
                locTheoNgay,
                locTinhTrangThoiHan
            });
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(
            string? format,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile))
                return Forbid();

            var rows = (await _service.GetAllAsync(tuKhoa, locMaLoaiDuAn, locTrangThaiDuAn, tuNgay, denNgay, locTheoNgay, locTinhTrangThoiHan))
                .Cast<object>()
                .ToList();

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "Danh sách dự án",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Từ khóa", tuKhoa),
                    ("Loại dự án", locMaLoaiDuAn?.ToString()),
                    ("Trạng thái", TrangThai.ToDisplay(locTrangThaiDuAn)),
                    ("Tình trạng thời hạn", DuAnDeadlineStatusHelper.ToDisplayFilter(locTinhTrangThoiHan)),
                    ("Lọc theo ngày", ExportSupport.ResolveTextOrDefault(locTheoNgay, "Ngày tạo")),
                    ("Từ ngày", ExportSupport.FormatDate(tuNgay)),
                    ("Đến ngày", ExportSupport.FormatDate(denNgay))),
                FileNamePrefix = "du-an",
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Mã dự án", ValueSelector = row => ((DuAnViewModel)row).MaDuAn.ToString() },
                    new() { Header = "Tên dự án", ValueSelector = row => ((DuAnViewModel)row).TenDuAn },
                    new() { Header = "Loại dự án", ValueSelector = row => ((DuAnViewModel)row).TenLoaiDuAn },
                    new() { Header = "Quản lý", ValueSelector = row => ((DuAnViewModel)row).TenNguoiQuanLy },
                    new() { Header = "Ngày bắt đầu", ValueSelector = row => ExportSupport.FormatDate(((DuAnViewModel)row).NgayBatDauDuAn) },
                    new() { Header = "Ngày kết thúc", ValueSelector = row => ExportSupport.FormatDate(((DuAnViewModel)row).NgayKetThucDuAn) },
                    new() { Header = "Ngày hoàn thành thực tế", ValueSelector = row => ExportSupport.FormatDate(((DuAnViewModel)row).NgayHoanThanhThucTeDuAn) },
                    new() { Header = "Tiến độ", ValueSelector = row => $"{((DuAnViewModel)row).PhanTramHoanThanh}%" },
                    new() { Header = "Trạng thái", ValueSelector = row => TrangThai.ToDisplay(((DuAnViewModel)row).TrangThaiDuAn) },
                    new() { Header = "Tình trạng thời hạn", ValueSelector = row => ((DuAnViewModel)row).TinhTrangThoiHan },
                    new() { Header = "Số team", ValueSelector = row => ((DuAnViewModel)row).SoLuongTeam.ToString() },
                    new() { Header = "Số thành viên", ValueSelector = row => ((DuAnViewModel)row).SoLuongThanhVien.ToString() }
                },
                Rows = rows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
        }

        private async Task GanNguCanhDetailsAsync(DuAnChiTietViewModel vm)
        {
            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
            vm.Permissions = permissions;

            var currentUserId = TryGetCurrentUserId();
            vm.CoTheQuanLyFile = currentUserId.HasValue
                                 && currentUserId.Value == vm.MaNguoiDung
                                 && permissions.Contains(Permissions.DuAn.Sua);
            vm.PhanTichNguyenNhanTre = await _aiService.LayPhanTichNguyenNhanDuAnAsync(vm.MaDuAn);
        }

        private int? TryGetCurrentUserId()
        {
            var claimValue = User.FindFirstValue("MaNguoiDung");
            if (int.TryParse(claimValue, out var currentUserId) && currentUserId > 0)
            {
                return currentUserId;
            }

            return null;
        }

        private int GetCurrentUserId()
        {
            var currentUserId = TryGetCurrentUserId();
            if (!currentUserId.HasValue)
                throw new Exception("Khong xac dinh duoc nguoi dung hien tai.");

            return currentUserId.Value;
        }

    }
}
