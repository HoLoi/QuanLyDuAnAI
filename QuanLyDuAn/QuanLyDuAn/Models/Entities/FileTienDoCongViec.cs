namespace QuanLyDuAn.Models.Entities;

public partial class FileTienDoCongViec
{
    public int MaFileTDCV { get; set; }
    public int MaTienDo { get; set; }
    public string? TenFileTDCV { get; set; }
    public string? DuongDanFileTDCV { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
