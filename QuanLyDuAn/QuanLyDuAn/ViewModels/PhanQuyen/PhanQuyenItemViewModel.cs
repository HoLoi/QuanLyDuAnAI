namespace QuanLyDuAn.ViewModels.PhanQuyen;

public class PhanQuyenItemViewModel
{
    public int MaDanhMucQuyen { get; set; }
    public string TenDanhMucQuyen { get; set; } = string.Empty;
    public string? MoTaDanhMucQuyen { get; set; }
    public bool IsSelected { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string ScreenKey { get; set; } = string.Empty;
    public string ScreenDisplayName { get; set; } = string.Empty;
    public string? ParentPermission { get; set; }
    public bool IsParent { get; set; }
    public string DependencyMessage { get; set; } = string.Empty;
    public bool IsDeniedByRole { get; set; }
    public bool IsRequiredByRole { get; set; }
    public string Description { get; set; } = string.Empty;
    public int GroupOrder { get; set; }
    public int ScreenOrder { get; set; }
    public int PermissionOrder { get; set; }
}
