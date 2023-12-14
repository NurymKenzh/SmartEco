namespace Reporter.Services
{
    public sealed class ScopedBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ScopedBackgroundService> logger) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<ScopedBackgroundService> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ScopedBackgroundService)} is running.");

            try
            {
                await DoWorkAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Worker stopping token was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);

                Environment.Exit(1);
            }
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ScopedBackgroundService)} is working.");

            using IServiceScope scope = _serviceProvider.CreateScope();
            IWorkerService workerService = scope.ServiceProvider.GetRequiredService<IWorkerService>();

            await workerService.DoWorkAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ScopedBackgroundService)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
