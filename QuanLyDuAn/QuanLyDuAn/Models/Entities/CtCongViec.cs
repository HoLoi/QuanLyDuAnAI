namespace QuanLyDuAn.Models.Entities;

public partial class CtCongViec
{
    public int MaChiTietCV { get; set; }
    public int MaCongViec { get; set; }
    public string? NoiDungChiTietCV { get; set; }
    public DateTime? NgayTaoCTCV { get; set; }
    public DateTime? NgayBaoCaoCTCV { get; set; }
    public double? PhanTramHoanThanhCTCV { get; set; }
    public string? TrangThaiCTCV { get; set; }
}
