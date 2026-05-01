using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.DeXuatCongViec
{
    public class DeXuatCongViecCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn dự án")]
        public int? MaDuAn { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục công việc")]
        public int? MaDanhMucCV { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn mức độ ưu tiên")]
        public int? MaMucDo { get; set; }

        [Required(ErrorMessage = "Tên công việc đề xuất không được để trống")]
        [MaxLength(255, ErrorMessage = "Tối đa 255 ký tự")]
        public string TenCongViecDeXuat { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả công việc đề xuất không được để trống")]
        public string MoTaCongViecDeXuat { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chi phí đề xuất không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Chi phí đề xuất phải lớn hơn 0")]
        public decimal? ChiPhiDeXuat { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu dự kiến")]
        public DateTime? NgayBatDauCongViecDeXuat { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc dự kiến")]
        public DateTime? NgayKetThucCVDeXuatDuKien { get; set; }
    }
}
