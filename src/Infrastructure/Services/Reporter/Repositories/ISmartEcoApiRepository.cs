using Reporter.Models;

namespace Reporter.Repositories
{
    internal interface ISmartEcoApiRepository
    {
        public Task<List<MeasuredDataDto>> GetMeasuredDatas(DateTime lastDateTime);
        public Task<List<MeasuredParameterDto>> GetMeasuredParameters();
        public Task<List<MonitoringPostDto>> GetMonitoringPosts();
        public Task<List<MonitoringPostMeasuredParameterDto>> GetMonitoringPostMeasuredParameters(List<MonitoringPostDto> monitoringPosts, List<MeasuredParameterDto> measuredParameters);
        public Task<List<PersonDto>> GetPersons(List<string> excludeEmails);
    }
}
