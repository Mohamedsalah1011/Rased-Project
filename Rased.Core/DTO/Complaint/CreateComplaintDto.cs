using System;
using System.ComponentModel.DataAnnotations;

namespace Rased.Core.DTO.Complaint
{
    public class CreateComplaintDto
    {
        [Required]
        public string Description { get; set; } = default!;

        public string? Image { get; set; }
        public string? Video { get; set; }
        public double? Lng { get; set; }
        public double? Lat { get; set; }
        public string? Location { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
    }
}
