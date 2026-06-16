using QuanLyDuAn.ViewModels.NhanVienDuAn;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface INhanVienDuAnService
    {
        Task<NhanVienDuAnPageViewModel> GetPageAsync(
            int maDuAn,
            string? tuKhoa,
            int? locMaLoaiDuAn,
            string? locTrangThaiDuAn,
            int pageNumber = 1,
            int pageSize = 20);

        Task AddAsync(int maDuAn, List<int> maNguoiDungDuocChon);
        Task UpdateRoleAsync(int maDuAn, int maNguoiDung, string vaiTroTrongDuAn);
        Task RemoveAsync(int maDuAn, int maNguoiDung);
    }
}
