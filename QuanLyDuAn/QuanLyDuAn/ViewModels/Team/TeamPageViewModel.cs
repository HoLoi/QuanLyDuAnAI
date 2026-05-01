namespace QuanLyDuAn.ViewModels.Team
{
    public class TeamPageViewModel
    {
        public List<TeamViewModel> DanhSach { get; set; } = new();
        public TeamCreateUpdateViewModel Form { get; set; } = new();
        public string? TuKhoa { get; set; }
        public string? LocTrangThaiTeam { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
