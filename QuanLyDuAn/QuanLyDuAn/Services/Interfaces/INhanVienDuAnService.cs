using QuanLyDuAn.ViewModels.NhanVienDuAn;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface INhanVienDuAnService
    {
        Task<NhanVienDuAnPageViewModel> GetPageAsync(
            int maDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn);

        Task AddAsync(int maDuAn, List<int> maNguoiDungDuocChon);
        Task UpdateRoleAsync(int maDuAn, int maNguoiDung, string vaiTroTrongDuAn);
        Task RemoveAsync(int maDuAn, int maNguoiDung);
    }
}
