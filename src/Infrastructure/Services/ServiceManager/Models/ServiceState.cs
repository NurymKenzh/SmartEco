using ServiceManager.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ServiceManager.Models
{
    public class ServiceState : INotifyPropertyChanged
    {
        private ServiceStatus? _status;
        private ColorType? _lampColor;

        public ServiceStatus? Status
        {
            get => _status;
            set
            {
                _status = value;
                NotifyPropertyChanged();
            }
        }

        public ColorType? LampColorType
        {
            get => _lampColor;
            set
            {
                _lampColor = value;
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
