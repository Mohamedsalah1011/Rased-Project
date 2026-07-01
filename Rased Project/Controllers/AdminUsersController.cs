using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rased_Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rased_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AdminUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // جلب جميع المستخدمين مع الفلترة والبحث
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? type = null, [FromQuery] string? search = null)
        {
            // 1. جلب المستخدمين مع البروفايل والأدوار الخاصة بهم من جداول الـ Identity
            var usersList = await _context.Users
                .Include(u => u.UserProfile)
                .ToListAsync();

            var resultList = new List<object>();

            foreach (var u in usersList)
            {
                // جلب الـ RoleId الخاص بالمسخدم الحالي من جدول الربط AspNetUserRoles
                var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == u.Id);
                string roleName = "راكب"; // افتراضي

                if (userRole != null)
                {
                    // جلب اسم الـ Role الفعلي من جدول AspNetRoles
                    var role = await _context.Roles.FindAsync(userRole.RoleId);
                    if (role != null)
                    {
                        if (role.Name == "Driver" || role.Name == "سائق")
                        {
                            roleName = "سائق";
                        }
                        else if (role.Name == "Passenger" || role.Name == "راكب")
                        {
                            roleName = "راكب";
                        }
                    }
                }

                // تجهيز الأوبجكت المبدئي للمستخدم مع إضافة الحقول الجديدة 🎯
                var mappedUser = new
                {
                    u.Id,
                    FullName = u.UserProfile?.FullName ?? "غير محدد",
                    u.Email,
                    PhoneNumber = u.PhoneNumber ?? "غير محدد",
                    Role = roleName,
                    IsActive = u.IsActive,

                    // 👇 تم إضافة السطرين دول هنا عشان يقروا من جدول الـ UserProfile ويطلعوا في سواجير
                    SSN = u.UserProfile?.SSN ?? "غير مسجل",
                    PlateNumber = u.UserProfile?.PlateNumber ?? "غير مسجل"
                };

                resultList.Add(mappedUser);
            }

            // 2. تطبيق الفلترة بالـ Role (سائقين / ركاب) بعد جلب الداتا بناء على الأدوار
            if (!string.IsNullOrWhiteSpace(type) && type != "الكل")
            {
                if (type == "سائقين")
                    resultList = resultList.Where(u => (string)((dynamic)u).Role == "سائق").ToList();
                else if (type == "ركاب")
                    resultList = resultList.Where(u => (string)((dynamic)u).Role == "راكب").ToList();
            }

            // 3. تطبيق السيرش بالاسم
            if (!string.IsNullOrWhiteSpace(search))
            {
                resultList = resultList.Where(u => ((string)((dynamic)u).FullName).Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return Ok(resultList);
        }

        [HttpPatch("{id:guid}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { isActive = user.IsActive });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}