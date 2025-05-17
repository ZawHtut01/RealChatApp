
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
[Authorize]
public class ChatHub : Hub
{
    public async Task SendMessage(int senderId, int receiverId, string message)
    {
        // Send to both sender and receiver groups
        await Clients.Group($"User_{senderId}").SendAsync("ReceiveMessage", senderId, receiverId, message);
        await Clients.Group($"User_{receiverId}").SendAsync("ReceiveMessage", senderId, receiverId, message);
    }

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
}



//using Microsoft.AspNetCore.SignalR;

//public class ChatHub : Hub
//{
//    public async Task SendMessage(string senderId, string receiverId, string message)
//    {
//        // Send to both sender and receiver
//        await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, receiverId, message);
//        await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, receiverId, message);
//    }

//    public override Task OnConnectedAsync()
//    {
//        var httpContext = Context.GetHttpContext();
//        var userId = httpContext?.Session.GetInt32("UserId");

//        if (userId != null)
//        {
//            // Map user to SignalR connection
//            Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
//        }

//        return base.OnConnectedAsync();
//    }
//}
