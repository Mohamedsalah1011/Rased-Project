using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rased.Core.DTO.Account;
using Rased.Core.DTO.Account.Otp;
using Rased.Core.DTO.Account.Otp.Enum;
using Rased.Core.DTO.Account.Profile;
using Rased.Core.Identity;
using Rased.Core.ServiseContracts;
using Rased_Project;
using Rased.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Infrustracture.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJWTService _jwtService;
        private readonly IOtpService _otpService;
        private readonly ApplicationDbContext _db;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IJWTService jwtService,
            IOtpService otpService,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _otpService = otpService;
            _db = db;
        }

        public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
                return new BadRequestResult();

            ApplicationUser user = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber
            };

            try
            {
                IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

                if (result.Succeeded)
                {
                    // Ensure Role exists
                    var roleName = registerDTO.UserType.ToString();
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                    }
                    await _userManager.AddToRoleAsync(user, roleName);

                    // Create Profile
                    var profile = new UserProfile
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        FullName = registerDTO.FullName!,
                        SSN = registerDTO.SSN,
                        PlateNumber = registerDTO.UserType == Core.Enums.UserType.Driver ? registerDTO.PlateNumber : null
                    };
                    _db.UserProfiles.Add(profile);
                    await _db.SaveChangesAsync();
                    
                    var sendOtpRequest = new SendOtpDto
                    {
                        Email = user.Email,
                        Type = OtpType.Register 
                    };

                    await _otpService.SendOtpAsync(sendOtpRequest);

                    return new OkObjectResult(new
                    {
                        Success = true,
                        Message = "User registered successfully. OTP sent to email.",
                        Email = user.Email
                    });
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
            var user = await _userManager.FindByEmailAsync(loginDTo.Email);

            if (user == null)
                return new BadRequestObjectResult("Invalid Email or Password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTo.Password, false);

            if (result.Succeeded)
            {
                var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
                var authenticationResponse = _jwtService.GenerateToken(user, profile?.FullName);
                return new OkObjectResult(authenticationResponse);
            }
            else
            {
                return new BadRequestObjectResult("Invalid Email or password");
            }
        }

        public async Task<IActionResult> PostLogout()
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

            bool isValidOtp = await _otpService.ValidateOtpAsync(request.Email, request.Code, OtpType.ForgotPassword);

            if (!isValidOtp)
                return new BadRequestObjectResult("OTP has expired or is invalid.");

            // Reset password without requiring the old password.
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (!result.Succeeded)
            {
                string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
                return new ObjectResult(errorMessage) { StatusCode = 500 };
            }

            return new OkObjectResult("Password has been reset successfully.");
        }

        public async Task<IActionResult> GetProfile(Guid userId)
        {
            var profile = await _db.UserProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return new NotFoundObjectResult("Profile not found");

            var userRoles = await _userManager.GetRolesAsync(profile.User);
            var response = new ProfileResponseDto
            {
                Id = profile.UserId,
                FullName = profile.FullName,
                Email = profile.User.Email!,
                PhoneNumber = profile.User.PhoneNumber!,
                SSN = profile.SSN,
                PlateNumber = userRoles.Contains("Driver") ? profile.PlateNumber : null,
                ProfilePictureUrl = profile.ProfilePictureUrl
            };

            return new OkObjectResult(response);
        }

        public async Task<IActionResult> UpdateProfile(Guid userId, UpdateProfileDto updateProfileDto)
        {
            var profile = await _db.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return new NotFoundObjectResult("Profile not found");

            profile.FullName = updateProfileDto.FullName;
            profile.PlateNumber = updateProfileDto.PlateNumber;
            
            // Update phone number in Identity table
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.PhoneNumber = updateProfileDto.PhoneNumber;
                await _userManager.UpdateAsync(user);
            }

            _db.UserProfiles.Update(profile);
            await _db.SaveChangesAsync();

            return new OkObjectResult(new { Message = "Profile updated successfully" });
        }

        public async Task<IActionResult> ChangePassword(Guid userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return new NotFoundObjectResult("User not found");

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
                return new BadRequestObjectResult("Passwords do not match");

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
                return new BadRequestObjectResult(errorMessage);
            }

            return new OkObjectResult(new { Message = "Password changed successfully" });
        }
    }
}
