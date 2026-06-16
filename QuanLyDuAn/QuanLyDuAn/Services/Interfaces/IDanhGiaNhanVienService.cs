using QuanLyDuAn.ViewModels.DanhGiaNhanVien;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IDanhGiaNhanVienService
    {
        Task<DanhGiaNhanVienPageViewModel> GetPageAsync(
            int? maDuAn,
            int? maNhanVien,
            string? tuKhoa,
            string? trangThai,
            DateTime? tuNgayDanhGia,
            DateTime? denNgayDanhGia,
            int pageNumber = 1,
            int pageSize = 20,
            bool paginate = true);
        Task<DanhGiaNhanVienFormViewModel> GetFormAsync(int maDuAn, int maNhanVien);
        Task LuuDanhGiaAsync(DanhGiaNhanVienFormViewModel form);
        Task GuiDuyetAsync(int maDanhGiaNhanVien);
        Task DuyetAsync(int maDanhGiaNhanVien);
        Task TuChoiAsync(int maDanhGiaNhanVien, string lyDoTuChoi);
        Task<DanhGiaNhanVienChiTietViewModel> GetChiTietAsync(int maDanhGiaNhanVien);
    }
}
