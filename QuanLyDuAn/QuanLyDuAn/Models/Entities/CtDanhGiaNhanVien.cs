namespace QuanLyDuAn.Models.Entities;

public partial class CtDanhGiaNhanVien
{
    public int MaChiTietDGNV { get; set; }
    public int MaDanhGiaNhanVien { get; set; }
    public int MaCongViec { get; set; }
    public string? NoiDungDanhGiaNhanVien { get; set; }
    public int? DiemDanhGiaNV { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
