using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.DeXuatNganSach
{
    public class DeXuatNganSachCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn dự án")]
        public int? MaDuAn { get; set; }

        [Required(ErrorMessage = "Ngân sách đề xuất không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ngân sách đề xuất phải lớn hơn 0")]
        public decimal? NganSachDeXuat { get; set; }

        [Required(ErrorMessage = "Lý do đề xuất không được để trống")]
        public string LyDoDeXuat { get; set; } = string.Empty;
    }
}
