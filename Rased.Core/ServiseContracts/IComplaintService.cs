using Rased.Core.DTO.Category;
using Rased.Core.DTO.Complaint;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Rased.Core.ServiseContracts
{
    public interface IComplaintService
    {
        // رجعنا الداتا الصافية بدون ActionResult
        Task<ComplaintResponseDto> CreateAsync(CreateComplaintDto dto, ClaimsPrincipal user);
        Task<ComplaintResponseDto> GetByIdAsync(Guid id);
        Task<List<ComplaintResponseDto>> GetMyComplaintsAsync(ClaimsPrincipal user);
        Task<List<ComplaintResponseDto>> GetByUserAsync(Guid userId);

        // جلب الكل مع دعم الـ status والـ search كـ داتا صافية
        Task<List<ComplaintResponseDto>> GetAllAsync(string? status = null, string? search = null);

        // تعديل الحالة يرجع bool (true لو نجح، false لو فشل) بدل IActionResult
        Task<bool> UpdateStatusAsync(Guid id, UpdateComplaintStatusDto dto);
        Task<object> GetStatsAsync();
    }
}