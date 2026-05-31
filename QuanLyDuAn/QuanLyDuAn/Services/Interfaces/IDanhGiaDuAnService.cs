using QuanLyDuAn.ViewModels.DanhGiaDuAn;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDanhGiaDuAnService
    {
        Task<DanhGiaDuAnPageViewModel> GetPageAsync(
            string? tuKhoa,
            string? trangThai,
            int? maDuAn,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia);
        Task<DanhGiaDuAnFormViewModel> GetFormAsync(int maDuAn);
        Task<DanhGiaDuAnAiInsightViewModel> PhanTichAiDuAnAsync(int maDuAn, CancellationToken cancellationToken = default);
        Task XacNhanNguyenNhanAsync(int maDuAn, int maDmNguyenNhan, double? doTinCay);
        Task LuuDanhGiaAsync(DanhGiaDuAnFormViewModel form);
        Task GuiDuyetAsync(int maDanhGiaDuAn);
        Task DuyetAsync(int maDanhGiaDuAn);
        Task TuChoiAsync(int maDanhGiaDuAn, string lyDoTuChoi);
        Task<DanhGiaDuAnChiTietViewModel> GetChiTietAsync(int maDanhGiaDuAn);
    }
}
