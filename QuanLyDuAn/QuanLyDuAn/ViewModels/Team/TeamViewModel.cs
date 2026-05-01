using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.Team
{
    public class TeamViewModel
    {
        public int MaTeam { get; set; }
        public string TenTeam { get; set; } = string.Empty;
        public string? MoTaTeam { get; set; }
        public DateTime? NgayLapTeam { get; set; }
        public string TrangThaiTeam { get; set; } = string.Empty;
        public int SoLuongThanhVien { get; set; }
        public bool CoTruongNhom { get; set; }
    }
}
