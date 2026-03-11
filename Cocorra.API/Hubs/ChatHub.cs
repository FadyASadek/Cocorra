using Cocorra.DAL;
using Cocorra.DAL.Repository.RoomRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Cocorra.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IRoomRepository _roomRepo;

        public ChatHub(IRoomRepository roomRepo)
        {
            _roomRepo = roomRepo;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(string roomId)
        {
            var userIdString = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                throw new HubException("Unauthorized user.");
            }

            if (!Guid.TryParse(roomId, out Guid roomGuid))
            {
                throw new HubException("Invalid Room ID.");
            }

            var participant = await _roomRepo.GetParticipantAsync(roomGuid, userId);

            if (participant == null)
            {
                throw new HubException("You are not a member of this room.");
            }

            if (participant.Status == ParticipantStatus.Kicked || participant.Status == ParticipantStatus.Rejected)
            {
                throw new HubException("You are not allowed to join this room.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            await Clients.Group(roomId).SendAsync("UserJoined", new
            {
                UserId = userId,
                Name = participant.User?.FirstName + " " + participant.User?.LastName,
                IsOnStage = participant.IsOnStage
            });
        }
    }
}