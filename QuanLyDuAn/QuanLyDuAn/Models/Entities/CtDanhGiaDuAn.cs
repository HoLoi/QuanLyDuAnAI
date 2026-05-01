namespace QuanLyDuAn.Models.Entities;

public partial class CtDanhGiaDuAn
{
    public int MaChiTietDGDA { get; set; }
    public int MaDanhGiaDuAn { get; set; }
    public string? NhanXetDuAn { get; set; }
    public string? TieuChi { get; set; }
    public int? DiemDanhGiaDA { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
