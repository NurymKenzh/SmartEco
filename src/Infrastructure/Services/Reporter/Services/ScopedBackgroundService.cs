namespace Reporter.Services
{
    public sealed class ScopedBackgroundService(
        IServiceProvider _serviceProvider,
        ILogger<ScopedBackgroundService> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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
            _logger.LogInformation($"{nameof(ScopedBackgroundService)} is running");

            using IServiceScope scope = _serviceProvider.CreateScope();
            IWorkerService workerService = scope.ServiceProvider.GetRequiredService<IWorkerService>();

            await workerService.DoWorkAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ScopedBackgroundService)} is stopping");

            await base.StopAsync(stoppingToken);
        }
    }
}
