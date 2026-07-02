namespace QuanLyDuAn.ViewModels.ChatDuAn
{
    public class ChatDuAnPhongBatchViewModel
    {
        public List<ChatDuAnPhongItemViewModel> DanhSachPhong { get; set; } = new();
        public int SoLuongPhong => DanhSachPhong.Count;
        public bool HasMore { get; set; }
        public int? NextRoomId { get; set; }
        public string? TuKhoa { get; set; }
    }
}
