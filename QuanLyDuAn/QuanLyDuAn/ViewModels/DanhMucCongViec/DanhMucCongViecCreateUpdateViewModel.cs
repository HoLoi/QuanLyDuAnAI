using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.DanhMucCongViec
{
    public class DanhMucCongViecCreateUpdateViewModel
    {
        public int? MaDanhMucCV { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn dự án")]
        public int? MaDuAn { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(255, ErrorMessage = "Tối đa 255 ký tự")]
        public string TenDanhMucCV { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả không được để trống")]
        [MaxLength(255, ErrorMessage = "Tối đa 255 ký tự")]
        public string MoTaDanhMucCV { get; set; } = string.Empty;
    }
}
