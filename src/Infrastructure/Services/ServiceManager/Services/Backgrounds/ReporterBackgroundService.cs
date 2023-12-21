using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceManager.Enums;
using ServiceManager.Models;
using ServiceManager.Options;
using ServiceManager.Services;
using ServiceManager.Services.Logging;
using SmartEco.Common.Services.Resources;

namespace Reporter.Services
{
    internal sealed class ReporterBackgroundService(
        ILogger<ReporterBackgroundService> _logger,
        IServiceProvider _serviceProvider,
        IOptions<DelayOptions> _delayOptions) : BackgroundService
    {
        private readonly int _delaySeconds = _delayOptions.Value.ReporterDelay;
        private readonly ColorType _colorType = ColorType.Black;
        private readonly string serviceName = ServiceNames.ServiceManager;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWorkAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning($"{nameof(ReporterBackgroundService)} stopping token was canceled");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{Message}", ex.Message);
                    await Task.Delay(TimeSpan.FromSeconds(_delaySeconds * 2), stoppingToken);
                }
            }
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var loggerService = scope.ServiceProvider.GetRequiredService<ILoggerService>();
                loggerService.AddInfoLog(serviceName, $"{nameof(ReporterBackgroundService)} is running", Bindings.ReporterTab, _colorType);
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                var reporterService = scope.ServiceProvider.GetRequiredService<IReporterService>();
                await reporterService.SetStatus();
                await Task.Delay(TimeSpan.FromSeconds(_delaySeconds), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ReporterBackgroundService)} is stopping");

            await base.StopAsync(stoppingToken);
        }
    }
}
