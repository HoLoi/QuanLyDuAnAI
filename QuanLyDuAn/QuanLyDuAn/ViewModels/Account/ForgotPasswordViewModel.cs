using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.Account;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email hoặc tên đăng nhập.")]
    [Display(Name = "Email hoặc tên đăng nhập")]
    public string EmailHoacTenDangNhap { get; set; } = string.Empty;

    public string? ThongBao { get; set; }
}
