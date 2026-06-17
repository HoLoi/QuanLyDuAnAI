using QuanLyDuAn.Services.Implementations;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Services
{
    public static class CauHinhDichVu
    {
        public static IServiceCollection AddTangDichVu(this IServiceCollection dichVu)
        {
            dichVu.AddSingleton<IPermissionDependencyProvider, PermissionDependencyProvider>();
            dichVu.AddScoped<IPhanQuyenService, PhanQuyenService>();
            dichVu.AddScoped<IPermissionHelper, PermissionHelper>();
            dichVu.AddScoped<IChucDanhService, ChucDanhService>();
            dichVu.AddScoped<INhanSuService, NhanSuService>();
            dichVu.AddScoped<ITeamService, TeamService>();
            dichVu.AddScoped<IThanhVienTeamService, ThanhVienTeamService>();
            dichVu.AddScoped<IDuAnService, DuAnService>();
            dichVu.AddScoped<IFileDuAnService, FileDuAnService>();
            dichVu.AddScoped<ITeamDuAnService, TeamDuAnService>();
            dichVu.AddScoped<INhanVienDuAnService, NhanVienDuAnService>();
            dichVu.AddScoped<IDanhMucCongViecScopeService, DanhMucCongViecScopeService>();
            dichVu.AddScoped<IDanhMucCongViecService, DanhMucCongViecService>();
            dichVu.AddScoped<ITrangThaiWorkflowService, TrangThaiWorkflowService>();
            dichVu.AddScoped<ICongViecService, CongViecService>();
            dichVu.AddScoped<IChiTietCongViecService, ChiTietCongViecService>();
            dichVu.AddScoped<IPhanCongCongViecService, PhanCongCongViecService>();
            dichVu.AddScoped<IPhanCongChiTietCongViecService, PhanCongChiTietCongViecService>();
            dichVu.AddScoped<IChatDuAnService, ChatDuAnService>();
            dichVu.AddScoped<ITienDoCongViecService, TienDoCongViecService>();
            dichVu.AddScoped<IFileTienDoCongViecService, FileTienDoCongViecService>();
            dichVu.AddScoped<IDanhGiaDuAnService, DanhGiaDuAnService>();
            dichVu.AddScoped<IDanhGiaNhanVienService, DanhGiaNhanVienService>();
            dichVu.AddScoped<IDeXuatCongViecService, DeXuatCongViecService>();
            dichVu.AddScoped<IDuyetDeXuatCongViecService, DyetDeXuatCongViecService>();
            dichVu.AddScoped<IYeuCauDoiQuanLyService, YeuCauDoiQuanLyService>();
            dichVu.AddScoped<IDuyetYeuCauDoiQuanLyService, DuyetYeuCauDoiQuanLyService>();
            dichVu.AddScoped<INganSachService, NganSachService>();
            dichVu.AddScoped<IDeXuatNganSachService, DeXuatNganSachService>();
            dichVu.AddScoped<IDuyetDeXuatNganSachService, DuyetDeXuatNganSachService>();
            dichVu.AddScoped<IAccountService, AccountService>();
            dichVu.AddScoped<IDashboardService, DashboardService>();
            dichVu.AddScoped<IAiService, AiService>();
            dichVu.AddScoped<IAiDatasetService, AiDatasetService>();
            dichVu.AddScoped<IExportFileService, ExportFileService>();
            dichVu.AddScoped<IEmailService, GmailEmailService>();

            return dichVu;
        }
    }
}
