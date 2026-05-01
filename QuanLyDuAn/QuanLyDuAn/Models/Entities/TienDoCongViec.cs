namespace QuanLyDuAn.Models.Entities;

public partial class TienDoCongViec
{
    public int MaTienDo { get; set; }
    public int MaCongViec { get; set; }
    public int MaNguoiDung { get; set; }
    public int? PhanTram { get; set; }
    public string? GhiChuTienDo { get; set; }
    public DateTime? ThoiGianCapNhat { get; set; }
    public string? TrangThaiTienDo { get; set; }
}
