using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TextrudeInteractive.Monaco;

/// <summary>
///     Interaction logic for InputMonacoPane.xaml
/// </summary>
public partial class InputMonacoPane
{
    private bool _isReadOnly;

    private EditPaneViewModel _vm = new();
    public ObservableCollection<string> AvailableFormats = new();

    public Action OnUserInput = () => { };

    public InputMonacoPane()
    {
        InitializeComponent();
        FormatSelection.ItemsSource = AvailableFormats;
        FileBar.OnLoad = NewFileLoaded;
        FileBar.ObtainText = () => _vm.Text;
        MonacoPane.TextChangedEvent = HandleUserInput;
        DataContext = new EditPaneViewModel();
        DataContextChanged += OnDataContextChanged;
        FormatSelection.SelectionChanged += FormatSelectionChanged;
    }


    private void SetAvailableFormats(IEnumerable<string> formats)
    {
        AvailableFormats.Clear();
        foreach (var format in formats)
        {
            AvailableFormats.Add(format);
            FormatSelection.Visibility = AvailableFormats.Count() > 1
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void HandleUserInput()
    {
        ReadToContext();
        if (!_isReadOnly)
        {
            OnUserInput();
        }
    }


    private void NewFileLoaded(string path, string text)
    {
        //  if (wasNewFile)
        //          Format = ModelDeserializerFactory.FormatFromExtension(Path.GetExtension(LinkedPath));

        _vm.LinkedPath = path;
        _vm.Text = text;
        HandleUserInput();
    }


    private void FormatSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _vm.Format = (string)FormatSelection.SelectedItem ?? _vm.Format;
        HandleUserInput();
    }

    #region configuration

    public void SetDirection(PaneType type)
    {
        _isReadOnly = type == PaneType.Output;

        MonacoPane.SetReadOnly(_isReadOnly);
    }

    #endregion

    #region vm

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        //slightly hacky way of just getting a kick whenever any property of any of the
        //DataContext properties change
        _vm.PropertyChanged -= VmOnPropertyChanged;
        if (DataContext is EditPaneViewModel vm)
            _vm = vm;
        else _vm = new EditPaneViewModel();
        _busy++;
        SetAvailableFormats(_vm.AvailableFormats);
        SetFromContext();
        _vm.PropertyChanged += VmOnPropertyChanged;
        _busy--;
    }

    private void VmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        SetFromContext();
    }

    private int _busy;

    private void SetFromContext()
    {
        _busy++;
        FormatSelection.SelectedItem = _vm.Format;
        MonacoPane.Format = _vm.Format;
        MonacoPane.Text = _vm.Text;
        FileBar.PathName = _vm.LinkedPath;
        FileBar.SetSaveOnly(_vm.FileLinkage);
        _busy--;
    }

    private void ReadToContext()
    {
        if (_busy != 0)
            return;
        _vm.Text = MonacoPane.Text;
        _vm.Format = MonacoPane.Format;
        _vm.LinkedPath = FileBar.PathName;
        //_vm.ScribanName = ScribanName;
    }

    #endregion
}
