namespace QuanLyDuAn.ViewModels.PhanQuyen;

public class PhanQuyenItemViewModel
{
    public int MaDanhMucQuyen { get; set; }
    public string TenDanhMucQuyen { get; set; } = string.Empty;
    public string? MoTaDanhMucQuyen { get; set; }
    public bool IsSelected { get; set; }
}
