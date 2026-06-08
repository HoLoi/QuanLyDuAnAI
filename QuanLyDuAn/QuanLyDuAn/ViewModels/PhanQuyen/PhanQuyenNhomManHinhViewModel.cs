namespace QuanLyDuAn.ViewModels.PhanQuyen;

public class PhanQuyenNhomManHinhViewModel
{
    public int MaManHinh { get; set; }
    public string TenManHinh { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int GroupOrder { get; set; }
    public int ScreenOrder { get; set; }
    public string SearchText { get; set; } = string.Empty;
    public List<PhanQuyenItemViewModel> DanhSachQuyen { get; set; } = new();
}
