using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WasteReductionPlatform.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            var smtpSettings = configuration.GetSection("SmtpSettings");
            _smtpClient = new SmtpClient(smtpSettings["Server"])
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = true, // Enable SSL for STARTTLS
                Port = 587 // Port 587 for STARTTLS
            };
            _fromAddress = smtpSettings["SenderEmail"];
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                _logger.LogInformation($"Sending email to {email} from {_fromAddress}");
                var mailMessage = new MailMessage(_fromAddress, email, subject, htmlMessage)
                {
                    IsBodyHtml = true
                };

                await _smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully.");
            }
            catch (SmtpException ex)
            {
                _logger.LogError($"SMTP error: {ex.Message} - Status Code: {ex.StatusCode}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }
                throw; // Re-throw the exception after logging
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error sending email: {ex.Message}");
                throw; // Re-throw the exception after logging
            }
        }
    }
}
