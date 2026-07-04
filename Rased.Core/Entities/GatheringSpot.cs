using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.Entities
{
    public class GatheringSpot
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
