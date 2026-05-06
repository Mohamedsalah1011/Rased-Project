using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Rased.Core.Enums;

namespace Rased.Core.DTO.Account
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Remote(action: "IsEmailAvailable", controller: "Account", ErrorMessage = "Email is already in use")]
        public string Email { get; set; } = default!;

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
        [RegularExpression(@"^[23][0-9]{13}$", ErrorMessage = "Invalid Egyptian SSN")]
        public string SSN { get; set; } = default!;

        public UserType UserType { get; set; } = UserType.User;

        public string? PlateNumber { get; set; }
    }
}
