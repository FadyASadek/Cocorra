using System;
using System.Collections.Generic;
using System.Text;

namespace Cocorra.DAL.DTOS.AdminDto
{
    public class UserDto
    {
        public string Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int Age { get; set; }
        public string MBTI { get; set; } = default!;
        public string Status { get; set; } = default!; // Active, Pending, Banned
        public DateTime CreatedAt { get; set; } // لو عندك في الداتابيز، لو مفيش شيلها
        public string? VoicePath { get; set; } // عشان لو عايز تسمع صوته
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
