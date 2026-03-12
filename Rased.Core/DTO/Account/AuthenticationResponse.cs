using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.DTO.Account
{
    public class AuthenticationResponse
    {
        public string PersonName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Token { get; set; } = default!;
        public DateTime Expiration { get; set; }



    }
}
