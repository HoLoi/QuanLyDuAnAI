namespace QuanLyDuAn.ViewModels.ChatDuAn;

public sealed class ChatDuAnTinNhanMoiBatchViewModel
{
    public List<ChatRealtimeMessageDto> DanhSachTinNhan { get; init; } = [];
    public bool HasMore { get; init; }
}
