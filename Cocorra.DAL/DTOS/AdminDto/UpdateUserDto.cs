using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cocorra.DAL.DTOS.AdminDto
{
    public class UpdateUserDto
    {
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
        [EmailAddress]
        public string Email { get; set; } = default!; // الأدمن يقدر يصحح الإيميل لو مكتوب غلط
        public int Age { get; set; }
        public string MBTI { get; set; } = default!;
    }
}
