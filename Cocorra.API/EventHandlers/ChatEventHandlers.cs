using Cocorra.API.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using static Cocorra.BLL.Services.Events.ChatEvents;

namespace Cocorra.API.EventHandlers
{
    public class ChatEventHandlers : INotificationHandler<MessagesReadEvent>
    {
        private readonly IHubContext<ChatHub> _chatHubContext;

        public ChatEventHandlers(IHubContext<ChatHub> chatHubContext)
        {
            _chatHubContext = chatHubContext;
        }

        public async Task Handle(MessagesReadEvent notification, CancellationToken cancellationToken)
        {
            await _chatHubContext.Clients.User(notification.SenderId.ToString())
                .SendAsync("MessagesMarkedAsRead", new
                {
                    ReadBy = notification.ReaderId
                }, cancellationToken);
        }
    }
}
