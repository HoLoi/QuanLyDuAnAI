namespace QuanLyDuAn.Models.Entities;

public partial class DanhGiaDuAn
{
    public int MaDanhGiaDuAn { get; set; }
    public int MaDuAn { get; set; }
    public int MaNguoiDung { get; set; }
    public int? DiemTongDanhGiaDA { get; set; }
    public string? NhanXetTongDuAn { get; set; }
    public DateTime? NgayDanhGiaDA { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
