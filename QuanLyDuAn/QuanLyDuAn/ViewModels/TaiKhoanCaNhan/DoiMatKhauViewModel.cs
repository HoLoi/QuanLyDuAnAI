using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.TaiKhoanCaNhan
{
    public class DoiMatKhauViewModel
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string MatKhauHienTai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự.")]
        [MaxLength(100, ErrorMessage = "Mật khẩu mới tối đa 100 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string MatKhauMoi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare(nameof(MatKhauMoi), ErrorMessage = "Xác nhận mật khẩu không khớp.")]
        public string XacNhanMatKhau { get; set; } = string.Empty;
    }
}
