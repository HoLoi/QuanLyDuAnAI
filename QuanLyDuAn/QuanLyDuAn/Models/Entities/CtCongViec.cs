namespace QuanLyDuAn.Models.Entities;

public partial class CtCongViec
{
    public int MaChiTietCV { get; set; }
    public int MaCongViec { get; set; }
    public string? TenCTCV { get; set; }
    public string? NoiDungChiTietCV { get; set; }
    public DateTime? NgayTaoCTCV { get; set; }
    public DateTime? NgayBatDauCTCV { get; set; }
    public DateTime? NgayKetThucCTCV { get; set; }
    public string? TrangThaiCTCV { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
