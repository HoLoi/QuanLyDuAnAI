namespace QuanLyDuAn.Models.Entities;

public partial class FileDuAn
{
    public int MaFileDA { get; set; }
    public int MaDuAn { get; set; }
    public string? TenFileDA { get; set; }
    public string? DuongDanFileDA { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
