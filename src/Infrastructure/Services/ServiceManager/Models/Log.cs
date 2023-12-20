using ServiceManager.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ServiceManager.Models
{
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
