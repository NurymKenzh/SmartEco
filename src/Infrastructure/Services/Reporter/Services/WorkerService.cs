using SmartEco.Common.Data.Repositories.Abstractions;
using SmartEco.Common.Enums;

namespace Reporter.Services
{
    internal sealed class WorkerService(
        ILogger<WorkerService> logger,
        ISmartEcoServicesPublicRepository publicRepository,
        ICheckDataService checkDataService) : IWorkerService
    {
        private readonly int _delayMinutes = 1;
        private readonly ILogger<WorkerService> _logger = logger;
        private readonly ISmartEcoServicesPublicRepository _publicRepository = publicRepository;
        private readonly ICheckDataService _checkDataService = checkDataService;

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
