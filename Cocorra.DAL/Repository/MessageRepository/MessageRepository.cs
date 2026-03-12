using Cocorra.DAL.Data;
using Cocorra.DAL.DTOS.ChatDto;
using Cocorra.DAL.Models;
using Cocorra.DAL.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cocorra.DAL.Repository.MessageRepository
{
    public class MessageRepository : GenericRepositoryAsync<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<List<Message>> GetChatHistoryAsync(Guid userId1, Guid userId2, int pageNumber, int pageSize)
        {
            var query = _dbContext.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1));

            var pagedMessages = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize) 
                .ToListAsync();

            return pagedMessages.OrderBy(m => m.CreatedAt).ToList();
        }
        public async Task<Message?> GetLastMessageAsync(Guid userId1, Guid userId2)
        {
            return await _dbContext.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderByDescending(m => m.CreatedAt) // بنرتب من الأحدث للأقدم
                .FirstOrDefaultAsync(); // بناخد أول واحدة بس
        }

        public async Task<int> GetUnreadCountAsync(Guid senderId, Guid receiverId)
        {
            // بنعد الرسايل اللي مبعوتالي أنا (receiverId) ومقرتهاش
            return await _dbContext.Messages
                .CountAsync(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead);
        }

        public async Task MarkMessagesAsReadAsync(Guid senderId, Guid receiverId)
        {
            // بنجيب كل الرسايل اللي مبعوتالي من الشخص ده ولسه False
            var unreadMessages = await _dbContext.Messages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true; // تحويل لـ Seen
                }
                _dbContext.Messages.UpdateRange(unreadMessages);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<List<ChatFriendDto>> GetFriendsChatSummariesAsync(Guid currentUserId, List<ApplicationUser> friends)
        {
            var friendIds = friends.Select(f => f.Id).ToList();

            var summaries = await _dbContext.Messages
                .Where(m => (m.SenderId == currentUserId && friendIds.Contains(m.ReceiverId)) ||
                            (m.ReceiverId == currentUserId && friendIds.Contains(m.SenderId)))
                .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    FriendId = g.Key,
                    LastMessage = g.OrderByDescending(m => m.CreatedAt).FirstOrDefault(),
                    UnreadCount = g.Count(m => m.ReceiverId == currentUserId && m.SenderId == g.Key && !m.IsRead)
                })
                .ToListAsync();

            return friends.Select(f => {
                var summary = summaries.FirstOrDefault(s => s.FriendId == f.Id);
                return new ChatFriendDto
                {
                    FriendId = f.Id,
                    FullName = $"{f.FirstName} {f.LastName}",
                    LastMessage = summary?.LastMessage?.Content,
                    LastMessageDate = summary?.LastMessage?.CreatedAt,
                    UnreadCount = summary?.UnreadCount ?? 0
                };
            }).OrderByDescending(d => d.LastMessageDate ?? DateTime.MinValue).ToList();
        }
    }
}
