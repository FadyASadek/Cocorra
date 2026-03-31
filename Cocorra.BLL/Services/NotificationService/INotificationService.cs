using Cocorra.DAL.DTOS.NotificationDto;
using Cocorra.BLL.Base;


namespace Cocorra.BLL.Services.NotificationService
{
    public interface INotificationService
    {
        Task<Response<IEnumerable<NotificationResponseDto>>> GetMyNotificationsAsync(Guid userId);
        Task<Response<string>> MarkNotificationAsReadAsync(Guid notificationId, Guid userId);
    }
}