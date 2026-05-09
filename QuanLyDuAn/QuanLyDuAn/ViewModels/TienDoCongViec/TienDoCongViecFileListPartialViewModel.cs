namespace QuanLyDuAn.ViewModels.TienDoCongViec
{
    public class TienDoCongViecFileListPartialViewModel
    {
        public List<TienDoCongViecFileItemViewModel> DanhSachFile { get; set; } = new();
        public TienDoCongViecFilterViewModel Filter { get; set; } = new();
    }
}
