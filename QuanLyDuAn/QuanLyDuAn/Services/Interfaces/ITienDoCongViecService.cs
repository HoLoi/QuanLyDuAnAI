using QuanLyDuAn.ViewModels.TienDoCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface ITienDoCongViecService
    {
        Task<TienDoCongViecPageViewModel> GetPageAsync(
            int? locMaDuAn,
            int? locMaCongViec,
            int? locMaChiTietCv,
            string? tuKhoa,
            DateTime? tuNgayBaoCao,
            DateTime? denNgayBaoCao,
            int pageNumber = 1,
            int pageSize = 20,
            bool paginate = true);

        Task CapNhatTienDoAsync(TienDoCongViecCapNhatViewModel form);
        Task DuyetBaoCaoTienDoAsync(TienDoCongViecDuyetViewModel form);
        Task YeuCauBoSungBaoCaoTienDoAsync(TienDoCongViecDuyetViewModel form);
        Task TuChoiBaoCaoTienDoAsync(TienDoCongViecDuyetViewModel form);
    }
}
