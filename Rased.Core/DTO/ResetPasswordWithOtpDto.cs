using System.ComponentModel.DataAnnotations;

namespace Rased.Core.DTO
{
    public class ResetPasswordWithOtpDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "OTP code is required.")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "OTP code length is invalid.")]
        public string Code { get; set; } = default!;

        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; } = default!;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
