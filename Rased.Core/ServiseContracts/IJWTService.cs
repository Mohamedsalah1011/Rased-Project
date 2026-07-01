using Rased.Core.DTO.Account;
using Rased.Core.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rased.Core.ServiseContracts
{
    public interface IJWTService
    {
        // التعديل هنا: أضفنا باراميتر الـ roles ليتطابق مع الـ Implementation
        AuthenticationResponse GenerateToken(ApplicationUser user, string? fullName = null, List<string>? roles = null);
    }
}