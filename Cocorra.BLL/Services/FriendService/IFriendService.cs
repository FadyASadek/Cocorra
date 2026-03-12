using Cocorra.DAL.DTOS.FriendDto;
using Cocorra.DAL.DTOS.NotificationDto;
using Core.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cocorra.BLL.Services.FriendService;

public interface IFriendService
{
    Task<Response<UserSearchDto>> SearchUserByIdAsync(Guid currentUserId, Guid targetUserId);
    Task<Response<string>> SendFriendRequestAsync(Guid currentUserId, Guid targetUserId);
    Task<Response<string>> RespondToFriendRequestAsync(Guid currentUserId, Guid senderId, bool accept);
    Task<Response<string>> RemoveFriendOrCancelRequestAsync(Guid currentUserId, Guid targetUserId);
}