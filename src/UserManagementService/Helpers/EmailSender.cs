using System;
using System.Threading.Tasks;
using UserManagementService.Abstractions;

namespace UserManagementService.Helpers
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            throw new NotImplementedException();
        }
    }
}
