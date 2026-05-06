using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rased.Core.DTO.Ads;
using Rased.Core.ServiseContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rased_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly IAdsService _adsService;

        public AdsController(IAdsService adsService)
        {
            _adsService = adsService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<AdResponseDto>>> GetAll([FromQuery] bool? activeOnly = null)
        {
            // If the user is not an admin, they can only see active ads
            if (!User.IsInRole("Admin"))
            {
                activeOnly = true;
            }
            return await _adsService.GetAllAdsAsync(activeOnly);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<AdResponseDto>> GetById(Guid id)
        {
            return await _adsService.GetAdByIdAsync(id);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // Only Admins can create Ads
        public async Task<ActionResult<AdResponseDto>> Create([FromBody] CreateAdDto dto)
        {
            return await _adsService.CreateAdAsync(dto);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await _adsService.DeleteAdAsync(id);
        }

        [HttpPatch("{id:guid}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            return await _adsService.ToggleAdStatusAsync(id);
        }
    }
}
