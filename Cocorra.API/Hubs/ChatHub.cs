using Cocorra.BLL.Services.ChatService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Cocorra.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(string receiverIdString, string content)
        {
            var senderIdString = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(senderIdString, out Guid senderId) ||
                !Guid.TryParse(receiverIdString, out Guid receiverId))
            {
                throw new HubException("Invalid User IDs.");
            }

            if (string.IsNullOrWhiteSpace(content))
                throw new HubException("Message cannot be empty.");

            var result = await _chatService.SaveMessageAsync(senderId, receiverId, content);

            if (!result.Succeeded)
            {
                throw new HubException(result.Message); 
            }

            var messageDto = result.Data;

            await Clients.User(receiverIdString).SendAsync("ReceiveMessage", messageDto);

            await Clients.Caller.SendAsync("MessageSent", messageDto);
        }
    }
}