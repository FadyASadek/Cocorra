using Cocorra.BLL.Events;
using Cocorra.DAL;
using Cocorra.DAL.DTOS.RoomDto;
using Cocorra.DAL.Enums;
using Cocorra.DAL.Models;
using Cocorra.DAL.Repository.RoomRepository;
using Core.Base;
using MediatR;
using Microsoft.AspNetCore.SignalR;
namespace Cocorra.BLL.Services.RoomService;

public class RoomService : ResponseHandler, IRoomService
{
    private readonly IRoomRepository _roomRepo;
    private readonly IMediator _mediator;
    public RoomService(IRoomRepository roomRepo, IMediator mediator)
    {
        _roomRepo = roomRepo;
        _mediator = mediator;
    }

    public async Task<Response<Guid>> CreateRoomAsync(CreateRoomDto dto, Guid hostId)
    {
        try
        {
            var status = RoomStatus.Live;
            if (dto.ScheduledStartDate.HasValue && dto.ScheduledStartDate > DateTime.UtcNow)
            {
                status = RoomStatus.Scheduled;
            }

            var room = new Room
            {
                RoomTitle = dto.RoomTitle,
                Description = dto.Description,
                TotalCapacity = dto.TotalCapacity,
                StageCapacity = dto.StageCapacity,
                DefaultSpeakerDurationMinutes = dto.DefaultSpeakerDurationMinutes,
                IsPrivate = dto.IsPrivate,
                SelectionMode = dto.SelectionMode,
                HostId = hostId,
                StartDate = dto.ScheduledStartDate ?? DateTime.UtcNow,
                status = status,
                CreatedAt = DateTime.UtcNow
            };

            if (status == RoomStatus.Live)
            {
                var hostParticipant = new RoomParticipant
                {
                    UserId = hostId,
                    Status = ParticipantStatus.Active,
                    IsOnStage = true,
                    IsMuted = false,
                    JoinedAt = DateTime.UtcNow,
                    LastUnmutedAt = DateTime.UtcNow
                };
                room.Participants.Add(hostParticipant);
            }

            await _roomRepo.AddAsync(room);
            return Success(room.Id);
        }
        catch (Exception ex)
        {
            return BadRequest<Guid>($"Failed to create room: {ex.Message}");
        }
    }
    public async Task<Response<bool>> JoinRoomAsync(Guid roomId, Guid userId)
    {
        var room = await _roomRepo.GetByIdAsync(roomId);
        if (room == null) return NotFound<bool>("Room not found.");

        if (room.status == RoomStatus.Scheduled)
        {
            return BadRequest<bool>("This room has not started yet. You can set a reminder instead.");
        }
        if (room.status == RoomStatus.Ended || room.status == RoomStatus.Cancelled)
        {
            return BadRequest<bool>("This room is no longer available.");
        }

        var allParticipants = await _roomRepo.GetRoomParticipantsAsync(roomId);
        var activeCount = allParticipants.Count(p => p.Status == ParticipantStatus.Active || p.Status == ParticipantStatus.PendingApproval);

        var existingParticipant = allParticipants.FirstOrDefault(p => p.UserId == userId);

        if (existingParticipant != null)
        {
            if (existingParticipant.Status == ParticipantStatus.Active) return Success(true);
            if (existingParticipant.Status == ParticipantStatus.Kicked) return BadRequest<bool>("You are banned from this room.");

            
            if (activeCount >= room.TotalCapacity) return BadRequest<bool>("Room is full.");

            existingParticipant.Status = room.IsPrivate ? ParticipantStatus.PendingApproval : ParticipantStatus.Active;
            existingParticipant.JoinedAt = DateTime.UtcNow;
            existingParticipant.IsOnStage = false;
            existingParticipant.IsMuted = true;

            await _roomRepo.UpdateParticipantAsync(existingParticipant);
            await _roomRepo.SaveChangesAsync();
            return Success(room.IsPrivate ? false : true, room.IsPrivate ? "Request sent." : "Rejoined successfully.");
        }

        if (activeCount >= room.TotalCapacity)
        {
            return BadRequest<bool>("Room is full.");
        }

        var newParticipant = new RoomParticipant
        {
            RoomId = roomId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            IsOnStage = false,
            IsMuted = true,
            Status = room.IsPrivate ? ParticipantStatus.PendingApproval : ParticipantStatus.Active
        };
        if (room.IsPrivate)
        {
            var notification = new Notification
            {
                UserId = room.HostId, 
                Title = "New Join Request 🚪",
                Message = "Someone has requested to join your private room.",
                Type = NotificationType.RoomReminder, 
                ReferenceId = room.Id,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            await _mediator.Publish(new UserRequestedToJoinRoomEvent(room.HostId, userId, roomId));

            await _roomRepo.AddNotificationsAsync(new List<Notification> { notification });
        }

        await _roomRepo.AddParticipantAsync(newParticipant);
        await _roomRepo.SaveChangesAsync();

        return Success(room.IsPrivate ? false : true, room.IsPrivate ? "Request sent, waiting for approval." : "Joined successfully.");
    }
    public async Task<Response<bool>> ApproveUserAsync(Guid roomId, Guid targetUserId, Guid hostId)
    {
        var room = await _roomRepo.GetByIdAsync(roomId);
        if (room == null) return NotFound<bool>("Room not found.");

        if (room.HostId != hostId)
            return BadRequest<bool>("Only the host can approve join requests.");

        var participant = await _roomRepo.GetParticipantAsync(roomId, targetUserId);
        if (participant == null)
            return NotFound<bool>("User request not found.");

        if (participant.Status == ParticipantStatus.Active)
            return Success(true, "User is already active in the room.");

        participant.Status = ParticipantStatus.Active;
        await _roomRepo.UpdateParticipantAsync(participant);

        var notification = new Notification
        {
            UserId = targetUserId,
            Title = "Request Approved ✅",
            Message = $"Your request to join the room '{room.RoomTitle}' has been approved! You can enter now.",
            Type = NotificationType.RoomReminder, 
            ReferenceId = room.Id,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };
        await _roomRepo.AddNotificationsAsync(new List<Notification> { notification });
        await _roomRepo.SaveChangesAsync();
        await _mediator.Publish(new UserApprovedToJoinRoomEvent(targetUserId, roomId));
        return Success(true, "User approved successfully.");
    }
    public async Task<Response<RoomStateDto>> GetRoomStateAsync(Guid roomId, Guid currentUserId)
    {
        var room = await _roomRepo.GetByIdAsync(roomId);
        if (room == null) return NotFound<RoomStateDto>("Room not found.");

        var currentParticipant = await _roomRepo.GetParticipantAsync(roomId, currentUserId);
        if (currentParticipant == null || currentParticipant.Status != ParticipantStatus.Active)
        {
            return BadRequest<RoomStateDto>("You are not an active member of this room.");
        }

        var participants = await _roomRepo.GetRoomParticipantsAsync(roomId);
        var activeParticipants = participants.Where(p => p.Status == ParticipantStatus.Active).ToList();

        var roomState = new RoomStateDto
        {
            RoomId = room.Id,
            RoomTitle = room.RoomTitle,
            HostId = room.HostId,
            TotalCapacity = room.TotalCapacity,
            StageCapacity = room.StageCapacity,
            Participants = activeParticipants.Select(p => new ParticipantStateDto
            {
                UserId = p.UserId,
                Name = p.User?.FirstName + " " + p.User?.LastName,
                IsOnStage = p.IsOnStage,
                IsMuted = p.IsMuted,
                IsHandRaised = p.IsHandRaised,
                JoinedAt = p.JoinedAt
            }).ToList()
        };

        return Success(roomState);
    }

    public async Task<Response<IEnumerable<RoomSummaryDto>>> GetRoomsFeedAsync(Guid currentUserId)
    {
        var activeRooms = await _roomRepo.GetActiveRoomsAsync();
        var resultList = new List<RoomSummaryDto>();

        foreach (var room in activeRooms)
        {
            var dto = new RoomSummaryDto
            {
                Id = room.Id,
                RoomTitle = room.RoomTitle,
                Description = room.Description,
                Status = room.status,
                ScheduledStartDate = room.StartDate,
                IsReminderSetByMe = false,
                ListenersCount = 0,
                HostName = room.Host!.FirstName + " " + room.Host.LastName,
            };

            if (room.status == RoomStatus.Live)
            {
                var participants = await _roomRepo.GetRoomParticipantsAsync(room.Id);
                dto.ListenersCount = participants.Count(p => p.Status == ParticipantStatus.Active);
            }
            else if (room.status == RoomStatus.Scheduled)
            {
                var reminder = await _roomRepo.GetRoomReminderAsync(room.Id, currentUserId);
                dto.IsReminderSetByMe = reminder != null;

                dto.ListenersCount = await _roomRepo.GetRoomRemindersCountAsync(room.Id);
            }

            resultList.Add(dto);
        }

        var sortedList = resultList
            .OrderBy(r => r.Status == RoomStatus.Live ? 0 : 1)
            .ThenBy(r => r.ScheduledStartDate)
            .ToList();

        return Success<IEnumerable<RoomSummaryDto>>(sortedList);
    }

    public async Task<Response<string>> StartScheduledRoomAsync(Guid roomId, Guid hostId)
    {
        var room = await _roomRepo.GetByIdAsync(roomId);
        if (room == null) return NotFound<string>("Room not found.");

        if (room.HostId != hostId)
            return BadRequest<string>("Only the host can start this room.");

        if (room.status == RoomStatus.Live)
            return BadRequest<string>("This room is already live.");

        if (room.status == RoomStatus.Ended || room.status == RoomStatus.Cancelled)
            return BadRequest<string>("This room is no longer available.");

        room.status = RoomStatus.Live;
        await _roomRepo.UpdateAsync(room); 

        var hostParticipant = new RoomParticipant
        {
            RoomId = roomId,
            UserId = hostId,
            Status = ParticipantStatus.Active,
            IsOnStage = true,
            IsMuted = false,
            JoinedAt = DateTime.UtcNow,
            LastUnmutedAt = DateTime.UtcNow
        };
        await _roomRepo.AddParticipantAsync(hostParticipant);

        var reminders = await _roomRepo.GetRemindersByRoomIdAsync(roomId);
        if (reminders.Any())
        {
            var notifications = reminders.Select(r => new Notification
            {
                UserId = r.UserId,
                Title = "Room Starting Now! 🎙️",
                Message = $"The room '{room.RoomTitle}' has just started. Join now!",
                Type = NotificationType.RoomReminder,
                ReferenceId = room.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _roomRepo.AddNotificationsAsync(notifications);

            await _roomRepo.RemoveRemindersAsync(reminders);
        }

        await _roomRepo.SaveChangesAsync();

        return Success("Room is now live and notifications have been sent!");
    }
    public async Task<Response<string>> ToggleReminderAsync(Guid roomId, Guid userId)
    {
        var room = await _roomRepo.GetByIdAsync(roomId);
        if (room == null) return NotFound<string>("Room not found.");

        if (room.status != RoomStatus.Scheduled)
            return BadRequest<string>("You can only set reminders for scheduled rooms.");

        var existingReminder = await _roomRepo.GetRoomReminderAsync(roomId, userId);

        if (existingReminder != null)
        {
            await _roomRepo.RemoveRoomReminderAsync(existingReminder);
            return Success("Reminder removed.");
        }
        else
        {
            var reminder = new RoomReminder
            {
                RoomId = roomId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            await _roomRepo.AddRoomReminderAsync(reminder);
            return Success("Reminder set successfully.");
        }
    }
}