using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ServiceManager.Models
{
    internal class ServiceMonitoring : INotifyPropertyChanged
    {
        ServiceState? _serviceState;
        private string? _displayName;
        private string? _lastTimeCheking;

        public int Number { get; set; }
        public string? Name { get; set; }
        public ServiceState? ServiceState
        {
            get => _serviceState;
            set
            {
                _serviceState = value;
                NotifyPropertyChanged();
            }
        }
        public string? DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                NotifyPropertyChanged();
            }
        }
        public string? LastTimeCheking
        {
            get => _lastTimeCheking;
            set
            {
                _lastTimeCheking = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
