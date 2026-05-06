using Microsoft.AspNetCore.Mvc;
using Rased.Core.DTO.Account;
using Rased.Core.DTO.Account.Otp;
using Rased.Core.Identity;
using Rased.Core.DTO.Account.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rased.Core.ServiseContracts
{
    public interface IAccountService
    {
        Task<ActionResult<ApplicationUser>> PostRegister(RegisterDTO registerDTO);
        Task<IActionResult> IsPhoneNumberAvailable(string phoneNumber);
        Task<IActionResult> IsEmailAvailable(string email);
        Task<IActionResult> PostLogin(LoginDTo loginDTo);
        Task<IActionResult> PostLogout();
        Task<IActionResult> ForgotPasswordSendOtp(SendOtpDto request);
        Task<IActionResult> ResetPasswordWithOtp(ResetPasswordWithOtpDto request);
        Task<IActionResult> GetProfile(Guid userId);
        Task<IActionResult> UpdateProfile(Guid userId, UpdateProfileDto updateProfileDto);
        Task<IActionResult> ChangePassword(Guid userId, ChangePasswordDto changePasswordDto);
    }
}
