namespace QuanLyDuAn.ViewModels.TeamDuAn
{
    public class TeamDuAnPageViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public bool CoTheChinhSua { get; set; }
        public string? LyDoKhongTheChinhSua { get; set; }

        public int? MaTeamDangChon { get; set; }
        public List<int> SelectedMaNguoiDung { get; set; } = new();

        public List<TeamDuAnItemViewModel> DanhSachTeamPhuTrach { get; set; } = new();
        public List<TeamDuAnTeamOptionViewModel> DanhSachTeamCoTheGan { get; set; } = new();
        public List<TeamDuAnMemberOptionViewModel> DanhSachThanhVienTeam { get; set; } = new();

        public string? TuKhoa { get; set; }
        public int? LocMaLoaiDuAn { get; set; }
        public string? LocTrangThaiDuAn { get; set; }

        public HashSet<string> Permissions { get; set; } = new();
    }
}
