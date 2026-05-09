namespace QuanLyDuAn.ViewModels.TienDoCongViec
{
    public class TienDoCongViecFileItemViewModel
    {
        public int MaFileTDCV { get; set; }
        public int MaTienDo { get; set; }
        public string TenFileTDCV { get; set; } = string.Empty;
        public DateTime? NgayUploadFileTDCV { get; set; }
        public bool CoTheXoa { get; set; }
    }
}

