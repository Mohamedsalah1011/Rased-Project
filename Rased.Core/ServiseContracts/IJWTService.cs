using Rased.Core.DTO;
using Rased.Core.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.ServiseContracts
{
    public interface IJWTService
    {
        AuthenticationResponse GenerateToken(ApplicationUser user);
    }
}
