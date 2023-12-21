using SmartEco.Common.Data.Repositories.Abstractions;
using SmartEco.Common.Enums;
using SmartEco.Common.Services.GrpcClients;

namespace Reporter.Services
{
    internal sealed class WorkerService(
        ISmartEcoServicesPublicRepository _publicRepository,
        ICheckDataService _checkDataService,
        IServiceManagerGrpcClient _serviceManagerClient) : IWorkerService
    {
        private readonly int _delayMinutes = 1;

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            await _serviceManagerClient.SendInfoLog(WorkerType.Reporter, $"Worker running at: {DateTimeOffset.Now}");

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
