using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rased.Core.DTO
{
    public class LoginDTo
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [RegularExpression("^[1-9]*$", ErrorMessage = "Phone number should be in digits only.")]
        public string PhoneNumber { get; set; } = default!;
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = default!;
    }
}
