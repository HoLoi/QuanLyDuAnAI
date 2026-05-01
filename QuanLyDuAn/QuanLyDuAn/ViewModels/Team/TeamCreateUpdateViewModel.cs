using System.ComponentModel.DataAnnotations;
using QuanLyDuAn.Constants;

namespace QuanLyDuAn.ViewModels.Team
{
    public class TeamCreateUpdateViewModel
    {
        public int? MaTeam { get; set; }

        [Required(ErrorMessage = "Tên team không được để trống")]
        [MaxLength(255, ErrorMessage = "Tối đa 255 ký tự")]
        public string TenTeam { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả team không được để trống")]
        [MaxLength(255, ErrorMessage = "Tối đa 255 ký tự")]
        public string MoTaTeam { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string TrangThaiTeam { get; set; } = TrangThai.HoatDong;
    }
}
