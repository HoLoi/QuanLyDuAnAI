namespace QuanLyDuAn.Models.Entities;

public partial class PhongChat
{
    public int MaPhongChat { get; set; }
    public int MaDuAn { get; set; }
    public string? TenPhong { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
