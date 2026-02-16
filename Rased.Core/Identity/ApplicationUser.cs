using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public String FullName { get; set; } = default!;
        public int SSN { get; set; } = default!;

    }
}
