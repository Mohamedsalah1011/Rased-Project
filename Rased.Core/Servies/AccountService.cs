using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rased.Core.DTO;
using Rased.Core.Identity;
using Rased.Core.ServiseContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.Servies
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJWTService _jwtService;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IJWTService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
                return new BadRequestResult();

            ApplicationUser user = new ApplicationUser
            {
                UserName = registerDTO.PhoneNumber,
                FullName = registerDTO.FullName,
                PhoneNumber = registerDTO.PhoneNumber,
                SSN = registerDTO.SSN
            };

            try
            {
                IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    var authenticationResonse = _jwtService.GenerateToken(user);
                    return new OkObjectResult(authenticationResonse);
                }
                else
                {
                    string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
                    return new ObjectResult(errorMessage) { StatusCode = 500 };
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> IsPhoneNumberAvailable(string phoneNumber)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
                return new OkObjectResult(true);
            else
                return new OkObjectResult(false);
        }

        public async Task<IActionResult> PostLogin(LoginDTo loginDTo)
        {
            var result = await _signInManager.PasswordSignInAsync(
                loginDTo.PhoneNumber,
                loginDTo.Password,
                false,
                false);

            if (result.Succeeded)
            {
                ApplicationUser? user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == loginDTo.PhoneNumber);

                if (user == null)
                    return new NoContentResult();

                await _signInManager.SignInAsync(user, false);

                var authenticationResonse = _jwtService.GenerateToken(user);

                return new OkObjectResult(authenticationResonse);
            }
            else
            {
                return new ObjectResult("Invalid phone number or password") { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> GetLogout()
        {
            await _signInManager.SignOutAsync();
            return new OkObjectResult("Logged out successfully");
        }
    }
}
