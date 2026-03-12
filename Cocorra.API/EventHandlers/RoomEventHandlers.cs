using Cocorra.API.Hubs;
using Cocorra.BLL.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace Cocorra.API.EventHandlers
{
    public class RoomEventHandlers :
        INotificationHandler<UserRequestedToJoinRoomEvent>,
        INotificationHandler<UserApprovedToJoinRoomEvent>
    {
        private readonly IHubContext<RoomHub> _hubContext;

        public RoomEventHandlers(IHubContext<RoomHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(UserRequestedToJoinRoomEvent notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.User(notification.HostId.ToString()).SendAsync("NewJoinRequest", new
            {
                UserId = notification.UserId,
                RoomId = notification.RoomId,
                Message = "Someone wants to join your private room."
            }, cancellationToken);
        }

        public async Task Handle(UserApprovedToJoinRoomEvent notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.User(notification.TargetUserId.ToString()).SendAsync("RequestApproved", new
            {
                RoomId = notification.RoomId,
                Message = "Your request was approved! Joining room..."
            }, cancellationToken);
        }
    }
}