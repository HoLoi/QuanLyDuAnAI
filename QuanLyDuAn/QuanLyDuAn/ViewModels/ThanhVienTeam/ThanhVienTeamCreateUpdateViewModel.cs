using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.ThanhVienTeam
{
    public class ThanhVienTeamCreateUpdateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn team")]
        public int? MaTeam { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhân sự")]
        public int? MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Vai trò trong team không được để trống")]
        [MaxLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string VaiTroTrongTeam { get; set; } = string.Empty;

        public bool IsLeader { get; set; }
    }
}
