using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.DTO.Account.Profile
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
    }
}
