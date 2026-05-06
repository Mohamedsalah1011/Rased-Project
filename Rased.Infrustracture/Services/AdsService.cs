using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rased.Core.DTO.Ads;
using Rased.Core.Entities;
using Rased.Core.ServiseContracts;
using Rased_Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rased.Infrustracture.Services
{
    public class AdsService : IAdsService
    {
        private readonly ApplicationDbContext _db;

        public AdsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ActionResult<List<AdResponseDto>>> GetAllAdsAsync(bool? activeOnly = null)
        {
            var query = _db.Ads.AsQueryable();

            if (activeOnly.HasValue && activeOnly.Value)
            {
                query = query.Where(a => a.IsActive);
            }

            var ads = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();

            var response = ads.Select(a => new AdResponseDto
            {
                Id = a.Id,
                Title = a.Title,
                ImageUrl = a.ImageUrl,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt
            }).ToList();

            return new OkObjectResult(response);
        }

        public async Task<ActionResult<AdResponseDto>> GetAdByIdAsync(Guid id)
        {
            var ad = await _db.Ads.FindAsync(id);

            if (ad == null)
                return new NotFoundResult();

            return new OkObjectResult(new AdResponseDto
            {
                Id = ad.Id,
                Title = ad.Title,
                ImageUrl = ad.ImageUrl,
                IsActive = ad.IsActive,
                CreatedAt = ad.CreatedAt
            });
        }

        public async Task<ActionResult<AdResponseDto>> CreateAdAsync(CreateAdDto dto)
        {
            var ad = new Ad
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                ImageUrl = dto.ImageUrl,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _db.Ads.Add(ad);
            await _db.SaveChangesAsync();

            return new OkObjectResult(new AdResponseDto
            {
                Id = ad.Id,
                Title = ad.Title,
                ImageUrl = ad.ImageUrl,
                IsActive = ad.IsActive,
                CreatedAt = ad.CreatedAt
            });
        }

        public async Task<IActionResult> DeleteAdAsync(Guid id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null)
                return new NotFoundResult();

            _db.Ads.Remove(ad);
            await _db.SaveChangesAsync();

            return new OkResult();
        }

        public async Task<IActionResult> ToggleAdStatusAsync(Guid id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null)
                return new NotFoundResult();

            ad.IsActive = !ad.IsActive;
            await _db.SaveChangesAsync();

            return new OkResult();
        }
    }
}
