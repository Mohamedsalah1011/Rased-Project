using System.ComponentModel.DataAnnotations;

namespace Rased.Core.DTO.Complaint
{
    public class UpdateComplaintStatusDto
    {
        [Required]
        public string Status { get; set; } = default!; // e.g. Pending, InProgress, Resolved
    }
}
