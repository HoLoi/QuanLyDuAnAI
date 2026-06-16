using QuanLyDuAn.ViewModels.DuyetDeXuatNganSach;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDuyetDeXuatNganSachService
    {
        Task<DuyetDeXuatNganSachPageViewModel> GetPageAsync(
            int? locMaDuAn,
            string? locTrangThai,
            int pageNumber = 1,
            int pageSize = 20,
            bool paginate = true);
        Task ApproveAsync(int maDeXuatNs);
        Task RejectAsync(int maDeXuatNs, string? lyDo);
    }
}
