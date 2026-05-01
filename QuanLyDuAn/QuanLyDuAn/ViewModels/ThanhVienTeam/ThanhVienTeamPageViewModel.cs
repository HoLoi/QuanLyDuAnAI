namespace QuanLyDuAn.ViewModels.ThanhVienTeam
{
    public class ThanhVienTeamPageViewModel
    {
        public List<ThanhVienTeamViewModel> DanhSach { get; set; } = new();
        public ThanhVienTeamCreateUpdateViewModel Form { get; set; } = new();
        public List<TeamOptionViewModel> DanhSachTeam { get; set; } = new();
        public List<NhanSuOptionViewModel> DanhSachNhanSu { get; set; } = new();
        public string? TuKhoa { get; set; }
        public int? LocMaTeam { get; set; }
        public string? LocVaiTroLanhDao { get; set; }
        public bool? CheDoGanTruongNhom { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
    }
}
