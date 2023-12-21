using ServiceManager.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ServiceManager.Models
{
    public class ButtonState : INotifyPropertyChanged
    {
        private bool _isEnabledUpdate = true;
        private bool _isEnabledCreate;
        private bool _isEnabledRemove;
        private bool _isEnabledStart;
        private bool _isEnabledStop;

        public bool IsEnabledUpdate
        {
            get => _isEnabledUpdate;
            set
            {
                _isEnabledUpdate = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsEnabledCreate
        {
            get => _isEnabledCreate;
            set
            {
                _isEnabledCreate = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsEnabledRemove
        {
            get => _isEnabledRemove;
            set
            {
                _isEnabledRemove = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsEnabledStart
        {
            get => _isEnabledStart;
            set
            {
                _isEnabledStart = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsEnabledStop
        {
            get => _isEnabledStop;
            set
            {
                _isEnabledStop = value;
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
