using Cocorra.DAL.DTOS.NotificationDto;
using Cocorra.DAL.Repository.NotificationRepository;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cocorra.BLL.Services.NotificationService
{
    public class NotificationService : ResponseHandler, INotificationService
    {
        private readonly INotificationRepository _notificationRepo;

        public NotificationService(INotificationRepository notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public async Task<Response<IEnumerable<NotificationResponseDto>>> GetMyNotificationsAsync(Guid userId)
        {
            var userNotifications = await _notificationRepo.GetTableNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type.ToString(),
                    ReferenceId = n.ReferenceId,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return Success<IEnumerable<NotificationResponseDto>>(userNotifications);
        }

        public async Task<Response<string>> MarkNotificationAsReadAsync(Guid notificationId, Guid userId)
        {
            var notification = await _notificationRepo.GetByIdAsync(notificationId);

            if (notification == null || notification.UserId != userId)
                return NotFound<string>("Notification not found.");

            notification.IsRead = true;
            await _notificationRepo.UpdateAsync(notification);

            return Success("Notification marked as read.");
        }
    }
}