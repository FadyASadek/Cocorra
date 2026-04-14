using System;
using Cocorra.DAL.Enums;

namespace Cocorra.DAL.DTOS.RoomDto
{
    public class RoomSummaryDto
    {
        public Guid Id { get; set; }
        public string RoomTitle { get; set; } = string.Empty;
        public string? Description { get; set; }

        public RoomStatus Status { get; set; }
        public DateTime? ScheduledStartDate { get; set; }
        public int DurationHours { get; set; }
        public RoomCategory Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public int ListenersCount { get; set; }

        public bool IsReminderSetByMe { get; set; }

        // Host info
        public Guid HostId { get; set; }
        public string HostName { get; set; } = string.Empty;
        public string? HostProfilePicture { get; set; }

        // Room image
        public string? RoomImage { get; set; }
    }
}
