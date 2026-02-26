using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rased.Core.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number Should be in digits only")]
        [Remote(action: "IsPhoneNumberAvailable", controller: "Account", ErrorMessage = "Phone number is already in use")]
        public string PhoneNumber { get; set; } = default!;
        [Required(ErrorMessage = "password is required")]
        public string Password { get; set; } = default!;
        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = default!;
        [Required(ErrorMessage = "SSN is required")]
        public int SSN { get; set; } = default!;
    }
}
