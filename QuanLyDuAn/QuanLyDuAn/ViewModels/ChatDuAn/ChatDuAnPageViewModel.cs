namespace QuanLyDuAn.ViewModels.ChatDuAn
{
    public class ChatDuAnPageViewModel
    {
        public List<ChatDuAnPhongItemViewModel> DanhSachPhong { get; set; } = new();
        public ChatDuAnPhongItemViewModel? PhongDangChon { get; set; }
        public List<ChatDuAnTinNhanItemViewModel> DanhSachTinNhan { get; set; } = new();
        public ChatDuAnPhongBatchViewModel PhongBatch { get; set; } = new();
        public ChatDuAnTinNhanBatchViewModel TinNhanBatch { get; set; } = new();
        public ChatDuAnGuiTinNhanViewModel Form { get; set; } = new();

        public string? TuKhoa { get; set; }
        public int? MaDuAnDangChon { get; set; }
        public int? MaPhongChatDangChon { get; set; }
        public HashSet<string> Permissions { get; set; } = new();
        public bool CoTheGuiTinNhan { get; set; }
        public string? ThongBaoTrangThai { get; set; }
    }
}
