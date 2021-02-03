using System.ComponentModel;
using System.Runtime.CompilerServices;
using TextrudeInteractive.Annotations;

namespace TextrudeInteractive
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _lineNumbers = true;
        private string _templateText;
        private double _textSize = 14;

        private bool _wordWrap;

        public bool LineNumbers
        {
            get => _lineNumbers;
            set
            {
                _lineNumbers = value;
                OnPropertyChanged();
            }
        }


        public double TextSize
        {
            get => _textSize;
            set
            {
                _textSize = value;
                OnPropertyChanged();
            }
        }

        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                _wordWrap = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
