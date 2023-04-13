using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartEco.Common.Data.Contexts;
using SmartEco.Common.Data.Repositories;
using SmartEco.Common.Data.Repositories.Abstractions;
using SmartEco.Common.Options;
using SmartEco.Common.Services;

namespace SmartEco.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterRepository(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SmartEcoDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ISmartEcoRepository, SmartEcoRepository>(sp =>
            {
                var dbContext = sp.GetRequiredService<SmartEcoDbContext>();
                ArgumentNullException.ThrowIfNull(dbContext);
                return new SmartEcoRepository(dbContext);
            });
        }

        public static void AddEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.Email));
            services.AddScoped<IEmailService, EmailService>();
        }
    }
}
