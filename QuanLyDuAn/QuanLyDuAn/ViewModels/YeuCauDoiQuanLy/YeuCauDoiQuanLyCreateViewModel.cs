using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.YeuCauDoiQuanLy
{
    public class YeuCauDoiQuanLyCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn dự án")]
        public int MaDuAn { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quản lý đề xuất")]
        public int? MaQuanLyDeXuat { get; set; }

        [MaxLength(500, ErrorMessage = "Lý do tối đa 500 ký tự")]
        public string? LyDoYeuCau { get; set; }
    }
}
