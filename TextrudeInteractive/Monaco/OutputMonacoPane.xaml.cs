using System.Windows;
using System.Windows.Controls;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for OutputMonacoPane.xaml
    /// </summary>
    // TODO OutputMonacoPane is extended-copy of OutputPane -> maybe use inheritance?
    public partial class OutputMonacoPane : UserControl, IPane
    {
        private const string DefaultFormat = "text";

        private string _format = string.Empty;


        public OutputMonacoPane()
        {
            InitializeComponent();


            var formats = new MonacoResourceFetcher().GetSupportedFormats();
            FormatSelection.ItemsSource = formats;
            FormatSelection.SelectedIndex = formats.IndexOf(DefaultFormat);

            FileBar.OnSave = () => Text;
        }

        public string Text
        {
            get => MonacoPane.Text;
            set => MonacoPane.Text = value;
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
                    //_monacoBinding.Format = Format;
                    FormatSelection.SelectedItem = _format;
                }
            }
        }

        /// <summary>
        ///     Currently unused - the name of the output
        /// </summary>
        public string OutputName { get; set; } = string.Empty;

        /// <summary>
        ///     Path to file that the output is connected to
        /// </summary>
        public string OutputPath
        {
            get => FileBar.PathName;
            set => FileBar.PathName = value;
        }

        public void Clear()
        {
            Text = string.Empty;
            OutputPath = string.Empty;
            Format = DefaultFormat;
        }

        public void SaveIfLinked() => FileBar.SaveIfLinked();

        private void FormatSelection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Format = FormatSelection.SelectedItem as string;
        }

        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            var maxAttempts = 3;
            for (var i = 0; i < maxAttempts; i++)
            {
                try
                {
                    Clipboard.SetText(Text);
                    return;
                }
                catch
                {
                }
            }
        }
    }
}
