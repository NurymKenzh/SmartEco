using Refit;
using SmartEco.Web.Helpers.Middleware;
using SmartEco.Web.Services;
using SmartEco.Web.Services.Directories;
using SmartEco.Web.Services.Providers;

namespace SmartEco.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiProviders(configuration);
            services.AddScoped<AuthService>();
            services.AddScoped<PersonService>();
        }

        private static void AddApiProviders(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<AuthHeaderHandler>();
            services
                .AddRefitClient<ISmartEcoApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(configuration.GetValue<string>("SmartEcoApiUrl")))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddHttpMessageHandler<AuthHeaderHandler>();
        }
    }
}
