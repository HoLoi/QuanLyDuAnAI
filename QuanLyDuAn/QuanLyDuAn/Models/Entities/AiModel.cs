namespace QuanLyDuAn.Models.Entities;

public partial class AiModel
{
    public int MaModel { get; set; }
    public string? TenModel { get; set; }
    public int? SoLuongDuLieu { get; set; }
    public double? DoChinhXac { get; set; }
    public DateTime? NgayTao { get; set; }
    public string? MoTaModel { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
