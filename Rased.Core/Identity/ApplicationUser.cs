using Microsoft.AspNetCore.Identity;
using System;
using Rased.Core.Entities;

namespace Rased.Core.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public bool IsActive { get; set; } = true;

        public virtual UserProfile? UserProfile { get; set; }
    }
}
