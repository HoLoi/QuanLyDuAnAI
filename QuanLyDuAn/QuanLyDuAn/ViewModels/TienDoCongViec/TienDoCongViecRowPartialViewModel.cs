namespace QuanLyDuAn.ViewModels.TienDoCongViec
{
    public class TienDoCongViecRowPartialViewModel
    {
        public TienDoCongViecItemViewModel Item { get; set; } = new();
        public TienDoCongViecFilterViewModel Filter { get; set; } = new();
        public bool CanUpdate { get; set; }
        public bool CanApprove { get; set; }
        public string CollapseId { get; set; } = string.Empty;
        public string LichSuCollapseId { get; set; } = string.Empty;
    }
}
