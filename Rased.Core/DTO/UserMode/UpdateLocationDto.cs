using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.DTO.UserMode
{
    public class UpdateLocationDto
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool IsSharingLive { get; set; }
        public string UserType { get; set; } = "Passenger"; // Passenger OR Driver
    }
}
