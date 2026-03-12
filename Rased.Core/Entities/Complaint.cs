using Rased.Core.Identity;
using System;

namespace Rased.Core.Entities
{
    public class Complaint
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = default!;
        public string? Image { get; set; }
        public string? Video { get; set; }
        public string Status { get; set; } = "Pending"; // e.g. Pending, InProgress, Resolved
        public double? Lng { get; set; }
        public double? Lat { get; set; }
        public string? Location { get; set; }
        public string? AIGeneratedText { get; set; }
        public string SerialNumber { get; set; } = default!;

        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = default!;

        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
