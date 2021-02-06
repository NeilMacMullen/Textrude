using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for OutputMonacoPane.xaml
    /// </summary>
    public partial class OutputMonacoPane : UserControl, IPane
    {
        public ObservableCollection<string> AvailableFormats = new ObservableCollection<string>();

        public Action OnUserInput = () => { };

        public OutputMonacoPane()
        {
            InitializeComponent();


            FormatSelection.ItemsSource = AvailableFormats;
            FileBar.OnLoad = NewFileLoaded;
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
        ///     Currently unused - the name of the model/output
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
            Format = string.Empty;
        }

        public void SetAvailableFormats(IEnumerable<string> formats)
        {
            AvailableFormats.Clear();
            foreach (var format in formats)
            {
                AvailableFormats.Add(format);
            }

            FormatSelection.SelectedIndex = 0;
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

        private void FormatSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Format = //(ModelFormat)
                (string) FormatSelection.SelectedItem ?? string.Empty;
            OnUserInput();
        }
    }
}
