namespace QuanLyDuAn.ViewModels.ChatDuAn
{
    public class ChatDuAnPhongItemViewModel
    {
        public int MaPhongChat { get; set; }
        public int MaDuAn { get; set; }
        public string TenPhong { get; set; } = string.Empty;
        public string TenDuAn { get; set; } = string.Empty;
        public string? TrangThaiDuAn { get; set; }
        public int SoThanhVien { get; set; }
        public int SoTinNhan { get; set; }
        public string? TinNhanMoiNhat { get; set; }
        public DateTime? ThoiGianTinNhanMoiNhat { get; set; }
        public bool DangChon { get; set; }
    }
}
