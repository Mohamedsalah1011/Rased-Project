using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rased.Core.DTO.Complaint;
using Rased.Core.Entities;
using Rased.Core.ServiseContracts;
using Rased_Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Rased.Infrustracture.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly ApplicationDbContext _context;

        public ComplaintService(ApplicationDbContext context)
        {
            _context = context;
        }

        private static string GenerateSerialNumber()
        {
            return $"CMP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }

        public async Task<ActionResult<ComplaintResponseDto>> CreateAsync(CreateComplaintDto dto, ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return new UnauthorizedResult();

            var complaint = new Complaint
            {
                Id = Guid.NewGuid(),
                Description = dto.Description,
                Image = dto.Image,
                Video = dto.Video,
                Lng = dto.Lng,
                Lat = dto.Lat,
                Location = dto.Location,
                complaintStatus = "Pending",
                SerialNumber = GenerateSerialNumber(),
                UserId = userId
            };

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            return new OkObjectResult(await MapToDto(complaint));
        }

        public async Task<ActionResult<List<ComplaintResponseDto>>> GetMyComplaintsAsync(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return new UnauthorizedResult();

            return await GetByUserAsync(userId);
        }

        public async Task<ActionResult<ComplaintResponseDto>> GetByIdAsync(Guid id)
        {
            var complaint = await _context.Complaints
                .Include(c => c.User)
                    .ThenInclude(u => u.UserProfile)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
                return new NotFoundResult();

            return new OkObjectResult(await MapToDto(complaint));
        }

        public async Task<ActionResult<List<ComplaintResponseDto>>> GetByUserAsync(Guid userId)
        {
            var list = await _context.Complaints
                .Include(c => c.User)
                    .ThenInclude(u => u.UserProfile)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var dtos = new List<ComplaintResponseDto>();
            foreach (var c in list)
                dtos.Add(await MapToDto(c));

            return new OkObjectResult(dtos);
        }

        public async Task<ActionResult<List<ComplaintResponseDto>>> GetAllAsync(string? status = null)
        {
            var query = _context.Complaints
                .Include(c => c.User)
                    .ThenInclude(u => u.UserProfile)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(c => c.complaintStatus == status);

            var list = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
            var dtos = new List<ComplaintResponseDto>();
            foreach (var c in list)
                dtos.Add(await MapToDto(c));

            return new OkObjectResult(dtos);
        }

        public async Task<IActionResult> UpdateStatusAsync(Guid id, UpdateComplaintStatusDto dto)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
                return new NotFoundResult();

            complaint.complaintStatus = dto.Status;
            await _context.SaveChangesAsync();
            return new OkResult();
        }

        private static Task<ComplaintResponseDto> MapToDto(Complaint c)
        {
            var dto = new ComplaintResponseDto
            {
                Id = c.Id,
                Description = c.Description,
                Image = c.Image,
                Video = c.Video,
                Status = c.complaintStatus,
                Lng = c.Lng,
                Lat = c.Lat,
                Location = c.Location,
                AIGeneratedText = c.AIGeneratedText,
                SerialNumber = c.SerialNumber,
                CreatedAt = c.CreatedAt,
                UserId = c.UserId,
                UserFullName = c.User?.UserProfile?.FullName
            };
            return Task.FromResult(dto);
        }
    }
}
