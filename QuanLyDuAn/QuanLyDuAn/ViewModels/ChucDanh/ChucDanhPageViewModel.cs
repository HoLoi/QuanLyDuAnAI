namespace QuanLyDuAn.ViewModels.ChucDanh
{
    public class ChucDanhPageViewModel
    {
        public List<ChucDanhViewModel> DanhSach { get; set; } = new();
        public ChucDanhCreateUpdateViewModel Form { get; set; } = new();
        public HashSet<string> Permissions { get; set; } = new();
    }
}
