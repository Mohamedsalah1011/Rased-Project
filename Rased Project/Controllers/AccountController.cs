using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rased.Core.DTO.Account;
using Rased.Core.DTO.Account.Otp;
using Rased.Core.Identity;
using Rased.Core.ServiseContracts;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Rased.Core.DTO.Account.Profile;

namespace Rased_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IOtpService _otpService;

        public AccountController(IAccountService accountService, IOtpService otpService)
        {
            _accountService = accountService;
            _otpService = otpService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApplicationUser>> postRegister(RegisterDTO registerDTO)
        {
            return await _accountService.PostRegister(registerDTO);
        }

        [HttpGet("is-phone-available")]
        [AllowAnonymous]
        public async Task<IActionResult> IsPhoneNumberAvailable(string phoneNumber)
        {
            return await _accountService.IsPhoneNumberAvailable(phoneNumber);
        }

        [HttpGet("is-email-available")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailAvailable(string email)
        {
            return await _accountService.IsEmailAvailable(email);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> postLogin(LoginDTo loginDTo)
        {
            return await _accountService.PostLogin(loginDTo);
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> PostLogout()
        {
            return await _accountService.PostLogout();
        }

        [HttpPost("send-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> SendOtp(SendOtpDto request)
        {
            return await _otpService.SendOtpAsync(request);
        }

        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto request)
        {
            return await _otpService.VerifyOtpAsync(request);
        }

        [HttpPost("forgot-password/send-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordSendOtp(SendOtpDto request)
        {
            return await _accountService.ForgotPasswordSendOtp(request);
        }

        [HttpPost("forgot-password/reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordWithOtp(ResetPasswordWithOtpDto request)
        {
            return await _accountService.ResetPasswordWithOtp(request);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return await _accountService.GetProfile(userId);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return await _accountService.UpdateProfile(userId, dto);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return await _accountService.ChangePassword(userId, dto);
        }
    }
}
