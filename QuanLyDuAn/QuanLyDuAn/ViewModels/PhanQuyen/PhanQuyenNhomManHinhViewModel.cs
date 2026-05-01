namespace QuanLyDuAn.ViewModels.PhanQuyen;

public class PhanQuyenNhomManHinhViewModel
{
    public int MaManHinh { get; set; }
    public string TenManHinh { get; set; } = string.Empty;
    public List<PhanQuyenItemViewModel> DanhSachQuyen { get; set; } = new();
}
