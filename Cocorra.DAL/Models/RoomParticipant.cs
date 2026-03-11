using System.ComponentModel.DataAnnotations.Schema;

namespace Cocorra.DAL.Models;

public class RoomParticipant
{
    // --- الربط (Composite Key) ---
    public Guid RoomId { get; set; }
    [ForeignKey(nameof(RoomId))]
    public virtual Room? Room { get; set; }

    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser? User { get; set; }

    // --- الحالة العامة ---
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Active;

    // ضفنا دي بدل CreatedAt اللي كانت في BaseEntity
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // --- حالة الاستيدج والمايك ---
    public bool IsOnStage { get; set; } = false;
    public bool IsHandRaised { get; set; } = false;
    public bool IsMuted { get; set; } = true;

    // --- منطق حساب الوقت ---
    public double TotalSpokenSeconds { get; set; } = 0;
    public DateTime? LastUnmutedAt { get; set; }
    public int ExtraMinutesGranted { get; set; } = 0;
}