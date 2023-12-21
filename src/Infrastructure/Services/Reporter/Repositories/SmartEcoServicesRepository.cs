using Microsoft.EntityFrameworkCore;
using Reporter.Models;
using SmartEco.Common.Data.Contexts;
using SmartEco.Common.Data.Entities.SmartEcoServices;

namespace Reporter.Repositories
{
    internal class SmartEcoServicesRepository(
        SmartEcoServicesDbContext _context) : ISmartEcoServicesRepository
    {
        public async Task<List<SendEmailEventLog>> GetSendEmailEventLogs(DateTime lastWriteDateTime)
            => await _context.SendEmailEventLog
            .Where(eventLog => eventLog.SendedOn > lastWriteDateTime)
            .OrderBy(eventLog => eventLog.SendedOn)
            .ToListAsync();

        public async Task AddSendEmailEventLog(
            string? information, 
            int? monitoringPostId = null, 
            int? measuredParameterId = null)
        {
            var sendEmailEventLog = new SendEmailEventLog
            {
                SendedOn = DateTime.Now,
                Information = information,
                MonitoringPostId = monitoringPostId,
                MeasuredParameterId = measuredParameterId
            };
            _context.Add(sendEmailEventLog);
            await _context.SaveChangesAsync();
        }
    }
}
