namespace QuanLyDuAn.Models.Entities;

public partial class NhatKyPhanCongCongViec
{
    public int MaNhatKyPCCV { get; set; }
    public int MaNguoiDung { get; set; }
    public int MaCongViec { get; set; }
    public int MaNguoiDungGhi { get; set; }
    public string? HanhDongPCCV { get; set; }
    public DateTime? ThoiGianPCCV { get; set; }
}
