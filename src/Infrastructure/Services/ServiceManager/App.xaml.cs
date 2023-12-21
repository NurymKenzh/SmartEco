using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        private readonly IWebHost _host;

        public App()
        {
            _host = CreateWebHostBuilder().Build();
        }

        private static IWebHostBuilder CreateWebHostBuilder() =>
           WebHost.CreateDefaultBuilder()
            .UseStartup<Startup>()
            .UseKestrel()
            .UseUrls($"https://*:7227");

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
    }
}
