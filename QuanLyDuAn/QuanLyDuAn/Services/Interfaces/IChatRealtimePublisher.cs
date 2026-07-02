using QuanLyDuAn.ViewModels.ChatDuAn;

namespace QuanLyDuAn.Services.Interfaces;

public interface IChatRealtimePublisher
{
    Task PublishMessageCreatedAsync(
        IReadOnlyCollection<string> identityUserIds,
        ChatRealtimeMessageDto message,
        CancellationToken cancellationToken = default);
}
