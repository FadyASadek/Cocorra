using Cocorra.DAL.Data;
using Cocorra.DAL.Models;
using Cocorra.DAL.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Cocorra.DAL.Repository.UserBlockRepository
{
    public class UserBlockRepository : GenericRepositoryAsync<UserBlock>, IUserBlockRepository
    {
        public UserBlockRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task BlockUserAsync(Guid blockerId, Guid blockedId)
        {
            var exists = await _dbContext.UserBlocks
                .AnyAsync(ub => ub.BlockerId == blockerId && ub.BlockedId == blockedId);
            
            if (!exists)
            {
                await _dbContext.UserBlocks.AddAsync(new UserBlock 
                { 
                    BlockerId = blockerId, 
                    BlockedId = blockedId,
                    CreatedAt = DateTime.UtcNow
                });
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UnblockUserAsync(Guid blockerId, Guid blockedId)
        {
            var block = await _dbContext.UserBlocks
                .FirstOrDefaultAsync(ub => ub.BlockerId == blockerId && ub.BlockedId == blockedId);
            
            if (block != null)
            {
                _dbContext.UserBlocks.Remove(block);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> IsBlockedAsync(Guid userId1, Guid userId2)
        {
            return await _dbContext.UserBlocks
                .AnyAsync(ub => 
                    (ub.BlockerId == userId1 && ub.BlockedId == userId2) || 
                    (ub.BlockerId == userId2 && ub.BlockedId == userId1));
        }
    }
}
