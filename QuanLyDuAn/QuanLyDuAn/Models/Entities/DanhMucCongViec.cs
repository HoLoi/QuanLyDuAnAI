namespace QuanLyDuAn.Models.Entities;

public partial class DanhMucCongViec
{
    public int MaDanhMucCV { get; set; }
    public int MaDuAn { get; set; }
    public string? TenDanhMucCV { get; set; }
    public string? MoTaDanhMucCV { get; set; }
    public DateTime? NgayTaoDMCV { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
