using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.NhanSu
{
    public class NhanSuCreateUpdateViewModel : IValidatableObject
    {
        public int? MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [MaxLength(255, ErrorMessage = "Tối đa 255 ký tự")]
        public string HoTenNguoiDung { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [MaxLength(255, ErrorMessage = "Tối đa 255 ký tự")]
        public string DiaChiNguoiDung { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Chỉ được nhập số")]
        public string SdtNguoiDung { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime? NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chức danh")]
        public int? MaChucDanh { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [MaxLength(256, ErrorMessage = "Tối đa 256 ký tự")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [MaxLength(256, ErrorMessage = "Tối đa 256 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [MaxLength(100, ErrorMessage = "Mật khẩu tối đa 100 ký tự")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò hệ thống")]
        public string RoleId { get; set; } = string.Empty;

        [MinLength(6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự")]
        [MaxLength(100, ErrorMessage = "Mật khẩu mới tối đa 100 ký tự")]
        public string? ResetPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MaNguoiDung == null && string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult(
                    "Mật khẩu không được để trống khi tạo mới.",
                    new[] { nameof(Password) });
            }
        }
    }
}
