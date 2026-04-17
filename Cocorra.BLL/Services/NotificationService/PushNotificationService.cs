using System;
using System.Threading.Tasks;

using FirebaseAdmin.Messaging;
using System.Collections.Generic;

namespace Cocorra.BLL.Services.NotificationService
{
    public class PushNotificationService : IPushNotificationService
    {
        public async Task SendPushNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string> data)
        {
            if (string.IsNullOrWhiteSpace(fcmToken)) return;

            var message = new Message()
            {
                Token = fcmToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };

            try
            {
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch (FirebaseMessagingException)
            {
                // Optionally log inactive token or mapping issues. 
                // Swallow so caller doesn't crash on invalid/expired tokens.
            }
        }
    }
}