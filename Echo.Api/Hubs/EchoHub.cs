using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Echo.Api.Data;
using Echo.Api.Models;
using System.Security.Claims;

namespace Echo.Api.Hubs;

[Authorize]
public class EchoHub : Hub
{
    private readonly AppDbContext _db;

    public EchoHub(AppDbContext db)
    {
        _db = db;
    }

    public async Task SendPrivateMessage(string receiverId, string message, string? attachmetUrl = null)
    {
        var senderId = Context.UserIdentifier;
        var senderName = Context.User?.Identity?.Name ?? "Unknown";

        if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId)) return;

        var chatMessage = new ChatMessage
        {
            SenderId = senderId,
            SenderName = senderName,
            ReceiverId = receiverId,
            Text = message,
            AttachmentUrl = attachmetUrl,
            SentAt = DateTime.UtcNow
        };

        _db.ChatMessages.Add(chatMessage);
        await _db.SaveChangesAsync();

        await Clients.User(receiverId).SendAsync("ReceiveMessage", senderName, message, attachmetUrl);

        await Clients.Caller.SendAsync("ReceiveMessage", senderName, message, attachmetUrl);
    }
}