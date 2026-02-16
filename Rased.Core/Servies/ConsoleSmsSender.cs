using Rased.Core.ServiseContracts;
using System;
using System.Threading.Tasks;

namespace Rased.Core.Servies
{
    /// <summary>
    /// Simple SMS sender that writes the OTP to the console.
    /// Replace this with a real SMS provider (e.g., Twilio) in production.
    /// </summary>
    public class ConsoleSmsSender : ISmsSender
    {
        public Task SendSmsAsync(string phoneNumber, string message)
        {
            Console.WriteLine($"[SMS] To: {phoneNumber}, Message: {message}");
            return Task.CompletedTask;
        }
    }
}
