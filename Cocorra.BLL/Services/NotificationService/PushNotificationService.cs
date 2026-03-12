using Cocorra.DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cocorra.BLL.Services.NotificationService
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PushNotificationService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task SendPushNotificationAsync(Guid receiverId, string title, string body, string? chatFriendId = null)
        {
            var user = await _userManager.FindByIdAsync(receiverId.ToString());

            if (user == null || string.IsNullOrEmpty(user.FcmToken))
            {
                return;
            }

            /*
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Token = user.FcmToken,
                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>()
                {
                    { "type", "chat" },
                    { "senderId", chatFriendId } 
                }
            };
            await FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.SendAsync(message);
            */

            await Task.CompletedTask;
        }
    }
}