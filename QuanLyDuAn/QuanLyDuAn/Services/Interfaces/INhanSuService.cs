using QuanLyDuAn.ViewModels.NhanSu;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface INhanSuService
    {
        Task<List<NhanSuViewModel>> GetAllAsync(string? tuKhoa, int? maChucDanh, string? trangThaiTaiKhoan);
        Task<NhanSuCreateUpdateViewModel?> GetByIdAsync(int id);
        Task<List<ChucDanhOptionViewModel>> GetChucDanhOptionsAsync();
        Task<List<VaiTroHeThongOptionViewModel>> GetVaiTroHeThongOptionsAsync();
        Task<string?> SaveAsync(NhanSuCreateUpdateViewModel model, bool laAdminDangThaoTac);
        Task DeleteAsync(int id);
        Task LockAccountAsync(int id);
        Task UnlockAccountAsync(int id);
    }
}
