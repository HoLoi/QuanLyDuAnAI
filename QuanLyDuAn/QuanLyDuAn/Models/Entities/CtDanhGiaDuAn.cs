namespace QuanLyDuAn.Models.Entities;

public partial class CtDanhGiaDuAn
{
    public int MaChiTietDGDA { get; set; }
    public int MaDanhGiaDuAn { get; set; }
    public int? MaTieuChi { get; set; }
    public string? NhanXetDuAn { get; set; }
    public int? DiemDanhGiaDA { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
