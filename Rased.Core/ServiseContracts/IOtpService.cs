using Microsoft.AspNetCore.Mvc;
using Rased.Core.DTO;
using System.Threading.Tasks;

namespace Rased.Core.ServiseContracts
{
    public interface IOtpService
    {
        Task<IActionResult> SendOtpAsync(SendOtpDto request);
        Task<IActionResult> VerifyOtpAsync(VerifyOtpDto request);
    }
}
