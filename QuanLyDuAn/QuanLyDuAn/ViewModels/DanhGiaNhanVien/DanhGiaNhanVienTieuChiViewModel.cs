using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.DanhGiaNhanVien
{
    public class DanhGiaNhanVienTieuChiViewModel
    {
        public int? MaChiTietDGNV { get; set; }

        [Required]
        public int MaTieuChi { get; set; }

        public string TenTieuChi { get; set; } = string.Empty;

        public int? MaCongViec { get; set; }
        public string? TenCongViec { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập điểm đánh giá.")]
        [Range(1, 10, ErrorMessage = "Điểm đánh giá phải nằm trong khoảng từ 1 đến 10.")]
        public int DiemDanhGiaNV { get; set; }

        [StringLength(500)]
        public string? NoiDungDanhGiaNhanVien { get; set; }
    }
}
