using SmartEco.Web.Services;
using SmartEco.Web.Services.Directories;

namespace SmartEco.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {

            services.AddScoped<AuthService>();
            services.AddScoped<CrudService>();
            services.AddScoped<PersonService>();
        }
    }
}
