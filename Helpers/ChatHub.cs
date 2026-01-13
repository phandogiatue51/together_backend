using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace Together.Helpers
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int chatId, string message)
        {
            var senderId = Context.UserIdentifier; 
            await Clients.Group(chatId.ToString())
                .SendAsync("ReceiveMessage", senderId, message, DateTime.UtcNow);
        }

        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task LeaveChat(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}