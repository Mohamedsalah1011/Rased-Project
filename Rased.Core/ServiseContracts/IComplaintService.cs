using Microsoft.AspNetCore.Mvc;
using Rased.Core.DTO.Complaint;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Rased.Core.ServiseContracts
{
    public interface IComplaintService
    {
        // 1. إنشاء بلاغ (للموبايل)
        Task<ActionResult<ComplaintResponseDto>> CreateAsync(CreateComplaintDto dto, ClaimsPrincipal user);

        // 2. جلب بلاغ بالـ ID (للتفاصيل)
        Task<ActionResult<ComplaintResponseDto>> GetByIdAsync(Guid id);

        // 3. بلاغات المستخدم الحالي (للموبايل)
        Task<ActionResult<List<ComplaintResponseDto>>> GetMyComplaintsAsync(ClaimsPrincipal user);

        // 4. بلاغات مستخدم محدد بالـ ID
        Task<ActionResult<List<ComplaintResponseDto>>> GetByUserAsync(Guid userId);

        // 5. جلب كل البلاغات مع الفلترة والبحث (للداش بورد)
        // بنستخدم البحث برقم اللوحة (AIGeneratedText) أو الرقم المسلسل (SerialNumber)
        Task<ActionResult<List<ComplaintResponseDto>>> GetAllAsync(string? status = null, string? search = null);

        // 6. إحصائيات الداش بورد (للكروت والرسم البياني)
        // هترجع object فيه (Total, Pending, Resolved, مرفوض, Today)
        Task<ActionResult<object>> GetDashboardStatsAsync();

        // 7. تحديث حالة البلاغ (مقبول / مرفوض / تم الحل)
        Task<IActionResult> UpdateStatusAsync(Guid id, UpdateComplaintStatusDto dto);
    }
}