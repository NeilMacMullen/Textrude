using System.ComponentModel;
using System.Runtime.CompilerServices;
using TextrudeInteractive.Properties;

namespace TextrudeInteractive;

public class MonacoVisualSettings : INotifyPropertyChanged
{
    private bool _isBusy;
    private bool _lineNumbers = true;
    private bool _showWhitespace;

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

    public bool ShowWhitespace
    {
        get => _showWhitespace;
        set
        {
            if (value == _showWhitespace) return;
            _showWhitespace = value;
            OnPropertyChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (value == _isBusy) return;
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
