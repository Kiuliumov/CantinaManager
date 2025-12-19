using System.Net;
using System.Net.Mail;

namespace CantinaManager.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var smtpHost = _config["EMAIL_SMTPHOST"];
            var smtpPort = int.Parse(_config["EMAIL_SMTPPORT"] ?? "587");
            var smtpUser = _config["EMAIL_SMTPUSER"];
            var smtpPass = _config["EMAIL_SMTPPASS"];
            var fromEmail = _config["EMAIL_FROM"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(fromEmail, toEmail, subject, htmlMessage)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}