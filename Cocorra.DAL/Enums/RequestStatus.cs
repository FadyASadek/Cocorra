namespace Cocorra.DAL.Enums;

public enum RequestStatus
{
    Pending = 0,    // لسه معلق
    Approved = 1,   // الكوتش وافق (ممكن يظهر لليوزر: "Wait for the room soon!")
    Rejected = 2,   // الكوتش رفض
    Completed = 3   // الروم اتعملت بالفعل
}
