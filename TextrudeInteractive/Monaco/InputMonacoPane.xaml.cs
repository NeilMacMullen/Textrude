using System;
using System.IO;
using System.Windows.Controls;
using Engine.Application;

namespace TextrudeInteractive
{
    /// <summary>
    ///     Interaction logic for InputMonacoPane.xaml
    /// </summary>
    public partial class InputMonacoPane : IPane
    {
        private ModelFormat _format = ModelFormat.Line;

        public Action OnUserInput = () => { };

        public InputMonacoPane()
        {
            InitializeComponent();

            FormatSelection.ItemsSource = Enum.GetValues(typeof(ModelFormat));
            FormatSelection.SelectedItem = ModelFormat.Yaml;
            FileBar.OnLoad = NewFileLoaded;
            FileBar.OnSave = () => Text;
            MonacoPane.TextChangedEvent = HandleUserInput;
        }

        public string Text
        {
            get => MonacoPane.Text;
            set => MonacoPane.Text = value;
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
                    MonacoPane.Format = ToMonacoFormat(Format);
                    FormatSelection.SelectedItem = _format;
                }
            }
        }

        /// <summary>
        ///     Currently unused - the name of the model
        /// </summary>
        public string ModelName { get; set; } = string.Empty;

        public string ModelPath
        {
            get => FileBar.PathName;
            set => FileBar.PathName = value;
        }

        public void Clear()
        {
            ModelName = string.Empty;
            ModelPath = string.Empty;
            Text = string.Empty;
            Format = ModelFormat.Line;
        }

        private void HandleUserInput()
        {
            OnUserInput();
        }

        private string ToMonacoFormat(ModelFormat format) =>
            format == ModelFormat.Line ? "text" : format.ToString().ToLowerInvariant();

        public void SaveIfLinked() => FileBar.SaveIfLinked();
        public void LoadIfLinked() => FileBar.LoadIfLinked();

        private void NewFileLoaded(string text, bool wasNewFile)
        {
            if (wasNewFile)
                Format = ModelDeserializerFactory.FormatFromExtension(Path.GetExtension(ModelPath));
            Text = text;
        }

        private void FormatSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Format = (ModelFormat) FormatSelection.SelectedItem;
            OnUserInput();
        }
    }
}
