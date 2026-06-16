using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.NhanSu
{
    public class NhanSuCreateUpdateViewModel
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

        public string? Password { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò hệ thống")]
        public string RoleId { get; set; } = string.Empty;

        public string? ResetPassword { get; set; }
    }
}
