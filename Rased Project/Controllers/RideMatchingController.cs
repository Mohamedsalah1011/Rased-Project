using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rased.Core.DTO.UserMode;
using Rased.Infrustracture.Services;
using System.Security.Claims;

namespace Rased_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // عشان نضمن إن اللي بينادي يكون عامل Login ومعاه الـ Token
    public class RideMatchingController : ControllerBase
    {
        private readonly RideMatchingService _rideMatchingService;

        public RideMatchingController(RideMatchingService rideMatchingService)
        {
            _rideMatchingService = rideMatchingService;
        }

        // 1. تحديث الموقع الحالي للمستخدم (راكب أو سواق)
        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto dto)
        {
            // استخراج الـ UserId من الـ JWT Token اللي مبعوت في الـ Header تلقائياً
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("المستخدم غير معروف");

            var userId = Guid.Parse(userIdClaim);
            await _rideMatchingService.UpdateUserLocationAsync(userId, dto);

            return Ok(new { message = "تم تحديث الموقع بنجاح" });
        }

        // 2. للسواق: هات الركاب القريبين مني (محتاج يبعت الـ Lat والـ Lng بتوعه)
        [HttpGet("active-passengers")]
        public async Task<IActionResult> GetActivePassengers([FromQuery] double lat, [FromQuery] double lng)
        {
            var passengers = await _rideMatchingService.GetActivePassengersAsync(lat, lng, radiusInKm: 5.0);
            return Ok(passengers);
        }

        // 3. للكل: هات الميادين (Hotspots) وعدد الناس اللي منتظرة فيها
        [HttpGet("hotspots")]
        public async Task<IActionResult> GetHotspots()
        {
            var hotspots = await _rideMatchingService.GetHotspotsAsync(radiusInMeters: 500);
            return Ok(hotspots);
        }
    }
}
