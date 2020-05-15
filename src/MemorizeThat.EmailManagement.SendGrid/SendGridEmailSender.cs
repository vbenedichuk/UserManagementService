using JetBrains.Annotations;
using MemorizeThat.EmailManagement.Abstractions;
using MemorizeThat.EmailManagement.SendGrid.Configuration;
using MemorizeThat.EmailManagement.SendGrid.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace MemorizeThat.EmailManagement.SendGrid
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridConfiguration _sendGridConfiguration;
        private readonly ILogger<SendGridEmailSender> _logger;

        public SendGridEmailSender(
            [NotNull]IOptions<SendGridConfiguration> sendGridConfiguration,
            ILogger<SendGridEmailSender> logger
            )
        {
            if(sendGridConfiguration.Value == null)
            {
                throw new SendGridEmailSenderException("sendGridConfiguration.Value is empty.");
            }
            _sendGridConfiguration = sendGridConfiguration.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            [NotNull] string toEmail, 
            [NotNull] string fromEmail,
            [NotNull] string fromName,
            [NotNull] string subject, 
            [NotNull] string message)
        {
            await Task.Run(async() =>
            {
                _logger.LogDebug("Sending email to {0}", toEmail);
                var client = new SendGridClient(_sendGridConfiguration.ApiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(fromEmail, fromName),
                    Subject = subject,
                    PlainTextContent = message,
                    HtmlContent = message
                };
                msg.AddTo(new EmailAddress(toEmail));
                msg.SetClickTracking(false, false);

                var result = await client.SendEmailAsync(msg);
                _logger.LogDebug("Message sent {0} {1}", result.StatusCode, await result.Body.ReadAsStringAsync());
            }).ConfigureAwait(false);
        }
    }
}
