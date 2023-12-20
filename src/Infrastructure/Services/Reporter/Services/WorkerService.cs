using SmartEco.Common.Data.Repositories.Abstractions;
using SmartEco.Common.Enums;

namespace Reporter.Services
{
    internal sealed class WorkerService(
        ILogger<WorkerService> _logger,
        ISmartEcoServicesPublicRepository _publicRepository,
        ICheckDataService _checkDataService) : IWorkerService
    {
        private readonly int _delayMinutes = 1;

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var lastCheckDataOn = await _publicRepository.GetLastWorked(WorkerType.ReporterCheckData) ?? new DateTime();
            while (!stoppingToken.IsCancellationRequested)
            {
                if (lastCheckDataOn.AddHours(1) < DateTime.Now)
                {
                    await _checkDataService.Checking();
                    lastCheckDataOn = await _publicRepository.AddOrUpdateWorkerLastStart(WorkerType.ReporterCheckData);
                }
                await Task.Delay(TimeSpan.FromMinutes(_delayMinutes), stoppingToken);
            }
        }
    }

    internal interface IWorkerService
    {
        Task DoWorkAsync(CancellationToken stoppingToken);
    }
}
