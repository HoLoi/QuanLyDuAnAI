using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.PhanQuyen;

public class PhanQuyenPageViewModel
{
    public string? SelectedRoleId { get; set; }
    public List<PhanQuyenRoleViewModel> Roles { get; set; } = new();
    public List<PhanQuyenNhomManHinhViewModel> PermissionGroups { get; set; } = new();
    public HashSet<string> Permissions { get; set; } = new();
}
