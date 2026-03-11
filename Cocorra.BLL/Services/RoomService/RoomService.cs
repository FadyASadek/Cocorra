using Cocorra.DAL;
using Cocorra.DAL.DTOS.RoomDto;
using Cocorra.DAL.Models;
using Cocorra.DAL.Repository.RoomRepository;
using Core.Base;

namespace Cocorra.BLL.Services.RoomService;

public class RoomService : ResponseHandler, IRoomService
{
    private readonly IRoomRepository _roomRepo;

    public RoomService(IRoomRepository roomRepo)
    {
        _roomRepo = roomRepo;
    }

    public async Task<Response<Guid>> CreateRoomAsync(CreateRoomDto dto, Guid hostId)
    {
        try
        {
            // 1. تجهيز كائن الروم (Mapping)
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
                StartDate = DateTime.UtcNow,
                status = RoomStatus.Live, // بنفترض إنها بتبدأ فوراً
                CreatedAt = DateTime.UtcNow
            };

            // 2. إضافة الـ Host كأول مشارك في الروم (Owner Logic)
            // الـ Host لازم يدخل الروم أوتوماتيك ويكون على الستيدج
            var hostParticipant = new RoomParticipant
            {
                UserId = hostId,
                Status = ParticipantStatus.Active,
                IsOnStage = true,   // طبعاً صاحب الروم على الستيدج
                IsMuted = false,    // والمايك مفتوح عشان يرحب بالناس
                JoinedAt = DateTime.UtcNow,
                // العلاقة هتتربط لما نضيفها للروم
            };

            // بنضيف المشارك لقائمة مشاركين الروم
            room.Participants.Add(hostParticipant);

            // 3. الحفظ في الداتابيز
            // الـ AddAsync هنا ذكية، هتحفظ الروم وهتحفظ المشارك اللي جواها في نفس الوقت (Transaction)
            await _roomRepo.AddAsync(room);

            // 4. إرجاع الـ ID عشان الفرونت يوجهه لصفحة الروم
            return Success(room.Id);
        }
        catch (Exception ex)
        {
            // ممكن تعمل Logging هنا
            return BadRequest<Guid>($"Failed to create room: {ex.Message}");
        }
    }
}