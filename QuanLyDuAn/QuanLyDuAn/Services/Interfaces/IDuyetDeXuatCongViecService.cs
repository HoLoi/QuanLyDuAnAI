using QuanLyDuAn.ViewModels.DuyetDeXuatCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDuyetDeXuatCongViecService
    {
        Task<DuyetDeXuatCongViecPageViewModel> GetPageAsync(
            int? locMaDuAn,
            string? locTrangThai,
            int? locMaNguoiDungDeXuat,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? tuKhoa,
            int pageNumber = 1,
            int pageSize = 20,
            bool paginate = true);
        Task ApproveAsync(int maDeXuatCv);
        Task RejectAsync(int maDeXuatCv);
    }
}
