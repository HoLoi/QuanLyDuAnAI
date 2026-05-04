using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.ChiTietCongViec
{
    public class ChiTietCongViecCreateUpdateViewModel
    {
        public int MaChiTietCV { get; set; }
        public int MaCongViec { get; set; }

        [Required(ErrorMessage = "Nội dung chi tiết công việc không được để trống.")]
        public string NoiDungChiTietCV { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày báo cáo.")]
        public DateTime? NgayBaoCaoCTCV { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phần trăm hoàn thành.")]
        [Range(0, 100, ErrorMessage = "Phần trăm hoàn thành phải từ 0 đến 100.")]
        public double? PhanTramHoanThanhCTCV { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái chi tiết công việc.")]
        public string TrangThaiCTCV { get; set; } = string.Empty;
    }
}
