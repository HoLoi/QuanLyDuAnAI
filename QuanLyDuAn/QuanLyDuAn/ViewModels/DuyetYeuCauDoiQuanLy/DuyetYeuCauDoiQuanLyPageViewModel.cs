namespace QuanLyDuAn.ViewModels.DuyetYeuCauDoiQuanLy
{
    public class DuyetYeuCauDoiQuanLyPageViewModel
    {
        public string? TrangThai { get; set; }
        public int? MaDuAn { get; set; }
        public string? TuKhoa { get; set; }

        public List<DuyetYeuCauDoiQuanLyItemViewModel> DanhSachYeuCau { get; set; } = new();
        public HashSet<string> Permissions { get; set; } = new();
    }
}
