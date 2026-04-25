using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rased.Core.DTO.Category;
using Rased.Core.DTO.Complaint;
using Rased.Core.ServiseContracts;
using System;

namespace Rased_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintService _complaintService;

        public ComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ComplaintResponseDto>> Create([FromBody] CreateComplaintDto dto)
        {
            return await _complaintService.CreateAsync(dto, User);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ComplaintResponseDto>> GetById(Guid id)
        {
            return await _complaintService.GetByIdAsync(id);
        }

        [HttpGet("my-complaints")]
        public async Task<ActionResult<List<ComplaintResponseDto>>> GetMyComplaints()
        {
            return await _complaintService.GetMyComplaintsAsync(User);
        }

        [HttpGet]
        public async Task<ActionResult<List<ComplaintResponseDto>>> GetAll([FromQuery] string? status = null)
        {
            return await _complaintService.GetAllAsync(status);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateComplaintStatusDto dto)
        {
            return await _complaintService.UpdateStatusAsync(id, dto);
        }

        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            return await _complaintService.GetCategoriesAsync();
        }
    }
}
