namespace QuanLyDuAn.Models.Entities;

public partial class TienDoCongViec
{
    public int MaTienDo { get; set; }
    public int MaChiTietCV { get; set; }
    public int MaNguoiDung { get; set; }
    public int? MaNguoiDungDuyet { get; set; }
    public DateTime? ThoiGianDuyet { get; set; }
    public string? GhiChuDuyet { get; set; }
    public int? PhanTram { get; set; }
    public string? GhiChuTienDo { get; set; }
    public DateTime? ThoiGianCapNhat { get; set; }
    public string? TrangThaiCTCVDeXuat { get; set; }
    public string? TrangThaiTienDo { get; set; }
}
