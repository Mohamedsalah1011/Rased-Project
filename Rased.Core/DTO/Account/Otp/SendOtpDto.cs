using Rased.Core.DTO.Account.Otp.Enum;
using System.ComponentModel.DataAnnotations;

namespace Rased.Core.DTO.Account.Otp
{
    public class SendOtpDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = default!;
        public OtpType Type { get; set; }
    }
}
