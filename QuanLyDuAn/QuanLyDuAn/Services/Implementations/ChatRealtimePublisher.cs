using Microsoft.AspNetCore.SignalR;
using QuanLyDuAn.Hubs;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.ChatDuAn;

namespace QuanLyDuAn.Services.Implementations;

public sealed class ChatRealtimePublisher : IChatRealtimePublisher
{
    public const string MessageCreatedEvent = "MessageCreated";

    private readonly IHubContext<ChatDuAnHub> _hubContext;
    private readonly ILogger<ChatRealtimePublisher> _logger;

    public ChatRealtimePublisher(
        IHubContext<ChatDuAnHub> hubContext,
        ILogger<ChatRealtimePublisher> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task PublishMessageCreatedAsync(
        IReadOnlyCollection<string> identityUserIds,
        ChatRealtimeMessageDto message,
        CancellationToken cancellationToken = default)
    {
        if (identityUserIds.Count == 0)
        {
            return;
        }

        try
        {
            await _hubContext.Clients
                .Users(identityUserIds)
                .SendAsync(MessageCreatedEvent, message, cancellationToken);

            _logger.LogDebug(
                "Đã phát realtime tin {MessageId}, phòng {RoomId}, {RecipientCount} người nhận.",
                message.MaTinNhan,
                message.MaPhongChat,
                identityUserIds.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Phát realtime thất bại cho tin {MessageId}, phòng {RoomId}. Tin đã được lưu.",
                message.MaTinNhan,
                message.MaPhongChat);
        }
    }
}
