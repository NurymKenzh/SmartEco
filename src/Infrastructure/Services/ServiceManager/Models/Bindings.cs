using ServiceManager.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ServiceManager.Models
{
    internal static class Bindings
    {
        public static ObservableCollection<ServiceMonitoring> ServiceMonitorings { get; set; } = [];
        public static ServiceTab ReporterTab { get; set; } = new();
    }

    public class ServiceTab : INotifyPropertyChanged
    {
        private ServiceState _serviceState = new();
        private ObservableCollection<Log> _logs = [];
        private ButtonState _btnState = new();

        public ServiceState ServiceState
        {
            get => _serviceState;
            set
            {
                _serviceState = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<Log> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                NotifyPropertyChanged();
            }
        }
        public ButtonState BtnState
        {
            get => _btnState;
            set
            {
                _btnState = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Log : INotifyPropertyChanged
    {
        private string? _text;
        private ColorType? _textColorType = ColorType.Green;

        public string? Text
        {
            get => _text;
            set
            {
                _text = value;
                NotifyPropertyChanged();
            }
        }
        public ColorType? TextColorType
        {
            get => _textColorType;
            set
            {
                _textColorType = value;
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
