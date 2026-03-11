using Cocorra.DAL.Models;
using Cocorra.DAL.Repository.GenericRepository;

namespace Cocorra.DAL.Repository.RoomRepository;

public interface IRoomRepository : IGenericRepositoryAsync<Room>
{
    // 2. هنزود هنا الدوال المعقدة اللي الـ Generic ميعرفش يعملها

    // هاتلي بيانات مشارك (عشان المفتاح المركب)
    Task<RoomParticipant?> GetParticipantAsync(Guid roomId, Guid userId);

    // هاتلي قائمة الناس اللي في الروم
    Task<List<RoomParticipant>> GetRoomParticipantsAsync(Guid roomId);

    // هاتلي الناس اللي على الستيدج بس
    Task<List<RoomParticipant>> GetStageSpeakersAsync(Guid roomId);
}