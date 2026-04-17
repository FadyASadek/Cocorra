using System;
using System.Collections.Generic;
using System.Text;

namespace Cocorra.BLL.Services.NotificationService
{
    public interface IPushNotificationService
    {
        Task SendPushNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string> data);
    }
}
