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
            => await ButtonAction(_reporterService.SetStatus, Bindings.ReporterTab);

        private async void BtnReporterCreate_Click(object sender, RoutedEventArgs e)
            => await ButtonAction(_reporterService.Create, Bindings.ReporterTab);

        private async void BtnReporterRemove_Click(object sender, RoutedEventArgs e)
            => await ButtonAction(_reporterService.Remove, Bindings.ReporterTab);

        private async void BtnReporterStart_Click(object sender, RoutedEventArgs e)
            => await ButtonAction(_reporterService.Start, Bindings.ReporterTab);

        private async void BtnReporterStop_Click(object sender, RoutedEventArgs e)
            => await ButtonAction(_reporterService.Stop, Bindings.ReporterTab);

        private void BtnReporterItemMenuCopy_Click(object sender, RoutedEventArgs e)
            => Clipboard.SetText(ReporterLogsList.SelectedItem.ToString());

        private void BtnReporterItemMenuClear_Click(object sender, RoutedEventArgs e)
            => Bindings.ReporterTab.Logs.Clear();
        #endregion Reporter

        private void CanExecuteCopyLog(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;

        private async Task ButtonAction(Func<Task> action, ServiceTab serviceTab)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                _loggerService.AddErrorLog(
                    $"{ServiceNames.ServiceManager}.{action.Method.Name}",
                    $"{ex.Message}\n{ex.StackTrace}",
                    serviceTab);
            }
        }
    }
}