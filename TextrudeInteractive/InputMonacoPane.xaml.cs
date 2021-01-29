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
		private string _text = "";
		private ModelFormat _format = ModelFormat.Line;

		public Action OnUserInput = () => { };

		public InputMonacoPane()
		{
			InitializeComponent();
			FormatSelection.ItemsSource = Enum.GetValues(typeof(ModelFormat));
			FormatSelection.SelectedItem = ModelFormat.Yaml;
		}

		public string Text
		{
			get => _text;
			set => _text = value;
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
