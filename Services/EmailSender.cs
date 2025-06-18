using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace JobMasterApi.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config["Email:From"]));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = htmlContent };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                // For development only: bypass SSL certificate validation
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // Try different secure socket options
                await smtp.ConnectAsync(
                    _config["Email:SmtpHost"],
                    int.Parse(_config["Email:SmtpPort"]),
                    SecureSocketOptions.StartTlsWhenAvailable
                );

                if (!smtp.IsConnected)
                {
                    throw new Exception("Failed to connect to SMTP server");
                }

                await smtp.AuthenticateAsync(_config["Email:Username"], _config["Email:Password"]);

                if (!smtp.IsAuthenticated)
                {
                    throw new Exception("Failed to authenticate with SMTP server");
                }

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", toEmail);
                throw; // Re-throw to maintain compatibility with IEmailSender
            }
        }
    }
}
