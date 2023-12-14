using SmartEco.Common.Data.Entities.SmartEcoServices;

namespace Reporter.Repositories
{
    internal interface ISmartEcoServicesRepository
    {
        public Task<List<SendEmailEventLog>> GetSendEmailEventLogs(DateTime lastWriteDateTime);
        public Task AddSendEmailEventLog(string? information, int? monitoringPostId = null, int? measuredParameterId = null);
    }
}
