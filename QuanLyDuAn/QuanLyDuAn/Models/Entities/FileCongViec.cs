namespace QuanLyDuAn.Models.Entities;

public partial class FileCongViec
{
    public int MaFileCV { get; set; }
    public int MaCongViec { get; set; }
    public string? TenFileCV { get; set; }
    public string? DuongDanFileCV { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
