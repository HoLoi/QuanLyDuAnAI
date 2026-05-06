using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.ChiTietCongViec
{
    public class ChiTietCongViecCreateUpdateViewModel
    {
        public int MaChiTietCV { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã công việc không hợp lệ.")]
        public int MaCongViec { get; set; }

        [MaxLength(255, ErrorMessage = "Tên chi tiết công việc tối đa 255 ký tự.")]
        public string TenCTCV { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung chi tiết công việc không được để trống.")]
        public string NoiDungChiTietCV { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu chi tiết công việc.")]
        public DateTime? NgayBatDauCTCV { get; set; }

        public DateTime? NgayKetThucCTCV { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái chi tiết công việc.")]
        [MaxLength(50, ErrorMessage = "Trạng thái chi tiết công việc không hợp lệ.")]
        public string TrangThaiCTCV { get; set; } = string.Empty;
    }
}
