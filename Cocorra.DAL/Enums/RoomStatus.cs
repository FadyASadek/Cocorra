namespace Cocorra.DAL;

public enum RoomStatus
{
    Scheduled = 0, // لسه مابدأتش (مجدولة)
    Live = 1,      // شغالة دلوقتي والناس بتتكلم
    Ended = 2,     // خلصت واتقفلت
    Cancelled = 3  // الكوتش لغاها قبل ما تبدأ
}
