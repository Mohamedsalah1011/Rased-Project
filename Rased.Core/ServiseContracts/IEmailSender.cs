using System.Threading.Tasks;

namespace Rased.Core.ServiseContracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
