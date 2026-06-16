using QuanLyDuAn.ViewModels.DuyetYeuCauDoiQuanLy;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDuyetYeuCauDoiQuanLyService
    {
        Task<DuyetYeuCauDoiQuanLyPageViewModel> GetPageAsync(
            string? trangThai,
            int? maDuAn,
            string? tuKhoa,
            int pageNumber = 1,
            int pageSize = 20,
            bool paginate = true);
        Task<DuyetYeuCauDoiQuanLyDetailsViewModel> GetDetailsAsync(int id);
        Task ApproveAsync(int id);
        Task RejectAsync(int id, string? lyDoTuChoi);
    }
}
