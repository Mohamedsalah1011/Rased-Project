using System;
using System.ComponentModel.DataAnnotations;

namespace Rased.Core.DTO.Ads
{
    public class AdResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAdDto
    {
        [Required]
        public string Title { get; set; } = default!;

        [Required]
        public string ImageUrl { get; set; } = default!;

        public bool IsActive { get; set; } = true;
    }
}
