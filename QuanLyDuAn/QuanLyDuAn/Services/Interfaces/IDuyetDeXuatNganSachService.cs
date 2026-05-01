using QuanLyDuAn.ViewModels.DuyetDeXuatNganSach;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDuyetDeXuatNganSachService
    {
        Task<DuyetDeXuatNganSachPageViewModel> GetPageAsync(int? locMaDuAn, string? locTrangThai);
        Task ApproveAsync(int maDeXuatNs);
        Task RejectAsync(int maDeXuatNs, string? lyDo);
    }
}
