using Microsoft.AspNetCore.Mvc;
using Rased.Core.DTO.Complaint;
using Rased.Core.DTO.Category;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Rased.Core.ServiseContracts
{
    public interface IComplaintService
    {
        Task<ActionResult<ComplaintResponseDto>> CreateAsync(CreateComplaintDto dto, ClaimsPrincipal user);
        Task<ActionResult<ComplaintResponseDto>> GetByIdAsync(Guid id);
        Task<ActionResult<List<ComplaintResponseDto>>> GetMyComplaintsAsync(ClaimsPrincipal user);
        Task<ActionResult<List<ComplaintResponseDto>>> GetByUserAsync(Guid userId);
        Task<ActionResult<List<ComplaintResponseDto>>> GetAllAsync(string? status = null);
        Task<IActionResult> UpdateStatusAsync(Guid id, UpdateComplaintStatusDto dto);
        Task<ActionResult<List<CategoryDto>>> GetCategoriesAsync();
    }
}
