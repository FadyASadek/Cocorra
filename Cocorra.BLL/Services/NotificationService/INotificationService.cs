using System;
using System.Collections.Generic;
using System.Text;

namespace Cocorra.BLL.Services.NotificationService
{
    public interface IPushNotificationService
    {
        Task SendPushNotificationAsync(Guid receiverId, string title, string body, string? chatFriendId = null);
    }
}
