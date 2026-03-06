using System.ComponentModel.DataAnnotations;

namespace Rased.Core.DTO
{
    public class SendOtpDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = default!;
    }
}
