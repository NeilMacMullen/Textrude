using System;
using System.Windows.Controls;
using Engine.Application;

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
    }
}