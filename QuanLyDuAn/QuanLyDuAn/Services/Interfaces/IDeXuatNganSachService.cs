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
            string? locTheoNgay);
        Task CreateAsync(DeXuatNganSachCreateViewModel model);
        Task CancelAsync(int maDeXuatNs);
    }
}
