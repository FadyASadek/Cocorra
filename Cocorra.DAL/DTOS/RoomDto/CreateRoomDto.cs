using System.ComponentModel.DataAnnotations;

namespace Cocorra.DAL.DTOS.RoomDto;

public class CreateRoomDto
{
    [Required(ErrorMessage = "Room title is required")]
    [MaxLength(100)]
    public string RoomTitle { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Description { get; set; }

    // إعدادات اختيارية (لو مبعتهاش بناخد الديفولت)
    public bool IsPrivate { get; set; } = false; // عامة ولا خاصة
    public int TotalCapacity { get; set; } = 50;
    public int StageCapacity { get; set; } = 5;
    public int DefaultSpeakerDurationMinutes { get; set; } = 5;

    // نوع الاختيار (0: أوتوماتيك، 1: يدوي)
    public RoomSelectionMode SelectionMode { get; set; } = RoomSelectionMode.Manual_CoachDecision;
}
