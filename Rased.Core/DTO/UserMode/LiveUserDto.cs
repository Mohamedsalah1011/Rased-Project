using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.DTO.UserMode
{
    public class LiveUserDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = default!;
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
