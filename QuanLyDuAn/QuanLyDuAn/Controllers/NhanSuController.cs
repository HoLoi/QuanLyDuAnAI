using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Helpers;
using QuanLyDuAn.Services.Exporting;
using QuanLyDuAn.ViewModels.NhanSu;

namespace QuanLyDuAn.Controllers
{
    [Authorize]
    public class NhanSuController : Controller
    {
        private readonly INhanSuService _service;
        private readonly IPermissionHelper _permission;
        private readonly IPhanQuyenService _phanQuyenService;
        private readonly IExportFileService _exportFileService;

        public NhanSuController(
            INhanSuService service,
            IPermissionHelper permission,
            IPhanQuyenService phanQuyenService,
            IExportFileService exportFileService)
        {
            _service = service;
            _permission = permission;
            _phanQuyenService = phanQuyenService;
            _exportFileService = exportFileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            int? locMaChucDanh,
            string? locTrangThaiTaiKhoan,
            int pageNumber = 1,
            int pageSize = 20)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Xem))
                return Forbid();

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

            var paged = await _service.GetPagedAsync(tuKhoa, locMaChucDanh, locTrangThaiTaiKhoan, pageNumber, pageSize);

            var vm = new NhanSuPageViewModel
            {
                DanhSach = paged.Items,
                Pagination = paged.Pagination,
                Form = new(),
                DanhSachChucDanh = await _service.GetChucDanhOptionsAsync(),
                DanhSachVaiTroHeThong = await _service.GetVaiTroHeThongOptionsAsync(),
                TuKhoa = tuKhoa,
                LocMaChucDanh = locMaChucDanh,
                LocTrangThaiTaiKhoan = locTrangThaiTaiKhoan,
                MaNguoiDungHienTai = GetCurrentMaNguoiDung(),
                Permissions = permissions
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Sua(int id)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Sua))
                return Forbid();

            var form = await _service.GetByIdAsync(id);
            if (form == null)
            {
                TempData["Error"] = "Không tìm thấy nhân sự";
                return RedirectToAction(nameof(Index));
            }

            var permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);

            var vm = new NhanSuPageViewModel
            {
                DanhSach = (await _service.GetPagedAsync(null, null, null)).Items,
                Form = form,
                DanhSachChucDanh = await _service.GetChucDanhOptionsAsync(),
                DanhSachVaiTroHeThong = await _service.GetVaiTroHeThongOptionsAsync(),
                MaNguoiDungHienTai = GetCurrentMaNguoiDung(),
                Permissions = permissions
            };

            return View("Index", vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuNhanSu(NhanSuPageViewModel vm)
        {
            var model = vm.Form;

            if (!ModelState.IsValid)
            {
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, vm.LocMaChucDanh, vm.LocTrangThaiTaiKhoan);
                vm.DanhSachChucDanh = await _service.GetChucDanhOptionsAsync();
                vm.DanhSachVaiTroHeThong = await _service.GetVaiTroHeThongOptionsAsync();
                vm.MaNguoiDungHienTai = GetCurrentMaNguoiDung();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }

            if (model.MaNguoiDung == null)
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Them))
                    return Forbid();
            }
            else
            {
                if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Sua))
                    return Forbid();
            }

            try
            {
                var laAdminDangThaoTac = User.IsInRole("ADMIN") || User.IsInRole("Admin");
                var warning = await _service.SaveAsync(model, laAdminDangThaoTac);
                TempData["Success"] = model.MaNguoiDung == null && string.IsNullOrWhiteSpace(warning)
                    ? "Đã tạo nhân sự. Máy chủ thư đã chấp nhận yêu cầu gửi email kích hoạt đến địa chỉ đã đăng ký. Vui lòng kiểm tra Hộp thư đến, Spam, Quảng cáo hoặc Tất cả thư."
                    : "Đã lưu nhân sự";
                if (!string.IsNullOrWhiteSpace(warning))
                {
                    TempData["Warning"] = warning;
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ModelState.AddModelError(string.Empty, ex.Message);
                vm.DanhSach = await _service.GetAllAsync(vm.TuKhoa, vm.LocMaChucDanh, vm.LocTrangThaiTaiKhoan);
                vm.DanhSachChucDanh = await _service.GetChucDanhOptionsAsync();
                vm.DanhSachVaiTroHeThong = await _service.GetVaiTroHeThongOptionsAsync();
                vm.MaNguoiDungHienTai = GetCurrentMaNguoiDung();
                vm.Permissions = await _phanQuyenService.GetGrantedPermissionNamesAsync(User);
                return View("Index", vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiLaiEmailKichHoat(int maNguoiDung)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Sua))
                return Forbid();

            try
            {
                var warning = await _service.GuiLaiEmailKichHoatAsync(maNguoiDung);
                if (string.IsNullOrWhiteSpace(warning))
                {
                    TempData["Success"] = "Máy chủ thư đã chấp nhận yêu cầu gửi lại email kích hoạt. Vui lòng kiểm tra Hộp thư đến, Spam, Quảng cáo hoặc Tất cả thư.";
                }
                else
                {
                    TempData["Warning"] = warning;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaNhanSu(int maNguoiDung)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Xoa))
                return Forbid();

            try
            {
                var maNguoiDungDangThaoTac = GetCurrentMaNguoiDung();
                if (!maNguoiDungDangThaoTac.HasValue)
                {
                    throw new Exception("Không xác định được tài khoản đang đăng nhập.");
                }

                await _service.DeleteAsync(maNguoiDung, maNguoiDungDangThaoTac.Value);
                TempData["Success"] = "Đã xóa nhân sự và vô hiệu hóa tài khoản đăng nhập.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KhoaTaiKhoan(int maNguoiDung)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.Khoa))
                return Forbid();

            try
            {
                var maNguoiDungDangThaoTac = GetCurrentMaNguoiDung();
                if (!maNguoiDungDangThaoTac.HasValue)
                {
                    throw new Exception("Không xác định được tài khoản đang đăng nhập.");
                }

                await _service.LockAccountAsync(maNguoiDung, maNguoiDungDangThaoTac.Value);
                TempData["Success"] = "Đã khóa tài khoản nhân sự";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoKhoaTaiKhoan(int maNguoiDung)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.NhanSu.MoKhoa))
                return Forbid();

            try
            {
                await _service.UnlockAccountAsync(maNguoiDung);
                TempData["Success"] = "Đã mở khóa tài khoản nhân sự";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> XuatFile(
            string? format,
            string? tuKhoa,
            int? locMaChucDanh,
            string? locTrangThaiTaiKhoan)
        {
            if (!await _permission.HasPermissionAsync(User, Permissions.ThongKe.XuatFile)
                || !await _permission.HasPermissionAsync(User, Permissions.NhanSu.Xem))
                return Forbid();

            var rows = (await _service.GetAllAsync(tuKhoa, locMaChucDanh, locTrangThaiTaiKhoan))
                .Cast<object>()
                .ToList();

            var exportRequest = new ExportFileRequest
            {
                ReportTitle = "Danh sách nhân sự",
                ExportedAt = DateTime.Now,
                ExportedBy = ExportSupport.ResolveExporterName(User),
                AppliedFiltersText = ExportSupport.BuildFiltersText(
                    ("Từ khóa", tuKhoa),
                    ("Mã chức danh", locMaChucDanh?.ToString()),
                    ("Trạng thái tài khoản", TrangThai.ToDisplay(locTrangThaiTaiKhoan))),
                FileNamePrefix = "DanhSachNhanSu",
                SheetName = "NhanSu",
                IncludeRowNumber = true,
                PdfLandscape = true,
                Format = _exportFileService.ParseFormat(format),
                Columns = new List<ExportColumnDefinition>
                {
                    new() { Header = "Họ tên", ValueSelector = row => ((NhanSuViewModel)row).HoTenNguoiDung, MinWidth = 18, MaxWidth = 28 },
                    new() { Header = "Chức danh", ValueSelector = row => ((NhanSuViewModel)row).TenChucDanh, MinWidth = 16, MaxWidth = 24 },
                    new() { Header = "Điện thoại", ValueSelector = row => ((NhanSuViewModel)row).SdtNguoiDung, Alignment = ExportColumnAlignment.Center, MinWidth = 13, MaxWidth = 16 },
                    new() { Header = "Email", ValueSelector = row => ((NhanSuViewModel)row).Email, MinWidth = 22, MaxWidth = 35 },
                    new() { Header = "Trạng thái tài khoản", ValueSelector = row => ((NhanSuViewModel)row).TrangThaiTaiKhoan, Alignment = ExportColumnAlignment.Center, MinWidth = 16, MaxWidth = 24 },
                    new() { Header = "Mã nhân sự", ValueSelector = row => ((NhanSuViewModel)row).MaNguoiDung, Alignment = ExportColumnAlignment.Center, MinWidth = 10, MaxWidth = 14, ShowInPdf = false }
                },
                Rows = rows
            };

            var result = _exportFileService.Export(exportRequest);
            return File(result.Content, result.ContentType, result.FileName);
        }

        private int? GetCurrentMaNguoiDung()
        {
            var claimValue = User.FindFirst("MaNguoiDung")?.Value;
            return int.TryParse(claimValue, out var maNguoiDung) && maNguoiDung > 0
                ? maNguoiDung
                : null;
        }
    }
}
