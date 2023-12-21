using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reporter.Services;
using ServiceManager.Models;
using ServiceManager.Options;
using ServiceManager.Services.Logging;
using ServiceManager.Services;
using System.Security.Principal;
using ServiceManager.GrpcServices;

namespace ServiceManager
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ServicesOptions>(Configuration.GetSection(nameof(ServicesOptions)));
            services.Configure<DelayOptions>(Configuration.GetSection(nameof(DelayOptions)));

            services.AddScoped<ILoggerService, LoggerService>();
            services.AddScoped<IReporterService, ReporterService>();
            services.AddHostedService<ReporterBackgroundService>();

            services.AddGrpc();

            services.AddTransient<MainWindow>();
            Bindings.IsAdministrator = IsAdmin();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ServiceManagerGrpcService>()
                    .RequireHost("*:7227");
            });
        }

        private static bool IsAdmin()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
