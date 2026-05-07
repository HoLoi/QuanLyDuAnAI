using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.PhanCongChiTietCongViec
{
    public class PhanCongChiTietCongViecCreateViewModel
    {
        [Required]
        public int MaChiTietCV { get; set; }

        [Required(ErrorMessage = "Vui l?ng ch?n nhân viên.")]
        public int? MaNguoiDung { get; set; }
    }
}
