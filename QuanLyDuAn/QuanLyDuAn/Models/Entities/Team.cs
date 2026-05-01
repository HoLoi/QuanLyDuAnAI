namespace QuanLyDuAn.Models.Entities;

public partial class Team
{
    public int MaTeam { get; set; }
    public string? TenTeam { get; set; }
    public string? MoTaTeam { get; set; }
    public DateTime? NgayLapTeam { get; set; }
    public string? TrangThaiTeam { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
