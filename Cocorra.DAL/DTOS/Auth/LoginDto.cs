using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cocorra.BLL.DTOS.Auth
{
    public class LoginDto
    {
        [Required, EmailAddress, MaxLength(100), MinLength(5), DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required, MinLength(6), MaxLength(100), DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}