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
        private readonly IOtpService _otpService;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IJWTService jwtService,
            IOtpService otpService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _otpService = otpService;
        }

        public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
                return new BadRequestResult();

            ApplicationUser user = new ApplicationUser
            {
                UserName = registerDTO.Email,
                FullName = registerDTO.FullName,
                Email = registerDTO.Email,
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

        public async Task<IActionResult> IsEmailAvailable(string email)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return new OkObjectResult(true);
            else
                return new OkObjectResult(false);
        }

        public async Task<IActionResult> PostLogin(LoginDTo loginDTo)
        {
            var result = await _signInManager.PasswordSignInAsync(
                loginDTo.Email,
                loginDTo.Password,
                false,
                false);

            if (result.Succeeded)
            {
                ApplicationUser? user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Email == loginDTo.Email);

                if (user == null)
                    return new NoContentResult();

                await _signInManager.SignInAsync(user, false);

                var authenticationResonse = _jwtService.GenerateToken(user);

                return new OkObjectResult(authenticationResonse);
            }
            else
            {
                return new ObjectResult("Invalid Email or password") { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> GetLogout()
        {
            await _signInManager.SignOutAsync();
            return new OkObjectResult("Logged out successfully");
        }

        public async Task<IActionResult> ForgotPasswordSendOtp(SendOtpDto request)
        {
            return await _otpService.SendOtpAsync(request);
        }

        public async Task<IActionResult> ResetPasswordWithOtp(ResetPasswordWithOtpDto request)
        {
            if (request == null)
                return new BadRequestObjectResult("Request is required.");

            if (request.NewPassword != request.ConfirmPassword)
                return new BadRequestObjectResult("Passwords do not match.");

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return new BadRequestObjectResult("Email is not registered.");

            bool isValidOtp = await _otpService.ValidateOtpAsync(request.Email, request.Code);

            if (!isValidOtp)
                return new BadRequestObjectResult("OTP has expired or is invalid.");

            // Reset password without requiring the old password.
            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                string errorMessage = string.Join(" | ", removeResult.Errors.Select(e => e.Description));
                return new ObjectResult(errorMessage) { StatusCode = 500 };
            }

            var addResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
            if (!addResult.Succeeded)
            {
                string errorMessage = string.Join(" | ", addResult.Errors.Select(e => e.Description));
                return new ObjectResult(errorMessage) { StatusCode = 500 };
            }

            return new OkObjectResult("Password has been reset successfully.");
        }
    }
}
