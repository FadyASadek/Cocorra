using System;
using System.Collections.Generic;
using System.Text;

namespace Cocorra.DAL.DTOS.Role
{
    public class ManageUserRolesDto
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; } = new List<string>(); 
    }
}
