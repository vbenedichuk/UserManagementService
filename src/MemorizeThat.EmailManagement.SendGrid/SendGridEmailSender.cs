using JetBrains.Annotations;
using MemorizeThat.EmailManagement.Abstractions;
using MemorizeThat.EmailManagement.SendGrid.Configuration;
using MemorizeThat.EmailManagement.SendGrid.Exceptions;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace MemorizeThat.EmailManagement.SendGrid
{
    public class SendGridEmailSender : IEmailSender
    {
        private SendGridConfiguration _sendGridConfiguration;

        public SendGridEmailSender([NotNull]IOptions<SendGridConfiguration> sendGridConfiguration)
        {
            if(sendGridConfiguration.Value == null)
            {
                throw new SendGridEmailSenderException("sendGridConfiguration.Value is empty.");
            }
            _sendGridConfiguration = sendGridConfiguration.Value;

        }

        public Task SendEmailAsync(
            [NotNull] string toEmail, 
            [NotNull] string fromEmail,
            [NotNull] string fromName,
            [NotNull] string subject, 
            [NotNull] string message)
        {
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

            return client.SendEmailAsync(msg);
        }
    }
}
