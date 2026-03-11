namespace Cocorra.DAL;

public enum ParticipantStatus
{
    Active = 0,          // جوه الروم ومسموح له يسمع/يتكلم
    Left = 1,            // خرج بمزاجه
    Kicked = 2,          // انطرد

    // --- الجديد ---
    PendingApproval = 3, // "بيخبط عالباب": بعت طلب ومستني الكوتش
    Rejected = 4         // الكوتش رفض طلبه
}
