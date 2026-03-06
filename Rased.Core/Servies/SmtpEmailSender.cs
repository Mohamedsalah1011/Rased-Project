using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Rased.Core.ServiseContracts;

namespace Rased.Core.Servies
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var host = _configuration["Smtp:Host"];
            var port = int.Parse(_configuration["Smtp:Port"]);
            var user = _configuration["Smtp:User"];
            var pass = _configuration["Smtp:Pass"];
            var from = _configuration["Smtp:From"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true
            };

            var message = new MailMessage(from, toEmail, subject, body);
            await client.SendMailAsync(message);
        }
    }
}
