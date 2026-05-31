using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.Account;

public class VerifyOtpViewModel
{
    [Required(ErrorMessage = "Thiếu mã phiên xác thực.")]
    public string MaPhien { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mã OTP.")]
    [Display(Name = "Mã OTP")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Mã OTP gồm đúng 6 chữ số.")]
    public string MaOtp { get; set; } = string.Empty;

    public string? ThongBao { get; set; }
}
