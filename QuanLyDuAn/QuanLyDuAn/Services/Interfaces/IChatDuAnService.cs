using QuanLyDuAn.ViewModels.ChatDuAn;

namespace QuanLyDuAn.Services.Interfaces
{
    public interface IChatDuAnService
    {
        Task<ChatDuAnPageViewModel> GetPageAsync(int? maDuAn, string? tuKhoa);
        Task GuiTinNhanAsync(ChatDuAnGuiTinNhanViewModel form);
        Task<int> DamBaoPhongChatDuAnAsync(int maDuAn);
        Task DongBoThanhVienPhongChatAsync(int maDuAn);
        Task<List<ChatDuAnPhongItemViewModel>> GetPhongChatDuocThamGiaAsync(string? tuKhoa);
        Task<List<ChatDuAnTinNhanItemViewModel>> GetTinNhanAsync(int maPhongChat);
    }
}
