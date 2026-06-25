using QuanLyDuAn.ViewModels.CongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface ICongViecService
    {
        Task<CongViecPageViewModel> GetPageAsync(
            int? locMaDuAn,
            string? locTrangThai,
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? locTheoNgay,
            string? locTinhTrangThoiHan,
            int pageNumber = 1,
            int pageSize = 20,
            bool paginate = true);
        Task XacNhanHoanThanhCongViecAsync(int maCongViec);
        Task MoLaiCongViecAsync(int maCongViec, string lyDo);
    }
}
