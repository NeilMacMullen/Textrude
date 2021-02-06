using System;
using System.Windows;
using System.Windows.Controls;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for OutputMonacoPane.xaml
    /// </summary>
    public partial class OutputMonacoPane : UserControl, IPane
    {
        private const string DefaultFormat = "text";

        public Action OnUserInput = () => { };

        public OutputMonacoPane()
        {
            InitializeComponent();


            var formats = new MonacoResourceFetcher().GetSupportedFormats();
            FormatSelection.ItemsSource = formats;
            FormatSelection.SelectedIndex = formats.IndexOf(DefaultFormat);
            FileBar.OnSave = () => Text;
            MonacoPane.SetReadOnly(true);
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
            get => MonacoPane.Format;
            set
            {
                if (MonacoPane.Format != value)
                {
                    FormatSelection.SelectedItem = value;
                }
            }
        }

        /// <summary>
        ///     Currently unused - the name of the output
        /// </summary>
        public string ScribanName { get; set; } = string.Empty;

        /// <summary>
        ///     Path to file that the content is connected to
        /// </summary>
        public string LinkedPath
        {
            get => FileBar.PathName;
            set => FileBar.PathName = value;
        }

        public void Clear()
        {
            ScribanName = string.Empty;
            LinkedPath = string.Empty;
            Text = string.Empty;
            Format = DefaultFormat;
        }

        private void HandleUserInput()
        {
            OnUserInput();
        }

        public void SaveIfLinked() => FileBar.SaveIfLinked();
        public void LoadIfLinked() => FileBar.LoadIfLinked();

        private void NewFileLoaded(string text, bool wasNewFile)
        {
            //  if (wasNewFile)
            //     Format = ModelDeserializerFactory.FormatFromExtension(Path.GetExtension(LinkedPath));
            Text = text;
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
