namespace QuanLyDuAn.Models.Entities;

public partial class NhatKyDuAn
{
    public int MaNhatKyTeamDA { get; set; }
    public int MaTeam { get; set; }
    public int MaDuAn { get; set; }
    public int? TeamCuPhuTrach { get; set; }
    public int? TeamMoiPhuTrach { get; set; }
    public string? HanhDongNKDA { get; set; }
    public DateTime? ThoiGianNKDA { get; set; }
}
