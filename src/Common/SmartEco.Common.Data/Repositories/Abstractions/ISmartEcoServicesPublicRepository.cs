using SmartEco.Common.Enums;

namespace SmartEco.Common.Data.Repositories.Abstractions
{
    public interface ISmartEcoServicesPublicRepository
    {
        public Task<DateTime?> GetLastWorked(WorkerType workerType);
        public Task<DateTime> AddOrUpdateWorkerLastStart(WorkerType workerType);
    }
}
