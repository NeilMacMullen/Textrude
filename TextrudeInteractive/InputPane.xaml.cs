using System;
using System.IO;
using System.Windows.Controls;
using Engine.Application;

namespace TextrudeInteractive
{
    public interface IPane
    {
        void Clear();
    }

    /// <summary>
    ///     Interaction logic for InputPane.xaml
    /// </summary>
    public partial class InputPane : UserControl, IPane
    {
        private ModelFormat _format = ModelFormat.Line;


        public Action OnUserInput = () => { };


        public InputPane()
        {
            InitializeComponent();
            FormatSelection.ItemsSource = Enum.GetValues(typeof(ModelFormat));
            FormatSelection.SelectedItem = ModelFormat.Yaml;
            fileBar.OnLoad = NewFileLoaded;
            fileBar.OnSave = () => Text;
        }

        public string Text
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        public string ModelPath
        {
            get =>
                fileBar.PathName;
            set => fileBar.PathName = value;
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

        /// <summary>
        ///     Currently unused - the name of the model
        /// </summary>
        public string ModelName { get; set; } = string.Empty;

        public void Clear()
        {
            ModelName = string.Empty;
            ModelPath = string.Empty;
            Text = string.Empty;
            Format = ModelFormat.Line;
        }

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

        private void TextBox_OnTextChanged(object sender, EventArgs e)
        {
            OnUserInput();
        }

        public void SaveIfLinked() => fileBar.SaveIfLinked();
        public void LoadIfLinked() => fileBar.LoadIfLinked();
    }
}