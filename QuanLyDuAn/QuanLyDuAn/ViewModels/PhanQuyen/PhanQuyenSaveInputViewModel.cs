using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.PhanQuyen;

public class PhanQuyenSaveInputViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn vai trò hệ thống.")]
    public string SelectedRoleId { get; set; } = string.Empty;

    public List<int> SelectedPermissionIds { get; set; } = new();
}
