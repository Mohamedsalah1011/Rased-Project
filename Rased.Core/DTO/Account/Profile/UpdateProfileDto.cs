using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.DTO.Account.Profile
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
    }
}
