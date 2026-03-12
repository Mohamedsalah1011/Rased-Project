using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rased.Core.DTO.Account
{
    public class LoginDTo
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Remote(action: "IsEmailAvailable", controller: "Account", ErrorMessage = "Email is already in use")]
        public string Email { get; set; } = default!;
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = default!;
    }
}
