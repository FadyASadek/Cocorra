
using MediatR;

namespace Cocorra.BLL.Events
{
    public record UserRequestedToJoinRoomEvent(Guid HostId, Guid UserId, Guid RoomId) : INotification;

    public record UserApprovedToJoinRoomEvent(Guid TargetUserId, Guid RoomId) : INotification;
}