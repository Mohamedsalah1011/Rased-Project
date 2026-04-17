using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rased.Core.DTO.Admin;
using Rased_Project;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rased_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

     
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAdminDto>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserAdminDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "لا يوجد",
                    PhoneNumber = u.PhoneNumber ?? "لا يوجد",
                    SSN = u.SSN,
                    IsActive = u.IsActive
                   
                }).ToListAsync();

            return Ok(users);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAdminDto>> GetUser(Guid id)
        {
            var user = await _context.Users
                .Select(u => new UserAdminDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "لا يوجد",
                    PhoneNumber = u.PhoneNumber ?? "لا يوجد",
                    SSN = u.SSN,
                    IsActive = u.IsActive
                })
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound("المستخدم غير موجود");

            return Ok(user);
        }

       
        [HttpPut("toggle-status/{id}")]
        public async Task<IActionResult> ToggleUserStatus(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تغيير حالة المستخدم", status = user.IsActive });
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("تم حذف المستخدم نهائياً");
        }
    }
}