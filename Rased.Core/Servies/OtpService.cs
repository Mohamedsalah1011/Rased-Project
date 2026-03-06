using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Rased.Core.DTO;
using Rased.Core.Identity;
using Rased.Core.ServiseContracts;
using System;
using System.Threading.Tasks;

namespace Rased.Core.Servies
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        private const int OtpLength = 6;
        private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(5);

        public OtpService(
            IMemoryCache cache,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager)
        {
            _cache = cache;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task<IActionResult> SendOtpAsync(SendOtpDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
                return new BadRequestObjectResult("Email is required.");

            // Optional: ensure the email exists in the system
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return new BadRequestObjectResult("Email is not registered.");

            string code = GenerateOtp(OtpLength);

            string cacheKey = GetCacheKey(request.Email);
            var entry = new OtpEntry
            {
                Code = code,
                ExpiresAt = DateTime.UtcNow.Add(OtpLifetime)
            };

            _cache.Set(cacheKey, entry, entry.ExpiresAt);

            string subject = "Your verification code";
            string message = $"Your verification code is: {code}. It will expire in {(int)OtpLifetime.TotalMinutes} minutes.";
            await _emailSender.SendEmailAsync(request.Email, subject, message);

            return new OkObjectResult(new { Success = true, Message = "OTP sent successfully." });
        }

        public async Task<IActionResult> VerifyOtpAsync(VerifyOtpDto request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Code))
            {
                return new BadRequestObjectResult("Email and OTP code are required.");
            }

            bool isValid = await ValidateOtpAsync(request.Email, request.Code);

            if (!isValid)
            {
                return new BadRequestObjectResult("OTP has expired or is invalid.");
            }

            return new OkObjectResult(new { Success = true, Message = "OTP verified successfully." });
        }

        public Task<bool> ValidateOtpAsync(string email, string code)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                return Task.FromResult(false);
            }

            string cacheKey = GetCacheKey(email);

            if (!_cache.TryGetValue<OtpEntry>(cacheKey, out var entry))
            {
                return Task.FromResult(false);
            }

            if (entry.ExpiresAt < DateTime.UtcNow)
            {
                _cache.Remove(cacheKey);
                return Task.FromResult(false);
            }

            if (!string.Equals(entry.Code, code, StringComparison.Ordinal))
            {
                return Task.FromResult(false);
            }

            _cache.Remove(cacheKey);

            return Task.FromResult(true);
        }

        private static string GetCacheKey(string Email) => $"otp:{Email}";

        private static string GenerateOtp(int length)
        {
            const string digits = "0123456789";
            var data = new byte[length];
            var result = new char[length];

            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            for (int i = 0; i < length; i++)
            {
                result[i] = digits[data[i] % digits.Length];
            }

            return new string(result);
        }

        private sealed class OtpEntry
        {
            public string Code { get; set; } = default!;
            public DateTime ExpiresAt { get; set; }
        }
    }
}
