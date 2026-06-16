using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.Account;

public class ActivateAccountViewModel : IValidatableObject
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    public string? TenDangNhapHienThi { get; set; }
    public string? EmailHienThi { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải từ 8 đến 100 ký tự.")]
    [Display(Name = "Mật khẩu mới")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Xác nhận mật khẩu")]
    [Compare(nameof(Password), ErrorMessage = "Xác nhận mật khẩu không khớp.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Password))
        {
            yield break;
        }

        if (!Password.Any(char.IsUpper))
        {
            yield return new ValidationResult("Mật khẩu phải có ít nhất một chữ hoa.", new[] { nameof(Password) });
        }

        if (!Password.Any(char.IsLower))
        {
            yield return new ValidationResult("Mật khẩu phải có ít nhất một chữ thường.", new[] { nameof(Password) });
        }

        if (!Password.Any(char.IsDigit))
        {
            yield return new ValidationResult("Mật khẩu phải có ít nhất một chữ số.", new[] { nameof(Password) });
        }

        if (!Password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            yield return new ValidationResult("Mật khẩu phải có ít nhất một ký tự đặc biệt.", new[] { nameof(Password) });
        }
    }
}
