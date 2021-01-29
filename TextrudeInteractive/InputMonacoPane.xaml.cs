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
	public partial class InputMonacoPane : UserControl
	{
		private ModelFormat _format = ModelFormat.Line;
		private MonacoBinding _monacoBinding;

		public Action OnUserInput = () => { };

		public InputMonacoPane()
		{
			InitializeComponent();
			_monacoBinding = new MonacoBinding(WebView, isReadOnly: false, _ => this.OnUserInput());
			_monacoBinding.Initialize().ConfigureAwait(false);

			FormatSelection.ItemsSource = Enum.GetValues(typeof(ModelFormat));
			FormatSelection.SelectedItem = ModelFormat.Yaml;
		}

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

		private void FormatSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Format = (ModelFormat)FormatSelection.SelectedItem;
			OnUserInput();
		}

		private void TextBox_OnTextChanged(object? sender, EventArgs e)
		{
			OnUserInput();
		}

		private void LoadFromFile(object sender, RoutedEventArgs e)
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
