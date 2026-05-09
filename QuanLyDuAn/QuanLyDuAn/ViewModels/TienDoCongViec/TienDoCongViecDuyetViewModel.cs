using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.TienDoCongViec
{
    public class TienDoCongViecDuyetViewModel
    {
        [Required]
        public int MaTienDo { get; set; }

        [MaxLength(255, ErrorMessage = "Ghi chú duyệt tối đa 255 ký tự.")]
        public string? GhiChuDuyet { get; set; }
    }
}
