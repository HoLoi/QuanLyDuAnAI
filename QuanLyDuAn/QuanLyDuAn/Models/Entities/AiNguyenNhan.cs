namespace QuanLyDuAn.Models.Entities;

public partial class AiNguyenNhan
{
    public int MaAINguyenNhan { get; set; }
    public int MaDuAn { get; set; }
    public int MaDMNguyenNhan { get; set; }
    public double? DoTinCay { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
