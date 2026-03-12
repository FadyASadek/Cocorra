using System;
using Cocorra.DAL.Enums;

namespace Cocorra.DAL.DTOS.ProfileDto
{
    public class PublicProfileDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePicturePath { get; set; }
        public string? Bio { get; set; }
        public string? MBTI { get; set; }
        public FriendRequestStatus? FriendshipStatus { get; set; }
        public bool IsFriend { get; set; }
    }
}