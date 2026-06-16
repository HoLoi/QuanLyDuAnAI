using QuanLyDuAn.ViewModels.DeXuatNganSach;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDeXuatNganSachService
    {
        Task<DeXuatNganSachPageViewModel> GetPageAsync(
            int? locMaDuAn,
            string? locTrangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            int pageNumber = 1,
            int pageSize = 20,
            bool paginate = true);
        Task CreateAsync(DeXuatNganSachCreateViewModel model);
        Task CancelAsync(int maDeXuatNs);
    }
}
