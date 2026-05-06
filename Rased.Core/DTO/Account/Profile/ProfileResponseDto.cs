using System;

namespace Rased.Core.DTO.Account.Profile
{
    public class ProfileResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string SSN { get; set; } = default!;
        public string? ProfilePictureUrl { get; set; }
    }
}
