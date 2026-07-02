namespace QuanLyDuAn.ViewModels.ChatDuAn;

public sealed class ChatRealtimeMessageDto
{
    public int MaTinNhan { get; init; }
    public int MaPhongChat { get; init; }
    public int MaDuAn { get; init; }
    public int MaNguoiDung { get; init; }
    public string TenNguoiGui { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string NoiDungTinNhan { get; init; } = string.Empty;
    public DateTime ThoiGianGui { get; init; }
    public string TenDuAn { get; init; } = string.Empty;
}
