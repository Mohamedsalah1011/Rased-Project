using Rased.Core.DTO.Account.Otp.Enum;
using System.ComponentModel.DataAnnotations;

namespace Rased.Core.DTO.Account.Otp
{
    public class VerifyOtpDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "OTP code is required.")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "OTP code length is invalid.")]
        public string Code { get; set; } = default!;
        public OtpType Type { get; set; }
    }
}
