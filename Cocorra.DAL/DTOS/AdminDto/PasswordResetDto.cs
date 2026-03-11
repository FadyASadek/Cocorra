using System;
using System.Collections.Generic;
using System.Text;

namespace Cocorra.DAL.DTOS.AdminDto
{
    public class PasswordResetDto
    {
        public string NewPassword { get; set; } = default!;
    }
}
