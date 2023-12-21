using ServiceManager.Enums;
using ServiceManager.Models;
using System.Management.Automation;

namespace ServiceManager.Services
{
    internal class BaseService
    {
        private static readonly string _failedAction = "Failed! Check the specified path, status or settings";

        public static async Task<(ServiceStatus, string?)> GetStatus(string serviceName)
        {
            using PowerShell PowerShellInst = PowerShell.Create();
            PowerShellInst.AddScript($"Get-Service \"{serviceName}\"");

            var PSOutput = await PowerShellInst.InvokeAsync();

            return ServiceInfo(PSOutput);
        }

        public static async Task<(ServiceStatus, string?)> CreateService(string serviceName, string servicePath)
        {
            using PowerShell PowerShellInst = PowerShell.Create();
            using PowerShell PowerShellInstFailure = PowerShell.Create();
            PowerShellInst.AddScript($"New-Service -Name \"{serviceName}\" -BinaryPathName '{servicePath}' -StartupType \"Automatic\"");
            PowerShellInstFailure.AddScript($"sc.exe failure \"{serviceName}\" reset=0 actions=restart/60000/restart/60000/run/1000");

            var PSOutput = await PowerShellInst.InvokeAsync();
            await PowerShellInstFailure.InvokeAsync();

            var serviceInfo = ServiceInfo(PSOutput);
            if (serviceInfo.Item1 is ServiceStatus.Undefined)
                throw new InvalidOperationException(_failedAction);
            return serviceInfo;
        }

        public static async Task<(ServiceStatus, string?)> RemoveService(string serviceName)
        {
            using PowerShell PowerShellInst = PowerShell.Create();
            PowerShellInst.AddScript($"Remove-Service \"{serviceName}\"");

            var PSOutput = await PowerShellInst.InvokeAsync();

            var serviceInfo = ServiceInfo(PSOutput);
            return serviceInfo;
        }

        public static async Task<(ServiceStatus, string?)> StartService(string serviceName)
        {
            using PowerShell PowerShellInst = PowerShell.Create();
            PowerShellInst.AddScript($"Start-Service \"{serviceName}\"");

            var PSOutput = await PowerShellInst.InvokeAsync();

            var serviceInfo = ServiceInfo(PSOutput);
            if (serviceInfo.Item1 is ServiceStatus.Undefined)
                throw new InvalidOperationException(_failedAction);
            return serviceInfo;
        }

        public static async Task<(ServiceStatus, string?)> StopService(string serviceName)
        {
            using PowerShell PowerShellInst = PowerShell.Create();
            PowerShellInst.AddScript($"Stop-Service \"{serviceName}\"");

            var PSOutput = await PowerShellInst.InvokeAsync();

            var serviceInfo = ServiceInfo(PSOutput);
            if (serviceInfo.Item1 is ServiceStatus.Undefined)
                throw new InvalidOperationException(_failedAction);
            return serviceInfo;
        }

        public static void UpdateMonitoringList(
            string serviceName,
            ServiceState serviceState, 
            ServiceMonitoring? serviceRow,
            string? displayName = null)
        {
            if (serviceRow is null)
            {
                Bindings.ServiceMonitorings.Add(new ServiceMonitoring()
                {
                    Number = Bindings.ServiceMonitorings.Count + 1,
                    Name = serviceName,
                    ServiceState = serviceState,
                    DisplayName = displayName,
                    LastTimeCheking = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            else
            {
                serviceRow.ServiceState = serviceState;
                serviceRow.DisplayName = displayName;
                serviceRow.LastTimeCheking = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public static ColorType GetLampColor(ServiceStatus status)
            => status switch
            {
                ServiceStatus.Running => ColorType.Green,
                ServiceStatus.Undefined => ColorType.Gray,
                ServiceStatus.Paused => ColorType.Orange,
                ServiceStatus.Stopped or ServiceStatus.Unknown => ColorType.Red,
                _ => ColorType.Blue
            };

        public static ButtonState GetButtonState(ServiceStatus status)
            => !Bindings.IsAdministrator ? new() : new()
            {
                IsEnabledUpdate = true,
                IsEnabledCreate = status is ServiceStatus.Undefined,
                IsEnabledRemove = status is not (ServiceStatus.Running or ServiceStatus.StartPending or ServiceStatus.Undefined),
                IsEnabledStart = status is not (ServiceStatus.Running or ServiceStatus.StartPending or ServiceStatus.Undefined),
                IsEnabledStop = status is (ServiceStatus.Running or ServiceStatus.StartPending),
            };

        private static (ServiceStatus, string?) ServiceInfo(PSDataCollection<PSObject>? PSOutput)
        {
            string? displayName = null;
            if (PSOutput == null || PSOutput.Count == 0)
            {
                return (ServiceStatus.Undefined, displayName);
            }
            else
            {
                Enum.TryParse(typeof(ServiceStatus), PSOutput.First().Properties["Status"].Value?.ToString(), out object? statusValue);
                displayName = PSOutput.FirstOrDefault()?.Properties["DisplayName"].Value?.ToString();
                return ((ServiceStatus?)statusValue ?? ServiceStatus.Unknown, displayName);
            }
        }
    }
}
