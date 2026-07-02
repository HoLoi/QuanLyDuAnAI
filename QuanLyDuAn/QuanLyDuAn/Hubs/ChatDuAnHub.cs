using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using QuanLyDuAn.Constants;

namespace QuanLyDuAn.Hubs;

[Authorize]
public sealed class ChatDuAnHub : Hub
{
    private readonly ILogger<ChatDuAnHub> _logger;

    public ChatDuAnHub(ILogger<ChatDuAnHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var user = Context.User;
        var coQuyenXem = user?.Claims.Any(x =>
            x.Value.Equals(Permissions.Chat.Xem, StringComparison.OrdinalIgnoreCase)) == true;
        var laAdmin = user?.IsInRole("Admin") == true;

        if (!coQuyenXem || laAdmin)
        {
            _logger.LogWarning(
                "Từ chối kết nối ChatDuAnHub cho connection {ConnectionId}.",
                Context.ConnectionId);
            Context.Abort();
            return;
        }

        _logger.LogDebug("ChatDuAnHub connected: {ConnectionId}.", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogDebug(
            exception,
            "ChatDuAnHub disconnected: {ConnectionId}.",
            Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
