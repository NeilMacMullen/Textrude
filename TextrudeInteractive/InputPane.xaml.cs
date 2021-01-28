using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Engine.Application;
using Microsoft.Win32;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for InputPane.xaml
    /// </summary>
    public partial class InputPane : UserControl
    {
        private ModelFormat _format = ModelFormat.Line;
        private string _text = string.Empty;

        public Action OnUserInput = () => { };


        public InputPane()
        {
            InitializeComponent();
            FormatSelection.ItemsSource = Enum.GetValues(typeof(ModelFormat));
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                SetText(_text);
            }
        }

        /// <summary>
        ///     Serialisable Format to be persisted in project
        /// </summary>
        public ModelFormat Format
        {
            get => _format;
            set
            {
                if (_format != value)
                {
                    _format = value;
                    FormatSelection.SelectedItem = _format;
                }
            }
        }


        private void SetText(string str)
        {
            textBox.Text = str;
        }

        private void FormatSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Format = (ModelFormat) FormatSelection.SelectedItem;
            OnUserInput();
        }

        private void TextBox_OnTextChanged(object? sender, EventArgs e)
        {
            OnUserInput();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter =
                "csv files (*.csv)|*.csv|" +
                "yaml files (*.yaml)|*.yaml|" +
                "json files (*.json)|*.json|" +
                "txt files (*.txt)|*.txt|" +
                "All files (*.*)|*.*";
            if (dlg.ShowDialog() != true) return;
            try
            {
                Text = File.ReadAllText(dlg.FileName);
                Format = ModelDeserializerFactory.FormatFromExtension(Path.GetExtension(dlg.FileName));
            }
            catch
            {
            }
        }
    }
}