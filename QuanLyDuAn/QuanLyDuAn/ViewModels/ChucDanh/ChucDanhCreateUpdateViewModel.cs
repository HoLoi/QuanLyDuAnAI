using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.ChucDanh
{
    public class ChucDanhCreateUpdateViewModel
    {
        public int? MaChucDanh { get; set; }

        [Required(ErrorMessage = "Tên chức danh không được để trống")]
        [MaxLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string TenChucDanh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả chức danh không được để trống")]
        [MaxLength(255, ErrorMessage = "Tối đa 255 ký tự")]
        public string MoTaChucDanh { get; set; } = string.Empty;
    }
}
