using Microsoft.AspNetCore.Mvc;
using Rased.Core.DTO;
using Rased.Core.Identity;
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
        Task<IActionResult> GetLogout();
        Task<IActionResult> ForgotPasswordSendOtp(SendOtpDto request);
        Task<IActionResult> ResetPasswordWithOtp(ResetPasswordWithOtpDto request);
    }
}
