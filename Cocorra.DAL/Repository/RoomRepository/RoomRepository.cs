using Cocorra.DAL.Data;
using Cocorra.DAL.Models;
using Cocorra.DAL.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Cocorra.DAL.Repository.RoomRepository;

public class RoomRepository : GenericRepositoryAsync<Room>, IRoomRepository
{
    // بنمرر الـ Context للـ Base Class (GenericRepository)
    public RoomRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    // --- تنفيذ الدوال الخاصة اللي زودناها في الانترفيس ---

    public async Task<RoomParticipant?> GetParticipantAsync(Guid roomId, Guid userId)
    {
        // هنا بنستخدم _dbContext اللي ورثناه من الـ GenericRepository
        return await _dbContext.RoomParticipants
                             .Include(p => p.User)
                             .FirstOrDefaultAsync(p => p.RoomId == roomId && p.UserId == userId);
    }

    public async Task<List<RoomParticipant>> GetRoomParticipantsAsync(Guid roomId)
    {
        return await _dbContext.RoomParticipants
                             .Where(p => p.RoomId == roomId)
                             .Include(p => p.User)
                             .OrderByDescending(p => p.IsOnStage)
                             .ThenBy(p => p.JoinedAt)
                             .ToListAsync();
    }

    public async Task<List<RoomParticipant>> GetStageSpeakersAsync(Guid roomId)
    {
        return await _dbContext.RoomParticipants
                             .Where(p => p.RoomId == roomId && p.IsOnStage == true)
                             .Include(p => p.User)
                             .ToListAsync();
    }
}