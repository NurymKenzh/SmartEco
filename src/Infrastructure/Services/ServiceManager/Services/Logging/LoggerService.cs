using ServiceManager.Enums;
using ServiceManager.Models;
using System.Windows;

namespace ServiceManager.Services.Logging
{
    internal class LoggerService : ILoggerService
    {
        public void AddInfoLog(string service, string? message, ServiceTab serviceTab, ColorType colorType)
        => AddLog(service, message, serviceTab, colorType);

        public void AddWarningLog(string service, string message, ServiceTab serviceTab)
            => AddLog(service, message, serviceTab, ColorType.Orange);

        public void AddErrorLog(string service, string message, ServiceTab serviceTab)
            => AddLog(service, message, serviceTab, ColorType.Red);

        private static void AddLog(string service, string? message, ServiceTab serviceTab, ColorType colorType)
        {
            if (serviceTab.Logs.Count > 5000)
                serviceTab.Logs.RemoveAt(serviceTab.Logs.Count - 1);

            Application.Current.Dispatcher.Invoke(() =>
                serviceTab.Logs.Insert(0, new()
                {
                    Text = $"{string.Join(" >> ", new List<object?> { DateTime.Now, service, message }.Where(s => s is not null))}",
                    TextColorType = colorType
                })
            );
        }
    }
}
