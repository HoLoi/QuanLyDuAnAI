using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.DanhGiaDuAn
{
    public class DanhGiaDuAnTieuChiViewModel
    {
        public int? MaChiTietDGDA { get; set; }

        [Required]
        public int MaTieuChi { get; set; }

        public string TenTieuChi { get; set; } = string.Empty;

        [Range(1, 10)]
        public int DiemDanhGiaDA { get; set; }

        [StringLength(500)]
        public string? NhanXetDuAn { get; set; }
    }
}
