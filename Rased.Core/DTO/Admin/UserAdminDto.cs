using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.DTO.Admin
{
    public class UserAdminDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int SSN { get; set; }
        public bool IsActive { get; set; }

    }
}
