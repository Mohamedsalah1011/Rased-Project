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

        public async Task<ComplaintResponseDto> CreateAsync(CreateComplaintDto dto, ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId)) return null;

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
                Status = "0", // جعل الحقلين متزامنين عند الإنشاء أيضاً
                SerialNumber = GenerateSerialNumber(),
                UserId = userId
            };
            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();
            return await MapToDto(complaint);
        }

        public async Task<List<ComplaintResponseDto>> GetMyComplaintsAsync(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId)) return new List<ComplaintResponseDto>();
            return await GetByUserAsync(userId);
        }

        public async Task<ComplaintResponseDto> GetByIdAsync(Guid id)
        {
            var complaint = await _context.Complaints.Include(c => c.User).ThenInclude(u => u.UserProfile).FirstOrDefaultAsync(c => c.Id == id);
            return complaint == null ? null : await MapToDto(complaint);
        }

        public async Task<List<ComplaintResponseDto>> GetByUserAsync(Guid userId)
        {
            var list = await _context.Complaints.Include(c => c.User).ThenInclude(u => u.UserProfile).Where(c => c.UserId == userId).OrderByDescending(c => c.CreatedAt).ToListAsync();
            var dtos = new List<ComplaintResponseDto>();
            foreach (var c in list) dtos.Add(await MapToDto(c));
            return dtos;
        }

        // --- ميثود جلب البيانات المعدلة لمنع تسريب البلاغات وتجنب التضارب ---
        public async Task<List<ComplaintResponseDto>> GetAllAsync(string? status = null, string? search = null)
        {
            var query = _context.Complaints.Include(c => c.User).ThenInclude(u => u.UserProfile).AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All" && status != "الكل")
            {
                if (status == "Pending")
                {
                    // جلب الشكاوى التي حالتها الحالية Pending أو قيد الانتظار، ولا نلجأ لحقل Status إلا لو كان الحقل الأساسي فارغاً
                    query = query.Where(c => c.complaintStatus == "Pending" || c.complaintStatus == "0" || c.complaintStatus == "قيد الانتظار" ||
                                            ((c.complaintStatus == null || c.complaintStatus == "") && c.Status == "0"));
                }
                else if (status == "Resolved")
                {
                    query = query.Where(c => c.complaintStatus == "Resolved" || c.complaintStatus == "1" || c.complaintStatus == "تم الحل" ||
                                            ((c.complaintStatus == null || c.complaintStatus == "") && c.Status == "1"));
                }
                else if (status == "Rejected")
                {
                    query = query.Where(c => c.complaintStatus == "Rejected" || c.complaintStatus == "2" || c.complaintStatus == "مرفوض" ||
                                            ((c.complaintStatus == null || c.complaintStatus == "") && c.Status == "2"));
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.SerialNumber.Contains(search) ||
                                     c.Description.Contains(search) ||
                                     (c.AIGeneratedText != null && c.AIGeneratedText.Contains(search)) ||
                                     (c.User != null && c.User.UserProfile != null && c.User.UserProfile.FullName.Contains(search)));
            }

            var list = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
            var dtos = new List<ComplaintResponseDto>();
            foreach (var c in list) dtos.Add(await MapToDto(c));
            return dtos;
        }

        // --- ميثود تحديث الحالة المعدلة لمزامنة الحقلين معاً في قاعدة البيانات ---
        public async Task<bool> UpdateStatusAsync(Guid id, UpdateComplaintStatusDto dto)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null) return false;

            // 1. تحديث الحقل الأساسي الذي تقرأ منه لوحة التحكم والـ View
            complaint.complaintStatus = dto.Status;

            // 2. تحديث الحقل الثاني لضمان المزامنة التامة ومنع تسريب البيانات عند الفلترة
            if (dto.Status == "Pending") complaint.Status = "0";
            else if (dto.Status == "Resolved") complaint.Status = "1";
            else if (dto.Status == "Rejected") complaint.Status = "2";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<object> GetStatsAsync()
        {
            var total = await _context.Complaints.CountAsync();
            var pending = await _context.Complaints.CountAsync(c => c.complaintStatus == "Pending" || c.complaintStatus == "0" || c.complaintStatus == "قيد الانتظار");
            var resolved = await _context.Complaints.CountAsync(c => c.complaintStatus == "Resolved" || c.complaintStatus == "1" || c.complaintStatus == "تم الحل");
            var rejected = await _context.Complaints.CountAsync(c => c.complaintStatus == "Rejected" || c.complaintStatus == "2" || c.complaintStatus == "مرفوض");

            var today = DateTime.UtcNow.Date;
            var todayComplaints = await _context.Complaints.CountAsync(c => c.CreatedAt >= today);

            var totalUsers = await _context.Users.CountAsync();
            var driverRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Driver" || r.Name == "سائق");
            var totalDrivers = driverRole != null ? await _context.UserRoles.CountAsync(ur => ur.RoleId == driverRole.Id) : 0;

            return new { totalComplaints = total, todayComplaints = todayComplaints, pending = pending, resolved = resolved, rejected = rejected, totalUsers = totalUsers, totalDrivers = totalDrivers };
        }

        private static Task<ComplaintResponseDto> MapToDto(Complaint c)
        {
            return Task.FromResult(new ComplaintResponseDto
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
            });
        }
    }
}