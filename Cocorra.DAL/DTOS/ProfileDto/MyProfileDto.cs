using System;

namespace Cocorra.DAL.DTOS.ProfileDto
{
    public class MyProfileDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfilePicturePath { get; set; }
        public string? Bio { get; set; }
        public int Age { get; set; }
        public string? MBTI { get; set; }
        // الـ Voice Verification Path مثلاً مش بنرجعه عشان دي حاجة في الباك إند بس
    }
}