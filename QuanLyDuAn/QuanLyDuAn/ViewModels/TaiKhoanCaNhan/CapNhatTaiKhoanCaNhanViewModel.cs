using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace QuanLyDuAn.ViewModels.TaiKhoanCaNhan
{
    public class CapNhatTaiKhoanCaNhanViewModel
    {
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [MaxLength(255, ErrorMessage = "Họ tên tối đa 255 ký tự.")]
        [Display(Name = "Họ tên")]
        public string HoTenNguoiDung { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
        [Display(Name = "Số điện thoại")]
        public string? SdtNguoiDung { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [MaxLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự.")]
        [Display(Name = "Địa chỉ")]
        public string DiaChiNguoiDung { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        public string? AnhDaiDienHienTai { get; set; }
        public string ChuCaiDau { get; set; } = "U";

        [Display(Name = "Ảnh đại diện")]
        public IFormFile? AnhDaiDienFile { get; set; }

        public string? TenChucDanh { get; set; }
        public List<string> VaiTroHeThong { get; set; } = new();
        public List<string> TeamHienTai { get; set; } = new();
    }
}
