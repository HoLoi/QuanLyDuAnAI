using QuanLyDuAn.ViewModels.Common;
using QuanLyDuAn.ViewModels.NhanSu;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface INhanSuService
    {
        Task<List<NhanSuViewModel>> GetAllAsync(string? tuKhoa, int? maChucDanh, string? trangThaiTaiKhoan);
        Task<PagedResultViewModel<NhanSuViewModel>> GetPagedAsync(
            string? tuKhoa,
            int? maChucDanh,
            string? trangThaiTaiKhoan,
            int pageNumber = 1,
            int pageSize = 20);
        Task<NhanSuCreateUpdateViewModel?> GetByIdAsync(int id);
        Task<List<ChucDanhOptionViewModel>> GetChucDanhOptionsAsync();
        Task<List<VaiTroHeThongOptionViewModel>> GetVaiTroHeThongOptionsAsync();
        Task<string?> SaveAsync(NhanSuCreateUpdateViewModel model, bool laAdminDangThaoTac);
        Task<string?> GuiLaiEmailKichHoatAsync(int id);
        Task DeleteAsync(int id, int maNguoiDungDangThaoTac);
        Task LockAccountAsync(int id, int maNguoiDungDangThaoTac);
        Task UnlockAccountAsync(int id);
    }
}

