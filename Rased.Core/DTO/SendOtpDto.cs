using System.ComponentModel.DataAnnotations;

namespace Rased.Core.DTO
{
    public class SendOtpDto
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [RegularExpression("^[1-9]*$", ErrorMessage = "Phone number should be in digits only.")]
        public string PhoneNumber { get; set; } = default!;
    }
}
