using QuanLyDuAn.ViewModels.TienDoCongViec;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface ITienDoCongViecService
    {
        Task<TienDoCongViecPageViewModel> GetPageAsync(int? locMaDuAn, int? locMaCongViec, int? locMaChiTietCv, string? tuKhoa);

        Task CapNhatTienDoAsync(TienDoCongViecCapNhatViewModel form);
        Task DuyetBaoCaoTienDoAsync(TienDoCongViecDuyetViewModel form);
        Task YeuCauBoSungBaoCaoTienDoAsync(TienDoCongViecDuyetViewModel form);
        Task TuChoiBaoCaoTienDoAsync(TienDoCongViecDuyetViewModel form);
    }
}
