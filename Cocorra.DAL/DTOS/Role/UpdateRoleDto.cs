using System;
using System.Collections.Generic;
using System.Text;

namespace Cocorra.DAL.DTOS;
    public class UpdateRoleDto
    {
        public string RoleId { get; set; } = default!;
        public string NewRoleName { get; set; } = default!;
    }