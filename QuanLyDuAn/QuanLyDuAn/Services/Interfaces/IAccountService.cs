using System.Security.Claims;

namespace QuanLyDuAn.Services.Interfaces;

public interface IAccountService
{
    Task<ClaimsPrincipal> AuthenticateAsync(string userName, string password);
}
