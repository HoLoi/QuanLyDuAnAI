namespace QuanLyDuAn.Models.Entities;

public partial class NhanVienTeam
{
    public int MaNguoiDung { get; set; }
    public int MaTeam { get; set; }
    public string? VaiTroTrongTeam { get; set; }
    public DateTime? NgayThamGiaTeam { get; set; }
    public bool? IsLeader { get; set; }
}
