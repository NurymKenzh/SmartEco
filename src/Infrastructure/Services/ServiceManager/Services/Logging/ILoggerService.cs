using ServiceManager.Enums;
using ServiceManager.Models;

namespace ServiceManager.Services.Logging
{
    public interface ILoggerService
    {
        public void AddInfoLog(string service, string? message, ServiceTab serviceTab, ColorType colorType);
        public void AddWarningLog(string service, string message, ServiceTab serviceTab);
        public void AddErrorLog(string service, string message, ServiceTab serviceTab);
    }
}
