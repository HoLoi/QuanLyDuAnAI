namespace QuanLyDuAn.Services.Interfaces
{
    public interface ITrangThaiWorkflowService
    {
        Task DongBoTrangThaiCongViecTheoChiTietAsync(int maCongViec, int? maNguoiDungThucHien = null, string? nguonCapNhat = null);
        Task DongBoTrangThaiDuAnTheoCongViecAsync(int maDuAn, int? maNguoiDungThucHien = null, string? nguonCapNhat = null);
        Task DongBoChuoiTrangThaiTuCongViecAsync(int maCongViec, int? maNguoiDungThucHien = null, string? nguonCapNhat = null);
    }
}
