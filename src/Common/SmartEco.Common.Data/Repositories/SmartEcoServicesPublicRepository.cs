using Microsoft.EntityFrameworkCore;
using SmartEco.Common.Data.Contexts;
using SmartEco.Common.Data.Entities.SmartEcoServices;
using SmartEco.Common.Data.Repositories.Abstractions;
using SmartEco.Common.Enums;

namespace SmartEco.Common.Data.Repositories
{
    public class SmartEcoServicesPublicRepository : ISmartEcoServicesPublicRepository
    {
        private readonly SmartEcoServicesDbContext _context;

        public SmartEcoServicesPublicRepository(SmartEcoServicesDbContext context)
        {
            _context = context;
        }

        public async Task<DateTime?> GetLastWorked(WorkerType workerType)
        {
            var workerLastStart = await _context.WorkerLastStart.FirstOrDefaultAsync(worker => worker.WorkerType == workerType);
            return workerLastStart?.WorkedOn;
        }

        public async Task<DateTime> AddOrUpdateWorkerLastStart(WorkerType workerType)
        {
            var workedOn = DateTime.Now;
            if (await _context.WorkerLastStart.FirstOrDefaultAsync(worker => worker.WorkerType == workerType) is WorkerLastStart workerLastStart)
            {
                workerLastStart.WorkedOn = workedOn;
                _context.Update(workerLastStart);
            }
            else
            {
                _context.Add(new WorkerLastStart
                {
                    WorkerType = workerType,
                    WorkedOn = workedOn
                });
            }

            await _context.SaveChangesAsync();
            return workedOn;
        }
    }
}
