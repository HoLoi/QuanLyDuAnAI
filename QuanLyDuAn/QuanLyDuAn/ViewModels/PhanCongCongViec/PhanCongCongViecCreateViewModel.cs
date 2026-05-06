using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.PhanCongCongViec
{
    public class PhanCongCongViecCreateViewModel
    {
        [Required]
        public int MaCongViec { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhân viên.")]
        public int? MaNguoiDung { get; set; }
    }
}
