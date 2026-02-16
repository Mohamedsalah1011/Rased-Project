using System.ComponentModel.DataAnnotations;

namespace Rased.Core.DTO
{
    public class VerifyOtpDto
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [RegularExpression("^[1-9]*$", ErrorMessage = "Phone number should be in digits only.")]
        public string PhoneNumber { get; set; } = default!;

        [Required(ErrorMessage = "OTP code is required.")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "OTP code length is invalid.")]
        public string Code { get; set; } = default!;
    }
}
