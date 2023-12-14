using Microsoft.Extensions.Logging;
using Reporter.Models;
using Reporter.Repositories;
using SmartEco.Common.Data.Entities.SmartEcoServices;
using SmartEco.Common.Services;
namespace Reporter.Services
{
    internal class CheckDataService(
        ILogger<CheckDataService> logger,
        ISmartEcoApiRepository apiRepository,
        ISmartEcoServicesRepository servicesRepository,
        IConfiguration configuration,
        IEmailService emailService) : ICheckDataService
    {
        const string Heading = "Нет данных!";

        readonly ILogger<CheckDataService> _logger = logger;
        readonly ISmartEcoApiRepository _apiRepository = apiRepository;
        readonly ISmartEcoServicesRepository _servicesRepository = servicesRepository;
        readonly IConfiguration _configuration = configuration;
        readonly IEmailService _emailService = emailService;

        public async Task Checking()
        {
            try
            {
                //Get datas
                var measuredDatas = await _apiRepository.GetMeasuredDatas(DateTime.Now.AddMinutes(-60)/*new DateTime(2023, 12, 08, 12, 42, 00)*/);
                var measuredParameters = await _apiRepository.GetMeasuredParameters();
                var monitoringPosts = await _apiRepository.GetMonitoringPosts();
                var monitoringPostMeasuredParameters = await _apiRepository.GetMonitoringPostMeasuredParameters(monitoringPosts, measuredParameters);
                var persons = await _apiRepository.GetPersons(GetExcludeEmails());
                var emails = GetEmails(persons);
                var eventLogs = await _servicesRepository.GetSendEmailEventLogs(DateTime.Now.AddHours(-24));

                string message = string.Empty;

                if (measuredDatas.Count is 0)
                {
                    //Checking has any data
                    await CheckingData(emails, eventLogs);
                }
                else
                {
                    foreach (var post in monitoringPosts)
                    {
                        foreach (var parameter in measuredParameters)
                        {
                            var monitoringPostMeasuredParameter = monitoringPostMeasuredParameters
                                .FirstOrDefault(m => m.MonitoringPostId == post.Id && m.MeasuredParameterId == parameter.Id);
                            var min = monitoringPostMeasuredParameter?.Min;
                            var max = monitoringPostMeasuredParameter?.Max;

                            //Checking post has any data
                            if (measuredDatas.FirstOrDefault(data => data.MonitoringPostId == post.Id) is null)
                            {
                                message += await CheckingPost(post, eventLogs);
                                break;
                            }
                            else if (monitoringPostMeasuredParameter is not null)
                            {
                                //Checking post and parameter has any data
                                if (measuredDatas.FirstOrDefault(data => data.MonitoringPostId == post.Id && data.MeasuredParameterId == parameter.Id) is null)
                                {
                                    message += await CheckingParameter(post, parameter, eventLogs);
                                }

                                var postParameterDatas = measuredDatas
                                    .Where(data => data.MonitoringPostId == post.Id && data.MeasuredParameterId == parameter.Id);

                                //Checking parameter has values less than acceptable
                                if (min is not null && postParameterDatas.Any(data => data.Value < Convert.ToDecimal(min)))
                                {
                                    message += await CheckingMinimum(post, parameter, eventLogs);
                                }

                                //Checking parameter has values more than acceptable
                                if (max is not null && postParameterDatas.Any(data => data.Value > Convert.ToDecimal(max)))
                                {
                                    message += await CheckingMaximum(post, parameter, eventLogs);
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        await SendMessage(message, emails);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking data");
            }
        }

        private async Task CheckingData(string[] emails, List<SendEmailEventLog> eventLogs)
        {
            var IsNoAnyDataLog = !eventLogs.Any(log => log.MonitoringPostId is null && log.MeasuredParameterId is null);

            if (eventLogs.Count is 0 || IsNoAnyDataLog)
            {
                var message = "Нет данных по всем постам!";
                await _servicesRepository.AddSendEmailEventLog(message);
                await SendMessage(message, emails);
            }
        }

        private async Task<string> CheckingPost(MonitoringPostDto post, List<SendEmailEventLog> eventLogs)
        {
            var IsNoEventLog = !eventLogs.Any(log => log.MonitoringPostId == post.Id && log.MeasuredParameterId is null);

            if (eventLogs.Count is 0 || IsNoEventLog)
            {
                string logText = $"Нет данных по посту {post.MN}";
                await _servicesRepository.AddSendEmailEventLog(logText, post.Id);
                return NewRowMessage(logText);
            }
            return string.Empty;
        }

        private async Task<string> CheckingParameter(MonitoringPostDto post, MeasuredParameterDto parameter, List<SendEmailEventLog> eventLogs)
        {
            var IsNoEventLog = !eventLogs.Any(log => log.MonitoringPostId == post.Id && log.MeasuredParameterId == parameter.Id);

            if (eventLogs.Count is 0 || IsNoEventLog)
            {
                string logText = $"Нет данных по \"{parameter.NameRU}\" по посту {post.MN}";
                await _servicesRepository.AddSendEmailEventLog(logText, post.Id, parameter.Id);
                return NewRowMessage(logText);
            }
            return string.Empty;
        }

        private async Task<string> CheckingMinimum(MonitoringPostDto post, MeasuredParameterDto parameter, List<SendEmailEventLog> eventLogs)
        {
            var IsNoEventLog = !eventLogs.Any(log => log.MonitoringPostId == post.Id && log.MeasuredParameterId == parameter.Id
                && (log.Information?.Contains("меньше минимума") ?? false));

            if (eventLogs.Count is 0 || IsNoEventLog)
            {
                string logText = $"На посту {post.MN} у параметра \"{parameter.NameRU}\" значение меньше минимума";
                await _servicesRepository.AddSendEmailEventLog(logText, post.Id, parameter.Id);
                return NewRowMessage(logText);
            }
            return string.Empty;
        }

        private async Task<string> CheckingMaximum(MonitoringPostDto post, MeasuredParameterDto parameter, List<SendEmailEventLog> eventLogs)
        {
            var IsNoEventLog = !eventLogs.Any(log => log.MonitoringPostId == post.Id && log.MeasuredParameterId == parameter.Id
                && (log.Information?.Contains("больше максимума") ?? false));

            if (eventLogs.Count is 0 || IsNoEventLog)
            {
                string logText = $"На посту {post.MN} у параметра \"{parameter.NameRU}\" значение больше максимума";
                await _servicesRepository.AddSendEmailEventLog(logText, post.Id, parameter.Id);
                return NewRowMessage(logText);
            }
            return string.Empty;
        }

        private static string NewRowMessage(string logText)
            => $"{logText} <br/>";

        private List<string> GetExcludeEmails()
            => _configuration
            .GetSection("ExcludeEmails")
            .Get<string[]>()
            ?.ToList() ?? [];

        private static string[] GetEmails(List<PersonDto> persons)
            => persons
            .Where(person => !string.IsNullOrEmpty(person.Email))
            .Select(person => person.Email!)
            .ToArray();

        private async Task SendMessage(string message, string[] emails)
        {
            try
            {
                _logger.LogInformation("Emails for sending:\n{Emails}", string.Join(", ", emails));
                _logger.LogInformation("Send checking result:\n{Message}", message);
                await _emailService.SendAsync(emails, Heading, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error send email");
            }
        }
    }

    internal interface ICheckDataService
    {
        public Task Checking();
    }
}
