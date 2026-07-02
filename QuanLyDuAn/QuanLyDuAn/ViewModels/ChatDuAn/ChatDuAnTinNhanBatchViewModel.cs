namespace QuanLyDuAn.ViewModels.ChatDuAn
{
    public class ChatDuAnTinNhanBatchViewModel
    {
        public int MaPhongChat { get; set; }
        public List<ChatDuAnTinNhanItemViewModel> DanhSachTinNhan { get; set; } = new();
        public bool HasMore { get; set; }
        public DateTime? CursorThoiGianGui { get; set; }
        public int? CursorMaTinNhan { get; set; }
    }
}
