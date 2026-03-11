using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cocorra.DAL.Models;

public class RoomTopicRequest : BaseEntity
{
    // --- تفاصيل الاقتراح ---
    [Required]
    [MaxLength(150)]
    public string TopicTitle { get; set; } = string.Empty; // مثلاً: "ازاي أبدأ في البرمجة"

    [MaxLength(500)]
    public string? Description { get; set; } // تفاصيل: "ياريت تتكلم عن لغة C# ومستقبلها"

    // --- مين اللي اقترح؟ ---
    public Guid RequesterId { get; set; }
    [ForeignKey(nameof(RequesterId))]
    public virtual ApplicationUser? Requester { get; set; }

    // --- الاقتراح ده موجه لمين؟ ---
    // (ممكن يكون null لو اقتراح عام لأي كوتش، أو محدد لكوتش معين)
    public Guid? TargetCoachId { get; set; }
    [ForeignKey(nameof(TargetCoachId))]
    public virtual ApplicationUser? TargetCoach { get; set; }

    // --- حالة الطلب ---
    // Pending: لسه محدش عبره
    // Approved: الكوتش وافق وهيعمل الروم
    // Rejected: موضوع غير مناسب
    // Completed: الروم اتعملت وخلاص
    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    // --- إحصائيات ---
    // عدد الناس اللي عملت "Upvote" أو أيدت الاقتراح ده
    public int VotesCount { get; set; } = 0;
}