using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rased.Core.Entities; // تأكد من النيم سبيس الخاص بـ Ad
using Rased_Project; // اسم الداتا بيز كونتكست

namespace Rased_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public AdsController(ApplicationDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Ads.ToListAsync());

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Ad adModel)
        {
            _db.Ads.Add(adModel);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "تمت الإضافة بنجاح!" });
        }

        // الميثود دي هي اللي كانت ناقصة وبتسبب الـ 404
        [HttpPost("{id}/toggle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return NotFound();
            ad.IsActive = !ad.IsActive; // عكس الحالة
            await _db.SaveChangesAsync();
            return Ok(new { IsActive = ad.IsActive });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return NotFound();
            _db.Ads.Remove(ad);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}