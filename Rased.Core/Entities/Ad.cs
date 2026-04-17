using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.Entities
{
    public class Ad
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!; // اسم الإعلان (عشان الأدمن)
        public string ImageUrl { get; set; } = default!; // لينك الصورة اللي هيقرأه الموبايل
        public bool IsActive { get; set; } = true; // شغال ولا واقف
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
