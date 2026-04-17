using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rased.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Rased_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1. عرض كل الإعلانات (للداتش بورد)
        [HttpGet]
        public async Task<IActionResult> GetAllAds()
        {
            var ads = await _context.Ads.OrderByDescending(a => a.CreatedAt).ToListAsync();

            // نجيب رابط السيرفر الحالي
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            // نركب الرابط الكامل لكل إعلان
            foreach (var ad in ads)
            {
                // نضمن إننا مش بنكرر السلاش لو المسار المتخزن بيبدأ بـ /
                var imagePath = ad.ImageUrl.StartsWith("/") ? ad.ImageUrl : "/" + ad.ImageUrl;
                ad.ImageUrl = baseUrl + imagePath;
            }

            return Ok(ads);
        }

        // 2. عرض الإعلانات النشطة (للفلاتر)
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveAds()
        {
            var ads = await _context.Ads.Where(a => a.IsActive).ToListAsync();
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            foreach (var ad in ads)
            {
                var imagePath = ad.ImageUrl.StartsWith("/") ? ad.ImageUrl : "/" + ad.ImageUrl;
                ad.ImageUrl = baseUrl + imagePath;
            }

            return Ok(ads);
        }

        // 3. رفع الإعلان
        [HttpPost("upload")]
        public async Task<IActionResult> UploadAd([FromForm] string title, IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("يرجى اختيار صورة");

            string webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            string uploadsFolder = Path.Combine(webRoot, "uploads", "ads");

            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var ad = new Ad
            {
                Id = Guid.NewGuid(),
                Title = title,
                ImageUrl = $"/uploads/ads/{fileName}", // بنخزن المسار النسبي
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Ads.Add(ad);
            await _context.SaveChangesAsync();

            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            ad.ImageUrl = baseUrl + ad.ImageUrl;

            return Ok(ad);
        }

        // 4. تغيير الحالة (Toggle) - زي ما هو
        [HttpPut("toggle/{id}")]
        public async Task<IActionResult> ToggleAdStatus(Guid id)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad == null) return NotFound("الإعلان غير موجود");

            ad.IsActive = !ad.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تغيير حالة الإعلان", status = ad.IsActive });
        }

        // 5. حذف الإعلان
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAd(Guid id)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad == null) return NotFound("الإعلان غير موجود");

            // هنا لازم نكون حذرين: ad.ImageUrl دلوقتي متخزن فيه المسار النسبي (بدون الدومين)
            // بس لازم نتأكد إننا بنجيب المسار الصح من الـ Database
            // ملحوظة: لو الـ Delete بيفشل، يفضل تعمل Query جديد تجيب الـ ImageUrl الأصلي قبل التعديل

            string webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            string relativePath = ad.ImageUrl.Replace($"{Request.Scheme}://{Request.Host}{Request.PathBase}", "");
            string filePath = Path.Combine(webRoot, relativePath.TrimStart('/'));

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();

            return Ok("تم حذف الإعلان وصورته نهائياً");
        }
    }
}