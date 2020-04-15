using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemorizeThat.EmailManagement.Abstractions
{
    public interface IEmailSender
    {
        Task SendEmailAsync(
            [NotNull] string toEmail,
            [NotNull] string fromEmail,
            [NotNull] string fromName,
            [NotNull] string subject,
            [NotNull] string message);

    }
}
