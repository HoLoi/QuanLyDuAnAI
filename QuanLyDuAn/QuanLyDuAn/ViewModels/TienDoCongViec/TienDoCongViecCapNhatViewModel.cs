using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.TienDoCongViec
{
    public class TienDoCongViecCapNhatViewModel
    {
        [Required]
        public int MaChiTietCV { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái đề xuất.")]
        [MaxLength(50, ErrorMessage = "Trạng thái đề xuất không hợp lệ.")]
        public string TrangThaiCTCVMoi { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự.")]
        public string? GhiChuTienDo { get; set; }

        public List<IFormFile> Files { get; set; } = new();
    }
}
