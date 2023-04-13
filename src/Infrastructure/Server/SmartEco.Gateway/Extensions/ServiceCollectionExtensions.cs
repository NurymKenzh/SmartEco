using SmartEco.Gateway.Services.Directories;
using SmartEco.Gateway.Services;

namespace SmartEco.Gateway.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<AuthService>();
            services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));
            services.AddScoped<PersonFilteringService>();
        }
    }
}
