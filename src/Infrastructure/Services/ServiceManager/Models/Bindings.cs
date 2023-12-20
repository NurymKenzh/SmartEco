using System.Collections.ObjectModel;

namespace ServiceManager.Models
{
    internal static class Bindings
    {
        public static bool IsAdministrator { get; set; }

        public static ObservableCollection<ServiceMonitoring> ServiceMonitorings { get; set; } = [];
        public static ServiceTab ReporterTab { get; set; } = new();
    }
}
