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
    [Authorize] 
    public class RideMatchingController : ControllerBase
    {
        private readonly RideMatchingService _rideMatchingService;

        public RideMatchingController(RideMatchingService rideMatchingService)
        {
            _rideMatchingService = rideMatchingService;
        }

        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("المستخدم غير معروف");

            var userId = Guid.Parse(userIdClaim);
            await _rideMatchingService.UpdateUserLocationAsync(userId, dto);

            return Ok(new { message = "تم تحديث الموقع بنجاح" });
        }

        [HttpGet("active-passengers")]
        public async Task<IActionResult> GetActivePassengers([FromQuery] double lat, [FromQuery] double lng)
        {
            var passengers = await _rideMatchingService.GetActivePassengersAsync(lat, lng, radiusInKm: 5000.0);
            return Ok(passengers);
        }

        [HttpGet("hotspots")]
        public async Task<IActionResult> GetHotspots()
        {
            var hotspots = await _rideMatchingService.GetHotspotsAsync(radiusInMeters: 50000);
            return Ok(hotspots);
        }
    }
}