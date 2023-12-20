using Newtonsoft.Json.Linq;
using ServiceManager.Models;
using ServiceManager.Services;
using ServiceManager.Services.Logging;
using SmartEco.Common.Services.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ServiceManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILoggerService _loggerService;
        private readonly IReporterService _reporterService;

        public MainWindow(ILoggerService loggerService, IReporterService reporterService)
        {
            InitializeComponent();

            DataGridServices.ItemsSource = Bindings.ServiceMonitorings;

            ReporterGrid.DataContext = Bindings.ReporterTab;
            ReporterLogsList.ItemsSource = Bindings.ReporterTab.Logs;

            _loggerService = loggerService;
            _reporterService = reporterService;
        }

        #region Reporter
        private async void BtnReporterUpdateStatus_Click(object sender, RoutedEventArgs e)
            => await ButtonAction(_reporterService.SetStatus, Bindings.ReporterTab, nameof(ButtonState.IsEnabledUpdate));

        private async void BtnReporterCreate_Click(object sender, RoutedEventArgs e)
            => await ButtonAction(_reporterService.Create, Bindings.ReporterTab, nameof(ButtonState.IsEnabledCreate));

        private async void BtnReporterRemove_Click(object sender, RoutedEventArgs e)
            => await ButtonAction(_reporterService.Remove, Bindings.ReporterTab, nameof(ButtonState.IsEnabledRemove));

        private async void BtnReporterStart_Click(object sender, RoutedEventArgs e)
            => await ButtonAction(_reporterService.Start, Bindings.ReporterTab, nameof(ButtonState.IsEnabledStart));

        private async void BtnReporterStop_Click(object sender, RoutedEventArgs e)
            => await ButtonAction(_reporterService.Stop, Bindings.ReporterTab, nameof(ButtonState.IsEnabledStop));

        private void BtnReporterItemMenuCopy_Click(object sender, RoutedEventArgs e)
            => CopyToClipboard(ReporterLogsList);

        private void BtnReporterItemMenuSelectAll_Click(object sender, RoutedEventArgs e)
            => ReporterLogsList.SelectAll();

        private void BtnReporterItemMenuClear_Click(object sender, RoutedEventArgs e)
            => Bindings.ReporterTab.Logs.Clear();
        #endregion Reporter

        private void CanExecuteLogs(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;

        private static void CopyToClipboard(ListBox listBox)
            => Clipboard.SetText(string.Join("\r\n", new List<Log>(listBox.SelectedItems.Cast<Log>()).Select(log => log.Text)));

        private static Action<bool> GetActionEnabled(ServiceTab serviceTab, string isEnabledName)
            => isEnabledName switch
            {
                nameof(ButtonState.IsEnabledUpdate) => (isEnabled => serviceTab.BtnState.IsEnabledUpdate = isEnabled),
                nameof(ButtonState.IsEnabledCreate) => (isEnabled => serviceTab.BtnState.IsEnabledCreate = isEnabled),
                nameof(ButtonState.IsEnabledRemove) => (isEnabled => serviceTab.BtnState.IsEnabledRemove = isEnabled),
                nameof(ButtonState.IsEnabledStart) => (isEnabled => serviceTab.BtnState.IsEnabledStart = isEnabled),
                nameof(ButtonState.IsEnabledStop) => (isEnabled => serviceTab.BtnState.IsEnabledStop = isEnabled),
                _ => throw new NotImplementedException()
            };

        private async Task ButtonAction(Func<Task> action, ServiceTab serviceTab, string isEnabledName)
        {
            var btnEnabledSetter = GetActionEnabled(serviceTab, isEnabledName);
            try
            {
                btnEnabledSetter(false);
                await action();
                _loggerService.AddInfoLog(
                   $"{ServiceNames.ServiceManager}.{action.Method.Name}",
                   null,
                   serviceTab,
                   Enums.ColorType.Black);
            }
            catch (InvalidOperationException ex)
            {
                _loggerService.AddErrorLog(
                    $"{ServiceNames.ServiceManager}.{action.Method.Name}",
                    $"{ex.Message}",
                    serviceTab);
                btnEnabledSetter(true);
            }
            catch (Exception ex)
            {
                _loggerService.AddErrorLog(
                    $"{ServiceNames.ServiceManager}.{action.Method.Name}",
                    $"{ex.Message}\n{ex.StackTrace}",
                    serviceTab);
                btnEnabledSetter(true);
            }
        }
    }
}