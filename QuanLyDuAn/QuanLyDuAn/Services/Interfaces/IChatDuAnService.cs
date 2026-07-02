using QuanLyDuAn.ViewModels.ChatDuAn;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IChatDuAnService
    {
        Task<ChatDuAnPageViewModel> GetPageAsync(int? maDuAn, string? tuKhoa);
        Task<ChatDuAnPageViewModel> GetPhongContentAsync(int maPhongChat);
        Task<ChatDuAnTinNhanItemViewModel> GuiTinNhanAsync(ChatDuAnGuiTinNhanViewModel form);
        Task<int> DamBaoPhongChatDuAnAsync(int maDuAn);
        Task DongBoThanhVienPhongChatAsync(int maDuAn);
        Task<List<ChatDuAnPhongItemViewModel>> GetPhongChatDuocThamGiaAsync(string? tuKhoa);
        Task<List<ChatDuAnTinNhanItemViewModel>> GetTinNhanAsync(int maPhongChat);
        Task<ChatDuAnPhongBatchViewModel> GetPhongChatBatchAsync(
            string? tuKhoa,
            int? lastRoomId = null,
            int pageSize = 20);
        Task<ChatDuAnTinNhanBatchViewModel> GetTinNhanBatchAsync(
            int maPhongChat,
            DateTime? cursorThoiGianGui = null,
            int? cursorMaTinNhan = null,
            int pageSize = 30);
        Task<ChatDuAnTinNhanMoiBatchViewModel> GetTinNhanMoiAsync(
            int maPhongChat,
            int afterMessageId,
            int pageSize = 50);
    }
}
