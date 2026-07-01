using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rased.Core.DTO.Category;
using Rased.Core.DTO.Complaint;
using Rased.Core.ServiseContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rased_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintService _complaintService;

        public ComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        // 1. إنشاء بلاغ جديد
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateComplaintDto dto)
        {
            var result = await _complaintService.CreateAsync(dto, User);
            if (result == null) return Unauthorized();
            return Ok(result);
        }

        // 2. جلب بلاغ محدد بواسطة الـ ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var complaint = await _complaintService.GetByIdAsync(id);
            if (complaint == null) return NotFound();
            return Ok(complaint);
        }

        // 3. جلب بلاغات المستخدم الحالي (خاص بالموبايل)
        [HttpGet("my-complaints")]
        public async Task<IActionResult> GetMyComplaints()
        {
            var complaints = await _complaintService.GetMyComplaintsAsync(User);
            return Ok(complaints);
        }

        // 4. جلب جميع البلاغات مع دعم الفلترة والبحث للـ Dashboard
        // 4. جلب جميع البلاغات مع دعم الفلترة والبحث للـ Dashboard
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status = null, [FromQuery] string? search = null)
        {
            List<ComplaintResponseDto> complaints = await _complaintService.GetAllAsync(status);

            if (complaints == null)
            {
                complaints = new List<ComplaintResponseDto>();
            }

            // تطبيق السيرش: دعم البحث بالسيريال، أو الوصف، أو نمرة العربية (AiGeneratedText)
            if (!string.IsNullOrEmpty(search))
            {
                complaints = complaints
                    .Where(c => (!string.IsNullOrEmpty(c.SerialNumber) && c.SerialNumber.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                (!string.IsNullOrEmpty(c.Description) && c.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                (!string.IsNullOrEmpty(c.AIGeneratedText) && c.AIGeneratedText.Contains(search, StringComparison.OrdinalIgnoreCase))) // 👈 السيرش بالنمرة هنا
                    .ToList();
            }

            return Ok(complaints);
        }

        // 5. تحديث حالة الشكوى (حل / رفض) من لوحة التحكم
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateComplaintStatusDto dto)
        {
            var success = await _complaintService.UpdateStatusAsync(id, dto);
            if (!success) return NotFound();
            return Ok(new { Message = "Status updated successfully" });
        }

        // 6. جلب إحصائيات البلاغات للـ Dashboard
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _complaintService.GetStatsAsync();
            return Ok(stats);
        }
    }
}