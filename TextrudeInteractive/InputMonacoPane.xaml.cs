using Engine.Application;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TextrudeInteractive
{
    /// <summary>
    /// Interaction logic for InputMonacoPane.xaml
    /// </summary>
    // TODO InputMonacoPane is extended-copy of InputPane -> maybe use inheritance?
    public partial class InputMonacoPane : UserControl, IPane
    {
		private ModelFormat _format = ModelFormat.Line;
		private MonacoBinding _monacoBinding;

		public InputMonacoPane()
		{
			InitializeComponent();
			_monacoBinding = new MonacoBinding(WebView, isReadOnly: false);
            _monacoBinding.OnUserInput = _ => this.OnUserInput();
			_monacoBinding.Initialize().ConfigureAwait(false);

			FormatSelection.ItemsSource = Enum.GetValues(typeof(ModelFormat));
			FormatSelection.SelectedItem = ModelFormat.Yaml;
            FileBar.OnLoad = NewFileLoaded;
            FileBar.OnSave = () => Text;
        }

        public Action OnUserInput = () => { };

        public string Text
		{
			get => _monacoBinding.Text;
			set => _monacoBinding.Text = value;
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
					_monacoBinding.Format = Format.ToString().ToLower();
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
			Format = (ModelFormat)FormatSelection.SelectedItem;
			OnUserInput();
		}
	}
}
