using System.ComponentModel.DataAnnotations.Schema;

namespace Cocorra.DAL.Models;

public class TopicVote
{
    // المفتاح المركب (User + Topic)
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser? User { get; set; }

    public Guid TopicRequestId { get; set; }
    [ForeignKey(nameof(TopicRequestId))]
    public virtual RoomTopicRequest? TopicRequest { get; set; }

    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
}