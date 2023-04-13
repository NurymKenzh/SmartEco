using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartEco.Common.Options;
using SmartEco.Common.Services;

namespace SmartEco.Common
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.Email));
            services.AddScoped<IEmailService, EmailService>();
        }
    }
}
