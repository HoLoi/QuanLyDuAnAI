namespace QuanLyDuAn.ViewModels.DuyetYeuCauDoiQuanLy
{
    public class DuyetYeuCauDoiQuanLyDetailsViewModel
    {
        public DuyetYeuCauDoiQuanLyItemViewModel YeuCau { get; set; } = new();
        public HashSet<string> Permissions { get; set; } = new();
    }
}
