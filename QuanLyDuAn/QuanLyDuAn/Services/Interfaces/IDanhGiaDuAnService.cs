using QuanLyDuAn.ViewModels.DanhGiaDuAn;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDanhGiaDuAnService
    {
        Task<DanhGiaDuAnPageViewModel> GetPageAsync(string? tuKhoa, string? trangThai, int? maDuAn);
        Task<DanhGiaDuAnFormViewModel> GetFormAsync(int maDuAn);
        Task LuuDanhGiaAsync(DanhGiaDuAnFormViewModel form);
        Task GuiDuyetAsync(int maDanhGiaDuAn);
        Task DuyetAsync(int maDanhGiaDuAn);
        Task TuChoiAsync(int maDanhGiaDuAn, string lyDoTuChoi);
        Task<DanhGiaDuAnChiTietViewModel> GetChiTietAsync(int maDanhGiaDuAn);
    }
}
