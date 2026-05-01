namespace QuanLyDuAn.ViewModels.TeamDuAn
{
    public class TeamDuAnMemberOptionViewModel
    {
        public int MaNguoiDung { get; set; }
        public string HoTenNguoiDung { get; set; } = string.Empty;
        public string VaiTroTrongTeam { get; set; } = string.Empty;
        public bool IsLeader { get; set; }
        public bool IsSelected { get; set; }
    }
}
