using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cocorra.BLL.DTOS.Auth
{
    public class RegisterDto
    {
        [Required, EmailAddress, MaxLength(100), MinLength(5), DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required, MinLength(6), MaxLength(100), DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required, MaxLength(100)]
        public string? FirstName { get; set; }
        [Required, MaxLength(100)]
        public string? LastName { get; set; }
        [Required]
        public IFormFile? VoiceVerification { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string? MBTI { get; set; }


    }
}