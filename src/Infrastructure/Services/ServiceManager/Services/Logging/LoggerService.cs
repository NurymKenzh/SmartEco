using ServiceManager.Enums;
using ServiceManager.Models;

namespace ServiceManager.Services.Logging
{
    internal class LoggerService : ILoggerService
    {
        public void AddInfoLog(string service, string message, ServiceTab serviceTab, ColorType colorType)
        => AddLog(service, message, serviceTab, colorType);

        public void AddWarningLog(string service, string message, ServiceTab serviceTab)
            => AddLog(service, message, serviceTab, ColorType.Orange);

        public void AddErrorLog(string service, string message, ServiceTab serviceTab)
            => AddLog(service, message, serviceTab, ColorType.Red);

        private static void AddLog(string service, string message, ServiceTab serviceTab, ColorType colorType)
        {
            if (serviceTab.Logs.Count > 10)
                serviceTab.Logs.RemoveAt(0);

            serviceTab.Logs.Add(new()
            {
                Text = $"{DateTime.Now} >> {service} >> {message}",
                TextColorType = colorType
            });
        }
    }
}
