using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.DanhGiaDuAn
{
    public class DanhGiaDuAnTieuChiViewModel
    {
        public int? MaChiTietDGDA { get; set; }

        [Required]
        public int MaTieuChi { get; set; }

        public string TenTieuChi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập điểm đánh giá.")]
        [Range(1, 10, ErrorMessage = "Điểm đánh giá phải nằm trong khoảng từ 1 đến 10.")]
        public int DiemDanhGiaDA { get; set; }

        [StringLength(500)]
        public string? NhanXetDuAn { get; set; }
    }
}
