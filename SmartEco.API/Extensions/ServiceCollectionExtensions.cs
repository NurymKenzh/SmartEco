using SmartEco.API.Services.Directories;
using SmartEco.API.Services;

namespace SmartEco.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<AuthService>();
            services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<PersonFilteringService>();
        }
    }
}
