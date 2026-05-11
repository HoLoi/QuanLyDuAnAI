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

        [Range(1, 10)]
        public int DiemDanhGiaNV { get; set; }

        [StringLength(500)]
        public string? NoiDungDanhGiaNhanVien { get; set; }
    }
}
