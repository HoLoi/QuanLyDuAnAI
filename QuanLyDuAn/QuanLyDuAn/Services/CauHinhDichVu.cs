using QuanLyDuAn.Services.Implementations;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Services
{
    public static class CauHinhDichVu
    {
        public static IServiceCollection AddTangDichVu(this IServiceCollection dichVu)
        {
            dichVu.AddScoped<IPhanQuyenService, PhanQuyenService>();
            dichVu.AddScoped<IPermissionHelper, PermissionHelper>();
            dichVu.AddScoped<IChucDanhService, ChucDanhService>();
            dichVu.AddScoped<INhanSuService, NhanSuService>();
            dichVu.AddScoped<ITeamService, TeamService>();
            dichVu.AddScoped<IThanhVienTeamService, ThanhVienTeamService>();
            dichVu.AddScoped<IDuAnService, DuAnService>();
            dichVu.AddScoped<ITeamDuAnService, TeamDuAnService>();
            dichVu.AddScoped<INhanVienDuAnService, NhanVienDuAnService>();
            dichVu.AddScoped<IDanhMucCongViecService, DanhMucCongViecService>();
            dichVu.AddScoped<ICongViecService, CongViecService>();
            dichVu.AddScoped<IDeXuatCongViecService, DeXuatCongViecService>();
            dichVu.AddScoped<IDuyetDeXuatCongViecService, DyetDeXuatCongViecService>();
            dichVu.AddScoped<INganSachService, NganSachService>();
            dichVu.AddScoped<IDeXuatNganSachService, DeXuatNganSachService>();
            dichVu.AddScoped<IDuyetDeXuatNganSachService, DuyetDeXuatNganSachService>();
            dichVu.AddScoped<IAccountService, AccountService>();
            dichVu.AddScoped<IDashboardService, DashboardService>();
            dichVu.AddScoped<IEmailService, GmailEmailService>();

            return dichVu;
        }
    }
}
