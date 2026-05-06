using Rased.Core.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rased.Core.Entities
{
    public class UserProfile
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = default!;

        [Required]
        [StringLength(14)]
        public string SSN { get; set; } = default!;

        public string? PlateNumber { get; set; }

        public string? ProfilePictureUrl { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = default!;
    }
}
