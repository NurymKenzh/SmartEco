using Microsoft.Extensions.Options;
using ServiceManager.Enums;
using ServiceManager.Models;
using ServiceManager.Options;

namespace ServiceManager.Services
{
    internal sealed class ReporterService(
        IOptions<ServicesOptions> servicesOption) : BaseService, IReporterService
    {
        private readonly string _serviceName = servicesOption.Value.Reporter.Name;
        private readonly string _servicePath = servicesOption.Value.Reporter.Path;

        public async Task SetStatus()
        {
            (var status, var displayName) = await GetStatus(_serviceName);
            UpdateListMonitoring(status, displayName);
        }

        public async Task Create()
        {
            (var status, var displayName) = await CreateService(_serviceName, _servicePath);
            UpdateListMonitoring(status, displayName);
        }

        public async Task Remove()
        {
            (var status, var displayName) = await RemoveService(_serviceName);
            UpdateListMonitoring(status, displayName);
        }

        public async Task Start()
        {
            (var status, var displayName) = await StartService(_serviceName);
            UpdateListMonitoring(status, displayName);
        }

        public async Task Stop()
        {
            (var status, var displayName) = await StopService(_serviceName);
            UpdateListMonitoring(status, displayName);
        }

        private void UpdateListMonitoring(ServiceStatus status, string? displayName = null)
        {
            ServiceMonitoring? serviceRow = Bindings.ServiceMonitorings
                .FirstOrDefault(c => c.Name == _serviceName);
            var serviceState = new ServiceState()
            {
                Status = status,
                LampColorType = GetLampColor(status)
            };

            Bindings.ReporterTab.ServiceState = serviceState;
            Bindings.ReporterTab.BtnState = GetButtonState(status);
            UpdateMonitoringList(_serviceName, serviceState, serviceRow, displayName);
        }
    }

    public interface IReporterService
    {
        public Task SetStatus();
        public Task Create();
        public Task Remove();
        public Task Start();
        public Task Stop();
    }
}
