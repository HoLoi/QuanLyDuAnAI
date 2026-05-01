using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.ThanhVienTeam
{
    public class ThanhVienTeamViewModel
    {
        public int MaTeam { get; set; }
        public string TenTeam { get; set; } = string.Empty;
        public int MaNguoiDung { get; set; }
        public string HoTenNguoiDung { get; set; } = string.Empty;
        public string VaiTroTrongTeam { get; set; } = string.Empty;
        public DateTime? NgayThamGiaTeam { get; set; }
        public bool IsLeader { get; set; }
        public bool TeamDaCoTruongNhom { get; set; }
    }
}
