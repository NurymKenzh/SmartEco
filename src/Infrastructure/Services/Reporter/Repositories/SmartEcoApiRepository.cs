using Microsoft.EntityFrameworkCore;
using Reporter.Models;
using SmartEco.Common.Data.Contexts;
using SmartEco.Common.Enums;

namespace Reporter.Repositories
{
    internal class SmartEcoApiRepository(
        SmartEcoApiDbContext context) : ISmartEcoApiRepository
    {
        private readonly SmartEcoApiDbContext _context = context;

        public async Task<List<MeasuredDataDto>> GetMeasuredDatas(DateTime lastDateTime)
            => await _context.MeasuredData
            .Where(data => data.DateTime > lastDateTime && data.DateTime != null)
            .Select(data => new MeasuredDataDto
            {
                Id = data.Id,
                DateTime = data.DateTime,
                MeasuredParameterId = data.MeasuredParameterId,
                MonitoringPostId = data.MonitoringPostId,
                Value = data.Value
            })
            .OrderBy(data => data.DateTime)
            .ToListAsync();

        public async Task<List<MeasuredParameterDto>> GetMeasuredParameters()
            => await _context.MeasuredParameter
            .Where(parameter => !string.IsNullOrEmpty(parameter.OceanusCode))
            .Select(parameter => new MeasuredParameterDto
            {
                Id = parameter.Id,
                OceanusCode = parameter.OceanusCode,
                NameRU = parameter.NameRU
            })
            .ToListAsync();

        public async Task<List<MonitoringPostDto>> GetMonitoringPosts()
            => await _context.MonitoringPost
            .Where(post => post.DataProviderId == (int)DataProviderType.Ecoservice)
            .Select(post => new MonitoringPostDto
            {
                Id = post.Id,
                MN = post.MN,
                DataProviderId = post.DataProviderId
            })
            .ToListAsync();

        public async Task<List<MonitoringPostMeasuredParameterDto>> GetMonitoringPostMeasuredParameters(List<MonitoringPostDto> monitoringPosts, List<MeasuredParameterDto> measuredParameters)
            => await _context.MonitoringPostMeasuredParameters
            .Where(mpmp => monitoringPosts.Select(post => post.Id).Contains(mpmp.MonitoringPostId)
                && measuredParameters.Select(p => p.Id).Contains(mpmp.MeasuredParameterId))
            .Select(mpmp => new MonitoringPostMeasuredParameterDto
            {
                MonitoringPostId = mpmp.MonitoringPostId,
                MeasuredParameterId = mpmp.MeasuredParameterId,
                Min = mpmp.Min,
                Max = mpmp.Max
            })
            .ToListAsync();

        public async Task<List<PersonDto>> GetPersons(List<string> excludeEmails)
            => await _context.Person
            .Where(person => (person.Role == $"{Role.Moderator}".ToLower() || person.Role == $"{Role.Admin}".ToLower())
                && person.Email != null && !excludeEmails.Contains(person.Email))
            .Select(person => new PersonDto
            {
                Id = person.Id,
                Email = person.Email
            })
            .ToListAsync();
    }
}
