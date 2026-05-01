namespace QuanLyDuAn.Models.Entities;

public partial class NhatKyQuanLyDuAn
{
    public int MaNhatKyQLDA { get; set; }
    public int MaDuAn { get; set; }
    public int MaNguoiDung { get; set; }
    public string? NkHanhDongQLDA { get; set; }
    public DateTime? NkThoiGianQLDA { get; set; }
    public DateTime? QLDATuNgay { get; set; }
    public DateTime? QLDADenNgay { get; set; }
}
