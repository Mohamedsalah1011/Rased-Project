using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Rased.Core.DTO.Complaint
{
    public class CreateComplaintDto
    {
        [Required(ErrorMessage = "وصف الشكوى مطلوب")]
        public string Description { get; set; } = default!;

        // ده لاستقبال الملف من الـ Request (زرار الـ Upload)
        public IFormFile? Image { get; set; }

        // السطر ده هو اللي هيحل لك إيرور الـ Controller
        // عشان نخزن فيه اسم الصورة اللي اتسيف في الـ Server
        public string? ImagePath { get; set; }

        public IFormFile? Video { get; set; }

        // لو حابب تسيف مسار الفيديو برضه ضيف دي:
        public string? VideoPath { get; set; }

        public double? Lng { get; set; }
        public double? Lat { get; set; }
        public string? Location { get; set; }
    }
}