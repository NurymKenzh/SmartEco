using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reporter.Services;
using ServiceManager.Models;
using ServiceManager.Options;
using ServiceManager.Services;
using ServiceManager.Services.Logging;
using System.Security.Principal;
using System.Windows;

namespace ServiceManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = new HostBuilder()
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
                    configBuilder.AddJsonFile("appsettings.json", optional: false);
                    configBuilder.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<ServicesOptions>(context.Configuration.GetSection(nameof(ServicesOptions)));
                    services.Configure<DelayOptions>(context.Configuration.GetSection(nameof(DelayOptions)));

                    services.AddScoped<ILoggerService, LoggerService>();
                    services.AddScoped<IReporterService, ReporterService>();
                    services.AddHostedService<ReporterBackgroundService>();

                    services.AddTransient<MainWindow>();
                    Bindings.IsAdministrator = IsAdmin();
                    ServiceProvider serviceProvider = services.BuildServiceProvider();
                    MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                    mainWindow.Show();
                })
                .Build();
        }


        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
            if (!Bindings.IsAdministrator)
                MessageBox.Show("If you want to manage services, you must run the application as administrator", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }

        private static bool IsAdmin()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
