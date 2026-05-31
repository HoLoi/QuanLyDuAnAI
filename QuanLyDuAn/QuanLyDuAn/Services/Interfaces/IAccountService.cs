using System.Security.Claims;

namespace QuanLyDuAn.Services.Interfaces;

public interface IAccountService
{
    Task<ClaimsPrincipal> AuthenticateAsync(string userName, string password);
    Task<string> KhoiTaoQuenMatKhauAsync(string tenDangNhapHoacEmail);
    Task<bool> XacNhanOtpDatLaiMatKhauAsync(string maPhien, string maOtp);
    Task<bool> CoPhienDatLaiMatKhauHopLeAsync(string maPhien);
    Task DatLaiMatKhauBangOtpAsync(string maPhien, string matKhauMoi);
}
