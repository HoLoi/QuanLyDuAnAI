using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.Account;

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "Thiếu mã phiên đặt lại mật khẩu.")]
    public string MaPhien { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải từ 6 đến 100 ký tự.")]
    [Display(Name = "Mật khẩu mới")]
    public string MatKhauMoi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Xác nhận mật khẩu")]
    [Compare(nameof(MatKhauMoi), ErrorMessage = "Xác nhận mật khẩu không khớp.")]
    public string XacNhanMatKhau { get; set; } = string.Empty;
}
