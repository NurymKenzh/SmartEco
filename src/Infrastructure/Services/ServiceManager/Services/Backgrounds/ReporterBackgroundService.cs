using Microsoft.Extensions.Hosting;
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
        ILoggerService loggerService,
        IOptions<ServicesOptions> servicesOption,
        IOptions<DelayOptions> delayOptions,
        IReporterService reporterService) : BackgroundService
    {
        private readonly int _delaySeconds = delayOptions.Value.ReporterDelay;
        private readonly ColorType _colorType = ColorType.Black;
        private readonly string serviceName = ServiceNames.ServiceManager;

        private readonly ILoggerService _loggerService = loggerService;
        private readonly ServicesOptions _servicesOption = servicesOption.Value;
        private readonly IReporterService _reporterService = reporterService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _loggerService.AddInfoLog(serviceName, $"{nameof(ReporterBackgroundService)} is running", Bindings.ReporterTab, _colorType);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWorkAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _loggerService.AddWarningLog(serviceName, $"{nameof(ReporterBackgroundService)} stopping token was canceled", Bindings.ReporterTab);
                }
                catch (Exception ex)
                {
                    _loggerService.AddErrorLog(serviceName, $"{nameof(ReporterBackgroundService)}: {ex.Message}\n{ex.StackTrace}", Bindings.ReporterTab);
                    await Task.Delay(TimeSpan.FromSeconds(_delaySeconds * 2), stoppingToken);
                }
            }
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _loggerService.AddInfoLog(serviceName, $"{nameof(ReporterBackgroundService)} is working", Bindings.ReporterTab, _colorType);

            while (!stoppingToken.IsCancellationRequested)
            {
                await _reporterService.SetStatus();
                await Task.Delay(TimeSpan.FromSeconds(_delaySeconds), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _loggerService.AddInfoLog(serviceName, $"{nameof(ReporterBackgroundService)} is stopping", Bindings.ReporterTab, _colorType);

            await base.StopAsync(stoppingToken);
        }
    }
}
