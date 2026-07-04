using Rased.Core.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.Entities
{
    public class UserLiveLocation
    {
        public Guid Id { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool IsSharingLive { get; set; } = false;

        // "Passenger" أو "Driver" عشان نحدد نوع المستخدم
        public string UserType { get; set; } = "Passenger";

        // مهم جداً عشان نعرف الراكب ده لسه نشط ولا قفل الأبليكيشن من فترة
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ربط الموقع بالمستخدم اللي مسجل دخول
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = default!;
    }
}
