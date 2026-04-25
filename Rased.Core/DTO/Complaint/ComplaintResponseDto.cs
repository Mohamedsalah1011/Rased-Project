using System;

namespace Rased.Core.DTO.Complaint
{
    public class ComplaintResponseDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = default!;
        public string? Image { get; set; }
        public string? Video { get; set; }
        public string Status { get; set; } = default!;
        public double? Lng { get; set; }
        public double? Lat { get; set; }
        public string? Location { get; set; }
        public string? AIGeneratedText { get; set; }
        public string SerialNumber { get; set; } = default!;
        public DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; }
        public string? UserFullName { get; set; }
    }
}
