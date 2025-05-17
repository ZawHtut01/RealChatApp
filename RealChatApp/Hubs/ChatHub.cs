
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
[Authorize]
public class ChatHub : Hub
{
    //public async Task SendMessage(int senderId, int receiverId, string message)
    //{
    //    // Send to both sender and receiver groups
    //    await Clients.Group($"User_{senderId}").SendAsync("ReceiveMessage", senderId, receiverId, message);
    //    await Clients.Group($"User_{receiverId}").SendAsync("ReceiveMessage", senderId, receiverId, message);
    //}

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendTyping(int senderId, int receiverId, bool isTyping)
    {
        await Clients.User(receiverId.ToString()).SendAsync("UserTyping", senderId, receiverId, isTyping);
    }

    public async Task SendMessage(int senderId, int receiverId, string message, string timestamp)
    {
        await Clients.Users(new[] { senderId.ToString(), receiverId.ToString() })
            .SendAsync("ReceiveMessage", senderId, receiverId, message, timestamp);
    }
}

