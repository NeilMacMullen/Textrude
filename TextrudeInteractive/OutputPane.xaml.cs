using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Highlighting;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for EditPane.xaml
    /// </summary>
    public partial class OutputPane : UserControl
    {
        private string _format = string.Empty;
        private string _text = string.Empty;

        public ObservableCollection<string> Highglighting = new ObservableCollection<string>();

        public OutputPane()
        {
            InitializeComponent();

            var definitionNames =
                new[] {"text"}
                    .Concat(HighlightingManager.Instance.HighlightingDefinitions.Select(d => d.Name))
                    .ToArray();

            Highglighting = new ObservableCollection<string>(definitionNames);
            FormatSelection.ItemsSource = Highglighting;
            FormatSelection.SelectedIndex = 0;
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
        public string Format
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

        private void FormatSelection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Format = FormatSelection.SelectedItem as string;
            textBox.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(Format);
        }

        private void OutputPane_OnLoaded(object sender, RoutedEventArgs e)
        {
            //Beware - if this is in a TabItem, this occurs every time a tab is switched
        }
    }
}