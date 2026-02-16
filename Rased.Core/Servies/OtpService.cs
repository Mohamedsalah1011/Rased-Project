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
        private readonly ISmsSender _smsSender;
        private readonly UserManager<ApplicationUser> _userManager;

        private const int OtpLength = 6;
        private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(5);

        public OtpService(
            IMemoryCache cache,
            ISmsSender smsSender,
            UserManager<ApplicationUser> userManager)
        {
            _cache = cache;
            _smsSender = smsSender;
            _userManager = userManager;
        }

        public async Task<IActionResult> SendOtpAsync(SendOtpDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PhoneNumber))
                return new BadRequestObjectResult("Phone number is required.");

            // Optional: ensure the phone number exists in the system
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null)
                return new BadRequestObjectResult("Phone number is not registered.");

            string code = GenerateOtp(OtpLength);

            string cacheKey = GetCacheKey(request.PhoneNumber);
            var entry = new OtpEntry
            {
                Code = code,
                ExpiresAt = DateTime.UtcNow.Add(OtpLifetime)
            };

            _cache.Set(cacheKey, entry, entry.ExpiresAt);

            string message = $"Your verification code is: {code}. It will expire in {(int)OtpLifetime.TotalMinutes} minutes.";
            await _smsSender.SendSmsAsync(request.PhoneNumber, message);

            return new OkObjectResult(new { Success = true, Message = "OTP sent successfully." });
        }

        public Task<IActionResult> VerifyOtpAsync(VerifyOtpDto request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Code))
            {
                return Task.FromResult<IActionResult>(new BadRequestObjectResult("Phone number and OTP code are required."));
            }

            string cacheKey = GetCacheKey(request.PhoneNumber);

            if (!_cache.TryGetValue<OtpEntry>(cacheKey, out var entry))
            {
                return Task.FromResult<IActionResult>(new BadRequestObjectResult("OTP has expired or is invalid."));
            }

            if (entry.ExpiresAt < DateTime.UtcNow)
            {
                _cache.Remove(cacheKey);
                return Task.FromResult<IActionResult>(new BadRequestObjectResult("OTP has expired."));
            }

            if (!string.Equals(entry.Code, request.Code, StringComparison.Ordinal))
            {
                return Task.FromResult<IActionResult>(new BadRequestObjectResult("Invalid OTP code."));
            }

            _cache.Remove(cacheKey);

            return Task.FromResult<IActionResult>(new OkObjectResult(new { Success = true, Message = "OTP verified successfully." }));
        }

        private static string GetCacheKey(string phoneNumber) => $"otp:{phoneNumber}";

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
