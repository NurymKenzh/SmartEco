using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ServiceManager.Models
{
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
}
