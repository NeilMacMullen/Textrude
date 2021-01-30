﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Highlighting;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for EditPane.xaml
    /// </summary>
    public partial class OutputPane : UserControl, IPane
    {
        private const string DefaultFormat = "text";
        private string _format = string.Empty;

        private string _text = string.Empty;

        public ObservableCollection<string> Highglighting = new ObservableCollection<string>();

        public OutputPane()
        {
            InitializeComponent();

            var definitionNames =
                new[] {DefaultFormat}
                    .Concat(HighlightingManager.Instance.HighlightingDefinitions.Select(d => d.Name))
                    .ToArray();

            Highglighting = new ObservableCollection<string>(definitionNames);
            FormatSelection.ItemsSource = Highglighting;
            FormatSelection.SelectedIndex = 0;
            fileBar.OnSave = () => Text;
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

        /// <summary>
        ///     Currently unused - the name of the output
        /// </summary>
        public string OutputName { get; set; } = string.Empty;

        /// <summary>
        ///     Path to file that the output is connected to
        /// </summary>
        public string OutputPath
        {
            get => fileBar.PathName;
            set => fileBar.PathName = value;
        }

        public void Clear()
        {
            Text = string.Empty;
            OutputPath = string.Empty;
            Format = DefaultFormat;
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

        public void SaveIfLinked() => fileBar.SaveIfLinked();

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