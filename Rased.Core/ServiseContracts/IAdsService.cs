using Microsoft.AspNetCore.Mvc;
using Rased.Core.DTO.Ads;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rased.Core.ServiseContracts
{
    public interface IAdsService
    {
        Task<ActionResult<List<AdResponseDto>>> GetAllAdsAsync(bool? activeOnly = null);
        Task<ActionResult<AdResponseDto>> GetAdByIdAsync(Guid id);
        Task<ActionResult<AdResponseDto>> CreateAdAsync(CreateAdDto dto);
        Task<IActionResult> DeleteAdAsync(Guid id);
        Task<IActionResult> ToggleAdStatusAsync(Guid id);
    }
}
