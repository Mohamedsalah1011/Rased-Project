using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.DTO.UserMode
{
    public class HotspotDto
    {
        public Guid SpotId { get; set; }
        public string SpotName { get; set; } = default!;
        public double Lat { get; set; }
        public double Lng { get; set; }
        public int PassengerCount { get; set; } // عدد الركاب المنتظرين هنا حالياً
    }
}
