using MemorizeThat.EmailManagement.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MemorizeThat.EmailManagement.SendGrid.Configuration
{
    public static class SendGridEmailSenderConfigurationExtension
    {
        public static IServiceCollection AddSendgridEmailSender(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<SendGridConfiguration>(configuration);
            serviceCollection.AddTransient<IEmailSender, SendGridEmailSender>();
            return serviceCollection;
        }
    }
}
